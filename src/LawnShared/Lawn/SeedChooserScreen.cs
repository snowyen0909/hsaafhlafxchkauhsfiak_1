using System.Collections.Generic;
using System.Linq;
using Lawn.Creative;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class SeedChooserScreen : Widget, StoreListener, AlmanacListener, LawnMessageBoxListener, SeedPacketsWidgetListener
{
	public GameButton mStartButton;

	public GameButton mRandomButton;

	public GameButton mViewLawnButton;

	public GameButton mStoreButton;

	public GameButton mAlmanacButton;

	public GameButton mLastCardButton;

	public GameButton mMenuButton;

	public GameButton mImitaterButton;

	public GameButton mCardGroupButton;

	public GameButton mLoadButton;

	public GameButton mSaveButton;

	public GameButton mRoseButton;

	public SeedPacketsWidget mSeedPacketsWidget;

	public ScrollWidget mScrollWidget;

	public CardGroupWidget mCardGroupWidget;

	public ScrollWidget mScrollWidget2;

	public ChosenSeed[] mChosenSeeds = new ChosenSeed[System.Math.Max((int)SeedType.SeedTypeCount, ModPlants.MaxSeed + 1)];

	public LawnApp mApp;

	public Board mBoard;

	public int mNumSeedsToChoose;

	public int mSeedChooserAge;

	public int mSeedsInFlight;

	public int mSeedsInBank;

	public int mLastMouseX;

	public int mLastMouseY;

	public SeedChooserState mChooseState;

	public int mViewLawnTime;

	public bool mDoStartButton;

	public bool[] mPickWarningsWaved = new bool[20];

	public int mPendingWarningId;

	public bool mInCardGroupScreen;

	private static TodWeightedArray[] aSeedArray;

	private static Dictionary<int, string> cachedChargesStringsRose;

	/// <summary>Seeds shown in the chooser grid (Peashooter .. Imitater).</summary>
	/// <summary>Seeds shown in the chooser grid: Peashooter .. Imitater, then the plant-mod plants (fixed after start-up).</summary>
	public static SeedType[] ChooserSeedTypes => sChooserSeedTypes ??= BuildChooserSeedTypes();

	private static SeedType[] sChooserSeedTypes;

	private static SeedType[] BuildChooserSeedTypes()
	{
		List<SeedType> list = new List<SeedType>();
		for (int i = 0; i < (int)SeedType.ExplodeONut; i++)
		{
			list.Add((SeedType)i);
		}
		foreach (ModPlantSpec p in ModPlants.Plants)
		{
			list.Add(p.Seed);
		}
		return list.ToArray();
	}

	public static bool IsChooserSeed(SeedType theSeedType)
	{
		return (theSeedType > SeedType.None && theSeedType < SeedType.ExplodeONut) || ModPlants.IsMod(theSeedType);
	}

	static SeedChooserScreen()
	{
		aSeedArray = new TodWeightedArray[(int)SeedType.SeedTypeCount];
		cachedChargesStringsRose = new Dictionary<int, string>();
		for (int i = 0; i < aSeedArray.Length; i++)
		{
			aSeedArray[i] = TodWeightedArray.GetNewTodWeightedArray();
		}
	}

	public SeedChooserScreen()
	{
		mZOrder = -1;
		mApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		mBoard = mApp.mBoard;
		mClip = false;
		mSeedsInFlight = 0;
		mSeedsInBank = 0;
		mLastMouseX = -1;
		mLastMouseY = -1;
		mChooseState = SeedChooserState.Normal;
		mViewLawnTime = 0;
		mDoStartButton = false;
		mSeedPacketsWidget = new SeedPacketsWidget(mApp, Has12Rows() ? 14 : 11, theIsImitaters: false, this);
		mScrollWidget = new ScrollWidget();
		mScrollWidget.Resize(Constants.SCROLL_AREA_OFFSET_X, Constants.SCROLL_AREA_OFFSET_Y, mSeedPacketsWidget.mWidth + (int)Constants.InvertAndScaleUI(10f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(227f)) : ((int)Constants.InvertAndScale(227f) + 30));
		mScrollWidget.AddWidget(mSeedPacketsWidget);
		mScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mSeedPacketsWidget.Move(0, 0);
		AddWidget(mScrollWidget);
		mCardGroupWidget = new CardGroupWidget(mApp, this);
		mScrollWidget2 = new ScrollWidget();
		mScrollWidget2.Resize(Constants.SCROLL_AREA_OFFSET_X, Constants.SCROLL_AREA_OFFSET_Y, mCardGroupWidget.mWidth + (int)Constants.InvertAndScaleUI(10f), mCardGroupWidget.mHeight + (int)Constants.InvertAndScaleUI(10f));
		mScrollWidget2.AddWidget(mCardGroupWidget);
		mScrollWidget2.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mCardGroupWidget.Move(0, 0);
		AddWidget(mScrollWidget2);
		mStartButton = new GameButton(100, this);
		mStartButton.SetLabel("[PLAY_BUTTON]");
		mStartButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_SMALL_BUTTON;
		mStartButton.mOverImage = null;
		mStartButton.mDownImage = AtlasResources.IMAGE_SEEDCHOOSER_SMALL_BUTTON_PRESSED;
		mStartButton.mDisabledImage = AtlasResources.IMAGE_SEEDCHOOSER_SMALL_BUTTON_DISABLED;
		mStartButton.mOverOverlayImage = null;
		mStartButton.SetFont(Resources.FONT_DWARVENTODCRAFT15);
		mStartButton.mColors[1] = new SexyColor(255, 231, 26);
		mStartButton.mColors[0] = new SexyColor(255, 231, 26);
		mStartButton.Resize((int)Constants.InvertAndScaleUI(147f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(280f)) : ((int)Constants.InvertAndScale(280f)), (int)Constants.InvertAndScaleUI(67f), mStartButton.mButtonImage.GetHeight());
		mStartButton.mTextOffsetX = 0;
		mStartButton.mTextOffsetX = 0;
		mStartButton.mTextPushOffsetX = 1;
		mStartButton.mTextPushOffsetY = 3;
		EnableStartButton(theEnabled: false);
		mMenuButton = new GameButton(104, this);
		mMenuButton.SetLabel("[MENU_BUTTON]");
		mMenuButton.Resize(Constants.UIMenuButtonPosition.X + Constants.Board_Offset_AspectRatio_Correction, Constants.UIMenuButtonPosition.Y, Constants.UIMenuButtonWidth, AtlasResources.IMAGE_BUTTON_LEFT.mHeight);
		mMenuButton.mDrawStoneButton = true;
		mRandomButton = new GameButton(101, this);
		mRandomButton.SetLabel("(Debug Play)");
		mRandomButton.mButtonImage = AtlasResources.IMAGE_BLANK;
		mRandomButton.mOverImage = AtlasResources.IMAGE_BLANK;
		mRandomButton.mDownImage = AtlasResources.IMAGE_BLANK;
		mRandomButton.SetFont(Resources.FONT_BRIANNETOD12);
		mRandomButton.mColors[0] = new SexyColor(255, 240, 0);
		mRandomButton.mColors[1] = new SexyColor(200, 200, 255);
		mRandomButton.Resize((int)Constants.InvertAndScaleUI(332f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(546f)) : ((int)Constants.InvertAndScale(546f)), (int)Constants.InvertAndScaleUI(100f), (int)Constants.InvertAndScaleUI(30f));
		if (!mApp.mTodCheatKeys)
		{
			mRandomButton.mBtnNoDraw = true;
			mRandomButton.mDisabled = true;
		}
		mViewLawnButton = new GameButton(102, this);
		mViewLawnButton.SetLabel("[VIEW_LAWN]");
		mViewLawnButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2;
		mViewLawnButton.mOverImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW;
		mViewLawnButton.mDownImage = null;
		mViewLawnButton.SetFont(Resources.FONT_BRIANNETOD12);
		mViewLawnButton.mColors[0] = new SexyColor(42, 42, 90);
		mViewLawnButton.mColors[1] = new SexyColor(42, 42, 90);
		mViewLawnButton.Resize((int)Constants.InvertAndScaleUI(22f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(300f)) : ((int)Constants.InvertAndScale(300f)), (int)Constants.InvertAndScaleUI(111f), (int)Constants.InvertAndScaleUI(26f));
		mViewLawnButton.mParentWidget = this;
		mViewLawnButton.mTextOffsetY = 1;
		if (!mBoard.mCutScene.IsSurvivalRepick())
		{
			mViewLawnButton.mBtnNoDraw = true;
			mViewLawnButton.mDisabled = true;
		}
		mAlmanacButton = new GameButton(103, this);
		mAlmanacButton.SetLabel("[ALMANAC_BUTTON]");
		mAlmanacButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2;
		mAlmanacButton.mOverImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW;
		mAlmanacButton.mDownImage = null;
		mAlmanacButton.SetFont(Resources.FONT_BRIANNETOD12);
		mAlmanacButton.mColors[0] = new SexyColor(42, 42, 90);
		mAlmanacButton.mColors[1] = new SexyColor(42, 42, 90);
		mAlmanacButton.Resize((int)Constants.InvertAndScaleUI(63f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(286f)) : ((int)Constants.InvertAndScale(286f)), AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2.mWidth, (int)Constants.InvertAndScaleUI(26f));
		mAlmanacButton.mParentWidget = this;
		mAlmanacButton.mTextOffsetY = (int)Constants.InvertAndScaleUI(2f);
		mStoreButton = new GameButton(105, this);
		mStoreButton.SetLabel("[SHOP_BUTTON]");
		mStoreButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2;
		mStoreButton.mOverImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW;
		mStoreButton.mDownImage = null;
		mStoreButton.SetFont(Resources.FONT_BRIANNETOD12);
		mStoreButton.mColors[0] = new SexyColor(42, 42, 90);
		mStoreButton.mColors[1] = new SexyColor(42, 42, 90);
		mStoreButton.Resize((int)Constants.InvertAndScaleUI(218f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(286f)) : ((int)Constants.InvertAndScale(286f)), AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2.mWidth, (int)Constants.InvertAndScaleUI(26f));
		mStoreButton.mParentWidget = this;
		mStoreButton.mTextOffsetY = (int)Constants.InvertAndScaleUI(2f);
		mImitaterButton = new GameButton(106, this);
		mImitaterButton.mButtonImage = null;
		mImitaterButton.mOverImage = null;
		mImitaterButton.mDownImage = null;
		mImitaterButton.mDisabledImage = null;
		mImitaterButton.Resize((int)Constants.InvertAndScale(248f), (int)Constants.InvertAndScale(27f), Constants.SMALL_SEEDPACKET_WIDTH, Constants.SMALL_SEEDPACKET_HEIGHT);
		mImitaterButton.mParentWidget = this;
		mLastCardButton = new GameButton(107, this);
		mLastCardButton.SetLabel("[LAST_CARD_BUTTON]");
		mLastCardButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2;
		mLastCardButton.mOverImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW;
		mLastCardButton.mDownImage = null;
		mLastCardButton.SetFont(Resources.FONT_BRIANNETOD12);
		mLastCardButton.mColors[0] = new SexyColor(42, 42, 90);
		mLastCardButton.mColors[1] = new SexyColor(42, 42, 90);
		mLastCardButton.Resize((int)Constants.InvertAndScale(248f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(227f)) : ((int)Constants.InvertAndScale(227f)), AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2.mWidth, (int)Constants.InvertAndScaleUI(26f));
		mLastCardButton.mParentWidget = this;
		mLastCardButton.mTextOffsetY = (int)Constants.InvertAndScale(2f);
		mCardGroupButton = new GameButton(108, this);
		mCardGroupButton.mButtonImage = null;
		mCardGroupButton.mOverImage = null;
		mCardGroupButton.mDownImage = null;
		mCardGroupButton.mDisabledImage = null;
		mCardGroupButton.Resize((int)Constants.InvertAndScale(248f), (int)Constants.InvertAndScale(77f), Constants.SMALL_SEEDPACKET_WIDTH, Constants.SMALL_SEEDPACKET_HEIGHT);
		mCardGroupButton.mParentWidget = this;
		mRoseButton = new GameButton(111, this);
		mRoseButton.mButtonImage = null;
		mRoseButton.mOverImage = null;
		mRoseButton.mDownImage = null;
		mRoseButton.mDisabledImage = null;
		mRoseButton.Resize((int)Constants.InvertAndScale(248f), (int)Constants.InvertAndScale(127f), Constants.SMALL_SEEDPACKET_WIDTH, Constants.SMALL_SEEDPACKET_HEIGHT);
		mRoseButton.mParentWidget = this;
		mRoseButton.mChecked = mBoard.CanPlaceRake();
		mLoadButton = new GameButton(109, this);
		mLoadButton.SetLabel("[LOAD_BUTTON]");
		mLoadButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2;
		mLoadButton.mOverImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW;
		mLoadButton.mDownImage = null;
		mLoadButton.SetFont(Resources.FONT_BRIANNETOD12);
		mLoadButton.mColors[0] = new SexyColor(42, 42, 90);
		mLoadButton.mColors[1] = new SexyColor(42, 42, 90);
		mLoadButton.Resize((int)Constants.InvertAndScaleUI(63f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(286f)) : ((int)Constants.InvertAndScale(286f)), AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2.mWidth, (int)Constants.InvertAndScaleUI(26f));
		mLoadButton.mParentWidget = this;
		mLoadButton.mTextOffsetY = (int)Constants.InvertAndScaleUI(2f);
		mSaveButton = new GameButton(110, this);
		mSaveButton.SetLabel("[SAVE_BUTTON]");
		mSaveButton.mButtonImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2;
		mSaveButton.mOverImage = AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW;
		mSaveButton.mDownImage = null;
		mSaveButton.SetFont(Resources.FONT_BRIANNETOD12);
		mSaveButton.mColors[0] = new SexyColor(42, 42, 90);
		mSaveButton.mColors[1] = new SexyColor(42, 42, 90);
		mSaveButton.Resize((int)Constants.InvertAndScaleUI(218f), Constants.Is500pMode ? ((int)Constants.InvertAndScaleUI(286f)) : ((int)Constants.InvertAndScale(286f)), AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2.mWidth, (int)Constants.InvertAndScaleUI(26f));
		mSaveButton.mParentWidget = this;
		mSaveButton.mTextOffsetY = (int)Constants.InvertAndScaleUI(2f);
		if (!mApp.CanShowAlmanac())
		{
			mAlmanacButton.mBtnNoDraw = true;
			mAlmanacButton.mDisabled = true;
		}
		if (!mApp.CanShowStore())
		{
			mStoreButton.mBtnNoDraw = true;
			mStoreButton.mDisabled = true;
		}
		int num = mApp.mPlayerInfo.mPurchases[36];
		if (num <= 0)
		{
			mCardGroupButton.mBtnNoDraw = true;
			mCardGroupButton.mDisabled = true;
		}
		else
		{
			mCardGroupWidget.mCols = num;
		}
		if (mApp.mPlayerInfo.mPurchases[24] <= 0)
		{
			mRoseButton.mBtnNoDraw = true;
			mRoseButton.mDisabled = true;
		}
		SetInCardGroupScreen(isIn: false);
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			int board_Offset_AspectRatio_Correction = Constants.Board_Offset_AspectRatio_Correction;
			int num2 = 87;
			int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
			int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			int theX = board_Offset_AspectRatio_Correction + 22;
			int theY = num2 + 36;
			int num3 = 6;
			int num4 = 6;
			int num5 = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_IMITATERADDON.mWidth - num4;
			int num6 = board_Offset_AspectRatio_Correction + AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BACKGROUND.mWidth - num4 + 5;
			mScrollWidget.Resize(theX, theY, mSeedPacketsWidget.mWidth + (int)Constants.InvertAndScaleUI(10f), num3 * celHeight);
			mScrollWidget2.Resize(theX, theY, mCardGroupWidget.mWidth + (int)Constants.InvertAndScaleUI(10f), num3 * celHeight);
			mStartButton.SetLabel("[LETS_ROCK_BUTTON]");
			mStartButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON;
			mStartButton.mOverImage = null;
			mStartButton.mDownImage = null;
			mStartButton.mDisabledImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON_DISABLED;
			mStartButton.mOverOverlayImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON_GLOW;
			mStartButton.SetFont(Resources.FONT_DWARVENTODCRAFT12);
			mStartButton.mColors[0] = new SexyColor(64, 64, 64);
			mStartButton.mColors[1] = Constants.YellowFontColour;
			mStartButton.mTextOffsetY = -1;
			mStartButton.mTextPushOffsetX = 0;
			mStartButton.mTextPushOffsetY = 0;
			mStartButton.Resize(board_Offset_AspectRatio_Correction + 154, 545, mStartButton.mButtonImage.mWidth, mStartButton.mButtonImage.mHeight);
			mRandomButton.Resize(board_Offset_AspectRatio_Correction + 332, 546, 100, 30);
			mViewLawnButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2;
			mViewLawnButton.mOverImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2_GLOW;
			mAlmanacButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2;
			mAlmanacButton.mOverImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2_GLOW;
			mStoreButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2;
			mStoreButton.mOverImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2_GLOW;
			mLoadButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2;
			mLoadButton.mOverImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2_GLOW;
			mSaveButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2;
			mSaveButton.mOverImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2_GLOW;
			mLastCardButton.mButtonImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2;
			mLastCardButton.mOverImage = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2_GLOW;
			mViewLawnButton.mTextOffsetY = 1;
			mAlmanacButton.mTextOffsetY = 1;
			mStoreButton.mTextOffsetY = 1;
			mLoadButton.mTextOffsetY = 1;
			mSaveButton.mTextOffsetY = 1;
			mLastCardButton.mTextOffsetY = 1;
			int theWidth = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2.mWidth;
			int theHeight = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BUTTON2.mHeight;
			mViewLawnButton.Resize(theX, 561, theWidth, theHeight);
			mAlmanacButton.Resize(board_Offset_AspectRatio_Correction + 560, 572, theWidth, theHeight);
			mStoreButton.Resize(board_Offset_AspectRatio_Correction + 680, 572, theWidth, theHeight);
			mLoadButton.Resize(board_Offset_AspectRatio_Correction + 560, 572, theWidth, theHeight);
			mSaveButton.Resize(board_Offset_AspectRatio_Correction + 680, 572, theWidth, theHeight);
			mMenuButton.Resize(board_Offset_AspectRatio_Correction + 681, -10, 117, 46);
			mImitaterButton.Resize(num6, 515, celWidth, celHeight);
			mCardGroupButton.Resize(num6 + num5, 515, celWidth, celHeight);
			mRoseButton.Resize(num6 + num5 * 2, 515, celWidth, celHeight);
			mLastCardButton.Resize(num6 + num5 * 3, 515, theWidth, theHeight);
		}
		foreach (SeedType seedType in ChooserSeedTypes)
		{
			ChosenSeed chosenSeed = new ChosenSeed
			{
				mSeedType = seedType
			};
			GetSeedPositionInChooser((int)seedType, ref chosenSeed.mX, ref chosenSeed.mY);
			chosenSeed.mTimeStartMotion = 0;
			chosenSeed.mTimeEndMotion = 0;
			chosenSeed.mStartX = chosenSeed.mX;
			chosenSeed.mStartY = chosenSeed.mY;
			chosenSeed.mEndX = chosenSeed.mX;
			chosenSeed.mEndY = chosenSeed.mY;
			chosenSeed.mSeedState = ChosenSeedState.SEED_IN_CHOOSER;
			chosenSeed.mSeedIndexInBank = 0;
			chosenSeed.mRefreshCounter = 0;
			chosenSeed.mRefreshing = false;
			chosenSeed.mImitaterType = SeedType.None;
			chosenSeed.mCrazyDavePicked = false;
			if (seedType == SeedType.Imitater)
			{
				chosenSeed.mSeedState = ChosenSeedState.SEED_PACKET_HIDDEN;
			}
			mChosenSeeds[(int)seedType] = chosenSeed;
		}
		if (mBoard.mCutScene.IsSurvivalRepick())
		{
			for (int i = 0; i < mBoard.mSeedBank.mNumPackets; i++)
			{
				SeedPacket seedPacket = mBoard.mSeedBank.mSeedPackets[i];
				SeedType mPacketType = seedPacket.mPacketType;
				if (mPacketType != SeedType.None)
				{
					ChosenSeed chosenSeed2 = mChosenSeeds[(int)mPacketType];
					chosenSeed2.mRefreshing = seedPacket.mRefreshing;
					chosenSeed2.mRefreshCounter = seedPacket.mRefreshCounter;
				}
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeSeeingStars)
		{
			ChosenSeed chosenSeed3 = mChosenSeeds[29];
			GetSeedPositionInBank(0, ref chosenSeed3.mX, ref chosenSeed3.mY);
			chosenSeed3.mEndX = chosenSeed3.mX;
			chosenSeed3.mEndY = chosenSeed3.mY;
			chosenSeed3.mStartX = chosenSeed3.mX;
			chosenSeed3.mStartY = chosenSeed3.mY;
			chosenSeed3.mSeedState = ChosenSeedState.SEED_IN_BANK;
			chosenSeed3.mSeedIndexInBank = 0;
			mSeedsInBank++;
		}
		if ((mApp.IsAdventureMode() && !mApp.IsFirstTimeAdventureMode()) || mApp.mGameMode == GameMode.ChallengeStageRandom)
		{
			CrazyDavePickSeeds();
		}
		if (mApp.IsSurvivalMode() && mBoard.ChooseSeedsOnCurrentLevel())
		{
			for (int j = 0; j < mBoard.mSeedBank.mNumPackets; j++)
			{
				if (mSeedsInBank >= mBoard.mSeedBank.mNumPackets)
				{
					break;
				}
				SeedPacket seedPacket2 = mBoard.mSeedBank.mSeedPackets[j];
				SeedType mPacketType2 = seedPacket2.mPacketType;
				if (mPacketType2 != SeedType.None)
				{
					ChosenSeed chosenSeed4 = mChosenSeeds[(int)mPacketType2];
					if (mPacketType2 == SeedType.Imitater && seedPacket2.mImitaterType != SeedType.None)
					{
						chosenSeed4.mImitaterType = seedPacket2.mImitaterType;
					}
					PreChooseSeed(mPacketType2, locked: false);
				}
			}
		}
		if (mApp.IsLevelUseJson())
		{
			CSSeedBank component = mApp.mCreativeLevel.GetComponent<CSSeedBank>();
			if (component != null && component.mLockedCards != null)
			{
				for (int k = 0; k < component.mLockedCards.Count; k++)
				{
					CardPair cardPair = component.mLockedCards[k];
					PreChooseSeed(cardPair.mSeedType, locked: true, cardPair.mImitator);
				}
			}
		}
		UpdateImitaterButton();
		for (int l = 0; l < mPickWarningsWaved.Length; l++)
		{
			mPickWarningsWaved[l] = false;
		}
	}

	public void PreChooseSeed(SeedType seedType, bool locked, bool imitator = false)
	{
		if (mSeedsInBank >= mBoard.mSeedBank.mNumPackets)
		{
			return;
		}
		SeedType seedType2 = (imitator ? SeedType.Imitater : seedType);
		if (seedType2 >= SeedType.Peashooter && (int)seedType2 < mChosenSeeds.Length)
		{
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType2];
			if (imitator)
			{
				chosenSeed.mImitaterType = seedType;
			}
			GetSeedPositionInBank(mSeedsInBank, ref chosenSeed.mX, ref chosenSeed.mY);
			chosenSeed.mEndX = chosenSeed.mX;
			chosenSeed.mEndY = chosenSeed.mY;
			chosenSeed.mStartX = chosenSeed.mX;
			chosenSeed.mStartY = chosenSeed.mY;
			chosenSeed.mSeedState = ChosenSeedState.SEED_IN_BANK;
			chosenSeed.mSeedIndexInBank = mSeedsInBank;
			chosenSeed.mCrazyDavePicked = locked;
			mSeedsInBank++;
			EnableStartButton(mSeedsInBank == mBoard.mSeedBank.mNumPackets);
		}
	}

	public override void Dispose()
	{
		mStartButton.Dispose();
		mRandomButton.Dispose();
		mViewLawnButton.Dispose();
		mAlmanacButton.Dispose();
		mLastCardButton.Dispose();
		mImitaterButton.Dispose();
		mCardGroupButton.Dispose();
		mRoseButton.Dispose();
		mLoadButton.Dispose();
		mSaveButton.Dispose();
		mStoreButton.Dispose();
		mMenuButton.Dispose();
		RemoveAllWidgets(doDelete: true);
	}

	public override void Update()
	{
		base.Update();
		mLastMouseX = mApp.mWidgetManager.mLastMouseX;
		mLastMouseY = mApp.mWidgetManager.mLastMouseY;
		mSeedChooserAge++;
		foreach (SeedType seedType in ChooserSeedTypes)
		{
			if (mApp.HasSeedType(seedType))
			{
				ChosenSeed theChosenSeed = mChosenSeeds[(int)seedType];
				if (theChosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_BANK || theChosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_CHOOSER)
				{
					theChosenSeed.mX = TodCommon.TodAnimateCurve(theChosenSeed.mTimeStartMotion, theChosenSeed.mTimeEndMotion, mSeedChooserAge, theChosenSeed.mStartX, theChosenSeed.mEndX, TodCurves.EaseInOut);
					theChosenSeed.mY = TodCommon.TodAnimateCurve(theChosenSeed.mTimeStartMotion, theChosenSeed.mTimeEndMotion, mSeedChooserAge, theChosenSeed.mStartY, theChosenSeed.mEndY, TodCurves.EaseInOut);
				}
				if (theChosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_BANK && mSeedChooserAge >= theChosenSeed.mTimeEndMotion)
				{
					LandFlyingSeed(ref theChosenSeed);
				}
				if (theChosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_CHOOSER && mSeedChooserAge >= theChosenSeed.mTimeEndMotion)
				{
					LandFlyingSeed(ref theChosenSeed);
				}
			}
		}
		mStartButton.Update();
		mRandomButton.Update();
		mViewLawnButton.Update();
		mAlmanacButton.Update();
		mLastCardButton.Update();
		mImitaterButton.Update();
		mCardGroupButton.Update();
		mRoseButton.Update();
		mLoadButton.Update();
		mSaveButton.Update();
		mStoreButton.Update();
		mMenuButton.Update();
		UpdateViewLawn();
		if (mDoStartButton)
		{
			mDoStartButton = false;
			OnStartButton();
		}
		MarkDirty();
	}

	public override void Draw(Graphics g)
	{
		if (mApp.GetDialog(Dialogs.DIALOG_STORE) != null || mApp.GetDialog(Dialogs.DIALOG_ALMANAC) != null)
		{
			return;
		}
		g.SetLinearBlend(linear: true);
		if (!mBoard.ChooseSeedsOnCurrentLevel() || (mBoard.mCutScene != null && mBoard.mCutScene.IsBeforePreloading()))
		{
			return;
		}
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			int board_Offset_AspectRatio_Correction = Constants.Board_Offset_AspectRatio_Correction;
			int theY = 87;
			int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
			int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			int num = 6;
			int num2 = board_Offset_AspectRatio_Correction + AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BACKGROUND.mWidth - num;
			int num3 = AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_IMITATERADDON.mWidth - num;
			g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_BACKGROUND, board_Offset_AspectRatio_Correction, theY);
			int thePosY = 110 + Resources.FONT_DWARVENTODCRAFT12.GetAscent();
			TodCommon.TodDrawString(g, "[CHOOSE_YOUR_PLANTS]", board_Offset_AspectRatio_Correction + 229, thePosY, Resources.FONT_DWARVENTODCRAFT12, Constants.YellowFontColour, DrawStringJustification.Center);
			if (mApp.HasSeedType(SeedType.Imitater))
			{
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_IMITATERADDON, num2, 503);
				if (!mImitaterButton.mDisabled)
				{
					SeedPacket.DrawClassicSeedPacket(g, mImitaterButton.mX, mImitaterButton.mY, SeedType.Imitater, SeedType.None, 0f, 255, theDrawCost: false, theUseCurrentCost: false);
				}
			}
			if (!mCardGroupButton.mDisabled)
			{
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_IMITATERADDON, num2 + num3, 503);
				float num4 = (float)celWidth / (float)AtlasResources.IMAGE_CARD_GROUP.mWidth;
				float thePosY2 = (float)mCardGroupButton.mY + ((float)celHeight - (float)AtlasResources.IMAGE_CARD_GROUP.mHeight * num4) * 0.5f;
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_CARD_GROUP, mCardGroupButton.mX, thePosY2, num4, num4);
				if (mInCardGroupScreen)
				{
					float num5 = (float)celWidth / (float)AtlasResources.IMAGE_CARD_GROUP_TAB_SELECT.mWidth;
					float thePosY3 = (float)mCardGroupButton.mY + ((float)celHeight - (float)AtlasResources.IMAGE_CARD_GROUP_TAB_SELECT.mHeight * num5) * 0.5f;
					TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_CARD_GROUP_TAB_SELECT, mCardGroupButton.mX, thePosY3, num5, num5);
				}
			}
			if (!mRoseButton.mDisabled)
			{
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDCHOOSER_IMITATERADDON, num2 + num3 * 2, 503);
				Image image = (mRoseButton.mChecked ? AtlasResources.IMAGE_SEEDPACKET_ZORROSE : AtlasResources.IMAGE_SEEDPACKET_ZORROSE_DISABLED);
				Image image2 = (mRoseButton.mChecked ? AtlasResources.IMAGE_SEEDPACKET_SYMBOL_ENABLE : AtlasResources.IMAGE_SEEDPACKET_SYMBOL_DISABLE);
				float num6 = (float)celWidth / (float)image.mWidth;
				float thePosY4 = (float)mRoseButton.mY + ((float)celHeight - (float)image.mHeight * num6) * 0.5f;
				TodCommon.TodDrawImageScaledF(g, image, mRoseButton.mX, thePosY4, num6, num6);
				float num7 = (float)celWidth / (float)image2.mWidth;
				float thePosY5 = (float)mRoseButton.mY + ((float)celHeight - (float)image2.mHeight * num7) * 0.5f;
				TodCommon.TodDrawImageScaledF(g, image2, mRoseButton.mX, thePosY5, num7, num7);
				int key = mApp.mPlayerInfo.mPurchases[24];
				if (!cachedChargesStringsRose.TryGetValue(key, out var value))
				{
					value = "x" + key;
					cachedChargesStringsRose.Add(key, value);
				}
				TodCommon.TodDrawString(g, value, mRoseButton.mX + celWidth - 5, mRoseButton.mY + 5, Resources.FONT_HOUSEOFTERROR16, SexyColor.White, DrawStringJustification.Right, 0.6f);
			}
			for (int i = 0; i < mBoard.mSeedBank.mNumPackets; i++)
			{
				SeedType seedType = FindSeedInBank(i);
				int x = 0;
				int y = 0;
				GetSeedPositionInBank(i, ref x, ref y);
				if (seedType == SeedType.None)
				{
					g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDPACKETSILHOUETTE, x, y);
					continue;
				}
				ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
				SeedPacket.DrawClassicSeedPacket(g, x, y, seedType, chosenSeed.mImitaterType, 0f, 255, theDrawCost: true, theUseCurrentCost: false);
				if (chosenSeed.mCrazyDavePicked)
				{
					g.DrawImage(AtlasResources.IMAGE_LOCK, x, y + 18);
				}
			}
			DeferOverlay();
			mStartButton.Draw(g);
			mRandomButton.Draw(g);
			mViewLawnButton.Draw(g);
			mAlmanacButton.Draw(g);
			mLastCardButton.Draw(g);
			mStoreButton.Draw(g);
			mLoadButton.Draw(g);
			mSaveButton.Draw(g);
			Graphics graphics = Graphics.GetNew(g);
			graphics.mTransX -= mX;
			graphics.mTransY -= mY;
			mMenuButton.Draw(graphics);
			graphics.PrepareForReuse();
			return;
		}
		g.DrawImage(AtlasResources.IMAGE_SEEDCHOOSER_BACKGROUND_TOP, Constants.SeedChooserScreen_Background_Top.X, Constants.SeedChooserScreen_Background_Top.Y);
		g.DrawImage(AtlasResources.IMAGE_SEEDCHOOSER_BACKGROUND_MIDDLE, Constants.SeedChooserScreen_Background_Middle.X, Constants.SeedChooserScreen_Background_Middle.Y, AtlasResources.IMAGE_SEEDCHOOSER_BACKGROUND_MIDDLE.mWidth, Constants.SeedChooserScreen_Background_Middle_Height);
		g.DrawImage(AtlasResources.IMAGE_SEEDCHOOSER_BACKGROUND_BOTTOM, Constants.SeedChooserScreen_Background_Bottom.X, Constants.SeedChooserScreen_Background_Bottom.Y);
		if (mApp.HasSeedType(SeedType.Imitater))
		{
			g.DrawImage(AtlasResources.IMAGE_SEEDCHOOSER_IMITATERADDON, (int)Constants.InvertAndScale(243f), (int)Constants.InvertAndScale(18f));
			if (!mImitaterButton.mDisabled)
			{
				SeedPacket.DrawSmallSeedPacket(g, mImitaterButton.mX, mImitaterButton.mY, SeedType.Imitater, SeedType.None, 0f, 255, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false);
			}
		}
		if (!mCardGroupButton.mDisabled)
		{
			g.DrawImage(AtlasResources.IMAGE_SEEDCHOOSER_IMITATERADDON, (int)Constants.InvertAndScale(243f), (int)Constants.InvertAndScale(68f));
			g.DrawImage(AtlasResources.IMAGE_CARD_GROUP, mCardGroupButton.mX, mCardGroupButton.mY - (int)Constants.InvertAndScale(7f), AtlasResources.IMAGE_CARD_GROUP.mWidth / 2, AtlasResources.IMAGE_CARD_GROUP.mHeight / 2);
			if (mInCardGroupScreen)
			{
				g.DrawImage(AtlasResources.IMAGE_CARD_GROUP_TAB_SELECT, mCardGroupButton.mX - 5, mCardGroupButton.mY - 4);
			}
		}
		if (!mRoseButton.mDisabled)
		{
			g.DrawImage(AtlasResources.IMAGE_SEEDCHOOSER_IMITATERADDON, (int)Constants.InvertAndScale(243f), (int)Constants.InvertAndScale(118f));
			if (!mRoseButton.mChecked)
			{
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_ZORROSE_DISABLED, mRoseButton.mX, mRoseButton.mY);
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_SYMBOL_DISABLE, mRoseButton.mX, mRoseButton.mY);
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_ZORROSE, mRoseButton.mX, mRoseButton.mY);
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_SYMBOL_ENABLE, mRoseButton.mX, mRoseButton.mY);
			}
			int key2 = mApp.mPlayerInfo.mPurchases[24];
			if (!cachedChargesStringsRose.TryGetValue(key2, out var value2))
			{
				value2 = "x" + key2;
				cachedChargesStringsRose.Add(key2, value2);
			}
			TodCommon.TodDrawString(g, value2, mRoseButton.mX + AtlasResources.IMAGE_SEEDPACKET_ZORROSE.mWidth - 5, mRoseButton.mY + 5, Resources.FONT_HOUSEOFTERROR16, SexyColor.White, DrawStringJustification.Right, 0.6f);
		}
		int mNumPackets = mBoard.mSeedBank.mNumPackets;
		for (int j = 0; j < mNumPackets; j++)
		{
			SeedType seedType2 = FindSeedInBank(j);
			int x2 = 0;
			int y2 = 0;
			GetSeedPositionInBank(j, ref x2, ref y2);
			if (seedType2 != SeedType.None)
			{
				ChosenSeed chosenSeed2 = mChosenSeeds[(int)seedType2];
				SeedPacket.DrawSmallSeedPacket(g, x2, y2, seedType2, chosenSeed2.mImitaterType, 0f, 255, theDrawCost: true, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: true);
				if (chosenSeed2.mCrazyDavePicked)
				{
					g.DrawImage(AtlasResources.IMAGE_LOCK, x2, y2 + (int)Constants.InvertAndScale(13f));
				}
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKETSILHOUETTE, x2, y2);
			}
		}
		DeferOverlay();
		mStartButton.Draw(g);
		mRandomButton.Draw(g);
		mViewLawnButton.Draw(g);
		mAlmanacButton.Draw(g);
		mLastCardButton.Draw(g);
		mStoreButton.Draw(g);
		mLoadButton.Draw(g);
		mSaveButton.Draw(g);
		Graphics graphics2 = Graphics.GetNew(g);
		graphics2.mTransX -= mX;
		graphics2.mTransY -= mY;
		mMenuButton.Draw(graphics2);
		graphics2.PrepareForReuse();
	}

	public override void DrawOverlay(Graphics g)
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			bool flag = false;
			foreach (SeedType seedType in ChooserSeedTypes)
			{
				if (!mApp.HasSeedType(seedType))
				{
					continue;
				}
				ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
				if (chosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_BANK)
				{
					g.SetClipRect(0, 0, mWidth, mHeight);
				}
				else
				{
					if (chosenSeed.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER)
					{
						continue;
					}
					g.SetClipRect(0, 0, mWidth, mScrollWidget.mY + mScrollWidget.mHeight);
					if (!flag)
					{
						g.HardwareClip();
						flag = true;
					}
				}
				SeedPacket.DrawClassicSeedPacket(g, chosenSeed.mX, chosenSeed.mY, chosenSeed.mSeedType, chosenSeed.mImitaterType, 0f, 255, theDrawCost: true, theUseCurrentCost: false);
			}
			if (flag)
			{
				g.EndHardwareClip();
			}
			g.ClearClipRect();
			g.SetColorizeImages(colorizeImages: false);
			return;
		}
		g.SetColor(Constants.SeedChooserScreen_BackColour);
		g.SetColorizeImages(colorizeImages: true);
		if (mInCardGroupScreen)
		{
			if (mCardGroupWidget.mY < 0)
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_TOPGRADIENT, mScrollWidget2.mX + Constants.SeedChooserScreen_Gradient_Top.X, Constants.SeedChooserScreen_Gradient_Top.Y, Constants.SeedChooserScreen_Gradient_Top.Width, Constants.SeedChooserScreen_Gradient_Top.Height);
			}
		}
		else
		{
			if (mSeedPacketsWidget.mY < 0)
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_TOPGRADIENT, mScrollWidget.mX + Constants.SeedChooserScreen_Gradient_Top.X, Constants.SeedChooserScreen_Gradient_Top.Y, Constants.SeedChooserScreen_Gradient_Top.Width, Constants.SeedChooserScreen_Gradient_Top.Height);
			}
			if ((float)(mSeedPacketsWidget.mY + mSeedPacketsWidget.mHeight) > Constants.InvertAndScale(227f))
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_BOTTOMGRADIENT, mScrollWidget.mX + Constants.SeedChooserScreen_Gradient_Bottom.X, Constants.SeedChooserScreen_Gradient_Bottom.Y, Constants.SeedChooserScreen_Gradient_Bottom.Width, Constants.SeedChooserScreen_Gradient_Bottom.Height);
			}
		}
		g.SetColorizeImages(colorizeImages: false);
		bool flag2 = false;
		foreach (SeedType seedType2 in ChooserSeedTypes)
		{
			if (!mApp.HasSeedType(seedType2))
			{
				continue;
			}
			ChosenSeed chosenSeed2 = mChosenSeeds[(int)seedType2];
			if (chosenSeed2.mSeedState == ChosenSeedState.SEED_FLYING_TO_BANK)
			{
				g.SetClipRect(0, 0, mWidth, mHeight);
			}
			else
			{
				if (chosenSeed2.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER)
				{
					continue;
				}
				g.SetClipRect(0, 0, mWidth, mScrollWidget.mY + mScrollWidget.mHeight);
				g.HardwareClip();
				flag2 = true;
			}
			SeedPacket.DrawSmallSeedPacket(g, chosenSeed2.mX, chosenSeed2.mY, chosenSeed2.mSeedType, chosenSeed2.mImitaterType, 0f, 255, theDrawCost: true, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: true);
		}
		if (flag2)
		{
			g.EndHardwareClip();
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public virtual void ButtonPress(int theId)
	{
	}

	public override bool BackButtonPress()
	{
		int mId = mMenuButton.mId;
		ButtonPress(mId);
		ButtonDepress(mId);
		return true;
	}

	public virtual void ButtonDepress(int theId)
	{
		if (mSeedsInFlight > 0 || mChooseState == SeedChooserState.ViewLawn || !mMouseVisible)
		{
			return;
		}
		switch (theId)
		{
		case 102:
			mChooseState = SeedChooserState.ViewLawn;
			mMenuButton.mDisabled = true;
			mViewLawnTime = 0;
			break;
		case 103:
			mApp.DoAlmanacDialog(SeedType.None, ZombieType.Invalid, this);
			break;
		case 107:
			LoadSeedGroup(mApp.mPlayerInfo.mCardGroup[0], 0);
			return;
		case 109:
			if (!LoadSeedGroup(mCardGroupWidget.GetCardGroup(), mCardGroupWidget.mSelectedGroupId))
			{
				mApp.DoDialog(64, isModal: true, "[DIALOG_WARNING]", "[WARNING_SELECT_CARD_GROUP]", "[DIALOG_BUTTON_OK]", 3);
			}
			return;
		case 110:
			if (!SaveToSeedGroup(mCardGroupWidget.GetCardGroup()))
			{
				mApp.DoDialog(64, isModal: true, "[DIALOG_WARNING]", "[WARNING_SELECT_CARD_GROUP]", "[DIALOG_BUTTON_OK]", 3);
			}
			return;
		case 111:
			if (mBoard.CanPlaceRake())
			{
				mApp.PlaySample(Resources.SOUND_TAP);
				mRoseButton.mChecked = !mRoseButton.mChecked;
				PlantVoice.Play(SeedType.None, SeedType.None, mRoseButton.mChecked ? PlantVoice.VoiceType.Special_Zorrose_Enable : PlantVoice.VoiceType.Special_Zorrose_Disable);
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
				mApp.DoDialog(66, isModal: true, "[ASSIST_ZORROSE_HEADER]", "[ASSIST_ZORROSE_NOT_ALLOWED]", "[DIALOG_BUTTON_OK]", 3);
			}
			break;
		case 105:
		{
			StoreScreen storeScreen = mApp.ShowStoreScreen(this);
			storeScreen.mBackButton.mButtonImage = AtlasResources.IMAGE_STORE_CONTINUEBUTTON;
			storeScreen.mBackButton.mDownImage = AtlasResources.IMAGE_STORE_CONTINUEBUTTONDOWN;
			return;
		}
		case 104:
			mMenuButton.mIsOver = false;
			mMenuButton.mIsDown = false;
			mApp.DoNewOptions(theFromGameSelector: false);
			break;
		}
		if (mApp.GetSeedsAvailable() >= mBoard.mSeedBank.mNumPackets)
		{
			switch (theId)
			{
			case 100:
				OnStartButton();
				break;
			case 101:
				PickRandomSeeds();
				break;
			}
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		base.MouseDown(x, y, theClickCount);
		if (mSeedsInFlight > 0)
		{
			foreach (SeedType seedType in ChooserSeedTypes)
			{
				ChosenSeed theChosenSeed = mChosenSeeds[(int)seedType];
				LandFlyingSeed(ref theChosenSeed);
			}
		}
		if (mChooseState == SeedChooserState.ViewLawn)
		{
			CancelLawnView();
			return;
		}
		mStartButton.Update();
		mRandomButton.Update();
		mViewLawnButton.Update();
		mAlmanacButton.Update();
		mLastCardButton.Update();
		mImitaterButton.Update();
		mCardGroupButton.Update();
		mRoseButton.Update();
		mLoadButton.Update();
		mSaveButton.Update();
		mStoreButton.Update();
		mMenuButton.Update();
		if (mRandomButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
			ButtonDepress(101);
			return;
		}
		if (mViewLawnButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
			ButtonDepress(102);
			return;
		}
		if (mMenuButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
		if (mStartButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (mAlmanacButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (mLastCardButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (mStoreButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (mCardGroupButton.IsMouseOver())
		{
			SetInCardGroupScreen(!mInCardGroupScreen);
			return;
		}
		if (mLoadButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (mSaveButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (!mImitaterButton.IsMouseOver())
		{
			foreach (SeedType seedType2 in ChooserSeedTypes)
			{
				ChosenSeed theChosenSeed2 = mApp.mSeedChooserScreen.mChosenSeeds[(int)seedType2];
				int num = (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() : Constants.SMALL_SEEDPACKET_WIDTH);
				int num2 = (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight() : Constants.SMALL_SEEDPACKET_HEIGHT);
				if (theChosenSeed2.mSeedState == ChosenSeedState.SEED_IN_BANK && x >= theChosenSeed2.mX && y >= theChosenSeed2.mY && x < theChosenSeed2.mX + num && y < theChosenSeed2.mY + num2)
				{
					ClickedSeedInBank(ref theChosenSeed2);
					break;
				}
			}
		}
		else if (mSeedsInBank != mBoard.mSeedBank.mNumPackets)
		{
			mApp.PlaySample(Resources.SOUND_TAP);
			ImitaterDialog imitaterDialog = new ImitaterDialog();
			mApp.AddDialog(imitaterDialog.mId, imitaterDialog);
			imitaterDialog.Resize((mWidth - imitaterDialog.mWidth) / 2, (mHeight - imitaterDialog.mHeight) / 2, imitaterDialog.mWidth, imitaterDialog.mHeight);
			mApp.mWidgetManager.SetFocus(imitaterDialog);
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (theClickCount != 1)
		{
			return;
		}
		if (mMenuButton.IsMouseOver())
		{
			ButtonDepress(104);
			return;
		}
		if (mStartButton.IsMouseOver())
		{
			ButtonDepress(100);
			return;
		}
		if (mAlmanacButton.IsMouseOver())
		{
			ButtonDepress(103);
			return;
		}
		if (mLastCardButton.IsMouseOver())
		{
			ButtonDepress(107);
			return;
		}
		if (mStoreButton.IsMouseOver())
		{
			ButtonDepress(105);
			return;
		}
		if (mImitaterButton.IsMouseOver())
		{
			ButtonDepress(106);
		}
		if (mCardGroupButton.IsMouseOver())
		{
			ButtonDepress(108);
		}
		if (mRoseButton.IsMouseOver())
		{
			ButtonDepress(111);
		}
		if (mLoadButton.IsMouseOver())
		{
			ButtonDepress(109);
		}
		if (mSaveButton.IsMouseOver())
		{
			ButtonDepress(110);
		}
	}

	public override void KeyChar(SexyChar theChar)
	{
		if (mChooseState == SeedChooserState.ViewLawn && (theChar.value_type == ' ' || theChar.value_type == '\r' || theChar.value_type == '\u001b'))
		{
			CancelLawnView();
		}
		else if (mApp.mTodCheatKeys && theChar == KeyCode.Escape)
		{
			PickRandomSeeds();
		}
		else
		{
			mBoard.KeyChar(theChar);
		}
	}

	public override void KeyDown(KeyCode theKey)
	{
		mBoard.DoTypingCheck(theKey);
	}

	public void GetSeedPositionInChooser(int theIndex, ref int x, ref int y)
	{
		if (theIndex == 53)
		{
			x = mImitaterButton.mX;
			y = mImitaterButton.mY;
		}
		else if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
			int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			int aCell = SeedPacketsWidget.GetSeedCell((SeedType)theIndex);
			int num = aCell / 8;
			int num2 = aCell % 8;
			x = Constants.Board_Offset_AspectRatio_Correction + 22 + num2 * (celWidth + 3);
			y = 123 + num * celHeight;
		}
		else
		{
			mSeedPacketsWidget.GetSeedPosition((SeedType)theIndex, ref x, ref y);
			x += Constants.SCROLL_AREA_OFFSET_X;
			y += Constants.SCROLL_AREA_OFFSET_Y;
		}
	}

	public void GetSeedPositionInBank(int theIndex, ref int x, ref int y)
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			x = Constants.Board_Offset_AspectRatio_Correction + mBoard.mSeedBank.mX + mBoard.GetSeedPacketPositionX(theIndex) - mX;
			y = mBoard.mSeedBank.mY + 8 - mY;
		}
		else
		{
			y = mBoard.mSeedBank.mY + mBoard.GetSeedPacketPositionY(theIndex) - mY;
			x = mBoard.mSeedBank.mX;
		}
	}

	public void ClickedSeedInChooser(ref ChosenSeed theChosenSeed)
	{
		if (mSeedsInBank == mBoard.mSeedBank.mNumPackets || !mApp.HasSeedType(theChosenSeed.mSeedType))
		{
			return;
		}
		if (mApp.mPlayerInfo.mNeedsGrayedPlantWarning && SeedNotRecommendedToPick(theChosenSeed.mSeedType) != 0)
		{
			mApp.mPlayerInfo.mNeedsGrayedPlantWarning = false;
			mApp.DoDialog(16, isModal: true, "[DIALOG_WARNING]", "[NOT_RECOMMENDED_FOR_LEVEL]", "[DIALOG_BUTTON_OK]", 3);
			return;
		}
		theChosenSeed.mTimeStartMotion = mSeedChooserAge;
		theChosenSeed.mTimeEndMotion = mSeedChooserAge + 25;
		GetSeedPositionInChooser((int)theChosenSeed.mSeedType, ref theChosenSeed.mStartX, ref theChosenSeed.mStartY);
		if (theChosenSeed.mSeedType != SeedType.Imitater)
		{
			theChosenSeed.mStartY += mSeedPacketsWidget.mY;
		}
		GetSeedPositionInBank(mSeedsInBank, ref theChosenSeed.mEndX, ref theChosenSeed.mEndY);
		theChosenSeed.mSeedState = ChosenSeedState.SEED_FLYING_TO_BANK;
		theChosenSeed.mSeedIndexInBank = mSeedsInBank;
		mSeedsInFlight++;
		mSeedsInBank++;
		mApp.PlaySample(Resources.SOUND_TAP);
		if (mSeedsInBank == mBoard.mSeedBank.mNumPackets)
		{
			EnableStartButton(theEnabled: true);
		}
	}

	public void ClickedSeedInBank(ref ChosenSeed theChosenSeed)
	{
		if (theChosenSeed.mSeedState == ChosenSeedState.SEED_IN_BANK && theChosenSeed.mCrazyDavePicked)
		{
			mApp.PlaySample(Resources.SOUND_BUZZER);
			return;
		}
		for (int i = theChosenSeed.mSeedIndexInBank + 1; i < mBoard.mSeedBank.mNumPackets; i++)
		{
			SeedType seedType = FindSeedInBank(i);
			if (seedType != SeedType.None)
			{
				ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
				chosenSeed.mTimeStartMotion = mSeedChooserAge;
				chosenSeed.mTimeEndMotion = mSeedChooserAge + 15;
				chosenSeed.mStartX = chosenSeed.mX;
				chosenSeed.mStartY = chosenSeed.mY;
				GetSeedPositionInBank(i - 1, ref chosenSeed.mEndX, ref chosenSeed.mEndY);
				chosenSeed.mSeedState = ChosenSeedState.SEED_FLYING_TO_BANK;
				chosenSeed.mSeedIndexInBank = i - 1;
				mSeedsInFlight++;
			}
		}
		theChosenSeed.mTimeStartMotion = mSeedChooserAge;
		theChosenSeed.mTimeEndMotion = mSeedChooserAge + 25;
		theChosenSeed.mStartX = theChosenSeed.mX;
		theChosenSeed.mStartY = theChosenSeed.mY;
		GetSeedPositionInChooser((int)theChosenSeed.mSeedType, ref theChosenSeed.mEndX, ref theChosenSeed.mEndY);
		if (theChosenSeed.mSeedType != SeedType.Imitater)
		{
			theChosenSeed.mEndY += mSeedPacketsWidget.mY;
		}
		theChosenSeed.mSeedState = ChosenSeedState.SEED_FLYING_TO_CHOOSER;
		theChosenSeed.mSeedIndexInBank = 0;
		mSeedsInFlight++;
		mSeedsInBank--;
		EnableStartButton(theEnabled: false);
		mApp.PlaySample(Resources.SOUND_TAP);
	}

	public SeedType FindSeedInBank(int theIndexInBank)
	{
		foreach (SeedType seedType in ChooserSeedTypes)
		{
			if (mApp.HasSeedType(seedType))
			{
				ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
				if (chosenSeed.mSeedState == ChosenSeedState.SEED_IN_BANK && chosenSeed.mSeedIndexInBank == theIndexInBank)
				{
					return seedType;
				}
			}
		}
		return SeedType.None;
	}

	public void EnableStartButton(bool theEnabled)
	{
		mStartButton.SetDisabled(!theEnabled);
		if (theEnabled)
		{
			mStartButton.mColors[0] = (mApp.mPlayerInfo.mClassicSeedChooser ? Constants.YellowFontColour : new SexyColor(255, 231, 26));
		}
		else
		{
			mStartButton.mColors[0] = new SexyColor(64, 64, 64);
		}
	}

	public uint SeedNotRecommendedToPick(SeedType theSeedType)
	{
		uint theNumber = mBoard.SeedNotRecommendedForLevel(theSeedType);
		if (TodCommon.TestBit(theNumber, 0) && PickedPlantType(SeedType.InstantCoffee))
		{
			TodCommon.SetBit(ref theNumber, 0, 0);
		}
		return theNumber;
	}

	public bool SeedNotAllowedToPick(SeedType theSeedType)
	{
		bool flag = false;
		if (mApp.mCreativeLevel != null)
		{
			CSSeedBank component = mApp.mCreativeLevel.GetComponent<CSSeedBank>();
			if (component != null && component.mBannedCards != null)
			{
				flag |= component.mBannedCards.Contains(theSeedType) ^ component.mReverseBanning;
			}
		}
		flag |= mApp.mGameMode == GameMode.ChallengeLastStand && (theSeedType == SeedType.Sunflower || theSeedType == SeedType.Sunshroom || theSeedType == SeedType.Twinsunflower || theSeedType == SeedType.Seashroom || theSeedType == SeedType.Puffshroom);
		return flag | (mApp.mGameMode == GameMode.ChallengeFinalBoss2 && theSeedType == SeedType.Hypnoshroom);
	}

	public void CloseSeedChooser()
	{
		Debug.ASSERT(mBoard.mSeedBank.mNumPackets == mBoard.GetNumSeedsInBank());
		SaveToSeedGroup(mApp.mPlayerInfo.mCardGroup[0]);
		for (int i = 0; i < mBoard.mSeedBank.mNumPackets; i++)
		{
			SeedType seedType = FindSeedInBank(i);
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
			SeedPacket seedPacket = mBoard.mSeedBank.mSeedPackets[i];
			seedPacket.SetPacketType(seedType, chosenSeed.mImitaterType);
			if (chosenSeed.mRefreshing)
			{
				seedPacket.mRefreshCounter = mChosenSeeds[(int)seedType].mRefreshCounter;
				seedPacket.mRefreshTime = Plant.GetRefreshTime(seedPacket.mPacketType, seedPacket.mImitaterType);
				seedPacket.mRefreshing = true;
				seedPacket.mActive = false;
			}
		}
		mBoard.mDoPlaceRose = mRoseButton.mChecked;
		mBoard.mCutScene.EndSeedChooser();
	}

	public bool PickedPlantType(SeedType theSeedType)
	{
		foreach (SeedType seedType in ChooserSeedTypes)
		{
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
			if (chosenSeed.mSeedState == ChosenSeedState.SEED_IN_BANK)
			{
				if (chosenSeed.mSeedType == theSeedType)
				{
					return true;
				}
				if (chosenSeed.mSeedType == SeedType.Imitater && chosenSeed.mImitaterType == theSeedType)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void OnStartButton()
	{
		if ((mApp.mGameMode == GameMode.ChallengeSeeingStars && !PickedPlantType(SeedType.Starfruit) && !DisplayRepickWarningDialog(0, "[SEED_CHOOSER_SEEING_STARS_WARNING]")) || (mApp.IsFirstTimeAdventureMode() && mBoard.mLevel == 11 && !PickedPlantType(SeedType.Puffshroom) && !DisplayRepickWarningDialog(1, "[SEED_CHOOSER_PUFFSHROOM_WARNING]")))
		{
			return;
		}
		if (!PickedPlantType(SeedType.Sunflower) && !PickedPlantType(SeedType.Twinsunflower) && !PickedPlantType(SeedType.Sunshroom) && !mBoard.mCutScene.IsSurvivalRepick() && mApp.mGameMode != GameMode.ChallengeLastStand)
		{
			if (mApp.IsFirstTimeAdventureMode() && mBoard.mLevel == 11)
			{
				if (!DisplayRepickWarningDialog(2, "[SEED_CHOOSER_NIGHT_SUN_WARNING]"))
				{
					return;
				}
			}
			else if (!DisplayRepickWarningDialog(3, "[SEED_CHOOSER_SUN_WARNING]"))
			{
				return;
			}
		}
		if (mBoard.StageHasPool() && !PickedPlantType(SeedType.Lilypad) && !PickedPlantType(SeedType.Seashroom) && !PickedPlantType(SeedType.Tanglekelp) && !mBoard.mCutScene.IsSurvivalRepick())
		{
			if (mApp.IsFirstTimeAdventureMode() && mBoard.mLevel == 21)
			{
				if (!DisplayRepickWarningDialog(4, "[SEED_CHOOSER_LILY_WARNING]"))
				{
					return;
				}
			}
			else if (!DisplayRepickWarningDialog(5, "[SEED_CHOOSER_POOL_WARNING]"))
			{
				return;
			}
		}
		if ((!mBoard.StageHasRoof() || PickedPlantType(SeedType.Flowerpot) || !mApp.HasSeedType(SeedType.Flowerpot) || DisplayRepickWarningDialog(6, "[SEED_CHOOSER_ROOF_WARNING]")) && (mApp.mGameMode != GameMode.ChallengeArtChallenge1 || PickedPlantType(SeedType.Wallnut) || DisplayRepickWarningDialog(7, "[SEED_CHOOSER_ART_WALLNUT_WARNING]")) && (mApp.mGameMode != GameMode.ChallengeArtChallenge2 || (PickedPlantType(SeedType.Starfruit) && PickedPlantType(SeedType.Umbrella) && PickedPlantType(SeedType.Wallnut)) || DisplayRepickWarningDialog(8, "[SEED_CHOOSER_ART_2_WARNING]")) && (!FlyersAreComing() || FlyProtectionCurrentlyPlanted() || PickedPlantType(SeedType.Cattail) || PickedPlantType(SeedType.Cactus) || PickedPlantType(SeedType.Blover) || DisplayRepickWarningDialog(9, "[SEED_CHOOSER_FLYER_WARNING]")) && CheckSeedUpgrade(10, SeedType.Gatlingpea, SeedType.Repeater) && CheckSeedUpgrade(11, SeedType.Wintermelon, SeedType.Melonpult) && CheckSeedUpgrade(12, SeedType.Twinsunflower, SeedType.Sunflower) && CheckSeedUpgrade(13, SeedType.Spikerock, SeedType.Spikeweed) && CheckSeedUpgrade(14, SeedType.Cobcannon, SeedType.Kernelpult) && CheckSeedUpgrade(15, SeedType.GoldMagnet, SeedType.Magnetshroom) && CheckSeedUpgrade(16, SeedType.Gloomshroom, SeedType.Fumeshroom) && CheckSeedUpgrade(17, SeedType.Cattail, SeedType.Lilypad) && CheckSeedUpgrade(18, SeedType.SuperChomper, SeedType.Chomper))
		{
			CloseSeedChooser();
		}
	}

	public bool DisplayRepickWarningDialog(int theWarningId, string theMessage)
	{
		if (mPickWarningsWaved[theWarningId])
		{
			return true;
		}
		mPendingWarningId = theWarningId;
		mApp.LawnMessageBox(28, "[DIALOG_WARNING]", theMessage, "[DIALOG_BUTTON_YES]", "[REPICK_BUTTON]", 1, this);
		return false;
	}

	public void UpdateViewLawn()
	{
		if (mChooseState == SeedChooserState.ViewLawn)
		{
			mViewLawnTime++;
			if (mViewLawnTime == 100)
			{
				mBoard.DisplayAdviceAgain("[CLICK_TO_CONTINUE]", MessageStyle.HintStay, AdviceType.ClickToContinue);
			}
			else if (mViewLawnTime == 251)
			{
				mViewLawnTime = 250;
			}
			int num = 800;
			if (mViewLawnTime <= 100)
			{
				int thePositionStart = -Constants.BOARD_OFFSET + Constants.ImageWidth - num;
				int thePositionEnd = 0;
				int num2 = TodCommon.TodAnimateCurve(0, 100, mViewLawnTime, thePositionStart, thePositionEnd, TodCurves.EaseInOut);
				mBoard.Move((int)((float)(-num2) * Constants.S), 0);
				int thePositionStart2 = Constants.SEED_CHOOSER_OFFSETSCREEN_OFFSET - 265;
				int sEED_CHOOSER_OFFSETSCREEN_OFFSET = Constants.SEED_CHOOSER_OFFSETSCREEN_OFFSET;
				int num3 = TodCommon.TodAnimateCurve(0, 40, mViewLawnTime, thePositionStart2, sEED_CHOOSER_OFFSETSCREEN_OFFSET, TodCurves.EaseInOut);
				Move(0, (int)((float)num3 * Constants.S));
			}
			else if (mViewLawnTime > 100 && mViewLawnTime <= 250)
			{
				mBoard.Move(0, 0);
				Move(0, Constants.SEED_CHOOSER_OFFSETSCREEN_OFFSET);
			}
			else if (mViewLawnTime > 250 && mViewLawnTime <= 350)
			{
				mBoard.ClearAdvice(AdviceType.ClickToContinue);
				int thePositionStart3 = 0;
				int thePositionEnd2 = -Constants.BOARD_OFFSET + Constants.ImageWidth - num;
				int num4 = TodCommon.TodAnimateCurve(250, 350, mViewLawnTime, thePositionStart3, thePositionEnd2, TodCurves.EaseInOut);
				mBoard.Move((int)((float)(-num4) * Constants.S), 0);
				int sEED_CHOOSER_OFFSETSCREEN_OFFSET2 = Constants.SEED_CHOOSER_OFFSETSCREEN_OFFSET;
				int thePositionEnd3 = Constants.SEED_CHOOSER_OFFSETSCREEN_OFFSET - 265;
				int num5 = TodCommon.TodAnimateCurve(310, 350, mViewLawnTime, sEED_CHOOSER_OFFSETSCREEN_OFFSET2, thePositionEnd3, TodCurves.EaseInOut);
				Move(0, (int)((float)num5 * Constants.S));
			}
			else
			{
				mChooseState = SeedChooserState.Normal;
				mViewLawnTime = 0;
				mMenuButton.mDisabled = false;
			}
		}
	}

	public void CancelLawnView()
	{
		if (mChooseState == SeedChooserState.ViewLawn && mViewLawnTime > 100 && mViewLawnTime <= 250)
		{
			mViewLawnTime = 251;
		}
	}

	public void LandFlyingSeed(ref ChosenSeed theChosenSeed)
	{
		if (theChosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_BANK)
		{
			theChosenSeed.mTimeStartMotion = 0;
			theChosenSeed.mTimeEndMotion = 0;
			theChosenSeed.mSeedState = ChosenSeedState.SEED_IN_BANK;
			theChosenSeed.mX = theChosenSeed.mEndX;
			theChosenSeed.mY = theChosenSeed.mEndY;
			mSeedsInFlight--;
		}
		else if (theChosenSeed.mSeedState == ChosenSeedState.SEED_FLYING_TO_CHOOSER)
		{
			theChosenSeed.mTimeStartMotion = 0;
			theChosenSeed.mTimeEndMotion = 0;
			theChosenSeed.mSeedState = ChosenSeedState.SEED_IN_CHOOSER;
			theChosenSeed.mX = theChosenSeed.mEndX;
			theChosenSeed.mY = theChosenSeed.mEndY;
			mSeedsInFlight--;
			if (theChosenSeed.mSeedType == SeedType.Imitater)
			{
				theChosenSeed.mSeedState = ChosenSeedState.SEED_PACKET_HIDDEN;
				theChosenSeed.mImitaterType = SeedType.None;
				UpdateImitaterButton();
			}
		}
	}

	public bool FlyProtectionCurrentlyPlanted()
	{
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && (plant.mSeedType == SeedType.Cattail || plant.mSeedType == SeedType.HypnoCattail || plant.mSeedType == SeedType.Cactus))
			{
				return true;
			}
		}
		return false;
	}

	public bool FlyersAreComing()
	{
		for (int i = 0; i < mBoard.mNumWaves; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				switch (mBoard.mZombiesInWave[i, j])
				{
				case ZombieType.Balloon:
				case ZombieType.Propeller:
					return true;
				default:
					continue;
				case ZombieType.Invalid:
					break;
				}
				break;
			}
		}
		return false;
	}

	public void UpdateAfterPurchase()
	{
		foreach (SeedType seedType in ChooserSeedTypes)
		{
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
			if (chosenSeed.mSeedState == ChosenSeedState.SEED_IN_BANK)
			{
				GetSeedPositionInBank(chosenSeed.mSeedIndexInBank, ref chosenSeed.mX, ref chosenSeed.mY);
				chosenSeed.mStartX = chosenSeed.mX;
				chosenSeed.mStartY = chosenSeed.mY;
				chosenSeed.mEndX = chosenSeed.mX;
				chosenSeed.mEndY = chosenSeed.mY;
			}
			else if (chosenSeed.mSeedState == ChosenSeedState.SEED_IN_CHOOSER)
			{
				GetSeedPositionInChooser((int)seedType, ref chosenSeed.mX, ref chosenSeed.mY);
				chosenSeed.mStartX = chosenSeed.mX;
				chosenSeed.mStartY = chosenSeed.mY;
				chosenSeed.mEndX = chosenSeed.mX;
				chosenSeed.mEndY = chosenSeed.mY;
			}
		}
		if (mSeedsInBank == mBoard.mSeedBank.mNumPackets)
		{
			EnableStartButton(theEnabled: true);
		}
		else
		{
			EnableStartButton(theEnabled: false);
		}
		UpdateImitaterButton();
	}

	public void UpdateImitaterButton()
	{
		if (!mApp.HasSeedType(SeedType.Imitater))
		{
			mImitaterButton.mBtnNoDraw = true;
			mImitaterButton.mDisabled = true;
			return;
		}
		mImitaterButton.mBtnNoDraw = false;
		bool disabled = false;
		ChosenSeed chosenSeed = mChosenSeeds[53];
		if (chosenSeed.mSeedState != ChosenSeedState.SEED_PACKET_HIDDEN)
		{
			disabled = true;
		}
		mImitaterButton.SetDisabled(disabled);
	}

	public void CrazyDavePickSeeds()
	{
		int num = 1;
		if (mApp.mGameMode == GameMode.ChallengeStageRandom)
		{
			num *= 100;
		}
		for (int i = 0; i < aSeedArray.Length; i++)
		{
			aSeedArray[i].Reset();
		}
		foreach (SeedType seedType in ChooserSeedTypes)
		{
			if ((int)seedType >= aSeedArray.Length)
			{
				continue; // Crazy Dave does not pick plant-mod plants
			}
			SeedType seedType2 = seedType;
			aSeedArray[(int)seedType].mItem = (int)seedType2;
			uint num2 = SeedNotRecommendedToPick(seedType2);
			if (!mApp.HasSeedType(seedType2) || num2 != 0 || SeedNotAllowedToPick(seedType2) || Plant.IsUpgrade(seedType2) || seedType2 == SeedType.Imitater || seedType2 == SeedType.Umbrella || seedType2 == SeedType.Blover)
			{
				aSeedArray[(int)seedType].mWeight = 0;
			}
			else
			{
				aSeedArray[(int)seedType].mWeight = num;
			}
		}
		if (mBoard.mZombieAllowed[22] || mBoard.mZombieAllowed[20])
		{
			aSeedArray[37].mWeight = num;
		}
		if (mBoard.mZombieAllowed[16] || mBoard.StageHasFog())
		{
			aSeedArray[27].mWeight = num;
		}
		if (mBoard.StageHasRoof())
		{
			aSeedArray[22].mWeight = 0;
		}
		if (mApp.mGameMode == GameMode.ChallengeStageRandom)
		{
			aSeedArray[38].mWeight = 0;
			aSeedArray[1].mWeight = 0;
			aSeedArray[9].mWeight = 0;
			aSeedArray[8].mWeight = 0;
			aSeedArray[33].mWeight = 0;
			aSeedArray[16].mWeight = 0;
			if (mBoard.StageHasRoof())
			{
				aSeedArray[0].mWeight = 30;
				aSeedArray[5].mWeight = 30;
				aSeedArray[7].mWeight = 30;
				aSeedArray[18].mWeight = 30;
				aSeedArray[28].mWeight = 5;
			}
		}
		int levelRandSeed = mBoard.GetLevelRandSeed();
		RandomNumbers.Seed(levelRandSeed);
		int num3 = ((mApp.mGameMode == GameMode.ChallengeStageRandom) ? 8 : (mApp.IsAdventureModeHard() ? 1 : 3));
		for (int j = 0; j < num3; j++)
		{
			SeedType seedType3 = (SeedType)PickFromWeightedArrayUsingSpecialRandSeed(aSeedArray, 54);
			if (mApp.mGameMode == GameMode.ChallengeStageRandom)
			{
				SeedType[] array = new SeedType[1] { SeedType.Sunflower };
				if (mBoard.mBackground == BackgroundType.Num1Day)
				{
					array = new SeedType[1] { SeedType.Sunflower };
				}
				if (mBoard.mBackground == BackgroundType.Num2Night)
				{
					array = new SeedType[3]
					{
						SeedType.Puffshroom,
						SeedType.Sunshroom,
						SeedType.Gravebuster
					};
				}
				if (mBoard.mBackground == BackgroundType.Num3Pool)
				{
					array = new SeedType[2]
					{
						SeedType.Sunflower,
						SeedType.Lilypad
					};
				}
				if (mBoard.mBackground == BackgroundType.Num4Fog)
				{
					array = new SeedType[4]
					{
						SeedType.Puffshroom,
						SeedType.Sunshroom,
						SeedType.Plantern,
						SeedType.Lilypad
					};
				}
				if (mBoard.mBackground == BackgroundType.Num5Roof)
				{
					array = new SeedType[2]
					{
						SeedType.Sunflower,
						SeedType.Flowerpot
					};
				}
				if (mBoard.mBackground == BackgroundType.BigPool)
				{
					array = new SeedType[2]
					{
						SeedType.Sunflower,
						SeedType.Lilypad
					};
				}
				if (mBoard.mBackground == BackgroundType.HighGround)
				{
					array = new SeedType[2]
					{
						SeedType.Puffshroom,
						SeedType.Sunshroom
					};
				}
				if (array.Length > j)
				{
					seedType3 = array[j];
				}
			}
			if (mApp.mGameMode == GameMode.ChallengeStageRandom)
			{
				if (seedType3 == SeedType.Sunflower && mApp.HasSeedType(SeedType.Twinsunflower))
				{
					aSeedArray[41].mWeight = num;
				}
				if (seedType3 == SeedType.Repeater && mApp.HasSeedType(SeedType.Gatlingpea))
				{
					aSeedArray[40].mWeight = num;
				}
				if (seedType3 == SeedType.Fumeshroom && mApp.HasSeedType(SeedType.Gloomshroom))
				{
					aSeedArray[42].mWeight = num;
				}
				if (seedType3 == SeedType.Lilypad && mApp.HasSeedType(SeedType.Cattail))
				{
					aSeedArray[43].mWeight = num;
				}
				if (seedType3 == SeedType.Melonpult && mApp.HasSeedType(SeedType.Wintermelon))
				{
					aSeedArray[44].mWeight = num;
				}
				if (seedType3 == SeedType.Spikeweed && mApp.HasSeedType(SeedType.Spikerock))
				{
					aSeedArray[46].mWeight = num;
				}
				if (seedType3 == SeedType.Kernelpult && mApp.HasSeedType(SeedType.Cobcannon))
				{
					aSeedArray[47].mWeight = num;
				}
				if (seedType3 == SeedType.Chomper && mApp.HasSeedType(SeedType.SuperChomper))
				{
					aSeedArray[48].mWeight = num;
				}
				if (mBoard.StageHasRoof() && (seedType3 == SeedType.Peashooter || seedType3 == SeedType.Repeater || seedType3 == SeedType.Threepeater || seedType3 == SeedType.Splitpea || seedType3 == SeedType.Snowpea))
				{
					aSeedArray[0].mWeight /= 2;
					aSeedArray[5].mWeight /= 2;
					aSeedArray[7].mWeight /= 2;
					aSeedArray[18].mWeight /= 2;
					aSeedArray[28].mWeight /= 2;
				}
				if (seedType3 == SeedType.Peashooter || seedType3 == SeedType.Repeater || seedType3 == SeedType.Threepeater || seedType3 == SeedType.Splitpea || seedType3 == SeedType.Gatlingpea)
				{
					bool flag = false;
					for (int k = 0; k < 10; k++)
					{
						if (FindSeedInBank(k) == SeedType.Torchwood)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						aSeedArray[22].mWeight = num;
					}
				}
			}
			aSeedArray[(int)seedType3].mWeight = 0;
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType3];
			GetSeedPositionInBank(j, ref chosenSeed.mX, ref chosenSeed.mY);
			chosenSeed.mEndX = chosenSeed.mX;
			chosenSeed.mEndY = chosenSeed.mY;
			chosenSeed.mStartX = chosenSeed.mX;
			chosenSeed.mStartY = chosenSeed.mY;
			chosenSeed.mSeedState = ChosenSeedState.SEED_IN_BANK;
			chosenSeed.mSeedIndexInBank = j;
			chosenSeed.mCrazyDavePicked = true;
			mSeedsInBank++;
		}
	}

	public int PickFromWeightedArrayUsingSpecialRandSeed(TodWeightedArray[] theArray, int theCount)
	{
		int num = 0;
		for (int i = 0; i < theCount; i++)
		{
			num += theArray[i].mWeight;
		}
		Debug.ASSERT(num > 0);
		int num2 = RandomNumbers.NextNumber(num);
		int num3 = 0;
		for (int j = 0; j < theCount; j++)
		{
			num3 += theArray[j].mWeight;
			if (num2 < num3)
			{
				return (int)theArray[j].mItem;
			}
		}
		Debug.ASSERT(value: false);
		return -666;
	}

	public bool CheckSeedUpgrade(int theWarningId, SeedType theSeedTypeTo, SeedType theSeedTypeFrom)
	{
		if (mApp.IsSurvivalMode() || mPickWarningsWaved[theWarningId])
		{
			return true;
		}
		if (PickedPlantType(theSeedTypeTo) && !PickedPlantType(theSeedTypeFrom))
		{
			string theText = TodStringFile.TodStringTranslate("[SEED_CHOOSER_UPGRADE_WARNING]");
			string nameString = Plant.GetNameString(theSeedTypeTo, SeedType.None);
			string nameString2 = Plant.GetNameString(theSeedTypeFrom, SeedType.None);
			theText = TodCommon.TodReplaceString(theText, "{UPGRADE_TO}", nameString);
			theText = TodCommon.TodReplaceString(theText, "{UPGRADE_FROM}", nameString2);
			if (!DisplayRepickWarningDialog(theWarningId, theText))
			{
				return false;
			}
		}
		return true;
	}

	public void PickRandomSeeds()
	{
		for (int i = mSeedsInBank; i < mBoard.mSeedBank.mNumPackets; i++)
		{
			int num;
			SeedType seedType;
			do
			{
				num = RandomNumbers.NextNumber(mApp.GetSeedsAvailable());
				seedType = (SeedType)num;
			}
			while (!mApp.HasSeedType(seedType) || seedType == SeedType.Imitater || mChosenSeeds[num].mSeedState != ChosenSeedState.SEED_IN_CHOOSER);
			ChosenSeed chosenSeed = mChosenSeeds[num];
			chosenSeed.mTimeStartMotion = 0;
			chosenSeed.mTimeEndMotion = 0;
			chosenSeed.mStartX = chosenSeed.mX;
			chosenSeed.mStartY = chosenSeed.mY;
			GetSeedPositionInBank(i, ref chosenSeed.mEndX, ref chosenSeed.mEndY);
			chosenSeed.mSeedState = ChosenSeedState.SEED_IN_BANK;
			chosenSeed.mSeedIndexInBank = i;
			mSeedsInBank++;
		}
		foreach (SeedType seedType2 in ChooserSeedTypes)
		{
			ChosenSeed theChosenSeed = mChosenSeeds[(int)seedType2];
			LandFlyingSeed(ref theChosenSeed);
		}
		CloseSeedChooser();
	}

	public bool Has12Rows()
	{
		if (!mApp.HasFinishedAdventure())
		{
			if (!mApp.HasSeedType(SeedType.Gatlingpea) && !mApp.HasSeedType(SeedType.Twinsunflower) && !mApp.HasSeedType(SeedType.Gloomshroom) && !mApp.HasSeedType(SeedType.Cattail) && !mApp.HasSeedType(SeedType.Wintermelon) && !mApp.HasSeedType(SeedType.Gatlingpea) && !mApp.HasSeedType(SeedType.GoldMagnet))
			{
				return mApp.HasSeedType(SeedType.Cobcannon);
			}
			return true;
		}
		return true;
	}

	public bool SeedNotAllowedDuringTrial(SeedType theSeedType)
	{
		if (mApp.IsTrialStageLocked())
		{
			if (theSeedType != SeedType.Squash)
			{
				return theSeedType == SeedType.Threepeater;
			}
			return true;
		}
		return false;
	}

	public void BackFromStore()
	{
		StoreScreen storeScreen = (StoreScreen)mApp.GetDialog(Dialogs.DIALOG_STORE);
		mApp.KillDialog(4);
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ChooseYourSeeds);
		int num = mApp.mPlayerInfo.mPurchases[36];
		if (num > 0)
		{
			mCardGroupButton.mBtnNoDraw = false;
			mCardGroupButton.mDisabled = false;
			mCardGroupWidget.mCols = num;
		}
		if (mApp.mPlayerInfo.mPurchases[24] > 0)
		{
			mRoseButton.mBtnNoDraw = false;
			mRoseButton.mDisabled = false;
		}
	}

	public void BackFromAlmanac()
	{
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ChooseYourSeeds);
		mApp.mBoard.LoadBackgroundImages();
	}

	public void LawnMessageBoxDone(int theResult)
	{
		if (theResult == 1000)
		{
			mPickWarningsWaved[mPendingWarningId] = true;
			mDoStartButton = true;
		}
	}

	public void SeedSelected(SeedType theSeedType)
	{
		if (theSeedType == SeedType.None)
		{
			return;
		}
		if (SeedNotAllowedToPick(theSeedType))
		{
			mApp.DoDialog(16, isModal: true, string.Empty, "[NOT_ALLOWED_ON_THIS_LEVEL]", "[DIALOG_BUTTON_OK]", 3);
		}
		else if (theSeedType >= SeedType.Peashooter && (int)theSeedType < mChosenSeeds.Length && (int)theSeedType <= mChosenSeeds.Length)
		{
			ChosenSeed theChosenSeed = mChosenSeeds[(int)theSeedType];
			if (theChosenSeed != null && theChosenSeed.mSeedState == ChosenSeedState.SEED_IN_CHOOSER)
			{
				ClickedSeedInChooser(ref theChosenSeed);
			}
		}
	}

	public void CleanSeed(bool immediate)
	{
		for (int i = 0; i < mChosenSeeds.Length; i++)
		{
			SeedType seedType = (SeedType)i;
			if (seedType == SeedType.None)
			{
				continue;
			}
			ChosenSeed theChosenSeed = mChosenSeeds[i];
			if (theChosenSeed != null && theChosenSeed.mSeedState != ChosenSeedState.SEED_PACKET_HIDDEN && theChosenSeed.mSeedState != ChosenSeedState.SEED_IN_CHOOSER && (theChosenSeed.mSeedState != ChosenSeedState.SEED_IN_BANK || !theChosenSeed.mCrazyDavePicked) && (immediate || theChosenSeed.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER))
			{
				theChosenSeed.mTimeStartMotion = mSeedChooserAge;
				theChosenSeed.mTimeEndMotion = mSeedChooserAge + 25;
				theChosenSeed.mStartX = theChosenSeed.mX;
				theChosenSeed.mStartY = theChosenSeed.mY;
				GetSeedPositionInChooser((int)theChosenSeed.mSeedType, ref theChosenSeed.mEndX, ref theChosenSeed.mEndY);
				if (theChosenSeed.mSeedType != SeedType.Imitater)
				{
					theChosenSeed.mEndY += mSeedPacketsWidget.mY;
				}
				if (immediate)
				{
					theChosenSeed.mX = theChosenSeed.mEndX;
					theChosenSeed.mY = theChosenSeed.mEndY;
					theChosenSeed.mTimeStartMotion -= 25;
					theChosenSeed.mTimeEndMotion -= 25;
				}
				if (theChosenSeed.mSeedState == ChosenSeedState.SEED_IN_BANK)
				{
					mSeedsInFlight++;
				}
				if (theChosenSeed.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER)
				{
					mSeedsInBank--;
				}
				theChosenSeed.mSeedState = ChosenSeedState.SEED_FLYING_TO_CHOOSER;
				theChosenSeed.mSeedIndexInBank = 0;
				if (immediate)
				{
					LandFlyingSeed(ref theChosenSeed);
				}
			}
		}
		EnableStartButton(theEnabled: false);
		UpdateImitaterButton();
	}

	public bool SaveToSeedGroup(CardGroup seedGroup)
	{
		if (seedGroup == null)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < mBoard.mSeedBank.mNumPackets && i < 10; i++)
		{
			SeedType seedType = FindSeedInBank(i);
			if (!IsChooserSeed(seedType))
			{
				continue;
			}
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType];
			if (!chosenSeed.mCrazyDavePicked)
			{
				SeedType mImitaterType = chosenSeed.mImitaterType;
				if (mImitaterType >= SeedType.None && mImitaterType < SeedType.ExplodeONut)
				{
					seedGroup.mCardInfo[num++].Init(chosenSeed.mSeedType, chosenSeed.mImitaterType);
				}
			}
		}
		for (int j = num; j < 10; j++)
		{
			seedGroup.mCardInfo[j].Init();
		}
		mApp.WriteCurrentUserConfig();
		return true;
	}

	public bool LoadSeedGroup(CardGroup seedGroup, int theIndex = -1)
	{
		if (seedGroup == null)
		{
			return false;
		}
		CleanSeed(theIndex < 0);
		for (int i = 0; i < 10 && mSeedsInBank != mBoard.mSeedBank.mNumPackets; i++)
		{
			var (seedType3, seedType4) = seedGroup.mCardInfo[i];
			if (!IsChooserSeed(seedType3))
			{
				continue;
			}
			if (seedType3 == SeedType.Imitater && seedType4 != SeedType.None)
			{
				if (seedType4 <= SeedType.None || seedType4 >= SeedType.ExplodeONut)
				{
					continue;
				}
			}
			else
			{
				seedType4 = SeedType.None;
			}
			if (SeedNotAllowedToPick(seedType3) || !mApp.HasSeedType(seedType3) || (seedType3 == SeedType.Imitater && seedType4 != SeedType.None && (SeedNotAllowedToPick(seedType4) || !mApp.HasSeedType(seedType4))))
			{
				continue;
			}
			ChosenSeed chosenSeed = mChosenSeeds[(int)seedType3];
			if (chosenSeed.mSeedState == ChosenSeedState.SEED_IN_BANK)
			{
				continue;
			}
			if (theIndex >= 0)
			{
				chosenSeed.mTimeStartMotion = mSeedChooserAge;
				chosenSeed.mTimeEndMotion = mSeedChooserAge + 25;
				if (chosenSeed.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER)
				{
					if (theIndex == 0)
					{
						GetSeedPositionInChooser((int)chosenSeed.mSeedType, ref chosenSeed.mX, ref chosenSeed.mY);
					}
					else
					{
						mCardGroupWidget.GetCardGroupPosition(theIndex, out chosenSeed.mX);
						chosenSeed.mY = mCardGroupWidget.GetCardPositionInGroup(i, seedGroup.mCardInfo.Count((CardInfo item) => item.mSeedType != SeedType.None));
						chosenSeed.mX += mCardGroupWidget.mX + mScrollWidget2.mX;
						chosenSeed.mY += mCardGroupWidget.mY + mScrollWidget2.mY;
					}
					if (chosenSeed.mSeedType != SeedType.Imitater)
					{
						chosenSeed.mY += mSeedPacketsWidget.mY;
					}
				}
				chosenSeed.mStartX = chosenSeed.mX;
				chosenSeed.mStartY = chosenSeed.mY;
				GetSeedPositionInBank(mSeedsInBank, ref chosenSeed.mEndX, ref chosenSeed.mEndY);
				if (chosenSeed.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER)
				{
					mSeedsInFlight++;
				}
				chosenSeed.mSeedState = ChosenSeedState.SEED_FLYING_TO_BANK;
			}
			else
			{
				GetSeedPositionInBank(mSeedsInBank, ref chosenSeed.mX, ref chosenSeed.mY);
				chosenSeed.mEndX = chosenSeed.mX;
				chosenSeed.mEndY = chosenSeed.mY;
				chosenSeed.mStartX = chosenSeed.mX;
				chosenSeed.mStartY = chosenSeed.mY;
				chosenSeed.mSeedState = ChosenSeedState.SEED_IN_BANK;
			}
			chosenSeed.mImitaterType = seedType4;
			chosenSeed.mSeedIndexInBank = mSeedsInBank;
			chosenSeed.mCrazyDavePicked = false;
			mSeedsInBank++;
		}
		EnableStartButton(mSeedsInBank == mBoard.mSeedBank.mNumPackets);
		UpdateImitaterButton();
		return true;
	}

	public void SetInCardGroupScreen(bool isIn)
	{
		mInCardGroupScreen = isIn;
		mScrollWidget.SetDisabled(isIn);
		mScrollWidget.mVisible = !isIn;
		mScrollWidget2.SetDisabled(!isIn);
		mScrollWidget2.mVisible = isIn;
		mLoadButton.mBtnNoDraw = !isIn;
		mLoadButton.mDisabled = !isIn;
		mSaveButton.mBtnNoDraw = !isIn;
		mSaveButton.mDisabled = !isIn;
		mAlmanacButton.mBtnNoDraw = isIn;
		mAlmanacButton.mDisabled = isIn;
		mStoreButton.mBtnNoDraw = isIn;
		mStoreButton.mDisabled = isIn;
		if (!isIn)
		{
			if (!mApp.CanShowAlmanac())
			{
				mAlmanacButton.mBtnNoDraw = true;
				mAlmanacButton.mDisabled = true;
			}
			if (!mApp.CanShowStore())
			{
				mStoreButton.mBtnNoDraw = true;
				mStoreButton.mDisabled = true;
			}
		}
	}
}
