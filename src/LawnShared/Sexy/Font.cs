using System.Text;
using Microsoft.Xna.Framework;

namespace Sexy;

public interface Font
{
	public class CachedStringInfo
	{
		public string[] Strings { get; private set; }

		public Vector2[] StringDimensions { get; private set; }

		public CachedStringInfo(string[] substrings, Font fontUsed)
		{
			Strings = substrings;
			StringDimensions = new Vector2[Strings.Length];
			for (int i = 0; i < Strings.Length; i++)
			{
				StringDimensions[i] = fontUsed.MeasureString(Strings[i]);
				Vector2[] stringDimensions = StringDimensions;
				int num = i;
				stringDimensions[num].Y = stringDimensions[num].Y * fontUsed.scale.Y;
			}
		}

		public override int GetHashCode()
		{
			return Strings.GetHashCode();
		}
	}

	protected static int nextId;

	int FontId { get; }

	Vector2 scale => new Vector2(mScaleX * FrameworkConstants.Font_Scale, mScaleY * FrameworkConstants.Font_Scale);

	int mHeight
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

	int LayerCount { get; }

	int mAscent { get; set; }

	int mAscentPadding { get; set; }

	int characterOffsetMagic { get; set; }

	protected int height { get; set; }

	float mScaleX { get; set; }

	float mScaleY { get; set; }

	int mLineSpacingOffset { get; }

	string SpaceChar { get; set; }

	bool StringWidthCachingEnabled { get; set; }

	static Font()
	{
	}

	CachedStringInfo GetWordWrappedSubStrings(string theLine, TRect theRect);

	static int FontComparer(Font a, Font b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return 1;
		}
		if (b == null)
		{
			return -1;
		}
		return a.FontId.CompareTo(b.FontId);
	}

	void AddLayer(dynamic font)
	{
		AddLayer(font, Vector2.Zero);
	}

	void AddLayer(dynamic font, Vector2 offset);

	void EnableLayer(int layer, bool enabled);

	void Dispose();

	int GetAscent()
	{
		return (int)((float)mAscent * scale.Y);
	}

	int GetAscentPadding()
	{
		return mAscentPadding;
	}

	int GetDescent()
	{
		int num = GetHeight();
		return num + mAscent;
	}

	int GetHeight()
	{
		return StringHeight("1");
	}

	int GetLineSpacingOffset()
	{
		return mLineSpacingOffset;
	}

	int GetLineSpacing();

	int StringWidth(string theString);

	int StringWidth(StringBuilder theString);

	int StringHeight(string theString);

	Vector2 MeasureString(string theString)
	{
		return new Vector2(StringWidth(theString), StringHeight(theString));
	}

	int CharWidth(char theChar);

	int CharWidthKern(char theChar, char thePrevChar)
	{
		return CharWidth(theChar);
	}

	void DrawStringLayer(Graphics g, int theX, int theY, string theString, Color theColor, int layer);

	void DrawString(Graphics g, int theX, int theY, string theString, Color theColor);

	void DrawString(Graphics g, int theX, int theY, StringBuilder theString, Color theColor);

	internal Font Duplicate()
	{
		return this;
	}
}
