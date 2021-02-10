using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveHeaderFlags.
	/// </summary>
	[Flags]
	public enum WaveHeaderFlags : int
	{
		Done			=	0x00000001,  /* done bit */
		Prepared		=	0x00000002,  /* set if this header has been prepared */
		BeginLoop		=	0x00000004,  /* loop start block */
		EndLoop			=	0x00000008,  /* loop end block */
		InQueue			=	0x00000010  /* reserved for driver */
	}
}
