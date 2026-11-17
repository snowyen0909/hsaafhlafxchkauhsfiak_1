#if !__IOS__
// Desktop-only scripted run of the dorm (env PVZ_DORM_AUTOTEST=1): opens it once loading is done and
// plays a fixed sequence of taps and drags, logging "DORMTEST <step>" so screenshots can be timed.
using System;
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
		if (d != null)
		{
			if (s == 7 && k > 0 && k <= 60)
			{
				d.TestCarry(1, k);
			}
			if (s == 13 && k > 0)
			{
				d.TestDrag(k);
			}
		}
		if (k != 0)
		{
			return;
		}
		Console.WriteLine("DORMTEST t=" + s);
		switch (s)
		{
		case 3:
			if (Environment.GetEnvironmentVariable("PVZ_DORM_KEEP") != "1" && System.IO.File.Exists(DormDialog.SavePath))
			{
				System.IO.File.Delete(DormDialog.SavePath);
			}
			DormDialog.Open(app);
			Console.WriteLine("DORMTEST open");
			break;
		case 7:
			d?.TestCarry(1, 0);
			Console.WriteLine("DORMTEST carry");
			break;
		case 9:
			d?.TestDrop();
			Console.WriteLine("DORMTEST dropped-girl");
			break;
		case 11:
			d?.TestButton("edit");
			Console.WriteLine("DORMTEST edit");
			break;
		case 12:
			d?.TestSelect("kotatsu");
			Console.WriteLine("DORMTEST selected");
			break;
		case 13:
			d?.TestBeginDrag("kotatsu");
			Console.WriteLine("DORMTEST drag");
			break;
		case 15:
			d?.TestEndDrag();
			Console.WriteLine("DORMTEST dropped");
			break;
		case 16:
			d?.TestButton("back");
			Console.WriteLine("DORMTEST confirm");
			break;
		case 18:
			d?.TestButton("c_save");
			Console.WriteLine("DORMTEST done");
			break;
		}
	}
}
#endif
