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
		bool at = sFrame % 100 == 0;
		if (!at)
		{
			if (d != null && sFrame >= 1300 && sFrame < 1400)
			{
				int k = sFrame - 1300;
				d.TestDrag(k);
			}
			return;
		}
		Console.WriteLine("DORMTEST t=" + s);
		switch (s)
		{
		case 3:
			if (System.IO.File.Exists(GlobalStaticVars.GetDocumentsDir() + "userdata/dorm.txt"))
			{
				System.IO.File.Delete(GlobalStaticVars.GetDocumentsDir() + "userdata/dorm.txt");
			}
			DormDialog.Open(app);
			Console.WriteLine("DORMTEST open");
			break;
		case 8:
			d?.TestPetFirst();
			Console.WriteLine("DORMTEST pet");
			break;
		case 11:
			d?.TestButton("edit");
			Console.WriteLine("DORMTEST edit");
			break;
		case 13:
			d?.TestBeginDrag("kotatsu");
			Console.WriteLine("DORMTEST drag");
			break;
		case 15:
			d?.TestEndDrag();
			d?.TestButton("rotate");
			Console.WriteLine("DORMTEST dropped");
			break;
		case 17:
			d?.TestButton("edit");
			Console.WriteLine("DORMTEST done");
			break;
		}
	}
}
#endif
