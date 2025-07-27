using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EraseBooster : Selectable, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioController.Instance.PlayClickSound();
        EventManager.ClearNumber();
    }
}
