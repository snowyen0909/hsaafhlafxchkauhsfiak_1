namespace Lawn;

public sealed class CardGroup
{
	public readonly CardInfo[] mCardInfo;

	public CardGroup()
	{
		mCardInfo = new CardInfo[10];
		for (int i = 0; i < mCardInfo.Length; i++)
		{
			mCardInfo[i] = new CardInfo();
		}
	}

	internal void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		for (int i = 0; i < 10; i++)
		{
			mCardInfo[i].Sync(theContext);
		}
	}
}
