using System.Collections.Generic;
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
    private int remainingHints = 5;
    private const int MAX_HINTS_PER_LEVEL = 5;

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

        remainingHints = MAX_HINTS_PER_LEVEL;
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

            bool isDefault = data.unsolved_data[i] != 0 && data.unsolved_data[i] == data.solved_data[i];
            lstSquareComponents[i].SetHasDefaultValue(isDefault);

            if (isDefault)
            {
                Debug.Log($"Square {i}: Set as DEFAULT - unsolved: {data.unsolved_data[i]}, solved: {data.solved_data[i]}");
            }
        }
    }

    private const int HINT_COST = 100;

    private void OnHintNumber()
    {
        if (currentBoardData == null) return;

        if (remainingHints <= 0)
        {
            if (!GameManager.instance.CanAfford(GameManager.instance.GetHintCost()))
            {
                EventManager.ShowNotEnoughCoinsMessage(HINT_COST);
                return;
            }
            if (!GameManager.instance.SpendCoins(GameManager.instance.GetHintCost()))
            {
                return;
            }
        }

        var availableSquares = new List<int>();

        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            var square = lstSquareComponents[i];
            if (!square.GetHasDefaultValue() &&
                (square.GetNumber() == 0 || square.HasWrongValue() || square.GetNumber() != currentBoardData.solved_data[i]))
            {
                availableSquares.Add(i);
            }
        }

        if (availableSquares.Count == 0)
        {
            Debug.Log("No squares available for hint");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableSquares.Count);
        int selectedIndex = availableSquares[randomIndex];
        var selectedSquare = lstSquareComponents[selectedIndex];

        int correctNumber = currentBoardData.solved_data[selectedIndex];

        if (selectedSquare.GetNumber() != 0)
        {
            RegisterUndoState(selectedSquare);
        }

        selectedSquare.SetNumber(correctNumber);
        selectedSquare.SetHasDefaultValue(true);
        selectedSquare.SetTextColor(Color.blue);

        var horizontalLine = LineIndicator.Instance.GetHorizontalLine(selectedIndex);
        var verticalLine = LineIndicator.Instance.GetVerticallLine(selectedIndex);
        var squareGroup = LineIndicator.Instance.GetSquare(selectedIndex);

        SetSquaresColor(LineIndicator.Instance.GetAllSquareIndex(), Color.white);

        SetSquaresColor(horizontalLine, hightLightColor);
        SetSquaresColor(verticalLine, hightLightColor);
        SetSquaresColor(squareGroup, hightLightColor);

        selectedSquare.SetColor(hightLightColor);

        Invoke("ClearHintHighlight", 1.5f);

        remainingHints--;

        EventManager.HintCountChanged();

        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayRightNumberSound();
        }
        CheckPuzzleComplete();
    }
    private void ClearHintHighlight()
    {
        EventManager.SquareSeleced(-1);
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
        if (square_index == -1)
        {
            SetSquaresColor(LineIndicator.Instance.GetAllSquareIndex(), Color.white);
            return;
        }

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

        int coinReward = 50;
        GameManager.instance.AddCoins(coinReward);
        Debug.Log($"Level completed! Earned {coinReward} coins");

        string levelInfo = GetCurrentLevelInfo();
        EventManager.LevelCompleted(levelInfo);
        AudioController.Instance.PlayWinSound();

        // bool hasNextLevel = GameConfigSetting.Instance.AdvanceToNextLevel();
        // if (hasNextLevel)
        // {
        //     Invoke("LoadNextLevel", 3f);
        // }
        // else
        // {
        //     OnGameComplete();
        // }
    }

    private string GetCurrentLevelInfo()
    {
        if (GameConfigSetting.Instance != null)
        {
            string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
            int currentSubLevel = GameConfigSetting.Instance.GetCurrentSubLevel();

            int totalLevel = CalculateTotalLevelFromProgression(currentDifficulty, currentSubLevel);

            var allLevels = FixedSudokuLevelData.GetAllLevels();
            if (totalLevel > 0 && totalLevel <= allLevels.Count)
            {
                var levelData = allLevels[totalLevel - 1];
                return $"Level {levelData.levelId} - {levelData.difficulty} Completed!";
            }
            else
            {
                return $"{currentDifficulty} Level Completed!";
            }
        }

        return "Level Completed!";
    }

    private int CalculateTotalLevelFromProgression(string difficulty, int subLevel)
    {
        int baseLevel = 0;

        switch (difficulty)
        {
            case "Easy":
                baseLevel = 0;
                break;
            case "Medium":
                baseLevel = GetTotalSubLevelsForDifficulty("Easy");
                break;
            case "Hard":
                baseLevel = GetTotalSubLevelsForDifficulty("Easy") + GetTotalSubLevelsForDifficulty("Medium");
                break;
            case "Impossible":
                baseLevel = GetTotalSubLevelsForDifficulty("Easy") + GetTotalSubLevelsForDifficulty("Medium") + GetTotalSubLevelsForDifficulty("Hard");
                break;
        }

        return baseLevel + subLevel + 1;
    }

    private int GetTotalSubLevelsForDifficulty(string difficulty)
    {
        if (LevelData.Instance != null && LevelData.Instance.gameDir.ContainsKey(difficulty))
        {
            return LevelData.Instance.gameDir[difficulty].Count;
        }
        return 2;
    }

    private void LoadNextLevel()
    {
        string nextLevel = GameConfigSetting.Instance.GetCurrentDifficulty();
        SetBoardNumber(nextLevel);

        ResetBoardState();
    }
    public void LoadLevel(string difficulty)
    {
        SetBoardNumber(difficulty);
        ResetBoardState();
        Debug.Log($"Loaded level: {difficulty}");
    }

    private void ResetBoardState()
    {
        SetSquaresColor(LineIndicator.Instance.GetAllSquareIndex(), Color.white);

        foreach (var square in lstSquareComponents)
        {
            square.SetColor(Color.white);
        }
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
            if (!GameManager.instance.CanAfford(GameManager.instance.GetUndoCost()))
            {
                Debug.Log("Not enough coins for undo!");
                return;
            }

            if (GameManager.instance.SpendCoins(GameManager.instance.GetUndoCost()))
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
    }
    public bool CanUndo()
    {
        return undoStack.Count > 0;
    }
    public int GetUndoCount()
    {
        return undoStack.Count;
    }

    public int GetHintCount()
    {
        return remainingHints;
    }
    public void ClearUndoHistory()
    {
        undoStack.Clear();
        EventManager.UndoCountChanged();
    }
    #endregion

    #region Hint Management
    public int GetRemainingHints()
    {
        return remainingHints;
    }

    public bool CanUseHint()
    {
        return remainingHints > 0;
    }

    // public void DebugShowDefaultSquares()
    // {
    //     for (int i = 0; i < lstSquareComponents.Count; i++)
    //     {
    //         var square = lstSquareComponents[i];
    //         if (square.GetHasDefaultValue())
    //         {
    //             Debug.Log($"Square {i}: DEFAULT - Number: {square.GetNumber()}, Correct: {square.GetCorrectNumber()}");
    //         }
    //     }
    //     Debug.Log("=== END DEBUG ===");
    // }

    public List<Square> GetSquareComponents()
    {
        return lstSquareComponents;
    }

    public void CheckPuzzleComplete()
    {
        if (currentBoardData == null) return;

        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            var square = lstSquareComponents[i];
            int currentNumber = square.GetNumber();
            int correctNumber = currentBoardData.solved_data[i];
            if (currentNumber == 0 || currentNumber != correctNumber)
            {
                return;
            }
        }
        EventManager.PuzzleComplete();
    }
    [ContextMenu("Toggle Debug Mode")]
    public void ToggleDebugMode()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ToggleDebugMode();
        }
    }
    [ContextMenu("Check Puzzle Complete")]
    public void DebugCheckPuzzleComplete()
    {
        CheckPuzzleComplete();
    }

    [ContextMenu("Show Puzzle Status")]
    public void DebugShowPuzzleStatus()
    {
        if (currentBoardData == null)
        {
            Debug.Log("No board data available");
            return;
        }

        int correctCount = 0;
        int totalSquares = lstSquareComponents.Count;

        Debug.Log("=== PUZZLE STATUS ===");
        for (int i = 0; i < lstSquareComponents.Count; i++)
        {
            var square = lstSquareComponents[i];
            int currentNumber = square.GetNumber();
            int correctNumber = currentBoardData.solved_data[i];
            bool isCorrect = currentNumber != 0 && currentNumber == correctNumber;

            if (isCorrect) correctCount++;
        }
    }
    #endregion
}
