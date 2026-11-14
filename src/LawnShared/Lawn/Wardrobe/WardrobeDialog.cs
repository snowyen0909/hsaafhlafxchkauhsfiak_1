// 时装柜 screen for the iOS mod edition (own implementation; layout follows the Android mod's three columns).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sexy;

namespace Lawn;

internal sealed class WardrobeDialog : LawnDialog
{
	public const int DIALOG_ID = 9043;

	private sealed class Group
	{
		public string Key;

		public string Title;

		public int Order;

		public readonly List<WardrobePack> Packs = new List<WardrobePack>();
	}

	private sealed class Btn
	{
		public string Id;

		public string Text;

		public TRect Rect;

		public bool Enabled = true;
	}

	/// <summary>Loads a single loose .xnb file (preview only).</summary>
	private sealed class FileContentManager : ContentManager
	{
		private readonly string mFile;

		public FileContentManager(IServiceProvider services, string file)
			: base(services)
		{
			mFile = file;
		}

		protected override Stream OpenStream(string assetName)
		{
			return File.OpenRead(mFile);
		}
	}

	private static readonly Color Bg = new Color(10, 12, 18, 245);

	private static readonly Color Panel = new Color(22, 26, 36, 235);

	private static readonly Color Border = new Color(124, 98, 168);

	private static readonly Color Row = new Color(34, 38, 52, 220);

	private static readonly Color RowSel = new Color(70, 58, 108, 235);

	private static readonly Color RowWorn = new Color(40, 72, 56, 230);

	private static readonly Color Text = new Color(238, 232, 245);

	private static readonly Color Muted = new Color(170, 162, 188);

	private static readonly Color Accent = new Color(150, 226, 140);

	private static readonly Color Warn = new Color(244, 186, 104);

	private static readonly Color BtnFill = new Color(78, 62, 120, 240);

	private static readonly Color BtnOff = new Color(50, 48, 60, 200);

	private readonly LawnApp mLawn;

	private readonly Font mFont;

	private readonly Font mBig;

	private readonly List<Group> mGroups = new List<Group>();

	private int mGroupSel;

	private int mPackSel;

	private float mGroupScroll;

	private float mPackScroll;

	private bool mPreviewOn;

	private int mPreviewPage;

	private Image mPreview;

	private ContentManager mPreviewContent;

	private string mPreviewKey;

	private WardrobeSpinePreview mSpine;

	private string mSpineKey;

	private readonly HashSet<string> mSpineFailed = new HashSet<string>();

	private string mStatus = string.Empty;

	private int mStatusTimer;

	private readonly List<Btn> mButtons = new List<Btn>();

	private int mDownX;

	private int mDownY;

	private int mLastY;

	private int mDragArea = -1;

	private bool mDragged;

	private int mRowH;

	private TRect mGroupRect;

	private TRect mPackRect;

	private TRect mPreviewRect;

	private TRect mInfoRect;

	public static void Open(LawnApp app)
	{
		if (app.GetDialog(DIALOG_ID) != null)
		{
			return;
		}
		WardrobeDialog d = new WardrobeDialog(app);
		int w = Constants.BOARD_WIDTH - 40;
		int h = Constants.BOARD_HEIGHT - 20;
		d.Resize(0, 0, w, h);
		LawnApp.CenterDialog(d, w, h);
		app.AddDialog(DIALOG_ID, d);
		app.mWidgetManager.SetFocus(d);
	}

	private WardrobeDialog(LawnApp app)
		: base(app, null, DIALOG_ID, isModal: true, "时装柜", string.Empty, string.Empty, 0)
	{
		mLawn = app;
		mFont = Resources.FONT_BRIANNETOD12 ?? Resources.FONT_DWARVENTODCRAFT12;
		mBig = Resources.FONT_DWARVENTODCRAFT15 ?? mFont;
		mWantsFocus = true;
		mClip = false;
		RebuildGroups();
		if (WardrobeService.Warnings.Count > 0)
		{
			SetStatus(WardrobeService.Warnings[0]);
		}
		else if (WardrobeService.Packs.Count == 0)
		{
			SetStatus("还没有补丁：用「文件」App 把补丁文件夹放进 mods/时装柜/patches，再点「刷新」");
		}
	}

	private void SetStatus(string s)
	{
		mStatus = s ?? string.Empty;
		mStatusTimer = 600;
	}

