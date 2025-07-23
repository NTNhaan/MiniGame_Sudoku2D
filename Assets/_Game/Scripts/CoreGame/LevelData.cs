using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LevelData : Singleton<LevelData>
{
    public Dictionary<string, List<BoardData>> gameDir;
    void Start()
    {
        gameDir = new Dictionary<string, List<BoardData>>();

        Debug.Log("[LevelData] Initializing with FixedSudokuLevelData system");

        // Validate all levels first
        FixedSudokuLevelData.ValidateAllLevels();

        // Get all levels from fixed data system
        var sudokuLevels = FixedSudokuLevelData.GetAllLevels();

        // Distribute levels by difficulty
        var easyLevels = new List<BoardData>();
        var mediumLevels = new List<BoardData>();
        var hardLevels = new List<BoardData>();
        var impossibleLevels = new List<BoardData>();

        foreach (var level in sudokuLevels)
        {
            var boardData = level.ToBoardData();

            switch (level.difficulty)
            {
                case "Very Easy":
                case "Easy":
                    easyLevels.Add(boardData);
                    Debug.Log($"Added Level {level.levelId} to Easy category");
                    break;
                case "Medium":
                    mediumLevels.Add(boardData);
                    Debug.Log($"Added Level {level.levelId} to Medium category");
                    break;
                case "Hard":
                case "Very Hard":
                    hardLevels.Add(boardData);
                    Debug.Log($"Added Level {level.levelId} to Hard category");
                    break;
                case "Extreme":
                    impossibleLevels.Add(boardData);
                    Debug.Log($"Added Level {level.levelId} to Impossible category");
                    break;
            }
        }

        // Add to dictionary
        gameDir.Add("Easy", easyLevels);
        gameDir.Add("Medium", mediumLevels);
        gameDir.Add("Hard", hardLevels);
        gameDir.Add("Impossible", impossibleLevels);

        Debug.Log($"LevelData initialized with: Easy={easyLevels.Count}, Medium={mediumLevels.Count}, Hard={hardLevels.Count}, Impossible={impossibleLevels.Count}");
    }
}
