using UnityEngine;
using Utils;

public class GameConfigSetting : Singleton<GameConfigSetting>
{
    public enum EGameMode
    {
        NOT_SET,
        EASY,
        MEDIUM,
        HARD,
        IMPOSSIBLE
    }
    public EGameMode gameMode;

    [Header("Level Progression")]
    public int currentDifficultyIndex = 0; // 0=Easy, 1=Medium, 2=Hard, 3=Impossible
    public int currentSubLevelIndex = 0;   // Index trong mỗi difficulty level

    private readonly string[] difficultyOrder = { "Easy", "Medium", "Hard", "Impossible" };

    protected override void CustomAwake()
    {
        gameMode = EGameMode.NOT_SET;
        LoadProgression();
    }

    void Start()
    {
        if (gameMode == EGameMode.NOT_SET)
        {
            SetGameModeByProgression();
        }
    }
    public void SetGameMode(EGameMode mode)
    {
        gameMode = mode;
    }

    public void SetGameMode(string mode)
    {
        switch (mode)
        {
            case "Easy": SetGameMode(EGameMode.EASY); break;
            case "Medium": SetGameMode(EGameMode.MEDIUM); break;
            case "Hard": SetGameMode(EGameMode.HARD); break;
            case "Impossible": SetGameMode(EGameMode.IMPOSSIBLE); break;
            default: SetGameMode(EGameMode.NOT_SET); break;
        }
    }
    public string GetGameMode()
    {
        switch (gameMode)
        {
            case EGameMode.EASY: return "Easy";
            case EGameMode.MEDIUM: return "Medium";
            case EGameMode.HARD: return "Hard";
            case EGameMode.IMPOSSIBLE: return "Impossible";
        }
        Debug.LogError($"ERROR: Game level is not set {gameMode}");
        return " ";
    }

    #region Level Progression System

    private void LoadProgression()
    {
        currentDifficultyIndex = PlayerPrefs.GetInt("CurrentDifficultyIndex", 0);
        currentSubLevelIndex = PlayerPrefs.GetInt("CurrentSubLevelIndex", 0);
    }

    private void SaveProgression()
    {
        PlayerPrefs.SetInt("CurrentDifficultyIndex", currentDifficultyIndex);
        PlayerPrefs.SetInt("CurrentSubLevelIndex", currentSubLevelIndex);
        PlayerPrefs.Save();
    }

    private void SetGameModeByProgression()
    {
        if (currentDifficultyIndex < difficultyOrder.Length)
        {
            SetGameMode(difficultyOrder[currentDifficultyIndex]);
        }
        else
        {
            SetGameMode(EGameMode.IMPOSSIBLE);
        }
    }
    public string GetCurrentDifficulty()
    {
        if (currentDifficultyIndex < difficultyOrder.Length)
        {
            return difficultyOrder[currentDifficultyIndex];
        }
        return "Impossible";
    }

    public int GetCurrentSubLevel()
    {
        return currentSubLevelIndex;
    }

    public bool AdvanceToNextLevel()
    {
        int maxSubLevels = GetMaxSubLevelsForDifficulty(currentDifficultyIndex);

        currentSubLevelIndex++;

        if (currentSubLevelIndex >= maxSubLevels)
        {
            currentSubLevelIndex = 0;
            currentDifficultyIndex++;

            if (currentDifficultyIndex >= difficultyOrder.Length)
            {
                currentDifficultyIndex = difficultyOrder.Length - 1;
                currentSubLevelIndex = maxSubLevels - 1;
                SaveProgression();
                return false;
            }
        }

        SetGameModeByProgression();
        SaveProgression();

        EventManager.LevelChanged();

        return true;
    }

    private int GetMaxSubLevelsForDifficulty(int difficultyIndex)
    {
        switch (difficultyIndex)
        {
            case 0: // Easy
                return LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey("Easy")
                    ? LevelData.Instance.gameDir["Easy"].Count : 3;
            case 1: // Medium
                return LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey("Medium")
                    ? LevelData.Instance.gameDir["Medium"].Count : 3;
            case 2: // Hard
                return LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey("Hard")
                    ? LevelData.Instance.gameDir["Hard"].Count : 2;
            case 3: // Impossible
                return LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey("Impossible")
                    ? LevelData.Instance.gameDir["Impossible"].Count : 2;
            default:
                return 2;
        }
    }

    public void ResetProgression()
    {
        currentDifficultyIndex = 0;
        currentSubLevelIndex = 0;
        SetGameModeByProgression();
        SaveProgression();

        EventManager.LevelChanged();
    }
    public bool IsLastLevel()
    {
        return currentDifficultyIndex >= difficultyOrder.Length - 1 &&
               currentSubLevelIndex >= GetMaxSubLevelsForDifficulty(currentDifficultyIndex) - 1;
    }

    #endregion
}