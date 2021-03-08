using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ErnstTech.SoundCore.Sampler;
using System.Collections.Specialized;

namespace Synthesizer.Views
{
    public class BeatLoopView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BeatLoop BeatLoop { get; init; }
        public ObservableCollection<BeatView> Beats { get; init; }

        public ISampler Sampler
        {
            get { return BeatLoop.Sampler; }
            set
            {
                BeatLoop.Sampler = value;
                NotifyPropertyChanged();
            }
        }

        public double BeatsPerMinute
        {
            get { return BeatLoop.BeatsPerMinute; }
            set
            {
                BeatLoop.BeatsPerMinute = value;
                NotifyPropertyChanged();
            }
        }

        public double BeatDuration
        {
            get { return BeatLoop.BeatDuration; }
        }

        public double FullHeight
        {
            get { return BeatLoop.FullHeight; }
            set
            {
                BeatLoop.FullHeight = value;
                NotifyPropertyChanged();
            }
        }

        public double HalfHeight
        {
            get { return BeatLoop.HalfHeight; }
            set
            {
                BeatLoop.HalfHeight = value;
                NotifyPropertyChanged();
            }
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

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
