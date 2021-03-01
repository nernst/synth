using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public class DrumGenerator
    {
        public double BaseFrequency { get; set; } = 111.0;
        public double PhaseShift1 { get; set; } = 175.0;
        public double PhaseShift2 { get; set; } = 224.0;

        public Func<double, double> Source { get; set; } = null;

        public ADSREnvelope Envelope { get; init; } = new ADSREnvelope();

        public DrumGenerator()
        { }


        public Func<double, double> Adapt()
        {
            if (this.Source == null)
                throw new InvalidOperationException("Source cannot be null.");

            return Envelope.Adapt((double t) => Source(BaseFrequency * t + PhaseShift1) + Source(BaseFrequency * t + PhaseShift2));
        }
    }
}
