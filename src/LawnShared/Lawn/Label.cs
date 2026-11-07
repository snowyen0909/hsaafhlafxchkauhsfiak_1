using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class Label : Widget
{
	public string mText;

	public Font mFont;

	public double mScale;

	public Color mColor;

	public DrawStringJustification mDrawStringJustification;

	public Label(string text, Font font, Color color, double scale = 1.0, DrawStringJustification just = DrawStringJustification.Left)
	{
		mText = text;
		mFont = font;
		mScale = scale;
		mColor = color;
		mDrawStringJustification = just;
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		int num = 0;
		int thePosY = 0;
		if (mDrawStringJustification == DrawStringJustification.Right)
		{
			num += mWidth;
		}
		if (mDrawStringJustification == DrawStringJustification.Center)
		{
			num += mWidth / 2;
		}
		TodCommon.TodDrawString(g, mText, num, thePosY, mFont, mColor, mDrawStringJustification, (float)mScale);
	}
}