	private void RebuildGroups()
	{
		string keepKey = (mGroupSel >= 0 && mGroupSel < mGroups.Count) ? mGroups[mGroupSel].Key : null;
		mGroups.Clear();
		Group all = new Group { Key = "*", Title = "全部", Order = -2 };
		Group worn = new Group { Key = "+", Title = "已穿上", Order = -1 };
		Group global = new Group { Key = "global", Title = "全局 / 其他", Order = int.MaxValue };
		Dictionary<string, Group> byKey = new Dictionary<string, Group>(StringComparer.OrdinalIgnoreCase);
		foreach (WardrobePack p in WardrobeService.Packs)
		{
			all.Packs.Add(p);
			if (WardrobeService.IsWorn(p))
			{
				worn.Packs.Add(p);
			}
			if (p.IsGlobal)
			{
				global.Packs.Add(p);
				continue;
			}
			foreach (string k in p.PlantKeys)
			{
				if (!byKey.TryGetValue(k, out Group g))
				{
					int order = WardrobeService.ResolveSeed(k, out SeedType seed) ? (int)seed : 100000;
					g = byKey[k] = new Group { Key = k, Title = WardrobeService.PlantTitle(k), Order = order };
				}
				g.Packs.Add(p);
			}
		}
		mGroups.Add(all);
		mGroups.Add(worn);
		mGroups.AddRange(byKey.Values.OrderBy((Group g) => g.Order).ThenBy((Group g) => g.Title));
		if (global.Packs.Count > 0)
		{
			mGroups.Add(global);
		}
		mGroupSel = Math.Max(0, mGroups.FindIndex((Group g) => g.Key == keepKey));
		ClampSelection();
	}

	private List<WardrobePack> CurrentPacks()
	{
		return (mGroupSel >= 0 && mGroupSel < mGroups.Count) ? mGroups[mGroupSel].Packs : new List<WardrobePack>();
	}

	private WardrobePack SelectedPack()
	{
		List<WardrobePack> l = CurrentPacks();
		return (mPackSel >= 0 && mPackSel < l.Count) ? l[mPackSel] : null;
	}

	private void ClampSelection()
	{
		int n = CurrentPacks().Count;
		mPackSel = n == 0 ? -1 : Math.Clamp(mPackSel, 0, n - 1);
	}

	// ------------------------------------------------------------------ layout

	private void Layout()
	{
		mRowH = Math.Max(30, mFont.GetHeight() * 2 + 10);
		int pad = 12;
		int top = mBig.GetHeight() + 24;
		int bottom = mHeight - mFont.GetHeight() * 2 - 22;
		int w = mWidth - pad * 2;
		int gw = (int)(w * 0.22f);
		int pw = (int)(w * 0.38f);
		mGroupRect = new TRect(pad, top, gw, bottom - top);
		mPackRect = new TRect(pad + gw + 8, top, pw, bottom - top);
		int rx = mPackRect.mX + pw + 8;
		int rw = mWidth - pad - rx;
		int ph = (int)((bottom - top) * 0.62f);
		mPreviewRect = new TRect(rx, top, rw, ph);
		mInfoRect = new TRect(rx, top + ph + 6, rw, bottom - top - ph - 6);
		mButtons.Clear();
		int bh = mFont.GetHeight() + 16;
		int bw = Math.Max(96, mFont.StringWidth("预览：关") + 24);
		mButtons.Add(new Btn { Id = "close", Text = "关闭", Rect = new TRect(mWidth - pad - bw, 8, bw, bh) });
		mButtons.Add(new Btn { Id = "preview", Text = mPreviewOn ? "预览：开" : "预览：关", Rect = new TRect(mWidth - pad - bw * 2 - 8, 8, bw, bh) });
		WardrobePack sel = SelectedPack();
		int by = mInfoRect.mY + mInfoRect.mHeight - bh - 4;
		int third = (mInfoRect.mWidth - 16) / 3;
		mButtons.Add(new Btn { Id = "wear", Text = WardrobeService.IsWorn(sel) ? "脱下" : "穿上", Rect = new TRect(mInfoRect.mX, by, third, bh), Enabled = sel != null });
		mButtons.Add(new Btn { Id = "all", Text = "全部复原", Rect = new TRect(mInfoRect.mX + third + 8, by, third, bh), Enabled = WardrobeService.Worn.Count > 0 });
		mButtons.Add(new Btn { Id = "refresh", Text = "刷新", Rect = new TRect(mInfoRect.mX + (third + 8) * 2, by, third, bh) });
		if (mPreviewOn && sel != null && PageCount(sel) > 1)
		{
			int aw = bh + 8;
			mButtons.Add(new Btn { Id = "prev", Text = "<", Rect = new TRect(mPreviewRect.mX + 4, mPreviewRect.mY + mPreviewRect.mHeight - bh - 4, aw, bh) });
			mButtons.Add(new Btn { Id = "next", Text = ">", Rect = new TRect(mPreviewRect.mX + mPreviewRect.mWidth - aw - 4, mPreviewRect.mY + mPreviewRect.mHeight - bh - 4, aw, bh) });
		}
	}

