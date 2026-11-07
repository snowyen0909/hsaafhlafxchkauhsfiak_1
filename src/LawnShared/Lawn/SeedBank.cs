using System;
using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class SeedBank : GameObject
{
	public int mNumPackets;

	public SeedPacket[] mSeedPackets = new SeedPacket[10];

	public int mCutSceneDarken;

	public int mConveyorBeltCounter;

	public List<CardPair> mConveyorBeltBuffer;

	/// <summary>Largest enlargement applied to the in-game seed packets on phones (500p layout).</summary>
	public const float MaxPhonePacketScale = 1.35f;

	/// <summary>Scale for the in-game seed packet column. iPhone (500p layout) only: the packets are
	/// enlarged as much as the column still fits on screen; everywhere else 1.</summary>
	public float GetPacketDrawScale()
	{
#if __IOS__
		if (!Constants.Is500pMode || mApp.mGameScene != GameScenes.Playing || mNumPackets <= 0 || mBoard == null
			|| mBoard.HasConveyorBeltSeedBank() || mApp.IsSlotMachineLevel() || mSeedPackets[0] == null)
		{
			if (mApp.mAppCounter % 300 == 0)
			{
				LawnPlatform.PvZActivity.Log($"SeedBank no-scale: 500p={Constants.Is500pMode} scene={mApp.mGameScene} packets={mNumPackets}");
			}
			return 1f;
		}
		int last = -1;
		for (int i = 0; i < mNumPackets; i++)
		{
			if (mSeedPackets[i] != null && mSeedPackets[i].mPacketType != SeedType.None)
			{
				last = i;
			}
		}
		if (last < 0)
		{
			return 1f;
		}
		int top = mSeedPackets[0].mY;
		int bottom = mBoard.GetSeedPacketPositionY(last) + Constants.SMALL_SEEDPACKET_HEIGHT;
		int used = bottom - top;
		int available = Constants.BOARD_HEIGHT - 4 - mY - top;
		float k = (used <= 0 || available <= used) ? 1f : Math.Min(MaxPhonePacketScale, (float)available / (float)used);
		if (mApp.mAppCounter % 300 == 0)
		{
			LawnPlatform.PvZActivity.Log($"SeedBank scale={k} packets={mNumPackets} last={last} top={top} bottom={bottom} mY={mY} boardH={Constants.BOARD_HEIGHT}");
		}
		return k;
#else
		return 1f;
#endif
	}

	/// <summary>Draw-space origin of the packet column (pivot of the enlargement).</summary>
	private int PacketPivotX => mSeedPackets[0] != null ? mSeedPackets[0].mX : 0;

	private int PacketPivotY => mSeedPackets[0] != null ? mSeedPackets[0].mY : 0;

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncInt(ref mNumPackets);
		for (int i = 0; i < 10; i++)
		{
			bool theBool = mSeedPackets[i] != null;
			theContext.SyncBool(ref theBool);
			if (theBool)
			{
				SeedPacket[] array = mSeedPackets;
				int num = i;
				if (array[num] == null)
				{
					array[num] = new SeedPacket();
				}
				mSeedPackets[i].Sync(theContext);
			}
		}
		theContext.SyncInt(ref mCutSceneDarken);
		theContext.SyncInt(ref mConveyorBeltCounter);
		mHeight = mApp.mHeight;
		for (int j = 0; j < mNumPackets; j++)
		{
			SeedPacket seedPacket = mSeedPackets[j];
			seedPacket.mY = mBoard.GetSeedPacketPositionY(j);
		}
	}

	public SeedBank()
	{
		mWidth = Constants.New.SeedBank_Width;
		mHeight = mApp.mHeight;
		mNumPackets = 0;
		mCutSceneDarken = 255;
		mConveyorBeltCounter = 0;
		mPosScaled = false;
		for (int i = 0; i < mSeedPackets.Length; i++)
		{
			mSeedPackets[i] = new SeedPacket();
		}
		mConveyorBeltBuffer = new List<CardPair>();
	}

	public void Draw(Graphics g)
	{
		if (mBoard.mCutScene != null && mBoard.mCutScene.IsBeforePreloading())
		{
			return;
		}
		if (mApp.mGameScene != GameScenes.Playing)
		{
			g.mTransX -= mBoard.mX;
			g.mTransY -= mBoard.mY;
		}
		else
		{
			g.mTransX -= Constants.Board_Offset_AspectRatio_Correction;
		}
		float packetScale = GetPacketDrawScale();
		if (packetScale != 1f)
		{
			float px = g.mTransX + PacketPivotX;
			float py = g.mTransY + PacketPivotY;
			SexyTransform2D t = default;
			t.Translate(0f - px, 0f - py);
			t.Scale(packetScale, packetScale);
			t.Translate(px, py);
			g.pushTransform(ref t, concatenate: true);
			SpineWidget.sApplyGraphicsTransform = true;
		}
		if (!mApp.IsSlotMachineLevel() && mBoard.HasConveyorBeltSeedBank())
		{
			TRect clipRect = g.GetClipRect();
			for (int i = -(mConveyorBeltCounter / 4) % Resources.IMAGE_CONVEYORBELT_BELT.mHeight; i < mHeight; i += Resources.IMAGE_CONVEYORBELT_BELT.mHeight)
			{
				g.SetClipRect(ref Constants.ConveyorBeltClipRect);
				g.DrawImage(Resources.IMAGE_CONVEYORBELT_BELT, 0, i);
			}
			g.SetClipRect(clipRect);
			if (mApp.mScreenScales.mVirtualWidth >= Constants.BackBufferSize.Y + 20)
			{
				g.DrawImageMirror(Resources.IMAGE_CONVEYORBELT_BACKDROP, -10, 0);
			}
			g.DrawImage(Resources.IMAGE_CONVEYORBELT_BACKDROP, 0, 0);
			g.SetClipRect(ref Constants.ConveyorBeltClipRect);
		}
		int num = -1;
		if (mBoard.mCursorObject.mCursorType == CursorType.PlantFromBank)
		{
			num = mBoard.mCursorObject.mSeedBankIndex;
		}
		for (int j = 0; j < mNumPackets; j++)
		{
			SeedPacket seedPacket = mSeedPackets[j];
			if (seedPacket.mIndex != num && seedPacket.mPacketType != SeedType.None && seedPacket.BeginDraw(g))
			{
				seedPacket.DrawBackground(g);
				seedPacket.EndDraw(g);
			}
		}
		for (int k = 0; k < mNumPackets; k++)
		{
			SeedPacket seedPacket2 = mSeedPackets[k];
			if (seedPacket2.mIndex != num && seedPacket2.mPacketType != SeedType.None && seedPacket2.BeginDraw(g))
			{
				seedPacket2.Draw(g);
				seedPacket2.EndDraw(g);
			}
		}
		for (int l = 0; l < mNumPackets; l++)
		{
			SeedPacket seedPacket3 = mSeedPackets[l];
			if (seedPacket3.mIndex != num && seedPacket3.mPacketType != SeedType.None && seedPacket3.BeginDraw(g))
			{
				seedPacket3.DrawOverlay(g);
				seedPacket3.EndDraw(g);
			}
		}
		for (int m = 0; m < mNumPackets; m++)
		{
			SeedPacket seedPacket4 = mSeedPackets[m];
			if (seedPacket4.mIndex == num && seedPacket4.mPacketType != SeedType.None)
			{
				if (seedPacket4.BeginDraw(g))
				{
					seedPacket4.DrawBackground(g);
					seedPacket4.EndDraw(g);
				}
				if (seedPacket4.BeginDraw(g))
				{
					seedPacket4.Draw(g);
					seedPacket4.EndDraw(g);
				}
				if (seedPacket4.BeginDraw(g))
				{
					seedPacket4.DrawOverlay(g);
					seedPacket4.EndDraw(g);
				}
			}
		}
		g.ClearClipRect();
		if (packetScale != 1f)
		{
			SpineWidget.sApplyGraphicsTransform = false;
			g.popTransform();
		}
		mApp.IsSlotMachineLevel();
		if (mApp.mGameScene != GameScenes.Playing)
		{
			g.mTransX += mBoard.mX;
			g.mTransY += mBoard.mY;
		}
	}

	public override bool SaveToFile(Sexy.Buffer b)
	{
		try
		{
			base.SaveToFile(b);
			b.WriteLong(mConveyorBeltCounter);
			b.WriteLong(mCutSceneDarken);
			b.WriteLong(mNumPackets);
			for (int i = 0; i < mSeedPackets.Length; i++)
			{
				b.WriteBoolean(mSeedPackets[i] != null);
				if (mSeedPackets[i] != null)
				{
					mSeedPackets[i].SaveToFile(b);
				}
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
		return true;
	}

	public override bool LoadFromFile(Sexy.Buffer b)
	{
		try
		{
			base.LoadFromFile(b);
			mConveyorBeltCounter = b.ReadLong();
			mCutSceneDarken = b.ReadLong();
			mNumPackets = b.ReadLong();
			for (int i = 0; i < 9; i++)
			{
				if (b.ReadBoolean())
				{
					mSeedPackets[i] = new SeedPacket();
					mSeedPackets[i].LoadFromFile(b);
				}
				else
				{
					mSeedPackets[i] = null;
				}
			}
			for (int j = 9; j < mSeedPackets.Length; j++)
			{
				SeedPacket seedPacket = mSeedPackets[j];
				seedPacket.mIndex = j;
				seedPacket.mY = mBoard.GetSeedPacketPositionY(j);
				seedPacket.mX = 0;
				seedPacket.mPacketType = SeedType.None;
			}
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
		return true;
	}

	public void DrawSun(Graphics g)
	{
		if ((mBoard.mCutScene == null || (!mBoard.mCutScene.IsBeforePreloading() && mApp.mGameMode != GameMode.Intro)) && !mBoard.HasConveyorBeltSeedBank())
		{
			int num = Math.Max(mBoard.mSunMoney, 0);
			string theText = LawnApp.ToString(num);
			SexyColor theColor = new SexyColor(0, 0, 0);
			if (mBoard.mOutOfMoneyCounter > 0 && mBoard.mOutOfMoneyCounter % 20 < 10)
			{
				theColor = new SexyColor(255, 0, 0);
			}
			int num2 = Constants.UISunBankPositionX - Constants.Board_Offset_AspectRatio_Correction;
			int num3 = mX;
			g.DrawImage(AtlasResources.IMAGE_DAN_SUNBANK, num2, num3);
			TodCommon.TodDrawString(g, theText, num2 + Constants.UISunBankTextOffset.X, num3 + Constants.UISunBankTextOffset.Y, Resources.FONT_CONTINUUMBOLD14OUTLINE, theColor, DrawStringJustification.Center);
		}
	}

	public bool MouseHitTest(int x, int y, out HitResult theHitResult)
	{
		theHitResult = default;
		float packetScale = GetPacketDrawScale();
		if (packetScale != 1f)
		{
			// Undo the drawing enlargement. Pivot = first packet's top-left, expressed in hit-test space
			// (the hit-test x range starts at the packets' left edge).
			float pivotX = (float)mWidth * Constants.S - 110f - (float)Constants.SMALL_SEEDPACKET_WIDTH;
			float pivotY = PacketPivotY;
			x = (int)Math.Round(pivotX + ((float)x - pivotX) / packetScale);
			y = (int)Math.Round(pivotY + ((float)y - pivotY) / packetScale);
		}
		if ((float)x > (float)mWidth * Constants.S - 110f || (float)x < (float)mWidth * Constants.S - 110f - (float)Constants.SMALL_SEEDPACKET_WIDTH || (mBoard.HasConveyorBeltSeedBank() && y > Constants.ConveyorBeltClipRect.mY + Constants.ConveyorBeltClipRect.mHeight))
		{
			return false;
		}
		SeedPacket seedPacket = null;
		int num = Constants.SMALL_SEEDPACKET_HEIGHT * 2;
		for (int i = 0; i < mNumPackets; i++)
		{
			SeedPacket seedPacket2 = mSeedPackets[i];
			if (seedPacket2.mPacketType == SeedType.None)
			{
				continue;
			}
			int num2 = seedPacket2.CenterY();
			if (num2 <= mHeight)
			{
				int num3 = Math.Abs(num2 - y);
				if (num3 < num)
				{
					num = num3;
					seedPacket = seedPacket2;
				}
			}
		}
		if (seedPacket != null)
		{
			theHitResult.mObject = seedPacket;
			theHitResult.mObjectType = GameObjectType.Seedpacket;
			return true;
		}
		theHitResult.mObject = null;
		theHitResult.mObjectType = GameObjectType.None;
		return false;
	}

	public void Move(int x, int y)
	{
		mX = x - ((mApp.mGameScene == GameScenes.Playing) ? Constants.Board_Offset_AspectRatio_Correction : 0);
		mY = y;
	}

	public bool ContainsPoint(int theX, int theY)
	{
		if (theX >= mX && theX < mX + mWidth && theY >= mY)
		{
			return theY < mY + mHeight;
		}
		return false;
	}

	public int GetNumSeedsOnConveyorBelt()
	{
		for (int i = 0; i < mNumPackets; i++)
		{
			SeedPacket seedPacket = mSeedPackets[i];
			if (seedPacket.mPacketType == SeedType.None)
			{
				return i;
			}
		}
		return mNumPackets;
	}

	public void AddSeed(SeedType theSeedType, bool isImitator = false)
	{
		Debug.ASSERT(mBoard.HasConveyorBeltSeedBank());
		Debug.ASSERT(theSeedType != SeedType.None);
		int numSeedsOnConveyorBelt = GetNumSeedsOnConveyorBelt();
		if (numSeedsOnConveyorBelt == mNumPackets)
		{
			mConveyorBeltBuffer.Add(new CardPair
			{
				mSeedType = theSeedType,
				mImitator = isImitator
			});
			return;
		}
		SeedPacket seedPacket = mSeedPackets[numSeedsOnConveyorBelt];
		if (isImitator)
		{
			seedPacket.mPacketType = SeedType.Imitater;
			seedPacket.mImitaterType = theSeedType;
		}
		else
		{
			seedPacket.mPacketType = theSeedType;
		}
		seedPacket.mRefreshCounter = 0;
		seedPacket.mRefreshTime = 0;
		seedPacket.mRefreshing = false;
		seedPacket.mActive = true;
		int num = Constants.ConveyorBeltClipRect.mY + Constants.ConveyorBeltClipRect.mHeight + seedPacket.mHeight / 2 + 15 - numSeedsOnConveyorBelt;
		if (numSeedsOnConveyorBelt > 0)
		{
			SeedPacket seedPacket2 = mSeedPackets[numSeedsOnConveyorBelt - 1];
			int num2 = seedPacket2.mY + seedPacket2.mOffsetY;
			if (num < num2 + 50)
			{
				num = num2 + 90;
			}
		}
		seedPacket.mOffsetY = num - seedPacket.mY;
	}

	public void RemoveSeed(int theIndex)
	{
		Debug.ASSERT(mBoard.HasConveyorBeltSeedBank());
		Debug.ASSERT(theIndex >= 0 && theIndex < GetNumSeedsOnConveyorBelt());
		for (int i = theIndex; i < mNumPackets; i++)
		{
			SeedPacket seedPacket = mSeedPackets[i];
			if (seedPacket.mPacketType == SeedType.None)
			{
				break;
			}
			if (i == mNumPackets - 1)
			{
				seedPacket.mPacketType = SeedType.None;
				seedPacket.mImitaterType = SeedType.None;
				seedPacket.mOffsetY = 0;
			}
			else
			{
				SeedPacket seedPacket2 = mSeedPackets[i + 1];
				seedPacket.mPacketType = seedPacket2.mPacketType;
				seedPacket.mImitaterType = seedPacket2.mImitaterType;
				seedPacket.mOffsetY = seedPacket2.mOffsetY + seedPacket2.mY - seedPacket.mY + 1;
			}
			seedPacket.mRefreshCounter = 0;
			seedPacket.mRefreshTime = 0;
			seedPacket.mRefreshing = false;
			seedPacket.mActive = true;
		}
	}

	public int CountOfTypeOnConveyorBelt(SeedType aSeedType)
	{
		int num = 0;
		for (int i = 0; i < mNumPackets; i++)
		{
			SeedPacket seedPacket = mSeedPackets[i];
			if (seedPacket.mPacketType == aSeedType)
			{
				num++;
			}
		}
		return num;
	}

	public void UpdateConveyorBelt()
	{
		mConveyorBeltCounter++;
		if (mConveyorBeltCounter % 4 != 0)
		{
			return;
		}
		for (int i = 0; i < mNumPackets; i++)
		{
			SeedPacket seedPacket = mSeedPackets[i];
			if (seedPacket.mOffsetY > 0)
			{
				seedPacket.mOffsetY = Math.Max(seedPacket.mOffsetY - 1, 0);
			}
		}
	}

	public void UpdateHeight()
	{
		mNumPackets = mBoard.GetNumSeedsInBank();
		mHeight = mApp.mHeight;
		for (int i = 0; i < mNumPackets; i++)
		{
			SeedPacket seedPacket = mSeedPackets[i];
			seedPacket.mY = mBoard.GetSeedPacketPositionY(i);
		}
	}

	public bool IsSeedPacketAccessible(SeedPacket theSeedPacket)
	{
		if (mBoard.HasConveyorBeltSeedBank())
		{
			return Constants.ConveyorBeltClipRect.Intersects(new TRect(theSeedPacket.mX, theSeedPacket.mY + theSeedPacket.mOffsetY, theSeedPacket.mWidth, theSeedPacket.mHeight));
		}
		return true;
	}

	public void RefreshAllPackets()
	{
		for (int i = 0; i < mNumPackets; i++)
		{
			SeedPacket seedPacket = mSeedPackets[i];
			if (seedPacket.mPacketType == SeedType.None)
			{
				break;
			}
			if (seedPacket.mRefreshing)
			{
				seedPacket.mRefreshCounter = 0;
				seedPacket.mRefreshing = false;
				seedPacket.Activate();
				seedPacket.FlashIfReady();
			}
		}
	}
}
