using System;
using System.IO;
using System.Threading;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WavePlayer.
	/// </summary>
	internal class WavePlayer : IDisposable
	{
		private IntPtr _Device;
		
		private int _BufferDataSize;
		private int _BufferCount;
		private WaveBufferEmptyHandler _BufferEmptyHandler;

		private AutoResetEvent _FinishEvent = new AutoResetEvent( false );
		private ManualResetEvent _WriteBufferEvent = new ManualResetEvent( true );

		public int BufferDataSize
		{
			get{ return this._BufferDataSize; }
		}

		public int BufferCount
		{
			get{ return this._BufferCount; }
		}

		private bool _Disposing = false;
		public bool Disposing
		{
			get{ return _Disposing; }
		}

		private bool _IsPlaying = false;
		public bool IsPlaying
		{
			get{ return _IsPlaying; }
		}

		private Thread _PlayThread;

		private readonly object SyncRoot = new object();

		private WaveOutProcDelegate _CallBack = new WaveOutProcDelegate( WaveOutBuffer.WaveOutProcCallback );
		private WaveOutBuffer[] _Buffers;
//
//		private Stream _DataStream;
//
//		public WavePlayer( int device, Stream s )
//		{
//			if ( s == null )
//				throw new ArgumentNullException( "s" );
//
//			_DataStream = s;
//
//		}

		public WavePlayer( int device, 
			int bufferDataSize, int bufferCount, 
			WaveFormat format,
			WaveBufferEmptyHandler bufferEmptyHandler )
		{
			if ( bufferDataSize <= 0 )
				throw new ArgumentOutOfRangeException( "bufferDataSize", bufferDataSize, "bufferDataSize must be greater than 0." );

			this._BufferDataSize = bufferDataSize;

			if ( bufferCount <= 0 )
				throw new ArgumentOutOfRangeException( "bufferCount", bufferCount, "bufferCount must be greater than 0." );

			if ( format == null )
				throw new ArgumentNullException( "format" );
			WaveFormatEx formatEx = format.GetFormat();

			this._BufferCount = 2;

			if ( bufferEmptyHandler == null )
				throw new ArgumentNullException( "bufferEmptyHandler" );

			this._BufferEmptyHandler = bufferEmptyHandler;


			int result = WaveFormNative.waveOutOpen( out _Device, 
				device, 
				ref formatEx,
				_CallBack,
				0,
				(int)DeviceOpenFlags.CallbackFunction );

			if ( result != WaveError.MMSYSERR_NOERROR )
				throw new SoundCoreException( WaveError.GetMessage( result ), result );

			AllocateBuffers();

			_IsPlaying = true;

			FillBuffer( _Buffers[0] );

			_Buffers[0].Play();

//			FillBuffers();
//
//			_FillThread = new Thread( new ThreadStart( FillBufferLoop ) );
//			_FillThread.Start();
//
//			_RootBuffer.Play();
			_PlayThread = new Thread( new ThreadStart( PlayLoop ) );
			_PlayThread.Priority = ThreadPriority.Highest;
			_PlayThread.Start();

		}

		~WavePlayer()
		{
			Dispose();
		}

		public static int GetDevices()
		{
			return WaveFormNative.waveOutGetNumDevs();
		}

		private void AllocateBuffers()
		{
//			_RootBuffer = new WaveOutBuffer( this._Device, this.BufferDataSize );
////			_RootBuffer.Completed += new EventHandler(buffer_Completed);
//		
//			WaveOutBuffer prev = _RootBuffer;

			_Buffers = new WaveOutBuffer[ BufferCount ];
			_Buffers[0] = new WaveOutBuffer( this._Device, this.BufferDataSize, new WaveBufferEmptyHandler( FillBuffer ) );
			_Buffers[1] = new WaveOutBuffer( this._Device, this.BufferDataSize, new WaveBufferEmptyHandler( FillBuffer ) );

			_Buffers[0].NextBuffer = _Buffers[1];
			_Buffers[1].NextBuffer = _Buffers[0];
//			for ( int i = 0; i < this.BufferCount; i++ )
//			{
//				_Buffers[i] = new WaveOutBuffer( this._Device, this.BufferDataSize );
//				prev.NextBuffer = new WaveOutBuffer( this._Device, this.BufferDataSize );
//				prev = prev.NextBuffer;
//				prev.Completed += new EventHandler( buffer_Completed );
//			}
//
//			prev.NextBuffer = _RootBuffer;
		}

		private void FreeBuffers()
		{
			for( int i = 0; i < this.BufferCount; i++ )
			{
				_Buffers[i].Dispose();
			}
//			WaveOutBuffer buffer = this._RootBuffer;
//
//			for ( int i = 0; i < this.BufferCount; i++ )
//			{
//				WaveOutBuffer temp = buffer;
//				buffer = temp.NextBuffer;
//				temp.Dispose();
//			}
//
//			_RootBuffer = null;
		}

//		private void FillBuffers()
//		{
//			this._CurrentWriteBuffer = this._RootBuffer;
//
//			do
//			{
//				FillBuffer( _CurrentWriteBuffer );
//
//				_CurrentWriteBuffer = _CurrentWriteBuffer.NextBuffer;
//
//			} while( _CurrentWriteBuffer != _RootBuffer );
//		}
//

		private void FillBuffer( object sender, WaveBufferEmptyEventArgs args )
		{
			// Fill the buffer
			this._BufferEmptyHandler( this, args);

			// TODO:  Examine args.BytesWritten to determine when end of data reached.
			if ( args.BytesWritten <= 0 )
			{
				// TODO:  Take action to signal playing should stop.
				this._IsPlaying = false;
			}
//
//			// Signal the buffer that it has data.
//			buffer.BufferFilled();

		}

		private void FillBuffer( WaveOutBuffer buffer )
		{
			WaveBufferEmptyEventArgs args = new WaveBufferEmptyEventArgs( buffer.Data, buffer.Size );
			
			// Fill the buffer
			this._BufferEmptyHandler( this, args);

			// TODO:  Examine args.BytesWritten to determine when end of data reached.
			if ( args.BytesWritten <= 0 )
			{
				// TODO:  Take action to signal playing should stop.
				this._IsPlaying = false;
			}
//
//			// Signal the buffer that it has data.
//			buffer.BufferFilled();
		}
//
//		private void FillBufferLoop()
//		{
//			WaveOutBuffer current = _RootBuffer;
//
//			while( IsPlaying )
//			{
//				// Wait until buffer has been read
//				current.WaitUntilEmpty();
//				
//				// Fill the buffer
//				FillBuffer( current );
//
//				// Move to next buffer
//				current = current.NextBuffer;
//			}
//		}

		private void PlayLoop()
		{
			int index = 0;

			WaveOutBuffer buffer = _Buffers[index];
			WaveOutBuffer playing = null;
			FillBuffer(buffer);

			while ( IsPlaying )
			{
				buffer.Play();

				playing = buffer;

				index = ++index % BufferCount;
				buffer = _Buffers[ index ];
				FillBuffer(buffer);

				playing.WaitUntilCompleted();
//
//				WaveOutBuffer buffer = _Buffers[index];
//				FillBuffer( buffer );
//
//
//				buffer.Play();
//				buffer.WaitUntilCompleted();
//				
//				current = current.NextBuffer;
			}

			this._FinishEvent.Set();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if ( Disposing )
				return;

			this._Disposing = true;

			if ( this.IsPlaying )
				Stop();


			WaveFormNative.waveOutClose( this._Device );
 
			FreeBuffers();
		}

		#endregion

		public void Stop()
		{
			this._IsPlaying = false;
			this._FinishEvent.WaitOne();
		}

		private void buffer_Completed(object sender, EventArgs e)
		{
//			WaveOutBuffer buffer = sender as WaveOutBuffer;
//
//			FillBuffer( buffer );
		}
	}
}
