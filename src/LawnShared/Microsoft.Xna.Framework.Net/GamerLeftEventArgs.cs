using System;

namespace Microsoft.Xna.Framework.Net;

public class GamerLeftEventArgs : EventArgs
{
	private NetworkGamer gamer;

	public NetworkGamer Gamer => gamer;

	public GamerLeftEventArgs(NetworkGamer aGamer)
	{
		gamer = aGamer;
	}
}
