using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSOverrideProjectileProperty : IComponent
{
	public class Info
	{
		public int? mDamage;

		public Info()
		{
			mDamage = null;
		}

		public void Init(JsonElement item)
		{
			if (item.TryGetProperty("Damage", out var value) && value.ValueKind != JsonValueKind.Null)
			{
				mDamage = value.GetInt32();
			}
		}
	}

	public bool? mAllowZombiePeaAttackFlowerPot;

	public Dictionary<ProjectileType, Info> mProperties;

	public int Version { get; init; }

	public CSOverrideProjectileProperty()
	{
		mAllowZombiePeaAttackFlowerPot = null;
		mProperties = new Dictionary<ProjectileType, Info>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("AllowZombiePeaAttackFlowerPot", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mAllowZombiePeaAttackFlowerPot = value.GetBoolean();
		}
		if (!root.TryGetProperty("Properties", out var value2) || value2.ValueKind == JsonValueKind.Null)
		{
			return;
		}
		foreach (JsonElement item in value2.EnumerateArray())
		{
			JsonElement property = item.GetProperty("ProjectileType");
			if (property.ValueKind == JsonValueKind.Number)
			{
				ProjectileType key = ProjectileTypeConvertor.ToEnum(item.GetProperty("ProjectileType").GetInt32(), Version);
				if (mProperties.TryGetValue(key, out var value3))
				{
					value3.Init(item);
					continue;
				}
				value3 = new Info();
				value3.Init(item);
				mProperties.TryAdd(key, value3);
			}
			else
			{
				if (property.ValueKind != JsonValueKind.Array)
				{
					continue;
				}
				foreach (JsonElement item2 in property.EnumerateArray())
				{
					ProjectileType key2 = ProjectileTypeConvertor.ToEnum(item2.GetInt32(), Version);
					if (mProperties.TryGetValue(key2, out var value4))
					{
						value4.Init(item);
						continue;
					}
					value4 = new Info();
					value4.Init(item);
					mProperties.TryAdd(key2, value4);
				}
			}
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
