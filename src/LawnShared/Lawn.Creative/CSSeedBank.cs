using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Lawn.Creative;

public class CSSeedBank : IComponent
{
	public HashSet<SeedType> mBannedCards;

	public List<CardPair> mLockedCards;

	public int mNumPackets;

	public bool mReverseBanning;

	public bool mUserChoose;

	public int Version { get; init; }

	public CSSeedBank()
	{
		mNumPackets = -1;
		mBannedCards = null;
		mLockedCards = null;
		mUserChoose = true;
		mReverseBanning = false;
	}

	public void Init(JsonElement root)
	{
		if (root.TryGetProperty("NumPackets", out var value) && value.ValueKind != JsonValueKind.Null)
		{
			mNumPackets = value.GetInt32();
		}
		if (root.TryGetProperty("BannedCards", out var value2) && value2.ValueKind != JsonValueKind.Null)
		{
			List<SeedType> list = new List<SeedType>();
			IComponent.PrepareOneIntListFromString(value2, list, (JsonElement item) => SeedTypeConvertor.ToEnum(item.GetInt32(), out var _, Version));
			mBannedCards = list.ToHashSet();
		}
		if (root.TryGetProperty("LockedCards", out var value3) && value3.ValueKind != JsonValueKind.Null)
		{
			mLockedCards = new List<CardPair>();
			IComponent.PrepareOneIntListFromString(value3, mLockedCards, (JsonElement item) => SeedTypeConvertor.ToEnum(item.GetInt32(), Version));
		}
		if (root.TryGetProperty("UserChoose", out var value4) && value4.ValueKind != JsonValueKind.Null)
		{
			mUserChoose = value4.GetBoolean();
		}
		if (root.TryGetProperty("ReverseBanning", out var value5) && value5.ValueKind != JsonValueKind.Null)
		{
			mReverseBanning = value5.GetBoolean();
		}
	}

	public void Check(CreativeLevel level)
	{
		if (level.GetComponent<CSConveyorBelt>() != null)
		{
			throw new Exception("Can not exist together with ConveyorBelt");
		}
		if (mNumPackets < -1 || mNumPackets > 10)
		{
			throw new Exception($"NumPackets({mNumPackets}) out of range [-1,{10}]");
		}
	}
}
