using GoSynth.Views;

namespace GoSynth
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SoundsPage), typeof(SoundsPage));
            Routing.RegisterRoute(nameof(SoundPage), typeof(SoundPage));
            Routing.RegisterRoute(nameof(EquationHelp), typeof(EquationHelp));
        }
    }
}