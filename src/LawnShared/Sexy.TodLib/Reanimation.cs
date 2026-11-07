using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spine;

namespace Sexy.TodLib;

public class Reanimation
{
	public delegate bool ReanimLoopAction(Reanimation self);

	public const string Attacher = "attacher__";

	public static bool mInterpolate = true;

	public ReanimationType mReanimationType;

	private bool mSpineDoAnimTimeUpdate;

	public float _animTime;

	public float mAnimRate;

	public ReanimatorDefinition mDefinition;

	private bool mSpineDoLoopTypeUpdate;

	private ReanimLoopType _loopType;

	public bool mDead;

	public short mFrameStart;

	public short mFrameCount;

	public short mFrameBasePose;

	public SexyTransform2D mOverlayMatrix;

	public SexyColor mColorOverride;

	public ReanimatorTrackInstance[] mTrackInstances = new ReanimatorTrackInstance[200];

	private int _loopCount;

	public ReanimationHolder mReanimationHolder;

	public bool mIsAttachment;

	public int mRenderOrder;

	public SexyColor mExtraAdditiveColor;

	public bool mEnableExtraAdditiveDraw;

	public SexyColor mExtraOverlayColor;

	public bool mEnableExtraOverlayDraw;

	public float mLastFrameTime;

	public FilterEffectType mFilterEffect;

	public bool mClip;

	public bool mActive;

	public bool mGetFrameTime = true;

	public ReanimatorFrameTime mFrameTime;

	public SexyTransform2D aOverlayMatrix;

	public ReanimLoopAction mAction;

	public static string ReanimTrackId_fullscreen = ReanimatorXnaHelpers.ReanimatorTrackNameToId("fullscreen");

	public static string ReanimTrackId__ground = ReanimatorXnaHelpers.ReanimatorTrackNameToId("_ground");

	public static string ReanimTrackId_anim_walk = ReanimatorXnaHelpers.ReanimatorTrackNameToId("anim_walk");

	public static string ReanimTrackId_anim_crawl = ReanimatorXnaHelpers.ReanimatorTrackNameToId("anim_crawl");

	public static string ReanimTrackIdEmpty = ReanimatorXnaHelpers.ReanimatorTrackNameToId("");

	private static Matrix tempMatrix;

	private static Dictionary<string, string> lowercaseCache = new Dictionary<string, string>();

	private static bool didClipIgnore = false;

	public Matrix aBasePoseMatrix;

	public Matrix tempOverlayMatrix;

	public SexyTransform2D basePose;

	private static readonly SexyTransform2D identity = new SexyTransform2D(loadIdentity: true);

	public bool mIsSpine;

	public bool mIsSpine_Save;

	public Skeleton mSpineSkeleton;

	public AnimationState mSpineAnimationState;

	public string mSpineCurrentTrackName;

	public int mSpineBlendTime;

	public int mSpineAnimTrackCount;

	private float mAccumulatedTime;

	private bool mNeedsRenderUpdate;

	public static Dictionary<FilterEffectType, SkeletonRenderer> mSpineSkeletonRenderer = new Dictionary<FilterEffectType, SkeletonRenderer>(4);

	private const string ANIM_PREFIX = "anim_";

	public float mAnimTime
	{
		get
		{
			return _animTime;
		}
		set
		{
			if (mIsSpine)
			{
				mSpineDoAnimTimeUpdate = true;
			}
			_animTime = value;
		}
	}

	public ref float mAnimTime_Save => ref _animTime;

	public ReanimLoopType mLoopType
	{
		get
		{
			return _loopType;
		}
		set
		{
			if (mIsSpine)
			{
				mSpineDoLoopTypeUpdate = true;
			}
			_loopType = value;
		}
	}

	public ref ReanimLoopType mLoopType_Save => ref _loopType;

	public int mLoopCount
	{
		get
		{
			return _loopCount;
		}
		set
		{
			if (mIsSpine)
			{
				mSpineDoAnimTimeUpdate = true;
			}
			_loopCount = value;
		}
	}

	public ref int mLoopCount_Save => ref _loopCount;

	public override string ToString()
	{
		return mReanimationType.ToString();
	}

	public static string ToLower(string s)
	{
		if (!lowercaseCache.TryGetValue(s, out var value))
		{
			value = s.ToLower();
			lowercaseCache.Add(s, value);
		}
		return value;
	}

	public static void PreallocateMemory()
	{
	}

	public static Reanimation GetNewReanimation()
	{
		return new Reanimation();
	}

	public void PrepareForReuse()
	{
		Reset();
	}

	protected void Reset()
	{
		for (int i = 0; i < mTrackInstances.Length; i++)
		{
			if (mTrackInstances[i] != null)
			{
				mTrackInstances[i].PrepareForReuse();
			}
			mTrackInstances[i] = null;
		}
		mClip = false;
		mAnimTime = 0f;
		mAnimRate = 12f;
		mLastFrameTime = -1f;
		mDefinition = null;
		mLoopType = ReanimLoopType.PlayOnce;
		mDead = false;
		mFrameStart = 0;
		mFrameCount = 0;
		mFrameBasePose = -1;
		mOverlayMatrix.LoadIdentity();
		mColorOverride = new SexyColor(Color.White);
		mExtraAdditiveColor = new SexyColor(Color.White);
		mEnableExtraAdditiveDraw = false;
		mExtraOverlayColor = new SexyColor(Color.White);
		mEnableExtraOverlayDraw = false;
		mLoopCount = 0;
		mIsAttachment = false;
		mRenderOrder = 0;
		mReanimationHolder = null;
		mFilterEffect = FilterEffectType.None;
		mReanimationType = ReanimationType.None;
		mActive = false;
		mGetFrameTime = true;
		if (mIsSpine)
		{
			mSpineAnimationState = null;
			mSpineSkeleton = null;
			mIsSpine = false;
			mSpineDoAnimTimeUpdate = false;
			mSpineDoLoopTypeUpdate = false;
			mSpineAnimTrackCount = 0;
			mSpineBlendTime = 0;
			mNeedsRenderUpdate = false;
			mAccumulatedTime = 0f;
			mSpineCurrentTrackName = null;
		}
	}

	private Reanimation()
	{
		Reset();
	}

	public void ReanimationInitialize(float theX, float theY, int theDefinition)
	{
		mDefinition = ReanimatorXnaHelpers.gReanimatorDefArray[theDefinition];
		if (mDefinition.mIsSpine)
		{
			ReanimationSpineInitialize(theX, theY);
			return;
		}
		mDead = false;
		SetPosition(theX, theY);
		mAnimRate = mDefinition.mFPS;
		mLastFrameTime = -1f;
		if (mDefinition.mTrackCount != 0)
		{
			mFrameCount = mDefinition.mTracks[0].mTransformCount;
			for (int i = 0; i < mDefinition.mTrackCount; i++)
			{
				ReanimatorTrackInstance newReanimatorTrackInstance = ReanimatorTrackInstance.GetNewReanimatorTrackInstance();
				mTrackInstances[i] = newReanimatorTrackInstance;
			}
		}
		else
		{
			mFrameCount = 0;
		}
	}

	public void ReanimationInitializeType(float theX, float theY, ReanimationType theReanimType)
	{
		ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(theReanimType, theIsPreloading: false);
		mReanimationType = theReanimType;
		ReanimationInitialize(theX, theY, (int)theReanimType);
	}

	public void ReanimationDie()
	{
		if (mDead)
		{
			return;
		}
		mActive = false;
		mDead = true;
		if (!mIsSpine)
		{
			for (int i = 0; i < mDefinition.mTrackCount; i++)
			{
				ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
				GlobalMembersAttachment.AttachmentDie(ref reanimatorTrackInstance.mAttachmentID);
			}
		}
	}

	public void Update()
	{
		mGetFrameTime = true;
		if (mIsSpine)
		{
			UpdateSpine();
		}
		else
		{
			if (mFrameCount == 0 || mDead)
			{
				return;
			}
			mLastFrameTime = mAnimTime;
			mAnimTime += ReanimatorXnaHelpers.SECONDS_PER_UPDATE * mAnimRate / (float)mFrameCount;
			if (mAnimRate > 0f)
			{
				if (mLoopType != ReanimLoopType.Loop && mLoopType != ReanimLoopType.LoopFullLastFrame)
				{
					if (mLoopType == ReanimLoopType.PlayOnce || mLoopType == ReanimLoopType.PlayOnceFullLastFrame)
					{
						if (mAnimTime >= 1f)
						{
							mAnimTime = 1f;
							mLoopCount = 1;
							mDead = true;
						}
					}
					else if (mLoopType == ReanimLoopType.PlayThenAction && mAnimTime >= 1f)
					{
						Debug.ASSERT(mAction != null);
						mAnimTime = 1f;
						mLoopCount = 1;
						ReanimLoopAction reanimLoopAction = mAction;
						mDead = reanimLoopAction != null && !reanimLoopAction(this);
					}
					else if ((mLoopType == ReanimLoopType.PlayOnceAndHold || mLoopType == ReanimLoopType.PlayOnceFullLastFrameAndHold) && mAnimTime >= 1f)
					{
						mLoopCount = 1;
						mAnimTime = 1f;
					}
				}
				else
				{
					while (mAnimTime >= 1f)
					{
						mLoopCount++;
						mAnimTime--;
					}
				}
			}
			else if (mLoopType != ReanimLoopType.Loop && mLoopType != ReanimLoopType.LoopFullLastFrame)
			{
				if (mLoopType == ReanimLoopType.PlayOnce || mLoopType == ReanimLoopType.PlayOnceFullLastFrame)
				{
					if (mAnimTime < 0f)
					{
						mAnimTime = 0f;
						mLoopCount = 1;
						mDead = true;
					}
				}
				else if ((mLoopType == ReanimLoopType.PlayOnceAndHold || mLoopType == ReanimLoopType.PlayOnceFullLastFrameAndHold) && mAnimTime < 0f)
				{
					mLoopCount = 1;
					mAnimTime = 0f;
				}
			}
			else
			{
				while (mAnimTime < 0f)
				{
					mLoopCount++;
					mAnimTime++;
				}
			}
			int mTrackCount = mDefinition.mTrackCount;
			for (int i = 0; i < mTrackCount; i++)
			{
				ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
				if (reanimatorTrackInstance.mRenderGroup != ReanimatorXnaHelpers.RENDER_GROUP_HIDDEN)
				{
					if (reanimatorTrackInstance.mBlendCounter > 0)
					{
						ReanimatorTrackInstance reanimatorTrackInstance2 = reanimatorTrackInstance;
						reanimatorTrackInstance2.mBlendCounter--;
					}
					if (reanimatorTrackInstance.mShakeOverride != 0f)
					{
						reanimatorTrackInstance.mShakeX = TodCommon.RandRangeFloat(0f - reanimatorTrackInstance.mShakeOverride, reanimatorTrackInstance.mShakeOverride);
						reanimatorTrackInstance.mShakeY = TodCommon.RandRangeFloat(0f - reanimatorTrackInstance.mShakeOverride, reanimatorTrackInstance.mShakeOverride);
					}
					ReanimatorTrack reanimatorTrack = mDefinition.mTracks[i];
					if (reanimatorTrack.IsAttacher)
					{
						UpdateAttacherTrack(i);
					}
					if (reanimatorTrackInstance.mAttachmentID != null)
					{
						GetAttachmentOverlayMatrix(i, out aOverlayMatrix);
						GlobalMembersAttachment.AttachmentUpdateAndSetMatrix(ref reanimatorTrackInstance.mAttachmentID, ref aOverlayMatrix);
					}
				}
			}
		}
	}

