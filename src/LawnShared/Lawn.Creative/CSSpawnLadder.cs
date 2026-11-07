using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public class CSSpawnLadder : IComponent
{
	public List<(int, int, int)> mLadders;

	public int Version { get; init; }

	public CSSpawnLadder()
	{
		mLadders = new List<(int, int, int)>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Ladders", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareThreeIntListFromString(value, mLadders, (JsonElement item1, JsonElement item2, JsonElement item3) => (item1.GetInt32(), item2.GetInt32(), item3.GetInt32()));
		}
	}

	public void Check(CreativeLevel level)
	{
		for (int i = 0; i < mLadders.Count; i++)
		{
			(int, int, int) tuple = mLadders[i];
			if (tuple.Item1 != 0)
			{
				throw new Exception($"Ladders[{i}]({tuple.Item1},{tuple.Item2},{tuple.Item3}) is not available");
			}
			if (tuple.Item2 < 0 || tuple.Item2 >= Constants.GRIDSIZEX || tuple.Item3 < 0 || tuple.Item3 >= Constants.MAX_GRIDSIZEY)
			{
				throw new Exception($"Ladders[{i}]({tuple.Item1},{tuple.Item2},{tuple.Item3}) out of range");
			}
		}
	}
}
