using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Example : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string _downloadUrl= "";
    private string _downloadFolderPath;

    private string _apkpath;
    private string packageName;
    [SerializeField]
    private TextMeshProUGUI _debug;

    private void Start()
    {

        _downloadFolderPath = Path.Combine(Application.persistentDataPath,"DownloadedGame");
        _apkpath = Path.Combine(_downloadFolderPath, Path.GetFileName(_downloadUrl));

    }

    public void DownloadBtn()
    {
        // Recreate the download folder.
        if (Directory.Exists(_downloadFolderPath))
        {
            Directory.Delete(_downloadFolderPath,true);
        }
        Directory.CreateDirectory(_downloadFolderPath);


        _apkpath = Path.Combine(_downloadFolderPath, Path.GetFileName(_downloadUrl));
        StartCoroutine(DownloadFileCoroutine(_downloadUrl, _apkpath, (onsucess) =>
        {
            _debug.text = onsucess.ToString();
        },
        (onerror) =>
        {
            _debug.text = onerror.ToString();
        }));
    }
    public void CheckPermision()
    {
        _debug.text = ApkUtils.HasInstallPermission() ? "it has permission to install apps from unknown sources" : "It doesnot have permission to install app from unknown source";
    }
    public void RequestPermission()
    {
        _debug.text = "Requesting permission...";
        ApkUtils.RequestInstallPermission();
    }

    public void InstallBtn()
    {
        if (!File.Exists(_apkpath))
        {
            _debug.text = "Apk not found in path: " + _apkpath;
        }

        ApkUtils.InstallApk(_apkpath, (result) =>
        {
            if (result == true)
            {
                _debug.text = "App sucessfully installed";
            }
            else
            {
                _debug.text = "App not Installed";
            }
        });
    }
    public void UninstallBtn()
    {
        if (string.IsNullOrEmpty(packageName))
        {
            _debug.text = "Package name is empty. Please again download apk or click on get Pkg from path btn";
            return;
        }
       
        ApkUtils.UninstallApp(packageName, (result) =>
        {
            if (result == true)
            {
                _debug.text = "App Uninstalled sucessfully";
            }
            else
            {
                _debug.text = "App is not Uninstalled";
            }
        });
    }
    public void LaunchBtn()
    {
        if (string.IsNullOrEmpty(packageName))
        {
            _debug.text = "Package name is empty. Please again download apk or click on Print PkgName from APK btn";
            return;
        }
        if (!ApkUtils.IsAppInstalled(packageName))
        {
            _debug.text = packageName+" the app with this package name is now installed";
            return;
        }
        _debug.text = " launching app...";
        ApkUtils.LaunchApp(packageName);
    }
    public void GetPackageNameFormApkBtn()
    {

        if (!File.Exists(_apkpath))
        {
            _debug.text = "Apk not found in path: " + _apkpath;
            return;
        }

        packageName = ApkUtils.GetPackageNameFromApk(_apkpath);
        _debug.text = "Package name is " + packageName;
    }
    public void CheckInstalledBtn()
    {
        _debug.text = ApkUtils.IsAppInstalled(packageName) ? "App is installed on this device" : "App is not installed on this device";
    }
    public void CurrentVersion()
    {
        if (string.IsNullOrEmpty(packageName))
        {
            _debug.text = "Package name is empty. Please again download apk or click on Print PkgName from APK btn";
            return;
        }
        if (!ApkUtils.IsAppInstalled(packageName))
        {
            _debug.text = packageName + " the app with this package name is now installed";
            return;
        }
        _debug.text = $" Version is {ApkUtils.GetInstalledAppVersion(packageName)}";
    }
    private IEnumerator DownloadFileCoroutine(string url, string destinationPath, Action<string> onSuccess, Action<string> onError)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            try
            {
                var dh = new DownloadHandlerFile(destinationPath);
                dh.removeFileOnAbort = true;
                webRequest.downloadHandler = dh;

                webRequest.SendWebRequest();
            }
            catch (SystemException error)
            {
                onError?.Invoke(error.Message);
            }
            while (!webRequest.isDone)
            {
                float percent = webRequest.downloadProgress*100;
                _debug.text = "Downloading..."+ percent.ToString()+"%";
                yield return null;
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke("File downloaded successfully");
            }
            else
            {
                onError?.Invoke(webRequest.error);
                Debug.Log("Error Occured");
            }
        }


    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
