using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Lawn;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.IMEHelper;

namespace Sexy;

public class Main : Game
{
	private bool touchPanelNeedsUpdate;

	private static SexyTransform2D orientationTransform;

	private static UI_ORIENTATION orientationUsed;

	private static bool newOrientation;

	public static bool trialModeChecked = false;

	private static bool trialModeCachedValue = true;

	internal static Graphics graphics;

	private int mFrameCnt;

	private static bool startedProfiler;

	private static bool wantToSuppressDraw;

	private GamePadState previousGamepadState;

	private MouseState previousMouseState;

	private KeyboardState previousKeyboardState;

	public static GamerServicesComponent GamerServicesComp;

	private Stopwatch _gameTimer = new Stopwatch();

	public LawnGameConfig mGameConfig = new LawnGameConfig();

	public static bool RunWhenLocked { get; set; }

	public static bool LOW_MEMORY_DEVICE { get; private set; }

	public static bool DO_LOW_MEMORY_OPTIONS { get; private set; }

	public static bool IsInTrialMode => trialModeCachedValue;

	private void GameSpecificCheatInputCheck()
	{
	}

	private static void SetupForResolution()
	{
		Strings.Culture = CultureInfo.CurrentCulture;
		if (Strings.Culture.TwoLetterISOLanguageName == "fr")
		{
			Constants.Language = Constants.LanguageIndex.fr;
		}
		else if (Strings.Culture.TwoLetterISOLanguageName == "de")
		{
			Constants.Language = Constants.LanguageIndex.de;
		}
		else if (Strings.Culture.TwoLetterISOLanguageName == "es")
		{
			Constants.Language = Constants.LanguageIndex.es;
		}
		else if (Strings.Culture.TwoLetterISOLanguageName == "it")
		{
			Constants.Language = Constants.LanguageIndex.it;
		}
		else if (Strings.Culture.TwoLetterISOLanguageName == "zh")
		{
			Constants.Language = Constants.LanguageIndex.zh_cn;
		}
		else
		{
			Constants.Language = Constants.LanguageIndex.en;
		}
		AtlasResources.mAtlasResources = new AtlasResources_600x1000();
		Constants.Load480x800();
	}

	public Main()
	{
		SetupTileSchedule();
		graphics = Graphics.GetNew(this);
		graphics.IsFullScreen = true;
		Guide.SimulateTrialMode = false;
		GraphicsState.mGraphicsDeviceManager.SupportedOrientations = Constants.SupportedOrientations;
		GraphicsState.mGraphicsDeviceManager.DeviceCreated += graphics_DeviceCreated;
		GraphicsState.mGraphicsDeviceManager.DeviceReset += graphics_DeviceReset;
		GraphicsState.mGraphicsDeviceManager.PreparingDeviceSettings += mGraphicsDeviceManager_PreparingDeviceSettings;
		TargetElapsedTime = TimeSpan.FromSeconds(0.01);
		Exiting += Main_Exiting;
		Window.ClientSizeChanged += OnResize;
		graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
		graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
	}

	internal string FetchApplicationStoragePath()
	{
		return LawnPlatform.PvZActivity.DataRoot;
	}

	private bool mInResize;

	public void OnResize(object sender, EventArgs e)
	{
		if (mInResize)
		{
			return;
		}
		mInResize = true;
		try
		{
			OnResizeImpl();
		}
		finally
		{
			mInResize = false;
		}
	}

	/// <summary>Physical screen size in pixels. The game is landscape-only, so on iOS
	/// always report width &gt;= height even if UIKit still reports portrait at launch.</summary>
	internal static void GetScreenSize(out int width, out int height)
	{
		width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
		height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#if __IOS__
		if (height > width)
		{
			(width, height) = (height, width);
		}
#endif
	}

	private void OnResizeImpl()
	{
		int num = (Constants.Is500pMode ? 1000 : 1000);
		int num2 = (Constants.Is500pMode ? 500 : 600);
		Rectangle clientBounds = Window.ClientBounds;
		if (clientBounds.Height * num >= clientBounds.Width * num2)
		{
			int width = clientBounds.Width;
			int num3 = num2 * clientBounds.Width / num;
		}
		else
		{
			int width = num * clientBounds.Height / num2;
			int num3 = clientBounds.Height;
		}
		GetScreenSize(out int width2, out int height);
		if (clientBounds.Width == width2)
		{
			_ = clientBounds.Height;
		}
		if (GlobalStaticVars.gSexyAppBase != null)
		{
			GlobalStaticVars.gSexyAppBase.mScreenScales.Init(width2, height, num, num2);
			Graphics.Resized();
		}
		GraphicsState.mGraphicsDeviceManager.PreferredBackBufferWidth = width2;
		GraphicsState.mGraphicsDeviceManager.PreferredBackBufferHeight = height;
		GraphicsState.mGraphicsDeviceManager.ApplyChanges();
		GraphicsState.mGraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth = width2;
		GraphicsState.mGraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight = height;
		touchPanelNeedsUpdate = true;
	}

