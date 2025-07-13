using UnityEditor;
using UnityEngine;

public class ImageUsageCheckerWindow : EditorWindow
{
    private Object imageObject;
    private int totalPrefabUsageCount;
    private int totalScriptableObjectUsageCount;
    private int totalSceneUsageCount;
    private string[] prefabUsageList;
    private string[] scriptableObjectUsageList;
    private string[] sceneUsageList;
    private Vector2 scrollPosition;

    [MenuItem("Tam's Window/ToolCheckResources/ImageUsageCheckerWindow")]
    static void CheckImageUsageInAll()
    {
        GetWindow<ImageUsageCheckerWindow>("ImageUsageCheckerWindow");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        // Hiển thị trường để chọn hình ảnh
        imageObject = EditorGUILayout.ObjectField("Image to Check", imageObject, typeof(Texture), false);

        // Nếu hình ảnh được chọn, kiểm tra sự sử dụng trong tất cả các loại
        if (GUILayout.Button("Check Usage In All") && imageObject != null)
        {
            string imagePath = AssetDatabase.GetAssetPath(imageObject);

            // Kiểm tra sự sử dụng trong Prefabs
            totalPrefabUsageCount = PrefabsSearch.CheckImageUsageInAllScenes(imagePath, out prefabUsageList);

            // Kiểm tra sự sử dụng trong ScriptableObjects
            totalScriptableObjectUsageCount = ScriptAbleObjectSearch.CheckImageUsageInAllScriptableObjects(imagePath, out scriptableObjectUsageList);

            // Kiểm tra sự sử dụng trong Scenes
            totalSceneUsageCount = SceneSearch.CheckImageUsageInAllScenes(imagePath, out sceneUsageList);
        }

        // Sử dụng EditorGUILayout.ScrollViewScope để tạo thanh cuộn cho cả khu vực nội dung
        using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            // Gán scrollPosition để lưu trạng thái cuộn
            scrollPosition = scrollViewScope.scrollPosition;

            // Hiển thị kết quả trên cửa sổ
            DisplayUsageList("Count use in Prefabs: ", prefabUsageList, totalPrefabUsageCount);
            DisplayUsageList("Count use in scriptsAbleObject: ", scriptableObjectUsageList, totalScriptableObjectUsageCount);
            DisplayUsageList("Count use in Scenes: ", sceneUsageList, totalSceneUsageCount);
        }

        EditorGUILayout.EndVertical();
    }

    private void DisplayUsageList(string label, string[] usageList, int countValue)
    {
        EditorGUILayout.LabelField(label + (usageList != null ? countValue.ToString() : "0"));
        if (usageList != null)
        {
            foreach (string path in usageList)
            {
                if (path != null && path.Length > 0)
                {
                    // Hiển thị tên đối tượng và tạo liên kết để điều hướng đến nó trên cửa sổ Project
                    if (GUILayout.Button($"- {path}"))
                    {
                        Object assetObject = AssetDatabase.LoadAssetAtPath<Object>(path);
                        EditorGUIUtility.PingObject(assetObject);
                    }

                    // Xuống hàng sau mỗi nút
                    EditorGUILayout.Space();
                }

            }
        }
    }
}
