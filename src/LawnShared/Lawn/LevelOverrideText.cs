using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FontStashSharp;
using FontStashSharp.RichText;
using Lawn.Creative;
using Microsoft.Xna.Framework;
using Sexy.TodLib;

namespace Lawn;

internal static class LevelOverrideText
{
	private static readonly HashSet<ProjectileType> ExcludedProjectileTypes = new HashSet<ProjectileType>
	{
		ProjectileType.Cobbig,
		ProjectileType.Talisman,
		ProjectileType.TalismanMove,
		ProjectileType.TalismanSeal
	};

	public static string Build(LawnApp app, SpriteFontBase font, Func<string, IRenderable> images, bool raw, int width, int columns)
	{
		List<(string, string, string, List<string>)> list = new List<(string, string, string, List<string>)>();
		CSOverrideZombieProperty cSOverrideZombieProperty = app.mCreativeLevel?.GetComponent<CSOverrideZombieProperty>();
		if (cSOverrideZombieProperty != null)
		{
			foreach (KeyValuePair<ZombieType, CSOverrideZombieProperty.Info> item in cSOverrideZombieProperty.mProperties.OrderBy((KeyValuePair<ZombieType, CSOverrideZombieProperty.Info> p) => p.Key))
			{
				ZombieType key = item.Key;
				if (key < ZombieType.Normal || (int)key >= GameConstants.gZombieDefs.Length)
				{
					continue;
				}
				CSOverrideZombieProperty.Info value = item.Value;
				List<string> list2 = new List<string>();
				float? mScaleZombie = value.mScaleZombie;
				if (mScaleZombie.HasValue)
				{
					float valueOrDefault = mScaleZombie.GetValueOrDefault();
					if (valueOrDefault != 1f)
					{
						list2.Add(Label("SCALE_ZOMBIE") + "：" + (raw ? ("1 → " + ChangeColor(valueOrDefault.ToString(CultureInfo.InvariantCulture), valueOrDefault - 1f)) : ChangeColor(valueOrDefault.ToString("0.##", CultureInfo.InvariantCulture) + "x", valueOrDefault - 1f)));
					}
				}
				AddStrength(list2, "BODY_HEALTH", Zombie.GetBaseBodyHealth(key, app), value.mBodyHealth, value, raw);
				AddStrength(list2, "HELM_HEALTH", Zombie.GetBaseHelmHealth(key), value.mHelmHealth, value, raw);
				AddStrength(list2, "SHIELD_HEALTH", Zombie.GetBaseShieldHealth(key), value.mShieldHealth, value, raw);
				AddStrength(list2, "FLYING_HEALTH", Zombie.GetBaseFlyingHealth(key), value.mFlyingHealth, value, raw);
				AddCapability(list2, "GO_IN_POOL", Zombie.ZombieTypeCanGoInPoolDefault(key), value.mCanGoInPool, raw);
				AddCapability(list2, "GO_ON_HIGH_GROUND", Zombie.ZombieTypeCanGoOnHighGroundDefault(key), value.mCanGoOnHighGround, raw);
				if (list2.Count > 0)
				{
					list.Add((TodStringFile.TodStringTranslate("[" + Zombie.GetZombieDefinition(key).mZombieName + "]"), "zombie:" + key, "lightgray", list2));
				}
			}
		}
		CSOverridePlantProperty cSOverridePlantProperty = app.mCreativeLevel?.GetComponent<CSOverridePlantProperty>();
		if (cSOverridePlantProperty != null)
		{
			foreach (KeyValuePair<SeedType, CSOverridePlantProperty.Info> item2 in cSOverridePlantProperty.mProperties.OrderBy((KeyValuePair<SeedType, CSOverridePlantProperty.Info> p) => p.Key))
			{
				SeedType key2 = item2.Key;
				if (key2 >= SeedType.Peashooter && (int)key2 < GameConstants.gPlantDefs.Length)
				{
					PlantDefinition plantDefinition = Plant.GetPlantDefinition(key2);
					CSOverridePlantProperty.Info value2 = item2.Value;
					List<string> list3 = new List<string>();
					AddNumber(list3, "SEED_COST", plantDefinition.mSeedCost, value2.mSeedCost, raw: true, inverse: false, lowerIsBetter: true);
					AddNumber(list3, "REFRESH_TIME", plantDefinition.mRefreshTime, value2.mRefreshTime, raw, inverse: false, lowerIsBetter: true);
					bool flag = key2 == SeedType.Sunflower || key2 == SeedType.Twinsunflower || key2 == SeedType.Sunshroom || key2 == SeedType.SunflowerPea || key2 == SeedType.SunflowerWallnut || key2 == SeedType.Marigold;
					AddNumber(list3, flag ? "LAUNCH_RATE_PRODUCTION" : "LAUNCH_RATE_ATTACK", plantDefinition.mLaunchRate, value2.mLaunchRate, raw, inverse: true, lowerIsBetter: true);
					AddNumber(list3, "PLANT_HEALTH", Plant.GetBaseHealth(key2), value2.mPlantHealth, raw);
					if (list3.Count > 0)
					{
						list.Add((Plant.GetNameString(key2, SeedType.None), "plant:" + key2, "lime", list3));
					}
				}
			}
		}
		CSOverrideProjectileProperty cSOverrideProjectileProperty = app.mCreativeLevel?.GetComponent<CSOverrideProjectileProperty>();
		if (cSOverrideProjectileProperty != null)
		{
			ProjectileDefinition[] gProjectileDefinition = GameConstants.gProjectileDefinition;
			foreach (ProjectileDefinition projectileDefinition in gProjectileDefinition)
			{
				ProjectileType mProjectileType = projectileDefinition.mProjectileType;
				if (!ExcludedProjectileTypes.Contains(mProjectileType))
				{
					List<string> list4 = new List<string>();
					if (cSOverrideProjectileProperty.mProperties.TryGetValue(mProjectileType, out var value3))
					{
						AddNumber(list4, "PROJECTILE_DAMAGE", projectileDefinition.mDamage, value3.mDamage, raw);
					}
					if (mProjectileType == ProjectileType.ZombiePea)
					{
						AddCapability(list4, "HIT_FLOWER_POT", before: true, cSOverrideProjectileProperty.mAllowZombiePeaAttackFlowerPot, raw);
					}
					if (list4.Count > 0)
					{
						list.Add((Label("PROJECTILE_" + ProjectileNameKey(mProjectileType)), "projectile:" + mProjectileType, "orange", list4));
					}
				}
			}
		}
		RichTextLayout measure = new RichTextLayout
		{
			Font = font
		};
		int num2 = Math.Max(1, (width - 24 * (columns - 1)) / columns);
		int val = 96;
		foreach (var item3 in list)
		{
			val = Math.Max(val, Width(Escape(item3.Item1)));
		}
		val = Math.Min(val, Math.Max(96, num2 / 2 - 24)) + 24;
		StringBuilder text = new StringBuilder();
		for (int num3 = 0; num3 < list.Count; num3++)
		{
			int num4 = num3 % columns;
			if (num4 == 0 && num3 > 0)
			{
				text.Append('\n');
			}
			(string, string, string, List<string>) tuple = list[num3];
			int num5 = num4 * (num2 + 24);
			int value4 = AppendWrapped("/c[" + tuple.Item3 + "]" + Escape(tuple.Item1) + "/cd", num5, 0, val - 24);
			IRenderable renderable = images(tuple.Item2);
			text.Append("/v[").Append(value4).Append("]/s[")
				.Append(num5)
				.Append("]/i[")
				.Append(tuple.Item2)
				.Append("]/s[")
				.Append(-(num5 + renderable.Size.X))
				.Append(']');
			int num6 = 0;
			foreach (string item4 in tuple.Item4)
			{
				num6 += AppendWrapped(item4, num5 + val, num6, num2 - val);
			}
			if (num4 == columns - 1 || num3 == list.Count - 1)
			{
				text.Append("/vd/s[").Append(width).Append(']');
			}
		}
		return text.ToString();
		int AppendWrapped(string text2, int x, int y, int availableWidth)
		{
			measure.Width = Math.Max(1, availableWidth);
			measure.Text = text2;
			int num7 = y;
			foreach (TextLine line in measure.Lines)
			{
				text.Append("/v[").Append(y).Append("]/s[")
					.Append(x)
					.Append(']');
				foreach (BaseChunk chunk in line.Chunks)
				{
					if (chunk is TextChunk { Color: var color } textChunk)
					{
						if (color.HasValue)
						{
							Color valueOrDefault2 = color.GetValueOrDefault();
							text.Append("/c[#").Append(valueOrDefault2.R.ToString("X2")).Append(valueOrDefault2.G.ToString("X2"))
								.Append(valueOrDefault2.B.ToString("X2"))
								.Append(']');
						}
						text.Append(Escape(textChunk.Text)).Append("/cd");
					}
				}
				text.Append("/s[").Append(-(x + line.Size.X)).Append(']');
				y += line.Size.Y + 4;
			}
			return y - num7;
		}
		static string Escape(string text2)
		{
			return text2.Replace("/", "//");
		}
		int Width(string text2)
		{
			measure.Text = text2;
			return measure.Size.X;
		}
	}

