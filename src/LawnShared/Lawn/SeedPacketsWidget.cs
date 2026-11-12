using Sexy;

namespace Lawn;

public class SeedPacketsWidget : Widget
{
	public LawnApp mApp;

	public int mRows;

	public bool mImitaters;

	public SeedPacketsWidgetListener mListener;

	public static int SEEDPACKETS_MARGIN_UP = Constants.SMALL_SEEDPACKET_HEIGHT / 4;

	public SeedPacketsWidget(LawnApp theApp, int theNumberOfRows, bool theIsImitaters, SeedPacketsWidgetListener theListener)
	{
		mApp = theApp;
		mListener = theListener;
		mImitaters = theIsImitaters;
		mRows = theNumberOfRows;
		mWidth = 4 * Constants.SMALL_SEEDPACKET_WIDTH + 3 * Constants.SEED_PACKET_HORIZ_GAP;
		mHeight = Constants.SMALL_SEEDPACKET_HEIGHT * mRows + (mRows - 1) * Constants.SEED_PACKET_VERT_GAP + SEEDPACKETS_MARGIN_UP;
	}

	/// <summary>Grid cell of a seed: the original seeds use their enum value; iOS-port plants take the free cells after Imitater.</summary>
	public static int GetSeedCell(SeedType theSeedType)
	{
		if (theSeedType == SeedType.IrisShooter)
		{
			return (int)SeedType.ExplodeONut;
		}
		return (int)theSeedType;
	}

	public static SeedType GetSeedFromCell(int theCell)
	{
		if (theCell == (int)SeedType.ExplodeONut)
		{
			return SeedType.IrisShooter;
		}
		if (theCell < 0 || theCell > (int)SeedType.ExplodeONut)
		{
			return SeedType.None;
		}
		return (SeedType)theCell;
	}

	public void GetSeedPosition(SeedType theSeedType, ref int theX, ref int theY)
	{
		int seedCell = GetSeedCell(theSeedType);
		theX = seedCell % 4 * (Constants.SMALL_SEEDPACKET_WIDTH + Constants.SEED_PACKET_HORIZ_GAP);
		theY = seedCell / 4 * (Constants.SMALL_SEEDPACKET_HEIGHT + Constants.SEED_PACKET_VERT_GAP) + SEEDPACKETS_MARGIN_UP;
	}

	public override void Dispose()
	{
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (x < 0)
		{
			x = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		int num = x / (Constants.SMALL_SEEDPACKET_WIDTH + Constants.SEED_PACKET_HORIZ_GAP);
		if (num > 3)
		{
			num = 3;
		}
		if (num < 0)
		{
			num = 0;
		}
		int num2 = (y - SEEDPACKETS_MARGIN_UP) / (Constants.SMALL_SEEDPACKET_HEIGHT + Constants.SEED_PACKET_VERT_GAP);
		SeedType unnamedParameter = GetSeedFromCell(num2 * 4 + num);
		if (mImitaters && unnamedParameter == SeedType.IrisShooter)
		{
			unnamedParameter = SeedType.None;
		}
		mListener.SeedSelected(unnamedParameter);
	}

	public void DrawPackets(Graphics g, bool theDrawCost, bool theDrawBackground)
	{
		foreach (SeedType seedType in SeedChooserScreen.ChooserSeedTypes)
		{
			int i = (int)seedType;
			int theX = 0;
			int theY = 0;
			GetSeedPosition(seedType, ref theX, ref theY);
			if (seedType == SeedType.Imitater || (mImitaters && seedType == SeedType.IrisShooter))
			{
				continue;
			}
			if (mApp.HasSeedType(seedType))
			{
				ChosenSeed chosenSeed = mApp.mSeedChooserScreen.mChosenSeeds[i];
				if (chosenSeed.mSeedState != ChosenSeedState.SEED_IN_CHOOSER)
				{
					if (mImitaters)
					{
						SeedPacket.DrawSmallSeedPacket(g, theX, theY, SeedType.Imitater, seedType, 0f, 255, theDrawCost, theUseCurrentCost: false, theDrawBackground, theDrawBackground);
					}
					else
					{
						SeedPacket.DrawSmallSeedPacket(g, theX, theY, seedType, SeedType.None, 0f, 55, theDrawCost, theUseCurrentCost: false, theDrawBackground, theDrawBackground);
					}
				}
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKETSILHOUETTE, theX, theY);
			}
		}
		foreach (SeedType seedType2 in SeedChooserScreen.ChooserSeedTypes)
		{
			if (!mApp.HasSeedType(seedType2) || (mImitaters && seedType2 == SeedType.IrisShooter))
			{
				continue;
			}
			ChosenSeed chosenSeed2 = mApp.mSeedChooserScreen.mChosenSeeds[(int)seedType2];
			if (chosenSeed2.mSeedState != ChosenSeedState.SEED_FLYING_TO_CHOOSER && chosenSeed2.mSeedState != ChosenSeedState.SEED_FLYING_TO_BANK && chosenSeed2.mSeedState != ChosenSeedState.SEED_PACKET_HIDDEN && chosenSeed2.mSeedState == ChosenSeedState.SEED_IN_CHOOSER)
			{
				bool flag = false;
				if (mApp.mSeedChooserScreen.SeedNotRecommendedToPick(chosenSeed2.mSeedType) != 0)
				{
					flag = true;
				}
				else if (mApp.mSeedChooserScreen.SeedNotAllowedToPick(chosenSeed2.mSeedType))
				{
					flag = true;
				}
				else if (mApp.mSeedChooserScreen.SeedNotAllowedDuringTrial(chosenSeed2.mSeedType))
				{
					flag = true;
				}
				int theX2 = 0;
				int theY2 = 0;
				GetSeedPosition(chosenSeed2.mSeedType, ref theX2, ref theY2);
				float num = (flag ? 115 : 255);
				if (mImitaters)
				{
					SeedPacket.DrawSmallSeedPacket(g, theX2, theY2, SeedType.Imitater, chosenSeed2.mSeedType, 0f, (int)num, theDrawCost, theUseCurrentCost: false, theDrawBackground, theDrawBackground);
				}
				else
				{
					SeedPacket.DrawSmallSeedPacket(g, theX2, theY2, chosenSeed2.mSeedType, chosenSeed2.mImitaterType, 0f, (int)num, theDrawCost, theUseCurrentCost: false, theDrawBackground, theDrawBackground);
				}
			}
		}
	}

	public override void Draw(Graphics g)
	{
		g.HardwareClip();
		DrawPackets(g, theDrawCost: false, theDrawBackground: true);
		DrawPackets(g, theDrawCost: true, theDrawBackground: false);
		g.EndHardwareClip();
	}
}
