using System;
using System.Threading.Tasks;
using Lawn.Creative;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ChallengeScreen : Widget, ButtonListener
{
	private const int NUM_TROPHIES_SURVIVAL = 10;

	private const int NUM_TROPHIES_CHALLENGE = 20;

	private const int NUM_TROPHIES_PUZZLE = 18;

	public static ChallengeDefinition[] gChallengeDefs = new ChallengeDefinition[102]
	{
		new ChallengeDefinition(GameMode.SurvivalNormalStage1, 0, ChallengePage.Survival, 0, 0, "[SURVIVAL_DAY_NORMAL]"),
		new ChallengeDefinition(GameMode.SurvivalNormalStage2, 1, ChallengePage.Survival, 0, 1, "[SURVIVAL_NIGHT_NORMAL]"),
		new ChallengeDefinition(GameMode.SurvivalNormalStage3, 2, ChallengePage.Survival, 0, 2, "[SURVIVAL_POOL_NORMAL]"),
		new ChallengeDefinition(GameMode.SurvivalNormalStage4, 3, ChallengePage.Survival, 0, 3, "[SURVIVAL_FOG_NORMAL]"),
		new ChallengeDefinition(GameMode.SurvivalNormalStage5, 4, ChallengePage.Survival, 0, 4, "[SURVIVAL_ROOF_NORMAL]"),
		new ChallengeDefinition(GameMode.SurvivalHardStage1, 5, ChallengePage.Survival, 1, 0, "[SURVIVAL_DAY_HARD]"),
		new ChallengeDefinition(GameMode.SurvivalHardStage2, 6, ChallengePage.Survival, 1, 1, "[SURVIVAL_NIGHT_HARD]"),
		new ChallengeDefinition(GameMode.SurvivalHardStage3, 7, ChallengePage.Survival, 1, 2, "[SURVIVAL_POOL_HARD]"),
		new ChallengeDefinition(GameMode.SurvivalHardStage4, 8, ChallengePage.Survival, 1, 3, "[SURVIVAL_FOG_HARD]"),
		new ChallengeDefinition(GameMode.SurvivalHardStage5, 9, ChallengePage.Survival, 1, 4, "[SURVIVAL_ROOF_HARD]"),
		new ChallengeDefinition(GameMode.SurvivalEndlessStage1, 10, ChallengePage.Survival, 3, 0, "[SURVIVAL_DAY_ENDLESS]"),
		new ChallengeDefinition(GameMode.SurvivalEndlessStage2, 10, ChallengePage.Survival, 3, 1, "[SURVIVAL_NIGHT_ENDLESS]"),
		new ChallengeDefinition(GameMode.SurvivalEndlessStage3, 10, ChallengePage.Survival, 3, 2, "[SURVIVAL_POOL_ENDLESS]"),
		new ChallengeDefinition(GameMode.SurvivalEndlessStage4, 10, ChallengePage.Survival, 3, 3, "[SURVIVAL_FOG_ENDLESS]"),
		new ChallengeDefinition(GameMode.SurvivalEndlessStage5, 10, ChallengePage.Survival, 3, 4, "[SURVIVAL_ROOF_ENDLESS]"),
		new ChallengeDefinition(GameMode.ChallengeWarAndPeas, 0, ChallengePage.Challenge, 0, 0, "[WAR_AND_PEAS]"),
		new ChallengeDefinition(GameMode.ChallengeWallnutBowling, 6, ChallengePage.Challenge, 0, 1, "[WALL_NUT_BOWLING]"),
		new ChallengeDefinition(GameMode.ChallengeSlotMachine, 2, ChallengePage.Challenge, 0, 2, "[SLOT_MACHINE]"),
		new ChallengeDefinition(GameMode.ChallengeRainingSeeds, 3, ChallengePage.Challenge, 0, 3, "[ITS_RAINING_SEEDS]"),
		new ChallengeDefinition(GameMode.ChallengeBeghouled, 1, ChallengePage.Challenge, 0, 4, "[BEGHOULED]"),
		new ChallengeDefinition(GameMode.ChallengeInvisighoul, 8, ChallengePage.Challenge, 1, 0, "[INVISIGHOUL]"),
		new ChallengeDefinition(GameMode.ChallengeSeeingStars, 5, ChallengePage.Challenge, 1, 1, "[SEEING_STARS]"),
		new ChallengeDefinition(GameMode.ChallengeBeghouledTwist, 20, ChallengePage.Challenge, 1, 3, "[BEGHOULED_TWIST]"),
		new ChallengeDefinition(GameMode.ChallengeLittleTrouble, 12, ChallengePage.Challenge, 1, 4, "[LITTLE_TROUBLE]"),
		new ChallengeDefinition(GameMode.ChallengePortalCombat, 15, ChallengePage.Challenge, 2, 0, "[PORTAL_COMBAT]"),
		new ChallengeDefinition(GameMode.ChallengeColumn, 4, ChallengePage.Challenge, 2, 1, "[COLUMN_AS_YOU_SEE_EM]"),
		new ChallengeDefinition(GameMode.ChallengeBobsledBonanza, 17, ChallengePage.Challenge, 2, 2, "[BOBSLED_BONANZA]"),
		new ChallengeDefinition(GameMode.ChallengeSpeed, 18, ChallengePage.Challenge, 2, 3, "[ZOMBIES_ON_SPEED]"),
		new ChallengeDefinition(GameMode.ChallengeWhackAZombie, 16, ChallengePage.Challenge, 2, 4, "[WHACK_A_ZOMBIE]"),
		new ChallengeDefinition(GameMode.ChallengeLastStand, 21, ChallengePage.Challenge, 3, 0, "[LAST_STAND]"),
		new ChallengeDefinition(GameMode.ChallengeWarAndPeas2, 0, ChallengePage.Challenge, 3, 1, "[WAR_AND_PEAS_2]"),
		new ChallengeDefinition(GameMode.ChallengeWallnutBowling2, 6, ChallengePage.Challenge, 3, 2, "[WALL_NUT_BOWLING_EXTREME]"),
		new ChallengeDefinition(GameMode.ChallengePogoParty, 14, ChallengePage.Challenge, 3, 3, "[POGO_PARTY]"),
		new ChallengeDefinition(GameMode.ChallengeFinalBoss, 19, ChallengePage.Challenge, 3, 4, "[FINAL_BOSS]"),
		new ChallengeDefinition(GameMode.ChallengeArtChallenge1, 0, ChallengePage.Challenge2, 0, 0, "[ART_CHALLENGE_WALL_NUT]"),
		new ChallengeDefinition(GameMode.ChallengeSunnyDay, 1, ChallengePage.Challenge2, 0, 1, "[SUNNY_DAY]"),
		new ChallengeDefinition(GameMode.ChallengeResodded, 2, ChallengePage.Challenge2, 0, 2, "[UNSODDED]"),
		new ChallengeDefinition(GameMode.ChallengeBigTime, 3, ChallengePage.Challenge2, 0, 3, "[BIG_TIME]"),
		new ChallengeDefinition(GameMode.ChallengeArtChallenge2, 4, ChallengePage.Challenge2, 0, 4, "[ART_CHALLENGE_SUNFLOWER]"),
		new ChallengeDefinition(GameMode.ChallengeAirRaid, 5, ChallengePage.Challenge2, 1, 0, "[AIR_RAID]"),
		new ChallengeDefinition(GameMode.ChallengeIce, 6, ChallengePage.Limbo, 1, 1, "[ICE_LEVEL]"),
		new ChallengeDefinition(GameMode.ChallengeZenGarden, 7, ChallengePage.Limbo, 1, 2, "[ZEN_GARDEN]"),
		new ChallengeDefinition(GameMode.ChallengeHighGravity, 8, ChallengePage.Challenge2, 1, 1, "[HIGH_GRAVITY]"),
		new ChallengeDefinition(GameMode.ChallengeGraveDanger, 11, ChallengePage.Challenge2, 1, 2, "[GRAVE_DANGER]"),
		new ChallengeDefinition(GameMode.ChallengeShovel, 10, ChallengePage.Challenge2, 1, 3, "[CAN_YOU_DIG_IT]"),
		new ChallengeDefinition(GameMode.ChallengeStormyNight, 13, ChallengePage.Challenge2, 1, 4, "[DARK_STORMY_NIGHT]"),
		new ChallengeDefinition(GameMode.ChallengeBungeeBlitz, 9, ChallengePage.Challenge2, 2, 0, "[BUNGEE_BLITZ]"),
		new ChallengeDefinition(GameMode.ChallengeSquirrel, 10, ChallengePage.Limbo, 2, 3, "Squirrel"),
		new ChallengeDefinition(GameMode.TreeOfWisdom, 10, ChallengePage.Limbo, 2, 4, "Tree of Wisdom"),
		new ChallengeDefinition(GameMode.ScaryPotter1, 10, ChallengePage.Puzzle, 0, 0, "[SCARY_POTTER_1]"),
		new ChallengeDefinition(GameMode.ScaryPotter2, 10, ChallengePage.Puzzle, 0, 1, "[SCARY_POTTER_2]"),
		new ChallengeDefinition(GameMode.ScaryPotter3, 10, ChallengePage.Puzzle, 0, 2, "[SCARY_POTTER_3]"),
		new ChallengeDefinition(GameMode.ScaryPotter4, 10, ChallengePage.Puzzle, 0, 3, "[SCARY_POTTER_4]"),
		new ChallengeDefinition(GameMode.ScaryPotter5, 10, ChallengePage.Puzzle, 0, 4, "[SCARY_POTTER_5]"),
		new ChallengeDefinition(GameMode.ScaryPotter6, 10, ChallengePage.Puzzle, 1, 0, "[SCARY_POTTER_6]"),
		new ChallengeDefinition(GameMode.ScaryPotter7, 10, ChallengePage.Puzzle, 1, 1, "[SCARY_POTTER_7]"),
		new ChallengeDefinition(GameMode.ScaryPotter8, 10, ChallengePage.Puzzle, 1, 2, "[SCARY_POTTER_8]"),
		new ChallengeDefinition(GameMode.ScaryPotter9, 10, ChallengePage.Puzzle, 1, 3, "[SCARY_POTTER_9]"),
		new ChallengeDefinition(GameMode.ScaryPotterEndless, 10, ChallengePage.Puzzle, 1, 4, "[SCARY_POTTER_ENDLESS]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie1, 11, ChallengePage.Puzzle, 2, 0, "[I_ZOMBIE_1]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie2, 11, ChallengePage.Puzzle, 2, 1, "[I_ZOMBIE_2]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie3, 11, ChallengePage.Puzzle, 2, 2, "[I_ZOMBIE_3]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie4, 11, ChallengePage.Puzzle, 2, 3, "[I_ZOMBIE_4]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie5, 11, ChallengePage.Puzzle, 2, 4, "[I_ZOMBIE_5]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie6, 11, ChallengePage.Puzzle, 3, 0, "[I_ZOMBIE_6]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie7, 11, ChallengePage.Puzzle, 3, 1, "[I_ZOMBIE_7]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie8, 11, ChallengePage.Puzzle, 3, 2, "[I_ZOMBIE_8]"),
		new ChallengeDefinition(GameMode.PuzzleIZombie9, 11, ChallengePage.Puzzle, 3, 3, "[I_ZOMBIE_9]"),
		new ChallengeDefinition(GameMode.PuzzleIZombieEndless, 11, ChallengePage.Puzzle, 3, 4, "[I_ZOMBIE_ENDLESS]"),
		new ChallengeDefinition(GameMode.Upsell, 10, ChallengePage.Limbo, 3, 4, "Upsell"),
		new ChallengeDefinition(GameMode.Intro, 10, ChallengePage.Limbo, 2, 3, "Intro"),
		new ChallengeDefinition(GameMode.ChallengeZombiquarium, 5, ChallengePage.Challenge, 1, 2, "[ZOMBIQUARIUM]"),
		new ChallengeDefinition(GameMode.ChallengeAttackOnTitans, -1, ChallengePage.Extra, 0, 1, "[EXTRA_ATTACK_ON_TITANS_1]"),
		new ChallengeDefinition(GameMode.ChallengeAttackOnTitans2, -1, ChallengePage.Extra, 0, 2, "[EXTRA_ATTACK_ON_TITANS_2]"),
		new ChallengeDefinition(GameMode.ChallengeAttackOnTitans3, -1, ChallengePage.Extra, 0, 3, "[EXTRA_ATTACK_ON_TITANS_3]"),
		new ChallengeDefinition(GameMode.MarvelousPeople, -1, ChallengePage.Extra, 1, 1, "[EXTRA_MARVELOUS_PEOPLE]"),
		new ChallengeDefinition(GameMode.MarvelousPeople2, -1, ChallengePage.Extra, 1, 2, "[EXTRA_MARVELOUS_PEOPLE2]"),
		new ChallengeDefinition(GameMode.MarvelousPeople3, -1, ChallengePage.Extra, 1, 3, "[EXTRA_MARVELOUS_PEOPLE3]"),
		new ChallengeDefinition(GameMode.RogueConveyorbelt, -1, ChallengePage.Extra, 2, 2, "[EXTRA_CONVEYORBELT_ENDLESS]"),
		new ChallengeDefinition(GameMode.RogueConveyorbeltHard, -1, ChallengePage.Extra, 2, 1, "[EXTRA_CONVEYORBELT_HARD]"),
		new ChallengeDefinition(GameMode.ChallengeFinalBoss2, -1, ChallengePage.Extra, 2, 3, "[EXTRA_FINALBOSS2]"),
		new ChallengeDefinition(GameMode.ImitaterRandom, -1, ChallengePage.Extra, 2, 4, "[EXTRA_IMITATER_RANDOM]"),
		new ChallengeDefinition(GameMode.ChallengeColumn2, -1, ChallengePage.Extra, 3, 1, "[COLUMN_AS_YOU_SEE_EM2]"),
		new ChallengeDefinition(GameMode.ChallengeColumn3, -1, ChallengePage.Extra, 3, 2, "[COLUMN_AS_YOU_SEE_EM3]"),
		new ChallengeDefinition(GameMode.ChallengeColumn4, -1, ChallengePage.Extra, -2, -2, "[COLUMN_AS_YOU_SEE_EM4]"),
		new ChallengeDefinition(GameMode.SurvivalHellStage1, -1, ChallengePage.Survival, 2, 0, "[EXTRA_SURVIVALHELLSTAGE1]"),
		new ChallengeDefinition(GameMode.SurvivalHellStage2, -1, ChallengePage.Survival, 2, 1, "[EXTRA_SURVIVALHELLSTAGE2]"),
		new ChallengeDefinition(GameMode.SurvivalHellStage3, -1, ChallengePage.Survival, 2, 2, "[EXTRA_SURVIVALHELLSTAGE3]"),
		new ChallengeDefinition(GameMode.SurvivalHellStage4, -1, ChallengePage.Survival, 2, 3, "[EXTRA_SURVIVALHELLSTAGE4]"),
		new ChallengeDefinition(GameMode.SurvivalHellStage5, -1, ChallengePage.Survival, 2, 4, "[EXTRA_SURVIVALHELLSTAGE5]"),
		new ChallengeDefinition(GameMode.MarvelousPeople4, -1, ChallengePage.Extra, 1, 4, "[EXTRA_MARVELOUS_PEOPLE4]"),
		new ChallengeDefinition(GameMode.PoolParty, -1, ChallengePage.Extra, 0, 4, "泳池派对"),
		new ChallengeDefinition(GameMode.ChallengeBobsledBonanza2, -1, ChallengePage.Extra, 1, 0, "雪橇区2"),
		new ChallengeDefinition(GameMode.ChallengeMoreAirRaid, -1, ChallengePage.Extra, 2, 0, "空袭2"),
		new ChallengeDefinition(GameMode.BigPoolSurvivalNormalStage, -1, ChallengePage.Survival, 0, -1, "大泳池"),
		new ChallengeDefinition(GameMode.BigPoolSurvivalHardStage, -1, ChallengePage.Survival, -2, 0, "大泳池 - 困难"),
		new ChallengeDefinition(GameMode.BigPoolSurvivalHellStage, -1, ChallengePage.Survival, -2, 0, "大泳池 - 地狱"),
		new ChallengeDefinition(GameMode.ChallengeFusion, -1, ChallengePage.Extra, 0, 0, "融合！"),
		new ChallengeDefinition(GameMode.ChallengeStageRandom, -1, ChallengePage.Extra, 2, 0, "随机挑战"),
		new ChallengeDefinition(GameMode.OccupyHighGround, -1, ChallengePage.Extra, 3, 0, "占领高地"),
		new ChallengeDefinition(GameMode.CornRoulette, -1, ChallengePage.Extra, 3, 3, "[EXTRA_CORN_ROULETTE]"),
		new ChallengeDefinition(GameMode.CornRouletteEndless, -1, ChallengePage.Extra, 3, 4, "[EXTRA_CORN_ROULETTE_ENDLESS]")
	};

	public NewLawnButton mBackButton;

	public ButtonWidget[] mPageButton = new ButtonWidget[8];

	public ButtonWidget[] mChallengeButton = new ButtonWidget[102];

	public ButtonWidget[] mChallengeExtraButton = new ButtonWidget[102];

	public LawnApp mApp;

	public ChallengePage mPageIndex;

	public bool mCheatEnableChallenges;

	public UnlockingState mUnlockState;

	public int mUnlockStateCounter;

	public int mUnlockChallengeIndex;

	public float mLockShakeX;

	public float mLockShakeY;

	public ChallengeScreenWidget mChallengeScreenWidget;

	public ScrollWidget mScrollWidget;

	public int mHalfDeltaWidth;

	public int mHalfDeltaHeight;

	public NewLawnButton mReloadButton;

	private NewLawnButton mPrevPageButton;

	private NewLawnButton mNextPageButton;

	public int mDIYLevelCount;

	public string mError;

	private bool mWaiting;

	private Task mPendingTask;

	private PendingAction mPendingAction;

	private GameMode mPendingGameMode;

	private string[] trophyStrings = new string[8];

	public ChallengeScreen(LawnApp theApp, ChallengePage thePage)
	{
		mApp = theApp;
		mClip = false;
		mCheatEnableChallenges = mApp.mPlayerInfo.mHasUsedCheatKeys;
		mPageIndex = thePage;
		mWaiting = false;
		mPendingTask = null;
		mPendingAction = PendingAction.None;
		mUnlockState = UnlockingState.Off;
		mUnlockChallengeIndex = -1;
		mUnlockStateCounter = 0;
		mLockShakeX = 0f;
		mLockShakeY = 0f;
		if (mPageIndex == ChallengePage.OnlineLevel && !OnlineLevel.IsInitialized)
		{
			mWaiting = true;
			mPendingTask = OnlineLevel.InitializeAsync();
			mPendingAction = PendingAction.Initialize;
		}
		mBackButton = GameButton.MakeNewButton(100, this, "[BACK_TO_MENU]", null, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW);
		mBackButton.Resize(18, Constants.BackBufferSize.X - 40, 130, 40);
		mBackButton.mTextDownOffsetX = 1;
		mBackButton.mTextDownOffsetY = 1;
		mBackButton.mColors[0] = new SexyColor(42, 42, 90);
		mBackButton.mColors[1] = new SexyColor(42, 42, 90);
		if (mPageIndex == ChallengePage.DIY || mPageIndex == ChallengePage.OnlineLevel)
		{
			mReloadButton = GameButton.MakeNewButton(101, this, (mPageIndex == ChallengePage.OnlineLevel) ? "Refresh" : "Reload", null, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW);
			mReloadButton.Resize(Constants.BackBufferSize.Y - 148, Constants.BackBufferSize.X - 40, 130, 40);
			mReloadButton.mTextDownOffsetX = 1;
			mReloadButton.mTextDownOffsetY = 1;
			mReloadButton.mColors[0] = new SexyColor(42, 42, 90);
			mReloadButton.mColors[1] = new SexyColor(42, 42, 90);
		}
		if (mPageIndex == ChallengePage.OnlineLevel)
		{
			mPrevPageButton = GameButton.MakeNewButton(102, this, "Prev", null, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW);
			mPrevPageButton.Resize(190, Constants.BackBufferSize.X - 40, 130, 40);
			mPrevPageButton.mTextDownOffsetX = 1;
			mPrevPageButton.mTextDownOffsetY = 1;
			mPrevPageButton.mColors[0] = new SexyColor(42, 42, 90);
			mPrevPageButton.mColors[1] = new SexyColor(42, 42, 90);
			mNextPageButton = GameButton.MakeNewButton(103, this, "Next", null, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW, AtlasResources.IMAGE_SEEDCHOOSER_BUTTON2_GLOW);
			mNextPageButton.Resize(660, Constants.BackBufferSize.X - 40, 130, 40);
			mNextPageButton.mTextDownOffsetX = 1;
			mNextPageButton.mTextDownOffsetY = 1;
			mNextPageButton.mColors[0] = new SexyColor(42, 42, 90);
			mNextPageButton.mColors[1] = new SexyColor(42, 42, 90);
		}
		mChallengeScreenWidget = new ChallengeScreenWidget(mApp);
		mChallengeScreenWidget.Resize(M(0), M1(0), M2(1000), M3(119) * 4);
		mScrollWidget = new ScrollWidget();
		mScrollWidget.AddWidget(mChallengeScreenWidget);
		mScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mScrollWidget.Resize(M(0), M1(0) + 93, M2(1000), M3(Constants.BackBufferSize.X) - 93);
		AddWidget(mScrollWidget);
		for (int i = 0; i < 8; i++)
		{
			mPageButton[i] = new ButtonWidget(3000 + i, this);
			mPageButton[i].mDoFinger = true;
			if (i == 2)
			{
				mPageButton[i].mLabel = TodStringFile.TodStringTranslate("Limbo Page");
			}
			else
			{
				mPageButton[i].mLabel = TodCommon.TodReplaceNumberString("[PAGE_X]", "{PAGE}", i);
			}
			mPageButton[i].mButtonImage = AtlasResources.IMAGE_BLANK;
			mPageButton[i].mOverImage = AtlasResources.IMAGE_BLANK;
			mPageButton[i].mDownImage = AtlasResources.IMAGE_BLANK;
			mPageButton[i].SetFont(Resources.FONT_BRIANNETOD12);
			mPageButton[i].mColors[0] = new SexyColor(255, 240, 0);
			mPageButton[i].mColors[1] = new SexyColor(220, 220, 0);
			mPageButton[i].Resize(S(200 + i * 100), S(Constants.BackBufferSize.X - 60), S(100), S(75));
			if (!ShowPageButtons() || i == 0 || i == 3)
			{
				mPageButton[i].mVisible = false;
			}
		}
		for (int j = 0; j < gChallengeDefs.Length; j++)
		{
			ChallengeDefinition challengeDefinition = gChallengeDefs[j];
			mChallengeButton[j] = new ButtonWidget((int)(200 + challengeDefinition.mChallengeMode - 1), this);
			mChallengeButton[j].mDoFinger = true;
			mChallengeButton[j].mFrameNoDraw = true;
			if (challengeDefinition.mPage == ChallengePage.Challenge || challengeDefinition.mPage == ChallengePage.Limbo || challengeDefinition.mPage == ChallengePage.Puzzle || challengeDefinition.mPage == ChallengePage.Extra)
			{
				mChallengeButton[j].Resize(100 + S(38 + challengeDefinition.mCol * 155), S(7 + challengeDefinition.mRow * 119), S(104), S(115));
			}
			else if (challengeDefinition.mPage == ChallengePage.Survival)
			{
				mChallengeButton[j].Resize(100 + S(68 + challengeDefinition.mCol * 140), S(7 + challengeDefinition.mRow * 119), S(104), S(115));
			}
			else if (challengeDefinition.mPage == ChallengePage.Challenge2)
			{
				mChallengeButton[j].Resize(100 + S(38 + challengeDefinition.mCol * 155), S(7 + (challengeDefinition.mRow + 4) * 119), S(104), S(115));
			}
			else
			{
				mChallengeButton[j].Resize(100 + S(38 + challengeDefinition.mCol * 155), S(125 + challengeDefinition.mRow * 145), S(104), S(115));
			}
			if (MoreTrophiesNeeded(j) != 0)
			{
				mChallengeButton[j].mDoFinger = false;
				mChallengeButton[j].mDisabled = true;
			}
			if (mApp.IsLevelWithHighPresentPlantDropRate(challengeDefinition.mChallengeMode))
			{
				mChallengeExtraButton[j] = GameButton.MakeNewButton(4000, this, string.Empty, null, AtlasResources.IMAGE_MINIGAME_HIGHDROPRATE, AtlasResources.IMAGE_MINIGAME_HIGHDROPRATE, AtlasResources.IMAGE_MINIGAME_HIGHDROPRATE);
				mChallengeExtraButton[j].Resize(mChallengeButton[j].mX + 55, mChallengeButton[j].mY, mChallengeButton[j].mWidth - 55, 45);
			}
		}
		if (mPageIndex == ChallengePage.DIY)
		{
			ReloadDIYLevels();
		}
		UpdateButtons();
		if (mApp.mGameMode != GameMode.Upsell || mApp.mGameScene != GameScenes.LevelIntro)
		{
			mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ChooseYourSeeds);
		}
		if (mPageIndex == ChallengePage.Survival && mApp.mPlayerInfo.mHasNewSurvival)
		{
			SetUnlockChallengeIndex(mPageIndex, aIsIZombie: false);
			mApp.mPlayerInfo.mHasNewSurvival = false;
		}
		else if (mPageIndex == ChallengePage.Challenge && mApp.mPlayerInfo.mHasNewMiniGame)
		{
			SetUnlockChallengeIndex(mPageIndex, aIsIZombie: false);
			mApp.mPlayerInfo.mHasNewMiniGame = false;
		}
		else if (mPageIndex == ChallengePage.Puzzle && mApp.mPlayerInfo.mHasNewVasebreaker)
		{
			SetUnlockChallengeIndex(mPageIndex, aIsIZombie: false);
			mApp.mPlayerInfo.mHasNewVasebreaker = false;
		}
		else if (mPageIndex == ChallengePage.Puzzle && mApp.mPlayerInfo.mHasNewIZombie)
		{
			SetUnlockChallengeIndex(mPageIndex, aIsIZombie: true);
			mApp.mPlayerInfo.mHasNewIZombie = false;
		}
	}

	public override bool BackButtonPress()
	{
		mApp.KillChallengeScreen();
		mApp.DoBackToMain();
		if (mPageIndex == ChallengePage.OnlineLevel)
		{
			mApp.ShowGameSelector();
		}
		else
		{
			mApp.ShowGameSelectorMoreGames();
		}
		return true;
	}

	public void SetUnlockChallengeIndex(ChallengePage thePage, bool aIsIZombie)
	{
		mUnlockState = UnlockingState.Shaking;
		mUnlockStateCounter = 100;
		mUnlockChallengeIndex = 0;
		bool flag = true;
		for (int i = 0; i < gChallengeDefs.Length; i++)
		{
			ChallengeDefinition challengeDefinition = gChallengeDefs[i];
			if (challengeDefinition.mPage != thePage || (thePage == ChallengePage.Puzzle && ((!aIsIZombie && !IsScaryPotterLevel(challengeDefinition.mChallengeMode)) || (aIsIZombie && !IsIZombieLevel(challengeDefinition.mChallengeMode)))))
			{
				continue;
			}
			int num = AccomplishmentsNeeded(i);
			if (num <= 0)
			{
				if (challengeDefinition.mChallengeMode == GameMode.ChallengeBeghouledTwist)
				{
					flag = true;
				}
				if (challengeDefinition.mChallengeMode != GameMode.ChallengeZombiquarium || !flag)
				{
					mUnlockChallengeIndex = i;
				}
			}
		}
	}

	public int MoreTrophiesNeeded(int aChallengeIndex)
	{
		ChallengeDefinition challengeDefinition = gChallengeDefs[aChallengeIndex];
		if (challengeDefinition.mPage == ChallengePage.Extra)
		{
			return 0;
		}
		if (mCheatEnableChallenges)
		{
			return 0;
		}
		if (mApp.mGameMode == GameMode.Upsell && mApp.mGameScene == GameScenes.LevelIntro)
		{
			if (challengeDefinition.mChallengeMode == GameMode.ChallengeFinalBoss)
			{
				return 1;
			}
			return 0;
		}
		if (mApp.IsTrialStageLocked())
		{
			if (challengeDefinition.mPage == ChallengePage.Puzzle && challengeDefinition.mChallengeMode >= GameMode.ScaryPotter4)
			{
				if (challengeDefinition.mChallengeMode == GameMode.ScaryPotter4)
				{
					return 1;
				}
				return 2;
			}
			if (challengeDefinition.mPage == ChallengePage.Challenge && challengeDefinition.mChallengeMode >= GameMode.ChallengeRainingSeeds)
			{
				if (challengeDefinition.mChallengeMode == GameMode.ChallengeRainingSeeds)
				{
					return 1;
				}
				return 2;
			}
			if (challengeDefinition.mPage == ChallengePage.Survival && challengeDefinition.mChallengeMode >= GameMode.SurvivalNormalStage4)
			{
				if (challengeDefinition.mChallengeMode == GameMode.SurvivalNormalStage4)
				{
					return 1;
				}
				return 2;
			}
		}
		if (challengeDefinition.mPage == ChallengePage.Puzzle && IsScaryPotterLevel(challengeDefinition.mChallengeMode))
		{
			int num = 0;
			for (int i = 0; i < gChallengeDefs.Length; i++)
			{
				ChallengeDefinition challengeDefinition2 = gChallengeDefs[i];
				if (IsScaryPotterLevel(challengeDefinition2.mChallengeMode) && mApp.HasBeatenChallenge(challengeDefinition2.mChallengeMode))
				{
					num++;
				}
			}
			if (challengeDefinition.mChallengeMode < GameMode.ScaryPotter4 || mApp.HasFinishedAdventure() || num < 3)
			{
				int num2 = (int)(challengeDefinition.mChallengeMode - 50);
				return TodCommon.ClampInt(num2 - num, 0, 9);
			}
			if (challengeDefinition.mChallengeMode == GameMode.ScaryPotter4)
			{
				return 1;
			}
			return 2;
		}
		if (challengeDefinition.mPage == ChallengePage.Puzzle && IsIZombieLevel(challengeDefinition.mChallengeMode))
		{
			int num3 = 0;
			for (int j = 0; j < gChallengeDefs.Length; j++)
			{
				ChallengeDefinition challengeDefinition3 = gChallengeDefs[j];
				if (IsIZombieLevel(challengeDefinition3.mChallengeMode) && mApp.HasBeatenChallenge(challengeDefinition3.mChallengeMode))
				{
					num3++;
				}
			}
			if (challengeDefinition.mChallengeMode < GameMode.PuzzleIZombie4 || mApp.HasFinishedAdventure() || num3 < 3)
			{
				int num4 = (int)(challengeDefinition.mChallengeMode - 60);
				return TodCommon.ClampInt(num4 - num3, 0, 9);
			}
			if (challengeDefinition.mChallengeMode == GameMode.PuzzleIZombie4)
			{
				return 1;
			}
			return 2;
		}
		int num5 = challengeDefinition.mRow * 5 + challengeDefinition.mCol;
		if (challengeDefinition.mPage == ChallengePage.Challenge && !mApp.HasFinishedAdventure())
		{
			if (num5 < 3)
			{
				return 0;
			}
			if (num5 == 3)
			{
				return 1;
			}
			return 2;
		}
		if (challengeDefinition.mPage == ChallengePage.Survival && !mApp.HasFinishedAdventure())
		{
			if (num5 < 3)
			{
				return 0;
			}
			if (num5 == 3)
			{
				return 1;
			}
			return 2;
		}
		int numTrophies = mApp.GetNumTrophies(challengeDefinition.mPage);
		int num6 = 0;
		if (challengeDefinition.mPage == ChallengePage.Limbo)
		{
			return 0;
		}
		if (mApp.IsSurvivalEndless(challengeDefinition.mChallengeMode))
		{
			return Math.Max(10 - numTrophies, 0);
		}
		if (challengeDefinition.mPage == ChallengePage.Survival || challengeDefinition.mPage == ChallengePage.Challenge || challengeDefinition.mPage == ChallengePage.Challenge2)
		{
			num6 = 3;
		}
		int num7 = numTrophies + num6;
		if (num5 >= num7)
		{
			return num5 - num7 + 1;
		}
		return 0;
	}

	public void ReloadDIYLevels()
	{
		mError = CreativeLevelManager.ReloadDIYLevel();
		mDIYLevelCount = 0;
		while (mDIYLevelCount < mChallengeButton.Length && CreativeLevelManager.IsCreativeMode((GameMode)(40000 + mDIYLevelCount)))
		{
			ButtonWidget buttonWidget = mChallengeButton[mDIYLevelCount];
			int num = mDIYLevelCount / 5;
			int num2 = mDIYLevelCount % 5;
			buttonWidget.mId = 200 + mDIYLevelCount;
			buttonWidget.Resize(100 + S(38 + num2 * 155), S(7 + num * 119), S(104), S(115));
			buttonWidget.mDoFinger = true;
			buttonWidget.mDisabled = false;
			mDIYLevelCount++;
		}
	}

	public GameMode IndexToGameMode(int index)
	{
		if (mPageIndex == ChallengePage.DIY)
		{
			return (GameMode)(40000 + index);
		}
		if (mPageIndex == ChallengePage.OnlineLevel)
		{
			return (GameMode)(50000 + index);
		}
		return (GameMode)(index + 1);
	}

	public bool ShowPageButtons()
	{
		if (mApp.mTodCheatKeys && mPageIndex != ChallengePage.Survival && mPageIndex != ChallengePage.Puzzle && mPageIndex != ChallengePage.DIY)
		{
			return mPageIndex != ChallengePage.OnlineLevel;
		}
		return false;
	}

	public void UpdateButtons()
	{
		if (mPageIndex == ChallengePage.DIY || mPageIndex == ChallengePage.OnlineLevel)
		{
			mScrollWidget.Resize(0, 68, 1000, mBackButton.mY - 68 - 8);
			ButtonWidget[] array = mChallengeExtraButton;
			foreach (ButtonWidget buttonWidget in array)
			{
				if (buttonWidget != null)
				{
					buttonWidget.mVisible = false;
				}
			}
		}
		if (mPageIndex == ChallengePage.OnlineLevel)
		{
			int num = ((!mWaiting) ? OnlineLevel.CurrentLevels.Count : 0);
			for (int j = 0; j < mChallengeButton.Length; j++)
			{
				bool flag = j < num;
				mChallengeButton[j].mVisible = flag;
				mChallengeButton[j].mDisabled = false;
				mChallengeButton[j].mDoFinger = flag;
				if (flag)
				{
					int num2 = j / 5;
					int num3 = j % 5;
					mChallengeButton[j].mId = 200 + j;
					mChallengeButton[j].Resize(100 + S(38 + num3 * 155), S(7 + num2 * 119), S(104), S(115));
				}
			}
			for (int k = 0; k < 8; k++)
			{
				mPageButton[k].mVisible = false;
			}
			if (mPrevPageButton != null)
			{
				mPrevPageButton.mVisible = true;
			}
			if (mNextPageButton != null)
			{
				mNextPageButton.mVisible = true;
			}
			UpdatePageButtons();
			int num4 = Math.Max(1, (Math.Max(num, 1) + 4) / 5);
			mChallengeScreenWidget.Resize(M(0), M1(0), M2(1000), M3(7 + num4 * 119));
			mScrollWidget.ClientSizeChanged();
			return;
		}
		if (mPageIndex == ChallengePage.DIY)
		{
			ReloadDIYLevels();
			for (int l = 0; l < gChallengeDefs.Length; l++)
			{
				mChallengeButton[l].mVisible = l < mDIYLevelCount;
				mChallengeButton[l].mDisabled = false;
				mChallengeButton[l].mDoFinger = l < mDIYLevelCount;
			}
			for (int m = 0; m < 8; m++)
			{
				mPageButton[m].mVisible = false;
			}
			int num5 = Math.Max(1, (mDIYLevelCount + 4) / 5);
			mChallengeScreenWidget.Resize(M(0), M1(0), M2(1000), M3(7 + num5 * 119));
			mScrollWidget.ClientSizeChanged();
			if (mPrevPageButton != null)
			{
				mPrevPageButton.mVisible = false;
			}
			if (mNextPageButton != null)
			{
				mNextPageButton.mVisible = false;
			}
			return;
		}
		for (int n = 0; n < gChallengeDefs.Length; n++)
		{
			ChallengeDefinition challengeDefinition = gChallengeDefs[n];
			GameMode mChallengeMode = challengeDefinition.mChallengeMode;
			bool flag2 = (uint)(mChallengeMode - 151) <= 1u;
			if (flag2 || (challengeDefinition.mPage != mPageIndex && (challengeDefinition.mPage != ChallengePage.Challenge2 || mPageIndex != ChallengePage.Challenge)))
			{
				mChallengeButton[n].mVisible = false;
				if (mChallengeExtraButton[n] != null)
				{
					mChallengeExtraButton[n].mVisible = false;
				}
			}
			else
			{
				mChallengeButton[n].mVisible = true;
				if (mChallengeExtraButton[n] != null)
				{
					mChallengeExtraButton[n].mVisible = true;
				}
			}
		}
		for (int num6 = 0; num6 < 8; num6++)
		{
			if (num6 == (int)mPageIndex)
			{
				mPageButton[num6].mColors[0] = new SexyColor(64, 64, 64);
				mPageButton[num6].mDisabled = true;
			}
			else
			{
				mPageButton[num6].mColors[0] = new SexyColor(255, 240, 0);
				mPageButton[num6].mDisabled = false;
			}
		}
		if (mPageIndex == ChallengePage.Challenge)
		{
			mChallengeScreenWidget.Resize(M(0), M1(0), M2(1000), M3(864));
			mScrollWidget.ClientSizeChanged();
		}
		else
		{
			mChallengeScreenWidget.Resize(M(0), M1(0), M2(1000), M3(507));
			mScrollWidget.ClientSizeChanged();
		}
		if (mPrevPageButton != null)
		{
			mPrevPageButton.mVisible = false;
		}
		if (mNextPageButton != null)
		{
			mNextPageButton.mVisible = false;
		}
	}

	public bool ChallengeModeRecordsTime(GameMode theGameMode)
	{
		if (theGameMode >= GameMode.SurvivalNormalStage1)
		{
			return theGameMode <= GameMode.SurvivalEndlessStage5;
		}
		return false;
	}

	public int AccomplishmentsNeeded(int aChallengeIndex)
	{
		int num = MoreTrophiesNeeded(aChallengeIndex);
		ChallengeDefinition challengeDefinition = gChallengeDefs[aChallengeIndex];
		if (mApp.IsSurvivalEndless(challengeDefinition.mChallengeMode) && num <= 3 && mApp.GetNumTrophies(ChallengePage.Survival) < 10 && mApp.HasFinishedAdventure() && !mApp.IsTrialStageLocked())
		{
			num = 1;
		}
		if (mCheatEnableChallenges)
		{
			num = 0;
		}
		return num;
	}

	public override void Draw(Graphics g)
	{
		g.SetLinearBlend(linear: true);
		LawnCommon.DrawImageBox(g, new TRect(-mHalfDeltaWidth, -mHalfDeltaHeight, mWidth, mHeight), AtlasResources.IMAGE_ALMANAC_ROUNDED_OUTLINE);
		if (mPageIndex == ChallengePage.Survival)
		{
			TodCommon.TodDrawString(g, "[PICK_AREA]", 500, 22, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(220, 220, 220), DrawStringJustification.Center);
		}
		else if (mPageIndex == ChallengePage.Puzzle)
		{
			TodCommon.TodDrawString(g, "[SCARY_POTTER]", 500, 22, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(220, 220, 220), DrawStringJustification.Center);
		}
		else if (mPageIndex == ChallengePage.Extra)
		{
			TodCommon.TodDrawString(g, "[GAMESELECTOR_EXTRA]", 500, 22, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(220, 220, 220), DrawStringJustification.Center);
		}
		else if (mPageIndex == ChallengePage.DIY)
		{
			TodCommon.TodDrawString(g, "本地关卡", 500, 22, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(220, 220, 220), DrawStringJustification.Center);
		}
		else if (mPageIndex == ChallengePage.OnlineLevel)
		{
			TodCommon.TodDrawString(g, "在线关卡", 500, 22, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(220, 220, 220), DrawStringJustification.Center);
		}
		else
		{
			TodCommon.TodDrawString(g, "[PICK_CHALLENGE]", 500, 22, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(220, 220, 220), DrawStringJustification.Center);
		}
		int num = 0;
		int num2 = 0;
		if (mPageIndex == ChallengePage.OnlineLevel)
		{
			num = 0;
			for (int i = 0; i < OnlineLevel.CurrentLevels.Count; i++)
			{
				OnlineLevelPreview onlineLevelPreview = OnlineLevel.CurrentLevels[i];
				if (!onlineLevelPreview.IsEndless)
				{
					num++;
					if (mApp.mPlayerInfo.GetOnlineChallengeRecord(onlineLevelPreview.LevelId) > 0)
					{
						num2++;
					}
				}
			}
		}
		else if (mPageIndex != ChallengePage.DIY)
		{
			num2 = mApp.GetNumTrophies(mPageIndex);
		}
		if (mPageIndex == ChallengePage.Survival)
		{
			num = 10;
		}
		else if (mPageIndex == ChallengePage.Challenge)
		{
			num = 20;
		}
		else if (mPageIndex == ChallengePage.Puzzle)
		{
			num = 18;
		}
		if (num > 0)
		{
			string theText;
			if (mPageIndex == ChallengePage.OnlineLevel)
			{
				theText = LawnApp.ToString(num2) + "/" + LawnApp.ToString(num);
			}
			else
			{
				if (trophyStrings[(int)mPageIndex] == null)
				{
					trophyStrings[(int)mPageIndex] = LawnApp.ToString(num2) + "/" + LawnApp.ToString(num);
				}
				theText = trophyStrings[(int)mPageIndex];
			}
			TodCommon.TodDrawString(g, theText, 700, 39, Resources.FONT_DWARVENTODCRAFT12, new SexyColor(255, 240, 0), DrawStringJustification.Center);
		}
		TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_TROPHY, 680f, 15f, 0.5f, 0.5f);
		if (mPageIndex != ChallengePage.OnlineLevel)
		{
			return;
		}
		if (mWaiting)
		{
			string theText2;
			if (string.IsNullOrEmpty(mError))
			{
				theText2 = (string.IsNullOrEmpty(OnlineLevel.LastError) ? "加载中..." : OnlineLevel.LastError);
			}
			else
			{
				theText2 = mError;
			}
			TodCommon.TodDrawString(g, theText2, 500, 270, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(255, 240, 0), DrawStringJustification.Center);
			return;
		}
		string theText3 = LawnApp.ToString(Math.Max(OnlineLevel.CurrentPage, 1)) + "/" + LawnApp.ToString(Math.Max(OnlineLevel.TotalPages, 1));
		TodCommon.TodDrawString(g, theText3, 500, Constants.BackBufferSize.X - 15, Resources.FONT_DWARVENTODCRAFT12, new SexyColor(42, 42, 90), DrawStringJustification.Center);
		if (!string.IsNullOrEmpty(mError))
		{
			TodCommon.TodDrawString(g, mError, 500, 270, Resources.FONT_BRIANNETOD12, new SexyColor(255, 240, 0), DrawStringJustification.Center);
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateScreen();
		UpdateToolTip();
		if (mPendingTask != null && mPendingTask.IsCompleted)
		{
			HandlePendingTaskCompletion();
		}
		if (mUnlockStateCounter > 0)
		{
			mUnlockStateCounter--;
		}
		if (mUnlockState == UnlockingState.Shaking)
		{
			if (mUnlockStateCounter == 0)
			{
				mApp.PlayFoley(FoleyType.Paper);
				mUnlockState = UnlockingState.Fading;
				mUnlockStateCounter = 50;
				mLockShakeX = 0f;
				mLockShakeY = 0f;
			}
			else
			{
				mLockShakeX = TodCommon.RandRangeFloat(-2f, 2f);
				mLockShakeY = TodCommon.RandRangeFloat(-2f, 2f);
			}
		}
		else if (mUnlockState == UnlockingState.Fading && mUnlockStateCounter == 0)
		{
			mUnlockState = UnlockingState.Off;
			mUnlockStateCounter = 0;
			mUnlockChallengeIndex = -1;
		}
		MarkDirty();
	}

	private void HandlePendingTaskCompletion()
	{
		bool flag = !mPendingTask.IsFaulted && !mPendingTask.IsCanceled;
		if (flag && mPendingTask is Task<bool> task)
		{
			flag = task.Result;
		}
		if (!flag)
		{
			mError = mPendingTask.Exception?.GetBaseException().Message ?? OnlineLevel.LastError;
		}
		else
		{
			mError = OnlineLevel.LastError;
		}
		PendingAction pendingAction = mPendingAction;
		GameMode theGameMode = mPendingGameMode;
		mWaiting = false;
		mPendingTask = null;
		mPendingAction = PendingAction.None;
		mPendingGameMode = GameMode.Adventure;
		if ((pendingAction == PendingAction.DownloadLevel) & flag)
		{
			mApp.KillChallengeScreen();
			mApp.PreNewGame(theGameMode, theLookForSavedGame: true);
		}
		else
		{
			UpdateButtons();
		}
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		AddWidget(mBackButton);
		if (mReloadButton != null)
		{
			AddWidget(mReloadButton);
		}
		if (mPrevPageButton != null)
		{
			AddWidget(mPrevPageButton);
		}
		if (mNextPageButton != null)
		{
			AddWidget(mNextPageButton);
		}
		for (int i = 0; i < mChallengeButton.Length; i++)
		{
			mChallengeScreenWidget.AddWidget(mChallengeButton[i]);
			if (mChallengeExtraButton[i] != null)
			{
				mChallengeScreenWidget.AddWidget(mChallengeExtraButton[i]);
			}
		}
		for (int j = 0; j < 8; j++)
		{
			AddWidget(mPageButton[j]);
		}
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		RemoveWidget(mBackButton);
		if (mReloadButton != null)
		{
			RemoveWidget(mReloadButton);
		}
		if (mPrevPageButton != null)
		{
			RemoveWidget(mPrevPageButton);
		}
		if (mNextPageButton != null)
		{
			RemoveWidget(mNextPageButton);
		}
		for (int i = 0; i < mChallengeButton.Length; i++)
		{
			mChallengeScreenWidget.RemoveWidget(mChallengeButton[i]);
			if (mChallengeExtraButton[i] != null)
			{
				mChallengeScreenWidget.RemoveWidget(mChallengeExtraButton[i]);
			}
		}
		for (int j = 0; j < 8; j++)
		{
			RemoveWidget(mPageButton[j]);
		}
	}

	public void ButtonPress(int theId)
	{
		mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
	}

	public void ButtonDepress(int theId)
	{
		if (theId == 100)
		{
			mApp.KillChallengeScreen();
			mApp.DoBackToMain();
			if (mPageIndex == ChallengePage.OnlineLevel || mPageIndex == ChallengePage.DIY)
			{
				mApp.ShowGameSelector();
			}
			else
			{
				mApp.ShowGameSelectorMoreGames();
			}
		}
		else
		{
			if (mWaiting)
			{
				return;
			}
			if (theId == 101 && mPageIndex == ChallengePage.DIY)
			{
				ReloadDIYLevels();
				UpdateButtons();
				return;
			}
			if (theId == 101 && mPageIndex == ChallengePage.OnlineLevel)
			{
				mError = null;
				mPendingTask = OnlineLevel.ReloadAsync();
				mPendingAction = ((!OnlineLevel.IsInitialized) ? PendingAction.Initialize : PendingAction.ReloadPage);
				mWaiting = true;
				UpdateButtons();
				return;
			}
			if (theId == 102 && mPageIndex == ChallengePage.OnlineLevel)
			{
				mError = null;
				mPendingTask = OnlineLevel.LoadPageAsync(OnlineLevel.CurrentPage - 1);
				mPendingAction = PendingAction.LoadPrevPage;
				mWaiting = true;
				UpdateButtons();
				return;
			}
			if (theId == 103 && mPageIndex == ChallengePage.OnlineLevel)
			{
				mError = null;
				mPendingTask = OnlineLevel.LoadPageAsync(OnlineLevel.CurrentPage + 1);
				mPendingAction = PendingAction.LoadNextPage;
				mWaiting = true;
				UpdateButtons();
				return;
			}
			int num = theId - 200;
			if (num >= 0 && num < 152)
			{
				GameMode theGameMode = IndexToGameMode(num);
				if (mPageIndex == ChallengePage.OnlineLevel)
				{
					if (num < OnlineLevel.CurrentLevels.Count)
					{
						OnlineLevelPreview onlineLevelPreview = OnlineLevel.CurrentLevels[num];
						string text = onlineLevelPreview.Name;
						if (Constants.Language == Constants.LanguageIndex.zh_cn && !string.IsNullOrEmpty(onlineLevelPreview.ChineseName))
						{
							text = onlineLevelPreview.ChineseName;
						}
						else if (Constants.Language == Constants.LanguageIndex.en && !string.IsNullOrEmpty(onlineLevelPreview.EnglishName))
						{
							text = onlineLevelPreview.EnglishName;
						}
						if (string.IsNullOrEmpty(text))
						{
							text = onlineLevelPreview.LevelId;
						}
						OnlineLevelDialog onlineLevelDialog = new OnlineLevelDialog(mApp, theGameMode, text.Replace("|", string.Empty), onlineLevelPreview);
						mApp.AddDialog(onlineLevelDialog.mId, onlineLevelDialog);
						LawnApp.CenterDialog(onlineLevelDialog, onlineLevelDialog.mWidth, onlineLevelDialog.mHeight);
						mApp.mWidgetManager.SetFocus(onlineLevelDialog);
					}
				}
				else
				{
					mApp.KillChallengeScreen();
					mApp.PreNewGame(theGameMode, theLookForSavedGame: true);
				}
			}
			else
			{
				int num2 = theId - 3000;
				if (num2 >= 0 && num2 < 8)
				{
					mPageIndex = (ChallengePage)num2;
					UpdateButtons();
				}
				if (theId == 4000)
				{
					mApp.DoDialog(66, isModal: true, "[DIALOG_HIGH_DROP_RATE_HEADER]", "[DIALOG_HIGH_DROP_RATE]", "[DIALOG_BUTTON_OK]", 3);
				}
			}
		}
	}

	public void StartEnterOnlineLevel(GameMode theGameMode, OnlineLevelPreview thePreview)
	{
		if (!thePreview.IsDataLoaded)
		{
			mError = null;
			mPendingTask = OnlineLevel.DownloadLevelDataAsync(thePreview, theGameMode);
			mPendingAction = PendingAction.DownloadLevel;
			mPendingGameMode = theGameMode;
			mWaiting = true;
			UpdateButtons();
		}
		else
		{
			mApp.KillChallengeScreen();
			mApp.PreNewGame(theGameMode, theLookForSavedGame: true);
		}
	}

	public bool IsScaryPotterLevel(GameMode theGameMode)
	{
		if (theGameMode >= GameMode.ScaryPotter1)
		{
			return theGameMode <= GameMode.ScaryPotterEndless;
		}
		return false;
	}

	public bool IsIZombieLevel(GameMode theGameMode)
	{
		if (theGameMode >= GameMode.PuzzleIZombie1)
		{
			return theGameMode <= GameMode.PuzzleIZombieEndless;
		}
		return false;
	}

	public void UpdateToolTip()
	{
	}

	public void ButtonMouseMove(int id, int x, int y)
	{
	}

	public void ButtonMouseLeave(int id)
	{
	}

	public void ButtonMouseEnter(int id)
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

	public void UpdateScreen()
	{
		Resize(mX, mY, mApp.mScreenScales.mVirtualWidth, mApp.mScreenScales.mVirtualHeight);
		mHalfDeltaWidth = (mWidth - Constants.BOARD_WIDTH) / 2;
		mHalfDeltaHeight = (mHeight - Constants.BOARD_HEIGHT) / 2;
		FullRect = new TRect(-mX - mHalfDeltaWidth, mY - mHalfDeltaHeight, mWidth, mHeight);
		UpdateUIPosition();
	}

	public void UpdateUIPosition()
	{
	}

	public static ChallengeDefinition GetChallengeDefFromMode(GameMode mode)
	{
		return gChallengeDefs[(int)(mode - 1 - ((mode > GameMode.Quickplay1) ? 50 : 0))];
	}

	private void UpdatePageButtons()
	{
		if (mPrevPageButton != null)
		{
			mPrevPageButton.mDisabled = mWaiting || OnlineLevel.CurrentPage <= 1;
		}
		if (mNextPageButton != null)
		{
			mNextPageButton.mDisabled = mWaiting || OnlineLevel.CurrentPage >= OnlineLevel.TotalPages;
		}
		if (mReloadButton != null)
		{
			mReloadButton.mDisabled = mWaiting;
		}
	}
}
