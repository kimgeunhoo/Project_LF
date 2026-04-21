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

        private DoorSpawner doorSpawner;

        private OldDungeonContext ctx;

        private GameObject playerObj;

        [Header("GameOver UI")]
        [SerializeField]
        private GameObject gameOverUI;


        private void Start()
        {
            ctx = dungeonGenerator.Ctx;
            playerObj = Instantiate(playerPF);
            //GameManager.Instance.RegisterPlayer(playerObj.GetComponent<Player>());

            PlayerSpawn(ctx, playerObj);
            
            var cam = FindFirstObjectByType<CinemachineCamera>();

            //doorSpawner.Run(ctx);

            if (cam != null)
            {
                cam.Follow = playerObj.transform;
                cam.LookAt = playerObj.transform;
            }

            //dungeonGenerator.SetupMonsterSpawnPoint(ctx);
            ResetRoomStates();
            AssignSpawnPointsToRooms(ctx);
            CreateRoomColliders();
            InitAllDoorsOpen();
        }

        private void InitAllDoorsOpen()
        {
            foreach (var room in ctx.RoomStates)
            {
                foreach (var door in room.Doors)
                {
                    if(door != null)
                    {
                        door.OpenDoor();
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
                colObj.transform.SetParent(transform, false);

                BoxCollider2D col = colObj.AddComponent<BoxCollider2D>();
                col.isTrigger = true;

                Vector3Int minCell = new Vector3Int(
                    rect.xMin - ctx.MapSize.x / 2,
                    rect.yMin - ctx.MapSize.y / 2,
                    0
                    );

                Vector3Int maxCell = new Vector3Int(
                    rect.xMax - 1 - ctx.MapSize.x / 2,
                    rect.yMax - 1 - ctx.MapSize.y / 2,
                    0
                    );

                Vector3 minWorld = floorTilemap.GetCellCenterWorld(minCell);
                Vector3 maxWorld = floorTilemap.GetCellCenterWorld(maxCell);

                Vector3 worldCenter = (minWorld + maxWorld) * 0.5f;

                colObj.transform.position = worldCenter;
                col.size = new Vector2(rect.width - 0.5f, rect.height - 0.5f);

                EnemyRoomTrigger trigger = colObj.AddComponent<EnemyRoomTrigger>();
                trigger.Init(room.RoomId, this);
            }

       
        }

        private void AssignSpawnPointsToRooms(OldDungeonContext _ctx)
        {
            if(roomDistribute == null)
            {
                //Debug.LogError("[AssignSpawnPointsToRooms] roomDistribute is null");
                return;
            }

            foreach(var spawnPos in roomDistribute.MonsterSpawnPoint)
            {
                foreach (var room in ctx.RoomStates)
                {
                    RectInt rect = room.RoomInfo.Rect;

                    if (rect.Contains(spawnPos))
                    {
                        room.SpawnPoint = spawnPos;
                        //Debug.Log($"[AssignSpawn] RoomId={ctx.RoomStates[i].RoomId}, Spawn={spawnPos}");
                        break;
                    }
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
        private void PlayerSpawn(OldDungeonContext _ctx, GameObject _playerObj)
        {
            Vector2Int psp = roomDistribute.StartSpawnPoint;
            Vector3Int cellPos = new Vector3Int(
                psp.x - _ctx.MapSize.x / 2, 
                psp.y - _ctx.MapSize.y / 2, 
                0);
            //Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0f, 0f, 0f);
            Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);

            _playerObj.transform.position = worldPos;

        }

        // 몬스터 스폰 roomID 
        private void SpawnMonstersInRoom(RoomRuntimeData room)
        {
            if (room == null)
                return;

            if (room.HasSpawnedMonsters)
                return;

            room.HasSpawnedMonsters = true;
            room.AliveMonsterCount = 0;
            room.SpawnedMonsters.Clear();

            List<Vector2Int> spawnPoints = roomDistribute.MonsterSpawnPoint;

            foreach (var spawnPoint in spawnPoints)
            {
                if(!room.RoomInfo.Rect.Contains(spawnPoint))
                    continue;

                Vector3Int cellPos = new Vector3Int(
                    spawnPoint.x - ctx.MapSize.x / 2,
                    spawnPoint.y - ctx.MapSize.y / 2,
                    0
                    );

                Vector3 worldPos = floorTilemap.GetCellCenterLocal(cellPos);

                RectInt rect = room.RoomInfo.Rect;

                for (int i = 0; i < monsterCount; i++)
                {

                    int randomX = Random.Range(rect.xMin + 1, rect.xMax - 1);
                    int randomY = Random.Range(rect.yMin + 1, rect.yMax - 1);

                    Vector3Int randCell = new Vector3Int(
                        randomX - ctx.MapSize.x / 2,
                        randomY - ctx.MapSize.y / 2,
                        0
                        );

                    Vector3 offset = floorTilemap.GetCellCenterWorld(randCell);
                        

                    GameObject monster = Instantiate(
                        monsterPF[0],
                        offset, 
                        Quaternion.identity, 
                        monsterHolder
                        );
                    Monster m = monster.GetComponent<Monster>();
                    if (m != null)
                    {
                        m.Init(room.RoomId, this);
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
                    door.OpenDoor();
                }
            }
        }
       

        // 문 닫기 roomId
        private void CloseRoomDoors(RoomRuntimeData room)
        {
            foreach (var door in room.Doors)
            {
                if (door != null)
                {
                    door.CloseDoor();
                }
            }
        }
     
        // enemyPoint 들어갔을 시
        public void EnterEnemyRoom(int roomId)
        {

            RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);


            if (room == null)
                return;

            if (room.RoomInfo.Type != RoomType.Monster)
            {
                return;
            }


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

            CloseRoomDoors(room);
            SpawnMonstersInRoom(room);
        }

        public void OnMonsterDead(int roomId)
        {
            var room = ctx.RoomStates[roomId];

            room.AliveMonsterCount--;

            //Debug.Log($"[Room {roomId}] Remaining: {room.AliveMonsterCount}");

            if (room.AliveMonsterCount <= 0)
            {
                OnRoomClear(roomId);
            }
        }

        public void OnPlayerDeath()
        {
            gameOverUI.SetActive(true);
        }

        private void OnRoomClear(int roomId)
        {
            var room = ctx.RoomStates[roomId];

            room.IsCleared = true;

            //Debug.Log($"[Room {roomId}] clear");

            foreach (var door in room.Doors)
            {
                if (door != null)
                    door.OpenDoor();
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
