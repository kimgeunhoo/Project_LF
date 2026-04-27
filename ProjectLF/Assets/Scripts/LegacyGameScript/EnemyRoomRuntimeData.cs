using BSPDungeonGenrator.Config;
using System.Collections.Generic;
using UnityEngine;


namespace LegacyGameScrpit
{

    [System.Serializable]
    public class EnemyRoomRuntimeData
    {
        public int RoomId;
        public RoomInfo RoomInfo;

        public List<DoorController> Doors = new List<DoorController>();
        public BoxCollider2D RoomCollider;

        public bool IsCleared = false;
        public bool IsBattleStarted = false;

        public int AliveMonsterCount = 0;
        public bool HasSpawnedMonsters = false;

        public Vector2Int SpawnPoint;
        public List<GameObject> SpawnedMonsters = new List<GameObject>();

        public EnemyRoomRuntimeData(int roomId, RoomInfo roomInfo)
        {
            RoomId = roomId;
            RoomInfo = roomInfo;
            Doors = new List<DoorController>();
            AliveMonsterCount = 0;
            SpawnPoint = roomInfo.Center;
        }
    }

}