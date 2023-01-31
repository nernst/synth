using GoSynth.Synthesis;
using Plugin.Maui.Audio;
using Microsoft.Extensions.Logging;
using GoSynth.ViewModels;
using GoSynth.Models;

namespace GoSynth
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IAudioManager>(AudioManager.Current);
            builder.Services.AddSingleton<Synthesizer>();
            builder.Services.AddSingleton<MainPage>();

            // models
            builder.Services.AddSingleton<SoundManager>();

            // view models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<SoundsViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}