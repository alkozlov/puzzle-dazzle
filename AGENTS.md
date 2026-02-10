# Agent Instructions

## Project Overview

**Puzzle Dazzle** is a mobile maze generation app for Android (iOS planned for future).

### Key Features
- Generate mazes with configurable size and difficulty
- Multiple visualization styles (Classic, Soft)
- Save mazes as PNG images
- Share via social networks/messengers

### Tech Stack
- **.NET 10** with **C#**
- **MAUI** (Multi-platform App UI) - Android only for POC
- Target: Android API 28+ (Android 9.0+)

### Documentation
- `docs/concept.en.md` - Full project concept and business model
- `docs/screens.en.md` - Screen descriptions and UI requirements

## Development Environment

**Setup: WSL + Windows Android Studio**

The project code lives in WSL, but Android emulator runs on Windows.

### Prerequisites
1. **WSL**: .NET 10 SDK with MAUI workloads
2. **Windows**: Android Studio with SDK and emulator

### Connecting WSL to Windows Android Emulator

1. Start Android emulator in Android Studio (Windows)
2. In WSL, connect to Windows ADB:
   ```bash
   # Get Windows host IP
   export WSL_HOST=$(cat /etc/resolv.conf | grep nameserver | awk '{print $2}')
   
   # Connect to Windows ADB server
   adb connect $WSL_HOST:5555
   
   # Or kill local adb and use Windows adb
   adb kill-server
   export ADB_SERVER_SOCKET=tcp:$WSL_HOST:5037
   ```

3. Verify connection:
   ```bash
   adb devices  # Should show emulator
   ```

### Environment Variables (WSL ~/.bashrc)
```bash
export ANDROID_HOME=/mnt/c/Users/<USERNAME>/AppData/Local/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/platform-tools
```

### Build Commands
```bash
# Build for Android
dotnet build -f net10.0-android

# Run on connected device/emulator
dotnet build -f net10.0-android -t:Run

# Build release APK
dotnet publish -f net10.0-android -c Release
```

### Project Structure (planned)
```
PuzzleDazzle/
├── PuzzleDazzle.sln
├── src/
│   ├── PuzzleDazzle/              # MAUI app project
│   │   ├── Views/                 # XAML pages
│   │   ├── ViewModels/            # MVVM view models
│   │   ├── Services/              # Business logic
│   │   └── Resources/             # Images, styles
│   └── PuzzleDazzle.Core/         # Core library (maze generation)
│       ├── Models/                # Maze, Cell, etc.
│       ├── Generation/            # Maze algorithms
│       └── Rendering/             # Visualization styles
└── tests/
    └── PuzzleDazzle.Core.Tests/   # Unit tests
```

## Issue Tracking

This project uses **bd** (beads) for issue tracking. Run `bd onboard` to get started.

## Quick Reference

```bash
bd ready              # Find available work
bd show <id>          # View issue details
bd update <id> --status in_progress  # Claim work
bd close <id>         # Complete work
bd sync               # Sync with git
```

## Landing the Plane (Session Completion)

**When ending a work session**, you MUST complete ALL steps below. Work is NOT complete until `git push` succeeds.

**MANDATORY WORKFLOW:**

1. **File issues for remaining work** - Create issues for anything that needs follow-up
2. **Run quality gates** (if code changed) - Tests, linters, builds
3. **Update issue status** - Close finished work, update in-progress items
4. **PUSH TO REMOTE** - This is MANDATORY:
   ```bash
   git pull --rebase
   bd sync
   git push
   git status  # MUST show "up to date with origin"
   ```
5. **Clean up** - Clear stashes, prune remote branches
6. **Verify** - All changes committed AND pushed
7. **Hand off** - Provide context for next session

**CRITICAL RULES:**
- Work is NOT complete until `git push` succeeds
- NEVER stop before pushing - that leaves work stranded locally
- NEVER say "ready to push when you are" - YOU must push
- If push fails, resolve and retry until it succeeds