	// ------------------------------------------------------------------ drawing

	public override void Draw(Graphics g)
	{
		Layout();
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(Bg);
		g.FillRect(0, 0, mWidth, mHeight);
		g.SetColor(Border);
		g.DrawRect(0, 0, mWidth - 1, mHeight - 1);
		TextAt(g, mBig, "时装柜", 14, 12, Text);
		string sub = WardrobeService.Packs.Count + " 个补丁 · 已穿 " + WardrobeService.Worn.Count;
		TextAt(g, mFont, sub, 22 + mBig.StringWidth("时装柜"), 14 + (mBig.GetHeight() - mFont.GetHeight()) / 2, Muted);
		DrawGroups(g);
		DrawPacks(g);
		DrawPreview(g);
		DrawInfo(g);
		foreach (Btn b in mButtons)
		{
			DrawButton(g, b);
		}
		int fy = mHeight - mFont.GetHeight() * 2 - 14;
		if (mStatusTimer > 0 && mStatus.Length > 0)
		{
			TextAt(g, mFont, Fit(mStatus, mWidth - 28), 14, fy, Warn);
		}
		else
		{
			TextAt(g, mFont, "补丁位置：文件 App → 我的 iPhone → 植物娘时装柜Mod版 → mods/时装柜/patches", 14, fy, Muted);
		}
		TextAt(g, mFont, "穿上 / 脱下立即生效（场上已种下的植物换骨骼要重种；reanim 动画、字体、粒子、音乐要重开游戏）", 14, fy + mFont.GetHeight() + 4, Muted);
	}

	private void DrawGroups(Graphics g)
	{
		PanelBox(g, mGroupRect);
		g.SetClipRect(mGroupRect.mX + 1, mGroupRect.mY + 1, mGroupRect.mWidth - 2, mGroupRect.mHeight - 2);
		int rh = mFont.GetHeight() + 18;
		for (int i = 0; i < mGroups.Count; i++)
		{
			int y = mGroupRect.mY + 4 + i * (rh + 4) - (int)mGroupScroll;
			if (y + rh < mGroupRect.mY || y > mGroupRect.mY + mGroupRect.mHeight)
			{
				continue;
			}
			TRect r = new TRect(mGroupRect.mX + 4, y, mGroupRect.mWidth - 8, rh);
			g.SetColor(i == mGroupSel ? RowSel : Row);
			g.FillRect(r);
			Group grp = mGroups[i];
			TextAt(g, mFont, Fit(grp.Title, r.mWidth - 46), r.mX + 8, r.mY + 9, Text);
			string n = grp.Packs.Count.ToString();
			TextAt(g, mFont, n, r.mX + r.mWidth - 8 - mFont.StringWidth(n), r.mY + 9, Muted);
		}
		g.ClearClipRect();
	}

	private int PackListHeight()
	{
		return CurrentPacks().Count * (mRowH + 4) + 8;
	}

