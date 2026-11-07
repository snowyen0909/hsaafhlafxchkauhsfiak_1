using System;
using System.Threading;

namespace Microsoft.Xna.Framework.Net;

internal class AsyncResult : IAsyncResult
{
	public WaitHandle AsyncWaitHandle
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool CompletedSynchronously
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsCompleted
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IAsyncResult.AsyncState
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object AsyncDelegate
	{
		get
		{
			throw new NotImplementedException();
		}
	}
}
