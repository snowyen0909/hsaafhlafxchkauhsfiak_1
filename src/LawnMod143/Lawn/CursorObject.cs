using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class CursorObject : GameObject
{
	public int mSeedBankIndex;

	public SeedType mType;

	public SeedType mImitaterType;

	internal bool mMagnifier;

	public CursorType mCursorType;

	public Coin mCoinID;

	public int mCoinID_Save;

	public Plant mGlovePlantID;

	public int mGlovePlantID_Save;

	public Plant mDuplicatorPlantID;

	public int mDuplicatorPlantID_Save;

	public Plant mCobCannonPlantID;

	public int mCobCannonPlantID_Save;

	public int mHammerDownCounter;

	public Reanimation mReanimCursorID;

	public int mReanimCursorID_Save;

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncInt(ref mSeedBankIndex);
		theContext.SyncEnum(ref mType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncEnum(ref mImitaterType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncEnum(ref mCursorType, theContext.aCursorTypeSaver, SaveFileVersion.Initial, CursorType.Normal);
		theContext.SyncInt(ref mCoinID_Save);
		theContext.SyncInt(ref mGlovePlantID_Save);
		theContext.SyncInt(ref mDuplicatorPlantID_Save);
		theContext.SyncInt(ref mCobCannonPlantID_Save);
		theContext.SyncInt(ref mHammerDownCounter);
		theContext.SyncInt(ref mReanimCursorID_Save);
	}

	internal override void SyncObj(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.SyncObj(theContext);
		TodLibObjSyncer.SyncObjFromList(ref mCoinID_Save, ref mCoinID, mBoard.mCoins, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mGlovePlantID_Save, ref mGlovePlantID, mBoard.mPlants, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mDuplicatorPlantID_Save, ref mDuplicatorPlantID, mBoard.mPlants, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mCobCannonPlantID_Save, ref mCobCannonPlantID, mBoard.mPlants, theContext.mReading);
		TodLibObjSyncer.SyncObjFromList(ref mReanimCursorID_Save, ref mReanimCursorID, mApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
	}

	public CursorObject()
	{
		mType = SeedType.None;
		mImitaterType = SeedType.None;
		mSeedBankIndex = -1;
		mX = 0;
		mY = 0;
		mCursorType = CursorType.Normal;
		mCoinID = null;
		mDuplicatorPlantID = null;
		mCobCannonPlantID = null;
		mGlovePlantID = null;
		mReanimCursorID = null;
		mPosScaled = false;
		if (mApp.IsWhackAZombieLevel())
		{
			ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.Hammer, theIsPreloading: true);
			Reanimation reanimation = mApp.AddReanimation(-25f, 16f, 0, ReanimationType.Hammer);
			reanimation.mIsAttachment = true;
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_whack_zombie, ReanimLoopType.PlayOnceAndHold, 0, 24f);
			reanimation.mAnimTime = 1f;
			mReanimCursorID = mApp.ReanimationGetID(reanimation);
		}
		mWidth = 80;
		mHeight = 80;
	}

	public void Update()
	{
		if (mApp.mGameScene != GameScenes.Playing && !mBoard.mCutScene.IsInShovelTutorial())
		{
			mVisible = false;
			return;
		}
		if (!mApp.mWidgetManager.mMouseIn)
		{
			mVisible = false;
			return;
		}
		mApp.ReanimationTryToGet(mReanimCursorID)?.Update();
		mVisible = true;
		mX = mBoard.mLastToolX;
		mY = mBoard.mLastToolY;
		if (mMagnifier)
		{
			mX = mApp.mWidgetManager.mLastMouseX - mBoard.mX;
			mY = mApp.mWidgetManager.mLastMouseY - mBoard.mY;
		}
		if (mCursorType != CursorType.PlantFromGlove || mGlovePlantID == null || !mGlovePlantID.mGloveGrabbed)
		{
			return;
		}
		mGlovePlantID.mX = (int)((float)mX * Constants.IS) - 40;
		mGlovePlantID.mY = (int)((float)mY * Constants.IS) - 40;
		mGlovePlantID.mHighlighted = false;
		mGlovePlantID.mRenderOrder = mRenderOrder + 1;
		if (mApp.mZenGarden != null && mGlovePlantID.mPottedPlantIndex != -1)
		{
			mApp.mZenGarden.PottedPlantFromIndex(mGlovePlantID.mPottedPlantIndex);
			Plant flowerPotAt = mBoard.GetFlowerPotAt(mGlovePlantID.mPlantCol, mGlovePlantID.mRow);
			if (flowerPotAt != null)
			{
				mGlovePlantID.mY -= (int)Plant.PlantDrawHeightOffset(mBoard, flowerPotAt, SeedType.Flowerpot, -1, -1);
				flowerPotAt.mHighlighted = false;
				flowerPotAt.mX = (int)((float)mX * Constants.IS) - 40;
				flowerPotAt.mY = (int)((float)mY * Constants.IS) - 40;
				flowerPotAt.mY -= (int)Plant.PlantDrawHeightOffset(mBoard, mGlovePlantID, SeedType.Flowerpot, -1, -1);
				flowerPotAt.mRenderOrder = mRenderOrder + 1;
			}
		}
	}

	public override bool LoadFromFile(Buffer b)
	{
		base.LoadFromFile(b);
		mCursorType = (CursorType)b.ReadLong();
		mHammerDownCounter = b.ReadLong();
		mImitaterType = SeedTypeLegacy.FromInt(b.ReadLong());
		mSeedBankIndex = b.ReadLong();
		mType = SeedTypeLegacy.FromInt(b.ReadLong());
		return true;
	}

	public override bool SaveToFile(Buffer b)
	{
		base.SaveToFile(b);
		b.WriteLong((int)mCursorType);
		b.WriteLong(mHammerDownCounter);
		b.WriteLong((int)mImitaterType);
		b.WriteLong(mSeedBankIndex);
		b.WriteLong((int)mType);
		return true;
	}

	public void DrawGroundLayer(Graphics g)
	{
		if (mCursorType == CursorType.Normal)
		{
			return;
		}
		int theX = (int)((float)mX * Constants.IS);
		int theY = (int)((float)mY * Constants.IS);
		int num;
		int num2;
		if (mCursorType == CursorType.PlantFromBank || mCursorType == CursorType.PlantFromGlove)
		{
			if (mBoard.mIgnoreMouseUp)
			{
				return;
			}
			num = mBoard.PlantingPixelToGridX(theX, theY, mType);
			num2 = mBoard.PlantingPixelToGridY(theX, theY, mType);
			if (mBoard.CanPlantAt(num, num2, mType) != PlantingReason.Ok)
			{
				return;
			}
		}
		else
		{
			num = mBoard.PixelToGridX(theX, theY);
			num2 = mBoard.PixelToGridY(theX, theY);
			if ((mCursorType == CursorType.Shovel && (mBoard.mIgnoreMouseUp || mBoard.ToolHitTest(mX, mY, posScaled: false, isTouch: false).mObjectType == GameObjectType.None)) || (mCursorType == CursorType.Glove && (mBoard.mIgnoreMouseUp || mBoard.ToolHitTest(mX, mY, posScaled: false, isTouch: false).mObjectType != GameObjectType.Plant)))
			{
				return;
			}
		}
		if (num2 < 0 || num < 0 || mBoard.GridToPixelY(num, num2) < 0)
		{
			return;
		}
		Graphics graphics = Graphics.GetNew();
		graphics.mTransX = Constants.Board_Offset_AspectRatio_Correction;
		for (int i = 0; i < Constants.GRIDSIZEX; i++)
		{
			mBoard.DrawCelHighlight(graphics, i, num2);
		}
		int num3 = (mBoard.StageHas6Rows() ? 6 : 5);
		for (int j = 0; j < num3; j++)
		{
			if (j != num2)
			{
				mBoard.DrawCelHighlight(graphics, num, j);
			}
		}
		graphics.PrepareForReuse();
	}

	public void DrawToolIconImage(Graphics g, Image image)
	{
		g.DrawImage(image, -image.mWidth / 2, -image.mHeight / 2, image.mWidth, image.mHeight);
	}

	public void DrawTopLayer(Graphics g)
	{
		if (mMagnifier)
		{
			DrawToolIconImage(g, AtlasResources.IMAGE_MAGNIFIER);
			return;
		}
		bool flag = mBoard.HasConveyorBeltSeedBank();
		if (flag)
		{
			g.SetClipRect(new TRect(-mX - (int)((float)(mBoard.mCamera?.BoardToScreen(Constants.Board_Offset_AspectRatio_Correction, 0).mX ?? 0) / Constants.BoardCameraScaleMultiplier) + Constants.New.SeedBank_Width, -mY, 32767, 32767));
			g.HardwareClip();
		}
		switch (mCursorType)
		{
		case CursorType.Shovel:
			if (!mBoard.mIgnoreMouseUp)
			{
				Image iMAGE_SHOVEL_PLANTGIRL_HI_RES = AtlasResources.IMAGE_SHOVEL_PLANTGIRL_HI_RES;
				int num3 = iMAGE_SHOVEL_PLANTGIRL_HI_RES.mWidth / 2;
				int num4 = iMAGE_SHOVEL_PLANTGIRL_HI_RES.mHeight / 2;
				g.SetColor(SexyColor.White);
				g.DrawImage(iMAGE_SHOVEL_PLANTGIRL_HI_RES, -num3 / 2, -num4 / 2, num3, num4);
			}
			break;
		case CursorType.Hammer:
		{
			Reanimation reanimation = mApp.ReanimationGet(mReanimCursorID);
			reanimation.Draw(g);
			break;
		}
		case CursorType.CobcannonTarget:
		{
			int num = AtlasResources.IMAGE_COBCANNON_TARGET.mWidth;
			int num2 = AtlasResources.IMAGE_COBCANNON_TARGET.mHeight;
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, 204));
			g.DrawImage(AtlasResources.IMAGE_COBCANNON_TARGET, -num / 4 / 2, -num2 / 4 / 2, num / 4, num2 / 4);
			g.SetColorizeImages(colorizeImages: false);
			break;
		}
		case CursorType.WateringCan:
			if (mApp.mPlayerInfo.mPurchases[13] == 1)
			{
				if (new TRect(Constants.ZEN_XMIN, Constants.ZEN_YMIN, Constants.ZEN_XMAX - Constants.ZEN_XMIN, Constants.ZEN_YMAX - Constants.ZEN_YMIN).Contains(mApp.mBoard.mLastToolX, mApp.mBoard.mLastToolY))
				{
					int num5 = AtlasResources.IMAGE_ZEN_GOLDTOOLRETICLE.mWidth;
					int num6 = AtlasResources.IMAGE_ZEN_GOLDTOOLRETICLE.mHeight;
					g.DrawImage(AtlasResources.IMAGE_ZEN_GOLDTOOLRETICLE, -num5 / 2, -num6 / 2, num5, num6);
				}
				DrawToolIconImage(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_GOLD);
			}
			else if (mApp.mPlayerInfo.mPurchases[13] == 0)
			{
				DrawToolIconImage(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1);
			}
			else
			{
				DrawToolIconImage(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_DIAMOND);
			}
			break;
		case CursorType.BugSpray:
			DrawToolIconImage(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_BUGSPRAY_BOTTLE);
			break;
		case CursorType.Phonograph:
			DrawToolIconImage(g, AtlasResources.IMAGE_PHONOGRAPH);
			break;
		case CursorType.Fertilizer:
			DrawToolIconImage(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_FERTILIZER_BAG1);
			break;
		case CursorType.Glove:
			DrawToolIconImage(g, (mApp.mGameMode == GameMode.ChallengeZenGarden) ? AtlasResources.IMAGE_ZEN_GARDENGLOVE : AtlasResources.IMAGE_GLOVE_PLANTGIRL);
			break;
		case CursorType.Chocolate:
			DrawToolIconImage(g, AtlasResources.IMAGE_CHOCOLATE);
			break;
		case CursorType.MoneySign:
			DrawToolIconImage(g, AtlasResources.IMAGE_ZEN_MONEYSIGN);
			break;
		case CursorType.TreeFood:
			DrawToolIconImage(g, AtlasResources.IMAGE_REANIM_TREEFOOD);
			break;
		case CursorType.Wheeelbarrow:
		{
			Image iMAGE_ZEN_WHEELBARROW = AtlasResources.IMAGE_ZEN_WHEELBARROW;
			DrawToolIconImage(g, iMAGE_ZEN_WHEELBARROW);
			PottedPlant pottedPlantInWheelbarrow = mApp.mZenGarden.GetPottedPlantInWheelbarrow();
			if (pottedPlantInWheelbarrow != null)
			{
				mApp.mZenGarden.DrawPottedPlant(g, (float)(-iMAGE_ZEN_WHEELBARROW.mWidth / 2) + (float)Constants.ZenGardenButton_WheelbarrowPlant_Offset.X, (float)(-iMAGE_ZEN_WHEELBARROW.mHeight / 2) + (float)Constants.ZenGardenButton_WheelbarrowPlant_Offset.Y, pottedPlantInWheelbarrow, 0.6f, theDrawPot: true);
			}
			break;
		}
		case CursorType.PlantFromBank:
		case CursorType.PlantFromUsableCoin:
		{
			if (mBoard.mIgnoreMouseUp || mType == SeedType.None || (!flag && mX + Constants.Board_Offset_AspectRatio_Correction < Constants.SMALL_SEEDPACKET_WIDTH))
			{
				break;
			}
			float thePosY = (float)(-Constants.New.Board_GridCellSizeY_6Rows) / 2f * Constants.S;
			float thePosX = (float)(-Constants.New.Board_GridCellSizeX) / 2f * Constants.S;
			if (Challenge.IsZombieSeedType(mType))
			{
				thePosX = (int)(-40f * Constants.S);
				thePosY = (int)(-90f * Constants.S);
				ZombieType type = Challenge.IZombieSeedTypeToZombieType(mType);
				if ((mType & SeedType.MindControlledCardFlag) != 0)
				{
					g.SetColorizeImages(colorizeImages: true);
					g.SetColor(GameConstants.ZOMBIE_MINDCONTROLLED_COLOR);
				}
				GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedZombieThumbnail(g, thePosX, thePosY, type);
				if ((mType & SeedType.MindControlledCardFlag) != 0)
				{
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
					GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedZombieThumbnail(g, thePosX, thePosY, type);
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
					g.SetColorizeImages(colorizeImages: false);
				}
				break;
			}
			if ((mType != SeedType.Lilypad && mType != SeedType.Tallnut) || (mBoard != null && mBoard.IsPlantInCursor() && mBoard.CanPlantAt(mBoard.mCursorPreview.mGridX, mBoard.mCursorPreview.mGridY, mType) == PlantingReason.Ok))
			{
				if (mType != SeedType.Cattail || !mApp.IsCattailSkinEnabled())
				{
					GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedPlant(g, thePosX, thePosY, mType, DrawVariation.Normal);
				}
				else
				{
					GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedPlant(g, thePosX, thePosY, SeedType.HypnoCattail, DrawVariation.Normal);
				}
			}
			else
			{
				GlobalStaticVars.gLawnApp.mReanimatorCache.DrawCachedPlant(g, thePosX, thePosY, mType, DrawVariation.ZenGarden);
			}
			if (mBoard.TrashcanHitTest(mX, mY))
			{
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_CROSS, -AtlasResources.IMAGE_SEEDPACKET_CROSS.mWidth / 2, -AtlasResources.IMAGE_SEEDPACKET_CROSS.mHeight / 2);
			}
			break;
		}
		case CursorType.PlantFromGlove:
			_ = mBoard.mCursorObject.mGlovePlantID.mPottedPlantIndex;
			_ = -1;
			if (mBoard.TrashcanHitTest(mX, mY))
			{
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_CROSS, -AtlasResources.IMAGE_SEEDPACKET_CROSS.mWidth / 2, -AtlasResources.IMAGE_SEEDPACKET_CROSS.mHeight / 2);
			}
			break;
		}
		if (flag)
		{
			g.EndHardwareClip();
		}
	}

	public void Die()
	{
		mApp.RemoveReanimation(ref mReanimCursorID);
	}
}
