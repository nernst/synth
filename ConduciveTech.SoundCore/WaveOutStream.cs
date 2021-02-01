using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveOutStream.
	/// </summary>
	public class WaveOutStream : System.IO.BinaryWriter
	{
		const int DefaultBufferSize = 512 * 1024; // 512 KB

		private Stream _BaseStream;
		public override Stream BaseStream
		{
			get
			{
				return _BaseStream;
			}
		}

		private bool _IsPlaying = false;
		public bool IsPlaying
		{
			get{ return _IsPlaying; }
		}

		private bool _IsPaused = false;
		public bool IsPaused
		{
			get{ return _IsPaused; }
		}

		private int _InitialCapacity = DefaultBufferSize;
		public int InitialCapacity
		{
			get
			{
				return this._InitialCapacity;
			}
		}

		public int Capacity
		{
			get
			{ 
				if ( _BaseStream is MemoryStream )
					return ((MemoryStream)_BaseStream).Capacity;

				return -1;
			}
		}

		public long Length
		{
			get
			{
				return _BaseStream.Length;
			}
		}

		public AudioBits Quality
		{
			get{ return (AudioBits)_Format.BitsPerSample; }
		}

		public AudioMode Mode
		{
			get{ return (AudioMode)_Format.Channels; }
		}

		public SampleRate Rate
		{
			get{ return (SampleRate)_Format.SamplesPerSecond; }
		}

		private int _Device = -1;
		public int Device
		{
			get{ return _Device; }
		}

		private int _BufferCount = 8;
		public int BufferCount
		{
			get{ return _BufferCount; }
		}

		public int BufferLength
		{
			get
			{
				int length = Convert.ToInt32(Convert.ToDouble( (int)Rate ) * 0.4 * Convert.ToDouble((int)this.Mode) * Convert.ToDouble((int)this.Quality) / 8.0 );
				int waste = length % BlockAlign;

				length += BlockAlign - waste;
				return length;
			}
		}

		public short BlockAlign
		{
			get
			{
				return _Format.BlockAlignment;
			}
		}

		private WavePlayer _Player;
		private WaveFormat _Format;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="waveIn"></param>
		public WaveOutStream( int device, Stream waveIn ) : base()
		{
			if ( waveIn == null )
				throw new ArgumentNullException( "waveIn" );

			_Device = device;
			_BaseStream = waveIn;

			ReadWaveHeader( waveIn );

			Init();
		}

		public WaveOutStream( int device, AudioBits quality, AudioMode mode, SampleRate rate ) : base()
		{
			_Format = new WaveFormat( (short)mode, (int)rate, (short)quality );

			this._BaseStream = new MemoryStream( this.InitialCapacity );
			Init();
		}

		private int ReadInt32( Stream stream )
		{
			if ( stream == null )
				throw new ArgumentNullException( "stream " );
			byte[] buffer = new byte[4];
			stream.Read( buffer, 0, 4 );

			return ((buffer[0])|(buffer[1] << 8)|(buffer[2] << 16)|(buffer[3] << 24));
		}

		private void ReadWaveHeader( Stream stream )
		{
			byte[] buffer = new byte[4];
			stream.Read( buffer, 0, 4 );

			string id = System.Text.ASCIIEncoding.ASCII.GetString( buffer );
			if ( id != "RIFF" )
				throw new SoundCoreException( "Expected Chunk type of 'RIFF'." );

			int length = ReadInt32( stream );
			if ( length != ( stream.Length - 8 ))
				throw new SoundCoreException( string.Format( "Unexpected RIFF Chunk size.\nExpected: {0}\nRead from stream: {1}", (stream.Length - 8), length ) );

			if ( !SeekToChunk( "WAVE" ) )
				throw new SoundCoreException( "Could not find chunk type of 'WAVE'." );

			_Format = new WaveFormat( stream );

			switch( (SampleRate)_Format.SamplesPerSecond )
			{
				case SampleRate.Rate44100:
				case SampleRate.Rate22050:
				case SampleRate.Rate11025:
					break;
				default:
					throw new SoundCoreException( "An unsupported sampling rate was encounted." );
			}

			if ( !SeekToChunk( "data" ) )
				throw new SoundCoreException( "Could not find chunk type of 'data'." );

			ReadInt32( _BaseStream );
		}

		private bool SeekToChunk( string id )
		{
			byte[] buffer = new byte[4];
			bool found = false;
			while( !found && _BaseStream.Position < _BaseStream.Length )
			{
				_BaseStream.Read( buffer, 0, 4 );
				if ( System.Text.ASCIIEncoding.ASCII.GetString( buffer ) == id )
					found = true;
			}

			return found;
		}

		/// <summary>
		/// Initializes the playback stream
		/// </summary>
		private void Init()
		{
		}

		public static int GetDevices()
		{
			return WavePlayer.NumDevices;
		}

		private void FillBufferHandler( object sender, WaveBufferEmptyEventArgs args )
		{
			byte[] buffer = new byte[args.Length];

			int length = _BaseStream.Read( buffer, 0, args.Length );

			for ( int i = length; i < args.Length; i++ )
			{
				buffer[i] = 0;
			}

			if ( length <= 0 )
			{
				_Player.Stop();
			}

			args.BytesWritten = length;

			Marshal.Copy(  buffer, 0, args.Buffer, length );
		}

		#region Audio Playback Controls
		/// <summary>
		/// Begins playing audio back from the current position.
		/// </summary>
		public virtual void Play()
		{
			this._IsPlaying = true;
			this._IsPaused = false;

			System.Diagnostics.Debug.Assert(false);

			//_Player = new WavePlayer( this.Device, this.BufferLength, this.BufferCount, 
			//	_Format, new WaveBufferEmptyHandler( FillBufferHandler ) );
		}

		/// <summary>
		/// Pause playback from the last point written to wave buffer.
		/// </summary>
		public virtual void Pause()
		{
			this._IsPaused = true;
			this._IsPlaying = false;
		}

		/// <summary>
		/// Stops playback, returns to beginning of written data.
		/// </summary>
		public virtual void Stop()
		{
			this._IsPlaying = false;
			this._IsPaused = false;

			if ( _Player == null )
				return;

			if ( _Player.IsPlaying )
			{
				_Player.Stop();
				_Player.Dispose();
			}
		}
		#endregion

		public override void Close()
		{
			if ( this.IsPlaying || this.IsPaused )
				this.Stop();

			base.Close ();
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.IsPlaying || this.IsPaused )
					this.Stop();

				if ( _Player != null )
					_Player.Dispose();
				// TODO:  Close wave output

				this._BaseStream.Close();
			}

			base.Dispose (disposing);
		}


	}
}
