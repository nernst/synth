using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ErnstTech.SoundCore.Synthesis;

namespace Synthesizer.Views
{
    public class BaseDrumView : ObservableObject
    {
        public ObservableCollection<double> Harmonics { get; init; } = new ObservableCollection<double>(BaseDrumGenerator.SampleHarmonics);
        private BaseDrumGenerator _Generator;

        public ADSREnvelope Envelope { get; init; } = new ADSREnvelope();

        public BaseDrumView()
        {
            this._Generator = new BaseDrumGenerator(new TriWave().Sample, Harmonics);
        }

        public Func<double, double> Adapt() => Envelope.Adapt(_Generator.Adapt());
    }
}
