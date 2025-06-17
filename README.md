Integrating Unity AR with .NET MAUI: A Comprehensive Guide
This repository demonstrates a successful integration of a Unity Augmented Reality (AR) project into a .NET MAUI application, specifically targeting Android. This guide will walk you through the entire process, including common pitfalls and solutions, enabling you to build your own hybrid AR apps.

Table of Contents
Project Overview

Prerequisites

Part 1: Unity Project Setup & Export

Unity Project Configuration

AR Foundation & ARCore Extensions

Unity Player Settings for Android

Exporting as Android Library

Part 2: Processing Unity Export in Android Studio (Optional but Recommended)

Why Android Studio?

[Importing the unityLibrary Module](#impo rting-the-unitylibrary-module)

Extracting AARs and .so Files

Troubleshooting Android Studio Build Issues

Part 3: .NET MAUI Project Setup & Integration

Create Your MAUI Project

Adding Android Libraries (AARs)

Adding Native Libraries (.so files)

Custom UnityActivity.cs Implementation

Updating AndroidManifest.xml

Launching Unity from MAUI

Part 4: Troubleshooting Common Issues

Type is defined multiple times Errors (Kotlin, GameTextInput, etc.)

ClassNotFoundException: com.unity3d.plugin.UnityAndroidPermissions

compileSdk Version Warnings

Black Screen after Unity Splash (Camera Not Initializing)

Debugging with Logcat

1. Project Overview
This guide aims to provide a robust method for embedding Unity AR experiences within a .NET MAUI application. The key is to correctly integrate Unity's exported Android libraries (.aar files) and native shared objects (.so files) into the MAUI project, and to properly manage the Android lifecycle and permissions.

2. Prerequisites
Before you start, ensure you have the following installed:

Unity Hub & Unity Editor: A version compatible with AR Foundation (e.g., Unity 2022.3.x LTS or newer).

Android Build Support for Unity: Installed via Unity Hub (includes Android SDK & NDK tools).

Android Studio: Latest stable version (for processing Unity export and general Android SDK management).

Visual Studio: Latest stable version with the ".NET Multi-platform App UI development" workload installed.

Physical Android Device: An ARCore-compatible device for testing, with USB debugging enabled.

3. Part 1: Unity Project Setup & Export
Unity Project Configuration
Create a New Unity Project: Start with a 3D (URP or HDRP) or a dedicated AR template.

Install AR Foundation: Go to Window > Package Manager. Ensure "Unity Registry" is selected. Install AR Foundation (latest verified compatible version).

Install ARCore Extensions: In the Package Manager, install ARCore Extensions (latest verified compatible version). This is crucial for ARCore-specific features.

Scene Setup:

Ensure your scene contains an AR Session and an AR Session Origin GameObject.

Your Main Camera should be replaced or configured by the AR Session Origin to be an AR Camera.

Add any AR interaction scripts (e.g., AR Plane Manager, AR Raycast Manager) and your AR content.

Unity Player Settings for Android
Go to File > Build Settings (ensure Android is selected as platform, Switch Platform if needed). Then click Player Settings....

Company Name & Product Name: Set these under Project Settings > Player > Company Name and Product Name. (e.g., DefaultCompany, UnityApp). These will be part of your Android package name.

Other Settings:

Minimum API Level: Set to API 24 (Android 7.0) or higher, as required by ARCore.

Target API Level: Set to the highest API level officially supported by your Android Gradle Plugin version. (e.g., API 34 for Android 14). This helps prevent compileSdk warnings in Android Studio later.

Scripting Backend: Set to IL2CPP. This is essential for performance and native library compatibility.

Target Architectures: Check ARM64. armeabi-v7a is also good for broader compatibility, but ARM64 is increasingly standard.

Stripping Level: For development, start with Disabled or Minimal to avoid IL2CPP stripping issues that can cause SIGSEGV crashes. You can increase this for release builds later, with caution.

Configuration:

Internet Access: Set to Required.

Write Permission: Set to External (SDCard) (if saving media).

XR Plug-in Management:

Under Project Settings > XR Plug-in Management > Android tab, ensure ARCore is checked. This tells Unity to include the necessary ARCore libraries.

Exporting as Android Library
In File > Build Settings, click Export Project.

Check Export Android Project.

Click Export.

Choose a destination folder (e.g., C:\UnityAndroidExport). This will create a Gradle project structure containing your unityLibrary and launcher modules.

4. Part 2: Processing Unity Export in Android Studio (Optional but Recommended)
Why Android Studio?
Unity exports a full Gradle project. Android Studio allows you to:

Build the Unity library (.aar) and test it standalone to verify functionality.

Easily inspect the generated build.gradle files.

Extract the necessary .aar and .so files for direct inclusion in your MAUI project. This avoids conflicts that might arise from NuGet packages.

Importing the unityLibrary Module
Open Android Studio.

Select File > Open.

Navigate to your Unity export folder (e.g., C:\UnityAndroidExport) and select the unityLibrary subfolder. Click OK.

Android Studio will import it as a Gradle module. Let it sync and download dependencies.

Extracting AARs and .so Files
After a successful build of unityLibrary in Android Studio:

Locate Generated AARs:

In Android Studio, find your unityLibrary module in the Project view.

Navigate to unityLibrary/build/outputs/aar/. You'll find unityLibrary-release.aar (or unityLibrary-debug.aar). This is your main Unity library.

Other essential AARs (like UnityARCore.aar, games-activity-4.0.0.aar, unityandroidpermissions.aar, core-1.49.0.aar) will typically be found within the unityLibrary/libs/ folder of your original Unity export, or within the Gradle cache (~/.gradle/caches/). For a robust MAUI integration, it's best to copy these directly.

Locate Generated .so Files:

Navigate to unityLibrary/build/intermediates/cmake/release/obj/ (or debug/obj/).

Inside, you'll find folders for each ABI (e.g., arm64-v8a, armeabi-v7a).

Within each ABI folder, you'll find .so files like libil2cpp.so, libunity.so, libmain.so, libgame.so, libarcore_sdk_c.so, etc.

Recommended Approach: Create a dedicated folder in your MAUI project (e.g., UnityUaal.Maui/Jars for AARs and UnityUaal.Maui/NativeLibs for .so files) and copy these collected files into them.

Troubleshooting Android Studio Build Issues
Plugin [id: 'com.android.application', version: 'X.Y.Z', apply: false] was not found:

Cause: The AGP version specified is too new or not in the configured repositories.

Solution: In your project-level build.gradle (the one in C:\Users\kwane\OneDrive\Desktop\Plugins\Library\build.gradle), change the AGP version (id 'com.android.application' version 'X.Y.Z') to a stable version like 8.4.1 or 8.5.0. Then Sync Gradle.

We recommend using a newer Android Gradle plugin to use compileSdk = 36:

Cause: Your compileSdk is higher than what your current AGP version was officially tested against.

Solution: In your module-level build.gradle (e.g., unityLibrary/build.gradle), downgrade compileSdk to 35 (or 34 for Android 14) to match the AGP's tested compatibility. Keep targetSdk at the desired highest level.

Duplicate class ... found in modules kotlin-stdlib-... (or games-activity-):

Cause: Multiple versions of the same library (like Kotlin Standard Library or games-activity) are being included by different dependencies.

Solution: This is complex.

For Kotlin: In your UnityUaal.Maui.csproj, add ExcludeAssets="Compile" for Xamarin.Kotlin.StdLib, Xamarin.Kotlin.StdLib.Jdk7, Xamarin.Kotlin.StdLib.Jdk8 PackageReference updates.

For games-activity: If you are explicitly including games-activity-4.0.0.aar, then ensure no NuGet package (Xamarin.GooglePlayServices.Games is a common culprit) is also pulling in a different version. You might need to remove the conflicting NuGet PackageReference if your explicitly included AAR is sufficient.

5. Part 3: .NET MAUI Project Setup & Integration
Create Your MAUI Project
In Visual Studio, create a new ".NET MAUI App" project.

Add a new project to your solution, either a Class Library (for AndroidBridge if you separate your native bindings) or directly use your MAUI project.

Adding Android Libraries (AARs)
These AARs are the packaged Android library modules that Unity exports or relies on.

Create a folder named Jars (or similar) inside your UnityUaal.Maui project directory (e.g., UnityUaal.Maui/Jars).

Copy the following AAR files into this Jars folder:

core-1.49.0.aar

games-activity-4.0.0.aar

UnityARCore.aar

unityLibrary-release.aar

unityandroidpermissions.aar (CRUCIAL for ClassNotFoundException fix)

Edit your UnityUaal.Maui.csproj file:

Right-click UnityUaal.Maui project in Solution Explorer > Edit Project File.

Add an <ItemGroup> for AndroidLibrary references:

<ItemGroup>
    <AndroidLibrary Include="Jars\core-1.49.0.aar" />
    <AndroidLibrary Include="Jars\games-activity-4.0.0.aar" />
    <AndroidLibrary Include="Jars\UnityARCore.aar" />
    <AndroidLibrary Include="Jars\unityLibrary-release.aar" />
    <AndroidLibrary Include="Jars\unityandroidpermissions.aar" />
</ItemGroup>

Ensure any existing PackageReference entries for related Google Play Services are managed to avoid conflicts (see Troubleshooting section).

Adding Native Libraries (.so files)
These are the compiled native code libraries from Unity and ARCore.

Create a folder structure inside your UnityUaal.Maui project: NativeLibs/arm64-v8a and NativeLibs/armeabi-v7a.

Copy the following .so files into their respective ABI folders (e.g., libil2cpp.so goes into both arm64-v8a and armeabi-v7a):

lib_burst_generated.so

libarcore_sdk_c.so

libarcore_sdk_jni.so

libarpresto_api.so

libc++_shared.so

libgame.so

libil2cpp.so

libmain.so

libunity.so

libUnityARCore.so

Edit your UnityUaal.Maui.csproj file:

Visual Studio usually automatically includes .so files if placed in NativeLibs with correct ABI subfolders. However, explicitly defining them ensures proper inclusion. Add an <ItemGroup> for AndroidNativeLibrary:

<ItemGroup>
    <AndroidNativeLibrary Include="NativeLibs\arm64-v8a\*.so" />
    <AndroidNativeLibrary Include="NativeLibs\armeabi-v7a\*.so" />
    <!-- Add more ABIs if needed, e.g., x86, x86_64 -->
</ItemGroup>

Custom UnityActivity.cs Implementation
You need a custom Android.App.Activity to host the UnityPlayer. This file (UnityActivity.cs) will be in your UnityUaal.Maui/Platforms/Android folder.

Key adjustments to your UnityActivity.cs:

Remove manual permission requests from OnCreate: Rely entirely on Unity's IUnityPermissionRequestSupport interface.

Implement IUnityPermissionRequestSupport: This interface is called by Unity when it needs permissions. Its RequestPermissions method must take the list of permissions Unity needs and pass them to the Android system.

Correct Lifecycle Management: Ensure UnityPlayer methods (Pause, Resume, NewIntent, etc.) are correctly called in your Activity's lifecycle overrides.

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
        UnityBridge.RegisterNativeBridge(null); // Unregister native bridge
        base.OnDestroy();
    }

    // --- IUnityPlayerLifecycleEvents (from Unity) ---
    public void OnUnityPlayerQuitted()
    {
        Log.Info(GetType().Name, $"{nameof(OnUnityPlayerQuitted)}|{GetHashCode()}|");
    }

    public void OnUnityPlayerUnloaded()
    {
        Log.Info(GetType().Name, $"{nameof(OnUnityPlayerUnloaded)}|{GetHashCode()}|");
        MoveTaskToBack(true);
    }

    // --- INativeUnityBridge (Your Custom Bridge) ---
    public void SendContent(string eventName, string? eventContent)
    {
        var content = eventName + "|" + (eventContent ?? string.Empty);
        UnityPlayer.UnitySendMessage("Bridge", "ReceiveContent", content);
    }

    // --- IUnityPermissionRequestSupport (Critical for Permissions) ---
    public void RequestPermissions(PermissionRequest request)
    {
        int requestCode = _permissionRequestCode++;
        _permissionRequests[requestCode] = request;
        player?.AddPermissionRequest(request); // Important: Pass the request to the UnityPlayer wrapper

        // --- CRITICAL FIX: Request all necessary AR/Camera permissions here ---
        // Since the C# binding for PermissionRequest doesn't expose the permissions array,
        // we proactively request all common AR/camera permissions when Unity triggers this.
        string[] permissionsToRequest = new string[]
        {
            Manifest.Permission.Camera,
            Manifest.Permission.RecordAudio, // Often needed for AR/video recording
            Manifest.Permission.AccessFineLocation // Often needed for Geospatial AR and precise tracking
        };

        if (permissionsToRequest.Length == 0)
        {
            Log.Error(GetType().Name, $"Unity PermissionRequest for code {requestCode} contained no permissions to ask for. Skipping request.");
            return;
        }

        Log.Info(GetType().Name, $"Requesting permissions for Unity (Code {requestCode}): {string.Join(", ", permissionsToRequest)}");
        RequestPermissions(permissionsToRequest, requestCode); // This is the actual Android API call
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        if (_permissionRequests.TryGetValue(requestCode, out var request))
        {
            // Pass the results back to Unity's PermissionResponse handler.
            // grantResults need to be converted to int[] for Unity's expected format.
            player?.PermissionResponse(this, requestCode, permissions, grantResults?.Select(g => (int)g).ToArray() ?? Array.Empty<int>());
            _permissionRequests.Remove(requestCode);
        }
    }

    // --- Android Input Events (Forwarding to UnityPlayer) ---
    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        Log.Info(GetType().Name, $"{nameof(DispatchKeyEvent)}|{GetHashCode()}|{e?.Action}");
        if (e?.Action == KeyEventActions.Multiple)
        {
            return player?.InjectEvent(e) ?? base.DispatchKeyEvent(e);
        }
        return base.DispatchKeyEvent(e);
    }

    public override bool OnKeyUp(Keycode keyCode, KeyEvent? e)
    {
        Log.Info(GetType().Name, nameof(OnKeyUp));
        return player?.InjectEvent(e) ?? base.OnKeyUp(keyCode, e);
    }

    public override bool OnKeyDown(Keycode keyCode, KeyEvent? e)
    {
        Log.Info(GetType().Name, nameof(OnKeyDown));
        return player?.InjectEvent(e) ?? base.OnKeyDown(keyCode, e);
    }

    public override bool OnTouchEvent(MotionEvent? e)
    {
        Log.Info(GetType().Name, nameof(OnTouchEvent));
        return player?.InjectEvent(e) ?? base.OnTouchEvent(e);
    }

    public override bool OnGenericMotionEvent(MotionEvent? e)
    {
        Log.Info(GetType().Name, nameof(OnGenericMotionEvent));
        return player?.InjectEvent(e) ?? base.OnGenericMotionEvent(e);
    }
}
