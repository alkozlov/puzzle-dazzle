# Agent Instructions

## Project Overview

**Puzzle Dazzle** is a mobile maze generation app for Android (iOS planned for future).

### Key Features
- Generate mazes with configurable size and difficulty
- Multiple visualization styles (Classic, Soft)
- Save mazes as PNG images
- Share via social networks/messengers

### Tech Stack
- **.NET 9** with **C#**
- **MAUI** (Multi-platform App UI) - Android only for POC
- Target: Android API 28+ (Android 9.0+)

### Documentation
- `docs/concept.en.md` - Full project concept and business model
- `docs/screens.en.md` - Screen descriptions and UI requirements

## Development Environment

**Setup: WSL (source code) + Windows (build & run)**

Source code lives in WSL, but build/run operations are executed from Windows PowerShell.

### Prerequisites
1. **WSL**: Git for version control, text editor/IDE
2. **Windows**: 
   - .NET 9 SDK with MAUI workload (`dotnet workload install maui`)
   - Android Studio with SDK and emulator
   - PowerShell or Command Prompt

### Workflow

1. **Edit code**: Work in WSL at `/home/<USERNAME>/projects/puzzle-dazzle`
2. **Build/Run**: From Windows PowerShell, access WSL filesystem:
   ```powershell
   # Navigate to project (replace <USERNAME> with your WSL username)
   cd \\wsl$\Ubuntu\home\<USERNAME>\projects\puzzle-dazzle
   
   # Or use the shorter wsl.localhost path
   cd \\wsl.localhost\Ubuntu\home\<USERNAME>\projects\puzzle-dazzle
   ```

### Build Commands (Windows PowerShell)

```powershell
# Build for Android
dotnet build -f net9.0-android

# Run on connected device/emulator
dotnet build -f net9.0-android -t:Run

# Build release APK
dotnet publish -f net9.0-android -c Release
```

### Running the Emulator

1. Start Android emulator from Android Studio (Windows)
2. Verify emulator is running: `adb devices`
3. Run the app from PowerShell: `dotnet build -f net9.0-android -t:Run`

### Notes

- No ADB bridging required (everything runs on Windows)
- Android SDK on Windows is automatically detected by .NET MAUI
- Visual Studio on Windows can directly edit files on WSL filesystem via `\\wsl$\` path

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

