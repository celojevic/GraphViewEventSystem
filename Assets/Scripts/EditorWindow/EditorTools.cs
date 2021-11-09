#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class EditorTools : EditorWindow
{

    [MenuItem("Tools/Open PersistentDataPath")]
    private static void OpenPersistentDataPath()
    {
		Process.Start(
            "explorer.exe", 
            "/root," + "\"" + Application.persistentDataPath.Replace("/", "\\") + "\""
        );
	}

}

#endif