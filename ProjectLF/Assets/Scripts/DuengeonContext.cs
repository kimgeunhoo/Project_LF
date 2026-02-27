using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Config;

namespace BSPDuengeonGenrator.Core
{
    public sealed class DuengeonContext : MonoBehaviour
    {
        public Vector2Int MapSize;
        public TileType[,] MapData;

        public int MaxNode;
        public int MinNode;
        public float MinDivideSize;
        public float MaxDivideSize;

        public BspTree.TreeNode Root;

        // 렌더 리소스
        public Tilemap FloorTilemap;
        public Tilemap WallTilemap;

        public TileBase FloorTile;
        public TileBase WallTile;
        public TileBase DoorTile;
        public TileBase[] PathTiles;

        // 결과물
        public List<DuengeonData.RoomInfo> Rooms = new List<DuengeonData.RoomInfo>();
    }

}