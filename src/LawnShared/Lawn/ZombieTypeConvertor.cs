using System.Collections.Generic;

namespace Lawn;

public class ZombieTypeConvertor
{
	private class ZombieTypeConvertorV1 : ZombieTypeConvertor
	{
		public ZombieTypeConvertorV1()
		{
			Add(ZombieType.Normal, 0);
			Add(ZombieType.Flag);
			Add(ZombieType.TrafficCone);
			Add(ZombieType.Polevaulter);
			Add(ZombieType.Pail);
			Add(ZombieType.Newspaper);
			Add(ZombieType.Door);
			Add(ZombieType.Football);
			Add(ZombieType.Dancer);
			Add(ZombieType.BackupDancer);
			Add(ZombieType.DuckyTube);
			Add(ZombieType.Snorkel);
			Add(ZombieType.Zamboni);
			Add(ZombieType.Bobsled);
			Add(ZombieType.DolphinRider);
			Add(ZombieType.JackInTheBox);
			Add(ZombieType.Balloon);
			Add(ZombieType.Digger);
			Add(ZombieType.Pogo);
			Add(ZombieType.Yeti);
			Add(ZombieType.Bungee);
			Add(ZombieType.Ladder);
			Add(ZombieType.Catapult);
			Add(ZombieType.Gargantuar);
			Add(ZombieType.Imp);
			Add(ZombieType.Boss);
			Add(ZombieType.RedeyeGargantuar, 32);
			Add(ZombieType.RobotTitan, 2000);
			Add(ZombieType.RedeyeRobotTitan);
			Add(ZombieType.Monk);
			Add(ZombieType.FootballPremium);
			Add(ZombieType.Ninja);
			Add(ZombieType.Talisman);
		}
	}

	private static readonly ZombieTypeConvertor sConvertor;

	private readonly Dictionary<ZombieType, int> sEnumToIntMap = new Dictionary<ZombieType, int>();

	private readonly Dictionary<int, ZombieType> sIntToEnumMap = new Dictionary<int, ZombieType>();

	private int mNextIndex;

	static ZombieTypeConvertor()
	{
		sConvertor = new ZombieTypeConvertorV1();
	}

	public static int ToInt(ZombieType theEnum, int version)
	{
		return GetConvertor(version).ToIntInternal(theEnum);
	}

	public static ZombieType ToEnum(int index, int version)
	{
		return GetConvertor(version).ToEnumInternal(index);
	}

	protected void Add(ZombieType theEnum, int? index = null)
	{
		if (index.HasValue)
		{
			mNextIndex = index.Value;
		}
		sEnumToIntMap.Add(theEnum, mNextIndex);
		sIntToEnumMap.Add(mNextIndex, theEnum);
		mNextIndex++;
	}

	private int ToIntInternal(ZombieType theEnum)
	{
		if (!sEnumToIntMap.TryGetValue(theEnum, out var value))
		{
			return -1;
		}
		return value;
	}

	private ZombieType ToEnumInternal(int index)
	{
		if (!sIntToEnumMap.TryGetValue(index, out var value))
		{
			return ZombieType.Invalid;
		}
		return value;
	}

	private static ZombieTypeConvertor GetConvertor(int version)
	{
		return sConvertor;
	}
}
