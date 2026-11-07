using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public class CSDefaultPlantOnLawn : IComponent
{
	public List<(CardPair, int, int)> mPlants;

	public int Version { get; init; }

	public CSDefaultPlantOnLawn()
	{
		mPlants = new List<(CardPair, int, int)>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Plants", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareThreeIntListFromString(value, mPlants, (JsonElement item1, JsonElement item2, JsonElement item3) => (SeedTypeConvertor.ToEnum(item1.GetInt32(), Version), item2.GetInt32(), item3.GetInt32()));
		}
	}

	public void Check(CreativeLevel level)
	{
		for (int i = 0; i < mPlants.Count; i++)
		{
			(CardPair, int, int) tuple = mPlants[i];
			if (tuple.Item1.mSeedType == SeedType.None)
			{
				throw new Exception($"Plants[{i}]({tuple.Item1},{tuple.Item2},{tuple.Item3}) None is not available");
			}
			if (tuple.Item2 < 0 || tuple.Item2 >= Constants.GRIDSIZEX || tuple.Item3 < 0 || tuple.Item3 >= Constants.MAX_GRIDSIZEY)
			{
				throw new Exception($"Plants[{i}]({tuple.Item1},{tuple.Item2},{tuple.Item3}) out of range");
			}
		}
	}
}
