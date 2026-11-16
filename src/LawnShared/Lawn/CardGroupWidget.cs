using System.Linq;
using Sexy;

namespace Lawn;

public class CardGroupWidget : Widget
{
	public LawnApp mApp;

	public SeedChooserScreen mListener;

	public int mCols;

	public int mSelectedGroupId;

	public CardGroupWidget(LawnApp theApp, SeedChooserScreen theListener)
	{
		mApp = theApp;
		mListener = theListener;
		mWidth = (mApp.mPlayerInfo.mClassicSeedChooser ? (3 * (AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() + 3) + AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth()) : (4 * Constants.SMALL_SEEDPACKET_WIDTH + 3 * Constants.SEED_PACKET_HORIZ_GAP));
		mHeight = (mApp.mPlayerInfo.mClassicSeedChooser ? (6 * AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight()) : (Constants.Is500pMode ? 344 : 450));
		mCols = 4;
		mSelectedGroupId = 0;
		mClip = false;
	}

	public override void MouseUp(int x, int y, int theBtnNum)
	{
		for (int i = 1; i <= mCols; i++)
		{
			GetCardGroupPosition(i, out var theX);
			if (x >= theX && x < theX + (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() : Constants.SMALL_SEEDPACKET_WIDTH))
			{
				if (mSelectedGroupId == i)
				{
					mSelectedGroupId = 0;
				}
				else
				{
					mSelectedGroupId = i;
				}
				break;
			}
		}
	}

	public override void Draw(Graphics g)
	{
		DrawGroups(g);
	}

	public void DrawGroups(Graphics g)
	{
		for (int i = 1; i <= mCols; i++)
		{
			DrawGroup(g, i, mSelectedGroupId == i);
		}
	}

	public void DrawGroup(Graphics g, int theGroupId, bool theDrawBack)
	{
		GetCardGroupPosition(theGroupId, out var theX);
		CardGroup cardGroup = GetCardGroup(theGroupId);
		int num = cardGroup.mCardInfo.Count((CardInfo item) => item.mSeedType != SeedType.None);
		if (num == 0)
		{
			for (int num2 = 0; num2 < 6; num2++)
			{
				int cardPositionInGroup = GetCardPositionInGroup(num2, 6);
				g.DrawImage(mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDPACKETSILHOUETTE : AtlasResources.IMAGE_SEEDPACKETSILHOUETTE, theX, cardPositionInGroup);
			}
		}
		else
		{
			for (int num3 = 0; num3 < num; num3++)
			{
				int cardPositionInGroup2 = GetCardPositionInGroup(num3, num);
				var (theSeedType, theImitaterType) = cardGroup.mCardInfo[num3];
				if (mApp.mPlayerInfo.mClassicSeedChooser)
				{
					SeedPacket.DrawClassicSeedPacket(g, theX, cardPositionInGroup2, theSeedType, theImitaterType, 0f, 255, theDrawCost: false, theUseCurrentCost: false);
				}
				else
				{
					SeedPacket.DrawSmallSeedPacket(g, theX, cardPositionInGroup2, theSeedType, theImitaterType, 0f, 255, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false);
				}
			}
		}
		if (theDrawBack)
		{
			int num4 = 5;
			int num5 = (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() : Constants.SMALL_SEEDPACKET_WIDTH);
			g.DrawImageBox(new TRect(theX - num4, -num4, num5 + num4 * 2, mHeight + num4 * 2), AtlasResources.IMAGE_SELECTED_PACKET);
		}
	}

	public CardGroup GetCardGroup()
	{
		return GetCardGroup(mSelectedGroupId);
	}

	public CardGroup GetCardGroup(int theGroup)
	{
		if (theGroup > 0 && theGroup < 5)
		{
			return mApp.mPlayerInfo.mCardGroup[theGroup];
		}
		return null;
	}

	public void GetCardGroupPosition(int theGroup, out int theX)
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			theX = (theGroup - 1) * (AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() + 3);
			return;
		}
		if (mCols == 1)
		{
			if (theGroup == 1)
			{
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 3 / 2;
				return;
			}
		}
		else if (mCols == 2)
		{
			switch (theGroup)
			{
			case 1:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 3 / 4;
				return;
			case 2:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 9 / 4;
				return;
			}
		}
		else if (mCols == 3)
		{
			switch (theGroup)
			{
			case 1:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) / 2;
				return;
			case 2:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 3 / 2;
				return;
			case 3:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 5 / 2;
				return;
			}
		}
		else if (mCols == 4)
		{
			switch (theGroup)
			{
			case 1:
				theX = 0;
				return;
			case 2:
				theX = Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH;
				return;
			case 3:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 2;
				return;
			case 4:
				theX = (Constants.SEED_PACKET_HORIZ_GAP + Constants.SMALL_SEEDPACKET_WIDTH) * 3;
				return;
			}
		}
		theX = 0;
	}

	public int GetCardPositionInGroup(int theCardIndex, int theCardCount)
	{
		if (theCardCount <= 1)
		{
			return 0;
		}
		return (mHeight - (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight() : Constants.SMALL_SEEDPACKET_HEIGHT)) * theCardIndex / (theCardCount - 1);
	}
}
