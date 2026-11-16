using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sexy;
using Sexy.TodLib;

namespace Lawn;

internal sealed class MailScreen : Widget, ButtonListener
{
	private sealed class LeavesWidget : Widget
	{
		private readonly Image mImage;

		internal LeavesWidget(Image theImage)
		{
			mImage = theImage;
			mMouseVisible = false;
		}

		public override void Draw(Graphics g)
		{
			DrawCover(g, mImage, mWidth, mHeight);
		}
	}

	private sealed class MailSheetWidget : Widget
	{
		internal readonly LocalMail mMail;

		internal MailMotion mTurn;

		private readonly MailPaperStyle mStyle;

		private readonly Image mPaper;

		private readonly float mPaperScale;

		private readonly Image mImage;

		private readonly TPoint mImagePosition;

		private readonly string mTitle;

		private readonly MailTextWidget mText;

		private readonly MailScrollWidget mScroll;

		private readonly List<MailRewardWidget> mRewards = new List<MailRewardWidget>();

		private int mBaseX;

		private int mBaseY;

		private bool mShowContent;

		internal MailSheetWidget(MailScreen theOwner, LocalMail theMail)
		{
			mMail = theMail;
			mStyle = MailPaperStyle.GetPaperStyle(theMail.IsText() ? MailPaperType.Cardstock : MailPaperType.Zombie);
			theOwner.LoadResourceGroup(mStyle.mResourceGroup);
			mPaper = theOwner.mApp.mResourceManager.GetImageThrow(mStyle.mImageId);
			mPaperScale = (theMail.IsText() ? 0.85f : 1f);
			mMouseVisible = false;
			mClip = false;
			ClipInput = true;
			if (theMail.IsText())
			{
				mTitle = TodStringFile.TodStringTranslate(theMail.mData.GetProperty("TitleKey").GetString());
				string text = TodStringFile.TodStringTranslate(theMail.mData.GetProperty("BodyKey").GetString());
				if (theMail.mData.TryGetProperty("Version", out var value) && value.ValueKind == JsonValueKind.String)
				{
					mTitle = mTitle.Replace("{VERSION}", value.GetString());
					text = text.Replace("{VERSION}", value.GetString());
				}
				mText = new MailTextWidget(text, mStyle);
				mScroll = new MailScrollWidget(mText);
				mScroll.SetScrollMode(ScrollWidget.ScrollMode.Vertical);
				mScroll.EnableIndicators(AtlasResources.IMAGE_SCROLL_INDICATOR);
				mScroll.ClipInput = true;
				AddWidget(mScroll);
				for (int i = 0; i < theMail.mRewards.Length; i++)
				{
					MailRewardWidget mailRewardWidget = new MailRewardWidget(theOwner, theMail, i);
					mRewards.Add(mailRewardWidget);
					mText.AddWidget(mailRewardWidget);
				}
				return;
			}
			switch (theMail.mType)
			{
			case MailType.HelpZombieNote:
				theOwner.LoadResourceGroup("DelayLoad_ZombieNoteHelp");
				mImage = Resources.IMAGE_ZOMBIE_NOTE_HELP;
				mImagePosition = new TPoint(51, 52);
				break;
			case MailType.ZombieNote1:
				theOwner.LoadResourceGroup("DelayLoad_ZombieNote1");
				mImage = Resources.IMAGE_ZOMBIE_NOTE1;
				mImagePosition = new TPoint(51, 52);
				break;
			case MailType.ZombieNote2:
				theOwner.LoadResourceGroup("DelayLoad_ZombieNote2");
				mImage = Resources.IMAGE_ZOMBIE_NOTE2;
				mImagePosition = new TPoint(53, 47);
				break;
			case MailType.ZombieNote3:
				theOwner.LoadResourceGroup("DelayLoad_ZombieNote3");
				mImage = Resources.IMAGE_ZOMBIE_NOTE3;
				mImagePosition = new TPoint(40, 37);
				break;
			case MailType.ZombieNote4:
				theOwner.LoadResourceGroup("DelayLoad_ZombieNote4");
				mImage = Resources.IMAGE_ZOMBIE_NOTE4;
				mImagePosition = new TPoint(22, 37);
				break;
			case MailType.FinalZombieNote:
				theOwner.LoadResourceGroup("DelayLoad_ZombieFinalNote");
				mImage = Resources.IMAGE_ZOMBIE_FINAL_NOTE;
				mImagePosition = new TPoint(34, 58);
				break;
			case MailType.CreditsZombieNote:
				theOwner.LoadResourceGroup("DelayLoad_Credits");
				mImage = Resources.IMAGE_CREDITS_ZOMBIENOTE;
				mImagePosition = new TPoint(74, 43);
				break;
			default:
				throw new ArgumentOutOfRangeException("mType");
			}
		}

		public override void Dispose()
		{
			RemoveAllWidgets(doDelete: true, recursive: true);
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			g.SetLinearBlend(linear: true);
			TodCommon.TodDrawImageScaledF(g, mPaper, 0f, 0f, mPaperScale, mPaperScale);
			if (mShowContent)
			{
				if (mImage != null)
				{
					g.DrawImage(mImage, mImagePosition.mX, mImagePosition.mY);
				}
				else if (mMail.IsText())
				{
					TRect mTitleBounds = mStyle.mTitleBounds;
					TodCommon.TodDrawString(g, mTitle, mTitleBounds.mX + mTitleBounds.mWidth / 2, mTitleBounds.mY + mTitleBounds.mHeight * 3 / 4, Resources.FONT_BRIANNETOD16, mStyle.mInk, DrawStringJustification.Center);
				}
			}
		}

		internal void LayoutPaper(int theWidth, int theHeight)
		{
			mBaseX = (theWidth - Resources.IMAGE_ZOMBIE_NOTE.mWidth) / 2 + mStyle.mOffsetX;
			mBaseY = (theHeight - Resources.IMAGE_ZOMBIE_NOTE.mHeight) / 2;
			Resize(mBaseX, mBaseY, (int)Math.Ceiling((float)mPaper.mWidth * mPaperScale), (int)Math.Ceiling((float)mPaper.mHeight * mPaperScale));
			if (mScroll == null)
			{
				return;
			}
			TRect mContentBounds = mStyle.mContentBounds;
			mText.LayoutText(Math.Max(1, mContentBounds.mWidth - AtlasResources.IMAGE_SCROLL_INDICATOR.mWidth - 4));
			if (mRewards.Count > 0)
			{
				int num = 110;
				int num2 = 20;
				int num3 = Math.Max(mText.mHeight + 20, mContentBounds.mHeight - 20 - num);
				int num4 = (mWidth - mRewards.Count * num - (mRewards.Count - 1) * num2) / 2 - mContentBounds.mX;
				for (int i = 0; i < mRewards.Count; i++)
				{
					mRewards[i].Resize(num4 + i * (num + num2), num3, num, num);
				}
				mText.Resize(mText.mX, mText.mY, mContentBounds.mWidth, num3 + num);
				mScroll.SetScrollInsets(new Insets(0, 0, 0, 20));
			}
			mScroll.Resize(mContentBounds.mX, mContentBounds.mY, mContentBounds.mWidth, mContentBounds.mHeight);
			mScroll.ClientSizeChanged();
			mScroll.ScrollPixels(0f);
		}

