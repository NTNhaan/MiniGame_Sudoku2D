using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadingManager : MonoBehaviour
{
    public static string NEXT_SCENE = "MainScene";
    public GameObject progressBar;
    public Text textPercent;
    private float fixedLoadingTime = 3f;

    public void Start()
    {
        LoadEasyGame(NEXT_SCENE);
        // StartCoroutine(LoadingSceneFixedTime(NEXT_SCENE));
    }
    public void ActivateObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void DeActivateObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    // public IEnumerator LoadSceneAsync(string sceneName)
    // {
    //     AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    //     while (!operation.isDone)
    //     {
    //         float progress = Mathf.Clamp01(operation.progress / 0.9f);
    //         progressBar.GetComponent<Image>().fillAmount = progress;
    //         textPercent.text = (progress * 100).ToString("0") + "%";

    //         yield return null;
    //     }
    // }
    public IEnumerator LoadingSceneFixedTime(string sceneName)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fixedLoadingTime)
        {
            float progress = Mathf.Clamp01(elapsedTime / fixedLoadingTime);
            progressBar.GetComponent<Image>().fillAmount = progress;
            textPercent.text = (progress * 100).ToString("0") + "%";

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }


    #region Load Level Game
    private void LoadEasyGame(string name)
    {
        Debug.Log("Loading level easy");
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.EASY);
        }
        else
        {
            Debug.LogError("GameConfigSetting.Instance is null!");
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    private void LoadMediumGame(string name)
    {
        Debug.Log("Loading level Medium");
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.MEDIUM);
        }
        else
        {
            Debug.LogError("GameConfigSetting.Instance is null!");
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    private void LoadHardGame(string name)
    {
        Debug.Log("Loading level hard");
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.HARD);
        }
        else
        {
            Debug.LogError("GameConfigSetting.Instance is null!");
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    private void LoadImpossibleGame(string name)
    {
        Debug.Log("Loading level Impossible");
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.IMPOSSIBLE);
        }
        else
        {
            Debug.LogError("GameConfigSetting.Instance is null!");
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    #endregion
}
