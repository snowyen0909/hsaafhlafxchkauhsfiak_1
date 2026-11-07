using System;
using System.Text.Json;

namespace Lawn.Creative;

public class CSSpeed : IComponent
{
	public int mExtraUpdateTimes;

	public int Version { get; init; }

	public CSSpeed()
	{
		mExtraUpdateTimes = 1;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("ExtraUpdateTimes", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mExtraUpdateTimes = value.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (mExtraUpdateTimes < 1)
		{
			throw new Exception($"ExtraUpdateTimes({mExtraUpdateTimes}) can not be smaller than 1");
		}
	}
}
