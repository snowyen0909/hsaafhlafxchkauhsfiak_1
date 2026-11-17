// 宿舍 (dorm) feasibility test, Mod edition only: an isometric tatami room with up to 4 plant girls walking
// around, furniture that can be dragged / rotated / stored, tap a girl for a heart and her garden voice line.
// Art: ContentMod/dorm/*.png (one sprite per facing, _0.._3). The layout is saved to userdata/dorm.txt.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;

namespace Lawn;

internal sealed class DormDialog : LawnDialog
{
	public const int DIALOG_ID = 9047;

	private const int N = 8;

	// ------------------------------------------------------------------ catalog

	private enum Kind
	{
		/// <summary>Drawn with the floor, girls walk over it.</summary>
		Flat,
		/// <summary>Blocks its cells, depth sorted with the girls.</summary>
		Solid,
		/// <summary>Hangs on the left or right wall.</summary>
		Wall
	}

	private sealed class Def
	{
		public string Id;

		public string Title;

		public int A;

		public int B;

		public Kind Kind;

		public bool Rotates = true;

		/// <summary>Squash for flat art drawn a little steeper than the 2:1 grid.</summary>
		public float SquashY = 1f;
	}

	private static readonly Def[] Catalog = new Def[]
	{
		new Def { Id = "kotatsu", Title = "暖桌", A = 2, B = 2, Kind = Kind.Solid },
		new Def { Id = "cushion", Title = "坐垫", A = 1, B = 1, Kind = Kind.Flat, SquashY = 0.9f },
		new Def { Id = "planter", Title = "盆栽", A = 1, B = 1, Kind = Kind.Solid },
		new Def { Id = "futon", Title = "被褥", A = 2, B = 3, Kind = Kind.Solid, SquashY = 0.9f },
		new Def { Id = "tv", Title = "电视", A = 1, B = 2, Kind = Kind.Solid },
		new Def { Id = "bookshelf", Title = "书架", A = 1, B = 1, Kind = Kind.Solid },
		new Def { Id = "lantern", Title = "灯笼", A = 1, B = 1, Kind = Kind.Solid },
		new Def { Id = "rug", Title = "地毯", A = 2, B = 2, Kind = Kind.Flat, SquashY = 0.9f },
		new Def { Id = "sunflower_pillow", Title = "向日葵抱枕", A = 1, B = 1, Kind = Kind.Solid },
		new Def { Id = "zombie_head", Title = "僵尸头玩偶", A = 1, B = 1, Kind = Kind.Solid },
		new Def { Id = "pot_hat", Title = "锅子帽", A = 1, B = 1, Kind = Kind.Wall, Rotates = false }
	};

	private static Def FindDef(string id) => Catalog.FirstOrDefault((Def d) => d.Id == id);

	/// <summary>Sheared wall art: width, height, bottom-line y at x = 0 and at x = width.</summary>
	private static readonly Dictionary<string, float[]> WallGeo = new Dictionary<string, float[]>
	{
		["wall_door_0"] = new float[] { 150, 180, 181.0f, 105.1f },
		["wall_door_1"] = new float[] { 147, 178, 104.5f, 179.0f },
		["wall_window_0"] = new float[] { 156, 191, 192.4f, 114.1f },
		["wall_window_1"] = new float[] { 149, 186, 113.6f, 188.0f }
	};

	private sealed class Item
	{
		public Def Def;

		public int I;

		public int J;

		public int Rot;

		/// <summary>Wall items: 0 = left wall, 1 = right wall; I is the slot along it.</summary>
		public int Side;

		public int A => (Rot & 1) == 0 ? Def.A : Def.B;

		public int B => (Rot & 1) == 0 ? Def.B : Def.A;

		public string Sprite => Def.Kind == Kind.Wall ? Def.Id + "_" + (Side == 0 ? 0 : 1) : Def.Id + "_" + Rot;
	}

	private static readonly (SeedType seed, string skel)[] Girls = new (SeedType, string)[]
	{
		(SeedType.Sunflower, "sunflower"),
		(SeedType.Marigold, "marigold"),
		(SeedType.Gatlingpea, "gatlingpea"),
		(SeedType.Snowpea, "snowpea")
	};

	// ------------------------------------------------------------------ state

	private static readonly Color Ink = new Color(70, 48, 30);

	private static readonly Color BarBg = new Color(64, 44, 30, 235);

	private static readonly Color BtnFill = new Color(122, 168, 70);

	private static readonly Color BtnOn = new Color(230, 160, 60);

