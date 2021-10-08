using Booliba.Client.Maui;
using Foundation;
using Microsoft.Maui;

namespace Booliba.Client.Maui.Platforms.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}