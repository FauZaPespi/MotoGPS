# MotoGPS

A .NET MAUI application for GPS tracking on mobile devices.

## Features

- Cross-platform: Windows, Android, iOS, macOS
- GPS location tracking
- Built with .NET 9 and MAUI

## Prerequisites

- .NET 9 SDK
- .NET MAUI workload

## Local Development

### Install Workloads

```bash
# Install MAUI workload for all platforms
dotnet workload restore
```

### Build for Windows

```bash
dotnet build MotoGPS/MotoGPS.csproj -f net9.0-windows10.0.19041.0
```

### Build for Android

```bash
dotnet build MotoGPS/MotoGPS.csproj -f net9.0-android
```

### Build for iOS (requires macOS)

```bash
dotnet build MotoGPS/MotoGPS.csproj -f net9.0-ios
```

## GitHub Actions

This project includes pre-configured GitHub Actions workflows:

### Build All Platforms

Automatically builds Windows, Android, and iOS on every push to `main` or `develop` branches.

### iOS Release Build

Manual workflow to build signed iOS apps for deployment.

**Requires the following GitHub Secrets:**

| Secret Name | Description |
|-------------|-------------|
| `APPLE_CERTIFICATE` | Base64-encoded .p12 certificate |
| `APPLE_CERTIFICATE_PASSWORD` | Certificate password |
| `APPLE_CERTIFICATE_NAME` | Certificate name (e.g., "iPhone Developer") |
| `APPLE_PROVISIONING_PROFILE` | Base64-encoded .mobileprovision file |

#### How to encode your certificate and provisioning profile:

```bash
# On macOS
base64 -i certificate.p12 -o certificate.base64
base64 -i profile.mobileprovision -o profile.base64
```

Then copy the contents of the `.base64` files into the GitHub secrets.

## iPhone Deployment

### Option 1: TestFlight (Recommended)

1. Build signed iOS app using GitHub Actions
2. Upload to App Store Connect
3. Distribute via TestFlight

### Option 2: AltStore (Free, 7-day limit)

1. Download the .ipa from GitHub Actions artifacts
2. Install AltStore on your iPhone
3. Sideload the .ipa using AltStore

### Option 3: Sideloadly

1. Download the .ipa from GitHub Actions
2. Connect iPhone to computer
3. Use Sideloadly to install

## Project Structure

```
MotoGPS/
├── .github/
│   └── workflows/
│       ├── build-all.yml      # Auto-build on push
│       └── ios-release.yml    # Manual iOS build
├── MotoGPS/
│   ├── Platforms/
│   │   ├── Android/
│   │   ├── iOS/
│   │   ├── MacCatalyst/
│   │   └── Windows/
│   ├── Resources/
│   │   ├── AppIcon/
│   │   ├── Fonts/
│   │   ├── Images/
│   │   ├── Raw/
│   │   └── Splash/
│   ├── App.xaml
│   ├── AppShell.xaml
│   ├── MainPage.xaml
│   └── MauiProgram.cs
├── .gitignore
├── README.md
└── MotoGPS.sln
```

## Troubleshooting

### Missing Workloads

If you get errors about missing workloads:

```bash
dotnet workload restore
```

### iOS Build Fails

- Ensure you're on macOS
- Install Xcode from the App Store
- Run `xcode-select --install`
- Accept Xcode license: `sudo xcodebuild -license accept`

### Android Build Fails

- Install Android SDK
- Set `ANDROID_HOME` environment variable
- Ensure Java JDK is installed

## License

MIT License
