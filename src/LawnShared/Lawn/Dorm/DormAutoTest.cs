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
		sFrame++;
		DormDialog d = app.GetDialog(DormDialog.DIALOG_ID) as DormDialog;
		int s = sFrame / 100;
		int k = sFrame % 100;
		if (k != 0)
		{
			return;
		}
		Console.WriteLine("DORMTEST t=" + s);
		switch (s)
		{
		case 3:
		{
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
		case 21:
			d?.TestButton("warehouse");
			d?.TestButton("recall");
			Console.WriteLine("DORMTEST recall");
			break;
		}
	}
}
#endif
