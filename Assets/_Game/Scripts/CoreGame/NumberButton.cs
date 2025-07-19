using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEditor.Events;
using UnityEditor.EventSystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class NumberButton : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private int value = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager.UpdateSquareNumber(value);
    }
    public void OnSubmit(BaseEventData eventData)
    {

    }
}