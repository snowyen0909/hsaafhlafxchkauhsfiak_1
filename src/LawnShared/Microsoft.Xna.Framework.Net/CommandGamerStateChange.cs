namespace Microsoft.Xna.Framework.Net;

internal class CommandGamerStateChange : ICommand
{
	private GamerStates newState;

	private GamerStates oldState;

	private NetworkGamer gamer;

	public NetworkGamer Gamer => gamer;

	public GamerStates NewState => newState;

	public GamerStates OldState => oldState;

	public CommandEventType Command => CommandEventType.GamerStateChange;

	public CommandGamerStateChange(NetworkGamer gamer)
	{
		this.gamer = gamer;
		newState = gamer.State;
		oldState = gamer.OldState;
	}
}
