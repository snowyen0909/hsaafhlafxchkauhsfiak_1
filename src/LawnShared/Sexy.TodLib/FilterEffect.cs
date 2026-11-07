using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sexy.TodLib;

public class FilterEffect
{
	public static List<ConditionalWeakTable<Texture2D, Texture2D>> gFilterMap;

	public static Dictionary<FilterEffectType, Effect> mEffectMap = new Dictionary<FilterEffectType, Effect>();

	public static HashSet<Texture2D> gFilterCacheThisFrame = new HashSet<Texture2D>(8192);

	public static HashSet<Texture2D> gFilterCacheLastFrame = new HashSet<Texture2D>(8192);

	public static void FilterEffectInitForApp()
	{
		gFilterMap = new List<ConditionalWeakTable<Texture2D, Texture2D>>();
		for (int i = 0; i < 3; i++)
		{
			gFilterMap.Add(new ConditionalWeakTable<Texture2D, Texture2D>());
		}
		mEffectMap[FilterEffectType.White] = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders/LawnFilterWhite");
		mEffectMap[FilterEffectType.WashedOut] = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders/LawnFilterWashedOut");
		mEffectMap[FilterEffectType.WashedOut].Parameters["Greyscale"].SetValue(0.7f);
	}

	public static void FilterEffectProcessDeleteQueue()
	{
		gFilterCacheLastFrame.Clear();
		HashSet<Texture2D> hashSet = gFilterCacheLastFrame;
		gFilterCacheLastFrame = gFilterCacheThisFrame;
		gFilterCacheThisFrame = hashSet;
	}

	public static void FilterEffectDisposeForApp()
	{
		gFilterMap = null;
		foreach (Effect value in mEffectMap.Values)
		{
			value.Dispose();
		}
	}

	public static Texture2D FilterEffectInitTexture(Texture2D texture, FilterEffectType theFilterEffect)
	{
		Texture2D value = null;
		if (!gFilterMap[(int)theFilterEffect].TryGetValue(texture, out value))
		{
			value = FilterEffectCreateTexture(texture, theFilterEffect);
			gFilterMap[(int)theFilterEffect].Add(texture, value);
		}
		return value;
	}

	public static Image FilterEffectGetImage(Image theImage, FilterEffectType theFilterEffect)
	{
		if (theFilterEffect == FilterEffectType.None)
		{
			return theImage;
		}
		Texture2D texture2D = FilterEffectInitTexture(theImage.Texture, theFilterEffect);
		gFilterCacheThisFrame.Add(texture2D);
		MemoryImage memoryImage = new MemoryImage();
		memoryImage.mS = theImage.mS;
		memoryImage.mT = theImage.mT;
		memoryImage.mWidth = theImage.mWidth;
		memoryImage.mHeight = theImage.mHeight;
		memoryImage.mNumCols = theImage.mNumCols;
		memoryImage.mNumRows = theImage.mNumRows;
		memoryImage.Texture = texture2D;
		memoryImage.mInvScale = theImage.mInvScale;
		return memoryImage;
	}

