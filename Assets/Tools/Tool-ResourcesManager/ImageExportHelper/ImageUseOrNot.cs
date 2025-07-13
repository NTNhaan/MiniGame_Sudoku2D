using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageUseOrNot
{
    public static bool CheckUseOrNot(string path)
    {
        path = path.Replace('\\', '/');

        bool prefabsUsed = PrefabsSearch.CheckImageUsageOrNot(path);
        if (prefabsUsed)
            return true;
        bool scriptsAbleObjectUsed = ScriptAbleObjectSearch.CheckImageUsageOrNot(path);
        if (scriptsAbleObjectUsed)
            return true;
        bool sceneUsed = SceneSearch.CheckImageUsageOrNot(path);
        if (sceneUsed)
            return true;
        return false;
    }
}
