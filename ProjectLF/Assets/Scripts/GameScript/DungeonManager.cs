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
            //dungeonGenerator.SetupMonsterSpawnPoint(ctx);
            ResetRoomStates();
            CheckStartRoom();

            AssignSpawnPointsToRooms(ctx);
        }

        private void InitAllDoorsOpen()
        {
            foreach (var room in ctx.RoomStates)
            {
                foreach (var door in room.Doors)
                {
                    if(door != null)
                    {
                        //door.SetOpen(true);
                    }
                }
            }

            //Debug.Log("[DungeonManager] all Door Open(default)");
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
            //Debug.Log("[DungeonManager] RoomStates reset complete");

        }

        // 방에 콜라이더 없다면 생성
        private void CreateRoomColliders()
        {
            foreach (var room in ctx.RoomStates)
            {
                RectInt rect = room.RoomInfo.Rect;

                GameObject colObj = new GameObject($"RoomCollider_{room.RoomId}");
                colObj.transform.parent = transform;

                BoxCollider2D col = colObj.AddComponent<BoxCollider2D>();

                Vector3Int cellCenter = new Vector3Int(
                    rect.x + rect.width / 2 - ctx.MapSize.x / 2,
                    rect.y + rect.height / 2 - ctx.MapSize.y / 2,
                    0
                    );
                Vector3 worldCenter = floorTilemap.GetCellCenterWorld(cellCenter);

                col.offset = worldCenter - colObj.transform.position;
            }

       
        }

        private void AssignSpawnPointsToRooms(DungeonContext _ctx)
        {
            if(roomDistribute == null)
            {
                //Debug.LogError("[AssignSpawnPointsToRooms] roomDistribute is null");
                return;
            }

            foreach(var spawnPos in roomDistribute.MonsterSpawnPoint)
            {
                for (int i = 0; i < ctx.RoomStates.Count; i++)
                {
                    RectInt rect = ctx.RoomStates[i].RoomInfo.Rect;

                    if (rect.Contains(spawnPos))
                    {
                        ctx.RoomStates[i].SpawnPoint = spawnPos;
                        //Debug.Log($"[AssignSpawn] RoomId={ctx.RoomStates[i].RoomId}, Spawn={spawnPos}");
                        break;
                    }
                }
            }

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
                    //Debug.Log($"[StartRoom] Player starts in Room {room.RoomId}");
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

        // 몬스터 스폰 roomID reFectorying 버전
        private void SpawnMonstersInRoom(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates[roomId];

            if (room.HasSpawnedMonsters)
                return;

            room.HasSpawnedMonsters = true;

            List<Vector2Int> spawnPoints = roomDistribute.MonsterSpawnPoint;

            foreach (var spawnPoint in spawnPoints)
            {
                if(!room.RoomInfo.Rect.Contains(spawnPoint))
                    continue;

                Vector3Int cellPos = new Vector3Int(
                    spawnPoint.x - ctx.MapSize.x / 2,
                    spawnPoint.y - ctx.MapSize.y / 2,
                    0);

                Vector3 worldPos = floorTilemap.GetCellCenterLocal(cellPos);

                for(int i = 0; i < monsterCount; i++)
                {
                    Vector3 offset = new Vector3
                    (
                        Random.Range(-5f, 5f),
                        Random.Range(-5f, 5f),
                        0);

                    GameObject monster = Instantiate(monsterPF[0], worldPos + offset, Quaternion.identity, monsterHolder);
                    Monster m = monster.GetComponent<Monster>();
                    if (m != null)
                    {
                        m.Init(roomId, this);
                        //Debug.Log($"[MonsterSpawn] Init monster / roomId={roomId}");
                    }

                    room.SpawnedMonsters.Add(monster);
                    room.AliveMonsterCount++;
                }

            }

        }

        // 문 열기 roomId
        private void OpenRoomDoors(int roomId)
        {
            var room = ctx.RoomStates[roomId];
            foreach (var door in room.Doors)
            {
                if (door != null)
                {
                   // door.SetOpen(true);
                }
            }
        }
       

        // 문 닫기 roomId
        private void CloseRoomDoors(int roomId)
        {
            var room = ctx.RoomStates[roomId];
            foreach (var door in room.Doors)
            {
                if (door != null)
                {
                   // door.SetOpen(false);
                }
            }
        }
     
        // enemyPoint 들어갔을 시
        public void EnterEnemyRoom(int roomId)
        {
            RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);
            if (room == null)
                return;

            if (room.IsCleared)
            {
                //Debug.Log($"[EnterEnemyRoom] Room {roomId} already cleared.");
                OpenRoomDoors(roomId);
                return;
            }

            if (room.IsBattleStarted)
            {
                //Debug.Log($"[EnterEnemyRoom] Room{roomId} Battle started.");
                return;
            }

            CloseRoomDoors(roomId);
            SpawnMonstersInRoom(roomId);
        }


        // refactorying 방식

        public void OnMonsterDead(int roomId)
        {
            var room = ctx.RoomStates[roomId];

            room.AliveMonsterCount--;

           // Debug.Log($"[Room {roomId}] Remaining: {room.AliveMonsterCount}");

            if (room.AliveMonsterCount <= 0)
            {
                OnRoomClear(roomId);
            }
        }

        private void OnRoomClear(int roomId)
        {
            var room = ctx.RoomStates[roomId];

            room.IsCleared = true;

            //Debug.Log($"[Room {roomId}] clear");

            foreach (var door in room.Doors)
            {
               // if (door != null)
                  //  door.SetOpen(true);
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

//------------------------ 레거시 코드 ----------------------------
// 문 열기
//private void OpenRoomDoors(RoomRuntimeData room)
//{
//    if (room == null)
//        return;
//    foreach (var door in room.Doors)
//    {
//        if(door != null)
//        {
//            door.SetOpen(true);
//        }
//    }
//}
// 문 닫기
//private void CloseRoomDoors(RoomRuntimeData room)
//{
//    if (room == null)
//        return;
//    foreach (var door in room.Doors)
//    {
//        if (door != null)
//        {
//            door.SetOpen(false);
//        }
//    }
//}

// enemyPoint 들어갔을 시
//public void EnterEnemyRoom(int roomId)
//{
//    RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);
//    if (room == null)
//        return;

//    if (room.IsCleared)
//    {
//        Debug.Log($"[EnterEnemyRoom] Room {roomId} already cleared.");
//        OpenRoomDoors(room);
//        return;
//    }

//    if (room.IsBattleStarted)
//    {
//        Debug.Log($"[EnterEnemyRoom] Room{roomId} Battle started.");
//        return;
//    }

//    CloseRoomDoors(room);
//    SpawnMonstersInRoom(ctx, room);
//}
// 몬스터 전부 사망시
//public void OnMonsterDead(int roomId, GameObject monsterObj)
//{
//    RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);
//    if (room == null)
//        return;
//    if(monsterObj != null)
//    {
//        room.SpawnedMonsters.Remove(monsterObj);
//    }

//    room.AliveMonsterCount--;
//    if(room.AliveMonsterCount < 0)
//    {
//        room.AliveMonsterCount = 0;
//    }

//    Debug.Log($"[OnMonsterDead] RoomId = {roomId}, Alive = {room.AliveMonsterCount}");

//    if (room.AliveMonsterCount == 0)
//    {
//        room.IsCleared = true;
//        room.IsBattleStarted = false;

//        OpenRoomDoors(room);

//        Debug.Log($"[OnMonsterDead] Room {roomId} Cleared");
//    }
//}
// 몬스터 스폰
//private void SpawnMonstersInRoom(DungeonContext _ctx, RoomRuntimeData room)
//{
//    // null, 클리어, 이미 스폰됨
//    if (room == null)
//        return;
//    if (room.IsCleared)
//        return;
//    if (room.HasSpawnedMonsters)
//        return;

//    Vector3Int cellPos = 
//        new Vector3Int(room.SpawnPoint.x - _ctx.MapSize.x / 2, room.SpawnPoint.y - _ctx.MapSize.y / 2, 0);

//    Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);
//    worldPos.z = 0f;

//    room.AliveMonsterCount = 0;
//    room.SpawnedMonsters.Clear();

//    for(int j = 0; j < monsterCount; j++)
//    {
//        Vector3 offset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f));

//        GameObject obj = Instantiate(monsterPF[0], worldPos + offset, Quaternion.identity, monsterHolder);
//        Monster monsterComp = obj.GetComponent<Monster>();

//        if (monsterComp != null)
//        {
//            monsterComp.Init(room.RoomId, this);
//        }

//        room.SpawnedMonsters.Add(obj);
//        room.AliveMonsterCount++;

//    }
//    room.HasSpawnedMonsters = true;
//    room.AliveMonsterCount++;

//    Debug.Log($"[SpawnMonstersInRoom] RoomId = {room.RoomId}, Count= {room.AliveMonsterCount}");
//}