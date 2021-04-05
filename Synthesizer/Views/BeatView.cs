using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using ErnstTech.SoundCore.Sampler;

namespace Synthesizer.Views
{
    public class BeatView : ObservableObject
    {

        int _Index = -1;
        public int Index
        {
            get => _Index;
            set => SetProperty(ref _Index, value);
        }

        BeatState _State = BeatState.Off;

        public BeatState State
        {
            get => _State;
            set => SetProperty(ref _State, value);
        }

        public double Level
        {
            get => Parent.Beats[Index].Level;
            set => SetProperty(Parent.Beats[Index].Level, value, Parent.Beats[Index], (o, v) => o.Level = v);
        }

        public BeatLoop Parent { get; init; }

        public BeatView(BeatLoop parent, int index)
        {
            this.Parent = parent;
            this._Index = index;
            this._State = StateFromLevel(Level);

            this.PropertyChanged += OnPropertyChanged; ;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.PropertyName) || e.PropertyName == "State")
                Level = LevelFromState(State);

            if (string.IsNullOrWhiteSpace(e.PropertyName) || e.PropertyName == "Level")
                _State = StateFromLevel(Level); 
        }

        static BeatState StateFromLevel(double level) => level switch
        {
            Beat.Full => BeatState.Full,
            Beat.Half => BeatState.Half,
            Beat.Off => BeatState.Off,
            _ => BeatState.Custom
        };

        double LevelFromState(BeatState state) => state switch
        {
            BeatState.Off => Beat.Off,
            BeatState.Half => Beat.Half,
            BeatState.Full => Beat.Full,
            _ => Parent.Beats[Index].Level
        };
    }
}