		internal int GetExitDistance()
		{
			return mBaseX + mWidth + 15;
		}

		internal bool IsFlying()
		{
			return mTurn.IsMoving;
		}

		internal void SetPaperOffset(float theOffset, bool theContentVisible)
		{
			Move(mBaseX + (int)Math.Round(theOffset), mBaseY);
			ShowContent(theContentVisible);
		}

		internal MailFlightWidget CreateFlight(int theStackIndex, float theSlideDistance, float theY)
		{
			return new MailFlightWidget(CaptureImage(), theStackIndex, mBaseX, mBaseY, theY, (0f - theSlideDistance) * mTurn.Position, 15f * (1f - mTurn.Position));
		}

		internal void ShowContent(bool theVisible)
		{
			mShowContent = theVisible;
			if (mScroll != null)
			{
				mScroll.SetVisible(theVisible);
			}
		}

		internal void ShowUnclaimedRewards()
		{
			mScroll?.ScrollToBottom();
			foreach (MailRewardWidget mReward in mRewards)
			{
				if (!mReward.IsClaimed())
				{
					mReward.Flash();
				}
			}
		}

		internal MailScrollWidget GetScrollAt(int theX, int theY)
		{
			if (IsFlying() || mScroll == null || !mScroll.mVisible || !mScroll.GetRect().Contains(theX, theY))
			{
				return null;
			}
			return mScroll;
		}

		internal void ScrollBy(float thePixels)
		{
			mScroll?.ScrollPixels(thePixels);
		}

		internal void ScrollToTop()
		{
			mScroll?.ScrollToOffset(0f);
		}

		private MemoryImage CaptureImage()
		{
			MemoryImage memoryImage = new MemoryImage();
			bool theVisible = mShowContent;
			ShowContent(theVisible: true);
			try
			{
				lock (ResourceManager.DrawLocker)
				{
					memoryImage.Create(mWidth, mHeight);
					Graphics graphics = Graphics.GetNew(memoryImage);
					try
					{
						graphics.BeginFrame();
						memoryImage.Clear();
						DrawAll(new ModalFlags
						{
							mIsOver = true,
							mOverFlags = 12
						}, graphics);
						graphics.EndFrame();
						return memoryImage;
					}
					finally
					{
						graphics.SetRenderTarget(null);
						graphics.PrepareForReuse();
					}
				}
			}
			catch
			{
				memoryImage.Dispose();
				throw;
			}
			finally
			{
				ShowContent(theVisible);
			}
		}
	}

	private sealed class MailFlightWidget : Widget
	{
		internal int mStackIndex;

		private readonly MemoryImage mImage;

		private readonly int mFromX;

		private readonly int mFromY;

		private readonly float mDistanceY;

		private readonly float mPaperOffset;

		private readonly float mStackSpacing;

		private int mTick;

		private float mProgress;

		internal MailFlightWidget(MemoryImage theImage, int theStackIndex, int theX, int theY, float theDistanceY, float thePaperOffset, float theStackSpacing)
		{
			mImage = theImage;
			mStackIndex = theStackIndex;
			mFromX = theX;
			mFromY = theY;
			mDistanceY = theDistanceY;
			mPaperOffset = thePaperOffset;
			mStackSpacing = theStackSpacing;
			mMouseVisible = false;
			mClip = false;
			Resize(theX, theY, theImage.mWidth, theImage.mHeight);
		}

		public override void Dispose()
		{
			mImage.Dispose();
			base.Dispose();
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			g.SetLinearBlend(linear: true);
			g.SetColorizeImages(colorizeImages: true);
			g.SetColor(new SexyColor(255, 255, 255, (int)(255f * (1f - mProgress))));
			g.DrawImage(mImage, 0, 0);
			g.PopState();
		}

		internal bool IsFinished()
		{
			return mTick >= 30;
		}

		internal void UpdateFlight()
		{
			mProgress = TodCommon.TodAnimateCurveFloat(0, 30, ++mTick, 0f, 1f, TodCurves.EaseIn);
			Move(mX, mFromY + (int)Math.Round(mDistanceY * mProgress));
		}

		internal void SetStackOffset(float theOffset)
		{
			Move(mFromX + (int)Math.Round(theOffset + mPaperOffset), mY);
		}

		internal float GetStackSpacing()
		{
			return mStackSpacing * (1f - mProgress);
		}
	}

	private struct MailMotion
	{
		private float mFrom;

		private float mTarget;

		private int mDuration;

		private TodCurves mCurve;

		private int mTicksLeft;

		internal float Position { get; private set; }

		internal bool IsMoving => mTicksLeft != 0;

		internal void Set(float thePosition)
		{
			Position = thePosition;
			mTicksLeft = 0;
		}

		internal void MoveTo(float theTarget, int theDuration, TodCurves theCurve)
		{
			if (!IsMoving || mTarget != theTarget)
			{
				mFrom = Position;
				mTarget = theTarget;
				mDuration = theDuration;
				mCurve = theCurve;
				mTicksLeft = ((Position != theTarget) ? mDuration : 0);
			}
		}

		internal bool Update()
		{
			if (!IsMoving)
			{
				return false;
			}
			Position = TodCommon.TodAnimateCurveFloat(0, mDuration, mDuration - --mTicksLeft, mFrom, mTarget, mCurve);
			return true;
		}
	}

	private sealed class MailRewardWidget : Widget
	{
		private readonly MailScreen mOwner;

		private readonly LocalMail mMail;

		private readonly int mRewardIndex;

		private readonly Coin mCoin;

		private Image mFlashImage;

		private int mFlashCounter;

		private const int FLASH_TICKS = 70;

		internal MailRewardWidget(MailScreen theOwner, LocalMail theMail, int theRewardIndex)
		{
			mOwner = theOwner;
			mMail = theMail;
			mRewardIndex = theRewardIndex;
			mCoin = new Coin
			{
				mType = theMail.mRewards[theRewardIndex].mType,
				mBoard = null
			};
			mClip = false;
		}

		public override void Dispose()
		{
			mCoin.Dispose();
			base.Dispose();
		}

		public override void Update()
		{
			base.Update();
			if (mFlashCounter > 0)
			{
				mFlashCounter--;
				MarkDirty();
			}
		}

