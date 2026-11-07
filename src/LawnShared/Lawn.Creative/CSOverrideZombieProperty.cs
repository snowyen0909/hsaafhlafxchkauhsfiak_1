using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSOverrideZombieProperty : IComponent
{
	public class Info
	{
		public int? mBodyHealth;

		public bool? mCanGoInPool;

		public bool? mCanGoOnHighGround;

		public int? mFirstAllowedWave;

		public int? mFlyingHealth;

		public int? mHelmHealth;

		public int? mPickWeight;

		public float? mScaleHealth;

		public float? mScaleZombie;

		public int? mShieldHealth;

		public int? mZombieValue;

		public Info()
		{
			mBodyHealth = null;
			mHelmHealth = null;
			mShieldHealth = null;
			mFlyingHealth = null;
			mZombieValue = null;
			mFirstAllowedWave = null;
			mPickWeight = null;
			mCanGoInPool = null;
			mCanGoOnHighGround = null;
			mScaleZombie = null;
		}

		public void Init(JsonElement item)
		{
			if (item.TryGetProperty("BodyHealth", out var value) && value.ValueKind != JsonValueKind.Null)
			{
				mBodyHealth = value.GetInt32();
			}
			if (item.TryGetProperty("HelmHealth", out var value2) && value2.ValueKind != JsonValueKind.Null)
			{
				mHelmHealth = value2.GetInt32();
			}
			if (item.TryGetProperty("ShieldHealth", out var value3) && value3.ValueKind != JsonValueKind.Null)
			{
				mShieldHealth = value3.GetInt32();
			}
			if (item.TryGetProperty("FlyingHealth", out var value4) && value4.ValueKind != JsonValueKind.Null)
			{
				mFlyingHealth = value4.GetInt32();
			}
			if (item.TryGetProperty("ZombieValue", out var value5) && value5.ValueKind != JsonValueKind.Null)
			{
				mZombieValue = value5.GetInt32();
			}
			if (item.TryGetProperty("FirstAllowedWave", out var value6) && value6.ValueKind != JsonValueKind.Null)
			{
				mFirstAllowedWave = value6.GetInt32();
			}
			if (item.TryGetProperty("PickWeight", out var value7) && value7.ValueKind != JsonValueKind.Null)
			{
				mPickWeight = value7.GetInt32();
			}
			if (item.TryGetProperty("CanGoInPool", out var value8) && value8.ValueKind != JsonValueKind.Null)
			{
				mCanGoInPool = value8.GetBoolean();
			}
			if (item.TryGetProperty("CanGoOnHighGround", out var value9) && value9.ValueKind != JsonValueKind.Null)
			{
				mCanGoOnHighGround = value9.GetBoolean();
			}
			if (item.TryGetProperty("ScaleZombie", out var value10) && value10.ValueKind != JsonValueKind.Null)
			{
				mScaleZombie = value10.GetSingle();
			}
			if (item.TryGetProperty("ScaleHealth", out var value11) && value11.ValueKind != JsonValueKind.Null)
			{
				mScaleHealth = value11.GetSingle();
			}
		}
	}

	public Dictionary<ZombieType, Info> mProperties;

	public int Version { get; init; }

	public CSOverrideZombieProperty()
	{
		mProperties = new Dictionary<ZombieType, Info>();
	}

	public void Init(JsonElement root)
	{
		if (!root.TryGetProperty("Properties", out var value) || value.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item in value.EnumerateArray())
		{
			JsonElement property = item.GetProperty("ZombieType");
			if (property.ValueKind == JsonValueKind.Number)
			{
				ZombieType key = ZombieTypeConvertor.ToEnum(item.GetProperty("ZombieType").GetInt32(), Version);
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
					ZombieType key2 = ZombieTypeConvertor.ToEnum(item2.GetInt32(), Version);
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
