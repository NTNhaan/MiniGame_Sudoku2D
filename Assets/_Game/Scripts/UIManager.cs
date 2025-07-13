using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float highScore;
    public Text scoreText;
    public Text HealthPlayer;
    public Text PanelScoreText;
    public Text HighScoreText;
    [SerializeField] private GameManager scoreManager;

    [SerializeField] Image imgStatus;
    [SerializeField] Sprite[] sprStatusPause;
    private bool isPause = false;

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        isPause = !isPause;
        if (isPause)
        {
            SetStatePause(isPause);
            // AudioManager.Instance.TurnOffMusic();
        }
        else
        {
            SetStatePause(isPause);
            // AudioManager.Instance.TurnOnMusic();
        }
    }
    void Start()
    {
        GameManager.instance.OnScoreChanged += UpdateScoreText;
        GameManager.instance.OnScoreChanged += UpdatePanelScoreText;
        GameManager.instance.OnHealthPlayerchanged += UpdateHealthPlayer;
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
        HealthPlayer.text = "Health: " + GameManager.instance.HealthPlayer;
    }
    public void SetStatePause(bool isPause)
    {
        if (isPause)
        {
            Time.timeScale = 0;
            imgStatus.sprite = sprStatusPause[0];
        }
        else
        {
            Time.timeScale = 1;
            imgStatus.sprite = sprStatusPause[1];
        }
    }
}
