using System;
using System.Text.Json;

namespace Lawn;

internal sealed class LocalMail
{
	internal MailType mType = MailType.Invalid;

	internal JsonElement mData = mEmptyData;

	internal bool mDeleteOnlyAfterRead;

	internal LocalMailReward[] mRewards = Array.Empty<LocalMailReward>();

	internal bool mSeen;

	internal int mReadOrder;

	private static readonly JsonElement mEmptyData = JsonSerializer.SerializeToElement(new object());

	internal LocalMail()
	{
	}

	internal LocalMail(MailType theType, JsonElement theData = default(JsonElement), bool theDeleteOnlyAfterRead = false, params (CoinType mType, int mCount)[] theRewards)
	{
		mType = theType;
		mData = ((theData.ValueKind == JsonValueKind.Undefined) ? mEmptyData : theData);
		mDeleteOnlyAfterRead = theDeleteOnlyAfterRead;
		mRewards = Array.ConvertAll(theRewards, ((CoinType mType, int mCount) aReward) => new LocalMailReward(aReward.mType, aReward.mCount));
	}

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncEnum(ref mType, theContext.aMailTypeSaver, SaveFileVersion.Initial, MailType.HelpZombieNote);
		string theStr = ((mData.ValueKind == JsonValueKind.Undefined) ? "{}" : mData.GetRawText());
		theContext.SyncString(ref theStr);
		if (theContext.mReading)
		{
			try
			{
				using JsonDocument jsonDocument = JsonDocument.Parse(theStr);
				mData = jsonDocument.RootElement.Clone();
			}
			catch (JsonException)
			{
				mData = default;
			}
		}
		theContext.SyncBool(ref mDeleteOnlyAfterRead);
		theContext.SyncBool(ref mSeen);
		theContext.SyncInt(ref mReadOrder);
		int theInt = mRewards.Length;
		theContext.SyncListLength(ref theInt);
		if (theContext.mReading)
		{
			mRewards = new LocalMailReward[theInt];
		}
		for (int i = 0; i < theInt; i++)
		{
			ref LocalMailReward reference = ref mRewards[i];
			if (reference == null)
			{
				reference = new LocalMailReward(CoinType.None);
			}
			mRewards[i].Sync(theContext);
		}
	}

	internal bool IsText()
	{
		return mType == MailType.Text;
	}

	internal bool IsSupported()
	{
		if (mType == MailType.Invalid || mData.ValueKind != JsonValueKind.Object)
		{
			return false;
		}
		if (IsText() && (!mData.TryGetProperty("TitleKey", out var value) || value.ValueKind != JsonValueKind.String || !mData.TryGetProperty("BodyKey", out var value2) || value2.ValueKind != JsonValueKind.String))
		{
			return false;
		}
		return Array.TrueForAll(mRewards, (LocalMailReward aReward) =>
		{
			switch (aReward.mType)
			{
			case CoinType.Silver:
			case CoinType.Gold:
			case CoinType.Diamond:
			case CoinType.PresentPlant:
			case CoinType.Chocolate:
			case CoinType.Award10PottedPlants:
				return true;
			default:
				return false;
			}
		});
	}

	internal bool HasUnclaimedRewards()
	{
		return Array.Exists(mRewards, (LocalMailReward aReward) => !aReward.mClaimed);
	}
}
