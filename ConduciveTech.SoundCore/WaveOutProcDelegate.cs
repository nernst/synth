using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveOutProcDelegate.
	/// </summary>
	public delegate void WaveOutProcDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHeader wavhdr, int dwParam2);
}
