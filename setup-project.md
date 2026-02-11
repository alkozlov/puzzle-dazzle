# Project Setup Instructions

## Initialize MAUI Project from Windows

Since MAUI templates are only available on Windows (where the workload is installed), follow these steps to create the project structure:

### Step 1: Open PowerShell as Administrator

Navigate to the project directory:
```powershell
cd \\wsl.localhost\Ubuntu-24.04\home\akazlou\projects\puzzle-dazzle
```

### Step 2: Create MAUI App Project

```powershell
# Create the main MAUI app in src/PuzzleDazzle
cd src\PuzzleDazzle
dotnet new maui -n PuzzleDazzle -f net9.0
```

### Step 3: Create Core Class Library

```powershell
# Navigate to Core project directory
cd ..\PuzzleDazzle.Core
dotnet new classlib -n PuzzleDazzle.Core -f net9.0
```

### Step 4: Create Test Project

```powershell
# Navigate to test project directory
cd ..\..\tests\PuzzleDazzle.Core.Tests
dotnet new xunit -n PuzzleDazzle.Core.Tests -f net9.0
```

### Step 5: Create Solution File

```powershell
# Navigate back to root
cd ..\..

# Create solution file
dotnet new sln -n PuzzleDazzle

# Add projects to solution
dotnet sln add src\PuzzleDazzle\PuzzleDazzle.csproj
dotnet sln add src\PuzzleDazzle.Core\PuzzleDazzle.Core.csproj
dotnet sln add tests\PuzzleDazzle.Core.Tests\PuzzleDazzle.Core.Tests.csproj
```

### Step 6: Add Project References

```powershell
# Main app references Core library
dotnet add src\PuzzleDazzle\PuzzleDazzle.csproj reference src\PuzzleDazzle.Core\PuzzleDazzle.Core.csproj

# Test project references Core library
dotnet add tests\PuzzleDazzle.Core.Tests\PuzzleDazzle.Core.Tests.csproj reference src\PuzzleDazzle.Core\PuzzleDazzle.Core.csproj
```

### Step 7: Configure Android-Only Build

Edit `src/PuzzleDazzle/PuzzleDazzle.csproj` and ensure it has:
```xml
<TargetFrameworks>net9.0-android</TargetFrameworks>
<OutputType>Exe</OutputType>
<UseMaui>true</UseMaui>
<SingleProject>true</SingleProject>
```

Remove or comment out other target frameworks (iOS, MacCatalyst, Windows, Tizen).

### Step 8: Verify Setup

```powershell
# Build the solution
dotnet build

# Build Android specifically
dotnet build -f net9.0-android
```

## Expected Result

After completion, you should have:
```
puzzle-dazzle/
├── PuzzleDazzle.sln
├── src/
│   ├── PuzzleDazzle/              # MAUI app
│   │   └── PuzzleDazzle.csproj
│   └── PuzzleDazzle.Core/         # Core library
│       └── PuzzleDazzle.Core.csproj
└── tests/
    └── PuzzleDazzle.Core.Tests/   # Unit tests
        └── PuzzleDazzle.Core.Tests.csproj
```

All projects should build successfully without errors.