	private static Texture2D FilterEffectCreateTexture(Texture2D theTexture, FilterEffectType theFilterEffect)
	{
		MemoryImage memoryImage = new MemoryImage();
		memoryImage.Create(theTexture.Width, theTexture.Height);
		lock (ResourceManager.DrawLocker)
		{
			GraphicsDevice graphicsDevice = GlobalStaticVars.g.GraphicsDevice;
			graphicsDevice.SetRenderTarget(memoryImage.RenderTarget);
			SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, mEffectMap.ContainsKey(theFilterEffect) ? mEffectMap[theFilterEffect] : null);
			spriteBatch.Draw(theTexture, theTexture.Bounds, Color.White);
			spriteBatch.End();
			spriteBatch.Dispose();
			graphicsDevice.SetRenderTarget(null);
		}
		switch (theFilterEffect)
		{
		case FilterEffectType.LessWashedOut:
			FilterEffectDoLessWashedOut(memoryImage);
			break;
		}
		return memoryImage.Texture;
	}

	private static void FilterEffectDoWashedOut(MemoryImage theImage)
	{
		FilterEffectDoLumSat(theImage, 1.8f, 0.2f);
	}

	private static void FilterEffectDoLessWashedOut(MemoryImage theImage)
	{
		FilterEffectDoLumSat(theImage, 1.2f, 0.3f);
	}

	private static void FilterEffectDoWhite(MemoryImage theImage)
	{
		int[] array = new int[theImage.mWidth * theImage.mHeight];
		theImage.Texture.GetData(array);
		for (int i = 0; i < theImage.mHeight; i++)
		{
			for (int j = 0; j < theImage.mWidth; j++)
			{
				char c = (char)(array[j + i * theImage.mWidth] >> 24);
				array[j + i * theImage.mWidth] = (int)(c | ((uint)c << 8) | ((uint)c << 16) | ((uint)c << 24));
			}
		}
		theImage.Texture.SetData(array);
	}

	private static void FilterEffectDoLumSat(MemoryImage theImage, float aLum, float aSat)
	{
		int[] array = new int[theImage.Texture.Width * theImage.Texture.Height];
		theImage.Texture.GetData(array);
		for (int i = 0; i < theImage.Texture.Height; i++)
		{
			for (int j = 0; j < theImage.Texture.Width; j++)
			{
				int num = array[j + i * theImage.Texture.Width];
				char c = (char)(num & 0xFF);
				char c2 = (char)((num >> 8) & 0xFF);
				char c3 = (char)((num >> 16) & 0xFF);
				char c4 = (char)(num >> 24);
				float r = (float)(int)c / 255f;
				float g = (float)(int)c2 / 255f;
				float b = (float)(int)c3 / 255f;
				RGB_to_HSL(r, g, b, out var h, out var s, out var l);
				s *= aSat;
				l *= aLum;
				HSL_to_RGB(h, s, l, out r, out g, out b);
				int num2 = TodCommon.ClampInt((int)(r * 255f), 0, 255);
				int num3 = TodCommon.ClampInt((int)(g * 255f), 0, 255);
				int num4 = TodCommon.ClampInt((int)(b * 255f), 0, 255);
				array[j + i * theImage.Texture.Width] = (int)(((uint)c4 << 24) | (uint)(num4 << 16) | (uint)(num3 << 8)) | num2;
			}
		}
		theImage.Texture.SetData(array);
	}

	private static void RGB_to_HSL(float r, float g, float b, out float h, out float s, out float l)
	{
		float val = Math.Max(r, g);
		val = Math.Max(val, b);
		float val2 = Math.Min(r, g);
		val2 = Math.Min(val2, b);
		h = (l = (s = 0f));
		float num;
		if (!((l = (val2 + val) / 2f) <= 0f) && (s = (num = val - val2)) > 0f)
		{
			s /= ((l <= 0.5f) ? (val + val2) : (2f - val - val2));
			float num2 = (val - r) / num;
			float num3 = (val - g) / num;
			float num4 = (val - b) / num;
			if (r == val)
			{
				h = ((g == val2) ? (5f + num4) : (1f - num3));
			}
			else if (g == val)
			{
				h = ((b == val2) ? (1f + num2) : (3f - num4));
			}
			else
			{
				h = ((r == val2) ? (3f + num3) : (5f - num2));
			}
			h /= 6f;
		}
	}

	private static void HSL_to_RGB(float h, float sl, float l, out float r, out float g, out float b)
	{
		r = (g = (b = 0f));
		float num = ((l <= 0.5f) ? (l * (1f + sl)) : (l + sl - l * sl));
		if (num <= 0f)
		{
			r = (g = (b = 0f));
			return;
		}
		float num2 = l + l - num;
		float num3 = (num - num2) / num;
		h *= 6f;
		int num4 = TodCommon.ClampInt((int)h, 0, 5);
		float num5 = h - (float)num4;
		float num6 = num * num3 * num5;
		float num7 = num2 + num6;
		float num8 = num - num6;
		switch (num4)
		{
		case 0:
			r = num;
			g = num7;
			b = num2;
			break;
		case 1:
			r = num8;
			g = num;
			b = num2;
			break;
		case 2:
			r = num2;
			g = num;
			b = num7;
			break;
		case 3:
			r = num2;
			g = num8;
			b = num;
			break;
		case 4:
			r = num7;
			g = num2;
			b = num;
			break;
		case 5:
			r = num;
			g = num2;
			b = num8;
			break;
		}
	}
}