	private static void SetupTileSchedule()
	{
	}

	private void mGraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
	}

	private void graphics_DeviceReset(object sender, EventArgs e)
	{
		GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
	}

	private void graphics_DeviceCreated(object sender, EventArgs e)
	{
		GraphicsDevice.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
		GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
	}

	private void Main_Exiting(object sender, EventArgs e)
	{
		GlobalStaticVars.gSexyAppBase.AppExit();
	}

	protected override void Initialize()
	{
		IsMouseVisible = true;
		Window.OrientationChanged += Window_OrientationChanged;
		ReportAchievement.Initialise();
		base.Initialize();
#if __IOS__
		// iOS always renders to the full screen; size the back buffer (and touch space) to match it.
		OnResize(null, null);
#else
		GlobalStaticVars.gSexyAppBase.mScreenScales.Init(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, 1000, 600);
		graphics.PreferredBackBufferWidth = 1000;
		graphics.PreferredBackBufferHeight = 600;
		GraphicsState.mGraphicsDeviceManager.ApplyChanges();
#endif
		GlobalStaticVars.gSexyAppBase.mWidgetManager.mIMEHandler = new LawnPlatform.PlatformIMEHandler(this);
		GlobalStaticVars.gSexyAppBase.mWidgetManager.mIMEHandler.TextInput += (object? s, MonoGame.IMEHelper.TextInputEventArgs e) =>
		{
			if (e.Character >= ' ')
			{
				GlobalStaticVars.gSexyAppBase.mWidgetManager.KeyChar(new SexyChar(e.Character));
			}
			else
			{
				GlobalStaticVars.gSexyAppBase.mWidgetManager.KeyDown((KeyCode)e.Key);
				GlobalStaticVars.gSexyAppBase.mWidgetManager.KeyUp((KeyCode)e.Key);
			}
		};
	}

	protected override void LoadContent()
	{
		GraphicsState.Init();
		SetupForResolution();
		GlobalStaticVars.initialize(this);
		GlobalStaticVars.mGlobalContent.LoadSplashScreen();
		GlobalStaticVars.gSexyAppBase.StartLoadingThread();
	}

	protected override void UnloadContent()
	{
		GlobalStaticVars.mGlobalContent.cleanUp();
	}

	protected override void BeginRun()
	{
		base.BeginRun();
	}

	public void CompensateForSlowUpdate()
	{
		if (_gameTimer != null)
		{
			_gameTimer.Reset();
			_gameTimer.Start();
		}
	}

	protected override void Update(GameTime gameTime)
	{
		try
		{
			if (!IsActive)
			{
				return;
			}
			if (GlobalStaticVars.gSexyAppBase.WantsToExit)
			{
				GlobalStaticVars.gSexyAppBase.WantsToExit = false;
				Main_Exiting(null, null);
				System.Environment.Exit(0);
			}
			HandleInput(gameTime);
			GlobalStaticVars.gSexyAppBase.UpdateApp();
			if (!trialModeChecked)
			{
				trialModeChecked = true;
				bool flag = trialModeCachedValue;
				trialModeCachedValue = false;
				if ((flag != trialModeCachedValue) & flag)
				{
					LeftTrialMode();
				}
			}
		}
		catch (Exception err)
		{
			GlobalStaticVars.gPvZActivity.OnException(this, err);
		}
		base.Update(gameTime);
	}

	private void LeftTrialMode()
	{
		if (GlobalStaticVars.gSexyAppBase != null)
		{
			GlobalStaticVars.gSexyAppBase.LeftTrialMode();
		}
		Window_OrientationChanged(null, null);
	}

	public static void SuppressNextDraw()
	{
		wantToSuppressDraw = true;
	}

	public static SignedInGamer GetGamer()
	{
		if (Gamer.SignedInGamers.Count == 0)
		{
			return null;
		}
		return Gamer.SignedInGamers[PlayerIndex.One];
	}

	public static void NeedToSetUpOrientationMatrix(UI_ORIENTATION orientation)
	{
		orientationUsed = orientation;
		newOrientation = true;
	}

	private static void SetupOrientationMatrix(UI_ORIENTATION orientation)
	{
		newOrientation = false;
	}

	private void Window_OrientationChanged(object sender, EventArgs e)
	{
		SetupInterfaceOrientation();
	}

	private void SetupInterfaceOrientation()
	{
		if (GlobalStaticVars.gSexyAppBase != null)
		{
			if (Window.CurrentOrientation == DisplayOrientation.LandscapeLeft || Window.CurrentOrientation == DisplayOrientation.LandscapeRight)
			{
				GlobalStaticVars.gSexyAppBase.InterfaceOrientationChanged(UI_ORIENTATION.UI_ORIENTATION_LANDSCAPE_LEFT);
			}
			else
			{
				GlobalStaticVars.gSexyAppBase.InterfaceOrientationChanged(UI_ORIENTATION.UI_ORIENTATION_PORTRAIT);
			}
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		if (newOrientation)
		{
			SetupOrientationMatrix(orientationUsed);
		}
		lock (ResourceManager.DrawLocker)
		{
			GraphicsDevice.Clear(Color.Black);
			try
			{
				GlobalStaticVars.gSexyAppBase.DrawGame(gameTime);
			}
			catch (InvalidOperationException err)
			{
				GlobalStaticVars.gPvZActivity.OnException(this, err, keepSilent: true);
			}
			catch (Exception err2)
			{
				GlobalStaticVars.gPvZActivity.OnException(this, err2);
			}
			base.Draw(gameTime);
		}
	}

	public void HandleInput(GameTime gameTime)
	{
		if (LoadingScreen.IsLoading)
		{
			return;
		}
		PresentationParameters pp = GraphicsState.mGraphicsDeviceManager.GraphicsDevice.PresentationParameters;
		if (TouchPanel.DisplayWidth != pp.BackBufferWidth || TouchPanel.DisplayHeight != pp.BackBufferHeight)
		{
			touchPanelNeedsUpdate = true;
		}
		if (touchPanelNeedsUpdate)
		{
			TouchPanel.DisplayWidth = GraphicsState.mGraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
			TouchPanel.DisplayHeight = GraphicsState.mGraphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight;
			touchPanelNeedsUpdate = false;
		}
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		if (state.Buttons.Back == ButtonState.Pressed && previousGamepadState.Buttons.Back == ButtonState.Released)
		{
			GlobalStaticVars.gSexyAppBase.BackButtonPress();
		}
		TouchCollection state2 = TouchPanel.GetState();
		ScreenScales mScreenScales = GlobalStaticVars.gSexyAppBase.mScreenScales;
		bool flag = false;
		foreach (TouchLocation item in state2)
		{
			_Touch touch = new _Touch
			{
				location = 
				{
					mX = mScreenScales.InvMapTouchX(item.Position.X),
					mY = mScreenScales.InvMapTouchY(item.Position.Y)
				}
			};
			if (item.TryGetPreviousLocation(out var aPreviousLocation))
			{
				touch.previousLocation = mScreenScales.InvMapTouch(new CGPoint(aPreviousLocation.Position.X, aPreviousLocation.Position.Y));
			}
			else
			{
				touch.previousLocation = touch.location;
			}
			touch.timestamp = gameTime.TotalGameTime.TotalSeconds;
			if (item.State == TouchLocationState.Pressed && !flag)
			{
				GlobalStaticVars.gSexyAppBase.TouchBegan(touch);
				flag = true;
			}
			else if (item.State == TouchLocationState.Moved)
			{
				GlobalStaticVars.gSexyAppBase.TouchMoved(touch);
			}
			else if (item.State == TouchLocationState.Released)
			{
				GlobalStaticVars.gSexyAppBase.TouchEnded(touch);
			}
			else if (item.State == TouchLocationState.Invalid)
			{
				GlobalStaticVars.gSexyAppBase.TouchesCanceled();
			}
		}
		List<string> list = new List<string>();
		KeyboardState state3 = Keyboard.GetState();
		Keys[] pressedKeys = state3.GetPressedKeys();
		for (int i = 0; i < pressedKeys.Length; i++)
		{
			Keys key = pressedKeys[i];
			if (previousKeyboardState.IsKeyUp(key))
			{
				GlobalStaticVars.gSexyAppBase.mWidgetManager.KeyDown((KeyCode)key);
				list.Add(key.ToString());
			}
		}
		Keys[] pressedKeys2 = previousKeyboardState.GetPressedKeys();
		foreach (Keys key2 in pressedKeys2)
		{
			if (state3.IsKeyUp(key2))
			{
				GlobalStaticVars.gSexyAppBase.mWidgetManager.KeyUp((KeyCode)key2);
			}
		}
		previousGamepadState = state;
		previousKeyboardState = state3;
	}

	private void Window_TextInput(object sender, Microsoft.Xna.Framework.TextInputEventArgs e)
	{
	}

	protected override void OnActivated(object sender, EventArgs args)
	{
		trialModeChecked = false;
		if (GlobalStaticVars.gSexyAppBase != null)
		{
			GlobalStaticVars.gSexyAppBase.GotFocus();
			if (!GlobalStaticVars.gSexyAppBase.mMusicInterface.isStopped)
			{
				GlobalStaticVars.gSexyAppBase.mMusicInterface.ResumeMusic();
			}
		}
		base.OnActivated(sender, args);
	}

	protected override void OnDeactivated(object sender, EventArgs args)
	{
		if (GlobalStaticVars.gSexyAppBase != null)
		{
			GlobalStaticVars.gSexyAppBase.LostFocus();
			if (!GlobalStaticVars.gSexyAppBase.mMusicInterface.isStopped)
			{
				GlobalStaticVars.gSexyAppBase.mMusicInterface.PauseMusic();
			}
			GlobalStaticVars.gSexyAppBase.AppEnteredBackground();
			base.OnDeactivated(sender, args);
		}
	}
}
