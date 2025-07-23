using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EraseBooster : Selectable, IPointerClickHandler
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioController.Instance.PlayClickSound();
        EventManager.ClearNumber();
    }
}
