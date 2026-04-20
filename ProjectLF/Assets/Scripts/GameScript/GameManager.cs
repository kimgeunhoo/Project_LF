using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player Player { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(Player player)
    {
        Player = player;
    }
}
