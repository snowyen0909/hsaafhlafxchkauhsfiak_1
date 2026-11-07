using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSOverridePlantProperty : IComponent
{
	public class Info
	{
		public int? mSeedCost;

		public int? mRefreshTime;

		public int? mLaunchRate;

		public int? mPlantHealth;

		public Info()
		{
			mSeedCost = null;
			mRefreshTime = null;
			mLaunchRate = null;
			mPlantHealth = null;
		}

		public void Init(JsonElement item)
		{
			if (item.TryGetProperty("SeedCost", out var value) && value.ValueKind != JsonValueKind.Null)
			{
				mSeedCost = value.GetInt32();
			}
			if (item.TryGetProperty("RefreshTime", out var value2) && value2.ValueKind != JsonValueKind.Null)
			{
				mRefreshTime = value2.GetInt32();
			}
			if (item.TryGetProperty("LaunchRate", out var value3) && value3.ValueKind != JsonValueKind.Null)
			{
				mLaunchRate = value3.GetInt32();
			}
			if (item.TryGetProperty("PlantHealth", out var value4) && value4.ValueKind != JsonValueKind.Null)
			{
				mPlantHealth = value4.GetInt32();
			}
		}
	}

	public Dictionary<SeedType, Info> mProperties;

	public int Version { get; init; }

	public CSOverridePlantProperty()
	{
		mProperties = new Dictionary<SeedType, Info>();
	}

	public void Init(JsonElement root)
	{
		if (!root.TryGetProperty("Properties", out var value) || value.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item in value.EnumerateArray())
		{
			JsonElement property = item.GetProperty("SeedType");
			bool isImitator;
			if (property.ValueKind == JsonValueKind.Number)
			{
				SeedType key = SeedTypeConvertor.ToEnum(item.GetProperty("SeedType").GetInt32(), out isImitator, Version);
				if (mProperties.TryGetValue(key, out var value2))
				{
					value2.Init(item);
					continue;
				}
				value2 = new Info();
				value2.Init(item);
				mProperties.TryAdd(key, value2);
			}
			else
			{
				if (property.ValueKind != JsonValueKind.Array)
				{
					continue;
				}
				foreach (JsonElement item2 in property.EnumerateArray())
				{
					SeedType key2 = SeedTypeConvertor.ToEnum(item2.GetInt32(), out isImitator, Version);
					if (mProperties.TryGetValue(key2, out var value3))
					{
						value3.Init(item);
						continue;
					}
					value3 = new Info();
					value3.Init(item);
					mProperties.TryAdd(key2, value3);
				}
			}
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
