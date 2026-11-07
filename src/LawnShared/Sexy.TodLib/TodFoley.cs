namespace Sexy.TodLib;

public class TodFoley
{
	public FoleyTypeData[] mFoleyTypeData = new FoleyTypeData[110];

	private int[] aVariationsArray = new int[10];

	public static FoleyParams[] gFoleyParamArray;

	public static int gFoleyParamArraySize;

	public TodFoley()
	{
		for (int i = 0; i < 110; i++)
		{
			mFoleyTypeData[i] = new FoleyTypeData();
		}
	}

	public void PlayFoley(FoleyType theFoleyType)
	{
		FoleyParams foleyParams = LookupFoley(theFoleyType);
		float aPitch = 0f;
		if (foleyParams.mPitchRange != 0f)
		{
			aPitch = TodCommon.RandRangeFloat(0f, foleyParams.mPitchRange);
		}
		PlayFoleyPitch(theFoleyType, aPitch);
	}

	public void StopFoley(FoleyType theFoleyType)
	{
		SoundSystemReleaseFinishedInstances(this);
		FoleyInstance foleyInstance = SoundSystemFindInstance(this, theFoleyType);
		if (foleyInstance != null)
		{
			foleyInstance.mRefCount--;
			if (foleyInstance.mRefCount == 0)
			{
				foleyInstance.mInstance.Release();
				foleyInstance.mInstance = null;
				foleyInstance.mPaused = false;
			}
		}
	}

	public bool IsFoleyPlaying(FoleyType theFoleyType)
	{
		SoundSystemReleaseFinishedInstances(this);
		return SoundSystemFindInstance(this, theFoleyType) != null;
	}

	public void GamePause(bool theEnteringPause)
	{
		SoundSystemReleaseFinishedInstances(this);
		for (int i = 0; i < gFoleyParamArraySize; i++)
		{
			FoleyParams foleyParams = LookupFoley((FoleyType)i);
			if (!TodCommon.TestBit(foleyParams.mFoleyFlags, 2))
			{
				continue;
			}
			FoleyTypeData foleyTypeData = mFoleyTypeData[i];
			for (int j = 0; j < 24; j++)
			{
				FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[j];
				if (foleyInstance.mRefCount != 0)
				{
					if (theEnteringPause)
					{
						foleyInstance.mPaused = true;
						foleyInstance.mInstance.Stop();
					}
					else if (foleyInstance.mPaused)
					{
						foleyInstance.mPaused = false;
						bool looping = TodCommon.TestBit(foleyParams.mFoleyFlags, 0);
						foleyInstance.mInstance.Play(looping);
					}
				}
			}
		}
	}