	private static readonly Color BtnOff = new Color(110, 96, 84);

	private static readonly Color Paper = new Color(255, 246, 225);

	private readonly LawnApp mLawn;

	private readonly Font mFont;

	private readonly Dictionary<string, Image> mImages = new Dictionary<string, Image>();

	private readonly List<Item> mItems = new List<Item>();

	private readonly List<DormActor> mActors = new List<DormActor>();

	private readonly List<(float x, float y, float age)> mHearts = new List<(float, float, float)>();

	private readonly List<(float x, float y, float age)> mDust = new List<(float, float, float)>();

	private readonly Random mRand = new Random();

	private readonly Stopwatch mClock = Stopwatch.StartNew();

	private double mLastTime;

	private bool mEdit;

	private Item mSel;

	/// <summary>Drag in progress: the item, its pick-up spot, the finger offset in grid cells.</summary>
	private Item mDrag;

	private int mDragOrigI;

	private int mDragOrigJ;

	private int mDragOrigSide;

	private bool mDragIsNew;

	private float mGrabDI;

	private float mGrabDJ;

	private bool mDragValid;

	private int mDownX;

	private int mDownY;

	private bool mMoved;

	private string mHint = string.Empty;

	private float mHintTimer;

	// layout
	private float mCW;

	private float mCH;

	private float mOX;

	private float mOY;

	private TRect mBar;

	private readonly List<(string id, string text, TRect rect, bool on)> mButtons = new List<(string, string, TRect, bool)>();

	private readonly List<(Def def, TRect rect)> mPalette = new List<(Def, TRect)>();

	public static void Open(LawnApp app)
	{
		if (app.GetDialog(DIALOG_ID) != null)
		{
			return;
		}
		DormDialog d = new DormDialog(app);
		Sexy.ScreenScales ss = app.mScreenScales;
		// cover everything that is on screen (wide iPhones show more than the 1000-wide layout)
		int w = ss.VisibleWidth;
		int h = ss.VisibleHeight;
		int x = ss.VisibleLeft;
		int y = ss.VisibleTop;
		d.Resize(x, y, w, h);
#if !__IOS__
		Console.WriteLine($"DORMTEST layout vis=({ss.VisibleLeft},{ss.VisibleTop},{ss.VisibleWidth},{ss.VisibleHeight}) scale=({ss.mScaleFactor},{ss.mScaleFactorY}) trans=({ss.mTransX},{ss.mTransY}) dlg=({x},{y},{w},{h}) board=({Constants.BOARD_WIDTH},{Constants.BOARD_HEIGHT})");
#endif
		app.AddDialog(DIALOG_ID, d);
		app.mWidgetManager.SetFocus(d);
	}

	private DormDialog(LawnApp app)
		: base(app, null, DIALOG_ID, isModal: true, "宿舍", string.Empty, string.Empty, 0)
	{
		mLawn = app;
		mFont = Resources.FONT_BRIANNETOD12 ?? Resources.FONT_DWARVENTODCRAFT12;
		mWantsFocus = true;
		mClip = false;
		foreach ((SeedType seed, string skel) in Girls)
		{
			DormActor a = DormActor.TryCreate(seed, skel);
			if (a != null)
			{
				mActors.Add(a);
			}
		}
		if (!Load())
		{
			DefaultLayout();
		}
		PlaceActorsInitially();
		Hint("点植物娘可以互动；点「布置」摆放家具");
	}

	// ------------------------------------------------------------------ art

	private Image Img(string name)
	{
		if (mImages.TryGetValue(name, out Image img))
		{
			return img;
		}
		img = null;
		try
		{
			using Stream s = WardrobeOverlay.Open("Content/dorm/" + name + ".png");
			Texture2D tex = Texture2D.FromStream(GlobalStaticVars.g.GraphicsDevice, s, DefaultColorProcessors.PremultiplyAlpha);
			img = new Image(tex);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm art " + name + ": " + ex.Message);
		}
		mImages[name] = img;
		return img;
	}

	// ------------------------------------------------------------------ grid maths

	private float SX(float i, float j) => mOX + (i - j) * mCW / 2f;

	private float SY(float i, float j) => mOY + (i + j) * mCH / 2f;

	/// <summary>Screen point → continuous grid coordinates.</summary>
	private void ToGrid(float x, float y, out float i, out float j)
	{
		float u = (x - mOX) / (mCW / 2f);
		float v = (y - mOY) / (mCH / 2f);
		i = (u + v) / 2f;
		j = (v - u) / 2f;
	}

