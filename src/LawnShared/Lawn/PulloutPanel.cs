using System;
using System.Diagnostics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class PulloutPanel : Widget
{
	private sealed class PanelHandle : Widget
	{
		private readonly PulloutPanel mOwner;

		public PanelHandle(PulloutPanel owner)
		{
			mOwner = owner;
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			if (mOwner.mHolding || mIsOver)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(208, 255, 144));
			}
			g.DrawImage(mOwner.mHandleImage, 0, 0);
			g.PopState();
		}

		public override void MouseDown(int x, int y, int button, int clicks)
		{
			mOwner.BeginGesture(x, y, button);
		}

		public override void MouseDrag(int x, int y)
		{
			mOwner.DragGesture(x, y);
		}

		public override void MouseUp(int x, int y, int button, int clicks)
		{
			mOwner.EndGesture(x, y, button);
		}

		public override void TouchesCanceled()
		{
			mOwner.CancelGesture();
		}
	}

	private sealed class PanelBody : Widget
	{
		public const int BorderSize = 20;

		public PanelBody()
		{
			ClipInput = true;
		}

		public override void Dispose()
		{
			RemoveAllWidgets(doDelete: true, recursive: true);
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			LawnCommon.DrawImageBox(g, new TRect(0, 0, mWidth, mHeight), AtlasResources.IMAGE_ALMANAC_STONE_BORDER);
		}
	}

	private readonly PanelBody mBody;

	private readonly PanelHandle mHandle;

	private Image mHandleImage;

	private int mHandleOverlap;

	private float mExpansion;

	private float mAnimationFrom;

	private float mAnimationTo;

	private float mPressExpansion;

	private long mAnimationStartedAt;

	private double mAnimationSeconds;

	private bool mHolding;

	private bool mMoved;

	private int mPressX;

	private int mPressY;

	public Widget Content { get; }

	public Widget Footer { get; }

	public float Expansion => mExpansion;

	public bool IsExpanded { get; private set; }

	public bool IsAnimating { get; private set; }

	public bool IsDragging
	{
		get
		{
			if (mHolding)
			{
				return mMoved;
			}
			return false;
		}
	}

	public TRect BodyBounds => mBody.GetRect();

	public TRect HandleBounds => mHandle.GetRect();

	public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromSeconds(0.3);

	private int Travel => Math.Max(0, mHeight - mHandleImage.mHeight);

	public PulloutPanel(Image handleImage, int handleOverlap = 15, Widget content = null, Widget footer = null)
	{
		mPriority = 1;
		mMouseVisible = false;
		ClipInput = true;
		mBody = new PanelBody();
		Content = content ?? new Widget
		{
			mMouseVisible = false
		};
		Content.ClipInput = true;
		mBody.AddWidget(Content);
		mHandle = new PanelHandle(this);
		AddWidget(mBody);
		Footer = footer;
		if (Footer != null)
		{
			Footer.ClipInput = true;
			AddWidget(Footer);
		}
		AddWidget(mHandle);
		SetHandleImage(handleImage, handleOverlap);
	}

	public void SetHandleImage(Image image, int overlap)
	{
		if (image == null)
		{
			throw new ArgumentNullException("image");
		}
		if (overlap < 0 || overlap >= image.mHeight)
		{
			throw new ArgumentOutOfRangeException("overlap");
		}
		CancelGesture();
		mHandleImage = image;
		mHandleOverlap = overlap;
		LayoutPanel();
	}

	public override void Resize(int x, int y, int width, int height)
	{
		CancelGesture();
		base.Resize(x, y, width, height);
		LayoutPanel();
	}

	public void SetExpanded(bool expanded, bool animate = true)
	{
		mHolding = false;
		AnimateTo(expanded, GetTimestamp(), animate);
	}

	protected virtual long GetTimestamp()
	{
		return Stopwatch.GetTimestamp();
	}

	private void LayoutPanel()
	{
		if (mHandleImage != null)
		{
			int num = mHandleImage.mHeight;
			int num2 = (int)Math.Round((float)mHeight - (float)Travel * mExpansion);
			mBody.Resize(0, num2, mWidth, Math.Max(0, mHeight - 2 * num));
			mHandle.Resize((mWidth - mHandleImage.mWidth) / 2, num2 - num + mHandleOverlap, mHandleImage.mWidth, num);
			int num3 = 20;
			Content.Resize(num3, num3, Math.Max(0, mWidth - 2 * num3), Math.Max(0, mBody.mHeight - 2 * num3));
			mBody.SetVisible(mExpansion > 0f);
			if (Footer != null)
			{
				Footer.Resize(0, mBody.mY + mBody.mHeight, mWidth, num);
				Footer.SetVisible(mExpansion > 0f);
			}
			MarkDirty();
		}
	}

	private void AdvanceAnimation(long now)
	{
		if (IsAnimating)
		{
			double num = (double)(now - mAnimationStartedAt) / (double)Stopwatch.Frequency;
			float num2 = ((mAnimationSeconds <= 0.0) ? 1f : ((float)(num / mAnimationSeconds)));
			mExpansion = TodCommon.TodCurveEvaluateClamped(num2, mAnimationFrom, mAnimationTo, TodCurves.EaseInOut);
			if (num2 >= 1f)
			{
				mExpansion = mAnimationTo;
				IsAnimating = false;
			}
			LayoutPanel();
		}
	}

	private void AnimateTo(bool expanded, long now, bool animate = true)
	{
		AdvanceAnimation(now);
		IsExpanded = expanded;
		mAnimationFrom = mExpansion;
		mAnimationTo = (expanded ? 1 : 0);
		mAnimationStartedAt = now;
		mAnimationSeconds = Math.Max(0.0, AnimationDuration.TotalSeconds);
		IsAnimating = animate && mAnimationSeconds > 0.0 && mAnimationFrom != mAnimationTo;
		if (!IsAnimating)
		{
			mExpansion = mAnimationTo;
		}
		LayoutPanel();
	}

	private void BeginGesture(int x, int y, int button)
	{
		if (button == 0 && !mHolding)
		{
			AdvanceAnimation(GetTimestamp());
			IsAnimating = false;
			mHolding = true;
			mMoved = false;
			CGPoint absPos = mHandle.GetAbsPos();
			mPressX = (int)absPos.mX + x;
			mPressY = (int)absPos.mY + y;
			mPressExpansion = mExpansion;
			MarkDirty();
		}
	}

	private void DragGesture(int x, int y)
	{
		if (mHolding)
		{
			CGPoint absPos = mHandle.GetAbsPos();
			int num = (int)absPos.mX + x - mPressX;
			int num2 = (int)absPos.mY + y - mPressY;
			mMoved |= num != 0 || num2 != 0;
			mExpansion = ((Travel == 0) ? 0f : Math.Clamp(mPressExpansion - (float)num2 / (float)Travel, 0f, 1f));
			LayoutPanel();
		}
	}

	private void EndGesture(int x, int y, int button)
	{
		if (button == 0 && mHolding)
		{
			if (mWidgetManager == null || !mWidgetManager.mHasFocus || !mVisible || mDisabled || (mWidgetManager.mBaseModalWidget != null && mWidgetManager.IsBelow(this, mWidgetManager.mBaseModalWidget)))
			{
				CancelGesture();
				return;
			}
			DragGesture(x, y);
			bool flag = !mMoved && mHandle.mIsOver && mWidgetManager != null && mWidgetManager.mHasFocus;
			mHolding = false;
			AnimateTo(flag ? (!IsExpanded) : (mExpansion > 0.5f), GetTimestamp());
		}
	}

	private void CancelGesture()
	{
		if (mHolding)
		{
			mHolding = false;
			AnimateTo(mExpansion > 0.5f, GetTimestamp());
		}
	}

	public override void Update()
	{
		if (mHolding && (!mHandle.mIsDown || mWidgetManager == null || !mWidgetManager.mHasFocus || mDisabled))
		{
			CancelGesture();
		}
		AdvanceAnimation(GetTimestamp());
		base.Update();
	}

	public override void SetVisible(bool visible)
	{
		if (!visible)
		{
			CancelGesture();
		}
		base.SetVisible(visible);
	}

	public override void RemovedFromManager(WidgetManager manager)
	{
		mHolding = false;
		IsAnimating = false;
		mExpansion = 0f;
		IsExpanded = false;
		LayoutPanel();
		base.RemovedFromManager(manager);
	}

	public override void Dispose()
	{
		mHolding = false;
		RemoveAllWidgets(doDelete: true, recursive: true);
		base.Dispose();
	}
}
