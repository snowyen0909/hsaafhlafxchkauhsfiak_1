using System.Collections.Generic;

namespace Lawn;

public class SeedTypeConvertor
{
	private class SeedTypeConvertorV1 : SeedTypeConvertor
	{
		public SeedTypeConvertorV1()
		{
			Add(SeedType.Peashooter, 0);
			Add(SeedType.Sunflower);
			Add(SeedType.Cherrybomb);
			Add(SeedType.Wallnut);
			Add(SeedType.Potatomine);
			Add(SeedType.Snowpea);
			Add(SeedType.Chomper);
			Add(SeedType.Repeater);
			Add(SeedType.Puffshroom);
			Add(SeedType.Sunshroom);
			Add(SeedType.Fumeshroom);
			Add(SeedType.Gravebuster);
			Add(SeedType.Hypnoshroom);
			Add(SeedType.Scaredyshroom);
			Add(SeedType.Iceshroom);
			Add(SeedType.Doomshroom);
			Add(SeedType.Lilypad);
			Add(SeedType.Squash);
			Add(SeedType.Threepeater);
			Add(SeedType.Tanglekelp);
			Add(SeedType.Jalapeno);
			Add(SeedType.Spikeweed);
			Add(SeedType.Torchwood);
			Add(SeedType.Tallnut);
			Add(SeedType.Seashroom);
			Add(SeedType.Plantern);
			Add(SeedType.Cactus);
			Add(SeedType.Blover);
			Add(SeedType.Splitpea);
			Add(SeedType.Starfruit);
			Add(SeedType.Pumpkinshell);
			Add(SeedType.Magnetshroom);
			Add(SeedType.Cabbagepult);
			Add(SeedType.Flowerpot);
			Add(SeedType.Kernelpult);
			Add(SeedType.InstantCoffee);
			Add(SeedType.Garlic);
			Add(SeedType.Umbrella);
			Add(SeedType.Marigold);
			Add(SeedType.Melonpult);
			Add(SeedType.Gatlingpea);
			Add(SeedType.Twinsunflower);
			Add(SeedType.Gloomshroom);
			Add(SeedType.Cattail);
			Add(SeedType.Wintermelon);
			Add(SeedType.GoldMagnet);
			Add(SeedType.Spikerock);
			Add(SeedType.Cobcannon);
			Add(SeedType.Imitater);
			Add(SeedType.ExplodeONut);
			Add(SeedType.GiantWallnut);
			Add(SeedType.Sprout);
			Add(SeedType.Leftpeater);
			Add(SeedType.SuperChomper, 2000);
			Add(SeedType.PickledPepper);
			Add(SeedType.FireShroom);
			Add(SeedType.Agave);
			Add(SeedType.Endoflame);
			Add(SeedType.ImitaterRandomPlant);
			Add(SeedType.ImitaterRandomZombie);
		}
	}

	private static readonly SeedTypeConvertor sConvertor;

	private readonly Dictionary<SeedType, int> sEnumToIntMap = new Dictionary<SeedType, int>();

	private readonly Dictionary<int, SeedType> sIntToEnumMap = new Dictionary<int, SeedType>();

	private int mNextIndex;

	static SeedTypeConvertor()
	{
		sConvertor = new SeedTypeConvertorV1();
	}

	public static int ToInt(CardPair thePair, int version)
	{
		return ToInt(thePair.mSeedType, thePair.mImitator, version);
	}

	public static CardPair ToEnum(int index, int version)
	{
		SeedType mSeedType = ToEnum(index, out var isImitator, version);
		return new CardPair
		{
			mSeedType = mSeedType,
			mImitator = isImitator
		};
	}

	public static int ToInt(SeedType theEnum, bool isImitator, int version)
	{
		if (theEnum == SeedType.None)
		{
			return -1;
		}
		if ((theEnum & SeedType.MindControlledCardFlag) != 0)
		{
			ZombieType theEnum2 = (ZombieType)(theEnum & (SeedType)(-1073741825));
			int num = ZombieTypeConvertor.ToInt(theEnum2, version);
			if (num < 0)
			{
				return -1;
			}
			return num + 30000;
		}
		if ((theEnum & SeedType.ZombieCardFlag) != 0)
		{
			ZombieType theEnum3 = (ZombieType)(theEnum & (SeedType)(-536870913));
			int num2 = ZombieTypeConvertor.ToInt(theEnum3, version);
			if (num2 < 0)
			{
				return -1;
			}
			return num2 + 20000;
		}
		int num3 = GetConvertor(version).ToIntInternal(theEnum);
		if (num3 < 0)
		{
			return -1;
		}
		if (isImitator)
		{
			num3 += 10000;
		}
		return num3;
	}

	public static SeedType ToEnum(int index, out bool isImitator, int version)
	{
		if (index >= 30000)
		{
			index -= 30000;
			isImitator = false;
			ZombieType zombieType = ZombieTypeConvertor.ToEnum(index, version);
			if (zombieType == ZombieType.Invalid)
			{
				return SeedType.None;
			}
			return (SeedType)(zombieType | (ZombieType)0x40000000);
		}
		if (index >= 20000)
		{
			index -= 20000;
			isImitator = false;
			ZombieType zombieType2 = ZombieTypeConvertor.ToEnum(index, version);
			if (zombieType2 == ZombieType.Invalid)
			{
				return SeedType.None;
			}
			return (SeedType)(zombieType2 | (ZombieType)0x20000000);
		}
		if (index >= 10000)
		{
			index -= 10000;
			isImitator = true;
		}
		else
		{
			isImitator = false;
		}
		return GetConvertor(version).ToEnumInternal(index);
	}

	protected void Add(SeedType theEnum, int? index = null)
	{
		if (index.HasValue)
		{
			mNextIndex = index.Value;
		}
		sEnumToIntMap[theEnum] = mNextIndex;
		sIntToEnumMap[mNextIndex] = theEnum;
		mNextIndex++;
	}

	private int ToIntInternal(SeedType theEnum)
	{
		if (!sEnumToIntMap.TryGetValue(theEnum, out var value))
		{
			return -1;
		}
		return value;
	}

	private SeedType ToEnumInternal(int index)
	{
		if (!sIntToEnumMap.TryGetValue(index, out var value))
		{
			return SeedType.None;
		}
		return value;
	}

	private static SeedTypeConvertor GetConvertor(int version)
	{
		return sConvertor;
	}
}
