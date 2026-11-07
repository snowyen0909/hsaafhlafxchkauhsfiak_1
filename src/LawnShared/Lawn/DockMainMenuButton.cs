using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

public class DockMainMenuButton : MainMenuButton
{
	private readonly List<MainMenuButton> mItems = new List<MainMenuButton>();

	private readonly Dictionary<MainMenuButton, (bool MouseVisible, bool Visible, bool DrawWithParent)> mItemDefaults = new Dictionary<MainMenuButton, (bool, bool, bool)>();

	private DockDirection mDirection = DockDirection.Right;

	private TimeSpan mHoldDuration = TimeSpan.FromSeconds(3.0);

	private TimeSpan mAnimationDuration = TimeSpan.FromMilliseconds(300.0);

	private int mItemSpacing = 16;

	private TRect mExpandedBounds;

	private float mExpansion;

	private float mAnimationFrom;

	private float mAnimationTo;

	private long mAnimationStartedAt;

	private bool mHolding;

	private int mHeldButton;

	private int mClickCount;

	private long? mHoldStartedAt;

	private MainMenuButton mSelected;

	public IReadOnlyList<MainMenuButton> Items { get; }

	public bool IsExpanded { get; private set; }

	public bool IsAnimating { get; private set; }

	public DockDirection ExpandDirection
	{
		get
		{
			return mDirection;
		}
		set
		{
			if (!Enum.IsDefined(typeof(DockDirection), value))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (mDirection != value)
			{
				ResetDock();
				mDirection = value;
				LayoutItems();
			}
		}
	}

