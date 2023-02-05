using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoSynth.Models;
using GoSynth.Synthesis;
using System.Windows.Input;
using Plugin.Maui.Audio;

namespace GoSynth.ViewModels;

public partial class SoundViewModel : ObservableObject, IQueryAttributable
{
    public class ConfirmDiscardEventArgs
    {
        public bool? Confirmed { get; set; } = null;
    }

    public ICommand PlayCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand HelpCommand { get;  }

    Sound sound;
    Sound? original = null;

    public Sound Sound
    {
        get => this.sound;
        set
        {
            this.sound = value;
            original = value.Clone();
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Equation));
            OnPropertyChanged(nameof(Duration));
        }
    }

    public bool HasChanges
    {
        get => !sound.Equals(original);
    }

    public Func<ConfirmDiscardEventArgs, Task>? ConfirmDiscard;

    public Guid Id
    {
        get => sound.Id;
    }

    public string Name
    {
        get => sound.Name;
        set => SetProperty(sound.Name, value, sound, (u, v) => u.Name = v);
    }

    public string Equation
    {
        get => sound.Equation;
        set => SetProperty(sound.Equation, value, sound, (u, v) => u.Equation = v);
    }

    public double Duration
    {
        get => sound.Duration;
        set => SetProperty(sound.Duration, value, sound, (u, v) => u.Duration = v);
    }

    Func<double, double>? _generatorFunc = null;

    Func<double, double>? GetGenerator()
    {
        if (_generatorFunc == null)
        {
            try
            {
                _generatorFunc = Synthesizer.Current.Compile(Equation);
            }
            catch
            { }
        }
        return _generatorFunc;
    }

    public SoundViewModel()
        : this(new Sound())
    { }

    public SoundViewModel(Sound sound)
    {
        PlayCommand = new RelayCommand(Play);
        SaveCommand = new AsyncRelayCommand(Save);
        CancelCommand = new AsyncRelayCommand(Cancel);
        DeleteCommand = new AsyncRelayCommand(Delete);
        HelpCommand = new AsyncRelayCommand(Help);

        this.sound = sound;

        this.PropertyChanged += (o, e) =>
        {
            if (e.PropertyName != null)
            {
                if (e.PropertyName == nameof(Equation))
                    _generatorFunc = null;
            }
        };
    }

    async Task Help()
    {
        await Shell.Current.GoToAsync(nameof(Views.EquationHelp));
    }

    private void Play()
    {
        if (string.IsNullOrWhiteSpace(this.Equation))
            return;

        if (Duration <= 0)
            return;

        var generator = GetGenerator();
        if (generator == null)
            return;

        var stream = Synthesizer.Current.Generate(generator, this.Duration);
        var manager = AudioManager.Current;
        var player = manager.CreatePlayer(stream);
        player.Play();
    }

    private async Task Cancel()
    {
        if (HasChanges && ConfirmDiscard != null)
        {
            var args = new ConfirmDiscardEventArgs();
            await ConfirmDiscard.Invoke(args);
            if (!(args.Confirmed ?? false))
                return;
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task Save()
    {
        await SoundManager.Current.Save(this.sound);
        await Shell.Current.GoToAsync("..", new Dictionary<string, object> { { "sound", this.Sound } });
    }

    private async Task Delete()
    {
        await SoundManager.Current.Remove(this.sound);
        await Shell.Current.GoToAsync("..", new Dictionary<string, object> { { "remove", this.Id } });
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("sound", out var model))
        {
            if (model is Sound sound)
            {
                this.Sound = sound;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Equation));
                OnPropertyChanged(nameof(Duration));
            }
        }
    }
}
