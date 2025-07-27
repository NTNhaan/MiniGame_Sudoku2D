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
    private void Start()
    {
        // Kiểm tra DatabaseController khi item được tạo
        if (DatabaseController.Instance == null)
        {
            Debug.LogError("DatabaseController is not in the scene! Please add it to the scene.");
        }
    }

    public void OnClickItem()
    {
        Debug.Log($"OnClickItem: name:{gameObject.name} key:{key}, coinReceive:{coinReceive}");

        // Kiểm tra DatabaseController
        if (DatabaseController.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] Cannot process purchase - DatabaseController is missing!");
            return;
        }

        try
        {
            // Lưu số coin hiện tại
            int currentCoins = DatabaseController.Instance.Coin;
            Debug.Log($"[{gameObject.name}] Current coins: {currentCoins}");
            Debug.Log($"[{gameObject.name}] Adding coins: {coinReceive}");

            // Cập nhật coins trong Database
            DatabaseController.Instance.Coin = currentCoins + coinReceive;
            Debug.Log($"[{gameObject.name}] New total in Database: {DatabaseController.Instance.Coin}");

            // Cập nhật UI qua GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateCoin();
                Debug.Log($"[{gameObject.name}] UI updated via GameManager");
            }

            // Phát âm thanh khi mua thành công
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayClickSound();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error updating coins: {e.Message}\n{e.StackTrace}");
        }

        // Vẫn giữ lại callback cho ShopController nếu cần
        actionOnClick?.Invoke(key);
    }
    public void OnSuccess()
    {
        if (DatabaseController.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] DatabaseController is null!");
            return;
        }

        try
        {
            int currentCoins = DatabaseController.Instance.Coin;
            Debug.Log($"[{gameObject.name}] Current coins: {currentCoins}");
            Debug.Log($"[{gameObject.name}] Adding coins: {coinReceive}");

            // Cập nhật coins trong Database
            DatabaseController.Instance.Coin = currentCoins + coinReceive;
            Debug.Log($"[{gameObject.name}] New total in Database: {DatabaseController.Instance.Coin}");

            // Cập nhật UI qua GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.UpdateCoin();
                Debug.Log($"[{gameObject.name}] UI updated via GameManager");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] GameManager instance is null!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{gameObject.name}] Error updating coins: {e.Message}\n{e.StackTrace}");
        }
    }
}
