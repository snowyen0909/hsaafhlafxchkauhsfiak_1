using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

internal class ZBCheatDialog : LawnDialog, SliderListener, CheckboxListener
{
	public readonly TPoint ZBCDIALOG_MINIMIZED_SIZE = new TPoint(45, 50);

	private TRect? beforeDrag;

	private TPoint sizeBeforeMinimalized;

	private TPoint? posLastMinimalized;

	private readonly LawnStoneButton mGridFocusButton;

	private Checkbox mCoinAutoCollectCheckbox;

	private Checkbox mRunWhileLockedCheckbox;

	public string mVersion = string.Empty;

	public int mVersionY;

	public Checkbox mEasyPlantingCheckbox;

	public LawnStoneButton mSunButton;

	public LawnStoneButton mBackToGameButton;

	public LawnStoneButton mPassLevelButton;

	public LawnStoneButton mTriggerMowersButton;

	public Checkbox mFastMoCheckbox;

	public LawnStoneButton mEnableVirtualKeyboardButton;

	private Checkbox mCheatKeyCheckBox;

	public LawnStoneButton mEnableSkipLevelButton;

	public LawnStoneButton mSkipLevelButton;

	private static bool _enableSkipLevel = false;

	private static bool _canEnableSkipLevel = true;

	public LawnStoneButton mPlusButton;

	public LawnStoneButton mMinusButton;

	private bool _isMinimalized;

	public StackPanel mPanel;

	public ScrollWidget mScroll;

	public ScrollbarWidget mVerticalScrollBar;

	private static bool _enableCoinAutoCollect = false;

	private static bool _advancedPause = false;

	private static bool _zombieSmallHead = false;

	private static readonly Queue<Image> _zombieSmallHeadOriginalImages = new Queue<Image>(10);

	private static bool _enableNoZombieInHouse = false;

	private static bool _enableNoCoolDownPlanting = false;

	private static bool _showZombieHealth = false;

	public WeakReference<Label> mGameSpeedLabel;

	private static readonly int[] MultiplierSteps = new int[6] { 1, 2, 3, 5, 10, 50 };

	/// <summary>无限阳光：种植 / 升级不扣阳光（合并了原来的“阳光加满”）。</summary>
	public static bool NoSunCost;

	/// <summary>收集阳光时的倍率。</summary>
	public static int SunMultiplier = 1;

	private LawnStoneButton mSunMultiplierButton;

	private static int NextMultiplier(int theCurrent)
	{
		int i = Array.IndexOf(MultiplierSteps, theCurrent);
		return MultiplierSteps[(i + 1) % MultiplierSteps.Length];
	}

	public static bool EnableSkipLevel
	{
		get
		{
			return _enableSkipLevel;
		}
		set
		{
			_enableSkipLevel = value;
			EnableSkipLevelChanged?.Invoke(value);
		}
	}

	public static bool CanEnableSkipLevel
	{
		get
		{
			return _canEnableSkipLevel;
		}
		set
		{
			_canEnableSkipLevel = value;
			if (!value)
			{
				EnableSkipLevel = false;
			}
			CanEnableSkipLevelChanged?.Invoke(value);
		}
	}

	public static bool EnableCoinAutoCollect
	{
		get
		{
			return _enableCoinAutoCollect;
		}
		set
		{
			if (value != _enableCoinAutoCollect)
			{
				_enableCoinAutoCollect = value;
			}
		}
	}

	public static bool AdvancedPause
	{
		get
		{
			return _advancedPause;
		}
		set
		{
			if (value != _advancedPause)
			{
				_advancedPause = value;
			}
		}
	}

	public static bool ZombieSmallHead
	{
		get
		{
			return _zombieSmallHead;
		}
		set
		{
			if (value != _zombieSmallHead)
			{
				throw new NotImplementedException();
			}
			_zombieSmallHead = value;
		}
	}

	public static bool EnableNoZombieInHouse
	{
		get
		{
			return _enableNoZombieInHouse;
		}
		set
		{
			if (value != _enableNoZombieInHouse)
			{
				_enableNoZombieInHouse = value;
			}
		}
	}

