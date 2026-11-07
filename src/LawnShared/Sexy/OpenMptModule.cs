using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Sexy;

internal sealed class OpenMptModule : IDisposable
{
	private IntPtr mModule;

	private IntPtr mModuleExt;

	private NativeOpenMpt.SetCurrentValueDelegate mSetCurrentSpeed;

	private NativeOpenMpt.SetCurrentValueDelegate mSetCurrentTempo;

	private NativeOpenMpt.SetChannelVolumeDelegate mSetChannelVolume;

	private NativeOpenMpt.SetChannelMuteStatusDelegate mSetChannelMuteStatus;

	public bool HasInteractive { get; private set; }

	public OpenMptModule(byte[] data)
	{
		mModuleExt = NativeOpenMpt.openmpt_module_ext_create_from_memory(data, (UIntPtr)(ulong)data.Length, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, out var error, out var errorMessage, IntPtr.Zero);
		string text = null;
		if (errorMessage != IntPtr.Zero)
		{
			text = Marshal.PtrToStringUTF8(errorMessage);
			NativeOpenMpt.openmpt_free_string(errorMessage);
		}
		if (mModuleExt == IntPtr.Zero)
		{
			throw new InvalidDataException("libopenmpt failed to load MO3 (error " + error + ")" + (string.IsNullOrEmpty(text) ? string.Empty : (": " + text)));
		}
		try
		{
			mModule = NativeOpenMpt.openmpt_module_ext_get_module(mModuleExt);
			if (mModule == IntPtr.Zero)
			{
				throw new InvalidDataException("libopenmpt returned an empty module handle");
			}
			if (NativeOpenMpt.openmpt_module_ctl_set_boolean(mModule, "seek.sync_samples", 0) == 0)
			{
				throw new InvalidOperationException("libopenmpt failed to disable sample synchronization while seeking");
			}
			NativeOpenMpt.InteractiveInterface moduleInterface = default;
			HasInteractive = NativeOpenMpt.openmpt_module_ext_get_interface(mModuleExt, "interactive", ref moduleInterface, (UIntPtr)(ulong)Marshal.SizeOf(typeof(NativeOpenMpt.InteractiveInterface))) != 0;
			if (HasInteractive)
			{
				mSetCurrentSpeed = GetDelegate<NativeOpenMpt.SetCurrentValueDelegate>(moduleInterface.SetCurrentSpeed);
				mSetCurrentTempo = GetDelegate<NativeOpenMpt.SetCurrentValueDelegate>(moduleInterface.SetCurrentTempo);
				mSetChannelVolume = GetDelegate<NativeOpenMpt.SetChannelVolumeDelegate>(moduleInterface.SetChannelVolume);
				mSetChannelMuteStatus = GetDelegate<NativeOpenMpt.SetChannelMuteStatusDelegate>(moduleInterface.SetChannelMuteStatus);
				HasInteractive = mSetCurrentSpeed != null && mSetCurrentTempo != null && mSetChannelVolume != null && mSetChannelMuteStatus != null;
			}
		}
		catch
		{
			Dispose();
			throw;
		}
	}

	public int ReadInterleavedStereo(int sampleRate, int frameCount, short[] output)
	{
		checked
		{
			return (int)NativeOpenMpt.openmpt_module_read_interleaved_stereo(mModule, sampleRate, (UIntPtr)unchecked((ulong)frameCount), output).ToUInt64();
		}
	}

	public void SetRepeatCount(int repeatCount)
	{
		NativeOpenMpt.openmpt_module_set_repeat_count(mModule, repeatCount);
	}

	public uint GetPackedOrder()
	{
		int num = NativeOpenMpt.openmpt_module_get_current_order(mModule);
		int num2 = NativeOpenMpt.openmpt_module_get_current_row(mModule);
		if (num < 0 || num2 < 0)
		{
			return uint.MaxValue;
		}
		uint num3 = (uint)(num2 * 4);
		return (num3 << 16) | (uint)(num & 0xFFFF);
	}

	public int GetCurrentTempo()
	{
		return NativeOpenMpt.openmpt_module_get_current_tempo(mModule);
	}

	public void SetCurrentTempo(int tempo)
	{
		if (mSetCurrentTempo != null)
		{
			mSetCurrentTempo(mModuleExt, tempo);
		}
	}

	public int GetCurrentSpeed()
	{
		return NativeOpenMpt.openmpt_module_get_current_speed(mModule);
	}

	public void SetCurrentSpeed(int speed)
	{
		if (mSetCurrentSpeed != null)
		{
			mSetCurrentSpeed(mModuleExt, speed);
		}
	}

	public int GetChannelCount()
	{
		return NativeOpenMpt.openmpt_module_get_num_channels(mModule);
	}

	public void SetPositionOrderRow(int order, int row)
	{
		NativeOpenMpt.openmpt_module_set_position_order_row(mModule, order, row);
	}

	public void SetChannelVolume(int channel, double volume)
	{
		if (mSetChannelVolume != null)
		{
			mSetChannelVolume(mModuleExt, channel, volume);
		}
	}

	public void SetChannelMuteStatus(int channel, bool mute)
	{
		if (mSetChannelMuteStatus != null)
		{
			mSetChannelMuteStatus(mModuleExt, channel, mute ? 1 : 0);
		}
	}

	public void SetAmplify(double amplify)
	{
		int value = (int)Math.Round(2000.0 * Math.Log10(Math.Max(0.01, amplify)));
		NativeOpenMpt.openmpt_module_set_render_param(mModule, 1, value);
	}

	public void Dispose()
	{
		if (mModuleExt != IntPtr.Zero)
		{
			NativeOpenMpt.openmpt_module_ext_destroy(mModuleExt);
			mModuleExt = IntPtr.Zero;
			mModule = IntPtr.Zero;
		}
	}

	private static T GetDelegate<T>(IntPtr functionPointer) where T : class
	{
		if (functionPointer == IntPtr.Zero)
		{
			return null;
		}
		return Marshal.GetDelegateForFunctionPointer(functionPointer, typeof(T)) as T;
	}
}
