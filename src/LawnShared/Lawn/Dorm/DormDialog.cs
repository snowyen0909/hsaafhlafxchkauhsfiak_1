// 宿舍 (dorm) test, Mod edition only: an isometric tatami room with up to 4 plant girls walking around.
// Normal mode: tap a girl for a heart + voice line, long-press to pick her up, drop her on the futon to sleep.
// 布置 mode: drag / rotate / store furniture, add from the catalogue; changes count only after 保存.
// Art: ContentMod/dorm/*.png (furniture, one sprite per facing _0.._3), ContentMod/dorm/ui/*.png (chrome).
// This file: room, furniture, girls. DormDialog.Ui.cs: chrome, buttons, input.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;

namespace Lawn;

internal sealed partial class DormDialog : LawnDialog
{
	public const int DIALOG_ID = 9047;

	private const int N = 8;

	/// <summary>Seconds a girl sleeps on the futon before she gets up by herself.</summary>
	private const float BedTime = 60f;

	// ------------------------------------------------------------------ catalogue

	private enum Kind
	{
		/// <summary>Drawn with the floor, girls walk over it.</summary>
		Flat,
		/// <summary>Blocks its cells, depth sorted with the girls.</summary>
		Solid,
		/// <summary>Hangs on the left or right wall.</summary>
		Wall,
		/// <summary>A wall or floor style for the whole room (bought once, applied in 布置).</summary>
		Style
	}

	/// <summary>Catalogue tabs: 0 全部 1 家具 2 装饰 3 地板 4 墙纸 5 主题小物.</summary>
	private static readonly string[] Tabs = { "全部", "家具", "装饰", "地板", "墙纸", "主题小物" };

	private sealed class Def
	{
		public string Id;

		public string Title;

		public int A;

		public int B;

		public Kind Kind;

		public int Tab;

		public int Comfort;

		public bool Rotates = true;

		/// <summary>Squash for flat art drawn a little steeper than the 2:1 grid.</summary>
		public float SquashY = 1f;

		/// <summary>Price in coins (the game shows coins × 10: 500 = $5,000). Set from <see cref="Prices"/>.</summary>
		public int Price = 10;

		/// <summary>Styles: "wall" or "floor".</summary>
		public string Layer;

		/// <summary>The room's original style: always owned, free, cannot be sold.</summary>
		public bool Default;

		/// <summary>Drawn at this fraction of the footprint's width (small decorations), centred on the footprint.</summary>
		public float Scale = 1f;

		/// <summary>4 facings, or 2 when the art has no back view (rotating flips between the two fronts).</summary>
		public int Dirs = 4;
	}

	private static Def Style(string id, string title, string layer, int comfort, bool isDefault = false) => new Def { Id = id, Title = title, A = 1, B = 1, Kind = Kind.Style, Tab = layer == "wall" ? 4 : 3, Comfort = comfort, Layer = layer, Default = isDefault, Rotates = false };