		public override void Draw(Graphics g)
		{
			g.PushState();
			g.SetLinearBlend(linear: true);
			TRect theDest = new TRect(0, 0, mWidth, mHeight);
			LawnCommon.DrawImageBox(g, theDest, AtlasResources.IMAGE_ALMANAC_CLAY_TABLET);
			LawnCommon.DrawImageBox(g, theDest, AtlasResources.IMAGE_ALMANAC_BROWN_RECT, theDrawCenter: false);
			if (mFlashCounter > 0 && mFlashCounter % 20 < 10)
			{
				g.SetColorizeImages(colorizeImages: true);
				g.SetColor(new SexyColor(255, 0, 0, 180));
				LawnCommon.DrawImageBox(g, theDest, mFlashImage, theDrawCenter: false);
				g.SetColorizeImages(colorizeImages: false);
			}
			bool flag = IsClaimed();
			mCoin.mPosX = (float)mWidth * Constants.IS / 2f;
			mCoin.mPosY = (float)mHeight * Constants.IS / 2f;
			mCoin.Draw(g, theIcon: true);
			TodCommon.TodDrawString(g, flag ? "[MAIL_REWARD_CLAIMED]" : "[MAIL_REWARD_CLAIM]", mWidth / 2, 88, Resources.FONT_DWARVENTODCRAFT12, new SexyColor(255, 220, 149), DrawStringJustification.Center);
			int mCount = mMail.mRewards[mRewardIndex].mCount;
			if (mCount > 1)
			{
				string text = "×" + mCount;
				int thePosX = mWidth - AtlasResources.IMAGE_ALMANAC_BROWN_RECT.mWidth / 3;
				int thePosY = mHeight - AtlasResources.IMAGE_ALMANAC_BROWN_RECT.mHeight / 3 - Resources.FONT_DWARVENTODCRAFT12.StringHeight(text);
				TodCommon.TodDrawString(g, text, thePosX, thePosY, Resources.FONT_DWARVENTODCRAFT12, SexyColor.White, DrawStringJustification.Right);
			}
			g.PopState();
		}

		public override void MouseUp(int theX, int theY, int theClickCount)
		{
			if (mIsDown && mIsOver && new TRect(0, 0, mWidth, mHeight).Contains(theX, theY))
			{
				Claim();
			}
		}

		internal bool IsClaimed()
		{
			return mMail.mRewards[mRewardIndex].mClaimed;
		}

		internal void Claim()
		{
			if (!mOwner.CanInteract() || IsClaimed())
			{
				return;
			}
			if (!LocalMailbox.ClaimReward(mOwner.mApp.mPlayerInfo, mMail, mRewardIndex, out var thePlants, out var theRequiredSpace))
			{
				mOwner.mApp.PlaySample(Resources.SOUND_BUZZER);
				Flash();
				if (theRequiredSpace > 0)
				{
					mOwner.mAdvice.SetLabel(TodCommon.TodReplaceNumberString("[MAIL_REWARD_GARDEN_FULL]", "{COUNT}", theRequiredSpace), MessageStyle.HintFast);
				}
			}
			else
			{
				Image iconImage = mCoin.GetIconImage();
				CGPoint cGPoint = GetAbsPos() - mOwner.GetAbsPos();
				mOwner.ReleaseReward(mCoin.mType, mMail.mRewards[mRewardIndex].mCount, (int)cGPoint.x + (mWidth - iconImage.mWidth) / 2, (int)cGPoint.y + (mHeight - iconImage.mHeight) / 2, thePlants);
				mFlashCounter = 0;
				MarkDirty();
			}
		}

		internal void Flash()
		{
			if (mFlashImage == null)
			{
				mFlashImage = FilterEffect.FilterEffectGetImage(AtlasResources.IMAGE_ALMANAC_BROWN_RECT, FilterEffectType.White);
			}
			mFlashCounter = 70;
			MarkDirty();
		}
	}

	private sealed class MailTextWidget : Widget
	{
		private readonly MailPaperStyle mStyle;

		private readonly RichTextLayout mLayout;

		private float mScale;

		internal MailTextWidget(string theText, MailPaperStyle theStyle)
		{
			//IL_0026: Expected Obj, but got Unknown
			mStyle = theStyle;
			mMouseVisible = false;
			mClip = false;
			mLayout = new RichTextLayout();
			mLayout.Font = ((StashFont)Resources.FONT_BRIANNETOD12).mFonts[0];
			mLayout.VerticalSpacing = 5;
			mLayout.Text = theText;
		}

		public override void Draw(Graphics g)
		{
			if (mWidth <= 0 || mHeight <= 0 || g.mClipRect.mWidth <= 0 || g.mClipRect.mHeight <= 0)
			{
				return;
			}
			g.HardwareClip();
			try
			{
				RichTextLayout val = mLayout;
				SpriteBatch spriteBatch = Graphics.spriteBatch;
				Vector2 val2 = new Vector2((float)g.mTransX, (float)g.mTransY);
				Color val3 = new Color(mStyle.mInk.mRed, mStyle.mInk.mGreen, mStyle.mInk.mBlue);
				Vector2? val4 = new Vector2(mScale);
				val.Draw(spriteBatch, val2, val3, 0f, default(Vector2), val4, 0f, (TextHorizontalAlignment)0);
			}
			finally
			{
				g.EndHardwareClip();
			}
		}

		internal void LayoutText(int theWidth)
		{
			mScale = FrameworkConstants.Font_Scale;
			mLayout.Width = Math.Max(1, (int)((float)theWidth / mScale));
			Resize(mX, mY, theWidth, (int)Math.Ceiling((float)mLayout.Size.Y * mScale));
		}
	}

	private sealed class MailScrollWidget : ScrollWidget
	{
		private float mScrollFrom;

		private float mScrollTo;

		private int mScrollTick = 30;

		private const int REWARD_SCROLL_TICKS = 30;

		internal MailScrollWidget(Widget theBody)
		{
			mMouseVisible = false;
			AddWidget(theBody);
		}

		public override void Update()
		{
			base.Update();
			if (mScrollTick < 30)
			{
				float y = TodCommon.TodAnimateCurveFloat(0, 30, ++mScrollTick, mScrollFrom, mScrollTo, TodCurves.EaseOut);
				SetScrollOffset(new CGPoint(0f, y), animated: false);
			}
		}

		public override void TouchBegan(_Touch theTouch)
		{
			mScrollTick = 30;
			mScrollVelocity = default;
			base.TouchBegan(theTouch);
		}

		public override void TouchEnded(_Touch theTouch)
		{
			if (!mScrollTracking && !new TRect(0, 0, mWidth, mHeight).Contains((int)theTouch.location.x, (int)theTouch.location.y))
			{
				base.TouchesCanceled();
			}
			else
			{
				base.TouchEnded(theTouch);
			}
		}

		public override void MouseWheel(int theDelta)
		{
			mScrollTick = 30;
			mSeekScrollTarget = false;
			base.MouseWheel(theDelta);
			FlashIndicators();
		}

		internal bool CanScroll()
		{
			return (mScrollPractical & ScrollMode.Vertical) != 0;
		}

		internal void ScrollPixels(float thePixels)
		{
			ScrollToOffset(GetScrollOffset().y - thePixels);
		}

		internal void ScrollToOffset(float theOffset)
		{
			mScrollTick = 30;
			mSeekScrollTarget = false;
			SetScrollOffset(new CGPoint(0f, Math.Clamp(theOffset, Math.Min(0f, mScrollMin.y), mScrollMax.y)), animated: false);
		}

		internal bool HasPressedChild()
		{
			if (mClientLastDown != null)
			{
				return mClientLastDown != mClient;
			}
			return false;
		}

		internal void ScrollToBottom()
		{
			mScrollFrom = GetScrollOffset().y;
			mScrollTo = Math.Min(0f, mScrollMin.y);
			mScrollVelocity = default;
			mSeekScrollTarget = false;
			mScrollTick = 0;
			FlashIndicators();
		}
	}

