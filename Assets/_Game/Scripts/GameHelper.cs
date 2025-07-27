using UnityEngine;
using Utils;

public class GameHelper : Singleton<GameHelper>
{
    [SerializeField] private BoardController boardController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Timmer timer;
    [SerializeField] private UIManager UImanager;
    public BoardController BoardController { get => boardController; }
    public GameManager GameManager { get => gameManager; }
    public Timmer Timmer { get => timer; }
    public UIManager UIManager { get => UImanager; }

}