using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Core;

namespace BSPDungeonGenrator.Config
{
    // 타일 타입 정의
    public enum TileType
    {
        Empty, // 0 빈 공간
        Room, // 바닥
        Path, // 통로
        Wall, // 벽
        Door // 문
    }

    // 방 타입 정의
    public enum RoomType
    {
        Start, // 스폰 포인트
        Stairs, // 계단
        Shop, // 상점
        Encounter, // 랜덤 인카운터
        Monster, // 몬스터 룸
    }


    [CreateAssetMenu(menuName ="Duengeon/Data")]
    public class DungeonData : ScriptableObject
    {
        
        // 맵 사이즈
        [Header("Map Size")]
        [SerializeField]
        private Vector2Int mapSize;

        public Vector2Int MapSize { get { return mapSize; } }

        // 노드 값이 라인의 갯수를 판별
        [Header("Node Value")]
        [SerializeField]
        private int maxNode;
        [SerializeField]
        private int minNode;

        public int MaxNode { get { return maxNode; } }
        public int MinNode { get { return minNode; } }

        // 사각형 분리
        [Header("Room Magnification")]
        [SerializeField]
        private float minDivideSize;
        [SerializeField]
        private float maxDivideSize;

        public float MinDivideSize { get { return minDivideSize; } }
        public float MaxDivideSize { get { return maxDivideSize; } }

        // 바닥과 벽을 정의
        [Header("Wall, FloorTile, Door")]

        [SerializeField]
        private TileBase floorTile;
        [SerializeField]
        private TileBase wallTile;
        [SerializeField]
        private TileBase doorTile;
        [SerializeField]
        private TileBase pathTile;
        [SerializeField]
        private TileBase openDoorTile;

        [SerializeField]
        private int doorHalfwidth;

        public int DoorHalfwidth { get { return doorHalfwidth; } }

        // 타일맵
        [Header("Random TileArray : Path")]
        [SerializeField]
        private TileBase[] pathTiles;

        [Header("Random TileArray : Room")]
        [SerializeField]
        private TileBase[] roomTiles;


        public TileBase FloorTile { get { return floorTile; } }
        public TileBase WallTile { get { return wallTile; } }
        public TileBase DoorTile { get { return doorTile; } }
        public TileBase OpenDoorTile { get { return openDoorTile; } }
        public TileBase PathTile { get { return pathTile; } }
        public TileBase[] PathTiles { get { return pathTiles; } }

        public TileBase[] RoomTiles { get { return roomTiles; } }


        // 맵데이터
        [SerializeField]
        private TileType[,] mapData;

        public TileType[,] MapData { get { return mapData; } }

        // 라인 그리기
        [Header("Random Liner")]
        [SerializeField]
        private GameObject line;
        [SerializeField]
        private GameObject rectangle;
        [SerializeField]
        private GameObject lineRenderer;

        public GameObject Line { get { return line; } }
        public GameObject Rectangle { get { return rectangle; } }
        public GameObject LineRenderer { get { return lineRenderer; } }

    }
    public class RoomInfo
    {
        private RectInt m_rect;
        //private RoomType m_type;
        public RoomType Type { get; set; }

        private TreeNode m_tree;

        public RoomInfo(RectInt rect)
        {
            this.m_rect = rect;
        }
        public RectInt Rect { get { return m_rect; } }

        public Vector2Int Center =>
            new Vector2Int(m_rect.xMin + m_rect.width / 2, m_rect.yMin + m_rect.height / 2);
        //Vector3Int cellPos = new Vector3Int(c.x - ctx.MapSize.x / 2, c.y - ctx.MapSize.y / 2, 0);
    }

}
