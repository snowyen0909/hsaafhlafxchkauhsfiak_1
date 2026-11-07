using System;
using Sexy;

namespace Lawn;

public class SyncGameException<T> : Exception
{
	public BufferNew buffer;

	public SyncGameException(BufferNew data, Exception innerException = null)
		: base("Error on syncing " + typeof(T).FullName + ".", innerException)
	{
		buffer = data;
	}
}