	private readonly LawnApp mApp;

	private readonly LocalMailboxState mMailbox;

	private readonly MessageWidget mAdvice;

	private readonly HashSet<string> mOwnedGroups = new HashSet<string>();

	private readonly List<Coin> mRewardCoins = new List<Coin>();

	private readonly List<TodParticleSystem> mRewardParticles = new List<TodParticleSystem>();

	private readonly Image mTable;

	private readonly Image mLeaves;

	private readonly LeavesWidget mLeavesWidget;

	private readonly LawnStoneButton mReadButton;

	private readonly LawnStoneButton mSwitchButton;

	private readonly LawnStoneButton mCloseButton;

	private readonly LawnStoneButton mPreviousButton;

	private readonly LawnStoneButton mNextButton;

	private Widget mSheetsHost = new Widget
	{
		mMouseVisible = false,
		mClip = false,
		ClipInput = true
	};

	private List<MailSheetWidget> mSheets = new List<MailSheetWidget>();

	private List<MailFlightWidget> mFlights = new List<MailFlightWidget>();

	private float mPagePosition;

	private float mSlideDistance;

	private MailMotion mOverscroll;

	private Widget mSwitchingHost;

	private List<MailSheetWidget> mSwitchingSheets;

	private List<MailFlightWidget> mSwitchingFlights;

	private int mStackSwitchTick;

	private int mStackSwitchDirection;

	private int mSwitchingIndex;

	private float mSwitchingSlideDistance;

	private MailMotion mSwitchingOverscroll;

	private bool mPointerDown;

	private MailSheetWidget mDraggedSheet;

	private int mPointerX;

	private int mPointerY;

	private int mPointerDirection;

	private int mDraggingTicks;

	private MailScrollWidget mPointerScroll;

	private bool mDisposed;

	private const int SHEET_FLIGHT_TICKS = 30;

	private const int OVERSCROLL_TICKS = 40;

	private const int STACK_SPACING = 15;

	private const int STACK_TOP = 85;

	private const int STACK_SWITCH_DISTANCE = 800;

	private const int STACK_SWITCH_TICKS = 50;

	private const int MAIL_READ_BUTTON = 1;

	private const int MAIL_SWITCH_BUTTON = 2;

	private const int MAIL_CLOSE_BUTTON = 3;

	private const int MAIL_PREVIOUS_BUTTON = 4;

	private const int MAIL_NEXT_BUTTON = 5;

	internal MailScreen(LawnApp theApp)
	{
		mApp = theApp;
		mMailbox = new LocalMailboxState(theApp.mPlayerInfo);
		mAdvice = new MessageWidget(theApp);
		try
		{
			LoadResourceGroup("DelayLoad_Mailbox");
			LoadResourceGroup("DelayLoad_ZombieNote");
			LoadResourceGroup("DelayLoad_GamePlay");
			mTable = mApp.mResourceManager.GetImageThrow("IMAGE_MAILBOX_TABLE");
			mLeaves = mApp.mResourceManager.GetImageThrow("IMAGE_MAILBOX_LEAVES");
			AddWidget(mSheetsHost);
			mLeavesWidget = new LeavesWidget(mLeaves);
			AddWidget(mLeavesWidget);
			mReadButton = CreateButton(1, "[MAIL_MARK_READ]");
			mSwitchButton = CreateButton(2, "[MAIL_GO_READ]");
			mCloseButton = CreateButton(3, "[MAIL_CLOSE]");
			mPreviousButton = CreateButton(4, "[MAIL_PREVIOUS]");
			mNextButton = CreateButton(5, "[MAIL_NEXT]");
			UpdateUIPosition();
			RebuildStack();
			LocalMailbox.MarkSeen(mApp.mPlayerInfo);
		}
		catch
		{
			Dispose();
			throw;
		}
	}

	public override void Dispose()
	{
		if (mDisposed)
		{
			return;
		}
		mDisposed = true;
		mPointerDown = false;
		CancelScroll();
		mAdvice.Dispose();
		foreach (Coin mRewardCoin in mRewardCoins)
		{
			mRewardCoin.Dispose();
		}
		mRewardCoins.Clear();
		foreach (TodParticleSystem mRewardParticle in mRewardParticles)
		{
			mRewardParticle.ParticleSystemDie();
		}
		mRewardParticles.Clear();
		RemoveAllWidgets(doDelete: true, recursive: true);
		mSheets.Clear();
		mFlights.Clear();
		mSwitchingFlights?.Clear();
		mSwitchingFlights = null;
		mSwitchingSheets?.Clear();
		mSwitchingSheets = null;
		mSwitchingHost = null;
		foreach (string mOwnedGroup in mOwnedGroups)
		{
			mApp.mResourceManager.UnloadBackground(mOwnedGroup);
		}
		mOwnedGroups.Clear();
		base.Dispose();
	}

	public override void Update()
	{
		base.Update();
		UpdateUIPosition();
		UpdateSheetAnimations(mSheetsHost, mSheets, mFlights, mSlideDistance, ref mOverscroll);
		if (IsSwitchingStacks())
		{
			UpdateSheetAnimations(mSwitchingHost, mSwitchingSheets, mSwitchingFlights, mSwitchingSlideDistance, ref mSwitchingOverscroll);
		}
		if (mPointerDown && mPointerDirection == 1)
		{
			mDraggingTicks++;
		}
		UpdateMailboxSwitch();
		UpdateRewards();
		mAdvice.Update();
		MarkDirty();
	}

	public override void Draw(Graphics g)
	{
		g.SetLinearBlend(linear: true);
		DrawCover(g, mTable, mWidth, mHeight);
		DeferOverlay();
	}

	public override void DrawOverlay(Graphics g)
	{
		g.PushState();
		TodCommon.TodDrawString(g, mMailbox.mShowingRead ? "[MAIL_READ]" : "[MAIL_INBOX]", mWidth / 2, 43, Resources.FONT_DWARVENTODCRAFT18, SexyColor.White, DrawStringJustification.Center);
		if (!IsSwitchingStacks() && mMailbox.GetCurrentMail() != null)
		{
			int thePosY = mPreviousButton.mY + (mPreviousButton.mHeight + Resources.FONT_DWARVENTODCRAFT12.GetAscent()) / 2 - (int)Constants.InvertAndScale(4f);
			TodCommon.TodDrawString(g, mMailbox.mIndex + 1 + " / " + mMailbox.GetMessages().Count, mWidth / 2, thePosY, Resources.FONT_DWARVENTODCRAFT12, SexyColor.White, DrawStringJustification.Center);
		}
		if (mMailbox.GetCurrentMail() == null && !IsAnimating())
		{
			TodCommon.TodDrawString(g, mMailbox.mShowingRead ? "[MAIL_EMPTY_READ]" : "[MAILBOX_EMPTY]", mWidth / 2, mHeight / 2, Resources.FONT_DWARVENTODCRAFT15, SexyColor.White, mWidth - 100, DrawStringJustification.Center);
		}
		foreach (Coin mRewardCoin in mRewardCoins)
		{
			if (!mRewardCoin.mDead)
			{
				mRewardCoin.Draw(g);
			}
		}
		foreach (TodParticleSystem mRewardParticle in mRewardParticles)
		{
			mRewardParticle.Draw(g, doScale: true);
		}
		g.Translate(-mX, -mY);
		mAdvice.Draw(g);
		g.PopState();
	}

