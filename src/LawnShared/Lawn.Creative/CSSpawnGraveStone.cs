using System;
using System.Text.Json;
using Sexy;

namespace Lawn.Creative;

public class CSSpawnGraveStone : IComponent
{
	public bool mAllowGraveStoneOnDirt;

	public int[] mCountPerColumn;

	public int Version { get; init; }

	public CSSpawnGraveStone()
	{
		mCountPerColumn = new int[9];
		mAllowGraveStoneOnDirt = false;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("CountPerColumn", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareOneIntArrayFromString(value, mCountPerColumn, (JsonElement item) => item.GetInt32());
		}
		if (root.TryGetProperty("AllowGraveStoneOnDirt", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mAllowGraveStoneOnDirt = value2.GetBoolean();
		}
	}

	public void Check(CreativeLevel level)
	{
		for (int i = 0; i < mCountPerColumn.Length; i++)
		{
			if (mCountPerColumn[i] < 0 || mCountPerColumn[i] > Constants.MAX_GRIDSIZEY)
			{
				throw new Exception($"CountPerColumn[{i}]({mCountPerColumn[i]}) out of range [0,{Constants.MAX_GRIDSIZEY}]");
			}
		}
	}
}