	public static bool EnableNoCoolDownPlanting
	{
		get
		{
			return _enableNoCoolDownPlanting;
		}
		set
		{
			if (value != _enableNoCoolDownPlanting)
			{
				_enableNoCoolDownPlanting = value;
			}
		}
	}

	public static bool ShowZombieHealth
	{
		get
		{
			return _showZombieHealth;
		}
		set
		{
			if (value != _showZombieHealth)
			{
				_showZombieHealth = value;
			}
		}
	}

	public static event Action<bool> EnableSkipLevelChanged = null;

	public static event Action<bool> CanEnableSkipLevelChanged = null;

	public ZBCheatDialog(LawnApp theApp, bool theFromGameSelector)
		: base(theApp, null, 114514, isModal: true, "作弊菜单", "", "", 0)
	{
		mApp = theApp;
		SetColor(3, new SexyColor(255, 255, 100));
		mBackToGameButton = GameButton.MakeButton(1000, this, "[BACK_TO_GAME]");
		mBackToGameButton.SetLabel("最小化");
		mPlusButton = GameButton.MakeButton(1001, this, "+");
		mMinusButton = GameButton.MakeButton(1002, this, "-");
		mScroll = new ScrollWidget();
		mPanel = new StackPanel(isVertical: true);
		int num = 212;
		int labelSize = num - 60;
		mPanel.AddWidget(GenerateCheckboxWithText("自动收集资源", 5, EnableCoinAutoCollect, labelSize, this, out mCoinAutoCollectCheckbox));
		mPanel.AddWidget(GenerateCheckboxWithText("无冷却种植", 1, mApp.mEasyPlantingCheat, labelSize, this, out var check));
		mPanel.AddWidget(GenerateCheckboxWithText("房主无敌", 8, EnableNoZombieInHouse, labelSize, this, out check));
		mPanel.AddWidget(GenerateCheckboxWithText("无限阳光", 20, NoSunCost, labelSize, this, out check));
		mPanel.AddWidget(GenerateButtonWithText("阳光倍率", 21, "x" + SunMultiplier, labelSize, num - labelSize, this, out mSunMultiplierButton));

		Label label = new Label("速度控制 当前:" + GetGameSpeedString() + "x", Resources.FONT_DWARVENTODCRAFT15, new SexyColor(107, 109, 145));
		label.Resize(0, 0, (int)Math.Ceiling(Resources.FONT_DWARVENTODCRAFT15.MeasureString("速度控制 当前:00.00x").X), Resources.FONT_DWARVENTODCRAFT15.GetHeight());
		mGameSpeedLabel = new WeakReference<Label>(label);
		mPanel.AddWidget(label);
		StackPanel stackPanel = new StackPanel(isVertical: false);
		LawnStoneButton lawnStoneButton = GameButton.MakeButton(101, this, "0.25x");
		lawnStoneButton.Resize(0, 0, num / 4, lawnStoneButton.mHeight);
		stackPanel.AddWidget(lawnStoneButton);
		lawnStoneButton = GameButton.MakeButton(102, this, "0.5x");
		lawnStoneButton.Resize(0, 0, num / 4, lawnStoneButton.mHeight);
		stackPanel.AddWidget(lawnStoneButton);
		lawnStoneButton = GameButton.MakeButton(103, this, "1x");
		lawnStoneButton.Resize(0, 0, num / 2, lawnStoneButton.mHeight);
		stackPanel.AddWidget(lawnStoneButton);
		stackPanel.RefreshSize();
		mPanel.AddWidget(stackPanel);
		label = new Label("跳关功能", Resources.FONT_DWARVENTODCRAFT15, new SexyColor(107, 109, 145));
		label.Resize(0, 0, (int)Math.Ceiling(Resources.FONT_DWARVENTODCRAFT15.MeasureString("跳关功能").X), Resources.FONT_DWARVENTODCRAFT15.GetHeight());
		mPanel.AddWidget(label);
		StackPanel stackPanel2 = new StackPanel(isVertical: false);
		lawnStoneButton = (mEnableSkipLevelButton = GameButton.MakeButton(4, this, " "));
		lawnStoneButton.Resize(0, 0, num / 5, lawnStoneButton.mHeight);
		stackPanel2.AddWidget(lawnStoneButton);
		lawnStoneButton = (mSkipLevelButton = GameButton.MakeButton(3, this, "直接过关"));
		lawnStoneButton.Resize(0, 0, num / 5 * 4, lawnStoneButton.mHeight);
		OnEnableSkipLevelChanged(EnableSkipLevel);
		EnableSkipLevelChanged += OnEnableSkipLevelChanged;
		OnCanEnableSkipLevelChanged(CanEnableSkipLevel);
		CanEnableSkipLevelChanged += OnCanEnableSkipLevelChanged;
		stackPanel2.AddWidget(lawnStoneButton);
		stackPanel2.RefreshSize();
		mPanel.AddWidget(stackPanel2);
		mGridFocusButton = GameButton.MakeButton(13, this, "放大模式");
		mGridFocusButton.Resize(0, 0, num, mGridFocusButton.mHeight);
		LawnStoneButton lawnStoneButton2 = mGridFocusButton;
		Board mBoard = mApp.mBoard;
		lawnStoneButton2.mDisabled = mBoard == null || !mBoard.CanSelectGridFocus;
		mPanel.AddWidget(mGridFocusButton);
		mPanel.RefreshSize();
		AddWidget(mScroll);
		mScroll.Resize(0, 0, mPanel.mWidth, 170);
		mScroll.AddWidget(mPanel);
		mScroll.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
		CalcSize(0, 0);
		mZOrder = 100000;
		static Widget GenerateButtonWithText(string text, int id, string theButtonText, int theWidth, int theButtonWidth, ButtonListener listener, out LawnStoneButton reference)
		{
			SexyColor sexyColor = new SexyColor(107, 109, 145);
			StackPanel row = new StackPanel(isVertical: false);
			Label label3 = new Label(text, Resources.FONT_DWARVENTODCRAFT15, sexyColor, 1.0, DrawStringJustification.Right);
			label3.Resize(0, 0, theWidth, Resources.FONT_DWARVENTODCRAFT15.GetHeight());
			row.AddWidget(label3);
			reference = GameButton.MakeButton(id, listener, theButtonText);
			reference.Resize(0, 0, Math.Max(theButtonWidth, 70), reference.mHeight);
			row.AddWidget(reference);
			row.RefreshSize();
			return row;
		}
		static Widget GenerateCheckboxWithText(string text, int id, bool theDefault, int theWidth, CheckboxListener listener, out Checkbox reference)
		{
			SexyColor sexyColor = new SexyColor(107, 109, 145);
			StackPanel stackPanel3 = new StackPanel(isVertical: false);
			Label label2 = new Label(text, Resources.FONT_DWARVENTODCRAFT15, sexyColor, 1.0, DrawStringJustification.Right);
			label2.Resize(0, 0, theWidth, Resources.FONT_DWARVENTODCRAFT15.GetHeight());
			stackPanel3.AddWidget(label2);
			reference = LawnCommon.MakeNewCheckbox(id, listener, theDefault);
			reference.Resize(0, 0, 70, 60);
			stackPanel3.AddWidget(reference);
			stackPanel3.RefreshSize();
			return stackPanel3;
		}
	}

