using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.marker;
using BSPDungeonGenrator.Rendering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;



namespace BSPDungeonGenrator.Generation
{
    public class DungeonManager : MonoBehaviour
    {
        [Header("User Object")]
        [SerializeField]
        private GameObject playerPF;

        [Header("Monster Object")]
        [SerializeField] 
        private GameObject[] monsterPF;
        [SerializeField]
        private int monsterCount = 7;
        [SerializeField]
        private Transform monsterHolder;

        [Header("Map Generator")]
        [SerializeField]
        private DungeonGeneraterByBSP dungeonGenerator;
        [SerializeField]
        private Tilemap floorTilemap;

        [SerializeField]
        private RoomDistribute roomDistribute;

        private DungeonContext ctx;

        private void Start()
        {
            ctx = dungeonGenerator.Ctx;
            GameObject playerObj = Instantiate(playerPF);
            PlayerSpawn(ctx, playerObj);
            
            var cam = FindFirstObjectByType<CinemachineCamera>();
            cam.Follow = playerObj.transform;
            cam.LookAt = playerObj.transform;

            InitAllDoorsOpen();

            dungeonGenerator.SetupMonsterSpawnPoint(ctx);

            ResetRoomStates();

            CheckStartRoom();

        }

        private void InitAllDoorsOpen()
        {
            foreach (var room in ctx.RoomStates)
            {
                foreach (var door in room.Doors)
                {
                    if(door != null)
                    {
                        door.SetOpen(true);
                    }
                }
            }

            Debug.Log("[DungeonManager] all Door Open(default)");
        }

        private void ResetRoomStates()
        {
            foreach(var room in ctx.RoomStates)
            {
                room.IsCleared = false;
                room.IsBattleStarted = false;
                room.HasSpawnedMonsters = false;
                room.AliveMonsterCount = 0;

                if (room.SpawnedMonsters != null)
                {
                    room.SpawnedMonsters.Clear();
                }

            }
            Debug.Log("[DungeonManager] RoomStates reset complete");

        }

        // 시작 방 체크
        private void CheckStartRoom()
        {
            if (playerPF == null)
                return;

            Vector3 playerPos = playerPF.transform.position;

            foreach (var room in ctx.RoomStates)
            {
                RectInt rect = room.RoomInfo.Rect;

                Vector2Int gridPos = WorldToGrid(playerPos);

                if (rect.Contains(gridPos))
                {
                    Debug.Log($"[StartRoom] Player starts in Room {room.RoomId}");
                    break;
                }

            }
        }

        // World를 Grid로 변환
        private Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3Int cell = floorTilemap.WorldToCell(worldPos);

            int x = cell.x + ctx.MapSize.x / 2;
            int y = cell.y + ctx.MapSize.y / 2;

            return new Vector2Int(x, y);
        }

        // 플레이어 스폰
        private void PlayerSpawn(DungeonContext _ctx, GameObject _playerObj)
        {
            Vector2Int psp = roomDistribute.StartSpawnPoint;

            Vector3Int cellPos = new Vector3Int(psp.x - _ctx.MapSize.x / 2, psp.y - _ctx.MapSize.y / 2, 0);

            Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);

            _playerObj.transform.position = worldPos;

        }

        // 몬스터 스폰
        private void SpawnMonstersInRoom(DungeonContext _ctx, RoomRuntimeData room)
        {
            // null, 클리어, 이미 스폰됨
            if (room == null)
                return;
            if (room.IsCleared)
                return;
            if (room.HasSpawnedMonsters)
                return;

            Vector3Int cellPos = 
                new Vector3Int(room.SpawnPoint.x - _ctx.MapSize.x / 2, room.SpawnPoint.y - _ctx.MapSize.y / 2, 0);

            Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);
            worldPos.z = 0f;

            room.AliveMonsterCount = 0;
            room.SpawnedMonsters.Clear();

            for(int j = 0; j < monsterCount; j++)
            {
                Vector3 offset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));

                GameObject obj = Instantiate(monsterPF[0], worldPos + offset, Quaternion.identity, monsterHolder);
                Monster monsterComp = obj.GetComponent<Monster>();

                if (monsterComp != null)
                {
                    monsterComp.Init(room.RoomId, this);
                }

                room.SpawnedMonsters.Add(obj);
                room.AliveMonsterCount++;

            }
            room.HasSpawnedMonsters = true;
            room.AliveMonsterCount++;

            Debug.Log($"[SpawnMonstersInRoom] RoomId = {room.RoomId}, Count= {room.AliveMonsterCount}");
        }

        // 문 열기
        private void OpenRoomDoors(RoomRuntimeData room)
        {
            if (room == null)
                return;
            foreach (var door in room.Doors)
            {
                if(door != null)
                {
                    door.SetOpen(true);
                }
            }
        }

        // 문 닫기
        private void CloseRoomDoors(RoomRuntimeData room)
        {
            if (room == null)
                return;
            foreach (var door in room.Doors)
            {
                if (door != null)
                {
                    door.SetOpen(false);
                }
            }
        }

        public void EnterEnemyRoom(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);
            if (room == null)
                return;

            if (room.IsCleared)
            {
                Debug.Log($"[EnterEnemyRoom] Room {roomId} already cleared.");
                OpenRoomDoors(room);
                return;
            }

            if (room.IsBattleStarted)
            {
                Debug.Log($"[EnterEnemyRoom] Room{roomId} Battle started.");
                return;
            }

            CloseRoomDoors(room);
            SpawnMonstersInRoom(ctx, room);
        }

        public void OnMonsterDead(int roomId, GameObject monsterObj)
        {
            RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);
            if (room == null)
                return;
            if(monsterObj != null)
            {
                room.SpawnedMonsters.Remove(monsterObj);
            }

            room.AliveMonsterCount--;
            if(room.AliveMonsterCount < 0)
            {
                room.AliveMonsterCount = 0;
            }

            Debug.Log($"[OnMonsterDead] RoomId = {roomId}, Alive = {room.AliveMonsterCount}");

            if (room.AliveMonsterCount == 0)
            {
                room.IsCleared = true;
                room.IsBattleStarted = false;

                OpenRoomDoors(room);

                Debug.Log($"[OnMonsterDead] Room {roomId} Cleared");
            }
        }


        private void ShopSpawn()
        {

        }
        private void EncounterSpawn()
        {

        }
    }
}
