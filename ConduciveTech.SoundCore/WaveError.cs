using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveError.
	/// </summary>
	public sealed class WaveError
	{
		private const int MMSYSERR_BASE = 0;
		public const int MMSYSERR_NOERROR      =  0;                    /* no error */
		public const int MMSYSERR_ERROR         = (MMSYSERR_BASE + 1);  /* unspecified error */
		public const int MMSYSERR_BADDEVICEID   = (MMSYSERR_BASE + 2);  /* device ID out of range */
		public const int MMSYSERR_NOTENABLED    = (MMSYSERR_BASE + 3);  /* driver failed enable */
		public const int MMSYSERR_ALLOCATED     = (MMSYSERR_BASE + 4);  /* device already allocated */
		public const int MMSYSERR_INVALHANDLE   = (MMSYSERR_BASE + 5);  /* device handle is invalid */
		public const int MMSYSERR_NODRIVER      = (MMSYSERR_BASE + 6);  /* no device driver present */
		public const int MMSYSERR_NOMEM         = (MMSYSERR_BASE + 7);  /* memory allocation error */
		public const int MMSYSERR_NOTSUPPORTED  = (MMSYSERR_BASE + 8);  /* function isn't supported */
		public const int MMSYSERR_BADERRNUM     = (MMSYSERR_BASE + 9);  /* error value out of range */
		public const int MMSYSERR_INVALFLAG     = (MMSYSERR_BASE + 10); /* invalid flag passed */
		public const int MMSYSERR_INVALPARAM    = (MMSYSERR_BASE + 11); /* invalid parameter passed */
		public const int MMSYSERR_HANDLEBUSY    = (MMSYSERR_BASE + 12); /* handle being used */
																		/* simultaneously on another */
																		/* thread  = (eg callback); */
		public const int MMSYSERR_INVALIDALIAS  = (MMSYSERR_BASE + 13); /* specified alias not found */
		public const int MMSYSERR_BADDB         = (MMSYSERR_BASE + 14); /* bad registry database */
		public const int MMSYSERR_KEYNOTFOUND   = (MMSYSERR_BASE + 15); /* registry key not found */
		public const int MMSYSERR_READERROR     = (MMSYSERR_BASE + 16); /* registry read error */
		public const int MMSYSERR_WRITEERROR    = (MMSYSERR_BASE + 17); /* registry write error */
		public const int MMSYSERR_DELETEERROR   = (MMSYSERR_BASE + 18); /* registry delete error */
		public const int MMSYSERR_VALNOTFOUND   = (MMSYSERR_BASE + 19); /* registry value not found */
		public const int MMSYSERR_NODRIVERCB    = (MMSYSERR_BASE + 20); /* driver does not call DriverCallback */
		public const int MMSYSERR_MOREDATA      = (MMSYSERR_BASE + 21); /* more data to be returned */

		private WaveError()
		{}

		public static string GetMessage( int waveError )
		{
			switch(waveError)
			{
				case WaveError.MMSYSERR_NOERROR:
					return "No Error";
				case WaveError.MMSYSERR_ERROR:
					return "Unspecified Error";
				case WaveError.MMSYSERR_BADDEVICEID:
					return "Device ID out of range.";
				case WaveError.MMSYSERR_NOTENABLED:
					return "Driver failed enable.";
				case WaveError.MMSYSERR_ALLOCATED:
					return "Device already allocated.";
				case WaveError.MMSYSERR_INVALHANDLE:
					return "Device handle is invalid.";
				case WaveError.MMSYSERR_NODRIVER:
					return "No device driver present.";
				case WaveError.MMSYSERR_NOMEM:
					return "Memory allocation error.";
				case WaveError.MMSYSERR_NOTSUPPORTED:
					return "Function is not supported.";
				case WaveError.MMSYSERR_BADERRNUM:
					return "Error value out of range.";
				case WaveError.MMSYSERR_INVALFLAG:
					return "Invalid flag passed.";
				case WaveError.MMSYSERR_INVALPARAM:
					return "Invalid parameter passed.";
				case WaveError.MMSYSERR_HANDLEBUSY:
					return "Handle is in use by another tread.";
				case WaveError.MMSYSERR_INVALIDALIAS:
					return "Specified alias not found.";
				case WaveError.MMSYSERR_BADDB:
					return "Bad registry database.";
				case WaveError.MMSYSERR_KEYNOTFOUND:
					return "Registry key not found.";
				case WaveError.MMSYSERR_READERROR:
					return "Registry read error.";
				case WaveError.MMSYSERR_WRITEERROR:
					return "Registry write error.";
				case WaveError.MMSYSERR_DELETEERROR:
					return "Registry delete error.";
				case WaveError.MMSYSERR_VALNOTFOUND:
					return "Registry value not found.";
				case WaveError.MMSYSERR_NODRIVERCB:
					return "Driver does not call DriverCallback.";
				case WaveError.MMSYSERR_MOREDATA:
					return "More data to be returned.";
				default:
					return string.Format( "Unknown error: Error number out of range.\nError Number: 0x{0:x8}", waveError);
			}
		}
	}
}
