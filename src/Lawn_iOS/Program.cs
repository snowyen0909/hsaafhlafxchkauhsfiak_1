using Foundation;
using LawnPlatform;
using Sexy;
using UIKit;

namespace Lawn_iOS;

[Register("AppDelegate")]
internal class Program : UIApplicationDelegate
{
	private static Sexy.Main game;

	internal static void RunGame()
	{
		var activity = new PvZActivity();
		GlobalStaticVars.gPvZActivity = activity;
		try
		{
			game = new Sexy.Main();
			game.Run();
		}
		catch (System.Exception e)
		{
			activity.OnException(game, e);
		}
	}

	private static void Main(string[] args)
	{
		UIApplication.Main(args, null, typeof(Program));
	}

	public override void FinishedLaunching(UIApplication app)
	{
		UIApplication.SharedApplication.IdleTimerDisabled = true;
		RunGame();
	}
}