	public override void KeyDown(KeyCode theKey)
	{
		if (mApp.GetDialogCount() == 0)
		{
			switch (theKey)
			{
			case KeyCode.Left:
				SwitchMail(-1);
				break;
			case KeyCode.Right:
				SwitchMail(1);
				break;
			case KeyCode.Return:
			case KeyCode.Space:
				ToggleMailRead();
				break;
			case (KeyCode)88:
				SwitchMailbox();
				break;
			case KeyCode.Up:
				ScrollMail(-60f);
				break;
			case KeyCode.Down:
				ScrollMail(60f);
				break;
			case KeyCode.Prior:
				ScrollMail(-240f);
				break;
			case KeyCode.Next:
				ScrollMail(240f);
				break;
			case KeyCode.Back:
			case KeyCode.Escape:
				BackButtonPress();
				break;
			default:
				base.KeyDown(theKey);
				break;
			}
		}
	}

	public override void MouseDown(int theX, int theY, int theClickCount)
	{
		if (!mDisposed && !IsSwitchingStacks() && !mPointerDown && mApp.GetDialogCount() == 0 && mMailbox.GetCurrentMail() != null && theClickCount >= 0 && theClickCount != 3 && mSheetsHost.GetRect().Contains(theX, theY))
		{
			MailSheetWidget mailSheetWidget = mSheets[mMailbox.mIndex];
			mPointerDown = true;
			mPointerX = theX;
			mPointerY = theY;
			mPointerDirection = 0;
			mDraggingTicks = 0;
			mPointerScroll = mailSheetWidget.GetScrollAt(theX - mSheetsHost.mX - mailSheetWidget.mX, theY - mSheetsHost.mY - mailSheetWidget.mY);
			if (mPointerScroll != null)
			{
				mPointerScroll.mIsDown = true;
				mPointerScroll.TouchBegan(GetScrollTouch(theX, theY));
			}
			UpdateButtons();
		}
	}

	public override void MouseDrag(int theX, int theY)
	{
		if (!mPointerDown)
		{
			return;
		}
		int num = theX - mPointerX;
		int value = theY - mPointerY;
		bool flag = mPointerDirection == 0;
		if (flag && Math.Max(Math.Abs(num), Math.Abs(value)) > 8)
		{
			mPointerDirection = ((Math.Abs(num) > Math.Abs(value)) ? 1 : 2);
			if (mPointerDirection == 1)
			{
				CancelScroll();
			}
		}
		if (mPointerDirection == 1)
		{
			DragMail(num, flag);
		}
		else if (mPointerDirection == 2 && mPointerScroll != null)
		{
			mPointerScroll.TouchMoved(GetScrollTouch(theX, theY));
		}
	}

	public override void MouseUp(int theX, int theY, int theClickCount)
	{
		if (!mPointerDown)
		{
			return;
		}
		MouseDrag(theX, theY);
		mPointerDown = false;
		bool flag = mPointerScroll != null && mPointerScroll.HasPressedChild();
		if (flag && mPointerDirection != 0)
		{
			CancelScroll();
		}
		if (mPointerScroll != null)
		{
			mPointerScroll.mIsDown = false;
			mPointerScroll.TouchEnded(GetScrollTouch(theX, theY));
		}
		if (mPointerDirection == 1)
		{
			bool flag2 = theX < mPointerX;
			int num = (flag2 ? ((int)Math.Floor(mPagePosition)) : ((int)Math.Ceiling(mPagePosition)));
			float num2 = Math.Abs(mPagePosition - (float)num) * mSlideDistance;
			if (num2 >= mSlideDistance / 15f && (num2 * 30f / (float)Math.Max(1, mDraggingTicks) > mSlideDistance / 4f || num2 > mSlideDistance / 3f))
			{
				num += (flag2 ? 1 : (-1));
			}
			if (CanInteract())
			{
				SwitchMail(num - mMailbox.mIndex);
			}
			else
			{
				FinishMailDrag();
			}
		}
		else if ((mPointerDirection == 0) & flag)
		{
			UpdateButtons();
		}
		else if (mPointerDirection == 0 && mSheetsHost.GetRect().Contains(theX, theY))
		{
			CancelScroll();
			SwitchMail((theX >= mWidth / 2) ? 1 : (-1));
		}
		else
		{
			FinishMailDrag();
		}
	}

	public override void TouchesCanceled()
	{
		if (mPointerDown)
		{
			FinishMailDrag();
		}
		else
		{
			CancelScroll();
		}
	}

	public override void MouseWheel(int theDelta)
	{
		if (!CanInteract() || mMailbox.GetCurrentMail() == null)
		{
			return;
		}
		CGPoint absPos = GetAbsPos();
		int num = mWidgetManager.mLastMouseX - (int)absPos.x;
		int num2 = mWidgetManager.mLastMouseY - (int)absPos.y;
		MailSheetWidget mailSheetWidget = mSheets[mMailbox.mIndex];
		if (mFlights.Count == 0 && !mSheets.Exists((MailSheetWidget aMailSheet) => aMailSheet.IsFlying()) && mSheetsHost.GetRect().Contains(num, num2))
		{
			MailScrollWidget scrollAt = mailSheetWidget.GetScrollAt(num - mSheetsHost.mX - mailSheetWidget.mX, num2 - mSheetsHost.mY - mailSheetWidget.mY);
			if (scrollAt != null && scrollAt.CanScroll())
			{
				scrollAt.MouseWheel(theDelta);
				return;
			}
		}
		if (theDelta < -60)
		{
			SwitchMail(1);
		}
		else if (theDelta > 60)
		{
			SwitchMail(-1);
		}
	}

	public override bool BackButtonPress()
	{
		if (!mDisposed && mApp.GetDialogCount() == 0)
		{
			mApp.ShowGameSelectorLeftPage();
		}
		return true;
	}

	public void ButtonPress(int theId)
	{
		mApp.PlaySample(Resources.SOUND_TAP);
	}

	public void ButtonPress(int theId, int theClickCount)
	{
	}

	public void ButtonDepress(int theId)
	{
		switch (theId)
		{
		case 1:
			ToggleMailRead();
			break;
		case 2:
			SwitchMailbox();
			break;
		case 3:
			BackButtonPress();
			break;
		case 4:
			SwitchMail(-1);
			break;
		case 5:
			SwitchMail(1);
			break;
		}
	}

	public void ButtonDownTick(int theId)
	{
	}

	public void ButtonMouseMove(int theId, int theX, int theY)
	{
	}

	public void ButtonMouseLeave(int theId)
	{
	}

	public void ButtonMouseEnter(int theId)
	{
	}

	public void ButtonMouseTick(int theId)
	{
	}

	private bool IsSwitchingStacks()
	{
		return mSwitchingHost != null;
	}

	private bool IsAnimating()
	{
		if (!IsSwitchingStacks() && mFlights.Count <= 0 && !mOverscroll.IsMoving)
		{
			return mSheets.Exists((MailSheetWidget aSheet) => aSheet.IsFlying());
		}
		return true;
	}

