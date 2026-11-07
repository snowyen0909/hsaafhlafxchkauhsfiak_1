using System;

namespace Microsoft.Xna.Framework.Net;

public class NetworkSessionEndedEventArgs : EventArgs
{
	private NetworkSessionEndReason endReason;

	public NetworkSessionEndReason EndReason => endReason;

	public NetworkSessionEndedEventArgs(NetworkSessionEndReason aEndReason)
	{
		endReason = aEndReason;
	}
}