	private void Layout()
	{
		int barH = Math.Max(64, mFont.GetHeight() * 2 + 30);
		mBar = new TRect(0, mHeight - barH, mWidth, barH);
		float availH = mHeight - barH - 10;
		// room = 8 cells wide; height ≈ 4 cells of floor + about 1.45 cells of wall above the back corner
		mCW = Math.Min(Math.Min((mWidth - 24) / (float)N, availH / 5.55f), 100f);
		mCH = mCW / 2f;
		mOX = mWidth / 2f;
		mOY = 6f + 1.45f * mCW;
		mButtons.Clear();
		mPalette.Clear();
		int pad = 8;
		int bh = mFont.GetHeight() + 14;
		int by = mBar.mY + (barH - bh) / 2;
		int bw = Math.Max(70, mFont.StringWidth("完成布置") + 20);
		mButtons.Add(("close", "关闭", new TRect(mWidth - pad - bw, by, bw, bh), false));
		mButtons.Add(("edit", mEdit ? "完成布置" : "布置", new TRect(mWidth - pad * 2 - bw * 2, by, bw, bh), mEdit));
		if (mEdit)
		{
			int x = pad;
			bool sel = mSel != null;
			mButtons.Add(("rotate", "旋转", new TRect(x, by, bw - 10, bh), false));
			x += bw - 4;
			mButtons.Add(("store", "收回", new TRect(x, by, bw - 10, bh), false));
			x += bw - 4;
			mButtons.Add(("clear", "清空", new TRect(x, by, bw - 10, bh), false));
			x += bw + 2;
			int right = mWidth - pad * 3 - bw * 2;
			int cell = Math.Min(barH - 10, Math.Max(30, (right - x) / Catalog.Length));
			for (int k = 0; k < Catalog.Length; k++)
			{
				mPalette.Add((Catalog[k], new TRect(x + k * cell, mBar.mY + (barH - cell) / 2, cell - 4, cell - 4)));
			}
		}
	}

	// ------------------------------------------------------------------ layout data

	private void DefaultLayout()
	{
		mItems.Clear();
		Add("rug", 3, 3, 0);
		Add("kotatsu", 3, 3, 0);
		Add("cushion", 2, 3, 0);
		Add("cushion", 5, 4, 0);
		Add("tv", 0, 3, 1);
		Add("bookshelf", 0, 0, 0);
		Add("planter", 7, 0, 0);
		Add("lantern", 0, 7, 0);
		Add("futon", 5, 0, 1);
		Add("sunflower_pillow", 6, 6, 0);
		Add("zombie_head", 7, 5, 0);
		mItems.Add(new Item { Def = FindDef("pot_hat"), Side = 0, I = 2 });
	}

	private void Add(string id, int i, int j, int rot)
	{
		Item it = new Item { Def = FindDef(id), I = i, J = j, Rot = rot };
		if (it.Def != null)
		{
			mItems.Add(it);
		}
	}

	private static string SavePath => GlobalStaticVars.GetDocumentsDir() + "userdata/dorm.txt";

	private bool Load()
	{
		try
		{
			if (!File.Exists(SavePath))
			{
				return false;
			}
			mItems.Clear();
			foreach (string line in File.ReadAllLines(SavePath))
			{
				string[] p = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (p.Length < 5 || p[0].StartsWith("#"))
				{
					continue;
				}
				Def def = FindDef(p[0]);
				if (def == null)
				{
					continue;
				}
				Item it = new Item { Def = def, I = int.Parse(p[1]), J = int.Parse(p[2]), Rot = int.Parse(p[3]) & 3, Side = int.Parse(p[4]) };
				if (def.Kind == Kind.Wall ? WallSlotFree(it, it.Side, it.I) : FitsAt(it, it.I, it.J, it.Rot))
				{
					mItems.Add(it);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm load: " + ex.Message);
			return false;
		}
	}

	private void Save()
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
			List<string> lines = new List<string> { "# dorm v1: id i j rot side" };
			lines.AddRange(mItems.Select((Item it) => $"{it.Def.Id} {it.I} {it.J} {it.Rot} {it.Side}"));
			File.WriteAllLines(SavePath, lines);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm save: " + ex.Message);
		}
	}

	// ------------------------------------------------------------------ occupancy

	private bool Covers(Item it, int i, int j) => it.Def.Kind != Kind.Wall && i >= it.I && j >= it.J && i < it.I + it.A && j < it.J + it.B;

