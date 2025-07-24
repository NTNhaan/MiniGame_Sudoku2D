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

[CreateAssetMenu(fileName = "SudokuLevelData", menuName = "Sudoku/Level Data")]
public class FixedSudokuLevelData : ScriptableObject
{
    private static FixedSudokuLevelData instance;
    public static FixedSudokuLevelData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<FixedSudokuLevelData>("SudokuLevelData");
                if (instance == null)
                {
                    instance = CreateInstance<FixedSudokuLevelData>();
                    instance.CreateLevels();
                }
            }
            return instance;
        }
    }

    [SerializeField] private List<FixedSudokuLevel> allLevels = new List<FixedSudokuLevel>();

    private void OnEnable()
    {
        instance = this;
        if (allLevels == null || allLevels.Count == 0)
        {
            CreateLevels();
        }
    }

    public void CreateLevels()
    {
        allLevels.Clear();
        CreateAllLevels();
    }

    private void CreateAllLevels()
    {
        // Tạo 5 bảng Sudoku solved khác nhau
        List<int[]> solvedBoards = new List<int[]>
        {
            new int[81] { // Board 1
                5, 3, 4, 6, 7, 8, 9, 1, 2,
                6, 7, 2, 1, 9, 5, 3, 4, 8,
                1, 9, 8, 3, 4, 2, 5, 6, 7,
                8, 5, 9, 7, 6, 1, 4, 2, 3,
                4, 2, 6, 8, 5, 3, 7, 9, 1,
                7, 1, 3, 9, 2, 4, 8, 5, 6,
                9, 6, 1, 5, 3, 7, 2, 8, 4,
                2, 8, 7, 4, 1, 9, 6, 3, 5,
                3, 4, 5, 2, 8, 6, 1, 7, 9
            },
            new int[81] { // Board 2
                1, 2, 3, 4, 5, 6, 7, 8, 9,
                4, 5, 6, 7, 8, 9, 1, 2, 3,
                7, 8, 9, 1, 2, 3, 4, 5, 6,
                2, 3, 1, 5, 6, 4, 8, 9, 7,
                5, 6, 4, 8, 9, 7, 2, 3, 1,
                8, 9, 7, 2, 3, 1, 5, 6, 4,
                3, 1, 2, 6, 4, 5, 9, 7, 8,
                6, 4, 5, 9, 7, 8, 3, 1, 2,
                9, 7, 8, 3, 1, 2, 6, 4, 5
            },
            new int[81] { // Board 3
                7, 3, 5, 6, 1, 4, 8, 9, 2,
                8, 4, 2, 9, 7, 3, 5, 6, 1,
                9, 6, 1, 2, 8, 5, 3, 7, 4,
                2, 8, 6, 3, 4, 9, 1, 5, 7,
                4, 1, 3, 8, 5, 7, 9, 2, 6,
                5, 7, 9, 1, 2, 6, 4, 3, 8,
                1, 5, 7, 4, 9, 2, 6, 8, 3,
                3, 2, 4, 7, 6, 8, 5, 1, 9,
                6, 9, 8, 5, 3, 1, 7, 4, 2
            },
            new int[81] { // Board 4
                2, 9, 5, 7, 4, 3, 8, 6, 1,
                4, 3, 1, 8, 6, 5, 9, 2, 7,
                8, 7, 6, 1, 9, 2, 5, 4, 3,
                3, 8, 7, 4, 5, 9, 2, 1, 6,
                6, 1, 2, 3, 8, 7, 4, 9, 5,
                5, 4, 9, 2, 1, 6, 7, 3, 8,
                7, 6, 3, 5, 2, 4, 1, 8, 9,
                9, 2, 8, 6, 7, 1, 3, 5, 4,
                1, 5, 4, 9, 3, 8, 6, 7, 2
            },
            new int[81] { // Board 5
                6, 2, 4, 1, 3, 9, 5, 8, 7,
                1, 3, 7, 8, 5, 6, 2, 4, 9,
                9, 5, 8, 7, 2, 4, 3, 1, 6,
                8, 7, 1, 2, 4, 3, 6, 9, 5,
                3, 4, 5, 6, 9, 8, 1, 7, 2,
                2, 9, 6, 5, 7, 1, 4, 3, 8,
                5, 6, 3, 4, 1, 7, 8, 2, 9,
                4, 1, 9, 3, 8, 2, 7, 5, 6,
                7, 8, 2, 9, 6, 5, 3, 4, 1
            }
        };

        // Mở rộng mảng độ khó và số ô cần xóa
        string[] difficulties = new string[50];
        int[] cluesToRemove = new int[50];

        // Thiết lập độ khó và số ô cần xóa cho 50 level
        for (int i = 0; i < 50; i++)
        {
            if (i < 10) { difficulties[i] = "Very Easy"; cluesToRemove[i] = 35 + (i % 5); }
            else if (i < 20) { difficulties[i] = "Easy"; cluesToRemove[i] = 40 + (i % 5); }
            else if (i < 30) { difficulties[i] = "Medium"; cluesToRemove[i] = 45 + (i % 5); }
            else if (i < 40) { difficulties[i] = "Hard"; cluesToRemove[i] = 50 + (i % 5); }
            else if (i < 45) { difficulties[i] = "Very Hard"; cluesToRemove[i] = 55 + (i % 5); }
            else { difficulties[i] = "Extreme"; cluesToRemove[i] = 60 + (i % 5); }
        }

        // Tạo 50 level từ 5 bảng gốc
        for (int levelIndex = 0; levelIndex < 50; levelIndex++)
        {
            int[] solvedData = new int[81];
            // Chọn một trong 5 bảng gốc theo thứ tự xoay vòng
            System.Array.Copy(solvedBoards[levelIndex % 5], solvedData, 81);

            int[] unsolvedData = new int[81];
            System.Array.Copy(solvedData, unsolvedData, 81);
            RemoveRandomClues(unsolvedData, cluesToRemove[levelIndex]);

            allLevels.Add(new FixedSudokuLevel(
                levelIndex + 1,
                difficulties[levelIndex],
                unsolvedData,
                solvedData
            ));
        }
    }
    private static void RemoveRandomClues(int[] data, int totalCluesToRemove)
    {
        var random = new System.Random();
        var filledIndices = new List<int>();
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != 0)
            {
                filledIndices.Add(i);
            }
        }

        int currentClues = filledIndices.Count;
        int targetClues = 81 - totalCluesToRemove;
        int cluesToRemove = currentClues - targetClues;

        cluesToRemove = UnityEngine.Mathf.Max(0, UnityEngine.Mathf.Min(cluesToRemove, filledIndices.Count));

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
        return Instance.allLevels;
    }

    public static FixedSudokuLevel GetLevel(int levelId)
    {
        return Instance.allLevels.Find(level => level.levelId == levelId);
    }

    public static List<FixedSudokuLevel> GetLevelsByDifficulty(string difficulty)
    {
        return Instance.allLevels.FindAll(level => level.difficulty == difficulty);
    }

    public static List<BoardData> GetBoardDataList()
    {
        List<BoardData> boardDataList = new List<BoardData>();
        foreach (var level in Instance.allLevels)
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

    public static bool ValidateLevel(FixedSudokuLevel level)
    {
        if (level.unsolvedData.Length != 81 || level.solvedData.Length != 81)
        {
            return false;
        }

        for (int i = 0; i < 81; i++)
        {
            if (level.unsolvedData[i] != 0 && level.unsolvedData[i] != level.solvedData[i])
            {
                return false;
            }
        }
        return true;
    }

    public static void ValidateAllLevels()
    {
        foreach (var level in Instance.allLevels)
        {
            if (!ValidateLevel(level))
            {
                Debug.LogError($"Level {level.levelId} is invalid!");
            }
        }
    }
}
