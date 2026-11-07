namespace Microsoft.Xna.Framework.Net;

internal class CommandSessionStateChange : ICommand
{
	private NetworkSessionState newState;

	private NetworkSessionState oldState;

	public NetworkSessionState NewState => newState;

	public NetworkSessionState OldState => oldState;

	public CommandEventType Command => CommandEventType.SessionStateChange;

	public CommandSessionStateChange(NetworkSessionState newState, NetworkSessionState oldState)
	{
		this.newState = newState;
		this.oldState = oldState;
	}
}
