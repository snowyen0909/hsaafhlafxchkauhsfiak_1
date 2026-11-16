using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;
using Sexy;

namespace Lawn;

public class PlayerInfo
{
	private const int saveCheckNumber = 666;

	public string mName;

	public uint mUseSeq;

	public uint mId;

	public int mLevel;

	public int mCoins;

	public int mFinishedAdventure;

	public int[] mChallengeRecords;

	public Dictionary<string, int> mOnlineChallengeRecords = new Dictionary<string, int>();

	internal List<LocalMail> mMail = new List<LocalMail>();

	public int[] mPurchases;

	public int mPlayTimeActivePlayer;

	public int mPlayTimeInactivePlayer;

	public bool mHasUsedCheatKeys;

	public bool mHasWokenStinky;

	public int mDidntPurchasePacketUpgrade;

	public DateTime mLastStinkyChocolateTime = DateTime.MinValue;

	public int mStinkyPosX;

	public int mStinkyPosY;

	public bool mHasUnlockedMinigames;

	public bool mHasUnlockedPuzzleMode;

	public bool mHasNewMiniGame;

	public bool mHasNewVasebreaker;

	public bool mHasNewIZombie;

	public bool mHasNewSurvival;

	public bool mHasUnlockedSurvivalMode;

	public bool mNeedsMessageOnGameSelector;

	public bool mNeedsMagicTacoReward;

	public bool mNeedsMagicBaconReward;

	public bool mNeedsTrialLevelReset;

	public bool mHasSeenStinky;

	public bool mHasSeenUpsell;

	public int[] mPlaceHolderPlayerStats;

	public int mNumPottedPlants;

	public PottedPlant[] mPottedPlant;

	public bool[] mShownAchievements;

	public bool[] mEarnedAchievements;

	public bool mDoVibration;

	public bool mRunWhileLocked;

	public bool mCoinAutoCollect;

	public bool m500pMode;

	public bool mShowName = true;

	private bool _classicSeedChooser;

	public DateTime mLastSeenMoreGames;

	public bool mNeedsGrayedPlantWarning;

	public bool[] mPlantTypesUsed;

	public bool mZenGardenTutorialComplete;

	public bool mIsDaveTalkingZenTutorial;

	public bool mIsInZenTutorial;

	public int mZenTutorialMessage;

	public bool mHasFinishedTutorial;

	public long mZombiesKilled;

	public long mVasebreakerScore;

	public long mIZombieScore;

	public int mMiniGamesUnlocked;

	public int mMiniGamesUnlockable;

	public int mVasebreakerUnlocked;

	public int mIZombieUnlocked;

	public bool mSeenLeaderboardArrow;

	public bool mHasSeenAchievementDialog;

	public double mSoundVolume = 0.8;

	public double mMusicVolume = 0.8;

	public int mMoneySpent;

	private bool mFirstRun;

	public CardGroup[] mCardGroup;

	public string mSaveStateAppVersionNumber;

	public string mHighestAppVersionNumber;

	public List<DateTime> mLastCollectedDynamicCoins = new List<DateTime>();

	public bool mAdventureModeHard;

	public bool FirstRun
	{
		get
		{
			return mFirstRun;
		}
		set
		{
			SexyAppBase.FirstRun = value;
			mFirstRun = false;
		}
	}

	public bool mClassicSeedChooser
	{
		get
		{
			return false;
		}
		set
		{
			_classicSeedChooser = value;
		}
	}

	public ref bool mClassicSeedChooser_Save => ref _classicSeedChooser;

	public PlayerInfo()
	{
		Reset();
	}

	public PlayerInfo(uint id)
		: this()
	{
		mId = id;
	}

	public void Dispose()
	{
	}

