using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lawn.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class LawnApp : SexyAppBase
{
	private sealed class NewVersionDialogListener : DialogListener
	{
		private readonly LawnApp mApp;

		public NewVersionDialogListener(LawnApp app)
		{
			mApp = app;
		}

		public void DialogButtonPress(int theDialogId, int theButtonId)
		{
		}

		public void DialogButtonDepress(int theDialogId, int theButtonId)
		{
			mApp.KillDialog(theDialogId);
			if (theButtonId == 1000)
			{
				OpenUrl("https://space.bilibili.com/3493090151107069/dynamic");
			}
		}
	}

	private class AboutDialogListener : DialogListener
	{
		public void DialogButtonPress(int theDialogId, int theButtonId)
		{
		}

		public void DialogButtonDepress(int theDialogId, int theButtonId)
		{
			if (theButtonId == 1001)
			{
				OpenUrl("https://space.bilibili.com/3493090151107069");
			}
			else
			{
				GlobalStaticVars.gSexyAppBase.DialogButtonDepress(theDialogId, theButtonId);
			}
		}
	}

	private class TableTmp
	{
		public int mNormalImageId;

		public int mAdditiveImageId;

		public TableTmp(int aNormal, int aAdditive)
		{
			mNormalImageId = aNormal;
			mAdditiveImageId = aAdditive;
		}
	}

	public record Edition(bool HasComic, bool HasOnlineLevels);

	private const string PLACEHOLDER_PLAYER = "{PLAYER_NAME}";

	private const string PLACEHOLDER_MONEY = "{MONEY}";

	private const string PLACEHOLDER_UPGRADECOST = "{UPGRADE_COST}";

	private const string PLACEHOLDER_CRAZYDAVE_0 = "[CRAZY_DAVE_{0}]";

	public static string AppVersionNumber = "PGvZ 1.3.4";

	private Task<string> mUpdateCheckTask;

	private bool mUpdateCheckHandled;

	public Board mBoard;

	public TitleScreen mTitleScreen;

	public GameSelector mGameSelector;

	public SeedChooserScreen mSeedChooserScreen;

	public AwardScreen mAwardScreen;

	public CreditScreen mCreditScreen;

	public TodFoley mSoundSystem;

	public LinkedList<ButtonWidget> mControlButtonList = new LinkedList<ButtonWidget>();

	public LinkedList<Image> mCreatedImageList = new LinkedList<Image>();

	public string mReferId;

	public string mRegisterLink;

	public string mMod;

	public bool mRegisterResourcesLoaded;

	public bool mTodCheatKeys;

	public GameMode mGameMode;

	public CreativeLevel mCreativeLevel;

	public PulloutPanel mLevelInfoPanel;

	public GameScenes mGameScene;

	public bool mLoadingZombiesThreadCompleted;

	public bool mFirstTimeGameSelector;

	public int mGamesPlayed;

	public int mMaxExecutions;

	public int mMaxPlays;

	public int mMaxTime;

	public bool mEasyPlantingCheat;

	public ZenGarden mZenGarden;

	public EffectSystem mEffectSystem;

	public ReanimatorCache mReanimatorCache;

	public ProfileMgr mProfileMgr;

	private PlayerInfo _playerInfo;

	public LevelStats mLastLevelStats;

	public bool mCloseRequest;

	public int mAppCounter;

	public Music mMusic;

	public Reanimation mCrazyDaveReanimID;

	public CrazyDaveState mCrazyDaveState;

	public int mCrazyDaveBlinkCounter;

	public Reanimation mCrazyDaveBlinkReanimID;

	public int mCrazyDaveMessageIndex;

	public string mCrazyDaveMessageText = string.Empty;

	public int mAppRandSeed;

	public int mSessionID;

	public int mPlayTimeActiveSession;

	public int mPlayTimeInactiveSession;

	public BoardResult mBoardResult;

	public bool mKilledYetiAndRestarted;

	public TypingCheck mKonamiCheck;

	public TypingCheck mMustacheCheck;

	public TypingCheck mMoustacheCheck;

	public TypingCheck mSuperMowerCheck;

	public TypingCheck mSuperMowerCheck2;

	public TypingCheck mFutureCheck;

	public TypingCheck mPinataCheck;

	public TypingCheck mDanceCheck;

	public TypingCheck mDaisyCheck;

	public TypingCheck mSukhbirCheck;

	public bool mMustacheMode;

	public bool mSuperMowerMode;

	public bool mFutureMode;

	public bool mPinataMode;

	public bool mDanceMode;

	public bool mDaisyMode;

	public bool mSukhbirMode;

	public TrialType mTrialType;

	public bool mDebugTrialLocked;

	public bool mMuteSoundsForCutscene;

	private string mLastBackgroundResGroupLoaded;

	private string mLastZombieNoteResGroupLoaded;

	private string mLastStoreResGroupLoaded;

	private string mLastPaperGroupLoaded;

	private string mLastZenGardenResourceLoaded;

	private string mLastUpsellResGroupLoaded;

	private string mLastComicThumbnailResGroupLoaded;

	public GameMode mRestoreGameMode;

	public RestoreLocation mRestoreLocation;

	public bool checkGiveAchievements;

	public AchievementId achievementToCheck = AchievementId.MAX_ACHIEVEMENTS;

	private Dictionary<int, string> cachedStageStrings = new Dictionary<int, string>();

	public ChallengeScreen mChallengeScreen;

	public LeaderboardScreen mLeaderboardScreen;

	private static Dictionary<string, string> noClickStringCache = new Dictionary<string, string>(10);

	public LawnMessageBoxListener mLawnMessageBoxListener;

	public Widget mOldFocus;

	private static Dictionary<int, string> moneyStrings = new Dictionary<int, string>();

	private static Dictionary<int, string> cachedIntToString = new Dictionary<int, string>();

	private bool leaderboardLoaded;

	private bool gamePlayLoaded;

	private bool cachedLoaded;

	private bool zenGardenLoaded;

	private bool mainMenuLoaded;

	private bool pileLoaded;

	public ConcurrentQueue<Texture2D> mTexturesToBePremultiplied = new ConcurrentQueue<Texture2D>();

	public ConcurrentQueue<Image> mImagesToBeFiltered = new ConcurrentQueue<Image>();

	internal bool mCheatEnabled;

	public Sexy.Dialog mCheatHoverDialog;

	public readonly Edition mEdition = new Edition(HasComic: false, HasOnlineLevels: true) /* no_comic.py */;

	public PlayerInfo mPlayerInfo
	{
		get
		{
			return _playerInfo;
		}
		set
		{
			_playerInfo = value;
			if (mPlayerInfo != null)
			{
				SetMusicVolume(mPlayerInfo.mMusicVolume);
				SetSfxVolume(mPlayerInfo.mSoundVolume);
				Main.RunWhenLocked = mPlayerInfo.mRunWhileLocked;
				ZBCheatDialog.EnableCoinAutoCollect = mPlayerInfo.mCoinAutoCollect;
			}
		}
	}

	public static void CenterDialog(Sexy.Dialog theDialog, int theWidth, int theHeight)
	{
		theDialog.Resize((Constants.BOARD_WIDTH - theWidth) / 2, (Constants.BOARD_HEIGHT - theHeight) / 2, theWidth, theHeight);
	}

	public LawnApp(Main m)
		: base(m)
	{
		mBoard = null;
		mGameSelector = null;
		mSeedChooserScreen = null;
		mAwardScreen = null;
		mCreditScreen = null;
		mTitleScreen = null;
		mSoundSystem = null;
		mKonamiCheck = null;
		mMustacheCheck = null;
		mMoustacheCheck = null;
		mSuperMowerCheck = null;
		mSuperMowerCheck2 = null;
		mFutureCheck = null;
		mPinataCheck = null;
		mDanceCheck = null;
		mDaisyCheck = null;
		mSukhbirCheck = null;
		mMustacheMode = false;
		mSuperMowerMode = false;
		mFutureMode = false;
		mPinataMode = false;
		mDanceMode = false;
		mDaisyMode = false;
		mSukhbirMode = false;
		mGameScene = GameScenes.Loading;
		mZenGarden = null;
		mEffectSystem = null;
		mReanimatorCache = null;
		mCloseRequest = false;
		mWidth = Constants.BOARD_WIDTH;
		mHeight = Constants.BOARD_HEIGHT;
		mAppCounter = 0;
		mAppRandSeed = DateTime.UtcNow.Millisecond;
		mTrialType = TrialType.None;
		mDebugTrialLocked = false;
		mMuteSoundsForCutscene = false;
		mMusicVolume = 0.85;
		mSfxVolume = 0.85;
		mAutoStartLoadingThread = false;
		mProdName = "PlantsVsZombies";
		string text = "Plants vs. Zombies";
		mTitle = text;
		mPlayerInfo = null;
		mLastLevelStats = new LevelStats();
		mFirstTimeGameSelector = true;
		mGameMode = GameMode.Adventure;
		mCreativeLevel = null;
		mEasyPlantingCheat = false;
		mLoadingZombiesThreadCompleted = true;
		mGamesPlayed = 0;
		mMaxExecutions = 0;
		mMaxPlays = 0;
		mMaxTime = 0;
		mCompletedLoadingThreadTasks = 0;
		mProfileMgr = new ProfileMgr();
		mRegisterResourcesLoaded = false;
		mTodCheatKeys = false;
		mCrazyDaveReanimID = null;
		mCrazyDaveState = CrazyDaveState.Off;
		mCrazyDaveBlinkCounter = 0;
		mCrazyDaveBlinkReanimID = null;
		mCrazyDaveMessageIndex = -1;
		mLawnMessageBoxListener = null;
		ReportAchievement.AchievementsChanged += ReportAchievement_AchievementsChanged;
	}

	private void ReportAchievement_AchievementsChanged()
	{
		if (mPlayerInfo != null)
		{
			mPlayerInfo.UpdateAchievementInfo();
		}
	}

	public override void Dispose()
	{
		if (mBoard != null)
		{
			WriteCurrentUserConfig();
		}
		if (mBoard != null)
		{
			mBoardResult = BoardResult.QuitApp;
			mBoard.TryToSaveGame();
			mWidgetManager.RemoveWidget(mBoard);
			mBoard.Dispose();
			mBoard = null;
		}
		if (mTitleScreen != null)
		{
			mWidgetManager.RemoveWidget(mTitleScreen);
			mTitleScreen.Dispose();
		}
		if (mGameSelector != null)
		{
			mWidgetManager.RemoveWidget(mGameSelector);
			mGameSelector.Dispose();
		}
		if (mSeedChooserScreen != null)
		{
			mWidgetManager.RemoveWidget(mSeedChooserScreen);
			mSeedChooserScreen.Dispose();
		}
		if (mAwardScreen != null)
		{
			mWidgetManager.RemoveWidget(mAwardScreen);
			mAwardScreen.Dispose();
		}
		if (mCreditScreen != null)
		{
			mWidgetManager.RemoveWidget(mCreditScreen);
			mCreditScreen.Dispose();
		}
		mProfileMgr.Dispose();
		mResourceManager.DeleteResources("");
	}

	public bool KillNewOptionsDialog()
	{
		if ((NewOptionsDialog)GetDialog(Dialogs.DIALOG_NEWOPTIONS) == null)
		{
			return false;
		}
		KillDialog(2);
		return true;
	}

	public bool KillLeaderboardDialog()
	{
		if ((LeaderboardDialog)GetDialog(Dialogs.DIALOG_LEADERBOARD) == null)
		{
			return false;
		}
		KillDialog(59);
		mLeaderboardScreen.SetGrayed(aGray: false);
		return true;
	}

	public override void GotFocus()
	{
		if (mSoundManager != null)
		{
			mSoundManager.Enable(enable: true);
			mSoundManager.SetMasterVolume(mSfxVolume);
		}
		if (mMusicInterface != null)
		{
			mMusicInterface.Enable(mMusicEnabled);
		}
		if (mCreditScreen != null)
		{
			mCreditScreen.AppGotFocus();
		}
		if (checkGiveAchievements && !SexyAppBase.IsInTrialMode)
		{
			checkGiveAchievements = false;
			ReportAchievement.GiveAchievement(achievementToCheck);
		}
		base.GotFocus();
	}

	public override void LostFocus()
	{
		base.LostFocus();
		if (mSoundManager != null)
		{
			mSoundManager.SetMasterVolume(0.0);
		}
		if (mBoard != null && mBoard.mBoardFadeOutCounter > 0)
		{
			mBoard.mBoardFadeOutCounter = 3;
		}
		if (!mTodCheatKeys && CanPauseNow() && !Main.RunWhenLocked)
		{
			if (mBoard != null)
			{
				mBoard.RefreshSeedPacketFromCursor();
			}
			DoPauseDialog();
		}
	}

	public override void AppEnteredBackground()
	{
		WriteRestoreInfo();
		if (mBoard != null)
		{
			mBoard.TryToSaveGame();
		}
		WriteCurrentUserConfig();
	}

	public override void InitHook()
	{
	}

	public override void WriteToRegistry()
	{
		if (mPlayerInfo != null)
		{
			RegistryWriteString("CurUser", mPlayerInfo.mName);
			mPlayerInfo.SaveDetails();
		}
		base.WriteToRegistry();
	}

	public override void LoadingThreadProc()
	{
		GameConstants.Init();
		CreativeLevelManager.Init();
		if (!TodCommon.TodLoadResources("LoaderBar") || !TodCommon.TodLoadResources("LoaderBarFont"))
		{
			throw new Exception(mResourceManager.Error);
		}
		Resources.ExtractLoaderBarFontResources(mResourceManager);
		Resources.ExtractLoaderBarResources(mResourceManager);
		AtlasResources.mAtlasResources.UnpackLoadingAtlasImages();
		Resources.LinkUpResArray();
		ReanimationParams[] theReanimationParamArray = new ReanimationParams[2]
		{
			new ReanimationParams(ReanimationType.LoadbarSprout, "reanim/LoadBar_sprout", 1),
			new ReanimationParams(ReanimationType.LoadbarZombiehead, "reanim/LoadBar_Zombiehead", 1)
		};
		ReanimatorXnaHelpers.ReanimatorLoadDefinitions(ref theReanimationParamArray, theReanimationParamArray.Length);
		TodStringFile.TodStringListLoad("Content/LawnStrings_" + Constants.LanguageSubDir + ".txt");
		mTitleScreen.mLoaderScreenIsLoaded = true;
		mNumLoadingThreadTasks += mResourceManager.GetNumResources("LoadingFonts") * 54;
		mNumLoadingThreadTasks += mResourceManager.GetNumResources("LoadingImages") * 9;
		mNumLoadingThreadTasks += mResourceManager.GetNumResources("LoadingSounds") * 54;
		mNumLoadingThreadTasks += 612;
		mNumLoadingThreadTasks += 8092;
		mNumLoadingThreadTasks += 360500;
		mNumLoadingThreadTasks += GetNumPreloadingTasks();
		mNumLoadingThreadTasks += mMusic.GetNumLoadingTasks();
		if (!Main.LOW_MEMORY_DEVICE)
		{
			DelayLoadGamePlayResources(doLoad: true);
			DelayLoadLeaderboardResource(doLoad: true);
			DelayLoadCachedResources(doLoad: true);
			DelayLoadZenGardenResources(doLoad: true);
		}
		DelayLoadMainMenuResource(doLoad: true);
		mResourceManager.LoadAllResources();
		Resources.ExtractResources(mResourceManager, AtlasResources.mAtlasResources);
		AtlasResources.mAtlasResources.ExtractResources();
		DiscreteResources.UnpackAtlases();
		ReanimatorXnaHelpers.ReanimatorLoadDefinitions(ref GameConstants.gLawnReanimationArray, 150);
		TodStringFile.TodStringListSetColors(GameConstants.gLawnStringFormats, GameConstants.gLawnStringFormatCount);
		if (mLoadingFailed || mShutdown || mCloseRequest)
		{
			throw new Exception();
		}
		mMusic.MusicInit();
		mZenGarden = new ZenGarden();
		mReanimatorCache = new ReanimatorCache();
		mReanimatorCache.ReanimatorCacheInitialize();
		TodFoley.TodFoleyInitialize(null, 105);
		GlobalMembersTrail.TrailLoadDefinitions(GameConstants.gLawnTrailArray, 4);
		TodParticleGlobal.TodParticleLoadDefinitions(ref GameConstants.gLawnParticleArray, 107);
		PreloadForUser();
		if (!mLoadingFailed && !mShutdown)
		{
			bool flag = mCloseRequest;
		}
	}

	public virtual void LoadingCompleted()
	{
		mWidgetManager.RemoveWidget(mTitleScreen);
		SafeDeleteWidget(mTitleScreen);
		mTitleScreen = null;
		mResourceManager.DeleteImage("IMAGE_TITLESCREEN");
		if (mRestoreLocation != RestoreLocation.Board || !RestoreGame())
		{
			if (mPlayerInfo != null)
			{
				Apply500pMode(mPlayerInfo.m500pMode);
			}
			ShowGameSelector();
			if (Main.LOW_MEMORY_DEVICE)
			{
				mResourceManager.UnloadInitResources();
			}
		}
	}

	public static void PreallocateMemory()
	{
		GC.Collect();
		ReanimatorTransform.PreallocateMemory();
		Reanimation.PreallocateMemory();
		ReanimatorTrackInstance.PreallocateMemory();
		Attachment.PreallocateMemory();
		Plant.PreallocateMemory();
		Zombie.PreallocateMemory();
		Projectile.PreallocateMemory();
		XNASoundInstance.PreallocateMemory();
		TodParticle.PreallocateMemory();
		RenderItem.PreallocateMemory();
		TodParticleEmitter.PreallocateMemory();
		SexyAppBase.XnaGame.CompensateForSlowUpdate();
	}

	public override bool DoUpdateFrames()
	{
		return base.DoUpdateFrames();
	}

	public override void LoadingThreadCompleted()
	{
		PropertiesParser propertiesParser = new PropertiesParser(this);
		if (propertiesParser.ParsePropertiesFile("properties/content.xml"))
		{
			string text = GetString("ContentUpdateSitePrefix");
			if (!string.IsNullOrEmpty(text))
			{
				Sexy.Debug.OutputDebug<string>(Common.StrFormat_("Content Update: URL={0}\n", text));
			}
			else
			{
				Sexy.Debug.OutputDebug<string>("Content Update: Failed to find property 'ContentUpdateSitePrefix'.\n");
			}
		}
		else
		{
			Sexy.Debug.OutputDebug<string>(Common.StrFormat_("Content Update: Failed to parse properties file: {0}\n", propertiesParser.GetErrorText()));
		}
		GC.Collect();
		SexyAppBase.XnaGame.CompensateForSlowUpdate();
	}

	public override IEnumerable<bool> LoadingThreadAfterWorks()
	{
		while (mTexturesToBePremultiplied.Count > 0)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			int num = 0;
			while ((double)stopwatch.ElapsedMilliseconds < 5.0 && mTexturesToBePremultiplied.Count > 0)
			{
				if (mTexturesToBePremultiplied.TryDequeue(out var result))
				{
					GlobalStaticVars.gSexyAppBase.mResourceManager.PremultiplyTexture(result, doRefTexture: true);
					num++;
				}
			}
			Sexy.Debug.Log(DebugType.Debug, $"Premultiplied {num}.");
			yield return true;
		}
		while (mImagesToBeFiltered.Count > 0)
		{
			Stopwatch stopwatch2 = Stopwatch.StartNew();
			int num2 = 0;
			while ((double)stopwatch2.ElapsedMilliseconds < 5.0 && mImagesToBeFiltered.Count > 0)
			{
				if (mImagesToBeFiltered.TryDequeue(out var _))
				{
					num2++;
				}
			}
			Sexy.Debug.Log(DebugType.Debug, $"Filtered {num2} in {stopwatch2.ElapsedMilliseconds} ms.");
			yield return true;
		}
		for (SeedType i = SeedType.Peashooter; i < SeedType.SeedTypeCount; i++)
		{
			if (i != SeedType.Sprout)
			{
				mReanimatorCache.MakeCachedPlantFrame(i, DrawVariation.Normal);
				yield return true;
			}
		}
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Lilypad, DrawVariation.ZenGarden);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Pumpkinshell, DrawVariation.ZenGarden);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Tallnut, DrawVariation.ZenGarden);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Flowerpot, DrawVariation.ZenGarden);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Flowerpot, DrawVariation.ZenGardenWater);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Sunshroom, DrawVariation.Bigidle);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Tanglekelp, DrawVariation.Aquarium);
		mReanimatorCache.MakeCachedPlantFrame(SeedType.Seashroom, DrawVariation.Aquarium);
		yield return true;
		for (LawnMowerType i2 = LawnMowerType.Lawn; i2 < LawnMowerType.MowerTypesCount; i2++)
		{
			mReanimatorCache.MakeCachedMowerFrame(i2);
			yield return true;
		}
		for (ZombieType i3 = ZombieType.Normal; i3 < ZombieType.CachedZombieTypesCount; i3++)
		{
			if (i3 != ZombieType.ZombieTypesCount)
			{
				mReanimatorCache.MakeCachedZombieFrame(i3);
				yield return true;
			}
		}
		for (DrawVariation j = DrawVariation.MarigoldWhite; j <= DrawVariation.MarigoldLightGreen; j++)
		{
			mReanimatorCache.MakeCachedPlantFrame(SeedType.Marigold, j);
			yield return true;
		}
		Sexy.Debug.Log(DebugType.Debug, "LoadingAfterWorks done.");
	}

	public virtual bool DebugKeyDown(int theKey)
	{
		return false;
	}

	public virtual void HandleCmdLineParam(string theParamName, string theParamValue)
	{
	}

	public override void PlaySample(int theSoundNum)
	{
		if (!mMuteSoundsForCutscene)
		{
			base.PlaySample(theSoundNum);
		}
	}

	public void ConfirmQuit()
	{
		string theDialogLines = TodStringFile.TodStringTranslate("[QUIT_MESSAGE]");
		string theDialogHeader = TodStringFile.TodStringTranslate("[QUIT_HEADER]");
		LawnDialog lawnDialog = DoDialog(13, isModal: true, theDialogHeader, theDialogLines, "", 2);
		lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[QUIT_BUTTON]");
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
	}

	public void ConfirmCheckForUpdates()
	{
		CheckForUpdates();
	}

	public void CheckForUpdates()
	{
		if (mUpdateCheckTask == null)
		{
			mUpdateCheckTask = GameUpdateChecker.GetNewVersionAsync("https://levels.pgvz.top/api/v2/version", AppVersionNumber.Replace("PGvZ ", ""));
		}
	}

	public void UpdateVersionCheck()
	{
		if (mUpdateCheckHandled)
		{
			return;
		}
		CheckForUpdates();
		if (!mUpdateCheckTask.IsCompleted || GetDialogCount() != 0)
		{
			return;
		}
		mUpdateCheckHandled = true;
		if (mUpdateCheckTask.IsCompletedSuccessfully)
		{
			string result = mUpdateCheckTask.Result;
			if (result != null)
			{
				ShowNewVersionDialog(result);
			}
		}
	}

	public void ShowNewVersionDialog(string version)
	{
		bool flag = Constants.Language == Constants.LanguageIndex.zh_cn;
		string theDialogHeader = (flag ? "发现新版本" : "Update available");
		string theDialogLines = (flag ? ("检测到新版本 " + version + "，是否前往更新？") : ("Version " + version + " is available. Would you like to update?"));
		LawnDialog lawnDialog = DoDialog(56, isModal: true, theDialogHeader, theDialogLines, string.Empty, 1);
		lawnDialog.mDialogListener = new NewVersionDialogListener(this);
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
		mWidgetManager.SetFocus(lawnDialog);
	}

	public void DoUserDialog()
	{
		KillDialog(29);
		UserDialog userDialog = new UserDialog(this);
		CenterDialog(userDialog, userDialog.mWidth, userDialog.mHeight);
		AddDialog(29, userDialog);
		mWidgetManager.SetFocus(userDialog);
	}

	public void FinishUserDialog(bool isYes)
	{
		UserDialog userDialog = (UserDialog)GetDialog(Dialogs.DIALOG_USERDIALOG);
		if (userDialog == null)
		{
			return;
		}
		if (isYes)
		{
			PlayerInfo profile = mProfileMgr.GetProfile(userDialog.GetSelName());
			if (profile != null)
			{
				mPlayerInfo = profile;
				mWidgetManager.MarkAllDirty();
				if (mGameSelector != null)
				{
					mGameSelector.SyncProfile(theShowLoading: true);
				}
			}
			Apply500pMode(mPlayerInfo.m500pMode);
			ShowGameSelector();
		}
		KillDialog(29);
	}

	public void DoCreateUserDialog(bool isOnlyUser)
	{
		KillDialog(30);
		NewUserDialog newUserDialog = new NewUserDialog(this, !isOnlyUser);
		if (isOnlyUser)
		{
			newUserDialog.SetName("Player");
		}
		newUserDialog.Resize((800 - newUserDialog.mWidth) / 2, (600 - newUserDialog.mHeight) / 2, newUserDialog.mWidth, newUserDialog.mHeight);
		CenterDialog(newUserDialog, newUserDialog.mWidth, newUserDialog.mHeight);
		AddDialog(30, newUserDialog);
	}

	public static void OpenUrl(string url)
	{
		LawnPlatform.PlatformUI.OpenUrl(url);
	}

	public void DoUpdateDialog()
	{
		string richTextContent = "游戏已更新至 /c[cyan]" + AppVersionNumber.Replace("PGvZ ", "") + "/cd 版本！本次更新包含：\n/c[cyan] - 修复安卓版鼓点相关的报错问题/cd\n/c[cyan] ------------------------------------/cd\n/c[cyan] - 裂荚射手娘化/cd\n/c[cyan] - 画质更新：玉米投手/cd\n/c[cyan] - 小喷菇漫画cd\n/c[lightgray] - 背景音乐动态鼓点/cd\n/c[cyan] - 调整了冒险模式高难的5-5关卡/cd\n/c[cyan] - 保存作弊菜单的一些设置/cd\n/c[cyan] - 修复手套移动火红莲造成的视觉问题/cd\n/c[cyan] - 修复传送带卡片出现时间过长的问题/cd\n/c[cyan] - 修复与大泳池香蒲有关的报错/cd\n/c[cyan] - 隐藏在线关卡的ID/cd";
		UpdateDialog updateDialog = new UpdateDialog(this, richTextContent, Resources.FONT_DWARVENTODCRAFT12);
		base.DoDialog(updateDialog, 65);
		CenterDialog(updateDialog, updateDialog.mWidth, updateDialog.mHeight);
	}

	public void DoAboutDialog()
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		empty = string.Empty;
		empty2 = TodStringFile.TodStringTranslate("[ABOUT_1]") + AppVersionNumber;
		LawnDialog lawnDialog = DoDialog(52, isModal: true, empty, empty2, "", 3);
		HyperlinkWidget hyperlinkWidget = new HyperlinkWidget(1001, lawnDialog);
		hyperlinkWidget.SetFont(Resources.FONT_DWARVENTODCRAFT15);
		hyperlinkWidget.mColor = new SexyColor(255, 255, 136);
		hyperlinkWidget.mOverColor = new SexyColor(0, 0, 255);
		hyperlinkWidget.mDoFinger = true;
		hyperlinkWidget.mLabel = "庄不纯";
		hyperlinkWidget.Resize(259, 96, (int)hyperlinkWidget.mFont.MeasureString(hyperlinkWidget.mLabel).X, (int)hyperlinkWidget.mFont.MeasureString(hyperlinkWidget.mLabel).Y + 10);
		lawnDialog.AddWidget(hyperlinkWidget);
		lawnDialog.mDialogListener = new AboutDialogListener();
		lawnDialog.mMinWidth = (int)Constants.InvertAndScaleUI(350f);
		lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[BACK]");
		lawnDialog.CalcSize(0, 0);
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
	}

	public void DoCheatDialog()
	{
		KillDialog(35);
		CheatDialog cheatDialog = new CheatDialog(this);
		CenterDialog(cheatDialog, cheatDialog.mWidth, cheatDialog.mHeight);
		AddDialog(35, cheatDialog);
	}

	public void FinishCheatDialog(bool isYes)
	{
		CheatDialog cheatDialog = (CheatDialog)GetDialog(Dialogs.DIALOG_CHEAT);
		if (cheatDialog != null && (!isYes || cheatDialog.ApplyCheat()))
		{
			KillDialog(35);
			if (isYes)
			{
				mMusic.StopAllMusic();
				mBoardResult = BoardResult.Cheat;
				PreNewGame(mGameMode, theLookForSavedGame: false);
			}
		}
	}

	public void FinishCreateUserDialog(bool isYes)
	{
		string text = (GetDialog(30) as NewUserDialog)?.GetName();
		string theDialogLines = "[ENTER_NEW_USER]";
		if (isYes && text.empty() && mPlayerInfo != null)
		{
			KillDialog(30);
			return;
		}
		if (mPlayerInfo == null && (!isYes || text.empty()))
		{
			DoDialog(33, isModal: true, "[ENTER_YOUR_NAME]", theDialogLines, "[DIALOG_BUTTON_OK]", 3);
			return;
		}
		if (!isYes)
		{
			KillDialog(30);
			return;
		}
		PlayerInfo playerInfo = mProfileMgr.AddProfile(text);
		if (playerInfo == null)
		{
			DoDialog(33, isModal: true, "[NAME_CONFLICT]", "[ENTER_UNIQUE_PLAYER_NAME]", "[DIALOG_BUTTON_OK]", 3);
			return;
		}
		mProfileMgr.Save();
		mPlayerInfo = playerInfo;
		KillDialog(29);
		KillDialog(30);
		mWidgetManager.MarkAllDirty();
		if (mGameSelector != null)
		{
			mGameSelector.SyncProfile(theShowLoading: true);
		}
	}

	public void DoConfirmDeleteUserDialog(string theName)
	{
		KillDialog(31);
		DoDialog(31, isModal: true, "[ARE_YOU_SURE]", Common.StrFormat_(TodStringFile.TodStringTranslate("[DELETE_USER_WARNING]"), theName), "", 1);
	}

	public void FinishConfirmDeleteUserDialog(bool isYes)
	{
		KillDialog(31);
		UserDialog userDialog = (UserDialog)GetDialog(Dialogs.DIALOG_USERDIALOG);
		if (userDialog == null)
		{
			return;
		}
		mWidgetManager.SetFocus(userDialog);
		if (!isYes)
		{
			return;
		}
		string text = ((mPlayerInfo != null) ? mPlayerInfo.mName : "");
		string selName = userDialog.GetSelName();
		if (selName == text)
		{
			mPlayerInfo = null;
		}
		mProfileMgr.DeleteProfile(selName);
		userDialog.FinishDeleteUser();
		if (mPlayerInfo == null)
		{
			mPlayerInfo = mProfileMgr.GetProfile(userDialog.GetSelName());
			if (mPlayerInfo == null)
			{
				mPlayerInfo = mProfileMgr.GetAnyProfile();
			}
		}
		mProfileMgr.Save();
		if (mPlayerInfo == null)
		{
			DoCreateUserDialog(isOnlyUser: true);
		}
		mWidgetManager.MarkAllDirty();
		if (mGameSelector != null)
		{
			mGameSelector.SyncProfile(theShowLoading: true);
		}
	}

	public void DoRenameUserDialog(string theName)
	{
		KillDialog(32);
		NewUserDialog newUserDialog = new NewUserDialog(this, isRename: true, allowCancel: true);
		newUserDialog.Move(mWidth / 2 - newUserDialog.mWidth / 2, (int)Constants.InvertAndScale(20f));
		newUserDialog.SetName(theName);
		AddDialog(32, newUserDialog);
	}

	public void FinishRenameUserDialog(bool isYes)
	{
		UserDialog userDialog = (UserDialog)GetDialog(29);
		if (!isYes)
		{
			KillDialog(32);
			mWidgetManager.SetFocus(userDialog);
			return;
		}
		NewUserDialog newUserDialog = (NewUserDialog)GetDialog(32);
		if (userDialog == null || newUserDialog == null)
		{
			return;
		}
		string selName = userDialog.GetSelName();
		string name = newUserDialog.GetName();
		if (string.IsNullOrEmpty(name))
		{
			KillDialog(32);
			mWidgetManager.SetFocus(userDialog);
			return;
		}
		bool flag = mProfileMgr.GetProfile(selName) == mPlayerInfo;
		if (!mProfileMgr.RenameProfile(selName, name))
		{
			DoDialog(34, isModal: true, "[NAME_CONFLICT]", "[ENTER_UNIQUE_PLAYER_NAME]", "[DIALOG_BUTTON_OK]", 3);
			return;
		}
		mProfileMgr.Save();
		if (flag)
		{
			mPlayerInfo = mProfileMgr.GetProfile(name);
		}
		userDialog.FinishRenameUser(name);
		mWidgetManager.MarkAllDirty();
		KillDialog(32);
		mWidgetManager.SetFocus(userDialog);
	}

	public void FinishNameError(int theId)
	{
		KillDialog(theId);
		NewUserDialog newUserDialog = (NewUserDialog)GetDialog(Dialogs.DIALOG_CREATEUSER);
		if (newUserDialog != null)
		{
			mWidgetManager.SetFocus(newUserDialog.mNameEditWidget);
		}
	}

	public void FinishRestartConfirmDialog()
	{
		mKilledYetiAndRestarted = mBoard.mKilledYeti;
		KillDialog(37);
		KillDialog(39);
		KillBoard();
		PreNewGame(mGameMode, theLookForSavedGame: false);
	}

	public void FinishInGameRestartConfirmDialog(bool isYes)
	{
		KillDialog(23);
		if (isYes)
		{
			mMusic.StopAllMusic();
			mSoundSystem.CancelPausedFoley();
			KillNewOptionsDialog();
			mBoardResult = BoardResult.Restart;
			if (mBoard != null)
			{
				mKilledYetiAndRestarted = mBoard.mKilledYeti;
			}
			PreNewGame(mGameMode, theLookForSavedGame: false);
		}
	}

	public void FinishAboutDialog(bool isYes)
	{
		KillDialog(52);
	}

	public void FinishRestartWarningDialog(bool isYes)
	{
		KillDialog(53);
	}

	public void FinishPacketSlotPurchaseDialog(bool isYes)
	{
		KillDialog(51);
		if (isYes)
		{
			int itemCost = StoreScreen.GetItemCost(StoreItem.STORE_ITEM_PACKET_UPGRADE);
			mPlayerInfo.AddCoins(-itemCost);
			mPlayerInfo.mPurchases[21]++;
			WriteCurrentUserConfig();
			mBoard.mSeedBank.UpdateHeight();
			if (mCrazyDaveMessageIndex == 1503)
			{
				CrazyDaveTalkIndex(1510);
			}
			else if (mCrazyDaveMessageIndex == 1553)
			{
				CrazyDaveTalkIndex(1560);
			}
		}
		else
		{
			mPlayerInfo.mDidntPurchasePacketUpgrade++;
			if (mCrazyDaveMessageIndex == 1503)
			{
				CrazyDaveTalkIndex(1520);
			}
			else if (mCrazyDaveMessageIndex == 1553)
			{
				CrazyDaveTalkIndex(1570);
			}
		}
	}

	public void FinishTimesUpDialog()
	{
		KillDialog(42);
	}

	public void FinishPlantSale(bool isYes)
	{
		mZenGarden.DoPlantSale(isYes);
		KillDialog(48);
	}

	public void FinishLawnDialogMessageBox(bool isYes)
	{
		if (mLawnMessageBoxListener != null)
		{
			mLawnMessageBoxListener.LawnMessageBoxDone(isYes ? 1000 : 1001);
			mLawnMessageBoxListener = null;
			mWidgetManager.SetFocus(mOldFocus);
			mOldFocus = null;
		}
	}

	public void KillBoard()
	{
		FinishModelessDialogs();
		KillSeedChooserScreen();
		if (mLevelInfoPanel != null)
		{
			mWidgetManager.RemoveWidget(mLevelInfoPanel);
			SafeDeleteWidget(mLevelInfoPanel);
			mLevelInfoPanel = null;
		}
		if (mBoard != null)
		{
			if (mBoard.mLevelComplete || (mGameScene == GameScenes.ZombiesWon && mBoardResult == BoardResult.Lost) || mBoardResult == BoardResult.Won || mBoardResult == BoardResult.Cheat)
			{
				string savedGameName = LawnCommon.GetSavedGameName(mGameMode, (int)mPlayerInfo.mId);
				Sexy.Debug.Log(DebugType.Info, $"{GetType()}: Erasing {savedGameName} as the level {mGameMode} has {mBoardResult}.");
				EraseFile(savedGameName);
			}
			mBoard.DisposeBoard();
			mWidgetManager.RemoveWidget(mBoard);
			SafeDeleteWidget(mBoard);
			mBoard = null;
		}
	}

	public void MakeNewBoard()
	{
		KillBoard();
		mBoard = new Board(this);
		mBoard.Resize(Constants.Board_Offset_AspectRatio_Correction, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mBoard);
		mWidgetManager.BringToBack(mBoard);
		mWidgetManager.SetFocus(mBoard);
		if (mCreativeLevel != null)
		{
			LevelInfoPanel levelInfoPanel = new LevelInfoPanel(this, 2);
			if (levelInfoPanel.HasOverrides)
			{
				mLevelInfoPanel = levelInfoPanel;
				int num = 800;
				mLevelInfoPanel.Resize((mWidth - num) / 2, 0, num, mHeight);
				mWidgetManager.AddWidget(mLevelInfoPanel);
			}
			else
			{
				levelInfoPanel.Dispose();
			}
		}
		GC.Collect();
		SexyAppBase.XnaGame.CompensateForSlowUpdate();
	}

	public void StartPlaying()
	{
		KillSeedChooserScreen();
		mBoard.StartLevel();
		mGameScene = GameScenes.Playing;
	}

	public bool TryLoadGame()
	{
		string savedGameName = LawnCommon.GetSavedGameName(mGameMode, (int)mPlayerInfo.mId);
		mMusic.StopAllMusic();
		if (FileExists(savedGameName))
		{
			MakeNewBoard();
			if (mBoard.LoadGame(savedGameName))
			{
				mFirstTimeGameSelector = false;
				DoContinueDialog();
				return true;
			}
			KillBoard();
		}
		return false;
	}

	internal override void NewGame()
	{
		mFirstTimeGameSelector = false;
		MakeNewBoard();
		mBoard.InitLevel();
		mBoardResult = BoardResult.None;
		mGameScene = GameScenes.LevelIntro;
		ShowSeedChooserScreen();
		mBoard.mCutScene.StartLevelIntro();
	}

	public bool RestoreGame()
	{
		string savedGameName = LawnCommon.GetSavedGameName(mRestoreGameMode, (int)mPlayerInfo.mId);
		mMusic.StopAllMusic();
		if (FileExists(savedGameName))
		{
			SetGameMode(mRestoreGameMode);
			MakeNewBoard();
			if (mBoard.LoadGame(savedGameName))
			{
				if (mBoard.mNextSurvivalStageCounter != 1)
				{
					string savedGameName2 = LawnCommon.GetSavedGameName(mRestoreGameMode, (int)mPlayerInfo.mId);
					EraseFile(savedGameName2);
				}
				mBoard.Pause(thePause: false);
				DoPauseDialog();
				return true;
			}
			KillBoard();
		}
		return false;
	}

	public void RestartLoopingSounds()
	{
		if (mBoard.HasRain())
		{
			PlayFoley(FoleyType.Rain);
		}
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && zombie.mPlayingSong)
			{
				zombie.mPlayingSong = false;
				zombie.StartZombieSound();
			}
		}
	}

	public void PreNewGame(GameMode theGameMode, bool theLookForSavedGame)
	{
		PreNewGame(theGameMode, theLookForSavedGame, checkForTutorialCompletion: true);
	}

	public void PreNewGame(GameMode theGameMode, bool theLookForSavedGame, bool checkForTutorialCompletion)
	{
		if (NeedRegister())
		{
			ShowGameSelector();
			return;
		}
		DelayLoadMainMenuResource(doLoad: false);
		if (theGameMode == GameMode.ChallengeZenGarden)
		{
			DelayLoadZenGardenResources(doLoad: true);
		}
		else
		{
			DelayLoadZenGardenResources(doLoad: false);
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
		DelayLoadGamePlayResources(doLoad: true);
		if (SexyAppBase.IsInTrialMode && mPlayerInfo.mLevel >= 7 && theGameMode != GameMode.ChallengeZenGarden)
		{
			if (mPlayerInfo.mNeedsTrialLevelReset)
			{
				mPlayerInfo.SetLevel(1);
				mPlayerInfo.mNeedsTrialLevelReset = false;
			}
			else
			{
				theGameMode = GameMode.Upsell;
			}
		}
		if ((SexyAppBase.IsInTrialMode & checkForTutorialCompletion) && theGameMode == GameMode.Adventure && mPlayerInfo.mLevel <= 3 && mPlayerInfo.mHasFinishedTutorial && mPlayerInfo.mFinishedAdventure == 0)
		{
			LawnDialog theDialog = DoDialog(58, isModal: true, string.Empty, "[SKIP_TUTORIAL_MESSAGE]", string.Empty, 1);
			CenterDialog(theDialog, (int)Constants.InvertAndScale(400f), (int)Constants.InvertAndScale(200f));
			return;
		}
		SetGameMode(theGameMode);
		ZBCheatDialog.DoCheckGameModeCanSkipLevel(mGameMode);
		if (!theLookForSavedGame || !TryLoadGame())
		{
			string savedGameName = LawnCommon.GetSavedGameName(mGameMode, (int)mPlayerInfo.mId);
			EraseFile(savedGameName);
			NewGame();
			if (mGameMode == GameMode.ChallengeZenGarden && !mPlayerInfo.mZenGardenTutorialComplete)
			{
				mZenGarden.SetupForZenTutorial();
			}
		}
	}

	public void ShowGameSelectorWithOptions()
	{
		ShowGameSelector();
		DoNewOptions(theFromGameSelector: true);
	}

	public void ShowGameSelector()
	{
		KillBoard();
		UpdateRegisterInfo();
		DelayLoadGamePlayResources(doLoad: false);
		DelayLoadMainMenuResource(doLoad: true);
		if (mGameSelector != null)
		{
			mWidgetManager.RemoveWidget(mGameSelector);
			SafeDeleteWidget(mGameSelector);
		}
		mGameScene = GameScenes.Menu;
		mGameSelector = new GameSelector(this);
		mGameSelector.Resize(0, 0, Constants.GameSelector_Width, Constants.GameSelector_Height);
		mWidgetManager.AddWidget(mGameSelector);
		mWidgetManager.BringToBack(mGameSelector);
		mWidgetManager.SetFocus(mGameSelector);
		if (NeedRegister())
		{
			DoNeedRegisterDialog();
		}
	}

	public void KillGameSelector()
	{
		if (mGameSelector != null)
		{
			mWidgetManager.RemoveWidget(mGameSelector);
			SafeDeleteWidget(mGameSelector);
			mGameSelector = null;
		}
	}

	public void Apply500pMode(bool enable)
	{
		if (enable && !IsWideScreen())
		{
			enable = false;
		}
		mPlayerInfo.m500pMode = enable;
		Constants.Is500pMode = enable;
		Constants.Load480x800();
		ZenGarden.RefreshMushroomGridPlacements();
		int screenWidth = mScreenScales.mWidth;
		int screenHeight = mScreenScales.mHeight;
		int y = Constants.BackBufferSize.Y;
		int x = Constants.BackBufferSize.X;
		mScreenScales.Init(screenWidth, screenHeight, y, x);
		Graphics.Resized();
		mWidth = Constants.BOARD_WIDTH;
		mHeight = Constants.BOARD_HEIGHT;
		mWidgetManager.Resize(mWidth, mHeight);
		bool flag = KillNewOptionsDialog();
		KillGameSelector();
		ShowGameSelector();
		if (flag)
		{
			DoNewOptions(theFromGameSelector: true);
		}
	}

	public void ShowGameSelectorQuickPlay(bool theDoFadeIn, GameSelectorButtons theButton)
	{
		ShowGameSelector();
		mGameSelector.mNeedToPlayRollIn = false;
		mGameSelector.MoveToQuickplay(theDoFadeIn, theButton);
	}

	public void ShowGameSelectorMoreGames()
	{
		ShowGameSelector();
		mGameSelector.mNeedToPlayRollIn = false;
		mGameSelector.SwitchToMoreGames(isInstant: true);
	}

	public void ShowGameSelectorQuickPlay(bool theDoFadeIn)
	{
		ShowGameSelectorQuickPlay(theDoFadeIn, GameSelectorButtons.MiniGames);
	}

	protected override void ShowUpdateMessage()
	{
		SexyAppBase.UseLiveServers = false;
		if (!Guide.IsVisible)
		{
			Guide.BeginShowMessageBox(TodStringFile.TodStringTranslate("[UPDATE]"), TodStringFile.TodStringTranslate("[UPDATE_REQUIRED]"), new string[2]
			{
				TodStringFile.TodStringTranslate("[BUTTON_YES]"),
				TodStringFile.TodStringTranslate("[BUTTON_NO]")
			}, 0, MessageBoxIcon.None, GameUpdateMessageClosed, null);
			wantToShowUpdateMessage = false;
		}
	}

	protected override bool ShowRunWhenLockedMessage()
	{
		if (Guide.IsVisible)
		{
			return false;
		}
		if (!TodStringFile.StringsLoaded)
		{
			return false;
		}
		return true;
	}

	private void RunWhenLockedMessageClosed(IAsyncResult result)
	{
		Main.RunWhenLocked = Guide.EndShowMessageBox(result) == 0;
		if (mPlayerInfo != null)
		{
			mPlayerInfo.mRunWhileLocked = Main.RunWhenLocked;
		}
	}

	private void GameUpdateMessageClosed(IAsyncResult result)
	{
		if (Guide.EndShowMessageBox(result) != 0)
		{
			SexyAppBase.UseLiveServers = false;
		}
		else if (Main.IsInTrialMode)
		{
			Guide.ShowMarketplace(PlayerIndex.One);
		}
	}

	public void ShowAwardScreen(AwardType theAwardType, bool theShowAchievements)
	{
		mGameScene = GameScenes.Award;
		mAwardScreen = new AwardScreen(this, theAwardType, theShowAchievements);
		mAwardScreen.UpdateScreen();
		mWidgetManager.AddWidget(mAwardScreen);
		mWidgetManager.BringToBack(mAwardScreen);
		mWidgetManager.SetFocus(mAwardScreen);
	}

	public void KillAwardScreen()
	{
		if (mAwardScreen != null)
		{
			mWidgetManager.RemoveWidget(mAwardScreen);
			SafeDeleteWidget(mAwardScreen);
			mAwardScreen = null;
		}
	}

	public void ShowSeedChooserScreen()
	{
		Sexy.Debug.ASSERT(mSeedChooserScreen == null);
		mSeedChooserScreen = new SeedChooserScreen();
		mSeedChooserScreen.Resize(0, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mSeedChooserScreen);
		mWidgetManager.BringToBack(mSeedChooserScreen);
	}

	public void KillSeedChooserScreen()
	{
		if (mSeedChooserScreen != null)
		{
			mWidgetManager.RemoveWidget(mSeedChooserScreen);
			SafeDeleteWidget(mSeedChooserScreen);
			mSeedChooserScreen = null;
		}
	}

	public void DoBackToMain()
	{
		DoBackToMain(stopMusic: true);
	}

	public void DoBackToMain(bool stopMusic)
	{
		if (stopMusic)
		{
			mMusic.StopAllMusic();
		}
		mSoundSystem.CancelPausedFoley();
		mSoundManager.StopAllSounds();
		WriteCurrentUserConfig();
		KillNewOptionsDialog();
		KillBoard();
		ShowGameSelector();
		mZenGarden.UnloadBackdrop();
	}

	public void DoConfirmBackToMain()
	{
		LawnDialog lawnDialog = DoDialog(22, isModal: true, "[LEAVE_GAME_HEADER]", "[LEAVE_GAME]", "", 1);
		lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[LEAVE_BUTTON]");
		lawnDialog.mLawnNoButton.mLabel = TodStringFile.TodStringTranslate("[DIALOG_BUTTON_CANCEL]");
		lawnDialog.CalcSize(0, 0);
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
	}

	public void DoNewOptions(bool theFromGameSelector)
	{
		FinishModelessDialogs();
		NewOptionsDialog newOptionsDialog = new NewOptionsDialog(this, theFromGameSelector);
		int theWidth = (int)Constants.InvertAndScale(420f);
		int preferredHeight = newOptionsDialog.GetPreferredHeight(theWidth);
		CenterDialog(newOptionsDialog, theWidth, preferredHeight);
		AddDialog(2, newOptionsDialog);
		mWidgetManager.SetFocus(newOptionsDialog);
	}

	public void ShowLeaderboardDialog(LeaderBoardType aType)
	{
		FinishModelessDialogs();
		LeaderboardDialog leaderboardDialog = new LeaderboardDialog(this, aType);
		int theWidth = (int)Constants.InvertAndScale(420f);
		int preferredHeight = leaderboardDialog.GetPreferredHeight(theWidth);
		CenterDialog(leaderboardDialog, theWidth, preferredHeight);
		AddDialog(59, leaderboardDialog);
		mLeaderboardScreen.SetGrayed(aGray: true);
		mWidgetManager.SetFocus(leaderboardDialog);
	}

	public void DoRegister()
	{
	}

	public void DoRegisterError()
	{
		DoDialog(9, isModal: true, "[INVALID_CODE]", "[INVALID_CODE_MESSAGE]", "[DIALOG_BUTTON_OK]", 3);
	}

	public bool CanDoRegisterDialog()
	{
		return true;
	}

	public bool WriteCurrentUserConfig()
	{
		if (mPlayerInfo != null)
		{
			mPlayerInfo.SaveDetails();
		}
		return true;
	}

	public void WriteRestoreInfo()
	{
		RestoreLocation theValue = RestoreLocation.Titlescreen;
		if (mGameSelector != null)
		{
			theValue = RestoreLocation.Mainmenu;
		}
		else if (mBoard != null)
		{
			theValue = RestoreLocation.Board;
			RegistryWriteInteger("RestoreGameMode", (int)mGameMode);
		}
		RegistryWriteInteger("RestoreLocation", (int)theValue);
	}

	public void ReadRestoreInfo()
	{
		mRestoreLocation = RestoreLocation.Mainmenu;
		mRestoreLocation = (RestoreLocation)RegistryReadInteger("RestoreLocation");
		if (mRestoreLocation == RestoreLocation.Board)
		{
			mRestoreGameMode = (GameMode)RegistryReadInteger("RestoreGameMode");
		}
	}

	public void DoLockedAchievementDialog(AchievementId theId)
	{
		string fmt = TodStringFile.TodStringTranslate("[UNLOCK_TO_EARN]");
		string theDialogLines = Common.StrFormat_(fmt, Achievements.GetAchievementItem(theId).Name);
		LawnDialog lawnDialog = DoDialog(55, isModal: true, "[ACHIEVEMENT_UNLOCKED]", theDialogLines, "", 2);
		lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[CONTINUE_BUTTON]");
		lawnDialog.mLawnNoButton.mLabel = TodStringFile.TodStringTranslate("[GET_FULL_VERSION_BUTTON]");
		lawnDialog.CalcSize(200, 10);
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
	}

	public void DoNeedRegisterDialog()
	{
		LawnDialog lawnDialog = DoDialog(5, isModal: true, "[REGISTER_HEADER]", "[REGISTER]", "", 2);
		lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[REGISTER_BUTTON]");
		lawnDialog.mLawnNoButton.mLabel = TodStringFile.TodStringTranslate("[QUIT_BUTTON]");
	}

	public void DoContinueDialog()
	{
		ContinueDialog continueDialog = new ContinueDialog(this);
		CenterDialog(continueDialog, continueDialog.mWidth, continueDialog.mHeight);
		AddDialog(37, continueDialog);
	}

	public void DoPauseDialog()
	{
		mBoard.Pause(thePause: true);
		FinishModelessDialogs();
		LawnDialog lawnDialog = DoDialog(19, isModal: true, "[GAME_PAUSED]", "", "[RESUME_GAME]", 3);
		int num = Math.Max(Resources.FONT_DWARVENTODCRAFT15.StringWidth(TodStringFile.TodStringTranslate("[GAME_PAUSED]")), Resources.FONT_DWARVENTODCRAFT15.StringWidth(TodStringFile.TodStringTranslate("[RESUME_GAME]")));
		if ((float)num < Constants.InvertAndScaleUI(125f))
		{
			num = (int)Constants.InvertAndScaleUI(125f);
		}
		int num2 = AtlasResources.IMAGE_DIALOG_TOPLEFT.mWidth + num + AtlasResources.IMAGE_DIALOG_TOPRIGHT.mWidth;
		lawnDialog.mReanimation.AddReanimation((float)(num2 / 2) - Constants.InvertAndScaleUI(85f), Constants.InvertAndScaleUI(30f), ReanimationType.ZombieNewspaper);
		lawnDialog.mSpaceAfterHeader = (int)Constants.InvertAndScaleUI(65f);
		lawnDialog.CalcSize((int)Constants.InvertAndScaleUI(20f), (int)Constants.InvertAndScaleUI(10f), num);
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
		if (mBoard.mCursorObject.mCursorType == CursorType.Hammer)
		{
			EnforceCursor();
		}
	}

	public void FinishModelessDialogs()
	{
	}

	public LawnDialog DoDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
	{
		TodStringFile.TodStringTranslate(theDialogHeader);
		TodStringFile.TodStringTranslate(theDialogLines);
		TodStringFile.TodStringTranslate(theDialogFooter);
		LawnDialog lawnDialog = new LawnDialog(this, null, theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode);
		base.DoDialog(lawnDialog, theDialogId);
		if (mWidgetManager.mFocusWidget == null)
		{
			mWidgetManager.mFocusWidget = lawnDialog;
		}
		Sexy.Debug.Log(DebugType.Debug, $"{lawnDialog.GetType()}: DoDialog");
		return lawnDialog;
	}

	public LawnDialog DoDialogDelay(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
	{
		LawnDialog lawnDialog = new LawnDialog(this, null, theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode);
		base.DoDialog(lawnDialog, theDialogId);
		lawnDialog.SetButtonDelay(30);
		return lawnDialog;
	}

	public override void Shutdown()
	{
		if (!mLoadingThreadCompleted)
		{
			mLoadingFailed = true;
		}
		else if (!mShutdown)
		{
			for (int i = 0; i < 60; i++)
			{
				KillDialog(i);
			}
			if (mBoard != null)
			{
				mBoardResult = BoardResult.QuitApp;
				mWidgetManager.mDownButtons = 0;
				mBoard.TryToSaveGame();
				KillBoard();
				WriteCurrentUserConfig();
			}
			mProfileMgr.Save();
			ProcessSafeDeleteList();
			if (mZenGarden != null)
			{
				mZenGarden = null;
			}
			if (mEffectSystem != null)
			{
				mEffectSystem.EffectSystemDispose();
				mEffectSystem.Dispose();
				mEffectSystem = null;
			}
			if (mReanimatorCache != null)
			{
				mReanimatorCache.ReanimatorCacheDispose();
				mReanimatorCache = null;
			}
			FilterEffect.FilterEffectDisposeForApp();
			TodParticleGlobal.TodParticleFreeDefinitions();
			ReanimatorXnaHelpers.ReanimatorFreeDefinitions();
			Coin.CoinFreeTextures();
			GlobalMembersTrail.TrailFreeDefinitions();
			UpdateRegisterInfo();
			base.Shutdown();
		}
	}

	public override void Init()
	{
		bool flag = mTodCheatKeys;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#1: mSessionID = (int)DateTime.UtcNow.Ticks;";
		mSessionID = (int)DateTime.UtcNow.Ticks;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#2: mPlayTimeActiveSession = 0;";
		mPlayTimeActiveSession = 0;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#3: mPlayTimeInactiveSession = 0;";
		mPlayTimeInactiveSession = 0;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#4: mBoardResult = BoardResult.None;";
		mBoardResult = BoardResult.None;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#5: mKilledYetiAndRestarted = false;";
		mKilledYetiAndRestarted = false;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#6: base.Init();";
		base.Init();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#7: ReadRestoreInfo();";
		ReadRestoreInfo();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#8: if (!mResourceManager.ParseResourcesFile(Content/resources.";
		if (!mResourceManager.ParseResourcesFile("Content/resources.xml"))
		{
			ShowResourceError(true);
			return;
		}
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#9: if (Constants.Language != Constants.LanguageIndex.de)";
		if (Constants.Language != Constants.LanguageIndex.de)
		{
			if (!TodCommon.TodLoadResources("Init"))
			{
				return;
			}
		}
		else if (!TodCommon.TodLoadResources("InitRegistered"))
		{
			return;
		}
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#10: Resources.ExtractInitResources(mResourceManager);";
		Resources.ExtractInitResources(mResourceManager);
		PerfTimer perfTimer = default;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#11: perfTimer.Start();";
		perfTimer.Start();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#12: mProfileMgr.Load();";
		mProfileMgr.Load();
		string text = string.Empty;
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#13: if (mPlayerInfo == null)";
		if (mPlayerInfo == null)
		{
			if (RegistryReadString("CurUser", text) == null)
			{
				string obj = SexyAppBase.XnaGame.mGameConfig.mCurrentUser ?? string.Empty;
				text = obj;
				if (string.IsNullOrEmpty(obj))
				{
					goto IL_00f5;
				}
			}
			mPlayerInfo = mProfileMgr.GetProfile(text);
		}
		goto IL_00f5;
		IL_00f5:
		try
		{
		}
		catch (Exception)
		{
		}
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#14: if (mPlayerInfo == null)";
		if (mPlayerInfo == null)
		{
			PlayerInfo anyProfile = mProfileMgr.GetAnyProfile();
			try
			{
				if (!Microsoft.Xna.Framework.Media.MediaPlayer.GameHasControl)
				{
					anyProfile.mMusicVolume = Microsoft.Xna.Framework.Media.MediaPlayer.Volume;
				}
			}
			catch (Exception)
			{
			}
			mPlayerInfo = anyProfile;
		}
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#15: mMaxExecutions = GetInteger(MaxExecutions, 0);";
		mMaxExecutions = GetInteger("MaxExecutions", 0);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#16: mMaxPlays = GetInteger(MaxPlays, 0);";
		mMaxPlays = GetInteger("MaxPlays", 0);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#17: mMaxTime = GetInteger(MaxTime, 60);";
		mMaxTime = GetInteger("MaxTime", 60);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#18: mTitleScreen = new TitleScreen(this);";
		mTitleScreen = new TitleScreen(this);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#19: mTitleScreen.Resize(-235, 0, mWidth + 470, mHeight);";
		mTitleScreen.Resize(-235, 0, mWidth + 470, mHeight);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#20: mWidgetManager.AddWidget(mTitleScreen);";
		mWidgetManager.AddWidget(mTitleScreen);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#21: mWidgetManager.SetFocus(mTitleScreen);";
		mWidgetManager.SetFocus(mTitleScreen);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#22: perfTimer.Start();";
		perfTimer.Start();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#23: mMusic = new Music();";
		mMusic = new Music();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#24: mSoundSystem = new TodFoley();";
		mSoundSystem = new TodFoley();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#25: mEffectSystem = new EffectSystem();";
		mEffectSystem = new EffectSystem();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#26: mEffectSystem.EffectSystemInitialize();";
		mEffectSystem.EffectSystemInitialize();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#27: FilterEffect.FilterEffectInitForApp();";
		FilterEffect.FilterEffectInitForApp();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#28: Reanimation.ReanimSpineInit(GlobalStaticVars.g, FilterEffect";
		Reanimation.ReanimSpineInit(GlobalStaticVars.g, FilterEffectType.None);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#29: Reanimation.ReanimSpineInit(GlobalStaticVars.g, FilterEffect";
		Reanimation.ReanimSpineInit(GlobalStaticVars.g, FilterEffectType.White);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#30: Reanimation.ReanimSpineInit(GlobalStaticVars.g, FilterEffect";
		Reanimation.ReanimSpineInit(GlobalStaticVars.g, FilterEffectType.WashedOut);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#31: mKonamiCheck = new TypingCheck();";
		mKonamiCheck = new TypingCheck();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#32: mKonamiCheck.AddKeyCode(KeyCode.Up);";
		mKonamiCheck.AddKeyCode(KeyCode.Up);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#33: mKonamiCheck.AddKeyCode(KeyCode.Up);";
		mKonamiCheck.AddKeyCode(KeyCode.Up);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#34: mKonamiCheck.AddKeyCode(KeyCode.Down);";
		mKonamiCheck.AddKeyCode(KeyCode.Down);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#35: mKonamiCheck.AddKeyCode(KeyCode.Down);";
		mKonamiCheck.AddKeyCode(KeyCode.Down);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#36: mKonamiCheck.AddKeyCode(KeyCode.Left);";
		mKonamiCheck.AddKeyCode(KeyCode.Left);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#37: mKonamiCheck.AddKeyCode(KeyCode.Right);";
		mKonamiCheck.AddKeyCode(KeyCode.Right);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#38: mKonamiCheck.AddKeyCode(KeyCode.Left);";
		mKonamiCheck.AddKeyCode(KeyCode.Left);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#39: mKonamiCheck.AddKeyCode(KeyCode.Right);";
		mKonamiCheck.AddKeyCode(KeyCode.Right);
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#40: mKonamiCheck.AddChar('b');";
		mKonamiCheck.AddChar('b');
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#41: mKonamiCheck.AddChar('a');";
		mKonamiCheck.AddChar('a');
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#42: mMustacheCheck = new TypingCheck(mustache);";
		mMustacheCheck = new TypingCheck("mustache");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#43: mMoustacheCheck = new TypingCheck(moustache);";
		mMoustacheCheck = new TypingCheck("moustache");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#44: mSuperMowerCheck = new TypingCheck(trickedout);";
		mSuperMowerCheck = new TypingCheck("trickedout");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#45: mSuperMowerCheck2 = new TypingCheck(tricked out);";
		mSuperMowerCheck2 = new TypingCheck("tricked out");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#46: mFutureCheck = new TypingCheck(future);";
		mFutureCheck = new TypingCheck("future");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#47: mPinataCheck = new TypingCheck(pinata);";
		mPinataCheck = new TypingCheck("pinata");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#48: mDanceCheck = new TypingCheck(dance);";
		mDanceCheck = new TypingCheck("dance");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#49: mDaisyCheck = new TypingCheck(daisies);";
		mDaisyCheck = new TypingCheck("daisies");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#50: mSukhbirCheck = new TypingCheck(sukhbir);";
		mSukhbirCheck = new TypingCheck("sukhbir");
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#51: perfTimer.Start();";
		perfTimer.Start();
		LawnPlatform.PvZActivity.Breadcrumb = "LawnApp.Init#52: perfTimer.Start();";
		perfTimer.Start();
	}

	public override void Start()
	{
		if (!mLoadingFailed)
		{
			base.Start();
		}
	}

	public string RegistryReadString(string key, string value)
	{
		return null;
	}

	public virtual Sexy.Dialog NewDialog(int theDialogId, bool isModal, string theDialogHeader, string theDialogLines, string theDialogFooter, int theButtonMode)
	{
		LawnDialog lawnDialog = new LawnDialog(this, null, theDialogId, isModal, theDialogHeader, theDialogLines, theDialogFooter, theButtonMode);
		if (lawnDialog.mWidth < 380)
		{
			lawnDialog.mWidth = 380;
		}
		if (lawnDialog.mHeight > 320)
		{
			lawnDialog.mHeight = 320;
		}
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
		return lawnDialog;
	}

	public override bool KillDialog(int theDialogId)
	{
		if (base.KillDialog(theDialogId))
		{
			if (mDialogMap.Count == 0)
			{
				if (mBoard != null)
				{
					mWidgetManager.SetFocus(mBoard);
				}
				else if (mGameSelector != null)
				{
					mWidgetManager.SetFocus(mGameSelector);
				}
			}
			if (mBoard != null && !NeedPauseGame())
			{
				mBoard.Pause(thePause: false);
			}
			return true;
		}
		return false;
	}

	public override void ModalOpen()
	{
		if (mBoard != null && NeedPauseGame())
		{
			mBoard.Pause(thePause: true);
		}
	}

	public override void ModalClose()
	{
	}

	public void PreDisplayHook()
	{
		PreDisplayHook();
	}

	public bool ChangeDirHook(string theIntendedPath)
	{
		return false;
	}

	public bool NeedRegister()
	{
		return false;
	}

	public void UpdateRegisterInfo()
	{
	}

	public override void ButtonPress(int theId)
	{
	}

	public override void ButtonDepress(int theId)
	{
		if (theId % 10000 >= 2000 && theId % 10000 < 3000)
		{
			switch (theId - 2000)
			{
			case 0:
				KillDialog(0);
				ShowGameSelector();
				break;
			case 2:
				KillNewOptionsDialog();
				if (mBoard != null)
				{
					RestartLoopingSounds();
				}
				break;
			case 5:
				DoRegister();
				break;
			case 6:
				break;
			case 7:
				KillDialog(7);
				CheckForUpdates();
				break;
			case 13:
				KillDialog(13);
				CloseRequestAsync();
				break;
			case 15:
				KillDialog(15);
				DoRegister();
				break;
			case 16:
				KillDialog(16);
				break;
			case 19:
				KillDialog(19);
				break;
			case 20:
				KillDialog(20);
				mBoard.AddSunMoney(100);
				break;
			case 21:
				KillDialog(21);
				break;
			case 22:
				KillDialog(22);
				mBoardResult = BoardResult.Quit;
				mBoard.TryToSaveGame();
				DoBackToMain();
				break;
			case 23:
				FinishInGameRestartConfirmDialog(isYes: true);
				break;
			case 28:
			case 49:
			case 66:
				KillDialog(theId - 2000);
				FinishLawnDialogMessageBox(isYes: true);
				break;
			case 29:
				FinishUserDialog(isYes: true);
				break;
			case 30:
				FinishCreateUserDialog(isYes: true);
				break;
			case 31:
				FinishConfirmDeleteUserDialog(isYes: true);
				break;
			case 32:
				FinishRenameUserDialog(isYes: true);
				break;
			case 33:
				FinishNameError(theId - 2000);
				break;
			case 34:
				FinishNameError(theId - 2000);
				break;
			case 35:
				FinishCheatDialog(isYes: true);
				break;
			case 39:
				FinishRestartConfirmDialog();
				break;
			case 42:
				FinishTimesUpDialog();
				break;
			case 46:
				((StoreScreen)GetDialog(4)).PurchasePendingItem();
				break;
			case 47:
				((StoreScreen)GetDialog(4)).FinishTreeOfWisdomDialog(isYes: true);
				break;
			case 48:
				FinishPlantSale(isYes: true);
				break;
			case 51:
				FinishPacketSlotPurchaseDialog(isYes: true);
				break;
			case 52:
				FinishAboutDialog(isYes: true);
				break;
			case 53:
				FinishRestartWarningDialog(isYes: true);
				break;
			case 55:
				KillDialog(55);
				break;
			case 58:
				KillDialog(theId - 2000);
				PreNewGame(GameMode.Adventure, theLookForSavedGame: true, checkForTutorialCompletion: false);
				mPlayerInfo.mHasFinishedTutorial = false;
				break;
			case 60:
				KillDialog(theId - 2000);
				mGameSelector?.UnlockGame();
				break;
			case 68:
				KillDialog(theId - 2000);
				mGameSelector?.FinishAdventureHardConfirmDialog();
				break;
			case 20008:
				KillDialog(20008);
				KillDialog(8);
				break;
			default:
				KillDialog(theId - 2000);
				break;
			}
		}
		else if (theId % 10000 >= 3000 && theId % 10000 < 4000)
		{
			switch (theId - 3000)
			{
			case 5:
				KillDialog(5);
				Shutdown();
				break;
			case 6:
				KillDialog(6);
				break;
			case 23:
				FinishInGameRestartConfirmDialog(isYes: false);
				break;
			case 29:
				FinishUserDialog(isYes: false);
				break;
			case 30:
				FinishCreateUserDialog(isYes: false);
				break;
			case 31:
				FinishConfirmDeleteUserDialog(isYes: false);
				break;
			case 32:
				FinishRenameUserDialog(isYes: false);
				break;
			case 35:
				FinishCheatDialog(isYes: false);
				break;
			case 42:
				FinishTimesUpDialog();
				break;
			case 47:
				((StoreScreen)GetDialog(4)).FinishTreeOfWisdomDialog(isYes: false);
				break;
			case 48:
				FinishPlantSale(isYes: false);
				break;
			case 51:
				FinishPacketSlotPurchaseDialog(isYes: false);
				break;
			case 55:
				checkGiveAchievements = true;
				BuyGame();
				break;
			case 66:
				KillDialog(theId - 3000);
				FinishLawnDialogMessageBox(isYes: false);
				break;
			case 58:
				KillDialog(theId - 3000);
				mPlayerInfo.SetLevel(4);
				PreNewGame(GameMode.Adventure, theLookForSavedGame: false, checkForTutorialCompletion: false);
				break;
			case 10008:
				KillDialog(10008);
				KillDialog(8);
				break;
			case 28:
			case 49:
				KillDialog(theId - 3000);
				FinishLawnDialogMessageBox(isYes: false);
				break;
			default:
				KillDialog(theId - 3000);
				break;
			}
		}
	}

	public override void LeftTrialMode()
	{
		base.LeftTrialMode();
		if (mGameSelector != null)
		{
			mGameSelector.SyncButtons();
		}
	}

	public override void UpdateFrames()
	{
		if (wantToShowUpdateMessage)
		{
			ShowUpdateMessage();
		}
		if (LoadingScreen.IsLoading)
		{
			LoadingScreen.gLoadingScreen.Update();
			return;
		}
		UpdatePlayTimeStats();
		int num = 1;
		if (GlobalStaticVars.gSlowMo)
		{
			GlobalStaticVars.gSlowMoCounter++;
			if (GlobalStaticVars.gSlowMoCounter >= ((GlobalStaticVars.gFastSlowMoNum != 0) ? GlobalStaticVars.gFastSlowMoNum : 4))
			{
				GlobalStaticVars.gSlowMoCounter = 0;
			}
			else
			{
				num = 0;
			}
		}
		else if (GlobalStaticVars.gFastMo)
		{
			num = ((GlobalStaticVars.gFastSlowMoNum != 0) ? GlobalStaticVars.gFastSlowMoNum : 20);
		}
		for (int i = 0; i < num; i++)
		{
			mAppCounter++;
			if (mBoard != null)
			{
				mBoard.ProcessDeleteQueue();
			}
			mMusic.MusicUpdate();
			base.UpdateFrames();
			if (mLoadingThreadCompleted && mEffectSystem != null)
			{
				mEffectSystem.ProcessDeleteQueue();
			}
			CheckForGameEnd();
		}
	}

	public bool IsAdventureMode()
	{
		return mGameMode == GameMode.Adventure;
	}

	public bool IsQuickPlayMode()
	{
		if (mGameMode >= GameMode.Quickplay1)
		{
			return mGameMode < GameMode.ChallengeZombiquarium;
		}
		return false;
	}

	public bool IsSurvivalMode()
	{
		if ((mGameMode < GameMode.SurvivalNormalStage1 || mGameMode >= GameMode.ChallengeWarAndPeas) && mGameMode != GameMode.RogueConveyorbelt && mGameMode != GameMode.RogueConveyorbeltHard && mGameMode != GameMode.SurvivalHellStage1 && mGameMode != GameMode.SurvivalHellStage2 && mGameMode != GameMode.SurvivalHellStage3 && mGameMode != GameMode.SurvivalHellStage4 && mGameMode != GameMode.SurvivalHellStage5 && mGameMode != GameMode.BigPoolSurvivalNormalStage && mGameMode != GameMode.BigPoolSurvivalHardStage && mGameMode != GameMode.BigPoolSurvivalHellStage)
		{
			return mCreativeLevel?.GetComponent<CSSurvivalLevel>() != null;
		}
		return true;
	}

	public bool IsContinuousChallenge()
	{
		if (!IsArtChallenge() && !IsSlotMachineLevel() && !IsFinalBossLevel() && mGameMode != GameMode.ChallengeBeghouled && mGameMode != GameMode.Upsell && mGameMode != GameMode.Intro)
		{
			return mGameMode == GameMode.ChallengeBeghouledTwist;
		}
		return true;
	}

	public bool IsArtChallenge()
	{
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeArtChallenge1 && mGameMode != GameMode.ChallengeArtChallenge2)
			{
				return mGameMode == GameMode.ChallengeSeeingStars;
			}
			return true;
		}
		return false;
	}

	public bool NeedPauseGame()
	{
		if (mDialogList.Count == 0)
		{
			return false;
		}
		int num = 0;
		if (mDialogList.Count == 1 && mDialogList.First.Value.mId != 0)
		{
			num = mDialogList.First.Value.mId;
		}
		if (num != 28 && num != 51 && num != 50 && (mBoard == null || mGameMode != GameMode.ChallengeZenGarden))
		{
			if (mBoard != null)
			{
				return mGameMode != GameMode.TreeOfWisdom;
			}
			return true;
		}
		return false;
	}

	public void ShowResourceError()
	{
		ShowResourceError(false);
	}

	public override void ShowResourceError(bool doExit)
	{
		base.ShowResourceError(doExit);
	}

	public void ToggleSlowMo()
	{
		GlobalStaticVars.gSlowMoCounter = 0;
		GlobalStaticVars.gSlowMo = !GlobalStaticVars.gSlowMo;
		GlobalStaticVars.gFastMo = false;
	}

	public void ToggleFastMo()
	{
		GlobalStaticVars.gFastMo = !GlobalStaticVars.gFastMo;
		GlobalStaticVars.gSlowMo = false;
	}

	public void PlayFoley(FoleyType theFoleyType)
	{
		if (!mMuteSoundsForCutscene)
		{
			mSoundSystem.PlayFoley(theFoleyType);
		}
	}

	public void PlayFoleyPitch(FoleyType theFoleyType, float aPitch)
	{
		if (!mMuteSoundsForCutscene)
		{
			mSoundSystem.PlayFoleyPitch(theFoleyType, aPitch);
		}
	}

	public void FastLoad(GameMode theGameMode)
	{
		if (!mShutdown)
		{
			mWidgetManager.RemoveWidget(mTitleScreen);
			SafeDeleteWidget(mTitleScreen);
			mTitleScreen = null;
			PreNewGame(theGameMode, theLookForSavedGame: false);
		}
	}

	public string GetStageString(int theLevel)
	{
		if (!cachedStageStrings.TryGetValue(theLevel, out var value))
		{
			int num = TodCommon.ClampInt((theLevel - 1) / 10 + 1, 1, 6);
			int num2 = theLevel - (num - 1) * 10;
			value = Common.StrFormat_(TodStringFile.TodStringTranslate("[STAGE_STRING]"), num, num2);
			cachedStageStrings.Add(theLevel, value);
		}
		return value;
	}

	public void KillChallengeScreen()
	{
		if (mChallengeScreen != null)
		{
			mWidgetManager.RemoveWidget(mChallengeScreen);
			SafeDeleteWidget(mChallengeScreen);
			mChallengeScreen = null;
		}
	}

	public void ShowChallengeScreen(ChallengePage thePage)
	{
		mGameScene = GameScenes.Challenge;
		mChallengeScreen = new ChallengeScreen(this, thePage);
		mChallengeScreen.Resize(0, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mChallengeScreen);
		mWidgetManager.BringToBack(mChallengeScreen);
		mWidgetManager.SetFocus(mChallengeScreen);
	}

	public void ShowLeaderboardScreen()
	{
		mGameScene = GameScenes.Leaderboard;
		mLeaderboardScreen = new LeaderboardScreen(this);
		mLeaderboardScreen.Resize(0, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mLeaderboardScreen);
		mWidgetManager.BringToBack(mLeaderboardScreen);
		mWidgetManager.SetFocus(mLeaderboardScreen);
	}

	public void KillLeaderboardScreen()
	{
		if (mLeaderboardScreen != null)
		{
			mLeaderboardScreen.UnloadResources();
			mWidgetManager.RemoveWidget(mLeaderboardScreen);
			SafeDeleteWidget(mLeaderboardScreen);
			mLeaderboardScreen = null;
		}
	}

	public void CheckForGameEnd()
	{
		if (mBoard == null || !mBoard.mLevelComplete)
		{
			return;
		}
		bool flag = mBoard.CheckForPostGameAchievements();
		flag = false;
		UpdatePlayerProfileForFinishingLevel();
		if (IsAdventureMode())
		{
			int mLevel = mBoard.mLevel;
			KillBoard();
			if (IsFirstTimeAdventureMode() && mLevel < 50)
			{
				ShowAwardScreen(AwardType.ForLevel, flag);
				return;
			}
			switch (mLevel)
			{
			case 50:
				if (mPlayerInfo.mFinishedAdventure != 1)
				{
					ShowAwardScreen(AwardType.ForLevel, flag);
				}
				else
				{
					ShowAwardScreen(AwardType.PreCreditsZombieNote, flag);
				}
				break;
			case 9:
			case 19:
			case 29:
			case 39:
			case 49:
				ShowAwardScreen(AwardType.ForLevel, flag);
				break;
			default:
				if (flag)
				{
					ShowAwardScreen(AwardType.AchievementOnly, theShowAchievements: true);
				}
				else
				{
					PreNewGame(mGameMode, theLookForSavedGame: false);
				}
				break;
			}
		}
		else if (IsQuickPlayMode())
		{
			KillBoard();
			if (flag)
			{
				ShowAwardScreen(AwardType.AchievementOnly, flag);
			}
			else
			{
				ShowGameSelectorQuickPlay(theDoFadeIn: false);
			}
		}
		else if (IsSurvivalMode() && !mBoard.IsFinalSurvivalStage())
		{
			mBoard.mChallenge.mSurvivalStage++;
			KillSeedChooserScreen();
			mBoard.InitSurvivalStage();
		}
		else if (IsOnlineLevelMode())
		{
			KillBoard();
			ShowChallengeScreen(ChallengePage.OnlineLevel);
		}
		else if (IsDIYMode())
		{
			KillBoard();
			ShowChallengeScreen(ChallengePage.DIY);
		}
		else if (IsSurvivalMode())
		{
			KillBoard();
			ShowChallengeScreen(ChallengePage.Survival);
		}
		else if (!IsPuzzleMode())
		{
			KillBoard();
			if (mGameMode >= GameMode.ChallengeAttackOnTitans)
			{
				ShowChallengeScreen(ChallengePage.Extra);
			}
			else
			{
				ShowChallengeScreen(ChallengePage.Challenge);
			}
		}
		else
		{
			bool flag2 = IsIZombieLevel();
			KillBoard();
			ShowChallengeScreen(ChallengePage.Puzzle);
		}
	}

	public virtual void CloseRequestAsync()
	{
		WantsToExit = true;
	}

	public bool IsChallengeWithoutSeedBank()
	{
		if (mCreativeLevel != null)
		{
			if (mCreativeLevel.GetComponent<CSSeedBank>() == null && mCreativeLevel.GetComponent<CSConveyorBelt>() == null)
			{
				return mCreativeLevel.GetComponent<CSSlotMachineLevel>() == null;
			}
			return false;
		}
		if (mGameMode != GameMode.ChallengeRainingSeeds && mGameMode != GameMode.Upsell && mGameMode != GameMode.Intro && !IsWhackAZombieLevel() && !IsSquirrelLevel() && !IsScaryPotterLevel() && mGameMode != GameMode.ChallengeZenGarden)
		{
			return mGameMode == GameMode.TreeOfWisdom;
		}
		return true;
	}

	public AlmanacDialog DoAlmanacDialog(SeedType theSeedType, ZombieType theZombieType, AlmanacListener theListener)
	{
		default(PerfTimer).Start();
		FinishModelessDialogs();
		AlmanacDialog almanacDialog = new AlmanacDialog(this, theListener);
		almanacDialog.UpdateScreen();
		AddDialog(3, almanacDialog);
		mWidgetManager.SetFocus(almanacDialog);
		if (theSeedType != SeedType.None)
		{
			almanacDialog.ShowPlant(theSeedType);
		}
		else if (theZombieType != ZombieType.Invalid)
		{
			almanacDialog.ShowZombie(theZombieType);
		}
		return almanacDialog;
	}

	public bool KillAlmanacDialog()
	{
		if ((AlmanacDialog)GetDialog(Dialogs.DIALOG_ALMANAC) == null)
		{
			return false;
		}
		KillDialog(3);
		return true;
	}

	public int GetSeedsAvailable()
	{
		int level = mPlayerInfo.GetLevel();
		if (HasFinishedAdventure() || level > 50)
		{
			return 49;
		}
		int awardSeedForLevel = (int)GetAwardSeedForLevel(level);
		return Math.Min(49, awardSeedForLevel);
	}

	public Reanimation AddReanimation(float theX, float theY, int aRenderOrder, ReanimationType theReanimationType)
	{
		return AddReanimation(theX, theY, aRenderOrder, theReanimationType, theDoScalePos: true);
	}

	public Reanimation AddReanimation(float theX, float theY, int aRenderOrder, ReanimationType theReanimationType, bool theDoScalePos)
	{
		if (theDoScalePos)
		{
			theX *= Constants.S;
			theY *= Constants.S;
		}
		return mEffectSystem.mReanimationHolder.AllocReanimation(theX, theY, aRenderOrder, theReanimationType);
	}

	public TodParticleSystem AddTodParticle(float theX, float theY, int aRenderOrder, ParticleEffect theEffect)
	{
		return mEffectSystem.mParticleHolder.AllocParticleSystem(theX, theY, aRenderOrder, theEffect);
	}

	public TodParticleSystem ParticleGetID(TodParticleSystem theParticle)
	{
		return theParticle;
	}

	public TodParticleSystem ParticleGet(TodParticleSystem theParticleID)
	{
		return theParticleID;
	}

	public TodParticleSystem ParticleTryToGet(TodParticleSystem theParticleID)
	{
		if (theParticleID == null || !theParticleID.mActive)
		{
			return null;
		}
		return theParticleID;
	}

	public Reanimation ReanimationGetID(Reanimation theReanimation)
	{
		if (theReanimation == null || theReanimation.mDead)
		{
			return null;
		}
		return theReanimation;
	}

	public Reanimation ReanimationGet(Reanimation theReanimID)
	{
		if (theReanimID == null || !theReanimID.mActive)
		{
			return mEffectSystem.mReanimationHolder.DUMMY;
		}
		return theReanimID;
	}

	public Reanimation ReanimationTryToGet(Reanimation theReanimID)
	{
		if (theReanimID == null || !theReanimID.mActive)
		{
			return null;
		}
		return theReanimID;
	}

	public void RemoveReanimation(ref Reanimation theReanimationID)
	{
		ReanimationTryToGet(theReanimationID)?.ReanimationDie();
		theReanimationID = null;
	}

	public void RemoveParticle(TodParticleSystem theParticleID)
	{
		ParticleTryToGet(theParticleID)?.ParticleSystemDie();
	}

	public StoreScreen ShowStoreScreen(StoreListener theListener)
	{
		DelayLoadGamePlayResources(doLoad: false);
		DelayLoadCachedResources(doLoad: true);
		DelayLoadMainMenuResource(doLoad: true);
		DelayLoadZenGardenResources(doLoad: true);
		FinishModelessDialogs();
		Sexy.Debug.ASSERT(GetDialog(Dialogs.DIALOG_STORE) == null);
		StoreScreen storeScreen = new StoreScreen(this, theListener);
		storeScreen.UpdateScreen();
		AddDialog(4, storeScreen);
		mWidgetManager.SetFocus(storeScreen);
		return storeScreen;
	}

	public UpsellScreen ShowUpsellScreen()
	{
		FinishModelessDialogs();
		UpsellScreen upsellScreen = new UpsellScreen(this);
		AddDialog(54, upsellScreen);
		mWidgetManager.SetFocus(upsellScreen);
		return upsellScreen;
	}

	public void KillStoreScreen()
	{
		if ((AlmanacDialog)GetDialog(Dialogs.DIALOG_STORE) != null)
		{
			KillDialog(4);
		}
	}

	public bool HasSeedType(SeedType theSeedType)
	{
		if (theSeedType == SeedType.Gatlingpea)
		{
			return mPlayerInfo.mPurchases[0] > 0;
		}
		if (IsTrialStageLocked() && theSeedType >= SeedType.Jalapeno)
		{
			return false;
		}
		return theSeedType switch
		{
			SeedType.Twinsunflower => mPlayerInfo.mPurchases[1] > 0, 
			SeedType.Gloomshroom => mPlayerInfo.mPurchases[2] > 0, 
			SeedType.Cattail => mPlayerInfo.mPurchases[3] > 0, 
			SeedType.Wintermelon => mPlayerInfo.mPurchases[4] > 0, 
			SeedType.GoldMagnet => mPlayerInfo.mPurchases[5] > 0, 
			SeedType.Spikerock => mPlayerInfo.mPurchases[6] > 0, 
			SeedType.Cobcannon => mPlayerInfo.mPurchases[7] > 0, 
			SeedType.Imitater => mPlayerInfo.mPurchases[8] > 0, 
			SeedType.SuperChomper => mPlayerInfo.mPurchases[31] > 0, 
			SeedType.PickledPepper => mPlayerInfo.mPurchases[32] > 0, 
			SeedType.FireShroom => mPlayerInfo.mPurchases[33] > 0, 
			SeedType.Agave => mPlayerInfo.mPurchases[34] > 0, 
			SeedType.Endoflame => true, 
			_ => (int)theSeedType < GetSeedsAvailable(), 
		};
	}

	public void EndLevel()
	{
		KillBoard();
		if (IsAdventureMode())
		{
			NewGame();
		}
	}

	public bool IsIceDemo()
	{
		return false;
	}

	public bool IsShovelLevel()
	{
		if (mBoard != null)
		{
			return mGameMode == GameMode.ChallengeShovel;
		}
		return false;
	}

	public bool IsWallnutBowlingLevel()
	{
		if (mCreativeLevel?.GetComponent<CSWallnutBowlingLevel>() != null)
		{
			return true;
		}
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeWallnutBowling && mGameMode != GameMode.ChallengeWallnutBowling2)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 5)
				{
					return mGameMode == GameMode.Quickplay5;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsMiniBossLevel()
	{
		if (mBoard != null)
		{
			if ((!IsAdventureMode() || mPlayerInfo.mLevel != 10) && mGameMode != GameMode.Quickplay10 && (!IsAdventureMode() || mPlayerInfo.mLevel != 20) && mGameMode != GameMode.Quickplay20)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 30)
				{
					return mGameMode == GameMode.Quickplay30;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsSlotMachineLevel()
	{
		if (mCreativeLevel?.GetComponent<CSSlotMachineLevel>() != null)
		{
			return true;
		}
		if (mBoard != null)
		{
			return mGameMode == GameMode.ChallengeSlotMachine;
		}
		return false;
	}

	public bool IsLittleTroubleLevel()
	{
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeLittleTrouble)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 25)
				{
					return mGameMode == GameMode.Quickplay25;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsStormyNightLevel()
	{
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeStormyNight)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 40)
				{
					return mGameMode == GameMode.Quickplay40;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsColumnLevel()
	{
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeColumn && mGameMode != GameMode.ChallengeColumn2 && mGameMode != GameMode.ChallengeColumn3)
			{
				return mGameMode == GameMode.ChallengeColumn4;
			}
			return true;
		}
		return false;
	}

	public bool IsFinalBossLevel()
	{
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeFinalBoss && mGameMode != GameMode.ChallengeFinalBoss2)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 50)
				{
					return mGameMode == GameMode.Quickplay50;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsBungeeBlitzLevel()
	{
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeBungeeBlitz)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 45)
				{
					return mGameMode == GameMode.Quickplay45;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public bool IsLevelWithExtendedPoolZombies()
	{
		CSExtendedPoolZombies cSExtendedPoolZombies = mCreativeLevel?.GetComponent<CSExtendedPoolZombies>();
		if (cSExtendedPoolZombies != null)
		{
			return cSExtendedPoolZombies.Enabled;
		}
		if (mBoard == null || (mGameMode != GameMode.ChallengeBobsledBonanza2 && mGameMode != GameMode.ChallengeFusion && !IsSurvivalHell(mGameMode) && !mBoard.StageHasBigPool()))
		{
			return IsAdventureModeHard();
		}
		return true;
	}

	public bool IsRogueConveyorbeltLevel()
	{
		if (mGameMode != GameMode.RogueConveyorbelt)
		{
			return mGameMode == GameMode.RogueConveyorbeltHard;
		}
		return true;
	}

	public bool IsLevelWithHighPresentPlantDropRate(GameMode theGameMode)
	{
		switch (theGameMode)
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
			return mPlayerInfo?.mAdventureModeHard ?? false;
		default:
			return false;
		case GameMode.RogueConveyorbelt:
		case GameMode.RogueConveyorbeltHard:
		case GameMode.ImitaterRandom:
		case GameMode.ChallengeStageRandom:
			return true;
		}
	}

	public bool IsAdventureModeHard()
	{
		if (IsAdventureMode())
		{
			return mPlayerInfo.mAdventureModeHard;
		}
		return false;
	}

	public SeedType GetAwardSeedForLevel(int theLevel)
	{
		int num = (theLevel - 1) / 10 + 1;
		int num2 = (theLevel - 1) % 10 + 1;
		int num3 = (num - 1) * 8 + num2;
		if (num2 >= 10)
		{
			num3 -= 2;
		}
		else if (num2 >= 5)
		{
			num3--;
		}
		if (num3 > 40)
		{
			num3 = 40;
		}
		return (SeedType)num3;
	}

	public string GetCrazyDaveText(int theMessageIndex)
	{
		string theText = Common.StrFormat_("[CRAZY_DAVE_{0}]", theMessageIndex);
		theText = TodCommon.TodReplaceString(theText, "{PLAYER_NAME}", mPlayerInfo.mName);
		theText = TodCommon.TodReplaceString(theText, "{MONEY}", GetMoneyString(mPlayerInfo.mCoins));
		int itemCost = StoreScreen.GetItemCost(StoreItem.STORE_ITEM_PACKET_UPGRADE);
		return TodCommon.TodReplaceString(theText, "{UPGRADE_COST}", GetMoneyString(itemCost));
	}

	public bool CanShowAlmanac()
	{
		if (!IsIceDemo() && mPlayerInfo != null)
		{
			if (!HasFinishedAdventure())
			{
				return mPlayerInfo.mLevel >= 15;
			}
			return true;
		}
		return false;
	}

	public bool IsNight()
	{
		if (mBoard == null || !mBoard.StageIsNight())
		{
			if (!IsIceDemo() && mPlayerInfo != null)
			{
				if ((mPlayerInfo.mLevel < 11 || mPlayerInfo.mLevel > 20) && (mPlayerInfo.mLevel < 31 || mPlayerInfo.mLevel > 40))
				{
					return mPlayerInfo.mLevel == 50;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public bool CanShowStore()
	{
		if (!IsIceDemo() && mPlayerInfo != null)
		{
			if (!HasFinishedAdventure() && !mPlayerInfo.mHasSeenUpsell)
			{
				return mPlayerInfo.mLevel >= 25;
			}
			return true;
		}
		return false;
	}

	public bool HasBeatenChallenge(GameMode theGameMode)
	{
		if (mPlayerInfo == null)
		{
			return false;
		}
		if (IsOnlineLevelMode(theGameMode))
		{
			CreativeLevel levelDefine = CreativeLevelManager.GetLevelDefine(theGameMode);
			if (levelDefine == null)
			{
				return false;
			}
			CSSurvivalLevel component = levelDefine.GetComponent<CSSurvivalLevel>();
			if (component != null && component.mEndless)
			{
				return false;
			}
			return mPlayerInfo.GetOnlineChallengeRecord(levelDefine.mLevelId) > 0;
		}
		if (IsDIYMode(theGameMode))
		{
			return false;
		}
		int num = (int)(theGameMode - 1);
		Sexy.Debug.ASSERT(num >= 0 && num < 151);
		if (IsSurvivalNormal(theGameMode))
		{
			return mPlayerInfo.mChallengeRecords[num] >= 5;
		}
		if (IsSurvivalHard(theGameMode))
		{
			return mPlayerInfo.mChallengeRecords[num] >= 10;
		}
		if (IsSurvivalHell(theGameMode))
		{
			return mPlayerInfo.mChallengeRecords[num] >= 10;
		}
		if (!IsSurvivalEndless(theGameMode) && !IsEndlessScaryPotter(theGameMode) && !IsEndlessIZombie(theGameMode))
		{
			return mPlayerInfo.mChallengeRecords[num] > 0;
		}
		return false;
	}

	public PottedPlant GetPottedPlantByIndex(int thePottedPlantIndex)
	{
		Sexy.Debug.ASSERT(thePottedPlantIndex >= 0 && thePottedPlantIndex < mPlayerInfo.mNumPottedPlants);
		return mPlayerInfo.mPottedPlant[thePottedPlantIndex];
	}

	public bool IsSurvivalNormal(GameMode theGameMode)
	{
		if (theGameMode < GameMode.SurvivalNormalStage1 || theGameMode > GameMode.SurvivalNormalStage5)
		{
			return theGameMode == GameMode.BigPoolSurvivalNormalStage;
		}
		return true;
	}

	public bool IsSurvivalHard(GameMode theGameMode)
	{
		if ((theGameMode < GameMode.SurvivalHardStage1 || theGameMode > GameMode.SurvivalHardStage5) && theGameMode != GameMode.BigPoolSurvivalHardStage)
		{
			return theGameMode == GameMode.RogueConveyorbeltHard;
		}
		return true;
	}

	public bool IsSurvivalEndless(GameMode theGameMode)
	{
		if (theGameMode < GameMode.SurvivalEndlessStage1 || theGameMode > GameMode.SurvivalEndlessStage5)
		{
			return theGameMode == GameMode.RogueConveyorbelt;
		}
		return true;
	}

	public bool IsSurvivalHell(GameMode theGameMode)
	{
		if (theGameMode != GameMode.SurvivalHellStage1 && theGameMode != GameMode.SurvivalHellStage2 && theGameMode != GameMode.SurvivalHellStage3 && theGameMode != GameMode.SurvivalHellStage4 && theGameMode != GameMode.SurvivalHellStage5)
		{
			return theGameMode == GameMode.BigPoolSurvivalHellStage;
		}
		return true;
	}

	public bool HasFinishedAdventure()
	{
		if (mPlayerInfo != null)
		{
			return mPlayerInfo.mFinishedAdventure > 0;
		}
		return false;
	}

	public bool IsFirstTimeAdventureMode()
	{
		if (IsAdventureMode())
		{
			return !HasFinishedAdventure();
		}
		return false;
	}

	public bool CanSpawnYetis()
	{
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(ZombieType.Yeti);
		if (HasFinishedAdventure())
		{
			if (mPlayerInfo.mFinishedAdventure < 2)
			{
				return mPlayerInfo.mLevel >= zombieDefinition.mStartingLevel;
			}
			return true;
		}
		return false;
	}

	public bool IsFusionEnabled()
	{
		return mCreativeLevel?.GetComponent<CSFusion>()?.Enabled ?? (mGameMode == GameMode.ChallengeFusion);
	}

	public bool IsCattailSkinEnabled()
	{
		if (!IsFusionEnabled() && mPlayerInfo.mPurchases[38] == 1)
		{
			return mPlayerInfo.mPurchases[39] == 0;
		}
		return false;
	}

	public void CrazyDaveEnter()
	{
		Sexy.Debug.ASSERT(mCrazyDaveState == CrazyDaveState.Off);
		Sexy.Debug.ASSERT(ReanimationTryToGet(mCrazyDaveReanimID) == null);
		Reanimation reanimation = AddReanimation(0f, 0f, 0, ReanimationType.CrazyDave);
		reanimation.mIsAttachment = true;
		reanimation.SetBasePoseFromAnim(GlobalMembersReanimIds.ReanimTrackId_anim_idle_handing);
		mCrazyDaveReanimID = ReanimationGetID(reanimation);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_enter, ReanimLoopType.PlayOnceAndHold, 0, 24f);
		mCrazyDaveState = CrazyDaveState.Entering;
		mCrazyDaveMessageIndex = -1;
		mCrazyDaveMessageText = string.Empty;
		mCrazyDaveBlinkCounter = TodCommon.RandRangeInt(400, 800);
		if (mGameScene == GameScenes.LevelIntro && mBoard.HasStorm())
		{
			reanimation.mColorOverride = new SexyColor(64, 64, 64);
		}
	}

	public void FinishZenGardenTutorial()
	{
		mZenGarden.mIsTutorial = false;
		mPlayerInfo.mZenGardenTutorialComplete = true;
		mPlayerInfo.mIsInZenTutorial = false;
		mBoardResult = BoardResult.Won;
		KillBoard();
		PreNewGame(GameMode.Adventure, theLookForSavedGame: false);
	}

	public void UpdateCrazyDave()
	{
		Reanimation theAttachReanim = ReanimationTryToGet(mCrazyDaveReanimID);
		if (theAttachReanim == null)
		{
			return;
		}
		if (mCrazyDaveState == CrazyDaveState.Entering || mCrazyDaveState == CrazyDaveState.Talking)
		{
			if (theAttachReanim.mLoopCount > 0)
			{
				theAttachReanim.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 12f);
				mCrazyDaveState = CrazyDaveState.Idling;
			}
		}
		else if (mCrazyDaveState == CrazyDaveState.HandingTalking)
		{
			if (theAttachReanim.mLoopCount > 0)
			{
				theAttachReanim.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle_handing, ReanimLoopType.Loop, 20, 12f);
				mCrazyDaveState = CrazyDaveState.HandingIdling;
			}
		}
		else if (mCrazyDaveState == CrazyDaveState.Leaving && theAttachReanim.mLoopCount > 0)
		{
			CrazyDaveDie();
		}
		if (mCrazyDaveState == CrazyDaveState.Idling || mCrazyDaveState == CrazyDaveState.HandingIdling)
		{
			if (mCrazyDaveMessageText.IndexOf("{MOUTH_BIG_SMILE}") != -1)
			{
				theAttachReanim.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, AtlasResources.IMAGE_REANIM_CRAZYDAVE_MOUTH1);
				mCrazyDaveMessageText = mCrazyDaveMessageText.Replace("{MOUTH_BIG_SMILE}", "");
			}
			else if (mCrazyDaveMessageText.IndexOf("{MOUTH_SMALL_SMILE}") != -1)
			{
				theAttachReanim.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, AtlasResources.IMAGE_REANIM_CRAZYDAVE_MOUTH5);
				mCrazyDaveMessageText = mCrazyDaveMessageText.Replace("{MOUTH_SMALL_SMILE}", "");
			}
			else if (mCrazyDaveMessageText.IndexOf("{MOUTH_BIG_OH}") != -1)
			{
				theAttachReanim.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, AtlasResources.IMAGE_REANIM_CRAZYDAVE_MOUTH4);
				mCrazyDaveMessageText = mCrazyDaveMessageText.Replace("{MOUTH_BIG_OH}", "");
			}
			else if (mCrazyDaveMessageText.IndexOf("{MOUTH_SMALL_OH}") != -1)
			{
				theAttachReanim.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, AtlasResources.IMAGE_REANIM_CRAZYDAVE_MOUTH6);
				mCrazyDaveMessageText = mCrazyDaveMessageText.Replace("{MOUTH_SMALL_OH}", "");
			}
		}
		Reanimation reanimation;
		if (mCrazyDaveState == CrazyDaveState.Idling || mCrazyDaveState == CrazyDaveState.Talking || mCrazyDaveState == CrazyDaveState.HandingTalking || mCrazyDaveState == CrazyDaveState.HandingIdling)
		{
			mCrazyDaveBlinkCounter--;
			if (mCrazyDaveBlinkCounter <= 0)
			{
				mCrazyDaveBlinkCounter = TodCommon.RandRangeInt(400, 800);
				reanimation = AddReanimation(0f, 0f, 0, ReanimationType.CrazyDave);
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_blink);
				reanimation.mLoopType = ReanimLoopType.PlayOnceFullLastFrameAndHold;
				reanimation.mAnimRate = 15f;
				reanimation.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_dave_head);
				reanimation.mColorOverride = theAttachReanim.mColorOverride;
				theAttachReanim.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_dave_eye, -1);
				mCrazyDaveBlinkReanimID = ReanimationGetID(reanimation);
			}
		}
		reanimation = ReanimationTryToGet(mCrazyDaveBlinkReanimID);
		if (reanimation != null && reanimation.mLoopCount > 0)
		{
			theAttachReanim.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_dave_eye, 0);
			RemoveReanimation(ref mCrazyDaveBlinkReanimID);
			mCrazyDaveBlinkReanimID = null;
		}
		theAttachReanim.Update();
	}

	public void CrazyDaveTalkIndex(int theMessageIndex)
	{
		mCrazyDaveMessageIndex = theMessageIndex;
		string crazyDaveText = GetCrazyDaveText(theMessageIndex);
		CrazyDaveTalkMessage(crazyDaveText);
	}

	public void CrazyDaveTalkMessage(string theMessage)
	{
		Reanimation reanimation = ReanimationGet(mCrazyDaveReanimID);
		bool flag = false;
		if (theMessage.IndexOf("{HANDING}") != -1)
		{
			flag = true;
			theMessage = theMessage.Replace("{HANDING}", string.Empty);
		}
		if ((mCrazyDaveState == CrazyDaveState.HandingTalking || mCrazyDaveState == CrazyDaveState.HandingIdling) && !flag)
		{
			CrazyDaveDoneHanding();
		}
		bool flag2 = true;
		if (theMessage.IndexOf("{NO_SOUND}") != -1)
		{
			flag2 = false;
			theMessage = theMessage.Replace("{NO_SOUND}", "");
		}
		else
		{
			CrazyDaveStopSound();
		}
		int num = 0;
		bool flag3 = false;
		for (int i = 0; i < theMessage.length(); i++)
		{
			if (theMessage[i] == '{')
			{
				flag3 = true;
			}
			else if (theMessage[i] == '}')
			{
				flag3 = false;
			}
			else if (!flag3)
			{
				num++;
			}
		}
		Image theImage = null;
		reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, theImage);
		if ((mCrazyDaveState != CrazyDaveState.Talking) | flag2)
		{
			if (flag)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_talk_handing, ReanimLoopType.Loop, 50, 12f);
				if (flag2 && theMessage.IndexOf("{SHORT_SOUND}") != -1)
				{
					PlayFoley(FoleyType.Crazydaveshort);
					theMessage = theMessage.Replace("{SHORT_SOUND}", "");
				}
				else if (flag2 && theMessage.IndexOf("{SCREAM}") != -1)
				{
					PlayFoley(FoleyType.Crazydavescream);
					theMessage = theMessage.Replace("{SCREAM}", "");
				}
				else if (flag2)
				{
					PlayFoley(FoleyType.Crazydavelong);
				}
				mCrazyDaveState = CrazyDaveState.HandingTalking;
			}
			else if (theMessage.IndexOf("{SHAKE}") != -1)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_crazy, ReanimLoopType.PlayOnceAndHold, 50, 12f);
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydavecrazy);
				}
				mCrazyDaveState = CrazyDaveState.Talking;
				theMessage = theMessage.Replace("{SHAKE}", "");
			}
			else if (theMessage.IndexOf("{SCREAM}") != -1)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_smalltalk, ReanimLoopType.PlayOnceAndHold, 50, 12f);
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydavescream);
				}
				mCrazyDaveState = CrazyDaveState.Talking;
				theMessage = theMessage.Replace("{SCREAM}", "");
			}
			else if (theMessage.IndexOf("{SCREAM2}") != -1)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_mediumtalk, ReanimLoopType.PlayOnceAndHold, 50, 12f);
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydavescream2);
				}
				mCrazyDaveState = CrazyDaveState.Talking;
				theMessage = theMessage.Replace("{SCREAM2}", "");
			}
			else if (theMessage.IndexOf("{SHOW_WALLNUT}") != -1)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_talk_handing, ReanimLoopType.Loop, 50, 12f);
				Reanimation reanimation2 = AddReanimation(0f, 0f, 0, ReanimationType.Wallnut);
				reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 12f);
				ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_dave_handinghand);
				AttachEffect attachEffect = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName.mAttachmentID, reanimation2, 100f * Constants.S, 393f * Constants.S);
				attachEffect.mOffset.mMatrix.M11 = 1.2f;
				attachEffect.mOffset.mMatrix.M22 = 1.2f;
				reanimation.Update();
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydavescream2);
				}
				mCrazyDaveState = CrazyDaveState.HandingTalking;
				theMessage = theMessage.Replace("{SHOW_WALLNUT}", "");
			}
			else if (theMessage.IndexOf("{SHOW_HAMMER}") != -1)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_talk_handing, ReanimLoopType.Loop, 50, 12f);
				Reanimation reanimation3 = AddReanimation(0f, 0f, 0, ReanimationType.Hammer);
				reanimation3.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_whack_zombie, ReanimLoopType.PlayOnceAndHold, 0, 24f);
				reanimation3.mAnimTime = 1f;
				ReanimatorTrackInstance trackInstanceByName2 = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_dave_handinghand);
				AttachEffect attachEffect2 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName2.mAttachmentID, reanimation3, 62f * Constants.S, 445f * Constants.S);
				attachEffect2.mOffset.mMatrix.M11 = 1.5f;
				attachEffect2.mOffset.mMatrix.M22 = 1.5f;
				reanimation.Update();
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydavelong);
				}
				mCrazyDaveState = CrazyDaveState.HandingTalking;
				theMessage = theMessage.Replace("{SHOW_HAMMER}", "");
			}
			else if (num < 23)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_smalltalk, ReanimLoopType.PlayOnceAndHold, 50, 12f);
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydaveshort);
				}
				mCrazyDaveState = CrazyDaveState.Talking;
			}
			else if (num < 52)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_mediumtalk, ReanimLoopType.PlayOnceAndHold, 50, 12f);
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydavelong);
				}
				mCrazyDaveState = CrazyDaveState.Talking;
			}
			else
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_blahblah, ReanimLoopType.PlayOnceAndHold, 50, 12f);
				if (flag2)
				{
					PlayFoley(FoleyType.Crazydaveextralong);
				}
				mCrazyDaveState = CrazyDaveState.Talking;
			}
		}
		mCrazyDaveMessageText = theMessage;
	}

	public void CrazyDaveLeave()
	{
		Reanimation reanimation = ReanimationTryToGet(mCrazyDaveReanimID);
		if (reanimation != null)
		{
			if (mCrazyDaveState == CrazyDaveState.HandingTalking || mCrazyDaveState == CrazyDaveState.HandingIdling)
			{
				CrazyDaveDoneHanding();
			}
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_leave, ReanimLoopType.PlayOnceAndHold, 20, 24f);
			Image theImage = null;
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, theImage);
			mCrazyDaveState = CrazyDaveState.Leaving;
			mCrazyDaveMessageIndex = -1;
			mCrazyDaveMessageText = string.Empty;
			CrazyDaveStopSound();
		}
	}

	public void DrawCrazyDave(Graphics g)
	{
		DrawCrazyDave(g, theUseSmallFont: false);
	}

	public void DrawCrazyDave(Graphics g, bool theUseSmallFont)
	{
		Reanimation reanimation = ReanimationTryToGet(mCrazyDaveReanimID);
		if (reanimation == null)
		{
			return;
		}
		int theWidth = Constants.RetardedDave_Bubble_Size;
		if (!mCrazyDaveMessageText.empty())
		{
			int num = (int)Constants.InvertAndScale(285f);
			int num2 = (int)Constants.InvertAndScale(80f);
			Image iMAGE_STORE_SPEECHBUBBLE = AtlasResources.IMAGE_STORE_SPEECHBUBBLE;
			if (GetDialog(Dialogs.DIALOG_STORE) != null)
			{
				num += Constants.RetardedDave_Bubble_Offset_Shop.X;
				num2 += Constants.RetardedDave_Bubble_Offset_Shop.Y;
				theWidth = (int)Constants.InvertAndScale(150f);
				int num3 = (int)Constants.InvertAndScale(105f);
				g.DrawImage(iMAGE_STORE_SPEECHBUBBLE, num, num2, new TRect(0, 0, (int)Constants.InvertAndScale(64f), iMAGE_STORE_SPEECHBUBBLE.mHeight));
				g.DrawImage(iMAGE_STORE_SPEECHBUBBLE, num + (int)Constants.InvertAndScale(64f), num2, new TRect(iMAGE_STORE_SPEECHBUBBLE.mWidth - num3, 0, num3, iMAGE_STORE_SPEECHBUBBLE.mHeight));
			}
			else if (mGameMode == GameMode.ChallengeZenGarden)
			{
				num += Constants.ZenGarden_RetardedDaveBubble_Pos.X;
				num2 += Constants.ZenGarden_RetardedDaveBubble_Pos.Y;
				g.DrawImage(iMAGE_STORE_SPEECHBUBBLE, num, num2);
			}
			else
			{
				num += Constants.RetardedDave_Bubble_Offset_Board.X;
				num2 += Constants.RetardedDave_Bubble_Offset_Board.Y;
				g.DrawImage(iMAGE_STORE_SPEECHBUBBLE, num, num2);
			}
			g.DrawImage(AtlasResources.IMAGE_STORE_SPEECHBUBBLE_TIP, num + (int)Constants.InvertAndScale(30f), num2 - Constants.RetardedDave_Bubble_Tip_Offset + iMAGE_STORE_SPEECHBUBBLE.mHeight);
			string text = mCrazyDaveMessageText;
			TRect theRect = new TRect(num + Constants.RetardedDave_Bubble_Rect.mX, num2 + Constants.RetardedDave_Bubble_Rect.mY, theWidth, Constants.RetardedDave_Bubble_Rect.mHeight);
			int mX = theRect.mX;
			if (text.IndexOf("{SHAKE}") != -1)
			{
				text = TodCommon.TodReplaceString(text, "{SHAKE}", "");
				theRect.mX += RandomNumbers.NextNumber() % 2;
				theRect.mY += RandomNumbers.NextNumber() % 2;
			}
			bool flag = true;
			if (mGameMode == GameMode.Upsell)
			{
				flag = false;
			}
			else if (text.IndexOf("{NO_CLICK}") != -1)
			{
				if (!noClickStringCache.TryGetValue(text, out var value))
				{
					value = TodCommon.TodReplaceString(text, "{NO_CLICK}", string.Empty);
					noClickStringCache.Add(text, value);
				}
				text = value;
				flag = false;
			}
			g.SetColor(SexyColor.Black);
			g.SetFont(theUseSmallFont ? Resources.FONT_BRIANNETOD12 : Resources.FONT_BRIANNETOD16);
			TodStringFile.TodDrawStringWrapped(g, text, theRect, theUseSmallFont ? Resources.FONT_BRIANNETOD12 : Resources.FONT_BRIANNETOD16, SexyColor.Black, DrawStringJustification.CenterVerticalMiddle);
			if (flag)
			{
				TodCommon.TodDrawString(g, "[TAP_TO_CONTINUE]", mX + theRect.mWidth / 2, num2 + Constants.RetardedDave_Bubble_TapToContinue_Y, Resources.FONT_PICO129, SexyColor.Black, DrawStringJustification.Center);
			}
		}
		reanimation.Draw(g);
	}

	public void CrazyDaveDie()
	{
		Reanimation reanimation = ReanimationTryToGet(mCrazyDaveReanimID);
		if (reanimation != null)
		{
			reanimation.ReanimationDie();
			mCrazyDaveState = CrazyDaveState.Off;
			mCrazyDaveReanimID = null;
			mCrazyDaveMessageIndex = -1;
			mCrazyDaveMessageText = string.Empty;
			CrazyDaveStopSound();
		}
	}

	public void DoUpsellScreen()
	{
	}

	public void CrazyDaveStopTalking()
	{
		bool flag = true;
		if (mGameMode == GameMode.Upsell)
		{
			flag = false;
		}
		if (flag && mCrazyDaveState == CrazyDaveState.HandingTalking)
		{
			CrazyDaveDoneHanding();
		}
		Image theImage = null;
		Reanimation reanimation = ReanimationGet(mCrazyDaveReanimID);
		reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_dave_mouths, theImage);
		if (mCrazyDaveState == CrazyDaveState.HandingTalking && !flag)
		{
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle_handing, ReanimLoopType.Loop, 20, 12f);
			mCrazyDaveState = CrazyDaveState.HandingIdling;
		}
		else if (mCrazyDaveState == CrazyDaveState.Talking || mCrazyDaveState == CrazyDaveState.HandingTalking)
		{
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 12f);
			mCrazyDaveState = CrazyDaveState.Idling;
		}
		mCrazyDaveMessageIndex = -1;
		mCrazyDaveMessageText = string.Empty;
		CrazyDaveStopSound();
	}

	public void PreloadForUser()
	{
		int num = mCompletedLoadingThreadTasks + GetNumPreloadingTasks();
		if (mTitleScreen != null && mTitleScreen.mQuickLoadKey != KeyCode.Unknown)
		{
			mCompletedLoadingThreadTasks = num;
			return;
		}
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Puff, theIsPreloading: true);
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.LawnMoweredZombie, theIsPreloading: true);
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Readysetplant, theIsPreloading: true);
		mCompletedLoadingThreadTasks += 68;
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.FinalWave, theIsPreloading: true);
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Sun, theIsPreloading: true);
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.TextFadeOn, theIsPreloading: true);
		mCompletedLoadingThreadTasks += 68;
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Zombie, theIsPreloading: true);
		mCompletedLoadingThreadTasks += 68;
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieNewspaper, theIsPreloading: true);
		mCompletedLoadingThreadTasks += 68;
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.SelectorScreen, theIsPreloading: true);
		mCompletedLoadingThreadTasks += 340;
		mCompletedLoadingThreadTasks += 68;
		if (mPlayerInfo != null)
		{
			for (SeedType seedType = SeedType.Peashooter; seedType < SeedType.SeedTypeCount; seedType++)
			{
				if (HasSeedType(seedType) || HasFinishedAdventure())
				{
					Plant.PreloadPlantResources(seedType);
					if (mCompletedLoadingThreadTasks < num)
					{
						mCompletedLoadingThreadTasks += 68;
					}
					if (mTitleScreen != null && mTitleScreen.mQuickLoadKey != KeyCode.Unknown)
					{
						mCompletedLoadingThreadTasks = num;
						return;
					}
					if (mShutdown || mCloseRequest)
					{
						return;
					}
				}
			}
			for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
			{
				if (!HasFinishedAdventure())
				{
					ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
					if (mPlayerInfo.mLevel < zombieDefinition.mStartingLevel)
					{
						continue;
					}
				}
				if (zombieType != ZombieType.Boss && zombieType != ZombieType.Catapult && zombieType != ZombieType.Gargantuar && zombieType != ZombieType.Digger && zombieType != ZombieType.Zamboni)
				{
					Zombie.PreloadZombieResources(zombieType);
					if (mCompletedLoadingThreadTasks < num)
					{
						mCompletedLoadingThreadTasks += 68;
					}
					if (mTitleScreen != null && mTitleScreen.mQuickLoadKey != KeyCode.Unknown)
					{
						mCompletedLoadingThreadTasks = num;
						return;
					}
					if (mShutdown || mCloseRequest)
					{
						return;
					}
				}
			}
		}
		if (mCompletedLoadingThreadTasks != num)
		{
			mCompletedLoadingThreadTasks = num;
		}
	}

	public void PreloadLoadingThreadReanimations()
	{
	}

	public void PreloadReanimation(ReanimationType theReanimType)
	{
	}

	public int GetNumPreloadingTasks()
	{
		int num = 10;
		if (mPlayerInfo != null)
		{
			for (SeedType seedType = SeedType.Peashooter; seedType < SeedType.SeedTypeCount; seedType++)
			{
				if (HasSeedType(seedType) || HasFinishedAdventure())
				{
					num++;
				}
			}
			for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
			{
				if (!HasFinishedAdventure())
				{
					ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
					if (mPlayerInfo.mLevel < zombieDefinition.mStartingLevel)
					{
						continue;
					}
				}
				if (zombieType != ZombieType.Boss && zombieType != ZombieType.Catapult && zombieType != ZombieType.Gargantuar && zombieType != ZombieType.Digger && zombieType != ZombieType.Zamboni)
				{
					num++;
				}
			}
		}
		return num * 68;
	}

	public void LawnMessageBox(int theDialogId, string theHeaderName, string theLinesName, string theButton1Name, string theButton2Name, int theButtonMode, LawnMessageBoxListener theListener)
	{
		mOldFocus = mWidgetManager.mFocusWidget;
		mLawnMessageBoxListener = theListener;
		LawnDialog lawnDialog = DoDialog(theDialogId, isModal: true, theHeaderName, theLinesName, theButton1Name, theButtonMode);
		if (lawnDialog.mLawnYesButton != null)
		{
			lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate(theButton1Name);
		}
		if (lawnDialog.mLawnNoButton != null)
		{
			lawnDialog.mLawnNoButton.mLabel = TodStringFile.TodStringTranslate(theButton2Name);
		}
		lawnDialog.CalcSize(0, 0, (int)Constants.InvertAndScale(400f));
		CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
		mWidgetManager.SetFocus(lawnDialog);
	}

	public virtual void EnforceCursor()
	{
	}

	public void ShowCreditScreen()
	{
		mCreditScreen = new CreditScreen(this);
		mCreditScreen.Resize(0, 0, mWidth, mHeight);
		mWidgetManager.AddWidget(mCreditScreen);
		mWidgetManager.BringToBack(mCreditScreen);
		mWidgetManager.SetFocus(mCreditScreen);
	}

	public void KillCreditScreen()
	{
		if (mCreditScreen != null)
		{
			mWidgetManager.RemoveWidget(mCreditScreen);
			SafeDeleteWidget(mCreditScreen);
			mCreditScreen = null;
		}
	}

	public string Pluralize(int theCount, string theSingular, string thePlural)
	{
		if (theCount == 1)
		{
			return TodCommon.TodReplaceNumberString(theSingular, "{COUNT}", theCount);
		}
		return TodCommon.TodReplaceNumberString(thePlural, "{COUNT}", theCount);
	}

	public int GetNumTrophies(ChallengePage thePage)
	{
		int num = 0;
		for (int i = 1; i < 100; i++)
		{
			ChallengeDefinition challengeDefinition = ChallengeScreen.gChallengeDefs[i - 1];
			if (thePage == challengeDefinition.mPage && HasBeatenChallenge(challengeDefinition.mChallengeMode))
			{
				num++;
			}
		}
		return num;
	}

	public bool EarnedGoldTrophy()
	{
		if (HasFinishedAdventure())
		{
			return TrophiesNeedForGoldSunflower() <= 0;
		}
		return false;
	}

	public void SetGameMode(GameMode theGameMode)
	{
		mGameMode = theGameMode;
		if (CreativeLevelManager.IsCreativeMode(theGameMode))
		{
			mCreativeLevel = CreativeLevelManager.GetLevelDefine(theGameMode);
		}
		else
		{
			mCreativeLevel = null;
		}
	}

	public bool IsRegistered()
	{
		return false;
	}

	public bool IsTrialStageLocked()
	{
		return false;
	}

	public bool IsDRMConnected()
	{
		return false;
	}

	public bool IsScaryPotterLevel()
	{
		if ((mGameMode < GameMode.ScaryPotter1 || mGameMode >= GameMode.PuzzleIZombie1) && (!IsAdventureMode() || mPlayerInfo.mLevel != 35) && mGameMode != GameMode.Quickplay35)
		{
			return mCreativeLevel?.GetComponent<CSVaseLevel>() != null;
		}
		return true;
	}

	public bool IsEndlessScaryPotter(GameMode theGameMode)
	{
		return theGameMode == GameMode.ScaryPotterEndless;
	}

	public bool IsSquirrelLevel()
	{
		if (mBoard != null)
		{
			return mGameMode == GameMode.ChallengeSquirrel;
		}
		return false;
	}

	public bool IsIZombieLevel()
	{
		if (mBoard == null || (mGameMode != GameMode.PuzzleIZombie1 && mGameMode != GameMode.PuzzleIZombie2 && mGameMode != GameMode.PuzzleIZombie3 && mGameMode != GameMode.PuzzleIZombie4 && mGameMode != GameMode.PuzzleIZombie5 && mGameMode != GameMode.PuzzleIZombie6 && mGameMode != GameMode.PuzzleIZombie7 && mGameMode != GameMode.PuzzleIZombie8 && mGameMode != GameMode.PuzzleIZombie9 && mGameMode != GameMode.PuzzleIZombieEndless))
		{
			return mCreativeLevel?.GetComponent<CSIZombieLevel>() != null;
		}
		return true;
	}

	public bool CanShowZenGarden()
	{
		if (mPlayerInfo != null && !IsTrialStageLocked())
		{
			if (!HasFinishedAdventure())
			{
				return mPlayerInfo.mLevel >= 45;
			}
			return true;
		}
		return false;
	}

	public bool CanShowFumeshroomGnome()
	{
		if (mPlayerInfo != null)
		{
			return mPlayerInfo.mPurchases[40] != 0;
		}
		return false;
	}

	public static string GetMoneyString(int theAmount)
	{
		int theValue = theAmount * 10;
		if (!moneyStrings.TryGetValue(theAmount, out var value))
		{
			value = TodCommon.TodReplaceString(TodStringFile.TodStringTranslate("[CURRENCY_STRING]"), "{CURRENCY_SYMBOL}", TodStringFile.TodStringTranslate("[CURRENCY_SYMBOL]"));
			value = TodCommon.TodReplaceString(value, "{AMOUNT}", Common.CommaSeperate(theValue));
			moneyStrings.Add(theAmount, value);
		}
		return value;
	}

	public static string ToString(int i)
	{
		if (!cachedIntToString.TryGetValue(i, out var value))
		{
			value = i.ToString();
			cachedIntToString.Add(i, value);
		}
		return value;
	}

	public bool AdvanceCrazyDaveText()
	{
		int num = mCrazyDaveMessageIndex + 1;
		string theString = Common.StrFormat_("[CRAZY_DAVE_{0}]", num);
		if (!TodStringFile.TodStringListExists(theString))
		{
			return false;
		}
		CrazyDaveTalkIndex(num);
		return true;
	}

	public bool IsWhackAZombieLevel()
	{
		if (mCreativeLevel?.GetComponent<CSWhackAZombieLevel>() != null)
		{
			return true;
		}
		if (mBoard != null)
		{
			if (mGameMode != GameMode.ChallengeWhackAZombie)
			{
				if (!IsAdventureMode() || mPlayerInfo.mLevel != 15)
				{
					return mGameMode == GameMode.Quickplay15;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public void UpdatePlayTimeStats()
	{
	}

	public bool CanPauseNow()
	{
		if (mBoard != null && (mSeedChooserScreen == null || !mSeedChooserScreen.mMouseVisible) && mBoard.mBoardFadeOutCounter < 0 && mCrazyDaveState == CrazyDaveState.Off && mGameMode != GameMode.ChallengeZenGarden && mGameMode != GameMode.TreeOfWisdom)
		{
			return GetDialogCount() <= 0;
		}
		return false;
	}

	public bool IsPuzzleMode()
	{
		if (mGameMode < GameMode.ScaryPotter1 || mGameMode > GameMode.ScaryPotterEndless)
		{
			if (mGameMode >= GameMode.PuzzleIZombie1)
			{
				return mGameMode <= GameMode.PuzzleIZombieEndless;
			}
			return false;
		}
		return true;
	}

	public bool IsLevelUseJson()
	{
		return mCreativeLevel != null;
	}

	public static bool IsDIYMode(GameMode theGameMode)
	{
		if (theGameMode >= GameMode.CreativeLevel)
		{
			return theGameMode < GameMode.OnlineLevel;
		}
		return false;
	}

	public bool IsDIYMode()
	{
		return IsDIYMode(mGameMode);
	}

	public static bool IsOnlineLevelMode(GameMode theGameMode)
	{
		return theGameMode >= GameMode.OnlineLevel;
	}

	public bool IsOnlineLevelMode()
	{
		return IsOnlineLevelMode(mGameMode);
	}

	public bool IsChallengeMode()
	{
		if (!IsAdventureMode() && !IsQuickPlayMode() && !IsPuzzleMode() && !IsSurvivalMode())
		{
			return !IsOnlineLevelMode();
		}
		return false;
	}

	public bool IsEndlessIZombie(GameMode theGameMode)
	{
		return theGameMode == GameMode.PuzzleIZombieEndless;
	}

	public void CrazyDaveDoneHanding()
	{
		Reanimation reanimation = ReanimationGet(mCrazyDaveReanimID);
		ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_dave_handinghand);
		GlobalMembersAttachment.AttachmentDie(ref trackInstanceByName.mAttachmentID);
	}

	public bool AboutToEarnGoldSunflower()
	{
		if (TrophiesNeedForGoldSunflower() == 1)
		{
			return LevelCanEarnGoldSunFlower();
		}
		return false;
	}

	public bool LevelCanEarnGoldSunFlower()
	{
		if (IsDIYMode())
		{
			return false;
		}
		if (!IsSurvivalMode() && !IsPuzzleMode() && (mGameMode < GameMode.ChallengeWarAndPeas || mGameMode > GameMode.ChallengeFinalBoss))
		{
			return mGameMode == GameMode.ChallengeZombiquarium;
		}
		return true;
	}

	public int TrophiesNeedForGoldSunflower()
	{
		int num = 48;
		int num2 = GetNumTrophies(ChallengePage.Puzzle) + GetNumTrophies(ChallengePage.Challenge) + GetNumTrophies(ChallengePage.Survival);
		return num - num2;
	}

	public int GetCurrentChallengeIndex()
	{
		return (int)(mGameMode - 1);
	}

	public void LoadGroup(string theGroupName, int theGroupAveMsToLoad)
	{
		PerfTimer perfTimer = default;
		perfTimer.Start();
		mResourceManager.StartLoadResources(theGroupName);
		while (!mShutdown && !mCloseRequest && !mLoadingFailed && TodCommon.TodLoadNextResource())
		{
			mCompletedLoadingThreadTasks += theGroupAveMsToLoad;
		}
		if (!mShutdown && !mCloseRequest)
		{
			if (mResourceManager.HadError())
			{
				ShowResourceError();
				mLoadingFailed = true;
			}
			else if (!Resources.ExtractResourcesByName(mResourceManager, theGroupName))
			{
				ShowResourceError();
				mLoadingFailed = true;
			}
			else
			{
				int theTotalGroupWeigth = mResourceManager.GetNumResources(theGroupName) * theGroupAveMsToLoad;
				int theGroupTime = Math.Max((int)perfTimer.GetDuration(), 0);
				TraceLoadGroup(theGroupName, theGroupTime, theTotalGroupWeigth, theGroupAveMsToLoad);
			}
		}
	}

	public void TraceLoadGroup(string theGroupName, int theGroupTime, int theTotalGroupWeigth, int theTaskWeight)
	{
	}

	public void DelayLoadBackgroundResource(string theGroupName)
	{
		if (mLastBackgroundResGroupLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastBackgroundResGroupLoaded))
			{
				mResourceManager.UnloadBackground(mLastBackgroundResGroupLoaded);
			}
			if (!string.IsNullOrEmpty(theGroupName))
			{
				TodCommon.TodLoadResources(theGroupName);
			}
			mLastBackgroundResGroupLoaded = theGroupName;
		}
	}

	private static void LoadResourceInThread(string resourceGroup)
	{
		if (Main.LOW_MEMORY_DEVICE)
		{
			Thread thread = new Thread(Load);
			LoadingScreen.IsLoading = true;
			thread.IsBackground = true;
			thread.Start(resourceGroup);
		}
		else
		{
			Load(resourceGroup);
		}
	}

	private static void Load(object resourceGroup)
	{
		string theGroup = resourceGroup as string;
		TodCommon.TodLoadResources(theGroup);
	}

	public void DelayLoadLeaderboardResource(bool doLoad)
	{
		if (Main.LOW_MEMORY_DEVICE || !leaderboardLoaded)
		{
			if (doLoad)
			{
				DelayLoadMainMenuResource(doLoad: false);
				DelayLoadGamePlayResources(doLoad: false);
			}
			if (doLoad && !leaderboardLoaded)
			{
				LoadResourceInThread("DelayLoad_Leaderboard");
			}
			else if (leaderboardLoaded && !doLoad)
			{
				mResourceManager.UnloadBackground("DelayLoad_Leaderboard");
			}
			DelayLoadPileResource(doLoad);
			if (!doLoad)
			{
				DelayLoadMainMenuResource(doLoad: true);
			}
			leaderboardLoaded = doLoad;
		}
	}

	public void DelayLoadGamePlayResources(bool doLoad)
	{
		if (!Main.LOW_MEMORY_DEVICE && gamePlayLoaded)
		{
			return;
		}
		if (doLoad && !gamePlayLoaded)
		{
			GC.Collect();
			TodCommon.TodLoadResources("DelayLoad_GamePlay");
			AtlasResources.mAtlasResources.UnpackPlantsZombiesAtlasImages();
			AtlasResources.mAtlasResources.UnpackParticlesAtlasImages();
			if (Main.LOW_MEMORY_DEVICE)
			{
				ResourceManager.mReanimContentManager.Unload();
				ReanimatorXnaHelpers.ReanimatorLoadDefinitions(ref GameConstants.gLawnReanimationArray, 150);
				ResourceManager.mParticleContentManager.Unload();
				TodParticleGlobal.TodParticleLoadDefinitions(ref GameConstants.gLawnParticleArray, 107);
			}
		}
		else if (gamePlayLoaded && !doLoad)
		{
			mResourceManager.UnloadBackground("DelayLoad_GamePlay");
		}
		gamePlayLoaded = doLoad;
	}

	private void DelayLoadCachedResources(bool doLoad)
	{
		if (Main.LOW_MEMORY_DEVICE || !cachedLoaded)
		{
			if (doLoad && !cachedLoaded)
			{
				GC.Collect();
				TodCommon.TodLoadResources("DelayLoad_Cached");
				AtlasResources.mAtlasResources.UnpackCachedAtlasImages();
			}
			else if (cachedLoaded && !doLoad)
			{
				mResourceManager.UnloadBackground("DelayLoad_Cached");
			}
			cachedLoaded = doLoad;
		}
	}

	public void DelayLoadZenGardenResources(bool doLoad)
	{
		if (!Main.LOW_MEMORY_DEVICE && zenGardenLoaded)
		{
			return;
		}
		if (doLoad && !zenGardenLoaded)
		{
			GC.Collect();
			TodCommon.TodLoadResources("DelayLoad_ZenGarden");
			AtlasResources.mAtlasResources.UnpackZengardenAtlasImages();
			if (Main.LOW_MEMORY_DEVICE)
			{
				ResourceManager.mReanimContentManager.Unload();
				ReanimatorXnaHelpers.ReanimatorLoadDefinitions(ref GameConstants.gLawnReanimationArray, 150);
				ResourceManager.mParticleContentManager.Unload();
				TodParticleGlobal.TodParticleLoadDefinitions(ref GameConstants.gLawnParticleArray, 107);
			}
		}
		else if (zenGardenLoaded && !doLoad)
		{
			mResourceManager.UnloadBackground("DelayLoad_ZenGarden");
		}
		zenGardenLoaded = doLoad;
	}

	public void DelayLoadMainMenuResource(bool doLoad)
	{
		if (Main.LOW_MEMORY_DEVICE || !mainMenuLoaded)
		{
			if (doLoad && !mainMenuLoaded)
			{
				TodCommon.TodLoadResources("DelayLoad_MainMenu");
				TodCommon.TodLoadResources("DelayLoad_Almanac");
				AtlasResources.mAtlasResources.UnpackGoodiesAtlasImages();
				AtlasResources.mAtlasResources.UnpackQuickplayAtlasImages();
				AtlasResources.mAtlasResources.UnpackMiniGamesAtlasImages();
			}
			else if (mainMenuLoaded && !doLoad)
			{
				mResourceManager.UnloadBackground("DelayLoad_MainMenu");
			}
			mainMenuLoaded = doLoad;
		}
	}

	public void DelayLoadPileResource(bool doLoad)
	{
		if (Main.LOW_MEMORY_DEVICE || !pileLoaded)
		{
			if (doLoad && !pileLoaded)
			{
				TodCommon.TodLoadResources("DelayLoad_Pile");
				AtlasResources.mAtlasResources.UnpackPileAtlasImages();
			}
			else if (pileLoaded && !doLoad)
			{
				mResourceManager.UnloadBackground("DelayLoad_Pile");
			}
			pileLoaded = doLoad;
		}
	}

	public void DelayLoadZombieNoteResource(string theGroupName)
	{
		if (mLastZombieNoteResGroupLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastZombieNoteResGroupLoaded))
			{
				mResourceManager.UnloadBackground(mLastZombieNoteResGroupLoaded);
			}
			TodCommon.TodLoadResources(theGroupName);
			mLastZombieNoteResGroupLoaded = theGroupName;
			Resources.ExtractResourcesByName(mResourceManager, theGroupName);
		}
	}

	public void DelayLoadZenGardenBackground(string theGroupName)
	{
		if (mLastZenGardenResourceLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastZenGardenResourceLoaded))
			{
				mResourceManager.UnloadBackground(mLastZenGardenResourceLoaded);
			}
			TodCommon.TodLoadResources(theGroupName);
			mLastZenGardenResourceLoaded = theGroupName;
			Resources.ExtractResourcesByName(mResourceManager, theGroupName);
		}
	}

	public void DelayLoadZombieNotePaperResource(string theGroupName)
	{
		if (mLastPaperGroupLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastPaperGroupLoaded))
			{
				mResourceManager.UnloadBackground(mLastPaperGroupLoaded);
			}
			TodCommon.TodLoadResources(theGroupName);
			mLastPaperGroupLoaded = theGroupName;
			Resources.ExtractResourcesByName(mResourceManager, theGroupName);
		}
	}

	public void DelayLoadUpsellResource(string theGroupName)
	{
		if (mLastStoreResGroupLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastUpsellResGroupLoaded))
			{
				mResourceManager.UnloadBackground(mLastUpsellResGroupLoaded);
			}
			TodCommon.TodLoadResources(theGroupName);
			mLastUpsellResGroupLoaded = theGroupName;
			Resources.ExtractResourcesByName(mResourceManager, theGroupName);
		}
	}

	public void DelayLoadStoreResource(string theGroupName)
	{
		if (mLastStoreResGroupLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastStoreResGroupLoaded))
			{
				mResourceManager.UnloadBackground(mLastStoreResGroupLoaded);
			}
			TodCommon.TodLoadResources(theGroupName);
			mLastStoreResGroupLoaded = theGroupName;
		}
	}

	public void DelayLoadComicThumbnailsResource(string theGroupName)
	{
		if (mLastComicThumbnailResGroupLoaded != theGroupName)
		{
			if (!string.IsNullOrEmpty(mLastComicThumbnailResGroupLoaded))
			{
				mResourceManager.UnloadBackground(mLastComicThumbnailResGroupLoaded);
			}
			TodCommon.TodLoadResources(theGroupName);
			mLastComicThumbnailResGroupLoaded = theGroupName;
		}
	}

	public void CrazyDaveStopSound()
	{
		mSoundSystem.StopFoley(FoleyType.Crazydaveshort);
		mSoundSystem.StopFoley(FoleyType.Crazydavelong);
		mSoundSystem.StopFoley(FoleyType.Crazydaveextralong);
		mSoundSystem.StopFoley(FoleyType.Crazydavecrazy);
	}

	public bool UpdatePlayerProfileForFinishingLevel()
	{
		if (IsOnlineLevelMode())
		{
			if (IsSurvivalMode() && !mBoard.IsFinalSurvivalStage())
			{
				return false;
			}
			CreativeLevel levelDefine = CreativeLevelManager.GetLevelDefine(mGameMode);
			if (levelDefine == null)
			{
				return false;
			}
			string mLevelId = levelDefine.mLevelId;
			int onlineChallengeRecord = mPlayerInfo.GetOnlineChallengeRecord(mLevelId);
			mPlayerInfo.SetOnlineChallengeRecord(mLevelId, onlineChallengeRecord + 1);
			WriteCurrentUserConfig();
			return false;
		}
		if (IsDIYMode())
		{
			return false;
		}
		bool flag = false;
		if (IsAdventureMode())
		{
			int mLevel = mBoard.mLevel;
			if (mLevel == 50)
			{
				if (mPlayerInfo.mIZombieUnlocked == 3 && HasBeatenChallenge(GameMode.PuzzleIZombie3))
				{
					mPlayerInfo.mIZombieUnlocked++;
				}
				if (mPlayerInfo.mVasebreakerUnlocked == 3 && HasBeatenChallenge(GameMode.ScaryPotter3))
				{
					mPlayerInfo.mVasebreakerUnlocked++;
				}
				mPlayerInfo.SetLevel(1);
				mPlayerInfo.mFinishedAdventure++;
				if (mPlayerInfo.mFinishedAdventure == 1)
				{
					mPlayerInfo.mNeedsMessageOnGameSelector = true;
					mPlayerInfo.mMiniGamesUnlockable = 20;
					int num = 0;
					if (HasBeatenChallenge(GameMode.ChallengeWarAndPeas))
					{
						num++;
					}
					if (HasBeatenChallenge(GameMode.ChallengeWallnutBowling))
					{
						num++;
					}
					if (HasBeatenChallenge(GameMode.ChallengeSlotMachine))
					{
						num++;
					}
					mPlayerInfo.mMiniGamesUnlocked += num;
				}
			}
			else
			{
				mPlayerInfo.SetLevel(mLevel + 1);
			}
			if (!HasFinishedAdventure() && mLevel == 34)
			{
				mPlayerInfo.mNeedsMagicTacoReward = true;
			}
			if (!HasFinishedAdventure() && mLevel == 44)
			{
				mPlayerInfo.mNeedsMagicBaconReward = true;
			}
			if ((mLevel >= 22 || mPlayerInfo.mFinishedAdventure > 0) && !mPlayerInfo.mHasUnlockedMinigames)
			{
				mPlayerInfo.UnlockFirstMiniGames();
			}
			if ((mLevel >= 36 || mPlayerInfo.mFinishedAdventure > 0) && !mPlayerInfo.mHasUnlockedPuzzleMode)
			{
				mPlayerInfo.UnlockPuzzleMode();
			}
		}
		else if (!IsQuickPlayMode())
		{
			if (IsSurvivalMode())
			{
				if (mBoard.IsFinalSurvivalStage())
				{
					flag = !HasBeatenChallenge(mGameMode);
					mBoard.SurvivalSaveScore();
					if (flag && HasFinishedAdventure())
					{
						int numTrophies = GetNumTrophies(ChallengePage.Survival);
						if (numTrophies != 8 && numTrophies != 9)
						{
							mPlayerInfo.mHasNewSurvival = true;
						}
					}
				}
			}
			else if (IsPuzzleMode())
			{
				flag = !HasBeatenChallenge(mGameMode);
				int currentChallengeIndex = GetCurrentChallengeIndex();
				mPlayerInfo.mChallengeRecords[currentChallengeIndex]++;
				if (!HasFinishedAdventure() && (mGameMode == GameMode.ScaryPotter3 || mGameMode == GameMode.PuzzleIZombie3))
				{
					flag = false;
				}
				if (flag)
				{
					if (IsScaryPotterLevel())
					{
						mPlayerInfo.mHasNewVasebreaker = true;
						mPlayerInfo.mVasebreakerUnlocked++;
						if (mPlayerInfo.mFinishedAdventure == 0 && mPlayerInfo.mVasebreakerUnlocked > 3)
						{
							mPlayerInfo.mVasebreakerUnlocked = 3;
						}
						if (mPlayerInfo.mVasebreakerUnlocked > 10)
						{
							mPlayerInfo.mVasebreakerUnlocked = 10;
						}
					}
					else
					{
						mPlayerInfo.mHasNewIZombie = true;
						mPlayerInfo.mIZombieUnlocked++;
						if (mPlayerInfo.mFinishedAdventure == 0 && mPlayerInfo.mIZombieUnlocked > 3)
						{
							mPlayerInfo.mIZombieUnlocked = 3;
						}
						if (mPlayerInfo.mIZombieUnlocked > 10)
						{
							mPlayerInfo.mIZombieUnlocked = 10;
						}
					}
				}
			}
			else
			{
				flag = !HasBeatenChallenge(mGameMode);
				int currentChallengeIndex2 = GetCurrentChallengeIndex();
				mPlayerInfo.mChallengeRecords[currentChallengeIndex2]++;
				if (mPlayerInfo.mMiniGamesUnlocked < mPlayerInfo.mMiniGamesUnlockable)
				{
					mPlayerInfo.mMiniGamesUnlocked++;
				}
				if (flag && HasFinishedAdventure())
				{
					int numTrophies2 = GetNumTrophies(ChallengePage.Challenge);
					if (numTrophies2 <= 17)
					{
						mPlayerInfo.mHasNewMiniGame = true;
					}
				}
			}
		}
		int numTrophies3 = GetNumTrophies(ChallengePage.Challenge);
		if (numTrophies3 == 20)
		{
			ReportAchievement.GiveAchievement(AchievementId.BeyondTheGrave);
		}
		WriteCurrentUserConfig();
		return flag;
	}

	public bool CanDoDaisyMode()
	{
		if (mPlayerInfo == null)
		{
			return false;
		}
		return mPlayerInfo.mChallengeRecords[48] >= 100;
	}

	public bool CanDoPinataMode()
	{
		if (mPlayerInfo == null)
		{
			return false;
		}
		return mPlayerInfo.mChallengeRecords[48] >= 1000;
	}

	public bool CanDoDanceMode()
	{
		if (mPlayerInfo == null)
		{
			return false;
		}
		return mPlayerInfo.mChallengeRecords[48] >= 500;
	}

	public override void SetSfxVolume(double theVolume)
	{
		base.SetSfxVolume(theVolume);
		mPlayerInfo.mSoundVolume = theVolume;
	}

	public override void SetMusicVolume(double theVolume)
	{
		base.SetMusicVolume(theVolume);
		if (mPlayerInfo != null)
		{
			mPlayerInfo.mMusicVolume = theVolume;
		}
	}

	public bool SaveFileExists()
	{
		string savedGameName = LawnCommon.GetSavedGameName(GameMode.Adventure, (int)mPlayerInfo.mId);
		return FileExists(savedGameName);
	}

	public void Vibrate(TimeSpan? vibrationTime = null)
	{
		if (mPlayerInfo != null && mPlayerInfo.mDoVibration)
		{
			DoVibration(vibrationTime ?? TimeSpan.FromMilliseconds(500.0));
		}
	}

	public override void MoviePlayerContentPreloadDidFinish(bool succeeded)
	{
		if (mCreditScreen != null)
		{
			mCreditScreen.VideoLoaded(succeeded);
		}
	}

	public override void MoviePlayerPlaybackDidFinish()
	{
		if (mCreditScreen != null)
		{
			mCreditScreen.VideoFinished();
		}
	}

	public override void DrawGame(GameTime gameTime)
	{
		GlobalStaticVars.g.BeginFrame();
		mWidgetManager.DrawScreen();
		if (mDebugScreenEnabled)
		{
			DrawDebugInfo(gameTime);
		}
		GlobalStaticVars.g.EndFrame();
		FilterEffect.FilterEffectProcessDeleteQueue();
	}

	public void DrawBlackFrame(Graphics g)
	{
		Graphics.DrawMode drawMode = g.GetDrawMode();
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		Color color = g.GetColor();
		g.SetColor(SexyColor.Black);
		if (mScreenScales.TranslationX > 0f)
		{
			TRect mClipRect = mScreenScales.InvScale(new TRect(0, 0, g.mScreenWidth, g.mScreenHeight));
			mClipRect.mWidth++;
			g.mClipRect = mClipRect;
			TRect theRect = new TRect(mWidgetManager.mWidth, 0, (int)Math.Ceiling((float)(g.mScreenWidth - mScreenScales.mTransX) / mScreenScales.mScaleFactor) - mWidgetManager.mWidth, (int)((float)g.mScreenHeight / mScreenScales.mScaleFactor));
			theRect.mX -= g.mTransX;
			theRect.mY -= g.mTransY;
			g.FillRect(theRect);
			theRect = mScreenScales.InvScale(new TRect(0, 0, mScreenScales.mTransX, 0));
			theRect.mHeight = mWidgetManager.mHeight;
			theRect.mX -= g.mTransX;
			theRect.mY -= g.mTransY;
			g.FillRect(theRect);
			g.ClearClipRect();
		}
		if (mScreenScales.TranslationY > 0f)
		{
			TRect mClipRect2 = mScreenScales.InvScale(new TRect(0, 0, g.mScreenWidth, g.mScreenHeight));
			mClipRect2.mHeight++;
			g.mClipRect = mClipRect2;
			TRect theRect2 = new TRect(0, mWidgetManager.mHeight, (int)((float)g.mScreenWidth / mScreenScales.mScaleFactor), (int)Math.Ceiling((float)(g.mScreenHeight - mScreenScales.mTransY) / mScreenScales.mScaleFactor) - mWidgetManager.mHeight);
			theRect2.mX -= g.mTransX;
			theRect2.mY -= g.mTransY;
			g.FillRect(theRect2);
			theRect2 = mScreenScales.InvScale(new TRect(0, 0, 0, mScreenScales.mTransY));
			theRect2.mWidth = mWidgetManager.mWidth;
			theRect2.mX -= g.mTransX;
			theRect2.mY -= g.mTransY;
			g.FillRect(theRect2);
			g.ClearClipRect();
		}
		g.SetColor(color);
		g.SetDrawMode(drawMode);
	}

	public void DrawDebugInfo(GameTime gameTime)
	{
		Action<string, SexyColor, SexyColor, int, int, int, int> action = (string str, SexyColor textColor, SexyColor outlineColor, int x, int y, int dx, int dy) =>
		{
			GlobalStaticVars.g.SetColor(outlineColor);
			GlobalStaticVars.g.SetFont(Resources.FONT_CONTINUUMBOLD14);
			GlobalStaticVars.g.DrawString(str, x + dx, y + dy);
			GlobalStaticVars.g.SetColor(textColor);
			GlobalStaticVars.g.DrawString(str, x, y);
		};
		float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
		mFrameCounter.Update(deltaTime);
		action($"FPS: {mFrameCounter.AverageFramesPerSecond:N3}", new SexyColor(180, 255, 90), new SexyColor(20, 20, 20), 0, 0, 1, 1);
	}

	public void DoExportData()
	{
		GlobalStaticVars.gPvZActivity.ExportUserdata();
	}

	public int GetAchievementIcon(AchievementId theAchievement)
	{
		return GameConstants.AchievementInfo[(int)theAchievement].mImageId;
	}

	public string GetAchievementName(AchievementId theAchievement)
	{
		return TodStringFile.TodStringTranslate(GameConstants.AchievementInfo[(int)theAchievement].mName);
	}

	public string GetAchievementDescription(AchievementId theAchievement)
	{
		return TodStringFile.TodStringTranslate(GameConstants.AchievementInfo[(int)theAchievement].mDesc);
	}

	public override bool ShouldAutorotateToInterfaceOrientation(UI_ORIENTATION theOrientation)
	{
		if (base.ShouldAutorotateToInterfaceOrientation(theOrientation))
		{
			if (theOrientation != UI_ORIENTATION.UI_ORIENTATION_LANDSCAPE_LEFT)
			{
				return theOrientation == UI_ORIENTATION.UI_ORIENTATION_LANDSCAPE_RIGHT;
			}
			return true;
		}
		return false;
	}

	public bool ShouldInit500p()
	{
		if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS())
		{
			return false;
		}
		return IsWideScreen();
	}

	/// <summary>500p (1000x500) layout only suits phone-shaped screens; iPads (4:3 .. 16:10) need the 600p layout.</summary>
	public static bool IsWideScreen()
	{
		Sexy.Main.GetScreenSize(out int w, out int h);
		return h > 0 && (float)w / (float)h >= 1.85f;
	}
}
