using System;
using Lawn.Creative;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Music
{
	private enum MusicBurstScheme
	{
		Mo3Day,
		Mo3Night
	}

	public static bool mUseMo3;

	public LawnApp mApp;

	public int mBaseBPM;

	public int mBaseModSpeed;

	public int mBurstOverride;

	public int mBurstStateCounter;

	public MusicTune mCurMusicFileDrums;

	public MusicTune mCurMusicFileHihats;

	public MusicTune mCurMusicFileMain;

	public MusicTune mCurMusicTune;

	public int mDrumsStateCounter;

	public int mFadeOutCounter;

	public int mFadeOutDuration;

	public MusicBurstState mMusicBurstState;

	public bool mMusicDisabled;

	public MusicDrumsState mMusicDrumsState;

	public MusicInterface mMusicInterface;

	public bool mPaused;

	public int mPauseOffset;

	public int mPauseOffsetDrums;

	public int mQueuedDrumTrackPackedOrder;

	public Music()
	{
		mApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		mMusicInterface = mApp.mMusicInterface;
		mPaused = false;
		mMusicDisabled = false;
		mCurMusicTune = MusicTune.None;
		mCurMusicFileMain = MusicTune.None;
		mCurMusicFileDrums = MusicTune.None;
		mCurMusicFileHihats = MusicTune.None;
		mBurstOverride = -1;
		mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_OFF;
		mQueuedDrumTrackPackedOrder = -1;
		mBaseBPM = 155;
		mBaseModSpeed = 3;
		mMusicBurstState = MusicBurstState.MUSIC_BURST_OFF;
		mPauseOffset = 0;
		mPauseOffsetDrums = 0;
		mFadeOutCounter = 0;
		mFadeOutDuration = 0;
	}

	public void MusicTitleScreenInit()
	{
		bool flag = LoadSong(MusicTune.MainMusic, "mainmusic.mo3");
		LoadSong(MusicTune.TitleCrazyDaveMainTheme, "crazydave");
		mUseMo3 = flag;
		MakeSureMusicIsPlaying(MusicTune.TitleCrazyDaveMainTheme);
	}

	public void MusicInit()
	{
		bool flag = mMusicInterface.HasMusic(15);
		if (!flag)
		{
			flag = LoadSong(MusicTune.MainMusic, "mainmusic.mo3");
		}
		flag = LoadSong(MusicTune.Drums, "mainmusic.mo3", flag);
		mApp.mCompletedLoadingThreadTasks += 7000;
		flag = LoadSong(MusicTune.Hihats, "mainmusic_hihats.mo3", flag);
		mApp.mCompletedLoadingThreadTasks += 7000;
		LoadSong(MusicTune.DayGrasswalk, "day", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.NightMoongrains, "night", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.PoolWaterygraves, "pool", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.FogRigormormist, "fog", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.RoofGrazetheroof, "roof", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.ChooseYourSeeds, "chooseyourseeds", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.FinalBossBrainiacManiac, "boss", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.PuzzleCerebrawl, "cerebrawl", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.MinigameLoonboon, "loonboon", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.Conveyer, "conveyor", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.ZenGarden, "zengarden", !flag);
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.FinalBoss2, "boss2");
		mApp.mCompletedLoadingThreadTasks += 3500;
		LoadSong(MusicTune.ZenGardenSH, "zengarden_sh");
		mApp.mCompletedLoadingThreadTasks += 3500;
		mUseMo3 = flag;
	}

	public void StopAllMusic()
	{
		mMusicInterface?.StopAllMusic();
		mCurMusicTune = MusicTune.None;
		mCurMusicFileMain = MusicTune.None;
		mCurMusicFileDrums = MusicTune.None;
		mCurMusicFileHihats = MusicTune.None;
		mQueuedDrumTrackPackedOrder = -1;
		mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_OFF;
		mMusicBurstState = MusicBurstState.MUSIC_BURST_OFF;
		mPauseOffset = 0;
		mPauseOffsetDrums = 0;
		mPaused = false;
		mFadeOutCounter = 0;
	}

	public void StartGameMusic()
	{
		Debug.ASSERT(mApp.mBoard != null);
		if (mApp.IsLevelUseJson())
		{
			CSLevelProperty component = mApp.mCreativeLevel.GetComponent<CSLevelProperty>();
			if (component.mMusicType.HasValue)
			{
				MakeSureMusicIsPlaying(component.mMusicType.GetValueOrDefault());
				return;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			MakeSureMusicIsPlaying(MusicTune.ZenGarden);
		}
		else if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
		{
			MakeSureMusicIsPlaying(MusicTune.FinalBoss2);
		}
		else if (mApp.IsFinalBossLevel())
		{
			MakeSureMusicIsPlaying(MusicTune.FinalBossBrainiacManiac);
		}
		else if (mApp.IsWallnutBowlingLevel() || mApp.IsWhackAZombieLevel() || mApp.IsLittleTroubleLevel() || mApp.IsBungeeBlitzLevel() || mApp.mGameMode == GameMode.ChallengeSpeed)
		{
			MakeSureMusicIsPlaying(MusicTune.MinigameLoonboon);
		}
		else if ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && (mApp.mPlayerInfo.mLevel == 10 || mApp.mPlayerInfo.mLevel == 20 || mApp.mPlayerInfo.mLevel == 30))
		{
			MakeSureMusicIsPlaying(MusicTune.Conveyer);
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn || mApp.mGameMode == GameMode.ChallengeColumn2 || mApp.mGameMode == GameMode.ChallengeColumn3 || mApp.mGameMode == GameMode.ChallengeColumn4)
		{
			MakeSureMusicIsPlaying(MusicTune.Conveyer);
		}
		else if (mApp.IsStormyNightLevel())
		{
			StopAllMusic();
		}
		else if (mApp.IsScaryPotterLevel() || mApp.IsIZombieLevel())
		{
			MakeSureMusicIsPlaying(MusicTune.PuzzleCerebrawl);
		}
		else if (mApp.mBoard.mBackground == BackgroundType.Num4Fog)
		{
			MakeSureMusicIsPlaying(MusicTune.FogRigormormist);
		}
		else if (mApp.mBoard.StageIsNight())
		{
			MakeSureMusicIsPlaying(MusicTune.NightMoongrains);
		}
		else if (mApp.mBoard.StageHasPool())
		{
			MakeSureMusicIsPlaying(MusicTune.PoolWaterygraves);
		}
		else if (mApp.mBoard.StageHasRoof())
		{
			MakeSureMusicIsPlaying(MusicTune.RoofGrazetheroof);
		}
		else
		{
			MakeSureMusicIsPlaying(MusicTune.DayGrasswalk);
		}
	}

	public void GameMusicPause(bool thePause)
	{
		GameMusicPause(thePause, theForcePause: false);
	}

	public void GameMusicPause(bool thePause, bool theForcePause)
	{
		if (mCurMusicTune == MusicTune.None)
		{
			mPaused = thePause;
			return;
		}
		if (thePause && (!mPaused | theForcePause))
		{
			PauseOrResumeCurrentFiles(pause: true);
		}
		else if (!thePause && (mPaused | theForcePause))
		{
			if (mCurMusicFileMain != MusicTune.None && !mMusicInterface.IsPlaying((int)mCurMusicFileMain))
			{
				PlayMusic(mCurMusicTune, -1, -1);
			}
			else
			{
				PauseOrResumeCurrentFiles(pause: false);
			}
		}
		mPaused = thePause;
	}

	public void MakeSureMusicIsPlaying(MusicTune theMusicTune)
	{
		if (mCurMusicTune != theMusicTune)
		{
			StopAllMusic();
			PlayMusic(theMusicTune, -1, -1);
		}
	}

	public void FadeOut(int theFadeOutDuration)
	{
		if (mCurMusicTune != MusicTune.None)
		{
			mFadeOutCounter = theFadeOutDuration;
			mFadeOutDuration = theFadeOutDuration;
		}
	}

	public int GetNumLoadingTasks()
	{
		return 59500;
	}

	private bool LoadSong(MusicTune theMusicTune, string theFileName, bool load = true)
	{
		if (!load)
		{
			return false;
		}
		if (!mMusicInterface.LoadMusic((int)theMusicTune, GlobalStaticVars.GetResourceDir() + "music/" + theFileName))
		{
			Console.WriteLine("Failed to load music file {0}", theFileName);
			return false;
		}
		return true;
	}

	private void MusicDispose()
	{
		StopAllMusic();
		mMusicInterface.UnloadAllMusic();
	}

	public static bool MusicIsInMo3(MusicTune theMusicTune)
	{
		if (!mUseMo3)
		{
			return false;
		}
		if (theMusicTune != MusicTune.DayGrasswalk && theMusicTune != MusicTune.NightMoongrains && theMusicTune != MusicTune.PoolWaterygraves && theMusicTune != MusicTune.FogRigormormist && theMusicTune != MusicTune.RoofGrazetheroof && theMusicTune != MusicTune.ChooseYourSeeds && theMusicTune != MusicTune.ZenGarden && theMusicTune != MusicTune.PuzzleCerebrawl && theMusicTune != MusicTune.MinigameLoonboon && theMusicTune != MusicTune.Conveyer)
		{
			return theMusicTune == MusicTune.FinalBossBrainiacManiac;
		}
		return true;
	}

	public static bool MusicTuneHasDrum(MusicTune theMusicTune)
	{
		return MusicIsInMo3(theMusicTune);
	}

	public void PlayMusic(MusicTune theMusicTune, int theOffset, int theDrumsOffset)
	{
		if (mMusicDisabled)
		{
			return;
		}
		mCurMusicTune = theMusicTune;
		mCurMusicFileMain = MusicTune.None;
		mCurMusicFileDrums = MusicTune.None;
		mCurMusicFileHihats = MusicTune.None;
		bool flag = theOffset != -1;
		if (MusicIsInMo3(theMusicTune))
		{
			switch (theMusicTune)
			{
			case MusicTune.DayGrasswalk:
				SetModuleFiles(drums: true, hihats: true);
				theOffset = DefaultOffset(theOffset, 0);
				PlayFromOffset(mCurMusicFileMain, theOffset, 1.0);
				PlayFromOffset(mCurMusicFileDrums, theOffset, 0.0);
				PlayFromOffset(mCurMusicFileHihats, theOffset, 0.0);
				break;
			case MusicTune.NightMoongrains:
				SetModuleFiles(drums: true, hihats: false);
				if (theOffset == -1)
				{
					theOffset = 48;
					theDrumsOffset = 92;
				}
				PlayFromOffset(mCurMusicFileMain, theOffset, 1.0);
				PlayFromOffset(mCurMusicFileDrums, theDrumsOffset, 0.0);
				break;
			case MusicTune.PoolWaterygraves:
				PlayThreeModuleLayers(DefaultOffset(theOffset, 94));
				break;
			case MusicTune.FogRigormormist:
				PlayThreeModuleLayers(DefaultOffset(theOffset, 125));
				break;
			case MusicTune.RoofGrazetheroof:
				PlayThreeModuleLayers(DefaultOffset(theOffset, 184));
				break;
			case MusicTune.ChooseYourSeeds:
				PlayMainModuleLayer(DefaultOffset(theOffset, 122));
				break;
			case MusicTune.TitleCrazyDaveMainTheme:
				PlayMainModuleLayer(DefaultOffset(theOffset, 152));
				break;
			case MusicTune.ZenGarden:
				PlayMainModuleLayer(DefaultOffset(theOffset, 221));
				break;
			case MusicTune.PuzzleCerebrawl:
				PlayMainModuleLayer(DefaultOffset(theOffset, 177));
				break;
			case MusicTune.MinigameLoonboon:
				PlayMainModuleLayer(DefaultOffset(theOffset, 166));
				break;
			case MusicTune.Conveyer:
				PlayMainModuleLayer(DefaultOffset(theOffset, 212));
				break;
			case MusicTune.FinalBossBrainiacManiac:
				PlayMainModuleLayer(DefaultOffset(theOffset, 158));
				break;
			}
			if (flag)
			{
				RestoreModuleTiming(mCurMusicFileMain);
				RestoreModuleTiming(mCurMusicFileDrums);
				RestoreModuleTiming(mCurMusicFileHihats);
				return;
			}
			int musicBPM = mMusicInterface.GetMusicBPM((int)mCurMusicFileMain);
			int musicSpeed = mMusicInterface.GetMusicSpeed((int)mCurMusicFileMain);
			if (musicBPM > 0)
			{
				mBaseBPM = musicBPM;
			}
			if (musicSpeed > 0)
			{
				mBaseModSpeed = musicSpeed;
			}
		}
		else
		{
			mCurMusicFileMain = mCurMusicTune;
			PlayFromOffset(mCurMusicFileMain, DefaultOffset(theOffset, 0), 1.0);
		}
	}

	public void PlayFromOffset(MusicTune theMusicTune, int theOffset, double theVolume)
	{
		if (MusicIsInMo3(mCurMusicTune) && mMusicInterface.HasMusic((int)theMusicTune) && !mMusicInterface.IsStream((int)theMusicTune))
		{
			SetupMusicFileForTune(theMusicTune, mCurMusicTune);
			mMusicInterface.PlayMusicFromOrder((int)theMusicTune, theOffset, theVolume, loop: true);
		}
		else
		{
			mMusicInterface.SetSongVolume((int)theMusicTune, theVolume);
			mMusicInterface.PlayMusic((int)theMusicTune, loop: true);
		}
	}

	public void SetupMusicFileForTune(MusicTune theMusicTune, MusicTune theCurMusicTune)
	{
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		int num5 = -1;
		switch (theCurMusicTune)
		{
		case MusicTune.DayGrasswalk:
			num = 29;
			switch (theMusicTune)
			{
			case MusicTune.MainMusic:
				num2 = 0;
				num3 = 23;
				break;
			case MusicTune.Hihats:
				num2 = 27;
				num3 = 27;
				break;
			case MusicTune.Drums:
				num2 = 24;
				num3 = 26;
				break;
			}
			break;
		case MusicTune.PoolWaterygraves:
			num = 29;
			switch (theMusicTune)
			{
			case MusicTune.MainMusic:
				num2 = 0;
				num3 = 17;
				break;
			case MusicTune.Hihats:
				num2 = 18;
				num3 = 24;
				num4 = 29;
				num5 = 29;
				break;
			case MusicTune.Drums:
				num2 = 18;
				num3 = 28;
				break;
			}
			break;
		case MusicTune.FogRigormormist:
			num = 29;
			switch (theMusicTune)
			{
			case MusicTune.MainMusic:
				num2 = 0;
				num3 = 15;
				break;
			case MusicTune.Hihats:
				num2 = 23;
				num3 = 23;
				break;
			case MusicTune.Drums:
				num2 = 16;
				num3 = 22;
				break;
			}
			break;
		case MusicTune.RoofGrazetheroof:
			num = 29;
			switch (theMusicTune)
			{
			case MusicTune.MainMusic:
				num2 = 0;
				num3 = 17;
				break;
			case MusicTune.Hihats:
				num2 = 21;
				num3 = 21;
				break;
			case MusicTune.Drums:
				num2 = 18;
				num3 = 20;
				break;
			}
			break;
		default:
			if (theMusicTune == MusicTune.MainMusic || theMusicTune == MusicTune.Drums)
			{
				num = 29;
				num2 = 0;
				num3 = 29;
			}
			break;
		}
		for (int i = 0; i <= num; i++)
		{
			bool flag = (i >= num2 && i <= num3) || (i >= num4 && i <= num5);
			mMusicInterface.SetMusicTrackVolume((int)theMusicTune, i, flag ? 1f : 0f);
		}
	}

	public void MusicResyncChannel(MusicTune theMusicFileToMatch, MusicTune theMusicFileToSync)
	{
		if (!MusicIsInMo3(mCurMusicTune))
		{
			return;
		}
		uint musicOrder = GetMusicOrder(theMusicFileToMatch);
		uint musicOrder2 = GetMusicOrder(theMusicFileToSync);
		if (musicOrder == uint.MaxValue || musicOrder2 == uint.MaxValue)
		{
			return;
		}
		int num = GetPackedMusicRow(musicOrder) - GetPackedMusicRow(musicOrder2);
		if (Math.Abs(num) <= 128)
		{
			int num2 = mBaseBPM;
			if (num > 2)
			{
				num2 += 2;
			}
			else if (num > 0)
			{
				num2++;
			}
			else if (num < -2)
			{
				num2 -= 2;
			}
			else if (num < 0)
			{
				num2--;
			}
			mMusicInterface.SetMusicBPM((int)theMusicFileToSync, num2);
		}
	}

	public void MusicResync()
	{
		if (mCurMusicFileMain != MusicTune.None)
		{
			if (mCurMusicFileDrums != MusicTune.None)
			{
				MusicResyncChannel(mCurMusicFileMain, mCurMusicFileDrums);
			}
			if (mCurMusicFileHihats != MusicTune.None)
			{
				MusicResyncChannel(mCurMusicFileMain, mCurMusicFileHihats);
			}
		}
	}

	public void StartBurst()
	{
		if (mMusicBurstState == MusicBurstState.MUSIC_BURST_OFF)
		{
			mMusicBurstState = MusicBurstState.MUSIC_BURST_STARTING;
			mBurstStateCounter = 400;
		}
	}

	public void MusicUpdate()
	{
		if (mFadeOutCounter > 0)
		{
			mFadeOutCounter--;
			if (mFadeOutCounter == 0)
			{
				StopAllMusic();
			}
			else if (mCurMusicFileMain != MusicTune.None)
			{
				float num = TodCommon.TodAnimateCurveFloat(mFadeOutDuration, 0, mFadeOutCounter, 1f, 0f, TodCurves.Linear);
				mMusicInterface.SetSongVolume((int)mCurMusicFileMain, num);
			}
		}
		if (mApp.mBoard != null && !mApp.mBoard.mPaused && MusicTuneHasDrum(mCurMusicTune))
		{
			UpdateMusicBurst();
			MusicResync();
		}
	}

	public void UpdateMusicBurst()
	{
		if (mApp.mBoard == null || !MusicTuneHasDrum(mCurMusicTune) || mApp.mGameMode == GameMode.Intro)
		{
			return;
		}
		MusicBurstScheme musicBurstScheme;
		if (mCurMusicTune == MusicTune.DayGrasswalk || mCurMusicTune == MusicTune.PoolWaterygraves || mCurMusicTune == MusicTune.FogRigormormist || mCurMusicTune == MusicTune.RoofGrazetheroof)
		{
			musicBurstScheme = MusicBurstScheme.Mo3Day;
		}
		else
		{
			if (mCurMusicTune != MusicTune.NightMoongrains)
			{
				return;
			}
			musicBurstScheme = MusicBurstScheme.Mo3Night;
		}
		uint musicOrder = GetMusicOrder(mCurMusicFileMain);
		if (musicOrder == uint.MaxValue)
		{
			return;
		}
		if (mBurstStateCounter > 0)
		{
			mBurstStateCounter--;
		}
		if (mDrumsStateCounter > 0)
		{
			mDrumsStateCounter--;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 1f;
		switch (mMusicBurstState)
		{
		case MusicBurstState.MUSIC_BURST_OFF:
			if (mApp.mBoard.CountZombiesOnScreen() >= 10 || mBurstOverride == 1)
			{
				StartBurst();
			}
			break;
		case MusicBurstState.MUSIC_BURST_STARTING:
			if (musicBurstScheme == MusicBurstScheme.Mo3Day)
			{
				num = TodCommon.TodAnimateCurveFloat(400, 0, mBurstStateCounter, 0f, 1f, TodCurves.Linear);
				if (mBurstStateCounter == 100)
				{
					mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_ON_QUEUED;
					mQueuedDrumTrackPackedOrder = (int)musicOrder;
				}
				else if (mBurstStateCounter == 0)
				{
					mMusicBurstState = MusicBurstState.MUSIC_BURST_ON;
					mBurstStateCounter = 800;
				}
			}
			else if (mMusicDrumsState == MusicDrumsState.MUSIC_DRUMS_OFF)
			{
				mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_ON_QUEUED;
				mQueuedDrumTrackPackedOrder = (int)musicOrder;
				mBurstStateCounter = 400;
			}
			else if (mMusicDrumsState == MusicDrumsState.MUSIC_DRUMS_ON_QUEUED)
			{
				mBurstStateCounter = 400;
			}
			else
			{
				num3 = TodCommon.TodAnimateCurveFloat(400, 0, mBurstStateCounter, 1f, 0f, TodCurves.Linear);
				if (mBurstStateCounter == 0)
				{
					mMusicBurstState = MusicBurstState.MUSIC_BURST_ON;
					mBurstStateCounter = 800;
				}
			}
			break;
		case MusicBurstState.MUSIC_BURST_ON:
			num = 1f;
			if (musicBurstScheme == MusicBurstScheme.Mo3Night)
			{
				num3 = 0f;
			}
			if (mBurstStateCounter == 0 && ((mApp.mBoard.CountZombiesOnScreen() < 4 && mBurstOverride == -1) || mBurstOverride == 2))
			{
				if (musicBurstScheme == MusicBurstScheme.Mo3Day)
				{
					mMusicBurstState = MusicBurstState.MUSIC_BURST_FINISHING;
					mBurstStateCounter = 800;
					mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_OFF_QUEUED;
					mQueuedDrumTrackPackedOrder = (int)musicOrder;
				}
				else
				{
					mMusicBurstState = MusicBurstState.MUSIC_BURST_FINISHING;
					mBurstStateCounter = 1100;
					mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_FADING;
					mDrumsStateCounter = 800;
				}
			}
			break;
		case MusicBurstState.MUSIC_BURST_FINISHING:
			if (musicBurstScheme == MusicBurstScheme.Mo3Day)
			{
				num = TodCommon.TodAnimateCurveFloat(800, 0, mBurstStateCounter, 1f, 0f, TodCurves.Linear);
			}
			else
			{
				num3 = TodCommon.TodAnimateCurveFloat(400, 0, mBurstStateCounter, 0f, 1f, TodCurves.Linear);
			}
			if (mBurstStateCounter == 0 && mMusicDrumsState == MusicDrumsState.MUSIC_DRUMS_OFF)
			{
				mMusicBurstState = MusicBurstState.MUSIC_BURST_OFF;
			}
			break;
		}
		int num4 = -1;
		int num5;
		int num6;
		if (musicBurstScheme == MusicBurstScheme.Mo3Day)
		{
			num5 = GetPackedMusicRow(musicOrder) / 128;
			num6 = ((mQueuedDrumTrackPackedOrder >= 0) ? (GetPackedMusicRow((uint)mQueuedDrumTrackPackedOrder) / 128) : (-1));
		}
		else
		{
			num5 = GetPackedMusicOrder(musicOrder);
			num6 = ((mQueuedDrumTrackPackedOrder >= 0) ? GetPackedMusicOrder((uint)mQueuedDrumTrackPackedOrder) : (-1));
			if (GetPackedMusicRow(musicOrder) > 252)
			{
				num5++;
			}
			if (mQueuedDrumTrackPackedOrder >= 0 && GetPackedMusicRow((uint)mQueuedDrumTrackPackedOrder) > 252)
			{
				num6++;
			}
		}
		switch (mMusicDrumsState)
		{
		case MusicDrumsState.MUSIC_DRUMS_ON_QUEUED:
			if (num5 != num6)
			{
				num2 = 1f;
				mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_ON;
				if (musicBurstScheme == MusicBurstScheme.Mo3Night)
				{
					num4 = ((num5 % 2 == 0) ? 76 : 77);
				}
			}
			break;
		case MusicDrumsState.MUSIC_DRUMS_ON:
			num2 = 1f;
			break;
		case MusicDrumsState.MUSIC_DRUMS_OFF_QUEUED:
			num2 = 1f;
			if (num5 != num6 && musicBurstScheme == MusicBurstScheme.Mo3Day)
			{
				mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_FADING;
				mDrumsStateCounter = 50;
			}
			break;
		case MusicDrumsState.MUSIC_DRUMS_FADING:
			num2 = TodCommon.TodAnimateCurveFloat((musicBurstScheme == MusicBurstScheme.Mo3Night) ? 800 : 50, 0, mDrumsStateCounter, 1f, 0f, TodCurves.Linear);
			if (mDrumsStateCounter == 0)
			{
				mMusicDrumsState = MusicDrumsState.MUSIC_DRUMS_OFF;
			}
			break;
		}
		if (musicBurstScheme == MusicBurstScheme.Mo3Day)
		{
			mMusicInterface.SetSongVolume((int)mCurMusicFileHihats, num);
			mMusicInterface.SetSongVolume((int)mCurMusicFileDrums, num2);
			return;
		}
		mMusicInterface.SetSongVolume((int)mCurMusicFileMain, num3);
		mMusicInterface.SetSongVolume((int)mCurMusicFileDrums, num2);
		if (num4 != -1)
		{
			mMusicInterface.SetMusicOrderPosition((int)mCurMusicFileDrums, num4);
		}
	}

	public uint GetMusicOrder(MusicTune theMusicFile)
	{
		return mMusicInterface.GetMusicOrder((int)theMusicFile);
	}

	private void PauseOrResumeCurrentFiles(bool pause)
	{
		if (MusicIsInMo3(mCurMusicTune))
		{
			PauseOrResumeFile(mCurMusicFileMain, pause);
			PauseOrResumeFile(mCurMusicFileDrums, pause);
			PauseOrResumeFile(mCurMusicFileHihats, pause);
		}
		else if (pause)
		{
			mMusicInterface.PauseMusic((int)mCurMusicTune);
		}
		else
		{
			mMusicInterface.ResumeMusic((int)mCurMusicTune);
		}
	}

	private void PauseOrResumeFile(MusicTune file, bool pause)
	{
		if (file != MusicTune.None)
		{
			if (pause)
			{
				mMusicInterface.PauseMusic((int)file);
			}
			else
			{
				mMusicInterface.ResumeMusic((int)file);
			}
		}
	}

	private void SetModuleFiles(bool drums, bool hihats)
	{
		mCurMusicFileMain = MusicTune.MainMusic;
		mCurMusicFileDrums = (drums ? MusicTune.Drums : MusicTune.None);
		mCurMusicFileHihats = (hihats ? MusicTune.Hihats : MusicTune.None);
	}

	private void PlayThreeModuleLayers(int offset)
	{
		SetModuleFiles(drums: true, hihats: true);
		PlayFromOffset(mCurMusicFileMain, offset, 1.0);
		PlayFromOffset(mCurMusicFileDrums, offset, 0.0);
		PlayFromOffset(mCurMusicFileHihats, offset, 0.0);
	}

	private void PlayMainModuleLayer(int offset)
	{
		SetModuleFiles(drums: false, hihats: false);
		PlayFromOffset(mCurMusicFileMain, offset, 1.0);
	}

	private void RestoreModuleTiming(MusicTune file)
	{
		if (file != MusicTune.None)
		{
			mMusicInterface.SetMusicBPM((int)file, mBaseBPM);
			mMusicInterface.SetMusicSpeed((int)file, mBaseModSpeed);
		}
	}

	private static int DefaultOffset(int offset, int defaultOffset)
	{
		if (offset != -1)
		{
			return offset;
		}
		return defaultOffset;
	}

	private static int GetPackedMusicOrder(uint thePackedOrder)
	{
		return (int)(thePackedOrder & 0xFFFF);
	}

	private static int GetPackedMusicRow(uint thePackedOrder)
	{
		return (int)(thePackedOrder >> 16);
	}
}
