using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GoSynth.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    private async Task GoToSoundsAsync()
    {
        await Shell.Current.GoToAsync(nameof(SoundsPage));
    }
}
