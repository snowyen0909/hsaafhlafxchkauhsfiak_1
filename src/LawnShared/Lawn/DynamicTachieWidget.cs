using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

public class DynamicTachieWidget : Widget
{
	public enum PlaybackState
	{
		Entering,
		Idle,
		PlayingInteraction
	}

	public enum TouchArea
	{
		Touch,
		Touch2,
		Touch3
	}

	public enum TouchRewards
	{
		Invalid = -1,
		FumeshroomTouch1,
		FumeshroomTouch2,
		FumeshroomTouch3,
		NumTouchRewards
	}

	public readonly record struct DisplayTransform(float Scale, float OffsetX, float OffsetY);

	private const int CLOSE_BUTTON_TOGGLE_TICKS = 20;

	private const int CLOSE_BUTTON_MARGIN = 30;

	private const int MAX_TAP_MOVEMENT = 8;

	private readonly GraphicsDevice mGraphicsDevice;

	private readonly SkeletonRenderer mSkeletonRenderer;

	private readonly Atlas mAtlas;

	private readonly Skeleton mSkeleton;

	private readonly AnimationState mAnimationState;

	private readonly SkeletonBounds mSkeletonBounds = new SkeletonBounds();

	private readonly GameButton mCloseButton;

	private readonly DisplayTransform mDisplayTransform;

	private TrackEntry mBlockingTrackEntry;

	private int mCloseButtonToggleCounter = 20;

	private bool mShowCloseButton = true;

	private bool mPointerDragged;

	private TPoint mPointerDownPosition;

	public Widget mLastFocusedWidget;

	public static DataArray<Coin> mCoins = new DataArray<Coin>();

	public readonly LawnApp mApp;

	public PlaybackState State { get; private set; }

	public bool IsInputLocked => State != PlaybackState.Idle;

	public event Action<TouchArea> TouchCollision;

	public DynamicTachieWidget(string skelPath, LawnApp theApp)
		: this(skelPath, new DisplayTransform(1f, 0f, 0f), theApp)
	{
	}

	public DynamicTachieWidget(string skelPath, DisplayTransform displayTransform, LawnApp theApp)
	{
		if (string.IsNullOrWhiteSpace(skelPath))
		{
			throw new ArgumentException("A Spine .skel path is required.", "skelPath");
		}
		if (!string.Equals(Path.GetExtension(skelPath), ".skel", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException("DynamicTachieWidget only loads Spine .skel files.", "skelPath");
		}
		if (displayTransform.Scale <= 0f || float.IsNaN(displayTransform.Scale) || float.IsInfinity(displayTransform.Scale))
		{
			throw new ArgumentOutOfRangeException("displayTransform", "The display scale must be a finite value greater than zero.");
		}
		mDisplayTransform = displayTransform;
		mWantsFocus = true;
		mClip = false;
		mGraphicsDevice = GlobalStaticVars.g.GraphicsDevice;
		mSkeletonRenderer = CreateSkeletonRenderer(mGraphicsDevice);
		mApp = theApp;
		mCoins.DataArrayInitialize(1024u, "coins");
		try
		{
			string path = Path.ChangeExtension(skelPath, ".atlas");
			mAtlas = new Atlas(path, new XnaTextureLoader(mGraphicsDevice));
			SkeletonBinary skeletonBinary = new SkeletonBinary(mAtlas);
			SkeletonData skeletonData = skeletonBinary.ReadSkeletonData(skelPath);
			if (skeletonData.FindAnimation("idle") == null)
			{
				throw new InvalidDataException("The Spine skeleton '" + skelPath + "' does not contain an idle animation.");
			}
			mSkeleton = new Skeleton(skeletonData);
			mAnimationState = new AnimationState(new AnimationStateData(skeletonData));
			StartEntranceAnimation();
		}
		catch
		{
			mAtlas?.Dispose();
			throw;
		}
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.ZenGardenSH);
		mCloseButton = new GameButton(1001, this);
		mCloseButton.mButtonImage = AtlasResources.IMAGE_ALMANAC_CLOSEBUTTON;
		mCloseButton.mOverImage = null;
		mCloseButton.mDownImage = null;
		mCloseButton.SetFont(Resources.FONT_BRIANNETOD12);
		mCloseButton.mColors[0] = new SexyColor(42, 42, 90);
		mCloseButton.mColors[1] = new SexyColor(42, 42, 90);
		mCloseButton.mParentWidget = this;
		mCloseButton.mTextOffsetX = -8;
		mCloseButton.mTextOffsetY = 1;
		UpdateCloseButtonPosition();
		UpdateInputLock();
	}

