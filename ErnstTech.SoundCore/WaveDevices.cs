using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveDevices.
	/// </summary>
	public sealed class WaveDevices
	{
		private WaveDevices()
		{}

		public int GetDeviceCount()
		{
			return WaveFormNative.waveOutGetNumDevs();

		}
	}
}
