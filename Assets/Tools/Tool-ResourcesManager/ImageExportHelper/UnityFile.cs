using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityFile
{
    private string path;
    private string extension;
    private string fileName;
    private string fileOnlyNameAndExtention;
    private UnityFolder parentFolder;
    private Texture2D fileIcon;
    private GUIContent fileContent;

    public UnityFile(string path, UnityFolder parentFolder)
    {
        this.path = path;
        this.parentFolder = parentFolder;
        extension = FindExtension(path);
        fileName = FindFileName(path);
        fileOnlyNameAndExtention = GetFileNameAndExtension(path);
        if (IsImageFile(path))
        {
            Object fileobj = AssetDatabase.LoadAssetAtPath(this.path, typeof(Object));
            fileIcon = AssetPreview.GetMiniThumbnail(fileobj);
            fileContent = new GUIContent(fileOnlyNameAndExtention, fileIcon, path);
        }
    }

    private string FindExtension(string path)
    {
        string[] splitPath = path.Split('.');
        string ext = splitPath[splitPath.Length - 1];
        return ext;
    }

    private string FindFileName(string path)
    {
        string[] splitPath = path.Split('/');
        string fullName = splitPath[splitPath.Length - 1];
        string splitExt = fullName.Split('.')[0];

        return splitExt;
    }

    private bool IsImageFile(string path)
    {
        string[] imageExtensions = { "png", "jpg", "jpeg", "bmp", "gif", "tiff" };
        string ext = FindExtension(path).ToLower();
        return System.Array.Exists(imageExtensions, element => element == ext);
    }
    private string GetFileNameAndExtension(string path)
    {
        string[] splitPath = path.Split('\\');
        string fullName = splitPath[splitPath.Length - 1];
        return fullName;
    }
    public void VisualizeFile()
    {
        if (fileContent != null)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(256));
            GUILayout.Label(fileContent, GUILayout.Width(256), GUILayout.Height(32));
            GUILayout.EndHorizontal();
        }
    }

    public string GetPath()
    {
        return path;
    }

    public string GetExtension()
    {
        return extension;
    }
}
