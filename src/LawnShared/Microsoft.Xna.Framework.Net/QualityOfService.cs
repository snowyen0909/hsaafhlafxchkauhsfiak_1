using System;

namespace Microsoft.Xna.Framework.Net;

public class QualityOfService
{
	public TimeSpan AverageRoundtripTime => TimeSpan.MinValue;

	public int BytesPerSecondDownstream => 0;

	public int BytesPerSecondUpstream => 0;

	public bool IsAvailable => false;

	public TimeSpan MinimumRoundtripTime => TimeSpan.MinValue;
}
