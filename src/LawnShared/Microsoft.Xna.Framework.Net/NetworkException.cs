using System;

namespace Microsoft.Xna.Framework.Net;

[Serializable]
public class NetworkException : Exception
{
	public NetworkException()
	{
	}

	public NetworkException(string message)
	{
	}

	public NetworkException(string message, Exception innerException)
	{
	}
}
