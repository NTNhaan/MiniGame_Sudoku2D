using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShopController : Singleton<ShopController>
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<ItemCoin> lstItemCoin;
    bool isOpen = false;

    public bool IsOpen { get => isOpen;}

    private async void Start()
    {
        await UniTask.WaitUntil(() => IAPController.Instance.IsInitialized());
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
        shopUI.gameObject.SetActive(true);
        isOpen = true;
    }

    public void Hide()
    {
        shopUI.gameObject.SetActive(false);
        isOpen = false;
    }

    public void InitializeIAP()
    {
        IAPController.Instance.OnPurchaseSuccess += (key) =>
        {
            Debug.Log($"Purchased item with key: {key}");
            for (int i = 0; i < lstItemCoin.Count; i++)
            {
                if (lstItemCoin[i].Key == key)
                    lstItemCoin[i].OnSuccess();
            }
        };
        Debug.Log("IAP initialized");

    }

    public void InitializeItemCoins()
    {
      //  for (int i = 0; i < lstItemCoin.Count; i++)
            for (int i = lstItemCoin.Count-1; i>=0; i--)
        {
            var product = IAPController.Instance.GetProductByKey(lstItemCoin[i].Key);
            Debug.Log($"InitializeItemCoins: name:{gameObject.name} key:{lstItemCoin[i].Key} {product==null}");
            var price = product != null ? product.metadata.localizedPriceString : "N/A";
            lstItemCoin[i].Init(price, OnClickItem);
        }

        Debug.Log("Item coins initialized");
    }
    public void OnClickItem(string key)
    {
        var product = IAPController.Instance.GetProductByKey(key);
        Debug.Log($"OnClickItem: name:{gameObject.name} key:{key}");
        if (product != null)
        {
            string resultMessage;
            bool success = IAPController.Instance.BuyProduct(key, out resultMessage);

            if (success)
            {
                Debug.Log("Mua thành công");
            }
            else
            {
                Debug.LogError("Mua thất bại: " + resultMessage);
            }
            Debug.Log($"Purchased item with key: {key}, price: {product.metadata.localizedPriceString}");
        }
        else
        {
            Debug.LogWarning($"Product with key: {key} not found");
        }
    }
}
