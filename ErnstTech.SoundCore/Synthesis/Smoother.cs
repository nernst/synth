using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public class Smoother
    {
        double[] _Samples;
        int _Index = 0;

        public Smoother(int sampleCount)
        {
            if (sampleCount <= 0)
                throw new ArgumentException("SampleCount must be positive.", nameof(sampleCount));

            _Samples = new double[sampleCount];
            Array.Fill(_Samples, 0.0);
        }

        double Sample(double value)
        {
            _Samples[_Index] = value;
            _Index = (_Index + 1) % _Samples.Length;

            return _Samples.Average();
        }

        public IEnumerable<double> Wrap(IEnumerable<double> source)
        {
            var e = source.GetEnumerator();
            while (e.MoveNext())
                yield return Sample(e.Current);
            
        }

        public Func<double, double> Wrap(Func<double, double> source) => (double t) => Sample(source(t));

        public static Func<double, double> Wrap(int sampleCount, Func<double, double> source) => new Smoother(sampleCount).Wrap(source);
    }
}
