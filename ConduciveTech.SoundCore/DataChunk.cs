using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for DataChunk.
	/// </summary>
	public class DataChunk : Chunk
	{
		internal DataChunk( string id, byte[] data )
            : base ( id, data )
		{
		}

		protected override void Init()
		{
			if ( ID != "data" )
				throw new SoundCoreException( "Expected chunk of type 'data'." );

			base.Init ();
		}
	}
}
