using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Text.RegularExpressions;



namespace BSPDungeonGenrator.Generation
{
    public class DoorGenerator
    {
   

        private DungeonContext ctx;


        //Vector2Int pos = new Vector2Int();
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            if (ctx.MapData == null)
            {
                Debug.LogError("[DoorGenerator] doorPrefab is NULL");
                return;
            }


            foreach (var pos in ctx.DoorCandidates)
            {
                if(IsValidDoorPosition(pos))
                {
                    ctx.MapData[pos.x, pos.y] = TileType.Door;
                }
            }
        }

        private bool IsValidDoorPosition(Vector2Int pos)
        {
            if (!IsInsideMap(pos))
                return false;

            TileType current = ctx.MapData[pos.x, pos.y];
            if (current!= TileType.Room && current!= TileType.Path)
                return false;

            // »óÇÏ
            if (CheckDoorPair(pos + Vector2Int.up, pos + Vector2Int.down))
                return true;
            // ÁÂ¿ì
            if (CheckDoorPair(pos + Vector2Int.left, pos + Vector2Int.right))
                return true;

            return false;
        }

        private bool CheckDoorPair(Vector2Int a, Vector2Int b)
        {
            if(!IsInsideMap(a) || !IsInsideMap(b))
                return false;

            TileType ta = ctx.MapData[a.x, a.y];
            TileType tb = ctx.MapData[b.x, b.y];

            return (ta == TileType.Room && tb == TileType.Path) ||
                (ta == TileType.Path && tb == TileType.Room);

        }


        private bool IsInsideMap(Vector2Int pos)
        {
            return pos.x >= 0 && pos.y >= 0 && pos.x < ctx.MapSize.x && pos.y < ctx.MapSize.y;
        }
    }

}
