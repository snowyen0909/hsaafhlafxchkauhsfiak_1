// 宿舍 测试选单: tap the 植物娘宿舍 title 5 times (within 3 seconds) in a build with DormPlatform.TestTools.
// For testing on a phone: make the wise tree come on every visit or right now, add coins, reset today's quiz count,
// unlock both sunflower trophies, enlarge the room for free.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	private TRect mTitleRect;

	private int mTitleTaps;

	private float mTitleTapTime = -10f;

	private bool mTestMenu;

	/// <summary>Testing: the wise tree comes on every visit.</summary>
	private bool mTreeAlways;

	/// <summary>Testing: both sunflower trophies count as earned.</summary>
	private bool mTestTrophies;

	private readonly List<(TRect r, string id)> mTestButtons = new List<(TRect, string)>();

	private TRect mTestPanel;

	/// <summary>A tap on the title: the fifth one in a row opens the menu. True when the tap was used.</summary>
	private bool TitleTap(int x, int y)
	{
		if (!DormPlatform.TestTools || !In(mTitleRect, x, y) || mQuiz || mBuy != null || mSell != null || mRecallAsk || mConfirm)
		{
			return false;
		}
		if (mTime - mTitleTapTime > 3f)
		{
			mTitleTaps = 0;
		}
		mTitleTaps++;
		mTitleTapTime = mTime;
		if (mTitleTaps >= 5)
		{
			mTitleTaps = 0;
			mTestMenu = true;
			DormPlatform.PlaySound(Resources.SOUND_TAP);
		}
		return true;
	}

	private void LayoutTestMenu()
	{
		int w = Math.Min(420, mWidth - 40), bh = 46, gap = 8;
		string[] ids = { "always", "now", "coins", "reset", "trophies", "grow", "close" };
		int h = 64 + ids.Length * (bh + gap) + 10;
		mTestPanel = new TRect((mWidth - w) / 2, (mHeight - h) / 2, w, h);
		mTestButtons.Clear();
		int y = mTestPanel.mY + 60;
		foreach (string id in ids)
		{
			mTestButtons.Add((new TRect(mTestPanel.mX + 24, y, w - 48, bh), id));
			y += bh + gap;
		}
	}

	private string TestLabel(string id)
	{
		switch (id)
		{
		case "always":
			return "智慧树必来：" + (mTreeAlways ? "开" : "关");
		case "now":
			return mVisitor ? "智慧树已经在房间里了" : "立刻叫智慧树来";
		case "coins":
			return "测试加钱 +" + LawnApp.GetMoneyString(TestCoins);
		case "reset":
			return "重置今天的查寝次数（今天 " + mQuizToday + " 次）";
		case "trophies":
			return "金银奖杯直接解锁：" + (mTestTrophies ? "开" : "关");
		case "grow":
			return NextSize == 0 ? "房间已是最大 12×12" : "免费扩建 " + mN + "×" + mN + " → " + NextSize + "×" + NextSize;
		default:
			return "关闭";
		}
	}

	/// <summary>+$10,000 (the game shows coins × 10).</summary>
	private const int TestCoins = 1000;

	private void TestMenuClick(int x, int y)
	{
		LayoutTestMenu();
		foreach ((TRect r, string id) in mTestButtons)
		{
			if (!In(r, x, y))
			{
				continue;
			}
			DormPlatform.PlaySound(Resources.SOUND_TAP);
			switch (id)
			{
			case "always":
				mTreeAlways = !mTreeAlways;
				WriteSave();
				Hint(mTreeAlways ? "测试：以后每次进宿舍智慧树都会来" : "测试：智慧树恢复随机出现");
				break;
			case "now":
				if (!mVisitor)
				{
					mScreen = Screen.Room;
					MaybeStartVisit(force: true);
					mTestMenu = false;
				}
				break;
			case "coins":
				DormPlatform.AddCoins(mLawn, TestCoins);
				Hint("测试：加了 " + LawnApp.GetMoneyString(TestCoins));
				break;
			case "reset":
				mQuizToday = 0;
				mQuizDay = DateTime.Now.ToString("yyyy-MM-dd");
				WriteSave();
				Hint("测试：今天的查寝次数已重置");
				break;
			case "trophies":
				mTestTrophies = !mTestTrophies;
				WriteSave();
				Hint(mTestTrophies ? "测试：金银奖杯已解锁（在仓库／布置里）" : "测试：奖杯恢复按通关进度");
				break;
			case "grow":
				if (NextSize != 0 && !mEdit)
				{
					DoExpand(NextSize);
				}
				else if (mEdit)
				{
					Hint("先退出布置再扩建");
				}
				break;
			default:
				mTestMenu = false;
				break;
			}
			return;
		}
		if (!In(mTestPanel, x, y))
		{
			mTestMenu = false;
		}
	}

	private void DrawTestMenu(Graphics g)
	{
		LayoutTestMenu();
		g.SetColor(new Color(30, 22, 12, 150));
		g.FillRect(0, 0, mWidth, mHeight);
		Draw9(g, "ui/panel", mTestPanel, 22);
		TextCentered(g, mFont, "测试选单（只在测试版）", new TRect(mTestPanel.mX, mTestPanel.mY + 14, mTestPanel.mWidth, 34), InkBrown);
		foreach ((TRect r, string id) in mTestButtons)
		{
			bool on = (id == "always" && mTreeAlways) || (id == "trophies" && mTestTrophies);
			Draw9(g, on ? "ui/btn_on" : "ui/btn", r, 20);
			TextCentered(g, mFont, TestLabel(id), r, InkBrown, -1);
		}
	}

#if !__IOS__
	internal void TestOpenTestMenu() => mTestMenu = true;

	internal void TestMenuTap(string id)
	{
		LayoutTestMenu();
		foreach ((TRect r, string tid) in mTestButtons.ToArray())
		{
			if (tid == id)
			{
				TestMenuClick(r.mX + 4, r.mY + 4);
				break;
			}
		}
	}
#endif
}