	public void Reset()
	{
		mLevel = 1;
		mCoins = 0;
		mFinishedAdventure = 0;
		mAdventureModeHard = false;
		mPottedPlant = new PottedPlant[200];
		for (int i = 0; i < mPottedPlant.Length; i++)
		{
			mPottedPlant[i] = new PottedPlant();
		}
		mShownAchievements = new bool[18];
		mEarnedAchievements = new bool[18];
		mPlantTypesUsed = new bool[54];
		mChallengeRecords = new int[200];
		for (int j = 0; j < 200; j++)
		{
			mChallengeRecords[j] = 0;
		}
		mPurchases = new int[80];
		for (int k = 0; k < 80; k++)
		{
			mPurchases[k] = 0;
		}
		mZombiesKilled = 0L;
		mVasebreakerScore = 0L;
		mIZombieScore = 0L;
		mPlayTimeActivePlayer = 0;
		mPlayTimeInactivePlayer = 0;
		mHasUsedCheatKeys = false;
		mHasWokenStinky = false;
		mDidntPurchasePacketUpgrade = 0;
		mLastStinkyChocolateTime = DateTime.MinValue;
		mStinkyPosX = 0;
		mStinkyPosY = 0;
		mHasUnlockedMinigames = false;
		mHasUnlockedPuzzleMode = false;
		mHasNewMiniGame = false;
		mHasNewVasebreaker = false;
		mHasNewIZombie = false;
		mHasNewSurvival = false;
		mHasUnlockedSurvivalMode = false;
		mNeedsMessageOnGameSelector = false;
		mNeedsMagicTacoReward = false;
		mNeedsMagicBaconReward = false;
		mHasSeenStinky = false;
		mHasSeenUpsell = false;
		mLastSeenMoreGames = default;
		mPlaceHolderPlayerStats = new int[1];
		for (int l = 0; l < 1; l++)
		{
			mPlaceHolderPlayerStats[l] = 0;
		}
		mNumPottedPlants = 0;
		for (int m = 0; m < 18; m++)
		{
			mEarnedAchievements[m] = false;
			mShownAchievements[m] = false;
		}
		mDoVibration = true;
		mRunWhileLocked = Main.RunWhenLocked;
		mCoinAutoCollect = false;
		m500pMode = GlobalStaticVars.gLawnApp.ShouldInit500p();
		mClassicSeedChooser = !m500pMode;
		mNeedsGrayedPlantWarning = true;
		for (int n = 0; n < mPlantTypesUsed.Length; n++)
		{
			mPlantTypesUsed[n] = false;
		}
		mMiniGamesUnlocked = 0;
		mMiniGamesUnlockable = 0;
		mVasebreakerUnlocked = 0;
		mIZombieUnlocked = 0;
		FirstRun = true;
		mZenTutorialMessage = -1;
		mZenGardenTutorialComplete = false;
		mIsDaveTalkingZenTutorial = false;
		mIsInZenTutorial = false;
		mSeenLeaderboardArrow = false;
		mHasSeenAchievementDialog = false;
		mOnlineChallengeRecords.Clear();
		mMail.Clear();
		mCardGroup = new CardGroup[5];
		for (int num = 0; num < mCardGroup.Length; num++)
		{
			mCardGroup[num] = new CardGroup();
		}
		mSaveStateAppVersionNumber = string.Empty;
		mHighestAppVersionNumber = string.Empty;
		mLastCollectedDynamicCoins.Clear();
		for (DynamicTachieWidget.TouchRewards touchRewards = DynamicTachieWidget.TouchRewards.FumeshroomTouch1; touchRewards < DynamicTachieWidget.TouchRewards.NumTouchRewards; touchRewards++)
		{
			mLastCollectedDynamicCoins.Add(DateTime.MinValue);
		}
		LocalMailbox.DeliverLocalMail(this);
	}

	public void AddCoins(int theAmount)
	{
		int num = 999999;
		mCoins += theAmount;
		if (mCoins > num)
		{
			mCoins = num;
		}
		else if (mCoins < 0)
		{
			mCoins = 0;
		}
	}

	public void DeleteUserFiles()
	{
		string theFileName = GlobalStaticVars.GetDocumentsDir() + Common.StrFormat_("userdata/user{0}.dat", mId);
		GlobalStaticVars.gSexyAppBase.EraseFile(theFileName);
		for (int i = 0; i <= 123; i++)
		{
			theFileName = LawnCommon.GetSavedGameName((GameMode)i, (int)mId);
			GlobalStaticVars.gSexyAppBase.EraseFile(theFileName);
		}
	}

