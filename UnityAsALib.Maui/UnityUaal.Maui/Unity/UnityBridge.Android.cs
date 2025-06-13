#if __ANDROID__

using Android.App;
using Android.Content;
using Android.Util;
using Java.Lang;
using UnityUaal.Maui;

namespace UnityUaalMaui.Unity
{
    public static partial class Platform
    {
        public static Activity? CurrentActivity { get; set; }
    }

    public static partial class UnityBridge
    {
        public static void ShowMainWindow()
        {
            var currentActivity = Platform.CurrentActivity;
            if (currentActivity == null)
            {
                Log.Warn("UnityBridge", "CurrentActivity is null");
                return;
            }

            var intent = new Intent(currentActivity, Class.FromType(typeof(MainActivity)));
            intent.AddFlags(ActivityFlags.ReorderToFront | ActivityFlags.SingleTop);
            currentActivity.StartActivity(intent);
        }


        public static void ShowUnityWindow()
        {
            var currentActivity = Platform.CurrentActivity;
            if (currentActivity == null)
            {
                Log.Warn("UnityBridge", "CurrentActivity is null");
                return;
            }

            var unityClass = Class.FromType(typeof(Com.Unity3d.Player.UnityPlayerGameActivity));
            var intent = new Intent(currentActivity, unityClass);
            intent.AddFlags(ActivityFlags.ReorderToFront);
            currentActivity.StartActivity(intent);
        }

    }
}

#endif
