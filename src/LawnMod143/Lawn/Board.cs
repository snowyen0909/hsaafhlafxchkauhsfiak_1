using System;
using System.Collections.Generic;
using System.Linq;
using Lawn.Creative;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Board : Widget, ButtonListener, AlmanacListener
{
	[Flags]
	public enum TopRightUIDrawMode
	{
		MenuButton = 1,
		StoreButton = 2,
		PauseButton = 4,
		AccelerateButton = 8,
		All = MenuButton | StoreButton | PauseButton | AccelerateButton
	}

	public class Camera
	{
		private const float DEFAULT_ZOOM = 1f;

		private float origX;

		private float origY;

		private float origZoom;

		private float targetX;

		private float targetY;

		private float targetZoom;

		private TodCurves curve;

		private int boardOffsetX;

		private int screenWidth;

		private int screenHeight;

		public float X { get; private set; }

		public float Y { get; private set; }

		public float Zoom { get; private set; }

		public int OperationCountdown { get; private set; }

		public int OperationMaxCountdown { get; private set; } = 1;

		public Camera(Board theBoard, int theScreenWidth, int theScreenHeight)
		{
			screenWidth = theScreenWidth;
			screenHeight = theScreenHeight;
			boardOffsetX = Constants.Board_Offset_AspectRatio_Correction;
			X = 0f;
			Y = 0f;
			Zoom = 1f;
			targetX = X;
			targetY = Y;
			targetZoom = Zoom;
		}

		public void Update()
		{
			if (OperationCountdown > 0)
			{
				OperationCountdown--;
			}
			X = TodCommon.TodAnimateCurveFloat(OperationMaxCountdown, 0, OperationCountdown, origX, targetX, curve);
			Y = TodCommon.TodAnimateCurveFloat(OperationMaxCountdown, 0, OperationCountdown, origY, targetY, curve);
			Zoom = TodCommon.TodAnimateCurveFloat(OperationMaxCountdown, 0, OperationCountdown, origZoom, targetZoom, curve);
		}

		public void ApplyTransform(Graphics g)
		{
			float num = (float)screenWidth / 2f;
			float num2 = (float)screenHeight / 2f;
			SexyTransform2D theTransform = default;
			theTransform.Translate(0f - num, 0f - num2);
			theTransform.Scale(Zoom, Zoom);
			theTransform.Translate(num + X, num2 + Y);
			g.pushTransform(ref theTransform, concatenate: true);
		}

		public void ResetTransform(Graphics g)
		{
			g.popTransform();
		}

		public TPoint ScreenToBoard(int screenX, int screenY)
		{
			float num = (float)screenWidth / 2f;
			float num2 = (float)screenHeight / 2f;
			float num3 = ((float)screenX - X - num + (float)boardOffsetX) / Zoom + num - (float)boardOffsetX;
			float num4 = ((float)screenY - Y - num2) / Zoom + num2;
			return new TPoint((int)num3, (int)num4);
		}

		public void ScreenToBoardReplace(ref int screenX, ref int screenY)
		{
			float num = (float)screenWidth / 2f;
			float num2 = (float)screenHeight / 2f;
			float num3 = ((float)screenX - X - num + (float)boardOffsetX) / Zoom + num - (float)boardOffsetX;
			float num4 = ((float)screenY - Y - num2) / Zoom + num2;
			screenX = (int)num3;
			screenY = (int)num4;
		}

		public void ScreenToBoardReplace(ref float screenX, ref float screenY)
		{
			float num = (float)screenWidth / 2f;
			float num2 = (float)screenHeight / 2f;
			float num3 = (screenX - X - num + (float)boardOffsetX) / Zoom + num - (float)boardOffsetX;
			float num4 = (screenY - Y - num2) / Zoom + num2;
			screenX = num3;
			screenY = num4;
		}

		public TPoint BoardToScreen(int boardX, int boardY)
		{
			float num = (float)screenWidth / 2f;
			float num2 = (float)screenHeight / 2f;
			float num3 = ((float)boardX - num + (float)boardOffsetX) * Zoom + num - (float)boardOffsetX + X;
			float num4 = ((float)boardY - num2) * Zoom + num2 + Y;
			return new TPoint((int)num3, (int)num4);
		}

		public void SetPosition(float x, float y, float scale)
		{
			targetX = x;
			targetY = y;
			targetZoom = scale;
		}

		public void SetPositionSmooth(float x, float y, float scale, int countdown, TodCurves theCurve)
		{
			origX = X;
			origY = Y;
			origZoom = Zoom;
			targetX = x;
			targetY = y;
			targetZoom = scale;
			int operationMaxCountdown = (OperationCountdown = countdown);
			OperationMaxCountdown = operationMaxCountdown;
			curve = theCurve;
		}

		public void ZoomAtSmooth(float focusWorldX, float focusWorldY, float zoom, int countdown, TodCurves theCurve)
		{
			origX = X;
			origY = Y;
			origZoom = Zoom;
			targetX = (0f - (focusWorldX - (float)screenWidth / 2f)) * zoom + focusWorldX - (float)screenWidth / 2f;
			targetY = (0f - (focusWorldY - (float)screenHeight / 2f)) * zoom + focusWorldY - (float)screenHeight / 2f;
			targetZoom = zoom;
			int operationMaxCountdown = (OperationCountdown = countdown);
			OperationMaxCountdown = operationMaxCountdown;
			curve = theCurve;
		}

		public void FocusWorldSmooth(float focusWorldX, float focusWorldY, float zoom, int countdown, TodCurves theCurve)
		{
			origX = X;
			origY = Y;
			origZoom = Zoom;
			targetX = (0f - (focusWorldX - (float)screenWidth / 2f)) * zoom;
			targetY = (0f - (focusWorldY - (float)screenHeight / 2f)) * zoom;
			targetZoom = zoom;
			int operationMaxCountdown = (OperationCountdown = countdown);
			OperationMaxCountdown = operationMaxCountdown;
			curve = theCurve;
		}

		public void Reset()
		{
			targetX = 0f;
			targetY = 0f;
			targetZoom = 1f;
		}
	}

	public bool mIsCheatDown;

	public const int LevelLimit = 3;

	public const int TRIALMODE_LEVEL_LIMIT = 7;

	private const int MAX_POOL_GRID_SIZE = 10;

	public const int FIRST_MINIGAME_UNLOCK_LEVEL = 22;

	public const int PUZZLE_UNLOCK_LEVEL = 36;

	private const int SAVE_CHECK_NUMBER = 777;

	private RenderItem[] aRenderList = new RenderItem[2048];

	public LawnApp mApp;

	public List<Zombie> mZombies = new List<Zombie>();

	public List<Plant> mPlants = new List<Plant>();

	public List<Projectile> mProjectiles = new List<Projectile>();

	public List<Coin> mCoins = new List<Coin>();

	public List<LawnMower> mLawnMowers = new List<LawnMower>();

	public List<GridItem> mGridItems = new List<GridItem>();

	public CursorObject mCursorObject;

	public CursorPreview mCursorPreview;

	public MessageWidget mAdvice;

	public SeedBank mSeedBank;

	public GameButton mMenuButton;

	public GameButton mStoreButton;

	public GameButton mAccelerateButton;

	public GameButton mPauseButton;

	internal GameButton mCornRouletteSelfButton;

	internal GameButton mCornRouletteOtherButton;

	public bool mIgnoreMouseUp;

	public bool mIgnoreNextMouseUp;

	public bool mIgnoreNextMouseUpSeedPacket;

	public int mLastToolX;

	public int mLastToolY;

	public CutScene mCutScene;

	public Challenge mChallenge;

	public static bool gShownMoreSunTutorial;

	private static bool needToSortRenderList;

	public List<Zombie> mZombiesRow1 = new List<Zombie>();

	public List<Zombie> mZombiesRow2 = new List<Zombie>();

	public List<Zombie> mZombiesRow3 = new List<Zombie>();

	public List<Zombie> mZombiesRow4 = new List<Zombie>();

	public List<Zombie> mZombiesRow5 = new List<Zombie>();

	public List<Zombie> mZombiesRow6 = new List<Zombie>();

	public bool mPaused;

	public GridSquareType[,] mGridSquareType = new GridSquareType[Constants.GRIDSIZEX, Constants.MAX_GRIDSIZEY];

	public int[,] mGridCelLook = new int[Constants.GRIDSIZEX, Constants.MAX_GRIDSIZEY];

	public int[,,] mGridCelOffset = new int[Constants.GRIDSIZEX, Constants.MAX_GRIDSIZEY, 2];

	public int[,] mGridCelFog = new int[Constants.GRIDSIZEX, 7];

	public bool mEnableGraveStones;

	public int mSpecialGraveStoneX;

	public int mSpecialGraveStoneY;

	public float mFogOffset;

	public int mFogBlownCountDown;

	public PlantRowType[] mPlantRow = new PlantRowType[Constants.MAX_GRIDSIZEY];

	public int[] mWaveRowGotLawnMowered = new int[Constants.MAX_GRIDSIZEY];

	public int mBonusLawnMowersRemaining;

	public int[] mIceMinX = new int[Constants.MAX_GRIDSIZEY];

	public int[] mIceTimer = new int[Constants.MAX_GRIDSIZEY];

	public ParticleSystemID[] mIceParticleID = new ParticleSystemID[Constants.MAX_GRIDSIZEY];

	public TodSmoothArray[] mRowPickingArray = new TodSmoothArray[Constants.MAX_GRIDSIZEY];

	public ZombieType[,] mZombiesInWave = new ZombieType[100, 50];

	public bool[] mZombieAllowed = new bool[100];

	public int mSunCountDown;

	public int mNumSunsFallen;

	public int mShakeCounter;

	public int mShakeAmountX;

	public int mShakeAmountY;

	public BackgroundType mBackground;

	public int mLevel;

	public int mSodPosition;

	public int mPrevMouseX;

	public int mPrevMouseY;

	public int mSunMoney;

	public int mNumWaves;

	public int mMainCounter;

	public int mEffectCounter;

	public int mDrawCount;

	public int mRiseFromGraveCounter;

	public int mOutOfMoneyCounter;

	public int mCurrentWave;

	public int mTotalSpawnedWaves;

	public TutorialState mTutorialState;

	public TodParticleSystem mTutorialParticleID;

	public int mTutorialParticleID_Save;

	public int mTutorialTimer;

	public int mLastBungeeWave;

	public int mZombieHealthToNextWave;

	public int mZombieHealthWaveStart;

	public int mZombieCountDown;

	public int mZombieCountDownStart;

	public int mHugeWaveCountDown;

	public bool[] mHelpDisplayed = new bool[73];

	public AdviceType mHelpIndex;

	public bool mFinalBossKilled;

	public bool mShowShovel;

	public int mCoinBankFadeCount;

	public int mLevelFadeCount;

	public DebugTextMode mDebugTextMode;

	public bool mLevelComplete;

	public int mBoardFadeOutCounter;

	public int mNextSurvivalStageCounter;

	public int mScoreNextMowerCounter;

	public bool mLevelAwardSpawned;

	public int mProgressMeterWidth;

	public int mFlagRaiseCounter;

	public int mIceTrapCounter;

	public int mBoardRandSeed;

	public TodParticleSystem mPoolSparklyParticleID;

	public int mPoolSparklyParticleID_Save;

	public Reanimation[,] mFwooshID = new Reanimation[Constants.MAX_GRIDSIZEY, 12];

	public int[,] mFwooshID_Save = new int[Constants.MAX_GRIDSIZEY, 12];

	public int mFwooshCountDown;

	public int mTimeStopCounter;

	public bool mDroppedFirstCoin;

	public int mFinalWaveSoundCounter;

	public int mCobCannonCursorDelayCounter;

	public int mCobCannonMouseX;

	public int mCobCannonMouseY;

	public bool mKilledYeti;

	public bool mMustacheMode;

	public bool mSuperMowerMode;

	public bool mFutureMode;

	public bool mPinataMode;

	public bool mDanceMode;

	public bool mDaisyMode;

	public bool mSukhbirMode;

	public BoardResult mPrevBoardResult;

	public int mTriggeredLawnMowers;

	public int mPlayTimeActiveLevel;

	public int mPlayTimeInactiveLevel;

	public int mMaxSunPlants;

	public int mStartDrawTime;

	public int mIntervalDrawTime;

	public int mIntervalDrawCountStart;

	public float mMinFPS;

	public int mPreloadTime;

	public int mGameID;

	public int mGravesCleared;

	public int mPlantsEaten;

	public int mPlantsShoveled;

	public int mCoinsCollected;

	public int mDiamondsCollected;

	public int mPottedPlantsCollected;

	public int mChocolateCollected;

	public bool mPeaShooterUsed;

	public bool mCatapultPlantsUsed;

	public int mCollectedCoinStreak;

	public int mGargantuarsKillsByCornCob;

	public bool mMushroomAndCoffeeBeansOnly;

	public bool mMushroomsUsed;

	public int mDoomsUsed;

	public bool mPlanternOrBloverUsed;

	public bool mNutsUsed;

	public bool mNomNomNomAchievementTracker;

	public bool mNoFungusAmongUsAchievementTracker;

	private static int mPeashootersPlanted;

	public Reanimation[] mCoverLayerAnimIDs = new Reanimation[7];

	public int[] mCoverLayerAnimIDs_Save = new int[7];

	public int mAgavePowerfulCountdown;

	public int mEndoflamePowerfulCountdown;

	public int mAgaveSkillDrawType;

	public int mEndoflameSkillDrawType;

	public int mAccelerationNumerator = 1;

	public int mAccelerationDenominator = 1;

	public int mAccelerationFrameIndex;

	public bool mManualPaused;

	public int mSealedCountdown;

	public bool mDoPlaceRose;

	public List<Wind> mWinds = new List<Wind>();

	public Vector2 mWindSum = Vector2.Zero;

	public static TodWeightedArray[] aZombieWeightArray;

	public static TodWeightedArray[] aPickArray;

	public static TodWeightedGridArray[] aGridArray;

	private static int MAX_GRAVE_STONES;

	private static GridItem[] aPicks;

	private string progressMeterString;

	private int progressMeterStringValue = int.MinValue;

	private bool doAddGraveStones = true;

	public string mLevelStr;

	private int levelStrVal = -1;

	private static TPoint[] mCelPoints;

	private static Dictionary<int, string> cachedChargesStringsFertilizer;

	private static Dictionary<int, string> cachedChargesStringsBugSpray;

	private static Dictionary<int, string> cachedChargesStringsChocolate;

	private bool mGridFocusReturning;

	private (float X, float Y, float Zoom)? mGridFocusReturn;

	public Camera mCamera;

	public bool mCameraEnabled;

	internal bool CanSelectGridFocus
	{
		get
		{
			bool flag = mApp.mGameScene == GameScenes.Playing;
			bool flag2 = flag;
			if (flag2)
			{
				GameMode mGameMode = mApp.mGameMode;
				bool flag3 = ((mGameMode == GameMode.ChallengeZenGarden || mGameMode == GameMode.TreeOfWisdom) ? true : false);
				flag2 = !flag3;
			}
			if (flag2 && mCameraEnabled && mCamera != null && !IsGridFocusActive)
			{
				return CanInteractWithBoardButtons();
			}
			return false;
		}
	}

	internal bool IsGridFocusActive => mGridFocusReturn.HasValue;

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		int theInt = mZombies.Count;
		theContext.SyncListLength(ref theInt);
		if (theContext.mReading)
		{
			mZombies.Clear();
			for (int i = 0; i < theInt; i++)
			{
				Zombie newZombie = Zombie.GetNewZombie();
				newZombie.Sync(theContext);
				AddToZombieList(newZombie);
			}
		}
		else
		{
			for (int j = 0; j < theInt; j++)
			{
				mZombies[j].Sync(theContext);
			}
		}
		int theInt2 = mPlants.Count;
		theContext.SyncListLength(ref theInt2);
		if (theContext.mReading)
		{
			mPlants.Clear();
			for (int k = 0; k < theInt2; k++)
			{
				mPlants.Add(Plant.GetNewPlant());
			}
		}
		for (int l = 0; l < theInt2; l++)
		{
			mPlants[l].Sync(theContext);
		}
		int theInt3 = mProjectiles.Count;
		theContext.SyncListLength(ref theInt3);
		if (theContext.mReading)
		{
			mProjectiles.Clear();
			for (int m = 0; m < theInt3; m++)
			{
				mProjectiles.Add(Projectile.GetNewProjectile());
			}
		}
		for (int n = 0; n < theInt3; n++)
		{
			mProjectiles[n].Sync(theContext);
		}
		int theInt4 = mCoins.Count;
		theContext.SyncListLength(ref theInt4);
		if (theContext.mReading)
		{
			mCoins.Clear();
			for (int num = 0; num < theInt4; num++)
			{
				mCoins.Add(new Coin());
			}
		}
		for (int num2 = 0; num2 < theInt4; num2++)
		{
			mCoins[num2].Sync(theContext);
		}
		int theInt5 = mLawnMowers.Count;
		theContext.SyncListLength(ref theInt5);
		if (theContext.mReading)
		{
			mLawnMowers.Clear();
			for (int num3 = 0; num3 < theInt5; num3++)
			{
				mLawnMowers.Add(LawnMower.GetNewLawnMower());
			}
		}
		for (int num4 = 0; num4 < theInt5; num4++)
		{
			mLawnMowers[num4].Sync(theContext);
		}
		int theInt6 = mGridItems.Count;
		theContext.SyncListLength(ref theInt6);
		if (theContext.mReading)
		{
			mGridItems.Clear();
			for (int num5 = 0; num5 < theInt6; num5++)
			{
				mGridItems.Add(GridItem.GetNewGridItem());
			}
		}
		for (int num6 = 0; num6 < theInt6; num6++)
		{
			mGridItems[num6].Sync(theContext);
		}
		mCursorObject.Sync(theContext);
		mCursorPreview.Sync(theContext);
		if (mSeedBank == null)
		{
			mSeedBank = new SeedBank();
		}
		mSeedBank.Sync(theContext);
		theContext.SyncBool(ref mIgnoreMouseUp);
		theContext.SyncBool(ref mIgnoreNextMouseUp);
		theContext.SyncBool(ref mIgnoreNextMouseUpSeedPacket);
		theContext.SyncInt(ref mLastToolX);
		theContext.SyncInt(ref mLastToolY);
		mCutScene.Sync(theContext);
		mChallenge.Sync(theContext);
		theContext.SyncBool(ref mPaused);
		for (int num7 = 0; num7 < 9; num7++)
		{
			for (int num8 = 0; num8 < 6; num8++)
			{
				theContext.SyncEnum(ref mGridSquareType[num7, num8], theContext.aGridSquareTypeSaver, SaveFileVersion.Initial, GridSquareType.None);
			}
		}
		for (int num9 = 0; num9 < 9; num9++)
		{
			for (int num10 = 0; num10 < 6; num10++)
			{
				theContext.SyncInt(ref mGridCelLook[num9, num10]);
			}
		}
		for (int num11 = 0; num11 < 9; num11++)
		{
			for (int num12 = 0; num12 < 6; num12++)
			{
				for (int num13 = 0; num13 < 2; num13++)
				{
					theContext.SyncInt(ref mGridCelOffset[num11, num12, num13]);
				}
			}
		}
		for (int num14 = 0; num14 < 9; num14++)
		{
			for (int num15 = 0; num15 < 7; num15++)
			{
				theContext.SyncInt(ref mGridCelFog[num14, num15]);
			}
		}
		theContext.SyncBool(ref mEnableGraveStones);
		theContext.SyncInt(ref mSpecialGraveStoneX);
		theContext.SyncInt(ref mSpecialGraveStoneY);
		theContext.SyncFloat(ref mFogOffset);
		theContext.SyncInt(ref mFogBlownCountDown);
		for (int num16 = 0; num16 < 6; num16++)
		{
			theContext.SyncEnum(ref mPlantRow[num16], theContext.aPlantRowTypeSaver, SaveFileVersion.Initial, PlantRowType.Dirt);
		}
		for (int num17 = 0; num17 < 6; num17++)
		{
			theContext.SyncInt(ref mWaveRowGotLawnMowered[num17]);
		}
		theContext.SyncInt(ref mBonusLawnMowersRemaining);
		for (int num18 = 0; num18 < 6; num18++)
		{
			theContext.SyncInt(ref mIceMinX[num18]);
		}
		for (int num19 = 0; num19 < 6; num19++)
		{
			theContext.SyncInt(ref mIceTimer[num19]);
		}
		for (int num20 = 0; num20 < 6; num20++)
		{
			TodSmoothArray[] array = mRowPickingArray;
			int num21 = num20;
			if (array[num21] == null)
			{
				array[num21] = new TodSmoothArray();
			}
			theContext.SyncInt(ref mRowPickingArray[num20].mItem);
			theContext.SyncFloat(ref mRowPickingArray[num20].mWeight);
			theContext.SyncFloat(ref mRowPickingArray[num20].mLastPicked);
			theContext.SyncFloat(ref mRowPickingArray[num20].mSecondLastPicked);
		}
		for (int num22 = 0; num22 < 100; num22++)
		{
			for (int num23 = 0; num23 < 50; num23++)
			{
				theContext.SyncEnum(ref mZombiesInWave[num22, num23], theContext.aZombieTypeSaver, SaveFileVersion.Initial, ZombieType.Normal);
			}
		}
		for (int num24 = 0; num24 < 100; num24++)
		{
			theContext.SyncBool(ref mZombieAllowed[num24]);
		}
		theContext.SyncInt(ref mSunCountDown);
		theContext.SyncInt(ref mNumSunsFallen);
		theContext.SyncInt(ref mShakeCounter);
		theContext.SyncInt(ref mShakeAmountX);
		theContext.SyncInt(ref mShakeAmountY);
		theContext.SyncEnum(ref mBackground, theContext.aBackgroundTypeSaver, SaveFileVersion.Initial, BackgroundType.Num1Day);
		theContext.SyncInt(ref mLevel);
		theContext.SyncInt(ref mSodPosition);
		theContext.SyncInt(ref mPrevMouseX);
		theContext.SyncInt(ref mPrevMouseY);
		theContext.SyncInt(ref mSunMoney);
		theContext.SyncInt(ref mNumWaves);
		theContext.SyncInt(ref mMainCounter);
		theContext.SyncInt(ref mEffectCounter);
		theContext.SyncInt(ref mDrawCount);
		theContext.SyncInt(ref mRiseFromGraveCounter);
		theContext.SyncInt(ref mOutOfMoneyCounter);
		theContext.SyncInt(ref mCurrentWave);
		theContext.SyncInt(ref mTotalSpawnedWaves);
		theContext.SyncEnum(ref mTutorialState, theContext.aTutorialStateSaver, SaveFileVersion.Initial, TutorialState.Off);
		theContext.SyncInt(ref mTutorialParticleID_Save);
		theContext.SyncInt(ref mLastBungeeWave);
		theContext.SyncInt(ref mZombieHealthToNextWave);
		theContext.SyncInt(ref mZombieHealthWaveStart);
		theContext.SyncInt(ref mZombieCountDown);
		theContext.SyncInt(ref mZombieCountDownStart);
		theContext.SyncInt(ref mHugeWaveCountDown);
		for (int num25 = 0; num25 < 67; num25++)
		{
			theContext.SyncBool(ref mHelpDisplayed[num25]);
		}
		theContext.SyncEnum(ref mHelpIndex, theContext.aAdviceTypeSaver, SaveFileVersion.Initial, AdviceType.ClickOnSun);
		theContext.SyncBool(ref mFinalBossKilled);
		theContext.SyncBool(ref mShowShovel);
		theContext.SyncInt(ref mCoinBankFadeCount);
		theContext.SyncInt(ref mLevelFadeCount);
		theContext.SyncBool(ref mLevelComplete);
		theContext.SyncInt(ref mBoardFadeOutCounter);
		theContext.SyncInt(ref mNextSurvivalStageCounter);
		theContext.SyncInt(ref mScoreNextMowerCounter);
		theContext.SyncBool(ref mLevelAwardSpawned);
		theContext.SyncInt(ref mProgressMeterWidth);
		theContext.SyncInt(ref mFlagRaiseCounter);
		theContext.SyncInt(ref mIceTrapCounter);
		theContext.SyncInt(ref mBoardRandSeed);
		theContext.SyncInt(ref mPoolSparklyParticleID_Save);
		for (int num26 = 0; num26 < 6; num26++)
		{
			for (int num27 = 0; num27 < 12; num27++)
			{
				theContext.SyncInt(ref mFwooshID_Save[num26, num27]);
			}
		}
		theContext.SyncInt(ref mFwooshCountDown);
		theContext.SyncInt(ref mTimeStopCounter);
		theContext.SyncBool(ref mDroppedFirstCoin);
		theContext.SyncInt(ref mFinalWaveSoundCounter);
		theContext.SyncInt(ref mCobCannonCursorDelayCounter);
		theContext.SyncInt(ref mCobCannonMouseX);
		theContext.SyncInt(ref mCobCannonMouseY);
		theContext.SyncBool(ref mKilledYeti);
		theContext.SyncBool(ref mMustacheMode);
		theContext.SyncBool(ref mSuperMowerMode);
		theContext.SyncBool(ref mFutureMode);
		theContext.SyncBool(ref mPinataMode);
		theContext.SyncBool(ref mDanceMode);
		theContext.SyncBool(ref mDaisyMode);
		theContext.SyncBool(ref mSukhbirMode);
		theContext.SyncInt(ref mTriggeredLawnMowers);
		theContext.SyncInt(ref mPlayTimeActiveLevel);
		theContext.SyncInt(ref mPlayTimeInactiveLevel);
		theContext.SyncInt(ref mMaxSunPlants);
		theContext.SyncInt(ref mStartDrawTime);
		theContext.SyncInt(ref mIntervalDrawTime);
		theContext.SyncInt(ref mIntervalDrawCountStart);
		theContext.SyncFloat(ref mMinFPS);
		theContext.SyncInt(ref mPreloadTime);
		theContext.SyncInt(ref mGameID);
		theContext.SyncInt(ref mGravesCleared);
		theContext.SyncInt(ref mPlantsEaten);
		theContext.SyncInt(ref mPlantsShoveled);
		theContext.SyncInt(ref mCoinsCollected);
		theContext.SyncInt(ref mDiamondsCollected);
		theContext.SyncInt(ref mPottedPlantsCollected);
		theContext.SyncInt(ref mChocolateCollected);
		theContext.SyncBool(ref mPeaShooterUsed);
		theContext.SyncBool(ref mCatapultPlantsUsed);
		theContext.SyncInt(ref mCollectedCoinStreak);
		theContext.SyncInt(ref mGargantuarsKillsByCornCob);
		theContext.SyncBool(ref mMushroomAndCoffeeBeansOnly);
		theContext.SyncBool(ref mMushroomsUsed);
		theContext.SyncInt(ref mDoomsUsed);
		theContext.SyncBool(ref mPlanternOrBloverUsed);
		theContext.SyncBool(ref mNutsUsed);
		theContext.SyncBool(ref mNomNomNomAchievementTracker);
		theContext.SyncBool(ref mNoFungusAmongUsAchievementTracker);
		if (theContext.mVersion <= SaveFileVersion.Initial)
		{
			int theInt7 = 0;
			theContext.SyncInt(ref theInt7);
			bool theBool = false;
			theContext.SyncBool(ref theBool);
		}
		theContext.SyncInt(ref mPeashootersPlanted);
		if (theContext.mReading)
		{
			for (int num28 = 0; num28 < mCoverLayerAnimIDs_Save.Length; num28++)
			{
				mCoverLayerAnimIDs_Save[num28] = -1;
			}
		}
		if (theContext.mVersion >= SaveFileVersion.CardGroup)
		{
			for (int num29 = 0; num29 < 7; num29++)
			{
				theContext.SyncInt(ref mCoverLayerAnimIDs_Save[num29]);
			}
		}
		theContext.SyncString(ref mLevelStr);
		theContext.SyncInt(ref mAgavePowerfulCountdown, SaveFileVersion.RecordAppVersionNumber);
		theContext.SyncInt(ref mAgaveSkillDrawType, SaveFileVersion.RecordAppVersionNumber);
		theContext.SyncInt(ref mEndoflamePowerfulCountdown, SaveFileVersion.Endoflame);
		theContext.SyncInt(ref mEndoflameSkillDrawType, SaveFileVersion.Endoflame);
		theContext.SyncInt(ref mSealedCountdown, SaveFileVersion.Talisman);
		if (!theContext.mReading)
		{
			return;
		}
		for (int num30 = 0; num30 < 10; num30++)
		{
			SeedPacket seedPacket = mSeedBank.mSeedPackets[num30];
			if (seedPacket.mIndex == -1)
			{
				seedPacket.mIndex = num30;
			}
		}
	}

	internal void SyncObj(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		TodLibObjSyncer.SyncObjFromList(ref mCutScene.mZombiesWonReanimID_Save, ref mCutScene.mZombiesWonReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mChallenge.mReanimChallenge_Save, ref mChallenge.mReanimChallenge, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		for (int i = 0; i < mChallenge.mReanimCloud.Length; i++)
		{
			TodLibObjSyncer.SyncObjFromList(ref mChallenge.mReanimCloud_Save[i], ref mChallenge.mReanimCloud[i], mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		}
		TodLibObjSyncer.SyncObjFromList(ref mTutorialParticleID_Save, ref mTutorialParticleID, mApp.mEffectSystem.mParticleHolder.mParticleSystems, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mPoolSparklyParticleID_Save, ref mPoolSparklyParticleID, mApp.mEffectSystem.mParticleHolder.mParticleSystems, theContext.mReading);
		for (int j = 0; j < mFwooshID.GetLength(0); j++)
		{
			for (int k = 0; k < mFwooshID.GetLength(1); k++)
			{
				TodLibObjSyncer.SyncObjFromList(ref mFwooshID_Save[j, k], ref mFwooshID[j, k], mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
			}
		}
		for (int l = 0; l < mCoverLayerAnimIDs.Length; l++)
		{
			TodLibObjSyncer.SyncObjFromList(ref mCoverLayerAnimIDs_Save[l], ref mCoverLayerAnimIDs[l], mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
			if (theContext.mVersion < SaveFileVersion.NewLayout600)
			{
				mCoverLayerAnimIDs[l]?.SetPosition(mCoverLayerAnimIDs[l].mOverlayMatrix.mMatrix.Translation.X * Constants.ISUI - 100f, mCoverLayerAnimIDs[l].mOverlayMatrix.mMatrix.Translation.Y * Constants.ISUI);
			}
		}
		for (int m = 0; m < mCoins.Count; m++)
		{
			mCoins[m].SyncObj(theContext);
		}
		mCursorObject.SyncObj(theContext);
		mCursorPreview.SyncObj(theContext);
		for (int n = 0; n < mGridItems.Count; n++)
		{
			mGridItems[n].SyncObj(theContext);
		}
		for (int num = 0; num < mLawnMowers.Count; num++)
		{
			mLawnMowers[num].SyncObj(theContext);
		}
		for (int num2 = 0; num2 < mPlants.Count; num2++)
		{
			mPlants[num2].SyncObj(theContext);
		}
		for (int num3 = 0; num3 < mProjectiles.Count; num3++)
		{
			mProjectiles[num3].SyncObj(theContext);
		}
		mSeedBank.SyncObj(theContext);
		for (int num4 = 0; num4 < mZombies.Count; num4++)
		{
			mZombies[num4].SyncObj(theContext);
		}
	}

	internal void ClearObj()
	{
		for (int i = 0; i < mCoins.Count; i++)
		{
			mCoins[i].PrepareForReuse();
		}
		mCoins.Clear();
		for (int j = 0; j < mPlants.Count; j++)
		{
			mPlants[j].PrepareForReuse();
		}
		mPlants.Clear();
		for (int k = 0; k < mZombies.Count; k++)
		{
			mZombies[k].PrepareForReuse();
		}
		mZombies.Clear();
		for (int l = 0; l < mLawnMowers.Count; l++)
		{
			mLawnMowers[l].PrepareForReuse();
		}
		mLawnMowers.Clear();
		for (int m = 0; m < mGridItems.Count; m++)
		{
			mGridItems[m].PrepareForReuse();
		}
		mGridItems.Clear();
		for (int n = 0; n < mProjectiles.Count; n++)
		{
			mProjectiles[n].PrepareForReuse();
		}
		mProjectiles.Clear();
	}

	public bool LoadFromFile(Sexy.Buffer b)
	{
		doAddGraveStones = false;
		mLevel = b.ReadLong();
		if (mApp.mGameMode == GameMode.Adventure && mLevel != mApp.mPlayerInfo.mLevel)
		{
			throw new Exception("Board Level does not match player level.");
		}
		mApp.mGameScene = (GameScenes)b.ReadLong();
		PickBackground();
		mPeashootersPlanted = b.ReadLong();
		mNomNomNomAchievementTracker = b.ReadBoolean();
		mNoFungusAmongUsAchievementTracker = b.ReadBoolean();
		mBackground = (BackgroundType)b.ReadLong();
		mBoardFadeOutCounter = b.ReadLong();
		mBoardRandSeed = b.ReadLong();
		mBonusLawnMowersRemaining = b.ReadLong();
		mCatapultPlantsUsed = b.ReadBoolean();
		mChallenge.LoadFromFile(b);
		mChocolateCollected = b.ReadLong();
		mCobCannonCursorDelayCounter = b.ReadLong();
		mCobCannonMouseX = b.ReadLong();
		mCobCannonMouseY = b.ReadLong();
		mCoinBankFadeCount = b.ReadLong();
		int num = b.ReadLong();
		mCoins.Clear();
		for (int i = 0; i < num; i++)
		{
			Coin coin = new Coin();
			coin.LoadFromFile(b);
			mCoins.Add(coin);
		}
		mCoinsCollected = b.ReadLong();
		mCollectedCoinStreak = b.ReadLong();
		mCurrentWave = b.ReadLong();
		mCursorObject.LoadFromFile(b);
		mDaisyMode = b.ReadBoolean();
		mDanceMode = b.ReadBoolean();
		mDiamondsCollected = b.ReadLong();
		mDoomsUsed = b.ReadLong();
		mDroppedFirstCoin = b.ReadBoolean();
		mEffectCounter = b.ReadLong();
		mEnableGraveStones = b.ReadBoolean();
		mFinalBossKilled = b.ReadBoolean();
		mFinalWaveSoundCounter = b.ReadLong();
		mFlagRaiseCounter = b.ReadLong();
		mFogBlownCountDown = b.ReadLong();
		mFogOffset = b.ReadFloat();
		mFwooshCountDown = b.ReadLong();
		mGameID = b.ReadLong();
		mGargantuarsKillsByCornCob = b.ReadLong();
		mGravesCleared = b.ReadLong();
		mGridCelFog = b.ReadLong2DArray();
		mGridCelLook = b.ReadLong2DArray();
		mGridCelOffset = b.ReadLong3DArray();
		int num2 = b.ReadLong();
		mGridItems.Clear();
		for (int j = 0; j < num2; j++)
		{
			GridItem newGridItem = GridItem.GetNewGridItem();
			newGridItem.LoadFromFile(b);
			mGridItems.Add(newGridItem);
		}
		int num3 = b.ReadLong();
		int num4 = b.ReadLong();
		for (int k = 0; k < num3; k++)
		{
			for (int l = 0; l < num4; l++)
			{
				mGridSquareType[k, l] = (GridSquareType)b.ReadLong();
			}
		}
		mHelpDisplayed = b.ReadBooleanArray();
		mHelpIndex = (AdviceType)b.ReadLong();
		mHugeWaveCountDown = b.ReadLong();
		mIceMinX = b.ReadLongArray();
		mIceTimer = b.ReadLongArray();
		mIceTrapCounter = b.ReadLong();
		mIgnoreMouseUp = b.ReadBoolean();
		mKilledYeti = b.ReadBoolean();
		mLastBungeeWave = b.ReadLong();
		mLastToolX = b.ReadLong();
		mLastToolY = b.ReadLong();
		mLastWMUpdateCount = b.ReadLong();
		int num5 = b.ReadLong();
		mLawnMowers.Clear();
		for (int m = 0; m < num5; m++)
		{
			LawnMower newLawnMower = LawnMower.GetNewLawnMower();
			newLawnMower.LoadFromFile(b);
			mLawnMowers.Add(newLawnMower);
		}
		mLevelAwardSpawned = b.ReadBoolean();
		mLevelComplete = b.ReadBoolean();
		mLevelFadeCount = b.ReadLong();
		mLevelStr = b.ReadString();
		mMainCounter = b.ReadLong();
		mMaxSunPlants = b.ReadLong();
		mMinFPS = b.ReadFloat();
		mMushroomAndCoffeeBeansOnly = b.ReadBoolean();
		mMushroomsUsed = b.ReadBoolean();
		mNextSurvivalStageCounter = b.ReadLong();
		mNumSunsFallen = b.ReadLong();
		mNumWaves = b.ReadLong();
		mNutsUsed = b.ReadBoolean();
		mOutOfMoneyCounter = b.ReadLong();
		mPeaShooterUsed = b.ReadBoolean();
		mPinataMode = b.ReadBoolean();
		mPlanternOrBloverUsed = b.ReadBoolean();
		int num6 = b.ReadLong();
		mPlants.Clear();
		for (int n = 0; n < num6; n++)
		{
			Plant newPlant = Plant.GetNewPlant();
			newPlant.LoadFromFile(b);
			if (mApp.IsIZombieLevel())
			{
				mChallenge.IZombieSetupPlant(newPlant);
			}
			mPlants.Add(newPlant);
		}
		mPlantsEaten = b.ReadLong();
		mPlantsShoveled = b.ReadLong();
		mPlayTimeActiveLevel = b.ReadLong();
		mPlayTimeInactiveLevel = b.ReadLong();
		mPottedPlantsCollected = b.ReadLong();
		mProgressMeterWidth = b.ReadLong();
		int num7 = b.ReadLong();
		mProjectiles.Clear();
		for (int num8 = 0; num8 < num7; num8++)
		{
			Projectile newProjectile = Projectile.GetNewProjectile();
			newProjectile.LoadFromFile(b);
			mProjectiles.Add(newProjectile);
		}
		mRiseFromGraveCounter = b.ReadLong();
		int num9 = b.ReadLong();
		for (int num10 = 0; num10 < num9; num10++)
		{
			if (mRowPickingArray[num10] == null)
			{
				mRowPickingArray[num10] = new TodSmoothArray();
			}
			mRowPickingArray[num10].LoadFromFile(b);
		}
		mScoreNextMowerCounter = b.ReadLong();
		mShowShovel = b.ReadBoolean();
		mSeedBank = new SeedBank();
		mSeedBank.LoadFromFile(b);
		mSodPosition = b.ReadLong();
		mSpecialGraveStoneX = b.ReadLong();
		mSpecialGraveStoneY = b.ReadLong();
		mSunCountDown = b.ReadLong();
		mSunMoney = b.ReadLong();
		mSuperMowerMode = b.ReadBoolean();
		mTimeStopCounter = b.ReadLong();
		mTotalSpawnedWaves = b.ReadLong();
		mTriggeredLawnMowers = b.ReadLong();
		mTutorialState = (TutorialState)b.ReadLong();
		mTutorialTimer = b.ReadLong();
		mWaveRowGotLawnMowered = b.ReadLongArray();
		mZombieAllowed = b.ReadBooleanArray();
		mZombieCountDown = b.ReadLong();
		mZombieCountDownStart = b.ReadLong();
		mZombieHealthToNextWave = b.ReadLong();
		mZombieHealthWaveStart = b.ReadLong();
		int num11 = b.ReadLong();
		mZombies.Clear();
		mZombiesRow1.Clear();
		mZombiesRow2.Clear();
		mZombiesRow3.Clear();
		mZombiesRow4.Clear();
		mZombiesRow5.Clear();
		mZombiesRow6.Clear();
		for (int num12 = 0; num12 < num11; num12++)
		{
			Zombie newZombie = Zombie.GetNewZombie();
			newZombie.LoadFromFile(b);
			AddToZombieList(newZombie);
		}
		num3 = b.ReadLong();
		num4 = b.ReadLong();
		for (int num13 = 0; num13 < num3; num13++)
		{
			for (int num14 = 0; num14 < num4; num14++)
			{
				mZombiesInWave[num13, num14] = (ZombieType)b.ReadLong();
			}
		}
		if (b.ReadLong() != 777)
		{
			throw new Exception("Check number mismatch while loading.");
		}
		foreach (Zombie mZombie in mZombies)
		{
			mZombie.LoadingComplete();
		}
		foreach (Plant mPlant in mPlants)
		{
			mPlant.LoadingComplete();
		}
		foreach (Projectile mProjectile in mProjectiles)
		{
			mProjectile.LoadingComplete();
		}
		foreach (LawnMower mLawnMower in mLawnMowers)
		{
			mLawnMower.LoadingComplete();
		}
		foreach (Coin mCoin in mCoins)
		{
			mCoin.LoadingComplete();
		}
		foreach (GridItem mGridItem in mGridItems)
		{
			mGridItem.LoadingComplete();
		}
		doAddGraveStones = true;
		mApp.mSoundManager.StopAllSounds();
		return true;
	}

	static Board()
	{
		gShownMoreSunTutorial = false;
		needToSortRenderList = true;
		aZombieWeightArray = new TodWeightedArray[40];
		aPickArray = new TodWeightedArray[Constants.MAX_GRIDSIZEY];
		aGridArray = new TodWeightedGridArray[30];
		MAX_GRAVE_STONES = Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY;
		aPicks = new GridItem[MAX_GRAVE_STONES];
		mCelPoints = new TPoint[4];
		cachedChargesStringsFertilizer = new Dictionary<int, string>();
		cachedChargesStringsBugSpray = new Dictionary<int, string>();
		cachedChargesStringsChocolate = new Dictionary<int, string>();
		for (int i = 0; i < aZombieWeightArray.Length; i++)
		{
			aZombieWeightArray[i] = TodWeightedArray.GetNewTodWeightedArray();
		}
		for (int j = 0; j < aPickArray.Length; j++)
		{
			aPickArray[j] = TodWeightedArray.GetNewTodWeightedArray();
		}
		for (int k = 0; k < aGridArray.Length; k++)
		{
			aGridArray[k] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
	}

	public Board()
		: this(GlobalStaticVars.gLawnApp)
	{
		SetFullRect();
	}

	private void SetFullRect()
	{
		FullRect = new TRect(-Constants.Board_Offset_AspectRatio_Correction, mY, mWidth, mHeight);
	}

	public override void Move(int theNewX, int theNewY)
	{
		base.Move(theNewX, theNewY);
		SetFullRect();
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		SetFullRect();
	}

	public override void Resize(TRect theRect)
	{
		base.Resize(theRect);
		SetFullRect();
	}

	protected override void Reset()
	{
		base.Reset();
		mX = Constants.Board_Offset_AspectRatio_Correction;
		SetFullRect();
	}

	public Board(LawnApp theApp)
	{
		mZOrder = -1;
		SetupRenderItems();
		mApp = theApp;
		mApp.mBoard = this;
		mZombies = new List<Zombie>(512);
		mPlants = new List<Plant>(512);
		mProjectiles = new List<Projectile>(512);
		mCoins = new List<Coin>(512);
		mLawnMowers = new List<LawnMower>(32);
		mGridItems = new List<GridItem>(128);
		mApp.mEffectSystem.EffectSystemFreeAll();
		mBoardRandSeed = mApp.mAppRandSeed;
		if (mApp.IsSurvivalMode())
		{
			mBoardRandSeed = RandomNumbers.NextNumber();
		}
		mCoinBankFadeCount = 0;
		mLevelFadeCount = 0;
		mLevel = 0;
		mCursorObject = new CursorObject();
		mCursorPreview = new CursorPreview();
		mSeedBank = new SeedBank();
		mCutScene = new CutScene();
		mSpecialGraveStoneX = -1;
		mSpecialGraveStoneY = -1;
		for (int i = 0; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j < Constants.MAX_GRIDSIZEY; j++)
			{
				mGridSquareType[i, j] = GridSquareType.Grass;
				mGridCelLook[i, j] = RandomNumbers.NextNumber(20);
				mGridCelOffset[i, j, 0] = RandomNumbers.NextNumber(10) - 5;
				mGridCelOffset[i, j, 1] = RandomNumbers.NextNumber(10) - 5;
			}
			for (int k = 0; k < 7; k++)
			{
				mGridCelFog[i, k] = 0;
			}
		}
		mSunCountDown = 0;
		mShakeCounter = 0;
		mShakeAmountX = 0;
		mShakeAmountY = 0;
		mPaused = false;
		mLevelAwardSpawned = false;
		mFlagRaiseCounter = 0;
		mIceTrapCounter = 0;
		mLevelComplete = false;
		mBoardFadeOutCounter = -1;
		mNextSurvivalStageCounter = 0;
		mScoreNextMowerCounter = 0;
		mProgressMeterWidth = 0;
		mPoolSparklyParticleID = null;
		mFogBlownCountDown = 0;
		mFogOffset = 0f;
		mFwooshCountDown = 0;
		mTimeStopCounter = 0;
		mCobCannonCursorDelayCounter = 0;
		mCobCannonMouseX = 0;
		mCobCannonMouseY = 0;
		mDroppedFirstCoin = false;
		mBonusLawnMowersRemaining = 0;
		mEnableGraveStones = false;
		mHelpIndex = AdviceType.None;
		mEffectCounter = 0;
		mDrawCount = 0;
		mRiseFromGraveCounter = 0;
		mFinalWaveSoundCounter = 0;
		mKilledYeti = false;
		mTriggeredLawnMowers = 0;
		mPlayTimeActiveLevel = 0;
		mPlayTimeInactiveLevel = 0;
		mMaxSunPlants = 0;
		mStartDrawTime = 0;
		mIntervalDrawTime = 0;
		mIntervalDrawCountStart = 0;
		mPreloadTime = 0;
		mGameID = DateTime.Now.Millisecond;
		mGravesCleared = 0;
		mPlantsEaten = 0;
		mPlantsShoveled = 0;
		mCoinsCollected = 0;
		mDiamondsCollected = 0;
		mPottedPlantsCollected = 0;
		mChocolateCollected = 0;
		mCollectedCoinStreak = 0;
		mGargantuarsKillsByCornCob = 0;
		mMushroomsUsed = false;
		mDoomsUsed = 0;
		mPlanternOrBloverUsed = false;
		mNutsUsed = false;
		mLastToolX = (mLastToolY = -32768);
		mMinFPS = 1000f;
		for (int l = 0; l < Constants.MAX_GRIDSIZEY; l++)
		{
			for (int m = 0; m < 12; m++)
			{
				mFwooshID[l, m] = null;
			}
		}
		mPrevMouseX = -1;
		mPrevMouseY = -1;
		mFinalBossKilled = false;
		mMustacheMode = mApp.mMustacheMode;
		mSuperMowerMode = mApp.mSuperMowerMode;
		mFutureMode = mApp.mFutureMode;
		mPinataMode = mApp.mPinataMode;
		mDanceMode = mApp.mDanceMode;
		mDaisyMode = mApp.mDaisyMode;
		mSukhbirMode = mApp.mSukhbirMode;
		mDoPlaceRose = false;
		mShowShovel = false;
		mAdvice = new MessageWidget(mApp);
		mBackground = BackgroundType.Num1Day;
		mMainCounter = 0;
		mTutorialState = TutorialState.Off;
		mTutorialTimer = -1;
		mTutorialParticleID = null;
		mChallenge = new Challenge();
		mClip = false;
		mDebugTextMode = DebugTextMode.None;
		mMenuButton = new GameButton(0, this);
		mMenuButton.mDrawStoneButton = true;
		mStoreButton = null;
		mAccelerateButton = null;
		mPauseButton = null;
		mIgnoreMouseUp = false;
		if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			mMenuButton.SetLabel("[MAIN_MENU_BUTTON]");
			mMenuButton.Resize(Constants.UIMenuButtonPosition.X, Constants.UIMenuButtonPosition.Y, Constants.UIMenuButtonWidth, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
			mStoreButton = new GameButton(1, this);
			mStoreButton.mButtonImage = AtlasResources.IMAGE_ZENSHOPBUTTON;
			mStoreButton.mOverImage = AtlasResources.IMAGE_ZENSHOPBUTTON_HIGHLIGHT;
			mStoreButton.mDownImage = AtlasResources.IMAGE_ZENSHOPBUTTON_HIGHLIGHT;
			mStoreButton.mParentWidget = this;
			mStoreButton.Resize(Constants.ZenGardenStoreButtonX, Constants.ZenGardenStoreButtonY, AtlasResources.IMAGE_ZENSHOPBUTTON.mWidth, AtlasResources.IMAGE_ZENSHOPBUTTON.mHeight);
		}
		else
		{
			mMenuButton.SetLabel("[MENU_BUTTON]");
			mMenuButton.Resize(Constants.UIMenuButtonPosition.X, Constants.UIMenuButtonPosition.Y, Constants.UIMenuButtonWidth, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
			mAccelerateButton = new GameButton(2, this);
			mAccelerateButton.mDrawStoneButton = true;
			if (mApp.mPlayerInfo.mClassicSeedChooser)
			{
				mAccelerateButton.Resize(755, Constants.BOARD_HEIGHT - AtlasResources.IMAGE_BUTTON_LEFT.mHeight, 45, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
			}
			else if (Constants.Is500pMode)
			{
				mAccelerateButton.Resize(Constants.UIMenuButtonPosition.X + 55, Constants.UIMenuButtonPosition.Y + 45, 45, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
			}
			else
			{
				mAccelerateButton.Resize(Constants.UIMenuButtonPosition.X - 45, Constants.UIMenuButtonPosition.Y, 45, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
			}
			if (HasPauseButton())
			{
				mPauseButton = new GameButton(3, this);
				mPauseButton.mDrawStoneButton = true;
				if (mApp.mPlayerInfo.mClassicSeedChooser)
				{
					mPauseButton.Resize(710, Constants.BOARD_HEIGHT - AtlasResources.IMAGE_BUTTON_LEFT.mHeight, 45, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
				}
				else if (Constants.Is500pMode)
				{
					mPauseButton.Resize(Constants.UIMenuButtonPosition.X, Constants.UIMenuButtonPosition.Y + 45, 45, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
				}
				else
				{
					mPauseButton.Resize(Constants.UIMenuButtonPosition.X - 90, Constants.UIMenuButtonPosition.Y, 45, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
				}
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			mStoreButton = new GameButton(1, this);
			mStoreButton.mDrawStoneButton = true;
			mStoreButton.mBtnNoDraw = true;
			mStoreButton.mDisabled = true;
		}
		if (mApp.mGameMode == GameMode.Upsell)
		{
			mMenuButton.SetLabel("[MAIN_MENU_BUTTON]");
			mMenuButton.Resize(Constants.UIMenuButtonPosition.X, 2, Constants.UIMenuButtonWidth, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
			mStoreButton = new GameButton(1, this);
			mStoreButton.mDrawStoneButton = true;
			mStoreButton.mBtnNoDraw = true;
			mStoreButton.Resize(mStoreButton.mX, mStoreButton.mY, mStoreButton.mWidth + 200, mStoreButton.mHeight);
			mStoreButton.SetLabel("[GET_FULL_VERSION_BUTTON]");
		}
		if (mApp.IsCornRouletteLevel())
		{
			int theY = (Constants.Is500pMode ? 185 : 227);
			mCornRouletteSelfButton = new GameButton(4, this)
			{
				mDrawStoneButton = true,
				mBtnNoDraw = true
			};
			mCornRouletteSelfButton.SetLabel("[CORN_ROULETTE_CHOICE_SELF]");
			mCornRouletteSelfButton.Resize(Constants.Is500pMode ? 151 : 118, theY, 163, 46);
			mCornRouletteOtherButton = new GameButton(5, this)
			{
				mDrawStoneButton = true,
				mBtnNoDraw = true
			};
			mCornRouletteOtherButton.SetLabel("[CORN_ROULETTE_CHOICE_OTHER]");
			mCornRouletteOtherButton.Resize(Constants.Is500pMode ? 486 : 519, theY, 163, 46);
		}
		Camera camera = new Camera(this, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
		if (Constants.BoardCameraScaleMultiplier != 1f)
		{
			camera.SetPosition(0f, Constants.Is500pMode ? (-50f * Constants.BoardCameraScaleMultiplier) : 0f, Constants.BoardCameraScaleMultiplier);
		}
		AttachCamera(camera);
	}

	public override void Dispose()
	{
		mAdvice.Dispose();
		mMenuButton.Dispose();
		if (mStoreButton != null)
		{
			mStoreButton.Dispose();
		}
		mCornRouletteSelfButton?.Dispose();
		mCornRouletteOtherButton?.Dispose();
		mZombiesRow1.Clear();
		mZombiesRow2.Clear();
		mZombiesRow3.Clear();
		mZombiesRow4.Clear();
		mZombiesRow5.Clear();
		mZombiesRow6.Clear();
		for (int i = 0; i < mZombies.Count; i++)
		{
			mZombies[i].PrepareForReuse();
		}
		mZombies.Clear();
		for (int j = 0; j < mPlants.Count; j++)
		{
			mPlants[j].PrepareForReuse();
		}
		mPlants.Clear();
		for (int k = 0; k < mProjectiles.Count; k++)
		{
			mProjectiles[k].PrepareForReuse();
		}
		mProjectiles.Clear();
		mCoins.Clear();
		for (int l = 0; l < mLawnMowers.Count; l++)
		{
			mLawnMowers[l].PrepareForReuse();
		}
		mLawnMowers.Clear();
		for (int m = 0; m < mGridItems.Count; m++)
		{
			mGridItems[m].PrepareForReuse();
		}
		mGridItems.Clear();
		for (int n = 0; n < aRenderList.Length; n++)
		{
			aRenderList[n].PrepareForReuse();
		}
		mCutScene.Dispose();
		mApp.DelayLoadBackgroundResource(string.Empty);
		RemoveAllWidgets();
	}

	public void DisposeBoard()
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mApp.mZenGarden.LeaveGarden();
		}
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			mChallenge.TreeOfWisdomLeave();
		}
		mApp.mSoundSystem.StopFoley(FoleyType.Rain);
		mApp.mZenGarden.mBoard = null;
		mApp.CrazyDaveDie();
		mApp.mEffectSystem.EffectSystemFreeAll();
	}

	public static void BoardInitForPlayer()
	{
		gShownMoreSunTutorial = false;
	}

	public int CountSunBeingCollected()
	{
		int num = 0;
		Coin theCoin = null;
		while (IterateCoins(ref theCoin))
		{
			if (theCoin.mIsBeingCollected && theCoin.IsSun())
			{
				num += theCoin.GetSunValue();
			}
		}
		return num;
	}

	public void DrawGameObjects(Graphics g)
	{
		Vector2 mPhysicalWind = g.mPhysicalWind;
		g.mPhysicalWind = mWindSum;
		int theCurRenderItem = 0;
		bool flag = mChallenge.IsStormyNightPitchBlack();
		bool isGridFocusActive = IsGridFocusActive;
		if (!flag)
		{
			int count = mPlants.Count;
			for (int i = 0; i < count; i++)
			{
				Plant plant = mPlants[i];
				if (!plant.mDead && plant.mOnBungeeState == PlantOnBungeeState.NotOnBungee && mX + plant.mX >= -32)
				{
					GlobalMembersBoard.AddGameObjectRenderItemPlant(aRenderList, ref theCurRenderItem, RenderObjectType.Plant, plant);
					if (mApp.mGameMode == GameMode.ChallengeZenGarden && plant.mPottedPlantIndex != -1)
					{
						RenderItem renderItem = aRenderList[theCurRenderItem];
						renderItem.mRenderObjectType = RenderObjectType.PlantOverlay;
						renderItem.mZPos = MakeRenderOrder(RenderLayer.Particle, 0, plant.mY);
						renderItem.mGameObject = plant;
						renderItem.mPlant = plant;
						renderItem.id = theCurRenderItem;
						theCurRenderItem++;
					}
					if ((plant.mSeedType == SeedType.Magnetshroom || plant.mSeedType == SeedType.GoldMagnet) && plant.DrawMagnetItemsOnTop())
					{
						RenderItem renderItem2 = aRenderList[theCurRenderItem];
						renderItem2.mRenderObjectType = RenderObjectType.PlantMagnetItems;
						renderItem2.mZPos = MakeRenderOrder(RenderLayer.Top, 0, -1);
						renderItem2.mGameObject = plant;
						renderItem2.mPlant = plant;
						renderItem2.id = theCurRenderItem;
						theCurRenderItem++;
					}
				}
			}
		}
		Coin theCoin = null;
		while (IterateCoins(ref theCoin))
		{
			GlobalMembersBoard.AddGameObjectRenderItemCoin(aRenderList, ref theCurRenderItem, RenderObjectType.Coin, theCoin);
		}
		if (!flag)
		{
			int count2 = mZombies.Count;
			for (int j = 0; j < count2; j++)
			{
				Zombie zombie = mZombies[j];
				if (zombie.mDead)
				{
					continue;
				}
				if (zombie.mZombieType == ZombieType.Boss)
				{
					AddBossRenderItem(aRenderList, ref theCurRenderItem, zombie);
					continue;
				}
				GlobalMembersBoard.AddGameObjectRenderItemZombie(aRenderList, ref theCurRenderItem, RenderObjectType.Zombie, zombie);
				if (zombie.HasShadow() && !GlobalStaticVars.gLowFramerate)
				{
					RenderItem renderItem3 = aRenderList[theCurRenderItem];
					renderItem3.mRenderObjectType = RenderObjectType.ZombieShadow;
					renderItem3.mZPos = 200000 + 10000 * zombie.mRow + 3;
					renderItem3.mGameObject = zombie;
					renderItem3.mZombie = zombie;
					renderItem3.id = theCurRenderItem;
					theCurRenderItem++;
				}
				if (zombie.mZombieType == ZombieType.Bungee)
				{
					RenderItem renderItem4 = aRenderList[theCurRenderItem];
					renderItem4.mRenderObjectType = RenderObjectType.ZombieBungeeTarget;
					renderItem4.mZPos = MakeRenderOrder(RenderLayer.Projectile, zombie.mRow, 1);
					renderItem4.mGameObject = zombie;
					renderItem4.mZombie = zombie;
					renderItem4.id = theCurRenderItem;
					theCurRenderItem++;
				}
			}
			int index = -1;
			Projectile theProjectile = null;
			while (IterateProjectiles(ref theProjectile, ref index))
			{
				GlobalMembersBoard.AddGameObjectRenderItemProjectile(aRenderList, ref theCurRenderItem, RenderObjectType.Projectile, theProjectile);
				if (!GlobalStaticVars.gLowFramerate)
				{
					RenderItem renderItem5 = aRenderList[theCurRenderItem];
					renderItem5.mRenderObjectType = RenderObjectType.ProjectileShadow;
					renderItem5.mZPos = 305000 + 10000 * theProjectile.mRow + 3;
					renderItem5.mGameObject = theProjectile;
					renderItem5.mProjectile = theProjectile;
					renderItem5.id = theCurRenderItem;
					theCurRenderItem++;
				}
			}
			LawnMower theLawnMower = null;
			while (IterateLawnMowers(ref theLawnMower))
			{
				RenderItem renderItem6 = aRenderList[theCurRenderItem];
				renderItem6.mRenderObjectType = RenderObjectType.Mower;
				renderItem6.mZPos = theLawnMower.mRenderOrder - 5000;
				renderItem6.mMower = theLawnMower;
				renderItem6.id = theCurRenderItem;
				theCurRenderItem++;
			}
			int index2 = -1;
			TodParticleSystem theParticle = null;
			while (IterateParticles(ref theParticle, ref index2))
			{
				if (!theParticle.mIsAttachment && (!isGridFocusActive || theParticle != mTutorialParticleID))
				{
					RenderItem renderItem7 = aRenderList[theCurRenderItem];
					renderItem7.mRenderObjectType = RenderObjectType.Particle;
					renderItem7.mZPos = theParticle.mRenderOrder;
					renderItem7.mParticleSytem = theParticle;
					renderItem7.id = theCurRenderItem;
					theCurRenderItem++;
				}
			}
			int index3 = -1;
			Reanimation theReanimation = null;
			while (IterateReanimations(ref theReanimation, ref index3))
			{
				if (!theReanimation.mIsAttachment)
				{
					RenderItem renderItem8 = aRenderList[theCurRenderItem];
					renderItem8.mRenderObjectType = RenderObjectType.Reanimation;
					renderItem8.mZPos = theReanimation.mRenderOrder;
					renderItem8.mReanimation = theReanimation;
					renderItem8.id = theCurRenderItem;
					theCurRenderItem++;
				}
			}
			int index4 = -1;
			GridItem theGridItem = null;
			while (IterateGridItems(ref theGridItem, ref index4))
			{
				RenderItem renderItem9 = aRenderList[theCurRenderItem];
				renderItem9.mRenderObjectType = RenderObjectType.GridItem;
				renderItem9.mZPos = theGridItem.mRenderOrder;
				renderItem9.mGridItem = theGridItem;
				renderItem9.id = theCurRenderItem;
				theCurRenderItem++;
				if (mApp.mGameMode == GameMode.ChallengeZenGarden && theGridItem.mGridItemType == GridItemType.Stinky)
				{
					RenderItem renderItem10 = aRenderList[theCurRenderItem];
					renderItem10.mRenderObjectType = RenderObjectType.GridItemOverlay;
					renderItem10.mZPos = MakeRenderOrder(RenderLayer.Particle, 0, (int)theGridItem.mPosY - 30);
					renderItem10.mGridItem = theGridItem;
					theCurRenderItem++;
				}
			}
		}
		for (int k = 0; k < Constants.MAX_GRIDSIZEY; k++)
		{
			if (mIceTimer[k] > 0)
			{
				RenderItem renderItem11 = aRenderList[theCurRenderItem];
				renderItem11.mRenderObjectType = RenderObjectType.Ice;
				renderItem11.mBoardGridY = k;
				renderItem11.mZPos = GetIceZPos(k);
				renderItem11.id = theCurRenderItem;
				theCurRenderItem++;
			}
		}
		int thePosZ;
		if (mTimeStopCounter > 0)
		{
			thePosZ = 800000;
		}
		else
		{
			thePosZ = ((mApp.mGameScene != GameScenes.Playing && mApp.mGameScene != GameScenes.ZombiesWon && !mCutScene.IsAfterSeedChooser() && !mCutScene.IsInShovelTutorial() && mHelpIndex != AdviceType.ClickToContinue) ? 800000 : 100001);
		}
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Backdrop, 100000);
		if (!isGridFocusActive)
		{
			GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.BottomUi, thePosZ);
			GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.CoinBank, 600000);
			GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.TopUi, 700000);
		}
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.ScreenFade, 900000);
		if (mApp.mGameScene == GameScenes.ZombiesWon)
		{
			int thePosZ2 = MakeRenderOrder(RenderLayer.GraveStone, 3, 2);
			GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.DoorMask, thePosZ2);
		}
		if (StageHasFog())
		{
			GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Fog, 500000);
		}
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover0, MakeRenderOrder(RenderLayer.CoverLayer, 0, 0));
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover1, MakeRenderOrder(RenderLayer.CoverLayer, 1, 0));
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover2, MakeRenderOrder(RenderLayer.CoverLayer, 2, 0));
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover3, MakeRenderOrder(RenderLayer.CoverLayer, 3, 0));
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover4, MakeRenderOrder(RenderLayer.CoverLayer, 4, 0));
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover5, MakeRenderOrder(RenderLayer.CoverLayer, 5, 0));
		GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Cover6, MakeRenderOrder(RenderLayer.CoverLayer, 6, 0));
		if (HasWeather())
		{
			GlobalMembersBoard.AddUIRenderItem(aRenderList, ref theCurRenderItem, RenderObjectType.Storm, 500003);
		}
		if (!isGridFocusActive)
		{
			GlobalMembersBoard.AddGameObjectRenderItemCursorPreview(aRenderList, ref theCurRenderItem, RenderObjectType.CursorPreview, mCursorPreview);
		}
		if (needToSortRenderList)
		{
			Array.Sort(aRenderList, 0, theCurRenderItem, null);
		}
		SortZombieRowLists();
		for (int l = 0; l < theCurRenderItem; l++)
		{
			RenderItem renderItem12 = aRenderList[l];
			if (renderItem12.mRenderObjectType == 0)
			{
				continue;
			}
			switch (renderItem12.mRenderObjectType)
			{
			case RenderObjectType.GridItem:
				if (renderItem12.mGridItem.mGridItemType == GridItemType.Gravestone)
				{
					renderItem12.mGridItem.DrawGridItem(g);
				}
				break;
			case RenderObjectType.Backdrop:
				if (!flag)
				{
					DrawBackdrop(g);
					if (!isGridFocusActive)
					{
						DrawCursorOnBackground(g);
					}
				}
				break;
			case RenderObjectType.Ice:
				DrawIce(g, renderItem12.mBoardGridY);
				break;
			}
		}
		for (int m = 0; m < theCurRenderItem; m++)
		{
			RenderItem renderItem13 = aRenderList[m];
			if (renderItem13.mRenderObjectType == 0)
			{
				continue;
			}
			switch (renderItem13.mRenderObjectType)
			{
			case RenderObjectType.Coin:
				if (renderItem13.mCoin.BeginDraw(g))
				{
					renderItem13.mCoin.Draw(g);
					renderItem13.mCoin.EndDraw(g);
				}
				break;
			case RenderObjectType.Projectile:
				if (renderItem13.mProjectile.BeginDraw(g))
				{
					renderItem13.mProjectile.Draw(g);
					renderItem13.mProjectile.EndDraw(g);
				}
				break;
			case RenderObjectType.Zombie:
				if (renderItem13.mZombie.BeginDraw(g))
				{
					renderItem13.mZombie.Draw(g);
					renderItem13.mZombie.EndDraw(g);
				}
				break;
			case RenderObjectType.ZombieShadow:
				if (renderItem13.mZombie.BeginDraw(g))
				{
					renderItem13.mZombie.DrawShadow(g);
					renderItem13.mZombie.EndDraw(g);
				}
				break;
			case RenderObjectType.ZombieBungeeTarget:
				renderItem13.mZombie.DrawBungeeTarget(g);
				break;
			case RenderObjectType.Plant:
				if (renderItem13.mPlant.BeginDraw(g))
				{
					renderItem13.mPlant.Draw(g);
					renderItem13.mPlant.EndDraw(g);
				}
				break;
			case RenderObjectType.PlantOverlay:
				if (renderItem13.mPlant.BeginDraw(g))
				{
					mApp.mZenGarden.DrawPlantOverlay(g, renderItem13.mPlant);
					renderItem13.mPlant.EndDraw(g);
				}
				break;
			case RenderObjectType.PlantMagnetItems:
				if (renderItem13.mPlant.BeginDraw(g))
				{
					renderItem13.mPlant.DrawMagnetItems(g);
					renderItem13.mPlant.EndDraw(g);
				}
				break;
			case RenderObjectType.CursorPreview:
				if (renderItem13.mCursorPreview.BeginDraw(g))
				{
					renderItem13.mCursorPreview.Draw(g);
					renderItem13.mCursorPreview.EndDraw(g);
				}
				break;
			case RenderObjectType.Particle:
				renderItem13.mParticleSytem.Draw(g, doScale: true);
				break;
			case RenderObjectType.Reanimation:
				renderItem13.mReanimation.Draw(g);
				break;
			case RenderObjectType.TopUi:
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ResetTransform(g);
				}
				DrawUITop(g);
				if (mCursorObject.mMagnifier)
				{
					DrawCursorOverlay(g);
				}
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ApplyTransform(g);
				}
				if (flag)
				{
					DrawCursorOnBackground(g);
				}
				if (!mCursorObject.mMagnifier)
				{
					DrawCursorOverlay(g);
				}
				break;
			case RenderObjectType.Cover0:
				DrawCoverLayer(g, 0);
				break;
			case RenderObjectType.Cover1:
				DrawCoverLayer(g, 1);
				break;
			case RenderObjectType.Cover2:
				DrawCoverLayer(g, 2);
				break;
			case RenderObjectType.Cover3:
				DrawCoverLayer(g, 3);
				break;
			case RenderObjectType.Cover4:
				DrawCoverLayer(g, 4);
				break;
			case RenderObjectType.Cover5:
				DrawCoverLayer(g, 5);
				break;
			case RenderObjectType.Cover6:
				DrawCoverLayer(g, 6);
				break;
			case RenderObjectType.Fog:
				DrawFog(g);
				break;
			case RenderObjectType.Storm:
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ResetTransform(g);
				}
				mChallenge.DrawWeather(g);
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ApplyTransform(g);
				}
				break;
			case RenderObjectType.BottomUi:
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ResetTransform(g);
				}
				DrawUIBottom(g);
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ApplyTransform(g);
				}
				break;
			case RenderObjectType.DoorMask:
				DrawHouseDoorTop(g);
				break;
			case RenderObjectType.CoinBank:
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ResetTransform(g);
				}
				DrawUICoinBank(g);
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ApplyTransform(g);
				}
				break;
			case RenderObjectType.ProjectileShadow:
				if (renderItem13.mProjectile.BeginDraw(g))
				{
					renderItem13.mProjectile.DrawShadow(g);
					renderItem13.mProjectile.EndDraw(g);
				}
				break;
			case RenderObjectType.Mower:
				renderItem13.mMower.Draw(g);
				break;
			case RenderObjectType.ScreenFade:
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ResetTransform(g);
				}
				DrawFadeOut(g);
				if (mCameraEnabled && mCamera != null)
				{
					mCamera.ApplyTransform(g);
				}
				break;
			case RenderObjectType.BossPart:
			{
				Zombie bossZombie = GetBossZombie();
				if (bossZombie != null && bossZombie.BeginDraw(g))
				{
					bossZombie.DrawBossPart(g, renderItem13.mBossPart);
					bossZombie.EndDraw(g);
				}
				break;
			}
			case RenderObjectType.GridItem:
				if (renderItem13.mGridItem.mGridItemType != GridItemType.Gravestone)
				{
					renderItem13.mGridItem.DrawGridItem(g);
				}
				break;
			case RenderObjectType.GridItemOverlay:
				renderItem13.mGridItem.DrawGridItemOverlay(g);
				break;
			}
		}
		g.mPhysicalWind = mPhysicalWind;
	}

	public void ClearCursor()
	{
		if (mAdvice.mDuration > 0 && (mHelpIndex == AdviceType.PlantGravebustersOnGraves || mHelpIndex == AdviceType.PlantLilypadOnWater || mHelpIndex == AdviceType.PlantTanglekelpOnWater || mHelpIndex == AdviceType.PlantSeashroomOnWater || mHelpIndex == AdviceType.PlantPotatoMineOnLily || mHelpIndex == AdviceType.PlantWrongArtType || mHelpIndex == AdviceType.PlantNeedPot || mHelpIndex == AdviceType.PlantNotPassedLine || mHelpIndex == AdviceType.PlantOnlyOnRepeaters || mHelpIndex == AdviceType.PlantOnlyOnMelonpult || mHelpIndex == AdviceType.PlantOnlyOnSunflower || mHelpIndex == AdviceType.PlantOnlyOnKernelpult || mHelpIndex == AdviceType.PlantOnlyOnChomper))
		{
			ClearAdvice(mHelpIndex);
		}
		if (mCursorObject.mCursorType == CursorType.PlantFromUsableCoin && mCursorObject.mCoinID != null)
		{
			mCursorObject.mCoinID.mIsBeingCollected = false;
		}
		mCursorObject.mType = SeedType.None;
		mCursorObject.mCursorType = CursorType.Normal;
		mCursorObject.mMagnifier = false;
		mCursorObject.mSeedBankIndex = -1;
		mCursorObject.mCoinID = null;
		mCursorObject.mDuplicatorPlantID = null;
		if (mCursorObject.mCobCannonPlantID != null && (mCursorObject.mCobCannonPlantID.mSeedType == SeedType.Agave || mCursorObject.mCobCannonPlantID.mSeedType == SeedType.Endoflame) && mCursorObject.mCobCannonPlantID.AgaveSkillCanCancel())
		{
			mCursorObject.mCobCannonPlantID.mState = PlantState.AgavePowerfulTendToCanceling;
		}
		mCursorObject.mCobCannonPlantID = null;
		if (mCursorObject.mGlovePlantID != null && mCursorObject.mGlovePlantID.mGloveGrabbed)
		{
			if (mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom)
			{
				if (CanPlantAt(mCursorObject.mGlovePlantID.mPlantCol, mCursorObject.mGlovePlantID.mRow, mCursorObject.mGlovePlantID.mSeedType, aIsMovePlant: true) == PlantingReason.Ok)
				{
					mChallenge.MovePlant(mCursorObject.mGlovePlantID, mCursorObject.mGlovePlantID.mPlantCol, mCursorObject.mGlovePlantID.mRow);
					mCursorObject.mGlovePlantID.mGloveGrabbed = false;
				}
				else
				{
					mCursorObject.mGlovePlantID.mGloveGrabbed = false;
					mCursorObject.mGlovePlantID.Die();
				}
			}
			else
			{
				mApp.mZenGarden.MovePlant(mCursorObject.mGlovePlantID, mCursorObject.mGlovePlantID.mPlantCol, mCursorObject.mGlovePlantID.mRow);
				mCursorObject.mGlovePlantID.mGloveGrabbed = false;
			}
		}
		mCursorObject.mGlovePlantID = null;
		mChallenge.ClearCursor();
		if (mTutorialState == TutorialState.Level1PlantPeashooter)
		{
			SetTutorialState(TutorialState.Level1PickUpPeashooter);
		}
		else if (mTutorialState == TutorialState.Level2PlantSunflower || mTutorialState == TutorialState.Level2RefreshSunflower)
		{
			if (!mSeedBank.mSeedPackets[1].CanPickUp())
			{
				SetTutorialState(TutorialState.Level2RefreshSunflower);
			}
			else
			{
				SetTutorialState(TutorialState.Level2PickUpSunflower);
			}
		}
		else if (mTutorialState != TutorialState.MoresunPlantSunflower && mTutorialState != TutorialState.MoresunRefreshSunflower)
		{
			if (mTutorialState == TutorialState.ShovelDig)
			{
				SetTutorialState(TutorialState.ShovelPickup);
			}
		}
		else if (!mSeedBank.mSeedPackets[1].CanPickUp())
		{
			SetTutorialState(TutorialState.MoresunRefreshSunflower);
		}
		else
		{
			SetTutorialState(TutorialState.MoresunPickUpSunflower);
		}
	}

	public bool AreEnemyZombiesOnScreen()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie.mHasHead && !zombie.IsDeadOrDying() && !zombie.mMindControlled)
			{
				return true;
			}
		}
		return false;
	}

	public LawnMower FindLawnMowerInRow(int theRow)
	{
		LawnMower theLawnMower = null;
		while (IterateLawnMowers(ref theLawnMower))
		{
			if (theLawnMower.mRow == theRow)
			{
				return theLawnMower;
			}
		}
		return null;
	}

	public void SaveGame(string theFilePath)
	{
		GlobalMembersSaveGame.LawnSaveGame(this, theFilePath);
	}

	public bool LoadGame(string theFilePath)
	{
		if (!GlobalMembersSaveGame.LawnLoadGame(this, theFilePath) && !GlobalMembersSaveGame.LawnLoadGameOld(this, theFilePath))
		{
			GlobalMembersSaveGame.LawnCreateCrashBackup(theFilePath);
			return false;
		}
		LoadBackgroundImages();
		ResetFPSStats();
		UpdateLayers();
		Debug.Log(DebugType.Info, $"{GetType()}: Loaded gamemode {mApp?.mGameMode}:\"{TodStringFile.TodStringTranslate(mLevelStr ?? string.Empty)}\" ({(int)(mApp?.mGameMode).Value}:{mLevel})");
		return true;
	}

	public void InitLevel()
	{
		mMainCounter = 0;
		mEnableGraveStones = false;
		mSodPosition = 0;
		mPrevBoardResult = mApp.mBoardResult;
		if (mApp.mGameMode != GameMode.TreeOfWisdom && mApp.mGameMode != GameMode.ChallengeZenGarden)
		{
			mApp.mMusic.StopAllMusic();
		}
		mNomNomNomAchievementTracker = true;
		mNoFungusAmongUsAchievementTracker = true;
		mPeaShooterUsed = false;
		mCatapultPlantsUsed = false;
		mMushroomAndCoffeeBeansOnly = true;
		if (mApp.IsAdventureMode())
		{
			mLevel = mApp.mPlayerInfo.GetLevel();
		}
		else if (mApp.IsQuickPlayMode())
		{
			mLevel = (int)(mApp.mGameMode - 72 + 1);
		}
		else
		{
			mLevel = 0;
		}
		mLevelStr = TodStringFile.TodStringTranslate("[LEVEL]") + " " + mApp.GetStageString(mLevel);
		mAgavePowerfulCountdown = 0;
		mEndoflamePowerfulCountdown = 0;
		mSealedCountdown = 0;
		Debug.Log(DebugType.Info, $"{GetType()}: Enter gamemode {mApp?.mGameMode}:\"{TodStringFile.TodStringTranslate(mLevelStr ?? string.Empty)}\" ({(int)(mApp?.mGameMode).Value}:{mLevel})");
		PickBackground();
		mCurrentWave = 0;
		InitZombieWaves();
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist || mApp.IsScaryPotterLevel() || mApp.IsWhackAZombieLevel())
		{
			mSunMoney = 0;
		}
		else if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			mSunMoney = 5000;
		}
		else if (mApp.IsIZombieLevel())
		{
			mSunMoney = 150;
		}
		else if (mApp.IsFirstTimeAdventureMode() && mLevel == 1)
		{
			mSunMoney = 150;
		}
		else if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
		{
			mSunMoney = 1500;
		}
		else if (mApp.mGameMode == GameMode.ImitaterRandom)
		{
			mSunMoney = 500;
		}
		else if (mApp.mGameMode == GameMode.ChallengeFusion)
		{
			mSunMoney = 50;
		}
		else
		{
			mSunMoney = 50;
		}
		if (mApp.IsLevelUseJson())
		{
			mSunMoney = mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mStartingSun;
		}
		for (int i = 0; i < mRowPickingArray.Length; i++)
		{
			mRowPickingArray[i] = new TodSmoothArray();
		}
		for (int j = 0; j < Constants.MAX_GRIDSIZEY; j++)
		{
			mWaveRowGotLawnMowered[j] = -100;
			mIceMinX[j] = Constants.Board_Ice_Start;
			mIceTimer[j] = 0;
			mIceParticleID[j] = ParticleSystemID.Null;
			mRowPickingArray[j].mItem = j;
		}
		mNumSunsFallen = 0;
		if (!StageIsNight())
		{
			mSunCountDown = TodCommon.RandRangeInt(425, 700);
		}
		for (int k = 0; k < 67; k++)
		{
			mHelpDisplayed[k] = false;
		}
		mSeedBank.mNumPackets = GetNumSeedsInBank();
		mSeedBank.UpdateHeight();
		for (int l = 0; l < 10; l++)
		{
			SeedPacket seedPacket = mSeedBank.mSeedPackets[l];
			seedPacket.mIndex = l;
			if (mApp.mPlayerInfo.mClassicSeedChooser)
			{
				seedPacket.mX = GetSeedPacketPositionX(l);
				seedPacket.mY = 8;
			}
			else
			{
				seedPacket.mX = 0;
				seedPacket.mY = GetSeedPacketPositionY(l);
			}
			seedPacket.mPacketType = SeedType.None;
		}
		if (!mApp.IsLevelUseJson() || mApp.mCreativeLevel.GetComponent<CSSeedBank>() == null)
		{
			if (mApp.IsSlotMachineLevel())
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Sunflower, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Peashooter, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Snowpea, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.ChallengeIce)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 6);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Peashooter, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Cherrybomb, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Wallnut, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.Repeater, SeedType.None);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.Snowpea, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.Chomper, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie1)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombieFootball, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie2)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombieScreenDoor, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombiePail, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie3)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombieDigger, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie4)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombieLadder, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie5)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 4);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombieBungee, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ZombieBalloon, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie6)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 4);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiePolevaulter, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ZombieGargantuar, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie7)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 4);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieNormal, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombiePolevaulter, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ZombieDancer, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie8)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 6);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieImp, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombieTrafficCone, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ZombieBungee, SeedType.None);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.ZombieDigger, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.ZombieLadder, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombie9)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 8);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieImp, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombieTrafficCone, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombiePolevaulter, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.ZombieBungee, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.ZombieDigger, SeedType.None);
				mSeedBank.mSeedPackets[6].SetPacketType(SeedType.ZombieLadder, SeedType.None);
				mSeedBank.mSeedPackets[7].SetPacketType(SeedType.ZombieFootball, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.PuzzleIZombieEndless)
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 9);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ZombieImp, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ZombieTrafficCone, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ZombiePolevaulter, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ZombiePail, SeedType.None);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.ZombieBungee, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.ZombieDigger, SeedType.None);
				mSeedBank.mSeedPackets[6].SetPacketType(SeedType.ZombieLadder, SeedType.None);
				mSeedBank.mSeedPackets[7].SetPacketType(SeedType.ZombieFootball, SeedType.None);
				mSeedBank.mSeedPackets[8].SetPacketType(SeedType.ZombieDancer, SeedType.None);
			}
			else if (mApp.mGameMode == GameMode.ImitaterRandom)
			{
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.ImitaterRandomPlant, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.ImitaterRandomPlant, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.ImitaterRandomPlant, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.ImitaterRandomPlant, SeedType.None);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.ImitaterRandomPlant, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.ImitaterRandomPlant, SeedType.None);
				mSeedBank.mSeedPackets[6].SetPacketType(SeedType.ImitaterRandomZombie, SeedType.None);
				mSeedBank.mSeedPackets[7].SetPacketType(SeedType.ImitaterRandomZombie, SeedType.None);
				mSeedBank.mSeedPackets[8].SetPacketType(SeedType.ImitaterRandomZombie, SeedType.None);
				mSeedBank.mSeedPackets[9].SetPacketType(SeedType.InstantCoffee, SeedType.None);
				mSeedBank.mNumPackets = 10;
			}
			else if (mApp.mGameMode == GameMode.MarvelousPeople4)
			{
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Sunflower, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Potatomine, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Squash, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.Jalapeno, SeedType.None);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.Repeater, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.Torchwood, SeedType.None);
				mSeedBank.mSeedPackets[6].SetPacketType(SeedType.Blover, SeedType.None);
				mSeedBank.mSeedPackets[7].SetPacketType(SeedType.Wallnut, SeedType.None);
				mSeedBank.mSeedPackets[8].SetPacketType(SeedType.Endoflame, SeedType.None);
				mSeedBank.mSeedPackets[9].SetPacketType(SeedType.PickledPepper, SeedType.None);
				mSeedBank.mNumPackets = 10;
			}
			else if (mApp.mGameMode == GameMode.ChallengeFusion)
			{
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Peashooter, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Imitater, SeedType.Peashooter);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Sunflower, SeedType.None);
				mSeedBank.mSeedPackets[3].SetPacketType(SeedType.Imitater, SeedType.Sunflower);
				mSeedBank.mSeedPackets[4].SetPacketType(SeedType.Lilypad, SeedType.None);
				mSeedBank.mSeedPackets[5].SetPacketType(SeedType.Cattail, SeedType.None);
				mSeedBank.mSeedPackets[6].SetPacketType(SeedType.Hypnoshroom, SeedType.None);
				mSeedBank.mSeedPackets[7].SetPacketType(SeedType.Cherrybomb, SeedType.None);
				mSeedBank.mSeedPackets[8].SetPacketType(SeedType.Wallnut, SeedType.None);
				mSeedBank.mSeedPackets[9].SetPacketType(SeedType.Iceshroom, SeedType.None);
				mSeedBank.mNumPackets = 10;
			}
			else if (mApp.IsScaryPotterLevel())
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 1);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Cherrybomb, SeedType.None);
			}
			else if (mApp.IsWhackAZombieLevel() && (mApp.IsAdventureMode() || mApp.IsQuickPlayMode()))
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Potatomine, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Gravebuster, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Cherrybomb, SeedType.None);
			}
			else if (mApp.IsWhackAZombieLevel() && !mApp.IsAdventureMode() && !mApp.IsQuickPlayMode())
			{
				Debug.ASSERT(mSeedBank.mNumPackets == 3);
				mSeedBank.mSeedPackets[0].SetPacketType(SeedType.Potatomine, SeedType.None);
				mSeedBank.mSeedPackets[1].SetPacketType(SeedType.Gravebuster, SeedType.None);
				mSeedBank.mSeedPackets[2].SetPacketType(SeedType.Iceshroom, SeedType.None);
			}
			else if (!ChooseSeedsOnCurrentLevel() && !HasConveyorBeltSeedBank() && !mApp.IsCornRouletteLevel())
			{
				mSeedBank.mNumPackets = GetNumSeedsInBank();
				for (int m = 0; m < mSeedBank.mNumPackets; m++)
				{
					SeedPacket seedPacket2 = mSeedBank.mSeedPackets[m];
					seedPacket2.SetPacketType((SeedType)m, SeedType.None);
				}
			}
		}
		if (mApp.IsLevelUseJson())
		{
			CSSeedBank component = mApp.mCreativeLevel.GetComponent<CSSeedBank>();
			if (component != null && !component.mUserChoose && component.mLockedCards != null)
			{
				if (component.mLockedCards.Count < mSeedBank.mNumPackets)
				{
					mSeedBank.mNumPackets = component.mLockedCards.Count;
				}
				for (int n = 0; n < mSeedBank.mNumPackets; n++)
				{
					SeedPacket seedPacket3 = mSeedBank.mSeedPackets[n];
					CardPair cardPair = component.mLockedCards[n];
					if (cardPair.mImitator)
					{
						seedPacket3.SetPacketType(SeedType.Imitater, cardPair.mSeedType);
					}
					else
					{
						seedPacket3.SetPacketType(cardPair.mSeedType, SeedType.None);
					}
				}
			}
		}
		mWidgetManager.MarkAllDirty();
		mPaused = false;
		mOutOfMoneyCounter = 0;
		if (StageHasFog())
		{
			mFogOffset = 1065f - (float)LeftFogColumn() * 80f;
			mFogBlownCountDown = 200;
		}
		mChallenge.InitLevel();
		SetupRenderItems();
		needToSortRenderList = true;
	}

	private void SetupRenderItems()
	{
		for (int i = 0; i < aRenderList.Length; i++)
		{
			if (aRenderList[i] != null)
			{
				aRenderList[i].PrepareForReuse();
			}
			aRenderList[i] = RenderItem.GetNewRenderItem();
		}
	}

	public void DisplayAdvice(string theAdvice, MessageStyle theMessageStyle, AdviceType theHelpIndex)
	{
		DisplayAdvice(theAdvice, theMessageStyle, theHelpIndex, null);
	}

	public void DisplayAdvice(string theAdvice, MessageStyle theMessageStyle, AdviceType theHelpIndex, Image theIcon)
	{
		if (theHelpIndex != AdviceType.None)
		{
			if (mHelpDisplayed[(int)theHelpIndex])
			{
				return;
			}
			mHelpDisplayed[(int)theHelpIndex] = true;
		}
		mAdvice.SetLabel(theAdvice, theMessageStyle, theIcon);
		mHelpIndex = theHelpIndex;
	}

	public void StartLevel()
	{
		mCoinBankFadeCount = 0;
		mLevelFadeCount = 1000;
		mApp.mLastLevelStats.Reset();
		mChallenge.StartLevel();
		if (!mApp.IsChallengeWithoutSeedBank())
		{
			mSeedBank.MoveToShown();
		}
		if (mApp.IsSurvivalMode() && mChallenge.mSurvivalStage > 0)
		{
			string savedGameName = LawnCommon.GetSavedGameName(mApp.mGameMode, (int)mApp.mPlayerInfo.mId);
			mApp.EraseFile(savedGameName);
		}
		if (mApp.IsSurvivalMode() && mChallenge.mSurvivalStage > 0)
		{
			FreezeEffectsForCutscene(theFreeze: false);
			mApp.mSoundSystem.GamePause(theEnteringPause: false);
		}
		if (mApp.mGameMode != GameMode.ChallengeIce && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mApp.mGameMode != GameMode.Upsell && mApp.mGameMode != GameMode.Intro && mApp.mGameMode != GameMode.Intro && (!mApp.IsFinalBossLevel() || mApp.mGameMode == GameMode.ChallengeFinalBoss2))
		{
			mApp.mMusic.StartGameMusic();
		}
	}

	public Plant AddPlant(int theGridX, int theGridY, SeedType theSeedType, SeedType theImitaterType)
	{
		Plant plant = NewPlant(theGridX, theGridY, theSeedType, theImitaterType);
		DoPlantingEffects(theGridX, theGridY, plant, forAquarium: false);
		mChallenge.PlantAdded(plant);
		int num = CountPlantByType(SeedType.Sunflower) + CountPlantByType(SeedType.Sunshroom);
		if (num > mMaxSunPlants)
		{
			mMaxSunPlants = num;
		}
		SeedType seedType = theSeedType;
		if (seedType == SeedType.Imitater)
		{
			seedType = theImitaterType;
		}
		if (seedType == SeedType.Peashooter)
		{
			mPeashootersPlanted++;
			if (mPeashootersPlanted >= 10)
			{
				if (SexyAppBase.IsInTrialMode)
				{
					if (!mApp.mPlayerInfo.mHasSeenAchievementDialog)
					{
						mApp.mPlayerInfo.mHasSeenAchievementDialog = true;
						mApp.achievementToCheck = AchievementId.SoilYourPlants;
						mApp.DoLockedAchievementDialog(AchievementId.SoilYourPlants);
					}
				}
				else
				{
					GrantAchievement(AchievementId.SoilYourPlants);
				}
			}
		}
		if (seedType != SeedType.Sunflower && seedType != SeedType.Wallnut && seedType != SeedType.Chomper)
		{
			mNomNomNomAchievementTracker = false;
		}
		if (IsFungus(seedType))
		{
			mNoFungusAmongUsAchievementTracker = false;
		}
		if (seedType == SeedType.Peashooter || seedType == SeedType.Snowpea || seedType == SeedType.Repeater || seedType == SeedType.Threepeater || seedType == SeedType.Splitpea || seedType == SeedType.Gatlingpea)
		{
			mPeaShooterUsed = true;
		}
		if (seedType == SeedType.Cabbagepult || seedType == SeedType.Kernelpult || seedType == SeedType.Melonpult || seedType == SeedType.Wintermelon)
		{
			mCatapultPlantsUsed = true;
		}
		bool flag = IsFungus(seedType);
		if (seedType != SeedType.InstantCoffee && !flag)
		{
			mMushroomAndCoffeeBeansOnly = false;
		}
		if (flag)
		{
			mMushroomsUsed = true;
		}
		return plant;
	}

	public void AddSunMoney(int theX, int theY, int theSunMoney, CoinMotion theCoinMotion)
	{
		while (theSunMoney - 25 >= 0)
		{
			theSunMoney -= 25;
			AddCoin(theX, theY, CoinType.Sun, theCoinMotion);
		}
		while (theSunMoney - 15 >= 0)
		{
			theSunMoney -= 15;
			AddCoin(theX, theY, CoinType.Smallsun, theCoinMotion);
		}
		while (theSunMoney - 5 >= 0)
		{
			theSunMoney -= 5;
			AddCoin(theX, theY, CoinType.Tinysun, theCoinMotion);
		}
	}

	private static bool IsPlantingSoundGuaranteed(Plant thePlant)
	{
		return thePlant.mSeedType switch
		{
			SeedType.Jalapeno => true, 
			SeedType.Cattail => true, 
			SeedType.HypnoCattail => true, 
			SeedType.Gravebuster => true, 
			SeedType.Agave => true, 
			SeedType.Pumpkinshell => true, 
			SeedType.Hypnoshroom => true, 
			SeedType.Endoflame => true, 
			SeedType.Imitater => true, 
			SeedType.Spikerock => true, 
			SeedType.Tallnut => true, 
			SeedType.Gatlingpea => true, 
			SeedType.Snowpea => true, 
			SeedType.Iceshroom => true, 
			SeedType.Marigold => true, 
			_ => false, 
		};
	}

	private static bool IsSleepingSoundGuaranteed(Plant thePlant)
	{
		return IsPlantingSoundGuaranteed(thePlant);
	}

	public void AddWind(WindType windType)
	{
		mWinds.Add(new Wind(windType));
	}

	public void DoPlantingSound(Plant thePlant)
	{
		if (thePlant.mSeedType != SeedType.Gravebuster && thePlant.mSeedType != SeedType.Blover)
		{
			SeedType seedType = thePlant.mSeedType;
			if (mApp.IsCattailSkinEnabled() && thePlant.mSeedType == SeedType.Cattail)
			{
				seedType = SeedType.HypnoCattail;
			}
			switch (Random.Shared.Next(IsPlantingSoundGuaranteed(thePlant) ? 2 : 5))
			{
			case 0:
				PlantVoice.Play(seedType, thePlant.mImitaterType, PlantVoice.VoiceType.Plant1);
				break;
			case 1:
				PlantVoice.Play(seedType, thePlant.mImitaterType, PlantVoice.VoiceType.Plant2);
				break;
			}
		}
	}

	public void DoSleepingSound(Plant thePlant)
	{
		switch (Random.Shared.Next(IsSleepingSoundGuaranteed(thePlant) ? 2 : 5))
		{
		case 0:
			PlantVoice.Play(thePlant.mSeedType, thePlant.mImitaterType, PlantVoice.VoiceType.Sleep1);
			break;
		case 1:
			PlantVoice.Play(thePlant.mSeedType, thePlant.mImitaterType, PlantVoice.VoiceType.Sleep2);
			break;
		}
	}

	public bool IsFungus(SeedType aCheckSeed)
	{
		if (aCheckSeed != SeedType.Puffshroom && aCheckSeed != SeedType.Sunshroom && aCheckSeed != SeedType.Fumeshroom && aCheckSeed != SeedType.Hypnoshroom && aCheckSeed != SeedType.Scaredyshroom && aCheckSeed != SeedType.Iceshroom && aCheckSeed != SeedType.Doomshroom && aCheckSeed != SeedType.Magnetshroom && aCheckSeed != SeedType.Seashroom)
		{
			return aCheckSeed == SeedType.Gloomshroom;
		}
		return true;
	}

	public Projectile AddProjectile(int theX, int theY, int aRenderOrder, int theRow, ProjectileType projectileType)
	{
		Projectile newProjectile = Projectile.GetNewProjectile();
		newProjectile.ProjectileInitialize(theX, theY, aRenderOrder, theRow, projectileType);
		mProjectiles.Add(newProjectile);
		return newProjectile;
	}

	public Coin AddCoin(int theX, int theY, CoinType theCoinType, CoinMotion theCoinMotion)
	{
		Coin coin = new Coin();
		coin.CoinInitialize(theX, theY, theCoinType, theCoinMotion);
		mCoins.Add(coin);
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 1)
		{
			DisplayAdvice("[ADVICE_CLICK_ON_SUN]", MessageStyle.TutorialLevel1Stay, AdviceType.ClickOnSun);
		}
		return coin;
	}

	public void AddCoinForDropLootType(int theX, int theY, DropLootType theDropLootType, CoinMotion theCoinMotionOverride = CoinMotion.Coin)
	{
		switch (theDropLootType)
		{
		case DropLootType.None:
			return;
		case DropLootType.PresentMinigames:
			if (!CanDropPresentMinigames())
			{
				theDropLootType = DropLootType.Silver;
				break;
			}
			mApp.PlayFoley(FoleyType.ArtChallenge);
			AddCoin(theX - 40, theY, CoinType.PresentMinigames, theCoinMotionOverride);
			return;
		case DropLootType.PresentPuzzleMode:
			if (!CanDropPresentPuzzleMode())
			{
				theDropLootType = DropLootType.Silver;
				break;
			}
			mApp.PlayFoley(FoleyType.ArtChallenge);
			AddCoin(theX - 40, theY, CoinType.PresentPuzzleMode, theCoinMotionOverride);
			return;
		case DropLootType.ThreeSun:
			mApp.PlayFoley(FoleyType.SpawnSun);
			AddCoin(theX - 20, theY, CoinType.Sun, theCoinMotionOverride);
			AddCoin(theX - 40, theY, CoinType.Sun, theCoinMotionOverride);
			AddCoin(theX - 60, theY, CoinType.Sun, theCoinMotionOverride);
			return;
		case DropLootType.ThreeDiamonds:
			mApp.PlaySample(Resources.SOUND_DIAMOND);
			AddCoin(theX - 20, theY, CoinType.Diamond, theCoinMotionOverride);
			AddCoin(theX - 40, theY, CoinType.Diamond, theCoinMotionOverride);
			AddCoin(theX - 60, theY, CoinType.Diamond, theCoinMotionOverride);
			return;
		}
		CoinType theCoinType = DropLootTypeToCoinType(theDropLootType);
		mApp.PlayFoley(FoleyType.SpawnSun);
		AddCoin(theX - 40, theY, theCoinType, theCoinMotionOverride);
		mDroppedFirstCoin = true;
	}

	public void RefreshSeedPacketFromCursor()
	{
		if (mCursorObject.mCursorType == CursorType.PlantFromUsableCoin)
		{
			Coin coin = mCoins[mCoins.IndexOf(mCursorObject.mCoinID)];
			coin.DroppedUsableSeed();
		}
		else if (mCursorObject.mCursorType == CursorType.PlantFromBank)
		{
			Debug.ASSERT(mCursorObject.mSeedBankIndex >= 0 && mCursorObject.mSeedBankIndex < mSeedBank.mNumPackets);
			SeedPacket seedPacket = mSeedBank.mSeedPackets[mCursorObject.mSeedBankIndex];
			seedPacket.Activate();
		}
		ClearCursor();
	}

	public void DeselectSeedPacket()
	{
		if (mCursorObject.mSeedBankIndex != -1)
		{
			SeedPacket seedPacket = mSeedBank.mSeedPackets[mCursorObject.mSeedBankIndex];
			seedPacket.Activate();
			ClearCursor();
		}
	}

	public ZombieType PickGraveRisingZombieType(int theZombiePoints)
	{
		for (int i = 0; i < aZombieWeightArray.Length; i++)
		{
			aZombieWeightArray[i].Reset();
		}
		int num = 0;
		aZombieWeightArray[num].mItem = ZombieType.Normal;
		aZombieWeightArray[num].mWeight = GetZombiePickWeightForCurrentLevel(ZombieType.Normal);
		num++;
		aZombieWeightArray[num].mItem = ZombieType.TrafficCone;
		aZombieWeightArray[num].mWeight = GetZombiePickWeightForCurrentLevel(ZombieType.TrafficCone);
		num++;
		if (!StageHasGraveStones())
		{
			aZombieWeightArray[num].mItem = ZombieType.Pail;
			aZombieWeightArray[num].mWeight = GetZombiePickWeightForCurrentLevel(ZombieType.Pail);
			num++;
		}
		for (int j = 0; j < num; j++)
		{
			ZombieType zombieType = (ZombieType)aZombieWeightArray[j].mItem;
			ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
			if (mApp.IsFirstTimeAdventureMode() && mLevel < zombieDefinition.mStartingLevel)
			{
				aZombieWeightArray[j].mWeight = 0;
			}
			else if (!mZombieAllowed[(int)zombieType] && zombieType != ZombieType.Normal)
			{
				aZombieWeightArray[j].mWeight = 0;
			}
			else
			{
				aZombieWeightArray[j].mWeight = GetZombiePickWeightForCurrentLevel(zombieType);
			}
		}
		return (ZombieType)TodCommon.TodPickFromWeightedArray(aZombieWeightArray, num);
	}

	public ZombieType PickZombieType(int theZombiePoints, int theWaveIndex, ZombiePicker theZombiePicker)
	{
		int num = 0;
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			aZombieWeightArray[(int)zombieType].Reset();
			ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
			if (!mZombieAllowed[(int)zombieType])
			{
				continue;
			}
			if (zombieType == ZombieType.Bungee && mApp.IsSurvivalEndless(mApp.mGameMode))
			{
				if (!IsFlagWave(theWaveIndex))
				{
					continue;
				}
			}
			else if (mApp.mGameMode != GameMode.ChallengePogoParty && mApp.mGameMode != GameMode.ChallengeBobsledBonanza && mApp.mGameMode != GameMode.ChallengeBobsledBonanza2 && mApp.mGameMode != GameMode.ChallengeAirRaid)
			{
				int num2 = GetZombieFirstAllowedWaveForCurrentLevel(zombieType);
				if (mApp.IsSurvivalEndless(mApp.mGameMode))
				{
					int survivalFlagsCompleted = GetSurvivalFlagsCompleted();
					int num3 = TodCommon.TodAnimateCurve(18, 50, survivalFlagsCompleted, 0, 15, TodCurves.Linear);
					num2 = Math.Max(num2 - num3, 1);
				}
				if (theWaveIndex + 1 < num2 || theZombiePoints < GetZombieValueForCurrentLevel(zombieType))
				{
					continue;
				}
			}
			int zombiePickWeightForCurrentLevel = GetZombiePickWeightForCurrentLevel(zombieType);
			int mWeight = zombiePickWeightForCurrentLevel;
			if (mApp.IsSurvivalMode())
			{
				int survivalFlagsCompleted2 = GetSurvivalFlagsCompleted();
				if (zombieType == ZombieType.Gargantuar || zombieType == ZombieType.Zamboni)
				{
					int num4 = TodCommon.TodAnimateCurve(10, 50, survivalFlagsCompleted2, 2, 50, TodCurves.Linear);
					if (theZombiePicker.mZombieTypeCount[(int)zombieType] >= num4)
					{
						continue;
					}
				}
				if (zombieType == ZombieType.RedeyeGargantuar)
				{
					if (IsFlagWave(theWaveIndex))
					{
						int num5 = TodCommon.TodAnimateCurve(14, 100, survivalFlagsCompleted2, 1, 50, TodCurves.Linear);
						if (theZombiePicker.mZombieTypeCount[(int)zombieType] >= num5)
						{
							continue;
						}
					}
					else
					{
						int num6 = TodCommon.TodAnimateCurve(10, 110, survivalFlagsCompleted2, 1, 50, TodCurves.Linear);
						if (theZombiePicker.mAllWavesZombieTypeCount[(int)zombieType] >= num6)
						{
							continue;
						}
						mWeight = 1000;
					}
				}
				if (zombieType == ZombieType.RobotTitan)
				{
					int num7 = TodCommon.TodAnimateCurve(10, 50, survivalFlagsCompleted2, 2, 50, TodCurves.Linear);
					if (theZombiePicker.mZombieTypeCount[(int)zombieType] >= num7)
					{
						continue;
					}
				}
				if (zombieType == ZombieType.RedeyeRobotTitan)
				{
					if (IsFlagWave(theWaveIndex))
					{
						int num8 = TodCommon.TodAnimateCurve(14, 100, survivalFlagsCompleted2, 1, 50, TodCurves.Linear);
						if (theZombiePicker.mZombieTypeCount[(int)zombieType] >= num8)
						{
							continue;
						}
					}
					else
					{
						int num9 = TodCommon.TodAnimateCurve(10, 110, survivalFlagsCompleted2, 1, 50, TodCurves.Linear);
						if (theZombiePicker.mAllWavesZombieTypeCount[(int)zombieType] >= num9)
						{
							continue;
						}
						mWeight = 1000;
					}
				}
				if (zombieType == ZombieType.Normal)
				{
					mWeight = TodCommon.TodAnimateCurve(10, 50, survivalFlagsCompleted2, zombiePickWeightForCurrentLevel, zombiePickWeightForCurrentLevel / 10, TodCurves.Linear);
				}
				if (zombieType == ZombieType.TrafficCone)
				{
					mWeight = TodCommon.TodAnimateCurve(10, 50, survivalFlagsCompleted2, zombiePickWeightForCurrentLevel, zombiePickWeightForCurrentLevel / 4, TodCurves.Linear);
				}
			}
			if (zombieType == ZombieType.FootballPremium && mApp.mGameMode == GameMode.MarvelousPeople3)
			{
				mWeight = 40000;
			}
			if (zombieType == ZombieType.Talisman && mApp.mGameMode == GameMode.MarvelousPeople4)
			{
				mWeight = 10000;
			}
			if (zombieType == ZombieType.Propeller)
			{
				if (mApp.mGameMode == GameMode.ChallengeBobsledBonanza2)
				{
					mWeight = 6000;
				}
				if (mApp.mGameMode == GameMode.ChallengeMoreAirRaid)
				{
					mWeight = 6000;
				}
			}
			aZombieWeightArray[num].mItem = zombieType;
			aZombieWeightArray[num].mWeight = mWeight;
			num++;
		}
		return (ZombieType)TodCommon.TodPickFromWeightedArray(aZombieWeightArray, num);
	}

	public ZombieType PickZombieTypeSurvival(int theZombiePoints, int theWaveIndex)
	{
		CSSurvivalLevel cSSurvivalLevel = mApp.mCreativeLevel?.GetComponent<CSSurvivalLevel>();
		if (cSSurvivalLevel == null)
		{
			return ZombieType.Normal;
		}
		int num = 0;
		foreach (var (zombieType2, info2) in cSSurvivalLevel.mZombies)
		{
			if (zombieType2 == ZombieType.Invalid || !mZombieAllowed[(int)zombieType2])
			{
				continue;
			}
			ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType2);
			CSOverrideZombieProperty component = mApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
			CSOverrideZombieProperty.Info value = null;
			component?.mProperties.TryGetValue(zombieType2, out value);
			int num2 = info2.mStartingWave ?? value?.mFirstAllowedWave ?? zombieDefinition.mFirstAllowedWave;
			int num3;
			if (cSSurvivalLevel.mEndless && info2.mUseDefault)
			{
				num2 -= TodCommon.TodAnimateCurve(18, 50, GetSurvivalFlagsCompleted(), 0, 15, TodCurves.Linear);
				if (num2 < 1)
				{
					num2 = 1;
				}
				num3 = theWaveIndex;
			}
			else
			{
				num3 = mChallenge.mSurvivalStage * GetNumWavesPerFlag() + theWaveIndex;
			}
			int num4 = info2.mValue ?? value?.mZombieValue ?? zombieDefinition.mZombieValue;
			if (num3 + 1 < num2 || theZombiePoints < num4)
			{
				continue;
			}
			int num5 = info2.mWeight ?? value?.mPickWeight ?? zombieDefinition.mPickWeight;
			if (info2.mWeightModifierPerWave.HasValue)
			{
				float valueOrDefault = info2.mWeightModifierPerWave.GetValueOrDefault();
				int num6 = num3 - 1 - info2.mStartWeightModifyingWave;
				if (num6 < 0)
				{
					num6 = 0;
				}
				for (int i = 0; i < num6; i++)
				{
					if (num3 - 1 <= info2.mStartWeightModifyingWave)
					{
						break;
					}
					num5 += (int)((float)num5 * valueOrDefault);
					if (info2.mWeightCap.HasValue && num5 >= info2.mWeightCap.GetValueOrDefault())
					{
						num5 = info2.mWeightCap.GetValueOrDefault();
						break;
					}
				}
			}
			aZombieWeightArray[num].mItem = zombieType2;
			aZombieWeightArray[num].mWeight = num5;
			num++;
		}
		if (num == 0)
		{
			return ZombieType.Normal;
		}
		return (ZombieType)TodCommon.TodPickFromWeightedArray(aZombieWeightArray, num);
	}

	public int PickRowForNewZombie(ZombieType theZombieType)
	{
		if (theZombieType == ZombieType.Boss)
		{
			return 0;
		}
		GridItem rake = GetRake();
		if (rake != null && rake.mGridItemState == GridItemState.RakeAttracting && RowCanHaveZombieType(rake.mGridY, theZombieType))
		{
			rake.mGridItemState = GridItemState.RakeWaiting;
			TodCommon.TodUpdateSmoothArrayPick(mRowPickingArray, Constants.MAX_GRIDSIZEY, rake.mGridY);
			return rake.mGridY;
		}
		for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
		{
			if (!RowCanHaveZombieType(i, theZombieType))
			{
				mRowPickingArray[i].mWeight = 0f;
				continue;
			}
			if (mApp.mGameMode != GameMode.ChallengePortalCombat)
			{
				CreativeLevel mCreativeLevel = mApp.mCreativeLevel;
				if (mCreativeLevel == null || mCreativeLevel.GetComponent<CSSpawnPortal>()?.mFixSpawnZombieView != true)
				{
					if (mApp.mGameMode == GameMode.ChallengeInvisighoul && mCurrentWave <= 3 && i == 5)
					{
						mRowPickingArray[i].mWeight = 0f;
						continue;
					}
					int num = mCurrentWave - mWaveRowGotLawnMowered[i];
					if (mApp.IsContinuousChallenge() && mCurrentWave == mNumWaves - 1)
					{
						num = 100;
					}
					if (num <= 1)
					{
						mRowPickingArray[i].mWeight = 0.01f;
					}
					else if (num <= 2)
					{
						mRowPickingArray[i].mWeight = 0.5f;
					}
					else
					{
						mRowPickingArray[i].mWeight = 1f;
					}
					continue;
				}
			}
			mRowPickingArray[i].mWeight = mChallenge.PortalCombatRowSpawnWeight(i);
		}
		return TodCommon.TodPickFromSmoothArray(mRowPickingArray, Constants.MAX_GRIDSIZEY);
	}

	public Zombie AddZombie(ZombieType theZombieType, int theFromWave)
	{
		int theRow = PickRowForNewZombie(theZombieType);
		return AddZombieInRow(theZombieType, theRow, theFromWave);
	}

	public Zombie AddZombie(ZombieType theZombieType, int theFromWave, bool theCover)
	{
		int theRow = PickRowForNewZombie(theZombieType);
		return AddZombieInRow(theZombieType, theRow, theFromWave, theCover);
	}

	public void SpawnZombieWave()
	{
		mChallenge.SpawnZombieWave();
		if (mApp.IsBungeeBlitzLevel() || mApp.mCreativeLevel?.GetComponent<CSBungeeBlitz>() != null)
		{
			BungeeDropGrid theBungeeDropGrid = new BungeeDropGrid();
			SetupBungeeDrop(theBungeeDropGrid);
			for (int i = 0; i < 50; i++)
			{
				ZombieType zombieType = mZombiesInWave[mCurrentWave, i];
				switch (zombieType)
				{
				case ZombieType.Zamboni:
				case ZombieType.Bungee:
					AddZombie(zombieType, mCurrentWave, theCover: true);
					continue;
				default:
					BungeeDropZombie(theBungeeDropGrid, zombieType);
					continue;
				case ZombieType.Invalid:
					break;
				}
				break;
			}
		}
		else
		{
			Debug.ASSERT(mCurrentWave >= 0 && mCurrentWave < 100 && mCurrentWave < mNumWaves);
			for (int j = 0; j < 50; j++)
			{
				ZombieType zombieType2 = mZombiesInWave[mCurrentWave, j];
				if (zombieType2 == ZombieType.Invalid)
				{
					break;
				}
				if (zombieType2 == ZombieType.Bobsled && !CanAddBobSled())
				{
					for (int k = 0; k < GameConstants.MAX_ZOMBIE_FOLLOWERS; k++)
					{
						AddZombie(ZombieType.Normal, mCurrentWave, theCover: true);
					}
				}
				else
				{
					AddZombie(zombieType2, mCurrentWave, theCover: true);
				}
			}
		}
		CSMustHaveZombie cSMustHaveZombie = mApp.mCreativeLevel?.GetComponent<CSMustHaveZombie>();
		if (cSMustHaveZombie != null && cSMustHaveZombie.mZombiesInRow.TryGetValue(mCurrentWave, out var value))
		{
			for (int l = 0; l < value.Count; l++)
			{
				AddZombieInRow(value[l].Item1, value[l].Item2, mCurrentWave, theCover: true);
			}
		}
		if (mCurrentWave == mNumWaves - 1 && !mApp.IsContinuousChallenge())
		{
			mRiseFromGraveCounter = 210;
		}
		else if (mApp.mGameMode == GameMode.PoolParty && mCurrentWave % 10 == 9)
		{
			mRiseFromGraveCounter = 210;
		}
		if (IsFlagWave(mCurrentWave))
		{
			mFlagRaiseCounter = 100;
		}
		Debug.Log(DebugType.Info, $"{GetType()} SpawnZombieWave: Zombies in wave {mCurrentWave} {{{string.Join(", ", (from num in Enumerable.Range(0, 50)
			select mZombiesInWave[mCurrentWave, num]).TakeWhile((ZombieType t) => t != ZombieType.Invalid))}}}");
		mCurrentWave++;
		mTotalSpawnedWaves++;
	}

	public void RemoveAllZombies()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying())
			{
				zombie.DieNoLoot(giveAchievements: false);
			}
		}
	}

	public void RemoveCutsceneZombies()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie.mFromWave == GameConstants.ZOMBIE_WAVE_CUTSCENE)
			{
				zombie.DieNoLoot(giveAchievements: false);
			}
		}
	}

	public void SpawnZombiesFromGraves()
	{
		if (mApp.mGameMode == GameMode.ChallengeWarAndPeas || mApp.mGameMode == GameMode.ChallengeWarAndPeas2)
		{
			return;
		}
		if (StageHasRoof())
		{
			SpawnZombiesFromSky();
		}
		else if (StageHasPool())
		{
			SpawnZombiesFromPool();
			return;
		}
		int num = GetGraveStoneCount();
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Gravestone && theGridItem.mGridItemCounter >= 100 && (mApp.mGameMode != GameMode.ChallengeGraveDanger || RandomNumbers.NextNumber(mNumWaves) <= mCurrentWave))
			{
				ZombieType zombieType = PickGraveRisingZombieType(num);
				ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
				Zombie zombie = AddZombie(zombieType, mCurrentWave);
				if (zombie == null)
				{
					break;
				}
				zombie.RiseFromGrave(theGridItem.mGridX, theGridItem.mGridY);
				num -= GetZombieValueForCurrentLevel(zombieType);
				num = Math.Max(1, num);
			}
		}
	}

	public PlantingReason CanPlantAt(int theGridX, int theGridY, SeedType theType, bool aIsMovePlant = false)
	{
		if (theGridX < 0 || theGridX >= Constants.GRIDSIZEX || theGridY < 0 || theGridY >= Constants.MAX_GRIDSIZEY)
		{
			return PlantingReason.NotHere;
		}
		PlantingReason plantingReason = mChallenge.CanPlantAt(theGridX, theGridY, theType);
		if (plantingReason != PlantingReason.Ok || Challenge.IsZombieSeedType(theType))
		{
			return plantingReason;
		}
		PlantsOnLawn thePlantOnLawn = default;
		GetPlantsOnLawn(theGridX, theGridY, ref thePlantOnLawn);
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			if (thePlantOnLawn.mUnderPlant != null || thePlantOnLawn.mNormalPlant != null || thePlantOnLawn.mFlyingPlant != null || thePlantOnLawn.mPumpkinPlant != null)
			{
				return PlantingReason.NotHere;
			}
			if (mApp.mZenGarden.mGardenType == GardenType.Aquarium && !Plant.IsAquatic(theType))
			{
				return PlantingReason.NotOnWater;
			}
			return PlantingReason.Ok;
		}
		bool flag = mSealedCountdown > 0;
		bool flag2 = flag;
		if (flag2)
		{
			bool flag3 = ((theType == SeedType.Jalapeno || theType == SeedType.PickledPepper) ? true : false);
			flag2 = flag3;
		}
		if (flag2)
		{
			return PlantingReason.JalapenoSealed;
		}
		if (GetTalismanAt(theGridX, theGridY) != null)
		{
			return PlantingReason.CelSealed;
		}
		bool flag4 = GetGraveStoneAt(theGridX, theGridY) != null;
		switch (theType)
		{
		case SeedType.Gravebuster:
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return PlantingReason.NotHere;
			}
			if (flag4)
			{
				return PlantingReason.Ok;
			}
			return PlantingReason.OnlyOnGraves;
		case SeedType.InstantCoffee:
			if (thePlantOnLawn.mFlyingPlant != null)
			{
				return PlantingReason.NotHere;
			}
			if (thePlantOnLawn.mNormalPlant == null || !thePlantOnLawn.mNormalPlant.mIsAsleep || thePlantOnLawn.mNormalPlant.mWakeUpCounter > 0 || thePlantOnLawn.mNormalPlant.mOnBungeeState == PlantOnBungeeState.GettingGrabbedByBungee)
			{
				return PlantingReason.NeedsSleeping;
			}
			return PlantingReason.Ok;
		default:
		{
			if (flag4)
			{
				if (Plant.IsFlying(theType))
				{
					return PlantingReason.Ok;
				}
				return PlantingReason.NotOnGrave;
			}
			Plant topPlantAt = GetTopPlantAt(theGridX, theGridY, TopPlant.DiggingOrder);
			if (topPlantAt != null && Plant.GetValidFusion(topPlantAt.mSeedType, theType) != SeedType.None)
			{
				return PlantingReason.Ok;
			}
			bool flag5 = thePlantOnLawn.mUnderPlant != null && thePlantOnLawn.mUnderPlant.mSeedType == SeedType.Lilypad && thePlantOnLawn.mUnderPlant.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee;
			bool flag6 = thePlantOnLawn.mUnderPlant != null && thePlantOnLawn.mUnderPlant.mSeedType == SeedType.Flowerpot && thePlantOnLawn.mUnderPlant.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee;
			bool flag7 = flag6;
			Plant mNormalPlant = thePlantOnLawn.mNormalPlant;
			int num;
			if (mNormalPlant != null && mNormalPlant.mSeedType == SeedType.Endoflame)
			{
				Plant mNormalPlant2 = thePlantOnLawn.mNormalPlant;
				num = ((mNormalPlant2 == null || mNormalPlant2.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			flag6 = (byte)((flag7 ? 1u : 0u) | (uint)num) != 0;
			if (GetCraterAt(theGridX, theGridY) != null)
			{
				if (Plant.IsFlying(theType))
				{
					return PlantingReason.Ok;
				}
				return PlantingReason.NotOnCrater;
			}
			if (GetScaryPotAt(theGridX, theGridY) != null)
			{
				return PlantingReason.NotHere;
			}
			if (IsIceAt(theGridX, theGridY))
			{
				return PlantingReason.NotHere;
			}
			if (mGridSquareType[theGridX, theGridY] == GridSquareType.Dirt || mGridSquareType[theGridX, theGridY] == GridSquareType.None)
			{
				return PlantingReason.NotHere;
			}
			if (theType == SeedType.Lilypad || theType == SeedType.Tanglekelp || theType == SeedType.Seashroom || (aIsMovePlant && (theType == SeedType.Cattail || theType == SeedType.HypnoCattail)))
			{
				if (!IsPoolSquare(theGridX, theGridY) && mGridSquareType[theGridX, theGridY] != GridSquareType.Shallow)
				{
					return PlantingReason.OnlyInPool;
				}
				if (thePlantOnLawn.mNormalPlant != null || thePlantOnLawn.mUnderPlant != null)
				{
					return PlantingReason.NotHere;
				}
				return PlantingReason.Ok;
			}
			if (Plant.IsFlying(theType))
			{
				if (thePlantOnLawn.mFlyingPlant != null)
				{
					return PlantingReason.NotHere;
				}
				return PlantingReason.Ok;
			}
			if ((theType == SeedType.Spikeweed || theType == SeedType.Spikerock) && (mGridSquareType[theGridX, theGridY] == GridSquareType.Pool || StageHasRoof() || thePlantOnLawn.mUnderPlant != null))
			{
				return PlantingReason.NeedsGround;
			}
			if (mGridSquareType[theGridX, theGridY] == GridSquareType.Pool && !flag5 && theType != SeedType.Cattail && (thePlantOnLawn.mNormalPlant == null || thePlantOnLawn.mNormalPlant.mSeedType != SeedType.Cattail || theType != SeedType.Pumpkinshell))
			{
				return PlantingReason.NotOnWater;
			}
			if (theType == SeedType.Flowerpot)
			{
				if (thePlantOnLawn.mNormalPlant != null || thePlantOnLawn.mUnderPlant != null || thePlantOnLawn.mPumpkinPlant != null)
				{
					return PlantingReason.NotHere;
				}
				return PlantingReason.Ok;
			}
			if (StageHasRoof() && !flag6 && theType != SeedType.Endoflame)
			{
				return PlantingReason.NeedsPot;
			}
			if (theType == SeedType.Pumpkinshell)
			{
				if (thePlantOnLawn.mNormalPlant != null && thePlantOnLawn.mNormalPlant.mSeedType == SeedType.Cobcannon)
				{
					return PlantingReason.NotHere;
				}
				if (thePlantOnLawn.mPumpkinPlant == null)
				{
					return PlantingReason.Ok;
				}
				if (!aIsMovePlant && mApp.mPlayerInfo.mPurchases[29] != 0 && thePlantOnLawn.mPumpkinPlant.mPlantHealth < thePlantOnLawn.mPumpkinPlant.mPlantMaxHealth * 2 / 3 && theType == thePlantOnLawn.mPumpkinPlant.mSeedType && thePlantOnLawn.mPumpkinPlant.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee)
				{
					return PlantingReason.Ok;
				}
				return PlantingReason.NotHere;
			}
			if (flag5 && (theType == SeedType.Potatomine || theType == SeedType.Endoflame))
			{
				return PlantingReason.OnlyOnGround;
			}
			if (!aIsMovePlant && thePlantOnLawn.mUnderPlant != null && theType == SeedType.Cattail)
			{
				if (thePlantOnLawn.mNormalPlant != null)
				{
					return PlantingReason.NotHere;
				}
				if (thePlantOnLawn.mUnderPlant.IsUpgradableTo(theType) && thePlantOnLawn.mUnderPlant.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee)
				{
					return PlantingReason.Ok;
				}
				if (Plant.IsUpgrade(theType))
				{
					return PlantingReason.NeedsUpgrade;
				}
			}
			if (thePlantOnLawn.mUnderPlant != null && (thePlantOnLawn.mUnderPlant.mSeedType == SeedType.Imitater || thePlantOnLawn.mUnderPlant.mSeedType == SeedType.ImitaterRandomPlant))
			{
				return PlantingReason.NotHere;
			}
			if (thePlantOnLawn.mNormalPlant == null)
			{
				if (!mApp.mEasyPlantingCheat && (!mApp.IsLevelUseJson() || !mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mEasyUpgrade) && Plant.IsUpgrade(theType) && !aIsMovePlant)
				{
					return PlantingReason.NeedsUpgrade;
				}
				if (!aIsMovePlant && theType == SeedType.Cobcannon && !IsValidCobCannonSpot(theGridX, theGridY))
				{
					return PlantingReason.NeedsUpgrade;
				}
				if (aIsMovePlant && theType == SeedType.Cobcannon && !IsValidCobCannonSpotMovePlant(theGridX, theGridY))
				{
					return PlantingReason.NotHere;
				}
				if (theType == SeedType.Cattail && !IsPoolSquare(theGridX, theGridY))
				{
					return PlantingReason.NotHere;
				}
				return PlantingReason.Ok;
			}
			if (!aIsMovePlant && thePlantOnLawn.mNormalPlant.IsUpgradableTo(theType) && thePlantOnLawn.mNormalPlant.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee)
			{
				return PlantingReason.Ok;
			}
			if (Plant.IsUpgrade(theType) && !aIsMovePlant)
			{
				return PlantingReason.NeedsUpgrade;
			}
			if (!aIsMovePlant && (((theType == SeedType.Wallnut || theType == SeedType.Tallnut || theType == SeedType.FireShroom) && mApp.mPlayerInfo.mPurchases[29] != 0) || (theType == SeedType.Garlic && mApp.mPlayerInfo.mPurchases[30] != 0)) && thePlantOnLawn.mNormalPlant.mPlantHealth < thePlantOnLawn.mNormalPlant.mPlantMaxHealth * 2 / 3 && theType == thePlantOnLawn.mNormalPlant.mSeedType && thePlantOnLawn.mNormalPlant.mOnBungeeState != PlantOnBungeeState.GettingGrabbedByBungee)
			{
				return PlantingReason.Ok;
			}
			return PlantingReason.NotHere;
		}
		}
	}

	public override void MouseMove(int x, int y)
	{
		MouseDrag(x, y);
		mChallenge.MouseMove(x, y);
	}

	public override void MouseDrag(int x, int y)
	{
		base.MouseDrag(x, y);
		MouseDragInternal(x, y, isTouch: false);
	}

	private void MouseDragInternal(int x, int y, bool isTouch)
	{
		int num;
		int screenX = (num = x);
		int num2;
		int screenY = (num2 = y);
		if (mCameraEnabled)
		{
			mCamera.ScreenToBoardReplace(ref screenX, ref screenY);
		}
		if (mCameraEnabled)
		{
			int num3 = screenX;
			y = screenY;
			x = num3;
		}
		if (isTouch && mIgnoreMouseUp && mCursorObject.mCursorType == CursorType.PlantFromUsableCoin)
		{
			mIgnoreMouseUp = false;
		}
		mLastToolX = x;
		mLastToolY = y;
		if (isTouch && mIgnoreNextMouseUp && new TRect(Constants.ZEN_XMIN, Constants.ZEN_YMIN, Constants.ZEN_XMAX - Constants.ZEN_XMIN, Constants.ZEN_YMAX - Constants.ZEN_YMIN).Contains(mApp.mBoard.mLastToolX, mApp.mBoard.mLastToolY))
		{
			mIgnoreNextMouseUp = false;
		}
		mChallenge.MouseMove(x, y);
	}

	public override void TouchMoved(_Touch touch)
	{
		int num = (int)touch.location.X;
		int num2 = (int)touch.location.y;
		int num3 = 1;
		int y = num2;
		int x = num;
		MouseDragInternal(x, y, isTouch: true);
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		base.MouseDown(x, y, theClickCount);
		MouseDownInternal(x, y, theClickCount, isTouch: false);
	}

	public void MouseDownInternal(int x, int y, int theClickCount, bool isTouch)
	{
		if (HandleGridFocusClick(x, y, theClickCount))
		{
			mIgnoreMouseUp = true;
			return;
		}
		int num;
		int screenX = (num = x);
		int num2;
		int screenY = (num2 = y);
		if (mCameraEnabled)
		{
			mCamera.ScreenToBoardReplace(ref screenX, ref screenY);
		}
		if (mCameraEnabled)
		{
			int num3 = screenX;
			y = screenY;
			x = num3;
		}
		mLastToolX = x;
		mLastToolY = y;
		if (mCameraEnabled)
		{
			y = num2;
			x = num;
		}
		mMenuButton.Update();
		if (mStoreButton != null)
		{
			mStoreButton.Update();
		}
		if (mAccelerateButton != null)
		{
			mAccelerateButton.Update();
		}
		if (mPauseButton != null)
		{
			mPauseButton.mChecked = mManualPaused;
			mPauseButton.Update();
		}
		mCornRouletteSelfButton?.Update();
		mCornRouletteOtherButton?.Update();
		mIgnoreMouseUp = !CanInteractWithBoardButtons();
		if (mTimeStopCounter > 0)
		{
			return;
		}
		MouseHitTest(x, y, out var theHitResult, posScaled: false, isTouch);
		if (mCameraEnabled)
		{
			int num4 = screenX;
			y = screenY;
			x = num4;
		}
		if (mChallenge.MouseDown(x, y, theClickCount, theHitResult))
		{
			return;
		}
		if (mCameraEnabled)
		{
			y = num2;
			x = num;
		}
		if (mMenuButton.IsMouseOver() && CanInteractWithBoardButtons() && theClickCount > 0)
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
		else if (mStoreButton != null && mStoreButton.IsMouseOver() && CanInteractWithBoardButtons() && theClickCount > 0)
		{
			if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom)
			{
				mApp.PlaySample(Resources.SOUND_TAP);
			}
			else if (mApp.mGameMode == GameMode.ChallengeLastStand || mApp.mGameMode == GameMode.Upsell)
			{
				mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
			}
		}
		else if (mCornRouletteSelfButton != null && CanInteractWithBoardButtons() && theClickCount > 0 && (mCornRouletteSelfButton.IsMouseOver() || mCornRouletteOtherButton.IsMouseOver()))
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
		else if (mAccelerateButton != null && mAccelerateButton.IsMouseOver() && CanInteractWithBoardButtons() && (theClickCount > 0 || theClickCount < 0))
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
		else if (mPauseButton != null && mPauseButton.IsMouseOver() && CanInteractWithBoardButtons() && theClickCount > 0)
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
		if (mCameraEnabled)
		{
			int num5 = screenX;
			y = screenY;
			x = num5;
		}
		if (mApp.mGameScene == GameScenes.LevelIntro && mApp.mSeedChooserScreen != null)
		{
			mApp.mSeedChooserScreen.CancelLawnView();
		}
		if (mApp.mGameScene == GameScenes.ZombiesWon)
		{
			mCutScene.ZombieWonClick();
			return;
		}
		if (mApp.mGameScene == GameScenes.LevelIntro)
		{
			mCutScene.MouseDown(x, y);
		}
		if (mApp.mTodCheatKeys && !mApp.IsScaryPotterLevel() && mNextSurvivalStageCounter > 0)
		{
			mNextSurvivalStageCounter = 2;
			for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
			{
				if (mIceTimer[i] > mNextSurvivalStageCounter)
				{
					mIceTimer[i] = mNextSurvivalStageCounter;
				}
			}
		}
		if (!isTouch && theHitResult.mObjectType == GameObjectType.None)
		{
			if ((float)mLastToolX >= (float)Constants.LAWN_XMIN * Constants.S && (float)mLastToolY >= (float)Constants.LAWN_YMIN * Constants.S && mCursorObject.mCursorType == CursorType.CobcannonTarget)
			{
				MouseDownCobcannonFire(mLastToolX, mLastToolY, theClickCount);
				mLastToolX = (mLastToolY = -32768);
				return;
			}
		}
		else if (theHitResult.mObjectType == GameObjectType.Coin && theClickCount >= 0 && (isTouch || mCursorObject.mCursorType == CursorType.Hammer || mCursorObject.mCursorType == CursorType.Normal))
		{
			Coin coin = (Coin)theHitResult.mObject;
			coin.MouseDown(x, y, theClickCount);
			if (isTouch)
			{
				mIgnoreMouseUp = true;
			}
			UpdateCursor();
			return;
		}
		if ((!isTouch && mCursorObject.mCursorType == CursorType.Shovel) || mCursorObject.mCursorType == CursorType.WateringCan || mCursorObject.mCursorType == CursorType.Fertilizer || mCursorObject.mCursorType == CursorType.BugSpray || mCursorObject.mCursorType == CursorType.Phonograph || mCursorObject.mCursorType == CursorType.Chocolate || mCursorObject.mCursorType == CursorType.Glove || mCursorObject.mCursorType == CursorType.MoneySign || mCursorObject.mCursorType == CursorType.Wheeelbarrow || mCursorObject.mCursorType == CursorType.TreeFood)
		{
			MouseDownWithTool(x, y, theClickCount, mCursorObject.mCursorType, posScaled: false, isTouch);
			return;
		}
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			mChallenge.ZombiquariumMouseDown(x, y);
		}
		if (mCameraEnabled)
		{
			y = num2;
			x = num;
		}
		if (theHitResult.mObjectType == GameObjectType.Seedpacket)
		{
			SeedPacket seedPacket = (SeedPacket)theHitResult.mObject;
			int mSeedBankIndex = mCursorObject.mSeedBankIndex;
			RefreshSeedPacketFromCursor();
			if (mSeedBankIndex == seedPacket.mIndex)
			{
				theHitResult.mObjectType = GameObjectType.None;
				return;
			}
			RefreshSeedPacketFromCursor();
			seedPacket.MouseDown(x, y, theClickCount);
			return;
		}
		if (mCameraEnabled)
		{
			int num6 = screenX;
			y = screenY;
			x = num6;
		}
		if (IsPlantInCursor())
		{
			if (isTouch)
			{
				MouseDownWithPlant(x, y, theClickCount);
			}
			else
			{
				MouseUpWithPlant(x, y, theClickCount);
			}
			return;
		}
		if (mCameraEnabled)
		{
			y = num2;
			x = num;
		}
		if (theHitResult.mObjectType == GameObjectType.NextGarden)
		{
			if (mApp.mGameMode == GameMode.ChallengeZenGarden)
			{
				mApp.mZenGarden.GotoNextGarden();
			}
			else if (mApp.mGameMode == GameMode.TreeOfWisdom)
			{
				mChallenge.TreeOfWisdomNextGarden();
			}
			mApp.PlaySample(Resources.SOUND_TAP);
			return;
		}
		if (theHitResult.mObjectType == GameObjectType.PrevGarden)
		{
			if (mApp.mGameMode == GameMode.ChallengeZenGarden)
			{
				mApp.mZenGarden.GotoPrevGarden();
			}
			else if (mApp.mGameMode == GameMode.TreeOfWisdom)
			{
				mChallenge.TreeOfWisdomPrevGarden();
			}
			mApp.PlaySample(Resources.SOUND_TAP);
			return;
		}
		if (theHitResult.mObjectType == GameObjectType.Shovel || theHitResult.mObjectType == GameObjectType.WateringCan || theHitResult.mObjectType == GameObjectType.Fertilizer || theHitResult.mObjectType == GameObjectType.BugSpray || theHitResult.mObjectType == GameObjectType.Phonograph || theHitResult.mObjectType == GameObjectType.Chocolate || theHitResult.mObjectType == GameObjectType.Glove || theHitResult.mObjectType == GameObjectType.MoneySign || theHitResult.mObjectType == GameObjectType.Wheelbarrow || theHitResult.mObjectType == GameObjectType.TreeFood)
		{
			if (mCursorObject.mCursorType != CursorType.Shovel)
			{
				RefreshSeedPacketFromCursor();
			}
			if (theHitResult.mObjectType == GameObjectType.Glove && HasGlove() && mChallenge.mGloveCounter > 0)
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
				return;
			}
			PickUpTool(theHitResult.mObjectType);
			if (isTouch)
			{
				mIgnoreMouseUp = true;
			}
			return;
		}
		if (mCameraEnabled)
		{
			int num7 = screenX;
			y = screenY;
			x = num7;
		}
		if (theHitResult.mObjectType == GameObjectType.Plant)
		{
			Plant plant = (Plant)theHitResult.mObject;
			plant.MouseDown(x, y, theClickCount);
		}
		else if (mChallenge.mChallengeState == ChallengeState.ZenPlantInteracting)
		{
			mApp.mZenGarden.LeavePlantInteraction(mApp.mZenGarden.mInteracting);
		}
		UpdateCursor();
	}

	internal void SelectGridFocus()
	{
		RefreshSeedPacketFromCursor();
		mCursorObject.mMagnifier = true;
		mCursorObject.Update();
	}

	private bool HandleGridFocusClick(int x, int y, int theClickCount)
	{
		if (!IsGridFocusActive && !mCursorObject.mMagnifier)
		{
			return false;
		}
		if (theClickCount == 0)
		{
			return true;
		}
		if (IsGridFocusActive || theClickCount < 0)
		{
			mCursorObject.mMagnifier = false;
			LeaveGridFocus();
			return true;
		}
		if (!CanSelectGridFocus)
		{
			return true;
		}
		mCamera.ScreenToBoardReplace(ref x, ref y);
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		int num = PixelToGridX(x, y);
		int board_GridCellSizeX = Constants.New.Board_GridCellSizeX;
		if (num < 0 || x >= GridToPixelX(num, 0) + board_GridCellSizeX)
		{
			return true;
		}
		int num2 = (StageHas6Rows() ? 6 : 5);
		for (int i = 0; i < num2; i++)
		{
			TPoint[] celPosition = GetCelPosition(num, i);
			if (y >= celPosition[0].mY && y < celPosition[2].mY)
			{
				mCursorObject.mMagnifier = false;
				mGridFocusReturn = (mCamera.X, mCamera.Y, mCamera.Zoom);
				float focusWorldX = ((float)celPosition[0].mX + (float)board_GridCellSizeX / 2f) * Constants.S + (float)Constants.Board_Offset_AspectRatio_Correction;
				float focusWorldY = (float)(celPosition[0].mY + celPosition[2].mY) / 2f * Constants.S;
				mCamera.FocusWorldSmooth(focusWorldX, focusWorldY, 3f * Constants.BoardCameraScaleMultiplier, 150, TodCurves.EaseInOut);
				break;
			}
		}
		return true;
	}

	private void LeaveGridFocus()
	{
		if (!mGridFocusReturning)
		{
			(float, float, float)? tuple = mGridFocusReturn;
			if (tuple.HasValue)
			{
				(float, float, float) valueOrDefault = tuple.GetValueOrDefault();
				mGridFocusReturning = true;
				mCamera.SetPositionSmooth(valueOrDefault.Item1, valueOrDefault.Item2, valueOrDefault.Item3, 150, TodCurves.EaseInOut);
			}
		}
	}

	public override void TouchBegan(_Touch touch)
	{
		int num = (int)touch.location.X;
		int num2 = (int)touch.location.y;
		int theClickCount = 1;
		int y = num2;
		int x = num;
		MouseDownInternal(x, y, theClickCount, isTouch: true);
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		base.MouseUp(x, y, theClickCount);
		MouseUpInternal(x, y, theClickCount, isTouch: false);
	}

	public void MouseUpInternal(int x, int y, int theClickCount, bool isTouch)
	{
		if (IsGridFocusActive || mCursorObject.mMagnifier)
		{
			mIgnoreMouseUp = false;
			return;
		}
		int num;
		int screenX = (num = x);
		int num2;
		int screenY = (num2 = y);
		if (mCameraEnabled)
		{
			mCamera.ScreenToBoardReplace(ref screenX, ref screenY);
		}
		if (mIgnoreMouseUp)
		{
			if (!isTouch)
			{
				mIgnoreMouseUp = false;
				return;
			}
			mLastToolX = (mLastToolY = -32768);
			mLastToolX = 0;
			return;
		}
		if (isTouch)
		{
			if (mIgnoreNextMouseUp)
			{
				mIgnoreNextMouseUp = false;
				return;
			}
			if (mIgnoreNextMouseUpSeedPacket)
			{
				MouseHitTest(x, y, out var theHitResult, posScaled: false, isTouch);
				if (theHitResult.mObjectType == GameObjectType.Coin)
				{
					Coin coin = (Coin)theHitResult.mObject;
					if (coin.mType == CoinType.UsableSeedPacket)
					{
						mLastToolX = (mLastToolY = -32768);
						return;
					}
				}
				mIgnoreNextMouseUpSeedPacket = false;
			}
		}
		if (mMenuButton.IsMouseOver() && CanInteractWithBoardButtons() && theClickCount > 0)
		{
			RefreshSeedPacketFromCursor();
			if (mApp.GetDialog(Dialogs.DIALOG_GAME_OVER) == null && mApp.GetDialog(Dialogs.DIALOG_LEVEL_COMPLETE) == null)
			{
				mMenuButton.mIsOver = false;
				mMenuButton.mIsDown = false;
				UpdateCursor();
				if (!isTouch)
				{
					ClearCursor();
				}
				if (mTutorialState == TutorialState.ZenGardenCompleted)
				{
					mApp.FinishZenGardenTutorial();
				}
				else if (mTutorialState == TutorialState.ZenGardenCompleted)
				{
					mApp.mBoardResult = BoardResult.Won;
					mApp.KillBoard();
					mApp.PreNewGame(GameMode.Adventure, theLookForSavedGame: false);
				}
				else if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom || mApp.mGameMode == GameMode.Upsell)
				{
					mApp.mPlayerInfo.mNeedsTrialLevelReset = true;
					mApp.mBoardResult = BoardResult.Quit;
					mApp.DoBackToMain();
				}
				else
				{
					mApp.PlaySample(Resources.SOUND_PAUSE);
					mApp.DoNewOptions(theFromGameSelector: false);
				}
			}
			return;
		}
		if (isTouch)
		{
			if (TrashcanHitTest(mLastToolX, mLastToolY) || (((float)mLastToolX >= (float)Constants.LAWN_XMIN * Constants.S || mApp.mGameMode == GameMode.ChallengeZenGarden) && IsPlantInCursor()))
			{
				MouseUpWithPlant(mLastToolX, mLastToolY, theClickCount);
				return;
			}
			if (((float)mLastToolX < (float)Constants.LAWN_XMIN * Constants.S || (float)mLastToolY >= (float)Constants.LAWN_YMIN * Constants.S) && mCursorObject.mCursorType == CursorType.Shovel)
			{
				MouseDownWithTool(mLastToolX, mLastToolY, theClickCount, mCursorObject.mCursorType, posScaled: false, isTouch);
				return;
			}
			if ((float)mLastToolX >= (float)Constants.LAWN_XMIN * Constants.S && (float)mLastToolY >= (float)Constants.LAWN_YMIN * Constants.S && mCursorObject.mCursorType == CursorType.CobcannonTarget)
			{
				MouseDownCobcannonFire(mLastToolX, mLastToolY, theClickCount);
				mLastToolX = (mLastToolY = -32768);
				return;
			}
			if ((mCursorObject.mCursorType == CursorType.Fertilizer || mCursorObject.mCursorType == CursorType.BugSpray || mCursorObject.mCursorType == CursorType.Phonograph || mCursorObject.mCursorType == CursorType.Chocolate || mCursorObject.mCursorType == CursorType.Glove || mCursorObject.mCursorType == CursorType.MoneySign || mCursorObject.mCursorType == CursorType.Wheeelbarrow || mCursorObject.mCursorType == CursorType.TreeFood || mCursorObject.mCursorType == CursorType.WateringCan) && (float)mLastToolY >= (float)Constants.ZEN_YMIN * Constants.S)
			{
				MouseDownWithTool(x, y, theClickCount, mCursorObject.mCursorType, posScaled: false, isTouch);
				return;
			}
		}
		if (mChallenge.MouseUp(mLastToolX, mLastToolY) && theClickCount > 0)
		{
			return;
		}
		if (mStoreButton != null && mStoreButton.IsMouseOver() && CanInteractWithBoardButtons() && theClickCount > 0)
		{
			if (mApp.mGameMode == GameMode.ChallengeZenGarden)
			{
				ClearAdviceImmediately();
				mApp.mZenGarden.OpenStore();
			}
			else if (mApp.mGameMode == GameMode.TreeOfWisdom)
			{
				mChallenge.TreeOfWisdomOpenStore();
			}
			if (mApp.mGameMode == GameMode.ChallengeLastStand)
			{
				mChallenge.mChallengeState = ChallengeState.LastStandOnslaught;
				mStoreButton.mBtnNoDraw = true;
				mStoreButton.mDisabled = true;
				mZombieCountDown = 9;
				mZombieCountDownStart = mZombieCountDown;
			}
			else if (mApp.mGameMode == GameMode.Upsell)
			{
				mApp.BuyGame();
				mApp.DoBackToMain();
			}
		}
		if (mCornRouletteSelfButton != null && CanInteractWithBoardButtons() && theClickCount > 0 && (mCornRouletteSelfButton.IsMouseOver() || mCornRouletteOtherButton.IsMouseOver()))
		{
			mChallenge.CornRouletteMakeChoice(mCornRouletteSelfButton.IsMouseOver());
			return;
		}
		if (mAccelerateButton != null && mAccelerateButton.IsMouseOver() && CanInteractWithBoardButtons())
		{
			if (theClickCount >= 1)
			{
				AccelerationIncrease();
			}
			if (theClickCount <= -1)
			{
				AccelerationDecrease();
			}
		}
		if (mPauseButton != null && mPauseButton.IsMouseOver() && CanInteractWithBoardButtons())
		{
			mManualPaused = !mManualPaused;
		}
		if (mAgaveSkillDrawType != 0 && CanCancelAgaveSkill() && GetAgaveSkillRect().Contains(mLastToolX, mLastToolY) && theClickCount > 0)
		{
			ClearCursor();
		}
		if (mEndoflameSkillDrawType != 0 && CanCancelEndoflameSkill() && GetEndoflameSkillRect().Contains(mLastToolX, mLastToolY) && theClickCount > 0)
		{
			ClearCursor();
		}
		if (isTouch)
		{
			mLastToolX = (mLastToolY = -32768);
		}
	}

	public void AccelerationIncrease()
	{
		mAccelerationNumerator = mAccelerationNumerator % 3 + 1;
		mAccelerationDenominator = 1;
		mAccelerationFrameIndex = 0;
	}

	public void AccelerationDecrease()
	{
		mAccelerationNumerator = (mAccelerationNumerator - 1 + 3 - 1) % 3 + 1;
		mAccelerationDenominator = 1;
		mAccelerationFrameIndex = 0;
	}

	public override void TouchEnded(_Touch touch)
	{
		int num = (int)touch.location.X;
		int num2 = (int)touch.location.y;
		int theClickCount = 1;
		int y = num2;
		int x = num;
		MouseUpInternal(x, y, theClickCount, isTouch: true);
	}

	public override void KeyChar(SexyChar theChar)
	{
		if (IsGridFocusActive)
		{
			return;
		}
		if (!mApp.mTodCheatKeys && !mIsCheatDown)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			switch (theChar)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			{
				int num3 = ((theChar == 48u) ? 9 : ((char)theChar - 49));
				if (mSeedBank.mSeedPackets[num3].CenterY() > Constants.BOARD_HEIGHT || !mSeedBank.IsSeedPacketAccessible(mSeedBank.mSeedPackets[num3]))
				{
					return;
				}
				bool flag2 = mCursorObject.mSeedBankIndex != num3;
				RefreshSeedPacketFromCursor();
				if (flag2)
				{
					mSeedBank.mSeedPackets[num3].MouseDown(0, 0, 1);
				}
				break;
			}
			case '`':
				if (CanUseCombatTool(GameObjectType.Shovel))
				{
					RefreshSeedPacketFromCursor();
					PickUpTool(GameObjectType.Shovel);
				}
				break;
			case ' ':
				if (mApp.CanPauseNow())
				{
					mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
					mManualPaused = !mManualPaused;
				}
				break;
			case '\t':
				if (mAccelerateButton != null)
				{
					mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
					AccelerationIncrease();
				}
				break;
			case 'q':
				flag = true;
				num = 2;
				num2 = 1;
				break;
			case 'w':
				flag = true;
				num = 2;
				num2 = 2;
				break;
			case 'e':
				flag = true;
				num = 2;
				num2 = 3;
				break;
			case 'r':
				flag = true;
				num = 2;
				num2 = 4;
				break;
			case 't':
				flag = true;
				num = 2;
				num2 = 5;
				break;
			case 'y':
				flag = true;
				num = 2;
				num2 = 6;
				break;
			case 'a':
				flag = true;
				num = 4;
				num2 = 1;
				break;
			case 's':
				flag = true;
				num = 4;
				num2 = 2;
				break;
			case 'd':
				flag = true;
				num = 4;
				num2 = 3;
				break;
			case 'f':
				flag = true;
				num = 4;
				num2 = 4;
				break;
			case 'g':
				flag = true;
				num = 4;
				num2 = 5;
				break;
			case 'h':
				flag = true;
				num = 4;
				num2 = 6;
				break;
			case 'z':
				flag = true;
				num = 6;
				num2 = 1;
				break;
			case 'x':
				flag = true;
				num = 6;
				num2 = 2;
				break;
			case 'c':
				flag = true;
				num = 6;
				num2 = 3;
				break;
			case 'v':
				flag = true;
				num = 6;
				num2 = 4;
				break;
			case 'b':
				flag = true;
				num = 6;
				num2 = 5;
				break;
			case 'n':
				flag = true;
				num = 6;
				num2 = 6;
				break;
			}
			if (!flag)
			{
				return;
			}
			int num4 = GridToPixelX(num - 1, num2 - 1);
			int num5 = GridToPixelY(num - 1, num2 - 1);
			int num6 = num4;
			MouseHitTest((int)((float)num6 * Constants.S), (int)((float)num5 * Constants.S), out var theHitResult, posScaled: false);
			if (theHitResult.mObjectType == GameObjectType.Plant)
			{
				Plant plant = (Plant)theHitResult.mObject;
				if (plant.mState == PlantState.CobcannonReady)
				{
					plant.MouseDown((int)((float)num6 * Constants.S), (int)((float)num5 * Constants.S), 1);
					plant.mEatenFlashCountdown += 80;
					plant.mBeghouledFlashCountdown = 100;
				}
			}
			return;
		}
		char value_type = theChar.value_type;
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			if (value_type == 'm')
			{
				if (!mApp.mZenGarden.IsZenGardenFull(theIncludeDroppedPresents: true))
				{
					PottedPlant pottedPlant = new PottedPlant();
					pottedPlant.InitializePottedPlant(SeedType.Marigold);
					pottedPlant.mPlantAge = PottedPlantAge.Full;
					pottedPlant.mDrawVariation = (DrawVariation)TodCommon.RandRangeInt(2, 12);
					mApp.mZenGarden.AddPottedPlant(pottedPlant);
				}
				return;
			}
			if (value_type == '+')
			{
				while (!mApp.mZenGarden.IsZenGardenFull(theIncludeDroppedPresents: true))
				{
					SeedType theSeedType = mApp.mZenGarden.PickRandomSeedType();
					PottedPlant pottedPlant2 = new PottedPlant();
					pottedPlant2.InitializePottedPlant(theSeedType);
					pottedPlant2.mPlantAge = PottedPlantAge.Full;
					mApp.mZenGarden.AddPottedPlant(pottedPlant2);
				}
				return;
			}
			switch (value_type)
			{
			case '-':
				mPlants.Clear();
				mApp.mPlayerInfo.mNumPottedPlants = 0;
				return;
			case 'a':
				if (!mApp.mZenGarden.IsZenGardenFull(theIncludeDroppedPresents: true))
				{
					SeedType theSeedType2 = mApp.mZenGarden.PickRandomSeedType();
					PottedPlant pottedPlant3 = new PottedPlant();
					pottedPlant3.InitializePottedPlant(theSeedType2);
					pottedPlant3.mPlantAge = PottedPlantAge.Small;
					mApp.mZenGarden.AddPottedPlant(pottedPlant3);
				}
				return;
			case 'f':
			{
				int index = -1;
				Plant thePlant = null;
				while (IteratePlants(ref thePlant, ref index))
				{
					if (GetZenToolAt(thePlant.mPlantCol, thePlant.mRow) != null || thePlant.mPottedPlantIndex < 0)
					{
						continue;
					}
					PottedPlant thePottedPlant = mApp.mZenGarden.PottedPlantFromIndex(thePlant.mPottedPlantIndex);
					switch (mApp.mZenGarden.GetPlantsNeed(thePottedPlant))
					{
					case PottedPlantNeed.Water:
						thePlant.mHighlighted = true;
						mApp.mZenGarden.MouseDownWithFeedingTool(thePlant.mX, thePlant.mY, CursorType.WateringCan);
						return;
					case PottedPlantNeed.Fertilizer:
						thePlant.mHighlighted = true;
						if (mApp.mPlayerInfo.mPurchases[14] <= 1000)
						{
							mApp.mPlayerInfo.mPurchases[14] = 1001;
						}
						mApp.mZenGarden.MouseDownWithFeedingTool(thePlant.mX, thePlant.mY, CursorType.Fertilizer);
						return;
					case PottedPlantNeed.Bugspray:
						thePlant.mHighlighted = true;
						if (mApp.mPlayerInfo.mPurchases[15] <= 1000)
						{
							mApp.mPlayerInfo.mPurchases[15] = 1001;
						}
						mApp.mZenGarden.MouseDownWithFeedingTool(thePlant.mX, thePlant.mY, CursorType.BugSpray);
						return;
					case PottedPlantNeed.Phonograph:
						thePlant.mHighlighted = true;
						mApp.mZenGarden.MouseDownWithFeedingTool(thePlant.mX, thePlant.mY, CursorType.Phonograph);
						return;
					}
				}
				return;
			}
			case 'r':
			{
				int index2 = -1;
				Plant thePlant2 = null;
				while (IteratePlants(ref thePlant2, ref index2))
				{
					if (thePlant2.mPottedPlantIndex >= 0)
					{
						Debug.ASSERT(thePlant2.mPottedPlantIndex < mApp.mPlayerInfo.mNumPottedPlants);
						PottedPlant thePottedPlant2 = mApp.mPlayerInfo.mPottedPlant[thePlant2.mPottedPlantIndex];
						mApp.mZenGarden.ResetPlantTimers(thePottedPlant2);
					}
				}
				return;
			}
			case 's':
				if (mApp.mZenGarden.IsStinkySleeping())
				{
					mApp.mZenGarden.WakeStinky();
				}
				else
				{
					mApp.mZenGarden.ResetStinkyTimers();
				}
				return;
			case 'c':
				if (mApp.mPlayerInfo.mPurchases[26] < 1000)
				{
					mApp.mPlayerInfo.mPurchases[26] = 1001;
				}
				else
				{
					mApp.mPlayerInfo.mPurchases[26]++;
				}
				return;
			case ']':
			{
				PottedPlant pottedPlantInWheelbarrow = mApp.mZenGarden.GetPottedPlantInWheelbarrow();
				if (pottedPlantInWheelbarrow != null)
				{
					pottedPlantInWheelbarrow.mSeedType++;
					if (pottedPlantInWheelbarrow.mSeedType == SeedType.Gatlingpea)
					{
						pottedPlantInWheelbarrow.mSeedType = SeedType.Peashooter;
					}
					if (pottedPlantInWheelbarrow.mSeedType == SeedType.Flowerpot)
					{
						pottedPlantInWheelbarrow.mSeedType++;
					}
				}
				return;
			}
			}
		}
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			switch (value_type)
			{
			case 'f':
				if (mApp.mPlayerInfo.mPurchases[28] <= 1000)
				{
					mApp.mPlayerInfo.mPurchases[28] = 1001;
				}
				mChallenge.TreeOfWisdomFertilize();
				break;
			case 'g':
				mChallenge.TreeOfWisdomGrow();
				break;
			case 'b':
				mChallenge.mChallengeStateCounter = 1;
				break;
			case '0':
			{
				int currentChallengeIndex9 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex9] = 0;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '1':
			{
				int currentChallengeIndex8 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex8] = 9;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '2':
			{
				int currentChallengeIndex7 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex7] = 19;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '3':
			{
				int currentChallengeIndex6 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex6] = 29;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '4':
			{
				int currentChallengeIndex5 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex5] = 39;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '5':
			{
				int currentChallengeIndex4 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex4] = 49;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '6':
			{
				int currentChallengeIndex3 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex3] = 98;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '7':
			{
				int currentChallengeIndex2 = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex2] = 498;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			case '8':
			{
				int currentChallengeIndex = mApp.GetCurrentChallengeIndex();
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex] = 998;
				mChallenge.TreeOfWisdomGrow();
				break;
			}
			}
			return;
		}
		switch (value_type)
		{
		case '<':
			mApp.DoNewOptions(theFromGameSelector: false);
			break;
		case '2':
			AddZombie(ZombieType.Digger, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'l':
			mApp.DoCheatDialog();
			break;
		case '#':
			if (mApp.IsSurvivalMode())
			{
				if (mApp.mGameScene == GameScenes.LevelIntro)
				{
					return;
				}
				mCurrentWave = mNumWaves;
				mChallenge.mSurvivalStage += 5;
				RemoveAllZombies();
				FadeOutLevel();
			}
			else
			{
				GetBossZombie()?.ApplyBossSmokeParticles(theEnable: true);
			}
			break;
		case '!':
			mApp.mBoardResult = BoardResult.Cheat;
			if (IsLastStandStageWithRepick())
			{
				if (mNextSurvivalStageCounter == 0)
				{
					mCurrentWave = mNumWaves;
					RemoveAllZombies();
					FadeOutLevel();
				}
			}
			else if ((mApp.IsScaryPotterLevel() && !IsFinalScaryPotterStage()) || mApp.IsEndlessIZombie(mApp.mGameMode))
			{
				if (mNextSurvivalStageCounter == 0)
				{
					RemoveAllZombies();
					FadeOutLevel();
				}
			}
			else if (mApp.IsSurvivalMode())
			{
				if (mApp.mGameScene == GameScenes.LevelIntro)
				{
					return;
				}
				mCurrentWave = mNumWaves;
				if (!IsSurvivalStageWithRepick())
				{
					RemoveAllZombies();
				}
				FadeOutLevel();
			}
			else
			{
				RemoveAllZombies();
				FadeOutLevel();
				mBoardFadeOutCounter = 200;
			}
			break;
		case '+':
			mApp.mBoardResult = BoardResult.Cheat;
			if (IsLastStandStageWithRepick())
			{
				if (mNextSurvivalStageCounter == 0)
				{
					mCurrentWave = mNumWaves;
					RemoveAllZombies();
					FadeOutLevel();
				}
			}
			else if ((mApp.IsScaryPotterLevel() && !IsFinalScaryPotterStage()) || mApp.IsEndlessIZombie(mApp.mGameMode))
			{
				if (mNextSurvivalStageCounter == 0)
				{
					RemoveAllZombies();
					FadeOutLevel();
				}
			}
			else if (mApp.IsSurvivalEndless(mApp.mGameMode))
			{
				if (mApp.mGameScene == GameScenes.LevelIntro)
				{
					return;
				}
				mCurrentWave = mNumWaves;
				RemoveAllZombies();
				FadeOutLevel();
			}
			else if (mApp.IsSurvivalMode())
			{
				mChallenge.mSurvivalStage = 5;
				RemoveAllZombies();
				FadeOutLevel();
				mBoardFadeOutCounter = 200;
			}
			else
			{
				RemoveAllZombies();
				FadeOutLevel();
				mBoardFadeOutCounter = 200;
			}
			break;
		case '8':
			mApp.mEasyPlantingCheat = !mApp.mEasyPlantingCheat;
			break;
		case '7':
			mApp.ToggleSlowMo();
			break;
		case '6':
			mApp.ToggleFastMo();
			break;
		case 'z':
			ClearAdviceImmediately();
			DisplayAdviceAgain("[ADVICE_HUGE_WAVE]", MessageStyle.HugeWave, AdviceType.HugeWave);
			mHugeWaveCountDown = 750;
			mDebugTextMode++;
			if (mDebugTextMode > DebugTextMode.Collision)
			{
				mDebugTextMode = DebugTextMode.None;
			}
			break;
		}
		if (mApp.mGameScene != GameScenes.Playing)
		{
			return;
		}
		Zombie bossZombie = GetBossZombie();
		if (bossZombie != null && !bossZombie.IsDeadOrDying())
		{
			switch (value_type)
			{
			case 'b':
				bossZombie.mBossBungeeCounter = 0;
				return;
			case 'u':
				bossZombie.mSummonCounter = 0;
				return;
			case 's':
				bossZombie.mBossStompCounter = 0;
				return;
			case 'r':
				bossZombie.BossRVAttack();
				return;
			case 'h':
				bossZombie.mBossHeadCounter = 0;
				return;
			case 'd':
				bossZombie.TakeDamage(10000, 0u);
				return;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeWarAndPeas || mApp.mGameMode == GameMode.ChallengeWarAndPeas2)
		{
			switch (value_type)
			{
			case 'w':
				AddZombie(ZombieType.WallnutHead, GameConstants.ZOMBIE_WAVE_DEBUG);
				return;
			case 't':
				AddZombie(ZombieType.TallnutHead, GameConstants.ZOMBIE_WAVE_DEBUG);
				return;
			case 'j':
				AddZombie(ZombieType.JalapenoHead, GameConstants.ZOMBIE_WAVE_DEBUG);
				return;
			case 'g':
				AddZombie(ZombieType.GatlingHead, GameConstants.ZOMBIE_WAVE_DEBUG);
				return;
			case 's':
				AddZombie(ZombieType.SquashHead, GameConstants.ZOMBIE_WAVE_DEBUG);
				return;
			}
		}
		if (value_type == 'q' && mApp.IsSurvivalEndless(mApp.mGameMode))
		{
			mApp.mEasyPlantingCheat = true;
			for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
			{
				for (int j = 0; j < Constants.GRIDSIZEX; j++)
				{
					if (CanPlantAt(j, i, SeedType.Lilypad) == PlantingReason.Ok)
					{
						AddPlant(j, i, SeedType.Lilypad, SeedType.None);
					}
					if (CanPlantAt(j, i, SeedType.Pumpkinshell) == PlantingReason.Ok && (j <= 6 || IsPoolSquare(j, i)))
					{
						AddPlant(j, i, SeedType.Pumpkinshell, SeedType.None);
					}
					if (CanPlantAt(j, i, SeedType.Gatlingpea) != PlantingReason.Ok)
					{
						continue;
					}
					if (j < 5)
					{
						AddPlant(j, i, SeedType.Gatlingpea, SeedType.None);
						continue;
					}
					switch (j)
					{
					case 5:
						AddPlant(j, i, SeedType.Torchwood, SeedType.None);
						continue;
					case 6:
						AddPlant(j, i, SeedType.Splitpea, SeedType.None);
						continue;
					}
					if (i == 2 || i == 3)
					{
						AddPlant(j, i, SeedType.Gloomshroom, SeedType.None);
						if (CanPlantAt(j, i, SeedType.InstantCoffee) == PlantingReason.Ok)
						{
							AddPlant(j, i, SeedType.InstantCoffee, SeedType.None);
						}
					}
				}
			}
			return;
		}
		if (value_type == 'q' && mApp.IsIZombieLevel())
		{
			mApp.mEasyPlantingCheat = true;
			if (mApp.IsIZombieLevel())
			{
				for (int k = 0; k < 5; k++)
				{
					mChallenge.IZombiePlaceZombie(ZombieType.Football, 6, k);
				}
			}
			return;
		}
		switch (value_type)
		{
		case 'q':
		{
			mApp.mEasyPlantingCheat = true;
			for (int l = 0; l < Constants.MAX_GRIDSIZEY; l++)
			{
				for (int m = 0; m < Constants.GRIDSIZEX; m++)
				{
					if (StageHasRoof() && CanPlantAt(m, l, SeedType.Flowerpot) == PlantingReason.Ok)
					{
						AddPlant(m, l, SeedType.Flowerpot, SeedType.None);
					}
					if (CanPlantAt(m, l, SeedType.Lilypad) == PlantingReason.Ok)
					{
						AddPlant(m, l, SeedType.Lilypad, SeedType.None);
					}
					if (CanPlantAt(m, l, SeedType.Threepeater) == PlantingReason.Ok)
					{
						AddPlant(m, l, SeedType.Threepeater, SeedType.None);
					}
				}
			}
			if (!mChallenge.UpdateZombieSpawning())
			{
				int num9 = Math.Min(mNumWaves - mCurrentWave, 20);
				for (int n = 0; n < num9; n++)
				{
					SpawnZombieWave();
				}
			}
			if (!mApp.IsScaryPotterLevel())
			{
				break;
			}
			int index3 = -1;
			GridItem theGridItem = null;
			while (IterateGridItems(ref theGridItem, ref index3))
			{
				if (theGridItem.mGridItemType == GridItemType.ScaryPot)
				{
					mChallenge.ScaryPotterOpenPot(theGridItem);
				}
			}
			break;
		}
		case ']':
		{
			mApp.mEasyPlantingCheat = true;
			for (int num12 = 0; num12 < Constants.MAX_GRIDSIZEY; num12++)
			{
				for (int num13 = 0; num13 < Constants.GRIDSIZEX; num13++)
				{
					if (StageHasRoof() && CanPlantAt(num13, num12, SeedType.Flowerpot) == PlantingReason.Ok)
					{
						AddPlant(num13, num12, SeedType.Flowerpot, SeedType.None);
					}
					if (CanPlantAt(num13, num12, SeedType.Lilypad) == PlantingReason.Ok)
					{
						AddPlant(num13, num12, SeedType.Lilypad, SeedType.None);
					}
					if (CanPlantAt(num13, num12, SeedType.Peashooter) == PlantingReason.Ok)
					{
						AddPlant(num13, num12, SeedType.Peashooter, SeedType.None);
					}
				}
			}
			if (!mChallenge.UpdateZombieSpawning())
			{
				int num14 = Math.Min(mNumWaves - mCurrentWave, 20);
				for (int num15 = 0; num15 < num14; num15++)
				{
					SpawnZombieWave();
				}
			}
			if (!mApp.IsScaryPotterLevel())
			{
				break;
			}
			int index4 = -1;
			GridItem theGridItem2 = null;
			while (IterateGridItems(ref theGridItem2, ref index4))
			{
				if (theGridItem2.mGridItemType == GridItemType.ScaryPot)
				{
					mChallenge.ScaryPotterOpenPot(theGridItem2);
				}
			}
			break;
		}
		case '[':
			if (!mChallenge.UpdateZombieSpawning())
			{
				int num10 = 1;
				for (int num11 = 0; num11 < num10; num11++)
				{
					SpawnZombieWave();
				}
			}
			break;
		case '/':
		case '?':
			if (mHugeWaveCountDown > 0)
			{
				mHugeWaveCountDown = 1;
			}
			else
			{
				mZombieCountDown = 6;
			}
			break;
		case 'b':
			if (mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist)
			{
				AddZombie(ZombieType.Bungee, GameConstants.ZOMBIE_WAVE_DEBUG);
			}
			break;
		case 'f':
			AddZombie(ZombieType.Propeller, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case '@':
			AddZombie(ZombieType.Ninja, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case '>':
			AddZombie(ZombieType.Imp, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'T':
			AddZombie(ZombieType.RobotTitan, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'O':
			AddZombie(ZombieType.Football, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 's':
			AddZombie(ZombieType.Door, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'L':
			AddZombie(ZombieType.Ladder, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'y':
			AddZombie(ZombieType.Yeti, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'a':
			AddZombie(ZombieType.Flag, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'w':
			AddZombie(ZombieType.Newspaper, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'F':
			AddZombie(ZombieType.Balloon, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'n':
			if (StageHasPool())
			{
				AddZombie(ZombieType.Snorkel, GameConstants.ZOMBIE_WAVE_DEBUG);
			}
			break;
		case 'c':
			AddZombie(ZombieType.TrafficCone, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'm':
			AddZombie(ZombieType.Dancer, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case '*':
			Console.Write("CRASHING ON PURPOSE\n");
			Debug.ASSERT(value: false);
			break;
		case ' ':
			mApp.DoPauseDialog();
			break;
		case 'h':
			AddZombie(ZombieType.Pail, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'H':
			AddZombie(ZombieType.Pail, GameConstants.ZOMBIE_WAVE_DEBUG);
			AddZombie(ZombieType.Pail, GameConstants.ZOMBIE_WAVE_DEBUG);
			AddZombie(ZombieType.Pail, GameConstants.ZOMBIE_WAVE_DEBUG);
			AddZombie(ZombieType.Pail, GameConstants.ZOMBIE_WAVE_DEBUG);
			AddZombie(ZombieType.Pail, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'D':
			AddZombie(ZombieType.Digger, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'p':
			AddZombie(ZombieType.Polevaulter, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'P':
			AddZombie(ZombieType.Pogo, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'R':
			if (StageHasPool())
			{
				AddZombie(ZombieType.DolphinRider, GameConstants.ZOMBIE_WAVE_DEBUG);
			}
			break;
		case 'j':
			AddZombie(ZombieType.JackInTheBox, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'g':
			AddZombie(ZombieType.Gargantuar, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'G':
			AddZombie(ZombieType.RedeyeGargantuar, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'i':
			AddZombie(ZombieType.Zamboni, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'C':
			AddZombie(ZombieType.Catapult, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case '1':
		{
			Plant topPlantAt = GetTopPlantAt(0, 0, TopPlant.Any);
			if (topPlantAt != null)
			{
				topPlantAt.Die();
				mChallenge.ZombieAtePlant(null, topPlantAt);
			}
			break;
		}
		case 'B':
			mFogBlownCountDown = 2200;
			break;
		case 't':
			if (!CanAddBobSled())
			{
				int num7 = RandomNumbers.NextNumber(5);
				int num8 = 400 + Constants.BOARD_EXTRA_ROOM;
				if (StageHasPool())
				{
					num7 = RandomNumbers.NextNumber(2);
				}
				else if (StageHasRoof())
				{
					num8 = 500 + Constants.BOARD_EXTRA_ROOM;
				}
				mIceTimer[num7] = 3000;
				mIceMinX[num7] = num8;
			}
			AddZombie(ZombieType.Bobsled, GameConstants.ZOMBIE_WAVE_DEBUG);
			break;
		case 'r':
			SpawnZombiesFromGraves();
			break;
		case '0':
			AddSunMoney(100);
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
			break;
		case '9':
			AddSunMoney(999999);
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
			break;
		case '$':
			mApp.mPlayerInfo.AddCoins(100);
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
			ShowCoinBank();
			break;
		case '-':
			mSunMoney -= 100;
			if (mSunMoney < 0)
			{
				mSunMoney = 0;
			}
			break;
		case 'M':
			mLawnMowers.Clear();
			break;
		}
	}

	public override void KeyUp(KeyCode theKey)
	{
	}

	public override void KeyDown(KeyCode theKey)
	{
		if (theKey == KeyCode.Escape && (IsGridFocusActive || mCursorObject.mMagnifier))
		{
			BackButtonPress();
			return;
		}
		DoTypingCheck(theKey);
		if ((ushort)theKey == 32 || (ushort)theKey == 13)
		{
			if (IsScaryPotterDaveTalking() && mApp.mCrazyDaveMessageIndex != -1)
			{
				mChallenge.AdvanceCrazyDaveDialog();
				return;
			}
			if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom)
			{
				mApp.mZenGarden.AdvanceCrazyDaveDialog();
				return;
			}
		}
		if ((ushort)theKey == 32)
		{
			mApp.CanPauseNow();
		}
		if (theKey == KeyCode.Escape)
		{
			if (mCursorObject.mCursorType != CursorType.Normal)
			{
				RefreshSeedPacketFromCursor();
				return;
			}
			if (CanInteractWithBoardButtons() && mApp.mGameScene != GameScenes.ZombiesWon)
			{
				mApp.DoNewOptions(theFromGameSelector: false);
			}
		}
		_ = 164;
	}

	public override void Update()
	{
		base.Update();
		MarkDirty();
		mCutScene.Update();
		if (mCameraEnabled && mCamera != null)
		{
			if (mApp.mGameScene != GameScenes.Playing)
			{
				mCursorObject.mMagnifier = false;
				(float, float, float)? tuple = mGridFocusReturn;
				if (tuple.HasValue)
				{
					(float, float, float) valueOrDefault = tuple.GetValueOrDefault();
					mGridFocusReturning = true;
					mCamera.SetPositionSmooth(valueOrDefault.Item1, valueOrDefault.Item2, valueOrDefault.Item3, 1, TodCurves.Linear);
				}
			}
			mCamera.Update();
			if (mGridFocusReturning && mCamera.OperationCountdown == 0)
			{
				mGridFocusReturn = null;
				mGridFocusReturning = false;
			}
		}
		UpdateMousePosition();
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mApp.mZenGarden.ZenGardenUpdate();
			mApp.UpdateCrazyDave();
		}
		if (IsScaryPotterDaveTalking())
		{
			mApp.UpdateCrazyDave();
		}
		if (mPaused)
		{
			mChallenge.Update();
			mCursorPreview.mVisible = false;
			mCursorObject.mVisible = false;
			return;
		}
		bool flag = !CanInteractWithBoardButtons() || mIgnoreMouseUp;
		if (!mMenuButton.mBtnNoDraw)
		{
			mMenuButton.mDisabled = flag;
		}
		mMenuButton.Update();
		if (mStoreButton != null)
		{
			mStoreButton.mDisabled = flag;
			mStoreButton.Update();
		}
		if (mCornRouletteSelfButton != null)
		{
			mCornRouletteSelfButton.mDisabled = flag;
			mCornRouletteOtherButton.mDisabled = flag;
			mCornRouletteSelfButton.Update();
			mCornRouletteOtherButton.Update();
		}
		if (mAccelerateButton != null)
		{
			mAccelerateButton.mDisabled = flag;
			mAccelerateButton.Update();
		}
		if (mPauseButton != null)
		{
			mPauseButton.mDisabled = flag;
			mPauseButton.mChecked = mManualPaused;
			mPauseButton.Update();
		}
		mApp.mEffectSystem.Update();
		mAdvice.Update();
		UpdateTutorial();
		if (mCobCannonCursorDelayCounter > 0)
		{
			mCobCannonCursorDelayCounter--;
		}
		if (mOutOfMoneyCounter > 0)
		{
			mOutOfMoneyCounter--;
		}
		if (mShakeCounter > 0)
		{
			mShakeCounter--;
			if (mShakeCounter == 0)
			{
				mX = Constants.Board_Offset_AspectRatio_Correction;
				mY = 0;
			}
			else
			{
				mX = TodCommon.TodAnimateCurve(12, 0, mShakeCounter, Constants.Board_Offset_AspectRatio_Correction, Constants.Board_Offset_AspectRatio_Correction - mShakeAmountX, TodCurves.Bounce);
				mY = TodCommon.TodAnimateCurve(12, 0, mShakeCounter, 0, mShakeAmountY, TodCurves.Bounce);
			}
		}
		if (mCoinBankFadeCount > 0 && mApp.GetDialog(Dialogs.DIALOG_PURCHASE_PACKET_SLOT) == null)
		{
			mCoinBankFadeCount--;
		}
		if (mLevelFadeCount > 0)
		{
			mLevelFadeCount--;
		}
		UpdateLayers();
		if (mTimeStopCounter > 0)
		{
			return;
		}
		if (mAccelerationNumerator == 0 || mManualPaused)
		{
			Coin theCoin = null;
			while (IterateCoins(ref theCoin))
			{
				if (theCoin.mIsBeingCollected)
				{
					theCoin.Update();
				}
			}
			mCursorPreview.Update();
			mCursorObject.Update();
			return;
		}
		int i = 0;
		int num = mAccelerationNumerator * (mAccelerationFrameIndex + 1) / mAccelerationDenominator - mAccelerationNumerator * mAccelerationFrameIndex / mAccelerationDenominator;
		if (mAccelerationDenominator != 1)
		{
			mAccelerationFrameIndex++;
			mAccelerationFrameIndex %= mAccelerationDenominator;
		}
		for (; i < num; i++)
		{
			mEffectCounter++;
			if (StageHasPool() && mPoolSparklyParticleID == null)
			{
				int aRenderOrder = 220000;
				TodParticleSystem theParticle = mApp.AddTodParticle(450 + Constants.BOARD_EXTRA_ROOM, 295f, aRenderOrder, StageHasBigPool() ? ParticleEffect.PoolSparklyFull : ParticleEffect.PoolSparkly);
				mPoolSparklyParticleID = mApp.ParticleGetID(theParticle);
			}
			UpdateGridItems();
			UpdateFwoosh();
			UpdateGame();
			UpdateFog();
			UpdateCoverLayer();
			mChallenge.Update();
			UpdateLevelEndSequence();
		}
		mPrevMouseX = mApp.mWidgetManager.mLastMouseX;
		mPrevMouseY = mApp.mWidgetManager.mLastMouseY;
	}

	public void UpdateMousePosition()
	{
		UpdateCursor();
		Plant thePlant = null;
		int index = -1;
		while (IteratePlants(ref thePlant, ref index))
		{
			thePlant.mHighlighted = false;
		}
		SeedType seedTypeInCursor = GetSeedTypeInCursor();
		int num = mApp.mWidgetManager.mLastMouseX - mX;
		int num2 = mApp.mWidgetManager.mLastMouseY - mY;
		_ = mCameraEnabled;
		if (mApp.IsScaryPotterLevel())
		{
			GridItem theGridItem = null;
			int index2 = -1;
			while (IterateGridItems(ref theGridItem, ref index2))
			{
				if (theGridItem.mGridItemType == GridItemType.ScaryPot)
				{
					theGridItem.mHighlighted = false;
				}
			}
			HitResult theHitResult = default;
			MouseHitTest(num, num2, out theHitResult, posScaled: false);
			if (theHitResult.mObjectType == GameObjectType.ScaryPot)
			{
				GridItem gridItem = (GridItem)theHitResult.mObject;
				gridItem.mHighlighted = true;
				return;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			GridItem stinky = mApp.mZenGarden.GetStinky();
			if (stinky != null)
			{
				MouseHitTest(num, num2, out var theHitResult2, posScaled: false);
				stinky.mHighlighted = theHitResult2.mObjectType == GameObjectType.Stinky;
			}
		}
		if (mCursorObject.mCursorType == CursorType.Shovel || mCursorObject.mCursorType == CursorType.WateringCan || mCursorObject.mCursorType == CursorType.Fertilizer || mCursorObject.mCursorType == CursorType.BugSpray || mCursorObject.mCursorType == CursorType.Phonograph || mCursorObject.mCursorType == CursorType.Chocolate || mCursorObject.mCursorType == CursorType.Glove || mCursorObject.mCursorType == CursorType.MoneySign || (mCursorObject.mCursorType == CursorType.Wheeelbarrow && mApp.mZenGarden.GetPottedPlantInWheelbarrow() == null))
		{
			HighlightPlantsForMouse(num, num2);
			return;
		}
		switch (seedTypeInCursor)
		{
		case SeedType.InstantCoffee:
		{
			int theGridX2 = PlantingPixelToGridX((int)((float)num * Constants.IS), (int)((float)num2 * Constants.IS), seedTypeInCursor);
			int theGridY2 = PlantingPixelToGridY((int)((float)num * Constants.IS), (int)((float)num2 * Constants.IS), seedTypeInCursor);
			thePlant = GetTopPlantAt(theGridX2, theGridY2, TopPlant.OnlyNormalPosition);
			if (thePlant != null && thePlant.mIsAsleep && CanPlantAt(theGridX2, theGridY2, SeedType.InstantCoffee) == PlantingReason.Ok)
			{
				thePlant.mHighlighted = true;
			}
			break;
		}
		case SeedType.Wallnut:
		case SeedType.Tallnut:
		{
			int theGridX3 = PlantingPixelToGridX((int)((float)num * Constants.IS), (int)((float)num2 * Constants.IS), seedTypeInCursor);
			int theGridY3 = PlantingPixelToGridY((int)((float)num * Constants.IS), (int)((float)num2 * Constants.IS), seedTypeInCursor);
			thePlant = GetTopPlantAt(theGridX3, theGridY3, TopPlant.OnlyPumpkin);
			if (thePlant != null && thePlant.mSeedType == seedTypeInCursor && CanPlantAt(theGridX3, theGridY3, seedTypeInCursor) == PlantingReason.Ok)
			{
				thePlant.mHighlighted = true;
			}
			break;
		}
		case SeedType.Pumpkinshell:
		{
			int theGridX = PlantingPixelToGridX((int)((float)num * Constants.IS), (int)((float)num2 * Constants.IS), seedTypeInCursor);
			int theGridY = PlantingPixelToGridY((int)((float)num * Constants.IS), (int)((float)num2 * Constants.IS), seedTypeInCursor);
			thePlant = GetTopPlantAt(theGridX, theGridY, TopPlant.OnlyNormalPosition);
			if (thePlant != null && thePlant.mSeedType == SeedType.Pumpkinshell && CanPlantAt(theGridX, theGridY, SeedType.Pumpkinshell) == PlantingReason.Ok)
			{
				thePlant.mHighlighted = true;
			}
			break;
		}
		}
	}

	public void HighlightPlantsForMouse(int theMouseX, int theMouseY)
	{
		if (mCursorObject.mCursorType == CursorType.WateringCan && mApp.mPlayerInfo.mPurchases[13] != 0)
		{
			bool flag = (uint)mApp.mPlayerInfo.mPurchases[13] >= 2u;
			Plant thePlant = null;
			int index = -1;
			while (IteratePlants(ref thePlant, ref index))
			{
				if ((flag && thePlant == GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.ZenToolOrder)) || (!flag && IsPlantInGoldWateringCanRange(theMouseX, theMouseY, thePlant)))
				{
					thePlant.mHighlighted = true;
					Plant topPlantAt = GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyUnderPlant);
					if (topPlantAt != null)
					{
						topPlantAt.mHighlighted = true;
					}
				}
			}
			return;
		}
		HitResult hitResult = ToolHitTest(theMouseX, theMouseY, posScaled: false, isTouch: false);
		if (hitResult.mObjectType != GameObjectType.Plant)
		{
			return;
		}
		Plant plant = (Plant)hitResult.mObject;
		plant.mHighlighted = true;
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			Plant topPlantAt2 = GetTopPlantAt(plant.mPlantCol, plant.mRow, TopPlant.OnlyUnderPlant);
			if (topPlantAt2 != null)
			{
				topPlantAt2.mHighlighted = true;
			}
		}
	}

	public void UpdateLayers()
	{
		if (mWidgetManager != null)
		{
			mWidgetManager.MarkAllDirty();
		}
		for (LinkedListNode<Dialog> linkedListNode = mApp.mDialogList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			Dialog value = linkedListNode.Value;
			mWidgetManager.BringToFront(value);
			value.MarkDirty();
		}
	}

	public override void Draw(Graphics g)
	{
		if (mApp.GetDialog(Dialogs.DIALOG_STORE) != null || mApp.GetDialog(Dialogs.DIALOG_ALMANAC) != null)
		{
			return;
		}
		g.SetLinearBlend(linear: true);
		if (mDrawCount == 0 || !mCutScene.mPreloaded)
		{
			ResetFPSStats();
		}
		else
		{
			int tickCount = Environment.TickCount;
			int num = mDrawCount - mIntervalDrawCountStart;
			int num2 = tickCount - mIntervalDrawTime;
			if (num2 > 10000)
			{
				float num3 = ((float)num * 1000f + 500f) / (float)num2;
				if (num3 < mMinFPS)
				{
					mMinFPS = num3;
				}
				mIntervalDrawCountStart = mDrawCount;
				mIntervalDrawTime = tickCount;
			}
		}
		mDrawCount++;
		if (mCameraEnabled && mCamera != null)
		{
			mCamera.ApplyTransform(g);
		}
		DrawGameObjects(g);
		if (mCameraEnabled && mCamera != null)
		{
			mCamera.ResetTransform(g);
		}
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
	}

	public void DrawBackdrop(Graphics g)
	{
		int num = 0;
		int num2 = 0;
		Image image = null;
		switch (mBackground)
		{
		case BackgroundType.Num1Day:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND1;
			break;
		case BackgroundType.Num2Night:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND2;
			break;
		case BackgroundType.Num3Pool:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND3;
			break;
		case BackgroundType.Num4Fog:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND4;
			break;
		case BackgroundType.Num5Roof:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND5;
			break;
		case BackgroundType.Num6Boss:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND6BOSS;
			break;
		case BackgroundType.HighGround:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND7;
			break;
		case BackgroundType.BigPool:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND8;
			break;
		case BackgroundType.MushroomGarden:
			num = -133;
			num2 = 0;
			image = Resources.IMAGE_BACKGROUND_MUSHROOMGARDEN;
			break;
		case BackgroundType.Greenhouse:
			num = -133;
			num2 = 0;
			image = Resources.IMAGE_BACKGROUND_GREENHOUSE;
			break;
		case BackgroundType.Zombiquarium:
			num = -133;
			num2 = 0;
			image = Resources.IMAGE_AQUARIUM1;
			break;
		case BackgroundType.TreeOfWisdom:
			image = null;
			break;
		case BackgroundType.GreenhouseNight:
			num = -133;
			num2 = 0;
			image = Resources.IMAGE_BACKGROUND_GREENHOUSE_NIGHT;
			break;
		case BackgroundType.ShallowDay:
			num = -192;
			num2 = -48;
			image = Resources.IMAGE_BACKGROUND1;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		num2 = (int)((float)num2 * 1.25f);
		num = (int)((float)num * 1.25f);
		GameMode mGameMode = mApp.mGameMode;
		if (mLevel == 1 && mApp.IsFirstTimeAdventureMode())
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND1UNSODDED, (float)num - (float)Constants.BOARD_OFFSET * Constants.S, num2);
			int theWidth = TodCommon.TodAnimateCurve(0, 1000, mSodPosition, 0, AtlasResources.IMAGE_SOD1ROW.GetWidth(), TodCurves.Linear);
			g.DrawImage(theSrcRect: new TRect(0, 0, theWidth, AtlasResources.IMAGE_SOD1ROW.GetHeight()), theImage: AtlasResources.IMAGE_SOD1ROW, theX: (int)((0f - (float)Constants.BOARD_OFFSET + 239f) * Constants.S), theY: (int)(265f * Constants.S));
		}
		else if (((mLevel == 2 || mLevel == 3) && mApp.IsFirstTimeAdventureMode()) || mApp.mGameMode == GameMode.ChallengeResodded)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND1UNSODDED, (float)num - (float)Constants.BOARD_OFFSET * Constants.S, num2);
			g.DrawImage(AtlasResources.IMAGE_SOD1ROW, (0f - (float)Constants.BOARD_OFFSET + 239f) * Constants.S, 265f * Constants.S);
			int theWidth2 = TodCommon.TodAnimateCurve(0, 1000, mSodPosition, 0, AtlasResources.IMAGE_SOD3ROW.GetWidth(), TodCurves.Linear);
			g.DrawImage(theSrcRect: new TRect(0, 0, theWidth2, AtlasResources.IMAGE_SOD3ROW.GetHeight()), theImage: AtlasResources.IMAGE_SOD3ROW, theX: (int)((0f - (float)Constants.BOARD_OFFSET + 235f) * Constants.S), theY: (int)(149f * Constants.S));
		}
		else if (mLevel == 4 && mApp.IsFirstTimeAdventureMode())
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND1UNSODDED, (float)num - (float)Constants.BOARD_OFFSET * Constants.S, num2);
			g.DrawImage(AtlasResources.IMAGE_SOD3ROW, (0f - (float)Constants.BOARD_OFFSET + 235f) * Constants.S, 149f * Constants.S);
			int num3 = TodCommon.TodAnimateCurve(0, 1000, mSodPosition, 0, 773, TodCurves.Linear);
			g.DrawImage(theSrcRect: new TRect((int)(232f * Constants.S) - num, -num2, (int)((float)num3 * Constants.S), Resources.IMAGE_BACKGROUND1.GetHeight()), theImage: Resources.IMAGE_BACKGROUND1, theX: (int)((0f - (float)Constants.BOARD_OFFSET + 232f) * Constants.S), theY: 0);
		}
		else if (image != null)
		{
			if (image == Resources.IMAGE_BACKGROUND_MUSHROOMGARDEN || image == Resources.IMAGE_BACKGROUND_GREENHOUSE || image == Resources.IMAGE_AQUARIUM1 || image == Resources.IMAGE_BACKGROUND_GREENHOUSE_NIGHT)
			{
				g.DrawImage(image, num - Constants.ZenGarden_Backdrop_X, num2);
			}
			else
			{
				g.DrawImage(image, (int)((float)num - (float)Constants.BOARD_OFFSET * Constants.S), num2);
			}
		}
		if (mApp.mGameScene == GameScenes.ZombiesWon)
		{
			DrawHouseDoorBottom(g);
		}
		if (mTutorialState == TutorialState.Level1PlantPeashooter)
		{
			Graphics graphics = Graphics.GetNew(g);
			SexyColor flashingColor = TodCommon.GetFlashingColor(mMainCounter, 75);
			graphics.SetColorizeImages(colorizeImages: true);
			graphics.SetColor(flashingColor);
			graphics.DrawImage(AtlasResources.IMAGE_SOD1ROW, (0f - (float)Constants.BOARD_OFFSET + 239f) * Constants.S, 265f * Constants.S);
			graphics.SetColorizeImages(colorizeImages: false);
			graphics.PrepareForReuse();
		}
		mChallenge.DrawBackdrop(g);
		if (mApp.mGameScene == GameScenes.LevelIntro && StageHasGraveStones())
		{
			g.DrawImage(AtlasResources.IMAGE_NIGHT_GRAVE_GRAPHIC, Constants.InvertAndScale(640f), 40f * Constants.S);
		}
	}

	public void DrawCursorOnBackground(Graphics g)
	{
		if (mTimeStopCounter == 0 && (!mApp.IsWhackAZombieLevel() || mCursorObject.mCursorType != CursorType.Hammer) && (float)mLastToolX >= (float)Constants.LAWN_XMIN * Constants.S && mCursorObject.BeginDraw(g))
		{
			mCursorObject.DrawGroundLayer(g);
			mCursorObject.EndDraw(g);
		}
	}

	public void DrawCursorOverlay(Graphics g)
	{
		if (mTimeStopCounter == 0 && mCursorObject.BeginDraw(g))
		{
			mCursorObject.DrawTopLayer(g);
			mCursorObject.EndDraw(g);
		}
	}

	public virtual void ButtonMouseEnter(int theId)
	{
	}

	public virtual void ButtonMouseLeave(int theId)
	{
	}

	public virtual void ButtonPress(int theId)
	{
	}

	public void AddSunMoney(int theAmount)
	{
		mSunMoney += theAmount;
		if (mSunMoney > 9990)
		{
			mSunMoney = 9990;
		}
	}

	public bool TakeSunMoney(int theAmount)
	{
		if (ZBCheatDialog.NoSunCost)
		{
			return true;
		}
		if (theAmount <= mSunMoney + CountSunBeingCollected())
		{
			mSunMoney -= theAmount;
			return true;
		}
		mApp.PlaySample(Resources.SOUND_BUZZER);
		mOutOfMoneyCounter = 70;
		return false;
	}

	public bool CanTakeSunMoney(int theAmount)
	{
		if (ZBCheatDialog.NoSunCost)
		{
			return true;
		}
		return theAmount <= mSunMoney + CountSunBeingCollected();
	}

	public void Pause(bool thePause)
	{
		if (mPaused == thePause)
		{
			return;
		}
		if (thePause)
		{
			mPaused = true;
			if (mApp.mPlayerInfo.mCoins > 0)
			{
				ShowCoinBank();
			}
			mLevelFadeCount = 1000;
			if (mApp.mGameScene != GameScenes.LevelIntro)
			{
				mApp.mSoundSystem.GamePause(theEnteringPause: true);
				mApp.mMusic.GameMusicPause(thePause: true);
			}
		}
		else
		{
			mPaused = false;
			mApp.mSoundSystem.GamePause(theEnteringPause: false);
			mApp.mMusic.GameMusicPause(thePause: false);
		}
	}

	public void TryToSaveGame()
	{
		string savedGameName = LawnCommon.GetSavedGameName(mApp.mGameMode, (int)mApp.mPlayerInfo.mId);
		if (NeedSaveGame())
		{
			Debug.Log(DebugType.Info, $"{GetType()}: Saving gamemode {mApp?.mGameMode}:\"{TodStringFile.TodStringTranslate(mLevelStr ?? string.Empty)}\" ({(int)(mApp?.mGameMode).Value}:{mLevel})");
			if (mBoardFadeOutCounter >= 0)
			{
				CompleteEndLevelSequenceForSaving();
				return;
			}
			Common.MkDir(GlobalStaticVars.GetDocumentsDir() + "userdata");
			mApp.mMusic.GameMusicPause(thePause: true);
			SaveGame(savedGameName);
			SurvivalSaveScore();
		}
	}

	public bool NeedSaveGame()
	{
		if (mApp.mGameMode != GameMode.ChallengeIce && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mApp.mGameMode != GameMode.Upsell && mApp.mGameMode != GameMode.Intro)
		{
			return mApp.mGameScene == GameScenes.Playing;
		}
		return false;
	}

	public bool RowCanHaveZombies(int theRow)
	{
		if (theRow >= 0 && theRow < Constants.MAX_GRIDSIZEY)
		{
			if (mApp.mGameMode != GameMode.ChallengeResodded || theRow > 4)
			{
				return mPlantRow[theRow] != PlantRowType.Dirt;
			}
			return true;
		}
		return false;
	}

	public void ProcessDeleteQueue()
	{
		for (int num = mPlants.Count - 1; num >= 0; num--)
		{
			if (mPlants[num].mDead)
			{
				mPlants[num].PrepareForReuse();
				mPlants.RemoveAt(num);
			}
		}
		for (int num2 = mZombies.Count - 1; num2 >= 0; num2--)
		{
			if (mZombies[num2].mDead)
			{
				if (mZombies[num2].mZombieType == ZombieType.Boss)
				{
					for (int i = 0; i < 6; i++)
					{
						List<Zombie> zombiesInRow = GetZombiesInRow(i);
						zombiesInRow.Remove(mZombies[num2]);
					}
				}
				else
				{
					List<Zombie> zombiesInRow2 = GetZombiesInRow(mZombies[num2].mRow);
					zombiesInRow2.Remove(mZombies[num2]);
				}
				mZombies[num2].PrepareForReuse();
				mZombies.RemoveAt(num2);
			}
		}
		for (int num3 = mProjectiles.Count - 1; num3 >= 0; num3--)
		{
			if (mProjectiles[num3].mDead)
			{
				mProjectiles[num3].PrepareForReuse();
				mProjectiles.RemoveAt(num3);
			}
		}
		for (int num4 = mCoins.Count - 1; num4 >= 0; num4--)
		{
			if (mCoins[num4].mDead)
			{
				mCoins.RemoveAt(num4);
			}
		}
		for (int num5 = mLawnMowers.Count - 1; num5 >= 0; num5--)
		{
			if (mLawnMowers[num5].mDead)
			{
				mLawnMowers[num5].PrepareForReuse();
				mLawnMowers.RemoveAt(num5);
			}
		}
		for (int num6 = mGridItems.Count - 1; num6 >= 0; num6--)
		{
			if (mGridItems[num6].mDead)
			{
				mGridItems[num6].PrepareForReuse();
				mGridItems.RemoveAt(num6);
			}
		}
		for (int num7 = mWinds.Count - 1; num7 >= 0; num7--)
		{
			if (mWinds[num7].mDead)
			{
				mWinds[num7].PrepareForReuse();
				mWinds.RemoveAt(num7);
			}
		}
	}

	public bool ChooseSeedsOnCurrentLevel()
	{
		if (mApp.IsLevelUseJson())
		{
			CSSeedBank component = mApp.mCreativeLevel.GetComponent<CSSeedBank>();
			if (component != null)
			{
				return component.mUserChoose;
			}
		}
		if (!mApp.IsChallengeWithoutSeedBank() && !HasConveyorBeltSeedBank() && mApp.mGameMode != GameMode.ChallengeIce && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist && mApp.mGameMode != GameMode.ChallengeZombiquarium && mApp.mGameMode != GameMode.ImitaterRandom && mApp.mGameMode != GameMode.MarvelousPeople4 && mApp.mGameMode != GameMode.ChallengeFusion && !mApp.IsIZombieLevel() && !mApp.IsSquirrelLevel() && !mApp.IsSlotMachineLevel())
		{
			if ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && mApp.IsFirstTimeAdventureMode())
			{
				return mLevel > 7;
			}
			return true;
		}
		return false;
	}

	public int GetNumSeedsInBank()
	{
		if (mApp.IsLevelUseJson())
		{
			CSSeedBank component = mApp.mCreativeLevel.GetComponent<CSSeedBank>();
			if (component != null && component.mNumPackets != -1)
			{
				return component.mNumPackets;
			}
		}
		if (mApp.IsScaryPotterLevel())
		{
			return 1;
		}
		if (mApp.IsWhackAZombieLevel())
		{
			return 3;
		}
		if (mApp.IsCornRouletteLevel())
		{
			return 8;
		}
		if (mApp.IsChallengeWithoutSeedBank())
		{
			return 0;
		}
		if (HasConveyorBeltSeedBank())
		{
			return 10;
		}
		if (mApp.mGameMode == GameMode.ChallengeIce)
		{
			return 6;
		}
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			return 0;
		}
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			return 2;
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie1 || mApp.mGameMode == GameMode.PuzzleIZombie2 || mApp.mGameMode == GameMode.PuzzleIZombie3 || mApp.mGameMode == GameMode.PuzzleIZombie4)
		{
			return 3;
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie5 || mApp.mGameMode == GameMode.PuzzleIZombie6 || mApp.mGameMode == GameMode.PuzzleIZombie7)
		{
			return 4;
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie8)
		{
			return 6;
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombie9)
		{
			return 8;
		}
		if (mApp.mGameMode == GameMode.PuzzleIZombieEndless)
		{
			return 9;
		}
		if (mApp.IsSlotMachineLevel())
		{
			return 3;
		}
		if (mApp.mGameMode == GameMode.ImitaterRandom)
		{
			return 10;
		}
		if (mApp.mGameMode == GameMode.ChallengeStageRandom)
		{
			return 10;
		}
		if (mApp.mGameMode == GameMode.MarvelousPeople4)
		{
			return 10;
		}
		if (mApp.mGameMode == GameMode.ChallengeFusion)
		{
			return 10;
		}
		int num = Math.Min(mApp.mPlayerInfo.mPurchases[21] + 6, 10);
		int seedsAvailable = mApp.GetSeedsAvailable();
		if (seedsAvailable < num)
		{
			num = seedsAvailable;
		}
		return num;
	}

	public bool StageIsDayWithoutPool()
	{
		return mBackground == BackgroundType.Num1Day;
	}

	public bool StageIsNight()
	{
		if (mBackground != BackgroundType.Num2Night && mBackground != BackgroundType.Num4Fog && mBackground != BackgroundType.HighGround && mBackground != BackgroundType.MushroomGarden && mBackground != BackgroundType.Num6Boss && mBackground != BackgroundType.Zombiquarium)
		{
			return mBackground == BackgroundType.GreenhouseNight;
		}
		return true;
	}

	public bool StageHasPool()
	{
		if (mBackground != BackgroundType.Num3Pool && mBackground != BackgroundType.Num4Fog)
		{
			return mBackground == BackgroundType.BigPool;
		}
		return true;
	}

	public bool StageHasBigPool()
	{
		return mBackground == BackgroundType.BigPool;
	}

	public bool StageHas6Rows()
	{
		if (mBackground != BackgroundType.Num3Pool && mBackground != BackgroundType.Num4Fog && mBackground != BackgroundType.HighGround)
		{
			return mBackground == BackgroundType.BigPool;
		}
		return true;
	}

	public bool StageHasFog()
	{
		if (mApp.IsLevelUseJson())
		{
			return mApp.mCreativeLevel.GetComponent<CSSpawnFog>() != null;
		}
		if (!mApp.IsStormyNightLevel() && mApp.mGameMode != GameMode.ChallengeInvisighoul && mBackground == BackgroundType.Num4Fog)
		{
			return !mApp.IsRogueConveyorbeltLevel();
		}
		return false;
	}

	public bool StageHasNinja()
	{
		if (!mApp.IsRogueConveyorbeltLevel() && mApp.mGameMode != GameMode.MarvelousPeople2)
		{
			return mApp.mGameMode == GameMode.MarvelousPeople3;
		}
		return true;
	}

	public bool StageHasGraveStones()
	{
		if (mApp.IsLevelUseJson())
		{
			if (mApp.mCreativeLevel.GetComponent<CSSpawnGraveStone>() == null)
			{
				return mApp.mCreativeLevel.GetComponent<CSWhackAZombieLevel>() != null;
			}
			return true;
		}
		if (mApp.IsWallnutBowlingLevel() || mApp.IsCornRouletteLevel() || mApp.mGameMode == GameMode.ChallengePogoParty || mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist || mApp.mGameMode == GameMode.ChallengePortalCombat || mApp.mGameMode == GameMode.ChallengeLastStand || mApp.IsIZombieLevel() || mApp.IsScaryPotterLevel() || mBackground != BackgroundType.Num2Night)
		{
			return mApp.mGameMode == GameMode.MarvelousPeople2;
		}
		return true;
	}

	public bool HasRain()
	{
		if (mApp.IsLevelUseJson())
		{
			CSWeather component = mApp.mCreativeLevel.GetComponent<CSWeather>();
			if (component != null)
			{
				return component.mHasRain;
			}
		}
		if (!mApp.IsStormyNightLevel())
		{
			return mApp.mGameMode == GameMode.ChallengeRainingSeeds;
		}
		return true;
	}

	public bool HasStorm()
	{
		if (mApp.IsLevelUseJson())
		{
			CSWeather component = mApp.mCreativeLevel.GetComponent<CSWeather>();
			if (component != null)
			{
				return component.mHasStorm;
			}
		}
		return mApp.IsStormyNightLevel();
	}

	public bool HasWeather()
	{
		if (!HasRain())
		{
			return HasStorm();
		}
		return true;
	}

	public int PixelToGridX(int theX, int theY)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && (mBackground == BackgroundType.MushroomGarden || mBackground == BackgroundType.Zombiquarium || mBackground == BackgroundType.Greenhouse || mBackground == BackgroundType.GreenhouseNight))
		{
			return mApp.mZenGarden.PixelToGridX(theX, theY);
		}
		if (theX < Constants.LAWN_XMIN)
		{
			return -1;
		}
		return TodCommon.ClampInt((theX - Constants.LAWN_XMIN) / 80, 0, Constants.GRIDSIZEX - 1);
	}

	public int PixelToGridY(int theX, int theY)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && (mBackground == BackgroundType.MushroomGarden || mBackground == BackgroundType.Zombiquarium || mBackground == BackgroundType.Greenhouse || mBackground == BackgroundType.GreenhouseNight))
		{
			return mApp.mZenGarden.PixelToGridY(theX, theY);
		}
		int num = PixelToGridX(theX, theY);
		if (num == -1 || theY < Constants.LAWN_YMIN)
		{
			return -1;
		}
		if (StageHasRoof())
		{
			int num2 = 0;
			if (num < 5)
			{
				num2 = (5 - num) * 20 - 20;
			}
			return TodCommon.ClampInt((theY - Constants.LAWN_YMIN - num2) / 85, 0, 4);
		}
		if (StageHas6Rows())
		{
			return TodCommon.ClampInt((theY - Constants.LAWN_YMIN) / 85, 0, 5);
		}
		return TodCommon.ClampInt((theY - Constants.LAWN_YMIN) / 100, 0, 4);
	}

	public int GridToPixelX(int theGridX, int theGridY)
	{
		Debug.ASSERT(theGridX >= 0 && theGridX < Constants.GRIDSIZEX);
		Debug.ASSERT(theGridY >= 0 && theGridY < Constants.MAX_GRIDSIZEY);
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && (mBackground == BackgroundType.MushroomGarden || mBackground == BackgroundType.Zombiquarium || mBackground == BackgroundType.Greenhouse || mBackground == BackgroundType.GreenhouseNight))
		{
			return mApp.mZenGarden.GridToPixelX(theGridX, theGridY);
		}
		return theGridX * Constants.New.Board_GridCellSizeX + Constants.LAWN_XMIN;
	}

	public int GridToPixelY(int theGridX, int theGridY)
	{
		Debug.ASSERT(theGridX >= 0 && theGridX < Constants.GRIDSIZEX);
		Debug.ASSERT(theGridY >= 0 && theGridY < Constants.MAX_GRIDSIZEY);
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && (mBackground == BackgroundType.MushroomGarden || mBackground == BackgroundType.Zombiquarium || mBackground == BackgroundType.Greenhouse || mBackground == BackgroundType.GreenhouseNight))
		{
			return mApp.mZenGarden.GridToPixelY(theGridX, theGridY);
		}
		int num;
		if (!StageHasRoof())
		{
			num = ((!StageHas6Rows()) ? (theGridY * Constants.New.Board_GridCellSizeY_5Rows + Constants.LAWN_YMIN) : (theGridY * Constants.New.Board_GridCellSizeY_6Rows + Constants.LAWN_YMIN));
		}
		else
		{
			int num2 = 0;
			if (theGridX < 5)
			{
				num2 = (5 - theGridX) * 20;
			}
			num = theGridY * Constants.New.Board_GridCellSizeY_6Rows + Constants.LAWN_YMIN + num2 - 10;
		}
		if (theGridX != -1 && mGridSquareType[theGridX, theGridY] == GridSquareType.HighGround)
		{
			num += -Constants.HIGH_GROUND_HEIGHT;
		}
		return num;
	}

	public int PixelToGridXKeepOnBoard(int theX, int theY)
	{
		return Math.Max(PixelToGridX(theX, theY), 0);
	}

	public int PixelToGridYKeepOnBoard(int theX, int theY)
	{
		int theX2 = Math.Max(theX, Constants.LAWN_XMIN);
		return Math.Max(PixelToGridY(theX2, theY), 0);
	}

	public void UpdateGameObjects()
	{
		int num = 0;
		int num2 = 0;
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead)
			{
				plant.Update();
				if (plant.mSeedType == SeedType.Agave)
				{
					num++;
				}
				if (plant.mSeedType == SeedType.Endoflame)
				{
					num2++;
				}
			}
		}
		count = mZombies.Count;
		for (int j = 0; j < count; j++)
		{
			Zombie zombie = mZombies[j];
			if (!zombie.mDead)
			{
				zombie.Update();
			}
		}
		count = mProjectiles.Count;
		for (int k = 0; k < count; k++)
		{
			Projectile projectile = mProjectiles[k];
			if (!projectile.mDead)
			{
				projectile.Update();
			}
		}
		Coin theCoin = null;
		while (IterateCoins(ref theCoin))
		{
			theCoin.Update();
		}
		LawnMower theLawnMower = null;
		while (IterateLawnMowers(ref theLawnMower))
		{
			theLawnMower.Update();
		}
		mCursorPreview.Update();
		mCursorObject.Update();
		for (int l = 0; l < mSeedBank.mNumPackets; l++)
		{
			SeedPacket seedPacket = mSeedBank.mSeedPackets[l];
			seedPacket.Update();
		}
		if (num == 0 || !IsPlantSkillEnabled(SeedType.Agave))
		{
			mAgaveSkillDrawType = 0;
		}
		else
		{
			mAgaveSkillDrawType = 1;
		}
		if (num2 != 0 && IsPlantSkillEnabled(SeedType.Endoflame))
		{
			mEndoflameSkillDrawType = 1;
		}
		else
		{
			mEndoflameSkillDrawType = 0;
		}
	}

	public bool MouseHitTest(int x, int y, out HitResult theHitResult, bool posScaled, bool isTouch = false)
	{
		if (!posScaled)
		{
			x = (int)((float)x * Constants.IS);
			y = (int)((float)y * Constants.IS);
		}
		int screenX;
		int num = (screenX = x);
		int screenY;
		int num2 = (screenY = y);
		if (mCameraEnabled && !posScaled)
		{
			mCamera.ScreenToBoardReplace(ref screenX, ref screenY);
		}
		theHitResult = default;
		if (mBoardFadeOutCounter >= 0)
		{
			theHitResult.mObject = null;
			theHitResult.mObjectType = GameObjectType.None;
			return false;
		}
		if (IsScaryPotterDaveTalking())
		{
			theHitResult.mObject = null;
			theHitResult.mObjectType = GameObjectType.None;
			return false;
		}
		if (mMenuButton.IsMouseOver() && CanInteractWithBoardButtons())
		{
			theHitResult.mObjectType = GameObjectType.MenuButton;
			return true;
		}
		if (mStoreButton != null && mStoreButton.IsMouseOver() && CanInteractWithBoardButtons())
		{
			theHitResult.mObjectType = GameObjectType.StoreButton;
			return true;
		}
		if (mCornRouletteSelfButton != null && CanInteractWithBoardButtons() && (mCornRouletteSelfButton.IsMouseOver() || mCornRouletteOtherButton.IsMouseOver()))
		{
			theHitResult.mObjectType = GameObjectType.CornRouletteButton;
			return true;
		}
		TRect shovelButtonRect = GetShovelButtonRect();
		x = (int)((float)x * Constants.S);
		y = (int)((float)y * Constants.S);
		if (mSeedBank.MouseHitTest(x, y, out theHitResult))
		{
			if (mCursorObject.mCursorType == CursorType.PlantFromBank)
			{
				if (isTouch)
				{
					SeedPacket seedPacket = (SeedPacket)theHitResult.mObject;
					int mSeedBankIndex = mCursorObject.mSeedBankIndex;
					_ = seedPacket.mIndex;
					return true;
				}
			}
			else if (mCursorObject.mCursorType == CursorType.Normal || mCursorObject.mCursorType == CursorType.Shovel || mCursorObject.mCursorType == CursorType.CobcannonTarget || mCursorObject.mCursorType == CursorType.Hammer || mCursorObject.mCursorType == CursorType.PlantFromUsableCoin)
			{
				return true;
			}
		}
		if (IsCombatToolVisible(GameObjectType.Shovel) && shovelButtonRect.Contains(x, y) && CanInteractWithBoardButtons())
		{
			theHitResult.mObjectType = GameObjectType.Shovel;
			return true;
		}
		if (mCameraEnabled)
		{
			int num3 = screenX;
			y = screenY;
			x = num3;
		}
		if (isTouch || (!isTouch && (mCursorObject.mCursorType == CursorType.Normal || mCursorObject.mCursorType == CursorType.Hammer)))
		{
			Coin coin = null;
			Coin theCoin = null;
			while (IterateCoins(ref theCoin))
			{
				if (theCoin.MouseHitTest(x, y, out var theHitResult2))
				{
					Coin coin2 = (Coin)theHitResult2.mObject;
					if (coin == null || coin2.mRenderOrder >= coin.mRenderOrder)
					{
						theHitResult = theHitResult2;
						coin = coin2;
					}
				}
			}
			if (coin != null)
			{
				return true;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			bool flag = false;
			if (mCursorObject.mCursorType == CursorType.Chocolate && !mApp.mZenGarden.IsStinkyHighOnChocolate())
			{
				flag = true;
			}
			else if (mCursorObject.mCursorType == CursorType.Normal && mApp.mZenGarden.IsStinkySleeping())
			{
				flag = true;
			}
			GridItem stinky = mApp.mZenGarden.GetStinky();
			if (flag && stinky != null && new TRect((int)(Constants.S * (stinky.mPosX - 6f)), (int)(Constants.S * (stinky.mPosY - 10f)), (int)(Constants.S * 84f), (int)(Constants.S * 90f)).Contains(x, y))
			{
				theHitResult.mObjectType = GameObjectType.Stinky;
				return true;
			}
		}
		if (mCameraEnabled)
		{
			y = num2;
			x = num;
		}
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		if (mApp.mGameMode == GameMode.TreeOfWisdom && mCursorObject.mCursorType == CursorType.TreeFood && mChallenge.TreeOfWisdomHitTest(x, y, out theHitResult))
		{
			return true;
		}
		x = (int)((float)x * Constants.S);
		y = (int)((float)y * Constants.S);
		if ((mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom || HasGlove()) && CanInteractWithBoardButtons())
		{
			for (int i = 6; i <= 16; i++)
			{
				GameObjectType gameObjectType = (GameObjectType)i;
				if (CanUseGameObject(gameObjectType) && (gameObjectType != GameObjectType.TreeFood || mChallenge.TreeOfWisdomCanFeed()) && (gameObjectType != GameObjectType.MoneySign || mApp.mPlayerInfo.mZenGardenTutorialComplete) && GetZenButtonRect(gameObjectType).Contains(x, y))
				{
					theHitResult.mObjectType = gameObjectType;
					return true;
				}
			}
		}
		if (mApp.IsSlotMachineLevel() && mCursorObject.mCursorType != CursorType.Shovel)
		{
			TRect tRect = mChallenge.SlotMachineGetHandleRect();
			TRect tRect2 = mChallenge.SlotMachineRect();
			if (!mApp.mPlayerInfo.mClassicSeedChooser)
			{
				int num4 = (int)(Constants.S * 50f);
				tRect.mX -= num4;
				tRect.mWidth += num4 * 2;
				tRect.mY -= num4;
				tRect.mHeight += num4 * 2;
			}
			if ((tRect.Contains(x, y) || (!mApp.mPlayerInfo.mClassicSeedChooser && tRect2.Contains(x, y))) && mChallenge.mChallengeState == ChallengeState.Normal && !HasLevelAwardDropped())
			{
				theHitResult.mObjectType = GameObjectType.SlotMachineHandle;
				return true;
			}
		}
		if (mCameraEnabled)
		{
			int num5 = screenX;
			y = screenY;
			x = num5;
		}
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		if (MouseHitTestPlant(x, y, out theHitResult, posScaled: true))
		{
			return true;
		}
		x = (int)((float)x * Constants.S);
		y = (int)((float)y * Constants.S);
		if (mApp.IsScaryPotterLevel() && mChallenge.mChallengeState != ChallengeState.ScaryPotterMalleting && mApp.mGameScene == GameScenes.Playing && mApp.GetDialog(Dialogs.DIALOG_GAME_OVER) == null && mApp.GetDialog(Dialogs.DIALOG_CONTINUE) == null)
		{
			int theGridX = PixelToGridX((int)((float)x * Constants.IS), (int)((float)y * Constants.IS));
			int theGridY = PixelToGridY((int)((float)x * Constants.IS), (int)((float)y * Constants.IS));
			GridItem scaryPotAt = GetScaryPotAt(theGridX, theGridY);
			if (scaryPotAt != null)
			{
				theHitResult.mObject = scaryPotAt;
				theHitResult.mObjectType = GameObjectType.ScaryPot;
				return true;
			}
		}
		if (mApp.IsSlotMachineLevel() && mCursorObject.mCursorType == CursorType.Shovel)
		{
			TRect tRect3 = mChallenge.SlotMachineGetHandleRect();
			TRect tRect4 = mChallenge.SlotMachineRect();
			if (!mApp.mPlayerInfo.mClassicSeedChooser)
			{
				int num6 = (int)(Constants.S * 50f);
				tRect3.mX -= num6;
				tRect3.mWidth += num6 * 2;
				tRect3.mY -= num6;
				tRect3.mHeight += num6 * 2;
			}
			if ((tRect3.Contains(x, y) || (!mApp.mPlayerInfo.mClassicSeedChooser && tRect4.Contains(x, y))) && mChallenge.mChallengeState == ChallengeState.Normal && !HasLevelAwardDropped())
			{
				theHitResult.mObjectType = GameObjectType.SlotMachineHandle;
				return true;
			}
		}
		theHitResult.mObject = null;
		theHitResult.mObjectType = GameObjectType.None;
		return false;
	}

	public void MouseDownWithPlant(int x, int y, int theClickCount)
	{
		if (theClickCount < 0)
		{
			RefreshSeedPacketFromCursor();
			mApp.PlayFoley(FoleyType.Drop);
		}
	}

	private bool TryDiscardHeldObject()
	{
		switch (mCursorObject.mCursorType)
		{
		case CursorType.PlantFromGlove:
		{
			Plant mGlovePlantID = mCursorObject.mGlovePlantID;
			if (mGlovePlantID == null)
			{
				return false;
			}
			mCursorObject.mGlovePlantID = null;
			mGlovePlantID.mGloveGrabbed = false;
			mGlovePlantID.Die();
			break;
		}
		case CursorType.PlantFromUsableCoin:
		{
			Coin mCoinID = mCursorObject.mCoinID;
			if (mCoinID == null)
			{
				return false;
			}
			mCursorObject.mCoinID = null;
			mCoinID.Die();
			break;
		}
		case CursorType.PlantFromBank:
		{
			int mSeedBankIndex = mCursorObject.mSeedBankIndex;
			if (mSeedBankIndex < 0 || mSeedBankIndex >= mSeedBank.mNumPackets)
			{
				return false;
			}
			mSeedBank.mSeedPackets[mSeedBankIndex].Discard();
			break;
		}
		default:
			return false;
		}
		ClearCursor();
		return true;
	}

	public void MouseUpWithPlant(int x, int y, int theClickCount)
	{
		bool flag = mCursorObject.mCursorType == CursorType.PlantFromGlove;
		bool flag2 = flag;
		if (flag2)
		{
			GameMode mGameMode = mApp.mGameMode;
			bool flag3 = ((mGameMode == GameMode.ChallengeZenGarden || mGameMode == GameMode.TreeOfWisdom) ? true : false);
			flag2 = !flag3;
		}
		if (flag2)
		{
			if (!HasGlove())
			{
				ClearCursor();
				return;
			}
			if (!CanUseCombatTool(GameObjectType.Glove))
			{
				return;
			}
		}
		if (theClickCount < 0)
		{
			RefreshSeedPacketFromCursor();
			mApp.PlayFoley(FoleyType.Drop);
			return;
		}
		if (TrashcanHitTest(x, y))
		{
			if (mApp.mGameScene == GameScenes.Playing && CanInteractWithBoardButtons())
			{
				TryDiscardHeldObject();
			}
			return;
		}
		if (mApp.IsIZombieLevel())
		{
			mChallenge.IZombieMouseDownWithZombie(x, y, theClickCount);
			return;
		}
		SeedType seedTypeInCursor = GetSeedTypeInCursor();
		if (Challenge.IsZombieSeedType(seedTypeInCursor))
		{
			mChallenge.IZombieMouseDownWithZombie(x, y, theClickCount);
			return;
		}
		SeedType seedType = SeedType.None;
		int num = PlantingPixelToGridX((int)((float)x * Constants.IS), (int)((float)y * Constants.IS), seedTypeInCursor);
		int num2 = PlantingPixelToGridY((int)((float)x * Constants.IS), (int)((float)y * Constants.IS), seedTypeInCursor);
		if (num < 0 || num >= Constants.GRIDSIZEX || num2 < 0 || num2 >= Constants.MAX_GRIDSIZEY)
		{
			RefreshSeedPacketFromCursor();
			mApp.PlayFoley(FoleyType.Drop);
			return;
		}
		if (mCursorObject.mCursorType == CursorType.PlantFromGlove && mCursorObject.mGlovePlantID.mDead)
		{
			RefreshSeedPacketFromCursor();
			mApp.PlayFoley(FoleyType.Drop);
			return;
		}
		PlantingReason plantingReason = CanPlantAt(num, num2, seedTypeInCursor, mCursorObject.mCursorType == CursorType.PlantFromGlove);
		if (plantingReason != PlantingReason.Ok)
		{
			if (plantingReason == PlantingReason.OnlyOnGraves)
			{
				DisplayAdvice("[ADVICE_GRAVEBUSTERS_ON_GRAVES]", MessageStyle.HintFast, AdviceType.PlantGravebustersOnGraves);
			}
			else if (seedTypeInCursor == SeedType.Lilypad && plantingReason == PlantingReason.OnlyInPool)
			{
				DisplayAdvice("[ADVICE_LILYPAD_ON_WATER]", MessageStyle.HintFast, AdviceType.PlantLilypadOnWater);
			}
			else if (seedTypeInCursor == SeedType.Tanglekelp && plantingReason == PlantingReason.OnlyInPool)
			{
				DisplayAdvice("[ADVICE_TANGLEKELP_ON_WATER]", MessageStyle.HintFast, AdviceType.PlantTanglekelpOnWater);
			}
			else if (seedTypeInCursor == SeedType.Seashroom && plantingReason == PlantingReason.OnlyInPool)
			{
				DisplayAdvice("[ADVICE_SEASHROOM_ON_WATER]", MessageStyle.HintFast, AdviceType.PlantSeashroomOnWater);
			}
			else if (plantingReason == PlantingReason.OnlyOnGround && seedTypeInCursor == SeedType.Potatomine)
			{
				DisplayAdvice("[ADVICE_POTATO_MINE_ON_LILY]", MessageStyle.HintFast, AdviceType.PlantPotatoMineOnLily);
			}
			else if (plantingReason == PlantingReason.OnlyOnGround && seedTypeInCursor == SeedType.Endoflame)
			{
				DisplayAdvice("[ADVICE_ENDOFLAME_ON_LILY]", MessageStyle.HintFast, AdviceType.PlantPotatoMineOnLily);
			}
			else if (plantingReason == PlantingReason.NotPassedLine)
			{
				DisplayAdvice("[ADVICE_NOT_PASSED_LINE]", MessageStyle.HintFast, AdviceType.PlantNotPassedLine);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Gatlingpea)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_REPEATERS]", MessageStyle.HintFast, AdviceType.PlantOnlyOnRepeaters);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Wintermelon)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_MELONPULT]", MessageStyle.HintFast, AdviceType.PlantOnlyOnMelonpult);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Twinsunflower)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_SUNFLOWER]", MessageStyle.HintFast, AdviceType.PlantOnlyOnSunflower);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Spikerock)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_SPIKEWEED]", MessageStyle.HintFast, AdviceType.PlantOnlyOnSpikeweed);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Cobcannon)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_KERNELPULT]", MessageStyle.HintFast, AdviceType.PlantOnlyOnKernelpult);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.GoldMagnet)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_MAGNETSHROOM]", MessageStyle.HintFast, AdviceType.PlantOnlyOnMagnetshroom);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Gloomshroom)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_FUMESHROOM]", MessageStyle.HintFast, AdviceType.PlantOnlyOnFumeshroom);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.Cattail)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_LILYPAD]", MessageStyle.HintFast, AdviceType.PlantOnlyOnLilypad);
			}
			else if (plantingReason == PlantingReason.NeedsUpgrade && seedTypeInCursor == SeedType.SuperChomper)
			{
				DisplayAdvice("[ADVICE_ONLY_ON_CHOMPER]", MessageStyle.HintFast, AdviceType.PlantOnlyOnChomper);
			}
			else
			{
				switch (plantingReason)
				{
				case PlantingReason.NotOnArt:
				{
					SeedType artChallengeSeed = mChallenge.GetArtChallengeSeed(num, num2);
					string nameString = Plant.GetNameString(artChallengeSeed, SeedType.None);
					string theAdvice = TodCommon.TodReplaceString("[ADVICE_WRONG_ART_TYPE]", "{SEED}", nameString);
					DisplayAdvice(theAdvice, MessageStyle.HintFast, AdviceType.PlantWrongArtType);
					break;
				}
				case PlantingReason.NeedsPot:
					if (mApp.IsFirstTimeAdventureMode() && mLevel == 41)
					{
						DisplayAdvice("[ADVICE_PLANT_NEED_POT1]", MessageStyle.HintFast, AdviceType.PlantNeedPot);
					}
					else
					{
						DisplayAdvice("[ADVICE_PLANT_NEED_POT2]", MessageStyle.HintFast, AdviceType.PlantNeedPot);
					}
					break;
				case PlantingReason.NotOnGrave:
					DisplayAdvice("[ADVICE_PLANT_NOT_ON_GRAVE]", MessageStyle.HintFast, AdviceType.PlantNotOnGrave);
					break;
				case PlantingReason.NotOnCrater:
					if (IsPoolSquare(num, num2))
					{
						DisplayAdvice("[ADVICE_CANT_PLANT_THERE]", MessageStyle.HintFast, AdviceType.CantPlantThere);
					}
					else
					{
						DisplayAdvice("[ADVICE_PLANT_NOT_ON_CRATER]", MessageStyle.HintFast, AdviceType.PlantNotOnCrater);
					}
					break;
				case PlantingReason.NotOnWater:
					if (mApp.mGameMode == GameMode.ChallengeZenGarden && mApp.mZenGarden.mGardenType == GardenType.Aquarium)
					{
						DisplayAdvice("[ZEN_ONLY_AQUATIC_PLANTS]", MessageStyle.HintTallFast, AdviceType.None);
					}
					else if (seedTypeInCursor == SeedType.Potatomine)
					{
						DisplayAdvice("[ADVICE_CANT_PLANT_THERE]", MessageStyle.HintFast, AdviceType.CantPlantThere);
					}
					else
					{
						DisplayAdvice("[ADVICE_PLANT_NOT_ON_WATER]", MessageStyle.HintFast, AdviceType.PlantNotOnWater);
					}
					break;
				case PlantingReason.NeedsGround:
					DisplayAdvice("[ADVICE_PLANTING_NEEDS_GROUND]", MessageStyle.HintFast, AdviceType.PlantingNeedsGround);
					break;
				case PlantingReason.NeedsSleeping:
					DisplayAdvice("[ADVICE_PLANTING_NEED_SLEEPING]", MessageStyle.HintFast, AdviceType.PlantingNeedSleeping);
					break;
				}
			}
			if (mCursorObject.mCursorType == CursorType.PlantFromGlove || mApp.IsWhackAZombieLevel())
			{
				RefreshSeedPacketFromCursor();
				mApp.PlayFoley(FoleyType.Drop);
			}
			return;
		}
		ClearAdvice(AdviceType.PlantingNeedSleeping);
		ClearAdvice(AdviceType.CantPlantThere);
		ClearAdvice(AdviceType.PlantingNeedsGround);
		ClearAdvice(AdviceType.PlantNotOnWater);
		ClearAdvice(AdviceType.PlantNotOnCrater);
		ClearAdvice(AdviceType.PlantNotOnGrave);
		ClearAdvice(AdviceType.PlantNeedPot);
		ClearAdvice(AdviceType.PlantWrongArtType);
		ClearAdvice(AdviceType.PlantOnlyOnLilypad);
		ClearAdvice(AdviceType.PlantOnlyOnMagnetshroom);
		ClearAdvice(AdviceType.PlantOnlyOnFumeshroom);
		ClearAdvice(AdviceType.PlantOnlyOnKernelpult);
		ClearAdvice(AdviceType.PlantOnlyOnSunflower);
		ClearAdvice(AdviceType.PlantOnlyOnSpikeweed);
		ClearAdvice(AdviceType.PlantOnlyOnMelonpult);
		ClearAdvice(AdviceType.PlantOnlyOnRepeaters);
		ClearAdvice(AdviceType.PlantOnlyOnChomper);
		ClearAdvice(AdviceType.PlantNotPassedLine);
		ClearAdvice(AdviceType.PlantGravebustersOnGraves);
		ClearAdvice(AdviceType.PlantLilypadOnWater);
		ClearAdvice(AdviceType.PlantTanglekelpOnWater);
		ClearAdvice(AdviceType.PlantSeashroomOnWater);
		ClearAdvice(AdviceType.PlantPotatoMineOnLily);
		ClearAdvice(AdviceType.SurviveFlags);
		if (!mApp.mEasyPlantingCheat && mCursorObject.mCursorType == CursorType.PlantFromBank && !HasConveyorBeltSeedBank())
		{
			int currentPlantCost = GetCurrentPlantCost(mCursorObject.mType, mCursorObject.mImitaterType);
			if (!TakeSunMoney(currentPlantCost))
			{
				return;
			}
		}
		Plant topPlantAt = GetTopPlantAt(num, num2, TopPlant.OnlyNormalPosition);
		bool flag4 = false;
		int mWakeUpCounter = 0;
		if (topPlantAt != null && topPlantAt.IsUpgradableTo(seedTypeInCursor))
		{
			if (seedTypeInCursor == SeedType.Gloomshroom)
			{
				if (!topPlantAt.mIsAsleep)
				{
					flag4 = true;
				}
				else
				{
					mWakeUpCounter = topPlantAt.mWakeUpCounter;
				}
			}
			seedType = Plant.GetValidFusion(topPlantAt.mSeedType, seedTypeInCursor);
			topPlantAt.Die();
		}
		Plant plant = null;
		Plant plant2 = null;
		bool flag5 = false;
		if ((seedTypeInCursor == SeedType.Wallnut || seedTypeInCursor == SeedType.Tallnut || seedTypeInCursor == SeedType.FireShroom || seedTypeInCursor == SeedType.Garlic) && topPlantAt != null && topPlantAt.mSeedType == seedTypeInCursor)
		{
			topPlantAt.Die();
			plant = topPlantAt;
		}
		if (seedTypeInCursor == SeedType.Pumpkinshell)
		{
			Plant topPlantAt2 = GetTopPlantAt(num, num2, TopPlant.OnlyPumpkin);
			if (topPlantAt2 != null && topPlantAt2.mSeedType == seedTypeInCursor)
			{
				topPlantAt2.Die();
				plant = topPlantAt2;
			}
		}
		if (seedTypeInCursor == SeedType.Cobcannon)
		{
			GetTopPlantAt(num + 1, num2, TopPlant.OnlyNormalPosition)?.Die();
		}
		if (seedTypeInCursor == SeedType.Cattail)
		{
			PlantsOnLawn thePlantOnLawn = default;
			GetPlantsOnLawn(num, num2, ref thePlantOnLawn);
			if (thePlantOnLawn.mUnderPlant != null)
			{
				thePlantOnLawn.mUnderPlant.Die();
			}
			if (thePlantOnLawn.mNormalPlant != null)
			{
				thePlantOnLawn.mNormalPlant.Die();
			}
		}
		if (mCursorObject.mCursorType == CursorType.PlantFromGlove)
		{
			Plant mGlovePlantID = mCursorObject.mGlovePlantID;
			bool flag6 = num != mGlovePlantID.mPlantCol || num2 != mGlovePlantID.mRow;
			if (seedType != SeedType.None)
			{
				Plant mGlovePlantID2 = mCursorObject.mGlovePlantID;
				if (mGlovePlantID2 != null && mGlovePlantID2.mSeedType == SeedType.Imitater)
				{
					mCursorObject.mImitaterType = seedType;
					mCursorObject.mType = SeedType.Imitater;
				}
				else
				{
					mCursorObject.mType = seedType;
					mCursorObject.mImitaterType = SeedType.None;
				}
				DoFusionEvent(num, num2, mCursorObject.mType, mCursorObject.mImitaterType);
				Plant plant3 = AddPlant(num, num2, mCursorObject.mType, mCursorObject.mImitaterType);
				if (flag4)
				{
					plant3.SetSleeping(theIsAsleep: false);
				}
				plant3.mWakeUpCounter = mWakeUpCounter;
				plant2 = plant3;
				flag5 = true;
				mCursorObject.mGlovePlantID.Die();
				mCursorObject.mGlovePlantID = null;
			}
			else
			{
				mApp.mZenGarden.MovePlant(mGlovePlantID, num, num2);
				mChallenge.MovePlant(mGlovePlantID, num, num2);
				mGlovePlantID.mGloveGrabbed = false;
				plant2 = mGlovePlantID;
				flag5 = true;
			}
			bool flag7 = flag6 && HasGlove();
			bool flag8 = flag7;
			if (flag8)
			{
				GameMode mGameMode = mApp.mGameMode;
				bool flag3 = ((mGameMode == GameMode.ChallengeZenGarden || mGameMode == GameMode.TreeOfWisdom) ? true : false);
				flag8 = !flag3;
			}
			if (flag8)
			{
				mChallenge.mGloveCounter = mChallenge.GetGloveCounterMax();
			}
		}
		else if (mCursorObject.mCursorType == CursorType.PlantFromWheelBarrow)
		{
			mApp.mZenGarden.MouseDownWithFullWheelBarrow(x, y);
		}
		else if (mCursorObject.mCursorType == CursorType.PlantFromUsableCoin)
		{
			if (seedType != SeedType.None)
			{
				if (mCursorObject.mType == SeedType.Imitater)
				{
					mCursorObject.mImitaterType = seedType;
				}
				else
				{
					mCursorObject.mType = seedType;
				}
				DoFusionEvent(num, num2, mCursorObject.mType, mCursorObject.mImitaterType);
			}
			plant2 = AddPlant(num, num2, mCursorObject.mType, mCursorObject.mImitaterType);
			Coin coin = mCoins[mCoins.IndexOf(mCursorObject.mCoinID)];
			mCursorObject.mCoinID = null;
			coin.Die();
		}
		else if (mCursorObject.mCursorType == CursorType.PlantFromBank)
		{
			if (seedType != SeedType.None)
			{
				if (mCursorObject.mType == SeedType.Imitater)
				{
					mCursorObject.mImitaterType = seedType;
				}
				else
				{
					mCursorObject.mType = seedType;
				}
				DoFusionEvent(num, num2, mCursorObject.mType, mCursorObject.mImitaterType);
			}
			Plant plant4 = AddPlant(num, num2, mCursorObject.mType, mCursorObject.mImitaterType);
			if (flag4)
			{
				plant4.SetSleeping(theIsAsleep: false);
			}
			plant4.mWakeUpCounter = mWakeUpCounter;
			Debug.ASSERT(mCursorObject.mSeedBankIndex >= 0 && mCursorObject.mSeedBankIndex < mSeedBank.mNumPackets);
			SeedPacket seedPacket = mSeedBank.mSeedPackets[mCursorObject.mSeedBankIndex];
			seedPacket.WasPlanted();
			plant2 = plant4;
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		if (plant != null && plant2 != null)
		{
			DoMending(plant, plant2, flag5);
		}
		else if (plant2 != null && !plant2.mIsAsleep && !flag5)
		{
			DoPlantingSound(plant2);
		}
		plant = null;
		if (mApp.IsColumnLevel() || mApp.mCreativeLevel?.GetComponent<CSColumnPlanting>() != null)
		{
			for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
			{
				if (i == num2 || CanPlantAt(num, i, seedTypeInCursor) != PlantingReason.Ok)
				{
					continue;
				}
				if (seedTypeInCursor == SeedType.Wallnut || seedTypeInCursor == SeedType.Tallnut || seedTypeInCursor == SeedType.FireShroom || seedTypeInCursor == SeedType.Garlic)
				{
					Plant topPlantAt3 = GetTopPlantAt(num, i, TopPlant.OnlyNormalPosition);
					if (topPlantAt3 != null && topPlantAt3.mSeedType == seedTypeInCursor)
					{
						topPlantAt3.Die();
						plant = topPlantAt3;
					}
				}
				if (seedTypeInCursor == SeedType.Pumpkinshell)
				{
					Plant topPlantAt4 = GetTopPlantAt(num, i, TopPlant.OnlyPumpkin);
					if (topPlantAt4 != null && topPlantAt4.mSeedType == seedTypeInCursor)
					{
						topPlantAt4.Die();
						plant = topPlantAt4;
					}
				}
				if (Plant.IsUpgrade(seedTypeInCursor))
				{
					Plant topPlantAt5 = GetTopPlantAt(num, i, TopPlant.OnlyNormalPosition);
					if (topPlantAt5 != null && topPlantAt5.IsUpgradableTo(seedTypeInCursor))
					{
						topPlantAt5.Die();
						plant = topPlantAt5;
					}
				}
				AddPlant(num, i, mCursorObject.mType, mCursorObject.mImitaterType);
				if (plant != null && plant2 != null)
				{
					DoMending(plant, plant2);
				}
				else if (plant2 != null && !plant2.mIsAsleep)
				{
					DoPlantingSound(plant2);
				}
			}
		}
		if (mTutorialState == TutorialState.Level1PlantPeashooter)
		{
			int count = mPlants.Count;
			if (count >= 2)
			{
				SetTutorialState(TutorialState.Level1Completed);
			}
			else
			{
				SetTutorialState(TutorialState.Level1RefreshPeashooter);
			}
		}
		if (mTutorialState == TutorialState.Level2PlantSunflower)
		{
			int num3 = CountSunFlowers();
			if (seedTypeInCursor == SeedType.Sunflower && num3 == 2)
			{
				DisplayAdvice("[ADVICE_MORE_SUNFLOWERS]", MessageStyle.TutorialLevel2, AdviceType.None);
			}
			if (num3 >= 3)
			{
				SetTutorialState(TutorialState.Level2Completed);
			}
			else if (!mSeedBank.mSeedPackets[1].CanPickUp())
			{
				SetTutorialState(TutorialState.Level2RefreshSunflower);
			}
			else
			{
				SetTutorialState(TutorialState.Level2PickUpSunflower);
			}
		}
		if (mTutorialState == TutorialState.MoresunPlantSunflower)
		{
			int num4 = CountSunFlowers();
			if (num4 >= 3)
			{
				SetTutorialState(TutorialState.MoresunCompleted);
				DisplayAdvice("[ADVICE_PLANT_SUNFLOWER5]", MessageStyle.TutorialLater, AdviceType.PlantSunflower5);
				mTutorialTimer = -1;
			}
			else if (!mSeedBank.mSeedPackets[1].CanPickUp())
			{
				SetTutorialState(TutorialState.MoresunRefreshSunflower);
			}
			else
			{
				SetTutorialState(TutorialState.MoresunPickUpSunflower);
			}
		}
		if (mApp.IsWallnutBowlingLevel())
		{
			mApp.PlaySample(Resources.SOUND_BOWLING);
		}
		ClearCursor();
		void DoMending(Plant oldP, Plant newP, bool theMovePlant = false)
		{
			if (oldP.mSeedType == SeedType.Garlic && newP.mSeedType == SeedType.Garlic)
			{
				Reanimation reanimation = mApp.ReanimationTryToGet(newP.mBodyReanimID);
				if (reanimation != null)
				{
					float theRate = reanimation.mAnimRate;
					newP.PlayBodyReanim("anim_repair", ReanimLoopType.PlayThenAction, 0, 24f);
					PlantVoice.Play(SeedType.Garlic, SeedType.None, PlantVoice.VoiceType.Mending);
					reanimation.mAction = (Reanimation _) =>
					{
						newP.PlayIdleAnim(theRate);
						return true;
					};
				}
			}
			else if (oldP.mSeedType == SeedType.Tallnut && newP.mSeedType == SeedType.Tallnut)
			{
				Reanimation reanimation2 = mApp.ReanimationTryToGet(newP.mBodyReanimID);
				if (reanimation2 != null)
				{
					float theRate2 = reanimation2.mAnimRate;
					newP.PlayBodyReanim("anim_repair", ReanimLoopType.PlayThenAction, 0, 30f);
					PlantVoice.Play(SeedType.Tallnut, SeedType.None, PlantVoice.VoiceType.Mending);
					reanimation2.mAction = (Reanimation _) =>
					{
						newP.PlayIdleAnim(theRate2);
						newP.mState = PlantState.Notready;
						return true;
					};
				}
			}
			else if (oldP.mSeedType == SeedType.Pumpkinshell && newP.mSeedType == SeedType.Pumpkinshell)
			{
				DoPlantingSound(newP);
			}
		}
	}

	public void MouseDownWithTool(int x, int y, int theClickCount, CursorType theCursorType, bool posScaled, bool isTouch)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			if (posScaled)
			{
				x = (int)(Constants.S * (float)x);
				y = (int)(Constants.S * (float)y);
				posScaled = false;
			}
			if (y < Constants.ZEN_YMIN)
			{
				ClearCursor();
				return;
			}
		}
		if (!posScaled)
		{
			x = (int)((float)x * Constants.IS);
			y = (int)((float)y * Constants.IS);
		}
		if (theClickCount < 0)
		{
			ClearCursor();
			mApp.PlayFoley(FoleyType.Drop);
			return;
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			mApp.mZenGarden.MouseDownWithTool(x, y, theCursorType, isTouch);
			return;
		}
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			mChallenge.TreeOfWisdomTool(x, y);
			return;
		}
		GameObjectType gameObjectType = theCursorType switch
		{
			CursorType.Glove => GameObjectType.Glove, 
			CursorType.Shovel => GameObjectType.Shovel, 
			_ => GameObjectType.None, 
		};
		if ((gameObjectType == GameObjectType.Shovel && !HasShovel()) || (gameObjectType == GameObjectType.Glove && !HasGlove()))
		{
			ClearCursor();
		}
		else
		{
			if (!CanUseCombatTool(gameObjectType) || (gameObjectType == GameObjectType.Glove && mChallenge.MouseDownWithZenTool(x, y, theCursorType, isTouch)))
			{
				return;
			}
			HitResult hitResult = ToolHitTest(x, y, posScaled: true, isTouch);
			Plant plant = null;
			if (hitResult.mObjectType == GameObjectType.Plant)
			{
				plant = (Plant)hitResult.mObject;
			}
			if (plant != null)
			{
				if (theCursorType == CursorType.Shovel)
				{
					mApp.PlayFoley(FoleyType.UseShovel);
					mPlantsShoveled++;
					plant.Die();
					if (plant.mSeedType == SeedType.Cattail && GetTopPlantAt(plant.mPlantCol, plant.mRow, TopPlant.OnlyPumpkin) != null)
					{
						NewPlant(plant.mPlantCol, plant.mRow, SeedType.Lilypad, SeedType.None);
					}
					if (mTutorialState == TutorialState.ShovelDig || mTutorialState == TutorialState.ShovelKeepDigging)
					{
						if (CountPlantByType(SeedType.Peashooter) == 0)
						{
							SetTutorialState(TutorialState.ShovelCompleted);
						}
						else
						{
							SetTutorialState(TutorialState.ShovelKeepDigging);
						}
					}
				}
				ClearCursor();
			}
			else
			{
				MouseHitTest(x, y, out var theHitResult, posScaled: true, isTouch);
				if (theHitResult.mObjectType != GameObjectType.Coin)
				{
					mApp.PlayFoley(FoleyType.Drop);
					ClearCursor();
				}
			}
		}
	}

	public bool CanInteractWithBoardButtons()
	{
		if (!mPaused && mApp.GetDialogCount() <= 0 && mBoardFadeOutCounter < 0 && mChallenge.mChallengeState != ChallengeState.ZenFading && mChallenge.mChallengeState != ChallengeState.ZenPlantInteracting)
		{
			if (mApp.mGameMode != GameMode.Upsell)
			{
				return mApp.mCrazyDaveState == CrazyDaveState.Off;
			}
			return true;
		}
		return false;
	}

	public bool CanCancelAgaveSkill()
	{
		Plant plant = null;
		if (mCursorObject.mCobCannonPlantID != null && mPlants.IndexOf(mCursorObject.mCobCannonPlantID) >= 0)
		{
			plant = mCursorObject.mCobCannonPlantID;
		}
		if (plant != null && !plant.NotOnGround() && plant.mSeedType == SeedType.Agave)
		{
			return true;
		}
		return false;
	}

	public bool CanCancelEndoflameSkill()
	{
		Plant plant = null;
		if (mCursorObject.mCobCannonPlantID != null && mPlants.IndexOf(mCursorObject.mCobCannonPlantID) >= 0)
		{
			plant = mCursorObject.mCobCannonPlantID;
		}
		if (plant != null && !plant.NotOnGround() && plant.mSeedType == SeedType.Endoflame)
		{
			return true;
		}
		return false;
	}

	public void DrawAgaveInfo(Graphics g)
	{
		if (mApp.mGameMode != GameMode.ChallengeZenGarden && mAgaveSkillDrawType != 0)
		{
			TRect agaveSkillRect = GetAgaveSkillRect();
			if (CanCancelAgaveSkill())
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(128, 128, 128));
				g.DrawImage(AtlasResources.IMAGE_AGAVE_SKILL, agaveSkillRect.mX, agaveSkillRect.mY, agaveSkillRect.mWidth, agaveSkillRect.mHeight);
				g.SetColorizeImages(colorizeImages: false);
				g.DrawImage(AtlasResources.IMAGE_AGAVE_SKILL_CANCEL, agaveSkillRect.mX, agaveSkillRect.mY, agaveSkillRect.mWidth, agaveSkillRect.mHeight);
			}
			else if (mAgavePowerfulCountdown > 0)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(128, 128, 128));
				g.DrawImage(AtlasResources.IMAGE_AGAVE_SKILL, agaveSkillRect.mX, agaveSkillRect.mY, agaveSkillRect.mWidth, agaveSkillRect.mHeight);
				g.SetColorizeImages(colorizeImages: false);
				int num = 4;
				int theHeight = TodCommon.FloatRoundToInt((float)(agaveSkillRect.mHeight - num - num) * Math.Clamp((float)mAgavePowerfulCountdown / 6000f, 0f, 1f));
				g.SetColor(new SexyColor(0, 0, 0, 150));
				g.FillRect(agaveSkillRect.mX + num, agaveSkillRect.mY + num, agaveSkillRect.mWidth - num - num, theHeight);
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_AGAVE_SKILL, agaveSkillRect.mX, agaveSkillRect.mY, agaveSkillRect.mWidth, agaveSkillRect.mHeight);
			}
		}
	}

	public void DrawEndoflameInfo(Graphics g)
	{
		if (mApp.mGameMode != GameMode.ChallengeZenGarden && mEndoflameSkillDrawType != 0)
		{
			TRect endoflameSkillRect = GetEndoflameSkillRect();
			if (CanCancelEndoflameSkill())
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(128, 128, 128));
				g.DrawImage(AtlasResources.IMAGE_ENDOFLAME_SKILL, endoflameSkillRect.mX, endoflameSkillRect.mY, endoflameSkillRect.mWidth, endoflameSkillRect.mHeight);
				g.SetColorizeImages(colorizeImages: false);
				g.DrawImage(AtlasResources.IMAGE_AGAVE_SKILL_CANCEL, endoflameSkillRect.mX, endoflameSkillRect.mY, endoflameSkillRect.mWidth, endoflameSkillRect.mHeight);
			}
			else if (mEndoflamePowerfulCountdown > 0)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(128, 128, 128));
				g.DrawImage(AtlasResources.IMAGE_ENDOFLAME_SKILL, endoflameSkillRect.mX, endoflameSkillRect.mY, endoflameSkillRect.mWidth, endoflameSkillRect.mHeight);
				g.SetColorizeImages(colorizeImages: false);
				int num = 4;
				int theHeight = TodCommon.FloatRoundToInt((float)(endoflameSkillRect.mHeight - num - num) * Math.Clamp((float)mEndoflamePowerfulCountdown / 3000f, 0f, 1f));
				g.SetColor(new SexyColor(0, 0, 0, 150));
				g.FillRect(endoflameSkillRect.mX + num, endoflameSkillRect.mY + num, endoflameSkillRect.mWidth - num - num, theHeight);
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_ENDOFLAME_SKILL, endoflameSkillRect.mX, endoflameSkillRect.mY, endoflameSkillRect.mWidth, endoflameSkillRect.mHeight);
			}
		}
	}

	public void DrawProgressMeter(Graphics g)
	{
		if (!HasProgressMeter())
		{
			return;
		}
		int celWidth = AtlasResources.IMAGE_FLAGMETER.GetCelWidth();
		int celHeight = AtlasResources.IMAGE_FLAGMETER.GetCelHeight();
		int num = (mApp.mPlayerInfo.mClassicSeedChooser ? (700 - celWidth) : (Constants.UIProgressMeterPosition.X - Constants.Board_Offset_AspectRatio_Correction));
		int num2 = (mApp.mPlayerInfo.mClassicSeedChooser ? (Constants.BOARD_HEIGHT - celHeight) : Constants.UIProgressMeterPosition.Y);
		g.DrawImageCel(AtlasResources.IMAGE_FLAGMETER, num, num2, 0);
		int thePosX = num + celWidth / 2;
		int thePosY = (mApp.mPlayerInfo.mClassicSeedChooser ? (num2 + Constants.Board_ProgressBarText_Pos - Constants.UIProgressMeterPosition.Y) : Constants.Board_ProgressBarText_Pos);
		int num3 = TodCommon.TodAnimateCurve(0, 150, mProgressMeterWidth, 0, Constants.UIProgressMeterBarEnd, TodCurves.Linear);
		g.DrawImage(theSrcRect: new TRect(celWidth - num3 - 7, celHeight, num3, celHeight), theDestRect: new TRect(num + celWidth - num3 - 7, num2, num3, celHeight), theImage: AtlasResources.IMAGE_FLAGMETER);
		SexyColor theColor = new SexyColor(224, 187, 98);
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			if (progressMeterStringValue != mChallenge.mChallengeScore)
			{
				progressMeterString = Common.StrFormat_(TodStringFile.TodStringTranslate("[PROGRESS_METER_MATCHES]"), mChallenge.mChallengeScore, 75, TodStringFile.TodStringTranslate("[MATCHES]"));
				progressMeterStringValue = mChallenge.mChallengeScore;
			}
			TodCommon.TodDrawString(g, progressMeterString, thePosX, thePosY, Resources.FONT_DWARVENTODCRAFT12, theColor, DrawStringJustification.Center);
		}
		else if (mApp.IsSquirrelLevel())
		{
			string theText = Common.StrFormat_(TodStringFile.TodStringTranslate("[PROGRESS_METER_SQUIRRELS]"), mChallenge.mChallengeScore, 7, TodStringFile.TodStringTranslate("[SQUIRRELS]"));
			TodCommon.TodDrawString(g, theText, thePosX, thePosY, Resources.FONT_DWARVENTODCRAFT12, theColor, DrawStringJustification.Center);
		}
		else if (mApp.mGameMode == GameMode.ChallengeSlotMachine)
		{
			int num4 = TodCommon.ClampInt(mSunMoney, 0, 2000);
			if (progressMeterStringValue != num4)
			{
				progressMeterString = Common.StrFormat_(TodStringFile.TodStringTranslate("[PROGRESS_METER_SUN_SLOT_MACHINE]"), num4, 2000, TodStringFile.TodStringTranslate("[SUN]"));
				progressMeterStringValue = num4;
			}
			TodCommon.TodDrawString(g, progressMeterString, thePosX, thePosY, Resources.FONT_DWARVENTODCRAFT12, theColor, celWidth - 10, DrawStringJustification.Center);
		}
		else if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			int num5 = TodCommon.ClampInt(mSunMoney, 0, 1000);
			if (progressMeterStringValue != num5)
			{
				progressMeterString = Common.StrFormat_(TodStringFile.TodStringTranslate("[PROGRESS_METER_SUN_ZOMBIQUARIUM]"), num5, 1000, TodStringFile.TodStringTranslate("[SUN]"));
				progressMeterStringValue = num5;
			}
			TodCommon.TodDrawString(g, progressMeterString, thePosX, thePosY, Resources.FONT_DWARVENTODCRAFT12, theColor, DrawStringJustification.Center);
		}
		else if (mApp.IsIZombieLevel())
		{
			if (progressMeterStringValue != mChallenge.mChallengeScore)
			{
				progressMeterString = Common.StrFormat_(TodStringFile.TodStringTranslate("[PROGRESS_METER_BRAINS]"), mChallenge.mChallengeScore, 5, TodStringFile.TodStringTranslate("[BRAINS]"));
				progressMeterStringValue = mChallenge.mChallengeScore;
			}
			Resources.FONT_DWARVENTODCRAFT12.characterOffsetMagic = 2;
			TodCommon.TodDrawString(g, progressMeterString, thePosX, thePosY, Resources.FONT_DWARVENTODCRAFT12, theColor, DrawStringJustification.Center);
			Resources.FONT_DWARVENTODCRAFT12.characterOffsetMagic = 0;
		}
		else if (ProgressMeterHasFlags())
		{
			int numWavesPerFlag = GetNumWavesPerFlag();
			for (int i = 1; i <= mNumWaves / numWavesPerFlag; i++)
			{
				int theTimeAge = i * numWavesPerFlag;
				int num6 = 0;
				int thePositionEnd = num + 6;
				int thePositionStart = num + celWidth - 10;
				int theX = TodCommon.TodAnimateCurve(0, mNumWaves, theTimeAge, thePositionStart, thePositionEnd, TodCurves.Linear);
				g.DrawImageCel(AtlasResources.IMAGE_FLAGMETERPARTS, theX, num2 - 4, 1, 0);
				g.DrawImageCel(AtlasResources.IMAGE_FLAGMETERPARTS, theX, num2 - num6 - 3, 2, 0);
			}
		}
		if (mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist && mApp.mGameMode != GameMode.ChallengeZombiquarium && !mApp.IsSquirrelLevel() && !mApp.IsSlotMachineLevel() && !mApp.IsIZombieLevel() && !mApp.IsFinalBossLevel())
		{
			int num7 = TodCommon.TodAnimateCurve(0, 150, mProgressMeterWidth, 0, Constants.UIProgressMeterHeadEnd, TodCurves.Linear);
			g.DrawImageCel(AtlasResources.IMAGE_FLAGMETERPARTS, num + celWidth - num7 - 20, num2 - 3, 0, 0);
		}
	}

	public Plant GetTopPlantAt(int theGridX, int theGridY, TopPlant thePriority)
	{
		if (theGridX < 0 || theGridX >= Constants.GRIDSIZEX || theGridY < 0 || theGridY >= Constants.MAX_GRIDSIZEY)
		{
			return null;
		}
		if (mApp.IsWallnutBowlingLevel() && !mCutScene.IsInShovelTutorial())
		{
			return null;
		}
		PlantsOnLawn thePlantOnLawn = default;
		GetPlantsOnLawn(theGridX, theGridY, ref thePlantOnLawn);
		switch (thePriority)
		{
		case TopPlant.OnlyFlying:
			return thePlantOnLawn.mFlyingPlant;
		case TopPlant.OnlyUnderPlant:
			return thePlantOnLawn.mUnderPlant;
		case TopPlant.OnlyPumpkin:
			return thePlantOnLawn.mPumpkinPlant;
		case TopPlant.OnlyNormalPosition:
			return thePlantOnLawn.mNormalPlant;
		case TopPlant.EatingOrder:
			if (thePlantOnLawn.mPumpkinPlant != null)
			{
				return thePlantOnLawn.mPumpkinPlant;
			}
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return thePlantOnLawn.mNormalPlant;
			}
			if (thePlantOnLawn.mUnderPlant != null)
			{
				return thePlantOnLawn.mUnderPlant;
			}
			return null;
		case TopPlant.DiggingOrder:
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return thePlantOnLawn.mNormalPlant;
			}
			if (thePlantOnLawn.mUnderPlant != null)
			{
				return thePlantOnLawn.mUnderPlant;
			}
			return null;
		case TopPlant.BungeeOrder:
			if (thePlantOnLawn.mFlyingPlant != null)
			{
				return thePlantOnLawn.mFlyingPlant;
			}
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return thePlantOnLawn.mNormalPlant;
			}
			if (thePlantOnLawn.mPumpkinPlant != null)
			{
				return thePlantOnLawn.mPumpkinPlant;
			}
			if (thePlantOnLawn.mUnderPlant != null)
			{
				return thePlantOnLawn.mUnderPlant;
			}
			return null;
		case TopPlant.CatapultOrder:
		case TopPlant.Any:
			if (thePlantOnLawn.mFlyingPlant != null)
			{
				return thePlantOnLawn.mFlyingPlant;
			}
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return thePlantOnLawn.mNormalPlant;
			}
			if (thePlantOnLawn.mPumpkinPlant != null)
			{
				return thePlantOnLawn.mPumpkinPlant;
			}
			if (thePlantOnLawn.mUnderPlant != null)
			{
				return thePlantOnLawn.mUnderPlant;
			}
			return null;
		default:
			Debug.ASSERT(value: false);
			return null;
		case TopPlant.ZenToolOrder:
			if (thePlantOnLawn.mFlyingPlant != null)
			{
				return thePlantOnLawn.mFlyingPlant;
			}
			if (thePlantOnLawn.mPumpkinPlant != null)
			{
				return thePlantOnLawn.mPumpkinPlant;
			}
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return thePlantOnLawn.mNormalPlant;
			}
			if (thePlantOnLawn.mUnderPlant != null)
			{
				return thePlantOnLawn.mUnderPlant;
			}
			return null;
		}
	}

	public void GetPlantsOnLawn(int theGridX, int theGridY, ref PlantsOnLawn thePlantOnLawn)
	{
		thePlantOnLawn.mUnderPlant = null;
		thePlantOnLawn.mPumpkinPlant = null;
		thePlantOnLawn.mFlyingPlant = null;
		thePlantOnLawn.mNormalPlant = null;
		if (theGridX < 0 || theGridX >= Constants.GRIDSIZEX || theGridY < 0 || theGridY >= Constants.MAX_GRIDSIZEY || (mApp.IsWallnutBowlingLevel() && !mCutScene.IsInShovelTutorial()))
		{
			return;
		}
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (plant.mDead)
			{
				continue;
			}
			SeedType seedType = plant.mSeedType;
			if (plant.mSeedType == SeedType.Imitater && plant.mImitaterType != SeedType.None)
			{
				seedType = plant.mImitaterType;
			}
			if (seedType == SeedType.Cobcannon)
			{
				if (plant.mPlantCol < theGridX - 1 || plant.mPlantCol > theGridX || plant.mRow != theGridY)
				{
					continue;
				}
			}
			else if (plant.mPlantCol != theGridX || plant.mRow != theGridY)
			{
				continue;
			}
			if (plant.NotOnGround() && (!plant.mGloveGrabbed || mApp.mGameMode != GameMode.ChallengeZenGarden))
			{
				continue;
			}
			if (Plant.IsFlying(seedType))
			{
				Debug.ASSERT(thePlantOnLawn.mFlyingPlant == null);
				thePlantOnLawn.mFlyingPlant = plant;
				continue;
			}
			switch (seedType)
			{
			case SeedType.Flowerpot:
				Debug.ASSERT(thePlantOnLawn.mUnderPlant == null);
				thePlantOnLawn.mUnderPlant = plant;
				break;
			case SeedType.Lilypad:
				if (mApp.mGameMode == GameMode.ChallengeZenGarden)
				{
					Debug.ASSERT(thePlantOnLawn.mNormalPlant == null);
					thePlantOnLawn.mNormalPlant = plant;
				}
				else
				{
					Debug.ASSERT(thePlantOnLawn.mUnderPlant == null);
					thePlantOnLawn.mUnderPlant = plant;
				}
				break;
			case SeedType.Pumpkinshell:
				Debug.ASSERT(thePlantOnLawn.mPumpkinPlant == null);
				thePlantOnLawn.mPumpkinPlant = plant;
				break;
			default:
				Debug.ASSERT(thePlantOnLawn.mNormalPlant == null);
				thePlantOnLawn.mNormalPlant = plant;
				break;
			}
		}
	}

	public int CountSunFlowers()
	{
		int num = 0;
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.MakesSun())
			{
				num++;
			}
		}
		return num;
	}

	public int GetSeedPacketPositionY(int theIndex)
	{
		int mNumPackets = mSeedBank.mNumPackets;
		int num = Constants.SMALL_SEEDPACKET_HEIGHT;
		if (mNumPackets <= 7)
		{
			num += (int)Constants.InvertAndScale(8f);
		}
		else
		{
			switch (mNumPackets)
			{
			case 8:
				num += (int)Constants.InvertAndScale(5f);
				break;
			case 10:
				num += (int)Constants.InvertAndScale(-3f);
				break;
			}
		}
		if (!HasConveyorBeltSeedBank() && !Constants.Is500pMode)
		{
			num += 10;
		}
		return theIndex * num + ((!HasConveyorBeltSeedBank() && !Constants.Is500pMode) ? 20 : 0);
	}

	public int GetSeedPacketPositionX(int theIndex)
	{
		int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
		if (mApp.IsSlotMachineLevel())
		{
			return theIndex * (celWidth + 9) + 247;
		}
		if (HasConveyorBeltSeedBank())
		{
			return theIndex * celWidth + 91;
		}
		int mNumPackets = mSeedBank.mNumPackets;
		if (mNumPackets <= 7)
		{
			return theIndex * (celWidth + 9) + 85;
		}
		return mNumPackets switch
		{
			8 => theIndex * (celWidth + 4) + 81, 
			9 => theIndex * (celWidth + 2) + 80, 
			_ => theIndex * (celWidth + 1) + 79, 
		};
	}

	public int GetSeedBankExtraWidth()
	{
		int mNumPackets = mSeedBank.mNumPackets;
		if (mNumPackets > 6)
		{
			return mNumPackets switch
			{
				9 => 112, 
				8 => 76, 
				7 => 60, 
				_ => 153, 
			};
		}
		return 0;
	}

	public void AddGraveStones(int theGridX, int theCount)
	{
		if (!doAddGraveStones)
		{
			return;
		}
		Debug.ASSERT(theCount <= Constants.MAX_GRIDSIZEY);
		int num = 0;
		while (num < theCount)
		{
			int theGridY = RandomNumbers.NextNumber(Constants.MAX_GRIDSIZEY);
			if (CanAddGraveStoneAt(theGridX, theGridY))
			{
				AddAGraveStone(theGridX, theGridY);
				num++;
			}
		}
	}

	public int GetGraveStoneCount()
	{
		int num = 0;
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Gravestone)
			{
				num++;
			}
		}
		return num;
	}

	public void ZombiesWon(Zombie aZombie)
	{
		if (ZBCheatDialog.EnableNoZombieInHouse)
		{
			aZombie?.DieNoLoot(giveAchievements: false);
			return;
		}
		ZombiesWon_Orig(aZombie);
	}

	private void ZombiesWon_Orig(Zombie aZombie)
	{
		if (mApp.mGameScene == GameScenes.ZombiesWon)
		{
			return;
		}
		ClearAdvice(AdviceType.None);
		ClearCursor();
		if (mNextSurvivalStageCounter > 0)
		{
			mNextSurvivalStageCounter = 0;
		}
		mApp.mBoardResult = BoardResult.Lost;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie != aZombie)
			{
				if ((float)zombie.GetZombieRect().mX < -50f || zombie.mZombiePhase == ZombiePhase.RisingFromGrave || zombie.mZombiePhase == ZombiePhase.DancerRising)
				{
					zombie.DieNoLoot(giveAchievements: false);
				}
				if ((zombie.mZombieType == ZombieType.Gargantuar || zombie.mZombieType == ZombieType.RedeyeGargantuar) && zombie.IsDeadOrDying() && zombie.mPosX < 140f)
				{
					zombie.DieNoLoot(giveAchievements: false);
				}
			}
		}
		string theMessage = string.Empty;
		bool flag = true;
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			theMessage = "[ZOMBIQUARIUM_DEATH_MESSAGE]";
			flag = false;
		}
		else if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			int survivalFlagsCompleted = GetSurvivalFlagsCompleted();
			string theStringToSubstitute = mApp.Pluralize(survivalFlagsCompleted, "[ONE_FLAG]", "[COUNT_FLAGS]");
			theMessage = TodCommon.TodReplaceString("[LAST_STAND_DEATH_MESSAGE]", "{FLAGS}", theStringToSubstitute);
			flag = false;
		}
		else if (mApp.IsEndlessScaryPotter(mApp.mGameMode) || mApp.IsEndlessIZombie(mApp.mGameMode))
		{
			int mSurvivalStage = mChallenge.mSurvivalStage;
			if (mApp.IsEndlessScaryPotter(mApp.mGameMode))
			{
				mApp.mPlayerInfo.mVasebreakerScore = Math.Max(mApp.mPlayerInfo.mVasebreakerScore, mSurvivalStage);
				LeaderBoardComm.RecordResult(LeaderboardGameMode.Vasebreaker, (int)mApp.mPlayerInfo.mVasebreakerScore);
			}
			else if (mApp.IsEndlessIZombie(mApp.mGameMode))
			{
				mApp.mPlayerInfo.mIZombieScore = Math.Max(mApp.mPlayerInfo.mIZombieScore, mSurvivalStage);
				LeaderBoardComm.RecordResult(LeaderboardGameMode.IZombie, (int)mApp.mPlayerInfo.mIZombieScore);
			}
			theMessage = TodCommon.TodReplaceNumberString("[ENDLESS_PUZZLE_DEATH_MESSAGE]", "{STREAK}", mSurvivalStage);
			flag = false;
		}
		else if (mApp.IsIZombieLevel())
		{
			theMessage = "[I_ZOMBIE_DEATH_MESSAGE]";
			flag = false;
		}
		else if (mApp.IsCornRouletteLevel())
		{
			theMessage = "[CORN_ROULETTE_DEATH_MESSAGE]";
			flag = false;
		}
		if (flag)
		{
			mApp.mGameScene = GameScenes.ZombiesWon;
			aZombie.WalkIntoHouse();
			ClearAdvice(AdviceType.None);
			mCutScene.StartZombiesWon();
			FreezeEffectsForCutscene(theFreeze: true);
			TutorialArrowRemove();
			UpdateCursor();
		}
		else
		{
			GameOverDialog theDialog = new GameOverDialog(theMessage, theShowChallengeName: true);
			mApp.AddDialog(17, theDialog);
			mApp.mMusic.StopAllMusic();
			StopAllZombieSounds();
			mApp.PlaySample(Resources.SOUND_LOSEMUSIC);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombiesWon, theIsPreloading: true);
			int num = Constants.BOARD_EXTRA_ROOM / 2;
			Reanimation reanimation = mApp.AddReanimation(0f - (float)Constants.BOARD_OFFSET + (float)num + (float)Constants.Board_Offset_AspectRatio_Correction, 0f, 900000, ReanimationType.ZombiesWon);
			reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
			ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(Reanimation.ReanimTrackId_fullscreen);
			trackInstanceByName.mTrackColor = SexyColor.Black;
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_screen);
		}
	}

	public void DrawLevel(Graphics g)
	{
		if (mLevelFadeCount <= 0)
		{
			return;
		}
		if (mApp.IsAdventureMode())
		{
			if (levelStrVal != mLevel)
			{
				mLevelStr = TodStringFile.TodStringTranslate("[LEVEL]") + " " + mApp.GetStageString(mLevel);
				levelStrVal = mLevel;
			}
		}
		else if (mApp.IsSurvivalMode() || mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			mApp.GetCurrentChallengeIndex();
			int survivalFlagsCompleted = GetSurvivalFlagsCompleted();
			if (survivalFlagsCompleted > 0)
			{
				string theStringToSubstitute = mApp.Pluralize(survivalFlagsCompleted, "[ONE_FLAG]", "[COUNT_FLAGS]");
				mLevelStr = string.Concat(str2: (survivalFlagsCompleted != 1) ? TodCommon.TodReplaceString("[FLAGS_COMPLETED_PLURAL]", "{FLAGS}", theStringToSubstitute) : TodCommon.TodReplaceString("[FLAGS_COMPLETED]", "{FLAGS}", theStringToSubstitute), str0: TodStringFile.TodStringTranslate(mChallenge.mName), str1: " - ");
			}
			else
			{
				mLevelStr = mChallenge.mName;
			}
		}
		else if (mApp.IsEndlessScaryPotter(mApp.mGameMode) || mApp.IsEndlessIZombie(mApp.mGameMode))
		{
			mApp.GetCurrentChallengeIndex();
			int num = mChallenge.mSurvivalStage;
			if (mNextSurvivalStageCounter > 0)
			{
				num++;
			}
			if (num > 0)
			{
				string text = TodCommon.TodReplaceNumberString("[ENDLESS_STREAK]", "{STREAK}", num);
				mLevelStr = TodStringFile.TodStringTranslate(mChallenge.mName) + " - " + text;
			}
			else
			{
				mLevelStr = mChallenge.mName;
			}
		}
		else
		{
			mLevelStr = mChallenge.mName;
		}
		int num2;
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			num2 = 700;
			num2 = ((!HasProgressMeter()) ? (num2 - 20) : (num2 - (AtlasResources.IMAGE_FLAGMETER.GetCelWidth() + 7)));
		}
		else
		{
			num2 = Constants.UILevelPosition.X - Constants.Board_Offset_AspectRatio_Correction;
		}
		int num3 = Constants.UILevelPosition.Y;
		if (mChallenge.mChallengeState == ChallengeState.ZenFading)
		{
			num3 += TodCommon.TodAnimateCurve(50, 0, mChallenge.mChallengeStateCounter, 0, 50, TodCurves.EaseInOut);
		}
		PulloutPanel mLevelInfoPanel = mApp.mLevelInfoPanel;
		if (mLevelInfoPanel != null && mLevelInfoPanel.mVisible)
		{
			TRect handleBounds = mLevelInfoPanel.HandleBounds;
			handleBounds.mX += mLevelInfoPanel.mX - mX;
			handleBounds.mY += mLevelInfoPanel.mY - mY;
			int num4 = Resources.FONT_HOUSEOFTERROR16.StringWidth(TodStringFile.TodStringTranslate(mLevelStr));
			int height = Resources.FONT_HOUSEOFTERROR16.GetHeight();
			if (num2 > handleBounds.mX - 8 && num2 - num4 < handleBounds.mX + handleBounds.mWidth + 8 && num3 + height > handleBounds.mY - 8 && num3 - height < handleBounds.mY + handleBounds.mHeight + 8)
			{
				num3 = handleBounds.mY - height - 8;
			}
		}
		int theAlpha = TodCommon.ClampInt(255 * mLevelFadeCount / 15, 0, 255);
		TodCommon.TodDrawString(g, mLevelStr, num2, num3, Resources.FONT_HOUSEOFTERROR16, new SexyColor(224, 187, 98, theAlpha), DrawStringJustification.Right);
	}

	public void DrawShovel(Graphics g)
	{
		if (IsCombatToolVisible(GameObjectType.Shovel))
		{
			TRect shovelButtonRect = GetShovelButtonRect();
			g.DrawImage(AtlasResources.IMAGE_SHOVELBANK_ZEN, shovelButtonRect.mX, shovelButtonRect.mY);
			if (mCursorObject.mCursorType != CursorType.Shovel)
			{
				if (mChallenge.mChallengeState == ChallengeState.ZenPlantInteracting)
				{
					SexyColor flashingColor = TodCommon.GetFlashingColor(mMainCounter, 75);
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(flashingColor);
				}
				g.DrawImage(AtlasResources.IMAGE_SHOVEL_PLANTGIRL, shovelButtonRect.mX - 7, shovelButtonRect.mY - 3);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		if (HasTrashcan())
		{
			if (mCameraEnabled && mCamera != null)
			{
				mCamera?.ApplyTransform(g);
			}
			Graphics graphics = Graphics.GetNew(g);
			graphics.SetScale(0.75f);
			if (IsPlantInCursor() && TrashcanHitTest(mCursorObject.mX, mCursorObject.mY))
			{
				graphics.DrawImage(AtlasResources.IMAGE_TRASHCAN, 0f * Constants.S, 70f * Constants.S);
				graphics.SetColorizeImages(colorizeImages: true);
				graphics.SetColor(new SexyColor(255, 255, 255, 128, premultiply: false));
			}
			else if (IsPlantInCursor())
			{
				SexyColor flashingColor2 = TodCommon.GetFlashingColor(mMainCounter, 75);
				graphics.SetColorizeImages(colorizeImages: true);
				graphics.SetColor(flashingColor2);
			}
			graphics.DrawImage(AtlasResources.IMAGE_TRASHCAN, 0f * Constants.S, 70f * Constants.S);
			graphics.PrepareForReuse();
			if (mCameraEnabled && mCamera != null)
			{
				mCamera?.ResetTransform(g);
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom || HasGlove())
		{
			DrawZenButtons(g);
		}
	}

	public bool IsPlantSkillEnabled(SeedType seedType)
	{
		if ((uint)(seedType - 51) > 1u)
		{
			return false;
		}
		bool? flag = mApp.mCreativeLevel?.GetComponent<CSPlantSkill>()?.Enabled;
		if (!flag.HasValue)
		{
			if (seedType != SeedType.Endoflame)
			{
				return mApp.mPlayerInfo.mPurchases[35] > 0;
			}
			return true;
		}
		return flag == true;
	}

	public bool HasPauseButton()
	{
		return mApp.mCreativeLevel?.GetComponent<CSPause>()?.Enabled ?? (!mApp.IsWhackAZombieLevel());
	}

	public bool HasTrashcan()
	{
		return mApp.mCreativeLevel?.GetComponent<CSTrashcan>()?.Enabled ?? mApp.IsRogueConveyorbeltLevel();
	}

	public bool HasShovel()
	{
		return mApp.mCreativeLevel?.GetComponent<CSShovel>()?.Enabled ?? mShowShovel;
	}

	public bool IsCombatToolVisible(GameObjectType tool)
	{
		GameMode mGameMode = mApp.mGameMode;
		bool flag = ((mGameMode == GameMode.ChallengeZenGarden || mGameMode == GameMode.TreeOfWisdom) ? true : false);
		if (flag || mApp.GetDialog(Dialogs.DIALOG_GAME_OVER) != null)
		{
			return false;
		}
		bool flag2 = mApp.mGameScene == GameScenes.Playing;
		return tool switch
		{
			GameObjectType.Shovel => HasShovel() && (flag2 || (mApp.mGameScene == GameScenes.LevelIntro && mCutScene.IsInShovelTutorial())), 
			GameObjectType.Glove => HasGlove() & flag2, 
			_ => false, 
		};
	}

	public bool CanUseCombatTool(GameObjectType tool)
	{
		if (IsCombatToolVisible(tool) && CanInteractWithBoardButtons())
		{
			if (tool == GameObjectType.Glove)
			{
				return mChallenge.mGloveCounter <= 0;
			}
			return true;
		}
		return false;
	}

	public bool HasGlove()
	{
		CSGlove cSGlove = mApp.mCreativeLevel?.GetComponent<CSGlove>();
		if (cSGlove != null)
		{
			return cSGlove.Enabled;
		}
		if (!mApp.IsRogueConveyorbeltLevel() && mApp.mGameMode != GameMode.ImitaterRandom && mApp.mGameMode != GameMode.ChallengeFusion && !mApp.IsSurvivalHell(mApp.mGameMode))
		{
			return mApp.IsAdventureModeHard();
		}
		return true;
	}

	public bool HasFusion()
	{
		return mApp.IsFusionEnabled();
	}

	public bool TrashcanHitTest(int x, int y)
	{
		if (HasTrashcan())
		{
			return new TRect(0, 70, 50, 80).Contains(x, y);
		}
		return false;
	}

	public void UpdateZombieSpawning()
	{
		if (mApp.mGameMode == GameMode.Upsell || mApp.mGameMode == GameMode.Intro)
		{
			return;
		}
		if (mFinalWaveSoundCounter > 0)
		{
			mFinalWaveSoundCounter--;
			if (mFinalWaveSoundCounter == 0)
			{
				mApp.PlaySample(Resources.SOUND_FINALWAVE);
			}
		}
		if (mTutorialState == TutorialState.Level1PickUpPeashooter || mTutorialState == TutorialState.Level1PlantPeashooter || mTutorialState == TutorialState.Level1RefreshPeashooter || mTutorialState == TutorialState.SlotMachinePull || HasLevelAwardDropped())
		{
			return;
		}
		if (mRiseFromGraveCounter > 0)
		{
			mRiseFromGraveCounter--;
			if (mRiseFromGraveCounter == 0)
			{
				SpawnZombiesFromGraves();
			}
		}
		if (mHugeWaveCountDown > 0)
		{
			mHugeWaveCountDown--;
			if (mHugeWaveCountDown == 0)
			{
				ClearAdvice(AdviceType.HugeWave);
				NextWaveComing();
				mZombieCountDown = 1;
			}
			else
			{
				if (mHugeWaveCountDown != 726)
				{
					if (Music.MusicTuneHasDrum(mApp.mMusic.mCurMusicTune) && (mApp.mMusic.mCurMusicTune == MusicTune.DayGrasswalk || mApp.mMusic.mCurMusicTune == MusicTune.PoolWaterygraves || mApp.mMusic.mCurMusicTune == MusicTune.FogRigormormist || mApp.mMusic.mCurMusicTune == MusicTune.RoofGrazetheroof))
					{
						if (mHugeWaveCountDown == 400)
						{
							mApp.mMusic.StartBurst();
						}
					}
					else if (Music.MusicTuneHasDrum(mApp.mMusic.mCurMusicTune) && mApp.mMusic.mCurMusicTune == MusicTune.NightMoongrains && mHugeWaveCountDown == 700)
					{
						mApp.mMusic.StartBurst();
					}
					return;
				}
				mApp.PlaySample(Resources.SOUND_HUGE_WAVE);
			}
		}
		if (mChallenge.UpdateZombieSpawning() || (mCurrentWave == mNumWaves && (IsFinalSurvivalStage() || mApp.mGameMode == GameMode.ChallengeLastStand || (!mApp.IsSurvivalMode() && !mApp.IsContinuousChallenge()))))
		{
			return;
		}
		mZombieCountDown--;
		if (mCurrentWave == mNumWaves && mApp.IsSurvivalMode())
		{
			if (mZombieCountDown == 0)
			{
				FadeOutLevel();
			}
			return;
		}
		int num = mZombieCountDownStart - mZombieCountDown;
		if (mZombieCountDown > 5 && num > 400)
		{
			int num2 = TotalZombiesHealthInWave(mCurrentWave - 1);
			if (num2 <= mZombieHealthToNextWave && mZombieCountDown > 200)
			{
				mZombieCountDown = 200;
			}
		}
		if (mZombieCountDown == 5)
		{
			if (IsFlagWave(mCurrentWave))
			{
				ClearAdviceImmediately();
				DisplayAdviceAgain("[ADVICE_HUGE_WAVE]", MessageStyle.HugeWave, AdviceType.HugeWave);
				mHugeWaveCountDown = 750;
				return;
			}
			NextWaveComing();
		}
		if (mZombieCountDown != 0)
		{
			return;
		}
		SpawnZombieWave();
		mZombieHealthWaveStart = TotalZombiesHealthInWave(mCurrentWave - 1);
		bool flag = mApp.IsWallnutBowlingLevel() || mApp.mGameMode == GameMode.ChallengeLastStand;
		if (mCurrentWave == mNumWaves && mApp.IsSurvivalMode())
		{
			mZombieHealthToNextWave = 0;
			mZombieCountDown = 5500;
		}
		else if (IsFlagWave(mCurrentWave) && !flag)
		{
			mZombieHealthToNextWave = 0;
			mZombieCountDown = 4500;
		}
		else
		{
			mZombieHealthToNextWave = (int)(TodCommon.RandRangeFloat(0.5f, 0.65f) * (float)mZombieHealthWaveStart);
			if (mApp.IsLittleTroubleLevel() || mApp.mGameMode == GameMode.ChallengeColumn || mApp.mGameMode == GameMode.ChallengeLastStand)
			{
				mZombieCountDown = 750;
			}
			else
			{
				mZombieCountDown = 2500 + RandomNumbers.NextNumber(600);
			}
		}
		CSManualZombieCountDown cSManualZombieCountDown = mApp.mCreativeLevel?.GetComponent<CSManualZombieCountDown>();
		if (cSManualZombieCountDown != null && cSManualZombieCountDown.mInfo.TryGetValue(mCurrentWave - 1, out var value))
		{
			(mZombieCountDown, mZombieHealthToNextWave) = value;
		}
		mZombieCountDownStart = mZombieCountDown;
	}

	public void UpdateSunSpawning()
	{
		CSSpawnSkySun cSSpawnSkySun = null;
		if (mApp.mCreativeLevel != null)
		{
			cSSpawnSkySun = mApp.mCreativeLevel.GetComponent<CSSpawnSkySun>();
			if (cSSpawnSkySun != null && !cSSpawnSkySun.mSpawnFromSky)
			{
				return;
			}
		}
		if ((cSSpawnSkySun == null && (StageIsNight() || HasLevelAwardDropped() || mApp.mGameMode == GameMode.ChallengeRainingSeeds || mApp.mGameMode == GameMode.ChallengeIce || mApp.mGameMode == GameMode.Upsell || mApp.mGameMode == GameMode.Intro || mApp.mGameMode == GameMode.ChallengeZombiquarium || mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom || mApp.mGameMode == GameMode.ChallengeLastStand || mApp.IsIZombieLevel() || mApp.IsScaryPotterLevel() || mApp.IsSquirrelLevel() || HasConveyorBeltSeedBank())) || mTutorialState == TutorialState.SlotMachinePull || ((mTutorialState == TutorialState.Level1PickUpPeashooter || mTutorialState == TutorialState.Level1PlantPeashooter) && mPlants.Count == 0))
		{
			return;
		}
		mSunCountDown--;
		if (mSunCountDown <= 0)
		{
			int theX = Constants.LAWN_XMIN + RandomNumbers.NextNumber(Constants.Board_SunCoinRange);
			mNumSunsFallen++;
			if (cSSpawnSkySun != null)
			{
				mSunCountDown = Math.Min(cSSpawnSkySun.mMaxTime, cSSpawnSkySun.mMinTime + mNumSunsFallen * cSSpawnSkySun.mIncrementPerSun) + RandomNumbers.NextNumber(cSSpawnSkySun.mRandomRangeTime);
			}
			else
			{
				mSunCountDown = Math.Min(950, 425 + mNumSunsFallen * 10) + RandomNumbers.NextNumber(275);
			}
			CoinType theCoinType = CoinType.Sun;
			if (mApp.mGameMode == GameMode.ChallengeSunnyDay)
			{
				theCoinType = CoinType.Largesun;
			}
			if (cSSpawnSkySun != null)
			{
				theCoinType = cSSpawnSkySun.mSunType switch
				{
					0 => CoinType.Smallsun, 
					2 => CoinType.Largesun, 
					_ => CoinType.Sun, 
				};
			}
			AddCoin(theX, 60, theCoinType, CoinMotion.FromSky);
		}
	}

	public void ClearAdvice(AdviceType theHelpIndex)
	{
		if (theHelpIndex == AdviceType.None || theHelpIndex == mHelpIndex)
		{
			mAdvice.ClearLabel();
			mHelpIndex = AdviceType.None;
		}
	}

	public bool RowCanHaveZombieType(int theRow, ZombieType theZombieType)
	{
		if (!RowCanHaveZombies(theRow))
		{
			return false;
		}
		if (mApp.mGameMode == GameMode.ChallengeResodded && mPlantRow[theRow] == PlantRowType.Dirt && mCurrentWave < 5)
		{
			return false;
		}
		if (theZombieType == ZombieType.Bungee)
		{
			return true;
		}
		if (mPlantRow[theRow] == PlantRowType.Pool && (!Zombie.ZombieTypeCanGoInPool(theZombieType) || (Zombie.ZombieTypeCanGoInPoolExtended(theZombieType) && !mApp.IsLevelWithExtendedPoolZombies())))
		{
			return false;
		}
		if (mPlantRow[theRow] == PlantRowType.HighGround && !Zombie.ZombieTypeCanGoOnHighGround(theZombieType))
		{
			return false;
		}
		int num = mCurrentWave;
		if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			num += mChallenge.mSurvivalStage * GetNumWavesPerSurvivalStage();
		}
		CSSurvivalLevel cSSurvivalLevel = mApp.mCreativeLevel?.GetComponent<CSSurvivalLevel>();
		if ((mPlantRow[theRow] != PlantRowType.Pool || mBackground == BackgroundType.BigPool || num >= 5 || IsZombieTypePoolOnly(theZombieType)) && (mPlantRow[theRow] == PlantRowType.Pool || !IsZombieTypePoolOnly(theZombieType)) && (theZombieType != ZombieType.Bobsled || mIceTimer[theRow] > 0) && (theRow != 0 || mApp.IsSurvivalEndless(mApp.mGameMode) || mApp.IsSurvivalHell(mApp.mGameMode) || (cSSurvivalLevel != null && cSSurvivalLevel.mEndless) || (theZombieType != ZombieType.Gargantuar && theZombieType != ZombieType.RedeyeGargantuar)))
		{
			if (theZombieType == ZombieType.Dancer && !StageHasPool())
			{
				if (RowCanHaveZombies(theRow - 1))
				{
					return RowCanHaveZombies(theRow + 1);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public int NumberZombiesInWave(int theWaveIndex)
	{
		Debug.ASSERT(theWaveIndex >= 0 && theWaveIndex < 100 && theWaveIndex < mNumWaves);
		for (int i = 0; i < 50; i++)
		{
			ZombieType zombieType = mZombiesInWave[theWaveIndex, i];
			if (zombieType == ZombieType.Invalid)
			{
				return i;
			}
		}
		Debug.ASSERT(value: false);
		return 0;
	}

	public int TotalZombiesHealthInWave(int theWaveIndex)
	{
		int num = 0;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie.mFromWave == theWaveIndex && !zombie.mMindControlled && !zombie.IsDeadOrDying() && zombie.mZombieType != ZombieType.Bungee && zombie.mRelatedZombieID == null)
			{
				num += zombie.mBodyHealth;
				num += zombie.mHelmHealth;
				num += (int)((float)zombie.mShieldHealth * 0.2f);
				num += zombie.mFlyingHealth;
			}
		}
		return num;
	}

	public void DrawUICoinBank(Graphics g)
	{
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		if ((mApp.mGameScene == GameScenes.Playing || mApp.mCrazyDaveState != CrazyDaveState.Off) && mCoinBankFadeCount > 0)
		{
			int num = Constants.UICoinBankPosition.X;
			int num2 = Constants.UICoinBankPosition.Y - AtlasResources.IMAGE_COINBANK.mHeight - 1;
			if (mApp.IsSlotMachineLevel())
			{
				num -= 50;
			}
			if (mApp.mGameMode == GameMode.ChallengeZenGarden)
			{
				num = 450 - mX;
			}
			else if (mApp.mCrazyDaveState != CrazyDaveState.Off)
			{
				num = 150 - mX;
			}
			g.SetColorizeImages(colorizeImages: true);
			int theAlpha = TodCommon.ClampInt(255 * mCoinBankFadeCount / 15, 0, 255);
			g.SetColor(new SexyColor(255, 255, 255, theAlpha));
			g.DrawImage(AtlasResources.IMAGE_COINBANK, num, num2);
			g.SetColor(new SexyColor(180, 255, 90, theAlpha));
			g.SetFont(Resources.FONT_CONTINUUMBOLD14);
			string moneyString = LawnApp.GetMoneyString(mApp.mPlayerInfo.mCoins);
			int theX = num + Constants.StoreScreen_Coinbank_TextOffset.X - Resources.FONT_CONTINUUMBOLD14.StringWidth(moneyString);
			g.DrawString(moneyString, theX, num2 + Constants.StoreScreen_Coinbank_TextOffset.Y);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public void ShowCoinBank()
	{
		mCoinBankFadeCount = 1000;
	}

	public void FadeOutLevel()
	{
		if (mManualPaused)
		{
			mManualPaused = false;
		}
		if (mApp.IsScaryPotterLevel())
		{
			if (mApp.mGameMode == GameMode.ScaryPotterEndless && mChallenge.mSurvivalStage >= 14)
			{
				GrantAchievement(AchievementId.ChinaShop);
			}
		}
		else if (mApp.IsIZombieLevel())
		{
			if (mApp.mGameMode == GameMode.PuzzleIZombieEndless && mChallenge.mSurvivalStage >= 9)
			{
				GrantAchievement(AchievementId.BetterOffDead);
			}
		}
		else if (!mApp.IsQuickPlayMode() && !HasConveyorBeltSeedBank() && !mApp.IsWhackAZombieLevel())
		{
			if (AwardCloseShave())
			{
				GrantAchievement(AchievementId.CloseShave);
			}
			if (mNomNomNomAchievementTracker)
			{
				GrantAchievement(AchievementId.NomNomNom);
			}
			if (StageIsNight() && mNoFungusAmongUsAchievementTracker)
			{
				GrantAchievement(AchievementId.NoFungusAmongUs);
			}
			if (StageHasPool() && !mPeaShooterUsed)
			{
				GrantAchievement(AchievementId.DontPeainthePool);
			}
			if (StageHasRoof() && !mCatapultPlantsUsed)
			{
				GrantAchievement(AchievementId.Grounded);
			}
			if (StageIsDayWithoutPool() && mMushroomAndCoffeeBeansOnly)
			{
				GrantAchievement(AchievementId.GoodMorning);
			}
		}
		if (mLevel >= 3)
		{
			mApp.mPlayerInfo.mHasFinishedTutorial = true;
		}
		if (mApp.mGameScene != GameScenes.Playing)
		{
			RefreshSeedPacketFromCursor();
			mApp.mLastLevelStats.mUnusedLawnMowers = 0;
			mLevelComplete = true;
			return;
		}
		bool flag = true;
		if (mApp.IsScaryPotterLevel() && !IsFinalScaryPotterStage())
		{
			flag = false;
		}
		else if (IsSurvivalStageWithRepick())
		{
			flag = false;
		}
		else if (IsLastStandStageWithRepick())
		{
			flag = false;
		}
		else if (mApp.IsEndlessIZombie(mApp.mGameMode))
		{
			flag = false;
		}
		if (flag)
		{
			mApp.mMusic.StopAllMusic();
			if (mApp.IsAdventureMode() && mLevel == 50)
			{
				mApp.PlayFoley(FoleyType.Finalfanfare);
			}
			else if (mApp.AboutToEarnGoldSunflower())
			{
				mApp.PlayFoley(FoleyType.Finalfanfare);
			}
			else
			{
				mApp.PlayFoley(FoleyType.Winmusic);
			}
		}
		if (mApp.IsEndlessScaryPotter(mApp.mGameMode))
		{
			mLevelAwardSpawned = true;
			mNextSurvivalStageCounter = 500;
			string theAdvice = TodCommon.TodReplaceNumberString("[ADVICE_MORE_SCARY_POTS]", "{STREAK}", mChallenge.mSurvivalStage + 1);
			PuzzleSaveStreak();
			ClearAdvice(AdviceType.None);
			DisplayAdvice(theAdvice, MessageStyle.BigMiddle, AdviceType.None);
			return;
		}
		if (mApp.IsAdventureMode() && mApp.IsScaryPotterLevel() && !IsFinalScaryPotterStage())
		{
			mNextSurvivalStageCounter = 500;
			ClearAdvice(AdviceType.None);
			return;
		}
		if (mApp.IsScaryPotterLevel() && !IsFinalScaryPotterStage())
		{
			mLevelAwardSpawned = true;
			mNextSurvivalStageCounter = 500;
			string theAdvice2 = TodCommon.TodReplaceNumberString("[ADVICE_3_IN_A_ROW]", "{STREAK}", mChallenge.mSurvivalStage + 1);
			PuzzleSaveStreak();
			ClearAdvice(AdviceType.None);
			DisplayAdvice(theAdvice2, MessageStyle.BigMiddle, AdviceType.None);
			return;
		}
		if (mApp.IsEndlessIZombie(mApp.mGameMode))
		{
			mNextSurvivalStageCounter = 500;
			string theAdvice3 = TodCommon.TodReplaceNumberString("[ADVICE_MORE_IZOMBIE]", "{STREAK}", mChallenge.mSurvivalStage + 1);
			PuzzleSaveStreak();
			ClearAdvice(AdviceType.None);
			DisplayAdvice(theAdvice3, MessageStyle.BigMiddle, AdviceType.None);
			return;
		}
		if (IsLastStandStageWithRepick())
		{
			mNextSurvivalStageCounter = 500;
			mChallenge.LastStandCompletedStage();
			return;
		}
		if (!IsSurvivalStageWithRepick())
		{
			RefreshSeedPacketFromCursor();
			mApp.mLastLevelStats.mUnusedLawnMowers = CountUntriggerLawnMowers();
			mBoardFadeOutCounter = 600;
			if (mLevel == 9 || mLevel == 19 || mLevel == 29 || mLevel == 39 || mLevel == 49)
			{
				mBoardFadeOutCounter = 500;
			}
			if (CanDropLoot())
			{
				mScoreNextMowerCounter = 200;
			}
			Coin theCoin = null;
			while (IterateCoins(ref theCoin))
			{
				theCoin.TryAutoCollectAfterLevelAward();
			}
			return;
		}
		Debug.ASSERT(mApp.IsSurvivalMode());
		mNextSurvivalStageCounter = 500;
		DisplayAdvice("[ADVICE_MORE_ZOMBIES]", MessageStyle.BigMiddle, AdviceType.None);
		mApp.mMusic.FadeOut(500);
		mApp.PlaySample(Resources.SOUND_HUGE_WAVE);
		for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
		{
			if (mIceTimer[i] > mNextSurvivalStageCounter)
			{
				mIceTimer[i] = mNextSurvivalStageCounter;
			}
		}
	}

	public bool AwardCloseShave()
	{
		if (GetBottomLawnMower() == null && (mBackground != BackgroundType.Num5Roof || mApp.mPlayerInfo.mPurchases[23] != 0) && mApp.mGameMode != GameMode.ChallengeBeghouled)
		{
			return mApp.mGameMode != GameMode.ChallengeBeghouledTwist;
		}
		return false;
	}

	public void DrawFadeOut(Graphics g)
	{
		if (mBoardFadeOutCounter >= 0 && !IsSurvivalStageWithRepick())
		{
			int theAlpha = TodCommon.TodAnimateCurve(200, 0, mBoardFadeOutCounter, 0, 255, TodCurves.Linear);
			if (mLevel == 9 || mLevel == 19 || mLevel == 29 || mLevel == 39 || mLevel == 49)
			{
				g.SetColor(new SexyColor(0, 0, 0, theAlpha, premultiply: false));
			}
			else
			{
				g.SetColor(new SexyColor(255, 255, 255, theAlpha, premultiply: false));
			}
			g.SetColorizeImages(colorizeImages: true);
			g.FillRect(-Constants.Board_Offset_AspectRatio_Correction - (mApp.mScreenScales.mVirtualWidth - Constants.BOARD_WIDTH) / 2, 0, mWidth + (mApp.mScreenScales.mVirtualWidth - Constants.BOARD_WIDTH), mHeight);
		}
	}

	public void DrawIce(Graphics g, int y)
	{
		int theY = (int)((float)(GridToPixelY(8, y) + 20) * Constants.S);
		int height = AtlasResources.IMAGE_ICE.GetHeight();
		int num = TodCommon.ClampInt((int)((float)(255 * mIceTimer[y]) / 10f), 0, 255);
		if (num < 255)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, num, premultiply: true));
		}
		int num2 = (int)((float)mIceMinX[y] * Constants.S);
		int num3 = 8;
		int num4 = num2 + num3;
		int width = AtlasResources.IMAGE_ICE.GetWidth();
		int bOARD_WIDTH = Constants.BOARD_WIDTH;
		int num5;
		for (int i = num4; i < bOARD_WIDTH; i += num5)
		{
			if (i == num4)
			{
				num5 = (bOARD_WIDTH - num4) % width;
				if (num5 == 0)
				{
					num5 = width;
				}
			}
			else
			{
				num5 = width;
			}
			g.DrawImage(theSrcRect: new TRect(width - num5, 0, num5, height), theDestRect: new TRect(i, theY, num5, height), theImage: AtlasResources.IMAGE_ICE);
		}
		g.DrawImage(AtlasResources.IMAGE_ICE_CAP, num2, theY);
		g.SetColorizeImages(colorizeImages: false);
	}

	public void DrawCelHighlight(Graphics g, int theCol, int theRow)
	{
		if (mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom)
		{
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			SexyColor sexyColor = new SexyColor
			{
				mAlpha = 0
			};
			int num = (sexyColor.mBlue = 50);
			int mRed = (sexyColor.mGreen = num);
			sexyColor.mRed = mRed;
			g.SetColor(sexyColor);
			TPoint[] celPosition = GetCelPosition(theCol, theRow);
			celPosition[0].mX = (int)((float)celPosition[0].mX * Constants.S);
			celPosition[0].mY = (int)((float)celPosition[0].mY * Constants.S);
			celPosition[1].mX = (int)((float)celPosition[1].mX * Constants.S);
			celPosition[1].mY = (int)((float)celPosition[1].mY * Constants.S);
			celPosition[2].mX = (int)((float)celPosition[2].mX * Constants.S);
			celPosition[2].mY = (int)((float)celPosition[2].mY * Constants.S);
			celPosition[3].mX = (int)((float)celPosition[3].mX * Constants.S);
			celPosition[3].mY = (int)((float)celPosition[3].mY * Constants.S);
			g.SetColorizeImages(colorizeImages: true);
			g.FillRect(new TRect(mCelPoints[0].mX, mCelPoints[0].mY, mCelPoints[2].mX - mCelPoints[0].mX, mCelPoints[2].mY - mCelPoints[0].mY));
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		}
	}

	public TPoint[] GetCelPosition(int theCol, int theRow)
	{
		int num = (StageHas6Rows() ? 6 : 5);
		if (StageHasPool())
		{
			int[] board_Cel_Y_Values_Pool = Constants.Board_Cel_Y_Values_Pool;
			mCelPoints[0].mX = GridToPixelX(theCol, theRow);
			mCelPoints[0].mY = board_Cel_Y_Values_Pool[theRow];
			if (theCol == Constants.GRIDSIZEX - 1)
			{
				mCelPoints[1].mX = 878;
			}
			else
			{
				mCelPoints[1].mX = mCelPoints[0].mX + 80;
			}
			mCelPoints[1].mY = board_Cel_Y_Values_Pool[theRow];
			mCelPoints[2].mX = mCelPoints[1].mX;
			mCelPoints[2].mY = board_Cel_Y_Values_Pool[theRow + 1];
			mCelPoints[3].mX = mCelPoints[0].mX;
			mCelPoints[3].mY = board_Cel_Y_Values_Pool[theRow + 1];
		}
		else if (StageHasRoof())
		{
			int num2 = 20;
			mCelPoints[0].mX = GridToPixelX(theCol, theRow);
			mCelPoints[0].mY = GridToPixelY(theCol, theRow) + num2;
			int num3 = 80;
			mCelPoints[1].mX = mCelPoints[0].mX + num3;
			mCelPoints[1].mY = mCelPoints[0].mY;
			mCelPoints[2].mX = mCelPoints[1].mX;
			mCelPoints[2].mY = ((theRow < num - 1) ? (GridToPixelY(theCol, theRow + 1) + num2) : (mCelPoints[0].mY + num3));
			mCelPoints[3].mX = mCelPoints[0].mX;
			mCelPoints[3].mY = mCelPoints[2].mY;
		}
		else if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			int[] board_Cel_Y_Values_ZenGarden = Constants.Board_Cel_Y_Values_ZenGarden;
			mCelPoints[0].mX = GridToPixelX(theCol, theRow);
			mCelPoints[0].mY = board_Cel_Y_Values_ZenGarden[theRow];
			if (theCol == Constants.GRIDSIZEX - 1)
			{
				mCelPoints[1].mX = 878;
			}
			else
			{
				mCelPoints[1].mX = mCelPoints[0].mX + 80;
			}
			mCelPoints[1].mY = board_Cel_Y_Values_ZenGarden[theRow];
			mCelPoints[2].mX = mCelPoints[1].mX;
			mCelPoints[2].mY = board_Cel_Y_Values_ZenGarden[theRow + 1];
			mCelPoints[3].mX = mCelPoints[0].mX;
			mCelPoints[3].mY = board_Cel_Y_Values_ZenGarden[theRow + 1];
		}
		else
		{
			int[] board_Cel_Y_Values_Normal = Constants.Board_Cel_Y_Values_Normal;
			mCelPoints[0].mX = GridToPixelX(theCol, theRow);
			mCelPoints[0].mY = board_Cel_Y_Values_Normal[theRow];
			if (theCol == Constants.GRIDSIZEX - 1)
			{
				mCelPoints[1].mX = 878;
			}
			else
			{
				mCelPoints[1].mX = mCelPoints[0].mX + 80;
			}
			mCelPoints[1].mY = board_Cel_Y_Values_Normal[theRow];
			mCelPoints[2].mX = mCelPoints[1].mX;
			mCelPoints[2].mY = board_Cel_Y_Values_Normal[theRow + 1];
			mCelPoints[3].mX = mCelPoints[0].mX;
			mCelPoints[3].mY = board_Cel_Y_Values_Normal[theRow + 1];
		}
		return mCelPoints;
	}

	public bool IsIceAt(int theGridX, int theGridY)
	{
		Debug.ASSERT(theGridY >= 0 && theGridY < Constants.MAX_GRIDSIZEY);
		if (mIceTimer[theGridY] <= 0)
		{
			return false;
		}
		if (mIceMinX[theGridY] > 750 + Constants.BOARD_EXTRA_ROOM)
		{
			return false;
		}
		int num = PixelToGridXKeepOnBoard(mIceMinX[theGridY] + 12, 0);
		return theGridX >= num;
	}

	public Zombie ZombieGetID(Zombie theZombie)
	{
		if (mZombies.IndexOf(theZombie) != -1)
		{
			return theZombie;
		}
		return null;
	}

	public Zombie ZombieGet(Zombie theZombieID)
	{
		return mZombies[mZombies.IndexOf(theZombieID)];
	}

	public Zombie ZombieTryToGet(Zombie theZombieID)
	{
		int num = mZombies.IndexOf(theZombieID);
		if (num != -1)
		{
			return mZombies[num];
		}
		return null;
	}

	public static void ZombiePickerInitForWave(ZombiePicker theZombiePicker)
	{
		theZombiePicker.mZombieCount = 0;
		theZombiePicker.mZombiePoints = 0;
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			theZombiePicker.mZombieTypeCount[(int)zombieType] = 0;
		}
	}

	public static void ZombiePickerInit(ZombiePicker theZombiePicker)
	{
		ZombiePickerInitForWave(theZombiePicker);
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			theZombiePicker.mAllWavesZombieTypeCount[(int)zombieType] = 0;
		}
	}

	public void UpdateWinds()
	{
		mWindSum = Vector2.Zero;
		foreach (Wind mWind in mWinds)
		{
			if (!mWind.mDead)
			{
				mWind.Update();
				mWindSum += mWind.mCurrentWind;
			}
		}
	}

	public void UpdateIce()
	{
		for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
		{
			if (mIceTimer[i] > 0)
			{
				mIceTimer[i]--;
				if (mIceTimer[i] < 0)
				{
					mIceTimer[i] = 0;
				}
				if (mIceTimer[i] == 0)
				{
					mIceMinX[i] = Constants.Board_Ice_Start;
				}
			}
		}
	}

	public int GetIceZPos(int theRow)
	{
		return 200000 + 10000 * theRow + 2;
	}

	public bool CanAddBobSled()
	{
		for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
		{
			if (mIceTimer[i] > 0 && mIceMinX[i] < 700 + Constants.BOARD_EXTRA_ROOM)
			{
				return true;
			}
		}
		return false;
	}

	public void ShakeBoard(int theShakeAmountX, int theShakeAmountY)
	{
		mShakeCounter = 12;
		mShakeAmountX = theShakeAmountX;
		mShakeAmountY = theShakeAmountY;
	}

	public int CountUntriggerLawnMowers()
	{
		int num = 0;
		LawnMower theLawnMower = null;
		while (IterateLawnMowers(ref theLawnMower))
		{
			if (theLawnMower.mMowerState != LawnMowerState.Triggered && theLawnMower.mMowerState != LawnMowerState.Squished)
			{
				num++;
			}
		}
		return num;
	}

	public bool IterateZombies(ref Zombie theZombie, ref int index)
	{
		if (index == -1 || index >= mZombies.Count)
		{
			index = mZombies.IndexOf(theZombie);
		}
		while (++index < mZombies.Count)
		{
			theZombie = mZombies[index];
			if (!theZombie.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IteratePlants(ref Plant thePlant, ref int index)
	{
		if (index == -1 || index >= mPlants.Count)
		{
			index = mPlants.IndexOf(thePlant);
		}
		while (++index < mPlants.Count)
		{
			thePlant = mPlants[index];
			if (!thePlant.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IterateProjectiles(ref Projectile theProjectile, ref int index)
	{
		if (index == -1 || index >= mProjectiles.Count)
		{
			index = mProjectiles.IndexOf(theProjectile);
		}
		while (++index < mProjectiles.Count)
		{
			theProjectile = mProjectiles[index];
			if (!theProjectile.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IterateCoins(ref Coin theCoin)
	{
		int num = mCoins.IndexOf(theCoin);
		for (int i = num + 1; i < mCoins.Count; i++)
		{
			theCoin = mCoins[i];
			if (!theCoin.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IterateLawnMowers(ref LawnMower theLawnMower)
	{
		int num = mLawnMowers.IndexOf(theLawnMower);
		for (int i = num + 1; i < mLawnMowers.Count; i++)
		{
			theLawnMower = mLawnMowers[i];
			if (!theLawnMower.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IterateParticles(ref TodParticleSystem theParticle, ref int index)
	{
		if (index == -1 || index >= mApp.mEffectSystem.mParticleHolder.mParticleSystems.Count)
		{
			index = mApp.mEffectSystem.mParticleHolder.mParticleSystems.IndexOf(theParticle);
		}
		while (++index < mApp.mEffectSystem.mParticleHolder.mParticleSystems.Count)
		{
			theParticle = mApp.mEffectSystem.mParticleHolder.mParticleSystems[index];
			if (theParticle != null && !theParticle.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IterateReanimations(ref Reanimation theReanimation, ref int index)
	{
		if (index == -1 || index >= mApp.mEffectSystem.mReanimationHolder.mReanimations.Count)
		{
			index = mApp.mEffectSystem.mReanimationHolder.mReanimations.IndexOf(theReanimation);
		}
		while (++index < mApp.mEffectSystem.mReanimationHolder.mReanimations.Count)
		{
			theReanimation = mApp.mEffectSystem.mReanimationHolder.mReanimations[index];
			if (theReanimation != null && !theReanimation.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public bool IterateGridItems(ref GridItem theGridItem, ref int index)
	{
		if (index == -1 || index >= mGridItems.Count)
		{
			index = mGridItems.IndexOf(theGridItem);
		}
		while (++index < mGridItems.Count)
		{
			theGridItem = mGridItems[index];
			if (!theGridItem.mDead)
			{
				return true;
			}
		}
		return false;
	}

	public void ZombieSwitchRow(Zombie aZombie, int aRow)
	{
		List<Zombie> zombiesInRow = GetZombiesInRow(aZombie.mRow);
		List<Zombie> zombiesInRow2 = GetZombiesInRow(aRow);
		zombiesInRow.Remove(aZombie);
		zombiesInRow2.Add(aZombie);
	}

	public void SortZombieRowLists()
	{
		mZombiesRow1.Sort();
		mZombiesRow2.Sort();
		mZombiesRow3.Sort();
		mZombiesRow4.Sort();
		mZombiesRow5.Sort();
		mZombiesRow6.Sort();
	}

	public List<Zombie> GetZombiesInRow(int aRow)
	{
		return aRow switch
		{
			0 => mZombiesRow1, 
			1 => mZombiesRow2, 
			2 => mZombiesRow3, 
			3 => mZombiesRow4, 
			4 => mZombiesRow5, 
			_ => mZombiesRow6, 
		};
	}

	public void AddToZombieList(Zombie aZombie)
	{
		mZombies.Add(aZombie);
		if (aZombie.mZombieType == ZombieType.Boss)
		{
			for (int i = 0; i < 6; i++)
			{
				List<Zombie> zombiesInRow = GetZombiesInRow(i);
				zombiesInRow.Add(aZombie);
			}
		}
		else
		{
			List<Zombie> zombiesInRow2 = GetZombiesInRow(aZombie.mRow);
			zombiesInRow2.Add(aZombie);
		}
	}

	public void AddToZombieList(Zombie aZombie, int row)
	{
		mZombies.Add(aZombie);
		List<Zombie> zombiesInRow = GetZombiesInRow(row);
		zombiesInRow.Add(aZombie);
	}

	public Zombie AddZombieInRow(ZombieType theZombieType, int theRow, int theFromWave)
	{
		return AddZombieInRow(theZombieType, theRow, theFromWave, theCover: false);
	}

	public Zombie AddZombieInRow(ZombieType theZombieType, int theRow, int theFromWave, bool theCover)
	{
		_ = mZombies.Count;
		_ = mZombies.Capacity - 1;
		bool theVariant = false;
		if (RandomNumbers.NextNumber(5) == 0)
		{
			theVariant = true;
		}
		Zombie newZombie = Zombie.GetNewZombie();
		newZombie.ZombieInitialize(theRow, theZombieType, theVariant, null, theFromWave);
		AddToZombieList(newZombie);
		if (theZombieType == ZombieType.Bobsled && newZombie.IsOnBoard())
		{
			Zombie newZombie2 = Zombie.GetNewZombie();
			Zombie newZombie3 = Zombie.GetNewZombie();
			Zombie newZombie4 = Zombie.GetNewZombie();
			AddToZombieList(newZombie2, theRow);
			AddToZombieList(newZombie3, theRow);
			AddToZombieList(newZombie4, theRow);
			newZombie2.ZombieInitialize(theRow, ZombieType.Bobsled, theVariant: false, newZombie, theFromWave);
			newZombie3.ZombieInitialize(theRow, ZombieType.Bobsled, theVariant: false, newZombie, theFromWave);
			newZombie4.ZombieInitialize(theRow, ZombieType.Bobsled, theVariant: false, newZombie, theFromWave);
		}
		if (theCover && theFromWave >= -1 && theRow >= 0 && theRow <= 5)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mCoverLayerAnimIDs[theRow]);
			if (reanimation != null && theZombieType != ZombieType.Bungee)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_rustle, ReanimLoopType.PlayOnceAndHold, 0, 10f);
			}
		}
		return newZombie;
	}

	public bool IsPoolSquare(int theGridX, int theGridY)
	{
		if (theGridX >= 0 && theGridY >= 0)
		{
			return mGridSquareType[theGridX, theGridY] == GridSquareType.Pool;
		}
		return false;
	}

	public void PickZombieWaves()
	{
		if (mApp.IsLevelUseJson())
		{
			mNumWaves = mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mNumWaves;
		}
		else if ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && mApp.IsWhackAZombieLevel())
		{
			mNumWaves = 8;
		}
		else if (mApp.mGameMode == GameMode.ChallengeStageRandom)
		{
			mNumWaves = 60;
		}
		else if (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
		{
			int num = TodCommon.ClampInt(mLevel - 1, 0, 49);
			mNumWaves = GameConstants.gZombieWaves[num];
			if (!mApp.IsFirstTimeAdventureMode() && !mApp.IsMiniBossLevel())
			{
				if (mNumWaves < 10)
				{
					mNumWaves = 20;
				}
				else
				{
					mNumWaves += 10;
				}
			}
		}
		else if (mApp.IsSurvivalMode() || mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			mNumWaves = GetNumWavesPerSurvivalStage();
		}
		else if (mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom || mApp.IsSquirrelLevel())
		{
			mNumWaves = 0;
		}
		else if (mApp.mGameMode == GameMode.ChallengeWhackAZombie)
		{
			mNumWaves = 12;
		}
		else if (mApp.mGameMode == GameMode.ChallengeWallnutBowling || mApp.mGameMode == GameMode.ChallengeAirRaid || mApp.mGameMode == GameMode.ChallengeGraveDanger || mApp.mGameMode == GameMode.ChallengeHighGravity || mApp.mGameMode == GameMode.ChallengePortalCombat || mApp.mGameMode == GameMode.ChallengeWarAndPeas || mApp.mGameMode == GameMode.ChallengeInvisighoul)
		{
			mNumWaves = 20;
		}
		else if (mApp.IsStormyNightLevel() || mApp.IsLittleTroubleLevel() || mApp.IsBungeeBlitzLevel() || mApp.mGameMode == GameMode.ChallengeColumn || mApp.IsShovelLevel() || mApp.mGameMode == GameMode.ChallengeWarAndPeas2 || mApp.mGameMode == GameMode.ChallengeWallnutBowling2 || mApp.mGameMode == GameMode.ChallengePogoParty || mApp.mGameMode == GameMode.ChallengePogoParty || mApp.mGameMode == GameMode.ChallengeColumn2 || mApp.mGameMode == GameMode.ChallengeColumn3 || mApp.mGameMode == GameMode.ChallengeColumn4)
		{
			mNumWaves = 30;
		}
		else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans || mApp.mGameMode == GameMode.ChallengeAttackOnTitans2 || mApp.mGameMode == GameMode.ChallengeAttackOnTitans3)
		{
			mNumWaves = 40;
			if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans3)
			{
				mNumWaves += 20;
			}
		}
		else if (mApp.mGameMode == GameMode.PoolParty)
		{
			mNumWaves = 30;
		}
		else if (mApp.mGameMode == GameMode.ChallengeMoreAirRaid)
		{
			mNumWaves = 30;
		}
		else if (mApp.mGameMode == GameMode.ChallengeFusion)
		{
			mNumWaves = 30;
		}
		else if (mApp.mGameMode == GameMode.OccupyHighGround)
		{
			mNumWaves = 30;
		}
		else
		{
			mNumWaves = 40;
		}
		ZombiePicker zombiePicker = new ZombiePicker();
		ZombiePickerInit(zombiePicker);
		ZombieType introducedZombieType = GetIntroducedZombieType();
		Debug.ASSERT(mNumWaves <= 100);
		for (int i = 0; i < mNumWaves; i++)
		{
			ZombiePickerInitForWave(zombiePicker);
			mZombiesInWave[i, 0] = ZombieType.Invalid;
			bool flag = IsFlagWave(i);
			bool flag2 = i == mNumWaves - 1;
			if (mApp.IsBungeeBlitzLevel() & flag)
			{
				for (int j = 0; j < 5; j++)
				{
					PutZombieInWave(ZombieType.Bungee, i, zombiePicker);
				}
				if (!flag2)
				{
					if ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) & flag2)
					{
						PutInMissingZombies(i, zombiePicker);
					}
					continue;
				}
			}
			if (mApp.mGameMode == GameMode.ChallengeLastStand)
			{
				zombiePicker.mZombiePoints = (mChallenge.mSurvivalStage * GetNumWavesPerSurvivalStage() + i + 10) * 2 / 5 + 1;
			}
			else if (mApp.IsSurvivalMode() && mChallenge.mSurvivalStage > 0)
			{
				zombiePicker.mZombiePoints = (mChallenge.mSurvivalStage * GetNumWavesPerSurvivalStage() + i) / 5 + 1;
			}
			else if (mApp.IsSurvivalMode() && mChallenge.mSurvivalStage > 0)
			{
				zombiePicker.mZombiePoints = (mChallenge.mSurvivalStage * GetNumWavesPerSurvivalStage() + i) * 2 / 5 + 1;
			}
			else if (mApp.IsAdventureMode() && mApp.HasFinishedAdventure() && mLevel != 5)
			{
				zombiePicker.mZombiePoints = i * 2 / 5 + 1;
			}
			else
			{
				zombiePicker.mZombiePoints = i / 3 + 1;
			}
			if (mApp.mCreativeLevel != null)
			{
				CSManualZombiePoint component = mApp.mCreativeLevel.GetComponent<CSManualZombiePoint>();
				if (component != null)
				{
					zombiePicker.mZombiePoints = component.mDensity[i];
				}
				else
				{
					CSAutoZombiePoint component2 = mApp.mCreativeLevel.GetComponent<CSAutoZombiePoint>();
					if (component2 != null)
					{
						int num2 = i;
						CSSurvivalLevel component3 = mApp.mCreativeLevel.GetComponent<CSSurvivalLevel>();
						if (component3 != null)
						{
							num2 += mChallenge.mSurvivalStage * GetNumWavesPerSurvivalStage();
						}
						zombiePicker.mZombiePoints = (int)((float)num2 * component2.mPointIncrementPerWave) + component2.mStartingPoints;
					}
				}
			}
			if (flag)
			{
				int num3 = Math.Min(zombiePicker.mZombiePoints, 8);
				bool flag3 = true;
				if (mApp.mCreativeLevel != null)
				{
					CSManualZombiePoint component4 = mApp.mCreativeLevel.GetComponent<CSManualZombiePoint>();
					if (component4 == null)
					{
						CSAutoZombiePoint component5 = mApp.mCreativeLevel.GetComponent<CSAutoZombiePoint>();
						if (component5 != null)
						{
							zombiePicker.mZombiePoints = (int)((float)zombiePicker.mZombiePoints * component5.mFlagPointMultiplier);
						}
						else
						{
							zombiePicker.mZombiePoints = (int)((float)zombiePicker.mZombiePoints * 2.5f);
						}
					}
					CSLevelProperty component6 = mApp.mCreativeLevel.GetComponent<CSLevelProperty>();
					if (component6 != null)
					{
						flag3 = component6.mSpawnFlagZombie;
						num3 = Math.Min(zombiePicker.mZombiePoints, component6.mMaxPlainZombiesPerFlag);
					}
				}
				else
				{
					zombiePicker.mZombiePoints = (int)((float)zombiePicker.mZombiePoints * 2.5f);
				}
				if (mApp.mGameMode != GameMode.ChallengeWarAndPeas && mApp.mGameMode != GameMode.ChallengeWarAndPeas2)
				{
					for (int k = 0; k < num3; k++)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
					}
					if (flag3)
					{
						PutZombieInWave(ZombieType.Flag, i, zombiePicker);
					}
				}
			}
			if (mApp.IsAdventureModeHard() && mLevel != 5)
			{
				zombiePicker.mZombiePoints *= 2;
			}
			if (mApp.mCreativeLevel != null)
			{
				CSAutoZombiePoint component7 = mApp.mCreativeLevel.GetComponent<CSAutoZombiePoint>();
				if (component7 != null)
				{
					zombiePicker.mZombiePoints = (int)((float)zombiePicker.mZombiePoints * component7.mPointMultiplier);
				}
				CSMustHaveZombie component8 = mApp.mCreativeLevel.GetComponent<CSMustHaveZombie>();
				if (component8 != null && component8.mZombies.TryGetValue(i, out var value))
				{
					foreach (ZombieType item in value)
					{
						PutZombieInWave(item, i, zombiePicker);
					}
				}
			}
			else if (mApp.IsSurvivalHell(mApp.mGameMode))
			{
				if (GetSurvivalFlagsCompleted() < 4)
				{
					zombiePicker.mZombiePoints *= 3;
				}
				else
				{
					zombiePicker.mZombiePoints *= 5;
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengeColumn || mApp.mGameMode == GameMode.ChallengeColumn4)
			{
				zombiePicker.mZombiePoints *= 6;
			}
			else if (mApp.mGameMode == GameMode.ChallengeColumn3)
			{
				if (i == 29)
				{
					zombiePicker.mZombiePoints *= 20;
				}
				else if (i > 20)
				{
					zombiePicker.mZombiePoints *= 12;
				}
				else
				{
					zombiePicker.mZombiePoints *= 8;
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengeColumn2)
			{
				zombiePicker.mZombiePoints *= 8;
			}
			else if (mApp.IsLittleTroubleLevel() || mApp.IsWallnutBowlingLevel())
			{
				zombiePicker.mZombiePoints *= 4;
			}
			else if (mApp.IsMiniBossLevel())
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.IsStormyNightLevel() && (mApp.IsAdventureMode() || mApp.IsQuickPlayMode()))
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.IsShovelLevel() || mApp.IsBungeeBlitzLevel() || mApp.mGameMode == GameMode.ChallengePortalCombat || mApp.mGameMode == GameMode.ChallengeInvisighoul)
			{
				zombiePicker.mZombiePoints *= 2;
			}
			else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans3)
			{
				zombiePicker.mZombiePoints = (int)((float)zombiePicker.mZombiePoints * 2.5f);
			}
			else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans2)
			{
				zombiePicker.mZombiePoints *= 2;
			}
			else if (mApp.mGameMode == GameMode.ChallengeAttackOnTitans)
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.mGameMode == GameMode.MarvelousPeople)
			{
				zombiePicker.mZombiePoints *= 2;
			}
			else if (mApp.mGameMode == GameMode.OccupyHighGround)
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.mGameMode == GameMode.MarvelousPeople2)
			{
				zombiePicker.mZombiePoints = (int)((float)zombiePicker.mZombiePoints * 1.5f);
			}
			else if (mApp.mGameMode == GameMode.MarvelousPeople3)
			{
				zombiePicker.mZombiePoints *= 2;
			}
			else if (mApp.mGameMode == GameMode.RogueConveyorbelt || mApp.mGameMode == GameMode.RogueConveyorbeltHard)
			{
				zombiePicker.mZombiePoints = (int)((double)zombiePicker.mZombiePoints * (2.0 - Math.Min(1.9900000095367432, Math.Log2(mChallenge.mSurvivalStage + 1) / 6.0)));
			}
			else if (mApp.mGameMode == GameMode.ImitaterRandom)
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.mGameMode == GameMode.MarvelousPeople4)
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.mGameMode == GameMode.PoolParty)
			{
				zombiePicker.mZombiePoints *= 3;
			}
			else if (mApp.mGameMode == GameMode.ChallengeMoreAirRaid)
			{
				zombiePicker.mZombiePoints *= 2;
			}
			else if (mApp.mGameMode == GameMode.ChallengeFusion)
			{
				zombiePicker.mZombiePoints *= 5;
			}
			if (mApp.mCreativeLevel == null && introducedZombieType != ZombieType.Invalid && introducedZombieType != ZombieType.DuckyTube)
			{
				bool flag4 = false;
				switch (introducedZombieType)
				{
				case ZombieType.Balloon:
				case ZombieType.Digger:
					if ((i + 1 == 7) | flag2)
					{
						flag4 = true;
					}
					break;
				case ZombieType.Yeti:
					if (i == mNumWaves / 2 && !mApp.mKilledYetiAndRestarted && !mApp.IsQuickPlayMode())
					{
						flag4 = true;
					}
					break;
				default:
					if ((i == mNumWaves / 2) | flag2)
					{
						flag4 = true;
					}
					break;
				}
				if (flag4)
				{
					PutZombieInWave(introducedZombieType, i, zombiePicker);
				}
			}
			if ((mApp.mCreativeLevel == null && mLevel == 50) & flag2)
			{
				PutZombieInWave(ZombieType.Gargantuar, i, zombiePicker);
			}
			if ((mApp.mCreativeLevel == null && (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())) & flag2)
			{
				PutInMissingZombies(i, zombiePicker);
			}
			if (mApp.mGameMode == GameMode.ChallengeColumn)
			{
				if (i % 10 == 5)
				{
					for (int l = 0; l < 10; l++)
					{
						PutZombieInWave(ZombieType.Ladder, i, zombiePicker);
					}
				}
				if (i % 10 == 8)
				{
					for (int m = 0; m < 10; m++)
					{
						PutZombieInWave(ZombieType.JackInTheBox, i, zombiePicker);
					}
				}
				if (i == 19)
				{
					for (int n = 0; n < 3; n++)
					{
						PutZombieInWave(ZombieType.Gargantuar, i, zombiePicker);
					}
				}
				if (i == 29)
				{
					for (int num4 = 0; num4 < 5; num4++)
					{
						PutZombieInWave(ZombieType.Gargantuar, i, zombiePicker);
					}
				}
			}
			else if (mApp.mGameMode == GameMode.BigPoolSurvivalNormalStage)
			{
				if (mChallenge.mSurvivalStage == 1)
				{
					if (i == 4)
					{
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
					}
					if (i == 6)
					{
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
					}
					if (i == 9)
					{
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
					}
				}
				if (mChallenge.mSurvivalStage == 2)
				{
					if (i == 3)
					{
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
					}
					if (i == 5)
					{
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
					}
					if (i == 6)
					{
						PutZombieInWave(ZombieType.Balloon, i, zombiePicker);
					}
					if (i == 7)
					{
						PutZombieInWave(ZombieType.JackInTheBox, i, zombiePicker);
					}
					if (i == 9)
					{
						PutZombieInWave(ZombieType.Balloon, i, zombiePicker);
						PutZombieInWave(ZombieType.JackInTheBox, i, zombiePicker);
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
					}
				}
				if (mChallenge.mSurvivalStage == 3)
				{
					if (i == 1)
					{
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
					}
					if (i == 3)
					{
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
					}
					if (i == 5)
					{
						PutZombieInWave(ZombieType.Balloon, i, zombiePicker);
					}
					if (i == 6)
					{
						PutZombieInWave(ZombieType.Football, i, zombiePicker);
					}
					if (i == 8)
					{
						PutZombieInWave(ZombieType.FootballPremium, i, zombiePicker);
					}
					if (i == 9)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Balloon, i, zombiePicker);
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
						PutZombieInWave(ZombieType.Football, i, zombiePicker);
						PutZombieInWave(ZombieType.FootballPremium, i, zombiePicker);
					}
				}
				if (mChallenge.mSurvivalStage >= 4)
				{
					if (i == 2)
					{
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
					}
					if (i == 3)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
					}
					if (i == 4)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
					}
					if (i == 5)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.FootballPremium, i, zombiePicker);
					}
					if (i == 6)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Football, i, zombiePicker);
					}
					if (i == 9)
					{
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Normal, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.Pail, i, zombiePicker);
						PutZombieInWave(ZombieType.DolphinRider, i, zombiePicker);
						PutZombieInWave(ZombieType.Snorkel, i, zombiePicker);
						PutZombieInWave(ZombieType.JackInTheBox, i, zombiePicker);
						PutZombieInWave(ZombieType.Football, i, zombiePicker);
						PutZombieInWave(ZombieType.FootballPremium, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
						PutZombieInWave(ZombieType.Propeller, i, zombiePicker);
					}
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengeColumn2)
			{
				if (i % 2 == 0)
				{
					for (int num5 = 0; num5 < 2; num5++)
					{
						PutZombieInWave(ZombieType.Zamboni, i, zombiePicker);
					}
				}
				if (i % 10 == 5)
				{
					for (int num6 = 0; num6 < 10; num6++)
					{
						PutZombieInWave(ZombieType.Ladder, i, zombiePicker);
					}
				}
				if (i % 3 == 2 && (i < 20 || i > 21))
				{
					for (int num7 = 0; num7 < 5; num7++)
					{
						PutZombieInWave(ZombieType.Gargantuar, i, zombiePicker);
					}
				}
				if (i >= 15 && i <= 28)
				{
					int num8 = ((i >= 19) ? 4 : 2);
					for (int num9 = 0; num9 < num8; num9++)
					{
						PutZombieInWave(ZombieType.Digger, i, zombiePicker);
					}
				}
				if (i == 19)
				{
					for (int num10 = 0; num10 < 4; num10++)
					{
						PutZombieInWave(ZombieType.RedeyeGargantuar, i, zombiePicker);
					}
				}
				if (i == 29)
				{
					for (int num11 = 0; num11 < 5; num11++)
					{
						PutZombieInWave(ZombieType.RedeyeGargantuar, i, zombiePicker);
					}
					for (int num12 = 0; num12 < 5; num12++)
					{
						PutZombieInWave(ZombieType.Zamboni, i, zombiePicker);
					}
					for (int num13 = 0; num13 < 10; num13++)
					{
						PutZombieInWave(ZombieType.Football, i, zombiePicker);
					}
					for (int num14 = 0; num14 < 6; num14++)
					{
						PutZombieInWave(ZombieType.RedeyeGargantuar, i, zombiePicker);
					}
					for (int num15 = 0; num15 < 8; num15++)
					{
						PutZombieInWave(ZombieType.Digger, i, zombiePicker);
					}
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengeColumn3)
			{
				int num16 = 0;
				if ((i > 18 && i % 2 == 0) || i == 29)
				{
					num16 = ((i > 25) ? 4 : 2);
					for (int num17 = 0; num17 < num16; num17++)
					{
						PutZombieInWave(ZombieType.FootballPremium, i, zombiePicker);
					}
				}
				if ((i > 14 && i % 2 == 1) || i == 29)
				{
					num16 = ((i > 20) ? 8 : 4);
					for (int num18 = 0; num18 < num16; num18++)
					{
						PutZombieInWave(ZombieType.Football, i, zombiePicker);
					}
				}
				if (i > 12 && i % 3 == 0)
				{
					num16 = ((i > 20) ? 6 : 3);
					for (int num19 = 0; num19 < num16; num19++)
					{
						PutZombieInWave(ZombieType.Polevaulter, i, zombiePicker);
					}
				}
			}
			CSSurvivalLevel cSSurvivalLevel = mApp.mCreativeLevel?.GetComponent<CSSurvivalLevel>();
			while (zombiePicker.mZombiePoints > 0 && zombiePicker.mZombieCount < 50)
			{
				if (cSSurvivalLevel != null)
				{
					ZombieType zombieType = PickZombieTypeSurvival(zombiePicker.mZombiePoints, i);
					ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
					CSOverrideZombieProperty component9 = mApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
					if (component9 == null || !component9.mProperties.TryGetValue(zombieType, out var value2))
					{
						value2 = null;
					}
					if (!cSSurvivalLevel.mZombies.TryGetValue(zombieType, out var value3))
					{
						value3 = null;
					}
					PutZombieInWave(zombieType, i, zombiePicker, value3?.mValue ?? value2?.mZombieValue ?? zombieDefinition.mZombieValue);
				}
				else
				{
					ZombieType theZombieType = PickZombieType(zombiePicker.mZombiePoints, i, zombiePicker);
					PutZombieInWave(theZombieType, i, zombiePicker);
				}
			}
			int mZombiePoints = zombiePicker.mZombiePoints;
		}
	}

	public void StopAllZombieSounds()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead)
			{
				zombie.StopZombieSound();
			}
		}
	}

	public bool HasLevelAwardDropped()
	{
		if (!mLevelAwardSpawned)
		{
			if (mNextSurvivalStageCounter <= 0)
			{
				return mBoardFadeOutCounter >= 0;
			}
			return true;
		}
		return true;
	}

	public void UpdateProgressMeter()
	{
		if (mApp.IsFinalBossLevel())
		{
			Zombie bossZombie = GetBossZombie();
			if (bossZombie != null && !bossZombie.IsDeadOrDying())
			{
				mProgressMeterWidth = 150 * (bossZombie.mBodyMaxHealth - bossZombie.mBodyHealth) / bossZombie.mBodyMaxHealth;
			}
			else
			{
				mProgressMeterWidth = 150;
			}
		}
		else if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			mProgressMeterWidth = 150 * mSunMoney / 1000;
		}
		else
		{
			if (mCurrentWave == 0)
			{
				return;
			}
			if (mFlagRaiseCounter > 0)
			{
				mFlagRaiseCounter--;
			}
			int num = 150;
			int numWavesPerFlag = GetNumWavesPerFlag();
			if (ProgressMeterHasFlags())
			{
				int num2 = mNumWaves / numWavesPerFlag;
				num -= num2 * 12;
			}
			int num3 = num / (mNumWaves - 1);
			int num4 = (mCurrentWave - 1) * num / (mNumWaves - 1);
			int num5 = mCurrentWave * num / (mNumWaves - 1);
			if (ProgressMeterHasFlags())
			{
				int num6 = mCurrentWave / numWavesPerFlag;
				num4 += num6 * 12;
				num5 += num6 * 12;
			}
			float num7 = (float)(mZombieCountDownStart - mZombieCountDown) / (float)mZombieCountDownStart;
			if (mZombieHealthToNextWave != -1)
			{
				int num8 = TotalZombiesHealthInWave(mCurrentWave - 1);
				int num9 = Math.Max(mZombieHealthWaveStart - mZombieHealthToNextWave, 1);
				float num10 = (float)(num9 - num8 + mZombieHealthToNextWave) / (float)num9;
				if (num10 > num7)
				{
					num7 = num10;
				}
			}
			int num11 = num4 + TodCommon.FloatRoundToInt((float)(num5 - num4) * num7);
			num11 = TodCommon.ClampInt(num11, 1, 150);
			int num12 = num11 - mProgressMeterWidth;
			if (num12 > num3 && IsInModRange(mMainCounter, 5))
			{
				mProgressMeterWidth++;
			}
			else if (num12 > 0 && IsInModRange(mMainCounter, 20))
			{
				mProgressMeterWidth++;
			}
		}
	}

	public static bool IsInModRange(int number, int mod)
	{
		if (number % mod != 0 && (number - 1) % mod != 0)
		{
			return (number + 1) % mod == 0;
		}
		return true;
	}

	public void DrawUIBottom(Graphics g)
	{
		if (mApp.mZenGarden != null && (mApp.mZenGarden.mIsInteracting || mApp.mZenGarden.mInteracting != null))
		{
			return;
		}
		if (mApp.mGameScene != GameScenes.ZombiesWon)
		{
			if (mSeedBank.BeginDraw(g))
			{
				if (mApp.mPlayerInfo.mClassicSeedChooser)
				{
					mSeedBank.Draw(g);
				}
				else
				{
					mSeedBank.DrawSun(g);
				}
				mSeedBank.EndDraw(g);
			}
			MessageStyle mMessageStyle = mAdvice.mMessageStyle;
		}
		DrawShovel(g);
		if (!StageHasFog())
		{
			DrawTopRightUI(g, (!Constants.Is500pMode) ? (mApp.mPlayerInfo.mClassicSeedChooser ? (TopRightUIDrawMode.MenuButton | TopRightUIDrawMode.StoreButton) : TopRightUIDrawMode.All) : ((TopRightUIDrawMode)0));
		}
		if (mApp.mGameScene == GameScenes.Playing)
		{
			if (!mApp.mPlayerInfo.mClassicSeedChooser)
			{
				DrawProgressMeter(g);
			}
			DrawAgaveInfo(g);
			DrawEndoflameInfo(g);
		}
	}

	public void DrawUITop(Graphics g)
	{
		if (!mApp.mPlayerInfo.mClassicSeedChooser && mApp.mGameScene != GameScenes.ZombiesWon && mSeedBank.BeginDraw(g))
		{
			mSeedBank.Draw(g);
			mSeedBank.EndDraw(g);
		}
		if (StageHasFog())
		{
			DrawTopRightUI(g);
		}
		else if (Constants.Is500pMode)
		{
			DrawTopRightUI(g);
		}
		else if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			DrawTopRightUI(g, TopRightUIDrawMode.PauseButton | TopRightUIDrawMode.AccelerateButton);
		}
		if (mTimeStopCounter > 0)
		{
			g.SetColor(new SexyColor(200, 200, 200, 210));
			g.FillRect(0, 0, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
		}
		if (mApp.IsSlotMachineLevel())
		{
			mChallenge.DrawSlotMachine(g);
		}
		if (mApp.IsCornRouletteLevel())
		{
			mChallenge.CornRouletteDraw(g);
		}
		if (mApp.mGameScene == GameScenes.Playing)
		{
			DrawLevel(g);
			if (mApp.mPlayerInfo.mClassicSeedChooser)
			{
				DrawProgressMeter(g);
			}
		}
		if (mStoreButton != null && mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			mStoreButton.Draw(g);
		}
		if ((mApp.mGameMode == GameMode.Upsell || mApp.mGameMode == GameMode.Intro) && mCutScene.mUpsellHideBoard)
		{
			g.SetColor(new SexyColor(0, 0, 0));
			g.FillRect(-Constants.Board_Offset_AspectRatio_Correction, 0, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
		}
		if (mApp.mGameMode == GameMode.Upsell)
		{
			mCutScene.DrawUpsell(g);
		}
		if (mApp.mGameMode == GameMode.Intro)
		{
			mCutScene.DrawIntro(g);
		}
		if (mApp.mGameScene == GameScenes.LevelIntro || mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom || IsScaryPotterDaveTalking())
		{
			Graphics graphics = Graphics.GetNew(g);
			graphics.mTransX -= mX;
			graphics.mTransY -= mY;
			mApp.DrawCrazyDave(graphics);
			graphics.PrepareForReuse();
		}
		mAdvice.Draw(g);
	}

	public Zombie ZombieHitTest(int theMouseX, int theMouseY)
	{
		Zombie zombie = null;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie2 = mZombies[i];
			if (!zombie2.mDead && !zombie2.IsDeadOrDying() && (mApp.mGameScene != GameScenes.LevelIntro || (zombie2.mZombieType != ZombieType.PeaHead && zombie2.mZombieType != ZombieType.WallnutHead && zombie2.mZombieType != ZombieType.TallnutHead && zombie2.mZombieType != ZombieType.JalapenoHead && zombie2.mZombieType != ZombieType.GatlingHead && zombie2.mZombieType != ZombieType.SquashHead)) && zombie2.GetZombieRect().Contains(theMouseX, theMouseY) && (zombie == null || zombie2.mY > zombie.mY))
			{
				zombie = zombie2;
			}
		}
		return zombie;
	}

	public void KillAllPlantsInRadius(int theX, int theY, int theRadius)
	{
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead)
			{
				TRect plantRect = plant.GetPlantRect();
				if (GameConstants.GetCircleRectOverlap(theX, theY, theRadius, plantRect))
				{
					mPlantsEaten++;
					plant.Die();
				}
			}
		}
	}

	public Plant GetPumpkinAt(int theGridX, int theGridY)
	{
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.mPlantCol == theGridX && plant.mRow == theGridY && !plant.NotOnGround() && plant.mSeedType == SeedType.Pumpkinshell)
			{
				return plant;
			}
		}
		return null;
	}

	public Plant GetFlowerPotAt(int theGridX, int theGridY)
	{
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.mPlantCol == theGridX && plant.mRow == theGridY && !plant.NotOnGround() && plant.mSeedType == SeedType.Flowerpot)
			{
				return plant;
			}
		}
		return null;
	}

	public static bool CanZombieSpawnOnLevel(ZombieType theZombieType, int theLevel)
	{
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(theZombieType);
		if (theZombieType == ZombieType.Yeti)
		{
			return GlobalStaticVars.gLawnApp.CanSpawnYetis();
		}
		if (theLevel < zombieDefinition.mStartingLevel)
		{
			return false;
		}
		if (zombieDefinition.mPickWeight == 0)
		{
			return false;
		}
		if ((GlobalStaticVars.gLawnApp.IsAdventureMode() || GlobalStaticVars.gLawnApp.IsQuickPlayMode()) && !GlobalStaticVars.gLawnApp.IsAdventureModeHard() && theZombieType >= ZombieType.RobotTitan)
		{
			return false;
		}
		Debug.ASSERT(GameConstants.gZombieAllowedLevels[(int)theZombieType].mZombieType == theZombieType);
		int num = TodCommon.ClampInt(theLevel - 1, 0, 49);
		return GameConstants.gZombieAllowedLevels[(int)theZombieType].mAllowedOnLevel[num] != 0;
	}

	public bool IsZombieWaveDistributionOk()
	{
		if (!mApp.IsAdventureMode() && !mApp.IsQuickPlayMode())
		{
			return true;
		}
		int[] array = new int[40];
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			array[(int)zombieType] = 0;
		}
		for (int i = 0; i < mNumWaves; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				ZombieType zombieType2 = mZombiesInWave[i, j];
				if (zombieType2 == ZombieType.Invalid)
				{
					break;
				}
				Debug.ASSERT(zombieType2 >= ZombieType.Normal && zombieType2 < ZombieType.ZombieTypesCount);
				array[(int)zombieType2]++;
			}
		}
		for (ZombieType zombieType3 = ZombieType.Normal; zombieType3 < ZombieType.ZombieTypesCount; zombieType3++)
		{
			if (zombieType3 != ZombieType.Yeti && CanZombieSpawnOnLevel(zombieType3, mLevel) && array[(int)zombieType3] == 0)
			{
				return false;
			}
		}
		return true;
	}

	public void PickBackground()
	{
		if (mApp.IsLevelUseJson())
		{
			mBackground = mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mBackground;
		}
		else
		{
			switch (mApp.mGameMode)
			{
			case GameMode.Adventure:
			case GameMode.Quickplay1:
			case GameMode.Quickplay2:
			case GameMode.Quickplay3:
			case GameMode.Quickplay4:
			case GameMode.Quickplay5:
			case GameMode.Quickplay6:
			case GameMode.Quickplay7:
			case GameMode.Quickplay8:
			case GameMode.Quickplay9:
			case GameMode.Quickplay10:
			case GameMode.Quickplay11:
			case GameMode.Quickplay12:
			case GameMode.Quickplay13:
			case GameMode.Quickplay14:
			case GameMode.Quickplay15:
			case GameMode.Quickplay16:
			case GameMode.Quickplay17:
			case GameMode.Quickplay18:
			case GameMode.Quickplay19:
			case GameMode.Quickplay20:
			case GameMode.Quickplay21:
			case GameMode.Quickplay22:
			case GameMode.Quickplay23:
			case GameMode.Quickplay24:
			case GameMode.Quickplay25:
			case GameMode.Quickplay26:
			case GameMode.Quickplay27:
			case GameMode.Quickplay28:
			case GameMode.Quickplay29:
			case GameMode.Quickplay30:
			case GameMode.Quickplay31:
			case GameMode.Quickplay32:
			case GameMode.Quickplay33:
			case GameMode.Quickplay34:
			case GameMode.Quickplay35:
			case GameMode.Quickplay36:
			case GameMode.Quickplay37:
			case GameMode.Quickplay38:
			case GameMode.Quickplay39:
			case GameMode.Quickplay40:
			case GameMode.Quickplay41:
			case GameMode.Quickplay42:
			case GameMode.Quickplay43:
			case GameMode.Quickplay44:
			case GameMode.Quickplay45:
			case GameMode.Quickplay46:
			case GameMode.Quickplay47:
			case GameMode.Quickplay48:
			case GameMode.Quickplay49:
			case GameMode.Quickplay50:
				if (mLevel <= 10)
				{
					mBackground = BackgroundType.Num1Day;
				}
				else if (mLevel <= 20)
				{
					mBackground = BackgroundType.Num2Night;
				}
				else if (mLevel <= 30)
				{
					mBackground = BackgroundType.Num3Pool;
				}
				else if (mApp.IsScaryPotterLevel())
				{
					mBackground = BackgroundType.Num2Night;
				}
				else if (mLevel <= 40)
				{
					mBackground = BackgroundType.Num4Fog;
				}
				else if (mLevel < 50)
				{
					mBackground = BackgroundType.Num5Roof;
				}
				else if (mLevel == 50)
				{
					mBackground = BackgroundType.Num6Boss;
				}
				else
				{
					mBackground = BackgroundType.Num1Day;
				}
				break;
			case GameMode.SurvivalNormalStage1:
			case GameMode.SurvivalHardStage1:
			case GameMode.SurvivalEndlessStage1:
			case GameMode.ChallengeWarAndPeas:
			case GameMode.ChallengeWallnutBowling:
			case GameMode.ChallengeSlotMachine:
			case GameMode.ChallengeSeeingStars:
			case GameMode.ChallengeWallnutBowling2:
			case GameMode.ChallengeArtChallenge1:
			case GameMode.ChallengeSunnyDay:
			case GameMode.ChallengeResodded:
			case GameMode.ChallengeBigTime:
			case GameMode.ChallengeArtChallenge2:
			case GameMode.ChallengeIce:
			case GameMode.ChallengeShovel:
			case GameMode.ChallengeSquirrel:
			case GameMode.ChallengeFinalBoss2:
			case GameMode.ChallengeColumn2:
			case GameMode.SurvivalHellStage1:
			case GameMode.MarvelousPeople4:
				mBackground = BackgroundType.Num1Day;
				break;
			case GameMode.SurvivalNormalStage2:
			case GameMode.SurvivalHardStage2:
			case GameMode.SurvivalEndlessStage2:
			case GameMode.ChallengeBeghouled:
			case GameMode.ChallengeBeghouledTwist:
			case GameMode.ChallengePortalCombat:
			case GameMode.ChallengeWhackAZombie:
			case GameMode.ChallengeGraveDanger:
			case GameMode.ScaryPotter1:
			case GameMode.ScaryPotter2:
			case GameMode.ScaryPotter3:
			case GameMode.ScaryPotter4:
			case GameMode.ScaryPotter5:
			case GameMode.ScaryPotter6:
			case GameMode.ScaryPotter7:
			case GameMode.ScaryPotter8:
			case GameMode.ScaryPotter9:
			case GameMode.ScaryPotterEndless:
			case GameMode.PuzzleIZombie1:
			case GameMode.PuzzleIZombie2:
			case GameMode.PuzzleIZombie3:
			case GameMode.PuzzleIZombie4:
			case GameMode.PuzzleIZombie5:
			case GameMode.PuzzleIZombie6:
			case GameMode.PuzzleIZombie7:
			case GameMode.PuzzleIZombie8:
			case GameMode.PuzzleIZombie9:
			case GameMode.PuzzleIZombieEndless:
			case GameMode.ChallengeAttackOnTitans2:
			case GameMode.MarvelousPeople:
			case GameMode.ChallengeColumn3:
			case GameMode.SurvivalHellStage2:
			case GameMode.CornRoulette:
			case GameMode.CornRouletteEndless:
				mBackground = BackgroundType.Num2Night;
				break;
			case GameMode.SurvivalNormalStage3:
			case GameMode.SurvivalHardStage3:
			case GameMode.SurvivalEndlessStage3:
			case GameMode.ChallengeLittleTrouble:
			case GameMode.ChallengeBobsledBonanza:
			case GameMode.ChallengeSpeed:
			case GameMode.ChallengeLastStand:
			case GameMode.ChallengeWarAndPeas2:
			case GameMode.Upsell:
			case GameMode.Intro:
			case GameMode.ChallengeColumn4:
			case GameMode.SurvivalHellStage3:
			case GameMode.ChallengeBobsledBonanza2:
			case GameMode.ChallengeFusion:
				mBackground = BackgroundType.Num3Pool;
				break;
			case GameMode.SurvivalNormalStage4:
			case GameMode.SurvivalHardStage4:
			case GameMode.SurvivalEndlessStage4:
			case GameMode.ChallengeRainingSeeds:
			case GameMode.ChallengeInvisighoul:
			case GameMode.ChallengeAirRaid:
			case GameMode.ChallengeStormyNight:
			case GameMode.ChallengeAttackOnTitans:
			case GameMode.MarvelousPeople2:
			case GameMode.SurvivalHellStage4:
				mBackground = BackgroundType.Num4Fog;
				break;
			case GameMode.SurvivalNormalStage5:
			case GameMode.SurvivalHardStage5:
			case GameMode.SurvivalEndlessStage5:
			case GameMode.ChallengeColumn:
			case GameMode.ChallengePogoParty:
			case GameMode.ChallengeHighGravity:
			case GameMode.ChallengeBungeeBlitz:
			case GameMode.ChallengeAttackOnTitans3:
			case GameMode.MarvelousPeople3:
			case GameMode.SurvivalHellStage5:
			case GameMode.ChallengeMoreAirRaid:
				mBackground = BackgroundType.Num5Roof;
				break;
			case GameMode.ChallengeFinalBoss:
				mBackground = BackgroundType.Num6Boss;
				break;
			case GameMode.ImitaterRandom:
				mBackground = BackgroundType.ShallowDay;
				break;
			case GameMode.PoolParty:
			case GameMode.BigPoolSurvivalNormalStage:
			case GameMode.BigPoolSurvivalHardStage:
			case GameMode.BigPoolSurvivalHellStage:
				mBackground = BackgroundType.BigPool;
				break;
			case GameMode.ChallengeZenGarden:
				mBackground = BackgroundType.Greenhouse;
				break;
			case GameMode.TreeOfWisdom:
				mBackground = BackgroundType.TreeOfWisdom;
				break;
			case GameMode.ChallengeZombiquarium:
				mBackground = BackgroundType.Zombiquarium;
				break;
			case GameMode.RogueConveyorbelt:
			case GameMode.RogueConveyorbeltHard:
				mBackground = (((mChallenge.mSurvivalStage & 1) == 0) ? BackgroundType.Num3Pool : BackgroundType.Num4Fog);
				break;
			case GameMode.ChallengeStageRandom:
			{
				int levelRandSeed = GetLevelRandSeed();
				RandomNumbers.Seed(levelRandSeed);
				switch (RandomNumbers.NextNumber(7))
				{
				case 0:
					mBackground = BackgroundType.Num1Day;
					break;
				case 1:
					mBackground = BackgroundType.Num2Night;
					break;
				case 2:
					mBackground = BackgroundType.Num3Pool;
					break;
				case 3:
					mBackground = BackgroundType.Num4Fog;
					break;
				case 4:
					mBackground = BackgroundType.Num5Roof;
					break;
				case 5:
					mBackground = BackgroundType.HighGround;
					break;
				case 6:
					mBackground = BackgroundType.BigPool;
					break;
				}
				break;
			}
			case GameMode.OccupyHighGround:
				mBackground = BackgroundType.HighGround;
				break;
			default:
				Debug.ASSERT(value: false);
				break;
			}
		}
		LoadBackgroundImages();
		if (mBackground == BackgroundType.Num1Day || mBackground == BackgroundType.Greenhouse || mBackground == BackgroundType.TreeOfWisdom)
		{
			mPlantRow[0] = PlantRowType.Normal;
			mPlantRow[1] = PlantRowType.Normal;
			mPlantRow[2] = PlantRowType.Normal;
			mPlantRow[3] = PlantRowType.Normal;
			mPlantRow[4] = PlantRowType.Normal;
			mPlantRow[5] = PlantRowType.Dirt;
			if (mApp.IsAdventureMode() && mApp.IsFirstTimeAdventureMode())
			{
				if (mLevel == 1)
				{
					mPlantRow[0] = PlantRowType.Dirt;
					mPlantRow[1] = PlantRowType.Dirt;
					mPlantRow[3] = PlantRowType.Dirt;
					mPlantRow[4] = PlantRowType.Dirt;
				}
				else if (mLevel == 2 || mLevel == 3)
				{
					mPlantRow[0] = PlantRowType.Dirt;
					mPlantRow[4] = PlantRowType.Dirt;
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengeResodded)
			{
				mPlantRow[0] = PlantRowType.Dirt;
				mPlantRow[4] = PlantRowType.Dirt;
			}
		}
		else if (mBackground == BackgroundType.Num2Night)
		{
			mPlantRow[0] = PlantRowType.Normal;
			mPlantRow[1] = PlantRowType.Normal;
			mPlantRow[2] = PlantRowType.Normal;
			mPlantRow[3] = PlantRowType.Normal;
			mPlantRow[4] = PlantRowType.Normal;
			mPlantRow[5] = PlantRowType.Dirt;
		}
		else if (mBackground == BackgroundType.Num3Pool || mBackground == BackgroundType.Zombiquarium)
		{
			mPlantRow[0] = PlantRowType.Normal;
			mPlantRow[1] = PlantRowType.Normal;
			mPlantRow[2] = PlantRowType.Pool;
			mPlantRow[3] = PlantRowType.Pool;
			mPlantRow[4] = PlantRowType.Normal;
			mPlantRow[5] = PlantRowType.Normal;
		}
		else if (mBackground == BackgroundType.Num4Fog)
		{
			mPlantRow[0] = PlantRowType.Normal;
			mPlantRow[1] = PlantRowType.Normal;
			mPlantRow[2] = PlantRowType.Pool;
			mPlantRow[3] = PlantRowType.Pool;
			mPlantRow[4] = PlantRowType.Normal;
			mPlantRow[5] = PlantRowType.Normal;
		}
		else if (mBackground == BackgroundType.Num5Roof || mBackground == BackgroundType.Num6Boss)
		{
			mPlantRow[0] = PlantRowType.Normal;
			mPlantRow[1] = PlantRowType.Normal;
			mPlantRow[2] = PlantRowType.Normal;
			mPlantRow[3] = PlantRowType.Normal;
			mPlantRow[4] = PlantRowType.Normal;
			mPlantRow[5] = PlantRowType.Dirt;
		}
		else if (mBackground == BackgroundType.ShallowDay)
		{
			mPlantRow[0] = PlantRowType.Shallow;
			mPlantRow[1] = PlantRowType.Shallow;
			mPlantRow[2] = PlantRowType.Shallow;
			mPlantRow[3] = PlantRowType.Shallow;
			mPlantRow[4] = PlantRowType.Shallow;
			mPlantRow[5] = PlantRowType.Dirt;
		}
		else if (mBackground == BackgroundType.HighGround)
		{
			mPlantRow[0] = PlantRowType.HighGround;
			mPlantRow[1] = PlantRowType.HighGround;
			mPlantRow[2] = PlantRowType.Normal;
			mPlantRow[3] = PlantRowType.Normal;
			mPlantRow[4] = PlantRowType.HighGround;
			mPlantRow[5] = PlantRowType.HighGround;
		}
		else if (mBackground == BackgroundType.BigPool)
		{
			mPlantRow[0] = PlantRowType.Pool;
			mPlantRow[1] = PlantRowType.Pool;
			mPlantRow[2] = PlantRowType.Pool;
			mPlantRow[3] = PlantRowType.Pool;
			mPlantRow[4] = PlantRowType.Pool;
			mPlantRow[5] = PlantRowType.Pool;
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		for (int i = 0; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j < Constants.MAX_GRIDSIZEY; j++)
			{
				if (mPlantRow[j] == PlantRowType.Dirt)
				{
					mGridSquareType[i, j] = GridSquareType.Dirt;
				}
				else if (mPlantRow[j] == PlantRowType.Pool && i >= 0 && i <= 8)
				{
					mGridSquareType[i, j] = GridSquareType.Pool;
				}
				else if (mPlantRow[j] == PlantRowType.HighGround && i >= 4 && i <= 8)
				{
					mGridSquareType[i, j] = GridSquareType.HighGround;
				}
				else if (mPlantRow[j] == PlantRowType.Shallow)
				{
					mGridSquareType[i, j] = GridSquareType.Shallow;
				}
			}
		}
		int levelRandSeed2 = GetLevelRandSeed();
		RandomNumbers.Seed(levelRandSeed2);
		if (StageHasGraveStones())
		{
			if (mApp.IsLevelUseJson() && !mApp.IsWhackAZombieLevel())
			{
				CSSpawnGraveStone component = mApp.mCreativeLevel.GetComponent<CSSpawnGraveStone>();
				if (component != null)
				{
					int[] mCountPerColumn = component.mCountPerColumn;
					for (int k = 0; k < Constants.GRIDSIZEX; k++)
					{
						if (mCountPerColumn[k] > 0 && mCountPerColumn[k] <= Constants.MAX_GRIDSIZEY)
						{
							AddGraveStones(k, mCountPerColumn[k]);
						}
					}
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengeGraveDanger)
			{
				AddGraveStones(6, TodCommon.RandRangeInt(1, 2));
				AddGraveStones(7, TodCommon.RandRangeInt(1, 3));
				AddGraveStones(8, TodCommon.RandRangeInt(2, 3));
			}
			else if (mApp.IsWhackAZombieLevel())
			{
				int theGraveCount = 9;
				if (mApp.IsLevelUseJson())
				{
					CSWhackAZombieLevel component2 = mApp.mCreativeLevel.GetComponent<CSWhackAZombieLevel>();
					if (component2 != null)
					{
						theGraveCount = component2.mInitGraveStoneCount;
					}
				}
				mChallenge.WhackAZombiePlaceGraves(theGraveCount);
			}
			else if (mBackground == BackgroundType.Num2Night)
			{
				if (mApp.IsSurvivalNormal(mApp.mGameMode))
				{
					AddGraveStones(5, 1);
					AddGraveStones(6, 1);
					AddGraveStones(7, 1);
					AddGraveStones(8, 2);
				}
				else if (mApp.mGameMode == GameMode.ChallengeColumn3)
				{
					for (int l = 1; l < 9; l++)
					{
						AddGraveStones(l, 5);
					}
				}
				else if (!mApp.IsAdventureMode() && !mApp.IsQuickPlayMode())
				{
					AddGraveStones(4, 1);
					AddGraveStones(5, 1);
					AddGraveStones(6, 2);
					AddGraveStones(7, 2);
					AddGraveStones(8, 3);
				}
				else if (mLevel == 11 || mLevel == 12 || mLevel == 13)
				{
					AddGraveStones(6, 1);
					AddGraveStones(7, 1);
					AddGraveStones(8, 2);
				}
				else if (mLevel == 14 || mLevel == 16 || mLevel == 18)
				{
					AddGraveStones(5, 1);
					AddGraveStones(6, 1);
					AddGraveStones(7, 2);
					AddGraveStones(8, 3);
				}
				else if (mLevel == 17 || mLevel == 19)
				{
					AddGraveStones(4, 1);
					AddGraveStones(5, 2);
					AddGraveStones(6, 2);
					AddGraveStones(7, 3);
					AddGraveStones(8, 3);
				}
				else if (mLevel >= 20)
				{
					AddGraveStones(3, 1);
					AddGraveStones(4, 2);
					AddGraveStones(5, 2);
					AddGraveStones(6, 2);
					AddGraveStones(7, 3);
					AddGraveStones(8, 3);
				}
				else
				{
					Debug.ASSERT(value: false);
				}
			}
			else if (mApp.mGameMode == GameMode.MarvelousPeople2)
			{
				AddGraveStones(3, 1);
				AddGraveStones(4, 2);
				AddGraveStones(5, 2);
				AddGraveStones(6, 2);
				AddGraveStones(7, 3);
				AddGraveStones(8, 3);
			}
		}
		PickSpecialGraveStone();
		InitCoverLayer();
	}

	public void InitZombieWaves()
	{
		Debug.ASSERT(value: true);
		Array.Clear(mZombieAllowed, 0, mZombieAllowed.Length);
		if (mApp.IsSurvivalMode() && mApp.IsLevelUseJson())
		{
			mChallenge.InitZombieWavesSurvivalComponent();
		}
		else if (mApp.IsLevelUseJson())
		{
			List<ZombieType> list = mApp.mCreativeLevel.GetComponent<CSLevelProperty>()?.mAllowedZombies;
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ZombieType zombieType = list[i];
					mZombieAllowed[(int)zombieType] = true;
				}
			}
			else
			{
				mZombieAllowed[0] = true;
				mZombieAllowed[2] = true;
				mZombieAllowed[4] = true;
			}
		}
		else if (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
		{
			InitZombieWavesForLevel(mLevel);
		}
		else
		{
			mChallenge.InitZombieWaves();
		}
		Debug.Log(DebugType.Info, $"{GetType()}: InitZombieWaves: Zombies allowed {{{string.Join(", ", from t in mZombieAllowed.Select((bool t, int num) =>
			{
				if (!t)
				{
					return (string)null;
				}
				ZombieType zombieType2 = (ZombieType)num;
				return zombieType2.ToString();
			})
			where t != null
			select t)}}}");
		PickZombieWaves();
		Debug.ASSERT(IsZombieWaveDistributionOk());
		mCurrentWave = 0;
		mTotalSpawnedWaves = 0;
		mApp.mKilledYetiAndRestarted = false;
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 2)
		{
			mZombieCountDown = 5000;
		}
		else if (mApp.IsSurvivalMode() && mChallenge.mSurvivalStage > 0)
		{
			mZombieCountDown = 600;
		}
		else
		{
			mZombieCountDown = 1800;
		}
		mZombieCountDownStart = mZombieCountDown;
		mZombieHealthToNextWave = -1;
		mZombieHealthWaveStart = 0;
		mLastBungeeWave = 0;
		mProgressMeterWidth = 0;
		mHugeWaveCountDown = 0;
		mLevelAwardSpawned = false;
	}

	public void InitSurvivalStage()
	{
		RefreshSeedPacketFromCursor();
		mApp.mSoundSystem.GamePause(theEnteringPause: true);
		FreezeEffectsForCutscene(theFreeze: true);
		mLevelComplete = false;
		if (mApp.IsRogueConveyorbeltLevel())
		{
			PickBackground();
			foreach (Plant mPlant in mPlants)
			{
				if (Plant.IsNocturnal(mPlant.mSeedType) && StageIsNight())
				{
					mPlant.SetSleeping(theIsAsleep: false);
				}
			}
		}
		InitZombieWaves();
		mApp.mGameScene = GameScenes.LevelIntro;
		mApp.ShowSeedChooserScreen();
		mCutScene.StartLevelIntro();
		mSeedBank.UpdateHeight();
		if (!HasConveyorBeltSeedBank())
		{
			CSSurvivalLevel cSSurvivalLevel = mApp.mCreativeLevel?.GetComponent<CSSurvivalLevel>();
			if (cSSurvivalLevel != null)
			{
				CSSeedBank component = mApp.mCreativeLevel.GetComponent<CSSeedBank>();
				if (component != null && component.mUserChoose)
				{
					for (int i = 0; i < 10; i++)
					{
						SeedPacket seedPacket = mSeedBank.mSeedPackets[i];
						if (mApp.mPlayerInfo.mClassicSeedChooser)
						{
							seedPacket.mX = GetSeedPacketPositionX(i);
							seedPacket.mY = 8;
						}
						else
						{
							seedPacket.mX = 0;
							seedPacket.mY = GetSeedPacketPositionY(i);
						}
						seedPacket.mPacketType = SeedType.None;
					}
				}
				else
				{
					for (int j = 0; j < 10; j++)
					{
						SeedPacket seedPacket2 = mSeedBank.mSeedPackets[j];
						seedPacket2.mActive = true;
					}
				}
			}
			else
			{
				for (int k = 0; k < 10; k++)
				{
					SeedPacket seedPacket3 = mSeedBank.mSeedPackets[k];
					if (mApp.mPlayerInfo.mClassicSeedChooser)
					{
						seedPacket3.mX = GetSeedPacketPositionX(k);
						seedPacket3.mY = 8;
					}
					else
					{
						seedPacket3.mX = 0;
						seedPacket3.mY = GetSeedPacketPositionY(k);
					}
					seedPacket3.mPacketType = SeedType.None;
					if (seedPacket3.mIndex == -1)
					{
						seedPacket3.mIndex = k;
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < 10; l++)
			{
				SeedPacket seedPacket4 = mSeedBank.mSeedPackets[l];
				seedPacket4.mActive = true;
			}
		}
		if (StageHasFog())
		{
			mFogBlownCountDown = 2000;
		}
		for (int m = 0; m < Constants.MAX_GRIDSIZEY; m++)
		{
			mWaveRowGotLawnMowered[m] = -100;
		}
	}

	public static int MakeRenderOrder(RenderLayer theRenderLayer, int theRow, int theLayerOffset)
	{
		return (int)(theRenderLayer + theLayerOffset + 10000 * theRow);
	}

	public void UpdateGame()
	{
		if (ZBCheatDialog.AdvancedPause)
		{
			ZBCheatHooks.AdvancedPauseUpdateGame(this);
			return;
		}
		UpdateGame_Orig();
	}

	private void UpdateGame_Orig()
	{
		UpdateGameObjects();
		if (StageHasFog() && mFogBlownCountDown > 0)
		{
			float num = 1065f - (float)LeftFogColumn() * 80f + (float)Constants.BOARD_EXTRA_ROOM;
			if (mApp.mGameScene == GameScenes.LevelIntro)
			{
				mFogOffset = TodCommon.TodAnimateCurveFloat(200, 0, mFogBlownCountDown, num, 0f, TodCurves.EaseOut);
			}
			else if (mFogBlownCountDown < 2000)
			{
				mFogOffset = TodCommon.TodAnimateCurveFloat(2000, 0, mFogBlownCountDown, num, 0f, TodCurves.EaseOut);
			}
			else if (mFogOffset < num)
			{
				mFogOffset = TodCommon.TodAnimateCurveFloat(-5, (int)num, (int)(mFogOffset * 1.1f), 0f, num, TodCurves.Linear);
			}
		}
		if (mApp.mGameScene != GameScenes.Playing && !mCutScene.ShouldRunUpsellBoard())
		{
			return;
		}
		mMainCounter++;
		UpdateSunSpawning();
		UpdateZombieSpawning();
		UpdateIce();
		UpdateWinds();
		if (mIceTrapCounter > 0)
		{
			mIceTrapCounter--;
			if (mIceTrapCounter == 0)
			{
				TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(mPoolSparklyParticleID);
				if (todParticleSystem != null)
				{
					todParticleSystem.mDontUpdate = false;
				}
			}
		}
		if (mFogBlownCountDown > 0)
		{
			mFogBlownCountDown--;
		}
		if (mAgavePowerfulCountdown > 0)
		{
			mAgavePowerfulCountdown--;
		}
		if (mEndoflamePowerfulCountdown > 0)
		{
			mEndoflamePowerfulCountdown--;
		}
		if (mSealedCountdown > 0)
		{
			mSealedCountdown--;
		}
		if (mMainCounter == 1)
		{
			if (mApp.IsFirstTimeAdventureMode() && mLevel == 1)
			{
				SetTutorialState(TutorialState.Level1PickUpPeashooter);
			}
			else if (mApp.IsFirstTimeAdventureMode() && mLevel == 2)
			{
				SetTutorialState(TutorialState.Level2PickUpSunflower);
				DisplayAdvice("[ADVICE_PLANT_SUNFLOWER1]", MessageStyle.TutorialLevel2, AdviceType.None);
				mTutorialTimer = 500;
			}
		}
		UpdateProgressMeter();
	}

	public void InitZombieWavesForLevel(int aForLevel)
	{
		if (mApp.IsWhackAZombieLevel())
		{
			mChallenge.InitZombieWaves();
			return;
		}
		if (mApp.IsWallnutBowlingLevel() && !mApp.IsFirstTimeAdventureMode())
		{
			mChallenge.InitZombieWaves();
			return;
		}
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			mZombieAllowed[(int)zombieType] = CanZombieSpawnOnLevel(zombieType, aForLevel);
		}
	}

	public uint SeedNotRecommendedForLevel(SeedType theSeedType)
	{
		uint theNumber = 0u;
		if (Plant.IsNocturnal(theSeedType) && !StageIsNight())
		{
			TodCommon.SetBit(ref theNumber, 0, 1);
		}
		if (theSeedType == SeedType.InstantCoffee && StageIsNight())
		{
			TodCommon.SetBit(ref theNumber, 7, 1);
		}
		if (theSeedType == SeedType.Gravebuster && !StageHasGraveStones())
		{
			TodCommon.SetBit(ref theNumber, 2, 1);
		}
		if (theSeedType == SeedType.Plantern && !StageHasFog() && !StageHasNinja())
		{
			TodCommon.SetBit(ref theNumber, 3, 1);
		}
		if (theSeedType == SeedType.Flowerpot && !StageHasRoof())
		{
			TodCommon.SetBit(ref theNumber, 4, 1);
		}
		if (StageHasRoof() && (theSeedType == SeedType.Spikeweed || theSeedType == SeedType.Spikerock))
		{
			TodCommon.SetBit(ref theNumber, 5, 1);
		}
		if (!StageHasPool() && (theSeedType == SeedType.Lilypad || theSeedType == SeedType.Tanglekelp || theSeedType == SeedType.Seashroom || theSeedType == SeedType.Cattail))
		{
			TodCommon.SetBit(ref theNumber, 1, 1);
		}
		if (StageHasBigPool() && (theSeedType == SeedType.Spikeweed || theSeedType == SeedType.Spikerock || theSeedType == SeedType.Endoflame))
		{
			TodCommon.SetBit(ref theNumber, 5, 1);
		}
		return theNumber;
	}

	public void DrawTopRightUI(Graphics g, TopRightUIDrawMode theDrawElements = TopRightUIDrawMode.All)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			if (mChallenge.mChallengeState == ChallengeState.ZenFading)
			{
				mMenuButton.mY = TodCommon.TodAnimateCurve(50, 0, mChallenge.mChallengeStateCounter, 2, -50, TodCurves.EaseInOut);
				mStoreButton.mX = TodCommon.TodAnimateCurve(50, 0, mChallenge.mChallengeStateCounter, Constants.ZenGardenStoreButtonX, 800, TodCurves.EaseInOut);
			}
			else
			{
				mMenuButton.mY = 2;
				mStoreButton.mX = Constants.ZenGardenStoreButtonX;
			}
		}
		if (mTutorialState == TutorialState.ZenGardenCompleted)
		{
			SexyColor flashingColor = TodCommon.GetFlashingColor(mMainCounter, 75);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(flashingColor);
		}
		if (theDrawElements.HasFlag(TopRightUIDrawMode.AccelerateButton) && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mAccelerateButton != null && !mAccelerateButton.mBtnNoDraw)
		{
			mAccelerateButton.Draw(g);
			bool flag = mAccelerateButton.mIsDown && mAccelerateButton.mIsOver && !mAccelerateButton.mDisabled;
			int num = mAccelerationNumerator;
			int num2 = mAccelerationDenominator;
			(Image, int, int) tuple;
			switch (num)
			{
			case 1:
				if (num2 != 1)
				{
					goto default;
				}
				tuple = (AtlasResources.IMAGE_ACCELERATION_01, 4, 0);
				break;
			case 2:
				if (num2 != 1)
				{
					goto default;
				}
				tuple = (AtlasResources.IMAGE_ACCELERATION_02, 4, 0);
				break;
			case 3:
				if (num2 != 1)
				{
					goto default;
				}
				tuple = (AtlasResources.IMAGE_ACCELERATION_03, 4, 0);
				break;
			case 0:
				tuple = (AtlasResources.IMAGE_ACCELERATION_PAUSE, 12, 5);
				break;
			default:
				tuple = (null, 10, -3);
				break;
			}
			(Image, int, int) tuple2 = tuple;
			Image image = tuple2.Item1;
			int item = tuple2.Item2;
			int item2 = tuple2.Item3;
			bool flag2 = false;
			if (image == null)
			{
				image = AtlasResources.IMAGE_REANIM_ZOMBIE_QUESTIONMARK;
				flag2 = true;
			}
			int num3 = mAccelerateButton.mX + item + (flag ? 1 : 0);
			int num4 = mAccelerateButton.mY + item2 + (flag ? 1 : 0);
			g.DrawImage(image, num3, num4);
			if (flag2)
			{
				TodCommon.TodDrawString(g, $"{(float)mAccelerationNumerator / (float)mAccelerationDenominator: 0.##}", num3 + 10, num4 + 25, Resources.FONT_DWARVENTODCRAFT12, SexyColor.White, DrawStringJustification.Center);
			}
		}
		if (theDrawElements.HasFlag(TopRightUIDrawMode.PauseButton) && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mPauseButton != null && !mPauseButton.mBtnNoDraw)
		{
			mPauseButton.Draw(g);
			bool flag3 = mPauseButton.mIsDown && mPauseButton.mIsOver && !mPauseButton.mDisabled;
			int theX = mPauseButton.mX + 12 + (flag3 ? 1 : 0);
			int theY = mPauseButton.mY + 5 + (flag3 ? 1 : 0);
			g.DrawImage(AtlasResources.IMAGE_ACCELERATION_PAUSE, theX, theY);
		}
		if (theDrawElements.HasFlag(TopRightUIDrawMode.MenuButton) && mChallenge.mChallengeState != ChallengeState.ZenPlantInteracting)
		{
			mMenuButton.Draw(g);
		}
		g.SetColorizeImages(colorizeImages: false);
		if (theDrawElements.HasFlag(TopRightUIDrawMode.StoreButton) && mStoreButton != null && mApp.mGameMode != GameMode.ChallengeLastStand && mChallenge.mChallengeState != ChallengeState.ZenPlantInteracting)
		{
			if (mTutorialState == TutorialState.ZenGardenVisitStore)
			{
				SexyColor flashingColor2 = TodCommon.GetFlashingColor(mMainCounter, 75);
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(flashingColor2);
			}
			mStoreButton.Draw(g);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public void DrawFog(Graphics g)
	{
		Image iMAGE_FOG = Resources.IMAGE_FOG;
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		for (int i = 0; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				int num = mGridCelFog[i, j];
				if (num != 0)
				{
					int num2 = mGridCelLook[i, j % Constants.MAX_GRIDSIZEY];
					int theCelCol = num2 % 8;
					float num3 = (float)(i * 80) + mFogOffset - 15f + (float)Constants.BOARD_EXTRA_ROOM;
					float num4 = (float)(j * 85) + 20f;
					int num5 = (int)(255f - (float)num2 * 1.5f);
					int num6 = 255 - num2;
					float num7 = (float)mMainCounter * (float)Math.PI * 2f / 900f;
					float num8 = (float)mMainCounter * (float)Math.PI * 2f / 500f;
					float num9 = 3f * (float)i * (float)Math.PI * 2f / (float)Constants.GRIDSIZEX;
					float num10 = 3f * (float)j * (float)Math.PI * 2f / 7f;
					float num11 = (float)(13.0 + 4.0 * Math.Sin(num10 + num7) + 8.0 * Math.Sin(num9 + num8));
					num5 -= (int)(num11 * 1.5f);
					num6 -= (int)num11;
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(num5, num5, num6, num));
					g.DrawImageCel(iMAGE_FOG, (int)(num3 * Constants.S), (int)(num4 * Constants.S), theCelCol, 0);
					if (i == Constants.GRIDSIZEX - 1)
					{
						int num12 = 120;
						g.DrawImageCel(iMAGE_FOG, (int)((num3 + (float)num12) * Constants.S), (int)(num4 * Constants.S), theCelCol, 0);
					}
					g.SetColorizeImages(colorizeImages: false);
				}
			}
		}
	}

	public void InitCoverLayer()
	{
		for (int i = 0; i < 6; i++)
		{
			int aRenderOrder = MakeRenderOrder(RenderLayer.CoverLayer, i, 0);
			float num = GameConstants.gCoverInfos[i].mX + (float)Constants.BOARD_EXTRA_ROOM;
			float theY = GameConstants.gCoverInfos[i].mY;
			float mScale = GameConstants.gCoverInfos[i].mScale;
			ReanimationType theReanimationType;
			switch (i)
			{
			case 0:
			case 3:
				theReanimationType = (StageIsNight() ? ReanimationType.NightBushes3 : ReanimationType.Bushes3);
				break;
			case 2:
			case 5:
				theReanimationType = (StageIsNight() ? ReanimationType.NightBushes4 : ReanimationType.Bushes4);
				break;
			default:
				theReanimationType = (StageIsNight() ? ReanimationType.NightBushes5 : ReanimationType.Bushes5);
				if (i == 4)
				{
					num -= 15f;
				}
				break;
			}
			Reanimation reanimation = mApp.AddReanimation(num, theY, aRenderOrder, theReanimationType);
			reanimation.mIsAttachment = true;
			reanimation.OverrideScale(mScale, mScale);
			reanimation.mAnimRate = 0f;
			mCoverLayerAnimIDs[i] = mApp.ReanimationTryToGet(reanimation);
		}
	}

	public void UpdateCoverLayer()
	{
		for (int i = 0; i < mCoverLayerAnimIDs.Length; i++)
		{
			mApp.ReanimationTryToGet(mCoverLayerAnimIDs[i])?.Update();
		}
	}

	public void DrawCoverLayer(Graphics g, int theRow)
	{
		if (mBackground == BackgroundType.Num1Day || mBackground == BackgroundType.Num2Night || mBackground == BackgroundType.Num3Pool || mBackground == BackgroundType.Num4Fog || mBackground == BackgroundType.ShallowDay || mBackground == BackgroundType.HighGround || mBackground == BackgroundType.BigPool)
		{
			mApp.ReanimationTryToGet(mCoverLayerAnimIDs[theRow])?.Draw(g);
		}
		if (theRow == 6)
		{
			if (mBackground == BackgroundType.Num5Roof)
			{
				g.DrawImage(Resources.IMAGE_ROOF_TREES, (int)(Constants.ISUI * 628f) - Constants.BoardOffsetChange + (mX - Constants.Board_Offset_AspectRatio_Correction) * 3, 0);
				g.DrawImage(Resources.IMAGE_ROOF_POLE, (int)(Constants.ISUI * 628f) - Constants.BoardOffsetChange + (mX - Constants.Board_Offset_AspectRatio_Correction) * 4, 0);
			}
			else if (mBackground == BackgroundType.Num6Boss && !mApp.IsFinalBossLevel())
			{
				g.DrawImage(Resources.IMAGE_ROOF_TREES_NIGHT, (int)(Constants.ISUI * 628f) - Constants.BoardOffsetChange + (mX - Constants.Board_Offset_AspectRatio_Correction) * 3, 0);
				g.DrawImage(Resources.IMAGE_ROOF_POLE_NIGHT, (int)(Constants.ISUI * 628f) - Constants.BoardOffsetChange + (mX - Constants.Board_Offset_AspectRatio_Correction) * 4, 0);
			}
		}
	}

	public void UpdateFog()
	{
		if (!StageHasFog())
		{
			return;
		}
		int num = 3;
		if (mFogBlownCountDown > 0 && mFogBlownCountDown < 2000)
		{
			num = 1;
		}
		else if (mFogBlownCountDown > 0)
		{
			num = 20;
		}
		int num2 = LeftFogColumn();
		for (int i = num2; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				int val = 255;
				if (i == num2)
				{
					val = 200;
				}
				mGridCelFog[i, j] = Math.Min(mGridCelFog[i, j] + num, val);
			}
		}
		int count = mPlants.Count;
		for (int k = 0; k < count; k++)
		{
			Plant plant = mPlants[k];
			if (plant.mDead || plant.NotOnGround() || plant.IsDisabled())
			{
				continue;
			}
			if (plant.mSeedType == SeedType.Plantern)
			{
				if (plant.mState == PlantState.PlanternHenshinBegin)
				{
					if (plant.mStateCountdown >= 50)
					{
						ClearFogAroundPlant(plant, 3);
					}
					else if (plant.mStateCountdown < 50)
					{
						ClearFogAroundPlant(plant, 18);
					}
				}
				else if (plant.mState == PlantState.PlanternHenshinOver)
				{
					ClearFogAroundPlant(plant, 18);
				}
				else
				{
					ClearFogAroundPlant(plant, 4);
				}
			}
			else if (plant.mSeedType == SeedType.Torchwood || plant.mSeedType == SeedType.FireShroom)
			{
				ClearFogAroundPlant(plant, 1);
			}
		}
	}

	public int LeftFogColumn()
	{
		if (mApp.IsLevelUseJson())
		{
			CSSpawnFog component = mApp.mCreativeLevel.GetComponent<CSSpawnFog>();
			if (component != null)
			{
				return 9 - component.mColumn;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeAirRaid)
		{
			return 6;
		}
		if (!mApp.IsAdventureMode() && !mApp.IsQuickPlayMode())
		{
			return 5;
		}
		if (mLevel == 31)
		{
			return 6;
		}
		if (mLevel >= 32 && mLevel <= 36)
		{
			return 5;
		}
		if (mLevel >= 37 && mLevel <= 40)
		{
			return 4;
		}
		Debug.ASSERT(value: false);
		return -666;
	}

	public static bool IsZombieTypePoolOnly(ZombieType theZombieType)
	{
		if (theZombieType != ZombieType.Snorkel)
		{
			return theZombieType == ZombieType.DolphinRider;
		}
		return true;
	}

	public CoinType DropLootTypeToCoinType(DropLootType theDropLootType)
	{
		return theDropLootType switch
		{
			DropLootType.None => CoinType.None, 
			DropLootType.Silver => CoinType.Silver, 
			DropLootType.Gold => CoinType.Gold, 
			DropLootType.Diamond => CoinType.Diamond, 
			DropLootType.Sun => CoinType.Sun, 
			DropLootType.Smallsun => CoinType.Smallsun, 
			DropLootType.Largesun => CoinType.Largesun, 
			DropLootType.PresentPlant => CoinType.PresentPlant, 
			DropLootType.Chocolate => CoinType.Chocolate, 
			DropLootType.PresentMinigames => CoinType.PresentMinigames, 
			DropLootType.PresentPuzzleMode => CoinType.PresentPuzzleMode, 
			_ => CoinType.None, 
		};
	}

	public bool CanDropPresentMinigames()
	{
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 22 && mCurrentWave > 5 && !mApp.mPlayerInfo.mHasUnlockedMinigames)
		{
			return CountCoinByType(CoinType.PresentMinigames) == 0;
		}
		return false;
	}

	public bool CanDropPresentPuzzleMode()
	{
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 36 && mCurrentWave > 5 && !mApp.mPlayerInfo.mHasUnlockedPuzzleMode)
		{
			return CountCoinByType(CoinType.PresentPuzzleMode) == 0;
		}
		return false;
	}

	public void DropLootPiece(int thePosX, int thePosY, int theDropFactor)
	{
		if (CanDropPresentMinigames())
		{
			mApp.PlayFoley(FoleyType.ArtChallenge);
			AddCoin(thePosX - 40, thePosY, CoinType.PresentMinigames, CoinMotion.Coin);
			return;
		}
		if (CanDropPresentPuzzleMode())
		{
			mApp.PlayFoley(FoleyType.ArtChallenge);
			AddCoin(thePosX - 40, thePosY, CoinType.PresentPuzzleMode, CoinMotion.Coin);
			return;
		}
		int num = RandomNumbers.NextNumber(30000);
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 11 && !mDroppedFirstCoin && mCurrentWave > 5)
		{
			num = 1000;
		}
		if (mApp.IsColumnLevel())
		{
			num *= 5;
		}
		if (mApp.IsWhackAZombieLevel())
		{
			int num2 = 2500;
			int num3;
			if (mSunMoney > 500)
			{
				num3 = num2 + 300;
			}
			else if (mSunMoney > 350)
			{
				num3 = num2 + 600;
			}
			else
			{
				num3 = ((mSunMoney <= 200) ? (num2 + 2500) : (num2 + 1200));
			}
			if (num >= num2 * theDropFactor && num < num3 * theDropFactor)
			{
				mApp.PlayFoley(FoleyType.SpawnSun);
				AddCoin(thePosX - 20, thePosY, CoinType.Sun, CoinMotion.Coin);
				AddCoin(thePosX - 40, thePosY, CoinType.Sun, CoinMotion.Coin);
				AddCoin(thePosX - 60, thePosY, CoinType.Sun, CoinMotion.Coin);
				return;
			}
		}
		if (mTotalSpawnedWaves > 70)
		{
			return;
		}
		int num4 = (mApp.mZenGarden.CanDropPottedPlantLoot() ? (((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && !mApp.IsFirstTimeAdventureMode()) ? 24 : ((!mApp.IsSurvivalEndless(mApp.mGameMode)) ? 12 : 3)) : 0);
		if (mApp.IsLevelWithHighPresentPlantDropRate(mApp.mGameMode))
		{
			num4 = (int)Math.Ceiling((float)num4 * 1.5f);
		}
		int num5 = num4;
		if (mApp.mZenGarden.CanDropChocolate())
		{
			if ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && !mApp.IsFirstTimeAdventureMode())
			{
				num5 += 72;
			}
			else
			{
				num5 = ((!mApp.IsSurvivalEndless(mApp.mGameMode)) ? (num5 + 36) : (num5 + 9));
			}
		}
		int num6 = 14 + num5;
		int num7 = 250 + num5;
		int num8 = 2500 + num5;
		CoinType coinType;
		if (num < num4 * theDropFactor)
		{
			coinType = CoinType.PresentPlant;
		}
		else if (num < num5 * theDropFactor)
		{
			coinType = CoinType.Chocolate;
		}
		else if (num < num6 * theDropFactor)
		{
			coinType = CoinType.Diamond;
		}
		else if (num < num7 * theDropFactor)
		{
			coinType = CoinType.Gold;
		}
		else
		{
			if (num >= num8 * theDropFactor)
			{
				return;
			}
			coinType = CoinType.Silver;
		}
		if (coinType == CoinType.Diamond && mApp.mPlayerInfo.mPurchases[21] < 1)
		{
			coinType = CoinType.Gold;
		}
		if (mApp.IsWallnutBowlingLevel() && (coinType == CoinType.Silver || coinType == CoinType.Gold || coinType == CoinType.Diamond))
		{
			return;
		}
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 11)
		{
			int num9 = Coin.GetCoinValue(CoinType.Gold) * mLawnMowers.Count;
			int itemCost = StoreScreen.GetItemCost(StoreItem.STORE_ITEM_PACKET_UPGRADE);
			int num10 = mApp.mPlayerInfo.mCoins + CountCoinsBeingCollected();
			if (Coin.GetCoinValue(coinType) + num10 + num9 >= itemCost)
			{
				return;
			}
		}
		mApp.PlayFoley(FoleyType.SpawnSun);
		AddCoin(thePosX - 40, thePosY, coinType, CoinMotion.Coin);
		mDroppedFirstCoin = true;
	}

	public DropLootType DropLootPieceWithResult(int theDropFactor)
	{
		if (CanDropPresentMinigames())
		{
			return DropLootType.PresentMinigames;
		}
		if (CanDropPresentPuzzleMode())
		{
			return DropLootType.PresentPuzzleMode;
		}
		int num = RandomNumbers.NextNumber(30000);
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 11 && !mDroppedFirstCoin && mCurrentWave > 5)
		{
			num = 1000;
		}
		if (mApp.IsColumnLevel())
		{
			num *= 5;
		}
		if (mApp.IsWhackAZombieLevel())
		{
			int num2 = 2500;
			int num3;
			if (mSunMoney > 500)
			{
				num3 = num2 + 300;
			}
			else if (mSunMoney > 350)
			{
				num3 = num2 + 600;
			}
			else
			{
				num3 = ((mSunMoney <= 200) ? (num2 + 2500) : (num2 + 1200));
			}
			if (num >= num2 * theDropFactor && num < num3 * theDropFactor)
			{
				return DropLootType.ThreeSun;
			}
		}
		if (mTotalSpawnedWaves > 70)
		{
			return DropLootType.None;
		}
		int num4 = (mApp.mZenGarden.CanDropPottedPlantLoot() ? (((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && !mApp.IsFirstTimeAdventureMode()) ? 24 : ((!mApp.IsSurvivalEndless(mApp.mGameMode)) ? 12 : 3)) : 0);
		int num5 = num4;
		if (mApp.mZenGarden.CanDropChocolate())
		{
			if ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && !mApp.IsFirstTimeAdventureMode())
			{
				num5 += 72;
			}
			else
			{
				num5 = ((!mApp.IsSurvivalEndless(mApp.mGameMode)) ? (num5 + 36) : (num5 + 9));
			}
		}
		int num6 = 14 + num5;
		int num7 = 250 + num5;
		int num8 = 2500 + num5;
		DropLootType dropLootType;
		if (num < num4 * theDropFactor)
		{
			dropLootType = DropLootType.PresentPlant;
		}
		else if (num < num5 * theDropFactor)
		{
			dropLootType = DropLootType.Chocolate;
		}
		else if (num < num6 * theDropFactor)
		{
			dropLootType = DropLootType.Diamond;
		}
		else if (num < num7 * theDropFactor)
		{
			dropLootType = DropLootType.Gold;
		}
		else
		{
			if (num >= num8 * theDropFactor)
			{
				return DropLootType.None;
			}
			dropLootType = DropLootType.Silver;
		}
		if (dropLootType == DropLootType.Diamond && mApp.mPlayerInfo.mPurchases[21] < 1)
		{
			dropLootType = DropLootType.Gold;
		}
		if (mApp.IsWallnutBowlingLevel() && (dropLootType == DropLootType.Silver || dropLootType == DropLootType.Gold || dropLootType == DropLootType.Diamond))
		{
			return DropLootType.None;
		}
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 11)
		{
			CoinType theType = DropLootTypeToCoinType(dropLootType);
			int num9 = Coin.GetCoinValue(CoinType.Gold) * mLawnMowers.Count;
			int itemCost = StoreScreen.GetItemCost(StoreItem.STORE_ITEM_PACKET_UPGRADE);
			int num10 = mApp.mPlayerInfo.mCoins + CountCoinsBeingCollected();
			if (Coin.GetCoinValue(theType) + num10 + num9 >= itemCost)
			{
				return DropLootType.None;
			}
		}
		return dropLootType;
	}

	public void UpdateLevelEndSequence()
	{
		if (mNextSurvivalStageCounter > 0)
		{
			if (!IsScaryPotterDaveTalking())
			{
				mNextSurvivalStageCounter--;
				if (mApp.IsAdventureMode() && mApp.IsScaryPotterLevel() && mNextSurvivalStageCounter == 300)
				{
					mApp.CrazyDaveEnter();
					if (mChallenge.mSurvivalStage == 0)
					{
						mApp.CrazyDaveTalkIndex(2700);
					}
					else
					{
						mApp.CrazyDaveTalkIndex(2800);
					}
					mChallenge.PuzzleNextStageClear();
					mNextSurvivalStageCounter = 100;
				}
			}
			if (mNextSurvivalStageCounter == 1 && mApp.IsSurvivalMode() && mApp.IsSurvivalMode())
			{
				TryToSaveGame();
			}
			if (mNextSurvivalStageCounter == 0)
			{
				if (!mApp.IsScaryPotterLevel() || !mApp.IsAdventureMode())
				{
					if (mApp.IsScaryPotterLevel() && !IsFinalScaryPotterStage())
					{
						mChallenge.PuzzleNextStageClear();
						mChallenge.ScaryPotterPopulate();
					}
					else if (mApp.IsCornRouletteLevel())
					{
						mApp.PlaySample(Resources.SOUND_HUGE_WAVE);
						mNextSurvivalStageCounter = 0;
						ClearAdviceImmediately();
						mApp.AddTodParticle(400f, 300f, 400000, ParticleEffect.ScreenFlash);
						mChallenge.CornRouletteRoundSetup();
					}
					else if (mApp.IsEndlessIZombie(mApp.mGameMode))
					{
						mChallenge.PuzzleNextStageClear();
						mChallenge.IZombieInitLevel();
					}
					else if (mApp.mGameMode == GameMode.ChallengeLastStand)
					{
						ClearAdvice(AdviceType.None);
					}
					else
					{
						mLevelComplete = true;
						RemoveZombiesForRepick();
					}
				}
				return;
			}
		}
		if (mBoardFadeOutCounter < 0)
		{
			return;
		}
		mBoardFadeOutCounter--;
		if (mBoardFadeOutCounter == 0)
		{
			mLevelComplete = true;
			return;
		}
		if (mBoardFadeOutCounter == 300)
		{
			bool flag = IsSurvivalStageWithRepick();
			bool flag2 = mLevel == 9 || mLevel == 19 || mLevel == 29 || mLevel == 39 || mLevel == 49;
			if (!flag && !flag2)
			{
				mApp.PlaySample(Resources.SOUND_LIGHTFILL);
			}
		}
		if (mScoreNextMowerCounter > 0)
		{
			mScoreNextMowerCounter--;
			if (mScoreNextMowerCounter != 0)
			{
				return;
			}
		}
		if (!CanDropLoot() || IsSurvivalStageWithRepick())
		{
			return;
		}
		mScoreNextMowerCounter = 40;
		LawnMower bottomLawnMower = GetBottomLawnMower();
		if (bottomLawnMower != null)
		{
			CoinType theCoinType = (mApp.IsSurvivalHell(mApp.mGameMode) ? CoinType.Diamond : CoinType.Gold);
			AddCoin((int)(bottomLawnMower.mPosX + (float)Constants.LawnMower_Coin_Offset.X), (int)(bottomLawnMower.mPosY + (float)Constants.LawnMower_Coin_Offset.Y), theCoinType, CoinMotion.LawnmowerCoin);
			SoundInstance soundInstance = mApp.mSoundManager.GetSoundInstance((uint)Resources.SOUND_POINTS);
			if (soundInstance != null)
			{
				soundInstance.Play(looping: false, autoRelease: true);
				float num = TodCommon.ClampFloat(6f - (float)CountUntriggerLawnMowers(), 0f, 6f);
				soundInstance.AdjustPitch(num);
			}
			else
			{
				Debug.Log(DebugType.Error, "FAILED TO PLAY SOUND INSTANCE");
			}
			bottomLawnMower.Die();
		}
	}

	public LawnMower GetBottomLawnMower()
	{
		LawnMower lawnMower = null;
		LawnMower theLawnMower = null;
		while (IterateLawnMowers(ref theLawnMower))
		{
			if (theLawnMower.mMowerState != LawnMowerState.Triggered && theLawnMower.mMowerState != LawnMowerState.Squished && (lawnMower == null || lawnMower.mRow < theLawnMower.mRow))
			{
				lawnMower = theLawnMower;
			}
		}
		return lawnMower;
	}

	public bool CanDropLoot()
	{
		if (!mCutScene.ShouldRunUpsellBoard())
		{
			if (mApp.IsFirstTimeAdventureMode())
			{
				return mLevel >= 11;
			}
			return true;
		}
		return false;
	}

	public ZombieType GetIntroducedZombieType()
	{
		if ((!mApp.IsAdventureMode() && !mApp.IsQuickPlayMode()) || mLevel == 1)
		{
			return ZombieType.Invalid;
		}
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
			if ((zombieType != ZombieType.Yeti || mApp.CanSpawnYetis()) && zombieDefinition.mStartingLevel == mLevel && ((!mApp.IsAdventureMode() && !mApp.IsQuickPlayMode()) || mApp.IsAdventureModeHard() || zombieType < ZombieType.RobotTitan))
			{
				return zombieType;
			}
		}
		return ZombieType.Invalid;
	}

	public void PickSpecialGraveStone()
	{
		int num = 0;
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Gravestone)
			{
				Debug.ASSERT(num < MAX_GRAVE_STONES);
				aPicks[num] = theGridItem;
				num++;
			}
		}
		if (num != 0)
		{
			GridItem gridItem = aPicks[RandomNumbers.NextNumber(num)];
			gridItem.mGridItemState = GridItemState.GravestoneSpecial;
		}
	}

	public float GetPosYBasedOnRow(float thePosX, int theRow)
	{
		if (StageHasRoof())
		{
			float num = 0f;
			int num2 = 440 + Constants.BOARD_EXTRA_ROOM;
			if (thePosX < (float)num2)
			{
				num = ((float)num2 - thePosX) * 0.25f;
			}
			return (float)GridToPixelY(8, theRow) + num;
		}
		return GridToPixelY(0, theRow);
	}

	public void NextWaveComing()
	{
		if (mCurrentWave + 1 == mNumWaves)
		{
			bool flag = true;
			if (IsSurvivalStageWithRepick())
			{
				flag = false;
			}
			else if (mApp.mGameMode == GameMode.ChallengeLastStand)
			{
				flag = false;
			}
			else if (mApp.IsContinuousChallenge())
			{
				flag = false;
			}
			if (flag)
			{
				mApp.AddReanimation(0f + (float)Constants.BOARD_EXTRA_ROOM, 30f, 800000, ReanimationType.FinalWave);
				mFinalWaveSoundCounter = 60;
			}
		}
		if (mCurrentWave == 0)
		{
			mApp.PlaySample(Resources.SOUND_AWOOGA);
		}
		else if (mApp.IsWhackAZombieLevel())
		{
			if (mCurrentWave == mNumWaves - 1)
			{
				mApp.PlaySample(Resources.SOUND_SIREN);
			}
		}
		else if (IsFlagWave(mCurrentWave))
		{
			mApp.PlaySample(Resources.SOUND_SIREN);
		}
	}

	public bool BungeeIsTargetingCell(int theCol, int theRow)
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying() && zombie.mZombieType == ZombieType.Bungee && zombie.mRow == theRow && zombie.mTargetCol == theCol)
			{
				return true;
			}
		}
		return false;
	}

	public int PlantingPixelToGridX(int theX, int theY, SeedType theSeedType)
	{
		int theY2 = theY;
		OffsetYForPlanting(ref theY2, theSeedType);
		return PixelToGridX(theX, theY2);
	}

	public int PlantingPixelToGridY(int theX, int theY, SeedType theSeedType)
	{
		int theY2 = theY;
		OffsetYForPlanting(ref theY2, theSeedType);
		if (theSeedType == SeedType.InstantCoffee)
		{
			int theGridX = PixelToGridX(theX, theY2);
			int num = PixelToGridY(theX, theY2);
			Plant topPlantAt = GetTopPlantAt(theGridX, num, TopPlant.OnlyNormalPosition);
			if (topPlantAt != null && topPlantAt.mIsAsleep)
			{
				return num;
			}
			num = PixelToGridY(theX, theY2 + 30);
			Plant topPlantAt2 = GetTopPlantAt(theGridX, num, TopPlant.OnlyNormalPosition);
			if (topPlantAt2 != null && topPlantAt2.mIsAsleep)
			{
				return num;
			}
			num = PixelToGridY(theX, theY2 - 50);
			Plant topPlantAt3 = GetTopPlantAt(theGridX, num, TopPlant.OnlyNormalPosition);
			if (topPlantAt3 != null && topPlantAt3.mIsAsleep)
			{
				return num;
			}
		}
		return PixelToGridY(theX, theY2);
	}

	public Plant FindUmbrellaPlant(int theGridX, int theGridY)
	{
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.mSeedType == SeedType.Umbrella && !plant.NotOnGround() && theGridX >= plant.mPlantCol - 1 && theGridX <= plant.mPlantCol + 1 && theGridY >= plant.mRow - 1 && theGridY <= plant.mRow + 1 && !plant.IsDisabled())
			{
				return plant;
			}
		}
		return null;
	}

	public void SetTutorialState(TutorialState theTutorialState)
	{
		switch (theTutorialState)
		{
		case TutorialState.Level1PickUpPeashooter:
			if (mPlants.Count == 0)
			{
				float screenX2 = (float)(mSeedBank.mX + mSeedBank.mSeedPackets[0].mX + Constants.SMALL_SEEDPACKET_WIDTH / 2) - Constants.InvertAndScale(13f);
				float screenY2 = 0f;
				if (mCameraEnabled && mCamera != null)
				{
					mCamera?.ScreenToBoardReplace(ref screenX2, ref screenY2);
				}
				TutorialArrowShow((int)screenX2, (int)screenY2);
				DisplayAdvice("[ADVICE_CLICK_SEED_PACKET]", MessageStyle.TutorialLevel1Stay, AdviceType.None);
			}
			else
			{
				DisplayAdvice("[ADVICE_ENOUGH_SUN]", MessageStyle.TutorialLevel1Stay, AdviceType.None);
				mTutorialTimer = 400;
			}
			break;
		case TutorialState.Level1PlantPeashooter:
			mTutorialTimer = -1;
			TutorialArrowRemove();
			if (mPlants.Count == 0)
			{
				DisplayAdvice("[ADVICE_CLICK_ON_GRASS]", MessageStyle.TutorialLevel1Stay, AdviceType.None);
			}
			else
			{
				ClearAdvice(AdviceType.None);
			}
			break;
		case TutorialState.Level1RefreshPeashooter:
			DisplayAdvice("[ADVICE_PLANTED_PEASHOOTER]", MessageStyle.TutorialLevel1Stay, AdviceType.None);
			mSunCountDown = 400;
			break;
		case TutorialState.Level1Completed:
			DisplayAdvice("[ADVICE_ZOMBIE_ONSLAUGHT]", MessageStyle.TutorialLevel1, AdviceType.None);
			mZombieCountDown = 99;
			mZombieCountDownStart = mZombieCountDown;
			break;
		case TutorialState.Level2PickUpSunflower:
		case TutorialState.MoresunPickUpSunflower:
		{
			float screenX3 = mSeedBank.mX + mSeedBank.mSeedPackets[1].mX;
			float screenY3 = mSeedBank.mY + mSeedBank.mSeedPackets[1].mY;
			if (mCameraEnabled && mCamera != null)
			{
				mCamera?.ScreenToBoardReplace(ref screenX3, ref screenY3);
			}
			TutorialArrowShow((int)(screenX3 + Constants.InvertAndScaleUI(13f)), (int)screenY3);
			break;
		}
		case TutorialState.Level2PlantSunflower:
		case TutorialState.Level2RefreshSunflower:
		case TutorialState.MoresunPlantSunflower:
		case TutorialState.MoresunRefreshSunflower:
			TutorialArrowRemove();
			break;
		case TutorialState.Level2Completed:
			if (mCurrentWave == 0)
			{
				mZombieCountDown = 999;
				mZombieCountDownStart = mZombieCountDown;
			}
			break;
		case TutorialState.SlotMachinePull:
			DisplayAdvice("[ADVICE_SLOT_MACHINE_PULL]", MessageStyle.SlotMachine, AdviceType.SlotMachinePull);
			break;
		case TutorialState.SlotMachineCompleted:
			ClearAdvice(AdviceType.SlotMachinePull);
			break;
		case TutorialState.ShovelPickup:
		{
			DisplayAdvice("[ADVICE_CLICK_SHOVEL]", MessageStyle.HintStay, AdviceType.None);
			int screenX = GetShovelButtonRect().mX;
			int screenY = 0;
			if (mCameraEnabled && mCamera != null)
			{
				mCamera?.ScreenToBoardReplace(ref screenX, ref screenY);
			}
			TutorialArrowShow(screenX + (int)Constants.InvertAndScale(16f) + Constants.Board_Offset_AspectRatio_Correction, screenY);
			break;
		}
		case TutorialState.ShovelDig:
			DisplayAdvice("[ADVICE_CLICK_PLANT]", MessageStyle.HintStay, AdviceType.None);
			TutorialArrowRemove();
			break;
		case TutorialState.ShovelKeepDigging:
			DisplayAdvice("[ADVICE_KEEP_DIGGING]", MessageStyle.HintStay, AdviceType.None);
			break;
		case TutorialState.ShovelCompleted:
			ClearAdvice(AdviceType.None);
			mCutScene.mCutsceneTime = 1500;
			mCutScene.mCrazyDaveDialogStart = 2410;
			break;
		}
		mTutorialState = theTutorialState;
	}

	public void DoFwooshLine(int theX)
	{
		int num = (StageHas6Rows() ? 6 : 5);
		for (int i = 0; i < num; i++)
		{
			int num2 = (theX + 30) / 170;
			mApp.ReanimationTryToGet(mFwooshID[i, num2])?.ReanimationDie();
			Reanimation reanimation = mApp.AddReanimation(theX + 45, 50 + i * 100, MakeRenderOrder(RenderLayer.Particle, i, 1), ReanimationType.JalapenoFire);
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_flame);
			reanimation.mLoopType = ReanimLoopType.LoopFullLastFrame;
			reanimation.mAnimRate *= TodCommon.RandRangeFloat(0.7f, 1.3f);
			float num3 = TodCommon.RandRangeFloat(0.9f, 1.1f);
			float num4 = 1f;
			if (RandomNumbers.NextNumber(2) == 0)
			{
				num4 = -1f;
			}
			reanimation.OverrideScale(num3 * num4, 1f);
			Debug.ASSERT(reanimation != null);
			mFwooshID[i, num2] = mApp.ReanimationGetID(reanimation);
		}
		mFwooshCountDown = 100;
	}

	public void DoFusionEvent(int theGridX, int theGridY, SeedType theSeedType, SeedType theImitaterType)
	{
		SeedType seedType = theSeedType;
		if (theSeedType == SeedType.Imitater)
		{
			seedType = theImitaterType;
		}
		switch (seedType)
		{
		case SeedType.ExplodeONut:
		{
			Plant plant2 = AddPlant(theGridX, theGridY, SeedType.Cherrybomb, SeedType.None);
			plant2.DoSpecial();
			break;
		}
		case SeedType.Snowpea:
		case SeedType.IceWallnut:
		{
			Plant plant = AddPlant(theGridX, theGridY, SeedType.Iceshroom, SeedType.None);
			plant.DoSpecial();
			break;
		}
		}
	}

	public void DoFwoosh(int theRow)
	{
		for (int i = 0; i < 12; i++)
		{
			mApp.ReanimationTryToGet(mFwooshID[theRow, i])?.ReanimationDie();
			float num = 750f * (float)i / 11f + 10f + (float)Constants.BOARD_EXTRA_ROOM;
			float theY = GetPosYBasedOnRow(num + 10f, theRow) - 10f;
			int aRenderOrder = MakeRenderOrder(RenderLayer.Particle, theRow, 1);
			Reanimation reanimation = mApp.AddReanimation(num, theY, aRenderOrder, ReanimationType.JalapenoFire);
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_flame);
			reanimation.mLoopType = ReanimLoopType.LoopFullLastFrame;
			reanimation.mAnimRate *= TodCommon.RandRangeFloat(0.7f, 1.3f);
			float num2 = TodCommon.RandRangeFloat(0.9f, 1.1f);
			float num3 = 1f;
			if (RandomNumbers.NextNumber(2) == 0)
			{
				num3 = -1f;
			}
			reanimation.OverrideScale(num2 * num3, 1f);
			mFwooshID[theRow, i] = mApp.ReanimationGetID(reanimation);
		}
		mFwooshCountDown = 100;
	}

	public void UpdateFwoosh()
	{
		if (mFwooshCountDown == 0)
		{
			return;
		}
		mFwooshCountDown--;
		int num = TodCommon.TodAnimateCurve(50, 0, mFwooshCountDown, 12, 0, TodCurves.Linear);
		for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
		{
			for (int j = 0; j < 12 - num; j++)
			{
				Reanimation reanimation = mApp.ReanimationTryToGet(mFwooshID[i, j]);
				if (reanimation != null)
				{
					reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_done);
					reanimation.mLoopType = ReanimLoopType.PlayOnceFullLastFrame;
					reanimation.mAnimRate = 15f;
				}
				mFwooshID[i, j] = null;
			}
		}
	}

	public Plant SpecialPlantHitTest(int x, int y)
	{
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (plant.mDead)
			{
				continue;
			}
			if (plant.mSeedType == SeedType.Pumpkinshell)
			{
				float num = 25f;
				if (GetTopPlantAt(plant.mPlantCol, plant.mRow, TopPlant.OnlyNormalPosition) == null)
				{
					num = 0f;
				}
				float num2 = TodCommon.Distance2D(x, y, (float)plant.mX + 40f, (float)plant.mY + 40f);
				if (num2 >= num && num2 <= 50f && (float)y > (float)plant.mY + 25f)
				{
					return plant;
				}
			}
			if (Plant.IsFlying(plant.mSeedType))
			{
				float num3 = TodCommon.Distance2D(x, y, (float)plant.mX + 40f, plant.mY);
				if (num3 < 15f)
				{
					return plant;
				}
			}
		}
		return null;
	}

	public HitResult ToolHitTest(int x, int y, bool posScaled, bool isTouch)
	{
		MouseHitTest(x, y, out var theHitResult, posScaled, isTouch);
		if (theHitResult.mObjectType == GameObjectType.Plant)
		{
			Plant plant = (Plant)theHitResult.mObject;
			if (plant.mSeedType == SeedType.Gravebuster && mApp.mGameMode != GameMode.ChallengeZenGarden)
			{
				theHitResult.mObjectType = GameObjectType.None;
				theHitResult.mObject = null;
			}
		}
		return theHitResult;
	}

	public bool CanAddGraveStoneAt(int theGridX, int theGridY)
	{
		if (mGridSquareType[theGridX, theGridY] != GridSquareType.Grass && mGridSquareType[theGridX, theGridY] != GridSquareType.HighGround)
		{
			return false;
		}
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridX == theGridX && theGridItem.mGridY == theGridY && (theGridItem.mGridItemType == GridItemType.Gravestone || theGridItem.mGridItemType == GridItemType.Crater || theGridItem.mGridItemType == GridItemType.Ladder))
			{
				return false;
			}
		}
		return true;
	}

	public void UpdateGridItems()
	{
		if (ZBCheatDialog.AdvancedPause)
		{
			return;
		}
		UpdateGridItems_Orig();
	}

	private void UpdateGridItems_Orig()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (mEnableGraveStones && theGridItem.mGridItemType == GridItemType.Gravestone && theGridItem.mGridItemCounter < 100)
			{
				theGridItem.mGridItemCounter++;
			}
			if (theGridItem.mGridItemType == GridItemType.Crater && mApp.mGameScene == GameScenes.Playing)
			{
				if (theGridItem.mGridItemCounter > 0)
				{
					theGridItem.mGridItemCounter--;
				}
				if (theGridItem.mGridItemCounter <= 0)
				{
					theGridItem.GridItemDie();
				}
			}
			theGridItem.Update();
		}
	}

	public GridItem AddAGraveStone(int theGridX, int theGridY)
	{
		if (!doAddGraveStones)
		{
			return null;
		}
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.Gravestone;
		newGridItem.mGridItemCounter = -RandomNumbers.NextNumber(50);
		newGridItem.mRenderOrder = MakeRenderOrder(RenderLayer.GraveStone, theGridY, 3);
		newGridItem.mGridX = theGridX;
		newGridItem.mGridY = theGridY;
		mGridItems.Add(newGridItem);
		return newGridItem;
	}

	public int GetSurvivalFlagsCompleted()
	{
		int numWavesPerFlag = GetNumWavesPerFlag();
		int num = mChallenge.mSurvivalStage * GetNumWavesPerSurvivalStage() / numWavesPerFlag;
		if (mApp.IsSurvivalHell(mApp.mGameMode) && mChallenge.mSurvivalStage >= 3)
		{
			num -= 3;
		}
		if (IsFlagWave(mCurrentWave - 1) && mBoardFadeOutCounter < 0 && mNextSurvivalStageCounter == 0)
		{
			return (mCurrentWave - 1) / numWavesPerFlag + num;
		}
		return mCurrentWave / numWavesPerFlag + num;
	}

	public bool HasProgressMeter()
	{
		if (mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist && !mApp.IsFinalBossLevel() && !mApp.IsSlotMachineLevel() && !mApp.IsSquirrelLevel() && !mApp.IsIZombieLevel())
		{
			if (mProgressMeterWidth != 0 && !mApp.IsContinuousChallenge() && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom)
			{
				return !mApp.IsScaryPotterLevel();
			}
			return false;
		}
		return true;
	}

	public void UpdateCursor()
	{
		bool flag = false;
		bool flag2 = false;
		int screenX = mApp.mWidgetManager.mLastMouseX - mX;
		int screenY = mApp.mWidgetManager.mLastMouseY - mY;
		if (mCameraEnabled)
		{
			mCamera.ScreenToBoardReplace(ref screenX, ref screenY);
		}
		if ((mApp.mSeedChooserScreen != null && mApp.mSeedChooserScreen.Contains(screenX + mX, screenY + mY)) || mApp.GetDialogCount() > 0)
		{
			return;
		}
		if (mPaused || mBoardFadeOutCounter >= 0 || mTimeStopCounter > 0 || mApp.mGameScene == GameScenes.ZombiesWon)
		{
			flag = false;
		}
		else
		{
			MouseHitTest(screenX, screenY, out var theHitResult, posScaled: false);
			if (theHitResult.mObjectType == GameObjectType.MenuButton || theHitResult.mObjectType == GameObjectType.StoreButton || theHitResult.mObjectType == GameObjectType.CornRouletteButton || theHitResult.mObjectType == GameObjectType.Shovel || theHitResult.mObjectType == GameObjectType.WateringCan || theHitResult.mObjectType == GameObjectType.Fertilizer || theHitResult.mObjectType == GameObjectType.BugSpray || theHitResult.mObjectType == GameObjectType.Phonograph || theHitResult.mObjectType == GameObjectType.Chocolate || theHitResult.mObjectType == GameObjectType.Glove || theHitResult.mObjectType == GameObjectType.MoneySign || theHitResult.mObjectType == GameObjectType.NextGarden || theHitResult.mObjectType == GameObjectType.Wheelbarrow || theHitResult.mObjectType == GameObjectType.SlotMachineHandle || theHitResult.mObjectType == GameObjectType.TreeFood || theHitResult.mObjectType == GameObjectType.PrevGarden)
			{
				flag = true;
			}
			else if (theHitResult.mObjectType == GameObjectType.Seedpacket)
			{
				SeedPacket seedPacket = (SeedPacket)theHitResult.mObject;
				if (seedPacket.CanPickUp())
				{
					flag = true;
				}
			}
			else if (theHitResult.mObjectType == GameObjectType.ScaryPot && mCursorObject.mCursorType == CursorType.Normal)
			{
				flag = true;
			}
			else if (theHitResult.mObjectType == GameObjectType.Stinky)
			{
				flag = true;
			}
			else if (theHitResult.mObjectType == GameObjectType.TreeOfWisdom)
			{
				flag = true;
			}
			else if (theHitResult.mObjectType == GameObjectType.Coin || theHitResult.mObjectType == GameObjectType.Projectile)
			{
				flag = true;
			}
			else if (theHitResult.mObjectType == GameObjectType.Plant)
			{
				if (mApp.mGameMode == GameMode.ChallengeBeghouled && !HasLevelAwardDropped())
				{
					flag2 = true;
				}
				if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist && !HasLevelAwardDropped())
				{
					flag2 = true;
				}
				Plant plant = (Plant)theHitResult.mObject;
				if (plant.mState == PlantState.CobcannonReady && mCursorObject.mCursorType == CursorType.Normal)
				{
					flag = true;
				}
			}
			else
			{
				_ = mCursorObject.mCursorType;
				_ = 7;
			}
			if (mChallenge.mBeghouledMouseCapture)
			{
				flag2 = true;
			}
		}
		if (!flag2)
		{
		}
	}

	public void UpdateTutorial()
	{
		if (mTutorialTimer > 0)
		{
			mTutorialTimer--;
		}
		if (mTutorialState == TutorialState.Level1PickUpPeashooter && mTutorialTimer == 0)
		{
			DisplayAdvice("[ADVICE_CLICK_PEASHOOTER]", MessageStyle.TutorialLevel1Stay, AdviceType.None);
			float screenX = mSeedBank.mX + mSeedBank.mSeedPackets[0].mX;
			float screenY = 0f;
			if (mCameraEnabled && mCamera != null)
			{
				mCamera?.ScreenToBoardReplace(ref screenX, ref screenY);
			}
			TutorialArrowShow((int)(screenX + (float)(Constants.SMALL_SEEDPACKET_WIDTH / 2) - Constants.InvertAndScale(13f)), (int)screenY);
			mTutorialTimer = -1;
		}
		if (mTutorialState == TutorialState.Level2PickUpSunflower || mTutorialState == TutorialState.Level2PlantSunflower || mTutorialState == TutorialState.Level2RefreshSunflower)
		{
			if (mTutorialTimer == 0)
			{
				DisplayAdvice("[ADVICE_PLANT_SUNFLOWER2]", MessageStyle.TutorialLevel2, AdviceType.None);
				mTutorialTimer = -1;
			}
			else if (mZombieCountDown == 750 && mCurrentWave == 0)
			{
				DisplayAdvice("[ADVICE_PLANT_SUNFLOWER3]", MessageStyle.TutorialLevel2, AdviceType.None);
			}
		}
		if ((mTutorialState == TutorialState.MoresunPickUpSunflower || mTutorialState == TutorialState.MoresunPlantSunflower || mTutorialState == TutorialState.MoresunRefreshSunflower) && mTutorialTimer == 0)
		{
			DisplayAdvice("[ADVICE_PLANT_SUNFLOWER5]", MessageStyle.TutorialLater, AdviceType.PlantSunflower5);
			mTutorialTimer = -1;
		}
		if (mApp.IsFirstTimeAdventureMode() && mLevel >= 3 && mLevel != 5 && mLevel <= 7 && mTutorialState == TutorialState.Off && mCurrentWave >= 5 && !gShownMoreSunTutorial && mSeedBank.mSeedPackets[1].CanPickUp() && CountPlantByType(SeedType.Sunflower) < 3)
		{
			Debug.ASSERT(!ChooseSeedsOnCurrentLevel());
			DisplayAdvice("[ADVICE_PLANT_SUNFLOWER4]", MessageStyle.TutorialLaterStay, AdviceType.None);
			gShownMoreSunTutorial = true;
			SetTutorialState(TutorialState.MoresunPickUpSunflower);
			mTutorialTimer = 500;
		}
	}

	public SeedType GetSeedTypeInCursor()
	{
		if (mCursorObject.mCursorType == CursorType.Wheeelbarrow && mApp.mZenGarden.GetPottedPlantInWheelbarrow() != null)
		{
			return mApp.mZenGarden.GetPottedPlantInWheelbarrow().mSeedType;
		}
		if (!IsPlantInCursor())
		{
			return SeedType.None;
		}
		if (mCursorObject.mType == SeedType.Imitater)
		{
			return mCursorObject.mImitaterType;
		}
		return mCursorObject.mType;
	}

	public int CountPlantByType(SeedType theSeedType)
	{
		int num = 0;
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.mSeedType == theSeedType)
			{
				num++;
			}
		}
		return num;
	}

	public bool PlantingRequirementsMet(SeedType theSeedType)
	{
		if (mApp.IsLevelUseJson() && mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mEasyUpgrade)
		{
			return true;
		}
		if ((theSeedType != SeedType.Gatlingpea || CountPlantByType(SeedType.Repeater) != 0) && (theSeedType != SeedType.Wintermelon || CountPlantByType(SeedType.Melonpult) != 0) && (theSeedType != SeedType.Twinsunflower || CountPlantByType(SeedType.Sunflower) != 0) && (theSeedType != SeedType.Spikerock || CountPlantByType(SeedType.Spikeweed) != 0) && (theSeedType != SeedType.Cobcannon || HasValidCobCannonSpot()) && (theSeedType != SeedType.GoldMagnet || CountPlantByType(SeedType.Magnetshroom) != 0) && (theSeedType != SeedType.Gloomshroom || CountPlantByType(SeedType.Fumeshroom) != 0) && (theSeedType != SeedType.Cattail || CountEmptyPotsOrLilies(SeedType.Lilypad) != 0))
		{
			if (theSeedType == SeedType.SuperChomper)
			{
				return CountPlantByType(SeedType.Chomper) != 0;
			}
			return true;
		}
		return false;
	}

	public bool HasValidCobCannonSpot()
	{
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.mSeedType == SeedType.Kernelpult && IsValidCobCannonSpot(plant.mPlantCol, plant.mRow))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsValidCobCannonSpot(int theGridX, int theGridY)
	{
		if (!IsValidCobCannonSpotHelper(theGridX, theGridY) || !IsValidCobCannonSpotHelper(theGridX + 1, theGridY))
		{
			return false;
		}
		bool flag = GetFlowerPotAt(theGridX, theGridY) != null;
		bool flag2 = GetFlowerPotAt(theGridX + 1, theGridY) != null;
		return flag == flag2;
	}

	public bool IsValidCobCannonSpotMovePlant(int theGridX, int theGridY)
	{
		if (CanPlantAt(theGridX, theGridY, SeedType.Kernelpult) != PlantingReason.Ok || CanPlantAt(theGridX + 1, theGridY, SeedType.Kernelpult) != PlantingReason.Ok)
		{
			return false;
		}
		bool flag = GetFlowerPotAt(theGridX, theGridY) != null;
		bool flag2 = GetFlowerPotAt(theGridX + 1, theGridY) != null;
		return flag == flag2;
	}

	public bool IsValidCobCannonSpotHelper(int theGridX, int theGridY)
	{
		PlantsOnLawn thePlantOnLawn = default;
		GetPlantsOnLawn(theGridX, theGridY, ref thePlantOnLawn);
		if (thePlantOnLawn.mPumpkinPlant != null)
		{
			return false;
		}
		if (!mApp.mEasyPlantingCheat && (!mApp.IsLevelUseJson() || !mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mEasyUpgrade))
		{
			if (thePlantOnLawn.mNormalPlant != null)
			{
				return thePlantOnLawn.mNormalPlant.mSeedType == SeedType.Kernelpult;
			}
			return false;
		}
		if (thePlantOnLawn.mNormalPlant == null || thePlantOnLawn.mNormalPlant.mSeedType != SeedType.Kernelpult)
		{
			return CanPlantAt(theGridX, theGridY, SeedType.Kernelpult) == PlantingReason.Ok;
		}
		return true;
	}

	public void MouseDownCobcannonFire(int x, int y, int theClickCount)
	{
		float num = TodCommon.Distance2D(x, y, mCobCannonMouseX, mCobCannonMouseY);
		x = (int)((float)x * Constants.IS);
		y = (int)((float)y * Constants.IS);
		if (theClickCount < 0)
		{
			ClearCursor();
		}
		else if (y < Constants.LAWN_YMIN)
		{
			ClearCursor();
		}
		else
		{
			if (mCobCannonCursorDelayCounter > 0 && num < 50f)
			{
				return;
			}
			if (mCursorObject.mCursorType == CursorType.PlantFromDuplicator)
			{
				ClearCursor();
				return;
			}
			Plant plant = null;
			if (mCursorObject.mCobCannonPlantID != null && mPlants.IndexOf(mCursorObject.mCobCannonPlantID) >= 0)
			{
				plant = mCursorObject.mCobCannonPlantID;
			}
			if (plant == null)
			{
				ClearCursor();
				return;
			}
			plant.CobCannonFire(x, y);
			ClearCursor();
		}
	}

	public int KillAllZombiesInRadius(int theRow, int theX, int theY, int theRadius, int theRowRange, bool theBurn, int theDamageRangeFlags)
	{
		int num = 0;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (zombie.mDead || !zombie.EffectedByDamage((uint)theDamageRangeFlags))
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			int num2 = zombie.mRow - theRow;
			if (zombie.mZombieType == ZombieType.Boss)
			{
				num2 = 0;
			}
			if (num2 <= theRowRange && num2 >= -theRowRange && GameConstants.GetCircleRectOverlap(theX, theY, theRadius, zombieRect))
			{
				bool flag = zombie.IsDeadOrDying();
				if (theBurn)
				{
					zombie.ApplyBurn();
				}
				else
				{
					zombie.TakeDamage(1800, 18u);
				}
				if (!flag && zombie.IsDeadOrDying())
				{
					num++;
				}
			}
		}
		int num3 = PixelToGridXKeepOnBoard(theX, theY);
		int num4 = PixelToGridYKeepOnBoard(theX, theY);
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Ladder)
			{
				int num5 = theGridItem.mGridX - num3;
				int num6 = theGridItem.mGridY - num4;
				if (num5 <= theRowRange && num5 >= -theRowRange && num6 <= theRowRange && num6 >= -theRowRange)
				{
					theGridItem.GridItemDie();
				}
			}
		}
		return num;
	}

	public bool IsFlagWave(int theWaveNumber)
	{
		if (mApp.IsFirstTimeAdventureMode() && mLevel == 1)
		{
			return false;
		}
		int numWavesPerFlag = GetNumWavesPerFlag();
		return theWaveNumber % numWavesPerFlag == numWavesPerFlag - 1;
	}

	public void DrawHouseDoorTop(Graphics g)
	{
		if (mBackground == BackgroundType.Num1Day)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND1_GAMEOVER_MASK, Constants.Board_GameOver_Exterior_Overlay_1.X, Constants.Board_GameOver_Exterior_Overlay_1.Y);
		}
		else if (mBackground == BackgroundType.Num2Night)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND2_GAMEOVER_MASK, Constants.Board_GameOver_Exterior_Overlay_2.X, Constants.Board_GameOver_Exterior_Overlay_2.Y);
		}
		else if (mBackground == BackgroundType.Num3Pool)
		{
			if (Zombie.WinningZombieReachedDesiredY)
			{
				g.DrawImage(Resources.IMAGE_BACKGROUND3_GAMEOVER_MASK, Constants.Board_GameOver_Exterior_Overlay_3.X, Constants.Board_GameOver_Exterior_Overlay_3.Y);
			}
		}
		else if (mBackground == BackgroundType.Num4Fog)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND4_GAMEOVER_MASK, Constants.Board_GameOver_Exterior_Overlay_4.X, Constants.Board_GameOver_Exterior_Overlay_4.Y);
		}
		else if (mBackground == BackgroundType.Num5Roof)
		{
			if (Zombie.WinningZombieReachedDesiredY)
			{
				g.DrawImage(Resources.IMAGE_BACKGROUND5_GAMEOVER_MASK, Constants.Board_GameOver_Exterior_Overlay_5.X, Constants.Board_GameOver_Exterior_Overlay_5.Y);
			}
		}
		else if (mBackground == BackgroundType.Num6Boss && Zombie.WinningZombieReachedDesiredY)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND6_GAMEOVER_MASK, Constants.Board_GameOver_Exterior_Overlay_6.X, Constants.Board_GameOver_Exterior_Overlay_6.Y);
		}
	}

	public void DrawHouseDoorBottom(Graphics g)
	{
		if (mBackground == BackgroundType.Num1Day)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND1_GAMEOVER_INTERIOR_OVERLAY, Constants.Board_GameOver_Interior_Overlay_1.X, Constants.Board_GameOver_Interior_Overlay_1.Y);
		}
		else if (mBackground == BackgroundType.Num2Night)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND2_GAMEOVER_INTERIOR_OVERLAY, Constants.Board_GameOver_Interior_Overlay_2.X, Constants.Board_GameOver_Interior_Overlay_2.Y);
		}
		else if (mBackground == BackgroundType.Num3Pool)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND3_GAMEOVER_INTERIOR_OVERLAY, Constants.Board_GameOver_Interior_Overlay_3.X, Constants.Board_GameOver_Interior_Overlay_3.Y);
		}
		else if (mBackground == BackgroundType.Num4Fog)
		{
			g.DrawImage(Resources.IMAGE_BACKGROUND4_GAMEOVER_INTERIOR_OVERLAY, Constants.Board_GameOver_Interior_Overlay_4.X, Constants.Board_GameOver_Interior_Overlay_4.Y);
		}
	}

	public Zombie GetBossZombie()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie.mZombieType == ZombieType.Boss)
			{
				return zombie;
			}
		}
		return null;
	}

	public bool HasConveyorBeltSeedBank()
	{
		if ((!mApp.IsFinalBossLevel() || mApp.mGameMode == GameMode.ChallengeFinalBoss2) && !mApp.IsMiniBossLevel() && !mApp.IsShovelLevel() && !mApp.IsWallnutBowlingLevel() && !mApp.IsLittleTroubleLevel() && !mApp.IsStormyNightLevel() && !mApp.IsBungeeBlitzLevel() && mApp.mGameMode != GameMode.ChallengePortalCombat && mApp.mGameMode != GameMode.ChallengeColumn && mApp.mGameMode != GameMode.ChallengeInvisighoul && mApp.mGameMode != GameMode.ChallengeAttackOnTitans && !mApp.IsRogueConveyorbeltLevel() && mApp.mGameMode != GameMode.ChallengeColumn2 && mApp.mGameMode != GameMode.ChallengeColumn3 && mApp.mGameMode != GameMode.ChallengeColumn4 && mApp.mGameMode != GameMode.ChallengeMoreAirRaid && mApp.mGameMode != GameMode.OccupyHighGround)
		{
			return mApp.mCreativeLevel?.GetComponent<CSConveyorBelt>() != null;
		}
		return true;
	}

	public bool StageHasRoof()
	{
		if (mBackground != BackgroundType.Num5Roof)
		{
			return mBackground == BackgroundType.Num6Boss;
		}
		return true;
	}

	public void SpawnZombiesFromPool()
	{
		if (mIceTrapCounter > 0)
		{
			return;
		}
		int num;
		int num2;
		if (mLevel == 21 || mLevel == 22 || mLevel == 31 || mLevel == 32)
		{
			num = 2;
			num2 = 3;
		}
		else if (mLevel == 23 || mLevel == 24 || mLevel == 25 || mLevel == 33 || mLevel == 34 || mLevel == 35)
		{
			num = 3;
			num2 = 5;
		}
		else
		{
			num = 3;
			num2 = 7;
		}
		int num3 = 0;
		for (int i = 0; i < aGridArray.Length; i++)
		{
			aGridArray[i].Reset();
		}
		for (int j = 5; j < Constants.GRIDSIZEX; j++)
		{
			if (StageHasBigPool())
			{
				for (int k = 0; k <= 5; k++)
				{
					aGridArray[num3].mX = j;
					aGridArray[num3].mY = k;
					aGridArray[num3].mWeight = 10000;
					num3++;
					Debug.ASSERT(num3 <= 30);
				}
			}
			else
			{
				for (int l = 2; l <= 3; l++)
				{
					aGridArray[num3].mX = j;
					aGridArray[num3].mY = l;
					aGridArray[num3].mWeight = 10000;
					num3++;
					Debug.ASSERT(num3 <= 10);
				}
			}
		}
		if (num > num3)
		{
			num = num3;
		}
		if (num3 == 0)
		{
			return;
		}
		for (int m = 0; m < num; m++)
		{
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(aGridArray, num3);
			todWeightedGridArray.mWeight = 0;
			ZombieType zombieType = PickGraveRisingZombieType(num2);
			ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
			Zombie zombie = AddZombieInRow(zombieType, todWeightedGridArray.mY, mCurrentWave);
			if (zombie == null)
			{
				break;
			}
			zombie.RiseFromGrave(todWeightedGridArray.mX, todWeightedGridArray.mY);
			zombie.mUsesClipping = true;
			num2 -= GetZombieValueForCurrentLevel(zombieType);
			num2 = Math.Max(1, num2);
		}
	}

	public void SpawnZombiesFromSky()
	{
		if (mIceTrapCounter > 0)
		{
			return;
		}
		int num;
		int num2;
		if (mLevel == 41 || mLevel == 42)
		{
			num = 2;
			num2 = 3;
		}
		else if (mLevel == 43 || mLevel == 44 || mLevel == 45)
		{
			num = 3;
			num2 = 5;
		}
		else
		{
			num = 3;
			num2 = 7;
		}
		BungeeDropGrid bungeeDropGrid = new BungeeDropGrid();
		SetupBungeeDrop(bungeeDropGrid);
		if (num > bungeeDropGrid.mGridArrayCount)
		{
			num = bungeeDropGrid.mGridArrayCount;
		}
		if (bungeeDropGrid.mGridArrayCount != 0)
		{
			for (int i = 0; i < num; i++)
			{
				ZombieType zombieType = PickGraveRisingZombieType(num2);
				BungeeDropZombie(bungeeDropGrid, zombieType);
				ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
				num2 -= GetZombieValueForCurrentLevel(zombieType);
				num2 = Math.Max(1, num2);
			}
		}
	}

	public void PickUpTool(GameObjectType theObjectType)
	{
		GameMode mGameMode = mApp.mGameMode;
		bool flag = ((mGameMode == GameMode.ChallengeZenGarden || mGameMode == GameMode.TreeOfWisdom) ? true : false);
		if ((!flag && !CanUseCombatTool(theObjectType)) || mPaused || (mApp.mGameScene != GameScenes.Playing && !mCutScene.IsInShovelTutorial()))
		{
			return;
		}
		switch (theObjectType)
		{
		case GameObjectType.Shovel:
			if (mCursorObject.mCursorType == CursorType.Shovel)
			{
				ClearCursor();
				return;
			}
			if (mTutorialState == TutorialState.ShovelPickup)
			{
				SetTutorialState(TutorialState.ShovelDig);
			}
			mCursorObject.mCursorType = CursorType.Shovel;
			mApp.PlayFoley(FoleyType.Shovel);
			break;
		case GameObjectType.WateringCan:
			if (mTutorialState == TutorialState.ZenGardenPickupWater)
			{
				mTutorialState = TutorialState.ZenGardenWaterPlant;
				mApp.mPlayerInfo.mZenTutorialMessage = 23;
				DisplayAdvice("[ADVICE_ZEN_GARDEN_WATER_PLANT]", MessageStyle.ZenGardenLong, AdviceType.None);
				TutorialArrowRemove();
			}
			mCursorObject.mCursorType = CursorType.WateringCan;
			if (mApp.mPlayerInfo.mPurchases[13] > 0)
			{
				mIgnoreNextMouseUp = true;
			}
			mApp.PlayFoley(FoleyType.Drop);
			break;
		case GameObjectType.Fertilizer:
			if (mApp.mPlayerInfo.mPurchases[14] > 1000)
			{
				mCursorObject.mCursorType = CursorType.Fertilizer;
				mApp.PlayFoley(FoleyType.Drop);
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
			}
			break;
		case GameObjectType.BugSpray:
			if (mApp.mPlayerInfo.mPurchases[15] > 1000)
			{
				mCursorObject.mCursorType = CursorType.BugSpray;
				mApp.PlayFoley(FoleyType.Drop);
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
			}
			break;
		case GameObjectType.Phonograph:
			mCursorObject.mCursorType = CursorType.Phonograph;
			mApp.PlayFoley(FoleyType.Drop);
			break;
		case GameObjectType.Chocolate:
			if (mApp.mPlayerInfo.mPurchases[26] > 1000)
			{
				mCursorObject.mCursorType = CursorType.Chocolate;
				mApp.PlayFoley(FoleyType.Drop);
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
			}
			break;
		case GameObjectType.Glove:
			mCursorObject.mCursorType = CursorType.Glove;
			mApp.PlayFoley(FoleyType.Drop);
			break;
		case GameObjectType.MoneySign:
			mCursorObject.mCursorType = CursorType.MoneySign;
			mApp.PlayFoley(FoleyType.Drop);
			break;
		case GameObjectType.Wheelbarrow:
			mCursorObject.mCursorType = CursorType.Wheeelbarrow;
			mApp.PlayFoley(FoleyType.Drop);
			break;
		case GameObjectType.TreeFood:
			if (mChallenge.TreeOfWisdomCanFeed())
			{
				if (mApp.mPlayerInfo.mPurchases[28] > 1000)
				{
					mCursorObject.mCursorType = CursorType.TreeFood;
					mApp.PlayFoley(FoleyType.Drop);
				}
				else
				{
					mApp.PlaySample(Resources.SOUND_BUZZER);
				}
			}
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		mCursorObject.mType = SeedType.None;
	}

	public void TutorialArrowShow(int x, int y)
	{
		TutorialArrowRemove();
		TodParticleSystem theParticle = mApp.AddTodParticle((float)(x - Constants.Board_Offset_AspectRatio_Correction) * Constants.IS, (float)y * Constants.IS, 800000, ParticleEffect.SeedPacketPick);
		mTutorialParticleID = mApp.ParticleGetID(theParticle);
	}

	public void TutorialArrowRemove()
	{
		mApp.ParticleTryToGet(mTutorialParticleID)?.ParticleSystemDie();
		mTutorialParticleID = null;
	}

	public int CountCoinsBeingCollected()
	{
		int num = 0;
		Coin theCoin = null;
		while (IterateCoins(ref theCoin))
		{
			if (theCoin.mIsBeingCollected && theCoin.IsMoney())
			{
				num += Coin.GetCoinValue(theCoin.mType);
			}
		}
		return num;
	}

	public void BungeeDropZombie(BungeeDropGrid theBungeeDropGrid, ZombieType theZombieType)
	{
		TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(theBungeeDropGrid.mGridArray, theBungeeDropGrid.mGridArrayCount);
		todWeightedGridArray.mWeight = 1;
		Zombie zombie = AddZombie(ZombieType.Bungee, mCurrentWave);
		Zombie zombie2 = AddZombie(theZombieType, mCurrentWave);
		Debug.ASSERT(zombie != null && zombie2 != null);
		zombie.BungeeDropZombie(zombie2, todWeightedGridArray.mX, todWeightedGridArray.mY);
	}

	public void SetupBungeeDrop(BungeeDropGrid theBungeeDropGrid)
	{
		theBungeeDropGrid.mGridArrayCount = 0;
		for (int i = 4; i < Constants.GRIDSIZEX; i++)
		{
			for (int j = 0; j <= 4; j++)
			{
				int mGridArrayCount = theBungeeDropGrid.mGridArrayCount;
				theBungeeDropGrid.mGridArray[mGridArrayCount].mX = i;
				theBungeeDropGrid.mGridArray[mGridArrayCount].mY = j;
				theBungeeDropGrid.mGridArray[mGridArrayCount].mWeight = 10000;
				theBungeeDropGrid.mGridArrayCount++;
				Debug.ASSERT(theBungeeDropGrid.mGridArrayCount <= theBungeeDropGrid.mGridArray.Length);
			}
		}
	}

	public void PutZombieInWave(ZombieType theZombieType, int theWaveNumber, ZombiePicker theZombiePicker, int theZombieValue)
	{
		Debug.ASSERT(theWaveNumber < 100 && theZombiePicker.mZombieCount < 50);
		mZombiesInWave[theWaveNumber, theZombiePicker.mZombieCount] = theZombieType;
		theZombiePicker.mZombieCount++;
		if (theZombiePicker.mZombieCount < 50)
		{
			mZombiesInWave[theWaveNumber, theZombiePicker.mZombieCount] = ZombieType.Invalid;
		}
		theZombiePicker.mZombiePoints -= theZombieValue;
		theZombiePicker.mZombieTypeCount[(int)theZombieType]++;
		theZombiePicker.mAllWavesZombieTypeCount[(int)theZombieType]++;
	}

	public void PutZombieInWave(ZombieType theZombieType, int theWaveNumber, ZombiePicker theZombiePicker)
	{
		Debug.ASSERT(theWaveNumber < 100 && theZombiePicker.mZombieCount < 50);
		mZombiesInWave[theWaveNumber, theZombiePicker.mZombieCount] = theZombieType;
		theZombiePicker.mZombieCount++;
		if (theZombiePicker.mZombieCount < 50)
		{
			mZombiesInWave[theWaveNumber, theZombiePicker.mZombieCount] = ZombieType.Invalid;
		}
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(theZombieType);
		theZombiePicker.mZombiePoints -= GetZombieValueForCurrentLevel(theZombieType);
		theZombiePicker.mZombieTypeCount[(int)theZombieType]++;
		theZombiePicker.mAllWavesZombieTypeCount[(int)theZombieType]++;
	}

	public void PutInMissingZombies(int theWaveNumber, ZombiePicker theZombiePicker)
	{
		for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
		{
			if (theZombiePicker.mZombieTypeCount[(int)zombieType] <= 0 && zombieType != ZombieType.Yeti && CanZombieSpawnOnLevel(zombieType, mLevel))
			{
				PutZombieInWave(zombieType, theWaveNumber, theZombiePicker);
			}
		}
	}

	private CSOverrideZombieProperty.Info GetZombieOverrideInfoForCurrentLevel(ZombieType zombieType)
	{
		if (!mApp.IsLevelUseJson())
		{
			return null;
		}
		CSOverrideZombieProperty component = mApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
		if (component != null && component.mProperties.TryGetValue(zombieType, out var value))
		{
			return value;
		}
		return null;
	}

	private int GetZombieValueForCurrentLevel(ZombieType zombieType)
	{
		return GetZombieOverrideInfoForCurrentLevel(zombieType)?.mZombieValue ?? Zombie.GetZombieDefinition(zombieType).mZombieValue;
	}

	private int GetZombiePickWeightForCurrentLevel(ZombieType zombieType)
	{
		return GetZombieOverrideInfoForCurrentLevel(zombieType)?.mPickWeight ?? Zombie.GetZombieDefinition(zombieType).mPickWeight;
	}

	private int GetZombieFirstAllowedWaveForCurrentLevel(ZombieType zombieType)
	{
		return GetZombieOverrideInfoForCurrentLevel(zombieType)?.mFirstAllowedWave ?? Zombie.GetZombieDefinition(zombieType).mFirstAllowedWave;
	}

	public TRect GetAgaveSkillRect()
	{
		int theX = 20;
		int theY = 45;
		int theWidth = 50;
		int theHeight = 50;
		return new TRect(theX, theY, theWidth, theHeight);
	}

	public TRect GetEndoflameSkillRect()
	{
		int num = 20;
		int theY = 45;
		int theWidth = 50;
		int theHeight = 50;
		if (mAgaveSkillDrawType != 0)
		{
			num += 50;
		}
		return new TRect(num, theY, theWidth, theHeight);
	}

	public TRect GetShovelButtonRect()
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom)
		{
			int theX = AtlasResources.IMAGE_CLASSIC_SEEDBANK.mWidth + GetSeedBankExtraWidth() + GameConstants.SEED_BANK_OFFSET_X_END;
			if (mApp.IsSlotMachineLevel() || mApp.IsSquirrelLevel())
			{
				theX = 600;
			}
			return new TRect(theX, 0, AtlasResources.IMAGE_SHOVELBANK_ZEN.mWidth, AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight);
		}
		TRect result = new TRect(Constants.UIShovelButtonPosition.X - Constants.Board_Offset_AspectRatio_Correction, Constants.UIShovelButtonPosition.Y, AtlasResources.IMAGE_SHOVELBANK_ZEN.mWidth, AtlasResources.IMAGE_SHOVELBANK_ZEN.mHeight);
		if (mApp.IsSquirrelLevel())
		{
			result.mX = 600;
		}
		return result;
	}

	public TRect GetZenShovelButtonRect()
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser && HasGlove() && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom)
		{
			TRect shovelButtonRect = GetShovelButtonRect();
			return new TRect(shovelButtonRect.mX + shovelButtonRect.mWidth, shovelButtonRect.mY, AtlasResources.IMAGE_SHOVELBANK_ZEN.mWidth, shovelButtonRect.mHeight);
		}
		TRect result = new TRect(Constants.ZenGardenTopButtonStart, Constants.UIShovelButtonPosition.Y, AtlasResources.IMAGE_SHOVELBANK_ZEN.mWidth, AtlasResources.IMAGE_SHOVELBANK_ZEN.mHeight);
		if (mApp.IsSlotMachineLevel() || mApp.IsSquirrelLevel())
		{
			result.mX = 600;
		}
		if (HasGlove())
		{
			result.mX += Constants.UIShovelButtonPosition.X - Constants.Board_Offset_AspectRatio_Correction + 90;
		}
		return result;
	}

	public TRect GetZenButtonRect(GameObjectType theObjectType)
	{
		TRect zenShovelButtonRect = GetZenShovelButtonRect();
		switch (theObjectType)
		{
		case GameObjectType.NextGarden:
			zenShovelButtonRect.mX = Constants.ZenGarden_NextGarden_Pos.X + 75;
			zenShovelButtonRect.mWidth = 75;
			return zenShovelButtonRect;
		case GameObjectType.PrevGarden:
			zenShovelButtonRect.mX = Constants.ZenGarden_NextGarden_Pos.X;
			zenShovelButtonRect.mWidth = 75;
			return zenShovelButtonRect;
		default:
		{
			bool flag = true;
			for (int i = 6; i <= 15; i++)
			{
				GameObjectType gameObjectType = (GameObjectType)i;
				if (gameObjectType != GameObjectType.TreeFood && !CanUseGameObject(gameObjectType))
				{
					flag = false;
				}
			}
			if (flag)
			{
				zenShovelButtonRect.mX = Constants.ZenGardenTopButtonStart;
			}
			for (int j = 6; j < (int)theObjectType; j++)
			{
				GameObjectType theGameObject = (GameObjectType)j;
				if (CanUseGameObject(theGameObject))
				{
					zenShovelButtonRect.mX += AtlasResources.IMAGE_SHOVELBANK_ZEN.GetWidth();
				}
			}
			return zenShovelButtonRect;
		}
		}
	}

	public Plant NewPlant(int theGridX, int theGridY, SeedType theSeedType, SeedType theImitaterType)
	{
		Plant newPlant = Plant.GetNewPlant();
		newPlant.mIsOnBoard = true;
		newPlant.PlantInitialize(theGridX, theGridY, theSeedType, theImitaterType);
		mPlants.Add(newPlant);
		return newPlant;
	}

	public void DoPlantingEffects(int theGridX, int theGridY, Plant thePlant, bool forAquarium)
	{
		int num = GridToPixelX(theGridX, theGridY) + 41;
		int num2 = GridToPixelY(theGridX, theGridY) + 74;
		if (thePlant != null)
		{
			if (thePlant.mSeedType == SeedType.Lilypad)
			{
				num2 += 15;
			}
			else if (thePlant.mSeedType == SeedType.Flowerpot)
			{
				num2 += 30;
			}
		}
		if (Plant.IsFlying(thePlant.mSeedType))
		{
			mApp.PlayFoley(FoleyType.Plant);
		}
		else if (forAquarium || IsPoolSquare(theGridX, theGridY))
		{
			mApp.PlayFoley(FoleyType.PlantWater);
			if (forAquarium)
			{
				num2 -= 30;
			}
			mApp.AddTodParticle(num, num2, 400000, ParticleEffect.PlantingPool);
		}
		else
		{
			mApp.PlayFoley(FoleyType.Plant);
			mApp.AddTodParticle(num, num2, 400000, ParticleEffect.Planting);
		}
	}

	public bool IsFinalSurvivalStage()
	{
		if (!mApp.IsSurvivalMode())
		{
			return false;
		}
		CSSurvivalLevel cSSurvivalLevel = mApp.mCreativeLevel?.GetComponent<CSSurvivalLevel>();
		if (cSSurvivalLevel != null)
		{
			if (cSSurvivalLevel.mEndless)
			{
				return false;
			}
			return mChallenge.mSurvivalStage == cSSurvivalLevel.mStages - 1;
		}
		int num = GetNumWavesPerSurvivalStage() * (mChallenge.mSurvivalStage + 1) / GetNumWavesPerFlag();
		if (mApp.IsSurvivalNormal(mApp.mGameMode))
		{
			return num >= 5;
		}
		if (mApp.IsSurvivalHard(mApp.mGameMode))
		{
			return num >= 10;
		}
		if (mApp.IsSurvivalHell(mApp.mGameMode))
		{
			return mChallenge.mSurvivalStage >= 4;
		}
		return false;
	}

	public void SurvivalSaveScore()
	{
		if (mApp.IsSurvivalMode() && !mApp.IsLevelUseJson())
		{
			int survivalFlagsCompleted = GetSurvivalFlagsCompleted();
			int currentChallengeIndex = mApp.GetCurrentChallengeIndex();
			if (survivalFlagsCompleted > mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex])
			{
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex] = survivalFlagsCompleted;
				mApp.WriteCurrentUserConfig();
			}
		}
	}

	public int CountZombiesOnScreen()
	{
		int num = 0;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && (zombie.mHasHead || zombie.IsAliveWithoutHead()) && !zombie.IsDeadOrDying() && !zombie.mMindControlled && zombie.IsOnBoard())
			{
				num++;
			}
		}
		return num;
	}

	public int GetNumWavesPerSurvivalStage()
	{
		if (mApp.IsLevelUseJson())
		{
			return mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mNumWaves;
		}
		if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			return 10;
		}
		if (mApp.IsSurvivalNormal(mApp.mGameMode))
		{
			return 10;
		}
		if (mApp.IsSurvivalHard(mApp.mGameMode))
		{
			return 20;
		}
		if (mApp.IsSurvivalHell(mApp.mGameMode))
		{
			if (mChallenge.mSurvivalStage >= 3)
			{
				return 30;
			}
			return 20;
		}
		if (mApp.IsSurvivalEndless(mApp.mGameMode))
		{
			return 20;
		}
		Debug.ASSERT(value: false);
		return -666;
	}

	public int GetLevelRandSeed()
	{
		int num = 101;
		int num2 = mBoardRandSeed + (int)mApp.mPlayerInfo.mId;
		if (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
		{
			return num2 + (mLevel + mApp.mPlayerInfo.mFinishedAdventure * num);
		}
		if (mApp.IsScaryPotterLevel() || mApp.IsIZombieLevel())
		{
			RandomNumbers.Seed();
			return RandomNumbers.NextNumber();
		}
		if (mApp.mGameMode == GameMode.ImitaterRandom || mApp.mGameMode == GameMode.ChallengeStageRandom || mApp.IsCornRouletteLevel())
		{
			return mBoardRandSeed + (int)mApp.mPlayerInfo.mId + (int)(DateTime.UtcNow.Ticks & 0x7FFFFFFF);
		}
		return (int)(num2 + (mApp.mGameMode + mChallenge.mSurvivalStage * num));
	}

	public void AddBossRenderItem(RenderItem[] theRenderList, ref int theCurRenderItem, Zombie theBossZombie)
	{
		Debug.ASSERT(theCurRenderItem < 2048);
		int theRow = 1;
		int theRow2 = 3;
		int theRow3 = 4;
		if (theBossZombie.IsDeadOrDying())
		{
			theRow3 = 1;
		}
		else if (theBossZombie.mZombiePhase == ZombiePhase.BossStomping)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(theBossZombie.mBodyReanimID);
			if (reanimation.mAnimTime > 0.25f && reanimation.mAnimTime < 0.75f)
			{
				if (theBossZombie.mTargetRow == 1)
				{
					theRow = 2;
				}
				else if (theBossZombie.mTargetRow == 3)
				{
					theRow2 = 4;
				}
			}
		}
		RenderItem renderItem = theRenderList[theCurRenderItem];
		renderItem.mRenderObjectType = RenderObjectType.BossPart;
		renderItem.mZPos = MakeRenderOrder(RenderLayer.Boss, theRow, 2);
		renderItem.mBossPart = BossPart.BackLeg;
		theCurRenderItem++;
		renderItem = theRenderList[theCurRenderItem];
		renderItem.mRenderObjectType = RenderObjectType.BossPart;
		renderItem.mZPos = MakeRenderOrder(RenderLayer.Boss, theRow2, 2);
		renderItem.mBossPart = BossPart.FrontLeg;
		theCurRenderItem++;
		renderItem = theRenderList[theCurRenderItem];
		renderItem.mRenderObjectType = RenderObjectType.BossPart;
		renderItem.mZPos = MakeRenderOrder(RenderLayer.Boss, 4, 2);
		renderItem.mBossPart = BossPart.Main;
		theCurRenderItem++;
		renderItem = theRenderList[theCurRenderItem];
		renderItem.mRenderObjectType = RenderObjectType.BossPart;
		renderItem.mZPos = MakeRenderOrder(RenderLayer.Boss, theRow3, 3);
		renderItem.mBossPart = BossPart.BackArm;
		theCurRenderItem++;
		Reanimation reanimation2 = mApp.ReanimationTryToGet(theBossZombie.mBossFireBallReanimID);
		if (reanimation2 != null)
		{
			renderItem = theRenderList[theCurRenderItem];
			renderItem.mRenderObjectType = RenderObjectType.BossPart;
			renderItem.mZPos = reanimation2.mRenderOrder;
			renderItem.mBossPart = BossPart.Fireball;
			theCurRenderItem++;
		}
	}

	public GridItem GetCraterAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.Crater, theGridX, theGridY);
	}

	public GridItem GetGraveStoneAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.Gravestone, theGridX, theGridY);
	}

	public GridItem GetTalismanAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.Talisman, theGridX, theGridY);
	}

	public GridItem GetLadderAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.Ladder, theGridX, theGridY);
	}

	public GridItem AddALadder(int theGridX, int theGridY)
	{
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.Ladder;
		newGridItem.mRenderOrder = 302000 + 10000 * theGridY + 800;
		newGridItem.mGridX = theGridX;
		newGridItem.mGridY = theGridY;
		mGridItems.Add(newGridItem);
		return newGridItem;
	}

	public GridItem AddACrater(int theGridX, int theGridY)
	{
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.Crater;
		newGridItem.mRenderOrder = 200000 + 10000 * theGridY + 1;
		newGridItem.mGridX = theGridX;
		newGridItem.mGridY = theGridY;
		mGridItems.Add(newGridItem);
		return newGridItem;
	}

	public void InitLawnMowers()
	{
		for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
		{
			if ((mApp.mGameMode == GameMode.ChallengeResodded && i <= 4) || ((mApp.IsAdventureMode() || mApp.IsQuickPlayMode()) && mLevel == 35) || (mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist && mApp.mGameMode != GameMode.ChallengeZombiquarium && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom && mApp.mGameMode != GameMode.ChallengeLastStand && !mApp.IsCornRouletteLevel() && !mApp.IsScaryPotterLevel() && !mApp.IsSquirrelLevel() && !mApp.IsIZombieLevel() && mApp.mGameMode != GameMode.ChallengeZombiquarium && (!StageHasRoof() || mApp.mPlayerInfo.mPurchases[23] != 0) && mPlantRow[i] != PlantRowType.Dirt))
			{
				LawnMower newLawnMower = LawnMower.GetNewLawnMower();
				newLawnMower.LawnMowerInitialize(i);
				newLawnMower.mVisible = false;
				mLawnMowers.Add(newLawnMower);
			}
		}
	}

	public bool IsPlantInCursor()
	{
		if (mCursorObject.mCursorType != CursorType.PlantFromBank && mCursorObject.mCursorType != CursorType.PlantFromUsableCoin && mCursorObject.mCursorType != CursorType.PlantFromGlove && mCursorObject.mCursorType != CursorType.PlantFromDuplicator)
		{
			return mCursorObject.mCursorType == CursorType.PlantFromWheelBarrow;
		}
		return true;
	}

	public void ClearFogAroundPlant(Plant thePlant, int theSize)
	{
		int num = 6;
		if (mFogBlownCountDown > 0 && mFogBlownCountDown < 2000)
		{
			num = 2;
		}
		else if (mFogBlownCountDown > 0)
		{
			num = 40;
		}
		int num2 = LeftFogColumn();
		for (int i = thePlant.mPlantCol - theSize; i <= thePlant.mPlantCol + theSize; i++)
		{
			int num3 = i + -(((int)mFogOffset + 50) / 100);
			for (int j = thePlant.mRow - theSize; j <= thePlant.mRow + theSize; j++)
			{
				if (num3 < num2 || num3 >= Constants.GRIDSIZEX || j < 0 || j >= 7)
				{
					continue;
				}
				int num4 = Math.Abs(i - thePlant.mPlantCol);
				int num5 = Math.Abs(j - thePlant.mRow);
				if (theSize == 4)
				{
					if (num4 > 3 || num5 > 2 || num4 + num5 == 5)
					{
						continue;
					}
				}
				else if (num4 + num5 > theSize)
				{
					continue;
				}
				mGridCelFog[num3, j] = Math.Max(mGridCelFog[num3, j] - num, 0);
			}
		}
	}

	public void RemoveParticleByType(ParticleEffect theEffectType)
	{
		int index = -1;
		TodParticleSystem theParticle = null;
		while (IterateParticles(ref theParticle, ref index))
		{
			if (theParticle.mEffectType == theEffectType)
			{
				theParticle.ParticleSystemDie();
			}
		}
	}

	public GridItem GetScaryPotAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.ScaryPot, theGridX, theGridY);
	}

	public void PuzzleSaveStreak()
	{
		if (mApp.IsEndlessScaryPotter(mApp.mGameMode) || mApp.IsEndlessIZombie(mApp.mGameMode))
		{
			int num = mChallenge.mSurvivalStage + 1;
			int currentChallengeIndex = mApp.GetCurrentChallengeIndex();
			if (num > mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex])
			{
				mApp.mPlayerInfo.mChallengeRecords[currentChallengeIndex] = num;
				mApp.WriteCurrentUserConfig();
			}
		}
	}

	public void ClearAdviceImmediately()
	{
		ClearAdvice(AdviceType.None);
		mAdvice.mDuration = 0;
	}

	public bool IsFinalScaryPotterStage()
	{
		if (!mApp.IsScaryPotterLevel())
		{
			return false;
		}
		if (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
		{
			return mChallenge.mSurvivalStage == 2;
		}
		return !mApp.IsEndlessScaryPotter(mApp.mGameMode);
	}

	public void DisplayAdviceAgain(string theAdvice, MessageStyle theMessageStyle, AdviceType theHelpIndex)
	{
		if (theHelpIndex != AdviceType.None && mHelpDisplayed[(int)theHelpIndex])
		{
			mHelpDisplayed[(int)theHelpIndex] = false;
		}
		DisplayAdvice(theAdvice, theMessageStyle, theHelpIndex);
	}

	public GridItem GetSquirrelAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.Squirrel, theGridX, theGridY);
	}

	public GridItem GetZenToolAt(int theGridX, int theGridY)
	{
		return GetGridItemAt(GridItemType.ZenTool, theGridX, theGridY);
	}

	public bool IsPlantInGoldWateringCanRange(int theMouseX, int theMouseY, Plant thePlant)
	{
		int num = theMouseX + Constants.ZenGarden_GoldenWater_Size.X;
		int num2 = theMouseX + Constants.ZenGarden_GoldenWater_Size.Width;
		int num3 = theMouseY + Constants.ZenGarden_GoldenWater_Size.Y;
		int num4 = theMouseY + Constants.ZenGarden_GoldenWater_Size.Height;
		if (GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.ZenToolOrder) == thePlant)
		{
			if (thePlant.mX + 40 >= num && thePlant.mX + 40 < num2 && thePlant.mY + 40 >= num3)
			{
				return thePlant.mY + 40 < num4;
			}
			return false;
		}
		return false;
	}

	public bool StageHasZombieWalkInFromRight()
	{
		if (!mApp.IsWhackAZombieLevel() && mApp.mGameMode != GameMode.ChallengeIce && !mApp.IsFinalBossLevel() && !mApp.IsIZombieLevel() && !mApp.IsSquirrelLevel() && !mApp.IsScaryPotterLevel() && mApp.mGameMode != GameMode.ChallengeZenGarden && mApp.mGameMode != GameMode.TreeOfWisdom)
		{
			return mApp.mGameMode != GameMode.ChallengeZombiquarium;
		}
		return false;
	}

	public bool CanPlaceRake()
	{
		if (!mApp.IsScaryPotterLevel())
		{
			if (StageHasZombieWalkInFromRight() && mApp.mGameMode != GameMode.ChallengeBeghouled)
			{
				return mApp.mGameMode != GameMode.ChallengeBeghouledTwist;
			}
			return false;
		}
		return true;
	}

	public void PlaceRake()
	{
		if (mApp.mPlayerInfo.mPurchases[24] == 0 || !mDoPlaceRose)
		{
			return;
		}
		int num;
		if (mApp.IsScaryPotterLevel())
		{
			num = 3;
			int index = -1;
			GridItem theGridItem = null;
			while (IterateGridItems(ref theGridItem, ref index))
			{
				if (theGridItem.mGridItemType == GridItemType.ScaryPot && theGridItem.mGridX <= num && theGridItem.mGridX > 0)
				{
					num = theGridItem.mGridX - 1;
				}
			}
		}
		else
		{
			if (!CanPlaceRake())
			{
				return;
			}
			num = 3;
		}
		int num2 = 0;
		for (int i = 0; i < aPickArray.Length; i++)
		{
			aPickArray[i].Reset();
		}
		for (int j = 0; j < Constants.MAX_GRIDSIZEY; j++)
		{
			if (j != 5 && (mPlantRow[j] == PlantRowType.Normal || mPlantRow[j] == PlantRowType.Shallow || mPlantRow[j] == PlantRowType.HighGround))
			{
				aPickArray[num2].mWeight = 1;
				aPickArray[num2].mItem = j;
				num2++;
			}
		}
		if (num2 != 0)
		{
			int mGridY = (int)TodCommon.TodPickFromWeightedArray(aPickArray, num2);
			mApp.mPlayerInfo.mPurchases[24]--;
			GridItem newGridItem = GridItem.GetNewGridItem();
			newGridItem.mGridItemType = GridItemType.Rake;
			newGridItem.mGridX = num;
			newGridItem.mGridY = mGridY;
			newGridItem.mPosX = GridToPixelX(newGridItem.mGridX, newGridItem.mGridY);
			newGridItem.mPosY = GridToPixelY(newGridItem.mGridX, newGridItem.mGridY);
			newGridItem.mRenderOrder = MakeRenderOrder(RenderLayer.GraveStone, newGridItem.mGridY, 9);
			mGridItems.Add(newGridItem);
			Reanimation theReanimation = CreateRakeReanim(newGridItem.mPosX, newGridItem.mPosY, 0);
			newGridItem.mGridItemReanimID = mApp.ReanimationGetID(theReanimation);
			newGridItem.mGridItemState = GridItemState.RakeAttracting;
		}
	}

	public Reanimation CreateRakeReanim(float rakeX, float rakeY, int renderOrder)
	{
		Reanimation reanimation = mApp.AddReanimation(rakeX + 20f, rakeY, renderOrder, ReanimationType.Rake);
		reanimation.mAnimRate = 0f;
		reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
		reanimation.mIsAttachment = true;
		return reanimation;
	}

	public GridItem GetRake()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Rake)
			{
				return theGridItem;
			}
		}
		return null;
	}

	public bool IsScaryPotterDaveTalking()
	{
		if (mApp.IsScaryPotterLevel() && mNextSurvivalStageCounter > 0)
		{
			return mApp.mCrazyDaveState != CrazyDaveState.Off;
		}
		return false;
	}

	public Zombie GetWinningZombie()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie.mFromWave == GameConstants.ZOMBIE_WAVE_WINNER)
			{
				return zombie;
			}
		}
		return null;
	}

	public void ResetFPSStats()
	{
		mIntervalDrawTime = (mStartDrawTime = Environment.TickCount);
		mDrawCount = 1;
		mIntervalDrawCountStart = 1;
	}

	public int CountEmptyPotsOrLilies(SeedType theSeedType)
	{
		int num = 0;
		int count = mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mPlants[i];
			if (!plant.mDead && plant.mSeedType == theSeedType && GetTopPlantAt(plant.mPlantCol, plant.mRow, TopPlant.OnlyNormalPosition) == null)
			{
				num++;
			}
		}
		return num;
	}

	public GridItem GetGridItemAt(GridItemType theGridItemType, int theGridX, int theGridY)
	{
		int index = -1;
		GridItem theGridItem = null;
		while (IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridX == theGridX && theGridItem.mGridY == theGridY && theGridItem.mGridItemType == theGridItemType)
			{
				return theGridItem;
			}
		}
		return null;
	}

	public bool ProgressMeterHasFlags()
	{
		if ((!mApp.IsFirstTimeAdventureMode() || mLevel != 1) && !mApp.IsWhackAZombieLevel() && !mApp.IsFinalBossLevel() && mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist && !mApp.IsSlotMachineLevel() && !mApp.IsSquirrelLevel())
		{
			return !mApp.IsIZombieLevel();
		}
		return false;
	}

	public bool IsLastStandFinalStage()
	{
		if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			return mChallenge.mSurvivalStage == 4;
		}
		return false;
	}

	public int GetNumWavesPerFlag()
	{
		if (mApp.IsLevelUseJson())
		{
			return mApp.mCreativeLevel.GetComponent<CSLevelProperty>().mWavesPerFlag;
		}
		if (mApp.IsFirstTimeAdventureMode() && mNumWaves < 10)
		{
			return mNumWaves;
		}
		return 10;
	}

	public int GetCurrentPlantCost(SeedType theSeedType, SeedType theImitaterType)
	{
		int num = Plant.GetCost(theSeedType, theImitaterType);
		if (PlantUsesAcceleratedPricing(theSeedType))
		{
			int num2 = CountPlantByType(theSeedType);
			num += num2 * 50;
		}
		return num;
	}

	public bool PlantUsesAcceleratedPricing(SeedType theSeedType)
	{
		if (!Plant.IsUpgrade(theSeedType))
		{
			return false;
		}
		if (mApp.IsSurvivalEndless(mApp.mGameMode))
		{
			return true;
		}
		if (mApp.IsLevelUseJson())
		{
			CSSurvivalLevel component = mApp.mCreativeLevel.GetComponent<CSSurvivalLevel>();
			if (component != null)
			{
				return component.mAcceleratedPricing;
			}
		}
		return false;
	}

	public void FreezeEffectsForCutscene(bool theFreeze)
	{
		int index = -1;
		TodParticleSystem theParticle = null;
		while (IterateParticles(ref theParticle, ref index))
		{
			if (theParticle.mEffectType == ParticleEffect.GraveBuster)
			{
				theParticle.mDontUpdate = theFreeze;
			}
			if ((theParticle.mEffectType == ParticleEffect.PoolSparkly || theParticle.mEffectType == ParticleEffect.PoolSparklyFull) && mIceTrapCounter == 0)
			{
				theParticle.mDontUpdate = theFreeze;
			}
		}
		int index2 = -1;
		Reanimation theReanimation = null;
		while (IterateReanimations(ref theReanimation, ref index2))
		{
			if (theReanimation.mReanimationType == ReanimationType.Sleeping)
			{
				if (theFreeze)
				{
					theReanimation.mAnimRate = 0f;
				}
				else
				{
					theReanimation.mAnimRate = TodCommon.RandRangeFloat(6f, 8f);
				}
			}
		}
	}

	public void LoadBackgroundImages()
	{
		if (mBackground == BackgroundType.Num1Day)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background1");
			if (mLevel <= 4 && mApp.IsFirstTimeAdventureMode())
			{
				TodCommon.TodLoadResources("DelayLoad_BackgroundUnsodded");
			}
			else if (mApp.mGameMode == GameMode.ChallengeResodded)
			{
				TodCommon.TodLoadResources("DelayLoad_BackgroundUnsodded");
			}
		}
		else if (mBackground == BackgroundType.Num2Night)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background2");
		}
		else if (mBackground == BackgroundType.Num3Pool)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background3");
		}
		else if (mBackground == BackgroundType.Num4Fog)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background4");
		}
		else if (mBackground == BackgroundType.Num5Roof)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background5");
		}
		else if (mBackground == BackgroundType.Num6Boss)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background6");
		}
		else if (mBackground == BackgroundType.HighGround)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background7");
		}
		else if (mBackground == BackgroundType.BigPool)
		{
			mApp.DelayLoadBackgroundResource("DelayLoad_Background8");
		}
		else if (mBackground == BackgroundType.Greenhouse)
		{
			mApp.DelayLoadZenGardenBackground("DelayLoad_GreenHouseGarden");
		}
		else if (mBackground != BackgroundType.TreeOfWisdom)
		{
			if (mBackground == BackgroundType.Zombiquarium)
			{
				mApp.DelayLoadZenGardenBackground("DelayLoad_Zombiquarium");
			}
			else if (mBackground == BackgroundType.MushroomGarden)
			{
				mApp.DelayLoadZenGardenBackground("DelayLoad_MushroomGarden");
			}
			else if (mBackground == BackgroundType.GreenhouseNight)
			{
				mApp.DelayLoadZenGardenBackground("DelayLoad_GreenHouseNight");
			}
			else if (mBackground == BackgroundType.ShallowDay)
			{
				mApp.DelayLoadBackgroundResource("DelayLoad_Background1");
			}
			else
			{
				Debug.ASSERT(value: false);
			}
		}
	}

	public bool CanUseGameObject(GameObjectType theGameObject)
	{
		if (mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			if (theGameObject != GameObjectType.TreeFood && theGameObject != GameObjectType.NextGarden)
			{
				return theGameObject == GameObjectType.PrevGarden;
			}
			return true;
		}
		if (mApp.mGameMode != GameMode.ChallengeZenGarden)
		{
			if (theGameObject == GameObjectType.Glove)
			{
				return IsCombatToolVisible(theGameObject);
			}
			return false;
		}
		switch (theGameObject)
		{
		case GameObjectType.WateringCan:
			return true;
		case GameObjectType.NextGarden:
			return true;
		case GameObjectType.PrevGarden:
			return true;
		case GameObjectType.Fertilizer:
			return mApp.mPlayerInfo.mPurchases[14] > 0;
		case GameObjectType.BugSpray:
			return mApp.mPlayerInfo.mPurchases[15] > 0;
		case GameObjectType.Phonograph:
			return mApp.mPlayerInfo.mPurchases[16] > 0;
		case GameObjectType.Chocolate:
			return mApp.mPlayerInfo.mPurchases[26] > 0;
		case GameObjectType.Wheelbarrow:
			return mApp.mPlayerInfo.mPurchases[19] > 0;
		case GameObjectType.Glove:
			return mApp.mPlayerInfo.mPurchases[17] > 0;
		case GameObjectType.MoneySign:
			return mApp.HasFinishedAdventure();
		case GameObjectType.TreeFood:
			return false;
		default:
			Debug.ASSERT(value: false);
			return false;
		}
	}

	public void SetMustacheMode(bool theEnableMustache)
	{
		mApp.PlayFoley(FoleyType.Polevault);
		mMustacheMode = theEnableMustache;
		mApp.mMustacheMode = theEnableMustache;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead)
			{
				zombie.EnableMustache(theEnableMustache);
			}
		}
	}

	public int CountCoinByType(CoinType theCoinType)
	{
		int num = 0;
		Coin theCoin = null;
		while (IterateCoins(ref theCoin))
		{
			if (theCoin.mType == theCoinType)
			{
				num++;
			}
		}
		return num;
	}

	public void SetSuperMowerMode(bool theEnableSuperMower)
	{
		mApp.PlayFoley(FoleyType.Zamboni);
		mSuperMowerMode = theEnableSuperMower;
		mApp.mSuperMowerMode = theEnableSuperMower;
		LawnMower theLawnMower = null;
		while (IterateLawnMowers(ref theLawnMower))
		{
			theLawnMower.EnableSuperMower(theEnableSuperMower);
		}
	}

	public void DrawZenWheelBarrowButton(Graphics g, int theOffsetY)
	{
		TRect zenButtonRect = GetZenButtonRect(GameObjectType.Wheelbarrow);
		PottedPlant pottedPlantInWheelbarrow = mApp.mZenGarden.GetPottedPlantInWheelbarrow();
		if (pottedPlantInWheelbarrow != null && mCursorObject.mCursorType != CursorType.PlantFromWheelBarrow)
		{
			if (mChallenge.mChallengeState == ChallengeState.ZenFading)
			{
				g.DrawImage(AtlasResources.IMAGE_ZEN_WHEELBARROW, zenButtonRect.mX + Constants.ZenGardenButton_Wheelbarrow_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Wheelbarrow_Offset.Y + theOffsetY);
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_ZEN_WHEELBARROW, zenButtonRect.mX + Constants.ZenGardenButton_Wheelbarrow_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Wheelbarrow_Offset.Y + theOffsetY);
			}
			if (pottedPlantInWheelbarrow.mPlantAge != PottedPlantAge.Small)
			{
				PottedPlantAge mPlantAge = pottedPlantInWheelbarrow.mPlantAge;
			}
			mApp.mZenGarden.DrawPottedPlant(g, zenButtonRect.mX + Constants.ZenGardenButton_WheelbarrowPlant_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_WheelbarrowPlant_Offset.Y + theOffsetY, pottedPlantInWheelbarrow, 0.6f, theDrawPot: true);
		}
		else
		{
			g.DrawImage(AtlasResources.IMAGE_ZEN_WHEELBARROW, zenButtonRect.mX + Constants.ZenGardenButton_Wheelbarrow_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Wheelbarrow_Offset.Y + theOffsetY);
		}
	}

	public void DrawZenButtons(Graphics g)
	{
		int num = 0;
		if (mChallenge.mChallengeState == ChallengeState.ZenFading)
		{
			num = TodCommon.TodAnimateCurve(50, 0, mChallenge.mChallengeStateCounter, 0, -72, TodCurves.EaseInOut);
		}
		for (int i = 6; i <= 16; i++)
		{
			GameObjectType gameObjectType = (GameObjectType)i;
			if (!CanUseGameObject(gameObjectType))
			{
				continue;
			}
			TRect zenButtonRect = GetZenButtonRect(gameObjectType);
			if (gameObjectType != GameObjectType.NextGarden && gameObjectType != GameObjectType.PrevGarden && (mApp.mPlayerInfo.mZenGardenTutorialComplete || gameObjectType != GameObjectType.MoneySign))
			{
				g.DrawImage(AtlasResources.IMAGE_SHOVELBANK_ZEN, zenButtonRect.mX, zenButtonRect.mY + num);
				if (mCursorObject.mCursorType == (CursorType)(9 + i - 6))
				{
					continue;
				}
			}
			switch (gameObjectType)
			{
			case GameObjectType.WateringCan:
				if (mApp.mPlayerInfo.mPurchases[13] != 0)
				{
					if ((uint)mApp.mPlayerInfo.mPurchases[13] >= 2u)
					{
						g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_DIAMOND, zenButtonRect.mX + Constants.ZenGardenButton_GoldenWateringCan_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_GoldenWateringCan_Offset.Y + num);
					}
					else
					{
						g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_GOLD, zenButtonRect.mX + Constants.ZenGardenButton_GoldenWateringCan_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_GoldenWateringCan_Offset.Y + num);
					}
				}
				else
				{
					g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1, zenButtonRect.mX + Constants.ZenGardenButton_NormalWateringCan_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_NormalWateringCan_Offset.Y + num);
				}
				continue;
			case GameObjectType.Fertilizer:
			{
				int num2 = mApp.mPlayerInfo.mPurchases[14] - 1000;
				if (num2 == 0)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(96, 96, 96));
				}
				else if (mTutorialState == TutorialState.ZenGardenFertilizePlants)
				{
					SexyColor flashingColor = TodCommon.GetFlashingColor(mMainCounter, 75);
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(flashingColor);
				}
				g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_FERTILIZER_BAG2, zenButtonRect.mX + Constants.ZenGardenButton_Fertiliser_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Fertiliser_Offset.Y + num);
				g.SetColorizeImages(colorizeImages: false);
				if (!cachedChargesStringsFertilizer.TryGetValue(num2, out var value))
				{
					value = "x" + num2;
					cachedChargesStringsFertilizer.Add(num2, value);
				}
				TodCommon.TodDrawString(g, value, zenButtonRect.mX + Constants.ZenGardenButtonCounterOffset.X, zenButtonRect.mY + Constants.ZenGardenButtonCounterOffset.Y + num, Resources.FONT_HOUSEOFTERROR16, SexyColor.White, DrawStringJustification.Right, 0.6f);
				continue;
			}
			case GameObjectType.BugSpray:
			{
				int num3 = mApp.mPlayerInfo.mPurchases[15] - 1000;
				if (num3 == 0)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(128, 128, 128));
				}
				g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_BUGSPRAY_BOTTLE, zenButtonRect.mX + Constants.ZenGardenButton_BugSpray_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_BugSpray_Offset.Y + num);
				g.SetColorizeImages(colorizeImages: false);
				if (!cachedChargesStringsBugSpray.TryGetValue(num3, out var value2))
				{
					value2 = Common.StrFormat_(TodStringFile.TodStringTranslate("[BUG_SPRAY_MULTIPLIED_X]"), num3);
					cachedChargesStringsBugSpray.Add(num3, value2);
				}
				TodCommon.TodDrawString(g, value2, zenButtonRect.mX + Constants.ZenGardenButtonCounterOffset.X, zenButtonRect.mY + Constants.ZenGardenButtonCounterOffset.Y + num, Resources.FONT_HOUSEOFTERROR16, SexyColor.White, DrawStringJustification.Right, 0.6f);
				continue;
			}
			case GameObjectType.Phonograph:
				g.DrawImage(AtlasResources.IMAGE_PHONOGRAPH, zenButtonRect.mX + Constants.ZenGardenButton_Phonograph_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Phonograph_Offset.Y + num);
				continue;
			case GameObjectType.Chocolate:
			{
				int num4 = mApp.mPlayerInfo.mPurchases[26] - 1000;
				if (num4 == 0)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(128, 128, 128));
				}
				g.DrawImage(AtlasResources.IMAGE_CHOCOLATE, zenButtonRect.mX + Constants.ZenGardenButton_Chocolate_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Chocolate_Offset.Y + num);
				g.SetColorizeImages(colorizeImages: false);
				if (!cachedChargesStringsChocolate.TryGetValue(num4, out var value3))
				{
					value3 = Common.StrFormat_(TodStringFile.TodStringTranslate("[CHOCOLATE_MULTIPLIED_X]"), num4);
					cachedChargesStringsChocolate.Add(num4, value3);
				}
				TodCommon.TodDrawString(g, value3, zenButtonRect.mX + Constants.ZenGardenButtonCounterOffset.X, zenButtonRect.mY + Constants.ZenGardenButtonCounterOffset.Y + num, Resources.FONT_HOUSEOFTERROR16, SexyColor.White, DrawStringJustification.Right, 0.6f);
				continue;
			}
			case GameObjectType.Glove:
				if (mCursorObject.mCursorType != CursorType.PlantFromGlove && mCursorObject.mCursorType != CursorType.PlantFromWheelBarrow)
				{
					Image theImage = ((mApp.mGameMode == GameMode.ChallengeZenGarden) ? AtlasResources.IMAGE_ZEN_GARDENGLOVE : AtlasResources.IMAGE_GLOVE_PLANTGIRL);
					g.DrawImage(theImage, zenButtonRect.mX + Constants.ZenGardenButton_Glove_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_Glove_Offset.Y + num);
					if (HasGlove() && mChallenge.mGloveCounter > 0)
					{
						int gloveCounterMax = mChallenge.GetGloveCounterMax();
						int theHeight = TodCommon.FloatRoundToInt((float)(zenButtonRect.mHeight * mChallenge.mGloveCounter) / (float)gloveCounterMax) + 2;
						g.SetColor(new SexyColor(0, 0, 0, 150));
						g.FillRect(zenButtonRect.mX, zenButtonRect.mY, zenButtonRect.mWidth, theHeight);
					}
					continue;
				}
				break;
			}
			if (gameObjectType == GameObjectType.MoneySign && mApp.mPlayerInfo.mZenGardenTutorialComplete)
			{
				g.DrawImage(AtlasResources.IMAGE_ZEN_MONEYSIGN, zenButtonRect.mX + Constants.ZenGardenButton_MoneySign_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_MoneySign_Offset.Y + num);
				continue;
			}
			switch (gameObjectType)
			{
			case GameObjectType.Wheelbarrow:
				DrawZenWheelBarrowButton(g, num);
				break;
			case GameObjectType.NextGarden:
				if (!mMenuButton.mBtnNoDraw)
				{
					g.DrawImage(AtlasResources.IMAGE_ZEN_NEXT_GARDEN, zenButtonRect.mX + Constants.ZenGardenButton_NextGarden_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_NextGarden_Offset.Y + num);
				}
				break;
			case GameObjectType.PrevGarden:
				if (!mMenuButton.mBtnNoDraw)
				{
					g.DrawImage(AtlasResources.IMAGE_ZEN_PREV_GARDEN, zenButtonRect.mX + Constants.ZenGardenButton_NextGarden_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_NextGarden_Offset.Y + num);
				}
				break;
			case GameObjectType.TreeFood:
			{
				int num5 = mApp.mPlayerInfo.mPurchases[28] - 1000;
				if (num5 <= 0)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(128, 128, 128));
					num5 = 0;
				}
				if (!mChallenge.TreeOfWisdomCanFeed())
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(128, 128, 128));
				}
				g.DrawImage(AtlasResources.IMAGE_REANIM_TREEFOOD, zenButtonRect.mX + Constants.ZenGardenButton_NormalWateringCan_Offset.X, zenButtonRect.mY + Constants.ZenGardenButton_NormalWateringCan_Offset.Y + num);
				g.SetColorizeImages(colorizeImages: false);
				string theText = Common.StrFormat_(TodStringFile.TodStringTranslate("[TREE_FOOD_MULTIPLIED_X]"), num5);
				TodCommon.TodDrawString(g, theText, zenButtonRect.mX + Constants.ZenGardenButtonCounterOffset.X, zenButtonRect.mY + Constants.ZenGardenButtonCounterOffset.Y + num, Resources.FONT_HOUSEOFTERROR16, SexyColor.White, DrawStringJustification.Right, 0.6f);
				break;
			}
			}
		}
	}

	public void OffsetYForPlanting(ref int theY, SeedType theSeedType)
	{
		if (Plant.IsFlying(theSeedType) || theSeedType == SeedType.Gravebuster)
		{
			theY += 15;
		}
		if (theSeedType == SeedType.Spikeweed || theSeedType == SeedType.Spikerock)
		{
			theY -= 15;
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && mBackground == BackgroundType.Greenhouse)
		{
			theY -= 25;
		}
	}

	public void SetFutureMode(bool theEnableFuture)
	{
		mApp.PlaySample(Resources.SOUND_BOING);
		mFutureMode = theEnableFuture;
		mApp.mFutureMode = theEnableFuture;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead)
			{
				zombie.EnableFuture(theEnableFuture);
			}
		}
	}

	public void SetPinataMode(bool theEnablePinata)
	{
		mApp.PlayFoley(FoleyType.Juicy);
		mPinataMode = theEnablePinata;
		mApp.mPinataMode = theEnablePinata;
	}

	public void SetDanceMode(bool theEnableDance)
	{
		mApp.PlayFoley(FoleyType.Dancer);
		mDanceMode = theEnableDance;
		mApp.mDanceMode = theEnableDance;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead)
			{
				zombie.EnableDanceMode(theEnableDance);
			}
		}
	}

	public void SetDaisyMode(bool theEnableDaisy)
	{
		mApp.PlaySample(Resources.SOUND_LOADINGBAR_FLOWER);
		mDaisyMode = theEnableDaisy;
		mApp.mDaisyMode = theEnableDaisy;
	}

	public void SetSukhbirMode(bool theEnableSukhbir)
	{
		mSukhbirMode = theEnableSukhbir;
		mApp.mSukhbirMode = theEnableSukhbir;
	}

	public bool MouseHitTestPlant(int x, int y, out HitResult theHitResult, bool posScaled)
	{
		theHitResult = default;
		if (mCursorObject.mCursorType == CursorType.CobcannonTarget || mCursorObject.mCursorType == CursorType.Hammer)
		{
			return false;
		}
		if (!posScaled)
		{
			x = (int)((float)x * Constants.IS);
			y = (int)((float)y * Constants.IS);
		}
		Plant plant = SpecialPlantHitTest(x, y);
		if (plant != null)
		{
			theHitResult.mObject = plant;
			theHitResult.mObjectType = GameObjectType.Plant;
			return true;
		}
		int theGridX = PixelToGridX(x, y);
		int theGridY = PixelToGridY(x, y);
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			plant = GetTopPlantAt(theGridX, theGridY, TopPlant.ZenToolOrder);
			if (mCursorObject.mCursorType == CursorType.WateringCan && (plant == null || !mApp.mZenGarden.PlantCanBeWatered(plant)))
			{
				int theGridX2 = PixelToGridX(x - 30, y - 20);
				int theGridY2 = PixelToGridY(x - 30, y - 20);
				Plant topPlantAt = GetTopPlantAt(theGridX2, theGridY2, TopPlant.ZenToolOrder);
				if (topPlantAt != null && mApp.mZenGarden.PlantCanBeWatered(topPlantAt))
				{
					plant = topPlantAt;
				}
			}
		}
		else
		{
			plant = GetTopPlantAt(theGridX, theGridY, TopPlant.DiggingOrder);
			if (plant != null && (plant.mSeedType == SeedType.Lilypad || plant.mSeedType == SeedType.Flowerpot) && GetTopPlantAt(theGridX, theGridY, TopPlant.OnlyPumpkin) != null)
			{
				plant = null;
			}
		}
		if (plant != null && mCursorObject.mCursorType == CursorType.Chocolate && !mApp.mZenGarden.PlantCanHaveChocolate(plant))
		{
			theHitResult.mObject = null;
			theHitResult.mObjectType = GameObjectType.None;
			return false;
		}
		if (plant != null)
		{
			theHitResult.mObject = plant;
			theHitResult.mObjectType = GameObjectType.Plant;
			return true;
		}
		return false;
	}

	public void DoTypingCheck(KeyCode theKey)
	{
		if (mApp.mKonamiCheck.Check(theKey))
		{
			mApp.PlayFoley(FoleyType.Drop);
		}
		else if (mApp.mMustacheCheck.Check(theKey) || mApp.mMoustacheCheck.Check(theKey))
		{
			SetMustacheMode(!mMustacheMode);
		}
		else if (mApp.mSuperMowerCheck.Check(theKey) || mApp.mSuperMowerCheck2.Check(theKey))
		{
			SetSuperMowerMode(!mSuperMowerMode);
		}
		else if (mApp.mFutureCheck.Check(theKey))
		{
			SetFutureMode(!mFutureMode);
		}
		else if (mApp.mPinataCheck.Check(theKey))
		{
			if (mApp.CanDoPinataMode())
			{
				SetPinataMode(!mPinataMode);
				return;
			}
			if (mApp.mGameScene == GameScenes.Playing)
			{
				DisplayAdvice("[CANT_USE_CODE]", MessageStyle.BigMiddleFast, AdviceType.None);
			}
			mApp.PlaySample(Resources.SOUND_BUZZER);
		}
		else if (mApp.mDanceCheck.Check(theKey))
		{
			if (mApp.CanDoDanceMode())
			{
				SetDanceMode(!mDanceMode);
				return;
			}
			if (mApp.mGameScene == GameScenes.Playing)
			{
				DisplayAdvice("[CANT_USE_CODE]", MessageStyle.BigMiddleFast, AdviceType.None);
			}
			mApp.PlaySample(Resources.SOUND_BUZZER);
		}
		else if (!mApp.mDaisyCheck.Check(theKey))
		{
			if (mApp.mSukhbirCheck.Check(theKey))
			{
				SetSukhbirMode(!mSukhbirMode);
			}
		}
		else if (mApp.CanDoDaisyMode())
		{
			SetDaisyMode(!mDaisyMode);
		}
		else
		{
			if (mApp.mGameScene == GameScenes.Playing)
			{
				DisplayAdvice("[CANT_USE_CODE]", MessageStyle.BigMiddleFast, AdviceType.None);
			}
			mApp.PlaySample(Resources.SOUND_BUZZER);
		}
	}

	public void CompleteEndLevelSequenceForSaving()
	{
		if (CanDropLoot() && !mApp.IsDIYMode())
		{
			LawnMower theLawnMower = null;
			while (IterateLawnMowers(ref theLawnMower))
			{
				if (theLawnMower.mMowerState != LawnMowerState.Triggered && theLawnMower.mMowerState != LawnMowerState.Squished)
				{
					int coinValue = Coin.GetCoinValue(CoinType.Gold);
					mApp.mPlayerInfo.AddCoins(coinValue);
					mCoinsCollected += coinValue;
				}
			}
		}
		Coin theCoin = null;
		while (IterateCoins(ref theCoin))
		{
			if (theCoin.mIsBeingCollected)
			{
				theCoin.ScoreCoin();
			}
			else
			{
				theCoin.Die();
			}
		}
		CheckForPostGameAchievements();
		mApp.UpdatePlayerProfileForFinishingLevel();
	}

	public void RemoveZombiesForRepick()
	{
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying() && zombie.mMindControlled && zombie.mPosX > 720f)
			{
				zombie.DieNoLoot(giveAchievements: false);
			}
		}
	}

	public bool IsSurvivalStageWithRepick()
	{
		if (mApp.IsSurvivalMode())
		{
			return !IsFinalSurvivalStage();
		}
		return false;
	}

	public bool IsLastStandStageWithRepick()
	{
		if (mApp.mGameMode == GameMode.ChallengeLastStand)
		{
			return !IsLastStandFinalStage();
		}
		return false;
	}

	public bool GrantAchievement(AchievementId theAchievement)
	{
		return GrantAchievement(theAchievement, show: true);
	}

	public bool GrantAchievement(AchievementId theAchievement, bool show)
	{
		return ReportAchievement.GiveAchievement(theAchievement);
	}

	public bool CheckForPostGameAchievements()
	{
		if (mApp.IsWhackAZombieLevel() || mApp.IsScaryPotterLevel() || mApp.IsWallnutBowlingLevel())
		{
			return false;
		}
		bool flag = false;
		if (mApp.IsAdventureMode() && mLevel == 50 && mApp.mPlayerInfo.mFinishedAdventure < 1)
		{
			flag = GrantAchievement(AchievementId.ACHIEVEMENT_HOME_SECURITY, show: false) | flag;
		}
		return flag;
	}

	public int GetLiveGargantuarCount()
	{
		int num = 0;
		int count = mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mZombies[i];
			if (!zombie.mDead && zombie.mHasHead && !zombie.IsDeadOrDying() && zombie.IsOnBoard() && (zombie.mZombieType == ZombieType.Gargantuar || zombie.mZombieType == ZombieType.RedeyeGargantuar))
			{
				num++;
			}
		}
		return num;
	}

	public void ButtonMouseMove(int id, int x, int y)
	{
	}

	public void ButtonMouseTick(int id)
	{
	}

	public void ButtonPress(int id, int id2)
	{
	}

	public void ButtonDownTick(int id)
	{
	}

	public void ButtonDepress(int id)
	{
	}

	public override bool BackButtonPress()
	{
		if (IsGridFocusActive || mCursorObject.mMagnifier)
		{
			mCursorObject.mMagnifier = false;
			LeaveGridFocus();
			return true;
		}
		if ((mApp.mGameMode == GameMode.ChallengeZenGarden && mChallenge.mChallengeState != ChallengeState.ZenPlantInteracting) || mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			mApp.mBoardResult = BoardResult.Quit;
			TryToSaveGame();
			mApp.DoBackToMain();
			return true;
		}
		if (!CanInteractWithBoardButtons())
		{
			return true;
		}
		RefreshSeedPacketFromCursor();
		mApp.DoNewOptions(theFromGameSelector: false);
		return true;
	}

	public bool SaveToFile(Sexy.Buffer b)
	{
		LeaderBoardComm.RecordResult(LeaderboardGameMode.Adventure, (int)mApp.mPlayerInfo.mZombiesKilled);
		try
		{
			b.WriteLong(mLevel);
			b.WriteLong((int)mApp.mGameScene);
			b.WriteLong(mPeashootersPlanted);
			b.WriteBoolean(mNomNomNomAchievementTracker);
			b.WriteBoolean(mNoFungusAmongUsAchievementTracker);
			b.WriteLong((int)mBackground);
			b.WriteLong(mBoardFadeOutCounter);
			b.WriteLong(mBoardRandSeed);
			b.WriteLong(mBonusLawnMowersRemaining);
			b.WriteBoolean(mCatapultPlantsUsed);
			mChallenge.SaveToFile(b);
			b.WriteLong(mChocolateCollected);
			b.WriteLong(mCobCannonCursorDelayCounter);
			b.WriteLong(mCobCannonMouseX);
			b.WriteLong(mCobCannonMouseY);
			b.WriteLong(mCoinBankFadeCount);
			b.WriteLong(mCoins.Count);
			for (int i = 0; i < mCoins.Count; i++)
			{
				mCoins[i].SaveToFile(b);
			}
			b.WriteLong(mCoinsCollected);
			b.WriteLong(mCollectedCoinStreak);
			b.WriteLong(mCurrentWave);
			mCursorObject.SaveToFile(b);
			b.WriteBoolean(mDaisyMode);
			b.WriteBoolean(mDanceMode);
			b.WriteLong(mDiamondsCollected);
			b.WriteLong(mDoomsUsed);
			b.WriteBoolean(mDroppedFirstCoin);
			b.WriteLong(mEffectCounter);
			b.WriteBoolean(mEnableGraveStones);
			b.WriteBoolean(mFinalBossKilled);
			b.WriteLong(mFinalWaveSoundCounter);
			b.WriteLong(mFlagRaiseCounter);
			b.WriteLong(mFogBlownCountDown);
			b.WriteFloat(mFogOffset);
			b.WriteLong(mFwooshCountDown);
			b.WriteLong(mGameID);
			b.WriteLong(mGargantuarsKillsByCornCob);
			b.WriteLong(mGravesCleared);
			b.WriteLong2DArray(mGridCelFog);
			b.WriteLong2DArray(mGridCelLook);
			b.WriteLong3DArray(mGridCelOffset);
			b.WriteLong(mGridItems.Count);
			for (int j = 0; j < mGridItems.Count; j++)
			{
				mGridItems[j].SaveToFile(b);
			}
			int length = mGridSquareType.GetLength(0);
			int length2 = mGridSquareType.GetLength(1);
			b.WriteLong(length);
			b.WriteLong(length2);
			for (int k = 0; k < length; k++)
			{
				for (int l = 0; l < length2; l++)
				{
					b.WriteLong((int)mGridSquareType[k, l]);
				}
			}
			b.WriteBooleanArray(mHelpDisplayed);
			b.WriteLong((int)mHelpIndex);
			b.WriteLong(mHugeWaveCountDown);
			b.WriteLongArray(mIceMinX);
			b.WriteLongArray(mIceTimer);
			b.WriteLong(mIceTrapCounter);
			b.WriteBoolean(mIgnoreMouseUp);
			b.WriteBoolean(mKilledYeti);
			b.WriteLong(mLastBungeeWave);
			b.WriteLong(mLastToolX);
			b.WriteLong(mLastToolY);
			b.WriteLong(mLastWMUpdateCount);
			b.WriteLong(mLawnMowers.Count);
			for (int m = 0; m < mLawnMowers.Count; m++)
			{
				mLawnMowers[m].SaveToFile(b);
			}
			b.WriteBoolean(mLevelAwardSpawned);
			b.WriteBoolean(mLevelComplete);
			b.WriteLong(mLevelFadeCount);
			b.WriteString(mLevelStr);
			b.WriteLong(mMainCounter);
			b.WriteLong(mMaxSunPlants);
			b.WriteFloat(mMinFPS);
			b.WriteBoolean(mMushroomAndCoffeeBeansOnly);
			b.WriteBoolean(mMushroomsUsed);
			b.WriteLong(mNextSurvivalStageCounter);
			b.WriteLong(mNumSunsFallen);
			b.WriteLong(mNumWaves);
			b.WriteBoolean(mNutsUsed);
			b.WriteLong(mOutOfMoneyCounter);
			b.WriteBoolean(mPeaShooterUsed);
			b.WriteBoolean(mPinataMode);
			b.WriteBoolean(mPlanternOrBloverUsed);
			b.WriteLong(mPlants.Count);
			for (int n = 0; n < mPlants.Count; n++)
			{
				mPlants[n].SaveToFile(b);
			}
			b.WriteLong(mPlantsEaten);
			b.WriteLong(mPlantsShoveled);
			b.WriteLong(mPlayTimeActiveLevel);
			b.WriteLong(mPlayTimeInactiveLevel);
			b.WriteLong(mPottedPlantsCollected);
			b.WriteLong(mProgressMeterWidth);
			b.WriteLong(mProjectiles.Count);
			for (int num = 0; num < mProjectiles.Count; num++)
			{
				mProjectiles[num].SaveToFile(b);
			}
			b.WriteLong(mRiseFromGraveCounter);
			b.WriteLong(mRowPickingArray.Length);
			for (int num2 = 0; num2 < mRowPickingArray.Length; num2++)
			{
				mRowPickingArray[num2].SaveToFile(b);
			}
			b.WriteLong(mScoreNextMowerCounter);
			b.WriteBoolean(mShowShovel);
			mSeedBank.SaveToFile(b);
			b.WriteLong(mSodPosition);
			b.WriteLong(mSpecialGraveStoneX);
			b.WriteLong(mSpecialGraveStoneY);
			b.WriteLong(mSunCountDown);
			b.WriteLong(mSunMoney);
			b.WriteBoolean(mSuperMowerMode);
			b.WriteLong(mTimeStopCounter);
			b.WriteLong(mTotalSpawnedWaves);
			b.WriteLong(mTriggeredLawnMowers);
			b.WriteLong((int)mTutorialState);
			b.WriteLong(mTutorialTimer);
			b.WriteLongArray(mWaveRowGotLawnMowered);
			b.WriteBooleanArray(mZombieAllowed);
			b.WriteLong(mZombieCountDown);
			b.WriteLong(mZombieCountDownStart);
			b.WriteLong(mZombieHealthToNextWave);
			b.WriteLong(mZombieHealthWaveStart);
			b.WriteLong(mZombies.Count);
			for (int num3 = 0; num3 < mZombies.Count; num3++)
			{
				mZombies[num3].SaveToFile(b);
			}
			length = mZombiesInWave.GetLength(0);
			length2 = mZombiesInWave.GetLength(1);
			b.WriteLong(length);
			b.WriteLong(length2);
			for (int num4 = 0; num4 < length; num4++)
			{
				for (int num5 = 0; num5 < length2; num5++)
				{
					b.WriteLong((int)mZombiesInWave[num4, num5]);
				}
			}
			b.WriteLong(777);
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
		return true;
	}

	public void AttachCamera(Camera theCamera)
	{
		mCamera = theCamera;
		mCameraEnabled = true;
	}

	public void DetachCamera()
	{
		mCameraEnabled = false;
		mCamera = null;
	}

	public void BackFromAlmanac()
	{
		LoadBackgroundImages();
	}
}
