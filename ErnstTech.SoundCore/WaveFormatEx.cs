using System;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <remarks>
	/// <see>ms-help://MS.PSDK.1033/multimed/mmstr_625u.htm</see>
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	public struct WaveFormatEx
	{
		public FormatTag format;
		public short nChannels;
		public int nSamplesPerSec;
		public int nAvgBytesPerSec;
		public short nBlockAlign;
		public short nBitsPerSample;
		public int cbSize;
	}
}