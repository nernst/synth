using System;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for SoundCoreException.
	/// </summary>
	public class SoundCoreException : Exception
	{
		private int _NativeError;
		public int NativeError
		{
			get{ return _NativeError; }
		}

		private string _Detail;
		public string Detail
		{
			get { return _Detail == null ? string.Empty : _Detail; }
		}

		public override string Message
		{
			get
			{
				return string.Format( "{0}\n{1}", base.Message, Detail );
			}
		}

		public SoundCoreException( string message ) : this(message, 0)
		{
		}

		public SoundCoreException( string message, int nativeError ) : base( message )
		{
			_NativeError = nativeError;

			if ( nativeError != 0 )
			{
				byte[] buffer = new byte[512];
				
				GCHandle handle = GCHandle.Alloc( buffer, GCHandleType.Pinned );

				IntPtr ptr = handle.AddrOfPinnedObject();

				WaveFormNative.waveOutGetErrorText( nativeError, ptr, buffer.Length );

				_Detail = System.Text.ASCIIEncoding.ASCII.GetString( buffer );

				handle.Free();
			}
		}
	}
}
