using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemCoin : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private int coinReceive;
    [SerializeField] private Text txtPrice;
    [SerializeField] private Text txtCoin;
    UnityAction<string> actionOnClick;

    public string Key => key;
    public int CoinReceive => coinReceive;

    public void Init(string txtPrice, UnityAction<string> actionOnClick)
    {
        this.txtPrice.text = txtPrice;
        txtCoin.text = coinReceive.ToString();
        this.actionOnClick = actionOnClick;
    }
    public void OnClickItem()
    {
        Debug.Log($"OnClickItem: name:{gameObject.name} key:{key}");
        actionOnClick?.Invoke(key);
    }
    public void OnSuccess()
    {
        Debug.Log($"OnSuccess: name:{gameObject.name} key:{key}");
        
        DatabaseController.Instance.Coin += coinReceive; // Increase the coin count
      //  GameplayUI.Instance.UpdateCoin();
    }
}
