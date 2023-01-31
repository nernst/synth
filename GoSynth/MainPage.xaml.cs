using GoSynth.ViewModels;

namespace GoSynth;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

}