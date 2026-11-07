using System;
using System.Text.Json;

namespace Lawn.Creative;

public class CSSpawnSkySun : IComponent
{
	public int mIncrementPerSun;

	public int mMaxTime;

	public int mMinTime;

	public int mRandomRangeTime;

	public bool mSpawnFromSky;

	public int mSunType;

	public int Version { get; init; }

	public CSSpawnSkySun()
	{
		mSpawnFromSky = true;
		mMinTime = 425;
		mMaxTime = 950;
		mRandomRangeTime = 275;
		mIncrementPerSun = 10;
		mSunType = 1;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("SpawnFromSky", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mSpawnFromSky = value.GetBoolean();
		}
		if (root.TryGetProperty("MinTime", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mMinTime = value2.GetInt32();
		}
		if (root.TryGetProperty("MaxTime", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mMaxTime = value3.GetInt32();
		}
		if (root.TryGetProperty("RandomRangeTime", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			mRandomRangeTime = value4.GetInt32();
		}
		if (root.TryGetProperty("IncrementPerSun", out var value5) && value5.ValueKind != JsonValueKind.Null)
		{
			mIncrementPerSun = value5.GetInt32();
		}
		if (root.TryGetProperty("SunType", out var value6) && value6.ValueKind != JsonValueKind.Null)
		{
			mSunType = value6.GetInt32();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (mSunType < 0 || mSunType > 2)
		{
			throw new Exception($"SunType({mSunType}) out of range [0,2]");
		}
	}
}
