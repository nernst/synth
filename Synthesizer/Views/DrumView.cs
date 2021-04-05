using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ErnstTech.SoundCore.Synthesis;

namespace Synthesizer.Views
{
    public class DrumView : ObservableObject
    {
        DrumGenerator _Generator = new DrumGenerator() { Source = new SawWave().Adapt() };

        public double BaseFrequency
        {
            get => _Generator.BaseFrequency;
            set => SetProperty(_Generator.BaseFrequency, value, _Generator, (g, v) => g.BaseFrequency = v);
        }

        public double PhaseShift1
        {
            get => _Generator.PhaseShift1;
            set => SetProperty(_Generator.PhaseShift1, value, _Generator, (g, v) => g.PhaseShift1 = v);
        }

        public double PhaseShift2
        {
            get => _Generator.PhaseShift2;
            set => SetProperty(_Generator.PhaseShift2, value, _Generator, (g, v) => g.PhaseShift2 = v);
        }

        public double AttackTime
        {
            get => _Generator.Envelope.AttackTime;
            set => SetProperty(_Generator.Envelope.AttackTime, value, _Generator.Envelope, (e, v) => e.AttackTime = v);
        }

        public double DecayTime
        {
            get => _Generator.Envelope.DecayTime;
            set => SetProperty(_Generator.Envelope.DecayTime, value, _Generator.Envelope, (e, v) => e.DecayTime = v);
        }

        public double SustainHeight
        {
            get => _Generator.Envelope.SustainHeight;
            set => SetProperty(_Generator.Envelope.SustainHeight, value, _Generator.Envelope, (e, v) => e.SustainHeight = v);
        }

        public double SustainTime
        {
            get => _Generator.Envelope.SustainTime;
            set => SetProperty(_Generator.Envelope.SustainTime, value, _Generator.Envelope, (e, v) => e.SustainTime = v);
        }

        public double ReleaseTime
        {
            get => _Generator.Envelope.ReleaseTime;
            set => SetProperty(_Generator.Envelope.ReleaseTime, value, _Generator.Envelope, (e, v) => e.ReleaseTime = v);
        }

        public double EffectTime => _Generator.Envelope.ReleaseEnd;

        public Func<double, double> Adapt() => _Generator.Adapt();
    }
}
