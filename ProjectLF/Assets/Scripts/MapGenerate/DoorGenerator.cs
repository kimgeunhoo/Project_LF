using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;

namespace BSPDungeonGenrator.Generation
{
    public struct DoorCandidate
    {
        public Vector2Int Pos;
        public bool VerticalBoundary;

        public DoorCandidate(Vector2Int pos, bool verticalBoundary)
        {
            Pos = pos;
            VerticalBoundary = verticalBoundary;
        }
    }

    public class DoorGenerator
    {
        private DungeonContext ctx;
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            GenerateDoors(ctx);
        }

        // 문 생성 함수
        private void GenerateDoors(DungeonContext ctx)
        {
            var candidates = new List<DoorCandidate>();
            for (int x = 1; x < ctx.MapSize.x - 1; x++)
            {
                for (int y = 1; y < ctx.MapSize.y - 1; y++)
                {
                    if (ctx.MapData[x, y] != TileType.Path)
                        continue;

                    // 각 통로 타일이 방과 접해 있는지
                    bool rightRoom = ctx.MapData[x + 1, y] == TileType.Room;
                    bool leftRoom = ctx.MapData[x - 1, y] == TileType.Room;
                    bool upRoom = ctx.MapData[x, y + 1] == TileType.Room;
                    bool downRoom = ctx.MapData[x, y - 1] == TileType.Room;

                    bool verticalBoundary = leftRoom || rightRoom;
                    bool horizontalBoundary = upRoom || downRoom;

                    int roomNeighborCount = 0;
                    
                    if(rightRoom) roomNeighborCount++;
                    if(leftRoom) roomNeighborCount++;
                    if(upRoom) roomNeighborCount++;
                    if(downRoom) roomNeighborCount++;

                    if (roomNeighborCount != 1)
                        continue;

                    if (verticalBoundary == horizontalBoundary)
                        continue;

                    // 통로 방향 판별
                    // 수평 통로는 좌/우, 수직은 상하 Path
                    bool hasLeft = (ctx.MapData[x - 1, y] == TileType.Path);
                    bool hasRight = (ctx.MapData[x + 1, y] == TileType.Path);
                    bool hasDown = (ctx.MapData[x, y - 1] == TileType.Path);
                    bool hasUp = (ctx.MapData[x, y + 1] == TileType.Path);

                    // 통로 방향 판별하고 직각 방향만 측정
                    bool isHorizontalPath = (hasLeft || hasRight) && !hasUp && !hasDown;
                    bool isVerticalPath = (hasUp || hasDown) && !hasLeft && !hasRight;
                    if (!isHorizontalPath && !isVerticalPath)
                        continue;

                    candidates.Add(new DoorCandidate(new Vector2Int(x,y), verticalBoundary));
                }
            }
            foreach (var path in candidates)
            {
                if (ctx.MapData[path.Pos.x, path.Pos.y] == TileType.Path && !HasAdjacentDoor(path.Pos.x, path.Pos.y))
                {
                    ctx.MapData[path.Pos.x, path.Pos.y] = TileType.Door;
                }
            }
        }

        private bool HasAdjacentDoor(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;
                    int nx = x + dx;
                    int ny = y + dy;

                    if (!IsInsideMap(nx, ny))
                        continue;
                    if (ctx.MapData[nx, ny] == TileType.Door)
                        return true;
                }
            }
            return false;
        }

        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }

        private void PlaceDoorVertical(int x, int y)
        {
            for (int w = -ctx.DoorHalfwidth; w <= ctx.DoorHalfwidth; w++)
            {
                int ny = y + w;
                if (!IsInsideMap(x, ny))
                    continue;

                // path 칸만 door 변경
                if (ctx.MapData[x, ny] == TileType.Path)
                {
                    //Debug.Log($"[GenerateDoors] Door created at ({x}, {y})");
                    ctx.MapData[x, ny] = TileType.Door;
                }

            }
        }

        private void PlaceDoorHorizontal(int x, int y)
        {
            for (int w = -ctx.DoorHalfwidth; w <= ctx.DoorHalfwidth; w++)
            {
                int nx = x + w;
                if (!IsInsideMap(nx, y)) continue;

                // path 칸만 door 변경
                if (ctx.MapData[nx, y] == TileType.Path)
                {
                    ctx.MapData[nx, y] = TileType.Door;
                }

            }
        }
    }

}