	public bool LoadDetailsOld()
	{
		try
		{
			string saveFileName = GetSaveFileName();
			Sexy.Buffer theBuffer = new Sexy.Buffer();
			if (!GlobalStaticVars.gSexyAppBase.ReadBufferFromFile(saveFileName, ref theBuffer, dontWriteToDemo: false))
			{
				return false;
			}
			FirstRun = theBuffer.ReadBoolean();
			mMoneySpent = theBuffer.ReadLong();
			mLastStinkyChocolateTime = theBuffer.ReadDateTime();
			mSoundVolume = theBuffer.ReadDouble();
			mMusicVolume = theBuffer.ReadDouble();
			mZenGardenTutorialComplete = theBuffer.ReadBoolean();
			mIsDaveTalkingZenTutorial = theBuffer.ReadBoolean();
			mIsInZenTutorial = theBuffer.ReadBoolean();
			mZenTutorialMessage = theBuffer.ReadLong();
			mChallengeRecords = theBuffer.ReadLongArray();
			mCoins = theBuffer.ReadLong();
			mDidntPurchasePacketUpgrade = theBuffer.ReadLong();
			mDoVibration = theBuffer.ReadBoolean();
			mRunWhileLocked = theBuffer.ReadBoolean();
			mFinishedAdventure = theBuffer.ReadLong();
			mHasNewIZombie = theBuffer.ReadBoolean();
			mHasNewMiniGame = theBuffer.ReadBoolean();
			mHasNewSurvival = theBuffer.ReadBoolean();
			mHasNewVasebreaker = theBuffer.ReadBoolean();
			mHasSeenStinky = theBuffer.ReadBoolean();
			mHasSeenUpsell = theBuffer.ReadBoolean();
			mHasUnlockedMinigames = theBuffer.ReadBoolean();
			mHasUnlockedPuzzleMode = theBuffer.ReadBoolean();
			mHasUnlockedSurvivalMode = theBuffer.ReadBoolean();
			mHasUsedCheatKeys = theBuffer.ReadBoolean();
			mHasWokenStinky = theBuffer.ReadBoolean();
			mHasFinishedTutorial = theBuffer.ReadBoolean();
			mId = (uint)theBuffer.ReadLong();
			mLevel = theBuffer.ReadLong();
			mName = theBuffer.ReadString();
			mNeedsGrayedPlantWarning = theBuffer.ReadBoolean();
			mNeedsMagicBaconReward = theBuffer.ReadBoolean();
			mNeedsTrialLevelReset = theBuffer.ReadBoolean();
			mNeedsMagicTacoReward = theBuffer.ReadBoolean();
			mNeedsMessageOnGameSelector = theBuffer.ReadBoolean();
			mPlaceHolderPlayerStats = theBuffer.ReadLongArray();
			mPlantTypesUsed = theBuffer.ReadBooleanArray();
			mPlayTimeActivePlayer = theBuffer.ReadLong();
			mPlayTimeInactivePlayer = theBuffer.ReadLong();
			mPurchases = theBuffer.ReadLongArray();
			mShownAchievements = theBuffer.ReadBooleanArray();
			mStinkyPosX = theBuffer.ReadLong();
			mStinkyPosY = theBuffer.ReadLong();
			mUseSeq = (uint)theBuffer.ReadLong();
			mZombiesKilled = theBuffer.ReadLong();
			mVasebreakerScore = theBuffer.ReadLong();
			mIZombieScore = theBuffer.ReadLong();
			mMiniGamesUnlocked = theBuffer.ReadLong();
			mMiniGamesUnlockable = theBuffer.ReadLong();
			mVasebreakerUnlocked = theBuffer.ReadLong();
			mIZombieUnlocked = theBuffer.ReadLong();
			mSeenLeaderboardArrow = theBuffer.ReadBoolean();
			mHasSeenAchievementDialog = theBuffer.ReadBoolean();
			mNumPottedPlants = theBuffer.ReadLong();
			for (int i = 0; i < mNumPottedPlants; i++)
			{
				mPottedPlant[i].Load(theBuffer);
			}
			if (theBuffer.ReadLong() != 666)
			{
				throw new Exception("User profile: Save check number mismatch");
			}
			return true;
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
	}

	public bool LoadDetails()
	{
		string saveFileName = GetSaveFileName();
		BufferNew bufferNew = GlobalStaticVars.gSexyAppBase.ReadBufferNewFromFile(saveFileName, dontWriteToDemo: false);
		if (bufferNew == null)
		{
			return false;
		}
		GlobalMembersSaveGame.SaveGameContext saveGameContext = new GlobalMembersSaveGame.SaveGameContext();
		saveGameContext.mBuffer = bufferNew;
		saveGameContext.mReading = true;
		int version = -1;
		try
		{
			int theInt = 0;
			saveGameContext.SyncInt(ref theInt);
			saveGameContext.SyncSaveFileVersion(ref saveGameContext.mVersion);
			saveGameContext.SyncInt(ref saveGameContext.mVersionType);
			if (theInt != 99816666)
			{
				throw new Exception();
			}
			if (saveGameContext.mVersionType != 20208888)
			{
				throw new Exception();
			}
			saveGameContext.InitializePlayerEnum();
			SyncPlayer(saveGameContext);
			version = (int)saveGameContext.mVersion;
		}
		catch (Exception innerException)
		{
			Reset();
			if (!LoadDetailsOld())
			{
				Debug.Log(DebugType.Info, "Dump of the savestate: " + Convert.ToBase64String(bufferNew.InnerStream.ToArray()));
				GlobalStaticVars.gPvZActivity.OnException(this, new SyncGameException<PlayerInfo>(bufferNew, innerException), keepSilent: true, areStatesDirty: false);
				Reset();
				mId = ProfileMgr.GetNewProfileId();
				return false;
			}
		}
		HandleAfterLoading(version);
		return true;
	}

	public void HandleAfterLoading(int version)
	{
		LocalMailbox.DeliverLocalMail(this);
		SignedInGamer.SignedIn += GamerSignedInCallback;
		if (version == -1 && mMiniGamesUnlockable == 19)
		{
			mMiniGamesUnlockable++;
			mMiniGamesUnlocked++;
		}
	}

	public void GamerSignedInCallback(object sender, SignedInEventArgs args)
	{
		SignedInGamer gamer = args.Gamer;
		mName = gamer.Gamertag;
	}

	private string GetSaveFileName()
	{
		return GlobalStaticVars.GetDocumentsDir() + Common.StrFormat_("userdata/user{0}.dat", mId);
	}

	public void UnlockFirstMiniGames()
	{
		mHasUnlockedMinigames = true;
		mMiniGamesUnlocked = 3;
		mMiniGamesUnlockable = 3;
	}

	public void UnlockPuzzleMode()
	{
		mHasUnlockedPuzzleMode = true;
		mIZombieUnlocked = 1;
		mVasebreakerUnlocked = 1;
	}

	public void UpdateAchievementInfo()
	{
		for (int i = 0; i < mEarnedAchievements.Length; i++)
		{
			AchievementItem achievementItem = Achievements.GetAchievementItem((AchievementId)i);
			if (achievementItem != null)
			{
				mEarnedAchievements[i] = achievementItem.IsEarned;
			}
			else
			{
				mEarnedAchievements[i] = false;
			}
		}
	}

	public bool SaveDetails()
	{
		string saveFileName = GetSaveFileName();
		GlobalMembersSaveGame.SaveGameContext saveGameContext = new GlobalMembersSaveGame.SaveGameContext();
		saveGameContext.mBuffer = new BufferNew();
		saveGameContext.mReading = false;
		int theInt = 99816666;
		saveGameContext.mVersion = SaveFileVersion.MailboxState;
		saveGameContext.mVersionType = 20208888;
		try
		{
			saveGameContext.SyncInt(ref theInt);
			saveGameContext.SyncSaveFileVersion(ref saveGameContext.mVersion);
			saveGameContext.SyncInt(ref saveGameContext.mVersionType);
			saveGameContext.InitializePlayerEnum();
			SyncPlayer(saveGameContext);
			return GlobalStaticVars.gLawnApp.WriteBufferNewToFile(saveFileName, saveGameContext.mBuffer);
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal void SyncPlayer(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		Sync(theContext);
		int theInt = 777;
		theContext.SyncInt(ref theInt);
		if (theInt != 777)
		{
			throw new Exception("Check error");
		}
	}

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncString(ref mName);
		theContext.SyncUint(ref mUseSeq);
		theContext.SyncUint(ref mId);
		theContext.SyncInt(ref mLevel);
		theContext.SyncInt(ref mCoins);
		theContext.SyncInt(ref mFinishedAdventure);
		for (int i = 0; i < 200; i++)
		{
			theContext.SyncInt(ref mChallengeRecords[i]);
		}
		for (int j = 0; j < 80; j++)
		{
			theContext.SyncInt(ref mPurchases[j]);
		}
		theContext.SyncInt(ref mPlayTimeActivePlayer);
		theContext.SyncInt(ref mPlayTimeInactivePlayer);
		theContext.SyncBool(ref mHasUsedCheatKeys);
		theContext.SyncBool(ref mHasWokenStinky);
		theContext.SyncInt(ref mDidntPurchasePacketUpgrade);
		theContext.SyncDateTime(ref mLastStinkyChocolateTime);
		theContext.SyncInt(ref mStinkyPosX);
		theContext.SyncInt(ref mStinkyPosY);
		theContext.SyncBool(ref mHasUnlockedMinigames);
		theContext.SyncBool(ref mHasUnlockedPuzzleMode);
		theContext.SyncBool(ref mHasNewMiniGame);
		theContext.SyncBool(ref mHasNewVasebreaker);
		theContext.SyncBool(ref mHasNewIZombie);
		theContext.SyncBool(ref mHasNewSurvival);
		theContext.SyncBool(ref mHasUnlockedSurvivalMode);
		theContext.SyncBool(ref mNeedsMessageOnGameSelector);
		theContext.SyncBool(ref mNeedsMagicTacoReward);
		theContext.SyncBool(ref mNeedsMagicBaconReward);
		theContext.SyncBool(ref mNeedsTrialLevelReset);
		theContext.SyncBool(ref mHasSeenStinky);
		theContext.SyncBool(ref mHasSeenUpsell);
		for (int k = 0; k < 1; k++)
		{
			theContext.SyncInt(ref mPlaceHolderPlayerStats[k]);
		}
		theContext.SyncInt(ref mNumPottedPlants);
		for (int l = 0; l < 200; l++)
		{
			bool theBool = mPottedPlant[l] != null;
			theContext.SyncBool(ref theBool);
			if (theBool)
			{
				PottedPlant[] array = mPottedPlant;
				int num = l;
				if (array[num] == null)
				{
					array[num] = new PottedPlant();
				}
				mPottedPlant[l].Sync(theContext);
			}
			else
			{
				mPottedPlant[l] = null;
			}
		}
		for (int m = 0; m < 18; m++)
		{
			theContext.SyncBool(ref mShownAchievements[m]);
		}
		for (int n = 0; n < 18; n++)
		{
			theContext.SyncBool(ref mEarnedAchievements[n]);
		}
		theContext.SyncBool(ref mDoVibration);
		theContext.SyncBool(ref mRunWhileLocked);
		theContext.SyncBool(ref m500pMode, SaveFileVersion.Layout500pMode, GlobalStaticVars.gLawnApp.ShouldInit500p());
		theContext.SyncBool(ref mShowName, SaveFileVersion.OptionShowName);
		theContext.SyncBool(ref mAdventureModeHard, SaveFileVersion.AdventureModeHard);
		theContext.SyncBool(ref mClassicSeedChooser_Save, SaveFileVersion.OptionClassicSeedChooser, !m500pMode);
		theContext.SyncDateTime(ref mLastSeenMoreGames);
		theContext.SyncBool(ref mNeedsGrayedPlantWarning);
		for (int num2 = 0; num2 < 49; num2++)
		{
			theContext.SyncBool(ref mPlantTypesUsed[num2]);
		}
		theContext.SyncBool(ref mZenGardenTutorialComplete);
		theContext.SyncBool(ref mIsDaveTalkingZenTutorial);
		theContext.SyncBool(ref mIsInZenTutorial);
		theContext.SyncInt(ref mZenTutorialMessage);
		theContext.SyncBool(ref mHasFinishedTutorial);
		theContext.SyncLong(ref mZombiesKilled, SaveFileVersion.Initial, 0L);
		theContext.SyncLong(ref mVasebreakerScore, SaveFileVersion.Initial, 0L);
		theContext.SyncLong(ref mIZombieScore, SaveFileVersion.Initial, 0L);
		theContext.SyncInt(ref mMiniGamesUnlocked);
		theContext.SyncInt(ref mMiniGamesUnlockable);
		theContext.SyncInt(ref mVasebreakerUnlocked);
		theContext.SyncInt(ref mIZombieUnlocked);
		theContext.SyncBool(ref mSeenLeaderboardArrow);
		theContext.SyncBool(ref mHasSeenAchievementDialog);
		theContext.SyncDouble(ref mSoundVolume);
		theContext.SyncDouble(ref mMusicVolume);
		theContext.SyncInt(ref mMoneySpent);
		theContext.SyncBool(ref mFirstRun);
		if (theContext.mVersion >= SaveFileVersion.CardGroup)
		{
			for (int num3 = 0; num3 < 5; num3++)
			{
				mCardGroup[num3].Sync(theContext);
			}
		}
		if (theContext.mVersion >= SaveFileVersion.RecordAppVersionNumber)
		{
			theContext.SyncString(ref mSaveStateAppVersionNumber);
		}
		if (theContext.mVersion >= SaveFileVersion.FumeshroomDynamic)
		{
			int num4 = theContext.aTouchRewardsSaver.Convert(DynamicTachieWidget.TouchRewards.NumTouchRewards);
			if (num4 < 0)
			{
				throw new Exception("TouchRewards count is missing from EnumSaver.");
			}
			if (theContext.mReading)
			{
				mLastCollectedDynamicCoins.Clear();
				for (int num5 = 0; num5 < 3; num5++)
				{
					mLastCollectedDynamicCoins.Add(DateTime.MinValue);
				}
			}
			for (int num6 = 0; num6 < num4; num6++)
			{
				DateTime theDateTime = (theContext.mReading ? DateTime.MinValue : mLastCollectedDynamicCoins[num6]);
				theContext.SyncDateTime(ref theDateTime);
				if (theContext.mReading)
				{
					DynamicTachieWidget.TouchRewards touchRewards = theContext.aTouchRewardsSaver.Convert(num6);
					int num7 = (int)touchRewards;
					if ((uint)num7 < (uint)mLastCollectedDynamicCoins.Count)
					{
						mLastCollectedDynamicCoins[num7] = theDateTime;
					}
				}
			}
		}
		SyncOnlineChallengeRecords(theContext);
		theContext.SyncBool(ref mCoinAutoCollect, SaveFileVersion.CoinAutoCollect);
		theContext.SyncString(ref mHighestAppVersionNumber, SaveFileVersion.MailboxState, mSaveStateAppVersionNumber);
		LocalMailbox.Sync(theContext, mMail);
	}

	private void SyncOnlineChallengeRecords(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		if (theContext.mVersion < SaveFileVersion.OnlineLevelRecords)
		{
			return;
		}
		int theInt = mOnlineChallengeRecords.Count;
		theContext.SyncListLength(ref theInt);
		if (theContext.mReading)
		{
			mOnlineChallengeRecords.Clear();
			for (int i = 0; i < theInt; i++)
			{
				string theStr = null;
				int theInt2 = 0;
				theContext.SyncString(ref theStr);
				theContext.SyncInt(ref theInt2);
				if (!string.IsNullOrEmpty(theStr))
				{
					mOnlineChallengeRecords[theStr] = theInt2;
				}
			}
			return;
		}
		foreach (KeyValuePair<string, int> mOnlineChallengeRecord in mOnlineChallengeRecords)
		{
			string theStr2 = mOnlineChallengeRecord.Key;
			int theInt3 = mOnlineChallengeRecord.Value;
			theContext.SyncString(ref theStr2);
			theContext.SyncInt(ref theInt3);
		}
	}

	public int GetLevel()
	{
		return mLevel;
	}

	public void SetLevel(int theLevel)
	{
		mLevel = theLevel;
		LocalMailbox.DeliverLocalMail(this);
	}

	public int GetOnlineChallengeRecord(string levelId)
	{
		if (levelId == null || !mOnlineChallengeRecords.TryGetValue(levelId, out var value))
		{
			return 0;
		}
		return value;
	}

	public void SetOnlineChallengeRecord(string levelId, int record)
	{
		if (!string.IsNullOrEmpty(levelId))
		{
			if (record <= 0)
			{
				mOnlineChallengeRecords.Remove(levelId);
			}
			else
			{
				mOnlineChallengeRecords[levelId] = record;
			}
		}
	}

	public void ResetChallengeRecord(GameMode theGameMode)
	{
		int num = (int)(theGameMode - 1);
		Debug.ASSERT(num >= 0 && num < 122);
		mChallengeRecords[num] = 0;
	}

	public void DeleteUserFilesAfterUnlock()
	{
		string theFileName = GlobalStaticVars.GetDocumentsDir() + Common.StrFormat_("userdata/game{0}_0.dat", mId);
		GlobalStaticVars.gSexyAppBase.EraseFile(theFileName);
	}
}
