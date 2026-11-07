using Microsoft.Xna.Framework.Net;

namespace Microsoft.Xna.Framework.GamerServices;

public class GamerServicesComponent : GameComponent
{
	private static LocalNetworkGamer lng;

	internal static LocalNetworkGamer LocalNetworkGamer
	{
		get
		{
			return lng;
		}
		set
		{
			lng = value;
		}
	}

	public GamerServicesComponent(Game game)
		: base(game)
	{
		Guide.Initialise(game);
	}

	public override void Update(GameTime gameTime)
	{
	}
}
