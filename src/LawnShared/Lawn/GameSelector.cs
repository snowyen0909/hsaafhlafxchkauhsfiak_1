using System;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

public class GameSelector : Widget, ButtonListener, StoreListener, AlmanacListener, MiniGamesWidgetListener, QuickPlayWidgetListener
{
	public enum GameSelectorAdventureType
	{
		First,
		Second,
		Quick
	}

	private enum GameSelectorScreenState
	{
		Main,
		MoreGames,
		Achievements,
		QuickPlay,
		MenuStyle2
	}

	public enum GameSelectorScene
	{
		Main,
		MoreGames,
		Puzzle,
		MiniGame,
		Survival,
		CreativeLevel
	}

	public LawnApp mApp;

	public NewLawnButton mAdventureButton;

	public NewLawnButton mMoreWaysToPlayButton;

	public NewLawnButton mZenGardenButton;

	public NewLawnButton mOptionsButton;

	public DialogButton mStoreButton;

	public DialogButton mAlmanacButton;

	public DialogButton mUserDialogButton;

	public DockMainMenuButton mOnlineLevelsButton;

	public DialogButton mPuzzleButton;

	public DialogButton mMiniGamesButton;

	public ToggleButton mVaseBreakerButton;

	public ToggleButton mIZombieButton;

	public ToggleButton mMiscButton;

	public DialogButton mSurvivalButton;

	public DialogButton mMoreWaysBackButton;

	public DialogButton mChallengePageSurvivalButton;

	public DialogButton mChallengePageLimboButton;

	public DialogButton mCreativeLevelButton;

	public DialogButton mComicButton;

	public DialogButton mFumeshroomGnomeButton;

	public DialogButton mUnlockButton;

	public ToggleButton mAdventureHardButton;

	public DialogButton mCommunityButton;

	public DialogButton mExitButton;

	public DialogButton mTrophyButton;

	public int mSelectedQuickplayButtonId;

	public MoreGamesListWidget mMoreGamesListWidget;

	public ScrollWidget mMoreGamesScrollWidget;

	public string mLexText = string.Empty;

	public AchievementsWidget mAchievementsWidget;

	public ScrollWidget mAchievementsScrollWidget;

	public QuickPlayWidget mQuickplayWidget;

	public ScrollWidget mQuickplayScrollWidget;

	public MiniGamesWidget mMiniGamesWidget;

	public ScrollWidget mMiniGamesScrollWidget;

	public bool mQuickplayLocked;

	public bool mShowStartButton;

	public TodParticleSystem mTrophyParticleID;

	public Reanimation mWoodSignReanimID;

	public Reanimation[] mCloudReanimID = new Reanimation[6];

	public int[] mCloudCounter = new int[6];

	public Reanimation[] mFlowerReanimID = new Reanimation[3];

	public int mLeafCounter;

	public SelectorSignState mSignState;

	public bool mInUserDialog;

	public int mLevel;

	public bool mLoading;

	public bool mHasTrophy;

	public bool mUnlockSelectorCheat;

	public int mSlideCounter;

	public int mStartX;

	public int mStartY;

	public int mDestX;

	public int mDestY;

	public bool mNeedToPlayRollIn;

	public int mFadeInCounter;

	public double mPekingCounter;

	private static string cachedWelcomeString;

	private static string cachedWelcomeStringName;

	private string cachedTrophyString = string.Empty;

	private int cachedTrophyStringCount = -1;

	private bool wantToExitFromAchievementsViaBackButton;

	private float woodSignY;

	public bool mDoNewGameAfterStore;

	public int mQuickplaySlideCounter;

	public bool mRetractingQuickplay;

	public int mLeftLength;

	public int mHalfDeltaWidth;

	public int mHalfDeltaHeight;

	private GameSelectorScene _scene;

	private GameSelectorScene _lastScene;

	public GameSelectorScene mScene
	{
		get
		{
			return _scene;
		}
		set
		{
			_lastScene = _scene;
			_scene = value;
		}
	}

	public GameSelectorScene mLastScene => _lastScene;

