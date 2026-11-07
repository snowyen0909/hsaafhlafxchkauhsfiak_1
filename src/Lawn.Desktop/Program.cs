using System;
using LawnPlatform;
using Sexy;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		var activity = new PvZActivity();
		GlobalStaticVars.gPvZActivity = activity;
		using var game = new Sexy.Main();
		game.Run();
	}
}
