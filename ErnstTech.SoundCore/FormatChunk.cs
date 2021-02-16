using System;
using System.Linq;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// Summary description for FormatChunk.
    /// </summary>
    public sealed class FormatChunk : Chunk
    {
        public override string ID => "fmt ";

        public short CompressionCode { get; private set; }
        public short Channels { get; private set; }
        public int SampleRate { get; private set; }
        public int AverageBytesPerSecond { get; private set; }
        public short BlockAlign { get; private set; }
        public short SignificantBitsPerSample { get; private set; }
        public short ExtraFormatBytesLength { get; private set; }
        public byte[] ExtraFormatBytes { get; private set; }

        internal FormatChunk(byte[] data) : base(data)
        {

            this.CompressionCode = ReadInt16(0);
            this.Channels = ReadInt16(2);
            this.SampleRate = ReadInt32(4);
            this.AverageBytesPerSecond = ReadInt32(8);
            this.BlockAlign = ReadInt16(10);
            this.SignificantBitsPerSample = ReadInt16(12);
            this.ExtraFormatBytesLength = ReadInt16(14);
            this.ExtraFormatBytes = data.Skip(16).ToArray();
        }
    }
}
