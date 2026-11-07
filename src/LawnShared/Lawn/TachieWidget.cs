using System;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class TachieWidget : Widget
{
	public Image[] mImages;

	protected int index;

	protected int prevIndex = -1;

	public float mTransX;

	public float mScale;

	public float mPrevTransX;

	public float mPrevScale;

	public bool mIsDragging;

	public TPoint mMouseDownPos;

	public int mSlideInCounter = 20;

	public bool mIsSlidingOut;

	public int mSlideOutCounter;

	public Image Image => mImages[index];

	public int Page
	{
		get
		{
			return index;
		}
		set
		{
			if (value < 0 || value >= mImages.Length)
			{
				throw new ArgumentException();
			}
			SwitchPage(value);
		}
	}

	public TachieWidget(Image mImage)
	{
		mImages = new Image[1] { mImage };
		index = 0;
	}

	public TachieWidget(params Image[] images)
	{
		if (images.Length < 1)
		{
			throw new ArgumentException();
		}
		mImages = images;
		index = 0;
	}

	public void SwitchPage(int newIndex)
	{
		prevIndex = index;
		index = newIndex;
		mPrevTransX = mTransX;
		mPrevScale = mScale;
		Resize(mX, mY, mWidth, mHeight);
		mSlideInCounter = 20;
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		mScale = (float)theHeight / (float)Image.mHeight;
		mTransX = ((float)theWidth - (float)Image.mWidth * mScale) / 2f;
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		DeferOverlay();
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		mMouseDownPos = new TPoint(x, y);
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		if (mIsDragging)
		{
			if (x < mMouseDownPos.x)
			{
				if (index + 1 < mImages.Length)
				{
					SwitchPage(index + 1);
				}
			}
			else if (index - 1 >= 0)
			{
				SwitchPage(index - 1);
			}
			mIsDragging = false;
		}
		else if (theClickCount >= 1)
		{
			mIsSlidingOut = true;
		}
	}

	public override void MouseDrag(int x, int y)
	{
		base.MouseDrag(x, y);
		if (!new TRect(mMouseDownPos.x - 8, mMouseDownPos.y - 8, 16, 16).Contains(x, y))
		{
			mIsDragging = true;
		}
	}

	public override void DrawOverlay(Graphics g)
	{
		float num = TodCommon.TodAnimateCurveFloat(0, 30, mSlideOutCounter, 1f, 0f, TodCurves.EaseOut);
		g.SetColor(new Color(num, num, num, num));
		g.SetColorizeImages(colorizeImages: true);
		float num2 = TodCommon.TodAnimateCurveFloat(20, 0, mSlideInCounter, 0f, 1f, TodCurves.EaseIn);
		g.SetColor(new Color(0f, 0f, 0f, (prevIndex != -1) ? num : (num * num2)));
		g.FillRect((int)((float)(-GlobalStaticVars.gSexyAppBase.mScreenScales.mTransX) / GlobalStaticVars.gSexyAppBase.mScreenScales.mScaleFactor), 0, GlobalStaticVars.gSexyAppBase.mScreenScales.mVirtualWidth, mHeight);
		if (prevIndex != -1)
		{
			g.mScaleX = mPrevScale;
			g.mScaleY = mPrevScale;
			float num3 = 1f - num2;
			g.SetColor(new Color(num3 * num, num3 * num, num3 * num, num));
			g.DrawImage(mImages[prevIndex], mPrevTransX, 0f);
			g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
		}
		g.mScaleX = mScale;
		g.mScaleY = mScale;
		g.SetColor(new Color(num2 * num, num2 * num, num2 * num, num2 * num));
		g.DrawImage(Image, mTransX, 0f);
		g.SetDrawMode(Graphics.DrawMode.DRAWMODE_NORMAL);
	}

	public override void Update()
	{
		base.Update();
		if (mSlideInCounter > 0)
		{
			mSlideInCounter--;
		}
		else
		{
			prevIndex = -1;
		}
		if (mIsSlidingOut)
		{
			if (mSlideOutCounter >= 30)
			{
				mParent.RemoveWidget(this);
				GlobalStaticVars.gSexyAppBase.SafeDeleteWidget(this);
			}
			else
			{
				mSlideOutCounter++;
			}
		}
	}
}
