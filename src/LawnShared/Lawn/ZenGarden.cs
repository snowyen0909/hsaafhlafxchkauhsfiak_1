using System;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ZenGarden : StoreListener, ButtonListener
{
	private struct PottedPlantOffset(int y, int xRight, int xLeft, int yWheelBarrowScale, int yCachedOffset, int xCachedOffset)
	{
		public int y = y;

		public int xRight = xRight;

		public int xLeft = xLeft;

		public int yWheelBarrowScale = yWheelBarrowScale;

		public int yCachedOffset = yCachedOffset;

		public int xCachedOffset = xCachedOffset;
	}

	public Plant mInteracting;

	public bool mIsInteracting;

	public bool mIsInteractingInTouch;

	public NewLawnButton mTouch1Button;

	public NewLawnButton mTouch2Button;

	public NewLawnButton mFeedButton;

	public NewLawnButton mHeartBarButton;

	public BoxedLabel mPlantNameLabel;

	public BoxedLabel mPlantVoiceLabel;

	public NewLawnButton mBackFromInteractionButton;

	private static PottedPlantOffset[] POTTED_PLANT_DRAW_OFFSETS;

	public static SpecialGridPlacement[] gGreenhouseGridPlacement;

	public static SpecialGridPlacement[] gAquariumGridPlacement;

	public LawnApp mApp;

	public Board mBoard;

	public GardenType mGardenType;

	public bool mIsTutorial;

	public readonly ulong STINKY_BASE_TIME = (ulong)TimeSpan.FromTicks(new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks).TotalSeconds;

	private DateTime aNow;

	public Plant mPlantForSale;

	public ZenGarden()
	{
		mApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		mBoard = null;
		mGardenType = GardenType.Main;
		mIsTutorial = false;
		mTouch1Button = GameButton.MakeNewButton(101, this, string.Empty, null, AtlasResources.IMAGE_ZENGARDEN_BUTTON_TOUCH1, null, null);
		mTouch1Button.Resize((int)(525f * Constants.ISUI), (int)(55f * Constants.ISUI), (int)(82f * Constants.ISUI), (int)(82f * Constants.ISUI));
		mTouch2Button = GameButton.MakeNewButton(102, this, string.Empty, null, AtlasResources.IMAGE_ZENGARDEN_BUTTON_TOUCH2, null, null);
		mTouch2Button.Resize((int)(525f * Constants.ISUI), (int)(175f * Constants.ISUI), (int)(82f * Constants.ISUI), (int)(82f * Constants.ISUI));
		mFeedButton = GameButton.MakeNewButton(103, this, string.Empty, null, AtlasResources.IMAGE_ZENGARDEN_BUTTON_FEED, null, null);
		mFeedButton.Resize((int)(525f * Constants.ISUI), (int)(295f * Constants.ISUI), (int)(82f * Constants.ISUI), (int)(82f * Constants.ISUI));
		mHeartBarButton = GameButton.MakeNewButton(104, this, string.Empty, null, AtlasResources.IMAGE_ZENGARDEN_HEARTBAR_EMPTY, null, null);
		mHeartBarButton.Resize((int)(39f * Constants.ISUI), 55, (int)(82f * Constants.ISUI), (int)(82f * Constants.ISUI));
		mPlantNameLabel = new BoxedLabel(AtlasResources.IMAGE_ALMANAC_CLAY_TABLET, new TRect(11, 6, 11, 11), "模仿者", Resources.FONT_BRIANNETOD16, new SexyColor(255, 220, 149), 1.0, DrawStringJustification.Center);
		mPlantVoiceLabel = new BoxedLabel(AtlasResources.IMAGE_ALMANAC_CLAY_TABLET, new TRect(11, 6, 11, 11), "模仿者", Resources.FONT_BRIANNETOD12, SexyColor.White, 1.0, DrawStringJustification.Center);
		mBackFromInteractionButton = GameButton.MakeNewButton(105, this, string.Empty, null, AtlasResources.IMAGE_ALMANAC_BACKBUTTON, null, null);
		mBackFromInteractionButton.Resize(Constants.UIMenuButtonPosition.X, Constants.UIMenuButtonPosition.Y, AtlasResources.IMAGE_ALMANAC_BACKBUTTON.mWidth, AtlasResources.IMAGE_ALMANAC_BACKBUTTON.mHeight);
	}

	static ZenGarden()
	{
		POTTED_PLANT_DRAW_OFFSETS = new PottedPlantOffset[53]
		{
			new PottedPlantOffset(30, 0, -35, -7, -16, 0),
			new PottedPlantOffset(30, 0, -40, -9, -14, 0),
			new PottedPlantOffset(36, 0, -30, -25, -17, 0),
			new PottedPlantOffset(34, -2, -35, -27, -17, 0),
			new PottedPlantOffset(25, -5, -35, -13, -17, 0),
			new PottedPlantOffset(30, -3, -40, -7, -17, -1),
			new PottedPlantOffset(28, 4, -50, -16, -19, 3),
			new PottedPlantOffset(30, -2, -37, -7, -17, 0),
			new PottedPlantOffset(32, -4, -37, -25, -17, 0),
			new PottedPlantOffset(35, 0, -37, -25, -17, 0),
			new PottedPlantOffset(28, 0, -37, -20, -17, 1),
			new PottedPlantOffset(70, 0, -37, 0, -14, 0),
			new PottedPlantOffset(28, 0, -37, -25, -18, 0),
			new PottedPlantOffset(34, 8, -43, -21, -20, -3),
			new PottedPlantOffset(28, 0, -40, -21, -14, 0),
			new PottedPlantOffset(28, 0, -40, -21, -18, 0),
			new PottedPlantOffset(12, 0, -40, -36, -17, 0),
			new PottedPlantOffset(28, 0, -40, -19, -17, 0),
			new PottedPlantOffset(30, 0, -40, -8, -17, 0),
			new PottedPlantOffset(10, 0, -40, -28, -17, 0),
			new PottedPlantOffset(28, 0, -40, -22, -17, 0),
			new PottedPlantOffset(22, 0, -40, -30, -17, 0),
			new PottedPlantOffset(27, 0, -40, -19, -17, 0),
			new PottedPlantOffset(28, 0, -40, -22, -17, 0),
			new PottedPlantOffset(5, 0, -40, -23, -11, 0),
			new PottedPlantOffset(30, 0, -40, -28, -17, 0),
			new PottedPlantOffset(33, 5, -45, -28, -17, -1),
			new PottedPlantOffset(35, 0, -40, -32, -22, 0),
			new PottedPlantOffset(30, 0, -40, -20, -17, -5),
			new PottedPlantOffset(30, 0, -40, -30, -14, 0),
			new PottedPlantOffset(15, 0, -40, -18, -14, 0),
			new PottedPlantOffset(30, 0, -40, -26, -14, -3),
			new PottedPlantOffset(33, 0, -40, -27, -18, -8),
			new PottedPlantOffset(0, 0, 0, 0, -14, 0),
			new PottedPlantOffset(30, 0, -37, -23, -14, -5),
			new PottedPlantOffset(30, 10, -30, -10, -14, 0),
			new PottedPlantOffset(32, 0, -40, -25, -14, 0),
			new PottedPlantOffset(35, 0, -40, -30, -17, 0),
			new PottedPlantOffset(30, 0, -35, -8, -17, -2),
			new PottedPlantOffset(33, 0, -40, -28, -17, -8),
			new PottedPlantOffset(30, 0, -40, -22, -14, 0),
			new PottedPlantOffset(30, 0, -38, -22, -14, 0),
			new PottedPlantOffset(35, 0, -40, -26, -14, 0),
			new PottedPlantOffset(30, 0, -40, -26, -14, 0),
			new PottedPlantOffset(34, 0, -40, -26, -14, -5),
			new PottedPlantOffset(30, 0, -40, -25, -14, -2),
			new PottedPlantOffset(25, 0, -40, -35, -14, 0),
			new PottedPlantOffset(0, 0, -40, 0, -14, 0),
			new PottedPlantOffset(33, 0, -40, 0, -14, 0),
			new PottedPlantOffset(20, 0, 0, 0, -14, 0),
			new PottedPlantOffset(0, 0, 0, 0, -14, 0),
			new PottedPlantOffset(30, 0, -38, -35, -14, 0),
			new PottedPlantOffset(30, 0, 37, 0, -14, -5)
		};
		gGreenhouseGridPlacement = new SpecialGridPlacement[32]
		{
			new SpecialGridPlacement(73, 73, 0, 0),
			new SpecialGridPlacement(155, 71, 1, 0),
			new SpecialGridPlacement(239, 68, 2, 0),
			new SpecialGridPlacement(321, 73, 3, 0),
			new SpecialGridPlacement(406, 71, 4, 0),
			new SpecialGridPlacement(484, 67, 5, 0),
			new SpecialGridPlacement(566, 70, 6, 0),
			new SpecialGridPlacement(648, 72, 7, 0),
			new SpecialGridPlacement(67, 168, 0, 1),
			new SpecialGridPlacement(150, 165, 1, 1),
			new SpecialGridPlacement(232, 170, 2, 1),
			new SpecialGridPlacement(314, 175, 3, 1),
			new SpecialGridPlacement(416, 173, 4, 1),
			new SpecialGridPlacement(497, 170, 5, 1),
			new SpecialGridPlacement(578, 164, 6, 1),
			new SpecialGridPlacement(660, 168, 7, 1),
			new SpecialGridPlacement(41, 268, 0, 2),
			new SpecialGridPlacement(130, 266, 1, 2),
			new SpecialGridPlacement(219, 260, 2, 2),
			new SpecialGridPlacement(310, 266, 3, 2),
			new SpecialGridPlacement(416, 267, 4, 2),
			new SpecialGridPlacement(504, 261, 5, 2),
			new SpecialGridPlacement(594, 265, 6, 2),
			new SpecialGridPlacement(684, 269, 7, 2),
			new SpecialGridPlacement(37, 371, 0, 3),
			new SpecialGridPlacement(124, 369, 1, 3),
			new SpecialGridPlacement(211, 368, 2, 3),
			new SpecialGridPlacement(302, 369, 3, 3),
			new SpecialGridPlacement(425, 375, 4, 3),
			new SpecialGridPlacement(512, 368, 5, 3),
			new SpecialGridPlacement(602, 365, 6, 3),
			new SpecialGridPlacement(691, 368, 7, 3)
		};
		gAquariumGridPlacement = new SpecialGridPlacement[8]
		{
			new SpecialGridPlacement(113, 185, 0, 0),
			new SpecialGridPlacement(306, 120, 1, 0),
			new SpecialGridPlacement(356, 270, 2, 0),
			new SpecialGridPlacement(622, 120, 3, 0),
			new SpecialGridPlacement(669, 270, 4, 0),
			new SpecialGridPlacement(122, 355, 5, 0),
			new SpecialGridPlacement(365, 458, 6, 0),
			new SpecialGridPlacement(504, 417, 7, 0)
		};
		for (int i = 0; i < gGreenhouseGridPlacement.Length; i++)
		{
			gGreenhouseGridPlacement[i].mPixelX = (int)((float)gGreenhouseGridPlacement[i].mPixelX * Constants.ZenGardenGreenhouseMultiplierX) + Constants.ZenGardenGreenhouseOffset.X;
			gGreenhouseGridPlacement[i].mPixelY = (int)((float)gGreenhouseGridPlacement[i].mPixelY * Constants.ZenGardenGreenhouseMultiplierY) + Constants.ZenGardenGreenhouseOffset.Y;
		}
		for (int j = 0; j < Constants.gMushroomGridPlacement.Length; j++)
		{
			Constants.gMushroomGridPlacement[j].mPixelX = Constants.gMushroomGridPlacement[j].mPixelX + Constants.ZenGardenMushroomGardenOffset.X;
			Constants.gMushroomGridPlacement[j].mPixelY = Constants.gMushroomGridPlacement[j].mPixelY + Constants.ZenGardenMushroomGardenOffset.Y;
		}
	}

	public void Dispose()
	{
		UnloadBackdrop();
	}

	public void UnloadBackdrop()
	{
		mApp.DelayLoadZenGardenBackground(string.Empty);
	}

	public void ReloadPlant(Plant thePlant)
	{
		thePlant.mDead = true;
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		if (pottedPlant.mWhichZenGarden == mGardenType)
		{
			PlacePottedPlant(thePlant.mPottedPlantIndex);
		}
	}

	public void ZenGardenInitLevel(bool theJustSwitchingGardens)
	{
		mBoard = mApp.mBoard;
		for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
		{
			PottedPlant pottedPlant = PottedPlantFromIndex(i);
			if (pottedPlant.mWhichZenGarden == mGardenType)
			{
				PlacePottedPlant(i);
			}
		}
		Challenge mChallenge = mBoard.mChallenge;
		mChallenge.mChallengeStateCounter = 3000;
		AddStinky();
		mApp.mMusic.StartGameMusic();
		if (!theJustSwitchingGardens)
		{
			Board.Camera camera = new Board.Camera(mBoard, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
			if (Constants.BoardCameraScaleMultiplier != 1f)
			{
				camera.SetPosition(0f, Constants.Is500pMode ? (-50f * Constants.BoardCameraScaleMultiplier) : 0f, Constants.BoardCameraScaleMultiplier);
			}
			mBoard.AttachCamera(camera);
			mTouch1Button.mDisabled = true;
			mTouch1Button.mVisible = false;
			mBoard.AddWidget(mTouch1Button);
			mTouch2Button.mDisabled = true;
			mTouch2Button.mVisible = false;
			mBoard.AddWidget(mTouch2Button);
			mFeedButton.mDisabled = true;
			mFeedButton.mVisible = false;
			mBoard.AddWidget(mFeedButton);
			mPlantNameLabel.mVisible = false;
			mBoard.AddWidget(mPlantNameLabel);
			mPlantVoiceLabel.mVisible = false;
			mBoard.AddWidget(mPlantVoiceLabel);
			mPlantNameLabel.Resize(250, Constants.Is500pMode ? 400 : 475, 300, 250);
			mPlantVoiceLabel.Resize(-100, Constants.Is500pMode ? 463 : 538, 1000, 250);
			mBackFromInteractionButton.mDisabled = true;
			mBackFromInteractionButton.mVisible = false;
			mBoard.AddWidget(mBackFromInteractionButton);
		}
	}

	public float GetPottedPlantXOffset(SeedType theType, bool isFlipped)
	{
		return 0f;
	}

	public float GetPottedPlantYOffset(SeedType theType, bool isFlipped)
	{
		return 0f;
	}

	public void DrawPottedPlantIcon(Graphics g, float x, float y, PottedPlant thePottedPlant)
	{
		DrawPottedPlant(g, x, y, thePottedPlant, 0.7f, theDrawPot: true);
	}

	public void DrawPottedPlant(Graphics g, float x, float y, PottedPlant thePottedPlant, float theScale, bool theDrawPot)
	{
		Graphics graphics = Graphics.GetNew(g);
		graphics.mScaleX = theScale;
		graphics.mScaleY = theScale;
		DrawVariation theDrawVariation = DrawVariation.Normal;
		SeedType seedType = thePottedPlant.mSeedType;
		if (thePottedPlant.mPlantAge != PottedPlantAge.Sprout)
		{
			if (seedType == SeedType.Tanglekelp && thePottedPlant.mWhichZenGarden == GardenType.Aquarium)
			{
				theDrawVariation = DrawVariation.Aquarium;
			}
			else
			{
				theDrawVariation = ((seedType == SeedType.Seashroom && thePottedPlant.mWhichZenGarden == GardenType.Aquarium) ? DrawVariation.Aquarium : (seedType switch
				{
					SeedType.Sunshroom => DrawVariation.Bigidle, 
					SeedType.Pumpkinshell => DrawVariation.ZenGarden, 
					SeedType.Tallnut => DrawVariation.ZenGarden, 
					_ => thePottedPlant.mDrawVariation, 
				}));
			}
		}
		else
		{
			seedType = SeedType.Sprout;
			if (thePottedPlant.mSeedType != SeedType.Marigold)
			{
				theDrawVariation = DrawVariation.SproutNoFlower;
			}
		}
		PottedPlant.FacingDirection mFacing = thePottedPlant.mFacing;
		float num = 0f;
		float num2 = 0f;
		if (theDrawPot)
		{
			num2 -= Plant.PlantDrawHeightOffset(mBoard, null, SeedType.Flowerpot, -1, -1) * theScale;
			DrawVariation theDrawVariation2 = DrawVariation.ZenGarden;
			if (Plant.IsAquatic(seedType))
			{
				theDrawVariation2 = DrawVariation.ZenGardenWater;
			}
			Plant.DrawSeedType(graphics, SeedType.Flowerpot, SeedType.None, theDrawVariation2, x + 40f, y + 64f);
		}
		if (thePottedPlant.mFacing == PottedPlant.FacingDirection.Left)
		{
			graphics.mScaleX = 0f - theScale;
		}
		num2 -= Plant.PlantDrawHeightOffset(mBoard, null, seedType, -1, -1) * theScale;
		if (theDrawPot)
		{
			int num3 = (int)seedType;
			if (seedType == SeedType.Sprout)
			{
				num3 = 52;
			}
			num2 -= Plant.PlantFlowerPotHeightOffset(seedType, theScale);
		}
		num2 -= PlantPottedDrawHeightOffset(seedType, graphics.mScaleY, bInWheelBarrow: false) * theScale;
		Plant.DrawSeedType(graphics, seedType, SeedType.None, theDrawVariation, x + num + 40f, y + num2 + 64f);
		graphics.PrepareForReuse();
	}

	public bool IsZenGardenFull(bool theIncludeDroppedPresents)
	{
		int num = 0;
		if ((mBoard != null) & theIncludeDroppedPresents)
		{
			num += mBoard.CountCoinByType(CoinType.AwardPresent);
			num += mBoard.CountCoinByType(CoinType.PresentPlant);
		}
		int num2 = 0;
		for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
		{
			PottedPlant pottedPlant = PottedPlantFromIndex(i);
			if (pottedPlant.mWhichZenGarden == GardenType.Main || pottedPlant.mWhichZenGarden == GardenType.Main2 || pottedPlant.mWhichZenGarden == GardenType.Night)
			{
				num2++;
			}
		}
		return num2 + num >= 96;
	}

	public bool FindOpenZenGardenSpot(ref int theSpotX, ref int theSpotY, GardenType theGardenType)
	{
		Debug.ASSERT(theGardenType == GardenType.Main || theGardenType == GardenType.Main2 || theGardenType == GardenType.Night);
		TodWeightedGridArray[] array = new TodWeightedGridArray[32];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = TodWeightedGridArray.GetNewTodWeightedGridArray();
		}
		int num = 0;
		for (int j = 0; j < 8; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				if (mApp.mCrazyDaveMessageIndex != -1 && (j < 2 || k < 1))
				{
					continue;
				}
				bool flag = false;
				for (int l = 0; l < mApp.mPlayerInfo.mNumPottedPlants; l++)
				{
					PottedPlant pottedPlant = PottedPlantFromIndex(l);
					if (pottedPlant.mWhichZenGarden == theGardenType && pottedPlant.mX == j && pottedPlant.mY == k)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					array[num].mX = j;
					array[num].mY = k;
					array[num].mWeight = 1;
					num++;
				}
			}
		}
		if (num == 0)
		{
			return false;
		}
		TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array, num);
		theSpotX = todWeightedGridArray.mX;
		theSpotY = todWeightedGridArray.mY;
		return true;
	}

	public void AddPottedPlant(PottedPlant thePottedPlant)
	{
		Debug.ASSERT(mApp.mPlayerInfo.mNumPottedPlants < 200);
		int mNumPottedPlants = mApp.mPlayerInfo.mNumPottedPlants;
		PottedPlant pottedPlant = mApp.mPlayerInfo.mPottedPlant[mNumPottedPlants];
		pottedPlant.mDrawVariation = thePottedPlant.mDrawVariation;
		pottedPlant.mFacing = thePottedPlant.mFacing;
		pottedPlant.mFeedingsPerGrow = thePottedPlant.mFeedingsPerGrow;
		pottedPlant.mFutureAttribute = thePottedPlant.mFutureAttribute;
		pottedPlant.mLastChocolateTime = thePottedPlant.mLastChocolateTime;
		pottedPlant.mLastFertilizedTime = thePottedPlant.mLastFertilizedTime;
		pottedPlant.mLastNeedFulfilledTime = thePottedPlant.mLastNeedFulfilledTime;
		pottedPlant.mPlantAge = thePottedPlant.mPlantAge;
		pottedPlant.mPlantNeed = thePottedPlant.mPlantNeed;
		pottedPlant.mSeedType = thePottedPlant.mSeedType;
		pottedPlant.mTimesFed = thePottedPlant.mTimesFed;
		pottedPlant.mX = thePottedPlant.mX;
		pottedPlant.mY = thePottedPlant.mY;
		pottedPlant.mWhichZenGarden = GardenType.Main;
		pottedPlant.mLastWateredTime = default;
		if (!FindOpenZenGardenSpot(ref pottedPlant.mX, ref pottedPlant.mY, GardenType.Main))
		{
			pottedPlant.mWhichZenGarden = GardenType.Main2;
			if (!FindOpenZenGardenSpot(ref pottedPlant.mX, ref pottedPlant.mY, GardenType.Main2))
			{
				pottedPlant.mWhichZenGarden = GardenType.Night;
				FindOpenZenGardenSpot(ref pottedPlant.mX, ref pottedPlant.mY, GardenType.Night);
			}
		}
		mApp.mPlayerInfo.mNumPottedPlants++;
		if (mApp.mGameMode == GameMode.ChallengeZenGarden && mBoard != null && pottedPlant.mWhichZenGarden == mGardenType)
		{
			Plant thePlant = PlacePottedPlant(mNumPottedPlants);
			if (mApp.GetDialog(Dialogs.DIALOG_STORE) == null)
			{
				mBoard.DoPlantingEffects(pottedPlant.mX, pottedPlant.mY, thePlant, mGardenType == GardenType.Aquarium);
			}
		}
	}

	public void MouseDownWithTool(int x, int y, CursorType theCursorType, bool isTouch)
	{
		if (theCursorType == CursorType.Wheeelbarrow && GetPottedPlantInWheelbarrow() != null)
		{
			MouseDownWithFullWheelBarrow(x, y);
			mBoard.ClearCursor();
			return;
		}
		if (theCursorType == CursorType.WateringCan || theCursorType == CursorType.Fertilizer || theCursorType == CursorType.BugSpray || theCursorType == CursorType.Phonograph || theCursorType == CursorType.Chocolate)
		{
			MouseDownWithFeedingTool(x, y, theCursorType, isTouch);
			return;
		}
		HitResult hitResult = mBoard.ToolHitTest(x, y, posScaled: true, isTouch);
		Plant plant = null;
		if (hitResult.mObjectType == GameObjectType.Plant)
		{
			plant = (Plant)hitResult.mObject;
		}
		if (plant == null || plant.mPottedPlantIndex == -1)
		{
			mApp.PlayFoley(FoleyType.Drop);
			mBoard.ClearCursor();
			return;
		}
		switch (theCursorType)
		{
		case CursorType.MoneySign:
			MouseDownWithMoneySign(plant);
			break;
		case CursorType.Wheeelbarrow:
			MouseDownWithEmptyWheelBarrow(plant);
			mBoard.ClearCursor();
			break;
		case CursorType.Glove:
			mBoard.mCursorObject.mType = plant.mSeedType;
			mBoard.mCursorObject.mImitaterType = plant.mImitaterType;
			mBoard.mCursorObject.mCursorType = CursorType.PlantFromGlove;
			mBoard.mCursorObject.mGlovePlantID = mBoard.mPlants[mBoard.mPlants.IndexOf(plant)];
			plant.mGloveGrabbed = true;
			mBoard.mIgnoreMouseUp = true;
			mApp.PlaySample(Resources.SOUND_TAP);
			break;
		}
	}

	public void MovePlant(Plant thePlant, int theGridX, int theGridY)
	{
		if (mApp.mGameMode == GameMode.ChallengeZenGarden)
		{
			int num = mBoard.GridToPixelX(theGridX, theGridY);
			int num2 = mBoard.GridToPixelY(theGridX, theGridY);
			if (mApp.mGameMode == GameMode.ChallengeZenGarden)
			{
				num2 -= Constants.ZenGardenGreenhouseOffset.Y;
			}
			Debug.ASSERT(mBoard.GetTopPlantAt(theGridX, theGridY, TopPlant.Any) == null || (theGridX == thePlant.mPlantCol && theGridY == thePlant.mRow));
			bool mIsAsleep = thePlant.mIsAsleep;
			thePlant.SetSleeping(theIsAsleep: false);
			Plant topPlantAt = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyUnderPlant);
			if (topPlantAt != null)
			{
				topPlantAt.mX = num;
				topPlantAt.mY = num2;
				topPlantAt.mPlantCol = theGridX;
				topPlantAt.mRow = theGridY;
				topPlantAt.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, topPlantAt.mY);
			}
			float num3 = num - thePlant.mX;
			float num4 = num2 - thePlant.mY;
			thePlant.mX = num;
			thePlant.mY = num2;
			thePlant.mPlantCol = theGridX;
			thePlant.mRow = theGridY;
			thePlant.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, thePlant.mY + 1);
			if (thePlant.mSleepingReanimID != null)
			{
				float[] sleepingReanimOffset = thePlant.GetSleepingReanimOffset();
				float num5 = (float)thePlant.mX + sleepingReanimOffset[0];
				float num6 = (float)thePlant.mY + sleepingReanimOffset[1];
				thePlant.mSleepingReanimID.SetPosition(num5 * Constants.S, num6 * Constants.S);
				thePlant.mSleepingReanimID.mRenderOrder = thePlant.mRenderOrder;
			}
			TodParticleSystem todParticleSystem = mApp.ParticleTryToGet(thePlant.mParticleID);
			if (todParticleSystem != null && todParticleSystem.mEmitterList.Count != 0)
			{
				TodParticleEmitter todParticleEmitter = todParticleSystem.mParticleHolder.mEmitters[0];
				todParticleSystem.SystemMove(todParticleEmitter.mSystemCenter.x + num3, todParticleEmitter.mSystemCenter.y + num4);
			}
			PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
			pottedPlant.mX = theGridX;
			pottedPlant.mY = theGridY;
			if (thePlant.mState == PlantState.ZenGardenHappy)
			{
				RemoveHappyEffect(thePlant);
				AddHappyEffect(thePlant);
			}
			if (topPlantAt != null)
			{
				mBoard.DoPlantingEffects(theGridX, theGridY, topPlantAt, mGardenType == GardenType.Aquarium);
			}
			else
			{
				mBoard.DoPlantingEffects(theGridX, theGridY, thePlant, mGardenType == GardenType.Aquarium);
			}
		}
	}

	public void MouseDownWithMoneySign(Plant thePlant)
	{
		mBoard.ClearCursor();
		string theDialogHeader = TodStringFile.TodStringTranslate("[ZEN_SELL_HEADER]");
		string theDialogLines = TodStringFile.TodStringTranslate("[ZEN_SELL_LINES]");
		int plantSellPrice = GetPlantSellPrice(thePlant);
		if (mApp.mCrazyDaveState == CrazyDaveState.Off)
		{
			mApp.CrazyDaveEnter();
		}
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		string crazyDaveText = mApp.GetCrazyDaveText(1700);
		crazyDaveText = TodCommon.TodReplaceString(crazyDaveText, "{SELL_PRICE}", Common.CommaSeperate(plantSellPrice * 10));
		string empty = string.Empty;
		empty = ((thePlant.mSeedType != SeedType.Sprout || pottedPlant.mSeedType != SeedType.Marigold) ? Plant.GetNameString(thePlant.mSeedType, thePlant.mImitaterType) : TodStringFile.TodStringTranslate("[MARIGOLD_SPROUT]"));
		crazyDaveText = TodCommon.TodReplaceString(crazyDaveText, "{PLANT_TYPE}", empty);
		mApp.CrazyDaveTalkMessage(crazyDaveText);
		Reanimation reanimation = mApp.ReanimationGet(mApp.mCrazyDaveReanimID);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_blahblah, ReanimLoopType.PlayOnceAndHold, 20, 12f);
		LawnDialog lawnDialog = mApp.DoDialog(48, isModal: true, theDialogHeader, theDialogLines, "", 1);
		lawnDialog.mX += Constants.ZenGarden_SellDialog_Offset.X;
		lawnDialog.mY += Constants.ZenGarden_SellDialog_Offset.Y;
		mBoard.ShowCoinBank();
		mPlantForSale = thePlant;
	}

	public Plant PlacePottedPlant(int thePottedPlantIndex)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePottedPlantIndex);
		SeedType seedType = pottedPlant.mSeedType;
		if (pottedPlant.mPlantAge == PottedPlantAge.Sprout)
		{
			seedType = SeedType.Sprout;
		}
		bool flag = true;
		if ((mGardenType == GardenType.Mushroom || mGardenType == GardenType.Mushroom2) && !Plant.IsAquatic(seedType))
		{
			flag = false;
		}
		else if (mGardenType == GardenType.Aquarium)
		{
			flag = false;
		}
		if (flag)
		{
			Plant plant = mBoard.NewPlant(pottedPlant.mX, pottedPlant.mY, SeedType.Flowerpot, SeedType.None);
			plant.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, plant.mY);
			plant.mStateCountdown = 0;
			Reanimation reanimation = mApp.ReanimationGet(plant.mBodyReanimID);
			if (Plant.IsAquatic(seedType))
			{
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_waterplants);
			}
			else
			{
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_zengarden);
			}
		}
		if (seedType < SeedType.Peashooter || seedType >= SeedType.SeedTypeCount)
		{
			pottedPlant.mSeedType = SeedType.Kernelpult;
			seedType = SeedType.Kernelpult;
		}
		if (seedType == SeedType.Cattail && mApp.mPlayerInfo.mPurchases[38] == 1 && mApp.mPlayerInfo.mPurchases[39] == 0)
		{
			seedType = SeedType.HypnoCattail;
		}
		Plant plant2 = mBoard.NewPlant(pottedPlant.mX, pottedPlant.mY, seedType, SeedType.None);
		plant2.mPottedPlantIndex = thePottedPlantIndex;
		plant2.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, plant2.mY + 1);
		plant2.mStateCountdown = 0;
		Reanimation reanimation2 = mApp.ReanimationTryToGet(plant2.mBodyReanimID);
		if (reanimation2 != null)
		{
			if (seedType == SeedType.Sprout)
			{
				if (pottedPlant.mSeedType != SeedType.Marigold)
				{
					reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle_noflower);
				}
			}
			else if (seedType == SeedType.Tanglekelp && mGardenType == GardenType.Aquarium)
			{
				reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle_aquarium);
			}
			else if (seedType == SeedType.Seashroom && mGardenType == GardenType.Aquarium)
			{
				reanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle_aquarium);
			}
			plant2.UpdateReanim();
			reanimation2.Update();
		}
		PlantSetLaunchCounter(plant2);
		UpdatePlantEffectState(plant2);
		return plant2;
	}

	public float PlantPottedDrawHeightOffset(SeedType theSeedType, float theScale, bool bInWheelBarrow)
	{
		return PlantPottedDrawHeightOffset(theSeedType, theScale, bInWheelBarrow, DrawVariation.Normal);
	}

	public float PlantPottedDrawHeightOffset(SeedType theSeedType, float theScale, bool bInWheelBarrow, DrawVariation theDrawVariation)
	{
		float num = 0f;
		float num2 = 0f;
		switch (theSeedType)
		{
		case SeedType.Gravebuster:
			num2 += 0f;
			num += 15f;
			break;
		case SeedType.Puffshroom:
			num2 += 0f;
			num += 24f;
			break;
		case SeedType.Sunshroom:
			num2 += 10f;
			num += 17f;
			break;
		case SeedType.Scaredyshroom:
			num2 += 5f;
			num += 5f;
			break;
		case SeedType.Tanglekelp:
			num2 += -18f;
			num += 20f;
			break;
		case SeedType.Seashroom:
			num2 += -20f;
			num += 15f;
			break;
		case SeedType.Lilypad:
			num2 += -10f;
			num += 30f;
			break;
		case SeedType.Chomper:
			num += 0f;
			break;
		case SeedType.Hypnoshroom:
			num += 10f;
			break;
		case SeedType.Magnetshroom:
			num += 10f;
			break;
		case SeedType.Peashooter:
		case SeedType.Sunflower:
		case SeedType.Snowpea:
		case SeedType.Repeater:
		case SeedType.Threepeater:
		case SeedType.Marigold:
		case SeedType.Leftpeater:
			num += 10f;
			break;
		case SeedType.Starfruit:
			num2 += 10f;
			num += 24f;
			break;
		case SeedType.Cabbagepult:
		case SeedType.Melonpult:
			num += 10f;
			num2 += 3f;
			break;
		case SeedType.Potatomine:
			num += 5f;
			break;
		case SeedType.Torchwood:
			num += 3f;
			break;
		case SeedType.Spikeweed:
		case SeedType.Spikerock:
			num += 10f;
			num2 -= 13f;
			break;
		case SeedType.Blover:
			num += 10f;
			break;
		case SeedType.Pumpkinshell:
			num += 20f;
			break;
		case SeedType.Plantern:
			num += -1f;
			break;
		}
		if (bInWheelBarrow && theSeedType != SeedType.Flowerpot)
		{
			float num3 = POTTED_PLANT_DRAW_OFFSETS[(int)theSeedType].yWheelBarrowScale;
			num2 += num3 + num3 * (theScale - 0.5f) / 2f;
		}
		num = Constants.InvertAndScale(num);
		return num2 + (0f - num + num * theScale);
	}

	public int GetPlantSellPrice(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		if (pottedPlant.mSeedType == SeedType.Marigold)
		{
			if (pottedPlant.mPlantAge == PottedPlantAge.Sprout)
			{
				return 150;
			}
			if (pottedPlant.mPlantAge == PottedPlantAge.Small)
			{
				return 200;
			}
			if (pottedPlant.mPlantAge == PottedPlantAge.Medium)
			{
				return 250;
			}
			if (pottedPlant.mPlantAge == PottedPlantAge.Full)
			{
				return 300;
			}
			Debug.ASSERT(value: false);
		}
		if (pottedPlant.mPlantAge == PottedPlantAge.Sprout)
		{
			return 150;
		}
		if (pottedPlant.mPlantAge == PottedPlantAge.Small)
		{
			return 300;
		}
		if (pottedPlant.mPlantAge == PottedPlantAge.Medium)
		{
			return 500;
		}
		if (pottedPlant.mPlantAge != PottedPlantAge.Full)
		{
			Debug.ASSERT(value: false);
			return -666;
		}
		if (Plant.IsNocturnal(pottedPlant.mSeedType) || Plant.IsAquatic(pottedPlant.mSeedType))
		{
			return 1000;
		}
		return 800;
	}

	public static void GetNextGarden(LawnApp theApp, ref GardenType theCurrent, ref BackgroundType theBackground, ref bool theTree, int theNext)
	{
		Span<GardenType> span = stackalloc GardenType[7];
		int num = 0;
		span[num++] = GardenType.Main;
		span[num++] = GardenType.Main2;
		if (theApp.mPlayerInfo.mPurchases[37] != 0)
		{
			span[num++] = GardenType.Night;
		}
		if (theApp.mPlayerInfo.mPurchases[18] != 0)
		{
			span[num++] = GardenType.Mushroom;
			span[num++] = GardenType.Mushroom2;
		}
		if (theApp.mPlayerInfo.mPurchases[25] != 0)
		{
			span[num++] = GardenType.Aquarium;
		}
		if (theApp.mPlayerInfo.mPurchases[27] != 0)
		{
			span[num++] = GardenType.GardenTypeCount;
		}
		int num2 = -1;
		for (int i = 0; i < num; i++)
		{
			if ((theTree && span[i] == GardenType.GardenTypeCount) || (!theTree && span[i] == theCurrent))
			{
				num2 = i;
				break;
			}
		}
		int index = ((num2 != -1) ? ((num2 + theNext + num) % num) : 0);
		theTree = span[index] == GardenType.GardenTypeCount;
		if (!theTree)
		{
			theCurrent = span[index];
			BackgroundType backgroundType;
			switch (theCurrent)
			{
			case GardenType.Mushroom:
			case GardenType.Mushroom2:
				backgroundType = BackgroundType.MushroomGarden;
				break;
			case GardenType.Aquarium:
				backgroundType = BackgroundType.Zombiquarium;
				break;
			case GardenType.Night:
				backgroundType = BackgroundType.GreenhouseNight;
				break;
			default:
				backgroundType = BackgroundType.Greenhouse;
				break;
			}
			theBackground = backgroundType;
		}
	}

	public void JumptoNextGarden(bool theTree, int theNext)
	{
		LeaveGarden();
		mBoard.ClearAdvice(AdviceType.None);
		mBoard.mPlants.Clear();
		mBoard.mCoins.Clear();
		mApp.mEffectSystem.EffectSystemFreeAll();
		GetNextGarden(mApp, ref mGardenType, ref mBoard.mBackground, ref theTree, theNext);
		if (theTree)
		{
			mApp.KillBoard();
			mApp.PreNewGame(GameMode.TreeOfWisdom, theLookForSavedGame: false);
			return;
		}
		if (mBoard.mBackground == BackgroundType.MushroomGarden)
		{
			mApp.DelayLoadZenGardenBackground("DelayLoad_MushroomGarden");
		}
		else if (mBoard.mBackground == BackgroundType.Greenhouse)
		{
			mApp.DelayLoadZenGardenBackground("DelayLoad_GreenHouseGarden");
		}
		else if (mBoard.mBackground == BackgroundType.Zombiquarium)
		{
			mApp.DelayLoadZenGardenBackground("DelayLoad_Zombiquarium");
		}
		else if (mBoard.mBackground == BackgroundType.GreenhouseNight)
		{
			mApp.DelayLoadZenGardenBackground("DelayLoad_GreenHouseNight");
		}
		else
		{
			Debug.ASSERT(value: false);
		}
		if ((mBoard.mBackground == BackgroundType.MushroomGarden || mBoard.mBackground == BackgroundType.Zombiquarium || mBoard.mBackground == BackgroundType.GreenhouseNight) && mApp.mPlayerInfo.mPurchases[19] == 0)
		{
			mBoard.DisplayAdvice("[ADVICE_NEED_WHEELBARROW]", MessageStyle.HintTallFast, AdviceType.NeedWheelbarrow);
		}
		ZenGardenInitLevel(theJustSwitchingGardens: true);
	}

	public void GotoNextGarden()
	{
		JumptoNextGarden(theTree: false, 1);
	}

	public void GotoPrevGarden(bool isTreeOfWisdom = false)
	{
		JumptoNextGarden(isTreeOfWisdom, -1);
	}

	public void ZenGardenUpdate()
	{
		if (mApp.GetDialog(4) != null)
		{
			return;
		}
		Challenge mChallenge = mBoard.mChallenge;
		if (mBoard.mCursorObject.mCursorType != CursorType.Normal)
		{
			mChallenge.mChallengeState = ChallengeState.Normal;
			mChallenge.mChallengeStateCounter = 3000;
		}
		else if (mApp.mBoard.mTutorialState == TutorialState.Off)
		{
			if (mChallenge.mChallengeStateCounter > 0)
			{
				mChallenge.mChallengeStateCounter--;
			}
			if (mChallenge.mChallengeState == ChallengeState.Normal && mChallenge.mChallengeStateCounter == 0)
			{
				mChallenge.mChallengeState = ChallengeState.ZenFading;
				mChallenge.mChallengeStateCounter = 50;
			}
		}
		UpdatePlantNeeds();
		int count = mBoard.mPlants.Count;
		for (int i = 0; i < count; i++)
		{
			Plant plant = mBoard.mPlants[i];
			if (!plant.mDead && plant.mPottedPlantIndex != -1)
			{
				PottedPlantUpdate(plant);
			}
		}
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ZenTool)
			{
				ZenToolUpdate(theGridItem);
			}
			if (theGridItem.mGridItemType == GridItemType.Stinky)
			{
				StinkyUpdate(theGridItem);
			}
		}
		if (mBoard.mTutorialState == TutorialState.ZenGardenKeepWatering && CountPlantsNeedingFertilizer() > 0)
		{
			mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_VISIT_STORE]", MessageStyle.HintTallLong, AdviceType.None);
			mBoard.mTutorialState = TutorialState.ZenGardenVisitStore;
			mApp.mPlayerInfo.mZenTutorialMessage = 25;
			mBoard.mStoreButton.mDisabled = false;
			mBoard.mStoreButton.mBtnNoDraw = false;
		}
		if (!mIsInteracting && mInteracting != null && mBoard.mCameraEnabled)
		{
			int num = mBoard.mCamera?.OperationCountdown ?? (-1);
			if (num == 0)
			{
				mIsInteracting = true;
			}
			SexyColor theColor = new SexyColor(255, 255, 255, TodCommon.TodAnimateCurve(90, 0, num, 0, 255, TodCurves.EaseInOut));
			mTouch1Button.SetColor(5, theColor);
			mTouch2Button.SetColor(5, theColor);
			mFeedButton.SetColor(5, theColor);
			mPlantNameLabel.SetTransparency((float)theColor.mAlpha / 255f);
			mPlantVoiceLabel.SetTransparency((float)theColor.mAlpha / 255f);
			mBackFromInteractionButton.SetColor(5, theColor);
		}
		else if (mIsInteracting && mInteracting == null && mBoard.mCameraEnabled)
		{
			int num2 = mBoard.mCamera?.OperationCountdown ?? (-1);
			if (num2 == 0)
			{
				mIsInteracting = false;
				ShowInteractingButtons(doShow: false);
				mBoard.mChallenge.mChallengeState = ChallengeState.Normal;
				return;
			}
			SexyColor theColor2 = new SexyColor(255, 255, 255, TodCommon.TodAnimateCurve(40, 150, num2, 0, 255, TodCurves.EaseInOut));
			mTouch1Button.SetColor(5, theColor2);
			mTouch2Button.SetColor(5, theColor2);
			mFeedButton.SetColor(5, theColor2);
			mPlantNameLabel.SetTransparency((float)theColor2.mAlpha / 255f);
			mPlantVoiceLabel.SetTransparency((float)theColor2.mAlpha / 255f);
			mBackFromInteractionButton.SetColor(5, theColor2);
		}
	}

	public void MouseDownWithFullWheelBarrow(int x, int y)
	{
		PottedPlant pottedPlantInWheelbarrow = GetPottedPlantInWheelbarrow();
		Debug.ASSERT(pottedPlantInWheelbarrow != null);
		if (mApp.mZenGarden.mGardenType == GardenType.Aquarium && !Plant.IsAquatic(pottedPlantInWheelbarrow.mSeedType))
		{
			mBoard.DisplayAdvice("[ZEN_ONLY_AQUATIC_PLANTS]", MessageStyle.HintTallFast, AdviceType.None);
			return;
		}
		int num = mBoard.PixelToGridX(x, y);
		int num2 = mBoard.PixelToGridY(x, y);
		if (num == -1 || num2 == -1 || mBoard.CanPlantAt(num, num2, pottedPlantInWheelbarrow.mSeedType) != PlantingReason.Ok)
		{
			return;
		}
		pottedPlantInWheelbarrow.mWhichZenGarden = mGardenType;
		pottedPlantInWheelbarrow.mX = num;
		pottedPlantInWheelbarrow.mY = num2;
		int thePottedPlantIndex = -1;
		for (int i = 0; i < mApp.mPlayerInfo.mPottedPlant.Length; i++)
		{
			if (pottedPlantInWheelbarrow == mApp.mPlayerInfo.mPottedPlant[i])
			{
				thePottedPlantIndex = i;
				break;
			}
		}
		Plant thePlant = PlacePottedPlant(thePottedPlantIndex);
		mBoard.DoPlantingEffects(pottedPlantInWheelbarrow.mX, pottedPlantInWheelbarrow.mY, thePlant, mGardenType == GardenType.Aquarium);
	}

	public void MouseDownWithEmptyWheelBarrow(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		RemovePottedPlant(thePlant);
		pottedPlant.mWhichZenGarden = GardenType.Wheelbarrow;
		pottedPlant.mX = 0;
		pottedPlant.mY = 0;
		mApp.PlayFoley(FoleyType.Plant);
	}

	public PottedPlant GetPottedPlantInWheelbarrow()
	{
		for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
		{
			PottedPlant pottedPlant = PottedPlantFromIndex(i);
			if (pottedPlant.mWhichZenGarden == GardenType.Wheelbarrow)
			{
				return pottedPlant;
			}
		}
		return null;
	}

	public void RemovePottedPlant(Plant thePlant)
	{
		thePlant.Die();
		mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyUnderPlant)?.Die();
	}

	public SpecialGridPlacement[] GetSpecialGridPlacements(ref int theCount)
	{
		if (mBoard.mBackground == BackgroundType.MushroomGarden)
		{
			theCount = Constants.gMushroomGridPlacement.Length;
			return Constants.gMushroomGridPlacement;
		}
		if (mBoard.mBackground == BackgroundType.Zombiquarium)
		{
			theCount = gAquariumGridPlacement.Length;
			return gAquariumGridPlacement;
		}
		if (mBoard.mBackground == BackgroundType.Greenhouse || mBoard.mBackground == BackgroundType.GreenhouseNight)
		{
			theCount = gGreenhouseGridPlacement.Length;
			return gGreenhouseGridPlacement;
		}
		Debug.ASSERT(value: false);
		return null;
	}

	public int PixelToGridX(int theX, int theY)
	{
		int theCount = 0;
		SpecialGridPlacement[] specialGridPlacements = GetSpecialGridPlacements(ref theCount);
		for (int i = 0; i < theCount; i++)
		{
			SpecialGridPlacement specialGridPlacement = specialGridPlacements[i];
			if (theX >= specialGridPlacement.mPixelX && theX <= specialGridPlacement.mPixelX + 80 && theY >= specialGridPlacement.mPixelY && theY <= specialGridPlacement.mPixelY + 85)
			{
				return specialGridPlacement.mGridX;
			}
		}
		return -1;
	}

	public int PixelToGridY(int theX, int theY)
	{
		int theCount = 0;
		SpecialGridPlacement[] specialGridPlacements = GetSpecialGridPlacements(ref theCount);
		for (int i = 0; i < theCount; i++)
		{
			SpecialGridPlacement specialGridPlacement = specialGridPlacements[i];
			if (theX >= specialGridPlacement.mPixelX && theX <= specialGridPlacement.mPixelX + 80 && theY >= specialGridPlacement.mPixelY && theY <= specialGridPlacement.mPixelY + 85)
			{
				return specialGridPlacement.mGridY;
			}
		}
		return -1;
	}

	public int GridToPixelX(int theGridX, int theGridY)
	{
		int theCount = 0;
		SpecialGridPlacement[] specialGridPlacements = GetSpecialGridPlacements(ref theCount);
		for (int i = 0; i < theCount; i++)
		{
			SpecialGridPlacement specialGridPlacement = specialGridPlacements[i];
			if (theGridX == specialGridPlacement.mGridX && theGridY == specialGridPlacement.mGridY)
			{
				return specialGridPlacement.mPixelX;
			}
		}
		return -1;
	}

	public int GridToPixelY(int theGridX, int theGridY)
	{
		int theCount = 0;
		SpecialGridPlacement[] specialGridPlacements = GetSpecialGridPlacements(ref theCount);
		for (int i = 0; i < theCount; i++)
		{
			SpecialGridPlacement specialGridPlacement = specialGridPlacements[i];
			if (theGridX == specialGridPlacement.mGridX && theGridY == specialGridPlacement.mGridY)
			{
				return specialGridPlacement.mPixelY;
			}
		}
		return -1;
	}

	public void DrawBackdrop(Graphics g)
	{
		if (mGardenType != GardenType.Aquarium || (mBoard.mCursorObject.mCursorType != CursorType.PlantFromWheelBarrow && mBoard.mCursorObject.mCursorType != CursorType.Wheeelbarrow && mBoard.mCursorObject.mCursorType != CursorType.PlantFromGlove))
		{
			return;
		}
		int theCount = 0;
		SpecialGridPlacement[] specialGridPlacements = GetSpecialGridPlacements(ref theCount);
		for (int i = 0; i < theCount; i++)
		{
			SpecialGridPlacement specialGridPlacement = specialGridPlacements[i];
			if (mBoard.GetTopPlantAt(specialGridPlacement.mGridX, specialGridPlacement.mGridY, TopPlant.ZenToolOrder) == null)
			{
				TodCommon.TodDrawImageCelScaled(g, AtlasResources.IMAGE_PLANTSHADOW, (int)(Constants.S * (float)(specialGridPlacement.mPixelX - Constants.ZenGarden_Aquarium_ShadowOffset.X)), (int)(Constants.S * (float)(specialGridPlacement.mPixelY + Constants.ZenGarden_Aquarium_ShadowOffset.Y)), 0, 0, 1.7f, 1.7f);
			}
		}
	}

	public bool MouseDownZenGarden(int x, int y, int theClickCount, HitResult theHitResult)
	{
		Challenge mChallenge = mBoard.mChallenge;
		if (mChallenge.mChallengeState == ChallengeState.ZenFading)
		{
			mChallenge.mChallengeState = ChallengeState.Normal;
		}
		mChallenge.mChallengeStateCounter = 3000;
		if (theHitResult.mObjectType == GameObjectType.Stinky && mBoard.mCursorObject.mCursorType == CursorType.Normal)
		{
			WakeStinky();
		}
		else if (mBoard.mCursorObject.mCursorType == CursorType.Glove)
		{
			if (mBoard.CanUseGameObject(GameObjectType.Wheelbarrow))
			{
				TRect zenButtonRect = mBoard.GetZenButtonRect(GameObjectType.Wheelbarrow);
				PottedPlant pottedPlantInWheelbarrow = GetPottedPlantInWheelbarrow();
				if (zenButtonRect.Contains(x, y) && pottedPlantInWheelbarrow != null)
				{
					mBoard.ClearCursor();
					mBoard.mCursorObject.mType = pottedPlantInWheelbarrow.mSeedType;
					mBoard.mCursorObject.mImitaterType = SeedType.None;
					mBoard.mCursorObject.mCursorType = CursorType.PlantFromWheelBarrow;
					return true;
				}
			}
		}
		else if (mBoard.mCursorObject.mCursorType == CursorType.PlantFromGlove)
		{
			if (mBoard.CanUseGameObject(GameObjectType.Wheelbarrow))
			{
				TRect zenButtonRect2 = mBoard.GetZenButtonRect(GameObjectType.Wheelbarrow);
				Plant plant = mBoard.mPlants[mBoard.mPlants.IndexOf(mBoard.mCursorObject.mGlovePlantID)];
				if (plant != null && zenButtonRect2.Contains(x, y) && GetPottedPlantInWheelbarrow() == null)
				{
					MouseDownWithEmptyWheelBarrow(plant);
					plant.mGloveGrabbed = false;
					mBoard.ClearCursor();
					return true;
				}
			}
		}
		else if (theHitResult.mObjectType == GameObjectType.None && mBoard.mCursorObject.mCursorType == CursorType.Normal)
		{
			_ = mGardenType;
			_ = 3;
		}
		if (mApp.mCrazyDaveMessageIndex != -1)
		{
			AdvanceCrazyDaveDialog();
			return true;
		}
		return false;
	}

	public void EnterPlantInteraction(Plant thePlant)
	{
		if (!mIsInteractingInTouch && mBoard.mCameraEnabled && mBoard.mCamera != null && mBoard.mCamera.OperationCountdown == 0)
		{
			if (thePlant != mInteracting)
			{
				ShowInteractingButtons(doShow: false);
			}
			int cd = 150 - mBoard.mCamera.OperationCountdown;
			InteractionFocusOnPlant(thePlant, 3f, cd);
			mInteracting = thePlant;
			mBoard.mChallenge.mChallengeState = ChallengeState.ZenPlantInteracting;
			ShowInteractingButtons(doShow: true);
			mIsInteracting = false;
		}
	}

	public void InteractionFocusOnPlant(Plant thePlant, float theScale, int cd)
	{
		mBoard.mCamera.FocusWorldSmooth((float)GridToPixelX(thePlant.mPlantCol, thePlant.mRow) * Constants.S + (float)Constants.LAWN_XMIN * Constants.S + (float)Constants.Board_Offset_AspectRatio_Correction, (float)GridToPixelY(thePlant.mPlantCol, thePlant.mRow) * Constants.S, theScale * Constants.BoardCameraScaleMultiplier, cd, TodCurves.EaseInOut);
	}

	public void LeavePlantInteraction(Plant thePlant)
	{
		if (!mIsInteractingInTouch && thePlant == mInteracting && mBoard.mCameraEnabled && mBoard.mCamera != null && mBoard.mCamera.OperationCountdown == 0)
		{
			int countdown = 150 - mBoard.mCamera.OperationCountdown;
			if (Constants.BoardCameraScaleMultiplier != 1f)
			{
				mBoard.mCamera.SetPositionSmooth(0f, Constants.Is500pMode ? (-50f * Constants.BoardCameraScaleMultiplier) : 0f, Constants.BoardCameraScaleMultiplier, countdown, TodCurves.EaseInOut);
			}
			else
			{
				mBoard.mCamera.SetPositionSmooth(0f, 0f, 1f, countdown, TodCurves.EaseInOut);
			}
			mInteracting = null;
			mIsInteracting = true;
		}
	}

	public void ShowInteractingButtons(bool doShow)
	{
		bool flag = doShow && (mInteracting?.CanDoTouch(1) ?? false);
		bool flag2 = doShow && (mInteracting?.CanDoTouch(2) ?? false);
		mTouch1Button.mDisabled = !flag;
		mTouch1Button.mVisible = flag;
		mTouch2Button.mDisabled = !flag2;
		mTouch2Button.mVisible = flag2;
		mPlantNameLabel.mVisible = mIsInteracting;
		if (doShow && mInteracting != null)
		{
			string theString = "[" + Plant.GetPlantDefinition(mInteracting.mSeedType).mPlantName + "_PLANTGIRL]";
			if (TodStringFile.TodStringListExists(theString))
			{
				mPlantNameLabel.mText = TodStringFile.TodStringTranslate(theString);
			}
			else
			{
				mPlantNameLabel.mText = TodStringFile.TodStringTranslate("[" + Plant.GetPlantDefinition(mInteracting.mSeedType).mPlantName + "]");
			}
			mPlantNameLabel.mVisible = true;
			mPlantNameLabel.SetTransparency(1f);
		}
		mFeedButton.mDisabled = !doShow;
		mFeedButton.mVisible = doShow;
		mHeartBarButton.mDisabled = !doShow;
		mHeartBarButton.mVisible = doShow;
		mPlantVoiceLabel.SetTransparency(1f);
		mPlantVoiceLabel.mVisible = false;
		mBackFromInteractionButton.mDisabled = !doShow;
		mBackFromInteractionButton.mVisible = doShow;
	}

	public void PlantFulfillNeed(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		pottedPlant.mLastNeedFulfilledTime = aNow;
		pottedPlant.mPlantNeed = PottedPlantNeed.None;
		pottedPlant.mTimesFed = 0;
		mApp.PlayFoley(FoleyType.Prize);
		mApp.PlayFoley(FoleyType.SpawnSun);
		mBoard.AddCoin(thePlant.mX + 40, thePlant.mY, CoinType.Gold, CoinMotion.Coin);
		if (Plant.IsNocturnal(thePlant.mSeedType) || Plant.IsAquatic(thePlant.mSeedType))
		{
			mBoard.AddCoin(thePlant.mX + 10, thePlant.mY, CoinType.Gold, CoinMotion.Coin);
			mBoard.AddCoin(thePlant.mX + 70, thePlant.mY, CoinType.Gold, CoinMotion.Coin);
		}
	}

	public void PlantWatered(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		pottedPlant.mTimesFed++;
		int num = TodCommon.RandRangeInt(0, 8);
		if (mBoard.mTutorialState == TutorialState.ZenGardenWaterPlant || mBoard.mTutorialState == TutorialState.ZenGardenKeepWatering)
		{
			num = 9;
		}
		pottedPlant.mLastWateredTime = aNow;
		pottedPlant.mLastWateredTime = pottedPlant.mLastWateredTime.Subtract(TimeSpan.FromSeconds((double)num));
		mApp.PlayFoley(FoleyType.SpawnSun);
		mBoard.AddCoin(thePlant.mX + 40, thePlant.mY, CoinType.Silver, CoinMotion.Coin);
		if (pottedPlant.mPlantAge == PottedPlantAge.Full && pottedPlant.mPlantNeed == PottedPlantNeed.None)
		{
			pottedPlant.mPlantNeed = (PottedPlantNeed)TodCommon.RandRangeInt(3, 4);
		}
		if (mBoard.mTutorialState == TutorialState.ZenGardenWaterPlant)
		{
			mBoard.mTutorialState = TutorialState.ZenGardenKeepWatering;
			mApp.mPlayerInfo.mZenTutorialMessage = 24;
			mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_KEEP_WATERING]", MessageStyle.ZenGardenLong, AdviceType.None);
		}
	}

	public PottedPlantNeed GetPlantsNeed(PottedPlant thePottedPlant)
	{
		if (thePottedPlant.mPlantAge != PottedPlantAge.Sprout && Plant.IsNocturnal(thePottedPlant.mSeedType) && (thePottedPlant.mWhichZenGarden == GardenType.Main || thePottedPlant.mWhichZenGarden == GardenType.Main2))
		{
			return PottedPlantNeed.None;
		}
		if (thePottedPlant.mWhichZenGarden == GardenType.Wheelbarrow)
		{
			return PottedPlantNeed.None;
		}
		TimeSpan timeSpan = aNow - thePottedPlant.mLastWateredTime;
		bool flag = timeSpan.TotalSeconds > 15.0;
		bool flag2 = timeSpan.TotalSeconds < 3.0;
		if (WasPlantFertilizedInLastHour(thePottedPlant))
		{
			return PottedPlantNeed.None;
		}
		if (WasPlantNeedFulfilledToday(thePottedPlant))
		{
			return PottedPlantNeed.None;
		}
		if (Plant.IsAquatic(thePottedPlant.mSeedType) && thePottedPlant.mPlantAge != PottedPlantAge.Sprout)
		{
			if (thePottedPlant.mPlantAge == PottedPlantAge.Full)
			{
				if (PlantShouldRefreshNeed(thePottedPlant))
				{
					return PottedPlantNeed.None;
				}
				return thePottedPlant.mPlantNeed;
			}
			if (thePottedPlant.mWhichZenGarden != GardenType.Aquarium)
			{
				return PottedPlantNeed.None;
			}
			return PottedPlantNeed.Fertilizer;
		}
		if (!flag)
		{
			return PottedPlantNeed.None;
		}
		if (thePottedPlant.mTimesFed < thePottedPlant.mFeedingsPerGrow)
		{
			return PottedPlantNeed.Water;
		}
		if (flag2)
		{
			return PottedPlantNeed.None;
		}
		if (thePottedPlant.mPlantAge != PottedPlantAge.Full)
		{
			return PottedPlantNeed.Fertilizer;
		}
		if (PlantShouldRefreshNeed(thePottedPlant))
		{
			return PottedPlantNeed.None;
		}
		if (thePottedPlant.mPlantNeed != PottedPlantNeed.None)
		{
			return thePottedPlant.mPlantNeed;
		}
		return PottedPlantNeed.Water;
	}

	public void MouseDownWithFeedingTool(int x, int y, CursorType theCursorType, bool isTouch = false)
	{
		HitResult hitResult = mApp.mBoard.ToolHitTest(x, y, posScaled: true, isTouch);
		Plant plant = null;
		if (hitResult.mObjectType == GameObjectType.Plant)
		{
			plant = (Plant)hitResult.mObject;
		}
		if ((plant == null || plant.mPottedPlantIndex == -1) && (theCursorType != CursorType.WateringCan || mApp.mPlayerInfo.mPurchases[13] == 0) && theCursorType != CursorType.Chocolate)
		{
			mBoard.ClearCursor();
			return;
		}
		if (theCursorType == CursorType.Chocolate)
		{
			Debug.ASSERT(mApp.mPlayerInfo.mPurchases[26] > 1000);
			GridItem stinky = GetStinky();
			if (!IsStinkyHighOnChocolate() && stinky != null && (stinky.mHighlighted || hitResult.mObjectType == GameObjectType.Stinky))
			{
				WakeStinky();
				mApp.AddTodParticle(stinky.mPosX + 40f, stinky.mPosY + 40f, stinky.mRenderOrder + 1, ParticleEffect.PresentPickup);
				mApp.mPlayerInfo.mLastStinkyChocolateTime = aNow;
				mApp.mPlayerInfo.mPurchases[26]--;
				mApp.PlayFoley(FoleyType.Wakeup);
				mApp.PlaySample(Resources.SOUND_MINDCONTROLLED);
			}
			if (plant != null)
			{
				mApp.mPlayerInfo.mPurchases[26]--;
				FeedChocolateToPlant(plant);
				mApp.PlayFoley(FoleyType.Wakeup);
			}
		}
		if (plant != null || (theCursorType == CursorType.WateringCan && mApp.mPlayerInfo.mPurchases[13] != 0))
		{
			GridItem newGridItem = GridItem.GetNewGridItem();
			newGridItem.mGridItemType = GridItemType.ZenTool;
			mBoard.mGridItems.Add(newGridItem);
			if (plant != null)
			{
				newGridItem.mGridX = plant.mPlantCol;
				newGridItem.mGridY = plant.mRow;
				newGridItem.mPosX = plant.mX + 40;
				newGridItem.mPosY = plant.mY + 40;
			}
			newGridItem.mRenderOrder = 800000;
			if (theCursorType == CursorType.WateringCan && mApp.mPlayerInfo.mPurchases[13] == 1)
			{
				newGridItem.mPosX = x;
				newGridItem.mPosY = y;
				Reanimation reanimation = mApp.AddReanimation(x + Constants.ZenGarden_GoldenWater_Pos.X, y + Constants.ZenGarden_GoldenWater_Pos.Y, 0, ReanimationType.ZengardenWateringcan, theDoScalePos: true);
				reanimation.PlayReanim("anim_water_area", ReanimLoopType.PlayOnceAndHold, 0, 8f);
				newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation);
				newGridItem.mGridItemState = GridItemState.ZenToolGoldWateringCan;
				mApp.PlayFoley(FoleyType.Watering);
			}
			else if (theCursorType == CursorType.WateringCan && mApp.mPlayerInfo.mPurchases[13] != 0)
			{
				GridItem gridItem = newGridItem;
				for (int i = 0; i < mBoard.mPlants.Count; i++)
				{
					Plant plant2 = mBoard.mPlants[i];
					if (!plant2.mDead)
					{
						if (gridItem == null)
						{
							gridItem = GridItem.GetNewGridItem();
							gridItem.mGridItemType = GridItemType.ZenTool;
							mBoard.mGridItems.Add(gridItem);
							gridItem.mRenderOrder = 800000;
						}
						gridItem.mGridX = plant2.mPlantCol;
						gridItem.mGridY = plant2.mRow;
						gridItem.mPosX = plant2.mX + 40;
						gridItem.mPosY = plant2.mY + 40;
						Reanimation reanimation2 = mApp.AddReanimation(plant2.mX + 32, plant2.mY, 0, ReanimationType.ZengardenWateringcanDiamond);
						reanimation2.PlayReanim("anim_water", ReanimLoopType.PlayOnceAndHold, 0, 0f);
						gridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation2);
						gridItem.mGridItemState = GridItemState.ZenToolWateringCan;
						gridItem = null;
					}
				}
				mApp.PlayFoley(FoleyType.Watering);
			}
			else
			{
				switch (theCursorType)
				{
				case CursorType.WateringCan:
				{
					Reanimation reanimation6 = mApp.AddReanimation(plant.mX + 32, plant.mY, 0, ReanimationType.ZengardenWateringcan);
					reanimation6.PlayReanim("anim_water", ReanimLoopType.PlayOnceAndHold, 0, 0f);
					newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation6);
					newGridItem.mGridItemState = GridItemState.ZenToolWateringCan;
					mApp.PlayFoley(FoleyType.Watering);
					break;
				}
				case CursorType.Fertilizer:
				{
					Reanimation reanimation5 = mApp.AddReanimation(plant.mX, plant.mY, 0, ReanimationType.ZengardenFertilizer);
					reanimation5.mLoopType = ReanimLoopType.PlayOnceAndHold;
					newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation5);
					newGridItem.mGridItemState = GridItemState.ZenToolFertilizer;
					mApp.PlayFoley(FoleyType.Fertilizer);
					Debug.ASSERT(mApp.mPlayerInfo.mPurchases[14] > 1000);
					mApp.mPlayerInfo.mPurchases[14]--;
					break;
				}
				case CursorType.BugSpray:
				{
					Reanimation reanimation4 = mApp.AddReanimation(plant.mX + 54, plant.mY, 0, ReanimationType.ZengardenBugspray);
					reanimation4.mLoopType = ReanimLoopType.PlayOnceAndHold;
					newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation4);
					newGridItem.mGridItemState = GridItemState.ZenToolBugSpray;
					mApp.PlayFoley(FoleyType.Bugspray);
					Debug.ASSERT(mApp.mPlayerInfo.mPurchases[15] > 1000);
					mApp.mPlayerInfo.mPurchases[15]--;
					break;
				}
				case CursorType.Phonograph:
				{
					Reanimation reanimation3 = mApp.AddReanimation(plant.mX + 20, plant.mY + 34, 0, ReanimationType.ZengardenPhonograph);
					reanimation3.mAnimRate = 20f;
					reanimation3.mLoopType = ReanimLoopType.Loop;
					newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation3);
					newGridItem.mGridItemState = GridItemState.ZenToolPhonograph;
					mApp.PlayFoley(FoleyType.Phonograph);
					break;
				}
				}
			}
		}
		mBoard.ClearCursor();
	}

	public void DrawPlantOverlay(Graphics g, Plant thePlant)
	{
		if (mIsInteracting || mInteracting != null || thePlant.mPottedPlantIndex == -1)
		{
			return;
		}
		PottedPlant thePottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		PottedPlantNeed plantsNeed = mApp.mZenGarden.GetPlantsNeed(thePottedPlant);
		if (plantsNeed != PottedPlantNeed.None)
		{
			g.DrawImage(AtlasResources.IMAGE_PLANTSPEECHBUBBLE, Constants.ZenGarden_PlantSpeechBubble_Pos.X, Constants.ZenGarden_PlantSpeechBubble_Pos.Y);
			switch (plantsNeed)
			{
			case PottedPlantNeed.Water:
				g.DrawImage(AtlasResources.IMAGE_WATERDROP, Constants.ZenGarden_WaterDrop_Pos.X, Constants.ZenGarden_WaterDrop_Pos.Y);
				break;
			case PottedPlantNeed.Fertilizer:
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_FERTILIZER_BAG3, Constants.ZenGarden_Fertiliser_Pos.X, Constants.ZenGarden_Fertiliser_Pos.Y, 0.5f, 0.5f);
				break;
			case PottedPlantNeed.Bugspray:
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_BUGSPRAY_BOTTLE, Constants.ZenGarden_BugSpray_Pos.X, Constants.ZenGarden_BugSpray_Pos.Y, 0.5f, 0.5f);
				break;
			case PottedPlantNeed.Phonograph:
				TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_PHONOGRAPH, Constants.ZenGarden_Phonograph_Pos.X, Constants.ZenGarden_Phonograph_Pos.Y, 0.5f, 0.5f);
				break;
			}
		}
		if (mBoard.mCursorObject.mCursorType == CursorType.WateringCan && (uint)mApp.mPlayerInfo.mPurchases[13] >= 2u)
		{
			SexyTransform2D sexyTransform2D = new SexyTransform2D(loadIdentity: true);
			sexyTransform2D.RotateDeg(-45f);
			sexyTransform2D.Translate((float)(thePlant.mX + 170) * Constants.S, (float)thePlant.mY * Constants.S);
			TodCommon.TodBltMatrix(g, AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_DIAMOND, ref sexyTransform2D.mMatrix, default, new SexyColor(255, 255, 255, 192), Graphics.DrawMode.DRAWMODE_NORMAL, new TRect(0, 0, AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_DIAMOND.mWidth, AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_DIAMOND.mHeight), clip: false);
		}
	}

	public PottedPlant PottedPlantFromIndex(int thePottedPlantIndex)
	{
		Debug.ASSERT(thePottedPlantIndex >= 0 && thePottedPlantIndex < mApp.mPlayerInfo.mNumPottedPlants);
		return mApp.mPlayerInfo.mPottedPlant[thePottedPlantIndex];
	}

	public bool WasPlantNeedFulfilledToday(PottedPlant thePottedPlant)
	{
		TimeSpan timeSpan = aNow - thePottedPlant.mLastNeedFulfilledTime;
		double totalSeconds = timeSpan.TotalSeconds;
		return timeSpan.TotalDays < 1.0;
	}

	public void PottedPlantUpdate(Plant thePlant)
	{
		if (thePlant.UseHeadReanim())
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(thePlant?.mHeadReanimID);
			if (thePlant.mState == PlantState.ZenGardenInteracting && reanimation != null && reanimation.mLoopCount == 1)
			{
				reanimation.mAnimRate = TodCommon.RandRangeFloat(15f, 20f);
				reanimation.StartBlend(20);
				reanimation.mLoopType = ReanimLoopType.Loop;
				reanimation.mLoopCount = 0;
				if (reanimation.TrackExists("anim_head_garden_idle"))
				{
					reanimation.SetFramesForLayer("anim_head_garden_idle");
				}
				else
				{
					reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
				}
				SetPlantAnimSpeed(thePlant);
				mIsInteractingInTouch = false;
				ShowInteractingButtons(doShow: true);
				thePlant.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, thePlant.mY);
				thePlant.mState = PlantState.ZenGardenBackFromInteracting;
			}
		}
		else
		{
			Reanimation reanimation = mApp.ReanimationTryToGet(thePlant?.mBodyReanimID);
			if (thePlant.mState == PlantState.ZenGardenInteracting && reanimation != null && (1f - reanimation.mAnimTime) * (float)reanimation.mFrameCount / reanimation.mAnimRate < 0.2f)
			{
				int mPottedPlantIndex = thePlant.mPottedPlantIndex;
				if (!reanimation.TrackExists(GlobalMembersReanimIds.ReanimTrackId_anim_zengarden))
				{
					thePlant.PlayIdleAnim(reanimation.mDefinition.mFPS);
				}
				else
				{
					thePlant.PlayBodyReanim(GlobalMembersReanimIds.ReanimTrackId_anim_zengarden, ReanimLoopType.Loop, 20, reanimation.mDefinition.mFPS);
				}
				SetPlantAnimSpeed(thePlant);
				mIsInteractingInTouch = false;
				ShowInteractingButtons(doShow: true);
				mApp.mZenGarden?.InteractionFocusOnPlant(thePlant, 3f, 50);
				thePlant.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, thePlant.mY);
				thePlant.mState = PlantState.ZenGardenBackFromInteracting;
			}
		}
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		if (pottedPlant.mLastWateredTime > aNow || pottedPlant.mLastNeedFulfilledTime > aNow || pottedPlant.mLastFertilizedTime > aNow || pottedPlant.mLastChocolateTime > aNow)
		{
			ResetPlantTimers(pottedPlant);
		}
		if (!thePlant.mIsAsleep)
		{
			if (thePlant.mStateCountdown > 0)
			{
				thePlant.mStateCountdown--;
			}
			if (pottedPlant.mPlantAge == PottedPlantAge.Full && WasPlantNeedFulfilledToday(pottedPlant))
			{
				PlantUpdateProduction(thePlant);
			}
			UpdatePlantEffectState(thePlant);
		}
	}

	public void AddHappyEffect(Plant thePlant)
	{
		Plant topPlantAt = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyUnderPlant);
		if (topPlantAt == null)
		{
			thePlant.AddAttachedParticle(thePlant.mX + 40, thePlant.mY + 60, thePlant.mRenderOrder - 1, ParticleEffect.PottedZenGlow);
		}
		else if (Plant.IsAquatic(thePlant.mSeedType))
		{
			topPlantAt.AddAttachedParticle(topPlantAt.mX + 40, topPlantAt.mY + 61, topPlantAt.mRenderOrder - 1, ParticleEffect.PottedWaterPlantGlow);
		}
		else
		{
			topPlantAt.AddAttachedParticle(topPlantAt.mX + 40, topPlantAt.mY + 63, topPlantAt.mRenderOrder - 1, ParticleEffect.PottedPlantGlow);
		}
	}

	public void RemoveHappyEffect(Plant thePlant)
	{
		Plant topPlantAt = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyUnderPlant);
		((topPlantAt == null) ? mApp.ParticleTryToGet(thePlant.mParticleID) : mApp.ParticleTryToGet(topPlantAt.mParticleID))?.ParticleSystemDie();
	}

	public void PlantUpdateProduction(Plant thePlant)
	{
		if (GlobalStaticVars.gFastMo && mApp.mAppCounter % GlobalStaticVars.gFastSlowMoNum != 0)
		{
			return;
		}
		thePlant.mLaunchCounter--;
		if (mApp.ReanimationTryToGet(thePlant.mBodyReanimID) == null || (!thePlant.mBodyReanimID.IsAnimPlaying("anim_garden1") && !thePlant.mBodyReanimID.IsAnimPlaying("anim_garden2")))
		{
			SetPlantAnimSpeed(thePlant);
		}
		PottedPlant thePottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		if (PlantHighOnChocolate(thePottedPlant))
		{
			thePlant.mLaunchCounter--;
		}
		if (thePlant.mLaunchCounter <= 0)
		{
			PlantSetLaunchCounter(thePlant);
			mApp.PlayFoley(FoleyType.SpawnSun);
			int num = RandomNumbers.NextNumber(1000);
			int theTimeAge = PlantGetMinutesSinceHappy(thePlant);
			num += TodCommon.TodAnimateCurve(5, 30, theTimeAge, 0, 80, TodCurves.Linear);
			CoinType theCoinType = CoinType.Silver;
			if (num < 100)
			{
				theCoinType = CoinType.Gold;
			}
			mBoard.AddCoin(thePlant.mX, thePlant.mY, theCoinType, CoinMotion.Coin);
		}
	}

	public bool CanDropPottedPlantLoot()
	{
		if (mApp.HasFinishedAdventure())
		{
			return !mApp.mZenGarden.IsZenGardenFull(theIncludeDroppedPresents: true);
		}
		return false;
	}

	public void ShowTutorialArrowOnWateringCan()
	{
		TRect zenButtonRect = mBoard.GetZenButtonRect(GameObjectType.WateringCan);
		float screenX = zenButtonRect.mX + Constants.ZenGarden_TutorialArrow_Offset;
		float screenY = (float)zenButtonRect.mY + Constants.S * 10f;
		if (mBoard.mCameraEnabled && mBoard.mCamera != null)
		{
			mBoard.mCamera?.ScreenToBoardReplace(ref screenX, ref screenY);
		}
		mBoard.TutorialArrowShow((int)screenX, (int)screenY);
		mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_PICK_UP_WATER]", MessageStyle.ZenGardenLong, AdviceType.None);
		mBoard.mTutorialState = TutorialState.ZenGardenPickupWater;
		mApp.mPlayerInfo.mIsInZenTutorial = true;
		mApp.mPlayerInfo.mZenTutorialMessage = 22;
	}

	public bool PlantsNeedWater()
	{
		for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
		{
			PottedPlant thePottedPlant = PottedPlantFromIndex(i);
			PottedPlantNeed plantsNeed = mApp.mZenGarden.GetPlantsNeed(thePottedPlant);
			if (plantsNeed == PottedPlantNeed.Water)
			{
				return true;
			}
		}
		return false;
	}

	public void ZenGardenStart()
	{
	}

	public void UpdatePlantEffectState(Plant thePlant)
	{
		PottedPlant thePottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		PlantState mState = thePlant.mState;
		PottedPlantNeed plantsNeed = GetPlantsNeed(thePottedPlant);
		if (mState == PlantState.ZenGardenInteracting)
		{
			return;
		}
		switch (plantsNeed)
		{
		case PottedPlantNeed.Water:
			thePlant.mState = PlantState.Notready;
			break;
		case PottedPlantNeed.None:
			if (WasPlantNeedFulfilledToday(thePottedPlant))
			{
				thePlant.mState = PlantState.ZenGardenHappy;
			}
			else if (thePlant.mIsAsleep)
			{
				thePlant.mState = PlantState.Notready;
			}
			else
			{
				thePlant.mState = PlantState.ZenGardenWatered;
			}
			break;
		default:
			thePlant.mState = PlantState.ZenGardenNeedy;
			break;
		}
		if (mState == thePlant.mState)
		{
			return;
		}
		Plant topPlantAt = mBoard.GetTopPlantAt(thePlant.mPlantCol, thePlant.mRow, TopPlant.OnlyUnderPlant);
		if (topPlantAt != null && !Plant.IsAquatic(thePlant.mSeedType))
		{
			Reanimation reanimation = mApp.ReanimationGet(topPlantAt.mBodyReanimID);
			if (thePlant.mState == PlantState.ZenGardenWatered || thePlant.mState == PlantState.ZenGardenNeedy || thePlant.mState == PlantState.ZenGardenHappy)
			{
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_pot_top, AtlasResources.IMAGE_REANIM_POT_TOP_DARK);
			}
			else
			{
				Image theImage = null;
				reanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_pot_top, theImage);
			}
		}
		if (mState == PlantState.ZenGardenHappy)
		{
			RemoveHappyEffect(thePlant);
		}
		if (thePlant.mState == PlantState.ZenGardenHappy)
		{
			thePlant.SetSleeping(theIsAsleep: false);
			AddHappyEffect(thePlant);
		}
		else if (Plant.IsNocturnal(thePlant.mSeedType) && !mBoard.StageIsNight())
		{
			thePlant.SetSleeping(theIsAsleep: true);
		}
	}

	public void ZenToolUpdate(GridItem theZenTool)
	{
		Reanimation reanimation = mApp.ReanimationTryToGet(theZenTool.mGridItemReanimID);
		if (reanimation != null)
		{
			int num = 1;
			if (theZenTool.mGridItemState == GridItemState.ZenToolPhonograph)
			{
				num = 2;
			}
			if (reanimation.mLoopCount >= num)
			{
				DoFeedingTool((int)theZenTool.mPosX, (int)theZenTool.mPosY, theZenTool.mGridItemState);
				theZenTool.GridItemDie();
			}
		}
	}

	public void DoFeedingTool(int x, int y, GridItemState theToolType)
	{
		if (theToolType == GridItemState.ZenToolGoldWateringCan)
		{
			int count = mBoard.mPlants.Count;
			for (int i = 0; i < count; i++)
			{
				Plant plant = mBoard.mPlants[i];
				if (!plant.mDead && mBoard.IsPlantInGoldWateringCanRange(x, y, plant))
				{
					PottedPlant thePottedPlant = PottedPlantFromIndex(plant.mPottedPlantIndex);
					PottedPlantNeed plantsNeed = GetPlantsNeed(thePottedPlant);
					if (plantsNeed == PottedPlantNeed.Water)
					{
						PlantWatered(plant);
					}
				}
			}
			return;
		}
		int theGridX = PixelToGridX(x, y);
		int theGridY = PixelToGridY(x, y);
		Plant topPlantAt = mBoard.GetTopPlantAt(theGridX, theGridY, TopPlant.ZenToolOrder);
		if (topPlantAt == null)
		{
			return;
		}
		PottedPlant thePottedPlant2 = PottedPlantFromIndex(topPlantAt.mPottedPlantIndex);
		PottedPlantNeed plantsNeed2 = GetPlantsNeed(thePottedPlant2);
		if (plantsNeed2 == PottedPlantNeed.Water && theToolType == GridItemState.ZenToolWateringCan)
		{
			PlantWatered(topPlantAt);
		}
		else if (plantsNeed2 == PottedPlantNeed.Fertilizer && theToolType == GridItemState.ZenToolFertilizer)
		{
			PlantFertilized(topPlantAt);
		}
		else if (plantsNeed2 == PottedPlantNeed.Bugspray && theToolType == GridItemState.ZenToolBugSpray)
		{
			PlantFulfillNeed(topPlantAt);
		}
		else if (plantsNeed2 == PottedPlantNeed.Phonograph && theToolType == GridItemState.ZenToolPhonograph)
		{
			PlantFulfillNeed(topPlantAt);
		}
		if (mBoard.mTutorialState == TutorialState.ZenGardenFertilizePlants && theToolType == GridItemState.ZenToolFertilizer)
		{
			if (AllPlantsHaveBeenFertilized())
			{
				mApp.mBoard.mTutorialState = TutorialState.ZenGardenCompleted;
				mApp.mPlayerInfo.mZenTutorialMessage = 27;
				mApp.mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_CONTINUE_ADVENTURE]", MessageStyle.HintTallFast, AdviceType.None);
				mBoard.mMenuButton.mDisabled = false;
				mBoard.mMenuButton.mBtnNoDraw = false;
			}
			else if (mApp.mPlayerInfo.mPurchases[14] == 1000)
			{
				mApp.mPlayerInfo.mPurchases[14] = 1005;
				mApp.mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_NEED_MORE_FERTILIZER]", MessageStyle.HintTallFast, AdviceType.None);
			}
		}
	}

	public void AddStinky()
	{
		if (HasPurchasedStinky() && (mGardenType == GardenType.Main || mGardenType == GardenType.Main2 || mGardenType == GardenType.Night))
		{
			if (!mApp.mPlayerInfo.mHasSeenStinky)
			{
				mApp.mPlayerInfo.mHasSeenStinky = true;
				mApp.mPlayerInfo.mPurchases[20] = GetStinkyTime();
			}
			GridItem newGridItem = GridItem.GetNewGridItem();
			newGridItem.mGridItemType = GridItemType.Stinky;
			newGridItem.mPosX = mApp.mPlayerInfo.mStinkyPosX;
			newGridItem.mPosY = mApp.mPlayerInfo.mStinkyPosY;
			newGridItem.mGoalX = newGridItem.mPosX;
			newGridItem.mGoalY = newGridItem.mPosY;
			mBoard.mGridItems.Add(newGridItem);
			Reanimation reanimation = mApp.AddReanimation(newGridItem.mPosX * Constants.S, newGridItem.mPosY * Constants.S, 0, ReanimationType.Stinky);
			reanimation.OverrideScale(0.8f, 0.8f);
			newGridItem.mGridItemReanimID = mApp.ReanimationGetID(reanimation);
			if (mApp.mPlayerInfo.mStinkyPosX == 0)
			{
				StinkyPickGoal(newGridItem);
				newGridItem.mPosX = newGridItem.mGoalX;
				newGridItem.mPosY = newGridItem.mGoalY;
			}
			if (ShouldStinkyBeAwake())
			{
				reanimation.PlayReanim(Reanimation.ReanimTrackId_anim_crawl, ReanimLoopType.Loop, 0, 6f);
				newGridItem.mGridItemState = GridItemState.StinkyWalkingLeft;
			}
			else
			{
				newGridItem.mPosY = Constants.STINKY_SLEEP_POS_Y;
				StinkyFinishFallingAsleep(newGridItem, 0);
			}
			newGridItem.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, (int)newGridItem.mPosY - 30);
			reanimation.SetPosition(newGridItem.mPosX * Constants.S, newGridItem.mPosY * Constants.S);
		}
	}

	private int GetStinkyTime()
	{
		return (int)(TimeSpan.FromTicks(aNow.Ticks).TotalSeconds - (double)STINKY_BASE_TIME);
	}

	public void StinkyUpdate(GridItem theStinky)
	{
		Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
		if (mApp.mPlayerInfo.mLastStinkyChocolateTime > aNow || mApp.mPlayerInfo.mPurchases[20] > GetStinkyTime())
		{
			ResetStinkyTimers();
		}
		bool flag = IsStinkyHighOnChocolate();
		UpdateStinkyMotionTrail(theStinky, flag);
		if (theStinky.mGridItemState == GridItemState.StinkyFallingAsleep)
		{
			if (reanimation.mLoopCount > 0)
			{
				StinkyFinishFallingAsleep(theStinky, 20);
			}
			return;
		}
		if (theStinky.mGridItemState == GridItemState.StinkySleeping)
		{
			ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName("shell");
			Reanimation reanimation2 = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName.mAttachmentID);
			Debug.ASSERT(reanimation2 != null);
			if (mBoard.mCursorObject.mCursorType == CursorType.Chocolate)
			{
				reanimation2.AssignRenderGroupToPrefix("z", -1);
			}
			else
			{
				reanimation2.AssignRenderGroupToPrefix("z", 0);
			}
			if (ShouldStinkyBeAwake())
			{
				StinkyWakeUp(theStinky);
			}
			return;
		}
		if (theStinky.mGridItemState == GridItemState.StinkyWakingUp)
		{
			if (reanimation.mLoopCount > 0)
			{
				theStinky.mGridItemState = GridItemState.StinkyWalkingLeft;
				reanimation.PlayReanim(Reanimation.ReanimTrackId_anim_crawl, ReanimLoopType.Loop, 10, 6f);
				StinkyPickGoal(theStinky);
			}
			return;
		}
		if (!ShouldStinkyBeAwake())
		{
			if (theStinky.mPosY < (float)Constants.STINKY_SLEEP_POS_Y)
			{
				if (theStinky.mGoalY != (float)Constants.STINKY_SLEEP_POS_Y)
				{
					theStinky.mGoalY = (float)Constants.STINKY_SLEEP_POS_Y + 10f;
				}
			}
			else
			{
				if (theStinky.mGridItemState == GridItemState.StinkyWalkingLeft)
				{
					StinkyStartFallingAsleep(theStinky);
					return;
				}
				if (theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
				{
					Reanimation reanimation3 = mApp.ReanimationGet(theStinky.mGridItemReanimID);
					theStinky.mGridItemState = GridItemState.StinkyTurningLeft;
					reanimation3.PlayReanim("turn", ReanimLoopType.PlayOnceAndHold, 10, 6f);
					theStinky.mMotionTrailCount = 0;
					theStinky.mGoalX = theStinky.mPosX;
					theStinky.mGoalY = theStinky.mPosY;
					return;
				}
			}
		}
		if (theStinky.mGridItemCounter > 0)
		{
			theStinky.mGridItemCounter--;
		}
		SexyVector2 sexyVector = new SexyVector2(theStinky.mPosX, theStinky.mPosY);
		SexyVector2 sexyVector2 = default;
		Coin theCoin = null;
		while (mBoard.IterateCoins(ref theCoin))
		{
			if (!theCoin.mIsBeingCollected)
			{
				sexyVector2.x = theCoin.mPosX;
				sexyVector2.y = theCoin.mPosY + 30f;
				float num = (sexyVector - sexyVector2).Magnitude();
				if (num < 20f)
				{
					theCoin.PlayCollectSound();
					theCoin.Collect();
				}
			}
		}
		if (theStinky.mGridItemState == GridItemState.StinkyWalkingLeft || theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
		{
			if (mBoard.mCursorObject.mCursorType == CursorType.Chocolate && !IsStinkyHighOnChocolate())
			{
				if (!reanimation.IsAnimPlaying("anim_idle"))
				{
					reanimation.PlayReanim("anim_idle", ReanimLoopType.Loop, 10, 6f);
				}
			}
			else if (!reanimation.IsAnimPlaying(Reanimation.ReanimTrackId_anim_crawl))
			{
				reanimation.PlayReanim(Reanimation.ReanimTrackId_anim_crawl, ReanimLoopType.Loop, 10, 6f);
			}
		}
		float value = theStinky.mPosX - theStinky.mGoalX;
		float num2 = theStinky.mPosY - theStinky.mGoalY;
		float num3 = 0.5f;
		float num4 = reanimation.GetTrackVelocity(Reanimation.ReanimTrackId__ground) * 5f;
		if (flag)
		{
			num3 = 1f;
			num4 = Math.Max(num4, 0.5f);
		}
		else if (mBoard.mCursorObject.mCursorType == CursorType.Chocolate)
		{
			num3 = 0f;
			num4 = 0f;
		}
		num3 *= TodCommon.TodAnimateCurveFloatTime(20f, 5f, Math.Abs(num2), 1f, 0.2f, TodCurves.Linear);
		if (theStinky.mGridItemState == GridItemState.StinkyWalkingLeft)
		{
			theStinky.mPosX -= num4;
			if (theStinky.mPosX < theStinky.mGoalX)
			{
				theStinky.mPosX = theStinky.mGoalX;
			}
		}
		else if (theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
		{
			theStinky.mPosX += num4;
			if (theStinky.mPosX > theStinky.mGoalX)
			{
				theStinky.mPosX = theStinky.mGoalX;
			}
		}
		if (theStinky.mGridItemState == GridItemState.StinkyWalkingLeft || theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
		{
			if (Math.Abs(num2) < num3)
			{
				theStinky.mPosY = theStinky.mGoalY;
			}
			else if (num2 > 0f)
			{
				theStinky.mPosY -= num3;
			}
			else
			{
				theStinky.mPosY += num3;
			}
			if (Math.Abs(value) < 5f && Math.Abs(num2) < 5f)
			{
				StinkyPickGoal(theStinky);
			}
			else if (theStinky.mGridItemCounter == 0)
			{
				StinkyPickGoal(theStinky);
			}
		}
		if (theStinky.mGridItemState == GridItemState.StinkyTurningLeft)
		{
			if (reanimation.mLoopCount > 0)
			{
				theStinky.mGridItemState = GridItemState.StinkyWalkingLeft;
				reanimation.PlayReanim(Reanimation.ReanimTrackId_anim_crawl, ReanimLoopType.Loop, 10, 6f);
			}
		}
		else if (theStinky.mGridItemState == GridItemState.StinkyTurningRight && reanimation.mLoopCount > 0)
		{
			theStinky.mGridItemState = GridItemState.StinkyWalkingRight;
			reanimation.PlayReanim(Reanimation.ReanimTrackId_anim_crawl, ReanimLoopType.Loop, 10, 6f);
		}
		StinkyAnimRateUpdate(theStinky);
		if (theStinky.mGridItemState == GridItemState.StinkyWalkingRight || theStinky.mGridItemState == GridItemState.StinkyTurningLeft)
		{
			reanimation.OverrideScale(-0.8f, 0.8f);
			reanimation.SetPosition((theStinky.mPosX + 69f) * Constants.S, theStinky.mPosY * Constants.S);
		}
		else
		{
			reanimation.OverrideScale(0.8f, 0.8f);
			reanimation.SetPosition(theStinky.mPosX * Constants.S, theStinky.mPosY * Constants.S);
		}
		theStinky.mRenderOrder = Board.MakeRenderOrder(RenderLayer.Plant, 0, (int)theStinky.mPosY - 30);
	}

	public void OpenStore()
	{
		LeaveGarden();
		StoreScreen storeScreen = mApp.ShowStoreScreen(this);
		storeScreen.SetupBackButtonForZenGarden();
		if (mBoard.mTutorialState == TutorialState.ZenGardenVisitStore)
		{
			storeScreen.SetupForIntro(2600);
			mApp.mPlayerInfo.mPurchases[14] = 1005;
		}
		storeScreen.mBackButton.SetLabel("[STORE_BACK_TO_GAME]");
		storeScreen.mPage = StorePage.Zen1;
	}

	public GridItem GetStinky()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.Stinky)
			{
				return theGridItem;
			}
		}
		return null;
	}

	public void StinkyPickGoal(GridItem theStinky)
	{
		float num = TodCommon.Distance2D(theStinky.mGoalX, theStinky.mGoalY, theStinky.mPosX, theStinky.mPosY);
		Coin coin = null;
		float num2 = 0f;
		Coin theCoin = null;
		while (mBoard.IterateCoins(ref theCoin))
		{
			if (!theCoin.mIsBeingCollected && theCoin.mPosY == (float)theCoin.mGroundY)
			{
				float num3 = TodCommon.Distance2D(theCoin.mPosX, theCoin.mPosY + 30f, theStinky.mPosX, theStinky.mPosY);
				if (theCoin.mType == CoinType.Gold)
				{
					num3 -= 40f;
				}
				else if (theCoin.mType == CoinType.Diamond)
				{
					num3 -= 80f;
				}
				float num4 = TodCommon.Distance2D(theCoin.mPosX, theCoin.mPosY + 30f, theStinky.mGoalX, theStinky.mGoalY);
				if (num4 < 5f)
				{
					num3 -= 20f;
				}
				if (num4 < 5f)
				{
					num3 += (float)TodCommon.TodAnimateCurve(3000, 6000, theCoin.mDisappearCounter, 0, -40, TodCurves.Linear);
				}
				if (coin == null || num3 < num2)
				{
					coin = theCoin;
					num2 = num3;
				}
			}
		}
		if (coin != null)
		{
			theStinky.mGoalX = coin.mPosX;
			theStinky.mGoalY = coin.mPosY + 30f;
		}
		else
		{
			if (num > 10f)
			{
				return;
			}
			TodWeightedGridArray[] array = new TodWeightedGridArray[Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = TodWeightedGridArray.GetNewTodWeightedGridArray();
			}
			int num5 = 0;
			int theCount = 0;
			SpecialGridPlacement[] specialGridPlacements = GetSpecialGridPlacements(ref theCount);
			Debug.ASSERT(theCount < Constants.GRIDSIZEX * Constants.MAX_GRIDSIZEY);
			for (int j = 0; j < theCount; j++)
			{
				SpecialGridPlacement specialGridPlacement = specialGridPlacements[j];
				Plant topPlantAt = mBoard.GetTopPlantAt(specialGridPlacement.mGridX, specialGridPlacement.mGridY, TopPlant.Any);
				array[num5].mX = specialGridPlacement.mPixelX + 15;
				array[num5].mY = specialGridPlacement.mPixelY + 80;
				if (topPlantAt != null)
				{
					array[num5].mWeight = 2000;
					array[num5].mWeight -= (int)Math.Abs((float)array[num5].mY - theStinky.mPosY);
				}
				else
				{
					array[num5].mWeight = 1;
				}
				num5++;
			}
			TodWeightedGridArray todWeightedGridArray = TodCommon.TodPickFromWeightedGridArray(array, num5);
			theStinky.mGoalX = todWeightedGridArray.mX;
			theStinky.mGoalY = todWeightedGridArray.mY;
		}
		theStinky.mGridItemCounter = 100;
		if (theStinky.mGoalX < theStinky.mPosX && theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
		{
			Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
			theStinky.mGridItemState = GridItemState.StinkyTurningLeft;
			reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_turn, ReanimLoopType.PlayOnceAndHold, 10, 6f);
			theStinky.mMotionTrailCount = 0;
		}
		else if (theStinky.mGoalX > theStinky.mPosX && theStinky.mGridItemState == GridItemState.StinkyWalkingLeft)
		{
			Reanimation reanimation2 = mApp.ReanimationGet(theStinky.mGridItemReanimID);
			theStinky.mGridItemState = GridItemState.StinkyTurningRight;
			reanimation2.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_turn, ReanimLoopType.PlayOnceAndHold, 10, 6f);
			theStinky.mMotionTrailCount = 0;
		}
	}

	public bool PlantShouldRefreshNeed(PottedPlant thePottedPlant)
	{
		TimeSpan timeSpan = aNow - thePottedPlant.mLastWateredTime;
		if (timeSpan.TotalSeconds >= 3600.0)
		{
			return timeSpan.TotalDays >= 1.0;
		}
		return false;
	}

	public void PlantFertilized(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		pottedPlant.mLastFertilizedTime = aNow;
		pottedPlant.mPlantNeed = PottedPlantNeed.None;
		pottedPlant.mTimesFed = 0;
		pottedPlant.mPlantAge++;
		if (pottedPlant.mPlantAge == PottedPlantAge.Small)
		{
			RemovePottedPlant(thePlant);
			PlacePottedPlant(thePlant.mPottedPlantIndex);
			mApp.PlaySample(Resources.SOUND_LOADINGBAR_FLOWER);
		}
		else
		{
			thePlant.mStateCountdown = 100;
			mApp.PlayFoley(FoleyType.Plantgrow);
		}
		mApp.PlayFoley(FoleyType.SpawnSun);
		if (pottedPlant.mPlantAge == PottedPlantAge.Small)
		{
			mBoard.AddCoin(thePlant.mX + 40, thePlant.mY, CoinType.Gold, CoinMotion.Coin);
		}
		else if (pottedPlant.mPlantAge == PottedPlantAge.Medium)
		{
			mBoard.AddCoin(thePlant.mX + 30, thePlant.mY, CoinType.Gold, CoinMotion.Coin);
			mBoard.AddCoin(thePlant.mX + 50, thePlant.mY, CoinType.Gold, CoinMotion.Coin);
		}
		else if (pottedPlant.mPlantAge == PottedPlantAge.Full)
		{
			if (pottedPlant.mSeedType == SeedType.Marigold)
			{
				mBoard.AddCoin(thePlant.mX + 40, thePlant.mY, CoinType.Diamond, CoinMotion.Coin);
				return;
			}
			mBoard.AddCoin(thePlant.mX + 10, thePlant.mY, CoinType.Diamond, CoinMotion.Coin);
			mBoard.AddCoin(thePlant.mX + 70, thePlant.mY, CoinType.Diamond, CoinMotion.Coin);
		}
	}

	public bool WasPlantFertilizedInLastHour(PottedPlant thePottedPlant)
	{
		return (aNow - thePottedPlant.mLastFertilizedTime).TotalSeconds < 3600.0;
	}

	public void SetupForZenTutorial()
	{
		mBoard.mMenuButton.SetLabel("[CONTINUE_BUTTON]");
		mBoard.mStoreButton.mDisabled = true;
		mBoard.mStoreButton.mBtnNoDraw = true;
		mBoard.mMenuButton.mDisabled = true;
		mBoard.mMenuButton.mBtnNoDraw = true;
		mIsTutorial = true;
		if (mApp.mPlayerInfo.mIsDaveTalkingZenTutorial)
		{
			mApp.CrazyDaveEnter();
			mApp.CrazyDaveTalkIndex(mApp.mPlayerInfo.mZenTutorialMessage);
			return;
		}
		if (!mApp.mPlayerInfo.mIsInZenTutorial)
		{
			mApp.mPlayerInfo.mIsDaveTalkingZenTutorial = true;
			mApp.mPlayerInfo.mZenTutorialMessage = 2050;
			mApp.CrazyDaveEnter();
			mApp.CrazyDaveTalkIndex(2050);
			return;
		}
		mBoard.mTutorialState = (TutorialState)mApp.mPlayerInfo.mZenTutorialMessage;
		if (mBoard.mTutorialState == TutorialState.ZenGardenPickupWater)
		{
			ShowTutorialArrowOnWateringCan();
		}
		else if (mBoard.mTutorialState == TutorialState.ZenGardenKeepWatering)
		{
			mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_KEEP_WATERING]", MessageStyle.ZenGardenLong, AdviceType.None);
		}
		else if (mBoard.mTutorialState == TutorialState.ZenGardenVisitStore)
		{
			mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_VISIT_STORE]", MessageStyle.HintTallLong, AdviceType.None);
			mBoard.mStoreButton.mDisabled = false;
			mBoard.mStoreButton.mBtnNoDraw = false;
		}
		else if (mBoard.mTutorialState == TutorialState.ZenGardenFertilizePlants)
		{
			mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_FERTILIZE]", MessageStyle.ZenGardenLong, AdviceType.None);
		}
		else if (mBoard.mTutorialState == TutorialState.ZenGardenCompleted)
		{
			mIsTutorial = false;
			mApp.mPlayerInfo.mZenGardenTutorialComplete = true;
			mApp.mPlayerInfo.mIsInZenTutorial = false;
		}
	}

	public bool HasPurchasedStinky()
	{
		return mApp.mPlayerInfo.mPurchases[20] != 0;
	}

	public int CountPlantsNeedingFertilizer()
	{
		int num = 0;
		for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
		{
			PottedPlant thePottedPlant = PottedPlantFromIndex(i);
			PottedPlantNeed plantsNeed = mApp.mZenGarden.GetPlantsNeed(thePottedPlant);
			if (plantsNeed == PottedPlantNeed.Fertilizer)
			{
				num++;
			}
		}
		return num;
	}

	public bool AllPlantsHaveBeenFertilized()
	{
		for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
		{
			PottedPlant pottedPlant = PottedPlantFromIndex(i);
			if (pottedPlant.mPlantAge == PottedPlantAge.Sprout)
			{
				return false;
			}
		}
		return true;
	}

	public void WakeStinky()
	{
		mApp.mPlayerInfo.mPurchases[20] = GetStinkyTime();
		mApp.PlaySample(Resources.SOUND_TAP);
		mBoard.ClearAdvice(AdviceType.StinkySleeping);
		GlobalStaticVars.gLawnApp.mPlayerInfo.mHasWokenStinky = true;
	}

	public bool ShouldStinkyBeAwake()
	{
		if (IsStinkyHighOnChocolate())
		{
			return true;
		}
		int num = (int)(TimeSpan.FromTicks(aNow.Ticks).TotalSeconds - (double)STINKY_BASE_TIME - (double)mApp.mPlayerInfo.mPurchases[20]);
		int num2 = 180;
		return num <= num2;
	}

	public bool IsStinkySleeping()
	{
		GridItem stinky = GetStinky();
		if (stinky != null)
		{
			return stinky.mGridItemState == GridItemState.StinkySleeping;
		}
		return false;
	}

	public SeedType PickRandomSeedType()
	{
		SeedType[] array = new SeedType[54];
		int num = 0;
		for (int i = 0; i < 40; i++)
		{
			SeedType seedType = (SeedType)i;
			if (seedType != SeedType.Marigold && seedType != SeedType.Flowerpot)
			{
				array[num] = seedType;
				num++;
			}
		}
		Span<SeedType> span = stackalloc SeedType[5]
		{
			SeedType.SuperChomper,
			SeedType.PickledPepper,
			SeedType.FireShroom,
			SeedType.Agave,
			SeedType.Endoflame
		};
		Span<SeedType> span2 = span;
		for (int j = 0; j < span2.Length; j++)
		{
			SeedType seedType2 = span2[j];
			array[num] = seedType2;
			num++;
		}
		int num2 = RandomNumbers.NextNumber(num);
		SeedType seedType3 = array[num2];
		return array[num2];
	}

	public SeedType PickRandomSeedTypeWithNewPlants()
	{
		SeedType[] array = new SeedType[54];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			SeedType seedType = (SeedType)i;
			if (seedType != SeedType.Marigold && seedType != SeedType.Flowerpot && seedType != SeedType.Cobcannon)
			{
				array[num] = seedType;
				num++;
			}
		}
		int num2 = RandomNumbers.NextNumber(num);
		SeedType seedType2 = array[num2];
		return array[num2];
	}

	public void StinkyWakeUp(GridItem theStinky)
	{
		Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_out, ReanimLoopType.PlayOnceAndHold, 20, 6f);
		theStinky.mGridItemState = GridItemState.StinkyWakingUp;
		ReanimatorTrackInstance trackInstanceByName = reanimation.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_shell);
		Reanimation reanimation2 = GlobalMembersAttachment.FindReanimAttachment(trackInstanceByName.mAttachmentID);
		reanimation2.ReanimationDie();
		GlobalStaticVars.gLawnApp.mPlayerInfo.mHasWokenStinky = true;
	}

	public void StinkyStartFallingAsleep(GridItem theStinky)
	{
		Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_in, ReanimLoopType.PlayOnceAndHold, 20, 6f);
		theStinky.mGridItemState = GridItemState.StinkyFallingAsleep;
	}

	public void StinkyFinishFallingAsleep(GridItem theStinky, byte theBlendTime)
	{
		Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
		reanimation.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_out, ReanimLoopType.PlayOnceAndHold, theBlendTime, 0f);
		reanimation.mAnimRate = 0f;
		Reanimation reanimation2 = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Sleeping);
		reanimation2.mLoopType = ReanimLoopType.Loop;
		reanimation2.mAnimRate = 3f;
		int num = reanimation.FindTrackIndex(GlobalMembersReanimIds.ReanimTrackId_shell);
		ReanimatorTrackInstance reanimatorTrackInstance = reanimation.mTrackInstances[num];
		GlobalMembersAttachment.AttachReanim(ref reanimatorTrackInstance.mAttachmentID, reanimation2, 34f * Constants.S, 39f * Constants.S);
		theStinky.mGridItemState = GridItemState.StinkySleeping;
		if (!GlobalStaticVars.gLawnApp.mPlayerInfo.mHasWokenStinky)
		{
			mApp.mBoard.DisplayAdvice("[ADVICE_STINKY_SLEEPING]", MessageStyle.HintLong, AdviceType.StinkySleeping);
		}
	}

	public void AdvanceCrazyDaveDialog()
	{
		if (mApp.mCrazyDaveMessageIndex == -1 || mApp.GetDialog(Dialogs.DIALOG_STORE) != null || mApp.GetDialog(Dialogs.DIALOG_ZEN_SELL) != null)
		{
			return;
		}
		if (mApp.mCrazyDaveMessageIndex == 2053 || mApp.mCrazyDaveMessageIndex == 2063)
		{
			mApp.mPlayerInfo.mIsDaveTalkingZenTutorial = false;
			ShowTutorialArrowOnWateringCan();
		}
		if (!mApp.AdvanceCrazyDaveText())
		{
			mApp.CrazyDaveLeave();
			return;
		}
		if (mApp.mCrazyDaveMessageIndex != 2054)
		{
			mApp.mPlayerInfo.mZenTutorialMessage = mApp.mCrazyDaveMessageIndex;
		}
		if ((mApp.mCrazyDaveMessageIndex == 2052 || mApp.mCrazyDaveMessageIndex == 2062) && mApp.mPlayerInfo.mNumPottedPlants == 0)
		{
			for (int i = 0; i < 2; i++)
			{
				PottedPlant pottedPlant = new PottedPlant();
				pottedPlant.InitializePottedPlant(SeedType.Marigold);
				pottedPlant.mDrawVariation = (DrawVariation)TodCommon.RandRangeInt(2, 12);
				pottedPlant.mFeedingsPerGrow = 3;
				mApp.mZenGarden.AddPottedPlant(pottedPlant);
			}
		}
	}

	public void LeaveGarden()
	{
		int index = -1;
		GridItem theGridItem = null;
		while (mBoard.IterateGridItems(ref theGridItem, ref index))
		{
			if (theGridItem.mGridItemType == GridItemType.ZenTool)
			{
				DoFeedingTool((int)theGridItem.mPosX, (int)theGridItem.mPosY, theGridItem.mGridItemState);
				theGridItem.GridItemDie();
			}
			if (theGridItem.mGridItemType == GridItemType.Stinky)
			{
				mApp.mPlayerInfo.mStinkyPosX = (int)theGridItem.mPosX;
				mApp.mPlayerInfo.mStinkyPosY = (int)theGridItem.mPosY;
				theGridItem.GridItemDie();
			}
		}
		Coin theCoin = null;
		while (mBoard.IterateCoins(ref theCoin))
		{
			if (theCoin.mIsBeingCollected)
			{
				theCoin.ScoreCoin();
			}
			else
			{
				theCoin.Die();
			}
		}
	}

	public bool CanDropChocolate()
	{
		int num = 999;
		if (HasPurchasedStinky())
		{
			return mApp.mPlayerInfo.mPurchases[26] - 1000 < num;
		}
		return false;
	}

	public void FeedChocolateToPlant(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		pottedPlant.mLastChocolateTime = aNow;
		thePlant.mLaunchCounter = 200;
		mApp.AddTodParticle((float)thePlant.mX + 40f, (float)thePlant.mY + 40f, thePlant.mRenderOrder + 1, ParticleEffect.PresentPickup);
	}

	public bool PlantHighOnChocolate(PottedPlant thePottedPlant)
	{
		return (aNow - thePottedPlant.mLastChocolateTime).TotalSeconds < 300.0;
	}

	public bool PlantCanHaveChocolate(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		if (pottedPlant.mPlantAge == PottedPlantAge.Full && WasPlantNeedFulfilledToday(pottedPlant))
		{
			return !PlantHighOnChocolate(pottedPlant);
		}
		return false;
	}

	public void SetPlantAnimSpeed(Plant thePlant)
	{
		Reanimation reanimation = mApp.ReanimationGet(thePlant.mBodyReanimID);
		PottedPlant thePottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		bool flag = PlantHighOnChocolate(thePottedPlant);
		bool flag2 = reanimation.mDefinition.mFPS == 30f;
		bool flag3 = (reanimation.mAnimRate >= 25f && !flag2) || reanimation.mAnimRate >= 45f;
		if (flag3 != flag)
		{
			float num;
			if (thePlant.mSeedType == SeedType.Peashooter || thePlant.mSeedType == SeedType.Snowpea || thePlant.mSeedType == SeedType.Repeater || thePlant.mSeedType == SeedType.Leftpeater || thePlant.mSeedType == SeedType.Gatlingpea || thePlant.mSeedType == SeedType.Splitpea || thePlant.mSeedType == SeedType.Threepeater || thePlant.mSeedType == SeedType.Marigold)
			{
				num = TodCommon.RandRangeFloat(15f, 20f);
			}
			else
			{
				num = ((thePlant.mSeedType != SeedType.Potatomine) ? TodCommon.RandRangeFloat(10f, 15f) : 12f);
			}
			if (flag2)
			{
				num = 30f;
			}
			if (flag)
			{
				num *= 2f;
				num = Math.Max(25f, num);
			}
			reanimation.mAnimRate = num;
			Reanimation reanimation2 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID);
			Reanimation reanimation3 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID2);
			Reanimation reanimation4 = mApp.ReanimationTryToGet(thePlant.mHeadReanimID3);
			if (reanimation2 != null)
			{
				reanimation2.mAnimRate = reanimation.mAnimRate;
				reanimation2.mAnimTime = reanimation.mAnimTime;
			}
			if (reanimation3 != null)
			{
				reanimation3.mAnimRate = reanimation.mAnimRate;
				reanimation3.mAnimTime = reanimation.mAnimTime;
			}
			if (reanimation4 != null)
			{
				reanimation4.mAnimRate = reanimation.mAnimRate;
				reanimation4.mAnimTime = reanimation.mAnimTime;
			}
		}
	}

	public void UpdateStinkyMotionTrail(GridItem theStinky, bool theStinkyHighOnChocolate)
	{
		Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
		if (!theStinkyHighOnChocolate)
		{
			theStinky.mMotionTrailCount = 0;
			return;
		}
		if (theStinky.mGridItemState != GridItemState.StinkyWalkingRight && theStinky.mGridItemState != GridItemState.StinkyWalkingLeft)
		{
			theStinky.mMotionTrailCount = 0;
			return;
		}
		if (theStinky.mMotionTrailCount == 12)
		{
			theStinky.mMotionTrailCount--;
		}
		if (theStinky.mMotionTrailCount > 0)
		{
			Array.Copy(theStinky.mMotionTrailFrames, 1, theStinky.mMotionTrailFrames, 0, theStinky.mMotionTrailCount);
		}
		theStinky.mMotionTrailFrames[0].mPosX = theStinky.mPosX;
		theStinky.mMotionTrailFrames[0].mPosY = theStinky.mPosY;
		theStinky.mMotionTrailFrames[0].mAnimTime = reanimation.mAnimTime;
		theStinky.mMotionTrailCount++;
	}

	public void ResetPlantTimers(PottedPlant thePottedPlant)
	{
		thePottedPlant.mLastWateredTime = default;
		thePottedPlant.mLastNeedFulfilledTime = default;
		thePottedPlant.mLastFertilizedTime = default;
		thePottedPlant.mLastChocolateTime = default;
	}

	public void ResetStinkyTimers()
	{
		mApp.mPlayerInfo.mPurchases[20] = 2;
		mApp.mPlayerInfo.mLastStinkyChocolateTime = DateTime.MinValue;
	}

	public void UpdatePlantNeeds()
	{
		aNow = DateTime.UtcNow;
		if (mApp.mPlayerInfo != null)
		{
			for (int i = 0; i < mApp.mPlayerInfo.mNumPottedPlants; i++)
			{
				PottedPlant thePottedPlant = PottedPlantFromIndex(i);
				RefreshPlantNeeds(thePottedPlant);
			}
		}
	}

	public void RefreshPlantNeeds(PottedPlant thePottedPlant)
	{
		if (thePottedPlant.mPlantAge == PottedPlantAge.Full && PlantShouldRefreshNeed(thePottedPlant))
		{
			if (Plant.IsAquatic(thePottedPlant.mSeedType))
			{
				thePottedPlant.mLastWateredTime = aNow;
				thePottedPlant.mPlantNeed = (PottedPlantNeed)TodCommon.RandRangeInt(3, 4);
			}
			else
			{
				thePottedPlant.mTimesFed = 0;
				thePottedPlant.mPlantNeed = PottedPlantNeed.None;
			}
		}
	}

	public void PlantSetLaunchCounter(Plant thePlant)
	{
		int theTimeAge = PlantGetMinutesSinceHappy(thePlant);
		int theMax = TodCommon.TodAnimateCurve(5, 30, theTimeAge, 3000, 15000, TodCurves.Linear);
		thePlant.mLaunchCounter = TodCommon.RandRangeInt(1800, theMax);
	}

	public int PlantGetMinutesSinceHappy(Plant thePlant)
	{
		PottedPlant pottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		int result = (int)((aNow - pottedPlant.mLastNeedFulfilledTime).TotalSeconds / 60.0);
		if (PlantHighOnChocolate(pottedPlant))
		{
			result = 0;
		}
		return result;
	}

	public bool IsStinkyHighOnChocolate()
	{
		return (aNow - mApp.mPlayerInfo.mLastStinkyChocolateTime).TotalSeconds < 3600.0;
	}

	public void StinkyAnimRateUpdate(GridItem theStinky)
	{
		Reanimation reanimation = mApp.ReanimationGet(theStinky.mGridItemReanimID);
		if (IsStinkyHighOnChocolate())
		{
			if (theStinky.mGridItemState == GridItemState.StinkyWalkingLeft || theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
			{
				reanimation.mAnimRate = 12f;
			}
			else if (theStinky.mGridItemState == GridItemState.StinkyTurningRight || theStinky.mGridItemState == GridItemState.StinkyTurningLeft)
			{
				reanimation.mAnimRate = 12f;
			}
		}
		else if (theStinky.mGridItemState == GridItemState.StinkyWalkingLeft || theStinky.mGridItemState == GridItemState.StinkyWalkingRight)
		{
			reanimation.mAnimRate = 6f;
		}
		else if (theStinky.mGridItemState == GridItemState.StinkyTurningRight || theStinky.mGridItemState == GridItemState.StinkyTurningLeft)
		{
			reanimation.mAnimRate = 6f;
		}
	}

	public bool PlantCanBeWatered(Plant thePlant)
	{
		if (thePlant.mPottedPlantIndex == -1)
		{
			return false;
		}
		PottedPlant thePottedPlant = PottedPlantFromIndex(thePlant.mPottedPlantIndex);
		PottedPlantNeed plantsNeed = mApp.mZenGarden.GetPlantsNeed(thePottedPlant);
		return plantsNeed == PottedPlantNeed.Water;
	}

	public void MakeStinkySleeping()
	{
		if (HasPurchasedStinky() && mApp.mPlayerInfo.mHasSeenStinky)
		{
			ResetStinkyTimers();
		}
	}

	public float ZenPlantOffsetX(PottedPlant thePottedPlant)
	{
		int num = 0;
		if (thePottedPlant.mFacing == PottedPlant.FacingDirection.Left && thePottedPlant.mSeedType == SeedType.Potatomine)
		{
			num -= 6;
		}
		return num;
	}

	public void DoPlantSale(bool wasSold)
	{
		mApp.CrazyDaveLeave();
		if (!wasSold)
		{
			return;
		}
		int plantSellPrice = GetPlantSellPrice(mPlantForSale);
		PottedPlantFromIndex(mPlantForSale.mPottedPlantIndex);
		mApp.mPlayerInfo.AddCoins(plantSellPrice);
		mBoard.mCoinsCollected += plantSellPrice;
		int num = mApp.mPlayerInfo.mNumPottedPlants - mPlantForSale.mPottedPlantIndex - 1;
		if (num > 0)
		{
			for (int i = mPlantForSale.mPottedPlantIndex; i < mApp.mPlayerInfo.mPottedPlant.Length; i++)
			{
				if (i != mApp.mPlayerInfo.mPottedPlant.Length - 1)
				{
					mApp.mPlayerInfo.mPottedPlant[i] = mApp.mPlayerInfo.mPottedPlant[i + 1];
				}
			}
			int count = mBoard.mPlants.Count;
			for (int j = 0; j < count; j++)
			{
				Plant plant = mBoard.mPlants[j];
				if (!plant.mDead && plant.mPottedPlantIndex > mPlantForSale.mPottedPlantIndex)
				{
					plant.mPottedPlantIndex--;
				}
			}
		}
		mApp.mPlayerInfo.mNumPottedPlants--;
		mApp.PlayFoley(FoleyType.UseShovel);
		RemovePottedPlant(mPlantForSale);
	}

	public void BackFromStore()
	{
		StoreScreen storeScreen = (StoreScreen)mApp.GetDialog(4);
		bool mGoToTreeNow = storeScreen.mGoToTreeNow;
		mApp.KillDialog(Dialogs.DIALOG_STORE);
		if (mGoToTreeNow)
		{
			mApp.KillBoard();
			mApp.PreNewGame(GameMode.TreeOfWisdom, theLookForSavedGame: false);
			return;
		}
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ZenGarden);
		if (mBoard.mTutorialState == TutorialState.ZenGardenVisitStore)
		{
			mBoard.DisplayAdvice("[ADVICE_ZEN_GARDEN_FERTILIZE]", MessageStyle.ZenGardenLong, AdviceType.None);
			mBoard.mTutorialState = TutorialState.ZenGardenFertilizePlants;
			mApp.mPlayerInfo.mZenTutorialMessage = 26;
		}
		AddStinky();
	}

	public void ButtonPress(int theId)
	{
	}

	public void ButtonPress(int theId, int theClickCount)
	{
	}

	public void ButtonDepress(int theId)
	{
		switch (theId)
		{
		case 101:
			if (mInteracting != null && mInteracting.DoTouch(1))
			{
				ShowInteractingButtons(doShow: false);
				mIsInteractingInTouch = true;
				mInteracting.mRenderOrder += 100000;
				mInteracting.mState = PlantState.ZenGardenInteracting;
			}
			break;
		case 102:
			if (mInteracting != null && mInteracting.DoTouch(2))
			{
				ShowInteractingButtons(doShow: false);
				mIsInteractingInTouch = true;
				mInteracting.mRenderOrder += 100000;
				mInteracting.mState = PlantState.ZenGardenInteracting;
				string theString = "[" + Plant.GetPlantDefinition(mInteracting.mSeedType).mPlantName + "_VOICETEXT_GARDEN2]";
				TodStringFile.TodStringListExists(theString);
			}
			break;
		case 103:
			if (mInteracting != null && mApp.mPlayerInfo.mPurchases[26] > 0 && PlantCanHaveChocolate(mInteracting))
			{
				mApp.mPlayerInfo.mPurchases[26]--;
				FeedChocolateToPlant(mInteracting);
				mApp.PlayFoley(FoleyType.Wakeup);
			}
			else
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
			}
			break;
		case 105:
			if (mBoard.mChallenge.mChallengeState == ChallengeState.ZenPlantInteracting)
			{
				mApp.mZenGarden.LeavePlantInteraction(mApp.mZenGarden.mInteracting);
			}
			break;
		case 104:
			break;
		}
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseEnter(int theId)
	{
	}

	public void ButtonMouseLeave(int theId)
	{
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public static void RefreshMushroomGridPlacements()
	{
		for (int i = 0; i < Constants.gMushroomGridPlacement.Length; i++)
		{
			Constants.gMushroomGridPlacement[i].mPixelX = Constants.gMushroomGridPlacement[i].mPixelX + Constants.ZenGardenMushroomGardenOffset.X;
			Constants.gMushroomGridPlacement[i].mPixelY = Constants.gMushroomGridPlacement[i].mPixelY + Constants.ZenGardenMushroomGardenOffset.Y;
		}
	}
}
