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
        LoadCurrentProgressionLevel(NEXT_SCENE);
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

    private void LoadCurrentProgressionLevel(string sceneName)
    {
        if (GameConfigSetting.Instance != null)
        {
            string currentDifficulty = GameConfigSetting.Instance.GetCurrentDifficulty();
            int currentSubLevel = GameConfigSetting.Instance.GetCurrentSubLevel();

            GameConfigSetting.Instance.SetGameMode(currentDifficulty);

            StartCoroutine(LoadingSceneFixedTime(sceneName));
        }
        else
        {
            LoadEasyGame(sceneName);
        }
    }

    #region LoadLevel
    private void LoadEasyGame(string name)
    {
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.EASY);
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    private void LoadMediumGame(string name)
    {
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.MEDIUM);
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    private void LoadHardGame(string name)
    {
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.HARD);
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    private void LoadImpossibleGame(string name)
    {
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.SetGameMode(GameConfigSetting.EGameMode.IMPOSSIBLE);
        }
        StartCoroutine(LoadingSceneFixedTime(name));
    }
    #endregion

    public void LoadSpecificDifficulty(string difficulty)
    {
        switch (difficulty.ToLower())
        {
            case "easy":
                LoadEasyGame(NEXT_SCENE);
                break;
            case "medium":
                LoadMediumGame(NEXT_SCENE);
                break;
            case "hard":
                LoadHardGame(NEXT_SCENE);
                break;
            case "impossible":
                LoadImpossibleGame(NEXT_SCENE);
                break;
            default:
                LoadCurrentProgressionLevel(NEXT_SCENE);
                break;
        }
    }

    public void ResetAndLoadFromBeginning()
    {
        if (GameConfigSetting.Instance != null)
        {
            GameConfigSetting.Instance.ResetProgression();
        }
        LoadCurrentProgressionLevel(NEXT_SCENE);
    }
    #endregion
}
