using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Generation;

namespace BSPDungeonGenrator.Core
{
    public sealed class DungeonContext
    {
        public struct RoomConnection
        {
            public int FromRoomId;
            public int ToRoomId;

            public RoomConnection(int _fromRoomId, int _toRoomId)
            {
                FromRoomId = _fromRoomId;
                ToRoomId = _toRoomId;
            }
        }

        /// <summary>
        /// Settings
        /// </summary>
        public Vector2Int MapSize;
        public int MaxNode;
        public int MinNode;
        public float MinDivideSize;
        public float MaxDivideSize;
        public int DoorHalfwidth;

        /// <summary>
        /// Leaf 경계, 방 사이와의 최소 거리
        /// </summary>
        public int RoomPadding = 5;
        public int MinRoomWidth = 12;
        public int MinRoomHeight = 12;
        public List<RoomConnection> RoomConnections = new List<RoomConnection>();

        /// <summary>
        /// 런타임 데이터 생성
        /// </summary>
        public TileType[,] MapData;
        public TreeNode Root;
        public List<LineSegment> SplitLines = new List<LineSegment>();
        public List<RoomInfo> Rooms = new List<RoomInfo>();

        /// <summary>
        /// 문 위치 생성
        /// </summary>
        public List<Vector2Int> DoorPositions = new List<Vector2Int>();

        /// <summary>
        /// 중간값
        /// </summary>
        public HashSet<Vector2Int> CorriderCells = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> WallCells = new HashSet<Vector2Int>();

        /// <summary>
        /// 렌더링 리소스 생성
        /// </summary>
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


        /// <summary>
        /// 레이아웃 결과물
        /// </summary>
        public Vector2Int StartPoint;
        public Vector2Int StairPoint;
        public Vector2Int ShopPoint;
        public List<Vector2Int> EncounterPoints = new List<Vector2Int>();
        public List<Vector2Int> MonsterPoints = new List<Vector2Int>();


        /// <summary>
        /// 런타임 방 상태
        /// </summary>
        public List<RoomRuntimeData> RoomStates;


    }

}