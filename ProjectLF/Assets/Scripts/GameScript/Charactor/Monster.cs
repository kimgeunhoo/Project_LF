using GameScript.Manager;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private GameObject monster;
    private int roomId;
    private DungeonManager dungeonManager;

    //[SerializeField]
    //private Character monsterConfig;


    public void Init(int _roomId)
    {
        roomId = _roomId;
    }

    public void Die()
    {
        GameManager.Instance.NotifyMonsterDead(roomId);
        Destroy(monster);
    }

}
