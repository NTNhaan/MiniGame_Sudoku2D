using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Star : MonoBehaviour
{
    public static Star Instance;
    private bool isClicked = false;
    public bool isPressed = false;
    private bool isAnimEnd = false;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D collider;

    [SerializeField] public HammerCtrl HammerCtrl;
    [SerializeField] public bool IsAnimationEnd => isAnimEnd;
    [SerializeField] public bool isClickObject => isClicked;

    // public bool IsPressed()
    // {
    //     return isPressed;
    // }
    void Awake()
    {
        Instance = this;
        collider.enabled = false;
    }
    // public void Move(List<Star> list)
    // {
    //     if (list.Count != 0)
    //     {
    //         isPress = true;
    //         transform.DOLocalMoveY(0, 1.5f);
    //     }
    // }
    // public void Back()
    // {
    //     isPress = false;
    //     transform.DOLocalMoveY(-1, 1f);
    // }

    public void PlayAppearAnim()
    {
        collider.enabled = true;
        isClicked = false;
        StartCoroutine(WaitForAnimEnd("Appear"));
    }

    private void Update()
    {
        Debug.Log($"OnMouseDown {isClicked}");
    }

    public IEnumerator WaitForAnimEnd(string animName)
    {
        // isClicked = false;
        isAnimEnd = true;
        collider.enabled = true;
        animator.SetBool(animName, true);
        yield return null;
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        Debug.Log($"Animation finished! {animName}");
        animator.SetBool(animName, false);
        collider.enabled = false;
        isAnimEnd = false;
    }
    public void OnMouseDown()
    {
        isPressed = true;
        // Debug.Log($"Clicked on: {isPressed} {collider.enabled} {gameObject.tag}");

        if (isPressed && collider.enabled && gameObject.CompareTag("CheckPoint"))
        {
            isClicked = true;
            Debug.Log($"OnMouseDown {isClicked}");
            StartCoroutine(WaitForAnimEnd("Hit"));

            HammerCtrl.ShowHammerAt(transform.position);

            GameManager.instance.HandleAddPoints(1);
            isPressed = false;  // kiểm tra spam click

        }

        // transform.DOLocalMoveY(-1, 1f);

        // Debug.Log($"OnMouseDown {gameObject.name}" );
    }
}

