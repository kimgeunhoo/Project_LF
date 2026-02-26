using UnityEngine;
using UnityEngine.Tilemaps;

public class DuengeonData : MonoBehaviour
{
    public enum TileType
    {
        Empty, // 0 빈 공간
        Room, // 바닥
        Path, // 통로
        Wall, // 벽
        Door, // 문
    }

    public enum RoomType
    {
        Start, // 스폰 포인트
        Stairs, // 계단
        Shop, // 상점
        Encounter, // 랜덤 인카운터
        Monster, // 몬스터 룸
    }

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
    public int MinNode { get { return maxNode; } }

    [Header("Room Magnification")]
    [SerializeField]
    private float minDivideSize;
    [SerializeField]
    private float maxDivideSize;

    public float MinDivideSize { get { return minDivideSize; } }
    public float MaxDivideSize { get { return maxDivideSize; } }

    [Header("Wall, FloorTile, Door")]
    // 바닥과 벽을 정의
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private Tilemap wallTilemap;
    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    private TileBase wallTile;
    [SerializeField]
    private TileBase doorTile;

    [Header("Random TileArray")]
    [SerializeField]
    private TileBase[] pathTiles;

    public Tilemap FloorTilemap { get { return floorTilemap; } }
    public Tilemap WallTilemap { get { return wallTilemap; } }

    public TileBase FloorTile { get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }   
    public TileBase DoorTile { get { return doorTile; } }

    public TileBase[] PathTiles { get { return pathTiles; } }

    // 맵데이터
    [SerializeField]
    private TileType[,] mapData;

    public TileType[,] MapData { get { return mapData; } }


    [Header("Random Liner")]
    [SerializeField]
    private GameObject line;
    [SerializeField]
    private Transform lineHolder;
    [SerializeField]
    private GameObject rectangle;
    [SerializeField]
    private GameObject lineRenderer;

    public GameObject Line { get { return line; } }
    public Transform LineHolder { get { return lineHolder; } }
    public GameObject Rectangle { get { return rectangle; } }
    public GameObject LineRenderer { get  { return lineRenderer; } }

    public class RoomInfo
    {
        private RectInt rect;
        private RoomType type;

        public RoomInfo(RectInt rect)
        {
            this.rect = rect;
            this.type = RoomType.Start;

        }

        public Vector2Int Center =>
            new Vector2Int(rect.x + rect.width / 2, rect.y + rect.height / 2);
    }

    private void DungeonConfig()
    {

    }

}
