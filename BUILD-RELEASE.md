# Building Release APK/AAB

This document describes how to build a signed release version of Puzzle Dazzle for distribution.

## Quick Build Command

From the project root directory in Windows PowerShell:

```powershell
dotnet publish src\PuzzleDazzle\PuzzleDazzle.csproj -f net9.0-android -c Release
```

## Output Location

After successful build, the signed artifacts are located in:
```
src/PuzzleDazzle/bin/Release/net9.0-android/publish/
```

Generated files:
- `com.companyname.puzzledazzle-Signed.apk` (~28 MB) - For direct installation or sideloading
- `com.companyname.puzzledazzle-Signed.aab` (~28 MB) - For Google Play Store upload (recommended)

## Prerequisites

### Keystore File
The project uses `puzzledazzle.keystore` in the project root for signing.

**IMPORTANT**: This file contains sensitive signing keys and is excluded from git via `.gitignore`.

**Keystore Details:**
- Location: `puzzledazzle.keystore` (project root)
- Alias: `puzzledazzle`
- Algorithm: RSA 2048-bit
- Validity: 10,000 days (~27 years)
- Owner: CN=Aliaksei Kazlou, OU=Personal, O=Personal, L=Wysoka, ST=Dolnoslaskie, C=PL

**BACKUP YOUR KEYSTORE**: If you lose this file, you cannot update the app on Google Play Store!

### Signing Configuration

The release signing is configured in `src/PuzzleDazzle/PuzzleDazzle.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release' and '$(TargetFramework)' == 'net9.0-android'">
    <AndroidKeyStore>true</AndroidKeyStore>
    <AndroidSigningKeyStore>..\..\puzzledazzle.keystore</AndroidSigningKeyStore>
    <AndroidSigningKeyAlias>puzzledazzle</AndroidSigningKeyAlias>
    <AndroidSigningKeyPass>***</AndroidSigningKeyPass>
    <AndroidSigningStorePass>***</AndroidSigningStorePass>
    <AndroidLinkMode>SdkOnly</AndroidLinkMode>
    <RunAOTCompilation>false</RunAOTCompilation>
    <AndroidPackageFormats>apk;aab</AndroidPackageFormats>
</PropertyGroup>
```

## Distribution Options

### Option 1: Direct APK Distribution (Testing/Friends)
Use the `com.companyname.puzzledazzle-Signed.apk` file:
1. Copy APK to device or share via email/cloud
2. Enable "Install from unknown sources" on device
3. Tap APK file to install

**Best for**: Early testing, sharing with friends/family

### Option 2: Google Play Store (Production)
Use the `com.companyname.puzzledazzle-Signed.aab` file:
1. Create Google Play Developer account ($25 one-time fee)
2. Create app listing in Play Console
3. Upload AAB to desired track (Internal/Closed/Open/Production)
4. Submit for review

**Best for**: Public release, automatic updates, monetization

## Troubleshooting

### Build Errors - Permission Denied
If you see "Permission denied" or "Renaming temporary file failed":
1. Clean the build output: `dotnet clean src\PuzzleDazzle\PuzzleDazzle.csproj -c Release`
2. Close any Android emulators or file explorers viewing the output directory
3. Try building again

### Missing Keystore
If keystore is missing, you'll need to regenerate it (this will be a NEW key, incompatible with previous builds):

```powershell
keytool -genkey -v -keystore puzzledazzle.keystore -alias puzzledazzle -keyalg RSA -keysize 2048 -validity 10000 -storepass YOUR_PASSWORD -keypass YOUR_PASSWORD -dname "CN=Aliaksei Kazlou, OU=Personal, O=Personal, L=Wysoka, ST=Dolnoslaskie, C=PL"
```

**WARNING**: A new keystore means you cannot update existing installations!

## Version Updates

Before building a new release, update version numbers in `PuzzleDazzle.csproj`:

```xml
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>  <!-- User-visible version -->
<ApplicationVersion>1</ApplicationVersion>                  <!-- Version code (integer) -->
```

For updates:
- Increment `ApplicationDisplayVersion` (e.g., 1.0 → 1.1 → 2.0)
- Increment `ApplicationVersion` (e.g., 1 → 2 → 3)

Google Play requires the version code to increase with each upload.

## Known Warnings

The build may show warnings about SkiaSharp and Android 16 page sizes:
```
warning XA0141: Android 16 will require 16 KB page sizes, shared library 'libSkiaSharp.so' does not have a 16 KB page size.
```

This is a **warning, not an error**. The APK will work fine on current Android versions (API 28-34). Future SkiaSharp updates will address Android 16 compatibility.

## Next Steps After Build

1. **Test on physical device** - Install the signed APK and verify all features work
2. **Test on multiple devices** - Different screen sizes, Android versions
3. **Share with beta testers** - Get feedback before public release
4. **Upload to Play Console** - Submit for review when ready

See `docs/release-flow.md` for complete release process details.
