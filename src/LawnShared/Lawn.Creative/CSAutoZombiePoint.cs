using System;
using System.Text.Json;

namespace Lawn.Creative;

public class CSAutoZombiePoint : IComponent
{
	public float mFlagPointMultiplier;

	public float mPointIncrementPerWave;

	public float mPointMultiplier;

	public int mStartingPoints;

	public int Version { get; init; }

	public CSAutoZombiePoint()
	{
		mStartingPoints = 1;
		mPointIncrementPerWave = 0.333334f;
		mFlagPointMultiplier = 2.5f;
		mPointMultiplier = 1f;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("StartingPoints", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mStartingPoints = value.GetInt32();
		}
		if (root.TryGetProperty("PointIncrementPerWave", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mPointIncrementPerWave = value2.GetSingle();
		}
		if (root.TryGetProperty("FlagPointMultiplier", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mFlagPointMultiplier = value3.GetSingle();
		}
		if (root.TryGetProperty("PointMultiplier", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			mPointMultiplier = value4.GetSingle();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (level.GetComponent<CSManualZombiePoint>() != null)
		{
			throw new Exception("Can not exist together with ManualZombiePoint");
		}
		if (mStartingPoints < 0)
		{
			throw new Exception($"StartingPoints({mStartingPoints}) can not be smaller than 0");
		}
		if (mFlagPointMultiplier < 0f)
		{
			throw new Exception($"FlagPointMultiplier({mFlagPointMultiplier}) can not be smaller than 0");
		}
		if (mPointMultiplier < 0f)
		{
			throw new Exception($"PointMultiplier({mPointMultiplier}) can not be smaller than 0");
		}
	}
}
