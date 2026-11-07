using System;
using System.Collections.Generic;
using System.Text;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sexy;

public class StashFont : Font
{
	private List<bool> enabledLayers = new List<bool>();

	public List<SpriteFontBase> mFonts;

	private List<Vector2> mOffsets;

	private Dictionary<char, Vector2> mCharOffsets = new Dictionary<char, Vector2>();

	private StringBuilder drawStringBuilder = new StringBuilder();

	private static Dictionary<TRect, Dictionary<string, Font.CachedStringInfo>> allCachedStringWidths = new Dictionary<TRect, Dictionary<string, Font.CachedStringInfo>>(20);

	private static Stack<Dictionary<char, int>> charDictionaries = new Stack<Dictionary<char, int>>(20);

	private static Stack<Dictionary<string, int>> stringDictionaries = new Stack<Dictionary<string, int>>(20);

	private Dictionary<float, Dictionary<string, int>> cachedStringWidths = new Dictionary<float, Dictionary<string, int>>(10);

	private Dictionary<float, Dictionary<char, int>> cachedStringBuilderWidths = new Dictionary<float, Dictionary<char, int>>(5);

	public Tuple<FontSystemEffect, int> mEffects;

	public FontSystem mSystem;

	public int FontId { get; private set; }

	public Vector2 scale => new Vector2(mScaleX * FrameworkConstants.Font_Scale, mScaleY * FrameworkConstants.Font_Scale);

	public int mHeight
	{
		get
		{
			return (int)((float)height * mScaleY * FrameworkConstants.Font_Scale);
		}
		set
		{
			height = value;
		}
	}

	public int LayerCount => mFonts.Count;

	public int mAscent { get; set; }

	public int mAscentPadding { get; set; }

	public int characterOffsetMagic { get; set; }

	public int height { get; set; }

	public float mScaleX { get; set; } = 1f;

	public float mScaleY { get; set; } = 1f;

	public int mLineSpacingOffset { get; set; }

	public string SpaceChar { get; set; } = "k";

	public bool StringWidthCachingEnabled { get; set; } = true;

	public StashFont()
	{
		FontId = Font.nextId++;
		drawStringBuilder.Append("A");
		mAscent = 0;
		mHeight = 0;
		mAscentPadding = 0;
		mLineSpacingOffset = 0;
		mFonts = new List<SpriteFontBase>();
		mOffsets = new List<Vector2>();
		mSystem = new FontSystem();
	}

	public StashFont(FontSystemSettings settings)
	{
		FontId = Font.nextId++;
		drawStringBuilder.Append("A");
		mAscent = 0;
		mHeight = 0;
		mAscentPadding = 0;
		mLineSpacingOffset = 0;
		mFonts = new List<SpriteFontBase>();
		mOffsets = new List<Vector2>();
		mSystem = new FontSystem(settings);
	}

