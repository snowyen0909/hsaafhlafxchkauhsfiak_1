// 宿舍 pages next to the room: 家具商店 (buy furniture with coins, test price $100 each, pieces go to the
// warehouse) and 角色管理 (choose which plant girls live in the dorm, at most MaxGirls).
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	private enum Screen
	{
		Room,
		Shop,
		Roster
	}

	private Screen mScreen = Screen.Room;

	private TRect mPagePanel;

	private int mShopTab;

	private int mShopPage;

	private int mShopPages = 1;

	/// <summary>Portrait grid: area, scroll offset (pixels), content height, finger drag in progress.</summary>
	private TRect mRosterGrid;

	private float mRosterScroll;

	private float mRosterScroll0;

	private int mRosterContentH;

	private bool mRosterDragging;

	/// <summary>Piece waiting for the purchase confirmation.</summary>
	private Def mBuy;

	private TRect mBuyPanel;

	/// <summary>Girl shown large in 角色管理 (skeleton name) and her preview skeleton.</summary>
	private string mRosterSel;

	private DormActor mRosterPreview;

	private TRect mRosterBox;

	private void OpenPage(Screen theScreen)
	{
		mScreen = theScreen;
		mSel = null;
		mDrag = null;
		mBuy = null;
		if (theScreen == Screen.Roster && mRosterSel == null)
		{
			SelectRosterGirl(mChosen.FirstOrDefault() ?? Girls[0].Skel);
		}
	}

	private void LeavePage()
	{
		mScreen = Screen.Room;
		mBuy = null;
	}

	private void SelectRosterGirl(string skel)
	{
		if (skel == mRosterSel && mRosterPreview != null)
		{
			return;
		}
		mRosterSel = skel;
		mRosterPreview?.Dispose();
		mRosterPreview = null;
		GirlDef def = FindGirl(skel);
		if (def != null)
		{
			mRosterPreview = DormActor.TryCreate(def.Seed, def.Skel, def.Hide);
		}
	}

	private void UpdatePages(float dt)
	{
		if (mScreen == Screen.Roster)
		{
			mRosterPreview?.Advance(dt);
		}
	}

	// ------------------------------------------------------------------ layout

	private void PagerArrows(string prefix, TRect panel, int page, int pages)
	{
		if (pages <= 1)
		{
			return;
		}
		int aw = 34, ah = 44;
		AddUi(prefix + "_prev", UiKind.Arrow, null, new TRect(panel.mX + panel.mWidth - 2 * aw - 70, panel.mY + panel.mHeight - ah - 6, aw, ah), "ui/ic_left", enabled: page > 0);
		AddUi(prefix + "_next", UiKind.Arrow, null, new TRect(panel.mX + panel.mWidth - aw - 10, panel.mY + panel.mHeight - ah - 6, aw, ah), "ui/ic_right", enabled: page < pages - 1);
	}

	private void LayoutShop(TRect panel)
	{
		mPagePanel = panel;
		int tabH = 34, top = panel.mY + 30;
		int tw = Math.Min(Math.Max(96, mFont.StringWidth("主题小物") + 20), (panel.mWidth - 24) / Tabs.Length - 2);
		for (int k = 0; k < Tabs.Length; k++)
		{
			AddUi("stab" + k, UiKind.Tab, Tabs[k], new TRect(panel.mX + 12 + k * (tw + 2), top, tw, tabH), on: k == mShopTab);
		}
		List<Def> defs = Catalog.Where((Def d) => mShopTab == 0 || d.Tab == mShopTab).ToList();
		int gx = panel.mX + 12, gy = top + tabH + 10, gw = panel.mWidth - 24, gh = panel.mY + panel.mHeight - 56 - gy;
		int gap = 8;
		int cw = Math.Max(96, Math.Min(132, (gw - 4 * gap) / 5));
		int ch = Math.Min(150, Math.Max(110, gh / 2 - gap));
		int cols = Math.Max(1, (gw + gap) / (cw + gap)), rows = Math.Max(1, (gh + gap) / (ch + gap));
		int per = cols * rows;
		mShopPages = Math.Max(1, (defs.Count + per - 1) / per);
		mShopPage = Math.Clamp(mShopPage, 0, mShopPages - 1);
		int ox = gx + (gw - (cols * cw + (cols - 1) * gap)) / 2;
		int idx = 0;
		foreach (Def d in defs.Skip(mShopPage * per).Take(per))
		{
			int r = idx / cols, c = idx % cols;
			AddUi("shopcard", UiKind.Shop, d.Title, new TRect(ox + c * (cw + gap), gy + r * (ch + gap), cw, ch), def: d);
			idx++;
		}
		PagerArrows("s", panel, mShopPage, mShopPages);
	}

	private void LayoutRoster(TRect panel)
	{
		mPagePanel = panel;
		int boxW = Math.Min(300, Math.Max(200, panel.mWidth * 36 / 100));
		mRosterBox = new TRect(panel.mX + 12, panel.mY + 30, boxW, panel.mHeight - 42);
		bool inDorm = mRosterSel != null && mChosen.Contains(mRosterSel);
		int bw = boxW - 40, bh = 42;
		AddUi("r_toggle", UiKind.Dialog, inDorm ? "让她离开宿舍" : "放进宿舍", new TRect(mRosterBox.mX + 20, mRosterBox.mY + mRosterBox.mHeight - bh - 12, bw, bh), on: !inDorm);
		int gx = mRosterBox.mX + boxW + 14, gy = panel.mY + 30, gw = panel.mX + panel.mWidth - 22 - gx, gh = panel.mY + panel.mHeight - 14 - gy;
		mRosterGrid = new TRect(gx, gy, gw + 10, gh);
		int gap = 6;
		int cw = Math.Max(76, Math.Min(104, (gw - 5 * gap) / 6));
		int ch = cw + mFont.GetHeight() + 6;
		int cols = Math.Max(1, (gw + gap) / (cw + gap));
		int rowsAll = (Girls.Length + cols - 1) / cols;
		// a scrolling list: drag up / down
		mRosterContentH = rowsAll * (ch + gap) - gap + 8;
		mRosterScroll = Math.Clamp(mRosterScroll, 0f, Math.Max(0f, mRosterContentH - gh));
		int ox = gx + (gw - (cols * cw + (cols - 1) * gap)) / 2;
		for (int idx = 0; idx < Girls.Length; idx++)
		{
			int r = idx / cols, c = idx % cols;
			TRect cell = new TRect(ox + c * (cw + gap), gy + 4 + r * (ch + gap) - (int)mRosterScroll, cw, ch);
			if (cell.mY + cell.mHeight < gy || cell.mY > gy + gh)
			{
				continue;
			}
			AddUi("face", UiKind.Face, Girls[idx].Skel, cell, on: Girls[idx].Skel == mRosterSel);
		}
	}

	private void LayoutBuyConfirm()
	{
		if (mBuy == null)
		{
			return;
		}
		int cw = Math.Min(440, mWidth - 40), ch = 270;
		mBuyPanel = new TRect((mWidth - cw) / 2, (mHeight - ch) / 2, cw, ch);
		int bw = (cw - 3 * 20) / 2, bh = 46, by = mBuyPanel.mY + ch - bh - 18;
		AddUi("b_cancel", UiKind.Dialog, "取消", new TRect(mBuyPanel.mX + 20, by, bw, bh));
		AddUi("b_buy", UiKind.Dialog, "购买", new TRect(mBuyPanel.mX + 40 + bw, by, bw, bh), on: true);
	}

	/// <summary>Page buttons that are not handled by the shared switch in Button().</summary>
	private bool PageButton(Ui u)
	{
		switch (u.Id)
		{
		case "s_prev":
			mShopPage = Math.Max(0, mShopPage - 1);
			return true;
		case "s_next":
			mShopPage = Math.Min(mShopPages - 1, mShopPage + 1);
			return true;
		}
		if (u.Id.StartsWith("stab", StringComparison.Ordinal) && int.TryParse(u.Id.Substring(4), out int t))
		{
			mShopTab = t;
			mShopPage = 0;
			return true;
		}
		return false;
	}

	// ------------------------------------------------------------------ drawing

	private void DrawPageBack(Graphics g)
	{
		g.SetColor(new Color(244, 230, 214));
		g.FillRect(0, 0, mWidth, mHeight);
	}

	private void DrawPanelTitle(Graphics g, TRect panel, string title)
	{
		Draw9(g, "ui/panel", panel, 26);
		Flowers(g, panel);
		int tw = mFont.StringWidth(title) + 56;
		TRect tr = new TRect(panel.mX + 18, panel.mY - 18, tw, 42);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
	}

	private void DrawCoins(Graphics g)
	{
		int coins = mLawn.mPlayerInfo?.mCoins ?? 0;
		string s = LawnApp.GetMoneyString(coins);
		int w = mBig.StringWidth(s) + 70;
		TRect r = new TRect(mWidth - Pad - w, Pad + 6, w, 42);
		Draw9(g, "ui/pill_dark", r, 20);
		Image sign = AtlasResources.IMAGE_ZEN_MONEYSIGN;
		if (sign != null)
		{
			float k = 30f / sign.mHeight;
			g.SetColorizeImages(colorizeImages: false);
			g.DrawImage(sign, new TRect(r.mX + 10, r.mY + 6, (int)(sign.mWidth * k), 30), new TRect(0, 0, sign.mWidth, sign.mHeight));
		}
		Text(g, mBig, s, r.mX + r.mWidth - 14 - mBig.StringWidth(s), r.mY + (r.mHeight - mBig.GetHeight()) / 2, new Color(255, 230, 120));
	}

	private void DrawShop(Graphics g)
	{
		DrawPanelTitle(g, mPagePanel, "家具商店");
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Shop))
		{
			Draw9(g, "ui/card", u.Rect, 18);
			Def d = u.Def;
			int nameH = mFont.GetHeight();
			int priceH = nameH + 8;
			Image img = Img(d.Id + "_0");
			if (img != null)
			{
				int box = u.Rect.mWidth - 20, boxH = u.Rect.mHeight - nameH - priceH - 22;
				float k = Math.Min((float)box / img.mWidth, (float)boxH / img.mHeight);
				float w = img.mWidth * k, h = img.mHeight * k;
				DrawSprite(g, d.Id + "_0", u.Rect.mX + (u.Rect.mWidth - w) / 2f, u.Rect.mY + 8 + (boxH - h) / 2f, w, h);
			}
			string t = d.Title;
			while (t.Length > 1 && mFont.StringWidth(t) > u.Rect.mWidth - 8)
			{
				t = t.Substring(0, t.Length - 1);
			}
			Text(g, mFont, t, u.Rect.mX + (u.Rect.mWidth - mFont.StringWidth(t)) / 2, u.Rect.mY + u.Rect.mHeight - priceH - nameH - 8, InkBrown);
			TRect pr = new TRect(u.Rect.mX + 8, u.Rect.mY + u.Rect.mHeight - priceH - 6, u.Rect.mWidth - 16, priceH);
			Draw9(g, "ui/tab", pr, 12);
			TextCentered(g, mFont, LawnApp.GetMoneyString(Price), pr, new Color(170, 110, 20), -1);
			int own = Owned(d);
			if (own > 0)
			{
				string o = "持有 " + own;
				int bw = mFont.StringWidth(o) + 20;
				TRect badge = new TRect(u.Rect.mX + u.Rect.mWidth - bw - 4, u.Rect.mY + 4, bw, nameH + 4);
				Draw9(g, "ui/pill_dark", badge, 10);
				TextCentered(g, mFont, o, badge, Color.White);
			}
		}
	}

	private void DrawFace(Graphics g, Ui u)
	{
		GirlDef def = FindGirl(u.Text);
		bool inDorm = mChosen.Contains(u.Text);
		Draw9(g, u.On ? "ui2/card_sel" : "ui/card", u.Rect, 18);
		int pic = u.Rect.mWidth - 10;
		// portrait: Content/dorm/face/<skel>.png (head crop of her idle pose)
		DrawSprite(g, "face/" + u.Text, u.Rect.mX + 5, u.Rect.mY + 5, pic, pic);
		string name = def?.Name ?? u.Text;
		while (name.Length > 1 && mFont.StringWidth(name) > u.Rect.mWidth - 4)
		{
			name = name.Substring(0, name.Length - 1);
		}
		Text(g, mFont, name, u.Rect.mX + (u.Rect.mWidth - mFont.StringWidth(name)) / 2, u.Rect.mY + u.Rect.mHeight - mFont.GetHeight() - 4, InkBrown);
		if (inDorm)
		{
			DrawSprite(g, "heart", u.Rect.mX + u.Rect.mWidth - 28, u.Rect.mY + 2, 26, 26);
		}
	}

	private void DrawRoster(Graphics g)
	{
		DrawPanelTitle(g, mPagePanel, "角色管理");
		TRect box = mRosterBox;
		Draw9(g, "ui/card", box, 22);
		string count = "宿舍成员 " + mChosen.Count + "/" + MaxGirls;
		Text(g, mFont, count, box.mX + 14, box.mY + 10, InkBrown);
		GirlDef def = mRosterSel != null ? FindGirl(mRosterSel) : null;
		if (def != null)
		{
			int nameY = box.mY + box.mHeight - 42 - 12 - 46;
			if (mRosterPreview != null)
			{
				float h = Math.Min(nameY - box.mY - 40, box.mWidth * 1.15f);
				// floor shadow, then the girl standing on it
				g.SetColor(new Color(120, 90, 60, 50));
				g.FillRect(box.mX + box.mWidth / 2 - 50, nameY - 12, 100, 6);
				mRosterPreview.Draw(g, box.mX + box.mWidth / 2f, nameY - 10, h);
			}
			int nw = mFont.StringWidth(def.Name) + 50;
			TRect nr = new TRect(box.mX + (box.mWidth - nw) / 2, nameY, nw, 40);
			Draw9(g, "ui/panel", nr, 20);
			TextCentered(g, mFont, def.Name, nr, InkBrown, -2);
			if (mChosen.Contains(def.Skel))
			{
				DrawSprite(g, "heart", nr.mX - 32, nr.mY + 4, 30, 30);
			}
		}
		// portraits scroll inside their area
		TRect oldClip = g.GetClipRect();
		g.ClipRect(mRosterGrid);
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Face))
		{
			DrawFace(g, u);
		}
		g.SetClipRect(oldClip.mX - (int)g.mTransX, oldClip.mY - (int)g.mTransY, oldClip.mWidth, oldClip.mHeight);
		if (mRosterContentH > mRosterGrid.mHeight)
		{
			// scroll bar
			int barH = Math.Max(30, mRosterGrid.mHeight * mRosterGrid.mHeight / mRosterContentH);
			int barY = mRosterGrid.mY + (int)((mRosterGrid.mHeight - barH) * mRosterScroll / Math.Max(1, mRosterContentH - mRosterGrid.mHeight));
			g.SetColor(new Color(150, 110, 70, 60));
			g.FillRect(mRosterGrid.mX + mRosterGrid.mWidth - 6, mRosterGrid.mY, 5, mRosterGrid.mHeight);
			g.SetColor(new Color(150, 100, 60, 200));
			g.FillRect(mRosterGrid.mX + mRosterGrid.mWidth - 6, barY, 5, barH);
		}
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Dialog && t.Id == "r_toggle"))
		{
			DrawUi(g, u);
		}
	}

	private void DrawBuyConfirm(Graphics g)
	{
		g.SetColor(new Color(40, 25, 15, 140));
		g.FillRect(0, 0, mWidth, mHeight);
		TRect p = mBuyPanel;
		Draw9(g, "ui/panel", p, 26);
		Flowers(g, p);
		string title = "购买家具";
		int tw = mFont.StringWidth(title) + 60;
		TRect tr = new TRect(p.mX + (p.mWidth - tw) / 2, p.mY - 20, tw, 44);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
		Image img = Img(mBuy.Id + "_0");
		int box = 110;
		if (img != null)
		{
			float k = Math.Min((float)box / img.mWidth, (float)box / img.mHeight);
			DrawSprite(g, mBuy.Id + "_0", p.mX + 30 + (box - img.mWidth * k) / 2f, p.mY + 34 + (box - img.mHeight * k) / 2f, img.mWidth * k, img.mHeight * k);
		}
		int tx = p.mX + 30 + box + 20;
		Text(g, mFont, mBuy.Title, tx, p.mY + 44, InkBrown);
		Text(g, mFont, "价格  " + LawnApp.GetMoneyString(Price), tx, p.mY + 84, new Color(170, 110, 20));
		Text(g, mFont, "持有  " + Owned(mBuy) + " 件", tx, p.mY + 110, InkBrown);
		int coins = mLawn.mPlayerInfo?.mCoins ?? 0;
		Text(g, mFont, "金币  " + LawnApp.GetMoneyString(coins), tx, p.mY + 136, coins >= Price ? InkBrown : InkWarn);
		foreach (Ui u in mUi.Where((Ui t) => t.Kind == UiKind.Dialog && t.Id.StartsWith("b_", StringComparison.Ordinal)))
		{
			DrawUi(g, u);
		}
	}

#if !__IOS__
	internal void TestOpenBuy(string id) => mBuy = FindDef(id);

	internal void TestBuy(string id)
	{
		Def d = FindDef(id);
		if (d != null)
		{
			Buy(d);
		}
	}

	internal void TestAdd(string id)
	{
		Def d = FindDef(id);
		if (d != null && Available(d) > 0)
		{
			AddNew(d);
		}
	}

	internal void TestScroll(float y) => mRosterScroll = y;

	internal void TestRoster(string skel)
	{
		SelectRosterGirl(skel);
	}

	internal void TestPet(int index)
	{
		if (index < mActors.Count)
		{
			Pet(mActors[index]);
		}
	}
#endif
}