	private bool CanInteract()
	{
		if (!mDisposed && !IsSwitchingStacks() && !mPointerDown)
		{
			return mApp.GetDialogCount() == 0;
		}
		return false;
	}

	private void SwitchMail(int theDirection)
	{
		if (CanInteract())
		{
			if (!mMailbox.SwitchMail(theDirection))
			{
				FinishMailDrag();
				return;
			}
			CancelScroll();
			mSheets[mMailbox.mIndex].ScrollToTop();
			SettleMailTurns();
			UpdateButtons();
			mApp.PlayFoley(FoleyType.Paper);
		}
	}

	private void ToggleMailRead()
	{
		if (!CanInteract())
		{
			return;
		}
		int mIndex = mMailbox.mIndex;
		LocalMail currentMail = mMailbox.GetCurrentMail();
		if (!mMailbox.ToggleCurrentRead())
		{
			mApp.PlaySample(Resources.SOUND_BUZZER);
			if (currentMail != null && currentMail.HasUnclaimedRewards())
			{
				mSheets[mIndex].ShowUnclaimedRewards();
			}
			return;
		}
		MailSheetWidget mailSheetWidget = mSheets[mIndex];
		MailFlightWidget mailFlightWidget = mailSheetWidget.CreateFlight(mIndex, mSlideDistance, (float)((mMailbox.mShowingRead ? 1 : (-1)) * mailSheetWidget.mHeight) * 0.7f);
		int num = mFlights.Count;
		while (num > 0 && mFlights[num - 1].mStackIndex > mIndex)
		{
			mFlights[--num].mStackIndex--;
		}
		mFlights.Insert(num, mailFlightWidget);
		mSheetsHost.AddWidget(mailFlightWidget);
		mSheetsHost.PutInfront(mailFlightWidget, mailSheetWidget);
		mSheetsHost.RemoveWidget(mailSheetWidget);
		mSheets.RemoveAt(mIndex);
		mailSheetWidget.Dispose();
		SettleMailTurns();
		if (mMailbox.GetCurrentMail() != null)
		{
			mSheets[mMailbox.mIndex].ScrollToTop();
		}
		UpdateButtons();
		mApp.PlayFoley(FoleyType.Paper);
	}

	private void SwitchMailbox()
	{
		if (mDisposed || mApp.GetDialogCount() != 0)
		{
			return;
		}
		if (IsSwitchingStacks())
		{
			int mIndex = mMailbox.mIndex;
			mMailbox.SwitchMailbox();
			mMailbox.SwitchMail(mSwitchingIndex);
			mSwitchingIndex = mIndex;
			Widget widget = mSheetsHost;
			mSheetsHost = mSwitchingHost;
			mSwitchingHost = widget;
			List<MailSheetWidget> list = mSheets;
			mSheets = mSwitchingSheets;
			mSwitchingSheets = list;
			List<MailFlightWidget> list2 = mFlights;
			mFlights = mSwitchingFlights;
			mSwitchingFlights = list2;
			float num = mSlideDistance;
			mSlideDistance = mSwitchingSlideDistance;
			mSwitchingSlideDistance = num;
			MailMotion mailMotion = mSwitchingOverscroll;
			MailMotion mailMotion2 = mOverscroll;
			mOverscroll = mailMotion;
			mSwitchingOverscroll = mailMotion2;
			mPagePosition = mMailbox.mIndex;
			mStackSwitchDirection = -mStackSwitchDirection;
			mStackSwitchTick = 50 - mStackSwitchTick;
			UpdateButtons();
		}
		else
		{
			if (!CanInteract())
			{
				return;
			}
			mSwitchingIndex = mMailbox.mIndex;
			mMailbox.SwitchMailbox();
			mSwitchingHost = mSheetsHost;
			mSwitchingSheets = mSheets;
			mSwitchingFlights = mFlights;
			mSwitchingSlideDistance = mSlideDistance;
			mSwitchingOverscroll = mOverscroll;
			mStackSwitchTick = 0;
			mStackSwitchDirection = (mMailbox.mShowingRead ? 1 : (-1));
			mSheets = new List<MailSheetWidget>();
			mFlights = new List<MailFlightWidget>();
			mSheetsHost = new Widget();
			mSheetsHost.mMouseVisible = false;
			mSheetsHost.mClip = false;
			mSheetsHost.ClipInput = true;
			mSheetsHost.Resize(0, 85 - mStackSwitchDirection * 800, mWidth, mHeight - 205);
			AddWidget(mSheetsHost);
			BringToBack(mSheetsHost);
			RebuildStack();
		}
		mApp.PlayFoley(FoleyType.Paper);
	}

	private _Touch GetScrollTouch(int theX, int theY)
	{
		CGPoint cGPoint = GetAbsPos() - mPointerScroll.GetAbsPos();
		return new _Touch
		{
			location = new CGPoint((float)theX + cGPoint.x, (float)theY + cGPoint.y),
			timestamp = (double)Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency
		};
	}

	private void CancelScroll()
	{
		if (mPointerScroll != null)
		{
			mPointerScroll.mIsDown = false;
			mPointerScroll.TouchesCanceled();
			mPointerScroll = null;
		}
	}

	private void ScrollMail(float theAmount)
	{
		if (CanInteract() && mMailbox.GetCurrentMail() != null)
		{
			MailSheetWidget mailSheetWidget = mSheets[mMailbox.mIndex];
			if (!mailSheetWidget.IsFlying())
			{
				mailSheetWidget.ScrollBy(theAmount);
			}
		}
	}

