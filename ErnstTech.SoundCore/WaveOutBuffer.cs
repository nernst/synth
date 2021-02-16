using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveOutBuffer.
	/// </summary>
	public class WaveOutBuffer : IDisposable
	{
		private AutoResetEvent _HasData = new AutoResetEvent( false );

		private GCHandle _DataHandle;
		private GCHandle _HeaderHandle;

		private WaveHeader _Header;

		public IntPtr Data
		{
			get{ return _DataHandle.AddrOfPinnedObject(); }
		}

		private int _Length;
		public int Length
		{
			get{ return _Length; }
		}

		private bool _IsEmpty = true;
		public bool IsEmpty
		{
			get{ return _IsEmpty; }
		}

		private IntPtr _DeviceHandle;

		private WavePlayer _Player;

		private WaveOutBuffer _NextBuffer = null;
		public WaveOutBuffer NextBuffer
		{
			get{ return _NextBuffer; }
			set{ _NextBuffer = value; }
		}

		/// <summary>
		/// Creates and prepares a WaveOutBuffer.
		/// </summary>
		/// <param name="device">Handle to WaveOut device to prepare the buffer for</param>
		/// <param name="size">Size of the buffer, in bytes</param>
		public WaveOutBuffer( WavePlayer player, IntPtr device, int size )
		{
			if ( player == null )
				throw new ArgumentNullException( "player" );
			_Player = player;

			if ( device == IntPtr.Zero )
				throw new ArgumentException( "Device must be a a valid pointer to a wave out device.", "device" );

			if ( size < 1 )
				throw new ArgumentOutOfRangeException( "size", size, "Size must be greater than zero." );

			_Length = size;
			_DeviceHandle = device;

			// Allocate memory for the buffer, and set up a GCHandle pointed to it.
			byte[] buffer = new byte[Length];
			_DataHandle = GCHandle.Alloc( buffer, GCHandleType.Pinned );

			// Create the header and a GC handle pointed to it.
			_Header	= new WaveHeader();
			_HeaderHandle = GCHandle.Alloc( _Header, GCHandleType.Pinned );
			
			_Header.Data = this.Data;
			_Header.BufferLength = Length;

			_Header.UserData = (IntPtr)GCHandle.Alloc( this );
			_Header.Loops = 0;
			_Header.Flags = 0;

			int result = WaveFormNative.waveOutPrepareHeader( _DeviceHandle, 
				ref _Header, Marshal.SizeOf( _Header ) );

			if ( result != WaveError.MMSYSERR_NOERROR )
				throw new SoundCoreException( WaveError.GetMessage( result ), result );
		}

		public void Play()
		{
			// Make sure we have data
			this.WaitForBufferFull();

			WaveFormNative.waveOutWrite( _DeviceHandle, ref _Header, Marshal.SizeOf( _Header ) );

			_IsEmpty = true;
		}

		public static void WaveOutCallback(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHeader wavhdr, int dwParam2)
		{
			if ( uMsg == (int)WaveFormOutputMessage.Done )
			{
				GCHandle handle = (GCHandle)wavhdr.UserData;
				WaveOutBuffer buffer = (WaveOutBuffer)handle.Target;

				if ( buffer._Player.IsPlaying )
				{
					buffer.NextBuffer.Play();

#if DEBUG
					System.Diagnostics.Debug.WriteLine( string.Format( "FillBuffer start at {0} ticks.", DateTime.Now.Ticks ) );
#endif

					buffer._Player.FillBuffer( buffer );

#if DEBUG
					System.Diagnostics.Debug.WriteLine( string.Format( "FillBuffer end   at {0} ticks.", DateTime.Now.Ticks ) );
#endif
					buffer._IsEmpty = false;
					buffer._HasData.Set();
				}
				else
				{
					buffer._Player.SignalDone();
				}
			}
		}

		public void WaitForBufferFull()
		{
			if ( IsEmpty )
				_HasData.WaitOne();
			else
				Thread.Sleep(0);
		}

		/// <summary>
		/// Allows an external object to signal that it has filled the buffer.
		/// </summary>
		public void BufferFilled()
		{
			this._IsEmpty = false;
			this._HasData.Set();
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.NextBuffer?.Dispose();
			WaveFormNative.waveOutUnprepareHeader( _DeviceHandle, ref _Header, Marshal.SizeOf( _Header ) );

			_DataHandle.Free();
			_HeaderHandle.Free();
		}

		#endregion
	}
}
