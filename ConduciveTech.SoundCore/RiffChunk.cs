using System;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for RiffChunk.
	/// </summary>
	public sealed class RiffChunk : Chunk
	{
		private WaveChunk _Wave;
		public WaveChunk Wave
		{
			get{ return _Wave; }
		}

		internal RiffChunk( string id, byte[] data ) : base( id, data )
		{
		}

		protected override void Init()
		{
			if ( ID != "RIFF" )
				throw new SoundCoreException( "Expected chunk of type 'RIFF'" );

			base.Init ();

			MemoryStream ms = new MemoryStream( Data );
			BinaryReader reader = new BinaryReader( ms );

			_Wave = (WaveChunk)Chunk.GetChunk( reader );

			reader.Close();
		}

	}
}
