using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace TestDrive
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

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddTransient<RecordingPage2>();
            builder.Services.AddSingleton<IAudioManager, AudioManager>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if WINDOWS
            builder.Services.AddSingleton<BluetoothBatteryReader>();
#endif

            return builder.Build();
        }
    }
}
