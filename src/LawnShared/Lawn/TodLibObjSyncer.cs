using System;
using System.Collections.Generic;
using Sexy;
using Sexy.TodLib;
using Spine;

namespace Lawn;

internal static class TodLibObjSyncer
{
	public static void SyncList<T, TH>(List<T> lst, TH helper, GlobalMembersSaveGame.SaveGameContext theContext, Func<T> getNew, Action<T, TH, GlobalMembersSaveGame.SaveGameContext> sync) where T : class
	{
		int theInt = lst.Count;
		theContext.SyncListLength(ref theInt);
		if (theContext.mReading)
		{
			lst.Clear();
		}
		for (int i = 0; i < theInt; i++)
		{
			if (theContext.mReading)
			{
				lst.Add(getNew());
			}
			sync(lst[i], helper, theContext);
		}
	}

	public static void ClearObj(this EffectSystem theEffectSystem)
	{
		for (int i = 0; i < theEffectSystem.mParticleHolder.mParticleSystems.Count; i++)
		{
			TodParticleSystem todParticleSystem = theEffectSystem.mParticleHolder.mParticleSystems[i];
			todParticleSystem.mParticleHolder = theEffectSystem.mParticleHolder;
			todParticleSystem.ParticleSystemDie();
			todParticleSystem.PrepareForReuse();
		}
		theEffectSystem.mParticleHolder.mParticleSystems.Clear();
		theEffectSystem.mParticleHolder.mEmitters.Clear();
		theEffectSystem.mParticleHolder.mParticles.Clear();
		for (int j = 0; j < theEffectSystem.mReanimationHolder.mReanimations.Count; j++)
		{
			theEffectSystem.mReanimationHolder.mReanimations[j].PrepareForReuse();
		}
		theEffectSystem.mReanimationHolder.mReanimations.Clear();
		theEffectSystem.mTrailHolder.mTrails.Clear();
		for (int k = 0; k < theEffectSystem.mAttachmentHolder.mAttachments.Count; k++)
		{
			theEffectSystem.mAttachmentHolder.mAttachments[k].PrepareForReuse();
		}
		theEffectSystem.mAttachmentHolder.mAttachments.Clear();
	}

	public static void Sync(this EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		SyncList(theEffectSystem.mParticleHolder.mParticles, theEffectSystem, theContext, TodParticle.GetNewTodParticle, SyncTodParticle);
		SyncList(theEffectSystem.mParticleHolder.mEmitters, theEffectSystem, theContext, TodParticleEmitter.GetNewTodParticleEmitter, SyncTodParticleEmitter);
		SyncList(theEffectSystem.mParticleHolder.mParticleSystems, theEffectSystem, theContext, TodParticleSystem.GetNewTodParticleSystem, SyncTodParticleSystem);
		SyncList(theEffectSystem.mTrailHolder.mTrails, theEffectSystem, theContext, () => new Trail(), SyncTrail);
		SyncList(theEffectSystem.mReanimationHolder.mReanimations, theEffectSystem, theContext, Reanimation.GetNewReanimation, SyncReanimation);
		SyncList(theEffectSystem.mAttachmentHolder.mAttachments, theEffectSystem, theContext, Sexy.TodLib.Attachment.GetNewAttachment, SyncAttachment);
	}

	public static void SyncTodParticle(TodParticle theParticle, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncInt(ref theParticle.mParticleEmitter_Save);
		theContext.SyncInt(ref theParticle.mParticleDuration);
		theContext.SyncInt(ref theParticle.mParticleAge);
		theContext.SyncFloat(ref theParticle.mParticleTimeValue);
		theContext.SyncFloat(ref theParticle.mParticleLastTimeValue);
		theContext.SyncFloat(ref theParticle.mAnimationTimeValue);
		theContext.SyncSexyVector2(ref theParticle.mVelocity);
		theContext.SyncSexyVector2(ref theParticle.mPosition);
		theContext.SyncInt(ref theParticle.mImageFrame);
		theContext.SyncFloat(ref theParticle.mSpinPosition);
		theContext.SyncFloat(ref theParticle.mSpinVelocity);
		theContext.SyncInt(ref theParticle.mCrossFadeParticleID_Save);
		theContext.SyncInt(ref theParticle.mCrossFadeDuration);
		for (int i = 0; i < 16; i++)
		{
			theContext.SyncFloat(ref theParticle.mParticleInterp[i]);
		}
		for (int j = 0; j < 5; j++)
		{
			for (int k = 0; k < 2; k++)
			{
				theContext.SyncFloat(ref theParticle.mParticleFieldInterp[j, k]);
			}
		}
	}

