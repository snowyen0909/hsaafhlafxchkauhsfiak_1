using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public sealed class NetworkMachine
{
	private GamerCollection<NetworkGamer> gamers;

	public GamerCollection<NetworkGamer> Gamers => gamers;

	public NetworkMachine()
	{
		gamers = new GamerCollection<NetworkGamer>();
	}
}
