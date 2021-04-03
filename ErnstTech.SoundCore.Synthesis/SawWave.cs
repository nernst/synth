using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public class SawWave
    {
        /// <summary>
        ///     Period, in seconds.
        /// </summary>
        public double Period { get; init; } = 1.0;
        public double MaxValue { get; init; } = 1.0;
        public double MinValue { get; init; } = -1.0;

        public SawWave()
        { }

        public double Sample(double time)
        {
            var local = time - Math.Truncate(time);
            var slope = (MaxValue - MinValue) / Period;
            return local * slope + MinValue;
        }

        public Func<double, double> Adapt() => (double t) => Sample(t);
    }
}
