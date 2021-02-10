using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveBufferEmptyEvent.
	/// </summary>
	public class WaveBufferEmptyEventArgs : EventArgs
	{
		private IntPtr _Buffer;
		public IntPtr Buffer
		{
			get{ return _Buffer; }
		}

		private int _Length;
		public int Length
		{
			get{ return _Length; }
		}

		private int _BytesWritten = -1;
		public int BytesWritten
		{
			get{ return _BytesWritten; }
			set{ _BytesWritten = value; }
		}

		/// <summary>
		/// Initializes the event arguments.
		/// </summary>
		/// <param name="buffer">Pointer to the data buffer that is empty.</param>
		/// <param name="length">Length of the buffer, in bytes.</param>
		public WaveBufferEmptyEventArgs( IntPtr buffer, int length )
		{
			if ( buffer == IntPtr.Zero )
				throw new ArgumentException( "buffer", "Buffer must be a valid pointer." );

			_Buffer = buffer;
			_Length = length;
		}
	}
}
