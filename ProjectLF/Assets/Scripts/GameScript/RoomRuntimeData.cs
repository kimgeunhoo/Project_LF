using BSPDungeonGenrator.Config;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoomRuntimeData
{
    public int RoomId;
    public RoomInfo RoomInfo;

    public List<DoorController> Doors = new List<DoorController>();
    public int AliveMonsterCount = 0;

    public bool IsCleared = false;
    public bool IsBattleStarted = false;
    public bool HasSpawnedMonsters = false;

    public Vector2Int SpawnPoint;
    public List<GameObject> SpawnedMonsters = new List<GameObject>();
}
