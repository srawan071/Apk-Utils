using System.IO;
using UnityEngine;
using UnityEditor.Android;
public class PostBuildScript : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 0;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        // Path to the source file_paths.xml
        string sourceFilePath = Path.Combine(Application.dataPath,"Editor" ,"BuildPostProcessor", "file_paths.xml");

        // Destination folder in the Android project
        string destinationFolderPath = Path.Combine(path, "src", "main", "res", "xml");

        // Ensure the destination directory exists
        if (!Directory.Exists(destinationFolderPath))
        {
            Directory.CreateDirectory(destinationFolderPath);
        }

        // Copy the file_paths.xml to the destination
        string destinationFilePath = Path.Combine(destinationFolderPath, "file_paths.xml");
        File.Copy(sourceFilePath, destinationFilePath, true);

        Debug.Log("file_paths.xml successfully copied to Android project."+path);
    }
}