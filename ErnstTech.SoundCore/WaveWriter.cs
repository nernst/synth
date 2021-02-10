using System;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveWriter.
	/// </summary>
	public class WaveWriter : WaveStream
	{
		private Stream _BaseStream;
		public override Stream BaseStream
		{
			get
			{
				return _BaseStream;
			}
		}

		private int _InitialCapacity = 4096;
		public virtual int InitialCapacity
		{
			get{ return this._InitialCapacity; }
		}

		public override bool CanRead
		{
			get
			{
				return BaseStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return BaseStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				return BaseStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return BaseStream.Position;
			}
			set
			{
				BaseStream.Position = value;
			}
		}

		public WaveWriter( Stream stream, WaveFormat format )
		{
			if ( stream == null )
				throw new ArgumentNullException( "stream" );

			if ( !stream.CanWrite )
				throw new ArgumentException( "Stream must be writable", "stream" );

			if ( format == null )
				throw new ArgumentNullException( "format" );

			_BaseStream = stream;
			this.SetFormat( format );

			Init();
		}

		public WaveWriter( int capacity, WaveFormat format )
		{
			this._InitialCapacity = capacity;

			_BaseStream = new MemoryStream( capacity );

			format.WriteHeader( BaseStream, -1 );

			Init();
		}

		private void Init()
		{
			_BaseStream = new MemoryStream( this.InitialCapacity );
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return BaseStream.Seek( offset, origin );
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			BaseStream.Write( buffer, offset, count );
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return BaseStream.Read( buffer, offset, count );
		}

		public override void SetLength(long value)
		{
			BaseStream.SetLength( value );
		}



	}
}
