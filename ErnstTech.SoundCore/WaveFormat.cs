using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace ErnstTech.SoundCore
{
    /// <summary>
    /// Summary description for Wave.
    /// </summary>
    public class WaveFormat
    {
        public const int HeaderSize = 44;

        private WaveFormatEx _WaveFormat;

        static readonly FormatTag[] _SupportedFormatTags = new[]
        {
            FormatTag.WAVE_FORMAT_PCM,
            FormatTag.WAVE_FORMAT_IEEE_FLOAT,
        };

        public FormatTag FormatTag { get; set; } = FormatTag.WAVE_FORMAT_PCM;

        private short _Channels = 2;
        private const short MinChannels = 1;
        private const short MaxChannels = 2;
        public short Channels
        {
            get { return _Channels; }
            set
            {
                if (value < MinChannels || value > MaxChannels)
                    throw new ArgumentOutOfRangeException("Channels", value, string.Format("Value must be greater than or equal to {0} and less than or equal to {1}.",
                        MinChannels, MaxChannels));

                _Channels = value;
            }
        }

        private readonly IList<int> AllowedSamplingRates = new int[]{
                                                                   8000,
                                                                   11025,
                                                                   22050,
                                                                   44100,
                                                                   48000,
                                                               };

        private int _SamplesPerSecond = 44100;
        public int SamplesPerSecond
        {
            get { return _SamplesPerSecond; }
            set
            {
                if (!AllowedSamplingRates.Contains(value))
                    throw new ArgumentOutOfRangeException("SamplesPerSecond", value, "Specified value is not an allowed sampling rate for PCM audio.");

                _SamplesPerSecond = value;
            }
        }

        static readonly IList<short> _SupportedBitsPerSample = new short[]{ 8, 16, 32 };

        private short _BitsPerSample = 16;
        public short BitsPerSample
        {
            get { return _BitsPerSample; }
            set
            {
                if (!_SupportedBitsPerSample.Contains(value))
                    throw new ArgumentOutOfRangeException("BitsPerSample", value, "BitsPerSample must be 8, 16 or 32 for PCM audio.");

                _BitsPerSample = value;
            }
        }

        public short BlockAlignment
        {
            get
            {
                return Convert.ToInt16(this.Channels * this.BitsPerSample / 8);
            }
        }

        public int AverageBytesPerSecond
        {
            get
            {
                return (int)(this.BlockAlignment * this.SamplesPerSecond);
            }
        }

        public WaveFormat()
        {
            Init();
        }

        public WaveFormat(short channels, int samplesPerSecond, short bitsPerSample)
        {
            this.Channels = channels;
            this.SamplesPerSecond = samplesPerSecond;
            this.BitsPerSample = bitsPerSample;

            Init();
        }

        private int ReadInt32(Stream stream)
        {
            short lo = ReadInt16(stream);
            short hi = ReadInt16(stream);

            return (((int)hi) << 16) | ((ushort)lo);
        }

        private short ReadInt16(Stream stream)
        {
            return (short)((stream.ReadByte()) | (stream.ReadByte() << 8));
        }

        public WaveFormat(Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);

            string id = Encoding.ASCII.GetString(buffer);
            if (id != "fmt ")
                throw new SoundCoreException(string.Format("Error reading format from BinaryReader: Unknown format 0x{0:X2}{1:X2}{2:X2}{3:X2} ('{4}').",
                    buffer[0], buffer[1], buffer[2], buffer[3], System.Text.Encoding.ASCII.GetString(buffer)));

            int size = ReadInt32(stream);

            this.FormatTag = (FormatTag)ReadInt16(stream);
            if (!_SupportedFormatTags.Contains(this.FormatTag))
                throw new SoundCoreException($"Unsupported format tag: {this.FormatTag}");

            this.Channels = ReadInt16(stream);
            this.SamplesPerSecond = ReadInt32(stream);

            int avgBps = ReadInt32(stream);
            short blockAlign = ReadInt16(stream);
            this.BitsPerSample = ReadInt16(stream);

            if (size > 16)
            {
                ushort extraBytes = (ushort)ReadInt16(stream);

                while (extraBytes > 0)
                {
                    stream.ReadByte();
                    --extraBytes;
                }
            }
        }

        private void Init()
        {
            _WaveFormat = new WaveFormatEx();

            SetFormatValues();
        }

        private void SetFormatValues()
        {
            _WaveFormat.format = FormatTag.WAVE_FORMAT_PCM;
            _WaveFormat.nSamplesPerSec = this.SamplesPerSecond;
            _WaveFormat.nBitsPerSample = this.BitsPerSample;
            _WaveFormat.nAvgBytesPerSec = this.AverageBytesPerSecond;
            _WaveFormat.nBlockAlign = this.BlockAlignment;
            _WaveFormat.nChannels = this.Channels;
            _WaveFormat.cbSize = 0;     // This member is ignored, anyway
        }

        public WaveFormatEx GetFormat()
        {
            SetFormatValues();

            return this._WaveFormat;
        }

        static readonly byte[] _RIFF = Encoding.ASCII.GetBytes("RIFF");
        static readonly byte[] _WAVE = Encoding.ASCII.GetBytes("WAVE");
        static readonly byte[] _FMT = Encoding.ASCII.GetBytes("fmt ");
        static readonly byte[] _DATA = Encoding.ASCII.GetBytes("data");

        public void WriteHeader(Stream stream, int dataSize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new ArgumentException("Stream is not writable.", nameof(stream)); 

            if (dataSize < 0)
                throw new ArgumentOutOfRangeException(nameof(dataSize), dataSize, "dataSize must be a nonnegative integer.");

            const int chunkSize = 36;
            const int fmtSize = 16;

            BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII);
            writer.Write(_RIFF);
            writer.Write(dataSize + chunkSize);
            writer.Write(_WAVE);
            writer.Write(_FMT);
            writer.Write(fmtSize);
            writer.Write((short)this.FormatTag);
            writer.Write((short)this.Channels);
            writer.Write((int)this.SamplesPerSecond);
            writer.Write((int)this.AverageBytesPerSecond);
            writer.Write((short)this.BlockAlignment);
            writer.Write((short)this.BitsPerSample);
            writer.Write(_DATA);
            writer.Write(dataSize);
        }

    }
}
