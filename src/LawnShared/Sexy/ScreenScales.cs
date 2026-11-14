using System;
using Microsoft.Xna.Framework;

namespace Sexy;

public class ScreenScales
{
	public int mTransX;

	public int mTransY;

	public int mWidth;

	public int mHeight;

	public int mVirtualWidth;

	public int mVirtualHeight;

	public float mScaleFactor = 1f;

	/// <summary>Vertical scale. Equals mScaleFactor except on iPad boards, where the art band is stretched slightly to fill the screen.</summary>
	public float mScaleFactorY = 1f;

	/// <summary>iPad / foldables only: true while a menu-type screen (no Board) is shown. Menus fit the full width
	/// (letterboxed top/bottom); boards keep the full width and stretch the art band slightly.</summary>
	public static bool sMenuLayout;

	public float TranslationX => mTransX;

	public float TranslationY => mTransY;

	public float ScaleRatioMin => mScaleFactor;

	public Matrix ScaleMatrix => Matrix.CreateScale(mScaleFactor, mScaleFactorY, 1f);

	public Matrix IdentityMatrix => Matrix.Identity;

	public Matrix TranslationMatrix => Matrix.CreateTranslation(TranslationX, TranslationY, 0f);

	public void Init(int screenWidth, int screenHeight, int virtualWidth, int virtualHeight)
	{
		if (screenHeight * virtualWidth >= screenWidth * virtualHeight)
		{
			double num = screenWidth;
			double num2 = virtualHeight * screenWidth / virtualWidth;
			mTransX = 0;
			mTransY = (int)(((double)screenHeight - num2) / 2.0);
			mScaleFactor = (float)num / (float)virtualWidth;
		}
		else
		{
			double num = virtualWidth * screenHeight / virtualHeight;
			double num2 = screenHeight;
			mTransX = (int)(((double)screenWidth - num) / 2.0);
			mTransY = 0;
			mScaleFactor = (float)num2 / (float)virtualHeight;
		}
mScaleFactorY = mScaleFactor;
#if __IOS__
		if (virtualHeight == 600 && screenHeight * virtualWidth > screenWidth * virtualHeight)
		{
			float sx, sy;
			if (sMenuLayout)
			{
				// Menus keep the whole 1000-wide layout visible (black bars above/below on 4:3 / foldables).
				// Filling the height used to crop ~100px off each side on iPad, hiding the cheat-menu button,
				// the edges of the wardrobe dialog and other edge buttons.
				sx = sy = Math.Min((float)screenHeight / 600f, (float)screenWidth / (float)virtualWidth);
			}
			else
			{
				// Board art is 720 tall and the HUD needs the full 1000 width:
				// fit the width and stretch vertically (<= ~5%) so the art fills the screen.
				sx = (float)screenWidth / (float)virtualWidth;
				sy = Math.Max(sx, (float)screenHeight / 720f);
			}
			mScaleFactor = sx;
			mScaleFactorY = sy;
			mTransX = (int)Math.Round((screenWidth - virtualWidth * sx) / 2.0);
			mTransY = (int)Math.Round((screenHeight - virtualHeight * sy) / 2.0);
		}
#endif
		mWidth = screenWidth;
		mHeight = screenHeight;
		mVirtualWidth = (int)((float)screenWidth / mScaleFactor);
		mVirtualHeight = (int)((float)screenHeight / mScaleFactorY);
	}

	public void Init(int screenWidth, int screenHeight, int virtualWidth, int virtualHeight, int transX, int transY)
	{
		mScaleFactor = MathHelper.Min((float)screenHeight / (float)virtualHeight, (float)screenWidth / (float)virtualWidth);
		mScaleFactorY = mScaleFactor;
		mTransX = transX;
		mTransY = transY;
		mWidth = screenWidth;
		mHeight = screenHeight;
		mVirtualWidth = (int)((float)screenWidth / mScaleFactor);
		mVirtualHeight = (int)((float)screenHeight / mScaleFactor);
	}

	public CGPoint InvMapTouch(CGPoint cGPoint)
	{
		return new CGPoint(InvMapTouchX(cGPoint.X), InvMapTouchY(cGPoint.Y));
	}

	public float InvMapTouchX(float x)
	{
		return (x - (float)mTransX) / ScaleRatioMin;
	}

	public float InvMapTouchY(float y)
	{
		return (y - (float)mTransY) / mScaleFactorY;
	}

	public void Scale(ref TRect inRect)
	{
		inRect.mX = (int)Math.Round(ScaleRatioMin * (float)inRect.mX + TranslationX);
		inRect.mY = (int)Math.Round(mScaleFactorY * (float)inRect.mY + TranslationY);
		inRect.mWidth = (int)Math.Round(ScaleRatioMin * (float)inRect.mWidth);
		inRect.mHeight = (int)Math.Round(mScaleFactorY * (float)inRect.mHeight);
	}

	public TRect Scale(TRect inRect)
	{
		Scale(ref inRect);
		return inRect;
	}

	public void InvScale(ref TRect inRect)
	{
		inRect.mX = (int)Math.Round(((float)inRect.mX - TranslationX) / ScaleRatioMin);
		inRect.mY = (int)Math.Round(((float)inRect.mY - TranslationY) / mScaleFactorY);
		inRect.mWidth = (int)Math.Round((float)inRect.mWidth / ScaleRatioMin);
		inRect.mHeight = (int)Math.Round((float)inRect.mHeight / mScaleFactorY);
	}

	public TRect InvScale(TRect inRect)
	{
		InvScale(ref inRect);
		return inRect;
	}
}
