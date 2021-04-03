using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore.Synthesis
{
    public static class Waves
    {
        static readonly TriWave _Tri = new TriWave();
        static readonly SawWave _Saw = new SawWave();

        public static double Tri(double time) => _Tri.Sample(time);
        public static double Saw(double time) => _Saw.Sample(time);
    }
}
