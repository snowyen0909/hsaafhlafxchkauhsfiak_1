#if !__IOS__
// Desktop-only scripted run of the dorm (env PVZ_DORM_AUTOTEST=1): opens it once loading is done and
// plays a fixed sequence (shop, roster, arranging, petting), logging "DORMTEST <step>" so screenshots can be timed.
using System;
using System.IO;
using Sexy;

namespace Lawn;

internal static class DormAutoTest
{
	private static readonly bool sOn = Environment.GetEnvironmentVariable("PVZ_DORM_AUTOTEST") == "1";

	private static int sFrame = -1;

	public static void Tick(LawnApp app)
	{
		if (!sOn || !app.mLoadingThreadCompleted || app.mWidgetManager == null)
		{
			return;
		}
		if (Environment.GetEnvironmentVariable("PVZ_DORM_MENU") == "1")
		{
			++sFrame;
			if (sFrame == 100 && app.mTitleScreen != null)
			{
				app.LoadingCompleted();
			}
			if (sFrame == 400)
			{
				Console.WriteLine("DORMTEST menu");
			}
			return;
		}
		sFrame++;
		DormDialog d = app.GetDialog(DormDialog.DIALOG_ID) as DormDialog;
		int s = sFrame / 100;
		int k = sFrame % 100;
		if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "input" && s >= 4)
		{
			InputScene(d, sFrame);
			return;
		}
		if (k != 0)
		{
			return;
		}
		Console.WriteLine("DORMTEST t=" + s);
		if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "save" && s >= 4)
		{
			SaveScene(app, d, s);
			return;
		}
		if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "quiz" && s >= 4)
		{
			QuizScene(d, s);
			return;
		}
		if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "furn" && s >= 4)
		{
			FurnScene(app, d, s);
			return;
		}
		if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "input" && s >= 4)
		{
			InputScene(d, sFrame);
			return;
		}
		if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "room" && s >= 4)
		{
			RoomScene(d, s);
			return;
		}
		switch (s)
		{
		case 3:
		{
			Console.WriteLine($"DORMTEST icons bg={AtlasResources.IMAGE_MENUSTYLEFREE_BG?.mWidth}x{AtlasResources.IMAGE_MENUSTYLEFREE_BG?.mHeight} user={AtlasResources.IMAGE_MENUSTYLEFREE_USER?.mWidth}x{AtlasResources.IMAGE_MENUSTYLEFREE_USER?.mHeight} comm={AtlasResources.IMAGE_MENUSTYLEFREE_COMMUNITY?.mWidth}x{AtlasResources.IMAGE_MENUSTYLEFREE_COMMUNITY?.mHeight}");
			string dir = Path.GetDirectoryName(DormDialog.SavePath);
			if (Environment.GetEnvironmentVariable("PVZ_DORM_KEEP") != "1")
			{
				foreach (string f in new[] { "dorm.txt", "dorm_owned.txt", "dorm_girls.txt", "dorm_quiz.txt" })
				{
					if (File.Exists(Path.Combine(dir, f)))
					{
						File.Delete(Path.Combine(dir, f));
					}
				}
				string mod = Path.Combine(dir, "mod");
				if (Directory.Exists(mod))
				{
					foreach (string f in Directory.GetFiles(mod, "dorm.json*", SearchOption.AllDirectories))
					{
						File.Delete(f);
					}
				}
				if (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "save")
				{
					// an old (shared, text) save to be moved into this profile's dorm.json
					Directory.CreateDirectory(dir);
					File.WriteAllLines(Path.Combine(dir, "dorm_owned.txt"), new[] { "sofa2 2", "cushion 1", "wall_modern 1" });
					File.WriteAllLines(Path.Combine(dir, "dorm.txt"), new[] { "# dorm v1", "@wall wall_modern", "@floor floor_tatami", "sofa2 2 2 0 0", "cushion 5 5 0 0" });
					File.WriteAllLines(Path.Combine(dir, "dorm_quiz.txt"), new[] { "day=2026-01-01", "today=1", "right=7", "seen=art,comic" });
				}
			}
			string girls = Environment.GetEnvironmentVariable("PVZ_DORM_GIRLS");
			if (!string.IsNullOrEmpty(girls))
			{
				Directory.CreateDirectory(dir);
				File.WriteAllLines(Path.Combine(dir, "dorm_girls.txt"), girls.Split(','));
			}
			if (app.mPlayerInfo == null)
			{
				app.mPlayerInfo = app.mProfileMgr.AddProfile("dormtest") ?? app.mProfileMgr.GetProfile("dormtest");
			}
			if (app.mPlayerInfo != null && (Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "furn" || Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "room"))
			{
				app.mPlayerInfo.AddCoins(20000);
			}
			if (app.mPlayerInfo != null && app.mPlayerInfo.mCoins < 500)
			{
				app.mPlayerInfo.AddCoins(500);
			}
			Console.WriteLine("DORMTEST player=" + (app.mPlayerInfo == null ? "null" : app.mPlayerInfo.mCoins.ToString()));
			DormDialog.Open(app);
			Console.WriteLine("DORMTEST open");
			break;
		}
		case 5:
			d?.TestButton("edit");
			Console.WriteLine("DORMTEST emptyedit");
			break;
		case 6:
			d?.TestButton("edit");
			d?.TestButton("shop");
			Console.WriteLine("DORMTEST shop");
			break;
		case 7:
			d?.TestOpenBuy("swing");
			Console.WriteLine("DORMTEST buyconfirm");
			break;
		case 8:
			d?.TestButton("b_buy");
			foreach (string id in new[] { "kotatsu", "cushion", "cushion", "futon", "rug", "planter", "lantern", "wall_forest", "floor_darkwood", "wall_starry", "floor_starry" })
			{
				d?.TestBuy(id);
			}
			Console.WriteLine("DORMTEST bought");
			break;
		case 9:
			d?.TestButton("warehouse");
			Console.WriteLine("DORMTEST store");
			break;
		case 10:
			d?.TestSell("cushion");
			Console.WriteLine("DORMTEST sell");
			break;
		case 11:
			d?.TestButton("sl_ok");
			Console.WriteLine("DORMTEST sold");
			break;
		case 13:
			d?.TestButton("edit");
			d?.TestStyle("wall_forest", "floor_darkwood");
			foreach (string id in new[] { "swing", "kotatsu", "cushion", "futon", "rug", "planter", "lantern" })
			{
				d?.TestAdd(id);
			}
			d?.TestButton("ok");
			Console.WriteLine("DORMTEST edit");
			break;
		case 15:
			d?.TestButton("save");
			d?.TestButton("edit");
			Console.WriteLine("DORMTEST room");
			break;
		case 16:
			d?.TestSit(0, "swing", 0);
			d?.TestSit(1, "kotatsu", 0);
			d?.TestSit(2, "kotatsu", 1);
			d?.TestSit(3, "cushion", 0);
			Console.WriteLine("DORMTEST sit");
			break;
		case 18:
			d?.TestPet(0);
			Console.WriteLine("DORMTEST pet");
			break;
		case 19:
			d?.TestStand();
			Console.WriteLine("DORMTEST stand");
			break;
		case 21:
			d?.TestButton("warehouse");
			d?.TestButton("recall");
			Console.WriteLine("DORMTEST recall");
			break;
		}
	}

	/// <summary>PVZ_DORM_SCENE=save: old saves moved into dorm.json, prices, selling at half the price paid, test menu, 宿舍管理.</summary>
	private static void SaveScene(LawnApp app, DormDialog d, int s)
	{
		if (d == null)
		{
			return;
		}
		string json = Path.Combine(DormPlatform.ModSaveDir, "profile" + DormPlatform.ProfileId(app), "dorm.json");
		switch (s)
		{
		case 4:
			Console.WriteLine("DORMTEST migrated json=" + File.Exists(json) + " old=" + File.Exists(Path.Combine(DormPlatform.SaveDir, "dorm_owned.txt.old")));
			Console.WriteLine(File.Exists(json) ? File.ReadAllText(json).Replace("\n", " ").Replace("\r", "") : "(none)");
			Console.WriteLine("DORMTEST room");
			break;
		case 6:
			d.TestButton("shop");
			Console.WriteLine("DORMTEST shop");
			break;
		case 8:
			d.TestOpenBuy("record_player");
			Console.WriteLine("DORMTEST buyconfirm coins=" + DormPlatform.Coins(app));
			break;
		case 9:
			d.TestButton("b_buy");
			Console.WriteLine("DORMTEST bought coins=" + DormPlatform.Coins(app));
			break;
		case 10:
			d.TestButton("warehouse");
			Console.WriteLine("DORMTEST warehouse");
			break;
		case 12:
		{
			int c0 = DormPlatform.Coins(app);
			d.TestSell("record_player");
			Console.WriteLine("DORMTEST sellconfirm");
			break;
		}
		case 13:
		{
			int c0 = DormPlatform.Coins(app);
			d.TestButton("sl_ok");
			Console.WriteLine("DORMTEST sold refund=" + (DormPlatform.Coins(app) - c0));
			d.TestSell("sofa2");
			break;
		}
		case 14:
		{
			int c0 = DormPlatform.Coins(app);
			d.TestButton("sl_ok");
			Console.WriteLine("DORMTEST sold sofa refund=" + (DormPlatform.Coins(app) - c0));
			break;
		}
		case 16:
			d.TestRoster(all: true, filter: 0, select: "sunflower", manage: false);
			Console.WriteLine("DORMTEST roster");
			break;
		case 18:
			d.TestRoster(all: true, filter: 0, select: "snowpea", manage: true);
			d.TestRoster(all: true, filter: 0, select: "repeater", manage: true);
			Console.WriteLine("DORMTEST manage");
			break;
		case 20:
			d.TestRoster(all: false, filter: 0, select: "sunflower", manage: false);
			Console.WriteLine("DORMTEST inDorm");
			break;
		case 22:
			app.mPlayerInfo.AddCoins(5000);
			d.TestButton("hall");
			d.TestUnlockFloor();
			Console.WriteLine("DORMTEST unlockask");
			break;
		case 23:
			d.TestUnlockOk();
			Console.WriteLine("DORMTEST floor2 coins=" + DormPlatform.Coins(app));
			break;
		case 24:
			d.TestMoveGirl("sunflower", 1);
			d.TestMoveGirl("blover", 1);
			d.TestPlace("sofa1", 3, 3, 0);
			d.TestButton("save");
			Console.WriteLine("DORMTEST floor2room");
			break;
		case 26:
			d.TestRoster(all: true, filter: 0, select: "sunflower", manage: true);
			Console.WriteLine("DORMTEST manage2");
			break;
		case 28:
			d.TestButton("hall");
			d.TestSwitchFloor(0);
			Console.WriteLine("DORMTEST back1f");
			break;
		case 30:
			Console.WriteLine(File.Exists(json) ? File.ReadAllText(json) : "(none)");
			Console.WriteLine("DORMTEST done");
			break;
		}
	}

	/// <summary>PVZ_DORM_SCENE=quiz: the wise tree visits, 3 questions (right, wrong, right), result.</summary>
	private static void QuizScene(DormDialog d, int s)
	{
		if (d == null)
		{
			return;
		}
		switch (s)
		{
		case 4:
			d.TestVisit();
			Console.WriteLine("DORMTEST visitor");
			break;
		case 6:
			d.TestOpenQuiz();
			Console.WriteLine("DORMTEST q1");
			break;
		case 8:
			d.TestAnswer(true);
			Console.WriteLine("DORMTEST right");
			break;
		case 10:
			d.TestNext();
			Console.WriteLine("DORMTEST q2");
			break;
		case 12:
			d.TestAnswer(false);
			Console.WriteLine("DORMTEST wrong");
			break;
		case 14:
			d.TestNext();
			Console.WriteLine("DORMTEST q3");
			break;
		case 15:
			d.TestAnswer(true);
			break;
		case 16:
			d.TestNext();
			Console.WriteLine("DORMTEST result");
			break;
		case 18:
			d.TestCloseQuiz();
			Console.WriteLine("DORMTEST closed");
			break;
		case 20:
			Console.WriteLine("DORMTEST done");
			break;
		}
	}

	/// <summary>PVZ_DORM_SCENE=furn: the modern room (t=5..), the botanist's study (t=11..), back views (t=17..).</summary>
	/// <summary>Real pointer paths through MouseDown / MouseDrag / MouseUp, and a faked two-finger pinch (frame by frame).</summary>
	private static void InputScene(DormDialog d, int f)
	{
		if (d == null)
		{
			return;
		}
		int t = f - 400;
		if (t == 0)
		{
			d.TestBuy("floor_redcarpet");
			d.TestBuy("floor_grass");
			d.TestButton("edit");
			d.TestBrush("floor_redcarpet");
			Console.WriteLine("DORMTEST i_ready");
		}
		// paint: press on cell (2,2), drag along to (2,6)
		if (t == 20)
		{
			(float x, float y) p = d.TestCellPoint(2.5f, 2.5f);
			d.TestPointer(0, (int)p.x, (int)p.y);
		}
		if (t > 20 && t <= 60)
		{
			(float x, float y) p = d.TestCellPoint(2.5f, 2.5f + 4f * (t - 20) / 40f);
			d.TestPointer(1, (int)p.x, (int)p.y);
		}
		if (t == 61)
		{
			(float x, float y) p = d.TestCellPoint(2.5f, 6.5f);
			d.TestPointer(2, (int)p.x, (int)p.y);
			Console.WriteLine("DORMTEST i_painted");
		}
		// eyedropper: pick the grass-less tatami at (5,5) then paint (6,2)
		if (t == 100)
		{
			d.TestButton("t_pick");
			(float x, float y) p = d.TestCellPoint(2.5f, 4.5f);
			d.TestPointer(0, (int)p.x, (int)p.y);
			d.TestPointer(2, (int)p.x, (int)p.y);
			Console.WriteLine("DORMTEST i_picked");
		}
		if (t == 140)
		{
			d.TestButton("t_paint");
			d.TestSaveLayout();
			d.TestButton("edit");
			Console.WriteLine("DORMTEST i_normal");
		}
		// pinch out around the room centre
		if (t >= 200 && t < 260)
		{
			(float x, float y) c = d.TestCellPoint(4f, 4f);
			d.TestPinch(c.x, c.y, 100f + (t - 200) * 4f);
		}
		if (t == 260)
		{
			d.TestPinch(0, 0, 0);
			Console.WriteLine("DORMTEST i_pinched");
		}
		// one finger pan on empty floor
		if (t == 300)
		{
			d.TestPointer(0, 500, 300);
		}
		if (t > 300 && t <= 330)
		{
			d.TestPointer(1, 500 - (t - 300) * 6, 300 - (t - 300) * 3);
		}
		if (t == 331)
		{
			d.TestPointer(2, 320, 210);
			Console.WriteLine("DORMTEST i_panned");
		}
		if (t == 400)
		{
			Console.WriteLine("DORMTEST done");
		}
	}

	/// <summary>Room size, tiles, hidden walls, zoom, trophies.</summary>
	private static void RoomScene(DormDialog d, int s)
	{
		// PVZ_DORM_STOP=n: stop after step n so the driver can try real input there
		if (d == null || (int.TryParse(Environment.GetEnvironmentVariable("PVZ_DORM_STOP"), out int stop) && s > stop))
		{
			return;
		}
		switch (s)
		{
		case 4:
			foreach (string id in new[] { "floor_white", "floor_checker", "floor_redcarpet", "floor_flowertile", "floor_grass", "floor_marble2", "floor_bluecarpet", "wall_european", "floor_lightwood" })
			{
				d.TestBuy(id);
			}
			d.TestPlace("sofa2", 1, 3, 1);
			d.TestPlace("kotatsu", 3, 3, 0);
			d.TestPlace("trophy_gold", 0, 6, 0);
			d.TestPlace("trophy_silver", 0, 7, 0);
			d.TestPlace("painting", 3, 0, 1);
			d.TestSaveLayout();
			Console.WriteLine("DORMTEST r8");
			break;
		case 5:
			d.TestExpandAsk();
			Console.WriteLine("DORMTEST rask");
			break;
		case 6:
			d.BackButtonPress();
			d.TestExpand();
			Console.WriteLine("DORMTEST r10");
			break;
		case 7:
			d.TestExpand();
			Console.WriteLine("DORMTEST r12");
			break;
		case 8:
			d.TestButton("edit");
			d.TestBrush("floor_redcarpet");
			for (int i = 4; i < 9; i++)
			{
				for (int j = 4; j < 9; j++)
				{
					d.TestTile((i + j) % 2 == 0 ? "floor_redcarpet" : "floor_flowertile", i, j);
				}
			}
			for (int k = 0; k < 12; k++)
			{
				d.TestTile("floor_checker", k, 11);
				d.TestTile("floor_checker", 11, k);
				d.TestTile("floor_grass", k, 10);
				d.TestTile("floor_grass", 10, k);
			}
			Console.WriteLine("DORMTEST rpaint");
			break;
		case 9:
			d.TestSaveLayout();
			d.TestButton("edit");
			Console.WriteLine("DORMTEST rsaved");
			break;
		case 10:
			d.TestWalls(true);
			Console.WriteLine("DORMTEST rnowall");
			break;
		case 11:
			d.TestWalls(false);
			d.TestZoom(2.2f);
			Console.WriteLine("DORMTEST rzoom");
			break;
		case 12:
			d.TestPan(-260, -120);
			Console.WriteLine("DORMTEST rpan");
			break;
		case 13:
			d.TestResetZoom();
			d.TestButton("shop");
			d.TestShopTab(5, 1);
			Console.WriteLine("DORMTEST rshop");
			break;
		case 14:
			d.TestShopTab(3);
			Console.WriteLine("DORMTEST rshopfloor");
			break;
		case 15:
			d.BackButtonPress();
			d.TestFill("floor_lightwood");
			Console.WriteLine("DORMTEST rwood");
			break;
		case 16:
			Console.WriteLine("DORMTEST done");
			break;
		}
	}

	private static void FurnScene(LawnApp app, DormDialog d, int s)
	{
		if (d == null)
		{
			return;
		}
		switch (s)
		{
		case 4:
			foreach (string id in new[] { "wall_modern", "floor_lightwood", "wall_forest", "floor_darkwood", "wall_european", "floor_marble" })
			{
				d.TestBuy(id);
			}
			break;
		case 5:
			d.TestClear();
			d.TestStyle("wall_modern", "floor_lightwood");
			d.TestPlace("tv_stand", 2, 0, 0);
			d.TestPlace("sofa2", 0, 3, 1);
			d.TestPlace("coffee_table", 2, 4, 0);
			d.TestPlace("bed", 5, 0, 0);
			d.TestPlace("wardrobe", 7, 0, 0);
			d.TestPlace("drawer", 4, 0, 0);
			d.TestPlace("floor_lamp", 0, 2, 0);
			d.TestPlace("plant_big", 1, 0, 0);
			d.TestPlace("bookshelf_modern", 0, 0, 1);
			d.TestPlace("desk", 0, 6, 1);
			d.TestPlace("sofa1", 5, 3, 0);
			d.TestPlace("record_player", 7, 3, 1);
			d.TestPlace("plant_small", 7, 7, 0);
			d.TestPlace("pillow_blue", 4, 6, 0);
			d.TestPlace("pillow_white", 5, 6, 0);
			d.TestPlace("tissue", 6, 6, 0);
			d.TestPlace("books_modern", 3, 7, 0);
			d.TestPlace("diffuser", 7, 5, 0);
			d.TestPlace("painting", 3, 0, 1);
			d.TestPlace("clock", 6, 0, 1);
			d.TestPlace("wall_shelf", 4, 0, 0);
			d.TestButton("save");
			break;
		case 6:
			d.TestTap("tv_stand");
			d.TestTap("record_player");
			d.TestSit(0, "sofa2", 0);
			d.TestSit(1, "sofa2", 1);
			d.TestSit(2, "bed", 0);
			d.TestSit(3, "tv_stand", 0);
			Console.WriteLine("DORMTEST modern");
			break;
		case 9:
			d.TestSit(2, "sofa1", 0);
			Console.WriteLine("DORMTEST modern2");
			break;
		case 11:
			d.TestTap("record_player");
			d.TestStand();
			d.TestClear();
			d.TestStyle("wall_forest", "floor_darkwood");
			d.TestPlace("bookcase", 1, 0, 0);
			d.TestPlace("specimen_cabinet", 2, 0, 0);
			d.TestPlace("drawer", 4, 0, 0);
			d.TestPlace("armchair", 4, 2, 0);
			d.TestPlace("chaise", 0, 3, 1);
			d.TestPlace("round_table", 2, 4, 0);
			d.TestPlace("street_lamp", 0, 1, 0);
			d.TestPlace("plant_leaf", 7, 0, 0);
			d.TestPlace("bell_jar", 6, 2, 0);
			d.TestPlace("study_rug", 3, 4, 0);
			d.TestPlace("tube_rack", 6, 5, 0);
			d.TestPlace("book_pile", 7, 6, 0);
			d.TestPlace("inkwell", 4, 7, 0);
			d.TestPlace("potion", 5, 7, 0);
			d.TestPlace("book_bundle", 6, 7, 0);
			d.TestPlace("tv", 6, 0, 0);
			d.TestPlace("herbarium", 3, 0, 1);
			d.TestPlace("leaf_frame", 6, 0, 1);
			d.TestPlace("ivy", 2, 0, 0);
			d.TestPlace("leaf_plaque", 5, 0, 0);
			break;
		case 12:
			d.TestTap("tv");
			d.TestSit(0, "armchair", 0);
			d.TestSit(1, "chaise", 0);
			d.TestSit(2, "tv", 0);
			Console.WriteLine("DORMTEST study");
			break;
		case 17:
			d.TestStand();
			d.TestClear();
			d.TestStyle("wall_european", "floor_marble");
			string[] ids = { "sofa2", "sofa1", "armchair", "chaise", "plant_leaf", "bookcase", "specimen_cabinet", "record_player", "coffee_table", "bed", "tv_stand", "desk" };
			int[,] at = { { 0, 0 }, { 3, 0 }, { 5, 0 }, { 0, 2 }, { 3, 2 }, { 6, 2 }, { 7, 4 }, { 0, 5 }, { 2, 5 }, { 5, 4 }, { 3, 7 }, { 6, 6 } };
			for (int k = 0; k < ids.Length; k++)
			{
				d.TestPlace(ids[k], at[k, 0], at[k, 1], k % 2 == 0 ? 2 : 3);
			}
			Console.WriteLine("DORMTEST backs");
			break;
		case 18:
			d.TestButton("shop");
			Console.WriteLine("DORMTEST shopnew");
			break;
		case 19:
			Console.WriteLine("DORMTEST done");
			break;
		}
	}
}
#endif
