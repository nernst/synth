using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ErnstTech.SoundCore.Sampler;

namespace Synthesizer.Views
{
    public class BeatView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        int _Index = -1;
        public int Index
        {
            get { return _Index; }
            set
            {
                if (this._Index != value)
                {
                    this._Index = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        BeatState _State = BeatState.Off;

        public BeatState State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    this.Parent.Beats[this.Index].Level = value switch
                    {
                        BeatState.Off => Beat.Off,
                        BeatState.Half => Beat.Half,
                        BeatState.Full => Beat.Full,
                        _ => (double?)null
                    } ?? this.Parent.Beats[this.Index].Level;

                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("Level");
                }

            }
        }

        public double Level
        {
            get { return Parent.Beats[this.Index].Level; }
            set
            {
                if (Parent.Beats[this.Index].Level != value)
                {
                    Parent.Beats[this.Index].Level = value;
                    this.State = StateFromLevel(value);
                    this.NotifyPropertyChanged();
                }
            }
        }

        public BeatLoop Parent { get; init; }

        public BeatView(BeatLoop parent, int index)
        {
            this.Parent = parent;
            this._Index = index;
            this._State = StateFromLevel(Level);
        }

        static BeatState StateFromLevel(double level) => level switch
        {
            Beat.Full => BeatState.Full,
            Beat.Half => BeatState.Half,
            Beat.Off => BeatState.Off,
            _ => BeatState.Custom
        };
    }
}
