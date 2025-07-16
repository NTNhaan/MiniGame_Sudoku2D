using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [SerializeField] private int columns = 0;
    [SerializeField] private int rows = 0;
    [SerializeField] private GameObject square;
    [SerializeField] private float square_offset = 2.5f;
    [SerializeField] private float square_scale = 1.0f;
    [SerializeField] private Vector2 startPos;
    private List<GameObject> lstSquare;

    void Start()
    {
        lstSquare = new List<GameObject>();
        CreateBoardGame();
        SetBoardNumber();
    }
    private void CreateBoardGame()
    {
        SpawnSquare();
        SetSquarePosition();
    }
    private void SpawnSquare()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                lstSquare.Add(Instantiate(square));
                lstSquare[lstSquare.Count - 1].transform.parent = this.transform;
                lstSquare[lstSquare.Count - 1].transform.localScale = new Vector3(square_scale, square_scale, square_scale);
            }
        }
    }
    private void SetSquarePosition()
    {
        RectTransform squareRect = (RectTransform)lstSquare[0].transform;
        Vector2 offset = new Vector2();
        offset.x = squareRect.rect.width * squareRect.transform.localScale.x + square_offset;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y + square_offset;

        int column_number = 0;
        int row_number = 0;
        foreach (GameObject square in lstSquare)
        {
            if (column_number + 1 > columns)
            {
                row_number++;
                column_number = 0;
            }
            var posOffsetX = offset.x * column_number;
            var posOffsetY = offset.y * row_number;
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPos.x + posOffsetX, startPos.y - posOffsetY);
            column_number++;
        }

    }
    private void SetBoardNumber()
    {
        foreach (var square in lstSquare)
        {
            square.GetComponent<Square>().SetNumber(Random.Range(0, 10));
        }
    }
}
