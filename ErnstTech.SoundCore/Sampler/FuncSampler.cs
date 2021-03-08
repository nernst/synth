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

        public long Length { get; set; } = -1;

        public Func<double, double>? SampleFunc { get; init; }

        public long GetSamples(double[] destination, long destOffset, long sampleStartOffset, long numSamples)
        {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            long count = 0;
            for (long offset = sampleStartOffset; destOffset < destination.LongLength && numSamples > 0; ++destOffset, --numSamples, ++offset, ++count)
                destination[destOffset] = Sample(offset);

            return count;
        }

        public double Sample(long sampleOffset) => SampleFunc!.Invoke(sampleOffset * TimeDelta);
    }
}
