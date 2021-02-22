using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public class NoiseGenerator : IWaveFormGenerator
    {
        Random _Random;

        public NoiseGenerator() : this((int)(DateTime.Now.Ticks / (double)int.MaxValue))
        { }

        public NoiseGenerator(int seed) : this(new Random(seed)) { }

        public NoiseGenerator(Random random)
        {
            this._Random = random;
        }

        /// <summary>
        ///     Generate a random sample in the range [-1.0, 1.0].
        /// </summary>
        /// <returns></returns>
        public double Sample()
        {
            return 2.0 * (this._Random.NextDouble() - 0.5);
        }

        public Func<double, double> Adapt() => (double _) => Sample();

        public double[] Generate(long nSamples)
        {
            return this.Take((int)nSamples).ToArray();
        }

        public IEnumerator<double> GetEnumerator()
        {
            while (true)
                yield return Sample();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            while (true)
                yield return Sample();
        }
    }
}
