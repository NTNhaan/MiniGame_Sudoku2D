using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System.Linq;

public class ProjectViewWindow : EditorWindow
{
    private UnityFolder Assets;
    private Vector2 Scroll;
    [MenuItem("Tam's Window/ToolCheckResources/ImageExportHelper")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ProjectViewWindow window = (ProjectViewWindow)EditorWindow.GetWindow(typeof(ProjectViewWindow));
        window.Show();
    }


    void OnGUI()
    {
        Scroll = GUILayout.BeginScrollView(Scroll);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Search", EditorStyles.toolbarButton))
            Assets = new UnityFolder("Assets", null, 0, position, false);
        if (GUILayout.Button("Search(Usage)", EditorStyles.toolbarButton))
            Assets = new UnityFolder("Assets", null, 0, position, true);
        if (GUILayout.Button("Export to assets/images.zip", EditorStyles.toolbarButton))
        {
            CompressImages();
        }
        GUILayout.EndHorizontal();

        // Remove the inner scroll view
        if (Assets != null)
            Assets.VisualizeFolder();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }


    public void CompressImages()
    {
        string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".gif" };
        string outputPath = "Assets/Images.zip";

        // Lấy tất cả đường dẫn của hình ảnh trong thư mục Assets và các thư mục con
        string[] imagePaths = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories)
            .Where(s => imageExtensions.Contains(Path.GetExtension(s).ToLower()))
            .ToArray();

        // Nén các hình ảnh vào tệp RAR
        CompressToZip(outputPath, imagePaths);
    }

    static void CompressToZip(string outputPath, string[] filesToCompress)
    {
        using (FileStream fsOut = File.Create(outputPath))
        using (ZipArchive zipArchive = new ZipArchive(fsOut, ZipArchiveMode.Create))
        {
            foreach (string filePath in filesToCompress)
            {
                string relativePath = filePath.Substring("Assets".Length + 1); // Lấy đường dẫn tương đối trong Assets

                ZipArchiveEntry entry = zipArchive.CreateEntry(relativePath, System.IO.Compression.CompressionLevel.Optimal);

                using (Stream entryStream = entry.Open())
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    fileStream.CopyTo(entryStream);
                }
            }
        }
    }
}