// 宿舍 second stage: furniture the girls use (cushion, kotatsu, garden swing), wall / floor styles,
// and the furniture warehouse (owned / placed counts, recall everything, sell back at half price).
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	// ------------------------------------------------------------------ seats

	/// <summary>Swing sprite geometry (fractions of the art): the beam the seat hangs from and the seat's sitting point.</summary>
	private const float SwingPivotX = 205f / 379f, SwingPivotY = 62f / 424f, SwingSeatX = 192f / 379f, SwingSeatY = 252f / 424f;

	/// <summary>Seats of a piece of furniture in grid coordinates; empty when nobody can sit on it.</summary>
	private List<Vector2> SeatsOf(Item it)
	{
		List<Vector2> seats = new List<Vector2>();
		switch (it.Def.Id)
		{
		case "cushion":
			seats.Add(new Vector2(it.I + 0.5f, it.J + 0.5f));
			break;
		case "kotatsu":
			// only the two back sides: the girls face the room (their models have no back view)
			seats.Add(new Vector2(it.I + 0.35f, it.J + it.B / 2f));
			seats.Add(new Vector2(it.I + it.A / 2f, it.J + 0.35f));
			break;
		case "swing":
			if (it.Rot < 2)
			{
				seats.Add(new Vector2(it.I + it.A / 2f, it.J + it.B / 2f));
			}
			break;
		}
		return seats;
	}

	private bool SeatFree(Item it, int k, DormActor theExcept) => !mActors.Any((DormActor o) => o != theExcept && ((o.Mode == DormActor.State.Seat && o.SeatItem == it && o.SeatIdx == k) || (o.PendingSeat == it && o.PendingIdx == k)));

	private bool UsableSeatItem(Item it) => mItems.Contains(it) && it != mDrag && it.Def.Kind != Kind.Wall;

	/// <summary>Free cell next to a piece of furniture where she gets on (or the cushion's own cell).</summary>
	private Point? ApproachCell(Item it, DormActor a)
	{
		if (it.Def.Kind == Kind.Flat)
		{
			return CellTaken(it.I, it.J, a) ? null : new Point(it.I, it.J);
		}
		Point? best = null;
		float bestD = float.MaxValue;
		for (int i = it.I - 1; i <= it.I + it.A; i++)
		{
			for (int j = it.J - 1; j <= it.J + it.B; j++)
			{
				if (i < 0 || j < 0 || i >= N || j >= N || Covers(it, i, j) || Blocked(i, j) || CellTaken(i, j, a))
				{
					continue;
				}
				float d = (i + 0.5f - a.FI) * (i + 0.5f - a.FI) + (j + 0.5f - a.FJ) * (j + 0.5f - a.FJ);
				if (d < bestD)
				{
					bestD = d;
					best = new Point(i, j);
				}
			}
		}
		return best;
	}

	/// <summary>An idle girl picks a free seat and walks over to it.</summary>
	private bool TryGoSit(DormActor a)
	{
		if (!a.CanSit)
		{
			return false;
		}
		List<(Item it, int k)> free = new List<(Item, int)>();
		foreach (Item it in mItems)
		{
			List<Vector2> seats = SeatsOf(it);
			for (int k = 0; k < seats.Count; k++)
			{
				if (SeatFree(it, k, a))
				{
					free.Add((it, k));
				}
			}
		}
		if (free.Count == 0)
		{
			return false;
		}
		(Item item, int idx) = free[mRand.Next(free.Count)];
		Point? cell = ApproachCell(item, a);
		if (cell == null)
		{
			return false;
		}
		Point from = new Point((int)a.FI, (int)a.FJ);
		a.PendingSeat = item;
		a.PendingIdx = idx;
		if (from == cell.Value)
		{
			EnterSeat(a, item, idx);
			return true;
		}
		List<Point> path = FindPath(from, cell.Value);
		if (path == null || path.Count == 0)
		{
			a.PendingSeat = null;
			return false;
		}
		a.Path.Clear();
		a.Path.AddRange(path);
		a.Mode = DormActor.State.Walk;
		a.StartWalk();
		return true;
	}

	private void EnterSeat(DormActor a, Item it, int k)
	{
		List<Vector2> seats = SeatsOf(it);
		a.PendingSeat = null;
		if (k >= seats.Count || !UsableSeatItem(it) || !SeatFree(it, k, a))
		{
			ToIdle(a, 1f);
			return;
		}
		a.Path.Clear();
		a.StopWalk();
		a.PlayIdle();
		a.Mode = DormActor.State.Seat;
		a.SeatItem = it;
		a.SeatIdx = k;
		a.FI = seats[k].X;
		a.FJ = seats[k].Y;
		a.SetSitting(it.Def.Id == "swing" ? DormActor.SitKind.Chair : DormActor.SitKind.Floor);
		a.Timer = 12f + (float)mRand.NextDouble() * 10f;
		a.Hop = 0.5f;
		if (it.Def.Id == "kotatsu")
		{
			// look across the table
			a.FaceLeft = SX(it.I + it.A / 2f, it.J + it.B / 2f) < SX(a.FI, a.FJ);
		}
		Hint(GirlName(a) + (it.Def.Id == "swing" ? "坐上了秋千" : it.Def.Id == "kotatsu" ? "钻进了暖桌" : "坐下来休息"));
	}

	private void LeaveSeat(DormActor a)
	{
		Item it = a.SeatItem as Item;
		a.SeatItem = null;
		a.SetSitting(DormActor.SitKind.None);
		float ci = it != null ? it.I + it.A / 2f : a.FI;
		float cj = it != null ? it.J + it.B / 2f : a.FJ;
		Point p = NearestFreeCell(ci + (it != null ? it.A / 2f : 0f), cj, a);
		a.FI = p.X + 0.5f;
		a.FJ = p.Y + 0.5f;
		a.Hop = 0.8f;
		ToIdle(a, 2f);
	}

	private void UpdateSeated(DormActor a, float dt)
	{
		Item it = a.SeatItem as Item;
		if (it == null || !UsableSeatItem(it) || (it.Def.Id == "swing" && it.Rot >= 2))
		{
			LeaveSeat(a);
			return;
		}
		a.Timer -= dt;
		if (a.Timer <= 0f)
		{
			LeaveSeat(a);
		}
	}

	/// <summary>A dropped girl lands on furniture under the finger: take its nearest free seat.</summary>
	private bool DropOnSeat(DormActor a, float gi, float gj)
	{
		if (!a.CanSit)
		{
			return false;
		}
		int ci = (int)Math.Floor(gi), cj = (int)Math.Floor(gj);
		if (ci < 0 || cj < 0 || ci >= N || cj >= N)
		{
			return false;
		}
		foreach (Item it in mItems.Where((Item t) => Covers(t, ci, cj)))
		{
			List<Vector2> seats = SeatsOf(it);
			int best = -1;
			float bestD = float.MaxValue;
			for (int k = 0; k < seats.Count; k++)
			{
				float d = (seats[k].X - gi) * (seats[k].X - gi) + (seats[k].Y - gj) * (seats[k].Y - gj);
				if (SeatFree(it, k, a) && d < bestD)
				{
					bestD = d;
					best = k;
				}
			}
			if (best >= 0)
			{
				EnterSeat(a, it, best);
				return true;
			}
		}
		return false;
	}

	// ------------------------------------------------------------------ swing

	/// <summary>Extra swing after a tap, per swing.</summary>
	private readonly Dictionary<Item, float> mSwingBoost = new Dictionary<Item, float>();

	private float SwingAngle(Item it)
	{
		bool used = mActors.Any((DormActor o) => o.Mode == DormActor.State.Seat && o.SeatItem == it);
		mSwingBoost.TryGetValue(it, out float boost);
		float amp = (used ? 5f : 0f) + boost * 5f;
		return amp * (float)Math.Sin(mTime * 2.0 * Math.PI / 2.4);
	}

	private void UpdateSwings(float dt)
	{
		foreach (Item it in mSwingBoost.Keys.ToList())
		{
			mSwingBoost[it] = Math.Max(0f, mSwingBoost[it] - dt * 0.25f);
		}
	}

	/// <summary>
	/// The garden swing: the frame (seat cut out), the seat sliding along its arc under the beam, and the girl on it.
	/// Back views are plain sprites (nobody sits there).
	/// </summary>
	private void DrawSwing(Graphics g, Item it, TRect r, Color? tint)
	{
		if (it.Rot >= 2 || it == mDrag)
		{
			DrawSprite(g, it.Sprite, r.mX, r.mY, r.mWidth, r.mHeight, tint);
			return;
		}
		bool mirror = it.Rot == 1;
		DrawSprite(g, "swing_base_" + it.Rot, r.mX, r.mY, r.mWidth, r.mHeight, tint);
		float deg = SwingAngle(it);
		float rad = MathHelper.ToRadians(deg);
		float rope = (SwingSeatY - SwingPivotY) * r.mHeight;
		float dx = (float)Math.Sin(rad) * rope;
		float dy = -(1f - (float)Math.Cos(rad)) * rope;
		DrawSprite(g, "swing_seat_" + it.Rot, r.mX + dx, r.mY + dy, r.mWidth, r.mHeight, tint);
		float seatX = r.mX + (mirror ? 1f - SwingSeatX : SwingSeatX) * r.mWidth + dx;
		float seatY = r.mY + SwingSeatY * r.mHeight + dy;
		foreach (DormActor a in mActors.Where((DormActor t) => t.Mode == DormActor.State.Seat && t.SeatItem == it))
		{
			a.Draw(g, seatX, seatY, mCW * 1.4f, -deg * 0.6f, 1f, 1f);
		}
	}

	// ------------------------------------------------------------------ wall / floor styles

	private string mWallStyle = "wall_sakura";

	private string mFloorStyle = "floor_tatami";

	private bool InUse(Def d) => d.Id == mWallStyle || d.Id == mFloorStyle;

	private void ApplyStyle(Def d)
	{
		if (d.Layer == "wall")
		{
			mWallStyle = d.Id;
		}
		else
		{
			mFloorStyle = d.Id;
		}
		Hint("换上了" + d.Title + "（记得保存）");
	}

	/// <summary>Floor layer then wall layer (both 1032x717, 128 px per cell, origin = the floor's back corner).</summary>
	private bool DrawStyledShell(Graphics g)
	{
		Image floor = Img(mFloorStyle), wall = Img(mWallStyle);
		if (floor == null || wall == null)
		{
			return false;
		}
		float s = mCW / ShellCell;
		DrawSprite(g, mFloorStyle, mOX - ShellOX * s, mOY - ShellOY * s, floor.mWidth * s, floor.mHeight * s);
		DrawSprite(g, mWallStyle, mOX - ShellOX * s, mOY - ShellOY * s, wall.mWidth * s, wall.mHeight * s);
		return true;
	}

	// ------------------------------------------------------------------ warehouse

	private int mStoreTab;

	private int mStorePage;

	private int mStorePages = 1;

	/// <summary>Piece waiting for the sell confirmation; recall-everything confirmation.</summary>
	private Def mSell;

	private bool mRecallAsk;

	private static int SellPrice(Def d) => d.Price / 2;

	private void LayoutStore(TRect panel)
	{
		mPagePanel = panel;
		int tabH = 34, top = panel.mY + 30;
		int tw = Math.Min(Math.Max(96, mFont.StringWidth("主题小物") + 20), (panel.mWidth - 24) / Tabs.Length - 2);
		for (int k = 0; k < Tabs.Length; k++)
		{
			AddUi("wtab" + k, UiKind.Tab, Tabs[k], new TRect(panel.mX + 12 + k * (tw + 2), top, tw, tabH), on: k == mStoreTab);
		}
		List<Def> defs = Catalog.Where((Def d) => (mStoreTab == 0 || d.Tab == mStoreTab) && !d.Default && Owned(d) > 0).ToList();
		mCatalogEmpty = defs.Count == 0;
		int gx = panel.mX + 12, gy = top + tabH + 10, gw = panel.mWidth - 24, gh = panel.mY + panel.mHeight - 56 - gy;
		int gap = 8;
		int cw = Math.Max(110, Math.Min(150, (gw - 3 * gap) / 4));
		int ch = Math.Min(150, Math.Max(110, gh / 2 - gap));
		int cols = Math.Max(1, (gw + gap) / (cw + gap)), rows = Math.Max(1, (gh + gap) / (ch + gap));
		int per = cols * rows;
		mStorePages = Math.Max(1, (defs.Count + per - 1) / per);
		mStorePage = Math.Clamp(mStorePage, 0, mStorePages - 1);
		int ox = gx + (gw - (cols * cw + (cols - 1) * gap)) / 2;
		int idx = 0;
		foreach (Def d in defs.Skip(mStorePage * per).Take(per))
		{
			int r = idx / cols, c = idx % cols;
			AddUi("storecard", UiKind.Shop, d.Title, new TRect(ox + c * (cw + gap), gy + r * (ch + gap), cw, ch), def: d);
			idx++;
		}
		string rc = "一键收回";
		int bw = mFont.StringWidth(rc) + 40;
		AddUi("recall", UiKind.Dialog, rc, new TRect(panel.mX + 12, panel.mY + panel.mHeight - 50, bw, 40), enabled: mItems.Count > 0);
		PagerArrows("w", panel, mStorePage, mStorePages);
	}

	private void LayoutStoreDialogs()
	{
		if (mSell == null && !mRecallAsk)
		{
			return;
		}
		int cw = Math.Min(440, mWidth - 40), ch = mSell != null ? 270 : 200;
		mBuyPanel = new TRect((mWidth - cw) / 2, (mHeight - ch) / 2, cw, ch);
		int bw = (cw - 3 * 20) / 2, bh = 46, by = mBuyPanel.mY + ch - bh - 18;
		AddUi(mSell != null ? "sl_cancel" : "rc_no", UiKind.Dialog, "取消", new TRect(mBuyPanel.mX + 20, by, bw, bh));
		AddUi(mSell != null ? "sl_ok" : "rc_yes", UiKind.Dialog, mSell != null ? "卖出" : "全部收回", new TRect(mBuyPanel.mX + 40 + bw, by, bw, bh), on: true);
	}

	private string StoreBlocker(Def d)
	{
		if (d.Kind == Kind.Style && InUse(d))
		{
			return "正在使用中，不能卖";
		}
		if (mEdit && Dirty)
		{
			return "先保存布置再卖";
		}
		if (d.Kind != Kind.Style && Available(d) <= 0)
		{
			return "都摆出来了 · 先收回才能卖";
		}
		return null;
	}

	private bool StoreButton(Ui u)
	{
		switch (u.Id)
		{
		case "storecard":
		{
			string why = StoreBlocker(u.Def);
			if (why != null)
			{
				Hint(why);
			}
			else
			{
				mSell = u.Def;
			}
			return true;
		}
		case "sl_cancel":
			mSell = null;
			return true;
		case "sl_ok":
			if (mSell != null && StoreBlocker(mSell) == null)
			{
				Def d = mSell;
				mOwned[d.Id] = Owned(d) - 1;
				if (mOwned[d.Id] <= 0)
				{
					mOwned.Remove(d.Id);
				}
				SaveOwned();
				DormPlatform.AddCoins(mLawn, SellPrice(d));
				DormPlatform.PlaySound(Resources.SOUND_COIN);
				Hint("卖出了" + d.Title + " · 得到 " + LawnApp.GetMoneyString(SellPrice(d)));
			}
			mSell = null;
			return true;
		case "recall":
			mRecallAsk = true;
			return true;
		case "rc_no":
			mRecallAsk = false;
			return true;
		case "rc_yes":
			mRecallAsk = false;
			mItems.Clear();
			mSel = null;
			Save();
			Hint("家具都收回仓库了（已保存）");
			return true;
		case "w_prev":
			mStorePage = Math.Max(0, mStorePage - 1);
			return true;
		case "w_next":
			mStorePage = Math.Min(mStorePages - 1, mStorePage + 1);
			return true;
		}
		if (u.Id.StartsWith("wtab", StringComparison.Ordinal) && int.TryParse(u.Id.Substring(4), out int t))
		{
			mStoreTab = t;
			mStorePage = 0;
			return true;
		}
		return false;
	}

	private void DrawStore(Graphics g)
	{
		DrawPanelTitle(g, mPagePanel, "家具仓库");
		if (mCatalogEmpty)
		{
			TextCentered(g, mFont, "仓库是空的 · 去家具商店买家具吧", mPagePanel, InkGrey);
		}
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Shop))
		{
			Def d = u.Def;
			Draw9(g, "ui/card", u.Rect, 18);
			int nameH = mFont.GetHeight();
			int infoH = nameH + 8;
			Image img = Img(d.Id + "_0");
			if (img != null)
			{
				int box = u.Rect.mWidth - 20, boxH = u.Rect.mHeight - nameH - infoH - 22;
				float k = Math.Min((float)box / img.mWidth, (float)boxH / img.mHeight);
				float w = img.mWidth * k, h = img.mHeight * k;
				DrawSprite(g, d.Id + "_0", u.Rect.mX + (u.Rect.mWidth - w) / 2f, u.Rect.mY + 8 + (boxH - h) / 2f, w, h);
			}
			string t = d.Title;
			while (t.Length > 1 && mFont.StringWidth(t) > u.Rect.mWidth - 8)
			{
				t = t.Substring(0, t.Length - 1);
			}
			Text(g, mFont, t, u.Rect.mX + (u.Rect.mWidth - mFont.StringWidth(t)) / 2, u.Rect.mY + u.Rect.mHeight - infoH - nameH - 8, InkBrown);
			string info = d.Kind == Kind.Style ? (InUse(d) ? "使用中" : "在仓库") : ("持有" + Owned(d) + " 摆出" + Placed(d));
			TRect ir = new TRect(u.Rect.mX + 6, u.Rect.mY + u.Rect.mHeight - infoH - 6, u.Rect.mWidth - 12, infoH);
			Draw9(g, "ui/tab", ir, 12);
			TextCentered(g, mFont, info, ir, InkBrown, -1);
			if (d.Kind != Kind.Style)
			{
				// what is left in the warehouse (the part that can be placed or sold)
				string left = "仓库 " + Available(d);
				int bw = mFont.StringWidth(left) + 18;
				TRect badge = new TRect(u.Rect.mX + u.Rect.mWidth - bw - 4, u.Rect.mY + 4, bw, nameH + 4);
				Draw9(g, "ui/pill_dark", badge, 10, Available(d) > 0 ? null : new Color(160, 150, 140));
				TextCentered(g, mFont, left, badge, Color.White);
			}
		}
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Dialog && t.Id == "recall"))
		{
			DrawUi(g, u);
		}
	}

	private void DrawStoreDialogs(Graphics g)
	{
		if (mSell == null && !mRecallAsk)
		{
			return;
		}
		g.SetColor(new Color(40, 25, 15, 140));
		g.FillRect(0, 0, mWidth, mHeight);
		TRect p = mBuyPanel;
		Draw9(g, "ui/panel", p, 26);
		Flowers(g, p);
		string title = mSell != null ? "卖回家具" : "一键收回";
		int tw = mFont.StringWidth(title) + 60;
		TRect tr = new TRect(p.mX + (p.mWidth - tw) / 2, p.mY - 20, tw, 44);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
		if (mSell != null)
		{
			Image img = Img(mSell.Id + "_0");
			int box = 110;
			if (img != null)
			{
				float k = Math.Min((float)box / img.mWidth, (float)box / img.mHeight);
				DrawSprite(g, mSell.Id + "_0", p.mX + 30 + (box - img.mWidth * k) / 2f, p.mY + 34 + (box - img.mHeight * k) / 2f, img.mWidth * k, img.mHeight * k);
			}
			int tx = p.mX + 30 + box + 20;
			Text(g, mFont, mSell.Title, tx, p.mY + 44, InkBrown);
			Text(g, mFont, "卖出可得  " + LawnApp.GetMoneyString(SellPrice(mSell)), tx, p.mY + 84, new Color(170, 110, 20));
			Text(g, mFont, "（买价的一半）", tx, p.mY + 110, InkGrey);
			if (mSell.Kind != Kind.Style)
			{
				Text(g, mFont, "仓库里还有  " + Available(mSell) + " 件", tx, p.mY + 136, InkBrown);
			}
		}
		else
		{
			TRect body = new TRect(p.mX + 20, p.mY + 40, p.mWidth - 40, 70);
			TextCentered(g, mFont, "房间里的 " + mItems.Count + " 件家具全部收回仓库？", body, InkBrown);
		}
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Dialog && (t.Id.StartsWith("sl_", StringComparison.Ordinal) || t.Id.StartsWith("rc_", StringComparison.Ordinal))))
		{
			DrawUi(g, u);
		}
	}

#if !__IOS__
	internal void TestSell(string id)
	{
		mSell = FindDef(id);
	}

	internal void TestSit(int index, string itemId, int seat)
	{
		Item it = mItems.FirstOrDefault((Item t) => t.Def.Id == itemId);
		if (it != null && itemId == "cushion" && FitsAt(it, 6, 5, 0))
		{
			it.I = 6;
			it.J = 5;
		}
		if (index < mActors.Count && it != null)
		{
			EnterSeat(mActors[index], it, seat);
			mActors[index].Timer = 60f;
			Console.WriteLine($"DORMTEST sat {mActors[index].Name} on {itemId} rot={it.Rot} mode={mActors[index].Mode} sit={mActors[index].Sitting} canSit={mActors[index].CanSit} seats={SeatsOf(it).Count}");
		}
	}

	internal void TestStyle(string wall, string floor)
	{
		Def w = FindDef(wall), f = FindDef(floor);
		if (w != null && Owned(w) > 0)
		{
			ApplyStyle(w);
		}
		if (f != null && Owned(f) > 0)
		{
			ApplyStyle(f);
		}
	}
#endif
}
