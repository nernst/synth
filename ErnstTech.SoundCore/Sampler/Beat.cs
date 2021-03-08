using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Sampler
{
    public class Beat
    {
        public const double Off = 0.0;
        public const double Half = 0.5;
        public const double Full = 1.0;

        public ISampler? Sampler { get; set; }
        public double Level { get; set; } = Off;
        public long? QueuePoint { get; set; } = null;

        public double Sample(long nSample) => Level * Sampler!.Sample(nSample - QueuePoint.GetValueOrDefault());
    }
}