	/// <summary>Can theItem stand at (i, j) with rotation rot? Flat items may overlap solid ones, never other flat ones.</summary>
	private bool FitsAt(Item theItem, int i, int j, int rot)
	{
		int a = (rot & 1) == 0 ? theItem.Def.A : theItem.Def.B;
		int b = (rot & 1) == 0 ? theItem.Def.B : theItem.Def.A;
		if (i < 0 || j < 0 || i + a > N || j + b > N)
		{
			return false;
		}
		foreach (Item o in mItems)
		{
			if (o == theItem || o.Def.Kind == Kind.Wall || o.Def.Kind != theItem.Def.Kind)
			{
				continue;
			}
			if (i < o.I + o.A && o.I < i + a && j < o.J + o.B && o.J < j + b)
			{
				return false;
			}
		}
		return true;
	}

	private bool WallSlotFree(Item theItem, int side, int slot)
	{
		if (slot < 0 || slot >= N)
		{
			return false;
		}
		return !mItems.Any((Item o) => o != theItem && o.Def.Kind == Kind.Wall && o.Side == side && o.I == slot);
	}

	private bool Blocked(int i, int j)
	{
		if (i < 0 || j < 0 || i >= N || j >= N)
		{
			return true;
		}
		foreach (Item it in mItems)
		{
			if (it.Def.Kind == Kind.Solid && it != mDrag && Covers(it, i, j))
			{
				return true;
			}
		}
		return false;
	}

	// ------------------------------------------------------------------ girls

	private void PlaceActorsInitially()
	{
		foreach (DormActor a in mActors)
		{
			Point p = RandomFreeCell(null);
			a.FI = p.X + 0.5f;
			a.FJ = p.Y + 0.5f;
			a.Timer = (float)mRand.NextDouble() * 2f + 0.5f;
			a.FaceLeft = mRand.Next(2) == 0;
		}
	}

