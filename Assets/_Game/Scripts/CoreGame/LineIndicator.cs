using UnityEngine;
using Utils;

public class LineIndicator : Singleton<LineIndicator>
{

    #region InitData
    private int[,] line_data = new int[9, 9] // lấy index row-col để ánh xạ vị trí cần hightlight
    {
        {  0,  1,  2,  3,  4,  5,  6,  7,  8 },
        {  9, 10, 11, 12, 13, 14, 15, 16, 17 },
        { 18, 19, 20, 21, 22, 23, 24, 25, 26 },

        { 27, 28, 29, 30, 31, 32, 33, 34, 35 },
        { 36, 37, 38, 39, 40, 41, 42, 43, 44 },
        { 45, 46, 47, 48, 49, 50, 51, 52, 53 },

        { 54, 55, 56, 57, 58, 59, 60, 61, 62 },
        { 63, 64, 65, 66, 67, 68, 69, 70, 71 },
        { 72, 73, 74, 75, 76, 77, 78, 79, 80 }
    };

    private int[] line_data_falt = new int[81]
    {
        0,  1,  2,  3,  4,  5,  6,  7,  8,
        9, 10, 11, 12, 13, 14, 15, 16, 17,
        18, 19, 20, 21, 22, 23, 24, 25, 26,
        27, 28, 29, 30, 31, 32, 33, 34, 35,
        36, 37, 38, 39, 40, 41, 42, 43, 44,
        45, 46, 47, 48, 49, 50, 51, 52, 53,
        54, 55, 56, 57, 58, 59, 60, 61, 62,
        63, 64, 65, 66, 67, 68, 69, 70, 71,
        72, 73, 74, 75, 76, 77, 78, 79, 80
    };
    private int[,] square_data = new int[9, 9]
    {
        {  0,  1,  2,   9, 10, 11,  18, 19, 20 },
        {  3,  4,  5,  12, 13, 14,  21, 22, 23 },
        {  6,  7,  8,  15, 16, 17,  24, 25, 26 },

        { 27, 28, 29,  36, 37, 38,  45, 46, 47 },
        { 30, 31, 32,  39, 40, 41,  48, 49, 50 },
        { 33, 34, 35,  42, 43, 44,  51, 52, 53 },

        { 54, 55, 56,  63, 64, 65,  72, 73, 74 },
        { 57, 58, 59,  66, 67, 68,  75, 76, 77 },
        { 60, 61, 62,  69, 70, 71,  78, 79, 80 }
    };
    #endregion

    private LinePosition GetSquarePosition(int square_index)
    {
        if (square_index < 0 || square_index > 80)
        {
            return new LinePosition(-1, -1);
        }

        int rowPos = -1;
        int colPos = -1;
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (line_data[row, col] == square_index)
                {
                    rowPos = row;
                    colPos = col;
                    return new LinePosition(rowPos, colPos);
                }
            }
        }
        return new LinePosition(-1, -1);
    }
    public int[] GetHorizontalLine(int square_index)
    {
        int[] line = new int[9];
        var squarePos = GetSquarePosition(square_index);

        if (squarePos.row < 0 || squarePos.row >= 9 || squarePos.col < 0 || squarePos.col >= 9)
        {
            return line;
        }

        for (int index = 0; index < 9; index++)
        {
            line[index] = line_data[squarePos.row, index];
        }
        return line;
    }
    public int[] GetVerticallLine(int square_index)
    {
        int[] line = new int[9];
        var squarePos = GetSquarePosition(square_index);

        if (squarePos.row < 0 || squarePos.row >= 9 || squarePos.col < 0 || squarePos.col >= 9)
        {
            return line;
        }

        for (int index = 0; index < 9; index++)
        {
            line[index] = line_data[index, squarePos.col];
        }
        return line;
    }
    public int[] GetSquare(int square_index)
    {
        int[] line = new int[9];
        int pos_row = -1;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (square_data[row, col] == square_index)
                {
                    pos_row = row;
                    break;
                }
            }
            if (pos_row != -1) break;
        }

        if (pos_row < 0 || pos_row >= 9)
        {
            return line;
        }

        for (int index = 0; index < 9; index++)
        {
            line[index] = square_data[pos_row, index];
        }
        return line;
    }
    public int[] GetAllSquareIndex()
    {
        return line_data_falt;
    }
}