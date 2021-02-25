using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public class SquareWave
    {
        public double DutyCycle { get; init; }
        
        public SquareWave(double dutyCycle)
        {
            if (dutyCycle < 0 || dutyCycle > 1.0)
                throw new ArgumentOutOfRangeException(nameof(dutyCycle), dutyCycle, "dutyCycle must be in the closed range [0, 1.0].");

            this.DutyCycle = dutyCycle;
        }

        public double Sample(double time)
        {
            time -= Math.Truncate(time);

            return time <= DutyCycle ? 1.0 : 0.0;
        }

        public Func<double, double> Adapt() => Sample;
        public Func<double, double> Adapt(Func<double, double> func) => (double t) => Sample(func(t));
    }
}
