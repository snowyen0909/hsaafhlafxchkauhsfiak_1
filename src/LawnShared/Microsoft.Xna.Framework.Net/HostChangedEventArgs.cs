using System;

namespace Microsoft.Xna.Framework.Net;

public class HostChangedEventArgs : EventArgs
{
	private NetworkGamer newHost;

	private NetworkGamer oldHost;

	public NetworkGamer NewHost => newHost;

	public NetworkGamer OldHost => oldHost;

	public HostChangedEventArgs(NetworkGamer aNewHost, NetworkGamer aOldHost)
	{
		newHost = aNewHost;
		oldHost = aOldHost;
	}
}
