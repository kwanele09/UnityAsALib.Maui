using Android; // Required for Manifest.Permission
using Android.App;
using Android.Content; // Required for Intent
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Com.Unity3d.Player;
using UnityUaal.Maui.Unity;
using UnityUaalMaui.Unity; // Assuming this is needed for UnityBridge

namespace UnityUaal.Maui.Platforms.Android;

[Activity(Label = "UnityActivity",
          MainLauncher = false,
          ConfigurationChanges = ConfigChanges.Mcc
                                 | ConfigChanges.Mnc
                                 | ConfigChanges.Locale
                                 | ConfigChanges.Touchscreen
                                 | ConfigChanges.Keyboard
                                 | ConfigChanges.KeyboardHidden
                                 | ConfigChanges.Navigation
                                 | ConfigChanges.Orientation
                                 | ConfigChanges.ScreenLayout
                                 | ConfigChanges.UiMode
                                 | ConfigChanges.ScreenSize
                                 | ConfigChanges.SmallestScreenSize
                                 | ConfigChanges.FontScale
                                 | ConfigChanges.LayoutDirection
                                 | ConfigChanges.Density,
          ResizeableActivity = false, // Disable multi-window support
          LaunchMode = LaunchMode.SingleTask)]
[MetaData(name: "unityplayer.UnityActivity", Value = "true")]
[MetaData(name: "notch_support", Value = "true")]
public class UnityActivity : Activity,
                             IUnityPlayerLifecycleEvents,
                             INativeUnityBridge,
                             IUnityPermissionRequestSupport
{
    private UnityPlayerForActivityOrService? player;

    private int _permissionRequestCode = 1000; // start from some number

    // Keep track of PermissionRequest by requestCode
    private readonly Dictionary<int, PermissionRequest> _permissionRequests = new();

    protected override void OnCreate(Bundle? savedInstanceState) // Nullable fix for savedInstanceState
    {
        base.OnCreate(savedInstanceState);

        // Request no title before setting content
        RequestWindowFeature(WindowFeatures.NoTitle);

        // --- FIX: REMOVED MANUAL CAMERA PERMISSION REQUEST FROM HERE ---
        // We will now rely exclusively on Unity's IUnityPermissionRequestSupport interface
        // to trigger permission prompts when Unity needs them.
        // This avoids redundancy and potential conflicts with Unity's internal flow.

        player = new UnityPlayerForActivityOrService(this, this);

        SetContentView(player.FrameLayout);
        player.FrameLayout.RequestFocus();

        UnityBridge.RegisterNativeBridge(this);
    }

    public override void OnConfigurationChanged(Configuration newConfig)
    {
        Log.Info(GetType().Name, $"{nameof(OnConfigurationChanged)}|{GetHashCode()}|{newConfig}");
        base.OnConfigurationChanged(newConfig);
        player?.ConfigurationChanged(newConfig);
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        Log.Info(GetType().Name, $"{nameof(OnWindowFocusChanged)}|{GetHashCode()}|hasFocus={hasFocus}");
        base.OnWindowFocusChanged(hasFocus);
        player?.WindowFocusChanged(hasFocus);
    }

    protected override void OnNewIntent(Intent? intent) // Nullable fix for intent
    {
        Log.Info(GetType().Name, $"{nameof(OnNewIntent)}|{GetHashCode()}|Intent={intent?.Action},{intent?.Flags}");
        Intent = intent;
        player?.NewIntent(intent);
    }

    protected override void OnStop()
    {
        Log.Info(GetType().Name, $"{nameof(OnStop)}|{GetHashCode()}|");
        base.OnStop();

        Log.Info(GetType().Name, "UnityPlayer.Pause");
        player?.Pause();
    }

    protected override void OnPause()
    {
        Log.Info(GetType().Name, $"{nameof(OnPause)}|{GetHashCode()}|");
        base.OnPause();

        Log.Info(GetType().Name, "UnityPlayer.Pause");
        player?.Pause();
    }

    protected override void OnStart()
    {
        Log.Info(GetType().Name, $"{nameof(OnStart)}|{GetHashCode()}|");
        base.OnStart();

        Log.Info(GetType().Name, "UnityPlayer.Resume");
        player?.Resume();
    }

    protected override void OnResume()
    {
        Log.Info(GetType().Name, $"{nameof(OnResume)}|{GetHashCode()}|");
        base.OnResume();

        Log.Info(GetType().Name, "UnityPlayer.Resume");
        player?.Resume();
    }

    protected override void OnDestroy()
    {
        Log.Info(GetType().Name, $"{nameof(OnDestroy)}|{GetHashCode()}|");

        if (player != null)
        {
            if (player is IDisposable disposablePlayer)
            {
                disposablePlayer.Dispose();
                Log.Info(GetType().Name, "UnityPlayerForActivityOrService Disposed successfully.");
            }
            else
            {
                Log.Warn(GetType().Name, "UnityPlayerForActivityOrService does not implement IDisposable directly. " +
                                         "Ensure its resources are properly released via its own API or native finalization.");
            }
            player = null;
        }

        UnityBridge.RegisterNativeBridge(null);

        base.OnDestroy();
    }

    public void OnUnityPlayerQuitted()
    {
        Log.Info(GetType().Name, $"{nameof(OnUnityPlayerQuitted)}|{GetHashCode()}|");
    }

    public void OnUnityPlayerUnloaded()
    {
        Log.Info(GetType().Name, $"{nameof(OnUnityPlayerUnloaded)}|{GetHashCode()}|");
        MoveTaskToBack(true);
    }

    public void SendContent(string eventName, string? eventContent)
    {
        var content = eventName + "|" + (eventContent ?? string.Empty);
        UnityPlayer.UnitySendMessage("Bridge", "ReceiveContent", content);
    }

    public void RequestPermissions(PermissionRequest request)
    {
        int requestCode = _permissionRequestCode++;
        _permissionRequests[requestCode] = request;

        player?.AddPermissionRequest(request);

        // --- FIX: REQUEST ALL COMMON AR/CAMERA PERMISSIONS HERE ---
        // Since we cannot read the specific permissions from the 'PermissionRequest' object
        // through its C# binding, we will proactively request all permissions typically needed
        // for AR and camera functionality when Unity triggers this method.
        string[] permissionsToRequest = new string[]
        {
            Manifest.Permission.Camera,
            Manifest.Permission.RecordAudio, // Often needed for AR/video
            Manifest.Permission.AccessFineLocation // Often needed for Geospatial AR
        };
        // --- END FIX ---

        if (permissionsToRequest.Length == 0)
        {
            Log.Error(GetType().Name, $"Unity PermissionRequest for code {requestCode} led to no permissions to ask for. Skipping request.");
            return;
        }

        Log.Info(GetType().Name, $"Requesting permissions for Unity (Code {requestCode}): {string.Join(", ", permissionsToRequest)}");
        RequestPermissions(permissionsToRequest, requestCode); // This is the Android API call
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (_permissionRequests.TryGetValue(requestCode, out var request))
        {
            player?.PermissionResponse(this, requestCode, permissions, grantResults?.Select(g => (int)g).ToArray() ?? Array.Empty<int>());
            _permissionRequests.Remove(requestCode);
        }
    }

    public override bool DispatchKeyEvent(KeyEvent? e) // Nullable fix
    {
        Log.Info(GetType().Name, $"{nameof(DispatchKeyEvent)}|{GetHashCode()}|{e?.Action}");
        if (e?.Action == KeyEventActions.Multiple)
        {
            return player?.InjectEvent(e) ?? base.DispatchKeyEvent(e);
        }
        return base.DispatchKeyEvent(e);
    }

    public override bool OnKeyUp(Keycode keyCode, KeyEvent? e) // Nullable fix
    {
        Log.Info(GetType().Name, nameof(OnKeyUp));
        return player?.InjectEvent(e) ?? base.OnKeyUp(keyCode, e);
    }

    public override bool OnKeyDown(Keycode keyCode, KeyEvent? e) // Nullable fix
    {
        Log.Info(GetType().Name, nameof(OnKeyDown));
        return player?.InjectEvent(e) ?? base.OnKeyDown(keyCode, e);
    }

    public override bool OnTouchEvent(MotionEvent? e) // Nullable fix
    {
        Log.Info(GetType().Name, nameof(OnTouchEvent));
        return player?.InjectEvent(e) ?? base.OnTouchEvent(e);
    }

    public override bool OnGenericMotionEvent(MotionEvent? e) // Nullable fix
    {
        Log.Info(GetType().Name, nameof(OnGenericMotionEvent));
        return player?.InjectEvent(e) ?? base.OnGenericMotionEvent(e);
    }
}