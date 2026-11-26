// 宿舍 room: size (8 → 10 → 12 cells, 扩建房间), floor tiles (铺瓷砖), wall art for each size, hiding the walls,
// pinch / wheel zoom and panning, the sunflower trophies.
// Art: ContentMod/dorm/tiles/<material>.png (8x8 floors cut into 130x66 cell slots + 64x44 edge pieces, or one
// cell of a single tile), ContentMod/dorm/walls/wall_<style>_<N>.png (walls cut into strips and re-joined, see
// tools/dorm_room), *_base.png (only the skirting, shown while the walls are hidden).
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog
{
	// ------------------------------------------------------------------ size & capacity

	/// <summary>Girls a room of n x n cells holds: 8 → 4, 10 → 5, 12 → 6.</summary>
	private static int CapacityFor(int n) => n >= 12 ? 6 : n >= 10 ? 5 : 4;

	private const int MaxRoom = 12;

	/// <summary>Price of enlarging to n x n, in coins (10x10 $20,000, 12x12 $40,000).</summary>
	private static int ExpandPrice(int n) => n >= 12 ? 4000 : 2000;

	private int NextSize => mN >= MaxRoom ? 0 : mN + 2;

	/// <summary>Girls floor k holds.</summary>
	private int Cap(int k) => k == mFloor ? MaxGirls : CapacityFor(LayoutSize(mFloors[k].Layout));

	private int TotalCap => Enumerable.Range(0, mFloors.Count).Sum(Cap);

	private static int LayoutSize(string layout)
	{
		foreach (string line in layout.Split('\n'))
		{
			if (line.StartsWith("@size ", StringComparison.Ordinal) && int.TryParse(line.Substring(6).Trim(), out int n))
			{
				return ClampSize(n);
			}
		}
		return 8;
	}

	private static int ClampSize(int n) => n >= 12 ? 12 : n >= 10 ? 10 : 8;

	/// <summary>Changes the room size; cells already there keep their tiles, new cells have none (the floor shows).</summary>
	private void SetSize(int n)
	{
		n = ClampSize(n);
		if (n == mN && mTiles.Length == n * n)
		{
			return;
		}
		string[] t = new string[n * n];
		int old = (int)Math.Round(Math.Sqrt(mTiles.Length));
		for (int i = 0; i < Math.Min(old, n); i++)
		{
			for (int j = 0; j < Math.Min(old, n); j++)
			{
				t[i * n + j] = mTiles[i * old + j];
			}
		}
		mN = n;
		mTiles = t;
	}

	// ------------------------------------------------------------------ floor (地板) + tiles (地砖)

	/// <summary>
	/// The tile layer, one entry per cell, row by row (i * N + j): a tile material ("floor_redcarpet", "floor_tatami_t", ...)
	/// or null where no tile is laid, so the room's floor (mFloorStyle, one whole picture) shows through.
	/// </summary>
	private string[] mTiles = new string[64];

	private string TileAt(int i, int j) => mTiles[i * mN + j];

	private bool HasTiles => mTiles.Any((string t) => t != null);

	/// <summary>The same tile on every cell (整间铺满); null takes every tile away (全部移除).</summary>
	private void FillTiles(string id)
	{
		for (int k = 0; k < mTiles.Length; k++)
		{
			mTiles[k] = id;
		}
	}

	private bool SetTile(int i, int j, string id)
	{
		if (i < 0 || j < 0 || i >= mN || j >= mN || mTiles[i * mN + j] == id)
		{
			return false;
		}
		mTiles[i * mN + j] = id;
		return true;
	}

	/// <summary>Comfort of the floor: the floor's own, or the tile on at least half of the cells when that is more.</summary>
	private int FloorComfort
	{
		get
		{
			int c = FindDef(mFloorStyle)?.Comfort ?? 0;
			IGrouping<string, string> top = mTiles.Where((string t) => t != null).GroupBy((string t) => t).OrderByDescending((IGrouping<string, string> g) => g.Count()).FirstOrDefault();
			if (top != null && top.Count() * 2 >= mTiles.Length)
			{
				c = Math.Max(c, FindDef(top.Key)?.Comfort ?? 0);
			}
			return c;
		}
	}

	/// <summary>Run-length text of the tile layer ("-*60,floor_white*4", "-" = no tile), or null when no tile is laid.</summary>
	private string TilesText()
	{
		if (!HasTiles)
		{
			return null;
		}
		StringBuilder sb = new StringBuilder();
		for (int k = 0; k < mTiles.Length;)
		{
			int e = k;
			while (e < mTiles.Length && mTiles[e] == mTiles[k])
			{
				e++;
			}
			if (sb.Length > 0)
			{
				sb.Append(',');
			}
			sb.Append(mTiles[k] ?? "-").Append('*').Append(e - k);
			k = e;
		}
		return sb.ToString();
	}

	/// <summary>
	/// Reads the tile layer. Test builds of 11/24 kept the whole floor in the same list: a plain floor id there means
	/// "no tile", "floor_x~" was floor x laid tile by tile and is now the tile floor_x_t (given for free).
	/// </summary>
	private void ReadTiles(string text)
	{
		List<string> cells = new List<string>();
		foreach (string run in text.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			int star = run.LastIndexOf('*');
			string id = star > 0 ? run.Substring(0, star) : run;
			int n = star > 0 && int.TryParse(run.Substring(star + 1), out int v) ? v : 1;
			id = TileFromSave(id);
			for (int k = 0; k < n && cells.Count < mTiles.Length; k++)
			{
				cells.Add(id);
			}
		}
		if (cells.Count == mTiles.Length)
		{
			mTiles = cells.ToArray();
		}
	}

	private string TileFromSave(string id)
	{
		if (id == "-")
		{
			return null;
		}
		bool laid = id.EndsWith("~", StringComparison.Ordinal);
		if (laid)
		{
			id = id.Substring(0, id.Length - 1) + "_t";
		}
		Def d = FindDef(id);
		if (d == null || !d.Tile)
		{
			return null;
		}
		if (Owned(d) <= 0)
		{
			Def whole = laid ? FindDef(id.Substring(0, id.Length - 2)) : null;
			if (whole == null || Owned(whole) <= 0)
			{
				return null;
			}
			AddOwned(d, 0);
		}
		return id;
	}

	/// <summary>Does a saved floor layout use this wall style / floor / tile?</summary>
	private static bool LayoutUses(string layout, string id)
	{
		foreach (string line in layout.Split('\n'))
		{
			if (line == "@wall " + id || line == "@floor " + id)
			{
				return true;
			}
			if (line.StartsWith("@tiles ", StringComparison.Ordinal) && line.Substring(7).Split(',').Any((string r) => r.Split('*')[0] == id))
			{
				return true;
			}
		}
		return false;
	}

	private const int TileW = 130, TileH = 66, EdgeW = 64, EdgeH = 44;

	/// <summary>
	/// Which of the 8 source rows/columns of a floor picture a cell uses. Bigger rooms repeat rows 2-3 in the middle, so the
	/// border shading of the original art stays at the border; tatami mats are 2 cells long and simply repeat every 8.
	/// </summary>
	private int TileSrc(int t, string id)
	{
		if (mN <= 8 || id == "floor_tatami")
		{
			return t % 8;
		}
		int extra = mN - 8;
		if (t < 4)
		{
			return t;
		}
		return t < 4 + extra ? 2 + (t - 4) % 2 : t - extra;
	}

	/// <summary>tiles/&lt;name&gt;.png: a floor cut into 8x8 cells (1040 px wide), or a tile's 2x2 block (260 px wide).</summary>
	private static string TileAtlas(string id) => "tiles/" + (id.StartsWith("floor_", StringComparison.Ordinal) ? id.Substring(6) : id);

	/// <summary>Part of an atlas, the destination widened to whole pixels so neighbouring tiles never leave a hairline gap.</summary>
	private void DrawAtlas(Graphics g, Image img, int sx, int sy, int sw, int sh, float x, float y, float w, float h)
	{
		int x0 = (int)Math.Floor(x), y0 = (int)Math.Floor(y);
		int x1 = (int)Math.Ceiling(x + w), y1 = (int)Math.Ceiling(y + h);
		if (x1 < 0 || y1 < 0 || x0 > mWidth || y0 > mHeight)
		{
			return;
		}
		// same state as DrawSprite (white tint), the path every other dorm sprite takes
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(Color.White);
		g.DrawImage(img, new TRect(x0, y0, x1 - x0, y1 - y0), new TRect(sx, sy, sw, sh));
	}

	/// <summary>Floor picture under every cell without a tile, tiles on the others, then the thickness of the two front edges.</summary>
	private void DrawTiles(Graphics g)
	{
		float s = mCW / 128f;
		Image floor = FloorArt(out string floorId);
		for (int i = 0; i < mN; i++)
		{
			for (int j = 0; j < mN; j++)
			{
				float x = SX(i, j) - 65f * s, y = SY(i, j) - s;
				Image tile = TileArt(TileAt(i, j));
				if (tile != null)
				{
					// a tile's atlas holds a 2x2 block: cells take turns
					DrawAtlas(g, tile, i % 2 * TileW, j % 2 * TileH, TileW, TileH, x, y, TileW * s, TileH * s);
				}
				else if (floor != null)
				{
					DrawAtlas(g, floor, TileSrc(i, floorId) * TileW, TileSrc(j, floorId) * TileH, TileW, TileH, x, y, TileW * s, TileH * s);
				}
			}
		}
		for (int k = 0; k < mN; k++)
		{
			Image tile = TileArt(TileAt(k, mN - 1));
			if (tile != null)
			{
				DrawAtlas(g, tile, k % 2 * EdgeW, 2 * TileH, EdgeW, EdgeH, SX(k, mN), SY(k, mN), EdgeW * s, EdgeH * s);
			}
			else if (floor != null)
			{
				DrawAtlas(g, floor, TileSrc(k, floorId) * EdgeW, 8 * TileH, EdgeW, EdgeH, SX(k, mN), SY(k, mN), EdgeW * s, EdgeH * s);
			}
			tile = TileArt(TileAt(mN - 1, k));
			if (tile != null)
			{
				DrawAtlas(g, tile, k % 2 * EdgeW, 2 * TileH + EdgeH, EdgeW, EdgeH, SX(mN, k + 1), SY(mN, k), EdgeW * s, EdgeH * s);
			}
			else if (floor != null)
			{
				DrawAtlas(g, floor, TileSrc(k, floorId) * EdgeW, 8 * TileH + EdgeH, EdgeW, EdgeH, SX(mN, k + 1), SY(mN, k), EdgeW * s, EdgeH * s);
			}
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	/// <summary>The room's floor picture (cut into cells); tatami if it cannot be read.</summary>
	private Image FloorArt(out string id)
	{
		id = mFloorStyle;
		Image img = Img(TileAtlas(id));
		if (img == null || img.mWidth < 8 * TileW)
		{
			id = "floor_tatami";
			img = Img(TileAtlas(id));
		}
		return img;
	}

	/// <summary>A tile's 2x2 atlas, or null for no tile (or one that cannot be read: the floor then shows).</summary>
	private Image TileArt(string id)
	{
		if (id == null)
		{
			return null;
		}
		Image img = Img(TileAtlas(id));
		return img != null && img.mWidth >= 2 * TileW ? img : null;
	}

	// ------------------------------------------------------------------ walls

	/// <summary>Walls switched off (墙壁隐藏): only the skirting is drawn; the room keeps its size and nothing walks through.</summary>
	private bool mWallsHidden;

	/// <summary>Walls are drawn when switched on, and also while a wall piece is being dragged.</summary>
	private bool WallsShown => !mWallsHidden || (mDrag != null && mDrag.Def.Kind == Kind.Wall);

	private string WallArt(bool skirting) => (mN == 8 ? mWallStyle : "walls/" + mWallStyle + "_" + mN) + (skirting ? "_base" : "");

	/// <summary>Walls (or just their skirting) at 128 px per cell, the floor's back corner at (64 N + 4, 185). False when hidden.</summary>
	private bool DrawRoomWalls(Graphics g)
	{
		bool shown = WallsShown;
		string name = WallArt(!shown);
		Image img = Img(name);
		if (img != null)
		{
			float s = mCW / 128f;
			DrawSprite(g, name, mOX - (64 * mN + 4) * s, mOY - 185f * s, img.mWidth * s, img.mHeight * s);
		}
		return shown;
	}

	private void ToggleWalls()
	{
		mWallsHidden = !mWallsHidden;
		if (mSel != null && mSel.Def.Kind == Kind.Wall && mWallsHidden)
		{
			mSel = null;
		}
		WriteSave();
		Hint(mWallsHidden ? "墙壁隐藏了 · 墙边还是走不过去" : "墙壁显示出来了");
	}

	// ------------------------------------------------------------------ zoom & pan

	private float mZoom = 1f;

	private float mPanX;

	private float mPanY;

	private float mBaseCW;

	private float mBaseOX;

	private float mBaseOY;

	private TRect mRoomBox;

	private const float MaxZoom = 2.5f;

	private bool Zoomed => mZoom > 1.01f;

	/// <summary>Zoom to z keeping the screen point (px, py) still.</summary>
	private void ZoomAt(float z, float px, float py)
	{
		z = Math.Clamp(z, 1f, MaxZoom);
		float k = z / mZoom;
		float ox = mBaseOX + mPanX, oy = mBaseOY + mPanY;
		float nox = px - (px - ox) * k, noy = py - (py - oy) * k;
		// the vertical anchor scales with the cell size as well: oy is the back corner, which keeps its relative place
		mZoom = z;
		mPanX = nox - mBaseOX;
		mPanY = noy - mBaseOY;
		ClampPan();
		ApplyView();
	}

	private void ResetZoom()
	{
		mZoom = 1f;
		mPanX = mPanY = 0f;
		ApplyView();
	}

	/// <summary>The room may not leave the room box further than its overhang: at zoom 1 it sits exactly where it was laid out.</summary>
	private void ClampPan()
	{
		if (mBaseCW <= 0f || mRoomBox.mWidth <= 0)
		{
			return;
		}
		float cw = mBaseCW * mZoom;
		float w = cw * mN, h = cw * (mN / 2f + 1.55f);
		float bx = mRoomBox.mX + mRoomBox.mWidth / 2f, by = mRoomBox.mY + mRoomBox.mHeight / 2f;
		float limX = Math.Max(0f, (w - mRoomBox.mWidth) / 2f) + (mZoom > 1.01f ? cw : 0f);
		float ox = Math.Clamp(mBaseOX + mPanX, bx - limX, bx + limX);
		mPanX = ox - mBaseOX;
		float off = (mN / 2f * cw + 0.1f * cw - 1.45f * cw) / 2f;
		float baseOff = (mN / 2f * mBaseCW + 0.1f * mBaseCW - 1.45f * mBaseCW) / 2f;
		float baseCy = mBaseOY + baseOff;
		float limY = Math.Max(0f, (h - mRoomBox.mHeight) / 2f) + (mZoom > 1.01f ? cw : 0f);
		float cy = Math.Clamp(mBaseOY + mPanY + off, baseCy - limY, baseCy + limY);
		mPanY = cy - off - mBaseOY;
	}

	private void ApplyView()
	{
		mCW = mBaseCW * mZoom;
		mCH = mCW / 2f;
		mOX = mBaseOX + mPanX;
		mOY = mBaseOY + mPanY;
	}

	private bool mPinching;

	/// <summary>A pinch happened since the first finger went down: its release is not a tap.</summary>
	private bool mPinchUsed;

	private float mPinchD0;

	private float mPinchZ0;

	private float mPinchMX;

	private float mPinchMY;

	/// <summary>One finger dragging the room around.</summary>
	private bool mPanning;

	/// <summary>Fingers on the screen (desktop tests can fake a pinch through mFakeTouches).</summary>
	private List<CGPoint> Touches
	{
		get
		{
#if !__IOS__
			if (mFakeTouches != null)
			{
				return mFakeTouches;
			}
#endif
			return Main.ActiveTouches;
		}
	}

	private bool CanZoom => mScreen == Screen.Room && !mQuiz && !mUnlockAsk && !mConfirm && !mExpandAsk && mHeld == null;

	private void UpdatePinch()
	{
		List<CGPoint> t = Touches;
		if (t.Count >= 2 && CanZoom)
		{
			TPoint abs = GetAbsPos();
			float x0 = t[0].X - abs.mX, y0 = t[0].Y - abs.mY, x1 = t[1].X - abs.mX, y1 = t[1].Y - abs.mY;
			float mx = (x0 + x1) / 2f, my = (y0 + y1) / 2f;
			float d = (float)Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
			if (!mPinching)
			{
				mPinching = true;
				mPinchUsed = true;
				mPinchD0 = Math.Max(20f, d);
				mPinchZ0 = mZoom;
				mPinchMX = mx;
				mPinchMY = my;
				CancelPointer();
				return;
			}
			ZoomAt(mPinchZ0 * d / mPinchD0, mx, my);
			mPanX += mx - mPinchMX;
			mPanY += my - mPinchMY;
			mPinchMX = mx;
			mPinchMY = my;
			ClampPan();
			ApplyView();
		}
		else if (mPinching && t.Count < 2)
		{
			mPinching = false;
		}
	}

	/// <summary>A second finger came down: whatever the first one was doing is dropped.</summary>
	private void CancelPointer()
	{
		mPress = null;
		mPainting = false;
		mPanning = false;
		if (mDrag != null)
		{
			Item it = mDrag;
			mDrag = null;
			if (mDragIsNew)
			{
				mItems.Remove(it);
				mSel = null;
			}
			else
			{
				it.I = mDragOrigI;
				it.J = mDragOrigJ;
				it.Side = mDragOrigSide;
			}
		}
	}

	private int mHoverX = -1;

	private int mHoverY = -1;

	public override void MouseMove(int x, int y)
	{
		base.MouseMove(x, y);
		mHoverX = x;
		mHoverY = y;
	}

	/// <summary>Desktop: the mouse wheel zooms around the pointer.</summary>
	public override void MouseWheel(int theDelta)
	{
		if (!CanZoom)
		{
			return;
		}
		float px = mHoverX >= 0 ? mHoverX : mOX, py = mHoverY >= 0 ? mHoverY : mOY + mN * mCH / 2f;
		ZoomAt(mZoom * (theDelta > 0 ? 1.15f : 1f / 1.15f), px, py);
	}

	// ------------------------------------------------------------------ 铺地砖 (arranging mode, 地砖 tab)

	/// <summary>Tile the brush lays.</summary>
	private Def mBrush;

	/// <summary>铺地砖 on: tapping / dragging over the floor lays tiles instead of moving furniture.</summary>
	private bool mPaint;

	/// <summary>橡皮擦 on: tapping / dragging takes tiles away (the floor shows again).</summary>
	private bool mErase;

	/// <summary>吸管: the next tap on the floor picks that cell's tile.</summary>
	private bool mPick;

	private bool mPainting;

	private const int TileTab = 4;

	private bool ShowTileTools => mEdit && (mTab == TileTab || mPaint || mPick || mErase);

	private void SelectBrush(Def d)
	{
		mBrush = d;
		mPaint = true;
		mErase = false;
		mPick = false;
		mSel = null;
		Hint("铺「" + d.Title + "」· 在地板上点或拖动 · 「整间铺满」一次铺好");
	}

	/// <summary>Lays the brush (or, with the eraser, takes the tile away) on the cell under (x, y); true when something changed.</summary>
	private bool PaintAt(int x, int y)
	{
		ToGrid(x, y, out float gi, out float gj);
		int ci = (int)Math.Floor(gi), cj = (int)Math.Floor(gj);
		if ((!mErase && mBrush == null) || ci < 0 || cj < 0 || ci >= mN || cj >= mN)
		{
			return false;
		}
		return SetTile(ci, cj, mErase ? null : mBrush.Id);
	}

	private bool PickAt(int x, int y)
	{
		ToGrid(x, y, out float gi, out float gj);
		int ci = (int)Math.Floor(gi), cj = (int)Math.Floor(gj);
		if (ci < 0 || cj < 0 || ci >= mN || cj >= mN)
		{
			return false;
		}
		string id = TileAt(ci, cj);
		Def d = id == null ? null : FindDef(id);
		if (d == null)
		{
			Hint("这一格没有地砖");
			return false;
		}
		SelectBrush(d);
		Hint("吸取了「" + d.Title + "」· 接着点地板铺上");
		return true;
	}

	/// <summary>The brush, or the first tile owned; null (with a hint) when no tile has been bought yet.</summary>
	private Def BrushOrFirst()
	{
		Def d = mBrush ?? Catalog.FirstOrDefault((Def t) => t.Tile && Owned(t) > 0);
		if (d == null)
		{
			Hint("还没有地砖 · 去「家具商店」的地砖分页买");
		}
		return d;
	}

	private void TileTool(string id)
	{
		switch (id)
		{
		case "t_paint":
			if (mPaint)
			{
				mPaint = false;
				Hint("铺地砖结束 · 可以继续摆家具");
			}
			else if (BrushOrFirst() is Def b)
			{
				SelectBrush(b);
			}
			break;
		case "t_fill":
			if (BrushOrFirst() is Def d)
			{
				FillTiles(d.Id);
				mBrush = d;
				Hint("整间铺满了「" + d.Title + "」（记得保存）");
			}
			break;
		case "t_pick":
			mPick = true;
			mPaint = false;
			mErase = false;
			mSel = null;
			Hint("点地板上的一格，吸取它的地砖");
			break;
		case "t_erase":
			mErase = !mErase;
			mPaint = false;
			mPick = false;
			mSel = null;
			Hint(mErase ? "橡皮擦 · 点或拖过地砖就拿掉，露出地板" : "橡皮擦关了");
			break;
		case "t_clear":
			if (!HasTiles)
			{
				Hint("房间里没有地砖");
				break;
			}
			FillTiles(null);
			Hint("地砖全部移除了，露出" + (FindDef(mFloorStyle)?.Title ?? "地板") + "（记得保存）");
			break;
		}
	}

	private void LeaveTileTools()
	{
		mPaint = false;
		mPick = false;
		mErase = false;
		mPainting = false;
	}

	// ------------------------------------------------------------------ 扩建房间

	private bool mExpandAsk;

	private TRect mExpandPanel, mExpandOk, mExpandNo;

	private void LayoutExpand()
	{
		int pw = 400, ph = 250;
		mExpandPanel = new TRect((mWidth - pw) / 2, (mHeight - ph) / 2, pw, ph);
		int bw = 140, bh = 44, by = mExpandPanel.mY + ph - bh - 22;
		mExpandNo = new TRect(mExpandPanel.mX + pw / 2 - bw - 10, by, bw, bh);
		mExpandOk = new TRect(mExpandPanel.mX + pw / 2 + 10, by, bw, bh);
	}

	private void ExpandButton()
	{
		if (NextSize == 0)
		{
			Hint("房间已经是最大的 12×12 了");
			return;
		}
		mExpandAsk = true;
	}

	private void ExpandClick(int x, int y)
	{
		LayoutExpand();
		int next = NextSize;
		if (In(mExpandOk, x, y) && next > 0)
		{
			if (!DormPlatform.SpendCoins(mLawn, ExpandPrice(next)))
			{
				Hint("金币不够 · 扩建需要 " + LawnApp.GetMoneyString(ExpandPrice(next)));
				mExpandAsk = false;
				return;
			}
			DoExpand(next);
			DormPlatform.PlaySound(Resources.SOUND_COIN);
			mExpandAsk = false;
		}
		else if (In(mExpandNo, x, y) || !In(mExpandPanel, x, y))
		{
			mExpandAsk = false;
		}
	}

	/// <summary>Bigger room: furniture and girls stay where they are (the back corner does not move), the new cells get the main floor.</summary>
	private void DoExpand(int n)
	{
		SetSize(n);
		mSnapshot = Serialize();
		WriteSave();
		ResetZoom();
		mFloorFade = 0.6f;
		Hint("房间扩建成 " + n + "×" + n + " 了！" + FloorName(mFloor) + " 可以住 " + MaxGirls + " 位植物娘");
	}

	private void DrawExpand(Graphics g)
	{
		LayoutExpand();
		g.SetColor(new Color(40, 25, 15, 140));
		g.FillRect(0, 0, mWidth, mHeight);
		TRect p = mExpandPanel;
		Draw9(g, "ui/panel", p, 26);
		Flowers(g, p);
		string title = "扩建房间";
		int tw = mFont.StringWidth(title) + 60;
		TRect tr = new TRect(p.mX + (p.mWidth - tw) / 2, p.mY - 20, tw, 44);
		Draw9(g, "ui/pill_dark", tr, 20);
		TextCentered(g, mFont, title, tr, Color.White, -2);
		int next = NextSize;
		DrawSpritePart(g, "ui2/bt_expand", new TRect(p.mX + 18, p.mY + 56, 240, (int)(240 * 245f / 584f)), 0f, 0f, 0.44f, 1f);
		int tx = p.mX + 136, lh = mFont.GetHeight() + 4, ty = p.mY + 40;
		int coins = DormPlatform.Coins(mLawn);
		Text(g, mFont, FloorName(mFloor) + " 房间  " + mN + "×" + mN + " → " + next + "×" + next, tx, ty, InkBrown);
		Text(g, mFont, "可以住 " + MaxGirls + " → " + CapacityFor(next) + " 位植物娘", tx, ty + lh, InkBrown);
		Text(g, mFont, "家具和地板都会保留", tx, ty + 2 * lh, InkGrey);
		Text(g, mFont, "价格  " + LawnApp.GetMoneyString(ExpandPrice(next)), tx, ty + 3 * lh + 6, new Color(170, 110, 20));
		Text(g, mFont, "金币  " + LawnApp.GetMoneyString(coins), tx, ty + 4 * lh + 6, coins >= ExpandPrice(next) ? InkBrown : InkWarn);
		Draw9(g, "ui/btn", mExpandNo, 22);
		TextCentered(g, mFont, "取消", mExpandNo, InkBrown, -2);
		bool can = coins >= ExpandPrice(next);
		Draw9(g, can ? "ui/btn_on" : "ui/btn_off", mExpandOk, 22);
		TextCentered(g, mFont, "扩建", mExpandOk, can ? InkBrown : InkGrey, -2);
	}

	// ------------------------------------------------------------------ art buttons

	/// <summary>A button drawn from the new button sheet (ContentMod/dorm/ui2/bt_*.png, text area left blank), with our own label.</summary>
	private void DrawArt(Graphics g, Ui u)
	{
		Image img = Img(u.Icon);
		if (img == null)
		{
			Draw9(g, !u.Enabled ? "ui/btn_off" : u.On ? "ui/btn_on" : "ui/btn", u.Rect, 22);
			TextCentered(g, mFont, u.Text, u.Rect, u.Enabled ? InkBrown : InkGrey, -2);
			return;
		}
		float k = Math.Min((float)u.Rect.mWidth / img.mWidth, (float)u.Rect.mHeight / img.mHeight);
		float w = img.mWidth * k, h = img.mHeight * k;
		float x = u.Rect.mX + (u.Rect.mWidth - w) / 2f, y = u.Rect.mY + (u.Rect.mHeight - h) / 2f;
		Color? tint = !u.Enabled ? new Color(190, 180, 170) : u.On ? new Color(255, 236, 150) : (Color?)null;
		DrawSprite(g, u.Icon, x, y, w, h, tint);
		if (string.IsNullOrEmpty(u.Text))
		{
			return;
		}
		bool wide = img.mWidth > img.mHeight * 1.8f;
		TRect tr = wide ? new TRect((int)(x + w * 0.48f), (int)(y + h * 0.12f), (int)(w * 0.50f), (int)(h * 0.72f)) : new TRect((int)(x + w * 0.06f), (int)(y + h * 0.64f), (int)(w * 0.88f), (int)(h * 0.28f));
		TextCentered(g, mFont, u.Text, tr, u.Enabled ? InkBrown : InkGrey, -1);
	}

	/// <summary>Buttons of the room view (normal mode): walls on / off, enlarge, zoom back.</summary>
	private void LayoutRoomButtons(int W, int H)
	{
		int ww = 150, wh = 62;
		AddUi("walls", UiKind.Art, mWallsHidden ? "墙壁隐藏" : "墙壁显示", new TRect(W - Pad - ww, H - Pad - wh, ww, wh), mWallsHidden ? "ui2/bt_wall_hide" : "ui2/bt_wall_show");
		AddUi("expand", UiKind.Art, NextSize == 0 ? "已是最大" : "扩建房间", new TRect(W - Pad - ww, H - Pad - 2 * wh - 6, ww, wh), "ui2/bt_expand", enabled: NextSize != 0);
		if (Zoomed || Math.Abs(mPanX) > 1f || Math.Abs(mPanY) > 1f)
		{
			AddUi("zoom_reset", UiKind.Art, "缩放还原", new TRect(W - Pad - 96, H - Pad - 2 * wh - 6 - 74, 96, 70), "ui2/bt_zoom_reset");
		}
	}

	/// <summary>Arranging mode: walls on / off under the side menu, the tile tools above the catalogue.</summary>
	private void LayoutEditRoomButtons(int W, int sideW, int y, int sq, int tabTop)
	{
		AddUi("walls", UiKind.Art, mWallsHidden ? "墙壁隐藏" : "墙壁显示", new TRect(Pad, y, sideW, 58), mWallsHidden ? "ui2/bt_wall_hide" : "ui2/bt_wall_show");
		if (Zoomed || Math.Abs(mPanX) > 1f || Math.Abs(mPanY) > 1f)
		{
			AddUi("zoom_reset", UiKind.Art, "缩放还原", new TRect(W - Pad - sq - 14, 78 + 222, sq + 14, 66), "ui2/bt_zoom_reset");
		}
		if (ShowTileTools)
		{
			int bw = 84, bh = 62, gap = 5;
			int x = W - Pad - 5 * bw - 4 * gap, by = tabTop - bh - 4;
			AddUi("t_paint", UiKind.Art, "铺地砖", new TRect(x, by, bw, bh), "ui2/bt_tile_mode", on: mPaint);
			AddUi("t_fill", UiKind.Art, "整间铺满", new TRect(x + bw + gap, by, bw, bh), "ui2/bt_fill_all");
			AddUi("t_pick", UiKind.Art, "吸管", new TRect(x + 2 * (bw + gap), by, bw, bh), "ui2/bt_eyedrop", on: mPick);
			AddUi("t_erase", UiKind.Art, "橡皮擦", new TRect(x + 3 * (bw + gap), by, bw, bh), "ui2/bt_erase", on: mErase);
			AddUi("t_clear", UiKind.Art, "全部移除", new TRect(x + 4 * (bw + gap), by, bw, bh), "ui2/bt_clear", enabled: HasTiles);
		}
	}

	// ------------------------------------------------------------------ trophies

	private bool TrophyEarned(Def d)
	{
		if (mTestTrophies)
		{
			return true;
		}
#if !__IOS__
		if (Environment.GetEnvironmentVariable("PVZ_DORM_TROPHIES") == "1")
		{
			return true;
		}
#endif
		return d.Id == "trophy_gold" ? DormPlatform.EarnedGoldTrophy(mLawn) : DormPlatform.FinishedAdventure(mLawn);
	}

	private static string TrophyHow(Def d) => d.Id == "trophy_gold" ? "达成金向日葵后获得" : "通关冒险模式后获得";

#if !__IOS__
	private List<CGPoint> mFakeTouches;

	/// <summary>Fake two fingers (absolute coordinates) for one frame each call; null lifts them.</summary>
	internal void TestPinch(float cx, float cy, float dist)
	{
		TPoint abs = GetAbsPos();
		mFakeTouches = dist <= 0f ? null : new List<CGPoint> { new CGPoint(abs.mX + cx - dist / 2f, abs.mY + cy), new CGPoint(abs.mX + cx + dist / 2f, abs.mY + cy) };
		UpdatePinch();
		if (dist <= 0f)
		{
			mFakeTouches = new List<CGPoint>();
			UpdatePinch();
			mFakeTouches = null;
		}
	}

	/// <summary>Pointer in dialog coordinates: phase 0 down, 1 drag, 2 up.</summary>
	internal void TestPointer(int phase, int x, int y)
	{
		if (phase == 0)
		{
			MouseDown(x, y, 1);
		}
		else if (phase == 1)
		{
			MouseDrag(x, y);
		}
		else
		{
			MouseUp(x, y, 1);
		}
	}

	internal (float x, float y) TestCellPoint(float i, float j) => (SX(i, j), SY(i, j));

	internal void TestZoom(float z) => ZoomAt(z, mOX, mOY + mN * mCH / 2f);

	internal void TestPan(float dx, float dy)
	{
		mPanX += dx;
		mPanY += dy;
		ClampPan();
		ApplyView();
	}

	internal void TestResetZoom() => ResetZoom();

	internal void TestExpand() => DoExpand(NextSize == 0 ? 12 : NextSize);

	internal void TestExpandAsk() => mExpandAsk = true;

	internal void TestWalls(bool hidden)
	{
		if (mWallsHidden != hidden)
		{
			ToggleWalls();
		}
	}

	internal void TestTile(string id, int i, int j)
	{
		Def d = id == null ? null : FindDef(id);
		if (id == null || (d != null && d.Tile && Owned(d) > 0))
		{
			SetTile(i, j, id);
		}
	}

	internal void TestFloor(string id)
	{
		Def d = FindDef(id);
		if (d != null && Owned(d) > 0)
		{
			mFloorStyle = id;
		}
	}

	internal void TestBrush(string id)
	{
		mTab = TileTab;
		Def d = FindDef(id);
		if (d != null)
		{
			SelectBrush(d);
		}
	}

	internal void TestSaveLayout() => Save();
#endif
}
