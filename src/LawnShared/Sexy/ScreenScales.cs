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

	/// <summary>iPad / foldables only: true while a menu-type screen (no Board) is shown. Menus fill the screen
	/// height (no black bars; the sides of the 1000-wide layout are cropped, the original 800-wide UI stays
	/// visible), boards keep the full width and stretch the art band slightly.</summary>
	public static bool sMenuLayout;

	public enum DeviceClass
	{
		/// <summary>iPhone and other wide screens (landscape aspect >= 1.6).</summary>
		Phone,
		/// <summary>Foldables / "iPhone Duo" and 11"/13" iPads (about 1.38 - 1.6).</summary>
		Foldable,
		/// <summary>4:3 iPads (below about 1.38).</summary>
		Tablet
	}

	public static DeviceClass Classify(int theScreenWidth, int theScreenHeight)
	{
		float aspect = (float)Math.Max(theScreenWidth, theScreenHeight) / Math.Max(1, Math.Min(theScreenWidth, theScreenHeight));
		if (aspect >= 1.6f)
		{
			return DeviceClass.Phone;
		}
		return aspect >= 1.38f ? DeviceClass.Foldable : DeviceClass.Tablet;
	}

	public DeviceClass Device => Classify(mWidth, mHeight);

	/// <summary>Left edge of the visible screen in virtual (layout) coordinates; negative on wide phones,
	/// positive when the sides are cropped (iPad / foldable menus).</summary>
	public int VisibleLeft => (int)Math.Ceiling(-mTransX / mScaleFactor);

	public int VisibleTop => (int)Math.Ceiling(-mTransY / mScaleFactorY);

	public int VisibleWidth => (int)Math.Floor(mWidth / mScaleFactor);

	public int VisibleHeight => (int)Math.Floor(mHeight / mScaleFactorY);

	/// <summary>Keeps a rectangle of the given size inside the visible screen (with a margin).</summary>
	public TPoint ClampToVisible(int theX, int theY, int theWidth, int theHeight, int theMargin = 4)
	{
		int minX = VisibleLeft + theMargin;
		int maxX = VisibleLeft + VisibleWidth - theWidth - theMargin;
		int minY = VisibleTop + theMargin;
		int maxY = VisibleTop + VisibleHeight - theHeight - theMargin;
		return new TPoint(maxX < minX ? minX : Math.Clamp(theX, minX, maxX), maxY < minY ? minY : Math.Clamp(theY, minY, maxY));
	}

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
				// Menu art is 1400x600: fill the height (no black bars), crop the sides. Mod-added widgets
				// (cheat button, wardrobe) position themselves with VisibleLeft / ClampToVisible.
				sx = sy = (float)screenHeight / 600f;
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
