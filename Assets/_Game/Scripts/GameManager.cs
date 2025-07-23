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
    private const string COINS_KEY = "PlayerCoins";
    private const int UNDO_COST = 10;
    private const int ERASE_COST = 5;

    // Debug feature
    [SerializeField] private bool showCorrectNumberDebug = false;
    public bool ShowCorrectNumberDebug => showCorrectNumberDebug;

    public event UnityAction<int> OnScoreChanged;
    public event UnityAction<int> OnHealthPlayerchanged;
    public event UnityAction<int> OnCoinsChanged;

    public static bool isGameOver => Time.timeScale == 0;
    public static GameManager instance { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int HealthPlayer { get; private set; }
    public int Coins { get; private set; }
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
        HealthPlayer = 3;
        Coins = PlayerPrefs.GetInt(COINS_KEY, 100); // Start with 100 coins
        EventManager.OnAddPoints += HandleAddPoints;
        EventManager.OnHPchanged += HandleHealPlayer;
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY);

        // Trigger initial coin UI update
        OnCoinsChanged?.Invoke(Coins);
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
        OnScoreChanged?.Invoke(Score);
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
            // Play lose sound
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayLoseSound();
            }

            // Notify UI to show cover
            EventManager.GameOverUI();

            EventManager.IsGameOver();
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

    #region Coin Management
    public bool CanAfford(int amount)
    {
        return Coins >= amount;
    }

    public bool SpendCoins(int amount)
    {
        if (CanAfford(amount))
        {
            Coins -= amount;
            PlayerPrefs.SetInt(COINS_KEY, Coins);
            PlayerPrefs.Save();
            OnCoinsChanged?.Invoke(Coins);
            Debug.Log($"Spent {amount} coins. Remaining: {Coins}");
            return true;
        }
        Debug.Log($"Not enough coins! Need {amount}, have {Coins}");
        return false;
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        PlayerPrefs.SetInt(COINS_KEY, Coins);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(Coins);
        Debug.Log($"Added {amount} coins. Total: {Coins}");
    }

    public int GetCoins()
    {
        return Coins;
    }

    public int GetUndoCost()
    {
        return UNDO_COST;
    }

    public int GetEraseCost()
    {
        return ERASE_COST;
    }

    #region Debug Features
    [ContextMenu("Toggle Debug Mode")]
    public void ToggleDebugMode()
    {
        showCorrectNumberDebug = !showCorrectNumberDebug;
        Debug.Log($"Debug Mode: {(showCorrectNumberDebug ? "ON" : "OFF")} - Correct numbers will {(showCorrectNumberDebug ? "be shown" : "be hidden")} when selecting squares");
    }

    public void SetDebugMode(bool enabled)
    {
        showCorrectNumberDebug = enabled;
        Debug.Log($"Debug Mode: {(showCorrectNumberDebug ? "ON" : "OFF")}");
    }
    #endregion
    #endregion
}
