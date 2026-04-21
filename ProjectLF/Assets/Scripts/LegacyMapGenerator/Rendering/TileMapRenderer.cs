using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDungeonGenrator.Rendering
{
    public class TileMapRenderer : MonoBehaviour
    {
        public void Render(
            TileType[,] mapData,
            Vector2Int mapSize,
            TilemapRenderRefs renderRefs,
            TileAssetRefs assetRefs
            )
        {
            if (mapData == null || renderRefs == null || assetRefs == null)
                return;

            ClearAll(renderRefs);

            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    Vector3Int pos = new Vector3Int(
                        x - mapSize.x / 2,
                        y - mapSize.y / 2,
                        0
                        );
                    switch (mapData[x, y])
                    {
                        case TileType.Room:
                            if (assetRefs.RoomTiles != null && assetRefs.RoomTiles.Length > 0)
                            {
                                renderRefs.FloorTilemap.SetTile(
                                    pos,
                                    assetRefs.RoomTiles[Random.Range(0, assetRefs.RoomTiles.Length)]
                                );
                            }
                            else
                            {
                                renderRefs.FloorTilemap.SetTile(pos, assetRefs.FloorTile);
                            }
                            break;

                        case TileType.Path:
                            if (assetRefs.PathTiles != null && assetRefs.PathTiles.Length > 0)
                            {
                                renderRefs.PathTilemap.SetTile(
                                    pos,
                                    assetRefs.PathTiles[Random.Range(0, assetRefs.PathTiles.Length)]
                                );
                            }
                            else
                            {
                                renderRefs.PathTilemap.SetTile(pos, assetRefs.PathTile);
                            }
                            break;

                        case TileType.Wall:
                            renderRefs.WallTilemap.SetTile(pos, assetRefs.WallTile);
                            break;

                        case TileType.Door:
                            renderRefs.DoorTilemap.SetTile(pos, assetRefs.DoorTile);
                            break;

                        case TileType.Empty:
                            if (assetRefs.BackgroundTiles != null && assetRefs.BackgroundTiles.Length > 0)
                            {
                                renderRefs.BackgroundTilemap.SetTile(
                                    pos,
                                    assetRefs.BackgroundTiles[Random.Range(0, assetRefs.BackgroundTiles.Length)]
                                );
                            }
       
                            break;

                    }

                }

            }

        }

        // 예전 테두리 벽 생성
        private void CreateWallAroundByRoom(OldDungeonContext ctx)
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
        private void ClearAll(TilemapRenderRefs renderRefs)
        {
            renderRefs.FloorTilemap?.ClearAllTiles();
            renderRefs.WallTilemap?.ClearAllTiles();
            renderRefs.PathTilemap?.ClearAllTiles();
            renderRefs.DoorTilemap?.ClearAllTiles();
            renderRefs.OpenDoorTileMap?.ClearAllTiles();
        }

    }

}