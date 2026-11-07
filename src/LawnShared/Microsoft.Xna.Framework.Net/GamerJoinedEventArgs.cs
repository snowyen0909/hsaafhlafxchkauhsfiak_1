using System;

namespace Microsoft.Xna.Framework.Net;

public class GamerJoinedEventArgs : EventArgs
{
	private NetworkGamer gamer;

	public NetworkGamer Gamer => gamer;

	public GamerJoinedEventArgs(NetworkGamer aGamer)
	{
		gamer = aGamer;
	}
}
