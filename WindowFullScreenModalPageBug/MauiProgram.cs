using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace WindowFullScreenModalPageBug
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
#if WINDOWS
                .ConfigureLifecycleEvents(events =>
                {
                    // Make sure to add "using Microsoft.Maui.LifecycleEvents;" in the top of the file
                    events.AddWindows(windowsLifecycleBuilder =>
                    {
                        windowsLifecycleBuilder.OnWindowCreated(window =>
                        {
                            window.ExtendsContentIntoTitleBar = false;

#if WORKAROUND
							// Workaround: (worked on doubly title bars problem)
							if (window.Content is FrameworkElement elem) {
								elem.Margin = new(0, -32, 0, 0);
								Canvas.SetZIndex(elem, 0);
							}
#endif // WORKAROUND

                            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                            switch (appWindow.Presenter)
                            {
                                case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                                    overlappedPresenter.SetBorderAndTitleBar(false, false);
                                    overlappedPresenter.Maximize();
                                    break;
                            }
                        });
                    });
                })
#endif
                .ConfigureFonts(fonts =>
                {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
