using System;
using System.Collections.Generic;
using System.Linq;
using Lawn.Creative;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Challenge : StoreListener
{
	private const int MAX_SPAWNING_SEED_TYPES = 20;

	public static SeedType[,] gArtChallengeWallnut;

	public static SeedType[,] gArtChallengeSunFlower;

	public static SeedType[,] gArtChallengeStarfruit;

	public LawnApp mApp;

	public Board mBoard;

	public bool mBeghouledMouseCapture;

	public int mBeghouledMouseDownX;

	public int mBeghouledMouseDownY;

	public bool[,] mBeghouledEated = new bool[Constants.GRIDSIZEX, Constants.MAX_GRIDSIZEY];

	public bool[] mBeghouledPurcasedUpgrade = new bool[3];

	public int mBeghouledMatchesThisMove;

	public ChallengeState mChallengeState;

	public int mChallengeStateCounter;

	public int mConveyorBeltCounter;

	public int mChallengeScore;

	public bool mShowBowlingLine;

	public SeedType mLastConveyorSeedType;

	public int mSurvivalStage;

	public int mSlotMachineRollCount;

	public Reanimation mReanimChallenge;

	public int mReanimChallenge_Save;

	public Reanimation[] mReanimCloud = new Reanimation[6];

	public int[] mReanimCloud_Save = new int[6];

	public int[] mCloudCounter = new int[6];

	public int mChallengeGridX;

	public int mChallengeGridY;

	public int mScaryPotterPots;

	public int mRainCounter;

	public int mTreeOfWisdomTalkIndex;

	public string mName = "Put a name here";

	public int mGloveCounter;

	public int mRandomPortalCounter;

	private static int MAX_PICKS;

	private static GridItem[] aGridPickItemArray;

	private static TodWeightedArray[] aGridPicks;

	private static TodWeightedArray[] aSeedPickArray;

	private static TodWeightedArray[] aPotArray;

	private static TodWeightedGridArray[] aGridArray;

	private static TodWeightedGridArray[] aPicks;

	private string slotMachineMessage;

	private int slotMachineMessageCached = -1;

	private int[] aDoubleChance = new int[6] { 0, 30, 10, 10, 15, 18 };

	private int[] aTripleChance = new int[6] { 0, 0, 0, 0, 10, 13 };

	private int[] aPailChance = new int[6] { 0, 0, 0, 10, 15, 15 };

	private int[] aConeChance = new int[6] { 0, 0, 30, 30, 30, 30 };

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncBool(ref mBeghouledMouseCapture);
		theContext.SyncInt(ref mBeghouledMouseDownX);
		theContext.SyncInt(ref mBeghouledMouseDownY);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				theContext.SyncBool(ref mBeghouledEated[i, j]);
			}
		}
		for (int k = 0; k < 3; k++)
		{
			theContext.SyncBool(ref mBeghouledPurcasedUpgrade[k]);
		}
		theContext.SyncInt(ref mBeghouledMatchesThisMove);
		theContext.SyncEnum(ref mChallengeState, theContext.aChallengeStateSaver, SaveFileVersion.Initial, ChallengeState.Normal);
		theContext.SyncInt(ref mChallengeStateCounter);
		theContext.SyncInt(ref mConveyorBeltCounter);
		theContext.SyncInt(ref mChallengeScore);
		theContext.SyncBool(ref mShowBowlingLine);
		theContext.SyncEnum(ref mLastConveyorSeedType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncInt(ref mSurvivalStage);
		theContext.SyncInt(ref mSlotMachineRollCount);
		theContext.SyncInt(ref mReanimChallenge_Save);
		for (int l = 0; l < 6; l++)
		{
			theContext.SyncInt(ref mReanimCloud_Save[l]);
		}
		for (int m = 0; m < 6; m++)
		{
			theContext.SyncInt(ref mCloudCounter[m]);
		}
		theContext.SyncInt(ref mChallengeGridX);
		theContext.SyncInt(ref mChallengeGridY);
		theContext.SyncInt(ref mScaryPotterPots);
		theContext.SyncInt(ref mRainCounter);
		theContext.SyncInt(ref mTreeOfWisdomTalkIndex);
		theContext.SyncInt(ref mGloveCounter, SaveFileVersion.Board_GloveCounter);
	}

	static Challenge()
	{
		gArtChallengeWallnut = new SeedType[6, 9]
		{
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.Wallnut,
				SeedType.Wallnut,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Wallnut,
				SeedType.Wallnut,
				SeedType.Wallnut,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			}
		};
		gArtChallengeSunFlower = new SeedType[6, 9]
		{
			{
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.Starfruit,
				SeedType.Wallnut,
				SeedType.Wallnut,
				SeedType.Wallnut,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Umbrella,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.Umbrella,
				SeedType.Umbrella,
				SeedType.Umbrella,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			}
		};
		gArtChallengeStarfruit = new SeedType[6, 9]
		{
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None,
				SeedType.Starfruit,
				SeedType.None,
				SeedType.None
			},
			{
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None,
				SeedType.None
			}
		};
		MAX_PICKS = (Constants.GRIDSIZEX + 1) * Constants.MAX_GRIDSIZEY;
		aGridPickItemArray = new GridItem[MAX_PICKS];
		aGridPicks = new TodWeightedArray[MAX_PICKS];
		aSeedPickArray = new TodWeightedArray[200];
		aPotArray = new TodWeightedArray[54];
		aGridArray = new TodWeightedGridArray[54];
		aPicks = new TodWeightedGridArray[Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY];
		for (int i = 0; i < aGridPickItemArray.Length; i++)
		{
			aGridPickItemArray[i] = GridItem.GetNewGridItem();
		}
		for (int j = 0; j < aGridPicks.Length; j++)
		{
			aGridPicks[j] = TodWeightedArray.GetNewTodWeightedArray();
		}
		for (int k = 0; k < aSeedPickArray.Length; k++)
		{
			aSeedPickArray[k] = TodWeightedArray.GetNewTodWeightedArray();
		}
		for (int l = 0; l < aPotArray.Length; l++)
		{
			aPotArray[l] = TodWeightedArray.GetNewTodWeightedArray();
		}
		for (int m = 0; m < aGridArray.Length; m++)
		{
			aGridArray[m] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
		for (int n = 0; n < aPicks.Length; n++)
		{
			aPicks[n] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
	}

	public Challenge()
	{
		Init();
	}

	private void Init()
	{
		mApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		mBoard = mApp.mBoard;
		mBeghouledMouseCapture = false;
		mBeghouledMouseDownX = 0;
		mBeghouledMouseDownY = 0;
		mChallengeStateCounter = 0;
		mConveyorBeltCounter = 0;
		mChallengeScore = 0;
		mChallengeState = ChallengeState.Normal;
		mShowBowlingLine = false;
		mLastConveyorSeedType = SeedType.None;
		mSurvivalStage = 0;
		mSlotMachineRollCount = 0;
		mReanimChallenge = null;
		mChallengeGridX = 0;
		mChallengeGridY = 0;
		mScaryPotterPots = 0;
		mBeghouledMatchesThisMove = 0;
		mRainCounter = 0;
		mRandomPortalCounter = 0;
		mTreeOfWisdomTalkIndex = 0;
		mGloveCounter = 0;
		for (int i = 0; i < 6; i++)
		{
			mReanimCloud[i] = null;
		}
		for (int j = 0; j < Constants.GRIDSIZEX; j++)
		{
			for (int k = 0; k < Constants.MAX_GRIDSIZEY; k++)
			{
				mBeghouledEated[j, k] = false;
			}
		}
		for (int l = 0; l < 3; l++)
		{
			mBeghouledPurcasedUpgrade[l] = false;
		}
		if (mApp.IsSlotMachineLevel())
		{
			TRect tRect = SlotMachineGetHandleRect();
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.SlotMachineHandle, theIsPreloading: true);
			Reanimation reanimation = mApp.AddReanimation((float)tRect.mX - 243f, (float)tRect.mY + 50f, 0, ReanimationType.SlotMachineHandle);
			reanimation.mIsAttachment = true;
			reanimation.mAnimRate = 0f;
			mReanimChallenge = mApp.ReanimationGetID(reanimation);
		}
		if (mApp.IsLevelUseJson())
		{
			mName = CreativeLevelManager.GetLevelName(mApp.mGameMode);
			if (mName != null)
			{
				string newValue = " ";
				if (Constants.Language == Constants.LanguageIndex.zh_cn)
				{
					newValue = string.Empty;
				}
				mName = mName.Replace("|", newValue);
			}
			if (mName != null)
			{
				return;
			}
			if (mApp.IsDIYMode())
			{
				mName = string.Empty;
				return;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mName = TodStringFile.TodStringTranslate("[ZEN_GARDEN]");
		}
		else if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			mName = TodStringFile.TodStringTranslate("[TREE_OF_WISDOM]");
		}
		else if (mApp.mGameMode != GameMode.Intro && mApp.mGameMode != GameMode.Adventure)
		{
			mName = ChallengeScreen.GetChallengeDefFromMode(mApp.mGameMode).mChallengeName;
		}
	}

	public bool SaveToFile(Sexy.Buffer b)
	{
		b.WriteBoolean2DArray(mBeghouledEated);
		b.WriteLong(mBeghouledMatchesThisMove);
		b.WriteBoolean(mBeghouledMouseCapture);
		b.WriteLong(mBeghouledMouseDownX);
		b.WriteLong(mBeghouledMouseDownY);
		b.WriteBooleanArray(mBeghouledPurcasedUpgrade);
		b.WriteLong(mChallengeGridX);
		b.WriteLong(mChallengeGridY);
		b.WriteLong(mChallengeScore);
		b.WriteLong((int)mChallengeState);
		b.WriteLong(mChallengeStateCounter);
		b.WriteLongArray(mCloudCounter);
		b.WriteLong(mConveyorBeltCounter);
		b.WriteLong((int)mLastConveyorSeedType);
		b.WriteLong(mRainCounter);
		b.WriteLong(mScaryPotterPots);
		b.WriteBoolean(mShowBowlingLine);
		b.WriteLong(mSlotMachineRollCount);
		b.WriteLong(mSurvivalStage);
		b.WriteLong(mTreeOfWisdomTalkIndex);
		return true;
	}

	public bool LoadFromFile(Sexy.Buffer b)
	{
		mApp = GlobalStaticVars.gLawnApp;
		mBoard = mApp.mBoard;
		Init();
		mBeghouledEated = b.ReadBoolean2DArray();
		mBeghouledMatchesThisMove = b.ReadLong();
		mBeghouledMouseCapture = b.ReadBoolean();
		mBeghouledMouseDownX = b.ReadLong();
		mBeghouledMouseDownY = b.ReadLong();
		mBeghouledPurcasedUpgrade = b.ReadBooleanArray();
		mChallengeGridX = b.ReadLong();
		mChallengeGridY = b.ReadLong();
		mChallengeScore = b.ReadLong();
		mChallengeState = (ChallengeState)b.ReadLong();
		mChallengeStateCounter = b.ReadLong();
		mCloudCounter = b.ReadLongArray();
		mConveyorBeltCounter = b.ReadLong();
		mLastConveyorSeedType = SeedTypeLegacy.FromInt(b.ReadLong());
		mRainCounter = b.ReadLong();
		mScaryPotterPots = b.ReadLong();
		mShowBowlingLine = b.ReadBoolean();
		mSlotMachineRollCount = b.ReadLong();
		mSurvivalStage = b.ReadLong();
		mTreeOfWisdomTalkIndex = b.ReadLong();
		return true;
	}

	public void StartLevel()
	{
		if (mApp.IsWhackAZombieLevel())
		{
			mBoard.mCursorObject.mCursorType = CursorType.Hammer;
			if (!mApp.IsLevelUseJson())
			{
				mBoard.mZombieCountDown = 200;
				mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
			}
		}
		if (mApp.IsStormyNightLevel())
		{
			mChallengeState = ChallengeState.StormFlash1;
			mChallengeStateCounter = 400;
		}
		if (mApp.mGameMode == GameMode.ChallengeBobsledBonanza || mApp.mGameMode == GameMode.ChallengeBobsledBonanza2)
		{
			for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
			{
				if (mBoard.mPlantRow[i] != PlantRowType.Pool)
				{
					mBoard.mIceMinX[i] = 400 + Constants.BOARD_EXTRA_ROOM;
					mBoard.mIceTimer[i] = int.MaxValue;
				}
			}
		}
		if (mApp.IsWallnutBowlingLevel())
		{
			mBoard.mZombieCountDown = 200;
			mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
			if (!mApp.IsLevelUseJson())
			{
				mBoard.mSeedBank.AddSeed(SeedType.Wallnut);
			}
			mConveyorBeltCounter = 400;
		}
		if (mApp.IsWallnutBowlingLevel())
		{
			mShowBowlingLine = true;
		}
		if (mApp.mGameMode == GameMode.ChallengeShovel || mApp.mGameMode == GameMode.ChallengeSquirrel)
		{
			ShovelAddWallnuts();
		}
		if (mApp.IsScaryPotterLevel())
		{
			ScaryPotterStart();
		}
		if (mApp.IsLittleTroubleLevel() || mApp.IsStormyNightLevel() || mApp.IsBungeeBlitzLevel() || mApp.mGameMode == GameMode.ChallengeInvisighoul)
		{
			mBoard.mZombieCountDown = 200;
			mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
			mConveyorBeltCounter = 200;
		}
		if (mApp.IsSurvivalMode() && mSurvivalStage == 0)
		{
			string theAdvice = string.Empty;
			if (!mApp.IsLevelUseJson())
			{
				if (mApp.IsSurvivalNormal(mApp.mGameMode))
				{
					theAdvice = TodCommon.TodReplaceNumberString("[ADVICE_SURVIVE_FLAGS]", "{FLAGS}", 5);
				}
				else if (mApp.IsSurvivalHard(mApp.mGameMode))
				{
					theAdvice = TodCommon.TodReplaceNumberString("[ADVICE_SURVIVE_FLAGS]", "{FLAGS}", 10);
				}
				else
				{
					theAdvice = ((!mApp.IsSurvivalHell(mApp.mGameMode)) ? "[ADVICE_SURVIVE_ENDLESS]" : TodCommon.TodReplaceNumberString("[ADVICE_SURVIVE_FLAGS]", "{FLAGS}", 12));
				}
			}
			else
			{
				CSSurvivalLevel component = mApp.mCreativeLevel.GetComponent<CSSurvivalLevel>();
				if (component != null && component.mEndless)
				{
					theAdvice = "[ADVICE_SURVIVE_ENDLESS]";
				}
				else if (component != null)
				{
					CSLevelProperty component2 = mApp.mCreativeLevel.GetComponent<CSLevelProperty>();
					int num = component2.mNumWaves / Math.Max(component2.mWavesPerFlag, 1);
					theAdvice = TodCommon.TodReplaceNumberString("[ADVICE_SURVIVE_FLAGS]", "{FLAGS}", component.mStages * num);
				}
			}
			mBoard.DisplayAdvice(theAdvice, MessageStyle.HintFast, AdviceType.SurviveFlags);
		}
		if (mApp.mGameMode == GameMode.ChallengeLastStand && mSurvivalStage == 0)
		{
			string theAdvice2 = TodCommon.TodReplaceNumberString("[ADVICE_SURVIVE_FLAGS]", "{FLAGS}", 5);
			mBoard.DisplayAdvice(theAdvice2, MessageStyle.BigMiddleFast, AdviceType.SurviveFlags);
		}
		if (mApp.mGameMode == GameMode.ChallengeArtChallenge1)
		{
			mBoard.DisplayAdvice("[ADVICE_FILL_IN_WALLNUTS]", MessageStyle.HintFast, AdviceType.None);
		}
		if (mApp.mGameMode == GameMode.ChallengeArtChallenge2)
		{
			mBoard.DisplayAdvice("[ADVICE_FILL_IN_SPACES]", MessageStyle.HintFast, AdviceType.None);
		}
		if (mApp.mGameMode == GameMode.ChallengeSeeingStars)
		{
			mBoard.DisplayAdvice("[ADVICE_FILL_IN_STARFRUIT]", MessageStyle.HintFast, AdviceType.None);
		}
		if (mApp.IsSlotMachineLevel())
		{
			mBoard.SetTutorialState(TutorialState.SlotMachinePull);
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			mBoard.mZombieCountDown = 200;
			mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
			mChallengeStateCounter = 1500;
			BeghouledMakeStartBoard();
			BeghouledUpdateCraters();
			if (mApp.mGameMode == GameMode.ChallengeBeghouled)
			{
				mBoard.DisplayAdvice("[ADVICE_BEGHOULED_DRAG_TO_MATCH_3]", MessageStyle.HintFast, AdviceType.None);
			}
			else if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
			{
				mBoard.DisplayAdvice("[ADVICE_BEGHOULED_TWIST_TO_MATCH_3]", MessageStyle.HintFast, AdviceType.None);
			}
		}
		if (mApp.IsFirstTimeAdventureMode())
		{
			mApp.IsSquirrelLevel();
		}
		if (mApp.IsMiniBossLevel())
		{
			mBoard.mZombieCountDown = 100;
			mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
			mConveyorBeltCounter = 200;
		}
		if (!mApp.IsFinalBossLevel() && !mApp.IsWhackAZombieLevel())
		{
			GameMode mGameMode = mApp.mGameMode;
		}
		if (mApp.mGameMode == GameMode.ChallengePortalCombat)
		{
			PortalStart();
		}
		if (mSurvivalStage == 0)
		{
			CSSpawnPortal cSSpawnPortal = mApp.mCreativeLevel?.GetComponent<CSSpawnPortal>();
			if (cSSpawnPortal != null)
			{
				mRandomPortalCounter = cSSpawnPortal.mRandomPortalTime * 3 / 2;
				foreach (var mPortal in cSSpawnPortal.mPortals)
				{
					int item = mPortal.Item1;
					int item2 = mPortal.Item2;
					int item3 = mPortal.Item3;
					GridItem newGridItem = GridItem.GetNewGridItem();
					newGridItem.mGridItemType = ((item != 0) ? GridItemType.PortalSquare : GridItemType.PortalCircle);
					newGridItem.mGridX = item2;
					newGridItem.mGridY = item3;
					newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, newGridItem.mGridY, 0);
					newGridItem.OpenPortal();
					mBoard.mGridItems.Add(newGridItem);
				}
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeColumn || mApp.mGameMode == GameMode.ChallengeColumn2 || mApp.mGameMode == GameMode.ChallengeColumn4)
		{
			mBoard.mCurrentWave = 9;
			mBoard.mZombieCountDown = 2400;
		}
		if (mApp.mGameMode == GameMode.ChallengeColumn3)
		{
			mBoard.mCurrentWave = 9;
			mBoard.mZombieCountDown = 4000;
		}
		if (mApp.mGameMode == GameMode.ChallengeAirRaid || mApp.mGameMode == GameMode.ChallengeBobsledBonanza || mApp.mGameMode == GameMode.ChallengeBobsledBonanza2)
		{
			mBoard.mZombieCountDown = 4500;
		}
		if (mApp.mGameMode == GameMode.ChallengePogoParty)
		{
			mBoard.mZombieCountDown = 5500;
		}
		CSLevelProperty cSLevelProperty = mApp.mCreativeLevel?.GetComponent<CSLevelProperty>();
		if (cSLevelProperty != null)
		{
			mBoard.mCurrentWave = cSLevelProperty.mStartingWave;
			mBoard.mZombieCountDown = cSLevelProperty.mStartingTime;
			if (mBoard.mZombieCountDown <= 0)
			{
				mBoard.mZombieCountDown = 1;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mApp.mZenGarden.ZenGardenStart();
		}
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			mBoard.mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombiquariumSnorkel, SeedType.None);
			mBoard.mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiquariumTrophy, SeedType.None);
			mBoard.mSeedBank.mNumPackets = 2;
			mBoard.DisplayAdvice("[ADVICE_ZOMBIQUARIUM_CLICK_TO_FEED]", MessageStyle.HintTallFast, AdviceType.ZombiquariumClickToFeed);
			ZombiquariumSpawnSnorkel();
			ZombiquariumSpawnSnorkel();
		}
		if (mApp.IsIZombieLevel())
		{
			IZombieStart();
		}
		if (mApp.IsSquirrelLevel())
		{
			SquirrelStart();
		}
	}

	public void BeghouledPopulateBoard()
	{
		BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
		LoadBeghouledBoardState(newBeghouledBoardState);
		bool theAllowMatches = false;
		if (BeghouledBoardHasMatch(newBeghouledBoardState))
		{
			theAllowMatches = true;
		}
		BeghouledBoardState newBeghouledBoardState2 = BeghouledBoardState.GetNewBeghouledBoardState();
		for (int i = 0; i < 2; i++)
		{
			LoadBeghouledBoardState(newBeghouledBoardState2);
			BeghouledFillHoles(newBeghouledBoardState2, theAllowMatches);
			if (BeghouledCheckForPossibleMoves(newBeghouledBoardState2))
			{
				break;
			}
		}
		BeghouledCreatePlants(newBeghouledBoardState, newBeghouledBoardState2);
		newBeghouledBoardState2.PrepareForReuse();
		newBeghouledBoardState.PrepareForReuse();
	}

	public void LoadBeghouledBoardState(BeghouledBoardState theBoardState)
	{
		for (int i = 0; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j < Constants.MAX_GRIDSIZEY; j++)
			{
				theBoardState.mSeedType[i, j] = SeedType.None;
			}
		}
		int count = mBoard.mPlants.Count;
		for (int k = 0; k < count; k++)
		{
			Plant plant = mBoard.mPlants[k];
			if (!plant.mDead)
			{
				theBoardState.mSeedType[plant.mPlantCol, plant.mRow] = plant.mSeedType;
			}
		}
	}

	public SeedType BeghouledPickSeed(int theGridX, int theGridY, BeghouledBoardState theBoardState, bool theAllowMatches)
	{
		Debug.ASSERT(theBoardState.mSeedType[theGridX, theGridY] == SeedType.None);
		int[] array = new int[6];
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			SeedType seedType = SeedType.Peashooter;
			switch (i)
			{
			case 0:
				seedType = SeedType.Puffshroom;
				break;
			case 1:
				seedType = SeedType.Starfruit;
				break;
			case 2:
				seedType = SeedType.Magnetshroom;
				break;
			case 3:
				seedType = SeedType.Snowpea;
				break;
			case 4:
				seedType = SeedType.Wallnut;
				break;
			case 5:
				seedType = SeedType.Peashooter;
				break;
			default:
				Debug.ASSERT(value: false);
				break;
			}
			if (mBeghouledPurcasedUpgrade[0] && seedType == SeedType.Peashooter)
			{
				seedType = SeedType.Repeater;
			}
			else if (mBeghouledPurcasedUpgrade[1] && seedType == SeedType.Puffshroom)
			{
				seedType = SeedType.Fumeshroom;
			}
			else if (mBeghouledPurcasedUpgrade[2] && seedType == SeedType.Wallnut)
			{
				seedType = SeedType.Tallnut;
			}
			theBoardState.mSeedType[theGridX, theGridY] = seedType;
			if (theAllowMatches || !BeghouledBoardHasMatch(theBoardState))
			{
				array[num] = (int)seedType;
				num++;
			}
		}
		theBoardState.mSeedType[theGridX, theGridY] = SeedType.None;
		return (SeedType)TodCommon.TodPickFromArray(array, num);
	}

	public bool BeghouledBoardHasMatch(BeghouledBoardState theBoardState)
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (BeghouledHorizontalMatchLength(j, i, theBoardState) >= 3)
				{
					return true;
				}
				if (BeghouledVerticalMatchLength(j, i, theBoardState) >= 3)
				{
					return true;
				}
			}
		}
		return false;
	}

	public SeedType BeghouledGetPlantAt(int x, int y, BeghouledBoardState theBoardState)
	{
		if (x < 0 || x >= Constants.GRIDSIZEX || y < 0 || y >= Constants.MAX_GRIDSIZEY)
		{
			return SeedType.None;
		}
		return theBoardState.mSeedType[x, y];
	}

	public int BeghouledVerticalMatchLength(int x, int y, BeghouledBoardState theBoardState)
	{
		SeedType seedType = BeghouledGetPlantAt(x, y, theBoardState);
		if (seedType == SeedType.None)
		{
			return 0;
		}
		if (BeghouledGetPlantAt(x, y - 1, theBoardState) == seedType)
		{
			return 0;
		}
		int i;
		for (i = 1; BeghouledGetPlantAt(x, y + i, theBoardState) == seedType; i++)
		{
		}
		return i;
	}

	public int BeghouledHorizontalMatchLength(int x, int y, BeghouledBoardState theBoardState)
	{
		SeedType seedType = BeghouledGetPlantAt(x, y, theBoardState);
		if (seedType == SeedType.None)
		{
			return 0;
		}
		if (BeghouledGetPlantAt(x - 1, y, theBoardState) == seedType)
		{
			return 0;
		}
		int i;
		for (i = 1; BeghouledGetPlantAt(x + i, y, theBoardState) == seedType; i++)
		{
		}
		return i;
	}

	public void BeghouledDragStart(int x, int y)
	{
		if (!mBoard.HasLevelAwardDropped())
		{
			mBeghouledMouseCapture = true;
			mBeghouledMouseDownX = (int)((float)x * Constants.IS);
			mBeghouledMouseDownY = (int)((float)y * Constants.IS);
		}
	}

	public void BeghouledDragUpdate(int x, int y)
	{
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		int num = x - mBeghouledMouseDownX;
		int num2 = y - mBeghouledMouseDownY;
		if (Math.Abs(num) < 10 && Math.Abs(num2) < 10)
		{
			return;
		}
		mBoard.ClearAdvice(AdviceType.BeghouledDragToMatch3);
		mBeghouledMouseCapture = false;
		int num3 = mBoard.PixelToGridX(mBeghouledMouseDownX, mBeghouledMouseDownY);
		int num4 = mBoard.PixelToGridY(mBeghouledMouseDownX, mBeghouledMouseDownY);
		int num5;
		int num6;
		if (Math.Abs(num) > Math.Abs(num2))
		{
			if (num > 0)
			{
				num5 = num3 + 1;
				num6 = num4;
			}
			else
			{
				num5 = num3 - 1;
				num6 = num4;
			}
		}
		else if (num2 > 0)
		{
			num5 = num3;
			num6 = num4 + 1;
		}
		else
		{
			num5 = num3;
			num6 = num4 - 1;
		}
		BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
		LoadBeghouledBoardState(newBeghouledBoardState);
		Plant topPlantAt = mBoard.GetTopPlantAt(num3, num4, TopPlant.Any);
		if (!BeghouledIsValidMove(num3, num4, num5, num6, newBeghouledBoardState))
		{
			BeghouledDragCancel();
			if (topPlantAt != null)
			{
				if (num5 > num3)
				{
					topPlantAt.mX += 30;
				}
				else if (num5 < num3)
				{
					topPlantAt.mX -= 30;
				}
				else if (num6 > num4)
				{
					topPlantAt.mY += 30;
				}
				else if (num6 < num4)
				{
					topPlantAt.mY -= 30;
				}
				mApp.PlayFoley(FoleyType.Floop);
			}
			newBeghouledBoardState.PrepareForReuse();
		}
		else
		{
			Plant topPlantAt2 = mBoard.GetTopPlantAt(num5, num6, TopPlant.Any);
			if (topPlantAt != null)
			{
				topPlantAt.mPlantCol = num5;
				topPlantAt.mRow = num6;
				topPlantAt.mRenderOrder = topPlantAt.CalcRenderOrder();
			}
			if (topPlantAt2 != null)
			{
				topPlantAt2.mPlantCol = num3;
				topPlantAt2.mRow = num4;
				topPlantAt2.mRenderOrder = topPlantAt2.CalcRenderOrder();
			}
			BeghouledStartFalling(ChallengeState.BeghouledMoving);
			newBeghouledBoardState.PrepareForReuse();
		}
	}

	public void BeghouledDragCancel()
	{
		mBeghouledMouseCapture = false;
	}

	public bool MouseMove(int x, int y)
	{
		if (mApp.mGameMode == GameMode.ChallengeBeghouled && !mBoard.HasLevelAwardDropped())
		{
			if (mBeghouledMouseCapture)
			{
				BeghouledDragUpdate(x, y);
				return true;
			}
			mBoard.MouseHitTest(x, y, out var theHitResult, posScaled: false);
			if (theHitResult.mObjectType == GameObjectType.Plant)
			{
				return true;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			if (mChallengeState == ChallengeState.ZenFading)
			{
				mChallengeState = ChallengeState.Normal;
			}
			mChallengeStateCounter = 3000;
		}
		return false;
	}

	public bool MouseDown(int x, int y, int theClickCount, HitResult theHitResult)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			return mApp.mZenGarden.MouseDownZenGarden(x, y, theClickCount, theHitResult);
		}
		if (mApp.mGameScene != GameScenes.Playing)
		{
			return false;
		}
		if (mBoard.IsScaryPotterDaveTalking() && mApp.mCrazyDaveMessageIndex != -1)
		{
			AdvanceCrazyDaveDialog();
			return true;
		}
		if (theHitResult.mObjectType == GameObjectType.Coin && theClickCount >= 0)
		{
			return false;
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouled)
		{
			if (mChallengeState != ChallengeState.Normal)
			{
				return false;
			}
			if (theHitResult.mObjectType == GameObjectType.Plant)
			{
				BeghouledDragStart(x, y);
				return true;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			if (mChallengeState != ChallengeState.Normal)
			{
				return false;
			}
			BeghouledTwistMouseDown(x, y);
		}
		if (mApp.IsSlotMachineLevel() && theHitResult.mObjectType == GameObjectType.SlotMachineHandle && mBoard.mCursorObject.mCursorType == CursorType.Normal && mChallengeState == ChallengeState.Normal)
		{
			if (mBoard.TakeSunMoney(25))
			{
				for (int i = 0; i < 3; i++)
				{
					mBoard.mSeedBank.mSeedPackets[i].SlotMachineStart();
				}
				Reanimation reanimation = mApp.ReanimationGet(mReanimChallenge);
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_pull, ReanimLoopType.PlayOnceAndHold, 0, 36f);
				mChallengeState = ChallengeState.SlotMachineRolling;
				mBoard.SetTutorialState(TutorialState.SlotMachineCompleted);
				mBoard.ClearAdvice(AdviceType.None);
				mSlotMachineRollCount++;
				mApp.PlaySample(Resources.SOUND_SLOT_MACHINE);
			}
			return true;
		}
		if (mApp.IsWhackAZombieLevel() && theHitResult.mObjectType == GameObjectType.None && mBoard.mCursorObject.mCursorType == CursorType.Hammer && theClickCount >= 0)
		{
			MouseDownWhackAZombie(x, y);
			return true;
		}
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium && theClickCount <= 0)
		{
			mApp.PlaySample(Resources.SOUND_TAPGLASS);
		}
		if (mApp.IsScaryPotterLevel() && theHitResult.mObjectType == GameObjectType.ScaryPot)
		{
			ScaryPotterMalletPot((GridItem)theHitResult.mObject);
			return true;
		}
		return false;
	}

	public bool MouseUp(int x, int y)
	{
		if (mApp.mGameMode == GameMode.ChallengeBeghouled)
		{
			if (mBeghouledMouseCapture && !mBoard.mAdvice.IsBeingDisplayed() && mChallengeScore == 0)
			{
				mBoard.DisplayAdvice("[ADVICE_BEGHOULED_DRAG_TO_MATCH_3]", MessageStyle.HintFast, AdviceType.BeghouledDragToMatch3);
			}
			BeghouledDragCancel();
		}
		return false;
	}

	public void ClearCursor()
	{
		if (mApp.mGameMode == GameMode.ChallengeBeghouled)
		{
			BeghouledDragCancel();
		}
		if (mApp.IsWhackAZombieLevel() && !mBoard.HasLevelAwardDropped())
		{
			mBoard.mCursorObject.mCursorType = CursorType.Hammer;
		}
	}

	public void BeghouledRemoveHorizontalMatch(int x, int y, BeghouledBoardState theBoardState)
	{
		SeedType seedType = BeghouledGetPlantAt(x, y, theBoardState);
		do
		{
			mBoard.GetTopPlantAt(x, y, TopPlant.Any)?.Die();
			x++;
		}
		while (BeghouledGetPlantAt(x, y, theBoardState) == seedType);
	}

	public void BeghouledRemoveVerticalMatch(int x, int y, BeghouledBoardState theBoardState)
	{
		SeedType seedType = BeghouledGetPlantAt(x, y, theBoardState);
		do
		{
			mBoard.GetTopPlantAt(x, y, TopPlant.Any)?.Die();
			y++;
		}
		while (BeghouledGetPlantAt(x, y, theBoardState) == seedType);
	}

	public void BeghouledRemoveMatches(BeghouledBoardState theBoardState)
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int num = BeghouledHorizontalMatchLength(j, i, theBoardState);
				if (num >= 3)
				{
					BeghouledRemoveHorizontalMatch(j, i, theBoardState);
					BeghouledScore(j, i, num, theIsHorizontal: true);
				}
				int num2 = BeghouledVerticalMatchLength(j, i, theBoardState);
				if (num2 >= 3)
				{
					BeghouledRemoveVerticalMatch(j, i, theBoardState);
					BeghouledScore(j, i, num2, theIsHorizontal: false);
				}
			}
		}
	}

	public void Update()
	{
		if (ZBCheatDialog.AdvancedPause)
		{
			return;
		}
		Update_Orig();
	}

	private void Update_Orig()
	{
		if (mBoard.HasStorm())
		{
			UpdateStormyNight();
		}
		if (mBoard.mPaused)
		{
			if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
			{
				mChallengeGridX = -1;
				mChallengeGridY = -1;
			}
			return;
		}
		if (mBoard.HasRain())
		{
			UpdateRain();
		}
		if (mApp.mGameScene != GameScenes.Playing)
		{
			return;
		}
		if (mBoard.HasConveyorBeltSeedBank())
		{
			UpdateConveyorBelt();
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			UpdateBeghouled();
		}
		if (mApp.IsScaryPotterLevel())
		{
			ScaryPotterUpdate();
		}
		if ((mApp.IsScaryPotterLevel() || mApp.IsWhackAZombieLevel()) && mBoard.mSeedBank.mX < 0)
		{
			int num = mBoard.mSunMoney + mBoard.CountSunBeingCollected();
			if (num > 0 || mBoard.mSeedBank.mX > -mBoard.mSeedBank.mWidth)
			{
				mBoard.mSeedBank.mX += 2;
				if (mBoard.mSeedBank.mX > 0)
				{
					mBoard.mSeedBank.mX = 0;
				}
			}
		}
		if (mApp.IsWhackAZombieLevel())
		{
			WhackAZombieUpdate();
		}
		if (mApp.IsIZombieLevel())
		{
			IZombieUpdate();
		}
		if (mApp.IsSlotMachineLevel())
		{
			UpdateSlotMachine();
		}
		if (mApp.mGameMode == GameMode.ChallengeSpeed)
		{
			mBoard.UpdateGame();
		}
		CSSpeed cSSpeed = mApp.mCreativeLevel?.GetComponent<CSSpeed>();
		if (cSSpeed != null)
		{
			for (int i = 0; i < cSSpeed.mExtraUpdateTimes; i++)
			{
				mBoard.UpdateGame();
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeRainingSeeds || mApp.mGameMode == GameMode.ChallengeFinalBoss2 || mApp.mCreativeLevel?.GetComponent<CSRainingSeeds>() != null)
		{
			UpdateRainingSeeds();
		}
		if (mApp.mGameMode == GameMode.ChallengePortalCombat)
		{
			UpdatePortalCombat();
		}
		else if (mApp.mCreativeLevel?.GetComponent<CSSpawnPortal>() != null)
		{
			for (int j = 0; j < mBoard.mGridItems.Count; j++)
			{
				GridItem gridItem = mBoard.mGridItems[j];
				if (!gridItem.mDead && gridItem.mGridItemState != GridItemState.PortalClosed && (gridItem.mGridItemType == GridItemType.PortalCircle || gridItem.mGridItemType == GridItemType.PortalSquare))
				{
					UpdatePortal(gridItem);
				}
			}
			CSSpawnPortal component = mApp.mCreativeLevel.GetComponent<CSSpawnPortal>();
			if (component.mRandomPortalTime >= 0)
			{
				if (mBoard.HasLevelAwardDropped())
				{
					mBoard.ClearAdvice(AdviceType.PortalRelocating);
					return;
				}
				mRandomPortalCounter--;
				if (mRandomPortalCounter == 500)
				{
					mBoard.DisplayAdviceAgain("[ADVICE_PORTAL_RELOCATING]", MessageStyle.HintFast, AdviceType.PortalRelocating);
				}
				if (mRandomPortalCounter <= 0)
				{
					mBoard.ClearAdvice(AdviceType.PortalRelocating);
					mRandomPortalCounter = component.mRandomPortalTime;
					MoveAPortal();
				}
			}
		}
		if (mApp.IsSquirrelLevel())
		{
			SquirrelUpdate();
		}
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			ZombiquariumUpdate();
		}
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			TreeOfWisdomUpdate();
		}
		if (mApp.mGameMode == GameMode.ChallengeIce && mBoard.mMainCounter == 3000)
		{
			mApp.PlayFoley(FoleyType.Floop);
			mApp.PlaySample(Resources.SOUND_LOSEMUSIC);
		}
		if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			LastStandUpate();
		}
		if (mBoard.HasGlove() && mGloveCounter > 0)
		{
			mGloveCounter--;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mReanimChallenge);
		if (reanimation != null && reanimation.mIsAttachment)
		{
			reanimation.Update();
		}
	}

	public void UpdateBeghouled()
	{
		mBoard.mProgressMeterWidth = TodCommon.TodAnimateCurve(0, 75, mChallengeScore, 0, 150, TodCurves.Linear);
		bool flag = false;
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && UpdateBeghouledPlant(plant))
			{
				flag = true;
			}
		}
		if (mBoard.mSeedBank.mNumPackets > 4 && !mBoard.mAdvice.IsBeingDisplayed() && !mBoard.mHelpDisplayed[22])
		{
			int currentPlantCost = mBoard.GetCurrentPlantCost(SeedType.BeghouledButtonCrater, SeedType.None);
			if (mBoard.CanTakeSunMoney(currentPlantCost) && BeghouledCanClearCrater() && !mBoard.HasLevelAwardDropped())
			{
				mBoard.DisplayAdvice("[ADVICE_BEGHOULED_USE_CRATER_2]", MessageStyle.HintFast, AdviceType.BeghouledUseCrater2);
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist && mChallengeState == ChallengeState.Normal)
		{
			if (BeghouledTwistSquareFromMouse(mApp.mWidgetManager.mLastMouseX, mApp.mWidgetManager.mLastMouseY, ref mChallengeGridX, ref mChallengeGridY))
			{
				BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
				LoadBeghouledBoardState(newBeghouledBoardState);
				if (!BeghouledTwistValidMove(mChallengeGridX, mChallengeGridY, newBeghouledBoardState))
				{
					mChallengeGridX = -1;
					mChallengeGridY = -1;
				}
				newBeghouledBoardState.PrepareForReuse();
			}
		}
		else
		{
			mChallengeGridX = -1;
			mChallengeGridY = -1;
		}
		if (!flag && (mChallengeState == ChallengeState.BeghouledFalling || mChallengeState == ChallengeState.BeghouledMoving))
		{
			mChallengeState = ChallengeState.Normal;
			mChallengeStateCounter = 1500;
			BeghouledBoardState newBeghouledBoardState2 = BeghouledBoardState.GetNewBeghouledBoardState();
			LoadBeghouledBoardState(newBeghouledBoardState2);
			BeghouledRemoveMatches(newBeghouledBoardState2);
			LoadBeghouledBoardState(newBeghouledBoardState2);
			BeghouledMakePlantsFall(newBeghouledBoardState2);
			BeghouledPopulateBoard();
			if (mChallengeState == ChallengeState.BeghouledFalling)
			{
				newBeghouledBoardState2.PrepareForReuse();
				return;
			}
			mChallengeStateCounter = 1500;
			mBeghouledMatchesThisMove = 0;
			BeghouledCheckStuckState();
			newBeghouledBoardState2.PrepareForReuse();
		}
		if (mChallengeStateCounter == 0)
		{
			return;
		}
		mChallengeStateCounter--;
		if (mChallengeStateCounter <= 0 && !mBoard.HasLevelAwardDropped())
		{
			if (mChallengeState == ChallengeState.Normal)
			{
				BeghouledFlashAMatch();
				mChallengeStateCounter = 1500;
			}
			else if (mChallengeState == ChallengeState.BeghouledNoMatches)
			{
				mApp.AddTodParticle(400f, 300f, 400000, ParticleEffect.ScreenFlash);
				BeghouledShuffle();
			}
		}
	}

	public bool UpdateBeghouledPlant(Plant thePlant)
	{
		bool flag = false;
		int num = mBoard.GridToPixelX(thePlant.mPlantCol, thePlant.mRow) - thePlant.mX;
		int num2 = mBoard.GridToPixelY(thePlant.mPlantCol, thePlant.mRow) - thePlant.mY;
		int val = ((mChallengeState != ChallengeState.BeghouledMoving) ? TodCommon.TodAnimateCurve(90, 30, mChallengeStateCounter, 1, 20, TodCurves.EaseIn) : 3);
		int num3 = 0;
		int num4 = 0;
		if (num > 0)
		{
			num3 = Math.Min(val, num);
			thePlant.mX += num3;
			flag = true;
		}
		else if (num < 0)
		{
			num3 = -Math.Min(val, -num);
			thePlant.mX += num3;
			flag = true;
		}
		if (num2 > 0)
		{
			num4 = Math.Min(val, num2);
			thePlant.mY += num4;
			flag = true;
		}
		else if (num2 < 0)
		{
			num4 = -Math.Min(val, -num2);
			thePlant.mY += num4;
			flag = true;
		}
		if (flag && (thePlant.mState == PlantState.MagnetshroomCharging || thePlant.mState == PlantState.MagnetshroomSucking))
		{
			MagnetItem magnetItem = thePlant.mMagnetItems[0];
			magnetItem.mPosX += num3;
			magnetItem.mPosY += num4;
		}
		return flag;
	}

	public void BeghouledFallIntoSquare(int x, int y, BeghouledBoardState theBoardState)
	{
		if (mBeghouledEated[x, y])
		{
			return;
		}
		for (int num = y - 1; num >= 0; num--)
		{
			Plant topPlantAt = mBoard.GetTopPlantAt(x, num, TopPlant.Any);
			if (topPlantAt != null)
			{
				topPlantAt.mRow = y;
				topPlantAt.mRenderOrder = topPlantAt.CalcRenderOrder();
				theBoardState.mSeedType[x, y] = topPlantAt.mSeedType;
				theBoardState.mSeedType[x, num] = SeedType.None;
				BeghouledStartFalling(ChallengeState.BeghouledFalling);
				break;
			}
		}
	}

	public void BeghouledMakePlantsFall(BeghouledBoardState theBoardState)
	{
		for (int num = 4; num >= 0; num--)
		{
			for (int i = 0; i < 8; i++)
			{
				SeedType seedType = BeghouledGetPlantAt(i, num, theBoardState);
				if (seedType == SeedType.None)
				{
					BeghouledFallIntoSquare(i, num, theBoardState);
				}
			}
		}
	}

	public void ZombieAtePlant(Zombie theZombie, Plant thePlant)
	{
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			mBeghouledEated[thePlant.mPlantCol, thePlant.mRow] = true;
			if (mBoard.mSeedBank.mNumPackets == 4)
			{
				mBoard.mSeedBank.mSeedPackets[4].SetPacketType(SeedType.BeghouledButtonCrater, SeedType.None);
				mBoard.mSeedBank.mNumPackets = 5;
				mBoard.DisplayAdvice("[ADVICE_BEGHOULED_USE_CRATER_1]", MessageStyle.HintFast, AdviceType.BeghouledUseCrater1);
			}
			BeghouledCheckStuckState();
			BeghouledUpdateCraters();
		}
	}

	public void DrawBackdrop(Graphics g)
	{
		if (mApp.IsArtChallenge())
		{
			DrawArtChallenge(g);
		}
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			TreeOfWisdomDraw(g);
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			DrawBeghouled(g);
		}
		if (mApp.IsWallnutBowlingLevel() && mShowBowlingLine)
		{
			g.DrawImage(AtlasResources.IMAGE_WALLNUT_BOWLINGSTRIPE, 268f, 77f * Constants.S);
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie1 || mApp.mGameMode == GameMode.PuzzleIZombie2 || mApp.mGameMode == GameMode.PuzzleIZombie3 || mApp.mGameMode == GameMode.PuzzleIZombie4 || mApp.mGameMode == GameMode.PuzzleIZombie5)
		{
			g.DrawImage(AtlasResources.IMAGE_WALLNUT_BOWLINGSTRIPE, 352f, 73f * Constants.S);
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie6 || mApp.mGameMode == GameMode.PuzzleIZombie7 || mApp.mGameMode == GameMode.PuzzleIZombie8 || mApp.mGameMode == GameMode.PuzzleIZombieEndless)
		{
			g.DrawImage(AtlasResources.IMAGE_WALLNUT_BOWLINGSTRIPE, 432f, 73f * Constants.S);
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie9)
		{
			g.DrawImage(AtlasResources.IMAGE_WALLNUT_BOWLINGSTRIPE, 512f, 73f * Constants.S);
		}
		CSIZombieLevel cSIZombieLevel = mApp.mCreativeLevel?.GetComponent<CSIZombieLevel>();
		if (cSIZombieLevel != null)
		{
			g.DrawImage(AtlasResources.IMAGE_WALLNUT_BOWLINGSTRIPE, 32 + 80 * cSIZombieLevel.mLimit, 73f * Constants.S);
		}
		if (mApp.mGameMode == GameMode.ChallengeIce)
		{
			int mMainCounter = mBoard.mMainCounter;
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mApp.mZenGarden.DrawBackdrop(g);
		}
	}

	public void DrawArtChallenge(Graphics g)
	{
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new SexyColor(255, 255, 255, 150));
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				SeedType artChallengeSeed = GetArtChallengeSeed(j, i);
				if (artChallengeSeed != SeedType.None && mBoard.GetTopPlantAt(j, i, TopPlant.OnlyNormalPosition) == null)
				{
					float num = mBoard.GridToPixelX(j, i);
					float num2 = mBoard.GridToPixelY(j, i);
					num2 += Plant.PlantDrawHeightOffset(mBoard, null, artChallengeSeed, j, i);
					Plant.DrawSeedType(g, artChallengeSeed, SeedType.None, DrawVariation.Normal, num * Constants.S + 40f, num2 * Constants.S + 64f);
				}
			}
		}
		g.SetColorizeImages(colorizeImages: false);
		GameMode mGameMode = mApp.mGameMode;
		if (mGameMode == GameMode.ChallengeArtChallenge1)
		{
			TPoint[] celPosition = mBoard.GetCelPosition(4, 1);
			g.DrawImage(AtlasResources.IMAGE_REANIM_GOOGLYEYE, (float)celPosition[0].mX * Constants.S, (float)celPosition[0].mY * Constants.S);
			celPosition = mBoard.GetCelPosition(6, 1);
			g.DrawImage(AtlasResources.IMAGE_REANIM_GOOGLYEYE, (float)celPosition[0].mX * Constants.S, (float)celPosition[0].mY * Constants.S);
		}
	}

	public void CheckForCompleteArtChallenge(int theGridX, int theGridY)
	{
		if (mBoard.HasLevelAwardDropped())
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				SeedType artChallengeSeed = GetArtChallengeSeed(j, i);
				if (artChallengeSeed != SeedType.None)
				{
					Plant topPlantAt = mBoard.GetTopPlantAt(j, i, TopPlant.OnlyNormalPosition);
					if (topPlantAt == null || topPlantAt.mSeedType != artChallengeSeed)
					{
						return;
					}
				}
			}
		}
		SpawnLevelAward(theGridX, theGridY);
	}

	public SeedType GetArtChallengeSeed(int theGridX, int theGridY)
	{
		if (theGridY > 6)
		{
			return SeedType.None;
		}
		Debug.ASSERT(theGridX >= 0 && theGridX < 9 && theGridY >= 0);
		if (mApp.mGameMode == GameMode.ChallengeArtChallenge1)
		{
			return gArtChallengeWallnut[theGridY, theGridX];
		}
		if (mApp.mGameMode == GameMode.ChallengeArtChallenge2)
		{
			return gArtChallengeSunFlower[theGridY, theGridX];
		}
		if (mApp.mGameMode == GameMode.ChallengeSeeingStars)
		{
			return gArtChallengeStarfruit[theGridY, theGridX];
		}
		return SeedType.None;
	}

	public void PlantAdded(Plant thePlant)
	{
		if (mApp.IsArtChallenge())
		{
			SeedType artChallengeSeed = GetArtChallengeSeed(thePlant.mPlantCol, thePlant.mRow);
			if (artChallengeSeed != SeedType.None && artChallengeSeed == thePlant.mSeedType)
			{
				mApp.PlayFoley(FoleyType.ArtChallenge);
				mApp.AddTodParticle(thePlant.mX + 40, thePlant.mY + 40, 400000, ParticleEffect.PresentPickup);
				CheckForCompleteArtChallenge(thePlant.mPlantCol, thePlant.mRow);
			}
		}
	}

	public PlantingReason CanPlantAt(int theGridX, int theGridY, SeedType theType)
	{
		if (mApp.IsWallnutBowlingLevel())
		{
			if (theGridX > 2)
			{
				return PlantingReason.NotPassedLine;
			}
		}
		else if (mApp.IsIZombieLevel())
		{
			int num = 6;
			if (mApp.mGameMode == GameMode.PuzzleIZombie1 || mApp.mGameMode == GameMode.PuzzleIZombie2 || mApp.mGameMode == GameMode.PuzzleIZombie3 || mApp.mGameMode == GameMode.PuzzleIZombie4 || mApp.mGameMode == GameMode.PuzzleIZombie5)
			{
				num = 4;
			}
			if (mApp.mGameMode == GameMode.PuzzleIZombie6 || mApp.mGameMode == GameMode.PuzzleIZombie7 || mApp.mGameMode == GameMode.PuzzleIZombie8 || mApp.mGameMode == GameMode.PuzzleIZombieEndless)
			{
				num = 5;
			}
			CSIZombieLevel cSIZombieLevel = mApp.mCreativeLevel?.GetComponent<CSIZombieLevel>();
			if (cSIZombieLevel != null)
			{
				num = cSIZombieLevel.mLimit;
			}
			if (theType == SeedType.ZombieBungee || (IsZombieSeedType(theType) && IZombieSeedTypeToZombieType(theType) == ZombieType.Bungee))
			{
				if (theGridX < num)
				{
					return PlantingReason.Ok;
				}
				return PlantingReason.NotHere;
			}
			if (IsZombieSeedType(theType))
			{
				if (theGridX >= num)
				{
					return PlantingReason.Ok;
				}
				return PlantingReason.NotHere;
			}
		}
		else if (mApp.IsArtChallenge())
		{
			SeedType artChallengeSeed = GetArtChallengeSeed(theGridX, theGridY);
			if (artChallengeSeed != SeedType.None && artChallengeSeed != theType && theType != SeedType.Lilypad && theType != SeedType.Pumpkinshell)
			{
				return PlantingReason.NotOnArt;
			}
			if (mApp.mGameMode == GameMode.ChallengeArtChallenge1)
			{
				if (theGridX == 4 && theGridY == 1)
				{
					return PlantingReason.NotHere;
				}
				if (theGridX == 6 && theGridY == 1)
				{
					return PlantingReason.NotHere;
				}
			}
		}
		else if (mApp.IsFinalBossLevel() && theGridX >= 8)
		{
			return PlantingReason.NotHere;
		}
		return PlantingReason.Ok;
	}

	public void DrawBeghouled(Graphics g)
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 9; j++)
			{
				if (mBeghouledEated[j, i])
				{
					int num = mBoard.GridToPixelX(j, i) - 8;
					int num2 = mBoard.GridToPixelY(j, i) + 40;
					g.DrawImageCel(AtlasResources.IMAGE_CRATER, (int)((float)num * Constants.S), (int)((float)num2 * Constants.S), 1, 0);
				}
			}
		}
	}

	public bool BeghouledIsValidMove(int x1, int y1, int x2, int y2, BeghouledBoardState theBoardState)
	{
		if (x1 < 0 || x1 >= Constants.GRIDSIZEX || x2 < 0 || x2 >= Constants.GRIDSIZEX || y1 < 0 || y1 >= Constants.MAX_GRIDSIZEY || y2 < 0 || y2 >= Constants.MAX_GRIDSIZEY)
		{
			return false;
		}
		if (mBeghouledEated[x1, y1] || mBeghouledEated[x2, y2])
		{
			return false;
		}
		SeedType seedType = theBoardState.mSeedType[x1, y1];
		SeedType seedType2 = theBoardState.mSeedType[x2, y2];
		if (seedType == SeedType.None)
		{
			return false;
		}
		theBoardState.mSeedType[x1, y1] = seedType2;
		theBoardState.mSeedType[x2, y2] = seedType;
		bool result = BeghouledBoardHasMatch(theBoardState);
		theBoardState.mSeedType[x1, y1] = seedType;
		theBoardState.mSeedType[x2, y2] = seedType2;
		return result;
	}

	public bool BeghouledCheckForPossibleMoves(BeghouledBoardState theBoardState)
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				if (mApp.mGameMode == GameMode.ChallengeBeghouled)
				{
					if (BeghouledIsValidMove(j, i, j + 1, i, theBoardState) || BeghouledIsValidMove(j, i, j, i + 1, theBoardState))
					{
						return true;
					}
				}
				else if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist && BeghouledTwistMoveCausesMatch(j, i, theBoardState))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void BeghouledCheckStuckState()
	{
		if (mChallengeState == ChallengeState.Normal && !mBoard.HasLevelAwardDropped())
		{
			BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
			LoadBeghouledBoardState(newBeghouledBoardState);
			if (!BeghouledCheckForPossibleMoves(newBeghouledBoardState))
			{
				mChallengeState = ChallengeState.BeghouledNoMatches;
				mChallengeStateCounter = 500;
				mBoard.DisplayAdviceAgain("[ADVICE_BEGHOULED_NO_MOVES]", MessageStyle.HintFast, AdviceType.BeghouledNoMoves);
			}
			newBeghouledBoardState.PrepareForReuse();
		}
	}

	public void InitZombieWavesSurvival()
	{
		mBoard.mZombieAllowed[0] = true;
		int levelRandSeed = mBoard.GetLevelRandSeed();
		RandomNumbers.Seed(levelRandSeed);
		if ((int)RandomNumbers.NextNumber(5f) == 0)
		{
			mBoard.mZombieAllowed[5] = true;
		}
		else
		{
			mBoard.mZombieAllowed[2] = true;
		}
		int num = Math.Min(mSurvivalStage + 1, 9);
		if (mApp.IsSurvivalHell(mApp.mGameMode))
		{
			mBoard.mZombieAllowed[38] = true;
			if (mSurvivalStage >= 1)
			{
				num += 2;
				num = Math.Min(num, 9);
			}
			if (mSurvivalStage == 1)
			{
				mBoard.mZombieAllowed[36] = true;
			}
			if (mApp.mGameMode == GameMode.SurvivalHellStage5 && mSurvivalStage >= 2)
			{
				mBoard.mZombieAllowed[20] = true;
			}
			if (mApp.mGameMode == GameMode.SurvivalHellStage3 || mApp.mGameMode == GameMode.SurvivalHellStage4)
			{
				if (mSurvivalStage >= 1)
				{
					mBoard.mZombieAllowed[14] = true;
					mBoard.mZombieAllowed[7] = true;
				}
				if (mSurvivalStage >= 2)
				{
					mBoard.mZombieAllowed[39] = true;
				}
				mBoard.mZombieAllowed[15] = true;
			}
			if (mSurvivalStage > 3)
			{
				if (mApp.mGameMode == GameMode.SurvivalHellStage5)
				{
					mBoard.mZombieAllowed[21] = true;
				}
				else
				{
					mBoard.mZombieAllowed[17] = true;
				}
				mBoard.mZombieAllowed[16] = true;
				mBoard.mZombieAllowed[36] = true;
				mBoard.mZombieAllowed[33] = true;
				mBoard.mZombieAllowed[34] = true;
			}
		}
		if (mBoard.mBackground == BackgroundType.BigPool)
		{
			mBoard.mZombieAllowed[5] = false;
			mBoard.mZombieAllowed[2] = false;
			num = Math.Min(num, 5);
		}
		while (num > 0)
		{
			if (mApp.mGameMode == GameMode.BigPoolSurvivalNormalStage)
			{
				if (mSurvivalStage == 1)
				{
					mBoard.mZombieAllowed[0] = true;
					mBoard.mZombieAllowed[2] = true;
					mBoard.mZombieAllowed[4] = true;
					mBoard.mZombieAllowed[14] = true;
					mBoard.mZombieAllowed[11] = true;
				}
				if (mSurvivalStage == 2)
				{
					mBoard.mZombieAllowed[0] = true;
					mBoard.mZombieAllowed[2] = true;
					mBoard.mZombieAllowed[4] = true;
					mBoard.mZombieAllowed[14] = true;
					mBoard.mZombieAllowed[11] = true;
					mBoard.mZombieAllowed[16] = true;
					mBoard.mZombieAllowed[15] = true;
				}
				if (mSurvivalStage == 3)
				{
					mBoard.mZombieAllowed[0] = true;
					mBoard.mZombieAllowed[4] = true;
					mBoard.mZombieAllowed[11] = true;
					mBoard.mZombieAllowed[14] = true;
					mBoard.mZombieAllowed[16] = true;
					mBoard.mZombieAllowed[7] = true;
				}
				if (mSurvivalStage >= 4)
				{
					mBoard.mZombieAllowed[0] = true;
					mBoard.mZombieAllowed[4] = true;
					mBoard.mZombieAllowed[11] = true;
					mBoard.mZombieAllowed[14] = true;
					mBoard.mZombieAllowed[39] = true;
					mBoard.mZombieAllowed[15] = true;
					mBoard.mZombieAllowed[7] = true;
					mBoard.mZombieAllowed[36] = true;
				}
				break;
			}
			ZombieType zombieType = (ZombieType)RandomNumbers.NextNumber(40);
			if (mApp.IsSurvivalHell(mApp.mGameMode))
			{
				if (mSurvivalStage > 1 && mBoard.mZombieAllowed[5])
				{
					mBoard.mZombieAllowed[5] = false;
					zombieType = ZombieType.Pail;
				}
				if ((mSurvivalStage > 1 && zombieType == ZombieType.Newspaper) || (mSurvivalStage < 2 && zombieType == ZombieType.RobotTitan) || (mSurvivalStage < 2 && zombieType == ZombieType.RedeyeRobotTitan))
				{
					continue;
				}
				if (mApp.mGameMode == GameMode.SurvivalHellStage5)
				{
					mBoard.mZombieAllowed[12] = false;
					if (zombieType == ZombieType.Zamboni)
					{
						continue;
					}
				}
				if (zombieType == ZombieType.Propeller && (mApp.mGameMode == GameMode.SurvivalHellStage1 || mApp.mGameMode == GameMode.SurvivalHellStage2))
				{
					continue;
				}
			}
			if (mSurvivalStage > 3 && mApp.mGameMode == GameMode.SurvivalHellStage2)
			{
				mBoard.mZombieAllowed[22] = false;
				mBoard.mZombieAllowed[34] = false;
				mBoard.mZombieAllowed[38] = false;
				if (zombieType == ZombieType.Catapult || zombieType == ZombieType.RedeyeRobotTitan || zombieType == ZombieType.Talisman)
				{
					continue;
				}
			}
			if (!mBoard.mZombieAllowed[(int)zombieType] && (Zombie.ZombieTypeCanGoInPool(zombieType) || mBoard.mBackground != BackgroundType.BigPool) && (!Board.IsZombieTypePoolOnly(zombieType) || mBoard.StageHasPool()) && (!mBoard.StageHasRoof() || (zombieType != ZombieType.Digger && zombieType != ZombieType.Dancer)) && (!mBoard.StageHasGraveStones() || zombieType != ZombieType.Zamboni) && (mBoard.StageHasRoof() || mApp.IsSurvivalEndless(mApp.mGameMode) || zombieType != ZombieType.Bungee) && (mBoard.GetSurvivalFlagsCompleted() >= 4 || (zombieType != ZombieType.Gargantuar && zombieType != ZombieType.Zamboni)) && (mBoard.GetSurvivalFlagsCompleted() >= 10 || zombieType != ZombieType.RedeyeGargantuar) && ((mApp.mGameMode != GameMode.SurvivalNormalStage1 && mApp.mGameMode != GameMode.SurvivalNormalStage2 && mApp.mGameMode != GameMode.SurvivalNormalStage3) || zombieType <= ZombieType.Snorkel) && zombieType != ZombieType.Bobsled && zombieType != ZombieType.BackupDancer && zombieType != ZombieType.Imp && zombieType != ZombieType.DuckyTube && zombieType != ZombieType.PeaHead && zombieType != ZombieType.WallnutHead && zombieType != ZombieType.TallnutHead && zombieType != ZombieType.JalapenoHead && zombieType != ZombieType.GatlingHead && zombieType != ZombieType.SquashHead && zombieType != ZombieType.Yeti && (mApp.IsSurvivalHell(mApp.mGameMode) || (zombieType != ZombieType.RobotTitan && zombieType != ZombieType.RedeyeRobotTitan && zombieType != ZombieType.Monk && zombieType != ZombieType.FootballPremium && zombieType != ZombieType.Ninja && zombieType != ZombieType.Talisman && zombieType != ZombieType.Propeller)))
			{
				mBoard.mZombieAllowed[(int)zombieType] = true;
				num--;
			}
		}
	}

	public void InitZombieWavesChallengStageRandom()
	{
		int num = 9;
		ZombieType[] array = new ZombieType[4]
		{
			ZombieType.Gargantuar,
			ZombieType.RedeyeGargantuar,
			ZombieType.RobotTitan,
			ZombieType.RedeyeRobotTitan
		};
		mBoard.mZombieAllowed[0] = true;
		int levelRandSeed = mBoard.GetLevelRandSeed();
		RandomNumbers.Seed(levelRandSeed);
		if (mBoard.mBackground == BackgroundType.BigPool)
		{
			num = 6;
			mBoard.mZombieAllowed[36] = true;
		}
		while (num > 0)
		{
			ZombieType zombieType = (ZombieType)RandomNumbers.NextNumber(40);
			if (mBoard.mZombieAllowed[(int)zombieType] || (!Zombie.ZombieTypeCanGoInPool(zombieType) && mBoard.mBackground == BackgroundType.BigPool) || (Board.IsZombieTypePoolOnly(zombieType) && !mBoard.StageHasPool()) || (mBoard.StageHasRoof() && (zombieType == ZombieType.Digger || zombieType == ZombieType.Dancer)) || (mBoard.StageHasGraveStones() && zombieType == ZombieType.Zamboni) || (!mBoard.StageHasRoof() && !mApp.IsSurvivalEndless(mApp.mGameMode) && zombieType == ZombieType.Bungee) || zombieType == ZombieType.Bobsled || zombieType == ZombieType.BackupDancer || zombieType == ZombieType.Imp || zombieType == ZombieType.DuckyTube || zombieType == ZombieType.PeaHead || zombieType == ZombieType.WallnutHead || zombieType == ZombieType.TallnutHead || zombieType == ZombieType.JalapenoHead || zombieType == ZombieType.GatlingHead || zombieType == ZombieType.SquashHead || zombieType == ZombieType.Yeti)
			{
				continue;
			}
			bool flag = false;
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (zombieType == array[i])
				{
					flag = true;
				}
				if (mBoard.mZombieAllowed[(int)array[i]])
				{
					num2++;
				}
			}
			if (!flag || num2 < 2)
			{
				mBoard.mZombieAllowed[(int)zombieType] = true;
				num--;
			}
		}
	}

	public void InitZombieWavesFromList(ZombieType[] theZombieList, int theListLength)
	{
		for (int i = 0; i < theListLength; i++)
		{
			ZombieType zombieType = theZombieList[i];
			mBoard.mZombieAllowed[(int)zombieType] = true;
		}
	}

	public void InitZombieWaves()
	{
		if (mApp.IsSurvivalMode())
		{
			if (mApp.mGameMode == GameMode.RogueConveyorbelt || mApp.mGameMode == GameMode.RogueConveyorbeltHard)
			{
				TodWeightedArray[] array = new TodWeightedArray[100];
				int num = 0;
				ZombieType[] array2 = ((mSurvivalStage <= 2) ? new ZombieType[24]
				{
					ZombieType.TrafficCone,
					ZombieType.Polevaulter,
					ZombieType.Pail,
					ZombieType.Newspaper,
					ZombieType.Door,
					ZombieType.Football,
					ZombieType.Dancer,
					ZombieType.BackupDancer,
					ZombieType.DuckyTube,
					ZombieType.Snorkel,
					ZombieType.Zamboni,
					ZombieType.Bobsled,
					ZombieType.DolphinRider,
					ZombieType.JackInTheBox,
					ZombieType.Balloon,
					ZombieType.Digger,
					ZombieType.Pogo,
					ZombieType.Bungee,
					ZombieType.Ladder,
					ZombieType.Catapult,
					ZombieType.Gargantuar,
					ZombieType.RobotTitan,
					ZombieType.Monk,
					ZombieType.FootballPremium
				} : new ZombieType[26]
				{
					ZombieType.TrafficCone,
					ZombieType.Polevaulter,
					ZombieType.Pail,
					ZombieType.Newspaper,
					ZombieType.Door,
					ZombieType.Football,
					ZombieType.Dancer,
					ZombieType.BackupDancer,
					ZombieType.DuckyTube,
					ZombieType.Snorkel,
					ZombieType.Zamboni,
					ZombieType.Bobsled,
					ZombieType.DolphinRider,
					ZombieType.JackInTheBox,
					ZombieType.Balloon,
					ZombieType.Digger,
					ZombieType.Pogo,
					ZombieType.Bungee,
					ZombieType.Ladder,
					ZombieType.Catapult,
					ZombieType.Gargantuar,
					ZombieType.RobotTitan,
					ZombieType.RedeyeRobotTitan,
					ZombieType.Monk,
					ZombieType.FootballPremium,
					ZombieType.Ninja
				});
				foreach (ZombieType zombieType in array2)
				{
					array[num] = TodWeightedArray.GetNewTodWeightedArray();
					array[num].mItem = zombieType;
					array[num].mWeight = 1;
					num++;
				}
				ZombieType[] array3 = new ZombieType[11]
				{
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal,
					ZombieType.Normal
				};
				for (int j = 1; j < array3.Length; j++)
				{
					array3[j] = (ZombieType)TodCommon.TodPickArrayItemFromWeightedArray(array, num).mItem;
				}
				InitZombieWavesFromList(array3, array3.Length);
			}
			else if (mSurvivalStage == 0 && mApp.IsSurvivalNormal(mApp.mGameMode))
			{
				ZombieType[] array4 = new ZombieType[2]
				{
					ZombieType.Normal,
					ZombieType.TrafficCone
				};
				InitZombieWavesFromList(array4, array4.Length);
			}
			else if (mSurvivalStage == 0 && mApp.IsSurvivalHell(mApp.mGameMode))
			{
				ZombieType[] array5 = new ZombieType[3]
				{
					ZombieType.Normal,
					ZombieType.TrafficCone,
					ZombieType.Pail
				};
				if (mApp.mGameMode == GameMode.SurvivalHellStage1)
				{
					array5 = new ZombieType[5]
					{
						ZombieType.Normal,
						ZombieType.TrafficCone,
						ZombieType.Pail,
						ZombieType.Polevaulter,
						ZombieType.Talisman
					};
				}
				else if (mApp.mGameMode == GameMode.SurvivalHellStage2)
				{
					array5 = new ZombieType[4]
					{
						ZombieType.Normal,
						ZombieType.TrafficCone,
						ZombieType.Pail,
						ZombieType.Talisman
					};
				}
				else if (mApp.mGameMode == GameMode.SurvivalHellStage3)
				{
					array5 = new ZombieType[5]
					{
						ZombieType.Normal,
						ZombieType.TrafficCone,
						ZombieType.Pail,
						ZombieType.Zamboni,
						ZombieType.Talisman
					};
				}
				else if (mApp.mGameMode == GameMode.SurvivalHellStage4)
				{
					array5 = new ZombieType[4]
					{
						ZombieType.Normal,
						ZombieType.TrafficCone,
						ZombieType.Pail,
						ZombieType.Talisman
					};
				}
				else if (mApp.mGameMode == GameMode.SurvivalHellStage5)
				{
					array5 = new ZombieType[4]
					{
						ZombieType.Normal,
						ZombieType.TrafficCone,
						ZombieType.Pail,
						ZombieType.Talisman
					};
				}
				InitZombieWavesFromList(array5, array5.Length);
			}
			else if (mSurvivalStage == 0)
			{
				ZombieType[] array6 = new ZombieType[3]
				{
					ZombieType.Normal,
					ZombieType.TrafficCone,
					ZombieType.Pail
				};
				InitZombieWavesFromList(array6, array6.Length);
			}
			else
			{
				InitZombieWavesSurvival();
			}
		}
		else if (mApp.mGameMode == GameMode.ChallengeSpeed)
		{
			ZombieType[] array7 = new ZombieType[4]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.DolphinRider,
				ZombieType.Polevaulter
			};
			InitZombieWavesFromList(array7, array7.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengePogoParty)
		{
			ZombieType[] array8 = new ZombieType[1] { ZombieType.Pogo };
			InitZombieWavesFromList(array8, array8.Length);
		}
		else if (mApp.IsBungeeBlitzLevel())
		{
			ZombieType[] array9 = new ZombieType[4]
			{
				ZombieType.Normal,
				ZombieType.Pail,
				ZombieType.TrafficCone,
				ZombieType.Ladder
			};
			InitZombieWavesFromList(array9, array9.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeSunnyDay)
		{
			ZombieType[] array10 = new ZombieType[6]
			{
				ZombieType.Normal,
				ZombieType.Pail,
				ZombieType.TrafficCone,
				ZombieType.Football,
				ZombieType.Polevaulter,
				ZombieType.JackInTheBox
			};
			InitZombieWavesFromList(array10, array10.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengePortalCombat)
		{
			ZombieType[] array11 = new ZombieType[4]
			{
				ZombieType.Normal,
				ZombieType.Pail,
				ZombieType.Football,
				ZombieType.Balloon
			};
			InitZombieWavesFromList(array11, array11.Length);
		}
		else if (mApp.IsLittleTroubleLevel())
		{
			ZombieType[] array12 = new ZombieType[4]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Football,
				ZombieType.Snorkel
			};
			InitZombieWavesFromList(array12, array12.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeBigTime)
		{
			ZombieType[] array13 = new ZombieType[6]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Door,
				ZombieType.Football,
				ZombieType.JackInTheBox
			};
			InitZombieWavesFromList(array13, array13.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeRainingSeeds)
		{
			ZombieType[] array14 = new ZombieType[8]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Door,
				ZombieType.Football,
				ZombieType.Newspaper,
				ZombieType.JackInTheBox,
				ZombieType.Bungee
			};
			InitZombieWavesFromList(array14, array14.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeHighGravity)
		{
			ZombieType[] array15 = new ZombieType[5]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Door,
				ZombieType.Balloon
			};
			InitZombieWavesFromList(array15, array15.Length);
		}
		else if (mApp.IsWhackAZombieLevel())
		{
			ZombieType[] array16 = new ZombieType[3]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail
			};
			InitZombieWavesFromList(array16, array16.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeStageRandom)
		{
			InitZombieWavesChallengStageRandom();
		}
		else if (mApp.mGameMode == GameMode.ChallengeBobsledBonanza)
		{
			ZombieType[] array17 = new ZombieType[2]
			{
				ZombieType.Bobsled,
				ZombieType.Zamboni
			};
			InitZombieWavesFromList(array17, array17.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeBobsledBonanza2)
		{
			ZombieType[] array18 = new ZombieType[7]
			{
				ZombieType.Bobsled,
				ZombieType.Zamboni,
				ZombieType.Football,
				ZombieType.FootballPremium,
				ZombieType.Talisman,
				ZombieType.Propeller,
				ZombieType.JackInTheBox
			};
			InitZombieWavesFromList(array18, array18.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeAirRaid)
		{
			ZombieType[] array19 = new ZombieType[1] { ZombieType.Balloon };
			InitZombieWavesFromList(array19, array19.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeMoreAirRaid)
		{
			ZombieType[] array20 = new ZombieType[7]
			{
				ZombieType.Propeller,
				ZombieType.Normal,
				ZombieType.Balloon,
				ZombieType.Newspaper,
				ZombieType.Polevaulter,
				ZombieType.Ladder,
				ZombieType.Bungee
			};
			InitZombieWavesFromList(array20, array20.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			ZombieType[] array21 = new ZombieType[6]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Door,
				ZombieType.Football,
				ZombieType.Newspaper
			};
			InitZombieWavesFromList(array21, array21.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			ZombieType[] array22 = new ZombieType[10]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Door,
				ZombieType.Football,
				ZombieType.Newspaper,
				ZombieType.JackInTheBox,
				ZombieType.Polevaulter,
				ZombieType.DolphinRider,
				ZombieType.Ladder
			};
			InitZombieWavesFromList(array22, array22.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn)
		{
			ZombieType[] array23 = new ZombieType[4]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Football
			};
			InitZombieWavesFromList(array23, array23.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn2)
		{
			ZombieType[] array24 = new ZombieType[6]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Football,
				ZombieType.Polevaulter,
				ZombieType.Zamboni
			};
			InitZombieWavesFromList(array24, array24.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn3)
		{
			ZombieType[] array25 = new ZombieType[3]
			{
				ZombieType.Normal,
				ZombieType.Pail,
				ZombieType.Digger
			};
			InitZombieWavesFromList(array25, array25.Length);
		}
		else if (mApp.mGameMode == GameMode.OccupyHighGround)
		{
			ZombieType[] array26 = new ZombieType[7]
			{
				ZombieType.Normal,
				ZombieType.Pail,
				ZombieType.Digger,
				ZombieType.Football,
				ZombieType.Propeller,
				ZombieType.JackInTheBox,
				ZombieType.Polevaulter
			};
			InitZombieWavesFromList(array26, array26.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeInvisighoul)
		{
			ZombieType[] array27 = new ZombieType[6]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.DolphinRider,
				ZombieType.Zamboni,
				ZombieType.JackInTheBox
			};
			InitZombieWavesFromList(array27, array27.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeWarAndPeas)
		{
			ZombieType[] array28 = new ZombieType[1];
			InitZombieWavesFromList(array28, array28.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeWarAndPeas2)
		{
			ZombieType[] array29 = new ZombieType[1];
			InitZombieWavesFromList(array29, array29.Length);
		}
		else if (mApp.IsShovelLevel())
		{
			ZombieType[] array30 = new ZombieType[2]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone
			};
			InitZombieWavesFromList(array30, array30.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeWallnutBowling || mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
		{
			ZombieType[] array31 = new ZombieType[5]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Polevaulter,
				ZombieType.Newspaper
			};
			InitZombieWavesFromList(array31, array31.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeWallnutBowling2)
		{
			ZombieType[] array32 = new ZombieType[7]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Polevaulter,
				ZombieType.Newspaper,
				ZombieType.Dancer,
				ZombieType.Door
			};
			InitZombieWavesFromList(array32, array32.Length);
		}
		else if (mApp.IsStormyNightLevel())
		{
			ZombieType[] array33 = new ZombieType[4]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.DolphinRider,
				ZombieType.Balloon
			};
			InitZombieWavesFromList(array33, array33.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans)
		{
			ZombieType[] array34 = new ZombieType[7]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Polevaulter,
				ZombieType.JackInTheBox,
				ZombieType.Football,
				ZombieType.Gargantuar,
				ZombieType.RobotTitan
			};
			InitZombieWavesFromList(array34, array34.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans2)
		{
			ZombieType[] array35 = new ZombieType[8]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Polevaulter,
				ZombieType.JackInTheBox,
				ZombieType.Football,
				ZombieType.Gargantuar,
				ZombieType.RobotTitan,
				ZombieType.RedeyeRobotTitan
			};
			InitZombieWavesFromList(array35, array35.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans3)
		{
			ZombieType[] array36 = new ZombieType[9]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Polevaulter,
				ZombieType.JackInTheBox,
				ZombieType.Football,
				ZombieType.Gargantuar,
				ZombieType.RobotTitan,
				ZombieType.RedeyeRobotTitan,
				ZombieType.Ladder
			};
			InitZombieWavesFromList(array36, array36.Length);
		}
		else if (mApp.mGameMode == GameMode.MarvelousPeople)
		{
			ZombieType[] array37 = new ZombieType[6]
			{
				ZombieType.Normal,
				ZombieType.Football,
				ZombieType.Monk,
				ZombieType.Polevaulter,
				ZombieType.Dancer,
				ZombieType.RobotTitan
			};
			InitZombieWavesFromList(array37, array37.Length);
		}
		else if (mApp.mGameMode == GameMode.MarvelousPeople2)
		{
			ZombieType[] array38 = new ZombieType[9]
			{
				ZombieType.Normal,
				ZombieType.FootballPremium,
				ZombieType.Monk,
				ZombieType.Polevaulter,
				ZombieType.Bungee,
				ZombieType.DolphinRider,
				ZombieType.Snorkel,
				ZombieType.JackInTheBox,
				ZombieType.Ninja
			};
			InitZombieWavesFromList(array38, array38.Length);
		}
		else if (mApp.mGameMode == GameMode.MarvelousPeople3)
		{
			ZombieType[] array39 = new ZombieType[9]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.FootballPremium,
				ZombieType.Monk,
				ZombieType.Polevaulter,
				ZombieType.Bungee,
				ZombieType.RobotTitan,
				ZombieType.RedeyeRobotTitan,
				ZombieType.Ninja
			};
			InitZombieWavesFromList(array39, array39.Length);
		}
		else if (mApp.mGameMode == GameMode.ImitaterRandom)
		{
			ZombieType[] array40 = new ZombieType[12]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.FootballPremium,
				ZombieType.Monk,
				ZombieType.Dancer,
				ZombieType.Polevaulter,
				ZombieType.Bungee,
				ZombieType.Football,
				ZombieType.Door,
				ZombieType.Gargantuar,
				ZombieType.RobotTitan,
				ZombieType.RedeyeRobotTitan
			};
			InitZombieWavesFromList(array40, array40.Length);
		}
		else if (mApp.mGameMode == GameMode.MarvelousPeople4)
		{
			ZombieType[] array41 = new ZombieType[10]
			{
				ZombieType.Talisman,
				ZombieType.Football,
				ZombieType.FootballPremium,
				ZombieType.Monk,
				ZombieType.Polevaulter,
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Gargantuar,
				ZombieType.RobotTitan
			};
			InitZombieWavesFromList(array41, array41.Length);
		}
		else if (mApp.mGameMode == GameMode.PoolParty)
		{
			ZombieType[] array42 = new ZombieType[11]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail,
				ZombieType.Football,
				ZombieType.FootballPremium,
				ZombieType.Snorkel,
				ZombieType.DolphinRider,
				ZombieType.Bungee,
				ZombieType.Balloon,
				ZombieType.JackInTheBox,
				ZombieType.Propeller
			};
			InitZombieWavesFromList(array42, array42.Length);
		}
		else if (mApp.mGameMode == GameMode.ChallengeFusion)
		{
			ZombieType[] array43 = new ZombieType[15]
			{
				ZombieType.Football,
				ZombieType.FootballPremium,
				ZombieType.Monk,
				ZombieType.Propeller,
				ZombieType.Normal,
				ZombieType.Door,
				ZombieType.Pail,
				ZombieType.Gargantuar,
				ZombieType.RobotTitan,
				ZombieType.RedeyeRobotTitan,
				ZombieType.Zamboni,
				ZombieType.DolphinRider,
				ZombieType.Snorkel,
				ZombieType.JackInTheBox,
				ZombieType.Talisman
			};
			InitZombieWavesFromList(array43, array43.Length);
		}
		else
		{
			ZombieType[] array44 = new ZombieType[3]
			{
				ZombieType.Normal,
				ZombieType.TrafficCone,
				ZombieType.Pail
			};
			InitZombieWavesFromList(array44, array44.Length);
		}
		if (mApp.CanSpawnYetis() && !mApp.IsWhackAZombieLevel() && !mApp.IsLittleTroubleLevel())
		{
			mBoard.mZombieAllowed[19] = true;
		}
	}

	public void InitZombieWavesSurvivalComponent()
	{
		CSSurvivalLevel component = mApp.mCreativeLevel.GetComponent<CSSurvivalLevel>();
		CSOverrideZombieProperty component2 = mApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
		RandomNumbers.Seed(mBoard.GetLevelRandSeed());
		List<ZombieType> list = component.mZombies.Keys.ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			bool flag = true;
			if (component.mZombies.TryGetValue(list[num], out var value))
			{
				ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(list[num]);
				CSOverrideZombieProperty.Info value2 = null;
				component2?.mProperties.TryGetValue(list[num], out value2);
				int num2 = value.mStartingWave ?? value2?.mFirstAllowedWave ?? zombieDefinition.mFirstAllowedWave;
				int num3 = (mSurvivalStage + 1) * mBoard.GetNumWavesPerFlag();
				if (num3 >= num2)
				{
					flag = false;
				}
			}
			if (flag)
			{
				list.RemoveAt(num);
			}
		}
		int val = 0;
		foreach (List<(ZombieType, int)> item in component.mMustHaveZombieGroup)
		{
			val = Math.Max(val, item.Count);
		}
		TodWeightedArray[] array = new TodWeightedArray[Math.Max(val, 1)];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = TodWeightedArray.GetNewTodWeightedArray();
		}
		foreach (List<(ZombieType, int)> item2 in component.mMustHaveZombieGroup)
		{
			List<(ZombieType, int)> list2 = item2.ToList();
			for (int num4 = list2.Count - 1; num4 >= 0; num4--)
			{
				if (!list.Contains(list2[num4].Item1))
				{
					list2.RemoveAt(num4);
				}
			}
			int count = list2.Count;
			if (count != 0)
			{
				for (int j = 0; j < count; j++)
				{
					array[j].mItem = (int)list2[j].Item1;
					array[j].mWeight = list2[j].Item2;
					list.Remove(list2[j].Item1);
				}
				ZombieType zombieType = (ZombieType)TodCommon.TodPickFromWeightedArray(array, count);
				mBoard.mZombieAllowed[(int)zombieType] = true;
			}
		}
		for (int k = 0; k < array.Length; k++)
		{
			array[k].PrepareForReuse();
		}
		if (list.Count != 0)
		{
			int num5 = Math.Min(mSurvivalStage + 1, component.mMaxRandomZombieTypeCount);
			while (num5 > 0 && list.Count > 0)
			{
				int index = RandomNumbers.NextNumber(list.Count);
				ZombieType zombieType2 = list[index];
				list.RemoveAt(index);
				mBoard.mZombieAllowed[(int)zombieType2] = true;
				num5--;
			}
		}
	}

	public void UpdateSlotMachine()
	{
		int num = 2000;
		CSSlotMachineLevel cSSlotMachineLevel = mApp.mCreativeLevel?.GetComponent<CSSlotMachineLevel>();
		if (cSSlotMachineLevel != null)
		{
			num = cSSlotMachineLevel.mTargetSunCount;
		}
		int num2 = TodCommon.ClampInt(mBoard.mSunMoney, 0, num);
		if (num2 >= num - 100)
		{
			mBoard.DisplayAdvice("[ADVICE_ALMOST_THERE]", MessageStyle.HintFast, AdviceType.AlmostThere);
		}
		if (num2 >= num)
		{
			SpawnLevelAward(4, 2);
			mBoard.ClearAdvice(AdviceType.None);
		}
		mBoard.mProgressMeterWidth = TodCommon.TodAnimateCurve(0, num, num2, 0, 150, TodCurves.Linear);
		if (!mBoard.mAdvice.IsBeingDisplayed())
		{
			if (slotMachineMessageCached != num)
			{
				slotMachineMessage = TodCommon.TodReplaceNumberString("[ADVICE_SLOT_MACHINE_COLLECT_SUN]", "{SCORE}", num);
				slotMachineMessageCached = num;
			}
			mBoard.DisplayAdvice(slotMachineMessage, MessageStyle.SlotMachine, AdviceType.SlotMachineCollectSun);
		}
		if (mChallengeState != ChallengeState.SlotMachineRolling)
		{
			if (!mBoard.mAdvice.IsBeingDisplayed() && !mBoard.HasLevelAwardDropped())
			{
				mBoard.DisplayAdviceAgain("[ADVICE_SLOT_MACHINE_SPIN_AGAIN]", MessageStyle.SlotMachine, AdviceType.SlotMachineSpinAgain);
			}
		}
		else
		{
			if (mBoard.mSeedBank.mSeedPackets[0].mSlotMachineCountDown > 0)
			{
				return;
			}
			Reanimation reanimation = mApp.ReanimationGet(mReanimChallenge);
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_return, ReanimLoopType.PlayOnceAndHold, 0, 24f);
			mChallengeState = ChallengeState.Normal;
			SeedType mPacketType = mBoard.mSeedBank.mSeedPackets[0].mPacketType;
			SeedType mPacketType2 = mBoard.mSeedBank.mSeedPackets[1].mPacketType;
			SeedType mPacketType3 = mBoard.mSeedBank.mSeedPackets[2].mPacketType;
			if (mPacketType != mPacketType2 || mPacketType2 != mPacketType3)
			{
				if (mPacketType != mPacketType2 && mPacketType2 != mPacketType3 && mPacketType != mPacketType3)
				{
					return;
				}
				mApp.PlayFoley(FoleyType.ArtChallenge);
				SeedType seedType = ((mPacketType != mPacketType2 && mPacketType != mPacketType3) ? mPacketType2 : mPacketType);
				switch (seedType)
				{
				case SeedType.SlotMachineDiamond:
					mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_2_DIAMONDS]", MessageStyle.SlotMachine, AdviceType.None);
					mBoard.AddCoin(360, 85, CoinType.Diamond, CoinMotion.Coin);
					break;
				case SeedType.SlotMachineSun:
				{
					mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_2_SUNS]", MessageStyle.SlotMachine, AdviceType.None);
					int num3 = 4;
					for (int i = 0; i < num3; i++)
					{
						int theX = 320 + i * 60 / num3;
						mBoard.AddCoin(theX, 85, CoinType.Sun, CoinMotion.Coin);
					}
					break;
				}
				default:
				{
					mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_2_OF_A_KIND]", MessageStyle.SlotMachine, AdviceType.None);
					Coin coin = mBoard.AddCoin(360, 85, CoinType.UsableSeedPacket, CoinMotion.Coin);
					coin.mUsableSeedType = seedType;
					break;
				}
				}
				return;
			}
			mApp.PlayFoley(FoleyType.ArtChallenge);
			switch (mPacketType)
			{
			case SeedType.SlotMachineDiamond:
			{
				mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_DIAMOND_JACKPOT]", MessageStyle.SlotMachine, AdviceType.None);
				int num4 = 5;
				for (int j = 0; j < num4; j++)
				{
					int theX2 = 320 + j * 60 / num4;
					mBoard.AddCoin(theX2, 85, CoinType.Diamond, CoinMotion.Coin);
				}
				return;
			}
			case SeedType.SlotMachineSun:
			{
				mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_SUN_JACKPOT]", MessageStyle.SlotMachine, AdviceType.None);
				int num5 = 20;
				for (int k = 0; k < num5; k++)
				{
					int theX3 = 320 + k * 60 / num5;
					mBoard.AddCoin(theX3, 85, CoinType.Sun, CoinMotion.Coin);
				}
				return;
			}
			}
			mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_3_OF_A_KIND]", MessageStyle.SlotMachine, AdviceType.None);
			int num6 = 3;
			for (int l = 0; l < num6; l++)
			{
				int theX4 = 320 + l * 60 / num6;
				Coin coin2 = mBoard.AddCoin(theX4, 85, CoinType.UsableSeedPacket, CoinMotion.Coin);
				coin2.mUsableSeedType = mPacketType;
			}
		}
	}

	public void DrawSlotMachine(Graphics g)
	{
		if (mApp.mGameScene != GameScenes.ZombiesWon)
		{
			Graphics graphics = Graphics.GetNew(g);
			graphics.mTransX = mBoard.mX;
			graphics.mTransY = mBoard.mY;
			TRect tRect = SlotMachineRect();
			graphics.DrawImage(Resources.IMAGE_SLOTMACHINE_OVERLAY, tRect.mX, tRect.mY);
			Reanimation reanimation = mApp.ReanimationGet(mReanimChallenge);
			reanimation.Draw(graphics);
			if (mSlotMachineRollCount < 3 && mBoard.mCursorObject.mCursorType == CursorType.Normal && mChallengeState != ChallengeState.SlotMachineRolling && !mBoard.HasLevelAwardDropped())
			{
				byte b = (byte)(150.0 * Math.Sin((double)mBoard.mMainCounter % 150.0 / 150.0 * 3.1415927410125732));
				SexyColor sexyColor = new SexyColor(b, b, b, 255);
				graphics.SetColorizeImages(colorizeImages: true);
				graphics.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
				graphics.SetColor(sexyColor);
				graphics.DrawImage(Resources.IMAGE_SLOTMACHINE_OVERLAY, tRect.mX, tRect.mY);
				reanimation.Draw(graphics);
			}
			graphics.PrepareForReuse();
		}
	}

	public TRect SlotMachineGetHandleRect()
	{
		return new TRect(Constants.Challenge_SlotMachineHandle_Pos.mX, Constants.Challenge_SlotMachineHandle_Pos.mY, Constants.Challenge_SlotMachineHandle_Pos.mWidth, Constants.Challenge_SlotMachineHandle_Pos.mHeight);
	}

	public TRect SlotMachineRect()
	{
		return new TRect(Constants.Challenge_SlotMachine_Pos.X, Constants.Challenge_SlotMachine_Pos.Y, Resources.IMAGE_SLOTMACHINE_OVERLAY.mWidth, Resources.IMAGE_SLOTMACHINE_OVERLAY.mHeight);
	}

	public void WhackAZombieSpawning()
	{
		if (mBoard.mCurrentWave == mBoard.mNumWaves && mBoard.mZombieCountDown == 0)
		{
			return;
		}
		CSWhackAZombieLevel cSWhackAZombieLevel = mApp.mCreativeLevel?.GetComponent<CSWhackAZombieLevel>();
		mBoard.mZombieCountDown--;
		int num = 300;
		if (mBoard.mZombieCountDown == 100 && mBoard.mCurrentWave > 0)
		{
			int graveStoneCount = mBoard.GetGraveStoneCount();
			int num2 = cSWhackAZombieLevel?.mMinGraveStoneCount ?? 5;
			int theGraveCount = Math.Max(1, num2 - graveStoneCount);
			WhackAZombiePlaceGraves(theGraveCount);
		}
		if (mBoard.mZombieCountDown == 5)
		{
			mBoard.NextWaveComing();
		}
		if (mBoard.mZombieCountDown == 0)
		{
			mBoard.mZombieCountDown = cSWhackAZombieLevel?.mSpawnInterval ?? 2000;
			mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
			mBoard.mCurrentWave++;
			if (mBoard.mCurrentWave == mBoard.mNumWaves)
			{
				mChallengeStateCounter = 100;
			}
			else
			{
				mChallengeStateCounter = 1;
			}
		}
		else if (mBoard.mZombieCountDown < 300)
		{
			return;
		}
		mChallengeStateCounter--;
		if (mChallengeStateCounter != 0)
		{
			return;
		}
		int num3 = TodCommon.ClampInt((mBoard.mCurrentWave - 1) * 6 / 12, 0, 5);
		int num4 = 1;
		ZombieType theZombieType = ZombieType.Normal;
		int num5 = RandomNumbers.NextNumber(100);
		int num6 = RandomNumbers.NextNumber(100);
		Span<int> span = aDoubleChance;
		Span<int> span2 = aTripleChance;
		Span<int> span3 = aPailChance;
		Span<int> span4 = aConeChance;
		if (cSWhackAZombieLevel != null)
		{
			span = cSWhackAZombieLevel.mDoubleChance;
			span2 = cSWhackAZombieLevel.mTripleChance;
		}
		bool flag = mBoard.mCurrentWave == mBoard.mNumWaves;
		if (flag)
		{
			num4 = 20;
		}
		else if (num5 < span2[num3])
		{
			num4 = 3;
		}
		else if (num5 < span2[num3] + span[num3])
		{
			num4 = 2;
		}
		ZombieType key;
		CSWhackAZombieLevel.Info value;
		if (cSWhackAZombieLevel != null && cSWhackAZombieLevel.mZombies.Count > 0)
		{
			TodWeightedArray[] array = new TodWeightedArray[cSWhackAZombieLevel.mZombies.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = TodWeightedArray.GetNewTodWeightedArray();
			}
			int num7 = 0;
			foreach (KeyValuePair<ZombieType, CSWhackAZombieLevel.Info> mZombie in cSWhackAZombieLevel.mZombies)
			{
				mZombie.Deconstruct(out key, out value);
				ZombieType zombieType = key;
				CSWhackAZombieLevel.Info info = value;
				if (info.mSpawnAtTriple || num4 < 3)
				{
					array[num7].mItem = zombieType;
					array[num7].mWeight = info.mWeights[num3];
					num7++;
				}
			}
			if (num7 != 0)
			{
				theZombieType = (ZombieType)TodCommon.TodPickFromWeightedArray(array, num7);
			}
			for (int j = 0; j < array.Length; j++)
			{
				array[j].PrepareForReuse();
			}
		}
		else if (num6 < span3[num3] && num4 < 3)
		{
			theZombieType = ZombieType.Pail;
		}
		else if (num6 < span3[num3] + span4[num3])
		{
			theZombieType = ZombieType.TrafficCone;
		}
		int num8 = 0;
		for (int k = 0; k < aGridPicks.Length; k++)
		{
			aGridPicks[k].Reset();
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Gravestone)
			{
				Plant topPlantAt = mBoard.GetTopPlantAt(theGridItem.mGridX, theGridItem.mGridY, TopPlant.OnlyNormalPosition);
				if (topPlantAt == null || topPlantAt.mSeedType != SeedType.Gravebuster)
				{
					aGridPickItemArray[num8] = theGridItem;
					aGridPicks[num8].mItem = num8;
					aGridPicks[num8].mWeight = 1;
					num8++;
				}
			}
		}
		float theMax = TodCommon.TodAnimateCurve(1, 12, mBoard.mCurrentWave, 1, 3, TodCurves.EaseIn);
		if (num4 > num8)
		{
			num4 = num8;
		}
		for (int l = 0; l < num4; l++)
		{
			int num9 = (int)TodCommon.TodPickFromWeightedArray(aGridPicks, num8);
			GridItem gridItem = aGridPickItemArray[num9];
			aGridPicks[num9].mWeight = 0;
			if (flag)
			{
				bool flag2 = false;
				if (cSWhackAZombieLevel != null)
				{
					List<ZombieType> list = new List<ZombieType>();
					foreach (KeyValuePair<ZombieType, CSWhackAZombieLevel.Info> mZombie2 in cSWhackAZombieLevel.mZombies)
					{
						mZombie2.Deconstruct(out key, out value);
						ZombieType item = key;
						CSWhackAZombieLevel.Info info2 = value;
						if (info2.mSpawnOnLastWave)
						{
							list.Add(item);
						}
					}
					if (list.Count != 0)
					{
						flag2 = true;
						theZombieType = list[RandomNumbers.NextNumber(list.Count)];
					}
				}
				if (!flag2)
				{
					ZombieType[] array2 = new ZombieType[2]
					{
						ZombieType.TrafficCone,
						ZombieType.Pail
					};
					theZombieType = array2[RandomNumbers.NextNumber(array2.Length)];
				}
				theMax = 2f;
			}
			Zombie zombie = mBoard.AddZombie(theZombieType, mBoard.mCurrentWave);
			if (zombie == null)
			{
				break;
			}
			zombie.RiseFromGrave(gridItem.mGridX, gridItem.mGridY);
			zombie.mPhaseCounter = 50;
			zombie.mVelX = TodCommon.RandRangeFloat(0.5f, theMax);
			zombie.UpdateAnimSpeed();
		}
		int theMin = TodCommon.TodAnimateCurve(1, 12, mBoard.mCurrentWave, 100, 30, TodCurves.Linear);
		int theMax2 = TodCommon.TodAnimateCurve(1, 12, mBoard.mCurrentWave, 200, 60, TodCurves.Linear);
		mChallengeStateCounter = TodCommon.RandRangeInt(theMin, theMax2);
		if (flag)
		{
			mBoard.mZombieCountDown = 0;
			mChallengeStateCounter = 0;
		}
	}

	public bool UpdateZombieSpawning()
	{
		if (mApp.IsWhackAZombieLevel())
		{
			WhackAZombieSpawning();
			return true;
		}
		if (!mApp.IsFinalBossLevel() && mApp.mGameMode != GameMode.ChallengeIce && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mApp.mGameMode != GameMode.ChallengeZombiquarium && !mApp.IsIZombieLevel() && !mApp.IsSquirrelLevel() && !mApp.IsScaryPotterLevel())
		{
			if (mApp.mGameMode == GameMode.ChallengeLastStand)
			{
				return mChallengeState != ChallengeState.LastStandOnslaught;
			}
			return false;
		}
		return true;
	}

	public void BeghouledClearCrater(int theCount)
	{
		mBoard.ClearAdvice(AdviceType.BeghouledUseCrater1);
		mBoard.ClearAdvice(AdviceType.BeghouledUseCrater2);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				if (mBeghouledEated[i, j])
				{
					mBeghouledEated[i, j] = false;
					theCount--;
					if (theCount == 0)
					{
						BeghouledUpdateCraters();
						return;
					}
				}
			}
		}
	}

	public void MouseDownWhackAZombie(int x, int y)
	{
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		Reanimation reanimation = mApp.ReanimationGet(mBoard.mCursorObject.mReanimCursorID);
		reanimation.mAnimTime = 0.2f;
		mApp.PlayFoley(FoleyType.Swing);
		Zombie zombie = null;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie2 = mBoard.mZombies[i];
			if (!zombie2.mDead && !zombie2.IsDeadOrDying())
			{
				TRect zombieRect = zombie2.GetZombieRect();
				zombieRect.mHeight -= 50;
				if (GameConstants.GetCircleRectOverlap(x, y - 20, 45, zombieRect) && (zombie == null || zombie2.mRenderOrder >= zombie.mRenderOrder))
				{
					zombie = zombie2;
				}
			}
		}
		if (zombie == null)
		{
			return;
		}
		if (zombie.mHelmType != HelmType.None)
		{
			if (zombie.mHelmType == HelmType.Pail)
			{
				mApp.PlayFoley(FoleyType.ShieldHit);
			}
			else if (zombie.mHelmType == HelmType.TrafficCone)
			{
				mApp.PlayFoley(FoleyType.PlasticHit);
			}
			zombie.TakeHelmDamage(900, 0u);
		}
		else
		{
			mApp.PlayFoley(FoleyType.Bonk);
			mApp.AddTodParticle(TodCommon.PixelAligned((float)x - 3f), TodCommon.PixelAligned((float)y + 9f), 800000, ParticleEffect.Pow);
			zombie.DieWithLoot();
			mBoard.ClearCursor();
		}
	}

	public bool IsStormyNightPitchBlack()
	{
		if (mBoard.HasStorm() && (mChallengeState != ChallengeState.StormFlash1 || mChallengeStateCounter >= 300) && (mChallengeState != ChallengeState.StormFlash2 || mChallengeStateCounter >= 300))
		{
			if (mChallengeState == ChallengeState.StormFlash3)
			{
				return mChallengeStateCounter >= 150;
			}
			return true;
		}
		return false;
	}

	public void DrawStormNight(Graphics g)
	{
		if (mChallengeState == ChallengeState.StormFlash1 && mChallengeStateCounter < 300)
		{
			if (mChallengeStateCounter > 150)
			{
				DrawStormFlash(g, mChallengeStateCounter - 150, 255);
			}
			else
			{
				DrawStormFlash(g, mChallengeStateCounter, 92);
			}
		}
		else if (mChallengeState == ChallengeState.StormFlash2 && mChallengeStateCounter < 300)
		{
			DrawStormFlash(g, mChallengeStateCounter / 2, 255);
		}
		else if (mChallengeState == ChallengeState.StormFlash3 && mChallengeStateCounter < 150)
		{
			DrawStormFlash(g, mChallengeStateCounter, 255);
		}
		else
		{
			g.SetColor(new SexyColor(0, 0, 0, 255));
			g.FillRect(-1000, -1000, Constants.BOARD_WIDTH + 2000, Constants.BOARD_HEIGHT + 2000);
		}
		mBoard.DrawUIBottom(g);
		mBoard.DrawTopRightUI(g);
	}

	public void UpdateStormyNight()
	{
		if (mBoard.mPaused)
		{
			if (mChallengeStateCounter == 1)
			{
				return;
			}
			if (mChallengeStateCounter == 150 && mChallengeState == ChallengeState.StormFlash1)
			{
				mChallengeStateCounter = 1;
				return;
			}
		}
		mChallengeStateCounter--;
		if ((mChallengeStateCounter == 300 && mChallengeState == ChallengeState.StormFlash1) || (mChallengeStateCounter == 150 && mChallengeState == ChallengeState.StormFlash1) || (mChallengeStateCounter == 300 && mChallengeState == ChallengeState.StormFlash2) || (mChallengeStateCounter == 150 && mChallengeState == ChallengeState.StormFlash3))
		{
			mApp.PlayFoley(FoleyType.Thunder);
		}
		if (mChallengeStateCounter > 0)
		{
			return;
		}
		if (mApp.mGameScene == GameScenes.ZombiesWon)
		{
			mChallengeStateCounter = 150 + TodCommon.RandRangeInt(-50, 50);
			mChallengeState = ChallengeState.StormFlash3;
			return;
		}
		if (mApp.mGameScene != GameScenes.Playing)
		{
			mChallengeStateCounter = 0;
			mChallengeState = ChallengeState.Normal;
			return;
		}
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && zombie.mZombieType == ZombieType.Yeti)
			{
				mChallengeStateCounter = 150 + TodCommon.RandRangeInt(200, 300);
				mChallengeState = (ChallengeState)TodCommon.RandRangeInt(5, 7);
				return;
			}
		}
		int theMax = ((RandomNumbers.NextNumber(2) != 0) ? 400 : 750);
		mChallengeStateCounter = 150 + TodCommon.RandRangeInt(300, theMax);
		mChallengeState = (ChallengeState)TodCommon.RandRangeInt(5, 7);
	}

	public void InitLevel()
	{
		if (mApp.mGameMode == GameMode.ChallengeRainingSeeds)
		{
			mChallengeStateCounter = 100;
			mApp.PlayFoley(FoleyType.Rain);
		}
		else if (mBoard.HasStorm())
		{
			mChallengeState = ChallengeState.StormFlash2;
			mChallengeStateCounter = 150;
			mApp.PlayFoley(FoleyType.Rain);
		}
		else if (mBoard.HasRain())
		{
			mApp.PlayFoley(FoleyType.Rain);
		}
		if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
		{
			mChallengeStateCounter = 100;
		}
		if (mApp.IsFinalBossLevel() && mApp.mGameMode != GameMode.ChallengeFinalBoss2)
		{
			mBoard.mSeedBank.AddSeed(SeedType.Cabbagepult);
			mBoard.mSeedBank.AddSeed(SeedType.Jalapeno);
			mBoard.mSeedBank.AddSeed(SeedType.Cabbagepult);
			mBoard.mSeedBank.AddSeed(SeedType.Iceshroom);
			mConveyorBeltCounter = 1000;
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mApp.mZenGarden.mGardenType = GardenType.Main;
			mApp.mZenGarden.ZenGardenInitLevel(theJustSwitchingGardens: false);
		}
		if (mBoard.HasConveyorBeltSeedBank() && mApp.IsLevelUseJson())
		{
			CSConveyorBelt component = mApp.mCreativeLevel.GetComponent<CSConveyorBelt>();
			if (component?.mInitCards != null)
			{
				for (int i = 0; i < component.mInitCards.Count; i++)
				{
					mBoard.mSeedBank.AddSeed(component.mInitCards[i].mSeedType, component.mInitCards[i].mImitator);
				}
				mConveyorBeltCounter = 1000;
			}
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn)
		{
			mBoard.mSeedBank.AddSeed(SeedType.Potatomine);
			mBoard.mSeedBank.AddSeed(SeedType.Tallnut);
			mBoard.mSeedBank.AddSeed(SeedType.Melonpult);
			mBoard.mSeedBank.AddSeed(SeedType.Magnetshroom);
			mBoard.mSeedBank.AddSeed(SeedType.InstantCoffee);
			mBoard.mSeedBank.AddSeed(SeedType.Melonpult);
			mConveyorBeltCounter = 1000;
		}
		if (mApp.mGameMode == GameMode.ChallengeColumn2)
		{
			mBoard.mSeedBank.AddSeed(SeedType.Agave);
			mBoard.mSeedBank.AddSeed(SeedType.Pumpkinshell);
			mBoard.mSeedBank.AddSeed(SeedType.Spikeweed);
			mBoard.mSeedBank.AddSeed(SeedType.Spikerock);
			mBoard.mSeedBank.AddSeed(SeedType.Repeater);
			mBoard.mSeedBank.AddSeed(SeedType.InstantCoffee);
			mBoard.mSeedBank.AddSeed(SeedType.Iceshroom);
			mConveyorBeltCounter = 1000;
		}
		if (mApp.mGameMode == GameMode.ChallengeColumn3)
		{
			mBoard.mSeedBank.AddSeed(SeedType.Chomper);
			mBoard.mSeedBank.AddSeed(SeedType.Gravebuster);
			mBoard.mSeedBank.AddSeed(SeedType.Fumeshroom);
			mBoard.mSeedBank.AddSeed(SeedType.Gravebuster);
			mBoard.mSeedBank.AddSeed(SeedType.FireShroom);
			mBoard.mSeedBank.AddSeed(SeedType.Gravebuster);
			mBoard.mSeedBank.AddSeed(SeedType.SuperChomper);
			mBoard.mSeedBank.AddSeed(SeedType.Iceshroom);
			mBoard.mSeedBank.AddSeed(SeedType.Splitpea);
			mBoard.mSeedBank.AddSeed(SeedType.Gravebuster);
			mConveyorBeltCounter = 1000;
		}
		if (mApp.mGameMode == GameMode.OccupyHighGround)
		{
			mBoard.mSeedBank.AddSeed(SeedType.Iceshroom);
			mBoard.mSeedBank.AddSeed(SeedType.Doomshroom);
			mConveyorBeltCounter = 1000;
		}
		if (mApp.mGameMode == GameMode.ChallengeInvisighoul)
		{
			mBoard.mSeedBank.AddSeed(SeedType.Peashooter);
			mBoard.mSeedBank.AddSeed(SeedType.Iceshroom);
			mConveyorBeltCounter = 1000;
		}
		if (mApp.IsIZombieLevel())
		{
			IZombieInitLevel();
		}
		if (mApp.IsScaryPotterLevel())
		{
			ScaryPotterPopulate();
		}
		if (mApp.IsFirstTimeAdventureMode() && mBoard.mLevel == 5)
		{
			mBoard.NewPlant(5, 1, SeedType.Peashooter, SeedType.None);
			mBoard.NewPlant(7, 2, SeedType.Peashooter, SeedType.None);
			mBoard.NewPlant(6, 3, SeedType.Peashooter, SeedType.None);
		}
		if (mApp.mGameMode == GameMode.ChallengeFusion)
		{
			mBoard.NewPlant(0, 0, SeedType.Endoflame, SeedType.None);
			mBoard.NewPlant(0, 5, SeedType.Endoflame, SeedType.None);
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			mChallengeGridX = -1;
			mChallengeGridY = -1;
		}
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			TreeOfWisdomInit();
		}
	}

	public void SpawnZombieWave()
	{
		if (mApp.IsContinuousChallenge() && mBoard.mCurrentWave == mBoard.mNumWaves)
		{
			mBoard.mCurrentWave = mBoard.mNumWaves - 1;
			for (int i = 0; i < 50; i++)
			{
				switch (mBoard.mZombiesInWave[mBoard.mCurrentWave, i])
				{
				case ZombieType.Flag:
					mBoard.mZombiesInWave[mBoard.mCurrentWave, i] = ZombieType.Normal;
					continue;
				default:
					continue;
				case ZombieType.Invalid:
					break;
				}
				break;
			}
		}
		bool flag = mBoard.IsFlagWave(mBoard.mCurrentWave);
		if (mApp.mGameMode == GameMode.ChallengeGraveDanger && mBoard.mCurrentWave != mBoard.mNumWaves - 1)
		{
			if (flag)
			{
				mBoard.SpawnZombiesFromGraves();
			}
			else if (mBoard.mCurrentWave > 5)
			{
				GraveDangerSpawnRandomGrave();
			}
		}
		if (mApp.IsSurvivalMode() && mBoard.mBackground == BackgroundType.Num2Night && mBoard.mCurrentWave == mBoard.mNumWaves - 1)
		{
			int graveStoneCount = mBoard.GetGraveStoneCount();
			if (mApp.IsSurvivalNormal(mApp.mGameMode))
			{
				if (graveStoneCount < 8)
				{
					GraveDangerSpawnRandomGrave();
				}
			}
			else if (graveStoneCount < 12)
			{
				GraveDangerSpawnRandomGrave();
			}
		}
		if ((mApp.IsBungeeBlitzLevel() || ((mApp.mCreativeLevel?.GetComponent<CSBungeeBlitz>())?.mShowHugeFlagAdvice ?? false)) & flag)
		{
			mBoard.DisplayAdvice("[ADVICE_BUNGEES_INCOMING]", MessageStyle.HintFast, AdviceType.None);
		}
	}

	public void GraveDangerSpawnRandomGrave()
	{
		TodWeightedGridArray[] array = new TodWeightedGridArray[Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
		int num = 0;
		for (int j = 4; j < Constants.GRIDSIZEX; j++)
		{
			for (int k = 0; k < Constants.MAX_GRIDSIZEY; k++)
			{
				if (mBoard.CanAddGraveStoneAt(j, k))
				{
					Plant topPlantAt = mBoard.GetTopPlantAt(j, k, TopPlant.Any);
					if (topPlantAt != null)
					{
						array[num].mWeight = 1;
					}
					else
					{
						array[num].mWeight = 100000;
					}
					array[num].mX = j;
					array[num].mY = k;
					num++;
				}
			}
		}
		if (num != 0)
		{
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array, num);
			GraveDangerSpawnGraveAt(todWeightedGridArray.mX, todWeightedGridArray.mY);
		}
	}

	public void GraveDangerSpawnGraveAt(int x, int y)
	{
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mPlantCol == x && plant.mRow == y)
			{
				plant.Die();
			}
		}
		mBoard.mEnableGraveStones = true;
		mBoard.AddAGraveStone(x, y)?.AddGraveStoneParticles();
	}

	public void SpawnLevelAward(int theGridX, int theGridY)
	{
		if (mBoard.HasLevelAwardDropped())
		{
			return;
		}
		float num = mBoard.GridToPixelX(theGridX, theGridY) + 40;
		float num2 = mBoard.GridToPixelY(theGridX, theGridY) + 40;
		CoinType theCoinType;
		if (mApp.IsAdventureMode() && mApp.IsFirstTimeAdventureMode())
		{
			theCoinType = CoinType.FinalSeedPacket;
		}
		else if (mApp.IsAdventureMode() || mApp.HasBeatenChallenge(mApp.mGameMode) || mApp.IsDIYMode())
		{
			theCoinType = CoinType.AwardMoneyBag;
		}
		else
		{
			theCoinType = ((!mApp.AboutToEarnGoldSunflower()) ? CoinType.Trophy : CoinType.AwardGoldSunflower);
		}
		mBoard.mLevelAwardSpawned = true;
		mApp.mBoardResult = BoardResult.Won;
		mApp.PlayFoley(FoleyType.SpawnSun);
		Coin coin = mBoard.AddCoin((int)num, (int)num2, theCoinType, CoinMotion.Coin);
		mApp.AddTodParticle(400f, 300f, 400000, ParticleEffect.ScreenFlash);
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			coin.Collect();
		}
		else
		{
			if (mApp.IsIZombieLevel())
			{
				return;
			}
			int count = mBoard.mZombies.Count;
			for (int i = 0; i < count; i++)
			{
				Zombie zombie = mBoard.mZombies[i];
				if (!zombie.mDead && !zombie.IsDeadOrDying())
				{
					zombie.TakeDamage(1800, 0u);
				}
			}
		}
	}

	public void BeghouledScore(int x, int y, int theNumPlants, bool theIsHorizontal)
	{
		mApp.PlayFoley(FoleyType.ArtChallenge);
		float num = mBoard.GridToPixelX(x, y);
		float num2 = mBoard.GridToPixelY(x, y);
		if (!theIsHorizontal)
		{
			num2 = theNumPlants switch
			{
				3 => num2 + 80f, 
				4 => num2 + 120f, 
				_ => num2 + 160f, 
			};
		}
		else
		{
			num = theNumPlants switch
			{
				3 => num + 80f, 
				4 => num + 120f, 
				_ => num + 160f, 
			};
		}
		mChallengeScore++;
		if (mBoard.mSeedBank.mNumPackets == 0)
		{
			mBoard.mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Repeater, SeedType.None);
			mBoard.mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Fumeshroom, SeedType.None);
			mBoard.mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Tallnut, SeedType.None);
			mBoard.mSeedBank.mSeedPackets[3].SetPacketType(SeedType.BeghouledButtonShuffle, SeedType.None);
			mBoard.mSeedBank.mNumPackets = 4;
			mBoard.DisplayAdvice("[ADVICE_BEGHOULED_SAVE_SUN]", MessageStyle.HintFast, AdviceType.BeghouledSaveSun);
			if (BeghouledCanClearCrater())
			{
				mBoard.mSeedBank.mSeedPackets[4].SetPacketType(SeedType.BeghouledButtonCrater, SeedType.None);
				mBoard.mSeedBank.mNumPackets = 5;
			}
		}
		else
		{
			if (!mBoard.mAdvice.IsBeingDisplayed())
			{
				string theAdvice = TodCommon.TodReplaceNumberString("[ADVICE_BEGHOULED_MATCH_3]", "{SCORE}", 75);
				mBoard.DisplayAdvice(theAdvice, MessageStyle.HintFast, AdviceType.BeghouledMatch3);
			}
			if (mChallengeScore >= 70)
			{
				mBoard.DisplayAdvice("[ADVICE_BEGHOULED_ALMOST_THERE]", MessageStyle.HintFast, AdviceType.AlmostThere);
			}
		}
		if (mChallengeScore >= 75)
		{
			mChallengeScore = 75;
			SpawnLevelAward(x, y);
			mBoard.ClearAdvice(AdviceType.None);
		}
		else
		{
			int num3 = theNumPlants - 2 + mBeghouledMatchesThisMove;
			if (theNumPlants >= 5)
			{
				num3 += 2;
			}
			num3 = TodCommon.ClampInt(num3, 1, 5);
			for (int i = 0; i < num3; i++)
			{
				mBoard.AddCoin((int)(num - 10f + 20f * (float)i), (int)num2, CoinType.Sun, CoinMotion.Coin);
			}
		}
		mBeghouledMatchesThisMove++;
	}

	public void DrawStormFlash(Graphics g, int theTime, int theMaxAmount)
	{
		RandomNumbers.Seed(mBoard.mMainCounter / 6);
		int num = TodCommon.TodAnimateCurve(150, 0, theTime, 255 - theMaxAmount, 255, TodCurves.Linear);
		int theAlpha = TodCommon.ClampInt((int)((float)num + RandomNumbers.NextNumber(64f) - 32f), 0, 255);
		g.SetColor(new SexyColor(0, 0, 0, theAlpha));
		g.FillRect(-1000, -1000, Constants.BOARD_WIDTH + 2000, Constants.BOARD_HEIGHT + 2000);
		int theAlpha2 = TodCommon.TodAnimateCurve(150, 75, theTime, theMaxAmount, 0, TodCurves.Linear);
		g.SetColor(new SexyColor(255, 255, 255, theAlpha2));
		g.FillRect(-1000, -1000, Constants.BOARD_WIDTH + 2000, Constants.BOARD_HEIGHT + 2000);
	}

	public void UpdateRainingSeeds()
	{
		if (mBoard.HasLevelAwardDropped())
		{
			return;
		}
		mChallengeStateCounter--;
		if (mChallengeStateCounter != 0)
		{
			return;
		}
		int num = 500;
		if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
		{
			num = 1500;
		}
		mChallengeStateCounter = num + RandomNumbers.NextNumber(500);
		int theX = 100 + RandomNumbers.NextNumber(550);
		Coin coin = mBoard.AddCoin(theX, 60, CoinType.UsableSeedPacket, CoinMotion.FromSkySlow);
		CSRainingSeeds cSRainingSeeds = mApp.mCreativeLevel?.GetComponent<CSRainingSeeds>();
		if (cSRainingSeeds != null)
		{
			mChallengeStateCounter = TodCommon.RandRangeInt(cSRainingSeeds.mMinPrepareTime, cSRainingSeeds.mMaxPrepareTime);
			TodWeightedArray[] array = new TodWeightedArray[cSRainingSeeds.mAllowedPlants.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = TodWeightedArray.GetNewTodWeightedArray();
				array[i].mItem = (int)cSRainingSeeds.mAllowedPlants[i].Item1;
				array[i].mWeight = cSRainingSeeds.mAllowedPlants[i].Item2;
			}
			coin.mUsableSeedType = (SeedType)TodCommon.TodPickFromWeightedArray(array, array.Length);
			for (int j = 0; j < array.Length; j++)
			{
				array[j].PrepareForReuse();
			}
			return;
		}
		SeedType[] array2 = null;
		if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
		{
			array2 = GameConstants.gFinalBoss2RainingSeeds;
		}
		if (array2 == null || array2.Length < 1)
		{
			do
			{
				coin.mUsableSeedType = (SeedType)RandomNumbers.NextNumber(mApp.GetSeedsAvailable());
			}
			while (mBoard.SeedNotRecommendedForLevel(coin.mUsableSeedType) != 0 || !mApp.HasSeedType(coin.mUsableSeedType) || Plant.IsUpgrade(coin.mUsableSeedType) || coin.mUsableSeedType == SeedType.Sunflower || coin.mUsableSeedType == SeedType.Twinsunflower || coin.mUsableSeedType == SeedType.InstantCoffee || coin.mUsableSeedType == SeedType.Umbrella || coin.mUsableSeedType == SeedType.Sunshroom || coin.mUsableSeedType == SeedType.Imitater);
			int theTimeAge = mBoard.CountPlantByType(SeedType.Lilypad);
			int theTimeEnd = 18;
			int thePositionStart = 30;
			int num2 = TodCommon.TodAnimateCurve(0, theTimeEnd, theTimeAge, thePositionStart, 1, TodCurves.Linear);
			if (RandomNumbers.NextNumber(100) < num2)
			{
				coin.mUsableSeedType = SeedType.Lilypad;
			}
		}
		else
		{
			coin.mUsableSeedType = array2[RandomNumbers.NextNumber(array2.Length)];
		}
	}

	public void PlayBossEnter()
	{
		mBoard.AddZombie(ZombieType.Boss, 0);
	}

	public void UpdateConveyorBelt()
	{
		if (mBoard.HasLevelAwardDropped())
		{
			return;
		}
		mBoard.mSeedBank.UpdateConveyorBelt();
		mConveyorBeltCounter--;
		if (mConveyorBeltCounter > 0)
		{
			return;
		}
		float num = 1f;
		if (mApp.IsLevelUseJson())
		{
			num = mApp.mCreativeLevel.GetComponent<CSConveyorBelt>().mPrepareTimeMultiplier;
		}
		else if (mApp.IsFinalBossLevel())
		{
			num = 0.875f;
		}
		else if (mApp.IsShovelLevel() || mApp.mGameMode == GameMode.ChallengePortalCombat)
		{
			num = 1.5f;
		}
		else if (mApp.mGameMode == GameMode.ChallengeInvisighoul)
		{
			num = 2f;
		}
		else if (mApp.IsColumnLevel())
		{
			if (mApp.mGameMode != GameMode.ChallengeColumn3)
			{
				num = 3f;
			}
			else if (mBoard.mCurrentWave != 23 && mBoard.mCurrentWave != 25)
			{
				num = ((mBoard.mCurrentWave < 20) ? 3f : 2.5f);
			}
			else
			{
				num = 1.5f;
			}
		}
		else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans)
		{
			num = 1f;
		}
		else if (mApp.mGameMode == GameMode.RogueConveyorbelt || mApp.mGameMode == GameMode.RogueConveyorbeltHard)
		{
			num = 1f;
		}
		if (mBoard.mSeedBank.GetNumSeedsOnConveyorBelt() > 8)
		{
			mConveyorBeltCounter = 1000 * (int)num;
		}
		else if (mBoard.mSeedBank.GetNumSeedsOnConveyorBelt() > 6)
		{
			mConveyorBeltCounter = 500 * (int)num;
		}
		else if (mBoard.mSeedBank.GetNumSeedsOnConveyorBelt() > 4)
		{
			mConveyorBeltCounter = 425 * (int)num;
		}
		else
		{
			mConveyorBeltCounter = 400 * (int)num;
		}
		if (mApp.IsAdventureModeHard())
		{
			mConveyorBeltCounter = (int)((float)mConveyorBeltCounter * 0.8f);
		}
		if (mBoard.mSeedBank.mConveyorBeltBuffer.Count > 0)
		{
			if (mBoard.mSeedBank.GetNumSeedsOnConveyorBelt() == mBoard.mSeedBank.mNumPackets)
			{
				mConveyorBeltCounter = 300;
				return;
			}
			CardPair cardPair = mBoard.mSeedBank.mConveyorBeltBuffer[0];
			mBoard.mSeedBank.mConveyorBeltBuffer.RemoveAt(0);
			mBoard.mSeedBank.AddSeed(cardPair.mSeedType, cardPair.mImitator);
			mLastConveyorSeedType = cardPair.mSeedType;
			mConveyorBeltCounter = 300;
			return;
		}
		for (int i = 0; i < 200; i++)
		{
			aSeedPickArray[i].Reset();
		}
		int num2 = 0;
		if (mApp.IsLevelUseJson())
		{
			CSConveyorBelt component = mApp.mCreativeLevel.GetComponent<CSConveyorBelt>();
			if (component != null)
			{
				for (int j = 0; j < component.mAllowedPlants.Count && j < aSeedPickArray.Length; j++)
				{
					aSeedPickArray[num2].mItem = component.mAllowedPlants[j].Item1;
					aSeedPickArray[num2].mWeight = component.mAllowedPlants[j].Item2;
					num2++;
				}
			}
		}
		else if (mBoard.mLevel == 10)
		{
			aSeedPickArray[num2].mItem = SeedType.Peashooter;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Wallnut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Repeater;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Snowpea;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Chomper;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Potatomine;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mBoard.mLevel == 20)
		{
			aSeedPickArray[num2].mItem = SeedType.Gravebuster;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Iceshroom;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Doomshroom;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Hypnoshroom;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Scaredyshroom;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Fumeshroom;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Puffshroom;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mBoard.mLevel == 30)
		{
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Squash;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Threepeater;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Tanglekelp;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Jalapeno;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Spikeweed;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Torchwood;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Tallnut;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mBoard.mLevel == 40)
		{
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Seashroom;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Magnetshroom;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Blover;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cactus;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Starfruit;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Splitpea;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Pumpkinshell;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mApp.IsFinalBossLevel())
		{
			if (mApp.IsAdventureModeHard())
			{
				aSeedPickArray[num2].mItem = SeedType.Flowerpot;
				aSeedPickArray[num2].mWeight = 55;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Melonpult;
				aSeedPickArray[num2].mWeight = 10;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Jalapeno;
				aSeedPickArray[num2].mWeight = 12;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Cabbagepult;
				aSeedPickArray[num2].mWeight = 3;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Kernelpult;
				aSeedPickArray[num2].mWeight = 5;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Iceshroom;
				aSeedPickArray[num2].mWeight = 8;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Cactus;
				aSeedPickArray[num2].mWeight = 8;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Magnetshroom;
				aSeedPickArray[num2].mWeight = 3;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Plantern;
				aSeedPickArray[num2].mWeight = 1;
				num2++;
			}
			else
			{
				aSeedPickArray[num2].mItem = SeedType.Flowerpot;
				aSeedPickArray[num2].mWeight = 55;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Melonpult;
				aSeedPickArray[num2].mWeight = 10;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Jalapeno;
				aSeedPickArray[num2].mWeight = 12;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Cabbagepult;
				aSeedPickArray[num2].mWeight = 10;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Kernelpult;
				aSeedPickArray[num2].mWeight = 5;
				num2++;
				aSeedPickArray[num2].mItem = SeedType.Iceshroom;
				aSeedPickArray[num2].mWeight = 8;
				num2++;
			}
		}
		else if (mApp.IsShovelLevel())
		{
			aSeedPickArray[num2].mItem = SeedType.Peashooter;
			aSeedPickArray[num2].mWeight = 100;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.ChallengeWallnutBowling2)
		{
			aSeedPickArray[num2].mItem = SeedType.Wallnut;
			aSeedPickArray[num2].mWeight = 85;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.ExplodeONut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.GiantWallnut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
		}
		else if (mApp.IsWallnutBowlingLevel())
		{
			aSeedPickArray[num2].mItem = SeedType.Wallnut;
			aSeedPickArray[num2].mWeight = 85;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.ExplodeONut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
		}
		else if (mApp.IsLittleTroubleLevel())
		{
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Wallnut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Peashooter;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2].mWeight = 35;
			num2++;
		}
		else if (mApp.IsStormyNightLevel())
		{
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2].mWeight = 30;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cactus;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Peashooter;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Puffshroom;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
		}
		else if (mApp.IsBungeeBlitzLevel())
		{
			if (mApp.IsAdventureModeHard() && mApp.mPlayerInfo.mLevel == 45)
			{
				aSeedPickArray[num2].mItem = SeedType.SuperChomper;
				aSeedPickArray[num2].mWeight = 12;
				num2++;
			}
			aSeedPickArray[num2].mItem = SeedType.Flowerpot;
			aSeedPickArray[num2].mWeight = 50;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Chomper;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Pumpkinshell;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.ChallengePortalCombat)
		{
			aSeedPickArray[num2].mItem = SeedType.Peashooter;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Repeater;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Torchwood;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cactus;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Wallnut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn)
		{
			aSeedPickArray[num2].mItem = SeedType.Flowerpot;
			aSeedPickArray[num2].mWeight = 155;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Melonpult;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Chomper;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Pumpkinshell;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Jalapeno;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Squash;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn2)
		{
			aSeedPickArray[num2].mItem = SeedType.Agave;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Spikeweed;
			aSeedPickArray[num2].mWeight = 20;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Spikerock;
			aSeedPickArray[num2].mWeight = 1;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Pumpkinshell;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Squash;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.ChallengeColumn3)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = mBoard.CountPlantByType(SeedType.Chomper);
			int num9 = mBoard.CountPlantByType(SeedType.SuperChomper);
			int num10 = mBoard.CountPlantByType(SeedType.Fumeshroom);
			int num11 = mBoard.CountPlantByType(SeedType.Gravebuster);
			int num12 = mBoard.CountPlantByType(SeedType.FireShroom);
			int num13 = mBoard.CountPlantByType(SeedType.Splitpea);
			int num14 = mBoard.CountPlantByType(SeedType.Repeater);
			int num15 = mBoard.mPlants.size();
			int graveStoneCount = mBoard.GetGraveStoneCount();
			for (int k = 0; k < mBoard.mSeedBank.mNumPackets; k++)
			{
				SeedPacket seedPacket = mBoard.mSeedBank.mSeedPackets[k];
				if (seedPacket.mPacketType == SeedType.Gravebuster)
				{
					flag = true;
				}
				else if (seedPacket.mPacketType == SeedType.Doomshroom)
				{
					flag2 = true;
				}
				else if (seedPacket.mPacketType == SeedType.Chomper)
				{
					flag3 = true;
				}
				else if (seedPacket.mPacketType == SeedType.SuperChomper)
				{
					flag4 = true;
				}
				else if (seedPacket.mPacketType == SeedType.Splitpea)
				{
					flag5 = true;
				}
				else if (seedPacket.mPacketType == SeedType.Fumeshroom)
				{
					num3++;
				}
				else if (seedPacket.mPacketType == SeedType.FireShroom)
				{
					num4++;
				}
				else if (seedPacket.mPacketType == SeedType.Squash)
				{
					num5++;
				}
				else if (seedPacket.mPacketType == SeedType.Iceshroom)
				{
					num6++;
				}
				else if (seedPacket.mPacketType == SeedType.Repeater)
				{
					num7++;
				}
			}
			num14 += num7 * 5;
			if (num14 < 10)
			{
				aSeedPickArray[num2].mItem = SeedType.Repeater;
				aSeedPickArray[num2].mWeight = 60;
				num2++;
			}
			if (!flag5 && num13 < 5)
			{
				int mWeight = 0;
				for (int l = 0; l < mBoard.mZombies.Count; l++)
				{
					Zombie zombie = mBoard.mZombies[l];
					if (zombie.mZombieType == ZombieType.Digger)
					{
						mWeight = 50;
						break;
					}
				}
				aSeedPickArray[num2].mItem = SeedType.Splitpea;
				aSeedPickArray[num2].mWeight = mWeight;
				num2++;
			}
			if (num12 <= 15)
			{
				aSeedPickArray[num2].mItem = SeedType.FireShroom;
				aSeedPickArray[num2].mWeight = ((num4 <= 3) ? (100 / (num4 + 1)) : 0);
				num2++;
			}
			aSeedPickArray[num2].mItem = SeedType.Doomshroom;
			aSeedPickArray[num2].mWeight = ((!flag2) ? 5 : 0);
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Iceshroom;
			aSeedPickArray[num2].mWeight = ((num6 < 2) ? (30 / (num6 + 1)) : 0);
			num2++;
			if (num3 < 2)
			{
				aSeedPickArray[num2].mItem = SeedType.Fumeshroom;
				aSeedPickArray[num2].mWeight = ((num10 >= 5 || num3 == 1) ? 5 : 60);
				num2++;
			}
			aSeedPickArray[num2].mItem = SeedType.Squash;
			aSeedPickArray[num2].mWeight = ((num4 < 2) ? (25 / (num5 + 1)) : 0);
			num2++;
			if (mBoard.mCurrentWave >= 20 && num9 < 6)
			{
				if (!flag3 && num8 <= 0)
				{
					aSeedPickArray[num2].mItem = SeedType.Chomper;
					aSeedPickArray[num2].mWeight = 120;
					num2++;
				}
				else if ((flag3 && !flag4) || (!flag4 && num8 > 0))
				{
					aSeedPickArray[num2].mItem = SeedType.SuperChomper;
					aSeedPickArray[num2].mWeight = 120;
					num2++;
				}
			}
			if (!((graveStoneCount <= 0 || num11 > 0) | flag))
			{
				float num16 = 0f;
				for (int m = 0; m <= num2; m++)
				{
					num16 += (float)aSeedPickArray[m].mWeight;
				}
				bool flag6 = false;
				if (graveStoneCount > 10)
				{
					flag6 = num15 >= 45 - graveStoneCount - 3;
				}
				if (flag6)
				{
					num16 = 0.8f * (num16 + 1f) / 0.19999999f;
				}
				else
				{
					num16 = (num16 = 0.65f * (num16 + 1f) / 0.35000002f);
					num16 *= (float)graveStoneCount;
					num16 /= 40f;
				}
				aSeedPickArray[num2].mItem = SeedType.Gravebuster;
				aSeedPickArray[num2].mWeight = (int)num16;
				num2++;
			}
		}
		else if (mApp.mGameMode == GameMode.ChallengeInvisighoul)
		{
			aSeedPickArray[num2].mItem = SeedType.Peashooter;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Wallnut;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Kernelpult;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Squash;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2].mWeight = 30;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Iceshroom;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans)
		{
			aSeedPickArray[num2].mItem = SeedType.Scaredyshroom;
			aSeedPickArray[num2++].mWeight = 25;
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2++].mWeight = 30;
			aSeedPickArray[num2].mItem = SeedType.Plantern;
			aSeedPickArray[num2++].mWeight = 10;
			aSeedPickArray[num2].mItem = SeedType.Magnetshroom;
			aSeedPickArray[num2++].mWeight = 10;
			aSeedPickArray[num2].mItem = SeedType.Kernelpult;
			aSeedPickArray[num2++].mWeight = 25;
			aSeedPickArray[num2].mItem = SeedType.Repeater;
			aSeedPickArray[num2++].mWeight = 30;
			aSeedPickArray[num2].mItem = SeedType.Cattail;
			aSeedPickArray[num2++].mWeight = 5;
			aSeedPickArray[num2].mItem = SeedType.Iceshroom;
			aSeedPickArray[num2++].mWeight = 8;
			aSeedPickArray[num2].mItem = SeedType.Jalapeno;
			aSeedPickArray[num2++].mWeight = 10;
		}
		else if (mApp.mGameMode == GameMode.ChallengeMoreAirRaid)
		{
			aSeedPickArray[num2].mItem = SeedType.Flowerpot;
			aSeedPickArray[num2++].mWeight = 70;
			aSeedPickArray[num2].mItem = SeedType.Cactus;
			aSeedPickArray[num2++].mWeight = 25;
			aSeedPickArray[num2].mItem = SeedType.Tallnut;
			aSeedPickArray[num2++].mWeight = 10;
			aSeedPickArray[num2].mItem = SeedType.Pumpkinshell;
			aSeedPickArray[num2++].mWeight = 15;
			aSeedPickArray[num2].mItem = SeedType.Melonpult;
			aSeedPickArray[num2++].mWeight = 15;
			aSeedPickArray[num2].mItem = SeedType.Squash;
			aSeedPickArray[num2++].mWeight = 12;
			aSeedPickArray[num2].mItem = SeedType.Blover;
			aSeedPickArray[num2++].mWeight = 18;
			aSeedPickArray[num2].mItem = SeedType.Umbrella;
			aSeedPickArray[num2++].mWeight = 8;
			aSeedPickArray[num2].mItem = SeedType.Cherrybomb;
			aSeedPickArray[num2++].mWeight = 10;
		}
		else if (mApp.mGameMode == GameMode.OccupyHighGround)
		{
			aSeedPickArray[num2].mItem = SeedType.Puffshroom;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Fumeshroom;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Iceshroom;
			aSeedPickArray[num2].mWeight = 8;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Doomshroom;
			aSeedPickArray[num2].mWeight = 8;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Kernelpult;
			aSeedPickArray[num2].mWeight = 15;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.FireShroom;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Cactus;
			aSeedPickArray[num2].mWeight = 25;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Garlic;
			aSeedPickArray[num2].mWeight = 8;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Starfruit;
			aSeedPickArray[num2].mWeight = 10;
			num2++;
			aSeedPickArray[num2].mItem = SeedType.Repeater;
			aSeedPickArray[num2].mWeight = 5;
			num2++;
		}
		else if (mApp.mGameMode == GameMode.RogueConveyorbelt || mApp.mGameMode == GameMode.RogueConveyorbeltHard)
		{
			bool doCob = mApp.mGameMode != GameMode.RogueConveyorbelt;
			bool flag7 = true;
			if (mBoard.mBackground == BackgroundType.Num3Pool || mBoard.mBackground == BackgroundType.Num1Day || mBoard.mBackground == BackgroundType.Num5Roof)
			{
				flag7 = false;
			}
			aSeedPickArray[num2].mItem = SeedType.Lilypad;
			aSeedPickArray[num2++].mWeight = (flag7 ? 40 : 20);
			aSeedPickArray[num2].mItem = SeedType.Pumpkinshell;
			aSeedPickArray[num2++].mWeight = (flag7 ? 14 : 7);
			if (mSurvivalStage > 0)
			{
				SeedType s = SearchForSunCost(0, 9999, includeUpgrades: true, includeProductionPlants: false, flag7, doCob);
				if (!Plant.IsUpgrade(s) || mBoard.mPlants.Find((Plant p) => p.IsUpgradableTo(s)) != null)
				{
					aSeedPickArray[num2].mItem = s;
					aSeedPickArray[num2++].mWeight = 100;
				}
				else
				{
					aSeedPickArray[num2].mItem = SearchForSunCost(0, 9999, includeUpgrades: false, includeProductionPlants: false, flag7, doCob);
					aSeedPickArray[num2++].mWeight = 100;
				}
				SeedType theSeedtype = SearchForSunCost(0, 9999, includeUpgrades: true, includeProductionPlants: false, includeNocturnal: false, doCob);
				if (!Plant.IsUpgrade(theSeedtype) || mBoard.mPlants.Find((Plant p) => p.IsUpgradableTo(s)) != null)
				{
					aSeedPickArray[num2].mItem = s;
					aSeedPickArray[num2++].mWeight = 100;
				}
				else
				{
					aSeedPickArray[num2].mItem = SearchForSunCost(0, 9999, includeUpgrades: false, includeProductionPlants: false, includeNocturnal: false, doCob);
					aSeedPickArray[num2++].mWeight = 100;
				}
			}
			else
			{
				aSeedPickArray[num2].mItem = SearchForSunCost(0, 9999, includeUpgrades: false, includeProductionPlants: false, flag7, doCob);
				aSeedPickArray[num2++].mWeight = 100;
			}
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		Debug.ASSERT(num2 <= 20);
		SeedType seedType;
		for (int num17 = 0; num17 < num2; num17++)
		{
			TodWeightedArray todWeightedArray = aSeedPickArray[num17];
			seedType = ((todWeightedArray.mItem is CardPair cardPair2) ? cardPair2.mSeedType : ((SeedType)todWeightedArray.mItem));
			int num18 = mBoard.mSeedBank.CountOfTypeOnConveyorBelt(seedType);
			if (seedType == SeedType.Gravebuster)
			{
				int graveStoneCount2 = mBoard.GetGraveStoneCount();
				int num19 = mBoard.CountPlantByType(seedType);
				if (graveStoneCount2 <= num19 + num18)
				{
					todWeightedArray.mWeight = 0;
					continue;
				}
			}
			if (seedType == SeedType.Lilypad)
			{
				int num20 = mBoard.CountPlantByType(seedType);
				int theTimeEnd = 18;
				if (mApp.IsColumnLevel())
				{
					theTimeEnd = 28;
				}
				todWeightedArray.mWeight = TodCommon.TodAnimateCurve(0, theTimeEnd, num20 + num18, todWeightedArray.mWeight, 1, TodCurves.Linear);
			}
			if (seedType == SeedType.Flowerpot)
			{
				int num21 = mBoard.CountPlantByType(seedType);
				int theTimeEnd2 = 35;
				if (mApp.IsColumnLevel())
				{
					theTimeEnd2 = 45;
				}
				todWeightedArray.mWeight = TodCommon.TodAnimateCurve(0, theTimeEnd2, num21 + num18, todWeightedArray.mWeight, 1, TodCurves.Linear);
			}
			if (mApp.IsFinalBossLevel())
			{
				if (seedType == SeedType.Melonpult || seedType == SeedType.Kernelpult || seedType == SeedType.Cabbagepult)
				{
					int num22 = mBoard.CountEmptyPotsOrLilies(SeedType.Flowerpot);
					if (num22 <= 2)
					{
						todWeightedArray.mWeight /= 5;
					}
					else if (num22 <= 5)
					{
						todWeightedArray.mWeight /= 3;
					}
				}
				if (seedType == SeedType.Flowerpot)
				{
					Zombie bossZombie = mBoard.GetBossZombie();
					if (bossZombie.mZombiePhase == ZombiePhase.BossDropRv)
					{
						todWeightedArray.mWeight = 500;
					}
				}
			}
			if (num2 > 2)
			{
				if (num18 >= 4)
				{
					todWeightedArray.mWeight = 1;
				}
				else if (num18 >= 3)
				{
					todWeightedArray.mWeight = 5;
				}
				else if (seedType == mLastConveyorSeedType)
				{
					todWeightedArray.mWeight /= 2;
				}
			}
		}
		object obj = TodCommon.TodPickFromWeightedArray(aSeedPickArray, num2);
		bool isImitator;
		if (obj is CardPair cardPair3)
		{
			seedType = cardPair3.mSeedType;
			isImitator = cardPair3.mImitator;
		}
		else
		{
			seedType = (SeedType)obj;
			isImitator = false;
		}
		mBoard.mSeedBank.AddSeed(seedType, isImitator);
		mLastConveyorSeedType = seedType;
		static bool MakesSun(SeedType mSeedType)
		{
			if (mSeedType != SeedType.Sunflower && mSeedType != SeedType.Twinsunflower)
			{
				return mSeedType == SeedType.Sunshroom;
			}
			return true;
		}
		static SeedType SearchForSunCost(int low, int high, bool includeUpgrades, bool includeProductionPlants, bool includeNocturnal, bool flag8)
		{
			IEnumerable<SeedType> source = from plantDef in GameConstants.gPlantDefs
				where plantDef.mSeedType != SeedType.None && plantDef.mSeedType != SeedType.Imitater && plantDef.mSeedType != SeedType.InstantCoffee && plantDef.mSeedType != SeedType.GoldMagnet && ((plantDef.mSeedType != SeedType.Cobcannon) | flag8) && plantDef.mSeedType != SeedType.Flowerpot && (includeUpgrades || !Plant.IsUpgrade(plantDef.mSeedType)) && (includeProductionPlants || !MakesSun(plantDef.mSeedType)) && (includeNocturnal || !Plant.IsNocturnal(plantDef.mSeedType)) && (!includeNocturnal || Plant.IsNocturnal(plantDef.mSeedType)) && plantDef.mSeedType < SeedType.ExplodeONut && plantDef.mSeedCost >= low && plantDef.mSeedCost < high
				select plantDef.mSeedType;
			int ceiling = source.Count();
			return source.ElementAt(RandomNumbers.NextNumber(ceiling));
		}
	}

	public void PortalStart()
	{
		mChallengeStateCounter = 9000;
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.PortalSquare;
		newGridItem.mGridX = 2;
		newGridItem.mGridY = 0;
		newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, newGridItem.mGridY, 0);
		newGridItem.OpenPortal();
		mBoard.mGridItems.Add(newGridItem);
		GridItem newGridItem2 = GridItem.GetNewGridItem();
		newGridItem2.mGridItemType = GridItemType.PortalSquare;
		newGridItem2.mGridX = 9;
		newGridItem2.mGridY = 1;
		newGridItem2.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, newGridItem2.mGridY, 0);
		newGridItem2.OpenPortal();
		mBoard.mGridItems.Add(newGridItem2);
		GridItem newGridItem3 = GridItem.GetNewGridItem();
		newGridItem3.mGridItemType = GridItemType.PortalCircle;
		newGridItem3.mGridX = 9;
		newGridItem3.mGridY = 3;
		newGridItem3.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, newGridItem3.mGridY, 0);
		newGridItem3.OpenPortal();
		mBoard.mGridItems.Add(newGridItem3);
		GridItem newGridItem4 = GridItem.GetNewGridItem();
		newGridItem4.mGridItemType = GridItemType.PortalCircle;
		newGridItem4.mGridX = 2;
		newGridItem4.mGridY = 4;
		newGridItem4.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, newGridItem4.mGridY, 0);
		newGridItem4.OpenPortal();
		mBoard.mGridItems.Add(newGridItem4);
		mBoard.mZombieCountDown = 200;
		mBoard.mZombieCountDownStart = mBoard.mZombieCountDown;
		mConveyorBeltCounter = 200;
	}

	public void UpdatePortalCombat()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemState != GridItemState.PortalClosed && (theGridItem.mGridItemType == GridItemType.PortalCircle || theGridItem.mGridItemType == GridItemType.PortalSquare))
			{
				UpdatePortal(theGridItem);
			}
		}
		if (mBoard.HasLevelAwardDropped())
		{
			mBoard.ClearAdvice(AdviceType.PortalRelocating);
			return;
		}
		mChallengeStateCounter--;
		if (mChallengeStateCounter == 500)
		{
			mBoard.DisplayAdviceAgain("[ADVICE_PORTAL_RELOCATING]", MessageStyle.HintFast, AdviceType.PortalRelocating);
		}
		if (mChallengeStateCounter <= 0)
		{
			mBoard.ClearAdvice(AdviceType.PortalRelocating);
			mChallengeStateCounter = 6000;
			MoveAPortal();
		}
	}

	public GridItem GetOtherPortal(GridItem thePortal)
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (thePortal != theGridItem && theGridItem.mGridItemState != GridItemState.PortalClosed && thePortal.mGridItemType == theGridItem.mGridItemType)
			{
				return theGridItem;
			}
		}
		return null;
	}

	public void UpdatePortal(GridItem thePortal)
	{
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead || zombie.mRow != thePortal.mGridY || zombie.mLastPortalX == thePortal.mGridX)
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			int num = zombieRect.mX + zombieRect.mWidth / 2;
			int num2 = thePortal.mGridX * 80 + 25 + Constants.BOARD_EXTRA_ROOM;
			int num3 = Math.Abs(num - num2);
			if (num3 > 45)
			{
				continue;
			}
			GridItem otherPortal = GetOtherPortal(thePortal);
			if (otherPortal != null)
			{
				int num4 = num - zombie.mX;
				num4 = (int)((float)num4 * 0.5f);
				if (zombie.IsWalkingBackwards())
				{
					num4 -= 60;
				}
				zombie.mLastPortalX = otherPortal.mGridX;
				zombie.mX = otherPortal.mGridX * 80 - num4 + Constants.BOARD_EXTRA_ROOM;
				zombie.mPosX = zombie.mX;
				zombie.SetRow(otherPortal.mGridY);
				zombie.mY = (int)zombie.GetPosYBasedOnRow(otherPortal.mGridY);
				zombie.mPosY = zombie.mY;
				zombie.cachedZombieRectUpToDate = false;
			}
		}
		int index = -1;
		Projectile theProjectile = null;
		while (mBoard.IterateProjectiles(ref theProjectile, ref index))
		{
			if ((theProjectile.mMotionType != ProjectileMotion.Straight && theProjectile.mMotionType != ProjectileMotion.YAwareStraight) || theProjectile.mRow != thePortal.mGridY || theProjectile.mLastPortalX == thePortal.mGridX)
			{
				continue;
			}
			TRect projectileRect = theProjectile.GetProjectileRect();
			int num5 = projectileRect.mX + projectileRect.mWidth / 2;
			int num6 = thePortal.mGridX * 80 + 55 + Constants.BOARD_EXTRA_ROOM;
			int num7 = Math.Abs(num5 - num6);
			if (num7 <= 40)
			{
				GridItem otherPortal2 = GetOtherPortal(thePortal);
				if (otherPortal2 != null)
				{
					int num8 = num5 - theProjectile.mX;
					int num9 = otherPortal2.mGridY - thePortal.mGridY;
					int num10 = num9 * 100;
					theProjectile.mX = otherPortal2.mGridX * 80 - num8 + 60 + Constants.BOARD_EXTRA_ROOM;
					theProjectile.mPosX = theProjectile.mX;
					theProjectile.mRow = otherPortal2.mGridY;
					theProjectile.mY += num10;
					theProjectile.mPosY = theProjectile.mY;
					theProjectile.mShadowY += num10;
					theProjectile.mLastPortalX = otherPortal2.mGridX;
					theProjectile.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Projectile, theProjectile.mRow, 0);
				}
			}
		}
		LawnMower theLawnMower = null;
		while (mBoard.IterateLawnMowers(ref theLawnMower))
		{
			if (theLawnMower.mMowerState != LawnMowerState.Triggered || theLawnMower.mRow != thePortal.mGridY || theLawnMower.mLastPortalX == thePortal.mGridX)
			{
				continue;
			}
			int num11 = thePortal.mGridX * 80 + 25 + Constants.BOARD_EXTRA_ROOM;
			int num12 = (int)theLawnMower.mPosX + 45;
			int num13 = Math.Abs(num12 - num11);
			if (num13 <= 20)
			{
				GridItem otherPortal3 = GetOtherPortal(thePortal);
				if (otherPortal3 != null)
				{
					int num14 = otherPortal3.mGridY - thePortal.mGridY;
					int num15 = num14 * 100;
					theLawnMower.mPosX = otherPortal3.mGridX * 80 + 25 + Constants.BOARD_EXTRA_ROOM;
					theLawnMower.mRow = otherPortal3.mGridY;
					theLawnMower.mPosY = num15;
					theLawnMower.mLastPortalX = otherPortal3.mGridX;
					theLawnMower.mRenderOrder = Board.MakeRenderOrder(RenderLayer.LawnMower, theLawnMower.mRow, 0);
					theLawnMower.Update();
				}
			}
		}
	}

	public float PortalCombatRowSpawnWeight(int theGridY)
	{
		int portalDistanceToMower = GetPortalDistanceToMower(theGridY);
		if (portalDistanceToMower < 5)
		{
			return 0.01f;
		}
		bool flag = false;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemState != GridItemState.PortalClosed && (theGridItem.mGridItemType == GridItemType.PortalCircle || theGridItem.mGridItemType == GridItemType.PortalSquare) && theGridItem.mGridY == theGridY)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return 1f;
		}
		return 0.2f;
	}

	public bool CanTargetZombieWithPortals(Plant thePlant, Zombie theZombie)
	{
		int num = thePlant.mPlantCol + 1;
		int num2 = thePlant.mRow;
		for (int i = 0; i < 3; i++)
		{
			GridItem portalToRight = GetPortalToRight(num, num2);
			if (num2 == theZombie.mRow)
			{
				int num3 = num * 80;
				int num4 = 900;
				if (portalToRight != null)
				{
					num4 = portalToRight.mGridX * 80;
				}
				if (theZombie.mX > num3 && theZombie.mX < num4)
				{
					return true;
				}
			}
			if (portalToRight == null)
			{
				break;
			}
			GridItem otherPortal = GetOtherPortal(portalToRight);
			if (otherPortal == null)
			{
				break;
			}
			num = otherPortal.mGridX;
			num2 = otherPortal.mGridY;
		}
		return false;
	}

	public GridItem GetPortalToRight(int theGridX, int theGridY)
	{
		GridItem gridItem = null;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemState != GridItemState.PortalClosed && (theGridItem.mGridItemType == GridItemType.PortalCircle || theGridItem.mGridItemType == GridItemType.PortalSquare) && theGridItem.mGridX - 1 >= theGridX && theGridItem.mGridY == theGridY && (gridItem == null || gridItem.mGridX >= theGridItem.mGridX))
			{
				gridItem = theGridItem;
			}
		}
		return gridItem;
	}

	public GridItem GetPortalAt(int theGridX, int theGridY)
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridX == theGridX && theGridItem.mGridY == theGridY && theGridItem.mGridItemState != GridItemState.PortalClosed && (theGridItem.mGridItemType == GridItemType.PortalCircle || theGridItem.mGridItemType == GridItemType.PortalSquare))
			{
				return theGridItem;
			}
		}
		return null;
	}

	public void MoveAPortal()
	{
		TodWeightedArray[] array = new TodWeightedArray[4];
		int num = 0;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemState != GridItemState.PortalClosed && (theGridItem.mGridItemType == GridItemType.PortalCircle || theGridItem.mGridItemType == GridItemType.PortalSquare))
			{
				Debug.ASSERT(num < 4);
				array[num] = TodWeightedArray.GetNewTodWeightedArray();
				array[num].mWeight = 1;
				array[num].mItem = theGridItem;
				num++;
			}
		}
		Debug.ASSERT(num != 0);
		GridItem gridItem = (GridItem)TodCommon.TodPickFromWeightedArray(array, num);
		GridItem otherPortal = GetOtherPortal(gridItem);
		Debug.ASSERT(otherPortal != null);
		TodWeightedGridArray[] array2 = new TodWeightedGridArray[50];
		int num2 = 0;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
		for (int j = 2; j < 10; j++)
		{
			for (int k = 0; k < 5; k++)
			{
				if (GetPortalAt(j, k) == null && otherPortal.mGridY != k && gridItem.mGridY != k)
				{
					Debug.ASSERT(num2 < 50);
					array2[num2].mX = j;
					array2[num2].mY = k;
					array2[num2].mWeight = 1;
					num2++;
				}
			}
		}
		TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array2, num2);
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = gridItem.mGridItemType;
		newGridItem.mGridX = todWeightedGridArray.mX;
		newGridItem.mGridY = todWeightedGridArray.mY;
		newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, newGridItem.mGridY, 0);
		newGridItem.OpenPortal();
		mBoard.mGridItems.Add(newGridItem);
		gridItem.ClosePortal();
	}

	public int GetPortalDistanceToMower(int theGridY)
	{
		int theGridY2 = theGridY;
		int num = 10;
		int num2 = 0;
		while (num2 < 40)
		{
			GridItem portalToLeft = GetPortalToLeft(num, theGridY2);
			if (portalToLeft == null)
			{
				num2 += num;
				break;
			}
			GridItem otherPortal = GetOtherPortal(portalToLeft);
			Debug.ASSERT(otherPortal != null);
			num2 += num - portalToLeft.mGridX;
			num = otherPortal.mGridX;
			theGridY2 = otherPortal.mGridY;
		}
		return num2;
	}

	public GridItem GetPortalToLeft(int theGridX, int theGridY)
	{
		GridItem gridItem = null;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemState != GridItemState.PortalClosed && (theGridItem.mGridItemType == GridItemType.PortalCircle || theGridItem.mGridItemType == GridItemType.PortalSquare) && theGridItem.mGridX < theGridX && theGridItem.mGridY == theGridY && (gridItem == null || gridItem.mGridX <= theGridItem.mGridX))
			{
				gridItem = theGridItem;
			}
		}
		return gridItem;
	}

	public void BeghouledPacketClicked(SeedPacket theSeedPacket)
	{
		int currentPlantCost = mBoard.GetCurrentPlantCost(theSeedPacket.mPacketType, SeedType.None);
		if (!mBoard.CanTakeSunMoney(currentPlantCost))
		{
			return;
		}
		if (theSeedPacket.mPacketType == SeedType.Repeater && !mBoard.mChallenge.mBeghouledPurcasedUpgrade[0])
		{
			mBoard.mChallenge.mBeghouledPurcasedUpgrade[0] = true;
			int count = mBoard.mPlants.Count;
			for (int i = 0; i < count; i++)
			{
				Plant plant = mBoard.mPlants[i];
				if (!plant.mDead && plant.mSeedType == SeedType.Peashooter)
				{
					plant.Die();
					mBoard.AddPlant(plant.mPlantCol, plant.mRow, SeedType.Repeater, SeedType.None);
				}
			}
			theSeedPacket.Deactivate();
		}
		else if (theSeedPacket.mPacketType == SeedType.Fumeshroom && !mBoard.mChallenge.mBeghouledPurcasedUpgrade[1])
		{
			mBoard.mChallenge.mBeghouledPurcasedUpgrade[1] = true;
			int count2 = mBoard.mPlants.Count;
			for (int j = 0; j < count2; j++)
			{
				Plant plant2 = mBoard.mPlants[j];
				if (!plant2.mDead && plant2.mSeedType == SeedType.Puffshroom)
				{
					plant2.Die();
					mBoard.AddPlant(plant2.mPlantCol, plant2.mRow, SeedType.Fumeshroom, SeedType.None);
				}
			}
			theSeedPacket.Deactivate();
		}
		else if (theSeedPacket.mPacketType == SeedType.Tallnut && !mBoard.mChallenge.mBeghouledPurcasedUpgrade[2])
		{
			mBoard.mChallenge.mBeghouledPurcasedUpgrade[2] = true;
			int count3 = mBoard.mPlants.Count;
			for (int k = 0; k < count3; k++)
			{
				Plant plant3 = mBoard.mPlants[k];
				if (!plant3.mDead && plant3.mSeedType == SeedType.Wallnut)
				{
					plant3.Die();
					mBoard.AddPlant(plant3.mPlantCol, plant3.mRow, SeedType.Tallnut, SeedType.None);
				}
			}
			theSeedPacket.Deactivate();
		}
		else if (theSeedPacket.mPacketType == SeedType.BeghouledButtonShuffle)
		{
			if (mChallengeState == ChallengeState.BeghouledFalling || mChallengeState == ChallengeState.BeghouledMoving)
			{
				return;
			}
			BeghouledShuffle();
		}
		else if (theSeedPacket.mPacketType == SeedType.BeghouledButtonCrater)
		{
			if (!BeghouledCanClearCrater() || mChallengeState == ChallengeState.BeghouledFalling || mChallengeState == ChallengeState.BeghouledMoving)
			{
				return;
			}
			BeghouledClearCrater(1);
			BeghouledStartFalling(ChallengeState.BeghouledFalling);
		}
		mBoard.TakeSunMoney(currentPlantCost);
	}

	public void BeghouledShuffle()
	{
		mBoard.ClearAdvice(AdviceType.None);
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead)
			{
				plant.Die();
			}
		}
		BeghouledStartFalling(ChallengeState.BeghouledFalling);
	}

	public bool BeghouledCanClearCrater()
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				if (mBeghouledEated[i, j])
				{
					return true;
				}
			}
		}
		return false;
	}

	public void BeghouledUpdateCraters()
	{
		if (mBoard.mSeedBank.mNumPackets == 5)
		{
			SeedPacket seedPacket = mBoard.mSeedBank.mSeedPackets[4];
			Debug.ASSERT(seedPacket.mPacketType == SeedType.BeghouledButtonCrater);
			if (BeghouledCanClearCrater())
			{
				seedPacket.Activate();
			}
			else
			{
				seedPacket.Deactivate();
			}
		}
	}

	public Zombie ZombiquariumSpawnSnorkel()
	{
		Zombie zombie = mBoard.AddZombieInRow(ZombieType.Snorkel, 0, 0);
		zombie.mPosX = TodCommon.RandRangeFloat(50f, 650f);
		zombie.mPosY = TodCommon.RandRangeFloat(100f, 400f);
		return zombie;
	}

	public void ZombiquariumPacketClicked(SeedPacket theSeedPacket)
	{
		int currentPlantCost = mBoard.GetCurrentPlantCost(theSeedPacket.mPacketType, SeedType.None);
		if (!mBoard.CanTakeSunMoney(currentPlantCost))
		{
			return;
		}
		if (theSeedPacket.mPacketType == SeedType.ZombiquariumSnorkel)
		{
			int num = mBoard.CountZombiesOnScreen();
			if (num > 100)
			{
				return;
			}
			if (mBoard.mTutorialState == TutorialState.ZombiquariumBuySnorkel)
			{
				mBoard.ClearAdvice(AdviceType.ZombiquariumBuySnorkel);
				mBoard.TutorialArrowRemove();
				mBoard.mTutorialState = TutorialState.ZombiquariumBoughtSnorkel;
			}
			Zombie zombie = ZombiquariumSpawnSnorkel();
			mApp.PlayFoley(FoleyType.Zombiesplash);
			mApp.AddTodParticle(zombie.mPosX + 60f, zombie.mPosY + 20f, 400000, ParticleEffect.PlantingPool);
		}
		else if (theSeedPacket.mPacketType == SeedType.ZombiquariumTrophy)
		{
			SpawnLevelAward(2, 0);
			mBoard.ClearAdvice(AdviceType.None);
		}
		mBoard.TakeSunMoney(currentPlantCost);
	}

	public void ZombiquariumMouseDown(int x, int y)
	{
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		if (x < 80 - Constants.BOARD_EXTRA_ROOM || x > 720 || y < 90 || y > 430)
		{
			return;
		}
		int num = 0;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Brain)
			{
				num++;
			}
		}
		if (num < 3 && mBoard.TakeSunMoney(5))
		{
			ZombiquariumDropBrain(x, y);
		}
	}

	public void ZombiquariumDropBrain(int x, int y)
	{
		mBoard.ClearAdvice(AdviceType.ZombiquariumClickToFeed);
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.Brain;
		newGridItem.mRenderOrder = 400000;
		newGridItem.mGridX = 0;
		newGridItem.mGridY = 0;
		newGridItem.mGridItemCounter = 0;
		newGridItem.mPosX = (float)x - 15f;
		newGridItem.mPosY = (float)y - 15f;
		mApp.PlaySample(Resources.SOUND_TAP);
		mBoard.mGridItems.Add(newGridItem);
	}

	public void ZombiquariumUpdate()
	{
		if (mApp.mBoard.mZombies.Count == 0 && !mBoard.HasLevelAwardDropped())
		{
			mBoard.ZombiesWon(null);
			return;
		}
		if (!mBoard.mAdvice.IsBeingDisplayed() && !mBoard.mHelpDisplayed[49])
		{
			string theAdvice = TodCommon.TodReplaceNumberString("[ADVICE_ZOMBIQUARIUM_COLLECT_SUN]", "{SCORE}", 1000);
			mBoard.DisplayAdvice(theAdvice, MessageStyle.HintTallFast, AdviceType.ZombiquariumCollectSun);
		}
		int num = TodCommon.ClampInt(mBoard.mSunMoney, 0, 1000);
		mBoard.mProgressMeterWidth = TodCommon.TodAnimateCurve(0, 1000, num, 0, 150, TodCurves.Linear);
		if (num >= 900)
		{
			mBoard.DisplayAdvice("[ADVICE_ALMOST_THERE]", MessageStyle.HintTallFast, AdviceType.AlmostThere);
		}
		if (num >= 110 && mBoard.mTutorialState == TutorialState.Off)
		{
			mBoard.mTutorialState = TutorialState.ZombiquariumBuySnorkel;
			float screenX = mBoard.mSeedBank.mX + mBoard.mSeedBank.mSeedPackets[0].mX;
			float screenY = mBoard.mSeedBank.mY + mBoard.mSeedBank.mSeedPackets[0].mY;
			if (mBoard.mCameraEnabled && mBoard.mCamera != null)
			{
				mBoard.mCamera?.ScreenToBoardReplace(ref screenX, ref screenY);
			}
			mBoard.TutorialArrowShow((int)screenX, (int)screenY);
			mBoard.DisplayAdvice("[ADVICE_ZOMBIQUARIUM_BUY_SNORKEL]", MessageStyle.HintTallFast, AdviceType.ZombiquariumBuySnorkel);
		}
		else if (num < 100 && mBoard.mTutorialState == TutorialState.ZombiquariumBuySnorkel)
		{
			mBoard.TutorialArrowRemove();
			mBoard.ClearAdvice(AdviceType.ZombiquariumBuySnorkel);
			mBoard.mTutorialState = TutorialState.Off;
		}
		if (num >= 1000 && mBoard.mTutorialState == TutorialState.ZombiquariumBoughtSnorkel)
		{
			mBoard.mTutorialState = TutorialState.ZombiquariumClickTrophy;
			float screenX2 = mBoard.mSeedBank.mX + mBoard.mSeedBank.mSeedPackets[1].mX;
			float screenY2 = mBoard.mSeedBank.mY + mBoard.mSeedBank.mSeedPackets[1].mY;
			if (mBoard.mCameraEnabled && mBoard.mCamera != null)
			{
				mBoard.mCamera?.ScreenToBoardReplace(ref screenX2, ref screenY2);
			}
			mBoard.TutorialArrowShow((int)screenX2, (int)screenY2);
			mBoard.DisplayAdvice("[ADVICE_ZOMBIQUARIUM_CLICK_TROPHY]", MessageStyle.HintTallFast, AdviceType.ZombiquariumClickTrophy);
		}
		else if (num < 1000 && mBoard.mTutorialState == TutorialState.ZombiquariumClickTrophy)
		{
			mBoard.TutorialArrowRemove();
			mBoard.ClearAdvice(AdviceType.ZombiquariumClickTrophy);
			mBoard.mTutorialState = TutorialState.ZombiquariumBoughtSnorkel;
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Brain)
			{
				theGridItem.mGridItemCounter++;
				theGridItem.mPosY += 0.15f;
				if (theGridItem.mPosY >= 500f)
				{
					theGridItem.GridItemDie();
				}
			}
		}
	}

	public void ShovelAddWallnuts()
	{
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				mBoard.AddPlant(i, j, SeedType.Wallnut, SeedType.None);
			}
		}
	}

	public void ScaryPotterPlacePot(ScaryPotType theScaryPotType, ZombieType theZombieType, SeedType theSeedType, int theCount, TodWeightedGridArray[] theGridArray, int theGridArrayCount)
	{
		Debug.ASSERT(theScaryPotType != ScaryPotType.Seed || theSeedType != SeedType.None);
		Debug.ASSERT(theScaryPotType != ScaryPotType.Zombie || theZombieType != ZombieType.Invalid);
		for (int i = 0; i < theCount; i++)
		{
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(theGridArray, theGridArrayCount);
			todWeightedGridArray.mWeight = 0;
			GridItem newGridItem = GridItem.GetNewGridItem();
			newGridItem.mGridItemType = GridItemType.ScaryPot;
			newGridItem.mGridItemState = GridItemState.ScaryPotQuestion;
			newGridItem.mGridX = todWeightedGridArray.mX;
			newGridItem.mGridY = todWeightedGridArray.mY;
			newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, newGridItem.mGridY, 0);
			newGridItem.mSeedType = theSeedType;
			newGridItem.mZombieType = theZombieType;
			newGridItem.mScaryPotType = theScaryPotType;
			mBoard.mGridItems.Add(newGridItem);
			if (theScaryPotType == ScaryPotType.Sun)
			{
				newGridItem.mSunCount = TodCommon.RandRangeInt(1, 3);
			}
		}
	}

	public void ScaryPotterStart()
	{
		if (mApp.IsAdventureMode())
		{
			mBoard.DisplayAdvice("[ADVICE_USE_SHOVEL_ON_POTS]", MessageStyle.HintStay, AdviceType.UseShovelOnPots);
		}
	}

	public void ScaryPotterUpdate()
	{
		if (mChallengeState != ChallengeState.ScaryPotterMalleting)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mReanimChallenge);
		if (reanimation.mLoopCount > 0)
		{
			GridItem scaryPotAt = mBoard.GetScaryPotAt(mChallengeGridX, mChallengeGridY);
			if (scaryPotAt != null)
			{
				ScaryPotterOpenPot(scaryPotAt);
			}
			mChallengeGridX = 0;
			mChallengeGridY = 0;
			reanimation.ReanimationDie();
			mReanimChallenge = null;
			mChallengeState = ChallengeState.Normal;
		}
	}

	public void ScaryPotterOpenPot(GridItem theScaryPot)
	{
		int num = mBoard.GridToPixelX(theScaryPot.mGridX, theScaryPot.mGridY);
		int num2 = mBoard.GridToPixelY(theScaryPot.mGridX, theScaryPot.mGridY);
		switch (theScaryPot.mScaryPotType)
		{
		case ScaryPotType.Seed:
		{
			Coin coin = mBoard.AddCoin(num + 20, num2, CoinType.UsableSeedPacket, CoinMotion.FromPlant);
			coin.mUsableSeedType = theScaryPot.mSeedType;
			break;
		}
		case ScaryPotType.Zombie:
		{
			Zombie zombie = mBoard.AddZombieInRow(theScaryPot.mZombieType, theScaryPot.mGridY, 0);
			zombie.mPosX = num;
			break;
		}
		case ScaryPotType.Sun:
		{
			int num3 = ScaryPotterCountSunInPot(theScaryPot);
			for (int i = 0; i < num3; i++)
			{
				mBoard.AddCoin(num, num2, CoinType.Sun, CoinMotion.FromPlant);
				num += 15;
			}
			break;
		}
		default:
			Debug.ASSERT(value: false);
			break;
		}
		theScaryPot.GridItemDie();
		if (mBoard.mHelpIndex == AdviceType.UseShovelOnPots)
		{
			mBoard.DisplayAdvice("[ADVICE_DESTROY_POTS_TO_FINISH_LEVEL]", MessageStyle.HintFast, AdviceType.DestroyPotsToFinisihLevel);
		}
		if (ScaryPotterIsCompleted())
		{
			if (mApp.IsScaryPotterLevel() && !mBoard.IsFinalScaryPotterStage())
			{
				PuzzlePhaseComplete(theScaryPot.mGridX, theScaryPot.mGridY);
			}
			else
			{
				SpawnLevelAward(theScaryPot.mGridX, theScaryPot.mGridY);
			}
		}
		mApp.PlaySample(Resources.SOUND_BONK);
		mApp.PlayFoley(FoleyType.VaseBreaking);
		if (theScaryPot.mGridItemState == GridItemState.ScaryPotLeaf)
		{
			mApp.AddTodParticle(num + 20, num2, 200000, ParticleEffect.VaseShatterLeaf);
		}
		else if (theScaryPot.mGridItemState == GridItemState.ScaryPotZombie)
		{
			mApp.AddTodParticle(num + 20, num2, 200000, ParticleEffect.VaseShatterZombie);
		}
		else
		{
			mApp.AddTodParticle(num + 20, num2, 200000, ParticleEffect.VaseShatter);
		}
	}

	public void ScaryPotterJackExplode(int aPosX, int aPosY)
	{
		int num = mBoard.PixelToGridX(aPosX, aPosY);
		int num2 = mBoard.PixelToGridY(aPosX, aPosY);
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ScaryPot && theGridItem.mGridX >= num - 1 && theGridItem.mGridX <= num + 1 && theGridItem.mGridY >= num2 - 1 && theGridItem.mGridY <= num2 + 1)
			{
				ScaryPotterOpenPot(theGridItem);
			}
		}
	}

	public bool ScaryPotterIsCompleted()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ScaryPot)
			{
				return false;
			}
		}
		return !mBoard.AreEnemyZombiesOnScreen();
	}

	public void ScaryPotterChangePotType(GridItemState thePotType, int theCount)
	{
		int num = 0;
		for (int i = 0; i < aPotArray.Length; i++)
		{
			aPotArray[i].Reset();
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemState == GridItemState.ScaryPotQuestion && (thePotType != GridItemState.ScaryPotLeaf || theGridItem.mScaryPotType == ScaryPotType.Seed) && (thePotType != GridItemState.ScaryPotZombie || theGridItem.mZombieType == ZombieType.Gargantuar))
			{
				aPotArray[num].mItem = theGridItem;
				aPotArray[num].mWeight = 1;
				num++;
				Debug.ASSERT(num <= 54);
			}
		}
		if (theCount > num)
		{
			theCount = num;
		}
		for (int j = 0; j < theCount; j++)
		{
			TodWeightedArray todWeightedArray = TodCommon.TodPickArrayItemFromWeightedArray(aPotArray, num);
			todWeightedArray.mWeight = 0;
			GridItem gridItem = (GridItem)todWeightedArray.mItem;
			gridItem.mGridItemState = thePotType;
		}
	}

	public void ScaryPotterPopulate()
	{
		int num = 0;
		for (int i = 0; i < aGridArray.Length; i++)
		{
			aGridArray[i].Reset();
		}
		for (int j = 0; j < 9; j++)
		{
			for (int k = 0; k < 5; k++)
			{
				aGridArray[num].mX = j;
				aGridArray[num].mY = k;
				aGridArray[num].mWeight = 1;
				num++;
				Debug.ASSERT(num <= 54);
			}
		}
		if (mApp.IsLevelUseJson())
		{
			CSVaseLevel component = mApp.mCreativeLevel.GetComponent<CSVaseLevel>();
			if (component != null)
			{
				for (int l = 0; l < Constants.GRIDSIZEX; l++)
				{
					if (l < component.mMinColumn || l > component.mMaxColumn)
					{
						ScaryPotterDontPlaceInCol(l, aGridArray, num);
					}
				}
				foreach (var (theSeedType, theCount) in component.mPlantVases)
				{
					ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, theSeedType, theCount, aGridArray, num);
				}
				foreach (var (theZombieType, theCount2) in component.mZombieVases)
				{
					ScaryPotterPlacePot(ScaryPotType.Zombie, theZombieType, SeedType.None, theCount2, aGridArray, num);
				}
				ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, component.mNumPlantVases);
				ScaryPotterChangePotType(GridItemState.ScaryPotZombie, component.mNumZombieVases);
				mScaryPotterPots = ScaryPotterCountPots();
				return;
			}
		}
		if ((mApp.IsAdventureMode() && mBoard.mLevel == 35) || mApp.mGameMode == GameMode.Quickplay35)
		{
			if (mSurvivalStage == 0)
			{
				ScaryPotterDontPlaceInCol(0, aGridArray, num);
				ScaryPotterDontPlaceInCol(1, aGridArray, num);
				ScaryPotterDontPlaceInCol(2, aGridArray, num);
				ScaryPotterDontPlaceInCol(3, aGridArray, num);
				ScaryPotterDontPlaceInCol(4, aGridArray, num);
				ScaryPotterDontPlaceInCol(5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Peashooter, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 4, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 1, aGridArray, num);
			}
			else if (mSurvivalStage == 1)
			{
				ScaryPotterDontPlaceInCol(0, aGridArray, num);
				ScaryPotterDontPlaceInCol(1, aGridArray, num);
				ScaryPotterDontPlaceInCol(2, aGridArray, num);
				ScaryPotterDontPlaceInCol(3, aGridArray, num);
				ScaryPotterDontPlaceInCol(4, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Peashooter, 4, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 4, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 1, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Football, SeedType.None, 1, aGridArray, num);
				ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
			}
			else if (mSurvivalStage == 2)
			{
				ScaryPotterDontPlaceInCol(0, aGridArray, num);
				ScaryPotterDontPlaceInCol(1, aGridArray, num);
				ScaryPotterDontPlaceInCol(2, aGridArray, num);
				ScaryPotterDontPlaceInCol(3, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Peashooter, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Hypnoshroom, 5, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 6, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 2, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Dancer, SeedType.None, 1, aGridArray, num);
				ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
				ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 3);
			}
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter1)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterDontPlaceInCol(2, aGridArray, num);
			ScaryPotterDontPlaceInCol(3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Peashooter, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter2)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterDontPlaceInCol(2, aGridArray, num);
			ScaryPotterDontPlaceInCol(8, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 7, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Wallnut, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Potatomine, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter3)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterDontPlaceInCol(2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Hypnoshroom, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Wallnut, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 8, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Dancer, SeedType.None, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter4)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Puffshroom, 11, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Hypnoshroom, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 8, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 7, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Football, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter5)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Pumpkinshell, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Hypnoshroom, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Magnetshroom, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Football, SeedType.None, 3, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter6)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 7, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Tallnut, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Threepeater, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Torchwood, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 7, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Polevaulter, SeedType.None, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Football, SeedType.None, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter7)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterDontPlaceInCol(2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Spikeweed, 13, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Wallnut, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 10, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter8)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Puffshroom, 7, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Tallnut, 3, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 8, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 4, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pogo, SeedType.None, 4, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotter9)
		{
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Peashooter, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Threepeater, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Potatomine, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Wallnut, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Plantern, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 8, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Gargantuar, SeedType.None, 1, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else if (mApp.mGameMode == GameMode.ScaryPotterEndless)
		{
			int num2 = TodCommon.ClampInt(mSurvivalStage / 10, 0, 8);
			ScaryPotterDontPlaceInCol(0, aGridArray, num);
			ScaryPotterDontPlaceInCol(1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Leftpeater, 6, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Snowpea, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Peashooter, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Threepeater, 2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Squash, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Potatomine, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Wallnut, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Seed, ZombieType.Invalid, SeedType.Plantern, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Sun, ZombieType.Invalid, SeedType.None, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Normal, SeedType.None, 8 - num2, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Pail, SeedType.None, 5, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.JackInTheBox, SeedType.None, 1, aGridArray, num);
			ScaryPotterPlacePot(ScaryPotType.Zombie, ZombieType.Gargantuar, SeedType.None, 1 + num2, aGridArray, num);
			ScaryPotterChangePotType(GridItemState.ScaryPotLeaf, 2);
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		mScaryPotterPots = ScaryPotterCountPots();
	}

	public void ScaryPotterDontPlaceInCol(int theCol, TodWeightedGridArray[] theGridArray, int theGridArrayCount)
	{
		for (int i = 0; i < theGridArrayCount; i++)
		{
			if (theGridArray[i].mX == theCol)
			{
				theGridArray[i].mWeight = 0;
			}
		}
	}

	public void ScaryPotterFillColumnWithPlant(int theCol, SeedType theSeedType, TodWeightedGridArray[] theGridArray, int theGridArrayCount)
	{
		ScaryPotterDontPlaceInCol(theCol, theGridArray, theGridArrayCount);
		for (int i = 0; i < 5; i++)
		{
			Plant plant = mBoard.NewPlant(theCol, i, theSeedType, SeedType.None);
			if (theSeedType == SeedType.Potatomine)
			{
				plant.mStateCountdown = 10;
			}
		}
	}

	public void PuzzleNextStageClear()
	{
		mApp.PlaySample(Resources.SOUND_HUGE_WAVE);
		mBoard.mNextSurvivalStageCounter = 0;
		mBoard.mProgressMeterWidth = 0;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && zombie.IsOnBoard())
			{
				zombie.DieNoLoot(giveAchievements: false);
			}
		}
		count = mBoard.mPlants.Count;
		for (int j = 0; j < count; j++)
		{
			Plant plant = mBoard.mPlants[j];
			if (!plant.mDead && plant.IsOnBoard())
			{
				plant.Die();
			}
		}
		mBoard.RefreshSeedPacketFromCursor();
		Coin theCoin = null;
		while (mBoard.IterateCoins(ref theCoin))
		{
			if (theCoin.mType == CoinType.UsableSeedPacket)
			{
				theCoin.Die();
			}
			else
			{
				theCoin.TryAutoCollectAfterLevelAward();
			}
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			theGridItem.GridItemDie();
		}
		mSurvivalStage++;
		mBoard.ClearAdviceImmediately();
		mBoard.mLevelAwardSpawned = false;
		mApp.AddTodParticle(400f, 300f, 400000, ParticleEffect.ScreenFlash);
	}

	public void ScaryPotterMalletPot(GridItem theScaryPot)
	{
		mChallengeGridX = theScaryPot.mGridX;
		mChallengeGridY = theScaryPot.mGridY;
		int num = mBoard.GridToPixelX(theScaryPot.mGridX, theScaryPot.mGridY);
		int num2 = mBoard.GridToPixelY(theScaryPot.mGridX, theScaryPot.mGridY);
		Reanimation reanimation = mApp.AddReanimation(num, num2, 400000, ReanimationType.Hammer);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_open_pot, ReanimLoopType.PlayOnceAndHold, 0, 40f);
		mReanimChallenge = mApp.ReanimationGetID(reanimation);
		mChallengeState = ChallengeState.ScaryPotterMalleting;
		mApp.PlayFoley(FoleyType.Swing);
	}

	public static ZombieType IZombieSeedTypeToZombieType(SeedType theSeedType)
	{
		if (theSeedType == SeedType.None)
		{
			return ZombieType.Invalid;
		}
		if ((theSeedType & SeedType.MindControlledCardFlag) != 0)
		{
			return (ZombieType)(theSeedType & (SeedType)(-1073741825));
		}
		if ((theSeedType & SeedType.ZombieCardFlag) != 0)
		{
			return (ZombieType)(theSeedType & (SeedType)(-536870913));
		}
		switch (theSeedType)
		{
		case SeedType.ZombieNormal:
			return ZombieType.Normal;
		case SeedType.ZombieTrafficCone:
			return ZombieType.TrafficCone;
		case SeedType.ZombiePolevaulter:
			return ZombieType.Polevaulter;
		case SeedType.ZombiePail:
			return ZombieType.Pail;
		case SeedType.ZombieLadder:
			return ZombieType.Ladder;
		case SeedType.ZombieDigger:
			return ZombieType.Digger;
		case SeedType.ZombieBungee:
			return ZombieType.Bungee;
		case SeedType.ZombieFootball:
			return ZombieType.Football;
		case SeedType.ZombieBalloon:
			return ZombieType.Balloon;
		case SeedType.ZombieScreenDoor:
			return ZombieType.Door;
		case SeedType.Zomboni:
			return ZombieType.Zamboni;
		case SeedType.ZombiePogo:
			return ZombieType.Pogo;
		case SeedType.ZombieDancer:
			return ZombieType.Dancer;
		case SeedType.ZombieGargantuar:
			return ZombieType.Gargantuar;
		case SeedType.ZombieImp:
			return ZombieType.Imp;
		default:
			Debug.ASSERT(value: false);
			return ZombieType.Normal;
		}
	}

	public static bool IsZombieSeedType(SeedType theSeedType)
	{
		if (theSeedType == SeedType.None)
		{
			return false;
		}
		if ((theSeedType & SeedType.MindControlledCardFlag) != 0)
		{
			return true;
		}
		if ((theSeedType & SeedType.ZombieCardFlag) != 0)
		{
			return true;
		}
		if (theSeedType != SeedType.ZombieNormal && theSeedType != SeedType.ZombieTrafficCone && theSeedType != SeedType.ZombiePolevaulter && theSeedType != SeedType.ZombiePail && theSeedType != SeedType.ZombieLadder && theSeedType != SeedType.ZombieDigger && theSeedType != SeedType.ZombieBungee && theSeedType != SeedType.ZombieFootball && theSeedType != SeedType.ZombieBalloon && theSeedType != SeedType.ZombieScreenDoor && theSeedType != SeedType.Zomboni && theSeedType != SeedType.ZombiePogo && theSeedType != SeedType.ZombieDancer && theSeedType != SeedType.ZombieGargantuar)
		{
			return theSeedType == SeedType.ZombieImp;
		}
		return true;
	}

	public void IZombieMouseDownWithZombie(int x, int y, int theClickCount)
	{
		if (theClickCount < 0)
		{
			mBoard.RefreshSeedPacketFromCursor();
			mApp.PlayFoley(FoleyType.Drop);
			return;
		}
		int num = mBoard.PlantingPixelToGridX((int)((float)x * Constants.IS), (int)((float)y * Constants.IS), mBoard.mCursorObject.mType);
		int num2 = mBoard.PlantingPixelToGridY((int)((float)x * Constants.IS), (int)((float)y * Constants.IS), mBoard.mCursorObject.mType);
		if (num == -1 || num2 == -1)
		{
			mBoard.RefreshSeedPacketFromCursor();
			mApp.PlayFoley(FoleyType.Drop);
		}
		else if (CanPlantAt(num, num2, mBoard.mCursorObject.mType) == PlantingReason.Ok)
		{
			if (!mApp.mEasyPlantingCheat && mBoard.mCursorObject.mCursorType == CursorType.PlantFromBank && !mBoard.HasConveyorBeltSeedBank())
			{
				int currentPlantCost = mBoard.GetCurrentPlantCost(mBoard.mCursorObject.mType, mBoard.mCursorObject.mImitaterType);
				if (!mBoard.TakeSunMoney(currentPlantCost))
				{
					return;
				}
			}
			mBoard.ClearAdvice(AdviceType.IZombieLeftOfLine);
			mBoard.ClearAdvice(AdviceType.IZombieNotPassedLine);
			ZombieType theZombieType = IZombieSeedTypeToZombieType(mBoard.mCursorObject.mType);
			Zombie zombie = IZombiePlaceZombie(theZombieType, num, num2);
			if ((mBoard.mCursorObject.mType & SeedType.MindControlledCardFlag) != 0)
			{
				zombie.StartMindControlled();
			}
			if (mBoard.mCursorObject.mCursorType == CursorType.PlantFromUsableCoin)
			{
				Coin mCoinID = mBoard.mCursorObject.mCoinID;
				mBoard.mCursorObject.mCoinID = null;
				mCoinID.Die();
			}
			else if (mBoard.mCursorObject.mCursorType == CursorType.PlantFromBank)
			{
				Debug.ASSERT(mBoard.mCursorObject.mSeedBankIndex >= 0 && mBoard.mCursorObject.mSeedBankIndex < mBoard.mSeedBank.mNumPackets);
				SeedPacket seedPacket = mBoard.mSeedBank.mSeedPackets[mBoard.mCursorObject.mSeedBankIndex];
				seedPacket.WasPlanted();
			}
			mApp.PlayFoley(FoleyType.Plant);
			mBoard.ClearCursor();
		}
		else
		{
			mBoard.ClearAdvice(AdviceType.None);
			if (mBoard.mCursorObject.mType == SeedType.ZombieBungee)
			{
				mBoard.DisplayAdvice("[ADVICE_I_ZOMBIE_LEFT_OF_LINE]", MessageStyle.HintLong, AdviceType.IZombieLeftOfLine);
			}
			else
			{
				mBoard.DisplayAdvice("[ADVICE_I_ZOMBIE_NOT_PASSED_LINE]", MessageStyle.HintLong, AdviceType.IZombieNotPassedLine);
			}
		}
	}

	public void IZombieStart()
	{
		mBoard.DisplayAdvice("[ADVICE_I_ZOMBIE_EAT_ALL_BRAINS]", MessageStyle.HintFast, AdviceType.IZombieEatAllBrains);
	}

	public void IZombiePlacePlants(SeedType theSeedType, int theCount, int theGridY)
	{
		int num = 0;
		int num2 = 6;
		if (mApp.mGameMode == GameMode.PuzzleIZombie1 || mApp.mGameMode == GameMode.PuzzleIZombie2 || mApp.mGameMode == GameMode.PuzzleIZombie3 || mApp.mGameMode == GameMode.PuzzleIZombie4 || mApp.mGameMode == GameMode.PuzzleIZombie5)
		{
			num2 = 4;
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie6 || mApp.mGameMode == GameMode.PuzzleIZombie7 || mApp.mGameMode == GameMode.PuzzleIZombie8 || mApp.mGameMode == GameMode.PuzzleIZombieEndless)
		{
			num2 = 5;
		}
		CSIZombieLevel cSIZombieLevel = mApp.mCreativeLevel?.GetComponent<CSIZombieLevel>();
		if (cSIZombieLevel != null)
		{
			num2 = cSIZombieLevel.mLimit;
		}
		int num3;
		int num4;
		if (theGridY == -1)
		{
			num3 = 0;
			num4 = 4;
		}
		else
		{
			num3 = theGridY;
			num4 = theGridY;
		}
		for (int i = num3; i <= num4; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if (mBoard.CanPlantAt(j, i, theSeedType) == PlantingReason.Ok && ((theSeedType != SeedType.Wallnut && theSeedType != SeedType.Tallnut && theSeedType != SeedType.Torchwood) || num2 - j <= 3))
				{
					aGridArray[num].mX = j;
					aGridArray[num].mY = i;
					aGridArray[num].mWeight = 1;
					num++;
				}
			}
		}
		if (theCount > num)
		{
			theCount = num;
		}
		for (int k = 0; k < theCount; k++)
		{
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(aGridArray, num);
			Plant thePlant = mBoard.NewPlant(todWeightedGridArray.mX, todWeightedGridArray.mY, theSeedType, SeedType.None);
			todWeightedGridArray.mWeight = 0;
			IZombieSetupPlant(thePlant);
		}
	}

	public void IZombieUpdate()
	{
		int num = mBoard.mSunMoney;
		Coin theCoin = null;
		while (mBoard.IterateCoins(ref theCoin))
		{
			if (theCoin.IsSun())
			{
				num += theCoin.GetSunValue();
			}
		}
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying() && zombie.mZombiePhase != ZombiePhase.PolevaulterInVault && !zombie.mIsEating && zombie.mJustGotShotCounter < -500)
			{
				zombie.PickRandomSpeed();
			}
		}
		bool flag = false;
		count = mBoard.mPlants.Count;
		for (int j = 0; j < count; j++)
		{
			Plant plant = mBoard.mPlants[j];
			if (!plant.mDead && (plant.mState == PlantState.SquashFalling || plant.mState == PlantState.SquashDoneFalling || plant.mState == PlantState.ChomperBiting || plant.mState == PlantState.ChomperBitingGotOne))
			{
				flag = true;
				break;
			}
		}
		int index = -1;
		TodParticleSystem theParticle = null;
		while (mBoard.IterateParticles(ref theParticle, ref index))
		{
			if (theParticle.mEffectType == ParticleEffect.PotatoMine)
			{
				flag = true;
				break;
			}
		}
		if (mApp.mBoard.mZombies.Count != 0 || num >= 50 || mBoard.HasLevelAwardDropped() || flag)
		{
			return;
		}
		Coin theCoin2 = null;
		while (mBoard.IterateCoins(ref theCoin2))
		{
			if (theCoin2.IsMoney())
			{
				theCoin2.Die();
			}
		}
		mBoard.ZombiesWon(null);
	}

	public void IZombieDrawPlant(Graphics g, Plant thePlant)
	{
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		Reanimation reanimation = mApp.ReanimationTryToGet(thePlant.mBodyReanimID);
		if (reanimation != null)
		{
			IZombieSetPlantFilterEffect(thePlant, FilterEffectType.White);
			float num = g.mTransX;
			float num2 = g.mTransY;
			g.SetColorizeImages(colorizeImages: true);
			g.mTransX = (int)(num + Constants.S * 4f);
			g.mTransY = (int)(num2 + Constants.S * 4f);
			g.SetColor(new SexyColor(122, 86, 58));
			reanimation.DrawRenderGroup(g, 0);
			g.mTransX = (int)(num + Constants.S * 2f);
			g.mTransY = (int)(num2 + Constants.S * 2f);
			g.SetColor(new SexyColor(171, 135, 107));
			reanimation.DrawRenderGroup(g, 0);
			g.mTransX = (int)(num - Constants.S * 2f);
			g.mTransY = (int)(num2 - Constants.S * 2f);
			g.SetColor(new SexyColor(171, 135, 107));
			reanimation.DrawRenderGroup(g, 0);
			g.mTransX = (int)num;
			g.mTransY = (int)num2;
			g.SetColor(new SexyColor(255, 201, 160));
			IZombieSetPlantFilterEffect(thePlant, FilterEffectType.None);
			reanimation.DrawRenderGroup(g, 0);
			IZombieSetPlantFilterEffect(thePlant, FilterEffectType.None);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public void IZombieSetPlantFilterEffect(Plant thePlant, FilterEffectType theFilterEffect)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(thePlant.mBodyReanimID);
		Reanimation reanimation2 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID);
		Reanimation reanimation3 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID2);
		Reanimation reanimation4 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID3);
		if (reanimation != null)
		{
			reanimation.mFilterEffect = theFilterEffect;
		}
		if (reanimation2 != null)
		{
			reanimation2.mFilterEffect = theFilterEffect;
		}
		if (reanimation3 != null)
		{
			reanimation3.mFilterEffect = theFilterEffect;
		}
		if (reanimation4 != null)
		{
			reanimation4.mFilterEffect = theFilterEffect;
		}
	}

	public int ScaryPotterCountSunInPot(GridItem theScaryPot)
	{
		return theScaryPot.mSunCount;
	}

	public int ScaryPotterCountPots()
	{
		int num = 0;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ScaryPot)
			{
				num++;
			}
		}
		return num;
	}

	public void IZombieInitLevel()
	{
		mChallengeScore = 0;
		for (int i = 0; i < 5; i++)
		{
			GridItem newGridItem = GridItem.GetNewGridItem();
			newGridItem.mGridItemType = GridItemType.IzombieBrain;
			newGridItem.mGridX = 0;
			newGridItem.mGridY = i;
			newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, newGridItem.mGridY, 0);
			newGridItem.mGridItemCounter = 70;
			newGridItem.mPosX = (float)mBoard.GridToPixelX(newGridItem.mGridX, newGridItem.mGridY) - 40f;
			newGridItem.mPosY = (float)mBoard.GridToPixelY(newGridItem.mGridX, newGridItem.mGridY) + 40f;
			mBoard.mGridItems.Add(newGridItem);
		}
		CSIZombieLevel cSIZombieLevel = mApp.mCreativeLevel?.GetComponent<CSIZombieLevel>();
		if (cSIZombieLevel != null)
		{
			foreach (var (theSeedType, theGridX, theGridY) in cSIZombieLevel.mPlantInSquare)
			{
				IZombiePlacePlantInSquare(theSeedType, theGridX, theGridY);
			}
			foreach (var (theSeedType2, theCount, theGridY2) in cSIZombieLevel.mPlantInRow)
			{
				IZombiePlacePlants(theSeedType2, theCount, theGridY2);
			}
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie1)
		{
			IZombiePlacePlantInSquare(SeedType.Sunflower, 3, 2);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 3, 3);
			IZombiePlacePlants(SeedType.Sunflower, 7, -1);
			IZombiePlacePlants(SeedType.Squash, 3, -1);
			IZombiePlacePlants(SeedType.Peashooter, 6, -1);
			IZombiePlacePlants(SeedType.Snowpea, 2, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie2)
		{
			IZombiePlacePlantInSquare(SeedType.Spikeweed, 3, 0);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 2, 0);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 3, 3);
			IZombiePlacePlants(SeedType.Spikeweed, 1, 0);
			IZombiePlacePlants(SeedType.Peashooter, 1, 0);
			IZombiePlacePlants(SeedType.Snowpea, 2, 3);
			IZombiePlacePlants(SeedType.Sunflower, 1, 3);
			IZombiePlacePlants(SeedType.Sunflower, 4, -1);
			IZombiePlacePlants(SeedType.Spikeweed, 2, -1);
			IZombiePlacePlants(SeedType.Snowpea, 2, -1);
			IZombiePlacePlants(SeedType.Peashooter, 4, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie3)
		{
			IZombiePlacePlantInSquare(SeedType.Potatomine, 3, 0);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 2, 0);
			IZombiePlacePlantInSquare(SeedType.Potatomine, 2, 2);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 2, 4);
			IZombiePlacePlantInSquare(SeedType.Torchwood, 3, 3);
			IZombiePlacePlants(SeedType.Torchwood, 2, -1);
			IZombiePlacePlants(SeedType.Sunflower, 5, -1);
			IZombiePlacePlants(SeedType.Peashooter, 7, -1);
			IZombiePlacePlants(SeedType.Splitpea, 1, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie4)
		{
			IZombiePlacePlantInSquare(SeedType.Wallnut, 3, 0);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 2, 0);
			IZombiePlacePlantInSquare(SeedType.Wallnut, 3, 1);
			IZombiePlacePlantInSquare(SeedType.Wallnut, 3, 2);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 2, 2);
			IZombiePlacePlantInSquare(SeedType.Wallnut, 3, 3);
			IZombiePlacePlantInSquare(SeedType.Wallnut, 3, 4);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 2, 4);
			IZombiePlacePlants(SeedType.Peashooter, 1, 0);
			IZombiePlacePlants(SeedType.Snowpea, 1, 1);
			IZombiePlacePlants(SeedType.Fumeshroom, 2, 2);
			IZombiePlacePlants(SeedType.Snowpea, 1, 3);
			IZombiePlacePlants(SeedType.Peashooter, 1, 4);
			IZombiePlacePlants(SeedType.Peashooter, 2, -1);
			IZombiePlacePlants(SeedType.Sunflower, 4, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie5)
		{
			IZombiePlacePlantInSquare(SeedType.Sunflower, 3, 2);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 3, 3);
			IZombiePlacePlants(SeedType.Cactus, 1, 1);
			IZombiePlacePlants(SeedType.Cactus, 1, 4);
			IZombiePlacePlants(SeedType.Magnetshroom, 1, -1);
			IZombiePlacePlants(SeedType.Sunflower, 5, -1);
			IZombiePlacePlants(SeedType.Peashooter, 8, -1);
			IZombiePlacePlants(SeedType.Snowpea, 2, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie6)
		{
			IZombiePlacePlantInSquare(SeedType.Garlic, 4, 1);
			IZombiePlacePlantInSquare(SeedType.Garlic, 4, 3);
			IZombiePlacePlants(SeedType.Sunflower, 3, 1);
			IZombiePlacePlants(SeedType.Sunflower, 3, 3);
			IZombiePlacePlants(SeedType.Torchwood, 2, -1);
			IZombiePlacePlants(SeedType.Sunflower, 2, -1);
			IZombiePlacePlants(SeedType.Spikeweed, 3, -1);
			IZombiePlacePlants(SeedType.Snowpea, 1, -1);
			IZombiePlacePlants(SeedType.Peashooter, 5, -1);
			IZombiePlacePlants(SeedType.Squash, 2, -1);
			IZombiePlacePlants(SeedType.Kernelpult, 2, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie7)
		{
			IZombiePlacePlantInSquare(SeedType.Sunflower, 4, 2);
			IZombiePlacePlantInSquare(SeedType.Sunflower, 4, 4);
			IZombiePlacePlants(SeedType.Sunflower, 6, -1);
			IZombiePlacePlants(SeedType.Potatomine, 9, -1);
			IZombiePlacePlants(SeedType.Chomper, 8, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie8)
		{
			IZombiePlacePlants(SeedType.Wallnut, 3, -1);
			IZombiePlacePlants(SeedType.Magnetshroom, 2, -1);
			IZombiePlacePlants(SeedType.Peashooter, 8, -1);
			IZombiePlacePlants(SeedType.Squash, 2, -1);
			IZombiePlacePlants(SeedType.Potatomine, 2, -1);
			IZombiePlacePlants(SeedType.Sunflower, 8, -1);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombie9)
		{
			IZombiePlacePlantInSquare(SeedType.Tallnut, 5, 1);
			IZombiePlacePlantInSquare(SeedType.Torchwood, 5, 3);
			IZombiePlacePlants(SeedType.Potatomine, 4, 0);
			IZombiePlacePlants(SeedType.Sunflower, 2, 0);
			IZombiePlacePlants(SeedType.Sunflower, 2, 1);
			IZombiePlacePlants(SeedType.Threepeater, 1, 1);
			IZombiePlacePlants(SeedType.Snowpea, 1, 1);
			IZombiePlacePlants(SeedType.Splitpea, 1, 1);
			IZombiePlacePlants(SeedType.Chomper, 3, 2);
			IZombiePlacePlants(SeedType.Sunflower, 2, 2);
			IZombiePlacePlants(SeedType.Squash, 1, 2);
			IZombiePlacePlants(SeedType.Peashooter, 3, 3);
			IZombiePlacePlants(SeedType.Sunflower, 2, 3);
			IZombiePlacePlants(SeedType.Sunflower, 1, 4);
			IZombiePlacePlants(SeedType.Fumeshroom, 1, 4);
			IZombiePlacePlants(SeedType.Scaredyshroom, 1, 4);
			IZombiePlacePlants(SeedType.Starfruit, 1, 4);
			IZombiePlacePlants(SeedType.Splitpea, 1, 4);
			IZombiePlacePlants(SeedType.Magnetshroom, 1, 4);
		}
		else if (mApp.mGameMode == GameMode.PuzzleIZombieEndless)
		{
			int num = TodCommon.RandRangeInt(0, 4);
			int theMax = TodCommon.ClampInt(3 + mSurvivalStage / 2, 2, 6);
			int theMin = TodCommon.ClampInt(2 + mSurvivalStage / 3, 2, 4);
			int num2 = TodCommon.RandRangeInt(theMin, theMax);
			if (mSurvivalStage == 0)
			{
				num2 = 0;
			}
			else if (mSurvivalStage == 1)
			{
				num2 = 1;
			}
			else if (mSurvivalStage >= 10)
			{
				theMax = TodCommon.ClampInt(3 + mSurvivalStage / 2, 2, 7);
				theMin = TodCommon.ClampInt(2 + mSurvivalStage / 3, 2, 5);
				num2 = TodCommon.RandRangeInt(theMin, theMax);
			}
			int theCount2 = 8 - num2;
			IZombiePlacePlants(SeedType.Sunflower, theCount2, -1);
			IZombiePlacePlants(SeedType.Puffshroom, num2, -1);
			if (num == 0 && mSurvivalStage >= 1)
			{
				switch (TodCommon.RandRangeInt(0, 4))
				{
				case 0:
					IZombiePlacePlants(SeedType.Snowpea, 9, -1);
					IZombiePlacePlants(SeedType.Splitpea, 4, -1);
					IZombiePlacePlants(SeedType.Repeater, 4, -1);
					break;
				case 1:
					IZombiePlacePlants(SeedType.Potatomine, 9, -1);
					IZombiePlacePlants(SeedType.Chomper, 8, -1);
					break;
				case 2:
					IZombiePlacePlants(SeedType.Spikeweed, 9, -1);
					IZombiePlacePlants(SeedType.Starfruit, 8, -1);
					break;
				case 3:
					IZombiePlacePlants(SeedType.Fumeshroom, 9, -1);
					IZombiePlacePlants(SeedType.Magnetshroom, 8, -1);
					break;
				default:
					IZombiePlacePlants(SeedType.Scaredyshroom, 12, -1);
					IZombiePlacePlants(SeedType.Sunflower, 5, -1);
					break;
				}
			}
			else
			{
				switch (TodCommon.RandRangeInt(0, 5))
				{
				case 0:
				case 1:
				case 2:
					IZombiePlacePlants(SeedType.Wallnut, 1, -1);
					IZombiePlacePlants(SeedType.Torchwood, 1, -1);
					IZombiePlacePlants(SeedType.Potatomine, 1, -1);
					IZombiePlacePlants(SeedType.Chomper, 2, -1);
					IZombiePlacePlants(SeedType.Peashooter, 1, -1);
					IZombiePlacePlants(SeedType.Splitpea, 1, -1);
					IZombiePlacePlants(SeedType.Kernelpult, 1, -1);
					IZombiePlacePlants(SeedType.Threepeater, 1, -1);
					IZombiePlacePlants(SeedType.Snowpea, 1, -1);
					IZombiePlacePlants(SeedType.Squash, 1, -1);
					IZombiePlacePlants(SeedType.Fumeshroom, 1, -1);
					IZombiePlacePlants(SeedType.Umbrella, 1, -1);
					IZombiePlacePlants(SeedType.Starfruit, 1, -1);
					IZombiePlacePlants(SeedType.Magnetshroom, 1, -1);
					IZombiePlacePlants(SeedType.Spikeweed, 2, -1);
					break;
				case 3:
				case 4:
					IZombiePlacePlants(SeedType.Torchwood, 1, -1);
					IZombiePlacePlants(SeedType.Splitpea, 3, -1);
					IZombiePlacePlants(SeedType.Repeater, 1, -1);
					IZombiePlacePlants(SeedType.Kernelpult, 3, -1);
					IZombiePlacePlants(SeedType.Threepeater, 1, -1);
					IZombiePlacePlants(SeedType.Snowpea, 3, -1);
					IZombiePlacePlants(SeedType.Umbrella, 1, -1);
					IZombiePlacePlants(SeedType.Magnetshroom, 1, -1);
					IZombiePlacePlants(SeedType.Spikeweed, 3, -1);
					break;
				default:
					IZombiePlacePlants(SeedType.Potatomine, 4, -1);
					IZombiePlacePlants(SeedType.Chomper, 3, -1);
					IZombiePlacePlants(SeedType.Squash, 3, -1);
					IZombiePlacePlants(SeedType.Fumeshroom, 4, -1);
					IZombiePlacePlants(SeedType.Spikeweed, 3, -1);
					break;
				}
			}
		}
		mBoard.mBonusLawnMowersRemaining = 0;
	}

	public void DrawRain(Graphics g)
	{
		if ((mBoard.mCutScene != null && mBoard.mCutScene.IsBeforePreloading()) || !mApp.Is3DAccelerated())
		{
			return;
		}
		int num = ((mBoard.mX <= 0) ? (mBoard.mX / 100 * -100) : ((mBoard.mX + 100) / 100 * -100));
		int num2 = TodCommon.TodAnimateCurve(0, 100, mBoard.mEffectCounter % 100, 0, -100, TodCurves.Linear);
		int num3 = TodCommon.TodAnimateCurve(0, 20, mBoard.mEffectCounter % 20, -100, 0, TodCurves.Linear);
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int theX = num2 + i * 100 + num;
				int theY = num3 + j * 100;
				g.DrawImage(AtlasResources.IMAGE_RAIN, theX, theY);
			}
		}
		int num4 = TodCommon.TodAnimateCurve(0, 161, mBoard.mEffectCounter % 161, 0, -100, TodCurves.Linear);
		int num5 = TodCommon.TodAnimateCurve(0, 33, mBoard.mEffectCounter % 33, -100, 0, TodCurves.Linear);
		for (int k = 0; k < 5; k++)
		{
			for (int l = 0; l < 4; l++)
			{
				float num6 = 1.7f;
				float thePosX = ((float)num4 + (float)k * 100f) * num6 + (float)num;
				float thePosY = ((float)num5 + (float)l * 100f) * num6;
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_RAIN, thePosX, thePosY, num6, num6);
			}
		}
	}

	public void DrawWeather(Graphics g)
	{
		if (mBoard.HasRain())
		{
			DrawRain(g);
		}
		if (mBoard.HasStorm())
		{
			DrawStormNight(g);
		}
	}

	public void SquirrelUpdate()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Squirrel)
			{
				SquirrelUpdateOne(theGridItem);
			}
		}
		mChallengeScore = 7 - SquirrelCountUncaught();
		mBoard.mProgressMeterWidth = TodCommon.TodAnimateCurve(0, 7, mChallengeScore, 0, 150, TodCurves.Linear);
	}

	public int SquirrelCountUncaught()
	{
		int num = 0;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Squirrel && theGridItem.mGridItemState != GridItemState.SquirrelCaught && theGridItem.mGridItemState != GridItemState.SquirrelZombie)
			{
				num++;
			}
		}
		return num;
	}

	public void SquirrelStart()
	{
		TodWeightedGridArray[] array = new TodWeightedGridArray[Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY];
		int num = 0;
		for (int i = 0; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				array[num] = TodWeightedGridArray.GetNewTodWeightedGridArray();
				array[num].mX = i;
				array[num].mY = j;
				array[num].mWeight = 1;
				num++;
			}
		}
		TodWeightedGridArray todWeightedGridArray;
		GridItem newGridItem;
		for (int k = 0; k < 7; k++)
		{
			todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array, num);
			todWeightedGridArray.mWeight = 0;
			newGridItem = GridItem.GetNewGridItem();
			newGridItem.mGridItemType = GridItemType.Squirrel;
			newGridItem.mGridX = todWeightedGridArray.mX;
			newGridItem.mGridY = todWeightedGridArray.mY;
			newGridItem.mGridItemState = GridItemState.SquirrelWaiting;
			newGridItem.mGridItemCounter = TodCommon.RandRangeInt(100, 500);
			newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, newGridItem.mGridY, 1);
			mBoard.mGridItems.Add(newGridItem);
		}
		for (int l = 0; l < num; l++)
		{
			if (array[l].mX < 4)
			{
				array[l].mWeight = 0;
			}
		}
		todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array, num);
		newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.Squirrel;
		newGridItem.mGridX = todWeightedGridArray.mX;
		newGridItem.mGridY = todWeightedGridArray.mY;
		newGridItem.mGridItemState = GridItemState.SquirrelZombie;
		newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, newGridItem.mGridY, 1);
		mBoard.mGridItems.Add(newGridItem);
	}

	public void SquirrelFound(GridItem theSquirrel)
	{
		if (theSquirrel.mGridItemState == GridItemState.SquirrelZombie)
		{
			Zombie zombie = mBoard.AddZombieInRow(ZombieType.Normal, theSquirrel.mGridY, 0);
			zombie.mPosX = mBoard.GridToPixelX(theSquirrel.mGridX, theSquirrel.mGridY);
			theSquirrel.GridItemDie();
			mBoard.DisplayAdvice("[ADVICE_SQUIRREL_ZOMBIE]", MessageStyle.HintFast, AdviceType.None);
			return;
		}
		TodWeightedGridArray[] array = new TodWeightedGridArray[4];
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			int num2 = theSquirrel.mGridX;
			int num3 = theSquirrel.mGridY;
			switch (i)
			{
			case 0:
				num2--;
				break;
			case 1:
				num2++;
				break;
			case 2:
				num3--;
				break;
			case 3:
				num3++;
				break;
			}
			if (mBoard.GetSquirrelAt(num2, num3) == null)
			{
				Plant topPlantAt = mBoard.GetTopPlantAt(num2, num3, TopPlant.EatingOrder);
				if (topPlantAt != null)
				{
					array[num] = TodWeightedGridArray.GetNewTodWeightedGridArray();
					array[num].mX = num2;
					array[num].mY = num3;
					array[num].mWeight = 1;
					num++;
				}
			}
		}
		if (num > 0)
		{
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array, num);
			if (todWeightedGridArray.mX < theSquirrel.mGridX)
			{
				theSquirrel.mGridItemState = GridItemState.SquirrelRunningLeft;
			}
			else if (todWeightedGridArray.mX > theSquirrel.mGridX)
			{
				theSquirrel.mGridItemState = GridItemState.SquirrelRunningRight;
			}
			else if (todWeightedGridArray.mY < theSquirrel.mGridY)
			{
				theSquirrel.mGridItemState = GridItemState.SquirrelRunningUp;
			}
			else
			{
				theSquirrel.mGridItemState = GridItemState.SquirrelRunningDown;
			}
			theSquirrel.mGridItemCounter = 50;
			theSquirrel.mGridX = todWeightedGridArray.mX;
			theSquirrel.mGridY = todWeightedGridArray.mY;
			theSquirrel.mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, theSquirrel.mGridY, 1);
		}
		else
		{
			theSquirrel.mGridItemState = GridItemState.SquirrelCaught;
			theSquirrel.mGridItemCounter = 100;
			int num4 = SquirrelCountUncaught();
			if (num4 == 0)
			{
				mBoard.ClearAdvice(AdviceType.None);
				SpawnLevelAward(theSquirrel.mGridX, theSquirrel.mGridY);
			}
			else
			{
				string theAdvice = mApp.Pluralize(num4, "[ADVICE_SQUIRRELS_ONE_LEFT]", "[ADVICE_SQUIRRELS_LEFT]");
				mBoard.DisplayAdvice(theAdvice, MessageStyle.HintFast, AdviceType.None);
			}
		}
	}

	public void SquirrelPeek(GridItem theSquirrel)
	{
		theSquirrel.mGridItemCounter = 50;
		theSquirrel.mGridItemState = GridItemState.SquirrelPeeking;
	}

	public void SquirrelChew(GridItem theSquirrel)
	{
		theSquirrel.mGridItemCounter = TodCommon.RandRangeInt(100, 500);
		Plant topPlantAt = mBoard.GetTopPlantAt(theSquirrel.mGridX, theSquirrel.mGridY, TopPlant.EatingOrder);
		if (topPlantAt != null)
		{
			float num = mBoard.GridToPixelX(theSquirrel.mGridX, theSquirrel.mGridY);
			float num2 = mBoard.GridToPixelY(theSquirrel.mGridX, theSquirrel.mGridY);
			mApp.AddTodParticle(num + 40f, num2 + 40f, topPlantAt.mRenderOrder + 1, ParticleEffect.WallnutEatSmall);
			topPlantAt.mBeghouledFlashCountdown = Math.Max(topPlantAt.mBeghouledFlashCountdown, 25);
		}
	}

	public void SquirrelUpdateOne(GridItem theSquirrel)
	{
		if (theSquirrel.mGridItemCounter > 0)
		{
			theSquirrel.mGridItemCounter--;
		}
		if (theSquirrel.mGridItemState == GridItemState.SquirrelWaiting || theSquirrel.mGridItemState == GridItemState.SquirrelZombie)
		{
			if (mBoard.GetTopPlantAt(theSquirrel.mGridX, theSquirrel.mGridY, TopPlant.EatingOrder) == null)
			{
				SquirrelFound(theSquirrel);
			}
			if (theSquirrel.mGridItemCounter == 0)
			{
				if (TodCommon.RandRangeInt(0, 1) == 0 || theSquirrel.mGridItemState == GridItemState.SquirrelZombie)
				{
					SquirrelChew(theSquirrel);
				}
				else
				{
					SquirrelPeek(theSquirrel);
				}
			}
		}
		if ((theSquirrel.mGridItemState == GridItemState.SquirrelPeeking || theSquirrel.mGridItemState == GridItemState.SquirrelRunningUp || theSquirrel.mGridItemState == GridItemState.SquirrelRunningDown || theSquirrel.mGridItemState == GridItemState.SquirrelRunningLeft || theSquirrel.mGridItemState == GridItemState.SquirrelRunningRight) && theSquirrel.mGridItemCounter == 0)
		{
			theSquirrel.mGridItemState = GridItemState.SquirrelWaiting;
			theSquirrel.mGridItemCounter = TodCommon.RandRangeInt(100, 500);
		}
		if (theSquirrel.mGridItemState == GridItemState.SquirrelCaught && theSquirrel.mGridItemCounter == 0)
		{
			theSquirrel.GridItemDie();
		}
	}

	public void IZombieSetupPlant(Plant thePlant)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(thePlant.mBodyReanimID);
		Reanimation reanimation2 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID);
		Reanimation reanimation3 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID2);
		Reanimation reanimation4 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID3);
		if (reanimation != null)
		{
			reanimation.mAnimRate = 0f;
		}
		if (reanimation2 != null)
		{
			reanimation2.mAnimRate = 0f;
		}
		if (reanimation3 != null)
		{
			reanimation3.mAnimRate = 0f;
		}
		if (reanimation4 != null)
		{
			reanimation4.mAnimRate = 0f;
		}
		if (thePlant.mSeedType == SeedType.Potatomine)
		{
			thePlant.PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_armed, ReanimLoopType.Loop, 0, 0f);
			thePlant.mState = PlantState.PotatoArmed;
		}
		thePlant.mBlinkCountdown = 0;
		thePlant.UpdateReanim();
	}

	public void UpdateRain()
	{
		mRainCounter--;
		if (mRainCounter < 0 && !mBoard.mCutScene.IsBeforePreloading())
		{
			ReanimationType theReanimationType = ReanimationType.RainSplash;
			float theX = TodCommon.RandRangeFloat(40f, 740f);
			float theY = TodCommon.RandRangeFloat(90f, 240f);
			Reanimation reanimation = mApp.AddReanimation(theX, theY, 200000, theReanimationType);
			int theAlpha = TodCommon.RandRangeInt(100, 200);
			float num = TodCommon.RandRangeFloat(0.7f, 1.2f);
			reanimation.mColorOverride = new SexyColor(255, 255, 255, theAlpha);
			reanimation.OverrideScale(num, num);
			theReanimationType = ReanimationType.RainCircle;
			theX = TodCommon.RandRangeFloat(40f, 740f);
			theY = TodCommon.RandRangeFloat(290f, 410f);
			reanimation = mApp.AddReanimation(theX, theY, 200000, theReanimationType);
			theAlpha = TodCommon.RandRangeInt(50, 150);
			num = TodCommon.RandRangeFloat(0.7f, 1.1f);
			reanimation.mColorOverride = new SexyColor(255, 255, 255, theAlpha);
			reanimation.OverrideScale(num, num);
			theReanimationType = ReanimationType.RainSplash;
			theX = TodCommon.RandRangeFloat(40f, 740f);
			theY = TodCommon.RandRangeFloat(450f, 580f);
			reanimation = mApp.AddReanimation(theX, theY, 200000, theReanimationType);
			theAlpha = TodCommon.RandRangeInt(100, 200);
			num = TodCommon.RandRangeFloat(0.7f, 1.2f);
			reanimation.mColorOverride = new SexyColor(255, 255, 255, theAlpha);
			reanimation.OverrideScale(num, num);
			mRainCounter = TodCommon.RandRangeInt(10, 20);
		}
	}

	public bool IZombieEatBrain(Zombie theZombie)
	{
		GridItem gridItem = IZombieGetBrainTarget(theZombie);
		if (gridItem == null)
		{
			return false;
		}
		theZombie.StartEating();
		gridItem.mGridItemCounter--;
		if (gridItem.mGridItemCounter <= 0)
		{
			mApp.PlaySample(Resources.SOUND_GULP);
			gridItem.GridItemDie();
			IZombieScoreBrain(gridItem);
		}
		return true;
	}

	public GridItem IZombieGetBrainTarget(Zombie theZombie)
	{
		if (theZombie.mZombieType == ZombieType.Digger && theZombie.mZombiePhase == ZombiePhase.DiggerWalking)
		{
			return null;
		}
		if (theZombie.mZombieType == ZombieType.Bungee)
		{
			return null;
		}
		if (theZombie.IsWalkingBackwards())
		{
			return null;
		}
		TRect tRect = theZombie.GetZombieAttackRect();
		if (theZombie.mZombiePhase == ZombiePhase.PolevaulterPreVault)
		{
			tRect = new TRect(50 + theZombie.mX, 0, 20, 115);
		}
		if (theZombie.mZombieType == ZombieType.Balloon)
		{
			tRect.mX += 25;
		}
		if (tRect.mX > Constants.IZombieBrainPosition)
		{
			return null;
		}
		GridItem gridItemAt = mBoard.GetGridItemAt(GridItemType.IzombieBrain, 0, theZombie.mRow);
		if (gridItemAt == null)
		{
			return null;
		}
		if (gridItemAt.mGridItemState == GridItemState.BrainSquished)
		{
			return null;
		}
		return gridItemAt;
	}

	public void IZombiePlacePlantInSquare(SeedType theSeedType, int theGridX, int theGridY)
	{
		if (mBoard.CanPlantAt(theGridX, theGridY, theSeedType) == PlantingReason.Ok)
		{
			Plant thePlant = mBoard.NewPlant(theGridX, theGridY, theSeedType, SeedType.None);
			IZombieSetupPlant(thePlant);
		}
	}

	public void AdvanceCrazyDaveDialog()
	{
		if (mBoard.IsScaryPotterDaveTalking() && mApp.mCrazyDaveMessageIndex != -1)
		{
			if (!mApp.AdvanceCrazyDaveText())
			{
				mApp.CrazyDaveLeave();
			}
			else if (mApp.mCrazyDaveMessageIndex == 2702 || mApp.mCrazyDaveMessageIndex == 2801)
			{
				ScaryPotterPopulate();
				mApp.PlayFoley(FoleyType.Plant);
				mBoard.PlaceRake();
			}
		}
	}

	public void BeghouledFlashPlant(int x, int y, int theSwap1X, int theSwap1Y, int theSwap2X, int theSwap2Y)
	{
		int num = x;
		int num2 = y;
		if (num == theSwap1X && num2 == theSwap1Y)
		{
			num = theSwap2X;
			num2 = theSwap2Y;
		}
		else if (num == theSwap2X && num2 == theSwap2Y)
		{
			num = theSwap1X;
			num2 = theSwap1Y;
		}
		Plant topPlantAt = mBoard.GetTopPlantAt(num, num2, TopPlant.OnlyNormalPosition);
		if (topPlantAt != null)
		{
			topPlantAt.mBeghouledFlashCountdown = Math.Max(topPlantAt.mBeghouledFlashCountdown, 300);
		}
	}

	public void BeghouledFlashAMatch()
	{
		BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
		LoadBeghouledBoardState(newBeghouledBoardState);
		if (mApp.mGameMode == GameMode.ChallengeBeghouled)
		{
			for (int i = 0; i <= 4; i++)
			{
				for (int j = 0; j <= 7; j++)
				{
					if (j < 7 && BeghouledFlashFromBoardState(newBeghouledBoardState, j, i, j + 1, i))
					{
						newBeghouledBoardState.PrepareForReuse();
						return;
					}
					if (i < 4 && BeghouledFlashFromBoardState(newBeghouledBoardState, j, i, j, i + 1))
					{
						newBeghouledBoardState.PrepareForReuse();
						return;
					}
				}
			}
		}
		else if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			for (int k = 0; k < 4; k++)
			{
				for (int l = 0; l < 7; l++)
				{
					if (BeghouledTwistFlashMatch(newBeghouledBoardState, l, k))
					{
						newBeghouledBoardState.PrepareForReuse();
						return;
					}
				}
			}
		}
		newBeghouledBoardState.PrepareForReuse();
	}

	public bool BeghouledFlashFromBoardState(BeghouledBoardState theBoardState, int theSwap1X, int theSwap1Y, int theSwap2X, int theSwap2Y)
	{
		Debug.ASSERT(theSwap1X >= 0 && theSwap1X < 8 && theSwap1Y >= 0 && theSwap1Y < 5);
		Debug.ASSERT(theSwap2X >= 0 && theSwap2X < 8 && theSwap2Y >= 0 && theSwap2Y < 5);
		if (mBeghouledEated[theSwap1X, theSwap1Y] || mBeghouledEated[theSwap2X, theSwap2Y])
		{
			return false;
		}
		SeedType seedType = theBoardState.mSeedType[theSwap1X, theSwap1Y];
		SeedType seedType2 = theBoardState.mSeedType[theSwap2X, theSwap2Y];
		theBoardState.mSeedType[theSwap1X, theSwap1Y] = seedType2;
		theBoardState.mSeedType[theSwap2X, theSwap2Y] = seedType;
		bool flag = false;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int num = BeghouledHorizontalMatchLength(j, i, theBoardState);
				if (num >= 3)
				{
					BeghouledFlashPlant(j, i, theSwap1X, theSwap1Y, theSwap2X, theSwap2Y);
					BeghouledFlashPlant(j + 1, i, theSwap1X, theSwap1Y, theSwap2X, theSwap2Y);
					BeghouledFlashPlant(j + 2, i, theSwap1X, theSwap1Y, theSwap2X, theSwap2Y);
					flag = true;
					break;
				}
				int num2 = BeghouledVerticalMatchLength(j, i, theBoardState);
				if (num2 >= 3)
				{
					BeghouledFlashPlant(j, i, theSwap1X, theSwap1Y, theSwap2X, theSwap2Y);
					BeghouledFlashPlant(j, i + 1, theSwap1X, theSwap1Y, theSwap2X, theSwap2Y);
					BeghouledFlashPlant(j, i + 2, theSwap1X, theSwap1Y, theSwap2X, theSwap2Y);
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		theBoardState.mSeedType[theSwap1X, theSwap1Y] = seedType;
		theBoardState.mSeedType[theSwap2X, theSwap2Y] = seedType2;
		return flag;
	}

	public void IZombiePlantDropRemainingSun(Plant thePlant)
	{
		if (thePlant.mSeedType == SeedType.Sunflower)
		{
			int num = thePlant.mPlantHealth / 40 + 1;
			for (int i = 0; i < num; i++)
			{
				mBoard.AddCoin(thePlant.mX + 5 * i, thePlant.mY, CoinType.Sun, CoinMotion.FromPlant);
			}
		}
	}

	public void IZombieSquishBrain(GridItem theBrain)
	{
		theBrain.mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, theBrain.mGridY, 0);
		theBrain.mGridItemState = GridItemState.BrainSquished;
		theBrain.mGridItemCounter = 500;
		theBrain.mApp.PlayFoley(FoleyType.Squish);
		IZombieScoreBrain(theBrain);
	}

	public void IZombieScoreBrain(GridItem theBrain)
	{
		mBoard.mChallenge.mChallengeScore++;
		mBoard.mProgressMeterWidth = TodCommon.TodAnimateCurve(0, 5, mBoard.mChallenge.mChallengeScore, 0, 150, TodCurves.Linear);
		if (mBoard.mChallenge.mChallengeScore == 5)
		{
			if (mApp.IsEndlessIZombie(mApp.mGameMode))
			{
				PuzzlePhaseComplete(0, theBrain.mGridY);
			}
			else
			{
				mBoard.mChallenge.SpawnLevelAward(0, theBrain.mGridY);
			}
		}
		if (mBoard.mChallenge.mChallengeScore != 5 || !PuzzleIsAwardStage())
		{
			mBoard.DropLootPiece((int)theBrain.mPosX + 40, (int)theBrain.mPosY - 50, 12);
		}
	}

	public void LastStandUpate()
	{
		if (mBoard.mNextSurvivalStageCounter == 0 && mChallengeState == ChallengeState.Normal && mBoard.mStoreButton.mBtnNoDraw)
		{
			if (mSurvivalStage == 0)
			{
				mBoard.mStoreButton.mBtnNoDraw = false;
				mBoard.mStoreButton.mDisabled = false;
				mBoard.mStoreButton.SetLabel("[START_ONSLAUGHT]");
				mBoard.mStoreButton.Resize(Constants.LastStandButtonRect.mX, Constants.LastStandButtonRect.mY, Constants.LastStandButtonRect.mWidth, Constants.LastStandButtonRect.mHeight);
			}
			else
			{
				mBoard.mStoreButton.mBtnNoDraw = false;
				mBoard.mStoreButton.mDisabled = false;
				mBoard.mStoreButton.SetLabel("[CONTINUE_ONSLAUGHT]");
				mBoard.mStoreButton.Resize(Constants.LastStandButtonRect.mX, Constants.LastStandButtonRect.mY, Constants.LastStandButtonRect.mWidth, Constants.LastStandButtonRect.mHeight);
			}
		}
		if (mChallengeState == ChallengeState.LastStandOnslaught && mApp.mGameScene == GameScenes.Playing)
		{
			mChallengeStateCounter++;
		}
	}

	public void WhackAZombiePlaceGraves(int theGraveCount)
	{
		int num = 0;
		for (int i = 0; i < aPicks.Length; i++)
		{
			aPicks[i].Reset();
		}
		for (int j = 3; j < Constants.GRIDSIZEX; j++)
		{
			for (int k = 0; k < Constants.MAX_GRIDSIZEY; k++)
			{
				if (mBoard.CanAddGraveStoneAt(j, k))
				{
					Plant topPlantAt = mBoard.GetTopPlantAt(j, k, TopPlant.Any);
					if (topPlantAt != null)
					{
						aPicks[num].mWeight = 1;
					}
					else
					{
						aPicks[num].mWeight = 100000;
					}
					aPicks[num].mX = j;
					aPicks[num].mY = k;
					num++;
				}
			}
		}
		if (theGraveCount > num)
		{
			theGraveCount = num;
		}
		if (num == 0)
		{
			return;
		}
		for (int l = 0; l < theGraveCount; l++)
		{
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(aPicks, num);
			int count = mBoard.mPlants.Count;
			for (int m = 0; m < count; m++)
			{
				Plant plant = mBoard.mPlants[m];
				if (!plant.mDead && plant.mPlantCol == todWeightedGridArray.mX && plant.mRow == todWeightedGridArray.mY)
				{
					plant.Die();
				}
			}
			mBoard.AddAGraveStone(todWeightedGridArray.mX, todWeightedGridArray.mY);
			todWeightedGridArray.mWeight = 0;
		}
	}

	public bool BeghouledTwistSquareFromMouse(int theMouseX, int theMouseY, ref int theGridX, ref int theGridY)
	{
		theGridX = mBoard.PixelToGridX(theMouseX - 40, theMouseY - 40);
		theGridY = mBoard.PixelToGridY(theMouseX - 40, theMouseY - 40);
		if (theGridX == -1 || theGridY == -1 || theGridX > 6 || theGridY > 3)
		{
			theGridX = -1;
			theGridY = -1;
			return false;
		}
		return true;
	}

	public bool BeghouledTwistValidMove(int theGridX, int theGridY, BeghouledBoardState theBoardState)
	{
		if (theGridX == -1 || theGridY == -1 || theGridX > 6 || theGridY > 3)
		{
			return false;
		}
		SeedType seedType = theBoardState.mSeedType[theGridX, theGridY];
		SeedType seedType2 = theBoardState.mSeedType[theGridX + 1, theGridY];
		SeedType seedType3 = theBoardState.mSeedType[theGridX, theGridY + 1];
		SeedType seedType4 = theBoardState.mSeedType[theGridX + 1, theGridY + 1];
		if (seedType != SeedType.None && seedType2 != SeedType.None && seedType3 != SeedType.None)
		{
			return seedType4 != SeedType.None;
		}
		return false;
	}

	public void BeghouledTwistMouseDown(int x, int y)
	{
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		if (mBoard.HasLevelAwardDropped())
		{
			return;
		}
		BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
		LoadBeghouledBoardState(newBeghouledBoardState);
		int theGridX = 0;
		int theGridY = 0;
		if (!BeghouledTwistSquareFromMouse(x, y, ref theGridX, ref theGridY) || !BeghouledTwistValidMove(theGridX, theGridY, newBeghouledBoardState))
		{
			newBeghouledBoardState.PrepareForReuse();
			return;
		}
		Plant topPlantAt = mBoard.GetTopPlantAt(theGridX, theGridY, TopPlant.Any);
		Plant topPlantAt2 = mBoard.GetTopPlantAt(theGridX + 1, theGridY, TopPlant.Any);
		Plant topPlantAt3 = mBoard.GetTopPlantAt(theGridX, theGridY + 1, TopPlant.Any);
		Plant topPlantAt4 = mBoard.GetTopPlantAt(theGridX + 1, theGridY + 1, TopPlant.Any);
		if (!BeghouledTwistMoveCausesMatch(theGridX, theGridY, newBeghouledBoardState))
		{
			topPlantAt.mX = mBoard.GridToPixelX(topPlantAt.mPlantCol, topPlantAt.mRow) + 20;
			topPlantAt2.mY = mBoard.GridToPixelY(topPlantAt2.mPlantCol, topPlantAt2.mRow) + 20;
			topPlantAt3.mY = mBoard.GridToPixelY(topPlantAt3.mPlantCol, topPlantAt3.mRow) - 20;
			topPlantAt4.mX = mBoard.GridToPixelX(topPlantAt4.mPlantCol, topPlantAt4.mRow) - 20;
			mApp.PlayFoley(FoleyType.Floop);
			newBeghouledBoardState.PrepareForReuse();
		}
		else
		{
			topPlantAt.mPlantCol++;
			topPlantAt.mRenderOrder = topPlantAt.CalcRenderOrder();
			topPlantAt2.mRow++;
			topPlantAt2.mRenderOrder = topPlantAt2.CalcRenderOrder();
			topPlantAt3.mRow--;
			topPlantAt3.mRenderOrder = topPlantAt3.CalcRenderOrder();
			topPlantAt4.mPlantCol--;
			topPlantAt4.mRenderOrder = topPlantAt4.CalcRenderOrder();
			BeghouledStartFalling(ChallengeState.BeghouledMoving);
			newBeghouledBoardState.PrepareForReuse();
		}
	}

	public bool BeghouledTwistMoveCausesMatch(int theGridX, int theGridY, BeghouledBoardState theBoardState)
	{
		if (!BeghouledTwistValidMove(theGridX, theGridY, theBoardState))
		{
			return false;
		}
		SeedType seedType = theBoardState.mSeedType[theGridX, theGridY];
		SeedType seedType2 = theBoardState.mSeedType[theGridX + 1, theGridY];
		SeedType seedType3 = theBoardState.mSeedType[theGridX, theGridY + 1];
		SeedType seedType4 = theBoardState.mSeedType[theGridX + 1, theGridY + 1];
		theBoardState.mSeedType[theGridX, theGridY] = seedType3;
		theBoardState.mSeedType[theGridX + 1, theGridY] = seedType;
		theBoardState.mSeedType[theGridX, theGridY + 1] = seedType4;
		theBoardState.mSeedType[theGridX + 1, theGridY + 1] = seedType2;
		bool result = BeghouledBoardHasMatch(theBoardState);
		theBoardState.mSeedType[theGridX, theGridY] = seedType;
		theBoardState.mSeedType[theGridX + 1, theGridY] = seedType2;
		theBoardState.mSeedType[theGridX, theGridY + 1] = seedType3;
		theBoardState.mSeedType[theGridX + 1, theGridY + 1] = seedType4;
		return result;
	}

	public bool BeghouledTwistFlashMatch(BeghouledBoardState theBoardState, int theGridX, int theGridY)
	{
		if (!BeghouledTwistMoveCausesMatch(theGridX, theGridY, theBoardState))
		{
			return false;
		}
		Plant topPlantAt = mBoard.GetTopPlantAt(theGridX, theGridY, TopPlant.Any);
		Plant topPlantAt2 = mBoard.GetTopPlantAt(theGridX + 1, theGridY, TopPlant.Any);
		Plant topPlantAt3 = mBoard.GetTopPlantAt(theGridX, theGridY + 1, TopPlant.Any);
		Plant topPlantAt4 = mBoard.GetTopPlantAt(theGridX + 1, theGridY + 1, TopPlant.Any);
		topPlantAt.mBeghouledFlashCountdown = Math.Max(topPlantAt.mBeghouledFlashCountdown, 300);
		topPlantAt2.mBeghouledFlashCountdown = Math.Max(topPlantAt2.mBeghouledFlashCountdown, 300);
		topPlantAt3.mBeghouledFlashCountdown = Math.Max(topPlantAt3.mBeghouledFlashCountdown, 300);
		topPlantAt4.mBeghouledFlashCountdown = Math.Max(topPlantAt4.mBeghouledFlashCountdown, 300);
		return true;
	}

	public void BeghouledCancelMatchFlashing()
	{
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead)
			{
				plant.mBeghouledFlashCountdown = Math.Min(plant.mBeghouledFlashCountdown, 25);
			}
		}
	}

	public void BeghouledStartFalling(ChallengeState theChallengeState)
	{
		mChallengeStateCounter = 100;
		mChallengeState = theChallengeState;
		BeghouledCancelMatchFlashing();
		mBoard.ClearAdvice(AdviceType.BeghouledNoMoves);
	}

	public void BeghouledFillHoles(BeghouledBoardState theBoardState, bool theAllowMatches)
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				if (theBoardState.mSeedType[i, j] == SeedType.None && !mBeghouledEated[i, j])
				{
					SeedType seedType = BeghouledPickSeed(i, j, theBoardState, theAllowMatches);
					theBoardState.mSeedType[i, j] = seedType;
				}
			}
		}
	}

	public void BeghouledMakeStartBoard()
	{
		BeghouledBoardState newBeghouledBoardState = BeghouledBoardState.GetNewBeghouledBoardState();
		LoadBeghouledBoardState(newBeghouledBoardState);
		BeghouledBoardState newBeghouledBoardState2 = BeghouledBoardState.GetNewBeghouledBoardState();
		LoadBeghouledBoardState(newBeghouledBoardState2);
		BeghouledFillHoles(newBeghouledBoardState2, theAllowMatches: false);
		Debug.ASSERT(!BeghouledBoardHasMatch(newBeghouledBoardState2));
		BeghouledCreatePlants(newBeghouledBoardState, newBeghouledBoardState2);
		newBeghouledBoardState.PrepareForReuse();
		newBeghouledBoardState2.PrepareForReuse();
	}

	public void BeghouledCreatePlants(BeghouledBoardState theOldBoardState, BeghouledBoardState theNewBoardState)
	{
		for (int i = 0; i < 8; i++)
		{
			int num = 0;
			for (int num2 = 4; num2 >= 0; num2--)
			{
				if (theOldBoardState.mSeedType[i, num2] == SeedType.None && theNewBoardState.mSeedType[i, num2] != SeedType.None)
				{
					num++;
					Plant plant = mBoard.NewPlant(i, num2, theNewBoardState.mSeedType[i, num2], SeedType.None);
					plant.mY = 80 - num * 100;
					BeghouledStartFalling(ChallengeState.BeghouledFalling);
				}
			}
		}
	}

	public void PuzzlePhaseComplete(int theGridX, int theGridY)
	{
		if (PuzzleIsAwardStage())
		{
			int num = TodCommon.RandRangeInt(0, 99);
			CoinType theCoinType;
			if (num < 15)
			{
				theCoinType = ((!mApp.mZenGarden.CanDropPottedPlantLoot()) ? CoinType.AwardMoneyBag : CoinType.AwardPresent);
			}
			else if (num >= 30)
			{
				theCoinType = CoinType.AwardBagDiamond;
			}
			else
			{
				theCoinType = ((!mApp.mZenGarden.CanDropChocolate()) ? CoinType.AwardMoneyBag : CoinType.AwardChocolate);
			}
			float num2 = mBoard.GridToPixelX(theGridX, theGridY) + 40;
			float num3 = mBoard.GridToPixelY(theGridX, theGridY) + 40;
			mBoard.AddCoin((int)num2, (int)num3, theCoinType, CoinMotion.Coin);
		}
		else
		{
			mBoard.FadeOutLevel();
		}
	}

	public Zombie IZombiePlaceZombie(ZombieType theZombieType, int theGridX, int theGridY)
	{
		Zombie zombie = mBoard.AddZombieInRow(theZombieType, theGridY, 0);
		if (theZombieType == ZombieType.Bungee)
		{
			zombie.mTargetCol = theGridX;
			zombie.SetRow(theGridY);
			zombie.mPosX = mBoard.GridToPixelX(theGridX, theGridY);
			zombie.mPosY = zombie.GetPosYBasedOnRow(theGridY);
			zombie.mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, theGridY, 7);
			return zombie;
		}
		zombie.mPosX = (float)mBoard.GridToPixelX(theGridX, theGridY) - 30f;
		return zombie;
	}

	public void WhackAZombieUpdate()
	{
		if (mBoard.mSunMoney > 0 && mBoard.mTutorialState == TutorialState.Off)
		{
			mBoard.SetTutorialState(TutorialState.WhackAZombieBeforePickSeed);
			mBoard.mTutorialTimer = 1500;
		}
		if (mBoard.mTutorialState == TutorialState.WhackAZombieBeforePickSeed && mBoard.mTutorialTimer == 0)
		{
			mBoard.SetTutorialState(TutorialState.WhackAZombiePickSeed);
			mBoard.mTutorialTimer = 400;
		}
		if (mBoard.mTutorialState == TutorialState.WhackAZombiePickSeed && mBoard.mTutorialTimer == 0)
		{
			mBoard.SetTutorialState(TutorialState.WhackAZombieCompleted);
		}
	}

	public void LastStandCompletedStage()
	{
		mApp.PlaySample(Resources.SOUND_HUGE_WAVE);
		mChallengeState = ChallengeState.Normal;
		mBoard.mSeedBank.RefreshAllPackets();
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead)
			{
				if (plant.mState == PlantState.ChomperDigesting)
				{
					plant.mStateCountdown = Math.Min(plant.mStateCountdown, 200);
				}
				if (plant.mState == PlantState.CobcannonArming)
				{
					plant.mStateCountdown = Math.Min(plant.mStateCountdown, 200);
				}
				if (plant.mState == PlantState.MagnetshroomSucking || plant.mState == PlantState.MagnetshroomCharging)
				{
					plant.mStateCountdown = Math.Min(plant.mStateCountdown, 200);
				}
			}
		}
		int survivalFlagsCompleted = mBoard.GetSurvivalFlagsCompleted();
		string theStringToSubstitute = mApp.Pluralize(survivalFlagsCompleted, "[ONE_FLAG]", "[COUNT_FLAGS]");
		string theAdvice = TodCommon.TodReplaceString("[SUCCESSFULLY_DEFENDED]", "{FLAGS}", theStringToSubstitute);
		mBoard.DisplayAdvice(theAdvice, MessageStyle.BigMiddleFast, AdviceType.None);
		mSurvivalStage++;
		mBoard.mLevelComplete = false;
		mBoard.InitZombieWaves();
	}

	public void TreeOfWisdomUpdate()
	{
		GridItem theGridItem = null;
		int index = -1;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			TreeOfWisdomToolUpdate(theGridItem);
		}
		if (mApp.mGameScene == GameScenes.Playing && mChallengeStateCounter > 0)
		{
			mChallengeStateCounter--;
		}
		if (mChallengeStateCounter == 0)
		{
			if (mChallengeState == ChallengeState.TreeJustGrew)
			{
				TreeOfWisdomGiveWisdom();
			}
			else if (mChallengeState == ChallengeState.TreeWaitingToBabble)
			{
				TreeOfWisdomBabble();
			}
			else if (mChallengeState == ChallengeState.TreeBabbling || mChallengeState == ChallengeState.TreeGiveWisdom)
			{
				if (mTreeOfWisdomTalkIndex == 500)
				{
					TreeOfWisdomSayRepeat();
				}
				else
				{
					mChallengeState = ChallengeState.TreeWaitingToBabble;
					mChallengeStateCounter = TodCommon.RandRangeInt(700, 1000);
				}
			}
		}
		for (int i = 0; i < 6; i++)
		{
			Reanimation reanimation = mApp.ReanimationGet(mReanimCloud[i]);
			if (mCloudCounter[i] > 0)
			{
				mCloudCounter[i]--;
				if (mCloudCounter[i] == 0)
				{
					reanimation.mLoopCount = 0;
					reanimation.mAnimTime = 0f;
					reanimation.mAnimRate = 0.2f;
				}
			}
			else if (reanimation.mLoopCount > 0)
			{
				mCloudCounter[i] = TodCommon.RandRangeInt(2000, 4000);
			}
			reanimation.Update();
		}
	}

	public void TreeOfWisdomFertilize()
	{
		GridItem newGridItem = GridItem.GetNewGridItem();
		mBoard.mGridItems.Add(newGridItem);
		newGridItem.mPosX = 340f;
		newGridItem.mPosY = 300f;
		newGridItem.mGridItemType = GridItemType.ZenTool;
		newGridItem.mGridX = 0;
		newGridItem.mGridY = 0;
		newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.AboveUI, 0, 0);
		Reanimation reanimation = mApp.AddReanimation(340f, 300f, 0, ReanimationType.TreeFood);
		reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
		newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation);
		newGridItem.mGridItemState = GridItemState.ZenToolFertilizer;
		mApp.PlayFoley(FoleyType.Fertilizer);
		mApp.mPlayerInfo.mPurchases[28]--;
		mChallengeState = ChallengeState.Normal;
		mBoard.ClearCursor();
	}

	public void TreeOfWisdomInit()
	{
		Reanimation reanimation = mApp.AddReanimation(-239.5f, -59.5f, 0, ReanimationType.TreeOfWisdom);
		reanimation.mIsAttachment = true;
		reanimation.AssignRenderGroupToPrefix("bg", 1);
		reanimation.AssignRenderGroupToPrefix("tree", 2);
		reanimation.AssignRenderGroupToPrefix("grass", 3);
		reanimation.AssignRenderGroupToPrefix("overlay", 4);
		reanimation.AssignRenderGroupToPrefix("leaf", 4);
		reanimation.AssignRenderGroupToPrefix("bunch", 4);
		reanimation.SetTruncateDisappearingFrames(null, theTruncateDisappearingFrames: false);
		mReanimChallenge = mApp.ReanimationGetID(reanimation);
		int num = TodCommon.ClampInt(TreeOfWisdomGetSize(), 1, 50);
		reanimation.PlayReanim($"anim_grow{num}", ReanimLoopType.PlayOnceAndHold, 0, 18f);
		if (num == 1 && mApp.mPlayerInfo.mPurchases[28] < 1000)
		{
			reanimation.mFrameCount += reanimation.mFrameStart;
			reanimation.mFrameStart = 0;
		}
		else
		{
			reanimation.mAnimTime = 0.99f;
		}
		for (int i = 0; i < 6; i++)
		{
			Reanimation reanimation2 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.TreeOfWisdomCloud);
			reanimation2.PlayReanim($"Cloud{i + 1}", ReanimLoopType.PlayOnceAndHold, 0, 0f);
			reanimation2.mIsAttachment = true;
			mReanimCloud[i] = mApp.ReanimationGetID(reanimation2);
			int num2 = TodCommon.RandRangeInt(-6000, 2000);
			mCloudCounter[i] = num2;
			if (num2 < 0)
			{
				reanimation2.mAnimTime = (float)(-num2) / 6000f;
				reanimation2.mAnimRate = 0.2f;
				mCloudCounter[i] = 0;
			}
			else
			{
				reanimation2.mAnimRate = 0f;
			}
		}
		mChallengeState = ChallengeState.TreeWaitingToBabble;
		mChallengeStateCounter = TodCommon.RandRangeInt(700, 1500);
	}

	public int TreeOfWisdomGetSize()
	{
		return mApp.mPlayerInfo.mChallengeRecords[48];
	}

	public void TreeOfWisdomDraw(Graphics g)
	{
		bool flag = TreeOfWisdomMouseOn(mApp.mWidgetManager.mLastMouseX - mBoard.mX, mApp.mWidgetManager.mLastMouseY - mBoard.mY, posScaled: false);
		Reanimation reanimation = mApp.ReanimationTryToGet(mReanimChallenge);
		if (reanimation == null)
		{
			return;
		}
		reanimation.SetPosition(100f * Constants.S - ((float)Constants.BOARD_WIDTH - 800f * Constants.S) / 2f, 2f);
		reanimation.mEnableExtraAdditiveDraw = false;
		reanimation.DrawRenderGroup(g, 1);
		for (int i = 0; i < 6; i++)
		{
			mApp.ReanimationGet(mReanimCloud[i]).Draw(g);
		}
		int num = TreeOfWisdomGetSize();
		if (flag)
		{
			reanimation.mExtraOverlayColor = new SexyColor(255, 255, 255, (num < 18) ? 128 : 48);
			reanimation.mEnableExtraAdditiveDraw = true;
		}
		else
		{
			reanimation.mEnableExtraAdditiveDraw = false;
		}
		reanimation.DrawRenderGroup(g, 2);
		reanimation.mEnableExtraAdditiveDraw = false;
		reanimation.DrawRenderGroup(g, 3);
		if (flag)
		{
			reanimation.mExtraOverlayColor = new SexyColor(255, 255, 255, 32);
			reanimation.mEnableExtraAdditiveDraw = true;
		}
		else
		{
			reanimation.mEnableExtraAdditiveDraw = false;
		}
		reanimation.DrawRenderGroup(g, 4);
		if (mChallengeState == ChallengeState.TreeGiveWisdom || mChallengeState == ChallengeState.TreeBabbling)
		{
			int num2;
			int num3;
			if (num < 7)
			{
				num2 = 400;
				num3 = 152;
			}
			else if (num < 12)
			{
				num2 = 395;
				num3 = 60;
			}
			else
			{
				num2 = 390;
				num3 = 52;
			}
			g.DrawImage(AtlasResources.IMAGE_REANIM_STORE_SPEECHBUBBLE2, (int)((float)num2 * Constants.S), (int)((float)num3 * Constants.S));
			string theText = Common.StrFormat_("[TREE_OF_WISDOM_{0}]", mTreeOfWisdomTalkIndex);
			TRect theRect = new TRect((int)((float)(num2 + 25) * Constants.S), (int)((float)(num3 + 6) * Constants.S), (int)(233f * Constants.S), (int)(144f * Constants.S));
			TodStringFile.TodDrawStringWrapped(g, theText, theRect, Resources.FONT_BRIANNETOD12, SexyColor.Black, DrawStringJustification.CenterVerticalMiddle, widowSupression: false);
		}
		int num4 = num;
		float num5 = 1f;
		if (num4 >= 50)
		{
			string theString = TodCommon.TodReplaceNumberString("[TREE_OF_WISDOM_HIEGHT]", "{HEIGHT}", num4);
			float num6 = (float)Resources.FONT_HOUSEOFTERROR16.StringWidth(theString) * num5;
			float num7 = (float)Resources.FONT_HOUSEOFTERROR16.mAscent * num5;
			Matrix m = default;
			TodCommon.TodScaleTransformMatrix(ref m, 400f * Constants.S - num6 * 0.5f, 20f * Constants.S + num7 * 0.5f, num5, num5);
			TodCommon.TodDrawStringMatrix(g, Resources.FONT_HOUSEOFTERROR16, m, theString, new SexyColor(255, 255, 255));
		}
	}

	public void TreeOfWisdomNextGarden()
	{
		TreeOfWisdomLeave();
		mApp.KillBoard();
		mApp.PreNewGame(GameMode.ChallengeZenGarden, theLookForSavedGame: false);
	}

	public void TreeOfWisdomPrevGarden()
	{
		TreeOfWisdomLeave();
		mApp.KillBoard();
		mApp.PreNewGame(GameMode.ChallengeZenGarden, theLookForSavedGame: false);
		mApp.mZenGarden.GotoPrevGarden(isTreeOfWisdom: true);
	}

	public void TreeOfWisdomToolUpdate(GridItem theZenTool)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(theZenTool.mGridItemReanimID);
		if (reanimation != null && reanimation.mLoopCount > 0)
		{
			TreeOfWisdomGrow();
			theZenTool.GridItemDie();
		}
	}

	public void TreeOfWisdomOpenStore()
	{
		StoreScreen storeScreen = mApp.ShowStoreScreen(this);
		storeScreen.mBackButton.SetLabel("[STORE_BACK_TO_GAME]");
		storeScreen.mBackButton.mButtonImage = AtlasResources.IMAGE_STORE_CONTINUEBUTTON;
		storeScreen.mBackButton.mDownImage = AtlasResources.IMAGE_STORE_CONTINUEBUTTONDOWN;
		storeScreen.mPage = StorePage.Zen2;
	}

	public void TreeOfWisdomBackFromStore()
	{
		TreeOfWisdomLeave();
		TreeOfWisdomInit();
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ZenGarden);
	}

	public void TreeOfWisdomLeave()
	{
		GridItem theGridItem = null;
		int index = -1;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ZenTool)
			{
				TreeOfWisdomGrow();
				theGridItem.GridItemDie();
			}
		}
		for (int i = 0; i < 6; i++)
		{
			mApp.RemoveReanimation(ref mReanimCloud[i]);
		}
		mApp.RemoveReanimation(ref mReanimChallenge);
	}

	public void TreeOfWisdomGrow()
	{
		mApp.mPlayerInfo.mChallengeRecords[48]++;
		mApp.WriteCurrentUserConfig();
		int num = TodCommon.ClampInt(TreeOfWisdomGetSize(), 1, 51);
		mApp.ReanimationGet(mReanimChallenge).PlayReanim($"anim_grow{num}", ReanimLoopType.PlayOnceAndHold, 0, 8f);
		mApp.PlayFoley(FoleyType.Plantgrow);
		if (num > 1)
		{
			mChallengeState = ChallengeState.TreeJustGrew;
			mChallengeStateCounter = 120;
		}
		else
		{
			mChallengeState = ChallengeState.Normal;
		}
	}

	public void TreeOfWisdomTool(int aMouseX, int aMouseY)
	{
		if (TreeOfWisdomMouseOn(aMouseX, aMouseY, posScaled: true) && mApp.mPlayerInfo.mPurchases[28] > 1000)
		{
			TreeOfWisdomFertilize();
		}
		mBoard.ClearCursor();
	}

	public bool TreeOfWisdomMouseOn(int theX, int theY, bool posScaled)
	{
		mBoard.MouseHitTest(theX, theY, out var theHitResult, posScaled);
		return theHitResult.mObjectType == GameObjectType.TreeOfWisdom;
	}

	public bool TreeOfWisdomHitTest(int theX, int theY, out HitResult theHitResult)
	{
		int num = TreeOfWisdomGetSize();
		if (((num <= 1) ? new TRect(310, 275, 175, 175) : ((num < 7) ? new TRect(290, 255, 205, 195) : ((num >= 12) ? new TRect(280, 155, 225, 305) : new TRect(290, 215, 205, 225)))).Contains(theX, theY))
		{
			theHitResult.mObject = null;
			theHitResult.mObjectType = GameObjectType.TreeOfWisdom;
			return true;
		}
		theHitResult.mObject = null;
		theHitResult.mObjectType = GameObjectType.None;
		return false;
	}

	public void TreeOfWisdomBabble()
	{
		mChallengeState = ChallengeState.TreeBabbling;
		mChallengeStateCounter = 400;
		int num = TreeOfWisdomGetSize();
		int num2 = TodCommon.RandRangeInt(0, 2);
		if (num <= 1)
		{
			mTreeOfWisdomTalkIndex = 600;
		}
		else if (num2 == 0 && num >= 5)
		{
			mTreeOfWisdomTalkIndex = 500;
		}
		else if (num2 == 1)
		{
			mTreeOfWisdomTalkIndex = TodCommon.RandRangeInt(101, 110);
		}
		else if (num < 12)
		{
			mTreeOfWisdomTalkIndex = TodCommon.RandRangeInt(201, 205);
		}
		else if (num < 50)
		{
			mTreeOfWisdomTalkIndex = TodCommon.RandRangeInt(301, 305);
		}
		else
		{
			mTreeOfWisdomTalkIndex = TodCommon.RandRangeInt(401, 405);
		}
	}

	public void TreeOfWisdomGiveWisdom()
	{
		mChallengeState = ChallengeState.TreeGiveWisdom;
		mChallengeStateCounter = 1000;
		int num = TreeOfWisdomGetSize();
		if (num == 100)
		{
			mTreeOfWisdomTalkIndex = 800;
		}
		else if (num == 500)
		{
			mTreeOfWisdomTalkIndex = 900;
		}
		else if (num == 1000)
		{
			mTreeOfWisdomTalkIndex = 1000;
		}
		else if (num > 1000)
		{
			mTreeOfWisdomTalkIndex = 1100;
		}
		else
		{
			mTreeOfWisdomTalkIndex = TodCommon.ClampInt(num - 1, 1, 49);
		}
	}

	public bool TreeOfWisdomCanFeed()
	{
		if (mChallengeState == ChallengeState.TreeJustGrew)
		{
			return false;
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ZenTool)
			{
				return false;
			}
		}
		return true;
	}

	public void TreeOfWisdomSayRepeat()
	{
		int num = TreeOfWisdomGetSize();
		if (num >= 100 && RandomNumbers.NextNumber(47) == 0)
		{
			mTreeOfWisdomTalkIndex = 800;
		}
		else if (num >= 500 && RandomNumbers.NextNumber(47) == 0)
		{
			mTreeOfWisdomTalkIndex = 900;
		}
		else if (num >= 1000 && RandomNumbers.NextNumber(47) == 0)
		{
			mTreeOfWisdomTalkIndex = 1000;
		}
		else
		{
			mTreeOfWisdomTalkIndex = TodCommon.RandRangeInt(2, TodCommon.ClampInt(num, 3, 49) - 1);
		}
		mChallengeStateCounter = 600;
	}

	public bool PuzzleIsAwardStage()
	{
		if (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
		{
			return false;
		}
		int num = 1;
		if (mApp.mGameMode == GameMode.PuzzleIZombieEndless)
		{
			num = 3;
		}
		else if (mApp.mGameMode == GameMode.ScaryPotterEndless)
		{
			num = 10;
		}
		return (mSurvivalStage + 1) % num == 0;
	}

	public void BackFromStore()
	{
		mApp.KillDialog(4);
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			TreeOfWisdomBackFromStore();
		}
	}

	public bool MouseDownWithZenTool(int x, int y, CursorType theCursorType, bool isTouch)
	{
		HitResult hitResult = mBoard.ToolHitTest(x, y, posScaled: true, isTouch);
		if (hitResult.mObjectType != GameObjectType.Plant || hitResult.mObject == null)
		{
			return false;
		}
		Plant plant = (Plant)hitResult.mObject;
		if (theCursorType == CursorType.Glove)
		{
			mBoard.mCursorObject.mType = plant.mSeedType;
			mBoard.mCursorObject.mImitaterType = plant.mImitaterType;
			mBoard.mCursorObject.mCursorType = CursorType.PlantFromGlove;
			mBoard.mCursorObject.mGlovePlantID = mBoard.mPlants[mBoard.mPlants.IndexOf(plant)];
			plant.mGloveGrabbed = true;
			mApp.PlaySample(Resources.SOUND_TAP);
			return true;
		}
		return false;
	}

	public int GetGloveCounterMax()
	{
		int? num = mApp.mCreativeLevel?.GetComponent<CSGlove>()?.Cooldown;
		if (num.HasValue)
		{
			return num.GetValueOrDefault();
		}
		if (mApp.IsRogueConveyorbeltLevel())
		{
			return 400;
		}
		if (mApp.IsSurvivalHell(mApp.mGameMode))
		{
			return 1000;
		}
		if (mApp.IsAdventureModeHard())
		{
			if (mApp.IsFinalBossLevel())
			{
				return 400;
			}
			return 1500;
		}
		return 50;
	}

	public void MovePlant(Plant thePlant, int theGridX, int theGridY)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			return;
		}
		Plant anOldFlowerPot = ((thePlant.mSeedType == SeedType.Endoflame) ? mBoard.GetFlowerPotAt(thePlant.mPlantCol, thePlant.mRow) : null);
		int num = mBoard.GridToPixelX(theGridX, theGridY);
		int num2 = mBoard.GridToPixelY(theGridX, theGridY);
		bool flag = true;
		Plant topPlantAt = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyPumpkin);
		if (topPlantAt != null && topPlantAt.mSeedType == SeedType.Pumpkinshell && topPlantAt != thePlant)
		{
			flag = false;
		}
		if (flag)
		{
			foreach (GridItem mGridItem in mBoard.mGridItems)
			{
				if (mGridItem.mGridX == thePlant.mPlantCol && mGridItem.mGridY == thePlant.mRow && mGridItem.mGridItemType == GridItemType.Ladder)
				{
					mGridItem.mGridX = theGridX;
					mGridItem.mGridY = theGridY;
					mGridItem.mRenderOrder = 302000 + 10000 * theGridY + 800;
				}
			}
		}
		float num3 = num - thePlant.mX;
		float num4 = num2 - thePlant.mY;
		thePlant.mX = num;
		thePlant.mY = num2;
		thePlant.mPlantCol = theGridX;
		thePlant.mRow = theGridY;
		thePlant.mRenderOrder = thePlant.CalcRenderOrder();
		Plant flowerPotAt = mBoard.GetFlowerPotAt(theGridX, theGridY);
		thePlant.mInFlowerPot = thePlant.mSeedType != SeedType.Flowerpot && flowerPotAt != null;
		if (thePlant.mSeedType == SeedType.Endoflame)
		{
			if (anOldFlowerPot != null && anOldFlowerPot != flowerPotAt && anOldFlowerPot.mState == PlantState.FlowerpotEndoflameOverrideNoDraw && !mBoard.mPlants.Any((Plant aPlant) => aPlant.mSeedType == SeedType.Endoflame && !aPlant.mDead && !aPlant.mSquished && aPlant.mPlantCol == anOldFlowerPot.mPlantCol && aPlant.mRow == anOldFlowerPot.mRow && (!aPlant.NotOnGround() || aPlant.mGloveGrabbed)))
			{
				anOldFlowerPot.mState = PlantState.Notready;
			}
			if (flowerPotAt != null)
			{
				flowerPotAt.mState = PlantState.FlowerpotEndoflameOverrideNoDraw;
			}
		}
		if (thePlant.mSleepingReanimID != null)
		{
			float[] sleepingReanimOffset = thePlant.GetSleepingReanimOffset();
			float num5 = (float)thePlant.mX + sleepingReanimOffset[0];
			float num6 = (float)thePlant.mY + sleepingReanimOffset[1];
			thePlant.mSleepingReanimID.SetPosition(num5 * Constants.S, num6 * Constants.S);
			thePlant.mSleepingReanimID.mRenderOrder = thePlant.mRenderOrder;
		}
		if (thePlant.mSealedReanimID != null)
		{
			float num7 = thePlant.mX;
			float num8 = thePlant.mY - 20;
			thePlant.mSealedReanimID.SetPosition(num7 * Constants.S, num8 * Constants.S);
			thePlant.mSealedReanimID.mRenderOrder = thePlant.mRenderOrder;
		}
		TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(thePlant.mParticleID);
		if (todParticleSystem != null && todParticleSystem.mEmitterList.Count != 0)
		{
			TodParticleEmitter todParticleEmitter = todParticleSystem.mParticleHolder.mEmitters[0];
			todParticleSystem.SystemMove(todParticleEmitter.mSystemCenter.x + num3, todParticleEmitter.mSystemCenter.y + num4);
		}
		if (thePlant.mSeedType == SeedType.Agave)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(thePlant.mLightReanimID);
			if (reanimation != null)
			{
				reanimation.mOverlayMatrix.mMatrix.M41 = (float)thePlant.mX * Constants.S;
				reanimation.mOverlayMatrix.mMatrix.M42 = ((float)thePlant.mY + Plant.PlantDrawHeightOffset(mBoard, thePlant, thePlant.mSeedType, thePlant.mPlantCol, thePlant.mRow)) * Constants.S;
				reanimation.mRenderOrder = thePlant.mRenderOrder + 1;
			}
		}
	}
}
