#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;

public class BatchFbxExporter
{
    [MenuItem("Tools/Export Multiple FBX")]
    static void ExportSelectedObjects()
    {
        string exportPath = "Assets/ExportedFBX/";
        if (!System.IO.Directory.Exists(exportPath))
            System.IO.Directory.CreateDirectory(exportPath);

        foreach (GameObject obj in Selection.gameObjects)
        {
            string fileName = exportPath + obj.name + ".fbx";
            ModelExporter.ExportObject(fileName, obj);
            Debug.Log("Exported: " + fileName);
        }

        AssetDatabase.Refresh();
    }
}
#endif