	private static string ProjectileNameKey(ProjectileType type)
	{
		return type switch
		{
			ProjectileType.ZombiePea => "ZOMBIE_PEA", 
			ProjectileType.ZombiePeaMindControl => "ZOMBIE_PEA_MIND_CONTROL", 
			ProjectileType.CattailSpike => "CATTAIL_SPIKE", 
			ProjectileType.HypnoCattailSpike => "HYPNO_CATTAIL_SPIKE", 
			ProjectileType.EndoflameSpike => "ENDOFLAME_SPIKE", 
			ProjectileType.EndoflameFireball => "ENDOFLAME_FIREBALL", 
			ProjectileType.PuffGreen => "PUFF_GREEN", 
			_ => type.ToString().ToUpperInvariant(), 
		};
	}

	private static string Label(string key)
	{
		return TodStringFile.TodStringTranslate("[OVERRIDE_" + key + "]");
	}

	private static string Number(double value)
	{
		return value.ToString("G", CultureInfo.InvariantCulture);
	}

	private static string ChangeColor(string text, double difference)
	{
		if (difference != 0.0)
		{
			return "/c[" + ((difference > 0.0) ? "lime" : "red") + "]" + text + "/cd";
		}
		return text;
	}

	private static string Transition(double before, double after, bool lowerIsBetter = false)
	{
		return Number(before) + " → " + ChangeColor(Number(after), lowerIsBetter ? (before - after) : (after - before));
	}