	public static void SyncTodParticleEmitter(TodParticleEmitter theParticleEmitter, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncInt(ref theParticleEmitter.mEmitterDef_Save);
		theContext.SyncInt(ref theParticleEmitter.mParticleSystem_Save);
		int theInt = theParticleEmitter.mParticleList.Count;
		theContext.SyncInt(ref theInt);
		if (theContext.mReading)
		{
			theParticleEmitter.mParticleList.Clear();
		}
		for (int i = 0; i < theInt; i++)
		{
			if (theContext.mReading)
			{
				int theInt2 = 0;
				theContext.SyncInt(ref theInt2);
				TodParticle item = null;
				if (theInt2 >= 0 && theInt2 < theEffectSystem.mParticleHolder.mParticles.Count)
				{
					item = theEffectSystem.mParticleHolder.mParticles[theInt2];
				}
				theParticleEmitter.mParticleList.Add(item);
			}
			else
			{
				int theInt3 = theEffectSystem.mParticleHolder.mParticles.IndexOf(theParticleEmitter.mParticleList[i]);
				theContext.SyncInt(ref theInt3);
			}
		}
		theContext.SyncFloat(ref theParticleEmitter.mSpawnAccum);
		theContext.SyncSexyVector2(ref theParticleEmitter.mSystemCenter);
		theContext.SyncInt(ref theParticleEmitter.mParticlesSpawned);
		theContext.SyncInt(ref theParticleEmitter.mSystemAge);
		theContext.SyncInt(ref theParticleEmitter.mSystemDuration);
		theContext.SyncFloat(ref theParticleEmitter.mSystemTimeValue);
		theContext.SyncFloat(ref theParticleEmitter.mSystemLastTimeValue);
		theContext.SyncBool(ref theParticleEmitter.mDead);
		theContext.SyncSexyColor(ref theParticleEmitter.mColorOverride);
		theContext.SyncBool(ref theParticleEmitter.mExtraAdditiveDrawOverride);
		theContext.SyncFloat(ref theParticleEmitter.mScaleOverride);
		theContext.SyncImage(ref theParticleEmitter.mImageOverride);
		theContext.SyncInt(ref theParticleEmitter.mCrossFadeEmitterID_Save);
		theContext.SyncInt(ref theParticleEmitter.mEmitterCrossFadeCountDown);
		theContext.SyncInt(ref theParticleEmitter.mFrameOverride);
		theContext.SyncBool(ref theParticleEmitter.mActive);
		for (int j = 0; j < 10; j++)
		{
			theContext.SyncFloat(ref theParticleEmitter.mTrackInterp[j]);
		}
		for (int k = 0; k < 5; k++)
		{
			for (int l = 0; l < 2; l++)
			{
				theContext.SyncFloat(ref theParticleEmitter.mSystemFieldInterp[k, l]);
			}
		}
	}