	public void PlayFoleyPitch(FoleyType theFoleyType, float aPitch)
	{
		FoleyParams foleyParams = LookupFoley(theFoleyType);
		SoundSystemReleaseFinishedInstances(this);
		if (SoundSystemHasFoleyPlayedTooRecently(this, theFoleyType) && !TodCommon.TestBit(foleyParams.mFoleyFlags, 0))
		{
			return;
		}
		if (TodCommon.TestBit(foleyParams.mFoleyFlags, 1))
		{
			FoleyInstance foleyInstance = SoundSystemFindInstance(this, theFoleyType);
			if (foleyInstance != null)
			{
				foleyInstance.mRefCount++;
				foleyInstance.mStartTime = GlobalStaticVars.gSexyAppBase.mUpdateCount;
				return;
			}
		}
		FoleyInstance foleyInstance2 = SoundSystemGetFreeInstanceIndex(this, theFoleyType);
		if (foleyInstance2 == null)
		{
			return;
		}
		FoleyTypeData foleyTypeData = mFoleyTypeData[(int)theFoleyType];
		int num = 0;
		for (int i = 0; i < foleyParams.mSfxID.Length; i++)
		{
			if (!TodCommon.TestBit(foleyParams.mFoleyFlags, 4) || foleyTypeData.mLastVariationPlayed != i)
			{
				int num2 = foleyParams.mSfxID[i];
				aVariationsArray[num] = i;
				num++;
			}
		}
		int num3 = (foleyTypeData.mLastVariationPlayed = TodCommon.TodPickFromArray(aVariationsArray, num));
		int theSfxID = foleyParams.mSfxID[num3];
		SoundInstance soundInstance = GlobalStaticVars.gSexyAppBase.mSoundManager.GetSoundInstance((uint)theSfxID);
		if (soundInstance != null)
		{
			foleyInstance2.mInstance = soundInstance as XNASoundInstance;
			foleyInstance2.mRefCount = 1;
			foleyInstance2.mStartTime = GlobalStaticVars.gSexyAppBase.mUpdateCount;
			foleyTypeData.mLastVariationPlayed = num3;
			if (aPitch != 0f)
			{
				soundInstance.AdjustPitch(aPitch / 10f);
			}
			if (TodCommon.TestBit(foleyParams.mFoleyFlags, 3))
			{
				ApplyMusicVolume(foleyInstance2);
			}
			bool looping = TodCommon.TestBit(foleyParams.mFoleyFlags, 0);
			soundInstance.Play(looping);
		}
	}

	public void CancelPausedFoley()
	{
		SoundSystemReleaseFinishedInstances(this);
		for (int i = 0; i < gFoleyParamArraySize; i++)
		{
			FoleyTypeData foleyTypeData = mFoleyTypeData[i];
			for (int j = 0; j < 24; j++)
			{
				FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[j];
				if (foleyInstance.mRefCount != 0 && foleyInstance.mPaused)
				{
					foleyInstance.mRefCount = 0;
					foleyInstance.mInstance.Release();
					foleyInstance.mInstance = null;
				}
			}
		}
	}

	public void ApplyMusicVolume(FoleyInstance theFoleyInstance)
	{
		if (GlobalStaticVars.gSexyAppBase.mSfxVolume < 9.999999974752427E-07)
		{
			theFoleyInstance.mInstance.SetVolume(0.0);
			return;
		}
		double volume = GlobalStaticVars.gSexyAppBase.mMusicVolume * GlobalStaticVars.gSexyAppBase.mSfxVolume;
		theFoleyInstance.mInstance.SetVolume(volume);
	}

	public void RehookupSoundWithMusicVolume()
	{
		SoundSystemReleaseFinishedInstances(this);
		for (int i = 0; i < gFoleyParamArraySize; i++)
		{
			FoleyParams foleyParams = LookupFoley((FoleyType)i);
			if (!TodCommon.TestBit(foleyParams.mFoleyFlags, 3))
			{
				continue;
			}
			FoleyTypeData foleyTypeData = mFoleyTypeData[i];
			for (int j = 0; j < 24; j++)
			{
				FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[j];
				if (foleyInstance.mRefCount != 0)
				{
					ApplyMusicVolume(foleyInstance);
				}
			}
		}
	}

