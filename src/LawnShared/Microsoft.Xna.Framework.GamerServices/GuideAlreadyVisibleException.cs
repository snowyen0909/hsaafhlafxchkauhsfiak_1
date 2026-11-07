using System;
using System.Runtime.Serialization;

namespace Microsoft.Xna.Framework.GamerServices;

[DataContract]
public class GuideAlreadyVisibleException : Exception
{
	public GuideAlreadyVisibleException()
	{
	}

	public GuideAlreadyVisibleException(string message)
	{
	}

	public GuideAlreadyVisibleException(string message, Exception innerException)
	{
	}
}
