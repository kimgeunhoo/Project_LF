using System.Collections.Generic;
using MapGenerator.Core;
using ModularBSP.Core;
using UnityEngine;

namespace GameScript.Manager
{
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] monsterPF;

        [SerializeField]
        private Transform[] monsterHolder;

        [SerializeField]
        private DungeonContext ctx;

        public void OnEnterRoom(int roomId, RoomType roomType)
        {

            switch (roomType)
            {
                case RoomType.Start:
                    break;
                case RoomType.Shop:
                    break;
                case RoomType.Stairs:
                    break;
                case RoomType.Encounter:
                    break;
                case RoomType.Enemy:
                    EnterEnemyRoom(roomId);
                    break;
                default:
                    break;
            }

        }

        public void OnMonsterDead(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            room.AliveMonsterCount--;

            if(room.AliveMonsterCount <= 0)
            {
                OnRoomClear(roomId);
            }
        }

        private void OnRoomClear(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            room.IsCleared = true;

            OpenDoor(roomId);

            Debug.Log($"[Room {roomId}] Clear");
        }

        private void EnterEnemyRoom(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            if (room.IsCleared)
                return;

            Debug.Log($"[Dungeon] Enter Room {roomId}");

            CloseDoor(roomId);
            SpawnMonsters(roomId);
        }

        private void CloseDoor(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            foreach (var door in room.Doors)
            {
                door.gameObject.SetActive(true);
            }
        }

        private void OpenDoor(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            foreach (var door in room.Doors)
            {
                door.gameObject.SetActive(false);
            }
        }

        private void SpawnMonsters(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            if (room.monsterSpawnPoints.Count <= 0)
            {
                Debug.LogWarning($"[Room {roomId}] SpawnPoint ¾øÀ½");
                return;
            }

            int monsterCount = Random.Range(6, 7);


            Debug.Log($"[Spawn] Room {roomId} / Count = {room.AliveMonsterCount}");
        }

        private void Shuffled(List<MonsterSpawnPoint> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }

    }

}