	private void DrawPacks(Graphics g)
	{
		PanelBox(g, mPackRect);
		g.SetClipRect(mPackRect.mX + 1, mPackRect.mY + 1, mPackRect.mWidth - 2, mPackRect.mHeight - 2);
		List<WardrobePack> packs = CurrentPacks();
		if (packs.Count == 0)
		{
			TextAt(g, mFont, "这里还没有补丁", mPackRect.mX + 12, mPackRect.mY + 12, Muted);
		}
		for (int i = 0; i < packs.Count; i++)
		{
			int y = mPackRect.mY + 4 + i * (mRowH + 4) - (int)mPackScroll;
			if (y + mRowH < mPackRect.mY || y > mPackRect.mY + mPackRect.mHeight)
			{
				continue;
			}
			WardrobePack p = packs[i];
			bool worn = WardrobeService.IsWorn(p);
			TRect r = new TRect(mPackRect.mX + 4, y, mPackRect.mWidth - 8, mRowH);
			g.SetColor(i == mPackSel ? RowSel : (worn ? RowWorn : Row));
			g.FillRect(r);
			if (worn)
			{
				g.SetColor(Accent);
				g.FillRect(r.mX, r.mY, 5, r.mHeight);
			}
			TextAt(g, mFont, Fit(p.Title, r.mWidth - 70), r.mX + 12, r.mY + 4, Text);
			string line2 = p.Kind + (p.Author.Length > 0 ? "  作者 " + p.Author : string.Empty);
			TextAt(g, mFont, Fit(line2, r.mWidth - 20), r.mX + 12, r.mY + 6 + mFont.GetHeight(), Muted);
			if (worn)
			{
				TextAt(g, mFont, "已穿", r.mX + r.mWidth - 10 - mFont.StringWidth("已穿"), r.mY + 4, Accent);
			}
		}
		g.ClearClipRect();
	}

	private void DrawPreview(Graphics g)
	{
		PanelBox(g, mPreviewRect);
		WardrobePack p = SelectedPack();
		string msg = null;
		if (p == null)
		{
			msg = "选一个补丁";
		}
		else if (!mPreviewOn)
		{
			msg = "预览已关闭（补丁可能含成人内容）";
		}
		else if (PageCount(p) == 0)
		{
			msg = "这个补丁没有可预览的图片";
		}
		else if (IsSpinePage(p) && EnsureSpine(p))
		{
			TRect box = new TRect(mPreviewRect.mX + 10, mPreviewRect.mY + 10, mPreviewRect.mWidth - 20, mPreviewRect.mHeight - mFont.GetHeight() - 24);
			try
			{
				mSpine.Draw(g, box, mLawn.mAppCounter);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Wardrobe spine preview draw: " + ex.Message);
				mSpineFailed.Add(p.Id);
				ReleaseSpine();
			}
			string label = "组装预览" + (PageCount(p) > 1 ? "  " + (mPreviewPage + 1) + "/" + PageCount(p) : string.Empty);
			TextAt(g, mFont, label, mPreviewRect.mX + (mPreviewRect.mWidth - mFont.StringWidth(label)) / 2, mPreviewRect.mY + mPreviewRect.mHeight - mFont.GetHeight() - 10, Text);
		}
		else
		{
			EnsurePreview(p);
			if (mPreview == null)
			{
				msg = "这张图无法预览";
			}
			else
			{
				int bw = mPreviewRect.mWidth - 12;
				int bh = mPreviewRect.mHeight - 12;
				float k = Math.Min((float)bw / mPreview.mWidth, (float)bh / mPreview.mHeight);
				int dw = (int)(mPreview.mWidth * k);
				int dh = (int)(mPreview.mHeight * k);
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(Color.White);
				g.DrawImage(mPreview, new TRect(mPreviewRect.mX + (mPreviewRect.mWidth - dw) / 2, mPreviewRect.mY + (mPreviewRect.mHeight - dh) / 2, dw, dh), new TRect(0, 0, mPreview.mWidth, mPreview.mHeight));
				g.SetColorizeImages(colorizeImages: false);
				string page = (mPreviewPage + 1) + "/" + PageCount(p);
				TextAt(g, mFont, page, mPreviewRect.mX + (mPreviewRect.mWidth - mFont.StringWidth(page)) / 2, mPreviewRect.mY + mPreviewRect.mHeight - mFont.GetHeight() - 10, Text);
			}
		}
		if (msg != null)
		{
			TextAt(g, mFont, Fit(msg, mPreviewRect.mWidth - 20), mPreviewRect.mX + (mPreviewRect.mWidth - Math.Min(mFont.StringWidth(msg), mPreviewRect.mWidth - 20)) / 2, mPreviewRect.mY + mPreviewRect.mHeight / 2 - mFont.GetHeight() / 2, Muted);
		}
	}

