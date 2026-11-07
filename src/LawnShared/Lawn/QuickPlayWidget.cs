using System.Collections.Generic;
using Sexy;

namespace Lawn;

public class QuickPlayWidget : Widget
{
	public LawnApp mApp;

	public QuickPlayWidgetListener mListener;

	private List<int> mLevels = new List<int>();

	private static readonly ZombieDescriptor[] ZombieThumbTab;

	static QuickPlayWidget()
	{
		ZombieThumbTab = new ZombieDescriptor[255]
		{
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Normal, 110, 80),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.TrafficCone, 110, 70),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Normal, 110, 80),
			new ZombieDescriptor(ZombieType.Normal, 135, 80),
			new ZombieDescriptor(ZombieType.Normal, 160, 80),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Polevaulter, 30, 60),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Pail, 100, 65),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Newspaper, 100, 70),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Door, 100, 70),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Football, 90, 50),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Dancer, 90, 60),
			new ZombieDescriptor(ZombieType.BackupDancer, 140, 50),
			new ZombieDescriptor(ZombieType.BackupDancer, -89, 50),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.DuckyTube, 110, 70),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Snorkel, 110, 60),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Bobsled, 110, 60),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.DolphinRider, 110, 60),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.JackInTheBox, 110, 60),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Balloon, 80, 30),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Digger, 110, 50),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Pogo, 120, 60),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Bungee, 80, 35),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Ladder, 50, 30),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Bungee, 35, 54),
			new ZombieDescriptor(ZombieType.Bungee, 115, 45),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Catapult, 100, 70),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Gargantuar, 100, 50),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0),
			new ZombieDescriptor(ZombieType.Invalid, 0, 0)
		};
		for (int i = 0; i < ZombieThumbTab.Length; i++)
		{
			ZombieThumbTab[i].x = (int)Constants.InvertAndScale(ZombieThumbTab[i].x);
			ZombieThumbTab[i].y = (int)Constants.InvertAndScale(ZombieThumbTab[i].y);
		}
	}

	public QuickPlayWidget(LawnApp theApp, QuickPlayWidgetListener theListener)
	{
		mApp = theApp;
		mListener = theListener;
		mWidth = 0;
		mHeight = AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight;
	}

	public override void Draw(Graphics g)
	{
		int num = 10;
		for (int i = 0; i < mLevels.Count; i++)
		{
			int num2 = mLevels[i];
			DrawBackgroundThumbnailForLevel(g, num + Constants.QuickPlayWidget_Thumb_X, Constants.QuickPlayWidget_Thumb_Y, num2);
			if (num2 == 10 || num2 == 20 || num2 == 30 || num2 == 40 || num2 == 45)
			{
				g.SetColor(new SexyColor(0, 0, 0));
				g.FillRect(num + (int)Constants.InvertAndScale(25f), (int)Constants.InvertAndScale(24f), (int)Constants.InvertAndScale(27f), (int)Constants.InvertAndScale(139f));
				g.DrawImage(Resources.IMAGE_CONVEYORBELT_BACKDROP, num + (int)Constants.InvertAndScale(25f), (int)Constants.InvertAndScale(24f), (int)Constants.InvertAndScale(27f), (int)Constants.InvertAndScale(139f));
			}
			if (num2 == 40)
			{
				g.DrawImage(AtlasResources.IMAGE_RAIN, num + (int)Constants.InvertAndScale(33f), (int)Constants.InvertAndScale(19f));
				g.DrawImage(AtlasResources.IMAGE_RAIN, num + (int)Constants.InvertAndScale(93f), (int)Constants.InvertAndScale(30f));
				g.DrawImage(AtlasResources.IMAGE_RAIN, num + (int)Constants.InvertAndScale(60f), (int)Constants.InvertAndScale(60f), (int)Constants.InvertAndScale(120f), (int)Constants.InvertAndScale(120f));
			}
			num += AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10;
		}
		num = 10;
		for (int j = 0; j < mLevels.Count; j++)
		{
			int num3 = mLevels[j];
			for (int k = num3 * 5; ZombieThumbTab[k].type != ZombieType.Invalid; k++)
			{
				int num4 = ZombieThumbTab[k].x;
				bool mirror = false;
				if (num4 < 0)
				{
					num4 = -num4;
					mirror = true;
				}
				if (ZombieThumbTab[k].type == ZombieType.Bungee)
				{
					g.DrawImage(AtlasResources.IMAGE_BUNGEECORD, (float)(num + num4) + Constants.InvertAndScale(33f), ZombieThumbTab[k].y + Constants.QuickPlayWidget_Bungee_Y);
				}
				DrawZombieThumbnail(g, ZombieThumbTab[k].type, num + num4, ZombieThumbTab[k].y, mirror);
			}
			num += AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10;
		}
		num = 10;
		for (int l = 0; l < mLevels.Count; l++)
		{
			g.DrawImage(AtlasResources.IMAGE_MINI_GAME_FRAME, num, 0);
			num += AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10;
		}
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		if (mLevels.Count != 0)
		{
			int index = x / (AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth + 10);
			mListener.QuickPlayStageSelected(mLevels[index]);
		}
	}

	public void AddLevel(int theLevel)
	{
		mLevels.Add(theLevel);
	}

	public void Clear()
	{
		mLevels.Clear();
	}

	public void SizeToFit()
	{
		mWidth = (10 + AtlasResources.IMAGE_MINI_GAME_FRAME.mWidth) * mLevels.Count;
		mHeight = AtlasResources.IMAGE_MINI_GAME_FRAME.mHeight;
	}

	public void DrawBackgroundThumbnailForLevel(Graphics g, int theX, int theY, int theLevel)
	{
		Image theImage = null;
		if (theLevel == 5)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_BOWLING;
		}
		else if (theLevel <= 10)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_BACKGROUND1_THUMB;
		}
		else if (theLevel == 15)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_WACK;
		}
		else if (theLevel <= 20)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_BACKGROUND2_THUMB;
		}
		else if (theLevel == 25)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_LITTLE_TROUBLE;
		}
		else if (theLevel <= 30)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_BACKGROUND3_THUMB;
		}
		else if (theLevel == 35)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_VASES;
		}
		else if (theLevel <= 40)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_BACKGROUND4_THUMB;
		}
		else if (theLevel < 50)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_BACKGROUND5_THUMB;
		}
		else if (theLevel == 50)
		{
			theImage = AtlasResources.IMAGE_QUICKPLAY_ZOMBOSS;
		}
		g.DrawImage(theImage, theX, theY);
	}

	public void DrawZombieThumbnail(Graphics g, ZombieType theZombieType, int theX, int theY)
	{
		DrawZombieThumbnail(g, theZombieType, theX, theY, mirror: false);
	}

	public void DrawZombieThumbnail(Graphics g, ZombieType theZombieType, int theX, int theY, bool mirror)
	{
		float mScaleX = g.mScaleX;
		if (mirror)
		{
			g.mScaleX = 0f - g.mScaleX;
		}
		mApp.mReanimatorCache.DrawCachedZombieThumbnail(g, theX, theY, theZombieType);
		g.mScaleX = mScaleX;
	}
}
