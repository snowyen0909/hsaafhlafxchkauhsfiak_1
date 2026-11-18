// 宿舍 chrome and input (layout follows the GPT mock-ups: title top-left, comfort top-right, side menu on the
// left, catalogue at the bottom while arranging, round buttons around the selected piece, exit confirmation).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	private static readonly Color BgColor = new Color(236, 220, 204);

	private static readonly Color InkBrown = new Color(92, 58, 32);

	private static readonly Color InkGrey = new Color(150, 140, 130);

	private static readonly Color InkWarn = new Color(196, 70, 50);

	private enum UiKind
	{
		Side,
		Square,
		Bottom,
		Circle,
		Tab,
		Card,
		Arrow,
		Dialog,
		/// <summary>Furniture shop card.</summary>
		Shop,
		/// <summary>Girl portrait in 角色管理.</summary>
		Face
	}

	private sealed class Ui
	{
		public string Id;

		public UiKind Kind;

		public string Text;

		public string Icon;

		public TRect Rect;

		public bool On;

		public bool Enabled = true;

		public Def Def;
	}

	private readonly List<Ui> mUi = new List<Ui>();

	private int mTab;

	private int mPage;

	private int mPageCount = 1;

	private bool mConfirm;

	private TRect mCardPanel;

	private TRect mConfirmPanel;

	// press / long-press tracking
	private int mDownX;

	private int mDownY;

	private int mLastX;

	private int mLastY;

	private bool mMoved;

	private bool mDownOnUi;

	private DormActor mPress;

	private float mPressTime;

	private const int Pad = 10;

	// ------------------------------------------------------------------ layout

	private Ui AddUi(string id, UiKind kind, string text, TRect rect, string icon = null, bool on = false, bool enabled = true, Def def = null)
	{
		Ui u = new Ui { Id = id, Kind = kind, Text = text, Rect = rect, Icon = icon, On = on, Enabled = enabled, Def = def };
		mUi.Add(u);
		return u;
	}

	private void Layout()
	{
		mUi.Clear();
		int W = mWidth, H = mHeight;
		// top-left: back + title
		AddUi("back", UiKind.Circle, null, new TRect(Pad, Pad, 52, 52), "ui/ic_back");
		// left side menu
		int sideW = Math.Max(140, mFont.StringWidth("墙纸/地板") + 52), sideH = 42, y = 78;
		if (mScreen != Screen.Room)
		{
			AddUi("hall", UiKind.Side, "宿舍大厅", new TRect(Pad, y, sideW, sideH), "ui/ic_house");
			AddUi("edit", UiKind.Side, "布置", new TRect(Pad, y += sideH + 8, sideW, sideH), "ui/ic_move");
			AddUi("shop", UiKind.Side, "家具商店", new TRect(Pad, y += sideH + 8, sideW, sideH), on: mScreen == Screen.Shop);
			AddUi("roster", UiKind.Side, "角色管理", new TRect(Pad, y += sideH + 8, sideW, sideH), on: mScreen == Screen.Roster);
			AddUi("soon_store", UiKind.Side, "家具仓库", new TRect(Pad, y += sideH + 8, sideW, sideH), enabled: false);
			TRect panel = new TRect(Pad + sideW + 12, 66, W - Pad - (Pad + sideW + 12), H - Pad - 66);
			if (mScreen == Screen.Shop)
			{
				LayoutShop(panel);
			}
			else
			{
				LayoutRoster(panel);
			}
			LayoutBuyConfirm();
			return;
		}
		if (!mEdit)
		{
			AddUi("hall", UiKind.Side, "宿舍大厅", new TRect(Pad, y, sideW, sideH), "ui/ic_house", on: true);
			AddUi("edit", UiKind.Side, "布置", new TRect(Pad, y += sideH + 8, sideW, sideH), "ui/ic_move");
			AddUi("shop", UiKind.Side, "家具商店", new TRect(Pad, y += sideH + 8, sideW, sideH));
			AddUi("roster", UiKind.Side, "角色管理", new TRect(Pad, y += sideH + 8, sideW, sideH));
			AddUi("soon_store", UiKind.Side, "家具仓库", new TRect(Pad, y += sideH + 8, sideW, sideH), enabled: false);
			string[] bottom = { "好感度", "舒适度加成" };
			int bw = Math.Min(Math.Max(110, bottom.Max((string t) => mFont.StringWidth(t)) + 44), (W - 2 * Pad - 4 * 8) / bottom.Length), bh = 40;
			int bx = (W - (bw * bottom.Length + 8 * (bottom.Length - 1))) / 2;
			for (int k = 0; k < bottom.Length; k++)
			{
				AddUi("soon_b" + (k + 1), UiKind.Bottom, bottom[k], new TRect(bx + k * (bw + 8), H - Pad - bh, bw, bh), enabled: false);
			}
			PlaceRoom(Pad + 90, 6, W - Pad - 90, H - Pad - bh - 6);
		}
		else
		{
			AddUi("edit", UiKind.Side, "布置", new TRect(Pad, y, sideW, sideH), "ui/ic_move", on: true);
			AddUi("shop", UiKind.Side, "家具商店", new TRect(Pad, y += sideH + 8, sideW, sideH));
			AddUi("soon_store", UiKind.Side, "家具仓库", new TRect(Pad, y += sideH + 8, sideW, sideH), enabled: false);
			AddUi("soon_paper", UiKind.Side, "墙纸/地板", new TRect(Pad, y += sideH + 8, sideW, sideH), enabled: false);
			AddUi("soon_theme", UiKind.Side, "主题套装", new TRect(Pad, y += sideH + 8, sideW, sideH), enabled: false);
			// right column: save / load / clear
			int sq = 70;
			AddUi("save", UiKind.Square, "保存", new TRect(W - Pad - sq, 78, sq, 66), "ui/ic_save");
			AddUi("load", UiKind.Square, "载入", new TRect(W - Pad - sq, 78 + 74, sq, 66), "ui/ic_load");
			AddUi("clear", UiKind.Square, "清空", new TRect(W - Pad - sq, 78 + 148, sq, 66), "ui/ic_clear");
			// catalogue: tabs + cards
			int cardH = 98, tabH = 34;
			mCardPanel = new TRect(Pad, H - Pad - cardH, W - 2 * Pad, cardH);
			int tw = Math.Min(104, (W - 2 * Pad - 20) / Tabs.Length);
			for (int k = 0; k < Tabs.Length; k++)
			{
				AddUi("tab" + k, UiKind.Tab, Tabs[k], new TRect(Pad + 14 + k * (tw + 2), mCardPanel.mY - tabH + 4, tw, tabH), on: k == mTab);
			}
			List<Def> defs = Catalog.Where((Def d) => mTab == 0 || d.Tab == mTab).OrderByDescending((Def d) => Available(d) > 0).ToList();
			int card = cardH - 16, gap = 6, arrow = 34;
			int perPage = Math.Max(1, (mCardPanel.mWidth - 2 * arrow - 12) / (card + gap));
			mPageCount = Math.Max(1, (defs.Count + perPage - 1) / perPage);
			mPage = Math.Clamp(mPage, 0, mPageCount - 1);
			int cx = mCardPanel.mX + arrow + 6;
			foreach (Def d in defs.Skip(mPage * perPage).Take(perPage))
			{
				AddUi("card", UiKind.Card, d.Title, new TRect(cx, mCardPanel.mY + 8, card, card), def: d);
				cx += card + gap;
			}
			AddUi("prev", UiKind.Arrow, null, new TRect(mCardPanel.mX + 2, mCardPanel.mY + (cardH - 44) / 2, arrow, 44), "ui/ic_left", enabled: mPage > 0);
			AddUi("next", UiKind.Arrow, null, new TRect(mCardPanel.mX + mCardPanel.mWidth - arrow - 2, mCardPanel.mY + (cardH - 44) / 2, arrow, 44), "ui/ic_right", enabled: mPage < mPageCount - 1);
			PlaceRoom(Pad + 90, 6, W - Pad - sq - 10, mCardPanel.mY - tabH - 4);
			// round buttons around the selected piece
			if (mSel != null && mDrag == null && mItems.Contains(mSel))
			{
				TRect r = mSel.Def.Kind == Kind.Wall ? WallItemRect(mSel) : ItemRect(mSel);
				int s = 46;
				int mx = r.mX + r.mWidth / 2, my = r.mY + r.mHeight / 2;
				if (mSel.Def.Rotates)
				{
					AddUi("rotate", UiKind.Circle, null, Clamp(new TRect(mx - s / 2, r.mY - s - 2, s, s)), "ui/ic_rotate");
				}
				AddUi("store", UiKind.Circle, null, Clamp(new TRect(r.mX - s - 2, my - s / 2, s, s)), "ui/ic_x");
				AddUi("ok", UiKind.Circle, null, Clamp(new TRect(r.mX + r.mWidth + 2, my - s / 2, s, s)), "ui/ic_ok");
			}
		}
		if (mConfirm)
		{
			int cw = Math.Min(480, W - 40), ch = 250;
			mConfirmPanel = new TRect((W - cw) / 2, (H - ch) / 2, cw, ch);
			int bw = (cw - 4 * 14) / 3, bh = 46, by = mConfirmPanel.mY + ch - bh - 22;
			AddUi("c_continue", UiKind.Dialog, "继续布置", new TRect(mConfirmPanel.mX + 14, by, bw, bh));
			AddUi("c_discard", UiKind.Dialog, "不储存退出", new TRect(mConfirmPanel.mX + 28 + bw, by, bw, bh));
			AddUi("c_save", UiKind.Dialog, "储存并退出", new TRect(mConfirmPanel.mX + 42 + 2 * bw, by, bw, bh), on: true);
		}
	}

	private TRect Clamp(TRect r)
	{
		r.mX = Math.Clamp(r.mX, 2, mWidth - r.mWidth - 2);
		r.mY = Math.Clamp(r.mY, 2, mHeight - r.mHeight - 2);
		return r;
	}

	// ------------------------------------------------------------------ drawing

	public override void Draw(Graphics g)
	{
		Layout();
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(BgColor);
		g.FillRect(0, 0, mWidth, mHeight);
		if (mScreen == Screen.Room)
		{
			DrawRoom(g);
		}
		else
		{
			DrawPageBack(g);
		}
		DrawChrome(g);
	}

	/// <summary>Nine-slice: corners keep their size, edges and centre stretch.</summary>
	private void Draw9(Graphics g, string name, TRect r, int corner, Color? tint = null)
	{
		Image img = Img(name);
		if (img == null)
		{
			return;
		}
		int c = Math.Min(corner, Math.Min(r.mWidth, r.mHeight) / 2);
		int sc = corner;
		int sw = img.mWidth, sh = img.mHeight;
		int[] dx = { r.mX, r.mX + c, r.mX + r.mWidth - c, r.mX + r.mWidth };
		int[] dy = { r.mY, r.mY + c, r.mY + r.mHeight - c, r.mY + r.mHeight };
		int[] sx = { 0, sc, sw - sc, sw };
		int[] sy = { 0, sc, sh - sc, sh };
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(tint ?? Color.White);
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				if (dx[i + 1] - dx[i] <= 0 || dy[j + 1] - dy[j] <= 0)
				{
					continue;
				}
				g.DrawImage(img, new TRect(dx[i], dy[j], dx[i + 1] - dx[i], dy[j + 1] - dy[j]), new TRect(sx[i], sy[j], sx[i + 1] - sx[i], sy[j + 1] - sy[j]));
			}
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	private void Text(Graphics g, Font f, string s, int x, int y, Color c)
	{
		g.SetFont(f);
		g.SetColor(c);
		g.DrawString(s, x, y);
	}

	private void TextCentered(Graphics g, Font f, string s, TRect r, Color c, int dy = 0)
	{
		Text(g, f, s, r.mX + (r.mWidth - f.StringWidth(s)) / 2, r.mY + (r.mHeight - f.GetHeight()) / 2 + dy, c);
	}

	private void DrawChrome(Graphics g)
	{
		// title
		string title = "植物娘宿舍";
		int tw = mFont.StringWidth(title) + 66;
		TRect titleR = new TRect(Pad + 58, Pad + 6, tw, 42);
		Draw9(g, "ui/panel", titleR, 22);
		DrawSprite(g, "ui/ic_house", titleR.mX + 10, titleR.mY + 6, 30, 30);
		Text(g, mFont, title, titleR.mX + 48, titleR.mY + (titleR.mHeight - mFont.GetHeight()) / 2 - 1, InkBrown);
		if (mScreen != Screen.Room)
		{
			DrawCoins(g);
			if (mScreen == Screen.Shop)
			{
				DrawShop(g);
			}
			else
			{
				DrawRoster(g);
			}
			foreach (Ui u in mUi)
			{
				if (u.Kind != UiKind.Dialog && u.Kind != UiKind.Card && u.Kind != UiKind.Shop && u.Kind != UiKind.Face)
				{
					DrawUi(g, u);
				}
			}
			DrawHint(g);
			if (mBuy != null)
			{
				DrawBuyConfirm(g);
			}
			return;
		}
		// comfort
		string val = Comfort.ToString();
		int cw = 70 + mFont.StringWidth("舒适度") + mBig.StringWidth(val) + 20;
		TRect cr = new TRect(mWidth - Pad - cw, Pad + 6, cw, 42);
		Draw9(g, "ui/pill_dark", cr, 20);
		DrawSprite(g, "heart", cr.mX + 6, cr.mY + 3, 36, 36);
		Text(g, mFont, "舒适度", cr.mX + 48, cr.mY + (cr.mHeight - mFont.GetHeight()) / 2, new Color(255, 240, 220));
		Text(g, mBig, val, cr.mX + cr.mWidth - 14 - mBig.StringWidth(val), cr.mY + (cr.mHeight - mBig.GetHeight()) / 2, Color.White);
		if (mEdit)
		{
			Draw9(g, "ui/panel", mCardPanel, 22);
			if (mTab == 4)
			{
				TextCentered(g, mFont, "墙纸 · 即将推出", mCardPanel, InkGrey);
			}
		}
		foreach (Ui u in mUi)
		{
			if (u.Kind != UiKind.Dialog)
			{
				DrawUi(g, u);
			}
		}
		DrawHint(g);
		if (mConfirm)
		{
			DrawConfirm(g);
		}
	}

	private void DrawHint(Graphics g)
	{
		if (mHintTimer > 0f && mHint.Length > 0)
		{
			int hw = mFont.StringWidth(mHint) + 28;
			TRect hr = new TRect((mWidth - hw) / 2, Pad + 58, hw, mFont.GetHeight() + 14);
			Draw9(g, "ui/pill_dark", hr, 16, new Color(255, 255, 255, (int)(255 * Math.Min(1f, mHintTimer * 2f))));
			TextCentered(g, mFont, mHint, hr, Color.White);
		}
	}

	private void DrawUi(Graphics g, Ui u)
	{
		switch (u.Kind)
		{
		case UiKind.Side:
		case UiKind.Bottom:
		{
			Draw9(g, u.On ? "ui/btn_on" : (u.Enabled ? "ui/btn" : "ui/btn_off"), u.Rect, 22);
			Color ink = u.Enabled ? InkBrown : InkGrey;
			int x = u.Rect.mX + 14;
			if (u.Icon != null)
			{
				DrawSprite(g, u.Icon, x, u.Rect.mY + (u.Rect.mHeight - 28) / 2, 28, 28);
				x += 34;
				Text(g, mFont, u.Text, x, u.Rect.mY + (u.Rect.mHeight - mFont.GetHeight()) / 2 - 2, ink);
			}
			else if (!u.Enabled)
			{
				Text(g, mFont, u.Text, u.Rect.mX + 12, u.Rect.mY + (u.Rect.mHeight - mFont.GetHeight()) / 2 - 2, ink);
			}
			else
			{
				TextCentered(g, mFont, u.Text, u.Rect, ink, -2);
			}
			if (!u.Enabled)
			{
				DrawSprite(g, "ui/ic_lock", u.Rect.mX + u.Rect.mWidth - 26, u.Rect.mY + (u.Rect.mHeight - 18) / 2 - 2, 18, 18);
			}
			break;
		}
		case UiKind.Square:
			Draw9(g, "ui/btn", u.Rect, 22);
			DrawSprite(g, u.Icon, u.Rect.mX + (u.Rect.mWidth - 32) / 2, u.Rect.mY + 6, 32, 32);
			TextCentered(g, mFont, u.Text, new TRect(u.Rect.mX, u.Rect.mY + 38, u.Rect.mWidth, u.Rect.mHeight - 42), InkBrown);
			break;
		case UiKind.Circle:
			DrawSprite(g, "ui/circle", u.Rect.mX, u.Rect.mY, u.Rect.mWidth, u.Rect.mHeight);
			DrawSprite(g, u.Icon, u.Rect.mX + u.Rect.mWidth * 0.2f, u.Rect.mY + u.Rect.mHeight * 0.18f, u.Rect.mWidth * 0.6f, u.Rect.mHeight * 0.6f);
			break;
		case UiKind.Tab:
			Draw9(g, u.On ? "ui/tab_on" : "ui/tab", u.Rect, 18);
			TextCentered(g, mFont, u.Text, u.Rect, u.On ? Color.White : InkBrown, -2);
			break;
		case UiKind.Arrow:
			DrawSprite(g, u.Icon, u.Rect.mX, u.Rect.mY, u.Rect.mWidth, u.Rect.mHeight, u.Enabled ? Color.White : new Color(255, 255, 255, 70));
			break;
		case UiKind.Card:
		{
			int left = Available(u.Def);
			Draw9(g, "ui/card", u.Rect, 18, left > 0 ? null : new Color(200, 190, 180));
			Image img = Img(u.Def.Id + "_0");
			if (img != null)
			{
				int box = u.Rect.mWidth - 14;
				int boxH = u.Rect.mHeight - mFont.GetHeight() - 12;
				float k = Math.Min((float)box / img.mWidth, (float)boxH / img.mHeight);
				float w = img.mWidth * k, h = img.mHeight * k;
				DrawSprite(g, u.Def.Id + "_0", u.Rect.mX + (u.Rect.mWidth - w) / 2f, u.Rect.mY + 4 + (boxH - h) / 2f, w, h);
			}
			string t = u.Text;
			while (t.Length > 1 && mFont.StringWidth(t) > u.Rect.mWidth - 6)
			{
				t = t.Substring(0, t.Length - 1);
			}
			Text(g, mFont, t, u.Rect.mX + (u.Rect.mWidth - mFont.StringWidth(t)) / 2, u.Rect.mY + u.Rect.mHeight - mFont.GetHeight() - 5, InkBrown);
			string cnt = "x" + left;
			TRect badge = new TRect(u.Rect.mX + u.Rect.mWidth - mFont.StringWidth(cnt) - 14, u.Rect.mY + 3, mFont.StringWidth(cnt) + 10, mFont.GetHeight() + 2);
			Draw9(g, "ui/pill_dark", badge, 10, left > 0 ? null : new Color(160, 150, 140));
			TextCentered(g, mFont, cnt, badge, Color.White);
			if (left <= 0)
			{
				g.SetColor(new Color(236, 220, 204, 120));
				g.FillRect(u.Rect.mX + 4, u.Rect.mY + 4, u.Rect.mWidth - 8, u.Rect.mHeight - 8);
			}
			break;
		}
		case UiKind.Dialog:
			Draw9(g, u.On ? "ui/btn_on" : "ui/btn", u.Rect, 22);
			TextCentered(g, mFont, u.Text, u.Rect, InkBrown, -2);
			break;
		}
	}

	private void DrawConfirm(Graphics g)
	{
		g.SetColor(new Color(40, 25, 15, 140));
		g.FillRect(0, 0, mWidth, mHeight);
		TRect p = mConfirmPanel;
		Draw9(g, "ui/panel", p, 26);
		string title = "退出布置";
		int tw = mFont.StringWidth(title) + 60;
		TRect tr = new TRect(p.mX + (p.mWidth - tw) / 2, p.mY - 20, tw, 44);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
		TextCentered(g, mFont, "要退出布置模式吗？", new TRect(p.mX, p.mY + 40, p.mWidth, 40), InkBrown);
		TRect warn = new TRect(p.mX + 40, p.mY + 92, p.mWidth - 80, 38);
		Draw9(g, "ui/tab", warn, 18);
		TextCentered(g, mFont, "未储存的变更将会遗失！", warn, InkWarn, -2);
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Dialog))
		{
			DrawUi(g, u);
		}
	}

	private void Hint(string s)
	{
		mHint = s;
		mHintTimer = 3f;
	}

	// ------------------------------------------------------------------ input

	private static bool In(TRect r, int x, int y) => x >= r.mX && y >= r.mY && x < r.mX + r.mWidth && y < r.mY + r.mHeight;

	private Ui HitUi(int x, int y)
	{
		if (mBuy != null)
		{
			return mUi.FirstOrDefault((Ui u) => u.Kind == UiKind.Dialog && In(u.Rect, x, y)) ?? new Ui { Id = "panel" };
		}
		if (mScreen != Screen.Room)
		{
			for (int k = mUi.Count - 1; k >= 0; k--)
			{
				if (In(mUi[k].Rect, x, y))
				{
					return mUi[k];
				}
			}
			return new Ui { Id = "panel" };
		}
		if (mConfirm)
		{
			return mUi.FirstOrDefault((Ui u) => u.Kind == UiKind.Dialog && In(u.Rect, x, y));
		}
		for (int k = mUi.Count - 1; k >= 0; k--)
		{
			if (In(mUi[k].Rect, x, y))
			{
				return mUi[k];
			}
		}
		if (mEdit && (In(mCardPanel, x, y) || mUi.Any((Ui u) => u.Kind == UiKind.Tab && In(u.Rect, x, y))))
		{
			return new Ui { Id = "panel" };
		}
		return null;
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		mDownX = mLastX = x;
		mDownY = mLastY = y;
		mMoved = false;
		mPress = null;
		mDownOnUi = mConfirm || HitUi(x, y) != null;
		if (mDownOnUi)
		{
			return;
		}
		if (mEdit)
		{
			Item hit = HitItem(x, y);
			if (hit != null)
			{
				BeginDrag(hit, x, y, isNew: false);
			}
			return;
		}
		DormActor a = HitActor(x, y);
		if (a != null)
		{
			mPress = a;
			mPressTime = 0f;
		}
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		MouseDown(x, y, theClickCount);
	}

	/// <summary>Long press on a girl picks her up.</summary>
	private void UpdatePress(float dt)
	{
		if (mPress == null)
		{
			return;
		}
		mPressTime += dt;
		if (mPressTime >= 0.35f)
		{
			PickUp(mPress, mLastX, mLastY);
			mPress = null;
		}
	}

	private void BeginDrag(Item it, int x, int y, bool isNew)
	{
		mSel = it;
		mDrag = it;
		mDragOrigI = it.I;
		mDragOrigJ = it.J;
		mDragOrigSide = it.Side;
		mDragIsNew = isNew;
		ToGrid(x, y, out float gi, out float gj);
		mGrabDI = it.Def.Kind == Kind.Wall ? 0f : gi - it.I;
		mGrabDJ = it.Def.Kind == Kind.Wall ? 0f : gj - it.J;
		mDragValid = !isNew || (it.Def.Kind == Kind.Wall ? WallSlotFree(it, it.Side, it.I) : FitsAt(it, it.I, it.J, it.Rot));
	}

	public override void MouseDrag(int x, int y)
	{
		if (Math.Abs(x - mDownX) > 8 || Math.Abs(y - mDownY) > 8)
		{
			mMoved = true;
		}
		int ddx = x - mLastX;
		mLastX = x;
		mLastY = y;
		if (mHeld != null)
		{
			mHeldVX = mHeldVX * 0.6f + ddx * 60f * 0.4f;
			mHeldX = x;
			mHeldY = y;
			return;
		}
		if (mPress != null)
		{
			// dragging a girl straight away also picks her up
			if (Math.Abs(x - mDownX) > 14 || Math.Abs(y - mDownY) > 14)
			{
				PickUp(mPress, x, y);
				mPress = null;
			}
			return;
		}
		if (mDrag == null || !mMoved)
		{
			return;
		}
		if (mDrag.Def.Kind == Kind.Wall)
		{
			// nearest wall slot to the finger
			int bestSide = mDrag.Side, bestSlot = mDrag.I;
			float best = float.MaxValue;
			for (int side = 0; side < 2; side++)
			{
				for (int slot = 0; slot < N; slot++)
				{
					WallSlotPoint(side, slot, out float wx, out float wy);
					wy -= mCW * 0.95f;
					float d = (wx - x) * (wx - x) + (wy - y) * (wy - y);
					if (d < best)
					{
						best = d;
						bestSide = side;
						bestSlot = slot;
					}
				}
			}
			mDrag.Side = bestSide;
			mDrag.I = bestSlot;
			mDragValid = WallSlotFree(mDrag, bestSide, bestSlot);
			return;
		}
		ToGrid(x, y, out float gi, out float gj);
		int ni = Math.Clamp((int)Math.Round(gi - mGrabDI), -1, N);
		int nj = Math.Clamp((int)Math.Round(gj - mGrabDJ), -1, N);
		mDrag.I = ni;
		mDrag.J = nj;
		mDragValid = FitsAt(mDrag, ni, nj, mDrag.Rot);
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mHeld != null)
		{
			DropHeld();
			return;
		}
		if (mPress != null)
		{
			DormActor a = mPress;
			mPress = null;
			Pet(a);
			return;
		}
		if (mDrag != null)
		{
			Item it = mDrag;
			mDrag = null;
			if (mMoved)
			{
				if (mDragValid)
				{
					GlobalStaticVars.gLawnApp.PlaySample(Resources.SOUND_PLANT);
				}
				else if (mDragIsNew)
				{
					mItems.Remove(it);
					mSel = null;
					Hint("这里放不下");
				}
				else
				{
					it.I = mDragOrigI;
					it.J = mDragOrigJ;
					it.Side = mDragOrigSide;
					Hint("这里放不下");
				}
				return;
			}
		}
		if (!mMoved)
		{
			Tap(x, y);
		}
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		MouseUp(x, y, theClickCount);
	}

	private Item HitItem(int x, int y)
	{
		// front-most first: solid items by depth, then wall items, then flat ones
		foreach (Item it in mItems.Where((Item t) => t.Def.Kind == Kind.Solid).OrderByDescending((Item t) => t.I + t.A + t.J + t.B))
		{
			if (In(ItemRect(it), x, y))
			{
				return it;
			}
		}
		foreach (Item it in mItems.Where((Item t) => t.Def.Kind == Kind.Wall))
		{
			if (In(WallItemRect(it), x, y))
			{
				return it;
			}
		}
		ToGrid(x, y, out float gi, out float gj);
		int ci = (int)Math.Floor(gi), cj = (int)Math.Floor(gj);
		return mItems.FirstOrDefault((Item t) => t.Def.Kind == Kind.Flat && Covers(t, ci, cj));
	}

	private void Tap(int x, int y)
	{
		Ui u = HitUi(x, y);
		if (u != null)
		{
			if (u.Id != "panel")
			{
				Button(u);
			}
			return;
		}
		if (mConfirm)
		{
			return;
		}
		if (mEdit)
		{
			mSel = HitItem(x, y);
		}
	}

	private void Button(Ui u)
	{
		if (!u.Enabled)
		{
			if (u.Id.StartsWith("soon"))
			{
				Hint(u.Text + " · 即将推出");
			}
			return;
		}
		GlobalStaticVars.gLawnApp.PlaySample(Resources.SOUND_TAP);
		if (PageButton(u))
		{
			return;
		}
		switch (u.Id)
		{
		case "back":
			if (mScreen != Screen.Room)
			{
				LeavePage();
			}
			else if (mEdit)
			{
				RequestExitEdit();
			}
			else
			{
				Close();
			}
			break;
		case "hall":
			if (mScreen != Screen.Room)
			{
				LeavePage();
				if (mEdit)
				{
					RequestExitEdit();
				}
			}
			break;
		case "shop":
			OpenPage(Screen.Shop);
			break;
		case "roster":
			OpenPage(Screen.Roster);
			break;
		case "edit":
			if (mScreen != Screen.Room)
			{
				LeavePage();
				if (!mEdit)
				{
					mEdit = true;
					mSel = null;
				}
				break;
			}
			if (mEdit)
			{
				RequestExitEdit();
			}
			else
			{
				mEdit = true;
				mSel = null;
				Hint("拖动家具摆放 · 点家具可旋转 / 收回 · 记得保存");
			}
			break;
		case "save":
			Hint(Save() ? "已保存" : "保存失败");
			break;
		case "load":
			if (!Load())
			{
				DefaultLayout();
			}
			mSnapshot = Serialize();
			mSel = null;
			Hint("已载入上次保存的布置");
			break;
		case "clear":
			mItems.Clear();
			mSel = null;
			Hint("已清空（还没保存）");
			break;
		case "rotate":
			if (mSel != null && mSel.Def.Rotates)
			{
				int next = (mSel.Rot + 1) & 3;
				if (FitsAt(mSel, mSel.I, mSel.J, next))
				{
					mSel.Rot = next;
				}
				else
				{
					Hint("旁边没有空间旋转");
				}
			}
			break;
		case "store":
			if (mSel != null)
			{
				mItems.Remove(mSel);
				mSel = null;
			}
			break;
		case "ok":
			mSel = null;
			break;
		case "prev":
			mPage = Math.Max(0, mPage - 1);
			break;
		case "next":
			mPage = Math.Min(mPageCount - 1, mPage + 1);
			break;
		case "card":
			if (Available(u.Def) <= 0)
			{
				Hint(Owned(u.Def) > 0 ? "这件已经全部摆出来了 · 可以去家具商店再买" : "还没有这件家具 · 去家具商店购买");
				break;
			}
			AddNew(u.Def);
			break;
		case "shopcard":
			mBuy = u.Def;
			break;
		case "b_cancel":
			mBuy = null;
			break;
		case "b_buy":
			if (mBuy != null)
			{
				if (Buy(mBuy))
				{
					GlobalStaticVars.gLawnApp.PlaySample(Resources.SOUND_COIN);
					Hint("购买成功 · " + mBuy.Title + " 持有 " + Owned(mBuy) + " 件");
				}
				else
				{
					Hint("金币不够");
				}
				mBuy = null;
			}
			break;
		case "face":
			SelectRosterGirl(u.Text);
			break;
		case "r_toggle":
			if (mRosterSel != null)
			{
				bool inDorm = mChosen.Contains(mRosterSel);
				if (!ToggleGirl(mRosterSel))
				{
					Hint("宿舍最多住 " + MaxGirls + " 位 · 先让一位离开");
				}
				else
				{
					Hint(FindGirl(mRosterSel).Name + (inDorm ? " 离开了宿舍" : " 住进了宿舍"));
				}
			}
			break;
		case "c_continue":
			mConfirm = false;
			break;
		case "c_discard":
			Deserialize(mSnapshot);
			mConfirm = false;
			mEdit = false;
			mSel = null;
			break;
		case "c_save":
			Save();
			mConfirm = false;
			mEdit = false;
			mSel = null;
			Hint("已保存");
			break;
		default:
			if (u.Id.StartsWith("tab") && int.TryParse(u.Id.Substring(3), out int t))
			{
				mTab = t;
				mPage = 0;
			}
			break;
		}
	}

	private void RequestExitEdit()
	{
		mSel = null;
		if (Dirty)
		{
			mConfirm = true;
		}
		else
		{
			mEdit = false;
		}
	}

	/// <summary>Catalogue tap: put a new piece on the first free spot nearest the middle of the room.</summary>
	private void AddNew(Def def)
	{
		Item it = new Item { Def = def };
		if (def.Kind == Kind.Wall)
		{
			for (int k = 0; k < 2 * N; k++)
			{
				int side = k % 2, slot = (N / 2 + (k / 2 + 1) / 2 * ((k / 2) % 2 == 0 ? 1 : -1) + N) % N;
				if (WallSlotFree(it, side, slot))
				{
					it.Side = side;
					it.I = slot;
					mItems.Add(it);
					mSel = it;
					return;
				}
			}
			Hint("墙上没有空位了");
			return;
		}
		List<Point> cells = new List<Point>();
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				cells.Add(new Point(i, j));
			}
		}
		foreach (Point c in cells.OrderBy((Point p) => Math.Abs(p.X - N / 2 + 1) + Math.Abs(p.Y - N / 2 + 1)))
		{
			if (FitsAt(it, c.X, c.Y, 0))
			{
				it.I = c.X;
				it.J = c.Y;
				mItems.Add(it);
				mSel = it;
				GlobalStaticVars.gLawnApp.PlaySample(Resources.SOUND_PLANT);
				return;
			}
		}
		Hint("房间里放不下了");
	}

	public override bool BackButtonPress()
	{
		if (mBuy != null)
		{
			mBuy = null;
		}
		else if (mScreen != Screen.Room)
		{
			LeavePage();
		}
		else if (mConfirm)
		{
			mConfirm = false;
		}
		else if (mEdit)
		{
			RequestExitEdit();
		}
		else
		{
			Close();
		}
		return true;
	}

	private void Close()
	{
		try
		{
			if (mPrevMusic != MusicTune.None && mLawn.mMusic.mCurMusicTune == MusicTune.ZenGarden)
			{
				mLawn.mMusic.MakeSureMusicIsPlaying(mPrevMusic);
			}
			mPrevMusic = MusicTune.None;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm music: " + ex.Message);
		}
		mLawn.KillDialog(DIALOG_ID);
	}

