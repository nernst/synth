using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ErnstTech.SoundCore.Synthesis
{
    public class ADSREnvelope
    {
        /// <summary>
        ///     Maximum value of the attack.
        /// </summary>
        public double AttackHeight { get; set; } = 1.0;

        /// <summary>
        ///     Duration of the attack, in seconds. Must be non-negative.
        /// </summary>
        public double AttackTime { get; set; } = 0.05;

        public double AttackRate
        {
            get { return AttackHeight / AttackTime; }
        }

        /// <summary>
        ///     Duration of the decay, in seconds. Must be non-negative.
        /// </summary>
        public double DecayTime { get; set; } = 0.25;

        public double DecayStart
        {
            get { return AttackTime; }
        }

        public double DecayEnd => AttackTime + DecayTime;
        public double DecayRate => (AttackHeight - SustainHeight) / DecayTime;

        /// <summary>
        ///     Maximum value of the sustained signal.
        /// </summary>
        public double SustainHeight { get; set; } = 0.5;

        /// <summary>
        ///     Duration of the Sustain, in seconds. Must be non-negative.
        /// </summary>
        public double SustainTime { get; set; } = 0.50;

        public double SustainStart => DecayEnd;

        public double SustainEnd => SustainStart + SustainTime;

        /// <summary>
        ///     Duration of the Release, in seconds. Must be non-negative.
        /// </summary>
        public double ReleaseTime { get; set; } = 0.25;

        public double ReleaseStart => SustainEnd;
        public double ReleaseEnd => ReleaseStart + ReleaseTime;
        public double ReleaseRate => SustainHeight / ReleaseTime;

        public ADSREnvelope()
        { }

        public double Factor(double time)
        {
            if (time <= AttackTime)
                return AttackHeight * (time / AttackTime);
            else if (DecayStart < time && time <= DecayEnd)
                return AttackHeight - DecayRate * (time - DecayStart);
            else if (SustainStart < time && time <= SustainEnd)
                return SustainHeight;
            else if (ReleaseStart < time && time <= ReleaseEnd)
                return SustainHeight - ReleaseRate * (time - ReleaseStart);

            return 0;
        }

        public double ValueAt(double time, double value) => value * Factor(time);

        public double ValueAt(double time, Func<double, double> func) => ValueAt(time, func(time));

        public Func<double, double> Adapt(Func<double, double> func) => (double t) => ValueAt(t, func);
    }
}
