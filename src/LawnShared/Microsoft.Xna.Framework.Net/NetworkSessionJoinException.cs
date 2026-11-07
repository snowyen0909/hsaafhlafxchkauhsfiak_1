using System;

namespace Microsoft.Xna.Framework.Net;

[Serializable]
public class NetworkSessionJoinException : NetworkException
{
	private NetworkSessionJoinError _JoinError;

	public NetworkSessionJoinError JoinError
	{
		get
		{
			return _JoinError;
		}
		set
		{
			if (_JoinError != value)
			{
				JoinError = value;
			}
		}
	}

	public NetworkSessionJoinException()
	{
	}

	public NetworkSessionJoinException(string message)
		: base(message)
	{
	}

	public NetworkSessionJoinException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public NetworkSessionJoinException(string message, NetworkSessionJoinError joinError)
		: base(message)
	{
		_JoinError = JoinError;
	}
}
