using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class StoreScreen : Dialog
{
	private const int aNumpicks = 4;

	public LawnApp mApp;

	public NewLawnButton mBackButton;

	public NewLawnButton mPrevButton;

	public NewLawnButton mNextButton;

	public Widget mOverlayWidget;

	public int mStoreTime;

	public string mBubbleText = string.Empty;

	public int mBubbleCountDown;

	public bool mBubbleClickToContinue;

	public bool mBubbleAutoAdvance;

	public int mAmbientSpeechCountDown;

	public int mPreviousAmbientSpeechIndex;

	public StorePage mPage;

	public StoreItem mMouseOverItem;

	public int mHatchTimer;

	public bool mHatchOpen;

	public int mShakeX;

	public int mShakeY;

	public int mStartDialog;

	public bool mEasyBuyingCheat;

	public PottedPlant mPottedPlantSpecs = new PottedPlant();

	public static DataArray<Coin> mCoins;

	public bool mDrawnOnce;

	public bool mGoToTreeNow;

	public bool mPurchasedFullVersion;

	public bool mTrialLockedWhenStoreOpened;

	public StoreListener mListener;

	public StoreItem mPendingPurchaseItem;

	public int mHalfDeltaWidth;

	public int mHalfDeltaHeight;

	private static TodWeightedArray[] aPickArray;

	static StoreScreen()
	{
		mCoins = new DataArray<Coin>();
		aPickArray = new TodWeightedArray[4];
		for (int i = 0; i < aPickArray.Length; i++)
		{
			aPickArray[i] = TodWeightedArray.GetNewTodWeightedArray();
		}
	}

	public StoreScreen(LawnApp theApp, StoreListener theListener)
		: base(null, null, 4, isModal: true, "[STORE]", "", "", 0)
	{
		mListener = theListener;
		mApp = theApp;
		mClip = false;
		mStoreTime = 0;
		mBubbleCountDown = 0;
		mBubbleClickToContinue = false;
		mBubbleAutoAdvance = false;
		mAmbientSpeechCountDown = 200;
		mPreviousAmbientSpeechIndex = -1;
		mPage = StorePage.SlotUpgrades;
		mMouseOverItem = StoreItem.STORE_ITEM_INVALID;
		mHatchTimer = 0;
		mShakeX = 0;
		mShakeY = 0;
		mStartDialog = -1;
		mHatchOpen = true;
		mEasyBuyingCheat = false;
		mCoins.DataArrayInitialize(1024u, "coins");
		mApp.DelayLoadStoreResource("DelayLoad_Store");
		Resize(0, 0, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
		mPottedPlantSpecs.InitializePottedPlant(SeedType.Marigold);
		mPottedPlantSpecs.mDrawVariation = (DrawVariation)TodCommon.RandRangeInt(2, 12);
		mBackButton = new NewLawnButton(null, 100, this);
		mBackButton.mDoFinger = true;
		mBackButton.mLabel = "";
		mBackButton.mButtonImage = AtlasResources.IMAGE_STORE_MAINMENUBUTTON;
		mBackButton.mDownImage = AtlasResources.IMAGE_STORE_MAINMENUBUTTONDOWN;
		mBackButton.mColors[0] = new SexyColor(98, 153, 235);
		mBackButton.mColors[1] = new SexyColor(167, 192, 235);
		mBackButton.Resize(Constants.StoreScreen_BackButton_X, Constants.StoreScreen_BackButton_Y, AtlasResources.IMAGE_STORE_MAINMENUBUTTON.mWidth, AtlasResources.IMAGE_STORE_MAINMENUBUTTON.mHeight);
		mBackButton.mTextOffsetX = -7;
		mBackButton.mTextOffsetY = 1;
		mBackButton.mTextDownOffsetX = 2;
		mBackButton.mTextDownOffsetY = 1;
		mPrevButton = new NewLawnButton(null, 101, this);
		mPrevButton.mDoFinger = true;
		mPrevButton.mLabel = "";
		mPrevButton.mButtonImage = AtlasResources.IMAGE_STORE_PREVBUTTON;
		mPrevButton.mOverImage = AtlasResources.IMAGE_STORE_PREVBUTTONHIGHLIGHT;
		mPrevButton.mDownImage = AtlasResources.IMAGE_STORE_PREVBUTTONHIGHLIGHT;
		mPrevButton.mColors[0] = new SexyColor(255, 240, 0);
		mPrevButton.mColors[1] = new SexyColor(200, 200, 255);
		mPrevButton.Resize(Constants.StoreScreen_PrevButton_X, Constants.StoreScreen_PrevButton_Y, AtlasResources.IMAGE_STORE_PREVBUTTON.mWidth, AtlasResources.IMAGE_STORE_PREVBUTTON.mHeight);
		mNextButton = new NewLawnButton(null, 102, this);
		mNextButton.mDoFinger = true;
		mNextButton.mLabel = "";
		mNextButton.mButtonImage = AtlasResources.IMAGE_STORE_NEXTBUTTON;
		mNextButton.mOverImage = AtlasResources.IMAGE_STORE_NEXTBUTTONHIGHLIGHT;
		mNextButton.mDownImage = AtlasResources.IMAGE_STORE_NEXTBUTTONHIGHLIGHT;
		mNextButton.mColors[0] = new SexyColor(255, 240, 0);
		mNextButton.mColors[1] = new SexyColor(200, 200, 255);
		mNextButton.Resize(Constants.StoreScreen_NextButton_X, Constants.StoreScreen_NextButton_Y, AtlasResources.IMAGE_STORE_NEXTBUTTON.mWidth, AtlasResources.IMAGE_STORE_NEXTBUTTON.mHeight);
		mOverlayWidget = new StoreScreenOverlay(this);
		mOverlayWidget.Resize(0, 0, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
		if (!IsPageShown(StorePage.PlantUpgrades))
		{
			mPrevButton.mDisabledImage = AtlasResources.IMAGE_STORE_PREVBUTTONDISABLED;
			mPrevButton.SetDisabled(isDisabled: true);
			mNextButton.mDisabledImage = AtlasResources.IMAGE_STORE_NEXTBUTTONDISABLED;
			mNextButton.SetDisabled(isDisabled: true);
		}
		mDrawnOnce = false;
		mGoToTreeNow = false;
		mPurchasedFullVersion = false;
		mTrialLockedWhenStoreOpened = mApp.IsTrialStageLocked();
	}

	public override void Dispose()
	{
		mCoins.DataArrayDispose();
		mBackButton.Dispose();
		mPrevButton.Dispose();
		mNextButton.Dispose();
		mOverlayWidget.Dispose();
		mApp.DelayLoadStoreResource(string.Empty);
	}

	public override void KeyDown(KeyCode theKey)
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

	public override void Update()
	{
		UpdateScreen();
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.TitleCrazyDaveMainTheme);
		if (mApp.mGameMode != GameMode.ChallengeZenGarden || mApp.mBoard == null)
		{
			mApp.UpdateCrazyDave();
		}
		if (IsWaitingForDialog())
		{
			return;
		}
		if (mApp.mCrazyDaveState == CrazyDaveState.Off)
		{
			if (mDrawnOnce)
			{
				StorePreLoad();
			}
			return;
		}
		mStoreTime += 3;
		if (mApp.mCrazyDaveState != CrazyDaveState.Off && mApp.mCrazyDaveState != CrazyDaveState.Entering)
		{
			if (mHatchTimer > 0)
			{
				mHatchTimer -= 3;
				mBackButton.mX -= mShakeX;
				mBackButton.mY -= mShakeY;
				mPrevButton.mX -= mShakeX;
				mPrevButton.mY -= mShakeY;
				mNextButton.mX -= mShakeX;
				mNextButton.mY -= mShakeY;
				if (mHatchTimer >= 0 && mHatchTimer < 3)
				{
					if (!mBubbleClickToContinue)
					{
						EnableButtons(theEnable: true);
					}
					mShakeX = 0;
					mShakeY = 0;
				}
				else if (mHatchTimer > 35)
				{
					mShakeX = 0;
					mShakeY = RandomNumbers.NextNumber(3) - 1;
				}
				else
				{
					mShakeX = 0;
					mShakeY = 0;
				}
				mBackButton.mX += mShakeX;
				mBackButton.mY += mShakeY;
				mPrevButton.mX += mShakeX;
				mPrevButton.mY += mShakeY;
				mNextButton.mX += mShakeX;
				mNextButton.mY += mShakeY;
			}
			else if (mStartDialog != -1)
			{
				SetBubbleText(mStartDialog, 0, theClickToContinue: true);
				mStartDialog = -1;
			}
			else if (!mBubbleClickToContinue)
			{
				if (mBubbleCountDown > 0)
				{
					mBubbleCountDown -= 3;
					if (mBubbleCountDown >= 0 && mBubbleCountDown < 3)
					{
						if (mApp.mSoundSystem.IsFoleyPlaying(FoleyType.Crazydaveshort) || mApp.mSoundSystem.IsFoleyPlaying(FoleyType.Crazydavelong) || mApp.mSoundSystem.IsFoleyPlaying(FoleyType.Crazydaveextralong))
						{
							mBubbleCountDown = 3;
						}
						else if (mBubbleAutoAdvance)
						{
							AdvanceCrazyDaveDialog();
						}
						else
						{
							mApp.CrazyDaveStopTalking();
						}
					}
				}
				else
				{
					mAmbientSpeechCountDown -= 3;
					if (mAmbientSpeechCountDown <= 0)
					{
						for (int i = 0; i < 4; i++)
						{
							aPickArray[i].Reset();
						}
						for (int j = 0; j < 4; j++)
						{
							aPickArray[j].mItem = 2015 + j;
							if (mPreviousAmbientSpeechIndex == (int)aPickArray[j].mItem)
							{
								aPickArray[j].mWeight = 0;
							}
							else if (j == 3)
							{
								if (!mApp.HasFinishedAdventure())
								{
									aPickArray[j].mWeight = 0;
								}
								else
								{
									aPickArray[j].mWeight = 20;
								}
							}
							else
							{
								aPickArray[j].mWeight = 100;
							}
						}
						SetBubbleText(mPreviousAmbientSpeechIndex = (int)TodCommon.TodPickFromWeightedArray(aPickArray, 4), 800, theClickToContinue: false);
						mAmbientSpeechCountDown = TodCommon.RandRangeInt(500, 1000);
					}
				}
			}
		}
		UpdateMouse();
		if (CanInteractWithButtons() && mTrialLockedWhenStoreOpened && !mApp.IsTrialStageLocked())
		{
			mPurchasedFullVersion = true;
			mResult = 1000;
		}
		else
		{
			base.Update();
			MarkDirty();
		}
	}

	public override void Draw(Graphics g)
	{
		g.SetLinearBlend(linear: true);
		mDrawnOnce = true;
		int theY = TodCommon.TodAnimateCurve(50, 110, mStoreTime, Constants.StoreScreen_StoreSign_Y_Min, Constants.StoreScreen_StoreSign_Y_Max, TodCurves.EaseInOut);
		if (mApp.IsNight())
		{
			g.DrawImage(Resources.IMAGE_STORE_BACKGROUNDNIGHT, -160, Constants.UIOffsetY);
		}
		else
		{
			g.DrawImage(Resources.IMAGE_STORE_BACKGROUND, -160, Constants.UIOffsetY);
		}
		g.DrawImage(Resources.IMAGE_STORE_CAR, Constants.StoreScreen_Car_X + mShakeX, Constants.StoreScreen_Car_Y + mShakeY);
		if (mHatchTimer == 0 && mHatchOpen)
		{
			g.DrawImage(Resources.IMAGE_STORE_HATCHBACKOPEN, Constants.StoreScreen_HatchOpen_X + mShakeX, Constants.StoreScreen_HatchOpen_Y + mShakeY);
			if (mApp.IsNight())
			{
				g.DrawImage(Resources.IMAGE_STORE_CAR_NIGHT, Constants.StoreScreen_CarNight_X + mShakeX, Constants.StoreScreen_CarNight_Y + mShakeY);
			}
		}
		else
		{
			g.DrawImage(Resources.IMAGE_STORE_CARCLOSED, Constants.StoreScreen_HatchClosed_X + mShakeX, Constants.StoreScreen_HatchClosed_Y + mShakeY);
			if (mApp.IsNight())
			{
				g.DrawImage(Resources.IMAGE_STORE_CAR_NIGHT, Constants.StoreScreen_CarNight_X + mShakeX, Constants.StoreScreen_CarNight_Y + mShakeY);
				g.DrawImage(Resources.IMAGE_STORE_CARCLOSED_NIGHT, Constants.StoreScreen_HatchClosed_X + mShakeX, Constants.StoreScreen_HatchClosed_Y + mShakeY);
			}
		}
		g.DrawImage(AtlasResources.IMAGE_STORE_SIGN, Constants.StoreScreen_StoreSign_X, theY);
		Graphics graphics = Graphics.GetNew(g);
		graphics.mTransX += Constants.StoreScreen_RetardedDave_Offset_X;
		Graphics graphics2 = graphics;
		mApp.DrawCrazyDave(graphics);
		graphics.PrepareForReuse();
		if (mHatchTimer == 0 && mHatchOpen)
		{
			for (int i = 0; i < 8; i++)
			{
				StoreItem storeItemType = GetStoreItemType(i);
				if (storeItemType != StoreItem.STORE_ITEM_INVALID)
				{
					DrawItem(g, i, storeItemType);
				}
			}
		}
		int storeScreen_Coinbank_X = Constants.StoreScreen_Coinbank_X;
		int storeScreen_Coinbank_Y = Constants.StoreScreen_Coinbank_Y;
		g.DrawImage(AtlasResources.IMAGE_COINBANK, storeScreen_Coinbank_X, storeScreen_Coinbank_Y);
		g.SetColor(new SexyColor(180, 255, 90));
		g.SetFont(Resources.FONT_CONTINUUMBOLD14);
		string moneyString = LawnApp.GetMoneyString(mApp.mPlayerInfo.mCoins);
		int theX = storeScreen_Coinbank_X + Constants.StoreScreen_Coinbank_TextOffset.X - Resources.FONT_CONTINUUMBOLD14.StringWidth(moneyString);
		g.DrawString(moneyString, theX, storeScreen_Coinbank_Y + Constants.StoreScreen_Coinbank_TextOffset.Y);
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		AddWidget(mBackButton);
		AddWidget(mPrevButton);
		AddWidget(mNextButton);
		AddWidget(mOverlayWidget);
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		RemoveWidget(mBackButton);
		RemoveWidget(mPrevButton);
		RemoveWidget(mNextButton);
		RemoveWidget(mOverlayWidget);
		mApp.CrazyDaveDie();
	}

	public override void ButtonPress(int theId)
	{
		if (theId != 101 && theId != 102)
		{
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
		}
	}

	public override bool BackButtonPress()
	{
		ButtonDepress(mBackButton.mId);
		return true;
	}

	public override void ButtonDepress(int theId)
	{
		switch (theId)
		{
		case 100:
			mResult = 1000;
			if (mListener != null)
			{
				mListener.BackFromStore();
			}
			return;
		case 101:
			mHatchTimer = 51;
			mApp.PlaySample(Resources.SOUND_HATCHBACK_CLOSE);
			mBubbleCountDown = 1;
			EnableButtons(theEnable: false);
			do
			{
				mPage--;
				if (mPage < StorePage.SlotUpgrades)
				{
					mPage = StorePage.ExtraSlot;
				}
			}
			while (!IsPageShown(mPage));
			break;
		}
		if (theId != 102)
		{
			return;
		}
		mHatchTimer = 51;
		mApp.PlaySample(Resources.SOUND_HATCHBACK_CLOSE);
		mBubbleCountDown = 1;
		EnableButtons(theEnable: false);
		do
		{
			mPage++;
			if (mPage >= StorePage.NumStorePages)
			{
				mPage = StorePage.SlotUpgrades;
			}
		}
		while (!IsPageShown(mPage));
	}

	public virtual void KeyChar(char theChar)
	{
		if (mBubbleClickToContinue && (theChar == ' ' || theChar == '\r'))
		{
			AdvanceCrazyDaveDialog();
			return;
		}
		if (theChar == 'c' || theChar == 'C')
		{
			mEasyBuyingCheat = true;
			mNextButton.mMouseVisible = true;
			mNextButton.SetDisabled(isDisabled: false);
			mPrevButton.mMouseVisible = true;
			mPrevButton.SetDisabled(isDisabled: false);
		}
		if (theChar == '0')
		{
			mApp.mPlayerInfo.AddCoins(50000);
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
		}
		if (theChar == '$')
		{
			mApp.mPlayerInfo.AddCoins(100);
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
		}
		if (theChar == 'l')
		{
			mApp.mDebugTrialLocked = !mApp.mDebugTrialLocked;
			mApp.mTrialType = TrialType.Stagelocked;
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (mBubbleClickToContinue)
		{
			AdvanceCrazyDaveDialog();
			return;
		}
		UpdateMouse();
		if (!CanInteractWithButtons())
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			StoreItem storeItemType = GetStoreItemType(i);
			if (storeItemType == StoreItem.STORE_ITEM_INVALID)
			{
				continue;
			}
			int thePosX = 0;
			int thePosY = 0;
			GetStorePosition(i, ref thePosX, ref thePosY);
			if (new TRect(thePosX, thePosY, Constants.StoreScreen_MouseRegion.X, Constants.StoreScreen_MouseRegion.Y).Contains(x, y))
			{
				if (IsItemSoldOut(storeItemType) || IsItemUnavailable(storeItemType) || IsComingSoon(storeItemType))
				{
					break;
				}
				PurchaseItem(storeItemType);
			}
		}
	}

	public void SetBubbleText(int theCrazyDaveMessage, int theTime, bool theClickToContinue)
	{
		mApp.CrazyDaveTalkIndex(theCrazyDaveMessage);
		mBubbleCountDown = theTime;
		mBubbleClickToContinue = theClickToContinue;
	}

	public void GetStorePosition(int theSpotIndex, ref int thePosX, ref int thePosY)
	{
		if (theSpotIndex <= 3)
		{
			thePosX = Constants.StoreScreen_ItemOffset_1_X + Constants.StoreScreen_ItemSize * theSpotIndex;
			thePosY = Constants.StoreScreen_ItemOffset_1_Y;
		}
		else
		{
			thePosX = Constants.StoreScreen_ItemOffset_2_X + Constants.StoreScreen_ItemSize * (theSpotIndex - Constants.StoreScreen_ItemSize_Offset);
			thePosY = Constants.StoreScreen_ItemOffset_2_Y;
		}
	}

	public StoreItem GetStoreItemType(int theSpotIndex)
	{
		if (mPage == StorePage.SlotUpgrades)
		{
			switch (theSpotIndex)
			{
			case 0:
				return StoreItem.STORE_ITEM_PACKET_UPGRADE;
			case 1:
				return StoreItem.STORE_ITEM_POOL_CLEANER;
			case 2:
				return StoreItem.STORE_ITEM_RAKE;
			case 3:
				return StoreItem.STORE_ITEM_ROOF_CLEANER;
			case 4:
				return StoreItem.STORE_ITEM_PLANT_GATLINGPEA;
			case 5:
				return StoreItem.STORE_ITEM_PLANT_TWINSUNFLOWER;
			case 6:
				return StoreItem.STORE_ITEM_PLANT_GLOOMSHROOM;
			case 7:
				if (mApp.mPlayerInfo.mPurchases[3] >= 1)
				{
					if (mApp.mPlayerInfo.mPurchases[38] > 0 && mApp.mPlayerInfo.mPurchases[39] == 0)
					{
						return StoreItem.STORE_ITEM_CATTAIL_DRIVER;
					}
					return StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO;
				}
				return StoreItem.STORE_ITEM_PLANT_CATTAIL;
			default:
				return StoreItem.STORE_ITEM_INVALID;
			}
		}
		if (mPage == StorePage.PlantUpgrades)
		{
			return theSpotIndex switch
			{
				0 => StoreItem.STORE_ITEM_PLANT_SPIKEROCK, 
				1 => StoreItem.STORE_ITEM_PLANT_GOLD_MAGNET, 
				2 => StoreItem.STORE_ITEM_PLANT_WINTERMELON, 
				3 => StoreItem.STORE_ITEM_PLANT_COBCANNON, 
				4 => StoreItem.STORE_ITEM_PLANT_IMITATER, 
				5 => StoreItem.STORE_ITEM_FIRSTAID, 
				6 => StoreItem.STORE_ITEM_GARLICFIRSTAID, 
				7 => StoreItem.STORE_ITEM_FUMESHROOM_GNOME, 
				_ => StoreItem.STORE_ITEM_INVALID, 
			};
		}
		if (mPage == StorePage.Zen1)
		{
			return theSpotIndex switch
			{
				0 => StoreItem.STORE_ITEM_POTTED_MARIGOLD_1, 
				1 => StoreItem.STORE_ITEM_POTTED_MARIGOLD_2, 
				2 => StoreItem.STORE_ITEM_POTTED_MARIGOLD_3, 
				3 => StoreItem.STORE_ITEM_GOLD_WATERINGCAN, 
				4 => StoreItem.STORE_ITEM_FERTILIZER, 
				5 => StoreItem.STORE_ITEM_BUG_SPRAY, 
				6 => StoreItem.STORE_ITEM_PHONOGRAPH, 
				7 => StoreItem.STORE_ITEM_GARDENING_GLOVE, 
				_ => StoreItem.STORE_ITEM_INVALID, 
			};
		}
		if (mPage == StorePage.Zen2)
		{
			return theSpotIndex switch
			{
				0 => StoreItem.STORE_ITEM_MUSHROOM_GARDEN, 
				1 => StoreItem.STORE_ITEM_AQUARIUM_GARDEN, 
				2 => StoreItem.STORE_ITEM_WHEEL_BARROW, 
				3 => StoreItem.STORE_ITEM_STINKY_THE_SNAIL, 
				4 => StoreItem.STORE_ITEM_TREE_OF_WISDOM, 
				5 => StoreItem.STORE_ITEM_TREE_FOOD, 
				6 => StoreItem.STORE_ITEM_GREENHOUSE_NIGHT, 
				_ => StoreItem.STORE_ITEM_INVALID, 
			};
		}
		if (mPage == StorePage.ExtraSlot)
		{
			return theSpotIndex switch
			{
				0 => StoreItem.STORE_ITEM_PLANT_SUPER_CHOMPER, 
				1 => StoreItem.STORE_ITEM_PLANT_PICKLED_PEPPER, 
				2 => StoreItem.STORE_ITEM_PLANT_FIRE_SHROOM, 
				3 => StoreItem.STORE_ITEM_PLANT_AGAVE, 
				4 => StoreItem.STORE_ITEM_AGAVE_SKILL, 
				5 => StoreItem.STORE_ITEM_CARD_GROUP_UPGRADE, 
				6 => LawnApp.PortPlantsEnabled ? StoreItem.STORE_ITEM_PLANT_IRIS_SHOOTER : StoreItem.STORE_ITEM_INVALID, 
				_ => StoreItem.STORE_ITEM_INVALID, 
			};
		}
		Debug.ASSERT(value: false);
		return StoreItem.STORE_ITEM_INVALID;
	}

	public void UpdateMouse()
	{
	}

	public bool IsItemUnavailable(StoreItem theStoreItem)
	{
		if (mEasyBuyingCheat)
		{
			return false;
		}
		if (theStoreItem == StoreItem.STORE_ITEM_ROOF_CLEANER)
		{
			if (mApp.IsTrialStageLocked())
			{
				return true;
			}
			if (!mApp.HasFinishedAdventure() && mApp.mPlayerInfo.mLevel < 42)
			{
				return true;
			}
		}
		if (theStoreItem == StoreItem.STORE_ITEM_PLANT_GLOOMSHROOM)
		{
			if (mApp.IsTrialStageLocked())
			{
				return true;
			}
			if (!mApp.HasFinishedAdventure() && mApp.mPlayerInfo.mLevel < 35)
			{
				return true;
			}
		}
		if (theStoreItem == StoreItem.STORE_ITEM_PLANT_CATTAIL)
		{
			if (mApp.IsTrialStageLocked())
			{
				return true;
			}
			if (!mApp.HasFinishedAdventure() && mApp.mPlayerInfo.mLevel < 35)
			{
				return true;
			}
		}
		if ((theStoreItem != StoreItem.STORE_ITEM_PLANT_SPIKEROCK || mApp.HasFinishedAdventure() || mApp.mPlayerInfo.mLevel >= 41) && (theStoreItem != StoreItem.STORE_ITEM_PLANT_GOLD_MAGNET || mApp.HasFinishedAdventure() || mApp.mPlayerInfo.mLevel >= 41))
		{
			if (theStoreItem == StoreItem.STORE_ITEM_PLANT_WINTERMELON || theStoreItem == StoreItem.STORE_ITEM_PLANT_COBCANNON || theStoreItem == StoreItem.STORE_ITEM_PLANT_IMITATER || theStoreItem == StoreItem.STORE_ITEM_FIRSTAID)
			{
				return !mApp.HasFinishedAdventure();
			}
			return false;
		}
		return true;
	}

	public bool IsItemSoldOut(StoreItem theStoreItem)
	{
		switch (theStoreItem)
		{
		case StoreItem.STORE_ITEM_INVALID:
			return false;
		case StoreItem.STORE_ITEM_PACKET_UPGRADE:
			return mApp.mPlayerInfo.mPurchases[21] >= 4;
		case StoreItem.STORE_ITEM_CARD_GROUP_UPGRADE:
			return mApp.mPlayerInfo.mPurchases[(int)theStoreItem] >= 4;
		case StoreItem.STORE_ITEM_GOLD_WATERINGCAN:
			return mApp.mPlayerInfo.mPurchases[(int)theStoreItem] >= 2;
		case StoreItem.STORE_ITEM_FERTILIZER:
		case StoreItem.STORE_ITEM_BUG_SPRAY:
			return mApp.mPlayerInfo.mPurchases[(int)theStoreItem] - 1000 > 15;
		case StoreItem.STORE_ITEM_BONUS_LAWN_MOWER:
			return mApp.mPlayerInfo.mPurchases[9] >= 2;
		case StoreItem.STORE_ITEM_TREE_FOOD:
			return mApp.mPlayerInfo.mPurchases[(int)theStoreItem] - 1000 >= 10;
		default:
			if (IsPottedPlant(theStoreItem))
			{
				int currentDaysSince = LawnCommon.GetCurrentDaysSince2000();
				if (!mApp.mZenGarden.IsZenGardenFull(theIncludeDroppedPresents: true))
				{
					return mApp.mPlayerInfo.mPurchases[(int)theStoreItem] == currentDaysSince;
				}
				return true;
			}
			if (theStoreItem == StoreItem.STORE_ITEM_CATTAIL_DRIVER || theStoreItem == StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO)
			{
				return false;
			}
			Debug.ASSERT(theStoreItem >= StoreItem.STORE_ITEM_PLANT_GATLINGPEA && theStoreItem < (StoreItem)80);
			return mApp.mPlayerInfo.mPurchases[(int)theStoreItem] != 0;
		}
	}

	public static int GetItemCost(StoreItem theStoreItem)
	{
		LawnApp lawnApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		switch (theStoreItem)
		{
		case StoreItem.STORE_ITEM_PLANT_GATLINGPEA:
			return 500;
		case StoreItem.STORE_ITEM_PLANT_TWINSUNFLOWER:
			return 500;
		case StoreItem.STORE_ITEM_PLANT_GLOOMSHROOM:
			return 750;
		case StoreItem.STORE_ITEM_PLANT_CATTAIL:
			return 1000;
		case StoreItem.STORE_ITEM_PLANT_WINTERMELON:
			return 1000;
		case StoreItem.STORE_ITEM_PLANT_GOLD_MAGNET:
			return 300;
		case StoreItem.STORE_ITEM_PLANT_SPIKEROCK:
			return 750;
		case StoreItem.STORE_ITEM_PLANT_COBCANNON:
			return 2000;
		case StoreItem.STORE_ITEM_PLANT_IMITATER:
			return 3000;
		case StoreItem.STORE_ITEM_POTTED_MARIGOLD_1:
			return 250;
		case StoreItem.STORE_ITEM_POTTED_MARIGOLD_2:
			return 250;
		case StoreItem.STORE_ITEM_POTTED_MARIGOLD_3:
			return 250;
		case StoreItem.STORE_ITEM_GOLD_WATERINGCAN:
			if (lawnApp.mPlayerInfo.mPurchases[13] + 1 == 1)
			{
				return 1000;
			}
			return 5000;
		case StoreItem.STORE_ITEM_FERTILIZER:
			return 75;
		case StoreItem.STORE_ITEM_BUG_SPRAY:
			return 100;
		case StoreItem.STORE_ITEM_PHONOGRAPH:
			return 1500;
		case StoreItem.STORE_ITEM_GARDENING_GLOVE:
			return 100;
		case StoreItem.STORE_ITEM_MUSHROOM_GARDEN:
			return 3000;
		case StoreItem.STORE_ITEM_WHEEL_BARROW:
			return 20;
		case StoreItem.STORE_ITEM_STINKY_THE_SNAIL:
			return 300;
		case StoreItem.STORE_ITEM_PACKET_UPGRADE:
			if (lawnApp.mPlayerInfo.mPurchases[21] + 1 == 1)
			{
				return 75;
			}
			if (lawnApp.mPlayerInfo.mPurchases[21] + 1 == 2)
			{
				return 500;
			}
			if (lawnApp.mPlayerInfo.mPurchases[21] + 1 == 3)
			{
				return 2000;
			}
			return 8000;
		case StoreItem.STORE_ITEM_POOL_CLEANER:
			return 100;
		case StoreItem.STORE_ITEM_ROOF_CLEANER:
			return 300;
		case StoreItem.STORE_ITEM_RAKE:
			return 70;
		case StoreItem.STORE_ITEM_AQUARIUM_GARDEN:
			return 3000;
		case StoreItem.STORE_ITEM_FIRSTAID:
			return 200;
		case StoreItem.STORE_ITEM_GARLICFIRSTAID:
			return 400;
		case StoreItem.STORE_ITEM_TREE_OF_WISDOM:
			return 1000;
		case StoreItem.STORE_ITEM_TREE_FOOD:
			return 250;
		case StoreItem.STORE_ITEM_PLANT_SUPER_CHOMPER:
		case StoreItem.STORE_ITEM_PLANT_PICKLED_PEPPER:
		case StoreItem.STORE_ITEM_PLANT_FIRE_SHROOM:
		case StoreItem.STORE_ITEM_PLANT_AGAVE:
		case StoreItem.STORE_ITEM_AGAVE_SKILL:
		case StoreItem.STORE_ITEM_PLANT_IRIS_SHOOTER:
			return 10000;
		case StoreItem.STORE_ITEM_CARD_GROUP_UPGRADE:
			if (lawnApp.mPlayerInfo.mPurchases[36] + 1 == 1)
			{
				return 1000;
			}
			if (lawnApp.mPlayerInfo.mPurchases[36] + 1 == 2)
			{
				return 5000;
			}
			if (lawnApp.mPlayerInfo.mPurchases[36] + 1 == 3)
			{
				return 10000;
			}
			return 15000;
		case StoreItem.STORE_ITEM_GREENHOUSE_NIGHT:
			return 15000;
		case StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO:
			if (lawnApp.mPlayerInfo.mPurchases[38] == 1)
			{
				return 0;
			}
			return 10000;
		case StoreItem.STORE_ITEM_CATTAIL_DRIVER:
			return 0;
		case StoreItem.STORE_ITEM_FUMESHROOM_GNOME:
			return 5000;
		default:
			Debug.ASSERT(value: false);
			return 0;
		case StoreItem.STORE_ITEM_BONUS_LAWN_MOWER:
			if (lawnApp.mPlayerInfo.mPurchases[9] == 0)
			{
				return 200;
			}
			return 500;
		}
	}

	public bool CanAffordItem(StoreItem theStoreItem)
	{
		int itemCost = GetItemCost(theStoreItem);
		return mApp.mPlayerInfo.mCoins >= itemCost;
	}

	public void PurchaseItem(StoreItem theItemType)
	{
		mBubbleCountDown = 0;
		mApp.CrazyDaveStopTalking();
		if (!CanAffordItem(theItemType))
		{
			mApp.DoDialog(25, isModal: true, "[NOT_ENOUGH_MONEY]", "[CANNOT_AFFORD_ITEM]", "[DIALOG_BUTTON_OK]", 3);
			return;
		}
		mPendingPurchaseItem = theItemType;
		int theMessageIndex = 0;
		switch (theItemType)
		{
		case StoreItem.STORE_ITEM_PLANT_GATLINGPEA:
			theMessageIndex = 2000;
			break;
		case StoreItem.STORE_ITEM_PLANT_TWINSUNFLOWER:
			theMessageIndex = 2001;
			break;
		case StoreItem.STORE_ITEM_PLANT_GLOOMSHROOM:
			theMessageIndex = 2002;
			break;
		case StoreItem.STORE_ITEM_PLANT_CATTAIL:
			theMessageIndex = 2003;
			break;
		case StoreItem.STORE_ITEM_PLANT_WINTERMELON:
			theMessageIndex = 2004;
			break;
		case StoreItem.STORE_ITEM_PLANT_GOLD_MAGNET:
			theMessageIndex = 2005;
			break;
		case StoreItem.STORE_ITEM_PLANT_SPIKEROCK:
			theMessageIndex = 2006;
			break;
		case StoreItem.STORE_ITEM_PLANT_COBCANNON:
			theMessageIndex = 2007;
			break;
		case StoreItem.STORE_ITEM_PLANT_IMITATER:
			theMessageIndex = 2008;
			break;
		case StoreItem.STORE_ITEM_BONUS_LAWN_MOWER:
			theMessageIndex = 2009;
			break;
		case StoreItem.STORE_ITEM_POTTED_MARIGOLD_1:
			theMessageIndex = 2010;
			break;
		case StoreItem.STORE_ITEM_POTTED_MARIGOLD_2:
			theMessageIndex = 2010;
			break;
		case StoreItem.STORE_ITEM_POTTED_MARIGOLD_3:
			theMessageIndex = 2010;
			break;
		case StoreItem.STORE_ITEM_GOLD_WATERINGCAN:
			theMessageIndex = ((mApp.mPlayerInfo.mPurchases[13] + 1 != 1) ? 10010 : 2019);
			break;
		case StoreItem.STORE_ITEM_FERTILIZER:
			theMessageIndex = 2020;
			break;
		case StoreItem.STORE_ITEM_BUG_SPRAY:
			theMessageIndex = 2022;
			break;
		case StoreItem.STORE_ITEM_PHONOGRAPH:
			theMessageIndex = 2021;
			break;
		case StoreItem.STORE_ITEM_GARDENING_GLOVE:
			theMessageIndex = 2023;
			break;
		case StoreItem.STORE_ITEM_MUSHROOM_GARDEN:
			theMessageIndex = 2032;
			break;
		case StoreItem.STORE_ITEM_WHEEL_BARROW:
			theMessageIndex = 2024;
			break;
		case StoreItem.STORE_ITEM_STINKY_THE_SNAIL:
			theMessageIndex = 2025;
			break;
		case StoreItem.STORE_ITEM_PACKET_UPGRADE:
			if (mApp.mPlayerInfo.mPurchases[21] + 1 != 1)
			{
				if (mApp.mPlayerInfo.mPurchases[21] + 1 != 2)
				{
					theMessageIndex = ((mApp.mPlayerInfo.mPurchases[21] + 1 != 3) ? 2014 : 2013);
				}
				else
				{
					theMessageIndex = 2012;
				}
			}
			else
			{
				theMessageIndex = 2011;
			}
			break;
		case StoreItem.STORE_ITEM_POOL_CLEANER:
			theMessageIndex = 2026;
			break;
		case StoreItem.STORE_ITEM_ROOF_CLEANER:
			theMessageIndex = 2027;
			break;
		case StoreItem.STORE_ITEM_RAKE:
			theMessageIndex = 2028;
			break;
		case StoreItem.STORE_ITEM_AQUARIUM_GARDEN:
			theMessageIndex = 2029;
			break;
		case StoreItem.STORE_ITEM_TREE_OF_WISDOM:
			theMessageIndex = 2030;
			break;
		case StoreItem.STORE_ITEM_TREE_FOOD:
			theMessageIndex = 2031;
			break;
		case StoreItem.STORE_ITEM_FIRSTAID:
			theMessageIndex = 2033;
			break;
		case StoreItem.STORE_ITEM_GARLICFIRSTAID:
			theMessageIndex = 10000;
			break;
		case StoreItem.STORE_ITEM_PLANT_SUPER_CHOMPER:
			theMessageIndex = 10001;
			break;
		case StoreItem.STORE_ITEM_PLANT_PICKLED_PEPPER:
			theMessageIndex = 10002;
			break;
		case StoreItem.STORE_ITEM_PLANT_FIRE_SHROOM:
			theMessageIndex = 10003;
			break;
		case StoreItem.STORE_ITEM_PLANT_AGAVE:
			theMessageIndex = 10004;
			break;
		case StoreItem.STORE_ITEM_AGAVE_SKILL:
			theMessageIndex = 10005;
			break;
		case StoreItem.STORE_ITEM_CARD_GROUP_UPGRADE:
			if (mApp.mPlayerInfo.mPurchases[36] + 1 != 1)
			{
				if (mApp.mPlayerInfo.mPurchases[36] + 1 != 2)
				{
					theMessageIndex = ((mApp.mPlayerInfo.mPurchases[36] + 1 != 3) ? 10009 : 10008);
				}
				else
				{
					theMessageIndex = 10007;
				}
			}
			else
			{
				theMessageIndex = 10006;
			}
			break;
		case StoreItem.STORE_ITEM_GREENHOUSE_NIGHT:
			theMessageIndex = 10011;
			break;
		case StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO:
			theMessageIndex = mApp.mPlayerInfo.mPurchases[38] switch
			{
				0 => 10012, 
				1 => 10013, 
				_ => -1, 
			};
			break;
		case StoreItem.STORE_ITEM_CATTAIL_DRIVER:
			theMessageIndex = 10014;
			break;
		case StoreItem.STORE_ITEM_FUMESHROOM_GNOME:
			theMessageIndex = 10015;
			break;
		case StoreItem.STORE_ITEM_PLANT_IRIS_SHOOTER:
			theMessageIndex = 10016;
			break;
		default:
			Debug.ASSERT(value: false);
			break;
		}
		string crazyDaveText = mApp.GetCrazyDaveText(theMessageIndex);
		int num = mApp.mPlayerInfo.mPurchases[(int)theItemType] + 1;
		if (theItemType == StoreItem.STORE_ITEM_PACKET_UPGRADE)
		{
			num += 6;
		}
		ConfirmPurchaseDialog confirmPurchaseDialog = new ConfirmPurchaseDialog(mApp, theItemType, num, GetItemCost(theItemType), crazyDaveText);
		confirmPurchaseDialog.Resize(Constants.StoreScreen_Dialog.X, Constants.StoreScreen_Dialog.Y, Constants.StoreScreen_Dialog.Width, Constants.StoreScreen_Dialog.Height);
		mApp.AddDialog(46, confirmPurchaseDialog);
	}

	public void PurchasePendingItem()
	{
		mApp.KillDialog(46);
		StoreItem storeItem = mPendingPurchaseItem;
		int itemCost = GetItemCost(storeItem);
		mApp.mPlayerInfo.AddCoins(-itemCost);
		mApp.mPlayerInfo.mMoneySpent += itemCost;
		if (mApp.mPlayerInfo.mMoneySpent >= 2500)
		{
			ReportAchievement.GiveAchievement(AchievementId.Shopaholic);
		}
		switch (storeItem)
		{
		case StoreItem.STORE_ITEM_PACKET_UPGRADE:
		{
			mApp.mPlayerInfo.mPurchases[21]++;
			string theDialogLines2 = Common.StrFormat_(TodStringFile.TodStringTranslate("[NOW_YOU_CAN_CHOOSE_X_SEEDS]"), 6 + mApp.mPlayerInfo.mPurchases[21]);
			LawnDialog lawnDialog2 = mApp.DoDialog(26, isModal: true, "[MORE_SLOTS]", theDialogLines2, "[DIALOG_BUTTON_OK]", 3);
			lawnDialog2.CalcSize(0, 0);
			LawnApp.CenterDialog(lawnDialog2, lawnDialog2.mWidth, lawnDialog2.mHeight);
			if (mApp.mBoard != null)
			{
				mApp.mBoard.mSeedBank.UpdateHeight();
			}
			break;
		}
		case StoreItem.STORE_ITEM_CARD_GROUP_UPGRADE:
			mApp.mPlayerInfo.mPurchases[(int)storeItem]++;
			if (mApp.mPlayerInfo.mPurchases[(int)storeItem] > 1)
			{
				string theDialogLines = Common.StrFormat_(TodStringFile.TodStringTranslate("[NOW_YOU_CAN_CHOOSE_X_CARD_GROUPS]"), mApp.mPlayerInfo.mPurchases[(int)storeItem]);
				LawnDialog lawnDialog = mApp.DoDialog(26, isModal: true, "[MORE_CARD_GROUPS]", theDialogLines, "[DIALOG_BUTTON_OK]", 3);
				lawnDialog.CalcSize(0, 0);
				LawnApp.CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
			}
			break;
		case StoreItem.STORE_ITEM_GOLD_WATERINGCAN:
			mApp.mPlayerInfo.mPurchases[(int)storeItem]++;
			break;
		case StoreItem.STORE_ITEM_BONUS_LAWN_MOWER:
			mApp.mPlayerInfo.mPurchases[9]++;
			break;
		case StoreItem.STORE_ITEM_RAKE:
			mApp.mPlayerInfo.mPurchases[24] = 10;
			break;
		case StoreItem.STORE_ITEM_FERTILIZER:
		case StoreItem.STORE_ITEM_BUG_SPRAY:
			if (mApp.mPlayerInfo.mPurchases[(int)storeItem] < 1000)
			{
				mApp.mPlayerInfo.mPurchases[(int)storeItem] = 1000;
			}
			mApp.mPlayerInfo.mPurchases[(int)storeItem] += 5;
			break;
		case StoreItem.STORE_ITEM_TREE_FOOD:
			if (mApp.mPlayerInfo.mPurchases[(int)storeItem] < 1000)
			{
				mApp.mPlayerInfo.mPurchases[(int)storeItem] = 1000;
			}
			mApp.mPlayerInfo.mPurchases[(int)storeItem]++;
			break;
		case StoreItem.STORE_ITEM_TREE_OF_WISDOM:
			mApp.mPlayerInfo.mPurchases[(int)storeItem] = 1;
			mApp.mPlayerInfo.mChallengeRecords[48] = 1;
			break;
		default:
			if (IsPottedPlant(storeItem))
			{
				mApp.mZenGarden.AddPottedPlant(mPottedPlantSpecs);
				mPottedPlantSpecs.InitializePottedPlant(SeedType.Marigold);
				mPottedPlantSpecs.mDrawVariation = (DrawVariation)TodCommon.RandRangeInt(2, 12);
				mApp.mPlayerInfo.mPurchases[(int)storeItem] = LawnCommon.GetCurrentDaysSince2000();
			}
			else if (storeItem == StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO || storeItem == StoreItem.STORE_ITEM_CATTAIL_DRIVER)
			{
				if (mApp.mPlayerInfo.mPurchases[38] != 1)
				{
					mApp.mPlayerInfo.mPurchases[38] = 1;
				}
				else if (storeItem == StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO)
				{
					mApp.mPlayerInfo.mPurchases[39] = 0;
				}
				if (storeItem == StoreItem.STORE_ITEM_CATTAIL_DRIVER)
				{
					mApp.mPlayerInfo.mPurchases[(int)storeItem] = 1;
				}
				if (mApp.mGameMode != GameMode.ChallengeZenGarden || mApp.mBoard == null || mApp.mBoard.mPlants == null)
				{
					break;
				}
				for (int num = mApp.mBoard.mPlants.Count - 1; num >= 0; num--)
				{
					Plant plant = mApp.mBoard.mPlants[num];
					if (!plant.mDead && (plant.mSeedType == SeedType.Cattail || plant.mSeedType == SeedType.HypnoCattail) && plant.mPottedPlantIndex != -1)
					{
						PlantsOnLawn thePlantOnLawn = default;
						mApp.mBoard.GetPlantsOnLawn(plant.mPlantCol, plant.mRow, ref thePlantOnLawn);
						Plant mUnderPlant = thePlantOnLawn.mUnderPlant;
						if (mUnderPlant != null && mUnderPlant.mSeedType == SeedType.Flowerpot)
						{
							thePlantOnLawn.mUnderPlant.mDead = true;
						}
						mApp.mZenGarden.ReloadPlant(plant);
					}
				}
			}
			else
			{
				Debug.ASSERT(storeItem >= StoreItem.STORE_ITEM_PLANT_GATLINGPEA && storeItem < (StoreItem)80);
				mApp.mPlayerInfo.mPurchases[(int)storeItem] = 1;
			}
			break;
		}
		if (storeItem == StoreItem.STORE_ITEM_FIRSTAID)
		{
			SetBubbleText(3400, 800, theClickToContinue: false);
		}
		if (mApp.mSeedChooserScreen != null)
		{
			mApp.mSeedChooserScreen.UpdateAfterPurchase();
		}
		SeedType seedType;
		for (seedType = SeedType.Peashooter; seedType < SeedType.ExplodeONut; seedType++)
		{
			if (!mApp.HasSeedType(seedType))
			{
				seedType = SeedType.None;
				break;
			}
		}
		if (seedType == SeedType.ExplodeONut && !mApp.mPlayerInfo.mShownAchievements[1])
		{
			ReportAchievement.GiveAchievement(AchievementId.ACHIEVEMENT_MORTICULTURALIST);
			mApp.mPlayerInfo.mShownAchievements[1] = true;
			SetBubbleText(4000, 150, theClickToContinue: false);
			mBubbleAutoAdvance = true;
		}
		mApp.WriteCurrentUserConfig();
	}

	public void FinishTreeOfWisdomDialog(bool isYes)
	{
		mApp.KillDialog(47);
		if (mApp.mSeedChooserScreen != null)
		{
			mApp.mSeedChooserScreen.UpdateAfterPurchase();
		}
		mApp.WriteCurrentUserConfig();
		if (isYes)
		{
			mGoToTreeNow = true;
			mResult = 1000;
			if (mListener != null)
			{
				mListener.BackFromStore();
			}
		}
	}

	public void DrawItem(Graphics g, int theItemPosition, StoreItem theItemType)
	{
		if (!IsItemUnavailable(theItemType))
		{
			int thePosX = 0;
			int thePosY = 0;
			GetStorePosition(theItemPosition, ref thePosX, ref thePosY);
			int num = mApp.mPlayerInfo.mPurchases[(int)theItemType] + 1;
			if (theItemType == StoreItem.STORE_ITEM_PACKET_UPGRADE)
			{
				num += 6;
			}
			GlobalMembersStoreScreen.DrawStoreItem(g, thePosX, thePosY, theItemType, IsComingSoon(theItemType), IsItemSoldOut(theItemType), num, GetItemCost(theItemType));
		}
	}

	public void EnableButtons(bool theEnable)
	{
		bool flag = true;
		if (!mEasyBuyingCheat && !IsPageShown(StorePage.PlantUpgrades))
		{
			flag = false;
		}
		if (flag || !theEnable)
		{
			mNextButton.mMouseVisible = theEnable;
			mNextButton.SetDisabled(!theEnable);
			mPrevButton.mMouseVisible = theEnable;
			mPrevButton.SetDisabled(!theEnable);
		}
		mBackButton.mMouseVisible = theEnable;
		mBackButton.SetDisabled(!theEnable);
	}

	public void SetupForIntro(int theDialogIndex)
	{
		mStartDialog = theDialogIndex;
		mHatchOpen = false;
		SetupBackButtonForZenGarden();
		EnableButtons(theEnable: false);
	}

	public void SetupBackButtonForZenGarden()
	{
		mBackButton.mButtonImage = AtlasResources.IMAGE_STORE_CONTINUEBUTTON;
		mBackButton.mDownImage = AtlasResources.IMAGE_STORE_CONTINUEBUTTONDOWN;
	}

	public bool CanInteractWithButtons()
	{
		if (mStoreTime >= 120 && !mBubbleClickToContinue && !mBubbleAutoAdvance && mHatchTimer <= 0)
		{
			return !IsWaitingForDialog();
		}
		return false;
	}

	public static bool IsPottedPlant(StoreItem theStoreItem)
	{
		if (theStoreItem != StoreItem.STORE_ITEM_POTTED_MARIGOLD_1 && theStoreItem != StoreItem.STORE_ITEM_POTTED_MARIGOLD_2)
		{
			return theStoreItem == StoreItem.STORE_ITEM_POTTED_MARIGOLD_3;
		}
		return true;
	}

	public void AdvanceCrazyDaveDialog()
	{
		if (!mBubbleClickToContinue && !mBubbleAutoAdvance)
		{
			return;
		}
		if (mApp.mCrazyDaveMessageIndex == 3100)
		{
			mHatchTimer = 150;
			mHatchOpen = true;
			mApp.PlaySample(Resources.SOUND_HATCHBACK_OPEN);
		}
		if (!mApp.AdvanceCrazyDaveText())
		{
			mApp.CrazyDaveStopTalking();
			mBubbleClickToContinue = false;
			mBubbleAutoAdvance = false;
			mAmbientSpeechCountDown = 500;
			if (mHatchTimer == 0)
			{
				EnableButtons(theEnable: true);
			}
		}
		else
		{
			int theTime = 0;
			if (mApp.mCrazyDaveMessageIndex == 4001)
			{
				theTime = 200;
			}
			else if (mApp.mCrazyDaveMessageIndex == 4002)
			{
				theTime = 400;
			}
			else if (mApp.mCrazyDaveMessageIndex == 4003)
			{
				theTime = 150;
			}
			else if (mApp.mCrazyDaveMessageIndex == 4004)
			{
				theTime = 500;
			}
			SetBubbleText(mApp.mCrazyDaveMessageIndex, theTime, mBubbleClickToContinue);
		}
		if (mApp.mCrazyDaveMessageIndex == 303 || mApp.mCrazyDaveMessageIndex == 606 || mApp.mCrazyDaveMessageIndex == 2105 || mApp.mCrazyDaveMessageIndex == 2601)
		{
			mHatchTimer = 150;
			mHatchOpen = true;
			mApp.PlaySample(Resources.SOUND_HATCHBACK_OPEN);
		}
		if (mApp.mCrazyDaveMessageIndex == 603)
		{
			mApp.mPlayerInfo.AddCoins(100);
			mApp.mPlayerInfo.mNeedsMagicTacoReward = false;
			mApp.WriteCurrentUserConfig();
		}
		if (mApp.mCrazyDaveMessageIndex == 2103)
		{
			mApp.mPlayerInfo.mNeedsMagicBaconReward = false;
			mApp.WriteCurrentUserConfig();
			mApp.PlaySample(Resources.SOUND_DIAMOND);
			Coin coin = mCoins.DataArrayAlloc();
			coin.CoinInitialize(80, 520, CoinType.Diamond, CoinMotion.FromPresent);
			coin.mVelX = 0f;
			coin.mVelY = -5f;
		}
		if (mApp.mCrazyDaveMessageIndex == 902)
		{
			mApp.mPlayerInfo.AddCoins(100);
		}
		if (mApp.mCrazyDaveMessageIndex == 1002)
		{
			mApp.mPlayerInfo.AddCoins(100);
		}
	}

	public bool IsComingSoon(StoreItem theStoreItem)
	{
		if (!IsFullVersionOnly(theStoreItem) && (!IsPottedPlant(theStoreItem) || mApp.HasFinishedAdventure()))
		{
			if (theStoreItem == StoreItem.STORE_ITEM_TREE_FOOD)
			{
				if (mApp.mPlayerInfo.mPurchases[27] != 0)
				{
					return mApp.mPlayerInfo.mPurchases[28] < 1000;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public void StorePreLoad()
	{
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(ReanimationType.CrazyDave, theIsPreloading: true);
		mApp.CrazyDaveEnter();
		Plant.PreloadPlantResources(SeedType.Gatlingpea);
		Plant.PreloadPlantResources(SeedType.Twinsunflower);
		if (mApp.HasFinishedAdventure())
		{
			Plant.PreloadPlantResources(SeedType.Twinsunflower);
			Plant.PreloadPlantResources(SeedType.Gloomshroom);
			Plant.PreloadPlantResources(SeedType.Cattail);
			Plant.PreloadPlantResources(SeedType.Wintermelon);
			Plant.PreloadPlantResources(SeedType.GoldMagnet);
			Plant.PreloadPlantResources(SeedType.Spikerock);
			Plant.PreloadPlantResources(SeedType.Cobcannon);
			Plant.PreloadPlantResources(SeedType.Imitater);
		}
	}

	public bool IsPageShown(StorePage thePage)
	{
		if (mApp.IsTrialStageLocked())
		{
			return thePage == StorePage.SlotUpgrades;
		}
		if ((thePage != StorePage.Zen1 || mApp.mPlayerInfo.mZenGardenTutorialComplete || mApp.mZenGarden.mIsTutorial) && (thePage != StorePage.Zen2 || mApp.mPlayerInfo.mZenGardenTutorialComplete || mApp.HasFinishedAdventure()))
		{
			if (!mApp.HasFinishedAdventure() && thePage == StorePage.PlantUpgrades)
			{
				return mApp.mPlayerInfo.mLevel >= 42;
			}
			return true;
		}
		return false;
	}

	public override void DrawOverlay(Graphics g)
	{
	}

	public bool IsFullVersionOnly(StoreItem theStoreItem)
	{
		if (mApp.IsTrialStageLocked())
		{
			if (theStoreItem == StoreItem.STORE_ITEM_PACKET_UPGRADE && mApp.mPlayerInfo.mPurchases[21] >= 2)
			{
				return true;
			}
			if (theStoreItem == StoreItem.STORE_ITEM_PLANT_TWINSUNFLOWER)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsWaitingForDialog()
	{
		if (mApp.GetDialog(Dialogs.DIALOG_STORE_PURCHASE) == null && mApp.GetDialog(Dialogs.DIALOG_NOT_ENOUGH_MONEY) == null && mApp.GetDialog(Dialogs.DIALOG_VISIT_TREE_OF_WISDOM) == null)
		{
			return mApp.GetDialog(Dialogs.DIALOG_UPGRADED) != null;
		}
		return true;
	}
}
