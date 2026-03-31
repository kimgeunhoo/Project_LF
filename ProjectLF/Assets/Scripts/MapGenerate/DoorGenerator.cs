using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace BSPDungeonGenrator.Generation
{
    public class DoorGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject doorPrefab;
        [SerializeField]
        private Tilemap floorTilemap;
        [SerializeField]
        private Transform doorParent;


        private DungeonContext ctx;


        //Vector2Int pos = new Vector2Int();
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            if (doorPrefab == null)
            {
                Debug.LogError("[DoorGenerator] doorPrefab is NULL");
                return;
            }

            if (floorTilemap == null)
            {
                Debug.LogError("[DoorGenerator] floorTilemap is NULL");
                return;
            }

            if (ctx.RoomStates == null)
            {
                Debug.LogError("[DoorGenerator] ctx.RoomStates is NULL");
                return;
            }
            ClearOldDoors();
            CalculateDoorPositions();
            GenerateDoors(ctx);
        }

        /// <summary>
        /// DoorParent 밑에 생성된 문 제거
        /// </summary>
        /// <param name="ctx"></param>
        private void ClearOldDoors()
        {
            if (doorPrefab != null)
                return;
            for(int i = doorParent.childCount - 1; i >= 0; i--)
            {
                Destroy(doorParent.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 문이 놓일 후보를 Mapdata에 기록
        /// </summary>
        /// <param name="ctx"></param>
        private void CalculateDoorPositions()
        {
            int candidateCount = 0;

            for (int x = 1; x < ctx.MapSize.x - 1; x++)
            {
                for (int y = 1; y < ctx.MapSize.y - 1; y++)
                {
                    if (ctx.MapData[x, y] != TileType.Path)
                        continue;

                    //Debug.Log($"[1] Path at ({x},{y})");
                    // 통로 타일이 방과 접해 있는지 체크
                    bool hasRoomNeighbor =
                        ctx.MapData[x + 1, y] == TileType.Room ||
                        ctx.MapData[x - 1, y] == TileType.Room ||
                        ctx.MapData[x, y + 1] == TileType.Room ||
                        ctx.MapData[x, y - 1] == TileType.Room;

                    if (!hasRoomNeighbor)
                        continue;

                    int roomNeighborCount = 0;

                    // 주변 벽 체크
                    if (ctx.MapData[x + 1, y] == TileType.Room)
                        roomNeighborCount++;
                    if (ctx.MapData[x - 1, y] == TileType.Room)
                        roomNeighborCount++;
                    if (ctx.MapData[x, y + 1] == TileType.Room)
                        roomNeighborCount++;
                    if (ctx.MapData[x, y - 1] == TileType.Room)
                        roomNeighborCount++;
                    if (roomNeighborCount > 3)
                        continue;

                    candidateCount++;

                    bool hasLeft = (ctx.MapData[x - 1, y] == TileType.Path);
                    bool hasRight = (ctx.MapData[x + 1, y] == TileType.Path);
                    bool hasDown = (ctx.MapData[x, y - 1] == TileType.Path);
                    bool hasUp = (ctx.MapData[x, y + 1] == TileType.Path);

                    int horizontal = (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);
                    int vertical = (hasDown ? 1 : 0) + (hasUp ? 1 : 0);

                    if (horizontal >= vertical)
                    {
                        MarkDoorVertical(x, y);
                    }
                    else
                    {
                        MarkDoorHorizontal(x, y);
                    }
                }
            }
        }

        private void MarkDoorVertical(int x, int y)
        {
            for (int w = -ctx.DoorHalfwidth; w <= ctx.DoorHalfwidth; w++)
            {
                int ny = y + w;

                if (!IsInsideMap(x, ny))
                    continue;
                if (ctx.MapData[x, ny] == TileType.Path)
                {
                    Vector2Int pos = new Vector2Int(x, ny);
                    ctx.MapData[x, ny] = TileType.Door;

                }
            }

        }
        private void MarkDoorHorizontal(int x, int y)
        {
            for (int w = -ctx.DoorHalfwidth; w <= ctx.DoorHalfwidth; w++)
            {
                int nx = x + w;

                if (!IsInsideMap(nx, y))
                    continue;
                if (ctx.MapData[nx, y] == TileType.Path)
                {
                    Vector2Int pos = new Vector2Int(nx, y);
                    ctx.MapData[nx, y] = TileType.Door;

                }
            }
        }


        // 문 생성 함수
        private void GenerateDoors(DungeonContext ctx)
        {
            int spawnCount = 0;
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    if (!IsDoorPosition(x, y))
                        continue;

                    Vector2Int doorGridPos = new Vector2Int(x, y);
                    int roomId = FindRoomIdFromDoorPos(doorGridPos);

                    if(roomId == -1)
                        continue;

                    SpawnDoors(doorGridPos, roomId);
                    spawnCount++;
                }
            }
        }


   
        private void SpawnDoors(Vector2Int doorGridPos, int roomId)
        {
         //   Debug.Log($"[SpawnDoors] 1. Start / pos={doorGridPos}, roomId={roomId}");
            Vector3Int cellPos = 
                new Vector3Int(doorGridPos.x - ctx.MapSize.x / 2, doorGridPos.y - ctx.MapSize.y / 2, 0);
         //   Debug.Log($"[SpawnDoors] 2. cellPos={cellPos}");
         //   Debug.Log($"[SpawnDoors] 3. floorTilemap null? {floorTilemap == null}");
         //   Debug.Log($"[SpawnDoors] 4. doorPrefab null? {doorPrefab == null}");

            Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);
            worldPos.z = 0f;

        //    Debug.Log($"[SpawnDoors]5 doorPrefab = {(doorPrefab == null ? "NULL" : doorPrefab.name)}");
            GameObject obj = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorParent);
        //    Debug.Log($"[SpawnDoors]6 spawned obj = {(obj == null ? "NULL" : obj.name)}");
            DoorController door = obj.GetComponent<DoorController>();
          //  Debug.Log($"[SpawnDoors]7 DoorController = {(door == null ? "NULL" : "FOUND")}");
            if (door == null) 
            { 
                Debug.LogError("Door prefab에 DoorController가 없습니다.");
                return; 
            }
          //  Debug.Log($"[SpawnDoors] 8. Before Init");
            door.Init(doorGridPos, roomId, false);
           // Debug.Log($"[SpawnDoors] 8. Before Init");

            RoomRuntimeData roomState = ctx.RoomStates.Find(r => r.RoomId == roomId);
           // Debug.Log($"[SpawnDoors] 10. roomState null? {roomState == null}");
           // Debug.Log($"[SpawnDoors] 11. roomState.Doors null? {roomState?.Doors == null}");
            if (roomState != null)
            {
          //      Debug.LogWarning("Door 상태 생성");
                roomState.Doors.Add(door);
            }
            //Debug.Log($"[Door] Spawned - GridPos: {doorGridPos}, RoomId: {roomId}, WorldPos: {worldPos}");
        }

        private int FindRoomIdFromDoorPos(Vector2Int doorPos)
        {
            //Debug.Log($"[FindRoomId] Start doorPos={doorPos}, RoomStates.Count={ctx.RoomStates.Count}");

            for (int i = 0; i< ctx.RoomStates.Count; i++)
            {
                RectInt rect = ctx.RoomStates[i].RoomInfo.Rect;

                RectInt expanded = new RectInt(
                    rect.xMin - 1,
                    rect.yMin - 1,
                    rect.width + 2,
                    rect.height + 2
                );

                //Debug.Log(
                //$"[FindRoomId] Check i={i}, RoomId={ctx.RoomStates[i].RoomId}, " +
                //$"Rect={rect}, Expanded={expanded}, Contains={expanded.Contains(doorPos)}");

                if (expanded.Contains(doorPos))
                {
             ///       Debug.Log($"[FindRoomId] MATCH doorPos={doorPos} -> RoomId={ctx.RoomStates[i].RoomId}");
                    return ctx.RoomStates[i].RoomId;
                }
                   
            }
            //Debug.LogWarning($"[FindRoomId] FAIL doorPos={doorPos}");
            return -1;
        }
        private bool IsDoorPosition(int x, int y)
        {
            // 예시: 기존 Door 판정 조건을 여기에 옮기기
            return ctx.MapData[x, y] == TileType.Door;
        }



        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }

    }

}

