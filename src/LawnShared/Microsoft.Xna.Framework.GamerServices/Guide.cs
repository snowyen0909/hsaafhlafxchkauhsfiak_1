// Rewritten for iOS: Android AlertDialog/Intents replaced with LawnPlatform.PlatformUI,
// and Delegate.BeginInvoke (unsupported on modern .NET) replaced with Tasks.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Net;
using LawnPlatform;

namespace Microsoft.Xna.Framework.GamerServices;

public static class Guide
{
	private static bool isScreenSaverEnabled;

	private static bool isTrialMode;

	private static bool isVisible;

	private static bool simulateTrialMode;

	public static bool IsScreenSaverEnabled
	{
		get => isScreenSaverEnabled;
		set => isScreenSaverEnabled = value;
	}

	public static bool IsTrialMode
	{
		get => isTrialMode;
		set => isTrialMode = value;
	}

	public static bool IsVisible
	{
		get => isVisible;
		internal set => isVisible = value;
	}

	public static bool SimulateTrialMode
	{
		get => simulateTrialMode;
		set => simulateTrialMode = value;
	}

	internal static void Initialise(Game game)
	{
		MonoGameGamerServicesHelper.Initialise(game);
	}

	private sealed class TaskAsyncResult<T> : IAsyncResult
	{
		public readonly Task<T> Task;
		private readonly object _state;
		public TaskAsyncResult(Task<T> task, object state) { Task = task; _state = state; }
		public object AsyncState => _state;
		public System.Threading.WaitHandle AsyncWaitHandle => ((IAsyncResult)Task).AsyncWaitHandle;
		public bool CompletedSynchronously => false;
		public bool IsCompleted => Task.IsCompleted;
	}

	private static IAsyncResult Wrap<T>(Task<T> task, AsyncCallback callback, object state)
	{
		var ar = new TaskAsyncResult<T>(task, state);
		task.ContinueWith(_ =>
		{
			IsVisible = false;
			callback?.Invoke(ar);
		}, TaskScheduler.Default);
		return ar;
	}

	public static string ShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText, bool usePasswordMode)
	{
		IsVisible = true;
		try
		{
			return PlatformUI.ShowKeyboardInputAsync(title, description, defaultText, usePasswordMode).GetAwaiter().GetResult();
		}
		finally
		{
			IsVisible = false;
		}
	}

	public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText, AsyncCallback callback, object state)
	{
		return BeginShowKeyboardInput(player, title, description, defaultText, callback, state, usePasswordMode: false);
	}

	public static IAsyncResult BeginShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText, AsyncCallback callback, object state, bool usePasswordMode)
	{
		if (IsVisible)
		{
			throw new GuideAlreadyVisibleException("The function cannot be completed at this time: the Guide UI is already active. Wait until Guide.IsVisible is false before issuing this call.");
		}
		IsVisible = true;
		return Wrap(PlatformUI.ShowKeyboardInputAsync(title, description, defaultText, usePasswordMode), callback, state);
	}

	public static string EndShowKeyboardInput(IAsyncResult result)
	{
		try
		{
			return ((TaskAsyncResult<string>)result).Task.GetAwaiter().GetResult();
		}
		finally
		{
			IsVisible = false;
		}
	}

	public static int? ShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon)
	{
		IsVisible = true;
		try
		{
			return PlatformUI.ShowMessageBoxAsync(title, text, buttons.Take(2).ToList()).GetAwaiter().GetResult();
		}
		finally
		{
			IsVisible = false;
		}
	}

	public static IAsyncResult BeginShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
	{
		if (IsVisible)
		{
			throw new GuideAlreadyVisibleException("The function cannot be completed at this time: the Guide UI is already active. Wait until Guide.IsVisible is false before issuing this call.");
		}
		IsVisible = true;
		return Wrap(PlatformUI.ShowMessageBoxAsync(title, text, buttons.Take(2).ToList()), callback, state);
	}

	public static IAsyncResult BeginShowMessageBox(string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
	{
		return BeginShowMessageBox(PlayerIndex.One, title, text, buttons, focusButton, icon, callback, state);
	}

	public static int? EndShowMessageBox(IAsyncResult result)
	{
		try
		{
			return ((TaskAsyncResult<int?>)result).Task.GetAwaiter().GetResult();
		}
		finally
		{
			IsVisible = false;
		}
	}

	public static void ShowMarketplace(PlayerIndex player)
	{
	}

	public static void Show()
	{
		ShowSignIn(1, onlineOnly: false);
	}

	public static void ShowSignIn(int paneCount, bool onlineOnly)
	{
		if (paneCount != 1)
		{
			return;
		}
		MonoGameGamerServicesHelper.ShowSigninSheet();
		if (GamerServicesComponent.LocalNetworkGamer == null)
		{
			GamerServicesComponent.LocalNetworkGamer = new LocalNetworkGamer();
		}
		else
		{
			GamerServicesComponent.LocalNetworkGamer.SignedInGamer.BeginAuthentication(null, null);
		}
	}

	public static void ShowLeaderboard()
	{
	}

	public static void ShowAchievements()
	{
	}

	public static void ShowPeerPicker()
	{
	}

	public static void ShowMatchMaker()
	{
	}
}