	public static void DoCheckGameModeCanSkipLevel(GameMode gamemode)
	{
		bool canEnableSkipLevel = true;
		switch (gamemode)
		{
		case GameMode.Adventure:
			canEnableSkipLevel = GlobalStaticVars.gLawnApp.mPlayerInfo == null || GlobalStaticVars.gLawnApp.mPlayerInfo.mFinishedAdventure <= 0;
			break;
		case GameMode.SurvivalEndlessStage1:
		case GameMode.SurvivalEndlessStage2:
		case GameMode.SurvivalEndlessStage3:
		case GameMode.SurvivalEndlessStage4:
		case GameMode.SurvivalEndlessStage5:
		case GameMode.ChallengeZenGarden:
		case GameMode.TreeOfWisdom:
		case GameMode.ScaryPotterEndless:
		case GameMode.PuzzleIZombieEndless:
		case GameMode.Upsell:
		case GameMode.ChallengeAttackOnTitans:
		case GameMode.ChallengeAttackOnTitans2:
		case GameMode.ChallengeAttackOnTitans3:
		case GameMode.MarvelousPeople:
			canEnableSkipLevel = false;
			break;
		}
		CanEnableSkipLevel = canEnableSkipLevel;
	}

	private void OnCanEnableSkipLevelChanged(bool value)
	{
		int num = 212;
		if (value)
		{
			mEnableSkipLevelButton.Resize(0, 0, num / 5, mEnableSkipLevelButton.mHeight);
			mSkipLevelButton.Resize(0, 0, num / 5 * 4, mEnableSkipLevelButton.mHeight);
			mEnableSkipLevelButton.mLabel = mEnableSkipLevelButton.mLabel.Remove(1);
		}
		else
		{
			mSkipLevelButton.Resize(0, 0, 0, mEnableSkipLevelButton.mHeight);
			mEnableSkipLevelButton.Resize(0, 0, num, mEnableSkipLevelButton.mHeight);
			mEnableSkipLevelButton.mLabel += ((mApp.mGameMode == GameMode.Adventure) ? " 多周目禁止跳关" : "   此关禁止跳关");
		}
		mEnableSkipLevelButton.mDisabled = !value;
	}

