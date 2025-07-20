using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class Square : Selectable, IPointerClickHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Text textNumber;
    [SerializeField] private Image imgSquare;
    [SerializeField] private List<Sprite> lstSprSquare;
    private int number = 0;
    private int correctNumber = 0;

    private bool isSelected = false;
    private int square_index = -1;
    private bool hasDefaultValue = false;
    public void SetHasDefaultValue(bool isHasDefault) { hasDefaultValue = isHasDefault; }
    public bool GetHasDefaultValue() { return hasDefaultValue; }
    public bool IsSelected() { return isSelected; }
    public void SetSquareIndex(int index) { square_index = index; }
    public void SetCorrectNumber(int number) { correctNumber = number; }
    void Start()
    {
        isSelected = false;
    }
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
    private void OnEnable()
    {
        EventManager.OnUpdateNumber += OnSetNumber;
        EventManager.OnSquareSelected += OnSquareSelected;
    }
    private void OnDisable()
    {
        EventManager.OnUpdateNumber -= OnSetNumber;
        EventManager.OnSquareSelected -= OnSquareSelected;

    }
    public void OnSetNumber(int number)
    {
        if (isSelected && hasDefaultValue == false)
        {
            SetNumber(number);
            SetTextColor(Color.blue);
            if (number != correctNumber)
            {
                var color = this.colors;
                color.normalColor = Color.red;
                this.colors = color;

                EventManager.SelectWrongNumber();
            }
            else
            {
                var color = this.colors;
                color.normalColor = Color.white;
                this.colors = color;
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
}
