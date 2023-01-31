using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoSynth.Models;
using System.Collections.ObjectModel;

namespace GoSynth.ViewModels;

public partial class SoundsViewModel : ObservableObject, IQueryAttributable
{
    public IAsyncRelayCommand NewSoundCommand { get; }
    public IAsyncRelayCommand SelectSound { get; }

    SoundManager soundManager;

    [ObservableProperty]
    ObservableCollection<SoundViewModel> sounds;

    public SoundsViewModel()
    {
        NewSoundCommand = new AsyncRelayCommand(NewSoundAsync);
        SelectSound = new AsyncRelayCommand<SoundViewModel>(SelectSoundAsync);
        this.soundManager = SoundManager.Current;
        sounds = new ObservableCollection<SoundViewModel>(soundManager.Sounds.Select(s => new SoundViewModel(s)));
    }

    private async Task SelectSoundAsync(SoundViewModel? sound)
    {
        if (sound != null)
            await Shell.Current.GoToAsync($"{nameof(SoundPage)}?id={sound.Id}");

    }

    private async Task NewSoundAsync()
    {
        var sound = new SoundViewModel(new Sound());
        Sounds.Add(sound);
        await SelectSoundAsync(sound);
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
    }
}
