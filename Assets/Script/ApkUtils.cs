using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using UnityEngine;
using UnityEngine.Android;
using System.Collections.Generic;

public static class ApkUtils
{
   
    // Get the package name from APK file
    public static string GetPackageNameFromApk(string apkPath)
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject packageManager = context.Call<AndroidJavaObject>("getPackageManager"))
        {
            // Call the Android PackageManager to get the package information
            AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageArchiveInfo", apkPath, 0);

            if (packageInfo != null)
            {
                // Get the package name
                string packageName = packageInfo.Get<string>("packageName");
                return packageName;
            }
            else
            {
                Debug.LogError("Failed to retrieve package info from APK.");
                return null;
            }
        }
#else
        Debug.LogError("This method is only supported on Android.");
        return null;
#endif
    }


    public static bool IsAppInstalled(string packageName)
    {
#if UNITY_ANDROID
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
            {
                // Check if the package is installed
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
                /* if (packageInfo != null)
                 {
                     Debug.Log($"{packageName} is installed");
                 }
                 else
                 {
                     Debug.Log($"{packageName} is not installed");
                 }*/
                return packageInfo != null; // If no exception, the package is installed
            }
        }
        catch (AndroidJavaException e)
        {
            if (e.Message.Contains("PackageManager$NameNotFoundException"))
            {
                // The package is not installed
                return false;
            }
            else
            {
                // An unexpected error occurred
                Debug.LogError("Error checking package installation: " + e.Message);
                return false;
            }
        }
#else
        Debug.LogError("App installation status check is only supported on Android.");
        return false;
#endif
    }

    // Get the version name of the installed app
    public static string GetInstalledAppVersion(string packageName)
    {
#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
        {
            try
            {
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
                return packageInfo.Get<string>("versionName");
            }
            catch (AndroidJavaException)
            {
                Debug.LogError($"Failed to get version name for package: {packageName}");
                return null;
            }
        }
#else
        Debug.LogError("Getting app version is only supported on Android.");
        return null;
#endif
    }
    public static void InstallApk(string apkPath, Action<bool> result)
    {
        string packageName = ApkUtils.GetPackageNameFromApk(apkPath);
      

        if (string.IsNullOrEmpty(packageName))
        {

            Debug.LogError("Failed to extract package name from APK." + apkPath);
            return;
        }


        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Ensure the app has permission to read external storage
            //  RequestExternalStoragePermission();


            // Use FileProvider to get URI for APK file
            AndroidJavaClass fileProvider = new AndroidJavaClass("androidx.core.content.FileProvider");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject file = new AndroidJavaObject("java.io.File", apkPath);

            string authority = context.Call<string>("getPackageName") + ".fileprovider";
            AndroidJavaObject uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", context, authority, file);

            // Intent to install APK
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW");
            intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/vnd.android.package-archive");
            intent.Call<AndroidJavaObject>("addFlags", 1);  // FLAG_GRANT_READ_URI_PERMISSION
            intent.Call<AndroidJavaObject>("addFlags", 268435456); // FLAG_ACTIVITY_NEW_TASK

           
            //  Debug.Log(" System box popup");

            currentActivity.Call("startActivity", intent);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error during APK installation: " + e.Message);
        }
    }
    public static void UninstallApp(string packageName, Action<bool> result)
    {

#if UNITY_ANDROID
        if (ApkUtils.IsAppInstalled(packageName))
        {
            //  Debug.Log($"Uninstalling {packageName}");
            try
            {

                // Obtain the UnityPlayer activity
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // Create the Intent to uninstall the package
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.DELETE");

                // Create Uri object for the package name
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "package:" + packageName);

                // Set the Uri data to the intent
                intent.Call<AndroidJavaObject>("setData", uri);

                // Set necessary flags for starting a new activity
                intent.Call<AndroidJavaObject>("addFlags", 268435456); // FLAG_ACTIVITY_NEW_TASK


                // Start the activity with the intent to uninstall
                currentActivity.Call("startActivity", intent);

                // Debug.Log("Uninstall intent launched for package: " + packageName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to uninstall APK: " + e.Message);
            }
        }
        else
        {
            Debug.Log(" Package name not found");
        }

#else
        Debug.LogError("Application uninstalling is only supported on Android.");
#endif
    }
    public static void LaunchApp(string packageName)
    {


#if UNITY_ANDROID
        if (ApkUtils.IsAppInstalled(packageName))
        {
            // Debug.Log("Inside the Launch game method: " + packageName + " is installed.");

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager"))
            {
                AndroidJavaObject launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);

                if (launchIntent != null)
                {
                    launchIntent.Call<AndroidJavaObject>("setFlags", 268435456); // Intent.FLAG_ACTIVITY_NEW_TASK
                   

                    currentActivity.Call("startActivity", launchIntent);

                    Debug.Log("App launched successfully.");
                }
                else
                {
                    Debug.LogError($"No launch intent found for package {packageName}. The app might not have a launcher activity.");
                }
            }
        }
        else
        {
            Debug.LogError($"App with package name {packageName} is not installed.");

        }
#else
        Debug.LogError("Game launching is only supported on Android.");
#endif
    }
   
   
    public static bool HasInstallPermission()
    {
#if UNITY_ANDROID
        if (!AndroidVersionIsAtLeast(26))
        {
            // If the Android version is below 26, permission is not needed
            return true;
        }

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
        bool hasInstallPermission = packageManager.Call<bool>("canRequestPackageInstalls");

        return hasInstallPermission;
#else
        return false;
#endif
    }

    public static void RequestInstallPermission()
    {
#if UNITY_ANDROID
       

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (AndroidVersionIsAtLeast(26))
        {
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            string packageName = currentActivity.Call<string>("getPackageName");

            bool hasInstallPermission = packageManager.Call<bool>("canRequestPackageInstalls");

            if (!hasInstallPermission)
            {
                // Create an Intent for the settings page
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent",
                    "android.settings.MANAGE_UNKNOWN_APP_SOURCES");

                // Create Uri object for the package
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "package:" + packageName);

                // Set the Uri data to the intent
                intent.Call<AndroidJavaObject>("setData", uri);

                // Start the activity
                currentActivity.Call("startActivity", intent);
            }
        }
#endif
    }
    /* public static void RequestExternalStoragePermission()
     {

         //Check and request External storage permission in Android.
         if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
         {
           //  DebugUI.Instance.SetUp("Request External Storage Permissions");
             //   GUI.Label(new Rect(10, 10, 200, 20), "Request External Storage Permissions");
             UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
         }
     }*/
    private static bool AndroidVersionIsAtLeast(int version)
    {
#if UNITY_ANDROID
        AndroidJavaClass versionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int sdkInt = versionClass.GetStatic<int>("SDK_INT");
        return sdkInt >= version;
#else
        return false;
#endif
    }

}
