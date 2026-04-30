using System.Collections.Generic;
using ModularBSP.Core;
using UnityEngine;

namespace MapGenerator.Core
{
    [System.Serializable]
    public class RoomRuntimeData
    {
        public int RoomId;
        public IntRect RoomRect;
        public RoomType RoomType;
        public Vector2Int CenterCell;
        public Vector3 CenterWorld;

        // 몬스터 룸 데이터
        public List<DoorController> Doors = new List<DoorController>();
        public List<GameObject> SpawnedMonsters = new List<GameObject>();
        public List<MonsterSpawnPoint> monsterSpawnPoints = new List<MonsterSpawnPoint>();
        public int AliveMonsterCount;
        public bool IsCleared = false;

        public RoomRuntimeData(int roomId, IntRect roomRect, RoomType roomType, Vector2Int centerCell, Vector3 centerWorld)
        {
            RoomId = roomId;
            RoomRect = roomRect;
            RoomType = roomType;
            CenterCell = centerCell;
            CenterWorld = centerWorld;
        }

    }
}
