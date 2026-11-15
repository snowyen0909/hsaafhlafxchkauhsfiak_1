using System;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

internal class NewOptionsDialog : LawnDialog, SliderListener, CheckboxListener
{
	public Slider mMusicVolumeSlider;

	public Slider mSfxVolumeSlider;

	public Checkbox mVibrateCheckbox;

	public Checkbox mRunWhileLocked;

	public Checkbox mEnableCheat;

	public Checkbox m500pModeCheckbox;

	public Checkbox mShowName;

	public string mVersion = string.Empty;

	public int mVersionY;

	public LawnStoneButton mAlmanacButton;

	public LawnStoneButton mBackToMainButton;

	public LawnStoneButton mRestartButton;

	public LawnStoneButton mBackToGameButton;

	public HyperlinkWidget mLinkCredits;

	public LawnStoneButton mAboutButton;

	public LawnStoneButton mHelpButton;

	public LawnStoneButton mUpdateButton;

	public bool mFromGameSelector;

	public bool mMusicSliderOn;

	public NewOptionsDialog(LawnApp theApp, bool theFromGameSelector)
		: base(theApp, null, 2, isModal: true, "[OPTIONS_DIALOG_TITLE]", "", "", 0)
	{
		mApp = theApp;
		mFromGameSelector = theFromGameSelector;
		mMusicSliderOn = mApp.mMusicEnabled;
		SetColor(3, new SexyColor(255, 255, 100));
		mAlmanacButton = GameButton.MakeButton(0, this, "[VIEW_ALMANAC_BUTTON]");
		mRestartButton = GameButton.MakeButton(2, this, "[RESTART_LEVEL_BUTTON]");
		mBackToMainButton = GameButton.MakeButton(1, this, "[MAIN_MENU_BUTTON]");
		mBackToGameButton = GameButton.MakeButton(1000, this, "[BACK_TO_GAME]");
		mHelpButton = GameButton.MakeButton(6, this, "[HELP]");
		mAboutButton = GameButton.MakeButton(7, this, "[ABOUT]");
		mUpdateButton = GameButton.MakeButton(11, this, "更新公告");
		mMusicVolumeSlider = new Slider(AtlasResources.IMAGE_OPTIONS_SLIDERSLOT, AtlasResources.IMAGE_OPTIONS_SLIDERKNOB2, 4, this);
		double musicVolume = theApp.GetMusicVolume();
		musicVolume = Math.Max(0.0, Math.Min(1.0, musicVolume));
		mMusicVolumeSlider.SetValue(musicVolume);
		mSfxVolumeSlider = new Slider(AtlasResources.IMAGE_OPTIONS_SLIDERSLOT, AtlasResources.IMAGE_OPTIONS_SLIDERKNOB2, 5, this);
		mSfxVolumeSlider.SetValue(theApp.GetSfxVolume());
		mVibrateCheckbox = LawnCommon.MakeNewCheckbox(8, this, mApp.mPlayerInfo.mDoVibration);
		mRunWhileLocked = LawnCommon.MakeNewCheckbox(11, this, mApp.mPlayerInfo.mRunWhileLocked);
		mEnableCheat = LawnCommon.MakeNewCheckbox(12, this, mApp.mCheatEnabled);
		m500pModeCheckbox = LawnCommon.MakeNewCheckbox(13, this, mApp.mPlayerInfo.m500pMode);
		mShowName = LawnCommon.MakeNewCheckbox(15, this, mApp.mPlayerInfo.mShowName);
		mLinkCredits = new HyperlinkWidget(10, this);
		mLinkCredits.SetFont(Resources.FONT_BRIANNETOD12);
		mLinkCredits.mColor = new SexyColor(255, 255, 136);
		mLinkCredits.mOverColor = new SexyColor(0, 0, 255);
		mLinkCredits.mDoFinger = true;
		mLinkCredits.mLabel = TodStringFile.TodStringTranslate("[OPTIONS_CREDITS_LINK]");
		mLinkCredits.mUnderlineSize = 1;
		mLinkCredits.mUnderlineOffset = 1;
		if (mFromGameSelector)
		{
			mRestartButton.SetVisible(isVisible: false);
			mBackToMainButton.SetVisible(isVisible: false);
			mBackToGameButton.SetLabel("[DIALOG_BUTTON_OK]");
		}
		else
		{
			mTallBottom = mApp.mGameScene != GameScenes.LevelIntro;
			mLinkCredits.SetVisible(isVisible: false);
		}
		if (mApp.mGameMode == GameMode.ChallengeIce || mApp.mGameMode == GameMode.ChallengeZenGarden || mApp.mGameMode == GameMode.TreeOfWisdom)
		{
			mRestartButton.SetVisible(isVisible: false);
		}
		if (mApp.mGameScene == GameScenes.LevelIntro && !mApp.mBoard.mCutScene.IsSurvivalRepick())
		{
			mRestartButton.SetVisible(isVisible: false);
		}
		if (!mApp.CanShowAlmanac() || mApp.mGameScene == GameScenes.LevelIntro || mFromGameSelector)
		{
			mAlmanacButton.SetVisible(isVisible: false);
		}
		CalcSize(0, 0);
	}

