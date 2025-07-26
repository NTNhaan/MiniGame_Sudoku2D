using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWatchAdsGetCoin : MonoBehaviour
{
    [SerializeField] private int coinReward;
    [SerializeField] private Text txtCoin;

    private void Start()
    {
        txtCoin.text = $"+ {coinReward}";// coinReward.ToString();
    }
    public void OnclickRewardAds()
    {
       /* AdMobRewardedAd.Instance.ShowRewardedAd(() =>
        {
            var coin = PlayerPrefs.GetInt("coin", 0);
            coin += coinReward;
            PlayerPrefs.SetInt("coin", coin);
            GameplayUI.Instance.UpdateCoin();
            Debug.Log($"Reward ads complete {coin}");
        });*/
    }
}
