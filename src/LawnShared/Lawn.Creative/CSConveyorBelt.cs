using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Lawn.Creative;

public class CSConveyorBelt : IComponent
{
	public List<(CardPair, int)> mAllowedPlants;

	public List<CardPair> mInitCards;

	public float mPrepareTimeMultiplier;

	public int Version { get; init; }

	public CSConveyorBelt()
	{
		mAllowedPlants = new List<(CardPair, int)>();
		mInitCards = null;
		mPrepareTimeMultiplier = 1f;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("AllowedPlants", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			IComponent.PrepareTwoIntListFromString(value, mAllowedPlants, (JsonElement item1, JsonElement item2) => (SeedTypeConvertor.ToEnum(item1.GetInt32(), Version), item2.GetInt32()));
		}
		if (root.TryGetProperty("InitCards", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			mInitCards = new List<CardPair>();
			IComponent.PrepareOneIntListFromString(value2, mInitCards, (JsonElement item) => SeedTypeConvertor.ToEnum(item.GetInt32(), Version));
		}
		if (root.TryGetProperty("PrepareTimeMultiplier", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mPrepareTimeMultiplier = value3.GetSingle();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (level.GetComponent<CSSeedBank>() != null)
		{
			throw new Exception("Can not exist together with SeedBank");
		}
		if (mAllowedPlants.Count < 1)
		{
			throw new Exception($"AllowedPlantsCount({mAllowedPlants.Count}) can not be smaller than 1");
		}
		if (mPrepareTimeMultiplier <= 0f)
		{
			throw new Exception($"PrepareTimeMultiplier({mPrepareTimeMultiplier}) can not be smaller than 0");
		}
	}
}
