using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LevelData : Singleton<LevelData>
{
    public Dictionary<string, List<BoardData>> gameDir;
    void Start()
    {
        gameDir = new Dictionary<string, List<BoardData>>();
        gameDir.Add("Easy", LevelEasyData.GetData());
        gameDir.Add("Medium", LevelMediumData.GetData());
        gameDir.Add("Hard", LevelHardData.GetData());
        gameDir.Add("Impossible", LevelImpossibleData.GetData());
    }
}
