using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using UnityEngine;


namespace BSPDungeonGenrator.Generation
{
    public class MapDataPainter : MonoBehaviour
    {
        public void Run(DungeonContext _ctx)
        {
            PaintCorriders(_ctx);
            PaintWalls(_ctx);
        }

        private void PaintCorriders(DungeonContext _ctx)
        {
            foreach (var pos in _ctx.CorriderCells)
            {
                if (!IsInsideMap(_ctx, pos))
                    continue;
                if (_ctx.MapData[pos.x, pos.y] == TileType.Room)
                    continue;

                _ctx.MapData[pos.x, pos.y] = TileType.Path;
            }
        }

        private void PaintWalls(DungeonContext _ctx)
        {
            foreach (var pos in _ctx.WallCells)
            {
                if (!IsInsideMap(_ctx, pos))
                    continue;

                if (_ctx.MapData[pos.x, pos.y] != TileType.Empty)
                    continue;

                _ctx.MapData[pos.x, pos.y] = TileType.Wall;
            }
        }


        private bool IsInsideMap(DungeonContext ctx, Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 &&
                   pos.x < ctx.MapSize.x && pos.y < ctx.MapSize.y;
        }
    }
}


