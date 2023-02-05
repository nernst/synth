using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GoSynth.Views;

namespace GoSynth.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private async Task GoToSoundsAsync()
    {
        await Shell.Current.GoToAsync(nameof(SoundsPage));
    }

    [RelayCommand]
    async Task GotoLoopsAsync()
    {
        await Shell.Current.GoToAsync(nameof(LoopPage));
    }
}
