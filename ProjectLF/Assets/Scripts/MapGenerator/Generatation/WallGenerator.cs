using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;

namespace BSPDungeonGenrator.Generation
{
    public class WallGenerator
    {

        private DungeonContext ctx;
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            ctx.WallCells.Clear();
            CollectWallCandidates();
            //GeneratedCheckWalls(ctx);
        }

        private void CollectWallCandidates()
        {
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    bool isRoom = ctx.MapData[x, y] == TileType.Room;
                    bool isCorrider = ctx.CorriderCells.Contains(new Vector2Int(x, y));

                    if(isRoom || isCorrider)
                    {
                        CollectWallAround(x, y);
                    }
                }
            }
        }

        private void CollectWallAround(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if(!IsInsideMap(nx, ny))
                        continue;

                    if (ctx.MapData[nx, ny] != TileType.Empty)
                        continue;

                    Vector2Int pos = new Vector2Int(nx, ny);

                    if (ctx.CorriderCells.Contains(pos))
                        continue;

                    ctx.WallCells.Add(pos);
                }
            }
        }
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }

    }

}