#if !__IOS__
// Desktop-only scripted run of 塔维尔's plants (PVZ_DORM_AUTOTEST=1 PVZ_DORM_SCENE=tavel): the 选项 switch, the chooser
// with the pack's cards, all twelve plants on one lawn against zombies, their ultimates, a save and a reload.
// Logs "TAVELTEST <step>" so screenshots can be timed. PVZ_TAVEL_UI=1: only the options dialog.
using System;
using System.Linq;
using Sexy;

namespace Lawn;

internal static class TavelAutoTest
{
	private static int sFrame = -1;

	private static void Say(string t) => Console.WriteLine("TAVELTEST " + t);

	private static void KillDialogs(LawnApp app)
	{
		foreach (int id in app.mDialogMap.Keys.ToArray())
		{
			app.KillDialog(id);
		}
	}

	private static void Lawn(LawnApp app, string theWhat)
	{
		foreach (Plant p in app.mBoard.mPlants.Where((Plant q) => !q.mDead))
		{
			TavelSpec spec = Tavel.GetFor(p);
			Say($"{theWhat} {p.mPlantCol},{p.mRow} {(spec != null ? spec.Code : p.mSeedType.ToString())} hp={p.mPlantHealth} state={p.mTavel?.GetType().Name}");
		}
		Say($"{theWhat} zombies={app.mBoard.mZombies.Count((Zombie z) => !z.mDead)} stock={app.mBoard.mTavel?.UltStock}");
	}

