# Agent Instructions

## Project Overview

**Mazele Dazzle** is a mobile maze generation app for Android (iOS planned for future).

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

**Setup: Windows (native development)**

Development is done natively on Windows due to MAUI build issues under WSL/Ubuntu.

### Prerequisites
- .NET 9 SDK with MAUI workload (`dotnet workload install maui`)
- Android Studio with SDK and emulator
- PowerShell or Command Prompt
- Git for Windows (for version control)

### Workflow

All operations (edit, build, run) are performed on Windows:
```powershell
# Navigate to project directory
cd C:\Users\AKazlou\Projects\puzzle-dazzle

# Or if working from WSL, access via Windows path
cd /mnt/c/Users/AKazlou/Projects/puzzle-dazzle
```

### Build Commands (Windows PowerShell)

```powershell
# Build the entire solution
dotnet build PuzzleDazzle.sln

# Build for Android
dotnet build src/PuzzleDazzle/PuzzleDazzle.csproj -f net9.0-android

# Run on connected device/emulator (fast rebuild + deploy)
dotnet build src/PuzzleDazzle/PuzzleDazzle.csproj -f net9.0-android -t:Run

# Build release APK
dotnet publish src/PuzzleDazzle/PuzzleDazzle.csproj -f net9.0-android -c Release
```

**Note:** XAML Hot Reload is unreliable on Android via CLI. Use fast rebuild (`-t:Run`) for development iteration.

### Running the Emulator

1. Start Android emulator from Android Studio (Windows)
2. Verify emulator is running: `adb devices`
3. Run the app from PowerShell: `dotnet build -f net9.0-android -t:Run`

### Notes

- All development happens natively on Windows (no WSL/Windows bridging needed)
- Android SDK is automatically detected by .NET MAUI
- Hot reload is unreliable; use fast rebuild workflow for changes
- Project can still be accessed from WSL via `/mnt/c/Users/AKazlou/Projects/puzzle-dazzle` if needed

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