	private void ReleaseReward(CoinType theType, int theCount, int theX, int theY, PottedPlant[] thePlants)
	{
		var (num, num2) = theType switch
		{
			CoinType.Diamond => (18, 11), 
			CoinType.Chocolate => (0, 0), 
			CoinType.PresentPlant => (0, 60), 
			_ => (2, -2), 
		};
		if (theType == CoinType.Award10PottedPlants)
		{
			num = (int)((float)(AtlasResources.IMAGE_PRESENT.mWidth - AtlasResources.IMAGE_PRESENTOPEN.GetCelWidth()) * Constants.IS / 2f) + 10;
			num2 = (int)((float)(AtlasResources.IMAGE_PRESENT.mHeight - AtlasResources.IMAGE_PRESENTOPEN.GetCelHeight()) * Constants.IS / 2f) + 50;
		}
		if (theType == CoinType.PresentPlant || theType == CoinType.Award10PottedPlants)
		{
			mApp.DelayLoadZenGardenResources(doLoad: true);
			foreach (PottedPlant pottedPlant in thePlants)
			{
				Plant.PreloadPlantResources((theType == CoinType.PresentPlant) ? SeedType.Sprout : pottedPlant.mSeedType);
			}
		}
		for (int j = 0; j < theCount; j++)
		{
			Coin coin = new Coin
			{
				mBoard = null
			};
			coin.CoinInitialize((int)((float)theX * Constants.IS) + num, (int)((float)theY * Constants.IS) + num2, theType, CoinMotion.FromPresent, (theType == CoinType.PresentPlant) ? thePlants[j] : null);
			float num3 = (float)Math.PI / 2f + (float)Math.PI * (float)(j + 1) / (float)(theCount + 1);
			coin.mVelX = 5f * (float)Math.Sin(num3);
			coin.mVelY = 5f * (float)Math.Cos(num3);
			coin.mScored = true;
			if (theType == CoinType.Award10PottedPlants)
			{
				coin.aTempSeedTypes = new SeedType[10];
				for (int k = 0; k < 10; k++)
				{
					coin.aTempSeedTypes[k] = thePlants[j * 10 + k].mSeedType;
				}
				coin.mCoinAge = 0;
				coin.mIsBeingCollected = true;
				Image iMAGE_PRESENT = AtlasResources.IMAGE_PRESENT;
				float num4 = ((float)theX + (float)iMAGE_PRESENT.GetCelWidth() / 2f) * Constants.IS;
				float num5 = ((float)theY + (float)iMAGE_PRESENT.GetCelHeight() / 2f) * Constants.IS;
				coin.AttachAwardRays(num4 - coin.mPosX, num5 - coin.mPosY);
				TodParticleSystem todParticleSystem = mApp.AddTodParticle(num4, num5, coin.mRenderOrder + 1, ParticleEffect.PresentPickup);
				if (todParticleSystem != null)
				{
					mRewardParticles.Add(todParticleSystem);
				}
			}
			mRewardCoins.Add(coin);
			if (j == 0)
			{
				if (theType == CoinType.Award10PottedPlants)
				{
					mApp.PlayFoley(FoleyType.Finalfanfare);
				}
				else
				{
					coin.PlayCollectSound();
				}
			}
		}
	}

	private void UpdateRewards()
	{
		for (int num = mRewardCoins.Count - 1; num >= 0; num--)
		{
			Coin coin = mRewardCoins[num];
			if (!coin.mIsBeingCollected && coin.mCoinAge >= 79 && (coin.mType == CoinType.Chocolate || coin.mType == CoinType.PresentPlant))
			{
				coin.mIsBeingCollected = true;
				coin.mDisappearCounter = 0;
				coin.mFadeCount = 0;
				float num2 = ((coin.mType == CoinType.PresentPlant) ? Constants.InvertAndScale(30f) : 30f);
				TodParticleSystem todParticleSystem = mApp.AddTodParticle(coin.mPosX + num2, coin.mPosY + num2, coin.mRenderOrder + 1, ParticleEffect.PresentPickup);
				if (todParticleSystem != null)
				{
					mRewardParticles.Add(todParticleSystem);
				}
				if (coin.mType == CoinType.Chocolate)
				{
					coin.StartFade();
				}
			}
			coin.Update();
			if (coin.mDead)
			{
				coin.Dispose();
				mRewardCoins.RemoveAt(num);
			}
		}
		for (int num3 = mRewardParticles.Count - 1; num3 >= 0; num3--)
		{
			TodParticleSystem todParticleSystem2 = mRewardParticles[num3];
			todParticleSystem2.Update();
			if (todParticleSystem2.mDead)
			{
				mRewardParticles.RemoveAt(num3);
			}
		}
	}

	private void DragMail(int theDeltaX, bool theNewDrag)
	{
		float max = mSheets.Count - 1;
		float num = (float)mMailbox.mIndex - (float)theDeltaX / mSlideDistance;
		float num2 = Math.Clamp(num, 0f, max);
		num = num2 + (num - num2) * 0.25f;
		float num3 = Math.Clamp(mPagePosition, 0f, max);
		float num4 = num2 - num - (num3 - mPagePosition);
		if (num4 != 0f)
		{
			mOverscroll.Set(mOverscroll.Position + num4 * mSlideDistance);
		}
		for (int i = 0; i < mSheets.Count - 1; i++)
		{
			MailSheetWidget mailSheetWidget = mSheets[i];
			if (!mailSheetWidget.IsFlying())
			{
				float num5 = ((mailSheetWidget == mDraggedSheet) ? (num - mPagePosition) : (Math.Clamp(num2 - (float)i, 0f, 1f) - Math.Clamp(num3 - (float)i, 0f, 1f)));
				if (num5 != 0f)
				{
					mailSheetWidget.mTurn.Set(Math.Clamp(mailSheetWidget.mTurn.Position + num5, 0f, 1f));
				}
			}
		}
		if (theNewDrag)
		{
			int num6 = mMailbox.mIndex - ((theDeltaX > 0) ? 1 : 0);
			if (num6 >= 0 && num6 < mSheets.Count - 1 && mSheets[num6].IsFlying())
			{
				mDraggedSheet = mSheets[num6];
				mDraggedSheet.mTurn.Set(mDraggedSheet.mTurn.Position);
			}
		}
		mPagePosition = num;
		UpdateSheetPositions();
	}

	private void FinishMailDrag()
	{
		mPointerDown = false;
		CancelScroll();
		SettleMailTurns();
		UpdateButtons();
	}

	private void SettleMailTurns(bool theImmediately = false)
	{
		mDraggedSheet = null;
		mPagePosition = mMailbox.mIndex;
		if (theImmediately)
		{
			mOverscroll.Set(0f);
		}
		else
		{
			mOverscroll.MoveTo(0f, 40, TodCurves.EaseInOut);
		}
		for (int i = 0; i < mSheets.Count; i++)
		{
			float num = ((i < mMailbox.mIndex) ? 1 : 0);
			if (theImmediately)
			{
				mSheets[i].mTurn.Set(num);
			}
			else
			{
				mSheets[i].mTurn.MoveTo(num, 30, (num == 0f) ? TodCurves.EaseOut : TodCurves.EaseIn);
			}
		}
		UpdateSheetPositions();
	}

