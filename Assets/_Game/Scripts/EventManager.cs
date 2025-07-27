using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static event UnityAction<int> OnAddPoints;
    public static void AddPoints(int points) => OnAddPoints?.Invoke(points);

    public static event UnityAction<int> OnHPchanged;
    public static void HPChanged(int hp) => OnHPchanged?.Invoke(hp);


    public static event UnityAction<int> OnUpdateNumber;
    public static void UpdateSquareNumber(int number) => OnUpdateNumber?.Invoke(number);

    public static event UnityAction<int> OnSquareSelected;
    public static void SquareSeleced(int squareIndex) => OnSquareSelected?.Invoke(squareIndex);

    public static event UnityAction OnWrongNumber;
    public static void SelectWrongNumber() => OnWrongNumber?.Invoke();

    public static event UnityAction OnGameOver;
    public static void IsGameOver() => OnGameOver?.Invoke();

    public static event UnityAction OnGameOverUI;
    public static void GameOverUI() => OnGameOverUI?.Invoke();

    #region Booster Event
    public static event UnityAction<bool> OnNotesActive;
    public static void NotesActive(bool active) => OnNotesActive?.Invoke(active);
    public static event UnityAction OnClearNumber;
    public static void ClearNumber() => OnClearNumber?.Invoke();
    public static event UnityAction OnUndoNumber;
    public static void UndoNumber() => BoardController.Instance.PerformUndo();
    public static event UnityAction OnHintNumber;
    public static void HintNumber() => OnHintNumber?.Invoke();
    #endregion

    #region Level Progression
    public static event UnityAction OnPuzzleComplete;
    public static void PuzzleComplete() => OnPuzzleComplete?.Invoke();

    public static event UnityAction<string> OnLevelCompleted;
    public static void LevelCompleted(string levelInfo) => OnLevelCompleted?.Invoke(levelInfo);

    public static event UnityAction OnLevelChanged;
    public static void LevelChanged() => OnLevelChanged?.Invoke();

    public static event UnityAction OnUndoCountChanged;
    public static void UndoCountChanged() => OnUndoCountChanged?.Invoke();

    public static event UnityAction OnHintCountChanged;
    public static void HintCountChanged() => OnHintCountChanged?.Invoke();

    public static event UnityAction<int> OnShowNotEnoughCoinsMessage;
    public static void ShowNotEnoughCoinsMessage(int requiredCoins) => OnShowNotEnoughCoinsMessage?.Invoke(requiredCoins);
    #endregion
}
