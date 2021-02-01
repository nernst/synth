using System;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveStream.
	/// </summary>
	public class WaveStream : Stream
	{
		private Stream _BaseStream;
		public virtual Stream BaseStream
		{
			get{ return _BaseStream; }
		}

		private WaveFormat _Format;
		public WaveFormat Format
		{
			get{ return _Format; }
		}

		public override bool CanRead
		{
			get{ return true; }
		}

		public override bool CanSeek
		{
			get{ return true; }
		}

		public override bool CanWrite
		{
			get{ return false; }
		}

		public override long Length
		{
			get
			{
				return (BaseStream.Length - _StartPosition);
			}
		}

		public override long Position
		{
			get
			{
				return (BaseStream.Position - _StartPosition);
			}
			set
			{
				BaseStream.Position = ( value + _StartPosition );
			}
		}

		private long _StartPosition;
		public int HeaderLength
		{
			get{ return (int)(_StartPosition); }
		}

        public long NumberOfSamples
        {
            get
            {
                return this.Length / (this.Format.Channels * this.Format.BlockAlignment);
            }
        }


		#region Constructors
		protected WaveStream()
		{
		}

		public WaveStream( Stream stream )
		{
			if ( stream == null )
				throw new ArgumentNullException( "stream" );

			_BaseStream = stream;
			ReadHeader();
		}

		public WaveStream( string filename )
		{
			if ( filename == null )
				throw new ArgumentNullException( "filename" );

			_BaseStream = new FileStream(  filename, FileMode.Open, FileAccess.Read, FileShare.Read );
			ReadHeader();
		}
		#endregion // Constructors

		protected virtual void SetFormat( WaveFormat format )
		{
			if ( format == null )
				throw new ArgumentNullException( "format" );

			_Format = format;
		}

		private void SeekChunk( string id )
		{
			byte[] buffer = new byte[4];

			while ( _BaseStream.Length != BaseStream.Position )
			{
				BaseStream.Read( buffer, 0, 4 );

				string temp = System.Text.Encoding.ASCII.GetString( buffer );

				if ( temp == id )
					return;
			}
			
			throw new SoundCoreException( string.Format( "Unable to find chunk type '{0}'.", id ) );
		}

		private void ReadHeader()
		{
			SeekChunk( "RIFF" );

			for ( int i = 0; i < 4; i++ )
				_BaseStream.ReadByte();

			SeekChunk( "WAVE" );

			_Format = new WaveFormat( BaseStream );

			SeekChunk( "data" );

			for ( int i = 0; i < 4; i++ )
				_BaseStream.ReadByte();

			_StartPosition = BaseStream.Position;
		}

		public override void Close()
		{
			BaseStream.Close();
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return BaseStream.BeginRead (buffer, offset, count, callback, state);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return BaseStream.BeginWrite (buffer, offset, count, callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return BaseStream.EndRead( asyncResult );
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			BaseStream.EndWrite (asyncResult);
		}

		public override int GetHashCode()
		{
			return BaseStream.GetHashCode();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return BaseStream.Read( buffer, offset, count );
		}

		public override int ReadByte()
		{
			return BaseStream.ReadByte ();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException( "Length of a WaveStream cannot be set." );
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch( origin )
			{
				case SeekOrigin.Begin:
					return BaseStream.Seek( offset + _StartPosition, origin );
				default:
					return BaseStream.Seek( offset, origin );					
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException( "WaveStream cannot be written to." );
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException( "WaveStream cannot be written to." );
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}
		
		public byte[] GetRawHeader()
		{
			byte[] buffer = new byte[ _StartPosition ];

			lock( BaseStream )
			{
				long position = BaseStream.Position;
				
				BaseStream.Position = 0;

				BaseStream.Read( buffer, 0, buffer.Length );

				BaseStream.Position = position;
			}

			return buffer;
		}

	}
}