	public GameSelector(LawnApp theApp)
	{
		mApp = theApp;
		mClip = false;
		mLevel = 1;
		mLoading = false;
		mHasTrophy = false;
		mDoNewGameAfterStore = false;
		mInUserDialog = false;
		mScene = GameSelectorScene.Main;
		InitMainButton();
		mFadeInCounter = 0;
		InitMainSmallButton();
		InitMoreGamesButton();
		InitPuzzleButton();
		mMoreWaysBackButton = MainMenuButton.MakeNewButton(102, this, null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS_BACK, 5, 10);
		mMiniGamesButton.mTranslateX = -1;
		mMoreGamesListWidget = new MoreGamesListWidget(mApp);
		float num = 190f;
		mChallengePageLimboButton = GameButton.MakeButton(118, this, "QUICK");
		mChallengePageLimboButton.Resize(Constants.MORE_GAMES_ORIGIN_X + (int)Constants.InvertAndScale(80f), (int)Constants.InvertAndScale(num + 30f), (int)Constants.InvertAndScale(100f), (int)Constants.InvertAndScale(30f));
		LoadGames();
		mMoreGamesListWidget.Resize(0, 0, mMoreGamesListWidget.mWidth, mMoreGamesListWidget.GetPreferredHeight() + (int)Constants.InvertAndScale(20f));
		mMoreGamesScrollWidget = new ScrollWidget();
		mMoreGamesScrollWidget.Resize(0, 0, mMoreGamesListWidget.mWidth, Constants.BOARD_HEIGHT);
		mMoreGamesScrollWidget.AddWidget(mMoreGamesListWidget);
		mMoreGamesScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mAchievementsWidget = new AchievementsWidget(mApp);
		mAchievementsScrollWidget = new ScrollWidget();
		mAchievementsScrollWidget.EnableBounce(enable: false);
		mAchievementsScrollWidget.Resize(Constants.ACHIEVEMENTS_ORIGIN_X, Constants.BOARD_HEIGHT - Constants.GameSelector_TransY, mAchievementsWidget.mWidth, Constants.BOARD_HEIGHT);
		mAchievementsScrollWidget.AddWidget(mAchievementsWidget);
		mQuickplayWidget = new QuickPlayWidget(mApp, this);
		mQuickplayScrollWidget = new ScrollWidget();
		mQuickplayScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.Horizontal);
		mQuickplayWidget.SizeToFit();
		mQuickplayScrollWidget.Resize(Constants.QUICKPLAY_ORIGIN_X + 30, 0, Constants.BOARD_WIDTH, mQuickplayWidget.mHeight);
		mQuickplayScrollWidget.AddWidget(mQuickplayWidget);
		mQuickplayScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mMiniGamesWidget = new MiniGamesWidget(mApp, this);
		mMiniGamesWidget.SizeToFit();
		mMiniGamesScrollWidget = new ScrollWidget();
		mMiniGamesScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.Horizontal);
		mMiniGamesScrollWidget.AddWidget(mMiniGamesWidget);
		mMiniGamesScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mMiniGamesScrollWidget.AddWidget(mQuickplayWidget);
		mRetractingQuickplay = false;
		mQuickplaySlideCounter = 0;
		mNeedToPlayRollIn = false;
		AddWidget(mUnlockButton);
		AddWidget(mAdventureHardButton);
		AddWidget(mAdventureButton);
		AddWidget(mMoreWaysToPlayButton);
		if (mApp.mEdition.HasComic)
		{
			AddWidget(mComicButton);
		}
		AddWidget(mFumeshroomGnomeButton);
		AddWidget(mOptionsButton);
		AddWidget(mStoreButton);
		AddWidget(mAlmanacButton);
		AddWidget(mZenGardenButton);
		AddWidget(mCommunityButton);
		if (mApp.mEdition.HasOnlineLevels)
		{
			AddWidget(mOnlineLevelsButton);
		}
		AddWidget(mExitButton);
		AddWidget(mTrophyButton);
		if (mApp.mPlayerInfo != null && mApp.mPlayerInfo.mHasUnlockedPuzzleMode)
		{
			AddWidget(mVaseBreakerButton);
			AddWidget(mIZombieButton);
			AddWidget(mMiscButton);
		}
		AddWidget(mMiniGamesButton);
		AddWidget(mPuzzleButton);
		AddWidget(mSurvivalButton);
		AddWidget(mCreativeLevelButton);
		AddWidget(mAchievementsScrollWidget);
		AddWidget(mMoreWaysBackButton);
		AddWidget(mMiniGamesScrollWidget);
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.TitleCrazyDaveMainTheme);
		mQuickplayLocked = false;
		mUnlockSelectorCheat = false;
		mTrophyParticleID = null;
		mShowStartButton = false;
		AddWidget(mUserDialogButton);
		mLeafCounter = 50;
		mSignState = SelectorSignState.Up;
		SyncProfile(theShowLoading: false);
		LowerSign();
		SwitchToMain();
		SyncAllHidableButton();
		RemoveWidget(mMiniGamesScrollWidget);
		RemoveWidget(mQuickplayScrollWidget);
	}

	public void SetButtonAvailable(DialogButton button, bool available, bool preserveVisibility = false)
	{
		button.mVisible = available || (preserveVisibility && button.mVisible);
		button.mDisabled = !available;
		if (button is ToggleButton toggleButton)
		{
			toggleButton.SetToggled(toggled: false);
		}
	}

	public void SyncButtonVisible(DialogButton button)
	{
		button.mVisible = !button.mDisabled;
	}

	public void HideAllHidableButton()
	{
		SetButtonAvailable(mMoreWaysBackButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mCreativeLevelButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mMiniGamesButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mPuzzleButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mSurvivalButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mVaseBreakerButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mMiscButton, available: false, preserveVisibility: true);
		SetButtonAvailable(mIZombieButton, available: false, preserveVisibility: true);
		mAchievementsScrollWidget.SetDisabled(isDisabled: true);
	}

	public void SyncAllHidableButton()
	{
		SyncButtonVisible(mMoreWaysBackButton);
		SyncButtonVisible(mMiniGamesButton);
		SyncButtonVisible(mPuzzleButton);
		SyncButtonVisible(mSurvivalButton);
		SyncButtonVisible(mCreativeLevelButton);
		SyncButtonVisible(mVaseBreakerButton);
		SyncButtonVisible(mMiscButton);
		SyncButtonVisible(mIZombieButton);
	}

	public void SwitchToMoreGames(bool isInstant = false)
	{
		HideAllHidableButton();
		SetButtonAvailable(mMoreWaysBackButton, available: true);
		SetButtonAvailable(mMiniGamesButton, available: true);
		SetButtonAvailable(mPuzzleButton, available: true);
		SetButtonAvailable(mSurvivalButton, available: true);
		SetButtonAvailable(mCreativeLevelButton, available: true);
		SlideTo(-Constants.MORE_GAMES_ORIGIN_X, 0);
		RetractQuickPlayWidget();
		mScene = GameSelectorScene.MoreGames;
		if (isInstant)
		{
			mSlideCounter = 1;
		}
	}

	public void SwitchToMain()
	{
		HideAllHidableButton();
		SlideTo(-Constants.MAIN_MENU_ORIGIN_X, 0);
		mScene = GameSelectorScene.Main;
	}

	public void SwitchToCreativeLevel()
	{
		HideAllHidableButton();
		SetButtonAvailable(mMoreWaysBackButton, available: true);
		AddWidget(mMiniGamesScrollWidget);
		mMiniGamesWidget.mMode = MiniGameMode.Extra;
		mMiniGamesWidget.SizeToFit();
		SlideTo(-Constants.QUICKPLAY_ORIGIN_X, 0);
		SlideOutQuickPlayWidget();
		mScene = GameSelectorScene.CreativeLevel;
	}

	public void PuzzleSetPosition(GameSelectorButtons theId)
	{
		Debug.ASSERT(mScene == GameSelectorScene.Puzzle);
		if (mSelectedQuickplayButtonId != (int)theId)
		{
			SetButtonAvailable(mIZombieButton, available: true);
			SetButtonAvailable(mMiscButton, available: true);
			SetButtonAvailable(mVaseBreakerButton, available: true);
			mSelectedQuickplayButtonId = (int)theId;
			if (mSlideCounter == 0)
			{
				RetractQuickPlayWidget();
			}
			mQuickplayWidget.Clear();
			mQuickplayWidget.SizeToFit();
			mMiniGamesWidget.SizeToFit();
			switch (theId)
			{
			case GameSelectorButtons.Vasebreaker:
				mVaseBreakerButton.SetToggled(toggled: true);
				break;
			case GameSelectorButtons.IZombie:
				mIZombieButton.SetToggled(toggled: true);
				break;
			case GameSelectorButtons.Misc:
				mMiscButton.SetToggled(toggled: true);
				break;
			}
		}
	}

	public void SwitchToPuzzle()
	{
		if (!mApp.mPlayerInfo.mHasUnlockedPuzzleMode)
		{
			mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[PUZZLE_LOCKED_MESSAGE]", "[DIALOG_BUTTON_OK]", "", 3, null);
			return;
		}
		HideAllHidableButton();
		SetButtonAvailable(mMoreWaysBackButton, available: true);
		SetButtonAvailable(mVaseBreakerButton, available: true);
		SetButtonAvailable(mMiscButton, available: true);
		SetButtonAvailable(mIZombieButton, available: true);
		mSelectedQuickplayButtonId = -1;
		AddWidget(mMiniGamesScrollWidget);
		mMiniGamesWidget.mMode = MiniGameMode.IZombie;
		mMiniGamesWidget.SizeToFit();
		SlideTo(-Constants.QUICKPLAY_ORIGIN_X, 0);
		SlideOutQuickPlayWidget();
		mScene = GameSelectorScene.Puzzle;
	}

	public void SwitchToMiniGames()
	{
		HideAllHidableButton();
		SetButtonAvailable(mMoreWaysBackButton, available: true);
		AddWidget(mMiniGamesScrollWidget);
		mMiniGamesWidget.mMode = MiniGameMode.Games;
		mMiniGamesWidget.SizeToFit();
		SlideTo(-Constants.QUICKPLAY_ORIGIN_X, 0);
		SlideOutQuickPlayWidget();
		mScene = GameSelectorScene.MiniGame;
	}

	public void SwitchToSurvival()
	{
		if (!mApp.HasFinishedAdventure())
		{
			mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[SURVIVAL_LOCKED_MESSAGE]", "[DIALOG_BUTTON_OK]", "", 3, null);
			return;
		}
		HideAllHidableButton();
		SetButtonAvailable(mMoreWaysBackButton, available: true);
		AddWidget(mMiniGamesScrollWidget);
		mMiniGamesWidget.mMode = MiniGameMode.Survival;
		mMiniGamesWidget.SizeToFit();
		SlideTo(-Constants.QUICKPLAY_ORIGIN_X, 0);
		SlideOutQuickPlayWidget();
		mScene = GameSelectorScene.Survival;
	}

	public void UpdateSmallButtonPosition()
	{
		if (mZenGardenButton != null)
		{
			mZenGardenButton.mX = Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_ZenGardenButton_X - (int)(63f * Constants.S) + Math.Max(0, -mHalfDeltaWidth);
		}
		mUserDialogButton.Resize(Constants.MAIN_MENU_ORIGIN_X - mHalfDeltaWidth + 40, 20, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight);
		mCommunityButton.Resize(Constants.MAIN_MENU_ORIGIN_X - mHalfDeltaWidth + 40, 100, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight);
		mOnlineLevelsButton.Resize(Constants.MAIN_MENU_ORIGIN_X - mHalfDeltaWidth + 40, 180, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight);
		mExitButton.Resize(Constants.MAIN_MENU_ORIGIN_X + mWidgetManager.mWidth + mHalfDeltaWidth - 40 - mExitButton.mWidth, 20, mExitButton.mWidth, mExitButton.mHeight);
		mOptionsButton.Resize(Constants.MAIN_MENU_ORIGIN_X - mHalfDeltaWidth + 40, Constants.BackBufferSize.X - 80, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight);
		mStoreButton.Resize(Constants.MAIN_MENU_ORIGIN_X - mHalfDeltaWidth + 200, Constants.BackBufferSize.X - 80, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight);
		mAlmanacButton.Resize(Constants.MAIN_MENU_ORIGIN_X - mHalfDeltaWidth + 120, Constants.BackBufferSize.X - 80, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight);
	}

	public void InitMainSmallButton()
	{
		mUserDialogButton = MainMenuButton.MakeNewButton(116, this, "用户存档", null, AtlasResources.IMAGE_MENUSTYLEFREE_USER, -3, 6);
		mOptionsButton = MainMenuButton.MakeNewButton(103, this, null, AtlasResources.IMAGE_MENUSTYLEFREE_HELP, -1, -30);
		mStoreButton = MainMenuButton.MakeNewButton(106, this, null, AtlasResources.IMAGE_MENUSTYLEFREE_SHOP, 15, -5);
		mAlmanacButton = MainMenuButton.MakeNewButton(108, this, null, AtlasResources.IMAGE_MENUSTYLEFREE_ALMANAC, 7, 3);
		mCommunityButton = MainMenuButton.MakeNewButton(126, this, "作者主页", null, AtlasResources.IMAGE_MENUSTYLEFREE_COMMUNITY, 7, 3);
		mOnlineLevelsButton = DockMainMenuButton.MakeNewButton(130, this, "在线关卡", null, AtlasResources.IMAGE_MENUSTYLEFREE_ONLINE_LEVELS, 7, -5);
		mOnlineLevelsButton.ExpandDirection = DockDirection.Right;
		mOnlineLevelsButton.HoldDuration = TimeSpan.FromSeconds(0.5);
		mOnlineLevelsButton.AnimationDuration = TimeSpan.FromMilliseconds(300.0);
		mOnlineLevelsButton.AddItem(MainMenuButton.MakeNewButton(132, this, "本地关卡", null, AtlasResources.IMAGE_MENUSTYLEFREE_ONLINE_LEVELS, 7, -5));
		mExitButton = GameButton.MakeNewButton(105, this, "", null, AtlasResources.IMAGE_BUTTON_QUIT, AtlasResources.IMAGE_BUTTON_QUIT_LIGHT, AtlasResources.IMAGE_BUTTON_QUIT_LIGHT);
	}

	public void InitMainButton()
	{
		mAdventureButton = GameButton.MakeNewButton(100, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ADVENTURE_BUTTON, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ADVENTURE_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ADVENTURE_HIGHLIGHT);
		mAdventureButton.Resize(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_AdventureButton_X, (int)(90f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ADVENTURE_BUTTON.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ADVENTURE_BUTTON.mHeight);
		mAdventureButton.mClip = false;
		mMoreWaysToPlayButton = GameButton.MakeNewButton(101, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS_HIGHLIGHT);
		mMoreWaysToPlayButton.Resize(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_MiniGameButton_X, Constants.GameSelector_MiniGameButton_Y + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS.mHeight);
		mMoreWaysToPlayButton.mClip = false;
		mUnlockButton = GameButton.MakeNewButton(125, this, "", null, AtlasResources.IMAGE_BUTTON_UNLOCK, AtlasResources.IMAGE_BUTTON_UNLOCK_LIT, AtlasResources.IMAGE_BUTTON_UNLOCK_LIT);
		mUnlockButton.Resize(Constants.MAIN_MENU_ORIGIN_X + (int)(750f * Constants.S), (int)(315f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_BUTTON_UNLOCK.mWidth + 2, AtlasResources.IMAGE_BUTTON_UNLOCK.mHeight + 2);
		mAdventureHardButton = ToggleButton.MakeNewButton(131, this, AtlasResources.IMAGE_BUTTON_ADVENTURE_HARD, AtlasResources.IMAGE_BUTTON_ADVENTURE_HARD_ACTIVATED);
		mAdventureHardButton.Resize(mUnlockButton.mX + 40, mUnlockButton.mY - mAdventureHardButton.mHeight - 2 - (int)(10f * Constants.S), mAdventureHardButton.mWidth + 2, mAdventureHardButton.mHeight + 2);
		mZenGardenButton = GameButton.MakeNewButton(115, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ZENGARDEN, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ZENGARDEN_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ZENGARDEN_HIGHLIGHT);
		mZenGardenButton.Resize(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_ZenGardenButton_X - (int)(63f * Constants.S), Constants.GameSelector_ZenGardenButton_Y + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ZENGARDEN.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_ZENGARDEN.mHeight);
		mZenGardenButton.mClip = false;
		mComicButton = GameButton.MakeNewButton(124, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_COMIC, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_COMIC_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_COMIC_HIGHLIGHT);
		mComicButton.Resize(Constants.MAIN_MENU_ORIGIN_X + (int)(248f * Constants.S), (int)(354f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_COMIC.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_COMIC.mHeight);
		mFumeshroomGnomeButton = GameButton.MakeNewButton(129, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME_HIGHLIGHT);
		mFumeshroomGnomeButton.Resize(Constants.MAIN_MENU_ORIGIN_X + (int)(448f * Constants.S), (int)(400f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME.mHeight);
		mTrophyButton = GameButton.MakeNewButton(128, this, "", null, Resources.IMAGE_SUNFLOWER_TROPHY_1, null, null);
		mTrophyButton.Resize(Constants.MAIN_MENU_ORIGIN_X + (int)(813f * Constants.S), (int)(316f * Constants.S) + Constants.GameSelector_TransY, Resources.IMAGE_SUNFLOWER_TROPHY_1.mWidth, Resources.IMAGE_SUNFLOWER_TROPHY_1.mHeight);
		mTrophyButton.mClip = false;
	}

	public void InitMoreGamesButton()
	{
		mMiniGamesButton = GameButton.MakeNewButton(111, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MINIGAMES, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MINIGAMES_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MINIGAMES_HIGHLIGHT);
		mMiniGamesButton.Resize(Constants.MORE_GAMES_ORIGIN_X + Constants.GameSelector_MoreWaysToPlay_MiniGames_X, Constants.GameSelector_MoreWaysToPlay_MiniGames_Y + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MINIGAMES.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MINIGAMES.mHeight + 2);
		mPuzzleButton = GameButton.MakeNewButton(120, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_PUZZLE, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_PUZZLE_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_PUZZLE_HIGHLIGHT);
		mPuzzleButton.Resize(Constants.MORE_GAMES_ORIGIN_X + (int)(180f * Constants.S), (int)(279f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_PUZZLE.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_PUZZLE.mHeight + 2);
		mSurvivalButton = GameButton.MakeNewButton(119, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_SURVIVAL, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_SURVIVAL_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_SURVIVAL_HIGHLIGHT);
		mSurvivalButton.Resize(Constants.MORE_GAMES_ORIGIN_X + (int)(656f * Constants.S), (int)(244f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_SURVIVAL.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_SURVIVAL.mHeight + 2);
		mCreativeLevelButton = GameButton.MakeNewButton(121, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_QUICKPLAY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_QUICKPLAY_HIGHLIGHT, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_QUICKPLAY_HIGHLIGHT);
		mCreativeLevelButton.Resize(Constants.MORE_GAMES_ORIGIN_X + (int)(669f * Constants.S), (int)(363f * Constants.S) + Constants.GameSelector_TransY, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_QUICKPLAY.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_QUICKPLAY.mHeight + 2);
	}

	public void InitPuzzleButton()
	{
		mVaseBreakerButton = ToggleButton.MakeNewButton(112, this, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_VASEBREAKER, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_VASEBREAKER_HIGHLIGHT);
		mVaseBreakerButton.Resize(Constants.QUICKPLAY_ORIGIN_X + (int)(598f * Constants.S), (int)(379f * Constants.S), AtlasResources.IMAGE_REANIM_SELECTORSCREEN_VASEBREAKER.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_VASEBREAKER.mHeight + 2);
		mMiscButton = ToggleButton.MakeNewButton(114, this, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MISC, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MISC_HIGHLIGHT);
		mMiscButton.Resize(Constants.QUICKPLAY_ORIGIN_X + (int)(473f * Constants.S), (int)(340f * Constants.S), AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MISC.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MISC.mHeight + 2);
		mIZombieButton = ToggleButton.MakeNewButton(113, this, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_IZOMBIE, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_IZOMBIE_HIGHLIGHT);
		mIZombieButton.Resize(Constants.QUICKPLAY_ORIGIN_X + (int)(336f * Constants.S), (int)(379f * Constants.S), AtlasResources.IMAGE_REANIM_SELECTORSCREEN_IZOMBIE.mWidth + 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_IZOMBIE.mHeight + 2);
	}

	private void SetupUnlockFullGameReanim()
	{
		if (mWoodSignReanimID != null)
		{
			mWoodSignReanimID.AssignRenderGroupToPrefix("short rope1", -1);
			mWoodSignReanimID.AssignRenderGroupToPrefix("short rope2", -1);
		}
	}

	public override void Dispose()
	{
		RemoveAllWidgets(doDelete: true, recursive: true);
	}

	public void SyncProfile(bool theShowLoading)
	{
		if (theShowLoading)
		{
			mLoading = true;
			mApp.PreloadForUser();
			mLoading = false;
		}
		TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(mTrophyParticleID);
		if (todParticleSystem != null)
		{
			todParticleSystem.ParticleSystemDie();
			mTrophyParticleID = null;
		}
		mLevel = 1;
		if (mApp.mPlayerInfo != null)
		{
			mLevel = mApp.mPlayerInfo.mLevel;
		}
		mShowStartButton = true;
		mQuickplayLocked = true;
		if (mApp.mPlayerInfo != null && !mApp.IsIceDemo())
		{
			if (mLevel >= 2 || mApp.HasFinishedAdventure())
			{
				mShowStartButton = false;
			}
			if (mApp.mPlayerInfo.mHasUnlockedMinigames)
			{
				mQuickplayLocked = false;
			}
		}
		if (mApp.HasFinishedAdventure() && !mApp.IsTrialStageLocked())
		{
			mHasTrophy = true;
		}
		else
		{
			mHasTrophy = false;
		}
		if (mHasTrophy)
		{
			AddTrophySparkle();
		}
		SyncButtons();
		AlmanacDialog.AlmanacInitForPlayer();
		Board.BoardInitForPlayer();
	}

	public override void Draw(Graphics g)
	{
		g.PushState();
		g.Translate(0, Constants.GameSelector_TransY);
		if (mApp.GetDialog(Dialogs.DIALOG_STORE) == null && mApp.GetDialog(Dialogs.DIALOG_ALMANAC) == null)
		{
			DrawScreen2(g);
			DrawMainMenuArea(g);
			DrawScreen3(g);
			DeferOverlay();
			g.PopState();
		}
	}

	public override void DrawOverlay(Graphics g)
	{
		g.PushState();
		g.Translate(0, Constants.GameSelector_TransY);
		if (mApp.GetDialog(Dialogs.DIALOG_STORE) != null || mApp.GetDialog(Dialogs.DIALOG_ALMANAC) != null)
		{
			return;
		}
		g.SetLinearBlend(linear: true);
		if (mApp.mPlayerInfo == null)
		{
			return;
		}
		if (!mApp.IsIceDemo() && (!mShowStartButton || mApp.HasFinishedAdventure()))
		{
			bool mAdventureModeHard = mApp.mPlayerInfo.mAdventureModeHard;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = TodCommon.ClampInt((mLevel - 1) / 10 + 1, 1, 6);
			int num6 = mLevel - (num5 - 1) * 10;
			if (num5 == 1)
			{
				num2++;
			}
			if (num5 == 4)
			{
				num--;
			}
			if (num6 == 3)
			{
				num3--;
			}
			if (mAdventureButton.mIsDown || mAdventureButton.mIsOver)
			{
				int num7 = Constants.GameSelector_LevelNumber_ButtonDown_Offset;
				if (!mAdventureButton.mIsDown || !mAdventureButton.mIsOver)
				{
					num7--;
				}
				num += (float)num7;
				num3 += (float)num7;
				num2 += (float)num7;
				num4 += (float)num7;
			}
			g.SetColorizeImages(colorizeImages: true);
			SexyColor sexyColor = (mAdventureModeHard ? new SexyColor(200, 30, 30) : ((SexyColor)mAdventureButton.mColors[5]));
			g.SetColor(sexyColor);
			TodCommon.TodDrawImageCelF(g, AtlasResources.IMAGE_SELECTORSCREEN_LEVELNUMBERS, (float)(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_LevelNumber_1_Pos.X) + num, (float)Constants.GameSelector_LevelNumber_1_Pos.Y + num2, num5, 0);
			if (num6 < 10)
			{
				TodCommon.TodDrawImageCelF(g, AtlasResources.IMAGE_SELECTORSCREEN_LEVELNUMBERS, (float)(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_LevelNumber_2_Pos.X) + num3, (float)Constants.GameSelector_LevelNumber_2_Pos.Y + num4, num6, 0);
			}
			else if (num6 == 10)
			{
				TodCommon.TodDrawImageCelF(g, AtlasResources.IMAGE_SELECTORSCREEN_LEVELNUMBERS, (float)(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_LevelNumber_2_Pos.X) + num3, (float)Constants.GameSelector_LevelNumber_2_Pos.Y + num4, 1, 0);
				TodCommon.TodDrawImageCelF(g, AtlasResources.IMAGE_SELECTORSCREEN_LEVELNUMBERS, (float)(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_LevelNumber_3_Pos.X) + num3, (float)Constants.GameSelector_LevelNumber_3_Pos.Y + num4, 0, 0);
			}
			g.SetColor(mAdventureModeHard ? new SexyColor(200, 30, 30) : new SexyColor(255, 255, 255));
			g.FillRect(Constants.MAIN_MENU_ORIGIN_X + Constants.GameSelector_LevelNumber_Bar.X + (int)num, Constants.GameSelector_LevelNumber_Bar.Y + (int)num2, Constants.GameSelector_LevelNumber_Bar.Width, Constants.GameSelector_LevelNumber_Bar.Height);
			g.SetColorizeImages(colorizeImages: false);
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mWoodSignReanimID);
		if (reanimation != null)
		{
			g.Translate(Constants.MAIN_MENU_ORIGIN_X, 0);
			reanimation.Draw(g);
			g.Translate(-Constants.MAIN_MENU_ORIGIN_X, 0);
			if (mApp.mPlayerInfo != null && !string.IsNullOrEmpty(mApp.mPlayerInfo.mName) && (mSignState == SelectorSignState.Down || mSignState == SelectorSignState.MovingDown))
			{
				string welcomeString = GetWelcomeString();
				int theTrackIndex = reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_welcome);
				SexyTransform2D theOverlayMatrix = default;
				reanimation.GetAttachmentOverlayMatrix(theTrackIndex, out theOverlayMatrix);
				float num8 = Resources.FONT_BRIANNETOD16.StringWidth(welcomeString);
				SexyTransform2D sexyTransform2D = default;
				sexyTransform2D.Translate(Constants.GameSelector_PlayerName_Pos.X - (int)(num8 * 0.5f) + Constants.MAIN_MENU_ORIGIN_X + mX, Constants.GameSelector_PlayerName_Pos.Y + mY);
				sexyTransform2D.Translate(-g.mTransX, -g.mTransY);
				TodCommon.TodDrawStringMatrix(g, Resources.FONT_BRIANNETOD16, theOverlayMatrix.mMatrix * sexyTransform2D.mMatrix, welcomeString, new SexyColor(255, 245, 200));
			}
			g.SetColor(new SexyColor(115, 101, 66));
			g.SetColorizeImages(colorizeImages: true);
			int num9 = mMoreGamesListWidget.mY + mMoreGamesListWidget.mHeight;
			int num10 = -mMoreGamesListWidget.mY + mMoreGamesScrollWidget.mHeight - 52;
			if (mMoreGamesListWidget.mY < -48)
			{
				int num11 = Constants.MORE_GAMES_PLANK_HEIGHT + Constants.MORE_GAMES_ITEM_GAP;
				int num12 = (num10 + num11 / 2) / num11 * num11;
				int theTimeAge = Math.Abs(num12 - num10);
				int theAlpha = TodCommon.TodAnimateCurve(0, 35, theTimeAge, 0, 220, TodCurves.Linear);
				g.SetColor(new SexyColor(115, 101, 66, theAlpha));
			}
			if (num9 > 340)
			{
				int num13 = Constants.MORE_GAMES_PLANK_HEIGHT + Constants.MORE_GAMES_ITEM_GAP;
				int num14 = (num10 + num13 / 2) / num13 * num13;
				int theTimeAge2 = Math.Abs(num14 - num10);
				int theAlpha2 = TodCommon.TodAnimateCurve(0, 35, theTimeAge2, 0, 220, TodCurves.Linear);
				g.SetColor(new SexyColor(115, 101, 66, theAlpha2));
			}
			g.SetColorizeImages(colorizeImages: false);
			g.Translate(-(Constants.QUICKPLAY_ORIGIN_X + mX), 0);
			g.PopState();
		}
	}

	private string GetWelcomeString()
	{
		if (cachedWelcomeString == null || cachedWelcomeStringName != mApp.mPlayerInfo.mName)
		{
			cachedWelcomeString = Common.StrFormat_(TodStringFile.TodStringTranslate("[WELCOME_USER_NAME]"), mApp.mPlayerInfo.mName);
			cachedWelcomeStringName = mApp.mPlayerInfo.mName;
		}
		return cachedWelcomeString;
	}

	public void DrawMainMenuArea(Graphics g)
	{
		g.SetLinearBlend(linear: true);
		g.DrawImage(Resources.IMAGE_SELECTORSCREEN_MAIN_BACKGROUND, Constants.MAIN_MENU_ORIGIN_X - 200, 0);
		g.SetColor(SexyColor.White);
		g.SetFont(Resources.FONT_PICO129);
	}

	public void DrawScreen3(Graphics g)
	{
		g.DrawImage(Resources.IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND, Constants.QUICKPLAY_ORIGIN_X - (int)(200f * Constants.S), 0);
		if (mScene == GameSelectorScene.Puzzle || (mLastScene == GameSelectorScene.Puzzle && mSlideCounter != 0))
		{
			g.DrawImage(Resources.IMAGE_SELECTORSCREEN_PUZZLE_BACKGROUND, Constants.QUICKPLAY_ORIGIN_X - (int)(171f * Constants.S), (int)(325f * Constants.S));
		}
		else if (mScene == GameSelectorScene.MiniGame || mScene == GameSelectorScene.Survival || mScene == GameSelectorScene.CreativeLevel || ((mLastScene == GameSelectorScene.MiniGame || mLastScene == GameSelectorScene.Survival || mLastScene == GameSelectorScene.CreativeLevel) && mSlideCounter != 0))
		{
			g.DrawImage(Resources.IMAGE_SELECTORSCREEN_MINIGAME_BACKGROUND, Constants.QUICKPLAY_ORIGIN_X - (int)(171f * Constants.S), (int)(325f * Constants.S));
		}
		g.DrawImage(AtlasResources.IMAGE_TROPHY, Constants.QUICKPLAY_ORIGIN_X + Constants.BOARD_WIDTH - (int)(194f * Constants.S), Constants.BOARD_HEIGHT - (int)(63f * Constants.S));
		int num = mApp.GetNumTrophies(ChallengePage.Challenge) + mApp.GetNumTrophies(ChallengePage.Survival) + mApp.GetNumTrophies(ChallengePage.Puzzle) + mApp.GetNumTrophies(ChallengePage.Extra);
		int num2 = 55;
		if (cachedTrophyStringCount != num)
		{
			cachedTrophyString = num.ToString();
			cachedTrophyStringCount = num;
		}
		g.SetFont(Resources.FONT_DWARVENTODCRAFT18);
		g.SetColor(new Color(224, 187, 98));
		g.DrawString(cachedTrophyString, Constants.QUICKPLAY_ORIGIN_X + Constants.BOARD_WIDTH - (int)(113f * Constants.S), Constants.BOARD_HEIGHT - (int)(63f * Constants.S));
	}

	public void DrawScreen2(Graphics g)
	{
		g.DrawImage(Resources.IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND, Constants.MORE_GAMES_ORIGIN_X - (int)(200f * Constants.S), 0);
		g.DrawImage(AtlasResources.IMAGE_TROPHY, Constants.MORE_GAMES_ORIGIN_X + Constants.BOARD_WIDTH - (int)(194f * Constants.S), Constants.BOARD_HEIGHT - (int)(63f * Constants.S));
		int num = mApp.GetNumTrophies(ChallengePage.Challenge) + mApp.GetNumTrophies(ChallengePage.Survival) + mApp.GetNumTrophies(ChallengePage.Puzzle) + mApp.GetNumTrophies(ChallengePage.Extra);
		int num2 = 55;
		if (cachedTrophyStringCount != num)
		{
			cachedTrophyString = num.ToString();
			cachedTrophyStringCount = num;
		}
		g.SetFont(Resources.FONT_DWARVENTODCRAFT18);
		g.SetColor(new Color(224, 187, 98));
		g.DrawString(cachedTrophyString, Constants.MORE_GAMES_ORIGIN_X + Constants.BOARD_WIDTH - (int)(113f * Constants.S), Constants.BOARD_HEIGHT - (int)(63f * Constants.S));
	}

	public void RaiseSign()
	{
		mSignState = SelectorSignState.MovingUp;
		mNeedToPlayRollIn = false;
	}

	public void LowerSign()
	{
		Reanimation reanimation = mApp.ReanimationGet(mWoodSignReanimID);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_drop, ReanimLoopType.PlayOnceAndHold, 0, 10f);
		mSignState = SelectorSignState.MovingDown;
		mNeedToPlayRollIn = true;
		woodSignY = -30f;
	}

	public override bool BackButtonPress()
	{
		if (mScene == GameSelectorScene.Main)
		{
			mApp.WantsToExit = true;
		}
		else
		{
			ButtonPress(mMoreWaysBackButton.mId);
			ButtonDepress(mMoreWaysBackButton.mId);
		}
		return true;
	}

	public void UpdateScreen()
	{
		Resize(mX, mY, 3360, 960);
		mHalfDeltaWidth = (mApp.mScreenScales.mVirtualWidth - Constants.BOARD_WIDTH) / 2;
		mHalfDeltaHeight = (mApp.mScreenScales.mVirtualHeight - Constants.BOARD_HEIGHT) / 2;
		FullRect = new TRect(-mX - mHalfDeltaWidth, mY - mHalfDeltaHeight, mWidth, mHeight);
		UpdateUIPosition();
	}

	public void UpdateUIPosition()
	{
		UpdateSmallButtonPosition();
		mMiniGamesScrollWidget.Resize(Constants.QUICKPLAY_ORIGIN_X - mHalfDeltaWidth, 10, mApp.mScreenScales.mVirtualWidth, mMiniGamesWidget.mHeight);
	}

	public const int WARDROBE_BUTTON_ID = 9070;

	private MainMenuButton mWardrobeButton;

	/// <summary>Mod edition: 「时装柜」 button next to the store / almanac on the main menu.</summary>
	private void UpdateWardrobeButton()
	{
		if (!LawnApp.ModBuild || Resources.FONT_BRIANNETOD12 == null)
		{
			return;
		}
		Image img = AtlasResources.IMAGE_MENUSTYLEFREE_USER ?? AtlasResources.IMAGE_MENUSTYLEFREE_ALMANAC;
		if (img == null)
		{
			return;
		}
		if (mWardrobeButton == null)
		{
			mWardrobeButton = MainMenuButton.MakeNewButton(WARDROBE_BUTTON_ID, this, "时装柜", Resources.FONT_BRIANNETOD12, img, 0, 5);
			mWardrobeButton.mClip = false;
			AddWidget(mWardrobeButton);
		}
		DialogButton anchor = mStoreButton ?? mAlmanacButton;
		int step = 80;
		if (mAlmanacButton != null && mStoreButton != null && mStoreButton.mX > mAlmanacButton.mX)
		{
			step = mStoreButton.mX - mAlmanacButton.mX;
		}
		int x = (anchor?.mX ?? 40) + step;
		int y = anchor?.mY ?? Math.Max(0, mHeight - 80);
		if (x + mWardrobeButton.mWidth > mWidth - 8 && anchor != null)
		{
			x = anchor.mX;
			y = anchor.mY - mWardrobeButton.mHeight - 6;
		}
		mWardrobeButton.Resize(x, y, mWardrobeButton.mWidth, mWardrobeButton.mHeight);
		mWardrobeButton.SetVisible(mScene == GameSelectorScene.Main && mApp.GetDialog(WardrobeDialog.DIALOG_ID) == null);
	}

	public override void Update()
	{
		base.Update();
		UpdateWardrobeButton();
		UpdateScreen();
		if (wantToExitFromAchievementsViaBackButton && mAchievementsScrollWidget.GetScrollOffset().y > -5f)
		{
			mAchievementsScrollWidget.SetScrollOffset(0f, 0f);
			mAchievementsScrollWidget.mSpringOverride = 0f;
			ButtonPress(102);
			ButtonDepress(102);
			wantToExitFromAchievementsViaBackButton = false;
		}
		if (mApp.GetDialog(Dialogs.DIALOG_STORE) != null || mApp.GetDialog(Dialogs.DIALOG_ALMANAC) != null)
		{
			return;
		}
		MarkDirty();
		if (mSlideCounter > 0)
		{
			int num = TodCommon.TodAnimateCurve(75, 0, mSlideCounter, mStartX, mDestX, TodCurves.EaseInOut);
			int num2 = TodCommon.TodAnimateCurve(75, 0, mSlideCounter, mStartY, mDestY, TodCurves.EaseInOut);
			if (mMoreWaysBackButton.mVisible)
			{
				int num3 = 40;
				if (mScene == GameSelectorScene.Main || mLastScene == GameSelectorScene.Main)
				{
					num3 = TodCommon.TodAnimateCurve(40, 0, (mScene == GameSelectorScene.Main) ? (75 - mSlideCounter) : (mSlideCounter - 75 + 40), -(AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth + 2), 40, TodCurves.EaseOut);
				}
				int theX = -num + num3 - mHalfDeltaWidth;
				int theY = -num2 + Constants.BackBufferSize.X - 80 + mHalfDeltaHeight;
				WidgetManager widgetManager = mMoreWaysBackButton.mWidgetManager;
				mMoreWaysBackButton.mWidgetManager = null;
				mMoreWaysBackButton.Resize(theX, theY, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth + 2, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight + 2);
				mMoreWaysBackButton.mWidgetManager = widgetManager;
			}
			Move(num, num2);
			mSlideCounter--;
			if (mSlideCounter == 0)
			{
				if (mX == -Constants.MAIN_MENU_ORIGIN_X && mY == 0 && mSignState == SelectorSignState.Up)
				{
					LowerSign();
				}
				SyncAllHidableButton();
			}
			else if (mSignState == SelectorSignState.Down && mStartX == -Constants.MAIN_MENU_ORIGIN_X && mDestX == 0)
			{
				RaiseSign();
			}
		}
		if (mMoreWaysBackButton.mVisible && mSlideCounter == 0)
		{
			int num4 = 40;
			if (mScene == GameSelectorScene.Main || mLastScene == GameSelectorScene.Main)
			{
				num4 = TodCommon.TodAnimateCurve(40, 0, (mScene == GameSelectorScene.Main) ? (75 - mSlideCounter) : (mSlideCounter - 75 + 40), -(AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth + 2), 40, TodCurves.EaseOut);
			}
			int theX2 = -mX + num4 - mHalfDeltaWidth;
			int theY2 = -mY + Constants.BackBufferSize.X - 80 + mHalfDeltaHeight;
			mMoreWaysBackButton.Resize(theX2, theY2, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mWidth + 2, AtlasResources.IMAGE_MENUSTYLEFREE_BG.mHeight + 2);
		}
		if (mQuickplaySlideCounter > 0)
		{
			int num5 = 150;
			int theNewY = ((!mRetractingQuickplay) ? TodCommon.TodAnimateCurve(30, 0, mQuickplaySlideCounter, -mMiniGamesScrollWidget.mHeight - num5, 10, TodCurves.EaseInOut) : TodCommon.TodAnimateCurve(30, 0, mQuickplaySlideCounter, 10, -mMiniGamesScrollWidget.mHeight - num5, TodCurves.EaseInOut));
			mQuickplayScrollWidget.Move(mQuickplayScrollWidget.mX, theNewY);
			mMiniGamesScrollWidget.Move(mMiniGamesScrollWidget.mX, theNewY);
			mQuickplaySlideCounter--;
			if (mQuickplaySlideCounter == 0 && mRetractingQuickplay && (mSlideCounter == 0 || mDestX == -Constants.QUICKPLAY_ORIGIN_X))
			{
				PopulateQuickPlayWidget();
				SlideOutQuickPlayWidget();
			}
			else if (mQuickplaySlideCounter == 0)
			{
				if (!mRetractingQuickplay)
				{
					mMiniGamesWidget.RecoverLastPlayedMode();
				}
				else
				{
					RemoveWidget(mQuickplayScrollWidget);
					RemoveWidget(mMiniGamesScrollWidget);
				}
			}
		}
		if (mApp.mZenGarden != null)
		{
			mApp.mZenGarden.UpdatePlantNeeds();
		}
		mApp.ParticleTryToGet(mTrophyParticleID)?.Update();
		if (mSignState == SelectorSignState.Down)
		{
			if (mApp.mPlayerInfo == null)
			{
				mInUserDialog = true;
				mApp.DoCreateUserDialog(isOnlyUser: true);
				RaiseSign();
			}
			else if (mApp.mPlayerInfo?.mSaveStateAppVersionNumber != LawnApp.AppVersionNumber)
			{
				mApp.mPlayerInfo.mSaveStateAppVersionNumber = LawnApp.AppVersionNumber;
				mApp.mPlayerInfo.SaveDetails();
				mApp.DoUpdateDialog();
			}
			if (mHasTrophy)
			{
				AddTrophySparkle();
			}
			if (mApp.mPlayerInfo != null && mApp.mPlayerInfo.mNeedsMessageOnGameSelector)
			{
				mApp.mPlayerInfo.mNeedsMessageOnGameSelector = false;
				mApp.WriteCurrentUserConfig();
				mApp.LawnMessageBox(49, "[ADVENTURE_COMPLETE_HEADER]", "[ADVENTURE_COMPLETE_BODY]", "[DIALOG_BUTTON_OK]", "", 3, null);
			}
		}
		else if (mSignState == SelectorSignState.MovingUp)
		{
			mSignState = SelectorSignState.Up;
		}
		else if (mSignState == SelectorSignState.MovingDown)
		{
			mSignState = SelectorSignState.Down;
		}
		if (mInUserDialog && mApp.GetDialog(30) == null)
		{
			mInUserDialog = false;
			LowerSign();
		}
		if (mNeedToPlayRollIn)
		{
			mApp.PlaySample(Resources.SOUND_ROLL_IN);
			mNeedToPlayRollIn = false;
		}
		if (woodSignY != 0f)
		{
			woodSignY++;
			if (woodSignY > 0f)
			{
				woodSignY = 0f;
			}
		}
		if (mFadeInCounter > 0)
		{
			mFadeInCounter--;
		}
		if (mApp.mPlayerInfo != null && !mInUserDialog && mSignState == SelectorSignState.Down)
		{
			mApp.UpdateVersionCheck();
		}
	}

	public virtual void ButtonMouseEnter(int theId)
	{
		if (!mQuickplayLocked || theId != 101)
		{
			mApp.PlayFoley(FoleyType.Bleep);
		}
	}

	public virtual void ButtonPress(int theId, int theClickCount)
	{
		if (mSlideCounter == 0)
		{
			if (theId == 100 || theId == 101)
			{
				mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_TAP);
			}
		}
	}

	public virtual void ButtonDepress(int theId)
	{
		if (theId == WARDROBE_BUTTON_ID)
		{
			WardrobeDialog.Open(mApp);
			return;
		}
		if (mApp.mPlayerInfo == null || mSlideCounter != 0)
		{
			return;
		}
		if (theId == 101 && mQuickplayLocked)
		{
			mApp.LawnMessageBox(49, "[MODE_LOCKED]", "[QUICKPLAY_LOCKED_MESSAGE]", "[DIALOG_BUTTON_OK]", "", 3, null);
			return;
		}
		switch ((GameSelectorButtons)theId)
		{
		case GameSelectorButtons.Adventure:
			mApp.KillGameSelector();
			ClickedAdventure();
			break;
		case GameSelectorButtons.AdventureHard:
			if (mApp.SaveFileExists())
			{
				mApp.DoDialog(68, isModal: true, "检测到保存的冒险模式游戏", "切换难度将会清除保存的游戏，\n你确定要继续吗？", "", 1);
			}
			else
			{
				ToggleAdventureHard();
			}
			break;
		case GameSelectorButtons.Unlock:
			if (mApp.mPlayerInfo.mFinishedAdventure >= 1)
			{
				UnlockGame();
			}
			else
			{
				mApp.DoDialog(60, isModal: true, "[UNLOCK_ALL]", "[UNLOCK_ALL_WARNING]", "", 1);
			}
			break;
		case GameSelectorButtons.MoreWays:
			SwitchToMoreGames();
			break;
		case GameSelectorButtons.CreativeLevel:
			mApp.KillGameSelector();
			mApp.ShowChallengeScreen(ChallengePage.Extra);
			break;
		case GameSelectorButtons.Comic:
		{
			mApp.FinishModelessDialogs();
			ComicSelector comicSelector = new ComicSelector(mApp, this);
			comicSelector.UpdateScreen();
			mApp.AddDialog(3, comicSelector);
			mWidgetManager.SetFocus(comicSelector);
			mApp.mMusic.StopAllMusic();
			break;
		}
		case GameSelectorButtons.FumeshroomGnome:
			ShowDynamicTachie();
			break;
		case GameSelectorButtons.Community:
			LawnApp.OpenUrl("https://space.bilibili.com/3493090151107069");
			break;
		case GameSelectorButtons.OnlineLevels:
			ShowOnlineLevels();
			break;
		case GameSelectorButtons.LocalLevels:
			ShowLocalLevels();
			break;
		case GameSelectorButtons.QuickplayBack:
			if (mScene == GameSelectorScene.MoreGames)
			{
				SwitchToMain();
			}
			else
			{
				SwitchToMoreGames();
			}
			break;
		case GameSelectorButtons.Options:
			mApp.DoNewOptions(theFromGameSelector: true);
			break;
		case GameSelectorButtons.Help:
			mApp.KillGameSelector();
			mApp.ShowAwardScreen(AwardType.HelpZombieNote, theShowAchievements: false);
			break;
		case GameSelectorButtons.Quit:
			mApp.ConfirmQuit();
			break;
		case GameSelectorButtons.Store:
			mDoNewGameAfterStore = false;
			mApp.ShowStoreScreen(this);
			break;
		case GameSelectorButtons.UnlockGame:
			mApp.BuyGame();
			break;
		case GameSelectorButtons.Almanac:
			mApp.DoAlmanacDialog(SeedType.None, ZombieType.Invalid, this);
			break;
		case GameSelectorButtons.Leaderboards:
			mApp.KillGameSelector();
			mApp.ShowLeaderboardScreen();
			break;
		case GameSelectorButtons.Survival:
			mApp.KillGameSelector();
			mApp.ShowChallengeScreen(ChallengePage.Survival);
			break;
		case GameSelectorButtons.MiniGames:
			mApp.KillGameSelector();
			mApp.ShowChallengeScreen(ChallengePage.Challenge);
			break;
		case GameSelectorButtons.Puzzle:
			mApp.KillGameSelector();
			mApp.ShowChallengeScreen(ChallengePage.Puzzle);
			break;
		case GameSelectorButtons.Vasebreaker:
		case GameSelectorButtons.IZombie:
		case GameSelectorButtons.Misc:
			PuzzleSetPosition((GameSelectorButtons)theId);
			break;
		case GameSelectorButtons.ZenGarden:
			mApp.KillGameSelector();
			mApp.PreNewGame(GameMode.ChallengeZenGarden, theLookForSavedGame: false);
			break;
		case GameSelectorButtons.ChangeUser:
			mApp.DoUserDialog();
			break;
		case GameSelectorButtons.ChallengePageSurvival:
			mApp.KillGameSelector();
			mApp.ShowChallengeScreen(ChallengePage.Survival);
			break;
		case GameSelectorButtons.ChallengePageLimbo:
			mApp.KillGameSelector();
			mApp.ShowChallengeScreen(ChallengePage.Limbo);
			break;
		case GameSelectorButtons.Trophy:
		{
			LawnDialog lawnDialog = mApp.DoDialog(62, isModal: true, string.Empty, mApp.EarnedGoldTrophy() ? mApp.Pluralize(mApp.mPlayerInfo.mFinishedAdventure, "[GOLD_SUNFLOWER_TOOLTIP]", "[GOLD_SUNFLOWER_TOOLTIP_PLURAL]") : "[SILVER_SUNFLOWER_TOOLTIP]", "[BUTTON_OK]", 3);
			lawnDialog.Resize(0, 0, 600, lawnDialog.GetPreferredHeight(600));
			LawnApp.CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
			break;
		}
		case GameSelectorButtons.MiniGameRaw:
		case GameSelectorButtons.MiniGameHidden:
		case GameSelectorButtons.FullScreen:
			break;
		case GameSelectorButtons.UpdateAvailable:
			break;
		}
	}

	public override void KeyDown(KeyCode theKey)
	{
		if (mApp.mKonamiCheck.Check(theKey))
		{
			mApp.PlayFoley(FoleyType.Drop);
		}
		else if (mApp.mMustacheCheck.Check(theKey) || mApp.mMoustacheCheck.Check(theKey))
		{
			mApp.PlayFoley(FoleyType.Polevault);
			mApp.mMustacheMode = !mApp.mMustacheMode;
		}
		else if (mApp.mSuperMowerCheck.Check(theKey) || mApp.mSuperMowerCheck2.Check(theKey))
		{
			mApp.PlayFoley(FoleyType.Zamboni);
			mApp.mSuperMowerMode = !mApp.mSuperMowerMode;
		}
		else if (mApp.mFutureCheck.Check(theKey))
		{
			mApp.PlaySample(Resources.SOUND_BOING);
			mApp.mFutureMode = !mApp.mFutureMode;
		}
		else if (mApp.mPinataCheck.Check(theKey))
		{
			if (mApp.CanDoPinataMode())
			{
				mApp.PlayFoley(FoleyType.Juicy);
				mApp.mPinataMode = !mApp.mPinataMode;
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
			}
		}
		else if (!mApp.mDanceCheck.Check(theKey))
		{
			if (mApp.mDaisyCheck.Check(theKey))
			{
				if (mApp.CanDoDaisyMode())
				{
					mApp.PlaySample(Resources.SOUND_LOADINGBAR_FLOWER);
					mApp.mDaisyMode = !mApp.mDaisyMode;
				}
				else
				{
					mApp.PlaySample(Resources.SOUND_BUZZER);
				}
			}
		}
		else if (mApp.CanDoDanceMode())
		{
			mApp.PlayFoley(FoleyType.Dancer);
			mApp.mDanceMode = !mApp.mDanceMode;
		}
		else
		{
			mApp.PlaySample(Resources.SOUND_BUZZER);
		}
	}

	public void UnlockGame()
	{
		if (mApp.mPlayerInfo.mFinishedAdventure >= 1)
		{
			mApp.mPlayerInfo.mHasUnlockedMinigames = true;
			mApp.mPlayerInfo.mHasUnlockedSurvivalMode = true;
			mApp.mPlayerInfo.mHasUnlockedPuzzleMode = true;
			mApp.mPlayerInfo.mMiniGamesUnlocked = 20;
			mApp.mPlayerInfo.mVasebreakerUnlocked = 10;
			mApp.mPlayerInfo.mIZombieUnlocked = 10;
			mApp.mPlayerInfo.mZenGardenTutorialComplete = true;
			mApp.mPlayerInfo.mHasUsedCheatKeys = true;
			mApp.KillGameSelector();
			mApp.ShowGameSelector();
		}
		else
		{
			mApp.mPlayerInfo.mLevel = 1;
			mApp.mPlayerInfo.mFinishedAdventure = 1;
			mApp.mPlayerInfo.DeleteUserFilesAfterUnlock();
			mApp.mPlayerInfo.mHasUnlockedMinigames = true;
			mApp.mPlayerInfo.mHasUnlockedSurvivalMode = true;
			mApp.mPlayerInfo.mHasUnlockedPuzzleMode = true;
			mApp.mPlayerInfo.mMiniGamesUnlocked = 20;
			mApp.mPlayerInfo.mVasebreakerUnlocked = 10;
			mApp.mPlayerInfo.mIZombieUnlocked = 10;
			mApp.mPlayerInfo.AddCoins(20000);
			mApp.mPlayerInfo.mZenGardenTutorialComplete = true;
			mApp.mPlayerInfo.mHasUsedCheatKeys = true;
			mApp.KillGameSelector();
			mApp.ShowGameSelector();
			mApp.DoDialog(61, isModal: true, "[ALREADY_UNLOCKED]", "[ALREADY_UNLOCKED_MESSAGE]", "[BUTTON_OK]", 3);
		}
		mApp.WriteCurrentUserConfig();
	}

	public override void KeyChar(SexyChar theChar)
	{
		if (!mApp.mTodCheatKeys)
		{
			return;
		}
		if (theChar.value_type == 'd')
		{
			ShowDynamicTachie();
		}
		else
		{
			if (theChar.value_type != 'u' || mApp.mPlayerInfo == null)
			{
				return;
			}
			mApp.mPlayerInfo.mFinishedAdventure = 2;
			mApp.mPlayerInfo.AddCoins(50000);
			mApp.mPlayerInfo.mHasUsedCheatKeys = true;
			mApp.mPlayerInfo.mHasUnlockedMinigames = true;
			mApp.mPlayerInfo.mHasUnlockedPuzzleMode = true;
			mApp.mPlayerInfo.mHasUnlockedSurvivalMode = true;
			mApp.mPlayerInfo.mMiniGamesUnlocked = 20;
			mApp.mPlayerInfo.mVasebreakerUnlocked = 10;
			mApp.mPlayerInfo.mIZombieUnlocked = 10;
			for (int i = 0; i < 200; i++)
			{
				GameMode gameMode = (GameMode)(i + 1);
				if (gameMode != GameMode.ScaryPotterEndless && gameMode != GameMode.PuzzleIZombieEndless && gameMode != GameMode.SurvivalEndlessStage3)
				{
					mApp.mPlayerInfo.mChallengeRecords[i] = 20;
				}
			}
			for (int j = 0; j < 80; j++)
			{
				mApp.mPlayerInfo.mPurchases[j] = 1;
			}
			mApp.mPlayerInfo.mPurchases[7] = 0;
			SyncProfile(theShowLoading: true);
			string savedGameName = LawnCommon.GetSavedGameName(GameMode.Adventure, (int)mApp.mPlayerInfo.mId);
			mApp.EraseFile(savedGameName);
		}
	}

	public void ShowOnlineLevels()
	{
		mApp.KillGameSelector();
		mApp.ShowChallengeScreen(ChallengePage.OnlineLevel);
	}

	public void ShowLocalLevels()
	{
		mApp.KillGameSelector();
		mApp.ShowChallengeScreen(ChallengePage.DIY);
	}

	private void ShowDynamicTachie()
	{
		DynamicTachieWidget widget = new DynamicTachieWidget("Content/spanim/fumeshroom_tachie.skel", new DynamicTachieWidget.DisplayTransform(1f / 7f, 514.2857f, 678.5714f), mApp)
		{
			mLastFocusedWidget = this
		};
		PlantVoice.Play(SeedType.None, SeedType.None, (RandomNumbers.NextNumber(2) == 0) ? PlantVoice.VoiceType.Special_Fumeshroom_Touch_Enter_1 : PlantVoice.VoiceType.Special_Fumeshroom_Touch_Enter_2);
		widget.TouchCollision += (DynamicTachieWidget.TouchArea a) =>
		{
			switch (a)
			{
			case DynamicTachieWidget.TouchArea.Touch:
				PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Fumeshroom_Touch1);
				widget.PlayInteractionAnimation("touch", 20, Reward(DynamicTachieWidget.TouchRewards.FumeshroomTouch1));
				break;
			case DynamicTachieWidget.TouchArea.Touch2:
				PlantVoice.Play(SeedType.None, SeedType.None, (RandomNumbers.NextNumber(2) == 0) ? PlantVoice.VoiceType.Special_Fumeshroom_Touch2_1 : PlantVoice.VoiceType.Special_Fumeshroom_Touch2_2);
				widget.PlayInteractionAnimation("touch2", 20, Reward(DynamicTachieWidget.TouchRewards.FumeshroomTouch2));
				break;
			case DynamicTachieWidget.TouchArea.Touch3:
				PlantVoice.Play(SeedType.None, SeedType.None, (RandomNumbers.NextNumber(2) == 0) ? PlantVoice.VoiceType.Special_Fumeshroom_Touch3_1 : PlantVoice.VoiceType.Special_Fumeshroom_Touch3_2);
				widget.PlayInteractionAnimation("touch3", 20, Reward(DynamicTachieWidget.TouchRewards.FumeshroomTouch3));
				break;
			}
			widget.RetractCloseButton();
		};
		widget.Resize(0, 0, Constants.BackBufferSize.Y, Constants.BackBufferSize.X);
		mApp.mWidgetManager.AddWidget(widget);
		mApp.mWidgetManager.BringToFront(widget);
		mApp.mWidgetManager.SetFocus(widget);
		AnimationState.TrackEntryDelegate Reward(DynamicTachieWidget.TouchRewards d)
		{
			return (TrackEntry _) =>
			{
				if (mApp.mPlayerInfo.mLastCollectedDynamicCoins[(int)d].AddDays(1.0) <= DateTime.UtcNow)
				{
					widget.FumeshroomReward();
					mApp.mPlayerInfo.mLastCollectedDynamicCoins[(int)d] = DateTime.UtcNow;
				}
			};
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (y > Constants.BOARD_HEIGHT)
		{
			mApp.PlaySample(Resources.SOUND_TAP);
			ButtonDepress(102);
		}
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		mX = -Constants.MAIN_MENU_ORIGIN_X;
	}

	public void FinishAdventureHardConfirmDialog()
	{
		if (mApp.mPlayerInfo != null)
		{
			string savedGameName = LawnCommon.GetSavedGameName(GameMode.Adventure, (int)mApp.mPlayerInfo.mId);
			if (mApp.EraseFile(savedGameName))
			{
				ToggleAdventureHard();
			}
		}
	}

	private void ToggleAdventureHard()
	{
		mApp.mPlayerInfo.mAdventureModeHard = !mApp.mPlayerInfo.mAdventureModeHard;
		mApp.WriteCurrentUserConfig();
		SyncAdventureHardButton();
		MarkDirty();
	}

	private void SyncAdventureHardButton()
	{
		bool flag = mApp.mPlayerInfo?.mAdventureModeHard ?? false;
		mAdventureHardButton.SetDisabled(mApp.mPlayerInfo == null);
		mAdventureHardButton.SetToggled(flag);
		mAdventureHardButton.mOverImage = (flag ? AtlasResources.IMAGE_BUTTON_ADVENTURE_HARD : AtlasResources.IMAGE_BUTTON_ADVENTURE_HARD_ACTIVATED);
		mAdventureHardButton.mDownImage = mAdventureHardButton.mOverImage;
		mAdventureHardButton.MarkDirty();
	}

	public void SyncButtons()
	{
		SyncAdventureHardButton();
		bool flag = mApp.CanShowAlmanac() || mUnlockSelectorCheat;
		bool flag2 = mApp.CanShowStore() || mUnlockSelectorCheat;
		bool flag3 = mApp.CanShowZenGarden() || mUnlockSelectorCheat;
		bool flag4 = mApp.HasFinishedAdventure();
		bool flag5 = mApp.EarnedGoldTrophy();
		bool flag6 = mApp.CanShowFumeshroomGnome() || mUnlockSelectorCheat;
		mAlmanacButton.mDisabled = !flag;
		mAlmanacButton.mVisible = flag;
		mStoreButton.mDisabled = !flag2;
		mStoreButton.mVisible = flag2;
		mZenGardenButton.mDisabled = !flag3;
		mZenGardenButton.mVisible = flag3;
		mTrophyButton.mDisabled = !flag4;
		mTrophyButton.mVisible = flag4;
		mFumeshroomGnomeButton.mDisabled = !flag6;
		mFumeshroomGnomeButton.mVisible = flag6;
		if (flag5)
		{
			mTrophyButton.mButtonImage = Resources.IMAGE_SUNFLOWER_TROPHY_2;
		}
		SetupUnlockFullGameReanim();
		if (mQuickplayLocked)
		{
			mMoreWaysToPlayButton.mOverImage = AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS;
			mMoreWaysToPlayButton.mDownImage = AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS;
			mMoreWaysToPlayButton.SetColor(5, new SexyColor(128, 128, 128));
		}
		else
		{
			mMoreWaysToPlayButton.mOverImage = AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS_HIGHLIGHT;
			mMoreWaysToPlayButton.mDownImage = AtlasResources.IMAGE_REANIM_SELECTORSCREEN_MOREWAYS_HIGHLIGHT;
			mMoreWaysToPlayButton.SetColor(5, SexyColor.White);
		}
	}

	public void AddTrophySparkle()
	{
	}

	public void ClickedAdventure()
	{
		mApp.mMusic.StopAllMusic();
		mAdventureButton.SetDisabled(isDisabled: true);
		mMoreWaysToPlayButton.SetDisabled(isDisabled: true);
		mZenGardenButton.SetDisabled(isDisabled: true);
		mOptionsButton.SetDisabled(isDisabled: true);
		mUserDialogButton.SetDisabled(isDisabled: true);
		mCommunityButton.SetDisabled(isDisabled: true);
		mOnlineLevelsButton.SetDisabled(isDisabled: true);
		mExitButton.SetDisabled(isDisabled: true);
		mStoreButton.SetDisabled(isDisabled: true);
		mAlmanacButton.SetDisabled(isDisabled: true);
		mTrophyButton.SetDisabled(isDisabled: true);
		mApp.KillGameSelector();
		if (mApp.IsIceDemo())
		{
			mApp.PreNewGame(GameMode.ChallengeIce, theLookForSavedGame: false);
		}
		else if (mApp.IsFirstTimeAdventureMode() && mLevel == 1 && !mApp.SaveFileExists())
		{
			mApp.PreNewGame(GameMode.Intro, theLookForSavedGame: false);
		}
		else if (mApp.mPlayerInfo.mNeedsMagicTacoReward && mLevel == 35)
		{
			StoreScreen storeScreen = mApp.ShowStoreScreen(this);
			storeScreen.SetupForIntro(601);
			mDoNewGameAfterStore = true;
		}
		else if (ShouldDoZenTuturialBeforeAdventure())
		{
			mApp.PreNewGame(GameMode.ChallengeZenGarden, theLookForSavedGame: false);
			mApp.mZenGarden.SetupForZenTutorial();
		}
		else
		{
			mApp.PreNewGame(GameMode.Adventure, theLookForSavedGame: true);
		}
	}

	public bool ShouldDoZenTuturialBeforeAdventure()
	{
		if (!mApp.HasFinishedAdventure() && mApp.mPlayerInfo.mLevel == 45)
		{
			return mApp.mPlayerInfo.mNumPottedPlants == 0;
		}
		return false;
	}

	public void BackFromStore()
	{
		StoreScreen storeScreen = (StoreScreen)mApp.GetDialog(Dialogs.DIALOG_STORE);
		mApp.KillDialog(4);
		if (mDoNewGameAfterStore)
		{
			mApp.PreNewGame(GameMode.Adventure, theLookForSavedGame: false);
		}
		else
		{
			mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.TitleCrazyDaveMainTheme);
		}
	}

	public void QuickPlayStageSelected(int theLevel)
	{
		mApp.PlaySample(Resources.SOUND_TAP);
		mApp.KillGameSelector();
		GameMode theGameMode = (GameMode)(theLevel - 1 + 72);
		mApp.PreNewGame(theGameMode, theLookForSavedGame: true);
	}

	public void MiniGamesStageSelected(int theLevel)
	{
		mApp.PlaySample(Resources.SOUND_TAP);
		GameMode theGameMode = (GameMode)(theLevel + 1);
		mApp.KillGameSelector();
		mApp.PreNewGame(theGameMode, theLookForSavedGame: true);
	}

	public void BackFromAlmanac()
	{
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.TitleCrazyDaveMainTheme);
	}

	public void LoadGames()
	{
	}

	public void SlideTo(int theX, int theY)
	{
		mSlideCounter = 75;
		mDestX = theX;
		mDestY = theY;
		mStartX = mX;
		mStartY = mY;
	}

	public void MoveToQuickplay(bool theDoFadeIn, GameSelectorButtons theButton)
	{
		switch (theButton)
		{
		case GameSelectorButtons.Vasebreaker:
		case GameSelectorButtons.IZombie:
		case GameSelectorButtons.Misc:
			SwitchToPuzzle();
			PuzzleSetPosition(theButton);
			break;
		case GameSelectorButtons.MiniGames:
			SwitchToMiniGames();
			break;
		case GameSelectorButtons.Survival:
			SwitchToSurvival();
			break;
		case GameSelectorButtons.CreativeLevel:
			SwitchToCreativeLevel();
			break;
		case GameSelectorButtons.ZenGarden:
		case GameSelectorButtons.ChangeUser:
		case GameSelectorButtons.ChallengePageSurvival:
		case GameSelectorButtons.ChallengePageLimbo:
		case GameSelectorButtons.Puzzle:
			break;
		}
	}

	public void SlideOutQuickPlayWidget()
	{
		if (mQuickplaySlideCounter <= 0)
		{
			mQuickplaySlideCounter = 30;
		}
		else if (mRetractingQuickplay)
		{
			mQuickplaySlideCounter = 30 - mQuickplaySlideCounter;
		}
		mRetractingQuickplay = false;
	}

	public void RetractQuickPlayWidget()
	{
		if (mQuickplaySlideCounter <= 0)
		{
			mQuickplaySlideCounter = 30;
		}
		else if (!mRetractingQuickplay)
		{
			mQuickplaySlideCounter = 30 - mQuickplaySlideCounter;
		}
		mRetractingQuickplay = true;
	}

	public void PopulateQuickPlayWidget()
	{
		MiniGameMode mode = MiniGameMode.Games;
		switch (mSelectedQuickplayButtonId)
		{
		case 111:
			mode = MiniGameMode.Games;
			break;
		case 118:
			mode = MiniGameMode.Limbo;
			break;
		case 112:
		case 120:
			mode = MiniGameMode.Vasebreaker;
			break;
		case 113:
			mode = MiniGameMode.IZombie;
			break;
		case 114:
			mode = MiniGameMode.Limbo;
			break;
		case 119:
			mode = MiniGameMode.Survival;
			break;
		case 121:
			mode = MiniGameMode.Extra;
			break;
		}
		mMiniGamesWidget.SwitchMode(mode);
	}

	public void ButtonMouseMove(int id, int x, int y)
	{
	}

	public void ButtonMouseLeave(int id)
	{
	}

	public void ButtonMouseTick(int id)
	{
	}

	public void ButtonDownTick(int id)
	{
	}

	public void ButtonPress(int id)
	{
	}
}
