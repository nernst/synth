using System;

namespace ErnstTech.SoundCore.Synthesis
{
    /// <summary>
    ///     Applies an ADSR (Attack-Decay-Sustain-Release) envelope to 
    /// </summary>
    public class ADSREnvelope
    {
        /// <summary>
        ///     Maximum value of the attack.
        /// </summary>
        public double AttackHeight { get; set; } = 1.0;

        /// <summary>
        ///     Duration of the attack, in seconds. Must be non-negative.
        /// </summary>
        public double AttackTime { get; set; } = 0.01;

        public double AttackRate => AttackHeight / AttackTime;

        /// <summary>
        ///     Duration of the decay, in seconds. Must be non-negative.
        /// </summary>
        public double DecayTime { get; set; } = 0.10;

        public double DecayStart => AttackTime;

        public double DecayEnd => AttackTime + DecayTime;
        public double DecayRate => (SustainHeight - AttackHeight) / DecayTime;

        /// <summary>
        ///     Maximum value of the sustained signal.
        /// </summary>
        public double SustainHeight { get; set; } = 0.5;

        /// <summary>
        ///     Duration of the Sustain, in seconds. Must be non-negative.
        /// </summary>
        public double SustainTime { get; set; } = 0.250;

        public double SustainStart => DecayEnd;

        public double SustainEnd => SustainStart + SustainTime;

        /// <summary>
        ///     Duration of the Release, in seconds. Must be non-negative.
        /// </summary>
        public double ReleaseTime { get; set; } = 0.10;

        public double ReleaseStart => SustainEnd;
        public double ReleaseEnd => ReleaseStart + ReleaseTime;
        public double ReleaseRate => SustainHeight / ReleaseTime;

        public ADSREnvelope()
        { }

        /// <summary>
        ///     The scaling factor as a function of time.
        /// </summary>
        /// <param name="time">The time, in seconds, from the start of the envelope.</param>
        /// <returns>The scaling factor to be applied.</returns>
        public double Factor(double time)
        {
            if (time <= AttackTime)
                return AttackHeight * (time / AttackTime);
            else if (DecayStart < time && time <= DecayEnd)
                return AttackHeight + DecayRate * (time - DecayStart);
            else if (SustainStart < time && time <= SustainEnd)
                return SustainHeight;
            else if (ReleaseStart < time && time <= ReleaseEnd)
                return SustainHeight - ReleaseRate * (time - ReleaseStart);

            return 0;
        }

        /// <summary>
        ///     Scale a factor to a moment in time.
        /// </summary>
        /// <param name="time">The time, in seconds, from the start of the envelope.</param>
        /// <param name="value">The value to be scaled.</param>
        /// <returns>The scaled value for the moment in time.</returns>
        public double ValueAt(double time, double value) => value * Factor(time);

        /// <summary>
        ///     Scale a factor from a function of time to a moment in time.
        /// </summary>
        /// <param name="time">The time, in seconds, for the function to be applied and scaled.</param>
        /// <param name="func">A function of time, in seconds, returning a value to be scaled.</param>
        /// <returns>The scaled value of the function at the moment in time.</returns>
        public double ValueAt(double time, Func<double, double> func) => ValueAt(time, func(time));

        /// <summary>
        ///     Adapts a function, wrapping an ADSR evelope around the source function.
        /// </summary>
        /// <param name="func">The function, parameterized by time, to be adapted.</param>
        /// <returns>A function that has been adapted to apply the envelope.</returns>
        public Func<double, double> Adapt(Func<double, double> func) => (double t) => ValueAt(t, func);
    }
}
