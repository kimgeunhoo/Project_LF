using BSPDuengeonGenrator.Config;
using BSPDuengeonGenrator.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDuengeonGenrator.Rendering
{
    public class TileMapRenderer : MonoBehaviour
    {
        public void Run(DuengeonContext ctx)
        {
            CreateWallAroundByRoom(ctx);
        }

        private void CreateWallAroundByRoom(DuengeonContext ctx)
        {
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x - ctx.MapSize.x / 2, y - ctx.MapSize.y / 2);

                    if (ctx.MapData[x, y] == TileType.Room)
                    {
                        ctx.FloorTilemap.SetTile(pos, ctx.FloorTile);
                    }
                    else if (ctx.MapData[x, y] == TileType.Wall)
                    {
                        ctx.WallTilemap.SetTile(pos, ctx.WallTile);
                    }
                    else if (ctx.MapData[x, y] == TileType.Door)
                    {
                        ctx.DoorTilemap.SetTile(pos, ctx.DoorTile);
                    }

                }

            }
        }

    }

}