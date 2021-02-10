using System;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveHeader.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct WaveHeader
	{
		public IntPtr			Data;			// WaveForm buffer
		public int				BufferLength;	// Length, in bytes, of the buffer
		public int				BytesRecorded;	// When header is used as input, specifies how much data is in the buffer
		public IntPtr			UserData;		// User data
		public WaveHeaderFlags	Flags;			// Flags supplying info about the buffer
		public int				Loops;			// Number of times to play loop
		public IntPtr			Next;			// Reserved
		public int				Reserved;		// Reserved
	}
}
