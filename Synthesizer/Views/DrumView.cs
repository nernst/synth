using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ErnstTech.SoundCore.Synthesis;

namespace Synthesizer.Views
{
    public class DrumView : INotifyPropertyChanged
    {
        DrumGenerator _Generator = new DrumGenerator() { Source = new SawWave().Adapt() };

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public double BaseFrequency
        {
            get { return _Generator.BaseFrequency; }
            set
            {
                _Generator.BaseFrequency = value;
                this.NotifyPropertyChanged();
            }
        }

        public double PhaseShift1
        {
            get { return _Generator.PhaseShift1; }
            set 
            { 
                _Generator.PhaseShift1 = value;
                this.NotifyPropertyChanged();
                
            }
        }

        public double PhaseShift2
        {
            get { return _Generator.PhaseShift2; }
            set
            {
                _Generator.PhaseShift2 = value;
                this.NotifyPropertyChanged();
            }
        }

        public double AttackTime
        {
            get { return _Generator.Envelope.AttackTime; }
            set
            {
                _Generator.Envelope.AttackTime = value;
                this.NotifyPropertyChanged();
            }
        }

        public double DecayTime
        {
            get { return _Generator.Envelope.DecayTime; }
            set
            {
                _Generator.Envelope.DecayTime = value;
                this.NotifyPropertyChanged();
            }
        }

        public double SustainHeight
        {
            get { return _Generator.Envelope.SustainHeight; }
            set
            {
                _Generator.Envelope.SustainHeight = value;
                this.NotifyPropertyChanged();
            }
        }

        public double SustainTime
        {
            get { return _Generator.Envelope.SustainTime; }
            set
            {
                _Generator.Envelope.SustainTime = value;
                this.NotifyPropertyChanged();
            }
        }

        public double ReleaseTime
        {
            get { return _Generator.Envelope.ReleaseTime; }
            set
            {
                _Generator.Envelope.ReleaseTime = value;
                this.NotifyPropertyChanged();
            }
        }

        public double EffectTime => _Generator.Envelope.ReleaseEnd;

        public Func<double, double> Adapt() => _Generator.Adapt();
    }
}
