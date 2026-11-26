#if !__IOS__
// Desktop-only scripted run of the plant mods (PVZ_DORM_AUTOTEST=1 PVZ_DORM_SCENE=mods): seed chooser with the
// mod packets, planting a mod plant next to the plant it acts as, and a zombie to shoot at. Logs "MODTEST <step>".
using System;
using System.Linq;
using Sexy.TodLib;

namespace Lawn;

internal static class ModAutoTest
{
	private static int sFrame = -1;

	/// <summary>PVZ_MOD_UI=1: the main menu with the 「模组」 button, the mod screen, switching a pack off and on.</summary>
	private static void UiScene(LawnApp app, int s)
	{
		try
		{
			switch (s)
			{
			case 2:
				if (app.mTitleScreen != null)
				{
					app.LoadingCompleted();
				}
				break;
			case 3:
				if (app.mPlayerInfo == null)
				{
					app.mPlayerInfo = app.mProfileMgr.AddProfile("modtest") ?? app.mProfileMgr.GetProfile("modtest");
				}
				break;
			case 6:
			case 7:
				foreach (int id in app.mDialogMap.Keys.ToArray())
				{
					app.KillDialog(id);
				}
				break;
			case 9:
				Console.WriteLine("MODTEST menu");
				break;
			case 10:
				ModDialog.Open(app);
				break;
			case 12:
				Console.WriteLine("MODTEST dialog");
				break;
			case 13:
				(app.GetDialog(ModDialog.DIALOG_ID) as ModDialog)?.OnButton("toggle");
				break;
			case 14:
				Console.WriteLine("MODTEST toggled " + ModPlants.RestartNeeded);
				break;
			case 15:
				(app.GetDialog(ModDialog.DIALOG_ID) as ModDialog)?.OnButton("toggle");
				Console.WriteLine("MODTEST back " + ModPlants.RestartNeeded);
				break;
			case 17:
				Console.WriteLine("MODTEST end");
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("MODTEST error " + ex);
		}
	}

	public static void Tick(LawnApp app)
	{
		sFrame++;
		if (sFrame % 100 != 0)
		{
			return;
		}
		int s = sFrame / 100;
		Console.WriteLine("MODTEST t=" + s);
		if (Environment.GetEnvironmentVariable("PVZ_MOD_UI") == "1")
		{
			UiScene(app, s);
			return;
		}
		if (s >= 6 && s < 43)
		{
			foreach (int id in app.mDialogMap.Keys.ToArray())
			{
				app.KillDialog(id);
			}
		}
		ModPlantSpec mod = ModPlants.Plants.FirstOrDefault();
		try
		{
			switch (s)
			{
			case 2:
				if (app.mTitleScreen != null)
				{
					app.LoadingCompleted();
				}
				break;
			case 3:
				if (app.mPlayerInfo == null)
				{
					app.mPlayerInfo = app.mProfileMgr.AddProfile("modtest") ?? app.mProfileMgr.GetProfile("modtest");
				}
				app.KillDialog((int)Sexy.Dialogs.DIALOG_CREATEUSER);
				app.mPlayerInfo.SetLevel(30);
				app.mPlayerInfo.mFinishedAdventure = 1;
				app.mPlayerInfo.mPurchases[0] = 1;
				foreach (ModInfo m in ModPlants.Mods)
				{
					Console.WriteLine($"MODTEST pack {m.Id} '{m.Name}' enabled={m.Enabled} loaded={m.Loaded} error={m.Error} plants={m.Plants.Count}");
				}
				foreach (ModPlantSpec p in ModPlants.Plants)
				{
					Console.WriteLine($"MODTEST plant {p.Key} seed={(int)p.Seed} base={p.Base} reanim={(int)p.Reanim} cost={Plant.GetCost(p.Seed, SeedType.None)}");
				}
				break;
			case 5:
				app.KillGameSelector();
				app.PreNewGame(GameMode.SurvivalNormalStage1, theLookForSavedGame: false);
				break;
			case 9:
				app.mSeedChooserScreen?.mScrollWidget?.ScrollToBottom(animated: false);
				break;
			case 10:
				Console.WriteLine("MODTEST chooser");
				break;
			case 11:
				app.mSeedChooserScreen?.SeedSelected(SeedType.Sunflower);
				if (mod != null)
				{
					app.mSeedChooserScreen?.SeedSelected(mod.Seed);
				}
				foreach (SeedType t in new[] { SeedType.Peashooter, SeedType.Wallnut, SeedType.Cherrybomb, SeedType.Repeater, SeedType.Snowpea, SeedType.Squash, SeedType.Chomper, SeedType.Potatomine })
				{
					app.mSeedChooserScreen?.SeedSelected(t);
				}
				break;
			case 13:
				Console.WriteLine("MODTEST picked");
				break;
			case 14:
				app.mSeedChooserScreen?.OnStartButton();
				break;
			case 22:
				if (app.mBoard != null && mod != null)
				{
					app.mBoard.mSunMoney = 5000;
					app.mBoard.AddPlant(2, 1, mod.Seed, SeedType.None);
					app.mBoard.AddPlant(2, 3, mod.Base, SeedType.None);
					app.mBoard.AddZombieInRow(ZombieType.Normal, 1, 0);
					app.mBoard.AddZombieInRow(ZombieType.Normal, 3, 0);
				}
				break;
			case 24:
				foreach (Plant p in app.mBoard.mPlants)
				{
					Reanimation body = app.ReanimationTryToGet(p.mBodyReanimID);
					Console.WriteLine($"MODTEST lawn {p.mSeedType} mod={(int)p.mModSeed} hp={p.mPlantHealth} rate={p.mLaunchRate} body={body?.mOverlayMatrix.mMatrix.Translation} scale={body?.mOverlayMatrix.mMatrix.M11}");
				}
				Console.WriteLine("MODTEST planted");
				break;
			case 27:
				if (app.mBoard != null && mod != null)
				{
					// the player's way: tap the mod packet, then a lawn cell
					SeedPacket packet = app.mBoard.mSeedBank.mSeedPackets.FirstOrDefault((SeedPacket q) => q != null && q.mPacketType == mod.Seed);
					int sun = app.mBoard.mSunMoney;
					packet?.MouseDown(5, 5, 1);
					Console.WriteLine($"MODTEST cursor {app.mBoard.mCursorObject.mCursorType} {app.mBoard.mCursorObject.mType}");
					int px = app.mBoard.GridToPixelX(5, 0) + 40;
					int py = app.mBoard.GridToPixelY(5, 0) + 40;
					Console.WriteLine($"MODTEST can {app.mBoard.CanPlantAt(5, 0, mod.Seed, false)} peashooter {app.mBoard.CanPlantAt(5, 0, SeedType.Peashooter, false)} grid {app.mBoard.PlantingPixelToGridX(px, py, mod.Seed)},{app.mBoard.PlantingPixelToGridY(px, py, mod.Seed)} px {px},{py}");
					app.mBoard.MouseUpWithPlant((int)(px * Sexy.Constants.S), (int)(py * Sexy.Constants.S), 1);
					Plant planted = app.mBoard.GetTopPlantAt(5, 0, TopPlant.EatingOrder);
					Console.WriteLine($"MODTEST tapped plant={planted?.mSeedType} mod={(int)(planted?.mModSeed ?? SeedType.None)} sun {sun}->{app.mBoard.mSunMoney} refreshing={packet?.mRefreshing} time={packet?.mRefreshTime}");
				}
				break;
			case 30:
				Console.WriteLine("MODTEST shooting");
				break;
			case 40:
				Console.WriteLine("MODTEST later");
				break;
			case 42:
				app.mBoard.TryToSaveGame();
				Console.WriteLine("MODTEST saved");
				break;
			case 43:
				Console.WriteLine("MODTEST load " + app.TryLoadGame());
				break;
			case 44:
				foreach (Sexy.Dialog dlg in app.mDialogMap.Values.ToArray())
				{
					if (dlg is ContinueDialog cd)
					{
						cd.ButtonDepress(0);
					}
				}
				break;
			case 46:
				foreach (Plant p in app.mBoard.mPlants)
				{
					Console.WriteLine($"MODTEST loaded {p.mSeedType} mod={(int)p.mModSeed} hp={p.mPlantHealth} rate={p.mLaunchRate}");
				}
				Console.WriteLine("MODTEST reloaded");
				break;
			case 50:
				Console.WriteLine("MODTEST end");
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("MODTEST error " + ex);
		}
	}
}
#endif