	private static readonly Def[] Catalog = new Def[]
	{
		new Def { Id = "kotatsu", Title = "暖桌", A = 2, B = 2, Kind = Kind.Solid, Tab = 1, Comfort = 60 },
		new Def { Id = "cushion", Title = "坐垫", A = 1, B = 1, Kind = Kind.Flat, Tab = 1, Comfort = 15, SquashY = 0.9f },
		new Def { Id = "planter", Title = "盆栽", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 20 },
		new Def { Id = "futon", Title = "被褥", A = 2, B = 3, Kind = Kind.Solid, Tab = 1, Comfort = 50, SquashY = 0.9f },
		new Def { Id = "tv", Title = "电视", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 40 },
		new Def { Id = "bookshelf", Title = "书架", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 35 },
		new Def { Id = "rug", Title = "地毯", A = 2, B = 2, Kind = Kind.Flat, Tab = 3, Comfort = 30, SquashY = 0.9f },
		new Def { Id = "lantern", Title = "灯笼", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 25 },
		new Def { Id = "sunflower_pillow", Title = "向日葵抱枕", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 20 },
		new Def { Id = "zombie_head", Title = "僵尸头玩偶", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 20 },
		new Def { Id = "pot_hat", Title = "锅子帽挂饰", A = 1, B = 1, Kind = Kind.Wall, Tab = 2, Comfort = 15, Rotates = false },
		new Def { Id = "swing", Title = "花园秋千", A = 3, B = 2, Kind = Kind.Solid, Tab = 1, Comfort = 80 },
		// modern set, study set (botanist's room) and the sunflower record player
		new Def { Id = "sofa2", Title = "双人沙发", A = 2, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 45 },
		new Def { Id = "sofa1", Title = "单人沙发", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 30, Scale = 0.95f },
		new Def { Id = "coffee_table", Title = "茶几", A = 2, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 20, Scale = 0.9f },
		new Def { Id = "tv_stand", Title = "电视柜", A = 2, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 45, Dirs = 2 },
		new Def { Id = "bookshelf_modern", Title = "现代书架", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 30, Dirs = 2 },
		new Def { Id = "desk", Title = "书桌", A = 2, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 30, Scale = 0.95f, Dirs = 2 },
		new Def { Id = "bed", Title = "单人床", A = 1, B = 2, Kind = Kind.Solid, Tab = 1, Comfort = 50, Dirs = 2 },
		new Def { Id = "wardrobe", Title = "衣柜", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 30, Dirs = 2 },
		new Def { Id = "drawer", Title = "收纳柜", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 20, Scale = 0.9f, Dirs = 2 },
		new Def { Id = "floor_lamp", Title = "落地灯", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 20, Scale = 0.5f, Dirs = 2 },
		new Def { Id = "plant_big", Title = "大盆栽", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 25, Scale = 0.9f, Dirs = 2 },
		new Def { Id = "plant_small", Title = "小盆栽", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 15, Scale = 0.6f, Dirs = 2 },
		new Def { Id = "painting", Title = "风景挂画", A = 1, B = 1, Kind = Kind.Wall, Tab = 2, Comfort = 15, Scale = 0.9f, Rotates = false },
		new Def { Id = "wall_shelf", Title = "壁挂置物架", A = 1, B = 1, Kind = Kind.Wall, Tab = 2, Comfort = 20, Scale = 1.4f, Rotates = false },
		new Def { Id = "pillow_white", Title = "白色抱枕", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 8, Scale = 0.45f, Dirs = 2 },
		new Def { Id = "pillow_blue", Title = "蓝色抱枕", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 8, Scale = 0.45f, Dirs = 2 },
		new Def { Id = "books_modern", Title = "书本", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 5, Scale = 0.4f, Dirs = 2 },
		new Def { Id = "tissue", Title = "纸巾盒", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 5, Scale = 0.4f, Dirs = 2 },
		new Def { Id = "clock", Title = "挂钟", A = 1, B = 1, Kind = Kind.Wall, Tab = 5, Comfort = 8, Scale = 0.6f, Rotates = false },
		new Def { Id = "diffuser", Title = "香薰瓶", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 5, Scale = 0.25f, Dirs = 2 },
		new Def { Id = "armchair", Title = "扶手椅", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 30 },
		new Def { Id = "bookcase", Title = "书柜", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 35 },
		new Def { Id = "specimen_cabinet", Title = "玻璃标本柜", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 35 },
		new Def { Id = "chaise", Title = "躺椅", A = 2, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 45 },
		new Def { Id = "round_table", Title = "圆茶几", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 20, Dirs = 2 },
		new Def { Id = "street_lamp", Title = "立灯", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 20, Scale = 0.42f, Dirs = 2 },
		new Def { Id = "plant_leaf", Title = "绿叶盆栽", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 25, Scale = 0.9f, Dirs = 2 },
		new Def { Id = "herbarium", Title = "植物标本架", A = 1, B = 1, Kind = Kind.Wall, Tab = 2, Comfort = 20, Scale = 1.25f, Rotates = false },
		new Def { Id = "tube_rack", Title = "试管架", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 10, Scale = 0.5f, Dirs = 2 },
		new Def { Id = "bell_jar", Title = "玻璃罩标本", A = 1, B = 1, Kind = Kind.Solid, Tab = 2, Comfort = 20, Scale = 0.6f, Dirs = 2 },
		new Def { Id = "book_pile", Title = "书本堆", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 8, Scale = 0.5f, Dirs = 2 },
		new Def { Id = "study_rug", Title = "研究室地毯", A = 2, B = 2, Kind = Kind.Flat, Tab = 3, Comfort = 30, SquashY = 0.9f },
		new Def { Id = "ivy", Title = "垂吊常春藤", A = 1, B = 1, Kind = Kind.Wall, Tab = 2, Comfort = 15, Scale = 0.95f, Rotates = false },
		new Def { Id = "leaf_frame", Title = "叶子画框", A = 1, B = 1, Kind = Kind.Wall, Tab = 2, Comfort = 10, Scale = 0.75f, Rotates = false },
		new Def { Id = "book_bundle", Title = "捆书", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 8, Scale = 0.45f, Dirs = 2 },
		new Def { Id = "inkwell", Title = "羽毛笔墨水", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 5, Scale = 0.32f, Dirs = 2 },
		new Def { Id = "potion", Title = "药水瓶", A = 1, B = 1, Kind = Kind.Solid, Tab = 5, Comfort = 5, Scale = 0.32f, Dirs = 2 },
		new Def { Id = "leaf_plaque", Title = "叶子壁饰", A = 1, B = 1, Kind = Kind.Wall, Tab = 5, Comfort = 8, Scale = 0.8f, Rotates = false },
		new Def { Id = "record_player", Title = "向日葵唱片机", A = 1, B = 1, Kind = Kind.Solid, Tab = 1, Comfort = 40 },
		Style("wall_sakura", "樱花和风墙壁", "wall", 0, isDefault: true),
		Style("wall_modern", "现代温馨墙壁", "wall", 30),
		Style("wall_european", "欧式华丽墙壁", "wall", 30),
		Style("wall_beach", "海滨度假墙壁", "wall", 30),
		Style("wall_forest", "森林小屋墙壁", "wall", 30),
		Style("wall_starry", "星空魔法墙壁", "wall", 30),
		Style("floor_tatami", "榻榻米地板", "floor", 0, isDefault: true),
		Style("floor_lightwood", "浅木地板", "floor", 20),
		Style("floor_darkwood", "深木地板", "floor", 20),
		Style("floor_marble", "大理石地板", "floor", 20),
		Style("floor_beach", "海滨蓝白地板", "floor", 20),
		Style("floor_starry", "星空地板", "floor", 20)
	};

	/// <summary>
	/// Shop prices in dollars as the game shows them (计划书 v3, 第四章): interactive pieces most, then seats and beds,
	/// furniture, rugs, decorations, wall pieces, small items. Walls $5,000, floors $4,000; the default styles are free.
	/// </summary>
	private static readonly Dictionary<string, int> Prices = new Dictionary<string, int>
	{
		["swing"] = 12000,
		["record_player"] = 15000,
		["tv_stand"] = 10000,
		["tv"] = 8000,
		["kotatsu"] = 6000,
		["chaise"] = 5000,
		["sofa2"] = 5000,
		["bed"] = 4500,
		["futon"] = 4000,
		["sofa1"] = 3000,
		["armchair"] = 3000,
		["cushion"] = 800,
		["specimen_cabinet"] = 3500,
		["bookcase"] = 3000,
		["bookshelf"] = 2500,
		["wardrobe"] = 3000,
		["bookshelf_modern"] = 2500,
		["desk"] = 3000,
		["coffee_table"] = 2000,
		["drawer"] = 2000,
		["round_table"] = 1500,
		["rug"] = 2000,
		["study_rug"] = 2500,
		["lantern"] = 1500,
		["plant_big"] = 1500,
		["plant_leaf"] = 1500,
		["bell_jar"] = 1500,
		["floor_lamp"] = 1200,
		["street_lamp"] = 1200,
		["planter"] = 1000,
		["plant_small"] = 600,
		["wall_shelf"] = 1500,
		["herbarium"] = 1500,
		["painting"] = 1000,
		["ivy"] = 1000,
		["pot_hat"] = 1000,
		["leaf_frame"] = 800,
		["clock"] = 600,
		["leaf_plaque"] = 600,
		["sunflower_pillow"] = 1200,
		["zombie_head"] = 1200,
		["tube_rack"] = 800,
		["pillow_white"] = 500,
		["pillow_blue"] = 500,
		["book_pile"] = 500,
		["book_bundle"] = 500,
		["books_modern"] = 300,
		["tissue"] = 300,
		["diffuser"] = 300,
		["inkwell"] = 300,
		["potion"] = 300
	};

