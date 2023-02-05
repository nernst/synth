using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Sampler
{
    public class FuncSampler : ISampler
    {
        public SampleRate SampleRate { get; init; }

        public double TimeDelta { get; init; }

        public AudioBits BitsPerSample { get; init; }

        public long Length { get; set; }

        public Func<double, double> SampleFunc { get; init; }

        public FuncSampler(SampleRate sampleRate, AudioBits bitsPerSample, Func<double, double> sampleFunc, long length)
        {
            if (!Enum.IsDefined(typeof(SampleRate), sampleRate))
                throw new ArgumentException("Not a valid SampleRate.", nameof(sampleRate));
            if (!Enum.IsDefined(typeof(AudioBits), bitsPerSample))
                throw new ArgumentException("Not a valid AudioBits.", nameof(bitsPerSample));

            SampleRate = sampleRate;
            TimeDelta = 1.0 / (int)sampleRate;
            BitsPerSample = bitsPerSample;
            SampleFunc =  sampleFunc;
            Length = length;
        }

        public long GetSamples(double[] destination, long destOffset, long sampleStartOffset, long numSamples)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            long count = 0;
            for (long offset = sampleStartOffset; destOffset < destination.LongLength && numSamples > 0; ++destOffset, --numSamples, ++offset, ++count)
                destination[destOffset] = Sample(offset);

            return count;
        }

        public double Sample(long sampleOffset) => SampleFunc.Invoke(sampleOffset * TimeDelta);
    }
}
