// 宿舍 · 智慧树查寝: now and then (15 % per visit, at most 2 quizzes a day) the wise-tree girl drops by the room
// with a "?" over her head. Tap her for 3 questions: fixed trivia from the group, "who is this girl?" (only girls the
// player has unlocked) and sun costs read from the game itself. Each right answer pays coins; the running total of
// right answers is kept for the statue reward.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	private sealed class QuizQ
	{
		public string Id;

		public string Text;

		public string[] Opts;

		public int Answer;

		/// <summary>Girl portrait shown with the question (face/&lt;skel&gt;.png), or null.</summary>
		public string Face;
	}

	/// <summary>Fixed questions (from the group). The first option is the right one; options are shuffled when asked.</summary>
	private static readonly (string id, string text, string[] opts)[] FixedQuestions =
	{
		("bili", "庄哥 B 站的昵称是什么？", new[] { "庄不纯或庄特纯", "庄不凡或庄特别纯", "庄纯纯或庄不正经", "庄特不纯或庄纯特" }),
		("art", "每个植物娘有几个官方立绘？", new[] { "2 个", "1 个", "3 个", "4 个" }),
		("comic", "植物娘第一章漫画的主角是谁？", new[] { "火爆辣椒", "胆小菇", "向日葵", "大嘴花" }),
		("thorn", "荆棘族的仇人是谁？", new[] { "赞博尼", "戴夫", "雪橇僵尸", "舞王僵尸" }),
		("balloon", "大嘴花在免费气球日的气球卖多少钱？", new[] { "一元一个", "五元一个", "十元一个", "免费赠送" }),
		("download", "正版植物娘游戏在哪里下载？", new[] { "夸克网盘", "百度网盘", "迅雷网盘", "蓝奏云" })
	};

	private const float QuizChance = 0.15f;

	private const int QuizPerDay = 2;

	private const int QuizLength = 3;

	/// <summary>Coins per right answer (shown × 10: 10 coins = $100). Placeholder until the rewards are decided.</summary>
	private const int QuizCoinsPerRight = 10;

	private const int StatueGoal = 10;

	private static string QuizPath => Path.Combine(DormPlatform.SaveDir, "dorm_quiz.txt");

	private string mQuizDay = "";

	private int mQuizToday;

	private int mQuizTotalRight;

	private readonly HashSet<string> mQuizSeen = new HashSet<string>();

	/// <summary>The visitor is in the room (cell), waiting to be tapped.</summary>
	private bool mVisitor;

	private float mVisitorI, mVisitorJ, mVisitorT;

	private TRect mVisitorRect;

	/// <summary>Quiz panel open.</summary>
	private bool mQuiz;

	private List<QuizQ> mQs;

	private int mQIdx;

	/// <summary>-1 = not answered yet.</summary>
	private int mPicked = -1;

	private int mRight;

	private bool mQuizDone;

	private float mQuizT;

	private TRect mQBoard, mQClose, mQNext;

	private readonly TRect[] mQOpt = new TRect[4];

	// ------------------------------------------------------------------ saves / trigger

	private void LoadQuiz()
	{
		try
		{
			if (!File.Exists(QuizPath))
			{
				return;
			}
			foreach (string line in File.ReadAllLines(QuizPath))
			{
				int eq = line.IndexOf('=');
				if (eq <= 0)
				{
					continue;
				}
				string k = line.Substring(0, eq), v = line.Substring(eq + 1);
				switch (k)
				{
				case "day":
					mQuizDay = v;
					break;
				case "today":
					int.TryParse(v, out mQuizToday);
					break;
				case "right":
					int.TryParse(v, out mQuizTotalRight);
					break;
				case "seen":
					foreach (string s in v.Split(',', StringSplitOptions.RemoveEmptyEntries))
					{
						mQuizSeen.Add(s);
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm quiz load: " + ex.Message);
		}
	}

	private void SaveQuiz()
	{
		try
		{
			Directory.CreateDirectory(DormPlatform.SaveDir);
			File.WriteAllLines(QuizPath, new[] { "day=" + mQuizDay, "today=" + mQuizToday, "right=" + mQuizTotalRight, "seen=" + string.Join(",", mQuizSeen) });
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm quiz save: " + ex.Message);
		}
	}

	private static string Today => DateTime.Now.ToString("yyyy-MM-dd");

	/// <summary>Called when the dorm opens: maybe the wise tree comes by.</summary>
	private void MaybeStartVisit(bool force = false)
	{
		LoadQuiz();
		if (mQuizDay != Today)
		{
			mQuizDay = Today;
			mQuizToday = 0;
		}
		if (!force && (mQuizToday >= QuizPerDay || mRand.NextDouble() >= QuizChance))
		{
			return;
		}
		Point p = NearestFreeCell(4.5f, 5.5f, null);
		mVisitorI = p.X + 0.5f;
		mVisitorJ = p.Y + 0.5f;
		mVisitorT = 0f;
		mVisitor = true;
		Hint("智慧树来查寝了！点她答题");
	}

	// ------------------------------------------------------------------ questions

	private List<QuizQ> MakeQuestions()
	{
		List<QuizQ> list = new List<QuizQ>();
		List<GirlDef> unlocked = UnlockedGirls.ToList();
		List<(string id, string text, string[] opts)> fixedLeft = FixedQuestions.Where((q) => !mQuizSeen.Contains(q.id)).ToList();
		if (fixedLeft.Count == 0)
		{
			// every fixed question has been asked once: start the round again
			foreach (var q in FixedQuestions)
			{
				mQuizSeen.Remove(q.id);
			}
			fixedLeft = FixedQuestions.ToList();
		}
		int guard = 0;
		while (list.Count < QuizLength && guard++ < 50)
		{
			int kind = mRand.Next(3);
			QuizQ q = null;
			if (kind == 0 && fixedLeft.Count > 0)
			{
				var f = fixedLeft[mRand.Next(fixedLeft.Count)];
				fixedLeft.Remove(f);
				q = Shuffled(f.id, f.text, f.opts, null);
			}
			else if (kind == 1 && unlocked.Count > 0)
			{
				GirlDef girl = unlocked[mRand.Next(unlocked.Count)];
				List<string> pool = (unlocked.Count >= 4 ? unlocked : Girls.ToList()).Where((GirlDef x) => x != girl).Select((GirlDef x) => x.Name).OrderBy((string _) => mRand.Next()).Take(3).ToList();
				q = Shuffled("face_" + girl.Skel, "这位植物娘是谁？", new[] { girl.Name }.Concat(pool).ToArray(), girl.Skel);
			}
			else if (kind == 2 && unlocked.Count > 0)
			{
				GirlDef girl = unlocked[mRand.Next(unlocked.Count)];
				int cost = DormPlatform.SunCost(girl.Seed);
				if (cost <= 0)
				{
					continue;
				}
				List<int> wrong = new[] { -75, -50, -25, 25, 50, 75, 100 }.Select((int d) => cost + d).Where((int c) => c > 0).OrderBy((int _) => mRand.Next()).Take(3).ToList();
				q = Shuffled("sun_" + girl.Skel, girl.Name + "需要多少阳光？", new[] { cost }.Concat(wrong).Select((int c) => c + " 阳光").ToArray(), null);
			}
			if (q != null && list.All((QuizQ o) => o.Id != q.Id))
			{
				list.Add(q);
			}
		}
		return list;
	}

	private QuizQ Shuffled(string id, string text, string[] opts, string face)
	{
		int[] order = Enumerable.Range(0, opts.Length).OrderBy((int _) => mRand.Next()).ToArray();
		return new QuizQ
		{
			Id = id,
			Text = text,
			Opts = order.Select((int k) => opts[k]).ToArray(),
			Answer = Array.IndexOf(order, 0),
			Face = face
		};
	}

	private void OpenQuiz()
	{
		mQs = MakeQuestions();
		if (mQs.Count == 0)
		{
			Hint("智慧树：今天先不考你啦");
			mVisitor = false;
			return;
		}
		mQuiz = true;
		mQIdx = 0;
		mPicked = -1;
		mRight = 0;
		mQuizDone = false;
		mQuizT = 0f;
		DormPlatform.PlaySound(Resources.SOUND_TAP);
	}

	private void Answer(int k)
	{
		if (mPicked >= 0 || mQuizDone)
		{
			return;
		}
		QuizQ q = mQs[mQIdx];
		mPicked = k;
		mQuizT = 0f;
		if (q.Id != null && FixedQuestions.Any((f) => f.id == q.Id))
		{
			mQuizSeen.Add(q.Id);
		}
		if (k == q.Answer)
		{
			mRight++;
			mQuizTotalRight++;
			DormPlatform.PlaySound(Resources.SOUND_COIN);
		}
		else
		{
			DormPlatform.PlaySound(Resources.SOUND_TAP);
		}
	}

	private void NextQuestion()
	{
		if (mPicked < 0)
		{
			return;
		}
		if (mQIdx + 1 < mQs.Count)
		{
			mQIdx++;
			mPicked = -1;
			mQuizT = 0f;
			return;
		}
		// finished: pay out, count the quiz, the visitor leaves
		mQuizDone = true;
		mQuizT = 0f;
		mQuizToday++;
		if (mRight > 0)
		{
			DormPlatform.AddCoins(mLawn, mRight * QuizCoinsPerRight);
		}
		SaveQuiz();
	}

	private void CloseQuiz()
	{
		if (!mQuizDone && mPicked >= 0)
		{
			// leaving half-way still counts what was answered
			mQuizToday++;
			if (mRight > 0)
			{
				DormPlatform.AddCoins(mLawn, mRight * QuizCoinsPerRight);
			}
			SaveQuiz();
			mVisitor = false;
		}
		else if (mQuizDone)
		{
			mVisitor = false;
			Hint("智慧树回去了 · 累计答对 " + mQuizTotalRight + " 题");
		}
		mQuiz = false;
	}

	// ------------------------------------------------------------------ room

	private void UpdateVisitor(float dt)
	{
		if (mVisitor || mQuiz)
		{
			mVisitorT += dt;
			mQuizT += dt;
		}
	}

	/// <summary>The visitor standing in the room, depth-sorted with the furniture and the girls.</summary>
	private void AddVisitorDraw(Graphics g, List<(float key, int order, Action draw)> list)
	{
		if (!mVisitor || mScreen != Screen.Room)
		{
			return;
		}
		list.Add((mVisitorI + mVisitorJ, 1, () => DrawVisitor(g)));
	}

	private void DrawVisitor(Graphics g)
	{
		Image img = Img("tree/stand");
		if (img == null)
		{
			return;
		}
		float h = mCW * 1.5f;
		float breathe = 1f + 0.015f * (float)Math.Sin(mVisitorT * 2.4f);
		float w = img.mWidth * h / img.mHeight;
		float fx = SX(mVisitorI, mVisitorJ), fy = SY(mVisitorI, mVisitorJ);
		// arrival: a little pop
		float pop = Math.Min(1f, mVisitorT * 3f);
		float sh = h * breathe * (0.6f + 0.4f * pop);
		float sw = w * (1.2f - 0.2f * pop);
		DrawSprite(g, "dust", fx - mCW * 0.3f, fy - mCW * 0.1f, mCW * 0.6f, mCW * 0.2f, new Color(0, 0, 0, 90));
		DrawSprite(g, "tree/stand", fx - sw / 2f, fy - sh, sw, sh);
		mVisitorRect = new TRect((int)(fx - sw / 2f), (int)(fy - sh), (int)sw, (int)sh);
		if (!mQuiz)
		{
			float b = mCW * 0.55f * (1f + 0.06f * (float)Math.Sin(mVisitorT * 5f));
			float by = fy - sh - b * 0.85f + (float)Math.Sin(mVisitorT * 3f) * mCW * 0.05f;
			DrawSprite(g, "tree/qbubble", fx - b * 0.2f, by, b, b * 230f / 268f);
		}
	}

	private bool HitVisitor(int x, int y) => mVisitor && !mQuiz && mScreen == Screen.Room && !mEdit && In(mVisitorRect, x, y);

	// ------------------------------------------------------------------ panel

	private void LayoutQuiz()
	{
		int bustW = (int)(mHeight * 0.42f);
		int boardW = Math.Min(560, mWidth - bustW - 60);
		int left = (mWidth - bustW - boardW) / 2 + bustW;
		int top = (int)(mHeight * 0.16f);
		int boardH = (int)(mHeight * 0.36f);
		mQBoard = new TRect(left, top, boardW, boardH);
		mQClose = new TRect(left + boardW - 30, top - 14, 44, 44);
		int ow = (boardW - 12) / 2, oh = Math.Max(40, (int)(mHeight * 0.095f));
		int oy = top + boardH + 10;
		for (int k = 0; k < 4; k++)
		{
			mQOpt[k] = new TRect(left + (k % 2) * (ow + 12), oy + (k / 2) * (oh + 8), ow, oh);
		}
		int nw = 180, nh = 52;
		mQNext = new TRect(left + (boardW - nw) / 2, oy + 2 * (oh + 8) + 4, nw, nh);
	}

	private bool QuizClick(int x, int y)
	{
		LayoutQuiz();
		if (In(mQClose, x, y))
		{
			CloseQuiz();
			return true;
		}
		if (mQuizDone)
		{
			if (In(mQNext, x, y))
			{
				CloseQuiz();
			}
			return true;
		}
		if (mPicked < 0)
		{
			for (int k = 0; k < 4; k++)
			{
				if (In(mQOpt[k], x, y) && k < mQs[mQIdx].Opts.Length)
				{
					Answer(k);
					return true;
				}
			}
		}
		else if (In(mQNext, x, y))
		{
			NextQuestion();
		}
		return true;
	}

	private void DrawQuiz(Graphics g)
	{
		LayoutQuiz();
		g.SetColor(new Color(30, 22, 12, 150));
		g.FillRect(0, 0, mWidth, mHeight);
		QuizQ q = mQuizDone ? null : mQs[mQIdx];
		bool answered = mPicked >= 0;
		bool right = answered && q != null && mPicked == q.Answer;
		// the wise tree, left of the board
		string bust = mQuizDone ? (mRight >= 2 ? "tree/bust_laugh" : "tree/bust_smile")
			: !answered ? ((mQuizT < 1.6f && (int)(mQuizT * 6f) % 2 == 0) ? "tree/bust_talk" : "tree/bust_smile")
			: right ? "tree/bust_laugh" : "tree/bust_think";
		Image bi = Img(bust);
		float bh = mHeight * 0.5f;
		if (bi != null)
		{
			float bw = bi.mWidth * bh / bi.mHeight;
			float bx = mQBoard.mX - bw + 10;
			float by = mQOpt[2].mY + mQOpt[2].mHeight - bh;
			DrawSprite(g, bust, bx, by, bw, bh);
			// what she says
			string say = mQuizDone ? (mRight == mQs.Count ? "全对！真厉害～" : mRight > 0 ? "答对 " + mRight + " 题，不错哦" : "下次加油吧～")
				: !answered ? "第 " + (mQIdx + 1) + " 题来啦" : right ? "答对了！" : "是「" + q.Opts[q.Answer] + "」哦";
			int sw = Math.Max(150, mFont.StringWidth(say) + 40), shh = 62;
			TRect sr = new TRect((int)(bx + bw * 0.5f - sw * 0.55f), (int)(by - shh + 8), sw, shh);
			if (sr.mX < 6)
			{
				sr.mX = 6;
			}
			DrawSprite(g, "tree/speech", sr.mX, sr.mY, sr.mWidth, sr.mHeight);
			TextCentered(g, mFont, say, new TRect(sr.mX, sr.mY, sr.mWidth, sr.mHeight - 14), InkBrown);
		}
		// board + title
		DrawSprite(g, "tree/board", mQBoard.mX, mQBoard.mY, mQBoard.mWidth, mQBoard.mHeight);
		string title = "智慧树查寝";
		int tw = mFont.StringWidth(title) + 70;
		TRect tr = new TRect(mQBoard.mX + (mQBoard.mWidth - tw) / 2, mQBoard.mY - 30, tw, 44);
		DrawSprite(g, "tree/leaves_l", tr.mX - 64, tr.mY - 4, 76, 43);
		DrawSprite(g, "tree/leaves_r", tr.mX + tr.mWidth - 12, tr.mY - 4, 76, 43);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
		DrawSprite(g, "tree/close", mQClose.mX, mQClose.mY, mQClose.mWidth, mQClose.mHeight);
		TRect inner = new TRect(mQBoard.mX + mQBoard.mWidth / 14, mQBoard.mY + mQBoard.mHeight / 9, mQBoard.mWidth * 12 / 14, mQBoard.mHeight * 7 / 9);
		if (mQuizDone)
		{
			DrawQuizResult(g, inner);
			return;
		}
		// "第 n/3 题"
		string num = "第 " + (mQIdx + 1) + " / " + mQs.Count + " 题";
		int pw = mFont.StringWidth(num) + 34;
		TRect pr = new TRect(inner.mX, inner.mY + 2, pw, 34);
		DrawSprite(g, "tree/pill", pr.mX, pr.mY, pr.mWidth, pr.mHeight);
		TextCentered(g, mFont, num, pr, Color.White, -1);
		int textX = inner.mX, textW = inner.mWidth;
		if (q.Face != null && Img("face/" + q.Face) != null)
		{
			int fs = Math.Min(inner.mHeight - 40, 110);
			DrawSprite(g, "face/" + q.Face, inner.mX + inner.mWidth - fs, inner.mY + 40, fs, fs);
			textW -= fs + 10;
		}
		int ty = pr.mY + pr.mHeight + 12;
		foreach (string line in Wrap(q.Text, mFont, textW))
		{
			Text(g, mFont, line, textX, ty, InkBrown);
			ty += mFont.GetHeight() + 4;
		}
		// options
		string[] opt = { "tree/opt_a", "tree/opt_b", "tree/opt_c", "tree/opt_d" };
		for (int k = 0; k < q.Opts.Length && k < 4; k++)
		{
			TRect r = mQOpt[k];
			Color? tint = null;
			if (answered)
			{
				tint = k == q.Answer ? new Color(190, 255, 170) : k == mPicked ? new Color(255, 160, 150) : new Color(200, 200, 200);
			}
			DrawSprite(g, opt[k], r.mX, r.mY, r.mWidth, r.mHeight, tint);
			string s = q.Opts[k];
			int maxW = r.mWidth - r.mHeight - 30;
			while (s.Length > 1 && mFont.StringWidth(s) > maxW)
			{
				s = s.Substring(0, s.Length - 1);
			}
			Text(g, mFont, s, r.mX + r.mHeight + 4, r.mY + (r.mHeight - mFont.GetHeight()) / 2 - 1, InkBrown);
		}
		if (answered)
		{
			DrawSprite(g, "tree/button", mQNext.mX, mQNext.mY, mQNext.mWidth, mQNext.mHeight);
			TextCentered(g, mFont, mQIdx + 1 < mQs.Count ? "下一题" : "看结果", mQNext, InkBrown, -2);
		}
	}

	private void DrawQuizResult(Graphics g, TRect inner)
	{
		TextCentered(g, mFont, "答对 " + mRight + " / " + mQs.Count + " 题", new TRect(inner.mX, inner.mY, inner.mWidth, 40), InkBrown);
		// reward card
		int rw = Math.Min(260, inner.mWidth - 20), rh = 92;
		TRect rr = new TRect(inner.mX + (inner.mWidth - rw) / 2, inner.mY + 44, rw, rh);
		DrawSprite(g, "tree/reward", rr.mX, rr.mY, rr.mWidth, rr.mHeight);
		int lw = mFont.StringWidth("答对奖励") + 30;
		TextCentered(g, mFont, "答对奖励", new TRect(rr.mX + (rr.mWidth - lw) / 2, rr.mY, lw, (int)(rr.mHeight * 0.28f)), Color.White, -1);
		string amount = mRight > 0 ? "+" + LawnApp.GetMoneyString(mRight * QuizCoinsPerRight) : "这次没有奖励";
		int iw = 38;
		int total = (mRight > 0 ? iw + 8 : 0) + mFont.StringWidth(amount);
		int ax = rr.mX + (rr.mWidth - total) / 2, ay = rr.mY + (int)(rr.mHeight * 0.42f);
		if (mRight > 0)
		{
			DrawSprite(g, "tree/coin", ax, ay, iw, iw);
			ax += iw + 8;
		}
		Text(g, mFont, amount, ax, ay + (iw - mFont.GetHeight()) / 2, new Color(170, 110, 20));
		string prog = mQuizTotalRight >= StatueGoal ? "累计答对 " + mQuizTotalRight + " 题 · 铁粉雕像制作中，敬请期待" : "累计答对 " + mQuizTotalRight + " / " + StatueGoal + " 题 · 满 " + StatueGoal + " 题送植物娘铁粉雕像";
		// running total under the board, where the options were
		List<string> lines = Wrap(prog, mFont, mQBoard.mWidth - 40);
		int lh = mFont.GetHeight() + 4;
		TRect pb = new TRect(mQBoard.mX, mQOpt[0].mY + 4, mQBoard.mWidth, lines.Count * lh + 20);
		Draw9(g, "ui/panel", pb, 22);
		int py = pb.mY + 10;
		foreach (string line in lines)
		{
			TextCentered(g, mFont, line, new TRect(pb.mX, py, pb.mWidth, lh), InkBrown);
			py += lh;
		}
		DrawSprite(g, "tree/button", mQNext.mX, mQNext.mY, mQNext.mWidth, mQNext.mHeight);
		TextCentered(g, mFont, "谢谢智慧树", mQNext, InkBrown, -2);
	}

	private static List<string> Wrap(string s, Font f, int width)
	{
		List<string> lines = new List<string>();
		string cur = "";
		foreach (char c in s)
		{
			if (cur.Length > 0 && f.StringWidth(cur + c) > width)
			{
				lines.Add(cur);
				cur = "";
			}
			cur += c;
		}
		if (cur.Length > 0)
		{
			lines.Add(cur);
		}
		return lines;
	}

#if !__IOS__
	internal void TestVisit() => MaybeStartVisit(force: true);

	internal void TestOpenQuiz()
	{
		if (mVisitor)
		{
			OpenQuiz();
		}
	}

	internal void TestAnswer(bool right)
	{
		if (mQuiz && !mQuizDone)
		{
			if (mPicked < 0)
			{
				Answer(right ? mQs[mQIdx].Answer : (mQs[mQIdx].Answer + 1) % mQs[mQIdx].Opts.Length);
			}
		}
	}

	internal void TestNext() => NextQuestion();

	internal void TestCloseQuiz() => CloseQuiz();
#endif
}
