using System;
using System.Collections.Generic;
using Lawn.Creative;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Zombie : GameObject, IComparable
{
	/// <summary>塔维尔 effects on this zombie (poison, quantum, pulled ...); null when none.</summary>
	public TavelZombie mTavel;

	public enum ZombieRenderLayerOffset
	{
		Bobsled4,
		Bobsled3,
		Bobsled2,
		Bobsled1,
		Normal,
		DogWalker,
		Dog,
		Digger,
		Zamboni
	}

	public enum ZombieParts
	{
		Body,
		Head,
		HeadEating,
		Tongue,
		Arm,
		Hair,
		HeadYucky,
		ArmPickaxe,
		ArmPolevault,
		ArmLeash,
		ArmFlag,
		Pogo,
		Digger
	}

	public MemoryImage mReanimPreview;

	private const int DefaultBodyHealth = 270;

	private const int IZombieImpHealth = 70;

	private const int AdventureBossHealth = 40000;

	private const int ChallengeBossHealth = 30000;

	private const int DefaultBossHealth = 60000;

	private const float FADE_SPEED = 30f;

	private const float FADE_TIME_LAND = 100f;

	private const float FADE_SPEED_BUNGEE = 50f;

	private const float FADE_SPEED_BALOON = 50f;

	public ZombieType mZombieType;

	public ZombiePhase mZombiePhase;

	public float mPosX;

	public float mPosY;

	public float mVelX;

	public int mAnimCounter;

	public int mGroanCounter;

	public int mAnimTicksPerFrame;

	public int mAnimFrames;

	public int mFrame;

	public int mPrevFrame;

	public bool mVariant;

	public bool mIsEating;

	public int mJustGotShotCounter;

	public int mShieldJustGotShotCounter;

	public int mShieldRecoilCounter;

	public int mZombieAge;

	public ZombieHeight mZombieHeight;

	public int mPhaseCounter;

	public int mFromWave;

	public bool mDroppedLoot;

	public int mZombieFade;

	public bool mFlatTires;

	public int mUseLadderCol;

	public int mTargetCol;

	public float mAltitude;

	public bool mHitUmbrella;

	public TRect mZombieRect;

	public TRect mZombieAttackRect;

	public int mChilledCounter;

	public int mButteredCounter;

	public int mIceTrapCounter;

	public bool mMindControlled;

	public bool mBlowingAway;

	public bool mHasHead;

	public bool mHasArm;

	public bool mHasObject;

	public bool mInPool;

	public bool mOnHighGround;

	public bool mYuckyFace;

	public int mYuckyFaceCounter;

	public HelmType mHelmType;

	public int mBodyHealth;

	public int mBodyMaxHealth;

	public int mHelmHealth;

	public int mHelmMaxHealth;

	public ShieldType mShieldType;

	public int mShieldHealth;

	public int mShieldMaxHealth;

	public int mFlyingHealth;

	public int mFlyingMaxHealth;

	public bool mDead;

	public Zombie mRelatedZombieID;

	public int mRelatedZombieID_Save;

	private int mRelatedZombieIDSaved;

	public Zombie[] mFollowerZombieID = new Zombie[GameConstants.MAX_ZOMBIE_FOLLOWERS];

	public int[] mFollowerZombieID_Save = new int[GameConstants.MAX_ZOMBIE_FOLLOWERS];

	private int[] mFollowerZombieIDSaved = new int[GameConstants.MAX_ZOMBIE_FOLLOWERS];

	public Zombie mLeaderZombie;

	public int mLeaderZombie_Save;

	private int mLeaderZombieIDSaved = -1;

	public bool mPlayingSong;

	public int mParticleOffsetX;

	public int mParticleOffsetY;

	public Attachment mAttachmentID;

	public int mAttachmentID_Save;

	public int mSummonCounter;

	public Reanimation mBodyReanimID;

	public int mBodyReanimID_Save;

	public float mScaleZombie;

	public float mVelZ;

	public float mOrginalAnimRate;

	public Plant mTargetPlantID;

	public int mTargetPlantID_Save;

	private int mTargetPlantIDSaved;

	public int mBossMode;

	public int mTargetRow;

	public int mBossBungeeCounter;

	public int mBossStompCounter;

	public int mBossHeadCounter;

	public Reanimation mBossFireBallReanimID;

	public int mBossFireBallReanimID_Save;

	public Reanimation mSpecialHeadReanimID;

	public int mSpecialHeadReanimID_Save;

	public int mFireballRow;

	public bool mIsFireBall;

	public Reanimation mMoweredReanimID;

	public int mMoweredReanimID_Save;

	public int mLastPortalX;

	public bool mHasGroundTrack;

	public bool mSummonedDancers;

	public bool mSurprised;

	private int mGroundTrackIndex;

	public bool mUsesClipping;

	private bool doLoot = true;

	private bool doParticle = true;

	private static Stack<Zombie> unusedObjects;

	private static TodWeightedGridArray[] aPicks;

	public bool mIsButterShowing;

	public bool cachedZombieRectUpToDate;

	private TRect cachedZombieRect;

	public int mYuckyToRow;

	public bool mYuckySwitchRowsLate;

	public bool draggedByTangleKelp;

	private bool justLoaded;

	private string lastPlayedReanimName;

	private ReanimLoopType lastPlayedReanimLoopType;

	private byte lastPlayedReanimBlendTime;

	private float lastPlayedReanimAnimRate;

	public static bool WinningZombieReachedDesiredY;

	public bool mHasHelm = true;

	public bool mHasShield = true;

	public int mAwayCounter;

	public int mSunMoneyLoot;

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncEnum(ref mZombieType, theContext.aZombieTypeSaver, SaveFileVersion.Initial, ZombieType.Normal);
		theContext.SyncEnum(ref mZombiePhase, theContext.aZombiePhaseSaver, SaveFileVersion.Initial, ZombiePhase.ZombieNormal);
		theContext.SyncFloat(ref mPosX);
		theContext.SyncFloat(ref mPosY);
		theContext.SyncFloat(ref mVelX);
		theContext.SyncInt(ref mAnimCounter);
		theContext.SyncInt(ref mGroanCounter);
		theContext.SyncInt(ref mAnimTicksPerFrame);
		theContext.SyncInt(ref mAnimFrames);
		theContext.SyncInt(ref mFrame);
		theContext.SyncInt(ref mPrevFrame);
		theContext.SyncBool(ref mVariant);
		theContext.SyncBool(ref mIsEating);
		theContext.SyncInt(ref mJustGotShotCounter);
		theContext.SyncInt(ref mShieldJustGotShotCounter);
		theContext.SyncInt(ref mShieldRecoilCounter);
		theContext.SyncInt(ref mZombieAge);
		theContext.SyncEnum(ref mZombieHeight, theContext.aZombieHeightSaver, SaveFileVersion.Initial, ZombieHeight.ZombieNormal);
		theContext.SyncInt(ref mPhaseCounter);
		theContext.SyncInt(ref mFromWave);
		theContext.SyncBool(ref mDroppedLoot);
		theContext.SyncInt(ref mZombieFade);
		theContext.SyncBool(ref mFlatTires);
		theContext.SyncInt(ref mUseLadderCol);
		theContext.SyncInt(ref mTargetCol);
		theContext.SyncFloat(ref mAltitude);
		theContext.SyncBool(ref mHitUmbrella);
		theContext.SyncTRect(ref mZombieRect);
		theContext.SyncTRect(ref mZombieAttackRect);
		theContext.SyncInt(ref mChilledCounter);
		theContext.SyncInt(ref mButteredCounter);
		theContext.SyncInt(ref mIceTrapCounter);
		theContext.SyncBool(ref mMindControlled);
		theContext.SyncBool(ref mBlowingAway);
		theContext.SyncBool(ref mHasHead);
		theContext.SyncBool(ref mHasArm);
		theContext.SyncBool(ref mHasObject);
		theContext.SyncBool(ref mInPool);
		theContext.SyncBool(ref mOnHighGround);
		theContext.SyncBool(ref mYuckyFace);
		theContext.SyncInt(ref mYuckyFaceCounter);
		theContext.SyncEnum(ref mHelmType, theContext.aHelmTypeSaver, SaveFileVersion.Initial, HelmType.None);
		theContext.SyncInt(ref mBodyHealth);
		theContext.SyncInt(ref mBodyMaxHealth);
		theContext.SyncInt(ref mHelmHealth);
		theContext.SyncInt(ref mHelmMaxHealth);
		theContext.SyncEnum(ref mShieldType, theContext.aShieldTypeSaver, SaveFileVersion.Initial, ShieldType.None);
		theContext.SyncInt(ref mShieldHealth);
		theContext.SyncInt(ref mShieldMaxHealth);
		theContext.SyncInt(ref mFlyingHealth);
		theContext.SyncInt(ref mFlyingMaxHealth);
		theContext.SyncBool(ref mDead);
		theContext.SyncInt(ref mRelatedZombieID_Save);
		for (int i = 0; i < 4; i++)
		{
			theContext.SyncInt(ref mFollowerZombieID_Save[i]);
		}
		theContext.SyncInt(ref mLeaderZombie_Save);
		theContext.SyncBool(ref mPlayingSong);
		theContext.SyncInt(ref mParticleOffsetX);
		theContext.SyncInt(ref mParticleOffsetY);
		theContext.SyncInt(ref mAttachmentID_Save);
		theContext.SyncInt(ref mSummonCounter);
		theContext.SyncInt(ref mBodyReanimID_Save);
		theContext.SyncFloat(ref mScaleZombie);
		theContext.SyncFloat(ref mVelZ);
		theContext.SyncFloat(ref mOrginalAnimRate);
		theContext.SyncInt(ref mTargetPlantID_Save);
		theContext.SyncInt(ref mBossMode);
		theContext.SyncInt(ref mTargetRow);
		theContext.SyncInt(ref mBossBungeeCounter);
		theContext.SyncInt(ref mBossStompCounter);
		theContext.SyncInt(ref mBossHeadCounter);
		theContext.SyncInt(ref mBossFireBallReanimID_Save);
		theContext.SyncInt(ref mSpecialHeadReanimID_Save);
		theContext.SyncInt(ref mFireballRow);
		theContext.SyncBool(ref mIsFireBall);
		theContext.SyncInt(ref mMoweredReanimID_Save);
		theContext.SyncInt(ref mLastPortalX);
		theContext.SyncBool(ref mHasGroundTrack);
		theContext.SyncBool(ref mSummonedDancers);
		theContext.SyncBool(ref mSurprised);
		theContext.SyncInt(ref mGroundTrackIndex);
		theContext.SyncBool(ref mUsesClipping);
		theContext.SyncBool(ref doLoot);
		theContext.SyncBool(ref doParticle);
		theContext.SyncBool(ref mIsButterShowing);
		theContext.SyncBool(ref cachedZombieRectUpToDate);
		theContext.SyncTRect(ref cachedZombieRect);
		theContext.SyncInt(ref mYuckyToRow);
		theContext.SyncBool(ref mYuckySwitchRowsLate);
		theContext.SyncBool(ref draggedByTangleKelp);
		theContext.SyncBool(ref mHasHelm);
		theContext.SyncBool(ref mHasShield);
		theContext.SyncInt(ref mAwayCounter, SaveFileVersion.RecordAppVersionNumber);
		theContext.SyncInt(ref mSunMoneyLoot, SaveFileVersion.ImitaterRandom);
	}

	internal override void SyncObj(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.SyncObj(theContext);
		TodLibObjSyncer.SyncObjFromList(ref mRelatedZombieID_Save, ref mRelatedZombieID, mBoard.mZombies, theContext.mReading);
		for (int i = 0; i < GameConstants.MAX_ZOMBIE_FOLLOWERS; i++)
		{
			TodLibObjSyncer.SyncObjFromList(ref mFollowerZombieID_Save[i], ref mFollowerZombieID[i], mBoard.mZombies, theContext.mReading);
		}
		TodLibObjSyncer.SyncObjFromList(ref mLeaderZombie_Save, ref mLeaderZombie, mBoard.mZombies, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mAttachmentID_Save, ref mAttachmentID, mApp.mEffectSystem.mAttachmentHolder.mAttachments, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mBodyReanimID_Save, ref mBodyReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mTargetPlantID_Save, ref mTargetPlantID, mBoard.mPlants, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mBossFireBallReanimID_Save, ref mBossFireBallReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mSpecialHeadReanimID_Save, ref mSpecialHeadReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mMoweredReanimID_Save, ref mMoweredReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
	}

	public static void PreallocateMemory()
	{
		for (int i = 0; i < 200; i++)
		{
			new Zombie().PrepareForReuse();
		}
	}

	public static Zombie GetNewZombie()
	{
		if (unusedObjects.Count > 0)
		{
			Zombie zombie = unusedObjects.Pop();
			zombie.Reset();
			return zombie;
		}
		return new Zombie();
	}

	static Zombie()
	{
		unusedObjects = new Stack<Zombie>();
		aPicks = new TodWeightedGridArray[Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY];
		WinningZombieReachedDesiredY = false;
		for (int i = 0; i < aPicks.Length; i++)
		{
			aPicks[i] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
	}

	private Zombie()
	{
		Reset();
	}

	public override void PrepareForReuse()
	{
		if (mReanimPreview != null)
		{
			mReanimPreview.Dispose();
		}
		unusedObjects.Push(this);
	}

	protected override void Reset()
	{
		base.Reset();
		mTavel = null;
		lastPlayedReanimName = string.Empty;
		lastPlayedReanimLoopType = ReanimLoopType.PlayOnce;
		lastPlayedReanimBlendTime = 0;
		lastPlayedReanimAnimRate = 0f;
		doLoot = true;
		doParticle = true;
		draggedByTangleKelp = false;
		cachedZombieRectUpToDate = false;
		mZombieType = ZombieType.Normal;
		mZombiePhase = ZombiePhase.ZombieNormal;
		mPosX = 0f;
		mPosY = 0f;
		mVelX = 0f;
		mAnimCounter = 0;
		mGroanCounter = 0;
		mAnimTicksPerFrame = 0;
		mAnimFrames = 0;
		mFrame = 0;
		mPrevFrame = 0;
		mVariant = false;
		mIsEating = false;
		mJustGotShotCounter = 0;
		mShieldJustGotShotCounter = 0;
		mShieldRecoilCounter = 0;
		mZombieAge = 0;
		mZombieHeight = ZombieHeight.ZombieNormal;
		mPhaseCounter = 0;
		mFromWave = 0;
		mDroppedLoot = false;
		mZombieFade = 0;
		mFlatTires = false;
		mUseLadderCol = 0;
		mTargetCol = 0;
		mAltitude = 0f;
		mHitUmbrella = false;
		mZombieRect = default;
		mZombieAttackRect = default;
		mChilledCounter = 0;
		mButteredCounter = 0;
		mIceTrapCounter = 0;
		mMindControlled = false;
		mBlowingAway = false;
		mHasHead = false;
		mHasArm = false;
		mHasObject = false;
		mInPool = false;
		mOnHighGround = false;
		mYuckyFace = false;
		mYuckyFaceCounter = 0;
		mHelmType = HelmType.None;
		mBodyHealth = 0;
		mBodyMaxHealth = 0;
		mHelmHealth = 0;
		mHelmMaxHealth = 0;
		mShieldType = ShieldType.None;
		mShieldHealth = 0;
		mShieldMaxHealth = 0;
		mFlyingHealth = 0;
		mFlyingMaxHealth = 0;
		mDead = false;
		mRelatedZombieID = null;
		for (int i = 0; i < mFollowerZombieID.Length; i++)
		{
			mFollowerZombieID[i] = null;
		}
		mLeaderZombieIDSaved = -1;
		mLeaderZombie = null;
		mPlayingSong = false;
		mParticleOffsetX = 0;
		mParticleOffsetY = 0;
		mSummonCounter = 0;
		mBodyReanimID = null;
		mScaleZombie = 0f;
		mVelZ = 0f;
		mOrginalAnimRate = 0f;
		mTargetPlantID = null;
		mBossMode = 0;
		mTargetRow = 0;
		mBossBungeeCounter = 0;
		mBossStompCounter = 0;
		mBossHeadCounter = 0;
		mBossFireBallReanimID = null;
		mSpecialHeadReanimID = null;
		mFireballRow = 0;
		mIsFireBall = false;
		mMoweredReanimID = null;
		mLastPortalX = 0;
		mHasGroundTrack = false;
		mSummonedDancers = false;
		mUsesClipping = false;
		mSurprised = false;
		mGroundTrackIndex = -1;
		mHasArm = true;
		mHasHead = true;
		mHasHelm = true;
		mHasShield = true;
		mAwayCounter = 0;
		mSunMoneyLoot = 0;
		mIsButterShowing = false;
		mYuckyToRow = 0;
		mYuckySwitchRowsLate = false;
		if (mReanimPreview != null)
		{
			mReanimPreview.Dispose();
		}
		mReanimPreview = null;
	}

	int IComparable.CompareTo(object toCompare)
	{
		Zombie zombie = (Zombie)toCompare;
		return mX.CompareTo(zombie.mX);
	}

	public void ZombieInitialize(int theRow, ZombieType theType, bool theVariant, Zombie theParentZombie, int theFromWave)
	{
		Debug.ASSERT(theType >= ZombieType.Normal && theType < ZombieType.ZombieTypesCount);
		mRow = theRow;
		mTavel = null;
		mFromWave = theFromWave;
		mPosX = 780f + RandomNumbers.NextNumber(Constants.Zombie_StartRandom_Offset);
		mPosX += Constants.Zombie_StartOffset;
		mPosY = GetPosYBasedOnRow(theRow);
		mWidth = 120;
		mHeight = 120;
		mVelX = 0f;
		mVelZ = 0f;
		mFrame = 0;
		mPrevFrame = 0;
		mZombieType = theType;
		mVariant = theVariant;
		mIsEating = false;
		mJustGotShotCounter = 0;
		mShieldJustGotShotCounter = 0;
		mShieldRecoilCounter = 0;
		mChilledCounter = 0;
		mIceTrapCounter = 0;
		mButteredCounter = 0;
		mMindControlled = false;
		mBlowingAway = false;
		mHasHead = true;
		mHasArm = true;
		mHasObject = false;
		mInPool = false;
		mOnHighGround = false;
		mHelmType = HelmType.None;
		mShieldType = ShieldType.None;
		mYuckyFace = false;
		mYuckyFaceCounter = 0;
		mAnimCounter = 0;
		mGroanCounter = TodCommon.RandRangeInt(400, 500);
		mAnimTicksPerFrame = 12;
		mAnimFrames = 12;
		mZombieAge = 0;
		mTargetCol = -1;
		mZombiePhase = ZombiePhase.ZombieNormal;
		mZombieHeight = ZombieHeight.ZombieNormal;
		mPhaseCounter = 0;
		mHitUmbrella = false;
		mDroppedLoot = false;
		mRelatedZombieID = null;
		mZombieRect = new TRect(36, 0, 42, 115);
		mZombieAttackRect = new TRect(50, 0, 20, 115);
		mPlayingSong = false;
		mZombieFade = -1;
		mFlatTires = false;
		mUseLadderCol = -1;
		mShieldHealth = 0;
		mHelmHealth = 0;
		mFlyingHealth = 0;
		mAttachmentID = null;
		mSummonCounter = 0;
		mBossStompCounter = -1;
		mBossBungeeCounter = -1;
		mBossHeadCounter = -1;
		mBodyReanimID = null;
		mScaleZombie = 1f;
		mAltitude = 0f;
		mOrginalAnimRate = 0f;
		mTargetPlantID = null;
		mBossMode = 0;
		mBossFireBallReanimID = null;
		mSpecialHeadReanimID = null;
		mTargetRow = -1;
		mFireballRow = -1;
		mIsFireBall = false;
		mMoweredReanimID = null;
		mLastPortalX = -1;
		mHasGroundTrack = false;
		mSummonedDancers = false;
		mSurprised = false;
		for (int i = 0; i < GameConstants.MAX_ZOMBIE_FOLLOWERS; i++)
		{
			mFollowerZombieID[i] = null;
		}
		if (mBoard != null && mBoard.IsFlagWave(mFromWave))
		{
			mPosX += 40f;
		}
		PickRandomSpeed();
		mBodyHealth = 270;
		RenderLayer theRenderLayer = RenderLayer.Zombie;
		int theLayerOffset = 4;
		ZombieDefinition zombieDefinition = GetZombieDefinition(mZombieType);
		if (zombieDefinition.mReanimationType != ReanimationType.None)
		{
			LoadReanim(zombieDefinition.mReanimationType);
		}
		switch (mZombieType)
		{
		case ZombieType.Normal:
			LoadPlainZombieReanim();
			break;
		case ZombieType.DuckyTube:
			LoadPlainZombieReanim();
			break;
		case ZombieType.TrafficCone:
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_cone", 0);
			ReanimShowPrefix("anim_hair", -1);
			mHelmType = HelmType.TrafficCone;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			break;
		case ZombieType.Pail:
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_bucket", 0);
			ReanimShowPrefix("anim_hair", -1);
			mHelmType = HelmType.Pail;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			break;
		case ZombieType.Door:
			mShieldType = ShieldType.Door;
			mShieldHealth = GetBaseShieldHealth(mZombieType);
			mPosX += 60f;
			LoadPlainZombieReanim();
			AttachShield();
			break;
		case ZombieType.Yeti:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mPhaseCounter = TodCommon.RandRangeInt(1500, 2000);
			mHasObject = true;
			mZombieAttackRect = new TRect(20, 0, 50, 115);
			mPosX += 60f;
			break;
		case ZombieType.Ladder:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mShieldType = ShieldType.Ladder;
			mShieldHealth = GetBaseShieldHealth(mZombieType);
			mZombieAttackRect = new TRect(10, 0, 50, 115);
			if (IsOnBoard())
			{
				mZombiePhase = ZombiePhase.LadderCarrying;
				StartWalkAnim(0);
			}
			AttachShield();
			break;
		case ZombieType.Bungee:
		{
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mAnimFrames = 4;
			mAltitude = GameConstants.BUNGEE_ZOMBIE_HEIGHT + TodCommon.RandRangeInt(0, 150);
			mVelX = 0f;
			if (IsOnBoard())
			{
				PickBungeeZombieTarget(-1);
				if (mDead)
				{
					return;
				}
				mZombiePhase = ZombiePhase.BungeeDiving;
			}
			else
			{
				mZombiePhase = ZombiePhase.BungeeCutscene;
				mPhaseCounter = TodCommon.RandRangeInt(0, 200);
			}
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_drop, ReanimLoopType.Loop, 0, 24f);
			Reanimation reanimation15 = mApp.ReanimationGet(mBodyReanimID);
			reanimation15.AssignRenderGroupToPrefix("Zombie_bungi_rightarm_lower2", GameConstants.RENDER_GROUP_ARMS);
			reanimation15.AssignRenderGroupToPrefix("Zombie_bungi_rightarm_hand2", GameConstants.RENDER_GROUP_ARMS);
			reanimation15.AssignRenderGroupToPrefix("Zombie_bungi_leftarm_lower2", GameConstants.RENDER_GROUP_ARMS);
			reanimation15.AssignRenderGroupToPrefix("Zombie_bungi_leftarm_hand2", GameConstants.RENDER_GROUP_ARMS);
			reanimation15.SetTruncateDisappearingFrames(string.Empty, theTruncateDisappearingFrames: false);
			theRenderLayer = RenderLayer.GraveStone;
			theLayerOffset = 7;
			mZombieRect = new TRect(-20, 22, 110, 94);
			mZombieAttackRect = new TRect(0, 0, 0, 0);
			mVariant = false;
			break;
		}
		case ZombieType.Football:
			LoadWaterZombieReanim();
			mZombieRect = new TRect(50, 0, 57, 115);
			ReanimShowPrefix("anim_hair", -1);
			mHelmType = HelmType.Football;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			mAnimTicksPerFrame = 6;
			mVariant = false;
			break;
		case ZombieType.FootballPremium:
			LoadWaterZombieReanim();
			mZombieRect = new TRect(50, 0, 57, 115);
			ReanimShowPrefix("anim_hair", -1);
			mHelmType = HelmType.FootballPremium;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			mAnimTicksPerFrame = 6;
			mVariant = false;
			break;
		case ZombieType.Digger:
		{
			mHelmType = HelmType.Digger;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			mVariant = false;
			mHasObject = true;
			mZombieRect = new TRect(50, 0, 28, 115);
			Reanimation reanimation21 = mApp.ReanimationGet(mBodyReanimID);
			reanimation21.SetTruncateDisappearingFrames(string.Empty, theTruncateDisappearingFrames: false);
			if (!IsOnBoard())
			{
				mZombiePhase = ZombiePhase.DiggerCutscene;
				break;
			}
			mZombiePhase = ZombiePhase.DiggerTunneling;
			AddAttachedParticle(60, 100, ParticleEffect.DiggerTunnel);
			theLayerOffset = 7;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_dig, ReanimLoopType.LoopFullLastFrame, 0, 12f);
			PickRandomSpeed();
			break;
		}
		case ZombieType.Polevaulter:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mAnimTicksPerFrame = 6;
			mZombiePhase = ZombiePhase.PolevaulterPreVault;
			mHasObject = true;
			mVariant = false;
			mPosX = Constants.WIDE_BOARD_WIDTH + 70 + RandomNumbers.NextNumber(10);
			if (IsOnBoard())
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_run, ReanimLoopType.Loop, 0, 0f);
				PickRandomSpeed();
			}
			if (mApp.IsWallnutBowlingLevel())
			{
				mZombieAttackRect = new TRect(-229, 0, 270, 115);
			}
			else
			{
				mZombieAttackRect = new TRect(-29, 0, 70, 115);
			}
			break;
		case ZombieType.DolphinRider:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mAnimTicksPerFrame = 6;
			mZombiePhase = ZombiePhase.DolphinWalking;
			mVariant = false;
			if (IsOnBoard())
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_walkdolphin, ReanimLoopType.Loop, 0, 0f);
				PickRandomSpeed();
			}
			SetupWaterTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_dolphinrider_whitewater);
			SetupWaterTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_dolphinrider_dolphininwater);
			break;
		case ZombieType.Gargantuar:
		case ZombieType.RedeyeGargantuar:
		{
			mBodyHealth = GetBaseBodyHealth(ZombieType.Gargantuar, mApp);
			mAnimFrames = 24;
			mAnimTicksPerFrame = 8;
			mWidth = 180;
			mHeight = 180;
			mPosX = Constants.WIDE_BOARD_WIDTH + 45 + RandomNumbers.NextNumber(10);
			mZombieRect = new TRect(-17, -38, 125, 154);
			mZombieAttackRect = new TRect(-30, -38, 89, 154);
			mVariant = false;
			theLayerOffset = 8;
			mHasObject = true;
			int num4 = RandomNumbers.NextNumber(100);
			int num5 = ((IsOnBoard() && mBoard.mLevel != 48) ? ((num4 < 10) ? 2 : ((num4 < 35) ? 1 : 0)) : 0);
			Reanimation reanimation18 = mApp.ReanimationGet(mBodyReanimID);
			switch (num5)
			{
			case 2:
				reanimation18.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantuar_telephonepole, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_ZOMBIE);
				break;
			case 1:
				reanimation18.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantuar_telephonepole, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_DUCKXING);
				break;
			}
			if (mZombieType == ZombieType.RedeyeGargantuar)
			{
				reanimation18.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_HEAD_REDEYE);
				mBodyHealth = GetBaseBodyHealth(ZombieType.RedeyeGargantuar, mApp);
			}
			break;
		}
		case ZombieType.Zamboni:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mAnimFrames = 2;
			mAnimTicksPerFrame = 8;
			mPosX = Constants.WIDE_BOARD_WIDTH + RandomNumbers.NextNumber(10);
			theLayerOffset = 8;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_drive, ReanimLoopType.Loop, 0, 12f);
			mZombieRect = new TRect(0, -13, 153, 140);
			mZombieAttackRect = new TRect(10, -13, 133, 140);
			mVariant = false;
			break;
		case ZombieType.Catapult:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mPosX = Constants.WIDE_BOARD_WIDTH + 25 + RandomNumbers.NextNumber(10);
			mSummonCounter = 20;
			if (IsOnBoard())
			{
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 0, 5.5f);
			}
			else
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 8f);
			}
			mZombieRect = new TRect(0, -13, 153, 140);
			mZombieAttackRect = new TRect(10, -13, 133, 140);
			mVariant = false;
			break;
		case ZombieType.Snorkel:
			mZombieRect = new TRect(12, 0, 62, 115);
			mZombieAttackRect = new TRect(-5, 0, 55, 115);
			SetupWaterTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_snorkle_whitewater);
			SetupWaterTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_snorkle_whitewater2);
			mVariant = false;
			mZombiePhase = ZombiePhase.SnorkelWalking;
			break;
		case ZombieType.JackInTheBox:
		{
			LoadWaterZombieReanim();
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mAnimTicksPerFrame = 6;
			int num = 450 + RandomNumbers.NextNumber(300);
			if (RandomNumbers.NextNumber(20) == 0)
			{
				num /= 3;
			}
			mPhaseCounter = (int)((float)num / mVelX) * GameConstants.ZOMBIE_LIMP_SPEED_FACTOR;
			mZombieAttackRect = new TRect(20, 0, 50, 115);
			if (mApp.IsScaryPotterLevel())
			{
				mPhaseCounter = 10;
			}
			if (IsOnBoard())
			{
				mZombiePhase = ZombiePhase.JackInTheBoxRunning;
			}
			break;
		}
		case ZombieType.Bobsled:
			theLayerOffset = 3;
			if (theParentZombie != null)
			{
				int j;
				for (j = 0; j < 3 && theParentZombie.mFollowerZombieID[j] != null; j++)
				{
				}
				Debug.ASSERT(j < 3);
				theParentZombie.mFollowerZombieID[j] = mBoard.ZombieGetID(this);
				mRelatedZombieID = mBoard.ZombieGetID(theParentZombie);
				mPosX = theParentZombie.mPosX + (float)((j + 1) * 50);
				switch (j)
				{
				case 0:
					theLayerOffset = 1;
					mAltitude = 9f;
					break;
				case 1:
					theLayerOffset = 2;
					mAltitude = -7f;
					break;
				default:
					theLayerOffset = 0;
					mAltitude = 9f;
					break;
				}
			}
			else
			{
				mPosX = Constants.WIDE_BOARD_WIDTH + 80;
				mAltitude = -10f;
				mHelmType = HelmType.Bobsled;
				mHelmHealth = GetBaseHelmHealth(mZombieType);
				mZombieRect = new TRect(-50, 0, 275, 115);
			}
			mVelX = 0.6f;
			mZombiePhase = ZombiePhase.BobsledSliding;
			mPhaseCounter = 500;
			mVariant = false;
			if (theFromWave == GameConstants.ZOMBIE_WAVE_CUTSCENE)
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_jump, ReanimLoopType.PlayOnceAndHold, 0, 20f);
				mApp.ReanimationGet(mBodyReanimID).mAnimTime = 1f;
				mAltitude = 18f;
			}
			else if (IsOnBoard())
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_push, ReanimLoopType.Loop, 0, 30f);
			}
			break;
		case ZombieType.Flag:
		{
			mHasObject = true;
			LoadPlainZombieReanim();
			Reanimation reanimation19 = mApp.ReanimationGet(mBodyReanimID);
			Reanimation reanimation20 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.ZombieFlagpole);
			reanimation20.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_zombie_flag, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation20);
			ReanimatorTrackInstance trackInstanceByName7 = reanimation19.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_flaghand);
			GlobalMembersAttachment.AttachReanim(ref trackInstanceByName7.mAttachmentID, reanimation20, 0f, 0f);
			reanimation19.mFrameBasePose = 0;
			mPosX = Constants.WIDE_BOARD_WIDTH;
			break;
		}
		case ZombieType.Pogo:
			mVariant = false;
			mZombiePhase = ZombiePhase.PogoBouncing;
			mPhaseCounter = RandomNumbers.NextNumber(GameConstants.POGO_BOUNCE_TIME) + 1;
			mHasObject = true;
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mZombieAttackRect = new TRect(10, 0, 30, 115);
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_pogo, ReanimLoopType.PlayOnceAndHold, 0, 40f);
			mApp.ReanimationGet(mBodyReanimID).mAnimTime = 1f;
			break;
		case ZombieType.Newspaper:
			mZombieAttackRect = new TRect(20, 0, 50, 115);
			mZombiePhase = ZombiePhase.NewspaperReading;
			mShieldType = ShieldType.Newspaper;
			mShieldHealth = GetBaseShieldHealth(mZombieType);
			mVariant = false;
			AttachShield();
			break;
		case ZombieType.Balloon:
		{
			Reanimation theAttachReanim = mApp.ReanimationGet(mBodyReanimID);
			theAttachReanim.SetTruncateDisappearingFrames(string.Empty, theTruncateDisappearingFrames: false);
			if (IsOnBoard())
			{
				mZombiePhase = ZombiePhase.BalloonFlying;
				mAltitude = 25f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, theAttachReanim.mAnimRate);
			}
			else
			{
				float animRate2 = TodCommon.RandRangeFloat(8f, 10f);
				SetAnimRate(animRate2);
			}
			Reanimation reanimation12 = mApp.AddReanimation(0f, 0f, 0, zombieDefinition.mReanimationType);
			reanimation12.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_propeller);
			reanimation12.mLoopType = ReanimLoopType.LoopFullLastFrame;
			reanimation12.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_hat);
			mFlyingHealth = GetBaseFlyingHealth(mZombieType);
			mZombieRect = new TRect(36, 30, 42, 115);
			mZombieAttackRect = new TRect(20, 30, 50, 115);
			mVariant = false;
			break;
		}
		case ZombieType.Dancer:
			if (!IsOnBoard())
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_moonwalk, ReanimLoopType.Loop, 0, 12f);
			}
			else
			{
				mZombiePhase = ZombiePhase.DancerDancingIn;
				mPhaseCounter = 300 + RandomNumbers.NextNumber(12);
				mVelX = 0.5f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_moonwalk, ReanimLoopType.Loop, 0, 24f);
			}
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mVariant = false;
			break;
		case ZombieType.BackupDancer:
			if (!IsOnBoard())
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_armraise, ReanimLoopType.Loop, 0, 12f);
			}
			ReanimShowPrefix("Zombie_disco_outerarm_upper_bone", -1);
			mZombiePhase = ZombiePhase.DancerDancingLeft;
			mVariant = false;
			break;
		case ZombieType.Imp:
			if (!IsOnBoard())
			{
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 0, 12f);
			}
			if (mApp.IsIZombieLevel())
			{
				mBodyHealth = 70;
			}
			break;
		case ZombieType.Boss:
			mPosX = Constants.BOARD_EXTRA_ROOM;
			mPosY = 0f;
			mZombieRect = new TRect(700, 80, 90, 430);
			mZombieAttackRect = default;
			theRenderLayer = RenderLayer.Top;
			if (mApp.IsAdventureMode() || mApp.IsQuickPlayMode())
			{
				mBodyHealth = 40000;
			}
			else if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
			{
				mBodyHealth = 30000;
			}
			else
			{
				mBodyHealth = 60000;
			}
			if (IsOnBoard())
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_enter, ReanimLoopType.PlayOnceAndHold, 0, 12f);
				mSummonCounter = 500;
				mBossHeadCounter = 5000;
				mZombiePhase = ZombiePhase.BossEnter;
			}
			else
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 12f);
			}
			BossSetupReanim();
			break;
		case ZombieType.PeaHead:
		{
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_hair", -1);
			ReanimShowPrefix("anim_head2", -1);
			Reanimation reanimation16 = mApp.ReanimationGet(mBodyReanimID);
			if (IsOnBoard())
			{
				reanimation16.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_walk2);
			}
			ReanimatorTrackInstance trackInstanceByName6 = reanimation16.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
			trackInstanceByName6.mImageOverride = AtlasResources.IMAGE_BLANK;
			Reanimation reanimation17 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.PeashooterOriginal);
			reanimation17.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation17);
			AttachEffect attachEffect6 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName6.mAttachmentID, reanimation17, 0f, 0f);
			reanimation16.mFrameBasePose = 0;
			TodCommon.TodScaleRotateTransformMatrix(ref attachEffect6.mOffset.mMatrix, 65f * Constants.S, -8f * Constants.S, 0.2f, -1f, 1f);
			mPhaseCounter = 150;
			mVariant = false;
			break;
		}
		case ZombieType.WallnutHead:
		{
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_hair", -1);
			ReanimShowPrefix("anim_head", -1);
			ReanimShowPrefix("Zombie_tie", -1);
			Reanimation reanimation13 = mApp.ReanimationGet(mBodyReanimID);
			if (IsOnBoard())
			{
				reanimation13.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_walk2);
			}
			ReanimatorTrackInstance trackInstanceByName5 = reanimation13.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_body);
			Reanimation reanimation14 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.WallnutOriginal);
			reanimation14.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation14);
			AttachEffect attachEffect5 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName5.mAttachmentID, reanimation14, 0f, 0f);
			reanimation13.mFrameBasePose = 0;
			TodCommon.TodScaleRotateTransformMatrix(ref attachEffect5.mOffset.mMatrix, 50f * Constants.S, 0f, 0.2f, -0.8f, 0.8f);
			mHelmType = HelmType.Wallnut;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			mVariant = false;
			break;
		}
		case ZombieType.TallnutHead:
		{
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_hair", -1);
			ReanimShowPrefix("anim_head", -1);
			ReanimShowPrefix("Zombie_tie", -1);
			Reanimation reanimation10 = mApp.ReanimationGet(mBodyReanimID);
			if (IsOnBoard())
			{
				reanimation10.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_walk2);
			}
			ReanimatorTrackInstance trackInstanceByName4 = reanimation10.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_body);
			Reanimation reanimation11 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.TallnutOriginal);
			reanimation11.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation11);
			AttachEffect attachEffect4 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName4.mAttachmentID, reanimation11, 0f, 0f);
			reanimation10.mFrameBasePose = 0;
			TodCommon.TodScaleRotateTransformMatrix(ref attachEffect4.mOffset.mMatrix, 37f * Constants.S, 0f, 0.2f, -0.8f, 0.8f);
			mHelmType = HelmType.Tallnut;
			mHelmHealth = GetBaseHelmHealth(mZombieType);
			mVariant = false;
			mPosX += 30f;
			break;
		}
		case ZombieType.JalapenoHead:
		{
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_hair", -1);
			ReanimShowPrefix("anim_head", -1);
			ReanimShowPrefix("Zombie_tie", -1);
			Reanimation reanimation8 = mApp.ReanimationGet(mBodyReanimID);
			if (IsOnBoard())
			{
				reanimation8.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_walk2);
			}
			ReanimatorTrackInstance trackInstanceByName3 = reanimation8.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_body);
			Reanimation reanimation9 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.JalapenoOriginal);
			reanimation9.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation9);
			AttachEffect attachEffect3 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName3.mAttachmentID, reanimation9, 0f, 0f);
			reanimation8.mFrameBasePose = 0;
			TodCommon.TodScaleRotateTransformMatrix(ref attachEffect3.mOffset.mMatrix, 55f * Constants.S, -5f * Constants.S, 0.2f, -1f, 1f);
			mVariant = false;
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			int num3 = 275 + RandomNumbers.NextNumber(175);
			mPhaseCounter = (int)((float)num3 / mVelX) * GameConstants.ZOMBIE_LIMP_SPEED_FACTOR;
			break;
		}
		case ZombieType.GatlingHead:
		{
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_hair", -1);
			ReanimShowPrefix("anim_head2", -1);
			Reanimation reanimation6 = mApp.ReanimationGet(mBodyReanimID);
			if (IsOnBoard())
			{
				reanimation6.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_walk2);
			}
			ReanimatorTrackInstance trackInstanceByName2 = reanimation6.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
			trackInstanceByName2.mImageOverride = AtlasResources.IMAGE_BLANK;
			Reanimation reanimation7 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.GatlingpeaOriginal);
			reanimation7.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation7);
			AttachEffect attachEffect2 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName2.mAttachmentID, reanimation7, 0f, 0f);
			reanimation6.mFrameBasePose = 0;
			TodCommon.TodScaleRotateTransformMatrix(ref attachEffect2.mOffset.mMatrix, 65f * Constants.S, -8f * Constants.S, 0.2f, -1f, 1f);
			mPhaseCounter = 150;
			mVariant = false;
			break;
		}
		case ZombieType.SquashHead:
		{
			LoadPlainZombieReanim();
			ReanimShowPrefix("anim_hair", -1);
			ReanimShowPrefix("anim_head2", -1);
			Reanimation reanimation4 = mApp.ReanimationGet(mBodyReanimID);
			if (IsOnBoard())
			{
				reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_walk2);
			}
			ReanimatorTrackInstance trackInstanceByName = reanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
			trackInstanceByName.mImageOverride = AtlasResources.IMAGE_BLANK;
			Reanimation reanimation5 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.SquashOriginal);
			reanimation5.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
			mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation5);
			AttachEffect attachEffect = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName.mAttachmentID, reanimation5, 0f, 0f);
			reanimation4.mFrameBasePose = 0;
			TodCommon.TodScaleRotateTransformMatrix(ref attachEffect.mOffset.mMatrix, 55f * Constants.S, -15f * Constants.S, 0.2f, -0.75f, 0.75f);
			mZombiePhase = ZombiePhase.SquashPreLaunch;
			mVariant = false;
			break;
		}
		case ZombieType.RobotTitan:
		case ZombieType.RedeyeRobotTitan:
			mBodyHealth = GetBaseBodyHealth(mZombieType, mApp);
			mHelmType = HelmType.RobotTitanHead;
			mHelmHealth = GetBaseHelmHealth(ZombieType.RobotTitan);
			mAnimFrames = 24;
			mAnimTicksPerFrame = 8;
			mWidth = 180;
			mHeight = 180;
			mPosX = Constants.WIDE_BOARD_WIDTH + 45 + RandomNumbers.NextNumber(10);
			mZombieRect = new TRect(-17, -38, 125, 154);
			mZombieAttackRect = new TRect(0, -38, 89, 154);
			mVariant = false;
			theLayerOffset = 8;
			mHasObject = true;
			mVelX *= 0.7f;
			if (mZombieType == ZombieType.RedeyeRobotTitan)
			{
				Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
				reanimation3.SetImageOverride("anim_head", AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_HEAD_REDEYE);
				mHelmType = HelmType.RedeyeRobotTitanHead;
				mHelmHealth = GetBaseHelmHealth(ZombieType.RedeyeRobotTitan);
			}
			break;
		case ZombieType.Monk:
		{
			LoadPlainZombieReanim();
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			reanimation2.AssignRenderGroupToTrack("special", -1);
			mPhaseCounter = 600;
			mZombieRect.mX -= 30;
			mZombieRect.mWidth += 60;
			mBodyHealth *= 2;
			mBodyMaxHealth *= 2;
			mHasArm = false;
			break;
		}
		case ZombieType.Ninja:
			mBodyHealth *= 2;
			mBodyMaxHealth *= 2;
			mHasArm = false;
			break;
		case ZombieType.Talisman:
		{
			mBodyHealth *= 2;
			mHasArm = false;
			int num2 = 350 + RandomNumbers.NextNumber(100);
			if (RandomNumbers.NextNumber(20) == 0)
			{
				num2 /= 3;
			}
			mPhaseCounter = (int)((float)num2 / mVelX) * GameConstants.ZOMBIE_LIMP_SPEED_FACTOR;
			break;
		}
		case ZombieType.Propeller:
		{
			LoadWaterZombieReanim();
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			reanimation.SetTruncateDisappearingFrames(string.Empty, theTruncateDisappearingFrames: false);
			if (IsOnBoard())
			{
				mZombiePhase = ZombiePhase.BalloonFlying;
				mAltitude = 25f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, reanimation.mAnimRate);
			}
			else
			{
				float animRate = TodCommon.RandRangeFloat(8f, 10f);
				SetAnimRate(animRate);
			}
			mFlyingHealth = GetBaseFlyingHealth(mZombieType);
			mBodyHealth *= 2;
			mZombieRect = new TRect(36, 30, 42, 115);
			mZombieAttackRect = new TRect(20, 30, 50, 115);
			mVariant = false;
			break;
		}
		}
		if (IsOnBoard() && mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			float theAnimRate = TodCommon.RandRangeFloat(8f, 10f);
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_aquarium_swim, ReanimLoopType.Loop, 0, theAnimRate);
			mZombieHeight = ZombieHeight.Zombiquarium;
			mZombiePhase = ZombiePhase.ZombiquariumDrift;
			mPhaseCounter = 200;
			mBodyHealth = 200;
			mSummonCounter = TodCommon.RandRangeInt(200, 400);
		}
		if (mApp.IsLevelUseJson())
		{
			ZombieType key = mZombieType;
			CSOverrideZombieProperty component = mApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
			if (component != null && component.mProperties.TryGetValue(key, out var value))
			{
				if (value.mBodyHealth.HasValue)
				{
					mBodyHealth = value.mBodyHealth.GetValueOrDefault();
				}
				if (value.mHelmHealth.HasValue)
				{
					mHelmHealth = value.mHelmHealth.GetValueOrDefault();
				}
				if (value.mShieldHealth.HasValue)
				{
					mShieldHealth = value.mShieldHealth.GetValueOrDefault();
				}
				if (value.mFlyingHealth.HasValue)
				{
					mFlyingHealth = value.mFlyingHealth.GetValueOrDefault();
				}
				if (value.mScaleZombie.HasValue)
				{
					mScaleZombie *= value.mScaleZombie.GetValueOrDefault();
				}
				if (value.mScaleHealth.HasValue)
				{
					float valueOrDefault = value.mScaleHealth.GetValueOrDefault();
					mBodyHealth = (int)((float)mBodyHealth * valueOrDefault);
					mHelmHealth = (int)((float)mHelmHealth * valueOrDefault);
					mShieldHealth = (int)((float)mShieldHealth * valueOrDefault);
					mFlyingHealth = (int)((float)mFlyingHealth * valueOrDefault);
				}
			}
		}
		if (mApp.IsLittleTroubleLevel() && (IsOnBoard() || theFromWave == GameConstants.ZOMBIE_WAVE_CUTSCENE))
		{
			mScaleZombie = 0.5f;
			mBodyHealth /= 4;
			mHelmHealth /= 4;
			mShieldHealth /= 4;
			mFlyingHealth /= 4;
		}
		UpdateAnimSpeed();
		if (mVariant)
		{
			ReanimShowPrefix("anim_tongue", 0);
		}
		mBodyMaxHealth = mBodyHealth;
		mHelmMaxHealth = mHelmHealth;
		mShieldMaxHealth = mShieldHealth;
		mFlyingMaxHealth = mFlyingHealth;
		mDead = false;
		mX = (int)mPosX;
		mY = (int)mPosY;
		mRenderOrder = Board.MakeRenderOrder(theRenderLayer, mRow, theLayerOffset);
		if (mZombieHeight == ZombieHeight.Zombiquarium)
		{
			mBodyMaxHealth = 300;
		}
		if (IsOnBoard())
		{
			PlayZombieAppearSound();
			StartZombieSound();
		}
		UpdateReanim();
		if (mBodyReanimID != null && mBodyReanimID.TrackExists("zombie_butter"))
		{
			mBodyReanimID.AssignRenderGroupToTrack("zombie_butter", -1);
		}
	}

	public void Dispose()
	{
		GlobalMembersAttachment.AttachmentDie(ref mAttachmentID);
		StopZombieSound();
		PrepareForReuse();
	}

	public void Animate()
	{
		mPrevFrame = mFrame;
		if (mZombiePhase == ZombiePhase.JackInTheBoxPopping || mZombiePhase == ZombiePhase.NewspaperMaddening || mZombiePhase == ZombiePhase.DiggerRising || mZombiePhase == ZombiePhase.DiggerTunnelingPauseWithoutAxe || mZombiePhase == ZombiePhase.DiggerRiseWithoutAxe || mZombiePhase == ZombiePhase.DiggerStunned || mZombiePhase == ZombiePhase.TalismanLeaving || IsImmobilizied())
		{
			return;
		}
		mAnimCounter++;
		if (mYuckyFace)
		{
			UpdateYuckyFace();
		}
		if (mIsEating && mHasHead)
		{
			int num = 6;
			if (mChilledCounter > 0)
			{
				num = 12;
			}
			if (mAnimCounter >= mAnimFrames * num)
			{
				mAnimCounter = num;
			}
			mFrame = mAnimCounter / num;
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (reanimation != null)
			{
				float theEventTime = 0.14f;
				float theEventTime2 = 0.68f;
				if (mZombieType == ZombieType.Polevaulter)
				{
					theEventTime = 0.38f;
					theEventTime2 = 0.8f;
				}
				else if (mZombieType == ZombieType.Newspaper || mZombieType == ZombieType.Ladder)
				{
					theEventTime = 0.42f;
					theEventTime2 = 0.42f;
				}
				else if (mZombieType == ZombieType.JackInTheBox)
				{
					theEventTime = 0.53f;
					theEventTime2 = 0.53f;
				}
				else if (mZombieType == ZombieType.Bobsled)
				{
					theEventTime = 0.33f;
					theEventTime2 = 0.83f;
				}
				else if (mZombieType == ZombieType.Imp)
				{
					theEventTime = 0.33f;
					theEventTime2 = 0.79f;
				}
				if (reanimation.ShouldTriggerTimedEvent(theEventTime) || reanimation.ShouldTriggerTimedEvent(theEventTime2))
				{
					AnimateChewSound();
					AnimateChewEffect();
				}
			}
			else
			{
				if (mAnimCounter == 4 * num)
				{
					AnimateChewSound();
				}
				if (mAnimCounter == 7 * num && !mMindControlled)
				{
					AnimateChewEffect();
				}
			}
		}
		else
		{
			if (mAnimCounter >= mAnimFrames * mAnimTicksPerFrame)
			{
				mAnimCounter = 0;
			}
			mFrame = mAnimCounter / mAnimTicksPerFrame;
		}
	}

	public void CheckIfPreyCaught()
	{
		if (mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar || mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Catapult || mZombieType == ZombieType.Boss || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan || IsBouncingPogo() || IsBobsledTeamWithSled() || mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.PolevaulterPreVault || mZombiePhase == ZombiePhase.NewspaperMaddening || mZombiePhase == ZombiePhase.DiggerRising || mZombiePhase == ZombiePhase.DiggerTunnelingPauseWithoutAxe || mZombiePhase == ZombiePhase.DiggerRiseWithoutAxe || mZombiePhase == ZombiePhase.DiggerStunned || mZombiePhase == ZombiePhase.RisingFromGrave || mZombiePhase == ZombiePhase.ImpGettingThrown || mZombiePhase == ZombiePhase.ImpLanding || mZombiePhase == ZombiePhase.DancerRising || mZombiePhase == ZombiePhase.DancerSnappingFingers || mZombiePhase == ZombiePhase.DancerSnappingFingersWithLight || mZombiePhase == ZombiePhase.DancerSnappingFingersHold || mZombiePhase == ZombiePhase.DolphinWalking || mZombiePhase == ZombiePhase.DolphinWalkingWithoutDolphin || mZombiePhase == ZombiePhase.DolphinIntoPool || mZombiePhase == ZombiePhase.DolphinRiding || mZombiePhase == ZombiePhase.DolphinInJump || mZombiePhase == ZombiePhase.SnorkelIntoPool || mZombiePhase == ZombiePhase.SnorkelWalking || mZombiePhase == ZombiePhase.LadderPlacing || mZombieHeight == ZombieHeight.GettingBungeeDropped || mZombieHeight == ZombieHeight.UpLadder || mZombieHeight == ZombieHeight.InToPool || mZombieHeight == ZombieHeight.OutOfPool || IsTangleKelpTarget() || mZombieHeight == ZombieHeight.Falling || mZombiePhase == ZombiePhase.TalismanLeaving || !mHasHead || IsFlying())
		{
			return;
		}
		int num = GameConstants.TICKS_BETWEEN_EATS;
		if (mChilledCounter > 0)
		{
			num *= 2;
		}
		if (mZombieAge % num != 0)
		{
			return;
		}
		Zombie zombie = FindZombieTarget();
		if (zombie != null)
		{
			EatZombie(zombie);
			return;
		}
		if (!mMindControlled)
		{
			Plant plant = FindPlantTarget(ZombieAttackType.Chew);
			if (plant != null)
			{
				EatPlant(plant);
				return;
			}
		}
		if (((!mApp.IsIZombieLevel() && !mApp.IsCornRouletteLevel()) || !mBoard.mChallenge.IZombieEatBrain(this)) && mIsEating)
		{
			StopEating();
		}
	}

	public void EatZombie(Zombie theZombie)
	{
		theZombie.TakeDamage(GameConstants.TICKS_BETWEEN_EATS, 9u);
		StartEating();
		if (theZombie.mBodyHealth <= 0)
		{
			mApp.PlaySample(Resources.SOUND_GULP);
		}
	}

	public void EatPlant(Plant thePlant)
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieEatPlant(this, thePlant);
			return;
		}
		EatPlantCore(thePlant);
	}

	public void EatPlantCore(Plant thePlant)
	{
		if (mZombiePhase == ZombiePhase.DancerDancingIn)
		{
			mPhaseCounter = 1;
		}
		else
		{
			if (mYuckyFace)
			{
				return;
			}
			if (mBoard.GetLadderAt(thePlant.mPlantCol, thePlant.mRow) != null && mZombieType != ZombieType.Digger)
			{
				StopEating();
				if (mZombieHeight == ZombieHeight.ZombieNormal && mUseLadderCol != thePlant.mPlantCol)
				{
					mZombieHeight = ZombieHeight.UpLadder;
					mUseLadderCol = thePlant.mPlantCol;
				}
				return;
			}
			StartEating();
			if ((thePlant.mSeedType == SeedType.PickledPepper || thePlant.mSeedType == SeedType.Jalapeno || thePlant.mSeedType == SeedType.Cherrybomb || thePlant.mSeedType == SeedType.Doomshroom || thePlant.mSeedType == SeedType.Iceshroom || thePlant.mSeedType == SeedType.Hypnoshroom || thePlant.mState == PlantState.FlowerpotInvulnerable || thePlant.mState == PlantState.LilypadInvulnerable || thePlant.mState == PlantState.SquashLook || thePlant.mState == PlantState.SquashPreLaunch) && !thePlant.IsDisabled())
			{
				if (mZombieType == ZombieType.Dancer && thePlant.mSeedType == SeedType.Hypnoshroom)
				{
					mBoard.GrantAchievement(AchievementId.DiscoisUndead);
				}
			}
			else
			{
				if (thePlant.mSeedType == SeedType.Potatomine && thePlant.mState != PlantState.Notready && !thePlant.IsDisabled())
				{
					return;
				}
				bool flag = false;
				if (thePlant.mSeedType == SeedType.Blover && !thePlant.IsDisabled())
				{
					flag = true;
				}
				if (thePlant.mSeedType == SeedType.Iceshroom && !thePlant.IsDisabled())
				{
					flag = true;
				}
				if (flag)
				{
					thePlant.DoSpecial();
				}
				else
				{
					if (mChilledCounter > 0 && mZombieAge % 2 == 1)
					{
						return;
					}
					if (mApp.IsIZombieLevel() && thePlant.mSeedType == SeedType.Sunflower)
					{
						int num = thePlant.mPlantHealth / 40;
						int num2 = (thePlant.mPlantHealth - GameConstants.TICKS_BETWEEN_EATS) / 40;
						if (num2 < num || thePlant.mPlantHealth - GameConstants.TICKS_BETWEEN_EATS <= 0)
						{
							mBoard.AddCoin(thePlant.mX, thePlant.mY, CoinType.Sun, CoinMotion.FromPlant);
						}
					}
					thePlant.mPlantHealth -= GameConstants.TICKS_BETWEEN_EATS;
					thePlant.mRecentlyEatenCountdown = 50;
					if (mApp.IsIZombieLevel() && mJustGotShotCounter < -500 && (thePlant.mSeedType == SeedType.Wallnut || thePlant.mSeedType == SeedType.Tallnut || thePlant.mSeedType == SeedType.Pumpkinshell))
					{
						thePlant.mPlantHealth -= GameConstants.TICKS_BETWEEN_EATS;
					}
					if (thePlant.mSeedType == SeedType.FireShroom && !thePlant.IsDisabled() && mZombieAge % 50 == 0)
					{
						TRect zombieRect = GetZombieRect();
						Reanimation reanimation = mApp.AddReanimation(zombieRect.mX + zombieRect.mWidth / 2, zombieRect.mY + zombieRect.mHeight / 2 - 20, Board.MakeRenderOrder(RenderLayer.Projectile, mRow, 0) + 1, ReanimationType.JalapenoFire);
						reanimation.mAnimTime = 0.25f;
						reanimation.mAnimRate = 24f;
						reanimation.OverrideScale(0.7f, 0.4f);
						mApp.PlayFoley(FoleyType.Ignite);
						RemoveColdEffects();
						TakeDamage(40, 2u);
					}
					if (thePlant.mPlantHealth > 0)
					{
						return;
					}
					mApp.PlaySample(Resources.SOUND_GULP);
					mBoard.mPlantsEaten++;
					if (thePlant.mSeedType == SeedType.Pumpkinshell && thePlant.mLightReanimID != null && thePlant.mAttackingReanimID != null)
					{
						if (thePlant.mState == PlantState.Doingspecial)
						{
							return;
						}
						thePlant.mState = PlantState.Doingspecial;
						thePlant.mDisappearCountdown = 70;
						thePlant.mPlantHealth = 10000;
						mApp.PlaySample(Resources.SOUND_PUMPKIN_SHIELDBROKEN);
						mApp.ReanimationGet(thePlant.mLightReanimID).PlayReanim("anim_idle3_xiaoshi", ReanimLoopType.PlayOnce, 0, 30f);
						mApp.ReanimationGet(thePlant.mAttackingReanimID).PlayReanim("anim_idle3_xiaoshi", ReanimLoopType.PlayOnce, 0, 30f);
					}
					else if (thePlant.mSeedType == SeedType.Tallnut || thePlant.mSeedType == SeedType.ExplodeONut || thePlant.mSeedType == SeedType.IceWallnut)
					{
						thePlant.DoDyingSpecial();
					}
					else
					{
						thePlant.Die();
					}
					mBoard.mChallenge.ZombieAtePlant(this, thePlant);
					if (mBoard.mLevel >= 2 && mBoard.mLevel <= 4 && mApp.IsFirstTimeAdventureMode() && thePlant.mPlantCol > 4 && mBoard.mPlants.Count < 15 && thePlant.mSeedType == SeedType.Peashooter)
					{
						mBoard.DisplayAdvice("[ADVICE_PEASHOOTER_DIED]", MessageStyle.HintTallFast, AdviceType.PeashooterDied);
					}
				}
			}
		}
	}

	public override bool SaveToFile(Sexy.Buffer b)
	{
		base.SaveToFile(b);
		b.WriteLong((int)mZombieType);
		b.WriteBoolean(mVariant);
		b.WriteLong(mFromWave);
		b.WriteFloat(mAltitude);
		b.WriteLong(mAnimCounter);
		b.WriteLong(mAnimFrames);
		b.WriteLong(mAnimTicksPerFrame);
		b.WriteBoolean(mBlowingAway);
		b.WriteLong(mBodyHealth);
		b.WriteLong(mBodyMaxHealth);
		b.WriteLong(mBossBungeeCounter);
		b.WriteLong(mBossHeadCounter);
		b.WriteLong(mBossMode);
		b.WriteLong(mBossStompCounter);
		b.WriteLong(mButteredCounter);
		b.WriteLong(mChilledCounter);
		b.WriteBoolean(mDroppedLoot);
		b.WriteLong(mFireballRow);
		b.WriteBoolean(mFlatTires);
		b.WriteLong(mFlyingHealth);
		b.WriteLong(mFlyingMaxHealth);
		for (int i = 0; i < mFollowerZombieID.Length; i++)
		{
			GameObject.SaveId(mFollowerZombieID[i], b);
		}
		GameObject.SaveId(mLeaderZombie, b);
		b.WriteLong(mFrame);
		b.WriteLong(mGroanCounter);
		b.WriteBoolean(mHasGroundTrack);
		b.WriteBoolean(mHasObject);
		b.WriteLong(mHelmHealth);
		b.WriteLong(mHelmMaxHealth);
		b.WriteBoolean(mHitUmbrella);
		b.WriteLong(mIceTrapCounter);
		b.WriteBoolean(mInPool);
		b.WriteBoolean(mIsEating);
		b.WriteBoolean(mIsFireBall);
		b.WriteLong(mJustGotShotCounter);
		b.WriteLong(mLastPortalX);
		b.WriteBoolean(mMindControlled);
		b.WriteBoolean(mOnHighGround);
		b.WriteFloat(mOrginalAnimRate);
		b.WriteLong(mParticleOffsetX);
		b.WriteLong(mParticleOffsetY);
		b.WriteLong(mPhaseCounter);
		b.WriteBoolean(mPlayingSong);
		b.WriteFloat(mPosX);
		b.WriteFloat(mPosY);
		b.WriteLong(mPrevFrame);
		b.WriteFloat(mPrevTransX);
		b.WriteFloat(mPrevTransY);
		GameObject.SaveId(mRelatedZombieID, b);
		b.WriteFloat(mScaleZombie);
		b.WriteLong(mShieldHealth);
		b.WriteLong(mShieldJustGotShotCounter);
		b.WriteLong(mShieldMaxHealth);
		b.WriteLong(mShieldRecoilCounter);
		b.WriteLong(mSummonCounter);
		b.WriteBoolean(mSummonedDancers);
		b.WriteLong(mTargetCol);
		GameObject.SaveId(mTargetPlantID, b);
		b.WriteLong(mTargetRow);
		b.WriteLong(mUseLadderCol);
		b.WriteBoolean(mUsesClipping);
		b.WriteFloat(mVelX);
		b.WriteFloat(mVelZ);
		b.WriteBoolean(mYuckyFace);
		b.WriteLong(mYuckyFaceCounter);
		b.WriteLong(mZombieAge);
		b.WriteRect(mZombieAttackRect);
		b.WriteLong((int)mZombieHeight);
		b.WriteLong((int)mZombiePhase);
		b.WriteRect(mZombieRect);
		b.WriteString(lastPlayedReanimName);
		b.WriteFloat(lastPlayedReanimAnimRate);
		b.WriteByte(lastPlayedReanimBlendTime);
		b.WriteLong((int)lastPlayedReanimLoopType);
		b.WriteBoolean(mHasArm);
		b.WriteBoolean(mHasHead);
		b.WriteBoolean(mHasHelm);
		b.WriteBoolean(mHasShield);
		return true;
	}

	public override bool LoadFromFile(Sexy.Buffer b)
	{
		base.LoadFromFile(b);
		mZombieType = (ZombieType)b.ReadLong();
		mVariant = b.ReadBoolean();
		mFromWave = b.ReadLong();
		doLoot = false;
		doParticle = false;
		int num = mRow;
		ZombieInitialize(mRow, mZombieType, mVariant, null, mFromWave);
		mAltitude = b.ReadFloat();
		mAnimCounter = b.ReadLong();
		mAnimFrames = b.ReadLong();
		mAnimTicksPerFrame = b.ReadLong();
		mBlowingAway = b.ReadBoolean();
		mBodyHealth = b.ReadLong();
		mBodyMaxHealth = b.ReadLong();
		mBossBungeeCounter = b.ReadLong();
		mBossHeadCounter = b.ReadLong();
		mBossMode = b.ReadLong();
		mBossStompCounter = b.ReadLong();
		mButteredCounter = b.ReadLong();
		mChilledCounter = b.ReadLong();
		mDroppedLoot = b.ReadBoolean();
		mFireballRow = b.ReadLong();
		mFlatTires = b.ReadBoolean();
		mFlyingHealth = b.ReadLong();
		mFlyingMaxHealth = b.ReadLong();
		for (int i = 0; i < mFollowerZombieID.Length; i++)
		{
			mFollowerZombieIDSaved[i] = GameObject.LoadId(b);
		}
		mLeaderZombieIDSaved = GameObject.LoadId(b);
		mFrame = b.ReadLong();
		mGroanCounter = b.ReadLong();
		mHasGroundTrack = b.ReadBoolean();
		mHasObject = b.ReadBoolean();
		mHelmHealth = b.ReadLong();
		mHelmMaxHealth = b.ReadLong();
		mHitUmbrella = b.ReadBoolean();
		mIceTrapCounter = b.ReadLong();
		mInPool = b.ReadBoolean();
		mIsEating = b.ReadBoolean();
		mIsFireBall = b.ReadBoolean();
		mJustGotShotCounter = b.ReadLong();
		mLastPortalX = b.ReadLong();
		mMindControlled = b.ReadBoolean();
		mOnHighGround = b.ReadBoolean();
		mOrginalAnimRate = b.ReadFloat();
		mParticleOffsetX = b.ReadLong();
		mParticleOffsetY = b.ReadLong();
		mPhaseCounter = b.ReadLong();
		mPlayingSong = b.ReadBoolean();
		mPosX = b.ReadFloat();
		mPosY = b.ReadFloat();
		mPrevFrame = b.ReadLong();
		mPrevTransX = b.ReadFloat();
		mPrevTransY = b.ReadFloat();
		mRelatedZombieIDSaved = GameObject.LoadId(b);
		mScaleZombie = b.ReadFloat();
		mShieldHealth = b.ReadLong();
		mShieldJustGotShotCounter = b.ReadLong();
		mShieldMaxHealth = b.ReadLong();
		mShieldRecoilCounter = b.ReadLong();
		mSummonCounter = b.ReadLong();
		mSummonedDancers = b.ReadBoolean();
		mTargetCol = b.ReadLong();
		mTargetPlantIDSaved = GameObject.LoadId(b);
		mTargetRow = b.ReadLong();
		mUseLadderCol = b.ReadLong();
		mUsesClipping = b.ReadBoolean();
		mVelX = b.ReadFloat();
		mVelZ = b.ReadFloat();
		mYuckyFace = b.ReadBoolean();
		mYuckyFaceCounter = b.ReadLong();
		mZombieAge = b.ReadLong();
		mZombieAttackRect = b.ReadRect();
		mZombieHeight = (ZombieHeight)b.ReadLong();
		mZombiePhase = (ZombiePhase)b.ReadLong();
		mZombieRect = b.ReadRect();
		lastPlayedReanimName = b.ReadString();
		lastPlayedReanimAnimRate = b.ReadFloat();
		lastPlayedReanimBlendTime = b.ReadByte();
		lastPlayedReanimLoopType = (ReanimLoopType)b.ReadLong();
		mHasArm = b.ReadBoolean();
		mHasHead = b.ReadBoolean();
		mHasHelm = b.ReadBoolean();
		mHasShield = b.ReadBoolean();
		if (!mHasArm)
		{
			mHasArm = true;
			DropArm(16u);
		}
		if (!mHasHead)
		{
			mHasHead = true;
			DropHead(16u);
		}
		if (!mHasShield && mShieldType != ShieldType.None)
		{
			DropShield(16u);
		}
		if (!mHasHelm && mHelmType != HelmType.None)
		{
			DropHelm(16u);
		}
		if (!mHasObject && mZombieType == ZombieType.Digger)
		{
			DiggerLoseAxe();
		}
		if (mButteredCounter > 0 && !IsZombotany())
		{
			mBodyReanimID.AssignRenderGroupToTrack("zombie_butter", 0);
		}
		if (mZombieType == ZombieType.Bungee)
		{
			SetRow(num);
			mPosX = mBoard.GridToPixelX(mTargetCol, num);
			mPosY = GetPosYBasedOnRow(num);
			mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, num, 7);
		}
		return true;
	}

	public override void LoadingComplete()
	{
		justLoaded = true;
		base.LoadingComplete();
		for (int i = 0; i < mFollowerZombieID.Length; i++)
		{
			mFollowerZombieID[i] = GameObject.GetObjectById(mFollowerZombieIDSaved[i]) as Zombie;
		}
		mLeaderZombie = GameObject.GetObjectById(mLeaderZombieIDSaved) as Zombie;
		mTargetPlantID = GameObject.GetObjectById(mTargetPlantIDSaved) as Plant;
		mRelatedZombieID = GameObject.GetObjectById(mRelatedZombieIDSaved) as Zombie;
		if (mZombieType != ZombieType.Boss || lastPlayedReanimName != "anim_spawn_1")
		{
			PlayZombieReanim(ref lastPlayedReanimName, lastPlayedReanimLoopType, lastPlayedReanimBlendTime, lastPlayedReanimAnimRate);
		}
		int num = mJustGotShotCounter;
		TakeHelmDamage(0, 0u);
		if (IsFlying())
		{
			TakeFlyingDamage(0, 0u);
		}
		TakeShieldDamage(0, 0u);
		TakeBodyDamage(0, 0u);
		TakeDamage(0, 0u);
		UpdateDamageStates(0u);
		mJustGotShotCounter = num;
		if (mZombieType == ZombieType.Boss)
		{
			int bodyDamageIndex = GetBodyDamageIndex();
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			switch (bodyDamageIndex)
			{
			case 1:
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_head, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_HEAD_DAMAGE1);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_jaw, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_JAW_DAMAGE1);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_hand, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_HAND_DAMAGE1);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_thumb2, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_THUMB_DAMAGE1);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_innerleg_foot, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_FOOT_DAMAGE1);
				break;
			case 2:
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_head, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_HEAD_DAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_jaw, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_JAW_DAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_hand, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_HAND_DAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_thumb2, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_THUMB_DAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerleg_foot, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_FOOT_DAMAGE2);
				ApplyBossSmokeParticles(theEnable: true);
				break;
			}
		}
		UpdateReanim();
		doLoot = true;
		doParticle = true;
		justLoaded = false;
	}

	public void RemoveSurprise()
	{
		for (int i = 0; i < mAttachmentID.mEffectArray.Length; i++)
		{
			if (mAttachmentID.mEffectArray[i].mEffectType == EffectType.Reanim)
			{
				Reanimation reanimation = (Reanimation)mAttachmentID.mEffectArray[i].mEffectID;
				if (reanimation.mReanimationType == ReanimationType.ZombieSurprise && reanimation.mLoopCount == 1)
				{
					GlobalMembersAttachment.AttachmentDetach(ref mAttachmentID);
					break;
				}
			}
		}
	}

	public void Update()
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieUpdate(this);
			return;
		}
		UpdateCore();
	}

	public void UpdateCore()
	{
		cachedZombieRectUpToDate = false;
		Debug.ASSERT(!mDead);
		mZombieAge++;
		bool flag = mSurprised;
		if ((mApp.mGameScene != GameScenes.LevelIntro || mZombieType != ZombieType.Boss) && (!IsOnBoard() || !mBoard.mCutScene.ShouldRunUpsellBoard()) && mApp.mGameScene != GameScenes.Playing && IsOnBoard() && mFromWave != GameConstants.ZOMBIE_WAVE_WINNER)
		{
			return;
		}
		if (mZombiePhase == ZombiePhase.ZombieBurned)
		{
			UpdateBurn();
		}
		else if (mZombiePhase == ZombiePhase.ZombieMowered)
		{
			UpdateMowered();
		}
		else if (mZombiePhase == ZombiePhase.ZombieDying)
		{
			UpdateDeath();
			UpdateZombieWalking();
		}
		else
		{
			if (mPhaseCounter > 0 && !IsImmobilizied())
			{
				mPhaseCounter--;
			}
			if (mApp.mGameScene == GameScenes.ZombiesWon)
			{
				if (mBoard.mCutScene.ShowZombieWalking())
				{
					UpdateZombieChimney();
					UpdateZombieWalkingIntoHouse();
				}
			}
			else if (IsOnBoard())
			{
				UpdatePlaying();
			}
			if (mZombieType == ZombieType.Bungee)
			{
				UpdateZombieBungee();
			}
			if (mZombieType == ZombieType.Pogo)
			{
				UpdateZombiePogo();
			}
			Animate();
		}
		mJustGotShotCounter--;
		if (mShieldJustGotShotCounter > 0)
		{
			mShieldJustGotShotCounter--;
		}
		if (mShieldRecoilCounter > 0)
		{
			mShieldRecoilCounter--;
		}
		if (mZombieFade > 0)
		{
			mZombieFade--;
			if (mZombieFade <= 0)
			{
				DieNoLoot(giveAchievements: true);
			}
		}
		mX = (int)mPosX;
		mY = (int)mPosY;
		GlobalMembersAttachment.AttachmentUpdateAndMove(ref mAttachmentID, mPosX, mPosY);
		UpdateReanim();
	}

	public void DieNoLoot(bool giveAchievements)
	{
		StopZombieSound();
		GlobalMembersAttachment.AttachmentDie(ref mAttachmentID);
		mApp.RemoveReanimation(ref mBodyReanimID);
		mApp.RemoveReanimation(ref mMoweredReanimID);
		mApp.RemoveReanimation(ref mSpecialHeadReanimID);
		mDead = true;
		TrySpawnLevelAward();
		if ((mApp.mPlayerInfo != null && mFromWave != GameConstants.ZOMBIE_WAVE_UI) & giveAchievements)
		{
			mApp.mPlayerInfo.mZombiesKilled++;
		}
		if (mZombieType == ZombieType.Bobsled)
		{
			BobsledDie();
		}
		if (mZombieType == ZombieType.Bungee)
		{
			BungeeDie();
		}
		if (mZombieType == ZombieType.Boss)
		{
			BossDie();
		}
		if (giveAchievements && mZombieType == ZombieType.Gargantuar && mBoard != null)
		{
			mBoard.GrantAchievement(AchievementId.CrashoftheTitan);
		}
		if (mLeaderZombie == null || mLeaderZombie.mFollowerZombieID == null)
		{
			return;
		}
		for (int i = 0; i < mLeaderZombie.mFollowerZombieID.Length; i++)
		{
			if (mLeaderZombie.mFollowerZombieID[i] == this)
			{
				mLeaderZombie.mFollowerZombieID[i] = null;
			}
		}
	}

	public void DieWithLoot()
	{
		DieNoLoot(giveAchievements: true);
		if (doLoot)
		{
			DropLoot();
		}
	}

	public void Draw(Graphics g)
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieDraw(this, g);
			return;
		}
		DrawCore(g);
	}

	public void DrawCore(Graphics g)
	{
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		if (mZombieHeight == ZombieHeight.GettingBungeeDropped)
		{
			return;
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		if (mApp.mGameScene == GameScenes.ZombiesWon && !SetupDrawZombieWon(g))
		{
			return;
		}
		if (mIceTrapCounter > 0)
		{
			DrawIceTrap(g, ref theDrawPos, theFront: false);
		}
		if ((mApp.mGameMode != GameMode.ChallengeInvisighoul && mApp.mCreativeLevel?.GetComponent<CSInvisibleZombie>() == null) || mFromWave == GameConstants.ZOMBIE_WAVE_UI)
		{
			if (mBodyReanimID != null)
			{
				DrawReanim(g, ref theDrawPos, 0);
			}
			else
			{
				DrawZombie(g, ref theDrawPos);
			}
		}
		if (mIceTrapCounter > 0)
		{
			DrawIceTrap(g, ref theDrawPos, theFront: true);
		}
		if (mButteredCounter > 0)
		{
			bool flag = mBodyReanimID?.TrackExists("zombie_butter") ?? false;
			if (!mIsButterShowing & flag)
			{
				if (!IsZombotany())
				{
					mBodyReanimID.AssignRenderGroupToTrack("zombie_butter", 0);
				}
				mIsButterShowing = true;
			}
			if (IsZombotany() || mApp.mGameMode == GameMode.ChallengeInvisighoul || mApp.mCreativeLevel?.GetComponent<CSInvisibleZombie>() != null || !flag)
			{
				DrawButter(g, ref theDrawPos);
			}
		}
		else if (mIsButterShowing)
		{
			if (!IsZombotany())
			{
				mBodyReanimID.AssignRenderGroupToTrack("zombie_butter", -1);
			}
			mIsButterShowing = false;
		}
		if (mAttachmentID != null)
		{
			Graphics graphics = Graphics.GetNew(g);
			MakeParentGraphicsFrame(graphics);
			graphics.mTransY += (int)(theDrawPos.mBodyY * Constants.S);
			if (theDrawPos.mClipHeight > GameConstants.CLIP_HEIGHT_LIMIT)
			{
				float num = 120f - theDrawPos.mClipHeight + 21f;
				float mImageOffsetX = theDrawPos.mImageOffsetX;
				float num2 = theDrawPos.mImageOffsetY - 28f;
				graphics.ClipRect((int)(((float)mX + mImageOffsetX - 400f) * Constants.S), (int)(((float)mY + num2) * Constants.S), (int)(920f * Constants.S), (int)(num * Constants.S));
			}
			GlobalMembersAttachment.AttachmentDraw(mAttachmentID, graphics, theParentHidden: false, doScale: true);
			graphics.PrepareForReuse();
		}
		g.ClearClipRect();
	}

	public bool IsZombotany()
	{
		if (mZombieType != ZombieType.SquashHead && mZombieType != ZombieType.WallnutHead && mZombieType != ZombieType.TallnutHead && mZombieType != ZombieType.PeaHead && mZombieType != ZombieType.JalapenoHead)
		{
			return mZombieType == ZombieType.GatlingHead;
		}
		return true;
	}

	public void DrawZombie(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
		Image theImage = null;
		int theRow = 0;
		bool flag = false;
		switch (mZombieType)
		{
		case ZombieType.Normal:
		case ZombieType.Flag:
		case ZombieType.TrafficCone:
		case ZombieType.Pail:
		case ZombieType.Newspaper:
		case ZombieType.Door:
		case ZombieType.Football:
		case ZombieType.DolphinRider:
		case ZombieType.Ladder:
		case ZombieType.FootballPremium:
			flag = true;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		if (flag)
		{
			DrawZombieWithParts(g, ref theDrawPos);
		}
		else
		{
			DrawZombiePart(g, theImage, mFrame, theRow, ref theDrawPos);
		}
	}

	public void DrawZombieWithParts(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
	}

	public void DrawZombiePart(Graphics g, Image theImage, int theFrame, int theRow, ref ZombieDrawPosition theDrawPos)
	{
		int celWidth = theImage.GetCelWidth();
		int celHeight = theImage.GetCelHeight();
		float num = theDrawPos.mImageOffsetX;
		float num2 = theDrawPos.mImageOffsetY + theDrawPos.mBodyY;
		if (mZombiePhase == ZombiePhase.PolevaulterInVault)
		{
			num += -120f;
			num2 += -120f;
		}
		if (mZombiePhase == ZombiePhase.DiggerTunneling)
		{
			num2 += 50f;
		}
		if (mZombieType == ZombieType.Zamboni)
		{
			num2 += -19f;
		}
		float num3 = celHeight;
		if (theDrawPos.mClipHeight > GameConstants.CLIP_HEIGHT_LIMIT)
		{
			num3 = TodCommon.ClampFloat((float)celHeight - theDrawPos.mClipHeight, 0f, celHeight);
		}
		int num4 = 255;
		if (mZombieFade >= 0)
		{
			num4 = TodCommon.ClampInt((int)((float)(255 * mZombieFade) / 30f), 0, 255);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, num4));
		}
		bool flag = false;
		if (mZombiePhase == ZombiePhase.DancerDancingIn || mZombiePhase == ZombiePhase.DancerDancingLeft)
		{
			int dancerFrame = GetDancerFrame();
			if (!mIsEating && (dancerFrame == 12 || dancerFrame == 13 || dancerFrame == 14 || dancerFrame == 18 || dancerFrame == 19 || dancerFrame == 20))
			{
				flag = true;
				num -= 30f;
			}
		}
		if (flag)
		{
			num = 0f - num;
		}
		TRect theSrcRect = new TRect(theFrame * celWidth, theRow * celHeight, celWidth, (int)num3);
		TRect theDestRect = new TRect((int)num, (int)num2, celWidth, (int)num3);
		if (mZombiePhase == ZombiePhase.ZombieBurned)
		{
			if (mMindControlled)
			{
				flag = true;
			}
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(SexyColor.Black);
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
		}
		else if (mMindControlled)
		{
			flag = true;
			g.SetColorizeImages(colorizeImages: true);
			SexyColor zOMBIE_MINDCONTROLLED_COLOR = GameConstants.ZOMBIE_MINDCONTROLLED_COLOR;
			zOMBIE_MINDCONTROLLED_COLOR.mAlpha = num4;
			g.SetColor(zOMBIE_MINDCONTROLLED_COLOR);
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		}
		else if (mChilledCounter > 0 || mIceTrapCounter > 0)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(75, 75, 255, num4));
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		}
		else
		{
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
		}
		if (mJustGotShotCounter > 0)
		{
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			g.SetColorizeImages(colorizeImages: true);
			int num5 = mJustGotShotCounter * 10;
			g.SetColor(new SexyColor(num5, num5, num5, 255));
			g.DrawImageMirror(theImage, theDestRect, theSrcRect, flag);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public void DrawBungeeCord(Graphics g, int theOffsetX, int theOffsetY)
	{
		int num = (int)((float)AtlasResources.IMAGE_BUNGEECORD.GetCelHeight() * mScaleZombie);
		float thePosX = 0f;
		float thePosY = 0f;
		GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_bungi_body, ref thePosX, ref thePosY);
		bool flag = false;
		if (IsOnBoard() && mApp.IsFinalBossLevel())
		{
			Zombie bossZombie = mBoard.GetBossZombie();
			int num2 = 55;
			if (bossZombie.mZombiePhase == ZombiePhase.BossBungeesLeave)
			{
				Reanimation reanimation = mApp.ReanimationGet(bossZombie.mBodyReanimID);
				num2 = (int)TodCommon.TodAnimateCurveFloatTime(0f, 0.2f, reanimation.mAnimTime, 55f, 0f, TodCurves.Linear);
			}
			if (mTargetCol > bossZombie.mTargetCol)
			{
				g.SetClipRect(new TRect(-g.mTransX, (int)((float)num2 * Constants.S) - g.mTransY, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT_GAME));
				flag = true;
			}
		}
		for (float num3 = thePosY - (float)num; num3 > (float)(-num); num3 -= (float)num)
		{
			float thePosX2 = (float)(theOffsetX + Constants.Zombie_Bungee_Offset.X) - 4f / mScaleZombie;
			float thePosY2 = num3 - mPosY - (float)Constants.Zombie_Bungee_Offset.Y;
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_BUNGEECORD, thePosX2, thePosY2, mScaleZombie, mScaleZombie);
		}
		if (mZombiePhase == ZombiePhase.ZombieBurned)
		{
			g.SetColor(g.mColor, premultiply: false);
			g.SetColorizeImages(g.mColorizeImages);
		}
		if (flag)
		{
			g.ClearClipRect();
		}
	}

	public void TakeDamage(int theDamage, uint theDamageFlags)
	{
		if (mZombiePhase == ZombiePhase.JackInTheBoxPopping || IsDeadOrDying())
		{
			return;
		}
		int num = theDamage;
		if (IsFlying())
		{
			num = TakeFlyingDamage(theDamage, theDamageFlags);
		}
		if (num > 0 && mShieldType != ShieldType.None && !TodCommon.TestBit(theDamageFlags, 0))
		{
			num = TakeShieldDamage(theDamage, theDamageFlags);
			if (TodCommon.TestBit(theDamageFlags, 1))
			{
				num = theDamage;
			}
		}
		if (num > 0 && mHelmType != HelmType.None)
		{
			num = TakeHelmDamage(theDamage, theDamageFlags);
		}
		if (num > 0)
		{
			TakeBodyDamage(num, theDamageFlags);
		}
	}

	public void SetRow(int theRow)
	{
		Debug.ASSERT(theRow >= 0 && theRow < Constants.MAX_GRIDSIZEY);
		mRow = theRow;
		mRenderOrder = Board.MakeRenderOrder(RenderLayer.Zombie, mRow, 4);
	}

	private static int GetRowHeightOffset(ZombieType type)
	{
		switch (type)
		{
		case ZombieType.Balloon:
		case ZombieType.Propeller:
			return -30;
		case ZombieType.Pogo:
			return -16;
		default:
			return 0;
		}
	}

	internal static TPoint GetReanimationDrawOffset(ZombieType type)
	{
		switch (type)
		{
		case ZombieType.Football:
		case ZombieType.FootballPremium:
			return new TPoint(0, -16);
		case ZombieType.Yeti:
			return new TPoint(0, -20);
		case ZombieType.Catapult:
			return new TPoint(-25, -18);
		case ZombieType.Pogo:
			return new TPoint(0, 16);
		case ZombieType.Balloon:
			return new TPoint(0, 17);
		case ZombieType.Propeller:
			return new TPoint(0, 30);
		case ZombieType.Polevaulter:
			return new TPoint(-6, -11);
		case ZombieType.Zamboni:
			return new TPoint(68, -23);
		case ZombieType.Gargantuar:
		case ZombieType.RedeyeGargantuar:
			return new TPoint(0, -8);
		case ZombieType.Bobsled:
			return new TPoint(0, -12);
		case ZombieType.Dancer:
		case ZombieType.BackupDancer:
			return new TPoint(0, 15);
		default:
			return new TPoint(0, 0);
		}
	}

	public static bool IsPlacementPreviewMirrored(ZombieType type, bool mindControlled)
	{
		return type == ZombieType.Dancer != mindControlled;
	}

	public static Vector2 GetPlacementPreviewOrigin(Board board, ZombieType type, int col, int row, bool mindControlled = false)
	{
		float num = board.GridToPixelX(col, row) - ((type != ZombieType.Bungee) ? 30 : 0);
		float num2 = board.GetPosYBasedOnRow(num + 40f, row) - 30f + (float)GetRowHeightOffset(type);
		TPoint reanimationDrawOffset = GetReanimationDrawOffset(type);
		num += (float)(reanimationDrawOffset.x + 15);
		num2 += (float)(reanimationDrawOffset.y - 8);
		if (type == ZombieType.Balloon || type == ZombieType.Propeller)
		{
			num2 -= 25f;
		}
		if (type == ZombieType.Bungee)
		{
			num -= 18f;
		}
		if (IsPlacementPreviewMirrored(type, mindControlled))
		{
			num += 90f;
		}
		return new Vector2(num, num2);
	}

	public float GetPosYBasedOnRow(int theRow)
	{
		if (!IsOnBoard())
		{
			return 0f;
		}
		if (IsOnHighGround())
		{
			if (mAltitude < (float)Constants.HIGH_GROUND_HEIGHT)
			{
				mZombieHeight = ZombieHeight.UpToHighGround;
			}
			mOnHighGround = true;
		}
		float num = mBoard.GetPosYBasedOnRow(mPosX + 40f, theRow) - 30f;
		return num + (float)GetRowHeightOffset(mZombieType);
	}

	public void ApplyChill(bool theIsIceTrap)
	{
		if (CanBeChilled())
		{
			if (mChilledCounter == 0)
			{
				mApp.PlayFoley(FoleyType.Frozen);
			}
			int val = 1000;
			if (theIsIceTrap)
			{
				val = 2000;
			}
			mChilledCounter = Math.Max(val, mChilledCounter);
			UpdateAnimSpeed();
		}
	}

	public void UpdateZombieBungee()
	{
		if (IsDeadOrDying() || IsImmobilizied())
		{
			return;
		}
		if (mZombiePhase == ZombiePhase.BungeeDiving || mZombiePhase == ZombiePhase.BungeeDivingScreaming)
		{
			float num = (float)GameConstants.BUNGEE_ZOMBIE_HEIGHT - 404f;
			float num2 = mAltitude;
			mAltitude -= 8f;
			if (mAltitude <= num && num2 > num && mRelatedZombieID == null)
			{
				mApp.PlayFoley(FoleyType.Grassstep);
			}
			BungeeLanding();
		}
		else if (mZombiePhase == ZombiePhase.BungeeAtBottom)
		{
			if (mPhaseCounter <= 0)
			{
				BungeeStealTarget();
				mZombiePhase = ZombiePhase.BungeeGrabbing;
			}
		}
		else if (mZombiePhase == ZombiePhase.BungeeGrabbing)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				BungeeLiftTarget();
				mZombiePhase = ZombiePhase.BungeeRising;
			}
		}
		else if (mZombiePhase == ZombiePhase.BungeeHitOuchy)
		{
			if (mPhaseCounter <= 0)
			{
				DieWithLoot();
			}
		}
		else if (mZombiePhase == ZombiePhase.BungeeRising)
		{
			mAltitude += 8f;
			if (mAltitude >= 600f)
			{
				DieNoLoot(giveAchievements: false);
			}
		}
		else if (mZombiePhase == ZombiePhase.BungeeCutscene)
		{
			mAltitude = TodCommon.TodAnimateCurve(200, 0, mPhaseCounter, 40, 0, TodCurves.SinWave);
			if (mPhaseCounter <= 0)
			{
				mPhaseCounter = 200;
			}
		}
		mX = (int)mPosX;
		mY = (int)mPosY;
	}

	public void BungeeLanding()
	{
		if (mZombiePhase == ZombiePhase.BungeeDiving && mAltitude < 1500f && !mApp.IsFinalBossLevel())
		{
			mApp.PlayFoley(FoleyType.BungeeScream);
			mZombiePhase = ZombiePhase.BungeeDivingScreaming;
		}
		if (mAltitude > 40f)
		{
			return;
		}
		Plant plant = mBoard.FindUmbrellaPlant(mTargetCol, mRow);
		if (plant != null)
		{
			mApp.PlaySample(Resources.SOUND_BOING);
			mApp.PlayFoley(FoleyType.Umbrella);
			plant.DoSpecial();
			mZombiePhase = ZombiePhase.BungeeRising;
			mRenderOrder = Board.MakeRenderOrder(RenderLayer.Top, 0, 1);
			mHitUmbrella = true;
			return;
		}
		mBoard.GetTopPlantAt(mTargetCol, mRow, TopPlant.BungeeOrder);
		if (mAltitude > 0f)
		{
			return;
		}
		mAltitude = 0f;
		Zombie zombie = null;
		int num = mBoard.mZombies.IndexOf(mRelatedZombieID);
		if (num != -1)
		{
			zombie = mBoard.mZombies[num];
		}
		if (zombie != null)
		{
			zombie.mZombieHeight = ZombieHeight.ZombieNormal;
			if (mApp.mCreativeLevel?.GetComponent<CSBungeeBlitz>() != null)
			{
				Reanimation reanimation = mApp.ReanimationTryToGet(zombie.mBodyReanimID);
				if (reanimation != null && reanimation.IsAnimPlaying("anim_idle"))
				{
					zombie.StartWalkAnim(0);
				}
				else
				{
					zombie.UpdateAnimSpeed();
				}
			}
			else
			{
				zombie.StartWalkAnim(0);
			}
			mRelatedZombieID = null;
			mZombiePhase = ZombiePhase.BungeeRising;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_raise, ReanimLoopType.PlayOnceAndHold, 0, 36f);
		}
		else
		{
			mZombiePhase = ZombiePhase.BungeeAtBottom;
			mPhaseCounter = 300;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 5, 24f);
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			reanimation2.mAnimTime = 0.5f;
		}
	}

	public bool EffectedByDamage(uint theDamageRangeFlags)
	{
		if (!TodCommon.TestBit(theDamageRangeFlags, 5) && IsDeadOrDying())
		{
			return false;
		}
		if (TodCommon.TestBit(theDamageRangeFlags, 7))
		{
			if (!mMindControlled)
			{
				return false;
			}
		}
		else if (mMindControlled)
		{
			return false;
		}
		if (mZombieType == ZombieType.Bungee && mZombiePhase != ZombiePhase.BungeeAtBottom && mZombiePhase != ZombiePhase.BungeeGrabbing)
		{
			return false;
		}
		if (mZombieHeight == ZombieHeight.GettingBungeeDropped)
		{
			return false;
		}
		if (mZombieType == ZombieType.Boss)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (mZombiePhase == ZombiePhase.BossHeadEnter && reanimation.mAnimTime < 0.5f)
			{
				return false;
			}
			if (mZombiePhase == ZombiePhase.BossHeadLeave && reanimation.mAnimTime > 0.5f)
			{
				return false;
			}
			if (mZombiePhase != ZombiePhase.BossHeadIdleBeforeSpit && mZombiePhase != ZombiePhase.BossHeadIdleAfterSpit && mZombiePhase != ZombiePhase.BossHeadSpit)
			{
				return false;
			}
		}
		if (mZombieType == ZombieType.Bobsled && GetBobsledPosition() > 0)
		{
			return false;
		}
		if (mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.ImpGettingThrown || mZombiePhase == ZombiePhase.DiggerRising || mZombiePhase == ZombiePhase.DiggerTunnelingPauseWithoutAxe || mZombiePhase == ZombiePhase.DiggerRiseWithoutAxe || mZombiePhase == ZombiePhase.DolphinIntoPool || mZombiePhase == ZombiePhase.DolphinInJump || mZombiePhase == ZombiePhase.SnorkelIntoPool || mZombiePhase == ZombiePhase.BalloonPopping || mZombiePhase == ZombiePhase.RisingFromGrave || mZombiePhase == ZombiePhase.BobsledCrashing || mZombiePhase == ZombiePhase.DancerRising || mZombiePhase == ZombiePhase.TalismanLeaving)
		{
			return TodCommon.TestBit(theDamageRangeFlags, 4);
		}
		if (mZombieType != ZombieType.Bobsled && mZombieType != ZombieType.Boss && GetZombieRect().mX > Constants.WIDE_BOARD_WIDTH)
		{
			return false;
		}
		bool flag = mZombieType == ZombieType.Snorkel && mInPool && !mIsEating;
		if (TodCommon.TestBit(theDamageRangeFlags, 2) & flag)
		{
			return true;
		}
		bool flag2 = mZombiePhase == ZombiePhase.DiggerTunneling || (mZombieType == ZombieType.Ninja && mZombiePhase != ZombiePhase.NinjaShownByPlantern);
		if (!(TodCommon.TestBit(theDamageRangeFlags, 6) & flag2) && (!TodCommon.TestBit(theDamageRangeFlags, 1) || !IsFlying()))
		{
			if (TodCommon.TestBit(theDamageRangeFlags, 0) && !IsFlying() && !flag)
			{
				return !flag2;
			}
			return false;
		}
		return true;
	}

	public void PickRandomSpeed()
	{
		if (mZombiePhase == ZombiePhase.SnorkelWalkingInPool)
		{
			mVelX = 0.3f;
		}
		else if (mZombiePhase == ZombiePhase.DiggerWalking)
		{
			if (mApp.IsIZombieLevel())
			{
				mVelX = 0.23f;
			}
			else
			{
				mVelX = 0.12f;
			}
		}
		else if (mZombieType == ZombieType.Imp && mApp.IsIZombieLevel())
		{
			mVelX = 0.9f;
		}
		else if (mZombiePhase == ZombiePhase.YetiRunning)
		{
			mVelX = 0.8f;
		}
		else if (mZombieType == ZombieType.Yeti)
		{
			mVelX = 0.4f;
		}
		else if (mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer || mZombieType == ZombieType.Pogo || mZombieType == ZombieType.Flag)
		{
			mVelX = 0.45f;
		}
		else if (mZombiePhase == ZombiePhase.DiggerTunneling || mZombiePhase == ZombiePhase.PolevaulterPreVault || mZombieType == ZombieType.Football || mZombieType == ZombieType.Snorkel || mZombieType == ZombieType.JackInTheBox || mZombieType == ZombieType.FootballPremium)
		{
			mVelX = TodCommon.RandRangeFloat(0.66f, 0.68f);
		}
		else if (mZombiePhase == ZombiePhase.LadderCarrying || mZombieType == ZombieType.SquashHead)
		{
			mVelX = TodCommon.RandRangeFloat(0.79f, 0.81f);
		}
		else if (mZombiePhase == ZombiePhase.NewspaperMad || mZombiePhase == ZombiePhase.DolphinWalking || mZombiePhase == ZombiePhase.DolphinWalkingWithoutDolphin)
		{
			mVelX = TodCommon.RandRangeFloat(0.89f, 0.91f);
		}
		else
		{
			mVelX = TodCommon.RandRangeFloat(0.23f, 0.37f);
			if ((double)mVelX < 0.3)
			{
				mAnimTicksPerFrame = 12;
			}
			else
			{
				mAnimTicksPerFrame = 15;
			}
		}
		UpdateAnimSpeed();
	}

	public void UpdateZombiePolevaulter()
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieRunWithSwgAsTallnut(this, UpdateZombiePolevaulterCore);
			return;
		}
		UpdateZombiePolevaulterCore();
	}

	public void UpdateZombiePolevaulterCore()
	{
		if (mZombiePhase == ZombiePhase.PolevaulterPreVault && mHasHead && mZombieHeight == ZombieHeight.ZombieNormal)
		{
			Plant plant = FindPlantTarget(ZombieAttackType.Vault);
			if (plant != null)
			{
				if (mBoard.GetLadderAt(plant.mPlantCol, plant.mRow) != null)
				{
					if ((float)(mBoard.GridToPixelX(plant.mPlantCol, plant.mRow) + 40) > mPosX && mZombieHeight == ZombieHeight.ZombieNormal && mUseLadderCol != plant.mPlantCol)
					{
						mZombieHeight = ZombieHeight.UpLadder;
						mUseLadderCol = plant.mPlantCol;
					}
					return;
				}
				mZombiePhase = ZombiePhase.PolevaulterInVault;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_jump, ReanimLoopType.PlayOnceAndHold, 20, 24f);
				Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
				float num = (float)reanimation.mFrameCount / reanimation.mAnimRate * 100f;
				int num2 = mX - plant.mX - 80;
				if (mApp.IsWallnutBowlingLevel())
				{
					num2 = 0;
				}
				mVelX = (float)num2 / num;
				mHasObject = false;
			}
			if (mApp.IsIZombieLevel() && mBoard.mChallenge.IZombieGetBrainTarget(this) != null)
			{
				mZombiePhase = ZombiePhase.PolevaulterPostVault;
				StartWalkAnim(0);
			}
		}
		else
		{
			if (mZombiePhase != ZombiePhase.PolevaulterInVault)
			{
				return;
			}
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			bool flag = false;
			if (reanimation2.mAnimTime > 0.6f && reanimation2.mAnimTime <= 0.7f)
			{
				Plant plant2 = FindPlantTarget(ZombieAttackType.Vault);
				if (plant2 != null && plant2.mSeedType == SeedType.Tallnut)
				{
					mApp.PlayFoley(FoleyType.Bonk);
					flag = true;
					mApp.AddTodParticle(plant2.mX + 60, plant2.mY - 20, mRenderOrder + 1, ParticleEffect.TallNutBlock);
					mPosX = plant2.mX;
					mPosY -= 30f;
					mZombieHeight = ZombieHeight.Falling;
				}
			}
			if (reanimation2.mLoopCount > 0)
			{
				flag = true;
				mPosX -= 150f;
			}
			if (reanimation2.ShouldTriggerTimedEvent(0.2f))
			{
				mApp.PlayFoley(FoleyType.Grassstep);
			}
			if (reanimation2.ShouldTriggerTimedEvent(0.4f))
			{
				mApp.PlayFoley(FoleyType.Polevault);
			}
			if (flag)
			{
				mX = (int)mPosX;
				mZombiePhase = ZombiePhase.PolevaulterPostVault;
				mZombieAttackRect = new TRect(50, 0, 20, 115);
				StartWalkAnim(0);
			}
			else
			{
				float num3 = mPosX;
				mPosX -= 150f * reanimation2.mAnimTime;
				mPosY = GetPosYBasedOnRow(mRow);
				mPosX = num3;
			}
		}
	}

	public void UpdateZombieDolphinRider()
	{
		if (IsTangleKelpTarget())
		{
			return;
		}
		bool flag = IsWalkingBackwards();
		mUsesClipping = false;
		if (mZombiePhase == ZombiePhase.DolphinWalking && !flag)
		{
			if (mX > 700 && mX <= 720)
			{
				mZombiePhase = ZombiePhase.DolphinIntoPool;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_jumpinpool, ReanimLoopType.PlayOnceAndHold, 20, 16f);
			}
		}
		else if (mZombiePhase == ZombiePhase.DolphinIntoPool)
		{
			mUsesClipping = true;
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.ShouldTriggerTimedEvent(0.56f))
			{
				Reanimation reanimation2 = mApp.AddReanimation(mX - 83, mY + 73, mRenderOrder + 1, ReanimationType.Splash);
				reanimation2.OverrideScale(1.2f, 0.8f);
				mApp.AddTodParticle(mX - 46, mY + 115, mRenderOrder + 1, ParticleEffect.PlantingPool);
				mApp.PlayFoley(FoleyType.ZombieEnteringWater);
			}
			if (reanimation.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.DolphinRiding;
				mInPool = true;
				mPosX -= 70f;
				mZombieAttackRect = new TRect(-29, 0, 70, 115);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_ride, ReanimLoopType.LoopFullLastFrame, 0, 12f);
			}
		}
		else if (mZombiePhase == ZombiePhase.DolphinRiding)
		{
			if (mX <= 10)
			{
				mZombieHeight = ZombieHeight.OutOfPool;
				mZombiePhase = ZombiePhase.DolphinWalking;
				mAltitude = -40f;
				PoolSplash(theInToPoolSound: false);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_walkdolphin, ReanimLoopType.Loop, 0, 0f);
				PickRandomSpeed();
				return;
			}
			if (mHasHead && !IsTanglekelpTarget())
			{
				Plant plant = FindPlantTarget(ZombieAttackType.Vault);
				if (plant != null)
				{
					mApp.PlayFoley(FoleyType.DolphinBeforeJumping);
					mApp.PlayFoley(FoleyType.PlantWater);
					mZombiePhase = ZombiePhase.DolphinInJump;
					mPhaseCounter = GameConstants.DOLPHIN_JUMP_TIME;
					mVelX = 0.5f;
					PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_dolphinjump, ReanimLoopType.PlayOnceAndHold, 0, 10f);
				}
			}
		}
		else if (mZombiePhase == ZombiePhase.DolphinInJump)
		{
			Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
			bool flag2 = false;
			mAltitude = TodCommon.TodAnimateCurveFloat(GameConstants.DOLPHIN_JUMP_TIME, 0, mPhaseCounter, 0f, 10f, TodCurves.Linear);
			if (reanimation3.ShouldTriggerTimedEvent(0.3f))
			{
				Plant plant2 = FindPlantTarget(ZombieAttackType.Vault);
				if (plant2 != null && plant2.mSeedType == SeedType.Tallnut)
				{
					mApp.PlayFoley(FoleyType.Bonk);
					flag2 = true;
					mApp.AddTodParticle(plant2.mX + 60, plant2.mY - 20, mRenderOrder + 1, ParticleEffect.TallNutBlock);
					mPosX = (float)plant2.mX + 25f;
					mAltitude = 30f;
					mZombieHeight = ZombieHeight.Falling;
				}
			}
			else if (reanimation3.ShouldTriggerTimedEvent(0.49f))
			{
				Reanimation reanimation4 = mApp.AddReanimation(mX - 63, mY + 73, mRenderOrder + 1, ReanimationType.Splash);
				reanimation4.OverrideScale(1.2f, 0.8f);
				mApp.AddTodParticle(mX - 26, mY + 115, mRenderOrder + 1, ParticleEffect.PlantingPool);
				mApp.PlayFoley(FoleyType.ZombieEnteringWater);
				mVelX = 0f;
			}
			else if (reanimation3.mLoopCount > 0)
			{
				flag2 = true;
				mPosX -= 94f;
				mAltitude = 0f;
			}
			if (flag2)
			{
				mZombiePhase = ZombiePhase.DolphinWalkingInPool;
				mZombieAttackRect = new TRect(30, 0, 30, 115);
				mZombieRect = new TRect(20, 0, 42, 115);
				StartWalkAnim(0);
			}
		}
		else if (mZombiePhase == ZombiePhase.DolphinWalkingInPool)
		{
			if (mX <= 10 && !flag)
			{
				mZombieHeight = ZombieHeight.OutOfPool;
				mZombiePhase = ZombiePhase.DolphinWalkingWithoutDolphin;
				mAltitude = -40f;
				PoolSplash(theInToPoolSound: false);
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 0, 0f);
				PickRandomSpeed();
			}
			else if ((mX > 680) & flag)
			{
				mZombieHeight = ZombieHeight.OutOfPool;
				mZombiePhase = ZombiePhase.DolphinWalkingWithoutDolphin;
				mAltitude = -40f;
				PoolSplash(theInToPoolSound: false);
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 0, 0f);
				PickRandomSpeed();
			}
		}
		if (mZombiePhase == ZombiePhase.DolphinWalkingWithoutDolphin)
		{
			mUsesClipping = mAltitude < 0f;
		}
	}

	public void PickBungeeZombieTarget(int theColumn)
	{
		bool flag = true;
		if (CountBungeesTargetingSunFlowers() == mBoard.CountSunFlowers() - 1)
		{
			flag = false;
		}
		int num = 0;
		for (int i = 0; i < aPicks.Length; i++)
		{
			aPicks[i].Reset();
		}
		for (int j = 0; j < Constants.GRIDSIZEX; j++)
		{
			if (theColumn != -1 && theColumn != j)
			{
				continue;
			}
			for (int k = 0; k < Constants.MAX_GRIDSIZEY; k++)
			{
				int mWeight = 1;
				if (mBoard.GetGraveStoneAt(j, k) != null || mBoard.mGridSquareType[j, k] == GridSquareType.Dirt)
				{
					continue;
				}
				Plant topPlantAt = mBoard.GetTopPlantAt(j, k, TopPlant.BungeeOrder);
				if (topPlantAt != null)
				{
					if ((!flag && topPlantAt.MakesSun()) || topPlantAt.mSeedType == SeedType.Gravebuster || topPlantAt.mSeedType == SeedType.Cobcannon)
					{
						continue;
					}
					mWeight = 10000;
				}
				if (!mBoard.BungeeIsTargetingCell(j, k))
				{
					aPicks[num].mX = j;
					aPicks[num].mY = k;
					aPicks[num].mWeight = mWeight;
					num++;
				}
			}
		}
		if (num == 0)
		{
			DieNoLoot(giveAchievements: false);
			return;
		}
		TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(aPicks, num);
		mTargetCol = todWeightedGridArray.mX;
		SetRow(todWeightedGridArray.mY);
		mPosX = mBoard.GridToPixelX(mTargetCol, mRow);
		mPosY = GetPosYBasedOnRow(mRow);
	}

	public int CountBungeesTargetingSunFlowers()
	{
		int num = 0;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying() && zombie.mZombieType == ZombieType.Bungee && zombie.mTargetCol != -1)
			{
				Plant topPlantAt = mBoard.GetTopPlantAt(zombie.mTargetCol, zombie.mRow, TopPlant.BungeeOrder);
				if (topPlantAt != null && topPlantAt.MakesSun())
				{
					num++;
				}
			}
		}
		return num;
	}

	public Plant FindPlantTarget(ZombieAttackType theAttackType)
	{
		return Tavel.Loaded ? TavelHooks.ZombieFindPlantTarget(this, theAttackType) : FindPlantTargetCore(theAttackType);
	}

	public Plant FindPlantTargetCore(ZombieAttackType theAttackType)
	{
		TRect zombieAttackRect = GetZombieAttackRect();
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && mRow == plant.mRow)
			{
				TRect plantRect = plant.GetPlantRect();
				int rectOverlap = GameConstants.GetRectOverlap(zombieAttackRect, plantRect);
				if (rectOverlap >= ((mZombieType == ZombieType.Digger) ? 5 : 20) && CanTargetPlant(plant, theAttackType))
				{
					return plant;
				}
			}
		}
		return null;
	}

	public void CheckSquish(ZombieAttackType theAttackType)
	{
		TRect zombieAttackRect = GetZombieAttackRect();
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && mRow == plant.mRow)
			{
				TRect plantRect = plant.GetPlantRect();
				int rectOverlap = GameConstants.GetRectOverlap(zombieAttackRect, plantRect);
				if (rectOverlap >= 20 && CanTargetPlant(plant, theAttackType) && !plant.IsSpiky())
				{
					SquishAllInSquare(plant.mPlantCol, plant.mRow, theAttackType);
					break;
				}
			}
		}
		if (mApp.IsIZombieLevel())
		{
			GridItem gridItem = mBoard.mChallenge.IZombieGetBrainTarget(this);
			if (gridItem != null)
			{
				mBoard.mChallenge.IZombieSquishBrain(gridItem);
			}
		}
	}

	public void RiseFromGrave(int theCol, int theRow)
	{
		Debug.ASSERT(mZombiePhase == ZombiePhase.ZombieNormal);
		mPosX = mBoard.GridToPixelX(theCol, mRow) - 25;
		mPosY = GetPosYBasedOnRow(theRow);
		SetRow(theRow);
		mX = (int)mPosX;
		mY = (int)mPosY;
		mZombiePhase = ZombiePhase.RisingFromGrave;
		mPhaseCounter = 150;
		mAltitude = -200f;
		mUsesClipping = true;
		if (mBoard.StageHasPool())
		{
			mInPool = true;
			mPhaseCounter = 50;
			mAltitude = -150f;
			mZombieHeight = ZombieHeight.ZombieNormal;
			StartWalkAnim(0);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_duckytube, theIgnoreClipRect: false);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_whitewater, theIgnoreClipRect: false);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_hand, theIgnoreClipRect: false);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_innerarm3, theIgnoreClipRect: false);
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			TodParticleSystem theParticleSystem = mApp.AddTodParticle(0f, 0f, 0, ParticleEffect.ZombieSeaweed);
			OverrideParticleScale(theParticleSystem);
			if (mZombieType == ZombieType.TrafficCone && theParticleSystem != null)
			{
				reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_cone, ref theParticleSystem, 37f * Constants.S, 20f * Constants.S);
			}
			else if (mZombieType == ZombieType.Pail && theParticleSystem != null)
			{
				reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_bucket, ref theParticleSystem, 37f * Constants.S, 20f * Constants.S);
			}
			else if (theParticleSystem != null)
			{
				reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_head1, ref theParticleSystem, 30f * Constants.S, 20f * Constants.S);
			}
			PoolSplash(theInToPoolSound: false);
		}
		else
		{
			int num = (int)mPosX + 60;
			int num2 = (int)mPosY + 110;
			if (IsOnHighGround())
			{
				num2 -= Constants.HIGH_GROUND_HEIGHT;
			}
			int aRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, theRow, 0);
			if (mApp.IsWhackAZombieLevel())
			{
				mApp.PlayFoley(FoleyType.DirtRise);
				mApp.AddTodParticle(num, num2, aRenderOrder, ParticleEffect.WhackAZombieRise);
			}
			else
			{
				mApp.PlayFoley(FoleyType.GravestoneRumble);
				mApp.AddTodParticle(num, num2, aRenderOrder, ParticleEffect.ZombieRise);
			}
		}
	}

	public void UpdateZombieRiseFromGrave()
	{
		if (mInPool)
		{
			mBodyReanimID.mClip = false;
			mAltitude = (float)TodCommon.TodAnimateCurve(50, 0, mPhaseCounter, -150, -40, TodCurves.Linear) * mScaleZombie;
			mUsesClipping = true;
		}
		else
		{
			mBodyReanimID.mClip = true;
			mAltitude = TodCommon.TodAnimateCurve(50, 0, mPhaseCounter, -200, 0, TodCurves.Linear);
			mUsesClipping = mAltitude < 0f;
		}
		if (mPhaseCounter <= 0)
		{
			mZombiePhase = ZombiePhase.ZombieNormal;
			if (IsOnHighGround())
			{
				mAltitude = Constants.HIGH_GROUND_HEIGHT;
			}
			if (mInPool)
			{
				ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_duckytube, theIgnoreClipRect: true);
				ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_whitewater, theIgnoreClipRect: true);
				ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_hand, theIgnoreClipRect: true);
				ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_innerarm3, theIgnoreClipRect: true);
			}
		}
	}

	public void UpdateDamageStates(uint theDamageFlags)
	{
		if (!CanLoseBodyParts())
		{
			return;
		}
		if (mHasArm && mBodyHealth < 2 * mBodyMaxHealth / 3 && mBodyHealth > 0)
		{
			DropArm(theDamageFlags);
		}
		if (mHasHead && mBodyHealth < mBodyMaxHealth / 3)
		{
			DropHead(theDamageFlags);
			DropLoot();
			StopZombieSound();
			if (mBoard.HasLevelAwardDropped())
			{
				PlayDeathAnim(theDamageFlags);
			}
			if (mZombiePhase == ZombiePhase.SnorkelWalkingInPool)
			{
				DieNoLoot(giveAchievements: false);
			}
		}
	}

	public void UpdateZombiePool()
	{
		if (mZombieHeight == ZombieHeight.OutOfPool)
		{
			mAltitude++;
			if (mZombieType == ZombieType.Snorkel)
			{
				mAltitude++;
			}
			if (mAltitude >= 0f)
			{
				mAltitude = 0f;
				mZombieHeight = ZombieHeight.ZombieNormal;
				mInPool = false;
			}
		}
		else if (mZombieHeight == ZombieHeight.InToPool)
		{
			mAltitude--;
			int num = -40;
			num *= (int)mScaleZombie;
			if (mAltitude <= (float)num)
			{
				mAltitude = num;
				mZombieHeight = ZombieHeight.ZombieNormal;
				StartWalkAnim(0);
			}
		}
		else if (mZombieHeight == ZombieHeight.DraggedUnder)
		{
			mAltitude--;
		}
	}

	public void CheckForPool()
	{
		if (!ZombieTypeCanGoInPool(mZombieType) || IsFlying() || mZombieType == ZombieType.DolphinRider || mZombieType == ZombieType.Snorkel || mZombieHeight == ZombieHeight.InToPool || mZombieHeight == ZombieHeight.OutOfPool)
		{
			return;
		}
		if (mBoard.IsPoolSquare(mBoard.PixelToGridX(mX + 75, mY), mRow) && mBoard.IsPoolSquare(mBoard.PixelToGridX(mX + 45, mY), mRow) && (float)mX < 680f)
		{
			if (!mInPool)
			{
				if (mBoard.mIceTrapCounter > 0)
				{
					mIceTrapCounter = mBoard.mIceTrapCounter;
					ApplyChill(theIsIceTrap: true);
					return;
				}
				mZombieHeight = ZombieHeight.InToPool;
				mInPool = true;
				PoolSplash(theInToPoolSound: true);
			}
			mUsesClipping = true;
		}
		else if (mInPool)
		{
			mZombieHeight = ZombieHeight.OutOfPool;
			StartWalkAnim(0);
			PoolSplash(theInToPoolSound: false);
		}
	}

	public void GetDrawPos(ref ZombieDrawPosition theDrawPos)
	{
		theDrawPos.mImageOffsetX = mPosX - (float)mX;
		theDrawPos.mImageOffsetY = mPosY - (float)mY;
		if (mIsEating)
		{
			theDrawPos.mHeadX = 47;
			theDrawPos.mHeadY = 4;
		}
		else
		{
			switch (mFrame)
			{
			case 0:
				theDrawPos.mHeadX = 50;
				theDrawPos.mHeadY = 2;
				break;
			case 1:
				theDrawPos.mHeadX = 49;
				theDrawPos.mHeadY = 1;
				break;
			case 2:
				theDrawPos.mHeadX = 49;
				theDrawPos.mHeadY = 2;
				break;
			case 3:
				theDrawPos.mHeadX = 48;
				theDrawPos.mHeadY = 4;
				break;
			case 4:
				theDrawPos.mHeadX = 48;
				theDrawPos.mHeadY = 5;
				break;
			case 5:
				theDrawPos.mHeadX = 48;
				theDrawPos.mHeadY = 4;
				break;
			case 6:
				theDrawPos.mHeadX = 48;
				theDrawPos.mHeadY = 2;
				break;
			case 7:
				theDrawPos.mHeadX = 49;
				theDrawPos.mHeadY = 1;
				break;
			case 8:
				theDrawPos.mHeadX = 49;
				theDrawPos.mHeadY = 2;
				break;
			case 9:
				theDrawPos.mHeadX = 50;
				theDrawPos.mHeadY = 4;
				break;
			case 10:
				theDrawPos.mHeadX = 50;
				theDrawPos.mHeadY = 5;
				break;
			default:
				theDrawPos.mHeadX = 50;
				theDrawPos.mHeadY = 4;
				break;
			}
		}
		theDrawPos.mArmY = theDrawPos.mHeadY / 2;
		TPoint reanimationDrawOffset = GetReanimationDrawOffset(mZombieType);
		theDrawPos.mImageOffsetX += reanimationDrawOffset.x;
		theDrawPos.mImageOffsetY += reanimationDrawOffset.y;
		if (mZombiePhase == ZombiePhase.RisingFromGrave)
		{
			theDrawPos.mBodyY = 0f - mAltitude;
			if (mInPool)
			{
				theDrawPos.mClipHeight = theDrawPos.mBodyY;
			}
			else
			{
				float num = Math.Min(mPhaseCounter, 40f);
				theDrawPos.mClipHeight = theDrawPos.mBodyY + num;
			}
			if (IsOnHighGround())
			{
				theDrawPos.mBodyY -= Constants.HIGH_GROUND_HEIGHT;
			}
			return;
		}
		switch (mZombieType)
		{
		case ZombieType.DolphinRider:
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mClipHeight = GameConstants.CLIP_HEIGHT_OFF;
			switch (mZombiePhase)
			{
			case ZombiePhase.DolphinIntoPool:
			{
				Reanimation reanimation2 = mBodyReanimID;
				if (reanimation2.mAnimTime >= 0.56f && reanimation2.mAnimTime <= 0.65f)
				{
					theDrawPos.mClipHeight = 0f;
				}
				else if (reanimation2.mAnimTime >= 0.75f)
				{
					theDrawPos.mClipHeight = 0f - mAltitude - 10f;
				}
				break;
			}
			case ZombiePhase.DolphinRiding:
				theDrawPos.mImageOffsetX += 70f;
				if (mZombieHeight == ZombieHeight.DraggedUnder)
				{
					theDrawPos.mClipHeight = 0f - mAltitude - 15f;
				}
				else
				{
					theDrawPos.mClipHeight = 0f - mAltitude - 10f;
				}
				break;
			case ZombiePhase.DolphinInJump:
			{
				theDrawPos.mImageOffsetX += 70f + mAltitude;
				Reanimation reanimation3 = mBodyReanimID;
				if (reanimation3.mAnimTime <= 0.06f)
				{
					theDrawPos.mClipHeight = 0f - mAltitude - 10f;
				}
				else if (reanimation3.mAnimTime >= 0.5f && reanimation3.mAnimTime <= 0.76f)
				{
					theDrawPos.mClipHeight = -13f;
				}
				break;
			}
			case ZombiePhase.ZombieDying:
			case ZombiePhase.DolphinWalkingInPool:
				theDrawPos.mImageOffsetY += 50f;
				if (mZombiePhase == ZombiePhase.ZombieDying)
				{
					theDrawPos.mClipHeight = 0f - mAltitude + 44f;
				}
				else if (mZombieHeight == ZombieHeight.DraggedUnder)
				{
					theDrawPos.mClipHeight = 0f - mAltitude + 36f;
				}
				break;
			case ZombiePhase.DolphinWalking:
				if (mZombieHeight == ZombieHeight.OutOfPool)
				{
					theDrawPos.mClipHeight = 0f - mAltitude;
				}
				break;
			case ZombiePhase.DolphinWalkingWithoutDolphin:
				if (mZombieHeight == ZombieHeight.OutOfPool)
				{
					theDrawPos.mClipHeight = 0f - mAltitude;
				}
				break;
			}
			return;
		case ZombieType.Snorkel:
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mClipHeight = GameConstants.CLIP_HEIGHT_OFF;
			if (mZombiePhase == ZombiePhase.SnorkelIntoPool)
			{
				Reanimation reanimation = mBodyReanimID;
				if (reanimation.mAnimTime >= 0.8f)
				{
					theDrawPos.mClipHeight = -10f;
				}
			}
			else if (mInPool)
			{
				theDrawPos.mClipHeight = 0f - mAltitude - 5f;
				theDrawPos.mClipHeight += 20f - 20f * mScaleZombie;
			}
			return;
		}
		if (mInPool)
		{
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mClipHeight = 0f - mAltitude - 7f;
			theDrawPos.mClipHeight += 10f - 10f * mScaleZombie;
			if (mIsEating)
			{
				theDrawPos.mClipHeight += 7f;
			}
			return;
		}
		switch (mZombiePhase)
		{
		case ZombiePhase.DancerRising:
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mClipHeight = 0f - mAltitude;
			if (IsOnHighGround())
			{
				theDrawPos.mBodyY -= Constants.HIGH_GROUND_HEIGHT;
			}
			return;
		case ZombiePhase.DiggerRising:
		case ZombiePhase.DiggerRiseWithoutAxe:
			theDrawPos.mBodyY = 0f - mAltitude;
			if (mPhaseCounter > 20)
			{
				theDrawPos.mClipHeight = 0f - mAltitude;
			}
			else
			{
				theDrawPos.mClipHeight = GameConstants.CLIP_HEIGHT_OFF;
			}
			return;
		case ZombiePhase.TalismanLeaving:
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mClipHeight = 0f - mAltitude - 45f;
			if (IsOnHighGround())
			{
				theDrawPos.mBodyY -= Constants.HIGH_GROUND_HEIGHT;
			}
			return;
		}
		if (mZombieType == ZombieType.Bungee)
		{
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mImageOffsetX += -18f;
			if (IsOnHighGround())
			{
				theDrawPos.mBodyY -= Constants.HIGH_GROUND_HEIGHT;
			}
			theDrawPos.mClipHeight = GameConstants.CLIP_HEIGHT_OFF;
		}
		else
		{
			theDrawPos.mBodyY = 0f - mAltitude;
			theDrawPos.mClipHeight = GameConstants.CLIP_HEIGHT_OFF;
		}
	}

	public void UpdateZombieHighGround()
	{
		if (mZombieType == ZombieType.Pogo)
		{
			return;
		}
		if (mZombieHeight == ZombieHeight.UpToHighGround)
		{
			mAltitude++;
			if (mAltitude >= (float)Constants.HIGH_GROUND_HEIGHT)
			{
				mAltitude = Constants.HIGH_GROUND_HEIGHT;
				mZombieHeight = ZombieHeight.ZombieNormal;
			}
		}
		else if (mZombieHeight == ZombieHeight.DownOffHighGround)
		{
			mAltitude--;
			if (mAltitude <= 0f)
			{
				mAltitude = 0f;
				mZombieHeight = ZombieHeight.ZombieNormal;
				mOnHighGround = false;
			}
		}
	}

	public void CheckForHighGround()
	{
		if (mZombieHeight == ZombieHeight.ZombieNormal && mZombieType != ZombieType.Bungee)
		{
			bool flag = IsOnHighGround();
			if (!mOnHighGround & flag)
			{
				mZombieHeight = ZombieHeight.UpToHighGround;
				mOnHighGround = true;
			}
			else if (mOnHighGround && !flag)
			{
				mZombieHeight = ZombieHeight.DownOffHighGround;
			}
		}
	}

	public bool IsOnHighGround()
	{
		if (!IsOnBoard())
		{
			return false;
		}
		int num = mBoard.PixelToGridXKeepOnBoard(mX + 75, mY);
		return mBoard.mGridSquareType[num, mRow] == GridSquareType.HighGround;
	}

	public bool IsAboutToLeaveHighGround()
	{
		if (!IsOnBoard())
		{
			return false;
		}
		int num = mBoard.PixelToGridXKeepOnBoard(mX + 75, mY);
		if (mBoard.mGridSquareType[num, mRow] == GridSquareType.HighGround && num - 1 >= 0)
		{
			return mBoard.mGridSquareType[num - 1, mRow] != GridSquareType.HighGround;
		}
		return false;
	}

	public void DropLoot()
	{
		if (!IsOnBoard())
		{
			return;
		}
		AlmanacDialog.AlmanacPlayerDefeatedZombie(mZombieType);
		if (mZombieType == ZombieType.Yeti)
		{
			mBoard.mKilledYeti = true;
		}
		TrySpawnLevelAward();
		if (mDroppedLoot || mBoard.HasLevelAwardDropped() || !mBoard.CanDropLoot())
		{
			return;
		}
		mDroppedLoot = true;
		ZombieDefinition zombieDefinition = GetZombieDefinition(mZombieType);
		int theDropFactor = zombieDefinition.mZombieValue;
		if (mApp.IsLevelUseJson())
		{
			CSOverrideZombieProperty component = mApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
			if (component != null && component.mProperties.TryGetValue(mZombieType, out var value) && value.mZombieValue.HasValue)
			{
				theDropFactor = value.mZombieValue.GetValueOrDefault();
			}
		}
		if ((!mApp.IsLittleTroubleLevel() || RandomNumbers.NextNumber(4) == 0) && mApp.mGameMode != GameMode.ChallengeZombiquarium && !mApp.IsIZombieLevel())
		{
			TRect zombieRect = GetZombieRect();
			int num = zombieRect.mX + zombieRect.mWidth / 2;
			int num2 = zombieRect.mY + zombieRect.mHeight / 4;
			if (mSunMoneyLoot != 0)
			{
				mBoard.AddSunMoney(num - 35, num2, mSunMoneyLoot, CoinMotion.Coin);
			}
			if (mZombieType == ZombieType.Yeti)
			{
				mApp.PlayFoley(FoleyType.SpawnSun);
				mBoard.AddCoin(num - 20, num2, CoinType.Diamond, CoinMotion.Coin);
				mBoard.AddCoin(num - 30, num2, CoinType.Diamond, CoinMotion.Coin);
				mBoard.AddCoin(num - 40, num2, CoinType.Diamond, CoinMotion.Coin);
				mBoard.AddCoin(num - 50, num2, CoinType.Diamond, CoinMotion.Coin);
			}
			else
			{
				mBoard.DropLootPiece(num, num2, theDropFactor);
			}
		}
	}

	public bool TrySpawnLevelAward()
	{
		if (!IsOnBoard())
		{
			return false;
		}
		if (mBoard.HasLevelAwardDropped())
		{
			return false;
		}
		if (mBoard.mLevelComplete)
		{
			return false;
		}
		if (mDroppedLoot)
		{
			return false;
		}
		if (mApp.IsFinalBossLevel())
		{
			if (mZombieType != ZombieType.Boss)
			{
				return false;
			}
		}
		else if (mApp.IsScaryPotterLevel())
		{
			if (!mBoard.mChallenge.ScaryPotterIsCompleted())
			{
				return false;
			}
		}
		else
		{
			if (mApp.IsContinuousChallenge())
			{
				return false;
			}
			if (mBoard.mCurrentWave < mBoard.mNumWaves)
			{
				return false;
			}
			if (mBoard.AreEnemyZombiesOnScreen())
			{
				return false;
			}
		}
		if (mApp.IsWhackAZombieLevel() && mBoard.mZombieCountDown > 0)
		{
			return false;
		}
		mBoard.mLevelAwardSpawned = true;
		mApp.mBoardResult = BoardResult.Won;
		TRect zombieRect = GetZombieRect();
		int num = zombieRect.mX + zombieRect.mWidth / 2;
		int theY = zombieRect.mY + zombieRect.mHeight / 2;
		if (!mBoard.IsSurvivalStageWithRepick())
		{
			mBoard.RemoveAllZombies();
		}
		CoinType coinType;
		if (mApp.IsScaryPotterLevel() && !mBoard.IsFinalScaryPotterStage())
		{
			coinType = CoinType.None;
			int theGridX = mBoard.PixelToGridXKeepOnBoard((int)mPosX + 75, (int)mPosY);
			mBoard.mChallenge.PuzzlePhaseComplete(theGridX, mRow);
		}
		else if (mApp.IsDIYMode())
		{
			coinType = CoinType.AwardMoneyBag;
		}
		else if (mApp.IsAdventureMode() && mBoard.mLevel <= 50)
		{
			if (mBoard.mLevel != 9 && mBoard.mLevel != 19 && mBoard.mLevel != 29 && mBoard.mLevel != 39 && mBoard.mLevel != 49)
			{
				if (mBoard.mLevel == 50)
				{
					coinType = ((!mApp.HasFinishedAdventure()) ? CoinType.AwardSliverSunflower : CoinType.Award10PottedPlants);
				}
				else if (mApp.HasFinishedAdventure())
				{
					coinType = CoinType.AwardMoneyBag;
				}
				else if (mBoard.mLevel == 4)
				{
					coinType = CoinType.Shovel;
				}
				else if (mBoard.mLevel == 14)
				{
					coinType = CoinType.Almanac;
				}
				else if (mBoard.mLevel == 24)
				{
					coinType = CoinType.Carkeys;
				}
				else if (mBoard.mLevel == 34)
				{
					coinType = CoinType.Taco;
				}
				else
				{
					coinType = ((mBoard.mLevel != 44) ? CoinType.FinalSeedPacket : CoinType.WateringCan);
				}
			}
			else
			{
				coinType = CoinType.Note;
			}
		}
		else if (mBoard.IsSurvivalStageWithRepick())
		{
			coinType = CoinType.None;
			mBoard.FadeOutLevel();
		}
		else if (mApp.IsQuickPlayMode())
		{
			coinType = CoinType.AwardMoneyBag;
		}
		else if (mBoard.IsLastStandStageWithRepick())
		{
			coinType = CoinType.None;
			mBoard.FadeOutLevel();
			mApp.PlayFoley(FoleyType.SpawnSun);
			for (int i = 0; i < 10; i++)
			{
				mBoard.AddCoin(num + i * 5, theY, CoinType.Sun, CoinMotion.Coin);
			}
		}
		else if (!mApp.IsAdventureMode())
		{
			if (!mApp.HasBeatenChallenge(mApp.mGameMode))
			{
				coinType = ((!mApp.AboutToEarnGoldSunflower()) ? CoinType.Trophy : CoinType.AwardGoldSunflower);
			}
			else
			{
				coinType = CoinType.AwardMoneyBag;
				if (mApp.IsSurvivalHell(mApp.mGameMode))
				{
					coinType = CoinType.AwardBagDiamond;
				}
			}
		}
		else
		{
			coinType = CoinType.AwardMoneyBag;
		}
		CoinMotion theCoinMotion = CoinMotion.Coin;
		if (mZombieType == ZombieType.Boss)
		{
			theCoinMotion = CoinMotion.FromBoss;
		}
		if (coinType != CoinType.None)
		{
			mApp.PlayFoley(FoleyType.SpawnSun);
			mBoard.AddCoin(num, theY, coinType, theCoinMotion);
		}
		mDroppedLoot = true;
		return true;
	}

	public void StartZombieSound()
	{
		if (!mPlayingSong)
		{
			if (mZombiePhase == ZombiePhase.JackInTheBoxRunning && mHasHead)
			{
				mApp.PlayFoley(FoleyType.Jackinthebox);
				mPlayingSong = true;
			}
			else if (mZombiePhase == ZombiePhase.DiggerTunneling)
			{
				mApp.PlayFoley(FoleyType.Digger);
				mPlayingSong = true;
			}
		}
	}

	public void StopZombieSound()
	{
		if (mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer)
		{
			if (mApp.mBoard != null)
			{
				bool flag = false;
				int count = mApp.mBoard.mZombies.Count;
				for (int i = 0; i < count; i++)
				{
					Zombie zombie = mApp.mBoard.mZombies[i];
					if (!zombie.mDead && zombie.mHasHead && !zombie.IsDeadOrDying() && zombie.IsOnBoard() && (zombie.mZombieType == ZombieType.Dancer || zombie.mZombieType == ZombieType.BackupDancer))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					mApp.mSoundSystem.StopFoley(FoleyType.Dancer);
				}
			}
			else
			{
				mApp.mSoundSystem.StopFoley(FoleyType.Dancer);
			}
		}
		if (mPlayingSong)
		{
			mPlayingSong = false;
			if (mZombieType == ZombieType.JackInTheBox)
			{
				mApp.mSoundSystem.StopFoley(FoleyType.Jackinthebox);
			}
			else if (mZombieType == ZombieType.Digger)
			{
				mApp.mSoundSystem.StopFoley(FoleyType.Digger);
			}
		}
	}

	public void UpdateZombieJackInTheBox()
	{
		if (mZombiePhase == ZombiePhase.JackInTheBoxRunning)
		{
			if (mPhaseCounter <= 0 && mHasHead)
			{
				mPhaseCounter = 110;
				mZombiePhase = ZombiePhase.JackInTheBoxPopping;
				StopZombieSound();
				mApp.PlaySample(Resources.SOUND_BOING);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_pop, ReanimLoopType.PlayOnceAndHold, 20, 28f);
			}
		}
		else
		{
			if (mZombiePhase != ZombiePhase.JackInTheBoxPopping)
			{
				return;
			}
			if (mPhaseCounter == 80)
			{
				mApp.PlayFoley(FoleyType.JackSurprise);
			}
			if (mPhaseCounter <= 0)
			{
				mApp.PlayFoley(FoleyType.Explosion);
				int num = mX + mWidth / 2;
				int num2 = mY + mHeight / 2;
				if (mMindControlled)
				{
					mBoard.KillAllZombiesInRadius(mRow, num, num2, Constants.JackInTheBoxZombieRadius, 1, theBurn: true, 127);
				}
				else
				{
					mBoard.KillAllZombiesInRadius(mRow, num, num2, Constants.JackInTheBoxZombieRadius, 1, theBurn: true, 255);
					mBoard.KillAllPlantsInRadius(num, num2, Constants.JackInTheBoxPlantRadius);
				}
				mApp.AddTodParticle(num, num2, 400000, ParticleEffect.Jackexplode);
				mBoard.ShakeBoard(4, -6);
				DieNoLoot(giveAchievements: false);
				if (mApp.IsScaryPotterLevel())
				{
					mBoard.mChallenge.ScaryPotterJackExplode(num, num2);
				}
			}
		}
	}

	public void DrawZombieHead(Graphics g, ref ZombieDrawPosition theDrawPos, int theFrame)
	{
	}

	public void UpdateZombiePosition()
	{
		if (mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Boss || mZombiePhase == ZombiePhase.RisingFromGrave || mZombieHeight == ZombieHeight.Zombiquarium)
		{
			return;
		}
		UpdateZombieWalking();
		CheckForZombieStep();
		if (mBlowingAway)
		{
			mPosX += 10f;
			if (mX > 850)
			{
				DieWithLoot();
				return;
			}
		}
		if (mAwayCounter > 0)
		{
			mAwayCounter--;
			mPosX++;
		}
		if (mZombieHeight != ZombieHeight.ZombieNormal)
		{
			return;
		}
		float posYBasedOnRow = GetPosYBasedOnRow(mRow);
		if (mPosY < posYBasedOnRow)
		{
			mPosY += Math.Min(1f, posYBasedOnRow - mPosY);
			if (mPosY > posYBasedOnRow)
			{
				mPosY = posYBasedOnRow;
			}
		}
		else if (mPosY > posYBasedOnRow)
		{
			mPosY -= Math.Min(1f, mPosY - posYBasedOnRow);
			if (mPosY < posYBasedOnRow)
			{
				mPosY = posYBasedOnRow;
			}
		}
	}

	public TRect GetZombieRect()
	{
		if (cachedZombieRectUpToDate)
		{
			return cachedZombieRect;
		}
		cachedZombieRect = mZombieRect;
		if (IsWalkingBackwards())
		{
			cachedZombieRect.mX = mWidth - cachedZombieRect.mX - cachedZombieRect.mWidth;
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		cachedZombieRect.Offset(mX, mY + (int)theDrawPos.mBodyY);
		if (theDrawPos.mClipHeight > GameConstants.CLIP_HEIGHT_LIMIT)
		{
			cachedZombieRect.mHeight -= (int)theDrawPos.mClipHeight;
		}
		cachedZombieRectUpToDate = true;
		return cachedZombieRect;
	}

	public TRect GetZombieAttackRect()
	{
		TRect result = mZombieAttackRect;
		if (mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.DolphinInJump)
		{
			result = new TRect(-40, 0, 100, 115);
		}
		if (IsWalkingBackwards())
		{
			result.mX = mWidth - result.mX - result.mWidth;
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		result.Offset(mX, mY + (int)theDrawPos.mBodyY);
		if (theDrawPos.mClipHeight > GameConstants.CLIP_HEIGHT_LIMIT)
		{
			result.mHeight -= (int)theDrawPos.mClipHeight;
		}
		return result;
	}

	public void UpdateZombieWalking()
	{
		if (ZombieNotWalking())
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			float num;
			if (IsBouncingPogo() || mZombiePhase == ZombiePhase.BalloonFlying || mZombiePhase == ZombiePhase.DolphinRiding || mZombiePhase == ZombiePhase.SnorkelWalkingInPool || mZombieType == ZombieType.Catapult)
			{
				num = mVelX;
				if (IsMovingAtChilledSpeed())
				{
					num *= GameConstants.CHILLED_SPEED_FACTOR;
				}
			}
			else if (mZombieType == ZombieType.Zamboni || mZombiePhase == ZombiePhase.DiggerTunneling || mZombiePhase == ZombiePhase.DolphinInJump || IsBobsledTeamWithSled() || mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.SnorkelIntoPool)
			{
				num = mVelX;
			}
			else if (mHasGroundTrack)
			{
				mGroundTrackIndex = reanimation.GetTrackIndex(Reanimation.ReanimTrackId__ground);
				num = reanimation.GetTrackVelocity(mGroundTrackIndex) * mScaleZombie;
			}
			else
			{
				num = mVelX;
				if (IsMovingAtChilledSpeed())
				{
					num *= GameConstants.CHILLED_SPEED_FACTOR;
				}
			}
			if (mApp.mGameScene == GameScenes.ZombiesWon)
			{
				ZombieType zombieType = mZombieType;
				if (num > 0.3f)
				{
					num = 0.3f;
				}
			}
			if (IsWalkingBackwards() || mZombiePhase == ZombiePhase.DancerDancingIn)
			{
				mPosX += num;
			}
			else
			{
				mPosX -= num;
			}
			if (mZombieType == ZombieType.Football && mFromWave != GameConstants.ZOMBIE_WAVE_WINNER && !mInPool)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.03f))
				{
					mApp.AddTodParticle(mX + 81, mY + 106, mRenderOrder - 1, ParticleEffect.DustFoot);
				}
				if (reanimation.ShouldTriggerTimedEvent(0.61f))
				{
					mApp.AddTodParticle(mX + 87, mY + 110, mRenderOrder - 1, ParticleEffect.DustFoot);
				}
			}
			if (mZombiePhase == ZombiePhase.PolevaulterPreVault)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.16f))
				{
					mApp.AddTodParticle(mX + 81, mY + 106, mRenderOrder - 1, ParticleEffect.DustFoot);
				}
				if (reanimation.ShouldTriggerTimedEvent(0.67f))
				{
					mApp.AddTodParticle(mX + 87, mY + 110, mRenderOrder - 1, ParticleEffect.DustFoot);
				}
			}
			return;
		}
		bool flag = false;
		if (mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.DiggerTunneling || mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer || mZombieType == ZombieType.Bobsled || mZombieType == ZombieType.Pogo || mZombieType == ZombieType.DolphinRider || mZombieType == ZombieType.Balloon || mZombieType == ZombieType.Propeller)
		{
			flag = true;
		}
		else if (mZombieType == ZombieType.Snorkel && mInPool)
		{
			flag = true;
		}
		else if (mFrame >= 0 && mFrame <= 2)
		{
			flag = true;
		}
		else if (mFrame >= 6 && mFrame <= 8)
		{
			flag = true;
		}
		if (flag)
		{
			float num2 = mVelX;
			if (IsMovingAtChilledSpeed())
			{
				num2 *= GameConstants.CHILLED_SPEED_FACTOR;
			}
			if (IsWalkingBackwards())
			{
				mPosX += num2;
			}
			else
			{
				mPosX -= num2;
			}
		}
	}

	public void UpdateZombieWalkingIntoHouse()
	{
		if (mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Boss || mZombiePhase == ZombiePhase.RisingFromGrave || mZombieHeight == ZombieHeight.Zombiquarium)
		{
			return;
		}
		int num = 1;
		if (mZombieType == ZombieType.Normal || mZombieType == ZombieType.Pail || mZombieType == ZombieType.TrafficCone)
		{
			num = 2;
		}
		else if (mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			num = 4;
		}
		while (num-- != 0)
		{
			UpdateZombieWalking();
			if (mZombieHeight != ZombieHeight.ZombieNormal)
			{
				continue;
			}
			float num2 = GameConstants.ZOMBIE_WALK_IN_FRONT_DOOR_Y;
			float val = 1f;
			if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
			{
				num2 += 30f;
			}
			else if (mZombiePhase == ZombiePhase.PolevaulterPreVault)
			{
				num2 += 35f;
			}
			else if (mZombieType == ZombieType.Zamboni)
			{
				num2 += 15f;
			}
			else if (mZombieType == ZombieType.Football)
			{
				num2 += 15f;
				if (mRow == 0 || mRow == 1)
				{
					val = 2f;
				}
			}
			if (!WinningZombieReachedDesiredY && (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan))
			{
				WinningZombieReachedDesiredY = true;
				ReanimReenableClipping();
			}
			if (mPosY < num2)
			{
				mPosY += Math.Min(val, num2 - mPosY);
			}
			else if (mPosY > num2)
			{
				mPosY -= Math.Min(val, mPosY - num2);
			}
			else if (!WinningZombieReachedDesiredY)
			{
				WinningZombieReachedDesiredY = true;
				ReanimReenableClipping();
			}
		}
	}

	public void UpdateZombieBobsled()
	{
		if (mZombiePhase == ZombiePhase.BobsledCrashing)
		{
			if (mPhaseCounter > 0)
			{
				return;
			}
			mZombiePhase = ZombiePhase.ZombieNormal;
			if (GetBobsledPosition() == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Zombie zombie = mBoard.ZombieGet(mFollowerZombieID[i]);
					zombie.mRelatedZombieID = null;
					mFollowerZombieID[i] = null;
					zombie.PickRandomSpeed();
				}
				PickRandomSpeed();
			}
			return;
		}
		if (mZombiePhase == ZombiePhase.BobsledSliding)
		{
			if (mPhaseCounter == 0)
			{
				mZombiePhase = ZombiePhase.BobsledBoarding;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_jump, ReanimLoopType.PlayOnceAndHold, 0, 20f);
			}
		}
		else
		{
			if (mZombiePhase != ZombiePhase.BobsledBoarding)
			{
				return;
			}
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			int theTimeAge = (int)(reanimation.mAnimTime * 50f);
			int bobsledPosition = GetBobsledPosition();
			if (bobsledPosition == 1 || bobsledPosition == 3)
			{
				mAltitude = TodCommon.TodAnimateCurveFloat(0, 50, theTimeAge, 8f, 18f, TodCurves.Linear);
			}
			else
			{
				mAltitude = TodCommon.TodAnimateCurveFloat(0, 50, theTimeAge, -9f, 18f, TodCurves.Linear);
			}
		}
		mBoard.mIceTimer[mRow] = Math.Max(500, mBoard.mIceTimer[mRow]);
		if (mPosX + 10f < (float)mBoard.mIceMinX[mRow] && GetBobsledPosition() == 0)
		{
			TakeDamage(6, 8u);
		}
	}

	public void BobsledCrash()
	{
		mZombiePhase = ZombiePhase.BobsledCrashing;
		mPhaseCounter = GameConstants.BOBSLED_CRASH_TIME;
		mAltitude = 0f;
		mZombieRect = new TRect(36, 0, 42, 115);
		StartWalkAnim(0);
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		for (int i = 0; i < 3; i++)
		{
			Zombie zombie = mBoard.ZombieGet(mFollowerZombieID[i]);
			zombie.mZombiePhase = ZombiePhase.BobsledCrashing;
			zombie.mPhaseCounter = GameConstants.BOBSLED_CRASH_TIME;
			zombie.mPosY = GetPosYBasedOnRow(mRow);
			zombie.mAltitude = 0f;
			zombie.StartWalkAnim(0);
			Reanimation reanimation2 = mApp.ReanimationGet(zombie.mBodyReanimID);
			if (reanimation2 != null)
			{
				zombie.mVelX = mVelX;
				reanimation2.mAnimTime = TodCommon.RandRangeFloat(0f, 1f);
				reanimation2.mAnimRate = reanimation.mAnimRate;
			}
		}
	}

	public Plant IsStandingOnSpikeweed()
	{
		if (mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Catapult)
		{
			return null;
		}
		TRect zombieRect = GetZombieRect();
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && mRow == plant.mRow && plant.IsSpiky() && !plant.NotOnGround() && (!mOnHighGround || plant.IsOnHighGround()))
			{
				TRect plantAttackRect = plant.GetPlantAttackRect(PlantWeapon.Primary);
				int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
				if (rectOverlap > 0)
				{
					return plant;
				}
			}
		}
		return null;
	}

	public void CheckForZombieStep()
	{
		if (mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Catapult)
		{
			if (!mFlatTires)
			{
				CheckSquish(ZombieAttackType.DriveOver);
			}
		}
		else if (mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			CheckSquish(ZombieAttackType.Chew);
		}
	}

	public void OverrideParticleColor(TodParticleSystem aParticle)
	{
		if (aParticle != null)
		{
			if (mMindControlled)
			{
				aParticle.OverrideColor(null, GameConstants.ZOMBIE_MINDCONTROLLED_COLOR);
				aParticle.OverrideExtraAdditiveDraw(null, theEnableExtraAdditiveDraw: true);
			}
			else if (mChilledCounter > 0 || mIceTrapCounter > 0)
			{
				aParticle.OverrideColor(null, new SexyColor(75, 75, 255, 255));
				aParticle.OverrideExtraAdditiveDraw(null, theEnableExtraAdditiveDraw: true);
			}
		}
	}

	public void OverrideParticleScale(TodParticleSystem aParticle)
	{
		aParticle?.OverrideScale(null, mScaleZombie);
	}

	public void PoolSplash(bool theInToPoolSound)
	{
		float num = 23f;
		float num2 = 78f;
		if (mZombiePhase == ZombiePhase.SnorkelWalkingInPool)
		{
			num += -37f;
			num2 += -8f;
		}
		int num3 = ((mAltitude != 0f) ? ((int)num2) : ((int)(num2 * mScaleZombie)));
		mApp.AddReanimation(mX + (int)(num * mScaleZombie), mY + num3, mRenderOrder + 1, ReanimationType.Splash).OverrideScale(0.8f, 0.8f);
		mApp.AddTodParticle((float)mX + num + 37f, (float)mY + num2 + 42f, mRenderOrder + 1, ParticleEffect.PlantingPool);
		if (theInToPoolSound)
		{
			mApp.PlayFoley(FoleyType.Zombiesplash);
		}
		else
		{
			mApp.PlayFoley(FoleyType.PlantWater);
		}
	}

	public void UpdateZombieFlyer()
	{
		if (mApp.mGameMode == GameMode.ChallengeHighGravity && mPosX < 720f + (float)Constants.BOARD_EXTRA_ROOM)
		{
			mAltitude -= 0.1f;
			if (mAltitude < -35f)
			{
				LandFlyer(0u);
			}
		}
		if (mZombiePhase == ZombiePhase.PropellerBlownAway)
		{
			mPosX += 2f;
			if (mPhaseCounter <= 0)
			{
				mZombiePhase = ZombiePhase.PropellerTeeter;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_teeter, ReanimLoopType.PlayOnceAndHold, 0, 0f);
			}
		}
		if (mZombiePhase == ZombiePhase.PropellerTeeter)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 0f);
				mZombiePhase = ZombiePhase.BalloonFlying;
			}
		}
		if (mZombiePhase == ZombiePhase.BalloonPopping)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation2.mLoopCount > 0 || (mZombieType == ZombieType.Propeller && mZombieHeight == ZombieHeight.ZombieNormal))
			{
				mZombiePhase = ZombiePhase.BalloonWalking;
				StartWalkAnim(0);
			}
		}
		if (mApp.IsIZombieLevel() && mZombiePhase == ZombiePhase.BalloonFlying && mBoard.mChallenge.IZombieGetBrainTarget(this) != null)
		{
			LandFlyer(0u);
		}
	}

	public void UpdateZombiePogo()
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieRunWithSwgAsTallnut(this, UpdateZombiePogoCore);
			return;
		}
		UpdateZombiePogoCore();
	}

	public void UpdateZombiePogoCore()
	{
		if (IsDeadOrDying() || IsImmobilizied() || !IsBouncingPogo() || mZombieHeight == ZombieHeight.InToChimney)
		{
			return;
		}
		float num = 40f;
		if (mZombiePhase >= ZombiePhase.PogoHighBounce1 && mZombiePhase <= ZombiePhase.PogoHighBounce6)
		{
			num = 50f + 20f * (float)(mZombiePhase - 21);
		}
		else if (mZombiePhase == ZombiePhase.PogoForwardBounce2)
		{
			num = 90f;
		}
		else if (mZombiePhase == ZombiePhase.PogoForwardBounce7)
		{
			num = 170f;
		}
		float num2 = 9f;
		mAltitude = TodCommon.TodAnimateCurveFloat(GameConstants.POGO_BOUNCE_TIME, 0, mPhaseCounter, num2, num + num2, TodCurves.BounceSlowMiddle);
		mFrame = TodCommon.ClampInt(3 - (int)mAltitude / 3, 0, 3);
		if (mPhaseCounter == 7)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			reanimation.mAnimTime = 0f;
			reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
		}
		if (IsOnBoard() && mPhaseCounter == 5)
		{
			mApp.PlayFoley(FoleyType.PogoZombie);
		}
		if (mZombieHeight == ZombieHeight.UpToHighGround)
		{
			mAltitude += Constants.HIGH_GROUND_HEIGHT;
			mZombieHeight = ZombieHeight.ZombieNormal;
		}
		else if (mZombieHeight == ZombieHeight.DownOffHighGround)
		{
			mOnHighGround = false;
			mZombieHeight = ZombieHeight.ZombieNormal;
		}
		else if (mOnHighGround)
		{
			mAltitude += Constants.HIGH_GROUND_HEIGHT;
		}
		if (mZombiePhase == ZombiePhase.PogoForwardBounce2 && mPhaseCounter == 70)
		{
			Plant plant = FindPlantTarget(ZombieAttackType.Vault);
			if (plant != null && plant.mSeedType == SeedType.Tallnut)
			{
				mApp.PlayFoley(FoleyType.Bonk);
				mApp.AddTodParticle(plant.mX + 60, plant.mY - 20, mRenderOrder + 1, ParticleEffect.TallNutBlock);
				mShieldType = ShieldType.None;
				PogoBreak(0u);
				return;
			}
		}
		if (mPhaseCounter <= 0)
		{
			Plant plant = null;
			if (IsOnBoard())
			{
				plant = FindPlantTarget(ZombieAttackType.Vault);
			}
			if (plant == null)
			{
				mZombiePhase = ZombiePhase.PogoBouncing;
				PickRandomSpeed();
				mPhaseCounter = GameConstants.POGO_BOUNCE_TIME;
			}
			else if (mZombiePhase == ZombiePhase.PogoHighBounce1)
			{
				mZombiePhase = ZombiePhase.PogoForwardBounce2;
				int num3 = mX - plant.mX + 60;
				mVelX = (float)num3 / (float)GameConstants.POGO_BOUNCE_TIME;
				mPhaseCounter = GameConstants.POGO_BOUNCE_TIME;
			}
			else
			{
				mZombiePhase = ZombiePhase.PogoHighBounce1;
				mVelX = 0f;
				mPhaseCounter = GameConstants.POGO_BOUNCE_TIME;
			}
		}
	}

	public void UpdateZombieNewspaper()
	{
		if (mZombiePhase != ZombiePhase.NewspaperMaddening)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (reanimation.mLoopCount > 0)
		{
			mZombiePhase = ZombiePhase.NewspaperMad;
			if (mBoard.CountZombiesOnScreen() <= 10 && mHasHead)
			{
				mApp.PlayFoley(FoleyType.NewspaperRarrgh);
			}
			StartWalkAnim(20);
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, AtlasResources.IMAGE_REANIM_ZOMBIE_PAPER_MADHEAD);
		}
	}

	public void LandFlyer(uint theDamageFlags)
	{
		if (!TodCommon.TestBit(theDamageFlags, 4) && (mZombiePhase == ZombiePhase.BalloonFlying || mZombiePhase == ZombiePhase.PropellerBlownAway || mZombiePhase == ZombiePhase.PropellerTeeter))
		{
			mApp.PlaySample(Resources.SOUND_BALLOON_POP);
			mZombiePhase = ZombiePhase.BalloonPopping;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_pop, ReanimLoopType.PlayOnceAndHold, 20, 24f);
		}
		if (mBoard.mPlantRow[mRow] == PlantRowType.Pool && mZombieType == ZombieType.Balloon)
		{
			DieWithLoot();
		}
		else
		{
			mZombieHeight = ZombieHeight.Falling;
		}
	}

	public void UpdateZombieDigger()
	{
		if (mZombiePhase == ZombiePhase.DiggerTunneling)
		{
			if (mPosX < 10f + (float)Constants.BOARD_EXTRA_ROOM)
			{
				mZombiePhase = ZombiePhase.DiggerRising;
				mPhaseCounter = 130;
				mAltitude = -120f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_drill, ReanimLoopType.Loop, 0, 20f);
				mApp.PlayFoley(FoleyType.DirtRise);
				mApp.PlayFoley(FoleyType.Wakeup);
				GlobalMembersAttachment.AttachmentDetachCrossFadeParticleType(ref mAttachmentID, ParticleEffect.DiggerTunnel, null);
				StopZombieSound();
				mApp.AddTodParticle(mPosX + 60f, mPosY + 118f, mRenderOrder + 1, ParticleEffect.DiggerRise);
				Reanimation reanimation = mApp.AddReanimation(mPosX + 13f, mPosY + 97f, mRenderOrder + 1, ReanimationType.DiggerDirt);
				reanimation.mAnimRate = 24f;
			}
			if (IsOnHighGround() && IsAboutToLeaveHighGround())
			{
				DiggerLoseAxe();
			}
		}
		else if (mZombiePhase == ZombiePhase.DiggerRising)
		{
			if (mPhaseCounter > 40)
			{
				mAltitude = TodCommon.TodAnimateCurve(130, 40, mPhaseCounter, -120, 20, TodCurves.EaseOut);
			}
			else
			{
				mAltitude = TodCommon.TodAnimateCurve(30, 0, mPhaseCounter, 20, 0, TodCurves.EaseIn);
			}
			if (mPhaseCounter == 30)
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_landing, ReanimLoopType.PlayOnceAndHold, 0, 12f);
			}
			if (mPhaseCounter == 0)
			{
				mZombiePhase = ZombiePhase.DiggerStunned;
				mAltitude = 0f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_dizzy, ReanimLoopType.Loop, 10, 12f);
			}
			mRenderOrder = Board.MakeRenderOrder(RenderLayer.LawnMower, mRow, 1);
		}
		else if (mZombiePhase == ZombiePhase.DiggerTunnelingPauseWithoutAxe)
		{
			if (mPhaseCounter == 150)
			{
				AddAttachedReanim(23, 93, ReanimationType.ZombieSurprise);
			}
			if (mPhaseCounter == 0)
			{
				mZombiePhase = ZombiePhase.DiggerRiseWithoutAxe;
				mPhaseCounter = 130;
				mAltitude = -120f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_landing, ReanimLoopType.PlayOnceAndHold, 0, 0f);
				mApp.PlayFoley(FoleyType.DirtRise);
				mApp.AddTodParticle(mPosX + 60f, mPosY + 118f, mRenderOrder + 1, ParticleEffect.DiggerRise);
				Reanimation reanimation2 = mApp.AddReanimation(mPosX + 13f, mPosY + 97f, mRenderOrder + 1, ReanimationType.DiggerDirt);
				reanimation2.mAnimRate = 24f;
			}
		}
		else if (mZombiePhase == ZombiePhase.DiggerRiseWithoutAxe)
		{
			if (mPhaseCounter > 40)
			{
				mAltitude = TodCommon.TodAnimateCurve(130, 40, mPhaseCounter, -120, 20, TodCurves.EaseOut);
			}
			else
			{
				mAltitude = TodCommon.TodAnimateCurve(30, 0, mPhaseCounter, 20, 0, TodCurves.EaseIn);
			}
			if (mPhaseCounter == 30)
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_landing, ReanimLoopType.PlayOnceAndHold, 20, 12f);
			}
			if (mPhaseCounter == 0)
			{
				mZombiePhase = ZombiePhase.DiggerWalkingWithoutAxe;
				mAltitude = 0f;
				StartWalkAnim(20);
			}
		}
		else if (mZombiePhase == ZombiePhase.DiggerStunned)
		{
			Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation3.mLoopCount > 1)
			{
				mZombiePhase = ZombiePhase.DiggerWalking;
				StartWalkAnim(20);
			}
		}
		mUsesClipping = mAltitude < 0f;
	}

	public bool IsWalkingBackwards()
	{
		if (mMindControlled)
		{
			return true;
		}
		if (mZombieHeight == ZombieHeight.Zombiquarium)
		{
			float num = mVelZ;
			if (num < (float)Math.PI / 2f || num > 4.712389f)
			{
				return true;
			}
		}
		if (mZombieType == ZombieType.Digger)
		{
			if (mZombiePhase != ZombiePhase.DiggerRising && mZombiePhase != ZombiePhase.DiggerStunned && mZombiePhase != ZombiePhase.DiggerWalking)
			{
				if (mZombiePhase == ZombiePhase.ZombieDying || mZombiePhase == ZombiePhase.ZombieBurned || mZombiePhase == ZombiePhase.ZombieMowered)
				{
					return mHasObject;
				}
				return false;
			}
			return true;
		}
		if (mZombieType == ZombieType.Yeti)
		{
			return !mHasObject;
		}
		return false;
	}

	public TodParticleSystem AddAttachedParticle(int thePosX, int thePosY, ParticleEffect theEffect)
	{
		if (mDead)
		{
			return null;
		}
		if (!doParticle)
		{
			return null;
		}
		if (GlobalMembersAttachment.IsFullOfAttachments(ref mAttachmentID))
		{
			return null;
		}
		TodParticleSystem todParticleSystem = mApp.AddTodParticle(mX + thePosX, mY + thePosY, 0, theEffect);
		if (todParticleSystem != null && !todParticleSystem.mDead)
		{
			GlobalMembersAttachment.AttachParticle(ref mAttachmentID, todParticleSystem, thePosX, thePosY);
		}
		return todParticleSystem;
	}

	public void PogoBreak(uint theDamageFlags)
	{
		if (mHasObject)
		{
			if (!TodCommon.TestBit(theDamageFlags, 4))
			{
				ZombieDrawPosition theDrawPos = default;
				GetDrawPos(ref theDrawPos);
				int aRenderOrder = mRenderOrder + 1;
				float thePosX = 0f;
				float thePosY = 0f;
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_pogo_stick, ref thePosX, ref thePosY);
				TodParticleSystem aParticle = mApp.AddTodParticle(thePosX, thePosY + 30f, aRenderOrder, ParticleEffect.ZombiePogo);
				OverrideParticleScale(aParticle);
			}
			Debug.ASSERT(mZombiePhase != ZombiePhase.ZombieDying && mZombiePhase != ZombiePhase.ZombieBurned && !mDead);
			mZombieHeight = ZombieHeight.Falling;
			mZombiePhase = ZombiePhase.ZombieNormal;
			StartWalkAnim(0);
			mZombieRect = new TRect(36, 17, 42, 115);
			mZombieAttackRect = new TRect(20, 17, 50, 115);
			mShieldHealth = 0;
			mShieldType = ShieldType.None;
			mHasObject = false;
		}
	}

	public void UpdateZombieFalling()
	{
		mAltitude--;
		if (mZombiePhase == ZombiePhase.PolevaulterPreVault)
		{
			mAltitude--;
		}
		int num = 0;
		if (IsOnHighGround())
		{
			num = Constants.HIGH_GROUND_HEIGHT;
		}
		if (mAltitude <= (float)num)
		{
			mAltitude = num;
			mZombieHeight = ZombieHeight.ZombieNormal;
		}
	}

	public void UpdateZombieDancer()
	{
		if (mIsEating)
		{
			return;
		}
		if (mSummonCounter > 0)
		{
			mSummonCounter--;
			if (mSummonCounter <= 0)
			{
				int dancerFrame = GetDancerFrame();
				if (dancerFrame == 12 && mHasHead && mPosX < (float)Constants.Zombie_Dancer_Dance_Limit_X)
				{
					mZombiePhase = ZombiePhase.DancerSnappingFingersWithLight;
					PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_point, ReanimLoopType.PlayOnceAndHold, 20, 24f);
				}
				else
				{
					mSummonCounter = 1;
				}
			}
		}
		switch (mZombiePhase)
		{
		case ZombiePhase.DancerDancingIn:
			if (mHasHead && mPhaseCounter <= 0)
			{
				mZombiePhase = ZombiePhase.DancerSnappingFingers;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_point, ReanimLoopType.PlayOnceAndHold, 20, 24f);
				PickRandomSpeed();
			}
			return;
		case ZombiePhase.DancerSnappingFingers:
		case ZombiePhase.DancerSnappingFingersWithLight:
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				if (mZombiePhase == ZombiePhase.DancerSnappingFingers && mBoard.CountZombiesOnScreen() <= 15)
				{
					mApp.PlayFoley(FoleyType.Dancer);
				}
				SummonBackupDancers();
				mZombiePhase = ZombiePhase.DancerSnappingFingersHold;
				mPhaseCounter = 200;
			}
			return;
		}
		case ZombiePhase.DancerSnappingFingersHold:
			if (mPhaseCounter > 0)
			{
				return;
			}
			mZombiePhase = ZombiePhase.DancerDancingLeft;
			PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 20, 0f);
			break;
		}
		ZombiePhase dancerPhase = GetDancerPhase();
		if (dancerPhase != mZombiePhase)
		{
			switch (dancerPhase)
			{
			case ZombiePhase.DancerDancingLeft:
				mZombiePhase = dancerPhase;
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 10, 0f);
				break;
			case ZombiePhase.DancerWalkToRaise:
			{
				mZombiePhase = dancerPhase;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_armraise, ReanimLoopType.Loop, 10, 18f);
				Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
				reanimation2.mAnimTime = 0.6f;
				break;
			}
			case ZombiePhase.DancerRaiseLeft1:
			case ZombiePhase.DancerRaiseRight1:
			case ZombiePhase.DancerRaiseLeft2:
			case ZombiePhase.DancerRaiseRight2:
				mZombiePhase = dancerPhase;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_armraise, ReanimLoopType.Loop, 10, 18f);
				break;
			}
		}
		if (mHasHead && mSummonCounter == 0 && NeedsMoreBackupDancers())
		{
			mSummonCounter = 100;
		}
	}

	public Zombie SummonBackupDancer(int theRow, int thePosX)
	{
		if (!mBoard.RowCanHaveZombieType(theRow, ZombieType.BackupDancer))
		{
			return null;
		}
		Zombie zombie = mBoard.AddZombie(ZombieType.BackupDancer, mFromWave);
		if (zombie == null)
		{
			return null;
		}
		zombie.mPosX = thePosX;
		zombie.mPosY = GetPosYBasedOnRow(theRow);
		zombie.SetRow(theRow);
		zombie.mX = (int)zombie.mPosX;
		zombie.mY = (int)zombie.mPosY;
		zombie.mZombiePhase = ZombiePhase.DancerRising;
		zombie.mPhaseCounter = 150;
		zombie.mAltitude = Constants.ZOMBIE_BACKUP_DANCER_RISE_HEIGHT;
		zombie.mUsesClipping = true;
		zombie.mRelatedZombieID = mBoard.ZombieGetID(this);
		zombie.SetAnimRate(0f);
		zombie.mMindControlled = mMindControlled;
		int num = (int)zombie.mPosX + 60;
		int num2 = (int)zombie.mPosY + 110;
		if (zombie.IsOnHighGround())
		{
			num2 -= Constants.HIGH_GROUND_HEIGHT;
		}
		int aRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, theRow, 0);
		mApp.AddTodParticle(num, num2, aRenderOrder, ParticleEffect.DancerRise);
		mApp.PlayFoley(FoleyType.GravestoneRumble);
		return mBoard.ZombieGetID(zombie);
	}

	public void SummonBackupDancers()
	{
		if (!mHasHead)
		{
			return;
		}
		for (int i = 0; i < GameConstants.NUM_BACKUP_DANCERS; i++)
		{
			if (mBoard.ZombieTryToGet(mFollowerZombieID[i]) != null)
			{
				continue;
			}
			int theRow = 0;
			int thePosX = 0;
			switch (i)
			{
			case 0:
				theRow = mRow - 1;
				thePosX = (int)mPosX;
				break;
			case 1:
				theRow = mRow + 1;
				thePosX = (int)mPosX;
				break;
			case 2:
				if (mPosX < 130f)
				{
					continue;
				}
				theRow = mRow;
				thePosX = (int)mPosX - 100;
				break;
			case 3:
				theRow = mRow;
				thePosX = (int)mPosX + 100;
				break;
			default:
				Debug.ASSERT(value: false);
				break;
			}
			mFollowerZombieID[i] = SummonBackupDancer(theRow, thePosX);
			if (mFollowerZombieID[i] != null)
			{
				mFollowerZombieID[i].mLeaderZombie = this;
			}
			mSummonedDancers = true;
		}
	}

	public int GetDancerFrame()
	{
		if (mFromWave == GameConstants.ZOMBIE_WAVE_UI)
		{
			return 0;
		}
		if (IsImmobilizied())
		{
			return 0;
		}
		int num = 20;
		int num2 = 23;
		if (mZombiePhase == ZombiePhase.DancerDancingIn)
		{
			num2 = 11;
			num = 10;
		}
		return mBoard.mMainCounter % (num * num2) / num;
	}

	public void BungeeStealTarget()
	{
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_grab, ReanimLoopType.PlayOnceAndHold, 20, 24f);
		Plant topPlantAt = mBoard.GetTopPlantAt(mTargetCol, mRow, TopPlant.BungeeOrder);
		if (topPlantAt != null && !topPlantAt.NotOnGround() && topPlantAt.mSeedType != SeedType.Cobcannon)
		{
			Debug.ASSERT(topPlantAt.mSeedType != SeedType.Gravebuster);
			mTargetPlantID = topPlantAt;
			topPlantAt.mOnBungeeState = PlantOnBungeeState.GettingGrabbedByBungee;
			mRenderOrder = Board.MakeRenderOrder(RenderLayer.Projectile, mRow, 0);
		}
	}

	public void UpdateYuckyFace()
	{
		mYuckyFaceCounter++;
		if (mYuckyFaceCounter > GameConstants.YUCKI_SHORT_PAUSE_TIME && mYuckyFaceCounter < GameConstants.YUCKI_HOLD_TIME && !HasYuckyFaceImage())
		{
			StopEating();
			mYuckyFaceCounter = GameConstants.YUCKI_HOLD_TIME;
			if (mBoard.CountZombiesOnScreen() <= 5 && mHasHead)
			{
				mApp.PlayFoley(FoleyType.Yuck);
			}
			else if (mBoard.CountZombiesOnScreen() <= 10 && mHasHead && RandomNumbers.NextNumber(2) == 0)
			{
				mApp.PlayFoley(FoleyType.Yuck);
			}
		}
		if (mYuckyFaceCounter > GameConstants.YUCKI_WALK_TIME)
		{
			ShowYuckyFace(theShow: false);
			mYuckyFace = false;
			mYuckyFaceCounter = 0;
			if (mYuckySwitchRowsLate)
			{
				mYuckySwitchRowsLate = false;
				SetRow(mYuckyToRow);
			}
			return;
		}
		if (mYuckyFaceCounter == GameConstants.YUCKI_PAUSE_TIME)
		{
			StopEating();
			ShowYuckyFace(theShow: true);
			if (mBoard.CountZombiesOnScreen() <= 5 && mHasHead)
			{
				mApp.PlayFoley(FoleyType.Yuck);
			}
			else if (mBoard.CountZombiesOnScreen() <= 10 && mHasHead && RandomNumbers.NextNumber(2) == 0)
			{
				mApp.PlayFoley(FoleyType.Yuck);
			}
		}
		if (mYuckyFaceCounter != GameConstants.YUCKI_HOLD_TIME)
		{
			return;
		}
		StartWalkAnim(20);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = mBoard.mPlantRow[mRow] == PlantRowType.Pool;
		if (!mBoard.RowCanHaveZombies(mRow - 1))
		{
			flag = false;
		}
		else if (mBoard.mPlantRow[mRow - 1] == PlantRowType.Pool && !flag3)
		{
			flag = false;
		}
		else if ((mBoard.mPlantRow[mRow - 1] != PlantRowType.Pool) & flag3)
		{
			flag = false;
		}
		if (!mBoard.RowCanHaveZombies(mRow + 1))
		{
			flag2 = false;
		}
		else if (mBoard.mPlantRow[mRow + 1] == PlantRowType.Pool && !flag3)
		{
			flag2 = false;
		}
		else if ((mBoard.mPlantRow[mRow + 1] != PlantRowType.Pool) & flag3)
		{
			flag2 = false;
		}
		if (flag && !flag2)
		{
			mBoard.ZombieSwitchRow(this, mRow - 1);
			SetRow(mRow - 1);
		}
		else if (!flag & flag2)
		{
			mBoard.ZombieSwitchRow(this, mRow + 1);
			mYuckyToRow = mRow + 1;
			mYuckySwitchRowsLate = true;
			mRow = mYuckyToRow;
		}
		else if (flag & flag2)
		{
			if (RandomNumbers.NextNumber(2) == 0)
			{
				mBoard.ZombieSwitchRow(this, mRow + 1);
				mYuckyToRow = mRow + 1;
				mYuckySwitchRowsLate = true;
				mRow = mYuckyToRow;
			}
			else
			{
				mBoard.ZombieSwitchRow(this, mRow - 1);
				SetRow(mRow - 1);
			}
		}
		else
		{
			Debug.ASSERT(value: false);
		}
	}

	public void DrawIceTrap(Graphics g, ref ZombieDrawPosition theDrawPos, bool theFront)
	{
		if (!mInPool && mZombieType != ZombieType.Boss)
		{
			float num = 46f;
			float num2 = 92f + theDrawPos.mBodyY;
			float num3 = 1f;
			switch (mZombieType)
			{
			case ZombieType.Pogo:
				num -= 10f;
				num2 += 20f;
				break;
			case ZombieType.Gargantuar:
			case ZombieType.RedeyeGargantuar:
				num -= 20f;
				num2 -= 7f;
				num3 = 1.6f;
				break;
			case ZombieType.Bungee:
				num -= 45f;
				num2 -= 23f;
				num3 = 1.2f;
				break;
			case ZombieType.Digger:
				num -= 27f;
				break;
			case ZombieType.Catapult:
				num += 32f;
				break;
			case ZombieType.Balloon:
				num -= 9f;
				num2 += 27f;
				break;
			}
			if (theFront)
			{
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_ICETRAP, num * Constants.S, num2 * Constants.S, num3, num3);
			}
			else
			{
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_ICETRAP2, num * Constants.S, num2 * Constants.S, num3, num3);
			}
		}
	}

	public bool HitIceTrap()
	{
		bool flag = false;
		if (mChilledCounter > 0 || mIceTrapCounter != 0)
		{
			flag = true;
		}
		ApplyChill(theIsIceTrap: true);
		if (!CanBeFrozen())
		{
			return false;
		}
		if (mInPool)
		{
			mIceTrapCounter = 300;
		}
		else if (flag)
		{
			mIceTrapCounter = TodCommon.RandRangeInt(300, 400);
		}
		else
		{
			mIceTrapCounter = TodCommon.RandRangeInt(400, 600);
		}
		StopZombieSound();
		if (mZombieType == ZombieType.Balloon || mZombieType == ZombieType.Propeller)
		{
			BalloonPropellerHatSpin(theSpinning: false);
		}
		if (mZombiePhase == ZombiePhase.BossHeadSpit)
		{
			mBoard.RemoveParticleByType(ParticleEffect.ZombieBossFireball);
		}
		TakeDamage(20, 1u);
		UpdateAnimSpeed();
		return true;
	}

	public int GetHelmDamageIndex()
	{
		if (mHelmHealth < mHelmMaxHealth / 3)
		{
			return 2;
		}
		if (mHelmHealth < mHelmMaxHealth * 2 / 3)
		{
			return 1;
		}
		return 0;
	}

	public int GetShieldDamageIndex()
	{
		if (mShieldHealth < mShieldMaxHealth / 3)
		{
			return 2;
		}
		if (mShieldHealth < mShieldMaxHealth * 2 / 3)
		{
			return 1;
		}
		return 0;
	}

	public void DrawReanim(Graphics g, ref ZombieDrawPosition theDrawPos, int theBaseRenderGroup)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return;
		}
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		if (g.mDestImage == null && mZombieType == ZombieType.Ninja && ((mZombiePhase == ZombiePhase.NinjaShownByPlantern && mPhaseCounter > 140) || (mZombiePhase != ZombiePhase.NinjaShownByPlantern && mIsEating)))
		{
			if (mReanimPreview != null)
			{
				Color mColor = g.mColor;
				bool mColorizeImages = g.mColorizeImages;
				int num = 192;
				if (mZombiePhase == ZombiePhase.NinjaShownByPlantern && mPhaseCounter > 140)
				{
					num = (int)(255f * ((float)(180 - mPhaseCounter) / 40f));
				}
				if (mIsEating)
				{
					num = Math.Max(192, num);
				}
				g.SetColor(new SexyColor(255, 255, 255, num), premultiply: false);
				g.SetColorizeImages(colorizeImages: true);
				g.mTransY -= 100;
				g.DrawImage(mReanimPreview, 0, 0);
				g.mTransY += 100;
				g.SetColorizeImages(mColorizeImages);
				g.SetColor(mColor, premultiply: false);
				return;
			}
		}
		else if (g.mDestImage == null && mZombieType == ZombieType.Ninja && mZombiePhase == ZombiePhase.ZombieNormal && IsOnBoard())
		{
			return;
		}
		float mImageOffsetX = theDrawPos.mImageOffsetX;
		float num2 = theDrawPos.mImageOffsetY + theDrawPos.mBodyY;
		switch (mZombieType)
		{
		case ZombieType.Normal:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Normal * mScaleZombie);
			break;
		case ZombieType.Flag:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Normal * mScaleZombie);
			break;
		case ZombieType.TrafficCone:
		case ZombieType.Pail:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Pail * mScaleZombie);
			break;
		case ZombieType.Polevaulter:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Default * mScaleZombie);
			break;
		case ZombieType.Snorkel:
			if (mZombiePhase != ZombiePhase.ZombieDying && mZombiePhase != ZombiePhase.SnorkelWalking)
			{
				num2 = ((mZombiePhase != ZombiePhase.SnorkelIntoPool || mScaleZombie == 1f) ? (num2 - (float)(int)((float)Constants.Zombie_ClipOffset_Snorkel * mScaleZombie)) : (num2 - (float)Constants.Zombie_ClipOffset_Snorkel_intoPool_Small));
			}
			else if (mScaleZombie == 1f)
			{
				num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Snorkel_Dying * mScaleZombie);
			}
			else
			{
				num2 -= (float)Constants.Zombie_ClipOffset_Snorkel_Dying_Small;
				mUsesClipping = true;
			}
			if (draggedByTangleKelp)
			{
				num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Snorkel_Grabbed * mScaleZombie);
				mUsesClipping = true;
			}
			break;
		case ZombieType.DolphinRider:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Dolphin_Into_Pool * mScaleZombie);
			break;
		case ZombieType.Digger:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Digger * mScaleZombie);
			break;
		case ZombieType.PeaHead:
		case ZombieType.WallnutHead:
		case ZombieType.JalapenoHead:
		case ZombieType.GatlingHead:
		case ZombieType.TallnutHead:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_PeaHead_InPool * mScaleZombie);
			break;
		case ZombieType.Football:
		case ZombieType.JackInTheBox:
		case ZombieType.FootballPremium:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_PeaHead_InPool * mScaleZombie);
			break;
		case ZombieType.Propeller:
			num2 -= (float)(int)(72f * mScaleZombie);
			break;
		default:
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Default * mScaleZombie);
			break;
		}
		if (mZombiePhase == ZombiePhase.RisingFromGrave)
		{
			num2 = ((mScaleZombie != 1f) ? (num2 + (float)(int)((float)Constants.Zombie_ClipOffset_RisingFromGrave_Small * mScaleZombie)) : (num2 + (float)(int)((float)Constants.Zombie_ClipOffset_RisingFromGrave * mScaleZombie)));
		}
		else if (mZombiePhase == ZombiePhase.SnorkelIntoPool)
		{
			num2 -= (float)(int)((float)Constants.Zombie_ClipOffset_Snorkel_Into_Pool * mScaleZombie);
		}
		if (mZombieType == ZombieType.Normal && mZombiePhase == ZombiePhase.ZombieNormal && mInPool)
		{
			num2 = ((mScaleZombie != 1f) ? (num2 + (float)Constants.Zombie_ClipOffset_Normal_In_Pool_SMALL) : (num2 + (float)Constants.Zombie_ClipOffset_Normal_In_Pool));
		}
		if (mZombieType == ZombieType.TrafficCone && mZombiePhase == ZombiePhase.ZombieNormal && mInPool && mScaleZombie != 1f)
		{
			num2 += (float)Constants.Zombie_ClipOffset_TrafficCone_In_Pool_SMALL;
		}
		if (mZombieType == ZombieType.Normal && mZombiePhase == ZombiePhase.ZombieDying && mInPool)
		{
			num2 = ((mScaleZombie != 1f) ? (num2 + (float)Constants.Zombie_ClipOffset_Normal_In_Pool_SMALL) : (num2 + (float)Constants.Zombie_ClipOffset_Normal_In_Pool));
		}
		if (mZombieType == ZombieType.Flag && mInPool)
		{
			num2 += (float)Constants.Zombie_ClipOffset_Flag_In_Pool;
		}
		if (mScaleZombie != 1f && mAltitude != 0f && mZombiePhase != ZombiePhase.RisingFromGrave && mInPool)
		{
			num2 -= mAltitude;
		}
		if (theDrawPos.mClipHeight > GameConstants.CLIP_HEIGHT_LIMIT)
		{
			float num3 = 120f - theDrawPos.mClipHeight + 71f;
			g.SetClipRect((int)((mImageOffsetX - 200f) * Constants.S), (int)(num2 * Constants.S), (int)(520f * Constants.S), (int)(num3 * Constants.S));
		}
		if (mUsesClipping)
		{
			g.mClipRect.mX = g.mClipRect.mX + 1;
			g.HardwareClip();
		}
		int num4 = 255;
		if (mZombieFade >= 0)
		{
			num4 = TodCommon.ClampInt((int)((float)(255 * mZombieFade) / 30f), 0, 255);
		}
		SexyColor sexyColor = new SexyColor(255, 255, 255, num4);
		SexyColor sexyColor2 = SexyColor.Black;
		bool mEnableExtraAdditiveDraw = false;
		if (mZombiePhase == ZombiePhase.ZombieBurned)
		{
			sexyColor = new SexyColor(0, 0, 0, num4);
			sexyColor2 = SexyColor.Black;
			mEnableExtraAdditiveDraw = false;
		}
		else if (mZombieType == ZombieType.Boss && mZombiePhase != ZombiePhase.ZombieDying && mBodyHealth < mBodyMaxHealth / GameConstants.BOSS_FLASH_HEALTH_FRACTION)
		{
			int num5 = TodCommon.TodAnimateCurve(0, 39, mBoard.mMainCounter % 40, (int)Constants.InvertAndScale(155f), (int)Constants.InvertAndScale(255f), TodCurves.Bounce);
			if (mChilledCounter > 0 || mIceTrapCounter > 0)
			{
				int num6 = TodCommon.TodAnimateCurve(0, 39, mBoard.mMainCounter % 40, 65, 75, TodCurves.Bounce);
				sexyColor = new SexyColor(num6, num6, num5, num4);
			}
			else
			{
				sexyColor = new SexyColor(num5, num5, num5, num4);
			}
			sexyColor2 = SexyColor.Black;
			mEnableExtraAdditiveDraw = false;
		}
		else if (mMindControlled)
		{
			sexyColor = GameConstants.ZOMBIE_MINDCONTROLLED_COLOR;
			sexyColor.mAlpha = num4;
			sexyColor2 = sexyColor;
			mEnableExtraAdditiveDraw = true;
		}
		else if (mChilledCounter > 0 || mIceTrapCounter > 0)
		{
			sexyColor = new SexyColor(75, 75, 255, num4);
			sexyColor2 = sexyColor;
			mEnableExtraAdditiveDraw = true;
		}
		else if (mZombieHeight == ZombieHeight.Zombiquarium && mBodyHealth < 100)
		{
			sexyColor = new SexyColor(100, 150, 25, num4);
			sexyColor2 = sexyColor;
			mEnableExtraAdditiveDraw = true;
		}
		if (mJustGotShotCounter > 0 && !IsBobsledTeamWithSled() && !GlobalStaticVars.gLowFramerate)
		{
			int num7 = mJustGotShotCounter * 10;
			SexyColor theColor = new SexyColor(num7, num7, num7, 255);
			sexyColor2 = TodCommon.ColorAdd(theColor, sexyColor2);
			mEnableExtraAdditiveDraw = true;
		}
		reanimation.mColorOverride = sexyColor;
		reanimation.mExtraAdditiveColor = sexyColor2;
		reanimation.mEnableExtraAdditiveDraw = mEnableExtraAdditiveDraw;
		switch (mZombieType)
		{
		case ZombieType.Bobsled:
			DrawBobsledReanim(g, ref theDrawPos, theBeforeZombie: true);
			reanimation.DrawRenderGroup(g, theBaseRenderGroup);
			DrawBobsledReanim(g, ref theDrawPos, theBeforeZombie: false);
			break;
		case ZombieType.Bungee:
			DrawBungeeReanim(g, ref theDrawPos);
			break;
		case ZombieType.Dancer:
			DrawDancerReanim(g, ref theDrawPos);
			break;
		default:
			reanimation.DrawRenderGroup(g, theBaseRenderGroup);
			break;
		}
		if (mShieldType != ShieldType.None)
		{
			if (mZombiePhase == ZombiePhase.ZombieBurned)
			{
				reanimation.mColorOverride = new SexyColor(0, 0, 0, num4);
				reanimation.mExtraAdditiveColor = SexyColor.Black;
				reanimation.mEnableExtraAdditiveDraw = false;
			}
			else if (mShieldJustGotShotCounter > 0)
			{
				reanimation.mColorOverride = new SexyColor(255, 255, 255, num4);
				reanimation.mExtraAdditiveColor = SexyColor.White;
				reanimation.mEnableExtraAdditiveDraw = true;
			}
			else
			{
				reanimation.mColorOverride = new SexyColor(255, 255, 255, num4);
				reanimation.mExtraAdditiveColor = SexyColor.Black;
				reanimation.mEnableExtraAdditiveDraw = false;
			}
			float num8 = 0f;
			if (mShieldRecoilCounter > 0)
			{
				num8 = TodCommon.TodAnimateCurveFloat(12, 0, mShieldRecoilCounter, 3f, 0f, TodCurves.Linear);
			}
			g.mTransX += (int)num8;
			reanimation.DrawRenderGroup(g, GameConstants.RENDER_GROUP_SHIELD);
			g.mTransX -= (int)num8;
		}
		if (mShieldType == ShieldType.Newspaper || mShieldType == ShieldType.Door || mShieldType == ShieldType.Ladder)
		{
			reanimation.mColorOverride = sexyColor;
			reanimation.mExtraAdditiveColor = sexyColor2;
			reanimation.mEnableExtraAdditiveDraw = mEnableExtraAdditiveDraw;
			reanimation.DrawRenderGroup(g, GameConstants.RENDER_GROUP_OVER_SHIELD);
		}
		g.ClearClipRect();
		if (mUsesClipping)
		{
			g.EndHardwareClip();
		}
	}

	public void UpdatePlaying()
	{
		Debug.ASSERT(mBodyHealth > 0 || mZombiePhase == ZombiePhase.BobsledCrashing);
		mGroanCounter--;
		int count = mBoard.mZombies.Count;
		if (mGroanCounter <= 0 && RandomNumbers.NextNumber(count) == 0 && mHasHead && mZombieType != ZombieType.Boss && !mBoard.HasLevelAwardDropped())
		{
			float aPitch = 0f;
			if (mApp.IsLittleTroubleLevel())
			{
				aPitch = TodCommon.RandRangeFloat(40f, 50f);
			}
			if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
			{
				mApp.PlayFoley(FoleyType.Lowgroan);
			}
			else if (mVariant)
			{
				mApp.PlayFoleyPitch(FoleyType.Brains, aPitch);
			}
			else
			{
				mApp.PlayFoleyPitch(FoleyType.Groan, aPitch);
			}
			mGroanCounter = RandomNumbers.NextNumber(1000) + 500;
		}
		if (mIceTrapCounter > 0)
		{
			mIceTrapCounter--;
			if (mIceTrapCounter <= 0)
			{
				RemoveIceTrap();
				AddAttachedParticle(75, 106, ParticleEffect.IceTrapRelease);
			}
		}
		if (mChilledCounter > 0)
		{
			mChilledCounter--;
			if (mChilledCounter <= 0)
			{
				UpdateAnimSpeed();
			}
		}
		if (mButteredCounter > 0)
		{
			mButteredCounter--;
			if (mButteredCounter <= 0)
			{
				RemoveButter();
			}
		}
		if (mZombiePhase == ZombiePhase.RisingFromGrave)
		{
			UpdateZombieRiseFromGrave();
			return;
		}
		mBodyReanimID.mClip = false;
		if (!IsImmobilizied())
		{
			UpdateActions();
			UpdateZombiePosition();
			CheckIfPreyCaught();
			CheckForPool();
			CheckForHighGround();
			CheckForBoardEdge();
		}
		if (mZombieType == ZombieType.Boss)
		{
			UpdateBoss();
		}
		if (mZombieType == ZombieType.Ninja)
		{
			UpdateNinja();
		}
		if (IsDeadOrDying() || mFromWave == GameConstants.ZOMBIE_WAVE_WINNER)
		{
			return;
		}
		bool flag = !mHasHead;
		if ((mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan) && !mHasHead)
		{
			flag = false;
		}
		if ((mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Catapult) && mBodyHealth < 200)
		{
			flag = true;
		}
		if (flag)
		{
			int theDamage = 1;
			if (mZombieType == ZombieType.Yeti)
			{
				theDamage = 10;
			}
			if (mBodyMaxHealth >= 500)
			{
				theDamage = 3;
			}
			if (RandomNumbers.NextNumber(5) == 0)
			{
				TakeDamage(theDamage, 9u);
			}
		}
	}

	public void UpdateNinja()
	{
		int index = -1;
		Plant thePlant = null;
		float num = 180f;
		while (mBoard.IteratePlants(ref thePlant, ref index))
		{
			if (thePlant.mSeedType == SeedType.Plantern)
			{
				TRect zombieRect = GetZombieRect();
				float num2 = TodCommon.Distance2D((float)thePlant.mX + 40f, (float)thePlant.mY + 40f, zombieRect.mX + zombieRect.mWidth / 2, zombieRect.mY + zombieRect.mHeight / 2);
				if (thePlant.mState == PlantState.PlanternHenshinOver || (thePlant.mState == PlantState.PlanternHenshinBegin && thePlant.mStateCountdown < 50))
				{
					num = 1f;
				}
				else if (num2 / 340f * 180f < num)
				{
					num = num2 / 340f * 180f;
				}
			}
			else if (thePlant.mSeedType == SeedType.Torchwood)
			{
				TRect zombieRect2 = GetZombieRect();
				float num3 = TodCommon.Distance2D((float)thePlant.mX + 40f, (float)thePlant.mY + 40f, zombieRect2.mX + zombieRect2.mWidth / 2, zombieRect2.mY + zombieRect2.mHeight / 2);
				if (num3 / 120f * 180f < num)
				{
					num = num3 / 120f * 180f;
				}
			}
		}
		if (num < 180f && (mZombiePhase == ZombiePhase.ZombieNormal || mZombiePhase == ZombiePhase.NinjaShownByPlantern))
		{
			mZombiePhase = ZombiePhase.NinjaShownByPlantern;
			mPhaseCounter = (int)num;
		}
		else if (mZombiePhase == ZombiePhase.NinjaShownByPlantern)
		{
			mZombiePhase = ZombiePhase.ZombieNormal;
		}
		if (mBodyHealth < mBodyMaxHealth * 2 / 3)
		{
			mApp.ReanimationTryToGet(mBodyReanimID)?.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_NINJA_HAIR2);
		}
	}

	public bool NeedsMoreBackupDancers()
	{
		for (int i = 0; i < GameConstants.NUM_BACKUP_DANCERS; i++)
		{
			if (mBoard.ZombieTryToGet(mFollowerZombieID[i]) == null && (i != 0 || mBoard.RowCanHaveZombieType(mRow - 1, ZombieType.BackupDancer)) && (i != 1 || mBoard.RowCanHaveZombieType(mRow + 1, ZombieType.BackupDancer)))
			{
				return true;
			}
		}
		return false;
	}

	public void ConvertToNormalZombie()
	{
		StopZombieSound();
		mPosY = GetPosYBasedOnRow(mRow);
		mX = (int)mPosX;
		mY = (int)mPosY;
		mZombieType = ZombieType.Normal;
		mZombiePhase = ZombiePhase.ZombieNormal;
		mZombieAttackRect = new TRect(50, 0, 20, 115);
		mAnimFrames = 12;
		mAnimTicksPerFrame = 12;
		mPhaseCounter = 0;
		PickRandomSpeed();
	}

	public void StartEating()
	{
		if (mIsEating)
		{
			return;
		}
		mIsEating = true;
		if (mZombiePhase == ZombiePhase.DiggerTunneling)
		{
			return;
		}
		if (mZombiePhase == ZombiePhase.LadderCarrying)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_laddereat, ReanimLoopType.Loop, 20, 0f);
			return;
		}
		if (mZombiePhase == ZombiePhase.NewspaperMad)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_eat_nopaper, ReanimLoopType.Loop, 20, 0f);
			return;
		}
		if (mZombieType != ZombieType.Snorkel)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_eat, ReanimLoopType.Loop, 20, 0f);
		}
		if (mShieldType == ShieldType.Door)
		{
			ShowDoorArms(theShow: false);
		}
	}

	public void StopEating()
	{
		if (!mIsEating)
		{
			return;
		}
		mIsEating = false;
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (mZombiePhase != ZombiePhase.DiggerTunneling)
		{
			if (reanimation != null && mZombieType != ZombieType.Snorkel)
			{
				StartWalkAnim(20);
			}
			if (mShieldType == ShieldType.Door)
			{
				ShowDoorArms(theShow: true);
			}
			UpdateAnimSpeed();
		}
	}

	public void UpdateAnimSpeed()
	{
		if (!IsOnBoard())
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return;
		}
		if (IsImmobilizied())
		{
			ApplyAnimRate(0f);
		}
		else if (mYuckyFace && mYuckyFaceCounter < GameConstants.YUCKI_HOLD_TIME)
		{
			ApplyAnimRate(0f);
		}
		else if (mZombiePhase == ZombiePhase.SnorkelUpToEat || mZombiePhase == ZombiePhase.SnorkelDownFromEat || IsDeadOrDying())
		{
			ApplyAnimRate(mOrginalAnimRate);
		}
		else if (mIsEating)
		{
			if (mZombieType == ZombieType.Polevaulter || mZombieType == ZombieType.Balloon || mZombieType == ZombieType.Imp || mZombieType == ZombieType.Digger || mZombieType == ZombieType.JackInTheBox || mZombieType == ZombieType.Snorkel || mZombieType == ZombieType.Yeti || mZombieType == ZombieType.Propeller)
			{
				ApplyAnimRate(20f);
			}
			else
			{
				ApplyAnimRate(36f);
			}
		}
		else if (ZombieNotWalking())
		{
			ApplyAnimRate(mOrginalAnimRate);
		}
		else if (IsBobsledTeamWithSled() || mZombieType == ZombieType.Catapult || mZombiePhase == ZombiePhase.DolphinRiding || mZombiePhase == ZombiePhase.SnorkelWalkingInPool)
		{
			ApplyAnimRate(mOrginalAnimRate);
		}
		else if (mHasGroundTrack)
		{
			ReanimatorTrack reanimatorTrack = reanimation.mDefinition.mTracks[reanimation.FindTrackIndex(Reanimation.ReanimTrackId__ground)];
			float num = reanimatorTrack.mTransforms[reanimation.mFrameStart + reanimation.mFrameCount - 1].mTransX - reanimatorTrack.mTransforms[reanimation.mFrameStart].mTransX;
			if (!(num < 1E-06f))
			{
				float num2 = (float)reanimation.mFrameCount / num;
				float theAnimRate = mVelX * num2 * 47f / mScaleZombie;
				ApplyAnimRate(theAnimRate);
			}
		}
	}

	public void ReanimShowPrefix(string theTrackPrefix, int theRenderGroup)
	{
		mApp.ReanimationTryToGet(mBodyReanimID)?.AssignRenderGroupToPrefix(theTrackPrefix, theRenderGroup);
	}

	public void DetachPlantHead()
	{
		ReanimatorTrackInstance trackInstanceByName = mBodyReanimID.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
		GlobalMembersAttachment.AttachmentDetach(ref trackInstanceByName.mAttachmentID);
	}

	public void DetachFlag()
	{
		ReanimatorTrackInstance trackInstanceByName = mBodyReanimID.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_flaghand);
		GlobalMembersAttachment.AttachmentDetach(ref trackInstanceByName.mAttachmentID);
	}

	public void PlayDeathAnim(uint theDamageFlags)
	{
		if (mZombiePhase == ZombiePhase.ZombieDying || mZombiePhase == ZombiePhase.ZombieBurned || mZombiePhase == ZombiePhase.ZombieMowered)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null || !reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_death))
		{
			DieNoLoot(giveAchievements: true);
			return;
		}
		if (mZombieType == ZombieType.DolphinRider && mZombiePhase != ZombiePhase.DolphinWalkingInPool)
		{
			DieNoLoot(giveAchievements: true);
			return;
		}
		if (mZombiePhase == ZombiePhase.SnorkelIntoPool || mZombiePhase == ZombiePhase.SnorkelWalking)
		{
			DieNoLoot(giveAchievements: true);
			return;
		}
		if (mZombieType == ZombieType.Ninja)
		{
			int num = 0;
			foreach (Zombie mZombie in mBoard.mZombies)
			{
				if (mZombie.mZombieType == ZombieType.Ninja && mZombie.mZombiePhase == ZombiePhase.ZombieDying)
				{
					num++;
					if (num >= 2)
					{
						break;
					}
				}
			}
			if (num < 2)
			{
				PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Ninja_Die);
			}
		}
		if (mIceTrapCounter > 0)
		{
			AddAttachedParticle(75, 106, ParticleEffect.IceTrapRelease);
			mIceTrapCounter = 0;
		}
		if (mButteredCounter > 0)
		{
			mButteredCounter = 0;
		}
		if (mYuckyFace)
		{
			ShowYuckyFace(theShow: false);
			mYuckyFace = false;
			mYuckyFaceCounter = 0;
		}
		if (TodCommon.TestBit(theDamageFlags, 4) && mZombieType != ZombieType.Boss && mZombieType != ZombieType.Gargantuar && mZombieType != ZombieType.RedeyeGargantuar && mZombieType != ZombieType.RobotTitan && mZombieType != ZombieType.RedeyeRobotTitan)
		{
			DieNoLoot(giveAchievements: true);
			return;
		}
		if (mZombieType == ZombieType.Pogo)
		{
			mAltitude = 0f;
		}
		GlobalMembersAttachment.AttachmentReanimTypeDie(ref mAttachmentID, ReanimationType.ZombieSurprise);
		StopEating();
		if (mShieldType != ShieldType.None)
		{
			DropShield(1u);
		}
		if (mZombieType == ZombieType.SquashHead && !mHasHead)
		{
			DetachPlantHead();
			mApp.RemoveReanimation(ref mSpecialHeadReanimID);
			mSpecialHeadReanimID = null;
		}
		mZombiePhase = ZombiePhase.ZombieDying;
		mVelX = 0f;
		if (mZombieHeight == ZombieHeight.Zombiquarium)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_aquarium_death, ReanimLoopType.PlayOnceAndHold, 20, 14f);
			return;
		}
		if (mZombieHeight == ZombieHeight.UpLadder)
		{
			mZombieHeight = ZombieHeight.Falling;
		}
		float theAnimRate;
		if (mZombieType == ZombieType.Football)
		{
			theAnimRate = 24f;
		}
		else if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
		{
			theAnimRate = 14f;
			mApp.PlayFoley(FoleyType.Gargantudeath);
		}
		else if (mZombieType == ZombieType.Snorkel)
		{
			theAnimRate = 14f;
		}
		else if (mZombieType == ZombieType.Digger)
		{
			theAnimRate = 18f;
		}
		else if (mZombieType == ZombieType.Yeti)
		{
			theAnimRate = 14f;
		}
		else if (mZombieType == ZombieType.Boss)
		{
			theAnimRate = 18f;
			BossDie();
			Reanimation reanimation2 = mApp.ReanimationGet(mSpecialHeadReanimID);
			reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_death, ReanimLoopType.PlayOnceAndHold, 20, theAnimRate);
		}
		else
		{
			theAnimRate = TodCommon.RandRangeFloat(24f, 30f);
		}
		string theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_death;
		int num2 = RandomNumbers.NextNumber(100);
		bool flag = mApp.HasFinishedAdventure() || mBoard.mLevel > 5;
		if (mInPool && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_waterdeath))
		{
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_waterdeath;
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_duckytube, theIgnoreClipRect: false);
			if (mZombieType == ZombieType.Propeller)
			{
				ReanimIgnoreClipRect("zombie_propeller_duckytube_whole", theIgnoreClipRect: false);
				ReanimIgnoreClipRect("zombie_propeller_engine", theIgnoreClipRect: false);
				ReanimIgnoreClipRect("zombie_propeller_engine_whole", theIgnoreClipRect: false);
			}
		}
		else if (((num2 == 99 && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_superlongdeath)) & flag) && mChilledCounter == 0 && mBoard.CountZombiesOnScreen() <= 5)
		{
			theAnimRate = 14f;
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_superlongdeath;
		}
		else if (num2 > 50 && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_death2))
		{
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_death2;
		}
		PlayZombieReanim(ref theTrackName, ReanimLoopType.PlayOnceAndHold, 20, theAnimRate);
		ReanimShowPrefix("anim_tongue", -1);
	}

	public void UpdateDeath()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			DieNoLoot(giveAchievements: true);
			return;
		}
		if (mZombieHeight == ZombieHeight.Falling)
		{
			UpdateZombieFalling();
		}
		if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
		{
			if (reanimation.ShouldTriggerTimedEvent(0.89f))
			{
				mBoard.ShakeBoard(0, 3);
			}
			else if (reanimation.ShouldTriggerTimedEvent(0.98f))
			{
				mBoard.ShakeBoard(0, 1);
			}
		}
		float num = -1f;
		if (mInPool || mZombieType == ZombieType.Snorkel || mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.DolphinRider || mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Catapult || mZombieType == ZombieType.Imp || mZombieType == ZombieType.Boss)
		{
			num = -1f;
		}
		else if (mZombieType == ZombieType.Normal || mZombieType == ZombieType.Flag || mZombieType == ZombieType.TrafficCone || mZombieType == ZombieType.Pail || mZombieType == ZombieType.Door || mZombieType == ZombieType.PeaHead || mZombieType == ZombieType.WallnutHead || mZombieType == ZombieType.TallnutHead || mZombieType == ZombieType.JalapenoHead || mZombieType == ZombieType.GatlingHead || mZombieType == ZombieType.SquashHead || mZombieType == ZombieType.DuckyTube)
		{
			if (reanimation.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_superlongdeath))
			{
				num = 0.788f;
			}
			else
			{
				num = ((!reanimation.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_death2)) ? 0.77f : 0.71f);
			}
		}
		else if (mZombieType == ZombieType.Polevaulter)
		{
			num = 0.68f;
		}
		else if (mZombieType == ZombieType.Football)
		{
			num = 0.52f;
		}
		else if (mZombieType == ZombieType.Newspaper)
		{
			num = 0.63f;
		}
		else if (mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer)
		{
			num = 0.83f;
		}
		else if (mZombieType == ZombieType.Bobsled)
		{
			num = 0.81f;
		}
		else if (mZombieType == ZombieType.JackInTheBox)
		{
			num = 0.64f;
		}
		else if (mZombieType == ZombieType.Balloon)
		{
			num = 0.68f;
		}
		else if (mZombieType == ZombieType.Propeller)
		{
			num = 0.68f;
		}
		else if (mZombieType == ZombieType.Digger)
		{
			num = 0.85f;
		}
		else if (mZombieType == ZombieType.Pogo)
		{
			num = 0.84f;
		}
		else if (mZombieType == ZombieType.Yeti)
		{
			num = 0.68f;
		}
		else if (mZombieType == ZombieType.Ladder)
		{
			num = 0.62f;
		}
		else if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
		{
			num = 0.86f;
		}
		if (num > 0f && reanimation.ShouldTriggerTimedEvent(num))
		{
			mApp.PlayFoley(FoleyType.ZombieFalling);
			if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
			{
				mApp.PlayFoley(FoleyType.Thump);
				mApp.Vibrate();
			}
			if (mBoard.mDaisyMode)
			{
				DoDaisies();
			}
		}
		if (mZombieType == ZombieType.Boss)
		{
			if (reanimation.ShouldTriggerTimedEvent(0.1f) || reanimation.ShouldTriggerTimedEvent(0.12f) || reanimation.ShouldTriggerTimedEvent(0.15f) || reanimation.ShouldTriggerTimedEvent(0.19f) || reanimation.ShouldTriggerTimedEvent(0.2f) || reanimation.ShouldTriggerTimedEvent(0.26f) || reanimation.ShouldTriggerTimedEvent(0.3f) || reanimation.ShouldTriggerTimedEvent(0.4f) || reanimation.ShouldTriggerTimedEvent(0.42f) || reanimation.ShouldTriggerTimedEvent(0.5f) || reanimation.ShouldTriggerTimedEvent(0.58f) || reanimation.ShouldTriggerTimedEvent(0.61f) || reanimation.ShouldTriggerTimedEvent(0.71f))
			{
				float theX = TodCommon.RandRangeFloat(600f, 750f);
				float theY = TodCommon.RandRangeFloat(50f, 300f);
				mApp.AddTodParticle(theX, theY, 400000, ParticleEffect.BossExplosion);
				mApp.PlayFoley(FoleyType.BossExplosionSmall);
			}
			Reanimation reanimation2 = mApp.ReanimationTryToGet(mSpecialHeadReanimID);
			if (reanimation.ShouldTriggerTimedEvent(0.93f))
			{
				mBoard.ShakeBoard(1, 2);
				mApp.PlayFoley(FoleyType.BossExplosionSmall);
				mApp.PlayFoley(FoleyType.Thump);
				mApp.Vibrate();
			}
			if (reanimation.ShouldTriggerTimedEvent(0.99f))
			{
				reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_flag, ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			if (reanimation2.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_flag) && reanimation2.mLoopCount > 0)
			{
				reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_flag_loop, ReanimLoopType.Loop, 20, 17f);
			}
			if (reanimation.mLoopCount > 0)
			{
				DropLoot();
			}
		}
		if (mZombieType == ZombieType.Zamboni && mPhaseCounter > 0)
		{
			mPhaseCounter--;
			if (mPhaseCounter <= 0)
			{
				reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
				if (reanimation.IsTrackShowing(GlobalMembersReanimIds.ReanimTrackId_anim_wheelie2))
				{
					mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.ZamboniExplosion2);
				}
				else
				{
					mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.ZamboniExplosion);
				}
				DieWithLoot();
				mApp.PlayFoley(FoleyType.Explosion);
			}
		}
		else if ((mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan) && mPhaseCounter > 0)
		{
			mPhaseCounter--;
			if (mPhaseCounter <= 0)
			{
				reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
				if (reanimation.IsTrackShowing(GlobalMembersReanimIds.ReanimTrackId_anim_wheelie2))
				{
					mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.RobotTitanExplosion);
				}
				else
				{
					mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.RobotTitanExplosion);
				}
				DieWithLoot();
				mApp.PlayFoley(FoleyType.Explosion);
			}
		}
		else if (mZombieType == ZombieType.Catapult)
		{
			mPhaseCounter--;
			if (mPhaseCounter <= 0)
			{
				mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.CatapultExplosion);
				DieWithLoot();
				mApp.PlayFoley(FoleyType.Explosion);
			}
		}
		else if (mZombieFade == -1 && reanimation.mLoopCount > 0 && mZombieType != ZombieType.Boss)
		{
			if (mInPool)
			{
				mZombieFade = 30;
			}
			else
			{
				mZombieFade = 100;
			}
		}
	}

	public void DrawShadow(Graphics g)
	{
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		if (mApp.mGameScene == GameScenes.ZombiesWon && !SetupDrawZombieWon(g))
		{
			return;
		}
		int num = 0;
		float mImageOffsetX = theDrawPos.mImageOffsetX;
		float num2 = theDrawPos.mImageOffsetY + theDrawPos.mBodyY;
		float num3 = mScaleZombie;
		mImageOffsetX += mScaleZombie * 20f - 20f;
		if (IsOnBoard() && mBoard.StageIsNight())
		{
			num = 1;
		}
		switch (mZombieType)
		{
		case ZombieType.Football:
		case ZombieType.FootballPremium:
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + (20f + 21f * mScaleZombie)) : (mImageOffsetX + -11f * mScaleZombie));
			num2 += 16f;
			break;
		case ZombieType.Newspaper:
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 29f) : (mImageOffsetX + 5f));
			break;
		case ZombieType.Polevaulter:
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 36f) : (mImageOffsetX + -5f));
			num2 += 11f;
			break;
		case ZombieType.Bobsled:
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 20f) : (mImageOffsetX + 13f));
			num2 += 13f;
			break;
		case ZombieType.Imp:
			num3 *= 0.6f;
			num2 += 7f;
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 25f) : (mImageOffsetX + 13f));
			break;
		case ZombieType.Digger:
			num2 += 5f;
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 17f) : (mImageOffsetX + 14f));
			break;
		case ZombieType.Snorkel:
			num2 += 5f;
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 35f) : (mImageOffsetX + -2f));
			break;
		case ZombieType.DolphinRider:
			num2 += 11f;
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 19f) : (mImageOffsetX + 15f));
			break;
		case ZombieType.Yeti:
			num2 += 20f;
			mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 3f) : (mImageOffsetX + 20f));
			break;
		case ZombieType.Gargantuar:
		case ZombieType.RedeyeGargantuar:
			num3 *= 1.5f;
			mImageOffsetX += 27f;
			num2 += 7f;
			break;
		default:
			if (mApp.ReanimationTryToGet(mBodyReanimID) == null)
			{
				mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 35f) : (mImageOffsetX + -2f));
			}
			else
			{
				mImageOffsetX = ((!IsWalkingBackwards()) ? (mImageOffsetX + 23f) : (mImageOffsetX + 11f));
			}
			break;
		}
		switch (mZombieType)
		{
		case ZombieType.Newspaper:
			num2 += 4f;
			break;
		case ZombieType.Balloon:
			num2 += 13f;
			break;
		case ZombieType.Bungee:
			mImageOffsetX += -12f;
			num3 = TodCommon.TodAnimateCurveFloat(GameConstants.BUNGEE_ZOMBIE_HEIGHT - 1000, 100, (int)mAltitude, 0.1f, 1.5f, TodCurves.Linear);
			break;
		case ZombieType.Dancer:
		case ZombieType.BackupDancer:
			num2 -= 18f;
			break;
		case ZombieType.Ninja:
			if (mZombiePhase == ZombiePhase.ZombieNormal)
			{
				return;
			}
			break;
		}
		if (mZombieHeight == ZombieHeight.UpLadder || mZombieHeight == ZombieHeight.Falling || mZombiePhase == ZombiePhase.ImpGettingThrown || mZombieType == ZombieType.Bungee || IsBouncingPogo() || IsFlying())
		{
			num2 += mAltitude;
			if (mOnHighGround)
			{
				num2 -= (float)Constants.HIGH_GROUND_HEIGHT;
			}
		}
		if (mUsesClipping)
		{
			g.HardwareClip();
		}
		if (mInPool)
		{
			num2 += 67f;
			TodCommon.TodDrawImageCenterScaledF(g, AtlasResources.IMAGE_WHITEWATER_SHADOW, mImageOffsetX * Constants.S, num2 * Constants.S, num3, num3);
		}
		else
		{
			num2 += 92f;
			if (num == 0)
			{
				TodCommon.TodDrawImageCenterScaledF(g, AtlasResources.IMAGE_PLANTSHADOW, mImageOffsetX * Constants.S, num2 * Constants.S, num3, num3);
			}
			else
			{
				TodCommon.TodDrawImageCenterScaledF(g, AtlasResources.IMAGE_PLANTSHADOW2, mImageOffsetX * Constants.S, num2 * Constants.S, num3, num3);
			}
		}
		if (mUsesClipping)
		{
			g.EndHardwareClip();
		}
		g.ClearClipRect();
	}

	public bool HasShadow()
	{
		if (mZombiePhase != ZombiePhase.ZombieDying && mZombiePhase != ZombiePhase.DiggerRising && mZombiePhase != ZombiePhase.DiggerTunnelingPauseWithoutAxe && mZombiePhase != ZombiePhase.DiggerRiseWithoutAxe && mZombiePhase != ZombiePhase.DiggerTunneling && mZombiePhase != ZombiePhase.RisingFromGrave && mZombiePhase != ZombiePhase.DancerRising && mZombiePhase != ZombiePhase.BobsledBoarding && mZombiePhase != ZombiePhase.PolevaulterInVault && mZombiePhase != ZombiePhase.DolphinIntoPool && mZombiePhase != ZombiePhase.SnorkelIntoPool && mZombieType != ZombieType.Zamboni && mZombieType != ZombieType.Catapult && mZombieType != ZombieType.Boss && (mZombieType != ZombieType.Bungee || (IsOnBoard() && !mHitUmbrella)) && mZombieHeight != ZombieHeight.DraggedUnder && mZombieHeight != ZombieHeight.InToChimney && mZombieHeight != ZombieHeight.GettingBungeeDropped && !mInPool && (!IsOnBoard() || mApp.mGameMode != GameMode.ChallengeZombiquarium))
		{
			if (mApp.mGameMode == GameMode.ChallengeInvisighoul || mApp.mCreativeLevel?.GetComponent<CSInvisibleZombie>() != null)
			{
				return mFromWave == GameConstants.ZOMBIE_WAVE_UI;
			}
			return true;
		}
		return false;
	}

	public Reanimation LoadReanim(ReanimationType theReanimationType)
	{
		Reanimation reanimation = mApp.AddReanimation(0f, 0f, 0, theReanimationType);
		mBodyReanimID = mApp.ReanimationGetID(reanimation);
		reanimation.mLoopType = ReanimLoopType.Loop;
		reanimation.mIsAttachment = true;
		mHasGroundTrack = reanimation.TrackExists(Reanimation.ReanimTrackId__ground);
		if (!IsOnBoard())
		{
			int num = RandomNumbers.NextNumber(4);
			if (num > 0 && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle2))
			{
				float theAnimRate = TodCommon.RandRangeFloat(12f, 24f);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle2, ReanimLoopType.Loop, 0, theAnimRate);
			}
			else if (reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
			{
				float theAnimRate2 = TodCommon.RandRangeFloat(12f, 18f);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, theAnimRate2);
			}
			reanimation.mAnimTime = TodCommon.RandRangeFloat(0f, 0.99f);
		}
		else
		{
			StartWalkAnim(0);
		}
		return reanimation;
	}

	public int TakeFlyingDamage(int theDamage, uint theDamageFlags)
	{
		if (!TodCommon.TestBit(theDamageFlags, 3))
		{
			mJustGotShotCounter = 25;
		}
		int num = Math.Min(mFlyingHealth, theDamage);
		int result = theDamage - num;
		mFlyingHealth -= num;
		if (mFlyingHealth == 0)
		{
			LandFlyer(theDamageFlags);
		}
		return result;
	}

	public int TakeShieldDamage(int theDamage, uint theDamageFlags)
	{
		if (!TodCommon.TestBit(theDamageFlags, 3))
		{
			mShieldJustGotShotCounter = 25;
			if (mJustGotShotCounter < 0)
			{
				mJustGotShotCounter = 0;
			}
		}
		if (!TodCommon.TestBit(theDamageFlags, 3) && !TodCommon.TestBit(theDamageFlags, 1))
		{
			mShieldRecoilCounter = 12;
			if (mShieldType == ShieldType.Door || mShieldType == ShieldType.Ladder)
			{
				mApp.PlayFoley(FoleyType.ShieldHit);
			}
		}
		int shieldDamageIndex = GetShieldDamageIndex();
		int result = theDamage - Math.Min(mShieldHealth, theDamage);
		mShieldHealth -= Math.Min(mShieldHealth, theDamage);
		if (mShieldHealth == 0)
		{
			DropShield(theDamageFlags);
			return result;
		}
		int shieldDamageIndex2 = GetShieldDamageIndex();
		if (shieldDamageIndex != shieldDamageIndex2 || justLoaded)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (mShieldType == ShieldType.Door && shieldDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_screendoor, AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR2);
			}
			else if (mShieldType == ShieldType.Door && shieldDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_screendoor, AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR3);
			}
			else if (mShieldType == ShieldType.Newspaper && shieldDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_paper_paper, AtlasResources.IMAGE_REANIM_ZOMBIE_PAPER_PAPER2);
			}
			else if (mShieldType == ShieldType.Newspaper && shieldDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_paper_paper, AtlasResources.IMAGE_REANIM_ZOMBIE_PAPER_PAPER3);
			}
			else if (mShieldType == ShieldType.Ladder && shieldDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_ladder_1, AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_1_DAMAGE1);
			}
			else if (mShieldType == ShieldType.Ladder && shieldDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_ladder_1, AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_1_DAMAGE2);
			}
		}
		return result;
	}

	public int TakeHelmDamage(int theDamage, uint theDamageFlags)
	{
		if (!TodCommon.TestBit(theDamageFlags, 3))
		{
			mJustGotShotCounter = 25;
		}
		int helmDamageIndex = GetHelmDamageIndex();
		int result = theDamage - Math.Min(mHelmHealth, theDamage);
		mHelmHealth -= Math.Min(mHelmHealth, theDamage);
		if (TodCommon.TestBit(theDamageFlags, 2))
		{
			ApplyChill(theIsIceTrap: false);
		}
		if ((TodCommon.TestBit(theDamageFlags, 5) || (TodCommon.TestBit(theDamageFlags, 1) && !TodCommon.TestBit(theDamageFlags, 4))) && mHelmType == HelmType.Bell)
		{
			return theDamage;
		}
		if (mHelmHealth == 0)
		{
			if (mHelmType == HelmType.Bell)
			{
				result = 0;
			}
			DropHelm(theDamageFlags);
			return result;
		}
		int helmDamageIndex2 = GetHelmDamageIndex();
		if ((helmDamageIndex != helmDamageIndex2 && mBodyReanimID.mActive) || (justLoaded && mBodyReanimID.mActive))
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (mHelmType == HelmType.TrafficCone && helmDamageIndex2 == 1 && reanimation != null)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_cone, AtlasResources.IMAGE_REANIM_ZOMBIE_CONE2);
			}
			else if (mHelmType == HelmType.TrafficCone && helmDamageIndex2 == 2 && reanimation != null)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_cone, AtlasResources.IMAGE_REANIM_ZOMBIE_CONE3);
			}
			else if (mHelmType == HelmType.Pail && helmDamageIndex2 == 1)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_bucket, AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET2);
			}
			else if (mHelmType == HelmType.Pail && helmDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_bucket, AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET3);
			}
			else if (mHelmType == HelmType.Digger && helmDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_digger_hardhat, AtlasResources.IMAGE_REANIM_ZOMBIE_DIGGER_HARDHAT2);
			}
			else if (mHelmType == HelmType.Digger && helmDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_digger_hardhat, AtlasResources.IMAGE_REANIM_ZOMBIE_DIGGER_HARDHAT3);
			}
			else if (mHelmType == HelmType.Football && helmDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_HELMET2);
			}
			else if (mHelmType == HelmType.Football && helmDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_HELMET3);
			}
			else if (mHelmType == HelmType.FootballPremium && helmDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_HELMET2);
			}
			else if (mHelmType == HelmType.FootballPremium && helmDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_HELMET3);
			}
			else if (mHelmType == HelmType.Wallnut && helmDamageIndex2 == 1)
			{
				mApp.ReanimationGet(mSpecialHeadReanimID)?.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_face, AtlasResources.IMAGE_REANIM_WALLNUT_CRACKED1);
			}
			else if (mHelmType == HelmType.Wallnut && helmDamageIndex2 == 2)
			{
				mApp.ReanimationGet(mSpecialHeadReanimID)?.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_face, AtlasResources.IMAGE_REANIM_WALLNUT_CRACKED2);
			}
			else if (mHelmType == HelmType.Tallnut && helmDamageIndex2 == 1)
			{
				mApp.ReanimationGet(mSpecialHeadReanimID)?.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_idle, AtlasResources.IMAGE_REANIM_TALLNUT_CRACKED1);
			}
			else if (mHelmType == HelmType.Tallnut && helmDamageIndex2 == 2)
			{
				mApp.ReanimationGet(mSpecialHeadReanimID)?.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_idle, AtlasResources.IMAGE_REANIM_TALLNUT_CRACKED2);
			}
			else if (mHelmType == HelmType.RobotTitanHead && helmDamageIndex2 == 2)
			{
				Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
				reanimation2.SetImageOverride("anim_head", AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_HEAD2);
			}
			else if (mHelmType == HelmType.RedeyeRobotTitanHead && helmDamageIndex2 == 2)
			{
				Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
				reanimation3.SetImageOverride("anim_head", AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_HEAD2_REDEYE);
			}
			else if (mHelmType == HelmType.Bell && helmDamageIndex2 == 1)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride("special", AtlasResources.IMAGE_REANIM_SPECIAL_2);
			}
			else if (mHelmType == HelmType.Bell && helmDamageIndex2 == 2)
			{
				Debug.ASSERT(reanimation != null);
				reanimation.SetImageOverride("special", AtlasResources.IMAGE_REANIM_SPECIAL_3);
			}
		}
		return result;
	}

	public void TakeBodyDamage(int theDamage, uint theDamageFlags)
	{
		if (!TodCommon.TestBit(theDamageFlags, 3))
		{
			mJustGotShotCounter = 25;
		}
		if (TodCommon.TestBit(theDamageFlags, 2))
		{
			ApplyChill(theIsIceTrap: false);
		}
		int num = mBodyHealth;
		int bodyDamageIndex = GetBodyDamageIndex();
		mBodyHealth -= theDamage;
		int bodyDamageIndex2 = GetBodyDamageIndex();
		if (mZombieType == ZombieType.Zamboni)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (!TodCommon.TestBit(theDamageFlags, 3))
			{
				mApp.PlayFoley(FoleyType.ShieldHit);
			}
			if (TodCommon.TestBit(theDamageFlags, 5))
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_1, AtlasResources.IMAGE_REANIM_ZOMBIE_ZAMBONI_1_DAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_2, AtlasResources.IMAGE_REANIM_ZOMBIE_ZAMBONI_2_DAMAGE2);
				ZamboniDeath(theDamageFlags);
			}
			else if (mBodyHealth <= 0)
			{
				ZamboniDeath(theDamageFlags);
			}
			else if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 1)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_1, AtlasResources.IMAGE_REANIM_ZOMBIE_ZAMBONI_1_DAMAGE1);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_2, AtlasResources.IMAGE_REANIM_ZOMBIE_ZAMBONI_2_DAMAGE1);
			}
			else if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 2)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_1, AtlasResources.IMAGE_REANIM_ZOMBIE_ZAMBONI_1_DAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_2, AtlasResources.IMAGE_REANIM_ZOMBIE_ZAMBONI_2_DAMAGE2);
				AddAttachedParticle(27, 72, ParticleEffect.ZamboniSmoke);
			}
		}
		else if (mZombieType == ZombieType.Catapult)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			if (TodCommon.TestBit(theDamageFlags, 5) || mBodyHealth <= 0)
			{
				reanimation2.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_siding, AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_SIDING_DAMAGE);
				CatapultDeath(theDamageFlags);
			}
			else if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 1)
			{
				reanimation2.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_siding, AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_SIDING_DAMAGE);
			}
			else if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 2)
			{
				AddAttachedParticle(47, 77, ParticleEffect.ZamboniSmoke);
			}
		}
		else if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
		{
			Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
			if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 1)
			{
				reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantua_body1, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_BODY1_2);
				reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantuar_outerarm_lower, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_OUTERARM_LOWER2);
			}
			else if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 2)
			{
				reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantua_body1, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_BODY1_3);
				reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantuar_outerleg_foot, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_FOOT2);
				reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_gargantuar_outerarm_lower, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_OUTERARM_LOWER2);
				if (mZombieType == ZombieType.RedeyeGargantuar)
				{
					reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_HEAD2_REDEYE);
				}
				else
				{
					reanimation3.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_HEAD2);
				}
			}
		}
		else if (mZombieType == ZombieType.Boss)
		{
			if (!TodCommon.TestBit(theDamageFlags, 3))
			{
				mApp.PlayFoley(FoleyType.ShieldHit);
			}
			Reanimation reanimation4 = mApp.ReanimationGet(mBodyReanimID);
			if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 1)
			{
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_head, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_HEAD_DAMAGE1);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_jaw, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_JAW_DAMAGE1);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_hand, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_HAND_DAMAGE1);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_thumb2, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_THUMB_DAMAGE1);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_innerleg_foot, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_FOOT_DAMAGE1);
			}
			else if (bodyDamageIndex != bodyDamageIndex2 && bodyDamageIndex2 == 2)
			{
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_head, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_HEAD_DAMAGE2);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_jaw, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_JAW_DAMAGE2);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_hand, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_HAND_DAMAGE2);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerarm_thumb2, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_OUTERARM_THUMB_DAMAGE2);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_outerleg_foot, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_FOOT_DAMAGE2);
				ApplyBossSmokeParticles(theEnable: true);
			}
			if (num >= mBodyMaxHealth / GameConstants.BOSS_FLASH_HEALTH_FRACTION && mBodyHealth < mBodyMaxHealth / GameConstants.BOSS_FLASH_HEALTH_FRACTION)
			{
				mApp.AddTodParticle(770f, 260f, 400000, ParticleEffect.BossExplosion);
				mApp.PlayFoley(FoleyType.BossExplosionSmall);
				ApplyBossSmokeParticles(theEnable: true);
			}
			if (mBodyHealth <= 0)
			{
				mBodyHealth = 1;
			}
		}
		else if (mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			if (mBodyHealth <= 0)
			{
				mPhaseCounter = 150;
			}
		}
		else
		{
			UpdateDamageStates(theDamageFlags);
		}
		if (mBodyHealth <= 0)
		{
			mBodyHealth = 0;
			PlayDeathAnim(theDamageFlags);
			DropLoot();
		}
	}

	public void AttachShield()
	{
		string theTrackName = string.Empty;
		if (mShieldType == ShieldType.Door)
		{
			ShowDoorArms(theShow: true);
			ReanimShowPrefix("Zombie_outerarm_screendoor", GameConstants.RENDER_GROUP_OVER_SHIELD);
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_screendoor;
		}
		else if (mShieldType == ShieldType.Newspaper)
		{
			ReanimShowPrefix("Zombie_paper_hands", GameConstants.RENDER_GROUP_OVER_SHIELD);
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_zombie_paper_paper;
		}
		else if (mShieldType == ShieldType.Ladder)
		{
			ReanimShowPrefix("Zombie_outerarm", GameConstants.RENDER_GROUP_OVER_SHIELD);
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_zombie_ladder_1;
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		mApp.ReanimationGet(mBodyReanimID).AssignRenderGroupToTrack(theTrackName, GameConstants.RENDER_GROUP_SHIELD);
	}

	public void DetachShield()
	{
		if (mApp.ReanimationTryToGet(mBodyReanimID) == null)
		{
			mShieldType = ShieldType.None;
			mShieldHealth = 0;
			return;
		}
		if (mShieldType == ShieldType.Door)
		{
			ShowDoorArms(theShow: false);
		}
		else if (mShieldType == ShieldType.Newspaper)
		{
			ReanimShowPrefix("Zombie_paper_hands", 0);
		}
		else if (mShieldType == ShieldType.Ladder)
		{
			ReanimShowPrefix("Zombie_outerarm", 0);
			mZombiePhase = ZombiePhase.ZombieNormal;
			if (mIsEating)
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_eat, ReanimLoopType.Loop, 20, 0f);
			}
			else
			{
				StartWalkAnim(0);
			}
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		mShieldType = ShieldType.None;
		mShieldHealth = 0;
		mHasShield = false;
	}

	public void UpdateReanim()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null || reanimation.mDead)
		{
			return;
		}
		bool flag = false;
		if (mZombieType == ZombieType.Catapult)
		{
			flag = true;
			if (GetBodyDamageIndex() == 2 || mZombiePhase == ZombiePhase.ZombieDying)
			{
				reanimation = mApp.ReanimationGet(mBodyReanimID);
				Image currentTrackImage = reanimation.GetCurrentTrackImage(GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_pole);
				if (currentTrackImage == AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_POLE_WITHBALL && mSummonCounter != 0)
				{
					reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_pole, AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_POLE_DAMAGE_WITHBALL);
				}
				else
				{
					reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_pole, AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_POLE_DAMAGE);
				}
			}
			else if (mSummonCounter == 0)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_pole, AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_POLE);
			}
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		float mImageOffsetX = theDrawPos.mImageOffsetX;
		float num = theDrawPos.mImageOffsetY + theDrawPos.mBodyY - 28f;
		mImageOffsetX += 15f;
		num += 20f;
		if ((mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Catapult) && mZombiePhase != ZombiePhase.ZombieBurned)
		{
			if (mZombiePhase == ZombiePhase.ZombieDying)
			{
				float num2 = TodCommon.TodAnimateCurveFloatTime(0.7f, 1f, reanimation.mAnimTime, 0f, 1f, TodCurves.EaseOut);
				mImageOffsetX += TodCommon.RandRangeFloat(0f - num2, num2);
				num += TodCommon.RandRangeFloat(0f - num2, num2);
			}
			else if (mBodyHealth < 200)
			{
				mImageOffsetX += TodCommon.RandRangeFloat(-1f, 1f);
				num += TodCommon.RandRangeFloat(-1f, 1f);
			}
		}
		if (mZombieType == ZombieType.Football && mScaleZombie < 1f)
		{
			num += 20f - mScaleZombie * 20f;
		}
		bool flag2 = false;
		if (IsWalkingBackwards())
		{
			flag2 = true;
		}
		if (mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer)
		{
			bool flag3 = false;
			if ((mZombiePhase == ZombiePhase.DancerDancingIn || mZombiePhase == ZombiePhase.DancerRaiseRight1 || mZombiePhase == ZombiePhase.DancerRaiseRight2) && !mIsEating)
			{
				flag3 = true;
			}
			if (mMindControlled)
			{
				flag3 = !flag3;
			}
			flag2 = flag3;
		}
		if (flag2)
		{
			mImageOffsetX += 90f * mScaleZombie;
		}
		Matrix identity = Matrix.Identity;
		identity.M11 = mScaleZombie;
		identity.M22 = mScaleZombie;
		identity.M41 = (mImageOffsetX + 30f - mScaleZombie * 30f) * Constants.S;
		identity.M42 = (num + 120f - mScaleZombie * 120f) * Constants.S;
		if (flag2)
		{
			identity.M11 = 0f - mScaleZombie;
		}
		if (reanimation.mOverlayMatrix.mMatrix != identity)
		{
			flag = true;
			reanimation.mOverlayMatrix.mMatrix = identity;
		}
		Reanimation reanimation2 = mApp.ReanimationTryToGet(mMoweredReanimID);
		if (reanimation2 != null)
		{
			reanimation2.Update();
			SexyTransform2D theOverlayMatrix = default;
			reanimation2.GetAttachmentOverlayMatrix(0, out theOverlayMatrix);
			theOverlayMatrix.mMatrix.M11 = theOverlayMatrix.mMatrix.M11 * reanimation.mOverlayMatrix.mMatrix.M11;
			theOverlayMatrix.mMatrix.M21 = theOverlayMatrix.mMatrix.M21 * reanimation.mOverlayMatrix.mMatrix.M11;
			theOverlayMatrix.mMatrix.M12 = theOverlayMatrix.mMatrix.M12 * reanimation.mOverlayMatrix.mMatrix.M22;
			theOverlayMatrix.mMatrix.M22 = theOverlayMatrix.mMatrix.M22 * reanimation.mOverlayMatrix.mMatrix.M22;
			theOverlayMatrix.mMatrix.M41 = theOverlayMatrix.mMatrix.M41 * reanimation.mOverlayMatrix.mMatrix.M11;
			theOverlayMatrix.mMatrix.M42 = theOverlayMatrix.mMatrix.M42 * reanimation.mOverlayMatrix.mMatrix.M22;
			theOverlayMatrix.mMatrix.M41 = theOverlayMatrix.mMatrix.M41 + reanimation.mOverlayMatrix.mMatrix.M22;
			theOverlayMatrix.mMatrix.M42 = theOverlayMatrix.mMatrix.M42 + reanimation.mOverlayMatrix.mMatrix.M42;
			if (reanimation.mOverlayMatrix != theOverlayMatrix)
			{
				flag = true;
				reanimation.mOverlayMatrix = theOverlayMatrix;
			}
		}
		reanimation.Update();
		if (mZombieType == ZombieType.Ninja && (mIsEating || mZombiePhase == ZombiePhase.NinjaShownByPlantern))
		{
			float num3 = 2.5f;
			if (mReanimPreview == null)
			{
				mReanimPreview = new MemoryImage();
				mReanimPreview.Create((int)(250f * num3), (int)(250f * num3), num3);
			}
			Graphics graphics = Graphics.GetNew(mReanimPreview);
			graphics.BeginFrame();
			graphics.mTransY += 100;
			float num4 = mScaleZombie;
			Draw(graphics);
			graphics.mTransY -= 100;
			graphics.EndFrame();
			graphics.SetRenderTarget(null);
			graphics.PrepareForReuse();
			mScaleZombie = num4;
		}
		if (flag)
		{
			reanimation.PropogateColorToAttachments();
		}
	}

	public void GetTrackPosition(ref string theTrackName, ref float thePosX, ref float thePosY)
	{
		GetTrackPosition(theTrackName, ref thePosX, ref thePosY);
	}

	public void GetTrackPosition(string theTrackName, ref float thePosX, ref float thePosY)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			thePosX = mPosX;
			thePosY = mPosY;
			return;
		}
		int theTrackIndex = reanimation.FindTrackIndex(theTrackName);
		SexyTransform2D theMatrix = default;
		reanimation.GetTrackMatrix(theTrackIndex, ref theMatrix);
		thePosX = theMatrix.mMatrix.M41 * Constants.IS + mPosX;
		thePosY = theMatrix.mMatrix.M42 * Constants.IS + mPosY;
	}

	public void LoadPlainZombieReanim()
	{
		mZombieAttackRect = new TRect(20, 0, 50, 115);
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			SetupReanimLayers(reanimation, mZombieType);
			if (mBoard != null)
			{
				EnableMustache(mBoard.mMustacheMode);
				EnableFuture(mBoard.mFutureMode);
			}
			LoadWaterZombieReanim();
		}
	}

	public void LoadWaterZombieReanim()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return;
		}
		bool flag = false;
		if (mBoard != null && mBoard.mPlantRow[mRow] == PlantRowType.Pool)
		{
			flag = true;
		}
		else if (mZombieType == ZombieType.DuckyTube)
		{
			flag = true;
		}
		if (mZombieType == ZombieType.JackInTheBox)
		{
			if (flag)
			{
				ReanimShowTrack("Zombie_jackbox_clownhead", -1);
				ReanimShowTrack("Zombie_jackbox_clownneck1", -1);
				ReanimShowTrack("Zombie_jackbox_clownneck2", -1);
				ReanimShowTrack("Zombie_jackbox_clownneck3", -1);
				ReanimShowTrack("Zombie_jackbox_box2", -1);
				ReanimShowTrack("Zombie_jackbox_box", -1);
				ReanimShowTrack("Zombie_jackbox_handle", -1);
				ReanimShowTrack("anim_Zombie_jackbox_innerarm_lower", -1);
				ReanimShowTrack("anim_Zombie_jackbox_innerarm_upper", -1);
				ReanimShowTrack("anim_Zombie_jackbox_duckytube_whole", -1);
				ReanimIgnoreClipRect("zombie_jackbox_outerarm_lower", theIgnoreClipRect: true);
				ReanimIgnoreClipRect("anim_zombie_jackbox_duckytube_inwater", theIgnoreClipRect: true);
				ReanimIgnoreClipRect("zombie_whitewater", theIgnoreClipRect: true);
			}
			else
			{
				ReanimShowPrefix("anim_Zombie_jackbox_duckytube", -1);
				ReanimShowTrack("anim_Zombie_jackbox_handle_water", -1);
				ReanimShowTrack("anim_Zombie_jackbox_innerarm_upper_water", -1);
				ReanimShowTrack("anim_Zombie_jackbox_innerarm_lower_water", -1);
			}
		}
		else if (mZombieType == ZombieType.Propeller)
		{
			if (flag)
			{
				ReanimIgnoreClipRect("zombie_propeller_duckytube_whole", theIgnoreClipRect: true);
				ReanimIgnoreClipRect("zombie_propeller_engine", theIgnoreClipRect: true);
				ReanimIgnoreClipRect("zombie_propeller_engine_whole", theIgnoreClipRect: true);
			}
		}
		else if (flag)
		{
			ReanimShowPrefix("zombie_duckytube", 0);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_duckytube, theIgnoreClipRect: true);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_hand, theIgnoreClipRect: true);
			ReanimIgnoreClipRect(GlobalMembersReanimIds.ReanimTrackId_zombie_innerarm3, theIgnoreClipRect: true);
			SetupWaterTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_whitewater);
			SetupWaterTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_whitewater2);
			if (reanimation.mReanimationType == ReanimationType.ZombieFootball || reanimation.mReanimationType == ReanimationType.ZombieFootballPremium)
			{
				ReanimShowTrack("zombie_duckytube_whole.png", -1);
				ReanimIgnoreClipRect("zombie_duckytube.png", theIgnoreClipRect: true);
				ReanimIgnoreClipRect("zombie_duckytubehead.png", theIgnoreClipRect: true);
			}
		}
		else
		{
			reanimation.AssignRenderGroupToPrefix("Zombie_duckytube", -1);
		}
	}

	public void ShowDoorArms(bool theShow)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			SetupDoorArms(reanimation, theShow);
			if (!mHasArm)
			{
				ReanimShowPrefix("Zombie_outerarm_lower", -1);
				ReanimShowPrefix("Zombie_outerarm_hand", -1);
			}
		}
	}

	public void ReanimShowTrack(ref string theTrackName, int theRenderGroup)
	{
		ReanimShowTrack(theTrackName, theRenderGroup);
	}

	public void ReanimShowTrack(string theTrackName, int theRenderGroup)
	{
		mApp.ReanimationTryToGet(mBodyReanimID)?.AssignRenderGroupToTrack(theTrackName, theRenderGroup);
	}

	public void PlayZombieAppearSound()
	{
		if (mZombieType == ZombieType.DolphinRider)
		{
			mApp.PlayFoley(FoleyType.DolphinAppears);
			return;
		}
		if (mZombieType == ZombieType.Balloon || mZombieType == ZombieType.Propeller)
		{
			mApp.PlayFoley(FoleyType.Ballooninflate);
			return;
		}
		if (mZombieType == ZombieType.Zamboni)
		{
			mApp.PlayFoley(FoleyType.Zamboni);
		}
		if (mZombieType != ZombieType.Ninja)
		{
			return;
		}
		int num = 0;
		foreach (Zombie mZombie in mBoard.mZombies)
		{
			if (mZombie.mZombieType == ZombieType.Ninja && mZombie.mZombieAge < 300)
			{
				num++;
				if (num >= 2)
				{
					break;
				}
			}
		}
		if (num < 2)
		{
			PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Ninja_Appear);
		}
	}

	public void StartMindControlled()
	{
		mApp.PlaySample(Resources.SOUND_MINDCONTROLLED);
		mMindControlled = true;
		mLastPortalX = -1;
		if (mZombieType == ZombieType.Dancer)
		{
			for (int i = 0; i < GameConstants.NUM_BACKUP_DANCERS; i++)
			{
				mFollowerZombieID[i] = null;
			}
		}
		else if (mZombieType == ZombieType.BackupDancer)
		{
			Zombie zombie = mBoard.ZombieTryToGet(mRelatedZombieID);
			if (zombie != null)
			{
				Zombie zombie2 = mBoard.ZombieGetID(this);
				for (int j = 0; j < GameConstants.NUM_BACKUP_DANCERS; j++)
				{
					if (zombie.mFollowerZombieID[j] == zombie2)
					{
						zombie.mFollowerZombieID[j] = null;
						break;
					}
				}
			}
			mRelatedZombieID = null;
		}
		else
		{
			Zombie zombie3 = mBoard.ZombieTryToGet(mRelatedZombieID);
			if (zombie3 != null)
			{
				zombie3.mRelatedZombieID = null;
				mRelatedZombieID = null;
			}
		}
	}

	public bool IsFlying()
	{
		if (mZombiePhase != ZombiePhase.BalloonFlying && mZombiePhase != ZombiePhase.BalloonPopping && mZombiePhase != ZombiePhase.PropellerBlownAway)
		{
			return mZombiePhase == ZombiePhase.PropellerTeeter;
		}
		return true;
	}

	private void SetupReanimForLostHead()
	{
		ReanimShowPrefix("anim_head", -1);
		ReanimShowPrefix("anim_hair", -1);
		ReanimShowPrefix("anim_tongue", -1);
	}

	public void DropHead(uint theDamageFlags)
	{
		if (!CanLoseBodyParts() || !mHasHead)
		{
			return;
		}
		if (mButteredCounter > 0)
		{
			mButteredCounter = 0;
			UpdateAnimSpeed();
		}
		mHasHead = false;
		SetupReanimForLostHead();
		if (mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			ReanimShowPrefix("anim_jaw", -1);
		}
		if (TodCommon.TestBit(theDamageFlags, 4))
		{
			return;
		}
		if (mZombieType == ZombieType.PeaHead || mZombieType == ZombieType.WallnutHead || mZombieType == ZombieType.TallnutHead || mZombieType == ZombieType.JalapenoHead || mZombieType == ZombieType.GatlingHead || mZombieType == ZombieType.SquashHead)
		{
			DetachPlantHead();
			mApp.ReanimationGet(mSpecialHeadReanimID)?.ReanimationDie();
			mSpecialHeadReanimID = null;
		}
		else
		{
			if (mZombieType == ZombieType.Ninja)
			{
				return;
			}
			int aRenderOrder = mRenderOrder + 1;
			ZombieDrawPosition theDrawPos = default;
			GetDrawPos(ref theDrawPos);
			float thePosX = mPosX + theDrawPos.mImageOffsetX + (float)theDrawPos.mHeadX + 11f;
			float thePosY = mPosY + theDrawPos.mImageOffsetY + (float)theDrawPos.mHeadY + theDrawPos.mBodyY + 21f;
			if (mBodyReanimID != null)
			{
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_head1, ref thePosX, ref thePosY);
			}
			ParticleEffect theEffect = ParticleEffect.ZombieHead;
			if (mZombiePhase == ZombiePhase.ZombieMowered)
			{
				theEffect = ParticleEffect.MoweredZombieHead;
				thePosX -= 40f;
				thePosY -= 50f;
			}
			else if (mInPool)
			{
				theEffect = ParticleEffect.ZombieHeadPool;
			}
			if (mZombieType == ZombieType.Dancer)
			{
				aRenderOrder = mRenderOrder - 1;
			}
			if (mZombieType == ZombieType.Newspaper)
			{
				theEffect = ParticleEffect.ZombieNewspaperHead;
			}
			else if (mZombieType == ZombieType.Pogo)
			{
				PogoBreak(theDamageFlags);
				theEffect = ParticleEffect.ZombiePogoHead;
			}
			else if (mZombieType == ZombieType.Balloon)
			{
				ReanimShowPrefix("anim_hat", -1);
				ReanimShowPrefix("hat", -1);
				theEffect = ParticleEffect.ZombieBalloonHead;
			}
			else if (mZombieType == ZombieType.Polevaulter)
			{
				DropPole();
			}
			else if (mZombieType == ZombieType.Flag)
			{
				DropFlag();
			}
			TodParticleSystem todParticleSystem = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, theEffect);
			OverrideParticleColor(todParticleSystem);
			OverrideParticleScale(todParticleSystem);
			if (todParticleSystem != null && mZombieType == ZombieType.Dancer)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_DISCO);
				todParticleSystem.OverrideScale(null, 1.2f);
				ReanimShowPrefix("Zombie_disco_glasses", -1);
				TodParticleSystem todParticleSystem2 = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.ZombieMustache);
				OverrideParticleColor(todParticleSystem2);
				OverrideParticleScale(todParticleSystem2);
				todParticleSystem2?.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_DISCO_GLASSES);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.BackupDancer)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_DANCER_BACKUP_HEAD);
				todParticleSystem.OverrideScale(null, 1.2f);
				ReanimShowPrefix("Zombie_backup_stash", -1);
				todParticleSystem = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.ZombieMustache);
				OverrideParticleColor(todParticleSystem);
				OverrideParticleScale(todParticleSystem);
				todParticleSystem?.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_BACKUP_STASH);
				ReanimShowPrefix("anim_head2", -1);
				todParticleSystem = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.ZombieMustache);
				OverrideParticleColor(todParticleSystem);
				OverrideParticleScale(todParticleSystem);
				todParticleSystem?.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_JAW);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Bobsled)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEBOBSLEDHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Ladder)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIELADDERHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Imp)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEIMPHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Football)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEFOOTBALLHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Polevaulter)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEPOLEVAULTERHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Snorkel)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_SNORKLE_HEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Digger)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEDIGGERHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.DolphinRider)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEDOLPHINRIDERHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Yeti)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEYETIHEAD);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Propeller)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIE_PROPELLER_HEAD);
			}
			else if (todParticleSystem != null && (mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan))
			{
				int num = mBodyMaxHealth;
				int num2 = mBodyHealth;
				ZombieType zombieType = mZombieType;
				Image image;
				if (zombieType != ZombieType.RobotTitan)
				{
					if (zombieType != ZombieType.RedeyeRobotTitan)
					{
						goto IL_0519;
					}
					int num3 = num;
					int num4 = num2;
					if (num4 > num3 * 2 / 3)
					{
						image = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_REDWHOLEHEAD1;
					}
					else
					{
						int num5 = num;
						int num6 = num2;
						if (num6 > num5 * 2 / 3)
						{
							goto IL_0519;
						}
						image = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_REDWHOLEHEAD2;
					}
				}
				else if (num2 > num * 2 / 3)
				{
					image = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD1;
				}
				else
				{
					int num7 = num;
					int num8 = num2;
					if (num8 > num7 * 2 / 3)
					{
						goto IL_0519;
					}
					image = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD2;
				}
				Image theImage = image;
				todParticleSystem.OverrideImage(null, theImage);
				todParticleSystem.OverrideScale(null, 1.2f);
			}
			else if (todParticleSystem != null && mZombieType == ZombieType.Talisman)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_TALISMAN_HEAD);
				todParticleSystem.OverrideScale(null, 0.75f);
				ReanimShowPrefix("anim_beard1", -1);
				ReanimShowPrefix("anim_beard2", -1);
				ReanimShowPrefix("anim_jaw", -1);
				ReanimShowPrefix("anim_eyebraw1", -1);
				ReanimShowPrefix("anim_eyebraw2", -1);
			}
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (mBoard.mMustacheMode && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_zombie_mustache))
			{
				ReanimShowPrefix("Zombie_mustache", -1);
				TodParticleSystem todParticleSystem3 = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.ZombieMustache);
				OverrideParticleColor(todParticleSystem3);
				OverrideParticleScale(todParticleSystem3);
				Image imageOverride = reanimation.GetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_mustache);
				if (todParticleSystem3 != null && imageOverride != null)
				{
					todParticleSystem3.OverrideImage(null, imageOverride);
				}
			}
			if (mBoard.mFutureMode)
			{
				Image imageOverride2 = reanimation.GetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
				int num9 = -1;
				if (imageOverride2 != null && imageOverride2 == AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES1)
				{
					num9 = 0;
				}
				else if (imageOverride2 != null && imageOverride2 == AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES2)
				{
					num9 = 1;
				}
				else if (imageOverride2 != null && imageOverride2 == AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES3)
				{
					num9 = 2;
				}
				else if (imageOverride2 != null && imageOverride2 == AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES4)
				{
					num9 = 3;
				}
				if (num9 != -1)
				{
					TodParticleSystem todParticleSystem4 = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.ZombieFutureGlasses);
					OverrideParticleColor(todParticleSystem4);
					OverrideParticleScale(todParticleSystem4);
					todParticleSystem4?.OverrideFrame(null, num9);
				}
			}
			if (mBoard.mPinataMode && mZombiePhase != ZombiePhase.ZombieMowered)
			{
				TodParticleSystem aParticle = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.Pinata);
				OverrideParticleScale(aParticle);
			}
			mApp.PlayFoley(FoleyType.LimbsPop);
		}
		return;
		IL_0519:
		throw new NotImplementedException();
	}

	public bool CanTargetPlant(Plant thePlant, ZombieAttackType theAttackType)
	{
		if (mApp.IsWallnutBowlingLevel() && theAttackType != ZombieAttackType.Vault)
		{
			return false;
		}
		if (thePlant.NotOnGround())
		{
			return false;
		}
		if (thePlant.mInTalismanCounter > 0 && theAttackType != ZombieAttackType.DriveOver)
		{
			return false;
		}
		if (thePlant.mSeedType == SeedType.Tanglekelp || thePlant.mSeedType == SeedType.ImitaterRandomZombie)
		{
			return false;
		}
		if (!mInPool && mBoard.IsPoolSquare(thePlant.mPlantCol, thePlant.mRow))
		{
			return false;
		}
		if (mZombiePhase == ZombiePhase.DiggerTunneling)
		{
			if (thePlant.mSeedType == SeedType.Potatomine)
			{
				return thePlant.mState == PlantState.Notready;
			}
			return false;
		}
		if (thePlant.IsSpiky())
		{
			if (mZombieType != ZombieType.Gargantuar && mZombieType != ZombieType.RedeyeGargantuar && mZombieType != ZombieType.Zamboni)
			{
				if (!mBoard.IsPoolSquare(thePlant.mPlantCol, thePlant.mRow))
				{
					return mBoard.GetFlowerPotAt(thePlant.mPlantCol, thePlant.mRow) != null;
				}
				return true;
			}
			return true;
		}
		if (theAttackType == ZombieAttackType.DriveOver)
		{
			if (thePlant.mSeedType == SeedType.Cherrybomb || thePlant.mSeedType == SeedType.Jalapeno || thePlant.mSeedType == SeedType.Blover || thePlant.mSeedType == SeedType.Squash || thePlant.mSeedType == SeedType.PickledPepper)
			{
				return thePlant.IsDisabled();
			}
			if (thePlant.mSeedType == SeedType.Doomshroom || thePlant.mSeedType == SeedType.Iceshroom)
			{
				return thePlant.IsDisabled();
			}
		}
		if (mZombiePhase == ZombiePhase.LadderCarrying || mZombiePhase == ZombiePhase.LadderPlacing)
		{
			bool flag = false;
			if (thePlant.mSeedType == SeedType.Wallnut || thePlant.mSeedType == SeedType.Tallnut || thePlant.mSeedType == SeedType.Pumpkinshell)
			{
				flag = true;
			}
			if (mBoard.GetLadderAt(thePlant.mPlantCol, thePlant.mRow) != null)
			{
				flag = false;
			}
			if ((theAttackType == ZombieAttackType.Chew) & flag)
			{
				return false;
			}
			if (theAttackType == ZombieAttackType.Ladder && !flag)
			{
				return false;
			}
		}
		if (theAttackType == ZombieAttackType.Chew)
		{
			Plant topPlantAt = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.EatingOrder);
			if (topPlantAt != thePlant && topPlantAt != null && CanTargetPlant(topPlantAt, theAttackType))
			{
				return false;
			}
		}
		if (theAttackType == ZombieAttackType.Vault)
		{
			Plant topPlantAt2 = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyNormalPosition);
			if (topPlantAt2 != thePlant && topPlantAt2 != null && CanTargetPlant(topPlantAt2, theAttackType))
			{
				return false;
			}
		}
		return true;
	}

	public void UpdateZombieCatapult()
	{
		if (mZombiePhase == ZombiePhase.ZombieNormal)
		{
			if (mPosX <= (float)(650 + Constants.BOARD_EXTRA_ROOM) && FindCatapultTarget() != null && mSummonCounter > 0)
			{
				mZombiePhase = ZombiePhase.CatapultLaunching;
				mPhaseCounter = 300;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_shoot, ReanimLoopType.PlayOnceAndHold, 0, 24f);
			}
		}
		else if (mZombiePhase == ZombiePhase.CatapultLaunching)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.ShouldTriggerTimedEvent(0.545f))
			{
				Plant thePlant = FindCatapultTarget();
				ZombieCatapultFire(thePlant);
			}
			if (reanimation.mLoopCount > 0)
			{
				mSummonCounter--;
				if (mSummonCounter == 4)
				{
					ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_basketball, -1);
				}
				else if (mSummonCounter == 3)
				{
					ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_basketball2, -1);
				}
				else if (mSummonCounter == 2)
				{
					ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_basketball3, -1);
				}
				else if (mSummonCounter == 1)
				{
					ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_catapult_basketball4, -1);
				}
				if (mSummonCounter == 0)
				{
					PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 20, 6f);
					mZombiePhase = ZombiePhase.ZombieNormal;
				}
				else
				{
					PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 12f);
					mZombiePhase = ZombiePhase.CatapultReloading;
				}
			}
		}
		else if (mZombiePhase == ZombiePhase.CatapultReloading && mPhaseCounter == 0)
		{
			Plant plant = FindCatapultTarget();
			if (plant != null)
			{
				mZombiePhase = ZombiePhase.CatapultLaunching;
				mPhaseCounter = 300;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_shoot, ReanimLoopType.PlayOnceAndHold, 20, 24f);
			}
			else
			{
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 20, 6f);
				mZombiePhase = ZombiePhase.ZombieNormal;
			}
		}
	}

	public Plant FindCatapultTarget()
	{
		Plant plant = null;
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant2 = mBoard.mPlants[i];
			if (!plant2.mDead && mRow == plant2.mRow && mX >= plant2.mX + 100 && !plant2.NotOnGround() && !plant2.IsSpiky() && (plant == null || plant2.mPlantCol < plant.mPlantCol))
			{
				plant = mBoard.GetTopPlantAt(plant2.mPlantCol, plant2.mRow, TopPlant.CatapultOrder);
			}
		}
		return plant;
	}

	public void ZombieCatapultFire(Plant thePlant)
	{
		float num = mPosX + 113f;
		float num2 = mPosY - 44f;
		int num3 = thePlant?.mX ?? ((int)mPosX - 300);
		int num4 = thePlant?.mY ?? 0;
		mApp.PlayFoley(FoleyType.Basketball);
		Projectile projectile = mBoard.AddProjectile((int)num, (int)num2, mRenderOrder, mRow, ProjectileType.Basketball);
		float num5 = num - (float)num3 - 20f;
		float num6 = (float)num4 - num2;
		if (num5 < 40f)
		{
			num5 = 40f;
		}
		projectile.mMotionType = ProjectileMotion.Lobbed;
		float num7 = 120f;
		projectile.mVelX = (0f - num5) / num7;
		projectile.mVelY = 0f;
		projectile.mVelZ = -7f + num6 / num7;
		projectile.mAccZ = 0.115f;
	}

	public void UpdateClimbingLadder()
	{
		float num = mAltitude;
		if (mOnHighGround)
		{
			num -= (float)Constants.HIGH_GROUND_HEIGHT;
		}
		int theGridX = mBoard.PixelToGridXKeepOnBoard((int)((float)(mX + 5) + num * 0.5f), mY);
		if (mBoard.GetLadderAt(theGridX, mRow) == null)
		{
			mZombieHeight = ZombieHeight.Falling;
			return;
		}
		mAltitude += 0.8f;
		if (mVelX < 0.5f)
		{
			mPosX -= 0.5f;
		}
		float num2 = 90f;
		if (mOnHighGround)
		{
			num2 += (float)Constants.HIGH_GROUND_HEIGHT;
		}
		if (mAltitude >= num2)
		{
			mZombieHeight = ZombieHeight.Falling;
		}
	}

	public void UpdateZombieGargantuar()
	{
		if (mZombiePhase == ZombiePhase.GargantuarSmashing)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.ShouldTriggerTimedEvent(0.64f))
			{
				Plant plant = FindPlantTarget(ZombieAttackType.Chew);
				if (plant != null && plant.mSeedType == SeedType.Spikerock)
				{
					TakeDamage(20, 32u);
					plant.SpikeRockTakeDamage();
					if (plant.mPlantHealth <= 0)
					{
						SquishAllInSquare(plant.mPlantCol, plant.mRow, ZombieAttackType.Chew);
					}
				}
				else if (plant != null)
				{
					SquishAllInSquare(plant.mPlantCol, plant.mRow, ZombieAttackType.Chew);
				}
				if (mApp.IsScaryPotterLevel())
				{
					int theGridX = mBoard.PixelToGridX((int)mPosX, (int)mPosY);
					GridItem scaryPotAt = mBoard.GetScaryPotAt(theGridX, mRow);
					if (scaryPotAt != null)
					{
						mBoard.mChallenge.ScaryPotterOpenPot(scaryPotAt);
					}
				}
				if (mApp.IsIZombieLevel())
				{
					GridItem gridItem = mBoard.mChallenge.IZombieGetBrainTarget(this);
					if (gridItem != null)
					{
						mBoard.mChallenge.IZombieSquishBrain(gridItem);
					}
				}
				mApp.PlayFoley(FoleyType.Thump);
				mApp.Vibrate();
				mBoard.ShakeBoard(0, 3);
			}
			if (reanimation.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.ZombieNormal;
				StartWalkAnim(20);
			}
			return;
		}
		float num = mPosX - 360f - (float)Constants.BOARD_EXTRA_ROOM;
		if (mZombiePhase == ZombiePhase.GargantuarThrowing)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation2.ShouldTriggerTimedEvent(0.74f))
			{
				mHasObject = false;
				ReanimShowPrefix("Zombie_imp", -1);
				ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_gargantuar_whiterope, -1);
				mApp.PlayFoley(FoleyType.Swing);
				Zombie zombie = mBoard.AddZombie(ZombieType.Imp, mFromWave);
				if (zombie == null)
				{
					return;
				}
				float num2 = 40f;
				if (mBoard.StageHasRoof())
				{
					num -= 180f;
					num2 = -140f;
				}
				if (num < num2)
				{
					num = num2;
				}
				else if (num > 140f)
				{
					num -= TodCommon.RandRangeFloat(0f, 100f);
				}
				zombie.mPosX = mPosX - 133f;
				zombie.mPosY = GetPosYBasedOnRow(mRow);
				zombie.SetRow(mRow);
				zombie.mVariant = false;
				zombie.mRenderOrder = mRenderOrder + 1;
				zombie.mZombiePhase = ZombiePhase.ImpGettingThrown;
				zombie.mAltitude = 88f;
				zombie.mVelX = 3f;
				zombie.mChilledCounter = mChilledCounter;
				zombie.mVelZ = 0.5f * (num / zombie.mVelX) * GameConstants.THOWN_ZOMBIE_GRAVITY;
				zombie.PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_thrown, ReanimLoopType.PlayOnceAndHold, 0, 18f);
				zombie.UpdateReanim();
				mApp.PlayFoley(FoleyType.Imp);
			}
			if (reanimation2.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.ZombieNormal;
				StartWalkAnim(20);
			}
		}
		else
		{
			if (IsImmobilizied() || !mHasHead)
			{
				return;
			}
			if (mHasObject && mBodyHealth < mBodyMaxHealth / 2 && num > 40f)
			{
				mZombiePhase = ZombiePhase.GargantuarThrowing;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_throw, ReanimLoopType.PlayOnceAndHold, 20, 24f);
				return;
			}
			bool flag = false;
			Plant plant = FindPlantTarget(ZombieAttackType.Chew);
			if (plant != null)
			{
				flag = true;
			}
			else if (mApp.IsScaryPotterLevel())
			{
				int theGridX2 = mBoard.PixelToGridX((int)mPosX, (int)mPosY);
				if (mBoard.GetScaryPotAt(theGridX2, mRow) != null)
				{
					flag = true;
				}
			}
			else if (mApp.IsIZombieLevel() && mBoard.mChallenge.IZombieGetBrainTarget(this) != null)
			{
				flag = true;
			}
			if (flag)
			{
				mZombiePhase = ZombiePhase.GargantuarSmashing;
				mApp.PlayFoley(FoleyType.Lowgroan);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_smash, ReanimLoopType.PlayOnceAndHold, 20, 16f);
			}
		}
	}

	public int GetBodyDamageIndex()
	{
		if (mZombieType == ZombieType.Boss)
		{
			if (mBodyHealth < mBodyMaxHealth / 2)
			{
				return 2;
			}
			if (mBodyHealth < mBodyMaxHealth * 4 / 5)
			{
				return 1;
			}
			return 0;
		}
		if (mBodyHealth < mBodyMaxHealth / 3)
		{
			return 2;
		}
		if (mBodyHealth < mBodyMaxHealth * 2 / 3)
		{
			return 1;
		}
		return 0;
	}

	public void ApplyBurn()
	{
		if (mDead || mZombiePhase == ZombiePhase.ZombieBurned)
		{
			return;
		}
		if (mBodyHealth >= 1800 || mZombieType == ZombieType.Boss || mHelmType == HelmType.Bell || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			TakeDamage(1800, 18u);
			return;
		}
		if (mHelmType == HelmType.FootballPremium && mHelmHealth >= 1800)
		{
			TakeDamage(1800, 18u);
			return;
		}
		if (mZombieType == ZombieType.SquashHead && !mHasHead)
		{
			mApp.RemoveReanimation(ref mSpecialHeadReanimID);
			mSpecialHeadReanimID = null;
		}
		if (mIceTrapCounter > 0)
		{
			RemoveIceTrap();
		}
		if (mButteredCounter > 0)
		{
			mButteredCounter = 0;
		}
		GlobalMembersAttachment.AttachmentDetachCrossFadeParticleType(ref mAttachmentID, ParticleEffect.ZamboniSmoke, null);
		BungeeDropPlant();
		if (mZombiePhase == ZombiePhase.ZombieDying || mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.ImpGettingThrown || mZombiePhase == ZombiePhase.RisingFromGrave || mZombiePhase == ZombiePhase.DancerRising || mZombiePhase == ZombiePhase.DolphinIntoPool || mZombiePhase == ZombiePhase.DolphinInJump || mZombiePhase == ZombiePhase.DolphinRiding || mZombiePhase == ZombiePhase.SnorkelIntoPool || mZombiePhase == ZombiePhase.DiggerTunneling || mZombiePhase == ZombiePhase.DiggerTunnelingPauseWithoutAxe || mZombiePhase == ZombiePhase.DiggerRising || mZombiePhase == ZombiePhase.DiggerRiseWithoutAxe || mZombiePhase == ZombiePhase.ZombieMowered || mInPool)
		{
			DieWithLoot();
		}
		else if (mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Yeti || mZombieType == ZombieType.PeaHead || mZombieType == ZombieType.WallnutHead || mZombieType == ZombieType.JalapenoHead || mZombieType == ZombieType.GatlingHead || mZombieType == ZombieType.SquashHead || mZombieType == ZombieType.TallnutHead || IsBobsledTeamWithSled() || IsFlying() || !mHasHead || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			SetAnimRate(0f);
			Reanimation reanimation = mApp.ReanimationTryToGet(mSpecialHeadReanimID);
			if (reanimation != null)
			{
				reanimation.mAnimRate = 0f;
				reanimation.mColorOverride = Color.Black;
				reanimation.mEnableExtraAdditiveDraw = false;
				reanimation.mEnableExtraOverlayDraw = false;
			}
			mZombiePhase = ZombiePhase.ZombieBurned;
			mPhaseCounter = 300;
			mJustGotShotCounter = 0;
			DropLoot();
			if (mZombieType == ZombieType.Bungee)
			{
				mZombieFade = 50;
			}
			if (mZombieType == ZombieType.Balloon)
			{
				Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
				ReanimatorTrackInstance trackInstanceByName = reanimation2.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_hat);
				Reanimation reanimation3 = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName.mAttachmentID);
				if (reanimation3 != null)
				{
					reanimation3.mAnimRate = 0f;
				}
				mZombieFade = 50;
			}
		}
		else
		{
			ReanimationType theReanimationType = ReanimationType.ZombieCharred;
			float num = mPosX + 22f;
			float num2 = mPosY - 10f;
			if (mZombieType == ZombieType.Balloon)
			{
				num2 += 31f;
			}
			if (mZombieType == ZombieType.Imp)
			{
				num -= 6f;
				theReanimationType = ReanimationType.ZombieCharredImp;
			}
			if (mZombieType == ZombieType.Digger)
			{
				if (IsWalkingBackwards())
				{
					num += 14f;
				}
				theReanimationType = ReanimationType.ZombieCharredDigger;
			}
			if (mZombieType == ZombieType.Zamboni)
			{
				num += 61f;
				num2 += -16f;
				theReanimationType = ReanimationType.ZombieCharredZamboni;
			}
			if (mZombieType == ZombieType.Catapult)
			{
				num += -36f;
				num2 += -20f;
				theReanimationType = ReanimationType.ZombieCharredCatapult;
			}
			if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
			{
				num += -15f;
				num2 += -10f;
				theReanimationType = ReanimationType.ZombieCharredGargantuar;
			}
			Reanimation reanimation4 = mApp.AddReanimation(num, num2, mRenderOrder, theReanimationType);
			reanimation4.mAnimRate *= TodCommon.RandRangeFloat(0.9f, 1.1f);
			if (mZombiePhase == ZombiePhase.DiggerWalkingWithoutAxe)
			{
				reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_crumble_noaxe);
			}
			else if (mZombieType == ZombieType.Digger)
			{
				reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_crumble);
			}
			else if ((mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar) && !mHasObject)
			{
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_impblink, AtlasResources.IMAGE_BLANK);
				reanimation4.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_imphead, AtlasResources.IMAGE_BLANK);
			}
			if (mScaleZombie != 1f)
			{
				reanimation4.mOverlayMatrix.mMatrix.M11 = mScaleZombie;
				reanimation4.mOverlayMatrix.mMatrix.M22 = mScaleZombie;
				Reanimation reanimation5 = reanimation4;
				reanimation5.mOverlayMatrix.mMatrix.M41 = reanimation5.mOverlayMatrix.mMatrix.M41 + (20f - mScaleZombie * 20f) * Constants.S;
				Reanimation reanimation6 = reanimation4;
				reanimation6.mOverlayMatrix.mMatrix.M42 = reanimation6.mOverlayMatrix.mMatrix.M42 + (120f - mScaleZombie * 120f) * Constants.S;
				reanimation4.OverrideScale(mScaleZombie, mScaleZombie);
			}
			if (IsWalkingBackwards())
			{
				reanimation4.OverrideScale(0f - mScaleZombie, mScaleZombie);
				Reanimation reanimation7 = reanimation4;
				reanimation7.mOverlayMatrix.mMatrix.M41 = reanimation7.mOverlayMatrix.mMatrix.M41 + 60f * mScaleZombie * Constants.S;
			}
			DieWithLoot();
		}
		if (mZombieType == ZombieType.Bobsled)
		{
			BobsledBurn();
		}
	}

	public void UpdateBurn()
	{
		mPhaseCounter--;
		if (mPhaseCounter == 0)
		{
			DieWithLoot();
		}
	}

	public bool ZombieNotWalking()
	{
		if (mIsEating || IsImmobilizied())
		{
			return true;
		}
		if (mZombiePhase == ZombiePhase.JackInTheBoxPopping || mZombiePhase == ZombiePhase.NewspaperMaddening || mZombiePhase == ZombiePhase.GargantuarThrowing || mZombiePhase == ZombiePhase.GargantuarSmashing || mZombiePhase == ZombiePhase.CatapultLaunching || mZombiePhase == ZombiePhase.CatapultReloading || mZombiePhase == ZombiePhase.DiggerRising || mZombiePhase == ZombiePhase.DiggerTunnelingPauseWithoutAxe || mZombiePhase == ZombiePhase.DiggerRiseWithoutAxe || mZombiePhase == ZombiePhase.DiggerStunned || mZombiePhase == ZombiePhase.DancerSnappingFingers || mZombiePhase == ZombiePhase.DancerSnappingFingersWithLight || mZombiePhase == ZombiePhase.DancerSnappingFingersHold || mZombiePhase == ZombiePhase.DancerRising || mZombiePhase == ZombiePhase.ImpGettingThrown || mZombiePhase == ZombiePhase.ImpLanding || mZombiePhase == ZombiePhase.LadderPlacing || mZombieHeight == ZombieHeight.InToChimney || mZombieHeight == ZombieHeight.GettingBungeeDropped || mZombieHeight == ZombieHeight.Zombiquarium || mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Boss)
		{
			return true;
		}
		if (mZombiePhase == ZombiePhase.DancerRaiseLeft1 || mZombiePhase == ZombiePhase.DancerWalkToRaise || mZombiePhase == ZombiePhase.DancerRaiseRight1 || mZombiePhase == ZombiePhase.DancerRaiseLeft2 || mZombiePhase == ZombiePhase.DancerRaiseRight2 || mZombiePhase == ZombiePhase.CornRoulette || mZombiePhase == ZombiePhase.TalismanLeaving)
		{
			return true;
		}
		if (mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer)
		{
			Zombie zombie = ((mZombieType != ZombieType.Dancer) ? mBoard.ZombieTryToGet(mRelatedZombieID) : this);
			if (zombie == null)
			{
				return false;
			}
			if (zombie.IsImmobilizied() || zombie.mIsEating)
			{
				return true;
			}
			for (int i = 0; i < GameConstants.NUM_BACKUP_DANCERS; i++)
			{
				Zombie zombie2 = mBoard.ZombieTryToGet(zombie.mFollowerZombieID[i]);
				if (zombie2 != null && (zombie2.IsImmobilizied() || zombie2.mIsEating))
				{
					return true;
				}
			}
		}
		return false;
	}

	public Zombie FindZombieTarget()
	{
		if (mZombiePhase == ZombiePhase.DiggerTunneling)
		{
			return null;
		}
		TRect zombieAttackRect = GetZombieAttackRect();
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && mMindControlled != zombie.mMindControlled && !zombie.IsFlying() && zombie.mZombiePhase != ZombiePhase.DiggerTunneling && zombie.mZombiePhase != ZombiePhase.BungeeDiving && zombie.mZombiePhase != ZombiePhase.BungeeDivingScreaming && zombie.mZombiePhase != ZombiePhase.BungeeRising && zombie.mZombieHeight != ZombieHeight.GettingBungeeDropped && !zombie.IsDeadOrDying() && zombie.mRow == mRow)
			{
				TRect zombieRect = zombie.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(zombieAttackRect, zombieRect);
				if (rectOverlap >= 20 || (rectOverlap >= 0 && zombie.mIsEating))
				{
					return zombie;
				}
			}
		}
		return null;
	}

	public void PlayZombieReanim(ref string theTrackName, ReanimLoopType theLoopType, byte theBlendTime, float theAnimRate)
	{
		lastPlayedReanimName = theTrackName;
		lastPlayedReanimLoopType = theLoopType;
		lastPlayedReanimBlendTime = theBlendTime;
		lastPlayedReanimAnimRate = theAnimRate;
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			reanimation.PlayReanim(theTrackName, theLoopType, theBlendTime, theAnimRate);
			if (theAnimRate != 0f)
			{
				mOrginalAnimRate = theAnimRate;
			}
			UpdateAnimSpeed();
		}
	}

	public void UpdateZombieBackupDancer()
	{
		if (mIsEating)
		{
			return;
		}
		if (mZombiePhase == ZombiePhase.DancerRising)
		{
			mAltitude = TodCommon.TodAnimateCurve(150, 0, mPhaseCounter, Constants.ZOMBIE_BACKUP_DANCER_RISE_HEIGHT, 0, TodCurves.Linear);
			mUsesClipping = mAltitude < 0f;
			if (mPhaseCounter != 0)
			{
				return;
			}
			if (IsOnHighGround())
			{
				mAltitude = Constants.HIGH_GROUND_HEIGHT;
			}
		}
		ZombiePhase dancerPhase = GetDancerPhase();
		if (dancerPhase != mZombiePhase)
		{
			switch (dancerPhase)
			{
			case ZombiePhase.DancerDancingLeft:
				mZombiePhase = dancerPhase;
				PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, 10, 0f);
				break;
			case ZombiePhase.DancerWalkToRaise:
			{
				mZombiePhase = dancerPhase;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_armraise, ReanimLoopType.Loop, 10, 18f);
				Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
				reanimation.mAnimTime = 0.6f;
				break;
			}
			case ZombiePhase.DancerRaiseLeft1:
			case ZombiePhase.DancerRaiseRight1:
			case ZombiePhase.DancerRaiseLeft2:
			case ZombiePhase.DancerRaiseRight2:
				mZombiePhase = dancerPhase;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_armraise, ReanimLoopType.Loop, 10, 18f);
				break;
			}
		}
	}

	public ZombiePhase GetDancerPhase()
	{
		int dancerFrame = GetDancerFrame();
		if (dancerFrame <= 11)
		{
			return ZombiePhase.DancerDancingLeft;
		}
		if (dancerFrame <= 12)
		{
			return ZombiePhase.DancerWalkToRaise;
		}
		if (dancerFrame <= 15)
		{
			return ZombiePhase.DancerRaiseRight1;
		}
		if (dancerFrame <= 18)
		{
			return ZombiePhase.DancerRaiseLeft1;
		}
		if (dancerFrame <= 21)
		{
			return ZombiePhase.DancerRaiseRight2;
		}
		return ZombiePhase.DancerRaiseLeft2;
	}

	public bool IsMovingAtChilledSpeed()
	{
		if (mChilledCounter > 0)
		{
			return true;
		}
		if (mZombieType == ZombieType.Dancer || mZombieType == ZombieType.BackupDancer)
		{
			Zombie zombie = ((mZombieType != ZombieType.Dancer) ? mBoard.ZombieTryToGet(mRelatedZombieID) : this);
			if (zombie == null)
			{
				return false;
			}
			if (zombie.mChilledCounter > 0)
			{
				return true;
			}
			for (int i = 0; i < GameConstants.NUM_BACKUP_DANCERS; i++)
			{
				Zombie zombie2 = mBoard.ZombieTryToGet(zombie.mFollowerZombieID[i]);
				if (zombie2 != null && zombie2.mChilledCounter > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void StartWalkAnim(byte theBlendTime)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return;
		}
		PickRandomSpeed();
		if (mZombiePhase == ZombiePhase.LadderCarrying)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_ladderwalk, ReanimLoopType.Loop, theBlendTime, 0f);
			return;
		}
		if (mZombiePhase == ZombiePhase.NewspaperMad)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_walk_nopaper, ReanimLoopType.Loop, theBlendTime, 0f);
			return;
		}
		if (mInPool && mZombieHeight != ZombieHeight.InToPool && mZombieHeight != ZombieHeight.OutOfPool && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_swim))
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_swim, ReanimLoopType.Loop, theBlendTime, 0f);
			return;
		}
		if ((mZombieType == ZombieType.Normal || mZombieType == ZombieType.TrafficCone || mZombieType == ZombieType.Pail) && mBoard.mDanceMode)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_dance, ReanimLoopType.Loop, theBlendTime, 0f);
			return;
		}
		int num = RandomNumbers.NextNumber(2);
		if (IsZombotany())
		{
			num = 0;
		}
		if (mZombieType == ZombieType.Flag)
		{
			num = 0;
		}
		if (num == 0 && reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_walk2))
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_walk2, ReanimLoopType.Loop, theBlendTime, 0f);
		}
		else if (reanimation.TrackExists(Reanimation.ReanimTrackId_anim_walk))
		{
			PlayZombieReanim(ref Reanimation.ReanimTrackId_anim_walk, ReanimLoopType.Loop, theBlendTime, 0f);
		}
	}

	public Reanimation AddAttachedReanim(int thePosX, int thePosY, ReanimationType theReanimType)
	{
		if (mDead)
		{
			return null;
		}
		Reanimation reanimation = mApp.AddReanimation(mX + thePosX, mY + thePosY, 0, theReanimType);
		if (reanimation != null)
		{
			GlobalMembersAttachment.AttachReanim(ref mAttachmentID, reanimation, (float)thePosX * Constants.S, (float)thePosY * Constants.S);
		}
		return reanimation;
	}

	public void DragUnder()
	{
		mZombieHeight = ZombieHeight.DraggedUnder;
		StopEating();
		ReanimReenableClipping();
	}

	public static void SetupDoorArms(Reanimation aReanim, bool theShow)
	{
		int theRenderGroup = 0;
		int theRenderGroup2 = -1;
		if (theShow)
		{
			theRenderGroup = -1;
			theRenderGroup2 = 0;
		}
		aReanim.AssignRenderGroupToPrefix("Zombie_outerarm_hand", theRenderGroup);
		aReanim.AssignRenderGroupToPrefix("Zombie_outerarm_lower", theRenderGroup);
		aReanim.AssignRenderGroupToPrefix("Zombie_outerarm_upper", theRenderGroup);
		aReanim.AssignRenderGroupToPrefix("anim_innerarm", theRenderGroup);
		aReanim.AssignRenderGroupToPrefix("Zombie_outerarm_screendoor", theRenderGroup2);
		aReanim.AssignRenderGroupToPrefix("Zombie_innerarm_screendoor", theRenderGroup2);
		aReanim.AssignRenderGroupToPrefix("Zombie_innerarm_screendoor_hand", theRenderGroup2);
	}

	public static void SetupReanimLayers(Reanimation aReanim, ZombieType theZombieType)
	{
		aReanim.AssignRenderGroupToPrefix("anim_cone", -1);
		aReanim.AssignRenderGroupToPrefix("anim_bucket", -1);
		aReanim.AssignRenderGroupToPrefix("anim_screendoor", -1);
		aReanim.AssignRenderGroupToPrefix("Zombie_flaghand", -1);
		aReanim.AssignRenderGroupToPrefix("Zombie_duckytube", -1);
		aReanim.AssignRenderGroupToPrefix("anim_tongue", -1);
		aReanim.AssignRenderGroupToPrefix("Zombie_mustache", -1);
		SetupDoorArms(aReanim, theShow: false);
		switch (theZombieType)
		{
		case ZombieType.TrafficCone:
			aReanim.AssignRenderGroupToPrefix("anim_cone", 0);
			aReanim.AssignRenderGroupToPrefix("anim_hair", -1);
			break;
		case ZombieType.Pail:
			aReanim.AssignRenderGroupToPrefix("anim_bucket", 0);
			aReanim.AssignRenderGroupToPrefix("anim_hair", -1);
			break;
		case ZombieType.Door:
			SetupDoorArms(aReanim, theShow: true);
			break;
		case ZombieType.Newspaper:
			aReanim.AssignRenderGroupToPrefix("Zombie_paper_paper", -1);
			break;
		case ZombieType.Flag:
			aReanim.AssignRenderGroupToPrefix("anim_innerarm", -1);
			aReanim.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_zombie_flaghand, 0);
			aReanim.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_zombie_innerarm_screendoor, 0);
			break;
		case ZombieType.DuckyTube:
			aReanim.AssignRenderGroupToPrefix("Zombie_duckytube", 0);
			break;
		}
	}

	public bool IsOnBoard()
	{
		if (mFromWave == GameConstants.ZOMBIE_WAVE_CUTSCENE || mFromWave == GameConstants.ZOMBIE_WAVE_UI)
		{
			return false;
		}
		Debug.ASSERT(mBoard != null);
		return true;
	}

	public void DrawButter(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
		float thePosX = mPosX + theDrawPos.mImageOffsetX + (float)theDrawPos.mHeadX + 11f;
		float thePosY = mPosY + theDrawPos.mImageOffsetY + (float)theDrawPos.mHeadY + theDrawPos.mBodyY + 21f;
		float num = 1f;
		if (mBodyReanimID != null)
		{
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_head1, ref thePosX, ref thePosY);
		}
		thePosX += 0f - mPosX - 29f;
		thePosY += 0f - mPosY - 36f;
		TPoint tPoint = new TPoint(-15, -15);
		thePosX += (float)tPoint.x;
		thePosY += (float)tPoint.y;
		switch (mZombieType)
		{
		case ZombieType.SquashHead:
			thePosX += 6f;
			thePosY -= 9f;
			break;
		case ZombieType.WallnutHead:
			thePosX -= 10f;
			if (mInPool && mIsEating)
			{
				thePosX -= 5f;
				thePosY += 10f;
			}
			break;
		case ZombieType.TallnutHead:
			thePosX -= 30f;
			thePosY -= 30f;
			if (mInPool && mIsEating)
			{
				thePosY += 10f;
			}
			break;
		case ZombieType.GatlingHead:
			thePosY -= 10f;
			break;
		}
		TodCommon.TodDrawImageScaledF(g, Resources.IMAGE_REANIM_CORNPULT_BUTTER_SPLAT, thePosX * Constants.S + 0f, thePosY * Constants.S - 6f, num, num);
	}

	public bool IsImmobilizied()
	{
		if (mIceTrapCounter <= 0)
		{
			return mButteredCounter > 0;
		}
		return true;
	}

	public void ApplyButter()
	{
		if (!mHasHead || !CanBeFrozen() || mZombieType == ZombieType.Zamboni || mZombieType == ZombieType.Boss || IsTangleKelpTarget() || IsBobsledTeamWithSled() || IsFlying())
		{
			return;
		}
		mButteredCounter = 400;
		Zombie zombie = mBoard.ZombieTryToGet(mRelatedZombieID);
		if (zombie != null)
		{
			zombie.mRelatedZombieID = null;
			mRelatedZombieID = null;
		}
		switch (mZombieType)
		{
		case ZombieType.Pogo:
			mAltitude = 0f;
			if (mOnHighGround)
			{
				mAltitude += Constants.HIGH_GROUND_HEIGHT;
			}
			break;
		case ZombieType.Balloon:
			BalloonPropellerHatSpin(theSpinning: false);
			break;
		case ZombieType.PeaHead:
		case ZombieType.WallnutHead:
		case ZombieType.JalapenoHead:
		case ZombieType.GatlingHead:
		case ZombieType.SquashHead:
		case ZombieType.TallnutHead:
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mSpecialHeadReanimID);
			if (reanimation != null)
			{
				reanimation.mAnimRate = 0f;
			}
			break;
		}
		}
		UpdateAnimSpeed();
		StopZombieSound();
	}

	public void ApplyMindControl()
	{
		if (!mMindControlled && !IsDeadOrDying() && mZombieType != ZombieType.Boss && mZombieType != ZombieType.RobotTitan && mZombieType != ZombieType.RedeyeRobotTitan && mZombieType != ZombieType.Gargantuar && mZombieType != ZombieType.RedeyeGargantuar && mZombieType != ZombieType.Zamboni && mZombieType != ZombieType.Catapult && mZombieType != ZombieType.Bungee && !IsBobsledTeamWithSled())
		{
			StartMindControlled();
			mApp.AddTodParticle(mPosX + 60f, mPosY + 40f, mRenderOrder + 1, ParticleEffect.MindControl);
			TrySpawnLevelAward();
			mAnimTicksPerFrame = 18;
			UpdateAnimSpeed();
		}
	}

	public float ZombieTargetLeadX(float theTime)
	{
		float num = mVelX;
		if (mChilledCounter > 0)
		{
			num *= GameConstants.CHILLED_SPEED_FACTOR;
		}
		if (IsWalkingBackwards())
		{
			num = 0f - num;
		}
		if (ZombieNotWalking())
		{
			num = 0f;
		}
		float num2 = num * theTime;
		TRect zombieRect = GetZombieRect();
		int num3 = zombieRect.mX + zombieRect.mWidth / 2;
		return (float)num3 - num2;
	}

	public void UpdateZombieImp()
	{
		if (mZombiePhase == ZombiePhase.ImpGettingThrown)
		{
			mVelZ -= GameConstants.THOWN_ZOMBIE_GRAVITY;
			mAltitude += mVelZ;
			mPosX -= mVelX;
			float num = GetPosYBasedOnRow(mRow) - mPosY;
			mPosY += num;
			mAltitude += num;
			if (mAltitude <= 0f)
			{
				mAltitude = 0f;
				mZombiePhase = ZombiePhase.ImpLanding;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_land, ReanimLoopType.PlayOnceAndHold, 0, 24f);
			}
		}
		else if (mZombiePhase == ZombiePhase.ImpLanding)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.ZombieNormal;
				StartWalkAnim(0);
			}
		}
	}

	public void SquishAllInSquare(int theX, int theY, ZombieAttackType theAttackType)
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieSquishAllInSquare(this, theX, theY, theAttackType);
			return;
		}
		SquishAllInSquareCore(theX, theY, theAttackType);
	}

	public void SquishAllInSquareCore(int theX, int theY, ZombieAttackType theAttackType)
	{
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && theY == plant.mRow && theX == plant.mPlantCol && (theAttackType != ZombieAttackType.DriveOver || !plant.IsSpiky()) && plant.mSeedType != SeedType.Spikerock)
			{
				mBoard.mPlantsEaten++;
				plant.Squish();
			}
		}
	}

	public void RemoveIceTrap()
	{
		mIceTrapCounter = 0;
		if (mZombieType == ZombieType.Balloon)
		{
			BalloonPropellerHatSpin(theSpinning: true);
		}
		UpdateAnimSpeed();
		StartZombieSound();
	}

	public bool IsBouncingPogo()
	{
		if (mZombiePhase >= ZombiePhase.PogoBouncing)
		{
			return mZombiePhase <= ZombiePhase.PogoForwardBounce7;
		}
		return false;
	}

	public int GetBobsledPosition()
	{
		if (mZombieType != ZombieType.Bobsled)
		{
			return -1;
		}
		if (mRelatedZombieID == null && mFollowerZombieID[0] == null)
		{
			return -1;
		}
		if (mRelatedZombieID == null)
		{
			return 0;
		}
		Zombie zombie = mBoard.ZombieGetID(this);
		Zombie zombie2 = mBoard.ZombieGet(mRelatedZombieID);
		for (int i = 0; i < 3; i++)
		{
			if (zombie2.mFollowerZombieID[i] == zombie)
			{
				return i + 1;
			}
		}
		Debug.ASSERT(value: false);
		return -666;
	}

	public void DrawBobsledReanim(Graphics g, ref ZombieDrawPosition theDrawPos, bool theBeforeZombie)
	{
		int bobsledPosition = GetBobsledPosition();
		bool flag = false;
		bool flag2 = false;
		Zombie zombie;
		if (mFromWave == GameConstants.ZOMBIE_WAVE_CUTSCENE)
		{
			zombie = this;
		}
		else
		{
			switch (bobsledPosition)
			{
			case -1:
				return;
			case 0:
				zombie = this;
				break;
			default:
				zombie = mBoard.ZombieGet(mRelatedZombieID);
				break;
			}
		}
		if (mFromWave == GameConstants.ZOMBIE_WAVE_CUTSCENE)
		{
			if (theBeforeZombie)
			{
				flag2 = true;
			}
			else
			{
				flag = true;
			}
		}
		else if (mZombiePhase == ZombiePhase.BobsledCrashing)
		{
			if (bobsledPosition == 0 && !theBeforeZombie)
			{
				flag = true;
				flag2 = true;
			}
		}
		else if (mZombiePhase == ZombiePhase.BobsledSliding || mZombiePhase == ZombiePhase.ZombieBurned)
		{
			if ((bobsledPosition == 2) & theBeforeZombie)
			{
				flag = true;
				flag2 = true;
			}
		}
		else if (mZombiePhase == ZombiePhase.BobsledBoarding)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mAnimTime < 0.5f)
			{
				if ((bobsledPosition == 2) & theBeforeZombie)
				{
					flag = true;
					flag2 = true;
				}
			}
			else if (bobsledPosition == 0 && !theBeforeZombie)
			{
				flag = true;
			}
			else if ((bobsledPosition == 3) & theBeforeZombie)
			{
				flag2 = true;
			}
		}
		int num = 255;
		float num2 = theDrawPos.mImageOffsetX + zombie.mPosX - mPosX - 76f;
		float num3 = 15f;
		int num4;
		if (mZombiePhase == ZombiePhase.BobsledCrashing)
		{
			num4 = 3;
			num = TodCommon.TodAnimateCurve(30, 0, mPhaseCounter, 255, 0, TodCurves.Linear);
			num2 += (float)(GameConstants.BOBSLED_CRASH_TIME - mPhaseCounter) * mVelX / (float)GameConstants.ZOMBIE_LIMP_SPEED_FACTOR;
			num2 -= TodCommon.TodAnimateCurveFloat(GameConstants.BOBSLED_CRASH_TIME, 0, mPhaseCounter, 0f, 50f, TodCurves.EaseOut);
			num3 += TodCommon.TodAnimateCurveFloat(GameConstants.BOBSLED_CRASH_TIME, 75, mPhaseCounter, 5f, 10f, TodCurves.Linear);
		}
		else
		{
			num4 = zombie.GetHelmDamageIndex();
		}
		if (num != 255)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, num));
		}
		Image theImage = num4 switch
		{
			0 => AtlasResources.IMAGE_ZOMBIE_BOBSLED1, 
			1 => AtlasResources.IMAGE_ZOMBIE_BOBSLED2, 
			2 => AtlasResources.IMAGE_ZOMBIE_BOBSLED3, 
			_ => AtlasResources.IMAGE_ZOMBIE_BOBSLED4, 
		};
		if (mZombiePhase == ZombiePhase.ZombieBurned)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(SexyColor.Black);
		}
		num2 *= Constants.S;
		num3 *= Constants.S;
		if (flag2 && num4 != 3)
		{
			g.DrawImageF(AtlasResources.IMAGE_ZOMBIE_BOBSLED_INSIDE, num2, num3);
		}
		if (flag)
		{
			g.DrawImageF(theImage, num2, num3);
		}
		if (zombie.mJustGotShotCounter > 0)
		{
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			g.SetColorizeImages(colorizeImages: true);
			int num5 = zombie.mJustGotShotCounter * 10;
			g.SetColor(new SexyColor(num5, num5, num5, 255));
			if (flag2 && num4 != 3)
			{
				g.DrawImageF(AtlasResources.IMAGE_ZOMBIE_BOBSLED_INSIDE, num2, num3);
			}
			if (flag)
			{
				g.DrawImageF(theImage, num2, num3);
			}
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public void BobsledDie()
	{
		if (!IsBobsledTeamWithSled() || !IsOnBoard())
		{
			return;
		}
		Zombie zombie = ((mRelatedZombieID != null) ? mBoard.ZombieGet(mRelatedZombieID) : this);
		if (!zombie.mDead)
		{
			zombie.DieNoLoot(giveAchievements: true);
		}
		for (int i = 0; i < 3; i++)
		{
			Zombie zombie2 = mBoard.ZombieGet(zombie.mFollowerZombieID[i]);
			if (!zombie2.mDead)
			{
				zombie2.DieNoLoot(giveAchievements: true);
			}
		}
	}

	public void BobsledBurn()
	{
		if (IsBobsledTeamWithSled())
		{
			Zombie zombie = ((mRelatedZombieID != null) ? mBoard.ZombieGet(mRelatedZombieID) : this);
			zombie.ApplyBurn();
			for (int i = 0; i < 3; i++)
			{
				Zombie zombie2 = mBoard.ZombieGet(zombie.mFollowerZombieID[i]);
				zombie2.ApplyBurn();
			}
		}
	}

	public bool IsBobsledTeamWithSled()
	{
		return GetBobsledPosition() != -1;
	}

	public bool CanBeFrozen()
	{
		if (CanBeChilled() && mZombiePhase != ZombiePhase.PolevaulterInVault && mZombiePhase != ZombiePhase.DolphinIntoPool && mZombiePhase != ZombiePhase.DolphinInJump && mZombiePhase != ZombiePhase.SnorkelIntoPool && !IsFlying() && mZombiePhase != ZombiePhase.ImpGettingThrown && mZombiePhase != ZombiePhase.ImpLanding && mZombiePhase != ZombiePhase.BobsledCrashing && mZombiePhase != ZombiePhase.JackInTheBoxPopping && mZombiePhase != ZombiePhase.SquashRising && mZombiePhase != ZombiePhase.SquashFalling && mZombiePhase != ZombiePhase.SquashDoneFalling && !IsBouncingPogo() && (mZombieType != ZombieType.Bungee || mZombiePhase == ZombiePhase.BungeeAtBottom))
		{
			return mZombiePhase != ZombiePhase.TalismanLeaving;
		}
		return false;
	}

	public bool CanBeChilled()
	{
		if (mZombieType != ZombieType.Zamboni && !IsBobsledTeamWithSled() && !IsDeadOrDying() && mZombiePhase != ZombiePhase.CornRoulette && mZombiePhase != ZombiePhase.DiggerTunneling && mZombiePhase != ZombiePhase.DiggerRising && mZombiePhase != ZombiePhase.DiggerTunnelingPauseWithoutAxe && mZombiePhase != ZombiePhase.DiggerRiseWithoutAxe && mZombiePhase != ZombiePhase.RisingFromGrave && mZombiePhase != ZombiePhase.DancerRising && !mMindControlled && (mZombieType != ZombieType.Boss || mZombiePhase == ZombiePhase.BossHeadIdleBeforeSpit || mZombiePhase == ZombiePhase.BossHeadIdleAfterSpit || mZombiePhase == ZombiePhase.BossHeadSpit) && mZombieType != ZombieType.RobotTitan && mZombieType != ZombieType.RedeyeRobotTitan)
		{
			if (mZombieType == ZombieType.Monk)
			{
				return mHelmType != HelmType.Bell;
			}
			return true;
		}
		return false;
	}

	public void UpdateZombieSnorkel()
	{
		bool flag = IsWalkingBackwards();
		if (mZombiePhase == ZombiePhase.SnorkelWalking && !flag)
		{
			if (mX > 700 && mX <= 720)
			{
				mZombiePhase = ZombiePhase.SnorkelIntoPool;
				mVelX = 0.2f;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_jumpinpool, ReanimLoopType.PlayOnceAndHold, 20, 16f);
			}
		}
		else if (mZombiePhase == ZombiePhase.SnorkelIntoPool)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			mAltitude = TodCommon.TodAnimateCurveFloat(0, 1000, (int)(reanimation.mAnimTime * 1000f), 0f, 10f, TodCurves.Linear);
			if (reanimation.ShouldTriggerTimedEvent(0.83f))
			{
				Reanimation reanimation2 = mApp.AddReanimation(mX - 47, mY + 73, mRenderOrder + 1, ReanimationType.Splash);
				reanimation2.OverrideScale(1.2f, 0.8f);
				mApp.AddTodParticle(mX - 10, mY + 115, mRenderOrder + 1, ParticleEffect.PlantingPool);
				mApp.PlayFoley(FoleyType.ZombieEnteringWater);
			}
			if (reanimation.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.SnorkelWalkingInPool;
				mInPool = true;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_swim, ReanimLoopType.LoopFullLastFrame, 0, 12f);
			}
		}
		else if (mZombiePhase == ZombiePhase.SnorkelWalkingInPool)
		{
			if (!mHasHead)
			{
				TakeDamage(1800, 9u);
			}
			else if (mX <= 25 && !flag)
			{
				mZombieHeight = ZombieHeight.OutOfPool;
				mZombiePhase = ZombiePhase.SnorkelWalking;
				mAltitude = -90f;
				mPosX -= 15f;
				PoolSplash(theInToPoolSound: false);
				StartWalkAnim(0);
			}
			else if (((float)mX > 640f) & flag)
			{
				mZombieHeight = ZombieHeight.OutOfPool;
				mZombiePhase = ZombiePhase.SnorkelWalking;
				mAltitude = -90f;
				mPosX += 15f;
				PoolSplash(theInToPoolSound: false);
				StartWalkAnim(0);
			}
			else if (mIsEating)
			{
				mZombiePhase = ZombiePhase.SnorkelUpToEat;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_uptoeat, ReanimLoopType.PlayOnceAndHold, 0, 24f);
			}
		}
		else if (mZombiePhase == ZombiePhase.SnorkelUpToEat)
		{
			Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
			if (!mIsEating)
			{
				mZombiePhase = ZombiePhase.SnorkelDownFromEat;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_uptoeat, ReanimLoopType.PlayOnceAndHold, 0, -24f);
			}
			else if (reanimation3.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.SnorkelEatingInPool;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_eat, ReanimLoopType.Loop, 0, 0f);
			}
		}
		else if (mZombiePhase == ZombiePhase.SnorkelEatingInPool)
		{
			if (!mIsEating)
			{
				mZombiePhase = ZombiePhase.SnorkelDownFromEat;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_uptoeat, ReanimLoopType.PlayOnceAndHold, 0, -24f);
			}
		}
		else if (mZombiePhase == ZombiePhase.SnorkelDownFromEat)
		{
			Reanimation reanimation4 = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation4.mLoopCount > 0)
			{
				mZombiePhase = ZombiePhase.SnorkelWalkingInPool;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_swim, ReanimLoopType.LoopFullLastFrame, 0, 0f);
				PickRandomSpeed();
			}
		}
		mUsesClipping = mZombiePhase == ZombiePhase.SnorkelWalking || mZombiePhase == ZombiePhase.ZombieDying || mZombiePhase == ZombiePhase.SnorkelIntoPool;
	}

	public void ReanimIgnoreClipRect(string theTrackName, bool theIgnoreClipRect)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return;
		}
		for (int i = 0; i < reanimation.mDefinition.mTrackCount; i++)
		{
			if (reanimation.mDefinition.mTracks[i].mName == theTrackName)
			{
				reanimation.mTrackInstances[i].mIgnoreClipRect = theIgnoreClipRect;
			}
		}
		mUsesClipping = !theIgnoreClipRect;
	}

	public void SetAnimRate(float theAnimRate)
	{
		mOrginalAnimRate = theAnimRate;
		ApplyAnimRate(theAnimRate);
	}

	public void ApplyAnimRate(float theAnimRate)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			reanimation.mAnimRate = (IsMovingAtChilledSpeed() ? (theAnimRate * 0.5f) : theAnimRate);
		}
	}

	public bool IsDeadOrDying()
	{
		if (!mDead && mZombiePhase != ZombiePhase.ZombieDying && mZombiePhase != ZombiePhase.ZombieBurned)
		{
			return mZombiePhase == ZombiePhase.ZombieMowered;
		}
		return true;
	}

	public void DrawDancerReanim(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
		SexyColor sexyColor = default;
		if (mZombiePhase != ZombiePhase.DancerDancingIn && mZombiePhase != ZombiePhase.DancerSnappingFingers && mZombiePhase != ZombiePhase.ZombieNormal && mZombiePhase != ZombiePhase.ZombieDying && mApp.mGameScene != GameScenes.ZombiesWon)
		{
			switch ((mZombieAge >= 700) ? (mZombieAge / 100 * 7 % 5) : 0)
			{
			case 0:
				sexyColor = new SexyColor(250, 250, 160);
				break;
			case 1:
				sexyColor = new SexyColor(114, 234, 170);
				break;
			case 2:
				sexyColor = new SexyColor(216, 126, 202);
				break;
			case 3:
				sexyColor = new SexyColor(90, 110, 140);
				break;
			case 4:
				sexyColor = new SexyColor(240, 90, 130);
				break;
			}
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(sexyColor);
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_SPOTLIGHT2, (float)Constants.Zombie_Dancer_Spotlight_Offset.X * Constants.S, (float)Constants.Zombie_Dancer_Spotlight_Offset.Y * Constants.S, Constants.Zombie_Dancer_Spotlight_Scale, Constants.Zombie_Dancer_Spotlight_Scale);
			g.SetColorizeImages(colorizeImages: false);
			mApp.ReanimationTryToGet(mBodyReanimID).DrawRenderGroup(g, 0);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(sexyColor);
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_SPOTLIGHT, ((float)Constants.Zombie_Dancer_Spotlight_Pos.X + (float)Constants.Zombie_Dancer_Spotlight_Offset.X) * Constants.S, (float)(Constants.Zombie_Dancer_Spotlight_Pos.Y + Constants.Zombie_Dancer_Spotlight_Offset.Y) * Constants.S, Constants.Zombie_Dancer_Spotlight_Scale, Constants.Zombie_Dancer_Spotlight_Scale);
			g.SetColorizeImages(colorizeImages: false);
		}
		else
		{
			mApp.ReanimationTryToGet(mBodyReanimID).DrawRenderGroup(g, 0);
		}
	}

	public void DrawBungeeReanim(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		float num = Constants.InvertAndScaleGame(-22f);
		float num2 = Constants.InvertAndScaleGame(14f) + theDrawPos.mBodyY + theDrawPos.mImageOffsetY;
		DrawBungeeCord(g, (int)num, (int)num2);
		reanimation.DrawRenderGroup(g, 0);
		Zombie zombie = null;
		int num3 = -1;
		if (mBoard != null)
		{
			num3 = mBoard.mZombies.IndexOf(mRelatedZombieID);
		}
		if (num3 != -1)
		{
			zombie = mBoard.mZombies[num3];
		}
		if (zombie != null)
		{
			Graphics graphics = Graphics.GetNew(g);
			graphics.mTransY += (int)((0f - mAltitude) * Constants.S);
			graphics.mTransX += (int)((zombie.mPosX - mPosX) * Constants.S);
			ZombieDrawPosition theDrawPos2 = default;
			zombie.GetDrawPos(ref theDrawPos2);
			zombie.DrawReanim(graphics, ref theDrawPos2, 0);
			graphics.PrepareForReuse();
		}
		else
		{
			Plant plant = null;
			int num4 = -1;
			if (mBoard != null)
			{
				num4 = mBoard.mPlants.IndexOf(mTargetPlantID);
			}
			if (num4 != -1)
			{
				plant = mBoard.mPlants[num4];
			}
			if (plant != null)
			{
				Graphics graphics2 = Graphics.GetNew(g);
				graphics2.mTransY += (int)((30f - mAltitude) * Constants.S);
				if (mZombiePhase == ZombiePhase.BungeeRising && (plant.mSeedType == SeedType.Spikeweed || plant.mSeedType == SeedType.Spikerock))
				{
					graphics2.mTransY -= 34;
				}
				if (plant.mPlantCol <= 4 && mBoard.StageHasRoof())
				{
					graphics2.mTransY += 10;
				}
				plant.Draw(graphics2);
				graphics2.PrepareForReuse();
			}
		}
		reanimation.DrawRenderGroup(g, GameConstants.RENDER_GROUP_ARMS);
	}

	public void DrawBungeeTarget(Graphics g)
	{
		if (IsOnBoard() && !mApp.IsFinalBossLevel() && mZombiePhase != ZombiePhase.BungeeHitOuchy && mZombiePhase != ZombiePhase.BungeeRising && mRelatedZombieID == null)
		{
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
			ZombieDrawPosition theDrawPos = default;
			GetDrawPos(ref theDrawPos);
			float num = mX + Constants.Zombie_Bungee_Target_Offset.X;
			float num2 = (float)(mY + Constants.Zombie_Bungee_Target_Offset.Y) + theDrawPos.mBodyY + theDrawPos.mImageOffsetY;
			if (mZombiePhase == ZombiePhase.BungeeDiving || mZombiePhase == ZombiePhase.BungeeDivingScreaming)
			{
				num += TodCommon.TodAnimateCurveFloat(GameConstants.BUNGEE_ZOMBIE_HEIGHT, GameConstants.BUNGEE_ZOMBIE_HEIGHT - 400, (int)mAltitude, 30f, 0f, TodCurves.Linear);
				num2 += TodCommon.TodAnimateCurveFloat(GameConstants.BUNGEE_ZOMBIE_HEIGHT, GameConstants.BUNGEE_ZOMBIE_HEIGHT - 400, (int)mAltitude, -600f, 0f, TodCurves.Linear);
			}
			num2 += mAltitude;
			g.DrawImageF(AtlasResources.IMAGE_BUNGEETARGET, num * Constants.S, num2 * Constants.S);
		}
	}

	public void BungeeDie()
	{
		BungeeDropPlant();
		Plant plant = null;
		int num = -1;
		if (mBoard != null)
		{
			num = mBoard.mPlants.IndexOf(mTargetPlantID);
		}
		if (num != -1)
		{
			plant = mBoard.mPlants[num];
		}
		if (plant != null)
		{
			mBoard.mPlantsEaten++;
			plant.Die();
		}
		Zombie zombie = null;
		if (mBoard != null)
		{
			zombie = mBoard.ZombieTryToGet(mRelatedZombieID);
		}
		if (zombie != null && !zombie.mDead)
		{
			zombie.DieNoLoot(giveAchievements: true);
		}
	}

	public void ZamboniDeath(uint theDamageFlags)
	{
		if (!TodCommon.TestBit(theDamageFlags, 5))
		{
			mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.ZamboniExplosion);
			DieWithLoot();
			mApp.PlayFoley(FoleyType.Explosion);
			return;
		}
		mFlatTires = true;
		mApp.PlayFoley(FoleyType.TirePop);
		mZombiePhase = ZombiePhase.ZombieDying;
		mApp.AddTodParticle(mPosX + 29f, mPosY + 114f, mRenderOrder + 1, ParticleEffect.ZamboniTire);
		mVelX = 0f;
		if (RandomNumbers.NextNumber(4) == 0 && mPosX < 600f + (float)Constants.BOARD_EXTRA_ROOM)
		{
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_wheelie2, ReanimLoopType.PlayOnceAndHold, 10, 10f);
			mPhaseCounter = 280;
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		TodParticleSystem theParticleSystem = mApp.AddTodParticle(0f, 0f, 0, ParticleEffect.ZamboniSmoke);
		if (theParticleSystem != null)
		{
			reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_zombie_zamboni_1, ref theParticleSystem, 35f, 85f);
		}
		mPhaseCounter = 280;
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_wheelie1, ReanimLoopType.PlayOnceAndHold, 10, 12f);
	}

	public void CatapultDeath(uint theDamageFlags)
	{
		if (TodCommon.TestBit(theDamageFlags, 5))
		{
			mApp.PlayFoley(FoleyType.TirePop);
			mZombiePhase = ZombiePhase.ZombieDying;
			mApp.AddTodParticle(mPosX + 29f, mPosY + 114f, mRenderOrder + 1, ParticleEffect.ZamboniTire);
			mVelX = 0f;
			AddAttachedParticle(47, 77, ParticleEffect.ZamboniSmoke);
			mPhaseCounter = 280;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_bounce, ReanimLoopType.PlayOnceAndHold, 10, 12f);
		}
		else
		{
			mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.CatapultExplosion);
			DieWithLoot();
			mApp.PlayFoley(FoleyType.Explosion);
		}
	}

	public bool SetupDrawZombieWon(Graphics g)
	{
		if (mFromWave != GameConstants.ZOMBIE_WAVE_WINNER)
		{
			return true;
		}
		if (!mBoard.mCutScene.ShowZombieWalking())
		{
			return false;
		}
		switch (mBoard.mBackground)
		{
		case BackgroundType.Num1Day:
		case BackgroundType.Num2Night:
			g.ClipRect((int)((float)(-123 - mX) * Constants.S) + Constants.Zombie_GameOver_ClipOffset_1, (int)((float)(-mY) * Constants.S), 800, 600);
			break;
		case BackgroundType.Num3Pool:
		case BackgroundType.Num4Fog:
			g.ClipRect((int)((float)(-172 - mX) * Constants.S) + Constants.Zombie_GameOver_ClipOffset_2, (int)((float)(-mY) * Constants.S), 800, 600);
			break;
		case BackgroundType.Num5Roof:
		case BackgroundType.Num6Boss:
			g.ClipRect((int)((float)(-95 - mX) * Constants.S) + Constants.Zombie_GameOver_ClipOffset_3, (int)((float)(-mY) * Constants.S), 800, 600);
			break;
		}
		return true;
	}

	public void WalkIntoHouse()
	{
		WinningZombieReachedDesiredY = false;
		GlobalMembersAttachment.AttachmentDetachCrossFadeParticleType(ref mAttachmentID, ParticleEffect.ZamboniSmoke, null);
		mFromWave = GameConstants.ZOMBIE_WAVE_WINNER;
		if (mZombiePhase == ZombiePhase.PolevaulterPreVault)
		{
			mZombiePhase = ZombiePhase.PolevaulterPostVault;
			StartWalkAnim(0);
		}
		if (mBoard.mBackground == BackgroundType.Num1Day || mBoard.mBackground == BackgroundType.Num2Night || mBoard.mBackground == BackgroundType.Num3Pool || mBoard.mBackground == BackgroundType.Num4Fog || mBoard.mBackground == BackgroundType.Num5Roof || mBoard.mBackground == BackgroundType.Num6Boss)
		{
			mRenderOrder = Board.MakeRenderOrder(RenderLayer.Zombie, 2, 100);
			for (int i = 0; i < mBoard.mLawnMowers.Count; i++)
			{
				mBoard.mLawnMowers[i].PrepareForReuse();
			}
			mBoard.mLawnMowers.Clear();
			if (mZombiePhase == ZombiePhase.PolevaulterPreVault)
			{
				mPosX += 35f;
			}
			if (mBoard.mBackground == BackgroundType.Num3Pool || mBoard.mBackground == BackgroundType.Num4Fog)
			{
				ZombieType zombieType = mZombieType;
			}
		}
	}

	public void UpdateZamboni()
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieUpdateZamboni(this);
			return;
		}
		UpdateZamboniCore();
	}

	public void UpdateZamboniCore()
	{
		float num = mPosX - (float)Constants.BOARD_EXTRA_ROOM;
		if (num > 400f && !mFlatTires)
		{
			mVelX = TodCommon.TodAnimateCurveFloat(700, 300, (int)num, 0.25f, 0.05f, TodCurves.Linear);
		}
		else if (mFlatTires && mVelX > 0.0005f)
		{
			mVelX -= 0.0005f;
		}
		int val = (int)num + 118;
		val = ((!mBoard.StageHasRoof()) ? Math.Max(val, 25) : Math.Max(val, 500));
		if (val < mBoard.mIceMinX[mRow])
		{
			mBoard.mIceMinX[mRow] = val;
		}
		if (val < 800)
		{
			mBoard.mIceTimer[mRow] = 3000;
			if (mApp.mGameMode == GameMode.ChallengeBobsledBonanza || mApp.mGameMode == GameMode.ChallengeBobsledBonanza2)
			{
				mBoard.mIceTimer[mRow] = int.MaxValue;
			}
		}
	}

	public void UpdateZombieChimney()
	{
	}

	public void UpdateLadder()
	{
		if (mMindControlled || !mHasHead || IsDeadOrDying())
		{
			return;
		}
		if (mZombiePhase == ZombiePhase.LadderCarrying && mZombieHeight == ZombieHeight.ZombieNormal)
		{
			Plant plant = FindPlantTarget(ZombieAttackType.Ladder);
			if (plant != null)
			{
				StopEating();
				mZombiePhase = ZombiePhase.LadderPlacing;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_placeladder, ReanimLoopType.PlayOnceAndHold, 10, 24f);
			}
		}
		else
		{
			if (mZombiePhase != ZombiePhase.LadderPlacing)
			{
				return;
			}
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				Plant plant2 = FindPlantTarget(ZombieAttackType.Ladder);
				if (plant2 != null)
				{
					mBoard.AddALadder(plant2.mPlantCol, plant2.mRow);
					mApp.PlaySample(Resources.SOUND_LADDER_ZOMBIE);
					mZombieHeight = ZombieHeight.UpLadder;
					mUseLadderCol = plant2.mPlantCol;
					DetachShield();
				}
				else
				{
					mZombiePhase = ZombiePhase.LadderCarrying;
					StartWalkAnim(0);
				}
			}
		}
	}

	private void SetupReanimForLostArm(uint theDamageFlags)
	{
		switch (mZombieType)
		{
		case ZombieType.Football:
		case ZombieType.FootballPremium:
			ReanimShowPrefix("Zombie_football_leftarm_lower", -1);
			ReanimShowPrefix("Zombie_football_leftarm_hand", -1);
			break;
		case ZombieType.Newspaper:
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_paper_hands, -1);
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_paper_leftarm_lower, -1);
			break;
		case ZombieType.Polevaulter:
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_polevaulter_outerarm_lower, -1);
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_hand, -1);
			break;
		case ZombieType.Dancer:
			ReanimShowPrefix("Zombie_disco_outerarm_lower", -1);
			ReanimShowPrefix("Zombie_disco_outerhand_point", -1);
			ReanimShowPrefix("Zombie_disco_outerhand", -1);
			ReanimShowPrefix("Zombie_disco_outerarm_upper", -1);
			break;
		case ZombieType.BackupDancer:
			ReanimShowPrefix("Zombie_disco_outerarm_lower", -1);
			ReanimShowPrefix("Zombie_disco_outerhand", -1);
			break;
		default:
			ReanimShowPrefix("Zombie_outerarm_lower", -1);
			ReanimShowPrefix("Zombie_outerarm_hand", -1);
			break;
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		float thePosX = mPosX + theDrawPos.mImageOffsetX + 45f;
		float thePosY = mPosY + theDrawPos.mImageOffsetY + theDrawPos.mBodyY + 78f;
		if (IsWalkingBackwards())
		{
			thePosX += 36f;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			switch (mZombieType)
			{
			case ZombieType.Football:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_football_leftarm_hand, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_football_leftarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_LEFTARM_UPPER2);
				break;
			case ZombieType.Newspaper:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_paper_leftarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_paper_leftarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_PAPER_LEFTARM_UPPER2);
				break;
			case ZombieType.Polevaulter:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_polevaulter_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_polevaulter_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_POLEVAULTER_OUTERARM_UPPER2);
				break;
			case ZombieType.Balloon:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_BALLOON_OUTERARM_UPPER2);
				break;
			case ZombieType.Imp:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_imp_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_IMP_ARM1_BONE);
				break;
			case ZombieType.Digger:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_digger_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_DIGGER_OUTERARM_UPPER2);
				break;
			case ZombieType.Bobsled:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_dolphinrider_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_BOBSLED_OUTERARM_UPPER2);
				break;
			case ZombieType.JackInTheBox:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_jackbox_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_jackbox_outerarm_lower, AtlasResources.IMAGE_REANIM_ZOMBIE_JACKBOX_OUTERARM_LOWER2);
				break;
			case ZombieType.Snorkel:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_snorkle_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_SNORKLE_OUTERARM_UPPER2);
				break;
			case ZombieType.DolphinRider:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_dolphinrider_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_DOLPHINRIDER_OUTERARM_UPPER2);
				break;
			case ZombieType.Pogo:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_POGO_OUTERARM_UPPER2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_pogo_stickhands, AtlasResources.IMAGE_REANIM_ZOMBIE_POGO_STICKHANDS2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_pogo_stick, AtlasResources.IMAGE_REANIM_ZOMBIE_POGO_STICKDAMAGE2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_pogo_stick2, AtlasResources.IMAGE_REANIM_ZOMBIE_POGO_STICK2DAMAGE2);
				break;
			case ZombieType.Flag:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_OUTERARM_UPPER2);
				mApp.ReanimationTryToGet(mSpecialHeadReanimID)?.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_flag, AtlasResources.IMAGE_REANIM_ZOMBIE_FLAG3);
				break;
			case ZombieType.Dancer:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_disco_outerhand, ref thePosX, ref thePosY);
				ReanimShowPrefix("Zombie_disco_outerarm_upper_bone", 0);
				break;
			case ZombieType.BackupDancer:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_disco_outerhand, ref thePosX, ref thePosY);
				ReanimShowPrefix("Zombie_disco_outerarm_upper_bone", 0);
				break;
			case ZombieType.Ladder:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_hand, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_ladder_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_OUTERARM_UPPER2);
				break;
			case ZombieType.Yeti:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_hand, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_yeti_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_YETI_OUTERARM_UPPER2);
				break;
			case ZombieType.Propeller:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride("zombie_propeller_outerarm_upper", AtlasResources.IMAGE_REANIM_ZOMBIE_PROPELLER_OUTERARM_UPPER2);
				break;
			default:
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_lower, ref thePosX, ref thePosY);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_outerarm_upper, AtlasResources.IMAGE_REANIM_ZOMBIE_OUTERARM_UPPER2);
				break;
			}
		}
		if (mInPool || TodCommon.TestBit(theDamageFlags, 4))
		{
			return;
		}
		ParticleEffect theEffect = ParticleEffect.ZombieArm;
		if (mZombiePhase == ZombiePhase.ZombieMowered)
		{
			theEffect = ParticleEffect.MoweredZombieArm;
		}
		if (mZombiePhase == ZombiePhase.ZombieMowered)
		{
			thePosX -= 40f;
			thePosY -= 50f;
		}
		TodParticleSystem todParticleSystem = mApp.AddTodParticle(thePosX, thePosY, mRenderOrder + 1, theEffect);
		OverrideParticleColor(todParticleSystem);
		OverrideParticleScale(todParticleSystem);
		if (todParticleSystem != null)
		{
			switch (mZombieType)
			{
			case ZombieType.Football:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_LEFTARM_HAND);
				break;
			case ZombieType.FootballPremium:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_LEFTARM_HAND);
				break;
			case ZombieType.Newspaper:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_PAPER_LEFTARM_LOWER);
				break;
			case ZombieType.Dancer:
				ReanimShowPrefix("Zombie_disco_outerarm_lower", -1);
				ReanimShowPrefix("Zombie_disco_outerhand_point", -1);
				ReanimShowPrefix("Zombie_disco_outerhand", -1);
				ReanimShowPrefix("Zombie_disco_outerarm_upper", -1);
				break;
			case ZombieType.BackupDancer:
				ReanimShowPrefix("Zombie_disco_outerarm_lower", -1);
				ReanimShowPrefix("Zombie_disco_outerhand", -1);
				break;
			case ZombieType.Bobsled:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_BOBSLED_OUTERARM_HAND);
				break;
			case ZombieType.Imp:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_IMP_ARM2);
				break;
			case ZombieType.Yeti:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_YETI_OUTERARM_HAND);
				break;
			case ZombieType.JackInTheBox:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEJACKBOXARM);
				break;
			case ZombieType.Digger:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIEDIGGERARM);
				break;
			case ZombieType.Propeller:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_ZOMBIE_PROPELLER_ARM);
				break;
			case ZombieType.Polevaulter:
			case ZombieType.DolphinRider:
			case ZombieType.Balloon:
			case ZombieType.Pogo:
			case ZombieType.Ladder:
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_OUTERARM_HAND);
				break;
			}
		}
	}

	public void DropArm(uint theDamageFlags)
	{
		if (CanLoseBodyParts() && mShieldType != ShieldType.Door && mShieldType != ShieldType.Newspaper && mZombiePhase != ZombiePhase.SnorkelIntoPool && mZombiePhase != ZombiePhase.DolphinWalking && mZombiePhase != ZombiePhase.DolphinIntoPool && mZombiePhase != ZombiePhase.DolphinRiding && mZombiePhase != ZombiePhase.DolphinInJump && mZombiePhase != ZombiePhase.NewspaperReading && mHasArm)
		{
			mHasArm = false;
			SetupReanimForLostArm(theDamageFlags);
			mApp.PlayFoley(FoleyType.LimbsPop);
		}
	}

	public bool CanLoseBodyParts()
	{
		if (mZombieType != ZombieType.Zamboni && mZombieType != ZombieType.Bungee && mZombieType != ZombieType.Catapult && mZombieType != ZombieType.Gargantuar && mZombieType != ZombieType.RedeyeGargantuar && mZombieType != ZombieType.Boss && mZombieHeight != ZombieHeight.Zombiquarium && !IsFlying())
		{
			return !IsBobsledTeamWithSled();
		}
		return false;
	}

	public void DropHelm(uint theDamageFlags)
	{
		if (mHelmType == HelmType.None)
		{
			return;
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		float thePosX = mPosX + theDrawPos.mImageOffsetX + (float)theDrawPos.mHeadX + 14f;
		float thePosY = mPosY + theDrawPos.mImageOffsetY + (float)theDrawPos.mHeadY + theDrawPos.mBodyY + 18f;
		ParticleEffect particleEffect = ParticleEffect.None;
		bool flag = false;
		if (mHelmType == HelmType.TrafficCone)
		{
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_cone, ref thePosX, ref thePosY);
			ReanimShowPrefix("anim_cone", -1);
			ReanimShowPrefix("anim_hair", 0);
			particleEffect = ParticleEffect.ZombieTrafficCone;
		}
		else if (mHelmType == HelmType.Pail)
		{
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_bucket, ref thePosX, ref thePosY);
			ReanimShowPrefix("anim_bucket", -1);
			ReanimShowPrefix("anim_hair", 0);
			particleEffect = ParticleEffect.ZombiePail;
		}
		else if (mHelmType == HelmType.Football)
		{
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, ref thePosX, ref thePosY);
			ReanimShowPrefix("zombie_football_helmet", -1);
			ReanimShowPrefix("anim_hair", 0);
			particleEffect = ParticleEffect.ZombieHelmet;
		}
		else if (mHelmType == HelmType.FootballPremium)
		{
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, ref thePosX, ref thePosY);
			ReanimShowPrefix("zombie_football_helmet", -1);
			ReanimShowPrefix("anim_hair", 0);
			particleEffect = ParticleEffect.ZombieHelmet;
		}
		else if (mHelmType == HelmType.Digger)
		{
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_digger_hardhat, ref thePosX, ref thePosY);
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_digger_hardhat, -1);
			particleEffect = ParticleEffect.ZombieHeadlight;
		}
		else if (mHelmType == HelmType.Bobsled && !TodCommon.TestBit(theDamageFlags, 4))
		{
			BobsledCrash();
		}
		else if (mHelmType == HelmType.Bell)
		{
			mApp.ReanimationTryToGet(mBodyReanimID)?.AssignRenderGroupToTrack("special", -1);
			particleEffect = ParticleEffect.ZombieMonkBell;
		}
		else if (mHelmType == HelmType.RobotTitanHead || mHelmType == HelmType.RedeyeRobotTitanHead)
		{
			particleEffect = ParticleEffect.ZombieHead;
			flag = true;
			SetupReanimForLostHead();
			ReanimShowPrefix("anim_jaw", -1);
		}
		if ((!TodCommon.TestBit(theDamageFlags, 4) | flag) && particleEffect != ParticleEffect.None)
		{
			TodParticleSystem todParticleSystem = mApp.AddTodParticle(thePosX, thePosY, mRenderOrder + 1, particleEffect);
			OverrideParticleScale(todParticleSystem);
			if (mHelmType == HelmType.FootballPremium)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_HELMET3);
			}
			else if (mHelmType == HelmType.RobotTitanHead)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD2);
			}
			else if (mHelmType == HelmType.RedeyeRobotTitanHead)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_REDWHOLEHEAD2);
			}
		}
		mHasHelm = false;
		mHelmType = HelmType.None;
	}

	public void DetachRobotTitanHead(uint theDamageFlags)
	{
		SetupReanimForLostHead();
		ReanimShowPrefix("anim_jaw", -1);
	}

	public void DropShield(uint theDamageFlags)
	{
		if (mShieldType == ShieldType.None)
		{
			return;
		}
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		int aRenderOrder = mRenderOrder + 1;
		TodParticleSystem aParticle = null;
		switch (mShieldType)
		{
		case ShieldType.Door:
			DetachShield();
			if (!TodCommon.TestBit(theDamageFlags, 4))
			{
				float thePosX = 0f;
				float thePosY = 0f;
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_screendoor, ref thePosX, ref thePosY);
				aParticle = mApp.AddTodParticle(thePosX, thePosY, aRenderOrder, ParticleEffect.ZombieDoor);
			}
			break;
		case ShieldType.Newspaper:
			StopEating();
			if (mYuckyFace)
			{
				ShowYuckyFace(theShow: false);
				mYuckyFace = false;
				mYuckyFaceCounter = 0;
			}
			mZombiePhase = ZombiePhase.NewspaperMaddening;
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_gasp, ReanimLoopType.PlayOnceAndHold, 10, 8f);
			DetachShield();
			if (!TodCommon.TestBit(theDamageFlags, 4))
			{
				float thePosX2 = 0f;
				float thePosY2 = 0f;
				GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_paper_paper, ref thePosX2, ref thePosY2);
				aParticle = mApp.AddTodParticle(thePosX2, thePosY2, aRenderOrder, ParticleEffect.ZombieNewspaper);
			}
			if (!TodCommon.TestBit(theDamageFlags, 4) && !TodCommon.TestBit(theDamageFlags, 0))
			{
				mApp.PlayFoley(FoleyType.NewspaperRip);
				AddAttachedReanim(-11, 0, ReanimationType.ZombieSurprise);
				mSurprised = true;
			}
			break;
		case ShieldType.Ladder:
			DetachShield();
			if (!TodCommon.TestBit(theDamageFlags, 4))
			{
				float theX = mPosX + 31f;
				float theY = mPosY + 80f;
				aParticle = mApp.AddTodParticle(theX, theY, aRenderOrder, ParticleEffect.ZombieLadder);
			}
			break;
		}
		OverrideParticleScale(aParticle);
		mHasShield = false;
		mShieldType = ShieldType.None;
	}

	public void ReanimReenableClipping()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			for (int i = 0; i < reanimation.mDefinition.mTrackCount; i++)
			{
				ReanimatorTrackInstance reanimatorTrackInstance = reanimation.mTrackInstances[i];
				reanimatorTrackInstance.mIgnoreClipRect = false;
			}
			mUsesClipping = true;
		}
	}

	public void UpdateBoss()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (mApp.mGameScene == GameScenes.LevelIntro)
		{
			if (reanimation.ShouldTriggerTimedEvent(0.24f) || reanimation.ShouldTriggerTimedEvent(0.79f))
			{
				mApp.PlayFoley(FoleyType.Thump);
				mBoard.ShakeBoard(1, 4);
				mApp.Vibrate();
			}
			return;
		}
		Reanimation reanimation2 = mApp.ReanimationGet(mSpecialHeadReanimID);
		UpdateBossFireball();
		if (mIceTrapCounter == 0)
		{
			if (mSummonCounter > 0)
			{
				mSummonCounter--;
			}
			if (mBossBungeeCounter > 0)
			{
				mBossBungeeCounter--;
			}
			if (mBossStompCounter > 0)
			{
				mBossStompCounter--;
			}
			if (mBossHeadCounter > 0)
			{
				mBossHeadCounter--;
			}
			if (mChilledCounter > 0)
			{
				reanimation2.mAnimRate = 6f;
			}
			else if (reanimation2.mAnimRate == 0f)
			{
				reanimation2.mAnimRate = 12f;
			}
		}
		else
		{
			reanimation2.mAnimRate = 0f;
		}
		if (mZombiePhase == ZombiePhase.BossEnter)
		{
			BossPlayIdle();
		}
		else if (mZombiePhase != ZombiePhase.BossIdle)
		{
			if (mZombiePhase == ZombiePhase.BossSpawning)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.6f))
				{
					BossSpawnContact();
				}
				if (reanimation.mLoopCount > 0)
				{
					BossPlayIdle();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossStomping)
			{
				float theEventTime = 0.5f;
				if (mTargetRow >= 2)
				{
					theEventTime = 0.55f;
				}
				if (reanimation.ShouldTriggerTimedEvent(theEventTime))
				{
					BossStompContact();
				}
				if (reanimation.mLoopCount > 0)
				{
					BossPlayIdle();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossBungeesEnter)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.4f))
				{
					BossBungeeSpawn();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossBungeesDrop)
			{
				if (BossAreBungeesDone())
				{
					BossBungeeLeave();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossBungeesLeave)
			{
				if (reanimation.mLoopCount > 0)
				{
					BossPlayIdle();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossDropRv)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.65f))
				{
					BossRVLanding();
				}
				if (reanimation.mLoopCount > 0)
				{
					BossPlayIdle();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossHeadEnter)
			{
				if (GetBodyDamageIndex() == 2 && reanimation.ShouldTriggerTimedEvent(0.37f))
				{
					ApplyBossSmokeParticles(theEnable: true);
				}
				if (reanimation.ShouldTriggerTimedEvent(0.55f))
				{
					mApp.PlayFoley(FoleyType.Hydraulic);
				}
				if (reanimation.mLoopCount > 0)
				{
					mZombiePhase = ZombiePhase.BossHeadIdleBeforeSpit;
					PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 12f);
					mPhaseCounter = 500;
				}
			}
			else if (mZombiePhase == ZombiePhase.BossHeadIdleBeforeSpit)
			{
				if (mBodyHealth == 1)
				{
					BossStartDeath();
				}
				else if (mPhaseCounter <= 0)
				{
					BossHeadSpit();
				}
			}
			else if (mZombiePhase == ZombiePhase.BossHeadSpit)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.37f))
				{
					BossHeadSpitEffect();
				}
				if (reanimation.ShouldTriggerTimedEvent(0.42f))
				{
					BossHeadSpitContact();
				}
				if (reanimation.mLoopCount > 0)
				{
					reanimation2 = mApp.ReanimationTryToGet(mSpecialHeadReanimID);
					reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 18f);
					mZombiePhase = ZombiePhase.BossHeadIdleAfterSpit;
					PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 12f);
					mPhaseCounter = 300;
				}
			}
			else if (mZombiePhase == ZombiePhase.BossHeadIdleAfterSpit)
			{
				if (mBodyHealth == 1)
				{
					BossStartDeath();
				}
				else if (mPhaseCounter <= 0)
				{
					mZombiePhase = ZombiePhase.BossHeadLeave;
					PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_head_leave, ReanimLoopType.PlayOnceAndHold, 0, 12f);
				}
			}
			else if (mZombiePhase == ZombiePhase.BossHeadLeave)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.23f))
				{
					mChilledCounter = 0;
					UpdateAnimSpeed();
				}
				if (reanimation.ShouldTriggerTimedEvent(0.48f) || reanimation.ShouldTriggerTimedEvent(0.8f))
				{
					mApp.PlayFoley(FoleyType.Thump);
				}
				if (reanimation.mLoopCount > 0)
				{
					ApplyBossSmokeParticles(theEnable: false);
					BossPlayIdle();
				}
			}
			else
			{
				Debug.ASSERT(value: false);
			}
		}
		else if (mBodyHealth == 1)
		{
			PlayDeathAnim(0u);
		}
		else
		{
			if (mPhaseCounter > 0)
			{
				return;
			}
			int bodyDamageIndex = GetBodyDamageIndex();
			if (bodyDamageIndex != mBossMode)
			{
				mBossMode = bodyDamageIndex;
				if (mBossMode == 1)
				{
					BossBungeeAttack();
				}
				else
				{
					BossRVAttack();
				}
			}
			else if (mBossStompCounter == 0)
			{
				BossStompAttack();
			}
			else if (mBossBungeeCounter == 0)
			{
				int ceiling = ((!mApp.IsAdventureMode() && !mApp.IsQuickPlayMode()) ? 2 : 4);
				if (RandomNumbers.NextNumber(ceiling) == 0)
				{
					mBossBungeeCounter = TodCommon.RandRangeInt(4000, 5000);
					BossRVAttack();
				}
				else
				{
					BossBungeeAttack();
				}
			}
			else if (mBossHeadCounter <= 0)
			{
				BossHeadAttack();
			}
			else if (mSummonCounter <= 0)
			{
				BossSpawnAttack();
			}
			else
			{
				mPhaseCounter = TodCommon.RandRangeInt(100, 200);
			}
		}
	}

	public void BossPlayIdle()
	{
		mZombiePhase = ZombiePhase.BossIdle;
		mPhaseCounter = TodCommon.RandRangeInt(100, 200);
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 6f);
	}

	public void BossRVLanding()
	{
		Plant plant = null;
		if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
		{
			for (int i = 0; i < mBoard.mPlants.Count; i++)
			{
				Plant plant2 = mBoard.mPlants[i];
				if (!plant2.mDead && plant2.mRow >= mTargetRow - 1 && plant2.mRow <= mTargetRow + 2 && plant2.mPlantCol >= mTargetCol - 1 && plant2.mPlantCol <= mTargetCol + 3 && plant2.mSeedType == SeedType.Umbrella)
				{
					plant = plant2;
					break;
				}
			}
		}
		if (plant != null)
		{
			mApp.PlayFoley(FoleyType.Umbrella);
			plant.DoSpecial();
			plant.mState = PlantState.UmbrellaDeathTriggered;
		}
		else
		{
			for (int j = 0; j < mBoard.mPlants.Count; j++)
			{
				Plant plant3 = mBoard.mPlants[j];
				if (!plant3.mDead && plant3.mRow >= mTargetRow && plant3.mRow <= mTargetRow + 1 && plant3.mPlantCol >= mTargetCol && plant3.mPlantCol <= mTargetCol + 2)
				{
					plant3.Squish();
				}
			}
		}
		mBoard.ShakeBoard(1, 2);
		mApp.PlaySample(Resources.SOUND_RVTHROW);
		mApp.Vibrate();
		mSummonCounter = 500;
		mBossHeadCounter = 5000;
		if (mBossMode >= 1)
		{
			mBossStompCounter = 4000;
		}
		if (mBossMode >= 2)
		{
			mBossBungeeCounter = 6500;
		}
	}

	public void BossStompContact()
	{
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mRow >= mTargetRow && plant.mRow <= mTargetRow + 1 && plant.mPlantCol >= 5)
			{
				plant.Squish();
			}
		}
		mBoard.ShakeBoard(1, 4);
		mApp.PlayFoley(FoleyType.Thump);
		mApp.Vibrate();
	}

	public bool BossAreBungeesDone()
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			Zombie zombie = mBoard.ZombieTryToGet(mFollowerZombieID[i]);
			if (zombie != null)
			{
				if (zombie.mZombiePhase == ZombiePhase.BungeeRising)
				{
					return true;
				}
				num++;
			}
		}
		return num == 0;
	}

	public void BossBungeeSpawn()
	{
		mZombiePhase = ZombiePhase.BossBungeesDrop;
		for (int i = 0; i < 3; i++)
		{
			Zombie zombie = mBoard.AddZombieInRow(ZombieType.Bungee, 0, 0);
			zombie.PickBungeeZombieTarget(mTargetCol + i);
			zombie.mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, zombie.mRow, 7);
			zombie.mAltitude = zombie.mPosY - 30f;
			mFollowerZombieID[i] = mBoard.ZombieGetID(zombie);
		}
	}

	public void BossSpawnAttack()
	{
		RemoveColdEffects();
		mZombiePhase = ZombiePhase.BossSpawning;
		if (mBossMode == 0)
		{
			mSummonCounter = TodCommon.RandRangeInt(450, 550);
		}
		else if (mBossMode == 1)
		{
			mSummonCounter = TodCommon.RandRangeInt(350, 450);
		}
		else if (mBossMode == 2)
		{
			mSummonCounter = TodCommon.RandRangeInt(150, 250);
		}
		mTargetRow = mBoard.PickRowForNewZombie(ZombieType.Normal);
		string theTrackName = string.Empty;
		switch (mTargetRow)
		{
		case 0:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_spawn_1;
			break;
		case 1:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_spawn_2;
			break;
		case 2:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_spawn_3;
			break;
		case 3:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_spawn_4;
			break;
		case 4:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_spawn_5;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		PlayZombieReanim(ref theTrackName, ReanimLoopType.PlayOnceAndHold, 20, 12f);
		mApp.PlayFoley(FoleyType.HydraulicShort);
	}

	public void BossBungeeAttack()
	{
		RemoveColdEffects();
		mZombiePhase = ZombiePhase.BossBungeesEnter;
		mBossBungeeCounter = TodCommon.RandRangeInt(4000, 5000);
		mTargetCol = TodCommon.RandRangeInt(0, 2);
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_bungee_1_enter, ReanimLoopType.PlayOnceAndHold, 20, 12f);
		mApp.PlayFoley(FoleyType.HydraulicShort);
		mApp.PlayFoley(FoleyType.BungeeScream);
	}

	public void BossRVAttack()
	{
		RemoveColdEffects();
		mZombiePhase = ZombiePhase.BossDropRv;
		mTargetRow = TodCommon.RandRangeInt(0, 3);
		mTargetCol = TodCommon.RandRangeInt(0, 2);
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_rv_1, ReanimLoopType.PlayOnceAndHold, 20, 16f);
		mApp.PlayFoley(FoleyType.HydraulicShort);
	}

	public void BossSpawnContact()
	{
		ZombieType theZombieType;
		if (mZombieAge < 3500)
		{
			theZombieType = ZombieType.Normal;
		}
		else if (mZombieAge < 8000)
		{
			theZombieType = ZombieType.TrafficCone;
		}
		else if (mZombieAge < 12500)
		{
			theZombieType = ZombieType.Pail;
		}
		else
		{
			ZombieType[] array;
			int num;
			if (mApp.mGameMode == GameMode.ChallengeFinalBoss2)
			{
				array = GameConstants.gBossZombieList2;
				num = 2;
			}
			else if (mApp.IsAdventureModeHard())
			{
				array = GameConstants.gBossZombieListHard;
				num = 2;
			}
			else
			{
				array = GameConstants.gBossZombieList;
				num = 1;
			}
			int num2 = array.Length;
			if (mTargetRow == 0)
			{
				num2 -= num;
			}
			theZombieType = array[RandomNumbers.NextNumber(num2)];
		}
		Zombie zombie = mBoard.AddZombieInRow(theZombieType, mTargetRow, 0);
		zombie.mPosX = 600f + mPosX;
	}

	public void BossBungeeLeave()
	{
		mZombiePhase = ZombiePhase.BossBungeesLeave;
		for (int i = 0; i < 3; i++)
		{
			Zombie zombie = mBoard.ZombieTryToGet(mFollowerZombieID[i]);
			if (zombie != null && zombie.mButteredCounter > 0)
			{
				zombie.DieWithLoot();
			}
		}
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_bungee_1_leave, ReanimLoopType.PlayOnceAndHold, 20, 18f);
	}

	public void BossStompAttack()
	{
		RemoveColdEffects();
		mZombiePhase = ZombiePhase.BossStomping;
		mBossStompCounter = TodCommon.RandRangeInt(5500, 6500);
		int num = 0;
		int[] array = new int[4];
		for (int i = 0; i < 4; i++)
		{
			if (BossCanStompRow(i))
			{
				array[num] = i;
				num++;
			}
		}
		if (num != 0)
		{
			mTargetRow = TodCommon.TodPickFromArray(array, num);
			string theTrackName = string.Empty;
			switch (mTargetRow)
			{
			case 0:
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_stomp_1;
				break;
			case 1:
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_stomp_2;
				break;
			case 2:
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_stomp_3;
				break;
			case 3:
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_stomp_4;
				break;
			default:
				Debug.ASSERT(value: false);
				break;
			}
			PlayZombieReanim(ref theTrackName, ReanimLoopType.PlayOnceAndHold, 20, 12f);
			mApp.PlayFoley(FoleyType.HydraulicShort);
		}
	}

	public bool BossCanStompRow(int theRow)
	{
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && !plant.NotOnGround() && plant.mRow >= theRow && plant.mRow <= theRow + 1 && plant.mPlantCol >= 5)
			{
				return true;
			}
		}
		return false;
	}

	public void BossDie()
	{
		if (!IsOnBoard())
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBossFireBallReanimID);
		if (reanimation != null)
		{
			reanimation.ReanimationDie();
			mBossFireBallReanimID = null;
			BossDestroyIceballInRow(mFireballRow);
			BossDestroyFireball();
		}
		mApp.mMusic.FadeOut(200);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && zombie != this && !zombie.IsDeadOrDying())
			{
				zombie.DieWithLoot();
			}
		}
		RemoveColdEffects();
	}

	public void BossHeadAttack()
	{
		mZombiePhase = ZombiePhase.BossHeadEnter;
		mBossHeadCounter = TodCommon.RandRangeInt(4000, 5000);
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_head_enter, ReanimLoopType.PlayOnceAndHold, 20, 12f);
		mApp.PlayFoley(FoleyType.HydraulicShort);
	}

	public void BossHeadSpitContact()
	{
		Debug.ASSERT(mApp.ReanimationTryToGet(mBossFireBallReanimID) == null);
		float num = 550f + mPosX;
		float theY = mBoard.GetPosYBasedOnRow(num, mFireballRow) + GameConstants.BOSS_BALL_OFFSET_Y;
		int aRenderOrder = mRenderOrder + 1;
		Reanimation reanimation;
		if (mIsFireBall)
		{
			num -= 95f;
			reanimation = mApp.AddReanimation(num, theY, aRenderOrder, ReanimationType.BossFireball);
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_form, ReanimLoopType.PlayOnceAndHold, 0, 16f);
			reanimation.mIsAttachment = true;
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_additive, GameConstants.RENDER_GROUP_BOSS_FIREBALL_ADDITIVE);
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_superglow, GameConstants.RENDER_GROUP_BOSS_FIREBALL_ADDITIVE);
		}
		else
		{
			num -= 95f;
			reanimation = mApp.AddReanimation(num, theY, aRenderOrder, ReanimationType.BossIceball);
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_form, ReanimLoopType.PlayOnceAndHold, 0, 16f);
			reanimation.mIsAttachment = true;
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_ice_highlight, GameConstants.RENDER_GROUP_BOSS_FIREBALL_ADDITIVE);
		}
		mBossFireBallReanimID = mApp.ReanimationGetID(reanimation);
		mApp.ReanimationTryToGet(mSpecialHeadReanimID).PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_laugh, ReanimLoopType.Loop, 20, 18f);
		mApp.PlayFoley(FoleyType.HydraulicShort);
	}

	public void BossHeadSpit()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBossFireBallReanimID);
		if (reanimation != null)
		{
			reanimation.ReanimationDie();
			mBossFireBallReanimID = null;
		}
		mZombiePhase = ZombiePhase.BossHeadSpit;
		mFireballRow = TodCommon.RandRangeInt(0, 4);
		mIsFireBall = TodCommon.RandRangeInt(0, 1) == 0;
		string theTrackName = string.Empty;
		switch (mFireballRow)
		{
		case 0:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_head_attack_1;
			break;
		case 1:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_head_attack_2;
			break;
		case 2:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_head_attack_3;
			break;
		case 3:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_head_attack_4;
			break;
		case 4:
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_head_attack_5;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		PlayZombieReanim(ref theTrackName, ReanimLoopType.PlayOnceAndHold, 20, 12f);
		Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
		Image theImage = null;
		if (mIsFireBall)
		{
			reanimation2.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_eyeglow_red, theImage);
			reanimation2.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_mouthglow_red, theImage);
		}
		else
		{
			reanimation2.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_eyeglow_red, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_EYEGLOW_BLUE);
			reanimation2.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_boss_mouthglow_red, AtlasResources.IMAGE_REANIM_ZOMBIE_BOSS_MOUTHGLOW_BLUE);
		}
		Reanimation reanimation3 = mApp.ReanimationTryToGet(mSpecialHeadReanimID);
		reanimation3.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_drive, ReanimLoopType.Loop, 20, 36f);
	}

	public void UpdateBossFireball()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBossFireBallReanimID);
		if (reanimation == null)
		{
			return;
		}
		float trackVelocity = reanimation.GetTrackVelocity(Reanimation.ReanimTrackId__ground);
		float num = reanimation.mOverlayMatrix.mMatrix.M41 * Constants.IS;
		num -= trackVelocity;
		reanimation.mOverlayMatrix.mMatrix.M41 = num * Constants.S;
		float num2 = mBoard.GetPosYBasedOnRow(num + 75f, mFireballRow) + GameConstants.BOSS_BALL_OFFSET_Y;
		reanimation.mOverlayMatrix.mMatrix.M42 = num2 * Constants.S;
		if (num < -180f + (float)Constants.BOARD_EXTRA_ROOM)
		{
			reanimation.ReanimationDie();
			mBossFireBallReanimID = null;
		}
		int theX = mBoard.PixelToGridX((int)num + 75, (int)num2);
		SquishAllInSquare(theX, mFireballRow, ZombieAttackType.DriveOver);
		foreach (LawnMower mLawnMower in mBoard.mLawnMowers)
		{
			if (!mLawnMower.mDead && mLawnMower.mMowerState != LawnMowerState.Squished && mLawnMower.mRow == mFireballRow && mLawnMower.mPosX > num && mLawnMower.mPosX < num + 50f)
			{
				mLawnMower.SquishMower();
			}
		}
		if (mIsFireBall)
		{
			if (reanimation.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation.mLoopCount > 0)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_role, ReanimLoopType.Loop, 0, 2f);
				reanimation.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, mFireballRow, 0);
			}
			if (reanimation.mLoopType == ReanimLoopType.Loop && RandomNumbers.NextNumber(10) == 0)
			{
				float num3 = num + 100f + TodCommon.RandRangeFloat(0f, 20f);
				float theY = mBoard.GetPosYBasedOnRow(num3 - 40f, mFireballRow) + 90f + TodCommon.RandRangeFloat(-50f, 0f);
				mApp.AddTodParticle(num3, theY, Board.MakeRenderOrder(RenderLayer.GraveStone, mFireballRow, 6), ParticleEffect.FireballTrail);
			}
		}
		else
		{
			if (reanimation.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation.mLoopCount > 0)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_role, ReanimLoopType.Loop, 0, 2f);
				reanimation.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, mFireballRow, 0);
			}
			if (reanimation.mLoopType == ReanimLoopType.Loop && RandomNumbers.NextNumber(10) == 0)
			{
				float num4 = num + 100f + TodCommon.RandRangeFloat(0f, 20f);
				float theY2 = mBoard.GetPosYBasedOnRow(num4 - 40f, mFireballRow) + 90f + TodCommon.RandRangeFloat(-50f, 0f);
				mApp.AddTodParticle(num4, theY2, Board.MakeRenderOrder(RenderLayer.GraveStone, mFireballRow, 6), ParticleEffect.IceballTrail);
			}
		}
		reanimation.Update();
	}

	public void BossDestroyFireball()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBossFireBallReanimID);
		if (reanimation != null && mIsFireBall)
		{
			float num = reanimation.mOverlayMatrix.mMatrix.M41 * Constants.IS + 80f;
			float num2 = reanimation.mOverlayMatrix.mMatrix.M42 * Constants.IS + 40f;
			for (int i = 0; i < 6; i++)
			{
				float num3 = (float)Math.PI / 2f + (float)Math.PI * 2f * (float)i / 6f;
				float theX = num + 60f * (float)Math.Sin(num3);
				float theY = num2 + 60f * (float)Math.Cos(num3);
				Reanimation reanimation2 = mApp.AddReanimation(theX, theY, 400000, ReanimationType.JalapenoFire);
				reanimation2.mLoopType = ReanimLoopType.PlayOnceFullLastFrame;
				reanimation2.mAnimTime = 0.2f;
				reanimation2.mAnimRate = TodCommon.RandRangeFloat(20f, 25f);
			}
			reanimation.ReanimationDie();
			mBossFireBallReanimID = null;
			mBoard.RemoveParticleByType(ParticleEffect.FireballTrail);
		}
	}

	public void BossDestroyIceballInRow(int theRow)
	{
		if (theRow == mFireballRow)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBossFireBallReanimID);
			if (reanimation != null && !mIsFireBall)
			{
				float theX = reanimation.mOverlayMatrix.mMatrix.M41 * Constants.IS + 80f;
				float theY = reanimation.mOverlayMatrix.mMatrix.M42 * Constants.IS + 80f;
				mApp.AddTodParticle(theX, theY, 400000, ParticleEffect.IceballDeath);
				reanimation.ReanimationDie();
				mBossFireBallReanimID = null;
				mBoard.RemoveParticleByType(ParticleEffect.IceballTrail);
			}
		}
	}

	public void DiggerLoseAxe()
	{
		if (mZombiePhase == ZombiePhase.DiggerTunneling)
		{
			mZombiePhase = ZombiePhase.DiggerTunnelingPauseWithoutAxe;
			mPhaseCounter = 200;
			SetAnimRate(0f);
			UpdateAnimSpeed();
			GlobalMembersAttachment.AttachmentDetachCrossFadeParticleType(ref mAttachmentID, ParticleEffect.DiggerTunnel, null);
			StopZombieSound();
		}
		mHasObject = false;
		ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_digger_pickaxe, -1);
		ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_digger_dirt, -1);
	}

	public void BungeeDropZombie(Zombie theDroppedZombie, int theGridX, int theGridY)
	{
		mTargetCol = theGridX;
		SetRow(theGridY);
		mPosX = mBoard.GridToPixelX(mTargetCol, mRow);
		mPosY = GetPosYBasedOnRow(mRow);
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_raise, ReanimLoopType.PlayOnceAndHold, 0, 36f);
		mRelatedZombieID = mBoard.ZombieGetID(theDroppedZombie);
		theDroppedZombie.mPosX = mPosX - 15f;
		theDroppedZombie.SetRow(theGridY);
		theDroppedZombie.mPosY = GetPosYBasedOnRow(theGridY);
		theDroppedZombie.mZombieHeight = ZombieHeight.GettingBungeeDropped;
		if (mApp.mCreativeLevel?.GetComponent<CSBungeeBlitz>() == null)
		{
			theDroppedZombie.PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 0f);
		}
		theDroppedZombie.mRenderOrder = mRenderOrder + 1;
	}

	private Image GetYuckyFaceImage()
	{
		if (mZombieType == ZombieType.Dancer)
		{
			return AtlasResources.IMAGE_REANIM_ZOMBIE_DISCO_HEAD_GROSSOUT;
		}
		if (mZombieType == ZombieType.BackupDancer)
		{
			return AtlasResources.IMAGE_REANIM_ZOMBIE_DANCER_HEAD_GROSSOUT;
		}
		if (mZombieType == ZombieType.Polevaulter)
		{
			return AtlasResources.IMAGE_REANIM_ZOMBIE_POLEVAULTER_HEAD_GROSSOUT;
		}
		return AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_GROSSOUT;
	}

	public void ShowYuckyFace(bool theShow)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null || !HasYuckyFaceImage())
		{
			return;
		}
		if (theShow)
		{
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, GetYuckyFaceImage());
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_head2, -1);
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_head_jaw, -1);
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_tongue, -1);
		}
		else if (mHasHead)
		{
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, null);
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_head2, 0);
			reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_head_jaw, 0);
			if (mVariant)
			{
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_tongue, 0);
			}
		}
	}

	public void AnimateChewSound()
	{
		if (mZombiePhase == ZombiePhase.SnorkelUpToEat)
		{
			return;
		}
		Plant plant = FindPlantTarget(ZombieAttackType.Chew);
		if (plant != null)
		{
			if (plant.mSeedType == SeedType.Hypnoshroom && !plant.IsDisabled() && plant.mState != PlantState.Entering)
			{
				mApp.PlayFoley(FoleyType.Floop);
				plant.DoSpecial();
				StartMindControlled();
				mApp.AddTodParticle(mPosX + 60f, mPosY + 40f, mRenderOrder + 1, ParticleEffect.MindControl);
				TrySpawnLevelAward();
				mVelX = 0.17f;
				mAnimTicksPerFrame = 18;
				UpdateAnimSpeed();
			}
			else if (plant.mSeedType == SeedType.Garlic && !plant.IsDisabled())
			{
				if (!mYuckyFace)
				{
					mYuckyFace = true;
					mYuckyFaceCounter = 0;
					UpdateAnimSpeed();
					mApp.PlayFoley(FoleyType.Chomp);
				}
			}
			else if (plant.mSeedType == SeedType.Wallnut || plant.mSeedType == SeedType.Tallnut || plant.mSeedType == SeedType.Garlic || plant.mSeedType == SeedType.Pumpkinshell)
			{
				mApp.PlayFoley(FoleyType.ChompSoft);
			}
			else
			{
				mApp.PlayFoley(FoleyType.Chomp);
			}
		}
		else if (mMindControlled)
		{
			mApp.PlayFoley(FoleyType.ChompSoft);
		}
		else
		{
			mApp.PlayFoley(FoleyType.Chomp);
		}
	}

	public void AnimateChewEffect()
	{
		if (mZombiePhase == ZombiePhase.SnorkelUpToEat)
		{
			return;
		}
		if (mApp.IsIZombieLevel() || mApp.IsCornRouletteLevel())
		{
			GridItem gridItem = mBoard.mChallenge.IZombieGetBrainTarget(this);
			if (gridItem != null)
			{
				gridItem.mTransparentCounter = Math.Max(gridItem.mTransparentCounter, 25);
				return;
			}
		}
		Plant plant = FindPlantTarget(ZombieAttackType.Chew);
		if (plant == null)
		{
			return;
		}
		if (plant.mSeedType == SeedType.Wallnut || plant.mSeedType == SeedType.Tallnut)
		{
			int aRenderOrder = Board.MakeRenderOrder(RenderLayer.Projectile, mRow, 0);
			ZombieDrawPosition theDrawPos = default;
			GetDrawPos(ref theDrawPos);
			float num = mPosX + 37f;
			float num2 = mPosY + 40f + theDrawPos.mBodyY;
			if (mZombieType == ZombieType.Snorkel || mZombieType == ZombieType.DolphinRider)
			{
				num -= 7f;
				num2 += 70f;
			}
			else if (IsWalkingBackwards())
			{
				num += 47f;
			}
			else if (mZombieType == ZombieType.Balloon)
			{
				num2 += 47f;
			}
			else if (mZombieType == ZombieType.Imp)
			{
				num += 24f;
				num2 += 40f;
			}
			mApp.AddTodParticle(num, num2, aRenderOrder, (plant.mSeedType == SeedType.Tallnut) ? ParticleEffect.WallnutNewEatSmall : ParticleEffect.WallnutEatSmall);
		}
		plant.mEatenFlashCountdown = Math.Max(plant.mEatenFlashCountdown, 25);
	}

	public void UpdateActions()
	{
		if (mZombieHeight == ZombieHeight.UpLadder)
		{
			UpdateClimbingLadder();
		}
		if (mZombieHeight == ZombieHeight.Zombiquarium)
		{
			UpdateZombiquarium();
		}
		if (mZombieHeight == ZombieHeight.OutOfPool || mZombieHeight == ZombieHeight.InToPool || mInPool)
		{
			UpdateZombiePool();
		}
		if (mZombieHeight == ZombieHeight.UpToHighGround || mZombieHeight == ZombieHeight.DownOffHighGround)
		{
			UpdateZombieHighGround();
		}
		if (mZombieHeight == ZombieHeight.Falling)
		{
			UpdateZombieFalling();
		}
		if (mZombieHeight == ZombieHeight.InToChimney)
		{
			UpdateZombieChimney();
		}
		if (mZombieType == ZombieType.Polevaulter)
		{
			UpdateZombiePolevaulter();
		}
		if (mZombieType == ZombieType.Catapult)
		{
			UpdateZombieCatapult();
		}
		if (mZombieType == ZombieType.DolphinRider)
		{
			UpdateZombieDolphinRider();
		}
		if (mZombieType == ZombieType.Snorkel)
		{
			UpdateZombieSnorkel();
		}
		if (mZombieType == ZombieType.Balloon || mZombieType == ZombieType.Propeller)
		{
			UpdateZombieFlyer();
		}
		if (mZombieType == ZombieType.Newspaper)
		{
			UpdateZombieNewspaper();
		}
		if (mZombieType == ZombieType.Digger)
		{
			UpdateZombieDigger();
		}
		if (mZombieType == ZombieType.JackInTheBox)
		{
			UpdateZombieJackInTheBox();
		}
		if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
		{
			UpdateZombieGargantuar();
		}
		if (mZombieType == ZombieType.Bobsled)
		{
			UpdateZombieBobsled();
		}
		if (mZombieType == ZombieType.Zamboni)
		{
			UpdateZamboni();
		}
		if (mZombieType == ZombieType.Ladder)
		{
			UpdateLadder();
		}
		if (mZombieType == ZombieType.Yeti)
		{
			UpdateYeti();
		}
		if (mZombieType == ZombieType.Dancer)
		{
			UpdateZombieDancer();
		}
		if (mZombieType == ZombieType.BackupDancer)
		{
			UpdateZombieBackupDancer();
		}
		if (mZombieType == ZombieType.Imp)
		{
			UpdateZombieImp();
		}
		if (mZombieType == ZombieType.PeaHead)
		{
			UpdateZombiePeaHead();
		}
		if (mZombieType == ZombieType.JalapenoHead)
		{
			UpdateZombieJalapenoHead();
		}
		if (mZombieType == ZombieType.GatlingHead)
		{
			UpdateZombieGatlingHead();
		}
		if (mZombieType == ZombieType.SquashHead)
		{
			UpdateZombieSquashHead();
		}
		if (mZombieType == ZombieType.Monk)
		{
			UpdateZombieMonk();
		}
		if (mZombieType == ZombieType.Talisman)
		{
			UpdateZombieTalisman();
		}
	}

	public void CreateTalismanAt(int theGridX, int theGridY)
	{
		GridItem talismanAt = mBoard.GetTalismanAt(theGridX, theGridY);
		if (talismanAt != null)
		{
			talismanAt.mGridItemCounter = 3000;
			return;
		}
		talismanAt = GridItem.GetNewGridItem();
		talismanAt.mGridItemType = GridItemType.Talisman;
		talismanAt.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, theGridY, 2);
		talismanAt.mGridX = theGridX;
		talismanAt.mGridY = theGridY;
		int num = mBoard.GridToPixelX(theGridX, theGridY);
		int num2 = mBoard.GridToPixelY(theGridX, theGridY) - 30;
		talismanAt.mPosX = num;
		talismanAt.mPosY = num2;
		Reanimation reanimation = mApp.AddReanimation(num, num2, 0, ReanimationType.Talisman);
		reanimation.PlayReanim("anim_overdue_souls", ReanimLoopType.Loop, 10, 12f);
		talismanAt.mGridItemReanimID = mApp.ReanimationGetID(reanimation);
		talismanAt.mGridItemCounter = 3000;
		mBoard.mGridItems.Add(talismanAt);
	}

	public void CreateTalismanMoveAt(int theGridX, int theGridY)
	{
		GridItem newGridItem = GridItem.GetNewGridItem();
		newGridItem.mGridItemType = GridItemType.TalismanMove;
		newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, theGridY, 2);
		newGridItem.mGridX = theGridX;
		newGridItem.mGridY = theGridY;
		int num = mBoard.GridToPixelX(theGridX, theGridY);
		int num2 = mBoard.GridToPixelY(theGridX, theGridY) - 30;
		newGridItem.mPosX = num;
		newGridItem.mPosY = num2;
		Reanimation reanimation = mApp.AddReanimation(num, num2, 0, ReanimationType.Talisman);
		reanimation.PlayReanim("anim_five_ghosts_luck_money", ReanimLoopType.Loop, 10, 7.5f);
		newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation);
		newGridItem.mGridItemCounter = 200;
		mBoard.mGridItems.Add(newGridItem);
	}

	public void UpdateZombieTalisman()
	{
		if (mZombiePhase == ZombiePhase.ZombieNormal)
		{
			if (mHasHead && mPhaseCounter <= 0)
			{
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_attack, ReanimLoopType.PlayOnceAndHold, 20, 24f);
				mApp.PlaySample(Resources.SOUND_TALISMAN_ATTACK_1);
				mZombiePhase = ZombiePhase.TalismanAttacking;
				mPhaseCounter = 250;
			}
		}
		else if (mZombiePhase == ZombiePhase.TalismanAttacking)
		{
			if (mPhaseCounter == 200)
			{
				bool flag = false;
				bool flag2 = false;
				Plant plant = null;
				Plant plant2 = null;
				int num = 0;
				Plant thePlant = null;
				int index = -1;
				while (mBoard.IteratePlants(ref thePlant, ref index))
				{
					if (!thePlant.NotOnGround())
					{
						if (Plant.CanSwap(thePlant.mSeedType))
						{
							num++;
						}
						SeedType mSeedType = thePlant.mSeedType;
						if ((mSeedType == SeedType.Torchwood || mSeedType == SeedType.FireShroom || mSeedType == SeedType.Endoflame) ? true : false)
						{
							flag2 = true;
						}
					}
				}
				if (num > 0)
				{
					int num2 = RandomNumbers.NextNumber(num);
					thePlant = null;
					index = -1;
					while (mBoard.IteratePlants(ref thePlant, ref index))
					{
						if (!thePlant.NotOnGround() && Plant.CanSwap(thePlant.mSeedType))
						{
							if (num2 == 0)
							{
								plant = thePlant;
								break;
							}
							num2--;
						}
					}
					if (plant != null)
					{
						num = 0;
						thePlant = null;
						index = -1;
						while (mBoard.IteratePlants(ref thePlant, ref index))
						{
							if (!thePlant.NotOnGround() && Plant.CanSwap(thePlant.mSeedType) && thePlant.mSeedType != plant.mSeedType)
							{
								num++;
							}
						}
						if (num > 0)
						{
							int num3 = RandomNumbers.NextNumber(num);
							thePlant = null;
							index = -1;
							while (mBoard.IteratePlants(ref thePlant, ref index))
							{
								if (!thePlant.NotOnGround() && Plant.CanSwap(thePlant.mSeedType) && thePlant.mSeedType != plant.mSeedType)
								{
									if (num3 == 0)
									{
										plant2 = thePlant;
										break;
									}
									num3--;
								}
							}
							if (plant2 != null)
							{
								flag = true;
							}
						}
					}
				}
				int num4;
				if (!flag && !flag2)
				{
					num4 = 1;
				}
				else
				{
					num4 = RandomNumbers.NextNumber((flag & flag2) ? 3 : 2);
					if (!flag2 && num4 == 0)
					{
						num4 = 2;
					}
				}
				switch (num4)
				{
				case 0:
				{
					Plant thePlant2 = null;
					int index2 = -1;
					while (mBoard.IteratePlants(ref thePlant2, ref index2))
					{
						bool flag3 = !thePlant2.NotOnGround();
						bool flag4 = flag3;
						if (flag4)
						{
							SeedType mSeedType = thePlant2.mSeedType;
							bool flag5 = ((mSeedType == SeedType.Torchwood || mSeedType == SeedType.FireShroom || mSeedType == SeedType.Endoflame) ? true : false);
							flag4 = flag5;
						}
						if (flag4)
						{
							thePlant2.SetSealing(theIsSealed: true);
						}
					}
					if (mBoard.mSealedCountdown < 3000)
					{
						mBoard.mSealedCountdown = 3000;
					}
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Talisman_Tianshuihengliu);
					break;
				}
				case 1:
				{
					int num5 = 0;
					for (int i = 0; i < Constants.MAX_GRIDSIZEY; i++)
					{
						for (int j = 0; j < 7; j++)
						{
							GridSquareType gridSquareType = mBoard.mGridSquareType[j, i];
							if ((gridSquareType != GridSquareType.None && gridSquareType != GridSquareType.Dirt) || 1 == 0)
							{
								aPicks[num5].mX = j;
								aPicks[num5].mY = i;
								aPicks[num5].mWeight = ((mBoard.GetTalismanAt(j, i) != null || mBoard.GetTopPlantAt(j, i, TopPlant.Any) == null) ? 1 : 1000);
								num5++;
							}
						}
					}
					if (num5 > 0)
					{
						TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(aPicks, num5);
						CreateTalismanAt(todWeightedGridArray.mX, todWeightedGridArray.mY);
						num5--;
						todWeightedGridArray.mX = aPicks[num5].mX;
						todWeightedGridArray.mY = aPicks[num5].mY;
						todWeightedGridArray.mWeight = aPicks[num5].mWeight;
					}
					if (num5 > 0)
					{
						TodWeightedGridArray todWeightedGridArray2 = TodCommon.TodPickFromWeightedGridArray(aPicks, num5);
						CreateTalismanAt(todWeightedGridArray2.mX, todWeightedGridArray2.mY);
					}
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Talisman_Fengyin);
					break;
				}
				default:
					CreateTalismanMoveAt(plant.mPlantCol, plant.mRow);
					CreateTalismanMoveAt(plant2.mPlantCol, plant2.mRow);
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Talisman_Wuguiyuncai);
					break;
				}
			}
			else if (mPhaseCounter <= 0)
			{
				mZombiePhase = ZombiePhase.TalismanLeaving;
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_leave, ReanimLoopType.PlayOnceAndHold, 20, 24f);
				mApp.AddTodParticle(mPosX + 60f, mPosY + 130f, Board.MakeRenderOrder(RenderLayer.Particle, mRow, 0), ParticleEffect.WhackAZombieRise);
				mPhaseCounter = 50;
			}
		}
		else if (mZombiePhase == ZombiePhase.TalismanLeaving)
		{
			mAltitude = TodCommon.TodAnimateCurve(35, 0, mPhaseCounter, 0, -25, TodCurves.EaseOut);
			mUsesClipping = mAltitude < 0f;
			if (mPhaseCounter <= 0)
			{
				DieNoLoot(giveAchievements: false);
			}
		}
	}

	public void UpdateZombieMonk()
	{
		if (IsDeadOrDying() || !mHasHead)
		{
			if (mHelmType == HelmType.Bell)
			{
				DropHelm(0u);
			}
			if (mBodyHealth - 1 > 0)
			{
				mBodyHealth--;
			}
			return;
		}
		if (mShieldType == ShieldType.None && mHelmType == HelmType.None && 0 < mPhaseCounter && mPhaseCounter <= 140 && mHasHead)
		{
			mHelmType = HelmType.Bell;
			mHelmMaxHealth = 2000;
			mHelmHealth = mHelmMaxHealth;
			mApp.ReanimationTryToGet(mBodyReanimID)?.PlayReanim("anim_sp", ReanimLoopType.PlayOnceAndHold, 30, 8f);
			mZombiePhase = ZombiePhase.MonkInBell;
			mApp.PlayFoley(FoleyType.ZombieMonkSp);
		}
		if (mPhaseCounter <= 0 && mZombiePhase == ZombiePhase.MonkInBell && mHasHead)
		{
			mApp.ReanimationTryToGet(mBodyReanimID)?.AssignRenderGroupToTrack("special", 0);
			StartWalkAnim(50);
			mZombiePhase = ZombiePhase.ZombieNormal;
		}
	}

	public void CheckForBoardEdge()
	{
		if (mZombieType == ZombieType.Boss || mZombieHeight == ZombieHeight.Zombiquarium)
		{
			return;
		}
		if (IsWalkingBackwards() && mPosX > 900f)
		{
			DieNoLoot(giveAchievements: false);
			return;
		}
		int num = Constants.BOARD_EDGE;
		if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar)
		{
			num = Constants.BOARD_EDGE - 50;
		}
		else if (mZombieType == ZombieType.Polevaulter)
		{
			num = Constants.BOARD_EDGE - 50;
		}
		else if (mZombieType == ZombieType.Catapult || mZombieType == ZombieType.Football || mZombieType == ZombieType.Zamboni)
		{
			num = Constants.BOARD_EDGE - 75;
		}
		else if (mZombieType == ZombieType.Zamboni)
		{
			num = Constants.BOARD_EDGE - 75;
		}
		else if (mZombieType == ZombieType.BackupDancer || mZombieType == ZombieType.Dancer || mZombieType == ZombieType.Snorkel)
		{
			num = Constants.BOARD_EDGE - 30;
		}
		if (mX <= num && (mHasHead || IsAliveWithoutHead()))
		{
			if (mApp.IsIZombieLevel())
			{
				DieNoLoot(giveAchievements: false);
			}
			else
			{
				mBoard.ZombiesWon(this);
			}
		}
		if (mX <= num + 70 && !mHasHead && !IsAliveWithoutHead())
		{
			TakeDamage(1800, 9u);
		}
	}

	public bool IsAliveWithoutHead()
	{
		if (mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
		{
			return mBodyHealth > 0;
		}
		return false;
	}

	public void UpdateYeti()
	{
		if (!mMindControlled && mHasHead && !IsDeadOrDying() && mZombiePhase == ZombiePhase.ZombieNormal && mPhaseCounter <= 0)
		{
			mZombiePhase = ZombiePhase.YetiRunning;
			mHasObject = false;
			PickRandomSpeed();
		}
	}

	public void DrawBossPart(Graphics g, BossPart theBossPart)
	{
		if (Tavel.Loaded)
		{
			TavelHooks.ZombieDrawBossPart(this, g, theBossPart);
			return;
		}
		DrawBossPartCore(g, theBossPart);
	}

	public void DrawBossPartCore(Graphics g, BossPart theBossPart)
	{
		ZombieDrawPosition theDrawPos = default;
		GetDrawPos(ref theDrawPos);
		switch (theBossPart)
		{
		case BossPart.BackLeg:
			DrawReanim(g, ref theDrawPos, GameConstants.RENDER_GROUP_BOSS_BACK_LEG);
			break;
		case BossPart.FrontLeg:
			DrawReanim(g, ref theDrawPos, GameConstants.RENDER_GROUP_BOSS_FRONT_LEG);
			break;
		case BossPart.Main:
			DrawReanim(g, ref theDrawPos, 0);
			break;
		case BossPart.BackArm:
			DrawBossBackArm(g, ref theDrawPos);
			break;
		case BossPart.Fireball:
			DrawBossFireBall(g, ref theDrawPos);
			break;
		}
	}

	public void BossSetupReanim()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		reanimation.AssignRenderGroupToPrefix("Boss_innerleg", GameConstants.RENDER_GROUP_BOSS_BACK_LEG);
		reanimation.AssignRenderGroupToPrefix("Boss_outerleg", GameConstants.RENDER_GROUP_BOSS_FRONT_LEG);
		reanimation.AssignRenderGroupToPrefix("Boss_body2", GameConstants.RENDER_GROUP_BOSS_FRONT_LEG);
		reanimation.AssignRenderGroupToPrefix("Boss_innerarm", GameConstants.RENDER_GROUP_BOSS_BACK_ARM);
		reanimation.AssignRenderGroupToPrefix("Boss_RV", GameConstants.RENDER_GROUP_BOSS_BACK_ARM);
		Reanimation reanimation2 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.BossDriver);
		reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 12f);
		mSpecialHeadReanimID = mApp.ReanimationGetID(reanimation2);
		ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_boss_head2);
		AttachEffect attachEffect = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName.mAttachmentID, reanimation2, 25f * Constants.S, -70f * Constants.S);
		reanimation.mFrameBasePose = 0;
		attachEffect.mDontDrawIfParentHidden = true;
		attachEffect.mOffset.Scale(1.2f, 1.2f);
	}

	public void MowDown()
	{
		if (mDead || mZombiePhase == ZombiePhase.ZombieMowered || mZombieType == ZombieType.Boss)
		{
			return;
		}
		if (mZombieType == ZombieType.Catapult)
		{
			mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.CatapultExplosion);
			mApp.PlayFoley(FoleyType.Explosion);
			DieWithLoot();
			return;
		}
		if (mZombieType == ZombieType.Zamboni)
		{
			mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.ZamboniExplosion);
			mApp.PlayFoley(FoleyType.Explosion);
			DieWithLoot();
			return;
		}
		if (mZombieType == ZombieType.RobotTitan)
		{
			mApp.AddTodParticle(mPosX + 80f, mPosY + 60f, mRenderOrder + 1, ParticleEffect.RobotTitanExplosion);
			mApp.PlayFoley(FoleyType.Explosion);
			DieWithLoot();
			return;
		}
		if (mZombiePhase == ZombiePhase.ZombieDying || mZombiePhase == ZombiePhase.PolevaulterInVault || mZombiePhase == ZombiePhase.RisingFromGrave || mZombiePhase == ZombiePhase.DancerRising || mZombiePhase == ZombiePhase.SnorkelIntoPool || mZombiePhase == ZombiePhase.ZombieBurned || mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar || mZombieType == ZombieType.Bungee || mZombieType == ZombieType.Digger || mZombieType == ZombieType.Imp || mZombieType == ZombieType.Yeti || mZombieType == ZombieType.DolphinRider || IsBobsledTeamWithSled() || IsFlying() || mInPool)
		{
			Reanimation reanimation = mApp.AddReanimation(mPosX - 73f, mPosY - 56f, mRenderOrder + 2, ReanimationType.Puff);
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_puff);
			mApp.AddTodParticle(mPosX + 110f, mPosY + 0f, mRenderOrder + 1, ParticleEffect.MowerCloud);
			if (mBoard.mPlantRow[mRow] != PlantRowType.Pool)
			{
				DropHead(0u);
				DropArm(0u);
				DropHelm(0u);
				DropShield(0u);
			}
			DieWithLoot();
			return;
		}
		if (mIceTrapCounter > 0)
		{
			RemoveIceTrap();
		}
		if (mButteredCounter > 0)
		{
			mButteredCounter = 0;
		}
		DropShield(0u);
		DropHelm(0u);
		if (mZombieType == ZombieType.Flag)
		{
			DropFlag();
		}
		else if (mZombieType == ZombieType.Polevaulter)
		{
			DropPole();
		}
		else if (mZombieType == ZombieType.Newspaper || mZombieType == ZombieType.Balloon)
		{
			DropHead(0u);
		}
		else if (mZombieType == ZombieType.Pogo)
		{
			DropHead(0u);
			mAltitude = 0f;
		}
		Reanimation reanimation2 = mApp.AddReanimation(0f, 0f, mRenderOrder, ReanimationType.LawnMoweredZombie);
		reanimation2.mIsAttachment = false;
		reanimation2.mLoopType = ReanimLoopType.PlayOnceAndHold;
		reanimation2.mAnimRate = 8f;
		mMoweredReanimID = mApp.ReanimationGetID(reanimation2);
		mZombiePhase = ZombiePhase.ZombieMowered;
		DropLoot();
		mBoard.AreEnemyZombiesOnScreen();
	}

	public void UpdateMowered()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mMoweredReanimID);
		if (reanimation == null || reanimation.mLoopCount > 0)
		{
			DropHead(0u);
			DropArm(0u);
			DieWithLoot();
		}
	}

	public void DropFlag()
	{
		if (mZombieType == ZombieType.Flag && mHasObject)
		{
			DetachFlag();
			mApp.RemoveReanimation(ref mSpecialHeadReanimID);
			mSpecialHeadReanimID = null;
			ReanimShowPrefix("anim_innerarm", 0);
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_flaghand, -1);
			ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_innerarm_screendoor, -1);
			mHasObject = false;
			float thePosX = 0f;
			float thePosY = 0f;
			GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_flaghand, ref thePosX, ref thePosY);
			TodParticleSystem aParticle = mApp.AddTodParticle(thePosX + 6f, thePosY - 45f, mRenderOrder + 1, ParticleEffect.ZombieFlag);
			OverrideParticleColor(aParticle);
			OverrideParticleScale(aParticle);
		}
	}

	public void DropPole()
	{
		if (mZombieType == ZombieType.Polevaulter)
		{
			ReanimShowPrefix("Zombie_polevaulter_innerarm", -1);
			ReanimShowPrefix("Zombie_polevaulter_innerhand", -1);
			ReanimShowPrefix("Zombie_polevaulter_pole", -1);
		}
	}

	public void DrawBossBackArm(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
		float num = 0f;
		float num2 = 0f;
		if (mZombiePhase == ZombiePhase.BossDropRv)
		{
			num2 = (float)(mTargetRow - 1) * 85f - (float)mTargetCol * 20f;
			num = (float)mTargetCol * 80f;
			num *= Constants.S;
			num2 *= Constants.S;
		}
		else if (mZombiePhase == ZombiePhase.BossBungeesEnter || mZombiePhase == ZombiePhase.BossBungeesDrop || mZombiePhase == ZombiePhase.BossBungeesLeave)
		{
			num = (float)mTargetCol * 80f - 23f;
			num *= Constants.S;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		Reanimation reanimation2 = reanimation;
		reanimation2.mOverlayMatrix.mMatrix.M41 = reanimation2.mOverlayMatrix.mMatrix.M41 + num;
		Reanimation reanimation3 = reanimation;
		reanimation3.mOverlayMatrix.mMatrix.M42 = reanimation3.mOverlayMatrix.mMatrix.M42 + num2;
		DrawReanim(g, ref theDrawPos, GameConstants.RENDER_GROUP_BOSS_BACK_ARM);
		Reanimation reanimation4 = reanimation;
		reanimation4.mOverlayMatrix.mMatrix.M41 = reanimation4.mOverlayMatrix.mMatrix.M41 - num;
		Reanimation reanimation5 = reanimation;
		reanimation5.mOverlayMatrix.mMatrix.M42 = reanimation5.mOverlayMatrix.mMatrix.M42 - num2;
	}

	public static void PreloadZombieResources(ZombieType theZombieType)
	{
		ZombieDefinition zombieDefinition = GetZombieDefinition(theZombieType);
		if (zombieDefinition.mReanimationType != ReanimationType.None)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(zombieDefinition.mReanimationType, theIsPreloading: true);
		}
		if (theZombieType == ZombieType.Digger)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.DiggerDirt, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharredDigger, theIsPreloading: true);
		}
		if (theZombieType == ZombieType.Boss)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.BossDriver, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.BossFireball, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.BossIceball, theIsPreloading: true);
			for (int i = 0; i < GameConstants.gBossZombieList.Length; i++)
			{
				ZombieDefinition zombieDefinition2 = GetZombieDefinition(GameConstants.gBossZombieList[i]);
				ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(zombieDefinition2.mReanimationType, theIsPreloading: true);
			}
		}
		if (theZombieType == ZombieType.Dancer)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.BackupDancer, theIsPreloading: true);
		}
		if (theZombieType == ZombieType.Gargantuar || theZombieType == ZombieType.RedeyeGargantuar)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Imp, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharredImp, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharredGargantuar, theIsPreloading: true);
		}
		if (theZombieType == ZombieType.Zamboni)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Imp, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharredZamboni, theIsPreloading: true);
		}
		if (theZombieType == ZombieType.Catapult)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharredCatapult, theIsPreloading: true);
		}
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Puff, theIsPreloading: true);
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharred, theIsPreloading: true);
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.LawnMoweredZombie, theIsPreloading: true);
	}

	public void BossStartDeath()
	{
		mZombiePhase = ZombiePhase.BossHeadLeave;
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_head_leave, ReanimLoopType.PlayOnceAndHold, 0, 24f);
		mApp.AddTodParticle(700f, 150f, 400000, ParticleEffect.BossExplosion);
		mApp.PlaySample(Resources.SOUND_BOSSEXPLOSION);
		mApp.PlayFoley(FoleyType.Gargantudeath);
		BossDie();
	}

	public void RemoveColdEffects()
	{
		if (mIceTrapCounter > 0)
		{
			RemoveIceTrap();
		}
		if (mChilledCounter > 0)
		{
			mChilledCounter = 0;
			UpdateAnimSpeed();
		}
	}

	public void BossHeadSpitEffect()
	{
		int aRenderOrder = mRenderOrder + 2;
		if (mIsFireBall)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			int theTrackIndex = reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_boss_jaw);
			reanimation.GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
			float theX = mPosX + aTransformCurrent.mTransX * Constants.IS + 100f;
			float theY = mPosY + aTransformCurrent.mTransY * Constants.IS + 50f;
			mApp.AddTodParticle(theX, theY, aRenderOrder, ParticleEffect.ZombieBossFireball);
			aTransformCurrent.PrepareForReuse();
		}
		else
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			int theTrackIndex2 = reanimation2.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_boss_jaw);
			reanimation2.GetCurrentTransform(theTrackIndex2, out var aTransformCurrent2, nullIfInvalidFrame: false);
			float theX2 = mPosX + aTransformCurrent2.mTransX * Constants.IS + 100f;
			float theY2 = mPosY + aTransformCurrent2.mTransY * Constants.IS + 50f;
			mApp.AddTodParticle(theX2, theY2, aRenderOrder, ParticleEffect.ZombieBossFireball)?.OverrideImage(null, AtlasResources.IMAGE_ZOMBIE_BOSS_ICEBALL_PARTICLES);
			aTransformCurrent2.PrepareForReuse();
		}
		mApp.PlayFoley(FoleyType.Bossboulderattack);
	}

	public void DrawBossFireBall(Graphics g, ref ZombieDrawPosition theDrawPos)
	{
		MakeParentGraphicsFrame(g);
		Reanimation reanimation = mApp.ReanimationTryToGet(mBossFireBallReanimID);
		if (reanimation != null)
		{
			reanimation.DrawRenderGroup(g, 0);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			reanimation.DrawRenderGroup(g, GameConstants.RENDER_GROUP_BOSS_FIREBALL_ADDITIVE);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
			reanimation.DrawRenderGroup(g, GameConstants.RENDER_GROUP_BOSS_FIREBALL_TOP);
		}
	}

	public void UpdateZombiePeaHead()
	{
		if (!mHasHead)
		{
			return;
		}
		if (mPhaseCounter == 35)
		{
			mApp.ReanimationGet(mSpecialHeadReanimID)?.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 35f);
		}
		else
		{
			if (mPhaseCounter > 0)
			{
				return;
			}
			mApp.ReanimationGet(mSpecialHeadReanimID)?.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.PlayOnceAndHold, 20, 15f);
			mApp.PlayFoley(FoleyType.Throw);
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation != null)
			{
				reanimation.GetCurrentTransform(reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_anim_head1), out var aTransformCurrent, nullIfInvalidFrame: false);
				float num = mPosX + aTransformCurrent.mTransX * Constants.IS - 9f;
				float num2 = mPosY + aTransformCurrent.mTransY * Constants.IS + 6f - mAltitude;
				if (mMindControlled)
				{
					num += 90f * mScaleZombie;
					mBoard.AddProjectile((int)num, (int)num2, mRenderOrder, mRow, ProjectileType.ZombiePeaMindControl);
				}
				else
				{
					mBoard.AddProjectile((int)num, (int)num2, mRenderOrder, mRow, ProjectileType.ZombiePea).mMotionType = ProjectileMotion.Backwards;
				}
				mPhaseCounter = 150;
				aTransformCurrent.PrepareForReuse();
			}
		}
	}

	public void BurnRow(int theRow)
	{
		int theDamageRangeFlags = 127;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie != this && !zombie.mDead)
			{
				int num = zombie.mRow - mRow;
				if (zombie.mZombieType == ZombieType.Boss)
				{
					num = 0;
				}
				if (num == 0 && zombie.EffectedByDamage((uint)theDamageRangeFlags))
				{
					zombie.RemoveColdEffects();
					zombie.ApplyBurn();
				}
			}
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridY == theRow && theGridItem.mGridItemType == GridItemType.Ladder)
			{
				theGridItem.GridItemDie();
			}
		}
	}

	public void UpdateZombieJalapenoHead()
	{
		if (!mHasHead || mPhaseCounter > 0)
		{
			return;
		}
		mApp.PlayFoley(FoleyType.JalapenoIgnite);
		mApp.PlayFoley(FoleyType.Juicy);
		mBoard.DoFwoosh(mRow);
		mBoard.ShakeBoard(3, -4);
		if (mMindControlled)
		{
			BurnRow(mRow);
			return;
		}
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead)
			{
				TRect plantRect = plant.GetPlantRect();
				if (mRow == plant.mRow && !plant.NotOnGround())
				{
					mBoard.mPlantsEaten++;
					plant.Die();
				}
			}
		}
		DieNoLoot(giveAchievements: false);
	}

	public void ApplyBossSmokeParticles(bool theEnable)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_boss_head);
		GlobalMembersAttachment.AttachmentDetachCrossFadeParticleType(ref trackInstanceByName.mAttachmentID, ParticleEffect.ZamboniSmoke, null);
		if (!theEnable)
		{
			return;
		}
		TodParticleSystem theParticleSystem = mApp.AddTodParticle(0f, 0f, 0, ParticleEffect.ZamboniSmoke);
		TodParticleSystem theParticleSystem2 = mApp.AddTodParticle(0f, 0f, 0, ParticleEffect.ZamboniSmoke);
		if (theParticleSystem != null)
		{
			AttachEffect attachEffect = reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_boss_head, ref theParticleSystem, 60f, 30f);
			attachEffect.mDontDrawIfParentHidden = true;
			attachEffect.mDontPropogateColor = true;
		}
		if (theParticleSystem2 != null)
		{
			AttachEffect attachEffect2 = reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_boss_head, ref theParticleSystem2, 100f, 58f);
			attachEffect2.mDontDrawIfParentHidden = true;
			attachEffect2.mDontPropogateColor = true;
		}
		if (mBodyHealth < mBodyMaxHealth / GameConstants.BOSS_FLASH_HEALTH_FRACTION)
		{
			TodParticleSystem theParticleSystem3 = mApp.AddTodParticle(0f, 0f, 0, ParticleEffect.ZamboniSmoke);
			if (theParticleSystem3 != null)
			{
				AttachEffect attachEffect3 = reanimation.AttachParticleToTrack(GlobalMembersReanimIds.ReanimTrackId_boss_head, ref theParticleSystem3, 80f, 27f);
				attachEffect3.mDontDrawIfParentHidden = true;
				attachEffect3.mDontPropogateColor = true;
			}
		}
	}

	public void UpdateZombiquarium()
	{
		if (IsDeadOrDying())
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mZombiePhase == ZombiePhase.ZombiquariumBite)
		{
			if (reanimation.mLoopCount > 0)
			{
				float theAnimRate = TodCommon.RandRangeFloat(8f, 10f);
				PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_aquarium_swim, ReanimLoopType.Loop, 20, theAnimRate);
				mZombiePhase = ZombiePhase.ZombiquariumDrift;
				mPhaseCounter = 100;
			}
		}
		else if (!ZombiquariumFindClosestBrain() && mPhaseCounter == 0)
		{
			int num = RandomNumbers.NextNumber(7);
			if (num <= 4)
			{
				mZombiePhase = ZombiePhase.ZombiquariumAccel;
				mVelZ = TodCommon.RandRangeFloat(0f, (float)Math.PI * 2f);
				mPhaseCounter = TodCommon.RandRangeInt(300, 1000);
				reanimation.mAnimRate = TodCommon.RandRangeFloat(15f, 20f);
			}
			else
			{
				switch (num)
				{
				case 4:
					mZombiePhase = ZombiePhase.ZombiquariumDrift;
					mVelZ = 4.712389f;
					mPhaseCounter = TodCommon.RandRangeInt(300, 1000);
					reanimation.mAnimRate = TodCommon.RandRangeFloat(8f, 10f);
					break;
				case 5:
					mZombiePhase = ZombiePhase.ZombiquariumBackAndForth;
					mVelZ = 0f;
					mPhaseCounter = TodCommon.RandRangeInt(300, 1000);
					reanimation.mAnimRate = TodCommon.RandRangeFloat(15f, 20f);
					break;
				default:
					mZombiePhase = ZombiePhase.ZombiquariumBackAndForth;
					mVelZ = (float)Math.PI;
					mPhaseCounter = TodCommon.RandRangeInt(300, 1000);
					reanimation.mAnimRate = TodCommon.RandRangeFloat(15f, 20f);
					break;
				}
			}
		}
		float num2 = (float)Math.Cos(mVelZ);
		float num3 = (float)Math.Sin(mVelZ);
		bool flag = false;
		if (mPosX < (float)(-Constants.BOARD_EXTRA_ROOM) && num2 < 0f)
		{
			flag = true;
		}
		else if (mPosX > 680f && num2 > 0f)
		{
			flag = true;
		}
		else if (mPosY < 100f && num3 < 0f)
		{
			flag = true;
		}
		else if (mPosY > 400f && num3 > 0f)
		{
			flag = true;
		}
		float val = 0.5f;
		if (flag)
		{
			val = mVelX * 0.3f;
			mPhaseCounter = Math.Min(100, mPhaseCounter);
		}
		else if (mZombiePhase == ZombiePhase.ZombiquariumAccel)
		{
			val = 0.5f;
		}
		else if (mZombiePhase == ZombiePhase.ZombiquariumBackAndForth)
		{
			if (mPosX < 200f && num2 < 0f)
			{
				mVelZ = 0f;
			}
			if (mPosX > 550f && num2 > 0f)
			{
				mVelZ = 3.1416f;
			}
			val = 0.3f;
		}
		else if (mZombiePhase == ZombiePhase.ZombiquariumDrift || mZombiePhase == ZombiePhase.ZombiquariumBite)
		{
			val = 0.05f;
		}
		mVelX = Math.Min(val, mVelX + 0.01f);
		num2 *= mVelX;
		num3 *= mVelX;
		mPosX += num2;
		mPosY += num3;
		if (mBoard.HasLevelAwardDropped())
		{
			return;
		}
		if (mSummonCounter > 0)
		{
			mSummonCounter--;
			if (mSummonCounter == 0)
			{
				mApp.PlayFoley(FoleyType.SpawnSun);
				mBoard.AddCoin(mX + 50, mY + 40, CoinType.Sun, CoinMotion.FromPlant);
				mSummonCounter = TodCommon.RandRangeInt(1000, 1500);
			}
		}
		if (mZombieAge % 100 == 0)
		{
			TakeDamage(10, 8u);
			if (IsDeadOrDying())
			{
				mApp.PlaySample(Resources.SOUND_ZOMBAQUARIUM_DIE);
			}
		}
	}

	public bool ZombiquariumFindClosestBrain()
	{
		if (mBoard.HasLevelAwardDropped())
		{
			return false;
		}
		if (mBodyHealth > 150)
		{
			return false;
		}
		GridItem gridItem = null;
		float num = 0f;
		float num2 = 15f;
		float num3 = 15f;
		float num4 = 50f;
		float num5 = 40f;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Brain && theGridItem.mGridItemCounter >= 15)
			{
				float num6 = TodCommon.Distance2D(theGridItem.mPosX + num2, theGridItem.mPosY + num3, mPosX + num4, mPosY + num5);
				if (gridItem == null || num6 < num)
				{
					num = num6;
					gridItem = theGridItem;
				}
			}
		}
		if (gridItem != null && num < 50f)
		{
			gridItem.GridItemDie();
			mApp.PlayFoley(FoleyType.Slurp);
			mBodyHealth = Math.Min(mBodyMaxHealth, mBodyHealth + 200);
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_aquarium_bite, ReanimLoopType.PlayOnceAndHold, 10, 24f);
			mZombiePhase = ZombiePhase.ZombiquariumBite;
			mPhaseCounter = 200;
			return false;
		}
		if (gridItem != null)
		{
			float num7 = gridItem.mPosX + num2 - (mPosX + num4);
			float num8 = gridItem.mPosY + num3 - (mPosY + num5);
			ref float reference = ref mVelZ;
			reference = (float)Math.Atan2(num8, num7);
			if (reference < 0f)
			{
				reference += (float)Math.PI * 2f;
			}
			mZombiePhase = ZombiePhase.ZombiquariumAccel;
			return true;
		}
		return false;
	}

	public void UpdateZombieGatlingHead()
	{
		if (!mHasHead)
		{
			return;
		}
		if (mPhaseCounter == 100)
		{
			mApp.ReanimationGet(mSpecialHeadReanimID).PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 38f);
		}
		else if (mPhaseCounter == 18 || mPhaseCounter == 35 || mPhaseCounter == 51 || mPhaseCounter == 68)
		{
			mApp.PlayFoley(FoleyType.Throw);
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation != null)
			{
				int theTrackIndex = reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
				reanimation.GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
				float num = mPosX + aTransformCurrent.mTransX * Constants.IS - 9f;
				float num2 = mPosY + aTransformCurrent.mTransY * Constants.IS + 6f;
				if (mMindControlled)
				{
					num += 90f * mScaleZombie;
					mBoard.AddProjectile((int)num, (int)num2, mRenderOrder, mRow, ProjectileType.ZombiePeaMindControl);
				}
				else
				{
					mBoard.AddProjectile((int)num, (int)num2, mRenderOrder, mRow, ProjectileType.ZombiePea).mMotionType = ProjectileMotion.Backwards;
				}
				aTransformCurrent.PrepareForReuse();
			}
		}
		else if (mPhaseCounter <= 0)
		{
			mApp.ReanimationGet(mSpecialHeadReanimID).PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.PlayOnceAndHold, 20, 15f);
			mPhaseCounter = 150;
		}
	}

	public void UpdateZombieSquashHead()
	{
		float num = 6f;
		float num2 = -21f;
		if (mHasHead && mIsEating && mZombiePhase == ZombiePhase.SquashPreLaunch)
		{
			StopEating();
			PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 12f);
			mHasHead = false;
			Reanimation reanimation = mApp.ReanimationGet(mSpecialHeadReanimID);
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_jumpup, ReanimLoopType.PlayOnceAndHold, 20, 24f);
			reanimation.mRenderOrder = mRenderOrder + 1;
			reanimation.SetPosition((mPosX + num) * Constants.S, (mPosY + num2) * Constants.S);
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			ReanimatorTrackInstance trackInstanceByName = reanimation2.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
			GlobalMembersAttachment.AttachmentDetach(ref trackInstanceByName.mAttachmentID);
			reanimation.mOverlayMatrix.mMatrix.M11 = 0.75f;
			reanimation.mOverlayMatrix.mMatrix.M12 = 0f;
			reanimation.mOverlayMatrix.mMatrix.M22 = 0.75f;
			reanimation.mOverlayMatrix.mMatrix.M21 = 0f;
			mZombiePhase = ZombiePhase.SquashRising;
			mPhaseCounter = 95;
		}
		if (mZombiePhase == ZombiePhase.SquashRising)
		{
			int num3 = mBoard.GridToPixelX(mBoard.PixelToGridXKeepOnBoard(mX, mY), mRow);
			int num4 = TodCommon.TodAnimateCurve(50, 20, mPhaseCounter, 0, num3 - (int)mPosX, TodCurves.EaseInOut);
			int num5 = TodCommon.TodAnimateCurve(50, 20, mPhaseCounter, 0, -20, TodCurves.EaseInOut);
			Reanimation reanimation3 = mApp.ReanimationGet(mSpecialHeadReanimID);
			reanimation3.SetPosition((mPosX + num + (float)num4) * Constants.S, (mPosY + num2 + (float)num5) * Constants.S);
			if (mPhaseCounter <= 0)
			{
				reanimation3 = mApp.ReanimationGet(mSpecialHeadReanimID);
				reanimation3.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_jumpdown, ReanimLoopType.PlayOnceAndHold, 0, 60f);
				mZombiePhase = ZombiePhase.SquashFalling;
				mPhaseCounter = 10;
			}
		}
		if (mZombiePhase == ZombiePhase.SquashFalling)
		{
			int num6 = TodCommon.TodAnimateCurve(10, 0, mPhaseCounter, -20, 74, TodCurves.Linear);
			int num7 = mBoard.GridToPixelX(mBoard.PixelToGridXKeepOnBoard(mX, mY), mRow);
			Reanimation reanimation4 = mApp.ReanimationGet(mSpecialHeadReanimID);
			reanimation4.SetPosition((mPosX + num + (float)num7 - mPosX) * Constants.S, (mPosY + num2 + (float)num6) * Constants.S);
			if (mPhaseCounter == 2)
			{
				SquishAllInSquare(mBoard.PixelToGridXKeepOnBoard(mX, mY), mRow, ZombieAttackType.Chew);
			}
			if (mPhaseCounter <= 0)
			{
				mZombiePhase = ZombiePhase.SquashDoneFalling;
				mPhaseCounter = 100;
				mBoard.ShakeBoard(1, 4);
				mApp.PlayFoley(FoleyType.Thump);
			}
		}
		if (mZombiePhase == ZombiePhase.SquashDoneFalling && mPhaseCounter <= 0)
		{
			Reanimation reanimation5 = mApp.ReanimationGet(mSpecialHeadReanimID);
			reanimation5.ReanimationDie();
			mSpecialHeadReanimID = null;
			TakeDamage(1800, 9u);
		}
	}

	public bool IsTanglekelpTarget()
	{
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mState == PlantState.TanglekelpGrabbing && plant.mTargetZombieID == mBoard.ZombieGetID(this))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasYuckyFaceImage()
	{
		if (!mBoard.mFutureMode)
		{
			if (mZombieType != ZombieType.Normal && mZombieType != ZombieType.TrafficCone && mZombieType != ZombieType.Pail && mZombieType != ZombieType.Flag && mZombieType != ZombieType.Door && mZombieType != ZombieType.DuckyTube && mZombieType != ZombieType.Dancer && mZombieType != ZombieType.BackupDancer && mZombieType != ZombieType.Newspaper)
			{
				return mZombieType == ZombieType.Polevaulter;
			}
			return true;
		}
		return false;
	}

	public bool IsSquashTarget(Plant exceptMe)
	{
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant != exceptMe && plant.mSeedType == SeedType.Squash && plant.mTargetZombieID == mBoard.ZombieGetID(this))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsTangleKelpTarget()
	{
		if (!mApp.mBoard.StageHasPool())
		{
			return false;
		}
		if (mZombieHeight == ZombieHeight.DraggedUnder)
		{
			return true;
		}
		for (int i = 0; i < mBoard.mPlants.Count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mSeedType == SeedType.Tanglekelp && plant.mTargetZombieID == mBoard.ZombieGetID(this))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsFireResistant()
	{
		if (mZombieType != ZombieType.Catapult && mZombieType != ZombieType.Zamboni)
		{
			if (mShieldType != ShieldType.Door)
			{
				return mShieldType == ShieldType.Ladder;
			}
			return true;
		}
		return true;
	}

	public void EnableMustache(bool theEnableMustache)
	{
		if (mFromWave == GameConstants.ZOMBIE_WAVE_UI || !mHasHead)
		{
			return;
		}
		switch (mZombieType)
		{
		case ZombieType.Football:
		case ZombieType.JackInTheBox:
		case ZombieType.PeaHead:
		case ZombieType.WallnutHead:
		case ZombieType.JalapenoHead:
		case ZombieType.GatlingHead:
		case ZombieType.SquashHead:
		case ZombieType.TallnutHead:
		case ZombieType.Monk:
		case ZombieType.FootballPremium:
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null || !reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_zombie_mustache))
		{
			return;
		}
		if (!theEnableMustache)
		{
			reanimation.AssignRenderGroupToPrefix("Zombie_mustache", -1);
			return;
		}
		reanimation.AssignRenderGroupToPrefix("Zombie_mustache", 0);
		switch (TodCommon.RandRangeInt(1, 3))
		{
		case 1:
		{
			Image theImage = null;
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_mustache, theImage);
			break;
		}
		case 2:
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_mustache, AtlasResources.IMAGE_REANIM_ZOMBIE_MUSTACHE2);
			break;
		case 3:
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_zombie_mustache, AtlasResources.IMAGE_REANIM_ZOMBIE_MUSTACHE3);
			break;
		}
	}

	public void EnableFuture(bool theEnableFuture)
	{
		if (mFromWave == GameConstants.ZOMBIE_WAVE_UI || mZombieType == ZombieType.PeaHead || mZombieType == ZombieType.WallnutHead || mZombieType == ZombieType.JalapenoHead || mZombieType == ZombieType.GatlingHead || mZombieType == ZombieType.SquashHead || mZombieType == ZombieType.TallnutHead)
		{
			return;
		}
		ZombieType zombieType = mZombieType;
		if (zombieType == ZombieType.Football || zombieType == ZombieType.JackInTheBox || (uint)(zombieType - 35) <= 1u)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null || reanimation.mReanimationType != ReanimationType.Zombie)
		{
			return;
		}
		if (!theEnableFuture)
		{
			Image theImage = null;
			reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, theImage);
			return;
		}
		int num = RandomNumbers.NextNumber(8) % 4;
		Image theImage2 = null;
		switch (num)
		{
		case 0:
			theImage2 = AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES1;
			break;
		case 1:
			theImage2 = AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES2;
			break;
		case 2:
			theImage2 = AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES3;
			break;
		case 3:
			theImage2 = AtlasResources.IMAGE_REANIM_ZOMBIE_HEAD_SUNGLASSES4;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, theImage2);
	}

	public void BungeeDropPlant()
	{
		if (mZombiePhase != ZombiePhase.BungeeGrabbing)
		{
			return;
		}
		Plant plant = (mBoard.mPlants.Contains(mTargetPlantID) ? mTargetPlantID : null);
		if (plant != null)
		{
			if (plant.mOnBungeeState == PlantOnBungeeState.GettingGrabbedByBungee)
			{
				plant.mOnBungeeState = PlantOnBungeeState.NotOnBungee;
			}
			else if (plant.mOnBungeeState == PlantOnBungeeState.RisingWithBungee)
			{
				plant.Die();
			}
			mTargetPlantID = null;
		}
	}

	public void RemoveButter()
	{
		if (mZombieType == ZombieType.Balloon || mZombieType == ZombieType.Propeller)
		{
			BalloonPropellerHatSpin(theSpinning: true);
		}
		if (mZombieType == ZombieType.PeaHead || mZombieType == ZombieType.WallnutHead || mZombieType == ZombieType.TallnutHead || mZombieType == ZombieType.JalapenoHead || mZombieType == ZombieType.GatlingHead || mZombieType == ZombieType.SquashHead)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mSpecialHeadReanimID);
			if (reanimation != null)
			{
				if (mZombieType == ZombieType.PeaHead && reanimation.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_shooting))
				{
					reanimation.mAnimRate = 35f;
				}
				else if (mZombieType == ZombieType.GatlingHead && reanimation.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_shooting))
				{
					reanimation.mAnimRate = 38f;
				}
				else
				{
					reanimation.mAnimRate = 15f;
				}
			}
		}
		UpdateAnimSpeed();
		StartZombieSound();
	}

	public void BalloonPropellerHatSpin(bool theSpinning)
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		ReanimatorTrackInstance reanimatorTrackInstance = ((mZombieType == ZombieType.Propeller) ? reanimation.GetTrackInstanceByName("Zombie_propeller_duckytube_whole_fan") : reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_hat));
		Reanimation reanimation2 = GlobalMembersAttachment.FindReanimAttachment(reanimatorTrackInstance.mAttachmentID);
		if (reanimation2 != null)
		{
			if (theSpinning)
			{
				reanimation2.mAnimRate = reanimation2.mDefinition.mFPS;
			}
			else
			{
				reanimation2.mAnimRate = 0f;
			}
		}
	}

	public void DoDaisies()
	{
		if (!IsWalkingBackwards() && mBoard.mPlantRow[mRow] != PlantRowType.Pool && !mBoard.StageHasRoof())
		{
			float num = 20f;
			float num2 = 100f;
			switch (mZombieType)
			{
			case ZombieType.Zamboni:
			case ZombieType.Bobsled:
			case ZombieType.Catapult:
				return;
			case ZombieType.Football:
			case ZombieType.Dancer:
			case ZombieType.BackupDancer:
			case ZombieType.FootballPremium:
				num += 160f;
				break;
			case ZombieType.Pogo:
				num2 += 120f;
				break;
			case ZombieType.Balloon:
				num2 += 30f;
				num += 110f;
				break;
			}
			if (mBoard.StageHasGraveStones())
			{
				num2 += 15f;
			}
			mApp.AddTodParticle((float)mX + Constants.InvertAndScale(num), (float)mY + Constants.InvertAndScale(num2), Board.MakeRenderOrder(RenderLayer.GraveStone, mRow, 5), ParticleEffect.Daisy);
		}
	}

	public void EnableDanceMode(bool theEnableDance)
	{
		if (mFromWave != GameConstants.ZOMBIE_WAVE_UI && mFromWave != GameConstants.ZOMBIE_WAVE_CUTSCENE && !ZombieNotWalking() && !IsDeadOrDying() && (mZombieType == ZombieType.Normal || mZombieType == ZombieType.TrafficCone || mZombieType == ZombieType.Pail))
		{
			StartWalkAnim(0);
		}
	}

	public void BungeeLiftTarget()
	{
		PlayZombieReanim(ref GlobalMembersReanimIds.ReanimTrackId_anim_raise, ReanimLoopType.PlayOnceAndHold, 0, 36f);
		if (mTargetPlantID == null)
		{
			return;
		}
		Plant plant = mTargetPlantID;
		if (plant == null)
		{
			return;
		}
		for (int i = 0; i < mBoard.mZombies.Count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mZombieType == ZombieType.Bungee && zombie != this && zombie.mTargetPlantID == plant)
			{
				zombie.mTargetPlantID = null;
			}
		}
		plant.mOnBungeeState = PlantOnBungeeState.RisingWithBungee;
		mApp.PlayFoley(FoleyType.Floop);
		Reanimation reanimation = mApp.ReanimationTryToGet(plant.mBodyReanimID);
		if (reanimation != null)
		{
			reanimation.mAnimRate = 0.1f;
		}
		if ((plant.mSeedType == SeedType.Cattail || plant.mSeedType == SeedType.HypnoCattail) && mBoard.GetTopPlantAt(mTargetCol, mRow, TopPlant.OnlyPumpkin) != null)
		{
			mBoard.NewPlant(mTargetCol, mRow, SeedType.Lilypad, SeedType.None);
		}
		Plant plant2 = null;
		if ((plant.mSeedType == SeedType.Cattail || plant.mSeedType == SeedType.HypnoCattail) && (plant2 = mBoard.GetFlowerPotAt(mTargetCol, mRow)) != null)
		{
			plant2.mState = PlantState.Notready;
		}
		if (mApp.IsIZombieLevel())
		{
			mBoard.mChallenge.IZombiePlantDropRemainingSun(plant);
		}
	}

	public void SetupWaterTrack(ref string theTrackName)
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(theTrackName);
		trackInstanceByName.mIgnoreExtraAdditiveColor = true;
		trackInstanceByName.mIgnoreColorOverride = true;
		trackInstanceByName.mIgnoreClipRect = true;
	}

	public static int GetBaseBodyHealth(ZombieType type, LawnApp app)
	{
		switch (type)
		{
		case ZombieType.Zamboni:
		case ZombieType.Yeti:
			return 1350;
		case ZombieType.Polevaulter:
		case ZombieType.Dancer:
		case ZombieType.DolphinRider:
		case ZombieType.JackInTheBox:
		case ZombieType.Pogo:
		case ZombieType.Ladder:
		case ZombieType.JalapenoHead:
			return 500;
		case ZombieType.Bungee:
			return 450;
		case ZombieType.Gargantuar:
			return 3000;
		case ZombieType.RedeyeGargantuar:
			return 6000;
		case ZombieType.Catapult:
			return 850;
		case ZombieType.RobotTitan:
		case ZombieType.RedeyeRobotTitan:
			return 750;
		case ZombieType.Monk:
		case ZombieType.Ninja:
		case ZombieType.Talisman:
		case ZombieType.Propeller:
			return 540;
		case ZombieType.Imp:
			if (app.IsIZombieLevel())
			{
				return 70;
			}
			break;
		case ZombieType.Boss:
			if (app.IsAdventureMode() || app.IsQuickPlayMode())
			{
				return 40000;
			}
			if (app.mGameMode == GameMode.ChallengeFinalBoss2)
			{
				return 30000;
			}
			return 60000;
		}
		return 270;
	}

	public static int GetBaseHelmHealth(ZombieType type)
	{
		switch (type)
		{
		case ZombieType.TrafficCone:
			return 370;
		case ZombieType.Pail:
		case ZombieType.WallnutHead:
			return 1100;
		case ZombieType.Football:
			return 1400;
		case ZombieType.FootballPremium:
			return 2800;
		case ZombieType.Digger:
			return 100;
		case ZombieType.Bobsled:
			return 300;
		case ZombieType.TallnutHead:
			return 2200;
		case ZombieType.RobotTitan:
			return 2250;
		case ZombieType.RedeyeRobotTitan:
			return 5250;
		default:
			return 0;
		}
	}

	public static int GetBaseShieldHealth(ZombieType type)
	{
		return type switch
		{
			ZombieType.Door => 1100, 
			ZombieType.Ladder => 500, 
			ZombieType.Newspaper => 150, 
			_ => 0, 
		};
	}

	public static int GetBaseFlyingHealth(ZombieType type)
	{
		return type switch
		{
			ZombieType.Balloon => 20, 
			ZombieType.Propeller => 180, 
			_ => 0, 
		};
	}

	public static ZombieDefinition GetZombieDefinition(ZombieType theZombieType)
	{
		Debug.ASSERT(theZombieType >= ZombieType.Normal && theZombieType < ZombieType.ZombieTypesCount);
		Debug.ASSERT(GameConstants.gZombieDefs[(int)theZombieType].mZombieType == theZombieType);
		return GameConstants.gZombieDefs[(int)theZombieType];
	}

	public static bool ZombieTypeCanGoInPool(ZombieType theZombieType)
	{
		if (GlobalStaticVars.gLawnApp.IsLevelUseJson())
		{
			CSOverrideZombieProperty component = GlobalStaticVars.gLawnApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
			if (component != null && component.mProperties.TryGetValue(theZombieType, out var value) && value.mCanGoInPool.HasValue)
			{
				return value.mCanGoInPool == true;
			}
		}
		return ZombieTypeCanGoInPoolDefault(theZombieType);
	}

	public static bool ZombieTypeCanGoInPoolDefault(ZombieType theZombieType)
	{
		if (theZombieType != ZombieType.Normal && theZombieType != ZombieType.TrafficCone && theZombieType != ZombieType.Pail && theZombieType != ZombieType.Flag && theZombieType != ZombieType.Snorkel && theZombieType != ZombieType.DolphinRider && theZombieType != ZombieType.PeaHead && theZombieType != ZombieType.WallnutHead && theZombieType != ZombieType.JalapenoHead && theZombieType != ZombieType.GatlingHead && theZombieType != ZombieType.TallnutHead && theZombieType != ZombieType.Balloon && theZombieType != ZombieType.Propeller)
		{
			return ZombieTypeCanGoInPoolExtended(theZombieType);
		}
		return true;
	}

	public static bool ZombieTypeCanGoInPoolExtended(ZombieType theZombieType)
	{
		if (theZombieType != ZombieType.Football && theZombieType != ZombieType.FootballPremium)
		{
			return theZombieType == ZombieType.JackInTheBox;
		}
		return true;
	}

	public static bool ZombieTypeCanGoOnHighGround(ZombieType theZombieType)
	{
		if (GlobalStaticVars.gLawnApp.IsLevelUseJson())
		{
			CSOverrideZombieProperty component = GlobalStaticVars.gLawnApp.mCreativeLevel.GetComponent<CSOverrideZombieProperty>();
			if (component != null && component.mProperties.TryGetValue(theZombieType, out var value) && value.mCanGoOnHighGround.HasValue)
			{
				return value.mCanGoOnHighGround == true;
			}
		}
		return ZombieTypeCanGoOnHighGroundDefault(theZombieType);
	}

	public static bool ZombieTypeCanGoOnHighGroundDefault(ZombieType theZombieType)
	{
		if (theZombieType != ZombieType.Zamboni)
		{
			return theZombieType != ZombieType.Bobsled;
		}
		return false;
	}
}
