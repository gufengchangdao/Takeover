using UnityEditor;
using UnityEngine;

public static class OpenFileMenu
{
    [MenuItem("GameFramework/打开PersistentDataPath目录")]
    public static void OpenPersistentDataPath()
    {
        string logDirectory = Application.persistentDataPath;
        if (!System.IO.Directory.Exists(logDirectory))
            return;

        System.Diagnostics.Process.Start(logDirectory);
    }
}