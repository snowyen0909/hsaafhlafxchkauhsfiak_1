using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSSurvivalLevel : IComponent
{
	public class Info
	{
		public int? mStartingWave;

		public int mStartWeightModifyingWave;

		public bool mUseDefault;

		public int? mValue;

		public int? mWeight;

		public int? mWeightCap;

		public float? mWeightModifierPerWave;

		public Info()
		{
			mUseDefault = true;
			mValue = null;
			mStartingWave = null;
			mWeight = null;
			mStartWeightModifyingWave = 1;
			mWeightModifierPerWave = null;
			mWeightCap = null;
		}

		public void Init(JsonElement item)
		{
			if (item.TryGetProperty("UseDefault", out var value) && value.ValueKind != JsonValueKind.Null)
			{
				mUseDefault = value.GetBoolean();
			}
			if (item.TryGetProperty("Value", out var value2) && value2.ValueKind != JsonValueKind.Null)
			{
				mValue = value2.GetInt32();
			}
			if (item.TryGetProperty("StartingWave", out var value3) && value3.ValueKind != JsonValueKind.Null)
			{
				mStartingWave = value3.GetInt32();
			}
			if (item.TryGetProperty("Weight", out var value4) && value4.ValueKind != JsonValueKind.Null)
			{
				mWeight = value4.GetInt32();
			}
			if (item.TryGetProperty("StartWeightModifyingWave", out var value5) && value5.ValueKind != JsonValueKind.Null)
			{
				mStartWeightModifyingWave = value5.GetInt32();
			}
			if (item.TryGetProperty("WeightModifierPerWave", out var value6) && value6.ValueKind != JsonValueKind.Null)
			{
				mWeightModifierPerWave = value6.GetSingle();
			}
			if (item.TryGetProperty("WeightCap", out var value7) && value7.ValueKind != JsonValueKind.Null)
			{
				mWeightCap = value7.GetInt32();
			}
		}
	}

	public bool mAcceleratedPricing;

	public bool mEndless;

	public int mMaxRandomZombieTypeCount;

	public List<List<(ZombieType, int)>> mMustHaveZombieGroup;

	public int mStages;

	public Dictionary<ZombieType, Info> mZombies;

	public int Version { get; init; }

	public CSSurvivalLevel()
	{
		mEndless = false;
		mStages = 5;
		mMaxRandomZombieTypeCount = 9;
		mZombies = new Dictionary<ZombieType, Info>();
		mMustHaveZombieGroup = new List<List<(ZombieType, int)>>();
		mAcceleratedPricing = false;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Endless", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mEndless = value.GetBoolean();
		}
		if (root.TryGetProperty("Stages", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mStages = value2.GetInt32();
		}
		if (root.TryGetProperty("MaxRandomZombieTypeCount", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mMaxRandomZombieTypeCount = value3.GetInt32();
		}
		if (root.TryGetProperty("Zombies", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			foreach (JsonElement item4 in value4.EnumerateArray())
			{
				ZombieType key = ZombieTypeConvertor.ToEnum(item4.GetProperty("Type").GetInt32(), Version);
				Info info = new Info();
				info.Init(item4);
				mZombies.TryAdd(key, info);
			}
		}
		if (root.TryGetProperty("MustHaveZombieGroup", out var value5) && value5.ValueKind != JsonValueKind.Null)
		{
			foreach (JsonElement item5 in value5.EnumerateArray())
			{
				if (item5.ValueKind == JsonValueKind.Number)
				{
					ZombieType item = ZombieTypeConvertor.ToEnum(item5.GetInt32(), Version);
					mMustHaveZombieGroup.Add(new List<(ZombieType, int)> { (item, 10) });
				}
				else
				{
					if (item5.ValueKind != JsonValueKind.Array)
					{
						continue;
					}
					List<(ZombieType, int)> list = new List<(ZombieType, int)>();
					foreach (JsonElement item6 in item5.EnumerateArray())
					{
						if (item6.ValueKind == JsonValueKind.Number)
						{
							ZombieType item2 = ZombieTypeConvertor.ToEnum(item6.GetInt32(), Version);
							list.Add((item2, 10));
						}
						else if (item6.ValueKind == JsonValueKind.Array)
						{
							ZombieType item3 = ZombieTypeConvertor.ToEnum(item6[0].GetInt32(), Version);
							int @int = item6[1].GetInt32();
							if (@int > 0)
							{
								list.Add((item3, @int));
							}
						}
					}
					if (list.Count > 0)
					{
						mMustHaveZombieGroup.Add(list);
					}
				}
			}
		}
		if (root.TryGetProperty("AcceleratedPricing", out var value6) && value6.ValueKind != JsonValueKind.Null)
		{
			mAcceleratedPricing = value6.GetBoolean();
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
