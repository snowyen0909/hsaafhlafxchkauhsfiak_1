using System;
using System.Diagnostics;

namespace Sexy;

public static class Debug
{
	public static Action<string, DebugType> Logger = LoggerConsole;

	public static void Log(object msg)
	{
		Log(DebugType.Info, msg);
	}

	public static void Log(DebugType msgtype, object msg)
	{
		msg = msg.ToString();
		Logger($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {msgtype}] {msg}", msgtype);
	}

	public static void OutputDebug(string format, params object[] t)
	{
		Log(DebugType.Debug, string.Format(format, t));
	}

	public static void OutputDebug<T>(T t)
	{
		Log(DebugType.Debug, $"{t}");
	}

	public static void ASSERT(bool value)
	{
		if (!value)
		{
			Log(DebugType.Error, $"Assertion Failed\n{new StackTrace()}");
		}
	}

	private static void LoggerConsole(string s, DebugType msgtype)
	{
		if (OperatingSystem.IsAndroid() || OperatingSystem.IsBrowser() || OperatingSystem.IsIOS() || OperatingSystem.IsTvOS())
		{
			Console.WriteLine(s);
			return;
		}
		ConsoleColor foregroundColor = Console.ForegroundColor;
		ConsoleColor backgroundColor = Console.BackgroundColor;
		Action action = Console.ResetColor;
		switch (msgtype)
		{
		case DebugType.Info:
			foregroundColor = ConsoleColor.Cyan;
			break;
		case DebugType.Warn:
			foregroundColor = ConsoleColor.Yellow;
			break;
		case DebugType.Error:
			foregroundColor = ConsoleColor.Red;
			break;
		case DebugType.Fatal:
			foregroundColor = ConsoleColor.White;
			backgroundColor = ConsoleColor.DarkRed;
			break;
		}
		Console.ForegroundColor = foregroundColor;
		Console.BackgroundColor = backgroundColor;
		Console.WriteLine(s);
		action();
	}
}
