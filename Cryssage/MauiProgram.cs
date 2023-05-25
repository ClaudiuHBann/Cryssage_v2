#if DEBUG
using System.Runtime.InteropServices;
#endif

using Cryssage.Views;
using CommunityToolkit.Maui;

namespace Cryssage;

public static class MauiProgram
{
#if DEBUG
    [DllImport("kernel32.dll", SetLastError = true)]
    [return:MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();
#endif

    public static MauiApp CreateMauiApp()
    {
#if DEBUG
        AllocConsole();
#endif

        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
                            {
                                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                                fonts.AddFont("materialdesignicons-webfont.ttf", "IconFontTypes");
                            })
            .UseMauiCommunityToolkit();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<UserView>();

        return builder.Build();
    }
}
