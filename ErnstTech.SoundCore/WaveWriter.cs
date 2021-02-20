using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// </summary>
    public class WaveWriter
    {
        public Stream Stream { get; init; }
        public int SampleRate { get; init; }

        public WaveWriter(Stream stream, int sampleRate)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException("Stream must be writable.", nameof(stream));
            if (sampleRate <= 0)
                throw new ArgumentException("SampleRate must be positive.", nameof(sampleRate));

            this.Stream = stream;
            this.SampleRate = sampleRate;
        }

        /// <summary>
        ///     Write the channels to the stream.
        /// </summary>
        /// <param name="length">The length of the longest channel, in number of samples.</param>
        /// <param name="channels">The channels to be written.</param>
        public void Write(long length, params IEnumerable<float>[] channels)
        {
            if (length < 0)
                throw new ArgumentException("Length must be non-negative.", nameof(length));
            if (channels.Length <= 0)
                throw new ArgumentException("Must have at least one channel.");

            var format = new WaveFormat
            {
                BitsPerSample = 32,
                Channels = (short)channels.Length,
                SamplesPerSecond = this.SampleRate,
                FormatTag = FormatTag.WAVE_FORMAT_IEEE_FLOAT,
            };

            var dataSize = (long)(length * sizeof(float) * channels.Length);
            format.WriteHeader(this.Stream, (int)dataSize);

            var enumerators = channels.Select(c => c.GetEnumerator()).ToArray();

            for (; length > 0; --length)
            {
                for (int i = 0; i < enumerators.Length; ++i)
                {
                    if (!enumerators[i]?.MoveNext() ?? false)
                        enumerators[i] = null;
                    this.Stream.Write(BitConverter.GetBytes(enumerators[i]?.Current ?? (float)0));
                }
            }
        }

        /// <summary>
        ///     Write the channels to the stream.
        /// </summary>
        /// <param name="length">The length of the longest channel, in number of samples.</param>
        /// <param name="channels">The channels to be written.</param>
        public void Write(long length, params IEnumerable<short>[] channels)
        {
            if (length < 0)
                throw new ArgumentException("Length must be non-negative.", nameof(length));
            if (channels.Length <= 0)
                throw new ArgumentException("Must have at least one channel.");

            var format = new WaveFormat
            {
                BitsPerSample = 16,
                Channels = (short)channels.Length,
                SamplesPerSecond = this.SampleRate
            };

            var dataSize = (long)(length * sizeof(float) * channels.Length);
            format.WriteHeader(this.Stream, (int)dataSize);

            var enumerators = channels.Select(c => c.GetEnumerator()).ToArray();

            for (; length > 0; --length)
            {
                for (int i = 0; i < enumerators.Length; ++i)
                {
                    if (!enumerators[i]?.MoveNext() ?? false)
                        enumerators[i] = null;
                    this.Stream.Write(BitConverter.GetBytes(enumerators[i]?.Current ?? (short)0));
                }
            }
        }

        /// <summary>
        ///     Write the channels to the stream.
        /// </summary>
        /// <param name="length">The length of the longest channel, in number of samples.</param>
        /// <param name="channels">The channels to be written.</param>
        public void Write(long length, params IEnumerable<byte>[] channels)
        {
            if (length < 0)
                throw new ArgumentException("Length must be non-negative.", nameof(length));
            if (channels.Length <= 0)
                throw new ArgumentException("Must have at least one channel.");

            var format = new WaveFormat
            {
                BitsPerSample = 8,
                Channels = (short)channels.Length,
                SamplesPerSecond = this.SampleRate
            };

            var dataSize = (long)(length * sizeof(byte) * channels.Length);
            format.WriteHeader(this.Stream, (int)dataSize);

            var enumerators = channels.Select(c => c.GetEnumerator()).ToArray();

            for (; length > 0; --length)
            {
                for (int i = 0; i < enumerators.Length; ++i)
                {
                    if (!enumerators[i]?.MoveNext() ?? false)
                        enumerators[i] = null;
                    this.Stream.WriteByte(enumerators[i]?.Current ?? (byte)0);
                }
            }
        }
    }
}
