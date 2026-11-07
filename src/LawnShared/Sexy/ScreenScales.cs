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

	public float TranslationX => mTransX;

	public float TranslationY => mTransY;

	public float ScaleRatioMin => mScaleFactor;

	public Matrix ScaleMatrix => Matrix.CreateScale(ScaleRatioMin, ScaleRatioMin, 1f);

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
#if __IOS__
		// Tall screens (iPad): the scene art is 720 virtual px tall, so zoom in until it fills the
		// screen height, cropping at most 20 virtual px on each side of the 1000 px layout.
		if (virtualHeight == 600 && screenHeight * virtualWidth > screenWidth * virtualHeight)
		{
			float fill = Math.Min((float)screenHeight / 720f, (float)screenWidth / 960f);
			if (fill > mScaleFactor)
			{
				mScaleFactor = fill;
				mTransX = (int)Math.Round((screenWidth - virtualWidth * fill) / 2.0);
				mTransY = (int)Math.Round((screenHeight - virtualHeight * fill) / 2.0);
			}
		}
#endif
		mWidth = screenWidth;
		mHeight = screenHeight;
		mVirtualWidth = (int)((float)screenWidth / mScaleFactor);
		mVirtualHeight = (int)((float)screenHeight / mScaleFactor);
	}

	public void Init(int screenWidth, int screenHeight, int virtualWidth, int virtualHeight, int transX, int transY)
	{
		mScaleFactor = MathHelper.Min((float)screenHeight / (float)virtualHeight, (float)screenWidth / (float)virtualWidth);
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
		return (y - (float)mTransY) / ScaleRatioMin;
	}

	public void Scale(ref TRect inRect)
	{
		inRect.mX = (int)Math.Round(ScaleRatioMin * (float)inRect.mX + TranslationX);
		inRect.mY = (int)Math.Round(ScaleRatioMin * (float)inRect.mY + TranslationY);
		inRect.mWidth = (int)Math.Round(ScaleRatioMin * (float)inRect.mWidth);
		inRect.mHeight = (int)Math.Round(ScaleRatioMin * (float)inRect.mHeight);
	}

	public TRect Scale(TRect inRect)
	{
		Scale(ref inRect);
		return inRect;
	}

	public void InvScale(ref TRect inRect)
	{
		inRect.mX = (int)Math.Round(((float)inRect.mX - TranslationX) / ScaleRatioMin);
		inRect.mY = (int)Math.Round(((float)inRect.mY - TranslationY) / ScaleRatioMin);
		inRect.mWidth = (int)Math.Round((float)inRect.mWidth / ScaleRatioMin);
		inRect.mHeight = (int)Math.Round((float)inRect.mHeight / ScaleRatioMin);
	}

	public TRect InvScale(TRect inRect)
	{
		InvScale(ref inRect);
		return inRect;
	}
}
