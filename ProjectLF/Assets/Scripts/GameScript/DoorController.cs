using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;


public class DoorController : MonoBehaviour
{
    private void OpenDoor(DungeonContext ctx, DoorInfo door)
    {
        if (door == null || door.IsOpen)
            return;

        door.IsOpen = true;

        ctx.MapData[door.GridPos.x, door.GridPos.y] = TileType.DoorOpen;

        Vector3Int cellPos =
            new Vector3Int(door.GridPos.x - ctx.MapSize.x / 2, door.GridPos.y - ctx.MapSize.y / 2, 0);
        ctx.DoorTilemap.SetTile(cellPos, ctx.OpenDoorTile);
    }

    private void CloseDoor(DungeonContext ctx, DoorInfo door)
    {
        if (door == null || !door.IsOpen)
            return;
        door.IsOpen = false;

        ctx.MapData[door.GridPos.x, door.GridPos.y] = TileType.DoorClosed;

        Vector3Int cellPos =
           new Vector3Int(door.GridPos.x - ctx.MapSize.x / 2, door.GridPos.y - ctx.MapSize.y / 2, 0);

        ctx.DoorTilemap.SetTile(cellPos, ctx.DoorTile);
    }

    //private void RefreshDoorCollision(DungeonContext ctx)
    //{
    //    ctx.DoorTilemapCollider
    //}
}
