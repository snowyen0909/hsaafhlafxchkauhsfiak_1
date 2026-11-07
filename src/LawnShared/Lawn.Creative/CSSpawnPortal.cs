using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public class CSSpawnPortal : IComponent
{
	public List<(int, int, int)> mPortals;

	public bool mFixPlantAttackView;

	public bool mFixSpawnZombieView;

	public int mRandomPortalTime;

	public int Version { get; init; }

	public CSSpawnPortal()
	{
		mPortals = new List<(int, int, int)>();
		mFixPlantAttackView = false;
		mFixSpawnZombieView = true;
		mRandomPortalTime = -1;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Portals", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareThreeIntListFromString(value, mPortals, (JsonElement item1, JsonElement item2, JsonElement item3) => (item1.GetInt32(), item2.GetInt32(), item3.GetInt32()));
		}
		if (root.TryGetProperty("FixSpawnZombieView", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mFixSpawnZombieView = value2.GetBoolean();
		}
		if (root.TryGetProperty("FixPlantAttackView", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mFixPlantAttackView = value3.GetBoolean();
		}
		if (root.TryGetProperty("RandomPortalTime", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			mRandomPortalTime = value4.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
		for (int i = 0; i < mPortals.Count; i++)
		{
			(int, int, int) tuple = mPortals[i];
			var (num, _, _) = tuple;
			if ((uint)num > 1u)
			{
				throw new Exception($"Portals[{i}]({tuple.Item1},{tuple.Item2},{tuple.Item3}) is not available");
			}
			if (tuple.Item2 < 0 || tuple.Item2 > Constants.GRIDSIZEX || tuple.Item3 < 0 || tuple.Item3 >= Constants.MAX_GRIDSIZEY)
			{
				throw new Exception($"Portals[{i}]({tuple.Item1},{tuple.Item2},{tuple.Item3}) out of range");
			}
		}
	}
}