	private void OnEnableSkipLevelChanged(bool value)
	{
		mEnableSkipLevelButton.SetLabel(value ? "◉" : "○");
		mSkipLevelButton.mDisabled = !value;
		mSkipLevelButton.mBtnNoDraw = !value;
	}

	public override void Dispose()
	{
		mBackToGameButton.Dispose();
		EnableSkipLevelChanged -= OnEnableSkipLevelChanged;
		base.Dispose();
	}

	public override int GetPreferredHeight(int theWidth)
	{
		return 410;
	}

	public override void AddedToManager(WidgetManager theWidgetManager)
	{
		base.AddedToManager(theWidgetManager);
		AddWidget(mBackToGameButton);
		AddWidget(mPlusButton);
		AddWidget(mMinusButton);
	}

	public override void RemovedFromManager(WidgetManager theWidgetManager)
	{
		base.RemovedFromManager(theWidgetManager);
		RemoveWidget(mBackToGameButton);
		RemoveWidget(mPlusButton);
		RemoveWidget(mMinusButton);
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		int theHeight2 = AtlasResources.IMAGE_BUTTON_LEFT.mHeight;
		mBackToGameButton.Resize(95, mHeight - 70, mWidth - 200, theHeight2);
		mPlusButton.Resize(20, mHeight - 70, 60, theHeight2);
		mMinusButton.Resize(200, mHeight - 70, 60, theHeight2);
		mScroll.Resize(30, 90, mScroll.mWidth, mHeight - 180);
	}

