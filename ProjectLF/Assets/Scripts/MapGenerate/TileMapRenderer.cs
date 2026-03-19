using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDungeonGenrator.Rendering
{
    public class TileMapRenderer : MonoBehaviour
    {
        public void Run(DungeonContext ctx)
        {
            CreateWallAroundByRoom(ctx);
        }

        private void CreateWallAroundByRoom(DungeonContext ctx)
        {
           // Debug.Log($"TileMapRender : RoomTiles.Length{ctx.RoomTiles.Length}, PathTiles.Length{ctx.PathTiles.Length}");
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x - ctx.MapSize.x / 2, y - ctx.MapSize.y / 2);

                    if (ctx.MapData[x, y] == TileType.Room)
                    {
                        ctx.FloorTilemap.SetTile(pos, ctx.RoomTiles[Random.Range(0, ctx.RoomTiles.Length)]);
                    }
                    else if (ctx.MapData[x, y] == TileType.Wall)
                    {
                        ctx.WallTilemap.SetTile(pos, ctx.WallTile);
                    }
                    else if (ctx.MapData[x,y] == TileType.Path)
                    {
                        ctx.PathTilemap.SetTile(pos, ctx.PathTiles[Random.Range(0, ctx.PathTiles.Length)]);
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