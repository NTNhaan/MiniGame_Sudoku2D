using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class Square : Selectable, IPointerClickHandler, ISubmitHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject textNumber;
    private int number = 0;
    private bool isSelected = false;
    private int square_index = -1;
    public bool IsSelected()
    {
        return isSelected;
    }
    public void SetSquareIndex(int index)
    {
        square_index = index;
    }
    void Start()
    {
        isSelected = false;
    }
    public void DisplayText()
    {
        if (number <= 0)
        {
            textNumber.GetComponent<Text>().text = " ";
        }
        else
        {
            textNumber.GetComponent<Text>().text = number.ToString();
        }
    }
    public void SetNumber(int number)
    {
        this.number = number;
        DisplayText();
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
    private void Oisable()
    {
        EventManager.OnUpdateNumber -= OnSetNumber;
        EventManager.OnSquareSelected -= OnSquareSelected;

    }
    public void OnSetNumber(int number)
    {
        if (isSelected)
        {
            SetNumber(number);
        }
    }
    public void OnSquareSelected(int square_index)
    {
        if (this.square_index != square_index)
        {
            isSelected = false;
        }
    }
}
