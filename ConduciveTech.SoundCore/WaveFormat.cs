using System;
using System.IO;
using System.Text;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for Wave.
	/// </summary>
	public class WaveFormat
	{
		public const int HeaderSize = 44;

		private WaveFormatEx _WaveFormat;
        		
		private short _Channels = 2;
		private const short MinChannels = 1;
		private const short MaxChannels = 2;
		public short Channels
		{
			get{ return _Channels; }
			set
			{
				if ( value < MinChannels || value > MaxChannels )
					throw new ArgumentOutOfRangeException( "Channels", value, string.Format( "Value must be greater than or equal to {0} and less than or equal to {1}.", 
						MinChannels, MaxChannels ) );

				_Channels = value;
			}
		}

		private readonly int[] AllowedSamplingRates = new int[]{
																   8000,
																   11025,
																   22050,
																   44100
															   };

		private int _SamplesPerSecond = 44100;
		public int SamplesPerSecond
		{
			get{ return _SamplesPerSecond; }
			set
			{
				bool found = false;
				foreach( int rate in this.AllowedSamplingRates )
				{
					if ( value == rate )
					{
						found = true;
						break;
					}
				}

				if ( !found )
					throw new ArgumentOutOfRangeException( "SamplesPerSecond", value, "Specified value is not an allowed sampling rate for PCM audio." );

				_SamplesPerSecond = value;
			}
		}

		private short _BitsPerSample = 16;
		public short BitsPerSample
		{
			get{ return _BitsPerSample; }
			set
			{
				if ( value != 8 && value != 16 )
					throw new ArgumentOutOfRangeException( "BitsPerSample", value, "BitsPerSample must be 8 or 16 for PCM audio." );

				_BitsPerSample = value;
			}
		}

		public short BlockAlignment
		{
			get
			{
				return Convert.ToInt16( this.Channels * this.BitsPerSample / 8 );
			}
		}

		public int AverageBytesPerSecond
		{
			get
			{
				return (int)(this.BlockAlignment * this.SamplesPerSecond);
			}
		}

		public WaveFormat()
		{
			Init();
		}

		public WaveFormat( short channels, int samplesPerSecond, short bitsPerSample )
		{
			this.Channels = channels;
			this.SamplesPerSecond = samplesPerSecond;
			this.BitsPerSample = bitsPerSample;

			Init();
		}

		private int ReadInt32( Stream stream )
		{
			short lo = ReadInt16(stream);
			short hi = ReadInt16(stream);

			return (((int)hi) << 16) | ((ushort)lo);
		}

		private short ReadInt16( Stream stream )
		{
			return (short)( (stream.ReadByte()) | (stream.ReadByte() << 8 ) );
		}

		public WaveFormat( Stream stream  )
		{
			byte[] buffer = new byte[4];
			stream.Read( buffer, 0, 4 );

			string id = System.Text.ASCIIEncoding.ASCII.GetString( buffer );
			if ( id != "fmt " )
				throw new SoundCoreException( string.Format( "Error reading format from BinaryReader: Unknown format 0x{0:X2}{1:X2}{2:X2}{3:X2} ('{4}').",
					buffer[0], buffer[1], buffer[2], buffer[3], System.Text.Encoding.ASCII.GetString( buffer ) ) );

			int size = ReadInt32(stream);

			short code = ReadInt16(stream);
			if ( code != 1 )
				throw new SoundCoreException( "Unsupported compression code. Only PCM Audio is supported" );

			this.Channels = ReadInt16(stream);
			this.SamplesPerSecond = ReadInt32(stream);

			int avgBps = ReadInt32(stream);
			short blockAlign = ReadInt16(stream);
			this.BitsPerSample = ReadInt16(stream);
		
			if ( size > 16 )
			{
				ushort extraBytes = (ushort)ReadInt16(stream);

				while ( extraBytes > 0 )
				{
					stream.ReadByte();
					extraBytes--;
				}
			}
		}

		private void Init()
		{
			_WaveFormat = new WaveFormatEx();

			SetFormatValues();
		}

		private void SetFormatValues()
		{
			_WaveFormat.format = FormatTag.WAVE_FORMAT_PCM;
			_WaveFormat.nSamplesPerSec = this.SamplesPerSecond;
			_WaveFormat.nBitsPerSample = this.BitsPerSample;
			_WaveFormat.nAvgBytesPerSec = this.AverageBytesPerSecond;
			_WaveFormat.nBlockAlign = this.BlockAlignment;
			_WaveFormat.nChannels = this.Channels;
			_WaveFormat.cbSize = 0;		// This member is ignored, anyway
		}

		public WaveFormatEx GetFormat()
		{
			SetFormatValues();

			return this._WaveFormat;
		}

		public void WriteHeader( Stream stream, int dataSize )
		{
			if ( stream == null )
				throw new ArgumentNullException( "stream" );

			if ( !stream.CanWrite )
				throw new ArgumentException( "Stream is not writable.", "stream" );
			
			if ( dataSize < 0 )
				throw new ArgumentOutOfRangeException( "dataSize", dataSize, "dataSize must be a nonnegative integer." );

			const int chunkSize = 36;
			const int fmtSize = 16;
			byte[] buffer;

			BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.ASCII );
			buffer = Encoding.ASCII.GetBytes( "RIFF" );
			writer.Write( buffer );
			writer.Write( dataSize + chunkSize );
			buffer = Encoding.ASCII.GetBytes( "WAVE" );
			writer.Write( buffer );
			buffer = Encoding.ASCII.GetBytes( "fmt " );
			writer.Write( buffer );
			writer.Write( fmtSize );
			writer.Write( (short)0x0001 );
			writer.Write( (short)this.Channels );
			writer.Write( (int)this.SamplesPerSecond );
			writer.Write( (int)this.AverageBytesPerSecond );
			writer.Write( (short)this.BlockAlignment );
			writer.Write( (short)this.BitsPerSample );
			buffer = Encoding.ASCII.GetBytes( "data" );
			writer.Write( buffer );
			writer.Write( dataSize );
		}

	}
}