	public override void Draw(Graphics g)
	{
		if (_isMinimalized)
		{
			GameButton.DrawStoneButton(g, 0, 0, mWidth, mHeight, mDragging, mDragging, "+");
			return;
		}
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(255, 255, 255, 240));
		if (mDragging)
		{
			g.SetColor(new Color(255, 255, 255, 144));
		}
		base.Draw(g);
	}

	public virtual void SliderVal(int theId, double theVal)
	{
	}

	public override void CheckboxChecked(int theId, bool check)
	{
		mApp.PlaySample(Resources.SOUND_BUTTONCLICK);
		switch (theId)
		{
		case 1:
			if (check)
			{
				mApp?.mBoard?.mSeedBank?.RefreshAllPackets();
			}
			EnableNoCoolDownPlanting = check;
			break;
		case 5:
			EnableCoinAutoCollect = check;
			mApp.mPlayerInfo.mCoinAutoCollect = check;
			mApp.WriteCurrentUserConfig();
			break;
		case 7:
			mApp.mTodCheatKeys = check;
			break;
		case 8:
			EnableNoZombieInHouse = check;
			break;
		case 10:
			ZombieSmallHead = check;
			break;
		case 11:
			Main.RunWhenLocked = check;
			mApp.mPlayerInfo.mRunWhileLocked = check;
			mApp.WriteCurrentUserConfig();
			break;
		case 100:
			AdvancedPause = check;
			break;
		case 20:
			NoSunCost = check;
			break;
		}
	}

	public void ChangeGameSpeed(bool isFastMo, int num)
	{
		if (num == 1)
		{
			GlobalStaticVars.gFastMo = false;
			GlobalStaticVars.gSlowMo = false;
			GlobalStaticVars.gFastSlowMoNum = 0;
		}
		else if (isFastMo)
		{
			GlobalStaticVars.gFastMo = true;
			GlobalStaticVars.gSlowMo = false;
			GlobalStaticVars.gFastSlowMoNum = num;
		}
		else
		{
			GlobalStaticVars.gFastMo = false;
			GlobalStaticVars.gSlowMo = true;
			GlobalStaticVars.gFastSlowMoNum = num;
		}
		if (mGameSpeedLabel.TryGetTarget(out var target))
		{
			target.mText = "速度控制 当前:" + GetGameSpeedString() + "x";
		}
	}

	public string GetGameSpeedString()
	{
		if (GlobalStaticVars.gSlowMo)
		{
			return ((GlobalStaticVars.gFastSlowMoNum != 0) ? (1.0 / (double)GlobalStaticVars.gFastSlowMoNum) : 0.25).ToString();
		}
		if (GlobalStaticVars.gFastMo)
		{
			return ((GlobalStaticVars.gFastSlowMoNum != 0) ? GlobalStaticVars.gFastSlowMoNum : 20).ToString();
		}
		return 1.ToString();
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
		switch (theId)
		{
		case 2:
			if (mApp?.mBoard != null)
			{
				mApp.mBoard.mSunMoney = 9990;
			}
			break;
		case 21:
			SunMultiplier = NextMultiplier(SunMultiplier);
			mSunMultiplierButton?.SetLabel("x" + SunMultiplier);
			break;

		case 3:
			if (mApp?.mBoard != null)
			{
				if (mApp.IsSurvivalMode())
				{
					mApp.mBoard.mChallenge.mSurvivalStage = 5;
				}
				mApp.mBoard.RemoveAllZombies();
				mApp.mBoard.FadeOutLevel();
				mApp.mBoard.mBoardFadeOutCounter = 200;
			}
			break;
		case 4:
			EnableSkipLevel = !EnableSkipLevel;
			break;
		case 6:
			GlobalStaticVars.gLawnApp.mWidgetManager.mIMEHandler.StopTextComposition();
			GlobalStaticVars.gLawnApp.mWidgetManager.mIMEHandler.StartTextComposition();
			break;
		case 13:
		{
			Board mBoard = mApp.mBoard;
			if (mBoard != null && mBoard.CanSelectGridFocus)
			{
				ChangeGameSpeed(isFastMo: true, 1);
				mApp.mBoard.SelectGridFocus();
				ButtonDepress(1000);
				mWidgetManager.SetFocus(mApp.mBoard);
			}
			break;
		}
		case 1000:
		{
			sizeBeforeMinimalized = new CGPoint(mWidth, mHeight);
			int theX = posLastMinimalized?.mX ?? (mX + (mWidth - ZBCDIALOG_MINIMIZED_SIZE.x) / 2);
			int theY = posLastMinimalized?.mY ?? mY;
			Resize(theX, theY, ZBCDIALOG_MINIMIZED_SIZE.x, ZBCDIALOG_MINIMIZED_SIZE.y);
			_isMinimalized = true;
			RemoveWidget(mScroll);
			RemoveWidget(mPlusButton);
			RemoveWidget(mMinusButton);
			RemoveWidget(mBackToGameButton);
			break;
		}
		case 1001:
			if (mHeight + 100 <= mWidgetManager.mHeight)
			{
				Resize(mX, mY - 100, mWidth, mHeight + 100);
				mDragging = true;
				mDragMouseX = 0;
				mDragMouseY = 0;
				MouseDrag(0, 0);
				mDragging = false;
			}
			break;
		case 1002:
			if (mHeight - 100 >= 250)
			{
				Resize(mX, mY + 100, mWidth, mHeight - 100);
				mDragging = true;
				mDragMouseX = 0;
				mDragMouseY = 0;
				MouseDrag(0, 0);
				mDragging = false;
			}
			break;
		case 101:
			ChangeGameSpeed(isFastMo: false, 4);
			break;
		case 102:
			ChangeGameSpeed(isFastMo: false, 2);
			break;
		case 103:
			ChangeGameSpeed(isFastMo: true, 1);
			break;
		case 104:
			ChangeGameSpeed(isFastMo: true, 2);
			break;
		case 105:
			ChangeGameSpeed(isFastMo: true, 3);
			break;
		case 106:
			ChangeGameSpeed(isFastMo: true, 4);
			break;
		case 107:
			ChangeGameSpeed(isFastMo: true, 5);
			break;
		case 108:
			ChangeGameSpeed(isFastMo: true, 10);
			break;
		}
	}

	public override void MouseDown(int x, int y, int clickCount)
	{
		beforeDrag = new TRect(mX, mY, mWidth, mHeight);
		base.MouseDown(x, y, clickCount);
	}

	public override void MouseDrag(int x, int y)
	{
		if (_isMinimalized)
		{
			TRect? tRect = beforeDrag;
			if (tRect.HasValue)
			{
				CGPoint absPos = GetAbsPos();
				if (!beforeDrag.Value.Contains((int)((float)x + absPos.X), (int)((float)y + absPos.Y)))
				{
					beforeDrag = null;
				}
			}
		}
		else
		{
			posLastMinimalized = null;
		}
		base.MouseDrag(x, y);
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mDragging && beforeDrag.HasValue)
		{
			if (_isMinimalized)
			{
				posLastMinimalized = new TPoint(mX, mY);
				Resize(mX, mY, sizeBeforeMinimalized.mX, sizeBeforeMinimalized.mY);
				MouseDrag(x - (sizeBeforeMinimalized.mX - ZBCDIALOG_MINIMIZED_SIZE.x) / 2, y);
				AddWidget(mScroll);
				AddWidget(mPlusButton);
				AddWidget(mMinusButton);
				AddWidget(mBackToGameButton);
			}
			_isMinimalized = false;
		}
		base.MouseUp(x, y, theClickCount);
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
		Board mBoard = mApp.mBoard;
		SetVisible(mBoard == null || !mBoard.IsGridFocusActive);
		LawnStoneButton lawnStoneButton = mGridFocusButton;
		Board mBoard2 = mApp.mBoard;
		lawnStoneButton.mDisabled = mBoard2 == null || !mBoard2.CanSelectGridFocus;
		if (mCoinAutoCollectCheckbox.IsChecked() != EnableCoinAutoCollect)
		{
			mCoinAutoCollectCheckbox.SetChecked(EnableCoinAutoCollect, tellListener: false);
		}
		if (mRunWhileLockedCheckbox != null && mRunWhileLockedCheckbox.IsChecked() != Main.RunWhenLocked)
		{
			mRunWhileLockedCheckbox.SetChecked(Main.RunWhenLocked, tellListener: false);
		}
		KeepOnScreen();
	}

	/// <summary>
	/// iPad / foldable menus crop the sides of the 1000-wide layout and the screen size changes between menus and
	/// levels: keep the menu (and its minimised "+" button) inside the part of the screen that is actually visible.
	/// </summary>
	private void KeepOnScreen()
	{
		if (mDragging || mParent != null || GlobalStaticVars.gSexyAppBase?.mScreenScales == null)
		{
			return;
		}
		TPoint p = GlobalStaticVars.gSexyAppBase.mScreenScales.ClampToVisible(mX, mY, mWidth, mHeight);
		if (p.mX != mX || p.mY != mY)
		{
			Move(p.mX, p.mY);
		}
	}
}
