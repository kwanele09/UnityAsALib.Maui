using Android.App;
using Android.Content.PM;
using Android.OS;

namespace UnityUaal.Maui
{
    [Activity(
        Label = "UnityUaal.Maui",
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize
                             | ConfigChanges.Orientation
                             | ConfigChanges.UiMode
                             | ConfigChanges.ScreenLayout
                             | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            UnityUaalMaui.Unity.Platform.CurrentActivity = this;
        }
    }
}
