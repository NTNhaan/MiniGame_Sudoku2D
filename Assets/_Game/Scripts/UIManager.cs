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

    // [SerializeField] Image imgStatus;
    // [SerializeField] Sprite[] sprStatusPause;
    // private bool isPause = false;

    [SerializeField] private List<Image> lstImgHealth;
    [SerializeField] private Sprite imgHealthNonColor;
    [SerializeField] private Text LevelTxt;
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

        StartCoroutine(UpdateLevelTextWithDelay());
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
        if (GameConfigSetting.Instance != null && LevelTxt != null)
        {
            string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
            int currentSubLevel = GameConfigSetting.Instance.GetCurrentSubLevel() + 1;

            int totalSubLevels = GetTotalSubLevelsForCurrentDifficulty();

            string levelText = $"{currentDifficulty} {currentSubLevel}/{totalSubLevels}";
            LevelTxt.text = levelText;

            Debug.Log($"UIManager: Updated LevelTxt to '{levelText}'");
        }
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
        return 3; // Default fallback
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
