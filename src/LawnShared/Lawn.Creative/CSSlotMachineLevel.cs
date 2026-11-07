using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSSlotMachineLevel : IComponent
{
	public List<(SeedType, int, int, int, int)> mAllowedPlants;

	public int mTargetSunCount;

	public int Version { get; init; }

	public CSSlotMachineLevel()
	{
		mTargetSunCount = 2000;
		mAllowedPlants = new List<(SeedType, int, int, int, int)>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("TargetSunCount", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mTargetSunCount = value.GetInt32();
		}
		if (!root.TryGetProperty("AllowedPlants", out var value2) || value2.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item2 in value2.EnumerateArray())
		{
			SeedType item = SeedTypeConvertor.ToEnum(item2[0].GetInt32(), out var _, Version);
			int @int = item2[1].GetInt32();
			if (item2.GetArrayLength() >= 5)
			{
				mAllowedPlants.Add((item, @int, item2[2].GetInt32(), item2[3].GetInt32(), item2[4].GetInt32()));
			}
			else
			{
				mAllowedPlants.Add((item, @int, @int, 0, 0));
			}
		}
	}

	public void Check(CreativeLevel level)
	{
		if (level.GetComponent<CSSeedBank>() != null)
		{
			throw new Exception("Can not exist together with SeedBank");
		}
		if (level.GetComponent<CSConveyorBelt>() != null)
		{
			throw new Exception("Can not exist together with ConveyorBelt");
		}
		if (level.GetComponent<CSManualZombiePoint>() != null)
		{
			throw new Exception("Can not exist together with ManualZombiePoint");
		}
	}
}
