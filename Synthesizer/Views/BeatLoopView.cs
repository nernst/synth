using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ErnstTech.SoundCore.Sampler;

namespace Synthesizer.Views
{
    public class BeatLoopView : ObservableObject
    {
        public BeatLoop BeatLoop { get; init; }
        public ObservableCollection<BeatView> Beats { get; init; }

        public ISampler Sampler
        {
            get => BeatLoop.Sampler;
            set => SetProperty(BeatLoop.Sampler, value, BeatLoop, (o, v) => o.Sampler = v);
        }

        public double BeatsPerMinute
        {
            get => BeatLoop.BeatsPerMinute;
            set => SetProperty(BeatLoop.BeatsPerMinute, value, BeatLoop, (o, v) => o.BeatsPerMinute = v);
        }

        public double BeatDuration => BeatLoop.BeatDuration;

        public double FullHeight
        {
            get => BeatLoop.FullHeight;
            set => SetProperty(BeatLoop.FullHeight, value, BeatLoop, (o, v) => o.FullHeight = v);
        }

        public double HalfHeight
        {
            get => BeatLoop.HalfHeight;
            set => SetProperty(BeatLoop.HalfHeight, value, BeatLoop, (o, v) => o.HalfHeight = v);
        }

        public Stream WAVStream => BeatLoop.WAVStream;

        public BeatLoopView()
        {
            this.BeatLoop = new BeatLoop();
            this.Beats = new ObservableCollection<BeatView>(Enumerable.Range(0, BeatLoop.BeatCount).Select(i => new BeatView(this.BeatLoop, i)));
            this.Beats.CollectionChanged += OnBeatsChanged;
            this.PropertyChanged += (o, e) => BeatLoop.InvalidateWAVStream();
        }

        private void OnBeatsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