//-------------------------------- 타일맵 방식 -----------------------------------
//List<Vector2Int> doorCandidate = new List<Vector2Int>();
//for (int x = 1; x < ctx.MapSize.x - 1; x++)
//{
//    for (int y = 1; y < ctx.MapSize.y - 1; y++)
//    {
//        int roomNeighborCount = 0;
//        if (ctx.MapData[x, y] != TileType.Path) 
//            continue;
//        // 통로 타일이 방과 접해 있는지 체크
//        bool hasRoomNeighbor =
//            ctx.MapData[x + 1, y] == TileType.Room ||
//            ctx.MapData[x - 1, y] == TileType.Room ||
//            ctx.MapData[x, y + 1] == TileType.Room ||
//            ctx.MapData[x, y - 1] == TileType.Room;

//        if (!hasRoomNeighbor) 
//            continue;

//        // 주변 벽 체크
//        if (ctx.MapData[x + 1, y] == TileType.Room) 
//            roomNeighborCount++; 
//        if (ctx.MapData[x - 1, y] == TileType.Room) 
//            roomNeighborCount++; 
//        if (ctx.MapData[x, y + 1] == TileType.Room) 
//            roomNeighborCount++; 
//        if (ctx.MapData[x, y - 1] == TileType.Room) 
//            roomNeighborCount++; 
//        if (roomNeighborCount > 3) 
//            continue;