	private void DrawInfo(Graphics g)
	{
		PanelBox(g, mInfoRect);
		WardrobePack p = SelectedPack();
		if (p == null)
		{
			return;
		}
		int x = mInfoRect.mX + 10;
		int y = mInfoRect.mY + 8;
		int lh = mFont.GetHeight() + 4;
		TextAt(g, mFont, Fit(p.Name, mInfoRect.mWidth - 20), x, y, Text);
		y += lh;
		string plants = p.IsGlobal ? "全局" : string.Join("、", p.PlantKeys.Select(WardrobeService.PlantTitle));
		TextAt(g, mFont, Fit("对象：" + plants, mInfoRect.mWidth - 20), x, y, Muted);
		y += lh;
		TextAt(g, mFont, Fit(p.Files.Count + " 个文件 · " + (p.Bytes / 1048576.0).ToString("0.0") + " MB", mInfoRect.mWidth - 20), x, y, Muted);
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

	// ------------------------------------------------------------------ preview

	private void EnsurePreview(WardrobePack p)
	{
		if (p.PreviewImages.Count == 0)
		{
			ReleasePreview();
			return;
		}
		int index = Math.Clamp(mPreviewPage - (HasSpinePage(p) ? 1 : 0), 0, p.PreviewImages.Count - 1);
		WardrobeFile f = p.PreviewImages[index];
		string key = f.Source;
		if (key == mPreviewKey)
		{
			return;
		}
		ReleasePreview();
		mPreviewKey = key;
		try
		{
			Texture2D tex;
			if (f.Relative.EndsWith(".xnb", StringComparison.Ordinal))
			{
				mPreviewContent = new FileContentManager(SexyAppBase.XnaGame.Services, f.Source);
				tex = mPreviewContent.Load<Texture2D>("preview");
			}
			else
			{
				using FileStream s = File.OpenRead(f.Source);
				tex = Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, s, DefaultColorProcessors.PremultiplyAlpha);
			}
			mPreview = new Image(tex);
		}
		catch (Exception ex)
		{
			mPreview = null;
			Console.Error.WriteLine("Wardrobe preview: " + ex.Message);
		}
	}

	/// <summary>Character packs get an assembled (animated) first page before the raw atlas pages.</summary>
	private bool HasSpinePage(WardrobePack p)
	{
		return p != null && !mSpineFailed.Contains(p.Id) && p.Files.Any((WardrobeFile f) => f.Relative.StartsWith("spanim/", StringComparison.Ordinal));
	}

	private int PageCount(WardrobePack p)
	{
		return p == null ? 0 : p.PreviewImages.Count + (HasSpinePage(p) ? 1 : 0);
	}

	private bool IsSpinePage(WardrobePack p)
	{
		return HasSpinePage(p) && mPreviewPage == 0;
	}

	private bool EnsureSpine(WardrobePack p)
	{
		if (mSpine != null && mSpineKey == p.Id)
		{
			return true;
		}
		ReleaseSpine();
		try
		{
			mSpine = WardrobeSpinePreview.TryCreate(p);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Wardrobe spine preview: " + ex.Message);
			mSpine = null;
		}
		if (mSpine == null)
		{
			mSpineFailed.Add(p.Id);
			mPreviewPage = 0;
			return false;
		}
		mSpineKey = p.Id;
		return true;
	}

	private void ReleaseSpine()
	{
		mSpine?.Dispose();
		mSpine = null;
		mSpineKey = null;
	}

	private void ReleasePreview()
	{
		ReleaseSpine();
		mPreviewKey = null;
		if (mPreviewContent != null)
		{
			mPreviewContent.Unload();
			mPreviewContent.Dispose();
			mPreviewContent = null;
		}
		else if (mPreview?.Texture != null)
		{
			mPreview.Texture.Dispose();
		}
		mPreview = null;
	}

	// ------------------------------------------------------------------ input

