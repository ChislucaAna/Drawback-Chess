using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

#if WINDOWS
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinRT.Interop;
#endif

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
                 .ConfigureLifecycleEvents(lifecycle =>
                 {
#if WINDOWS
                     lifecycle.AddWindows(windows =>
                     {
                         windows.OnWindowCreated(window =>
                         {
                             // 'window' is Microsoft.UI.Xaml.Window
                             var hwnd = WindowNative.GetWindowHandle(window);
                             var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                             var appWindow = AppWindow.GetFromWindowId(windowId);

                             if (appWindow.Presenter is OverlappedPresenter presenter)
                             {
                                 presenter.IsResizable = false;
                             }
                         });
                     });
#endif
                 })
                .UseMauiApp<App>().UseMauiCommunityToolkit();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            SQLitePCL.Batteries_V2.Init();
            builder.Services.AddBlazorWebViewDeveloperTools();
            //builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
