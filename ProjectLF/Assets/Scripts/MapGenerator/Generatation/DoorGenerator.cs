using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;
using BSPDungeonGenrator.Utility;



namespace BSPDungeonGenrator.Generation
{
    public class DoorGenerator
    {



        private DungeonContext ctx;

        private CheckInsideMap insideMap = new CheckInsideMap();

        public void Run(DungeonContext _ctx)
        {
            this.ctx = _ctx;

            if (ctx.MapData == null)
            {
                return;
            }

            ctx.DoorPositions.Clear();

            foreach (var candidate in ctx.DoorCandidates)
            {
                Vector2Int doorPos;
                
                if(!GetDoorSpawnPosition(candidate, out doorPos))
                {
                    Debug.LogWarning($"[DoorGenerator] Invalid Door Candidate: {candidate}");
                    continue;
                }

                if(ctx.DoorPositions.Contains(doorPos))
                {
                    continue;
                }

                ctx.MapData[doorPos.x, doorPos.y] = TileType.Door;
                ctx.DoorPositions.Add(doorPos);

            }
            Debug.Log($"[DoorGenerator] DoorPositions Count = {ctx.DoorPositions.Count}");


        }

        private bool GetDoorSpawnPosition(Vector2Int pos, out Vector2Int doorPos)
        {
            Vector2Int[] dirs = new Vector2Int[]
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in dirs)
            {
                Vector2Int a = pos + dir;
                Vector2Int b = pos - dir;

                if (insideMap.IsInsideMap(a) || insideMap.IsInsideMap(b))
                    continue;

                TileType ta = ctx.MapData[a.x, a.y];
                TileType tb = ctx.MapData[b.x, b.y];

                if (ta == TileType.Room && tb == TileType.Path)
                {
                    doorPos = b;
                    return true;
                }
                if (ta == TileType.Path && tb == TileType.Room)
                {
                    doorPos = a;
                    return true;
                }
            }
            doorPos = Vector2Int.zero;
            return false;
        }

        

    }

}
