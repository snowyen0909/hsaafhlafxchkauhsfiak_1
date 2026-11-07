namespace Lawn;

public struct CardPair
{
	public SeedType mSeedType;

	public bool mImitator;

	public static implicit operator CardPair(SeedType theType)
	{
		return new CardPair
		{
			mSeedType = theType,
			mImitator = false
		};
	}

	public void Deconstruct(out SeedType theType, out bool theImitator)
	{
		theType = mSeedType;
		theImitator = mImitator;
	}
}
