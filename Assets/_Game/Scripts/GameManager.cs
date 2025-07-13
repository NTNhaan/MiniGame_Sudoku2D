using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    private const string HIGH_SCORE_KEY = "HighScore";
    public event UnityAction<int> OnScoreChanged;
    public event UnityAction<int> OnHealthPlayerchanged;

    public static bool isGameOver => Time.timeScale == 0;
    public static GameManager instance { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int HealthPlayer { get; private set; }
    private float target;
    [SerializeField] private GameObject panelGameOver;
    private void Awake()
    {
        Time.timeScale = 1;
        instance = this;
    }
    void Start()
    {
        Score = 0;
        HealthPlayer = 7;
        EventManager.OnAddPoints += HandleAddPoints;
        EventManager.OnHPchanged += HandleHealPlayer;
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY);
    }

    void Update()
    {
        CheckGameOver();
    }
    public void HandleAddPoints(int points)
    {
        Score += points;
        // AudioManager.Instance.PlayerDeadSound();
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
            PlayerPrefs.Save();
        }
        OnScoreChanged?.Invoke(Score);  // thông báo cho score changed obstacle và UI
        // Debug.Log("Points added: " +  Score);
    }

    public void HandleHealPlayer(int _HealPlayer)
    {
        // AudioManager.Instance.PlayClickSound();
        HealthPlayer -= _HealPlayer;
        // Debug.Log("HealthPlayer: " +  HealthPlayer);
        OnHealthPlayerchanged?.Invoke(HealthPlayer);
    }

    void CheckGameOver()
    {
        if (HealthPlayer <= 0)
        {
            // AudioManager.Instance.TurnOffMusic();
            panelGameOver.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public int GetHighScore()
    {
        return HighScore;
    }

    public int GetHealthPlayer()
    {
        return HealthPlayer;
    }
}