#if !__IOS__
	// ------------------------------------------------------------------ desktop autotest hooks

	private int mTestX;

	private int mTestY;

	internal void TestPetFirst()
	{
		if (mActors.Count > 0)
		{
			Pet(mActors[0]);
		}
	}

	internal void TestButton(string id)
	{
		Layout();
		Ui u = mUi.FirstOrDefault((Ui t) => t.Id == id);
		if (u != null)
		{
			Tap(u.Rect.mX + u.Rect.mWidth / 2, u.Rect.mY + u.Rect.mHeight / 2);
		}
		else
		{
			Console.WriteLine("DORMTEST no button " + id);
		}
	}

	internal void TestSelect(string id)
	{
		mSel = mItems.FirstOrDefault((Item t) => t.Def.Id == id);
	}

	internal void TestBeginDrag(string id)
	{
		Item it = mItems.FirstOrDefault((Item t) => t.Def.Id == id);
		if (it == null)
		{
			return;
		}
		TRect r = ItemRect(it);
		mTestX = r.mX + r.mWidth / 2;
		mTestY = r.mY + r.mHeight * 3 / 4;
		MouseDown(mTestX, mTestY, 1);
	}

	internal void TestDrag(int k)
	{
		float t = Math.Min(1f, k / 60f);
		MouseDrag((int)(mTestX + t * mCW), (int)(mTestY + t * mCH));
	}

	internal void TestEndDrag()
	{
		MouseUp((int)(mTestX + mCW), (int)(mTestY + mCH), 1);
	}

	/// <summary>Pick girl #index up by her chest and carry her over the futon (stage 0 = press, 1..60 = move).</summary>
	internal void TestCarry(int index, int stage)
	{
		if (index >= mActors.Count)
		{
			return;
		}
		DormActor a = mActors[index];
		if (stage == 0)
		{
			mTestX = a.LastRect.mX + a.LastRect.mWidth / 2;
			mTestY = a.LastRect.mY + a.LastRect.mHeight / 3;
			MouseDown(mTestX, mTestY, 1);
			return;
		}
		Item futon = mItems.FirstOrDefault((Item t) => t.Def.Id == "futon");
		if (futon == null)
		{
			return;
		}
		float ti = futon.I + futon.A / 2f, tj = futon.J + futon.B / 2f;
		float tx = SX(ti, tj), ty = SY(ti, tj) - HeldHeight * 0.75f;
		float t = Math.Min(1f, stage / 50f);
		MouseDrag((int)(mTestX + (tx - mTestX) * t), (int)(mTestY + (ty - mTestY) * t));
	}

	internal void TestDrop()
	{
		MouseUp(mLastX, mLastY, 1);
	}
#endif
}
