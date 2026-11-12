using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

internal static class GlobalMembersStoreScreen
{
	public const int PURCHASE_COUNT_OFFSET = 1000;

	private static Dictionary<int, string> seedNumberUpgradeCache = new Dictionary<int, string>(3);

	private static Dictionary<int, string> cardGroupNumberUpgradeCache = new Dictionary<int, string>(3);

	private static PottedPlant tempPottedPlant = new PottedPlant();

	internal static void DrawStoreItemIcon(Graphics g, int aPosX, int aPosY, StoreItem theItemType, int aQuantity)
	{
		switch (theItemType)
		{
		case StoreItem.STORE_ITEM_PACKET_UPGRADE:
		{
			g.DrawImage(AtlasResources.IMAGE_STORE_PACKETUPGRADE, aPosX - Constants.StoreScreen_PacketUpgrade_X, aPosY + Constants.StoreScreen_PacketUpgrade_Y);
			if (aQuantity > 10)
			{
				aQuantity = 10;
			}
			if (!seedNumberUpgradeCache.TryGetValue(aQuantity, out var value2))
			{
				value2 = TodCommon.TodReplaceNumberString("[STORE_UPGRADE_SLOTS]", "{SLOTS}", aQuantity);
				seedNumberUpgradeCache.Add(aQuantity, value2);
			}
			TodStringFile.TodDrawStringWrapped(theRect: new TRect(aPosX, aPosY + Constants.StoreScreen_PacketUpgrade_Text_Size.Y, Constants.StoreScreen_PacketUpgrade_Text_Size.Width, Constants.StoreScreen_PacketUpgrade_Text_Size.Height), g: g, theText: value2, theFont: Resources.FONT_HOUSEOFTERROR16, theColor: SexyColor.White, theJustification: DrawStringJustification.CenterVerticalMiddle);
			return;
		}
		case StoreItem.STORE_ITEM_CARD_GROUP_UPGRADE:
			g.DrawImage(AtlasResources.IMAGE_CARD_GROUP, aPosX - 4, aPosY + 5, AtlasResources.IMAGE_CARD_GROUP.mWidth / 2, AtlasResources.IMAGE_CARD_GROUP.mHeight / 2);
			if (aQuantity != 1)
			{
				if (aQuantity > 4)
				{
					aQuantity = 4;
				}
				if (!cardGroupNumberUpgradeCache.TryGetValue(aQuantity, out var value))
				{
					value = TodCommon.TodReplaceNumberString("[STORE_UPGRADE_CARD_GROUPS]", "{CARD_GROUPS}", aQuantity);
					cardGroupNumberUpgradeCache.Add(aQuantity, value);
				}
				TodStringFile.TodDrawStringWrapped(theRect: new TRect(aPosX - 4, aPosY + 10, Constants.StoreScreen_PacketUpgrade_Text_Size.Width, Constants.StoreScreen_PacketUpgrade_Text_Size.Height), g: g, theText: value, theFont: Resources.FONT_HOUSEOFTERROR16, theColor: SexyColor.White, theJustification: DrawStringJustification.CenterVerticalMiddle);
			}
			return;
		case StoreItem.STORE_ITEM_BONUS_LAWN_MOWER:
			return;
		case StoreItem.STORE_ITEM_POOL_CLEANER:
			g.DrawImage(AtlasResources.IMAGE_ICON_POOLCLEANER, aPosX + Constants.StoreScreen_PoolCleaner_Offset_X, aPosY + Constants.StoreScreen_PoolCleaner_Offset_Y);
			return;
		case StoreItem.STORE_ITEM_RAKE:
			g.DrawImage(AtlasResources.IMAGE_SEEDPACKET_ZORROSE, aPosX - 3, aPosY + Constants.StoreScreen_Default_Offset_Y);
			return;
		case StoreItem.STORE_ITEM_ROOF_CLEANER:
			g.DrawImage(AtlasResources.IMAGE_ICON_ROOFCLEANER, aPosX + Constants.StoreScreen_RoofCleaner_Offset_X, aPosY + Constants.StoreScreen_RoofCleaner_Offset_Y);
			return;
		case StoreItem.STORE_ITEM_PLANT_IMITATER:
			SeedPacket.DrawSmallSeedPacket(g, aPosX + Constants.StoreScreen_Imitater_Offset_X, aPosY + Constants.StoreScreen_Imitater_Offset_Y, SeedType.Imitater, SeedType.None, 0f, 255, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false);
			return;
		case StoreItem.STORE_ITEM_FIRSTAID:
			g.DrawImage(AtlasResources.IMAGE_STORE_FIRSTAIDWALLNUTICON, aPosX, aPosY + Constants.StoreScreen_FirstAidNut_Offset_Y);
			return;
		case StoreItem.STORE_ITEM_MUSHROOM_GARDEN:
			g.DrawImage(AtlasResources.IMAGE_STORE_MUSHROOMGARDENICON, aPosX + Constants.StoreScreenMushroomGardenOffsetX, aPosY + 28, 50, 40);
			return;
		case StoreItem.STORE_ITEM_AQUARIUM_GARDEN:
			g.DrawImage(AtlasResources.IMAGE_STORE_AQUARIUMGARDENICON, aPosX + Constants.StoreScreenAquariumGardenOffsetX, aPosY + 28, 50, 40);
			return;
		case StoreItem.STORE_ITEM_GREENHOUSE_NIGHT:
			g.DrawImage(AtlasResources.IMAGE_STORE_GREENHOUSENIGHTGARDENICON, aPosX + Constants.StoreScreenMushroomGardenOffsetX, aPosY + 28, 50, 40);
			return;
		case StoreItem.STORE_ITEM_STINKY_THE_SNAIL:
			g.DrawImage(AtlasResources.IMAGE_REANIM_STINKY_TURN3, aPosX, aPosY + 26);
			return;
		case StoreItem.STORE_ITEM_GOLD_WATERINGCAN:
			if (aQuantity >= 2)
			{
				g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_DIAMOND, aPosX + Constants.StoreScreenWateringCanOffsetX, aPosY + Constants.StoreScreenWateringCanOffsetY);
			}
			else
			{
				g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_WATERINGCAN1_GOLD, aPosX + Constants.StoreScreenWateringCanOffsetX, aPosY + Constants.StoreScreenWateringCanOffsetY);
			}
			return;
		case StoreItem.STORE_ITEM_FERTILIZER:
			g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_FERTILIZER_BAG1, aPosX + Constants.StoreScreenPlantFoodOffsetX, aPosY + Constants.StoreScreenPlantFoodOffsetY);
			TodCommon.TodDrawString(g, "x5", aPosX + 50, aPosY + 65, Resources.FONT_HOUSEOFTERROR16, Color.White, DrawStringJustification.Right);
			return;
		case StoreItem.STORE_ITEM_PHONOGRAPH:
			g.DrawImage(AtlasResources.IMAGE_PHONOGRAPH, aPosX + 3, aPosY + Constants.StoreScreenPhonographOffsetY);
			return;
		case StoreItem.STORE_ITEM_BUG_SPRAY:
			g.DrawImage(AtlasResources.IMAGE_REANIM_ZENGARDEN_BUGSPRAY_BOTTLE, aPosX + Constants.StoreScreenBugSprayOffsetX, aPosY + Constants.StoreScreenBugSprayOffsetY);
			TodCommon.TodDrawString(g, "x5", aPosX + 47, aPosY + 65, Resources.FONT_HOUSEOFTERROR16, Color.White, DrawStringJustification.Right);
			return;
		case StoreItem.STORE_ITEM_GARDENING_GLOVE:
			g.DrawImage(AtlasResources.IMAGE_ZEN_GARDENGLOVE, aPosX + 5, aPosY + Constants.StoreScreenGloveOffsetY);
			return;
		case StoreItem.STORE_ITEM_WHEEL_BARROW:
			g.DrawImage(AtlasResources.IMAGE_ZEN_WHEELBARROW, aPosX + 5, aPosY + Constants.StoreScreenWheelbarrowOffsetY);
			return;
		case StoreItem.STORE_ITEM_TREE_OF_WISDOM:
			g.DrawImage(AtlasResources.IMAGE_STORE_TREEOFWISDOMICON, aPosX + Constants.StoreScreenMushroomGardenOffsetX, aPosY + 28, 50, 40);
			return;
		case StoreItem.STORE_ITEM_TREE_FOOD:
			g.DrawImage(AtlasResources.IMAGE_REANIM_TREEFOOD, aPosX + Constants.StoreScreenWateringCanOffsetX, aPosY + Constants.StoreScreenWateringCanOffsetY);
			return;
		case StoreItem.STORE_ITEM_GARLICFIRSTAID:
		{
			float mScaleX2 = g.mScaleX;
			float mScaleY2 = g.mScaleY;
			g.SetScale(1f / 3f);
			g.DrawImage(AtlasResources.IMAGE_STORE_GARLIC_SOCKS, aPosX - 15, aPosY + Constants.StoreScreen_FirstAidNut_Offset_Y - 35);
			g.SetScaleX(mScaleX2);
			g.SetScaleY(mScaleY2);
			return;
		}
		case StoreItem.STORE_ITEM_AGAVE_SKILL:
		{
			float mScaleX = g.mScaleX;
			float mScaleY = g.mScaleY;
			g.SetScale(0.625f);
			g.DrawImage(AtlasResources.IMAGE_AGAVE_SKILL, aPosX, aPosY + Constants.StoreScreen_FirstAidNut_Offset_Y - 10);
			g.SetScaleX(mScaleX);
			g.SetScaleY(mScaleY);
			return;
		}
		case StoreItem.STORE_ITEM_CATTAIL_DRIVER_HYPNO:
			g.DrawImage(AtlasResources.IMAGE_STORE_CATTAIL_DRIVER_HYPNO, aPosX - 20, aPosY + Constants.StoreScreen_FirstAidNut_Offset_Y - 10);
			return;
		case StoreItem.STORE_ITEM_CATTAIL_DRIVER:
			g.DrawImage(AtlasResources.IMAGE_STORE_CATTAIL_DRIVER, aPosX - 20, aPosY + Constants.StoreScreen_FirstAidNut_Offset_Y - 10);
			return;
		case StoreItem.STORE_ITEM_FUMESHROOM_GNOME:
			g.DrawImage(AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME, aPosX + 10, aPosY + Constants.StoreScreen_FirstAidNut_Offset_Y, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME.mWidth / 2, AtlasResources.IMAGE_REANIM_SELECTORSCREEN_FUMESHROOM_GNOME.mHeight / 2);
			return;
		}
		if (StoreScreen.IsPottedPlant(theItemType))
		{
			LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
			tempPottedPlant.InitializePottedPlant(SeedType.Marigold);
			tempPottedPlant.mFacing = PottedPlant.FacingDirection.Right;
			gLawnApp.mZenGarden.DrawPottedPlantIcon(g, aPosX + Constants.StoreScreen_PotPlant_Offset.X, aPosY + Constants.StoreScreen_PotPlant_Offset.Y, tempPottedPlant);
			return;
		}
		SeedType theSeedType = (SeedType)(theItemType + 40);
		switch (theItemType)
		{
		case StoreItem.STORE_ITEM_PLANT_SUPER_CHOMPER:
			theSeedType = SeedType.SuperChomper;
			break;
		case StoreItem.STORE_ITEM_PLANT_PICKLED_PEPPER:
			theSeedType = SeedType.PickledPepper;
			break;
		case StoreItem.STORE_ITEM_PLANT_FIRE_SHROOM:
			theSeedType = SeedType.FireShroom;
			break;
		case StoreItem.STORE_ITEM_PLANT_AGAVE:
			theSeedType = SeedType.Agave;
			break;
		case StoreItem.STORE_ITEM_PLANT_IRIS_SHOOTER:
			theSeedType = SeedType.IrisShooter;
			break;
		}
		SeedPacket.DrawSmallSeedPacket(g, aPosX + -3, aPosY + Constants.StoreScreen_Default_Offset_Y, theSeedType, SeedType.None, 0f, 255, theDrawCost: false, theUseCurrentCost: false, theDrawBackground: true, theDrawCostBackground: false);
	}

	internal static void DrawStoreItem(Graphics g, int aPosX, int aPosY, StoreItem theItemType, bool isComingSoon, bool isSoldOut, int aQuantity, int aCost)
	{
		DrawStoreItemIcon(g, aPosX, aPosY, theItemType, aQuantity);
		g.DrawImage(AtlasResources.IMAGE_STORE_PRICETAG, aPosX - Constants.StoreScreen_PriceTag_X, aPosY + Constants.StoreScreen_PriceTag_Y);
		string moneyString = LawnApp.GetMoneyString(aCost);
		TodCommon.TodDrawString(g, moneyString, aPosX + Constants.StoreScreen_PriceTag_Text_Offset_X, aPosY + Constants.StoreScreen_PriceTag_Text_Offset_Y, Resources.FONT_BRIANNETOD12, new SexyColor(0, 0, 0), DrawStringJustification.Center);
		if (isComingSoon)
		{
			TRect theRect = new TRect(aPosX - (int)(Constants.S * 12f), aPosY, Constants.StoreScreen_ComingSoon_X, Constants.StoreScreen_ComingSoon_Y);
			if (theItemType == StoreItem.STORE_ITEM_PLANT_TWINSUNFLOWER || theItemType == StoreItem.STORE_ITEM_PACKET_UPGRADE)
			{
				theRect.mX -= (int)(Constants.S * 4f);
			}
			TodStringFile.TodDrawStringWrapped(g, "[COMING_SOON]", theRect, Resources.FONT_HOUSEOFTERROR16, new SexyColor(255, 0, 0), DrawStringJustification.CenterVerticalMiddle);
		}
		else if (isSoldOut)
		{
			TRect theRect2 = new TRect(aPosX, aPosY + Constants.StoreScreen_SoldOut_Y, Constants.StoreScreen_SoldOut_Width, Constants.StoreScreen_SoldOut_Height);
			TodStringFile.TodDrawStringWrapped(g, "[SOLD_OUT]", theRect2, Resources.FONT_HOUSEOFTERROR16, new SexyColor(255, 0, 0), DrawStringJustification.CenterVerticalMiddle);
		}
	}
}
