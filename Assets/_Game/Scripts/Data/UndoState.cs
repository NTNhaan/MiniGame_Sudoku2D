
using UnityEditor.UI;
using System;
using UnityEngine;

[System.Serializable]
public class UndoState
{
    public int squareIndex;
    public int previousNumber;
    public bool previousHasWrongValue;
    public Color previousTextColor;
    public Color previousBackgroundColor;

    public UndoState(int index, int number, bool wrongValue, Color textColor, Color bgColor)
    {
        squareIndex = index;
        previousNumber = number;
        previousHasWrongValue = wrongValue;
        previousTextColor = textColor;
        previousBackgroundColor = bgColor;
    }
}