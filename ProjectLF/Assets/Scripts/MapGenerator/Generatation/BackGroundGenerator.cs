using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using UnityEngine;

public class BackGroundGenerator : MonoBehaviour
{

    private DungeonContext ctx;
    private Vector2Int pos;


    public void Run(DungeonContext _ctx)
    {
        ctx = _ctx;

        pos = new Vector2Int(_ctx.MapSize.x, _ctx.MapSize.y);

        GenerateBackGround(pos);
    }

    private void GenerateBackGround(Vector2Int _pos)
    {
        TileType current = ctx.MapData[_pos.x, _pos.y];
        if (current != TileType.Room && current != TileType.Path)
        {
            ctx.MapData[_pos.x, _pos.y] = TileType.Empty;
        }
    }
}
