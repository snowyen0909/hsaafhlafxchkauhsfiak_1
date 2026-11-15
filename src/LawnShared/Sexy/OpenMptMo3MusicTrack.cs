using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Sexy;

internal sealed class OpenMptMo3MusicTrack : IDisposable
{
	private sealed class SubmittedAudioBuffer
	{
		public byte[] Data;

		public uint PackedOrder;
	}

	private const int SampleRate = 44100;

	private const int Channels = 2;

	private const int BytesPerSample = 2;

	// ~46 ms per buffer, ~370 ms queued. The old 512-frame x 12 queue held only ~140 ms: once the frame rate
	// dropped (busy waves, thermal throttling after a while) the stream starved and turned into crackling noise,
	// and it churned ~86 OpenAL buffers per second.
	private const int FramesPerBuffer = 2048;

	private const int TargetPendingBuffers = 8;

	private const int BytesPerBuffer = FramesPerBuffer * Channels * BytesPerSample;

	private readonly OpenMptModule mModule;

	private readonly short[] mDecodeBuffer = new short[FramesPerBuffer * Channels];

	private readonly Queue<SubmittedAudioBuffer> mSubmittedBuffers = new Queue<SubmittedAudioBuffer>();

	private readonly float[] mChannelVolumes;

	private DynamicSoundEffectInstance mSound;

	private bool mPlaying;

	private bool mPaused;

	private bool mReachedEnd;

	private bool mDisposed;

	private float mVolume = 0.5f;

	public bool IsPlaying => mPlaying;

	public bool IsPaused => mPaused;

	public OpenMptMo3MusicTrack(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		mModule = new OpenMptModule(data);
		if (!mModule.HasInteractive)
		{
			mModule.Dispose();
			throw new InvalidDataException("libopenmpt does not provide the interactive interface required by PvZ music");
		}
		mChannelVolumes = new float[mModule.GetChannelCount()];
		for (int i = 0; i < mChannelVolumes.Length; i++)
		{
			mChannelVolumes[i] = 1f;
		}
	}

	public void Play(bool loop)
	{
		PlayFromOrder(0, loop);
	}

	public void PlayFromOrder(int order, bool loop)
	{
		ThrowIfDisposed();
		EnsureSound();
		StopPlayback();
		mModule.SetRepeatCount(loop ? (-1) : 0);
		mModule.SetPositionOrderRow(Math.Max(0, order), 0);
		ApplyChannelVolumes();
		mReachedEnd = false;
		mPlaying = true;
		mPaused = false;
		FillPendingBuffers();
		if (mSound.PendingBufferCount > 0)
		{
			mSound.Play();
		}
		else
		{
			mPlaying = false;
		}
		LawnPlatform.PvZActivity.Log($"MO3 PlayFromOrder({order}, loop={loop}) pending={mSound.PendingBufferCount} state={mSound.State} vol={mVolume}");
	}

	private int mDiagCounter;

	public void Stop()
	{
		if (!mDisposed)
		{
			StopPlayback();
		}
	}

	public void Pause()
	{
		if (!mDisposed && mPlaying && !mPaused)
		{
			mSound.Pause();
			mPaused = true;
		}
	}

	public void Resume()
	{
		if (!mDisposed && mPlaying && mPaused)
		{
			mSound.Resume();
			mPaused = false;
		}
	}

	public void SetVolume(float volume)
	{
		mVolume = Math.Max(0f, Math.Min(1f, volume));
		if (mSound != null)
		{
			mSound.Volume = mVolume;
		}
	}

	public uint GetMusicOrder()
	{
		if (mSubmittedBuffers.Count > 0)
		{
			return mSubmittedBuffers.Peek().PackedOrder;
		}
		return mModule.GetPackedOrder();
	}

	public int GetMusicBPM()
	{
		return mModule.GetCurrentTempo();
	}

	public void SetMusicBPM(int bpm)
	{
		mModule.SetCurrentTempo(Math.Max(32, Math.Min(512, bpm)));
	}

	public int GetMusicSpeed()
	{
		return mModule.GetCurrentSpeed();
	}

	public void SetMusicSpeed(int speed)
	{
		mModule.SetCurrentSpeed(Math.Max(1, Math.Min(65535, speed)));
	}

	public void SetTrackVolume(int track, float volume)
	{
		if (track >= 0 && track < mChannelVolumes.Length)
		{
			float num = Math.Max(0f, Math.Min(1f, volume));
			mChannelVolumes[track] = num;
			ApplyChannelVolume(track, num);
		}
	}