	private static string Percentage(double ratio, bool lowerIsBetter = false)
	{
		return ChangeColor((ratio * 100.0).ToString("0.#", CultureInfo.InvariantCulture) + "%", lowerIsBetter ? (1.0 - ratio) : (ratio - 1.0));
	}

	private static void AddNumber(List<string> lines, string key, int before, int? after, bool raw, bool inverse = false, bool lowerIsBetter = false)
	{
		if (after.HasValue && before != after.Value)
		{
			double num = (inverse ? ((double)before / (double)after.Value) : ((double)after.Value / (double)before));
			bool flag = raw || before == 0 || !double.IsFinite(num);
			string text = (flag ? Transition(before, after.Value, lowerIsBetter) : Percentage(num, lowerIsBetter && !inverse));
			lines.Add(Label((inverse & flag) ? (key + "_2") : key) + "：" + text);
		}
	}

	private static void AddStrength(List<string> lines, string key, int before, int? replacement, CSOverrideZombieProperty.Info info, bool raw)
	{
		if (replacement.HasValue || info.mScaleHealth.HasValue || info.mScaleZombie.HasValue)
		{
			int num = replacement ?? before;
			float? mScaleHealth = info.mScaleHealth;
			if (mScaleHealth.HasValue)
			{
				float valueOrDefault = mScaleHealth.GetValueOrDefault();
				num = (int)((float)num * valueOrDefault);
			}
			double num2 = info.mScaleZombie ?? 1f;
			double num3 = (raw ? ((double)before) : ((double)before * num2 * num2));
			if ((double)num != num3)
			{
				double num4 = (double)num / num3;
				string text = ((raw || num3 == 0.0 || !double.IsFinite(num4)) ? Transition(before, num) : Percentage(num4));
				lines.Add(Label(key) + "：" + text);
			}
		}
	}

	private static void AddCapability(List<string> lines, string key, bool before, bool? after, bool raw)
	{
		if (after.HasValue && before != after.Value)
		{
			lines.Add(raw ? (Label("CAN_" + key) + "：" + (before ? "true" : "false") + " → " + (after.Value ? "true" : "false")) : Label((after.Value ? "CAN_" : "CANNOT_") + key));
		}
	}
}
