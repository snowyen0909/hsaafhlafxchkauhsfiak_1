using System;

namespace Microsoft.Xna.Framework.GamerServices;

public static class GamerServicesDispatcher
{
	public static bool IsInitialized => false;

	public static IntPtr WindowHandle { get; set; }

	public static event EventHandler<EventArgs> InstallingTitleUpdate;

	public static void Update()
	{
	}

	private static bool SuppressEventHandlerWarningsUntilEventsAreProperlyImplemented()
	{
		return InstallingTitleUpdate != null;
	}
}
