## Integrating Unity AR with .NET MAUI: A Comprehensive Guide

This README outlines the process of integrating a Unity AR project with a .NET MAUI application. It covers setup in Unity, preparing Android libraries, configuring your MAUI project, and troubleshooting.

---

### Table of Contents

- [Project Overview](#project-overview)
- [Prerequisites](#prerequisites)
- [Part 1: Unity Project Setup \& Export](#part-1-unity-project-setup--export)
- [Part 2: Processing Unity Export in Android Studio (Optional but Recommended)](#part-2-processing-unity-export-in-android-studio-optional-but-recommended)
- [Part 3: .NET MAUI Project Setup \& Integration](#part-3-net-maui-project-setup--integration)
- [Part 4: Troubleshooting Common Issues](#part-4-troubleshooting-common-issues)

---

## Project Overview

This guide provides a robust method for embedding Unity AR experiences within a .NET MAUI application. The key is to correctly integrate Unity's exported Android libraries (`.aar` files) and native shared objects (`.so` files) into the MAUI project, and to properly manage the Android lifecycle and permissions.

---

## Prerequisites

Ensure you have the following installed:

- **Unity Hub \& Unity Editor:** Compatible with AR Foundation (e.g., Unity 2022.3.x LTS or newer)
- **Android Build Support for Unity:** Includes Android SDK \& NDK tools
- **Android Studio:** Latest stable version
- **Visual Studio:** Latest stable version with ".NET Multi-platform App UI development" workload
- **Physical Android Device:** ARCore-compatible, with USB debugging enabled

---

## Part 1: Unity Project Setup \& Export

### Unity Project Configuration

- **Create a New Unity Project:** Use 3D (URP or HDRP) or a dedicated AR template.
- **Install AR Foundation:** Window > Package Manager > Unity Registry > AR Foundation (latest verified version).
- **Install ARCore Extensions:** Package Manager > ARCore Extensions (latest verified version).


#### Scene Setup

- Ensure your scene contains an **AR Session** and an **AR Session Origin** GameObject.
- Main Camera should be replaced or configured by the AR Session Origin to be an AR Camera.
- Add AR interaction scripts (e.g., AR Plane Manager, AR Raycast Manager) and AR content.


### Unity Player Settings for Android

- **Build Settings:** File > Build Settings (select Android, Switch Platform if needed), then Player Settings...
- **Company Name \& Product Name:** Set under Project Settings > Player.
- **Minimum API Level:** API 24 (Android 7.0) or higher.
- **Target API Level:** Highest supported by your Android Gradle Plugin (e.g., API 34 for Android 14).
- **Scripting Backend:** IL2CPP.
- **Target Architectures:** ARM64 (and optionally armeabi-v7a).
- **Stripping Level:** Disabled or Minimal for development.
- **Internet Access:** Required.
- **Write Permission:** External (SDCard) if saving media.
- **XR Plug-in Management:** Project Settings > XR Plug-in Management > Android tab > Ensure ARCore is checked.


### Exporting as Android Library

- **Build Settings:** File > Build Settings > Export Project.
- Check **Export Android Project**.
- Click **Export** and choose a destination folder (e.g., `C:\UnityAndroidExport`).

---

## Part 2: Processing Unity Export in Android Studio (Optional but Recommended)

### Why Android Studio?

- Build and test the Unity library (`.aar`) standalone.
- Inspect generated `build.gradle` files.
- Extract required `.aar` and `.so` files for direct inclusion in your MAUI project.


### Importing the unityLibrary Module

- Open Android Studio.
- File > Open > Navigate to your Unity export folder and select the `unityLibrary` subfolder.
- Let it sync and download dependencies.


### Extracting AARs and .so Files

- **AARs:** `unityLibrary/build/outputs/aar/unityLibrary-release.aar` (or debug). Other essentials (e.g., `UnityARCore.aar`, `games-activity-4.0.0.aar`, `core-1.49.0.aar`, `unityandroidpermissions.aar`) are in `unityLibrary/libs/` or the Gradle cache.
- **.so Files:** `unityLibrary/build/intermediates/cmake/release/obj/` (or debug/obj/), with folders for each ABI (e.g., `arm64-v8a`, `armeabi-v7a`).

**Recommended:** Create dedicated folders in your MAUI project (e.g., `Jars` for AARs, `NativeLibs` for .so files) and copy these files.

### Troubleshooting Android Studio Build Issues

- **AGP Version Not Found:** Downgrade AGP version in `build.gradle` to a stable release.
- **compileSdk Warnings:** Downgrade compileSdk in `build.gradle` to match AGP compatibility.
- **Duplicate Class Errors:** Exclude duplicate dependencies in your `.csproj` (see Troubleshooting).

---

## Part 3: .NET MAUI Project Setup \& Integration

### Create Your MAUI Project

- In Visual Studio, create a new ".NET MAUI App" project.
- Optionally, add a Class Library for native bindings.


### Adding Android Libraries (AARs)

- Create a `Jars` folder in your project.
- Copy these AARs into `Jars`:
    - `core-1.49.0.aar`
    - `games-activity-4.0.0.aar`
    - `UnityARCore.aar`
    - `unityLibrary-release.aar`
    - `unityandroidpermissions.aar`
- Edit your `.csproj`:

```xml
<ItemGroup>
    <AndroidLibrary Include="Jars\core-1.49.0.aar" />
    <AndroidLibrary Include="Jars\games-activity-4.0.0.aar" />
    <AndroidLibrary Include="Jars\UnityARCore.aar" />
    <AndroidLibrary Include="Jars\unityLibrary-release.aar" />
    <AndroidLibrary Include="Jars\unityandroidpermissions.aar" />
</ItemGroup>
```


### Adding Native Libraries (.so files)

- Create folders: `NativeLibs/arm64-v8a` and `NativeLibs/armeabi-v7a`.
- Copy these `.so` files into each ABI folder:
    - `lib_burst_generated.so`
    - `libarcore_sdk_c.so`
    - `libarcore_sdk_jni.so`
    - `libarpresto_api.so`
    - `libc++_shared.so`
    - `libgame.so`
    - `libil2cpp.so`
    - `libmain.so`
    - `libunity.so`
    - `libUnityARCore.so`
- Edit your `.csproj`:

```xml
<ItemGroup>
    <AndroidNativeLibrary Include="NativeLibs\arm64-v8a\*.so" />
    <AndroidNativeLibrary Include="NativeLibs\armeabi-v7a\*.so" />
    <!-- Add more ABIs if needed -->
</ItemGroup>
```


### Custom UnityActivity.cs Implementation

Below is the complete example of a custom `UnityActivity.cs` that integrates UnityPlayer lifecycle management and permission handling in a .NET MAUI Android project:

```csharp
// UnityActivity.cs
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Com.Unity3d.Player;
using Android.Util;
using Android; // For Manifest.Permission
using System;
using System.Linq;
using System.Collections.Generic;

// You might need a UnityBridge.cs and UnityPlayerForActivityOrService.cs
// from your Unity export or custom binding project. Ensure these are correctly
// set up to bridge between C# and Unity's Java code.
// For example:
// public class UnityBridge : INativeUnityBridge { public static INativeUnityBridge Instance; }
// public class UnityPlayerForActivityOrService : UnityPlayer { ... }

namespace UnityUaal.Maui.Platforms.Android;

[Activity(Label = "UnityActivity",
          MainLauncher = false,
          ConfigurationChanges = ConfigChanges.Mcc | ConfigChanges.Mnc | ConfigChanges.Locale | ConfigChanges.Touchscreen
                                 | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.Navigation
                                 | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.UiMode
                                 | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.FontScale
                                 | ConfigChanges.LayoutDirection | ConfigChanges.Density,
          ResizeableActivity = false, // Disable multi-window support to simplify layout
          LaunchMode = LaunchMode.SingleTask)] // Ensures only one instance of this Activity
[MetaData(name: "unityplayer.UnityActivity", Value = "true")]
[MetaData(name: "notch_support", Value = "true")]
public class UnityActivity : Activity,
                             IUnityPlayerLifecycleEvents,
                             INativeUnityBridge, // Your custom bridge interface if you have one
                             IUnityPermissionRequestSupport
{
    private UnityPlayerForActivityOrService? player; // Your wrapper around Com.Unity3d.Player.UnityPlayer
    private int _permissionRequestCode = 1000;
    private readonly Dictionary<int, PermissionRequest> _permissionRequests = new();

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestWindowFeature(WindowFeatures.NoTitle);

        // --- IMPORTANT FIX: Removed manual camera permission request from here. ---
        // Permissions are now handled exclusively by Unity's IUnityPermissionRequestSupport callback.

        player = new UnityPlayerForActivityOrService(this, this); // 'this' for context, 'this' for IUnityPlayerLifecycleEvents
        SetContentView(player.FrameLayout);
        player.FrameLayout.RequestFocus();

        UnityBridge.RegisterNativeBridge(this); // Register your custom bridge (if applicable)
    }

    // --- Android Activity Lifecycle Overrides (Crucial for UnityPlayer) ---
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

    protected override void OnNewIntent(Intent? intent)
    {
        Log.Info(GetType().Name, $"{nameof(OnNewIntent)}|{GetHashCode()}|Intent={intent?.Action},{intent?.Flags}");
        Intent = intent;
        player?.NewIntent(intent);
    }

    protected override void OnStop()
    {
        Log.Info(GetType().Name, $"{nameof(OnStop)}|{GetHashCode()}|");
        base.OnStop();
        Log.Info(GetType().Name, "UnityPlayer.Pause (OnStop)"); // Log for clarity
        player?.Pause();
    }

    protected override void OnPause()
    {
        Log.Info(GetType().Name, $"{nameof(OnPause)}|{GetHashCode()}|");
        base.OnPause();
        Log.Info(GetType().Name, "UnityPlayer.Pause (OnPause)"); // Log for clarity
        player?.Pause();
    }

    protected override void OnStart()
    {
        Log.Info(GetType().Name, $"{nameof(OnStart)}|{GetHashCode()}|");
        base.OnStart();
        Log.Info(GetType().Name, "UnityPlayer.Resume (OnStart)"); // Log for clarity
        player?.Resume();
    }

    protected override void OnResume()
    {
        Log.Info(GetType().Name, $"{nameof(OnResume)}|{GetHashCode()}|");
        base.OnResume();
        Log.Info(GetType().Name, "UnityPlayer.Resume (OnResume)"); // Log for clarity
        player?.Resume();
    }

    protected override void OnDestroy()
    {
        Log.Info(GetType().Name, $"{nameof(OnDestroy)}|{GetHashCode()}|");
        base.OnDestroy();
        player?.Destroy();
    }

    public override void OnBackPressed()
    {
        // Optional: Handle Unity back navigation
        base.OnBackPressed();
    }

    // --- IUnityPermissionRequestSupport Implementation ---
    public void RequestPermissions(string[] permissions, IPermissionRequestResultCallback callback)
    {
        var requestCode = _permissionRequestCode++;
        _permissionRequests[requestCode] = new PermissionRequest
        {
            Permissions = permissions,
            Callback = callback
        };

        RequestPermissions(permissions, requestCode);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (_permissionRequests.TryGetValue(requestCode, out var request))
        {
            bool allGranted = grantResults.All(r => r == Permission.Granted);
            request.Callback.OnRequestPermissionsResult(allGranted);
            _permissionRequests.Remove(requestCode);
        }
    }

    private class PermissionRequest
    {
        public string[] Permissions { get; set; }
        public IPermissionRequestResultCallback Callback { get; set; }
    }
}
```


---

## Part 4: Troubleshooting Common Issues

- **Type is defined multiple times:** Exclude duplicate dependencies in `.csproj`.
- **ClassNotFoundException:** Ensure `unityandroidpermissions.aar` is included.
- **compileSdk Version Warnings:** Align compileSdk with AGP compatibility.
- **Black Screen after Unity Splash:** Check camera permissions and ARCore initialization.
- **Debugging:** Use Logcat for diagnostics.

---

> For detailed code samples, error solutions, and further explanations, refer to this guide and your project’s README.

---