	public TimeSpan HoldDuration
	{
		get
		{
			return mHoldDuration;
		}
		set
		{
			if (value < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			ResetDock();
			mHoldDuration = value;
		}
	}

	public TimeSpan AnimationDuration
	{
		get
		{
			return mAnimationDuration;
		}
		set
		{
			if (value < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			ResetDock();
			mAnimationDuration = value;
		}
	}

	public int ItemSpacing
	{
		get
		{
			return mItemSpacing;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (mItemSpacing != value)
			{
				ResetDock();
				mItemSpacing = value;
				LayoutItems();
			}
		}
	}

	public DockMainMenuButton(Image image, int id, ButtonListener listener)
		: base(image, id, listener)
	{
		Items = mItems.AsReadOnly();
		mClip = false;
	}

	public new static DockMainMenuButton MakeNewButton(int id, ButtonListener listener, Font font, Image image, int x = 0, int y = 0)
	{
		return MakeNewButton(id, listener, null, font, image, x, y);
	}

	public new static DockMainMenuButton MakeNewButton(int id, ButtonListener listener, string label, Font font, Image image, int x = 0, int y = 0)
	{
		DockMainMenuButton dockMainMenuButton = new DockMainMenuButton(null, id, listener);
		dockMainMenuButton.InitializeAppearance(label, font, image, x, y);
		return dockMainMenuButton;
	}

	public void AddItem(MainMenuButton button)
	{
		AddWidget(button);
	}

	public void RemoveItem(MainMenuButton button)
	{
		RemoveWidget(button);
	}

	public override void AddWidget(Widget widget)
	{
		if (!(widget is MainMenuButton mainMenuButton) || widget is DockMainMenuButton || widget.mWidgets.Count != 0)
		{
			throw new ArgumentException("Dock items must be individual main menu buttons.", "widget");
		}
		if (!mItems.Contains(mainMenuButton))
		{
			if (mainMenuButton.mParent != null || mainMenuButton.mWidgetManager != null)
			{
				throw new ArgumentException("The button already belongs to another container.", "widget");
			}
			ResetDock();
			mItemDefaults.Add(mainMenuButton, (mainMenuButton.mMouseVisible, mainMenuButton.mVisible, mainMenuButton.DrawWithParent));
			mainMenuButton.mMouseVisible = false;
			mainMenuButton.DrawWithParent = true;
			mainMenuButton.SetVisible(isVisible: false);
			mItems.Add(mainMenuButton);
			base.AddWidget(mainMenuButton);
			LayoutItems();
		}
	}

	public override void RemoveWidget(Widget widget)
	{
		if (widget is MainMenuButton mainMenuButton && mItems.Contains(mainMenuButton))
		{
			ResetDock();
			base.RemoveWidget(mainMenuButton);
			mItems.Remove(mainMenuButton);
			(bool, bool, bool) tuple = mItemDefaults[mainMenuButton];
			mItemDefaults.Remove(mainMenuButton);
			(mainMenuButton.mMouseVisible, _, mainMenuButton.DrawWithParent) = tuple;
			mainMenuButton.SetVisible(tuple.Item2);
			mainMenuButton.SetPressedVisual(null);
			LayoutItems();
		}
	}

	public override void Resize(int x, int y, int width, int height)
	{
		base.Resize(x, y, width, height);
		LayoutItems();
	}

	private void LayoutItems()
	{
		mExpandedBounds = base.GetInsetRect();
		mExpandedBounds.Offset(-mX, -mY);
		bool flag = mDirection == DockDirection.Left || mDirection == DockDirection.Right;
		bool flag2 = mDirection == DockDirection.Right || mDirection == DockDirection.Down;
		int num = (flag2 ? (flag ? mWidth : mHeight) : 0);
		foreach (MainMenuButton mItem in mItems)
		{
			int num2 = (flag ? mItem.mWidth : mItem.mHeight);
			int num3 = (flag2 ? (num + mItemSpacing) : (num - mItemSpacing - num2));
			int num4 = (mWidth - mItem.mWidth) / 2;
			int num5 = (mHeight - mItem.mHeight) / 2;
			int num6 = (flag ? num3 : num4);
			int num7 = (flag ? num5 : num3);
			Insets insets = mItem.mMouseInsets;
			mExpandedBounds = mExpandedBounds.Union(new TRect(num6 + insets.mLeft, num7 + insets.mTop, mItem.mWidth - insets.mLeft - insets.mRight, mItem.mHeight - insets.mTop - insets.mBottom));
			mItem.Move((int)Math.Round((float)num4 + (float)(num6 - num4) * mExpansion), (int)Math.Round((float)num5 + (float)(num7 - num5) * mExpansion));
			mItem.SetVisible(mExpansion > 0f);
			num = (flag2 ? (num3 + num2) : num3);
		}
	}

	public override void Draw(Graphics g)
	{
		if (mBtnNoDraw)
		{
			return;
		}
		for (int num = mItems.Count - 1; num >= 0; num--)
		{
			MainMenuButton mainMenuButton = mItems[num];
			if (mainMenuButton.mVisible)
			{
				g.PushState();
				g.Translate(mainMenuButton.mX, mainMenuButton.mY);
				mainMenuButton.DrawButton(g);
				g.PopState();
			}
		}
		base.Draw(g);
	}

	public override TRect GetInsetRect()
	{
		if (!IsExpanded)
		{
			return base.GetInsetRect();
		}
		TRect result = mExpandedBounds;
		result.Offset(mX, mY);
		return result;
	}

	public override bool IsPointVisible(int x, int y)
	{
		if (!IsExpanded)
		{
			return base.IsPointVisible(x, y);
		}
		return mExpandedBounds.Contains(x, y);
	}

	private MainMenuButton ButtonAt(int x, int y)
	{
		if (base.GetInsetRect().Contains(x + mX, y + mY) && base.IsPointVisible(x, y))
		{
			return this;
		}
		if (IsExpanded)
		{
			foreach (MainMenuButton mItem in mItems)
			{
				if (mItem.mVisible && !mItem.mDisabled && mItem.GetInsetRect().Contains(x, y) && mItem.IsPointVisible(x - mItem.mX, y - mItem.mY))
				{
					return mItem;
				}
			}
		}
		return null;
	}

	protected virtual long GetTimestamp()
	{
		return Stopwatch.GetTimestamp();
	}

	private void AnimateTo(bool expanded, long now)
	{
		AdvanceAnimation(now);
		IsExpanded = expanded;
		mAnimationFrom = mExpansion;
		mAnimationTo = (expanded ? 1 : 0);
		mAnimationStartedAt = now;
		IsAnimating = mAnimationFrom != mAnimationTo;
		AdvanceAnimation(now);
	}

	private void AdvanceAnimation(long now)
	{
		if (IsAnimating)
		{
			double num = (double)(now - mAnimationStartedAt) / (double)Stopwatch.Frequency;
			float num2 = ((mAnimationDuration == TimeSpan.Zero) ? 1f : ((float)(num / mAnimationDuration.TotalSeconds)));
			TodCurves theCurve = (IsExpanded ? TodCurves.EaseOut : TodCurves.EaseIn);
			mExpansion = TodCommon.TodCurveEvaluateClamped(num2, mAnimationFrom, mAnimationTo, theCurve);
			if (num2 >= 1f)
			{
				mExpansion = mAnimationTo;
				IsAnimating = false;
			}
		}
		LayoutItems();
	}

	public override void MouseDown(int x, int y, int button, int clickCount)
	{
		if (!mHolding)
		{
			mHolding = true;
			mHeldButton = button;
			mClickCount = clickCount;
			mHoldStartedAt = GetTimestamp();
			SelectButton(this);
			base.MouseDown(x, y, button, clickCount);
		}
	}

	public override void MouseDrag(int x, int y)
	{
		long timestamp = GetTimestamp();
		AdvanceAnimation(timestamp);
		UpdateGesture(x, y, timestamp);
	}

	private void UpdateGesture(int x, int y, long now)
	{
		if (!mHolding)
		{
			return;
		}
		if (!IsExpanded && mHeldButton == 0 && mItems.Count != 0)
		{
			if (mHoldStartedAt.HasValue && (double)(now - mHoldStartedAt.Value) / (double)Stopwatch.Frequency >= mHoldDuration.TotalSeconds)
			{
				AnimateTo(expanded: true, now);
			}
			else if (ButtonAt(x, y) == this)
			{
				long valueOrDefault = mHoldStartedAt.GetValueOrDefault();
				if (!mHoldStartedAt.HasValue)
				{
					valueOrDefault = now;
					mHoldStartedAt = valueOrDefault;
				}
			}
			else
			{
				mHoldStartedAt = null;
			}
		}
		SelectButton(ButtonAt(x, y));
	}

	private void SelectButton(MainMenuButton selected)
	{
		MainMenuButton mainMenuButton = mSelected;
		mSelected = selected;
		SetPressedVisual(mHolding && selected == this);
		foreach (MainMenuButton mItem in mItems)
		{
			mItem.SetPressedVisual(mHolding && selected == mItem);
		}
		if (selected != null && selected != this && selected != mainMenuButton)
		{
			selected.mButtonListener.ButtonPress(selected.mId, mClickCount);
		}
	}

	public override void Update()
	{
		long timestamp = GetTimestamp();
		AdvanceAnimation(timestamp);
		if (mHolding)
		{
			if (!mIsDown || !mVisible || mDisabled || mWidgetManager == null || !mWidgetManager.mHasFocus)
			{
				EndGesture(timestamp);
			}
			else
			{
				CGPoint absPos = GetAbsPos();
				UpdateGesture(mWidgetManager.mLastMouseX - (int)absPos.mX, mWidgetManager.mLastMouseY - (int)absPos.mY, timestamp);
			}
		}
		base.Update();
	}

	public override void MouseUp(int x, int y, int button, int clickCount)
	{
		if (mHolding && button == mHeldButton)
		{
			MainMenuButton mainMenuButton = ((mIsOver && mWidgetManager != null && mWidgetManager.mHasFocus) ? ButtonAt(x, y) : null);
			EndGesture(GetTimestamp());
			mainMenuButton?.mButtonListener.ButtonDepress(mainMenuButton.mId);
		}
	}

	private void EndGesture(long now)
	{
		mHolding = false;
		mHoldStartedAt = null;
		SelectButton(null);
		AnimateTo(expanded: false, now);
	}

	private void ResetDock()
	{
		mHolding = false;
		mHoldStartedAt = null;
		SelectButton(null);
		IsExpanded = false;
		IsAnimating = false;
		mExpansion = 0f;
		LayoutItems();
	}

	public override void SetVisible(bool visible)
	{
		if (!visible)
		{
			ResetDock();
		}
		base.SetVisible(visible);
	}

	public override void RemovedFromManager(WidgetManager manager)
	{
		ResetDock();
		base.RemovedFromManager(manager);
	}

	public override void Dispose()
	{
		ResetDock();
		RemoveAllWidgets(doDelete: true, recursive: true);
		base.Dispose();
	}
}
