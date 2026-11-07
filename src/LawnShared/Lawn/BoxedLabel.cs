using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class BoxedLabel : Label
{
	public Image mImageBox;

	public Color mOriginalColor;

	public TRect mMargins;

	public BoxedLabel(Image boxImage, TRect margins, string text, Font font, Color color, double scale = 1.0, DrawStringJustification just = DrawStringJustification.Left)
		: base(text, font, color, scale, just)
	{
		mImageBox = boxImage;
		mMargins = margins;
		mOriginalColor = color;
	}

	public override void Draw(Graphics g)
	{
		Vector2 vector = mFont.MeasureString(mText);
		int num = 0;
		int num2 = 0;
		if (mDrawStringJustification == DrawStringJustification.Right)
		{
			num = mWidth - (int)vector.X;
		}
		if (mDrawStringJustification == DrawStringJustification.Center)
		{
			num = mWidth / 2 - (int)vector.X / 2;
		}
		g.PushState();
		g.ClearClipRect();
		g.SetColorizeImages(colorizeImages: true);
		g.SetColor(new Color(mColor.A, mColor.A, mColor.A, mColor.A), premultiply: false);
		LawnCommon.DrawImageBox(g, new TRect(num - mMargins.mX, num2 - mMargins.mY, (int)vector.X + mMargins.mX + mMargins.mWidth, (int)vector.Y + mMargins.mY + mMargins.mHeight), mImageBox);
		g.PopState();
		base.Draw(g);
	}

	public void SetTransparency(float c)
	{
		mColor = new Color((float)(int)mOriginalColor.R / 255f * c, (float)(int)mOriginalColor.G / 255f * c, (float)(int)mOriginalColor.B / 255f * c, (float)(int)mOriginalColor.A / 255f * c);
	}
}