	public override void Dispose()
	{
		mHelpButton.Dispose();
		mAboutButton.Dispose();
		mUpdateButton.Dispose();
		mRunWhileLocked.Dispose();
		mMusicVolumeSlider.Dispose();
		mSfxVolumeSlider.Dispose();
		mVibrateCheckbox.Dispose();
		m500pModeCheckbox.Dispose();
		mShowName.Dispose();
		mAlmanacButton.Dispose();
		mRestartButton.Dispose();
		mBackToMainButton.Dispose();
		mBackToGameButton.Dispose();
		base.Dispose();
	}

	public override int GetPreferredHeight(int theWidth)
	{
		return (int)Constants.InvertAndScaleUI((mFromGameSelector || mApp.mGameScene == GameScenes.LevelIntro) ? 310 : 340);
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		AddWidget(mAlmanacButton);
		AddWidget(mRestartButton);
		AddWidget(mBackToMainButton);
		AddWidget(mMusicVolumeSlider);
		AddWidget(mSfxVolumeSlider);
		AddWidget(mVibrateCheckbox);
		AddWidget(mEnableCheat);
		AddWidget(mBackToGameButton);
		AddWidget(mLinkCredits);
		if (mFromGameSelector)
		{
			AddWidget(mAboutButton);
			AddWidget(mHelpButton);
			AddWidget(mUpdateButton);
			AddWidget(m500pModeCheckbox);
		}
		AddWidget(mShowName);
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		RemoveWidget(mAlmanacButton);
		RemoveWidget(mMusicVolumeSlider);
		RemoveWidget(mSfxVolumeSlider);
		RemoveWidget(mVibrateCheckbox);
		RemoveWidget(mEnableCheat);
		RemoveWidget(mBackToMainButton);
		RemoveWidget(mBackToGameButton);
		RemoveWidget(mRestartButton);
		RemoveWidget(mLinkCredits);
		if (mFromGameSelector)
		{
			RemoveWidget(mAboutButton);
			RemoveWidget(mHelpButton);
			RemoveWidget(mUpdateButton);
			RemoveWidget(m500pModeCheckbox);
		}
		RemoveWidget(mShowName);
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		mX = mApp.mWidth / 2 - mWidth / 2;
		mY = mApp.mHeight / 2 - mHeight / 2;
		int theHeight2 = AtlasResources.IMAGE_BUTTON_LEFT.mHeight;
		int num = (int)Constants.InvertAndScaleUI(244f);
		mMusicVolumeSlider.Resize(num, (int)Constants.InvertAndScaleUI(66f), (int)Constants.InvertAndScaleUI(135f), (int)Constants.InvertAndScaleUI(40f));
		mSfxVolumeSlider.Resize(num, (int)Constants.InvertAndScaleUI(93f), (int)Constants.InvertAndScaleUI(135f), (int)Constants.InvertAndScaleUI(40f));
		mVibrateCheckbox.Resize(num, (int)Constants.InvertAndScaleUI(125f), (int)Constants.InvertAndScaleUI(46f), (int)Constants.InvertAndScaleUI(45f));
		mEnableCheat.Resize(num, (int)Constants.InvertAndScaleUI(195f), (int)Constants.InvertAndScaleUI(46f), (int)Constants.InvertAndScaleUI(45f));
		mMusicVolumeSlider.mY += (int)Constants.InvertAndScaleUI(5f);
		mSfxVolumeSlider.mY += (int)Constants.InvertAndScaleUI(15f);
		mVibrateCheckbox.mY += (int)Constants.InvertAndScaleUI(25f);
		mShowName.Resize(num + 250, (int)Constants.InvertAndScaleUI(195f), (int)Constants.InvertAndScaleUI(46f), (int)Constants.InvertAndScaleUI(45f));
		if (mFromGameSelector)
		{
			int num2 = (int)Constants.InvertAndScaleUI(100f);
			mBackToGameButton.Resize(mWidth * 4 / 5 - num2 / 2, theHeight - (int)Constants.InvertAndScaleUI(50f), num2, theHeight2);
			mHelpButton.Resize(mWidth / 2 - num2 / 2, theHeight - (int)Constants.InvertAndScaleUI(50f), num2, theHeight2);
			mAboutButton.Resize(mWidth / 5 - num2 / 2, theHeight - (int)Constants.InvertAndScaleUI(50f), num2, theHeight2);
			mUpdateButton.Resize(mWidth / 5 - num2 / 2, theHeight - (int)Constants.InvertAndScaleUI(90f), num2, theHeight2);
			m500pModeCheckbox.Resize(num + 250, (int)Constants.InvertAndScaleUI(125f), (int)Constants.InvertAndScaleUI(46f), (int)Constants.InvertAndScaleUI(45f));
			m500pModeCheckbox.mY += (int)Constants.InvertAndScaleUI(25f);
			mLinkCredits.Resize(theWidth - (int)Constants.InvertAndScaleUI(175f), theHeight - (int)Constants.InvertAndScaleUI(90f), (int)Constants.InvertAndScaleUI(190f), mLinkCredits.mFont.GetHeight() + (int)Constants.InvertAndScaleUI(8f));
			mVersionY = Constants.NewOptionsDialog_Version_Low_Y;
			return;
		}
		if (mApp.mGameScene == GameScenes.LevelIntro)
		{
			int num3 = (int)Constants.InvertAndScaleUI(150f);
			mBackToGameButton.Resize(mWidth / 4 - num3 / 2, theHeight - (int)Constants.InvertAndScaleUI(50f), num3, theHeight2);
			mBackToMainButton.Resize(mWidth * 3 / 4 - num3 / 2, theHeight - (int)Constants.InvertAndScaleUI(50f), num3, theHeight2);
			mVersionY = Constants.NewOptionsDialog_Version_Low_Y;
			return;
		}
		int num4 = (int)Constants.InvertAndScaleUI(160f);
		int theY2 = theHeight - (int)Constants.InvertAndScaleUI(79f);
		int theY3 = theHeight - (int)Constants.InvertAndScaleUI(46f);
		int theX2 = mWidth / 4 - num4 / 2;
		int theX3 = mWidth * 3 / 4 - num4 / 2 - (int)Constants.InvertAndScaleUI(4f);
		if (mApp.CanShowAlmanac())
		{
			mBackToGameButton.Resize(theX2, theY2, num4, theHeight2);
			mRestartButton.Resize(theX3, theY2, num4, theHeight2);
			mBackToMainButton.Resize(theX2, theY3, num4, theHeight2);
			mAlmanacButton.Resize(theX3, theY3, num4, theHeight2);
		}
		else
		{
			mBackToGameButton.Resize(theX2, theY2, num4, theHeight2);
			mRestartButton.Resize(theX3, theY2, num4, theHeight2);
			mBackToMainButton.Resize(mWidth / 2 - num4 / 2 - (int)Constants.InvertAndScaleUI(4f), theY3, num4, theHeight2);
		}
		mVersionY = Constants.NewOptionsDialog_Version_High_Y;
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		int newOptionsDialog_Music_Offset = Constants.NewOptionsDialog_Music_Offset;
		int newOptionsDialog_FX_Offset = Constants.NewOptionsDialog_FX_Offset;
		int newOptionsDialog_FullScreenOffset = Constants.NewOptionsDialog_FullScreenOffset;
		SexyColor theColor = new SexyColor(107, 109, 145);
		int num = (mMusicSliderOn ? Constants.NewOptionsDialog_MusicLabel_On_Y : Constants.NewOptionsDialog_MusicLabel_Off_Y);
		TodCommon.TodDrawString(g, mMusicSliderOn ? "[OPTIONS_MUSIC_VOLUME]" : "[OPTIONS_MUSIC_OFF]", Constants.NewOptionsDialog_MusicLabel_X, num + newOptionsDialog_Music_Offset, Resources.FONT_DWARVENTODCRAFT18, theColor, Constants.NewOptionsDialog_VibrationLabel_MaxWidth, DrawStringJustification.Right);
		TodCommon.TodDrawString(g, "[OPTIONS_SOUND_FX]", Constants.NewOptionsDialog_FXLabel_X, Constants.NewOptionsDialog_FXLabel_Y + newOptionsDialog_FX_Offset, Resources.FONT_DWARVENTODCRAFT18, theColor, Constants.NewOptionsDialog_VibrationLabel_MaxWidth, DrawStringJustification.Right);
		TodCommon.TodDrawString(g, "[OPTIONS_VABRATION]", Constants.NewOptionsDialog_VibrationLabel_X, Constants.NewOptionsDialog_VibrationLabel_Y + newOptionsDialog_FullScreenOffset, Resources.FONT_DWARVENTODCRAFT18, theColor, Constants.NewOptionsDialog_VibrationLabel_MaxWidth, DrawStringJustification.Right);
		TodCommon.TodDrawString(g, "[OPTIONS_ADDON_CHEAT]", Constants.NewOptionsDialog_VibrationLabel_X, Constants.NewOptionsDialog_LockedLabel_Y + newOptionsDialog_FullScreenOffset, Resources.FONT_DWARVENTODCRAFT18, theColor, Constants.NewOptionsDialog_VibrationLabel_MaxWidth, DrawStringJustification.Right);
		if (mFromGameSelector)
		{
			TodCommon.TodDrawString(g, "[OPTIONS_500P_MODE]", Constants.NewOptionsDialog_VibrationLabel_X + 250, Constants.NewOptionsDialog_VibrationLabel_Y + newOptionsDialog_FullScreenOffset, Resources.FONT_DWARVENTODCRAFT18, theColor, Constants.NewOptionsDialog_VibrationLabel_MaxWidth, DrawStringJustification.Right);
		}
		TodCommon.TodDrawString(g, "[OPTIONS_SHOW_NAME]", Constants.NewOptionsDialog_VibrationLabel_X + 250, Constants.NewOptionsDialog_LockedLabel_Y + newOptionsDialog_FullScreenOffset, Resources.FONT_DWARVENTODCRAFT18, theColor, Constants.NewOptionsDialog_VibrationLabel_MaxWidth, DrawStringJustification.Right);
		TodCommon.TodDrawString(g, LawnApp.AppVersionNumber, mWidth / 2, mVersionY, Resources.FONT_PICO129, theColor, DrawStringJustification.Center);
	}

