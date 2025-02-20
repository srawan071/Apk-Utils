# APK Utilities for Unity

This project provides a set of utilities for handling APK files on Android devices within a Unity project. The `ApkUtils` class offers various methods to interact with APK files, including installation, uninstallation, package name retrieval, version fetching, app launching, and permission handling.

## Features

- **Get Package Name from APK**: Retrieve the package name of an APK file.
- **Check if an App is Installed**: Verify if a specific app (by its package name) is installed on the device.
- **Get Installed App Version**: Retrieve the version name of an installed app.
- **Install APK**: Install an APK file from a given path on the Android device.
- **Uninstall App**: Uninstall an app from the Android device by its package name.
- **Launch App**: Launch an installed app by its package name.
- **Permission Handling**: Check if the app has the required permissions to install APKs, and request them if necessary.

## Requirements

- Unity 2020.3 or later.
- Android platform support enabled in Unity.

## Methods

### `GetPackageNameFromApk(string apkPath)`
- Retrieves the package name of an APK file located at the given `apkPath`.
- **Returns**: The package name of the APK or `null` if the package name cannot be retrieved.

### `IsAppInstalled(string packageName)`
- Checks whether an app with the specified `packageName` is installed on the device.
- **Returns**: `true` if the app is installed, `false` if not.

### `GetInstalledAppVersion(string packageName)`
- Retrieves the version name of an installed app by its `packageName`.
- **Returns**: The version name of the app or `null` if it cannot be fetched.

### `InstallApk(string apkPath, Action<bool> result)`
- Installs an APK file located at the specified `apkPath`.
- **Parameters**: 
  - `apkPath`: The file path of the APK to install.
  - `result`: A callback action to handle the result of the installation.
- **Returns**: No return value. The result is provided via the callback.

### `UninstallApp(string packageName, Action<bool> result)`
- Uninstalls the app with the given `packageName`.
- **Parameters**: 
  - `packageName`: The package name of the app to uninstall.
  - `result`: A callback action to handle the result of the uninstallation.
- **Returns**: No return value. The result is provided via the callback.

### `LaunchApp(string packageName)`
- Launches the app with the specified `packageName` if it is installed.
- **Parameters**: 
  - `packageName`: The package name of the app to launch.
- **Returns**: No return value.

### `HasInstallPermission()`
- Checks whether the app has permission to install APKs on the device.
- **Returns**: `true` if the app has permission, `false` otherwise.

### `RequestInstallPermission()`
- Requests the necessary permission for installing APKs if the app does not have it.
- **Returns**: No return value.

## Setup

1. **Import the APK Utilities**:
   - Import the `ApkUtils` class into your Unity project.
   
2. **Permissions**:
   - The APK utilities may require permissions to install APKs or access external storage.
   - The `HasInstallPermission()` and `RequestInstallPermission()` methods can be used to handle these permissions.

3. **Add Necessary Android Permissions**:
   - In your Unity project's `AndroidManifest.xml`, ensure the following permissions are added:
     ```xml
     <uses-permission android:name="android.permission.REQUEST_INSTALL_PACKAGES" />
     <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
     <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
     ```

## Example Usage

```csharp
// Check if an app is installed
bool isInstalled = ApkUtils.IsAppInstalled("com.example.myapp");
if (isInstalled)
{
    Debug.Log("App is installed!");
}
else
{
    Debug.Log("App is not installed.");
}

// Install an APK
ApkUtils.InstallApk("/path/to/your.apk", (result) => {
    if (result) {
        Debug.Log("APK installed successfully!");
    } else {
        Debug.LogError("APK installation failed.");
    }
});

// Uninstall an app
ApkUtils.UninstallApp("com.example.myapp", (result) => {
    if (result) {
        Debug.Log("App uninstalled successfully!");
    } else {
        Debug.LogError("App uninstallation failed.");
    }
});

// Launch an app
ApkUtils.LaunchApp("com.example.myapp");

// Check permission to install APK
if (!ApkUtils.HasInstallPermission()) {
    Debug.Log("App doesn't have permission to install APKs.");
    ApkUtils.RequestInstallPermission();
} else {
    Debug.Log("App has permission to install APKs.");
}
Notes
Android Permissions: This utility assumes that the necessary permissions to access external storage and install APKs have been requested and granted on Android devices.
Platform Support: This utility is designed to work specifically on Android and will not work on other platforms.
Error Handling: Methods like InstallApk and UninstallApp handle exceptions and errors internally and log appropriate messages to the Unity console.
API Level: The utility supports Android API level 21 and above for the installation and permission management functionality.
License
This project is licensed under the MIT License - see the LICENSE file for details.

vbnet
Copy
Edit

Now the entire README is properly formatted with markdown! Let me know if you'd like to make any other changes.






