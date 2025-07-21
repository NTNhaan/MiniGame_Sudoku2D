using UnityEngine;

public class LinePosition
{
    public int row;
    public int col;

    public LinePosition(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public override string ToString()
    {
        return $"Row: {row}, Col: {col}";
    }
}