using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoSynth.Models;
using GoSynth.Synthesis;
using System.Windows.Input;
using Plugin.Maui.Audio;

namespace GoSynth.ViewModels;

public partial class SoundViewModel : ObservableObject, IQueryAttributable
{
    public ICommand PlayCommand { get;}
    public IAsyncRelayCommand SaveCommand { get;  }
    public IAsyncRelayCommand CancelCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }

    Synthesizer synthesizer;
    Sound sound;
    

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
                _generatorFunc = synthesizer.Compile(this.Equation);
            }
            catch
            { }
        }
        return _generatorFunc;
    }

    bool hasChanges = false;


    public SoundViewModel()
        : this(new Sound())
    { }

    public SoundViewModel(Sound sound)
    {
        PlayCommand = new RelayCommand(Play);
        SaveCommand = new AsyncRelayCommand(Save);
        CancelCommand = new AsyncRelayCommand(Cancel);
        DeleteCommand = new AsyncRelayCommand(Delete);

        this.synthesizer = new Synthesizer();
        this.sound = sound;

        this.PropertyChanged += (o, e) =>
        {
            if (e.PropertyName != null)
            {
                hasChanges = true;
                if (e.PropertyName == nameof(Equation))
                    _generatorFunc = null;
            }
        };
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

        var stream = synthesizer.Generate(generator, this.Duration);
        var manager = AudioManager.Current;
        var player = manager.CreatePlayer(stream);
        player.Play();
    }

    async Task NavigateToParent()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task Cancel()
    {
        if (hasChanges)
        {
        }
        await NavigateToParent();
    }

    private async Task Save()
    {
        await SoundManager.Current.Save(this.sound);
        await NavigateToParent();
    }

    private async Task Delete()
    {
        await SoundManager.Current.Remove(this.sound);
        await NavigateToParent();
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("sound", out var model))
        {
            if (model is Sound sound)
            {
                this.sound = sound;
                OnPropertyChanged();
            }
        }
    }
}
