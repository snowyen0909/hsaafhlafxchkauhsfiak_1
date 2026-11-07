using System;
using System.Text.Json;

namespace Lawn.Creative;

public class CSWeather : IComponent
{
	public bool mHasRain;

	public bool mHasStorm;

	public int Version { get; init; }

	public CSWeather()
	{
		mHasRain = false;
		mHasStorm = false;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("HasRain", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mHasRain = value.GetBoolean();
		}
		if (root.TryGetProperty("HasStorm", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mHasStorm = value2.GetBoolean();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (mHasStorm)
		{
			if (level.GetComponent<CSRainingSeeds>() != null)
			{
				throw new Exception("Can not exist together with CSRainingSeeds when having storm");
			}
			if (level.GetComponent<CSVaseLevel>() != null)
			{
				throw new Exception("Can not exist together with CSVaseLevel when having storm");
			}
		}
	}
}
