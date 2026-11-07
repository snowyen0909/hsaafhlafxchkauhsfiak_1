using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSManualZombiePoint : IComponent
{
	public List<int> mDensity;

	public int Version { get; init; }

	public CSManualZombiePoint()
	{
		mDensity = new List<int>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Density", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareOneIntListFromString(value, mDensity, (JsonElement item) => item.GetInt32());
		}
	}

	public void Check(CreativeLevel level)
	{
		if (level.GetComponent<CSAutoZombiePoint>() != null)
		{
			throw new Exception("Can not exist together with AutoZombiePoint");
		}
		int mNumWaves = level.GetComponent<CSLevelProperty>().mNumWaves;
		if (mDensity.Count < mNumWaves)
		{
			throw new Exception($"DensityCount({mDensity.Count}) can not be smaller than NumWaves{mNumWaves}");
		}
	}
}