	public static void TodFoleyInitialize(FoleyParams[] theFoleyParamArray, int theFoleyParamArraySize)
	{
		gFoleyParamArray = new FoleyParams[105]
		{
			new FoleyParams(FoleyType.Sun, 10f, new int[1] { Resources.SOUND_POINTS }, 0u),
			new FoleyParams(FoleyType.Splat, 10f, new int[3]
			{
				Resources.SOUND_SPLAT,
				Resources.SOUND_SPLAT2,
				Resources.SOUND_SPLAT3
			}, 0u),
			new FoleyParams(FoleyType.Lawnmower, 10f, new int[1] { Resources.SOUND_LAWNMOWER }, 0u),
			new FoleyParams(FoleyType.Throw, 10f, new int[4]
			{
				Resources.SOUND_THROW,
				Resources.SOUND_THROW,
				Resources.SOUND_THROW,
				Resources.SOUND_THROW2
			}, 0u),
			new FoleyParams(FoleyType.SpawnSun, 10f, new int[1] { Resources.SOUND_THROW }, 0u),
			new FoleyParams(FoleyType.Chomp, 0f, new int[2]
			{
				Resources.SOUND_CHOMP,
				Resources.SOUND_CHOMP2
			}, 0u),
			new FoleyParams(FoleyType.ChompSoft, 4f, new int[1] { Resources.SOUND_CHOMPSOFT }, 0u),
			new FoleyParams(FoleyType.Plant, 0f, new int[2]
			{
				Resources.SOUND_PLANT,
				Resources.SOUND_PLANT2
			}, 0u),
			new FoleyParams(FoleyType.UseShovel, 0f, new int[1] { Resources.SOUND_PLANT2 }, 0u),
			new FoleyParams(FoleyType.Drop, 0f, new int[1] { Resources.SOUND_TAP2 }, 0u),
			new FoleyParams(FoleyType.Bleep, 0f, new int[1] { Resources.SOUND_BLEEP }, 0u),
			new FoleyParams(FoleyType.Groan, 0f, new int[6]
			{
				Resources.SOUND_GROAN,
				Resources.SOUND_GROAN2,
				Resources.SOUND_GROAN3,
				Resources.SOUND_GROAN4,
				Resources.SOUND_GROAN5,
				Resources.SOUND_GROAN6
			}, 0u),
			new FoleyParams(FoleyType.Brains, 0f, new int[9]
			{
				Resources.SOUND_GROAN,
				Resources.SOUND_GROAN2,
				Resources.SOUND_GROAN3,
				Resources.SOUND_GROAN4,
				Resources.SOUND_GROAN5,
				Resources.SOUND_GROAN6,
				Resources.SOUND_SUKHBIR4,
				Resources.SOUND_SUKHBIR5,
				Resources.SOUND_SUKHBIR6
			}, 0u),
			new FoleyParams(FoleyType.Jackinthebox, 0f, new int[1] { Resources.SOUND_JACKINTHEBOX }, 7u),
			new FoleyParams(FoleyType.ArtChallenge, 0f, new int[1] { Resources.SOUND_DIAMOND }, 0u),
			new FoleyParams(FoleyType.Zamboni, 5f, new int[1] { Resources.SOUND_ZAMBONI }, 0u),
			new FoleyParams(FoleyType.Thunder, 10f, new int[1] { Resources.SOUND_THUNDER }, 0u),
			new FoleyParams(FoleyType.Frozen, 0f, new int[1] { Resources.SOUND_FROZEN }, 0u),
			new FoleyParams(FoleyType.Zombiesplash, 10f, new int[2]
			{
				Resources.SOUND_PLANT_WATER,
				Resources.SOUND_ZOMBIE_ENTERING_WATER
			}, 0u),
			new FoleyParams(FoleyType.Bowlingimpact, -3f, new int[1] { Resources.SOUND_BOWLINGIMPACT }, 0u),
			new FoleyParams(FoleyType.Squish, 0f, new int[2]
			{
				Resources.SOUND_CHOMP,
				Resources.SOUND_CHOMP2
			}, 0u),
			new FoleyParams(FoleyType.TirePop, 0f, new int[1] { Resources.SOUND_BALLOON_POP }, 0u),
			new FoleyParams(FoleyType.Explosion, 0f, new int[1] { Resources.SOUND_EXPLOSION }, 0u),
			new FoleyParams(FoleyType.Slurp, 2f, new int[1] { Resources.SOUND_SLURP }, 0u),
			new FoleyParams(FoleyType.LimbsPop, 10f, new int[1] { Resources.SOUND_LIMBS_POP }, 0u),
			new FoleyParams(FoleyType.PogoZombie, 4f, new int[1] { Resources.SOUND_POGO_ZOMBIE }, 0u),
			new FoleyParams(FoleyType.SnowPeaSparkles, 10f, new int[1] { Resources.SOUND_SNOW_PEA_SPARKLES }, 0u),
			new FoleyParams(FoleyType.ZombieFalling, 10f, new int[2]
			{
				Resources.SOUND_ZOMBIE_FALLING_1,
				Resources.SOUND_ZOMBIE_FALLING_2
			}, 0u),
			new FoleyParams(FoleyType.Puff, 10f, new int[1] { Resources.SOUND_PUFF }, 0u),
			new FoleyParams(FoleyType.Fume, 10f, new int[1] { Resources.SOUND_FUME }, 0u),
			new FoleyParams(FoleyType.Coin, 10f, new int[1] { Resources.SOUND_COIN }, 0u),
			new FoleyParams(FoleyType.KernelSplat, 10f, new int[2]
			{
				Resources.SOUND_KERNELPULT,
				Resources.SOUND_KERNELPULT2
			}, 0u),
			new FoleyParams(FoleyType.Digger, 0f, new int[1] { Resources.SOUND_DIGGER_ZOMBIE }, 7u),
			new FoleyParams(FoleyType.JackSurprise, 1f, new int[3]
			{
				Resources.SOUND_JACK_SURPRISE,
				Resources.SOUND_JACK_SURPRISE,
				Resources.SOUND_JACK_SURPRISE2
			}, 0u),
			new FoleyParams(FoleyType.VaseBreaking, -5f, new int[1] { Resources.SOUND_VASE_BREAKING }, 0u),
			new FoleyParams(FoleyType.PoolCleaner, 2f, new int[1] { Resources.SOUND_POOL_CLEANER }, 0u),
			new FoleyParams(FoleyType.Basketball, 10f, new int[1] { Resources.SOUND_BASKETBALL }, 0u),
			new FoleyParams(FoleyType.Ignite, 5f, new int[4]
			{
				Resources.SOUND_IGNITE,
				Resources.SOUND_IGNITE,
				Resources.SOUND_IGNITE,
				Resources.SOUND_IGNITE2
			}, 0u),
			new FoleyParams(FoleyType.Firepea, 10f, new int[1] { Resources.SOUND_FIREPEA }, 0u),
			new FoleyParams(FoleyType.Thump, 2f, new int[1] { Resources.SOUND_GARGANTUAR_THUMP }, 0u),
			new FoleyParams(FoleyType.SquashHmm, 2f, new int[3]
			{
				Resources.SOUND_SQUASH_HMM,
				Resources.SOUND_SQUASH_HMM,
				Resources.SOUND_SQUASH_HMM2
			}, 0u),
			new FoleyParams(FoleyType.Magnetshroom, 2f, new int[1] { Resources.SOUND_MAGNETSHROOM }, 0u),
			new FoleyParams(FoleyType.Butter, 2f, new int[1] { Resources.SOUND_BUTTER }, 0u),
			new FoleyParams(FoleyType.BungeeScream, 2f, new int[3]
			{
				Resources.SOUND_BUNGEE_SCREAM,
				Resources.SOUND_BUNGEE_SCREAM2,
				Resources.SOUND_BUNGEE_SCREAM3
			}, 0u),
			new FoleyParams(FoleyType.BossExplosionSmall, 2f, new int[1] { Resources.SOUND_EXPLOSION }, 0u),
			new FoleyParams(FoleyType.ShieldHit, 10f, new int[2]
			{
				Resources.SOUND_SHIELDHIT,
				Resources.SOUND_SHIELDHIT2
			}, 0u),
			new FoleyParams(FoleyType.Swing, 2f, new int[1] { Resources.SOUND_SWING }, 0u),
			new FoleyParams(FoleyType.Bonk, 2f, new int[1] { Resources.SOUND_BONK }, 0u),
			new FoleyParams(FoleyType.Rain, 0f, new int[1] { Resources.SOUND_RAIN }, 5u),
			new FoleyParams(FoleyType.DolphinBeforeJumping, 0f, new int[1] { Resources.SOUND_DOLPHIN_BEFORE_JUMPING }, 0u),
			new FoleyParams(FoleyType.DolphinAppears, 0f, new int[1] { Resources.SOUND_DOLPHIN_APPEARS }, 0u),
			new FoleyParams(FoleyType.PlantWater, 0f, new int[1] { Resources.SOUND_PLANT_WATER }, 0u),
			new FoleyParams(FoleyType.ZombieEnteringWater, 0f, new int[1] { Resources.SOUND_ZOMBIE_ENTERING_WATER }, 0u),
			new FoleyParams(FoleyType.Gravebusterchomp, 0f, new int[1] { Resources.SOUND_GRAVEBUSTERCHOMP }, 4u),
			new FoleyParams(FoleyType.Cherrybomb, 0f, new int[1] { Resources.SOUND_CHERRYBOMB }, 0u),
			new FoleyParams(FoleyType.JalapenoIgnite, 0f, new int[1] { Resources.SOUND_JALAPENO }, 0u),
			new FoleyParams(FoleyType.ReverseExplosion, 0f, new int[1] { Resources.SOUND_REVERSE_EXPLOSION }, 0u),
			new FoleyParams(FoleyType.PlasticHit, 5f, new int[2]
			{
				Resources.SOUND_PLASTICHIT,
				Resources.SOUND_PLASTICHIT2
			}, 0u),
			new FoleyParams(FoleyType.Winmusic, 0f, new int[1] { Resources.SOUND_WINMUSIC }, 8u),
			new FoleyParams(FoleyType.Ballooninflate, 10f, new int[1] { Resources.SOUND_BALLOONINFLATE }, 0u),
			new FoleyParams(FoleyType.Bigchomp, -2f, new int[1] { Resources.SOUND_BIGCHOMP }, 0u),
			new FoleyParams(FoleyType.Melonimpact, -5f, new int[2]
			{
				Resources.SOUND_MELONIMPACT,
				Resources.SOUND_MELONIMPACT2
			}, 0u),
			new FoleyParams(FoleyType.Plantgrow, -2f, new int[1] { Resources.SOUND_PLANTGROW }, 0u),
			new FoleyParams(FoleyType.Shoop, -5f, new int[1] { Resources.SOUND_SHOOP }, 0u),
			new FoleyParams(FoleyType.Juicy, 2f, new int[1] { Resources.SOUND_JUICY }, 0u),
			new FoleyParams(FoleyType.NewspaperRarrgh, -2f, new int[3]
			{
				Resources.SOUND_NEWSPAPER_RARRGH,
				Resources.SOUND_NEWSPAPER_RARRGH2,
				Resources.SOUND_NEWSPAPER_RARRGH2
			}, 0u),
			new FoleyParams(FoleyType.NewspaperRip, -2f, new int[1] { Resources.SOUND_NEWSPAPER_RIP }, 0u),
			new FoleyParams(FoleyType.Floop, 0f, new int[1] { Resources.SOUND_FLOOP }, 0u),
			new FoleyParams(FoleyType.Coffee, 0f, new int[1] { Resources.SOUND_COFFEE }, 0u),
			new FoleyParams(FoleyType.Lowgroan, 2f, new int[2]
			{
				Resources.SOUND_LOWGROAN,
				Resources.SOUND_LOWGROAN2
			}, 0u),
			new FoleyParams(FoleyType.Prize, 0f, new int[1] { Resources.SOUND_PRIZE }, 0u),
			new FoleyParams(FoleyType.Yuck, 1f, new int[3]
			{
				Resources.SOUND_YUCK,
				Resources.SOUND_YUCK,
				Resources.SOUND_YUCK2
			}, 0u),
			new FoleyParams(FoleyType.Umbrella, 2f, new int[1] { Resources.SOUND_THROW2 }, 0u),
			new FoleyParams(FoleyType.Grassstep, 2f, new int[1] { Resources.SOUND_GRASSSTEP }, 0u),
			new FoleyParams(FoleyType.Shovel, 5f, new int[1] { Resources.SOUND_SHOVEL }, 0u),
			new FoleyParams(FoleyType.Coblaunch, 10f, new int[1] { Resources.SOUND_COBLAUNCH }, 0u),
			new FoleyParams(FoleyType.Watering, 10f, new int[1] { Resources.SOUND_WATERING }, 0u),
			new FoleyParams(FoleyType.Polevault, 5f, new int[1] { Resources.SOUND_POLEVAULT }, 0u),
			new FoleyParams(FoleyType.GravestoneRumble, 10f, new int[1] { Resources.SOUND_GRAVESTONE_RUMBLE }, 0u),
			new FoleyParams(FoleyType.DirtRise, 5f, new int[1] { Resources.SOUND_DIRT_RISE }, 0u),
			new FoleyParams(FoleyType.Fertilizer, 0f, new int[1] { Resources.SOUND_FERTILIZER }, 0u),
			new FoleyParams(FoleyType.Portal, 0f, new int[1] { Resources.SOUND_PORTAL }, 0u),
			new FoleyParams(FoleyType.Wakeup, 0f, new int[1] { Resources.SOUND_WAKEUP }, 0u),
			new FoleyParams(FoleyType.Bugspray, 0f, new int[1] { Resources.SOUND_BUGSPRAY }, 0u),
			new FoleyParams(FoleyType.Scream, 0f, new int[1] { Resources.SOUND_SCREAM }, 0u),
			new FoleyParams(FoleyType.Paper, 0f, new int[1] { Resources.SOUND_PAPER }, 0u),
			new FoleyParams(FoleyType.Moneyfalls, 0f, new int[1] { Resources.SOUND_MONEYFALLS }, 0u),
			new FoleyParams(FoleyType.Imp, 5f, new int[2]
			{
				Resources.SOUND_IMP,
				Resources.SOUND_IMP2
			}, 0u),
			new FoleyParams(FoleyType.HydraulicShort, 3f, new int[1] { Resources.SOUND_HYDRAULIC_SHORT }, 0u),
			new FoleyParams(FoleyType.Hydraulic, 0f, new int[1] { Resources.SOUND_HYDRAULIC }, 0u),
			new FoleyParams(FoleyType.Gargantudeath, 3f, new int[1] { Resources.SOUND_GARGANTUDEATH }, 0u),
			new FoleyParams(FoleyType.Ceramic, 0f, new int[1] { Resources.SOUND_CERAMIC }, 0u),
			new FoleyParams(FoleyType.Bossboulderattack, 0f, new int[1] { Resources.SOUND_BOSSBOULDERATTACK }, 0u),
			new FoleyParams(FoleyType.Chime, 0f, new int[1] { Resources.SOUND_CHIME }, 0u),
			new FoleyParams(FoleyType.Crazydaveshort, 0f, new int[3]
			{
				Resources.SOUND_CRAZYDAVESHORT1,
				Resources.SOUND_CRAZYDAVESHORT2,
				Resources.SOUND_CRAZYDAVESHORT3
			}, 16u),
			new FoleyParams(FoleyType.Crazydavelong, 0f, new int[3]
			{
				Resources.SOUND_CRAZYDAVELONG1,
				Resources.SOUND_CRAZYDAVELONG2,
				Resources.SOUND_CRAZYDAVELONG3
			}, 16u),
			new FoleyParams(FoleyType.Crazydaveextralong, 0f, new int[3]
			{
				Resources.SOUND_CRAZYDAVEEXTRALONG1,
				Resources.SOUND_CRAZYDAVEEXTRALONG2,
				Resources.SOUND_CRAZYDAVEEXTRALONG3
			}, 16u),
			new FoleyParams(FoleyType.Crazydavecrazy, 0f, new int[1] { Resources.SOUND_CRAZYDAVECRAZY }, 0u),
			new FoleyParams(FoleyType.Phonograph, 0f, new int[1] { Resources.SOUND_PHONOGRAPH }, 0u),
			new FoleyParams(FoleyType.Dancer, 0f, new int[1] { Resources.SOUND_DANCER }, 6u),
			new FoleyParams(FoleyType.Finalfanfare, 0f, new int[1] { Resources.SOUND_FINALFANFARE }, 0u),
			new FoleyParams(FoleyType.Crazydavescream, 0f, new int[1] { Resources.SOUND_CRAZYDAVESCREAM }, 0u),
			new FoleyParams(FoleyType.Crazydavescream2, 0f, new int[1] { Resources.SOUND_CRAZYDAVESCREAM2 }, 0u),
			new FoleyParams(FoleyType.CattailSplat, 0f, new int[1] { Resources.SOUND_CATTAILSPLAT }, 0u),
			new FoleyParams(FoleyType.ZombieMonkSp, 0f, new int[1] { Resources.SOUND_ZOMBIE_MONK_SP }, 0u)
		};
		gFoleyParamArraySize = theFoleyParamArraySize;
	}