	static DormDialog()
	{
		foreach (Def d in Catalog)
		{
			int dollars = d.Kind == Kind.Style ? (d.Default ? 0 : d.Layer == "wall" ? 5000 : 4000) : Prices.TryGetValue(d.Id, out int v) ? v : 1000;
			d.Price = dollars / 10;
		}
	}

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

	private sealed class GirlDef
	{
		public SeedType Seed;

		public string Skel;

		public string Name;

		/// <summary>Bones whose slots stay empty in the dorm: big weapons are left at the door.</summary>
		public string[] Hide;

		/// <summary>Hovers / glides instead of stepping (no footstep dust).</summary>
		public bool Glides;
	}

	private static GirlDef G(SeedType seed, string skel, string name, string[] hide = null, bool glides = false) => new GirlDef { Seed = seed, Skel = skel, Name = name, Hide = hide, Glides = glides };

	/// <summary>Every plant girl the dorm knows; walks / standing poses: Content/dorm/walk|pose/&lt;skel&gt;.json.</summary>
	private static readonly GirlDef[] Girls = new GirlDef[]
	{
		G(SeedType.Sunflower, "sunflower", "向日葵"),
		G(SeedType.Peashooter, "peashooter", "豌豆射手"),
		G(SeedType.Gatlingpea, "gatlingpea", "机枪射手", new[] { "bone11" }),
		G(SeedType.Repeater, "repeater", "双发射手", new[] { "1_58" }),
		G(SeedType.Splitpea, "splitpea", "裂荚射手"),
		G(SeedType.Marigold, "marigold", "金盏花"),
		G(SeedType.Blover, "blover", "三叶草"),
		G(SeedType.Spikeweed, "caltrop", "地刺", new[] { "bone10" }),
		G(SeedType.Spikerock, "spikerock", "地刺王", new[] { "diciwang68", "diciwang9" }),
		G(SeedType.Puffshroom, "puffshroom", "小喷菇"),
		G(SeedType.Fumeshroom, "fumeshroom", "大喷菇"),
		G(SeedType.Scaredyshroom, "scaredyshroom", "胆小菇"),
		G(SeedType.Hypnoshroom, "hypnoshroom", "魅惑菇"),
		G(SeedType.Iceshroom, "iceshroom", "寒冰菇"),
		G(SeedType.Seashroom, "seashroom", "海蘑菇", null, glides: true),
		G(SeedType.Gravebuster, "gravebuster", "墓碑吞噬者", new[] { "bone76", "bone71", "bone79" }),
		G(SeedType.Garlic, "garlic", "大蒜"),
		G(SeedType.Chomper, "chomper", "大嘴花"),
		G(SeedType.Squash, "squash", "倭瓜"),
		G(SeedType.Tallnut, "tallnut", "高坚果", new[] { "bone30", "dun_back", "盾牌2" }),
		G(SeedType.Pumpkinshell, "pumpkin_body", "南瓜头"),
		G(SeedType.Umbrella, "umbrellaleaf", "叶子保护伞"),
		G(SeedType.Kernelpult, "cornpult", "玉米投手"),
		G(SeedType.Cattail, "cattail", "香蒲", new[] { "bone15", "bone118" }, glides: true),
		G(SeedType.HypnoCattail, "hypno_cattail", "魅惑香蒲", null, glides: true),
		G(SeedType.Agave, "agave", "龙舌兰", new[] { "3_197" }),
		G(SeedType.Plantern, "plantern", "路灯花"),
		G(SeedType.Imitater, "imitater", "模仿者"),
		G(SeedType.Endoflame, "endoflame", "火红莲", null, glides: true)
	};

	private static GirlDef FindGirl(string skel) => Girls.FirstOrDefault((GirlDef g) => g.Skel == skel);

	/// <summary>
	/// Only girls the player has already met (unlocked in the almanac / seed chooser) can live in the dorm.
	/// The hypno cattail has no seed of her own, so she comes with the cattail.
	/// </summary>
	private bool Unlocked(GirlDef g)
	{
#if !__IOS__
		if (Environment.GetEnvironmentVariable("PVZ_DORM_ALLGIRLS") == "1")
		{
			return true;
		}
#endif
		try
		{
			if (mLawn.mPlayerInfo == null)
			{
				return false;
			}
			SeedType s = g.Seed == SeedType.HypnoCattail ? SeedType.Cattail : g.Seed;
			return DormPlatform.HasPlant(mLawn, s);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm unlock: " + ex.Message);
			return false;
		}
	}

	private IEnumerable<GirlDef> UnlockedGirls => Girls.Where(Unlocked);

	/// <summary>How many girls may live in the dorm at once (each one is a full Spine skeleton).</summary>
	private const int MaxGirls = 4;

	// ------------------------------------------------------------------ state

	private readonly LawnApp mLawn;

	private readonly Font mFont;

	private readonly Font mBig;

	private readonly Dictionary<string, Image> mImages = new Dictionary<string, Image>();

	private readonly List<Item> mItems = new List<Item>();

	private readonly List<DormActor> mActors = new List<DormActor>();

	/// <summary>Girls chosen in 角色管理 (skeleton names, at most MaxGirls).</summary>
	private readonly List<string> mChosen = new List<string>();

	/// <summary>Furniture bought in the shop: id → pieces owned (placed + in the warehouse).</summary>
	private readonly Dictionary<string, int> mOwned = new Dictionary<string, int>();

	private MusicTune mPrevMusic = MusicTune.None;

	private readonly List<(float x, float y, float age)> mHearts = new List<(float, float, float)>();

	private readonly List<(float x, float y, float age)> mDust = new List<(float, float, float)>();

	private readonly Random mRand = new Random();

	private readonly Stopwatch mClock = Stopwatch.StartNew();

	private double mLastTime;

	private float mTime;

	private bool mEdit;

	private Item mSel;

	/// <summary>Layout as last saved / loaded (布置 changes are compared against it).</summary>
	private string mSnapshot = string.Empty;

	/// <summary>Drag in progress: the item, its pick-up spot, the finger offset in grid cells.</summary>
	private Item mDrag;

	private int mDragOrigI;

	private int mDragOrigJ;

	private int mDragOrigSide;

