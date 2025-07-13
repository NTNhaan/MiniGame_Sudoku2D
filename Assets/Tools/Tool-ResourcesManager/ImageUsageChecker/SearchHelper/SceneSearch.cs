using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneSearch
{
    public static int CheckImageUsageInAllScenes(string imagePath, out string[] sceneUsageList)
    {
        // Lấy GUID của hình ảnh cần tìm
        string imageGuid = AssetDatabase.AssetPathToGUID(imagePath);
        if (string.IsNullOrEmpty(imageGuid))
        {
            Debug.LogWarning($"Invalid image path: {imagePath}");
            sceneUsageList = new string[0];
            return 0;
        }

        // Tìm tất cả các scene trong project
        string[] scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                                  .Select(AssetDatabase.GUIDToAssetPath)
                                  .ToArray();

        int totalUsageCount = 0;
        List<string> usageList = new List<string>();

        // Kiểm tra mỗi scene
        foreach (string scenePath in scenePaths)
        {
            if (CheckSceneForImageUsage(scenePath, imageGuid, false))
            {
                usageList.Add(scenePath);
                totalUsageCount++;
            }
        }

        sceneUsageList = usageList.ToArray();
        return totalUsageCount;
    }
    struct SceneCheckJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<FixedString512Bytes> scenePaths;
        [ReadOnly] public string imageGuid;
        public NativeArray<bool> results;

        public void Execute(int index)
        {
            string scenePath = scenePaths[index].ToString();
            if (CheckSceneForImageUsage(scenePath, imageGuid, true))
            {
                results[index] = true;
            }
        }
    }
    public static bool CheckImageUsageOrNot(string imagePath)
    {
        string imageGuid = AssetDatabase.AssetPathToGUID(imagePath);
        if (string.IsNullOrEmpty(imageGuid))
        {
            Debug.LogWarning($"Invalid image path: {imagePath}");
            return false;
        }

        string[] scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                                           .Select(AssetDatabase.GUIDToAssetPath)
                                           .ToArray();

        // Chuyển đổi sang NativeArray<FixedString512Bytes>
        NativeArray<FixedString512Bytes> nativeScenePaths = new NativeArray<FixedString512Bytes>(scenePaths.Length, Allocator.TempJob);
        for (int i = 0; i < scenePaths.Length; i++)
        {
            nativeScenePaths[i] = new FixedString512Bytes(scenePaths[i]);
        }

        NativeArray<bool> results = new NativeArray<bool>(scenePaths.Length, Allocator.TempJob);
        var job = new SceneCheckJob
        {
            imageGuid = imageGuid,
            scenePaths = nativeScenePaths,
            results = results
        };

        JobHandle handle = job.Schedule(scenePaths.Length, 64);
        handle.Complete();

        bool found = results.Contains(true);

        // Clean up
        nativeScenePaths.Dispose();
        results.Dispose();

        return found;
    }


    private static bool CheckSceneForImageUsage(string scenePath, string imageGuid, bool checkOnly)
    {
        // Mở scene nhưng không kích hoạt (tăng tốc độ)
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        bool found = false;
        GameObject[] allGameObjects = scene.GetRootGameObjects();

        foreach (GameObject go in allGameObjects)
        {
            Component[] components = go.GetComponentsInChildren<Component>(true);

            foreach (Component component in components)
            {
                if (component == null) continue;

                SerializedObject serializedObject = new SerializedObject(component);
                SerializedProperty serializedProperty = serializedObject.GetIterator();

                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (serializedProperty.objectReferenceValue != null &&
                            AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(serializedProperty.objectReferenceValue)) == imageGuid)
                        {
                            found = true;
                            if (checkOnly)
                                return true;
                        }
                    }
                }
            }
        }

        // Đóng scene sau khi quét xong
        EditorSceneManager.CloseScene(scene, true);
        return found;
    }
}
