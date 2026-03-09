using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;

namespace BSPDuengeonGenrator.Generation
{
    public class WallGenerator : MonoBehaviour
    {

        private DuengeonContext ctx;
        public void Run(DuengeonContext ctx)
        {
            this.ctx = ctx;
            GeneratedCheckWalls(ctx);
        }

        // 벽 체크 메서드
        private void GeneratedCheckWalls(DuengeonContext ctx)
        {
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {

                    // 주변 8칸 중 바닥이 하나라도 있다면
                    if (ctx.MapData[x, y] == TileType.Room ||
                        ctx.MapData[x, y] == TileType.Path)
                    {
                        CheckWallFind(x, y);
                        //CarveCorriderHorizontal(x, y);
                        //CarveCorriderVertical(x, y);
                    }

                }
            }
        }

        // 벽 생성 메서드
        private void CheckWallFind(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    // 맵 범위 체크
                    if (nx < 0 || ny < 0 || nx >= ctx.MapSize.x || ny >= ctx.MapSize.y)
                    {
                        continue;
                    }

                    // empty를 wall로
                    if (ctx.MapData[nx, ny] == TileType.Empty)
                    {
                        ctx.MapData[nx, ny] = TileType.Wall;
                    }
                }
            }
        }

        
    }

}