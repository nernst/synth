using System;
using System.IO;

namespace ErnstTech.SoundCore
{
	/// <summary>
	/// Summary description for WaveChunk.
	/// </summary>
	public sealed class WaveChunk : Chunk
	{
        public override string ID => "WAVE";
		public FormatChunk Format { get; init; }
		public DataChunk WaveData { get; init; }

		internal WaveChunk(byte[] data ) : base(data)
		{
			using var ms = new MemoryStream(Data);
			using var reader = new BinaryReader(ms);

			Format = (FormatChunk)GetChunk(reader);
			WaveData = (DataChunk)GetChunk(reader);
		}
	}
}
