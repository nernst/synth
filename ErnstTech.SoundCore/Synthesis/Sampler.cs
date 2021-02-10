using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnaryFunc = System.Func<double, double>;

namespace ErnstTech.SoundCore.Synthesis
{
    public class Sampler : IWaveFormGenerator
    {
        public long SamplesPerSecond { get; set; } = 44_100;
        public UnaryFunc Function { get; set; }


        public Sampler(UnaryFunc func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            this.Function = func;
        }

        public double[] Generate(long nSamples)
        {
            return this.Take((int)nSamples).ToArray();
        }

        public IEnumerator<double> GetEnumerator()
        {
            var delta = 1.0 / SamplesPerSecond;
            long sample = 0;
            while (true)
                yield return this.Function(delta * sample++);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
