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
}
