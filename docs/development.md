# Guide

## Commands

To fetch the generated mazes from the device, you can use the following command:

```bash
adb pull /storage/emulated/0/Pictures/PuzzleDazzle/ C:\Users\AKazlou\Downloads\mazes
```

To build and deploy app to Android Emulator device, use the following command:

```bash
dotnet build src/PuzzleDazzle/PuzzleDazzle.csproj -f net9.0-android -t:Run
```

## Installing a Debug APK on a Physical Device

### Background

Two things are required for a Debug APK to work when installed standalone on a physical device:

1. **Production keystore signing** — Android's Play Protect blocks sideloading of debug-signed APKs with a misleading "app not compatible with your device" message. Using the production keystore avoids this. The Debug build in this project is already configured to use it.

2. **Explicit runtime identifier (`-r android-arm64`)** — By default, `dotnet publish` in Debug mode produces a framework-dependent build that relies on .NET MAUI's "Fast Deployment" mechanism (assemblies pushed separately via adb). Without `-r`, native libraries and assemblies are not bundled in the APK, causing an immediate crash on launch. Passing `-r android-arm64` forces a self-contained build with everything embedded.

### Step 1 — Publish the Debug APK

```powershell
dotnet publish src/PuzzleDazzle/PuzzleDazzle.csproj -f net9.0-android -c Debug -r android-arm64
```

The signed APK will be at:
```
src\PuzzleDazzle\bin\Debug\net9.0-android\android-arm64\app.mazeledazzle-Signed.apk
```

### Step 2 — Install via adb

Connect the device via USB (USB debugging must be enabled), then run:

```powershell
adb install --no-incremental "src\PuzzleDazzle\bin\Debug\net9.0-android\android-arm64\app.mazeledazzle-Signed.apk"
```

> Note: use `--no-incremental` to avoid a user-confirmation dialog that adb's incremental installer triggers on some devices.

### Alternative — Sideload via file manager

Because the APK is signed with the production keystore, you can also transfer the APK file to the device and install it through the file manager — Play Protect will not block it.
