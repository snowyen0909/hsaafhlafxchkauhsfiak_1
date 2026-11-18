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
		case 6:
			d?.TestButton("shop");
			Console.WriteLine("DORMTEST shop");
			break;
		case 7:
			d?.TestOpenBuy("kotatsu");
			Console.WriteLine("DORMTEST buyconfirm");
			break;
		case 8:
			d?.TestButton("b_buy");
			foreach (string id in new[] { "rug", "futon", "planter", "lantern", "bookshelf", "tv", "cushion", "cushion", "sunflower_pillow", "pot_hat" })
			{
				d?.TestBuy(id);
			}
			Console.WriteLine("DORMTEST bought");
			break;
		case 9:
			d?.TestButton("roster");
			Console.WriteLine("DORMTEST roster");
			break;
		case 11:
			d?.TestRoster("snowpea");
			Console.WriteLine("DORMTEST roster-sel");
			break;
		case 13:
			d?.TestButton("edit");
			foreach (string id in new[] { "rug", "kotatsu", "futon", "planter", "lantern", "bookshelf", "tv", "cushion", "cushion", "sunflower_pillow", "pot_hat" })
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
		case 17:
			d?.TestPet(0);
			d?.TestPet(1);
			Console.WriteLine("DORMTEST pet");
			break;
		}
	}
}
#endif
