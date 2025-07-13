using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ScriptAbleObjectSearch
{
    public static int CheckImageUsageInAllScriptableObjects(string imagePath, out string[] scriptableObjectUsageList)
    {
        // Lấy tất cả các ScriptableObject trong thư mục Assets
        string[] scriptableObjectPaths = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" });

        int totalCount = 0;
        scriptableObjectUsageList = new string[scriptableObjectPaths.Length];

        int index = 0;
        foreach (string scriptableObjectPath in scriptableObjectPaths)
        {
            string fullPath = AssetDatabase.GUIDToAssetPath(scriptableObjectPath);
            ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(fullPath);

            if (scriptableObject != null)
            {
                int usageCount = CheckImageUsageInScriptableObjectRecursive(scriptableObject, imagePath, false);
                if (usageCount > 0)
                {
                    scriptableObjectUsageList[index++] = fullPath;
                }
                totalCount += usageCount;
            }
        }
        return totalCount;
    }
    public static bool CheckImageUsageOrNot(string imagePath)
    {
        // Lấy tất cả các ScriptableObject trong thư mục Assets
        string[] scriptableObjectPaths = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" });

        int totalCount = 0;

        foreach (string scriptableObjectPath in scriptableObjectPaths)
        {
            string fullPath = AssetDatabase.GUIDToAssetPath(scriptableObjectPath);
            ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(fullPath);

            if (scriptableObject != null)
            {
                int usageCount = CheckImageUsageInScriptableObjectRecursive(scriptableObject, imagePath, true);
                totalCount += usageCount;
            }
        }
        return totalCount > 0;
    }

    private static int CheckImageUsageInScriptableObjectRecursive(ScriptableObject scriptableObject, string imagePath, bool checkOnly)
    {
        // Kiểm tra sự sử dụng trong ScriptableObject hiện tại
        int totalCount = CheckImageUsageInScriptableObject(scriptableObject, imagePath, checkOnly);

        // Tìm các property kiểu ScriptableObject trong ScriptableObject
        SerializedObject serializedObject = new SerializedObject(scriptableObject);
        SerializedProperty property = serializedObject.GetIterator();

        while (property.Next(true))
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                ScriptableObject nestedScriptableObject = property.objectReferenceValue as ScriptableObject;

                // Gọi đệ quy cho từng ScriptableObject con
                if (nestedScriptableObject != null)
                {
                    totalCount += CheckImageUsageInScriptableObjectRecursive(nestedScriptableObject, imagePath, checkOnly);
                }
            }
        }
        return totalCount;
    }

    private static int CheckImageUsageInScriptableObject(ScriptableObject scriptableObject, string imagePath, bool checkOnly)
    {
        int count = 0;

        SerializedObject serializedObject = new SerializedObject(scriptableObject);
        SerializedProperty property = serializedObject.GetIterator();

        while (property.Next(true))
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null &&
                    AssetDatabase.GetAssetPath(property.objectReferenceValue) == imagePath)
                {
                    //Debug.Log($"Image is used in ScriptableObject: {scriptableObject.name}");
                    count++;
                    if (checkOnly)
                        return 1;
                    // return; // không cần return ngay, để kiểm tra tất cả các property
                }
            }
        }

        return count;
    }
}
