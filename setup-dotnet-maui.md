# Setup .NET SDK with MAUI Support

This guide helps you install .NET SDK with MAUI workloads from Microsoft's official repository.

## Problem

MAUI workloads are not available when .NET is installed from Ubuntu's default package repository.

## Solution

Follow these steps to install .NET SDK from Microsoft's repository:

### 1. Check current repository

```bash
apt-cache policy dotnet-sdk-9.0
```

### 2. Add Microsoft's package repository

```bash
# Download Microsoft package configuration
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

# Install the repository configuration
sudo dpkg -i packages-microsoft-prod.deb

# Clean up
rm packages-microsoft-prod.deb
```

### 3. Update and install .NET 9 SDK

```bash
# Update package list
sudo apt update

# Install .NET 9 SDK from Microsoft repository
sudo apt install dotnet-sdk-9.0
```

### 4. Install MAUI workload

```bash
# Install MAUI workload for cross-platform development
dotnet workload install maui

# Or install only Android workload (for POC)
dotnet workload install maui-android
```

### 5. Verify installation

```bash
# Check installed workloads
dotnet workload list

# Check .NET info
dotnet --info
```

## Expected Result

After successful installation, `dotnet workload list` should show:
- `maui-android` (or `maui` if you installed the full workload)

## Troubleshooting

If you still don't see MAUI workloads after installation:

1. Make sure you're using .NET 9 or .NET 8 (not .NET 10)
2. Try removing existing .NET installations and reinstalling from Microsoft's repository
3. Check that the workload manifests are properly installed in `/usr/lib/dotnet/sdk-manifests/`
