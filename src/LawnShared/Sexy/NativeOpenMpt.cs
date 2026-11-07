using System;
using System.Runtime.InteropServices;

namespace Sexy;

internal static class NativeOpenMpt
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int SetCurrentValueDelegate(IntPtr moduleExt, int value);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int SetChannelVolumeDelegate(IntPtr moduleExt, int channel, double volume);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int SetChannelMuteStatusDelegate(IntPtr moduleExt, int channel, int mute);

	public struct InteractiveInterface
	{
		public IntPtr SetCurrentSpeed;

		public IntPtr SetCurrentTempo;

		public IntPtr SetTempoFactor;

		public IntPtr GetTempoFactor;

		public IntPtr SetPitchFactor;

		public IntPtr GetPitchFactor;

		public IntPtr SetGlobalVolume;

		public IntPtr GetGlobalVolume;

		public IntPtr SetChannelVolume;

		public IntPtr GetChannelVolume;

		public IntPtr SetChannelMuteStatus;

		public IntPtr GetChannelMuteStatus;

		public IntPtr SetInstrumentMuteStatus;

		public IntPtr GetInstrumentMuteStatus;

		public IntPtr PlayNote;

		public IntPtr StopNote;
	}

	public const int OpenMptModuleRenderMasterGainMillibel = 1;

	private const string LibraryName = "libopenmpt";

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr openmpt_module_ext_create_from_memory([In] byte[] fileData, UIntPtr fileSize, IntPtr logFunction, IntPtr logUser, IntPtr errorFunction, IntPtr errorUser, out int error, out IntPtr errorMessage, IntPtr controls);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern void openmpt_module_ext_destroy(IntPtr moduleExt);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr openmpt_module_ext_get_module(IntPtr moduleExt);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_ext_get_interface(IntPtr moduleExt, [MarshalAs(UnmanagedType.LPUTF8Str)] string interfaceId, ref InteractiveInterface moduleInterface, UIntPtr interfaceSize);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern UIntPtr openmpt_module_read_interleaved_stereo(IntPtr module, int sampleRate, UIntPtr frameCount, [Out] short[] output);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_set_repeat_count(IntPtr module, int repeatCount);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_get_current_order(IntPtr module);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_get_current_row(IntPtr module);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_get_current_tempo(IntPtr module);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_get_current_speed(IntPtr module);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_get_num_channels(IntPtr module);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern double openmpt_module_set_position_order_row(IntPtr module, int order, int row);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_ctl_set_boolean(IntPtr module, [MarshalAs(UnmanagedType.LPUTF8Str)] string control, int value);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern int openmpt_module_set_render_param(IntPtr module, int parameter, int value);

	[DllImport("libopenmpt", CallingConvention = CallingConvention.Cdecl)]
	public static extern void openmpt_free_string(IntPtr text);
}
