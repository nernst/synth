using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public class BaseDrumGenerator
    {
        public Func<double, double> Source { get; set; }
        public IList<double> Harmonics { get; set; }

        public static IReadOnlyList<double> SampleHarmonics { get; private set; } = new List<double> { 43, 86, 129, 175, 218, 266 };

        public BaseDrumGenerator(Func<double, double> source, IList<double> harmonics)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (harmonics == null)
                throw new ArgumentNullException(nameof(harmonics));
            if (harmonics.Count <= 0)
                throw new ArgumentException("harmonics counts cannot be zero.", nameof(harmonics));

            Source = source;
            Harmonics = harmonics;
        }

        public BaseDrumGenerator()
            : this(new TriWave().Sample,  new List<double>(SampleHarmonics))
        { }

        public Func<double, double> Adapt()
        {
            var source = Source;
            var func = Harmonics
                .Select<double, Func<double, double>>(h => (double t) => source(h * t))
                .Aggregate((l, r) => (double t) => l(t) + r(t));

            return func;
        }
    }
}