	private bool mDragIsNew;

	private float mGrabDI;

	private float mGrabDJ;

	private bool mDragValid;

	/// <summary>A girl being carried by the finger, and where the finger is.</summary>
	private DormActor mHeld;

	private float mHeldX;

	private float mHeldY;

	private float mHeldVX;

	private string mHint = string.Empty;

	private float mHintTimer;

	// room placement
	private float mCW;

	private float mCH;

	private float mOX;

	private float mOY;

	public static void Open(LawnApp app)
	{
		if (app.GetDialog(DIALOG_ID) != null)
		{
			return;
		}
		DormDialog d = new DormDialog(app);
		// cover everything that is on screen (wide iPhones show more than the 1000-wide layout)
		TRect vis = DormPlatform.VisibleArea(app);
		int w = vis.mWidth;
		int h = vis.mHeight;
		int x = vis.mX;
		int y = vis.mY;
		d.Resize(x, y, w, h);
#if !__IOS__
		Console.WriteLine($"DORMTEST layout dlg=({x},{y},{w},{h})");
#endif
		app.AddDialog(DIALOG_ID, d);
		app.mWidgetManager.SetFocus(d);
	}

	private DormDialog(LawnApp app)
		: base(app, null, DIALOG_ID, isModal: true, "宿舍", string.Empty, string.Empty, 0)
	{
		mLawn = app;
		mFont = Resources.FONT_BRIANNETOD12 ?? Resources.FONT_DWARVENTODCRAFT12;
		mBig = Resources.FONT_DWARVENTODCRAFT15 ?? mFont;
		mWantsFocus = true;
		mClip = false;
		ReadSave();
		foreach (string skel in mChosen)
		{
			AddActor(skel);
		}
		if (mLoadedLayout != null)
		{
			Deserialize(mLoadedLayout);
		}
		else
		{
			DefaultLayout();
		}
		mSnapshot = Serialize();
		if (mFreshSave)
		{
			WriteSave();
		}
		PlaceActorsInitially();
		Hint("点植物娘互动 · 长按可以拖动她");
#if !__IOS__
		if (Environment.GetEnvironmentVariable("PVZ_DORM_AUTOTEST") == null)
#endif
		{
			MaybeStartVisit();
		}
		mPrevMusic = DormPlatform.StartDormMusic(app);
	}

	private DormActor AddActor(string skel)
	{
		GirlDef def = FindGirl(skel);
		if (def == null)
		{
			return null;
		}
		DormActor a = DormActor.TryCreate(def.Seed, def.Skel, def.Hide);
		if (a != null)
		{
			a.Glides = def.Glides;
			mActors.Add(a);
		}
		return a;
	}

	// ------------------------------------------------------------------ art

	/// <summary>Chrome re-skinned from the UI sheets (ContentMod/dorm/ui2): old name → new sprite.</summary>
	private static readonly Dictionary<string, string> Skin = new Dictionary<string, string>
	{
		["ui/btn"] = "ui2/btn",
		["ui/btn_on"] = "ui2/btn_on",
		["ui/btn_off"] = "ui2/btn_off",
		["ui/tab"] = "ui2/tab",
		["ui/tab_on"] = "ui2/btn_on",
		["ui/card"] = "ui2/card",
		["ui/pill_dark"] = "ui2/pill_dark",
		["ui/ic_save"] = "ui2/ic_save",
		["ui/ic_load"] = "ui2/ic_load",
		["ui/ic_clear"] = "ui2/ic_clear",
		["ui/ic_house"] = "ui2/ic_house",
		["ui/ic_move"] = "ui2/ic_move",
		["ui/ic_left"] = "ui2/arrow_l",
		["ui/ic_right"] = "ui2/arrow_r",
		["heart"] = "ui2/heart"
	};

	/// <summary>Nine-slice corner size in the source art of the re-skinned sprites.</summary>
	private static readonly Dictionary<string, int> SkinCorner = new Dictionary<string, int>
	{
		["ui2/btn"] = 40,
		["ui2/btn_on"] = 40,
		["ui2/btn_off"] = 40,
		["ui2/tab"] = 30,
		["ui2/pill_dark"] = 39,
		["ui2/card"] = 34,
		["ui2/card_sel"] = 36,
		["ui2/card_off"] = 34
	};

