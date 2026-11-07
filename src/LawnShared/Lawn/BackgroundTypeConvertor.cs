using System.Collections.Generic;

namespace Lawn;

public class BackgroundTypeConvertor
{
	private class BackgroundTypeConvertorV1 : BackgroundTypeConvertor
	{
		public BackgroundTypeConvertorV1()
		{
			Add(BackgroundType.Num1Day, 0);
			Add(BackgroundType.Num2Night);
			Add(BackgroundType.Num3Pool);
			Add(BackgroundType.Num4Fog);
			Add(BackgroundType.Num5Roof);
			Add(BackgroundType.Num6Boss);
			Add(BackgroundType.MushroomGarden);
			Add(BackgroundType.Greenhouse);
			Add(BackgroundType.Zombiquarium);
			Add(BackgroundType.TreeOfWisdom);
			Add(BackgroundType.GreenhouseNight, 2000);
			Add(BackgroundType.ShallowDay);
		}
	}

	private static readonly BackgroundTypeConvertor sConvertor;

	private readonly Dictionary<BackgroundType, int> sEnumToIntMap = new Dictionary<BackgroundType, int>();

	private readonly Dictionary<int, BackgroundType> sIntToEnumMap = new Dictionary<int, BackgroundType>();

	private int mNextIndex;

	static BackgroundTypeConvertor()
	{
		sConvertor = new BackgroundTypeConvertorV1();
	}

	public static int ToInt(BackgroundType theEnum, int version)
	{
		return GetConvertor(version).ToIntInternal(theEnum);
	}

	public static BackgroundType ToEnum(int index, int version)
	{
		return GetConvertor(version).ToEnumInternal(index);
	}

	protected void Add(BackgroundType theEnum, int? index = null)
	{
		if (index.HasValue)
		{
			mNextIndex = index.Value;
		}
		sEnumToIntMap.Add(theEnum, mNextIndex);
		sIntToEnumMap.Add(mNextIndex, theEnum);
		mNextIndex++;
	}

	private int ToIntInternal(BackgroundType theEnum)
	{
		if (!sEnumToIntMap.TryGetValue(theEnum, out var value))
		{
			return -1;
		}
		return value;
	}

	private BackgroundType ToEnumInternal(int index)
	{
		if (!sIntToEnumMap.TryGetValue(index, out var value))
		{
			return BackgroundType.Num1Day;
		}
		return value;
	}

	private static BackgroundTypeConvertor GetConvertor(int version)
	{
		return sConvertor;
	}
}
