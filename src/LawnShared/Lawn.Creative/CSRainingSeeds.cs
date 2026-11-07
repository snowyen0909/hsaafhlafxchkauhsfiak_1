using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSRainingSeeds : IComponent
{
	public List<(SeedType, int)> mAllowedPlants;

	public int mMaxPrepareTime;

	public int mMinPrepareTime;

	public int Version { get; init; }

	public CSRainingSeeds()
	{
		mAllowedPlants = new List<(SeedType, int)>();
		mMinPrepareTime = 500;
		mMaxPrepareTime = 1000;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("AllowedPlants", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareTwoIntListFromString(value, mAllowedPlants, (JsonElement item1, JsonElement item2) => (SeedTypeConvertor.ToEnum(item1.GetInt32(), out var _, Version), item2.GetInt32()));
		}
		if (root.TryGetProperty("MinPrepareTime", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mMinPrepareTime = value2.GetInt32();
		}
		if (root.TryGetProperty("MaxPrepareTime", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mMaxPrepareTime = value3.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (level.GetComponent<CSVaseLevel>() != null)
		{
			throw new Exception("Can not exist together with CSVaseLevel");
		}
	}
}
