using UnityEngine;
using Utils;

public class GameConfigSetting : Singleton<GameConfigSetting>
{
    public enum EGameMode
    {
        NOT_SET,
        EASY,
        MEDIUM,
        HARD,
        IMPOSSIBLE
    }
    public EGameMode gameMode;

    protected override void CustomAwake()
    {
        gameMode = EGameMode.NOT_SET;
    }

    void Start()
    {
        // Đảm bảo gameMode được set nếu chưa có
        if (gameMode == EGameMode.NOT_SET)
        {
            gameMode = EGameMode.NOT_SET;
        }
    }
    public void SetGameMode(EGameMode mode)
    {
        gameMode = mode;
    }

    public void SetGameMode(string mode)
    {
        switch (mode)
        {
            case "Easy": SetGameMode(EGameMode.EASY); break;
            case "Medium": SetGameMode(EGameMode.MEDIUM); break;
            case "Hard": SetGameMode(EGameMode.HARD); break;
            case "Impossible": SetGameMode(EGameMode.IMPOSSIBLE); break;
            default: SetGameMode(EGameMode.NOT_SET); break;
        }
    }
    public string GetGameMode()
    {
        switch (gameMode)
        {
            case EGameMode.EASY: return "Easy";
            case EGameMode.MEDIUM: return "Medium";
            case EGameMode.HARD: return "Hard";
            case EGameMode.IMPOSSIBLE: return "Impossible";
        }
        Debug.LogError($"ERROR: Game level is not set {gameMode}");
        return " ";
    }
}