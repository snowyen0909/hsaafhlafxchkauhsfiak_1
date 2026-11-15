using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Sexy;

internal class XNASoundManager : SoundManager
{
	private const int ACTIVE_SOUNDS_LIMIT = 31;

	private int mNumSoundsRemoving;

	private int mReservedChannels;

	private const float SOUND_LIMIT = 0.65f;

	private float mMasterVolume = 0.65f;

	private bool mEnabled;

	private ContentManager mContent;

	private XNASoundEntry[] mSounds = new XNASoundEntry[XNASoundConstants.MAX_SOUNDS];

	private List<XNASoundInstance> mInstances;

	public XNASoundManager(SexyAppBase theApp)
	{
		mEnabled = false;
		mContent = theApp.mContentManager;
		mInstances = new List<XNASoundInstance>();
		SoundEffect.MasterVolume = 1f;
		for (int i = 0; i < XNASoundConstants.MAX_SOUNDS; i++)
		{
			mSounds[i] = null;
		}
	}

	public override void Release()
	{
		ReleaseSounds();
		for (int i = 0; i < mInstances.Count; i++)
		{
			mInstances[i].Release();
		}
	}

	public override void Enable(bool enable)
	{
		mEnabled = enable;
	}

	public override bool Initialized()
	{
		return true;
	}

	public override bool LoadSound(uint theSfxID, string theFilename)
	{
		SoundEffect mSound = mContent.Load<SoundEffect>(theFilename);
		if (mSound == null || mSound.IsDisposed)
		{
			LawnPlatform.PvZActivity.AudioLog($"sound load FAILED id={theSfxID} {theFilename} disposed={mSound?.IsDisposed}");
			return false;
		}
		XNASoundEntry previous = theSfxID < mSounds.Length ? mSounds[theSfxID] : null;
		if (previous != null && previous.mSound != mSound)
		{
			LawnPlatform.PvZActivity.AudioLog($"sound id {theSfxID} overwritten by {theFilename} (owned={previous.mOwned})");
			StopInstancesOf(theSfxID);
		}
		XNASoundEntry xNASoundEntry = new XNASoundEntry();
		xNASoundEntry.mSound = mSound;
		xNASoundEntry.mOwned = false;
		mSounds[(uint)(UIntPtr)theSfxID] = xNASoundEntry;
		return true;
	}

	public bool LoadSound(uint theSfxID, SoundEffect theSound)
	{
		XNASoundEntry xNASoundEntry = new XNASoundEntry();
		xNASoundEntry.mSound = theSound;
		xNASoundEntry.mOwned = true;
		LawnPlatform.PvZActivity.AudioLog($"voice loaded id={theSfxID} dur={theSound?.Duration}");
		mSounds[(uint)(UIntPtr)theSfxID] = xNASoundEntry;
		return true;
	}

	public override int LoadSound(string theFilename)
	{
		for (int i = 0; i < XNASoundConstants.MAX_SOUNDS; i++)
		{
			if (mInstances[i] == null)
			{
				if (LoadSound((uint)i, theFilename))
				{
					return i;
				}
				return -1;
			}
		}
		return -1;
	}

	public override void ReleaseSound(uint theSfxID)
	{
		if (theSfxID >= mSounds.Length || mSounds[theSfxID] == null)
		{
			return;
		}
		// stop instances of this sound first: freeing a buffer that a voice is still playing is what turned
		// level changes into silence / noise on iOS
		int stopped = StopInstancesOf(theSfxID);
		LawnPlatform.PvZActivity.AudioLog($"sound release id={theSfxID} owned={mSounds[theSfxID].mOwned} stoppedInstances={stopped}");
		mSounds[theSfxID].Dispose();
		mSounds[theSfxID] = null;
	}

	public override void SetVolume(double theVolume)
	{
		SetMasterVolume(theVolume);
	}

	public override bool SetBaseVolume(uint theSfxID, double theBaseVolume)
	{
		if (theBaseVolume < 0.0 || theBaseVolume > 1.0)
		{
			return false;
		}
		mSounds[(uint)(UIntPtr)theSfxID].mBaseVolume = (float)theBaseVolume;
		for (int i = 0; i < mInstances.Count; i++)
		{
			if (mInstances[i].SoundId == theSfxID)
			{
				mInstances[i].SetBaseVolume(mSounds[(uint)(UIntPtr)theSfxID].mBaseVolume);
			}
		}
		return true;
	}

