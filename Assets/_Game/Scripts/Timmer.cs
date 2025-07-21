using System;
using UnityEngine;
using UnityEngine.UI;

public class Timmer : MonoBehaviour
{
    [SerializeField] private Text textTime;
    private int second = 0;
    private int minute = 0;
    private float deltaTime;
    private bool stopClock = false;

    #region Init Data
    private void OnEnable()
    {
        EventManager.OnGameOver += ActionOnGameOver;
    }
    private void OnDisable()
    {
        EventManager.OnGameOver -= ActionOnGameOver;
    }
    void Start()
    {
        stopClock = false;
        deltaTime = 0;
    }
    #endregion
    void Update()
    {
        if (stopClock == false)
        {
            deltaTime += Time.deltaTime;
            TimeSpan span = TimeSpan.FromSeconds(deltaTime);

            string minute = LeadingZero(span.Minutes);
            string second = LeadingZero(span.Seconds);

            textTime.text = minute + ":" + second;
        }

    }
    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
    public void ActionOnGameOver()
    {
        stopClock = true;
    }
}
