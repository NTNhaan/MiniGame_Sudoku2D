using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float highScore;
    public Text scoreText;
    public Text HealthPlayer;
    // public Text PanelScoreText;
    // public Text HighScoreText;
    // [SerializeField] private GameObject imgCover;
    [SerializeField] private GameManager scoreManager;
    [SerializeField] private GameObject PopupShopIAP;
    [SerializeField] private GameObject PopupWin;
    [SerializeField] private Text textLevelPopup;

    // [SerializeField] Image imgStatus;
    // [SerializeField] Sprite[] sprStatusPause;
    // private bool isPause = false;

    [SerializeField] private List<Image> lstImgHealth;
    [SerializeField] private Sprite imgHealthNonColor;
    [SerializeField] private Text LevelTxt;
    [SerializeField] private Text HintCountTxt;
    [SerializeField] private Text CoinCountTxt;
    [SerializeField] private Text DebugTxt;
    private int health = 0;
    private int errorNumber = 0;

    [SerializeField] private GameObject EnoughCoinsPopup;
    [SerializeField] private GameObject notEnoughCoinsPopup;
    [SerializeField] private Text notEnoughCoinsText;

    void Start()
    {
        health = GameManager.instance.HealthPlayer;
        errorNumber = 0;
        // GameManager.instance.OnScoreChanged += UpdateScoreText;
        // GameManager.instance.OnScoreChanged += UpdatePanelScoreText;
        GameManager.instance.OnHealthPlayerchanged += UpdateHealthPlayer;
        GameManager.instance.OnCoinsChanged += UpdateCoinCount;
        EventManager.OnWrongNumber += ActionWrongNumber;
        EventManager.OnLevelChanged += UpdateLevelText;
        EventManager.OnLevelCompleted += ShowWinPopup;
        EventManager.OnGameOverUI += ShowGameOverUI;
        EventManager.OnUndoCountChanged += UpdateUndoCount;
        EventManager.OnHintCountChanged += UpdateHintCount;
        EventManager.OnSquareSelected += UpdateDebugDisplay;
        EventManager.OnShowNotEnoughCoinsMessage += ShowNotEnoughCoinsPopup;

        StartCoroutine(UpdateLevelTextWithDelay());
        UpdateUndoCount();
        StartCoroutine(UpdateHintCountWithDelay());
        UpdateCoinCount(); // Initial coin display
    }

    private IEnumerator UpdateLevelTextWithDelay()
    {
        yield return null;

        int maxRetries = 20; // Tăng từ 10 lên 20
        int retryCount = 0;

        while (GameConfigSetting.Instance == null && retryCount < maxRetries)
        {
            yield return new WaitForSeconds(0.1f);
            retryCount++;
        }

        if (GameConfigSetting.Instance != null)
        {
            UpdateLevelText();
        }
        else
        {
            LevelTxt.text = "Level 1"; // Fallback
        }
    }

    private IEnumerator UpdateHintCountWithDelay()
    {
        yield return null;

        int maxRetries = 20;
        int retryCount = 0;

        while (BoardController.Instance == null && retryCount < maxRetries)
        {
            yield return new WaitForSeconds(0.1f);
            retryCount++;
        }

        if (BoardController.Instance != null)
        {
            UpdateHintCount();
        }
    }

    private void ShowNotEnoughCoinsPopup(int requiredCoins)
    {
        if (notEnoughCoinsPopup != null)
        {
            EnoughCoinsPopup.SetActive(false);
            notEnoughCoinsPopup.SetActive(true);
        }
    }

    private IEnumerator HideNotEnoughCoinsPopup()
    {
        yield return new WaitForSeconds(2f);
        if (notEnoughCoinsPopup != null)
        {
            UpdateHintCount();
        }
    }

    private void Update()
    {
        // HighScoreText.text = "HighScore: " + scoreManager.GetHighScore();
        // HealthPlayer.text = "Health: " + scoreManager.GetHealthPlayer();
        // Debug.Log("hightScore: "  + highScore);
    }

    void OnDestroy()
    {
        // GameManager.instance.OnScoreChanged -= UpdateScoreText;
        // GameManager.instance.OnScoreChanged -= UpdatePanelScoreText;
        GameManager.instance.OnHealthPlayerchanged -= UpdateHealthPlayer;
        GameManager.instance.OnCoinsChanged -= UpdateCoinCount;
        EventManager.OnWrongNumber -= ActionWrongNumber;
        EventManager.OnLevelChanged -= UpdateLevelText;
        EventManager.OnLevelCompleted -= ShowWinPopup;
        EventManager.OnGameOverUI -= ShowGameOverUI;
        EventManager.OnHintCountChanged -= UpdateHintCount;
        EventManager.OnSquareSelected -= UpdateDebugDisplay;
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

    void ShowGameOverUI()
    {
        // ShowCoverImg(true);
        Debug.Log("Game Over UI - Cover image shown");
    }
    void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + GameManager.instance.Score;
    }

    // void UpdatePanelScoreText(int score)
    // {
    //     PanelScoreText.text = "Score: " + GameManager.instance.Score;
    // }

    void UpdateHealthPlayer(int health)
    {
        HealthPlayer.text = "Health: " + GameManager.instance.HealthPlayer;
    }

    void UpdateLevelText()
    {
        if (LevelTxt != null)
        {
            if (GameConfigSetting.Instance != null)
            {
                string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
                int currentSubLevel = GameConfigSetting.Instance.GetCurrentSubLevel();

                int totalLevel = CalculateTotalLevelFromProgression(currentDifficulty, currentSubLevel);

                var allLevels = FixedSudokuLevelData.GetAllLevels();
                if (totalLevel > 0 && totalLevel <= allLevels.Count)
                {
                    var levelData = allLevels[totalLevel - 1]; // Convert to 0-based index
                    string levelText = $"Level {levelData.levelId}";
                    LevelTxt.text = levelText;
                }
                else
                {
                    int displaySubLevel = currentSubLevel + 1;
                    int totalSubLevels = GetTotalSubLevelsForCurrentDifficulty();
                    string levelText = $"{currentDifficulty} {displaySubLevel}/{totalSubLevels}";
                    LevelTxt.text = levelText;

                }
            }
            else
            {
                LevelTxt.text = "Level 1";
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

        return baseLevel + subLevel + 1;
    }

    private int GetTotalSubLevelsForDifficulty(string difficulty)
    {
        if (LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey(difficulty))
        {
            return LevelData.Instance.gameDir[difficulty].Count;
        }
        return 2;
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
        // if (UndoCountTxt != null && BoardController.Instance != null)
        // {
        //     int undoCount = BoardController.Instance.GetUndoCount();
        //     UndoCountTxt.text = $"Undo: {undoCount}";
        // }
    }
    public void UpdateUndoCount(int dummy)
    {
        UpdateUndoCount();
    }

    public void UpdateHintCount()
    {
        if (HintCountTxt != null && BoardController.Instance != null)
        {
            int hintCount = BoardController.Instance.GetHintCount();
            HintCountTxt.text = hintCount.ToString();
            if (hintCount <= 0)
            {
                if (notEnoughCoinsPopup != null && EnoughCoinsPopup != null)
                {
                    EnoughCoinsPopup.SetActive(false);
                    notEnoughCoinsPopup.SetActive(true);
                    if (notEnoughCoinsText != null)
                    {
                        notEnoughCoinsText.text = GameManager.instance.GetHintCost().ToString();
                    }
                }
            }
            else
            {
                if (notEnoughCoinsPopup != null && EnoughCoinsPopup != null)
                {
                    EnoughCoinsPopup.SetActive(true);
                    notEnoughCoinsPopup.SetActive(false);
                }
            }
        }
    }
    public void UpdateHintCount(int dummy)
    {
        UpdateHintCount();
    }

    public void UpdateCoinCount()
    {
        if (CoinCountTxt != null && GameManager.instance != null)
        {
            int coins = GameManager.instance.GetCoins();
            CoinCountTxt.text = coins.ToString();
        }
    }

    public void UpdateCoinCount(int coins)
    {
        if (CoinCountTxt != null)
        {
            CoinCountTxt.text = coins.ToString();
        }
    }

    public void ForceUpdateLevelText()
    {
        UpdateLevelText();
    }

    public void ForceUpdateHintCount()
    {
        UpdateHintCount();
    }

    [ContextMenu("Test Hint Count Update")]
    public void TestHintCountUpdate()
    {
        UpdateHintCount();
    }

    [ContextMenu("Force Update All UI")]
    public void ForceUpdateAllUI()
    {
        UpdateLevelText();
        UpdateHintCount();
        UpdateCoinCount();
    }

    public void UpdateDebugDisplay(int squareIndex)
    {
        if (DebugTxt != null && GameManager.instance != null && GameManager.instance.ShowCorrectNumberDebug)
        {
            if (squareIndex >= 0 && BoardController.Instance != null)
            {
                var squares = BoardController.Instance.GetSquareComponents();
                if (squares != null && squareIndex < squares.Count)
                {
                    var square = squares[squareIndex];
                    string debugText = $"DEBUG: Square {squareIndex}\nCurrent: {square.GetNumber()}\nCorrect: {square.GetCorrectNumber()}";

                    if (square.GetHasDefaultValue())
                    {
                        debugText += "\n[DEFAULT]";
                    }

                    DebugTxt.text = debugText;
                    DebugTxt.gameObject.SetActive(true);
                }
            }
            else
            {
                DebugTxt.gameObject.SetActive(false);
            }
        }
        else if (DebugTxt != null)
        {
            DebugTxt.gameObject.SetActive(false);
        }
    }


    #region On Click Popup Region
    // public void ShowCoverImg(bool enabled)
    // {
    //     imgCover.SetActive(enabled);
    // }
    public void OnCLickShowShopIAP()
    {
        AudioController.Instance.PlayClickSound();
        // ShowCoverImg(true);
        PopupShopIAP.SetActive(true);
    }
    public void OnClickHideShopIAP()
    {
        AudioController.Instance.PlayClickSound();
        // ShowCoverImg(false);
        PopupShopIAP.SetActive(false);
    }
    public void OnClickShowPopupWin()
    {
        AudioController.Instance.PlayWinSound();
        // ShowCoverImg(true);
        PopupWin.SetActive(true);
    }
    public void OnClickHidePopupWin()
    {
        // ShowCoverImg(false);
        PopupWin.SetActive(false);
    }

    public void ShowWinPopup(string levelInfo)
    {
        if (PopupWin != null)
        {
            PopupWin.SetActive(true);

            if (textLevelPopup != null)
            {
                textLevelPopup.text = levelInfo;
            }

            Debug.Log($"Win popup shown with text: {levelInfo}");

            // StartCoroutine(AutoHideWinPopup());
        }
    }

    private IEnumerator AutoHideWinPopup()
    {
        yield return new WaitForSeconds(2.5f);
        HideWinPopup();
    }

    public void HideWinPopup()
    {
        if (PopupWin != null)
        {
            PopupWin.SetActive(false);
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayClickSound();
        }

        HideWinPopup();
        // ShowCoverImg(false);

        if (GameConfigSetting.Instance != null)
        {
            bool hasNextLevel = GameConfigSetting.Instance.AdvanceToNextLevel();

            if (hasNextLevel)
            {
                if (BoardController.Instance != null)
                {
                    string nextDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
                    BoardController.Instance.LoadLevel(nextDifficulty);
                    Debug.Log("Loading next level...");
                }
            }
            else
            {
                ShowGameCompleteMessage();
            }
        }
    }

    private void ShowGameCompleteMessage()
    {
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.ResetToFirstLevel();
            if (BoardController.Instance != null)
            {
                string firstLevel = GameConfigSetting.Instance.GetCurrentDifficulty();
                BoardController.Instance.LoadLevel(firstLevel);
            }
        }
    }
    public void SkipToNextLevel()
    {
        if (GameConfigSetting.Instance != null && BoardController.Instance != null)
        {
            bool hasNextLevel = GameConfigSetting.Instance.AdvanceToNextLevel();

            if (hasNextLevel)
            {
                string nextDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
                BoardController.Instance.LoadLevel(nextDifficulty);
                Debug.Log("Skipped to next level");
            }
            else
            {
                Debug.Log("No more levels to skip to");
            }
        }
    }

    public void RestartCurrentLevel()
    {
        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayClickSound();
        }

        if (GameConfigSetting.Instance != null && BoardController.Instance != null)
        {
            string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
            BoardController.Instance.LoadLevel(currentDifficulty);
            Debug.Log("Restarted current level");
        }
        HideWinPopup();
        // ShowCoverImg(false);
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
    #endregion
}