	public void SetOrderPosition(int order)
	{
		mModule.SetPositionOrderRow(Math.Max(0, order), 0);
		ApplyChannelVolumes();
		FlushDecodedAudioAfterSeek();
	}

	public void SetDecodePosition(int position)
	{
		if (position == 0)
		{
			SetOrderPosition(0);
		}
	}

	public void SetAmplify(double amplify)
	{
		mModule.SetAmplify(amplify);
	}

	public void Update()
	{
		if (!mDisposed && mPlaying && !mPaused)
		{
			if (++mDiagCounter % 500 == 1)
			{
				LawnPlatform.PvZActivity.Log($"MO3 update: pending={mSound.PendingBufferCount} state={mSound.State} vol={mSound.Volume} order={mModule.GetPackedOrder()} reachedEnd={mReachedEnd}");
			}
			ReclaimCompletedBuffers();
			FillPendingBuffers();
			if (mReachedEnd && mSound.PendingBufferCount == 0)
			{
				StopPlayback();
			}
			else if (mSound.PendingBufferCount > 0 && mSound.State == SoundState.Stopped)
			{
				mSound.Play();
			}
		}
	}

	public void Dispose()
	{
		if (!mDisposed)
		{
			StopPlayback();
			if (mSound != null)
			{
				mSound.Dispose();
				mSound = null;
			}
			mModule.Dispose();
			mDisposed = true;
		}
	}

	private void EnsureSound()
	{
		if (mSound == null)
		{
			mSound = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Stereo);
			mSound.Volume = mVolume;
		}
	}

	private void FillPendingBuffers()
	{
		while (!mReachedEnd && mSound.PendingBufferCount < TargetPendingBuffers)
		{
			uint packedOrder = mModule.GetPackedOrder();
			int num = mModule.ReadInterleavedStereo(SampleRate, FramesPerBuffer, mDecodeBuffer);
			if (num <= 0)
			{
				mReachedEnd = true;
				break;
			}
			int count = num * Channels * BytesPerSample;
			byte[] array = ArrayPool<byte>.Shared.Rent(BytesPerBuffer);
			System.Buffer.BlockCopy(mDecodeBuffer, 0, array, 0, count);
			try
			{
				mSound.SubmitBuffer(array, 0, count);
				mSubmittedBuffers.Enqueue(new SubmittedAudioBuffer
				{
					Data = array,
					PackedOrder = packedOrder
				});
			}
			catch
			{
				ArrayPool<byte>.Shared.Return(array);
				throw;
			}
		}
	}

	private void ReclaimCompletedBuffers()
	{
		for (int num = mSubmittedBuffers.Count - mSound.PendingBufferCount; num > 0; num--)
		{
			ArrayPool<byte>.Shared.Return(mSubmittedBuffers.Dequeue().Data);
		}
	}

	private void FlushDecodedAudioAfterSeek()
	{
		if (mSound == null || !mPlaying)
		{
			return;
		}
		bool flag = mPaused;
		mSound.Stop();
		ReturnSubmittedBuffers();
		mReachedEnd = false;
		FillPendingBuffers();
		if (mSound.PendingBufferCount > 0)
		{
			mSound.Play();
			if (flag)
			{
				mSound.Pause();
			}
		}
	}

	private void ApplyChannelVolumes()
	{
		for (int i = 0; i < mChannelVolumes.Length; i++)
		{
			ApplyChannelVolume(i, mChannelVolumes[i]);
		}
	}

	private void ApplyChannelVolume(int channel, float volume)
	{
		mModule.SetChannelVolume(channel, volume);
		mModule.SetChannelMuteStatus(channel, volume <= 0.0001f);
	}

	private void StopPlayback()
	{
		if (mSound != null)
		{
			mSound.Stop();
		}
		ReturnSubmittedBuffers();
		mPlaying = false;
		mPaused = false;
		mReachedEnd = false;
	}

	private void ReturnSubmittedBuffers()
	{
		while (mSubmittedBuffers.Count > 0)
		{
			ArrayPool<byte>.Shared.Return(mSubmittedBuffers.Dequeue().Data);
		}
	}

	private void ThrowIfDisposed()
	{
		if (mDisposed)
		{
			throw new ObjectDisposedException("OpenMptMo3MusicTrack");
		}
	}
}
