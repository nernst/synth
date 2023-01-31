using GoSynth.ViewModels;

namespace GoSynth;

public partial class SoundsPage : ContentPage
{
	public SoundsPage()
	{
		InitializeComponent();
		this.BindingContext = new SoundsViewModel();
	}
}