	private Image Img(string name)
	{
		if (Skin.TryGetValue(name, out string skinned))
		{
			name = skinned;
		}
		if (mImages.TryGetValue(name, out Image img))
		{
			return img;
		}
		img = null;
		try
		{
			Texture2D tex = DormPlatform.LoadTexture("Content/dorm/" + name + ".png");
			img = new Image(tex);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Dorm art " + name + ": " + ex.Message);
		}
		mImages[name] = img;
		return img;
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

	/// <summary>Part of a sprite: source rectangle given as fractions of the image.</summary>
	private void DrawSpritePart(Graphics g, string name, TRect dest, float fx0, float fy0, float fx1, float fy1)
	{
		Image img = Img(name);
		if (img == null)
		{
			return;
		}
		TRect src = new TRect((int)(img.mWidth * fx0), (int)(img.mHeight * fy0), (int)(img.mWidth * (fx1 - fx0)), (int)(img.mHeight * (fy1 - fy0)));
		TRect dst = new TRect(dest.mX + (int)(dest.mWidth * fx0), dest.mY + (int)(dest.mHeight * fy0), (int)(dest.mWidth * (fx1 - fx0)), (int)(dest.mHeight * (fy1 - fy0)));
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(Color.White);
		g.DrawImage(img, dst, src);
		g.SetColorizeImages(colorizeImages: false);
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

	/// <summary>Fit the room (8 cells, walls ≈ 1.45 cells above the back corner) into a screen box.</summary>
	private void PlaceRoom(float left, float top, float right, float bottom)
	{
		mCW = Math.Min(Math.Min((right - left) / N, (bottom - top) / 5.55f), 100f);
		mCH = mCW / 2f;
		mOX = (left + right) / 2f;
		mOY = top + 1.45f * mCW + Math.Max(0f, (bottom - top - 5.55f * mCW) / 2f);
	}

	// ------------------------------------------------------------------ layout data

	/// <summary>A new dorm starts empty: every piece comes from the furniture shop.</summary>
	private void DefaultLayout()
	{
		mItems.Clear();
	}

	private int Owned(Def def) => def.Default ? 1 : (mOwned.TryGetValue(def.Id, out int n) ? n : 0);

	private int Owned() => mOwned.Values.Sum();

	private int Placed(Def def) => def.Kind == Kind.Style ? (InUse(def) ? 1 : 0) : mItems.Count((Item it) => it.Def == def) + PlacedElsewhere(def);

	/// <summary>Pieces still in the warehouse (owned and not in the room).</summary>
	private int Available(Def def) => Owned(def) - Placed(def);

	/// <summary>Buy one piece: coins go down, the piece goes to the warehouse. Saved at once (money was spent).</summary>
	private bool Buy(Def def)
	{
		if (!DormPlatform.SpendCoins(mLawn, def.Price))
		{
			return false;
		}
		AddOwned(def, def.Price);
		WriteSave();
		return true;
	}

	/// <summary>Put a girl into the dorm or send her back (角色管理).</summary>
	private bool ToggleGirl(string skel)
	{
		int k = mChosen.IndexOf(skel);
		if (k >= 0)
		{
			mChosen.RemoveAt(k);
			DormActor a = mActors.FirstOrDefault((DormActor t) => t.Name == skel);
			if (a != null)
			{
				if (mHeld == a)
				{
					mHeld = null;
				}
				if (mPress == a)
				{
					mPress = null;
				}
				mActors.Remove(a);
				a.Dispose();
			}
			WriteSave();
			return true;
		}
		if (mChosen.Count >= MaxGirls || FindGirl(skel) == null || !Unlocked(FindGirl(skel)))
		{
			return false;
		}
		mChosen.Add(skel);
		DormActor n = AddActor(skel);
		if (n != null)
		{
			Point p = RandomFreeCell(n);
			n.FI = p.X + 0.5f;
			n.FJ = p.Y + 0.5f;
			n.Timer = 1f;
			n.Hop = 1f;
		}
		WriteSave();
		return true;
	}

	private string Serialize()
	{
		List<string> lines = new List<string> { "# dorm v1: id i j rot side", "@wall " + mWallStyle, "@floor " + mFloorStyle };
		lines.AddRange(mItems.Select((Item it) => $"{it.Def.Id} {it.I} {it.J} {it.Rot} {it.Side}"));
		return string.Join("\n", lines);
	}

	private void Deserialize(string text)
	{
		mItems.Clear();
		mWallStyle = "wall_sakura";
		mFloorStyle = "floor_tatami";
		foreach (string line in text.Split('\n'))
		{
			string[] p = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (p.Length == 2 && (p[0] == "@wall" || p[0] == "@floor"))
			{
				Def sd = FindDef(p[1]);
				if (sd != null && sd.Kind == Kind.Style && Owned(sd) > 0 && sd.Layer == p[0].Substring(1))
				{
					ApplyStyleSilently(sd);
				}
				continue;
			}
			if (p.Length < 5 || p[0].StartsWith("#"))
			{
				continue;
			}
			Def def = FindDef(p[0]);
			if (def == null || !int.TryParse(p[1], out int i) || !int.TryParse(p[2], out int j) || !int.TryParse(p[3], out int rot) || !int.TryParse(p[4], out int side))
			{
				continue;
			}
			Item it = new Item { Def = def, I = i, J = j, Rot = rot & 3, Side = side };
			if (def.Kind == Kind.Style || Available(def) <= 0)
			{
				continue;
			}
			if (def.Kind == Kind.Wall ? WallSlotFree(it, it.Side, it.I) : FitsAt(it, it.I, it.J, it.Rot))
			{
				mItems.Add(it);
			}
		}
	}

	private bool Save()
	{
		mSnapshot = Serialize();
		return WriteSave();
	}

	private bool Dirty => Serialize() != mSnapshot;

	private int Comfort => mItems.Sum((Item it) => it.Def.Comfort) + (FindDef(mWallStyle)?.Comfort ?? 0) + (FindDef(mFloorStyle)?.Comfort ?? 0);

	private void ApplyStyleSilently(Def d)
	{
		if (d.Layer == "wall")
		{
			mWallStyle = d.Id;
		}
		else
		{
			mFloorStyle = d.Id;
		}
	}

	// ------------------------------------------------------------------ occupancy

	private static bool Covers(Item it, int i, int j) => it.Def.Kind != Kind.Wall && i >= it.I && j >= it.J && i < it.I + it.A && j < it.J + it.B;

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

	private Item FutonAt(int i, int j) => mItems.FirstOrDefault((Item t) => t.Def.Id == "futon" && Covers(t, i, j));

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

	private bool CellTaken(int i, int j, DormActor theExcept) => mActors.Any((DormActor a) => a != theExcept && a.Mode != DormActor.State.Bed && a.Mode != DormActor.State.Held && (int)a.FI == i && (int)a.FJ == j);

	private Point RandomFreeCell(DormActor theExcept)
	{
		List<Point> free = new List<Point>();
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				if (!Blocked(i, j) && !CellTaken(i, j, theExcept))
				{
					free.Add(new Point(i, j));
				}
			}
		}
		return free.Count > 0 ? free[mRand.Next(free.Count)] : new Point(N - 1, N - 1);
	}