	public static void SyncTodParticleSystem(TodParticleSystem theParticleSystem, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncEnum(ref theParticleSystem.mEffectType, theContext.aParticleEffectSaver, SaveFileVersion.Initial, ParticleEffect.Melonsplash);
		if (theContext.mReading)
		{
			int mEffectType = (int)theParticleSystem.mEffectType;
			theParticleSystem.mParticleDef = ((mEffectType >= 0 && mEffectType < TodParticleGlobal.gParticleDefArray.Length) ? TodParticleGlobal.gParticleDefArray[mEffectType] : null);
			theParticleSystem.mParticleHolder = theEffectSystem.mParticleHolder;
		}
		int theInt = theParticleSystem.mEmitterList.Count;
		theContext.SyncInt(ref theInt);
		if (theContext.mReading)
		{
			theParticleSystem.mEmitterList.Clear();
		}
		for (int i = 0; i < theInt; i++)
		{
			if (theContext.mReading)
			{
				int theInt2 = 0;
				theContext.SyncInt(ref theInt2);
				TodParticleEmitter item = null;
				if (theInt2 >= 0 && theInt2 < theEffectSystem.mParticleHolder.mEmitters.Count)
				{
					item = theEffectSystem.mParticleHolder.mEmitters[theInt2];
				}
				theParticleSystem.mEmitterList.Add(item);
			}
			else
			{
				int theInt3 = theEffectSystem.mParticleHolder.mEmitters.IndexOf(theParticleSystem.mEmitterList[i]);
				theContext.SyncInt(ref theInt3);
			}
		}
		theContext.SyncBool(ref theParticleSystem.mDead);
		theContext.SyncBool(ref theParticleSystem.mIsAttachment);
		theContext.SyncInt(ref theParticleSystem.mRenderOrder);
		theContext.SyncBool(ref theParticleSystem.mDontUpdate);
		theContext.SyncBool(ref theParticleSystem.mActive);
	}

	public static void SyncTrail(Trail theTrail, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		if (theContext.mReading)
		{
			TrailType theEnum = TrailType.None;
			theContext.SyncEnum(ref theEnum, theContext.aTrailTypeSaver, SaveFileVersion.Initial, TrailType.Ice);
			int num = (int)theEnum;
			theTrail.mDefinition = ((num >= 0 && num < GlobalMembersTrail.gTrailDefArray.Length) ? GlobalMembersTrail.gTrailDefArray[num] : null);
			theTrail.mTrailHolder = theEffectSystem.mTrailHolder;
		}
		else
		{
			int num2 = ((IList<TrailDefinition>)GlobalMembersTrail.gTrailDefArray).IndexOf(theTrail.mDefinition);
			TrailType theEnum2 = (TrailType)num2;
			theContext.SyncEnum(ref theEnum2, theContext.aTrailTypeSaver, SaveFileVersion.Initial, TrailType.Ice);
		}
		if (theContext.mVersion >= SaveFileVersion.CardGroup)
		{
			for (int i = 0; i < 20; i++)
			{
				TrailPoint trailPoint = theTrail.mTrailPoints[i];
				theContext.SyncSexyVector2(ref trailPoint.aPos);
				theTrail.mTrailPoints[i] = trailPoint;
			}
		}
		else
		{
			for (int j = 0; j < 20; j++)
			{
				bool theBool = true;
				theContext.SyncBool(ref theBool);
				if (theBool)
				{
					TrailPoint trailPoint2 = theTrail.mTrailPoints[j];
					theContext.SyncSexyVector2(ref trailPoint2.aPos);
					theTrail.mTrailPoints[j] = trailPoint2;
				}
				else
				{
					theTrail.mTrailPoints[j] = default;
				}
			}
		}
		theContext.SyncInt(ref theTrail.mNumTrailPoints);
		theContext.SyncBool(ref theTrail.mDead);
		theContext.SyncInt(ref theTrail.mRenderOrder);
		theContext.SyncInt(ref theTrail.mTrailAge);
		theContext.SyncInt(ref theTrail.mTrailDuration);
		for (int k = 0; k < 4; k++)
		{
			theContext.SyncFloat(ref theTrail.mTrailInterp[k]);
		}
		theContext.SyncSexyVector2(ref theTrail.mTrailCenter);
		theContext.SyncBool(ref theTrail.mIsAttachment);
		theContext.SyncSexyColor(ref theTrail.mColorOverride);
		if (theContext.mVersion < SaveFileVersion.NewLayout600 && !theTrail.mIsAttachment)
		{
			for (int l = 0; l < theTrail.mNumTrailPoints; l++)
			{
				TrailPoint trailPoint3 = theTrail.mTrailPoints[l];
				theTrail.mTrailPoints[l].aPos.x = trailPoint3.aPos.x + (float)Constants.BoardPosChange;
				theTrail.mTrailPoints[l].aPos.y = trailPoint3.aPos.y;
			}
		}
	}

