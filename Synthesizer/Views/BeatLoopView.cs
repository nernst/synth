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

namespace Synthesizer.Views
{
    public class BeatLoopView : INotifyPropertyChanged
    {
        BeatLoop _BeatLoop = new BeatLoop();

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ISampler Sampler
        {
            get { return _BeatLoop.Sampler; }
            set
            {
                _BeatLoop.Sampler = value;
                NotifyPropertyChanged();
            }
        }

        public double BeatsPerMinute
        {
            get { return _BeatLoop.BeatsPerMinute; }
            set
            {
                _BeatLoop.BeatsPerMinute = value;
                NotifyPropertyChanged();
            }
        }

        public double? BeatDuration
        {
            get { return _BeatLoop.BeatDuration; }
            set
            {
                _BeatLoop.BeatDuration = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<BeatLoop.Beat> Beats => _BeatLoop.Beats;

        public Stream WAVStream => _BeatLoop.WAVStream;
    }
}
