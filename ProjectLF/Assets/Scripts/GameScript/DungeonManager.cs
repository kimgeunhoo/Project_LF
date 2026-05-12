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
        private DungeonContext ctx;

        public void SetContext(DungeonContext context)
        {
            ctx = context;
        }

        #region 방 진입 메서드
        public void OnEnterRoom(int roomId, RoomType roomType)
        {

            //Debug.Log($"[DungeonManager] OnEnterRoom 호출 / roomId={roomId}, roomType={roomType}");

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
        #endregion

        #region 몬스터 사망 메서드
        public void OnMonsterDead(int roomId)
        {
            if (ctx == null || ctx.RoomStates == null)
                return;

            if (roomId < 0 || roomId >= ctx.RoomStates.Count)
                return;

            RoomRuntimeData room = ctx.RoomStates[roomId];

            if (room == null || room.IsCleared)
                return;
            Debug.Log($"[MonsterDead] 호출 전 / Room={roomId}, Alive={room.AliveMonsterCount}");
            room.AliveMonsterCount--;
            Debug.Log($"[MonsterDead] 호출 후 / Room={roomId}, Alive={room.AliveMonsterCount}");
            if (room.AliveMonsterCount <= 0)
            {
                room.AliveMonsterCount = 0;
                OnRoomClear(roomId);
            }

        }
        #endregion

        #region 룸 클리어(데이터 초기화)
        private void OnRoomClear(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            room.IsCleared = true;

            OpenDoor(roomId);

            Debug.Log($"[Room {roomId}] Clear");
        }
        #endregion

        #region 몬스터 방 잠금 메서드
        private void EnterEnemyRoom(int roomId)
        {
            Debug.Log($"[DungeonManager] EnterEnemyRoom 호출 / roomId={roomId}");

            if (ctx == null)
            {
                Debug.LogError("[DungeonManager] ctx가 null입니다. DungeonManager에 DungeonContext가 연결되지 않았습니다.");
                return;
            }

            if (ctx.RoomStates == null)
            {
                Debug.LogError("[DungeonManager] ctx.RoomStates가 null입니다.");
                return;
            }

            if (roomId < 0 || roomId >= ctx.RoomStates.Count)
            {
                Debug.LogError($"[DungeonManager] roomId 범위 오류 / roomId={roomId}, RoomStates.Count={ctx.RoomStates.Count}");
                return;
            }


            RoomRuntimeData room = ctx.RoomStates[roomId];

            if (room == null)
            {
                Debug.LogError($"[DungeonManager] RoomRuntimeData가 null입니다. roomId={roomId}");
                return;
            }

            if (room.IsCleared)
                return;

            Debug.Log($"[Dungeon] Enter Room {roomId}");

            CloseDoor(roomId);
            SpawnMonsters(roomId);
        }
        #endregion

        #region 문 닫힘 메서드
        private void CloseDoor(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            Debug.Log($"[CloseDoor] Room {roomId} Door Count = {room.Doors.Count}");

            foreach (var door in room.Doors)
            {
                door.Close();
            }
        }
        #endregion

        #region 문 열림 메서드
        private void OpenDoor(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            foreach (var door in room.Doors)
            {
                if (door == null)
                    continue;

                door.Open();
            }
        }
        #endregion

        #region 몬스터 스폰 메서드
        private bool SpawnMonsters(int roomId)
        {

            RoomRuntimeData room = ctx.RoomStates[roomId];
            Debug.Log($"[DungeonManager] Room {room.RoomId} SpawnPoint 수 = {room.monsterSpawnPoints.Count}");
            if (room.monsterSpawnPoints == null || room.monsterSpawnPoints.Count <= 0)
            {
                Debug.LogWarning($"[Room {roomId}] SpawnPoint 없음");
                return false;
            }

            if (monsterPF == null || monsterPF.Length <= 0)
            {
                Debug.LogWarning($"[DungeonManager] MonsterPF 없음");
            }

            if (room.AliveMonsterCount > 0)
                return false;

            int monsterCount = Mathf.Min(
                Random.Range(6, 9), room.monsterSpawnPoints.Count);

            List<MonsterSpawnPoint> spawnPoints = new List<MonsterSpawnPoint>(room.monsterSpawnPoints);

            Shuffled(spawnPoints);

            for (int i = 0; i < monsterCount; i++)
            {
                Transform spawnTrs = spawnPoints[i].transform;

                GameObject monster = Instantiate(monsterPF[0], spawnTrs.position, Quaternion.identity);

                room.SpawnedMonsters.Add(monster);
                room.AliveMonsterCount++;

                Monster monsterScript = monster.GetComponent<Monster>();
                if (monsterScript != null)
                {
                    monsterScript.Init(room.RoomId, this);
                }
            }

            Debug.Log($"[Spawn] Room {roomId} / Alive = {room.AliveMonsterCount}");
            return room.AliveMonsterCount > 0;
        }
        #endregion

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
