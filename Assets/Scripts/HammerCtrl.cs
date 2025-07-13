using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class HammerCtrl : MonoBehaviour
{
    [SerializeField] private Animator animator;   
    private float offsetY = 0.5f;    // Tùy chỉnh vị trí cao hơn player một chút
   
    public async UniTaskVoid ShowHammerAt(Vector3 targetPosition)
    {
        Debug.Log($"CheckShowHammerAt: {targetPosition}");
        transform.position = new Vector3(targetPosition.x-1, targetPosition.y+1, targetPosition.z);
        gameObject.SetActive(true);
        animator.SetTrigger("Slaping");
        
        
        await UniTask.Delay(500);
        HideHammer();
    }
    public void HideHammer()
    {
        gameObject.SetActive(false);
    }
}
