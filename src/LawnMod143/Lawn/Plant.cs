using System;
using System.Collections.Generic;
using Lawn.Creative;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Plant : GameObject
{
	private const int DefaultHealth = 300;

	public SeedType mSeedType;

	public int mPlantCol;

	public int mAnimCounter;

	public int mFrame;

	public int mFrameLength;

	public int mNumFrames;

	public PlantState mState;

	public int mPlantHealth;

	public int mPlantMaxHealth;

	public int mSubclass;

	public int mDisappearCountdown;

	public int mDoSpecialCountdown;

	public int mStateCountdown;

	public int mLaunchCounter;

	public int mLaunchRate;

	public TRect mPlantRect;

	public TRect mPlantAttackRect;

	public int mTargetX;

	public int mTargetY;

	public int mStartRow;

	public TodParticleSystem mParticleID;

	public int mParticleID_Save;

	public int mShootingCounter;

	public Reanimation mBodyReanimID;

	public int mBodyReanimID_Save;

	public Reanimation mHeadReanimID;

	public int mHeadReanimID_Save;

	public Reanimation mHeadReanimID2;

	public int mHeadReanimID2_Save;

	public Reanimation mHeadReanimID3;

	public int mHeadReanimID3_Save;

	public Reanimation mBlinkReanimID;

	public int mBlinkReanimID_Save;

	public Reanimation mLightReanimID;

	public int mLightReanimID_Save;

	public Reanimation mSleepingReanimID;

	public int mSleepingReanimID_Save;

	public int mBlinkCountdown;

	public int mRecentlyEatenCountdown;

	public int mEatenFlashCountdown;

	public int mBeghouledFlashCountdown;

	public float mShakeOffsetX;

	public float mShakeOffsetY;

	public MagnetItem[] mMagnetItems = new MagnetItem[5];

	public Zombie mTargetZombieID;

	// Iris Shooter (iOS port original plant): Face-ID style scan, then an armour-ignoring laser.
	public int mIrisScanCounter;

	public int mIrisTickCounter;

	public bool mIrisUnlocked;

	public int mIrisFlashCounter;

	public int mIrisEngage;

	public int mTargetZombieID_Save;

	private int mTargetZombieIDSaved;

	public int mWakeUpCounter;

	public PlantOnBungeeState mOnBungeeState;

	public SeedType mImitaterType;

	public int mPottedPlantIndex;

	public bool mAnimPing;

	public bool mDead;

	public bool mSquished;

	public bool mIsAsleep;

	public bool mIsOnBoard;

	public bool mHighlighted;

	public bool mInFlowerPot;

	private static Stack<Plant> unusedObjects = new Stack<Plant>();

	public bool mGloveGrabbed;

	public Reanimation mAttackingReanimID;

	public int mAttackingReanimID_Save;

	public int mSealedCountdown;

	public Reanimation mSealedReanimID;

	public int mSealedReanimID_Save;

	public int mInTalismanCounter;

	private string lastPlayedBodyReanim_Name = string.Empty;

	private ReanimLoopType lastPlayedBodyReanim_Type;

	private byte lastPlayedBodyReanim_BlendTime;

	private float lastPlayedBodyReanim_AnimRate;

	public DropLootType mGravebusterDrop;

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncEnum(ref mSeedType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncInt(ref mPlantCol);
		theContext.SyncInt(ref mAnimCounter);
		theContext.SyncInt(ref mFrame);
		theContext.SyncInt(ref mFrameLength);
		theContext.SyncInt(ref mNumFrames);
		theContext.SyncEnum(ref mState, theContext.aPlantStateSaver, SaveFileVersion.Initial, PlantState.Notready);
		theContext.SyncInt(ref mPlantHealth);
		theContext.SyncInt(ref mPlantMaxHealth);
		theContext.SyncInt(ref mSubclass);
		theContext.SyncInt(ref mDisappearCountdown);
		theContext.SyncInt(ref mDoSpecialCountdown);
		theContext.SyncInt(ref mStateCountdown);
		theContext.SyncInt(ref mLaunchCounter);
		theContext.SyncInt(ref mLaunchRate);
		theContext.SyncTRect(ref mPlantRect);
		theContext.SyncTRect(ref mPlantAttackRect);
		theContext.SyncInt(ref mTargetX);
		theContext.SyncInt(ref mTargetY);
		theContext.SyncInt(ref mStartRow);
		theContext.SyncInt(ref mParticleID_Save);
		theContext.SyncInt(ref mShootingCounter);
		theContext.SyncInt(ref mBodyReanimID_Save);
		theContext.SyncInt(ref mHeadReanimID_Save);
		theContext.SyncInt(ref mHeadReanimID2_Save);
		theContext.SyncInt(ref mHeadReanimID3_Save);
		theContext.SyncInt(ref mBlinkReanimID_Save);
		theContext.SyncInt(ref mLightReanimID_Save);
		theContext.SyncInt(ref mSleepingReanimID_Save);
		theContext.SyncInt(ref mBlinkCountdown);
		theContext.SyncInt(ref mRecentlyEatenCountdown);
		theContext.SyncInt(ref mEatenFlashCountdown);
		theContext.SyncInt(ref mBeghouledFlashCountdown);
		theContext.SyncFloat(ref mShakeOffsetX);
		theContext.SyncFloat(ref mShakeOffsetY);
		for (int i = 0; i < 5; i++)
		{
			bool theBool = mMagnetItems[i] != null;
			theContext.SyncBool(ref theBool);
			if (theBool)
			{
				MagnetItem[] array = mMagnetItems;
				int num = i;
				if (array[num] == null)
				{
					array[num] = new MagnetItem();
				}
				mMagnetItems[i].Sync(theContext);
			}
		}
		theContext.SyncInt(ref mTargetZombieID_Save);
		theContext.SyncInt(ref mWakeUpCounter);
		theContext.SyncEnum(ref mOnBungeeState, theContext.aPlantOnBungeeStateSaver, SaveFileVersion.Initial, PlantOnBungeeState.NotOnBungee);
		theContext.SyncEnum(ref mImitaterType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncInt(ref mPottedPlantIndex);
		theContext.SyncBool(ref mAnimPing);
		theContext.SyncBool(ref mDead);
		theContext.SyncBool(ref mSquished);
		theContext.SyncBool(ref mIsAsleep);
		theContext.SyncBool(ref mIsOnBoard);
		theContext.SyncBool(ref mHighlighted);
		theContext.SyncBool(ref mInFlowerPot);
		theContext.SyncBool(ref mGloveGrabbed);
		theContext.SyncInt(ref mAttackingReanimID_Save, SaveFileVersion.RecordAppVersionNumber, -1);
		theContext.SyncInt(ref mSealedCountdown, SaveFileVersion.Talisman);
		theContext.SyncInt(ref mSealedReanimID_Save, SaveFileVersion.Talisman, -1);
		theContext.SyncInt(ref mInTalismanCounter, SaveFileVersion.Talisman);
	}

	internal override void SyncObj(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.SyncObj(theContext);
		TodLibObjSyncer.SyncObjFromList(ref mParticleID_Save, ref mParticleID, mApp.mEffectSystem.mParticleHolder.mParticleSystems, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mBodyReanimID_Save, ref mBodyReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mHeadReanimID_Save, ref mHeadReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mHeadReanimID2_Save, ref mHeadReanimID2, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mHeadReanimID3_Save, ref mHeadReanimID3, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mBlinkReanimID_Save, ref mBlinkReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mLightReanimID_Save, ref mLightReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mSleepingReanimID_Save, ref mSleepingReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mTargetZombieID_Save, ref mTargetZombieID, mBoard.mZombies, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mAttackingReanimID_Save, ref mAttackingReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mSealedReanimID_Save, ref mSealedReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
	}

	public static void PreallocateMemory()
	{
		for (int i = 0; i < 100; i++)
		{
			new Plant().PrepareForReuse();
		}
	}

	public static Plant GetNewPlant()
	{
		if (unusedObjects.Count > 0)
		{
			Plant plant = unusedObjects.Pop();
			plant.Reset();
			return plant;
		}
		return new Plant();
	}

	public override void PrepareForReuse()
	{
		unusedObjects.Push(this);
	}

	protected override void Reset()
	{
		base.Reset();
		lastPlayedBodyReanim_Name = string.Empty;
		lastPlayedBodyReanim_Type = ReanimLoopType.PlayOnce;
		lastPlayedBodyReanim_BlendTime = 0;
		lastPlayedBodyReanim_AnimRate = 0f;
		mSeedType = SeedType.Peashooter;
		mPlantCol = 0;
		mAnimCounter = 0;
		mFrame = 0;
		mFrameLength = 0;
		mNumFrames = 0;
		mState = PlantState.Notready;
		mPlantHealth = 0;
		mPlantMaxHealth = 0;
		mSubclass = 0;
		mDisappearCountdown = 0;
		mDoSpecialCountdown = 0;
		mStateCountdown = 0;
		mLaunchCounter = 0;
		mLaunchRate = 0;
		mPlantRect = default;
		mPlantAttackRect = default;
		mTargetX = 0;
		mTargetY = 0;
		mStartRow = 0;
		mParticleID = null;
		mShootingCounter = 0;
		mBodyReanimID = null;
		mHeadReanimID = null;
		mHeadReanimID2 = null;
		mHeadReanimID3 = null;
		mBlinkReanimID = null;
		mLightReanimID = null;
		mSleepingReanimID = null;
		mSealedReanimID = null;
		mSealedCountdown = 0;
		mInTalismanCounter = 0;
		mBlinkCountdown = 0;
		mRecentlyEatenCountdown = 0;
		mEatenFlashCountdown = 0;
		mBeghouledFlashCountdown = 0;
		mShakeOffsetX = 0f;
		mShakeOffsetY = 0f;
		for (int i = 0; i < mMagnetItems.Length; i++)
		{
			mMagnetItems[i] = null;
		}
		mTargetZombieID = null;
		mIrisScanCounter = 0;
		mIrisTickCounter = 0;
		mIrisUnlocked = false;
		mIrisFlashCounter = 0;
		mIrisEngage = 0;
		mWakeUpCounter = 0;
		mOnBungeeState = PlantOnBungeeState.NotOnBungee;
		mImitaterType = SeedType.Peashooter;
		mPottedPlantIndex = 0;
		mAnimPing = false;
		mDead = false;
		mSquished = false;
		mIsAsleep = false;
		mIsOnBoard = false;
		mHighlighted = false;
		mInFlowerPot = false;
		mGloveGrabbed = false;
		mAttackingReanimID = null;
	}

	private Plant()
	{
		Reset();
	}

	public void PlantInitialize(int theGridX, int theGridY, SeedType theSeedType, SeedType theImitaterType)
	{
		for (int i = 0; i < mMagnetItems.Length; i++)
		{
			if (mMagnetItems[i] == null)
			{
				mMagnetItems[i] = new MagnetItem();
			}
			else
			{
				mMagnetItems[i].Reset();
			}
		}
		mPlantCol = theGridX;
		mRow = theGridY;
		if (mBoard != null)
		{
			mX = mBoard.GridToPixelX(theGridX, theGridY);
			mY = mBoard.GridToPixelY(theGridX, theGridY);
			if (mApp.mGameMode == GameMode.ChallengeZenGarden)
			{
				mY -= Constants.ZenGardenGreenhouseOffset.Y;
			}
		}
		mAnimCounter = 0;
		mAnimPing = true;
		mFrame = 0;
		mShootingCounter = 0;
		mFrameLength = TodCommon.RandRangeInt(12, 18);
		mNumFrames = 5;
		mState = PlantState.Notready;
		mDead = false;
		mSquished = false;
		mSeedType = theSeedType;
		mImitaterType = theImitaterType;
		mPlantHealth = 300;
		mDoSpecialCountdown = 0;
		mDisappearCountdown = 200;
		mTargetX = -1;
		mTargetY = -1;
		mStateCountdown = 0;
		mStartRow = mRow;
		mParticleID = null;
		mBodyReanimID = null;
		mHeadReanimID = null;
		mHeadReanimID2 = null;
		mHeadReanimID3 = null;
		mBlinkReanimID = null;
		mLightReanimID = null;
		mSleepingReanimID = null;
		mSealedReanimID = null;
		mSealedCountdown = 0;
		mInTalismanCounter = 0;
		mAttackingReanimID = null;
		mBlinkCountdown = 0;
		mRecentlyEatenCountdown = 0;
		mEatenFlashCountdown = 0;
		mBeghouledFlashCountdown = 0;
		mWidth = 80;
		mHeight = 80;
		mShakeOffsetX = 0f;
		mShakeOffsetY = 0f;
		mIsAsleep = false;
		mWakeUpCounter = 0;
		mOnBungeeState = PlantOnBungeeState.NotOnBungee;
		mPottedPlantIndex = -1;
		PlantDefinition plantDefinition = GetPlantDefinition(theSeedType);
		mLaunchRate = plantDefinition.mLaunchRate;
		if (mApp.IsLevelUseJson())
		{
			CSOverridePlantProperty component = mApp.mCreativeLevel.GetComponent<CSOverridePlantProperty>();
			if (component != null && component.mProperties.TryGetValue(mSeedType, out var value) && value.mLaunchRate.HasValue)
			{
				mLaunchRate = value.mLaunchRate.GetValueOrDefault();
			}
		}
		mSubclass = (int)plantDefinition.mSubClass;
		mRenderOrder = CalcRenderOrder();
		Reanimation theAttachReanim = null;
		string empty = string.Empty;
		ReanimationType reanimationType = plantDefinition.mReanimationType;
		if (mSeedType == SeedType.Cattail && mApp.IsCattailSkinEnabled())
		{
			reanimationType = ReanimationType.HypnoCattail;
		}
		if (plantDefinition.mReanimationType != ReanimationType.None)
		{
			float theY = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
			theAttachReanim = mApp.AddReanimation(0f, theY, mRenderOrder + 1, reanimationType);
			theAttachReanim.mLoopType = ReanimLoopType.Loop;
			theAttachReanim.mAnimRate = TodCommon.RandRangeFloat(10f, 15f);
			if (theSeedType == SeedType.Spikeweed || theSeedType == SeedType.Agave || (theAttachReanim.mDefinition.mIsSpine && theAttachReanim.mDefinition.mFPS == 30f))
			{
				theAttachReanim.mAnimRate = 30f;
			}
			if (theAttachReanim.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
			{
				theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
			}
			if (mApp.mGameMode == GameMode.ChallengeZenGarden && theAttachReanim.TrackExists("anim_garden_idle"))
			{
				theAttachReanim.SetFramesForLayer("anim_garden_idle");
			}
			if ((mApp.IsWallnutBowlingLevel() || mSeedType == SeedType.GiantWallnut) && theAttachReanim.TrackExists(Reanimation.ReanimTrackId__ground))
			{
				theAttachReanim.SetFramesForLayer(Reanimation.ReanimTrackId__ground);
				if (mSeedType == SeedType.Wallnut || mSeedType == SeedType.ExplodeONut)
				{
					theAttachReanim.mAnimRate = TodCommon.RandRangeFloat(12f, 18f);
				}
				else if (mSeedType == SeedType.GiantWallnut)
				{
					theAttachReanim.mAnimRate = TodCommon.RandRangeFloat(6f, 10f);
				}
			}
			theAttachReanim.mIsAttachment = true;
			mBodyReanimID = mApp.ReanimationGetID(theAttachReanim);
			mBlinkCountdown = 400 + RandomNumbers.NextNumber(400);
		}
		if (IsInPlay() && theAttachReanim.TrackExists("anim_enter") && !mApp.IsIZombieLevel())
		{
			theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
			theAttachReanim.SetFramesForLayer("anim_enter");
			mState = PlantState.Entering;
		}
		if (IsNocturnal(mSeedType) && mBoard != null && !mBoard.StageIsNight())
		{
			if (mSeedType == SeedType.Hypnoshroom && IsInPlay())
			{
				mBoard.DoSleepingSound(this);
				PlayBodyReanim("anim_idle_to_sleep", ReanimLoopType.Loop, 0, 0f);
				mIsAsleep = true;
			}
			else
			{
				if (IsInPlay())
				{
					mBoard.DoSleepingSound(this);
				}
				if (IsInPlay() && DoAdvancedSleeping(mSeedType) && theAttachReanim.TrackExists("anim_idle_to_sleep"))
				{
					PlayBodyReanim("anim_idle_to_sleep", ReanimLoopType.PlayOnceAndHold, 0, 0f);
					mIsAsleep = true;
				}
				else
				{
					SetSleeping(theIsAsleep: true);
				}
			}
		}
		if (mLaunchRate > 0)
		{
			if (MakesSun())
			{
				mLaunchCounter = TodCommon.RandRangeInt(300, mLaunchRate / 2);
			}
			else
			{
				mLaunchCounter = TodCommon.RandRangeInt(0, mLaunchRate);
			}
		}
		else
		{
			mLaunchCounter = 0;
		}
		if (theSeedType == SeedType.Blover)
		{
			mDoSpecialCountdown = 50;
			mDisappearCountdown = 410;
			if (IsInPlay())
			{
				theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_blow);
				theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
				theAttachReanim.mAnimRate = 30f;
				if (mBoard.mZombies.Exists((Zombie z) => z.mZombieType == ZombieType.Balloon))
				{
					theAttachReanim.SetFramesForLayer("anim_blow2");
					mDisappearCountdown = 473;
					PlantVoice.Play(SeedType.Blover, SeedType.None, PlantVoice.VoiceType.Plant2);
				}
				else
				{
					PlantVoice.Play(SeedType.Blover, SeedType.None, PlantVoice.VoiceType.Plant1);
				}
			}
			else
			{
				theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
				theAttachReanim.mAnimRate = 30f;
			}
		}
		else if (UseHeadReanim())
		{
			if (theAttachReanim != null)
			{
				theAttachReanim.mAnimRate = TodCommon.RandRangeFloat(15f, 20f);
				if (theAttachReanim.mIsSpine)
				{
					theAttachReanim.mAnimRate = 30f;
				}
				Reanimation reanimation = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, (mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.PeaWallnut) ? ReanimationType.Peashooter : reanimationType);
				reanimation.mLoopType = ReanimLoopType.Loop;
				reanimation.mAnimRate = theAttachReanim.mAnimRate;
				if (mApp.mGameMode == GameMode.ChallengeZenGarden && reanimation.TrackExists("anim_head_garden_idle"))
				{
					reanimation.SetFramesForLayer("anim_head_garden_idle");
				}
				else
				{
					reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
					if (IsInPlay() && reanimation.TrackExists("anim_head_enter") && !mApp.IsIZombieLevel())
					{
						reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
						reanimation.SetFramesForLayer("anim_head_enter");
						mState = PlantState.Entering;
					}
				}
				mHeadReanimID = mApp.ReanimationGetID(reanimation);
				if (theAttachReanim.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_stem))
				{
					reanimation.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_stem);
				}
				else if (theAttachReanim.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
				{
					AttachEffect attachEffect = reanimation.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_idle);
					if (mSeedType == SeedType.SunflowerPea)
					{
						attachEffect.mOffset.mMatrix.M41 += 36f;
					}
					else if (mSeedType == SeedType.PeaWallnut)
					{
						attachEffect.mOffset.mMatrix.M41 -= 36f;
					}
				}
			}
		}
		else
		{
			switch (theSeedType)
			{
			case SeedType.SunflowerWallnut:
			{
				if (theAttachReanim == null)
				{
					break;
				}
				Reanimation reanimation7 = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, ReanimationType.Wallnut);
				reanimation7.mLoopType = ReanimLoopType.Loop;
				reanimation7.mAnimRate = TodCommon.RandRangeFloat(10f, 15f);
				reanimation7.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
				mHeadReanimID = mApp.ReanimationGetID(reanimation7);
				if (theAttachReanim.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
				{
					AttachEffect attachEffect2 = reanimation7.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_idle);
					if (mSeedType == SeedType.SunflowerWallnut)
					{
						attachEffect2.mOffset.mMatrix.M41 += 36f;
					}
				}
				break;
			}
			case SeedType.Splitpea:
				if (mApp.mGameMode != GameMode.ChallengeZenGarden)
				{
					Debug.ASSERT(theAttachReanim != null);
					theAttachReanim.mAnimRate = 30f;
					Reanimation reanimation2 = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, reanimationType);
					reanimation2.mLoopType = ReanimLoopType.Loop;
					reanimation2.mAnimRate = theAttachReanim.mAnimRate;
					reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
					reanimation2.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_idle);
					mHeadReanimID = mApp.ReanimationGetID(reanimation2);
					Reanimation reanimation3 = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, reanimationType);
					reanimation3.mLoopType = ReanimLoopType.Loop;
					reanimation3.mAnimRate = theAttachReanim.mAnimRate;
					reanimation3.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_splitpea_idle);
					reanimation3.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_idle);
					mHeadReanimID2 = mApp.ReanimationGetID(reanimation3);
				}
				break;
			case SeedType.Threepeater:
			{
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.mAnimRate = TodCommon.RandRangeFloat(15f, 20f);
				Reanimation reanimation4 = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, reanimationType);
				reanimation4.mLoopType = ReanimLoopType.Loop;
				reanimation4.mAnimRate = theAttachReanim.mAnimRate;
				reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle1);
				reanimation4.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_head1);
				mHeadReanimID = mApp.ReanimationGetID(reanimation4);
				Reanimation reanimation5 = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, reanimationType);
				reanimation5.mLoopType = ReanimLoopType.Loop;
				reanimation5.mAnimRate = theAttachReanim.mAnimRate;
				reanimation5.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle2);
				reanimation5.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_head2);
				mHeadReanimID2 = mApp.ReanimationGetID(reanimation5);
				Reanimation reanimation6 = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, reanimationType);
				reanimation6.mLoopType = ReanimLoopType.Loop;
				reanimation6.mAnimRate = theAttachReanim.mAnimRate;
				reanimation6.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle3);
				reanimation6.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_head3);
				mHeadReanimID3 = mApp.ReanimationGetID(reanimation6);
				break;
			}
			case SeedType.Wallnut:
				mPlantHealth = GetBaseHealth(theSeedType);
				mBlinkCountdown = 1000 + RandomNumbers.NextNumber(1000);
				break;
			case SeedType.ExplodeONut:
			case SeedType.IceWallnut:
				mPlantHealth = GetBaseHealth(theSeedType);
				mBlinkCountdown = 1000 + RandomNumbers.NextNumber(1000);
				theAttachReanim.mColorOverride = new SexyColor(255, 64, 64);
				if (theSeedType == SeedType.IceWallnut)
				{
					theAttachReanim.mColorOverride = new SexyColor(64, 160, 255);
				}
				break;
			case SeedType.GiantWallnut:
				mPlantHealth = GetBaseHealth(theSeedType);
				mBlinkCountdown = 1000 + RandomNumbers.NextNumber(1000);
				break;
			case SeedType.Tallnut:
				if (!IsInPlay())
				{
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_zengarden);
				}
				mPlantHealth = GetBaseHealth(theSeedType);
				mHeight = 80;
				mBlinkCountdown = 1000 + RandomNumbers.NextNumber(1000);
				break;
			case SeedType.Garlic:
				Debug.ASSERT(theAttachReanim != null);
				mPlantHealth = GetBaseHealth(theSeedType);
				theAttachReanim.SetTruncateDisappearingFrames(empty, theTruncateDisappearingFrames: false);
				break;
			case SeedType.GoldMagnet:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.SetTruncateDisappearingFrames(empty, theTruncateDisappearingFrames: false);
				break;
			case SeedType.Cherrybomb:
				Debug.ASSERT(theAttachReanim != null);
				if (IsInPlay())
				{
					mDoSpecialCountdown = 100;
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_explode);
					theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
					mApp.PlayFoley(FoleyType.ReverseExplosion);
				}
				break;
			case SeedType.Imitater:
			case SeedType.ImitaterRandomPlant:
			case SeedType.ImitaterRandomZombie:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.mAnimRate = TodCommon.RandRangeFloat(25f, 30f);
				mStateCountdown = 200;
				break;
			case SeedType.Jalapeno:
				Debug.ASSERT(theAttachReanim != null);
				if (IsInPlay())
				{
					mDoSpecialCountdown = 100;
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_explode);
					theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
					mApp.PlayFoley(FoleyType.ReverseExplosion);
					mRenderOrder += 5000;
				}
				break;
			case SeedType.PickledPepper:
				Debug.ASSERT(theAttachReanim != null);
				if (IsInPlay())
				{
					mDoSpecialCountdown = 100;
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_explode);
					theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
					mApp.PlayFoley(FoleyType.ReverseExplosion);
				}
				break;
			case SeedType.Potatomine:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.mAnimRate = 12f;
				if (IsInPlay())
				{
					theAttachReanim.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_glow, -1);
					mStateCountdown = 1500;
				}
				else
				{
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_armed);
					mState = PlantState.PotatoArmed;
				}
				break;
			case SeedType.Gravebuster:
				Debug.ASSERT(theAttachReanim != null);
				if (IsInPlay())
				{
					mY += 8;
					mGravebusterDrop = mBoard.DropLootPieceWithResult(12);
					if (mGravebusterDrop == DropLootType.Diamond || mGravebusterDrop == DropLootType.PresentPlant || mGravebusterDrop == DropLootType.Chocolate)
					{
						theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_land_treasure);
						theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
						theAttachReanim.mAnimRate = 36f;
						PlantVoice.Play(SeedType.Gravebuster, SeedType.None, PlantVoice.VoiceType.Plant2);
					}
					else
					{
						theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_land);
						theAttachReanim.mLoopType = ReanimLoopType.PlayOnceAndHold;
						theAttachReanim.mAnimRate = 36f;
						PlantVoice.Play(SeedType.Gravebuster, SeedType.None, PlantVoice.VoiceType.Plant1);
					}
					mState = PlantState.GravebusterLanding;
				}
				break;
			case SeedType.Sunshroom:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.mFrameBasePose = 6;
				if (IsInPlay())
				{
					mX += RandomNumbers.NextNumber(10) - 5;
					mY += RandomNumbers.NextNumber(10) - 5;
				}
				else if (mIsAsleep)
				{
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_bigsleep);
				}
				else
				{
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_bigidle);
				}
				mState = PlantState.SunshroomSmall;
				mStateCountdown = 12000;
				break;
			case SeedType.Puffshroom:
			case SeedType.Seashroom:
				if (IsInPlay())
				{
					mX += RandomNumbers.NextNumber(10) - 5;
					mY += RandomNumbers.NextNumber(6) - 3;
				}
				break;
			case SeedType.Pumpkinshell:
				mPlantHealth = GetBaseHealth(theSeedType);
				mWidth = 120;
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_pumpkin_back, 1);
				break;
			case SeedType.Chomper:
			case SeedType.SuperChomper:
				mState = PlantState.Ready;
				break;
			case SeedType.Plantern:
			{
				mStateCountdown = 50;
				int num = RandomNumbers.NextNumber(100);
				if (num < 5)
				{
					if (!IsOnBoard() || mApp.mGameMode != GameMode.ChallengeZenGarden)
					{
						PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_henshin, ReanimLoopType.PlayOnceAndHold, 20, 30f);
						mState = PlantState.PlanternHenshinBegin;
						mStateCountdown = 457;
					}
					break;
				}
				if (!IsOnBoard() || mApp.mGameMode != GameMode.ChallengeZenGarden)
				{
					AddAttachedParticle(mX + 40, mY + 40, 500001, ParticleEffect.LanternShine);
				}
				if (IsInPlay())
				{
					mApp.PlaySample(Resources.SOUND_PLANTERN);
				}
				break;
			}
			case SeedType.Cattail:
			case SeedType.HypnoCattail:
				if (IsInPlay())
				{
					theAttachReanim.mAnimRate = 38f;
					if (theAttachReanim.mReanimationType == ReanimationType.HypnoCattail)
					{
						theAttachReanim.mAnimRate = 35f;
					}
					theAttachReanim.SetFramesForLayer("anim_henshin");
					mState = PlantState.LilypadInvulnerable;
					mStateCountdown = 392;
					mSubclass = 0;
				}
				else
				{
					PlayIdleAnim(theAttachReanim.mDefinition.mFPS);
				}
				break;
			case SeedType.Marigold:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.mAnimRate = 30f;
				break;
			case SeedType.Cactus:
				mState = PlantState.CactusLow;
				break;
			case SeedType.InstantCoffee:
				mDoSpecialCountdown = 100;
				break;
			case SeedType.Scaredyshroom:
				mState = PlantState.Ready;
				break;
			case SeedType.Cobcannon:
				if (IsInPlay())
				{
					mState = PlantState.CobcannonArming;
					mStateCountdown = 500;
					Debug.ASSERT(theAttachReanim != null);
					theAttachReanim.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_unarmed_idle);
				}
				break;
			case SeedType.Kernelpult:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.AssignRenderGroupToPrefix("Cornpult_butter", -1);
				break;
			case SeedType.Magnetshroom:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.SetTruncateDisappearingFrames(empty, theTruncateDisappearingFrames: false);
				break;
			case SeedType.Spikerock:
				mPlantHealth = GetBaseHealth(theSeedType);
				Debug.ASSERT(theAttachReanim != null);
				break;
			case SeedType.Flowerpot:
				if (IsInPlay())
				{
					mState = PlantState.FlowerpotInvulnerable;
					mStateCountdown = 100;
				}
				break;
			case SeedType.Lilypad:
				if (IsInPlay())
				{
					mState = PlantState.LilypadInvulnerable;
					mStateCountdown = 100;
				}
				if (IsInPlay() || mApp.mGameMode != GameMode.ChallengeZenGarden)
				{
					if (!IsInPlay())
					{
						theAttachReanim.PlayReanim("anim_idle", ReanimLoopType.Loop, 0, 0f);
					}
					else if (mBoard.PixelToGridX(mX, mY) == 0)
					{
						theAttachReanim.PlayReanim("anim_idle3", ReanimLoopType.Loop, 0, 0f);
					}
					else
					{
						theAttachReanim.PlayReanim("anim_idle2", ReanimLoopType.Loop, 0, 0f);
					}
				}
				break;
			case SeedType.Tanglekelp:
				Debug.ASSERT(theAttachReanim != null);
				theAttachReanim.SetTruncateDisappearingFrames(empty, theTruncateDisappearingFrames: false);
				break;
			case SeedType.FireShroom:
				mPlantHealth = GetBaseHealth(theSeedType);
				break;
			case SeedType.Agave:
				if (!mApp.IsIZombieLevel())
				{
					if (IsInPlay())
					{
						mState = PlantState.AgaveEntering;
						mStateCountdown = 350;
						PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_enter, ReanimLoopType.PlayOnceAndHold, 0, theAttachReanim.mDefinition.mFPS);
					}
					else
					{
						theAttachReanim.mAnimRate = theAttachReanim.mDefinition.mFPS;
					}
				}
				break;
			case SeedType.Endoflame:
				if (IsInPlay())
				{
					mState = PlantState.Entering;
					PlayBodyReanim("anim_debut", ReanimLoopType.PlayOnceAndHold, 20, theAttachReanim.mDefinition.mFPS);
					Plant flowerPotAt = mBoard.GetFlowerPotAt(mPlantCol, mRow);
					if (flowerPotAt != null)
					{
						flowerPotAt.mState = PlantState.FlowerpotEndoflameOverrideNoDraw;
					}
				}
				else
				{
					theAttachReanim.mAnimRate = theAttachReanim.mDefinition.mFPS;
				}
				break;
			}
		}
		if (theSeedType == SeedType.PeaWallnut || theSeedType == SeedType.SunflowerWallnut)
		{
			mPlantHealth = GetBaseHealth(theSeedType);
		}
		if (mApp.IsLevelUseJson())
		{
			CSOverridePlantProperty component2 = mApp.mCreativeLevel.GetComponent<CSOverridePlantProperty>();
			if (component2 != null && component2.mProperties.TryGetValue(mSeedType, out var value2) && value2.mPlantHealth.HasValue)
			{
				mPlantHealth = value2.mPlantHealth.GetValueOrDefault();
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeBigTime && (mSeedType == SeedType.Wallnut || mSeedType == SeedType.Sunflower || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.SunflowerWallnut || mSeedType == SeedType.PeaWallnut || mSeedType == SeedType.Marigold))
		{
			mPlantHealth *= 2;
		}
		mPlantMaxHealth = mPlantHealth;
		if (mSeedType != SeedType.Flowerpot && IsOnBoard())
		{
			Plant flowerPotAt2 = mBoard.GetFlowerPotAt(mPlantCol, mRow);
			if (flowerPotAt2 != null)
			{
				Reanimation reanimation8 = mApp.ReanimationGet(flowerPotAt2.mBodyReanimID);
				reanimation8.mAnimRate = 0f;
				mInFlowerPot = true;
			}
		}
		if (theImitaterType == SeedType.Imitater)
		{
			FilterEffectType mFilterEffect = FilterEffectType.WashedOut;
			if (mSeedType == SeedType.Hypnoshroom || mSeedType == SeedType.Squash || mSeedType == SeedType.Potatomine || mSeedType == SeedType.Garlic || mSeedType == SeedType.Lilypad)
			{
				mFilterEffect = FilterEffectType.LessWashedOut;
			}
			Reanimation reanimation9 = mApp.ReanimationTryToGet(mBodyReanimID);
			if (theAttachReanim != null)
			{
				theAttachReanim.mFilterEffect = mFilterEffect;
			}
			Reanimation reanimation10 = mApp.ReanimationTryToGet(mHeadReanimID);
			if (reanimation10 != null)
			{
				reanimation10.mFilterEffect = mFilterEffect;
			}
			Reanimation reanimation11 = mApp.ReanimationTryToGet(mHeadReanimID2);
			if (reanimation11 != null)
			{
				reanimation11.mFilterEffect = mFilterEffect;
			}
			Reanimation reanimation12 = mApp.ReanimationTryToGet(mHeadReanimID3);
			if (reanimation12 != null)
			{
				reanimation12.mFilterEffect = mFilterEffect;
			}
		}
		checkForPlantAchievements();
		UpdateReanim();
	}

	public void Update()
	{
		if ((!IsOnBoard() || mApp.mGameScene != GameScenes.LevelIntro || !mApp.IsWallnutBowlingLevel()) && (!IsOnBoard() || mApp.mGameMode != GameMode.ChallengeZenGarden) && (!IsOnBoard() || !mBoard.mCutScene.ShouldRunUpsellBoard()) && IsOnBoard() && mApp.mGameScene != GameScenes.Playing)
		{
			return;
		}
		if (IsInPlay() && mGloveGrabbed)
		{
			UpdateReanimColor();
			return;
		}
		UpdateAbilities();
		Animate();
		if (mPlantHealth < 0)
		{
			Die();
		}
		UpdateReanim();
	}

	public void Animate()
	{
		if ((mSeedType == SeedType.Cherrybomb || mSeedType == SeedType.PickledPepper) && mApp.mGameMode != GameMode.ChallengeZenGarden)
		{
			mShakeOffsetX = TodCommon.RandRangeFloat(0f, 2f) - 1f;
			mShakeOffsetY = TodCommon.RandRangeFloat(0f, 2f) - 1f;
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && mPottedPlantIndex != -1)
		{
			UpdateNeedsFood();
		}
		if (mRecentlyEatenCountdown > 0)
		{
			mRecentlyEatenCountdown--;
		}
		if (mEatenFlashCountdown > 0)
		{
			mEatenFlashCountdown--;
		}
		if (mBeghouledFlashCountdown > 0)
		{
			mBeghouledFlashCountdown--;
		}
		if (mSquished)
		{
			mFrame = 0;
			return;
		}
		if (mSeedType == SeedType.Wallnut || mSeedType == SeedType.Tallnut || mSeedType == SeedType.PeaWallnut || mSeedType == SeedType.SunflowerWallnut || mSeedType == SeedType.IceWallnut)
		{
			AnimateNuts();
		}
		else if (mSeedType == SeedType.Garlic)
		{
			AnimateGarlic();
		}
		else if (mSeedType == SeedType.Pumpkinshell)
		{
			AnimatePumpkin();
		}
		else if (mSeedType == SeedType.FireShroom)
		{
			AnimateFireShroom();
		}
		else if (mSeedType == SeedType.Hypnoshroom)
		{
			AnimateHypnoShroom();
		}
		if (DoAdvancedSleeping(mSeedType))
		{
			AnimateSleeping();
		}
		UpdateBlink();
		if (mAnimPing)
		{
			int num = mFrameLength * mNumFrames - 1;
			if (mAnimCounter < num)
			{
				mAnimCounter++;
			}
			else
			{
				mAnimPing = false;
				mAnimCounter -= mFrameLength;
			}
		}
		else if (mAnimCounter > 0)
		{
			mAnimCounter--;
		}
		else
		{
			mAnimPing = true;
			mAnimCounter += mFrameLength;
		}
		mFrame = mAnimCounter / mFrameLength;
	}

	public void Draw(Graphics g)
	{
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		if (mSeedType == SeedType.IrisShooter && !mSquished && !IrisArt.Ready)
		{
			DrawIrisShooterEffects(g);
		}
		int theCelRow = 0;
		float num = 0f;
		float num2 = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
		if (IsFlying(mSeedType) && mSquished)
		{
			num2 += 30f;
		}
		int theCelCol = mFrame;
		Image image = GetImage(mSeedType);
		if (mSquished)
		{
			if (mSeedType == SeedType.Flowerpot)
			{
				num2 -= 15f;
			}
			if (mSeedType == SeedType.InstantCoffee)
			{
				num2 -= 20f;
			}
			float num3 = 0.5f;
			g.SetScale(1f, num3, 0f, 0f);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new Color(255, 255, 255, (int)(255f * Math.Min(1f, (float)mDisappearCountdown / 100f))));
			if (mSeedType == SeedType.Puffshroom || mImitaterType == SeedType.Puffshroom)
			{
				g.SetScale(0.7f, 0.7f, 0f, 0f);
				g.DrawImage(AtlasResources.IMAGE_SQUISHED_PUFFSHROOM, -10, 30);
			}
			else
			{
				DrawSeedType(g, mSeedType, mImitaterType, DrawVariation.Normal, num * Constants.S + 40f + (float)Constants.Plant_Squished_Offset.X, num2 * Constants.S + (float)Constants.New.Board_GridCellSizeY_6Rows * (1f - num3) * Constants.S + (1f - num3) * 64f + (float)Constants.Plant_Squished_Offset.Y);
			}
			g.SetScale(1f, 1f, 0f, 0f);
			g.SetColorizeImages(colorizeImages: false);
		}
		else
		{
			if (mState == PlantState.FlowerpotEndoflameOverrideNoDraw)
			{
				return;
			}
			bool flag = false;
			Plant plant = null;
			if (IsOnBoard())
			{
				plant = mBoard.GetPumpkinAt(mPlantCol, mRow);
				if (plant != null)
				{
					Plant plant2 = mBoard.GetTopPlantAt(mPlantCol, mRow, TopPlant.OnlyNormalPosition);
					if (plant2 != null && plant2.mRenderOrder > plant.mRenderOrder)
					{
						plant2 = null;
					}
					if (plant2 != null && plant2.mOnBungeeState == PlantOnBungeeState.GettingGrabbedByBungee)
					{
						plant2 = null;
					}
					if (plant2 == this)
					{
						flag = true;
					}
					if (plant2 == null && mSeedType == SeedType.Pumpkinshell)
					{
						flag = true;
					}
				}
				else if (mSeedType == SeedType.Pumpkinshell)
				{
					flag = true;
					plant = this;
				}
			}
			else if (mSeedType == SeedType.Pumpkinshell)
			{
				flag = true;
				plant = this;
			}
			if (!GlobalStaticVars.gLowFramerate)
			{
				DrawShadow(g, num, num2);
			}
			if (IsFlying(mSeedType))
			{
				int num4 = ((!IsOnBoard()) ? mApp.mAppCounter : mBoard.mMainCounter);
				float num5 = (float)(num4 + mRow * 97 + mPlantCol * 61) * 0.03f;
				float num6 = (float)Math.Sin(num5) * 2f;
				num2 += num6;
			}
			if (flag)
			{
				Reanimation reanimation = plant.mLightReanimID;
				if (reanimation != null && reanimation.mActive)
				{
					Reanimation reanimation2 = mApp.ReanimationGet(plant.mLightReanimID);
					Graphics graphics = Graphics.GetNew(g);
					if (plant.mPosScaled)
					{
						graphics.mTransX += (int)((float)plant.mX * Constants.S);
						graphics.mTransY += (int)((float)plant.mY * Constants.S);
					}
					else
					{
						graphics.mTransX += plant.mX;
						graphics.mTransY += plant.mY;
					}
					if (mPosScaled)
					{
						graphics.mTransX -= (int)((float)mX * Constants.S);
						graphics.mTransY -= (int)((float)mY * Constants.S);
					}
					else
					{
						graphics.mTransX -= mX;
						graphics.mTransY -= mY;
					}
					reanimation2.Draw(graphics);
					graphics.PrepareForReuse();
				}
			}
			num += mShakeOffsetX;
			num2 += mShakeOffsetY;
			if (IsInPlay() && mApp.IsIZombieLevel())
			{
				mBoard.mChallenge.IZombieDrawPlant(g, this);
			}
			else if (mSeedType == SeedType.IrisShooter && IrisArt.Ready)
			{
				DrawIrisShooterBody(g, num, num2);
				DrawIrisShooterEffects(g);
			}
			else if (mBodyReanimID != null)
			{
				Reanimation reanimation3 = mApp.ReanimationTryToGet(mBodyReanimID);
				if (reanimation3 != null)
				{
					if (mGloveGrabbed)
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(new Color(255, 255, 255, 255));
					}
					if (mSeedType == SeedType.Spikeweed && (mState == PlantState.Doingspecial || mState == PlantState.SpikeweedAttacking))
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(new Color(255, 255, 255, (int)(255f * Math.Min(1f, (mState == PlantState.Doingspecial) ? ((float)mDisappearCountdown / 100f) : 0.7f))), premultiply: false);
					}
					float x = reanimation3.mOverlayMatrix.mMatrix.Translation.X;
					float y = reanimation3.mOverlayMatrix.mMatrix.Translation.Y;
					if (mSeedType == SeedType.Lilypad && IsInPlay() && mApp.mGameMode != GameMode.ChallengeZenGarden)
					{
						if (mPlantCol == 0)
						{
							if (!reanimation3.IsAnimPlaying("anim_idle3"))
							{
								reanimation3.PlayReanim("anim_idle3", ReanimLoopType.Loop, 0, 0f);
							}
							reanimation3.AssignRenderGroupToTrack("游泳圈", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline1", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline2", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline3", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline4", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline5", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline6", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline7", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline8", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline9", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline10", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline11", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline12", 114514);
							reanimation3.AssignRenderGroupToTrack("anim_waterline13", 114514);
							int num7 = ((mBoard == null) ? GlobalStaticVars.gLawnApp.mAppCounter : mBoard.mMainCounter);
							float num8 = (float)mRow * (float)Math.PI + (float)mPlantCol * (float)Math.PI * 0.25f;
							float num9 = (float)num7 * (float)Math.PI * 2f / 200f;
							float num10 = (float)Math.Sin(num8 + num9) * 2f;
							float num11 = num10;
							reanimation3.SetPosition(x, y - num11 - 5f);
							reanimation3.AssignRenderGroupToTrack("辫子2", 114513);
							reanimation3.DrawRenderGroup(g, 114513);
							reanimation3.SetPosition(x, y);
							reanimation3.DrawRenderGroup(g, 114514);
							reanimation3.SetPosition(x, y - num11 - 5f);
						}
						else if (!reanimation3.IsAnimPlaying("anim_idle2"))
						{
							reanimation3.PlayReanim("anim_idle2", ReanimLoopType.Loop, 0, 0f);
							reanimation3.AssignRenderGroupToTrack("游泳圈", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline1", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline2", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline3", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline4", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline5", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline6", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline7", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline8", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline9", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline10", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline11", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline12", 0);
							reanimation3.AssignRenderGroupToTrack("anim_waterline13", 0);
							reanimation3.AssignRenderGroupToTrack("辫子2", 0);
						}
					}
					reanimation3.DrawRenderGroup(g, 0);
					if (mSeedType == SeedType.Lilypad && mPlantCol == 0)
					{
						reanimation3.SetPosition(x, y);
					}
					if (mSeedType == SeedType.Pumpkinshell)
					{
						mApp.ReanimationTryToGet(mAttackingReanimID)?.Draw(g);
					}
					if (mGloveGrabbed || (mSeedType == SeedType.Spikeweed && (mState == PlantState.Doingspecial || mState == PlantState.SpikeweedAttacking)))
					{
						g.SetColorizeImages(colorizeImages: false);
					}
					if (mSeedType == SeedType.Kernelpult)
					{
						g.SetScaleX(1f);
						g.SetScaleY(1f);
					}
				}
			}
			else
			{
				SeedType seedType = SeedType.None;
				if (mBoard != null)
				{
					seedType = mBoard.GetSeedTypeInCursor();
				}
				if (IsPartOfUpgradableTo(seedType) && mBoard.CanPlantAt(mPlantCol, mRow, seedType) == PlantingReason.Ok && !mGloveGrabbed)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(TodCommon.GetFlashingColor(mBoard.mMainCounter, 90));
				}
				else if (seedType == SeedType.Cobcannon && mBoard.CanPlantAt(mPlantCol - 1, mRow, seedType) == PlantingReason.Ok)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(TodCommon.GetFlashingColor(mBoard.mMainCounter, 90));
				}
				else if (mBoard != null && mBoard.mTutorialState == TutorialState.ShovelDig)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(TodCommon.GetFlashingColor(mBoard.mMainCounter, 90));
				}
				if (image != null)
				{
					TodCommon.TodDrawImageCelF(g, image, num, num2, theCelCol, theCelRow);
				}
				if (mSeedType == SeedType.Sprout)
				{
					if (mGloveGrabbed)
					{
						g.SetColorizeImages(colorizeImages: true);
						g.SetColor(new Color(150, 255, 150, 255));
					}
					TodCommon.TodDrawImageCelF(g, AtlasResources.IMAGE_CACHED_MARIGOLD, Constants.ZenGarden_Marigold_Sprout_Offset.X, Constants.ZenGarden_Marigold_Sprout_Offset.Y, 0, 0);
					if (mGloveGrabbed)
					{
						g.SetColorizeImages(colorizeImages: false);
					}
				}
				g.SetColorizeImages(colorizeImages: false);
				if (mHighlighted)
				{
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(new SexyColor(255, 255, 255, 196));
					if (image != null)
					{
						TodCommon.TodDrawImageCelF(g, image, num, num2, theCelCol, theCelRow);
					}
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
					g.SetColorizeImages(colorizeImages: false);
				}
				else if (mEatenFlashCountdown > 0)
				{
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
					g.SetColorizeImages(colorizeImages: true);
					int theAlpha = TodCommon.ClampInt(mEatenFlashCountdown * 3, 0, 255);
					g.SetColor(new SexyColor(255, 255, 255, theAlpha));
					if (image != null)
					{
						TodCommon.TodDrawImageCelF(g, image, num, num2, theCelCol, theCelRow);
					}
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
					g.SetColorizeImages(colorizeImages: false);
				}
			}
			if (mSeedType == SeedType.Magnetshroom && !DrawMagnetItemsOnTop())
			{
				DrawMagnetItems(g);
			}
		}
	}

	public void MouseDown(int x, int y, int theClickCount)
	{
		if (theClickCount < 0)
		{
			return;
		}
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			if (mApp.mZenGarden != null && mPottedPlantIndex != -1)
			{
				if (this == mApp.mZenGarden.mInteracting)
				{
					mApp.mZenGarden.LeavePlantInteraction(this);
				}
				else
				{
					mApp.mZenGarden.EnterPlantInteraction(this);
				}
			}
		}
		else
		{
			if (mBoard.mCursorObject.mCursorType != CursorType.Normal)
			{
				return;
			}
			if (mState == PlantState.CobcannonReady)
			{
				mBoard.RefreshSeedPacketFromCursor();
				mBoard.mCursorObject.mType = SeedType.None;
				mBoard.mCursorObject.mCursorType = CursorType.CobcannonTarget;
				mBoard.mCursorObject.mSeedBankIndex = -1;
				mBoard.mCursorObject.mCoinID = null;
				mBoard.mCursorObject.mCobCannonPlantID = mBoard.mPlants[mBoard.mPlants.IndexOf(this)];
				mBoard.mCobCannonCursorDelayCounter = 30;
				mBoard.mCobCannonMouseX = x;
				mBoard.mCobCannonMouseY = y;
			}
			else if (mSeedType == SeedType.Agave && (mState == PlantState.AgaveAttacking || mState == PlantState.Ready) && mBoard.mAgavePowerfulCountdown <= 0 && mBoard.IsPlantSkillEnabled(SeedType.Agave))
			{
				if (mBoard.HasConveyorBeltSeedBank() || mBoard.CanTakeSunMoney(175))
				{
					mState = PlantState.AgavePowerfulTendToLaunching;
					mBoard.RefreshSeedPacketFromCursor();
					mBoard.mCursorObject.mType = SeedType.None;
					mBoard.mCursorObject.mCursorType = CursorType.CobcannonTarget;
					mBoard.mCursorObject.mSeedBankIndex = -1;
					mBoard.mCursorObject.mCoinID = null;
					mBoard.mCursorObject.mCobCannonPlantID = mBoard.mPlants[mBoard.mPlants.IndexOf(this)];
					mBoard.mCobCannonCursorDelayCounter = 30;
					mBoard.mCobCannonMouseX = x;
					mBoard.mCobCannonMouseY = y;
				}
				else
				{
					mApp.PlaySample(Resources.SOUND_BUZZER);
					mBoard.mOutOfMoneyCounter = 70;
				}
			}
			else if (mSeedType == SeedType.Endoflame && (mState == PlantState.Notready || mState == PlantState.Ready) && mBoard.mEndoflamePowerfulCountdown <= 0 && mBoard.IsPlantSkillEnabled(SeedType.Endoflame))
			{
				if (mBoard.HasConveyorBeltSeedBank() || mBoard.CanTakeSunMoney(225))
				{
					mSubclass = 0;
					mState = PlantState.AgavePowerfulTendToLaunching;
					mBoard.RefreshSeedPacketFromCursor();
					mBoard.mCursorObject.mType = SeedType.None;
					mBoard.mCursorObject.mCursorType = CursorType.CobcannonTarget;
					mBoard.mCursorObject.mSeedBankIndex = -1;
					mBoard.mCursorObject.mCoinID = null;
					mBoard.mCursorObject.mCobCannonPlantID = mBoard.mPlants[mBoard.mPlants.IndexOf(this)];
					mBoard.mCobCannonCursorDelayCounter = 30;
					mBoard.mCobCannonMouseX = x;
					mBoard.mCobCannonMouseY = y;
				}
				else
				{
					mApp.PlaySample(Resources.SOUND_BUZZER);
					mBoard.mOutOfMoneyCounter = 70;
				}
			}
		}
	}

	public bool CanDoTouch(int touchType)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return false;
		}
		switch (touchType)
		{
		case 1:
			if (mSeedType == SeedType.Repeater || mSeedType == SeedType.Peashooter)
			{
				return mApp.ReanimationTryToGet(mHeadReanimID)?.TrackExists("anim_garden1") ?? false;
			}
			return reanimation.TrackExists("anim_garden1");
		case 2:
			if (mSeedType == SeedType.Imitater)
			{
				return true;
			}
			if (mSeedType == SeedType.Repeater || mSeedType == SeedType.Peashooter)
			{
				return mApp.ReanimationTryToGet(mHeadReanimID)?.TrackExists("anim_garden2") ?? false;
			}
			return reanimation.TrackExists("anim_garden2");
		default:
			return false;
		}
	}

	public bool UseHeadReanim()
	{
		SeedType seedType = mSeedType;
		if (seedType != SeedType.Peashooter && seedType != SeedType.Snowpea && seedType != SeedType.Repeater && seedType != SeedType.Leftpeater && seedType != SeedType.Gatlingpea && seedType != SeedType.SunflowerPea)
		{
			return seedType == SeedType.PeaWallnut;
		}
		return true;
	}

	public bool DoTouch(int touchType)
	{
		SetSleeping(theIsAsleep: false);
		if (!CanDoTouch(touchType))
		{
			return false;
		}
		if (touchType == 2 && mSeedType == SeedType.Umbrella)
		{
			mApp.mZenGarden?.InteractionFocusOnPlant(this, 2f, 50);
		}
		switch (touchType)
		{
		case 1:
			if (UseHeadReanim())
			{
				Reanimation reanimation3 = mApp.ReanimationTryToGet(mHeadReanimID);
				if (reanimation3 == null)
				{
					return false;
				}
				reanimation3.PlayReanim("anim_garden1", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			else if (mSeedType == SeedType.Scaredyshroom || mSeedType == SeedType.Chomper || mSeedType == SeedType.Spikeweed)
			{
				mApp.ReanimationGet(mBodyReanimID).PlayReanim("anim_garden1", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			else
			{
				Reanimation reanimation4 = mApp.ReanimationTryToGet(mBodyReanimID);
				if (reanimation4 == null)
				{
					return false;
				}
				reanimation4.PlayReanim("anim_garden1", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			PlantVoice.Play(mSeedType, mImitaterType, PlantVoice.VoiceType.Garden1);
			return true;
		case 2:
			if (UseHeadReanim())
			{
				Reanimation reanimation = mApp.ReanimationTryToGet(mHeadReanimID);
				if (reanimation == null)
				{
					return false;
				}
				reanimation.PlayReanim("anim_garden2", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			else if (mSeedType == SeedType.Scaredyshroom || mSeedType == SeedType.Chomper || mSeedType == SeedType.Spikeweed)
			{
				mApp.ReanimationGet(mBodyReanimID).PlayReanim("anim_garden2", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			else
			{
				Reanimation reanimation2 = mApp.ReanimationTryToGet(mBodyReanimID);
				if (reanimation2 == null)
				{
					return false;
				}
				if (mSeedType == SeedType.Imitater)
				{
					int num = RandomNumbers.NextNumber(2) + 1;
					reanimation2.PlayReanim($"anim_garden2_{num}", ReanimLoopType.PlayOnceAndHold, 20, (reanimation2.mAnimRate < 20f) ? 30f : reanimation2.mAnimRate);
					PlantVoice.Play(mSeedType, mImitaterType, (num == 1) ? PlantVoice.VoiceType.Garden2 : PlantVoice.VoiceType.Garden2_2);
					return true;
				}
				reanimation2.PlayReanim("anim_garden2", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			}
			PlantVoice.Play(mSeedType, mImitaterType, PlantVoice.VoiceType.Garden2);
			return true;
		default:
			return false;
		}
	}

	public void DoSpecial()
	{
		int num = mX + mWidth / 2;
		int num2 = mY + mHeight / 2;
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		if (mSeedType == SeedType.Blover)
		{
			if (mState != PlantState.Doingspecial)
			{
				mState = PlantState.Doingspecial;
				mBoard.AddWind(WindType.Blover);
				BlowAwayFliers(mX, mRow);
			}
		}
		else if (mSeedType == SeedType.Cherrybomb)
		{
			mApp.PlayFoley(FoleyType.Cherrybomb);
			mApp.PlayFoley(FoleyType.Juicy);
			int num3 = mBoard.KillAllZombiesInRadius(mRow, num, num2, 115, 1, theBurn: true, damageRangeFlags);
			if (num3 >= 10 && !mApp.IsLittleTroubleLevel())
			{
				mBoard.GrantAchievement(AchievementId.ACHIEVEMENT_EXPLODONATOR, show: true);
			}
			mApp.AddTodParticle(num, num2, 400000, ParticleEffect.Powie);
			mApp.Vibrate();
			mBoard.ShakeBoard(3, -4);
			Die();
		}
		else if (mSeedType == SeedType.Doomshroom)
		{
			mApp.PlaySample(Resources.SOUND_DOOMSHROOM);
			mBoard.KillAllZombiesInRadius(mRow, num, num2, 250, 3, theBurn: true, damageRangeFlags);
			KillAllPlantsNearDoom();
			mApp.AddTodParticle(num, num2, 400000, ParticleEffect.Doom);
			GridItem gridItem = mBoard.AddACrater(mPlantCol, mRow);
			gridItem.mGridItemCounter = 18000;
			mBoard.ShakeBoard(3, -4);
			mApp.Vibrate();
			Die();
			mBoard.mDoomsUsed++;
		}
		else if (mSeedType == SeedType.Jalapeno)
		{
			if (mState != PlantState.Ready && mState != PlantState.JalapenoFwoosh)
			{
				mState = PlantState.JalapenoFwoosh;
				mDoSpecialCountdown = 500;
				mApp.PlayFoley(FoleyType.JalapenoIgnite);
				mApp.PlayFoley(FoleyType.Juicy);
				mBoard.DoFwoosh(mRow);
				mBoard.ShakeBoard(3, -4);
				mApp.Vibrate();
				BurnRow(mRow);
				if (mBoard.mIceTimer[mRow] > 0)
				{
					mBoard.mIceTimer[mRow] = 20;
				}
			}
			else
			{
				Die();
			}
		}
		else if (mSeedType == SeedType.PickledPepper)
		{
			mApp.PlayFoley(FoleyType.JalapenoIgnite);
			mApp.PlayFoley(FoleyType.Juicy);
			mBoard.DoFwooshLine(mX);
			mBoard.ShakeBoard(3, -4);
			mApp.Vibrate();
			BurnLine();
			Die();
		}
		else if (mSeedType == SeedType.Umbrella)
		{
			if (mState != PlantState.UmbrellaTriggered && mState != PlantState.UmbrellaReflecting && mState != PlantState.UmbrellaDeathTriggered && mState != PlantState.UmbrellaDeathReflecting)
			{
				mState = PlantState.UmbrellaTriggered;
				mStateCountdown = 5;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_block, ReanimLoopType.PlayOnceAndHold, 0, 30f);
			}
		}
		else if (mSeedType == SeedType.Hypnoshroom)
		{
			mState = PlantState.Doingspecial;
			mDisappearCountdown = 400;
			PlayBodyReanim("anim_explode", ReanimLoopType.PlayOnce, 0, 0f);
		}
		else if (mSeedType == SeedType.Iceshroom)
		{
			mApp.PlayFoley(FoleyType.Frozen);
			IceZombies();
			mApp.AddTodParticle(num, num2, 400000, ParticleEffect.IceTrap);
			mDisappearCountdown = 313;
			mState = PlantState.Dying;
		}
		else if (mSeedType == SeedType.Potatomine)
		{
			num = mX + mWidth / 2 - 20;
			num2 = mY + mHeight / 2;
			mApp.PlaySample(Resources.SOUND_POTATO_MINE);
			mBoard.KillAllZombiesInRadius(mRow, num, num2, 60, 0, theBurn: false, damageRangeFlags);
			int aRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, mRow, 0);
			mApp.AddTodParticle((float)num + 20f, num2, aRenderOrder, ParticleEffect.PotatoMine);
			mBoard.ShakeBoard(3, -4);
			mApp.Vibrate();
			Die();
		}
		else if (mSeedType == SeedType.InstantCoffee)
		{
			Plant topPlantAt = mBoard.GetTopPlantAt(mPlantCol, mRow, TopPlant.OnlyNormalPosition);
			if (topPlantAt != null && topPlantAt.mIsAsleep)
			{
				topPlantAt.mWakeUpCounter = 100;
			}
			mState = PlantState.Doingspecial;
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_crumble, ReanimLoopType.PlayOnceAndHold, 20, 22f);
			mApp.PlayFoley(FoleyType.Coffee);
		}
	}

	public void DoDyingSpecial()
	{
		if (mSeedType == SeedType.Tallnut)
		{
			mState = PlantState.Dying;
			mDisappearCountdown = 400;
			PlayBodyReanim("anim_explode", ReanimLoopType.PlayOnce, 20, 30f);
			PlantVoice.Play(SeedType.Tallnut, SeedType.None, PlantVoice.VoiceType.Die);
		}
		else if (mSeedType == SeedType.ExplodeONut)
		{
			int num = mX + mWidth / 2;
			int num2 = mY + mHeight / 2;
			mApp.PlayFoley(FoleyType.Cherrybomb);
			mApp.PlaySample(Resources.SOUND_BOWLINGIMPACT2);
			int theDamageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary) | 0x20;
			mBoard.KillAllZombiesInRadius(mRow, num, num2, 90, 1, theBurn: true, theDamageRangeFlags);
			mApp.AddTodParticle(num, num2, 400000, ParticleEffect.Powie);
			mBoard.ShakeBoard(3, -4);
			Die();
		}
		else if (mSeedType == SeedType.IceWallnut)
		{
			int num3 = mX + mWidth / 2;
			int num4 = mY + mHeight / 2;
			mApp.PlayFoley(FoleyType.Frozen);
			IceZombies();
			mApp.AddTodParticle(num3, num4, 400000, ParticleEffect.IceTrap);
			Die();
		}
	}

	public void Fire(Zombie theTargetZombie, int theRow, PlantWeapon thePlantWeapon)
	{
		if (mSeedType == SeedType.Fumeshroom)
		{
			DoRowAreaDamage(20, 2u);
			mApp.PlayFoley(FoleyType.Fume);
			return;
		}
		if (mSeedType == SeedType.Gloomshroom)
		{
			DoRowAreaDamage(20, 2u);
			return;
		}
		if (mSeedType == SeedType.Starfruit)
		{
			StarFruitFire();
			return;
		}
		ProjectileType projectileType = ProjectileType.ProjectilesCount;
		switch (mSeedType)
		{
		case SeedType.Peashooter:
			projectileType = ProjectileType.Pea;
			break;
		case SeedType.Snowpea:
			projectileType = ProjectileType.Snowpea;
			break;
		case SeedType.Repeater:
			projectileType = ProjectileType.Pea;
			break;
		case SeedType.Puffshroom:
			projectileType = ProjectileType.Puff;
			break;
		case SeedType.Scaredyshroom:
			projectileType = ProjectileType.Puff;
			break;
		case SeedType.Threepeater:
			projectileType = ProjectileType.Pea;
			break;
		case SeedType.Seashroom:
			projectileType = ProjectileType.PuffGreen;
			break;
		case SeedType.Cactus:
			projectileType = ProjectileType.Spike;
			break;
		case SeedType.Splitpea:
			projectileType = ProjectileType.Pea;
			break;
		case SeedType.Cabbagepult:
			projectileType = ProjectileType.Cabbage;
			break;
		case SeedType.Kernelpult:
			projectileType = ProjectileType.Kernel;
			break;
		case SeedType.Melonpult:
			projectileType = ProjectileType.Melon;
			break;
		case SeedType.Gatlingpea:
			projectileType = ProjectileType.Pea;
			break;
		case SeedType.Cattail:
			projectileType = ProjectileType.CattailSpike;
			break;
		case SeedType.HypnoCattail:
			projectileType = ProjectileType.HypnoCattailSpike;
			break;
		case SeedType.Wintermelon:
			projectileType = ProjectileType.Wintermelon;
			break;
		case SeedType.Cobcannon:
			projectileType = ProjectileType.Cobbig;
			break;
		case SeedType.Endoflame:
			projectileType = ProjectileType.EndoflameSpike;
			break;
		case SeedType.SunflowerPea:
		case SeedType.PeaWallnut:
			projectileType = ProjectileType.Pea;
			break;
		case SeedType.Leftpeater:
			projectileType = ProjectileType.Pea;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		if (mSeedType == SeedType.Kernelpult && thePlantWeapon == PlantWeapon.Secondary)
		{
			projectileType = ProjectileType.Butter;
		}
		mApp.PlayFoley(FoleyType.Throw);
		if (mSeedType == SeedType.Snowpea || mSeedType == SeedType.Wintermelon)
		{
			mApp.PlayFoley(FoleyType.SnowPeaSparkles);
		}
		else if (mSeedType == SeedType.Puffshroom || mSeedType == SeedType.Scaredyshroom || mSeedType == SeedType.Seashroom)
		{
			mApp.PlayFoley(FoleyType.Puff);
		}
		int num2;
		int num;
		if (mSeedType == SeedType.Puffshroom)
		{
			num = mX + 40;
			num2 = mY + 40;
			num += 15;
			num2 -= 15;
		}
		else if (mSeedType == SeedType.Seashroom)
		{
			num = mX + 45;
			num2 = mY + 63;
			num2 -= 15;
		}
		else if (mSeedType == SeedType.Cabbagepult)
		{
			num = mX + 5;
			num2 = mY - 12;
		}
		else if (mSeedType == SeedType.Melonpult || mSeedType == SeedType.Wintermelon)
		{
			num = mX + 25;
			num2 = mY - 46;
		}
		else if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
		{
			if (thePlantWeapon == PlantWeapon.Secondary)
			{
				num = mX + 80;
				num2 = mY - 45;
			}
			else
			{
				num = mX + 100;
				num2 = mY - 10;
			}
		}
		else if (mSeedType == SeedType.Kernelpult && thePlantWeapon == PlantWeapon.Primary)
		{
			num = mX + 19;
			num2 = mY - 37;
		}
		else if (mSeedType == SeedType.Kernelpult && thePlantWeapon == PlantWeapon.Secondary)
		{
			num = mX + 12;
			num2 = mY - 56;
		}
		else if (mSeedType == SeedType.Peashooter)
		{
			int theOffsetX = 0;
			int theOffsetY = 0;
			GetPeaHeadOffset(ref theOffsetX, ref theOffsetY);
			num = mX + theOffsetX + 24 + 15;
			num2 = mY + theOffsetY + -33 + 20;
		}
		else if (mSeedType == SeedType.SunflowerPea)
		{
			int theOffsetX2 = 0;
			int theOffsetY2 = 0;
			GetPeaHeadOffset(ref theOffsetX2, ref theOffsetY2);
			num = mX + theOffsetX2 + 24 + 15 + 16;
			num2 = mY + theOffsetY2 + -33 + 20;
		}
		else if (mSeedType == SeedType.PeaWallnut)
		{
			int num3 = 24;
			int num4 = 24;
			num = mX + num3 + 24 + 15 - 16;
			num2 = mY + num4 + -33 + 20;
		}
		else if (mSeedType == SeedType.Snowpea)
		{
			int theOffsetX3 = 0;
			int theOffsetY3 = 0;
			GetPeaHeadOffset(ref theOffsetX3, ref theOffsetY3);
			num = mX + theOffsetX3 + 24;
			num2 = mY + theOffsetY3 + -33;
			num += 42;
			num2 += 21;
		}
		else if (mSeedType == SeedType.Repeater)
		{
			int theOffsetX4 = 0;
			int theOffsetY4 = 0;
			GetPeaHeadOffset(ref theOffsetX4, ref theOffsetY4);
			num = mX + theOffsetX4 + 24 + 24 + 18;
			num2 = mY + theOffsetY4 + -33 + 10;
		}
		else if (mSeedType == SeedType.Leftpeater)
		{
			int theOffsetX5 = 0;
			int theOffsetY5 = 0;
			GetPeaHeadOffset(ref theOffsetX5, ref theOffsetY5);
			num = mX - theOffsetX5 + 27 - 24;
			num2 = mY + theOffsetY5 - 33 + 10;
		}
		else if (mSeedType == SeedType.Gatlingpea)
		{
			int theOffsetX6 = 0;
			int theOffsetY6 = 0;
			GetPeaHeadOffset(ref theOffsetX6, ref theOffsetY6);
			num = mX + theOffsetX6 + 34;
			num2 = mY + theOffsetY6 + -33;
			num += 32;
			num2 += 45;
		}
		else if (mSeedType == SeedType.Splitpea)
		{
			int theOffsetX7 = 0;
			int theOffsetY7 = 0;
			GetPeaHeadOffset(ref theOffsetX7, ref theOffsetY7);
			num2 = mY + theOffsetY7 + -33;
			switch (thePlantWeapon)
			{
			case PlantWeapon.Secondary:
				num = mX + theOffsetX7 - 64;
				num += 30;
				num2 -= 5;
				break;
			case PlantWeapon.Tertiary:
				num = mX + theOffsetX7 - 64;
				num += 30;
				num2 += 15;
				break;
			default:
				num = mX + theOffsetX7 + 24;
				num += 15;
				num2 += 15;
				break;
			}
		}
		else if (mSeedType == SeedType.Threepeater)
		{
			num2 = mY + 10;
			num = mX + 45;
		}
		else if (mSeedType == SeedType.Scaredyshroom)
		{
			num = mX + 29;
			num2 = mY + 21 - 20;
		}
		else if (mSeedType == SeedType.Cactus)
		{
			if (thePlantWeapon == PlantWeapon.Primary)
			{
				num = mX + 93;
				num2 = mY - 50;
			}
			else
			{
				num = mX + 70;
				num2 = mY + 23;
			}
		}
		else if (mSeedType == SeedType.Cobcannon)
		{
			num = mX + Constants.Plant_CobCannon_Projectile_Offset.X;
			num2 = mY + Constants.Plant_CobCannon_Projectile_Offset.Y;
		}
		else if (mSeedType == SeedType.Endoflame)
		{
			num = mX + 50 - 6;
			num2 = mY;
		}
		else
		{
			num = mX + 10;
			num2 = mY + 5;
		}
		Plant flowerPotAt = mBoard.GetFlowerPotAt(mPlantCol, mRow);
		if (flowerPotAt != null)
		{
			num2 -= 5;
		}
		if (mSeedType == SeedType.Snowpea)
		{
			int aRenderOrder = Board.MakeRenderOrder(RenderLayer.LawnMower, mRow, 1);
			mApp.AddTodParticle(num + 8, num2 + 13, aRenderOrder, ParticleEffect.SnowpeaPuff);
		}
		Projectile projectile = mBoard.AddProjectile(num, num2, mRenderOrder + -1, theRow, projectileType);
		projectile.mDamageRangeFlags = GetDamageRangeFlags(thePlantWeapon);
		projectile.mFromPlant = mSeedType;
		if (mSeedType == SeedType.Cabbagepult || mSeedType == SeedType.Kernelpult || mSeedType == SeedType.Melonpult || mSeedType == SeedType.Wintermelon)
		{
			float num6;
			float num7;
			if (theTargetZombie != null)
			{
				TRect zombieRect = theTargetZombie.GetZombieRect();
				float num5 = theTargetZombie.ZombieTargetLeadX(50f);
				num6 = num5 - (float)num - 30f;
				num7 = zombieRect.mY - num2;
				if (theTargetZombie.mZombiePhase == ZombiePhase.DolphinRiding)
				{
					num6 -= 60f;
				}
				if (theTargetZombie.mZombieType == ZombieType.Pogo && theTargetZombie.mHasObject)
				{
					num6 -= 60f;
				}
				if (theTargetZombie.mZombiePhase == ZombiePhase.SnorkelWalkingInPool)
				{
					num6 -= 40f;
				}
				if (theTargetZombie.mZombieType == ZombieType.Boss)
				{
					int num8 = mBoard.GridToPixelY(8, mRow);
					num7 = num8 - num2;
				}
			}
			else
			{
				num6 = 700f - (float)num;
				num7 = 0f;
			}
			if (num6 < 40f)
			{
				num6 = 40f;
			}
			projectile.mMotionType = ProjectileMotion.Lobbed;
			float num9 = 120f;
			projectile.mVelX = num6 / num9;
			projectile.mVelY = 0f;
			projectile.mVelZ = -7f + num7 / num9;
			projectile.mAccZ = 0.115f;
		}
		else if (mSeedType == SeedType.Threepeater)
		{
			if (theRow < mRow)
			{
				projectile.mMotionType = ProjectileMotion.Threepeater;
				projectile.mVelY = -3f;
				projectile.mShadowY += 80f;
			}
			else if (theRow > mRow)
			{
				projectile.mMotionType = ProjectileMotion.Threepeater;
				projectile.mVelY = 3f;
				projectile.mShadowY += -80f;
			}
		}
		else if (mSeedType == SeedType.Puffshroom || mSeedType == SeedType.Seashroom)
		{
			projectile.mMotionType = ProjectileMotion.Puff;
		}
		else if (mSeedType == SeedType.Splitpea && (thePlantWeapon == PlantWeapon.Secondary || thePlantWeapon == PlantWeapon.Tertiary))
		{
			projectile.mMotionType = ProjectileMotion.Backwards;
		}
		else if (mSeedType == SeedType.Leftpeater)
		{
			projectile.mMotionType = ProjectileMotion.Backwards;
		}
		else if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
		{
			projectile.mMotionType = ProjectileMotion.Homing;
			projectile.mVelX = 2f;
			projectile.mTargetZombieID = mBoard.ZombieGetID(theTargetZombie);
		}
		else if (mSeedType == SeedType.Endoflame)
		{
			projectile.mMotionType = ProjectileMotion.EndoflameHoming;
			projectile.mVelX = 0f;
			projectile.mTargetZombieID = mBoard.ZombieGetID(theTargetZombie);
		}
		else
		{
			if (mSeedType == SeedType.Cobcannon)
			{
				projectile.mDamageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
				projectile.mMotionType = ProjectileMotion.Lobbed;
				projectile.mVelX = 0.001f;
				projectile.mVelY = 0f;
				projectile.mAccZ = 0f;
				projectile.mVelZ = -8f;
				projectile.mCobTargetX = mTargetX - 40;
				projectile.mCobTargetRow = mBoard.PixelToGridYKeepOnBoard(mTargetX, mTargetY);
			}
			if (mSeedType == SeedType.Cactus)
			{
				projectile.mMotionType = ProjectileMotion.YAwareStraight;
			}
		}
	}

	public Zombie FindTargetZombie(int theRow, PlantWeapon thePlantWeapon)
	{
		int damageRangeFlags = GetDamageRangeFlags(thePlantWeapon);
		TRect plantAttackRect = GetPlantAttackRect(thePlantWeapon);
		int num = 0;
		Zombie zombie = null;
		for (int i = 0; i < mBoard.mZombies.Count; i++)
		{
			Zombie zombie2 = mBoard.mZombies[i];
			if (zombie2.mDead)
			{
				continue;
			}
			int num2 = zombie2.mRow - theRow;
			if (zombie2.mZombieType == ZombieType.Boss)
			{
				num2 = 0;
			}
			if ((!zombie2.mHasHead || zombie2.IsTangleKelpTarget()) && (mSeedType == SeedType.Potatomine || mSeedType == SeedType.Chomper || mSeedType == SeedType.SuperChomper || mSeedType == SeedType.Tanglekelp || (mSeedType == SeedType.Gatlingpea && thePlantWeapon == PlantWeapon.Secondary)))
			{
				continue;
			}
			bool flag = false;
			CSSpawnPortal cSSpawnPortal = mApp.mCreativeLevel?.GetComponent<CSSpawnPortal>();
			if (cSSpawnPortal != null && cSSpawnPortal.mFixPlantAttackView)
			{
				bool flag2;
				switch (mSeedType)
				{
				case SeedType.Peashooter:
				case SeedType.Snowpea:
				case SeedType.Repeater:
				case SeedType.Scaredyshroom:
				case SeedType.Threepeater:
				case SeedType.Cactus:
				case SeedType.Gatlingpea:
				case SeedType.SunflowerPea:
				case SeedType.PeaWallnut:
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
				if (flag2)
				{
					flag = true;
				}
				if (mSeedType == SeedType.Splitpea && thePlantWeapon == PlantWeapon.Primary)
				{
					flag = true;
				}
			}
			else if (mApp.mGameMode == GameMode.ChallengePortalCombat && (mSeedType == SeedType.Peashooter || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.PeaWallnut || mSeedType == SeedType.Cactus || mSeedType == SeedType.Repeater))
			{
				flag = true;
			}
			if (mSeedType != SeedType.Cattail && mSeedType != SeedType.HypnoCattail && mSeedType != SeedType.Endoflame)
			{
				if (mSeedType == SeedType.Gloomshroom)
				{
					if (num2 < -1 || num2 > 1)
					{
						continue;
					}
				}
				else if (flag)
				{
					if (!mBoard.mChallenge.CanTargetZombieWithPortals(this, zombie2))
					{
						continue;
					}
				}
				else if (num2 != 0)
				{
					continue;
				}
			}
			if (!zombie2.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			int num3 = 0;
			if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail || mSeedType == SeedType.Endoflame)
			{
				num3 = Constants.Board_Offset_AspectRatio_Correction;
			}
			if (mSeedType == SeedType.Chomper || mSeedType == SeedType.SuperChomper)
			{
				if (zombie2.mZombiePhase == ZombiePhase.DiggerWalking)
				{
					plantAttackRect.mX += 20;
					plantAttackRect.mWidth -= 20;
				}
				if (zombie2.mZombiePhase == ZombiePhase.PogoBouncing || (zombie2.mZombieType == ZombieType.Bungee && zombie2.mTargetCol == mPlantCol))
				{
					continue;
				}
				if (zombie2.mIsEating || mState == PlantState.ChomperBiting)
				{
					num3 = 60;
				}
			}
			if (mSeedType == SeedType.Potatomine)
			{
				if ((zombie2.mZombieType == ZombieType.Pogo && zombie2.mHasObject) || zombie2.mZombiePhase == ZombiePhase.PolevaulterInVault || zombie2.mZombiePhase == ZombiePhase.PolevaulterPreVault)
				{
					continue;
				}
				if (zombie2.mZombieType == ZombieType.Polevaulter)
				{
					plantAttackRect.mX += 40;
					plantAttackRect.mWidth -= 40;
				}
				if (zombie2.mZombieType == ZombieType.Bungee && zombie2.mTargetCol != mPlantCol)
				{
					continue;
				}
				if (zombie2.mIsEating)
				{
					num3 = 30;
				}
			}
			if (mSeedType == SeedType.Cactus && thePlantWeapon == PlantWeapon.Primary)
			{
				if ((num2 == 0 && zombie2.GetPosYBasedOnRow(mRow) + (float)mHeight - (float)mY < 16f && zombie2.GetZombieRect().mY <= mY - 50) || zombie2.IsFlying() || zombie2.mZombieType == ZombieType.Boss || (!IsOnHighGround() && zombie2.IsOnHighGround()))
				{
					return zombie2;
				}
			}
			else
			{
				if ((mSeedType == SeedType.Gatlingpea && thePlantWeapon == PlantWeapon.Secondary && ((zombie2.mZombieType == ZombieType.Bungee && (zombie2.mTargetCol != mPlantCol || zombie2.mRow != mRow)) || mBoard.GetLadderAt(mBoard.PixelToGridX((int)zombie2.mPosX, (int)zombie2.mPosY), zombie2.mRow) != null)) || (mSeedType == SeedType.ExplodeONut && zombie2.mZombiePhase == ZombiePhase.PolevaulterInVault) || (mSeedType == SeedType.Tanglekelp && !zombie2.mInPool))
				{
					continue;
				}
				TRect zombieRect = zombie2.GetZombieRect();
				if (!flag)
				{
					int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
					if (rectOverlap < -num3)
					{
						continue;
					}
				}
				int num4 = -zombieRect.mX;
				if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
				{
					num4 = -(int)TodCommon.Distance2D((float)mX + 40f, (float)mY + 40f, zombieRect.mX + zombieRect.mWidth / 2, zombieRect.mY + zombieRect.mHeight / 2);
					if (zombie2.IsFlying())
					{
						num4 += 10000;
					}
				}
				if (mSeedType == SeedType.Endoflame)
				{
					num4 = -(int)TodCommon.Distance2D((float)mX + 40f, (float)mY + 40f, zombieRect.mX + zombieRect.mWidth / 2, zombieRect.mY + zombieRect.mHeight / 2);
					if (((float)zombie2.mX < (float)mX + 40f || !zombie2.mHasHead) && zombie2.mZombieType != ZombieType.Boss)
					{
						continue;
					}
				}
				if (zombie == null || num4 > num)
				{
					num = num4;
					zombie = zombie2;
				}
			}
		}
		return zombie;
	}

	public void Die()
	{
		if (IsOnBoard() && mSeedType == SeedType.Tanglekelp)
		{
			mBoard.ZombieTryToGet(mTargetZombieID)?.DieWithLoot();
		}
		mDead = true;
		RemoveEffects();
		if (!IsFlying(mSeedType) && IsOnBoard())
		{
			mBoard.GetLadderAt(mPlantCol, mRow)?.GridItemDie();
		}
		if (IsOnBoard())
		{
			Plant topPlantAt = mBoard.GetTopPlantAt(mPlantCol, mRow, TopPlant.BungeeOrder);
			Plant flowerPotAt = mBoard.GetFlowerPotAt(mPlantCol, mRow);
			if (flowerPotAt != null && mSeedType == SeedType.Endoflame && flowerPotAt.mState == PlantState.FlowerpotEndoflameOverrideNoDraw)
			{
				flowerPotAt.mState = PlantState.Notready;
			}
			if (flowerPotAt != null && topPlantAt == flowerPotAt)
			{
				Reanimation reanimation = mApp.ReanimationGet(flowerPotAt.mBodyReanimID);
				reanimation.mAnimRate = TodCommon.RandRangeFloat(10f, 15f);
			}
		}
	}

	public void UpdateProductionPlant()
	{
		if (!IsInPlay() || mApp.IsIZombieLevel() || mApp.mGameMode == GameMode.Upsell || mApp.mGameMode == GameMode.Intro || mBoard.HasLevelAwardDropped())
		{
			return;
		}
		if (mSeedType == SeedType.Sunflower || mSeedType == SeedType.SunflowerWallnut || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.Marigold)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (reanimation != null && (reanimation.IsTrackShowing("anim_producing") || reanimation.IsTrackShowing("anim_producing2") || reanimation.IsTrackShowing("anim_producing3")) && reanimation.mLoopCount >= 1)
			{
				reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 0f);
			}
		}
		if (mSeedType == SeedType.Marigold && mBoard.mCurrentWave == mBoard.mNumWaves)
		{
			if (mState != PlantState.MarigoldEnding && mState != PlantState.MarigoldProducing)
			{
				mState = PlantState.MarigoldEnding;
				mStateCountdown = 6000;
			}
			else if (mStateCountdown <= 0)
			{
				return;
			}
		}
		if (mSeedType == SeedType.Marigold && mState == PlantState.MarigoldProducing)
		{
			if (mStateCountdown <= 0)
			{
				mBoard.AddCoinForDropLootType(mX + 40 - 5, mY, mGravebusterDrop, CoinMotion.FromPlant);
				mState = PlantState.Notready;
			}
		}
		else
		{
			if (mApp.mGameMode == GameMode.ChallengeLastStand && mBoard.mChallenge.mChallengeState != ChallengeState.LastStandOnslaught)
			{
				return;
			}
			mLaunchCounter--;
			if (mSeedType == SeedType.SunflowerPea && mLaunchCounter % 150 == 100)
			{
				FindTargetAndFire(mRow, PlantWeapon.Primary);
			}
			if (mLaunchCounter <= 100)
			{
				int num = TodCommon.TodAnimateCurve(100, 0, mLaunchCounter, 0, 100, TodCurves.Linear);
				if (mSeedType == SeedType.Sunflower || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.SunflowerWallnut)
				{
					Reanimation reanimation2 = mApp.ReanimationTryToGet(mBodyReanimID);
					if (reanimation2 != null && !reanimation2.IsTrackShowing("anim_producing"))
					{
						reanimation2.PlayReanim("anim_producing", ReanimLoopType.PlayOnceAndHold, 20, 0f);
					}
					num /= 3;
				}
				if (mSeedType == SeedType.Marigold)
				{
					Reanimation reanimation3 = mApp.ReanimationTryToGet(mBodyReanimID);
					if (reanimation3 != null && !reanimation3.IsTrackShowing("anim_producing"))
					{
						reanimation3.PlayReanim("anim_producing", ReanimLoopType.PlayOnceAndHold, 20, 0f);
					}
					num = 0;
				}
				mEatenFlashCountdown = Math.Max(mEatenFlashCountdown, num);
			}
			if (mLaunchCounter > 0)
			{
				return;
			}
			mLaunchCounter = TodCommon.RandRangeInt(mLaunchRate - 150, mLaunchRate);
			mApp.PlayFoley(FoleyType.SpawnSun);
			if (mSeedType == SeedType.Sunshroom)
			{
				if (mState == PlantState.SunshroomSmall)
				{
					mBoard.AddCoin(mX, mY, CoinType.Smallsun, CoinMotion.FromPlant);
				}
				else
				{
					mBoard.AddCoin(mX, mY, CoinType.Sun, CoinMotion.FromPlant);
				}
			}
			else if (mSeedType == SeedType.Sunflower || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.SunflowerWallnut)
			{
				mBoard.AddCoin(mX, mY, CoinType.Sun, CoinMotion.FromPlant);
			}
			else if (mSeedType == SeedType.Twinsunflower)
			{
				mBoard.AddCoin(mX, mY, CoinType.Sun, CoinMotion.FromPlant);
				mBoard.AddCoin(mX, mY, CoinType.Sun, CoinMotion.FromPlant);
			}
			else if (mSeedType == SeedType.Marigold)
			{
				int num2 = RandomNumbers.NextNumber(20000);
				Reanimation reanimation4 = mApp.ReanimationTryToGet(mBodyReanimID);
				CoinType coinType = CoinType.Silver;
				float mAnimTime = reanimation4.mAnimTime;
				if (num2 < 2000)
				{
					coinType = CoinType.Gold;
					reanimation4.PlayReanim("anim_producing2", ReanimLoopType.PlayOnceAndHold, 0, 0f);
				}
				else if (num2 - 2000 < 1)
				{
					coinType = CoinType.Diamond;
					reanimation4.PlayReanim("anim_producing3", ReanimLoopType.PlayOnceAndHold, 0, 0f);
					PlantVoice.Play(mSeedType, mImitaterType, PlantVoice.VoiceType.Diamond);
				}
				else
				{
					reanimation4.PlayReanim("anim_producing", ReanimLoopType.PlayOnceAndHold, 0, 0f);
				}
				reanimation4.mAnimTime = mAnimTime;
				mStateCountdown = 190;
				mState = PlantState.MarigoldProducing;
				mGravebusterDrop = coinType switch
				{
					CoinType.Silver => DropLootType.Silver, 
					CoinType.Gold => DropLootType.Gold, 
					CoinType.Diamond => DropLootType.ThreeDiamonds, 
					_ => DropLootType.None, 
				};
			}
			if (mApp.mGameMode == GameMode.ChallengeBigTime)
			{
				if (mSeedType == SeedType.Sunflower || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.SunflowerWallnut)
				{
					mBoard.AddCoin(mX, mY, CoinType.Sun, CoinMotion.FromPlant);
				}
				else if (mSeedType == SeedType.Marigold)
				{
					mBoard.AddCoin(mX, mY, CoinType.Silver, CoinMotion.Coin);
				}
			}
		}
	}

	public void UpdateShooter()
	{
		mLaunchCounter--;
		if (mLaunchCounter <= 0)
		{
			mLaunchCounter = mLaunchRate - RandomNumbers.NextNumber(15);
			if (mSeedType == SeedType.Threepeater)
			{
				LaunchThreepeater();
			}
			else if (mSeedType == SeedType.Starfruit)
			{
				LaunchStarFruit();
			}
			else if (mSeedType == SeedType.Splitpea)
			{
				Reanimation reanimation = mApp.ReanimationGet(mHeadReanimID);
				Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
				reanimation.StartBlend(20);
				reanimation.mLoopType = ReanimLoopType.Loop;
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
				reanimation.mAnimRate = reanimation2.mAnimRate;
				reanimation.mAnimTime = reanimation2.mAnimTime;
				Reanimation reanimation3 = mApp.ReanimationGet(mHeadReanimID2);
				reanimation3.StartBlend(20);
				reanimation3.mLoopType = ReanimLoopType.Loop;
				reanimation3.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_splitpea_idle);
				reanimation3.mAnimRate = reanimation2.mAnimRate;
				reanimation3.mAnimTime = reanimation2.mAnimTime;
			}
			else if (mSeedType == SeedType.Cactus)
			{
				if (mState == PlantState.CactusHigh)
				{
					if (!mApp.IsIZombieLevel())
					{
						mLaunchCounter /= 2;
					}
					FindTargetAndFire(mRow, PlantWeapon.Primary);
				}
				else if (mState == PlantState.CactusLow)
				{
					FindTargetAndFire(mRow, PlantWeapon.Secondary);
				}
			}
			else
			{
				FindTargetAndFire(mRow, PlantWeapon.Primary);
			}
		}
		if (mLaunchCounter == 65 && (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail))
		{
			FindTargetAndFire(mRow, PlantWeapon.Secondary);
		}
		if (mLaunchCounter == 25)
		{
			if (mSeedType == SeedType.Repeater || mSeedType == SeedType.Leftpeater)
			{
				FindTargetAndFire(mRow, PlantWeapon.Primary, LaunchTime.RepeaterPrelaunch);
				return;
			}
			if (mSeedType == SeedType.Splitpea)
			{
				FindTargetAndFire(mRow, PlantWeapon.Primary);
				FindTargetAndFire(mRow, PlantWeapon.Secondary);
			}
		}
		if (mLaunchCounter == 126 && mSeedType == SeedType.Gatlingpea)
		{
			Reanimation reanimation4 = mApp.ReanimationGet(mHeadReanimID);
			if (FindTargetZombie(mRow, PlantWeapon.Secondary) != null && !mApp.IsIZombieLevel())
			{
				reanimation4.StartBlend(20);
				reanimation4.mLoopType = ReanimLoopType.PlayOnceAndHold;
				reanimation4.SetFramesForLayer("anim_shooting2_pre");
				mState = PlantState.GatlingPreShooting2;
				mStateCountdown = 46;
			}
		}
		if (mLaunchCounter != 63 || mSeedType != SeedType.Gatlingpea)
		{
			return;
		}
		Reanimation reanimation5 = mApp.ReanimationGet(mHeadReanimID);
		if (FindTargetZombie(mRow, PlantWeapon.Primary) != null)
		{
			if (mState == PlantState.Notready)
			{
				reanimation5.StartBlend(20);
				reanimation5.mLoopType = ReanimLoopType.PlayOnceAndHold;
				reanimation5.SetFramesForLayer("anim_shooting_pre");
				mState = PlantState.GatlingPreShooting1;
			}
		}
		else if (mState == PlantState.GatlingIdle1)
		{
			reanimation5.StartBlend(20);
			reanimation5.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation5.SetFramesForLayer("anim_shooting_after");
			mState = PlantState.GatlingAfterShooting1;
		}
	}

	public bool FindTargetAndFire(int theRow, PlantWeapon thePlantWeapon)
	{
		return FindTargetAndFire(theRow, thePlantWeapon, LaunchTime.Normal);
	}

	public bool FindTargetAndFire(int theRow, PlantWeapon thePlantWeapon, LaunchTime theLaunchTime)
	{
		Zombie zombie = FindTargetZombie(theRow, thePlantWeapon);
		if (zombie == null)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mHeadReanimID);
			if (mSeedType == SeedType.Gatlingpea && mState == PlantState.GatlingPreShooting1)
			{
				reanimation.StartBlend(20);
				reanimation.mLoopType = ReanimLoopType.Loop;
				reanimation.SetFramesForLayer("anim_head_idle1");
				mState = PlantState.GatlingIdle1;
			}
			if (mSeedType == SeedType.Gatlingpea && mState == PlantState.GatlingAfterShooting1)
			{
				reanimation.StartBlend(20);
				reanimation.mLoopType = ReanimLoopType.Loop;
				reanimation.SetFramesForLayer("anim_head_idle");
				mState = PlantState.Notready;
				return false;
			}
			return false;
		}
		if (mSeedType == SeedType.Gatlingpea && (mState == PlantState.GatlingAfterShooting1 || mState == PlantState.Notready))
		{
			mState = PlantState.GatlingPreShooting1;
		}
		EndBlink();
		Reanimation reanimation2 = mApp.ReanimationTryToGet(mBodyReanimID);
		Reanimation reanimation3 = mApp.ReanimationTryToGet(mHeadReanimID);
		if (mSeedType == SeedType.Splitpea && thePlantWeapon == PlantWeapon.Secondary)
		{
			Reanimation reanimation4 = mApp.ReanimationGet(mHeadReanimID2);
			reanimation4.StartBlend(20);
			reanimation4.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation4.mAnimRate = 35f;
			reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_splitpea_shooting);
			mShootingCounter = 26;
		}
		else if (reanimation3 != null && reanimation3.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_shooting))
		{
			if ((mSeedType == SeedType.Repeater || mSeedType == SeedType.Leftpeater) && theLaunchTime != LaunchTime.RepeaterPrelaunch)
			{
				mShootingCounter = 26;
				return true;
			}
			reanimation3.StartBlend(20);
			reanimation3.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation3.mAnimRate = 35f;
			reanimation3.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_shooting);
			mShootingCounter = 33;
			if (mSeedType == SeedType.Repeater || mSeedType == SeedType.Leftpeater)
			{
				reanimation3.mAnimRate = 35f;
				mShootingCounter = 26;
			}
			if (mSeedType == SeedType.Splitpea)
			{
				reanimation3.mAnimRate = 45f;
				mShootingCounter = 26;
			}
			else if (mSeedType == SeedType.Gatlingpea)
			{
				reanimation3.mAnimRate = 30f;
				mShootingCounter = 100;
			}
		}
		else if (mState == PlantState.CactusHigh)
		{
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shootinghigh, ReanimLoopType.PlayOnceAndHold, 20, 35f);
			mShootingCounter = 23;
		}
		else if (mSeedType == SeedType.Gloomshroom)
		{
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 14f);
			mShootingCounter = 200;
		}
		else if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
		{
			if (thePlantWeapon == PlantWeapon.Secondary)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 10, 30f);
				mShootingCounter = 50;
			}
			else
			{
				mShootingCounter = 150;
			}
		}
		else if (reanimation2 != null && reanimation2.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_shooting))
		{
			string theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_shooting;
			float theAnimRate = 35f;
			if (mSeedType == SeedType.Puffshroom)
			{
				if (RandomNumbers.NextNumber(2) == 1)
				{
					theTrackName = "anim_shooting2";
				}
				theAnimRate = 30f;
			}
			PlayBodyReanim(theTrackName, ReanimLoopType.PlayOnceAndHold, 20, theAnimRate);
			if (mSeedType == SeedType.Fumeshroom)
			{
				mShootingCounter = 50;
			}
			else if (mSeedType == SeedType.Puffshroom)
			{
				mShootingCounter = 29;
			}
			else if (mSeedType == SeedType.Scaredyshroom)
			{
				mShootingCounter = 25;
			}
			else if (mSeedType == SeedType.Cabbagepult)
			{
				mShootingCounter = 32;
			}
			else if (mSeedType == SeedType.Melonpult || mSeedType == SeedType.Wintermelon)
			{
				mShootingCounter = 36;
			}
			else if (mSeedType == SeedType.Kernelpult)
			{
				if (RandomNumbers.NextNumber(4) == 0)
				{
					reanimation2 = mApp.ReanimationGet(mBodyReanimID);
					reanimation2.PlayReanim("anim_shooting2", ReanimLoopType.PlayOnceAndHold, 20, 35f);
					mState = PlantState.KernelpultButter;
				}
				mShootingCounter = 30;
			}
			else if (mSeedType == SeedType.Cactus)
			{
				mShootingCounter = 35;
			}
			else if (mSeedType == SeedType.Endoflame)
			{
				mShootingCounter = 75;
			}
			else
			{
				mShootingCounter = 29;
			}
		}
		else
		{
			Fire(zombie, theRow, thePlantWeapon);
		}
		return true;
	}

	public void LaunchThreepeater()
	{
		int theRow = mRow - 1;
		int theRow2 = mRow + 1;
		bool flag = false;
		if (FindTargetZombie(mRow, PlantWeapon.Primary) != null)
		{
			flag = true;
		}
		else if (mBoard.RowCanHaveZombies(theRow) && FindTargetZombie(theRow, PlantWeapon.Primary) != null)
		{
			flag = true;
		}
		else if (mBoard.RowCanHaveZombies(theRow2) && FindTargetZombie(theRow2, PlantWeapon.Primary) != null)
		{
			flag = true;
		}
		if (flag)
		{
			Reanimation reanimation = mApp.ReanimationGet(mHeadReanimID);
			Reanimation reanimation2 = mApp.ReanimationGet(mHeadReanimID2);
			Reanimation reanimation3 = mApp.ReanimationGet(mHeadReanimID3);
			if (mBoard.RowCanHaveZombies(theRow2))
			{
				reanimation.StartBlend(10);
				reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
				reanimation.mAnimRate = 20f;
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_shooting1);
			}
			reanimation2.StartBlend(10);
			reanimation2.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation2.mAnimRate = 20f;
			reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_shooting2);
			if (mBoard.RowCanHaveZombies(theRow))
			{
				reanimation3.StartBlend(10);
				reanimation3.mLoopType = ReanimLoopType.PlayOnceAndHold;
				reanimation3.mAnimRate = 20f;
				reanimation3.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_shooting3);
			}
			mShootingCounter = 35;
		}
	}

	public static int GetBaseHealth(SeedType type)
	{
		switch (type)
		{
		case SeedType.Wallnut:
		case SeedType.Pumpkinshell:
		case SeedType.ExplodeONut:
		case SeedType.GiantWallnut:
		case SeedType.PeaWallnut:
		case SeedType.SunflowerWallnut:
		case SeedType.IceWallnut:
			return 4000;
		case SeedType.Tallnut:
			return 8000;
		case SeedType.Garlic:
			return 400;
		case SeedType.Spikerock:
			return 450;
		case SeedType.FireShroom:
			return 2000;
		default:
			return 300;
		}
	}

	public static Image GetImage(SeedType theSeedtype)
	{
		PlantDefinition plantDefinition = GetPlantDefinition(theSeedtype);
		if (plantDefinition.mPlantImage == null)
		{
			return null;
		}
		return plantDefinition.mPlantImage[0];
	}

	public static int GetCost(SeedType theSeedType, SeedType theImitaterType)
	{
		if (GlobalStaticVars.gLawnApp.IsLevelUseJson())
		{
			CSOverridePlantProperty component = GlobalStaticVars.gLawnApp.mCreativeLevel.GetComponent<CSOverridePlantProperty>();
			if (component != null)
			{
				if (component.mProperties.TryGetValue(theSeedType, out var value) && value.mSeedCost.HasValue)
				{
					return value.mSeedCost.GetValueOrDefault();
				}
				if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None && component.mProperties.TryGetValue(theImitaterType, out var value2) && value2.mSeedCost.HasValue)
				{
					return value2.mSeedCost.GetValueOrDefault();
				}
			}
		}
		if (GlobalStaticVars.gLawnApp.mGameMode == GameMode.ChallengeBeghouled || GlobalStaticVars.gLawnApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			switch (theSeedType)
			{
			case SeedType.Repeater:
				return 1000;
			case SeedType.Fumeshroom:
				return 500;
			case SeedType.Tallnut:
				return 250;
			case SeedType.BeghouledButtonShuffle:
				return 100;
			case SeedType.BeghouledButtonCrater:
				return 200;
			}
		}
		switch (theSeedType)
		{
		case SeedType.SlotMachineSun:
			return 0;
		case SeedType.SlotMachineDiamond:
			return 0;
		case SeedType.ZombiquariumSnorkel:
			return 100;
		case SeedType.ZombiquariumTrophy:
			return 1000;
		case SeedType.CornRouletteChocolate:
			return 0;
		case SeedType.CornRouletteWatercan:
			return 0;
		case SeedType.CornRouletteFertilizer:
			return 0;
		case SeedType.CornRouletteMagnifier:
			return 0;
		case SeedType.ZombieNormal:
		case SeedType.ZombieCardFlag:
		case SeedType.MindControlledCardFlag:
			return 50;
		case SeedType.ZombieTrafficCone:
		case (SeedType)536870914:
		case (SeedType)1073741826:
			return 75;
		case SeedType.ZombiePolevaulter:
		case (SeedType)536870915:
		case (SeedType)1073741827:
			return 75;
		case SeedType.ZombiePail:
		case (SeedType)536870916:
		case (SeedType)1073741828:
			return 125;
		case SeedType.ZombieLadder:
		case (SeedType)536870933:
		case (SeedType)1073741845:
			return 150;
		case SeedType.ZombieDigger:
		case (SeedType)536870929:
		case (SeedType)1073741841:
			return 125;
		case SeedType.ZombieBungee:
		case (SeedType)536870932:
		case (SeedType)1073741844:
			return 125;
		case SeedType.ZombieFootball:
		case (SeedType)536870919:
		case (SeedType)1073741831:
			return 175;
		case SeedType.ZombieBalloon:
		case (SeedType)536870928:
		case (SeedType)1073741840:
			return 150;
		case SeedType.ZombieScreenDoor:
		case (SeedType)536870918:
		case (SeedType)1073741830:
			return 100;
		case SeedType.Zomboni:
		case (SeedType)536870924:
		case (SeedType)1073741836:
			return 175;
		case SeedType.ZombiePogo:
		case (SeedType)536870930:
		case (SeedType)1073741842:
			return 200;
		case SeedType.ZombieDancer:
		case (SeedType)536870920:
		case (SeedType)1073741832:
			return 350;
		case SeedType.ZombieGargantuar:
		case (SeedType)536870935:
		case (SeedType)1073741847:
			return 300;
		case SeedType.ZombieImp:
		case (SeedType)536870936:
		case (SeedType)1073741848:
			return 50;
		default:
		{
			if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
			{
				PlantDefinition plantDefinition = GetPlantDefinition(theImitaterType);
				return plantDefinition.mSeedCost;
			}
			PlantDefinition plantDefinition2 = GetPlantDefinition(theSeedType);
			return plantDefinition2.mSeedCost;
		}
		}
	}

	public static string GetNameString(SeedType theSeedtype, SeedType theImitaterType)
	{
		PlantDefinition plantDefinition = GetPlantDefinition(theSeedtype);
		string theString = Common.StrFormat_("[{0}]", plantDefinition.mPlantName);
		string result = TodStringFile.TodStringTranslate(theString);
		if (theSeedtype == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			PlantDefinition plantDefinition2 = GetPlantDefinition(theImitaterType);
			string theString2 = Common.StrFormat_("[{0}]", plantDefinition2.mPlantName);
			string theStringToSubstitute = TodStringFile.TodStringTranslate(theString2);
			return TodCommon.TodReplaceString(TodStringFile.TodStringTranslate("[IMITATED_PLANT]"), "{PLANT}", theStringToSubstitute);
		}
		return result;
	}

	public static string GetToolTip(SeedType theSeedType)
	{
		PlantDefinition plantDefinition = GetPlantDefinition(theSeedType);
		string theString = "[" + plantDefinition.mPlantName + "_TOOLTIP]";
		return TodStringFile.TodStringTranslate(theString);
	}

	public static int GetRefreshTime(SeedType theSeedType, SeedType theImitaterType)
	{
		if (GlobalStaticVars.gLawnApp.IsLevelUseJson())
		{
			CSOverridePlantProperty component = GlobalStaticVars.gLawnApp.mCreativeLevel.GetComponent<CSOverridePlantProperty>();
			if (component != null)
			{
				if (component.mProperties.TryGetValue(theSeedType, out var value) && value.mRefreshTime.HasValue)
				{
					return value.mRefreshTime.GetValueOrDefault();
				}
				if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None && component.mProperties.TryGetValue(theImitaterType, out var value2) && value2.mRefreshTime.HasValue)
				{
					return value2.mRefreshTime.GetValueOrDefault();
				}
			}
		}
		if (Challenge.IsZombieSeedType(theSeedType))
		{
			return 0;
		}
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			PlantDefinition plantDefinition = GetPlantDefinition(theImitaterType);
			return plantDefinition.mRefreshTime;
		}
		PlantDefinition plantDefinition2 = GetPlantDefinition(theSeedType);
		return plantDefinition2.mRefreshTime;
	}

	public static bool IsNocturnal(SeedType theSeedtype)
	{
		if (theSeedtype != SeedType.Puffshroom && theSeedtype != SeedType.Seashroom && theSeedtype != SeedType.Sunshroom && theSeedtype != SeedType.Fumeshroom && theSeedtype != SeedType.Hypnoshroom && theSeedtype != SeedType.Doomshroom && theSeedtype != SeedType.Iceshroom && theSeedtype != SeedType.Magnetshroom && theSeedtype != SeedType.Scaredyshroom && theSeedtype != SeedType.Gloomshroom)
		{
			return theSeedtype == SeedType.FireShroom;
		}
		return true;
	}

	public static bool IsAquatic(SeedType theSeedType)
	{
		if (theSeedType != SeedType.Lilypad && theSeedType != SeedType.Tanglekelp && theSeedType != SeedType.Seashroom && theSeedType != SeedType.Cattail)
		{
			return theSeedType == SeedType.HypnoCattail;
		}
		return true;
	}

	public static bool IsFlying(SeedType theSeedtype)
	{
		return theSeedtype == SeedType.InstantCoffee;
	}

	public static bool IsUpgrade(SeedType theSeedtype)
	{
		if (theSeedtype != SeedType.Gatlingpea && theSeedtype != SeedType.Wintermelon && theSeedtype != SeedType.Twinsunflower && theSeedtype != SeedType.Spikerock && theSeedtype != SeedType.Cobcannon && theSeedtype != SeedType.GoldMagnet && theSeedtype != SeedType.Gloomshroom && theSeedtype != SeedType.Cattail)
		{
			return theSeedtype == SeedType.SuperChomper;
		}
		return true;
	}

	// ---------------------------------------------------------------------------------
	// Iris Shooter (iOS port original plant)
	//   Scan 1.0 s (1.5 s when the face is hidden by a newspaper or the pool), 10 damage
	//   every 0.5 s while scanning, then 30 armour-ignoring damage every 0.5 s.
	//   Range: 6 tiles in front. Armoured zombies are targeted first.
	// ---------------------------------------------------------------------------------
	public const int IRIS_RANGE_TILES = 6;

	public const int IRIS_SCAN_TIME = 100;

	public const int IRIS_SCAN_TIME_OBSTRUCTED = 150;

	public const int IRIS_TICK = 50;

	public const int IRIS_SCAN_DAMAGE = 10;

	public const int IRIS_LASER_DAMAGE = 30;

	public TRect GetIrisRangeRect()
	{
		return new TRect(mX + 20, mY, 80 * IRIS_RANGE_TILES + 60, mHeight);
	}

	public bool IrisCanTarget(Zombie theZombie)
	{
		if (theZombie == null || theZombie.mDead || theZombie.IsDeadOrDying())
		{
			return false;
		}
		if (theZombie.mRow != mRow && theZombie.mZombieType != ZombieType.Boss)
		{
			return false;
		}
		if (!theZombie.EffectedByDamage((uint)GetDamageRangeFlags(PlantWeapon.Primary)))
		{
			return false;
		}
		if (theZombie.mZombiePhase == ZombiePhase.PogoBouncing || theZombie.IsFlying())
		{
			return false;
		}
		TRect zombieRect = theZombie.GetZombieRect();
		return GameConstants.GetRectOverlap(GetIrisRangeRect(), zombieRect) >= 0;
	}

	public static bool IrisIsArmoured(Zombie theZombie)
	{
		return theZombie.mHelmType != HelmType.None || theZombie.mShieldType != ShieldType.None;
	}

	public static bool IrisFaceHidden(Zombie theZombie)
	{
		return theZombie.mShieldType == ShieldType.Newspaper || theZombie.mInPool;
	}

	public Zombie IrisFindTarget()
	{
		Zombie best = null;
		bool bestArmoured = false;
		int bestX = int.MaxValue;
		for (int i = 0; i < mBoard.mZombies.Count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!IrisCanTarget(zombie))
			{
				continue;
			}
			bool armoured = IrisIsArmoured(zombie);
			int x = zombie.GetZombieRect().mX;
			if (best == null || (armoured && !bestArmoured) || (armoured == bestArmoured && x < bestX))
			{
				best = zombie;
				bestArmoured = armoured;
				bestX = x;
			}
		}
		return best;
	}

	public void IrisResetLock()
	{
		mTargetZombieID = null;
		mIrisScanCounter = 0;
		mIrisTickCounter = 0;
		mIrisUnlocked = false;
	}

	public static void IrisDealTrueDamage(Zombie theZombie, int theDamage)
	{
		if (theZombie.mZombiePhase == ZombiePhase.JackInTheBoxPopping || theZombie.IsDeadOrDying())
		{
			return;
		}
		// Skips shields (screen door, newspaper, ladder) and helmets (cone, bucket, football): straight to the body.
		theZombie.TakeBodyDamage(theDamage, 0u);
	}

	public void UpdateIrisShooter()
	{
		if (mIrisFlashCounter > 0)
		{
			mIrisFlashCounter--;
		}
		// arm raise / lower blend (0..IRIS_ENGAGE_TIME) drives the raise animation and the cannon glide
		if (mTargetZombieID != null && !mTargetZombieID.IsDeadOrDying())
		{
			mIrisEngage = Math.Min(IRIS_ENGAGE_TIME, mIrisEngage + 1);
		}
		else
		{
			mIrisEngage = Math.Max(0, mIrisEngage - 1);
		}
		if (NotOnGround() || mState == PlantState.Dying || mSquished)
		{
			return;
		}
		Zombie target = mTargetZombieID;
		if (!IrisCanTarget(target))
		{
			IrisResetLock();
			target = IrisFindTarget();
			if (target == null)
			{
				return;
			}
			mTargetZombieID = target;
		}
		mIrisTickCounter++;
		if (!mIrisUnlocked)
		{
			mIrisScanCounter++;
			if (mIrisTickCounter >= IRIS_TICK)
			{
				mIrisTickCounter = 0;
				target.TakeDamage(IRIS_SCAN_DAMAGE, 0u);
			}
			int scanTime = (IrisFaceHidden(target) ? IRIS_SCAN_TIME_OBSTRUCTED : IRIS_SCAN_TIME);
			if (mIrisScanCounter >= scanTime)
			{
				mIrisUnlocked = true;
				mIrisTickCounter = 0;
				mIrisFlashCounter = 40;
				mApp.PlayFoley(FoleyType.Bleep);
			}
		}
		else if (mIrisTickCounter >= IRIS_TICK)
		{
			mIrisTickCounter = 0;
			IrisDealTrueDamage(target, IRIS_LASER_DAMAGE);
		}
	}

	public const int IRIS_ENGAGE_TIME = 50;

	private int IrisCounter()
	{
		return (IsOnBoard() ? mBoard.mMainCounter : mApp.mAppCounter) + mRow * 53 + mPlantCol * 37;
	}

	/// <summary>Where the cannon is aiming (board units, plant-relative) and which pre-rendered angle to show.</summary>
	private void IrisAim(float theHeightOffset, out Vector2 theCenter, out int theIndex, out Vector2 theAimPoint)
	{
		float engage = (float)mIrisEngage / (float)IRIS_ENGAGE_TIME;
		theCenter = IrisArt.CannonCenter(IrisCounter(), engage, theHeightOffset);
		if (mIrisUnlocked && mIrisTickCounter < 8)
		{
			theCenter.X -= 2.5f * (1f - (float)mIrisTickCounter / 8f);     // recoil on each laser tick
		}
		Zombie target = mTargetZombieID;
		theAimPoint = new Vector2(theCenter.X + 100f, theCenter.Y);
		if (target != null && !target.IsDeadOrDying() && engage > 0f)
		{
			TRect rect = target.GetZombieRect();
			float faceX = (float)rect.mX + (float)rect.mWidth * 0.5f - (float)mX;
			float faceY = (float)rect.mY + (float)rect.mHeight * 0.22f - (float)mY + theHeightOffset;
			if (!mIrisUnlocked)
			{
				// sweep the scan up and down the zombie, two passes per scan
				float sweep = 0.5f - 0.5f * (float)Math.Cos((float)mIrisScanCounter * Math.PI * 4.0 / (float)IRIS_SCAN_TIME);
				faceY = (float)rect.mY - (float)mY + theHeightOffset + (float)rect.mHeight * (0.08f + 0.84f * sweep);
			}
			Vector2 face = new Vector2(faceX, faceY);
			theAimPoint = Vector2.Lerp(theAimPoint, face, engage);
		}
		float deg = MathHelper.ToDegrees((float)Math.Atan2(theAimPoint.Y - theCenter.Y, Math.Max(1f, theAimPoint.X - theCenter.X)));
		theIndex = IrisArt.AngleIndex(deg);
	}

	/// <summary>Iois (pre-baked rig) + floating apple cannon.</summary>
	public void DrawIrisShooterBody(Graphics g, float theOffsetX, float theOffsetY)
	{
		int counter = IrisCounter();
		int f12 = counter * IrisArt.FPS / 100;
		Image sheet;
		int frame;
		if (mIrisEngage <= 0)
		{
			sheet = IrisArt.Idle;
			frame = f12 % IrisArt.IDLE_FRAMES;
		}
		else if (mIrisEngage < IRIS_ENGAGE_TIME)
		{
			sheet = IrisArt.Raise;
			frame = Math.Clamp(mIrisEngage * IrisArt.RAISE_FRAMES / IRIS_ENGAGE_TIME, 0, IrisArt.RAISE_FRAMES - 1);
		}
		else if (mIrisUnlocked && mIrisFlashCounter > 26 && IrisArt.Wink != null)
		{
			sheet = IrisArt.Wink;
			frame = 0;
		}
		else
		{
			sheet = mIrisUnlocked ? IrisArt.FireLoop : IrisArt.ScanLoop;
			frame = f12 % IrisArt.LOOP_FRAMES;
		}
		bool colorize = g.GetColorizeImages();
		if (mEatenFlashCountdown > 0 || mBeghouledFlashCountdown > 0)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 200, 200, 255));
		}
		float ox = theOffsetX * Constants.S;
		float oy = theOffsetY * Constants.S;
		IrisArt.DrawCharacter(g, sheet, frame, ox, oy, 1f);
		IrisAim(0f, out Vector2 center, out int index, out _);
		IrisArt.DrawCannon(g, center, index, ox, oy);
		g.SetColor(new SexyColor(255, 255, 255, 255));
		g.SetColorizeImages(colorize);
	}

	/// <summary>Red scan fan + lock while scanning; violet laser from the cannon, hit spark and green lock once unlocked.</summary>
	public void DrawIrisShooterEffectsArt(Graphics g, Zombie target)
	{
		if (mIrisEngage < IRIS_ENGAGE_TIME)
		{
			return;
		}
		float s = Constants.S;
		float heightOffset = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
		TRect rect = target.GetZombieRect();
		float faceCX = ((float)rect.mX + (float)rect.mWidth * 0.5f - (float)mX) * s;
		float faceCY = ((float)rect.mY + (float)rect.mHeight * 0.22f - (float)mY + heightOffset) * s;
		float side = Math.Max((float)rect.mWidth * 0.7f, (float)rect.mHeight * 0.38f) * s;
		int counter = mBoard.mMainCounter;
		float pulse = 0.5f + 0.5f * (float)Math.Sin((float)counter * 0.3f);
		bool colorize = g.GetColorizeImages();
		IrisAim(heightOffset, out Vector2 center, out int index, out Vector2 aim);
		Vector2 mz = IrisArt.MuzzleAt(center, index) * s;
		Vector2 aimPx = aim * s;
		if (!mIrisUnlocked)
		{
			if (IrisArt.Scan != null && aimPx.X - mz.X > 4f)
			{
				float h = side * 1.1f;
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(255, 255, 255, (int)(150f + 60f * pulse)));
				float fanH = Math.Abs(aimPx.Y - mz.Y) * 2f + h;
				g.DrawImage(IrisArt.Scan, new TRect((int)mz.X, (int)(mz.Y - fanH * 0.52f), (int)(aimPx.X - mz.X + side * 0.3f), (int)fanH), new TRect(0, 0, IrisArt.Scan.mWidth, IrisArt.Scan.mHeight));
				g.SetColor(new SexyColor(255, 60, 80, 220));
				g.FillRect((int)(faceCX - side * 0.6f), (int)aimPx.Y, (int)(side * 1.2f), (int)Math.Max(1f, 1.5f * s));
			}
			IrisArt.DrawCentered(g, IrisArt.LockRed, faceCX, faceCY, side, side, 230);
		}
		else
		{
			Vector2 hit = new Vector2(faceCX - side * 0.15f, aimPx.Y);
			float bh = (14f + 3f * pulse) * s;
			IrisArt.DrawBand(g, IrisArt.Beam, mz, hit, bh, 255);
			float m = (22f + 4f * pulse) * s;
			IrisArt.DrawCentered(g, IrisArt.Muzzle, mz.X, mz.Y, m * 1.3f, m, 255);
			float hs = (22f + 8f * (float)((counter / 3) % 2)) * s;
			IrisArt.DrawCentered(g, IrisArt.Hit, hit.X, hit.Y, hs * 1.4f, hs, 255);
			IrisArt.DrawCentered(g, IrisArt.LockGreen, faceCX, faceCY, side, side, 240);
			if (mIrisFlashCounter > 0)
			{
				float t = (float)mIrisFlashCounter / 40f;
				float fs = side * (1.6f - 0.6f * t);
				IrisArt.DrawCentered(g, IrisArt.Flash, faceCX, faceCY, fs, fs, (int)(255f * t));
			}
		}
		g.SetColor(new SexyColor(255, 255, 255, 255));
		g.SetColorizeImages(colorize);
	}

	private static void IrisFrame(Graphics g, float x, float y, float w, float h, float t)
	{
		g.FillRect((int)x, (int)y, (int)w, (int)t);
		g.FillRect((int)x, (int)(y + h - t), (int)w, (int)t);
		g.FillRect((int)x, (int)y, (int)t, (int)h);
		g.FillRect((int)(x + w - t), (int)y, (int)t, (int)h);
	}

	public void DrawIrisShooterEffects(Graphics g)
	{
		if (!IsInPlay() || mBoard == null)
		{
			return;
		}
		Zombie target = mTargetZombieID;
		if (target == null || target.mDead || target.IsDeadOrDying())
		{
			return;
		}
		if (IrisArt.Ready)
		{
			DrawIrisShooterEffectsArt(g, target);
			return;
		}
		float s = Constants.S;
		float heightOffset = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
		TRect rect = target.GetZombieRect();
		float faceX = ((float)rect.mX + (float)rect.mWidth * 0.2f - (float)mX) * s;
		float faceY = ((float)rect.mY - (float)mY + heightOffset) * s;
		float faceW = (float)rect.mWidth * 0.6f * s;
		float faceH = (float)rect.mHeight * 0.4f * s;
		float mouthX = 62f * s;
		float mouthY = (32f + heightOffset) * s;
		float targetX = faceX + faceW * 0.5f;
		float targetY = faceY + faceH * 0.6f;
		float pulse = (float)(mBoard.mMainCounter % 20) / 20f;
		bool colorize = g.GetColorizeImages();
		if (!mIrisUnlocked)
		{
			int scanTime = (IrisFaceHidden(target) ? IRIS_SCAN_TIME_OBSTRUCTED : IRIS_SCAN_TIME);
			float progress = Math.Clamp((float)mIrisScanCounter / (float)scanTime, 0f, 1f);
			g.SetColor(new SexyColor(255, 40, 40, 70));
			g.FillRect((int)mouthX, (int)(mouthY - 1f * s), (int)Math.Max(0f, targetX - mouthX), (int)Math.Max(1f, 2f * s));
			g.SetColor(new SexyColor(255, 50, 50, 200));
			IrisFrame(g, faceX, faceY, faceW, faceH, Math.Max(1f, 2f * s));
			g.SetColor(new SexyColor(255, 60, 60, 120));
			for (int i = 1; i < 4; i++)
			{
				g.FillRect((int)faceX, (int)(faceY + faceH * (float)i / 4f), (int)faceW, (int)Math.Max(1f, s));
				g.FillRect((int)(faceX + faceW * (float)i / 4f), (int)faceY, (int)Math.Max(1f, s), (int)faceH);
			}
			g.SetColor(new SexyColor(255, 120, 120, 230));
			g.FillRect((int)faceX, (int)(faceY + faceH * progress), (int)faceW, (int)Math.Max(1f, 2f * s));
		}
		else
		{
			float beamW = Math.Max(0f, targetX - mouthX);
			int glow = (int)(140f + 60f * pulse);
			g.SetColor(new SexyColor(255, 205, 110, glow));
			g.FillRect((int)mouthX, (int)(mouthY - 5f * s), (int)beamW, (int)(10f * s));
			g.SetColor(new SexyColor(255, 245, 210, 235));
			g.FillRect((int)mouthX, (int)(mouthY - 2f * s), (int)beamW, (int)Math.Max(1f, 4f * s));
			g.SetColor(new SexyColor(70, 230, 90, 230));
			IrisFrame(g, faceX, faceY, faceW, faceH, Math.Max(1f, 2.5f * s));
		}
		g.SetColor(new SexyColor(255, 255, 255, 255));
		g.SetColorizeImages(colorize);
	}

	public void UpdateAbilities()
	{
		if (!IsInPlay())
		{
			return;
		}
		if (mState == PlantState.Doingspecial || mSquished || mState == PlantState.Dying)
		{
			mDisappearCountdown--;
			if (mDisappearCountdown < 0)
			{
				Die();
				return;
			}
		}
		if (mWakeUpCounter > 0)
		{
			mWakeUpCounter--;
			if (mWakeUpCounter == 60)
			{
				mApp.PlayFoley(FoleyType.Wakeup);
			}
			if (mWakeUpCounter == 0)
			{
				SetSleeping(theIsAsleep: false);
			}
		}
		if (mSealedCountdown > 0)
		{
			mSealedCountdown--;
			if (mSealedCountdown == 0)
			{
				SetSealing(theIsSealed: false);
			}
		}
		if (mInTalismanCounter > 0)
		{
			mInTalismanCounter--;
		}
		if (IsDisabled() || mSquished || mOnBungeeState != PlantOnBungeeState.NotOnBungee)
		{
			return;
		}
		if (mState == PlantState.Entering || mState == PlantState.AfterEntering)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			Reanimation reanimation2 = mApp.ReanimationTryToGet(mHeadReanimID);
			if (reanimation2 != null && reanimation2.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation2.mLoopCount > 0)
			{
				reanimation2.StartBlend(20);
				reanimation2.mLoopType = ReanimLoopType.Loop;
				reanimation2.mLoopCount = 0;
				reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
				reanimation2.mAnimRate = reanimation2.mDefinition.mFPS;
				mState = PlantState.Notready;
			}
			if (reanimation != null && reanimation.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation.mLoopCount > 0)
			{
				if (mSeedType == SeedType.Tallnut && mState != PlantState.AfterEntering)
				{
					PlayBodyReanim("anim_repair", ReanimLoopType.PlayOnceAndHold, 20, 30f);
					mState = PlantState.AfterEntering;
				}
				else
				{
					PlayIdleAnim(reanimation.mDefinition.mFPS);
					mState = PlantState.Notready;
				}
			}
			if (mState != PlantState.Entering)
			{
				return;
			}
			if (mSeedType == SeedType.Spikerock)
			{
				if (reanimation.ShouldTriggerTimedEvent(0.7f))
				{
					mBoard.ShakeBoard(3, -4);
				}
			}
			else if (mSeedType == SeedType.Gatlingpea && reanimation2.ShouldTriggerTimedEvent(0.05f))
			{
				mBoard.ShakeBoard(3, -4);
				DoGatlingDamage(PlantWeapon.Secondary, 80, 70);
			}
			return;
		}
		UpdateShooting();
		if (mStateCountdown > 0)
		{
			mStateCountdown--;
		}
		if (mApp.IsWallnutBowlingLevel() || mSeedType == SeedType.GiantWallnut)
		{
			UpdateBowling();
			return;
		}
		if (mSeedType == SeedType.Squash)
		{
			UpdateSquash();
		}
		else if (mSeedType == SeedType.Doomshroom)
		{
			UpdateDoomShroom();
		}
		else if (mSeedType == SeedType.Iceshroom)
		{
			UpdateIceShroom();
		}
		else if (mSeedType == SeedType.Chomper)
		{
			UpdateChomper();
		}
		else if (mSeedType == SeedType.Plantern)
		{
			UpdatePlantern();
		}
		else if (mSeedType == SeedType.Blover)
		{
			UpdateBlover();
		}
		else if (mSeedType == SeedType.Flowerpot)
		{
			UpdateFlowerPot();
		}
		else if (mSeedType == SeedType.Lilypad)
		{
			UpdateLilypad();
		}
		else if (mSeedType == SeedType.Imitater || mSeedType == SeedType.ImitaterRandomPlant || mSeedType == SeedType.ImitaterRandomZombie)
		{
			UpdateImitater();
		}
		else if (mSeedType == SeedType.InstantCoffee)
		{
			UpdateCoffeeBean();
		}
		else if (mSeedType == SeedType.Umbrella)
		{
			UpdateUmbrella();
		}
		else if (mSeedType == SeedType.Cobcannon)
		{
			UpdateCobCannon();
		}
		else if (mSeedType == SeedType.Cactus)
		{
			UpdateCactus();
		}
		else if (mSeedType == SeedType.Magnetshroom)
		{
			UpdateMagnetShroom();
		}
		else if (mSeedType == SeedType.GoldMagnet)
		{
			UpdateGoldMagnetShroom();
		}
		else if (mSeedType == SeedType.Sunshroom)
		{
			UpdateSunShroom();
		}
		else if (MakesSun() || mSeedType == SeedType.Marigold)
		{
			UpdateProductionPlant();
		}
		else if (mSeedType == SeedType.Gravebuster)
		{
			UpdateGraveBuster();
		}
		else if (mSeedType == SeedType.Torchwood)
		{
			UpdateTorchwood();
		}
		else if (mSeedType == SeedType.Potatomine)
		{
			UpdatePotato();
		}
		else if (mSeedType == SeedType.Spikeweed || mSeedType == SeedType.Spikerock)
		{
			UpdateSpikeweed();
		}
		else if (mSeedType == SeedType.Tanglekelp)
		{
			UpdateTanglekelp();
		}
		else if (mSeedType == SeedType.Scaredyshroom)
		{
			UpdateScaredyShroom();
		}
		else if (mSeedType == SeedType.SuperChomper)
		{
			UpdateSuperChomper();
		}
		else if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
		{
			if (mSeedType == SeedType.HypnoCattail && mStateCountdown == 100 && mState == PlantState.LilypadInvulnerable)
			{
				foreach (Zombie mZombie in mBoard.mZombies)
				{
					TRect zombieRect = mZombie.GetZombieRect();
					if (mZombie.mRow > mRow - 2 && mZombie.mRow < mRow + 2 && GameConstants.GetCircleRectOverlap(mX, mY, 115, zombieRect))
					{
						mZombie.ApplyMindControl();
					}
				}
			}
			if (mStateCountdown == 0 && mState == PlantState.LilypadInvulnerable)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, (mApp.ReanimationTryToGet(mBodyReanimID)?.mDefinition?.mFPS).GetValueOrDefault());
				mState = PlantState.Notready;
				mSubclass = 1;
			}
		}
		else if (mSeedType == SeedType.Agave)
		{
			UpdateAgave();
		}
		else if (mSeedType == SeedType.Endoflame)
		{
			UpdateEndoflame();
		}
		else if (mSeedType == SeedType.Gatlingpea)
		{
			UpdateGatlingpea();
		}
		else if (mSeedType == SeedType.IrisShooter)
		{
			UpdateIrisShooter();
		}
		if (mSubclass == 1)
		{
			UpdateShooter();
		}
		if (mDoSpecialCountdown > 0)
		{
			mDoSpecialCountdown--;
			if (mDoSpecialCountdown <= 0)
			{
				DoSpecial();
			}
		}
	}

	public void Squish()
	{
		if (NotOnGround())
		{
			return;
		}
		if (!IsDisabled())
		{
			if (mSeedType == SeedType.Cherrybomb || mSeedType == SeedType.Jalapeno || mSeedType == SeedType.Doomshroom || mSeedType == SeedType.Iceshroom || mSeedType == SeedType.PickledPepper)
			{
				DoSpecial();
				return;
			}
			if (mSeedType == SeedType.Potatomine && mState != PlantState.Notready)
			{
				DoSpecial();
				return;
			}
			if (mSeedType == SeedType.Tallnut || mSeedType == SeedType.IceWallnut || mSeedType == SeedType.ExplodeONut)
			{
				DoDyingSpecial();
				return;
			}
		}
		if (mSeedType != SeedType.Squash || mState == PlantState.Notready)
		{
			mRenderOrder = Board.MakeRenderOrder(RenderLayer.GraveStone, mRow, 8);
			if (mSeedType == SeedType.Flowerpot)
			{
				mRenderOrder--;
			}
			mSquished = true;
			mDisappearCountdown = 500;
			mApp.PlayFoley(FoleyType.Squish);
			RemoveEffects();
			mBoard.GetLadderAt(mPlantCol, mRow)?.GridItemDie();
			if (mApp.IsIZombieLevel())
			{
				mBoard.mChallenge.IZombiePlantDropRemainingSun(this);
			}
		}
	}

	public void DoRowAreaDamage(int theDamage, uint theDamageFlags)
	{
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		TRect plantAttackRect = GetPlantAttackRect(PlantWeapon.Primary);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead)
			{
				continue;
			}
			int num = zombie.mRow - mRow;
			if (zombie.mZombieType == ZombieType.Boss)
			{
				num = 0;
			}
			if (mSeedType == SeedType.Gloomshroom)
			{
				if (num < -1 || num > 1)
				{
					continue;
				}
			}
			else if (num != 0)
			{
				continue;
			}
			if (zombie.mOnHighGround != IsOnHighGround() || !zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
			if (rectOverlap <= 0)
			{
				continue;
			}
			int theDamage2 = theDamage;
			if ((zombie.mZombieType == ZombieType.Zamboni || zombie.mZombieType == ZombieType.Catapult) && TodCommon.TestBit(theDamageFlags, 5))
			{
				theDamage2 = 1800;
				if (mSeedType == SeedType.Spikerock)
				{
					SpikeRockTakeDamage();
				}
				else if (mSeedType == SeedType.Spikeweed)
				{
					PlayBodyReanim("anim_die", ReanimLoopType.PlayOnce, 0, 30f);
					mState = PlantState.Doingspecial;
					mDisappearCountdown = 295;
					mRenderOrder += 5000;
					PlantVoice.Play(SeedType.Spikeweed, SeedType.None, PlantVoice.VoiceType.Die);
				}
				else
				{
					Die();
				}
			}
			zombie.TakeDamage(theDamage2, theDamageFlags);
			mApp.PlayFoley(FoleyType.Splat);
		}
	}

	public int GetDamageRangeFlags(PlantWeapon thePlantWeapon)
	{
		if (mSeedType == SeedType.Cactus)
		{
			if (thePlantWeapon == PlantWeapon.Secondary)
			{
				return 1;
			}
			return 3;
		}
		if (mSeedType == SeedType.Cherrybomb || mSeedType == SeedType.Jalapeno || mSeedType == SeedType.Cobcannon || mSeedType == SeedType.Doomshroom || mSeedType == SeedType.PickledPepper || (mSeedType == SeedType.Endoflame && thePlantWeapon == PlantWeapon.Secondary))
		{
			return 127;
		}
		if (mSeedType == SeedType.Melonpult || mSeedType == SeedType.Cabbagepult || mSeedType == SeedType.Kernelpult || mSeedType == SeedType.Wintermelon)
		{
			return 13;
		}
		if (mSeedType == SeedType.Potatomine)
		{
			return 77;
		}
		if (mSeedType == SeedType.Squash)
		{
			return 13;
		}
		if (mSeedType == SeedType.Puffshroom || mSeedType == SeedType.Seashroom || mSeedType == SeedType.Fumeshroom || mSeedType == SeedType.Gloomshroom || mSeedType == SeedType.Chomper || mSeedType == SeedType.SuperChomper || mSeedType == SeedType.Agave)
		{
			return 9;
		}
		if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
		{
			return 11;
		}
		if (mSeedType == SeedType.Tanglekelp)
		{
			return 5;
		}
		if (mSeedType == SeedType.GiantWallnut)
		{
			return 17;
		}
		return 1;
	}

	public TRect GetPlantRect()
	{
		TRect tRect = default;
		TRect result;
		if (mSeedType == SeedType.Tallnut)
		{
			result = new TRect(mX + 10, mY, mWidth, mHeight);
		}
		else if (mSeedType == SeedType.Pumpkinshell)
		{
			result = new TRect(mX, mY, mWidth - 20, mHeight);
		}
		else
		{
			result = ((mSeedType != SeedType.Cobcannon) ? new TRect(mX + 10, mY, mWidth - 20, mHeight) : new TRect(mX, mY, 140, 80));
		}
		return result;
	}

	public TRect GetPlantAttackRect(PlantWeapon thePlantWeapon)
	{
		TRect tRect = default;
		if (mApp.IsWallnutBowlingLevel() || mSeedType == SeedType.GiantWallnut)
		{
			tRect = new TRect(mX, mY, mWidth - 20, mHeight);
		}
		else if (thePlantWeapon == PlantWeapon.Secondary && mSeedType == SeedType.Splitpea)
		{
			tRect = new TRect(0, mY, mX + 16, mHeight);
		}
		else
		{
			if (thePlantWeapon != PlantWeapon.Secondary || mSeedType != SeedType.Gatlingpea)
			{
				SeedType seedType = mSeedType;
				if (seedType <= SeedType.Seashroom)
				{
					switch (seedType)
					{
					case SeedType.Potatomine:
						return new TRect(mX, mY, mWidth - 25, mHeight);
					case SeedType.Chomper:
						return new TRect(mX + 80, mY, 40, mHeight);
					case SeedType.Fumeshroom:
						return new TRect(mX + 60, mY, 340, mHeight);
					case SeedType.Squash:
						return new TRect(mX + 20, mY, mWidth - 35, mHeight);
					case SeedType.Tanglekelp:
						return new TRect(mX, mY, mWidth, mHeight);
					case SeedType.Torchwood:
						return new TRect(mX + 50, mY, 30, mHeight);
					case SeedType.Puffshroom:
					case SeedType.Seashroom:
						return new TRect(mX + 60, mY, 230, mHeight);
					case SeedType.Spikeweed:
						break;
					default:
						goto IL_032b;
					}
					goto IL_0304;
				}
				switch (seedType)
				{
				case SeedType.Gloomshroom:
					return new TRect(mX - 80, mY - 80, 240, 240);
				case SeedType.Cattail:
				case SeedType.HypnoCattail:
					return new TRect(-800, -600, 1600, 1200);
				case SeedType.SuperChomper:
					return new TRect(mX + 80, mY, 120, mHeight);
				case SeedType.Agave:
					break;
				default:
					if (seedType == SeedType.Leftpeater)
					{
						return new TRect(0, mY, mX, mHeight);
					}
					goto IL_032b;
				case SeedType.Spikerock:
					goto IL_0304;
				case SeedType.Wintermelon:
				case SeedType.GoldMagnet:
					goto IL_032b;
				}
				if (thePlantWeapon == PlantWeapon.Secondary)
				{
					return new TRect(mX + 60, mY, 340, mHeight);
				}
				return new TRect(mX + 60, mY, 130, mHeight);
			}
			tRect = new TRect(mX + 30, mY, 115, mHeight);
		}
		goto IL_034c;
		IL_0304:
		return new TRect(mX + 20, mY, mWidth - 50, mHeight);
		IL_034c:
		return tRect;
		IL_032b:
		tRect = new TRect(mX + 60, mY, 800, mHeight);
		goto IL_034c;
	}

	public Zombie FindSquashTarget()
	{
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		TRect plantAttackRect = GetPlantAttackRect(PlantWeapon.Primary);
		int num = 0;
		Zombie zombie = null;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie2 = mBoard.mZombies[i];
			if (zombie2.mDead)
			{
				continue;
			}
			int num2 = zombie2.mRow - mRow;
			if (zombie2.mZombieType == ZombieType.Boss)
			{
				num2 = 0;
			}
			if (num2 != 0 || (!zombie2.mHasHead && !zombie2.IsAliveWithoutHead()) || zombie2.IsTangleKelpTarget() || !zombie2.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			TRect zombieRect = zombie2.GetZombieRect();
			if ((zombie2.mZombiePhase != ZombiePhase.PolevaulterPreVault || zombieRect.mX >= mX + 20) && (zombie2.mZombiePhase == ZombiePhase.PolevaulterPreVault || zombie2.mZombiePhase == ZombiePhase.PolevaulterInVault || zombie2.mZombiePhase == ZombiePhase.SnorkelIntoPool || zombie2.mZombiePhase == ZombiePhase.DolphinIntoPool || zombie2.mZombiePhase == ZombiePhase.DolphinRiding || zombie2.mZombiePhase == ZombiePhase.DolphinInJump || zombie2.IsBobsledTeamWithSled()))
			{
				continue;
			}
			int num3 = 70;
			if (zombie2.mIsEating)
			{
				num3 = 110;
			}
			int num4 = -GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
			if (num4 > num3)
			{
				continue;
			}
			int num5 = plantAttackRect.mX;
			if (zombie2.mZombiePhase == ZombiePhase.PolevaulterPostVault || zombie2.mZombiePhase == ZombiePhase.PolevaulterPreVault || zombie2.mZombiePhase == ZombiePhase.DolphinWalkingInPool || zombie2.mZombieType == ZombieType.Imp || zombie2.mZombieType == ZombieType.Football || mApp.IsScaryPotterLevel() || zombie2.mZombieType == ZombieType.FootballPremium)
			{
				num5 = plantAttackRect.mX - 60;
			}
			if (zombie2.IsWalkingBackwards() || zombieRect.mX + zombieRect.mWidth >= num5)
			{
				if (mBoard.ZombieGetID(zombie2) == mTargetZombieID)
				{
					return zombie2;
				}
				if (zombie == null || num4 < num)
				{
					zombie = zombie2;
					num = num4;
				}
			}
		}
		return zombie;
	}

	public void UpdateSquash()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		Reanimation reanimation2 = null;
		Debug.ASSERT(reanimation != null);
		if (mState == PlantState.Notready)
		{
			Zombie zombie = FindSquashTarget();
			if (zombie == null)
			{
				return;
			}
			mTargetZombieID = mBoard.ZombieGetID(zombie);
			mTargetX = (int)zombie.ZombieTargetLeadX(0f) - mWidth / 2;
			mState = PlantState.SquashLook;
			mStateCountdown = 45;
			int theGridX = mBoard.PixelToGridX(mTargetX, 0);
			bool flag = mBoard.IsPoolSquare(theGridX, mRow);
			if (mTargetX < mX)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_lookleft, ReanimLoopType.PlayOnceAndHold, 10, 30f);
				if (!flag)
				{
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Squash_Attack);
				}
			}
			else
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_lookright, ReanimLoopType.PlayOnceAndHold, 10, 30f);
				if (!flag)
				{
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Squash_Attack);
				}
			}
			if (flag)
			{
				PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Squash_Water);
			}
			return;
		}
		if (mState == PlantState.SquashLook)
		{
			if (mStateCountdown <= 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_jumpup, ReanimLoopType.PlayOnceAndHold, 20, 30f);
				mState = PlantState.SquashPreLaunch;
				mStateCountdown = 10;
			}
			return;
		}
		if (mState == PlantState.SquashPreLaunch)
		{
			if (mStateCountdown <= 0)
			{
				Zombie zombie2 = FindSquashTarget();
				if (zombie2 != null)
				{
					mTargetX = (int)zombie2.ZombieTargetLeadX(30f) - mWidth / 2;
				}
				mState = PlantState.SquashRising;
				mStateCountdown = 50;
				mRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, mRow, 0);
			}
			return;
		}
		int theGridX2 = mBoard.PixelToGridXKeepOnBoard(mTargetX, mY);
		int thePositionEnd = mTargetX;
		int num = mBoard.GridToPixelY(theGridX2, mRow) + 8;
		if (mState == PlantState.SquashRising)
		{
			int thePositionStart = mBoard.GridToPixelX(mPlantCol, mStartRow);
			mX = TodCommon.TodAnimateCurve(20, -20, mStateCountdown, thePositionStart, thePositionEnd, TodCurves.EaseInOut);
			if (mStateCountdown <= 0)
			{
				Zombie zombie3 = FindSquashTarget();
				if (zombie3 != null)
				{
					mTargetX = (int)zombie3.ZombieTargetLeadX(30f) - mWidth / 2;
				}
				thePositionEnd = mTargetX;
				int theGridX3 = mBoard.PixelToGridX(thePositionEnd, 0);
				bool flag2 = mBoard.IsPoolSquare(theGridX3, mRow);
				string text = "";
				if (flag2)
				{
					text = ((mX <= thePositionEnd) ? GlobalMembersReanimIds.ReanimTrackId_anim_jumpdown_water : GlobalMembersReanimIds.ReanimTrackId_anim_jumpdown_left_water);
				}
				else
				{
					text = ((mX <= thePositionEnd) ? GlobalMembersReanimIds.ReanimTrackId_anim_jumpdown : GlobalMembersReanimIds.ReanimTrackId_anim_jumpdown_left);
				}
				PlayBodyReanim(text, ReanimLoopType.PlayOnceAndHold, 0, 30f);
				if (flag2)
				{
					reanimation2 = mApp.AddReanimation(thePositionEnd, mY, Board.MakeRenderOrder(RenderLayer.Ground, mRow, 0), ReanimationType.SquashWater);
					reanimation2.PlayReanim(text, ReanimLoopType.PlayOnce, 10, 30f);
				}
				mState = PlantState.SquashFalling;
				mStateCountdown = 80;
			}
		}
		else if (mState == PlantState.SquashFalling)
		{
			int thePositionStart2 = mBoard.GridToPixelX(mPlantCol, mStartRow);
			mX = TodCommon.TodAnimateCurve(100, 60, mStateCountdown, thePositionStart2, thePositionEnd, TodCurves.EaseInOut);
			if (mStateCountdown == 4)
			{
				DoSquashDamage();
			}
			if (mStateCountdown <= 0)
			{
				bool flag3 = mBoard.IsPoolSquare(theGridX2, mRow);
				if (flag3)
				{
					mApp.PlayFoley(FoleyType.Splat);
					mApp.PlaySample(Resources.SOUND_ZOMBIESPLASH);
				}
				else
				{
					mApp.PlayFoley(FoleyType.Thump);
				}
				if ((RandomNumbers.NextNumber(1f) <= 0.1f) & flag3)
				{
					mState = PlantState.SquashCaidanInReady;
					mStateCountdown = 270;
				}
				else
				{
					mState = PlantState.SquashDoneFalling;
					mStateCountdown = 170;
				}
				mBoard.ShakeBoard(1, 4);
				float num2 = 80f;
				if (mBoard.StageHasRoof())
				{
					num2 -= 11f;
				}
				mApp.AddTodParticle(mX + 40, (float)mY + num2, mRenderOrder + 4, ParticleEffect.DustSquash);
			}
		}
		else if (mState == PlantState.SquashCaidanInReady && mStateCountdown <= 0)
		{
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_caidan, ReanimLoopType.PlayOnceAndHold, 10, 30f);
			reanimation2 = mApp.AddReanimation(mX, mY, Board.MakeRenderOrder(RenderLayer.Ground, mRow, 0), ReanimationType.SquashWater);
			reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_caidan, ReanimLoopType.PlayOnce, 10, 30f);
			PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Squash_Caidan);
			mState = PlantState.SquashDoneFalling;
			mStateCountdown = 500;
		}
		else if (mState == PlantState.SquashDoneFalling && mStateCountdown <= 0)
		{
			Die();
		}
	}

	public bool NotOnGround()
	{
		if (!mGloveGrabbed && (mSeedType != SeedType.Gravebuster || mState != PlantState.GravebusterEating) && (mSeedType != SeedType.Squash || (mState != PlantState.SquashRising && mState != PlantState.SquashFalling && mState != PlantState.SquashDoneFalling && mState != PlantState.SquashCaidanInReady)) && !mSquished && mOnBungeeState != PlantOnBungeeState.RisingWithBungee && !mDead && mState != PlantState.JalapenoFwoosh && ((mSeedType != SeedType.Hypnoshroom && mSeedType != SeedType.Spikerock && mSeedType != SeedType.Spikeweed) || mState != PlantState.Doingspecial) && mState != PlantState.Dying)
		{
			return mSeedType == SeedType.GiantWallnut;
		}
		return true;
	}

	public void DoSquashDamage()
	{
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		TRect plantAttackRect = GetPlantAttackRect(PlantWeapon.Primary);
		int num = 0;
		int count = mBoard.mZombies.Count;
		plantAttackRect.mWidth += 20;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead)
			{
				continue;
			}
			int num2 = zombie.mRow - mRow;
			if (zombie.mZombieType == ZombieType.Boss)
			{
				num2 = 0;
			}
			if (num2 == 0 && zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				TRect zombieRect = zombie.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
				int num3 = 0;
				if (zombie.mZombieType == ZombieType.Football)
				{
					num3 = -20;
				}
				if (rectOverlap > num3)
				{
					zombie.TakeDamage(1800, 18u);
					num++;
				}
			}
		}
	}

	public void BurnLine()
	{
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		int count = mBoard.mZombies.Count;
		int num = mX - 25;
		int num2 = mX + 75;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead)
			{
				TRect zombieRect = zombie.GetZombieRect();
				bool flag = false;
				if (zombieRect.mX <= num && zombieRect.mX + zombieRect.mWidth >= num)
				{
					flag = true;
				}
				else if (zombieRect.mX <= num2 && zombieRect.mX + zombieRect.mWidth >= num2)
				{
					flag = true;
				}
				else if (num <= zombieRect.mX && num2 >= zombieRect.mX + zombieRect.mWidth)
				{
					flag = true;
				}
				if (mPlantCol >= 7 && zombie.mZombieType == ZombieType.Boss)
				{
					flag = true;
				}
				if (flag && zombie.EffectedByDamage((uint)damageRangeFlags))
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
			if (theGridItem.mGridX == mPlantCol && theGridItem.mGridItemType == GridItemType.Ladder)
			{
				theGridItem.GridItemDie();
			}
		}
	}

	public void BurnRow(int theRow)
	{
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead)
			{
				int num = zombie.mRow - mRow;
				if (zombie.mZombieType == ZombieType.Boss)
				{
					num = 0;
				}
				if (num == 0 && zombie.EffectedByDamage((uint)damageRangeFlags))
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
		mBoard.GetBossZombie()?.BossDestroyIceballInRow(theRow);
	}

	public void IceZombies()
	{
		int num = 0;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && zombie.HitIceTrap())
			{
				num++;
			}
		}
		mBoard.mIceTrapCounter = 300;
		TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(mBoard.mPoolSparklyParticleID);
		if (todParticleSystem != null)
		{
			todParticleSystem.mDontUpdate = true;
		}
		mBoard.GetBossZombie()?.BossDestroyFireball();
	}

	public void BlowAwayFliers(int theX, int theRow)
	{
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead || zombie.IsDeadOrDying())
			{
				continue;
			}
			zombie.GetZombieRect();
			if (zombie.IsFlying() && zombie.mZombiePhase != ZombiePhase.BalloonPopping)
			{
				if (zombie.mZombieType == ZombieType.Propeller)
				{
					zombie.mPhaseCounter = 150;
					zombie.mZombiePhase = ZombiePhase.PropellerBlownAway;
				}
				else
				{
					zombie.mBlowingAway = true;
				}
			}
		}
		count = mBoard.mGridItems.Count;
		for (int j = 0; j < count; j++)
		{
			GridItem gridItem = mBoard.mGridItems[j];
			if (!gridItem.mDead)
			{
				GridItemType mGridItemType = gridItem.mGridItemType;
				if ((uint)(mGridItemType - 13) <= 1u)
				{
					gridItem.GridItemDie();
					GridItemType mGridItemType2 = gridItem.mGridItemType;
					(ProjectileType, int, int) tuple = ((mGridItemType2 != GridItemType.Talisman) ? (ProjectileType.TalismanMove, -17, -19) : (ProjectileType.Talisman, -20, -18));
					(ProjectileType, int, int) tuple2 = tuple;
					ProjectileType item = tuple2.Item1;
					int item2 = tuple2.Item2;
					int item3 = tuple2.Item3;
					Projectile projectile = mBoard.AddProjectile((int)gridItem.mPosX + item2, (int)gridItem.mPosY + item3, gridItem.mRenderOrder, gridItem.mGridY, item);
					projectile.mMotionType = ProjectileMotion.Straight;
				}
			}
		}
		count = mBoard.mPlants.Count;
		for (int k = 0; k < count; k++)
		{
			Plant plant = mBoard.mPlants[k];
			if (!plant.mDead)
			{
				if (plant.mSealedCountdown > 0)
				{
					plant.SetSealing(theIsSealed: false);
					Projectile projectile2 = mBoard.AddProjectile(plant.mX - 18, plant.mY - 38, plant.mRenderOrder, plant.mRow, ProjectileType.TalismanSeal);
					projectile2.mMotionType = ProjectileMotion.Straight;
				}
				if (plant.mInTalismanCounter > 0)
				{
					plant.mInTalismanCounter = 0;
				}
			}
		}
		mBoard.mSealedCountdown = 0;
		mApp.PlaySample(Resources.SOUND_BLOVER);
		mBoard.mFogBlownCountDown = 4000;
	}

	public void UpdateGraveBuster()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mState == PlantState.GravebusterLanding)
		{
			float theEventTime = 0.46f;
			if (reanimation.ShouldTriggerTimedEvent(theEventTime))
			{
				GridItem graveStoneAt = mBoard.GetGraveStoneAt(mPlantCol, mRow);
				if (graveStoneAt != null)
				{
					graveStoneAt.GridItemDie();
					mBoard.mGravesCleared++;
				}
				mApp.AddTodParticle(mX + 40, mY + 90, mRenderOrder + 4, ParticleEffect.GraveBusterDie);
				if (reanimation.IsAnimPlaying("anim_land"))
				{
					mBoard.AddCoinForDropLootType(mX + 40, mY + 40, mGravebusterDrop);
				}
				mStateCountdown = 0;
				mState = PlantState.GravebusterEating;
			}
		}
		else if (mState == PlantState.GravebusterEating && mStateCountdown <= 0)
		{
			if (reanimation.IsAnimPlaying("anim_land_treasure") && reanimation.ShouldTriggerTimedEvent(0.72f))
			{
				mBoard.AddCoinForDropLootType(mX + 40, mY + 40, mGravebusterDrop);
			}
			if (reanimation.mLoopCount > 0)
			{
				Die();
			}
		}
	}

	public TodParticleSystem AddAttachedParticle(int thePosX, int thePosY, int theRenderPostition, ParticleEffect theEffect)
	{
		mApp.ParticleTryToGet(mParticleID)?.ParticleSystemDie();
		TodParticleSystem todParticleSystem = mApp.AddTodParticle(thePosX, thePosY, theRenderPostition, theEffect);
		if (todParticleSystem != null)
		{
			mParticleID = mApp.ParticleGetID(todParticleSystem);
		}
		return todParticleSystem;
	}

	public void GetPeaHeadOffset(ref int theOffsetX, ref int theOffsetY)
	{
		if (mBodyReanimID.mActive)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			int theTrackIndex = 0;
			if (reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_stem))
			{
				theTrackIndex = reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_anim_stem);
			}
			else if (reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
			{
				theTrackIndex = reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
			}
			if (reanimation.mIsSpine)
			{
				theOffsetX = 24;
				theOffsetY = 24;
				return;
			}
			reanimation.GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
			theOffsetX = (int)(aTransformCurrent.mTransX * Constants.IS);
			theOffsetY = (int)(aTransformCurrent.mTransY * Constants.IS);
			aTransformCurrent.PrepareForReuse();
		}
	}

	public bool MakesSun()
	{
		if (mSeedType != SeedType.Sunflower && mSeedType != SeedType.Twinsunflower && mSeedType != SeedType.Sunshroom && mSeedType != SeedType.SunflowerPea)
		{
			return mSeedType == SeedType.SunflowerWallnut;
		}
		return true;
	}

	public static void DrawSeedType(Graphics g, SeedType theSeedType, SeedType theImitaterType, DrawVariation theDrawVariation, float thePosX, float thePosY)
	{
		SeedType seedType = theSeedType;
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			seedType = theImitaterType;
		}
		if (seedType == SeedType.Cattail && GlobalStaticVars.gLawnApp.IsCattailSkinEnabled())
		{
			seedType = SeedType.HypnoCattail;
		}
		if (Challenge.IsZombieSeedType(seedType))
		{
			ZombieType theZombieType = Challenge.IZombieSeedTypeToZombieType(seedType);
			if ((seedType & SeedType.MindControlledCardFlag) != 0)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(GameConstants.ZOMBIE_MINDCONTROLLED_COLOR);
			}
			GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedZombieNew(g, thePosX, thePosY, theZombieType);
			if ((seedType & SeedType.MindControlledCardFlag) != 0)
			{
				g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
				GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedZombieNew(g, thePosX, thePosY, theZombieType);
				g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
		else if (seedType == SeedType.Sprout)
		{
			Image iMAGE_CACHED_MARIGOLD = AtlasResources.IMAGE_CACHED_MARIGOLD;
			int num = (int)(thePosX - (float)(iMAGE_CACHED_MARIGOLD.mWidth / 2) * g.mScaleX);
			int num2 = (int)(thePosY - (float)iMAGE_CACHED_MARIGOLD.mHeight * g.mScaleX);
			TodCommon.TodDrawImageScaledF(g, iMAGE_CACHED_MARIGOLD, num, num2, g.mScaleX, g.mScaleY);
		}
		else if (seedType == SeedType.IrisShooter && IrisArt.Ready)
		{
			float irisOx = thePosX - 40f * Constants.S * g.mScaleX;
			float irisOy = thePosY - (8f + 56f * Constants.S) * g.mScaleY;
			IrisArt.DrawCharacter(g, IrisArt.Idle, 0, irisOx, irisOy, g.mScaleX);
		}
		else
		{
			GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedPlant(g, thePosX - 40f * Constants.S * g.mScaleX, thePosY - (8f + 56f * Constants.S) * g.mScaleY, seedType, theDrawVariation);
		}
	}

	public void KillAllPlantsNearDoom()
	{
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mRow == mRow && plant.mPlantCol - mPlantCol == 0)
			{
				plant.Die();
			}
		}
	}

	public bool IsOnHighGround()
	{
		if (mBoard != null)
		{
			return mBoard.mGridSquareType[mPlantCol, mRow] == GridSquareType.HighGround;
		}
		return false;
	}

	public void UpdateTorchwood()
	{
		TRect plantAttackRect = GetPlantAttackRect(PlantWeapon.Primary);
		int index = -1;
		Projectile theProjectile = null;
		while (mBoard.IterateProjectiles(ref theProjectile, ref index))
		{
			if (theProjectile.mRow != mRow || (theProjectile.mProjectileType != ProjectileType.Pea && theProjectile.mProjectileType != ProjectileType.Snowpea && theProjectile.mProjectileType != ProjectileType.EndoflameSpike))
			{
				continue;
			}
			TRect projectileRect = theProjectile.GetProjectileRect();
			int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, projectileRect);
			if (rectOverlap >= 1)
			{
				if (theProjectile.mProjectileType == ProjectileType.Pea)
				{
					theProjectile.ConvertToFireball(mPlantCol);
				}
				else if (theProjectile.mProjectileType == ProjectileType.EndoflameSpike)
				{
					theProjectile.ConvertToEndoflameFireball(mPlantCol);
				}
				else if (theProjectile.mProjectileType == ProjectileType.Snowpea)
				{
					theProjectile.ConvertToPea(mPlantCol);
				}
			}
		}
	}

	public void LaunchStarFruit()
	{
		if (FindStarFruitTarget())
		{
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shoot, ReanimLoopType.PlayOnceAndHold, 20, 28f);
			mShootingCounter = 40;
		}
	}

	public bool FindStarFruitTarget()
	{
		if (mRecentlyEatenCountdown > 0)
		{
			return true;
		}
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		int num = mX + 40;
		int num2 = mY + 40;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead)
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			if (!zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			if (zombie.mZombieType == ZombieType.Boss && mPlantCol >= 5)
			{
				return true;
			}
			if (zombie.mRow == mRow)
			{
				if (zombieRect.mX + zombieRect.mWidth < num)
				{
					return true;
				}
				continue;
			}
			if (zombie.mZombieType == ZombieType.Digger)
			{
				zombieRect.mWidth += 10;
			}
			float theTime = TodCommon.Distance2D(num, num2, zombieRect.mX + zombieRect.mWidth / 2, zombieRect.mY + zombieRect.mHeight / 2) / 3.33f;
			int num3 = (int)(zombie.ZombieTargetLeadX(theTime) - (float)(zombieRect.mWidth / 2));
			if (num3 + zombieRect.mWidth > num && num3 < num)
			{
				return true;
			}
			int num4 = num3 + zombieRect.mWidth / 2;
			int num5 = zombieRect.mY + zombieRect.mHeight / 2;
			float num6 = TodCommon.RadToDeg((float)Math.Atan2((float)(num5 - num2), (float)(num4 - num)));
			if (Math.Abs(zombie.mRow - mRow) < 2)
			{
				if (num6 > 20f && num6 < 40f)
				{
					return true;
				}
				if (num6 < -25f && num6 > -45f)
				{
					return true;
				}
			}
			else
			{
				if (num6 > 25f && num6 < 35f)
				{
					return true;
				}
				if (num6 < -28f && num6 > -38f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UpdateChomper()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (mState == PlantState.Ready)
		{
			Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
			if (zombie != null)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_bite, ReanimLoopType.PlayOnceAndHold, 20, 30f);
				mState = PlantState.ChomperBiting;
				mStateCountdown = 70;
			}
		}
		else if (mState == PlantState.ChomperBiting)
		{
			if (mStateCountdown > 0)
			{
				return;
			}
			mApp.PlayFoley(FoleyType.Bigchomp);
			Zombie zombie2 = FindTargetZombie(mRow, PlantWeapon.Primary);
			bool flag = false;
			if (zombie2 != null && (zombie2.mZombieType == ZombieType.Gargantuar || zombie2.mZombieType == ZombieType.RedeyeGargantuar || zombie2.mZombieType == ZombieType.Boss || zombie2.mZombieType == ZombieType.RobotTitan || zombie2.mZombieType == ZombieType.RedeyeRobotTitan))
			{
				flag = true;
			}
			bool flag2 = false;
			if (zombie2 == null)
			{
				flag2 = true;
			}
			else if (!zombie2.IsImmobilizied())
			{
				if (zombie2.IsBouncingPogo())
				{
					flag2 = true;
				}
				else if (zombie2.mZombiePhase == ZombiePhase.PolevaulterInVault || zombie2.mZombiePhase == ZombiePhase.PolevaulterPreVault)
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				mApp.PlayFoley(FoleyType.Splat);
				zombie2.TakeDamage(40, 0u);
				mState = PlantState.ChomperBitingMissed;
			}
			else if (flag2)
			{
				mState = PlantState.ChomperBitingMissed;
			}
			else
			{
				zombie2.DieWithLoot();
				mState = PlantState.ChomperBitingGotOne;
			}
		}
		else if (mState == PlantState.ChomperBitingGotOne)
		{
			if (reanimation.mLoopCount > 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_chew, ReanimLoopType.Loop, 0, 30f);
				if (mApp.IsIZombieLevel())
				{
					reanimation.mAnimRate = 0f;
				}
				mState = PlantState.ChomperDigesting;
				mStateCountdown = 4000;
			}
		}
		else if (mState == PlantState.ChomperDigesting)
		{
			if (mStateCountdown <= 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_swallow, ReanimLoopType.PlayOnceAndHold, 20, 30f);
				mState = PlantState.ChomperSwallowing;
			}
		}
		else if ((mState == PlantState.ChomperSwallowing || mState == PlantState.ChomperBitingMissed) && reanimation.mLoopCount > 0)
		{
			PlayIdleAnim(reanimation.mDefinition.mFPS);
			mState = PlantState.Ready;
		}
	}

	public void AnimateSleeping()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			if (reanimation.IsTrackShowing("anim_sleep_to_idle") && reanimation.mLoopCount > 0)
			{
				mState = PlantState.Notready;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 30f);
			}
			else if (reanimation.IsTrackShowing("anim_idle_to_sleep") && reanimation.mLoopCount > 0)
			{
				mIsAsleep = false;
				SetSleeping(theIsAsleep: true);
			}
			else if (mState == PlantState.Entering && reanimation.IsTrackShowing("anim_enter") && reanimation.mLoopCount > 0 && (!IsNocturnal(mSeedType) || mBoard == null || mBoard.StageIsNight()))
			{
				mState = PlantState.Notready;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 30f);
			}
		}
	}

	public static bool DoAdvancedSleeping(SeedType seedType)
	{
		if (seedType != SeedType.Puffshroom && seedType != SeedType.Fumeshroom && seedType != SeedType.Iceshroom)
		{
			return seedType == SeedType.Seashroom;
		}
		return true;
	}

	public void AnimateHypnoShroom()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			if (reanimation.IsTrackShowing("anim_sleep_to_idle") && reanimation.mAnimTime > 0.9f)
			{
				mState = PlantState.Notready;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 30f);
			}
			else if (reanimation.IsTrackShowing("anim_idle_to_sleep") && reanimation.mAnimTime > 0.9f)
			{
				mIsAsleep = false;
				SetSleeping(theIsAsleep: true);
			}
			else if (mState == PlantState.Entering && reanimation.IsTrackShowing("anim_enter") && reanimation.mAnimTime > 0.9f && (!IsNocturnal(mSeedType) || mBoard == null || mBoard.StageIsNight()))
			{
				mState = PlantState.Notready;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 20, 30f);
			}
		}
	}

	public void UpdateSuperChomper()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (mState == PlantState.Ready)
		{
			Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
			if (zombie != null)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_bite, ReanimLoopType.PlayOnceAndHold, 20, 30f);
				mState = PlantState.ChomperBiting;
				mStateCountdown = 70;
			}
		}
		else if (mState == PlantState.ChomperBiting)
		{
			if (mStateCountdown > 0)
			{
				return;
			}
			mApp.PlayFoley(FoleyType.Bigchomp);
			int num = 0;
			int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
			TRect plantAttackRect = GetPlantAttackRect(PlantWeapon.Primary);
			for (int i = 0; i < mBoard.mZombies.Count; i++)
			{
				Zombie zombie2 = mBoard.mZombies[i];
				if (zombie2.mDead || (!zombie2.IsImmobilizied() && (zombie2.IsBouncingPogo() || zombie2.mZombiePhase == ZombiePhase.PolevaulterInVault || zombie2.mZombiePhase == ZombiePhase.PolevaulterPreVault)))
				{
					continue;
				}
				int num2 = zombie2.mRow - mRow;
				if (zombie2.mZombieType == ZombieType.Boss)
				{
					num2 = 0;
				}
				if (!zombie2.mHasHead || zombie2.IsTangleKelpTarget() || num2 != 0 || !zombie2.EffectedByDamage((uint)damageRangeFlags))
				{
					continue;
				}
				int num3 = 0;
				if (zombie2.mZombiePhase == ZombiePhase.DiggerWalking)
				{
					plantAttackRect.mX += 20;
					plantAttackRect.mWidth -= 20;
				}
				if (zombie2.mZombiePhase == ZombiePhase.PogoBouncing || (zombie2.mZombieType == ZombieType.Bungee && zombie2.mTargetCol == mPlantCol))
				{
					continue;
				}
				if (zombie2.mIsEating || mState == PlantState.ChomperBiting)
				{
					num3 = 60;
				}
				TRect zombieRect = zombie2.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
				if (rectOverlap >= -num3)
				{
					if (zombie2.mZombieType == ZombieType.Boss)
					{
						zombie2.TakeDamage(40, 0u);
						continue;
					}
					zombie2.DieWithLoot();
					num++;
				}
			}
			if (num > 0)
			{
				mState = PlantState.ChomperBitingGotOne;
			}
			else
			{
				mState = PlantState.ChomperBitingMissed;
			}
		}
		else if (mState == PlantState.ChomperBitingGotOne)
		{
			if (reanimation.mLoopCount > 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_chew, ReanimLoopType.Loop, 0, 15f);
				if (mApp.IsIZombieLevel())
				{
					reanimation.mAnimRate = 0f;
				}
				mState = PlantState.ChomperDigesting;
				mStateCountdown = 2500;
			}
		}
		else if (mState == PlantState.ChomperDigesting)
		{
			if (mStateCountdown <= 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_swallow, ReanimLoopType.PlayOnceAndHold, 20, 30f);
				mState = PlantState.ChomperSwallowing;
			}
		}
		else if ((mState == PlantState.ChomperSwallowing || mState == PlantState.ChomperBitingMissed) && reanimation.mLoopCount > 0)
		{
			PlayIdleAnim(reanimation.mDefinition.mFPS);
			mState = PlantState.Ready;
		}
	}

	public void DoBlink()
	{
		mBlinkCountdown = 400 + RandomNumbers.NextNumber(400);
		if (NotOnGround() || mShootingCounter != 0 || (mSeedType == SeedType.Potatomine && mState != PlantState.PotatoArmed) || mState == PlantState.CactusRising || mState == PlantState.CactusHigh || mState == PlantState.CactusLowering || mState == PlantState.MagnetshroomSucking || mState == PlantState.MagnetshroomCharging)
		{
			return;
		}
		EndBlink();
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null && !reanimation.mDead && (mSeedType != SeedType.Tallnut || reanimation.GetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_idle) != AtlasResources.IMAGE_REANIM_TALLNUT_CRACKED2) && mSeedType != SeedType.Garlic)
		{
			if (mSeedType == SeedType.Wallnut || mSeedType == SeedType.Tallnut || mSeedType == SeedType.ExplodeONut || mSeedType == SeedType.GiantWallnut)
			{
				mBlinkCountdown = 1000 + RandomNumbers.NextNumber(1000);
			}
			Reanimation reanimation2 = AttachBlinkAnim(reanimation);
			if (reanimation2 != null)
			{
				mBlinkReanimID = mApp.ReanimationGetID(reanimation2);
			}
			reanimation.AssignRenderGroupToPrefix("anim_eye", -1);
		}
	}

	public void UpdateBlink()
	{
		if (mBlinkReanimID != null)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBlinkReanimID);
			if (reanimation == null || reanimation.mLoopCount > 0)
			{
				EndBlink();
			}
		}
		if (!IsDisabled() && mBlinkCountdown > 0)
		{
			mBlinkCountdown--;
			if (mBlinkCountdown == 0)
			{
				DoBlink();
			}
		}
	}

	public void PlayBodyReanim(string theTrackName, ReanimLoopType theLoopType, byte theBlendTime, float theAnimRate)
	{
		lastPlayedBodyReanim_Name = theTrackName;
		lastPlayedBodyReanim_Type = theLoopType;
		lastPlayedBodyReanim_BlendTime = theBlendTime;
		lastPlayedBodyReanim_AnimRate = theAnimRate;
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (theBlendTime > 0)
		{
			reanimation.StartBlend(theBlendTime);
		}
		if (theAnimRate > 0f)
		{
			reanimation.mAnimRate = theAnimRate;
		}
		reanimation.mLoopType = theLoopType;
		reanimation.mLoopCount = 0;
		reanimation.SetFramesForLayer(theTrackName);
	}

	public void UpdateMagnetShroom()
	{
		for (int i = 0; i < 5; i++)
		{
			MagnetItem magnetItem = mMagnetItems[i];
			if (magnetItem.mItemType != MagnetItemType.None)
			{
				SexyVector2 sexyVector = new SexyVector2((float)mX + magnetItem.mDestOffsetX - magnetItem.mPosX, (float)mY + magnetItem.mDestOffsetY - magnetItem.mPosY);
				float num = sexyVector.Magnitude();
				if (num >= 20f)
				{
					magnetItem.mPosX += sexyVector.x * 0.05f;
					magnetItem.mPosY += sexyVector.y * 0.05f;
				}
			}
		}
		if (mState == PlantState.MagnetshroomCharging)
		{
			if (mStateCountdown <= 0)
			{
				mState = PlantState.Ready;
				float theAnimRate = TodCommon.RandRangeFloat(10f, 15f);
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 30, theAnimRate);
				if (mApp.IsIZombieLevel())
				{
					Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
					reanimation.mAnimRate = 0f;
				}
				MagnetItem magnetItem2 = mMagnetItems[0];
				magnetItem2.mItemType = MagnetItemType.None;
			}
			return;
		}
		if (mState == PlantState.MagnetshroomSucking)
		{
			Reanimation reanimation2 = mApp.ReanimationTryToGet(mBodyReanimID);
			if (reanimation2.mLoopCount > 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_nonactive_idle2, ReanimLoopType.Loop, 20, 2f);
				if (mApp.IsIZombieLevel())
				{
					reanimation2 = mApp.ReanimationTryToGet(mBodyReanimID);
					reanimation2.mAnimRate = 0f;
				}
				mState = PlantState.MagnetshroomCharging;
			}
			return;
		}
		Zombie zombie = null;
		float num2 = 0f;
		int count = mBoard.mZombies.Count;
		for (int j = 0; j < count; j++)
		{
			Zombie zombie2 = mBoard.mZombies[j];
			if (zombie2.mDead)
			{
				continue;
			}
			TRect zombieRect = zombie2.GetZombieRect();
			int num3 = zombie2.mRow - mRow;
			if (zombie2.mMindControlled || !zombie2.mHasHead || zombie2.mZombieHeight != ZombieHeight.ZombieNormal || zombie2.mZombiePhase == ZombiePhase.RisingFromGrave || zombie2.IsDeadOrDying() || zombieRect.mX > Constants.WIDE_BOARD_WIDTH || num3 > 2 || num3 < -2)
			{
				continue;
			}
			if (zombie2.mZombiePhase == ZombiePhase.DiggerTunneling || zombie2.mZombiePhase == ZombiePhase.DiggerStunned || zombie2.mZombiePhase == ZombiePhase.DiggerWalking || zombie2.mZombieType == ZombieType.Pogo)
			{
				if (!zombie2.mHasObject)
				{
					continue;
				}
			}
			else if (zombie2.mHelmType != HelmType.Pail && zombie2.mHelmType != HelmType.Football && zombie2.mShieldType != ShieldType.Door && zombie2.mShieldType != ShieldType.Ladder && zombie2.mZombiePhase != ZombiePhase.JackInTheBoxRunning && zombie2.mHelmType != HelmType.FootballPremium && zombie2.mHelmType != HelmType.RobotTitanHead && zombie2.mHelmType != HelmType.RedeyeRobotTitanHead)
			{
				continue;
			}
			int theRadius = 270;
			if (zombie2.mIsEating)
			{
				theRadius = 320;
			}
			if (GameConstants.GetCircleRectOverlap(mX, mY + 20, theRadius, zombieRect))
			{
				float num4 = TodCommon.Distance2D(mX, mY, zombieRect.mX, zombieRect.mY);
				num4 += (float)Math.Abs(num3) * 80f;
				if (zombie == null || num4 < num2)
				{
					zombie = zombie2;
					num2 = num4;
				}
			}
		}
		if (zombie != null)
		{
			MagnetShroomAttactItem(zombie);
			return;
		}
		GridItem gridItem = null;
		float num5 = 0f;
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType != GridItemType.Ladder)
			{
				continue;
			}
			int value = theGridItem.mGridX - mPlantCol;
			int value2 = theGridItem.mGridY - mRow;
			int num6 = Math.Max(Math.Abs(value), Math.Abs(value2));
			if (num6 <= 2)
			{
				float num7 = num6;
				num7 += (float)Math.Abs(value2) * 0.05f;
				if (gridItem == null || num7 < num5)
				{
					gridItem = theGridItem;
					num5 = num7;
				}
			}
		}
		if (gridItem != null)
		{
			mState = PlantState.MagnetshroomSucking;
			mStateCountdown = 1500;
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 12f);
			mApp.PlayFoley(FoleyType.Magnetshroom);
			gridItem.GridItemDie();
			MagnetItem freeMagnetItem = GetFreeMagnetItem();
			freeMagnetItem.mPosX = mBoard.GridToPixelX(gridItem.mGridX, gridItem.mGridY) + 40;
			freeMagnetItem.mPosY = mBoard.GridToPixelY(gridItem.mGridX, gridItem.mGridY);
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 10f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f);
			freeMagnetItem.mItemType = MagnetItemType.LadderPlaced;
		}
	}

	public MagnetItem GetFreeMagnetItem()
	{
		if (mSeedType == SeedType.GoldMagnet)
		{
			for (int i = 0; i < 5; i++)
			{
				if (mMagnetItems[i].mItemType == MagnetItemType.None)
				{
					return mMagnetItems[i];
				}
			}
			return null;
		}
		return mMagnetItems[0];
	}

	public void DrawMagnetItems(Graphics g)
	{
		float num = 0f;
		float num2 = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
		for (int i = 0; i < 5; i++)
		{
			MagnetItem magnetItem = mMagnetItems[i];
			if (magnetItem.mItemType != MagnetItemType.None)
			{
				int theCelCol = 0;
				int theCelRow = 0;
				Image theImageStrip = null;
				float num3 = 1f;
				if (magnetItem.mItemType == MagnetItemType.Pail1)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET1;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.Pail2)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET2;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.Pail3)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET3;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.FootballHelmet1)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_HELMET;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.FootballHelmet2)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_HELMET2;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.FootballHelmet3)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_FOOTBALL_HELMET3;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.FootballPremiumHelmet1)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_HELMET;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.FootballPremiumHelmet2)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_HELMET2;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.FootballPremiumHelmet3)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_FOOTBALLPREMIUM_ZOMBIE_FOOTBALL_HELMET3;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.Door1)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR1;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.Door2)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR2;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.Door3)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR3;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType >= MagnetItemType.Pogo1 && magnetItem.mItemType <= MagnetItemType.Pogo3)
				{
					theCelCol = (int)(magnetItem.mItemType - 10);
					theImageStrip = AtlasResources.IMAGE_ZOMBIEPOGO;
					num3 = 0.8f;
				}
				else if (magnetItem.mItemType == MagnetItemType.Ladder1)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_1;
				}
				else if (magnetItem.mItemType == MagnetItemType.Ladder2)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_1_DAMAGE1;
				}
				else if (magnetItem.mItemType == MagnetItemType.Ladder3)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_1_DAMAGE2;
				}
				else if (magnetItem.mItemType == MagnetItemType.LadderPlaced)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_5;
				}
				else if (magnetItem.mItemType == MagnetItemType.JackInTheBox)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_JACKBOX_BOX;
				}
				else if (magnetItem.mItemType == MagnetItemType.PickAxe)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_DIGGER_PICKAXE;
				}
				else if (magnetItem.mItemType == MagnetItemType.SilverCoin)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_COIN_SILVER_DOLLAR;
				}
				else if (magnetItem.mItemType == MagnetItemType.GoldCoin)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_COIN_GOLD_DOLLAR;
				}
				else if (magnetItem.mItemType == MagnetItemType.Diamond)
				{
					theImageStrip = AtlasResources.IMAGE_REANIM_DIAMOND;
				}
				else if (magnetItem.mItemType == MagnetItemType.RobotTitanHead1)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD1;
				}
				else if (magnetItem.mItemType == MagnetItemType.RobotTitanHead2)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD2;
				}
				else if (magnetItem.mItemType == MagnetItemType.RedeyeRobotTitanHead1)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_REDWHOLEHEAD1;
				}
				else if (magnetItem.mItemType == MagnetItemType.RedeyeRobotTitanHead2)
				{
					num3 = 0.8f;
					theImageStrip = AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_REDWHOLEHEAD2;
				}
				else
				{
					Debug.ASSERT(value: false);
				}
				if (num3 == 1f)
				{
					g.DrawImageCel(theImageStrip, (int)((magnetItem.mPosX - (float)mX + num) * Constants.S), (int)((magnetItem.mPosY - (float)mY + num2) * Constants.S), theCelCol, theCelRow);
				}
				else
				{
					TodCommon.TodDrawImageCelScaledF(g, theImageStrip, (magnetItem.mPosX - (float)mX + num) * Constants.S, (magnetItem.mPosY - (float)mY + num2) * Constants.S, theCelCol, 0, num3, num3);
				}
			}
		}
	}

	public void UpdateDoomShroom()
	{
		if (!IsDisabled() && mState != PlantState.Doingspecial)
		{
			mState = PlantState.Doingspecial;
			mDoSpecialCountdown = 100;
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			Debug.ASSERT(reanimation != null);
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_explode);
			reanimation.mAnimRate = 23f;
			reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation.SetShakeOverride(GlobalMembersReanimIds.ReanimTrackId_doomshroom_head1, 1f);
			reanimation.SetShakeOverride(GlobalMembersReanimIds.ReanimTrackId_doomshroom_head2, 2f);
			reanimation.SetShakeOverride(GlobalMembersReanimIds.ReanimTrackId_doomshroom_head3, 2f);
			mApp.PlayFoley(FoleyType.ReverseExplosion);
		}
	}

	public void UpdateIceShroom()
	{
		if (IsDisabled() || mState == PlantState.Doingspecial || mState == PlantState.IceshroomCornRouletteWaiting)
		{
			return;
		}
		if (mState == PlantState.Dying)
		{
			if (mDisappearCountdown == 230)
			{
				PlayBodyReanim("anim_explode", ReanimLoopType.PlayOnce, 20, 0f);
			}
		}
		else
		{
			mState = PlantState.Doingspecial;
			PlayBodyReanim("anim_shooting", ReanimLoopType.Loop, 20, 30f);
			mDoSpecialCountdown = 100;
		}
	}

	public void UpdatePlantern()
	{
		if (mState == PlantState.PlanternHenshinBegin && mStateCountdown <= 0)
		{
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle2, ReanimLoopType.Loop, 20, 30f);
			mState = PlantState.PlanternHenshinOver;
		}
	}

	public void UpdatePotato()
	{
		Reanimation theAttachReanim = mApp.ReanimationGet(mBodyReanimID);
		if (mState == PlantState.Notready)
		{
			if (mStateCountdown <= 0)
			{
				int num = mX + mWidth / 2;
				int num2 = mY + mHeight / 2;
				mApp.AddTodParticle(num, num2, mRenderOrder, ParticleEffect.PotatoMineRise);
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_rise, ReanimLoopType.PlayOnceAndHold, 20, 18f);
				mState = PlantState.PotatoRising;
				mApp.PlayFoley(FoleyType.DirtRise);
			}
		}
		else if (mState == PlantState.PotatoRising)
		{
			if (theAttachReanim.mLoopCount > 0)
			{
				float num3 = TodCommon.RandRangeFloat(12f, 15f);
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_armed, ReanimLoopType.Loop, 0, num3);
				PlantDefinition plantDefinition = GetPlantDefinition(mSeedType);
				Reanimation reanimation = mApp.AddReanimation(0f, 0f, mRenderOrder + 2, plantDefinition.mReanimationType);
				reanimation.mLoopType = ReanimLoopType.Loop;
				reanimation.mAnimRate = num3 - 2f;
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_glow);
				reanimation.mFrameCount = 10;
				reanimation.ShowOnlyTrack(GlobalMembersReanimIds.ReanimTrackId_anim_glow);
				reanimation.SetTruncateDisappearingFrames(GlobalMembersReanimIds.ReanimTrackId_anim_glow, theTruncateDisappearingFrames: false);
				mLightReanimID = mApp.ReanimationGetID(reanimation);
				reanimation.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_light);
				mState = PlantState.PotatoArmed;
				mBlinkCountdown = 400 + RandomNumbers.NextNumber(400);
			}
		}
		else
		{
			if (mState != PlantState.PotatoArmed)
			{
				return;
			}
			Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
			if (zombie != null)
			{
				DoSpecial();
				return;
			}
			Reanimation reanimation2 = mApp.ReanimationTryToGet(mLightReanimID);
			if (reanimation2 != null)
			{
				int theTimeAge = DistanceToClosestZombie();
				reanimation2.mFrameCount = (short)TodCommon.TodAnimateCurve(200, 50, theTimeAge, 10, 3, TodCurves.Linear);
			}
		}
	}

	public int CalcRenderOrder()
	{
		PlantOrder plantOrder = PlantOrder.Normal;
		RenderLayer theRenderLayer = RenderLayer.Plant;
		int num = 0;
		SeedType seedType = mSeedType;
		if (mSeedType == SeedType.Imitater && mImitaterType != SeedType.None)
		{
			seedType = mImitaterType;
		}
		if (mApp.IsWallnutBowlingLevel())
		{
			theRenderLayer = RenderLayer.Projectile;
		}
		else if (seedType == SeedType.Pumpkinshell)
		{
			plantOrder = PlantOrder.Pumpkin;
		}
		else if (IsFlying(seedType))
		{
			plantOrder = PlantOrder.Flyer;
		}
		else
		{
			switch (seedType)
			{
			case SeedType.Flowerpot:
				plantOrder = PlantOrder.Lilypad;
				break;
			case SeedType.Lilypad:
				if (mApp.mGameMode != GameMode.ChallengeZenGarden)
				{
					plantOrder = PlantOrder.Lilypad;
				}
				break;
			}
		}
		if (seedType == SeedType.Cobcannon)
		{
			num = 0;
		}
		if (seedType == SeedType.Gravebuster)
		{
			num = 550;
		}
		if (seedType == SeedType.Pumpkinshell)
		{
			num = 200;
		}
		return Board.MakeRenderOrder(theRenderLayer, mRow, (int)plantOrder * 5 - mX + num);
	}

	public void AnimateNuts()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (mSeedType == SeedType.SunflowerWallnut)
		{
			reanimation = mApp.ReanimationTryToGet(mHeadReanimID);
		}
		if (reanimation == null)
		{
			return;
		}
		Image image;
		Image image2;
		string theTrackName;
		if (mSeedType == SeedType.Wallnut || mSeedType == SeedType.SunflowerWallnut || mSeedType == SeedType.PeaWallnut || mSeedType == SeedType.ExplodeONut || mSeedType == SeedType.IceWallnut)
		{
			image = AtlasResources.IMAGE_REANIM_WALLNUT_CRACKED1;
			image2 = AtlasResources.IMAGE_REANIM_WALLNUT_CRACKED2;
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face;
		}
		else
		{
			if (mSeedType != SeedType.Tallnut)
			{
				return;
			}
			image = AtlasResources.IMAGE_REANIM_TALLNUT_CRACKED1;
			image2 = AtlasResources.IMAGE_REANIM_TALLNUT_CRACKED2;
			theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_idle + GetStateTrackIdSuffix();
		}
		int num = mX + 40;
		int num2 = mY + 10;
		if (mSeedType == SeedType.Tallnut)
		{
			num2 -= 32;
		}
		Image imageOverride = reanimation.GetImageOverride(theTrackName);
		if (mSeedType == SeedType.Tallnut)
		{
			if (IsInPlay() && mState != PlantState.Dying && mState != PlantState.Entering && mState != PlantState.AfterEntering && !reanimation.IsAnimPlaying(theTrackName))
			{
				if (mPlantHealth < mPlantMaxHealth * 2 / 3)
				{
					mApp.AddTodParticle(num, num2, mRenderOrder + 4, ParticleEffect.WallnutNewEatLarge);
				}
				PlayIdleAnim(30f);
			}
			return;
		}
		if (mPlantHealth < mPlantMaxHealth / 3)
		{
			if (imageOverride != image2)
			{
				reanimation.SetImageOverride(theTrackName, image2);
				mApp.AddTodParticle(num, num2, mRenderOrder + 4, ParticleEffect.WallnutEatLarge);
			}
		}
		else if (mPlantHealth < mPlantMaxHealth * 2 / 3)
		{
			if (imageOverride != image)
			{
				reanimation.SetImageOverride(theTrackName, image);
				mApp.AddTodParticle(num, num2, mRenderOrder + 4, ParticleEffect.WallnutEatLarge);
			}
		}
		else
		{
			Image theImage = null;
			reanimation.SetImageOverride(theTrackName, theImage);
		}
		if (IsInPlay() && !mApp.IsIZombieLevel())
		{
			if (mRecentlyEatenCountdown > 0)
			{
				reanimation.mAnimRate = 0.1f;
			}
			else if (reanimation.mAnimRate < 1f && mOnBungeeState != PlantOnBungeeState.RisingWithBungee)
			{
				reanimation.mAnimRate = TodCommon.RandRangeFloat(10f, 15f);
			}
		}
	}

	public void SetSleeping(bool theIsAsleep)
	{
		if (mIsAsleep == theIsAsleep || NotOnGround())
		{
			return;
		}
		mIsAsleep = theIsAsleep;
		if (theIsAsleep && HasSleepingReanim())
		{
			float[] sleepingReanimOffset = GetSleepingReanimOffset();
			float theX = (float)mX + sleepingReanimOffset[0];
			float theY = (float)mY + sleepingReanimOffset[1];
			Reanimation reanimation = mApp.AddReanimation(theX, theY, mRenderOrder + 2, ReanimationType.Sleeping);
			reanimation.mLoopType = ReanimLoopType.Loop;
			reanimation.mAnimRate = TodCommon.RandRangeFloat(6f, 8f);
			reanimation.mAnimTime = TodCommon.RandRangeFloat(0f, 0.9f);
			mSleepingReanimID = mApp.ReanimationGetID(reanimation);
		}
		else
		{
			mApp.RemoveReanimation(ref mSleepingReanimID);
			mSleepingReanimID = null;
			if (IsInPlay() && mSeedType != SeedType.Hypnoshroom && !DoAdvancedSleeping(mSeedType))
			{
				mBoard.DoPlantingSound(this);
			}
		}
		Reanimation reanimation2 = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation2 == null)
		{
			return;
		}
		if (theIsAsleep)
		{
			if (!IsInPlay() && mSeedType == SeedType.Sunshroom)
			{
				reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_bigsleep);
			}
			else if (reanimation2.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_sleep))
			{
				float mAnimTime = reanimation2.mAnimTime;
				reanimation2.mLoopType = ReanimLoopType.Loop;
				reanimation2.StartBlend(20);
				reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_sleep);
				reanimation2.mAnimTime = mAnimTime;
			}
			else
			{
				reanimation2.mAnimRate = 1f;
			}
			EndBlink();
			return;
		}
		if (!IsInPlay() && mSeedType == SeedType.Sunshroom)
		{
			reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_bigidle);
		}
		else if (mSeedType == SeedType.Hypnoshroom)
		{
			reanimation2.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation2.mLoopCount = 0;
			reanimation2.SetFramesForLayer("anim_sleep_to_idle");
		}
		else if (DoAdvancedSleeping(mSeedType))
		{
			reanimation2.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation2.mLoopCount = 0;
			reanimation2.SetFramesForLayer("anim_sleep_to_idle");
		}
		else if (reanimation2.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
		{
			float mAnimTime2 = reanimation2.mAnimTime;
			reanimation2.StartBlend(20);
			reanimation2.mLoopType = ReanimLoopType.Loop;
			reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
			reanimation2.mAnimTime = mAnimTime2;
		}
		if (reanimation2.mAnimRate < 2f && IsInPlay())
		{
			reanimation2.mAnimRate = TodCommon.RandRangeFloat(10f, 15f);
		}
	}

	public bool HasSleepingReanim()
	{
		if (mSeedType == SeedType.Scaredyshroom)
		{
			return false;
		}
		return true;
	}

	public float[] GetSleepingReanimOffset()
	{
		float[] array = new float[2] { 50f, 40f };
		if (mSeedType == SeedType.Fumeshroom)
		{
			array[0] += 12f;
		}
		else if (mSeedType == SeedType.Scaredyshroom)
		{
			array[1] -= 20f;
		}
		else if (mSeedType == SeedType.Gloomshroom)
		{
			array[1] -= 12f;
		}
		return array;
	}

	public void UpdateShooting()
	{
		if (NotOnGround() || mState == PlantState.AgavePowerfulTendToLaunching || mState == PlantState.AgavePowerfulWaitingForTarget || mState == PlantState.AgavePowerfulPreparing || mState == PlantState.AgavePowerfulAttacking || mState == PlantState.AgavePowerfulAttacking || mState == PlantState.AgavePowerfulCanceling || mShootingCounter == 0)
		{
			return;
		}
		mShootingCounter--;
		if (mSeedType == SeedType.Fumeshroom && mShootingCounter == 15)
		{
			int theRenderPostition = Board.MakeRenderOrder(RenderLayer.Particle, mRow, 0);
			AddAttachedParticle(mX + 85 - 10, mY + 31 - 10, theRenderPostition, ParticleEffect.Fumecloud);
		}
		Reanimation reanimation6;
		Reanimation reanimation4;
		if (mSeedType == SeedType.Gloomshroom)
		{
			if (mShootingCounter == 136 || mShootingCounter == 108 || mShootingCounter == 80 || mShootingCounter == 52)
			{
				int theRenderPostition2 = Board.MakeRenderOrder(RenderLayer.Particle, mRow, 0);
				AddAttachedParticle(mX + 40, mY + 40, theRenderPostition2, ParticleEffect.Gloomcloud);
			}
			if (mShootingCounter == 126 || mShootingCounter == 98 || mShootingCounter == 70 || mShootingCounter == 42)
			{
				Fire(null, mRow, PlantWeapon.Primary);
			}
		}
		else if (mSeedType == SeedType.Gatlingpea)
		{
			if (mShootingCounter == 18 || mShootingCounter == 35 || mShootingCounter == 51 || mShootingCounter == 68)
			{
				Fire(null, mRow, PlantWeapon.Primary);
			}
		}
		else if (mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail)
		{
			if (mShootingCounter == 1)
			{
				Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
				if (zombie != null)
				{
					Fire(zombie, mRow, PlantWeapon.Primary);
				}
			}
			else if (mShootingCounter == 101)
			{
				Zombie zombie2 = FindTargetZombie(mRow, PlantWeapon.Primary);
				if (zombie2 != null)
				{
					Fire(zombie2, mRow, PlantWeapon.Secondary);
				}
				mShootingCounter -= 100;
			}
		}
		else if (mShootingCounter == 1)
		{
			if (mSeedType == SeedType.Threepeater)
			{
				int num = mRow - 1;
				int num2 = mRow + 1;
				int num3 = (mBoard.StageHas6Rows() ? 6 : 5);
				Reanimation reanimation = mApp.ReanimationTryToGet(mHeadReanimID);
				Reanimation reanimation2 = mApp.ReanimationTryToGet(mHeadReanimID2);
				Reanimation reanimation3 = mApp.ReanimationTryToGet(mHeadReanimID3);
				if (reanimation.mLoopType == ReanimLoopType.PlayOnceAndHold && num2 < num3)
				{
					Fire(null, num2, PlantWeapon.Primary);
				}
				if (reanimation2.mLoopType == ReanimLoopType.PlayOnceAndHold)
				{
					Fire(null, mRow, PlantWeapon.Primary);
				}
				if (reanimation3.mLoopType == ReanimLoopType.PlayOnceAndHold && num >= 0)
				{
					Fire(null, num, PlantWeapon.Primary);
				}
			}
			else if (mSeedType == SeedType.Splitpea)
			{
				reanimation4 = mApp.ReanimationTryToGet(mHeadReanimID);
				Reanimation reanimation5 = mApp.ReanimationTryToGet(mHeadReanimID2);
				if (reanimation4.mLoopType == ReanimLoopType.PlayOnceAndHold)
				{
					Fire(null, mRow, PlantWeapon.Primary);
				}
				if (reanimation5.mLoopType == ReanimLoopType.PlayOnceAndHold)
				{
					Fire(null, mRow, PlantWeapon.Secondary);
					Fire(null, mRow, PlantWeapon.Tertiary);
				}
			}
			else if (mSeedType == SeedType.Endoflame)
			{
				Zombie zombie3 = FindTargetZombie(mRow, PlantWeapon.Primary);
				if (zombie3 != null)
				{
					Fire(zombie3, mRow, PlantWeapon.Primary);
				}
			}
			else if (mState == PlantState.CactusLow)
			{
				Fire(null, mRow, PlantWeapon.Secondary);
			}
			else if (mSeedType == SeedType.Cabbagepult || mSeedType == SeedType.Kernelpult || mSeedType == SeedType.Melonpult || mSeedType == SeedType.Wintermelon)
			{
				PlantWeapon thePlantWeapon = PlantWeapon.Primary;
				if (mState == PlantState.KernelpultButter)
				{
					reanimation6 = mApp.ReanimationGet(mBodyReanimID);
					reanimation6.AssignRenderGroupToPrefix("Cornpult_butter", -1);
					reanimation6.AssignRenderGroupToPrefix("Cornpult_kernal", 0);
					mState = PlantState.Notready;
					thePlantWeapon = PlantWeapon.Secondary;
				}
				Zombie theTargetZombie = FindTargetZombie(mRow, thePlantWeapon);
				Fire(theTargetZombie, mRow, thePlantWeapon);
			}
			else
			{
				Fire(null, mRow, PlantWeapon.Primary);
			}
			return;
		}
		if (mShootingCounter > 0)
		{
			return;
		}
		reanimation6 = mApp.ReanimationTryToGet(mBodyReanimID);
		reanimation4 = mApp.ReanimationTryToGet(mHeadReanimID);
		if (mSeedType == SeedType.Threepeater)
		{
			Reanimation reanimation7 = reanimation4;
			Reanimation reanimation8 = mApp.ReanimationTryToGet(mHeadReanimID2);
			Reanimation reanimation9 = mApp.ReanimationTryToGet(mHeadReanimID3);
			if (reanimation8.mLoopCount > 0)
			{
				if (reanimation7.mLoopType == ReanimLoopType.PlayOnceAndHold)
				{
					reanimation7.StartBlend(20);
					reanimation7.mLoopType = ReanimLoopType.Loop;
					reanimation7.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle1);
					reanimation7.mAnimRate = reanimation6.mAnimRate;
					reanimation7.mAnimTime = reanimation6.mAnimTime;
				}
				reanimation8.StartBlend(20);
				reanimation8.mLoopType = ReanimLoopType.Loop;
				reanimation8.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle2);
				reanimation8.mAnimRate = reanimation6.mAnimRate;
				reanimation8.mAnimTime = reanimation6.mAnimTime;
				if (reanimation9.mLoopType == ReanimLoopType.PlayOnceAndHold)
				{
					reanimation9.StartBlend(20);
					reanimation9.mLoopType = ReanimLoopType.Loop;
					reanimation9.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle3);
					reanimation9.mAnimRate = reanimation6.mAnimRate;
					reanimation9.mAnimTime = reanimation6.mAnimTime;
				}
				return;
			}
		}
		else
		{
			if (mSeedType == SeedType.Splitpea)
			{
				Reanimation reanimation10 = mApp.ReanimationGet(mHeadReanimID2);
				if (reanimation4.mLoopCount > 0)
				{
					reanimation4.StartBlend(20);
					reanimation4.mLoopType = ReanimLoopType.Loop;
					reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
					reanimation4.mAnimRate = reanimation6.mAnimRate;
					reanimation4.mAnimTime = reanimation6.mAnimTime;
				}
				if (reanimation10.mLoopCount > 0)
				{
					reanimation10.StartBlend(20);
					reanimation10.mLoopType = ReanimLoopType.Loop;
					reanimation10.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_splitpea_idle);
					reanimation10.mAnimRate = reanimation6.mAnimRate;
					reanimation10.mAnimTime = reanimation6.mAnimTime;
				}
				return;
			}
			if (mSeedType == SeedType.Gatlingpea)
			{
				if (mState == PlantState.GatlingPreShooting1 || mState == PlantState.GatlingIdle1)
				{
					reanimation4.StartBlend(20);
					reanimation4.mLoopType = ReanimLoopType.Loop;
					reanimation4.SetFramesForLayer("anim_head_idle1");
					mState = PlantState.GatlingIdle1;
				}
				return;
			}
			if (mState == PlantState.CactusHigh)
			{
				if (reanimation6.mLoopCount > 0)
				{
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idlehigh, ReanimLoopType.Loop, 20, 0f);
					reanimation6.mAnimRate = reanimation6.mDefinition.mFPS;
					if (mApp.IsIZombieLevel())
					{
						reanimation6.mAnimRate = 0f;
					}
					return;
				}
			}
			else if (reanimation4 != null)
			{
				if (reanimation4.mLoopCount > 0)
				{
					reanimation4.StartBlend(20);
					reanimation4.mLoopType = ReanimLoopType.Loop;
					reanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
					reanimation4.mAnimRate = reanimation6.mAnimRate;
					reanimation4.mAnimTime = reanimation6.mAnimTime;
					return;
				}
			}
			else if (mSeedType == SeedType.Cobcannon)
			{
				if (reanimation6.mLoopCount > 0)
				{
					mState = PlantState.CobcannonArming;
					mStateCountdown = 3000;
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_unarmed_idle, ReanimLoopType.Loop, 20, reanimation6.mDefinition.mFPS);
					return;
				}
			}
			else if (reanimation6 != null && reanimation6.mLoopCount > 0)
			{
				PlayIdleAnim(reanimation6.mDefinition.mFPS);
				return;
			}
		}
		mShootingCounter = 1;
	}

	public void DrawShadow(Graphics g, float theOffsetX, float theOffsetY)
	{
		if (mSeedType == SeedType.Lilypad || mSeedType == SeedType.Starfruit || mSeedType == SeedType.Tanglekelp || mSeedType == SeedType.Seashroom || mSeedType == SeedType.Cobcannon || mSeedType == SeedType.Cattail || mSeedType == SeedType.HypnoCattail || mSeedType == SeedType.Pumpkinshell || mOnBungeeState == PlantOnBungeeState.RisingWithBungee || mState == PlantState.Dying || mSeedType == SeedType.Jalapeno || (mSeedType == SeedType.Hypnoshroom && (mState == PlantState.Doingspecial || mState == PlantState.Entering)) || (IsOnBoard() && mBoard.mApp.mGameMode == GameMode.ChallengeZenGarden && (mBoard.mApp.mZenGarden.mGardenType == GardenType.Main || mBoard.mApp.mZenGarden.mGardenType == GardenType.Main2 || mBoard.mApp.mZenGarden.mGardenType == GardenType.Night)) || (IsOnBoard() && mBoard.mPlantRow[mRow] == PlantRowType.Pool))
		{
			return;
		}
		int num = 0;
		float num2 = -3f;
		float num3 = 51f;
		float num4 = 1f;
		if (mBoard != null && mBoard.StageIsNight())
		{
			num = 1;
		}
		if (mSeedType == SeedType.Gravebuster && (mState == PlantState.GravebusterEating || mState == PlantState.GravebusterLanding))
		{
			num2 -= 20f;
		}
		if (mSeedType == SeedType.Blover && mDisappearCountdown < 30)
		{
			num4 *= TodCommon.TodAnimateCurveFloat(30, 0, mDisappearCountdown, 1f, 0.01f, TodCurves.EaseIn);
		}
		if (mSeedType == SeedType.Squash)
		{
			if (mBoard != null)
			{
				num3 += (float)(mBoard.GridToPixelY(mPlantCol, mRow) - mY);
			}
			if (mState == PlantState.SquashRising)
			{
				float num5 = mStateCountdown;
				float num6 = 30f;
				num5 = ((num5 <= 0f) ? 0.2f : num5);
				float num7 = Math.Max(num5 / num6, 0.001f);
				if (mStateCountdown <= 30)
				{
					num7 *= 0.5f;
				}
				num7 = Math.Min(num7, 1f);
				num4 *= num7;
			}
			else if (mState == PlantState.SquashFalling)
			{
				float num8 = 40 - mStateCountdown;
				float num9 = 36f;
				num8 = ((num8 <= 0f) ? 1f : num8);
				float num10 = Math.Min(num8 / num9, 1f);
				num4 *= num10;
			}
			else if (mState == PlantState.SquashDoneFalling)
			{
				float num11 = mStateCountdown;
				float num12 = 35f;
				num11 = ((num11 <= 0f) ? 0.2f : num11);
				float num13 = Math.Max(num11 / num12, 0.001f);
				if (mStateCountdown <= 35)
				{
					num13 *= 0.3f;
				}
				num13 = Math.Min(num13, 1f);
				num4 *= num13;
			}
			num3 += 5f;
		}
		else if (mSeedType == SeedType.Puffshroom || mSeedType == SeedType.Seashroom)
		{
			num4 = 0.5f;
			num3 -= 9f;
		}
		else if (mSeedType == SeedType.Sunshroom)
		{
			num3 += -9f;
			if (mState == PlantState.SunshroomSmall)
			{
				num4 = 0.5f;
			}
			else if (mState == PlantState.SunshroomGrowing)
			{
				Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
				num4 = 0.5f + 0.5f * reanimation.mAnimTime;
			}
			else
			{
				num4 = 1f;
			}
		}
		else if (mSeedType == SeedType.Umbrella)
		{
			num4 = 0.5f;
			num2 -= 4f;
			num3++;
		}
		else if (mSeedType == SeedType.Fumeshroom)
		{
			num4 = 0.9f;
			num3 -= 4f;
		}
		else if (mSeedType == SeedType.Gloomshroom)
		{
			num4 = 1.3f;
			num3 -= 4f;
		}
		else if (mSeedType == SeedType.Cabbagepult || mSeedType == SeedType.Melonpult || mSeedType == SeedType.Wintermelon)
		{
			num3 -= 4f;
		}
		else if (mSeedType == SeedType.Kernelpult)
		{
			num2 += 3f;
			num3 -= 4f;
		}
		else if (mSeedType == SeedType.Scaredyshroom)
		{
			num3 += 2f;
		}
		else if (mSeedType == SeedType.SuperChomper)
		{
			num2 += -18f;
			num3 += 6f;
		}
		else if (mSeedType == SeedType.Chomper)
		{
			num2 += 2f;
		}
		else if (mSeedType == SeedType.Flowerpot)
		{
			num2 += -1f;
			num3 += -5f;
		}
		else if (mSeedType == SeedType.Tallnut)
		{
			num3 += 3f;
			num4 = 1.3f;
		}
		else if (mSeedType == SeedType.Pumpkinshell)
		{
			num3 += -5f;
			num4 = 1.4f;
		}
		else if (mSeedType == SeedType.Cactus)
		{
			num2 += -5f;
			num3 += -1f;
		}
		else if (mSeedType == SeedType.Plantern)
		{
			num3 += 6f;
		}
		else if (mSeedType == SeedType.InstantCoffee)
		{
			num3 += 20f;
		}
		else if (mSeedType == SeedType.GiantWallnut)
		{
			num2 -= 30f;
			num3 += 5f;
			num4 = 1.7f;
		}
		if (IsFlying(mSeedType))
		{
			num3 += 10f;
			if (mBoard != null && (mBoard.GetTopPlantAt(mPlantCol, mRow, TopPlant.OnlyNormalPosition) != null || mBoard.GetTopPlantAt(mPlantCol, mRow, TopPlant.OnlyPumpkin) != null))
			{
				return;
			}
		}
		if (num == 0)
		{
			TodCommon.TodDrawImageCenterScaledF(g, AtlasResources.IMAGE_PLANTSHADOW, (theOffsetX + num2) * Constants.S, (theOffsetY + num3) * Constants.S, num4, num4);
		}
		else
		{
			TodCommon.TodDrawImageCenterScaledF(g, AtlasResources.IMAGE_PLANTSHADOW2, (theOffsetX + num2) * Constants.S, (theOffsetY + num3) * Constants.S, num4, num4);
		}
	}

	public void DoAgaveDamage(PlantWeapon theWeapon, int theDamage, bool isAway, bool isPowerfulSkill = false)
	{
		int theDamage2 = theDamage * 3 / 2;
		int damageRangeFlags = GetDamageRangeFlags(theWeapon);
		TRect plantAttackRect = GetPlantAttackRect(theWeapon);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead)
			{
				continue;
			}
			int num = zombie.mRow - mRow;
			if (zombie.mZombieType == ZombieType.Boss)
			{
				num = 0;
			}
			if ((num != 0 && !isPowerfulSkill) || zombie.mOnHighGround != IsOnHighGround() || !zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			if (isPowerfulSkill ? GameConstants.GetCircleRectOverlap(mTargetX, mTargetY, 115, zombieRect) : (GameConstants.GetRectOverlap(plantAttackRect, zombieRect) >= 0))
			{
				if (isPowerfulSkill && (zombie.mHelmType == HelmType.Pail || zombie.mHelmType == HelmType.Football || zombie.mHelmType == HelmType.Digger || zombie.mHelmType == HelmType.Bell || zombie.mHelmType == HelmType.FootballPremium || zombie.mHelmType == HelmType.RobotTitanHead || zombie.mHelmType == HelmType.RedeyeRobotTitanHead || zombie.mShieldType == ShieldType.Door || zombie.mShieldType == ShieldType.Ladder || zombie.mZombieType == ZombieType.Boss))
				{
					zombie.TakeDamage(theDamage2, 0u);
				}
				else
				{
					zombie.TakeDamage(theDamage, 0u);
				}
				if (isAway && zombie.mZombieType != ZombieType.Boss && zombie.mZombieType != ZombieType.RobotTitan && zombie.mZombieType != ZombieType.RedeyeRobotTitan && zombie.mZombieType != ZombieType.Zamboni && zombie.mZombieType != ZombieType.Bobsled && zombie.mZombieType != ZombieType.Catapult && zombie.mZombieType != ZombieType.Bungee && zombie.mAwayCounter < 50)
				{
					zombie.mAwayCounter = 50;
				}
				mApp.PlayFoley(FoleyType.Splat);
			}
		}
	}

	public void DoGatlingDamage(PlantWeapon theWeapon, int theDamage, int theAwayAmount = 0)
	{
		int num = theDamage * 3 / 2;
		int damageRangeFlags = GetDamageRangeFlags(theWeapon);
		TRect plantAttackRect = GetPlantAttackRect(theWeapon);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (zombie.mDead)
			{
				continue;
			}
			int num2 = zombie.mRow - mRow;
			if (zombie.mZombieType == ZombieType.Boss)
			{
				num2 = 0;
			}
			if (num2 != 0 || zombie.mOnHighGround != IsOnHighGround() || !zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			if (GameConstants.GetRectOverlap(plantAttackRect, zombieRect) >= 0)
			{
				zombie.TakeDamage(theDamage, 0u);
				mApp.PlayFoley(FoleyType.Splat);
				if (theAwayAmount != 0 && zombie.mZombieType != ZombieType.Boss && zombie.mZombieType != ZombieType.RobotTitan && zombie.mZombieType != ZombieType.RedeyeRobotTitan && zombie.mZombieType != ZombieType.Zamboni && zombie.mZombieType != ZombieType.Bobsled && zombie.mZombieType != ZombieType.Catapult && zombie.mZombieType != ZombieType.Bungee && zombie.mAwayCounter < 50 && zombie.mShieldType == ShieldType.None && zombie.mZombieType != ZombieType.Gargantuar && zombie.mZombieType != ZombieType.RedeyeGargantuar)
				{
					zombie.mAwayCounter = theAwayAmount;
				}
			}
		}
	}

	public void DoEndoflameDamage(PlantWeapon theWeapon, int theDamage, int theAroundDamage, bool theDoBossDamage = true)
	{
		int damageRangeFlags = GetDamageRangeFlags(theWeapon);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			bool flag = true;
			if (zombie.mDead)
			{
				continue;
			}
			int num = zombie.mRow - mBoard.PixelToGridY(mTargetX, mTargetY);
			if (zombie.mZombieType == ZombieType.Boss)
			{
				num = 0;
			}
			if (num == 0)
			{
				flag = false;
			}
			if (zombie.mOnHighGround != IsOnHighGround() || !zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				continue;
			}
			TRect zombieRect = zombie.GetZombieRect();
			bool circleRectOverlap = GameConstants.GetCircleRectOverlap(mTargetX, mTargetY, 115, zombieRect);
			bool flag2 = !flag && GameConstants.GetCircleRectOverlap(mTargetX, mTargetY, 60, zombieRect);
			if (!circleRectOverlap)
			{
				continue;
			}
			if (zombie.mZombieType == ZombieType.Boss)
			{
				if (theDoBossDamage)
				{
					zombie.ApplyBurn();
				}
			}
			else if (flag2)
			{
				if (theDamage >= 7200)
				{
					zombie.ApplyBurn();
					zombie.ApplyBurn();
					zombie.ApplyBurn();
					zombie.TakeDamage(theDamage - 7200, 18u);
					zombie.ApplyBurn();
				}
				else if (theDamage >= 5400)
				{
					zombie.ApplyBurn();
					zombie.ApplyBurn();
					zombie.TakeDamage(theDamage - 5400, 18u);
					zombie.ApplyBurn();
				}
				else if (theDamage >= 3600)
				{
					zombie.ApplyBurn();
					zombie.TakeDamage(theDamage - 3600, 18u);
					zombie.ApplyBurn();
				}
				else if (theDamage >= 1800)
				{
					zombie.ApplyBurn();
					zombie.TakeDamage(theDamage - 1800, 18u);
				}
				else
				{
					zombie.TakeDamage(theDamage, 18u);
				}
			}
			else
			{
				zombie.TakeDamage(theAroundDamage, 0u);
			}
		}
		Zombie bossZombie = mBoard.GetBossZombie();
		if (bossZombie == null)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(bossZombie.mBossFireBallReanimID);
		if (reanimation != null)
		{
			float num2 = reanimation.mOverlayMatrix.mMatrix.M41 * Constants.IS + 80f;
			float num3 = reanimation.mOverlayMatrix.mMatrix.M42 * Constants.IS + 80f;
			if (GameConstants.GetCircleRectOverlap(mTargetX, mTargetY, 60, new TRect((int)num2, (int)num3, 100, 100)))
			{
				bossZombie.BossDestroyIceballInRow(bossZombie.mFireballRow);
				bossZombie.BossDestroyFireball();
			}
		}
	}

	public bool AgaveSkillCanCancel()
	{
		if ((mSeedType == SeedType.Agave || mSeedType == SeedType.Endoflame) && (mState == PlantState.AgavePowerfulTendToLaunching || mState == PlantState.AgavePowerfulPreparing || mState == PlantState.AgavePowerfulWaitingForTarget))
		{
			return true;
		}
		return false;
	}

	public void UpdateAgave()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mLightReanimID);
		Reanimation reanimation2 = mApp.ReanimationTryToGet(mAttackingReanimID);
		if (reanimation != null && reanimation.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation.mLoopCount > 0)
		{
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.LoopFullLastFrame, 0, 30f);
		}
		if (reanimation2 != null && reanimation2.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation2.mLoopCount > 0)
		{
			mApp.RemoveReanimation(ref mAttackingReanimID);
			reanimation2 = null;
		}
		if (mState == PlantState.Notready)
		{
			mState = PlantState.Ready;
			mStateCountdown = mLaunchRate - RandomNumbers.NextNumber(15);
			Reanimation reanimation3 = mApp.ReanimationTryToGet(mBodyReanimID);
			if (reanimation3 != null && !reanimation3.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
			{
				PlayIdleAnim(reanimation3.mDefinition.mFPS);
			}
		}
		else if (mState == PlantState.Ready)
		{
			if (mStateCountdown <= 0)
			{
				if (FindTargetZombie(mRow, PlantWeapon.Primary) != null)
				{
					mState = PlantState.AgaveAttacking;
					mStateCountdown = 370;
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 30f);
				}
				else
				{
					mState = PlantState.Notready;
				}
			}
		}
		else if (mState == PlantState.AgaveAttacking)
		{
			if (mStateCountdown == 335 || mStateCountdown == 280)
			{
				DoAgaveDamage(PlantWeapon.Primary, 40, isAway: false);
			}
			else if (mStateCountdown == 245)
			{
				mApp.PlaySample(Resources.SOUND_AGAVE_ATTACK);
			}
			else if (mStateCountdown == 135)
			{
				DoAgaveDamage(PlantWeapon.Primary, 100, isAway: true);
			}
			else if (mStateCountdown <= 0)
			{
				mState = PlantState.Notready;
			}
		}
		else if (mState == PlantState.AgavePowerfulTendToLaunching)
		{
			mState = PlantState.AgavePowerfulPreparing;
			mStateCountdown = 30;
			PlayBodyReanim("anim_idle2ready", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			mApp.PlaySample(Resources.SOUND_AGAVE_IDLE2PREPARE);
		}
		else if (mState == PlantState.AgavePowerfulPreparing)
		{
			Reanimation reanimation4 = mApp.ReanimationTryToGet(mBodyReanimID);
			if (mStateCountdown <= 0 || (reanimation4 != null && reanimation4.mLoopCount > 0))
			{
				mState = PlantState.AgavePowerfulWaitingForTarget;
				mStateCountdown = 0;
				PlayBodyReanim("anim_ready", ReanimLoopType.Loop, 20, 30f);
			}
		}
		else
		{
			if (mState == PlantState.AgavePowerfulWaitingForTarget)
			{
				return;
			}
			if (mState == PlantState.AgavePowerfulTendToCanceling)
			{
				mState = PlantState.AgavePowerfulCanceling;
				mStateCountdown = 30;
				PlayBodyReanim("anim_ready2idle", ReanimLoopType.PlayOnceAndHold, 20, 30f);
				mApp.PlaySample(Resources.SOUND_AGAVE_PREPARE2IDLE);
			}
			else if (mState == PlantState.AgavePowerfulCanceling)
			{
				Reanimation reanimation5 = mApp.ReanimationTryToGet(mBodyReanimID);
				if (mStateCountdown <= 0 || (reanimation5 != null && reanimation5.mLoopCount > 0))
				{
					mState = PlantState.Notready;
				}
			}
			else if (mState == PlantState.AgavePowerfulAttacking)
			{
				if (mStateCountdown == 400)
				{
					if (reanimation2 != null)
					{
						reanimation2.ReanimationDie();
						reanimation2 = null;
					}
					Reanimation reanimation6 = mApp.AddReanimation(mTargetX - 310 - 40, mTargetY - 200, 800000, ReanimationType.AgaveFullattack);
					reanimation6.PlayReanim("anim_fullattack", ReanimLoopType.PlayOnce, 0, 60f);
					mAttackingReanimID = mApp.ReanimationGetID(reanimation6);
				}
				if (mStateCountdown == 390 || mStateCountdown == 370 || mStateCountdown == 350 || mStateCountdown == 330 || mStateCountdown == 310)
				{
					DoAgaveDamage(PlantWeapon.Secondary, 20, isAway: false, isPowerfulSkill: true);
				}
				else if (mStateCountdown == 200)
				{
					DoAgaveDamage(PlantWeapon.Secondary, 1700, isAway: false, isPowerfulSkill: true);
				}
				else if (mStateCountdown == 83)
				{
					PlayBodyReanim("anim_atk2_2", ReanimLoopType.PlayOnceAndHold, 20, 30f);
				}
				else if (mStateCountdown <= 0)
				{
					mState = PlantState.Notready;
				}
			}
			else if (mState == PlantState.AgaveEntering)
			{
				Reanimation reanimation7 = mApp.ReanimationTryToGet(mBodyReanimID);
				if (mStateCountdown <= 0 || (reanimation7 != null && reanimation7.mLoopCount > 0))
				{
					mState = PlantState.Ready;
					mStateCountdown = 0;
					PlayIdleAnim(reanimation7?.mDefinition.mFPS ?? 30f);
				}
			}
		}
	}

	public void UpdateEndoflame()
	{
		if (mState == PlantState.AgavePowerfulTendToLaunching)
		{
			PlantVoice.Play(SeedType.Endoflame, SeedType.None, PlantVoice.VoiceType.Power);
			mState = PlantState.AgavePowerfulPreparing;
			PlayBodyReanim("anim_idle_to_prepare", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			mStateCountdown = 30;
		}
		else if (mState == PlantState.AgavePowerfulPreparing)
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
			if (mStateCountdown <= 0 || (reanimation != null && reanimation.mLoopCount > 0))
			{
				mState = PlantState.AgavePowerfulWaitingForTarget;
				mStateCountdown = 0;
				PlayBodyReanim("anim_prepare", ReanimLoopType.Loop, 20, 30f);
			}
		}
		else if (mState == PlantState.AgavePowerfulAttacking)
		{
			Reanimation reanimation2 = mApp.ReanimationTryToGet(mAttackingReanimID);
			if (mStateCountdown == 150)
			{
				if (reanimation2 != null)
				{
					reanimation2.ReanimationDie();
					reanimation2 = null;
				}
				Reanimation reanimation3 = mApp.AddReanimation(mTargetX - 310 + 60, mTargetY - 380, 800000, ReanimationType.EndoflameFullattack);
				reanimation3.PlayReanim("anim_fullattack", ReanimLoopType.PlayOnce, 0, 60f);
				mAttackingReanimID = mApp.ReanimationGetID(reanimation3);
				mApp.PlaySample(Resources.SOUND_ENDOFLAME_POWERFUL);
			}
			else if (mStateCountdown == 50)
			{
				DoEndoflameDamage(PlantWeapon.Secondary, 6000, 900);
			}
			else if (mStateCountdown <= 0)
			{
				mSubclass = 1;
				mState = PlantState.Notready;
				PlayIdleAnim(30f);
			}
		}
		else if (mState == PlantState.AgavePowerfulTendToCanceling)
		{
			mState = PlantState.AgavePowerfulCanceling;
			mStateCountdown = 30;
			PlayBodyReanim("anim_prepare_to_idle", ReanimLoopType.PlayOnceAndHold, 20, 30f);
			PlantVoice.Play(SeedType.Endoflame, SeedType.None, PlantVoice.VoiceType.SkillCancel);
		}
		else if (mState == PlantState.AgavePowerfulCanceling)
		{
			Reanimation reanimation4 = mApp.ReanimationTryToGet(mBodyReanimID);
			if (mStateCountdown <= 0 || (reanimation4 != null && reanimation4.mLoopCount > 0))
			{
				mSubclass = 1;
				mState = PlantState.Notready;
			}
		}
	}

	public void UpdateGatlingpea()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mHeadReanimID);
		if (reanimation == null)
		{
			return;
		}
		if (mState == PlantState.GatlingShooting2)
		{
			if (mStateCountdown <= 0)
			{
				if (FindTargetZombie(mRow, PlantWeapon.Secondary) != null)
				{
					reanimation.StartBlend(20);
					reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
					reanimation.SetFramesForLayer("anim_shooting2");
					mStateCountdown = 80;
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Gatlingpea_Attack1);
				}
				else
				{
					reanimation.StartBlend(20);
					reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
					reanimation.SetFramesForLayer("anim_shooting2_after");
					mState = PlantState.GatlingAfterShooting2;
					mStateCountdown = 140;
					PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Gatlingpea_Attack2);
				}
			}
			else if (mStateCountdown == 50 || mStateCountdown == 20)
			{
				DoGatlingDamage(PlantWeapon.Secondary, 40);
			}
		}
		else if (mState == PlantState.GatlingAfterShooting2 && mStateCountdown <= 0)
		{
			mSubclass = 1;
			reanimation.StartBlend(20);
			reanimation.mLoopType = ReanimLoopType.Loop;
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
			mState = PlantState.Notready;
		}
		else if (mState == PlantState.GatlingPreShooting2 && mStateCountdown <= 0)
		{
			reanimation.StartBlend(20);
			reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
			reanimation.SetFramesForLayer("anim_shooting2");
			mState = PlantState.GatlingShooting2;
			mStateCountdown = 80;
			mSubclass = 0;
			PlantVoice.Play(SeedType.None, SeedType.None, PlantVoice.VoiceType.Special_Gatlingpea_Attack1);
		}
	}

	public void UpdateScaredyShroom()
	{
		if (mShootingCounter > 0)
		{
			return;
		}
		bool flag = false;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead)
			{
				TRect zombieRect = zombie.GetZombieRect();
				int num = zombie.mRow - mRow;
				if (zombie.mZombieType == ZombieType.Boss)
				{
					num = 0;
				}
				if (!zombie.mMindControlled && !zombie.IsDeadOrDying() && num <= 1 && num >= -1 && GameConstants.GetCircleRectOverlap(mX, mY + 20, 120, zombieRect))
				{
					flag = true;
					break;
				}
			}
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mState == PlantState.Ready)
		{
			if (flag)
			{
				mState = PlantState.ScaredyshroomLowering;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_scared, ReanimLoopType.PlayOnceAndHold, 10, 30f);
			}
		}
		else if (mState == PlantState.ScaredyshroomLowering)
		{
			if (reanimation.mLoopCount > 0)
			{
				mState = PlantState.ScaredyshroomScared;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_scaredidle, ReanimLoopType.Loop, 10, 20f);
			}
		}
		else if (mState == PlantState.ScaredyshroomScared)
		{
			if (!flag)
			{
				mState = PlantState.ScaredyshroomRaising;
				float theAnimRate = 0f;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_grow, ReanimLoopType.PlayOnceAndHold, 10, theAnimRate);
			}
		}
		else if (mState == PlantState.ScaredyshroomRaising && reanimation.mLoopCount > 0)
		{
			mState = PlantState.Ready;
			float theRate = TodCommon.RandRangeFloat(10f, 15f);
			PlayIdleAnim(theRate);
		}
		if (mState != PlantState.Ready)
		{
			mLaunchCounter = mLaunchRate;
		}
	}

	public int DistanceToClosestZombie()
	{
		int damageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
		TRect plantAttackRect = GetPlantAttackRect(PlantWeapon.Primary);
		int num = 1000;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && zombie.mRow == mRow && zombie.EffectedByDamage((uint)damageRangeFlags))
			{
				TRect zombieRect = zombie.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, zombieRect);
				if (num > -rectOverlap)
				{
					num = Math.Max(-rectOverlap, 0);
				}
			}
		}
		return num;
	}

	public void UpdateSpikeweed()
	{
		if (mState == PlantState.Doingspecial)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mSeedType == SeedType.Spikeweed && mState == PlantState.SpikeweedAttacking && mStateCountdown <= 0)
		{
			mState = PlantState.Notready;
			mRenderOrder -= 5000;
		}
		if (mState == PlantState.SpikeweedAttacking || mState == PlantState.SpikeweedAttacking2)
		{
			if (mStateCountdown <= 0)
			{
				mState = ((mState == PlantState.SpikeweedAttacking) ? PlantState.Ready : PlantState.Notready);
			}
			else if (mSeedType == SeedType.Spikerock)
			{
				if (mStateCountdown == 50)
				{
					PlayBodyReanim("anim_attack2" + GetStateTrackIdSuffix(), ReanimLoopType.PlayOnceAndHold, 20, 30f);
					reanimation.mAnimTime = 0.33f;
					mApp.PlaySample(Resources.SOUND_THROW);
				}
				if (mStateCountdown == 69 || mStateCountdown == 33)
				{
					DoRowAreaDamage(20, 33u);
				}
			}
			else if (mStateCountdown == 50)
			{
				DoRowAreaDamage(20, 33u);
			}
			if (reanimation.mLoopCount > 0)
			{
				if (mSeedType == SeedType.Spikeweed)
				{
					PlayIdleAnim(30f);
					return;
				}
				float theRate = 30f;
				PlayIdleAnim(theRate);
			}
		}
		else
		{
			Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
			if (zombie != null)
			{
				SpikeweedAttack();
			}
			else if (reanimation.mLoopCount > 0)
			{
				float num = 30f;
				PlayIdleAnim(30f);
			}
		}
	}

	public void MagnetShroomAttactItem(Zombie theZombie)
	{
		mState = PlantState.MagnetshroomSucking;
		mStateCountdown = 1500;
		PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 12f);
		mApp.PlayFoley(FoleyType.Magnetshroom);
		MagnetItem freeMagnetItem = GetFreeMagnetItem();
		if (theZombie.mHelmType == HelmType.Pail)
		{
			int helmDamageIndex = theZombie.GetHelmDamageIndex();
			theZombie.mHelmHealth = 0;
			theZombie.mHelmType = HelmType.None;
			theZombie.mHasHelm = false;
			theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_bucket, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			theZombie.ReanimShowPrefix("anim_bucket", -1);
			theZombie.ReanimShowPrefix("anim_hair", 0);
			freeMagnetItem.mPosX -= AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET1.mWidth / 2;
			freeMagnetItem.mPosY -= AtlasResources.IMAGE_REANIM_ZOMBIE_BUCKET1.mHeight / 2;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 25f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 20f;
			freeMagnetItem.mItemType = (MagnetItemType)(1 + helmDamageIndex);
		}
		else if (theZombie.mHelmType == HelmType.Football)
		{
			int helmDamageIndex2 = theZombie.GetHelmDamageIndex();
			theZombie.mHelmHealth = 0;
			theZombie.mHelmType = HelmType.None;
			theZombie.mHasHelm = false;
			theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			theZombie.ReanimShowPrefix("zombie_football_helmet", -1);
			theZombie.ReanimShowPrefix("anim_hair", 0);
			freeMagnetItem.mPosX = theZombie.mPosX + 37f;
			freeMagnetItem.mPosY = theZombie.mPosY - 60f;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 20f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 20f;
			freeMagnetItem.mItemType = (MagnetItemType)(4 + helmDamageIndex2);
		}
		else if (theZombie.mHelmType == HelmType.FootballPremium)
		{
			int helmDamageIndex3 = theZombie.GetHelmDamageIndex();
			theZombie.mHelmHealth = 0;
			theZombie.mHelmType = HelmType.None;
			theZombie.mHasHelm = false;
			theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_football_helmet, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			theZombie.ReanimShowPrefix("zombie_football_helmet", -1);
			theZombie.ReanimShowPrefix("anim_hair", 0);
			freeMagnetItem.mPosX = theZombie.mPosX + 37f;
			freeMagnetItem.mPosY = theZombie.mPosY - 60f;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 20f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 20f;
			freeMagnetItem.mItemType = (MagnetItemType)(26 + helmDamageIndex3);
		}
		else if (theZombie.mShieldType == ShieldType.Door)
		{
			int shieldDamageIndex = theZombie.GetShieldDamageIndex();
			theZombie.DetachShield();
			theZombie.mZombiePhase = ZombiePhase.ZombieNormal;
			if (!theZombie.mIsEating)
			{
				Debug.ASSERT(theZombie.mZombieHeight == ZombieHeight.ZombieNormal);
				theZombie.StartWalkAnim(0);
			}
			theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_anim_screendoor, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			freeMagnetItem.mPosX -= AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR1.mWidth / 2;
			freeMagnetItem.mPosY -= AtlasResources.IMAGE_REANIM_ZOMBIE_SCREENDOOR1.mHeight / 2;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 30f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f);
			freeMagnetItem.mItemType = (MagnetItemType)(7 + shieldDamageIndex);
		}
		else if (theZombie.mShieldType == ShieldType.Ladder)
		{
			int shieldDamageIndex2 = theZombie.GetShieldDamageIndex();
			theZombie.DetachShield();
			freeMagnetItem.mPosX = theZombie.mPosX + 31f;
			freeMagnetItem.mPosY = theZombie.mPosY + 20f;
			freeMagnetItem.mPosX -= AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_5.GetCelWidth() / 2;
			freeMagnetItem.mPosY -= AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_5.GetCelHeight() / 2;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 30f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f);
			freeMagnetItem.mItemType = (MagnetItemType)(14 + shieldDamageIndex2);
		}
		else if (theZombie.mZombieType == ZombieType.Pogo)
		{
			theZombie.PogoBreak(16u);
			ZombieDrawPosition theDrawPos = default;
			theZombie.GetDrawPos(ref theDrawPos);
			theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_pogo_stick, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			freeMagnetItem.mPosX += (0f - (float)AtlasResources.IMAGE_REANIM_ZOMBIE_POGO_STICKDAMAGE1.mWidth) / 2f + 40f;
			freeMagnetItem.mPosY += (0f - (float)AtlasResources.IMAGE_REANIM_ZOMBIE_POGO_STICKDAMAGE1.mHeight) / 2f + 84f;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 44f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 10f;
			if (theZombie.mHasArm)
			{
				freeMagnetItem.mItemType = MagnetItemType.Pogo1;
			}
			else
			{
				freeMagnetItem.mItemType = MagnetItemType.Pogo3;
			}
		}
		else if (theZombie.mZombiePhase == ZombiePhase.JackInTheBoxRunning)
		{
			theZombie.StopZombieSound();
			theZombie.PickRandomSpeed();
			theZombie.mZombiePhase = ZombiePhase.ZombieNormal;
			if (theZombie.mBoard != null && theZombie.mBoard.mPlantRow[theZombie.mRow] == PlantRowType.Pool)
			{
				theZombie.ReanimShowTrack("anim_Zombie_jackbox_duckytube_box", -1);
				theZombie.GetTrackPosition("anim_Zombie_jackbox_duckytubehead", ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			}
			else
			{
				theZombie.ReanimShowTrack("Zombie_jackbox_box2", -1);
				theZombie.ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_jackbox_box, -1);
				theZombie.ReanimShowTrack(ref GlobalMembersReanimIds.ReanimTrackId_zombie_jackbox_handle, -1);
				theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_jackbox_box, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			}
			freeMagnetItem.mPosX -= AtlasResources.IMAGE_REANIM_ZOMBIE_JACKBOX_BOX.mWidth / 2;
			freeMagnetItem.mPosY -= AtlasResources.IMAGE_REANIM_ZOMBIE_JACKBOX_BOX.mHeight / 2;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 20f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 15f;
			freeMagnetItem.mItemType = MagnetItemType.JackInTheBox;
		}
		else if (theZombie.mZombieType == ZombieType.Digger)
		{
			theZombie.DiggerLoseAxe();
			theZombie.GetTrackPosition(ref GlobalMembersReanimIds.ReanimTrackId_zombie_digger_pickaxe, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			freeMagnetItem.mPosX -= AtlasResources.IMAGE_REANIM_ZOMBIE_DIGGER_PICKAXE.mWidth / 2;
			freeMagnetItem.mPosY -= AtlasResources.IMAGE_REANIM_ZOMBIE_DIGGER_PICKAXE.mHeight / 2;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 45f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 15f;
			freeMagnetItem.mItemType = MagnetItemType.PickAxe;
		}
		else if (theZombie.mHelmType == HelmType.RobotTitanHead || theZombie.mHelmType == HelmType.RedeyeRobotTitanHead)
		{
			int helmDamageIndex4 = theZombie.GetHelmDamageIndex();
			HelmType mHelmType = theZombie.mHelmType;
			MagnetItemType magnetItemType;
			if (mHelmType != HelmType.RobotTitanHead)
			{
				if (mHelmType != HelmType.RedeyeRobotTitanHead)
				{
					goto IL_0701;
				}
				if (helmDamageIndex4 == 2)
				{
					magnetItemType = MagnetItemType.RedeyeRobotTitanHead2;
				}
				else
				{
					int num = helmDamageIndex4;
					if (num >= 2)
					{
						goto IL_0701;
					}
					magnetItemType = MagnetItemType.RedeyeRobotTitanHead1;
				}
			}
			else if (helmDamageIndex4 == 2)
			{
				magnetItemType = MagnetItemType.RobotTitanHead2;
			}
			else
			{
				if (helmDamageIndex4 >= 2)
				{
					goto IL_0701;
				}
				magnetItemType = MagnetItemType.RobotTitanHead1;
			}
			MagnetItemType mItemType = magnetItemType;
			theZombie.mHelmHealth = 0;
			theZombie.mHelmType = HelmType.None;
			theZombie.mHasHelm = false;
			theZombie.DetachRobotTitanHead(16u);
			string theTrackName = "anim_head";
			theZombie.GetTrackPosition(ref theTrackName, ref freeMagnetItem.mPosX, ref freeMagnetItem.mPosY);
			freeMagnetItem.mPosX -= AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD1.mWidth / 2;
			freeMagnetItem.mPosY -= AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_WHOLEHEAD1.mHeight / 2;
			freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(-10f, 10f) + 45f;
			freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-10f, 10f) + 15f;
			freeMagnetItem.mItemType = mItemType;
		}
		freeMagnetItem.mDestOffsetX *= Constants.S;
		freeMagnetItem.mDestOffsetY *= Constants.S;
		return;
		IL_0701:
		throw new NotImplementedException();
	}

	public void UpdateSunShroom()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mState == PlantState.SunshroomSmall)
		{
			if (mStateCountdown <= 0)
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_grow, ReanimLoopType.PlayOnceAndHold, 10, 12f);
				mState = PlantState.SunshroomGrowing;
				mApp.PlayFoley(FoleyType.Plantgrow);
			}
			UpdateProductionPlant();
		}
		else if (mState == PlantState.SunshroomGrowing)
		{
			if (reanimation.mLoopCount > 0)
			{
				float theAnimRate = TodCommon.RandRangeFloat(12f, 15f);
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_bigidle, ReanimLoopType.Loop, 10, theAnimRate);
				mState = PlantState.SunshroomBig;
			}
		}
		else
		{
			UpdateProductionPlant();
		}
	}

	public void UpdateBowling()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null && reanimation.TrackExists(Reanimation.ReanimTrackId__ground))
		{
			float num = reanimation.GetTrackVelocity(Reanimation.ReanimTrackId__ground);
			if (mSeedType == SeedType.GiantWallnut)
			{
				num *= 2f;
			}
			mX -= (int)num;
			if (mX > 900)
			{
				Die();
			}
		}
		int num2 = 3;
		if (mState == PlantState.BowlingUp)
		{
			mY -= num2;
		}
		else if (mState == PlantState.BowlingDown)
		{
			mY += num2;
		}
		int num3 = mBoard.GridToPixelY(0, mRow) - mY;
		if (num3 > 2 || num3 < -2)
		{
			return;
		}
		PlantState plantState = mState;
		if (plantState == PlantState.BowlingUp && mRow <= 0)
		{
			plantState = PlantState.BowlingDown;
		}
		else if (plantState == PlantState.BowlingDown && mRow >= 4)
		{
			plantState = PlantState.BowlingUp;
		}
		Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
		if (zombie != null)
		{
			int num4 = mX + mWidth / 2;
			int num5 = mY + mHeight / 2;
			if (mSeedType == SeedType.ExplodeONut)
			{
				mApp.PlayFoley(FoleyType.Cherrybomb);
				mApp.PlaySample(Resources.SOUND_BOWLINGIMPACT2);
				int theDamageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary) | 0x20;
				mBoard.KillAllZombiesInRadius(mRow, num4, num5, 90, 1, theBurn: true, theDamageRangeFlags);
				mApp.AddTodParticle(num4, num5, 400000, ParticleEffect.Powie);
				mBoard.ShakeBoard(3, -4);
				Die();
				return;
			}
			mApp.PlayFoley(FoleyType.Bowlingimpact);
			mBoard.ShakeBoard(1, -2);
			if (mSeedType == SeedType.GiantWallnut)
			{
				zombie.TakeDamage(1800, 0u);
			}
			else if (zombie.mShieldType == ShieldType.Door && mState != PlantState.Notready)
			{
				zombie.TakeDamage(1800, 0u);
			}
			else if (zombie.mShieldType != ShieldType.None)
			{
				zombie.TakeShieldDamage(400, 0u);
			}
			else if (zombie.mHelmType != HelmType.None)
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
				zombie.TakeDamage(1800, 0u);
			}
			if ((!mApp.IsFirstTimeAdventureMode() || mBoard.mLevel > 10) && mSeedType == SeedType.Wallnut)
			{
				mLaunchCounter++;
				if (mLaunchCounter == 2)
				{
					mApp.PlayFoley(FoleyType.SpawnSun);
					mBoard.AddCoin(num4, num5, CoinType.Silver, CoinMotion.Coin);
				}
				else if (mLaunchCounter == 3)
				{
					mApp.PlayFoley(FoleyType.SpawnSun);
					mBoard.AddCoin(num4 - 5, num5, CoinType.Silver, CoinMotion.Coin);
					mBoard.AddCoin(num4 + 5, num5, CoinType.Silver, CoinMotion.Coin);
				}
				else if (mLaunchCounter == 4)
				{
					mApp.PlayFoley(FoleyType.SpawnSun);
					mBoard.AddCoin(num4 - 10, num5, CoinType.Silver, CoinMotion.Coin);
					mBoard.AddCoin(num4, num5, CoinType.Silver, CoinMotion.Coin);
					mBoard.AddCoin(num4 + 10, num5, CoinType.Silver, CoinMotion.Coin);
				}
				else if (mLaunchCounter >= 5)
				{
					mApp.PlayFoley(FoleyType.SpawnSun);
					mBoard.AddCoin(num4, num5, CoinType.Gold, CoinMotion.Coin);
					mBoard.GrantAchievement(AchievementId.ACHIEVEMENT_ROLL_SOME_HEADS, show: true);
				}
			}
			if (mSeedType != SeedType.GiantWallnut)
			{
				if (mRow != 4 && mState != PlantState.BowlingDown)
				{
					if (mRow == 0 || mState == PlantState.BowlingUp)
					{
						plantState = PlantState.BowlingDown;
					}
					else
					{
						plantState = ((RandomNumbers.NextNumber(2) != 0) ? PlantState.BowlingUp : PlantState.BowlingDown);
					}
				}
				else
				{
					plantState = PlantState.BowlingUp;
				}
			}
		}
		switch (plantState)
		{
		case PlantState.BowlingUp:
			mState = PlantState.BowlingUp;
			mRow--;
			mRenderOrder = CalcRenderOrder();
			break;
		case PlantState.BowlingDown:
			mState = PlantState.BowlingDown;
			mRenderOrder = CalcRenderOrder();
			mRow++;
			break;
		}
	}

	public void AnimateFireShroom()
	{
		if (mSeedType != SeedType.FireShroom)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			Image iMAGE_REANIM_FIRESHROOM_1_BROKEN = AtlasResources.IMAGE_REANIM_FIRESHROOM_1_BROKEN1;
			Image iMAGE_REANIM_FIRESHROOM_ = AtlasResources.IMAGE_REANIM_FIRESHROOM_0;
			Image iMAGE_REANIM_FIRESHROOM_1_BROKEN2 = AtlasResources.IMAGE_REANIM_FIRESHROOM_1_BROKEN2;
			Image iMAGE_REANIM_FIRESHROOM_0_BROKEN = AtlasResources.IMAGE_REANIM_FIRESHROOM_0_BROKEN;
			string reanimTrackId_fire = GlobalMembersReanimIds.ReanimTrackId_fire;
			string reanimTrackId_fire_broken = GlobalMembersReanimIds.ReanimTrackId_fire_broken;
			int num = mX + 40;
			int num2 = mY + 10;
			if (mPlantHealth < mPlantMaxHealth / 3)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireShroom_sleepinghead, iMAGE_REANIM_FIRESHROOM_1_BROKEN2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireshroom_head1, iMAGE_REANIM_FIRESHROOM_1_BROKEN2);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireshroom_body, iMAGE_REANIM_FIRESHROOM_0_BROKEN);
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_fire, -1);
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_fire_broken, 0);
			}
			else if (mPlantHealth < mPlantMaxHealth * 2 / 3)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireShroom_sleepinghead, iMAGE_REANIM_FIRESHROOM_1_BROKEN);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireshroom_head1, iMAGE_REANIM_FIRESHROOM_1_BROKEN);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireshroom_body, iMAGE_REANIM_FIRESHROOM_);
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_fire, 0);
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_fire_broken, -1);
			}
			else
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireShroom_sleepinghead, null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireshroom_head1, null);
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_fireshroom_body, null);
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_fire, 0);
				reanimation.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_fire_broken, -1);
			}
		}
	}

	public void AnimatePumpkin()
	{
		if (mBodyReanimID == null || !mBodyReanimID.mActive || !IsInPlay())
		{
			return;
		}
		int num = 0;
		num = ((mPlantHealth < mPlantMaxHealth / 3) ? 2 : ((mPlantHealth < mPlantMaxHealth * 2 / 3) ? 1 : 0));
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (reanimation.IsAnimPlaying(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
		{
			mRenderOrder = CalcRenderOrder();
			reanimation.PlayReanim("anim_special", ReanimLoopType.PlayOnceAndHold, 0, 30f);
			mState = PlantState.PumpkinDeploying;
			mStateCountdown = 40;
		}
		if (reanimation.mLoopType == ReanimLoopType.PlayOnce)
		{
			reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
		}
		if (mState == PlantState.PumpkinDeploying && mStateCountdown <= 0)
		{
			mAttackingReanimID = mApp.AddReanimation(0f, PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow), mRenderOrder + 1 - 200, ReanimationType.PumpkinUpper);
			mAttackingReanimID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.PlayOnceAndHold, 0, 30f);
			mAttackingReanimID.mIsAttachment = true;
			mAttackingReanimID.mFilterEffect = reanimation.mFilterEffect;
			mLightReanimID = mApp.AddReanimation(0f, PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow), mRenderOrder - 10 - 200, ReanimationType.PumpkinLower);
			mLightReanimID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.PlayOnceAndHold, 0, 30f);
			mLightReanimID.mIsAttachment = true;
			mLightReanimID.mFilterEffect = reanimation.mFilterEffect;
			mState = PlantState.Ready;
		}
		if (mState == PlantState.Notready)
		{
			mRenderOrder = CalcRenderOrder();
			if (mLightReanimID != null)
			{
				mLightReanimID.mOverlayMatrix.mMatrix.M41 = 0f;
				mLightReanimID.mOverlayMatrix.mMatrix.M42 = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
				mLightReanimID.mRenderOrder = mRenderOrder - 10 - 200;
				mLightReanimID.mIsAttachment = true;
			}
			if (mAttackingReanimID != null)
			{
				mAttackingReanimID.mOverlayMatrix.mMatrix.M41 = 0f;
				mAttackingReanimID.mOverlayMatrix.mMatrix.M42 = PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
				mAttackingReanimID.mRenderOrder = mRenderOrder + 1 - 200;
				mAttackingReanimID.mIsAttachment = true;
			}
			mState = PlantState.Ready;
		}
		if (mLightReanimID == null)
		{
			return;
		}
		if (mLightReanimID.mLoopType == ReanimLoopType.PlayOnce)
		{
			mLightReanimID.mLoopType = ReanimLoopType.PlayOnceAndHold;
		}
		if (mAttackingReanimID.mLoopType == ReanimLoopType.PlayOnce)
		{
			mAttackingReanimID.mLoopType = ReanimLoopType.PlayOnceAndHold;
		}
		mLightReanimID.Update();
		mAttackingReanimID.Update();
		if (mLightReanimID.mLoopCount > 0)
		{
			if (mState == PlantState.Doingspecial)
			{
				Die();
				return;
			}
			mLightReanimID.PlayReanim($"anim_idle{num + 1}", ReanimLoopType.Loop, 0, 30f);
			mAttackingReanimID.PlayReanim($"anim_idle{num + 1}", ReanimLoopType.Loop, 0, 30f);
		}
		else if (mLightReanimID.IsAnimPlaying("anim_idle1") && num > 0)
		{
			mApp.PlaySample(Resources.SOUND_PUMPKIN_SHIELDBROKEN);
			mLightReanimID.PlayReanim("anim_idle1_idle2", ReanimLoopType.PlayOnceAndHold, 0, 30f);
			mAttackingReanimID.PlayReanim("anim_idle1_idle2", ReanimLoopType.PlayOnceAndHold, 0, 30f);
		}
		else if (mLightReanimID.IsAnimPlaying("anim_idle2") && num > 1)
		{
			mApp.PlaySample(Resources.SOUND_PUMPKIN_SHIELDBROKEN);
			mLightReanimID.PlayReanim("anim_idle2_idle3", ReanimLoopType.PlayOnceAndHold, 0, 30f);
			mAttackingReanimID.PlayReanim("anim_idle2_idle3", ReanimLoopType.PlayOnceAndHold, 0, 30f);
		}
	}

	public void UpdateBlover()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (reanimation.mLoopCount > 0 && reanimation.mLoopType != ReanimLoopType.Loop)
		{
			reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_loop);
			reanimation.mLoopType = ReanimLoopType.Loop;
		}
		if (mState != PlantState.Doingspecial && mDoSpecialCountdown == 0)
		{
			DoSpecial();
		}
	}

	public void UpdateCactus()
	{
		if (mShootingCounter > 0)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mState == PlantState.CactusRising)
		{
			if (reanimation.mLoopCount > 0)
			{
				mState = PlantState.CactusHigh;
				if (mApp.IsIZombieLevel())
				{
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idlehigh, ReanimLoopType.Loop, 20, reanimation.mDefinition.mFPS * 2f);
				}
				else
				{
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idlehigh, ReanimLoopType.Loop, 20, reanimation.mDefinition.mFPS * 2f);
				}
				if (mApp.IsIZombieLevel())
				{
					reanimation.mAnimRate = 0f;
				}
				mLaunchCounter = 1;
			}
			return;
		}
		if (mState == PlantState.CactusHigh)
		{
			if (FindTargetZombie(mRow, PlantWeapon.Primary) == null)
			{
				mState = PlantState.CactusLowering;
				if (mApp.IsIZombieLevel())
				{
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_lower, ReanimLoopType.PlayOnceAndHold, 20, reanimation.mDefinition.mFPS);
				}
				else
				{
					PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_lower, ReanimLoopType.PlayOnceAndHold, 20, reanimation.mDefinition.mFPS * 2f);
				}
			}
			return;
		}
		if (mState == PlantState.CactusLowering)
		{
			if (reanimation.mLoopCount > 0)
			{
				mState = PlantState.CactusLow;
				PlayIdleAnim(reanimation.mDefinition.mFPS);
			}
			return;
		}
		Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
		if (zombie != null)
		{
			mState = PlantState.CactusRising;
			if (mApp.IsIZombieLevel())
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_rise, ReanimLoopType.PlayOnceAndHold, 20, reanimation.mDefinition.mFPS);
			}
			else
			{
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_rise, ReanimLoopType.PlayOnceAndHold, 20, reanimation.mDefinition.mFPS * 2f);
			}
			mApp.PlayFoley(FoleyType.Plantgrow);
		}
	}

	public void StarFruitFire()
	{
		mApp.PlayFoley(FoleyType.Throw);
		for (int i = 0; i < 5; i++)
		{
			int theX = mX + 25;
			int theY = mY + 25;
			Projectile projectile = mBoard.AddProjectile(theX, theY, mRenderOrder + -1, mRow, ProjectileType.Star);
			projectile.mDamageRangeFlags = GetDamageRangeFlags(PlantWeapon.Primary);
			projectile.mMotionType = ProjectileMotion.Star;
			float mVelX = (float)Math.Cos(TodCommon.DegToRad(30f)) * 3.33f;
			float num = (float)Math.Sin(TodCommon.DegToRad(30f)) * 3.33f;
			switch (i)
			{
			case 0:
				projectile.mVelX = -3.33f;
				projectile.mVelY = 0f;
				break;
			case 1:
				projectile.mVelX = 0f;
				projectile.mVelY = 3.33f;
				break;
			case 2:
				projectile.mVelX = 0f;
				projectile.mVelY = -3.33f;
				break;
			case 3:
				projectile.mVelX = mVelX;
				projectile.mVelY = num;
				break;
			case 4:
				projectile.mVelX = mVelX;
				projectile.mVelY = 0f - num;
				break;
			default:
				Debug.ASSERT(value: false);
				break;
			}
		}
	}

	public void UpdateTanglekelp()
	{
		if (mState != PlantState.TanglekelpGrabbing)
		{
			Zombie zombie = FindTargetZombie(mRow, PlantWeapon.Primary);
			if (zombie != null)
			{
				mApp.PlayFoley(FoleyType.Floop);
				mState = PlantState.TanglekelpGrabbing;
				mStateCountdown = 99;
				zombie.PoolSplash(theInToPoolSound: false);
				float num = -13f;
				float num2 = 15f;
				if (zombie.mZombieType == ZombieType.Snorkel)
				{
					num = -43f;
					num2 = 55f;
				}
				if (zombie.mZombiePhase == ZombiePhase.DolphinRiding)
				{
					num = -20f;
					num2 = 37f;
				}
				Reanimation reanimation = zombie.AddAttachedReanim((int)num, (int)num2, ReanimationType.Tanglekelp);
				zombie.mAttachmentID.mUsesClipping = true;
				if (reanimation != null)
				{
					reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_grab);
					reanimation.mAnimRate = 24f;
					reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
				}
				mTargetZombieID = mBoard.ZombieGetID(zombie);
				mTargetZombieID.mUsesClipping = true;
				mTargetZombieID.draggedByTangleKelp = true;
			}
			return;
		}
		if (mStateCountdown == 51)
		{
			Zombie zombie2 = mBoard.ZombieTryToGet(mTargetZombieID);
			if (zombie2 != null)
			{
				zombie2.DragUnder();
				zombie2.PoolSplash(theInToPoolSound: false);
			}
		}
		if (mStateCountdown == 21)
		{
			int aRenderOrder = Board.MakeRenderOrder(RenderLayer.Particle, mRow, 0);
			Reanimation reanimation2 = mApp.AddReanimation(mX - 23, mY + 7, aRenderOrder, ReanimationType.Splash);
			reanimation2.OverrideScale(1.3f, 1.3f);
			mApp.AddTodParticle(mX + 31, mY + 64, aRenderOrder, ParticleEffect.PlantingPool);
			mApp.PlayFoley(FoleyType.ZombieEnteringWater);
		}
		if (mStateCountdown <= 0)
		{
			Die();
			mBoard.ZombieTryToGet(mTargetZombieID)?.DieWithLoot();
		}
	}

	public Reanimation AttachBlinkAnim(Reanimation theReanimBody)
	{
		PlantDefinition plantDefinition = GetPlantDefinition(mSeedType);
		LawnApp lawnApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		Reanimation theAttachReanim = theReanimBody;
		string text = GlobalMembersReanimIds.ReanimTrackId_anim_blink;
		string theTrackName = Reanimation.ReanimTrackIdEmpty;
		if (mSeedType == SeedType.Wallnut || mSeedType == SeedType.Tallnut || mSeedType == SeedType.ExplodeONut || mSeedType == SeedType.GiantWallnut)
		{
			int num = RandomNumbers.NextNumber(10);
			if (num < 1 && theReanimBody.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_blink_twitch))
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink_twitch;
			}
			else
			{
				text = ((num >= 7) ? GlobalMembersReanimIds.ReanimTrackId_anim_blink_thrice : GlobalMembersReanimIds.ReanimTrackId_anim_blink_twice);
			}
		}
		else if (mSeedType == SeedType.Threepeater)
		{
			switch (RandomNumbers.NextNumber(3))
			{
			case 0:
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink1;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face1;
				ReanimatorTrackInstance trackInstanceByName3 = theReanimBody.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
				theAttachReanim = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName3.mAttachmentID);
				break;
			}
			case 1:
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink2;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face2;
				ReanimatorTrackInstance trackInstanceByName2 = theReanimBody.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head2);
				theAttachReanim = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName2.mAttachmentID);
				break;
			}
			default:
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink3;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face3;
				ReanimatorTrackInstance trackInstanceByName = theReanimBody.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head3);
				theAttachReanim = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName.mAttachmentID);
				break;
			}
			}
		}
		else if (mSeedType == SeedType.Splitpea)
		{
			if (RandomNumbers.NextNumber(2) == 0)
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face;
				theAttachReanim = mApp.ReanimationTryToGet(mHeadReanimID);
			}
			else
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_splitpea_blink;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_splitpea_head;
				theAttachReanim = mApp.ReanimationTryToGet(mHeadReanimID2);
			}
		}
		else if (mSeedType == SeedType.Twinsunflower)
		{
			if (RandomNumbers.NextNumber(2) == 0)
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face;
			}
			else
			{
				text = GlobalMembersReanimIds.ReanimTrackId_anim_blink2;
				theTrackName = GlobalMembersReanimIds.ReanimTrackId_anim_face2;
			}
		}
		else if (mSeedType == SeedType.Peashooter || mSeedType == SeedType.Snowpea || mSeedType == SeedType.Repeater || mSeedType == SeedType.Leftpeater || mSeedType == SeedType.Gatlingpea)
		{
			if (theReanimBody.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_stem))
			{
				ReanimatorTrackInstance trackInstanceByName4 = theReanimBody.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_stem);
				theAttachReanim = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName4.mAttachmentID);
			}
			else if (theReanimBody.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
			{
				ReanimatorTrackInstance trackInstanceByName5 = theReanimBody.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
				theAttachReanim = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName5.mAttachmentID);
			}
		}
		if (theAttachReanim == null)
		{
			return null;
		}
		if (!theReanimBody.TrackExists(text))
		{
			return null;
		}
		Reanimation reanimation = lawnApp.mEffectSystem.mReanimationHolder.AllocReanimation(0f, 0f, 0, plantDefinition.mReanimationType);
		reanimation.SetFramesForLayer(text);
		reanimation.mLoopType = ReanimLoopType.PlayOnceFullLastFrameAndHold;
		reanimation.mAnimRate = 15f;
		reanimation.mColorOverride.CopyFrom(theReanimBody.mColorOverride);
		if (theAttachReanim.TrackExists(theTrackName))
		{
			reanimation.AttachToAnotherReanimation(ref theAttachReanim, theTrackName);
		}
		else if (theAttachReanim.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_face))
		{
			reanimation.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_face);
		}
		else if (theAttachReanim.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_idle))
		{
			reanimation.AttachToAnotherReanimation(ref theAttachReanim, GlobalMembersReanimIds.ReanimTrackId_anim_idle);
		}
		reanimation.mFilterEffect = theReanimBody.mFilterEffect;
		theAttachReanim.PropogateColorToAttachments();
		return reanimation;
	}

	public void UpdateReanimColor()
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
		SeedType seedTypeInCursor = mBoard.GetSeedTypeInCursor();
		bool flag = false;
		if (IsPartOfUpgradableTo(seedTypeInCursor) && mBoard.CanPlantAt(mPlantCol, mRow, seedTypeInCursor) == PlantingReason.Ok && !mGloveGrabbed)
		{
			SexyColor flashingColor = TodCommon.GetFlashingColor(mBoard.mMainCounter, 90);
			if (flashingColor != reanimation.mColorOverride)
			{
				flag = true;
				reanimation.mColorOverride = flashingColor;
			}
		}
		else if (seedTypeInCursor == SeedType.Cobcannon && mSeedType == SeedType.Kernelpult && mBoard.CanPlantAt(mPlantCol - 1, mRow, seedTypeInCursor) == PlantingReason.Ok)
		{
			SexyColor flashingColor2 = TodCommon.GetFlashingColor(mBoard.mMainCounter, 90);
			if (flashingColor2 != reanimation.mColorOverride)
			{
				flag = true;
				reanimation.mColorOverride = flashingColor2;
			}
		}
		else if (mSeedType == SeedType.ExplodeONut)
		{
			SexyColor sexyColor = new SexyColor(255, 64, 64);
			if (sexyColor != reanimation.mColorOverride)
			{
				flag = true;
				reanimation.mColorOverride = sexyColor;
			}
		}
		else if (mSeedType == SeedType.IceWallnut)
		{
			SexyColor sexyColor2 = new SexyColor(64, 160, 255);
			if (sexyColor2 != reanimation.mColorOverride)
			{
				flag = true;
				reanimation.mColorOverride = sexyColor2;
			}
		}
		else if (mState == PlantState.UmbrellaDeathReflecting)
		{
			reanimation.mColorOverride = new SexyColor(255, 255, 255, mStateCountdown * 10, premultiply: false);
		}
		else
		{
			SexyColor white = SexyColor.White;
			if (white != reanimation.mColorOverride)
			{
				flag = true;
				reanimation.mColorOverride = white;
			}
		}
		if (mHighlighted)
		{
			SexyColor sexyColor3 = new SexyColor(255, 255, 255, 196);
			if (sexyColor3 != reanimation.mExtraAdditiveColor)
			{
				flag = true;
				reanimation.mExtraAdditiveColor = sexyColor3;
			}
			if (!reanimation.mEnableExtraAdditiveDraw)
			{
				flag = true;
				reanimation.mEnableExtraAdditiveDraw = true;
			}
			if (mImitaterType == SeedType.Imitater)
			{
				sexyColor3 = new SexyColor(255, 255, 255, 92);
				if (sexyColor3 != reanimation.mExtraAdditiveColor)
				{
					flag = true;
					reanimation.mExtraAdditiveColor = sexyColor3;
				}
			}
		}
		else if (mBeghouledFlashCountdown > 0)
		{
			int theAlpha = TodCommon.TodAnimateCurve(50, 0, mBeghouledFlashCountdown % 50, 0, 128, TodCurves.Bounce);
			SexyColor sexyColor4 = new SexyColor(255, 255, 255, theAlpha);
			if (sexyColor4 != reanimation.mExtraAdditiveColor)
			{
				flag = true;
				reanimation.mExtraAdditiveColor = sexyColor4;
			}
			if (!reanimation.mEnableExtraAdditiveDraw)
			{
				flag = true;
				reanimation.mEnableExtraAdditiveDraw = true;
			}
		}
		else if (mEatenFlashCountdown > 0)
		{
			int maxNum = 255;
			if (mImitaterType == SeedType.Imitater)
			{
				maxNum = 128;
			}
			int num = TodCommon.ClampInt(mEatenFlashCountdown * 3, 0, maxNum);
			SexyColor sexyColor5 = new SexyColor(num, num, num);
			if (sexyColor5 != reanimation.mExtraAdditiveColor)
			{
				flag = true;
				reanimation.mExtraAdditiveColor = sexyColor5;
			}
			if (!reanimation.mEnableExtraAdditiveDraw)
			{
				flag = true;
				reanimation.mEnableExtraAdditiveDraw = true;
			}
		}
		else if (reanimation.mEnableExtraAdditiveDraw)
		{
			flag = true;
			reanimation.mEnableExtraAdditiveDraw = false;
		}
		if (mBeghouledFlashCountdown > 0)
		{
			int theAlpha2 = TodCommon.TodAnimateCurve(50, 0, mBeghouledFlashCountdown % 50, 0, 128, TodCurves.Bounce);
			SexyColor sexyColor6 = new SexyColor(255, 255, 255, theAlpha2);
			if (sexyColor6 != reanimation.mExtraOverlayColor)
			{
				flag = true;
				reanimation.mExtraOverlayColor = sexyColor6;
			}
		}
		else if (reanimation.mEnableExtraOverlayDraw)
		{
			flag = true;
			reanimation.mEnableExtraOverlayDraw = false;
		}
		if (flag && mSeedType == SeedType.Pumpkinshell && mAttackingReanimID != null && mLightReanimID != null)
		{
			mAttackingReanimID.mExtraAdditiveColor = reanimation.mExtraAdditiveColor;
			mAttackingReanimID.mEnableExtraAdditiveDraw = reanimation.mEnableExtraAdditiveDraw;
			mAttackingReanimID.mColorOverride = reanimation.mColorOverride;
			mLightReanimID.mExtraAdditiveColor = reanimation.mExtraAdditiveColor;
			mLightReanimID.mEnableExtraAdditiveDraw = reanimation.mEnableExtraAdditiveDraw;
			mLightReanimID.mColorOverride = reanimation.mColorOverride;
		}
		if (flag)
		{
			reanimation.PropogateColorToAttachments();
		}
	}

	public bool IsUpgradableTo(SeedType aUpdatedType)
	{
		if (aUpdatedType == SeedType.Gatlingpea && mSeedType == SeedType.Repeater)
		{
			return true;
		}
		if (aUpdatedType == SeedType.Wintermelon && mSeedType == SeedType.Melonpult)
		{
			return true;
		}
		if (aUpdatedType == SeedType.Twinsunflower && mSeedType == SeedType.Sunflower)
		{
			return true;
		}
		if (aUpdatedType == SeedType.Spikerock && mSeedType == SeedType.Spikeweed)
		{
			return true;
		}
		if (aUpdatedType == SeedType.Cobcannon && mSeedType == SeedType.Kernelpult)
		{
			if (mBoard.IsValidCobCannonSpot(mPlantCol, mRow))
			{
				return true;
			}
		}
		else
		{
			if (aUpdatedType == SeedType.GoldMagnet && mSeedType == SeedType.Magnetshroom)
			{
				return true;
			}
			if (aUpdatedType == SeedType.Gloomshroom && mSeedType == SeedType.Fumeshroom)
			{
				return true;
			}
			if (aUpdatedType == SeedType.SuperChomper && mSeedType == SeedType.Chomper)
			{
				return true;
			}
			if (aUpdatedType == SeedType.Cattail && mSeedType == SeedType.Lilypad)
			{
				Plant topPlantAt = mBoard.GetTopPlantAt(mPlantCol, mRow, TopPlant.OnlyNormalPosition);
				if (topPlantAt == null || topPlantAt.mSeedType != SeedType.Cattail)
				{
					return true;
				}
			}
		}
		if (GetValidFusion(mSeedType, aUpdatedType) != SeedType.None)
		{
			return true;
		}
		return false;
	}

	public static SeedType GetValidFusion(SeedType theBaseType, SeedType theCoverType)
	{
		if (GlobalStaticVars.gLawnApp.mBoard != null && !GlobalStaticVars.gLawnApp.mBoard.HasFusion())
		{
			return SeedType.None;
		}
		switch (theBaseType)
		{
		case SeedType.Peashooter:
			switch (theCoverType)
			{
			case SeedType.Sunflower:
				return SeedType.SunflowerPea;
			case SeedType.Peashooter:
				return SeedType.Repeater;
			case SeedType.Wallnut:
				return SeedType.PeaWallnut;
			case SeedType.Iceshroom:
				return SeedType.Snowpea;
			}
			break;
		case SeedType.Sunflower:
			switch (theCoverType)
			{
			case SeedType.Sunflower:
				return SeedType.Twinsunflower;
			case SeedType.Peashooter:
				return SeedType.SunflowerPea;
			case SeedType.Wallnut:
				return SeedType.SunflowerWallnut;
			}
			break;
		case SeedType.Iceshroom:
			switch (theCoverType)
			{
			case SeedType.Peashooter:
				return SeedType.Snowpea;
			case SeedType.Wallnut:
				return SeedType.IceWallnut;
			}
			break;
		case SeedType.Wallnut:
			switch (theCoverType)
			{
			case SeedType.Wallnut:
				return SeedType.GiantWallnut;
			case SeedType.Cherrybomb:
				return SeedType.ExplodeONut;
			case SeedType.Peashooter:
				return SeedType.PeaWallnut;
			case SeedType.Sunflower:
				return SeedType.SunflowerWallnut;
			case SeedType.Iceshroom:
				return SeedType.IceWallnut;
			}
			break;
		case SeedType.Cherrybomb:
			if (theCoverType == SeedType.Wallnut)
			{
				return SeedType.ExplodeONut;
			}
			break;
		case SeedType.Repeater:
			if (theCoverType == SeedType.Repeater)
			{
				return SeedType.Gatlingpea;
			}
			break;
		case SeedType.Cattail:
			if (theCoverType == SeedType.Hypnoshroom)
			{
				return SeedType.HypnoCattail;
			}
			break;
		}
		return SeedType.None;
	}

	public bool IsPartOfUpgradableTo(SeedType aUpdatedType)
	{
		if (aUpdatedType == SeedType.Cobcannon && mSeedType == SeedType.Kernelpult)
		{
			if (!mBoard.IsValidCobCannonSpot(mPlantCol, mRow))
			{
				return mBoard.IsValidCobCannonSpot(mPlantCol - 1, mRow);
			}
			return true;
		}
		return IsUpgradableTo(aUpdatedType);
	}

	public void UpdateCobCannon()
	{
		if (mState == PlantState.CobcannonArming)
		{
			if (mStateCountdown <= 0)
			{
				mState = PlantState.CobcannonLoading;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_charge, ReanimLoopType.PlayOnceAndHold, 20, 12f);
			}
		}
		else if (mState == PlantState.CobcannonLoading)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.ShouldTriggerTimedEvent(0.5f))
			{
				mApp.PlayFoley(FoleyType.Shoop);
			}
			if (reanimation.mLoopCount > 0)
			{
				mState = PlantState.CobcannonReady;
				PlayIdleAnim(12f);
			}
		}
		else if (mState == PlantState.CobcannonReady)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			ReanimatorTrackInstance trackInstanceByName = reanimation2.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_cobcannon_cob);
			trackInstanceByName.mTrackColor = TodCommon.GetFlashingColor(mBoard.mMainCounter, 75);
		}
		else if (mState == PlantState.CobcannonFiring)
		{
			Reanimation reanimation3 = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation3.ShouldTriggerTimedEvent(0.48f))
			{
				mApp.PlayFoley(FoleyType.Coblaunch);
			}
		}
	}

	public void CobCannonFire(int theTargetX, int theTargetY)
	{
		SeedType seedType = mSeedType;
		bool flag = (uint)(seedType - 51) <= 1u;
		if (flag && (!mBoard.IsPlantSkillEnabled(mSeedType) || ((mSeedType == SeedType.Agave) ? mBoard.mAgavePowerfulCountdown : mBoard.mEndoflamePowerfulCountdown) > 0))
		{
			if (AgaveSkillCanCancel())
			{
				mState = PlantState.AgavePowerfulTendToCanceling;
			}
		}
		else if (mSeedType == SeedType.Agave)
		{
			if (AgaveSkillCanCancel())
			{
				if (mBoard.HasConveyorBeltSeedBank() || mBoard.TakeSunMoney(175))
				{
					mBoard.mAgavePowerfulCountdown += 6000;
					mState = PlantState.AgavePowerfulAttacking;
					mStateCountdown = 600;
					PlayBodyReanim("anim_atk2_1", ReanimLoopType.PlayOnceAndHold, 20, 30f);
					mApp.ReanimationTryToGet(mLightReanimID)?.PlayReanim("anim_atk2_1", ReanimLoopType.PlayOnceAndHold, 0, 60f);
					mApp.PlaySample(Resources.SOUND_AGAVE_POWERFUL);
					mTargetX = theTargetX;
					mTargetY = theTargetY;
				}
				else
				{
					mState = PlantState.AgavePowerfulTendToCanceling;
				}
			}
		}
		else if (mSeedType == SeedType.Endoflame)
		{
			if (AgaveSkillCanCancel())
			{
				if (mBoard.HasConveyorBeltSeedBank() || mBoard.TakeSunMoney(225))
				{
					mBoard.mEndoflamePowerfulCountdown += 3000;
					mState = PlantState.AgavePowerfulAttacking;
					mStateCountdown = 250;
					PlayBodyReanim("anim_shooting2", ReanimLoopType.PlayOnceAndHold, 20, 30f);
					PlantVoice.Play(SeedType.Endoflame, SeedType.None, PlantVoice.VoiceType.Attack);
					mTargetX = theTargetX;
					mTargetY = theTargetY;
				}
				else
				{
					mState = PlantState.AgavePowerfulTendToCanceling;
				}
			}
		}
		else
		{
			Debug.ASSERT(mState == PlantState.CobcannonReady);
			mState = PlantState.CobcannonFiring;
			mShootingCounter = 206;
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_shooting, ReanimLoopType.PlayOnceAndHold, 20, 12f);
			mTargetX = theTargetX - 47;
			mTargetY = theTargetY;
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_cobcannon_cob);
			trackInstanceByName.mTrackColor = SexyColor.White;
		}
	}

	public void UpdateGoldMagnetShroom()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		bool flag = false;
		for (int i = 0; i < 5; i++)
		{
			MagnetItem magnetItem = mMagnetItems[i];
			if (magnetItem.mItemType == MagnetItemType.None)
			{
				continue;
			}
			SexyVector2 sexyVector = new SexyVector2((float)mX + magnetItem.mDestOffsetX - magnetItem.mPosX, (float)mY + magnetItem.mDestOffsetY - magnetItem.mPosY);
			float num = sexyVector.Magnitude();
			if (num < 20f)
			{
				CoinType theType = CoinType.Almanac;
				if (magnetItem.mItemType == MagnetItemType.SilverCoin)
				{
					theType = CoinType.Silver;
				}
				else if (magnetItem.mItemType == MagnetItemType.GoldCoin)
				{
					theType = CoinType.Gold;
				}
				else if (magnetItem.mItemType == MagnetItemType.Diamond)
				{
					theType = CoinType.Diamond;
				}
				else
				{
					Debug.ASSERT(value: false);
				}
				int coinValue = Coin.GetCoinValue(theType);
				mApp.mPlayerInfo.AddCoins(coinValue);
				mBoard.mCoinsCollected += coinValue;
				mApp.PlayFoley(FoleyType.Coin);
				magnetItem.mItemType = MagnetItemType.None;
			}
			else
			{
				float num2 = TodCommon.TodAnimateCurveFloatTime(300f, 0f, num, 0.02f, 0.05f, TodCurves.Linear);
				magnetItem.mPosX += sexyVector.x * num2;
				magnetItem.mPosY += sexyVector.y * num2;
				flag = true;
			}
		}
		if (mState == PlantState.MagnetshroomCharging)
		{
			if (mStateCountdown <= 0)
			{
				mState = PlantState.Ready;
			}
		}
		else if (mState == PlantState.MagnetshroomSucking)
		{
			if (reanimation.ShouldTriggerTimedEvent(0.4f))
			{
				mApp.PlayFoley(FoleyType.Magnetshroom);
				GoldMagnetFindTargets();
			}
			if (reanimation.mLoopCount > 0 && !flag)
			{
				PlayIdleAnim(14f);
				mState = PlantState.MagnetshroomCharging;
				mStateCountdown = TodCommon.RandRangeInt(200, 300);
			}
		}
		else if (!IsAGoldMagnetAboutToSuck() && RandomNumbers.NextNumber(50) == 0)
		{
			Coin coin = FindGoldMagnetTarget();
			if (coin != null)
			{
				mBoard.ShowCoinBank();
				mState = PlantState.MagnetshroomSucking;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_attract, ReanimLoopType.PlayOnceAndHold, 20, 12f);
			}
		}
	}

	public bool IsOnBoard()
	{
		if (!mIsOnBoard)
		{
			return false;
		}
		Debug.ASSERT(mBoard != null);
		return true;
	}

	public void RemoveEffects()
	{
		mApp.RemoveParticle(mParticleID);
		mParticleID = null;
		mApp.RemoveReanimation(ref mBodyReanimID);
		mApp.RemoveReanimation(ref mHeadReanimID);
		mApp.RemoveReanimation(ref mHeadReanimID2);
		mApp.RemoveReanimation(ref mHeadReanimID3);
		mApp.RemoveReanimation(ref mLightReanimID);
		mApp.RemoveReanimation(ref mBlinkReanimID);
		mApp.RemoveReanimation(ref mSleepingReanimID);
		mApp.RemoveReanimation(ref mAttackingReanimID);
		mApp.RemoveReanimation(ref mSealedReanimID);
	}

	public void UpdateCoffeeBean()
	{
		if (mState == PlantState.Doingspecial)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				Die();
			}
		}
	}

	public void UpdateUmbrella()
	{
		if (mState == PlantState.UmbrellaTriggered)
		{
			if (mStateCountdown <= 0)
			{
				mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, mRow, 0);
				mState = PlantState.UmbrellaReflecting;
			}
		}
		else if (mState == PlantState.UmbrellaDeathTriggered)
		{
			if (mStateCountdown <= 0)
			{
				mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, mRow, 0);
				mState = PlantState.UmbrellaDeathReflecting;
				mStateCountdown = 25;
			}
		}
		else if (mState == PlantState.UmbrellaReflecting)
		{
			Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
			if (reanimation.mLoopCount > 0)
			{
				PlayIdleAnim(0f);
				mState = PlantState.Notready;
				mRenderOrder = CalcRenderOrder();
			}
		}
		else if (mState == PlantState.UmbrellaDeathReflecting)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mBodyReanimID);
			if (mStateCountdown <= 0)
			{
				Die();
			}
		}
	}

	public void EndBlink()
	{
		if (mBlinkReanimID != null)
		{
			mApp.RemoveReanimation(ref mBlinkReanimID);
			mBlinkReanimID = null;
			mApp.ReanimationTryToGet(mBodyReanimID)?.AssignRenderGroupToPrefix("anim_eye", 0);
		}
	}

	public void AnimateGarlic()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mPlantHealth < mPlantMaxHealth / 3)
		{
			if (!reanimation.IsAnimPlaying("anim_idle3"))
			{
				reanimation.PlayReanim("anim_idle3", ReanimLoopType.Loop, 0, 0f);
			}
		}
		else if (mPlantHealth < mPlantMaxHealth * 2 / 3 && !reanimation.IsAnimPlaying("anim_idle2"))
		{
			reanimation.PlayReanim("anim_idle2", ReanimLoopType.Loop, 0, 0f);
		}
	}

	public Coin FindGoldMagnetTarget()
	{
		Coin coin = null;
		float num = 0f;
		Coin theCoin = null;
		while (mBoard.IterateCoins(ref theCoin))
		{
			if ((theCoin.mType == CoinType.Silver || theCoin.mType == CoinType.Gold || theCoin.mType == CoinType.Diamond) && theCoin.mCoinMotion != CoinMotion.FromPresent && !theCoin.mIsBeingCollected && theCoin.mCoinAge >= 50)
			{
				float num2 = TodCommon.Distance2D((float)mX + 40f, (float)mY + 40f, theCoin.mPosX + (float)(theCoin.mWidth / 2), theCoin.mPosY + (float)(theCoin.mHeight / 2));
				if (coin == null || num2 < num)
				{
					coin = theCoin;
					num = num2;
				}
			}
		}
		return coin;
	}

	public void SpikeweedAttack()
	{
		Debug.ASSERT(IsSpiky());
		if (mState == PlantState.SpikeweedAttacking)
		{
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (mSeedType == SeedType.Spikeweed && mState != PlantState.Doingspecial)
		{
			if (!reanimation.IsTrackShowing("anim_atk1"))
			{
				PlayBodyReanim("anim_atk1", ReanimLoopType.PlayOnceAndHold, 0, 30f);
			}
			else
			{
				PlayBodyReanim("anim_atk2", ReanimLoopType.PlayOnceAndHold, 0, 30f);
			}
			mApp.PlaySample(Resources.SOUND_THROW);
			mState = PlantState.SpikeweedAttacking;
			mStateCountdown = 99;
			mRenderOrder += 5000;
		}
		else
		{
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_attack + GetStateTrackIdSuffix(), ReanimLoopType.PlayOnceAndHold, 20, 30f);
			reanimation.mAnimTime = 0.17f;
			mApp.PlaySample(Resources.SOUND_SPIKEROCK_ATTACK);
			mState = PlantState.SpikeweedAttacking;
			mStateCountdown = 99;
		}
	}

	public void ImitaterMorph()
	{
		Die();
		Plant plant = null;
		bool flag = true;
		if (mSeedType == SeedType.Imitater)
		{
			plant = mBoard.AddPlant(mPlantCol, mRow, mImitaterType, SeedType.Imitater);
		}
		else if (mSeedType == SeedType.ImitaterRandomPlant)
		{
			SeedType seedType = SeedType.None;
			if (mApp.mGameMode == GameMode.ImitaterRandom)
			{
				TodWeightedArray[] array = new TodWeightedArray[53];
				int num = 0;
				for (SeedType seedType2 = SeedType.Peashooter; seedType2 < SeedType.Imitater; seedType2++)
				{
					int mWeight = 1;
					if (seedType2 != SeedType.Tanglekelp && seedType2 != SeedType.Flowerpot && mBoard.CanPlantAt(mPlantCol, mRow, seedType2, aIsMovePlant: true) == PlantingReason.Ok)
					{
						array[num] = TodWeightedArray.GetNewTodWeightedArray();
						array[num].mWeight = mWeight;
						array[num].mItem = seedType2;
						num++;
					}
				}
				seedType = ((num > 0) ? ((SeedType)TodCommon.TodPickFromWeightedArray(array, num)) : SeedType.ImitaterRandomPlant);
				TodWeightedArray[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.PrepareForReuse();
				}
			}
			else
			{
				seedType = (SeedType)RandomNumbers.NextNumber(53);
			}
			plant = mBoard.AddPlant(mPlantCol, mRow, seedType, SeedType.None);
			flag = false;
			if (plant == null)
			{
				return;
			}
		}
		else
		{
			if (mSeedType == SeedType.ImitaterRandomZombie)
			{
				Zombie zombie = mBoard.mChallenge.IZombiePlaceZombie(GetRandomZombieType(), mPlantCol, mRow);
				zombie.mSunMoneyLoot = Zombie.GetZombieDefinition(zombie.mZombieType).mZombieValue * 25;
				return;
			}
			Debug.ASSERT(value: false);
		}
		if (!plant.mIsAsleep)
		{
			mBoard.DoPlantingSound(plant);
		}
		FilterEffectType mFilterEffect = FilterEffectType.WashedOut;
		if (flag)
		{
			if (mImitaterType == SeedType.Hypnoshroom || mImitaterType == SeedType.Squash || mImitaterType == SeedType.Potatomine || mImitaterType == SeedType.Garlic || mImitaterType == SeedType.Lilypad)
			{
				mFilterEffect = FilterEffectType.LessWashedOut;
			}
			Reanimation reanimation = mApp.ReanimationTryToGet(plant.mBodyReanimID);
			if (reanimation != null)
			{
				reanimation.mFilterEffect = mFilterEffect;
			}
			Reanimation reanimation2 = mApp.ReanimationTryToGet(plant.mHeadReanimID);
			if (reanimation2 != null)
			{
				reanimation2.mFilterEffect = mFilterEffect;
			}
			Reanimation reanimation3 = mApp.ReanimationTryToGet(plant.mHeadReanimID2);
			if (reanimation3 != null)
			{
				reanimation3.mFilterEffect = mFilterEffect;
			}
			Reanimation reanimation4 = mApp.ReanimationTryToGet(plant.mHeadReanimID3);
			if (reanimation4 != null)
			{
				reanimation4.mFilterEffect = mFilterEffect;
			}
		}
		static ZombieType GetRandomZombieType()
		{
			int num2 = 0;
			for (ZombieType zombieType = ZombieType.Normal; zombieType < ZombieType.ZombieTypesCount; zombieType++)
			{
				Board.aZombieWeightArray[num2].Reset();
				ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(zombieType);
				Board.aZombieWeightArray[num2].mWeight = 100;
				if (zombieType == ZombieType.Yeti)
				{
					Board.aZombieWeightArray[num2].mWeight = 5;
				}
				else if (zombieType == ZombieType.Boss)
				{
					continue;
				}
				Board.aZombieWeightArray[num2].mItem = zombieType;
				num2++;
			}
			return (ZombieType)TodCommon.TodPickFromWeightedArray(Board.aZombieWeightArray, num2);
		}
	}

	public void UpdateImitater()
	{
		if (mState != PlantState.ImitaterMorphing)
		{
			if (mStateCountdown <= 150)
			{
				mState = PlantState.ImitaterMorphing;
				mStateCountdown = 0;
				PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_explode, ReanimLoopType.PlayOnceAndHold, 0, 30f);
			}
			return;
		}
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		if (reanimation.ShouldTriggerTimedEvent(0.9f))
		{
			Reanimation reanimation2 = mApp.AddReanimation(mX, mY, 400000, ReanimationType.Imitater);
			reanimation2.PlayReanim("anim_explode_particle", ReanimLoopType.PlayOnce, 0, 30f);
			reanimation2.mAnimTime = 0.7f;
			ImitaterMorph();
		}
	}

	public void UpdateReanim()
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation == null)
		{
			return;
		}
		UpdateReanimColor();
		float num = mShakeOffsetX;
		float num2 = mShakeOffsetY + PlantDrawHeightOffset(mBoard, this, mSeedType, mPlantCol, mRow);
		float num3 = 1f;
		float num4 = 1f;
		if (mApp.mGameMode == GameMode.ChallengeBigTime && (mSeedType == SeedType.Wallnut || mSeedType == SeedType.Sunflower || mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.Marigold || mSeedType == SeedType.SunflowerWallnut))
		{
			num3 = 1.5f;
			num4 = 1.5f;
			num += -20f;
			num2 += -40f;
		}
		if (mSeedType == SeedType.Gravebuster && IsInPlay())
		{
			num -= 18f;
		}
		_ = mSeedType;
		_ = 34;
		if (mSeedType == SeedType.GiantWallnut)
		{
			num3 = 2f;
			num4 = 2f;
			num += -76f;
			num2 += -64f;
		}
		if (mSeedType == SeedType.InstantCoffee)
		{
			num3 = 0.8f;
			num4 = 0.8f;
			num += 12f;
			num2 += 10f;
		}
		if (mSeedType == SeedType.Potatomine)
		{
			num3 = 0.8f;
			num4 = 0.8f;
			num += 12f;
			num2 += 12f;
		}
		if (mSeedType == SeedType.SunflowerPea || mSeedType == SeedType.SunflowerWallnut)
		{
			num -= 16f;
		}
		if (mSeedType == SeedType.PeaWallnut)
		{
			num += 16f;
		}
		if (mSeedType == SeedType.Jalapeno && reanimation != null && IsInPlay())
		{
			float num5 = 0.12f;
			float num6 = 0.14f;
			float num7 = 0.3f;
			float num8 = 0f;
			if (reanimation.mAnimTime > num6 && reanimation.mAnimTime < num7)
			{
				num8 = (reanimation.mAnimTime - num6) / (num7 - num6) * (float)Constants.BOARD_WIDTH * Constants.S;
			}
			else if (reanimation.mAnimTime >= num7)
			{
				num8 = (float)Constants.BOARD_WIDTH * Constants.S;
			}
			else if (reanimation.mAnimTime <= num5)
			{
				num += TodCommon.TodAnimateCurveFloatTime(0f, num5, reanimation.mAnimTime, mX - Constants.LAWN_XMIN, 0f, TodCurves.EaseInOutWeak);
				num2 += TodCommon.TodAnimateCurveFloatTime(0f, num5, reanimation.mAnimTime, (float)mY - mBoard.GetPosYBasedOnRow(Constants.LAWN_XMIN, mRow), 0f, TodCurves.EaseInOutWeak);
			}
			if (reanimation.mAnimTime >= 0.88f)
			{
				float num9 = TodCommon.TodAnimateCurveFloatTime(0.88f, 1f, reanimation.mAnimTime, 0f, 250f, TodCurves.EaseIn);
				num += num9;
				num2 -= num9;
			}
			num -= (float)(mX - Constants.LAWN_XMIN);
			num2 += mBoard.GetPosYBasedOnRow(num8 + (float)Constants.LAWN_XMIN, mRow);
			num2 -= (float)mY;
		}
		_ = mState;
		_ = 9;
		if (mWakeUpCounter > 0)
		{
			float num10 = TodCommon.TodAnimateCurveFloat(70, 0, mWakeUpCounter, 1f, 0.8f, TodCurves.EaseSinWave);
			num4 *= num10;
			num2 += 80f - num10 * 80f;
		}
		reanimation.Update();
		if (mSeedType == SeedType.Leftpeater)
		{
			num += 80f * num3;
			num3 *= -1f;
		}
		if (mPottedPlantIndex != -1)
		{
			PottedPlant pottedPlant = mApp.mPlayerInfo.mPottedPlant[mPottedPlantIndex];
			if (pottedPlant.mFacing == PottedPlant.FacingDirection.Left)
			{
				num += 80f * num3;
				num3 *= -1f;
			}
			float num11;
			float thePositionEnd;
			float num12;
			float thePositionEnd2;
			float num13;
			float thePositionEnd3;
			if (pottedPlant.mPlantAge == PottedPlantAge.Small)
			{
				num11 = 20f;
				thePositionEnd = num11;
				num12 = 40f;
				thePositionEnd2 = num12;
				num13 = 0.5f;
				thePositionEnd3 = num13;
			}
			else if (pottedPlant.mPlantAge == PottedPlantAge.Medium)
			{
				num11 = 20f;
				thePositionEnd = 10f;
				num12 = 40f;
				thePositionEnd2 = 20f;
				num13 = 0.5f;
				thePositionEnd3 = 0.75f;
			}
			else
			{
				num11 = 10f;
				thePositionEnd = 0f;
				num12 = 20f;
				thePositionEnd2 = 0f;
				num13 = 0.75f;
				thePositionEnd3 = 1f;
			}
			float num14 = TodCommon.TodAnimateCurveFloat(100, 0, mStateCountdown, num11, thePositionEnd, TodCurves.Linear);
			float num15 = TodCommon.TodAnimateCurveFloat(100, 0, mStateCountdown, num12, thePositionEnd2, TodCurves.Linear);
			float num16 = TodCommon.TodAnimateCurveFloat(100, 0, mStateCountdown, num13, thePositionEnd3, TodCurves.Linear);
			num += num14 * num3;
			num2 += num15 * num4;
			num3 *= num16;
			num4 *= num16;
			num += mApp.mZenGarden.ZenPlantOffsetX(pottedPlant);
			num2 += mApp.mZenGarden.PlantPottedDrawHeightOffset(mSeedType, num4, bInWheelBarrow: false);
		}
		reanimation.SetPosition(num * Constants.S, num2 * Constants.S);
		reanimation.OverrideScale(num3, num4);
	}

	public void SpikeRockTakeDamage()
	{
		Reanimation reanimation = mApp.ReanimationGet(mBodyReanimID);
		mPlantHealth -= 50;
		SpikeweedAttack();
		_ = mPlantHealth;
		_ = 300;
		_ = mPlantHealth;
		_ = 150;
		if (mPlantHealth <= 0)
		{
			PlantVoice.Play(SeedType.Spikerock, SeedType.None, PlantVoice.VoiceType.Die);
			PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_explode, ReanimLoopType.PlayOnce, 0, 30f);
			mState = PlantState.Doingspecial;
			mDisappearCountdown = 295;
			mRenderOrder += 5000;
		}
	}

	public bool IsSpiky()
	{
		if (mSeedType != SeedType.Spikeweed)
		{
			return mSeedType == SeedType.Spikerock;
		}
		return true;
	}

	public static void PreloadPlantResources(SeedType theSeedType)
	{
		PlantDefinition plantDefinition = GetPlantDefinition(theSeedType);
		if (plantDefinition.mReanimationType != ReanimationType.None)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(plantDefinition.mReanimationType, theIsPreloading: true);
		}
		if (theSeedType == SeedType.Cherrybomb)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.ZombieCharred, theIsPreloading: true);
		}
		if (theSeedType == SeedType.Jalapeno || theSeedType == SeedType.PickledPepper)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.JalapenoFire, theIsPreloading: true);
		}
		if (theSeedType == SeedType.Torchwood)
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.FirePea, theIsPreloading: true);
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.JalapenoFire, theIsPreloading: true);
		}
		if (IsNocturnal(theSeedType))
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Sleeping, theIsPreloading: true);
		}
	}

	public bool IsInPlay()
	{
		if (IsOnBoard() && mApp.mGameMode != GameMode.ChallengeZenGarden)
		{
			return mApp.mGameMode != GameMode.TreeOfWisdom;
		}
		return false;
	}

	public void UpdateNeedsFood()
	{
	}

	public string GetStateTrackIdSuffix()
	{
		if (mSeedType == SeedType.Spikerock)
		{
			if (mPlantHealth <= 150)
			{
				return "-3";
			}
			if (mPlantHealth <= 300)
			{
				return "-2";
			}
		}
		else if (mSeedType == SeedType.Tallnut)
		{
			if (mPlantHealth < mPlantMaxHealth / 3)
			{
				return "-3";
			}
			if (mPlantHealth < mPlantMaxHealth * 2 / 3)
			{
				return "-2";
			}
		}
		return string.Empty;
	}

	public void PlayIdleAnim(float theRate)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(mBodyReanimID);
		if (reanimation != null)
		{
			string text = GlobalMembersReanimIds.ReanimTrackId_anim_idle;
			if (mApp.mGameMode == GameMode.ChallengeZenGarden && reanimation.TrackExists("anim_garden_idle"))
			{
				text = "anim_garden_idle";
			}
			PlayBodyReanim(text + GetStateTrackIdSuffix(), ReanimLoopType.Loop, 20, theRate);
			if (mApp.IsIZombieLevel())
			{
				reanimation.mAnimRate = 0f;
			}
		}
	}

	public void UpdateFlowerPot()
	{
		if (mState == PlantState.FlowerpotInvulnerable && mStateCountdown <= 0)
		{
			mState = PlantState.Notready;
		}
	}

	public void UpdateLilypad()
	{
		if (mState == PlantState.LilypadInvulnerable && mStateCountdown <= 0)
		{
			mState = PlantState.Notready;
		}
	}

	public void GoldMagnetFindTargets()
	{
		while (true)
		{
			MagnetItem freeMagnetItem = GetFreeMagnetItem();
			if (freeMagnetItem != null)
			{
				Coin coin = FindGoldMagnetTarget();
				if (coin == null)
				{
					break;
				}
				freeMagnetItem.mPosX = coin.mPosX + 15f;
				freeMagnetItem.mPosY = coin.mPosY + 15f;
				freeMagnetItem.mDestOffsetX = TodCommon.RandRangeFloat(20f, 40f);
				freeMagnetItem.mDestOffsetY = TodCommon.RandRangeFloat(-20f, 0f) + 20f;
				if (coin.mType == CoinType.Silver)
				{
					freeMagnetItem.mItemType = MagnetItemType.SilverCoin;
				}
				else if (coin.mType == CoinType.Gold)
				{
					freeMagnetItem.mItemType = MagnetItemType.GoldCoin;
				}
				else if (coin.mType == CoinType.Diamond)
				{
					freeMagnetItem.mItemType = MagnetItemType.Diamond;
				}
				else
				{
					Debug.ASSERT(value: false);
				}
				coin.Die();
				continue;
			}
			break;
		}
	}

	public bool IsAGoldMagnetAboutToSuck()
	{
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && !plant.NotOnGround() && plant.mSeedType == SeedType.GoldMagnet && plant.mState == PlantState.MagnetshroomSucking)
			{
				Reanimation reanimation = mApp.ReanimationGet(plant.mBodyReanimID);
				if (reanimation.mAnimTime < 0.5f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool DrawMagnetItemsOnTop()
	{
		if (mSeedType == SeedType.GoldMagnet)
		{
			for (int i = 0; i < 5; i++)
			{
				MagnetItem magnetItem = mMagnetItems[i];
				if (magnetItem.mItemType != MagnetItemType.None)
				{
					return true;
				}
			}
			return false;
		}
		if (mSeedType == SeedType.Magnetshroom)
		{
			for (int j = 0; j < 5; j++)
			{
				MagnetItem magnetItem2 = mMagnetItems[j];
				if (magnetItem2.mItemType != MagnetItemType.None)
				{
					float num = new SexyVector2((float)mX + magnetItem2.mDestOffsetX - magnetItem2.mPosX, (float)mY + magnetItem2.mDestOffsetY - magnetItem2.mPosY).Magnitude();
					if (num > 20f)
					{
						return true;
					}
				}
			}
			return false;
		}
		return false;
	}

	public void checkForPlantAchievements()
	{
		if (mBoard == null || !IsOnBoard())
		{
			return;
		}
		if (mSeedType == SeedType.Twinsunflower)
		{
			mBoard.CountPlantByType(mSeedType);
		}
		if (mSeedType == SeedType.Cobcannon)
		{
			mBoard.CountPlantByType(mSeedType);
		}
		if (mSeedType >= SeedType.Peashooter && mSeedType < SeedType.PickledPepper)
		{
			mApp.mPlayerInfo.mPlantTypesUsed[(int)mSeedType] = true;
			for (int i = 0; i < mApp.mPlayerInfo.mPlantTypesUsed.Length && mApp.mPlayerInfo.mPlantTypesUsed[i]; i++)
			{
			}
		}
		if (mBoard.StageHasFog() && (mSeedType == SeedType.Plantern || mSeedType == SeedType.Blover))
		{
			mBoard.mPlanternOrBloverUsed = true;
		}
		if (mBoard.StageIsNight() && (mSeedType == SeedType.Wallnut || mSeedType == SeedType.Tallnut))
		{
			mBoard.mNutsUsed = true;
		}
		if (mSeedType != SeedType.Wintermelon)
		{
			return;
		}
		uint theNumber = 0u;
		int count = mBoard.mPlants.Count;
		for (int j = 0; j < count; j++)
		{
			Plant plant = mBoard.mPlants[j];
			if (!plant.mDead && plant.mSeedType == SeedType.Wintermelon)
			{
				TodCommon.SetBit(ref theNumber, plant.mRow, 1);
			}
		}
		mBoard.StageHas6Rows();
	}

	public static PlantDefinition GetPlantDefinition(SeedType theSeedtype)
	{
		Debug.ASSERT(theSeedtype >= SeedType.Peashooter && theSeedtype < SeedType.SeedTypeCount);
		Debug.ASSERT(GameConstants.gPlantDefs[(int)theSeedtype].mSeedType == theSeedtype);
		return GameConstants.gPlantDefs[(int)theSeedtype];
	}

	public override bool SaveToFile(Sexy.Buffer b)
	{
		base.SaveToFile(b);
		b.WriteString(lastPlayedBodyReanim_Name);
		b.WriteFloat(lastPlayedBodyReanim_AnimRate);
		b.WriteByte(lastPlayedBodyReanim_BlendTime);
		b.WriteLong((int)lastPlayedBodyReanim_Type);
		b.WriteLong((int)mSeedType);
		b.WriteLong((int)mImitaterType);
		b.WriteBoolean(mIsOnBoard);
		b.WriteLong(mAnimCounter);
		b.WriteBoolean(mAnimPing);
		b.WriteLong(mBeghouledFlashCountdown);
		b.WriteLong(mBlinkCountdown);
		b.WriteBoolean(mDead);
		b.WriteLong(mDisappearCountdown);
		b.WriteLong(mDoSpecialCountdown);
		b.WriteLong(mEatenFlashCountdown);
		b.WriteLong(mFrame);
		b.WriteLong(mFrameLength);
		b.WriteBoolean(mHighlighted);
		b.WriteBoolean(mInFlowerPot);
		b.WriteBoolean(mIsAsleep);
		b.WriteLong(mLaunchCounter);
		b.WriteLong(mLaunchRate);
		for (int i = 0; i < mMagnetItems.Length; i++)
		{
			b.WriteBoolean(mMagnetItems[i] != null);
			if (mMagnetItems[i] != null)
			{
				mMagnetItems[i].SaveToFile(b);
			}
		}
		b.WriteLong(mNumFrames);
		b.WriteLong((int)mOnBungeeState);
		b.WriteRect(mPlantAttackRect);
		b.WriteLong(mPlantCol);
		b.WriteLong(mPlantHealth);
		b.WriteLong(mPlantMaxHealth);
		b.WriteRect(mPlantRect);
		b.WriteLong(mPottedPlantIndex);
		b.WriteLong(mRecentlyEatenCountdown);
		b.WriteFloat(mShakeOffsetX);
		b.WriteFloat(mShakeOffsetY);
		b.WriteLong(mShootingCounter);
		b.WriteBoolean(mSquished);
		b.WriteLong(mStartRow);
		b.WriteLong((int)mState);
		b.WriteLong(mStateCountdown);
		b.WriteLong(mSubclass);
		b.WriteLong(mTargetX);
		b.WriteLong(mTargetY);
		GameObject.SaveId(mTargetZombieID, b);
		b.WriteLong(mWakeUpCounter);
		b.WriteFloat(mBodyReanimID?.mAnimTime ?? 0f);
		return true;
	}

	public override bool LoadFromFile(Sexy.Buffer b)
	{
		base.LoadFromFile(b);
		lastPlayedBodyReanim_Name = b.ReadString();
		lastPlayedBodyReanim_AnimRate = b.ReadFloat();
		lastPlayedBodyReanim_BlendTime = b.ReadByte();
		lastPlayedBodyReanim_Type = (ReanimLoopType)b.ReadLong();
		mSeedType = SeedTypeLegacy.FromInt(b.ReadLong());
		mImitaterType = SeedTypeLegacy.FromInt(b.ReadLong());
		mIsOnBoard = b.ReadBoolean();
		mBoard = GlobalStaticVars.gLawnApp.mBoard;
		int num = mX;
		int num2 = mY;
		PlantInitialize(mBoard.PixelToGridX(mX, mY), mRow, mSeedType, mImitaterType);
		mX = num;
		mY = num2;
		mAnimCounter = b.ReadLong();
		mAnimPing = b.ReadBoolean();
		mBeghouledFlashCountdown = b.ReadLong();
		mBlinkCountdown = b.ReadLong();
		mDead = b.ReadBoolean();
		mDisappearCountdown = b.ReadLong();
		mDoSpecialCountdown = b.ReadLong();
		mEatenFlashCountdown = b.ReadLong();
		mFrame = b.ReadLong();
		mFrameLength = b.ReadLong();
		mHighlighted = b.ReadBoolean();
		mInFlowerPot = b.ReadBoolean();
		mIsAsleep = b.ReadBoolean();
		mLaunchCounter = b.ReadLong();
		mLaunchRate = b.ReadLong();
		for (int i = 0; i < mMagnetItems.Length; i++)
		{
			if (b.ReadBoolean())
			{
				mMagnetItems[i] = new MagnetItem();
				mMagnetItems[i].LoadFromFile(b);
			}
			else
			{
				mMagnetItems[i] = null;
			}
		}
		mNumFrames = b.ReadLong();
		mOnBungeeState = (PlantOnBungeeState)b.ReadLong();
		mPlantAttackRect = b.ReadRect();
		mPlantCol = b.ReadLong();
		mPlantHealth = b.ReadLong();
		mPlantMaxHealth = b.ReadLong();
		mPlantRect = b.ReadRect();
		mPottedPlantIndex = b.ReadLong();
		mRecentlyEatenCountdown = b.ReadLong();
		mShakeOffsetX = b.ReadFloat();
		mShakeOffsetY = b.ReadFloat();
		mShootingCounter = b.ReadLong();
		mSquished = b.ReadBoolean();
		mStartRow = b.ReadLong();
		mState = (PlantState)b.ReadLong();
		mStateCountdown = b.ReadLong();
		mSubclass = b.ReadLong();
		mTargetX = b.ReadLong();
		mTargetY = b.ReadLong();
		mTargetZombieIDSaved = GameObject.LoadId(b);
		mWakeUpCounter = b.ReadLong();
		mBodyReanimID.mAnimTime = b.ReadFloat();
		return true;
	}

	public override void LoadingComplete()
	{
		base.LoadingComplete();
		mTargetZombieID = GameObject.GetObjectById(mTargetZombieIDSaved) as Zombie;
		if (!mIsAsleep && mSleepingReanimID != null)
		{
			mIsAsleep = true;
			SetSleeping(theIsAsleep: false);
		}
		float mAnimTime = mBodyReanimID.mAnimTime;
		if (!string.IsNullOrEmpty(lastPlayedBodyReanim_Name))
		{
			PlayBodyReanim(lastPlayedBodyReanim_Name, lastPlayedBodyReanim_Type, 0, lastPlayedBodyReanim_AnimRate);
		}
		mBodyReanimID.mAnimTime = mAnimTime;
	}

	public static float PlantDrawHeightOffset(Board theBoard, Plant thePlant, SeedType theSeedType, int theCol, int theRow)
	{
		float num = 0f;
		bool flag = false;
		if (IsFlying(theSeedType))
		{
			flag = false;
		}
		else if (theBoard == null)
		{
			if (theSeedType == SeedType.Lilypad || theSeedType == SeedType.Tanglekelp || theSeedType == SeedType.Seashroom || theSeedType == SeedType.Cattail || theSeedType == SeedType.HypnoCattail)
			{
				flag = true;
			}
		}
		else if (theBoard.IsPoolSquare(theCol, theRow))
		{
			flag = true;
		}
		if (flag)
		{
			int num2 = theBoard?.mMainCounter ?? GlobalStaticVars.gLawnApp.mAppCounter;
			float num3 = (float)theRow * (float)Math.PI + (float)theCol * (float)Math.PI * 0.25f;
			float num4 = (float)num2 * (float)Math.PI * 2f / 200f;
			float num5 = (float)Math.Sin(num3 + num4) * 2f;
			num += num5;
		}
		if (theBoard != null && (thePlant == null || !thePlant.mSquished) && thePlant != null && thePlant.mInFlowerPot)
		{
			Plant flowerPotAt = theBoard.GetFlowerPotAt(theCol, theRow);
			if (flowerPotAt != null && !flowerPotAt.mSquished && theSeedType != SeedType.Flowerpot)
			{
				num += PlantFlowerPotHeightOffset(theSeedType, 1f);
			}
		}
		switch (theSeedType)
		{
		case SeedType.Flowerpot:
			num += 26f;
			break;
		case SeedType.Lilypad:
			num += 25f;
			break;
		case SeedType.Starfruit:
			num += 10f;
			break;
		case SeedType.Tanglekelp:
			num += 24f;
			break;
		case SeedType.Seashroom:
			num += 28f;
			break;
		case SeedType.InstantCoffee:
			num -= 20f;
			break;
		default:
			if (IsFlying(theSeedType))
			{
				num -= 30f;
			}
			else
			{
				if (theSeedType == SeedType.Cactus)
				{
					break;
				}
				if (theSeedType == SeedType.Pumpkinshell)
				{
					num += 0f;
				}
				else if (theSeedType == SeedType.Puffshroom)
				{
					num += 5f;
				}
				else
				{
					if (theSeedType == SeedType.Scaredyshroom)
					{
						break;
					}
					if (theSeedType == SeedType.Gravebuster)
					{
						num += 8f;
					}
					else if (theSeedType == SeedType.Endoflame && GlobalStaticVars.gLawnApp.mGameMode != GameMode.ChallengeZenGarden)
					{
						num += 15f;
					}
					else if (theSeedType == SeedType.Spikeweed || theSeedType == SeedType.Spikerock)
					{
						int num6 = 4;
						if (theBoard != null && theBoard.StageHas6Rows())
						{
							num6 = 5;
						}
						_ = 46;
						if (theBoard != null && theBoard.GetFlowerPotAt(theCol, theRow) != null && GlobalStaticVars.gLawnApp.mGameMode != GameMode.ChallengeZenGarden)
						{
							num += 5f;
						}
						else if (theBoard != null && theBoard.StageHasRoof())
						{
							num += 15f;
						}
						else if (theBoard != null && theBoard.IsPoolSquare(theCol, theRow))
						{
							num += 0f;
						}
						else if (theRow == num6 && theCol >= 7 && theBoard.StageHas6Rows())
						{
							num += 6f;
						}
						else
						{
							num = ((theRow != num6) ? (num + 15f) : (num + 6f));
						}
					}
				}
			}
			break;
		}
		theBoard?.StageHasRoof();
		return num;
	}

	public static float PlantFlowerPotHeightOffset(SeedType theSeedType, float theFlowerPotScale)
	{
		float num = -5f * theFlowerPotScale;
		float num2 = 0f;
		switch (theSeedType)
		{
		case SeedType.Chomper:
		case SeedType.Plantern:
		case SeedType.SuperChomper:
			num -= 5f;
			break;
		case SeedType.Scaredyshroom:
			num += -3f;
			num2 += -8f;
			break;
		case SeedType.Puffshroom:
		case SeedType.Sunshroom:
			num2 += -4f;
			break;
		case SeedType.Hypnoshroom:
			num2 += -8f;
			break;
		case SeedType.Magnetshroom:
			num2 += -8f;
			break;
		case SeedType.Peashooter:
		case SeedType.Sunflower:
		case SeedType.Snowpea:
		case SeedType.Repeater:
		case SeedType.Threepeater:
		case SeedType.Marigold:
		case SeedType.Leftpeater:
		case SeedType.SunflowerPea:
		case SeedType.PeaWallnut:
		case SeedType.SunflowerWallnut:
			num2 += -8f;
			break;
		case SeedType.Cabbagepult:
		case SeedType.Melonpult:
			num2 += -8f;
			break;
		case SeedType.Tanglekelp:
			num2 += -8f;
			break;
		case SeedType.Blover:
			num2 += -8f;
			break;
		case SeedType.Spikeweed:
			num2 += -8f;
			break;
		case SeedType.Seashroom:
			num2 += -4f;
			break;
		case SeedType.Potatomine:
			num2 += -4f;
			break;
		case SeedType.Lilypad:
			num2 += -16f;
			break;
		case SeedType.InstantCoffee:
			num2 += -20f;
			break;
		case SeedType.Endoflame:
			num += 5f;
			break;
		}
		num2 = Constants.InvertAndScale(num2);
		return num + (0f - num2 + num2 * theFlowerPotScale);
	}

	public static bool CanSwap(SeedType aType)
	{
		if (aType != SeedType.Potatomine && aType != SeedType.Gravebuster && aType != SeedType.Tanglekelp && aType != SeedType.Spikeweed && aType != SeedType.Seashroom && aType != SeedType.Cattail && aType != SeedType.Spikerock && aType != SeedType.Cobcannon && aType != SeedType.Lilypad && aType != SeedType.Pumpkinshell && aType != SeedType.Flowerpot && aType != SeedType.InstantCoffee && aType != SeedType.Imitater)
		{
			return aType != SeedType.HypnoCattail;
		}
		return false;
	}

	public void SetSealing(bool theIsSealed)
	{
		if (!NotOnGround())
		{
			bool flag = mSealedCountdown > 0;
			mSealedCountdown = (theIsSealed ? 3000 : 0);
			if (!flag & theIsSealed)
			{
				float theX = mX;
				float theY = mY - 20;
				Reanimation reanimation = mApp.AddReanimation(theX, theY, mRenderOrder + 2, ReanimationType.Talisman);
				reanimation.PlayReanim("anim_rainwater_crossflow", ReanimLoopType.Loop, 10, 12f);
				mSealedReanimID = mApp.ReanimationGetID(reanimation);
			}
			if (!theIsSealed)
			{
				mApp.RemoveReanimation(ref mSealedReanimID);
				mSealedReanimID = null;
			}
		}
	}

	public bool IsDisabled()
	{
		if (!mIsAsleep && mSealedCountdown <= 0)
		{
			return mInTalismanCounter > 0;
		}
		return true;
	}
}
