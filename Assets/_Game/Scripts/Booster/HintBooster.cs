using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HintBooster : Selectable, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioController.Instance.PlayClickSound();
        EventManager.HintNumber();
    }
}
