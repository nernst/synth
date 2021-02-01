using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveDeviceVersion.
	/// </summary>
	public struct WaveDeviceDriverVersion
	{
		public short Version;
		public byte Major
		{
			get{ return (byte)(Version >> 8); }
		}

		public byte Minor
		{
			get{ return (byte)Version; }
		}
	}
}
