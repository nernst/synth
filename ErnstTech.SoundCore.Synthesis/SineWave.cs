using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ErnstTech.SoundCore.Synthesis
{
    class SineWave : IWaveFormGenerator
    {
        /// <summary>
        ///     The frequency (in Hertz) of the sine wave.
        /// </summary>
        public double Frequency { get; private set; }

        /// <summary>
        ///     The number of samples per second of the generator, floored.
        /// </summary>
        public long SampleRate { get; private set; } = 44_000;
        public double Magnitude { get; private set; }

        public SineWave(double frequency) : this(frequency, 1.0)
        { }

        public SineWave(double frequency, double magnitude)
        {
            if (frequency <= 0.0)
                throw new ArgumentException("Frequency must be positive.", "frequency");
            this.Frequency = frequency;
            this.Magnitude = magnitude;
        }

        public double[] Generate(long nSample)
        {
            if (nSample < 0)
                throw new ArgumentOutOfRangeException("nSample", nSample, "Number of samples must be non-negative.");

            long idx = 0;
            double[] wave = new double[nSample];
            foreach( double s in this )
            {
                wave[idx++] = s;
                if (idx >= nSample)
                    break;
            }
            return wave;
        }

        public IEnumerator<double> GetEnumerator()
        {
            long pos = -1;

            while (true)
            {
                pos = ++pos % this.SampleRate;
                yield return Math.Sin(2 * Math.PI * pos / this.Frequency);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
