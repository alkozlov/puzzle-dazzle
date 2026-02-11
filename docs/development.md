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
