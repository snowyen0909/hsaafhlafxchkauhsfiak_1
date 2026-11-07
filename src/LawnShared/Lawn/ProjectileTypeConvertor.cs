using System.Collections.Generic;

namespace Lawn;

public class ProjectileTypeConvertor
{
	private class ProjectileTypeConvertorV1 : ProjectileTypeConvertor
	{
		public ProjectileTypeConvertorV1()
		{
			Add(ProjectileType.Pea, 0);
			Add(ProjectileType.Snowpea);
			Add(ProjectileType.Cabbage);
			Add(ProjectileType.Melon);
			Add(ProjectileType.Puff);
			Add(ProjectileType.Wintermelon);
			Add(ProjectileType.Fireball);
			Add(ProjectileType.Star);
			Add(ProjectileType.Spike);
			Add(ProjectileType.Basketball);
			Add(ProjectileType.Kernel);
			Add(ProjectileType.Cobbig);
			Add(ProjectileType.Butter);
			Add(ProjectileType.ZombiePea);
			Add(ProjectileType.ZombiePeaMindControl);
			Add(ProjectileType.CattailSpike, 2000);
			Add(ProjectileType.EndoflameSpike);
			Add(ProjectileType.EndoflameFireball);
			Add(ProjectileType.Talisman);
			Add(ProjectileType.TalismanMove);
			Add(ProjectileType.TalismanSeal);
			Add(ProjectileType.ProjectilesCount);
		}
	}

	private static readonly ProjectileTypeConvertor sConvertor;

	private readonly Dictionary<ProjectileType, int> sEnumToIntMap = new Dictionary<ProjectileType, int>();

	private readonly Dictionary<int, ProjectileType> sIntToEnumMap = new Dictionary<int, ProjectileType>();

	private int mNextIndex;

	static ProjectileTypeConvertor()
	{
		sConvertor = new ProjectileTypeConvertorV1();
	}

	public static int ToInt(ProjectileType theEnum, int version)
	{
		return GetConvertor(version).ToIntInternal(theEnum);
	}

	public static ProjectileType ToEnum(int index, int version)
	{
		return GetConvertor(version).ToEnumInternal(index);
	}

	protected void Add(ProjectileType theEnum, int? index = null)
	{
		if (index.HasValue)
		{
			mNextIndex = index.Value;
		}
		sEnumToIntMap.Add(theEnum, mNextIndex);
		sIntToEnumMap.Add(mNextIndex, theEnum);
		mNextIndex++;
	}

	private int ToIntInternal(ProjectileType theEnum)
	{
		if (!sEnumToIntMap.TryGetValue(theEnum, out var value))
		{
			return -1;
		}
		return value;
	}

	private ProjectileType ToEnumInternal(int index)
	{
		if (!sIntToEnumMap.TryGetValue(index, out var value))
		{
			return ProjectileType.Pea;
		}
		return value;
	}

	private static ProjectileTypeConvertor GetConvertor(int version)
	{
		return sConvertor;
	}
}