	public virtual void SliderVal(int theId, double theVal)
	{
		switch (theId)
		{
		case 4:
			if (theVal > GameConstants.MUSIC_SLIDER_THRESHOLD && !mApp.mMusicEnabled)
			{
				mApp.EnableMusic(enable: true);
			}
			else if (theVal < GameConstants.MUSIC_SLIDER_THRESHOLD && mApp.mMusicEnabled)
			{
				mApp.EnableMusic(enable: false);
			}
			mMusicSliderOn = mApp.mMusicEnabled;
			mApp.SetMusicVolume(theVal);
			mApp.mSoundSystem.RehookupSoundWithMusicVolume();
			break;
		case 5:
			mApp.SetSfxVolume(theVal);
			mApp.mSoundSystem.RehookupSoundWithMusicVolume();
			if (!mSfxVolumeSlider.mDragging)
			{
				mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
			}
			break;
		}
	}

	public override void CheckboxChecked(int theId, bool check)
	{
		mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
		switch (theId)
		{
		case 8:
			mApp.mPlayerInfo.mDoVibration = check;
			break;
		case 11:
		{
			SetRunWhenLocked(check);
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty = TodStringFile.TodStringTranslate("[WARNING]");
			empty2 = TodStringFile.TodStringTranslate("[OPTIONS_RUN_LOCKED_MSG]");
			LawnDialog lawnDialog = mApp.DoDialog(53, isModal: true, empty, empty2, "", 3);
			lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[DIALOG_BUTTON_OK]");
			break;
		}
		case 12:
			mApp.mCheatEnabled = check;
			if (check)
			{
				ZBCheatDialog zBCheatDialog = new ZBCheatDialog(mApp, mFromGameSelector);
				zBCheatDialog.Resize(0, 0, 290, 450);
				LawnApp.CenterDialog(zBCheatDialog, zBCheatDialog.mWidth, zBCheatDialog.mHeight);
				mApp.mWidgetManager.AddWidget(zBCheatDialog);
				mApp.mWidgetManager.BringToFront(zBCheatDialog);
				mApp.mWidgetManager.SetFocus(mApp.mBoard);
				zBCheatDialog.ButtonDepress(1000);
				zBCheatDialog.Move(0, (int)((double)Constants.BackBufferSize.Y * 0.2));
				mApp.mCheatHoverDialog = zBCheatDialog;
			}
			else
			{
				mApp.mWidgetManager.RemoveWidget(mApp.mCheatHoverDialog);
				mApp.mCheatHoverDialog.Dispose();
			}
			break;
		case 13:
			mApp.Apply500pMode(check);
			if (OnlineLevel.IsInitialized)
			{
				OnlineLevel.ReloadAsync();
			}
			break;
		case 14:
			mApp.ToggleFullscreen();
			break;
		case 15:
			mApp.mPlayerInfo.mShowName = check;
			break;
		case 9:
		case 10:
			break;
		}
	}

