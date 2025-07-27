using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils;
public class ShopController : Singleton<ShopController>
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<ItemCoin> lstItemCoin;
    bool isOpen = false;

    public bool IsOpen { get => isOpen; }

    private async void Start()
    {
        // Đảm bảo thời gian game bình thường khi khởi động
        Time.timeScale = 1f;

        // Đợi cả DatabaseController và IAPController được khởi tạo
        await UniTask.WaitUntil(() => DatabaseController.Instance != null && IAPController.Instance != null && IAPController.Instance.IsInitialized());
        Debug.Log($"Shop initialization started. Current coins: {DatabaseController.Instance.Coin}");
        InitializeIAP();
        InitializeItemCoins();
    }
    public async UniTask ShowShop()
    {
        if (isOpen)
        {
            Hide();
            return;
        }
        Show();
        await UniTask.Delay(1000);
    }
    public void Show()
    {
        AudioController.Instance.PlayClickSound();
        shopUI.gameObject.SetActive(true);
        isOpen = true;
        Timmer.Instance.PauseTime();
    }

    public void Hide()
    {
        AudioController.Instance.PlayClickSound();
        shopUI.gameObject.SetActive(false);
        isOpen = false;
        Timmer.Instance.ResumeTime();
    }

    public void InitializeIAP()
    {
        IAPController.Instance.OnPurchaseSuccess += (key) =>
        {
            Debug.Log($"Purchased item with key: {key}");
            for (int i = 0; i < lstItemCoin.Count; i++)
            {
                if (lstItemCoin[i].Key == key)
                {
                    lstItemCoin[i].OnSuccess();
                    // Cập nhật số coins trong GameManager để refresh UI
                    GameManager.instance.UpdateCoin();
                    Debug.Log($"Added {lstItemCoin[i].CoinReceive} coins. New total: {DatabaseController.Instance.Coin}");
                }
            }
        };
        Debug.Log("IAP initialized");

    }

    public void InitializeItemCoins()
    {
        //  for (int i = 0; i < lstItemCoin.Count; i++)
        for (int i = lstItemCoin.Count - 1; i >= 0; i--)
        {
            var product = IAPController.Instance.GetProductByKey(lstItemCoin[i].Key);
            Debug.Log($"InitializeItemCoins: name:{gameObject.name} key:{lstItemCoin[i].Key} {product == null}");
            var price = product != null ? product.metadata.localizedPriceString : "N/A";
            lstItemCoin[i].Init(price, OnClickItem);
        }

        Debug.Log("Item coins initialized");
    }
    public void OnClickItem(string key)
    {
        try
        {
            AudioController.Instance?.PlayClickSound();

            Debug.Log("=== PURCHASE DEBUG INFO ===");
            Debug.Log($"1. Clicked item key: {key}");

            // Kiểm tra DatabaseController
            if (DatabaseController.Instance != null)
            {
                Debug.Log($"2. Database status: OK, Current coins: {DatabaseController.Instance.Coin}");
            }
            else
            {
                Debug.LogError("2. Database status: NULL!");
                return;
            }

            // Kiểm tra GameManager
            if (GameManager.instance != null)
            {
                Debug.Log($"3. GameManager status: OK");
            }
            else
            {
                Debug.LogError("3. GameManager status: NULL!");
            }

            // Tìm và kiểm tra ItemCoin
            ItemCoin selectedItem = lstItemCoin.Find(item => item.Key == key);
            if (selectedItem != null)
            {
                Debug.Log($"4. Found ItemCoin: {selectedItem.name}, CoinReceive: {selectedItem.CoinReceive}");

                // Lưu số coin trước khi thay đổi
                int beforeCoins = DatabaseController.Instance.Coin;

                // Thực hiện mua
                selectedItem.OnSuccess();

                // Kiểm tra số coin sau khi thay đổi
                int afterCoins = DatabaseController.Instance.Coin;
                Debug.Log($"5. Coin change: {beforeCoins} -> {afterCoins} (diff: {afterCoins - beforeCoins})");

                // Cập nhật UI
                GameManager.instance?.UpdateCoin();
                Debug.Log("6. UI update requested");
            }
            else
            {
                Debug.LogError($"4. ItemCoin not found for key: {key}");
            }

            // Kiểm tra thông tin sản phẩm
            var product = IAPController.Instance?.GetProductByKey(key);
            if (product != null)
            {
                Debug.Log($"7. Product info - Key: {key}, Price: {product.metadata.localizedPriceString}");
            }

            Debug.Log("=== END PURCHASE DEBUG ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in OnClickItem: {e.Message}\n{e.StackTrace}");
        }
    }
}
