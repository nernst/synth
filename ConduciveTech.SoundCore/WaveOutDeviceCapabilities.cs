using System;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveOutDeviceCapabilities.
	/// </summary>
	public class WaveOutDeviceCapabilities
	{
		[StructLayout(LayoutKind.Sequential)]
		private class WAVEOUTCAPS
		{
			private const int MAXPNAMELEN = 32;

			public short ManufacturerID = 0;
			public short ProductID = 0;
			public short DriverVersion = 0;
			public char[] ProductName = new char[MAXPNAMELEN];
			public int Formats = 0;
			public short Channels = 0;
			public short Reserved = 0;
			public int SupportedFunctionality = 0;
		}

		[DllImport( "winmm.dll", EntryPoint="waveOutGetDevCapsW", SetLastError=true, CharSet=CharSet.Unicode )]
		private static extern int waveOutGetDevCaps( int device, WAVEOUTCAPS caps, int size );

		private WAVEOUTCAPS _Capabilities;

		public WaveOutDeviceCapabilities( int device )
		{
			this._Capabilities = new WAVEOUTCAPS();

			int result = waveOutGetDevCaps( device, this._Capabilities, Marshal.SizeOf( this._Capabilities ) );

			if ( result != WaveError.MMSYSERR_NOERROR )
				throw new SoundCoreException( WaveError.GetMessage( result ), result );
		}
	}
}
