using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ZombiePileWidget : Widget, ButtonListener
{
	private struct Star(Point pos, float size, int cel)
	{
		public Point pos = pos;

		public float size = size;

		public int cel = cel;
	}

	public const float starParallax = 0.2f;

	public const float pileObjectParallax = 0.4f;

	public LawnApp mApp;

	public DialogButton mVasebreakerButton;

	public DialogButton mIZombieButton;

	public DialogButton mZombiesKilledButton;

	public NewLawnButton mBackButton;

	public TriVertex skyBottomLeft;

	public TriVertex skyBottomRight;

	public TriVertex transferBottomLeft;

	public TriVertex transferBottomRight;

	public TriVertex spaceBottomLeft;

	public TriVertex spaceBottomRight;

	public TriVertex spaceTopLeft;

	public TriVertex spaceTopRight;

	public TriVertex screenBottomRight;

	public TriVertex screenBottomLeft;

	public Color spaceColor;

	public Color skyColor;

	public static int maxHeight = 200;

	public static int skyEndHeight = 30;

	public static int spaceStartHeight = 75;

	public static int mZombieSpace = 140;

	public static int mZombieScale = 125;

	public int mScreenTop;

	public int mPileHeight;

	private bool mGrayed;

	private static Color mGray = new Color(0, 0, 0, 150);

	private VertexPositionTexture[] blackHoleVerts = new VertexPositionTexture[4]
	{
		new VertexPositionTexture(new Vector3((float)(-Resources.IMAGE_BLACKHOLE.mWidth) / 4f, (float)(-Resources.IMAGE_BLACKHOLE.mHeight) / 4f, 0f), new Vector2(0f, 1f)),
		new VertexPositionTexture(new Vector3((float)Resources.IMAGE_BLACKHOLE.mWidth / 4f, (float)(-Resources.IMAGE_BLACKHOLE.mHeight) / 4f, 0f), new Vector2(1f, 1f)),
		new VertexPositionTexture(new Vector3((float)(-Resources.IMAGE_BLACKHOLE.mWidth) / 4f, (float)Resources.IMAGE_BLACKHOLE.mHeight / 4f, 0f), new Vector2(0f, 0f)),
		new VertexPositionTexture(new Vector3((float)Resources.IMAGE_BLACKHOLE.mWidth / 4f, (float)Resources.IMAGE_BLACKHOLE.mHeight / 4f, 0f), new Vector2(1f, 0f))
	};

	private short[] blackHoleIndices = new short[6] { 0, 1, 2, 1, 3, 2 };

	private Matrix blackHoleView = Matrix.CreateLookAt(new Vector3(0f, 0f, -1000f), Vector3.Zero, Vector3.UnitY);

	private Matrix blackHoleProjection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1f, 1f, 5000f);

	public ZombiePileMarker[] friendMarkers;

	public bool mLoadedFriends;

	private Star[] stars;

	private Random rand = new Random();

	public ZombiePileObject[] gPileObjects;

	private static int SortGems(ZombiePileObject a, ZombiePileObject b)
	{
		return (int)(100f * (Math.Abs(a.gemSpeedX) - Math.Abs(b.gemSpeedX)));
	}

	public ZombiePileWidget(LawnApp theApp)
	{
		mApp = theApp;
		mWidth = Constants.BOARD_WIDTH;
		mPileHeight = (int)(mApp.mPlayerInfo.mZombiesKilled / mZombieScale);
		mPileHeight = Math.Min(mPileHeight, maxHeight);
		mHeight = Constants.BOARD_HEIGHT + CalculatePileHeight(mPileHeight) + 300;
		mY = -mHeight;
		mScreenTop = mHeight - Constants.BOARD_HEIGHT;
		List<ZombiePileObject> list = new List<ZombiePileObject>
		{
			new ZombiePileObject(5, ZombiePileObjectType.YellowCloud),
			new ZombiePileObject(15, ZombiePileObjectType.YellowCloud),
			new ZombiePileObject(30, ZombiePileObjectType.YellowCloud),
			new ZombiePileObject(45, ZombiePileObjectType.Airplane),
			new ZombiePileObject(50, ZombiePileObjectType.YellowCloud),
			new ZombiePileObject(60, ZombiePileObjectType.YellowCloud),
			new ZombiePileObject(85, ZombiePileObjectType.Satellite),
			new ZombiePileObject(110, ZombiePileObjectType.Moon),
			new ZombiePileObject(140, ZombiePileObjectType.Astronaut),
			new ZombiePileObject(160, ZombiePileObjectType.PeggleUrsamajor)
		};
		List<ZombiePileObject> list2 = new List<ZombiePileObject>();
		for (int i = 0; i < 100; i++)
		{
			list2.Add(new ZombiePileObject(180 + rand.Next(4) - 2, (ZombiePileObjectType)(9 + rand.Next(7))));
		}
		list2.Sort(SortGems);
		list.AddRange(list2);
		list.Add(new ZombiePileObject(200, ZombiePileObjectType.Blackhole));
		if (!theApp.mPlayerInfo.mSeenLeaderboardArrow && mPileHeight >= 1)
		{
			list.Add(new ZombiePileObject(0, ZombiePileObjectType.Arrow));
			theApp.mPlayerInfo.mSeenLeaderboardArrow = true;
		}
		gPileObjects = list.ToArray();
		SetObjectYVals();
		spaceColor = Color.Black;
		skyColor = new Color(134, 169, 197);
		int num = Constants.BOARD_HEIGHT + CalculatePileHeight(maxHeight);
		int num2 = Constants.BOARD_HEIGHT + CalculatePileHeight(skyEndHeight);
		int num3 = Constants.BOARD_HEIGHT + CalculatePileHeight(spaceStartHeight);
		skyBottomLeft = new TriVertex(-1f, mScreenTop);
		skyBottomRight = new TriVertex(Constants.BOARD_WIDTH, mScreenTop);
		transferBottomLeft = new TriVertex(-1f, mHeight - num2);
		transferBottomRight = new TriVertex(Constants.BOARD_WIDTH, mHeight - num2);
		spaceBottomLeft = new TriVertex(-1f, mHeight - num3);
		spaceBottomRight = new TriVertex(Constants.BOARD_WIDTH, mHeight - num3);
		spaceTopLeft = new TriVertex(-1f, mHeight - num);
		spaceTopRight = new TriVertex(Constants.BOARD_WIDTH, mHeight - num);
		screenBottomLeft = new TriVertex(-1f, mHeight);
		screenBottomRight = new TriVertex(Constants.BOARD_WIDTH, mHeight);
		mVasebreakerButton = GameButton.MakeNewButton(3, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_VASEBREAKER, null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_VASEBREAKER_HIGHLIGHT);
		mVasebreakerButton.Resize(Constants.LeaderboardScreen_Vasebreaker_Button_X, mScreenTop + Constants.LeaderboardScreen_Vasebreaker_Button_Y, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_VASEBREAKER_HIGHLIGHT.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_VASEBREAKER_HIGHLIGHT.mHeight);
		mVasebreakerButton.mTranslateX = (mVasebreakerButton.mTranslateY = 0);
		mIZombieButton = GameButton.MakeNewButton(4, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_IZOMBIE, null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_IZOMBIE_HIGHLIGHT);
		mIZombieButton.Resize(Constants.LeaderboardScreen_IZombie_Button_X, mScreenTop + Constants.LeaderboardScreen_IZombie_Button_Y, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_IZOMBIE_HIGHLIGHT.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_IZOMBIE_HIGHLIGHT.mHeight);
		mIZombieButton.mTranslateX = (mIZombieButton.mTranslateY = 0);
		mZombiesKilledButton = GameButton.MakeNewButton(5, this, "", null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_KILLED, null, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_KILLED_HIGHLIGHT);
		mZombiesKilledButton.Resize(Constants.LeaderboardScreen_Killed_Button_X, mScreenTop + Constants.LeaderboardScreen_Killed_Button_Y, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_KILLED_HIGHLIGHT.mWidth, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_LEADERBOARD_KILLED_HIGHLIGHT.mHeight);
		mZombiesKilledButton.mTranslateX = (mZombiesKilledButton.mTranslateY = 0);
		mBackButton = GameButton.MakeNewButton(0, this, "", null, AtlasResources.IMAGE_PILE_LEADERBOARD_BACK_BUTTON, AtlasResources.IMAGE_PILE_LEADERBOARD_BACK_BUTTON, AtlasResources.IMAGE_PILE_LEADERBOARD_BACK_BUTTON_HIGHLIGHT);
		mBackButton.Resize(0, mScreenTop + Constants.BOARD_HEIGHT - AtlasResources.IMAGE_PILE_LEADERBOARD_BACK_BUTTON.mHeight, AtlasResources.IMAGE_PILE_LEADERBOARD_BACK_BUTTON.mWidth, AtlasResources.IMAGE_PILE_LEADERBOARD_BACK_BUTTON.mHeight);
		mBackButton.mTranslateX = (mBackButton.mTranslateY = 0);
		AddWidget(mVasebreakerButton);
		AddWidget(mIZombieButton);
		AddWidget(mZombiesKilledButton);
		AddWidget(mBackButton);
		LeaderBoardComm.LoadResults(LeaderboardGameMode.Adventure);
		friendMarkers = new ZombiePileMarker[10];
		int maxValue = (int)((double)num3 * 1.75 * 0.20000000298023224);
		stars = new Star[mPileHeight * 10];
		for (int j = 0; j < stars.Length; j++)
		{
			stars[j] = new Star(new Point(rand.Next(mWidth), rand.Next(maxValue)), (float)rand.NextDouble() / 2f, rand.Next(3));
		}
	}

	public override bool DoScroll(_Touch touch)
	{
		CGPoint absPos = GetAbsPos();
		int theX = (int)(touch.location.x - absPos.x);
		int theY = (int)(touch.location.y - absPos.y);
		if (!mBackButton.Contains(theX, theY) && !mIZombieButton.Contains(theX, theY) && !mZombiesKilledButton.Contains(theX, theY))
		{
			return !mVasebreakerButton.Contains(theX, theY);
		}
		return false;
	}

	public override bool DoScroll(int x, int y)
	{
		CGPoint absPos = GetAbsPos();
		int theX = (int)((float)x - absPos.x);
		int theY = (int)((float)y - absPos.y);
		if (!mBackButton.Contains(theX, theY) && !mIZombieButton.Contains(theX, theY) && !mZombiesKilledButton.Contains(theX, theY))
		{
			return !mVasebreakerButton.Contains(theX, theY);
		}
		return false;
	}

	public void SetGray(bool aGrayed)
	{
		mGrayed = aGrayed;
	}

	public int CalculatePileHeight(int height)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < height; i++)
		{
			num++;
			switch (num)
			{
			case 1:
				num2 += AtlasResources.IMAGE_PILE_ZOMBIE_PILE_1A.mHeight;
				break;
			case 2:
				num2 += AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2A.mHeight;
				break;
			case 3:
				num2 += AtlasResources.IMAGE_PILE_ZOMBIE_PILE_1B.mHeight;
				break;
			case 4:
				num2 += AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2B.mHeight;
				break;
			}
			if (num >= 4)
			{
				num = 0;
			}
			num2 -= mZombieSpace;
		}
		return num2;
	}

	public override void Update()
	{
		base.Update();
		UpdatePileObjects();
		if (mLoadedFriends)
		{
			return;
		}
		int num = LeaderBoardComm.LoadResults(LeaderboardGameMode.Adventure);
		if (num < 0)
		{
			return;
		}
		int signedInGamerIndex = LeaderBoardComm.GetSignedInGamerIndex(LeaderboardState.Adventure);
		int num2 = LeaderBoardComm.LoadResults(LeaderboardGameMode.Adventure);
		long leaderboardScore = LeaderBoardComm.GetLeaderboardScore(signedInGamerIndex, LeaderboardState.Adventure);
		if (leaderboardScore > mApp.mPlayerInfo.mZombiesKilled)
		{
			mApp.mPlayerInfo.mZombiesKilled = leaderboardScore;
		}
		bool flag = false;
		int num3 = 1;
		int num4 = 0;
		while (!flag)
		{
			if (signedInGamerIndex + num3 < num2)
			{
				Gamer leaderboardGamer = LeaderBoardComm.GetLeaderboardGamer(signedInGamerIndex + num3, LeaderboardState.Adventure);
				int val = (int)(LeaderBoardComm.GetLeaderboardScore(signedInGamerIndex + num3, LeaderboardState.Adventure) / mZombieScale);
				val = Math.Min(val, maxHeight);
				ZombiePileMarker zombiePileMarker = new ZombiePileMarker();
				zombiePileMarker.mGamer = leaderboardGamer;
				zombiePileMarker.mHeight = val;
				friendMarkers[num4] = zombiePileMarker;
				num4++;
			}
			if (num4 >= 5)
			{
				break;
			}
			if (signedInGamerIndex - num3 >= 0)
			{
				Gamer leaderboardGamer2 = LeaderBoardComm.GetLeaderboardGamer(signedInGamerIndex - num3, LeaderboardState.Adventure);
				int val2 = (int)(LeaderBoardComm.GetLeaderboardScore(signedInGamerIndex - num3, LeaderboardState.Adventure) / mZombieScale);
				val2 = Math.Min(val2, maxHeight);
				ZombiePileMarker zombiePileMarker2 = new ZombiePileMarker();
				zombiePileMarker2.mGamer = leaderboardGamer2;
				zombiePileMarker2.mHeight = val2;
				friendMarkers[num4] = zombiePileMarker2;
				num4++;
			}
			if (num4 >= 5 || (signedInGamerIndex - num3 <= 0 && signedInGamerIndex + num3 > num2))
			{
				flag = true;
			}
			num3++;
		}
		mLoadedFriends = true;
	}

	public override void DrawOverlay(Graphics g)
	{
	}

	public override void Draw(Graphics g)
	{
		g.DrawImage(Resources.IMAGE_LEADERBOARDSCREEN_BACKGROUND, 0, mScreenTop);
		g.DrawTriangle(skyBottomLeft, skyColor, skyBottomRight, skyColor, transferBottomLeft, skyColor, Graphics.DrawMode.DRAWMODE_NORMAL);
		g.DrawTriangle(skyBottomRight, skyColor, transferBottomRight, skyColor, transferBottomLeft, skyColor, Graphics.DrawMode.DRAWMODE_NORMAL);
		g.DrawTriangle(transferBottomLeft, skyColor, transferBottomRight, skyColor, spaceBottomLeft, skyColor, Graphics.DrawMode.DRAWMODE_NORMAL);
		g.DrawTriangle(transferBottomRight, skyColor, spaceBottomRight, skyColor, spaceBottomLeft, skyColor, Graphics.DrawMode.DRAWMODE_NORMAL);
		g.DrawTriangle(spaceBottomLeft, spaceColor, spaceBottomRight, spaceColor, spaceTopLeft, spaceColor, Graphics.DrawMode.DRAWMODE_NORMAL);
		g.DrawTriangle(spaceBottomRight, spaceColor, spaceTopRight, spaceColor, spaceTopLeft, spaceColor, Graphics.DrawMode.DRAWMODE_NORMAL);
		g.DrawImage(Resources.IMAGE_EDGE_OF_SPACE, 0, (int)spaceBottomLeft.y - Resources.IMAGE_EDGE_OF_SPACE.mHeight / 2, Constants.BOARD_WIDTH, Resources.IMAGE_EDGE_OF_SPACE.mHeight);
		float y = spaceBottomRight.y;
		int bOARD_HEIGHT = Constants.BOARD_HEIGHT;
		int alpha = 255;
		g.SetColorizeImages(colorizeImages: true);
		Color color = new Color(255, 255, 255, alpha);
		g.SetColor(color);
		int mTransY = g.mTransY;
		g.mTransY = (int)((float)mTransY * 0.2f);
		for (int i = 0; i < stars.Length; i++)
		{
			g.SetScale(stars[i].size);
			color.A = (byte)(200 + rand.Next(55));
			g.SetColor(color);
			g.DrawImageCel(AtlasResources.IMAGE_ICE_SPARKLES, stars[i].pos.X, stars[i].pos.Y, stars[i].cel, 0);
		}
		g.SetScale(1f);
		g.SetColor(Color.White);
		g.mTransY = mTransY;
		DrawPileObjects(g);
		int num = mScreenTop - 100;
		int num2 = 0;
		int num3 = 0;
		for (int j = 0; j < mPileHeight; j++)
		{
			num2++;
			switch (num2)
			{
			case 1:
				g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_1A, Constants.Leaderboard_Pile_1_X, num);
				num3 = AtlasResources.IMAGE_PILE_ZOMBIE_PILE_1A.mHeight;
				break;
			case 2:
				g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2A, Constants.Leaderboard_Pile_1_X, num);
				num3 = AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2A.mHeight;
				break;
			case 3:
				g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_1B, Constants.Leaderboard_Pile_1_X, num);
				num3 = AtlasResources.IMAGE_PILE_ZOMBIE_PILE_1B.mHeight;
				break;
			case 4:
				g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2B, Constants.Leaderboard_Pile_1_X, num);
				num3 = AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2B.mHeight;
				break;
			}
			if (num2 >= 4)
			{
				num2 = 0;
			}
			num -= num3 - mZombieSpace;
		}
		if (mPileHeight == maxHeight)
		{
			g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_TOP, Constants.Leaderboard_Pile_1_X + 35, num - 20);
		}
		g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_2B, Constants.Leaderboard_Pile_1_X, mScreenTop - 80);
		g.DrawImage(AtlasResources.IMAGE_PILE_ZOMBIE_PILE_BASE, Constants.LeaderboardScreen_PileBase_X, mScreenTop + Constants.LeaderboardScreen_PileBase_Y);
		g.DrawImage(AtlasResources.IMAGE_PILE_CLOUD_RING, 0, (int)spaceBottomLeft.y - Resources.IMAGE_EDGE_OF_SPACE.mHeight / 2 + Constants.LeaderboardScreen_EdgeOfSpace_Overlay_Offset, Constants.BOARD_WIDTH, Resources.IMAGE_EDGE_OF_SPACE.mHeight);
		if (mLoadedFriends)
		{
			DrawPileMarkers(g);
		}
		DeferOverlay();
	}

	public void DrawPileMarkers(Graphics g)
	{
		for (int i = 0; i < friendMarkers.Length; i++)
		{
			if (friendMarkers[i] != null && friendMarkers[i].mGamer != null)
			{
				int num = mScreenTop - CalculatePileHeight(friendMarkers[i].mHeight) - AtlasResources.IMAGE_PILE_SIGN_OVERLAY.mHeight / 2;
				g.DrawImage(AtlasResources.IMAGE_PILE_BALLOON, 108, num - 152);
				g.DrawImage(AtlasResources.IMAGE_PILE_SIGN_OVERLAY, 0, num);
				Image gamerImage = LeaderBoardComm.GetGamerImage(friendMarkers[i].mGamer);
				if (gamerImage != null)
				{
					g.DrawImage(gamerImage, 51, num + 61);
				}
				TodCommon.TodDrawString(g, friendMarkers[i].mGamer.Gamertag, 126, num + 79, Resources.FONT_DWARVENTODCRAFT15, SexyColor.White, 211, DrawStringJustification.Left);
			}
		}
	}

	public void SetObjectYVals()
	{
		for (int i = 0; i < gPileObjects.Length; i++)
		{
			gPileObjects[i].mY = (int)((float)mScreenTop * 0.4f - (float)CalculatePileHeight(gPileObjects[i].mHeight) * 0.4f);
			if (gPileObjects[i].mType == ZombiePileObjectType.Arrow)
			{
				gPileObjects[i].mY = mScreenTop - CalculatePileHeight(gPileObjects[i].mHeight);
			}
		}
	}

	public void DrawPileObjects(Graphics g)
	{
		int mTransY = g.mTransY;
		g.mTransY = (int)((float)mTransY * 0.4f);
		for (int i = 0; i < gPileObjects.Length; i++)
		{
			ZombiePileObject zombiePileObject = gPileObjects[i];
			if (zombiePileObject.mHeight <= mPileHeight)
			{
				switch (zombiePileObject.mType)
				{
				case ZombiePileObjectType.Balloon:
					g.DrawImage(AtlasResources.IMAGE_PILE_BALLOON, 40f, (float)zombiePileObject.mY + zombiePileObject.mOffsetY);
					break;
				case ZombiePileObjectType.YellowCloud:
					g.DrawImage(AtlasResources.IMAGE_PILE_YELLOW_CLOUD, (int)zombiePileObject.mOffsetX, zombiePileObject.mY + (int)zombiePileObject.mOffsetY, AtlasResources.IMAGE_PILE_YELLOW_CLOUD.mWidth, AtlasResources.IMAGE_PILE_YELLOW_CLOUD.mHeight);
					break;
				case ZombiePileObjectType.Airplane:
					g.DrawImage(AtlasResources.IMAGE_PILE_AIRPLANE, zombiePileObject.mOffsetX, (float)zombiePileObject.mY + zombiePileObject.mOffsetY);
					break;
				case ZombiePileObjectType.Moon:
					g.DrawImage(AtlasResources.IMAGE_PILE_MOON, 40, zombiePileObject.mY);
					break;
				case ZombiePileObjectType.Satellite:
					g.DrawImage(AtlasResources.IMAGE_PILE_SATELLITE, zombiePileObject.mOffsetX, (float)zombiePileObject.mY + zombiePileObject.mOffsetY);
					break;
				case ZombiePileObjectType.PeggleUrsamajor:
					g.DrawImage(AtlasResources.IMAGE_PILE_PEGGLE_URSAMAJOR, 30, zombiePileObject.mY);
					break;
				case ZombiePileObjectType.Astronaut:
				{
					Graphics graphics = Graphics.GetNew(g);
					graphics.mTransX += (int)zombiePileObject.mOffsetX;
					graphics.mTransY += zombiePileObject.mY + (int)zombiePileObject.mOffsetY;
					zombiePileObject.mReanim.Draw(graphics);
					graphics.PrepareForReuse();
					break;
				}
				case ZombiePileObjectType.Blackhole:
				{
					Matrix identity = Matrix.Identity;
					new TRect(0, 0, Resources.IMAGE_BLACKHOLE.mWidth, Resources.IMAGE_BLACKHOLE.mHeight);
					int num2 = (int)((float)g.mTransY / 0.4f * 1.6f);
					blackHoleView = Matrix.CreateLookAt(new Vector3(0f, -num2, -1000f), Vector3.Zero, Vector3.UnitY);
					blackHoleVerts[0].Position.Y = (float)(-Resources.IMAGE_BLACKHOLE.mHeight) / 4f - (float)num2;
					blackHoleVerts[1].Position.Y = (float)(-Resources.IMAGE_BLACKHOLE.mHeight) / 4f - (float)num2;
					blackHoleVerts[2].Position.Y = (float)Resources.IMAGE_BLACKHOLE.mHeight / 4f - (float)num2;
					blackHoleVerts[3].Position.Y = (float)Resources.IMAGE_BLACKHOLE.mHeight / 4f - (float)num2;
					Vector3 vector = new Vector3(0f, num2, 0f);
					Vector3 position = new Vector3(-60f, 80f, 0f);
					identity = Matrix.CreateTranslation(vector) * Matrix.CreateScale(4f) * Matrix.CreateRotationZ(zombiePileObject.mCounter) * Matrix.CreateRotationX(-0.85f) * Matrix.CreateTranslation(-vector) * Matrix.CreateTranslation(position);
					g.DrawImageWithBasicEffect(Resources.IMAGE_BLACKHOLE, blackHoleVerts, blackHoleIndices, identity, blackHoleView, blackHoleProjection);
					break;
				}
				case ZombiePileObjectType.Arrow:
					g.DrawImageRotatedScaled(AtlasResources.IMAGE_DOWNARROW, 160f, zombiePileObject.mY + (int)zombiePileObject.mOffsetY + mTransY + 50, Math.PI, AtlasResources.IMAGE_DOWNARROW.mWidth / 2, AtlasResources.IMAGE_DOWNARROW.mHeight / 2, new TRect(0, 0, AtlasResources.IMAGE_DOWNARROW.mWidth, AtlasResources.IMAGE_DOWNARROW.mHeight), 10, 10);
					break;
				case ZombiePileObjectType.Gem0:
				case ZombiePileObjectType.Gem1:
				case ZombiePileObjectType.Gem2:
				case ZombiePileObjectType.Gem3:
				case ZombiePileObjectType.Gem4:
				case ZombiePileObjectType.Gem5:
				case ZombiePileObjectType.Gem6:
				{
					float num = Math.Abs(zombiePileObject.gemSpeedX / 5f);
					num = 1f / num;
					g.DrawImageRotatedScaled(zombiePileObject.gemImage, (int)zombiePileObject.mOffsetX + g.mTransX, zombiePileObject.mY + (int)zombiePileObject.mOffsetY + g.mTransY, zombiePileObject.mCounter, (int)((float)zombiePileObject.gemImage.mWidth / num), (int)((float)zombiePileObject.gemImage.mHeight / num), new TRect(0, 0, zombiePileObject.gemImage.mWidth, zombiePileObject.gemImage.mHeight), (int)(num * (float)zombiePileObject.gemImage.mWidth), (int)(num * (float)zombiePileObject.gemImage.mHeight));
					break;
				}
				}
			}
		}
		g.mTransY = mTransY;
	}

	public void UpdatePileObjects()
	{
		for (int i = 0; i < gPileObjects.Length; i++)
		{
			ZombiePileObject zombiePileObject = gPileObjects[i];
			if (zombiePileObject.mHeight > mPileHeight)
			{
				continue;
			}
			switch (zombiePileObject.mType)
			{
			case ZombiePileObjectType.Balloon:
				zombiePileObject.mCounter += 0.1f;
				if ((double)zombiePileObject.mCounter > Math.PI * 2.0)
				{
					zombiePileObject.mCounter -= (float)Math.PI * 2f;
				}
				zombiePileObject.mOffsetY = (int)(Math.Sin(zombiePileObject.mCounter) * 5.0);
				break;
			case ZombiePileObjectType.YellowCloud:
				zombiePileObject.mCounter--;
				zombiePileObject.mOffsetX = (int)zombiePileObject.mCounter;
				if (zombiePileObject.mOffsetX < (float)(-AtlasResources.IMAGE_PILE_YELLOW_CLOUD.mWidth))
				{
					zombiePileObject.mCounter = Constants.BOARD_WIDTH;
					zombiePileObject.mOffsetX = (int)zombiePileObject.mCounter;
				}
				break;
			case ZombiePileObjectType.Airplane:
				zombiePileObject.mOffsetX += 6f;
				zombiePileObject.mOffsetY += zombiePileObject.mCounter;
				zombiePileObject.mCounter -= 0.01f * zombiePileObject.mCounter;
				if (zombiePileObject.mOffsetX > (float)Constants.BOARD_WIDTH)
				{
					zombiePileObject.mOffsetX = -2 * AtlasResources.IMAGE_PILE_AIRPLANE.mWidth;
					zombiePileObject.mOffsetY = -100f;
					zombiePileObject.mCounter = 3f;
				}
				break;
			case ZombiePileObjectType.Satellite:
				zombiePileObject.mOffsetX -= 9f;
				zombiePileObject.mOffsetY++;
				if (zombiePileObject.mOffsetX < (float)(-AtlasResources.IMAGE_PILE_SATELLITE.mWidth))
				{
					zombiePileObject.mOffsetX = Constants.BOARD_WIDTH + AtlasResources.IMAGE_PILE_SATELLITE.mWidth;
					zombiePileObject.mOffsetY = -50f;
				}
				break;
			case ZombiePileObjectType.Astronaut:
				zombiePileObject.mReanim.Update();
				zombiePileObject.mCounter += 0.025f;
				if ((double)zombiePileObject.mCounter > Math.PI * 2.0)
				{
					zombiePileObject.mCounter -= (float)Math.PI * 2f;
				}
				zombiePileObject.mOffsetY = -(int)(Math.Sin(zombiePileObject.mCounter) * 80.0);
				zombiePileObject.mOffsetX -= 6f;
				if (zombiePileObject.mReanim.mLastFrameTime > 0.95f)
				{
					zombiePileObject.mOffsetX = 600f;
					zombiePileObject.mCounter = 0f;
				}
				break;
			case ZombiePileObjectType.Blackhole:
				zombiePileObject.mCounter += 0.05f;
				if ((double)zombiePileObject.mCounter > Math.PI * 2.0)
				{
					zombiePileObject.mCounter -= (float)Math.PI * 2f;
				}
				break;
			case ZombiePileObjectType.Arrow:
				zombiePileObject.mCounter += 0.1f;
				if ((double)zombiePileObject.mCounter > Math.PI * 2.0)
				{
					zombiePileObject.mCounter -= (float)Math.PI * 2f;
				}
				zombiePileObject.mOffsetY = (int)(Math.Sin(zombiePileObject.mCounter) * 10.0);
				break;
			case ZombiePileObjectType.Gem0:
			case ZombiePileObjectType.Gem1:
			case ZombiePileObjectType.Gem2:
			case ZombiePileObjectType.Gem3:
			case ZombiePileObjectType.Gem4:
			case ZombiePileObjectType.Gem5:
			case ZombiePileObjectType.Gem6:
				zombiePileObject.mOffsetX += zombiePileObject.gemSpeedX;
				zombiePileObject.mCounter += zombiePileObject.gemRotationSpeed;
				zombiePileObject.mOffsetY += zombiePileObject.gemSpeedY;
				if (zombiePileObject.gemSpeedX > 0f)
				{
					if (zombiePileObject.mOffsetX > (float)(Constants.BOARD_WIDTH + 200))
					{
						zombiePileObject.mOffsetX = 0f - (float)zombiePileObject.gemImage.mWidth - 100f;
						zombiePileObject.mOffsetY = 50f;
						zombiePileObject.mCounter = 0f;
					}
				}
				else if (zombiePileObject.mOffsetX < (float)(-zombiePileObject.gemImage.mWidth))
				{
					zombiePileObject.mOffsetX = Constants.BOARD_WIDTH + 200;
					zombiePileObject.mOffsetY = 50f;
					zombiePileObject.mCounter = 0f;
				}
				break;
			}
			gPileObjects[i] = zombiePileObject;
		}
	}

	public void ButtonPress(int theId)
	{
		mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
	}

	public void ButtonDepress(int theId)
	{
		switch (theId)
		{
		case 0:
			mApp.KillLeaderboardScreen();
			mApp.DoBackToMain(stopMusic: false);
			break;
		case 4:
			ShowLeaderboard(LeaderBoardType.Izombie);
			break;
		case 3:
			ShowLeaderboard(LeaderBoardType.Vasebreaker);
			break;
		case 5:
			ShowLeaderboard(LeaderBoardType.Killed);
			break;
		}
	}

	public void ShowLeaderboard(LeaderBoardType aType)
	{
		mApp.ShowLeaderboardDialog(aType);
	}

	public override bool BackButtonPress()
	{
		mApp.KillLeaderboardScreen();
		mApp.DoBackToMain(stopMusic: false);
		return true;
	}

	public override void MouseUp(int x, int y, int theClickCount)
	{
		ScrollWidget scrollWidget = (ScrollWidget)mParent;
		scrollWidget.ScrollToBottom(animated: true);
	}

	public void UpdateToolTip()
	{
	}

	public void ButtonMouseMove(int id, int x, int y)
	{
	}

	public void ButtonMouseLeave(int id)
	{
	}

	public void ButtonMouseEnter(int id)
	{
		mApp.PlayFoley(FoleyType.Bleep);
	}

	public void ButtonMouseTick(int id)
	{
	}

	public void ButtonPress(int id, int id2)
	{
		if (id != 0)
		{
			mApp.PlaySample(Resources.SOUND_GRAVEBUTTON);
		}
		else
		{
			mApp.PlaySample(Resources.SOUND_TAP);
		}
		mApp.PlayFoley(FoleyType.Bleep);
	}

	public void ButtonDownTick(int id)
	{
	}
}
