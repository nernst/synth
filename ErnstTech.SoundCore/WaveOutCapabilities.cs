using System;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveOutCapabilities.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct WaveOutCapabilities
	{
		public short ManufacturerID;
		public short ProductID;
		public WaveDeviceDriverVersion DriverVersion;
		public string Product;
		public int Formats;
		public short Channels;
		public short Reserved;
		public int Support;
	}
}
