using System;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveChunk.
	/// </summary>
	public sealed class WaveChunk : Chunk
	{
		private FormatChunk _Format;
		public FormatChunk Format
		{
			get{ return _Format; }
		}

		private DataChunk _WaveData;
		public DataChunk WaveData
		{
			get{ return _WaveData; }
		}

		internal WaveChunk( string id, byte[] data ) : base( id, data )
		{
		}

		protected override void Init()
		{
			if ( ID != "WAVE" )
				throw new SoundCoreException( "Expected chunk type of 'WAVE'." );

			base.Init ();

			MemoryStream ms = new MemoryStream( Data );
			BinaryReader reader = new BinaryReader( ms );

			_Format = (FormatChunk)Chunk.GetChunk( reader );
			_WaveData = (DataChunk)Chunk.GetChunk( reader );
			
			reader.Close();
		}
	}
}