	public static void SyncReanimation(Reanimation theReanimation, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		if (theContext.mVersion >= SaveFileVersion.SpineAnimFirstStage)
		{
			if (theContext.mReading)
			{
				theContext.SyncBool(ref theReanimation.mIsSpine_Save);
				theReanimation.mIsSpine = theReanimation.mIsSpine_Save;
			}
			else
			{
				theReanimation.mIsSpine_Save = theReanimation.mIsSpine;
				theContext.SyncBool(ref theReanimation.mIsSpine);
			}
		}
		else
		{
			theReanimation.mIsSpine_Save = false;
		}
		theContext.SyncEnum(ref theReanimation.mReanimationType, theContext.aReanimationTypeSaver, SaveFileVersion.Initial, ReanimationType.LoadbarSprout);
		if (theContext.mReading)
		{
			int mReanimationType = (int)theReanimation.mReanimationType;
			if (mReanimationType >= 0 && mReanimationType < ReanimatorXnaHelpers.gReanimatorDefArray.Length)
			{
				ReanimatorXnaHelpers.ReanimatorEnsureDefinitionLoaded(theReanimation.mReanimationType, theIsPreloading: true);
				theReanimation.mDefinition = ReanimatorXnaHelpers.gReanimatorDefArray[mReanimationType];
			}
			theReanimation.mReanimationHolder = theEffectSystem.mReanimationHolder;
			if (theReanimation.mDefinition.mIsSpine)
			{
				theReanimation.ReanimationSpineInitialize(0f, 0f);
			}
		}
		theContext.SyncFloat(ref theReanimation.mAnimTime_Save);
		theContext.SyncFloat(ref theReanimation.mAnimRate);
		theContext.SyncEnum(ref theReanimation.mLoopType_Save, theContext.aReanimLoopTypeSaver, SaveFileVersion.Initial, ReanimLoopType.Loop);
		theContext.SyncBool(ref theReanimation.mDead);
		theContext.SyncShort(ref theReanimation.mFrameStart, SaveFileVersion.Initial, 0);
		theContext.SyncShort(ref theReanimation.mFrameCount, SaveFileVersion.Initial, 0);
		theContext.SyncShort(ref theReanimation.mFrameBasePose, SaveFileVersion.Initial, 0);
		theContext.SyncSexyTransform2D(ref theReanimation.mOverlayMatrix);
		theContext.SyncSexyColor(ref theReanimation.mColorOverride);
		Dictionary<int, int> d = null;
		if (theReanimation.mIsSpine_Save && theContext.mVersion >= SaveFileVersion.SpineAnimTrackNames && theReanimation.mIsSpine)
		{
			ExposedList<Animation> animations = theReanimation.mSpineSkeleton.Data.Animations;
			int mSpineAnimTrackCount = theReanimation.mSpineAnimTrackCount;
			theContext.SyncInt(ref theReanimation.mSpineAnimTrackCount);
			for (int i = 0; i < theReanimation.mSpineAnimTrackCount; i++)
			{
				string trackName = (theContext.mReading ? null : animations.Items[i].Name);
				theContext.SyncString(ref trackName);
				if (theContext.mReading)
				{
					int b = animations.FindIndex((Animation a) => a.Name == trackName);
					LazyAdd(ref d, i, b);
				}
			}
			if (theContext.mReading)
			{
				theReanimation.mSpineAnimTrackCount = mSpineAnimTrackCount;
			}
		}
		else if (!theReanimation.mIsSpine_Save && theReanimation.mIsSpine && theContext.mReading)
		{
			ExposedList<Animation> animations2 = theReanimation.mSpineSkeleton.Data.Animations;
			int mSpineAnimTrackCount2 = theReanimation.mSpineAnimTrackCount;
			for (int num = 0; num < theReanimation.mDefinition.mTrackCount; num++)
			{
				string trackName2 = (theContext.mReading ? null : animations2.Items[num].Name);
				if (trackName2 == null)
				{
					trackName2 = theReanimation.mDefinition.mTracks[num].mName;
				}
				if (theContext.mReading)
				{
					int b2 = animations2.FindIndex((Animation a) => "anim_" + a.Name == trackName2);
					LazyAdd(ref d, num, b2);
				}
			}
		}
		for (int num2 = 0; num2 < 100; num2++)
		{
			int value = num2;
			if (d != null && !d.TryGetValue(num2, out value))
			{
				value = num2;
			}
			bool theBool = value != -1 && theReanimation.mTrackInstances[value] != null;
			theContext.SyncBool(ref theBool);
			if (theBool)
			{
				if (value != -1)
				{
					ReanimatorTrackInstance[] mTrackInstances = theReanimation.mTrackInstances;
					int num3 = value;
					if (mTrackInstances[num3] == null)
					{
						mTrackInstances[num3] = ReanimatorTrackInstance.GetNewReanimatorTrackInstance();
					}
				}
				ReanimatorTrackInstance reanimatorTrackInstance = ((value == -1) ? ReanimatorTrackInstance.GetNewReanimatorTrackInstance() : theReanimation.mTrackInstances[value]);
				theContext.SyncByte(ref reanimatorTrackInstance.mBlendCounter, SaveFileVersion.Initial, 0);
				theContext.SyncByte(ref reanimatorTrackInstance.mBlendTime, SaveFileVersion.Initial, 0);
				bool theBool2 = reanimatorTrackInstance.mBlendTransform != null;
				theContext.SyncBool(ref theBool2);
				if (theBool2)
				{
					ReanimatorTrackInstance reanimatorTrackInstance2 = reanimatorTrackInstance;
					if (reanimatorTrackInstance2.mBlendTransform == null)
					{
						reanimatorTrackInstance2.mBlendTransform = ReanimatorTransform.GetNewReanimatorTransform();
					}
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mTransX);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mTransY);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mSkewX);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mSkewY);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mScaleX);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mScaleY);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mFrame);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mAlpha);
					theContext.SyncImage(ref reanimatorTrackInstance.mBlendTransform.mImage);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mSkewXCos);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mSkewXSin);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mSkewYCos);
					theContext.SyncFloat(ref reanimatorTrackInstance.mBlendTransform.mSkewYSin);
				}
				theContext.SyncFloat(ref reanimatorTrackInstance.mShakeOverride);
				theContext.SyncFloat(ref reanimatorTrackInstance.mShakeX);
				theContext.SyncFloat(ref reanimatorTrackInstance.mShakeY);
				theContext.SyncInt(ref reanimatorTrackInstance.mAttachmentID_Save);
				theContext.SyncImage(ref reanimatorTrackInstance.mImageOverride);
				theContext.SyncInt(ref reanimatorTrackInstance.mRenderGroup);
				theContext.SyncSexyColor(ref reanimatorTrackInstance.mTrackColor);
				theContext.SyncBool(ref reanimatorTrackInstance.mIgnoreClipRect);
				theContext.SyncBool(ref reanimatorTrackInstance.mTruncateDisappearingFrames);
				theContext.SyncBool(ref reanimatorTrackInstance.mIgnoreColorOverride);
				theContext.SyncBool(ref reanimatorTrackInstance.mIgnoreExtraAdditiveColor);
			}
			else if (theContext.mReading && ((!theReanimation.mIsSpine_Save && value < theReanimation.mDefinition.mTrackCount) || (theReanimation.mIsSpine_Save && value < theReanimation.mSpineAnimTrackCount)) && value != -1)
			{
				theReanimation.mTrackInstances[value] = ReanimatorTrackInstance.GetNewReanimatorTrackInstance();
			}
		}
		theContext.SyncInt(ref theReanimation.mLoopCount_Save);
		theContext.SyncBool(ref theReanimation.mIsAttachment);
		theContext.SyncInt(ref theReanimation.mRenderOrder);
		theContext.SyncSexyColor(ref theReanimation.mExtraAdditiveColor);
		theContext.SyncBool(ref theReanimation.mEnableExtraAdditiveDraw);
		theContext.SyncSexyColor(ref theReanimation.mExtraOverlayColor);
		theContext.SyncBool(ref theReanimation.mEnableExtraOverlayDraw);
		theContext.SyncFloat(ref theReanimation.mLastFrameTime);
		theContext.SyncEnum(ref theReanimation.mFilterEffect, theContext.aFilterEffectTypeSaver, SaveFileVersion.Initial, FilterEffectType.WashedOut);
		theContext.SyncBool(ref theReanimation.mClip);
		theContext.SyncBool(ref theReanimation.mActive);
		theContext.SyncBool(ref theReanimation.mGetFrameTime);
		theContext.SyncFloat(ref theReanimation.mFrameTime.mFraction);
		theContext.SyncShort(ref theReanimation.mFrameTime.mAnimFrameBeforeInt, SaveFileVersion.Initial, 0);
		theContext.SyncShort(ref theReanimation.mFrameTime.mAnimFrameAfterInt, SaveFileVersion.Initial, 0);
		theContext.SyncSexyTransform2D(ref theReanimation.aOverlayMatrix);
		theContext.SyncMatrix(ref theReanimation.aBasePoseMatrix);
		theContext.SyncMatrix(ref theReanimation.tempOverlayMatrix);
		theContext.SyncSexyTransform2D(ref theReanimation.basePose);
		if (theContext.mVersion >= SaveFileVersion.SpineAnimFirstStage && theReanimation.mIsSpine_Save)
		{
			SyncReanimationSpine(theReanimation, theEffectSystem, theContext);
		}
		SyncReanimationSpineHelper(theReanimation, theEffectSystem, theContext);
		static void LazyAdd(ref Dictionary<int, int> reference, int a, int num4)
		{
			if (a != num4)
			{
				if (reference == null)
				{
					reference = new Dictionary<int, int>();
				}
				reference.Add(a, num4);
			}
		}
	}

	public static void SyncReanimationSpine(Reanimation theReanimation, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		theContext.SyncString(ref theReanimation.mSpineCurrentTrackName);
	}

	public static void SyncReanimationSpineHelper(Reanimation theReanimation, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		if (!theContext.mReading)
		{
			return;
		}
		if (theReanimation.mIsSpine_Save && theReanimation.mDefinition.mIsSpine)
		{
			theReanimation.mIsSpine = true;
			float anAnimTime = theReanimation.mAnimTime;
			int aLoopCount = theReanimation.mLoopCount;
			theReanimation.SetFramesForLayer(theReanimation.mSpineCurrentTrackName);
			theReanimation.mSpineAnimationState.Tracks.ForEach((TrackEntry track) =>
			{
				float num2 = track.AnimationEnd - track.AnimationStart;
				track.TrackTime = (float)aLoopCount * num2 + anAnimTime * num2;
			});
			theReanimation.mAnimTime = anAnimTime;
			theReanimation.mLoopCount = aLoopCount;
			theReanimation.mSpineAnimationState.Apply(theReanimation.mSpineSkeleton);
		}
		else if (!theReanimation.mIsSpine_Save && theReanimation.mDefinition.mIsSpine)
		{
			theReanimation.mIsSpine = false;
			float anAnimTime2 = theReanimation.mAnimTime;
			int aLoopCount2 = theReanimation.mLoopCount;
			string text = null;
			ReanimatorTrack[] mTracks = theReanimation.mDefinition.mTracks;
			foreach (ReanimatorTrack reanimatorTrack in mTracks)
			{
				if (theReanimation.IsAnimPlaying(reanimatorTrack.mName))
				{
					text = reanimatorTrack.mName;
					break;
				}
			}
			theReanimation.mIsSpine = true;
			if (!string.IsNullOrEmpty(text))
			{
				theReanimation.SetFramesForLayer(text);
				theReanimation.mSpineAnimationState.Tracks.ForEach((TrackEntry track) =>
				{
					float num2 = track.AnimationEnd - track.AnimationStart;
					track.TrackTime = (float)aLoopCount2 * num2 + anAnimTime2 * num2;
				});
			}
			theReanimation.mSpineAnimationState.Apply(theReanimation.mSpineSkeleton);
		}
		else if (theReanimation.mIsSpine_Save && !theReanimation.mDefinition.mIsSpine)
		{
			theReanimation.mIsSpine = false;
			float mAnimTime = theReanimation.mAnimTime;
			int mLoopCount = theReanimation.mLoopCount;
			theReanimation.SetFramesForLayer(theReanimation.mSpineCurrentTrackName);
			theReanimation.mAnimTime = mAnimTime;
			theReanimation.mLoopCount = mLoopCount;
		}
	}

	public static void SyncAttachment(Sexy.TodLib.Attachment theAttachment, EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		for (int i = 0; i < 16; i++)
		{
			bool theBool = theAttachment.mEffectArray[i] != null;
			theContext.SyncBool(ref theBool);
			if (theBool)
			{
				AttachEffect[] mEffectArray = theAttachment.mEffectArray;
				int num = i;
				if (mEffectArray[num] == null)
				{
					mEffectArray[num] = AttachEffect.GetNewAttachEffect();
				}
				AttachEffect attachEffect = theAttachment.mEffectArray[i];
				theContext.SyncEnum(ref attachEffect.mEffectType, theContext.aEffectTypeSaver, SaveFileVersion.Initial, EffectType.Particle);
				theContext.SyncInt(ref attachEffect.mEffectID_Save);
				theContext.SyncSexyTransform2D(ref attachEffect.mOffset);
				theContext.SyncBool(ref attachEffect.mDontDrawIfParentHidden);
				theContext.SyncBool(ref attachEffect.mDontPropogateColor);
			}
		}
		theContext.SyncInt(ref theAttachment.mNumEffects);
		theContext.SyncBool(ref theAttachment.mDead);
		theContext.SyncBool(ref theAttachment.mActive);
		theContext.SyncBool(ref theAttachment.mUsesClipping);
		theContext.SyncBool(ref theAttachment.reused);
	}

	public static void SyncObj(this EffectSystem theEffectSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		List<TodParticle> mParticles = theEffectSystem.mParticleHolder.mParticles;
		for (int i = 0; i < mParticles.Count; i++)
		{
			mParticles[i].SyncObj(theContext);
		}
		List<TodParticleEmitter> mEmitters = theEffectSystem.mParticleHolder.mEmitters;
		for (int j = 0; j < mEmitters.Count; j++)
		{
			mEmitters[j].SyncObj(theContext);
		}
		List<Reanimation> mReanimations = theEffectSystem.mReanimationHolder.mReanimations;
		for (int k = 0; k < mReanimations.Count; k++)
		{
			mReanimations[k].SyncObj(theContext);
		}
		List<Sexy.TodLib.Attachment> mAttachments = theEffectSystem.mAttachmentHolder.mAttachments;
		for (int l = 0; l < mAttachments.Count; l++)
		{
			mAttachments[l].SyncObj(theContext);
		}
		List<TodParticleSystem> mParticleSystems = theEffectSystem.mParticleHolder.mParticleSystems;
		for (int m = 0; m < mParticleSystems.Count; m++)
		{
			mParticleSystems[m].SyncObj(theContext);
		}
	}

	public static void SyncObj(this TodParticle theParticle, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
		SyncObjFromList(ref theParticle.mParticleEmitter_Save, ref theParticle.mParticleEmitter, gLawnApp.mEffectSystem.mParticleHolder.mEmitters, theContext.mReading);
		SyncObjFromList(ref theParticle.mCrossFadeParticleID_Save, ref theParticle.mCrossFadeParticleID, gLawnApp.mEffectSystem.mParticleHolder.mParticles, theContext.mReading);
	}

	public static void SyncObj(this TodParticleEmitter theParticleEmitter, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
		SyncObjFromList(ref theParticleEmitter.mParticleSystem_Save, ref theParticleEmitter.mParticleSystem, gLawnApp.mEffectSystem.mParticleHolder.mParticleSystems, theContext.mReading);
		SyncObjFromList(ref theParticleEmitter.mCrossFadeEmitterID_Save, ref theParticleEmitter.mCrossFadeEmitterID, gLawnApp.mEffectSystem.mParticleHolder.mEmitters, theContext.mReading);
		if (theContext.mReading)
		{
			if (theParticleEmitter.mEmitterDef_Save >= 0 && theParticleEmitter.mEmitterDef_Save < theParticleEmitter.mParticleSystem.mParticleDef.mEmitterDefs.Length)
			{
				theParticleEmitter.mEmitterDef = theParticleEmitter.mParticleSystem.mParticleDef.mEmitterDefs[theParticleEmitter.mEmitterDef_Save];
			}
			else
			{
				theParticleEmitter.mEmitterDef = null;
			}
		}
		else
		{
			theParticleEmitter.mEmitterDef_Save = ((IList<TodEmitterDefinition>)theParticleEmitter.mParticleSystem.mParticleDef.mEmitterDefs).IndexOf(theParticleEmitter.mEmitterDef);
		}
	}

	public static void SyncObj(this Reanimation theReanimation, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
		for (int i = 0; i < theReanimation.mTrackInstances.Length; i++)
		{
			ReanimatorTrackInstance reanimatorTrackInstance = theReanimation.mTrackInstances[i];
			if (reanimatorTrackInstance != null)
			{
				SyncObjFromList(ref reanimatorTrackInstance.mAttachmentID_Save, ref reanimatorTrackInstance.mAttachmentID, gLawnApp.mEffectSystem.mAttachmentHolder.mAttachments, theContext.mReading);
			}
		}
		if (theContext.mVersion < SaveFileVersion.NewLayout600 && !theReanimation.mIsAttachment)
		{
			theReanimation.SetPosition(theReanimation.mOverlayMatrix.mMatrix.Translation.X * Constants.ISUI + (float)Constants.BoardPosChange, theReanimation.mOverlayMatrix.mMatrix.Translation.Y * Constants.ISUI);
		}
	}

	public static void SyncObj(this Sexy.TodLib.Attachment theAttachment, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		LawnApp gLawnApp = GlobalStaticVars.gLawnApp;
		for (int i = 0; i < theAttachment.mEffectArray.Length; i++)
		{
			AttachEffect attachEffect = theAttachment.mEffectArray[i];
			if (attachEffect != null)
			{
				if (attachEffect.mEffectType == EffectType.Particle)
				{
					TodParticleSystem obj = attachEffect.mEffectID as TodParticleSystem;
					SyncObjFromList(ref attachEffect.mEffectID_Save, ref obj, gLawnApp.mEffectSystem.mParticleHolder.mParticleSystems, theContext.mReading);
					attachEffect.mEffectID = obj;
				}
				else if (attachEffect.mEffectType == EffectType.Trail)
				{
					Trail obj2 = attachEffect.mEffectID as Trail;
					SyncObjFromList(ref attachEffect.mEffectID_Save, ref obj2, gLawnApp.mEffectSystem.mTrailHolder.mTrails, theContext.mReading);
					attachEffect.mEffectID = obj2;
				}
				else if (attachEffect.mEffectType == EffectType.Reanim)
				{
					Reanimation obj3 = attachEffect.mEffectID as Reanimation;
					SyncObjFromList(ref attachEffect.mEffectID_Save, ref obj3, gLawnApp.mEffectSystem.mReanimationHolder.mReanimations, theContext.mReading);
					attachEffect.mEffectID = obj3;
				}
				else if (attachEffect.mEffectType == EffectType.Attachment)
				{
					Sexy.TodLib.Attachment obj4 = attachEffect.mEffectID as Sexy.TodLib.Attachment;
					SyncObjFromList(ref attachEffect.mEffectID_Save, ref obj4, gLawnApp.mEffectSystem.mAttachmentHolder.mAttachments, theContext.mReading);
					attachEffect.mEffectID = obj4;
				}
				else
				{
					Debug.ASSERT(value: false);
				}
			}
		}
	}

	public static void SyncObj(this TodParticleSystem theParticleSystem, GlobalMembersSaveGame.SaveGameContext theContext)
	{
		if (theContext.mVersion < SaveFileVersion.NewLayout600 && !theParticleSystem.mIsAttachment)
		{
			theParticleSystem.SystemMoveBy(Constants.BoardPosChange, 0f);
		}
	}

	public static void SyncObjFromList<T>(ref int id, ref T obj, List<T> lst, bool id2obj)
	{
		if (id2obj)
		{
			if (id >= 0 && id < lst.Count)
			{
				obj = lst[id];
			}
			else
			{
				obj = default;
			}
		}
		else
		{
			id = ((obj == null) ? (-1) : lst.IndexOf(obj));
		}
	}
}
