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
		if (!theIsImitaters && Tavel.Count > 0)
		{
			// plant-mod plants get rows of their own under the game's
			mRows = System.Math.Max(mRows, Tavel.FirstChooserCell / 4 + (Tavel.Count + 3) / 4);
		}
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
			int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			int num = 8;
			int num2 = (53 + num - 1) / num;
			if (!theIsImitaters && Tavel.Count > 0)
			{
				num2 = (Tavel.FirstChooserCell + Tavel.Count + num - 1) / num;
			}
			mWidth = (num - 1) * (celWidth + 3) + celWidth;
			mHeight = num2 * celHeight;
		}
		else
		{
			mWidth = 4 * Constants.SMALL_SEEDPACKET_WIDTH + 3 * Constants.SEED_PACKET_HORIZ_GAP;
			mHeight = Constants.SMALL_SEEDPACKET_HEIGHT * mRows + (mRows - 1) * Constants.SEED_PACKET_VERT_GAP + SEEDPACKETS_MARGIN_UP;
		}
	}

	/// <summary>Grid cell of a seed in the chooser / almanac grid.</summary>
	public static int GetSeedCell(SeedType theSeedType)
	{
		if ((int)theSeedType >= Tavel.FirstSeed)
		{
			int cell = Tavel.ChooserCell(theSeedType);
			return cell >= 0 ? cell : 0;
		}
		return (int)theSeedType;
	}

	public static SeedType GetSeedFromCell(int theCell)
	{
		if (theCell >= Tavel.FirstChooserCell)
		{
			return Tavel.SeedAtCell(theCell);
		}
		if (theCell < 0 || theCell >= (int)SeedType.ExplodeONut)
		{
			return SeedType.None;
		}
		return (SeedType)theCell;
	}

	public void GetSeedPosition(SeedType theSeedType, ref int theX, ref int theY)
	{
		int seedCell = GetSeedCell(theSeedType);
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			theX = seedCell % 8 * (AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() + 3);
			theY = seedCell / 8 * AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
		}
		else
		{
			theX = seedCell % 4 * (Constants.SMALL_SEEDPACKET_WIDTH + Constants.SEED_PACKET_HORIZ_GAP);
			theY = seedCell / 4 * (Constants.SMALL_SEEDPACKET_HEIGHT + Constants.SEED_PACKET_VERT_GAP) + SEEDPACKETS_MARGIN_UP;
		}
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
		int num = (mApp.mPlayerInfo.mClassicSeedChooser ? 8 : 4);
		int num2 = (mApp.mPlayerInfo.mClassicSeedChooser ? (AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() + 3) : (Constants.SMALL_SEEDPACKET_WIDTH + Constants.SEED_PACKET_HORIZ_GAP));
		int num3 = (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight() : (Constants.SMALL_SEEDPACKET_HEIGHT + Constants.SEED_PACKET_VERT_GAP));
		int num4 = ((!mApp.mPlayerInfo.mClassicSeedChooser) ? SEEDPACKETS_MARGIN_UP : 0);
		int num5 = x / num2;
		if (num5 >= num)
		{
			num5 = num - 1;
		}
		if (num5 < 0)
		{
			num5 = 0;
		}
		int num6 = (y - num4) / num3;
		SeedType unnamedParameter = GetSeedFromCell(num6 * num + num5);
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
			if (seedType == SeedType.Imitater || (mImitaters && Tavel.IsCustom(seedType)))
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
			if (!mApp.HasSeedType(seedType2) || (mImitaters && Tavel.IsCustom(seedType2)))
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
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			g.HardwareClip();
			foreach (SeedType seedType in SeedChooserScreen.ChooserSeedTypes)
			{
				if (seedType == SeedType.Imitater || (mImitaters && Tavel.IsCustom(seedType)))
				{
					continue;
				}
				int theX = 0;
				int theY = 0;
				GetSeedPosition(seedType, ref theX, ref theY);
				int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
				if (mParent != null && (theY + mY >= mParent.mHeight || theY + celHeight + mY <= 0))
				{
					continue;
				}
				if (!mApp.HasSeedType(seedType))
				{
					g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDPACKETSILHOUETTE, theX, theY);
					continue;
				}
				ChosenSeed chosenSeed = mApp.mSeedChooserScreen.mChosenSeeds[(int)seedType];
				bool flag;
				switch (chosenSeed.mSeedState)
				{
				case ChosenSeedState.SEED_FLYING_TO_BANK:
				case ChosenSeedState.SEED_FLYING_TO_CHOOSER:
				case ChosenSeedState.SEED_PACKET_HIDDEN:
					flag = true;
					break;
				default:
					flag = false;
					break;
				}
				if (!flag)
				{
					bool flag2 = chosenSeed.mSeedState != ChosenSeedState.SEED_IN_CHOOSER || mApp.mSeedChooserScreen.SeedNotRecommendedToPick(chosenSeed.mSeedType) != 0 || mApp.mSeedChooserScreen.SeedNotAllowedToPick(chosenSeed.mSeedType) || mApp.mSeedChooserScreen.SeedNotAllowedDuringTrial(chosenSeed.mSeedType);
					SeedPacket.DrawClassicSeedPacket(g, theX, theY, mImitaters ? SeedType.Imitater : chosenSeed.mSeedType, mImitaters ? chosenSeed.mSeedType : chosenSeed.mImitaterType, 0f, flag2 ? 115 : 255, theDrawCost: true, theUseCurrentCost: false);
				}
			}
			g.EndHardwareClip();
		}
		else
		{
			g.HardwareClip();
			DrawPackets(g, theDrawCost: false, theDrawBackground: true);
			DrawPackets(g, theDrawCost: true, theDrawBackground: false);
			g.EndHardwareClip();
		}
	}
}
