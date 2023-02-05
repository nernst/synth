using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ErnstTech.SoundCore.Sampler
{
    public class WAVSampler : ISampler
    {
        WaveReader _waveReader;
        short _channel = 0;
        double[] _data;


        public WAVSampler(Stream stream, short channel = 0)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _waveReader = new WaveReader(stream);
            if (channel >= _waveReader.Format.Channels)
                throw new ArgumentOutOfRangeException(nameof(channel), channel, $"Channel must be in range [0, {_waveReader.Format.Channels}).");
            _channel = channel;
            _data = _waveReader.GetChannelFloat(channel).Select(f => (double)f).ToArray();
            this.Length = _data.LongLength;
        }

        public SampleRate SampleRate => (SampleRate)_waveReader.Format.SamplesPerSecond;

        public AudioBits BitsPerSample => (AudioBits)_waveReader.Format.BitsPerSample;

        public long Length { get; set; }

        public long GetSamples(double[] destination, long destOffset, long sampleStartOffset, long numSamples)
        {
            var length = Math.Min(Math.Min(destination.LongLength - destOffset, numSamples), _data.LongLength - sampleStartOffset);
            Array.Copy(_data, sampleStartOffset, destination, destOffset, length);
            return length;
        }

        public double Sample(long sampleOffset)
        {
            try
            {
                return _data[sampleOffset];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(nameof(sampleOffset), sampleOffset, $"sampleOffset must be in range [0, {_data.LongLength}).");
            }
        }
    }
}
