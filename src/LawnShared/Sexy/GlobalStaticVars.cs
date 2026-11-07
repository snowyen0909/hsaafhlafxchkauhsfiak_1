using Lawn;
using LawnPlatform;

namespace Sexy;

public static class GlobalStaticVars
{
	public struct CGPoint
	{
		public float x;

		public float y;
	}

	public enum Phase
	{
		TOUCH_BEGAN,
		TOUCH_MOVED,
		TOUCH_STATIONARY,
		TOUCH_ENDED,
		TOUCH_CANCELLED
	}

	public struct Touch
	{
		public CGPoint location;
	}

	public static PvZActivity gPvZActivity;

	public static Graphics g;

	public static GlobalContentManager mGlobalContent;

	public static SexyAppBase gSexyAppBase;

	internal static int gProfileVersion = 14;

	public static bool gIsPartnerBuild = false;

	public static int gSlowMoCounter = 0;

	public static bool gSlowMo = false;

	public static int gFastSlowMoNum = 0;

	public static bool gFastMo = false;

	public static bool gLowFramerate = false;

	public static string gGetCurrentLevelName;

	public static bool gAppCloseRequest;

	public static bool gAppHasUsedCheatKeys;

	public static LawnApp gLawnApp => (LawnApp)gSexyAppBase;

	public static string LawnGetCurrentLevelName()
	{
		if (gLawnApp == null)
		{
			return "Before App";
		}
		if (gLawnApp.mGameScene == GameScenes.Loading)
		{
			return "Game Loading";
		}
		if (gLawnApp.mGameScene == GameScenes.Menu)
		{
			return "Game Selector";
		}
		if (gLawnApp.mGameScene == GameScenes.Award)
		{
			return "Award Screen";
		}
		if (gLawnApp.mGameScene == GameScenes.Menu)
		{
			return "Game Selector";
		}
		if (gLawnApp.mGameScene == GameScenes.Challenge)
		{
			return "Challenge Screen";
		}
		if (gLawnApp.mGameScene == GameScenes.Credit)
		{
			return "Credits";
		}
		if (gLawnApp.mBoard == null)
		{
			return "Not Playing";
		}
		if (gLawnApp.IsFirstTimeAdventureMode())
		{
			return gLawnApp.GetStageString(gLawnApp.mBoard.mLevel);
		}
		return Common.StrFormat_("F{0}", gLawnApp.GetStageString(gLawnApp.mBoard.mLevel));
	}

	public static bool LawnGetCloseRequest()
	{
		if (gLawnApp != null)
		{
			return gLawnApp.mCloseRequest;
		}
		return false;
	}

	public static bool LawnHasUsedCheatKeys()
	{
		if (gLawnApp != null && gLawnApp.mPlayerInfo != null)
		{
			return !gLawnApp.mPlayerInfo.mHasUsedCheatKeys;
		}
		return false;
	}

	public static void initialize(Main main)
	{
		initializeReusables();
		g = Main.graphics;
		g.Init();
		mGlobalContent = new GlobalContentManager(main);
		gGetCurrentLevelName = LawnGetCurrentLevelName();
		gAppCloseRequest = LawnGetCloseRequest();
		gAppHasUsedCheatKeys = LawnHasUsedCheatKeys();
		gSexyAppBase = new LawnApp(main);
		gSexyAppBase.Init();
		gSexyAppBase.Start();
	}

	public static void initializeReusables()
	{
		Graphics.PreAllocateMemory();
	}

	public static void shutdown()
	{
		gLawnApp.Shutdown();
		gLawnApp.Dispose();
	}

	internal static string GetResourceDir()
	{
		return "";
	}

	internal static string CommaSeperate_(int theDispPoints)
	{
		if (theDispPoints == 0)
		{
			return "0";
		}
		return $"{theDispPoints:#,#}";
	}

	internal static string GetDocumentsDir()
	{
		return "docs/";
	}
}
