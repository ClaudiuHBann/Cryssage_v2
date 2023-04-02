#if DEBUG
using System.Runtime.InteropServices;
#endif

using Cryssage.Views;

namespace Cryssage;

public static class MauiProgram
{
#if DEBUG
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();
#endif

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("materialdesignicons-webfont.ttf", "IconFontTypes");
            });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<UserView>();

#if DEBUG
        AllocConsole();
#endif

        return builder.Build();
    }
}
