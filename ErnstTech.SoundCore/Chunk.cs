using System;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for Chunk.
	/// </summary>
	public abstract class Chunk
	{
		public string ID { get; private set; }

		public int DataSize
		{
			get{ return Data.Length; }
		}

		public byte[] Data { get; private set; }

		internal Chunk( string id, byte[] data )
		{
			if ( id == null )
				throw new ArgumentNullException( "id" );

			if ( id.Length != 4 )
				throw new ArgumentException( "Specified ID must have a length of 4 to be a valid chunk ID.", "ID" );

            this.ID = id;

			if ( data == null )
				throw new ArgumentNullException( "data" );

            this.Data = data;

			Init();
		}

		protected virtual void Init()
		{
		}

		internal static Chunk GetChunk( BinaryReader reader )
		{
			byte[] buffer = new byte[4];
			reader.Read( buffer, 0, 4 );

			string id = System.Text.ASCIIEncoding.ASCII.GetString( buffer );
			int length = reader.ReadInt32();
			if ( length < 0 )
				throw new SoundCoreException( string.Format( "Chunk '{0}' specifies a negative length.", id ) );

			byte[] data = new byte[ length ];
			reader.Read( data, 0, length );

			switch(id)
			{
				case "fmt ":
					return new FormatChunk( id, data );
				case "data":
					return new DataChunk( id, data );
				case "RIFF":
					return new RiffChunk( id, data );
				case "WAVE":
					return new WaveChunk( id, data );
				default:
					throw new SoundCoreException( string.Format( "Unknown or Unexpected chunk type encountered: {0}.", id ) );
			}
		}

		protected short GetShortFromData( int offset )
		{
			return (short)((Data[offset]) | (Data[offset + 1] << 8));
		}

		protected int GetIntFromData( int offset )
		{
			return ((Data[offset]) | (Data[offset + 1] << 8) | (Data[offset + 2] << 16) | (Data[offset + 3] << 24));
		}
	}
}
