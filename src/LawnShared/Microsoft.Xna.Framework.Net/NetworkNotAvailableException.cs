using System;

namespace Microsoft.Xna.Framework.Net;

[Serializable]
public class NetworkNotAvailableException : NetworkException
{
	public NetworkNotAvailableException()
	{
	}

	public NetworkNotAvailableException(string message)
		: base(message)
	{
	}

	public NetworkNotAvailableException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
