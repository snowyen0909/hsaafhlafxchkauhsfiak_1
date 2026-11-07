using System.Collections.Generic;

namespace Lawn;

public class MusicTuneConvertor
{
	private class MusicTuneConvertorV1 : MusicTuneConvertor
	{
		public MusicTuneConvertorV1()
		{
			Add(MusicTune.None, 0);
			Add(MusicTune.DayGrasswalk);
			Add(MusicTune.NightMoongrains);
			Add(MusicTune.PoolWaterygraves);
			Add(MusicTune.FogRigormormist);
			Add(MusicTune.RoofGrazetheroof);
			Add(MusicTune.ChooseYourSeeds);
			Add(MusicTune.TitleCrazyDaveMainTheme);
			Add(MusicTune.ZenGarden);
			Add(MusicTune.PuzzleCerebrawl);
			Add(MusicTune.MinigameLoonboon);
			Add(MusicTune.Conveyer);
			Add(MusicTune.FinalBossBrainiacManiac);
			Add(MusicTune.FinalBoss2, 2000);
		}
	}

	private static readonly MusicTuneConvertor sConvertor;

	private readonly Dictionary<MusicTune, int> sEnumToIntMap = new Dictionary<MusicTune, int>();

	private readonly Dictionary<int, MusicTune> sIntToEnumMap = new Dictionary<int, MusicTune>();

	private int mNextIndex;

	static MusicTuneConvertor()
	{
		sConvertor = new MusicTuneConvertorV1();
	}

	public static int ToInt(MusicTune theEnum, int version)
	{
		return GetConvertor(version).ToIntInternal(theEnum);
	}

	public static MusicTune ToEnum(int index, int version)
	{
		return GetConvertor(version).ToEnumInternal(index);
	}

	protected void Add(MusicTune theEnum, int? index = null)
	{
		if (index.HasValue)
		{
			mNextIndex = index.Value;
		}
		sEnumToIntMap.Add(theEnum, mNextIndex);
		sIntToEnumMap.Add(mNextIndex, theEnum);
		mNextIndex++;
	}

	private int ToIntInternal(MusicTune theEnum)
	{
		if (!sEnumToIntMap.TryGetValue(theEnum, out var value))
		{
			return -1;
		}
		return value;
	}

	private MusicTune ToEnumInternal(int index)
	{
		if (!sIntToEnumMap.TryGetValue(index, out var value))
		{
			return MusicTune.None;
		}
		return value;
	}

	private static MusicTuneConvertor GetConvertor(int version)
	{
		return sConvertor;
	}
}
