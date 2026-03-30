using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Generation;

namespace BSPDungeonGenrator.Core
{
    public sealed class DungeonContext
    {
        public Vector2Int MapSize;
        public TileType[,] MapData;
        // 방 상태
        public List<RoomRuntimeData> RoomStates;


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
        public Tilemap OpenDoorTileMap;
        public Tilemap PathTilemap;


        public TileBase FloorTile;
        public TileBase WallTile;
        public TileBase DoorTile;
        public TileBase OpenDoorTile;
        public TileBase PathTile;

        public TileBase[] PathTiles;
        public TileBase[] RoomTiles;

        // Leaf 경계, 방 사이와의 최소 거리
        public int RoomPadding = 1;
        public int MinRoomWidth = 10;
        public int MinRoomHeight = 10;


        // Drawer가 읽는 값
        public List<LineSegment> SplitLines = new List<LineSegment>();

        // 결과물
        public List<RoomInfo> Rooms = new List<RoomInfo>();

        public Vector2Int StartPoint;
        public Vector2Int StairPoint;
        public Vector2Int ShopPoint;
        public List<Vector2Int> EncounterPoints = new List<Vector2Int>();
        public List<Vector2Int> MonsterPoints = new List<Vector2Int>();

    }

}