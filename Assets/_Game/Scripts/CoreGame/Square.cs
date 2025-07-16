using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class Square : Selectable
{
    [SerializeField] private GameObject textNumber;
    private int number = 0;
    void Update()
    {

    }
    public void DisplayText()
    {
        if (number < 0)
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
}
