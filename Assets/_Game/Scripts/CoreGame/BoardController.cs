using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class BoardController : Singleton<BoardController>
{
    [SerializeField] private int columns = 0;
    [SerializeField] private int rows = 0;
    [SerializeField] private GameObject square;
    [SerializeField] private float square_offset = 2.5f;
    [SerializeField] private float square_scale = 1.0f;
    [SerializeField] private float square_gap = 0.1f;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Color hightLightColor;
    private List<GameObject> lstSquare;
    private List<Square> lstSquareComponents;
    private int selected_data = -1;
    private BoardData currentBoardData;
    public Color GetHightColoir => hightLightColor;

    private Stack<UndoState> undoStack = new Stack<UndoState>();
    private const int MAX_UNDO_STEPS = 10;

    // Hint Management
    private int remainingHints = 3; // Số hint có thể sử dụng mỗi level
    private const int MAX_HINTS_PER_LEVEL = 3;

    #region Init Data
    private void OnEnable()
    {
        EventManager.OnSquareSelected += OnSquareSelected;
        EventManager.OnHintNumber += OnHintNumber;
        EventManager.OnPuzzleComplete += OnLevelComplete;
    }
    private void OnDisable()
    {
        EventManager.OnSquareSelected -= OnSquareSelected;
        EventManager.OnHintNumber -= OnHintNumber;
        EventManager.OnPuzzleComplete -= OnLevelComplete;
    }
    void Start()
    {
        lstSquare = new List<GameObject>();
        lstSquareComponents = new List<Square>();
        CreateBoardGame();
        SetBoardNumber(GameConfigSetting.Instance.GetGameMode());
    }
    #endregion
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
        ClearUndoHistory();

        // Reset hint counter
        remainingHints = MAX_HINTS_PER_LEVEL;

        // Trigger UI update
        EventManager.HintCountChanged();

        int subLevelIndex = GameConfigSetting.Instance.GetCurrentSubLevel();

        if (LevelData.Instance.gameDir.ContainsKey(level) &&
            subLevelIndex < LevelData.Instance.gameDir[level].Count)
        {
            selected_data = subLevelIndex;
            var data = LevelData.Instance.gameDir[level][selected_data];
            SetBoardSquareData(data);
            EventManager.LevelChanged();
        }
        else
        {
            selected_data = Random.Range(0, LevelData.Instance.gameDir[level].Count);
            var data = LevelData.Instance.gameDir[level][selected_data];
            SetBoardSquareData(data);
        }
    }

    private void SetBoardSquareData(BoardData data)
    {
        currentBoardData = data;
        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            lstSquareComponents[i].SetNumber(data.unsolved_data[i]);
            lstSquareComponents[i].SetCorrectNumber(data.solved_data[i]);

            // Kiểm tra và set default value
            bool isDefault = data.unsolved_data[i] != 0 && data.unsolved_data[i] == data.solved_data[i];
            lstSquareComponents[i].SetHasDefaultValue(isDefault);

            // Debug log để kiểm tra default values
            if (isDefault)
            {
                Debug.Log($"Square {i}: Set as DEFAULT - unsolved: {data.unsolved_data[i]}, solved: {data.solved_data[i]}");
            }
        }
    }

    private void OnHintNumber()
    {
        if (currentBoardData == null) return;

        // Kiểm tra còn hint không
        if (remainingHints <= 0)
        {
            Debug.Log("No hints remaining for this level");
            return;
        }

        // Tạo danh sách tất cả squares có thể hint (không phải default và chưa đúng)
        var availableSquares = new List<int>();

        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            var square = lstSquareComponents[i];
            // Chỉ thêm vào list nếu:
            // 1. Không phải default value
            // 2. Số hiện tại bằng 0 (trống) hoặc sai
            if (!square.GetHasDefaultValue() &&
                (square.GetNumber() == 0 || square.HasWrongValue() || square.GetNumber() != currentBoardData.solved_data[i]))
            {
                availableSquares.Add(i);
            }
        }

        // Nếu không có square nào có thể hint
        if (availableSquares.Count == 0)
        {
            Debug.Log("No squares available for hint");
            return;
        }

        // Random chọn một square từ danh sách available
        int randomIndex = UnityEngine.Random.Range(0, availableSquares.Count);
        int selectedIndex = availableSquares[randomIndex];
        var selectedSquare = lstSquareComponents[selectedIndex];

        // Lấy số đúng từ solved_data
        int correctNumber = currentBoardData.solved_data[selectedIndex];

        // Lưu undo state trước khi hint (nếu square hiện tại không trống)
        if (selectedSquare.GetNumber() != 0)
        {
            RegisterUndoState(selectedSquare);
        }

        // Set số đúng cho square đó
        selectedSquare.SetNumber(correctNumber);
        selectedSquare.SetHasDefaultValue(true); // Mark as default để không thể edit
        selectedSquare.SetTextColor(Color.green); // Màu xanh để phân biệt với default ban đầu

        var horizontalLine = LineIndicator.Instance.GetHorizontalLine(selectedIndex);
        var verticalLine = LineIndicator.Instance.GetVerticallLine(selectedIndex);
        var squareGroup = LineIndicator.Instance.GetSquare(selectedIndex);
        SetSquaresColor(LineIndicator.Instance.GetAllSquareIndex(), Color.white);
        SetSquaresColor(horizontalLine, hightLightColor);
        SetSquaresColor(verticalLine, hightLightColor);
        SetSquaresColor(squareGroup, hightLightColor);

        EventManager.SquareSeleced(-1);

        remainingHints--;

        EventManager.HintCountChanged();

        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayRightNumberSound();
        }
    }
    private void SetSquaresColor(int[] data, Color color)
    {
        foreach (var index in data)
        {
            var square = lstSquareComponents[index];
            // bool isWrongNonDefault = square.HasWrongValue() && !square.GetHasDefaultValue();
            if (!square.HasWrongValue() || square.GetHasDefaultValue())
            {
                square.SetColor(color);
            }
        }
    }
    public void OnSquareSelected(int square_index)
    {
        var horizontalLine = LineIndicator.Instance.GetHorizontalLine(square_index);
        var verticalLine = LineIndicator.Instance.GetVerticallLine(square_index);
        var square = LineIndicator.Instance.GetSquare(square_index);

        SetSquaresColor(LineIndicator.Instance.GetAllSquareIndex(), Color.white);

        SetSquaresColor(horizontalLine, hightLightColor);
        SetSquaresColor(verticalLine, hightLightColor);
        SetSquaresColor(square, hightLightColor);
    }

    #region Level Progression

    public void OnLevelComplete()
    {
        Debug.Log("Level Complete!");

        // Play win sound
        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayWinSound();
        }

        bool hasNextLevel = GameConfigSetting.Instance.AdvanceToNextLevel();

        if (hasNextLevel)
        {
            Invoke("LoadNextLevel", 2f);
        }
        else
        {
            OnGameComplete();
        }
    }

    private void LoadNextLevel()
    {
        string nextLevel = GameConfigSetting.Instance.GetCurrentDifficulty();
        SetBoardNumber(nextLevel);

        ResetBoardState();
    }

    private void ResetBoardState()
    {
        SetSquaresColor(LineIndicator.Instance.GetAllSquareIndex(), Color.white);

        foreach (var square in lstSquareComponents)
        {
            square.SetColor(Color.white);
        }
    }

    private void OnGameComplete()
    {
        Debug.Log("GAME COMPLETED! All levels finished!");

    }

    #endregion

    #region Booster Undo
    public void RegisterUndoState(Square square)
    {
        if (square != null && !square.GetHasDefaultValue())
        {
            int squareIndex = square.GetSquareIndex();
            var undoState = new UndoState(
                squareIndex,
                square.GetPreviousNumber(),
                square.GetPreviousHasWrongValue(),
                square.GetPreviousTextColor(),
                square.GetPreviousBackgroundColor()
            );

            undoStack.Push(undoState);
            if (undoStack.Count > MAX_UNDO_STEPS)
            {
                var tempArray = undoStack.ToArray();
                undoStack.Clear();
                for (int i = tempArray.Length - 2; i >= 0; i--)
                {
                    undoStack.Push(tempArray[i]);
                }
            }
            EventManager.UndoCountChanged();
        }
    }
    public void PerformUndo()
    {
        if (undoStack.Count > 0)
        {
            var undoState = undoStack.Pop();
            var square = lstSquareComponents[undoState.squareIndex];

            if (!square.GetHasDefaultValue())
            {
                square.RestoreState(
                    undoState.previousNumber,
                    undoState.previousHasWrongValue,
                    undoState.previousTextColor,
                    undoState.previousBackgroundColor
                );
                EventManager.UndoCountChanged();
                AudioController.Instance.PlayUndoSound();
            }
        }
    }
    public bool CanUndo()
    {
        return undoStack.Count > 0;
    }
    public int GetUndoCount()
    {
        return undoStack.Count;
    }
    public void ClearUndoHistory()
    {
        undoStack.Clear();
        Debug.Log("BoardController: Xoa du lieu stack undo");
        EventManager.UndoCountChanged();
    }
    #endregion

    #region Hint Management
    /// <summary>
    /// Lấy số hint còn lại
    /// </summary>
    public int GetRemainingHints()
    {
        return remainingHints;
    }

    /// <summary>
    /// Kiểm tra có thể sử dụng hint không
    /// </summary>
    public bool CanUseHint()
    {
        return remainingHints > 0;
    }

    /// <summary>
    /// Debug method - hiển thị tất cả default squares
    /// </summary>
    public void DebugShowDefaultSquares()
    {
        Debug.Log("=== DEFAULT SQUARES DEBUG ===");
        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            var square = lstSquareComponents[i];
            if (square.GetHasDefaultValue())
            {
                Debug.Log($"Square {i}: DEFAULT - Number: {square.GetNumber()}, Correct: {square.GetCorrectNumber()}");
            }
        }
        Debug.Log("=== END DEBUG ===");
    }
    #endregion
}
