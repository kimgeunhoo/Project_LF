using GameScript.Manager;
using UnityEngine;

public class Monster : Character
{
    private GameObject monster;
    private int roomId;
    private DungeonManager dungeonManager;

    public void Init(int _roomId, DungeonManager _manager, GameObject _monster)
    {
        roomId = _roomId;
        dungeonManager = _manager;
        monster = _monster;
    }

    public void Die()
    {
        dungeonManager.OnMonsterDead(roomId);
        Destroy(monster);
    }

}
