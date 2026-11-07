using System;
using System.Collections.Generic;
using Lawn.Creative;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Projectile : GameObject
{
	public int mFrame;

	public int mNumFrames;

	public int mAnimCounter;

	public float mPosX;

	public float mPosY;

	public float mPosZ;

	public float mVelX;

	public float mVelY;

	public float mVelZ;

	public float mAccZ;

	public float mShadowY;

	public bool mDead;

	public int mAnimTicksPerFrame;

	public ProjectileMotion mMotionType;

	public ProjectileType mProjectileType;

	public int mProjectileAge;

	public int mClickBackoffCounter;

	public float mRotation;

	public float mRotationSpeed;

	public bool mOnHighGround;

	public int mDamageRangeFlags;

	public int mHitTorchwoodGridX;

	public Attachment mAttachmentID;

	public int mAttachmentID_Save;

	public float mCobTargetX;

	public int mCobTargetRow;

	public Zombie mTargetZombieID;

	public int mTargetZombieID_Save;

	private int mTargetZombieIDSaved;

	public int mLastPortalX;

	public SeedType mFromPlant;

	public Reanimation mProjectileReanimID;

	private int mProjectileReanim_Save;

	private static Stack<Projectile> unusedObjects = new Stack<Projectile>(200);

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncInt(ref mFrame);
		theContext.SyncInt(ref mNumFrames);
		theContext.SyncInt(ref mAnimCounter);
		theContext.SyncFloat(ref mPosX);
		theContext.SyncFloat(ref mPosY);
		theContext.SyncFloat(ref mPosZ);
		theContext.SyncFloat(ref mVelX);
		theContext.SyncFloat(ref mVelY);
		theContext.SyncFloat(ref mVelZ);
		theContext.SyncFloat(ref mAccZ);
		theContext.SyncFloat(ref mShadowY);
		theContext.SyncBool(ref mDead);
		theContext.SyncInt(ref mAnimTicksPerFrame);
		theContext.SyncEnum(ref mMotionType, theContext.aProjectileMotionSaver, SaveFileVersion.Initial, ProjectileMotion.Straight);
		theContext.SyncEnum(ref mProjectileType, theContext.aProjectileTypeSaver, SaveFileVersion.Initial, ProjectileType.Pea);
		theContext.SyncInt(ref mProjectileAge);
		theContext.SyncInt(ref mClickBackoffCounter);
		theContext.SyncFloat(ref mRotation);
		theContext.SyncFloat(ref mRotationSpeed);
		theContext.SyncBool(ref mOnHighGround);
		theContext.SyncInt(ref mDamageRangeFlags);
		theContext.SyncInt(ref mHitTorchwoodGridX);
		theContext.SyncInt(ref mAttachmentID_Save);
		theContext.SyncFloat(ref mCobTargetX);
		theContext.SyncInt(ref mCobTargetRow);
		theContext.SyncInt(ref mTargetZombieID_Save);
		theContext.SyncInt(ref mLastPortalX);
		theContext.SyncEnum(ref mFromPlant, theContext.aSeedTypeSaver, SaveFileVersion.Projectile_FromPlant, SeedType.Peashooter);
		theContext.SyncInt(ref mProjectileReanim_Save, SaveFileVersion.Projectile_FromPlant);
	}

	internal override void SyncObj(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.SyncObj(theContext);
		TodLibObjSyncer.SyncObjFromList(ref mAttachmentID_Save, ref mAttachmentID, mApp.mEffectSystem.mAttachmentHolder.mAttachments, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mTargetZombieID_Save, ref mTargetZombieID, mBoard.mZombies, theContext.mReading);
		if (theContext.mVersion >= SaveFileVersion.Projectile_FromPlant)
		{
			TodLibObjSyncer.SyncObjFromList(ref mProjectileReanim_Save, ref mProjectileReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		}
		if (theContext.mVersion < SaveFileVersion.NewLayout600)
		{
			mPosX -= 100f;
			mX -= 100;
			mAttachmentID?.Upgrade600();
		}
	}

	public static void PreallocateMemory()
	{
		for (int i = 0; i < 200; i++)
		{
			new Projectile().PrepareForReuse();
		}
	}

	internal static Projectile GetNewProjectile()
	{
		if (unusedObjects.Count > 0)
		{
			Projectile projectile = unusedObjects.Pop();
			projectile.Reset();
			return projectile;
		}
		return new Projectile();
	}

	private Projectile()
	{
	}

	public override void PrepareForReuse()
	{
		unusedObjects.Push(this);
	}

	public override bool SaveToFile(Sexy.Buffer b)
	{
		base.SaveToFile(b);
		b.WriteFloat(mAccZ);
		b.WriteLong(mAnimCounter);
		b.WriteLong(mAnimTicksPerFrame);
		b.WriteLong(mClickBackoffCounter);
		b.WriteLong(mCobTargetRow);
		b.WriteFloat(mCobTargetX);
		b.WriteLong(mDamageRangeFlags);
		b.WriteBoolean(mDead);
		b.WriteLong(mFrame);
		b.WriteLong(mHitTorchwoodGridX);
		b.WriteLong(mLastPortalX);
		b.WriteLong((int)mMotionType);
		b.WriteLong(mNumFrames);
		b.WriteBoolean(mOnHighGround);
		b.WriteFloat(mPosX);
		b.WriteFloat(mPosY);
		b.WriteFloat(mPosZ);
		b.WriteLong(mProjectileAge);
		b.WriteLong((int)mProjectileType);
		b.WriteFloat(mRotation);
		b.WriteFloat(mRotationSpeed);
		b.WriteFloat(mShadowY);
		b.WriteFloat(mVelX);
		b.WriteFloat(mVelY);
		b.WriteFloat(mVelZ);
		GameObject.SaveId(mTargetZombieID, b);
		return true;
	}

	public override bool LoadFromFile(Sexy.Buffer b)
	{
		base.LoadFromFile(b);
		mAccZ = b.ReadFloat();
		mAnimCounter = b.ReadLong();
		mAnimTicksPerFrame = b.ReadLong();
		mClickBackoffCounter = b.ReadLong();
		mCobTargetRow = b.ReadLong();
		mCobTargetX = b.ReadFloat();
		mDamageRangeFlags = b.ReadLong();
		mDead = b.ReadBoolean();
		mFrame = b.ReadLong();
		mHitTorchwoodGridX = b.ReadLong();
		mLastPortalX = b.ReadLong();
		mMotionType = (ProjectileMotion)b.ReadLong();
		mNumFrames = b.ReadLong();
		mOnHighGround = b.ReadBoolean();
		mPosX = b.ReadFloat();
		mPosY = b.ReadFloat();
		mPosZ = b.ReadFloat();
		mProjectileAge = b.ReadLong();
		mProjectileType = (ProjectileType)b.ReadLong();
		mRotation = b.ReadFloat();
		mRotationSpeed = b.ReadFloat();
		mShadowY = b.ReadFloat();
		mVelX = b.ReadFloat();
		mVelY = b.ReadFloat();
		mVelZ = b.ReadFloat();
		mTargetZombieIDSaved = GameObject.LoadId(b);
		return true;
	}

	public override void LoadingComplete()
	{
		base.LoadingComplete();
		mTargetZombieID = GameObject.GetObjectById(mTargetZombieIDSaved) as Zombie;
		Projectile newProjectile = GetNewProjectile();
		newProjectile.ProjectileInitialize(mX, mY, mRenderOrder, mRow, mProjectileType);
		if (mProjectileType == ProjectileType.Fireball)
		{
			ConvertToFireball();
		}
	}

	protected override void Reset()
	{
		base.Reset();
		mFrame = 0;
		mNumFrames = 0;
		mAnimCounter = 0;
		mPosX = 0f;
		mPosY = 0f;
		mPosZ = 0f;
		mVelX = 0f;
		mVelY = 0f;
		mVelZ = 0f;
		mAccZ = 0f;
		mShadowY = 0f;
		mDead = false;
		mAnimTicksPerFrame = 0;
		mMotionType = ProjectileMotion.Straight;
		mProjectileType = ProjectileType.Pea;
		mProjectileAge = 0;
		mClickBackoffCounter = 0;
		mRotation = 0f;
		mRotationSpeed = 0f;
		mOnHighGround = false;
		mDamageRangeFlags = 0;
		mHitTorchwoodGridX = 0;
		mAttachmentID = null;
		mCobTargetX = 0f;
		mCobTargetRow = 0;
		mTargetZombieID = null;
		mLastPortalX = 0;
		mFromPlant = SeedType.None;
		mProjectileReanimID = null;
	}

	public void ProjectileInitialize(int theX, int theY, int theRenderOrder, int theRow, ProjectileType theProjectileType)
	{
		int num = mBoard.PixelToGridXKeepOnBoard(theX, theY);
		mProjectileType = theProjectileType;
		mPosX = theX;
		mPosY = theY;
		mPosZ = 0f;
		mVelX = 0f;
		mVelY = 0f;
		mVelZ = 0f;
		mAccZ = 0f;
		mShadowY = (float)mBoard.GridToPixelY(num, theRow) + 67f;
		mMotionType = ProjectileMotion.Straight;
		mFrame = 0;
		mNumFrames = 1;
		mRow = theRow;
		mDamageRangeFlags = 0;
		mHitTorchwoodGridX = -1;
		mDead = false;
		mAttachmentID = null;
		mCobTargetX = 0f;
		mCobTargetRow = 0;
		mTargetZombieID = null;
		mLastPortalX = -1;
		if (mBoard.mGridSquareType[num, mRow] == GridSquareType.HighGround)
		{
			mOnHighGround = true;
		}
		else
		{
			mOnHighGround = false;
		}
		if (mBoard.StageHasRoof() && (float)theX < 480f)
		{
			mShadowY -= 12f;
		}
		mRenderOrder = theRenderOrder;
		mProjectileAge = 0;
		mClickBackoffCounter = 0;
		mRotation = 0f;
		mRotationSpeed = 0f;
		mWidth = 40;
		mHeight = 40;
		mAnimTicksPerFrame = 0;
		bool flag = false;
		if (mProjectileType == ProjectileType.Cabbage || mProjectileType == ProjectileType.Butter)
		{
			mRotation = -0.87964594f;
			mRotationSpeed = TodCommon.RandRangeFloat(-0.08f, -0.02f);
		}
		else if (mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon)
		{
			mRotation = (float)Math.PI * -2f / 5f;
			mRotationSpeed = TodCommon.RandRangeFloat(-0.08f, -0.02f);
		}
		else if (mProjectileType == ProjectileType.Kernel)
		{
			mRotation = 0f;
			mRotationSpeed = TodCommon.RandRangeFloat(-0.2f, -0.08f);
		}
		else if (mProjectileType == ProjectileType.Snowpea)
		{
			float num2 = 8f;
			float num3 = 13f;
			TodParticleSystem theParticleSystem = mApp.AddTodParticle(mPosX + num2, mPosY + num3, 400000, ParticleEffect.SnowpeaTrail);
			GlobalMembersAttachment.AttachParticle(ref mAttachmentID, theParticleSystem, num2, num3);
		}
		else if (mProjectileType == ProjectileType.Fireball)
		{
			Debug.ASSERT(value: false);
		}
		else if (mProjectileType == ProjectileType.Cobbig)
		{
			mWidth = (int)((float)AtlasResources.IMAGE_REANIM_COBCANNON_COB.GetWidth() * Constants.IS);
			mHeight = (int)((float)AtlasResources.IMAGE_REANIM_COBCANNON_COB.GetHeight() * Constants.IS);
			mRotation = (float)Math.PI / 2f;
		}
		else if (mProjectileType == ProjectileType.Puff || mProjectileType == ProjectileType.PuffGreen)
		{
			float num4 = 13f;
			float num5 = 13f;
			TodParticleSystem todParticleSystem = mApp.AddTodParticle(mPosX + num4, mPosY + num5, 400000, ParticleEffect.PuffshroomTrail);
			if (mProjectileType == ProjectileType.PuffGreen)
			{
				todParticleSystem.OverrideImage(null, AtlasResources.IMAGE_SEASHROOM_PUFF_GREEN2);
			}
			GlobalMembersAttachment.AttachParticle(ref mAttachmentID, todParticleSystem, num4, num5);
		}
		else if (mProjectileType == ProjectileType.CattailSpike)
		{
			mWidth = 20;
			mHeight = 20;
			Trail theTrail = mApp.mEffectSystem.mTrailHolder.AllocTrail(0, TrailType.Cattail);
			GlobalMembersAttachment.AttachTrail(ref mAttachmentID, theTrail, 10f, 10f);
		}
		else if (mProjectileType == ProjectileType.HypnoCattailSpike)
		{
			mWidth = 20;
			mHeight = 20;
			Trail theTrail2 = mApp.mEffectSystem.mTrailHolder.AllocTrail(0, TrailType.HypnoCattail);
			GlobalMembersAttachment.AttachTrail(ref mAttachmentID, theTrail2, 10f, 10f);
		}
		else if (mProjectileType == ProjectileType.EndoflameSpike)
		{
			mWidth = 20;
			mHeight = 20;
			Trail theTrail3 = mApp.mEffectSystem.mTrailHolder.AllocTrail(0, TrailType.Endoflame);
			GlobalMembersAttachment.AttachTrail(ref mAttachmentID, theTrail3, 10f, 10f);
			float num6 = -15f;
			float num7 = -15f;
			Reanimation reanimation = mApp.AddReanimation(0f, 0f, mRenderOrder, ReanimationType.EndoflameSpike);
			reanimation.OverrideScale(0.5f, 0.5f);
			reanimation.SetFramesForLayer("anim_normal");
			reanimation.SetPosition((mPosX + num6) * Constants.S, (mPosY + num7) * Constants.S);
			reanimation.mLoopType = ReanimLoopType.Loop;
			reanimation.mAnimRate = TodCommon.RandRangeFloat(50f, 80f);
			GlobalMembersAttachment.AttachReanim(ref mAttachmentID, reanimation, num6, num7);
			mProjectileReanimID = mApp.ReanimationGetID(reanimation);
		}
		else if (mProjectileType == ProjectileType.Spike)
		{
			mHeight = 20;
		}
		else if (mProjectileType != ProjectileType.Butter)
		{
			if (mProjectileType == ProjectileType.Basketball)
			{
				mRotation = TodCommon.RandRangeFloat(0f, (float)Math.PI * 2f);
				mRotationSpeed = TodCommon.RandRangeFloat(0.05f, 0.1f);
			}
			else if (mProjectileType == ProjectileType.Star)
			{
				mShadowY += 15f;
				mRotationSpeed = TodCommon.RandRangeFloat(0.05f, 0.1f);
				if (RandomNumbers.NextNumber(2) == 0)
				{
					mRotationSpeed = 0f - mRotationSpeed;
				}
			}
			else if (mProjectileType == ProjectileType.ZombiePeaMindControl)
			{
				mDamageRangeFlags = 1;
			}
		}
		if (IsFromRepeatess())
		{
			mWidth /= 2;
			mHeight /= 2;
		}
		if (flag)
		{
			mAnimCounter = TodCommon.RandRangeInt(0, mNumFrames * mAnimTicksPerFrame);
		}
		else
		{
			mAnimCounter = 0;
		}
		mX = (int)mPosX;
		mY = (int)mPosY;
	}

	public void Dispose()
	{
		GlobalMembersAttachment.AttachmentDie(ref mAttachmentID);
	}

	public void Update()
	{
		mProjectileAge++;
		if (mApp.mGameScene == GameScenes.Playing || mBoard.mCutScene.ShouldRunUpsellBoard())
		{
			int num = 20;
			if (mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.Snowpea || mProjectileType == ProjectileType.Cabbage || mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon || mProjectileType == ProjectileType.Kernel || mProjectileType == ProjectileType.Butter || mProjectileType == ProjectileType.Cobbig || mProjectileType == ProjectileType.ZombiePea || mProjectileType == ProjectileType.Spike || mProjectileType == ProjectileType.CattailSpike || mProjectileType == ProjectileType.HypnoCattailSpike || mProjectileType == ProjectileType.EndoflameSpike || mProjectileType == ProjectileType.EndoflameFireball || mProjectileType == ProjectileType.ZombiePeaMindControl)
			{
				num = 0;
			}
			if (mProjectileAge > num)
			{
				mRenderOrder = Board.MakeRenderOrder(RenderLayer.Projectile, mRow, 0);
			}
			if (mApp.IsFinalBossLevel())
			{
				mRenderOrder = Board.MakeRenderOrder(RenderLayer.Projectile, 5, 0);
			}
			if (mClickBackoffCounter > 0)
			{
				mClickBackoffCounter--;
			}
			mRotation += mRotationSpeed;
			UpdateMotion();
			GlobalMembersAttachment.AttachmentUpdateAndMove(ref mAttachmentID, mPosX, mPosY + mPosZ);
		}
	}

	private bool IsFromRepeatess()
	{
		if (mFromPlant != SeedType.Repeater && mFromPlant != SeedType.Leftpeater && mFromPlant != SeedType.Peashooter && mFromPlant != SeedType.SunflowerPea && mFromPlant != SeedType.PeaWallnut && mFromPlant != SeedType.Gatlingpea && mFromPlant != SeedType.Snowpea)
		{
			return mFromPlant == SeedType.Splitpea;
		}
		return true;
	}

	public void Draw(Graphics g)
	{
		ProjectileDefinition projectileDef = GetProjectileDef();
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		Image image = null;
		float num = 1f;
		if (mProjectileType == ProjectileType.Cobbig)
		{
			image = AtlasResources.IMAGE_REANIM_COBCANNON_COB;
			num = 0.9f;
		}
		else if (mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.ZombiePea || mProjectileType == ProjectileType.ZombiePeaMindControl)
		{
			image = AtlasResources.IMAGE_PROJECTILEPEA;
			if (mProjectileType == ProjectileType.ZombiePeaMindControl)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(GameConstants.ZOMBIE_MINDCONTROLLED_COLOR);
			}
			if (IsFromRepeatess())
			{
				num = 0.5f;
			}
		}
		else if (mProjectileType == ProjectileType.Snowpea)
		{
			image = AtlasResources.IMAGE_PROJECTILESNOWPEA;
			if (IsFromRepeatess())
			{
				num = 0.5f;
			}
		}
		else if (mProjectileType == ProjectileType.Fireball)
		{
			image = null;
		}
		else if (mProjectileType == ProjectileType.Spike)
		{
			image = AtlasResources.IMAGE_PROJECTILECACTUS;
		}
		else if (mProjectileType == ProjectileType.CattailSpike)
		{
			image = AtlasResources.IMAGE_REANIM_PROJECTILECATTAIL;
			num = 1f / 3f;
		}
		else if (mProjectileType == ProjectileType.HypnoCattailSpike)
		{
			image = AtlasResources.IMAGE_REANIM_PROJECTILECATTAIL_HYPNO;
			num = 1f / 3f;
		}
		else if (mProjectileType == ProjectileType.Star)
		{
			image = AtlasResources.IMAGE_PROJECTILE_STAR;
		}
		else if (mProjectileType == ProjectileType.Puff)
		{
			image = AtlasResources.IMAGE_PUFFSHROOM_PUFF1;
			num = TodCommon.TodAnimateCurveFloat(0, 30, mProjectileAge, 0.3f, 1f, TodCurves.Linear);
		}
		else if (mProjectileType == ProjectileType.PuffGreen)
		{
			image = AtlasResources.IMAGE_SEASHROOM_PUFF_GREEN1;
			num = TodCommon.TodAnimateCurveFloat(0, 30, mProjectileAge, 0.3f, 1f, TodCurves.Linear);
		}
		else if (mProjectileType == ProjectileType.Basketball)
		{
			image = AtlasResources.IMAGE_REANIM_ZOMBIE_CATAPULT_BASKETBALL;
			num = 1.1f;
		}
		else if (mProjectileType == ProjectileType.Cabbage)
		{
			image = AtlasResources.IMAGE_REANIM_CABBAGEPULT_CABBAGE;
			num = 1f;
		}
		else if (mProjectileType == ProjectileType.Kernel)
		{
			image = AtlasResources.IMAGE_REANIM_CORNPULT_KERNAL;
			num = 0.95f;
		}
		else if (mProjectileType == ProjectileType.Butter)
		{
			image = AtlasResources.IMAGE_REANIM_CORNPULT_BUTTER;
			num = 0.8f;
		}
		else if (mProjectileType == ProjectileType.Melon)
		{
			image = AtlasResources.IMAGE_REANIM_MELONPULT_MELON;
			num = 1f;
		}
		else if (mProjectileType == ProjectileType.Wintermelon)
		{
			image = AtlasResources.IMAGE_REANIM_WINTERMELON_PROJECTILE;
			num = 1f;
		}
		else if (mProjectileType == ProjectileType.EndoflameSpike)
		{
			image = null;
		}
		else if (mProjectileType == ProjectileType.EndoflameFireball)
		{
			image = null;
		}
		else if (mProjectileType == ProjectileType.Talisman)
		{
			image = AtlasResources.IMAGE_REANIM_ZOMBIE_TALISMAN_OVERDUE_SOULS;
			num = 0.6f;
		}
		else if (mProjectileType == ProjectileType.TalismanMove)
		{
			image = AtlasResources.IMAGE_REANIM_ZOMBIE_TALISMAN_FIVE_GHOSTS_LUCK_MONEY;
			num = 0.6f;
		}
		else if (mProjectileType == ProjectileType.TalismanSeal)
		{
			image = AtlasResources.IMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW;
			num = 0.6f;
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		bool flag = false;
		if (mMotionType == ProjectileMotion.BeeBackwards)
		{
			flag = true;
		}
		if (image != null)
		{
			Debug.ASSERT(projectileDef.mImageRow < image.mNumRows);
			Debug.ASSERT(mFrame < image.mNumCols);
			int celWidth = image.GetCelWidth();
			int celHeight = image.GetCelHeight();
			TRect theSrcRect = new TRect(celWidth * mFrame, celHeight * projectileDef.mImageRow, celWidth, celHeight);
			float num2 = mPosX + (float)celWidth * 0.5f * num;
			float num3 = mPosZ + mPosY + (float)celHeight * 0.5f * num;
			SexyTransform2D sexyTransform2D = default;
			TodCommon.TodScaleRotateTransformMatrix(ref sexyTransform2D.mMatrix, num2 * Constants.S + (float)mBoard.mX, num3 * Constants.S + (float)mBoard.mY, mRotation, num, num);
			TodCommon.TodBltMatrix(g, image, sexyTransform2D.mMatrix, ref g.mClipRect, SexyColor.White, g.mDrawMode, theSrcRect);
		}
		if (mAttachmentID != null)
		{
			Graphics graphics = Graphics.GetNew(g);
			MakeParentGraphicsFrame(graphics);
			IsFromRepeatess();
			GlobalMembersAttachment.AttachmentDraw(mAttachmentID, graphics, theParentHidden: false, doScale: true);
			graphics.PrepareForReuse();
		}
		if (mProjectileType == ProjectileType.ZombiePeaMindControl)
		{
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	public void DrawShadow(Graphics g)
	{
		int theCelCol = 0;
		float num = 1f;
		float num2 = 1f;
		float num3 = mPosX - (float)mX;
		float num4 = mPosY - (float)mY;
		int num5 = mBoard.PixelToGridXKeepOnBoard(mX, mY);
		bool flag = false;
		if (mBoard.mGridSquareType[num5, mRow] == GridSquareType.HighGround)
		{
			flag = true;
		}
		if (mOnHighGround && !flag)
		{
			num4 += (float)Constants.HIGH_GROUND_HEIGHT;
		}
		else if (!mOnHighGround & flag)
		{
			num4 += (float)(-Constants.HIGH_GROUND_HEIGHT);
		}
		if (mBoard.StageIsNight())
		{
			theCelCol = 1;
		}
		if (mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.ZombiePea || mProjectileType == ProjectileType.ZombiePeaMindControl)
		{
			num3 += 3f;
		}
		else if (mProjectileType == ProjectileType.Snowpea)
		{
			num3 += -1f;
			num = 1.3f;
		}
		else if (mProjectileType == ProjectileType.Star)
		{
			num3 += 7f;
		}
		else if (mProjectileType == ProjectileType.Cabbage || mProjectileType == ProjectileType.Kernel || mProjectileType == ProjectileType.Butter || mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon)
		{
			num3 += 3f;
			num4 += 10f;
			num = 1.6f;
		}
		else
		{
			if (mProjectileType == ProjectileType.Puff || mProjectileType == ProjectileType.PuffGreen)
			{
				return;
			}
			if (mProjectileType == ProjectileType.Cobbig)
			{
				num = 1f;
				num2 = 3f;
				num3 += 57f;
			}
			else if (mProjectileType == ProjectileType.Fireball)
			{
				num = 1.4f;
			}
		}
		if (mMotionType == ProjectileMotion.Lobbed)
		{
			float num6 = 200f / (TodCommon.ClampFloat(0f - mPosZ, 0f, 200f) + 200f);
			num *= num6;
		}
		if (IsFromRepeatess())
		{
			num *= 0.5f;
		}
		TodCommon.TodDrawImageCelScaledF(g, AtlasResources.IMAGE_PEA_SHADOWS, num3 * Constants.S, (mShadowY - mPosY + num4) * Constants.S, theCelCol, 0, num * num2, num);
	}

	public void Die()
	{
		mDead = true;
		if (mProjectileType == ProjectileType.Puff || mProjectileType == ProjectileType.PuffGreen || mProjectileType == ProjectileType.Snowpea)
		{
			GlobalMembersAttachment.AttachmentCrossFade(mAttachmentID, "FadeOut");
			GlobalMembersAttachment.AttachmentDetach(ref mAttachmentID);
		}
		else
		{
			GlobalMembersAttachment.AttachmentDie(ref mAttachmentID);
		}
	}

	public void DoImpact(Zombie theZombie)
	{
		PlayImpactSound(theZombie);
		if (IsSplashDamage(theZombie))
		{
			if (mProjectileType == ProjectileType.Fireball || mProjectileType == ProjectileType.EndoflameFireball)
			{
				theZombie?.RemoveColdEffects();
			}
			DoSplashDamage(theZombie);
		}
		else if (theZombie != null)
		{
			ProjectileDefinition projectileDef = GetProjectileDef();
			uint damageFlags = GetDamageFlags(theZombie);
			int projectileDamage = GetProjectileDamage();
			ProjectileType projectileType = mProjectileType;
			theZombie.TakeDamage(GetProjectileDamage(), damageFlags);
		}
		int aRenderOrder = mRenderOrder + 1;
		float num = mPosX - mVelX;
		float num2 = mPosY + mPosZ - mVelY - mVelZ;
		ParticleEffect particleEffect = ParticleEffect.None;
		float num3 = mPosX + 12f;
		float num4 = mPosY + 12f;
		if (mProjectileType == ProjectileType.Melon)
		{
			mApp.AddTodParticle(num + 30f, num2 + 30f, aRenderOrder, ParticleEffect.Melonsplash);
		}
		else if (mProjectileType == ProjectileType.Wintermelon)
		{
			mApp.AddTodParticle(num + 30f, num2 + 30f, aRenderOrder, ParticleEffect.Wintermelon);
		}
		else if (mProjectileType == ProjectileType.Cobbig)
		{
			int aRenderOrder2 = Board.MakeRenderOrder(RenderLayer.Ground, mCobTargetRow, 2);
			mApp.AddTodParticle(mPosX + 80f, mPosY + 40f, aRenderOrder2, ParticleEffect.Blastmark);
			mApp.AddTodParticle(mPosX + 80f, mPosY + 40f, aRenderOrder, ParticleEffect.Popcornsplash);
			mApp.PlaySample(Resources.SOUND_DOOMSHROOM);
			mBoard.ShakeBoard(3, -4);
			mApp.Vibrate();
		}
		else if (mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.ZombiePeaMindControl)
		{
			num3 -= 15f;
			particleEffect = ParticleEffect.PeaSplat;
		}
		else if (mProjectileType == ProjectileType.Snowpea)
		{
			num3 -= 15f;
			particleEffect = ParticleEffect.SnowpeaSplat;
		}
		else if (mProjectileType == ProjectileType.Fireball)
		{
			if (IsSplashDamage(theZombie))
			{
				Reanimation reanimation = mApp.AddReanimation(mPosX + 38f, mPosY - 20f, aRenderOrder, ReanimationType.JalapenoFire);
				reanimation.mAnimTime = 0.25f;
				reanimation.mAnimRate = 24f;
				reanimation.OverrideScale(0.7f, 0.4f);
			}
		}
		else if (mProjectileType == ProjectileType.EndoflameFireball)
		{
			if (IsSplashDamage(theZombie))
			{
				Reanimation reanimation2 = mApp.AddReanimation(mPosX + 38f, mPosY - 20f, aRenderOrder, ReanimationType.JalapenoFire);
				reanimation2.mAnimTime = 0.25f;
				reanimation2.mAnimRate = 24f;
				reanimation2.OverrideScale(0.7f, 0.4f);
			}
			Coin coin = mBoard.AddCoin(mX, mY, CoinType.Tinysun, CoinMotion.Coin);
			coin.mIsBeingCollected = true;
		}
		else if (mProjectileType == ProjectileType.Star)
		{
			particleEffect = ParticleEffect.StarSplat;
		}
		else if (mProjectileType == ProjectileType.Puff || mProjectileType == ProjectileType.PuffGreen)
		{
			num3 -= 20f;
			particleEffect = ParticleEffect.PuffSplat;
		}
		else if (mProjectileType == ProjectileType.Cabbage)
		{
			num3 = num - 38f;
			num4 = num2 + 23f;
			particleEffect = ParticleEffect.CabbageSplat;
		}
		else if (mProjectileType == ProjectileType.Butter)
		{
			num3 = num - 20f;
			num4 = num2 + 63f;
			particleEffect = ParticleEffect.ButterSplat;
			theZombie?.ApplyButter();
		}
		else if (mProjectileType == ProjectileType.HypnoCattailSpike && theZombie != null && RandomNumbers.NextNumber(20) == 0)
		{
			theZombie.ApplyMindControl();
			return;
		}
		TodParticleSystem todParticleSystem = null;
		if (particleEffect != ParticleEffect.None)
		{
			if (theZombie != null)
			{
				float num5 = num3 + 52f - (float)theZombie.mX;
				float num6 = num4 - (float)theZombie.mY;
				if (theZombie.mZombiePhase == ZombiePhase.SnorkelWalkingInPool || theZombie.mZombiePhase == ZombiePhase.DolphinWalkingInPool)
				{
					num6 += 60f;
				}
				if (mMotionType == ProjectileMotion.Backwards)
				{
					num5 -= 80f;
				}
				else if (mPosX > (float)(theZombie.mX + 40) && mMotionType != ProjectileMotion.Lobbed)
				{
					num5 -= 60f;
				}
				num6 = TodCommon.ClampFloat(num6, 20f, 100f);
				todParticleSystem = theZombie.AddAttachedParticle((int)num5, (int)num6, particleEffect);
			}
			else
			{
				todParticleSystem = mApp.AddTodParticle(num3, num4, aRenderOrder, particleEffect);
			}
			if (mProjectileType == ProjectileType.PuffGreen)
			{
				todParticleSystem?.OverrideImage(null, AtlasResources.IMAGE_SEASHROOM_PUFF_GREEN2);
			}
		}
		Die();
	}

	public void UpdateMotion()
	{
		if (mAnimTicksPerFrame > 0)
		{
			mAnimCounter = (mAnimCounter + 1) % (mNumFrames * mAnimTicksPerFrame);
			mFrame = mAnimCounter / mAnimTicksPerFrame;
		}
		int theRow = mRow;
		float posYBasedOnRow = mBoard.GetPosYBasedOnRow(mPosX, mRow);
		if (mMotionType == ProjectileMotion.Lobbed)
		{
			UpdateLobMotion();
		}
		else
		{
			UpdateNormalMotion();
		}
		float posYBasedOnRow2 = mBoard.GetPosYBasedOnRow(mPosX, theRow);
		float num = posYBasedOnRow2 - posYBasedOnRow;
		if (mProjectileType == ProjectileType.Cobbig)
		{
			num = 0f;
		}
		if (mMotionType == ProjectileMotion.FloatOver)
		{
			mPosY += num;
		}
		if (mMotionType == ProjectileMotion.Lobbed)
		{
			mPosY += num;
			mPosZ -= num;
		}
		if (mMotionType == ProjectileMotion.EndoflameHoming)
		{
			mPosY += num;
		}
		mShadowY += num;
		mX = (int)mPosX;
		mY = (int)(mPosY + mPosZ);
	}

	private Zombie FindCollisionMindControlledTarget()
	{
		TRect projectileRect = GetProjectileRect();
		Zombie zombie = null;
		int num = 0;
		for (int i = 0; i < mBoard.mZombies.Count; i++)
		{
			Zombie zombie2 = mBoard.mZombies[i];
			if (!zombie2.mDead && zombie2.mRow - mRow == 0 && zombie2.mMindControlled)
			{
				TRect zombieRect = zombie2.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(projectileRect, zombieRect);
				if (rectOverlap >= 0 && (zombie != null || zombie2.mX > num))
				{
					zombie = zombie2;
					num = zombie2.mX;
				}
			}
		}
		return zombie;
	}

	public void CheckForCollision()
	{
		if (mMotionType == ProjectileMotion.Puff && mProjectileAge >= 75)
		{
			Die();
		}
		else if (mPosX > (float)(Constants.WIDE_BOARD_WIDTH + 40) || mPosX + (float)mWidth < 0f)
		{
			Die();
		}
		else if (mMotionType == ProjectileMotion.Homing || mMotionType == ProjectileMotion.EndoflameHoming)
		{
			Zombie zombie = mBoard.ZombieTryToGet(mTargetZombieID);
			if (zombie != null && zombie.EffectedByDamage((uint)mDamageRangeFlags))
			{
				TRect projectileRect = GetProjectileRect();
				TRect zombieRect = zombie.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(projectileRect, zombieRect);
				if (rectOverlap >= 0 && mPosY > (float)zombieRect.mY && mPosY < (float)(zombieRect.mY + zombieRect.mHeight))
				{
					DoImpact(zombie);
				}
			}
		}
		else if (mProjectileType == ProjectileType.Star && (mPosY > 600f || mPosY < 0f))
		{
			Die();
		}
		else
		{
			if (((mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.Star) && mShadowY - mPosY > 90f) || mMotionType == ProjectileMotion.FloatOver)
			{
				return;
			}
			ProjectileType projectileType = mProjectileType;
			if ((uint)(projectileType - 18) <= 2u)
			{
				return;
			}
			if (mProjectileType == ProjectileType.ZombiePea)
			{
				Plant plant = FindCollisionTargetPlant();
				if (plant != null)
				{
					ProjectileDefinition projectileDef = GetProjectileDef();
					plant.mPlantHealth -= GetProjectileDamage();
					plant.mEatenFlashCountdown = Math.Max(plant.mEatenFlashCountdown, 25);
					mApp.PlayFoley(FoleyType.Splat);
					mApp.AddTodParticle(mPosX - 3f, mPosY + 17f, mRenderOrder + 1, ParticleEffect.PeaSplat);
					Die();
				}
				Zombie zombie2 = FindCollisionMindControlledTarget();
				if (zombie2 != null && (!zombie2.mOnHighGround || !CantHitHighGround()))
				{
					DoImpact(zombie2);
				}
			}
			else
			{
				Zombie zombie = FindCollisionTarget();
				if (zombie != null && (!zombie.mOnHighGround || !CantHitHighGround()))
				{
					DoImpact(zombie);
				}
			}
		}
	}

	public Zombie FindCollisionTarget()
	{
		if (PeaAboutToHitTorchwood())
		{
			return null;
		}
		TRect projectileRect = GetProjectileRect();
		Zombie zombie = null;
		int num = 0;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie2 = mBoard.mZombies[i];
			if (zombie2.mDead || (zombie2.mZombiePhase == ZombiePhase.SnorkelWalkingInPool && !(mPosZ > 45f)) || (mProjectileType == ProjectileType.Star && mProjectileAge < 25 && !(mVelX < 0f) && zombie2.mZombieType == ZombieType.Digger) || !zombie2.EffectedByDamage((uint)mDamageRangeFlags) || (zombie2.mZombieType == ZombieType.Ninja && zombie2.mZombiePhase != ZombiePhase.NinjaShownByPlantern))
			{
				continue;
			}
			int num2 = zombie2.mRow - mRow;
			if (zombie2.mZombieType == ZombieType.Boss)
			{
				num2 = 0;
			}
			if (num2 == 0)
			{
				TRect zombieRect = zombie2.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(projectileRect, zombieRect);
				if (rectOverlap >= 0 && (zombie == null || zombie2.mX < num) && (mMotionType != ProjectileMotion.YAwareStraight || (mPosY + (float)mHeight > (float)zombieRect.mY && mPosY < (float)(zombieRect.mY + zombieRect.mHeight))))
				{
					zombie = zombie2;
					num = zombie2.mX;
				}
			}
		}
		return zombie;
	}

	public void UpdateLobMotion()
	{
		if (mProjectileType == ProjectileType.Cobbig && mPosZ < -700f)
		{
			mVelZ = 8f;
			mPosX = mCobTargetX;
			mRow = mCobTargetRow;
			int theGridX = mBoard.PixelToGridXKeepOnBoard((int)mCobTargetX, 0);
			mPosY = mBoard.GridToPixelY(theGridX, mCobTargetRow);
			mShadowY = mPosY + 67f;
			mRotation = -(float)Math.PI / 2f;
		}
		mVelZ += mAccZ;
		if (mApp.mGameMode == GameMode.ChallengeHighGravity)
		{
			mVelZ += mAccZ;
		}
		mPosX += mVelX;
		mPosY += mVelY;
		mPosZ += mVelZ;
		bool flag = mVelZ < 0f;
		if (flag && (mProjectileType == ProjectileType.Basketball || mProjectileType == ProjectileType.Cobbig))
		{
			return;
		}
		if (mProjectileAge > 20)
		{
			if (flag)
			{
				return;
			}
			float num = 0f;
			if (mProjectileType == ProjectileType.Butter)
			{
				num = -32f;
			}
			else if (mProjectileType == ProjectileType.Basketball)
			{
				num = 60f;
			}
			else if (mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon)
			{
				num = -35f;
			}
			else if (mProjectileType == ProjectileType.Cabbage || mProjectileType == ProjectileType.Kernel)
			{
				num = -30f;
			}
			else if (mProjectileType == ProjectileType.Cobbig)
			{
				num = -60f;
			}
			if (mBoard.mPlantRow[mRow] == PlantRowType.Pool)
			{
				num += 40f;
			}
			if (mPosZ <= num)
			{
				return;
			}
		}
		Plant plant = null;
		Zombie zombie = null;
		if (mProjectileType == ProjectileType.Basketball || mProjectileType == ProjectileType.ZombiePea)
		{
			plant = FindCollisionTargetPlant();
		}
		else
		{
			zombie = FindCollisionTarget();
		}
		float num2 = 80f;
		if (mProjectileType == ProjectileType.Cobbig)
		{
			num2 = -40f;
		}
		bool flag2 = mPosZ > num2;
		if (zombie == null && plant == null && !flag2)
		{
			return;
		}
		if (plant != null)
		{
			Plant plant2 = mBoard.FindUmbrellaPlant(plant.mPlantCol, plant.mRow);
			if (plant2 != null)
			{
				if (plant2.mState != PlantState.UmbrellaReflecting)
				{
					if (plant2.mState != PlantState.UmbrellaTriggered)
					{
						mApp.PlayFoley(FoleyType.Umbrella);
						plant2.DoSpecial();
					}
					return;
				}
				mApp.PlayFoley(FoleyType.Splat);
				mApp.AddTodParticle(mPosX + 20f, mPosY + 20f, 400001, ParticleEffect.UmbrellaReflect);
			}
			else
			{
				ProjectileDefinition projectileDef = GetProjectileDef();
				plant.mPlantHealth -= GetProjectileDamage();
				plant.mEatenFlashCountdown = Math.Max(plant.mEatenFlashCountdown, 25);
				mApp.PlayFoley(FoleyType.Splat);
			}
			Die();
		}
		else if (mProjectileType == ProjectileType.Cobbig)
		{
			int liveGargantuarCount = mBoard.GetLiveGargantuarCount();
			mBoard.KillAllZombiesInRadius(mRow, (int)mPosX + 80, (int)mPosY + 40, 115, 1, theBurn: true, mDamageRangeFlags);
			int liveGargantuarCount2 = mBoard.GetLiveGargantuarCount();
			mBoard.mGargantuarsKillsByCornCob += liveGargantuarCount - liveGargantuarCount2;
			if (mBoard.mGargantuarsKillsByCornCob >= 2)
			{
				mBoard.GrantAchievement(AchievementId.ACHIEVEMENT_POPCORN_PARTY, show: true);
			}
			DoImpact(null);
		}
		else
		{
			DoImpact(zombie);
		}
	}

	public void CheckForHighGround()
	{
		float num = mShadowY - mPosY;
		if ((mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.Snowpea || mProjectileType == ProjectileType.Fireball || mProjectileType == ProjectileType.Spike || mProjectileType == ProjectileType.CattailSpike || mProjectileType == ProjectileType.HypnoCattailSpike || mProjectileType == ProjectileType.Cobbig || mProjectileType == ProjectileType.EndoflameSpike || mProjectileType == ProjectileType.EndoflameFireball) && num < 28f)
		{
			DoImpact(null);
		}
		else if ((mProjectileType == ProjectileType.Puff || mProjectileType == ProjectileType.PuffGreen) && num < 0f)
		{
			DoImpact(null);
		}
		else if (mProjectileType == ProjectileType.Star && num < 23f)
		{
			DoImpact(null);
		}
		else if (CantHitHighGround())
		{
			int num2 = mBoard.PixelToGridXKeepOnBoard((int)mPosX + 30, (int)mPosY);
			if (mBoard.mGridSquareType[num2, mRow] == GridSquareType.HighGround)
			{
				DoImpact(null);
			}
		}
	}

	public bool CantHitHighGround()
	{
		if (mMotionType != ProjectileMotion.Backwards && mMotionType != ProjectileMotion.Homing && mMotionType != ProjectileMotion.EndoflameHoming && (mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.Snowpea || mProjectileType == ProjectileType.Star || mProjectileType == ProjectileType.Puff || mProjectileType == ProjectileType.PuffGreen || mProjectileType == ProjectileType.Fireball))
		{
			return !mOnHighGround;
		}
		return false;
	}

	public void DoSplashDamage(Zombie theZombie)
	{
		ProjectileDefinition projectileDef = GetProjectileDef();
		int num = 0;
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && theZombie != zombie && IsZombieHitBySplash(zombie))
			{
				num++;
			}
		}
		int projectileDamage = GetProjectileDamage();
		int num2 = 3;
		int num3 = projectileDamage / num2;
		int num4 = projectileDamage * 7;
		if (mProjectileType == ProjectileType.Fireball)
		{
			num4 = projectileDamage;
		}
		int num5 = num3 * num;
		if (num5 > num4)
		{
			num3 = Math.Max(1, projectileDamage * num4 / (num5 * num2));
		}
		for (int j = 0; j < count; j++)
		{
			Zombie zombie2 = mBoard.mZombies[j];
			if (!zombie2.mDead && IsZombieHitBySplash(zombie2))
			{
				uint damageFlags = GetDamageFlags(zombie2);
				if (theZombie == zombie2)
				{
					zombie2.TakeDamage(projectileDamage, damageFlags);
				}
				else
				{
					zombie2.TakeDamage(num3, damageFlags);
				}
			}
		}
	}

	public ProjectileDefinition GetProjectileDef()
	{
		ProjectileDefinition projectileDefinition = GameConstants.gProjectileDefinition[(int)mProjectileType];
		Debug.ASSERT(projectileDefinition.mProjectileType == mProjectileType);
		return projectileDefinition;
	}

	public int GetProjectileDamage()
	{
		int result = GetProjectileDef().mDamage;
		CSOverrideProjectileProperty cSOverrideProjectileProperty = mApp.mCreativeLevel?.GetComponent<CSOverrideProjectileProperty>();
		if (cSOverrideProjectileProperty != null && cSOverrideProjectileProperty.mProperties.TryGetValue(mProjectileType, out var value) && value.mDamage.HasValue)
		{
			result = value.mDamage.GetValueOrDefault();
		}
		return result;
	}

	public uint GetDamageFlags(Zombie theZombie)
	{
		uint theNumber = 0u;
		if (IsSplashDamage(theZombie))
		{
			TodCommon.SetBit(ref theNumber, 1, 1);
		}
		else if (mMotionType == ProjectileMotion.Lobbed || mMotionType == ProjectileMotion.Backwards)
		{
			TodCommon.SetBit(ref theNumber, 0, 1);
		}
		else if (mMotionType == ProjectileMotion.Star && mVelX < 0f)
		{
			TodCommon.SetBit(ref theNumber, 0, 1);
		}
		if (mProjectileType == ProjectileType.Snowpea || mProjectileType == ProjectileType.Wintermelon)
		{
			TodCommon.SetBit(ref theNumber, 2, 1);
		}
		return theNumber;
	}

	public TRect GetProjectileRect()
	{
		if (mProjectileType == ProjectileType.Pea || mProjectileType == ProjectileType.Snowpea || mProjectileType == ProjectileType.ZombiePea || mProjectileType == ProjectileType.ZombiePeaMindControl)
		{
			IsFromRepeatess();
			return new TRect(mX - 15, mY, mWidth + 15, mHeight);
		}
		if (mProjectileType == ProjectileType.Cobbig)
		{
			int num = 115;
			return new TRect(mX + mWidth / 2 - num, mY + mHeight / 2 - num, num * 2, num * 2);
		}
		if (mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon)
		{
			return new TRect(mX + 20, mY, 60, mHeight);
		}
		if (mProjectileType == ProjectileType.Fireball)
		{
			return new TRect(mX, mY, mWidth - 10, mHeight);
		}
		if (mProjectileType == ProjectileType.Spike || mProjectileType == ProjectileType.CattailSpike || mProjectileType == ProjectileType.HypnoCattailSpike)
		{
			return new TRect(mX - 25, mY, mWidth + 25, mHeight);
		}
		if (mProjectileType == ProjectileType.EndoflameSpike || mProjectileType == ProjectileType.EndoflameFireball)
		{
			return new TRect(mX - 25, mY, mWidth + 25, mHeight);
		}
		return new TRect(mX, mY, mWidth, mHeight);
	}

	public void EndoflameRetarget()
	{
		Zombie zombie = null;
		float num = 0f;
		for (int i = 0; i < mBoard.mZombies.Count; i++)
		{
			Zombie zombie2 = mBoard.mZombies[i];
			if (zombie2.mDead || !zombie2.mHasHead || zombie2.IsTangleKelpTarget())
			{
				continue;
			}
			bool flag = false;
			if (!zombie2.EffectedByDamage((uint)mDamageRangeFlags))
			{
				continue;
			}
			int num2 = 0;
			num2 = Constants.Board_Offset_AspectRatio_Correction;
			if (zombie2.mZombiePhase != ZombiePhase.PolevaulterInVault)
			{
				TRect zombieRect = zombie2.GetZombieRect();
				int num3 = -zombieRect.mX;
				num3 = -(int)TodCommon.Distance2D(mX, mY, zombieRect.mX + zombieRect.mWidth / 2, zombieRect.mY + zombieRect.mHeight / 2);
				if (((!((float)zombie2.mX < (float)mX + 40f) && zombie2.mHasHead) || zombie2.mZombieType == ZombieType.Boss) && (zombie == null || (float)num3 > num))
				{
					num = num3;
					zombie = zombie2;
				}
			}
		}
		mTargetZombieID = zombie;
	}

	public void UpdateNormalMotion()
	{
		if (mMotionType == ProjectileMotion.Backwards)
		{
			mPosX -= 3.33f;
		}
		else if (mMotionType == ProjectileMotion.Homing)
		{
			Zombie zombie = mBoard.ZombieTryToGet(mTargetZombieID);
			if (zombie != null && zombie.EffectedByDamage((uint)mDamageRangeFlags))
			{
				TRect zombieRect = zombie.GetZombieRect();
				SexyVector2 sexyVector = new SexyVector2(zombie.ZombieTargetLeadX(0f), zombieRect.mY + zombieRect.mHeight / 2);
				SexyVector2 sexyVector2 = new SexyVector2(mPosX + (float)(mWidth / 2), mPosY + (float)(mHeight / 2));
				SexyVector2 sexyVector3 = (sexyVector - sexyVector2).Normalize();
				SexyVector2 sexyVector4 = new SexyVector2(mVelX, mVelY);
				sexyVector4.mVector += sexyVector3.mVector * (0.001f * (float)mProjectileAge);
				sexyVector4 = sexyVector4.Normalize();
				sexyVector4.mVector *= 2f;
				mVelX = sexyVector4.x;
				mVelY = sexyVector4.y;
				mRotation = 0f - (float)Math.Atan2(mVelY, mVelX);
			}
			mPosY += mVelY;
			mPosX += mVelX;
			mShadowY += mVelY;
			mRow = mBoard.PixelToGridYKeepOnBoard((int)mPosX, (int)mPosY);
		}
		else if (mMotionType == ProjectileMotion.EndoflameHoming)
		{
			Zombie zombie2 = mBoard.ZombieTryToGet(mTargetZombieID);
			if (zombie2 != null && zombie2.EffectedByDamage((uint)mDamageRangeFlags) && zombie2.mHasHead)
			{
				float num = mBoard.GetPosYBasedOnRow(0f, zombie2.mRow) - mBoard.GetPosYBasedOnRow(0f, mRow);
				float num2 = mBoard.GetPosYBasedOnRow(zombie2.mPosX, zombie2.mRow) - mBoard.GetPosYBasedOnRow(mPosX, mRow) - num;
				TRect zombieRect2 = zombie2.GetZombieRect();
				SexyVector2 sexyVector5 = new SexyVector2(zombie2.ZombieTargetLeadX(0f), (float)(zombieRect2.mY + zombieRect2.mHeight / 2) - num2);
				SexyVector2 sexyVector6 = new SexyVector2(mPosX + (float)(mWidth / 2), mPosY + (float)(mHeight / 2));
				SexyVector2 sexyVector7 = (sexyVector5 - sexyVector6).Normalize();
				SexyVector2 sexyVector8 = new SexyVector2(mVelX, mVelY);
				sexyVector8.mVector += sexyVector7.mVector * (0.001f * (float)mProjectileAge);
				sexyVector8 = sexyVector8.Normalize();
				sexyVector8.mVector *= 2f;
				mVelX = sexyVector8.x;
				mVelY = sexyVector8.y;
				mRotation = 0f - (float)Math.Atan2(mVelY, mVelX);
			}
			else
			{
				EndoflameRetarget();
			}
			mPosY += mVelY;
			mPosX += mVelX;
			mShadowY += mVelY;
			mRow = mBoard.PixelToGridYKeepOnBoard((int)mPosX, (int)mPosY);
		}
		else if (mMotionType == ProjectileMotion.Star)
		{
			mPosY += mVelY;
			mPosX += mVelX;
			mShadowY += mVelY;
			if (mVelY != 0f)
			{
				int num3 = mBoard.PixelToGridYKeepOnBoard((int)mPosX, (int)mPosY);
				mRow = num3;
			}
		}
		else if (mMotionType == ProjectileMotion.Bee)
		{
			if (mProjectileAge < 60)
			{
				mPosY -= 0.5f;
			}
			mPosX += 3.33f;
		}
		else if (mMotionType == ProjectileMotion.FloatOver)
		{
			if (mVelZ < 0f)
			{
				mVelZ += 0.002f;
				mVelZ = Math.Min(mVelZ, 0f);
				mPosY += mVelZ;
				mRotation = 0.3f + -0.70000005f * mVelZ * (float)Math.PI * 0.25f;
			}
			mPosX += 0.2f;
		}
		else if (mMotionType == ProjectileMotion.BeeBackwards)
		{
			if (mProjectileAge < 60)
			{
				mPosY -= 0.5f;
			}
			mPosX -= 3.33f;
		}
		else if (mMotionType == ProjectileMotion.Threepeater)
		{
			mPosX += 3.33f;
			mPosY += mVelY;
			mVelY *= 0.97f;
			mShadowY += mVelY;
		}
		else
		{
			ProjectileType projectileType = mProjectileType;
			if ((uint)(projectileType - 18) <= 2u)
			{
				mPosX += 15f;
			}
			else
			{
				mPosX += 3.33f;
			}
		}
		if (mApp.mGameMode == GameMode.ChallengeHighGravity)
		{
			if (mMotionType == ProjectileMotion.FloatOver)
			{
				mVelZ += 0.004f;
			}
			else
			{
				mVelZ += 0.2f;
			}
			mPosY += mVelZ;
		}
		CheckForCollision();
		CheckForHighGround();
	}

	public Plant FindCollisionTargetPlant()
	{
		TRect projectileRect = GetProjectileRect();
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (plant.mDead || mRow != plant.mRow || ((mProjectileType == ProjectileType.ZombiePea || mProjectileType == ProjectileType.ZombiePeaMindControl) && (plant.mSeedType == SeedType.Puffshroom || plant.mSeedType == SeedType.Sunshroom || plant.mSeedType == SeedType.Potatomine || plant.mSeedType == SeedType.Spikeweed || plant.mSeedType == SeedType.Spikerock || plant.mSeedType == SeedType.Lilypad)))
			{
				continue;
			}
			if ((mProjectileType == ProjectileType.ZombiePea || mProjectileType == ProjectileType.ZombiePeaMindControl) && plant.mSeedType == SeedType.Flowerpot)
			{
				CSOverrideProjectileProperty cSOverrideProjectileProperty = mApp.mCreativeLevel?.GetComponent<CSOverrideProjectileProperty>();
				if (cSOverrideProjectileProperty != null && cSOverrideProjectileProperty.mAllowZombiePeaAttackFlowerPot == false)
				{
					continue;
				}
			}
			TRect plantRect = plant.GetPlantRect();
			int rectOverlap = GameConstants.GetRectOverlap(projectileRect, plantRect);
			if (rectOverlap > 8)
			{
				if (mProjectileType == ProjectileType.ZombiePea)
				{
					return mBoard.GetTopPlantAt(plant.mPlantCol, plant.mRow, TopPlant.EatingOrder);
				}
				return mBoard.GetTopPlantAt(plant.mPlantCol, plant.mRow, TopPlant.CatapultOrder);
			}
		}
		return null;
	}

	public void ConvertToFireball()
	{
		mProjectileType = ProjectileType.Fireball;
		mApp.PlayFoley(FoleyType.Firepea);
		float num = (IsFromRepeatess() ? 0.5f : 1f);
		bool flag = mMotionType == ProjectileMotion.Backwards;
		float num2 = -25f * num;
		float num3 = -25f * num;
		Reanimation reanimation = mApp.AddReanimation(0f, 0f, 0, ReanimationType.FirePea);
		reanimation.OverrideScale(flag ? (0f - num) : num, num);
		if (flag)
		{
			num2 += 80f * num;
		}
		reanimation.SetPosition((mPosX + num2) * Constants.S, (mPosY + num3) * Constants.S);
		reanimation.mLoopType = ReanimLoopType.Loop;
		reanimation.mAnimRate = TodCommon.RandRangeFloat(50f, 80f);
		GlobalMembersAttachment.AttachReanim(ref mAttachmentID, reanimation, num2, num3);
	}

	public void ConvertToFireball(int aGridX)
	{
		if (mHitTorchwoodGridX != aGridX)
		{
			mHitTorchwoodGridX = aGridX;
			ConvertToFireball();
		}
	}

	public void ConvertToEndoflameFireball(int aGridX)
	{
		if (mHitTorchwoodGridX != aGridX)
		{
			mProjectileType = ProjectileType.EndoflameFireball;
			mHitTorchwoodGridX = aGridX;
			mApp.PlayFoley(FoleyType.Firepea);
			Reanimation reanimation = mApp.ReanimationGet(mProjectileReanimID);
			reanimation.SetFramesForLayer("anim_burning");
		}
	}

	public void ConvertToPea(int aGridX)
	{
		if (mHitTorchwoodGridX != aGridX)
		{
			GlobalMembersAttachment.AttachmentDie(ref mAttachmentID);
			mProjectileType = ProjectileType.Pea;
			mHitTorchwoodGridX = aGridX;
			mApp.PlayFoley(FoleyType.Throw);
		}
	}

	public bool IsSplashDamage(Zombie theZombie)
	{
		if (((mProjectileType != ProjectileType.Fireball && mProjectileType != ProjectileType.EndoflameFireball) || theZombie == null || !theZombie.IsFireResistant()) && (mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon || mProjectileType == ProjectileType.Fireball || mProjectileType == ProjectileType.EndoflameFireball))
		{
			if (theZombie == null)
			{
				return true;
			}
			return theZombie.mHelmType != HelmType.Bell;
		}
		return false;
	}

	public void PlayImpactSound(Zombie theZombie)
	{
		bool flag = true;
		bool flag2 = true;
		if (mProjectileType == ProjectileType.Kernel)
		{
			mApp.PlayFoley(FoleyType.KernelSplat);
			flag = false;
			flag2 = false;
		}
		else if (mProjectileType == ProjectileType.Butter)
		{
			mApp.PlayFoley(FoleyType.Butter);
			flag2 = false;
		}
		else if ((mProjectileType == ProjectileType.Fireball || mProjectileType == ProjectileType.EndoflameFireball) && IsSplashDamage(theZombie))
		{
			mApp.PlayFoley(FoleyType.Ignite);
			flag = false;
			flag2 = false;
		}
		else if (mProjectileType == ProjectileType.Melon || mProjectileType == ProjectileType.Wintermelon)
		{
			mApp.PlayFoley(FoleyType.Melonimpact);
			flag2 = false;
		}
		else if (mProjectileType == ProjectileType.CattailSpike || mProjectileType == ProjectileType.HypnoCattailSpike)
		{
			mApp.PlayFoley(FoleyType.CattailSplat);
			flag2 = false;
		}
		if (flag)
		{
			if (theZombie != null && theZombie.mHelmType == HelmType.Pail)
			{
				mApp.PlayFoley(FoleyType.ShieldHit);
				flag2 = false;
			}
			else if (theZombie != null && theZombie.mHelmType == HelmType.TrafficCone)
			{
				mApp.PlayFoley(FoleyType.PlasticHit);
			}
			else if (theZombie != null && theZombie.mHelmType == HelmType.Digger)
			{
				mApp.PlayFoley(FoleyType.PlasticHit);
			}
			else if (theZombie != null && theZombie.mHelmType == HelmType.Football)
			{
				mApp.PlayFoley(FoleyType.PlasticHit);
			}
			else if (theZombie != null && theZombie.mZombieType == ZombieType.Propeller && theZombie.IsFlying())
			{
				mApp.PlayFoley(FoleyType.ShieldHit);
				flag2 = false;
			}
		}
		if (flag2)
		{
			mApp.PlayFoley(FoleyType.Splat);
		}
	}

	public bool IsZombieHitBySplash(Zombie theZombie)
	{
		TRect projectileRect = GetProjectileRect();
		if (mProjectileType == ProjectileType.Fireball)
		{
			projectileRect.mWidth = 100;
		}
		int num = theZombie.mRow - mRow;
		TRect zombieRect = theZombie.GetZombieRect();
		if (theZombie.IsFireResistant() && mProjectileType == ProjectileType.Fireball)
		{
			return false;
		}
		if (theZombie.mZombieType == ZombieType.Boss)
		{
			num = 0;
		}
		if (mProjectileType == ProjectileType.Fireball)
		{
			if (num != 0)
			{
				return false;
			}
		}
		else if (num > 1 || num < -1)
		{
			return false;
		}
		if (!theZombie.EffectedByDamage((uint)mDamageRangeFlags))
		{
			return false;
		}
		int rectOverlap = GameConstants.GetRectOverlap(projectileRect, zombieRect);
		return rectOverlap >= 0;
	}

	public bool PeaAboutToHitTorchwood()
	{
		if (mMotionType != ProjectileMotion.Straight)
		{
			return false;
		}
		if (mProjectileType != ProjectileType.Pea && mProjectileType != ProjectileType.Snowpea && mProjectileType != ProjectileType.ZombiePeaMindControl)
		{
			return false;
		}
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mSeedType == SeedType.Torchwood && plant.mRow == mRow && !plant.NotOnGround() && mHitTorchwoodGridX != plant.mPlantCol)
			{
				TRect plantAttackRect = plant.GetPlantAttackRect(PlantWeapon.Primary);
				TRect projectileRect = GetProjectileRect();
				projectileRect.mX += 40;
				int rectOverlap = GameConstants.GetRectOverlap(plantAttackRect, projectileRect);
				if (rectOverlap > 10)
				{
					return true;
				}
			}
		}
		return false;
	}
}
