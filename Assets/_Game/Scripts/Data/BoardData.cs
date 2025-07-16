using System;
using UnityEngine;

[Serializable]
public class BoardData
{
    public int[] unsolved_data;
    public int[] solved_data;

    public BoardData() { }

    public BoardData(int[] unsolved, int[] solved)
    {
        this.unsolved_data = unsolved;
        this.solved_data = solved;
    }

    // // Thêm một số method tiện ích
    // public int GetUnsolvedValue(int row, int col)
    // {
    //     if (row >= 0 && row < 9 && col >= 0 && col < 9)
    //     {
    //         return unsolved_data[row * 9 + col];
    //     }
    //     return -1;
    // }

    // public int GetSolvedValue(int row, int col)
    // {
    //     if (row >= 0 && row < 9 && col >= 0 && col < 9)
    //     {
    //         return solved_data[row * 9 + col];
    //     }
    //     return -1;
    // }

    // public bool IsEmpty(int row, int col)
    // {
    //     return GetUnsolvedValue(row, col) == 0;
    // }

    // public bool IsCorrect(int row, int col, int value)
    // {
    //     return GetSolvedValue(row, col) == value;
    // }
}
