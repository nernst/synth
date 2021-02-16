using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore
{
    public class WaveReader
    {
        public Stream Stream { get; init; }
        public WaveFormat Format { get; init; }
        public long BasePosition { get; init; }
        public long RiffLength { get; init; }
        public long DataLength { get; init; }
        public long NumberOfSamples { get; init; }

        public WaveReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            this.Stream = stream;
            var reader = new BinaryReader(stream);

            SeekChunk("RIFF");
            RiffLength = reader.ReadInt32();
            //for (int i = 0; i < 4; ++i) // Skip length?
            //    Stream.ReadByte();

            SeekChunk("WAVE");
            Format = new WaveFormat(stream);

            SeekChunk("data");
            DataLength = reader.ReadInt32();
            //for (int i = 0; i < 4; ++i) // Skip length?
            //    Stream.ReadByte();

            this.NumberOfSamples = this.DataLength / (Format.Channels * Format.BitsPerSample / 8);

            BasePosition = this.Stream.Position;
        }

        void SeekChunk(string id)
        {
            const int idSize = 4;
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));
            if (id.Length != idSize)
                throw new ArgumentException($"Expected ID length is 4. Got {id.Length}.", nameof(id));

            var seek = Encoding.ASCII.GetBytes(id);
            var buffer = new byte[idSize];

            while (true)
            {
                if (Stream.Read(buffer, 0, idSize) != idSize)
                    throw new SoundCoreException($"Failed to find chunk '{id}'");

                if (seek.SequenceEqual(buffer))
                    break;
            }
        }

        IEnumerable<byte[]> GetEnumerator(short channel)
        {
            if (channel < 0 || channel >= this.Format.Channels)
                throw new ArgumentOutOfRangeException(nameof(channel), channel, $"Channel must be in range [0, {this.Format.Channels}).");

            var numBytes = Format.BitsPerSample switch
            {
                8 => 1,
                16 => 2,
                32 => 4,
                _ => throw new SoundCoreException($"Invalid BitsPerSample. Got '{Format.BitsPerSample}', expected 8, 16, or 32.")
            };

            var buf = new byte[numBytes];
            var skipCount = (this.Format.Channels - 1 ) * numBytes;
            Debug.Assert(skipCount >= 0);

            // Queue the stream to the first byte of the channel
            this.Stream.Position = this.BasePosition + channel * numBytes;

            while (this.Stream.Position < this.Stream.Length)
            {
                int count = this.Stream.Read(buf);
                if (count != buf.Length)
                    throw new SoundCoreException($"Failed to read entire sample from channel {channel}. Got {count} bytes, expected {buf.Length}.");

                yield return buf;
                this.Stream.Position += skipCount;
            }
        }

        public IEnumerable<byte> GetChannelInt8(short channel)
        {
            foreach (var buf in GetEnumerator(channel))
                yield return (byte)buf[0];
        }

        public IEnumerable<short> GetChannelInt16(short channel)
        {
            foreach (var buf in GetEnumerator(channel))
                yield return BitConverter.ToInt16(buf);
        }

        public IEnumerable<int> GetChannelInt32(short channel)
        {
            foreach (var buf in GetEnumerator(channel))
                yield return BitConverter.ToInt32(buf);
        }

        public IEnumerable<float> GetChannelFloat(short channel)
        {
            foreach (var buf in GetEnumerator(channel))
                yield return BitConverter.ToSingle(buf);
        }
    }
}
