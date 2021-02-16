using System;
using System.IO;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// Summary description for RiffChunk.
    /// </summary>
    public sealed class RiffChunk : Chunk
    {
        public override string ID => "RIFF";

        public WaveChunk Wave { get; init; }

        internal RiffChunk(byte[] data) : base(data)
        {
            using var ms = new MemoryStream(Data);
            using var reader = new BinaryReader(ms);

            Wave = (WaveChunk)Chunk.GetChunk(reader);
        }
    }
}
