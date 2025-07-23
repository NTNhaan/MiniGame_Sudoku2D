using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float highScore;
    public Text scoreText;
    public Text HealthPlayer;
    public Text PanelScoreText;
    public Text HighScoreText;
    [SerializeField] private GameManager scoreManager;
    [SerializeField] private GameObject PopupShopIAP;
    // [SerializeField] Image imgStatus;
    // [SerializeField] Sprite[] sprStatusPause;
    // private bool isPause = false;

    [SerializeField] private List<Image> lstImgHealth;
    [SerializeField] private Sprite imgHealthNonColor;
    [SerializeField] private Text LevelTxt;
    [SerializeField] private Text UndoCountTxt;
    private int health = 0;
    private int errorNumber = 0;
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // public void PauseGame()
    // {
    //     isPause = !isPause;
    //     if (isPause)
    //     {
    //         SetStatePause(isPause);
    //         // AudioManager.Instance.TurnOffMusic();
    //     }
    //     else
    //     {
    //         SetStatePause(isPause);
    //         // AudioManager.Instance.TurnOnMusic();
    //     }
    // }
    void Start()
    {
        health = GameManager.instance.HealthPlayer;
        errorNumber = 0;
        GameManager.instance.OnScoreChanged += UpdateScoreText;
        GameManager.instance.OnScoreChanged += UpdatePanelScoreText;
        GameManager.instance.OnHealthPlayerchanged += UpdateHealthPlayer;
        EventManager.OnWrongNumber += ActionWrongNumber;
        EventManager.OnLevelChanged += UpdateLevelText;
        EventManager.OnUndoCountChanged += UpdateUndoCount;

        StartCoroutine(UpdateLevelTextWithDelay());
        UpdateUndoCount();
    }

    private IEnumerator UpdateLevelTextWithDelay()
    {
        yield return null;

        int maxRetries = 10;
        int retryCount = 0;

        while (GameConfigSetting.Instance == null && retryCount < maxRetries)
        {
            yield return new WaitForSeconds(0.1f);
            retryCount++;
        }

        UpdateLevelText();
    }

    private void Update()
    {
        HighScoreText.text = "HighScore: " + scoreManager.GetHighScore();
        // HealthPlayer.text = "Health: " + scoreManager.GetHealthPlayer();
        // Debug.Log("hightScore: "  + highScore);
    }

    void OnDestroy()
    {
        GameManager.instance.OnScoreChanged -= UpdateScoreText;
        GameManager.instance.OnScoreChanged -= UpdatePanelScoreText;
        GameManager.instance.OnHealthPlayerchanged -= UpdateHealthPlayer;
        EventManager.OnWrongNumber -= ActionWrongNumber;
        EventManager.OnLevelChanged -= UpdateLevelText;
    }
    void ActionWrongNumber()
    {
        if (errorNumber < lstImgHealth.Count)
        {
            lstImgHealth[errorNumber].sprite = imgHealthNonColor;
            errorNumber++;
            // health--;
            GameManager.instance.HandleHealPlayer(1);
        }
    }
    void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + GameManager.instance.Score;
    }

    void UpdatePanelScoreText(int score)
    {
        PanelScoreText.text = "Score: " + GameManager.instance.Score;
    }

    void UpdateHealthPlayer(int health)
    {
        // udpate số lượng lives cho user
        HealthPlayer.text = "Health: " + GameManager.instance.HealthPlayer;
    }

    void UpdateLevelText()
    {
        if (LevelTxt != null)
        {
            // Tính toán level tổng từ difficulty và sublevel
            if (GameConfigSetting.Instance != null)
            {
                string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
                int currentSubLevel = GameConfigSetting.Instance.GetCurrentSubLevel();

                // Tính level tổng dựa trên progression
                int totalLevel = CalculateTotalLevelFromProgression(currentDifficulty, currentSubLevel);

                // Lấy thông tin level từ FixedSudokuLevelData
                var allLevels = FixedSudokuLevelData.GetAllLevels();
                if (totalLevel > 0 && totalLevel <= allLevels.Count)
                {
                    var levelData = allLevels[totalLevel - 1]; // Convert to 0-based index
                    string levelText = $"Level {levelData.levelId} - {levelData.difficulty}";
                    LevelTxt.text = levelText;

                    Debug.Log($"UIManager: Updated LevelTxt to '{levelText}' (Total Level: {totalLevel})");
                }
                else
                {
                    // Fallback to old display format
                    int displaySubLevel = currentSubLevel + 1;
                    int totalSubLevels = GetTotalSubLevelsForCurrentDifficulty();
                    string levelText = $"{currentDifficulty} {displaySubLevel}/{totalSubLevels}";
                    LevelTxt.text = levelText;

                    Debug.Log($"UIManager: Updated LevelTxt to '{levelText}' (fallback)");
                }
            }
            else
            {
                LevelTxt.text = "Level 1";
                Debug.Log("UIManager: GameConfigSetting not available, using default");
            }
        }
    }

    private int CalculateTotalLevelFromProgression(string difficulty, int subLevel)
    {
        int baseLevel = 0;

        switch (difficulty)
        {
            case "Easy":
                baseLevel = 0;
                break;
            case "Medium":
                baseLevel = GetTotalSubLevelsForDifficulty("Easy");
                break;
            case "Hard":
                baseLevel = GetTotalSubLevelsForDifficulty("Easy") + GetTotalSubLevelsForDifficulty("Medium");
                break;
            case "Impossible":
                baseLevel = GetTotalSubLevelsForDifficulty("Easy") + GetTotalSubLevelsForDifficulty("Medium") + GetTotalSubLevelsForDifficulty("Hard");
                break;
        }

        return baseLevel + subLevel + 1; // +1 because levels are 1-based
    }

    private int GetTotalSubLevelsForDifficulty(string difficulty)
    {
        if (LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey(difficulty))
        {
            return LevelData.Instance.gameDir[difficulty].Count;
        }
        return 2; // Default fallback
    }

    private int GetTotalSubLevelsForCurrentDifficulty()
    {
        if (LevelData.Instance != null && GameConfigSetting.Instance != null)
        {
            string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
            if (LevelData.Instance.gameDir.ContainsKey(currentDifficulty))
            {
                return LevelData.Instance.gameDir[currentDifficulty].Count;
            }
        }
        return 3;
    }

    public void UpdateUndoCount()
    {
        if (UndoCountTxt != null && BoardController.Instance != null)
        {
            int undoCount = BoardController.Instance.GetUndoCount();
            UndoCountTxt.text = $"Undo: {undoCount}";
        }
    }
    public void UpdateUndoCount(int dummy)
    {
        UpdateUndoCount();
    }

    public void ForceUpdateLevelText()
    {
        UpdateLevelText();
    }

    // public void SetStatePause(bool isPause)
    // {
    //     if (isPause)
    //     {
    //         Time.timeScale = 0;
    //         imgStatus.sprite = sprStatusPause[0];
    //     }
    //     else
    //     {
    //         Time.timeScale = 1;
    //         imgStatus.sprite = sprStatusPause[1];
    //     }
    // }
}
