using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPartent : MonoBehaviour
{
   void OnMouseDown()
   {
      if (gameObject.CompareTag("Groud"))
      {
         Debug.Log("Press wrong");
         GameManager.instance.HandleHealPlayer(1);
      }
   }
}