	public void Draw(Graphics g, bool isHardwareClipRequired = true)
	{
		mGetFrameTime = true;
		if (mIsSpine)
		{
			DrawSpineGroup(g, ReanimatorXnaHelpers.RENDER_GROUP_NORMAL);
		}
		else
		{
			DrawRenderGroup(g, ReanimatorXnaHelpers.RENDER_GROUP_NORMAL, isHardwareClipRequired);
		}
	}

	public void DrawRenderGroup(Graphics g, int theRenderGroup, bool isHardwareClipRequired = true)
	{
		if (mDead)
		{
			return;
		}
		if (mIsSpine)
		{
			DrawSpineGroup(g, theRenderGroup);
			return;
		}
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
			if (reanimatorTrackInstance.mRenderGroup != theRenderGroup)
			{
				continue;
			}
			bool flag = DrawTrack(g, i, theRenderGroup, isHardwareClipRequired);
			if (reanimatorTrackInstance.mAttachmentID == null)
			{
				continue;
			}
			Attachment mAttachmentID = reanimatorTrackInstance.mAttachmentID;
			for (int j = 0; j < mAttachmentID.mNumEffects; j++)
			{
				AttachEffect attachEffect = mAttachmentID.mEffectArray[j];
				if (attachEffect.mEffectType == EffectType.Reanim)
				{
					Reanimation reanimation = (Reanimation)attachEffect.mEffectID;
					reanimation.mColorOverride = mColorOverride;
					reanimation.mExtraAdditiveColor = mExtraAdditiveColor;
					reanimation.mExtraOverlayColor = mExtraOverlayColor;
				}
			}
			GlobalMembersAttachment.AttachmentDraw(reanimatorTrackInstance.mAttachmentID, g, !flag, doScale: false);
		}
	}

	private bool DrawTrack(Graphics g, int theTrackIndex, int theRenderGroup, bool isHardwareClipRequired)
	{
		if (mIsSpine)
		{
			DrawSpineGroup(g, ReanimatorXnaHelpers.RENDER_GROUP_NORMAL);
			return true;
		}
		GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: true);
		if (aTransformCurrent == null)
		{
			return false;
		}
		if (aTransformCurrent.mFrame < 0f)
		{
			aTransformCurrent.PrepareForReuse();
			return false;
		}
		int num = (int)(aTransformCurrent.mFrame + 0.5f);
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[theTrackIndex];
		SexyColor mTrackColor = reanimatorTrackInstance.mTrackColor;
		if (!reanimatorTrackInstance.mIgnoreColorOverride)
		{
			mTrackColor.Color.R = (byte)((float)(mColorOverride.mRed * mTrackColor.mRed) / 255f);
			mTrackColor.Color.G = (byte)((float)(mColorOverride.mGreen * mTrackColor.mGreen) / 255f);
			mTrackColor.Color.B = (byte)((float)(mColorOverride.mBlue * mTrackColor.mBlue) / 255f);
			mTrackColor.Color.A = (byte)((float)(mColorOverride.mAlpha * mTrackColor.mAlpha) / 255f);
		}
		if (g.mColorizeImages)
		{
			mTrackColor.Color.R = (byte)((float)(g.mColor.R * mTrackColor.mRed) / 255f);
			mTrackColor.Color.G = (byte)((float)(g.mColor.G * mTrackColor.mGreen) / 255f);
			mTrackColor.Color.B = (byte)((float)(g.mColor.B * mTrackColor.mBlue) / 255f);
			mTrackColor.Color.A = (byte)((float)(g.mColor.A * mTrackColor.mAlpha) / 255f);
		}
		int num2 = TodCommon.ClampInt((int)(aTransformCurrent.mAlpha * (float)mTrackColor.mAlpha + 0.5f), 0, 255);
		if (num2 <= 0)
		{
			aTransformCurrent.PrepareForReuse();
			return false;
		}
		mTrackColor.mAlpha = num2;
		SexyColor theColor = ((!mEnableExtraAdditiveDraw) ? default(SexyColor) : new SexyColor(mExtraAdditiveColor.mRed, mExtraAdditiveColor.mGreen, mExtraAdditiveColor.mBlue, TodCommon.ColorComponentMultiply(mExtraAdditiveColor.mAlpha, num2)));
		Image image = aTransformCurrent.mImage;
		ReanimAtlasImage reanimAtlasImage = null;
		if (mDefinition.mReanimAtlas != null && image != null)
		{
			reanimAtlasImage = mDefinition.mReanimAtlas.GetEncodedReanimAtlas(image);
			if (reanimAtlasImage != null)
			{
				image = reanimAtlasImage.mOriginalImage;
			}
			if (reanimatorTrackInstance.mImageOverride != null)
			{
				reanimAtlasImage = null;
			}
		}
		bool flag = false;
		float num3 = 0f;
		float num4 = 0f;
		if (image != null)
		{
			float num5 = image.GetCelWidth();
			float num6 = image.GetCelHeight();
			num3 = num5 * 0.5f;
			num4 = num6 * 0.5f;
		}
		else if (aTransformCurrent.mFont != null && !string.IsNullOrEmpty(aTransformCurrent.mText))
		{
			float num7 = aTransformCurrent.mFont.StringWidth(aTransformCurrent.mText);
			num3 = (0f - num7) * 0.5f;
			num4 = aTransformCurrent.mFont.mAscent;
		}
		else
		{
			if (!(mDefinition.mTracks[theTrackIndex].mName == "fullscreen"))
			{
				aTransformCurrent.PrepareForReuse();
				return false;
			}
			flag = true;
		}
		TRect theClipRect = g.mClipRect;
		didClipIgnore = false;
		if (reanimatorTrackInstance.mIgnoreClipRect)
		{
			theClipRect = new TRect(0, 0, 1000, 600);
			didClipIgnore = true;
		}
		float num8 = aTransformCurrent.mSkewXCos * aTransformCurrent.mScaleX;
		float num9 = (0f - aTransformCurrent.mSkewXSin) * aTransformCurrent.mScaleX;
		float num10 = aTransformCurrent.mSkewYSin * aTransformCurrent.mScaleY;
		float num11 = aTransformCurrent.mSkewYCos * aTransformCurrent.mScaleY;
		float num12 = num8 * num3 + num10 * num4 + aTransformCurrent.mTransX;
		float num13 = num9 * num3 + num11 * num4 + aTransformCurrent.mTransY;
		tempMatrix = new Matrix
		{
			M11 = num8 * mOverlayMatrix.mMatrix.M11 + num9 * mOverlayMatrix.mMatrix.M21,
			M12 = num8 * mOverlayMatrix.mMatrix.M12 + num9 * mOverlayMatrix.mMatrix.M22,
			M13 = 0f,
			M14 = 0f,
			M21 = num10 * mOverlayMatrix.mMatrix.M11 + num11 * mOverlayMatrix.mMatrix.M21,
			M22 = num10 * mOverlayMatrix.mMatrix.M12 + num11 * mOverlayMatrix.mMatrix.M22,
			M23 = 0f,
			M24 = 0f,
			M31 = 0f,
			M32 = 0f,
			M33 = 1f,
			M34 = 0f,
			M41 = num12 * mOverlayMatrix.mMatrix.M11 + num13 * mOverlayMatrix.mMatrix.M21 + mOverlayMatrix.mMatrix.M41 + (float)g.mTransX + reanimatorTrackInstance.mShakeX - 0.5f,
			M42 = num12 * mOverlayMatrix.mMatrix.M12 + num13 * mOverlayMatrix.mMatrix.M22 + mOverlayMatrix.mMatrix.M42 + (float)g.mTransY + reanimatorTrackInstance.mShakeY - 0.5f,
			M43 = 0f,
			M44 = 1f
		};
		if (theTrackIndex == 9)
		{
			int num14 = 0;
			num14++;
		}
		if (reanimAtlasImage == null)
		{
			if (image != null)
			{
				if (reanimatorTrackInstance.mImageOverride != null)
				{
					image = reanimatorTrackInstance.mImageOverride;
				}
				if (mFilterEffect == FilterEffectType.White)
				{
					image = FilterEffect.FilterEffectGetImage(image, mFilterEffect);
				}
				if (mFilterEffect == FilterEffectType.WashedOut || mFilterEffect == FilterEffectType.LessWashedOut)
				{
					image = FilterEffect.FilterEffectGetImage(image, FilterEffectType.WashedOut);
				}
				while (num >= image.mNumCols)
				{
					num -= image.mNumCols;
				}
				int num15 = 0;
				int celWidth = image.GetCelWidth();
				int celHeight = image.GetCelHeight();
				TRect theSrcRect = new TRect(celWidth * num, celHeight * num15, celWidth, celHeight);
				ReanimBltMatrix(g, image, ref tempMatrix, ref theClipRect, mTrackColor, Graphics.DrawMode.DRAWMODE_NORMAL, theSrcRect, isHardwareClipRequired);
				if (mEnableExtraAdditiveDraw)
				{
					ReanimBltMatrix(g, image, ref tempMatrix, ref theClipRect, theColor, Graphics.DrawMode.DRAWMODE_ADDITIVE, theSrcRect, isHardwareClipRequired);
				}
				TodCommon.OffsetForGraphicsTranslation = true;
			}
			else if (aTransformCurrent.mFont != null && !string.IsNullOrEmpty(aTransformCurrent.mText))
			{
				TodCommon.TodDrawStringMatrix(g, aTransformCurrent.mFont, tempMatrix, aTransformCurrent.mText, mTrackColor);
				if (mEnableExtraAdditiveDraw)
				{
					Graphics.DrawMode mDrawMode = g.mDrawMode;
					g.SetDrawMode(Graphics.DrawMode.DRAWMODE_ADDITIVE);
					TodCommon.TodDrawStringMatrix(g, aTransformCurrent.mFont, tempMatrix, aTransformCurrent.mText, theColor);
					g.SetDrawMode(mDrawMode);
				}
			}
			else if (flag)
			{
				Color color = g.GetColor();
				g.SetColor(mTrackColor);
				g.FillRect(-g.mTransX, -g.mTransY, Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT_GAME);
				g.SetColor(color);
			}
		}
		aTransformCurrent.PrepareForReuse();
		return true;
	}

	public void GetCurrentTransform(int theTrackIndex, out ReanimatorTransform aTransformCurrent, bool nullIfInvalidFrame)
	{
		GetFrameTime(out var theFrameTime);
		GetTransformAtTime(theTrackIndex, out aTransformCurrent, theFrameTime, nullIfInvalidFrame);
		if (aTransformCurrent == null)
		{
			return;
		}
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[theTrackIndex];
		int num = (int)(aTransformCurrent.mFrame + 0.5f);
		if (num >= 0 && reanimatorTrackInstance.mBlendCounter > 0)
		{
			float theBlendFactor = (float)(int)reanimatorTrackInstance.mBlendCounter / (float)(int)reanimatorTrackInstance.mBlendTime;
			ReanimatorXnaHelpers.BlendTransform(out var theResult, ref aTransformCurrent, ref reanimatorTrackInstance.mBlendTransform, theBlendFactor);
			if (aTransformCurrent != null)
			{
				aTransformCurrent.PrepareForReuse();
			}
			aTransformCurrent = theResult;
		}
	}

	public void GetTransformAtTime(int theTrackIndex, out ReanimatorTransform aTransform, ReanimatorFrameTime theFrameTime, bool nullIfInvalidFrame)
	{
		if (mIsSpine)
		{
			GetTransformAtTimeSpine(theTrackIndex, out aTransform, theFrameTime, nullIfInvalidFrame);
			return;
		}
		ReanimatorTrack reanimatorTrack = mDefinition.mTracks[theTrackIndex];
		ReanimatorTransform reanimatorTransform = reanimatorTrack.mTransforms[theFrameTime.mAnimFrameBeforeInt];
		ReanimatorTransform reanimatorTransform2 = reanimatorTrack.mTransforms[theFrameTime.mAnimFrameAfterInt];
		if (nullIfInvalidFrame && (reanimatorTransform.mFrame == -1f || (reanimatorTransform.mFrame != -1f && reanimatorTransform2.mFrame == -1f && theFrameTime.mFraction > 0f && mTrackInstances[theTrackIndex].mTruncateDisappearingFrames)))
		{
			aTransform = null;
			return;
		}
		float mFraction = theFrameTime.mFraction;
		aTransform = ReanimatorTransform.GetNewReanimatorTransform();
		if (mInterpolate)
		{
			aTransform.mTransX = reanimatorTransform.mTransX + mFraction * (reanimatorTransform2.mTransX - reanimatorTransform.mTransX);
			aTransform.mTransY = reanimatorTransform.mTransY + mFraction * (reanimatorTransform2.mTransY - reanimatorTransform.mTransY);
			aTransform.mSkewX = reanimatorTransform.mSkewX + mFraction * (reanimatorTransform2.mSkewX - reanimatorTransform.mSkewX);
			aTransform.mSkewY = reanimatorTransform.mSkewY + mFraction * (reanimatorTransform2.mSkewY - reanimatorTransform.mSkewY);
			aTransform.mScaleX = reanimatorTransform.mScaleX + mFraction * (reanimatorTransform2.mScaleX - reanimatorTransform.mScaleX);
			aTransform.mScaleY = reanimatorTransform.mScaleY + mFraction * (reanimatorTransform2.mScaleY - reanimatorTransform.mScaleY);
			aTransform.mAlpha = reanimatorTransform.mAlpha + mFraction * (reanimatorTransform2.mAlpha - reanimatorTransform.mAlpha);
			aTransform.mSkewXCos = reanimatorTransform.mSkewXCos + mFraction * (reanimatorTransform2.mSkewXCos - reanimatorTransform.mSkewXCos);
			aTransform.mSkewXSin = reanimatorTransform.mSkewXSin + mFraction * (reanimatorTransform2.mSkewXSin - reanimatorTransform.mSkewXSin);
			aTransform.mSkewYCos = reanimatorTransform.mSkewYCos + mFraction * (reanimatorTransform2.mSkewYCos - reanimatorTransform.mSkewYCos);
			aTransform.mSkewYSin = reanimatorTransform.mSkewYSin + mFraction * (reanimatorTransform2.mSkewYSin - reanimatorTransform.mSkewYSin);
		}
		else
		{
			aTransform.mTransX = reanimatorTransform.mTransX;
			aTransform.mTransY = reanimatorTransform.mTransY;
			aTransform.mSkewX = reanimatorTransform.mSkewX;
			aTransform.mSkewY = reanimatorTransform.mSkewY;
			aTransform.mScaleX = reanimatorTransform.mScaleX;
			aTransform.mScaleY = reanimatorTransform.mScaleY;
			aTransform.mAlpha = reanimatorTransform.mAlpha;
			aTransform.mSkewXCos = reanimatorTransform.mSkewXCos;
			aTransform.mSkewXSin = reanimatorTransform.mSkewXSin;
			aTransform.mSkewYCos = reanimatorTransform.mSkewYCos;
			aTransform.mSkewYSin = reanimatorTransform.mSkewYSin;
		}
		aTransform.mImage = reanimatorTransform.mImage;
		aTransform.mFont = reanimatorTransform.mFont;
		aTransform.mText = reanimatorTransform.mText;
		if (reanimatorTransform.mFrame != -1f && reanimatorTransform2.mFrame == -1f && theFrameTime.mFraction > 0f && mTrackInstances[theTrackIndex].mTruncateDisappearingFrames)
		{
			aTransform.mFrame = -1f;
		}
		else
		{
			aTransform.mFrame = reanimatorTransform.mFrame;
		}
	}

	public void GetFrameTime(out ReanimatorFrameTime theFrameTime)
	{
		if (!mGetFrameTime)
		{
			theFrameTime = mFrameTime;
			return;
		}
		mGetFrameTime = false;
		theFrameTime = default;
		int num = ((mLoopType != ReanimLoopType.PlayOnceFullLastFrame && mLoopType != ReanimLoopType.LoopFullLastFrame && mLoopType != ReanimLoopType.PlayOnceFullLastFrameAndHold) ? (mFrameCount - 1) : mFrameCount);
		float num2 = (float)mFrameStart + (float)num * mAnimTime;
		float num3 = (int)num2;
		theFrameTime.mFraction = num2 - num3;
		theFrameTime.mAnimFrameBeforeInt = (short)(num3 + 0.5f);
		if (theFrameTime.mAnimFrameBeforeInt >= mFrameStart + mFrameCount - 1)
		{
			theFrameTime.mAnimFrameBeforeInt = (short)(mFrameStart + mFrameCount - 1);
			theFrameTime.mAnimFrameAfterInt = theFrameTime.mAnimFrameBeforeInt;
		}
		else
		{
			theFrameTime.mAnimFrameAfterInt = (short)(theFrameTime.mAnimFrameBeforeInt + 1);
		}
		mFrameTime = theFrameTime;
	}

	public int FindTrackIndex(string theTrackName)
	{
		if (mIsSpine)
		{
			return SpineFindTrackIndex(theTrackName);
		}
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			string mName = mDefinition.mTracks[i].mName;
			string text = ToLower(theTrackName);
			if (mName == text)
			{
				return i;
			}
		}
		return 0;
	}

	public AttachEffect AttachToAnotherReanimation(ref Reanimation theAttachReanim, string theTrackName)
	{
		if (mIsSpine)
		{
			return AttachToAnotherReanimationSpine(ref theAttachReanim, theTrackName);
		}
		if (theAttachReanim.mDefinition.mTrackCount == 0)
		{
			return null;
		}
		if (theAttachReanim.mFrameBasePose == -1)
		{
			theAttachReanim.mFrameBasePose = theAttachReanim.mFrameStart;
		}
		int num = theAttachReanim.FindTrackIndex(theTrackName);
		ReanimatorTrackInstance reanimatorTrackInstance = theAttachReanim.mTrackInstances[num];
		return GlobalMembersAttachment.AttachReanim(ref reanimatorTrackInstance.mAttachmentID, this, 0f, 0f);
	}

	public void GetAttachmentOverlayMatrix(int theTrackIndex, out SexyTransform2D theOverlayMatrix)
	{
		GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
		float num = aTransformCurrent.mSkewXCos * aTransformCurrent.mScaleX;
		float num2 = (0f - aTransformCurrent.mSkewXSin) * aTransformCurrent.mScaleX;
		float num3 = aTransformCurrent.mSkewYSin * aTransformCurrent.mScaleY;
		float num4 = aTransformCurrent.mSkewYCos * aTransformCurrent.mScaleY;
		float mTransX = aTransformCurrent.mTransX;
		float mTransY = aTransformCurrent.mTransY;
		aTransformCurrent.PrepareForReuse();
		GetTrackBasePoseMatrix(theTrackIndex, out basePose);
		Matrix.Invert(ref basePose.mMatrix, out aBasePoseMatrix);
		theOverlayMatrix = identity;
		tempOverlayMatrix = new Matrix
		{
			M11 = aBasePoseMatrix.M11 * num + aBasePoseMatrix.M12 * num3,
			M12 = aBasePoseMatrix.M11 * num2 + aBasePoseMatrix.M12 * num4,
			M13 = 0f,
			M14 = 0f,
			M21 = aBasePoseMatrix.M21 * num + aBasePoseMatrix.M22 * num3,
			M22 = aBasePoseMatrix.M21 * num2 + aBasePoseMatrix.M22 * num4,
			M23 = 0f,
			M24 = 0f,
			M31 = 0f,
			M32 = 0f,
			M33 = 1f,
			M34 = 0f,
			M41 = aBasePoseMatrix.M41 * num + aBasePoseMatrix.M42 * num3 + mTransX,
			M42 = aBasePoseMatrix.M41 * num2 + aBasePoseMatrix.M42 * num4 + mTransY,
			M43 = 0f,
			M44 = 1f
		};
		theOverlayMatrix.mMatrix = new Matrix
		{
			M11 = tempOverlayMatrix.M11 * mOverlayMatrix.mMatrix.M11 + tempOverlayMatrix.M12 * mOverlayMatrix.mMatrix.M21,
			M12 = tempOverlayMatrix.M11 * mOverlayMatrix.mMatrix.M12 + tempOverlayMatrix.M12 * mOverlayMatrix.mMatrix.M22,
			M13 = 0f,
			M14 = 0f,
			M21 = tempOverlayMatrix.M21 * mOverlayMatrix.mMatrix.M11 + tempOverlayMatrix.M22 * mOverlayMatrix.mMatrix.M21,
			M22 = tempOverlayMatrix.M21 * mOverlayMatrix.mMatrix.M12 + tempOverlayMatrix.M22 * mOverlayMatrix.mMatrix.M22,
			M23 = 0f,
			M24 = 0f,
			M31 = 0f,
			M32 = 0f,
			M33 = 1f,
			M34 = 0f,
			M41 = tempOverlayMatrix.M41 * mOverlayMatrix.mMatrix.M11 + tempOverlayMatrix.M42 * mOverlayMatrix.mMatrix.M21 + mOverlayMatrix.mMatrix.M41,
			M42 = tempOverlayMatrix.M41 * mOverlayMatrix.mMatrix.M12 + tempOverlayMatrix.M42 * mOverlayMatrix.mMatrix.M22 + mOverlayMatrix.mMatrix.M42,
			M43 = 0f,
			M44 = 1f
		};
	}

	public void SetFramesForLayer(string theTrackName)
	{
		if (mIsSpine)
		{
			SpineSetFramesForLayer(theTrackName);
			return;
		}
		if (mAnimRate >= 0f)
		{
			mAnimTime = 0f;
		}
		else
		{
			mAnimTime = 0.9999999f;
		}
		mLastFrameTime = -1f;
		GetFramesForLayer(theTrackName, out mFrameStart, out mFrameCount);
	}

	public static void MatrixFromTransform(ReanimatorTransform theTransform, out Matrix theMatrix)
	{
		theMatrix = new Matrix
		{
			M11 = (float)Math.Cos((double)theTransform.mSkewX * (0.0 - (double)TodCommon.DEG_TO_RAD)) * theTransform.mScaleX,
			M12 = (0f - (float)Math.Sin((double)theTransform.mSkewX * (0.0 - (double)TodCommon.DEG_TO_RAD))) * theTransform.mScaleX,
			M13 = 0f,
			M14 = 0f,
			M21 = (float)Math.Sin((double)theTransform.mSkewY * (0.0 - (double)TodCommon.DEG_TO_RAD)) * theTransform.mScaleY,
			M22 = (float)Math.Cos((double)theTransform.mSkewY * (0.0 - (double)TodCommon.DEG_TO_RAD)) * theTransform.mScaleY,
			M23 = 0f,
			M24 = 0f,
			M31 = 0f,
			M32 = 0f,
			M33 = 1f,
			M34 = 0f,
			M41 = theTransform.mTransX,
			M42 = theTransform.mTransY,
			M43 = 0f,
			M44 = 1f
		};
	}

	public bool TrackExists(string theTrackName)
	{
		if (mIsSpine)
		{
			return SpineTrackExists(theTrackName);
		}
		string text = ToLower(theTrackName);
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			string text2 = ToLower(mDefinition.mTracks[i].mName);
			if (text == text2)
			{
				return true;
			}
		}
		return false;
	}

	public void StartBlend(byte theBlendTime)
	{
		if (mReanimationType == ReanimationType.Garlic)
		{
			return;
		}
		if (mIsSpine)
		{
			SpineStartBlend(theBlendTime);
			return;
		}
		mGetFrameTime = true;
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			GetCurrentTransform(i, out var aTransformCurrent, nullIfInvalidFrame: true);
			if (aTransformCurrent == null)
			{
				continue;
			}
			int num = TodCommon.FloatRoundToInt(aTransformCurrent.mFrame);
			if (num < 0)
			{
				aTransformCurrent.PrepareForReuse();
				continue;
			}
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
			if (reanimatorTrackInstance.mBlendTransform != null)
			{
				reanimatorTrackInstance.mBlendTransform.PrepareForReuse();
			}
			reanimatorTrackInstance.mBlendTransform = aTransformCurrent;
			reanimatorTrackInstance.mBlendCounter = (byte)((float)(int)theBlendTime / 3f);
			reanimatorTrackInstance.mBlendTime = (byte)((float)(int)theBlendTime / 3f);
			reanimatorTrackInstance.mBlendTransform.mFont = null;
			reanimatorTrackInstance.mBlendTransform.mText = string.Empty;
			reanimatorTrackInstance.mBlendTransform.mImage = null;
		}
	}

	public void SetShakeOverride(string theTrackName, float theShakeAmount)
	{
		int num = FindTrackIndex(theTrackName);
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[num];
		reanimatorTrackInstance.mShakeOverride = theShakeAmount;
	}

	public void SetPosition(float theX, float theY)
	{
		mOverlayMatrix.mMatrix.Translation = new Vector3(theX, theY, 0f);
	}

	public void OverrideScale(float theScaleX, float theScaleY)
	{
		mOverlayMatrix.mMatrix.M11 = theScaleX;
		mOverlayMatrix.mMatrix.M22 = theScaleY;
	}

	public int GetTrackIndex(string theTrackName)
	{
		return FindTrackIndex(theTrackName);
	}

	public float GetTrackVelocity(string theTrackName)
	{
		return GetTrackVelocity(GetTrackIndex(theTrackName));
	}

	public float GetTrackVelocity(int aTrackIndex)
	{
		GetFrameTime(out var theFrameTime);
		ReanimatorTrack reanimatorTrack = mDefinition.mTracks[aTrackIndex];
		ReanimatorTransform reanimatorTransform = reanimatorTrack.mTransforms[theFrameTime.mAnimFrameBeforeInt];
		ReanimatorTransform reanimatorTransform2 = reanimatorTrack.mTransforms[theFrameTime.mAnimFrameAfterInt];
		return (reanimatorTransform2.mTransX - reanimatorTransform.mTransX) * ReanimatorXnaHelpers.SECONDS_PER_UPDATE * mAnimRate;
	}

	public void SetImageOverride(string theTrackName, Image theImage)
	{
		int num = FindTrackIndex(theTrackName);
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[num];
		reanimatorTrackInstance.mImageOverride = theImage;
	}

	public Image GetImageOverride(string theTrackName)
	{
		int num = FindTrackIndex(theTrackName);
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[num];
		return reanimatorTrackInstance.mImageOverride;
	}

	public void ShowOnlyTrack(string theTrackName)
	{
		string text = theTrackName.ToLower();
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			ReanimatorTrack reanimatorTrack = mDefinition.mTracks[i];
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
			string text2 = reanimatorTrack.mName.ToLower();
			if (text == text2)
			{
				reanimatorTrackInstance.mRenderGroup = ReanimatorXnaHelpers.RENDER_GROUP_NORMAL;
			}
			else
			{
				reanimatorTrackInstance.mRenderGroup = ReanimatorXnaHelpers.RENDER_GROUP_HIDDEN;
			}
		}
	}

	public void GetTrackMatrix(int theTrackIndex, ref SexyTransform2D theMatrix)
	{
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[theTrackIndex];
		mGetFrameTime = true;
		GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
		int num = TodCommon.FloatRoundToInt(aTransformCurrent.mFrame);
		Image image = aTransformCurrent.mImage;
		if (mDefinition.mReanimAtlas != null && image != null)
		{
			ReanimAtlasImage encodedReanimAtlas = mDefinition.mReanimAtlas.GetEncodedReanimAtlas(image);
			if (encodedReanimAtlas != null)
			{
				image = encodedReanimAtlas.mOriginalImage;
			}
		}
		theMatrix.LoadIdentity();
		tempMatrix = Matrix.Identity;
		if (image != null && num >= 0)
		{
			int celWidth = image.GetCelWidth();
			int celHeight = image.GetCelHeight();
			Matrix.CreateTranslation((float)celWidth * 0.5f, (float)celHeight * 0.5f, 0f, out tempMatrix);
		}
		else if (aTransformCurrent.mFont != null && !string.IsNullOrEmpty(aTransformCurrent.mText))
		{
			Matrix.CreateTranslation(0f, aTransformCurrent.mFont.mAscent, 0f, out tempMatrix);
		}
		SexyTransform2D sexyTransform2D = default;
		MatrixFromTransform(aTransformCurrent, out sexyTransform2D.mMatrix);
		TodCommon.SexyMatrix3Multiply(ref tempMatrix, sexyTransform2D.mMatrix, tempMatrix);
		TodCommon.SexyMatrix3Multiply(ref tempMatrix, mOverlayMatrix.mMatrix, tempMatrix);
		TodCommon.SexyMatrix3Translation(ref tempMatrix, reanimatorTrackInstance.mShakeX - 0.5f, reanimatorTrackInstance.mShakeY - 0.5f);
		theMatrix.mMatrix = tempMatrix;
		aTransformCurrent.PrepareForReuse();
	}

	public void AssignRenderGroupToTrack(string theTrackName, int theRenderGroup)
	{
		if (mIsSpine)
		{
			return;
		}
		string text = ToLower(theTrackName);
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			ReanimatorTrack reanimatorTrack = mDefinition.mTracks[i];
			string text2 = ToLower(reanimatorTrack.mName);
			if (!(text != text2))
			{
				mTrackInstances[i].mRenderGroup = theRenderGroup;
				break;
			}
		}
	}

	public void AssignRenderGroupToPrefix(string theTrackName, int theRenderGroup)
	{
		if (mIsSpine)
		{
			return;
		}
		int length = theTrackName.Length;
		string s = ToLower(theTrackName);
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			ReanimatorTrack reanimatorTrack = mDefinition.mTracks[i];
			if (reanimatorTrack.mName.Length >= length)
			{
				string contains = ToLower(reanimatorTrack.mName);
				if (s.StartsWithCharLimit(contains, length))
				{
					mTrackInstances[i].mRenderGroup = theRenderGroup;
				}
			}
		}
	}

	public void PropogateColorToAttachments()
	{
		if (mIsSpine)
		{
			PropogateColorToAttachmentsSpine();
			return;
		}
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
			GlobalMembersAttachment.AttachmentPropogateColor(reanimatorTrackInstance.mAttachmentID, mColorOverride, mEnableExtraAdditiveDraw, mExtraAdditiveColor, mEnableExtraOverlayDraw, mExtraOverlayColor);
		}
	}

	public bool ShouldTriggerTimedEvent(float theEventTime)
	{
		if (mFrameCount == 0 && !mIsSpine)
		{
			return false;
		}
		if (mLastFrameTime < 0f)
		{
			return false;
		}
		if (mAnimRate <= 0f)
		{
			return false;
		}
		if (mAnimTime >= mLastFrameTime)
		{
			if (theEventTime >= mLastFrameTime && theEventTime < mAnimTime)
			{
				return true;
			}
		}
		else if (theEventTime >= mLastFrameTime || theEventTime < mAnimTime)
		{
			return true;
		}
		return false;
	}

	public void TodTriangleGroupDraw(Graphics g, ref TodTriangleGroup theTriangleGroup)
	{
	}

	public Image GetCurrentTrackImage(string theTrackName)
	{
		int theTrackIndex = FindTrackIndex(theTrackName);
		GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
		Image image = aTransformCurrent.mImage;
		if (mDefinition.mReanimAtlas != null && image != null)
		{
			ReanimAtlasImage encodedReanimAtlas = mDefinition.mReanimAtlas.GetEncodedReanimAtlas(image);
			if (encodedReanimAtlas != null)
			{
				image = encodedReanimAtlas.mOriginalImage;
			}
		}
		aTransformCurrent.PrepareForReuse();
		return image;
	}

	public AttachEffect AttachParticleToTrack(string theTrackName, ref TodParticleSystem theParticleSystem, float thePosX, float thePosY)
	{
		int num = FindTrackIndex(theTrackName);
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[num];
		GetTrackBasePoseMatrix(num, out var theBasePoseMatrix);
		Vector2 vector = theBasePoseMatrix * new Vector2(thePosX, thePosY);
		return GlobalMembersAttachment.AttachParticle(ref reanimatorTrackInstance.mAttachmentID, theParticleSystem, vector.X, vector.Y);
	}

	public void GetTrackBasePoseMatrix(int theTrackIndex, out SexyTransform2D theBasePoseMatrix)
	{
		theBasePoseMatrix = default;
		if (mFrameBasePose == ReanimatorXnaHelpers.NO_BASE_POSE)
		{
			theBasePoseMatrix.LoadIdentity();
			return;
		}
		short num = mFrameBasePose;
		if (num == -1)
		{
			num = mFrameStart;
		}
		GetTransformAtTime(theTrackIndex, out var aTransform, new ReanimatorFrameTime
		{
			mFraction = 0f,
			mAnimFrameBeforeInt = num,
			mAnimFrameAfterInt = (short)(num + 1)
		}, nullIfInvalidFrame: false);
		MatrixFromTransform(aTransform, out theBasePoseMatrix.mMatrix);
		aTransform.PrepareForReuse();
	}

	public bool IsTrackShowing(string theTrackName)
	{
		if (mIsSpine)
		{
			return SpineIsTrackShowing(theTrackName);
		}
		GetFrameTime(out var theFrameTime);
		int num = FindTrackIndex(theTrackName);
		ReanimatorTrack reanimatorTrack = mDefinition.mTracks[num];
		ReanimatorTransform reanimatorTransform = reanimatorTrack.mTransforms[theFrameTime.mAnimFrameAfterInt];
		return reanimatorTransform.mFrame >= 0f;
	}

	public void SetTruncateDisappearingFrames(string theTrackName, bool theTruncateDisappearingFrames)
	{
		if (mIsSpine)
		{
			return;
		}
		if (string.IsNullOrEmpty(theTrackName))
		{
			for (int i = 0; i < mDefinition.mTrackCount; i++)
			{
				ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
				reanimatorTrackInstance.mTruncateDisappearingFrames = theTruncateDisappearingFrames;
			}
		}
		else
		{
			int num = FindTrackIndex(theTrackName);
			ReanimatorTrackInstance reanimatorTrackInstance2 = mTrackInstances[num];
			reanimatorTrackInstance2.mTruncateDisappearingFrames = theTruncateDisappearingFrames;
		}
	}

	public void PlayReanim(string theTrackName, ReanimLoopType theLoopType, byte theBlendTime, float theAnimRate)
	{
		if (theBlendTime > 0)
		{
			StartBlend(theBlendTime);
		}
		if (theAnimRate != 0f)
		{
			mAnimRate = theAnimRate;
		}
		mLoopType = theLoopType;
		mLoopCount = 0;
		SetFramesForLayer(theTrackName);
	}

	public void ReanimationDelete()
	{
		if (mTrackInstances != null)
		{
			for (int i = 0; i < mTrackInstances.Length; i++)
			{
				mTrackInstances[i] = null;
			}
			mTrackInstances = null;
		}
		if (mIsSpine)
		{
			mSpineAnimationState = null;
			mSpineSkeleton = null;
		}
	}

	public ReanimatorTrackInstance GetTrackInstanceByName(string theTrackName)
	{
		if (mIsSpine)
		{
			return mTrackInstances[FindTrackIndex(theTrackName)];
		}
		int num = FindTrackIndex(theTrackName);
		return mTrackInstances[num];
	}

	public void GetFramesForLayer(string theTrackName, out short theFrameStart, out short theFrameCount)
	{
		if (mDefinition.mTrackCount == 0 || mIsSpine)
		{
			theFrameStart = 0;
			theFrameCount = 0;
			return;
		}
		int num = FindTrackIndex(theTrackName);
		ReanimatorTrack reanimatorTrack = mDefinition.mTracks[num];
		theFrameStart = 0;
		theFrameCount = 1;
		short num2;
		for (num2 = 0; num2 < reanimatorTrack.mTransformCount; num2++)
		{
			ReanimatorTransform reanimatorTransform = reanimatorTrack.mTransforms[num2];
			if (reanimatorTransform.mFrame >= 0f)
			{
				theFrameStart = num2;
				break;
			}
		}
		for (int num3 = reanimatorTrack.mTransformCount - 1; num3 >= num2; num3--)
		{
			ReanimatorTransform reanimatorTransform2 = reanimatorTrack.mTransforms[num3];
			if (reanimatorTransform2.mFrame >= 0f)
			{
				theFrameCount = (short)(num3 - theFrameStart + 1);
				break;
			}
		}
	}

	public void UpdateAttacherTrack(int theTrackIndex)
	{
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[theTrackIndex];
		GetCurrentTransform(theTrackIndex, out var aTransformCurrent, nullIfInvalidFrame: false);
		ParseAttacherTrack(aTransformCurrent, out var theAttacherInfo);
		ReanimationType reanimationType = ReanimationType.None;
		if (theAttacherInfo.mReanimName.Length != 0)
		{
			string text = string.Format("reanim\\%s.reanim", theAttacherInfo.mReanimName);
			string text2 = text.ToLower();
			for (int i = 0; i < ReanimatorXnaHelpers.gReanimationParamArraySize; i++)
			{
				ReanimationParams reanimationParams = ReanimatorXnaHelpers.gReanimationParamArray[i];
				string text3 = reanimationParams.mReanimFileName.ToLower();
				if (text2 == text3)
				{
					reanimationType = reanimationParams.mReanimationType;
					break;
				}
			}
		}
		if (reanimationType == ReanimationType.None)
		{
			GlobalMembersAttachment.AttachmentDie(ref reanimatorTrackInstance.mAttachmentID);
			return;
		}
		Reanimation theAttachReanim = GlobalMembersAttachment.FindReanimAttachment(reanimatorTrackInstance.mAttachmentID);
		if (theAttachReanim == null || theAttachReanim.mReanimationType != reanimationType)
		{
			GlobalMembersAttachment.AttachmentDie(ref reanimatorTrackInstance.mAttachmentID);
			theAttachReanim = EffectSystem.gEffectSystem.mReanimationHolder.AllocReanimation(0f, 0f, 0, reanimationType);
			theAttachReanim.mLoopType = theAttacherInfo.mLoopType;
			theAttachReanim.mAnimRate = theAttacherInfo.mAnimRate;
			GlobalMembersAttachment.AttachReanim(ref reanimatorTrackInstance.mAttachmentID, theAttachReanim, 0f, 0f);
			mFrameBasePose = ReanimatorXnaHelpers.NO_BASE_POSE;
		}
		if (theAttacherInfo.mTrackName.Length != 0)
		{
			theAttachReanim.GetFramesForLayer(theAttacherInfo.mTrackName, out var theFrameStart, out var theFrameCount);
			if (theAttachReanim.mFrameStart != theFrameStart || theAttachReanim.mFrameCount != theFrameCount)
			{
				theAttachReanim.StartBlend(20);
				theAttachReanim.SetFramesForLayer(theAttacherInfo.mTrackName);
			}
			if (theAttacherInfo.mAnimRate == 12f && theAttacherInfo.mTrackName == "anim_walk" && theAttachReanim.TrackExists("_ground"))
			{
				AttacherSynchWalkSpeed(theTrackIndex, ref theAttachReanim, theAttacherInfo);
			}
			else
			{
				theAttachReanim.mAnimRate = theAttacherInfo.mAnimRate;
			}
			theAttachReanim.mLoopType = theAttacherInfo.mLoopType;
		}
		SexyColor theColor = TodCommon.ColorsMultiply(mColorOverride, reanimatorTrackInstance.mTrackColor);
		theColor.mAlpha = TodCommon.ClampInt(TodCommon.FloatRoundToInt(aTransformCurrent.mAlpha * (float)theColor.mAlpha), 0, 255);
		GlobalMembersAttachment.AttachmentPropogateColor(reanimatorTrackInstance.mAttachmentID, theColor, mEnableExtraAdditiveDraw, mExtraAdditiveColor, mEnableExtraOverlayDraw, mExtraOverlayColor);
	}

	public static void ParseAttacherTrack(ReanimatorTransform theTransform, out AttacherInfo theAttacherInfo)
	{
		theAttacherInfo = new AttacherInfo();
		theAttacherInfo.mReanimName = "";
		theAttacherInfo.mTrackName = "";
		theAttacherInfo.mAnimRate = 12f;
		theAttacherInfo.mLoopType = ReanimLoopType.Loop;
		if (theTransform.mFrame == -1f)
		{
			return;
		}
		int num = theTransform.mText.IndexOf("__");
		if (num == -1)
		{
			return;
		}
		int num2 = theTransform.mText.IndexOf("[", num + 2);
		int num3 = theTransform.mText.IndexOf("__", num + 2);
		if (num2 != -1 && num3 != -1 && num2 < num3)
		{
			return;
		}
		if (num3 != -1)
		{
			theAttacherInfo.mReanimName = theTransform.mText.Substring(num + 2, num3 - num - 2);
			if (num2 != -1)
			{
				theAttacherInfo.mTrackName = theTransform.mText.Substring(num3 + 2, num2 - num3 - 2);
			}
			else
			{
				theAttacherInfo.mTrackName = theTransform.mText.Substring(num3 + 2);
			}
		}
		else if (num2 != -1)
		{
			theAttacherInfo.mReanimName = theTransform.mText.Substring(num + 2, num2 - num - 2);
		}
		else
		{
			theAttacherInfo.mReanimName = theTransform.mText.Substring(num + 2);
		}
		while (num2 != -1)
		{
			int num4 = theTransform.mText.IndexOf("]", num2 + 1);
			if (num4 == -1)
			{
				break;
			}
			string text = theTransform.mText.Substring(num2 + 1, num4 - num2 - 1);
			if (float.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
			{
				theAttacherInfo.mAnimRate = result;
			}
			else if (text == "hold")
			{
				theAttacherInfo.mLoopType = ReanimLoopType.PlayOnceAndHold;
			}
			else if (text == "once")
			{
				theAttacherInfo.mLoopType = ReanimLoopType.PlayOnce;
			}
			num2 = theTransform.mText.IndexOf("[", num4 + 1);
		}
	}

	public void AttacherSynchWalkSpeed(int theTrackIndex, ref Reanimation theAttachReanim, AttacherInfo theAttacherInfo)
	{
		ReanimatorTrack reanimatorTrack = mDefinition.mTracks[theTrackIndex];
		GetFrameTime(out var theFrameTime);
		int num = theFrameTime.mAnimFrameBeforeInt;
		while (num > mFrameStart && !(reanimatorTrack.mTransforms[num - 1].mText != reanimatorTrack.mTransforms[num].mText))
		{
			num--;
		}
		int i;
		for (i = theFrameTime.mAnimFrameBeforeInt; i < mFrameStart + mFrameCount - 1 && !(reanimatorTrack.mTransforms[i + 1].mText != reanimatorTrack.mTransforms[i].mText); i++)
		{
		}
		int num2 = i - num;
		ReanimatorTransform reanimatorTransform = reanimatorTrack.mTransforms[num];
		ReanimatorTransform reanimatorTransform2 = reanimatorTrack.mTransforms[num + num2 - 1];
		if (TodCommon.FloatApproxEqual(mAnimRate, 0f))
		{
			theAttachReanim.mAnimRate = 0f;
			return;
		}
		float num3 = 0f - (reanimatorTransform2.mTransX - reanimatorTransform.mTransX);
		float num4 = (float)num2 / mAnimRate;
		if (TodCommon.FloatApproxEqual(num4, 0f))
		{
			theAttachReanim.mAnimRate = 0f;
			return;
		}
		int num5 = theAttachReanim.FindTrackIndex("_ground");
		ReanimatorTrack reanimatorTrack2 = theAttachReanim.mDefinition.mTracks[num5];
		ReanimatorTransform reanimatorTransform3 = reanimatorTrack2.mTransforms[theAttachReanim.mFrameStart];
		ReanimatorTransform reanimatorTransform4 = reanimatorTrack2.mTransforms[theAttachReanim.mFrameStart + theAttachReanim.mFrameCount - 1];
		float num6 = reanimatorTransform4.mTransX - reanimatorTransform3.mTransX;
		if (num6 < ReanimatorXnaHelpers.EPSILON || num3 < ReanimatorXnaHelpers.EPSILON)
		{
			theAttachReanim.mAnimRate = 0f;
			return;
		}
		float num7 = num3 / num6;
		theAttachReanim.GetCurrentTransform(num5, out var aTransformCurrent, nullIfInvalidFrame: false);
		float num8 = aTransformCurrent.mTransX - reanimatorTransform3.mTransX;
		float num9 = num6 * theAttachReanim.mAnimTime;
		ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[theTrackIndex];
		AttachEffect attachEffect = GlobalMembersAttachment.FindFirstAttachment(reanimatorTrackInstance.mAttachmentID);
		if (attachEffect != null)
		{
			attachEffect.mOffset.mMatrix.M13 = num9 - num8;
		}
		theAttachReanim.mAnimRate = num7 * (float)theAttachReanim.mFrameCount / num4;
	}

	public bool IsAnimPlaying(string theTrackName)
	{
		if (mIsSpine)
		{
			return SpineIsAnimPlaying(theTrackName);
		}
		GetFramesForLayer(theTrackName, out var theFrameStart, out var theFrameCount);
		if (mFrameStart == theFrameStart)
		{
			return mFrameCount == theFrameCount;
		}
		return false;
	}

	public void SetBasePoseFromAnim(string theTrackName)
	{
		GetFramesForLayer(theTrackName, out var theFrameStart, out var _);
		mFrameBasePose = theFrameStart;
	}

	public void ReanimBltMatrix(Graphics g, Image theImage, ref Matrix theTransform, ref TRect theClipRect, SexyColor theColor, Graphics.DrawMode theDrawMode, TRect theSrcRect, bool isHardwareClipRequired)
	{
		ReanimationParams reanimationParams = ReanimatorXnaHelpers.gReanimationParamArray[(int)mReanimationType];
		if (!GlobalStaticVars.gSexyAppBase.Is3DAccelerated() && TodCommon.TestBit((uint)reanimationParams.mReanimParamFlags, 1) && TodCommon.FloatApproxEqual(theTransform.M12, 0f) && TodCommon.FloatApproxEqual(theTransform.M21, 0f) && theTransform.M11 > 0f && theTransform.M22 > 0f && theColor == SexyColor.White)
		{
			float m = theTransform.M11;
			float m2 = theTransform.M22;
			int theX = TodCommon.FloatRoundToInt(theTransform.M41 - m * (float)theSrcRect.mWidth * 0.5f);
			int theY = TodCommon.FloatRoundToInt(theTransform.M42 - m2 * (float)theSrcRect.mHeight * 0.5f);
			Graphics.DrawMode drawMode = g.GetDrawMode();
			g.SetDrawMode(theDrawMode);
			TRect theRect = g.mClipRect;
			g.SetClipRect(ref theClipRect);
			if (TodCommon.FloatApproxEqual(m, 1f) && TodCommon.FloatApproxEqual(m2, 1f))
			{
				g.DrawImage(theImage, theX, theY, theSrcRect);
			}
			else
			{
				int theWidth = TodCommon.FloatRoundToInt(m * (float)theSrcRect.mWidth);
				int theHeight = TodCommon.FloatRoundToInt(m2 * (float)theSrcRect.mHeight);
				TRect theDestRect = new TRect(theX, theY, theWidth, theHeight);
				g.DrawImage(theImage, theDestRect, theSrcRect);
			}
			g.SetDrawMode(drawMode);
			g.SetClipRect(ref theRect);
		}
		else
		{
			TodCommon.TodBltMatrix(g, theImage, ref theTransform, theClipRect, theColor, theDrawMode, theSrcRect, mClip);
		}
	}

	public Reanimation FindSubReanim(ReanimationType theReanimType)
	{
		if (mReanimationType == theReanimType)
		{
			return this;
		}
		if (mIsSpine)
		{
			return null;
		}
		for (int i = 0; i < mDefinition.mTrackCount; i++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
			Reanimation reanimation = GlobalMembersAttachment.FindReanimAttachment(reanimatorTrackInstance.mAttachmentID);
			if (reanimation != null)
			{
				Reanimation reanimation2 = reanimation.FindSubReanim(theReanimType);
				if (reanimation2 != null)
				{
					return reanimation2;
				}
			}
		}
		return null;
	}

	private static float RotationFromQuaternion(Quaternion rotation)
	{
		return (float)Math.Atan2(2f * (rotation.Y * rotation.W - rotation.X * rotation.Z), 1f - 2f * (rotation.Y * rotation.Y + rotation.Z * rotation.Z)) * (180f / (float)Math.PI);
	}

	private void DrawSpineGroup(Graphics g, int theRenderGroup)
	{
		g.EndFrame();
		bool physicsTimeAdvanced = mNeedsRenderUpdate;
		if (mNeedsRenderUpdate)
		{
			mSpineSkeleton.Update(mAccumulatedTime);
			mAccumulatedTime = 0f;
			mNeedsRenderUpdate = false;
		}
		mSpineAnimationState.Apply(mSpineSkeleton);
		SkeletonRenderer skeletonRenderer = mSpineSkeletonRenderer[(mFilterEffect != FilterEffectType.LessWashedOut) ? mFilterEffect : FilterEffectType.WashedOut];
		skeletonRenderer.Effect.Parameters["Projection"].SetValue(g.SpineGetMatrices());
		Graphics.GetTransform(out var transform);
		float xPosition = 40f * Constants.S;
		float yPosition = 70f * Constants.S;
		Matrix matrix = Matrix.CreateTranslation(xPosition, yPosition, 0f) * mOverlayMatrix.mMatrix * Matrix.CreateTranslation(g.mTransX, g.mTransY, 0f);
		matrix.Decompose(out var scale, out var _, out var translation);
		Vector3 vector = translation * 0.5f;
		mSpineSkeleton.X = vector.X;
		mSpineSkeleton.Y = vector.Y;
		mSpineSkeleton.ScaleX = scale.X * (float)Math.Sign(mOverlayMatrix.mMatrix.M11);
		mSpineSkeleton.ScaleY = scale.Y * (float)Math.Sign(mOverlayMatrix.mMatrix.M22);
		bool flag = mSpineSkeleton.Slots.Find((Slot s) => s.Data.Name == "--noshow")?.Attachment != null;
		if (!flag)
		{
			Vector2 mPhysicalWind = g.mPhysicalWind;
			ExposedList<PhysicsConstraint> physicsConstraints = mSpineSkeleton.PhysicsConstraints;
			int count = physicsConstraints.Count;
			if (count > 0 && mPhysicalWind.LengthSquared() > 0.0001f)
			{
				Matrix matrix2 = matrix * (transform ?? Matrix.Identity);
				float value = matrix2.M11 * matrix2.M22 - matrix2.M12 * matrix2.M21;
				if (Math.Abs(value) > 1E-06f)
				{
					Matrix matrix3 = Matrix.Invert(matrix2);
					Vector2 vector2 = Vector2.TransformNormal(mPhysicalWind, matrix3);
					float[] array = new float[count];
					float[] array2 = new float[count];
					for (int num = 0; num < count; num++)
					{
						PhysicsConstraint physicsConstraint = physicsConstraints.Items[num];
						array[num] = physicsConstraint.Wind;
						array2[num] = physicsConstraint.Gravity;
						physicsConstraint.Wind = array[num] + vector2.X;
						physicsConstraint.Gravity = array2[num] + vector2.Y;
					}
					try
					{
						UpdateSkeletonWorldTranform();
					}
					finally
					{
						for (int num2 = 0; num2 < count; num2++)
						{
							PhysicsConstraint physicsConstraint2 = physicsConstraints.Items[num2];
							physicsConstraint2.Wind = array[num2];
							physicsConstraint2.Gravity = array2[num2];
						}
					}
				}
				else
				{
					UpdateSkeletonWorldTranform();
				}
			}
			else
			{
				UpdateSkeletonWorldTranform();
			}
		}
		Matrix value2 = Matrix.CreateTranslation(0f - vector.X, 0f - vector.Y, 0f) * Matrix.CreateScale(1f / scale.X * (float)Math.Sign(mOverlayMatrix.mMatrix.M11), 1f / scale.Y * (float)Math.Sign(mOverlayMatrix.mMatrix.M22), 1f) * matrix * (transform ?? Matrix.Identity);
		if (g.mColorizeImages)
		{
			mSpineSkeleton.R = (float)(int)g.mColor.R / 255f * ((float)mColorOverride.mRed / 255f);
			mSpineSkeleton.G = (float)(int)g.mColor.G / 255f * ((float)mColorOverride.mGreen / 255f);
			mSpineSkeleton.B = (float)(int)g.mColor.B / 255f * ((float)mColorOverride.mBlue / 255f);
			mSpineSkeleton.A = (float)(int)g.mColor.A / 255f * ((float)mColorOverride.mAlpha / 255f);
		}
		else
		{
			mSpineSkeleton.R = (float)mColorOverride.mRed / 255f;
			mSpineSkeleton.G = (float)mColorOverride.mGreen / 255f;
			mSpineSkeleton.B = (float)mColorOverride.mBlue / 255f;
			mSpineSkeleton.A = (float)mColorOverride.mAlpha / 255f;
		}
		if (mEnableExtraAdditiveDraw)
		{
			float num3 = (float)mExtraAdditiveColor.mAlpha / 255f;
			skeletonRenderer.Effect.Parameters["ExtraAdditiveColor"].SetValue(new Vector4((float)mExtraAdditiveColor.mRed / 255f * num3, (float)mExtraAdditiveColor.mGreen / 255f * num3, (float)mExtraAdditiveColor.mBlue / 255f * num3, num3));
		}
		else
		{
			skeletonRenderer.Effect.Parameters["ExtraAdditiveColor"].SetValue(new Vector4(0f, 0f, 0f, 0f));
		}
		SkeletonRenderer skeletonRenderer2 = skeletonRenderer;
		if (!flag)
		{
			skeletonRenderer2.Effect.Parameters["World"].SetValue(value2);
			skeletonRenderer2.Begin();
			skeletonRenderer2.Draw(mSpineSkeleton);
			skeletonRenderer2.End();
		}
		int num4 = mSpineAnimTrackCount;
		for (int num5 = 0; num5 < num4; num5++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[num5];
			if (reanimatorTrackInstance.mRenderGroup != theRenderGroup)
			{
				continue;
			}
			bool flag2 = true;
			if (reanimatorTrackInstance.mAttachmentID == null)
			{
				continue;
			}
			Attachment mAttachmentID = reanimatorTrackInstance.mAttachmentID;
			for (int num6 = 0; num6 < mAttachmentID.mNumEffects; num6++)
			{
				AttachEffect attachEffect = mAttachmentID.mEffectArray[num6];
				if (attachEffect.mEffectType == EffectType.Reanim)
				{
					Reanimation reanimation = (Reanimation)attachEffect.mEffectID;
					reanimation.mColorOverride = mColorOverride;
					reanimation.mExtraAdditiveColor = mExtraAdditiveColor;
					reanimation.mExtraOverlayColor = mExtraOverlayColor;
				}
			}
			GlobalMembersAttachment.AttachmentDraw(reanimatorTrackInstance.mAttachmentID, g, !flag2, doScale: false);
		}
		void UpdateSkeletonWorldTranform()
		{
			Skeleton.Physics physics = Skeleton.Physics.None;
			if (ShouldDoSpinePhysics())
			{
				physics = ((!physicsTimeAdvanced) ? Skeleton.Physics.Pose : Skeleton.Physics.Update);
			}
			mSpineSkeleton.UpdateWorldTransform(physics);
		}
	}

	public bool ShouldDoSpinePhysics()
	{
		if (mReanimationType != ReanimationType.PumpkinUpper)
		{
			return mReanimationType != ReanimationType.PumpkinLower;
		}
		return false;
	}

	private void UpdateSpine()
	{
		TrackEntry trackEntry = mSpineAnimationState.Tracks.First();
		float num = trackEntry.AnimationEnd - trackEntry.AnimationStart;
		if (mSpineDoAnimTimeUpdate)
		{
			SpineUpdateAnimTimeHook(_animTime, (_loopType == ReanimLoopType.Loop || _loopType == ReanimLoopType.LoopFullLastFrame) ? _loopCount : 0);
			mSpineDoAnimTimeUpdate = false;
		}
		if (mSpineDoLoopTypeUpdate)
		{
			mSpineAnimationState.Tracks.ForEach((TrackEntry t) =>
			{
				t.Loop = _loopType == ReanimLoopType.Loop || _loopType == ReanimLoopType.LoopFullLastFrame;
			});
			if (_loopType != ReanimLoopType.Loop && _loopType != ReanimLoopType.LoopFullLastFrame)
			{
				SpineUpdateAnimTimeHook(_animTime, 0);
			}
			else
			{
				SpineUpdateAnimTimeHook(_animTime, _loopCount);
			}
			mSpineDoLoopTypeUpdate = false;
		}
		if (_animTime >= 1f && _loopType == ReanimLoopType.PlayOnceAndHold)
		{
			return;
		}
		float num2 = 0.01f / mSpineSkeleton.Data.Fps * mAnimRate;
		mAccumulatedTime += num2;
		mNeedsRenderUpdate = true;
		mSpineAnimationState.Update(num2);
		if (ShouldDoSpinePhysics())
		{
			mSpineAnimationState.Apply(mSpineSkeleton);
		}
		mLastFrameTime = mAnimTime;
		if (_loopType != ReanimLoopType.Loop && _loopType != ReanimLoopType.LoopFullLastFrame && (int)((double)(trackEntry.TrackTime - trackEntry.AnimationStart) / (double)num) >= 1)
		{
			_animTime = 1f;
			_loopCount = 1;
		}
		else
		{
			_animTime = (float)((double)(trackEntry.TrackTime - trackEntry.AnimationStart) % (double)num / (double)num);
			_loopCount = (int)((double)(trackEntry.TrackTime - trackEntry.AnimationStart) / (double)num);
		}
		if (mLoopCount >= 1 && (_loopType == ReanimLoopType.PlayOnce || _loopType == ReanimLoopType.PlayOnceFullLastFrame))
		{
			mDead = true;
		}
		else if (mLoopType == ReanimLoopType.PlayThenAction && mAnimTime >= 1f)
		{
			Debug.ASSERT(mAction != null);
			mAnimTime = 1f;
			mLoopCount = 1;
			ReanimLoopAction reanimLoopAction = mAction;
			mDead = reanimLoopAction != null && !reanimLoopAction(this);
		}
		int num3 = mSpineAnimTrackCount;
		for (int num4 = 0; num4 < num3; num4++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[num4];
			if (reanimatorTrackInstance.mRenderGroup != ReanimatorXnaHelpers.RENDER_GROUP_HIDDEN)
			{
				if (reanimatorTrackInstance.mBlendCounter > 0)
				{
					ReanimatorTrackInstance reanimatorTrackInstance2 = reanimatorTrackInstance;
					reanimatorTrackInstance2.mBlendCounter--;
				}
				if (reanimatorTrackInstance.mShakeOverride != 0f)
				{
					reanimatorTrackInstance.mShakeX = TodCommon.RandRangeFloat(0f - reanimatorTrackInstance.mShakeOverride, reanimatorTrackInstance.mShakeOverride);
					reanimatorTrackInstance.mShakeY = TodCommon.RandRangeFloat(0f - reanimatorTrackInstance.mShakeOverride, reanimatorTrackInstance.mShakeOverride);
				}
				if (reanimatorTrackInstance.mAttachmentID != null)
				{
					GetAttachmentOverlayMatrix(num4, out aOverlayMatrix);
					GlobalMembersAttachment.AttachmentUpdateAndSetMatrix(ref reanimatorTrackInstance.mAttachmentID, ref aOverlayMatrix);
				}
			}
		}
	}

	private void SpineUpdateAnimTimeHook(float anAnimTime, int aLoopCount)
	{
		mSpineAnimationState.Tracks.ForEach((TrackEntry track) =>
		{
			float num = track.AnimationEnd - track.AnimationStart;
			track.TrackTime = (float)aLoopCount * num + anAnimTime * num;
		});
	}

	private bool SpineTrackExists(string theTrackName)
	{
		if (theTrackName.StartsWith("anim_"))
		{
			SkeletonData data = mSpineSkeleton.Data;
			int length = "anim_".Length;
			return data.FindAnimation(theTrackName.Substring(length, theTrackName.Length - length)) != null;
		}
		return mSpineSkeleton.Data.FindSlot(theTrackName) != null;
	}

	private void SpineSetFramesForLayer(string theTrackName)
	{
		if (mAnimRate >= 0f)
		{
			mAnimTime = 0f;
		}
		else
		{
			mAnimTime = 0.9999999f;
		}
		mLastFrameTime = -1f;
		GetFramesForLayer(theTrackName, out mFrameStart, out mFrameCount);
		if (!theTrackName.StartsWith("anim_"))
		{
			return;
		}
		mSpineCurrentTrackName = theTrackName;
		AnimationState animationState = mSpineAnimationState;
		int length = "anim_".Length;
		TrackEntry trackEntry = animationState.SetAnimation(0, theTrackName.Substring(length, theTrackName.Length - length), mLoopType == ReanimLoopType.Loop || mLoopType == ReanimLoopType.LoopFullLastFrame);
		if (mSpineBlendTime > 0)
		{
			float num = (float)mSpineBlendTime / 100f / mSpineSkeleton.Data.Fps * mAnimRate;
			if (trackEntry.MixingFrom != null)
			{
				trackEntry.MixingFrom.MixBlend = MixBlend.Replace;
				trackEntry.MixingFrom.Delay += num;
			}
			trackEntry.MixDuration = num;
			trackEntry.MixBlend = MixBlend.First;
			mSpineBlendTime = 0;
		}
		mFrameCount = (short)((trackEntry.AnimationEnd - trackEntry.AnimationStart) * mSpineSkeleton.Data.Fps);
	}

	private int SpineFindTrackIndex(string theTrackName)
	{
		int num = mSpineSkeleton.Data.Animations.FindIndex((Animation a) =>
		{
			string name = a.Name;
			string text = theTrackName;
			int length = "anim_".Length;
			return name == text.Substring(length, text.Length - length);
		});
		if (num != -1)
		{
			return num;
		}
		return 0;
	}

	private bool SpineIsTrackShowing(string theTrackName)
	{
		return mSpineAnimationState.Tracks.Any((TrackEntry t) =>
		{
			string name = t.Animation.Name;
			string text = theTrackName;
			int length = "anim_".Length;
			return name == text.Substring(length, text.Length - length);
		});
	}

	private bool SpineIsAnimPlaying(string theTrackName)
	{
		return mSpineAnimationState.Tracks.Any((TrackEntry t) =>
		{
			string name = t.Animation.Name;
			string text = theTrackName;
			int length = "anim_".Length;
			return name == text.Substring(length, text.Length - length) && (t.Loop || t.AnimationTime < t.AnimationEnd);
		});
	}

	public void SpineStartBlend(byte theBlendTime)
	{
		mSpineBlendTime = theBlendTime;
	}

	private AttachEffect AttachToAnotherReanimationSpine(ref Reanimation theAttachReanim, string theTrackName)
	{
		if (theAttachReanim.mFrameBasePose == -1)
		{
			theAttachReanim.mFrameBasePose = theAttachReanim.mFrameStart;
		}
		int num = theAttachReanim.FindTrackIndex(theTrackName);
		ReanimatorTrackInstance reanimatorTrackInstance = theAttachReanim.mTrackInstances[num];
		return GlobalMembersAttachment.AttachReanim(ref reanimatorTrackInstance.mAttachmentID, this, 0f, 0f);
	}

	private void PropogateColorToAttachmentsSpine()
	{
		for (int i = 0; i < mSpineAnimTrackCount; i++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = mTrackInstances[i];
			GlobalMembersAttachment.AttachmentPropogateColor(reanimatorTrackInstance.mAttachmentID, mColorOverride, mEnableExtraAdditiveDraw, mExtraAdditiveColor, mEnableExtraOverlayDraw, mExtraOverlayColor);
		}
	}

	private void GetTransformAtTimeSpine(int theTrackIndex, out ReanimatorTransform aTransform, ReanimatorFrameTime theFrameTime, bool nullIfInvalidFrame)
	{
		aTransform = ReanimatorTransform.GetNewReanimatorTransform();
		aTransform.mTransX = 0f;
		aTransform.mTransY = 0f;
		aTransform.mSkewX = 0f;
		aTransform.mSkewY = 0f;
		aTransform.mScaleX = 1f;
		aTransform.mScaleY = 1f;
		aTransform.mAlpha = 1f;
		aTransform.mSkewXCos = 1f;
		aTransform.mSkewXSin = 0f;
		aTransform.mSkewYCos = 1f;
		aTransform.mSkewYSin = 0f;
		aTransform.mImage = null;
		aTransform.mFont = null;
		aTransform.mText = string.Empty;
		aTransform.mFrame = -1f;
	}

	public void ReanimationSpineInitialize(float theX, float theY)
	{
		mDead = false;
		mIsSpine = true;
		SetPosition(theX, theY);
		mAnimRate = mDefinition.mSpineSkeletonData.Fps;
		mLastFrameTime = -1f;
		mFrameCount = 0;
		mSpineSkeleton = new Skeleton(mDefinition.mSpineSkeletonData);
		AnimationStateData data = new AnimationStateData(mSpineSkeleton.Data);
		mSpineAnimationState = new AnimationState(data);
		mSpineAnimTrackCount = mDefinition.mSpineSkeletonData.Animations.Count;
		if (mSpineSkeleton.Data.Animations.Exists((Animation a) => a.Name == "idle"))
		{
			SetFramesForLayer("anim_idle");
		}
		ReanimationSpineInitializeTrackInstances();
	}

	private void ReanimationSpineInitializeTrackInstances()
	{
		int num = 0;
		foreach (Animation animation in mDefinition.mSpineSkeletonData.Animations)
		{
			mTrackInstances[num++] = ReanimatorTrackInstance.GetNewReanimatorTrackInstance();
		}
	}

	public static void ReanimSpineInit(Graphics g, FilterEffectType theFilterEffect)
	{
		GraphicsDevice graphicsDevice = g.GraphicsDevice;
		mSpineSkeletonRenderer[theFilterEffect] = new SkeletonRenderer(graphicsDevice);
		mSpineSkeletonRenderer[theFilterEffect].PremultipliedAlpha = true;
		Effect effect;
		switch (theFilterEffect)
		{
		case FilterEffectType.None:
			effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\LawnSpineEffect");
			break;
		case FilterEffectType.White:
			effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\LawnSpineEffectWhite");
			break;
		case FilterEffectType.WashedOut:
		case FilterEffectType.LessWashedOut:
			effect = GlobalStaticVars.gLawnApp.mContentManager.Load<Effect>("shaders\\LawnSpineEffectWashedOut");
			effect.Parameters["Greyscale"].SetValue(0.7f);
			break;
		default:
			throw new NotSupportedException($"FilterEffect {theFilterEffect} is not currently supported by SpineReanim.");
		}
		effect.Parameters["World"].SetValue(Matrix.Identity);
		effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0f, 0f, 1f), Vector3.Zero, Vector3.Up));
		mSpineSkeletonRenderer[theFilterEffect].Effect = effect;
	}
}
