using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;

namespace BSPDuengeonGenrator.Generation
{
    public class DoorGenerator : MonoBehaviour
    {
        private DuengeonContext ctx;
        public void Run(DuengeonContext ctx)
        {
            this.ctx = ctx;
            GenerateDoors(ctx);
        }

        // 문 생성 함수
        private void GenerateDoors(DuengeonContext ctx)
        {

            for (int x = 1; x < ctx.MapSize.x - 1; x++)
            {
                for (int y = 1; y < ctx.MapSize.y - 1; y++)
                {
                    if (ctx.MapData[x, y] != TileType.Path) continue;

                    // 통로 타일이 방과 접해 있는지 체크
                    bool hasRoomNeighbor =
                        ctx.MapData[x + 1, y] == TileType.Room ||
                        ctx.MapData[x - 1, y] == TileType.Room ||
                        ctx.MapData[x, y + 1] == TileType.Room ||
                        ctx.MapData[x, y - 1] == TileType.Room;

                    if (!hasRoomNeighbor) continue;

                    // 주변 벽 체크
                    bool surrondedByWall =
                        ctx.MapData[x + 1, y] == TileType.Wall ||
                        ctx.MapData[x - 1, y] == TileType.Wall ||
                        ctx.MapData[x, y + 1] == TileType.Wall ||
                        ctx.MapData[x, y - 1] == TileType.Wall;

                    // 통로 방향 판별
                    // 수평 통로는 좌/우, 수직은 상하 Path
                    bool hasLeft = (ctx.MapData[x - 1, y] == TileType.Path);
                    bool hasRight = (ctx.MapData[x + 1, y] == TileType.Path);
                    bool hasDown = (ctx.MapData[x, y - 1] == TileType.Path);
                    bool hasUp = (ctx.MapData[x, y + 1] == TileType.Path);

                    int horizontal = (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);
                    int vertical = (hasDown ? 1 : 0) + (hasUp ? 1 : 0);

                    // 통로가 수평으로 더 이어져 있다면 문은 세로 3칸
                    // 통로가 수직으로 이어져 있다면 가로 3칸
                    if (horizontal >= vertical)
                    {
                        PlaceDoorVertical(x, y);
                    }
                    else
                    {
                        PlaceDoorHoriaontal(x, y);
                    }

                    if (surrondedByWall)
                    {
                        ctx.MapData[x, y] = TileType.Door;
                    }
                }
            }
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
                if (!IsInsideMap(x, ny)) continue;

                // path 칸만 door 변경
                if (ctx.MapData[x, ny] == TileType.Path)
                {
                    //Debug.Log($"[GenerateDoors] Door created at ({x}, {y})");
                    ctx.MapData[x, ny] = TileType.Door;
                }

            }
        }

        private void PlaceDoorHoriaontal(int x, int y)
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