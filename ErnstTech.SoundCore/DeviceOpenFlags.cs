using System;

namespace ErnstTech.SoundCore
{
	[Flags]
	public enum DeviceOpenFlags : int
	{
		CallbackEvent		= 0x00050000,
		CallbackFunction	= 0x00030000,
		CallbackNull		= 0x00000000,
		CallbackThread		= 0x00020000,
		CallbackWindow		= 0x00010000,
		WaveAllowSync		= 0x00000002,
		WaveFormatDirect	= 0x00000008,
		WaveFormatQuery		= 0x00000001,
		WaveMapped			= 0x00000004,
		WaveFormatDirectQuery	= WaveFormatQuery | WaveFormatDirect
	}
}