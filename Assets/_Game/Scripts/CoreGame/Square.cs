using UnityEngine;
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
    public void SetHasDefaultValue(bool isHasDefault) { hasDefaultValue = isHasDefault; }
    public bool GetHasDefaultValue() { return hasDefaultValue; }
    public bool HasWrongValue() { return hasWrongValue; }
    public bool IsSelected() { return isSelected; }
    public void SetSquareIndex(int index) { square_index = index; }
    public void SetCorrectNumber(int number)
    {
        correctNumber = number;
        hasWrongValue = false;
    }

    #region Init Data
    private void OnEnable()
    {
        EventManager.OnUpdateNumber += OnSetNumber;
        EventManager.OnSquareSelected += OnSquareSelected;
        EventManager.OnNotesActive += OnNotesActive;
    }
    private void OnDisable()
    {
        EventManager.OnUpdateNumber -= OnSetNumber;
        EventManager.OnSquareSelected -= OnSquareSelected;
        EventManager.OnNotesActive -= OnNotesActive;
    }
    void Start()
    {
        isSelected = false;
        isNoteActive = false;
        SetNotesNumberValue(0);
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
    }
    public void OnSubmit(BaseEventData eventData)
    {

    }
    public void OnSetNumber(int number)
    {
        if (isSelected && !hasDefaultValue && !hasWrongValue)
        {
            if (isNoteActive && !hasWrongValue)
            {
                SetNoteSingleNumberValue(number);
            }
            else if (!isNoteActive)
            {
                SetNotesNumberValue(0);
                SetNumber(number);
                SetTextColor(Color.blue);
                if (number != correctNumber)
                {
                    hasWrongValue = true;
                    SetColor(Color.red);
                    EventManager.SelectWrongNumber();
                }
                else
                {
                    SetColor(Color.white);
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
        else
        {
            Debug.LogWarning("lstSprSquare is null, empty, or imgSquare is null!");
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
}
