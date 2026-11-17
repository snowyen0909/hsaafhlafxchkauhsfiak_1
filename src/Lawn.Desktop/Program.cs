using System;
using LawnPlatform;
using Sexy;
using Microsoft.Xna.Framework.Input.Touch;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		var activity = new PvZActivity();
		GlobalStaticVars.gPvZActivity = activity;
		using var game = new Sexy.Main();
		// the game reads touches only; let the mouse act as a finger on desktop
		TouchPanel.EnableMouseTouchPoint = true;
		game.Run();
	}
}
