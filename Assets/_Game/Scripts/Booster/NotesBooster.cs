using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotesBooster : Selectable, IPointerClickHandler
{
    [SerializeField] private List<Sprite> lstNotesImg; // 0 is off, 1 is on
    [SerializeField] private Image imgNotes;
    private bool isActive;
    void Start()
    {
        isActive = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isActive = !isActive;
        if (isActive)
        {
            imgNotes.sprite = lstNotesImg[1];
        }
        else
        {
            imgNotes.sprite = lstNotesImg[0];
        }
        AudioController.Instance.PlayClickSound();
        EventManager.NotesActive(isActive);
    }
}
