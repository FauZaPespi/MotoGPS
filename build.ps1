#!/usr/bin/pwsh
<#
.MOTOGPS BUILD SCRIPT
=====================

PowerShell script to build MotoGPS for all platforms.

USAGE:
  .\build.ps1 [-Target <platform>] [-Configuration <config>]

EXAMPLES:
  .\build.ps1                    # Build all platforms (Debug)
  .\build.ps1 -Target windows    # Build only Windows
  .\build.ps1 -Configuration Release  # Build all in Release mode
  .\build.ps1 -Target ios -Configuration Release  # Build iOS Release
#>

param(
    [string]$Target = "all",
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"
$projectPath = "MotoGPS\MotoGPS.csproj"

function Write-Header {
    param([string]$message)
    Write-Host "`n==========================================" -ForegroundColor Cyan
    Write-Host "  $message" -ForegroundColor White
    Write-Host "==========================================" -ForegroundColor Cyan
}

function Invoke-Build {
    param(
        [string]$framework,
        [string]$name
    )
    Write-Header "Building $name ($framework)"
    
    $args = @(
        "publish",
        $projectPath,
        "-f", $framework,
        "-c", $Configuration
    )
    
    if ($framework -like "*android*" -or $framework -like "*ios*" -or $framework -like "*maccatalyst*") {
        $args += "/p:ArchiveOnBuild=true"
    }
    
    try {
        dotnet @args
        Write-Host "✓ $name build succeeded" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "✗ $name build failed" -ForegroundColor Red
        Write-Host "  $_" -ForegroundColor Red
        return $false
    }
}

# Check .NET installation
Write-Header "Checking .NET Installation"
try {
    $dotnetVersion = dotnet --version
    Write-Host "  .NET Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "  .NET SDK not found!" -ForegroundColor Red
    Write-Host "  Please install .NET 9 SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}

# Install MAUI workload if missing
Write-Header "Checking MAUI Workload"
try {
    dotnet workload list | Select-String "maui"
} catch {
    Write-Host "  MAUI workload not found. Installing..." -ForegroundColor Yellow
    dotnet workload install maui
}

# Build platforms
$allSucceeded = $true
$platforms = @{
    "windows" = @{ framework = "net9.0-windows10.0.19041.0"; name = "Windows" }
    "android" = @{ framework = "net9.0-android"; name = "Android" }
    "ios" = @{ framework = "net9.0-ios"; name = "iOS" }
    "maccatalyst" = @{ framework = "net9.0-maccatalyst"; name = "Mac Catalyst" }
}

if ($Target -eq "all") {
    Write-Header "Building All Platforms ($Configuration)"
    foreach ($key in $platforms.Keys) {
        if (-not (Invoke-Build -framework $platforms[$key].framework -name $platforms[$key].name)) {
            $allSucceeded = $false
        }
    }
} else {
    if ($platforms.ContainsKey($Target)) {
        Invoke-Build -framework $platforms[$Target].framework -name $platforms[$Target].name
    } else {
        Write-Host "Unknown target: $Target" -ForegroundColor Red
        Write-Host "Available targets: $($platforms.Keys -join ', ')" -ForegroundColor Yellow
        exit 1
    }
}

# Final result
Write-Header "Build Complete"
if ($allSucceeded) {
    Write-Host "All builds succeeded!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "Some builds failed!" -ForegroundColor Red
    exit 1
}