	public static void TodFoleyDispose()
	{
		gFoleyParamArray = null;
		gFoleyParamArraySize = 0;
	}

	public static FoleyParams LookupFoley(FoleyType theFoleyType)
	{
		return gFoleyParamArray[(int)theFoleyType];
	}

	public static void SoundSystemReleaseFinishedInstances(TodFoley theSoundSystem)
	{
		for (int i = 0; i < gFoleyParamArraySize; i++)
		{
			FoleyTypeData foleyTypeData = theSoundSystem.mFoleyTypeData[i];
			if (foleyTypeData == null)
			{
				continue;
			}
			for (int j = 0; j < 24; j++)
			{
				FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[j];
				if (foleyInstance.mRefCount != 0 && !foleyInstance.mPaused && !foleyInstance.mInstance.IsPlaying())
				{
					foleyInstance.mInstance.Release();
					foleyInstance.mInstance = null;
					foleyInstance.mRefCount = 0;
				}
			}
		}
	}

	public static FoleyInstance SoundSystemFindInstance(TodFoley theSoundSystem, FoleyType theFoleyType)
	{
		FoleyTypeData foleyTypeData = theSoundSystem.mFoleyTypeData[(int)theFoleyType];
		for (int i = 0; i < 24; i++)
		{
			FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[i];
			if (foleyInstance.mRefCount > 0)
			{
				return foleyInstance;
			}
		}
		return null;
	}

	public static bool SoundSystemHasFoleyPlayedTooRecently(TodFoley theSoundSystem, FoleyType theFoleyType)
	{
		FoleyTypeData foleyTypeData = theSoundSystem.mFoleyTypeData[(int)theFoleyType];
		if (foleyTypeData != null)
		{
			for (int i = 0; i < 24; i++)
			{
				FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[i];
				if (foleyInstance.mRefCount != 0)
				{
					int num = GlobalStaticVars.gSexyAppBase.mUpdateCount - foleyInstance.mStartTime;
					if (num < 2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static FoleyInstance SoundSystemGetFreeInstanceIndex(TodFoley theSoundSystem, FoleyType theFoleyType)
	{
		FoleyTypeData foleyTypeData = theSoundSystem.mFoleyTypeData[(int)theFoleyType];
		if (foleyTypeData != null)
		{
			for (int i = 0; i < 24; i++)
			{
				FoleyInstance foleyInstance = foleyTypeData.mFoleyInstances[i];
				if (foleyInstance.mRefCount == 0)
				{
					return foleyInstance;
				}
			}
		}
		return null;
	}
}
