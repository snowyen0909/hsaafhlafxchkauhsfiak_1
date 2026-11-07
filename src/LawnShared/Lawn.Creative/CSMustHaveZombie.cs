using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSMustHaveZombie : IComponent
{
	public Dictionary<int, List<ZombieType>> mZombies;

	public Dictionary<int, List<(ZombieType, int)>> mZombiesInRow;

	public int Version { get; init; }

	public CSMustHaveZombie()
	{
		mZombies = new Dictionary<int, List<ZombieType>>();
		mZombiesInRow = new Dictionary<int, List<(ZombieType, int)>>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Zombies", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			foreach (JsonElement item2 in value.EnumerateArray())
			{
				int @int = item2[0].GetInt32();
				List<ZombieType> list = new List<ZombieType>();
				IComponent.PrepareOneIntListFromString(item2[1], list, (JsonElement type) => ZombieTypeConvertor.ToEnum(type.GetInt32(), Version));
				if (mZombies.TryGetValue(@int, out var value2))
				{
					for (int num = 0; num < list.Count; num++)
					{
						value2.Add(list[num]);
					}
				}
				else
				{
					mZombies.Add(@int, list);
				}
			}
		}
		if (!root.TryGetProperty("ZombiesInRow", out var value3) || value3.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item3 in value3.EnumerateArray())
		{
			int int2 = item3[0].GetInt32();
			ZombieType item = ZombieTypeConvertor.ToEnum(item3[1].GetInt32(), Version);
			int int3 = item3[2].GetInt32();
			if (mZombiesInRow.TryGetValue(int2, out var value4))
			{
				value4.Add((item, int3));
				continue;
			}
			mZombiesInRow.Add(int2, new List<(ZombieType, int)> { (item, int3) });
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
