## Integrating Unity AR with .NET MAUI: A Comprehensive Guide

This Markdown document provides a structured, developer-friendly summary of the process for integrating a Unity Augmented Reality (AR) project into a .NET MAUI application, with a focus on Android deployment. It covers all major steps, from Unity setup to MAUI integration and troubleshooting.

---

### Table of Contents

- Project Overview
- Prerequisites
- Unity Project Setup \& Export
- Processing Unity Export in Android Studio
- .NET MAUI Project Setup \& Integration
- Troubleshooting Common Issues

---

## Project Overview

This guide details how to embed Unity AR experiences within a .NET MAUI app by integrating Unity's exported Android libraries (.aar files) and native shared objects (.so files), while managing the Android lifecycle and permissions[^1].

---

## Prerequisites

Ensure you have the following tools installed:

- **Unity Hub \& Unity Editor** (compatible with AR Foundation, e.g., Unity 2022.3.x LTS or newer)
- **Android Build Support for Unity** (via Unity Hub)
- **Android Studio** (latest stable version)
- **Visual Studio** (with ".NET Multi-platform App UI development" workload)
- **Physical Android Device** (ARCore-compatible, USB debugging enabled)[^1]

---

## Unity Project Setup \& Export

**1. Unity Project Configuration**

- Create a new Unity 3D or AR template project.
- Install **AR Foundation** and **ARCore Extensions** via the Package Manager.
- Set up your scene with:
    - AR Session and AR Session Origin GameObjects
    - AR Camera (configured via AR Session Origin)
    - AR interaction scripts (e.g., AR Plane Manager, AR Raycast Manager)[^1]

**2. Unity Player Settings for Android**

- Set Company Name and Product Name.
- Minimum API Level: **API 24** or higher.
- Target API Level: Highest supported by your Android Gradle Plugin (e.g., API 34 for Android 14).
- Scripting Backend: **IL2CPP**
- Target Architectures: **ARM64** (optionally also armeabi-v7a)
- Stripping Level: Start with Disabled or Minimal for development.
- Internet Access: Required.
- Write Permission: External (SDCard) if saving media.
- Enable ARCore under XR Plug-in Management for Android[^1].

**3. Exporting as Android Library**

- In Build Settings, select Android and check **Export Android Project**.
- Export to a destination folder (e.g., `C:\UnityAndroidExport`)[^1].

---

## Processing Unity Export in Android Studio

**Why Android Studio?**

- Build and test the Unity library (.aar) standalone.
- Inspect and adjust build.gradle files.
- Extract .aar and .so files for MAUI integration[^1].

**Key Steps:**

- Open the exported `unityLibrary` module in Android Studio.
- Build the module and locate the generated `.aar` files (e.g., `unityLibrary-release.aar`).
- Extract additional required `.aar` files from `unityLibrary/libs/` or Gradle cache.
- Locate `.so` files in `unityLibrary/build/intermediates/cmake/release/obj/` by ABI (e.g., `arm64-v8a`, `armeabi-v7a`)[^1].

---

## .NET MAUI Project Setup \& Integration

**1. Create Your MAUI Project**

- Use Visual Studio to create a new ".NET MAUI App" project[^1].

**2. Adding Android Libraries (.AARs)**

- Create a `Jars` folder in your MAUI project.
- Copy required `.aar` files:
    - `core-1.49.0.aar`
    - `games-activity-4.0.0.aar`
    - `UnityARCore.aar`
    - `unityLibrary-release.aar`
    - `unityandroidpermissions.aar`
- Edit your `.csproj` file to include:

```xml
<ItemGroup>
    <AndroidLibrary Include="Jars\core-1.49.0.aar" />
    <AndroidLibrary Include="Jars\games-activity-4.0.0.aar" />
    <AndroidLibrary Include="Jars\UnityARCore.aar" />
    <AndroidLibrary Include="Jars\unityLibrary-release.aar" />
    <AndroidLibrary Include="Jars\unityandroidpermissions.aar" />
</ItemGroup>
```

Manage PackageReferences to avoid conflicts[^1].

**3. Adding Native Libraries (.SO files)**

- Create `NativeLibs/arm64-v8a` and `NativeLibs/armeabi-v7a` folders.
- Copy all required `.so` files into the correct ABI folders.
- Add to `.csproj`:

```xml
<ItemGroup>
    <AndroidNativeLibrary Include="NativeLibs\arm64-v8a\*.so" />
    <AndroidNativeLibrary Include="NativeLibs\armeabi-v7a\*.so" />
</ItemGroup>
```

Visual Studio may auto-include them, but explicit definition is safer[^1].

**4. Custom UnityActivity.cs Implementation**

- Implement a custom `UnityActivity` in `Platforms/Android`.
- Key points:
    - Remove manual permission requests from `OnCreate`.
    - Implement `IUnityPermissionRequestSupport` for permissions.
    - Correctly manage UnityPlayer lifecycle methods in activity overrides.
- Example (partial):

```csharp
[Activity(Label = "UnityActivity", ...)]
public class UnityActivity : Activity, IUnityPlayerLifecycleEvents, INativeUnityBridge, IUnityPermissionRequestSupport
{
    private UnityPlayerForActivityOrService? player;
    // ... (lifecycle overrides and bridge setup)
}
```

Ensure you have supporting bridge classes as needed[^1].

---

## Troubleshooting Common Issues

- **Type is defined multiple times**: Exclude duplicate dependencies in `.csproj`.
- **ClassNotFoundException: com.unity3d.plugin.UnityAndroidPermissions**: Ensure `unityandroidpermissions.aar` is included.
- **compileSdk Version Warnings**: Align compileSdk with AGP version compatibility.
- **Black Screen after Unity Splash**: Check camera permissions and ARCore initialization.
- **Debugging**: Use Logcat for runtime diagnostics[^1].

---

> For detailed code samples, error solutions, and further explanations, refer to the full guide above or your project’s README.

---

This Markdown file can be used as a project README or developer integration guide for Unity AR + .NET MAUI on Android[^1].

<div style="text-align: center">⁂</div>
