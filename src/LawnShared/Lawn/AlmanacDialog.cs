using System.Linq;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class AlmanacDialog : LawnDialog
{
	public int mFullViewCountdown;

	public static TPoint[] ZombieOffsets = new TPoint[26]
	{
		new TPoint(20, 11),
		new TPoint(13, 15),
		new TPoint(22, 14),
		new TPoint(3, 13),
		new TPoint(20, 13),
		new TPoint(18, 13),
		new TPoint(19, 13),
		new TPoint(4, 11),
		new TPoint(14, 13),
		new TPoint(19, 12),
		new TPoint(20, 13),
		new TPoint(16, 10),
		new TPoint(1, 11),
		new TPoint(16, 10),
		new TPoint(16, 15),
		new TPoint(14, 10),
		new TPoint(13, 12),
		new TPoint(10, 13),
		new TPoint(20, 12),
		new TPoint(11, 12),
		new TPoint(0, 9),
		new TPoint(15, 14),
		new TPoint(4, 12),
		new TPoint(18, 10),
		new TPoint(23, 27),
		new TPoint(1, 15)
	};

	public static bool[] gZombieDefeated = new bool[40];

	public new LawnApp mApp;

	public TRect mViewPlantsRect;

	public TRect mViewZombiesRect;

	public ScrollWidget mPlantsScrollWidget;

	public ScrollWidget mZombiesScrollWidget;

	public ScrollWidget mDescriptionScrollWidget;

	public DescriptionWidget mDescriptionWidget;

	public PlantGalleryWidget mPlantGalleryWidget;

	public ZombieGalleryWidget mZombieGalleryWidget;

	public GameButton mCloseButton;

	public GameButton mIndexButton;

	public GameButton mShowTachieButton;

	public AlmanacPage mOpenPage;

	public Reanimation[] mReanim = new Reanimation[GameConstants.NUM_ALMANAC_REANIMS];

	public SeedType mSelectedSeed;

	public ZombieType mSelectedZombie;

	public Plant mPlant;

	public Zombie mZombie;

	public int mHalfDeltaWidth;

	public int mHalfDeltaHeight;

	public AlmanacListener mListener;

	public bool mInFullView;

	public static void AlmanacInitForPlayer()
	{
	}

	public static void AlmanacPlayerDefeatedZombie(ZombieType theZombieType)
	{
		gZombieDefeated[(int)theZombieType] = true;
	}

	public AlmanacDialog(LawnApp theApp, AlmanacListener theListener)
		: base(theApp, null, 3, isModal: true, "[ALMANAC_HEADER]", "", "", 0)
	{
		mApp = (LawnApp)GlobalStaticVars.gSexyAppBase;
		mListener = theListener;
		mOpenPage = AlmanacPage.Index;
		mSelectedSeed = SeedType.Peashooter;
		mSelectedZombie = ZombieType.Normal;
		mZombie = null;
		mPlant = null;
		mDrawStandardBack = false;
		mViewPlantsRect = Constants.Almanac_PlantsButtonRect;
		mViewZombiesRect = Constants.Almanac_ZombiesButtonRect;
		mPlantGalleryWidget = new PlantGalleryWidget(this);
		mPlantsScrollWidget = new ScrollWidget();
		mPlantsScrollWidget.Resize(Constants.Almanac_PlantScrollRect.mX, Constants.Almanac_PlantScrollRect.mY, Constants.Almanac_PlantScrollRect.mWidth, Constants.Almanac_PlantScrollRect.mHeight);
		mPlantsScrollWidget.AddWidget(mPlantGalleryWidget);
		mPlantGalleryWidget.Move(0, 0);
		AddWidget(mPlantsScrollWidget);
		mPlantsScrollWidget.SetVisible(isVisible: false);
		mPlantsScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mZombieGalleryWidget = new ZombieGalleryWidget(this);
		mZombiesScrollWidget = new ScrollWidget();
		mZombiesScrollWidget.Resize(Constants.Almanac_ZombieScrollRect.mX, Constants.Almanac_ZombieScrollRect.mY, Constants.Almanac_ZombieScrollRect.mWidth, Constants.Almanac_ZombieScrollRect.mHeight);
		mZombiesScrollWidget.AddWidget(mZombieGalleryWidget);
		mZombieGalleryWidget.Move(0, 0);
		AddWidget(mZombiesScrollWidget);
		mZombiesScrollWidget.SetVisible(isVisible: false);
		mZombiesScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		mDescriptionWidget = new DescriptionWidget();
		mDescriptionScrollWidget = new ScrollWidget();
		mDescriptionScrollWidget.Resize(Constants.Almanac_DescriptionScrollRect.mX, Constants.Almanac_DescriptionScrollRect.mY, Constants.Almanac_DescriptionScrollRect.mWidth, Constants.Almanac_DescriptionScrollRect.mHeight);
		mDescriptionWidget.mWidth = mDescriptionScrollWidget.mWidth;
		mDescriptionScrollWidget.AddWidget(mDescriptionWidget);
		mDescriptionWidget.Move(0, 0);
		mDescriptionScrollWidget.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		AddWidget(mDescriptionScrollWidget);
		Resize(0, 0, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT);
		mCloseButton = new GameButton(0, this);
		mCloseButton.mButtonImage = AtlasResources.IMAGE_ALMANAC_CLOSEBUTTON;
		mCloseButton.mOverImage = null;
		mCloseButton.mDownImage = null;
		mCloseButton.SetFont(Resources.FONT_BRIANNETOD12);
		mCloseButton.mColors[0] = new SexyColor(42, 42, 90);
		mCloseButton.mColors[1] = new SexyColor(42, 42, 90);
		mCloseButton.Resize(Constants.Almanac_CloseButtonRect.mX, Constants.Almanac_CloseButtonRect.mY, Constants.Almanac_CloseButtonRect.mWidth, Constants.Almanac_CloseButtonRect.mHeight);
		mCloseButton.mParentWidget = this;
		mCloseButton.mTextOffsetX = -8;
		mCloseButton.mTextOffsetY = 1;
		mIndexButton = new GameButton(3, this);
		mIndexButton.mButtonImage = AtlasResources.IMAGE_ALMANAC_BACKBUTTON;
		mIndexButton.mOverImage = null;
		mIndexButton.mDownImage = null;
		mIndexButton.SetFont(Resources.FONT_BRIANNETOD12);
		mIndexButton.mColors[0] = new SexyColor(42, 42, 90);
		mIndexButton.mColors[1] = new SexyColor(42, 42, 90);
		mIndexButton.Resize(Constants.Almanac_IndexButtonRect.mX, Constants.Almanac_IndexButtonRect.mY, Constants.Almanac_IndexButtonRect.mWidth, Constants.Almanac_IndexButtonRect.mHeight);
		mIndexButton.mParentWidget = this;
		mIndexButton.mTextOffsetX = 8;
		mIndexButton.mTextOffsetY = 1;
		mShowTachieButton = new GameButton(4, this);
		mShowTachieButton.mButtonImage = AtlasResources.IMAGE_REANIM_YIFU;
		mShowTachieButton.Resize(Constants.Almanac_BrownRect.mX + Constants.Almanac_BrownRect.mWidth + 10, Constants.Almanac_BrownRect.mY, mShowTachieButton.mButtonImage.mWidth, mShowTachieButton.mButtonImage.mHeight);
		mShowTachieButton.mParentWidget = this;
		SetPage(AlmanacPage.Index);
		if (mApp.mBoard == null || !mApp.mBoard.mPaused)
		{
			mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ChooseYourSeeds);
		}
	}

	public override void Dispose()
	{
		mCloseButton.Dispose();
		mIndexButton.Dispose();
		mShowTachieButton.Dispose();
		ClearPlantsAndZombies();
		RemoveAllWidgets(doDelete: true);
		base.Dispose();
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
		if (mFullViewCountdown > 0)
		{
			mFullViewCountdown--;
			if (!mInFullView && mFullViewCountdown < 50)
			{
				int theTimeAge = (mInFullView ? (200 - mFullViewCountdown) : mFullViewCountdown);
				mDescriptionScrollWidget.mY = TodCommon.TodAnimateCurve(50, 0, theTimeAge, Constants.Almanac_DescriptionScrollRect.mY + 140, Constants.Almanac_DescriptionScrollRect.mY - 220, TodCurves.EaseInOut);
				mDescriptionScrollWidget.mHeight = TodCommon.TodAnimateCurve(50, 0, theTimeAge, Constants.Almanac_DescriptionScrollRect.mHeight - 140, Constants.Almanac_DescriptionScrollRect.mHeight + 220, TodCurves.EaseInOut);
				mDescriptionScrollWidget.mX = Constants.Almanac_DescriptionScrollRect.mX;
				mDescriptionScrollWidget.SetVisible(isVisible: true);
				mDescriptionScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.Disabled);
				mDescriptionScrollWidget.ScrollToMin(animated: false);
			}
			else if (mInFullView && mFullViewCountdown >= 150)
			{
				int theTimeAge2 = (mInFullView ? (200 - mFullViewCountdown) : mFullViewCountdown);
				mDescriptionScrollWidget.mX = TodCommon.TodAnimateCurve(0, 50, theTimeAge2, Constants.Almanac_DescriptionScrollRect.mX, Constants.Almanac_DescriptionScrollRect.mX + 600, TodCurves.EaseIn);
				mDescriptionScrollWidget.SetVisible(isVisible: true);
				mDescriptionScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.Disabled);
				mDescriptionScrollWidget.ScrollToMin(animated: false);
			}
			if (!mInFullView)
			{
				if (mFullViewCountdown >= 50 && mFullViewCountdown < 150)
				{
					mDescriptionScrollWidget.SetVisible(isVisible: false);
					mPlantsScrollWidget.SetVisible(isVisible: false);
					mZombiesScrollWidget.SetVisible(isVisible: false);
				}
			}
			else if (mFullViewCountdown > 50 && mFullViewCountdown < 150)
			{
				mDescriptionScrollWidget.SetVisible(isVisible: false);
			}
			else if (mFullViewCountdown <= 50)
			{
				mDescriptionScrollWidget.mY = Constants.Almanac_DescriptionScrollRect.mY;
				mDescriptionScrollWidget.mHeight = Constants.Almanac_DescriptionScrollRect.mHeight;
				mDescriptionScrollWidget.mX = Constants.Almanac_DescriptionScrollRect.mX;
				mDescriptionScrollWidget.SetVisible(isVisible: true);
				mDescriptionScrollWidget.SetScrollMode(ScrollWidget.ScrollMode.Vertical);
				if (mOpenPage == AlmanacPage.Plants)
				{
					mPlantsScrollWidget.SetVisible(isVisible: true);
				}
				else if (mOpenPage == AlmanacPage.Zombies)
				{
					mZombiesScrollWidget.SetVisible(isVisible: true);
				}
			}
			if (mFullViewCountdown == 0)
			{
				mInFullView = !mInFullView;
				_ = mInFullView;
			}
		}
		UpdateScreen();
		mCloseButton.Update();
		mIndexButton.Update();
		if (!mInFullView && mFullViewCountdown == 0)
		{
			mShowTachieButton.Update();
		}
		if (mPlant != null)
		{
			mPlant.Update();
		}
		if (mZombie != null)
		{
			mZombie.Update();
		}
		MarkDirty();
	}

	public override bool BackButtonPress()
	{
		switch (mOpenPage)
		{
		case AlmanacPage.Index:
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
			mApp.KillAlmanacDialog();
			if (mListener != null)
			{
				mListener.BackFromAlmanac();
			}
			break;
		case AlmanacPage.Plants:
		case AlmanacPage.Zombies:
			mApp.PlaySample(Resources.SOUND_TAP);
			SetPage(AlmanacPage.Index);
			break;
		}
		return true;
	}

	public override void Draw(Graphics g)
	{
		g.SetLinearBlend(linear: true);
		g.SetClipRect(FullRect);
		if (mOpenPage == AlmanacPage.Index)
		{
			DrawIndex(g);
		}
		else if (mOpenPage == AlmanacPage.Plants)
		{
			DrawPlants(g);
		}
		else if (mOpenPage == AlmanacPage.Zombies)
		{
			DrawZombies(g);
		}
		mCloseButton.Draw(g);
		mIndexButton.Draw(g);
		if (!mInFullView && mFullViewCountdown == 0)
		{
			mShowTachieButton.Draw(g);
		}
		if (mOpenPage != AlmanacPage.Index)
		{
			DeferOverlay();
		}
	}

	public override void DrawOverlay(Graphics g)
	{
		if ((mInFullView && mFullViewCountdown < 50 && mFullViewCountdown > 0) || (!mInFullView && (mFullViewCountdown >= 150 || mFullViewCountdown == 0)))
		{
			if (mOpenPage == AlmanacPage.Plants)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(Constants.Almanac_Paper_Colour);
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_TOPGRADIENT, mPlantsScrollWidget.mX, Constants.Almanac_PlantTopGradientY, Constants.Almanac_PlantGradientWidth, AtlasResources.IMAGE_ALMANAC_PLANTS_TOPGRADIENT.mHeight);
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_BOTTOMGRADIENT, mPlantsScrollWidget.mX, Constants.Almanac_BottomGradientY, Constants.Almanac_PlantGradientWidth, AtlasResources.IMAGE_ALMANAC_PLANTS_BOTTOMGRADIENT.mHeight);
				g.SetColorizeImages(colorizeImages: false);
			}
			else if (mOpenPage == AlmanacPage.Zombies)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(Constants.Almanac_Paper_Colour);
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_TOPGRADIENT, mZombiesScrollWidget.mX, Constants.Almanac_ZombieTopGradientY, Constants.Almanac_ZombieGradientWidth, AtlasResources.IMAGE_ALMANAC_PLANTS_TOPGRADIENT.mHeight);
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_BOTTOMGRADIENT, mZombiesScrollWidget.mX, Constants.Almanac_BottomGradientY, Constants.Almanac_ZombieGradientWidth, AtlasResources.IMAGE_ALMANAC_PLANTS_BOTTOMGRADIENT.mHeight);
				g.SetColorizeImages(colorizeImages: false);
			}
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mOpenPage == AlmanacPage.Index && mViewPlantsRect.Contains(new TPoint(x, y)))
		{
			SetPage(AlmanacPage.Plants);
			return;
		}
		if (mOpenPage == AlmanacPage.Index && mViewZombiesRect.Contains(new TPoint(x, y)))
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
			SetPage(AlmanacPage.Zombies);
			return;
		}
		if (mCloseButton.IsMouseOver())
		{
			mApp.KillAlmanacDialog();
			if (mListener != null)
			{
				mListener.BackFromAlmanac();
			}
			return;
		}
		if (mIndexButton.IsMouseOver())
		{
			SetPage(AlmanacPage.Index);
		}
		if (mShowTachieButton.IsMouseOver())
		{
			SeedType seedType = mSelectedSeed;
			Image image = null;
			Image image2 = null;
			if (mSelectedSeed == SeedType.Chomper)
			{
				image = Resources.IMAGE_PAINTING6_0;
				image2 = Resources.IMAGE_PAINTING6_1;
			}
			else if (mSelectedSeed == SeedType.Repeater)
			{
				image = Resources.IMAGE_PAINTING7_0;
				image2 = Resources.IMAGE_PAINTING7_1;
			}
			else if (mSelectedSeed == SeedType.Gravebuster)
			{
				image = Resources.IMAGE_PAINTING11_0;
				image2 = Resources.IMAGE_PAINTING11_1;
			}
			else if (mSelectedSeed == SeedType.Scaredyshroom)
			{
				image = Resources.IMAGE_PAINTING13_0;
				image2 = Resources.IMAGE_PAINTING13_1;
			}
			else if (mSelectedSeed == SeedType.Lilypad)
			{
				image = Resources.IMAGE_PAINTING16_0;
				image2 = Resources.IMAGE_PAINTING16_1;
			}
			else if (mSelectedSeed == SeedType.Jalapeno)
			{
				image = Resources.IMAGE_PAINTING20_0;
				image2 = Resources.IMAGE_PAINTING20_1;
			}
			else if (mSelectedSeed == SeedType.Spikeweed)
			{
				image = Resources.IMAGE_PAINTING21_0;
				image2 = Resources.IMAGE_PAINTING21_1;
			}
			else if (mSelectedSeed == SeedType.Plantern)
			{
				image = Resources.IMAGE_PAINTING25_0;
				image2 = Resources.IMAGE_PAINTING25_1;
			}
			else if (mSelectedSeed == SeedType.Kernelpult)
			{
				image = Resources.IMAGE_PAINTING34_0;
				image2 = Resources.IMAGE_PAINTING34_1;
			}
			else if (mSelectedSeed == SeedType.Garlic)
			{
				image = Resources.IMAGE_PAINTING36_0;
				image2 = Resources.IMAGE_PAINTING36_1;
			}
			else if (mSelectedSeed == SeedType.Sunflower)
			{
				image = Resources.IMAGE_PAINTING1_0;
				image2 = Resources.IMAGE_PAINTING1_1;
			}
			else if (mSelectedSeed == SeedType.Cattail)
			{
				image = Resources.IMAGE_PAINTING43_0;
				image2 = Resources.IMAGE_PAINTING43_1;
			}
			else if (mSelectedSeed == SeedType.Agave)
			{
				image = Resources.IMAGE_PAINTING51_0;
				image2 = Resources.IMAGE_PAINTING51_1;
			}
			else if (mSelectedSeed == SeedType.Pumpkinshell)
			{
				image = Resources.IMAGE_PAINTING30_0;
				image2 = Resources.IMAGE_PAINTING30_1;
			}
			else if (mSelectedSeed == SeedType.Hypnoshroom)
			{
				image = Resources.IMAGE_PAINTING_HYPNOSHROOM_0;
				image2 = Resources.IMAGE_PAINTING_HYPNOSHROOM_1;
			}
			else if (mSelectedSeed == SeedType.Imitater)
			{
				image = Resources.IMAGE_PAINTING_IMITATER_0;
				image2 = Resources.IMAGE_PAINTING_IMITATER_1;
			}
			else if (mSelectedSeed == SeedType.Spikerock)
			{
				image = Resources.IMAGE_PAINTING_SPIKEROCK_0;
				image2 = Resources.IMAGE_PAINTING_SPIKEROCK_1;
			}
			else if (mSelectedSeed == SeedType.Peashooter)
			{
				image = Resources.IMAGE_PAINTING_PEASHOOTER_0;
				image2 = Resources.IMAGE_PAINTING_PEASHOOTER_1;
			}
			else if (mSelectedSeed == SeedType.Squash)
			{
				image = Resources.IMAGE_PAINTING_SQUASH_0;
				image2 = Resources.IMAGE_PAINTING_SQUASH_1;
			}
			else if (mSelectedSeed == SeedType.Tallnut)
			{
				image = Resources.IMAGE_PAINTING_TALLNUT_0;
				image2 = Resources.IMAGE_PAINTING_TALLNUT_1;
			}
			else if (mSelectedSeed == SeedType.Blover)
			{
				image = Resources.IMAGE_PAINTING_BLOVER_0;
				image2 = Resources.IMAGE_PAINTING_BLOVER_1;
			}
			else if (mSelectedSeed == SeedType.Puffshroom)
			{
				image = Resources.IMAGE_PAINTING_PUFFSHROOM_0;
				image2 = Resources.IMAGE_PAINTING_PUFFSHROOM_1;
			}
			else if (mSelectedSeed == SeedType.Gatlingpea)
			{
				image = Resources.IMAGE_PAINTING_GATLINGPEA_0;
				image2 = Resources.IMAGE_PAINTING_GATLINGPEA_1;
			}
			else if (mSelectedSeed == SeedType.Fumeshroom)
			{
				image = Resources.IMAGE_PAINTING_FUMESHROOM_0;
				image2 = Resources.IMAGE_PAINTING_FUMESHROOM_1;
			}
			else if (mSelectedSeed == SeedType.Snowpea)
			{
				image = Resources.IMAGE_PAINTING_SNOWPEA_0;
				image2 = Resources.IMAGE_PAINTING_SNOWPEA_1;
			}
			else if (mSelectedSeed == SeedType.Seashroom)
			{
				image = Resources.IMAGE_PAINTING_SEASHROOM_0;
				image2 = Resources.IMAGE_PAINTING_SEASHROOM_1;
			}
			else if (mSelectedSeed == SeedType.Iceshroom)
			{
				image = Resources.IMAGE_PAINTING_ICESHROOM_0;
				image2 = Resources.IMAGE_PAINTING_ICESHROOM_1;
			}
			else if (mSelectedSeed == SeedType.Umbrella)
			{
				image = Resources.IMAGE_PAINTING_UMBRELLA_0;
				image2 = Resources.IMAGE_PAINTING_UMBRELLA_1;
			}
			else if (mSelectedSeed == SeedType.Splitpea)
			{
				image = Resources.IMAGE_PAINTING_SPLITPEA_0;
				image2 = Resources.IMAGE_PAINTING_SPLITPEA_1;
			}
			Widget widget = ((image2 != null) ? new TachieWidget(image, image2) : new TachieWidget(image));
			widget.Resize(0, 0, Constants.BackBufferSize.Y, Constants.BackBufferSize.X);
			AddWidget(widget);
			BringToFront(widget);
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		mCloseButton.Update();
		mIndexButton.Update();
		mShowTachieButton.Update();
		if ((mOpenPage == AlmanacPage.Index && mViewPlantsRect.Contains(new TPoint(x, y))) || mCloseButton.IsMouseOver() || mIndexButton.IsMouseOver())
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		if (mOpenPage == AlmanacPage.Index && mViewZombiesRect.Contains(new TPoint(x, y)))
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
	}

	public virtual void KeyChar(char theChar)
	{
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		ClearPlantsAndZombies();
	}

	public void DrawIndex(Graphics g)
	{
		LawnCommon.DrawImageBox(g, new TRect(-mHalfDeltaWidth, -mHalfDeltaHeight, mWidth, mHeight), AtlasResources.IMAGE_ALMANAC_ROUNDED_OUTLINE);
		g.DrawImage(AtlasResources.IMAGE_ALMANAC_INDEX_HEADER, mWidth / 2 - AtlasResources.IMAGE_ALMANAC_INDEX_HEADER.mWidth / 2 - mHalfDeltaWidth, Constants.AlmanacHeaderY);
		float almanac_Text_Scale = Constants.Almanac_Text_Scale;
		if (mPlant != null)
		{
			Graphics graphics = Graphics.GetNew(g);
			LawnCommon.DrawImageBox(g, mViewPlantsRect, AtlasResources.IMAGE_ALMANAC_CLAY_BORDER);
			graphics.mTransX += mPlant.mX;
			graphics.mTransY += mPlant.mY;
			mPlant.Draw(graphics);
			TodCommon.TodDrawString(g, "[VIEW_PLANTS]", Constants.Almanac_IndexPlantTextPos.X, Constants.Almanac_IndexPlantTextPos.Y, Resources.FONT_DWARVENTODCRAFT15, Constants.YellowFontColour, DrawStringJustification.Center, almanac_Text_Scale);
			graphics.PrepareForReuse();
		}
		if (mZombie != null)
		{
			Graphics graphics2 = Graphics.GetNew(g);
			LawnCommon.DrawImageBox(g, mViewZombiesRect, AtlasResources.IMAGE_ALMANAC_STONE_BORDER);
			graphics2.mTransX += mZombie.mX;
			graphics2.mTransY += mZombie.mY;
			mZombie.Draw(graphics2);
			TodCommon.TodDrawString(g, "[VIEW_ZOMBIES]", Constants.Almanac_IndexZombieTextPos.X, Constants.Almanac_IndexZombieTextPos.Y, Resources.FONT_DWARVENTODCRAFT15, Constants.GreenFontColour, DrawStringJustification.Center, almanac_Text_Scale);
			graphics2.PrepareForReuse();
		}
	}

	public void SetPage(AlmanacPage thePage)
	{
		mOpenPage = thePage;
		ClearPlantsAndZombies();
		if (mOpenPage == AlmanacPage.Index)
		{
			mPlantsScrollWidget.SetVisible(isVisible: false);
			mZombiesScrollWidget.SetVisible(isVisible: false);
			mDescriptionScrollWidget.SetVisible(isVisible: false);
			mPlant = Plant.GetNewPlant();
			mPlant.mBoard = null;
			mPlant.mIsOnBoard = false;
			mPlant.PlantInitialize(0, 0, SeedType.Sunflower, SeedType.None);
			mPlant.mX = Constants.Almanac_IndexPlantPos.X;
			mPlant.mY = Constants.Almanac_IndexPlantPos.Y;
			if (mZombie != null)
			{
				mZombie.PrepareForReuse();
			}
			mZombie = Zombie.GetNewZombie();
			mZombie.mBoard = null;
			mZombie.ZombieInitialize(0, ZombieType.Normal, theVariant: false, null, GameConstants.ZOMBIE_WAVE_UI);
			mZombie.mPosX = Constants.Almanac_IndexZombiePos.X;
			mZombie.mPosY = Constants.Almanac_IndexZombiePos.Y;
			mIndexButton.mBtnNoDraw = true;
			mShowTachieButton.mBtnNoDraw = true;
		}
		else if (mOpenPage == AlmanacPage.Plants)
		{
			SetupPlant();
			mPlantsScrollWidget.SetVisible(isVisible: true);
			mZombiesScrollWidget.SetVisible(isVisible: false);
			mPlantsScrollWidget.ScrollToMin(animated: false);
			mDescriptionScrollWidget.SetVisible(isVisible: true);
			mDescriptionScrollWidget.ScrollToMin(animated: false);
			mIndexButton.mBtnNoDraw = false;
		}
		else if (mOpenPage == AlmanacPage.Zombies)
		{
			SetupZombie();
			mPlantsScrollWidget.SetVisible(isVisible: false);
			mZombiesScrollWidget.SetVisible(isVisible: true);
			mZombiesScrollWidget.ScrollToMin(animated: false);
			mDescriptionScrollWidget.SetVisible(isVisible: true);
			mDescriptionScrollWidget.ScrollToMin(animated: false);
			mIndexButton.mBtnNoDraw = false;
		}
		else
		{
			Debug.ASSERT(value: false);
		}
	}

	public void DrawZombies(Graphics g)
	{
		LawnCommon.DrawImageBox(g, new TRect(-mHalfDeltaWidth, -mHalfDeltaHeight, mWidth, mHeight), AtlasResources.IMAGE_ALMANAC_ROUNDED_OUTLINE);
		g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIES_HEADER, Constants.Almanac_ZombieHeader_Pos.X, Constants.Almanac_ZombieHeader_Pos.Y);
		LawnCommon.DrawImageBox(g, Constants.Almanac_ZombieStoneRect, AtlasResources.IMAGE_ALMANAC_STONE_TABLET);
		int x = Constants.Almanac_BackgroundPosition.X;
		int y = Constants.Almanac_BackgroundPosition.Y;
		if (mZombie.mZombieType == ZombieType.Zamboni || mZombie.mZombieType == ZombieType.Bobsled)
		{
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDICE, x, y);
		}
		else
		{
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDDAY, x, y);
		}
		if (mZombie != null && !ZombieHasSilhouette(mZombie.mZombieType))
		{
			Graphics graphics = Graphics.GetNew(g);
			graphics.mTransX += (int)((float)mZombie.mX * Constants.S);
			graphics.mTransY += (int)((float)mZombie.mY * Constants.S);
			graphics.SetClipRect(ref Constants.Almanac_ZombieClipRect);
			if (mZombie.mZombieType == ZombieType.Zamboni)
			{
				graphics.mTransX += (int)(-30f * Constants.S);
				graphics.mTransY += (int)(5f * Constants.S);
			}
			else if (mZombie.mZombieType == ZombieType.Gargantuar || mZombie.mZombieType == ZombieType.RedeyeGargantuar || mZombie.mZombieType == ZombieType.RobotTitan || mZombie.mZombieType == ZombieType.RedeyeRobotTitan)
			{
				graphics.mTransY += (int)(30f * Constants.S);
			}
			else if (mZombie.mZombieType == ZombieType.Football || mZombie.mZombieType == ZombieType.FootballPremium)
			{
				graphics.mTransX += (int)(-10f * Constants.S);
			}
			else if (mZombie.mZombieType == ZombieType.Balloon)
			{
				graphics.mTransY += (int)(-20f * Constants.S);
			}
			else if (mZombie.mZombieType == ZombieType.Bungee)
			{
				graphics.mTransX += (int)Constants.InvertAndScale(12f);
			}
			else if (mZombie.mZombieType == ZombieType.Catapult)
			{
				graphics.mTransX += (int)(-10f * Constants.S);
			}
			else if (mZombie.mZombieType == ZombieType.Boss)
			{
				graphics.mTransX += (int)(-540f * Constants.S);
				graphics.mTransY += (int)(-175f * Constants.S);
			}
			else if (mZombie.mZombieType == ZombieType.Polevaulter)
			{
				graphics.mTransX += (int)Constants.InvertAndScale(17f);
			}
			if (mZombie.mZombieType != ZombieType.Bungee && mZombie.mZombieType != ZombieType.Boss && mZombie.mZombieType != ZombieType.Zamboni && mZombie.mZombieType != ZombieType.Catapult)
			{
				mZombie.DrawShadow(graphics);
			}
			graphics.HardwareClip();
			mZombie.Draw(graphics);
			graphics.EndHardwareClip();
			graphics.PrepareForReuse();
		}
		LawnCommon.DrawImageBox(g, Constants.Almanac_NavyRect, AtlasResources.IMAGE_ALMANAC_NAVY_RECT, theDrawCenter: false);
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(mSelectedZombie);
		string theText = ((!ZombieHasSilhouette(mSelectedZombie)) ? Common.StrFormat_("[{0}]", zombieDefinition.mZombieName) : TodStringFile.TodStringTranslate("[NOT_ENCOUNTERED_YET_NAME]"));
		TodCommon.TodDrawString(g, theText, Constants.Almanac_NamePosition.X, Constants.Almanac_NamePosition.Y, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(0, 196, 0), Constants.Almanac_ItemName_MaxWidth, DrawStringJustification.Center);
		TRect rect = mDescriptionScrollWidget.GetRect();
		rect.Inflate((int)Constants.InvertAndScale(6f), (int)Constants.InvertAndScale(6f));
		DrawPaper(g, rect, new SexyColor(171, 159, 207));
	}

	public void DrawPlants(Graphics g)
	{
		bool flag = false;
		if (mInFullView || mFullViewCountdown != 0)
		{
			int x = Constants.Almanac_BackgroundPosition.X;
			int y = Constants.Almanac_BackgroundPosition.Y;
			Image theImage;
			if (mSelectedSeed == SeedType.Lilypad || mSelectedSeed == SeedType.Tanglekelp || mSelectedSeed == SeedType.Cattail)
			{
				mApp.DelayLoadBackgroundResource("DelayLoad_Background3");
				theImage = Resources.IMAGE_BACKGROUND3;
				x -= 590;
				y -= 248;
			}
			else if (mSelectedSeed == SeedType.Seashroom)
			{
				mApp.DelayLoadBackgroundResource("DelayLoad_Background4");
				theImage = Resources.IMAGE_BACKGROUND4;
				x -= 590;
				y -= 248;
			}
			else if (Plant.IsNocturnal(mSelectedSeed) || mSelectedSeed == SeedType.Gravebuster || mSelectedSeed == SeedType.Plantern)
			{
				mApp.DelayLoadBackgroundResource("DelayLoad_Background2");
				theImage = Resources.IMAGE_BACKGROUND2;
				x -= 670;
				y -= 165;
			}
			else if (mSelectedSeed == SeedType.Flowerpot)
			{
				mApp.DelayLoadBackgroundResource("DelayLoad_Background5");
				theImage = Resources.IMAGE_BACKGROUND5;
				x -= 740;
				y -= 220;
			}
			else
			{
				mApp.DelayLoadBackgroundResource("DelayLoad_Background1");
				theImage = Resources.IMAGE_BACKGROUND1;
				x -= 670;
				y -= 165;
			}
			float num = 3f;
			float num2 = TodCommon.TodAnimateCurveFloat(200, 0, (mInFullView && mFullViewCountdown != 0) ? (200 - mFullViewCountdown) : mFullViewCountdown, 0f, (float)Constants.Almanac_BackgroundPosition.X * num, TodCurves.EaseInOut);
			float num3 = TodCommon.TodAnimateCurveFloat(200, 0, (mInFullView && mFullViewCountdown != 0) ? (200 - mFullViewCountdown) : mFullViewCountdown, 0f, (float)Constants.Almanac_BackgroundPosition.Y * num, TodCurves.EaseInOut);
			float num4 = TodCommon.TodAnimateCurveFloat(200, 0, (mInFullView && mFullViewCountdown != 0) ? (200 - mFullViewCountdown) : mFullViewCountdown, 1f, num, TodCurves.EaseInOut);
			SexyTransform2D theTransform = default;
			theTransform.Scale(num4, num4);
			theTransform.Translate(0f - num2, 0f - num3);
			g.pushTransform(ref theTransform, concatenate: true);
			g.DrawImage(theImage, x, y);
			if (mPlant != null)
			{
				Graphics graphics = Graphics.GetNew(g);
				graphics.mTransX += mPlant.mX;
				graphics.mTransY += mPlant.mY;
				mPlant.Draw(graphics);
				graphics.PrepareForReuse();
			}
			flag = true;
		}
		if ((!mInFullView && (mFullViewCountdown > 150 || mFullViewCountdown <= 0)) || (mInFullView && mFullViewCountdown < 50 && mFullViewCountdown > 0))
		{
			LawnCommon.DrawImageBox(g, new TRect(-mHalfDeltaWidth, -mHalfDeltaHeight, mWidth, mHeight), AtlasResources.IMAGE_ALMANAC_ROUNDED_OUTLINE);
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_PLANTS_HEADER, Constants.Almanac_PlantsHeader_Pos.X, Constants.Almanac_PlantsHeader_Pos.Y);
			LawnCommon.DrawImageBox(g, Constants.Almanac_ClayRect, AtlasResources.IMAGE_ALMANAC_CLAY_TABLET);
			int x = Constants.Almanac_BackgroundPosition.X;
			int y = Constants.Almanac_BackgroundPosition.Y;
			if (mSelectedSeed == SeedType.Lilypad || mSelectedSeed == SeedType.Tanglekelp || mSelectedSeed == SeedType.Cattail)
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDPOOL, x, y);
			}
			else if (mSelectedSeed == SeedType.Seashroom)
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDNIGHTPOOL, x, y);
			}
			else if (Plant.IsNocturnal(mSelectedSeed) || mSelectedSeed == SeedType.Gravebuster || mSelectedSeed == SeedType.Plantern)
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDNIGHT, x, y);
			}
			else if (mSelectedSeed == SeedType.Flowerpot)
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDROOF, x, y);
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_ALMANAC_GROUNDDAY, x, y);
			}
			if (mPlant != null)
			{
				SexyTransform2D theTransform2 = default;
				theTransform2.Scale(0.8f, 0.8f);
				Graphics graphics2 = Graphics.GetNew(g);
				graphics2.pushTransform(ref theTransform2, concatenate: true);
				graphics2.mTransX += (int)((float)mPlant.mX / 0.8f);
				graphics2.mTransY += (int)((float)mPlant.mY / 0.8f);
				mPlant.Draw(graphics2);
				graphics2.popTransform();
				graphics2.PrepareForReuse();
			}
			LawnCommon.DrawImageBox(g, Constants.Almanac_BrownRect, AtlasResources.IMAGE_ALMANAC_BROWN_RECT, theDrawCenter: false);
			string nameString = Plant.GetNameString(mSelectedSeed, SeedType.None);
			TodCommon.TodDrawString(g, nameString, Constants.Almanac_NamePosition.X, Constants.Almanac_NamePosition.Y, Resources.FONT_DWARVENTODCRAFT15, new SexyColor(213, 159, 43), Constants.Almanac_ItemName_MaxWidth, DrawStringJustification.Center);
		}
		if (flag)
		{
			g.popTransform();
		}
		if (mDescriptionScrollWidget.mVisible)
		{
			TRect rect = mDescriptionScrollWidget.GetRect();
			rect.Inflate((int)Constants.InvertAndScale(6f), (int)Constants.InvertAndScale(6f));
			DrawPaper(g, rect, new SexyColor(253, 186, 117, ((!mInFullView && mFullViewCountdown < 50 && mFullViewCountdown != 0) || (mInFullView && (mFullViewCountdown >= 150 || mFullViewCountdown == 0))) ? 192 : 255));
		}
	}

	public void SetupPlant()
	{
		ClearPlantsAndZombies();
		float num = Constants.Almanac_PlantPosition.X;
		float num2 = Constants.Almanac_PlantPosition.Y;
		if (mSelectedSeed == SeedType.Tallnut)
		{
			num2 += (float)(int)Constants.InvertAndScaleUI(18f);
		}
		if (mSelectedSeed == SeedType.Cobcannon)
		{
			num += (float)(int)Constants.InvertAndScaleUI(-20f);
		}
		if (mSelectedSeed == SeedType.Flowerpot)
		{
			num2 += (float)(int)Constants.InvertAndScaleUI(-20f);
		}
		if (mSelectedSeed == SeedType.InstantCoffee)
		{
			num2 += (float)(int)Constants.InvertAndScaleUI(20f);
		}
		_ = mSelectedSeed;
		_ = 11;
		mPlant = Plant.GetNewPlant();
		mPlant.mBoard = null;
		mPlant.mIsOnBoard = false;
		mPlant.PlantInitialize(0, 0, mSelectedSeed, SeedType.None);
		mPlant.mX = (int)num;
		mPlant.mY = (int)num2;
		PlantDefinition plantDefinition = Plant.GetPlantDefinition(mSelectedSeed);
		string text = "";
		if (TodStringFile.TodStringListExists("[" + plantDefinition.mPlantName + "_DESCRIPTION_HEADER]"))
		{
			text = TodStringFile.TodStringTranslate("[" + plantDefinition.mPlantName + "_DESCRIPTION_HEADER]");
		}
		string text2 = TodStringFile.TodStringTranslate("[" + plantDefinition.mPlantName + "_DESCRIPTION]");
		if (text != string.Empty)
		{
			text2 = "{NORMAL}" + text + "\n" + text2 + "\n";
		}
		string theText = "{KEYWORD}{COST}:{STAT} " + plantDefinition.mSeedCost;
		theText = TodCommon.TodReplaceString(theText, "{COST}", "[COST]");
		string theText2;
		if (plantDefinition.mRefreshTime == 750)
		{
			theText2 = "{KEYWORD}{WAIT_TIME}:{STAT} {WAIT_TIME_LENGTH}";
			theText2 = TodCommon.TodReplaceString(theText2, "{WAIT_TIME_LENGTH}", "[WAIT_TIME_SHORT]");
		}
		else if (plantDefinition.mRefreshTime == 3000)
		{
			theText2 = "{KEYWORD}{WAIT_TIME}:{STAT} {WAIT_TIME_LENGTH}";
			theText2 = TodCommon.TodReplaceString(theText2, "{WAIT_TIME_LENGTH}", "[WAIT_TIME_LONG]");
		}
		else
		{
			theText2 = "{KEYWORD}{WAIT_TIME}:{STAT} {WAIT_TIME_LENGTH}";
			theText2 = TodCommon.TodReplaceString(theText2, "{WAIT_TIME_LENGTH}", "[WAIT_TIME_VERY_LONG]");
		}
		theText2 = TodCommon.TodReplaceString(theText2, "{WAIT_TIME}", "[WAIT_TIME]");
		text2 += "{NORMAL}";
		if (mSelectedSeed != SeedType.Imitater)
		{
			string text3 = text2;
			text2 = text3 + "\n" + theText2 + "\n" + theText;
		}
		mDescriptionWidget.SetText(ref text2);
		mDescriptionScrollWidget.ClientSizeChanged();
		mDescriptionScrollWidget.ScrollToMin(animated: false);
		if (Enumerable.Contains(new SeedType[29]
		{
			SeedType.Jalapeno,
			SeedType.Kernelpult,
			SeedType.Garlic,
			SeedType.Chomper,
			SeedType.Scaredyshroom,
			SeedType.Repeater,
			SeedType.Lilypad,
			SeedType.Sunflower,
			SeedType.Plantern,
			SeedType.Gravebuster,
			SeedType.Cattail,
			SeedType.Spikeweed,
			SeedType.Agave,
			SeedType.Pumpkinshell,
			SeedType.Hypnoshroom,
			SeedType.Imitater,
			SeedType.Spikerock,
			SeedType.Peashooter,
			SeedType.Squash,
			SeedType.Tallnut,
			SeedType.Blover,
			SeedType.Puffshroom,
			SeedType.Gatlingpea,
			SeedType.Fumeshroom,
			SeedType.Snowpea,
			SeedType.Seashroom,
			SeedType.Iceshroom,
			SeedType.Umbrella,
			SeedType.Splitpea
		}, mSelectedSeed))
		{
			mShowTachieButton.mBtnNoDraw = false;
		}
		else
		{
			mShowTachieButton.mBtnNoDraw = true;
		}
	}

	public void ClearPlantsAndZombies()
	{
		if (mPlant != null)
		{
			mPlant.Die();
			mPlant.PrepareForReuse();
			mPlant = null;
		}
		if (mZombie != null)
		{
			mZombie.DieNoLoot(giveAchievements: false);
			mZombie.Dispose();
			mZombie = null;
		}
	}

	public void SetupZombie()
	{
		ClearPlantsAndZombies();
		if (mZombie != null)
		{
			mZombie.PrepareForReuse();
		}
		mZombie = Zombie.GetNewZombie();
		mZombie.mBoard = null;
		mZombie.ZombieInitialize(0, mSelectedZombie, theVariant: false, null, GameConstants.ZOMBIE_WAVE_UI);
		mZombie.mScaleZombie = 1f;
		mZombie.mPosX = Constants.Almanac_ZombiePosition.X;
		mZombie.mPosY = Constants.Almanac_ZombiePosition.Y;
		string text = "";
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(mSelectedZombie);
		string theText;
		if (ZombieHasDescription(mSelectedZombie))
		{
			string theString = Common.StrFormat_("[{0}_DESCRIPTION]", zombieDefinition.mZombieName);
			theText = TodStringFile.TodStringTranslate(theString);
			if (TodStringFile.TodStringListExists(Common.StrFormat_("[{0}_DESCRIPTION_HEADER]", zombieDefinition.mZombieName)))
			{
				text = TodStringFile.TodStringTranslate(Common.StrFormat_("[{0}_DESCRIPTION_HEADER]", zombieDefinition.mZombieName));
			}
		}
		else
		{
			theText = "[NOT_ENCOUNTERED_YET]";
		}
		if (text != string.Empty)
		{
			theText = "{NORMAL}" + text + "\n" + theText + "\n";
		}
		for (int i = 0; i < GameConstants.gLawnStringFormatCount; i++)
		{
			TodStringListFormat todStringListFormat = GameConstants.gLawnStringFormats[i];
			if (TodCommon.TestBit(todStringListFormat.mFormatFlags, 1))
			{
				if (!mApp.HasSeedType(SeedType.Magnetshroom))
				{
					todStringListFormat.mNewColor = todStringListFormat.mBaseColor;
					todStringListFormat.mNewColor.mAlpha = 0;
					todStringListFormat.mLineSpacingOffset = -17;
				}
				else
				{
					todStringListFormat.mNewColor = todStringListFormat.mBaseColor;
					todStringListFormat.mNewColor.mAlpha = 255;
					todStringListFormat.mLineSpacingOffset = 0;
				}
			}
		}
		mDescriptionWidget.SetText(ref theText);
		mDescriptionScrollWidget.ClientSizeChanged();
		mDescriptionScrollWidget.ScrollToMin(animated: false);
	}

	public void ShowZombie(ZombieType theZombieType)
	{
		mSelectedZombie = theZombieType;
		SetPage(AlmanacPage.Zombies);
	}

	public void ShowPlant(SeedType theSeedType)
	{
		mSelectedSeed = theSeedType;
		SetPage(AlmanacPage.Plants);
	}

	public bool ZombieHasDescription(ZombieType theZombieType)
	{
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(theZombieType);
		int level = mApp.mPlayerInfo.GetLevel();
		if (theZombieType == ZombieType.Yeti)
		{
			if (!mApp.CanSpawnYetis())
			{
				return false;
			}
			if (mApp.mPlayerInfo.mFinishedAdventure >= 2)
			{
				return true;
			}
		}
		else if (mApp.HasFinishedAdventure())
		{
			return true;
		}
		if (zombieDefinition.mStartingLevel <= level)
		{
			if (zombieDefinition.mStartingLevel == level)
			{
				return gZombieDefeated[(int)theZombieType];
			}
			return true;
		}
		return false;
	}

	public bool ZombieHasSilhouette(ZombieType theZombieType)
	{
		if (theZombieType != ZombieType.Yeti)
		{
			return false;
		}
		if (mApp.CanSpawnYetis())
		{
			return false;
		}
		if (mApp.HasFinishedAdventure())
		{
			return true;
		}
		ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(theZombieType);
		int level = mApp.mPlayerInfo.GetLevel();
		return level > zombieDefinition.mStartingLevel;
	}

	public void DrawPaper(Graphics g, TRect theRect, SexyColor theColor)
	{
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(theColor);
		LawnCommon.DrawImageBox(g, theRect, AtlasResources.IMAGE_ALMANAC_PAPER);
		g.SetColorizeImages(colorizeImages: false);
	}

	public void PlantSelected(SeedType theSeedType)
	{
		if (mSelectedSeed != theSeedType)
		{
			mSelectedSeed = theSeedType;
			SetupPlant();
			mApp.PlaySample(Resources.SOUND_TAP);
		}
	}

	public void ZombieSelected(ZombieType theZombieType)
	{
		if (mSelectedZombie != theZombieType)
		{
			mSelectedZombie = theZombieType;
			SetupZombie();
			mApp.PlaySample(Resources.SOUND_TAP);
		}
	}
}