	private void UpdateUIPosition()
	{
		int mVirtualWidth = mApp.mScreenScales.mVirtualWidth;
		int mVirtualHeight = mApp.mScreenScales.mVirtualHeight;
		if (mVirtualWidth == mWidth && mVirtualHeight == mHeight)
		{
			return;
		}
		mPointerDown = false;
		CancelScroll();
		FinishMailAnimations();
		Resize(-(mVirtualWidth - Constants.BOARD_WIDTH) / 2, -(mVirtualHeight - Constants.BOARD_HEIGHT) / 2, mVirtualWidth, mVirtualHeight);
		mSheetsHost.Resize(0, 85, mVirtualWidth, mVirtualHeight - 205);
		mLeavesWidget.Resize(0, 0, mVirtualWidth, mVirtualHeight);
		mReadButton.Resize((mVirtualWidth - 220) / 2, mVirtualHeight - 105, 220, mReadButton.mHeight);
		mSwitchButton.Resize(20, mVirtualHeight - 55, 240, mSwitchButton.mHeight);
		mCloseButton.Resize(mVirtualWidth - 210, mVirtualHeight - 55, 190, mCloseButton.mHeight);
		mPreviousButton.Resize(mVirtualWidth / 2 - 200, mVirtualHeight - 55, 115, mPreviousButton.mHeight);
		mNextButton.Resize(mVirtualWidth / 2 + 85, mVirtualHeight - 55, 115, mNextButton.mHeight);
		mSlideDistance = 1f;
		foreach (MailSheetWidget mSheet in mSheets)
		{
			mSheet.LayoutPaper(mSheetsHost.mWidth, mSheetsHost.mHeight);
			mSlideDistance = Math.Max(mSlideDistance, mSheet.GetExitDistance());
		}
		UpdateSheetPositions();
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		bool flag = !IsSwitchingStacks();
		LocalMail currentMail = mMailbox.GetCurrentMail();
		mReadButton.SetVisible(flag && currentMail != null);
		LawnStoneButton lawnStoneButton = mReadButton;
		string label;
		if (mMailbox.mShowingRead)
		{
			label = ((currentMail != null && currentMail.mDeleteOnlyAfterRead) ? "[DELETE_BUTTON]" : "[MAIL_MARK_UNREAD]");
		}
		else
		{
			label = "[MAIL_MARK_READ]";
		}
		lawnStoneButton.SetLabel(label);
		mSwitchButton.SetVisible(isVisible: true);
		mPreviousButton.SetVisible(flag && mMailbox.mIndex > 0);
		mNextButton.SetVisible(flag && mMailbox.mIndex + 1 < mMailbox.GetMessages().Count);
		mReadButton.SetDisabled(!CanInteract());
		mSwitchButton.SetDisabled(!IsSwitchingStacks() && !CanInteract());
		mSwitchButton.SetLabel(mMailbox.mShowingRead ? "[MAIL_GO_INBOX]" : "[MAIL_GO_READ]");
		mPreviousButton.SetDisabled(!CanInteract());
		mNextButton.SetDisabled(!CanInteract());
	}

	private void RebuildStack()
	{
		mPointerDown = false;
		CancelScroll();
		mWidgetManager?.SetFocus(this);
		mSheetsHost.RemoveAllWidgets(doDelete: true, recursive: true);
		mSheets.Clear();
		mFlights.Clear();
		mSlideDistance = 1f;
		foreach (LocalMail message in mMailbox.GetMessages())
		{
			MailSheetWidget mailSheetWidget = new MailSheetWidget(this, message);
			mailSheetWidget.LayoutPaper(mSheetsHost.mWidth, mSheetsHost.mHeight);
			mSlideDistance = Math.Max(mSlideDistance, mailSheetWidget.GetExitDistance());
			mSheets.Add(mailSheetWidget);
		}
		for (int num = mSheets.Count - 1; num >= 0; num--)
		{
			mSheetsHost.AddWidget(mSheets[num]);
		}
		SettleMailTurns(theImmediately: true);
		UpdateButtons();
	}

	private void UpdateSheetPositions()
	{
		UpdateSheetPositions(mSheets, mFlights, mSlideDistance, mOverscroll.Position);
	}

	private static void UpdateSheetPositions(List<MailSheetWidget> theSheets, List<MailFlightWidget> theFlights, float theSlideDistance, float theOverscroll)
	{
		int num = 0;
		float num2 = theOverscroll;
		bool flag = false;
		for (int i = 0; i <= theSheets.Count; i++)
		{
			while (num < theFlights.Count && theFlights[num].mStackIndex == i)
			{
				MailFlightWidget mailFlightWidget = theFlights[num++];
				mailFlightWidget.SetStackOffset(num2);
				num2 += mailFlightWidget.GetStackSpacing();
			}
			if (i < theSheets.Count)
			{
				MailSheetWidget mailSheetWidget = theSheets[i];
				float position = mailSheetWidget.mTurn.Position;
				mailSheetWidget.SetPaperOffset(num2 - theSlideDistance * position, !flag || mailSheetWidget.IsFlying() || (position > 0f && position < 1f));
				flag |= position == 0f;
				num2 += 15f * (1f - position);
			}
		}
	}

	private static void UpdateSheetAnimations(Widget theHost, List<MailSheetWidget> theSheets, List<MailFlightWidget> theFlights, float theSlideDistance, ref MailMotion theOverscroll)
	{
		bool flag = theOverscroll.Update() || theFlights.Count != 0;
		foreach (MailSheetWidget theSheet in theSheets)
		{
			flag |= theSheet.mTurn.Update();
		}
		for (int num = theFlights.Count - 1; num >= 0; num--)
		{
			MailFlightWidget mailFlightWidget = theFlights[num];
			mailFlightWidget.UpdateFlight();
			if (mailFlightWidget.IsFinished())
			{
				theHost.RemoveWidget(mailFlightWidget);
				theFlights.RemoveAt(num);
				mailFlightWidget.Dispose();
			}
		}
		if (flag)
		{
			UpdateSheetPositions(theSheets, theFlights, theSlideDistance, theOverscroll.Position);
		}
	}

	private void UpdateMailboxSwitch()
	{
		if (IsSwitchingStacks())
		{
			int num = mStackSwitchDirection * 800;
			int num2 = (int)Math.Round(TodCommon.TodAnimateCurveFloat(0, 50, ++mStackSwitchTick, 0f, num, TodCurves.EaseInOut));
			mSwitchingHost.Move(0, 85 + num2);
			mSheetsHost.Move(0, 85 + num2 - num);
			if (mStackSwitchTick >= 50)
			{
				FinishMailboxSwitch();
				UpdateButtons();
			}
		}
	}

	private void FinishMailboxSwitch()
	{
		if (IsSwitchingStacks())
		{
			RemoveWidget(mSwitchingHost);
			mSwitchingHost.RemoveAllWidgets(doDelete: true, recursive: true);
			mSwitchingHost.Dispose();
			mSwitchingHost = null;
			mSwitchingSheets = null;
			mSwitchingFlights = null;
			mSheetsHost.Move(0, 85);
		}
	}

	private void FinishMailAnimations()
	{
		FinishMailboxSwitch();
		foreach (MailFlightWidget mFlight in mFlights)
		{
			mSheetsHost.RemoveWidget(mFlight);
			mFlight.Dispose();
		}
		mFlights.Clear();
		SettleMailTurns(theImmediately: true);
	}

	private void LoadResourceGroup(string theGroup)
	{
		if (!string.IsNullOrEmpty(theGroup) && !mApp.mResourceManager.IsGroupLoaded(theGroup))
		{
			mOwnedGroups.Add(theGroup);
			if (!TodCommon.TodLoadResources(theGroup))
			{
				throw new InvalidOperationException("Could not load mailbox resources: " + theGroup);
			}
		}
	}

	private LawnStoneButton CreateButton(int theId, string theLabel)
	{
		LawnStoneButton lawnStoneButton = GameButton.MakeButton(theId, this, theLabel);
		AddWidget(lawnStoneButton);
		return lawnStoneButton;
	}

	private static void DrawCover(Graphics g, Image theImage, int theWidth, int theHeight)
	{
		float num = Math.Max((float)theWidth / (float)theImage.mWidth, (float)theHeight / (float)theImage.mHeight);
		int num2 = (int)Math.Ceiling((float)theImage.mWidth * num);
		int num3 = (int)Math.Ceiling((float)theImage.mHeight * num);
		g.DrawImage(theImage, (theWidth - num2) / 2, (theHeight - num3) / 2, num2, num3);
	}
}
