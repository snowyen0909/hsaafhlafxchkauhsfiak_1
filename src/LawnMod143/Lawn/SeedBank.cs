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
		if (theContext.mReading && theContext.mVersion < SaveFileVersion.ClassicConveyorOffset)
		{
			for (int j = 0; j < mNumPackets; j++)
			{
				int num2 = (int)Constants.InvertAndScale(400f) + 40 * j;
				int num3 = 515 + 40 * j;
				mSeedPackets[j].mClassicOffsetX = ((num2 > 0) ? Math.Max(TodCommon.FloatRoundToInt((float)(num3 * mSeedPackets[j].mOffsetY) / (float)num2), 0) : 0);
			}
		}
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			mHeight = AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight;
			mWidth = (mBoard.HasConveyorBeltSeedBank() ? (83 + AtlasResources.IMAGE_CLASSIC_CONVEYORBELT_BACKDROP.mWidth) : (AtlasResources.IMAGE_CLASSIC_SEEDBANK.mWidth + mBoard.GetSeedBankExtraWidth()));
			for (int k = 0; k < mNumPackets; k++)
			{
				mSeedPackets[k].mX = mBoard.GetSeedPacketPositionX(k);
				mSeedPackets[k].mY = 8;
				mSeedPackets[k].mWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
				mSeedPackets[k].mHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			}
		}
		else
		{
			mWidth = Constants.New.SeedBank_Width;
			mHeight = mApp.mHeight;
			for (int l = 0; l < mNumPackets; l++)
			{
				mSeedPackets[l].mX = 0;
				mSeedPackets[l].mWidth = Constants.SMALL_SEEDPACKET_WIDTH;
				mSeedPackets[l].mHeight = Constants.SMALL_SEEDPACKET_HEIGHT;
				mSeedPackets[l].mY = mBoard.GetSeedPacketPositionY(l);
			}
		}
		if (theContext.mReading)
		{
			RestorePositionAfterLoad();
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

	private void RestorePositionAfterLoad()
	{
		bool flag = mApp.IsChallengeWithoutSeedBank();
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			int num = AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight;
			if (!flag)
			{
				mY = 0;
			}
			else if (mX < 0)
			{
				mY = Math.Clamp(TodCommon.FloatRoundToInt((float)((mX + Constants.Board_Offset_AspectRatio_Correction) * num) / (float)mWidth), -num, 0);
			}
			mX = GameConstants.SEED_BANK_OFFSET_X_END;
			if (flag && mNumPackets == 0)
			{
				mY = -num;
			}
		}
		else if (!flag)
		{
			mX = 0;
			mY = 0;
		}
		else if (mNumPackets == 0)
		{
			mX = -mWidth - (mApp.mScreenScales.mVirtualWidth - Constants.BOARD_WIDTH) / 2;
			mY = 0;
		}
	}

	public void MoveToHidden()
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			Move(GameConstants.SEED_BANK_OFFSET_X, -AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight);
		}
		else
		{
			Move(-mWidth - (mApp.mScreenScales.mVirtualWidth - Constants.BOARD_WIDTH) / 2, 0);
		}
	}

	public void MoveToShown()
	{
		Move(mApp.mPlayerInfo.mClassicSeedChooser ? GameConstants.SEED_BANK_OFFSET_X_END : 0, 0);
	}

	public void UpdateSlideIn(bool theShowSeedBank)
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			if (mY < 0 && (theShowSeedBank || mY > -mHeight))
			{
				mY = Math.Min(mY + 2, 0);
			}
		}
		else if (mX < 0 && (theShowSeedBank || mX > -mWidth))
		{
			mX = Math.Min(mX + 2, 0);
		}
	}

	public void Draw(Graphics g)
	{
		if (mBoard.mCutScene != null && mBoard.mCutScene.IsBeforePreloading())
		{
			return;
		}
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			bool flag = mApp.mGameScene != GameScenes.Playing;
			bool flag2 = false;
			if (flag)
			{
				g.mTransX -= mBoard.mX - Constants.Board_Offset_AspectRatio_Correction;
				g.mTransY -= mBoard.mY;
			}
			if (mApp.IsSlotMachineLevel())
			{
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SUNBANK, 0, 0);
			}
			else if (mBoard.HasConveyorBeltSeedBank())
			{
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_CONVEYORBELT_BACKDROP, 83, 0);
				g.DrawImageCel(AtlasResources.IMAGE_CLASSIC_CONVEYORBELT, 90, 63, mConveyorBeltCounter / 4 % 6);
				g.SetClipRect(90, 0, AtlasResources.IMAGE_CLASSIC_CONVEYORBELT.GetCelWidth() - 1, Constants.BOARD_HEIGHT);
				g.HardwareClip();
				flag2 = true;
			}
			else
			{
				int seedBankExtraWidth = mBoard.GetSeedBankExtraWidth();
				TRect theSrcRect = new TRect(AtlasResources.IMAGE_CLASSIC_SEEDBANK.mWidth - seedBankExtraWidth - 12, 0, seedBankExtraWidth + 12, AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight);
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDBANK, 0, 0);
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SEEDBANK, AtlasResources.IMAGE_CLASSIC_SEEDBANK.mWidth - 12, 0, theSrcRect);
			}
			for (int i = 0; i < mNumPackets; i++)
			{
				SeedPacket seedPacket = mSeedPackets[i];
				if (seedPacket.mPacketType != SeedType.None && seedPacket.BeginDraw(g))
				{
					seedPacket.Draw(g);
					seedPacket.EndDraw(g);
				}
			}
			if (flag2)
			{
				g.EndHardwareClip();
			}
			g.ClearClipRect();
			if (mApp.IsSlotMachineLevel() && mY > -AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight)
			{
				g.DrawImage(AtlasResources.IMAGE_CLASSIC_SLOTMACHINE_OVERLAY, 189, -2);
			}
			if (!mBoard.HasConveyorBeltSeedBank())
			{
				string theText = LawnApp.ToString(Math.Max(mBoard.mSunMoney, 0));
				SexyColor theColor = ((mBoard.mOutOfMoneyCounter > 0 && mBoard.mOutOfMoneyCounter % 20 < 10) ? new SexyColor(255, 0, 0) : SexyColor.Black);
				TodCommon.TodDrawString(g, theText, 37, 58, Resources.FONT_CONTINUUMBOLD14OUTLINE, theColor, DrawStringJustification.Center, 2f / 3f);
			}
			if (flag)
			{
				g.mTransX += mBoard.mX - Constants.Board_Offset_AspectRatio_Correction;
				g.mTransY += mBoard.mY;
			}
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
		if (!mApp.IsSlotMachineLevel() && mBoard.HasConveyorBeltSeedBank())
		{
			TRect clipRect = g.GetClipRect();
			for (int j = -(mConveyorBeltCounter / 4) % Resources.IMAGE_CONVEYORBELT_BELT.mHeight; j < mHeight; j += Resources.IMAGE_CONVEYORBELT_BELT.mHeight)
			{
				g.SetClipRect(ref Constants.ConveyorBeltClipRect);
				g.DrawImage(Resources.IMAGE_CONVEYORBELT_BELT, 0, j);
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
		int num2 = (mApp.IsCornRouletteLevel() ? Math.Min(mNumPackets, 4) : mNumPackets);
		for (int k = 0; k < num2; k++)
		{
			SeedPacket seedPacket2 = mSeedPackets[k];
			if (seedPacket2.mIndex != num && seedPacket2.mPacketType != SeedType.None && seedPacket2.BeginDraw(g))
			{
				seedPacket2.DrawBackground(g);
				seedPacket2.EndDraw(g);
			}
		}
		for (int l = 0; l < num2; l++)
		{
			SeedPacket seedPacket3 = mSeedPackets[l];
			if (seedPacket3.mIndex != num && seedPacket3.mPacketType != SeedType.None && seedPacket3.BeginDraw(g))
			{
				seedPacket3.Draw(g);
				seedPacket3.EndDraw(g);
			}
		}
		for (int m = 0; m < num2; m++)
		{
			SeedPacket seedPacket4 = mSeedPackets[m];
			if (seedPacket4.mIndex != num && seedPacket4.mPacketType != SeedType.None && seedPacket4.BeginDraw(g))
			{
				seedPacket4.DrawOverlay(g);
				seedPacket4.EndDraw(g);
			}
		}
		for (int n = 0; n < num2; n++)
		{
			SeedPacket seedPacket5 = mSeedPackets[n];
			if (seedPacket5.mIndex == num && seedPacket5.mPacketType != SeedType.None)
			{
				if (seedPacket5.BeginDraw(g))
				{
					seedPacket5.DrawBackground(g);
					seedPacket5.EndDraw(g);
				}
				if (seedPacket5.BeginDraw(g))
				{
					seedPacket5.Draw(g);
					seedPacket5.EndDraw(g);
				}
				if (seedPacket5.BeginDraw(g))
				{
					seedPacket5.DrawOverlay(g);
					seedPacket5.EndDraw(g);
				}
			}
		}
		if (mApp.IsCornRouletteLevel())
		{
			for (int num3 = 4; num3 < Math.Min(mNumPackets, 8); num3++)
			{
				SeedPacket seedPacket6 = mSeedPackets[num3];
				if (seedPacket6.mPacketType != SeedType.None && seedPacket6.BeginDraw(g))
				{
					g.mTransX += 920;
					g.mTransY += mSeedPackets[num3 - 4].mY - seedPacket6.mY;
					seedPacket6.DrawBackground(g);
					seedPacket6.Draw(g);
					seedPacket6.DrawOverlay(g);
					seedPacket6.EndDraw(g);
				}
			}
		}
		g.ClearClipRect();
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
				if (mApp.mPlayerInfo.mClassicSeedChooser)
				{
					seedPacket.mX = mBoard.GetSeedPacketPositionX(j);
					seedPacket.mY = 8;
				}
				else
				{
					seedPacket.mX = 0;
					seedPacket.mY = mBoard.GetSeedPacketPositionY(j);
				}
				seedPacket.mPacketType = SeedType.None;
			}
			if (mNumPackets == 0)
			{
				mWidth = (mApp.mPlayerInfo.mClassicSeedChooser ? AtlasResources.IMAGE_CLASSIC_SEEDBANK.mWidth : Constants.New.SeedBank_Width);
			}
			RestorePositionAfterLoad();
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
		if ((mBoard.mCutScene == null || (!mBoard.mCutScene.IsBeforePreloading() && mApp.mGameMode != GameMode.Intro)) && !mApp.mPlayerInfo.mClassicSeedChooser && !mBoard.HasConveyorBeltSeedBank())
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
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			int num = x - mX;
			int num2 = y - mY;
			if (num > mWidth - 5 || mNumPackets <= 0)
			{
				return false;
			}
			for (int i = 0; i < mNumPackets; i++)
			{
				SeedPacket seedPacket = mSeedPackets[i];
				int num3 = mBoard.GetSeedPacketPositionX(i) + seedPacket.mClassicOffsetX;
				int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
				int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
				int num4 = 90 + AtlasResources.IMAGE_CLASSIC_CONVEYORBELT.GetCelWidth() - 1;
				if (seedPacket.mPacketType != SeedType.None && seedPacket.mSlotMachineCountDown <= 0 && num >= num3 && num < num3 + celWidth && num2 >= 8 && num2 < 8 + celHeight && (!mBoard.HasConveyorBeltSeedBank() || num3 < num4))
				{
					theHitResult.mObject = seedPacket;
					theHitResult.mObjectType = GameObjectType.Seedpacket;
					return true;
				}
			}
			return false;
		}
		if ((float)x > (float)mWidth * Constants.S - 110f || (float)x < (float)mWidth * Constants.S - 110f - (float)Constants.SMALL_SEEDPACKET_WIDTH || (mBoard.HasConveyorBeltSeedBank() && y > Constants.ConveyorBeltClipRect.mY + Constants.ConveyorBeltClipRect.mHeight))
		{
			return false;
		}
		SeedPacket seedPacket2 = null;
		int num5 = Constants.SMALL_SEEDPACKET_HEIGHT * 2;
		int num6 = (mApp.IsCornRouletteLevel() ? Math.Min(mNumPackets, 4) : mNumPackets);
		for (int j = 0; j < num6; j++)
		{
			SeedPacket seedPacket3 = mSeedPackets[j];
			if (seedPacket3.mPacketType == SeedType.None)
			{
				continue;
			}
			int num7 = seedPacket3.CenterY();
			if (num7 <= mHeight)
			{
				int num8 = Math.Abs(num7 - y);
				if (num8 < num5)
				{
					num5 = num8;
					seedPacket2 = seedPacket3;
				}
			}
		}
		if (seedPacket2 != null)
		{
			theHitResult.mObject = seedPacket2;
			theHitResult.mObjectType = GameObjectType.Seedpacket;
			return true;
		}
		theHitResult.mObject = null;
		theHitResult.mObjectType = GameObjectType.None;
		return false;
	}

	public void Move(int x, int y)
	{
		mX = (mApp.mPlayerInfo.mClassicSeedChooser ? x : (x - ((mApp.mGameScene == GameScenes.Playing) ? Constants.Board_Offset_AspectRatio_Correction : 0)));
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
		seedPacket.mClassicOffsetX = 515 - (AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() + 1) * numSeedsOnConveyorBelt;
		if (numSeedsOnConveyorBelt > 0)
		{
			SeedPacket seedPacket2 = mSeedPackets[numSeedsOnConveyorBelt - 1];
			int num2 = seedPacket2.mY + seedPacket2.mOffsetY;
			if (num < num2 + 50)
			{
				num = num2 + 90;
			}
			if (seedPacket.mClassicOffsetX < seedPacket2.mClassicOffsetX)
			{
				seedPacket.mClassicOffsetX = seedPacket2.mClassicOffsetX + 40;
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
				seedPacket.mClassicOffsetX = 0;
			}
			else
			{
				SeedPacket seedPacket2 = mSeedPackets[i + 1];
				seedPacket.mPacketType = seedPacket2.mPacketType;
				seedPacket.mImitaterType = seedPacket2.mImitaterType;
				seedPacket.mOffsetY = seedPacket2.mOffsetY + seedPacket2.mY - seedPacket.mY + 1;
				seedPacket.mClassicOffsetX = seedPacket2.mClassicOffsetX + AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth() + 1;
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
			if (seedPacket.mClassicOffsetX > 0)
			{
				seedPacket.mClassicOffsetX = Math.Max(seedPacket.mClassicOffsetX - 1, 0);
			}
		}
	}

	public void UpdateHeight()
	{
		mNumPackets = mBoard.GetNumSeedsInBank();
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			mHeight = AtlasResources.IMAGE_CLASSIC_SEEDBANK.mHeight;
			mWidth = (mBoard.HasConveyorBeltSeedBank() ? (83 + AtlasResources.IMAGE_CLASSIC_CONVEYORBELT_BACKDROP.mWidth) : (AtlasResources.IMAGE_CLASSIC_SEEDBANK.mWidth + mBoard.GetSeedBankExtraWidth()));
			for (int i = 0; i < mNumPackets; i++)
			{
				mSeedPackets[i].mX = mBoard.GetSeedPacketPositionX(i);
				mSeedPackets[i].mY = 8;
				mSeedPackets[i].mWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
				mSeedPackets[i].mHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			}
		}
		else
		{
			mWidth = Constants.New.SeedBank_Width;
			mHeight = mApp.mHeight;
			for (int j = 0; j < mNumPackets; j++)
			{
				mSeedPackets[j].mWidth = Constants.SMALL_SEEDPACKET_WIDTH;
				mSeedPackets[j].mHeight = Constants.SMALL_SEEDPACKET_HEIGHT;
				mSeedPackets[j].mY = mBoard.GetSeedPacketPositionY(j);
			}
		}
	}

	public bool IsSeedPacketAccessible(SeedPacket theSeedPacket)
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			if (!mBoard.HasConveyorBeltSeedBank())
			{
				return true;
			}
			int theX = mBoard.GetSeedPacketPositionX(theSeedPacket.mIndex) + theSeedPacket.mClassicOffsetX;
			return new TRect(90, 0, AtlasResources.IMAGE_CLASSIC_CONVEYORBELT.GetCelWidth() - 1, Constants.BOARD_HEIGHT).Intersects(new TRect(theX, 8, AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth(), AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight()));
		}
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