	public override void ButtonPress(int theId)
	{
		mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
	}

	public override bool BackButtonPress()
	{
		int theId = mBackToGameButton.mId;
		ButtonPress(theId);
		ButtonDepress(theId);
		return true;
	}

	public override void ButtonDepress(int theId)
	{
		base.ButtonDepress(theId);
		switch (theId)
		{
		case 0:
			mApp.DoAlmanacDialog(SeedType.None, ZombieType.Invalid, mApp.mBoard);
			break;
		case 1:
			if (mApp.mBoard != null && mApp.mBoard.NeedSaveGame())
			{
				mApp.DoConfirmBackToMain();
				break;
			}
			if (mApp.mBoard != null && mApp.mBoard.mCutScene != null && mApp.mBoard.mCutScene.IsSurvivalRepick())
			{
				mApp.DoConfirmBackToMain();
				break;
			}
			mApp.mBoardResult = BoardResult.Quit;
			mApp.DoBackToMain();
			break;
		case 2:
			if (mApp.mBoard != null)
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (mApp.IsPuzzleMode())
				{
					empty = "[RESTART_PUZZLE_HEADER]";
					empty2 = "[RESTART_PUZZLE_BODY]";
				}
				else if (mApp.IsChallengeMode())
				{
					empty = "[RESTART_CHALLENGE_HEADER]";
					empty2 = "[RESTART_CHALLENGE_BODY]";
				}
				else if (mApp.IsSurvivalMode())
				{
					empty = "[RESTART_SURVIVAL_HEADER]";
					empty2 = "[RESTART_SURVIVAL_BODY]";
				}
				else
				{
					empty = "[RESTART_LEVEL_HEADER]";
					empty2 = "[RESTART_LEVEL_BODY]";
				}
				LawnDialog lawnDialog = mApp.DoDialog(23, isModal: true, empty, empty2, "", 1);
				lawnDialog.mLawnYesButton.mLabel = TodStringFile.TodStringTranslate("[RESTART_LABEL]");
				lawnDialog.mLawnNoButton.mLabel = TodStringFile.TodStringTranslate("[DIALOG_BUTTON_CANCEL]");
				lawnDialog.mSpaceAfterHeader = (int)Constants.InvertAndScaleUI(20f);
				lawnDialog.mMinWidth = (int)Constants.InvertAndScaleUI(350f);
				lawnDialog.CalcSize(0, 0);
				LawnApp.CenterDialog(lawnDialog, lawnDialog.mWidth, lawnDialog.mHeight);
			}
			break;
		case 3:
			mApp.CheckForUpdates();
			break;
		case 6:
			mApp.KillNewOptionsDialog();
			mApp.KillGameSelector();
			mApp.ShowAwardScreen(AwardType.HelpZombieNote, theShowAchievements: false);
			break;
		case 7:
			mApp.DoAboutDialog();
			break;
		case 10:
			mApp.KillNewOptionsDialog();
			mApp.KillGameSelector();
			mApp.ShowCreditScreen();
			break;
		case 11:
			mApp.DoUpdateDialog();
			break;
		case 1000:
			mApp.WriteCurrentUserConfig();
			break;
		}
	}

	public override void KeyDown(KeyCode theKey)
	{
		if (mApp.mBoard != null)
		{
			mApp.mBoard.DoTypingCheck(theKey);
		}
		switch (theKey)
		{
		case KeyCode.Return:
		case KeyCode.Space:
			base.ButtonDepress(1000);
			break;
		case KeyCode.Escape:
			base.ButtonDepress(1001);
			break;
		}
	}

	public override void Update()
	{
		base.Update();
		if (mMusicSliderOn && (!mApp.mMusicEnabled || mApp.GetMusicVolume() == 0.0))
		{
			mMusicSliderOn = false;
			mMusicVolumeSlider.SetValue(0.0);
		}
	}

	private void SetRunWhenLocked(bool value)
	{
		Main.RunWhenLocked = value;
		mApp.mPlayerInfo.mRunWhileLocked = value;
	}
}
