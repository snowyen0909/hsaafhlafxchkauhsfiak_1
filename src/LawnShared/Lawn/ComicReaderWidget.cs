using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class ComicReaderWidget : Widget
{
	public record struct ImageTransform(float mTransX = 0f, float mTransY = 0f, float mScale = 1f)
	{
		public void UpdateTransform(int theX, int theY, int theWidth, int theHeight, int imageWidth, int imageHeight)
		{
			mScale = (float)theHeight / (float)imageHeight;
			mTransX = ((float)theWidth - (float)imageWidth * mScale) / 2f;
		}
	}

	public class ComicProgressBar : Widget
	{
		public bool mIsDragging;

		public int mProgressBarHeight = 32;

		public int mPageCount;

		private float mProgressBarPosition;

		public static TPoint SELECTOR_SIZE = new TPoint(40, 56);

		public float ProgressInPixel
		{
			get
			{
				return (float)ProgressWidth * mProgressBarPosition;
			}
			set
			{
				mProgressBarPosition = Math.Clamp(value / (float)ProgressWidth, 0f, 1f);
			}
		}

		private int ProgressWidth => mWidth - SELECTOR_SIZE.mX;

		public event Action<int> PageChanged;

		public override void Draw(Graphics g)
		{
			g.SetColor(new SexyColor(255, 255, 255, 208));
			g.SetColorizeImages(colorizeImages: true);
			LawnCommon.DrawImageBox(g, new TRect(0, (SELECTOR_SIZE.mY - mProgressBarHeight) / 2, mWidth, mProgressBarHeight), AtlasResources.IMAGE_ALMANAC_BROWN_RECT, theDrawCenter: false);
			g.SetColorizeImages(colorizeImages: false);
			LawnCommon.DrawImageBox(g, new TRect((int)ProgressInPixel, 0, SELECTOR_SIZE.mX, SELECTOR_SIZE.mY), AtlasResources.IMAGE_ALMANAC_CLAY_TABLET);
		}

		public override void MouseDown(int x, int y, int theMagicCode)
		{
			ProgressInPixel = x - SELECTOR_SIZE.mX / 2;
		}

		public override void MouseDrag(int x, int y)
		{
			mIsDragging = true;
			ProgressInPixel = x - SELECTOR_SIZE.mX / 2;
		}

		public override void MouseUp(int x, int y, int theMagicCode)
		{
			base.MouseUp(x, y, theMagicCode);
			SyncSelector();
			PageChanged(GetPageNumber());
		}

		public void SyncSelector()
		{
			SetPageNumber(GetPageNumber());
		}

		public void SetPageNumber(int num)
		{
			int pageNumber = GetPageNumber();
			mProgressBarPosition = (float)num / (float)(mPageCount - 1);
			if (pageNumber != num)
			{
				PageChanged(num);
			}
		}

		public int GetPageNumber()
		{
			return Math.Clamp((int)Math.Round(mProgressBarPosition * (float)(mPageCount - 1)), 0, mPageCount - 1);
		}
	}

	public const int SCROLL_PAGE_EASING_TICKS = 40;

	public const int PAGE_APPEAR_EASING_TICKS = 30;

	public string[] mImageLoadGroups;

	public string[] mImageNames;

	public Image[] mThumbnails;

	public int[] mPageEasings;

	public string mOwnedThumbnailLoadGroup;

	protected int index = -1;

	protected int accumulatedX;

	protected int currentDeviatedX;

	private int mStartDraggingCountdown;

	private int mDraggingTime;

	private int mReleaseCountdown;

	protected int releaseX;

	public bool mAlwaysShowPageNumber = true;

	public ImageTransform mImageTransform;

	public ImageTransform mPrevImageTransform;

	public ImageTransform mNextImageTransform;

	private int mToggleToolbarCountdown;

	private float _toolbarStickOutPercent;

	public Dictionary<string, Image> mLastLoadedImageGroups = new Dictionary<string, Image>();

	public bool mIsDragging;

	public TPoint mMouseDownPos;

	public bool mShowToolbar;

	public ComicProgressBar mProgressBar;

	public GameButton mCloseButton;

	public Widget mLastFocusedWidget;

	public TRect mPageNumberRect;

	protected int deviatedX
	{
		get
		{
			return accumulatedX + currentDeviatedX;
		}
		set
		{
			accumulatedX = value - currentDeviatedX;
		}
	}

	private float ToolbarStickOutPercent
	{
		get
		{
			return _toolbarStickOutPercent;
		}
		set
		{
			bool flag = value != _toolbarStickOutPercent;
			_toolbarStickOutPercent = value;
			if (flag)
			{
				ResizeProgressBar();
				mCloseButton.Resize(mWidth - Constants.Almanac_CloseButtonRect.mWidth - 30, (int)((float)(30 + Constants.Almanac_CloseButtonRect.mHeight) * _toolbarStickOutPercent) - Constants.Almanac_CloseButtonRect.mHeight, Constants.Almanac_CloseButtonRect.mWidth, Constants.Almanac_CloseButtonRect.mHeight);
				if (!mAlwaysShowPageNumber)
				{
					mPageNumberRect.mY = (int)((float)(26 + mPageNumberRect.mHeight) * _toolbarStickOutPercent) - mPageNumberRect.mHeight;
				}
			}
		}
	}

	public Image mImage => TryLoadPage(mImageLoadGroups[index], mImageNames[index]);

	public Image mNextImage
	{
		get
		{
			if (index + 1 >= mImageLoadGroups.Length)
			{
				return null;
			}
			return TryLoadPage(mImageLoadGroups[index + 1], mImageNames[index + 1]);
		}
	}

	public Image mPrevImage
	{
		get
		{
			if (index - 1 < 0)
			{
				return null;
			}
			return TryLoadPage(mImageLoadGroups[index - 1], mImageNames[index - 1]);
		}
	}

	public ComicReaderWidget(string[] imagesGroups, string[] imageNames, Image[] thumbnails, int pageCount)
	{
		if (imagesGroups.Length != pageCount || imageNames.Length != pageCount || thumbnails.Length != pageCount)
		{
			throw new ArgumentException("imagesGroups, imageNames and thumbnails must have the same length as pageCount");
		}
		mProgressBar = new ComicProgressBar();
		mProgressBar.mPageCount = pageCount;
		mProgressBar.PageChanged += SwitchPage;
		mImageLoadGroups = imagesGroups;
		mImageNames = imageNames;
		mThumbnails = thumbnails;
		mPageEasings = new int[pageCount];
		SwitchPage(0, preserveEasings: false);
		mCloseButton = new GameButton(1001, this);
		mCloseButton.mButtonImage = AtlasResources.IMAGE_ALMANAC_CLOSEBUTTON;
		mCloseButton.mOverImage = null;
		mCloseButton.mDownImage = null;
		mCloseButton.SetFont(Resources.FONT_BRIANNETOD12);
		mCloseButton.mColors[0] = new SexyColor(42, 42, 90);
		mCloseButton.mColors[1] = new SexyColor(42, 42, 90);
		mCloseButton.Resize(mWidth - Constants.Almanac_CloseButtonRect.mWidth - 30, 30, Constants.Almanac_CloseButtonRect.mWidth, Constants.Almanac_CloseButtonRect.mHeight);
		mCloseButton.mParentWidget = this;
		mCloseButton.mTextOffsetX = -8;
		mCloseButton.mTextOffsetY = 1;
	}

	public override void Draw(Graphics g)
	{
		g.SetColor(SexyColor.Black);
		g.FillRect(0, 0, mWidth, mHeight);
		DrawComic(g);
		if (mShowToolbar || mToggleToolbarCountdown > 0)
		{
			DrawToolbar(g);
		}
		if (mAlwaysShowPageNumber)
		{
			DrawPageNumber(g, doDrawBox: false);
		}
	}

	public void DrawComic(Graphics g)
	{
		Graphics graphics = Graphics.GetNew(g);
		graphics.mScaleX = mImageTransform.mScale;
		graphics.mScaleY = mImageTransform.mScale;
		graphics.SetColorizeImages(colorizeImages: true);
		float num = TodCommon.TodAnimateCurveFloatTime(0f, 30f, mPageEasings[index], 0f, 1f, TodCurves.EaseOut);
		graphics.SetColor(new Color(num, num, num, num));
		graphics.DrawImage(mImage, mImageTransform.mTransX + (float)deviatedX, mImageTransform.mTransY);
		graphics.PrepareForReuse();
		if (mPrevImage != null)
		{
			Graphics graphics2 = Graphics.GetNew(g);
			graphics2.mScaleX = mPrevImageTransform.mScale;
			graphics2.mScaleY = mPrevImageTransform.mScale;
			graphics2.SetColorizeImages(colorizeImages: true);
			float num2 = TodCommon.TodAnimateCurveFloatTime(0f, 30f, mPageEasings[index - 1], 0f, 1f, TodCurves.EaseOut);
			graphics2.SetColor(new Color(num2, num2, num2, num2));
			graphics2.DrawImage(mPrevImage, mPrevImageTransform.mTransX + (float)deviatedX - (float)mPrevImage.mWidth * mPrevImageTransform.mScale, mPrevImageTransform.mTransY);
			graphics2.PrepareForReuse();
		}
		if (mNextImage != null)
		{
			Graphics graphics3 = Graphics.GetNew(g);
			graphics3.mScaleX = mNextImageTransform.mScale;
			graphics3.mScaleY = mNextImageTransform.mScale;
			graphics3.SetColorizeImages(colorizeImages: true);
			float num3 = TodCommon.TodAnimateCurveFloatTime(0f, 30f, mPageEasings[index + 1], 0f, 1f, TodCurves.EaseOut);
			graphics3.SetColor(new Color(num3, num3, num3, num3));
			graphics3.DrawImage(mNextImage, mNextImageTransform.mTransX + (float)deviatedX + (float)mImage.mWidth * mImageTransform.mScale, mNextImageTransform.mTransY);
			graphics3.PrepareForReuse();
		}
	}

	public void DrawToolbar(Graphics g)
	{
		g.SetColor(new SexyColor(255, 255, 255, 240));
		g.SetColorizeImages(colorizeImages: true);
		g.SetColorizeImages(colorizeImages: false);
		if (mProgressBar.mIsDown)
		{
			Graphics graphics = Graphics.GetNew(g);
			int pageNumber = mProgressBar.GetPageNumber();
			int num = (int)(mProgressBar.ProgressInPixel + (float)ComicProgressBar.SELECTOR_SIZE.mX / 2f + (float)mProgressBar.mX);
			TPoint tPoint = new TPoint(mThumbnails[pageNumber].mWidth, mThumbnails[pageNumber].mHeight);
			float num2 = 160f / (float)tPoint.mY;
			tPoint.mY = 160;
			tPoint.mX = (int)Math.Round((float)tPoint.mX * num2);
			graphics.mScaleX *= num2;
			graphics.mScaleY *= num2;
			graphics.DrawImage(mThumbnails[pageNumber], Math.Clamp(num - tPoint.mX / 2, 0, mWidth - tPoint.mX), mHeight - tPoint.mY - 100);
			graphics.PrepareForReuse();
		}
		if (!mAlwaysShowPageNumber)
		{
			DrawPageNumber(g, doDrawBox: true);
		}
		mCloseButton.Draw(g);
	}

	public void DrawPageNumber(Graphics g, bool doDrawBox)
	{
		if (doDrawBox)
		{
			LawnCommon.DrawImageBox(g, mPageNumberRect, AtlasResources.IMAGE_ALMANAC_BROWN_RECT);
		}
		string theText = $"{mProgressBar.GetPageNumber() + 1}/{mProgressBar.mPageCount}";
		TodCommon.TodDrawString(g, theText, mWidth / 2 - 1, mPageNumberRect.mY + 6, Resources.FONT_BRIANNETOD16, SexyColor.Black, DrawStringJustification.Center);
		TodCommon.TodDrawString(g, theText, mWidth / 2 + 1, mPageNumberRect.mY + 4, Resources.FONT_BRIANNETOD16, SexyColor.White, DrawStringJustification.Center);
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		mImageTransform.UpdateTransform(theX, theY, theWidth, theHeight, mImage.mWidth, mImage.mHeight);
		if (mPrevImage != null)
		{
			mPrevImageTransform.UpdateTransform(theX, theY, theWidth, theHeight, mPrevImage.mWidth, mPrevImage.mHeight);
		}
		if (mNextImage != null)
		{
			mNextImageTransform.UpdateTransform(theX, theY, theWidth, theHeight, mNextImage.mWidth, mNextImage.mHeight);
		}
		mPageNumberRect = new TRect(mWidth / 2 - 50, 26, 100, 40);
		if (mProgressBar != null)
		{
			ResizeProgressBar();
		}
		mCloseButton.Resize(mWidth - Constants.Almanac_CloseButtonRect.mWidth - 30, 30, Constants.Almanac_CloseButtonRect.mWidth, Constants.Almanac_CloseButtonRect.mHeight);
	}

	private void ResizeProgressBar()
	{
		int num = 50;
		int num2 = 32;
		TRect theRect = new TRect(num - num2 / 2, mHeight - (int)((float)(num + ComicProgressBar.SELECTOR_SIZE.mY / 2) * ToolbarStickOutPercent), mWidth - 2 * (num - num2 / 2), ComicProgressBar.SELECTOR_SIZE.mY);
		mProgressBar.Resize(theRect);
		mProgressBar.mProgressBarHeight = num2;
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		if (mPageNumberRect.Contains(x, y))
		{
			mAlwaysShowPageNumber = !mAlwaysShowPageNumber;
		}
		mMouseDownPos = new TPoint(x, y);
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		if (!mPageNumberRect.Contains(x, y))
		{
			if (mCloseButton.IsMouseOver() && mShowToolbar)
			{
				mParent.RemoveWidget(this);
				GlobalStaticVars.gSexyAppBase.mWidgetManager.SetFocus(mLastFocusedWidget);
				GlobalStaticVars.gSexyAppBase.SafeDeleteWidget(this);
			}
			else if (mIsDragging)
			{
				mIsDragging = false;
				mDraggingTime = mStartDraggingCountdown;
			}
			else if (theClickCount >= 1)
			{
				ToggleToolbar();
			}
		}
	}

	public override void MouseDrag(int x, int y)
	{
		base.MouseDrag(x, y);
		if (!new TRect(mMouseDownPos.x - 8, mMouseDownPos.y - 8, 16, 16).Contains(x, y))
		{
			if (!mIsDragging && deviatedX == 0)
			{
				mStartDraggingCountdown = 0;
			}
			mIsDragging = true;
			currentDeviatedX = x - mMouseDownPos.x;
		}
	}

	public override void MouseWheel(int theDelta)
	{
		base.MouseWheel(theDelta);
		if (theDelta < -60)
		{
			SwitchPage(index + 1);
		}
		else if (theDelta > 60)
		{
			SwitchPage(index - 1);
		}
	}

	public void ToggleToolbar()
	{
		mShowToolbar = !mShowToolbar;
		if (mShowToolbar)
		{
			ResizeProgressBar();
			AddWidget(mProgressBar);
			mToggleToolbarCountdown = 0;
		}
	}

	public override void Update()
	{
		base.Update();
		mCloseButton.Update();
		if (!mIsDragging && currentDeviatedX != 0)
		{
			accumulatedX += currentDeviatedX;
			currentDeviatedX = 0;
			releaseX = accumulatedX;
			mReleaseCountdown = 0;
		}
		if (mReleaseCountdown < 40)
		{
			mReleaseCountdown++;
		}
		mStartDraggingCountdown++;
		if (deviatedX != 0)
		{
			if (deviatedX > 0)
			{
				bool flag = !mIsDragging;
				if (index - 1 >= 0)
				{
					float num = (float)mPrevImage.mWidth * mPrevImageTransform.mScale;
					float num2 = (float)mImage.mWidth * mImageTransform.mScale;
					int num3 = (int)((num + num2) / 2f - mPrevImageTransform.mTransX + mImageTransform.mTransX);
					if (deviatedX >= num3)
					{
						SwitchPage(index - 1, preserveEasings: true);
						deviatedX -= num3;
						releaseX -= num3;
					}
					if (flag && IsValidScrollX(deviatedX, mDraggingTime, num3))
					{
						int val = TodCommon.TodAnimateCurve(0, 40, mReleaseCountdown, releaseX, num3, TodCurves.EaseInOut);
						deviatedX = Math.Min(val, num3 + 1);
						flag = false;
					}
				}
				if (flag)
				{
					int val = TodCommon.TodAnimateCurve(0, 40, mReleaseCountdown, releaseX, 0, TodCurves.EaseInOut);
					deviatedX = Math.Max(val, 0);
				}
			}
			if (deviatedX < 0)
			{
				bool flag2 = !mIsDragging;
				if (index + 1 < mProgressBar.mPageCount)
				{
					float num4 = (float)mImage.mWidth * mImageTransform.mScale;
					float num5 = (float)mNextImage.mWidth * mNextImageTransform.mScale;
					int num6 = (int)((0f - num4 - num5) / 2f - mNextImageTransform.mTransX + mImageTransform.mTransX);
					if (deviatedX <= num6)
					{
						SwitchPage(index + 1, preserveEasings: true);
						deviatedX += -num6;
						releaseX += -num6;
					}
					if (flag2 && IsValidScrollX(-deviatedX, mDraggingTime, -num6))
					{
						int val = TodCommon.TodAnimateCurve(0, 40, mReleaseCountdown, releaseX, num6, TodCurves.EaseInOut);
						deviatedX = Math.Max(val, num6 - 1);
						flag2 = false;
					}
				}
				if (flag2)
				{
					int val = TodCommon.TodAnimateCurve(0, 40, mReleaseCountdown, releaseX, 0, TodCurves.EaseInOut);
					deviatedX = Math.Min(val, 0);
				}
			}
		}
		UpdateEasings();
		if (mShowToolbar)
		{
			if (mToggleToolbarCountdown < 20)
			{
				mToggleToolbarCountdown++;
				ToolbarStickOutPercent = TodCommon.TodAnimateCurveFloat(0, 20, mToggleToolbarCountdown, 0f, 1f, TodCurves.EaseOut);
			}
		}
		else if (mToggleToolbarCountdown > 0)
		{
			mToggleToolbarCountdown--;
			ToolbarStickOutPercent = TodCommon.TodAnimateCurveFloat(20, 0, mToggleToolbarCountdown, 1f, 0f, TodCurves.EaseIn);
		}
		else
		{
			RemoveWidget(mProgressBar);
		}
		static bool IsValidScrollX(int distanceX, int draggingTime, int frameWidth)
		{
			if (distanceX >= frameWidth / 15)
			{
				if (!((double)distanceX * 30.0 / (double)draggingTime > (double)(frameWidth / 4)))
				{
					return distanceX > frameWidth / 3;
				}
				return true;
			}
			return false;
		}
	}

	public void UpdateEasings()
	{
		for (int i = 0; i < mPageEasings.Length; i++)
		{
			ref int reference = ref mPageEasings[i];
			if (i == index || (i == index + 1 && deviatedX < 0) || (i == index - 1 && deviatedX > 0))
			{
				if (reference < 30)
				{
					reference++;
				}
			}
			else if (reference > 0)
			{
				reference--;
			}
		}
	}

	public override void Dispose()
	{
		mCloseButton.Dispose();
		foreach (KeyValuePair<string, Image> mLastLoadedImageGroup in mLastLoadedImageGroups)
		{
			UnloadPage(mLastLoadedImageGroup.Key);
		}
		base.Dispose();
		if (!string.IsNullOrEmpty(mOwnedThumbnailLoadGroup))
		{
			GlobalStaticVars.gLawnApp.mResourceManager.UnloadBackground(mOwnedThumbnailLoadGroup);
		}
	}

	public void SwitchPage(int newPageNumber)
	{
		SwitchPage(newPageNumber, preserveEasings: false);
	}

	public void SwitchPage(int newPageNumber, bool preserveEasings)
	{
		if (newPageNumber >= mProgressBar.mPageCount || newPageNumber < 0 || newPageNumber == index)
		{
			return;
		}
		if (newPageNumber > index + 1 || newPageNumber < index - 1)
		{
			foreach (KeyValuePair<string, Image> mLastLoadedImageGroup in mLastLoadedImageGroups)
			{
				UnloadPage(mLastLoadedImageGroup.Key);
			}
		}
		else
		{
			int num = index + index - newPageNumber;
			if (num >= 0 && num < mImageLoadGroups.Length)
			{
				UnloadPage(mImageLoadGroups[num]);
			}
		}
		index = newPageNumber;
		if (mPrevImage != null || mImage != null || mNextImage != null)
		{
			mProgressBar.SetPageNumber(newPageNumber);
		}
		if (!preserveEasings)
		{
			for (int i = 0; i < mPageEasings.Length; i++)
			{
				ref int reference = ref mPageEasings[i];
				if (i == index)
				{
					reference = 30;
				}
				else
				{
					reference = 0;
				}
			}
		}
		mImageTransform.UpdateTransform(mX, mY, mWidth, mHeight, mImage.mWidth, mImage.mHeight);
		if (mPrevImage != null)
		{
			mPrevImageTransform.UpdateTransform(mX, mY, mWidth, mHeight, mPrevImage.mWidth, mPrevImage.mHeight);
		}
		if (mNextImage != null)
		{
			mNextImageTransform.UpdateTransform(mX, mY, mWidth, mHeight, mNextImage.mWidth, mNextImage.mHeight);
		}
	}

	public Image TryLoadPage(string mLoadGroup, string mImageName)
	{
		if (mLastLoadedImageGroups.ContainsKey(mLoadGroup))
		{
			return mLastLoadedImageGroups[mLoadGroup];
		}
		if (TodCommon.TodLoadResources(mLoadGroup))
		{
			mLastLoadedImageGroups[mLoadGroup] = GlobalStaticVars.gSexyAppBase.mResourceManager.GetImageThrow(mImageName);
			return mLastLoadedImageGroups[mLoadGroup];
		}
		return null;
	}

	public void UnloadPage(string mLoadGroup)
	{
		if (mLastLoadedImageGroups.ContainsKey(mLoadGroup))
		{
			GlobalStaticVars.gLawnApp.mResourceManager.UnloadBackground(mLoadGroup);
			mLastLoadedImageGroups.Remove(mLoadGroup);
		}
	}
}
