namespace Lawn;

public sealed class CardInfo
{
	public SeedType mSeedType;

	public bool mImitater;

	public CardInfo()
	{
		mSeedType = SeedType.None;
		mImitater = false;
	}

	public CardInfo(SeedType theSeedType, bool theImitater)
	{
		mSeedType = theSeedType;
		mImitater = theImitater;
	}

	public CardInfo(SeedType theSeedType, SeedType theImitaterType)
	{
		Init(theSeedType, theImitaterType);
	}

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncEnum(ref mSeedType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncBool(ref mImitater);
	}

	public void Init()
	{
		mSeedType = SeedType.None;
		mImitater = false;
	}

	public void Init(SeedType theSeedType, SeedType theImitaterType)
	{
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			mSeedType = theImitaterType;
			mImitater = true;
		}
		else
		{
			mSeedType = theSeedType;
			mImitater = false;
		}
	}

	public void Deconstruct(out SeedType theSeedType, out SeedType theImitaterType)
	{
		if (mImitater)
		{
			theSeedType = SeedType.Imitater;
			theImitaterType = mSeedType;
		}
		else
		{
			theSeedType = mSeedType;
			theImitaterType = SeedType.None;
		}
	}
}
