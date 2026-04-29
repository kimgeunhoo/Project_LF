using BSPDungeonGenrator.Config;
using LegacyGameScrpit;
using System.Collections.Generic;
using UnityEngine;


namespace BSPDungeonGenrator.Core
{
    [System.Serializable]
    public class RoomRuntimeData
    {
        public int RoomId;
        public RoomInfo RoomInfo;

        //public List<DoorController> Doors = new List<DoorController>();
        public BoxCollider2D RoomCollider;

        public bool IsCleared = false;
        public bool IsBattleStarted = false;


        public int AliveMonsterCount = 0;
        public bool HasSpawnedMonsters = false;

        public Vector2Int SpawnPoint;
        public List<GameObject> SpawnedMonsters = new List<GameObject>();
        private int i;
        private IntRect room;
        private RoomType type;
        private Vector2Int centerCell;
        private Vector3 centerWorld;

        public RoomRuntimeData(int i, RoomType type, Vector2Int centerCell, Vector3 centerWorld)
        {
            this.i = i;
            this.type = type;
            this.centerCell = centerCell;
            this.centerWorld = centerWorld;
        }

        private class IntRect
        {

        }
    }

}