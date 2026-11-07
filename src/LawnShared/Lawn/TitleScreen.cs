using System;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class TitleScreen : Widget, ButtonListener
{
	private const int NUM_TRIGGERS = 5;

	public HyperlinkWidget mStartButton;

	public ButtonWidget mNoAIButton;

	public float mCurBarWidth;

	public float mTotalBarWidth;

	public float mBarVel;

	public float mBarStartProgress;

	public bool mRegisterClicked;

	public bool mLoadingThreadComplete;

	public int mTitleAge;

	public KeyCode mQuickLoadKey;

	public bool mNeedRegister;

	public bool mNeedShowRegisterBox;

	public bool mNeedToInit;

	public float mPrevLoadingPercent;

	public TitleState mTitleState;

	public int mTitleStateCounter;

	public int mTitleStateDuration;

	public bool mLoaderScreenIsLoaded;

	public bool mNeedToUnpackAtlas;

	public int mNextImageIndex;

	public bool mTitleVoicePlayed;

	public LawnApp mApp;

	private static Image[] PreflightImages;

	private float[] aTriggerPoint = new float[5];

	public const int LEFT_LENGTH = 235;

	private int VersionTextRight => mApp.mScreenScales.mVirtualWidth / 2 + Constants.BOARD_WIDTH / 2 + 235 - 10;

	private int VersionTextY => mHeight - 40;

	public void ButtonPress(int theId, int theClickCount)
	{
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

	public TitleScreen(LawnApp theApp)
	{
		mApp = theApp;
		mCurBarWidth = 0f;
		mTotalBarWidth = Constants.InvertAndScale(314f);
		mBarVel = 0.2f;
		mBarStartProgress = 0f;
		mPrevLoadingPercent = 0f;
		mTitleAge = 0;
		mNeedRegister = false;
		mRegisterClicked = false;
		mNeedShowRegisterBox = false;
		mLoadingThreadComplete = false;
		mNeedToInit = true;
		mQuickLoadKey = KeyCode.Unknown;
		mTitleState = TitleState.WaitingForFirstDraw;
		mTitleStateDuration = 0;
		mTitleStateCounter = 0;
		mLoaderScreenIsLoaded = false;
		mNeedToUnpackAtlas = true;
		mStartButton = new HyperlinkWidget(0, this);
		mStartButton.mColor = new SexyColor(218, 184, 33);
		mStartButton.mOverColor = new SexyColor(250, 90, 15);
		mStartButton.mUnderlineSize = 0;
		mStartButton.mDisabled = true;
		mStartButton.mVisible = false;
		mNoAIButton = new ButtonWidget(2, this);
		mNoAIButton.mDoFinger = true;
		mNoAIButton.mDisabled = true;
		mNoAIButton.mVisible = false;
		mNextImageIndex = 0;
		PreflightImages = new Image[22]
		{
			Resources.IMAGE_PLANTSZOMBIES,
			Resources.IMAGE_CHARREDZOMBIES,
			Resources.IMAGE_ALMANACUI,
			Resources.IMAGE_SEEDATLAS,
			Resources.IMAGE_DAVE,
			Resources.IMAGE_CACHED,
			Resources.IMAGE_DIALOG,
			Resources.IMAGE_PARTICLES,
			Resources.IMAGE_CONVEYORBELT_BACKDROP,
			Resources.IMAGE_CONVEYORBELT_BELT,
			Resources.IMAGE_QUICKPLAY,
			Resources.IMAGE_SPEECHBUBBLE,
			Resources.IMAGE_GOODIES,
			Resources.IMAGE_ZOMBIE_NOTE_SMALL,
			Resources.IMAGE_SELECTORSCREEN_ACHIEVEMENTS_TOP_BACKGROUND,
			Resources.IMAGE_SELECTORSCREEN_MOREGAMES_BACKGROUND,
			Resources.IMAGE_SELECTORSCREEN_MAIN_BACKGROUND,
			Resources.IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE,
			Resources.IMAGE_SELECTORSCREEN_ACHIEVEMENTS_HOLE_CHINA,
			Resources.IMAGE_SELECTORSCREEN_QUICKPLAY_BACKGROUND,
			Resources.IMAGE_REANIM_ZOMBIESWON,
			Resources.IMAGE_SCARY_POT
		};
		aTriggerPoint[0] = 0.11f * mTotalBarWidth;
		aTriggerPoint[1] = 0.32f * mTotalBarWidth;
		aTriggerPoint[2] = 0.52f * mTotalBarWidth;
		aTriggerPoint[3] = 0.72f * mTotalBarWidth;
		aTriggerPoint[4] = 0.906f * mTotalBarWidth;
	}

	public override void Dispose()
	{
		if (Main.LOW_MEMORY_DEVICE)
		{
			mApp.mResourceManager.DeleteResources("Init");
			mApp.mResourceManager.DeleteResources("LoaderBar");
		}
		mStartButton.Dispose();
		mNoAIButton.Dispose();
	}

	public override void Update()
	{
		base.Update();
		if (mApp.mShutdown)
		{
			return;
		}
		MarkDirty();
		if (mTitleState == TitleState.WaitingForFirstDraw)
		{
			mApp.mMusic.MusicTitleScreenInit();
			mApp.StartLoadingThread();
			mTitleState = TitleState.PopcapLogo;
			mTitleStateDuration = 200;
			mTitleStateCounter = mTitleStateDuration;
		}
		if (mQuickLoadKey != KeyCode.Unknown && mTitleState != TitleState.Screen)
		{
			mTitleState = TitleState.Screen;
			mTitleStateDuration = 0;
			mTitleStateCounter = 100;
		}
		mTitleAge++;
		if (mTitleStateCounter > 0)
		{
			mTitleStateCounter--;
		}
		if (mTitleState == TitleState.PopcapLogo)
		{
			if (mTitleStateCounter <= 0)
			{
				mTitleState = TitleState.Screen;
				mTitleStateDuration = 100;
				mTitleStateCounter = mTitleStateDuration;
			}
		}
		else
		{
			if (!mLoaderScreenIsLoaded)
			{
				return;
			}
			if (!mTitleVoicePlayed)
			{
				PlantVoice.Play(SeedType.None, SeedType.None, (Random.Shared.Next(3) == 1) ? PlantVoice.VoiceType.Special_Title_Cn : PlantVoice.VoiceType.Special_Title_Jp);
				mTitleVoicePlayed = true;
			}
			if (mNeedToUnpackAtlas)
			{
				AtlasResources.mAtlasResources.UnpackLoadingAtlasImages();
				mNeedToUnpackAtlas = false;
			}
			if (mNoAIButton.mButtonImage == null && AtlasResources.IMAGE_NO_AI != null)
			{
				mNoAIButton.mButtonImage = AtlasResources.IMAGE_NO_AI;
				ResizeNoAIButton();
				mNoAIButton.SetVisible(isVisible: true);
			}
			float num = (float)mApp.GetLoadingThreadProgress();
			if (mNeedToInit)
			{
				mNeedToInit = false;
				mStartButton.mLabel = TodStringFile.TodStringTranslate("[LOADING]");
				mStartButton.SetFont(Resources.FONT_BRIANNETOD16);
				mStartButton.Resize(mWidth / 2 - 235 - AtlasResources.IMAGE_LOADBAR_DIRT.mWidth / 2, (int)(Constants.S * 650f), (int)mTotalBarWidth, (int)Constants.InvertAndScale(18f));
				mStartButton.mVisible = true;
				float num2 = ((!(num > 1E-06f)) ? 3000f : ((float)mTitleAge / num));
				float num3 = num2 * (1f - num);
				num3 = TodCommon.ClampFloat(num3, 100f, 3000f);
				mBarVel = mTotalBarWidth / num3;
				mBarStartProgress = Math.Min(num, 0.9f);
			}
			float num4 = (num - mBarStartProgress) / (1f - mBarStartProgress);
			int theY = ((mTitleStateCounter <= 10) ? TodCommon.TodAnimateCurve(10, 0, mTitleStateCounter, (int)(Constants.S * 532f), (int)(Constants.S * 523f), TodCurves.Bounce) : TodCommon.TodAnimateCurve(60, 10, mTitleStateCounter, (int)(Constants.S * 731f), (int)(Constants.S * 532f), TodCurves.EaseIn));
			mStartButton.Resize(mStartButton.mX, theY, (int)mTotalBarWidth, mStartButton.mHeight);
			if (mTitleStateCounter > 0)
			{
				return;
			}
			mApp.mEffectSystem.Update();
			float num5 = mCurBarWidth;
			mCurBarWidth += mBarVel;
			if (!mLoadingThreadComplete)
			{
				if (mCurBarWidth > mTotalBarWidth * 0.99f)
				{
					mCurBarWidth = mTotalBarWidth * 0.99f;
				}
			}
			else if (mCurBarWidth > mTotalBarWidth)
			{
				if (mApp.mRestoreLocation == RestoreLocation.Board)
				{
					mApp.LoadingCompleted();
				}
				else
				{
					mStartButton.mLabel = TodStringFile.TodStringTranslate("[CLICK_TO_START]");
					mCurBarWidth = mTotalBarWidth;
				}
			}
			if (num4 > mPrevLoadingPercent + 0.01f || mLoadingThreadComplete)
			{
				float num6 = TodCommon.TodAnimateCurveFloatTime(0f, 1f, num4, 0f, mTotalBarWidth, TodCurves.EaseIn);
				float num7 = num6 - mCurBarWidth;
				float num8 = TodCommon.TodAnimateCurveFloatTime(0f, 1f, num4, 0.0001f, 1E-05f, TodCurves.Linear);
				if (mLoadingThreadComplete)
				{
					num8 = 0.0001f;
				}
				mBarVel += num7 * Math.Abs(num7) * num8;
				float num9 = TodCommon.TodAnimateCurveFloatTime(0f, 1f, num4, 0.2f, 0.01f, TodCurves.Linear);
				float num10 = 2f;
				if (mApp.mTodCheatKeys)
				{
					num9 = 0f;
					num10 = 100f;
				}
				if (mBarVel < num9)
				{
					mBarVel = num9;
				}
				else if (mBarVel > num10)
				{
					mBarVel = num10;
				}
				mPrevLoadingPercent = num4;
			}
			if (GameConstants.TESTING_LOAD_BAR && mBarVel > 0.5f)
			{
				mBarVel = 0.5f;
			}
			if (!mLoadingThreadComplete && mApp.mLoadingThreadCompleted)
			{
				LawnApp.PreallocateMemory();
				mLoadingThreadComplete = true;
				mStartButton.SetDisabled(isDisabled: false);
				mStartButton.SetVisible(isVisible: true);
				mNoAIButton.SetDisabled(isDisabled: false);
			}
			for (int i = 0; i < 5; i++)
			{
				if (aTriggerPoint[i] > num5 && aTriggerPoint[i] <= mCurBarWidth)
				{
					ReanimationType theReanimationType = ReanimationType.LoadbarSprout;
					if (i == 4)
					{
						theReanimationType = ReanimationType.LoadbarZombiehead;
					}
					float num11 = (float)Constants.TitleScreen_ReanimStart_X + aTriggerPoint[i] + 235f;
					float num12 = (float)mStartButton.mY - Constants.InvertAndScale(42f);
					Reanimation reanimation = mApp.AddReanimation(num11, num12, 0, theReanimationType, theDoScalePos: false);
					reanimation.mLoopType = ReanimLoopType.PlayOnceAndHold;
					reanimation.mAnimRate = 18f;
					switch (i)
					{
					case 1:
					case 3:
						reanimation.OverrideScale(-1f, 1f);
						break;
					case 2:
						reanimation.SetPosition(num11, num12 - 5f);
						reanimation.OverrideScale(1.1f, 1.2f);
						break;
					case 4:
						reanimation.SetPosition(num11 - Constants.InvertAndScale(20f), num12);
						break;
					}
					if (i == 4)
					{
						mApp.PlaySample(Resources.SOUND_LOADINGBAR_FLOWER);
						mApp.PlaySample(Resources.SOUND_LOADINGBAR_ZOMBIE);
					}
					else
					{
						mApp.PlaySample(Resources.SOUND_LOADINGBAR_FLOWER);
					}
				}
			}
		}
	}

	public override void Draw(Graphics g)
	{
		g.SetLinearBlend(linear: true);
		base.Draw(g);
		if (mTitleState == TitleState.PopcapLogo)
		{
			g.SetColor(SexyColor.Black);
			g.FillRect(0, 0, mWidth, mHeight);
			int num = 50;
			int theAlpha = 255;
			if (mTitleStateCounter < mTitleStateDuration - num)
			{
				theAlpha = TodCommon.TodAnimateCurve(num, 0, mTitleStateCounter, 255, 0, TodCurves.Linear);
			}
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, theAlpha));
			if (Constants.Language != Constants.LanguageIndex.de)
			{
				g.DrawImage(Resources.IMAGE_POPCAP_LOGO, (mWidth - Resources.IMAGE_POPCAP_LOGO.mWidth) / 2, (mHeight - Resources.IMAGE_POPCAP_LOGO.mHeight) / 2);
			}
			else
			{
				g.DrawImage(Resources.IMAGE_POPCAP_LOGO_REGISTERED, (mWidth - Resources.IMAGE_POPCAP_LOGO_REGISTERED.mWidth) / 2, (mHeight - Resources.IMAGE_POPCAP_LOGO_REGISTERED.mHeight) / 2);
			}
			g.SetColorizeImages(colorizeImages: false);
			return;
		}
		if (!mLoaderScreenIsLoaded)
		{
			g.SetColor(SexyColor.Black);
			g.FillRect(0, 0, mWidth, mHeight);
			return;
		}
		PreflightNextImage(g);
		g.DrawImage(Resources.IMAGE_TITLESCREEN, (mWidth - Resources.IMAGE_TITLESCREEN.mWidth) / 2, 0);
		TodCommon.TodDrawString(g, LawnApp.AppVersionNumber, VersionTextRight, VersionTextY, Resources.FONT_BRIANNETOD16, new SexyColor(48, 48, 48, 192), DrawStringJustification.Right);
		if (mNeedToInit)
		{
			return;
		}
		g.DrawImage(theY: (mTitleStateCounter <= 60) ? TodCommon.TodAnimateCurve(60, 50, mTitleStateCounter, (int)Constants.InvertAndScale(10f), (int)Constants.InvertAndScale(15f), TodCurves.Bounce) : TodCommon.TodAnimateCurve(100, 60, mTitleStateCounter, (int)Constants.InvertAndScale(-150f), (int)Constants.InvertAndScale(10f), TodCurves.EaseIn), theImage: Resources.IMAGE_PVZ_LOGO, theX: mWidth / 2 - Resources.IMAGE_PVZ_LOGO.mWidth / 2);
		int num2 = mStartButton.mX + 235;
		int num3 = mStartButton.mY - (int)Constants.InvertAndScale(34f);
		g.DrawImage(AtlasResources.IMAGE_LOADBAR_DIRT, num2, (float)num3 + Constants.InvertAndScale(18f));
		if (mCurBarWidth >= mTotalBarWidth)
		{
			g.DrawImage(AtlasResources.IMAGE_LOADBAR_GRASS, num2, num3);
			if (mLoadingThreadComplete)
			{
				DrawToPreload(g);
			}
		}
		else
		{
			Graphics graphics = Graphics.GetNew(g);
			graphics.ClipRect(num2, num3, (int)mCurBarWidth, AtlasResources.IMAGE_LOADBAR_GRASS.mHeight);
			graphics.DrawImage(AtlasResources.IMAGE_LOADBAR_GRASS, num2, num3);
			float num4 = mCurBarWidth * 0.94f;
			float rad = (0f - num4) / 180f * (float)Math.PI * 2f;
			float num5 = TodCommon.TodAnimateCurveFloatTime(0f, mTotalBarWidth, mCurBarWidth, 1f, 0.5f, TodCurves.Linear);
			SexyTransform2D sexyTransform2D = default;
			TodCommon.TodScaleRotateTransformMatrix(ref sexyTransform2D.mMatrix, (float)num2 + Constants.InvertAndScale(11f) + num4 - 235f, (float)num3 - Constants.InvertAndScale(3f) - Constants.InvertAndScale(35f) * num5 + Constants.InvertAndScale(35f), rad, num5, num5);
			TodCommon.TodBltMatrix(theSrcRect: new TRect(0, 0, AtlasResources.IMAGE_REANIM_LOAD_SODROLLCAP.mWidth, AtlasResources.IMAGE_REANIM_LOAD_SODROLLCAP.mHeight), g: g, theImage: AtlasResources.IMAGE_REANIM_LOAD_SODROLLCAP, theTransform: sexyTransform2D.mMatrix, theClipRect: ref g.mClipRect, theColor: SexyColor.White, theDrawMode: g.mDrawMode);
			graphics.PrepareForReuse();
		}
		foreach (Reanimation mReanimation in mApp.mEffectSystem.mReanimationHolder.mReanimations)
		{
			mReanimation.Draw(g);
		}
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		ResizeNoAIButton();
	}

	private void ResizeNoAIButton()
	{
		Image mButtonImage = mNoAIButton.mButtonImage;
		if (mButtonImage != null)
		{
			mNoAIButton.Resize(VersionTextRight - mButtonImage.mWidth, VersionTextY - 10 - mButtonImage.mHeight, mButtonImage.mWidth, mButtonImage.mHeight);
		}
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		theWidgetManager.AddWidget(mStartButton);
		AddWidget(mNoAIButton);
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		theWidgetManager.RemoveWidget(mStartButton);
		RemoveWidget(mNoAIButton);
	}

	public virtual void ButtonPress(int theId)
	{
		mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
	}

	public override bool BackButtonPress()
	{
		mApp.AppExit();
		return true;
	}

	public virtual void ButtonDepress(int theId)
	{
		switch ((TitleScreens)theId)
		{
		case TitleScreens.TitleScreen_Start:
			mApp.LoadingCompleted();
			break;
		case TitleScreens.TitleScreen_Register:
			mRegisterClicked = true;
			break;
		case TitleScreens.TitleScreen_NoAI:
			if (mLoadingThreadComplete)
			{
				mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
				LawnDialog lawnDialog = mApp.DoDialog(69, isModal: true, "[NO_AI_HEADER]", "[NO_AI_MESSAGE]", "[DIALOG_BUTTON_OK]", 3);
				lawnDialog.CalcSize(0, 0, (int)Constants.InvertAndScale(400f));
				LawnApp.CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
				mApp.mWidgetManager.SetFocus(lawnDialog);
			}
			break;
		}
	}

	public override void MouseDown(int x, int y, int theClickCount)
	{
		if (mLoadingThreadComplete)
		{
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
			mApp.LoadingCompleted();
		}
	}

	public override void KeyDown(KeyCode theKey)
	{
		if (mLoadingThreadComplete)
		{
			mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
			mApp.LoadingCompleted();
		}
		else if (mApp.mTodCheatKeys && mApp.mPlayerInfo != null)
		{
			mQuickLoadKey = theKey;
		}
	}

	public void SetRegistered()
	{
	}

	public void DrawToPreload(Graphics g)
	{
	}

	public void PreflightNextImage(Graphics g)
	{
		if (mNextImageIndex < PreflightImages.Length && PreflightImages[mNextImageIndex] != null)
		{
			g.DrawImage(PreflightImages[mNextImageIndex], 0, 0, new TRect(0, 0, 1, 1));
			mNextImageIndex++;
		}
	}
}
