using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ReanimatorCache
{
	public struct CachedPlantVariation
	{
		public SeedType mSeedType;

		public DrawVariation mDrawVariation;
	}

	public Dictionary<CachedPlantVariation, MemoryImage> mImageVariationList = new Dictionary<CachedPlantVariation, MemoryImage>();

	public List<MemoryImage> mPlantImages = new List<MemoryImage>();

	public List<MemoryImage> mLawnMowers = new List<MemoryImage>();

	public List<MemoryImage> mZombieImages = new List<MemoryImage>();

	public Dictionary<ZombieType, TRect> mZombieImageBounds = new Dictionary<ZombieType, TRect>();

	public LawnApp mApp;

	public void ReanimatorCacheInitialize()
	{
		for (int i = 0; i < (int)SeedType.SeedTypeCount; i++)
		{
			mPlantImages.Add(null);
		}
		for (int j = 0; j < 4; j++)
		{
			mLawnMowers.Add(null);
		}
		for (int k = 0; k < 42; k++)
		{
			mZombieImages.Add(null);
		}
		mApp = GlobalStaticVars.gLawnApp;
	}

	public void ReanimatorCacheDispose()
	{
		mLawnMowers = null;
		mPlantImages = null;
		mImageVariationList = null;
		mZombieImages = null;
		mZombieImageBounds.Clear();
	}

	public void DrawCachedPlant(Graphics g, float thePosX, float thePosY, SeedType theSeedType, DrawVariation theDrawVariation)
	{
		MemoryImage value = null;
		if (theDrawVariation == DrawVariation.Normal || theDrawVariation == DrawVariation.Imitater || theDrawVariation == DrawVariation.ImitaterLess)
		{
			if (mPlantImages[(int)theSeedType] == null)
			{
				Debug.Log(DebugType.Warn, $"(!!) ReanimatorCache uninitialized plant variation ({theSeedType}).\n");
				return;
			}
			value = mPlantImages[(int)theSeedType];
		}
		else if (!mImageVariationList.TryGetValue(new CachedPlantVariation
		{
			mSeedType = theSeedType,
			mDrawVariation = theDrawVariation
		}, out value))
		{
			Debug.Log(DebugType.Warn, $"(!!) ReanimatorCache uninitialized plant variation ({theSeedType})-({theDrawVariation}).\n");
			return;
		}
		TRect plantImageSize = GetPlantImageSize(theSeedType);
		if (!mApp.Is3DAccelerated() && g.mScaleX == 1f && g.mScaleY == 1f)
		{
			g.DrawImage(value, thePosX + Constants.S * (float)plantImageSize.mX, thePosY + Constants.S * (float)plantImageSize.mY);
		}
		else
		{
			TodCommon.TodDrawImageScaledF(g, value, thePosX + Constants.S * ((float)plantImageSize.mX * g.mScaleX), thePosY + Constants.S * ((float)plantImageSize.mY * g.mScaleY), g.mScaleX, g.mScaleY);
		}
	}

	public void DrawCachedZombieNew(Graphics g, float thePosX, float thePosY, ZombieType theZombieType)
	{
		if (theZombieType >= ZombieType.Normal && (int)theZombieType < mZombieImages.Count && theZombieType != ZombieType.ZombieTypesCount)
		{
			MemoryImage memoryImage = mZombieImages[(int)theZombieType];
			if (memoryImage == null)
			{
				Debug.Log(DebugType.Warn, $"(!!) ReanimatorCache uninitialized zombie variation ({theZombieType}).\n");
			}
			else
			{
				TodCommon.TodDrawImageScaledF(g, memoryImage, thePosX - Constants.S * 20f * g.mScaleX, thePosY - Constants.S * 20f * g.mScaleY, g.mScaleX, g.mScaleY);
			}
		}
	}

	public void DrawCachedZombieThumbnail(Graphics g, float x, float y, ZombieType type)
	{
		if (mZombieImageBounds.TryGetValue(type, out var value))
		{
			DrawCachedZombieNew(g, x + (20f * Constants.S - (float)value.mX) * g.mScaleX, y + (20f * Constants.S - (float)value.mY) * g.mScaleY, type);
		}
	}

	private void CacheZombieImageBounds(ZombieType type, MemoryImage image)
	{
		int width = image.Texture.Width;
		int height = image.Texture.Height;
		Color[] array = TexturePixelReader.ReadRegion(image.Texture, image.Texture.Bounds);
		int num = width;
		int num2 = height;
		int num3 = -1;
		int num4 = -1;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (array[i * width + j].A != 0)
				{
					num = Math.Min(num, j);
					num2 = Math.Min(num2, i);
					num3 = Math.Max(num3, j);
					num4 = Math.Max(num4, i);
				}
			}
		}
		mZombieImageBounds[type] = ((num3 < num) ? default(TRect) : new TRect((int)((float)num / image.mInvScale), (int)((float)num2 / image.mInvScale), (int)Math.Ceiling((float)(num3 + 1) / image.mInvScale) - (int)((float)num / image.mInvScale), (int)Math.Ceiling((float)(num4 + 1) / image.mInvScale) - (int)((float)num2 / image.mInvScale)));
	}

	public void DrawCachedMower(Graphics g, float thePosX, float thePosY, LawnMowerType mowerType)
	{
		if (mLawnMowers[(int)mowerType] == null)
		{
			Debug.Log(DebugType.Warn, $"(!!) ReanimatorCache uninitialized mower variation ({mowerType}).\n");
		}
		else
		{
			TodCommon.TodDrawImageScaledF(g, mLawnMowers[(int)mowerType], thePosX - Constants.S * 20f, thePosY, g.mScaleX, g.mScaleY);
		}
	}

	public TRect GetPlantImageSize(SeedType seedtype)
	{
		TRect result = new TRect(-20, -20, 120, 120);
		switch (seedtype)
		{
		case SeedType.Tallnut:
			result.mY -= 20;
			result.mHeight += 40;
			break;
		case SeedType.Melonpult:
		case SeedType.Wintermelon:
			result.mX -= 20;
			result.mWidth += 40;
			break;
		case SeedType.Cobcannon:
			result.mWidth += 80;
			break;
		case SeedType.Squash:
		case SeedType.Umbrella:
			result.mX -= 180;
			result.mY -= 195;
			result.mWidth += 180;
			result.mHeight += 195;
			break;
		case SeedType.Peashooter:
		case SeedType.Sunflower:
		case SeedType.Snowpea:
		case SeedType.Chomper:
		case SeedType.Repeater:
		case SeedType.Puffshroom:
		case SeedType.Fumeshroom:
		case SeedType.Hypnoshroom:
		case SeedType.Iceshroom:
		case SeedType.Lilypad:
		case SeedType.Jalapeno:
		case SeedType.Spikeweed:
		case SeedType.Plantern:
		case SeedType.Blover:
		case SeedType.Splitpea:
		case SeedType.Pumpkinshell:
		case SeedType.Kernelpult:
		case SeedType.Garlic:
		case SeedType.Gatlingpea:
		case SeedType.Cattail:
		case SeedType.Spikerock:
		case SeedType.Agave:
		case SeedType.Endoflame:
		case SeedType.Imitater:
		case SeedType.Leftpeater:
		case SeedType.ImitaterRandomPlant:
		case SeedType.ImitaterRandomZombie:
		case SeedType.HypnoCattail:
		case SeedType.SunflowerPea:
		case SeedType.PeaWallnut:
		case SeedType.SunflowerWallnut:
			result.mX -= 40;
			result.mY -= 35;
			result.mWidth += 40;
			result.mHeight += 35;
			break;
		case SeedType.Gravebuster:
			result.mX -= 180;
			result.mY -= 195;
			result.mWidth += 180;
			result.mHeight += 195;
			break;
		}
		return result;
	}

	public void DrawReanimatorFrame(Graphics g, float x, float y, ReanimationType reanimType, string trackName, DrawVariation variation)
	{
		Reanimation newReanimation = Reanimation.GetNewReanimation();
		newReanimation.ReanimationInitializeType(x, y, reanimType);
		if (trackName != null && newReanimation.TrackExists(trackName))
		{
			newReanimation.SetFramesForLayer(trackName);
		}
		if (reanimType == ReanimationType.Kernelpult)
		{
			newReanimation.AssignRenderGroupToTrack("Cornpult_butter", -1);
		}
		if (reanimType == ReanimationType.Sunflower)
		{
			newReanimation.mAnimTime = 0.15f;
		}
		newReanimation.AssignRenderGroupToTrack("zombie_butter", -1);
		newReanimation.AssignRenderGroupToTrack("anim_waterline", -1);
		if (g.mColorizeImages)
		{
			newReanimation.mColorOverride = g.mColor;
		}
		newReanimation.OverrideScale(g.mScaleX, g.mScaleY);
		UpdateReanimationforVariation(newReanimation, variation);
		newReanimation.Draw(g);
		newReanimation.PrepareForReuse();
	}

	public void UpdateReanimationforVariation(Reanimation reanimation, DrawVariation variation)
	{
		if (variation != DrawVariation.Normal)
		{
			if (DrawVariation.MarigoldWhite <= variation && variation <= DrawVariation.MarigoldLightGreen)
			{
				List<SexyColor> list = new List<SexyColor>
				{
					new SexyColor(255, 255, 255),
					new SexyColor(230, 30, 195),
					new SexyColor(250, 125, 5),
					new SexyColor(255, 145, 215),
					new SexyColor(160, 255, 245),
					new SexyColor(230, 30, 30),
					new SexyColor(5, 130, 255),
					new SexyColor(195, 55, 235),
					new SexyColor(235, 210, 255),
					new SexyColor(255, 245, 55),
					new SexyColor(180, 255, 105)
				};
				reanimation.GetTrackInstanceByName("Marigold_petals").mTrackColor = list[(int)(variation - 2)];
			}
			switch (variation)
			{
			case DrawVariation.Imitater:
				reanimation.mFilterEffect = FilterEffectType.WashedOut;
				break;
			case DrawVariation.ImitaterLess:
				reanimation.mFilterEffect = FilterEffectType.LessWashedOut;
				break;
			case DrawVariation.ZenGarden:
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_zengarden);
				break;
			case DrawVariation.ZenGardenWater:
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_waterplants);
				break;
			case DrawVariation.Aquarium:
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle_aquarium);
				break;
			case DrawVariation.Bigidle:
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_bigidle);
				break;
			case DrawVariation.SproutNoFlower:
				reanimation.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle_noflower);
				break;
			}
		}
	}

	public MemoryImage MakeBlankCanvasImage(int width, int height)
	{
		MemoryImage memoryImage = new MemoryImage();
		memoryImage.Create(width, height);
		memoryImage.Clear();
		return memoryImage;
	}

	public MemoryImage MakeCachedPlantFrame(SeedType seedType, DrawVariation drawVariation)
	{
		MemoryImage value = null;
		if (drawVariation == DrawVariation.Normal)
		{
			if (mPlantImages[(int)seedType] != null)
			{
				return mPlantImages[(int)seedType];
			}
		}
		else if (mImageVariationList.TryGetValue(new CachedPlantVariation
		{
			mSeedType = seedType,
			mDrawVariation = drawVariation
		}, out value))
		{
			return value;
		}
		TRect plantImageSize = GetPlantImageSize(seedType);
		value = MakeBlankCanvasImage((int)(Constants.S * (float)plantImageSize.mWidth * 2f), (int)(Constants.S * (float)plantImageSize.mHeight * 2f));
		value.mInvScale = 2f;
		lock (ResourceManager.DrawLocker)
		{
			Graphics graphics = Graphics.GetNew(value);
			graphics.SetLinearBlend(linear: true);
			PlantDefinition plantDefinition = Plant.GetPlantDefinition(seedType);
			ReanimationType mReanimationType = plantDefinition.mReanimationType;
			switch (seedType)
			{
			case SeedType.Potatomine:
			{
				int num = 12;
				graphics.mScaleX = 0.8f;
				graphics.mScaleY = 0.8f;
				DrawReanimatorFrame(graphics, Constants.S * (float)(-(plantImageSize.mX - num)), Constants.S * (float)(-(plantImageSize.mY - num)), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_armed, drawVariation);
				break;
			}
			case SeedType.InstantCoffee:
			{
				int num = 12;
				graphics.mScaleX = 0.8f;
				graphics.mScaleY = 0.8f;
				DrawReanimatorFrame(graphics, Constants.S * (float)(-(plantImageSize.mX - num)), Constants.S * (float)(-(plantImageSize.mY - num)), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				break;
			}
			case SeedType.ExplodeONut:
				graphics.SetColorizeImages(colorizeImages: true);
				graphics.SetColor(new SexyColor(255, 64, 64));
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				break;
			case SeedType.IceWallnut:
				graphics.SetColorizeImages(colorizeImages: true);
				graphics.SetColor(new SexyColor(64, 160, 255));
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				break;
			case SeedType.Peashooter:
			case SeedType.Snowpea:
			case SeedType.Repeater:
			case SeedType.Gatlingpea:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, drawVariation);
				break;
			case SeedType.Leftpeater:
				graphics.mScaleX = -1f;
				DrawReanimatorFrame(graphics, Constants.S * ((float)(-plantImageSize.mX) + 80f), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * ((float)(-plantImageSize.mX) + 80f), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, drawVariation);
				break;
			case SeedType.Splitpea:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_splitpea_idle, drawVariation);
				break;
			case SeedType.Threepeater:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle1, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle3, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle2, drawVariation);
				break;
			case SeedType.Scaredyshroom:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY + 20), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				break;
			case SeedType.Lilypad:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, "anim_idle3", drawVariation);
				break;
			case SeedType.Pumpkinshell:
				if (drawVariation == DrawVariation.ZenGarden)
				{
					DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, DrawVariation.Normal);
					break;
				}
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), ReanimationType.PumpkinLower, "anim_idle1", drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), ReanimationType.PumpkinUpper, "anim_idle1", drawVariation);
				break;
			case SeedType.SunflowerPea:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX) - 18f, Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX) + 18f, Constants.S * (float)(-plantImageSize.mY), ReanimationType.Peashooter, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, drawVariation);
				break;
			case SeedType.PeaWallnut:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX) + 18f, Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX) - 18f, Constants.S * (float)(-plantImageSize.mY), ReanimationType.Peashooter, GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, drawVariation);
				break;
			case SeedType.SunflowerWallnut:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX) - 18f, Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX) + 18f, Constants.S * (float)(-plantImageSize.mY), ReanimationType.Wallnut, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				break;
			default:
				DrawReanimatorFrame(graphics, Constants.S * (float)(-plantImageSize.mX), Constants.S * (float)(-plantImageSize.mY), mReanimationType, GlobalMembersReanimIds.ReanimTrackId_anim_idle, drawVariation);
				break;
			}
			if (drawVariation == DrawVariation.Normal)
			{
				mPlantImages[(int)seedType] = value;
			}
			else
			{
				mImageVariationList[new CachedPlantVariation
				{
					mSeedType = seedType,
					mDrawVariation = drawVariation
				}] = value;
			}
			graphics.EndFrame();
			graphics.SetRenderTarget(null);
			graphics.PrepareForReuse();
			return value;
		}
	}

	public MemoryImage MakeCachedMowerFrame(LawnMowerType mowerType)
	{
		if (mLawnMowers[(int)mowerType] != null)
		{
			return mLawnMowers[(int)mowerType];
		}
		MemoryImage memoryImage = MakeBlankCanvasImage((int)(Constants.S * 90f), (int)(Constants.S * 100f));
		lock (ResourceManager.DrawLocker)
		{
			Graphics graphics = Graphics.GetNew(memoryImage);
			graphics.SetLinearBlend(linear: true);
			graphics.BeginFrame();
			ReanimationType reanimType = ReanimationType.Lawnmower;
			string trackName = null;
			graphics.mScaleX = 0.85f;
			graphics.mScaleY = 0.85f;
			float num = 10f;
			float num2 = 0f;
			switch (mowerType)
			{
			case LawnMowerType.Lawn:
				trackName = GlobalMembersReanimIds.ReanimTrackId_anim_normal;
				break;
			case LawnMowerType.Pool:
				graphics.mScaleX = 0.8f;
				graphics.mScaleY = 0.8f;
				num2 = 25f;
				reanimType = ReanimationType.PoolCleaner;
				break;
			case LawnMowerType.Roof:
				reanimType = ReanimationType.RoofCleaner;
				break;
			case LawnMowerType.SuperMower:
				trackName = GlobalMembersReanimIds.ReanimTrackId_anim_tricked;
				break;
			}
			DrawReanimatorFrame(graphics, Constants.S * num, Constants.S * num2, reanimType, trackName, DrawVariation.Normal);
			mLawnMowers[(int)mowerType] = memoryImage;
			graphics.EndFrame();
			graphics.SetRenderTarget(null);
			graphics.PrepareForReuse();
			return memoryImage;
		}
	}

	public static TPoint GetZombieFrameOrigin(ZombieType type)
	{
		return new TPoint(40, (type == ZombieType.Gargantuar || type == ZombieType.RedeyeGargantuar || type == ZombieType.RedeyeRobotTitan) ? 60 : 40);
	}

	public MemoryImage MakeCachedZombieFrame(ZombieType zombieType)
	{
		if (mZombieImages[(int)zombieType] != null)
		{
			return mZombieImages[(int)zombieType];
		}
		MemoryImage memoryImage = MakeBlankCanvasImage((int)(Constants.S * 200f * 2f + 0.5f), (int)(Constants.S * 210f * 2f + 0.5f));
		mZombieImages[(int)zombieType] = memoryImage;
		lock (ResourceManager.DrawLocker)
		{
			Graphics graphics = Graphics.GetNew(memoryImage);
			graphics.mScaleX = 2f;
			graphics.mScaleY = 2f;
			graphics.SetLinearBlend(linear: true);
			graphics.BeginFrame();
			ZombieType theZombieType = ((zombieType != ZombieType.CachedPolevaulterWithPole) ? zombieType : ZombieType.Polevaulter);
			ZombieDefinition zombieDefinition = Zombie.GetZombieDefinition(theZombieType);
			ReanimationType mReanimationType = zombieDefinition.mReanimationType;
			TPoint zombieFrameOrigin = GetZombieFrameOrigin(zombieType);
			float num = zombieFrameOrigin.x;
			float num2 = zombieFrameOrigin.y;
			switch (mReanimationType)
			{
			case ReanimationType.None:
			{
				Image image = null;
				int num3 = 0;
				int num4 = 0;
				if (image == null)
				{
					graphics.EndFrame();
					graphics.SetRenderTarget(null);
					graphics.PrepareForReuse();
					return memoryImage;
				}
				int celWidth = image.GetCelWidth();
				int celHeight = image.GetCelHeight();
				TRect theSrcRect = new TRect(num3 * celWidth, num4 * celHeight, celWidth, celHeight);
				TRect theDestRect = new TRect((int)(num * Constants.S * 2f), (int)(num2 * Constants.S * 2f), celWidth, celHeight);
				graphics.DrawImage(image, theDestRect, theSrcRect);
				graphics.EndFrame();
				graphics.SetRenderTarget(null);
				graphics.PrepareForReuse();
				memoryImage.mInvScale = 2f;
				memoryImage.mWidth = (int)((float)memoryImage.mWidth / memoryImage.mInvScale + 0.5f);
				memoryImage.mHeight = (int)((float)memoryImage.mHeight / memoryImage.mInvScale + 0.5f);
				CacheZombieImageBounds(zombieType, memoryImage);
				return memoryImage;
			}
			case ReanimationType.Zombie:
			{
				Reanimation newReanimation4 = Reanimation.GetNewReanimation();
				newReanimation4.ReanimationInitializeType(Constants.S * num * 2f, Constants.S * num2 * 2f, mReanimationType);
				newReanimation4.OverrideScale(2f, 2f);
				newReanimation4.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
				newReanimation4.AssignRenderGroupToTrack("zombie_butter", -1);
				Zombie.SetupReanimLayers(newReanimation4, theZombieType);
				if (zombieType == ZombieType.SquashHead || zombieType == ZombieType.WallnutHead || zombieType == ZombieType.TallnutHead || zombieType == ZombieType.PeaHead || zombieType == ZombieType.JalapenoHead || zombieType == ZombieType.GatlingHead)
				{
					Reanimation theReanimationID = null;
					switch (zombieType)
					{
					case ZombieType.PeaHead:
					{
						newReanimation4.AssignRenderGroupToPrefix("anim_hair", -1);
						newReanimation4.AssignRenderGroupToPrefix("anim_head2", -1);
						ReanimatorTrackInstance trackInstanceByName6 = newReanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
						trackInstanceByName6.mImageOverride = AtlasResources.IMAGE_BLANK;
						theReanimationID = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Peashooter);
						theReanimationID.OverrideScale(2f, 2f);
						theReanimationID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 15f);
						AttachEffect attachEffect6 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName6.mAttachmentID, theReanimationID, 0f, 0f);
						newReanimation4.mFrameBasePose = 0;
						TodCommon.TodScaleRotateTransformMatrix(ref attachEffect6.mOffset.mMatrix, 65f * Constants.S, -8f * Constants.S, 0.2f, -1f, 1f);
						break;
					}
					case ZombieType.WallnutHead:
					{
						newReanimation4.AssignRenderGroupToPrefix("anim_hair", -1);
						newReanimation4.AssignRenderGroupToPrefix("anim_head", -1);
						newReanimation4.AssignRenderGroupToPrefix("Zombie_tie", -1);
						ReanimatorTrackInstance trackInstanceByName5 = newReanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_body);
						theReanimationID = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Wallnut);
						theReanimationID.OverrideScale(2f, 2f);
						theReanimationID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
						AttachEffect attachEffect5 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName5.mAttachmentID, theReanimationID, 0f, 0f);
						newReanimation4.mFrameBasePose = 0;
						TodCommon.TodScaleRotateTransformMatrix(ref attachEffect5.mOffset.mMatrix, 50f * Constants.S, 0f, 0.2f, -0.8f, 0.8f);
						break;
					}
					case ZombieType.TallnutHead:
					{
						newReanimation4.AssignRenderGroupToPrefix("anim_hair", -1);
						newReanimation4.AssignRenderGroupToPrefix("anim_head", -1);
						newReanimation4.AssignRenderGroupToPrefix("Zombie_tie", -1);
						ReanimatorTrackInstance trackInstanceByName4 = newReanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_body);
						theReanimationID = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Tallnut);
						theReanimationID.OverrideScale(2f, 2f);
						theReanimationID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
						AttachEffect attachEffect4 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName4.mAttachmentID, theReanimationID, 0f, 0f);
						newReanimation4.mFrameBasePose = 0;
						TodCommon.TodScaleRotateTransformMatrix(ref attachEffect4.mOffset.mMatrix, 37f * Constants.S, 0f, 0.2f, -0.8f, 0.8f);
						break;
					}
					case ZombieType.JalapenoHead:
					{
						newReanimation4.AssignRenderGroupToPrefix("anim_hair", -1);
						newReanimation4.AssignRenderGroupToPrefix("anim_head", -1);
						newReanimation4.AssignRenderGroupToPrefix("Zombie_tie", -1);
						ReanimatorTrackInstance trackInstanceByName3 = newReanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_zombie_body);
						theReanimationID = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Jalapeno);
						theReanimationID.OverrideScale(2f, 2f);
						theReanimationID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
						AttachEffect attachEffect3 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName3.mAttachmentID, theReanimationID, 0f, 0f);
						newReanimation4.mFrameBasePose = 0;
						TodCommon.TodScaleRotateTransformMatrix(ref attachEffect3.mOffset.mMatrix, 55f * Constants.S, -5f * Constants.S, 0.2f, -1f, 1f);
						break;
					}
					case ZombieType.GatlingHead:
					{
						newReanimation4.AssignRenderGroupToPrefix("anim_hair", -1);
						newReanimation4.AssignRenderGroupToPrefix("anim_head2", -1);
						ReanimatorTrackInstance trackInstanceByName2 = newReanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
						trackInstanceByName2.mImageOverride = AtlasResources.IMAGE_BLANK;
						theReanimationID = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Gatlingpea);
						theReanimationID.OverrideScale(2f, 2f);
						theReanimationID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle, ReanimLoopType.Loop, 0, 15f);
						AttachEffect attachEffect2 = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName2.mAttachmentID, theReanimationID, 0f, 0f);
						newReanimation4.mFrameBasePose = 0;
						TodCommon.TodScaleRotateTransformMatrix(ref attachEffect2.mOffset.mMatrix, 65f * Constants.S, -8f * Constants.S, 0.2f, -1f, 1f);
						break;
					}
					case ZombieType.SquashHead:
					{
						newReanimation4.AssignRenderGroupToPrefix("anim_hair", -1);
						newReanimation4.AssignRenderGroupToPrefix("anim_head2", -1);
						ReanimatorTrackInstance trackInstanceByName = newReanimation4.GetTrackInstanceByName(GlobalMembersReanimIds.ReanimTrackId_anim_head1);
						trackInstanceByName.mImageOverride = AtlasResources.IMAGE_BLANK;
						theReanimationID = mApp.AddReanimation(0f, 0f, 0, ReanimationType.Squash);
						theReanimationID.OverrideScale(2f, 2f);
						theReanimationID.PlayReanim(GlobalMembersReanimIds.ReanimTrackId_anim_idle, ReanimLoopType.Loop, 0, 15f);
						AttachEffect attachEffect = GlobalMembersAttachment.AttachReanim(ref trackInstanceByName.mAttachmentID, theReanimationID, 0f, 0f);
						newReanimation4.mFrameBasePose = 0;
						TodCommon.TodScaleRotateTransformMatrix(ref attachEffect.mOffset.mMatrix, 55f * Constants.S, -15f * Constants.S, 0.2f, -0.75f, 0.75f);
						break;
					}
					}
					newReanimation4.Update();
					newReanimation4.Draw(graphics);
					if (theReanimationID != null)
					{
						mApp.RemoveReanimation(ref theReanimationID);
					}
				}
				else
				{
					switch (zombieType)
					{
					case ZombieType.Door:
						newReanimation4.AssignRenderGroupToTrack(GlobalMembersReanimIds.ReanimTrackId_anim_screendoor, 0);
						break;
					case ZombieType.Flag:
					{
						Reanimation newReanimation5 = Reanimation.GetNewReanimation();
						newReanimation5.ReanimationInitializeType(Constants.S * num * 2f, Constants.S * num2 * 2f, ReanimationType.ZombieFlagpole);
						newReanimation5.OverrideScale(2f, 2f);
						newReanimation5.SetFramesForLayer("Zombie_flag");
						newReanimation5.Draw(graphics);
						newReanimation5.PrepareForReuse();
						break;
					}
					}
					newReanimation4.Draw(graphics);
				}
				newReanimation4.PrepareForReuse();
				break;
			}
			default:
				if (zombieType == ZombieType.RedeyeGargantuar || zombieType == ZombieType.RedeyeRobotTitan)
				{
					Reanimation newReanimation = Reanimation.GetNewReanimation();
					newReanimation.ReanimationInitializeType(Constants.S * num * 2f, Constants.S * num2 * 2f, zombieDefinition.mReanimationType);
					newReanimation.OverrideScale(2f, 2f);
					newReanimation.SetFramesForLayer("anim_idle");
					if (zombieType == ZombieType.RedeyeRobotTitan)
					{
						newReanimation.SetImageOverride("anim_head", AtlasResources.IMAGE_REANIM_ZOMBIE_ROBOTGARGANTUAR_HEAD_REDEYE);
					}
					else
					{
						newReanimation.SetImageOverride(GlobalMembersReanimIds.ReanimTrackId_anim_head1, AtlasResources.IMAGE_REANIM_ZOMBIE_GARGANTUAR_HEAD_REDEYE);
					}
					newReanimation.Draw(graphics);
					newReanimation.PrepareForReuse();
				}
				else if (mReanimationType == ReanimationType.Boss)
				{
					Reanimation newReanimation2 = Reanimation.GetNewReanimation();
					newReanimation2.ReanimationInitializeType(Constants.S * -524f * 2f, Constants.S * -88f * 2f, mReanimationType);
					newReanimation2.OverrideScale(2f, 2f);
					newReanimation2.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_head_idle);
					Reanimation newReanimation3 = Reanimation.GetNewReanimation();
					newReanimation3.ReanimationInitializeType(Constants.S * 46f * 2f, Constants.S * 22f * 2f, ReanimationType.BossDriver);
					newReanimation3.OverrideScale(2f, 2f);
					newReanimation3.SetFramesForLayer(GlobalMembersReanimIds.ReanimTrackId_anim_idle);
					newReanimation2.Draw(graphics);
					newReanimation3.Draw(graphics);
					newReanimation2.AssignRenderGroupToTrack("boss_body1", -1);
					newReanimation2.AssignRenderGroupToTrack("boss_neck", -1);
					newReanimation2.AssignRenderGroupToTrack("boss_head2", -1);
					newReanimation2.Draw(graphics);
					newReanimation2.PrepareForReuse();
					newReanimation3.PrepareForReuse();
				}
				else
				{
					string trackName = GlobalMembersReanimIds.ReanimTrackId_anim_idle;
					switch (zombieType)
					{
					case ZombieType.Pogo:
						trackName = GlobalMembersReanimIds.ReanimTrackId_anim_pogo;
						break;
					case ZombieType.CachedPolevaulterWithPole:
						trackName = GlobalMembersReanimIds.ReanimTrackId_anim_idle;
						break;
					case ZombieType.Polevaulter:
						trackName = "anim_walk";
						break;
					}
					DrawReanimatorFrame(graphics, Constants.S * num * 2f, Constants.S * num2 * 2f, zombieDefinition.mReanimationType, trackName, DrawVariation.Normal);
				}
				break;
			}
			graphics.EndFrame();
			graphics.SetRenderTarget(null);
			graphics.PrepareForReuse();
			memoryImage.mInvScale = 2f;
			memoryImage.mWidth = (int)((float)memoryImage.mWidth / memoryImage.mInvScale + 0.5f);
			memoryImage.mHeight = (int)((float)memoryImage.mHeight / memoryImage.mInvScale + 0.5f);
			CacheZombieImageBounds(zombieType, memoryImage);
			return memoryImage;
		}
	}

	public void LoadCachedImages()
	{
	}

	public void SaveCachedImages()
	{
	}
}
