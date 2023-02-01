using GoSynth.ViewModels;
using System.Diagnostics;

namespace GoSynth;

public partial class SoundPage : ContentPage
{
	public SoundPage()
	{
		InitializeComponent();
		var model = new SoundViewModel();
		this.BindingContext = model;

		model.ConfirmDiscard = async (a) =>
		{
			try
			{
				a.Confirmed = await this.DisplayAlert("Discard pending changes?", "Discard pending changes?", "Yes", "No"); 
			}
			catch(Exception ex)
			{
				Debug.WriteLine($"While confirming discard, caught exception: {ex}");
			}
		};
	}
}