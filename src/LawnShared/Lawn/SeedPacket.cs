using System;
using Lawn.Creative;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class SeedPacket : GameObject
{
	public int mRefreshCounter;

	public int mRefreshTime;

	public int mIndex;

	public int mOffsetY;

	public int mClassicOffsetX;

	public SeedType mPacketType;

	public SeedType mImitaterType;

	public int mSlotMachineCountDown;

	public SeedType mSlotMachiningNextSeed;

	public float mSlotMachiningPosition;

	public bool mActive;

	public bool mRefreshing;

	public int mTimesUsed;

	public int mBuzzedCounter;

	public static SpineWidget[] gDynamicTachieSeedPacketSpineWidgets = new SpineWidget[(int)SeedType.SeedTypeCount];

	private TodWeightedArray[] aSeedWeightArray = new TodWeightedArray[53];

	private SeedType[] SLOT_SEED_TYPES = new SeedType[6]
	{
		SeedType.Sunflower,
		SeedType.Peashooter,
		SeedType.Snowpea,
		SeedType.Wallnut,
		SeedType.SlotMachineSun,
		SeedType.SlotMachineDiamond
	};

	internal override void Sync(GlobalMembersSaveGame.SaveGameContext theContext)
	{
		base.Sync(theContext);
		theContext.SyncInt(ref mRefreshCounter);
		theContext.SyncInt(ref mRefreshTime);
		theContext.SyncInt(ref mIndex);
		theContext.SyncInt(ref mOffsetY);
		theContext.SyncInt(ref mClassicOffsetX, SaveFileVersion.ClassicConveyorOffset);
		theContext.SyncEnum(ref mPacketType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncEnum(ref mImitaterType, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncInt(ref mSlotMachineCountDown);
		theContext.SyncEnum(ref mSlotMachiningNextSeed, theContext.aSeedTypeSaver, SaveFileVersion.Initial, SeedType.Peashooter);
		theContext.SyncFloat(ref mSlotMachiningPosition);
		theContext.SyncBool(ref mActive);
		theContext.SyncBool(ref mRefreshing);
		theContext.SyncInt(ref mTimesUsed);
	}

	public SeedPacket()
	{
		mPacketType = SeedType.None;
		mImitaterType = SeedType.None;
		mIndex = -1;
		mWidth = Constants.SMALL_SEEDPACKET_WIDTH;
		mHeight = Constants.SMALL_SEEDPACKET_HEIGHT;
		mRefreshCounter = 0;
		mRefreshTime = 0;
		mRefreshing = false;
		mActive = true;
		mOffsetY = 0;
		mClassicOffsetX = 0;
		mSlotMachineCountDown = 0;
		mSlotMachiningNextSeed = SeedType.None;
		mSlotMachiningPosition = 0f;
		mTimesUsed = 0;
		mPosScaled = false;
	}

	public override bool SaveToFile(Sexy.Buffer b)
	{
		try
		{
			base.SaveToFile(b);
			b.WriteBoolean(mActive);
			b.WriteLong((int)mImitaterType);
			b.WriteLong(mIndex);
			b.WriteLong(mOffsetY);
			b.WriteLong((int)mPacketType);
			b.WriteLong(mRefreshCounter);
			b.WriteBoolean(mRefreshing);
			b.WriteLong(mRefreshTime);
			b.WriteLong(mSlotMachineCountDown);
			b.WriteLong((int)mSlotMachiningNextSeed);
			b.WriteFloat(mSlotMachiningPosition);
			b.WriteLong(mTimesUsed);
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
			mActive = b.ReadBoolean();
			mImitaterType = SeedTypeLegacy.FromInt(b.ReadLong());
			mIndex = b.ReadLong();
			mOffsetY = b.ReadLong();
			mPacketType = SeedTypeLegacy.FromInt(b.ReadLong());
			mRefreshCounter = b.ReadLong();
			mRefreshing = b.ReadBoolean();
			mRefreshTime = b.ReadLong();
			mSlotMachineCountDown = b.ReadLong();
			mSlotMachiningNextSeed = SeedTypeLegacy.FromInt(b.ReadLong());
			mSlotMachiningPosition = b.ReadFloat();
			mTimesUsed = b.ReadLong();
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			return false;
		}
		return true;
	}

	public void Update()
	{
		if (mApp.mGameScene != GameScenes.Playing || mPacketType == SeedType.None)
		{
			return;
		}
		if (mBoard.mMainCounter == 0)
		{
			FlashIfReady();
		}
		if (!mActive && mRefreshing)
		{
			mRefreshCounter++;
			if (mRefreshCounter > mRefreshTime)
			{
				mRefreshCounter = 0;
				mRefreshing = false;
				Activate();
				FlashIfReady();
			}
		}
		if (mSlotMachineCountDown <= 0)
		{
			return;
		}
		mSlotMachineCountDown--;
		float num = TodCommon.TodAnimateCurveFloat(400, 0, mSlotMachineCountDown, 6f, 2f, TodCurves.Linear);
		mSlotMachiningPosition += num * 0.01f;
		if (mSlotMachiningPosition >= 1f)
		{
			mPacketType = mSlotMachiningNextSeed;
			if (mSlotMachineCountDown >= 0 && mSlotMachineCountDown < 3)
			{
				mSlotMachiningPosition = 0f;
				Activate();
			}
			else
			{
				mSlotMachiningPosition--;
				PickNextSlotMachineSeed();
			}
		}
		else if (mSlotMachineCountDown == 0)
		{
			mSlotMachineCountDown = 3;
		}
	}

	public void DrawBackground(Graphics g)
	{
		float thePercentDark = 0f;
		int theGrayness = 255;
		if (mBoard.mCursorObject.mCursorType != CursorType.PlantFromBank || mBoard.mCursorObject.mSeedBankIndex != mIndex)
		{
			GetGraynessAndDarkness(ref theGrayness, ref thePercentDark);
		}
		if (mApp.IsSlotMachineLevel())
		{
			TRect tRect = mApp.mBoard.mChallenge.SlotMachineRect();
			int num = tRect.mY + Constants.Challenge_SlotMachine_Y_Offset;
			float num2 = 0.9f;
			int challenge_SlotMachine_ClipHeight = Constants.Challenge_SlotMachine_ClipHeight;
			float num3 = (float)Resources.IMAGE_SEEDPACKETS.GetCelWidth() * num2;
			int num4 = num - Constants.Challenge_SlotMachine_Y_Pos + TodCommon.FloatRoundToInt(mSlotMachiningPosition * (float)(-challenge_SlotMachine_ClipHeight));
			Graphics graphics = Graphics.GetNew(g);
			graphics.mTransY = 0;
			graphics.mTransX = mBoard.mX;
			int challenge_SlotMachine_Gap = Constants.Challenge_SlotMachine_Gap;
			int num5 = tRect.mX + Constants.Challenge_SlotMachine_Offset + mIndex * TodCommon.FloatRoundToInt((float)challenge_SlotMachine_Gap + num3);
			int challenge_SlotMachine_Shadow_Offset = Constants.Challenge_SlotMachine_Shadow_Offset;
			graphics.ClipRect(num5, num, (int)num3, challenge_SlotMachine_ClipHeight);
			graphics.HardwareClip();
			if (mSlotMachineCountDown > 0)
			{
				DrawSeedPacketSlotMachine(graphics, num5, num4, mPacketType, SeedType.None, theGrayness, num2);
				DrawSeedPacketSlotMachine(graphics, num5, (float)num4 + (float)(Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2) * num2 - (float)challenge_SlotMachine_Shadow_Offset, mSlotMachiningNextSeed, SeedType.None, theGrayness, num2);
			}
			else
			{
				DrawSeedPacketSlotMachine(graphics, num5, num4, mPacketType, SeedType.None, theGrayness, num2);
			}
			graphics.EndHardwareClip();
			graphics.PrepareForReuse();
		}
		else
		{
			bool theDrawCostBackground = !mBoard.HasConveyorBeltSeedBank() && !mApp.IsSlotMachineLevel();
			DrawSmallSeedPacket(g, 0f, mOffsetY, mPacketType, mImitaterType, 0f, theGrayness, theDrawCost: false, theUseCurrentCost: true, theDrawBackground: true, theDrawCostBackground);
		}
	}

	public void Draw(Graphics g)
	{
		if (mApp.mPlayerInfo.mClassicSeedChooser)
		{
			float thePercentDark = 0f;
			int theGrayness = 255;
			GetGraynessAndDarkness(ref theGrayness, ref thePercentDark);
			int celWidth = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
			int celHeight = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			if (mSlotMachineCountDown > 0)
			{
				int num = TodCommon.FloatRoundToInt((float)(-celHeight) * mSlotMachiningPosition);
				Graphics graphics = Graphics.GetNew(g);
				graphics.ClipRect(mClassicOffsetX, 0, celWidth, celHeight);
				DrawClassicSeedPacket(graphics, mClassicOffsetX, num, mPacketType, SeedType.None, 0f, 128, theDrawCost: false, theUseCurrentCost: false);
				DrawClassicSeedPacket(graphics, mClassicOffsetX, celHeight + num, mSlotMachiningNextSeed, SeedType.None, 0f, 128, theDrawCost: false, theUseCurrentCost: false);
				graphics.PrepareForReuse();
				return;
			}
			bool theDrawCost = !mBoard.HasConveyorBeltSeedBank() && !mApp.IsSlotMachineLevel() && !mApp.IsCornRouletteLevel();
			DrawClassicSeedPacket(g, mClassicOffsetX, 0f, mPacketType, mImitaterType, thePercentDark, theGrayness, theDrawCost, theUseCurrentCost: true);
			SeedType seedType = ((mPacketType == SeedType.Imitater && mImitaterType != SeedType.None) ? mImitaterType : mPacketType);
			bool flag = ((seedType == SeedType.Jalapeno || seedType == SeedType.PickledPepper) ? true : false);
			if (flag && mBoard.mSealedCountdown > 0)
			{
				Image iMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW = AtlasResources.IMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW;
				float num2 = Math.Min((float)celWidth / (float)iMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW.mWidth, (float)celHeight / (float)iMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW.mHeight);
				float thePosX = (float)mClassicOffsetX + ((float)celWidth - (float)iMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW.mWidth * num2) * 0.5f;
				float thePosY = ((float)celHeight - (float)iMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW.mHeight * num2) * 0.5f;
				TodCommon.TodDrawImageScaledF(g, iMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW, thePosX, thePosY, num2, num2);
			}
		}
		else if (!mBoard.HasConveyorBeltSeedBank() && !mApp.IsSlotMachineLevel())
		{
			DrawSmallSeedPacket(g, 0f, mOffsetY, mPacketType, mImitaterType, 0f, 255, theDrawCost: true, theUseCurrentCost: true, theDrawBackground: false, theDrawCostBackground: false);
			SeedType seedType2 = mPacketType;
			if (mPacketType == SeedType.Imitater && mImitaterType != SeedType.None)
			{
				seedType2 = mImitaterType;
			}
			bool flag = ((seedType2 == SeedType.Jalapeno || seedType2 == SeedType.PickledPepper) ? true : false);
			if (flag && mBoard.mSealedCountdown > 0)
			{
				g.DrawImage(AtlasResources.IMAGE_REANIM_ZOMBIE_TALISMAN_RAINWATER_CROSSFLOW, 0, (int)((float)mOffsetY * Constants.S), Constants.SMALL_SEEDPACKET_WIDTH, Constants.SMALL_SEEDPACKET_HEIGHT);
			}
		}
	}

	public void DrawOverlay(Graphics g)
	{
		float thePercentDark = 0f;
		int theGrayness = 255;
		bool flag = mBoard.mCursorObject.mCursorType == CursorType.PlantFromBank && mBoard.mCursorObject.mSeedBankIndex == mIndex;
		if (!flag)
		{
			GetGraynessAndDarkness(ref theGrayness, ref thePercentDark);
		}
		if (mSlotMachineCountDown > 0)
		{
			int num = TodCommon.FloatRoundToInt(mSlotMachiningPosition * (float)(-mHeight));
			Graphics graphics = Graphics.GetNew(g);
			graphics.ClipRect(0, 0, mWidth, mHeight);
			DrawSmallSeedPacket(graphics, 0f, num, mPacketType, SeedType.None, 0f, 128, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: false, theDrawCostBackground: false);
			DrawSmallSeedPacket(graphics, 0f, num + mHeight, mSlotMachiningNextSeed, SeedType.None, 0f, 128, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: false, theDrawCostBackground: false);
			graphics.PrepareForReuse();
			return;
		}
		DrawSmallSeedPacket(g, 0f, mOffsetY, mPacketType, mImitaterType, thePercentDark, theGrayness, theDrawCost: false, theUseCurrentCost: true, theDrawBackground: false, theDrawCostBackground: false);
		if (flag)
		{
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, 180, premultiply: true));
			g.DrawImage(AtlasResources.IMAGE_SELECTED_PACKET, Constants.SeedPacket_Selector_Pos.X, mOffsetY - Constants.SeedPacket_Selector_Pos.Y);
			g.SetColorizeImages(colorizeImages: false);
		}
		if (mBuzzedCounter > 0)
		{
			mBuzzedCounter--;
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 100, 100, 180 * mBuzzedCounter / 25, premultiply: true));
			g.DrawImage(AtlasResources.IMAGE_SELECTED_PACKET, Constants.SeedPacket_Selector_Pos.X, mOffsetY - Constants.SeedPacket_Selector_Pos.Y);
			g.SetColorizeImages(colorizeImages: false);
		}
	}

	internal static void DrawSeedType(Graphics g, float x, float y, SeedType theSeedType, SeedType theImitaterType)
	{
		Plant.DrawSeedType(g, theSeedType, theImitaterType, DrawVariation.Normal, x, y);
	}

	public static void DrawClassicSeedPacket(Graphics g, float x, float y, SeedType theSeedType, SeedType theImitaterType, float thePercentDark, int theGrayness, bool theDrawCost, bool theUseCurrentCost)
	{
		SeedType seedType = theSeedType;
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			seedType = theImitaterType;
		}
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		if (theGrayness != 255)
		{
			g.SetColor(new SexyColor(theGrayness, theGrayness, theGrayness));
			g.SetColorizeImages(colorizeImages: true);
		}
		else if (thePercentDark > 0f)
		{
			g.SetColor(new SexyColor(128, 128, 128, 255));
			g.SetColorizeImages(colorizeImages: true);
		}
		int theCelCol = ((theSeedType != SeedType.Imitater) ? (Plant.IsUpgrade(seedType) ? 1 : (theSeedType switch
		{
			SeedType.ZombiquariumTrophy => 8, 
			SeedType.ZombiquariumSnorkel => 7, 
			SeedType.SlotMachineDiamond => 6, 
			SeedType.SlotMachineSun => 5, 
			SeedType.BeghouledButtonShuffle => 4, 
			SeedType.BeghouledButtonCrater => 3, 
			_ => 2, 
		})) : 0);
		bool flag = (uint)(seedType - 72) <= 3u;
		bool flag2 = flag;
		Image theImageStrip = (flag2 ? AtlasResources.IMAGE_CLASSIC_SEEDPACKET_BROWN : AtlasResources.IMAGE_CLASSIC_SEEDS);
		if (flag2)
		{
			theCelCol = 0;
		}
		if (!flag2 && g.mScaleX > 1f)
		{
			TodCommon.TodDrawImageCelScaledF(g, AtlasResources.IMAGE_CLASSIC_SEEDPACKET_LARGER, x, y, 0, 0, g.mScaleX * 0.5f, g.mScaleY * 0.5f);
		}
		else
		{
			TodCommon.TodDrawImageCelScaledF(g, theImageStrip, x, y, theCelCol, 0, g.mScaleX, g.mScaleY);
		}
		float num = 0.5f;
		float num2 = 5f;
		float num3 = 8f;
		bool flag3 = true;
		switch (seedType)
		{
		case SeedType.Tallnut:
			num = 0.3f;
			num2 = 12f;
			num3 = 22f;
			break;
		case SeedType.InstantCoffee:
			num = 0.55f;
			num2 = 0f;
			num3 = 9f;
			break;
		case SeedType.Cobcannon:
			num = 0.26f;
			num2 = 6f;
			num3 = 22f;
			break;
		case SeedType.Cactus:
			num2 = 9f;
			num3 = 13f;
			break;
		case SeedType.Potatomine:
			num = 0.4f;
			num2 = 8f;
			num3 = 12f;
			break;
		case SeedType.Magnetshroom:
			num3 = 12f;
			break;
		case SeedType.Chomper:
		case SeedType.Fumeshroom:
		case SeedType.Hypnoshroom:
		case SeedType.Doomshroom:
		case SeedType.Squash:
		case SeedType.Tanglekelp:
		case SeedType.Spikeweed:
		case SeedType.Torchwood:
		case SeedType.Plantern:
		case SeedType.Pumpkinshell:
		case SeedType.Spikerock:
			num = 0.4f;
			num2 = 8f;
			num3 = 12f;
			break;
		case SeedType.Twinsunflower:
		case SeedType.Gloomshroom:
			num = 0.45f;
			num2 = 7f;
			num3 = 14f;
			break;
		case SeedType.Cattail:
			num = 0.45f;
			num2 = 8f;
			num3 = 13f;
			break;
		case SeedType.Umbrella:
			num2 = 5f;
			num3 = 10f;
			break;
		case SeedType.Kernelpult:
			num = 0.4f;
			num2 = 13f;
			num3 = 14f;
			break;
		case SeedType.Cabbagepult:
			num = 0.4f;
			num2 = 15f;
			num3 = 14f;
			break;
		case SeedType.Melonpult:
		case SeedType.Wintermelon:
			num = 0.35f;
			num2 = 18f;
			num3 = 19f;
			break;
		case SeedType.Gravebuster:
			num = 0.4f;
			num2 = 10f;
			num3 = 15f;
			break;
		case SeedType.Splitpea:
			num = 0.45f;
			num2 = 12f;
			num3 = 12f;
			break;
		case SeedType.Blover:
			num = 0.4f;
			num2 = 8f;
			num3 = 17f;
			break;
		case SeedType.Starfruit:
			num2 = 6f;
			break;
		case SeedType.Threepeater:
			num3 = 10f;
			break;
		case SeedType.Gatlingpea:
			num2 = 2f;
			break;
		case SeedType.ZombieNormal:
		case SeedType.ZombieTrafficCone:
		case SeedType.ZombiePail:
		case SeedType.ZombieDancer:
			num = 0.35f;
			num2 = -3f;
			num3 = -7f;
			break;
		case SeedType.ZombiePolevaulter:
			num = 0.35f;
			num2 = -8f;
			num3 = -12f;
			break;
		case SeedType.ZombieLadder:
		case SeedType.ZombieDigger:
		case SeedType.ZombieScreenDoor:
		case SeedType.ZombiePogo:
			num = 0.35f;
			num2 = -3f;
			num3 = -10f;
			break;
		case SeedType.ZombieBungee:
			num = 0.3f;
			num2 = 1f;
			num3 = -1f;
			break;
		case SeedType.ZombieFootball:
			num = 0.33f;
			num2 = -7f;
			num3 = -9f;
			break;
		case SeedType.ZombieBalloon:
			num = 0.35f;
			num2 = -3f;
			num3 = -5f;
			break;
		case SeedType.ZombieImp:
			num = 0.4f;
			num2 = -12f;
			num3 = -17f;
			break;
		case SeedType.Zomboni:
			num = 0.23f;
			num2 = 12f;
			num3 = 3f;
			break;
		case SeedType.ZombieGargantuar:
			num = 0.23f;
			num2 = 4f;
			num3 = 3f;
			break;
		case SeedType.BeghouledButtonShuffle:
		case SeedType.BeghouledButtonCrater:
		case SeedType.SlotMachineSun:
		case SeedType.SlotMachineDiamond:
		case SeedType.ZombiquariumSnorkel:
		case SeedType.ZombiquariumTrophy:
			flag3 = false;
			break;
		}
		bool flag4 = GlobalStaticVars.gLawnApp.mGameMode == GameMode.ChallengeBigTime;
		bool flag5 = flag4;
		if (flag5)
		{
			flag = ((seedType == SeedType.Sunflower || seedType == SeedType.Wallnut || seedType == SeedType.Marigold) ? true : false);
			flag5 = flag;
		}
		if (flag5)
		{
			num2 = 16f;
			num3 = 34f;
		}
		if (seedType == SeedType.GiantWallnut)
		{
			num *= 0.75f;
			num2 = 52f;
			num3 = 58f;
		}
		num2 *= g.mScaleX;
		num3 = (num3 + 1f) * g.mScaleY;
		if (ModPlants.IsMod(seedType))
		{
			// the pack's card inside the classic packet frame
			int cw = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth();
			int ch = AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelHeight();
			ModPlants.DrawCard(g, seedType, x + cw * 0.1f * g.mScaleX, y + ch * 0.08f * g.mScaleY, cw * 0.8f * g.mScaleX, ch * 0.62f * g.mScaleY);
		}
		else if (flag3)
		{
			DrawClassicSeed(g, x, y, theSeedType, theImitaterType, num2, num3, num);
		}
		if (thePercentDark > 0f)
		{
			int theHeight = TodCommon.FloatRoundToInt(68f * thePercentDark) + 2;
			Graphics graphics = Graphics.GetNew(g);
			graphics.SetColor(new SexyColor(64, 64, 64, 255));
			graphics.SetColorizeImages(colorizeImages: true);
			graphics.ClipRect((int)x, (int)y, AtlasResources.IMAGE_CLASSIC_SEEDS.GetCelWidth(), theHeight);
			TodCommon.TodDrawImageCelScaledF(graphics, theImageStrip, x, y, theCelCol, 0, graphics.mScaleX, graphics.mScaleY);
			if (flag3)
			{
				DrawClassicSeed(graphics, x, y, theSeedType, theImitaterType, num2, num3, num);
			}
			graphics.PrepareForReuse();
		}
		if (theDrawCost)
		{
			LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
			string text;
			if (gLawnApp.mBoard == null || !gLawnApp.mBoard.PlantUsesAcceleratedPricing(seedType))
			{
				text = LawnApp.ToString(Plant.GetCost(theSeedType, theImitaterType));
			}
			else
			{
				text = (theUseCurrentCost ? gLawnApp.mBoard.GetCurrentPlantCost(theSeedType, theImitaterType).ToString() : Common.StrFormat_(TodStringFile.TodStringTranslate("[SEED_PACKET_COST_PLUS]"), Plant.GetCost(theSeedType, theImitaterType)));
			}
			int num4 = 32 - (int)((float)(Resources.FONT_PICO129.StringWidth(text) * 2) / 3f);
			int num5 = Resources.FONT_PICO129.GetAscent() + 54 + 16;
			TodCommon.TodDrawString(g, text, TodCommon.FloatRoundToInt(x + (float)num4 * g.mScaleX), TodCommon.FloatRoundToInt(y + (float)num5 * g.mScaleY), Resources.FONT_PICO129, SexyColor.Black, DrawStringJustification.Left, g.mScaleX * 2f / 3f);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	private static void DrawClassicSeed(Graphics g, float x, float y, SeedType theSeedType, SeedType theImitaterType, float theOffsetX, float theOffsetY, float theScale)
	{
		bool flag = !g.IsHardWareClipping();
		if (flag)
		{
			g.HardwareClip();
		}
		Image image = theSeedType switch
		{
			SeedType.CornRouletteWatercan => AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_GOLD, 
			SeedType.CornRouletteChocolate => AtlasResources.IMAGE_CHOCOLATE, 
			SeedType.CornRouletteFertilizer => AtlasResources.IMAGE_REANIM_ZENGARDEN_FERTILIZER_BAG1, 
			SeedType.CornRouletteMagnifier => AtlasResources.IMAGE_MAGNIFIER, 
			_ => null, 
		};
		if (image != null)
		{
			float num = Math.Min(38f / (float)image.mWidth, 40f / (float)image.mHeight);
			float thePosX = x + (6f + (38f - (float)image.mWidth * num) * 0.5f) * g.mScaleX;
			float thePosY = y + (10f + (40f - (float)image.mHeight * num) * 0.5f) * g.mScaleY;
			TodCommon.TodDrawImageScaledF(g, image, thePosX, thePosY, num * g.mScaleX, num * g.mScaleY);
			if (flag)
			{
				g.EndHardwareClip();
			}
			return;
		}
		Image image2 = AtlasResources.IMAGE_CLASSIC_PACKET_PLANTS;
		SeedType seedType = theSeedType;
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			seedType = theImitaterType;
			bool flag2;
			switch (theImitaterType)
			{
			case SeedType.Potatomine:
			case SeedType.Hypnoshroom:
			case SeedType.Lilypad:
			case SeedType.Squash:
			case SeedType.Garlic:
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			bool flag3 = flag2;
			image2 = FilterEffect.FilterEffectGetImage(image2, flag3 ? FilterEffectType.LessWashedOut : FilterEffectType.WashedOut);
		}
		int num2 = seedType switch
		{
			SeedType.Potatomine => 0, 
			SeedType.Chomper => 1, 
			SeedType.Hypnoshroom => 2, 
			SeedType.Tallnut => 3, 
			SeedType.Blover => 4, 
			SeedType.Pumpkinshell => 5, 
			SeedType.Twinsunflower => 6, 
			SeedType.Cobcannon => 7, 
			SeedType.Cabbagepult => 8, 
			SeedType.Kernelpult => 9, 
			SeedType.Melonpult => 10, 
			SeedType.Wintermelon => 11, 
			SeedType.Spikerock => 12, 
			_ => -1, 
		};
		if (num2 >= 0 && g.mScaleX <= 1f)
		{
			TodCommon.TodDrawImageCelScaledF(g, image2, x, y, num2, 0, g.mScaleX, g.mScaleY);
			if (flag)
			{
				g.EndHardwareClip();
			}
			return;
		}
		Graphics graphics = Graphics.GetNew(g);
		graphics.mScaleX = theScale * g.mScaleX;
		graphics.mScaleY = theScale * g.mScaleY;
		float num3 = x + theOffsetX;
		float num4 = y + theOffsetY;
		if (!Challenge.IsZombieSeedType(seedType) && seedType != SeedType.Sprout)
		{
			num3 += 40f * Constants.S * graphics.mScaleX;
			num4 += (8f + 56f * Constants.S) * graphics.mScaleY;
		}
		Plant.DrawSeedType(graphics, theSeedType, theImitaterType, DrawVariation.Normal, num3, num4);
		graphics.PrepareForReuse();
		if (flag)
		{
			g.EndHardwareClip();
		}
	}

	public static void DrawSmallSeedPacket(Graphics g, float x, float y, SeedType theSeedType, SeedType theImitaterType, float thePercentDark, int theGrayness, bool theDrawCost, bool theUseCurrentCost, bool theDrawBackground, bool theDrawCostBackground, bool canShowName = true)
	{
		SeedType seedType = theSeedType;
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			seedType = theImitaterType;
		}
		if (theSeedType == SeedType.ImitaterRandomPlant || theSeedType == SeedType.ImitaterRandomZombie)
		{
			seedType = SeedType.Imitater;
		}
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
		g.SetColor(new SexyColor(255, 255, 255, 255));
		if (theGrayness != 255)
		{
			g.SetColor(new SexyColor(theGrayness, theGrayness, theGrayness));
			g.SetColorizeImages(colorizeImages: true);
		}
		else if (thePercentDark > 0f)
		{
			g.SetColor(new SexyColor(128, 128, 128, 255));
			g.SetColorizeImages(colorizeImages: true);
		}
		if (theDrawBackground)
		{
			switch (theSeedType)
			{
			case SeedType.SlotMachineSun:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_SUN, x, y);
				break;
			case SeedType.SlotMachineDiamond:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_DIAMOND, x, y);
				break;
			case SeedType.ZombiquariumSnorkel:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_ZOMBIEQUARIUM, x, y);
				break;
			case SeedType.ZombiquariumTrophy:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_TROPHY, x, y);
				break;
			case SeedType.BeghouledButtonShuffle:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_SHUFFLE, x, y);
				break;
			case SeedType.BeghouledButtonCrater:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_CRATER, x, y);
				break;
			case SeedType.CornRouletteWatercan:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_WATERCAN, x, y);
				break;
			case SeedType.CornRouletteChocolate:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_CHOCOLATE, x, y);
				break;
			case SeedType.CornRouletteFertilizer:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_FERTILIZER, x, y);
				break;
			case SeedType.CornRouletteMagnifier:
				g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_MAGNIFIER, x, y);
				break;
			default:
			{
				if (Challenge.IsZombieSeedType(seedType))
				{
					DrawIZombieSeedPacket(GlobalStaticVars.gLawnApp, g, x, y, seedType, thePercentDark, theGrayness, theDrawCost, theUseCurrentCost, theDrawBackground, theDrawCostBackground);
					break;
				}
				Image image = Resources.IMAGE_SEEDPACKETS;
				Image image2 = Resources.IMAGE_SEEDPACKETS_ADD;
				if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
				{
					image = FilterEffect.FilterEffectGetImage(image, FilterEffectType.WashedOut);
					image2 = FilterEffect.FilterEffectGetImage(image2, FilterEffectType.WashedOut);
				}
				if (ModPlants.IsMod(seedType))
				{
					if (!ModPlants.DrawCard(g, seedType, (int)x, (int)y - Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2, Resources.IMAGE_SEEDPACKETS.GetCelWidth(), Resources.IMAGE_SEEDPACKETS.GetCelHeight()))
					{
						g.DrawImageCel(image, (int)x, (int)y - Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2, (int)ModPlants.Get(seedType).Base);
					}
				}
				else if (seedType >= SeedType.SuperChomper && seedType < SeedType.Imitater)
				{
					g.DrawImageCel(image2, (int)x, (int)y - Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2, (int)(seedType - 48));
				}
				else if (theSeedType == SeedType.ImitaterRandomZombie)
				{
					g.SetScale(0.75f, 0.5f, 0f, 0f);
					g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEWINDOW, x + (float)Constants.IZombie_SeedOffset.X, y + (float)Constants.IZombie_SeedOffset.Y);
					Graphics graphics = Graphics.GetNew(g);
					graphics.SetScale(1f);
					graphics.ClipRect((int)x + Constants.IZombie_ClipOffset.X, (int)y + Constants.IZombie_ClipOffset.Y, Constants.IZombie_ClipOffset.Width, Constants.IZombie_ClipOffset.Height);
					graphics.HardwareClip();
					graphics.SetColorizeImages(colorizeImages: false);
					graphics.EndHardwareClip();
					graphics.PrepareForReuse();
					g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEWINDOW2, x, y);
					g.SetScale(1f, 1f, 0f, 0f);
				}
				else if (seedType >= SeedType.Imitater && seedType <= SeedType.GiantWallnut)
				{
					g.DrawImageCel(image, (int)x, (int)y - Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2, (int)(seedType - 53 + 48));
				}
				else if (seedType == SeedType.Leftpeater)
				{
					g.DrawImageCel(image, (int)x, (int)y - Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2, 51);
				}
				else
				{
					g.DrawImageCel(image, (int)x, (int)y - Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2, (int)seedType);
				}
				if (theSeedType == SeedType.Imitater)
				{
					_ = -1;
				}
				if (seedType == SeedType.Cattail || seedType == SeedType.Imitater || seedType == SeedType.Spikerock || seedType == SeedType.Gatlingpea)
				{
					DrawDynamicTachieSmallSeedPacket(g, x, y, seedType);
				}
				if (seedType == SeedType.Cattail && GlobalStaticVars.gLawnApp.IsCattailSkinEnabled())
				{
					g.DrawImage(AtlasResources.IMAGE_STORE_CATTAIL_DRIVER_HYPNO_TINY, x, y);
				}
				break;
			}
			}
			if (GlobalStaticVars.gLawnApp.mPlayerInfo.mShowName & canShowName)
			{
				string theText = string.Empty;
				SeedType seedType2 = seedType;
				switch (seedType2)
				{
				case SeedType.SlotMachineSun:
					theText = "[SLOT_MACHINE_SUN_TOOLTIP]";
					break;
				case SeedType.SlotMachineDiamond:
					theText = "[SLOT_MACHINE_DIAMOND_TOOLTIP]";
					break;
				case SeedType.ZombiquariumSnorkel:
					theText = "[ZOMBIQUARIUM_SNORKEL_TOOLTIP]";
					break;
				case SeedType.ZombiquariumTrophy:
					theText = "[ZOMBIQUARIUM_TROPHY_TOOLTIP]";
					break;
				case SeedType.BeghouledButtonShuffle:
					theText = "[BEGHOULED_SHUFFLE_TOOLTIP]";
					break;
				case SeedType.BeghouledButtonCrater:
					theText = "[BEGHOULED_CRATER_TOOLTIP]";
					break;
				default:
					if (Challenge.IsZombieSeedType(seedType))
					{
						theText = Common.StrFormat_("[{0}]", Zombie.GetZombieDefinition(GetIZombieTypeFromSeed(seedType)).mZombieName);
					}
					else if (seedType2 > SeedType.None && (seedType2 < SeedType.SeedTypeCount || ModPlants.IsMod(seedType2)))
					{
						theText = Plant.GetNameString(seedType2, SeedType.None);
					}
					break;
				}
				if (GlobalStaticVars.gLawnApp.mGameMode == GameMode.ChallengeBeghouled || GlobalStaticVars.gLawnApp.mGameMode == GameMode.ChallengeBeghouledTwist)
				{
					switch (seedType2)
					{
					case SeedType.Repeater:
						theText = "[BEGHOULED_REPEATER_UPGRADE_TOOLTIP]";
						break;
					case SeedType.Fumeshroom:
						theText = "[BEGHOULED_FUMESHROOM_UPGRADE_TOOLTIP]";
						break;
					case SeedType.Tallnut:
						theText = "[BEGHOULED_TALLNUT_UPGRADE_TOOLTIP]";
						break;
					}
				}
				(int, int)[] array = new (int, int)[5]
				{
					(0, 0),
					(1, 0),
					(0, 1),
					(-1, 0),
					(0, -1)
				};
				Font fONT_BRIANNETOD = Resources.FONT_BRIANNETOD12;
				fONT_BRIANNETOD.mScaleX = 0.5f;
				fONT_BRIANNETOD.mScaleY = 0.5f;
				g.HardwareClip();
				for (int num = 4; num >= 0; num--)
				{
					TodStringFile.TodDrawStringWrapped(g, theText, new TRect(22 + array[num].Item1 + (int)x - 20, 33 + array[num].Item2 + (int)y - 52, 55, 80), fONT_BRIANNETOD, (num == 0) ? SexyColor.Black : new SexyColor(theGrayness, theGrayness, theGrayness), DrawStringJustification.CenterVerticalBottom, widowSupression: false);
				}
				g.EndHardwareClip();
				fONT_BRIANNETOD.mScaleX = 1f;
				fONT_BRIANNETOD.mScaleY = 1f;
			}
		}
		if (theDrawCostBackground)
		{
			Image theImage;
			if (theSeedType == SeedType.Imitater)
			{
				theImage = AtlasResources.IMAGE_SEEDPACKETS_GRAY_TAB;
			}
			else
			{
				theImage = ((theSeedType >= SeedType.Gatlingpea) ? AtlasResources.IMAGE_SEEDPACKETS_PURPLE_TAB : AtlasResources.IMAGE_SEEDPACKETS_GREEN_TAB);
			}
			int x2;
			int y2;
			if (GlobalStaticVars.gLawnApp.IsIZombieLevel())
			{
				x2 = Constants.SeedPacket_Cost_IZombie.X;
				y2 = Constants.SeedPacket_Cost_IZombie.Y;
			}
			else
			{
				x2 = Constants.SeedPacket_Cost.X;
				y2 = Constants.SeedPacket_Cost.Y;
			}
			g.DrawImageF(theImage, x + (float)x2, y + (float)y2);
		}
		if (thePercentDark > 0f)
		{
			int theHeight = TodCommon.FloatRoundToInt((float)Constants.SMALL_SEEDPACKET_HEIGHT * thePercentDark) + 2;
			g.SetColor(new SexyColor(0, 0, 0, 150));
			g.FillRect((int)x, (int)y, Constants.SMALL_SEEDPACKET_WIDTH, theHeight);
		}
		if (theDrawCost)
		{
			string empty = string.Empty;
			if (((LawnApp)GlobalStaticVars.gSexyAppBase).mBoard != null && ((LawnApp)GlobalStaticVars.gSexyAppBase).mBoard.PlantUsesAcceleratedPricing(seedType))
			{
				if (theUseCurrentCost)
				{
					empty = ((LawnApp)GlobalStaticVars.gSexyAppBase).mBoard.GetCurrentPlantCost(theSeedType, theImitaterType).ToString();
				}
				else
				{
					int cost = Plant.GetCost(theSeedType, theImitaterType);
					empty = Common.StrFormat_(TodStringFile.TodStringTranslate("[SEED_PACKET_COST_PLUS]"), cost);
				}
			}
			else
			{
				int cost2 = Plant.GetCost(theSeedType, theImitaterType);
				empty = LawnApp.ToString(cost2);
			}
			int x3;
			int y3;
			if (GlobalStaticVars.gLawnApp.IsIZombieLevel())
			{
				x3 = Constants.SeedPacket_CostText_IZombie_Pos.X;
				y3 = Constants.SeedPacket_CostText_IZombie_Pos.Y;
			}
			else
			{
				x3 = Constants.SeedPacket_CostText_Pos.X;
				y3 = Constants.SeedPacket_CostText_Pos.Y;
			}
			TodCommon.TodDrawString(g, empty, (int)x + x3, (int)y + y3, Resources.FONT_PICO129, SexyColor.Black, DrawStringJustification.Right);
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	private static void DrawDynamicTachieSmallSeedPacket(Graphics g, float x, float y, SeedType theSeedType)
	{
		int num = 0;
		int num2 = 0;
		float scale = 1f;
		int num3 = -Constants.SMALL_SEEDPACKET_HEIGHT;
		switch (theSeedType)
		{
		case SeedType.Cattail:
			num += 25;
			num2 += 200 - num3;
			scale = 1f / 28f;
			break;
		case SeedType.Imitater:
			num += 25;
			num2 += 150 - num3;
			scale = 1f / 12f;
			break;
		case SeedType.Spikerock:
			num += 30;
			num2 += 70 - num3;
			scale = 0.05f;
			break;
		case SeedType.Gatlingpea:
			num += 12;
			num2 += 170 - num3;
			scale = 0.05f;
			break;
		}
		ref SpineWidget reference = ref gDynamicTachieSeedPacketSpineWidgets[(int)theSeedType];
		if (reference == null)
		{
			switch (theSeedType)
			{
			case SeedType.Cattail:
				reference = new SpineWidget("190306", "190306", scale);
				reference.SetAnimationTrack("wait1", ReanimLoopType.Loop);
				break;
			case SeedType.Imitater:
				reference = new SpineWidget("imitater_tachie", "imitater_tachie", scale);
				reference.SetAnimationTrack("idle", ReanimLoopType.Loop);
				break;
			case SeedType.Spikerock:
				reference = new SpineWidget("spikerock_tachie", "spikerock_tachie", scale, SpineWidget.SkeletonFormat.Skel);
				reference.SetAnimationTrack("idle2", ReanimLoopType.Loop);
				break;
			case SeedType.Gatlingpea:
				reference = new SpineWidget("gatlingpea_tachie", "gatlingpea_tachie", scale, SpineWidget.SkeletonFormat.Skel);
				reference.SetAnimationTrack("idle2", ReanimLoopType.Loop);
				break;
			}
		}
		reference.Resize((int)x + g.mTransX + 5, (int)y + g.mTransY + num3, Constants.SMALL_SEEDPACKET_WIDTH - 12, Constants.SMALL_SEEDPACKET_HEIGHT - 8 - num3);
		reference.SetSkeletonPos(num, num2);
		reference.Update_FromDraw(GlobalStaticVars.gLawnApp.mAppCounter);
		reference.Draw(g);
	}

	private static void DrawIZombieSeedPacket(LawnApp theApp, Graphics g, float x, float y, SeedType aSeedType, float thePercentDark, int theGrayness, bool theDrawCost, bool theUseCurrentCost, bool theDrawBackground, bool theDrawCostBackground)
	{
		ZombieType iZombieTypeFromSeed = GetIZombieTypeFromSeed(aSeedType);
		if ((uint)iZombieTypeFromSeed < 40u)
		{
			g.SetScale(0.75f, 0.5f, 0f, 0f);
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEWINDOW, x + (float)Constants.IZombie_SeedOffset.X, y + (float)Constants.IZombie_SeedOffset.Y);
			Graphics graphics = Graphics.GetNew(g);
			graphics.ClipRect((int)x + Constants.IZombie_ClipOffset.X, (int)y + Constants.IZombie_ClipOffset.Y, Constants.IZombie_ClipOffset.Width, Constants.IZombie_ClipOffset.Height);
			graphics.HardwareClip();
			graphics.SetScale(1f);
			TPoint zombieCardPortraitOffset = GetZombieCardPortraitOffset(iZombieTypeFromSeed);
			float thePosX = x + (float)Constants.IZombie_ClipOffset.X + ((float)Constants.IZombie_ClipOffset.Width - 80f * Constants.S) / 2f + (float)zombieCardPortraitOffset.x * Constants.S;
			float thePosY = y + (float)Constants.IZombie_ClipOffset.Y + (float)zombieCardPortraitOffset.y * Constants.S;
			if ((aSeedType & SeedType.MindControlledCardFlag) != 0)
			{
				graphics.SetColorizeImages(colorizeImages: true);
				graphics.SetColor(GameConstants.ZOMBIE_MINDCONTROLLED_COLOR);
			}
			theApp.mReanimatorCache.DrawCachedZombieNew(graphics, thePosX, thePosY, iZombieTypeFromSeed);
			if ((aSeedType & SeedType.MindControlledCardFlag) != 0)
			{
				graphics.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
				theApp.mReanimatorCache.DrawCachedZombieNew(graphics, thePosX, thePosY, iZombieTypeFromSeed);
				graphics.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
			}
			graphics.SetColorizeImages(colorizeImages: false);
			graphics.EndHardwareClip();
			graphics.PrepareForReuse();
			g.DrawImage(AtlasResources.IMAGE_ALMANAC_ZOMBIEWINDOW2, x, y);
			g.SetScale(1f, 1f, 0f, 0f);
		}
	}

	private static TPoint GetZombieCardPortraitOffset(ZombieType type)
	{
		return type switch
		{
			ZombieType.Normal => new TPoint(-10, -18), 
			ZombieType.Flag => new TPoint(0, -15), 
			ZombieType.TrafficCone => new TPoint(-10, -18), 
			ZombieType.Polevaulter => new TPoint(-10, -22), 
			ZombieType.Pail => new TPoint(-8, -22), 
			ZombieType.Newspaper => new TPoint(-3, -22), 
			ZombieType.Door => new TPoint(-3, -19), 
			ZombieType.Football => new TPoint(-18, -22), 
			ZombieType.Dancer => new TPoint(-1, 5), 
			ZombieType.BackupDancer => new TPoint(-8, 15), 
			ZombieType.DuckyTube => new TPoint(-5, -19), 
			ZombieType.Snorkel => new TPoint(-26, -14), 
			ZombieType.Zamboni => new TPoint(-43, -30), 
			ZombieType.Bobsled => new TPoint(-8, -32), 
			ZombieType.DolphinRider => new TPoint(-6, -37), 
			ZombieType.JackInTheBox => new TPoint(3, -13), 
			ZombieType.Balloon => new TPoint(-3, -50), 
			ZombieType.Digger => new TPoint(-1, -29), 
			ZombieType.Pogo => new TPoint(-11, -15), 
			ZombieType.Yeti => new TPoint(1, -5), 
			ZombieType.Bungee => new TPoint(-2, -23), 
			ZombieType.Ladder => new TPoint(2, -33), 
			ZombieType.Catapult => new TPoint(-47, -12), 
			ZombieType.Gargantuar => new TPoint(28, -27), 
			ZombieType.Imp => new TPoint(-30, -55), 
			ZombieType.Boss => new TPoint(-30, -20), 
			ZombieType.PeaHead => new TPoint(-8, 2), 
			ZombieType.WallnutHead => new TPoint(-5, -22), 
			ZombieType.JalapenoHead => new TPoint(-10, 5), 
			ZombieType.GatlingHead => new TPoint(-18, 5), 
			ZombieType.SquashHead => new TPoint(-8, 5), 
			ZombieType.TallnutHead => new TPoint(5, 2), 
			ZombieType.RedeyeGargantuar => new TPoint(28, -27), 
			ZombieType.RobotTitan => new TPoint(26, 10), 
			ZombieType.RedeyeRobotTitan => new TPoint(26, 5), 
			ZombieType.Monk => new TPoint(-10, -25), 
			ZombieType.FootballPremium => new TPoint(-18, -22), 
			ZombieType.Ninja => new TPoint(-29, -25), 
			ZombieType.Talisman => new TPoint(-16, -25), 
			ZombieType.Propeller => new TPoint(-15, -17), 
			_ => new TPoint(-10, -18), 
		};
	}

	private static ZombieType GetIZombieTypeFromSeed(SeedType theSeedType)
	{
		return Challenge.IZombieSeedTypeToZombieType(theSeedType);
	}

	private void DrawSeedPacketSlotMachine(Graphics g, float x, float y, SeedType theSeedType, SeedType theImitaterType, int theGrayness, float scale)
	{
		SeedType seedType = theSeedType;
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			seedType = theImitaterType;
		}
		if (seedType == SeedType.Leftpeater)
		{
			seedType = SeedType.Sprout;
		}
		if (theGrayness != 255)
		{
			g.SetColor(new SexyColor(theGrayness, theGrayness, theGrayness));
			g.SetColorizeImages(colorizeImages: true);
		}
		switch (theSeedType)
		{
		case SeedType.SlotMachineSun:
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_SEEDPACKET_SUN, x, y, scale, scale);
			break;
		case SeedType.SlotMachineDiamond:
			TodCommon.TodDrawImageScaledF(g, AtlasResources.IMAGE_SEEDPACKET_DIAMOND, x, y, scale, scale);
			break;
		default:
		{
			if (Challenge.IsZombieSeedType(seedType))
			{
				DrawIZombieSeedPacket(GlobalStaticVars.gLawnApp, g, x, y, seedType, 1f, theGrayness, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false);
				break;
			}
			if (ModPlants.IsMod(seedType))
			{
				ModPlants.DrawCard(g, seedType, x, y - (float)(Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2) * scale, Resources.IMAGE_SEEDPACKETS.GetCelWidth() * scale, Resources.IMAGE_SEEDPACKETS.GetCelHeight() * scale);
				break;
			}
			int theCelCol = (int)seedType % Resources.IMAGE_SEEDPACKETS.mNumCols;
			int theCelRow = (int)seedType / Resources.IMAGE_SEEDPACKETS.mNumCols;
			TodCommon.TodDrawImageCelScaledF(g, Resources.IMAGE_SEEDPACKETS, x, y - (float)(Resources.IMAGE_SEEDPACKETS.GetCelHeight() / 2) * scale, theCelCol, theCelRow, scale, scale);
			break;
		}
		}
		g.SetColorizeImages(colorizeImages: false);
	}

	public void MouseDown(int x, int y, int theClickCount)
	{
		if (mBoard.mPaused || mApp.mGameScene != GameScenes.Playing || mPacketType == SeedType.None)
		{
			return;
		}
		if (mApp.IsSlotMachineLevel())
		{
			if (!mBoard.mAdvice.IsBeingDisplayed())
			{
				mBoard.DisplayAdvice("[ADVICE_SLOT_MACHINE_PULL]", MessageStyle.HintTallFast, AdviceType.None);
			}
			mBoard.mChallenge.mSlotMachineRollCount = Math.Min(mBoard.mChallenge.mSlotMachineRollCount, 2);
			return;
		}
		if (mApp.IsCornRouletteLevel())
		{
			if (mIndex < 4)
			{
				mBoard.mChallenge.CornRouletteUsePacket(this);
			}
			return;
		}
		SeedType seedType = mPacketType;
		if (seedType == SeedType.Imitater && mImitaterType != SeedType.None)
		{
			seedType = mImitaterType;
		}
		if (!mApp.mEasyPlantingCheat)
		{
			if (!mActive)
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
				mBuzzedCounter = 35;
				if (mApp.IsFirstTimeAdventureMode() && mBoard.mLevel == 1 && mBoard.mHelpDisplayed[0])
				{
					mBoard.DisplayAdvice("[ADVICE_SEED_REFRESH]", MessageStyle.TutorialLevel1, AdviceType.SeedRefresh);
				}
				return;
			}
			if (seedType == SeedType.Jalapeno && mBoard.mSealedCountdown > 0)
			{
				mBoard.DisplayAdvice("[ADVICE_JALAPENO_SEALED]", MessageStyle.TutorialLevel1, AdviceType.JalapenoSealed);
				return;
			}
			if (seedType == SeedType.PickledPepper && mBoard.mSealedCountdown > 0)
			{
				mBoard.DisplayAdvice("[ADVICE_PICKLED_PEPPER_SEALED]", MessageStyle.TutorialLevel1, AdviceType.PickledPepperSealed);
				return;
			}
			int currentPlantCost = mBoard.GetCurrentPlantCost(mPacketType, mImitaterType);
			if (!mBoard.CanTakeSunMoney(currentPlantCost) && !mBoard.HasConveyorBeltSeedBank())
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
				mBuzzedCounter = 35;
				mBoard.mOutOfMoneyCounter = 70;
				if (mApp.IsFirstTimeAdventureMode() && mBoard.mLevel == 1 && mBoard.mHelpDisplayed[0])
				{
					mBoard.DisplayAdvice("[ADVICE_CANT_AFFORD_PLANT]", MessageStyle.TutorialLevel1, AdviceType.CantAffordPlant);
				}
				return;
			}
			if (!mBoard.PlantingRequirementsMet(seedType) && !mBoard.HasTrashcan())
			{
				mApp.PlaySample(Resources.SOUND_BUZZER);
				mBuzzedCounter = 35;
				switch (seedType)
				{
				case SeedType.Gatlingpea:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_REPEATER]", MessageStyle.HintLong, AdviceType.PlantNeedsRepeater);
					break;
				case SeedType.Wintermelon:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_MELONPULT]", MessageStyle.HintLong, AdviceType.PlantNeedsMelonpult);
					break;
				case SeedType.Twinsunflower:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_SUNFLOWER]", MessageStyle.HintLong, AdviceType.PlantNeedsSunflower);
					break;
				case SeedType.Spikerock:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_SPIKEWEED]", MessageStyle.HintLong, AdviceType.PlantNeedsSpikeweed);
					break;
				case SeedType.Cobcannon:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_KERNELPULT]", MessageStyle.HintLong, AdviceType.PlantNeedsKernelpult);
					break;
				case SeedType.GoldMagnet:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_MAGNETSHROOM]", MessageStyle.HintLong, AdviceType.PlantNeedsMagnetshroom);
					break;
				case SeedType.Gloomshroom:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_FUMESHROOM]", MessageStyle.HintLong, AdviceType.PlantNeedsFumeshroom);
					break;
				case SeedType.Cattail:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_LILYPAD]", MessageStyle.HintLong, AdviceType.PlantNeedsLilypad);
					break;
				case SeedType.SuperChomper:
					mBoard.DisplayAdvice("[ADVICE_PLANT_NEEDS_CHOMPER]", MessageStyle.HintLong, AdviceType.PlantNeedsChomper);
					break;
				default:
					Debug.ASSERT(value: false);
					break;
				}
				return;
			}
		}
		mBoard.ClearAdvice(AdviceType.CantAffordPlant);
		mBoard.ClearAdvice(AdviceType.PlantNeedsRepeater);
		mBoard.ClearAdvice(AdviceType.PlantNeedsMelonpult);
		mBoard.ClearAdvice(AdviceType.PlantNeedsSunflower);
		mBoard.ClearAdvice(AdviceType.PlantNeedsKernelpult);
		mBoard.ClearAdvice(AdviceType.PlantNeedsSpikeweed);
		mBoard.ClearAdvice(AdviceType.PlantNeedsMagnetshroom);
		mBoard.ClearAdvice(AdviceType.PlantNeedsFumeshroom);
		mBoard.ClearAdvice(AdviceType.PlantNeedsLilypad);
		mBoard.ClearAdvice(AdviceType.PlantNeedsChomper);
		if (mApp.mGameMode == GameMode.ChallengeBeghouled || mApp.mGameMode == GameMode.ChallengeBeghouledTwist)
		{
			mBoard.mChallenge.BeghouledPacketClicked(this);
			return;
		}
		if (mApp.mGameMode == GameMode.ChallengeZombiquarium)
		{
			mBoard.mChallenge.ZombiquariumPacketClicked(this);
			return;
		}
		mBoard.mCursorObject.mType = mPacketType;
		mBoard.mCursorObject.mImitaterType = mImitaterType;
		mBoard.mCursorObject.mCursorType = CursorType.PlantFromBank;
		mBoard.mCursorObject.mSeedBankIndex = mIndex;
		mApp.PlaySample(Resources.SOUND_SEEDLIFT);
		if (mBoard.mTutorialState == TutorialState.Level1PickUpPeashooter)
		{
			mBoard.SetTutorialState(TutorialState.Level1PlantPeashooter);
		}
		else if (mBoard.mTutorialState == TutorialState.Level2PickUpSunflower)
		{
			if (mPacketType == SeedType.Sunflower)
			{
				mBoard.SetTutorialState(TutorialState.Level2PlantSunflower);
			}
			else
			{
				mBoard.SetTutorialState(TutorialState.Level2RefreshSunflower);
			}
		}
		else if (mBoard.mTutorialState == TutorialState.MoresunPickUpSunflower)
		{
			if (mPacketType == SeedType.Sunflower)
			{
				mBoard.SetTutorialState(TutorialState.MoresunPlantSunflower);
			}
			else
			{
				mBoard.SetTutorialState(TutorialState.MoresunRefreshSunflower);
			}
		}
		else if (mBoard.mTutorialState == TutorialState.WhackAZombiePickSeed || mBoard.mTutorialState == TutorialState.WhackAZombieBeforePickSeed)
		{
			mBoard.SetTutorialState(TutorialState.WhackAZombieCompleted);
		}
		Deactivate();
	}

	public bool MouseHitTest(int theX, int theY, HitResult theHitResult)
	{
		if (mSlotMachineCountDown > 0 || mPacketType == SeedType.None)
		{
			theHitResult.mObject = null;
			theHitResult.mObjectType = GameObjectType.None;
			return false;
		}
		if (theX >= mX && theX < mX + mWidth && theY >= mY + mOffsetY && theY < mY + mHeight + mOffsetY)
		{
			theHitResult.mObject = this;
			theHitResult.mObjectType = GameObjectType.Seedpacket;
			return true;
		}
		theHitResult.mObject = null;
		theHitResult.mObjectType = GameObjectType.None;
		return false;
	}

	public void Deactivate()
	{
		mActive = false;
		mRefreshCounter = 0;
		mRefreshTime = 0;
		mRefreshing = false;
	}

	public void Activate()
	{
		Debug.ASSERT(mPacketType != SeedType.None);
		mActive = true;
	}

	public void PickNextSlotMachineSeedForJsonLevel()
	{
		CSSlotMachineLevel component = mApp.mCreativeLevel.GetComponent<CSSlotMachineLevel>();
		if (component == null)
		{
			return;
		}
		int num = component.mAllowedPlants.Count + 2;
		TodWeightedArray[] array = new TodWeightedArray[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = TodWeightedArray.GetNewTodWeightedArray();
			SeedType seedType;
			int num2;
			if (i == num - 1)
			{
				seedType = SeedType.SlotMachineDiamond;
				num2 = 30;
				if (mApp.IsDIYMode())
				{
					num2 = 0;
				}
			}
			else if (i == num - 2)
			{
				seedType = SeedType.SlotMachineSun;
				num2 = 100;
			}
			else
			{
				(SeedType, int, int, int, int) tuple = component.mAllowedPlants[i];
				seedType = tuple.Item1;
				int item = tuple.Item2;
				int item2 = tuple.Item3;
				int item3 = tuple.Item4;
				int item4 = tuple.Item5;
				num2 = ((item != item2) ? TodCommon.TodAnimateCurve(item3, item4, mBoard.CountPlantByType(seedType), item, item2, TodCurves.Linear) : item2);
			}
			if (mIndex == 2 && seedType != SeedType.SlotMachineDiamond && (seedType == mBoard.mSeedBank.mSeedPackets[0].mSlotMachiningNextSeed || seedType == mBoard.mSeedBank.mSeedPackets[1].mSlotMachiningNextSeed))
			{
				num2 += num2 / 2;
			}
			array[i].mItem = seedType;
			array[i].mWeight = num2;
		}
		mSlotMachiningNextSeed = (SeedType)TodCommon.TodPickFromWeightedArray(array, num);
		for (int j = 0; j < num; j++)
		{
			array[j].PrepareForReuse();
		}
	}

	public void PickNextSlotMachineSeed()
	{
		if (mApp.IsLevelUseJson())
		{
			PickNextSlotMachineSeedForJsonLevel();
			return;
		}
		int theTimeAge = mBoard.CountPlantByType(SeedType.Peashooter);
		for (int i = 0; i < aSeedWeightArray.Length; i++)
		{
			if (aSeedWeightArray[i] != null)
			{
				aSeedWeightArray[i].PrepareForReuse();
			}
			aSeedWeightArray[i] = TodWeightedArray.GetNewTodWeightedArray();
		}
		int num = 0;
		for (int j = 0; j < 6; j++)
		{
			SeedType seedType = SLOT_SEED_TYPES[j];
			int num2 = 100;
			if (seedType == SeedType.Peashooter)
			{
				num2 = TodCommon.TodAnimateCurve(0, 5, theTimeAge, 200, 100, TodCurves.Linear);
			}
			if (seedType == SeedType.SlotMachineDiamond)
			{
				num2 = 30;
			}
			if (mIndex == 2 && seedType != SeedType.SlotMachineDiamond && (seedType == mBoard.mSeedBank.mSeedPackets[0].mSlotMachiningNextSeed || seedType == mBoard.mSeedBank.mSeedPackets[1].mSlotMachiningNextSeed))
			{
				num2 += num2 / 2;
			}
			aSeedWeightArray[num].mItem = (int)seedType;
			aSeedWeightArray[num].mWeight = num2;
			num++;
		}
		mSlotMachiningNextSeed = (SeedType)TodCommon.TodPickFromWeightedArray(aSeedWeightArray, num);
	}

	public void SlotMachineStart()
	{
		mSlotMachineCountDown = 400;
		mSlotMachiningPosition = 0f;
		PickNextSlotMachineSeed();
	}

	public void Discard()
	{
		Debug.ASSERT(mPacketType != SeedType.None);
		if (mBoard.HasConveyorBeltSeedBank())
		{
			mBoard.mSeedBank.RemoveSeed(mIndex);
			return;
		}
		Deactivate();
		if (!mApp.IsSlotMachineLevel())
		{
			mRefreshing = true;
			mRefreshTime = Plant.GetRefreshTime(mPacketType, mImitaterType);
		}
	}

	public void WasPlanted()
	{
		WasPlanted_Orig();
		if ((ZBCheatDialog.EnableNoCoolDownPlanting || ZBCheatDialog.ShowZombieHealth) && mRefreshing)
		{
			Activate();
		}
	}

	private void WasPlanted_Orig()
	{
		Debug.ASSERT(mPacketType != SeedType.None);
		if (mBoard.HasConveyorBeltSeedBank())
		{
			mBoard.mSeedBank.RemoveSeed(mIndex);
		}
		else if (mApp.IsSlotMachineLevel())
		{
			Deactivate();
		}
		else if (mApp.mGameMode == GameMode.ChallengeLastStand && mBoard.mChallenge.mChallengeState != ChallengeState.LastStandOnslaught)
		{
			mTimesUsed++;
			mActive = true;
			FlashIfReady();
		}
		else
		{
			mTimesUsed++;
			mRefreshing = true;
			mRefreshTime = Plant.GetRefreshTime(mPacketType, mImitaterType);
		}
	}

	public void FlashIfReady()
	{
		if (CanPickUp() && !mApp.mEasyPlantingCheat)
		{
			if (mBoard.mTutorialState == TutorialState.Level1RefreshPeashooter)
			{
				mBoard.SetTutorialState(TutorialState.Level1PickUpPeashooter);
			}
			else if (mBoard.mTutorialState == TutorialState.Level2RefreshSunflower && mPacketType == SeedType.Sunflower)
			{
				mBoard.SetTutorialState(TutorialState.Level2PickUpSunflower);
			}
			else if (mBoard.mTutorialState == TutorialState.MoresunRefreshSunflower && mPacketType == SeedType.Sunflower)
			{
				mBoard.SetTutorialState(TutorialState.MoresunPickUpSunflower);
			}
		}
	}

	public bool CanPickUp()
	{
		if (mBoard.mPaused || mApp.mGameScene != GameScenes.Playing)
		{
			return false;
		}
		if (mPacketType == SeedType.None)
		{
			return false;
		}
		SeedType theSeedType = mPacketType;
		if (mPacketType == SeedType.Imitater && mImitaterType != SeedType.None)
		{
			theSeedType = mImitaterType;
		}
		if (mApp.IsSlotMachineLevel())
		{
			return false;
		}
		if (!mApp.mEasyPlantingCheat)
		{
			if (!mActive)
			{
				return false;
			}
			int currentPlantCost = mBoard.GetCurrentPlantCost(mPacketType, mImitaterType);
			if (!mBoard.CanTakeSunMoney(currentPlantCost) && !mBoard.HasConveyorBeltSeedBank())
			{
				return false;
			}
			if (!mBoard.PlantingRequirementsMet(theSeedType))
			{
				return false;
			}
		}
		return true;
	}

	public void SetPacketType(SeedType theSeedType, SeedType theImitaterType)
	{
		SetPacketType_Orig(theSeedType, theImitaterType);
		if ((ZBCheatDialog.EnableNoCoolDownPlanting || ZBCheatDialog.ShowZombieHealth) && mRefreshing)
		{
			Activate();
		}
	}

	private void SetPacketType_Orig(SeedType theSeedType, SeedType theImitaterType)
	{
		mPacketType = theSeedType;
		mImitaterType = theImitaterType;
		mRefreshCounter = 0;
		mRefreshTime = 0;
		mRefreshing = false;
		mActive = true;
		SeedType theSeedtype = theSeedType;
		if (theSeedType == SeedType.Imitater && theImitaterType != SeedType.None)
		{
			theSeedtype = theImitaterType;
		}
		if (mApp.mGameMode != GameMode.ChallengeBeghouled && mApp.mGameMode != GameMode.ChallengeBeghouledTwist && mApp.mGameMode != GameMode.ChallengeZombiquarium && mApp.mGameMode != GameMode.ChallengeLastStand && !mApp.IsCornRouletteLevel() && !mApp.IsIZombieLevel() && !mApp.IsScaryPotterLevel() && !mApp.IsWhackAZombieLevel() && (!mApp.IsSurvivalMode() || mBoard.mChallenge.mSurvivalStage <= 0))
		{
			if ((Plant.IsUpgrade(theSeedtype) && !((LawnApp)GlobalStaticVars.gSexyAppBase).IsSurvivalMode()) || Plant.GetRefreshTime(mPacketType, mImitaterType) == 5000)
			{
				mRefreshTime = 3500;
				mRefreshing = true;
				mActive = false;
			}
			else if (Plant.IsUpgrade(theSeedtype) && ((LawnApp)GlobalStaticVars.gSexyAppBase).IsSurvivalMode())
			{
				mRefreshTime = 8000;
				mRefreshing = true;
				mActive = false;
			}
			else if (Plant.GetRefreshTime(mPacketType, mImitaterType) == 3000)
			{
				mRefreshTime = 2000;
				mRefreshing = true;
				mActive = false;
			}
		}
	}

	public void GetGraynessAndDarkness(ref int theGrayness, ref float thePercentDark)
	{
		float num = 0f;
		if (!mActive)
		{
			num = ((mRefreshTime != 0) ? ((float)(mRefreshTime - mRefreshCounter) / (float)mRefreshTime) : 1f);
		}
		SeedType theSeedType = mPacketType;
		if (mPacketType == SeedType.Imitater && mImitaterType != SeedType.None)
		{
			theSeedType = mImitaterType;
		}
		bool flag = true;
		if (mBoard.HasConveyorBeltSeedBank() || mApp.IsSlotMachineLevel())
		{
			flag = false;
		}
		int currentPlantCost = mBoard.GetCurrentPlantCost(mPacketType, mImitaterType);
		int num2 = 255;
		if (mApp.mGameMode == GameMode.ChallengeBeghouled && !mActive)
		{
			num2 = 64;
		}
		else if (mApp.mGameMode == GameMode.ChallengeBeghouledTwist && !mActive)
		{
			num2 = 64;
		}
		else if (mApp.mGameScene != GameScenes.Playing)
		{
			num2 = mBoard.mSeedBank.mCutSceneDarken;
			num = 0f;
		}
		else if (mBoard.mTutorialState == TutorialState.Level1PickUpPeashooter && mBoard.mTutorialTimer == -1 && mPacketType == SeedType.Peashooter)
		{
			num2 = TodCommon.GetFlashingColor(mBoard.mMainCounter, 75).mRed;
		}
		else if (mBoard.mTutorialState == TutorialState.Level2PickUpSunflower && mPacketType == SeedType.Sunflower)
		{
			num2 = TodCommon.GetFlashingColor(mBoard.mMainCounter, 75).mRed;
		}
		else if (mBoard.mTutorialState == TutorialState.MoresunPickUpSunflower && mPacketType == SeedType.Sunflower)
		{
			num2 = TodCommon.GetFlashingColor(mBoard.mMainCounter, 75).mRed;
		}
		else if (mBoard.mTutorialState == TutorialState.WhackAZombiePickSeed)
		{
			num2 = TodCommon.GetFlashingColor(mBoard.mMainCounter, 75).mRed;
		}
		else if (mApp.mEasyPlantingCheat)
		{
			num2 = 255;
			num = 0f;
		}
		else if (!mBoard.CanTakeSunMoney(currentPlantCost) & flag)
		{
			num2 = 128;
		}
		else if (num > 0f)
		{
			num2 = 128;
		}
		else if (!mBoard.PlantingRequirementsMet(theSeedType))
		{
			num2 = 128;
		}
		theGrayness = num2;
		thePercentDark = num;
	}

	public int CenterY()
	{
		return mY + mOffsetY + mHeight / 2;
	}

	public override string ToString()
	{
		return $"X:{mX}, Y:{mY},Seed:{mPacketType}";
	}
}
