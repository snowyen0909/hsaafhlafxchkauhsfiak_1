// Platform layer replacing the Android-only Lawn_Android.PvZActivity.
// Keeps the same public surface the game code calls, implemented for iOS
// (UIKit) with a plain fallback so the code also compiles on desktop.
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Lawn;
using Microsoft.Xna.Framework;
using Sexy;
#if __IOS__
using Foundation;
using UIKit;
#endif

namespace LawnPlatform;

public sealed class PlatformDir
{
	public PlatformDir(string path) { AbsolutePath = path; }
	public string AbsolutePath { get; }
}

public sealed class PlatformAssets
{
	public Stream Open(string path)
	{
		path = path.Replace('\\', '/');
		return WardrobeOverlay.Open(path);
	}
}

public class PvZActivity
{
	public bool isErrorAlreadyHappen;

	private int mLoggedCount;

	public static Action<Exception> ExceptionReporter = (Exception e) => { };

	public static Action Modded = () => { };

	public PlatformAssets Assets { get; } = new PlatformAssets();

	/// <summary>Last step reached during startup; written into error.log (AOT builds have no line numbers).</summary>
	public static volatile string Breadcrumb = "";

	/// <summary>Writable per-app data root (iOS: Documents, visible in the Files app).</summary>
	public static string DataRoot
	{
		get
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (string.IsNullOrEmpty(path))
			{
				path = Path.Combine(AppContext.BaseDirectory, "docs");
			}
			Directory.CreateDirectory(path);
			return path;
		}
	}

	public string CustRoot => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

	private static int sLogLines;

	private static readonly object sLogLock = new object();

	/// <summary>Diagnostic log (Documents/game.log, first 400 lines per launch).</summary>
	public static void Log(string msg)
	{
		try
		{
			lock (sLogLock)
			{
				string path = Path.Combine(DataRoot, "game.log");
				if (sLogLines == 0 && File.Exists(path))
				{
					File.Delete(path);
				}
				if (++sLogLines > 400)
				{
					return;
				}
				File.AppendAllText(path, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n");
			}
		}
		catch
		{
		}
	}

	public PlatformDir GetExternalFilesDir(string sub)
	{
		string path = string.IsNullOrEmpty(sub) ? DataRoot : Path.Combine(DataRoot, sub);
		Directory.CreateDirectory(path);
		return new PlatformDir(path);
	}

	public void OnException(object sender, Exception err, bool keepSilent = false, bool areStatesDirty = true)
	{
		Sexy.Debug.Log(DebugType.Error, $"{err.Message}\n{err.StackTrace}\n{err.InnerException}");
		try
		{
			if (++mLoggedCount <= 50)
			{
				File.AppendAllText(Path.Combine(DataRoot, "error.log"), $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] (step: {Breadcrumb})\n{err}\n\n");
			}
		}
		catch
		{
		}
		if (!isErrorAlreadyHappen)
		{
			ExceptionReporter(err);
		}
		if (!keepSilent)
		{
			string msg = $"应用版本：{LawnApp.AppVersionNumber}\n\n出现错误{(areStatesDirty ? "，请尽早退出/重启游戏" : "")}\n{err.Message}\n{err.StackTrace}";
			_ = PlatformUI.ShowMessageBoxAsync(err.GetType().ToString(), msg, new[] { "Close" });
		}
		if (areStatesDirty)
		{
			isErrorAlreadyHappen = true;
			Modded();
		}
	}

	/// <summary>Zips the userdata folder into Documents (reachable through the iOS Files app).</summary>
	public void ExportUserdata()
	{
		try
		{
			string src = Path.Combine(GlobalStaticVars.gLawnApp.applicationStoragePath, GlobalStaticVars.GetDocumentsDir(), "userdata");
			string dst = Path.Combine(DataRoot, $"userdata{DateTime.Now:_yyyy-MM-dd_HH_mm_ss}.zip");
			new FastZip().CreateZip(dst, src, recurse: true, fileFilter: null);
			_ = PlatformUI.ShowMessageBoxAsync("Export", "成功导出：\n" + Path.GetFileName(dst) + "\n(在「文件」App → 我的 iPhone → 本游戏 中查看)", new[] { "OK" });
		}
		catch (Exception ex)
		{
			_ = PlatformUI.ShowMessageBoxAsync("Error", ex.Message, new[] { "OK" });
		}
	}
}