//        // 주변 벽 체크
//        bool surrondedByWall = 
//            ctx.MapData[x + 1, y] == TileType.Wall ||
//            ctx.MapData[x - 1, y] == TileType.Wall ||
//            ctx.MapData[x, y + 1] == TileType.Wall || 
//            ctx.MapData[x, y - 1] == TileType.Wall;

//        // 통로 방향 판별 
//        // 수평 통로는 좌/우, 수직은 상하 Path
//        bool hasLeft = (ctx.MapData[x - 1, y] == TileType.Path);
//        bool hasRight = (ctx.MapData[x + 1, y] == TileType.Path);
//        bool hasDown = (ctx.MapData[x, y - 1] == TileType.Path);
//        bool hasUp = (ctx.MapData[x, y + 1] == TileType.Path);

//        int horizontal = (hasLeft ? 1 : 0) + (hasRight ? 1 : 0); 
//        int vertical = (hasDown ? 1 : 0) + (hasUp ? 1 : 0);

//        if (horizontal >= vertical) 
//        { 
//            PlaceDoorVertical(x, y); 
//        } 
//        else
//        { 
//            PlaceDoorHorizontal(x, y); 
//        }
//        if (surrondedByWall) 
//        {
//            ctx.MapData[x, y] = TileType.DoorOpen;
//        }

//    }
//}
// 맵 범위 체크 (예외방지)
//private bool IsInsideMap(int x, int y)
//{
//    return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
//}

//private void PlaceDoorVertical(int x, int y)
//{

//    for (int w = -ctx.DoorHalfwidth; w <= ctx.DoorHalfwidth; w++)
//    {
//        int ny = y + w; 
//        if (!IsInsideMap(x, ny)) 
//            continue;


//        if (ctx.MapData[x, ny] == TileType.Path)
//        {
//            //Debug.Log($"[GenerateDoors] Door created at ({x}, {y})");
//            ctx.MapData[x, ny] = TileType.DoorClosed;
//            pos.x = x;
//            pos.y = ny;
//            //DoorInfo door = new DoorInfo
//            //{
//            //    GridPos = pos,
//            //    RoomId = roomId,
//            //    IsOpen = false
//            //};

//           //NullByDoor(x, ny);
//        }

//    }
//}

//private void PlaceDoorHorizontal(int x, int y)
//{
//    for (int w = -ctx.DoorHalfwidth; w <= ctx.DoorHalfwidth; w++)
//    {
//        int nx = x + w; 
//        if (!IsInsideMap(nx, y)) 
//            continue;

//        // Path 칸만 Door로
//        if (ctx.MapData[nx, y] == TileType.Path) 
//        { 
//            ctx.MapData[nx, y] = TileType.DoorClosed;
//            //NullByDoor(nx, y);
//        }
//    }
//}