	public override bool SetBasePan(uint theSfxID, int theBasePan)
	{
		if (theBasePan < -100 || theBasePan > 100)
		{
			return false;
		}
		mSounds[(uint)(UIntPtr)theSfxID].mBasePan = (float)theBasePan / 100f;
		for (int i = 0; i < mInstances.Count; i++)
		{
			if (mInstances[i].SoundId == theSfxID)
			{
				mInstances[i].SetBasePan(theBasePan);
			}
		}
		return true;
	}

	public override SoundInstance GetSoundInstance(uint theSfxID)
	{
		if (mInstances.Count >= 31 - mReservedChannels - mNumSoundsRemoving)
		{
			return null;
		}
		XNASoundEntry entry = theSfxID < mSounds.Length ? mSounds[theSfxID] : null;
		if (entry?.mSound == null || entry.mSound.IsDisposed)
		{
			return null;
		}
		SoundEffectInstance instance;
		try
		{
			instance = entry.mSound.CreateInstance();
		}
		catch (Exception ex)
		{
			LawnPlatform.PvZActivity.Log("sound instance failed: " + ex.Message);
			return null;
		}
		XNASoundInstance newXNASoundInstance = XNASoundInstance.GetNewXNASoundInstance(theSfxID, instance);
		newXNASoundInstance.SetMasterVolume(mMasterVolume);
		mInstances.Add(newXNASoundInstance);
		return newXNASoundInstance;
	}

	public override bool TryReserveChannels(int count)
	{
		if (count <= 0)
		{
			throw new ArgumentOutOfRangeException("count", "The channel count must be positive.");
		}
		if (count > 31 - mReservedChannels - mInstances.Count - mNumSoundsRemoving)
		{
			return false;
		}
		mReservedChannels += count;
		return true;
	}

	public override void ReleaseReservedChannels(int count)
	{
		if (count <= 0 || count > mReservedChannels)
		{
			throw new ArgumentOutOfRangeException("count", "The channel count must be positive and no greater than the reserved total.");
		}
		mReservedChannels -= count;
	}

	public override void ReleaseSounds()
	{
		for (uint num = 0u; num < XNASoundConstants.MAX_SOUNDS; num++)
		{
			ReleaseSound(num);
		}
	}

	public override void ReleaseChannels()
	{
	}

	public override double GetMasterVolume()
	{
		return mMasterVolume / 0.65f;
	}

	public override void SetMasterVolume(double theVolume)
	{
		float num = (mMasterVolume = (float)Math.Min(Math.Max(theVolume, 0.0), 1.0) * 0.65f);
		SoundEffect.MasterVolume = 1f;
		for (int i = 0; i < mInstances.Count; i++)
		{
			mInstances[i].SetMasterVolume(num);
		}
	}

	public override void Flush()
	{
	}

	public override void StopAllSounds()
	{
		for (int i = 0; i < mInstances.Count; i++)
		{
			mInstances[i].Stop();
		}
	}

	private int StopInstancesOf(uint theSfxID)
	{
		int n = 0;
		for (int i = 0; i < mInstances.Count; i++)
		{
			if (mInstances[i].SoundId == theSfxID && !mInstances[i].IsReleased())
			{
				mInstances[i].Stop();
				mInstances[i].Release();
				n++;
			}
		}
		return n;
	}

	public override int GetFreeSoundId()
	{
		for (int i = 0; i < XNASoundConstants.MAX_SOUNDS; i++)
		{
			if (mSounds[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public override int GetNumSounds()
	{
		int num = 0;
		for (int i = 0; i < XNASoundConstants.MAX_SOUNDS; i++)
		{
			if (mSounds[i] != null)
			{
				num++;
			}
		}
		return num;
	}

	private int mDiagTicks;

	public override void Update()
	{
		if (++mDiagTicks >= 500)
		{
			mDiagTicks = 0;
			int playing = 0;
			for (int i = 0; i < mInstances.Count; i++)
			{
				if (mInstances[i].IsPlaying())
				{
					playing++;
				}
			}
			LawnPlatform.PvZActivity.AudioLog($"sfx status: instances={mInstances.Count} playing={playing}");
		}
		mNumSoundsRemoving = 0;
		for (int num = mInstances.Count - 1; num >= 0; num--)
		{
			if (mInstances[num].IsReleased())
			{
				mInstances[num].PrepareForReuse();
				mInstances.RemoveAt(num);
				mNumSoundsRemoving++;
			}
			else if (mInstances[num].IsDormant() || mInstances[num].IsBroken())
			{
				if (!mInstances[num].IsReleased())
				{
					mInstances[num].Release();
				}
				mInstances[num].PrepareForReuse();
				mInstances.RemoveAt(num);
				mNumSoundsRemoving++;
			}
		}
	}
}
