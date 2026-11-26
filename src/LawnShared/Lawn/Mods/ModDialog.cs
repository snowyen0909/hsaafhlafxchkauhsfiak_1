// 「模组」 screen (mod edition): the plant-mod packs found in Documents/mods/植物模组, each switched on or off.
// Packs are read once when the game starts, so a change shows after a restart (like Minecraft mods).
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed class ModDialog : LawnDialog
{
	public const int DIALOG_ID = 9044;

	private sealed class Btn
	{
		public string Id;

		public string Text;

		public TRect Rect;

		public bool Enabled = true;
	}

	private static readonly Color Bg = new Color(10, 14, 12, 255);

	private static readonly Color Panel = new Color(22, 30, 26, 235);

	private static readonly Color Border = new Color(104, 160, 96);

	private static readonly Color Row = new Color(32, 42, 36, 220);

	private static readonly Color RowSel = new Color(58, 88, 62, 235);

	private static readonly Color Text = new Color(236, 244, 232);

	private static readonly Color Muted = new Color(160, 180, 160);

	private static readonly Color On = new Color(150, 226, 120);

	private static readonly Color Off = new Color(190, 120, 110);

	private static readonly Color Warn = new Color(244, 196, 104);

	private static readonly Color BtnFill = new Color(60, 104, 64, 240);

	private static readonly Color BtnOff = new Color(48, 54, 50, 200);

	private readonly LawnApp mLawn;

	private readonly Font mFont;

	private readonly Font mBig;

	private readonly List<Btn> mButtons = new List<Btn>();

	private int mSel;

	private float mScroll;

	private int mRowH;

	private TRect mListRect;

	private TRect mInfoRect;

	private int mDownX;

	private int mDownY;

	private int mLastY;

	private bool mDragged;

	public static void Open(LawnApp app)
	{
		if (app.GetDialog(DIALOG_ID) != null)
		{
			return;
		}
		ModPlants.Scan();
		ModDialog d = new ModDialog(app);
		Sexy.ScreenScales ss = app.mScreenScales;
		int w = Math.Min(Constants.BOARD_WIDTH - 40, ss.VisibleWidth - 16);
		int h = Math.Min(Constants.BOARD_HEIGHT - 20, ss.VisibleHeight - 16);
		int x = ss.VisibleLeft + (ss.VisibleWidth - w) / 2;
		int y = Math.Max(ss.VisibleTop + 8, (Constants.BOARD_HEIGHT - h) / 2);
		d.Resize(x, y, w, h);
		app.AddDialog(DIALOG_ID, d);
		app.mWidgetManager.SetFocus(d);
	}

	private ModDialog(LawnApp app)
		: base(app, null, DIALOG_ID, isModal: true, "模组", string.Empty, string.Empty, 0)
	{
		mLawn = app;
		mFont = Resources.FONT_BRIANNETOD12 ?? Resources.FONT_DWARVENTODCRAFT12;
		mBig = Resources.FONT_DWARVENTODCRAFT15 ?? mFont;
		mWantsFocus = true;
		mClip = false;
	}

	private ModInfo Selected => mSel >= 0 && mSel < ModPlants.Mods.Count ? ModPlants.Mods[mSel] : null;

	private void Layout()
	{
		mRowH = Math.Max(30, mFont.GetHeight() * 2 + 12);
		int pad = 12;
		int top = mBig.GetHeight() + 24;
		int bottom = mHeight - mFont.GetHeight() * 2 - 22;
		int lw = (int)((mWidth - pad * 2) * 0.42f);
		mListRect = new TRect(pad, top, lw, bottom - top);
		mInfoRect = new TRect(pad + lw + 8, top, mWidth - pad * 2 - lw - 8, bottom - top);
		mButtons.Clear();
		int bh = mFont.GetHeight() + 16;
		int bw = Math.Max(96, mFont.StringWidth("关闭") + 40);
		mButtons.Add(new Btn { Id = "close", Text = "关闭", Rect = new TRect(mWidth - pad - bw, 8, bw, bh) });
		ModInfo m = Selected;
		int tw = Math.Max(140, mFont.StringWidth("停用这个模组") + 30);
		mButtons.Add(new Btn
		{
			Id = "toggle",
			Text = m == null ? "启用" : (m.Enabled ? "停用这个模组" : "启用这个模组"),
			Rect = new TRect(mInfoRect.mX + 10, mInfoRect.mY + mInfoRect.mHeight - bh - 10, tw, bh),
			Enabled = m != null && m.Error == null
		});
	}

	public override void Draw(Graphics g)
	{
		Layout();
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(Bg);
		g.FillRect(0, 0, mWidth, mHeight);
		g.SetColor(Border);
		g.DrawRect(0, 0, mWidth - 1, mHeight - 1);
		TextAt(g, mBig, "模组", 14, 12, Text);
		int on = 0;
		foreach (ModInfo m in ModPlants.Mods)
		{
			on += m.Enabled ? 1 : 0;
		}
		string sub = ModPlants.Mods.Count + " 个模组 · 启用 " + on + " · 本次已载入 " + ModPlants.Count + " 种植物";
		TextAt(g, mFont, sub, 22 + mBig.StringWidth("模组"), 14 + (mBig.GetHeight() - mFont.GetHeight()) / 2, Muted);
		DrawList(g);
		DrawInfo(g);
		foreach (Btn b in mButtons)
		{
			DrawButton(g, b);
		}
		int fy = mHeight - mFont.GetHeight() * 2 - 14;
		if (ModPlants.RestartNeeded)
		{
			TextAt(g, mFont, "已更改：重开游戏后生效", 14, fy, Warn);
		}
		else
		{
			TextAt(g, mFont, Fit("模组位置：文件 App → 我的 iPhone → 本游戏 → mods/" + ModPlants.FolderName + "（放 .zip 或文件夹）", mWidth - 28), 14, fy, Muted);
		}
		TextAt(g, mFont, Fit("模组只在游戏启动时载入：加入、删除、启用或停用模组后都要重开游戏", mWidth - 28), 14, fy + mFont.GetHeight() + 4, Muted);
	}

	private void DrawList(Graphics g)
	{
		PanelBox(g, mListRect);
		g.SetClipRect(mListRect.mX + 1, mListRect.mY + 1, mListRect.mWidth - 2, mListRect.mHeight - 2);
		IReadOnlyList<ModInfo> mods = ModPlants.Mods;
		if (mods.Count == 0)
		{
			TextAt(g, mFont, "还没有模组", mListRect.mX + 12, mListRect.mY + 12, Muted);
			TextAt(g, mFont, Fit("把模组放进 mods/" + ModPlants.FolderName + " 再重开游戏", mListRect.mWidth - 24), mListRect.mX + 12, mListRect.mY + 18 + mFont.GetHeight(), Muted);
		}
		for (int i = 0; i < mods.Count; i++)
		{
			int y = mListRect.mY + 4 + i * (mRowH + 4) - (int)mScroll;
			if (y + mRowH < mListRect.mY || y > mListRect.mY + mListRect.mHeight)
			{
				continue;
			}
			ModInfo m = mods[i];
			TRect r = new TRect(mListRect.mX + 4, y, mListRect.mWidth - 8, mRowH);
			g.SetColor(i == mSel ? RowSel : Row);
			g.FillRect(r);
			g.SetColor(m.Error != null ? Off : (m.Enabled ? On : BtnOff));
			g.FillRect(r.mX, r.mY, 5, r.mHeight);
			string state = m.Error != null ? "有问题" : (m.Enabled ? "启用" : "停用");
			int sw = mFont.StringWidth(state);
			TextAt(g, mFont, Fit(m.Name, r.mWidth - sw - 30), r.mX + 12, r.mY + 5, Text);
			TextAt(g, mFont, state, r.mX + r.mWidth - 10 - sw, r.mY + 5, m.Error != null ? Off : (m.Enabled ? On : Muted));
			string line2 = m.Plants.Count + " 种植物" + (m.Author.Length > 0 ? " · 作者 " + m.Author : string.Empty) + (m.Version.Length > 0 ? " · v" + m.Version : string.Empty);
			TextAt(g, mFont, Fit(line2, r.mWidth - 20), r.mX + 12, r.mY + 8 + mFont.GetHeight(), Muted);
		}
		g.ClearClipRect();
	}

	private void DrawInfo(Graphics g)
	{
		PanelBox(g, mInfoRect);
		ModInfo m = Selected;
		if (m == null)
		{
			return;
		}
		int x = mInfoRect.mX + 10;
		int w = mInfoRect.mWidth - 20;
		int y = mInfoRect.mY + 8;
		int lh = mFont.GetHeight() + 4;
		TextAt(g, mFont, Fit(m.Name, w), x, y, Text);
		y += lh;
		TextAt(g, mFont, Fit("文件：" + m.Source, w), x, y, Muted);
		y += lh;
		if (m.Error != null)
		{
			TextAt(g, mFont, Fit("问题：" + m.Error, w), x, y, Off);
			y += lh;
		}
		else if (m.Enabled != m.Loaded)
		{
			TextAt(g, mFont, m.Enabled ? "重开游戏后启用" : "重开游戏后停用", x, y, Warn);
			y += lh;
		}
		foreach (string line in Wrap(m.Description, w, 3))
		{
			TextAt(g, mFont, line, x, y, Muted);
			y += lh;
		}
		// the pack's plants: card, name, cost
		y += 6;
		int cardW = 96;
		int cardH = 126;
		int cx = x;
		int bottom = mInfoRect.mY + mInfoRect.mHeight - mFont.GetHeight() - 36;
		foreach (ModPlantSpec p in m.Plants)
		{
			if (cx + cardW > x + w)
			{
				cx = x;
				y += cardH + lh * 2 + 8;
			}
			if (y + cardH > bottom)
			{
				break;
			}
			if (p.Card != null)
			{
				Image card = p.Card;
				float k = Math.Min((float)cardW / card.mWidth, (float)cardH / card.mHeight);
				Sexy.TodLib.TodCommon.TodDrawImageScaledF(g, card, cx + (cardW - card.mWidth * k) / 2f, y, k, k);
			}
			else
			{
				g.SetColor(Row);
				g.FillRect(cx, y, cardW, cardH);
			}
			string label = Fit(p.Name, cardW + 36);
			TextAt(g, mFont, label, cx + (cardW - mFont.StringWidth(label)) / 2, y + cardH + 2, Text);
			string cost = p.Cost + " 阳光";
			TextAt(g, mFont, cost, cx + (cardW - mFont.StringWidth(cost)) / 2, y + cardH + 4 + mFont.GetHeight(), Muted);
			cx += cardW + 44;
		}
	}

	private IEnumerable<string> Wrap(string s, int width, int maxLines)
	{
		if (string.IsNullOrEmpty(s))
		{
			yield break;
		}
		int lines = 0;
		foreach (string para in s.Split('\n'))
		{
			string rest = para;
			while (rest.Length > 0 && lines < maxLines)
			{
				int n = rest.Length;
				while (n > 1 && mFont.StringWidth(rest.Substring(0, n)) > width)
				{
					n--;
				}
				lines++;
				string line = rest.Substring(0, n);
				rest = rest.Substring(n);
				if (lines == maxLines && rest.Length > 0)
				{
					line = Fit(line + rest, width);
				}
				yield return line;
			}
		}
	}

	private void DrawButton(Graphics g, Btn b)
	{
		g.SetColor(b.Enabled ? BtnFill : BtnOff);
		g.FillRect(b.Rect);
		g.SetColor(Border);
		g.DrawRect(b.Rect);
		int tw = mFont.StringWidth(b.Text);
		TextAt(g, mFont, b.Text, b.Rect.mX + (b.Rect.mWidth - tw) / 2, b.Rect.mY + (b.Rect.mHeight - mFont.GetHeight()) / 2, b.Enabled ? Text : Muted);
	}

	private static void PanelBox(Graphics g, TRect r)
	{
		g.SetColor(Panel);
		g.FillRect(r);
		g.SetColor(Border);
		g.DrawRect(r);
	}

	private static void TextAt(Graphics g, Font f, string s, int x, int y, Color c)
	{
		g.SetFont(f);
		g.SetColor(c);
		g.DrawString(s ?? string.Empty, x, y);
	}

	private string Fit(string s, int width)
	{
		if (string.IsNullOrEmpty(s) || mFont.StringWidth(s) <= width)
		{
			return s ?? string.Empty;
		}
		int n = s.Length;
		while (n > 1 && mFont.StringWidth(s.Substring(0, n) + "…") > width)
		{
			n--;
		}
		return s.Substring(0, n) + "…";
	}

	// ------------------------------------------------------------------ input

	public override void MouseDown(int x, int y, int theClickCount)
	{
		mDownX = x;
		mDownY = y;
		mLastY = y;
		mDragged = false;
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		MouseDown(x, y, theClickCount);
	}

	public override void MouseDrag(int x, int y)
	{
		if (Math.Abs(y - mDownY) > 10 || Math.Abs(x - mDownX) > 10)
		{
			mDragged = true;
		}
		int dy = y - mLastY;
		mLastY = y;
		if (Contains(mListRect, mDownX, mDownY))
		{
			mScroll = ClampScroll(mScroll - dy);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (!mDragged)
		{
			Tap(x, y);
		}
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		MouseUp(x, y, theClickCount);
	}

	public override void MouseWheel(int theDelta)
	{
		mScroll = ClampScroll(mScroll - theDelta * 40);
	}

	private float ClampScroll(float v)
	{
		return Math.Clamp(v, 0f, Math.Max(0, ModPlants.Mods.Count * (mRowH + 4) + 8 - mListRect.mHeight));
	}

	private static bool Contains(TRect r, int x, int y)
	{
		return x >= r.mX && y >= r.mY && x < r.mX + r.mWidth && y < r.mY + r.mHeight;
	}

	/// <summary>Taps in dialog coordinates (also used by the desktop test).</summary>
	public void Tap(int x, int y)
	{
		foreach (Btn b in mButtons)
		{
			if (Contains(b.Rect, x, y))
			{
				if (b.Enabled)
				{
					mApp.PlaySample(Resources.SOUND_TAP);
					OnButton(b.Id);
				}
				return;
			}
		}
		if (Contains(mListRect, x, y))
		{
			int i = (int)((y - mListRect.mY - 4 + mScroll) / (mRowH + 4));
			if (i >= 0 && i < ModPlants.Mods.Count)
			{
				mSel = i;
			}
		}
	}

	public void OnButton(string id)
	{
		switch (id)
		{
		case "close":
			mLawn.KillDialog(DIALOG_ID);
			break;
		case "toggle":
			if (Selected != null)
			{
				ModPlants.SetEnabled(Selected, !Selected.Enabled);
			}
			break;
		}
	}

	public override void Update()
	{
		base.Update();
		MarkDirty();
	}

	public override bool BackButtonPress()
	{
		mLawn.KillDialog(DIALOG_ID);
		return true;
	}
}
