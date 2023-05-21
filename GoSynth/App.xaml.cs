 
namespace GoSynth;

public partial class App : Application
{
    const int WindowDefaultWidth = 1080;
    const int WindowDefaultHeight = 800;

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
           
    }

#if WINDOWS
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        window.Width = WindowDefaultWidth;
        window.Height = WindowDefaultHeight;

        return window;
    }
#endif
}