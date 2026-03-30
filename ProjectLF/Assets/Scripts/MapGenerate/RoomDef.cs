using BSPDungeonGenrator;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.marker;
using UnityEngine;

namespace BSPDungeonGenrator.marker
{
    public class RoomDef
    {
        private DungeonContext ctx;

        public void Run(DungeonContext _ctx)
        {
            this.ctx = _ctx;
            SpawnRoomPoint(ctx);
        }
        private void SpawnRoomPoint(DungeonContext ctx)
        {
            foreach (var room in ctx.Rooms)
            {
                Vector2Int center = room.Center;
                switch (room.Type)
                {
                    case RoomType.Start:
                        ctx.StartPoint = center;
                        //Debug.Log($"[SpawnPoint] ctx.StartPoint {ctx.StartPoint}");
                        break;
                    case RoomType.Stairs:
                        ctx.StairPoint = center;
                        break;
                    case RoomType.Shop:
                        ctx.ShopPoint = center;
                        break;
                    case RoomType.Encounter:
                        ctx.EncounterPoints.Add(center);
                        break;
                    case RoomType.Monster:
                        ctx.MonsterPoints.Add(center);
                       // Debug.Log($"[SpawnPoint] Monster Ãß°¡: {center}");
                        break;
                    default:
                        break;
                }
            }
        }
    }

}
