using System.Collections.Generic;
using UnityEngine;
using Utils;

public class LevelData : Singleton<LevelData>
{
    public Dictionary<string, List<BoardData>> gameDir;
    void Start()
    {
        gameDir = new Dictionary<string, List<BoardData>>();

        FixedSudokuLevelData.ValidateAllLevels();
        var sudokuLevels = FixedSudokuLevelData.GetAllLevels();

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
                    break;
                case "Medium":
                    mediumLevels.Add(boardData);
                    break;
                case "Hard":
                case "Very Hard":
                    hardLevels.Add(boardData);
                    break;
                case "Extreme":
                    impossibleLevels.Add(boardData);
                    break;
            }
        }
        gameDir.Add("Easy", easyLevels);
        gameDir.Add("Medium", mediumLevels);
        gameDir.Add("Hard", hardLevels);
        gameDir.Add("Impossible", impossibleLevels);
    }
}
