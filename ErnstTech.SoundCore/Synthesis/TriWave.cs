using System;

namespace ErnstTech.SoundCore.Synthesis
{
    public class TriWave
    {
        /// <summary>
        ///     Period, in seconds.
        /// </summary>
        public double Period { get; init; } = 1.0;
        public double MaxValue { get; init; } = 1.0;
        public double MinValue { get; init; } = -1.0;

        public TriWave()
        { }

        public double Sample(double time)
        {
            var local = time - Period * Math.Truncate(time / Period);
            var half = this.Period / 2.0;
            var slope = 2 * (MaxValue - MinValue) / Period;

            if (local < half)
                return local * slope + MinValue;
            else if (local > half)
                return MaxValue - (local - half) * slope;
            return MaxValue;
        }

        public Func<double, double> Adapt() => (double t) => Sample(t);
    }
}