	/// <summary>Free cell nearest to a grid point (where a dropped girl ends up when the spot itself is taken).</summary>
	private Point NearestFreeCell(float fi, float fj, DormActor theExcept)
	{
		Point best = new Point(N - 1, N - 1);
		float bestD = float.MaxValue;
		for (int i = 0; i < N; i++)
		{
			for (int j = 0; j < N; j++)
			{
				if (Blocked(i, j) || CellTaken(i, j, theExcept))
				{
					continue;
				}
				float d = (i + 0.5f - fi) * (i + 0.5f - fi) + (j + 0.5f - fj) * (j + 0.5f - fj);
				if (d < bestD)
				{
					bestD = d;
					best = new Point(i, j);
				}
			}
		}
		return best;
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

	private void ToIdle(DormActor a, float theWait)
	{
		a.Path.Clear();
		a.Mode = DormActor.State.Idle;
		a.StopWalk();
		a.PlayIdle();
		a.Timer = theWait;
	}

	private void PutToBed(DormActor a, Item futon)
	{
		a.Path.Clear();
		a.StopWalk();
		a.Mode = DormActor.State.Bed;
		a.BedItem = futon;
		a.Timer = BedTime;
		a.EnterBed();
		Hint(GirlName(a) + "睡着了 · 一分钟后会自己起床");
	}

	private void WakeUp(DormActor a)
	{
		Item futon = a.BedItem as Item;
		a.BedItem = null;
		a.LeaveBed();
		float ci = futon != null ? futon.I + futon.A / 2f : a.FI;
		float cj = futon != null ? futon.J + futon.B / 2f : a.FJ;
		Point p = NearestFreeCell(ci, cj, a);
		a.FI = p.X + 0.5f;
		a.FJ = p.Y + 0.5f;
		a.Hop = 1f;
		ToIdle(a, 1.5f);
	}

	private static string GirlName(DormActor a) => FindGirl(a.Name)?.Name ?? "植物娘";

	private void UpdateActor(DormActor a, float dt)
	{
		a.Advance(dt);
		a.Hop = Math.Max(0f, a.Hop - dt * 3f);
		switch (a.Mode)
		{
		case DormActor.State.Held:
		{
			// pendulum: the faster the finger moves sideways, the more she swings
			float target = Math.Clamp(-mHeldVX * 0.06f, -28f, 28f);
			a.SwingVel += ((target - a.Swing) * 60f - a.SwingVel * 7f) * dt;
			a.Swing += a.SwingVel * dt;
			return;
		}
		case DormActor.State.Seat:
			UpdateSeated(a, dt);
			return;
		case DormActor.State.Bed:
		{
			Item futon = a.BedItem as Item;
			if (futon == null || !mItems.Contains(futon) || futon == mDrag)
			{
				WakeUp(a);
				return;
			}
			a.Timer -= dt;
			if (a.Timer <= 0f)
			{
				WakeUp(a);
			}
			return;
		}
		}
		// furniture dropped on top of a girl: she steps aside
		if (Blocked((int)a.FI, (int)a.FJ) && a.Path.Count == 0)
		{
			Point p = NearestFreeCell(a.FI, a.FJ, a);
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
				if (a.Mode == DormActor.State.Idle && mPlaying.Count > 0 && mRand.Next(3) == 0 && TryDance(a))
				{
					break;
				}
				if (a.Mode == DormActor.State.Idle && mRand.Next(3) == 0 && TryGoSit(a))
				{
					break;
				}
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
					ToIdle(a, 1f + (float)mRand.NextDouble() * 2f);
				}
			}
			break;
		case DormActor.State.Walk:
		{
			if (a.Path.Count == 0)
			{
				if (a.PendingSeat is Item seatItem)
				{
					EnterSeat(a, seatItem, a.PendingIdx);
					break;
				}
				ToIdle(a, 2f + (float)mRand.NextDouble() * 3f);
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
			float step = 1.3f * dt;
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
			int after = (int)Math.Floor(a.WalkPhase / Math.PI);
			if (after != before && after % 2 == 0 && !a.Glides)
			{
				mDust.Add((SX(a.FI, a.FJ) + (a.FaceLeft ? 1 : -1) * mCW * 0.12f, SY(a.FI, a.FJ), 0f));
			}
			break;
		}
		}
	}

	// ------------------------------------------------------------------ carrying girls

	private void PickUp(DormActor a, float x, float y)
	{
		if (a.Mode == DormActor.State.Bed)
		{
			a.BedItem = null;
			a.LeaveBed();
		}
		if (a.Mode == DormActor.State.Seat && IsLounger(a.SeatItem as Item))
		{
			a.LeaveBed();
		}
		a.SeatItem = null;
		a.PendingSeat = null;
		a.SetSitting(DormActor.SitKind.None);
		a.Path.Clear();
		a.StopWalk();
		a.PlayIdle();
		a.React();
		a.Mode = DormActor.State.Held;
		DormPlatform.PlayVoice(a.Seed, 2);
		a.Swing = 0f;
		a.SwingVel = 0f;
		mHeld = a;
		mHeldX = x;
		mHeldY = y;
		mHeldVX = 0f;
		DormPlatform.PlaySound(Resources.SOUND_TAP);
	}

	private float HeldHeight => mCW * 1.4f;

	private float GloveW => mCW * 0.62f;

	private float GloveH
	{
		get
		{
			Image glove = AtlasResources.IMAGE_ZEN_GARDENGLOVE;
			return glove != null && glove.mWidth > 0 ? glove.mHeight * GloveW / glove.mWidth : GloveW;
		}
	}

	/// <summary>Where the carried girl's feet are: the finger holds the glove, the glove pinches the top of her head.</summary>
	private void HeldFeet(out float x, out float y)
	{
		x = mHeldX;
		y = mHeldY + GloveH * 0.4f + HeldHeight * 0.93f;
	}

	private void DropHeld()
	{
		DormActor a = mHeld;
		mHeld = null;
		if (a == null)
		{
			return;
		}
		HeldFeet(out float fx, out float fy);
		ToGrid(fx, fy, out float gi, out float gj);
		int ci = (int)Math.Floor(gi), cj = (int)Math.Floor(gj);
		Item futon = (ci >= 0 && cj >= 0 && ci < N && cj < N) ? FutonAt(ci, cj) : null;
		if (futon != null && !mActors.Any((DormActor o) => o != a && o.Mode == DormActor.State.Bed && o.BedItem == futon))
		{
			PutToBed(a, futon);
			return;
		}
		if (DropOnSeat(a, gi, gj))
		{
			return;
		}
		Point p = (ci >= 0 && cj >= 0 && ci < N && cj < N && !Blocked(ci, cj) && !CellTaken(ci, cj, a)) ? new Point(ci, cj) : NearestFreeCell(gi, gj, a);
		a.FI = p.X + 0.5f;
		a.FJ = p.Y + 0.5f;
		a.Hop = 0.6f;
		ToIdle(a, 2f);
	}

	// ------------------------------------------------------------------ update

	public override void Update()
	{
		base.Update();
		double now = mClock.Elapsed.TotalSeconds;
		float dt = (float)Math.Clamp(now - mLastTime, 0.0, 0.1);
		mLastTime = now;
		mTime += dt;
		mHeldVX *= Math.Max(0f, 1f - dt * 8f);
		foreach (DormActor a in mActors)
		{
			UpdateActor(a, dt);
		}
		UpdatePress(dt);
		UpdatePages(dt);
		UpdateSwings(dt);
		UpdateMedia();
		UpdateVisitor(dt);
		UpdateFloors(dt);
		Age(mHearts, dt, 1.4f);
		Age(mDust, dt, 0.45f);
		if (mHintTimer > 0f)
		{
			mHintTimer -= dt;
		}
		MarkDirty();
	}

