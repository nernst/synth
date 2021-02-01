using System;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveFormNative.
	/// </summary>
	internal sealed class WaveFormNative
	{
		private const string WaveFormLibrary = "winmm.dll";

		#region Constructors
		private WaveFormNative(){}
		#endregion

        // Import Native Library Functions
		[DllImport(WaveFormLibrary)]
		public static extern int waveOutGetNumDevs();

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutOpen(out IntPtr device, int deviceID, ref WaveFormatEx format, WaveOutProcDelegate callback, int instance, int flags);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutClose(IntPtr device);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutReset(IntPtr device);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutPrepareHeader(IntPtr device, ref WaveHeader header, int size);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutUnprepareHeader(IntPtr device, ref WaveHeader header, int size);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutWrite(IntPtr device, ref WaveHeader header, int size);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutPause(IntPtr device);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutRestart(IntPtr device);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutGetPosition(IntPtr device, out int info, int size);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutSetVolume(IntPtr device, int volume);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutGetVolume(IntPtr device, out int volume);

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutGetErrorText( int errorNumber, IntPtr buffer, int size );

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutGetDevCaps( int device, ref WaveOutCapabilities cap, int size );

		[DllImport(WaveFormLibrary)]
		public static extern int waveOutSetPlaybackRate( IntPtr device, int rate );
	}
}
