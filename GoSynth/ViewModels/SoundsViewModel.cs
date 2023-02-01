using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoSynth.Models;
using System.Collections.ObjectModel;

namespace GoSynth.ViewModels;

public partial class SoundsViewModel : ObservableObject, IQueryAttributable
{
    public IAsyncRelayCommand NewSoundCommand { get; }
    public IAsyncRelayCommand SelectSoundCommand { get; }

    SoundManager soundManager;

    [ObservableProperty]
    ObservableCollection<SoundViewModel> sounds = new();

    public SoundsViewModel()
    {
        NewSoundCommand = new AsyncRelayCommand(NewSoundAsync);
        SelectSoundCommand = new AsyncRelayCommand<SoundViewModel>(SelectSoundAsync);
        this.soundManager = SoundManager.Current;
        InitSounds();
    }

    void InitSounds() => Sounds = new ObservableCollection<SoundViewModel>(soundManager.Sounds.Select(s => new SoundViewModel(s)));

    private async Task SelectSoundAsync(SoundViewModel? sound)
    {
        if (sound != null)
            await Shell.Current.GoToAsync($"{nameof(SoundPage)}", new Dictionary<string, object> { { "sound", sound.Sound.Clone() } });
    }

    private async Task NewSoundAsync()
    {
        await Shell.Current.GoToAsync(nameof(SoundPage));
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("sound", out var soundObj))
        {
            // Sound was saved
            var sound = soundObj as Sound ?? throw new ArgumentException("'sound' query atribute was not of type Sound.", nameof(query));
            var model = this.Sounds.Where(s => s.Id == sound.Id).FirstOrDefault();
            if (model == null)
                this.Sounds.Add(new SoundViewModel(sound));
            else
                model.Sound = sound;
        }

        if (query.TryGetValue("remove", out var idObj))
        {
            // Sound was deleted
            var id = idObj as Guid? ?? throw new ArgumentException("'remove' query attribute was not of type Guid.", nameof(query));
            var toRemove = this.Sounds.Where(s => s.Id == id).FirstOrDefault();
            if (toRemove != null)
                this.Sounds.Remove(toRemove);
        }
    }
}
