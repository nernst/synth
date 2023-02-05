using GoSynth.Synthesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoSynth.Models
{
    public class Sound : ICloneable, IEquatable<Sound>
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Equation { get; set; } = "";
        public double Duration { get; set; } = 1.0;

        public Sound Clone()
        {
            return new Sound { Id = Id, Name = Name, Equation = Equation, Duration = Duration };
        }

        public bool Equals(Sound? other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id && Name == other.Name && Equation == other.Equation && Duration == other.Duration;
        }

        object ICloneable.Clone() => Clone();

        public Func<double, double>? Compile()
        {
            try
            {
                return Synthesizer.Current.Compile(this.Equation);
            }
            catch
            {
                return null;
            }
        }
    }
}