	private static SkeletonRenderer CreateSkeletonRenderer(GraphicsDevice graphicsDevice)
	{
		SkeletonRenderer skeletonRenderer = new SkeletonRenderer(graphicsDevice, scissorTest: true)
		{
			PremultipliedAlpha = true
		};
		Effect effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\SpineEffect");
		effect.Parameters["World"].SetValue(Matrix.Identity);
		effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up));
		skeletonRenderer.Effect = effect;
		return skeletonRenderer;
	}

	public void FumeshroomReward()
	{
		mApp.WriteCurrentUserConfig();
		Coin coin = mCoins.DataArrayAlloc();
		float num = 5f;
		if (RandomNumbers.NextNumber(10) == 0)
		{
			mApp.PlaySample(Resources.SOUND_DIAMOND);
			coin.CoinInitialize(500, 320, CoinType.Diamond, CoinMotion.FromPresent);
			coin.mVelX = 0f;
			coin.mVelY = 0f - num;
			return;
		}
		mApp.PlaySample(Resources.SOUND_COIN);
		coin.CoinInitialize(500, 320, CoinType.Gold, CoinMotion.FromPresent);
		coin.mVelX = (float)Math.Sin(SexyMath.DegToRad(-150f)) * num;
		coin.mVelY = (float)Math.Cos(SexyMath.DegToRad(-150f)) * num;
		coin = mCoins.DataArrayAlloc();
		coin.CoinInitialize(500, 320, CoinType.Gold, CoinMotion.FromPresent);
		coin.mVelX = 0f;
		coin.mVelY = 0f - num;
		coin = mCoins.DataArrayAlloc();
		coin.CoinInitialize(500, 320, CoinType.Gold, CoinMotion.FromPresent);
		coin.mVelX = (float)Math.Sin(SexyMath.DegToRad(150f)) * num;
		coin.mVelY = (float)Math.Cos(SexyMath.DegToRad(150f)) * num;
	}

	private void StartEntranceAnimation()
	{
		if (mSkeleton.Data.FindAnimation("enter") == null)
		{
			StartIdleAnimation();
			return;
		}
		State = PlaybackState.Entering;
		mShowCloseButton = false;
		mCloseButtonToggleCounter = 0;
		mBlockingTrackEntry = mAnimationState.SetAnimation(0, "enter", loop: false);
		mBlockingTrackEntry.Complete += BlockingAnimationComplete;
		mAnimationState.AddAnimation(0, "idle", loop: true, 0f);
	}

	private void StartIdleAnimation()
	{
		mBlockingTrackEntry = null;
		mAnimationState.SetAnimation(0, "idle", loop: true);
		State = PlaybackState.Idle;
		UpdateInputLock();
	}

	private void BlockingAnimationComplete(TrackEntry entry)
	{
		if (entry == mBlockingTrackEntry)
		{
			mBlockingTrackEntry = null;
			State = PlaybackState.Idle;
			UpdateInputLock();
		}
	}

	public bool PlayInteractionAnimation(string animationName, int transitionFrames, AnimationState.TrackEntryDelegate animationCompleteAction = null)
	{
		if (transitionFrames < 0)
		{
			throw new ArgumentOutOfRangeException("transitionFrames");
		}
		if (IsInputLocked || string.IsNullOrWhiteSpace(animationName))
		{
			return false;
		}
		Animation animation = mSkeleton.Data.FindAnimation(animationName);
		if (animation == null || string.Equals(animationName, "idle", StringComparison.Ordinal))
		{
			return false;
		}
		float mixDuration = (float)transitionFrames / 100f;
		State = PlaybackState.PlayingInteraction;
		mBlockingTrackEntry = mAnimationState.SetAnimation(0, animation, loop: false);
		mBlockingTrackEntry.MixDuration = mixDuration;
		mBlockingTrackEntry.Complete += BlockingAnimationComplete;
		if (animationCompleteAction != null)
		{
			mBlockingTrackEntry.Complete += animationCompleteAction;
		}
		TrackEntry trackEntry = mAnimationState.AddAnimation(0, "idle", loop: true, 0f);
		trackEntry.SetMixDuration(mixDuration, 0f);
		UpdateInputLock();
		return true;
	}

	public override void Resize(int theX, int theY, int theWidth, int theHeight)
	{
		base.Resize(theX, theY, theWidth, theHeight);
		int num = Constants.BackBufferSize.X * 21 / 9;
		FullRect = new TRect((Constants.BackBufferSize.Y - num) / 2, 0, num, mHeight);
		CGPoint absPos = GetAbsPos();
		mSkeleton.ScaleX = mDisplayTransform.Scale;
		mSkeleton.ScaleY = mDisplayTransform.Scale;
		mSkeleton.X = absPos.mX + mDisplayTransform.OffsetX;
		mSkeleton.Y = absPos.mY + mDisplayTransform.OffsetY;
		UpdateCloseButtonPosition();
	}

	public override void Draw(Graphics g)
	{
		base.Draw(g);
		DeferOverlay();
	}

	public override void DrawOverlay(Graphics g)
	{
		g.SetColor(SexyColor.Black);
		g.FillRect(FullRect);
		mSkeletonRenderer.Effect.Parameters["Projection"].SetValue(g.SpineGetMatrices());
		mSkeletonRenderer.Effect.Parameters["World"].SetValue(Matrix.Identity);
		g.EndFrame();
		TRect inRect = g.GetClipRect();
		GlobalStaticVars.gSexyAppBase.mScreenScales.Scale(ref inRect);
		inRect = inRect.Intersection(new TRect(0, 0, mGraphicsDevice.PresentationParameters.BackBufferWidth, mGraphicsDevice.PresentationParameters.BackBufferHeight));
		if (inRect.mWidth > 0 && inRect.mHeight > 0)
		{
			mGraphicsDevice.ScissorRectangle = inRect;
			mSkeletonRenderer.Begin();
			mSkeletonRenderer.Draw(mSkeleton);
			mSkeletonRenderer.End();
		}
		g.EndDrawImageTransformed();
		Coin theItem = null;
		while (mCoins.IterateNext(ref theItem))
		{
			if (!theItem.mDead)
			{
				theItem.Draw(g);
			}
		}
		if (mShowCloseButton || mCloseButtonToggleCounter > 0)
		{
			mCloseButton.Draw(g);
		}
	}

	public override void Update()
	{
		base.Update();
		Coin theItem = null;
		while (mCoins.IterateNext(ref theItem))
		{
			if (!theItem.mDead)
			{
				theItem.Update();
			}
		}
		float delta = 0.01f;
		mSkeleton.Update(delta);
		mAnimationState.Update(delta);
		mAnimationState.Apply(mSkeleton);
		mSkeleton.UpdateWorldTransform(Skeleton.Physics.Update);
		mSkeletonBounds.Update(mSkeleton, updateAabb: true);
		UpdateCloseButtonToggle();
		mCloseButton.Update();
		MarkDirty();
	}

	private void UpdateCloseButtonToggle()
	{
		if (mShowCloseButton)
		{
			if (mCloseButtonToggleCounter < 20)
			{
				mCloseButtonToggleCounter++;
			}
		}
		else if (mCloseButtonToggleCounter > 0)
		{
			mCloseButtonToggleCounter--;
		}
		UpdateCloseButtonPosition();
	}

	private void UpdateCloseButtonPosition()
	{
		if (mCloseButton != null)
		{
			float num = TodCommon.TodAnimateCurveFloat(0, 20, mCloseButtonToggleCounter, 0f, 1f, TodCurves.EaseOut);
			int num2 = Constants.Almanac_CloseButtonRect.mHeight;
			int theY = (int)((float)(30 + num2) * num) - num2;
			int num3 = (GlobalStaticVars.gSexyAppBase.mScreenScales.mVirtualWidth + Constants.BOARD_WIDTH) / 2;
			mCloseButton.Resize(num3 - Constants.Almanac_CloseButtonRect.mWidth - 30, theY, Constants.Almanac_CloseButtonRect.mWidth, num2);
		}
	}

	public void ToggleCloseButton()
	{
		if (!IsInputLocked)
		{
			mShowCloseButton = !mShowCloseButton;
		}
	}

	public void RetractCloseButton()
	{
		mShowCloseButton = false;
	}

	private void UpdateInputLock()
	{
		mCloseButton?.SetDisabled(IsInputLocked);
	}

	public override void MouseDown(int x, int y, int theBtnNum, int theClickCount)
	{
		mPointerDownPosition = new TPoint(x, y);
		mPointerDragged = false;
	}

	public override void MouseDrag(int x, int y)
	{
		if (!new TRect(mPointerDownPosition.mX - 8, mPointerDownPosition.mY - 8, 16, 16).Contains(x, y))
		{
			mPointerDragged = true;
		}
	}

	public override void MouseUp(int x, int y, int theBtnNum, int theClickCount)
	{
		if (!IsInputLocked && !mPointerDragged && theClickCount >= 1)
		{
			TouchArea area;
			if (mShowCloseButton && CloseButtonContains(x, y))
			{
				DoExit();
			}
			else if (TryGetTouchArea(x, y, out area))
			{
				OnTouchCollision(area);
			}
			else
			{
				ToggleCloseButton();
			}
		}
	}

	private bool CloseButtonContains(int x, int y)
	{
		if (x >= mCloseButton.mX && x < mCloseButton.mX + mCloseButton.mWidth && y >= mCloseButton.mY)
		{
			return y < mCloseButton.mY + mCloseButton.mHeight;
		}
		return false;
	}

	private bool TryGetTouchArea(int x, int y, out TouchArea area)
	{
		area = TouchArea.Touch;
		if (IsInputLocked)
		{
			return false;
		}
		CGPoint absPos = GetAbsPos();
		float x2 = absPos.mX + (float)x;
		float y2 = absPos.mY + (float)y;
		BoundingBoxAttachment[] items = mSkeletonBounds.BoundingBoxes.Items;
		Polygon[] items2 = mSkeletonBounds.Polygons.Items;
		for (int i = 0; i < mSkeletonBounds.BoundingBoxes.Count; i++)
		{
			if (TryParseTouchArea(items[i].Name, out var area2) && mSkeletonBounds.ContainsPoint(items2[i], x2, y2))
			{
				area = area2;
				return true;
			}
		}
		return false;
	}

	private static bool TryParseTouchArea(string name, out TouchArea area)
	{
		switch (name)
		{
		case "touch":
			area = TouchArea.Touch;
			return true;
		case "touch2":
			area = TouchArea.Touch2;
			return true;
		case "touch3":
			area = TouchArea.Touch3;
			return true;
		default:
			area = TouchArea.Touch;
			return false;
		}
	}

	protected virtual void OnTouchCollision(TouchArea area)
	{
		TouchCollision?.Invoke(area);
	}

	public override bool BackButtonPress()
	{
		if (!IsInputLocked)
		{
			DoExit();
		}
		return true;
	}

	private void DoExit()
	{
		mApp.mMusic.StopAllMusic();
		WidgetContainer widgetContainer = mParent;
		if (widgetContainer != null)
		{
			widgetContainer.RemoveWidget(this);
			if (mLastFocusedWidget != null)
			{
				GlobalStaticVars.gSexyAppBase.mWidgetManager.SetFocus(mLastFocusedWidget);
			}
			GlobalStaticVars.gSexyAppBase.SafeDeleteWidget(this);
		}
		mApp.mMusic.MakeSureMusicIsPlaying(MusicTune.TitleCrazyDaveMainTheme);
	}

	public override void Dispose()
	{
		mCoins.DataArrayDispose();
		mCloseButton.Dispose();
		mAtlas.Dispose();
		base.Dispose();
	}
}
