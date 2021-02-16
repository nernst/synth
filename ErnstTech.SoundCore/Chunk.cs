using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// Summary description for Chunk.
    /// </summary>
    public abstract class Chunk
    {
        public abstract string ID { get; }

        public int DataSize
        {
            get { return Data.Length; }
        }

        public byte[] Data { get; private set; }

        internal Chunk(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            this.Data = data;
        }

        internal static Chunk GetChunk(BinaryReader reader)
        {
            var buffer = new byte[4];
            reader.Read(buffer, 0, buffer.Length);

            var id = Encoding.ASCII.GetString(buffer);
            var length = reader.ReadInt32();
            if (length < 0)
                throw new SoundCoreException($"Chunk '{id}' specifies a negative length: {length}.");

            var data = new byte[length];
            reader.Read(data, 0, length);

            return id switch
            {
                "fmt " => new FormatChunk(data),
                "data" => new DataChunk(data),
                "RIFF" => new RiffChunk(data),
                "WAVE" => new WaveChunk(data),
                _ => throw new SoundCoreException($"Unknown or Unexpected chunk type encountered: '{id}'."),
            };
        }

        protected short ReadInt16(int offset)
        {
            return BitConverter.ToInt16(Data.AsSpan()[offset..]);
        }

        protected int ReadInt32(int offset)
        {
            return BitConverter.ToInt32(Data.AsSpan()[offset..]);
        }
    }
}
