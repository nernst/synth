using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Sampler
{
    public class FuncSampler : ISampler
    {
        public int SampleRate { get; init; } = 48_000;

        public double TimeDelta => 1.0 / SampleRate;

        public int BitsPerSample { get; init; } = 32;

        public long Length => -1;

        public Func<double, double> SampleFunc { get; init; }

        public long GetSamples(double[] destination, long destOffset, long sampleStartOffset, long numSamples)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            for (long offset = sampleStartOffset; destOffset < destination.LongLength && numSamples > 0; ++destOffset, --numSamples, ++offset)
                destination[destOffset] = Sample(offset);

            return Math.Min(destination.LongLength - sampleStartOffset, numSamples);
        }

        public double Sample(long sampleOffset) => SampleFunc(sampleOffset * TimeDelta);
    }
}
