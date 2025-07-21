using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    #region Booster Event
    public static event UnityAction<bool> OnNotesActive;
    public static void NotesActive(bool active) => OnNotesActive?.Invoke(active);
    public static event UnityAction OnClearNumber;
    public static void ClearNumber() => OnClearNumber?.Invoke();
    public static event UnityAction OnUndoNumber;
    public static void UndoNumber() => OnUndoNumber?.Invoke();
    public static event UnityAction OnHintNumber;
    public static void HintNumber() => OnHintNumber?.Invoke();
    #endregion

    #region Level Progression
    public static event UnityAction OnPuzzleComplete;
    public static void PuzzleComplete() => OnPuzzleComplete?.Invoke();

    public static event UnityAction OnLevelChanged;
    public static void LevelChanged() => OnLevelChanged?.Invoke();
    #endregion
}
