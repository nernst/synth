using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Wraps the WaveHeaderEx structure
	/// </summary>
	internal class WaveOutBuffer : IDisposable
	{
		private WaveOutBuffer _NextBuffer;
		public WaveOutBuffer NextBuffer
		{
			get{ return _NextBuffer; }
			set{ _NextBuffer = value; }
		}

		private EventHandler _Completed;
		public event EventHandler Completed
		{
			add{ _Completed += value; }
			remove{ _Completed -= value; }
		}

		private ManualResetEvent _WaitForCompletion = new ManualResetEvent( false );
		private ManualResetEvent _WaitForEmpty = new ManualResetEvent( true );

//		private ManualResetEvent _PlayEvent = new ManualResetEvent( false );

		IntPtr _DeviceHandle;
		GCHandle _HeaderHandle;
		GCHandle _DataHandle;
		
		WaveHeader _Header;
		byte[] _Data;

		private bool _Disposing = false;
		public bool Disposing
		{
			get{ return _Disposing; }
		}

		private bool _IsPlaying = false;
		public bool IsPlaying
		{
			get{ return this._IsPlaying; }
		}

		public int Size
		{
			get{ return this._Header.BufferLength; }
		}

		public IntPtr Data
		{
			get{ return this._Header.Data; }
		}

		private bool _IsEmpty = true;
		public bool IsEmpty
		{
			get{ return this._IsEmpty; }
			set{ this._IsEmpty = value; }
		}

		WaveBufferEmptyHandler _FillDelegate;

		/// <summary>
		/// Initializes the buffer.
		/// </summary>
		/// <param name="device">Handle to the wave out device to use.</param>
		/// <param name="size">Size of the buffer to allocate.</param>
		public WaveOutBuffer( IntPtr device, int size, WaveBufferEmptyHandler fillDelegate )
		{
			if ( device == IntPtr.Zero )
				throw new ArgumentException( "Device must be a a valid pointer to a wave out device.", "device" );

			if ( size < 1 )
				throw new ArgumentOutOfRangeException( "size", size, "Size must be greater than zero." );

			if ( fillDelegate == null )
				throw new ArgumentNullException( "fillDelegate" );

			_FillDelegate = fillDelegate;

			_DeviceHandle = device;

			// Allocate memory for the buffer, and set up a GCHandle pointed to it.
			_Data = new byte[size];
			_DataHandle = GCHandle.Alloc( _Data, GCHandleType.Pinned );

			// Create the header and a GC handle pointed to it.
			_Header	= new WaveHeader();
			_HeaderHandle = GCHandle.Alloc( _Header, GCHandleType.Pinned );
			
			_Header.Data = _DataHandle.AddrOfPinnedObject();
			_Header.BufferLength = size;

			_Header.UserData = (IntPtr)GCHandle.Alloc( this );
			_Header.Loops = 0;
			_Header.Flags = 0;

			int result = WaveFormNative.waveOutPrepareHeader( _DeviceHandle, 
				ref _Header, Marshal.SizeOf( _Header ) );

			if ( result != WaveError.MMSYSERR_NOERROR )
				throw new SoundCoreException( WaveError.GetMessage( result ), result );
		}

		~WaveOutBuffer()
		{
			Dispose();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if ( Disposing )
				return;

			_Disposing = true;

			WaveFormNative.waveOutUnprepareHeader( this._DeviceHandle, ref _Header, Marshal.SizeOf( _Header ) );

			if ( _HeaderHandle.IsAllocated )
				_HeaderHandle.Free();

			if ( _DataHandle.IsAllocated )
				_DataHandle.Free();

			this._WaitForCompletion.Close();
		}

		#endregion

		public virtual void Play()
		{
			this._WaitForCompletion.Reset();
			
//			this._PlayEvent.WaitOne();
//			this._PlayEvent.Reset();
			this._IsPlaying = true;

			int result = WaveFormNative.waveOutWrite( this._DeviceHandle, ref this._Header, Marshal.SizeOf( this._Header ) );

			if ( result != WaveError.MMSYSERR_NOERROR )
				throw new SoundCoreException( WaveError.GetMessage( result ), result );
		}

		protected virtual void OnCompleted( EventArgs e )
		{
			if ( this.IsPlaying )
			{
//				this._IsEmpty  = true;
//				this._WaitForCompletion.Set();
////
////				this._NextBuffer.Play();
//				
//				WaveBufferEmptyEventArgs args = new WaveBufferEmptyEventArgs( this.Data, this.Size );
////				this._FillDelegate( this, args );

				_IsPlaying = false;
				_IsEmpty = true;
                
				this._WaitForCompletion.Set();
				this._WaitForEmpty.Set();

				if ( this._Completed != null )
					this._Completed( this, e );
			}
		}
		
		public void BufferFilled()
		{
			this._IsEmpty = false;
			this._WaitForEmpty.Reset();
		}

		/// <summary>
		/// Allows a thread to wait until this buffer block is finished playing
		/// </summary>
		public void WaitUntilCompleted()
		{
			// Verify we're playing.  If we're not, cause a context switch.
			if ( this.IsPlaying )
			{
				this._WaitForCompletion.WaitOne();		// Wait for block to finish
			}
			else
				Thread.Sleep( 0 );	// Allow context switch
		}

		/// <summary>
		/// Waits until the buffer has been marked as empty.
		/// </summary>
		public void WaitUntilEmpty()
		{
			if ( this.IsEmpty )
				Thread.Sleep( 0 );
			else
				this._WaitForEmpty.WaitOne();
		}

		/// <summary>
		/// Callback that receives signal when completed
		/// </summary>
		/// <param name="audioDevice">Device that signaled the event</param>
		/// <param name="message">Message passed back to us.</param>
		/// <param name="instance">Not used</param>
		/// <param name="header">Header that is being signalled</param>
		/// <param name="reserved">Not used</param>
		internal static void WaveOutProcCallback(IntPtr audioDevice, int message, int instance, ref WaveHeader header, int reserved)
		{
			// Make sure that we're signalled done
			if ( message == (int)WaveFormOutputMessage.Done )
			{
				// Get IntPtr to WaveOutBuffer
				GCHandle handle = (GCHandle)header.UserData;

				// Get buffer
				WaveOutBuffer buffer = (WaveOutBuffer)handle.Target;

				// Signal completed to buffer
				buffer.OnCompleted( EventArgs.Empty );
			}
		}
	}
}