	public override void MouseDown(int x, int y, int theClickCount)
	{
		mDownX = x;
		mDownY = y;
		mLastY = y;
		mDragged = false;
		mDragArea = Contains(mGroupRect, x, y) ? 0 : (Contains(mPackRect, x, y) ? 1 : -1);
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
		if (mDragArea == 0)
		{
			mGroupScroll = ClampScroll(mGroupScroll - dy, mGroups.Count * (mFont.GetHeight() + 22) + 8, mGroupRect.mHeight);
		}
		else if (mDragArea == 1)
		{
			mPackScroll = ClampScroll(mPackScroll - dy, PackListHeight(), mPackRect.mHeight);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (!mDragged)
		{
			Tap(x, y);
		}
		mDragArea = -1;
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		MouseUp(x, y, theClickCount);
	}

	public override void MouseWheel(int theDelta)
	{
		mPackScroll = ClampScroll(mPackScroll - theDelta * 40, PackListHeight(), mPackRect.mHeight);
	}

	private static float ClampScroll(float v, int content, int view)
	{
		return Math.Clamp(v, 0f, Math.Max(0, content - view));
	}

	private static bool Contains(TRect r, int x, int y)
	{
		return x >= r.mX && y >= r.mY && x < r.mX + r.mWidth && y < r.mY + r.mHeight;
	}

	private void Tap(int x, int y)
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
		if (Contains(mGroupRect, x, y))
		{
			int rh = mFont.GetHeight() + 18 + 4;
			int i = (int)((y - mGroupRect.mY - 4 + mGroupScroll) / rh);
			if (i >= 0 && i < mGroups.Count && i != mGroupSel)
			{
				mGroupSel = i;
				mPackSel = 0;
				mPackScroll = 0;
				mPreviewPage = 0;
				ClampSelection();
			}
			return;
		}
		if (Contains(mPackRect, x, y))
		{
			int i = (int)((y - mPackRect.mY - 4 + mPackScroll) / (mRowH + 4));
			if (i >= 0 && i < CurrentPacks().Count)
			{
				mPackSel = i;
				mPreviewPage = 0;
			}
		}
	}

	private void OnButton(string id)
	{
		WardrobePack p = SelectedPack();
		switch (id)
		{
		case "close":
			Close();
			break;
		case "preview":
			mPreviewOn = !mPreviewOn;
			if (!mPreviewOn)
			{
				ReleasePreview();
			}
			break;
		case "prev":
			if (p != null)
			{
				mPreviewPage = (mPreviewPage + PageCount(p) - 1) % Math.Max(1, PageCount(p));
			}
			break;
		case "next":
			if (p != null)
			{
				mPreviewPage = (mPreviewPage + 1) % Math.Max(1, PageCount(p));
			}
			break;
		case "wear":
			if (p == null)
			{
				break;
			}
			if (WardrobeService.IsWorn(p))
			{
				WardrobeService.TakeOff(p);
				SetStatus("已脱下：" + p.Title + ReloadNote());
			}
			else
			{
				List<string> replaced = WardrobeService.Wear(p);
				SetStatus("已穿上：" + p.Title + (replaced.Count > 0 ? "（替换了 " + string.Join("、", replaced) + "）" : string.Empty) + ReloadNote());
			}
			RebuildGroupsKeepPack(p);
			break;
		case "all":
			WardrobeService.TakeOffAll();
			SetStatus("已全部复原" + ReloadNote());
			RebuildGroupsKeepPack(p);
			break;
		case "refresh":
			WardrobeService.Rescan();
			ReleasePreview();
			RebuildGroupsKeepPack(p);
			SetStatus(WardrobeService.Warnings.Count > 0 ? WardrobeService.Warnings[0] : ("已刷新，共 " + WardrobeService.Packs.Count + " 个补丁"));
			break;
		}
	}

	private static string ReloadNote()
	{
		HotReloadResult r = WardrobeService.LastReload;
		if (r == null)
		{
			return string.Empty;
		}
		string s = r.Summary();
		if (r.Errors.Count > 0)
		{
			s += (s.Length > 0 ? "；" : string.Empty) + "部分换装失败：" + r.Errors[0];
		}
		return s.Length > 0 ? " — " + s : string.Empty;
	}

	private void RebuildGroupsKeepPack(WardrobePack p)
	{
		RebuildGroups();
		if (p != null)
		{
			int i = CurrentPacks().FindIndex((WardrobePack q) => q.Id == p.Id);
			if (i >= 0)
			{
				mPackSel = i;
			}
		}
		ClampSelection();
	}

	public override void Update()
	{
		base.Update();
		if (mStatusTimer > 0)
		{
			mStatusTimer--;
		}
		MarkDirty();
	}

	public override bool BackButtonPress()
	{
		Close();
		return true;
	}

	private void Close()
	{
		ReleasePreview();
		mLawn.KillDialog(DIALOG_ID);
	}

	public override void Dispose()
	{
		ReleasePreview();
		base.Dispose();
	}
}
