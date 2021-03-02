using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Sampler
{
    public interface ISampler
    {
        /// <summary>
        ///     The number of samples per second. Must be non-negative.
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        ///     The number of bits per sample. Expected to be one of: 8, 16, 32.
        /// </summary>
        int BitsPerSample { get; }

        /// <summary>
        ///     The amount of time between samples, in seconds.
        /// </summary>
        double TimeDelta { get { return 1.0 / SampleRate; } }

        /// <summary>
        ///     The length of the sample in terms of number of samples. May be -1, if length cannot be determined.
        /// </summary>
        long Length { get; }

        /// <summary>
        ///     Get a sample at a specific offset.
        /// </summary>
        /// <param name="sampleOffset">The offset of the sample to retrieve.</param>
        /// <returns>A double containing the sample indicated.</returns>
        double Sample(long sampleOffset);

        long GetSamples(double[] destination, long destOffset, long sampleStartOffset, long numSamples);

        double[] GetSamples(long sampleStartOffset, long numSamples)
        {
            var dest = new double[numSamples];
            var count = GetSamples(dest, 0, sampleStartOffset, numSamples);
            if (count != numSamples)
                throw new InvalidOperationException($"Failed to read expected number of samples. Read {count}, expected {numSamples}.");
            return dest;
        }
        double[] GetSamples() => GetSamples(0, Length);
    }
}
