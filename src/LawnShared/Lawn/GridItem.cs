using System;
using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class GridItem
{
	public LawnApp mApp;

	public Board mBoard;

	public GridItemType mGridItemType;

	public GridItemState mGridItemState;

	public int mGridX;

	public int mGridY;

	public int mGridItemCounter;

	public int mRenderOrder;

	public bool mDead;

	public float mPosX;

	public float mPosY;

	public float mGoalX;

	public float mGoalY;

	public Reanimation mGridItemReanimID;

	public int mGridItemReanimID_Save;

	public TodParticleSystem mGridItemParticleID;

	public int mGridItemParticleID_Save;

	public ZombieType mZombieType;

	public SeedType mSeedType;

	public ScaryPotType mScaryPotType;

	public bool mHighlighted;

	public int mTransparentCounter;

	public int mSunCount;

	public MotionTrailFrame[] mMotionTrailFrames = new MotionTrailFrame[12];

	public int mMotionTrailCount;

	private static Stack<GridItem> unusedObjects = new Stack<GridItem>();

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncEnum(ref mGridItemType, theContext.aGridItemTypeSaver, SaveFileVersion.Initial, GridItemType.None);
		theContext.SyncEnum(ref mGridItemState, theContext.aGridItemStateSaver, SaveFileVersion.Initial, GridItemState.Normal);
		theContext.SyncInt(ref mGridX);
		theContext.SyncInt(ref mGridY);
		theContext.SyncInt(ref mGridItemCounter);
		theContext.SyncInt(ref mRenderOrder);
		theContext.SyncBool(ref mDead);
		theContext.SyncFloat(ref mPosX);
		theContext.SyncFloat(ref mPosY);
		theContext.SyncFloat(ref mGoalX);
		theContext.SyncFloat(ref mGoalY);
		theContext.SyncInt(ref mGridItemReanimID_Save);
		theContext.SyncInt(ref mGridItemParticleID_Save);
		theContext.SyncEnum(ref mZombieType, theContext.aZombieTypeSaver, SaveFileVersion.Initial, ZombieType.Normal);
		theContext.SyncEnum(ref mSeedType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncEnum(ref mScaryPotType, theContext.aScaryPotTypeSaver, SaveFileVersion.Initial, ScaryPotType.None);
		theContext.SyncBool(ref mHighlighted);
		theContext.SyncInt(ref mTransparentCounter);
		theContext.SyncInt(ref mSunCount);
		theContext.SyncInt(ref mMotionTrailCount);
		for (int i = 0; i < 12; i++)
		{
			bool theBool = mMotionTrailFrames[i] != null;
			theContext.SyncBool(ref theBool);
			if (theBool)
			{
				MotionTrailFrame[] array = mMotionTrailFrames;
				int num = i;
				if (array[num] == null)
				{
					array[num] = new MotionTrailFrame();
				}
				mMotionTrailFrames[i].Sync(theContext);
			}
		}
	}

	internal void SyncObj(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		if (theContext.mReading)
		{
			mApp = GlobalStaticVars.gLawnApp;
			mBoard = mApp.mBoard;
		}
		if (theContext.mVersion < SaveFileVersion.NewLayout600)
		{
			mPosX -= 100f;
		}
		TodLibObjSyncer.SyncObjFromList(ref mGridItemReanimID_Save, ref mGridItemReanimID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mGridItemParticleID_Save, ref mGridItemParticleID, mApp.mEffectSystem.mParticleHolder.mParticleSystems, theContext.mReading);
	}

	public static GridItem GetNewGridItem()
	{
		if (unusedObjects.Count > 0)
		{
			GridItem gridItem = unusedObjects.Pop();
			gridItem.Reset();
			return gridItem;
		}
		return new GridItem();
	}

	public void PrepareForReuse()
	{
		unusedObjects.Push(this);
	}

	private GridItem()
	{
		Reset();
	}

	private void Reset()
	{
		mGridItemState = GridItemState.Normal;
		for (int i = 0; i < mMotionTrailFrames.Length; i++)
		{
			mMotionTrailFrames[i] = new MotionTrailFrame();
		}
		mApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		mBoard = mApp.mBoard;
		mGridItemType = GridItemType.None;
		mGridX = 0;
		mGridY = 0;
		mGridItemCounter = 0;
		mRenderOrder = 0;
		mDead = false;
		mPosX = 0f;
		mPosY = 0f;
		mGoalX = 0f;
		mGoalY = 0f;
		mGridItemReanimID = null;
		mGridItemParticleID = null;
		mZombieType = ZombieType.Invalid;
		mSeedType = SeedType.None;
		mScaryPotType = ScaryPotType.None;
		mHighlighted = false;
		mTransparentCounter = 0;
		mSunCount = 0;
		mMotionTrailCount = 0;
	}

	public bool SaveToFile(Sexy.Buffer b)
	{
		try
		{
			b.WriteBoolean(mDead);
			b.WriteFloat(mGoalX);
			b.WriteFloat(mGoalY);
			b.WriteLong(mGridItemCounter);
			b.WriteLong((int)mGridItemState);
			b.WriteLong((int)mGridItemType);
			b.WriteLong(mGridX);
			b.WriteLong(mGridY);
			b.WriteBoolean(mHighlighted);
			b.WriteLong(mMotionTrailCount);
			for (int i = 0; i < mMotionTrailCount; i++)
			{
				mMotionTrailFrames[i].SaveToFile(b);
			}
			b.WriteFloat(mPosX);
			b.WriteFloat(mPosY);
			b.WriteLong(mRenderOrder);
			b.WriteLong((int)mScaryPotType);
			b.WriteLong((int)mSeedType);
			b.WriteLong(mSunCount);
			b.WriteLong(mTransparentCounter);
			b.WriteLong((int)mZombieType);
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
		return true;
	}

	public bool LoadFromFile(Sexy.Buffer b)
	{
		try
		{
			mDead = b.ReadBoolean();
			mGoalX = b.ReadFloat();
			mGoalY = b.ReadFloat();
			mGridItemCounter = b.ReadLong();
			mGridItemState = (GridItemState)b.ReadLong();
			mGridItemType = (GridItemType)b.ReadLong();
			mGridX = b.ReadLong();
			mGridY = b.ReadLong();
			mHighlighted = b.ReadBoolean();
			mMotionTrailCount = b.ReadLong();
			Array.Clear(mMotionTrailFrames, 0, mMotionTrailFrames.Length);
			for (int i = 0; i < mMotionTrailCount; i++)
			{
				mMotionTrailFrames[i] = new MotionTrailFrame();
				mMotionTrailFrames[i].LoadFromFile(b);
			}
			mPosX = b.ReadFloat();
			mPosY = b.ReadFloat();
			mRenderOrder = b.ReadLong();
			mScaryPotType = (ScaryPotType)b.ReadLong();
			mSeedType = SeedTypeLegacy.FromInt(b.ReadLong());
			mSunCount = b.ReadLong();
			mTransparentCounter = b.ReadLong();
			mZombieType = (ZombieType)b.ReadLong();
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
		return true;
	}

	public void LoadingComplete()
	{
		mApp = GlobalStaticVars.gLawnApp;
		mBoard = mApp.mBoard;
		if (mGridItemType == GridItemType.Rake)
		{
			mGridItemReanimID = mBoard.CreateRakeReanim(mPosX, mPosY, mRenderOrder);
		}
		if (mGridItemType == GridItemType.PortalCircle || mGridItemType == GridItemType.PortalSquare)
		{
			OpenPortal();
		}
	}

	public void DrawLadder(Graphics g)
	{
		int num = mBoard.GridToPixelX(mGridX, mGridY);
		int num2 = mBoard.GridToPixelY(mGridX, mGridY);
		TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_REANIM_ZOMBIE_LADDER_5, ((float)num + 25f) * Constants.S, ((float)num2 - 4f) * Constants.S, 0.8f, 0.8f);
	}

	public void DrawCrater(Graphics g)
	{
		float num = (float)mBoard.GridToPixelX(mGridX, mGridY) - 8f;
		float num2 = (float)mBoard.GridToPixelY(mGridX, mGridY) + 40f;
		if (mGridItemCounter < 25)
		{
			int theAlpha = TodCommon.TodAnimateCurve(25, 0, mGridItemCounter, 255, 0, TodCurves.Linear);
			g.SetColor(new SexyColor(255, 255, 255, theAlpha));
			g.SetColorizeImages(colorizeImages: true);
		}
		bool flag = mGridItemCounter < 9000;
		Image theImageStrip = AtlasResources.IMAGE_CRATER;
		int theCelCol = 0;
		if (mBoard.IsPoolSquare(mGridX, mGridY))
		{
			theImageStrip = ((!mBoard.StageIsNight()) ? AtlasResources.IMAGE_CRATER_WATER_DAY : AtlasResources.IMAGE_CRATER_WATER_NIGHT);
			if (flag)
			{
				theCelCol = 1;
			}
			float num3 = (float)mGridY * (float)Math.PI + (float)mGridX * (float)Math.PI * 0.25f;
			float num4 = (float)mBoard.mMainCounter * (float)Math.PI * 2f / 200f;
			float num5 = (float)Math.Sin(num3 + num4) * 2f;
			num2 += num5;
		}
		else if (mBoard.StageHasRoof())
		{
			if (mGridX < 5)
			{
				theImageStrip = AtlasResources.IMAGE_CRATER_ROOF_LEFT;
				num += 16f;
				num2 += -16f;
			}
			else
			{
				theImageStrip = AtlasResources.IMAGE_CRATER_ROOF_CENTER;
				num += 18f;
				num2 += -9f;
			}
			if (flag)
			{
				theCelCol = 1;
			}
		}
		else if (mBoard.StageIsNight())
		{
			theCelCol = 1;
			if (flag)
			{
				theImageStrip = AtlasResources.IMAGE_CRATER_FADING;
			}
		}
		else if (flag)
		{
			theImageStrip = AtlasResources.IMAGE_CRATER_FADING;
		}
		num = TodCommon.PixelAligned(num);
		num2 = TodCommon.PixelAligned(num2);
		TodCommon.TodDrawImageCelF(g, theImageStrip, num * Constants.S, num2 * Constants.S, theCelCol, 0);
		g.SetColorizeImages(colorizeImages: false);
	}

	public void DrawGraveStone(Graphics g)
	{
		if (mGridItemCounter > 0)
		{
			int theTimeAge = TodCommon.TodAnimateCurve(0, 100, mGridItemCounter, 1000, 0, TodCurves.EaseInOut);
			int num = mBoard.mGridCelLook[mGridX, mGridY];
			int num2 = mBoard.mGridCelOffset[mGridX, mGridY, 0];
			int num3 = mBoard.mGridCelOffset[mGridX, mGridY, 1];
			int num4 = (int)((float)AtlasResources.IMAGE_TOMBSTONES.GetCelWidth() * Constants.IS);
			int num5 = (int)((float)AtlasResources.IMAGE_TOMBSTONES.GetCelHeight() * Constants.IS);
			int num6 = num % 5;
			int num7 = ((mGridY == 0) ? 1 : ((mGridItemState != GridItemState.GravestoneSpecial) ? (2 + num % 2) : 0));
			int num8 = TodCommon.TodAnimateCurve(0, 1000, theTimeAge, num5, 0, TodCurves.EaseInOut);
			int num9 = TodCommon.TodAnimateCurve(0, 50, theTimeAge, 0, 14, TodCurves.EaseInOut);
			int num10 = TodCommon.TodAnimateCurve(500, 1000, theTimeAge, num5, 0, TodCurves.EaseInOut);
			int num11 = 0;
			Plant topPlantAt = mBoard.GetTopPlantAt(mGridX, mGridY, TopPlant.OnlyNormalPosition);
			if (topPlantAt != null && topPlantAt.mState == PlantState.GravebusterEating)
			{
				num11 = TodCommon.TodAnimateCurve(400, 0, topPlantAt.mStateCountdown, 10, 40, TodCurves.Linear);
			}
			TRect theSrcRect = new TRect((int)((float)(num4 * num6) * Constants.S), (int)((float)(num5 * num7 + num11) * Constants.S), (int)((float)num4 * Constants.S), (int)((float)(num8 - num9 - num11) * Constants.S));
			int num12 = mBoard.GridToPixelX(mGridX, mGridY) - 4 + num2;
			int num13 = mBoard.GridToPixelY(mGridX, mGridY) + num5 + num3;
			g.DrawImage(AtlasResources.IMAGE_TOMBSTONES, (int)((float)num12 * Constants.S), (int)((double)((float)(num13 - num8 + num11) * Constants.S) + 0.5), theSrcRect);
			int num14 = (int)((float)num10 * Constants.S);
			if (num14 >= (int)Constants.InvertAndScale(34f))
			{
				g.DrawImage(theSrcRect: new TRect(AtlasResources.IMAGE_TOMBSTONE_MOUNDS.GetCelWidth() * num6, AtlasResources.IMAGE_TOMBSTONE_MOUNDS.GetCelHeight() * num7, AtlasResources.IMAGE_TOMBSTONE_MOUNDS.GetCelWidth(), (int)((float)num10 * Constants.S) - (int)Constants.InvertAndScale(34f)), theImage: AtlasResources.IMAGE_TOMBSTONE_MOUNDS, theX: (int)((float)num12 * Constants.S), theY: (int)((float)(num13 - num10) * Constants.S) + (int)Constants.InvertAndScale(34f));
			}
		}
	}

	public void GridItemDie()
	{
		mDead = true;
		Reanimation reanimation = mApp.ReanimationTryToGet(mGridItemReanimID);
		if (reanimation != null)
		{
			reanimation.ReanimationDie();
			mGridItemReanimID = null;
		}
		TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(mGridItemParticleID);
		if (todParticleSystem != null)
		{
			todParticleSystem.ParticleSystemDie();
			mGridItemParticleID = null;
		}
	}

	public void AddGraveStoneParticles()
	{
		int num = mBoard.mGridCelOffset[mGridX, mGridY, 0];
		int num2 = mBoard.mGridCelOffset[mGridX, mGridY, 1];
		int num3 = mBoard.GridToPixelX(mGridX, mGridY) + 14 + num;
		int num4 = mBoard.GridToPixelY(mGridX, mGridY) + 78 + num2;
		mApp.AddTodParticle(num3, num4, mRenderOrder + 1, ParticleEffect.GraveStoneRise);
		mApp.PlayFoley(FoleyType.DirtRise);
	}

	public void DrawGridItem(Graphics g)
	{
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		switch (mGridItemType)
		{
		case GridItemType.Gravestone:
			DrawGraveStone(g);
			break;
		case GridItemType.Crater:
			DrawCrater(g);
			break;
		case GridItemType.Ladder:
			DrawLadder(g);
			break;
		case GridItemType.Brain:
			g.DrawImageF(AtlasResources.IMAGE_BRAIN, mPosX * Constants.S, mPosY * Constants.S);
			break;
		case GridItemType.ScaryPot:
			DrawScaryPot(g);
			break;
		case GridItemType.Squirrel:
			DrawSquirrel(g);
			break;
		case GridItemType.Stinky:
			DrawStinky(g);
			return;
		case GridItemType.IzombieBrain:
			DrawIZombieBrain(g);
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		case GridItemType.PortalCircle:
		case GridItemType.PortalSquare:
		case GridItemType.ZenTool:
		case GridItemType.Rake:
		case GridItemType.Talisman:
		case GridItemType.TalismanMove:
			break;
		}
		mApp.ReanimationTryToGet(mGridItemReanimID)?.Draw(g);
		mApp.ParticleTryToGet(mGridItemParticleID)?.Draw(g, doScale: true);
	}

	public void DrawGridItemOverlay(Graphics g)
	{
		GridItemType gridItemType = mGridItemType;
		if (gridItemType == GridItemType.Stinky && mBoard.mCursorObject.mCursorType == CursorType.Chocolate && !mApp.mZenGarden.IsStinkyHighOnChocolate())
		{
			g.DrawImage(AtlasResources.IMAGE_PLANTSPEECHBUBBLE, (float)Constants.ZenGarden_StinkySpeechBubble_Pos.X + Constants.S * mPosX, (float)Constants.ZenGarden_StinkySpeechBubble_Pos.Y + Constants.S * mPosY);
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_CHOCOLATE, (float)Constants.ZenGarden_Chocolate_Pos.X + mPosX * Constants.S, (float)Constants.ZenGarden_Chocolate_Pos.Y + mPosY * Constants.S, 0.44f, 0.44f);
		}
	}

	public void OpenPortal()
	{
		float num = (float)mGridX * 80f - 6f + (float)Constants.BOARD_EXTRA_ROOM;
		float num2 = (float)mBoard.GridToPixelY(0, mGridY) - 65f;
		Reanimation reanimation = mApp.ReanimationTryToGet(mGridItemReanimID);
		if (reanimation == null)
		{
			ReanimationType theReanimationType = ReanimationType.PortalCircle;
			if (mGridItemType == GridItemType.PortalSquare)
			{
				num2 += 25f;
				num += -4f;
				theReanimationType = ReanimationType.PortalSquare;
			}
			reanimation = mApp.AddReanimation(num, num2, 0, theReanimationType);
			reanimation.mIsAttachment = true;
			mGridItemReanimID = mApp.ReanimationGetID(reanimation);
		}
		else
		{
			reanimation.SetPosition(num * Constants.S, num2 * Constants.S);
		}
		TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(mGridItemParticleID);
		if (todParticleSystem != null)
		{
			todParticleSystem.ParticleSystemDie();
			mGridItemParticleID = null;
		}
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_appear, ReanimLoopType.PlayOnceAndHold, 0, 12f);
		mApp.PlayFoley(FoleyType.Portal);
	}

	public void Update()
	{
		mApp.ReanimationTryToGet(mGridItemReanimID)?.Update();
		mApp.ParticleTryToGet(mGridItemParticleID)?.Update();
		if (mGridItemType == GridItemType.PortalCircle || mGridItemType == GridItemType.PortalSquare)
		{
			UpdatePortal();
		}
		if (mGridItemType == GridItemType.ScaryPot)
		{
			UpdateScaryPot();
		}
		if (mGridItemType == GridItemType.Rake)
		{
			UpdateRake();
		}
		if (mGridItemType == GridItemType.IzombieBrain)
		{
			UpdateBrain();
		}
		if (mGridItemType == GridItemType.Talisman)
		{
			UpdateTalisman();
		}
		if (mGridItemType == GridItemType.TalismanMove)
		{
			UpdateTalismanMove();
		}
	}

	public void ClosePortal()
	{
		mApp.ReanimationTryToGet(mGridItemReanimID)?.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_dissapear, ReanimLoopType.PlayOnceAndHold, 0, 12f);
		TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(mGridItemParticleID);
		if (todParticleSystem != null)
		{
			todParticleSystem.ParticleSystemDie();
			mGridItemParticleID = null;
		}
		mGridItemState = GridItemState.PortalClosed;
	}

	public void DrawScaryPot(Graphics g)
	{
		int num = (int)(mGridItemState - 3);
		Debug.ASSERT(num >= 0 && num < 3);
		int num2 = mBoard.GridToPixelX(mGridX, mGridY) - 5;
		int num3 = mBoard.GridToPixelY(mGridX, mGridY) - 15;
		if (mTransparentCounter > 0)
		{
			g.DrawImageCel(Resources.IMAGE_SCARY_POT, (int)((float)num2 * Constants.S), (int)((float)num3 * Constants.S), num, 0);
			Graphics graphics = Graphics.GetNew(g);
			if (mScaryPotType == ScaryPotType.Seed)
			{
				graphics.mScaleX = 0.7f;
				graphics.mScaleY = 0.7f;
				SeedPacket.DrawSmallSeedPacket(graphics, (float)(num2 + Constants.GridItem_ScaryPot_SeedPacket_Offset.X) * Constants.S, (float)(num3 + Constants.GridItem_ScaryPot_SeedPacket_Offset.Y) * Constants.S, mSeedType, SeedType.None, 0f, 255, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false);
			}
			else if (mScaryPotType == ScaryPotType.Zombie)
			{
				graphics.mScaleX = 0.4f;
				graphics.mScaleY = 0.4f;
				float num4 = Constants.GridItem_ScaryPot_Zombie_Offset.X;
				float num5 = Constants.GridItem_ScaryPot_Zombie_Offset.Y;
				if (mZombieType == ZombieType.Football || mZombieType == ZombieType.FootballPremium)
				{
					graphics.mScaleX = 0.4f;
					graphics.mScaleY = 0.4f;
					num4 = Constants.GridItem_ScaryPot_ZombieFootball_Offset.X;
					num5 = Constants.GridItem_ScaryPot_ZombieFootball_Offset.Y;
				}
				if (mZombieType == ZombieType.Gargantuar || mZombieType == ZombieType.RedeyeGargantuar || mZombieType == ZombieType.RobotTitan || mZombieType == ZombieType.RedeyeRobotTitan)
				{
					graphics.mScaleX = 0.3f;
					graphics.mScaleY = 0.3f;
					num4 += (float)Constants.GridItem_ScaryPot_ZombieGargantuar_Offset.X;
					num5 += (float)Constants.GridItem_ScaryPot_ZombieGargantuar_Offset.Y;
				}
				mApp.mReanimatorCache.DrawCachedZombieThumbnail(graphics, ((float)num2 + num4) * Constants.S, ((float)num3 + num5) * Constants.S, mZombieType);
			}
			else if (mScaryPotType == ScaryPotType.Sun)
			{
				int num6 = mBoard.mChallenge.ScaryPotterCountSunInPot(this);
				Reanimation newReanimation = Reanimation.GetNewReanimation();
				newReanimation.ReanimationInitializeType(0f, 0f, ReanimationType.Sun);
				newReanimation.OverrideScale(0.5f, 0.5f);
				for (int i = 0; i < num6; i++)
				{
					float num7 = Constants.GridItem_ScaryPot_Sun_Offset.X;
					float num8 = Constants.GridItem_ScaryPot_Sun_Offset.Y;
					switch (i)
					{
					case 1:
						num7 += 3f;
						num8 += -20f;
						break;
					case 2:
						num7 += -6f;
						num8 += -10f;
						break;
					case 3:
						num7 += 6f;
						num8 += -5f;
						break;
					case 4:
						num7 += 5f;
						num8 += -15f;
						break;
					}
					newReanimation.SetPosition(((float)num2 + num7) * Constants.S, ((float)num3 + num8) * Constants.S);
					newReanimation.Draw(g);
				}
				newReanimation.PrepareForReuse();
			}
			graphics.PrepareForReuse();
			int theAlpha = TodCommon.TodAnimateCurve(0, 50, mTransparentCounter, 255, 58, TodCurves.Linear);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, theAlpha));
		}
		g.DrawImageCel(Resources.IMAGE_SCARY_POT, (int)((float)num2 * Constants.S), (int)((float)num3 * Constants.S), num, 1);
		if (mHighlighted)
		{
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			g.SetColorizeImages(colorizeImages: true);
			if (mTransparentCounter == 0)
			{
				g.SetColor(new SexyColor(255, 255, 255, 196));
			}
			g.DrawImageCel(Resources.IMAGE_SCARY_POT, (int)((float)num2 * Constants.S), (int)((float)num3 * Constants.S), num, 1);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public void UpdateScaryPot()
	{
		if (mApp.mTodCheatKeys && mApp.mWidgetManager.mKeyDown[16])
		{
			if (mTransparentCounter < 50)
			{
				mTransparentCounter++;
			}
			return;
		}
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (plant.mDead || plant.mSeedType != SeedType.Plantern || plant.NotOnGround() || plant.IsDisabled())
			{
				continue;
			}
			if (plant.mState == PlantState.PlanternHenshinOver || (plant.mState == PlantState.PlanternHenshinBegin && plant.mStateCountdown < 50))
			{
				if (mTransparentCounter < 50)
				{
					mTransparentCounter++;
				}
				return;
			}
			int num = Math.Max(Math.Abs(plant.mPlantCol - mGridX), Math.Abs(plant.mRow - mGridY));
			if (num <= 1)
			{
				if (mTransparentCounter < 50)
				{
					mTransparentCounter++;
				}
				return;
			}
		}
		if (mTransparentCounter > 0)
		{
			mTransparentCounter--;
		}
	}

	public void UpdatePortal()
	{
		Reanimation reanimation = mApp.ReanimationGet(mGridItemReanimID);
		if (reanimation == null)
		{
			return;
		}
		if (mGridItemState == GridItemState.PortalClosed)
		{
			if (reanimation.mLoopCount > 0)
			{
				GridItemDie();
			}
		}
		else if (reanimation.mLoopType == ReanimLoopType.PlayOnceAndHold && reanimation.mLoopCount > 0)
		{
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_pulse, ReanimLoopType.Loop, 0, 12f);
			ParticleEffect theEffect = ParticleEffect.PortalCircle;
			float num = (float)mGridX * 80f + 13f + (float)Constants.BOARD_EXTRA_ROOM;
			float num2 = (float)mBoard.GridToPixelY(0, mGridY) - 39f;
			if (mGridItemType == GridItemType.PortalSquare)
			{
				theEffect = ParticleEffect.PortalSquare;
				num += -8f;
				num2 += 15f;
			}
			TodParticleSystem theParticle = mApp.AddTodParticle(num, num2, 0, theEffect);
			mGridItemParticleID = mApp.ParticleGetID(theParticle);
		}
	}

	public void DrawSquirrel(Graphics g)
	{
		int num = mBoard.GridToPixelX(mGridX, mGridY);
		int num2 = mBoard.GridToPixelY(mGridX, mGridY);
		if (mGridItemState == GridItemState.SquirrelPeeking)
		{
			num2 += TodCommon.TodAnimateCurve(50, 0, mGridItemCounter, 0, -40, TodCurves.BounceSlowMiddle);
		}
		else if (mGridItemState == GridItemState.SquirrelRunningUp)
		{
			num2 += TodCommon.TodAnimateCurve(50, 0, mGridItemCounter, 100, 0, TodCurves.EaseIn);
		}
		else if (mGridItemState == GridItemState.SquirrelRunningDown)
		{
			num2 += TodCommon.TodAnimateCurve(50, 0, mGridItemCounter, -100, 0, TodCurves.EaseIn);
		}
		else if (mGridItemState == GridItemState.SquirrelRunningLeft)
		{
			num += TodCommon.TodAnimateCurve(50, 0, mGridItemCounter, 80, 0, TodCurves.EaseIn);
		}
		else if (mGridItemState == GridItemState.SquirrelRunningRight)
		{
			num += TodCommon.TodAnimateCurve(50, 0, mGridItemCounter, -80, 0, TodCurves.EaseIn);
		}
	}

	public void UpdateRake()
	{
		if (mGridItemState == GridItemState.RakeAttracting || mGridItemState == GridItemState.RakeWaiting)
		{
			Zombie zombie = RakeFindZombie();
			if (zombie != null)
			{
				Reanimation reanimation = mApp.ReanimationGet(mGridItemReanimID);
				reanimation.ReanimationDie();
				mRenderOrder = zombie.mRenderOrder + 1;
				reanimation = mApp.AddReanimation(mPosX, mPosY, mRenderOrder, ReanimationType.Zorrose);
				reanimation.SetFramesForLayer("anim_special");
				reanimation.mIsAttachment = true;
				reanimation.mAnimRate = 30f;
				mGridItemReanimID = mApp.ReanimationGetID(reanimation);
				mGridItemCounter = 300;
				mGridItemState = GridItemState.RakeTriggered;
				int num = Random.Shared.Next(2);
				PlantVoice.Play(SeedType.None, SeedType.None, (num == 0) ? PlantVoice.VoiceType.Special_Zorrose_Attack1 : PlantVoice.VoiceType.Special_Zorrose_Attack2);
			}
		}
		else if (mGridItemState == GridItemState.RakeTriggered)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(mGridItemReanimID);
			if (reanimation2 != null && reanimation2.ShouldTriggerTimedEvent(0.2f))
			{
				DoRakeDamage();
			}
			mGridItemCounter--;
			if (mGridItemCounter <= 0)
			{
				GridItemDie();
			}
		}
	}

	public void DoRakeDamage()
	{
		TRect rect = new TRect((int)mPosX, (int)mPosY, 263, 80);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying() && !zombie.IsBobsledTeamWithSled() && zombie.mRow - mGridY == 0 && zombie.EffectedByDamage(1u))
			{
				TRect zombieRect = zombie.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(rect, zombieRect);
				if (rectOverlap >= 0)
				{
					zombie.TakeDamage(1800, 0u);
				}
			}
		}
	}

	public Zombie RakeFindZombie()
	{
		TRect rect = new TRect((int)mPosX, (int)mPosY, 63, 80);
		int count = mBoard.mZombies.Count;
		for (int i = 0; i < count; i++)
		{
			Zombie zombie = mBoard.mZombies[i];
			if (!zombie.mDead && !zombie.IsDeadOrDying() && !zombie.IsBobsledTeamWithSled() && zombie.mRow - mGridY == 0 && zombie.EffectedByDamage(1u))
			{
				TRect zombieRect = zombie.GetZombieRect();
				int rectOverlap = GameConstants.GetRectOverlap(rect, zombieRect);
				if (rectOverlap >= 0)
				{
					return zombie;
				}
			}
		}
		return null;
	}

	public void DrawIZombieBrain(Graphics g)
	{
		if (mGridItemState == GridItemState.BrainSquished)
		{
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_BRAIN, mPosX * Constants.S, (mPosY + 20f) * Constants.S, 1f, 0.25f);
			return;
		}
		if (mBoard.mAdvice.mDuration > 0 && mBoard.mHelpIndex == AdviceType.IZombieEatAllBrains)
		{
			SexyColor flashingColor = TodCommon.GetFlashingColor(mBoard.mMainCounter, 75);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(flashingColor);
		}
		g.DrawImageF(AtlasResources.IMAGE_BRAIN, mPosX * Constants.S, mPosY * Constants.S);
		if (mTransparentCounter > 0)
		{
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
			g.SetColorizeImages(colorizeImages: true);
			int theAlpha = TodCommon.ClampInt(mTransparentCounter * 3, 0, 255);
			g.SetColor(new SexyColor(255, 255, 255, theAlpha));
			g.DrawImageF(AtlasResources.IMAGE_BRAIN, mPosX * Constants.S, mPosY * Constants.S);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
			g.SetColorizeImages(colorizeImages: false);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public void UpdateTalismanMove()
	{
		mGridItemCounter--;
		if (mGridItemState == GridItemState.Normal)
		{
			if (mGridItemCounter <= -500)
			{
				GridItemDie();
			}
			else
			{
				if (mGridItemCounter > 0)
				{
					return;
				}
				GridItem gridItem = null;
				GridItem theGridItem = null;
				int index = -1;
				while (mBoard.IterateGridItems(ref theGridItem, ref index))
				{
					if (theGridItem.mGridItemType == GridItemType.TalismanMove && theGridItem != this && theGridItem.mGridItemState == GridItemState.Normal && theGridItem.mGridItemCounter <= 0)
					{
						gridItem = theGridItem;
						break;
					}
				}
				if (gridItem != null)
				{
					Plant topPlantAt = mBoard.GetTopPlantAt(mGridX, mGridY, TopPlant.OnlyNormalPosition);
					Plant topPlantAt2 = mBoard.GetTopPlantAt(gridItem.mGridX, gridItem.mGridY, TopPlant.OnlyNormalPosition);
					if (topPlantAt != null && topPlantAt2 != null && !topPlantAt.NotOnGround() && Plant.CanSwap(topPlantAt.mSeedType) && !topPlantAt2.NotOnGround() && Plant.CanSwap(topPlantAt2.mSeedType))
					{
						int mPlantCol = topPlantAt.mPlantCol;
						int mRow = topPlantAt.mRow;
						topPlantAt.mGloveGrabbed = true;
						topPlantAt2.mGloveGrabbed = true;
						mBoard.mChallenge.MovePlant(topPlantAt, topPlantAt2.mPlantCol, topPlantAt2.mRow);
						mBoard.mChallenge.MovePlant(topPlantAt2, mPlantCol, mRow);
						topPlantAt.mGloveGrabbed = false;
						topPlantAt2.mGloveGrabbed = false;
						gridItem.mGridItemState = GridItemState.TalismanDisappearing;
						gridItem.mGridItemCounter = 500;
						mApp.ReanimationGet(gridItem.mGridItemReanimID).mLoopCount = 0;
						mGridItemState = GridItemState.TalismanDisappearing;
						mGridItemCounter = 500;
						mApp.ReanimationGet(mGridItemReanimID).mLoopCount = 0;
					}
				}
			}
		}
		else
		{
			if (mGridItemState != GridItemState.TalismanDisappearing)
			{
				return;
			}
			if (mGridItemCounter <= 0)
			{
				GridItemDie();
				return;
			}
			Reanimation reanimation = mApp.ReanimationTryToGet(mGridItemReanimID);
			if (reanimation != null)
			{
				if (reanimation.mLoopCount > 0)
				{
					GridItemDie();
				}
			}
			else
			{
				GridItemDie();
			}
		}
	}

	public void UpdateTalisman()
	{
		mGridItemCounter--;
		if (mGridItemState == GridItemState.Normal)
		{
			if (mGridItemCounter <= 0)
			{
				mApp.ReanimationTryToGet(mGridItemReanimID)?.PlayReanim("anim_overdue_souls_disappear", ReanimLoopType.PlayOnceAndHold, 10, 12f);
				mGridItemState = GridItemState.TalismanDisappearing;
				mGridItemCounter = 500;
			}
			else
			{
				if (mGridItemCounter % 50 != 0)
				{
					return;
				}
				Plant thePlant = null;
				int index = -1;
				while (mBoard.IteratePlants(ref thePlant, ref index))
				{
					if (thePlant.mPlantCol == mGridX && thePlant.mRow == mGridY && !thePlant.NotOnGround())
					{
						thePlant.mInTalismanCounter = 100;
					}
				}
			}
		}
		else
		{
			if (mGridItemState != GridItemState.TalismanDisappearing)
			{
				return;
			}
			if (mGridItemCounter <= 0)
			{
				GridItemDie();
				return;
			}
			Reanimation reanimation = mApp.ReanimationTryToGet(mGridItemReanimID);
			if (reanimation != null)
			{
				if (reanimation.mLoopCount > 0)
				{
					GridItemDie();
				}
			}
			else
			{
				GridItemDie();
			}
		}
	}

	public void UpdateBrain()
	{
		if (mGridItemState == GridItemState.BrainSquished)
		{
			mGridItemCounter--;
			if (mGridItemCounter <= 0)
			{
				GridItemDie();
			}
		}
		if (mTransparentCounter > 0)
		{
			mTransparentCounter--;
		}
	}

	public void DrawStinky(Graphics g)
	{
		Reanimation reanimation = mApp.ReanimationGet(mGridItemReanimID);
		float mAnimTime = reanimation.mAnimTime;
		Debug.ASSERT(mMotionTrailCount <= 12);
		for (int num = mMotionTrailCount - 1; num >= 0; num--)
		{
			if (num % 2 != 0)
			{
				MotionTrailFrame motionTrailFrame = mMotionTrailFrames[num];
				float num2 = motionTrailFrame.mPosX - mPosX;
				float num3 = motionTrailFrame.mPosY - mPosY;
				int theAlpha = TodCommon.TodAnimateCurve(0, 11, num, 64, 16, TodCurves.Linear);
				g.SetColor(new SexyColor(255, 255, 255, theAlpha));
				g.SetColorizeImages(colorizeImages: true);
				reanimation.mAnimTime = motionTrailFrame.mAnimTime;
				float num4 = g.mTransX;
				float num5 = g.mTransY;
				g.mTransX += (int)(num2 * Constants.S);
				g.mTransY += (int)(num3 * Constants.S);
				reanimation.Draw(g);
				g.SetColorizeImages(colorizeImages: false);
				g.mTransX = (int)num4;
				g.mTransY = (int)num5;
			}
		}
		reanimation.mAnimTime = mAnimTime;
		if (mGridItemType == GridItemType.Stinky && mHighlighted)
		{
			reanimation.mEnableExtraAdditiveDraw = true;
			reanimation.mExtraAdditiveColor = new SexyColor(255, 255, 255, 196);
		}
		reanimation.Draw(g);
		reanimation.mEnableExtraAdditiveDraw = false;
	}
}
