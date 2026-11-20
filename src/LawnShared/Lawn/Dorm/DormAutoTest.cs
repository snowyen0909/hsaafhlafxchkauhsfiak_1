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
		if (k != 0)
		{
			return;
		}
		Console.WriteLine("DORMTEST t=" + s);
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
		switch (s)
		{
		case 3:
		{
			Console.WriteLine($"DORMTEST icons bg={AtlasResources.IMAGE_MENUSTYLEFREE_BG?.mWidth}x{AtlasResources.IMAGE_MENUSTYLEFREE_BG?.mHeight} user={AtlasResources.IMAGE_MENUSTYLEFREE_USER?.mWidth}x{AtlasResources.IMAGE_MENUSTYLEFREE_USER?.mHeight} comm={AtlasResources.IMAGE_MENUSTYLEFREE_COMMUNITY?.mWidth}x{AtlasResources.IMAGE_MENUSTYLEFREE_COMMUNITY?.mHeight}");
			string dir = Path.GetDirectoryName(DormDialog.SavePath);
			if (Environment.GetEnvironmentVariable("PVZ_DORM_KEEP") != "1")
			{
				foreach (string f in new[] { "dorm.txt", "dorm_owned.txt", "dorm_girls.txt" })
				{
					if (File.Exists(Path.Combine(dir, f)))
					{
						File.Delete(Path.Combine(dir, f));
					}
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
			if (app.mPlayerInfo != null && Environment.GetEnvironmentVariable("PVZ_DORM_SCENE") == "furn")
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
