using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace DrawbackChess
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
                })
                .UseMauiApp<App>().UseMauiCommunityToolkit();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