	private static void Age(List<(float x, float y, float age)> theList, float dt, float theLife)
	{
		for (int k = theList.Count - 1; k >= 0; k--)
		{
			(float x, float y, float age) p = theList[k];
			p.age += dt;
			if (p.age > theLife)
			{
				theList.RemoveAt(k);
			}
			else
			{
				theList[k] = p;
			}
		}
	}

	// ------------------------------------------------------------------ room drawing

	private void DrawRoom(Graphics g)
	{
		DrawShell(g);
		DrawFloor(g);
		DrawShadows(g);
		DrawDust(g);
		DrawSorted(g);
		DrawHearts(g);
		DrawHeld(g);
	}

	/// <summary>Walls + one-piece tatami floor baked into one picture at 128 px per cell; origin = the floor's back corner.</summary>
	private const float ShellOX = 516f, ShellOY = 185f, ShellCell = 128f;

	private void DrawShell(Graphics g)
	{
		Image img = Img("room_shell");
		if (!DrawStyledShell(g))
		{
			if (img == null)
			{
				DrawWalls(g);
				return;
			}
			float s = mCW / ShellCell;
			DrawSprite(g, "room_shell", mOX - ShellOX * s, mOY - ShellOY * s, img.mWidth * s, img.mHeight * s);
		}
		foreach (Item it in mItems.Where((Item t) => t.Def.Kind == Kind.Wall))
		{
			DrawWallItem(g, it, it == mDrag ? (mDragValid ? new Color(200, 255, 200) : new Color(255, 160, 160)) : (mEdit && it == mSel ? new Color(255, 245, 190) : (Color?)null));
		}
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
			DrawWallItem(g, it, it == mDrag ? (mDragValid ? new Color(200, 255, 200) : new Color(255, 160, 160)) : (mEdit && it == mSel ? new Color(255, 245, 190) : (Color?)null));
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
		float w = mCW * 0.62f * it.Def.Scale;
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
		for (int s = 0; s <= 2 * (N - 1) && Img("room_shell") == null; s++)
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
		// carrying a girl over the futon: show where she will lie down
		if (mHeld != null)
		{
			HeldFeet(out float fx, out float fy);
			ToGrid(fx, fy, out float gi, out float gj);
			int ci = (int)Math.Floor(gi), cj = (int)Math.Floor(gj);
			Item futon = (ci >= 0 && cj >= 0 && ci < N && cj < N) ? FutonAt(ci, cj) : null;
			if (futon != null)
			{
				for (int i = futon.I; i < futon.I + futon.A; i++)
				{
					for (int j = futon.J; j < futon.J + futon.B; j++)
					{
						DrawSprite(g, "diamond", SX(i, j) - mCW / 2f, SY(i, j), mCW, mCH, new Color(120, 170, 255, 130));
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
		float cx = SX(it.I + it.A, it.J + it.B);
		float w = (right - left) * it.Def.Scale;
		left = cx - w * (cx - left) / (right - left);
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
		if (it.Def.Id == "swing")
		{
			DrawSwing(g, it, r, tint);
			return;
		}
		DrawSprite(g, it.Sprite, r.mX, r.mY, r.mWidth, r.mHeight, tint);
		if (IsTv(it))
		{
			DrawTvScreen(g, it, r, tint);
		}
		else if (it.Def.Id == "record_player")
		{
			DrawRecordPlayer(g, it, r, tint);
		}
		else if (IsChair(it))
		{
			DrawChairSitters(g, it, r);
		}
		if (it.Def.Id == "futon" && it != mDrag)
		{
			foreach (DormActor a in mActors.Where((DormActor t) => t.Mode == DormActor.State.Bed && t.BedItem == it))
			{
				DrawInBed(g, a, it, r);
			}
		}
	}

	/// <summary>
	/// A girl lying on the futon: her body along the futon's long side with the head on the pillows, breathing,
	/// the quilt half of the futon drawn again over her legs, and Zzz rising from her head.
	/// </summary>
	private void DrawInBed(Graphics g, DormActor a, Item futon, TRect r)
	{
		// pillows are on the right of the art for facings 0/2, on the left for 1/3 (quilt on the other side)
		bool pillowsRight = (futon.Rot & 1) == 0;
		float hx = r.mX + r.mWidth * (pillowsRight ? 0.78f : 0.22f);
		float hy = r.mY + r.mHeight * 0.28f;
		float body = r.mWidth * 0.5f;
		float tilt = pillowsRight ? 63.4f : -63.4f;
		float rad = MathHelper.ToRadians(tilt);
		// head sits 0.83 of her height above the feet, along the tilted body
		float footX = hx - (float)Math.Sin(rad) * body * 0.83f;
		float footY = hy + (float)Math.Cos(rad) * body * 0.83f;
		float breathe = 1f + 0.025f * (float)Math.Sin(mTime * 2f * Math.PI / 3.2f);
		a.FaceLeft = pillowsRight;
		a.Draw(g, footX, footY, body, tilt, breathe, 1f);
		// quilt pulled up to her shoulders
		DrawSprite(g, "futon_quilt_" + futon.Rot, r.mX, r.mY, r.mWidth, r.mHeight);
		// Zzz
		for (int k = 0; k < 3; k++)
		{
			float t = (mTime * 0.5f + k / 3f) % 1f;
			float size = mCW * (0.16f + 0.12f * t);
			int alpha = (int)(255 * Math.Min(1f, (1f - t) * 2f) * Math.Min(1f, t * 4f));
			DrawSprite(g, "ui/ic_z", hx + (pillowsRight ? 1 : -1) * t * mCW * 0.3f - size / 2f, hy - mCW * 0.35f - t * mCW * 0.6f - size / 2f, size, size, new Color(255, 255, 255, alpha));
		}
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
			if (a.Mode == DormActor.State.Held || a.Mode == DormActor.State.Bed || (a.Mode == DormActor.State.Seat && ((a.SeatItem as Item)?.Def.Id == "swing" || IsChair(a.SeatItem as Item))))
			{
				continue;
			}
			DormActor cur = a;
			if (a.Mode == DormActor.State.Seat)
			{
				// on a cushion: a little above the floor; at the kotatsu: the table hides her legs
				float lift2 = (a.SeatItem as Item)?.Def.Id == "cushion" ? mCW * 0.08f : IsTv(a.SeatItem as Item) ? 0f : mCW * 0.02f;
				float hop2 = -(float)Math.Sin(a.Hop * Math.PI) * mCW * 0.15f;
				list.Add((a.FI + a.FJ, 1, () => cur.Draw(g, SX(cur.FI, cur.FJ), SY(cur.FI, cur.FJ) - lift2 + hop2, height, 0f, 1f, 1f)));
				continue;
			}
			bool walking = a.Mode == DormActor.State.Walk;
			float wave = (float)Math.Sin(a.WalkPhase);
			float up = Math.Abs(wave);
			// quick little steps with the body kept upright (reference: Azur Lane dorm)
			float bob = walking ? -up * mCW * (a.HasCustomWalk ? 0.02f : 0.035f) : 0f;
			float tilt = walking ? wave * (a.HasCustomWalk ? 0.8f : 2f) : 0f;
			float squash = walking ? (float)Math.Pow(1f - up, 4) * 0.035f : 0f;
			float hop = -(float)Math.Sin(a.Hop * Math.PI) * mCW * 0.25f;
			list.Add((a.FI + a.FJ, 1, () => cur.Draw(g, SX(cur.FI, cur.FJ), SY(cur.FI, cur.FJ) + bob + hop, height, tilt, 1f + squash * 0.6f, 1f - squash)));
		}
		AddVisitorDraw(g, list);
		foreach (var e in list.OrderBy((x) => x.key).ThenBy((x) => x.order))
		{
			e.draw();
		}
	}

	/// <summary>Soft oval under each standing girl; shrinks a little while she is in the air.</summary>
	private void DrawShadows(Graphics g)
	{
		foreach (DormActor a in mActors)
		{
			if (a.Mode == DormActor.State.Bed || (a.Mode == DormActor.State.Seat && (a.SeatItem as Item)?.Def.Id != "cushion" && !IsTv(a.SeatItem as Item)))
			{
				continue;
			}
			float gx, gy, lift;
			if (a.Mode == DormActor.State.Held)
			{
				HeldFeet(out gx, out gy);
				gy += mCW * 0.15f;
				lift = 0.8f;
			}
			else
			{
				gx = SX(a.FI, a.FJ);
				gy = SY(a.FI, a.FJ);
				lift = (float)Math.Sin(a.Hop * Math.PI);
			}
			float w = mCW * 0.62f * (1f - 0.3f * lift);
			float h = w * 0.42f;
			DrawSprite(g, "dust", gx - w / 2f, gy - h / 2f, w, h, new Color(0, 0, 0, (int)(90 * (1f - 0.4f * lift))));
		}
	}

	private void DrawDust(Graphics g)
	{
		foreach ((float x, float y, float age) in mDust)
		{
			float t = age / 0.45f;
			float size = mCW * (0.08f + 0.12f * t);
			int alpha = (int)(120 * (1f - t));
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

	/// <summary>The carried girl, hanging from the finger and swinging.</summary>
	private void DrawHeld(Graphics g)
	{
		if (mHeld == null)
		{
			return;
		}
		HeldFeet(out float fx, out float fy);
		// rotate around the grip point: feet swing opposite to the tilt
		float rad = MathHelper.ToRadians(mHeld.Swing);
		// she swings around the finger, which holds the glove
		float r = fy - mHeldY;
		float footX = mHeldX - (float)Math.Sin(rad) * r;
		float footY = mHeldY + (float)Math.Cos(rad) * r;
		mHeld.Draw(g, footX, footY, HeldHeight, mHeld.Swing, 1f, 1f);
		Image glove = AtlasResources.IMAGE_ZEN_GARDENGLOVE;
		if (glove != null)
		{
			float gw = GloveW, gh = GloveH;
			// glove centred under the finger, its fingertips on the top of her head
			g.SetColorizeImages(colorizeImages: false);
			g.DrawImage(glove, new TRect((int)(mHeldX - gw * 0.5f), (int)(mHeldY - gh * 0.55f), (int)gw, (int)gh), new TRect(0, 0, glove.mWidth, glove.mHeight));
		}
	}

	// ------------------------------------------------------------------ girls: tapping

	private DormActor HitActor(int x, int y)
	{
		foreach (DormActor a in mActors.OrderByDescending((DormActor t) => t.FI + t.FJ))
		{
			if (a.Mode != DormActor.State.Held && In(a.LastRect, x, y))
			{
				return a;
			}
		}
		return null;
	}

	private void Pet(DormActor a)
	{
		if (a.Mode == DormActor.State.Bed || (a.Mode == DormActor.State.Seat && IsLounger(a.SeatItem as Item)))
		{
			Hint("嘘……" + GirlName(a) + "正在睡觉");
			return;
		}
		a.Hop = 1f;
		// zen-garden interaction: her garden reaction with the matching voice line
		int which = mRand.Next(2) + 1;
		if (a.Mode == DormActor.State.Seat)
		{
			// she stays seated; a swing goes higher for a while
			a.PlayGarden(which);
			a.Timer = Math.Max(a.Timer, 6f);
			if (a.SeatItem is Item seat && seat.Def.Id == "swing")
			{
				mSwingBoost[seat] = 1f;
			}
			mHearts.Add((SX(a.FI, a.FJ), a.LastRect.mY, 0f));
			DormPlatform.PlayVoice(a.Seed, which);
			return;
		}
		a.Path.Clear();
		a.StopWalk();
		a.Mode = DormActor.State.Idle;
		float len = a.PlayGarden(which);
		a.Timer = Math.Max(2.5f, len + 0.5f);
		mHearts.Add((SX(a.FI, a.FJ), a.LastRect.mY, 0f));
		DormPlatform.PlayVoice(a.Seed, which);
	}

	public override void Dispose()
	{
		if (mPlaying.Count > 0)
		{
			DormPlatform.PlayTune(mLawn, MusicTune.ZenGarden);
		}
		DormPlatform.StopDormMusic(mLawn, mPrevMusic);
		mRosterPreview?.Dispose();
		mRosterPreview = null;
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
}
