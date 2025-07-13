using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] private List<Star> listStars;
    private float countDown = 3f;
    private int duration = 2;
    [SerializeField] private Star _star;
    void Update()
    {
        countDown -= Time.deltaTime;
        if (countDown <= 0f)
        {
            StartCoroutine(ShowStarOnBoard());
            countDown = 3f;
        }
    }

    IEnumerator ShowStarOnBoard()
    {
        List<Star> list = GetRandomStars(3);
        Debug.Log("Test" + list.Count);
        foreach (var star in list)
        {
            star.PlayAppearAnim();
            // AudioManager.Instance.PlayerAppearSound();
        }

        // Chờ đến khi cả 3 animation của star xong
        yield return new WaitUntil(() =>
        {
            bool isEndAnimation = true;
            for (int i = 0; i < list.Count; i++)
            {
                bool isEndAnimation1 = !list[i].IsAnimationEnd;
                if (isEndAnimation1 == false)
                {
                    isEndAnimation = false;
                    break;
                }
            }

            return isEndAnimation;
        });

        Debug.Log("End Animation");





        /// kiểm tra cả 3 có được đập chưa, nếu có 1,2, hoặc 3 thằng chưa bị đập thì trừ điểm
        int clickedCount = CountClickedStars(list);
        if (clickedCount <= 0)
        {
            GameManager.instance.HandleHealPlayer(1);
        }
        // StartCoroutine(HideStars(list, 1));

    }
    List<Star> GetRandomStars(int count)
    {
        List<int> lstIndexResult = new List<int>();
        List<int> lstIndexAvailable = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        for (int i = 1; i <= count; i++)
        {
            int index = Random.Range(0, lstIndexAvailable.Count);
            lstIndexResult.Add(lstIndexAvailable[index]);
            lstIndexAvailable.RemoveAt(index);
        }
        List<Star> result = new List<Star>();
        for (int i = 0; i < lstIndexResult.Count; i++)
        {
            // Debug.Log($"List star: {lstIndexResult[i]}");
            result.Add(listStars[lstIndexResult[i]]);
        }
        return result;
    }
    IEnumerator HideStars(List<Star> stars, float delay)
    {
        // Hàm này đổi thành animation down
        yield return new WaitForSeconds(delay);

        foreach (var star in stars)
        {
            // star.Back();
        }
        if (!_star.isPressed)  // nếu chưa ấn thì trừ điểm
        {
            Debug.Log("test " + Star.Instance.isPressed);
            GameManager.instance.HandleHealPlayer(1);
            _star.isPressed = true;
        }
    }
    int CountClickedStars(List<Star> list)
    {
        int count = 0;
        foreach (var star in list)
        {
            if (star.isClickObject)
            {
                count++;
                Debug.Log($"Clicked Count: {count} {list.Count}");
            }
        }
        return count;
    }
}
