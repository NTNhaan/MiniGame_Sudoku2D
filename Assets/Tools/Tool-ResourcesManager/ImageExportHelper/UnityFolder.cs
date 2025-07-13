using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityFolder
{
    private string folderPath;
    private string folderName;
    private UnityFolder parentFolder;

    private List<UnityFile> child_files;
    private UnityFile[,] groupedFiles;
    private List<UnityFolder> child_folders;

    private GUIContent folderContent;

    private Texture2D folderIcon;

    private bool fold = false;
    private int depth;

    private Rect position;

    private int totalImages;  // Số lượng hình ảnh trong thư mục và thư mục con
    private int totalImagesUsage;  // Số lượng hình ảnh trong thư mục và thư mục con
    public delegate void FolderSelectedEventHandler(UnityFolder selectedFolder);
    public UnityFolder(string folderPath, UnityFolder parentFolder, int depth, Rect position, bool isUsage)
    {
        this.folderPath = folderPath;
        this.depth = depth;
        this.position = position;

        child_folders = FindChildFolders(isUsage);
        child_files = FindChildFiles();

        // Create the object by giving its path. Then get the asset preview.
        Object folderobj = AssetDatabase.LoadAssetAtPath(this.folderPath, typeof(Object));
        folderIcon = AssetPreview.GetMiniThumbnail(folderobj);

        // Assets/New Folder -> folderName:New Folder
        string[] splitPath = this.folderPath.Split('\\');
        folderName = splitPath[splitPath.Length - 1];

        // Tổng số hình ảnh trong thư mục và thư mục con
        totalImages = CalculateTotalImages();
        if (isUsage)
        {
            totalImagesUsage = CalculateTotalImagesUsage();
            folderContent = new GUIContent($"{folderName} ({totalImages}) || ({totalImagesUsage})", folderIcon, folderPath);
        }
        else
        {
            folderContent = new GUIContent($"{folderName} ({totalImages})", folderIcon, folderPath);
        }
        // Update folderContent with the correct totalImages

        // This is a 2D array to group files by rows of 3.
        groupedFiles = GroupChildFiles(child_files);


    }


    private int CalculateTotalImages()
    {
        int total = 0;

        foreach (var file in child_files)
        {
            if (IsImageFile(file.GetPath()))
            {
                total++;
            }
        }

        foreach (var folder in child_folders)
        {
            total += folder.CalculateTotalImages();
        }

        return total;
    }
    private int CalculateTotalImagesUsage()
    {
        int total = 0;

        foreach (var file in child_files)
        {
            if (IsImageFileUsage(file.GetPath()))
            {
                total++;
            }
        }

        foreach (var folder in child_folders)
        {
            total += folder.CalculateTotalImagesUsage();
        }

        return total;
    }

    private bool IsImageFile(string path)
    {
        string[] imageExtensions = { "png", "jpg", "jpeg", "bmp", "gif", "tiff" };
        string ext = FindExtension(path).ToLower();
        return System.Array.Exists(imageExtensions, element => element == ext);
    }
    private bool IsImageFileUsage(string path)
    {
        string[] imageExtensions = { "png", "jpg", "jpeg", "bmp", "gif", "tiff" };
        string ext = FindExtension(path).ToLower();
        bool isImage = System.Array.Exists(imageExtensions, element => element == ext);
        if (!isImage) return false;


        bool isImageUsage = ImageUseOrNot.CheckUseOrNot(path);
        return isImageUsage;
    }
    private string FindExtension(string path)
    {
        string[] splitPath = path.Split('.');
        string ext = splitPath[splitPath.Length - 1];
        return ext;
    }

    public void VisualizeFolder()
    {
        if (totalImages > 0)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15 * depth);

            Rect foldoutRect = GUILayoutUtility.GetRect(folderContent, EditorStyles.foldoutHeader);

            // Draw the foldout
            fold = EditorGUI.Foldout(foldoutRect, fold, folderContent, true);

            // Detect right-click
            Event e = Event.current;
            if (e.type == EventType.ContextClick && foldoutRect.Contains(e.mousePosition))
            {
                ShowContextMenu();
                e.Use();
            }

            GUILayout.EndHorizontal();

            if (fold)
            {
                VisualizeChildFiles();
                foreach (var VARIABLE in child_folders)
                    VARIABLE.VisualizeFolder();
            }

            GUILayout.EndVertical();
        }
    }

    private List<UnityFolder> FindChildFolders(bool isUsage)
    {
        string[] dirs = Directory.GetDirectories(folderPath);
        List<UnityFolder> folders = new List<UnityFolder>();
        foreach (var directory in dirs)
        {
            UnityFolder newfolder = new UnityFolder(directory, this, depth + 1, position, isUsage);
            folders.Add(newfolder);
        }
        return folders;
    }
    public List<UnityFile> GetChildFiles()
    {
        List<UnityFile> files = new List<UnityFile>();
        foreach (var file in child_files)
        {
            files.Add(file);
        }

        foreach (var folder in child_folders)
        {
            files.AddRange(folder.GetChildFiles());
        }

        return files;
    }
    private List<UnityFile> FindChildFiles()
    {
        string[] fileNames = Directory.GetFiles(folderPath);
        List<UnityFile> files = new List<UnityFile>();
        foreach (var file in fileNames)
        {
            UnityFile newfile = new UnityFile(file, this);
            if (!newfile.GetExtension().Equals("meta"))
            {
                files.Add(newfile);
            }
        }

        return files;
    }

    private UnityFile[,] GroupChildFiles(List<UnityFile> files)
    {
        int size = files.Count;
        int rows = (size / 3) + 1;
        UnityFile[,] groupedFiles = new UnityFile[rows, 3];
        int index = 0;
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < 3; j++)
                if (i * 3 + j <= size - 1)
                    groupedFiles[i, j] = files[index++];

        return groupedFiles;
    }

    private void VisualizeChildFiles()
    {
        int size = child_files.Count;
        int rows = (size / 3) + 1;

        int i = 0, j = 0;
        for (i = 0; i < rows; i++)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            for (j = 0; j < 3; j++)
            {
                if (i * 3 + j <= size - 1)
                    groupedFiles[i, j].VisualizeFile();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }

    public string GetName()
    {
        return folderPath;
    }

    public int GetDepth()
    {
        return depth;
    }
    private void ShowContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Show in Explorer"), false, () => EditorUtility.RevealInFinder(folderPath));
        menu.AddItem(new GUIContent("Show in Project"), false, () => Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(folderPath));
        menu.AddItem(new GUIContent("Export begin from this folder"), false, () => ExportFromThisFolder());
        menu.ShowAsContext();
    }

    private void ExportFromThisFolder()
    {
        // TODO: Implement your export logic here
        Debug.Log("Export from: " + folderPath);
    }
}
