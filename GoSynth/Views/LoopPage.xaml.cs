using GoSynth.ViewModels;

namespace GoSynth.Views;

public partial class LoopPage : ContentPage
{
	public LoopPage()
	{
		InitializeComponent();
		this.BindingContext = new BeatLoopViewModel();
	}
}