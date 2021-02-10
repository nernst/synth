using System;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for FormatChunk.
	/// </summary>
	public sealed class FormatChunk : Chunk
	{
        public short CompressionCode { get; private set; }
        public short Channels { get; private set; }
        public int SampleRate { get; private set; }
		public int AverageBytesPerSecond { get; private set; }
		public short BlockAlign { get; private set; }
		public short SignificantBitsPerSample { get; private set; }
		public short ExtraFormatBytesLength { get; private set; }
        public byte[] ExtraFormatBytes { get; private set; }

        internal FormatChunk( string id, byte[] data ) : base( id, data )
		{}

		protected override void Init()
		{
			if ( ID != "fmt " )
				throw new SoundCoreException( "Expected chunk type 'fmt '" );

			base.Init ();

			this.CompressionCode = GetShortFromData( 0 );
			this.Channels = GetShortFromData( 2 );
			this.SampleRate = GetIntFromData( 4 );
			this.AverageBytesPerSecond = GetIntFromData( 8 );
			this.BlockAlign = GetShortFromData( 10 );
			this.SignificantBitsPerSample = GetShortFromData( 12 );
			this.ExtraFormatBytesLength = GetShortFromData( 14 );
            this.ExtraFormatBytes = new byte[this.ExtraFormatBytesLength];

			for ( int i = 0; i < this.ExtraFormatBytesLength; i++ )
				this.ExtraFormatBytes[i] = Data[ 16 + i ];
		}

	}
}
