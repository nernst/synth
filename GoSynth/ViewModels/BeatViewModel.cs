
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErnstTech.SoundCore.Sampler;

namespace GoSynth.ViewModels;

public partial class BeatViewModel : ObservableObject
{
    static readonly Color FullColorDefault = Color.FromRgb(255, 0, 0);
    static readonly Color HalfColorDefault = Color.FromRgb(255, 128, 128);
    static readonly Color OffColorDefault = Color.FromRgb(128, 128, 128);

    public Beat Beat { get; set; }

    public double OffLevel { get => Beat.Off; }
    public double HalfLevel { get => Beat.Half; }
    public double FullLevel { get => Beat.Full; }

    [ObservableProperty]
    Color fullColor = FullColorDefault;

    [ObservableProperty]
    Color halfColor = HalfColorDefault;

    [ObservableProperty]
    Color offColor = OffColorDefault;

    [ObservableProperty]
    Color activeColor = OffColorDefault;

    public double Level
    {
        get => Beat.Level;
        set => SetProperty(Beat.Level, value, Beat, (u, v) => u.Level = v);
    }

    public BeatViewModel(Beat beat)
    {
        Beat = beat;
        SetActiveColor();

        PropertyChanged += (o, a) =>
        {
            if (a.PropertyName?.Equals(nameof(Level)) ?? false)
                SetActiveColor();
        };
    }

    void SetActiveColor()
    {
        ActiveColor = Level switch
        {
            < Beat.Half => OffColor,
            Beat.Half => HalfColor,
            > Beat.Half => FullColor,
            _ => throw new InvalidOperationException($"{nameof(Level)} has invalid value.")
        }; ;
    }

    [RelayCommand]
    void Toggle()
    {
        Level = Level switch
        {
            < Beat.Half => Beat.Half,
            Beat.Half => Beat.Full,
            > Beat.Half => Beat.Off,
            _ => throw new InvalidOperationException($"{nameof(Level)} has invalid value.")
        };
    }
}
