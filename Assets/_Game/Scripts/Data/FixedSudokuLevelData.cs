using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FixedSudokuLevel
{
    public int levelId;
    public string difficulty;
    public int[] unsolvedData;
    public int[] solvedData;

    public FixedSudokuLevel(int id, string diff, int[] unsolved, int[] solved)
    {
        levelId = id;
        difficulty = diff;
        unsolvedData = unsolved;
        solvedData = solved;
    }

    public BoardData ToBoardData()
    {
        return new BoardData(unsolvedData, solvedData);
    }
}

public class FixedSudokuLevelData
{
    private static List<FixedSudokuLevel> allLevels;

    public static void InitializeLevels()
    {
        allLevels = new List<FixedSudokuLevel>();

        // Create completely new Sudoku data - no dependency on LevelEasyData
        CreateAllLevels();
    }

    private static void CreateAllLevels()
    {
        // Base solved Sudoku pattern
        int[] baseSolvedData = new int[81] {
            5, 3, 4, 6, 7, 8, 9, 1, 2,
            6, 7, 2, 1, 9, 5, 3, 4, 8,
            1, 9, 8, 3, 4, 2, 5, 6, 7,
            8, 5, 9, 7, 6, 1, 4, 2, 3,
            4, 2, 6, 8, 5, 3, 7, 9, 1,
            7, 1, 3, 9, 2, 4, 8, 5, 6,
            9, 6, 1, 5, 3, 7, 2, 8, 4,
            2, 8, 7, 4, 1, 9, 6, 3, 5,
            3, 4, 5, 2, 8, 6, 1, 7, 9
        };

        // Alternative solved pattern for variation
        int[] altSolvedData = new int[81] {
            1, 2, 3, 4, 5, 6, 7, 8, 9,
            4, 5, 6, 7, 8, 9, 1, 2, 3,
            7, 8, 9, 1, 2, 3, 4, 5, 6,
            2, 3, 1, 5, 6, 4, 8, 9, 7,
            5, 6, 4, 8, 9, 7, 2, 3, 1,
            8, 9, 7, 2, 3, 1, 5, 6, 4,
            3, 1, 2, 6, 4, 5, 9, 7, 8,
            6, 4, 5, 9, 7, 8, 3, 1, 2,
            9, 7, 8, 3, 1, 2, 6, 4, 5
        };

        // Create 10 levels with progressive difficulty
        string[] difficulties = { "Very Easy", "Very Easy", "Easy", "Easy", "Medium", "Medium", "Hard", "Hard", "Very Hard", "Extreme" };
        int[] cluesToRemove = { 35, 40, 45, 48, 52, 55, 58, 60, 62, 65 }; // Progressive difficulty

        for (int i = 0; i < 10; i++)
        {
            // Alternate between base patterns for variety
            int[] solvedData = new int[81];
            if (i % 2 == 0)
                System.Array.Copy(baseSolvedData, solvedData, 81);
            else
                System.Array.Copy(altSolvedData, solvedData, 81);

            // Create unsolved data by removing clues
            int[] unsolvedData = new int[81];
            System.Array.Copy(solvedData, unsolvedData, 81);

            // Remove clues based on difficulty
            RemoveRandomClues(unsolvedData, cluesToRemove[i]);

            allLevels.Add(new FixedSudokuLevel(
                i + 1,
                difficulties[i],
                unsolvedData,
                solvedData
            ));
        }
    }
    private static void RemoveRandomClues(int[] data, int totalCluesToRemove)
    {
        var random = new System.Random(); // Use different seed each time for variety
        var filledIndices = new List<int>();

        // Find all positions that have numbers
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != 0)
            {
                filledIndices.Add(i);
            }
        }

        // Calculate how many clues to remove (start with 81 filled, remove specified amount)
        int currentClues = filledIndices.Count;
        int targetClues = 81 - totalCluesToRemove; // Target number of clues to keep
        int cluesToRemove = currentClues - targetClues;

        // Ensure we don't remove more than available
        cluesToRemove = UnityEngine.Mathf.Max(0, UnityEngine.Mathf.Min(cluesToRemove, filledIndices.Count));

        // Remove random clues
        for (int i = 0; i < cluesToRemove && filledIndices.Count > 0; i++)
        {
            int randomIndex = random.Next(filledIndices.Count);
            int positionToRemove = filledIndices[randomIndex];
            data[positionToRemove] = 0;
            filledIndices.RemoveAt(randomIndex);
        }
    }

    public static List<FixedSudokuLevel> GetAllLevels()
    {
        if (allLevels == null || allLevels.Count == 0)
        {
            InitializeLevels();
        }
        return allLevels;
    }

    public static FixedSudokuLevel GetLevel(int levelId)
    {
        var levels = GetAllLevels();
        return levels.Find(level => level.levelId == levelId);
    }

    public static List<FixedSudokuLevel> GetLevelsByDifficulty(string difficulty)
    {
        var levels = GetAllLevels();
        return levels.FindAll(level => level.difficulty == difficulty);
    }

    public static List<BoardData> GetBoardDataList()
    {
        var levels = GetAllLevels();
        List<BoardData> boardDataList = new List<BoardData>();

        foreach (var level in levels)
        {
            boardDataList.Add(level.ToBoardData());
        }

        return boardDataList;
    }

    public static BoardData GetBoardData(int levelId)
    {
        var level = GetLevel(levelId);
        return level?.ToBoardData();
    }

    // Utility method to validate data consistency
    public static bool ValidateLevel(FixedSudokuLevel level)
    {
        if (level.unsolvedData.Length != 81 || level.solvedData.Length != 81)
        {
            Debug.LogError($"Level {level.levelId}: Array length mismatch!");
            return false;
        }

        // Check if unsolved data is a subset of solved data
        for (int i = 0; i < 81; i++)
        {
            if (level.unsolvedData[i] != 0 && level.unsolvedData[i] != level.solvedData[i])
            {
                Debug.LogError($"Level {level.levelId}: Data inconsistency at index {i}: unsolved={level.unsolvedData[i]}, solved={level.solvedData[i]}");
                return false;
            }
        }

        Debug.Log($"Level {level.levelId} ({level.difficulty}) validation passed!");
        return true;
    }

    public static void ValidateAllLevels()
    {
        Debug.Log("=== VALIDATING ALL SUDOKU LEVELS ===");
        var levels = GetAllLevels();
        bool allValid = true;

        foreach (var level in levels)
        {
            if (!ValidateLevel(level))
            {
                allValid = false;
            }
        }

        if (allValid)
        {
            Debug.Log("✅ ALL LEVELS VALIDATED SUCCESSFULLY!");
        }
        else
        {
            Debug.LogError("❌ SOME LEVELS FAILED VALIDATION!");
        }
    }
}
