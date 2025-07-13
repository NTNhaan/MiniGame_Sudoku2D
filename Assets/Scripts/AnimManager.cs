using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour
{
    // [SerializeField] private Animator animator;
    // private static readonly int TakeHash = Animator.StringToHash("Take");
    //
    //
    // public void PlayTakeAnim()
    // {
    //     if (!animator || animator.HasState(0, TakeHash))
    //         return;
    //     animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    //     animator.CrossFade(TakeHash, 0.1f);
    //
    //     StartCoroutine(WaitForAnimToEnd());
    // }
    //
    // private System.Collections.IEnumerator WaitForAnimToEnd()
    // {
    //     yield return null;
    //     
    //     float animationLenght = animator.GetCurrentAnimatorStateInfo(0).length;
    //     if (animationLenght > 0)
    //     {
    //         yield return new WaitForSecondsRealtime(animationLenght);
    //     }
    //     DetectivateObject();
    // }
    // private void DetectivateObject()
    // {
    //     transform.parent.gameObject.SetActive(false);
    // }
}
