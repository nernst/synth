using GoSynth.ViewModels;

namespace GoSynth;

public partial class SoundPage : ContentPage
{
	public SoundPage()
	{
		InitializeComponent();
		this.BindingContext = new SoundViewModel();
	}
}