	private Point RandomFreeCell(DormActor theExcept)
	{
		List<Point> free = new List<Point>();
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				if (!Blocked(i, j) && !mActors.Any((DormActor a) => a != theExcept && (int)a.FI == i && (int)a.FJ == j))
				{
					free.Add(new Point(i, j));
				}
			}
		}
		return free.Count > 0 ? free[mRand.Next(free.Count)] : new Point(N - 1, N - 1);
	}

	/// <summary>Breadth-first path on the 8x8 grid (4-neighbour) around solid furniture.</summary>
	private List<Point> FindPath(Point from, Point to)
	{
		Dictionary<Point, Point> prev = new Dictionary<Point, Point>();
		Queue<Point> q = new Queue<Point>();
		q.Enqueue(from);
		prev[from] = from;
		Point[] dirs = { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
		while (q.Count > 0)
		{
			Point c = q.Dequeue();
			if (c == to)
			{
				List<Point> path = new List<Point>();
				while (c != from)
				{
					path.Add(c);
					c = prev[c];
				}
				path.Reverse();
				return path;
			}
			foreach (Point d in dirs)
			{
				Point n = new Point(c.X + d.X, c.Y + d.Y);
				if (!prev.ContainsKey(n) && !Blocked(n.X, n.Y))
				{
					prev[n] = c;
					q.Enqueue(n);
				}
			}
		}
		return null;
	}

	private void UpdateActor(DormActor a, float dt)
	{
		a.Advance(dt);
		a.Hop = Math.Max(0f, a.Hop - dt * 3f);
		// furniture dropped on top of a girl: she steps aside
		if (Blocked((int)a.FI, (int)a.FJ) && a.Path.Count == 0)
		{
			Point p = RandomFreeCell(a);
			a.FI = p.X + 0.5f;
			a.FJ = p.Y + 0.5f;
		}
		switch (a.Mode)
		{
		case DormActor.State.Idle:
		case DormActor.State.Sleep:
			a.Timer -= dt;
			if (a.Timer <= 0f)
			{
				if (a.Mode == DormActor.State.Idle && a.CanSleep && mRand.Next(6) == 0)
				{
					a.Mode = DormActor.State.Sleep;
					a.PlaySleep();
					a.Timer = 4f + (float)mRand.NextDouble() * 3f;
					break;
				}
				Point from = new Point((int)a.FI, (int)a.FJ);
				Point to = RandomFreeCell(a);
				List<Point> path = FindPath(from, to);
				if (path != null && path.Count > 0)
				{
					a.Path.Clear();
					a.Path.AddRange(path.Take(6));
					a.Mode = DormActor.State.Walk;
					a.StartWalk();
				}
				else
				{
					a.Mode = DormActor.State.Idle;
					a.PlayIdle();
					a.Timer = 1f + (float)mRand.NextDouble() * 2f;
				}
			}
			break;
		case DormActor.State.Walk:
		{
			if (a.Path.Count == 0)
			{
				a.Mode = DormActor.State.Idle;
				a.StopWalk();
				a.Timer = 2f + (float)mRand.NextDouble() * 3f;
				break;
			}
			Point next = a.Path[0];
			if (Blocked(next.X, next.Y))
			{
				a.Path.Clear();
				break;
			}
			float ti = next.X + 0.5f, tj = next.Y + 0.5f;
			float di = ti - a.FI, dj = tj - a.FJ;
			float dist = (float)Math.Sqrt(di * di + dj * dj);
			float step = 1.6f * dt;
			float sx = di - dj;
			if (Math.Abs(sx) > 0.01f)
			{
				a.FaceLeft = sx < 0f;
			}
			if (dist <= step)
			{
				a.FI = ti;
				a.FJ = tj;
				a.Path.RemoveAt(0);
			}
			else
			{
				a.FI += di / dist * step;
				a.FJ += dj / dist * step;
			}
			// two steps per walk cycle: a footfall every half turn of the phase
			int before = (int)Math.Floor(a.WalkPhase / Math.PI);
			a.WalkPhase += dt * (float)(2 * Math.PI) / a.WalkCycle;
			if ((int)Math.Floor(a.WalkPhase / Math.PI) != before)
			{
				mDust.Add((SX(a.FI, a.FJ) + (a.FaceLeft ? 1 : -1) * mCW * 0.12f, SY(a.FI, a.FJ), 0f));
			}
			break;
		}
		}
	}

	// ------------------------------------------------------------------ update / draw

	public override void Update()
	{
		base.Update();
		double now = mClock.Elapsed.TotalSeconds;
		float dt = (float)Math.Clamp(now - mLastTime, 0.0, 0.1);
		mLastTime = now;
		foreach (DormActor a in mActors)
		{
			UpdateActor(a, dt);
		}
		for (int k = mHearts.Count - 1; k >= 0; k--)
		{
			(float x, float y, float age) h = mHearts[k];
			h.age += dt;
			if (h.age > 1.4f)
			{
				mHearts.RemoveAt(k);
			}
			else
			{
				mHearts[k] = h;
			}
		}
		for (int k = mDust.Count - 1; k >= 0; k--)
		{
			(float x, float y, float age) p = mDust[k];
			p.age += dt;
			if (p.age > 0.45f)
			{
				mDust.RemoveAt(k);
			}
			else
			{
				mDust[k] = p;
			}
		}
		if (mHintTimer > 0f)
		{
			mHintTimer -= dt;
		}
		MarkDirty();
	}

	public override void Draw(Graphics g)
	{
		Layout();
		g.SetColorizeImages(colorizeImages: false);
		g.SetColor(new Color(58, 40, 28));
		g.FillRect(0, 0, mWidth, mHeight);
		DrawWalls(g);
		DrawFloor(g);
		DrawDust(g);
		DrawSorted(g);
		DrawHearts(g);
		DrawBar(g);
	}

	private void DrawSprite(Graphics g, string name, float x, float y, float w, float h, Color? tint = null)
	{
		Image img = Img(name);
		if (img == null)
		{
			return;
		}
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(tint ?? Color.White);
		g.DrawImage(img, new TRect((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(w), (int)Math.Round(h)), new TRect(0, 0, img.mWidth, img.mHeight));
		g.SetColorizeImages(colorizeImages: false);
	}

	private void DrawWalls(Graphics g)
	{
		// left back wall runs along j at i = 0, right back wall along i at j = 0; two 4-cell pieces each
		string[] left = { "wall_window_0", "wall_door_0" };
		string[] right = { "wall_door_1", "wall_window_1" };
		for (int k = 0; k < 2; k++)
		{
			float[] geo = WallGeo[left[k]];
			float x0 = SX(0, 4 * k + 4), y0 = SY(0, 4 * k + 4);
			float s = (SX(0, 4 * k) - x0) / geo[0];
			DrawSprite(g, left[k], x0, y0 - geo[2] * s, geo[0] * s, geo[1] * s);
			geo = WallGeo[right[k]];
			x0 = SX(4 * k, 0);
			y0 = SY(4 * k, 0);
			s = (SX(4 * k + 4, 0) - x0) / geo[0];
			DrawSprite(g, right[k], x0, y0 - geo[2] * s, geo[0] * s, geo[1] * s);
		}
		foreach (Item it in mItems.Where((Item t) => t.Def.Kind == Kind.Wall))
		{
			DrawWallItem(g, it, it == mDrag ? (mDragValid ? new Color(200, 255, 200) : new Color(255, 160, 160)) : (Color?)null);
		}
	}

	private void WallSlotPoint(int side, int slot, out float x, out float y)
	{
		if (side == 0)
		{
			x = (SX(0, slot) + SX(0, slot + 1)) / 2f;
			y = (SY(0, slot) + SY(0, slot + 1)) / 2f;
		}
		else
		{
			x = (SX(slot, 0) + SX(slot + 1, 0)) / 2f;
			y = (SY(slot, 0) + SY(slot + 1, 0)) / 2f;
		}
	}

	private TRect WallItemRect(Item it)
	{
		Image img = Img(it.Sprite);
		WallSlotPoint(it.Side, it.I, out float x, out float y);
		float w = mCW * 0.62f;
		float h = img == null ? w : img.mHeight * w / img.mWidth;
		return new TRect((int)(x - w / 2f), (int)(y - mCW * 0.95f - h / 2f), (int)w, (int)h);
	}

	private void DrawWallItem(Graphics g, Item it, Color? tint)
	{
		TRect r = WallItemRect(it);
		DrawSprite(g, it.Sprite, r.mX, r.mY, r.mWidth, r.mHeight, tint);
	}

	private void DrawFloor(Graphics g)
	{
		for (int s = 0; s <= 2 * (N - 1); s++)
		{
			for (int i = 0; i < N; i++)
			{
				int j = s - i;
				if (j < 0 || j >= N)
				{
					continue;
				}
				string name = (i + j) % 2 == 0 ? "tatami_0" : "tatami_1";
				Image img = Img(name);
				if (img == null)
				{
					continue;
				}
				float w = mCW * 1.02f;
				float h = img.mHeight * (w / img.mWidth) * 0.856f;
				DrawSprite(g, name, SX(i, j) - w / 2f, SY(i, j) - 1f, w, h);
			}
		}
		if (mEdit)
		{
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					DrawSprite(g, "diamond", SX(i, j) - mCW / 2f, SY(i, j), mCW, mCH, new Color(255, 255, 255, 40));
				}
			}
		}
		foreach (Item it in mItems.Where((Item t) => t.Def.Kind == Kind.Flat).OrderBy((Item t) => t.I + t.J))
		{
			DrawItem(g, it);
		}
		if (mDrag != null && mDrag.Def.Kind != Kind.Wall)
		{
			Color c = mDragValid ? new Color(80, 220, 90, 150) : new Color(235, 60, 60, 150);
			for (int i = mDrag.I; i < mDrag.I + mDrag.A; i++)
			{
				for (int j = mDrag.J; j < mDrag.J + mDrag.B; j++)
				{
					if (i >= 0 && j >= 0 && i < N && j < N)
					{
						DrawSprite(g, "diamond", SX(i, j) - mCW / 2f, SY(i, j), mCW, mCH, c);
					}
				}
			}
		}
	}

	private TRect ItemRect(Item it)
	{
		Image img = Img(it.Sprite);
		float left = SX(it.I, it.J + it.B);
		float right = SX(it.I + it.A, it.J);
		float bottom = SY(it.I + it.A, it.J + it.B);
		float w = right - left;
		float h = img == null ? w : img.mHeight * (w / img.mWidth) * it.Def.SquashY;
		float lift = mCW * 0.06f;
		return new TRect((int)left, (int)(bottom - h + lift), (int)w, (int)h);
	}

	private void DrawItem(Graphics g, Item it)
	{
		TRect r = ItemRect(it);
		Color? tint = null;
		if (it == mDrag)
		{
			tint = mDragValid ? new Color(255, 255, 255, 150) : new Color(255, 150, 150, 150);
		}
		else if (mEdit && it == mSel)
		{
			tint = new Color(255, 245, 190);
		}
		DrawSprite(g, it.Sprite, r.mX, r.mY, r.mWidth, r.mHeight, tint);
	}

	private void DrawSorted(Graphics g)
	{
		List<(float key, int order, Action draw)> list = new List<(float, int, Action)>();
		foreach (Item it in mItems.Where((Item t) => t.Def.Kind == Kind.Solid))
		{
			Item cur = it;
			list.Add((it.I + it.A + it.J + it.B - 1f, 0, () => DrawItem(g, cur)));
		}
		float height = mCW * 1.4f;
		foreach (DormActor a in mActors)
		{
			DormActor cur = a;
			bool walking = a.Mode == DormActor.State.Walk;
			float wave = (float)Math.Sin(a.WalkPhase);
			float up = Math.Abs(wave);
			// generic walk: bounce per step, sway toward the planted foot, squash on each footfall
			float bob = walking ? -up * mCW * (a.HasCustomWalk ? 0.035f : 0.06f) : 0f;
			float tilt = walking ? wave * (a.HasCustomWalk ? 1.5f : 4f) : 0f;
			float squash = walking ? (float)Math.Pow(1f - up, 4) * 0.06f : 0f;
			float hop = -(float)Math.Sin(a.Hop * Math.PI) * mCW * 0.25f;
			list.Add((a.FI + a.FJ, 1, () => cur.Draw(g, SX(cur.FI, cur.FJ), SY(cur.FI, cur.FJ) + bob + hop, height, tilt, 1f + squash * 0.6f, 1f - squash)));
		}
		foreach (var e in list.OrderBy((x) => x.key).ThenBy((x) => x.order))
		{
			e.draw();
		}
	}

	private void DrawDust(Graphics g)
	{
		foreach ((float x, float y, float age) in mDust)
		{
			float t = age / 0.45f;
			float size = mCW * (0.12f + 0.18f * t);
			int alpha = (int)(170 * (1f - t));
			DrawSprite(g, "dust", x - size / 2f, y - size * 0.55f - t * mCW * 0.05f, size, size * 0.6f, new Color(255, 255, 255, alpha));
		}
	}

	private void DrawHearts(Graphics g)
	{
		foreach ((float x, float y, float age) in mHearts)
		{
			float t = age / 1.4f;
			float size = mCW * (0.3f + 0.15f * (float)Math.Sin(Math.Min(1f, age * 5f) * Math.PI / 2));
			int alpha = (int)(255 * Math.Clamp(1.4f - age, 0f, 1f));
			DrawSprite(g, "heart", x - size / 2f, y - t * mCW * 0.9f - size / 2f, size, size, new Color(255, 255, 255, alpha));
		}
	}

	private void DrawBar(Graphics g)
	{
		g.SetColor(BarBg);
		g.FillRect(mBar);
		foreach ((string id, string text, TRect rect, bool on) b in mButtons)
		{
			bool enabled = (b.id != "rotate" && b.id != "store") || (mSel != null && (b.id != "rotate" || mSel.Def.Rotates));
			g.SetColor(b.on ? BtnOn : (enabled ? BtnFill : BtnOff));
			g.FillRect(b.rect);
			g.SetColor(Ink);
			g.DrawRect(b.rect);
			Text(g, b.text, b.rect.mX + (b.rect.mWidth - mFont.StringWidth(b.text)) / 2, b.rect.mY + (b.rect.mHeight - mFont.GetHeight()) / 2, Paper);
		}
		foreach ((Def def, TRect rect) p in mPalette)
		{
			g.SetColor(new Color(255, 246, 225, 60));
			g.FillRect(p.rect);
			Image img = Img(p.def.Id + (p.def.Kind == Kind.Wall ? "_0" : "_0"));
			if (img != null)
			{
				float k = Math.Min((float)(p.rect.mWidth - 4) / img.mWidth, (float)(p.rect.mHeight - 4) / img.mHeight);
				float w = img.mWidth * k, h = img.mHeight * k;
				DrawSprite(g, p.def.Id + "_0", p.rect.mX + (p.rect.mWidth - w) / 2f, p.rect.mY + (p.rect.mHeight - h) / 2f, w, h);
			}
		}
		string hint = mHintTimer > 0f ? mHint : (mEdit ? "拖动家具摆放 · 点选后可旋转 / 收回 · 下方点图标添加" : "宿舍测试版 · 点植物娘互动");
		if (!mEdit || mHintTimer > 0f)
		{
			int hx = mEdit ? 8 : 12;
			int hy = mEdit ? mBar.mY - mFont.GetHeight() - 8 : mBar.mY + (mBar.mHeight - mFont.GetHeight()) / 2;
			if (mEdit)
			{
				g.SetColor(new Color(0, 0, 0, 120));
				g.FillRect(hx - 4, hy - 3, mFont.StringWidth(hint) + 8, mFont.GetHeight() + 6);
			}
			Text(g, hint, hx, hy, Paper);
		}
	}

	private void Text(Graphics g, string s, int x, int y, Color c)
	{
		g.SetFont(mFont);
		g.SetColor(c);
		g.DrawString(s, x, y);
	}

	private void Hint(string s)
	{
		mHint = s;
		mHintTimer = 3.5f;
	}

	// ------------------------------------------------------------------ input

	private static bool In(TRect r, int x, int y) => x >= r.mX && y >= r.mY && x < r.mX + r.mWidth && y < r.mY + r.mHeight;

	public override void MouseDown(int x, int y, int theClickCount)
	{
		mDownX = x;
		mDownY = y;
		mMoved = false;
		if (!mEdit || In(mBar, x, y))
		{
			return;
		}
		Item hit = HitItem(x, y);
		if (hit != null)
		{
			BeginDrag(hit, x, y, isNew: false);
		}
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		MouseDown(x, y, theClickCount);
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
		int ni = (int)Math.Round(gi - mGrabDI);
		int nj = (int)Math.Round(gj - mGrabDJ);
		ni = Math.Clamp(ni, -1, N);
		nj = Math.Clamp(nj, -1, N);
		mDrag.I = ni;
		mDrag.J = nj;
		mDragValid = FitsAt(mDrag, ni, nj, mDrag.Rot);
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mDrag != null)
		{
			Item it = mDrag;
			mDrag = null;
			if (mMoved)
			{
				if (mDragValid)
				{
					GlobalStaticVars.gLawnApp.PlaySample(Resources.SOUND_PLANT);
					Save();
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
		// front-most first: wall items, then solid items by depth, then flat ones
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
		foreach ((string id, string text, TRect rect, bool on) b in mButtons)
		{
			if (In(b.rect, x, y))
			{
				Button(b.id);
				return;
			}
		}
		foreach ((Def def, TRect rect) p in mPalette)
		{
			if (In(p.rect, x, y))
			{
				AddNew(p.def);
				return;
			}
		}
		if (In(mBar, x, y))
		{
			return;
		}
		if (mEdit)
		{
			mSel = HitItem(x, y);
			return;
		}
		// tap a girl: front-most first
		foreach (DormActor a in mActors.OrderByDescending((DormActor t) => t.FI + t.FJ))
		{
			if (In(a.LastRect, x, y))
			{
				Pet(a);
				return;
			}
		}
	}

	private void Pet(DormActor a)
	{
		a.Hop = 1f;
		a.Path.Clear();
		a.Mode = DormActor.State.Idle;
		a.StopWalk();
		a.PlayIdle();
		a.Timer = 2.5f;
		mHearts.Add((SX(a.FI, a.FJ), a.LastRect.mY, 0f));
		try
		{
			PlantVoice.Play(a.Seed, SeedType.None, mRand.Next(2) == 0 ? PlantVoice.VoiceType.Garden1 : PlantVoice.VoiceType.Garden2);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm voice: " + ex.Message);
		}
	}

	private void Button(string id)
	{
		GlobalStaticVars.gLawnApp.PlaySample(Resources.SOUND_TAP);
		switch (id)
		{
		case "close":
			Close();
			break;
		case "edit":
			mEdit = !mEdit;
			mSel = null;
			if (!mEdit)
			{
				Save();
			}
			break;
		case "rotate":
			if (mSel != null && mSel.Def.Rotates)
			{
				int next = (mSel.Rot + 1) & 3;
				if (FitsAt(mSel, mSel.I, mSel.J, next))
				{
					mSel.Rot = next;
					Save();
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
				Save();
			}
			break;
		case "clear":
			mItems.Clear();
			mSel = null;
			Save();
			Hint("已清空，点下方图标重新添加家具");
			break;
		}
	}

	/// <summary>Palette tap: put a new piece on the first free spot nearest the middle of the room.</summary>
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
					Save();
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
				Save();
				return;
			}
		}
		Hint("房间里放不下了");
	}

	public override bool BackButtonPress()
	{
		Close();
		return true;
	}

	private void Close()
	{
		Save();
		mLawn.KillDialog(DIALOG_ID);
	}

	public override void Dispose()
	{
		foreach (DormActor a in mActors)
		{
			a.Dispose();
		}
		mActors.Clear();
		foreach (Image img in mImages.Values)
		{
			img?.Texture?.Dispose();
		}
		mImages.Clear();
		base.Dispose();
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

	internal void TestButton(string id) => Button(id);

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
		// two cells along +i, then hold
		float t = Math.Min(1f, k / 60f);
		MouseDrag((int)(mTestX + t * mCW), (int)(mTestY + t * mCH));
	}

	internal void TestEndDrag()
	{
		MouseUp((int)(mTestX + mCW), (int)(mTestY + mCH), 1);
	}
#endif
}