	public Font.CachedStringInfo GetWordWrappedSubStrings(string theLine, TRect theRect)
	{
		if (!allCachedStringWidths.TryGetValue(theRect, out var value))
		{
			value = new Dictionary<string, Font.CachedStringInfo>(10);
			allCachedStringWidths.Add(theRect, value);
		}
		if (!value.TryGetValue(theLine, out var value2))
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			List<StringBuilder> list = new List<StringBuilder>();
			list.Add(new StringBuilder());
			for (int i = 0; i < theLine.Length; i++)
			{
				if (char.IsWhiteSpace(theLine[i]))
				{
					int num5 = StringWidth(theLine.Substring(num2, i - num2));
					if (num5 < theRect.mWidth)
					{
						list[list.Count - 1].Append(theLine, num, i - num);
						num = (num3 = i);
						num4 = num5;
					}
					else
					{
						list.Add(new StringBuilder());
						list[list.Count - 1].Append(theLine, num, i - num);
						num2 = num;
						num4 = StringWidth(theLine.Substring(num, i - num));
						num = (num3 = i);
					}
				}
			}
			if (num4 + StringWidth(theLine.Substring(num3, theLine.Length - num3)) > theRect.mWidth && list[list.Count - 1].Length > 0)
			{
				list.Add(new StringBuilder());
			}
			list[list.Count - 1].Append(theLine, num3, theLine.Length - num3);
			string[] array = new string[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				array[j] = list[j].ToString().Trim();
			}
			value2 = new Font.CachedStringInfo(array, this);
			value.Add(theLine, value2);
		}
		return value2;
	}

	public SpriteFontBase GetFontLayer(int i)
	{
		return mFonts[i];
	}

	public void AddLayer(dynamic font, Vector2 offset)
	{
		if (font is SpriteFontBase font2)
		{
			AddLayer(font2, offset);
			return;
		}
		throw new NotImplementedException();
	}

	public void AddLayer(SpriteFontBase font)
	{
		AddLayer(font, Vector2.Zero);
	}

	public void AddLayer(SpriteFontBase font, Vector2 offset)
	{
		mFonts.Add(font);
		mOffsets.Add(offset);
		enabledLayers.Add(item: true);
	}

	public void EnableLayer(int layer, bool enabled)
	{
		enabledLayers[layer] = enabled;
	}

	public void Dispose()
	{
	}

	public int GetAscent()
	{
		return (int)((float)mAscent * scale.Y);
	}

	public int GetAscentPadding()
	{
		return mAscentPadding;
	}

	public int GetDescent()
	{
		int num = -1 * mHeight;
		return num + mAscent;
	}

	public int GetHeight()
	{
		return -1 * mHeight - GetDescent();
	}

	public int GetLineSpacingOffset()
	{
		return mLineSpacingOffset;
	}

	public int GetLineSpacing()
	{
		int num = 0;
		for (int i = 0; i < mFonts.Count; i++)
		{
			if (mFonts[i].LineHeight > num)
			{
				num = mFonts[i].LineHeight;
			}
		}
		return (int)((float)num * scale.Y);
	}

	private static Dictionary<string, int> GetNewStringDictionary()
	{
		if (stringDictionaries.Count > 0)
		{
			return stringDictionaries.Pop();
		}
		return new Dictionary<string, int>(100);
	}

	private int ComputeStringWidthForCache(string theString)
	{
		float num = 0f;
		for (int i = 0; i < mFonts.Count; i++)
		{
			float num2 = mOffsets[i].X * mScaleX * FrameworkConstants.Font_Scale;
			for (int j = 0; j < theString.Length; j++)
			{
				if (char.IsWhiteSpace(theString[j]))
				{
					num2 += (float)StringWidth(SpaceChar);
					continue;
				}
				drawStringBuilder[0] = theString[j];
				num2 += (float)(StringWidth(drawStringBuilder) - characterOffsetMagic);
			}
			if (num2 > num)
			{
				num = num2;
			}
		}
		return (int)num;
	}

	public int StringWidth(string theString)
	{
		int value2;
		if (StringWidthCachingEnabled)
		{
			Dictionary<string, int> value = null;
			if (StringWidthCachingEnabled && !cachedStringWidths.TryGetValue(mScaleX, out value))
			{
				value = GetNewStringDictionary();
				cachedStringWidths.Add(mScaleX, value);
			}
			if (!value.TryGetValue(theString, out value2))
			{
				value2 = ComputeStringWidthForCache(theString);
				value.Add(theString, value2);
			}
		}
		else
		{
			value2 = ComputeStringWidthForCache(theString);
		}
		return value2;
	}

	public int StringWidth(StringBuilder theString)
	{
		int result = 0;
		if (theString.Length == 0)
		{
			return result;
		}
		return ComputeStringWidthForCache(theString);
	}

	private int ComputeStringWidthForCache(StringBuilder theString)
	{
		int num = int.MaxValue;
		for (int i = 0; i < mFonts.Count; i++)
		{
			int num2 = 0;
			if (theString.Length <= 1)
			{
				if (char.IsWhiteSpace(theString[0]))
				{
					num2 += StringWidth(SpaceChar);
				}
				else if (mCharOffsets.ContainsKey(theString[0]) || mFonts[i] != null)
				{
					num2 += (int)(mFonts[i].MeasureString(theString).X * mScaleX * FrameworkConstants.Font_Scale);
				}
			}
			else
			{
				for (int j = 0; j < theString.Length; j++)
				{
					drawStringBuilder[0] = theString[j];
					num2 += StringWidth(drawStringBuilder);
				}
			}
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num - characterOffsetMagic * theString.Length;
	}

	public int StringHeight(string theString)
	{
		int num = 0;
		for (int i = 0; i < mFonts.Count; i++)
		{
			for (int j = 0; j < theString.Length; j++)
			{
				if (char.IsWhiteSpace(theString[j]))
				{
					drawStringBuilder[0] = SpaceChar[0];
				}
				else
				{
					drawStringBuilder[0] = theString[j];
				}
				int num2 = 0;
				if (mCharOffsets.ContainsKey(drawStringBuilder[0]) || mFonts[i] != null)
				{
					num2 = (int)mFonts[i].MeasureString(drawStringBuilder).Y;
				}
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return (int)((float)num * scale.Y);
	}

	public int CharWidth(char theChar)
	{
		drawStringBuilder[0] = theChar;
		return StringWidth(drawStringBuilder);
	}

	public void DrawStringLayer(Graphics g, int theX, int theY, string theString, Color theColor, int layer)
	{
		if (!enabledLayers[layer])
		{
			return;
		}
		Vector2 vector = new Vector2(theX, theY);
		Vector2 vector2 = vector + mOffsets[layer] * mScaleX * FrameworkConstants.Font_Scale;
		if (!string.IsNullOrEmpty(theString) && mCharOffsets.ContainsKey(theString[0]))
		{
			vector2.X -= mCharOffsets[theString[0]].X * mScaleX * FrameworkConstants.Font_Scale;
		}
		SpriteFontBase spriteFontBase = mFonts[layer];
		if (spriteFontBase != null)
		{
			FontSystemEffect fontSystemEffect = mEffects?.Item1 ?? FontSystemEffect.None;
			int num = mEffects?.Item2 ?? 0;
			Vector2 vector3 = vector2;
			if (fontSystemEffect == FontSystemEffect.Stroked)
			{
				vector3.Y -= num;
				vector3.X -= num;
			}
			g.HardwareClip();
			SpriteBatch spriteBatch = Graphics.spriteBatch;
			Vector2 position = vector3;
			Vector2 zero = Vector2.Zero;
			Vector2? vector4 = scale;
			FontSystemEffect effect = fontSystemEffect;
			spriteFontBase.DrawText(spriteBatch, theString, position, theColor, 0f, zero, vector4, 1f - (float)layer / (float)mFonts.Count, 0f, 0f, TextStyle.None, effect, num);
			g.EndHardwareClip();
		}
	}

	public void DrawString(Graphics g, int theX, int theY, string theString, Color theColor)
	{
		if (!string.IsNullOrEmpty(theString))
		{
			g.EndDrawImageTransformed();
			for (int i = 0; i < mFonts.Count; i++)
			{
				DrawStringLayer(g, theX, theY, theString, theColor, i);
			}
		}
	}

	public void DrawString(Graphics g, int theX, int theY, StringBuilder theString, Color theColor)
	{
		if (theString.Length == 0)
		{
			return;
		}
		g.EndDrawImageTransformed();
		Vector2 vector = new Vector2(theX, theY);
		for (int i = 0; i < mFonts.Count; i++)
		{
			if (!enabledLayers[i])
			{
				continue;
			}
			Vector2 vector2 = vector + mOffsets[i] * mScaleX * FrameworkConstants.Font_Scale;
			SpriteFontBase spriteFontBase = mFonts[i];
			if (spriteFontBase != null)
			{
				FontSystemEffect fontSystemEffect = mEffects?.Item1 ?? FontSystemEffect.None;
				int num = mEffects?.Item2 ?? 0;
				if (fontSystemEffect == FontSystemEffect.Stroked)
				{
					vector2.Y -= num;
					vector2.X -= num;
				}
				SpriteBatch spriteBatch = Graphics.spriteBatch;
				Vector2 position = vector2;
				Vector2 zero = Vector2.Zero;
				Vector2? vector3 = scale;
				FontSystemEffect effect = fontSystemEffect;
				spriteFontBase.DrawText(spriteBatch, theString, position, theColor, 0f, zero, vector3, 1f - (float)i / (float)mFonts.Count, 0f, 0f, TextStyle.None, effect, num);
			}
		}
	}

	internal StashFont Duplicate()
	{
		return this;
	}
}
