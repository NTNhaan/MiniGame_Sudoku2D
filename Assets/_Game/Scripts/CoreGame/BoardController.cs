using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [SerializeField] private int columns = 0;
    [SerializeField] private int rows = 0;
    [SerializeField] private GameObject square;
    [SerializeField] private float square_offset = 2.5f;
    [SerializeField] private float square_scale = 1.0f;
    [SerializeField] private float square_gap = 0.1f;
    [SerializeField] private Vector2 startPos;
    private List<GameObject> lstSquare;
    private List<Square> lstSquareComponents; // Cache Square components
    private int selected_data = -1;

    void Start()
    {
        lstSquare = new List<GameObject>();
        lstSquareComponents = new List<Square>();
        CreateBoardGame();
        SetBoardNumber(GameConfigSetting.Instance.GetGameMode());
    }
    private void CreateBoardGame()
    {
        SpawnSquare();
        SetSquarePosition();
    }
    private void SpawnSquare()
    {
        int square_index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject newSquare = Instantiate(square);
                lstSquare.Add(newSquare);
                newSquare.transform.parent = this.transform;
                newSquare.transform.localScale = new Vector3(square_scale, square_scale, square_scale);

                Square squareComponent = newSquare.GetComponent<Square>();
                lstSquareComponents.Add(squareComponent);

                squareComponent.SetSquareIndex(square_index);
                squareComponent.ChangeSpriteSquare();
                square_index++;
            }
        }
    }
    private void SetSquarePosition()
    {
        RectTransform squareRect = (RectTransform)lstSquare[0].transform;
        Vector2 offset = new Vector2();
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool rowMoved = false;

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
                square_gap_number.x = 0;
                rowMoved = false;
            }
            var posOffsetX = offset.x * column_number + (square_gap_number.x * square_gap);
            var posOffsetY = offset.y * row_number + (square_gap_number.y * square_gap);
            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                posOffsetX += square_gap;
            }
            if (row_number > 0 && row_number % 3 == 0 && !rowMoved)
            {
                rowMoved = true;
                square_gap_number.y++;
                posOffsetY += square_gap;
            }
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPos.x + posOffsetX, startPos.y - posOffsetY);
            column_number++;
        }

    }
    private void SetBoardNumber(string level)
    {
        selected_data = Random.Range(0, LevelData.Instance.gameDir[level].Count);
        var data = LevelData.Instance.gameDir[level][selected_data];

        SetBoardSquareData(data);
    }

    private void SetBoardSquareData(BoardData data)
    {
        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            lstSquareComponents[i].SetNumber(data.unsolved_data[i]);
            lstSquareComponents[i].SetCorrectNumber(data.solved_data[i]);
            lstSquareComponents[i].SetHasDefaultValue(data.unsolved_data[i] != 0 && data.unsolved_data[i] == data.solved_data[i]);
        }
    }
}
