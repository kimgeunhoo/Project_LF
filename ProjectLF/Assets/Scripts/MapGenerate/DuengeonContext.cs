using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Config;

namespace BSPDuengeonGenrator.Core
{
    public sealed class DuengeonContext
    {
        public Vector2Int MapSize;
        public TileType[,] MapData;

        // 노드 사이즈
        public int MaxNode;
        public int MinNode;
        public float MinDivideSize;
        public float MaxDivideSize;
        public int DoorHalfwidth;

        public TreeNode Root;

        // 렌더 리소스
        public Tilemap FloorTilemap;
        public Tilemap WallTilemap;
        public Tilemap DoorTilemap;
        public Tilemap PathTilemap;

        public TileBase FloorTile;
        public TileBase WallTile;
        public TileBase DoorTile;
        public TileBase PathTile;
        public TileBase[] PathTiles;
        public TileBase[] RoomTiles;

        // Drawer가 읽는 값
        public List<LineSegment> SplitLines = new List<LineSegment>();

        // 결과물
        public List<RoomInfo> Rooms = new List<RoomInfo>();
    }

}