using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoSynth.Models
{
    public class Sound : ICloneable
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Equation { get; set; } = "";
        public double Duration { get; set; } = 1.0;

        public Sound Clone()
        {
            return new Sound { Id = Id, Name = Name, Equation = Equation, Duration = Duration };
        }

        object ICloneable.Clone() => Clone();
    }
}
