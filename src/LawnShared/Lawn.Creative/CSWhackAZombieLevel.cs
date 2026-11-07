using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSWhackAZombieLevel : IComponent
{
	public class Info
	{
		public bool mSpawnAtTriple;

		public bool mSpawnOnLastWave;

		public readonly int[] mWeights;

		public Info()
		{
			mSpawnAtTriple = true;
			mSpawnOnLastWave = true;
			mWeights = new int[6] { 10, 10, 10, 10, 10, 10 };
		}

		public void Init(JsonElement item)
		{
			if (item.TryGetProperty("SpawnAtTriple", out var value) && value.ValueKind != JsonValueKind.Null)
			{
				mSpawnAtTriple = value.GetBoolean();
			}
			if (item.TryGetProperty("SpawnOnLastWave", out var value2) && value2.ValueKind != JsonValueKind.Null)
			{
				mSpawnOnLastWave = value2.GetBoolean();
			}
			if (item.TryGetProperty("Weights", out var value3) && value3.ValueKind != JsonValueKind.Null)
			{
				List<int> list = new List<int>();
				IComponent.PrepareOneIntListFromString(value3, list, (JsonElement jsonElement) => jsonElement.GetInt32());
				for (int num = Math.Min(list.Count, mWeights.Length) - 1; num >= 0; num--)
				{
					mWeights[num] = list[num];
				}
			}
		}
	}

	public int mInitGraveStoneCount;

	public int mMinGraveStoneCount;

	public int mSpawnInterval;

	public int[] mDoubleChance;

	public int[] mTripleChance;

	public Dictionary<ZombieType, Info> mZombies;

	public int Version { get; init; }

	public CSWhackAZombieLevel()
	{
		mInitGraveStoneCount = 9;
		mMinGraveStoneCount = 5;
		mSpawnInterval = 2000;
		mDoubleChance = new int[6] { 0, 30, 10, 10, 15, 18 };
		mTripleChance = new int[6] { 0, 0, 0, 0, 10, 13 };
		mZombies = new Dictionary<ZombieType, Info>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("InitGraveStoneCount", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mInitGraveStoneCount = value.GetInt32();
		}
		if (root.TryGetProperty("MinGraveStoneCount", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mMinGraveStoneCount = value2.GetInt32();
		}
		if (root.TryGetProperty("SpawnInterval", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mSpawnInterval = value3.GetInt32();
		}
		if (root.TryGetProperty("DoubleChance", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			List<int> list = new List<int>();
			IComponent.PrepareOneIntListFromString(value4, list, (JsonElement item) => item.GetInt32());
			for (int num = Math.Min(list.Count, mDoubleChance.Length) - 1; num >= 0; num--)
			{
				mDoubleChance[num] = list[num];
			}
		}
		if (root.TryGetProperty("TripleChance", out var value5) && value5.ValueKind != JsonValueKind.Null)
		{
			List<int> list2 = new List<int>();
			IComponent.PrepareOneIntListFromString(value5, list2, (JsonElement item) => item.GetInt32());
			for (int num2 = Math.Min(list2.Count, mTripleChance.Length) - 1; num2 >= 0; num2--)
			{
				mTripleChance[num2] = list2[num2];
			}
		}
		if (!root.TryGetProperty("Zombies", out var value6) || value6.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item in value6.EnumerateArray())
		{
			JsonElement property = item.GetProperty("Type");
			if (property.ValueKind == JsonValueKind.Number)
			{
				ZombieType key = ZombieTypeConvertor.ToEnum(property.GetInt32(), Version);
				if (mZombies.TryGetValue(key, out var value7))
				{
					value7.Init(item);
					continue;
				}
				value7 = new Info();
				value7.Init(item);
				mZombies.TryAdd(key, value7);
			}
			else
			{
				if (property.ValueKind != JsonValueKind.Array)
				{
					continue;
				}
				foreach (JsonElement item2 in property.EnumerateArray())
				{
					ZombieType key2 = ZombieTypeConvertor.ToEnum(item2.GetInt32(), Version);
					if (mZombies.TryGetValue(key2, out var value8))
					{
						value8.Init(item);
						continue;
					}
					value8 = new Info();
					value8.Init(item);
					mZombies.TryAdd(key2, value8);
				}
			}
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
