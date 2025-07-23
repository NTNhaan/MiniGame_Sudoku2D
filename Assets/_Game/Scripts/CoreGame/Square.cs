using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class Square : Selectable, IPointerClickHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text textNumber;
    [SerializeField] private Image imgSquare;
    [SerializeField] private List<Sprite> lstSprSquare;
    [SerializeField] private List<Text> lstnotesNumber;
    private bool isNoteActive;
    private int number = 0;
    private int correctNumber = 0;

    private bool isSelected = false;
    private int square_index = -1;
    private bool hasDefaultValue = false;
    private bool hasWrongValue = false;

    private int previousNumber = 0;
    private bool previousHasWrongValue = false;
    private Color previousTextColor = Color.black;
    private Color previousBackgroundColor = Color.white;
    public void SetHasDefaultValue(bool isHasDefault) { hasDefaultValue = isHasDefault; }
    public bool GetHasDefaultValue() { return hasDefaultValue; }
    public bool HasWrongValue() { return hasWrongValue; }
    public bool IsSelected() { return isSelected; }
    public int GetNumber() { return number; }
    public int GetCorrectNumber() { return correctNumber; }
    public void SetSquareIndex(int index) { square_index = index; }
    public int GetSquareIndex() { return square_index; }

    #region Geter cho undo number
    public int GetPreviousNumber() { return previousNumber; }
    public bool GetPreviousHasWrongValue() { return previousHasWrongValue; }
    public Color GetPreviousTextColor() { return previousTextColor; }
    public Color GetPreviousBackgroundColor() { return previousBackgroundColor; }
    #endregion
    public void SetCorrectNumber(int number)
    {
        correctNumber = number;
        hasWrongValue = false;
    }

    #region Init Data
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnUpdateNumber += OnSetNumber;
        EventManager.OnSquareSelected += OnSquareSelected;
        EventManager.OnNotesActive += OnNotesActive;
        EventManager.OnClearNumber += OnClearNumber;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.OnUpdateNumber -= OnSetNumber;
        EventManager.OnSquareSelected -= OnSquareSelected;
        EventManager.OnNotesActive -= OnNotesActive;
        EventManager.OnClearNumber -= OnClearNumber;
    }
    protected override void Start()
    {
        base.Start();
        isSelected = false;
        isNoteActive = false;
        SetNotesNumberValue(0);

        previousNumber = 0;
        previousHasWrongValue = false;
        previousTextColor = Color.black;
        previousBackgroundColor = Color.white;
    }
    #endregion

    public void DisplayText()
    {
        if (number <= 0)
        {
            textNumber.text = " ";
        }
        else
        {
            textNumber.text = number.ToString();
        }
    }
    public void SetNumber(int number)
    {
        this.number = number;
        DisplayText();
    }

    public void SetTextColor(Color color)
    {
        if (textNumber != null)
        {
            Text textComponent = textNumber;
            if (textComponent != null)
            {
                textComponent.color = color;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = true;
        EventManager.SquareSeleced(square_index);
        AudioController.Instance.PlayClickSound();

        // Debug feature: Show correct number when square is selected
        if (GameManager.instance != null && GameManager.instance.ShowCorrectNumberDebug)
        {
            string debugInfo = $"Square {square_index}: Current={number}, Correct={correctNumber}";
            if (hasDefaultValue)
            {
                debugInfo += " [DEFAULT]";
            }
            if (number == correctNumber && number != 0)
            {
                debugInfo += " ✓ CORRECT";
            }
            else if (number != 0 && number != correctNumber)
            {
                debugInfo += " ✗ WRONG";
            }
            else if (number == 0)
            {
                debugInfo += " [EMPTY]";
            }

            Debug.Log($"[DEBUG] {debugInfo}");
        }
    }
    public void OnSubmit(BaseEventData eventData)
    {

    }
    public void OnSetNumber(int number)
    {
        if (isSelected && !hasDefaultValue && !hasWrongValue)
        {
            SaveCurrentState();

            if (isNoteActive && !hasWrongValue)
            {
                SetNoteSingleNumberValue(number);
            }
            else if (!isNoteActive)
            {
                SetNotesNumberValue(0);
                SetNumber(number);
                SetTextColor(Color.blue);
                if (BoardController.Instance != null)
                {
                    BoardController.Instance.RegisterUndoState(this);
                }

                if (number != correctNumber)
                {
                    hasWrongValue = true;
                    SetColor(Color.red);
                    EventManager.SelectWrongNumber();
                    if (AudioController.Instance != null)
                    {
                        AudioController.Instance.PlayWrongNumberSound();
                    }
                }
                else
                {
                    hasWrongValue = false;
                    SetColor(Color.white);
                    if (AudioController.Instance != null)
                    {
                        AudioController.Instance.PlayRightNumberSound();
                    }

                    // Check if puzzle is completed after setting correct number
                    if (BoardController.Instance != null)
                    {
                        BoardController.Instance.CheckPuzzleComplete();
                    }
                }
            }
        }
    }
    public void OnSquareSelected(int square_index)
    {
        if (this.square_index != square_index)
        {
            isSelected = false;
        }
    }
    public void SetColor(Color color)
    {
        var curColor = this.colors;
        curColor.normalColor = color;
        this.colors = curColor;
    }
    public void ChangeSpriteSquare()
    {
        if (lstSprSquare != null && lstSprSquare.Count != 0 && imgSquare != null)
        {
            int randomIndex = Random.Range(0, lstSprSquare.Count);
            imgSquare.sprite = lstSprSquare[randomIndex];
        }
    }

    #region Notes Booster Region
    public List<string> GetSquareNotes()
    {
        List<string> notes = new List<string>();
        foreach (Text number in lstnotesNumber)
        {
            notes.Add(number.text);
        }
        return notes;
    }
    private void SetClearEmptyNotes()
    {
        foreach (Text number in lstnotesNumber)
        {
            if (number.text == "0")
            {
                number.text = " ";
            }
        }
    }
    private void SetNotesNumberValue(int value)
    {
        foreach (Text number in lstnotesNumber)
        {
            if (value <= 0)
            {
                number.text = " ";
            }
            else
            {
                number.text = value.ToString();
            }
        }
    }
    private void SetNoteSingleNumberValue(int value, bool force_update = false)
    {
        if (!isNoteActive && !force_update)
        {
            return;
        }
        if (value <= 0)
        {
            lstnotesNumber[value - 1].text = " ";
        }
        else
        {
            if (lstnotesNumber[value - 1].text == " " || force_update)
            {
                lstnotesNumber[value - 1].text = value.ToString();
            }
            else
            {
                lstnotesNumber[value - 1].text = " ";
            }
        }
    }
    public void SetGridNotes(List<int> notes)
    {
        foreach (var note in notes)
        {
            SetNoteSingleNumberValue(note, true);
        }
    }
    public void OnNotesActive(bool active)
    {
        isNoteActive = active;
    }
    #endregion

    #region Erase Booster Region
    public void OnClearNumber()
    {
        Debug.Log($"Square {square_index}: OnClearNumber attempt - isSelected: {isSelected}, hasDefaultValue: {hasDefaultValue}, number: {number}, correctNumber: {correctNumber}");

        // Kiểm tra square có được select và không phải default value
        if (!isSelected)
        {
            Debug.Log($"Square {square_index}: Cannot clear - not selected");
            return;
        }

        if (hasDefaultValue)
        {
            Debug.Log($"Square {square_index}: Cannot clear - is default value");

            // Visual feedback - flash red briefly để báo không thể xóa
            StartCoroutine(FlashCannotEraseEffect());
            return;
        }

        // Check if player has enough coins for erase
        if (!GameManager.instance.CanAfford(GameManager.instance.GetEraseCost()))
        {
            Debug.Log($"Square {square_index}: Not enough coins for erase!");
            // You can add UI notification here
            return;
        }

        // Spend coins for erase
        if (!GameManager.instance.SpendCoins(GameManager.instance.GetEraseCost()))
        {
            Debug.Log($"Square {square_index}: Failed to spend coins for erase");
            return;
        }

        // Additional check: Nếu number = correctNumber và có số != 0, 
        // có thể đây là original default mà flag bị sai
        if (number != 0 && number == correctNumber)
        {
            Debug.LogWarning($"Square {square_index}: WARNING - This might be a default number! number: {number}, correct: {correctNumber}");
            // Optional: có thể block luôn hoặc cảnh báo
        }

        // Lưu trạng thái trước khi clear
        SaveCurrentState();

        // Đăng ký với BoardController trước khi clear
        if (BoardController.Instance != null)
        {
            BoardController.Instance.RegisterUndoState(this);
        }

        // Clear number
        number = 0;
        hasWrongValue = false;
        isSelected = false;
        SetColor(Color.white);
        SetTextColor(Color.black);
        SetNotesNumberValue(0);
        DisplayText();

        Debug.Log($"Square {square_index}: Number cleared successfully");

        // Play erase sound
        if (AudioController.Instance != null)
        {
            AudioController.Instance.PlayEraseSound();
        }
    }
    #endregion

    #region Undo Booster Region
    private void SaveCurrentState()
    {
        previousNumber = number;
        previousHasWrongValue = hasWrongValue;
        previousTextColor = textNumber.color;
        previousBackgroundColor = colors.normalColor;
    }

    /// <summary>
    /// Undo action được gọi từ UndoManager, không cần select square
    /// </summary>
    public void UndoLastAction()
    {
        // Không cần kiểm tra isSelected, undo được gọi từ UndoManager
        if (!hasDefaultValue)
        {
            number = previousNumber;
            hasWrongValue = previousHasWrongValue;

            SetTextColor(previousTextColor);
            SetColor(previousBackgroundColor);
            DisplayText();

            if (number <= 0)
            {
                SetNotesNumberValue(0);
            }

            Debug.Log($"Undo: Square {square_index} restored to number {previousNumber}");

            // Play undo sound
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayUndoSound();
            }
        }
    }

    /// <summary>
    /// Restore square từ undo state - được gọi từ BoardController
    /// </summary>
    public void RestoreState(int restoreNumber, bool restoreWrongValue, Color restoreTextColor, Color restoreBackgroundColor)
    {
        if (!hasDefaultValue)
        {
            number = restoreNumber;
            hasWrongValue = restoreWrongValue;

            SetTextColor(restoreTextColor);
            SetColor(restoreBackgroundColor);
            DisplayText();

            if (number <= 0)
            {
                SetNotesNumberValue(0);
            }

            Debug.Log($"Square {square_index}: Restored to number {restoreNumber}");
        }
    }
    public void OnUndoNumber()
    {
        if (isSelected && !hasDefaultValue)
        {
            number = previousNumber;
            hasWrongValue = previousHasWrongValue;

            SetTextColor(previousTextColor);
            SetColor(previousBackgroundColor);
            DisplayText();

            if (number <= 0)
            {
                SetNotesNumberValue(0);
            }

            Debug.Log($"Undo: Square {square_index} restored to number {previousNumber}");
        }
    }

    /// <summary>
    /// Coroutine để flash màu đỏ khi không thể erase default value
    /// </summary>
    private System.Collections.IEnumerator FlashCannotEraseEffect()
    {
        Color originalColor = colors.normalColor;

        // Flash red
        SetColor(Color.red);
        yield return new WaitForSeconds(0.1f);

        // Back to original
        SetColor(originalColor);
        yield return new WaitForSeconds(0.1f);

        // Flash red again
        SetColor(Color.red);
        yield return new WaitForSeconds(0.1f);

        // Back to original
        SetColor(originalColor);
    }

    /// <summary>
    /// Validate xem default value flag có consistent không
    /// </summary>
    public bool ValidateDefaultValue()
    {
        // Nếu được mark là default, phải là số từ board data gốc
        // (không thể validate vì không có access vào original unsolved_data ở đây)
        // Chỉ có thể kiểm tra logic cơ bản

        if (hasDefaultValue && number == 0)
        {
            Debug.LogError($"Square {square_index}: INVALID - marked as default but number is 0");
            return false;
        }

        return true;
    }
    #endregion
}