public static class PlatformUI
{
	public static void OpenUrl(string url)
	{
#if __IOS__
		try
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				var nsurl = new NSUrl(url);
				UIApplication.SharedApplication.OpenUrl(nsurl, new NSDictionary(), null);
			});
		}
		catch
		{
		}
#endif
	}

	public static void Vibrate(TimeSpan duration)
	{
#if __IOS__
		try
		{
			UIApplication.SharedApplication.InvokeOnMainThread(() =>
			{
				using var gen = new UIImpactFeedbackGenerator(duration.TotalMilliseconds > 300 ? UIImpactFeedbackStyle.Heavy : UIImpactFeedbackStyle.Medium);
				gen.Prepare();
				gen.ImpactOccurred();
			});
		}
		catch
		{
		}
#endif
	}

#if __IOS__
	internal static UIViewController TopViewController()
	{
		UIViewController vc = null;
		foreach (var scene in UIApplication.SharedApplication.ConnectedScenes)
		{
			if (scene is UIWindowScene ws)
			{
				foreach (var w in ws.Windows)
				{
					if (w.IsKeyWindow) { vc = w.RootViewController; break; }
				}
			}
			if (vc != null) break;
		}
		vc ??= UIApplication.SharedApplication.KeyWindow?.RootViewController;
		while (vc?.PresentedViewController != null)
		{
			vc = vc.PresentedViewController;
		}
		return vc;
	}
#endif

	/// <summary>Returns index of the pressed button, or null if no UI is available.</summary>
	public static Task<int?> ShowMessageBoxAsync(string title, string text, IList<string> buttons)
	{
		var tcs = new TaskCompletionSource<int?>();
#if __IOS__
		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			try
			{
				var alert = UIAlertController.Create(title, text, UIAlertControllerStyle.Alert);
				for (int i = 0; i < buttons.Count; i++)
				{
					int idx = i;
					alert.AddAction(UIAlertAction.Create(buttons[i], i == 1 ? UIAlertActionStyle.Cancel : UIAlertActionStyle.Default, _ => tcs.TrySetResult(idx)));
				}
				var vc = TopViewController();
				if (vc == null) { tcs.TrySetResult(null); return; }
				vc.PresentViewController(alert, true, null);
			}
			catch
			{
				tcs.TrySetResult(null);
			}
		});
#else
		Console.WriteLine($"[{title}] {text}");
		tcs.TrySetResult(0);
#endif
		return tcs.Task;
	}

	/// <summary>Returns entered text, or null if cancelled.</summary>
	public static Task<string> ShowKeyboardInputAsync(string title, string description, string defaultText, bool password)
	{
		var tcs = new TaskCompletionSource<string>();
#if __IOS__
		UIApplication.SharedApplication.InvokeOnMainThread(() =>
		{
			try
			{
				var alert = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);
				alert.AddTextField(tf =>
				{
					tf.Text = defaultText ?? string.Empty;
					tf.SecureTextEntry = password;
				});
				alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, _ => tcs.TrySetResult(null)));
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, _ => tcs.TrySetResult(alert.TextFields[0].Text)));
				var vc = TopViewController();
				if (vc == null) { tcs.TrySetResult(defaultText); return; }
				vc.PresentViewController(alert, true, null);
			}
			catch
			{
				tcs.TrySetResult(null);
			}
		});
#else
		tcs.TrySetResult(defaultText);
#endif
		return tcs.Task;
	}
}
