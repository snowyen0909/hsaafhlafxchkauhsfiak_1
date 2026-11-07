namespace Microsoft.Xna.Framework.GamerServices;

internal class MonoGameGamerServicesHelper
{
	private static MonoLiveGuide guide;

	public static void ShowSigninSheet()
	{
		guide.Enabled = true;
		guide.Visible = true;
		Guide.IsVisible = true;
	}

	internal static void Initialise(Game game)
	{
		if (guide == null)
		{
			guide = new MonoLiveGuide(game);
			game.Components.Add(guide);
		}
	}
}
