using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;

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


        Vector2Int pos = new Vector2Int();
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            GenerateDoors(ctx);
        }

        // 문 생성 함수
        private void GenerateDoors(DungeonContext ctx)
        {
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    if (IsDoorPosition(x, y))
                    {
                        Vector2Int doorGridPos = new Vector2Int(x, y);
                        int roomId = FindRoomIdFromDoorPos(doorGridPos);

                        if (roomId != -1)
                        {
                            SpawnDoors(doorGridPos, roomId);
                        }
                    }
                }
            }

        }

        private void SpawnDoors(Vector2Int doorGridPos, int roomId)
        {
            Vector3Int cellPos = new Vector3Int(
                doorGridPos.x - ctx.MapSize.x / 2,
                doorGridPos.y - ctx.MapSize.y / 2,
                0
                );
            Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);
            worldPos.z = 0f;

            GameObject obj = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorParent);

            DoorController door = obj.GetComponent<DoorController>();
            if (door == null)
            {
                Debug.LogError("Door prefab에 DoorController가 없습니다.");
                return;
            }
            door.Init(doorGridPos, roomId, false);

            RoomRuntimeData roomState = ctx.RoomStates.Find(r => r.RoomId == roomId);
            if (roomState != null)
            {
                roomState.Doors.Add(door);
            }

            Debug.Log($"[Door] Spawned - GridPos: {doorGridPos}, RoomId: {roomId}, WorldPos: {worldPos}");
        }

        private int FindRoomIdFromDoorPos(Vector2Int doorPos)
        {
            for(int i = 0; i< ctx.RoomStates.Count; i++)
            {
                RectInt rect = ctx.RoomStates[i].RoomInfo.Rect;

                RectInt expanded = new RectInt(
                    rect.xMin - 1,
                    rect.yMin - 1,
                    rect.width + 2,
                    rect.height + 2
                );

                if(expanded.Contains(doorPos))
                    return ctx.RoomStates[i].RoomId;
                   
            }
            return -1;
        }
        private bool IsDoorPosition(int x, int y)
        {
            // 예시: 기존 Door 판정 조건을 여기에 옮기기
            return ctx.MapData[x, y] == TileType.Door;
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
