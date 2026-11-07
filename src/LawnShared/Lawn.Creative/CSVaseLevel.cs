using System;
using System.Collections.Generic;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public class CSVaseLevel : IComponent
{
	public int mMaxColumn;

	public int mMinColumn;

	public int mNumPlantVases;

	public int mNumZombieVases;

	public List<(SeedType, int)> mPlantVases;

	public List<(ZombieType, int)> mZombieVases;

	public int Version { get; init; }

	public CSVaseLevel()
	{
		mMinColumn = 4;
		mMaxColumn = 8;
		mPlantVases = new List<(SeedType, int)>();
		mZombieVases = new List<(ZombieType, int)>();
		mNumPlantVases = 3;
		mNumZombieVases = 0;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("MinColumn", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mMinColumn = value.GetInt32();
		}
		if (root.TryGetProperty("MaxColumn", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mMaxColumn = value2.GetInt32();
		}
		if (root.TryGetProperty("PlantVases", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareTwoIntListFromString(value3, mPlantVases, (JsonElement item1, JsonElement item2) => (SeedTypeConvertor.ToEnum(item1.GetInt32(), out var _, Version), item2.GetInt32()));
		}
		if (root.TryGetProperty("ZombieVases", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareTwoIntListFromString(value4, mZombieVases, (JsonElement item1, JsonElement item2) => (ZombieTypeConvertor.ToEnum(item1.GetInt32(), Version), item2.GetInt32()));
		}
		if (root.TryGetProperty("NumPlantVases", out var value5) && value5.ValueKind != JsonValueKind.Null)
		{
			mNumPlantVases = value5.GetInt32();
		}
		if (root.TryGetProperty("NumZombieVases", out var value6) && value6.ValueKind != JsonValueKind.Null)
		{
			mNumZombieVases = value6.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (mMinColumn > mMaxColumn)
		{
			throw new Exception($"MinColumn({mMinColumn}) can not be bigger than MaxColumn({mMaxColumn})!");
		}
		if (mMinColumn < 0)
		{
			throw new Exception($"MinColumn({mMinColumn}) can not be smaller than 0!");
		}
		if (mMaxColumn >= Constants.GRIDSIZEX)
		{
			throw new Exception($"MaxColumn({mMaxColumn}) can not be bigger than {Constants.GRIDSIZEX - 1}!");
		}
		int num = 5 * (mMaxColumn - mMinColumn + 1);
		int num2 = 0;
		foreach (var mPlantVase in mPlantVases)
		{
			num2 += mPlantVase.Item2;
		}
		foreach (var mZombieVase in mZombieVases)
		{
			num2 += mZombieVase.Item2;
		}
		if (num2 > num)
		{
			throw new Exception($"ScaryPotCount({num2}) can not be bigger than MaxScaryPotCount{num}!");
		}
	}
}
