using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSIZombieLevel : IComponent
{
	public int mLimit;

	public List<(SeedType, int, int)> mPlantInRow;

	public List<(SeedType, int, int)> mPlantInSquare;

	public int Version { get; init; }

	public CSIZombieLevel()
	{
		mLimit = 5;
		mPlantInSquare = new List<(SeedType, int, int)>();
		mPlantInRow = new List<(SeedType, int, int)>();
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("Limit", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mLimit = value.GetInt32();
		}
		if (root.TryGetProperty("PlantInSquare", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareThreeIntListFromString(value2, mPlantInSquare, (JsonElement item1, JsonElement item2, JsonElement item3) => (SeedTypeConvertor.ToEnum(item1.GetInt32(), out var _, Version), item2.GetInt32(), item3.GetInt32()));
		}
		if (root.TryGetProperty("PlantInRow", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareThreeIntListFromString(value3, mPlantInRow, (JsonElement item1, JsonElement item2, JsonElement item3) => (SeedTypeConvertor.ToEnum(item1.GetInt32(), out var _, Version), item2.GetInt32(), item3.GetInt32()));
		}
	}

	public void Check(CreativeLevel level)
	{
	}
}
