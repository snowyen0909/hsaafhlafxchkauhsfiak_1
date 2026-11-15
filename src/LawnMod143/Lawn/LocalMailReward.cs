using System;

namespace Lawn;

internal sealed class LocalMailReward
{
	internal CoinType mType;

	internal int mCount;

	internal bool mClaimed;

	internal LocalMailReward(CoinType theType, int theCount = 1)
	{
		if (theCount <= 0)
		{
			throw new ArgumentOutOfRangeException("theCount");
		}
		mType = theType;
		mCount = theCount;
	}

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncEnum(ref mType, theContext.aCoinTypeSaver, SaveFileVersion.Initial, CoinType.None);
		theContext.SyncInt(ref mCount);
		theContext.SyncBool(ref mClaimed);
	}
}
