using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;



namespace BSPDungeonGenrator.Generation
{
    public class DoorGenerator : MonoBehaviour
    {
        private struct DoorCandidate
        {
            public Vector2Int Pos;
            public bool IsVertical;
            public Vector2Int RoomDir;
            public DoorCandidate(Vector2Int pos, bool isVertical, Vector2Int roomDir)
            {
                Pos = pos;
                IsVertical = isVertical;
                RoomDir = roomDir;
            }

        }

        private DungeonContext ctx;


        //Vector2Int pos = new Vector2Int();
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            if (ctx.MapData == null)
            {
                Debug.LogError("[DoorGenerator] doorPrefab is NULL");
                return;
            }

            ClearOldDoorTiles();
            CalculateDoorPositions();
        }

        private void ClearOldDoorTiles()
        {
            ctx.DoorPositions.Clear();
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    if (ctx.MapData[x, y] == TileType.Door)
                    {
                        // 기존 Door를 Path로 돌리기
                        ctx.MapData[x, y] = TileType.Path;
                    }
                }
            }
        }

        /// <summary>
        /// 문이 놓일 후보를 Mapdata에 기록
        /// </summary>
        /// <param name="ctx"></param>
        private void CalculateDoorPositions()
        {
            List<DoorCandidate> candidates = new List<DoorCandidate>();

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
                    if (roomNeighborCount != 1)
                        continue;

                    // 경로 체크
                    bool hasLeft = (ctx.MapData[x - 1, y] == TileType.Path);
                    bool hasRight = (ctx.MapData[x + 1, y] == TileType.Path);
                    bool hasDown = (ctx.MapData[x, y - 1] == TileType.Path);
                    bool hasUp = (ctx.MapData[x, y + 1] == TileType.Path);

                    int horizontal = (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);
                    int vertical = (hasDown ? 1 : 0) + (hasUp ? 1 : 0);

                    bool isStraightHorizontal = horizontal == 2 && vertical == 0;
                    bool isStraightVertical = vertical == 2 && horizontal == 0;

                    if (!isStraightHorizontal && !isStraightVertical)
                        continue;

                    bool isVerticalDoor = isStraightHorizontal;

                    Vector2Int roomDir = Vector2Int.zero;
                    if (ctx.MapData[x - 1, y] == TileType.Room) roomDir = Vector2Int.left;
                    else if (ctx.MapData[x + 1, y] == TileType.Room) roomDir = Vector2Int.right;
                    else if (ctx.MapData[x, y - 1] == TileType.Room) roomDir = Vector2Int.down;
                    else if (ctx.MapData[x, y + 1] == TileType.Room) roomDir = Vector2Int.up;

                    candidates.Add(new DoorCandidate(new Vector2Int(x, y), isVerticalDoor, roomDir));
                }
            }

            bool[] visited = new bool[candidates.Count];

            for(int i = 0; i < candidates.Count; i++)
            {
                if (visited[i]) 
                    continue;

                List<DoorCandidate> group = CollectConnectedCandidates(candidates, i, visited);

                if(group.Count == 0) 
                    continue;

                DoorCandidate selected = SelectMiddleCandidate(group);

                ctx.MapData[selected.Pos.x, selected.Pos.y] = TileType.Door;
                ctx.DoorPositions.Add(selected.Pos);
            }    
        }

        private List<DoorCandidate> CollectConnectedCandidates(List<DoorCandidate> _candidates, int _startIndex, bool[] _visited)
        {
            List<DoorCandidate> group = new List<DoorCandidate>();
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(_startIndex);
            _visited[_startIndex] = true;

            while (queue.Count > 0)
            {
                int idx = queue.Dequeue();
                DoorCandidate cur = _candidates[idx];
                group.Add(cur);

                for (int i = 0; i < _candidates.Count; i++)
                {
                    if (_visited[i]) 
                        continue;

                    DoorCandidate other = _candidates[i];
                    if (other.IsVertical != cur.IsVertical)
                        continue;
                    if(other.RoomDir != cur.RoomDir)
                        continue;

                    bool connected = cur.IsVertical
                        ? (other.Pos.x == cur.Pos.x && Mathf.Abs(other.Pos.y - cur.Pos.y) == 1)
                        : (other.Pos.y == cur.Pos.y && Mathf.Abs(other.Pos.x - cur.Pos.x) == 1);

                    if(!connected)
                        continue;

                    _visited[i] = true;
                    queue.Enqueue(i);
                }
            }
            return group;
        }
        private DoorCandidate SelectMiddleCandidate(List<DoorCandidate> group)
        {
            if (group[0].IsVertical)
                group.Sort((a, b) => a.Pos.y.CompareTo(b.Pos.y));
            else
                group.Sort((a, b) => a.Pos.x.CompareTo(b.Pos.x));

            return group[group.Count / 2];
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