	public static void Tick(LawnApp app)
	{
		sFrame++;
		if (sFrame % 100 != 0)
		{
			return;
		}
		int s = sFrame / 100;
		try
		{
			if (Environment.GetEnvironmentVariable("PVZ_TAVEL_UI") == "1")
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
						app.mPlayerInfo = app.mProfileMgr.AddProfile("taveltest") ?? app.mProfileMgr.GetProfile("taveltest");
					}
					break;
				case 6:
				case 7:
					KillDialogs(app);
					break;
				case 8:
					app.DoNewOptions(theFromGameSelector: true);
					break;
				case 10:
					Say("options " + Tavel.Enabled);
					break;
				case 11:
					(app.GetDialog(Dialogs.DIALOG_NEWOPTIONS) as NewOptionsDialog)?.mTavelCheckbox?.SetChecked(!Tavel.Enabled, tellListener: true);
					break;
				case 12:
					Say("toggled " + Tavel.Enabled);
					break;
				case 14:
					Say("end");
					break;
				}
				return;
			}
			if (s >= 6 && s < 46)
			{
				KillDialogs(app);
			}
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
					app.mPlayerInfo = app.mProfileMgr.AddProfile("taveltest") ?? app.mProfileMgr.GetProfile("taveltest");
				}
				app.KillDialog((int)Dialogs.DIALOG_CREATEUSER);
				app.mPlayerInfo.SetLevel(30);
				app.mPlayerInfo.mFinishedAdventure = 1;
				Tavel.Enabled = Environment.GetEnvironmentVariable("PVZ_TAVEL_OFF") != "1";
				Say($"plants {Tavel.Plants.Count} chooser {Tavel.Count} enabled {Tavel.Enabled}");
				foreach (TavelSpec p in Tavel.Plants)
				{
					Say($"plant {p.Code} seed={p.SeedId} reanim={(int)p.Reanim} cost={Plant.GetCost(p.Seed, SeedType.None)} name={Plant.GetNameString(p.Seed, SeedType.None)}");
				}
				break;
			case 5:
				KillDialogs(app);
				app.KillGameSelector();
				app.PreNewGame(GameMode.SurvivalNormalStage1, theLookForSavedGame: false);
				break;
			case 9:
				app.mSeedChooserScreen?.mScrollWidget?.ScrollToBottom(animated: false);
				break;
			case 10:
				Say("chooser");
				break;
			case 11:
			{
				SeedChooserScreen ch = app.mSeedChooserScreen;
				ch?.SeedSelected(SeedType.Sunflower);
				ch?.SeedSelected(TavelCzlb.Seed);
				foreach (TavelSpec p in Tavel.ChooserPlants.Take(app.mBoard.mSeedBank.mNumPackets))
				{
					ch?.SeedSelected(p.Seed);
				}
				foreach (SeedType t in new[] { SeedType.Sunflower, SeedType.Peashooter, SeedType.Wallnut, SeedType.Cherrybomb, SeedType.Repeater, SeedType.Snowpea, SeedType.Squash, SeedType.Chomper, SeedType.Potatomine, SeedType.Threepeater })
				{
					ch?.SeedSelected(t);
				}
				break;
			}
			case 13:
				Say("picked");
				break;
			case 14:
				app.mSeedChooserScreen?.OnStartButton();
				break;
			case 22 when !Tavel.Enabled:
				for (int row = 0; row < 5; row++)
				{
					app.mBoard.AddZombieInRow(ZombieType.Normal, row, 0);
				}
				break;
			case 23 when !Tavel.Enabled:
				Say($"off ult={Tavel.UltimateUiAllowed(app.mBoard)} next={Tavel.CanShowNextWaveButton(app.mBoard)}");
				break;
			case 22:
			{
				Board b = app.mBoard;
				b.mSunMoney = 9990;
				string[] order = { "super_gatling", "fjth", "dmxj", "ssbps", "qgxlj", "nbjqs", "czlb", "hjbl", "swg", "tclc", "njspg", "dmdf" };
				for (int i = 0; i < order.Length; i++)
				{
					TavelSpec spec = Tavel.Plants.First((TavelSpec q) => q.Code == order[i]);
					int row = i / 3;
					int col = 1 + i % 3 * 2;
					if (order[i] == "czlb")
					{
						continue; // planted by tapping below
					}
					b.AddPlant(col, row, spec.Seed, SeedType.None);
				}
				for (int row = 0; row < 5; row++)
				{
					b.AddZombieInRow(ZombieType.Normal, row, 0);
					b.AddZombieInRow(row % 2 == 0 ? ZombieType.Pail : ZombieType.TrafficCone, row, 0);
				}
				break;
			}
			case 23:
			{
				// the player's way: tap the packet, then a cell
				Board b = app.mBoard;
				SeedPacket packet = b.mSeedBank.mSeedPackets.FirstOrDefault((SeedPacket q) => q != null && q.mPacketType == TavelCzlb.Seed);
				int sun = b.mSunMoney;
				packet?.MouseDown(5, 5, 1);
				Say($"cursor {b.mCursorObject.mCursorType} {b.mCursorObject.mType}");
				int px = b.GridToPixelX(1, 2) + 40;
				int py = b.GridToPixelY(1, 2) + 40;
				b.MouseUpWithPlant((int)(px * Constants.S), (int)(py * Constants.S), 1);
				Plant planted = b.GetTopPlantAt(1, 2, TopPlant.EatingOrder);
				Say($"tapped plant={(int)(planted?.mSeedType ?? SeedType.None)} sun {sun}->{b.mSunMoney} refreshing={packet?.mRefreshing}");
				break;
			}
			case 25:
				Lawn(app, "lawn");
				Say("planted");
				break;
			case 30:
				Say("fighting");
				break;
			case 33:
				Say("fighting2");
				break;
			case 35:
				foreach (Plant p in app.mBoard.mPlants.Where((Plant q) => !q.mDead).ToArray())
				{
					if (Tavel.CanUseUltimate(p))
					{
						Say($"ultimate {Tavel.Get(p.mSeedType).Code} {Tavel.StartUltimate(p)}");
					}
				}
				for (int row = 0; row < 5; row++)
				{
					app.mBoard.AddZombieInRow(ZombieType.Football, row, 0);
				}
				break;
			case 36:
				Say("ultimates");
				break;
			case 38:
				Say("ultimates2");
				break;
			case 41:
				Say("ultimates3");
				break;
			case 45:
				Lawn(app, "before-save");
				app.mBoard.TryToSaveGame();
				Say("saved");
				break;
			case 46:
				Say("load " + app.TryLoadGame());
				break;
			case 47:
				foreach (Dialog dlg in app.mDialogMap.Values.ToArray())
				{
					if (dlg is ContinueDialog cd)
					{
						cd.ButtonDepress(0);
					}
				}
				break;
			case 50:
				Lawn(app, "loaded");
				Say("reloaded");
				break;
			case 54:
				Say("end");
				break;
			}
		}
		catch (Exception ex)
		{
			Say("error " + ex);
		}
	}
}
#endif
