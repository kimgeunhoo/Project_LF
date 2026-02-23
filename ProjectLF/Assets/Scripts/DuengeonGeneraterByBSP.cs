using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDuengeonGenrator
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

    public class RoomInfo
    {
        private RectInt rect;
        private RoomType type;

        private TreeNode m_tree;

        public RoomInfo(RectInt rect)
        {
            this.rect = rect;
            this.type = RoomType.Start;

        }

        public Vector2Int Center =>
            new Vector2Int(rect.x + rect.width / 2, rect.y + rect.height / 2);
    }



    public class TreeNode
    {
        public TreeNode leftTree;
        public TreeNode rightTree;
        public TreeNode parentTree;
        // RectInt 
        // 정수 좌표(x, y)와 크기(width, height)로 정의되는 2D 직사각형 구조체
        public RectInt treeSize;
        public RectInt dungeonSize;


        private TileBase[] RoomTiles;

        // 맵 데이터 생성, 초기화
        //private int[,] mapData = new int[mapSize.x, mapSize.y];
        // 0 = 빈공간
        // 1 = 바닥
        // 2 = 벽

        public TreeNode(int _x, int _y, int _width, int _height)
        {
            treeSize.x = _x;
            treeSize.y = _y;
            treeSize.width = _width;
            treeSize.height = _height;
        }
    }

    public class DuengeonGeneraterByBSP : MonoBehaviour
    {
        [Header("Map Size")]
        [SerializeField]
        private Vector2Int mapSize;

        // 노드 값이 라인의 갯수를 판별
        [Header("Node Value")]
        [SerializeField]
        private int maxNode;
        [SerializeField]
        private int minNode;

        [Header("Room Magnification")]
        [SerializeField]
        private float minDivideSize;
        [SerializeField]
        private float maxDivideSize;

        [Header("Random Liner")]
        [SerializeField]
        private GameObject line;
        [SerializeField]
        private Transform lineHolder;
        [SerializeField]
        private GameObject rectangle;
        [SerializeField]
        private GameObject LineRenderer;

        // 타일맵 배치
        [Header("Tile")]
        [SerializeField]
        private Tile tile;
        // 타일맵 랜덤변수
        [SerializeField]
        private Tilemap tilemap;

        [Header("Random TileArray")]
        [SerializeField]
        private TileBase[] PathTiles;

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

        [SerializeField]
        private int doorHalfwidth;

        [Header("SpawnPoint")]
        [SerializeField]
        private Vector3 spawnPoint;

        // 맵 데이터 배열 생성, 초기화
        private TileType[,] mapData;
        // 0 = 빈공간
        // 1 = 바닥
        // 2 = 벽

        private void Awake()
        {
            // 벽과 관련된 데이터 배열 생성
            InitializeMap();
            // 던전 사이즈에 맞게 벽을 그림
            OnDrawRectangle(0, 0, mapSize.x, mapSize.y);
            // 루트가 될 트리 생성
            TreeNode rootNode = new TreeNode(0, 0, mapSize.x, mapSize.y);
            // 트리 분할
            DivideTree(rootNode, 0);
            // 방 생성
            GenerateDeungeuon(rootNode, 0);
            // 길 연결
            GenerateRoad(rootNode, 0);
            // 벽 생성 범위 체크
            GeneratedCheckWalls();
            // 문 생성
            GenerateDoors();
            // 벽 생성
            CreateWallAroundByRoom();


            //LineRenderer.SetActive(false);
        }

        // 벽 함수 초기화
        private void InitializeMap()
        {

            mapData = new TileType[mapSize.x, mapSize.y];

        }


        // 재귀 함수
        private void DivideTree(TreeNode treeNode, int n)
        {
            if (n < maxNode) // 0부터 노드 최대값에 이를 때 까지 반복
            {
                // 이진 트리의 범위 값 저장, 사각형 범위 담기
                RectInt size = treeNode.treeSize;
                // 사각형의 가로와 세로 길이 중 길이가 긴 축을 트리 반으로 나누는 기준선으로
                int length = size.width >= size.height ? size.width : size.height;
                // 기준선 위에서 최소 범위와 최대 범위 사이의 값 무작위 선택
                int split = Mathf.RoundToInt(Random.Range(length * minDivideSize, length * maxDivideSize));
                // 노드 크기 안정처리
                split = Mathf.Clamp(split, minNode, length - minNode);
                // 가로
                if (size.width >= size.height)
                {
                    // 기준선을 반으로 나눈 값인 split을 가로 길이로, 이진트리의 height값을 세로 길이로 사용
                    treeNode.leftTree = new TreeNode(size.x, size.y, split, size.height);
                    // x 값에 split값을 더해 좌표 설정. 이전 트리의 width값에 split값을 빼 가로 길이 설정
                    treeNode.rightTree = new TreeNode(size.x + split, size.y, size.width - split, size.height);
                    OnDrawLine(new Vector2(size.x + split, size.y),
                        new Vector2(size.x + split, size.y + size.height));
                }
                // 세로
                else
                {
                    treeNode.leftTree = new TreeNode(size.x, size.y, size.width, split);
                    treeNode.rightTree = new TreeNode(size.x, size.y + split, size.width, size.height - split);
                    OnDrawLine(new Vector2(size.x, size.y + split),
                        new Vector2(size.x + size.width, size.y + split));
                }
                // 분할한 트리의 부모 트리를 매개 변수로 받은 트리로 할당
                treeNode.leftTree.parentTree = treeNode;
                treeNode.rightTree.parentTree = treeNode;
                // 재귀 함수, 자식 트리를 매개변수로 넘기고 노드 값 1 증가시킴
                // 순회 방식
                DivideTree(treeNode.leftTree, n + 1);
                DivideTree(treeNode.rightTree, n + 1);
            }
        }
        // 방 생성 메서드
        private RectInt GenerateDeungeuon(TreeNode treeNode, int node)
        {
            if (node == maxNode)
            {
                RectInt size = treeNode.treeSize;
                // 트리 범위 내에서 무작위 크기 선택, 최소 크기 : width / 2
                //int width = Mathf.Max(Random.Range(size.width / 2, size.width - 1));
                //int height = Mathf.Max(Random.Range(size.height / 2, size.height - 1));

                int width = Random.Range(size.width / 2, size.width - 1);
                int height = Random.Range(size.height / 2, size.height - 1);

                // 최대 크기 : width / 2
                int x = treeNode.treeSize.x + Random.Range(1, size.width - width);
                int y = treeNode.treeSize.y + Random.Range(1, size.height - height);
                // 던전 렌더링
                OnDrawDungeon(x, y, width, height);
                // 리턴 값은 던전의 크기로 길을 생성할 때 크기 정보로 활용
                return new RectInt(x, y, width, height);
            }
            // 리턴 값 = 던전 크기
            treeNode.leftTree.dungeonSize = GenerateDeungeuon(treeNode.leftTree, node + 1);
            treeNode.rightTree.dungeonSize = GenerateDeungeuon(treeNode.rightTree, node + 1);
            // 부모 트리의 던전 크기는 자식 트리의 던전 크기 그대로 사용
            return treeNode.leftTree.dungeonSize;
        }


        // 길 연결 메서드
        private void GenerateRoad(TreeNode treeNode, int depth)
        {
            // 노드가 최하위일 때는 길을 연결하지 않음. 최하위 노드는 자식 트리가 없다.
            if (depth == maxNode) return;
            // 자식 트리의 던전 중앙 위치를 가져옴
            RectInt leftRoom = treeNode.leftTree.dungeonSize;
            RectInt rightRoom = treeNode.rightTree.dungeonSize;

            // 중심 계산
            Vector2Int leftCenter = GetRoomCenter(leftRoom);
            Vector2Int rightCenter = GetRoomCenter(rightRoom);

            // 연결 방향은 랜덤
            if (Random.value < 0.5f)
            {
                CreateHorizontalCorridor(leftCenter.x, rightCenter.x, leftCenter.y);
                CreateVerticalCorridor(leftCenter.y, rightCenter.y, rightCenter.x);
            }
            else
            {
                CreateVerticalCorridor(leftCenter.y, rightCenter.y, leftCenter.x);
                CreateHorizontalCorridor(leftCenter.x, rightCenter.x, leftCenter.y);
            }

            // 길 생성
            GenerateRoad(treeNode.leftTree, depth + 1);
            GenerateRoad(treeNode.rightTree, depth + 1);
        }

        // 문 생성 함수
        private void GenerateDoors()
        {
            for (int x = 1; x < mapSize.x - 1; x++)
            {
                for (int y = 1; y < mapSize.y - 1; y++)
                {
                    if (mapData[x, y] != TileType.Path) continue;

                    // 통로 타일이 방과 접해 있는지 체크
                    bool hasRoomNeighbor =
                        mapData[x + 1, y] == TileType.Room ||
                        mapData[x - 1, y] == TileType.Room ||
                        mapData[x, y + 1] == TileType.Room ||
                        mapData[x, y - 1] == TileType.Room;

                    if (!hasRoomNeighbor) continue;

                    // 주변 벽 체크
                    bool surrondedByWall =
                        mapData[x + 1, y] == TileType.Wall ||
                        mapData[x - 1, y] == TileType.Wall ||
                        mapData[x, y + 1] == TileType.Wall ||
                        mapData[x, y - 1] == TileType.Wall;

                    // 통로 방향 판별
                    // 수평 통로는 좌/우, 수직은 상하 Path
                    bool hasLeft = (mapData[x - 1, y] == TileType.Path);
                    bool hasRight = (mapData[x + 1, y] == TileType.Path);
                    bool hasDown = (mapData[x, y - 1] == TileType.Path);
                    bool hasUp = (mapData[x, y + 1] == TileType.Path);

                    int horizontal = (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);
                    int vertical = (hasDown ? 1 : 0) + (hasUp ? 1 : 0);

                    // 통로가 수평으로 더 이어져 있다면 문은 세로 3칸
                    // 통로가 수직으로 이어져 있다면 가로 3칸
                    if(horizontal >= vertical)
                    {
                        PlaceDoorVertical(x, y);
                    }
                    else
                    {
                        PlaceDoorHoriaontal(x, y);
                    }

                    if (surrondedByWall)
                    {
                        mapData[x, y] = TileType.Door;
                    }
                }
            }
        }
        private void PlaceDoorVertical(int x, int y)
        {
            for(int w = - doorHalfwidth; w <= doorHalfwidth; w++)
            {
                int ny = y + w;
                if (!IsInsideMap(x, ny)) continue;

                // path 칸만 door 변경
                if (mapData[x, ny] == TileType.Path)
                {
                    mapData[x, ny] = TileType.Door;
                }

            }
        }

        private void PlaceDoorHoriaontal(int x, int y)
        {
            for (int w = -doorHalfwidth; w <= doorHalfwidth; w++)
            {
                int nx = x + w;
                if (!IsInsideMap(nx, y)) continue;

                // path 칸만 door 변경
                if (mapData[nx, y] == TileType.Path)
                {
                    mapData[nx, y] = TileType.Door;
                }

            }
        }


        // 방 중심 계산
        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int
                (room.x + room.width / 2, room.y + room.height / 2);
        }

        // 수평 통로
        private void CreateHorizontalCorridor(int xStart, int xEnd, int y)
        {
            for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++) 
            {
                for (int w = -1; w <= 1; w++) // 통로 두께 계산
                {
                    int ny = y + w;
                    if (!IsInsideMap(x, ny)) continue;

                    // 이미 같은 경로에 통로가 생성되어 있다면 스킵한다
                    if (mapData[x, ny] == TileType.Path) continue;

                    // 방도 마찬가지
                    if (mapData[x, ny] == TileType.Room) continue;

                    mapData[x, ny] = TileType.Path;

                    TileBase selectedTile = PathTiles[Random.Range(0, PathTiles.Length)];
                    tilemap.SetTile(new Vector3Int(x - mapSize.x / 2, ny - mapSize.y / 2, 0), selectedTile);
                }
            }
        }

        // 수직 통로
        private void CreateVerticalCorridor(int yStart, int yEnd, int x)
        {
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
                for (int w = -1; w <= 1; w++) // 통로 두께 계산
                {
                    int nx = x + w;
                    if (!IsInsideMap(nx, y)) continue;

                    // 이미 같은 경로에 통로가 생성되어 있다면 스킵한다
                    if (mapData[nx, y] == TileType.Path) continue;

                    // 방도 마찬가지
                    if (mapData[nx, y] == TileType.Room) continue;

                    mapData[nx, y] = TileType.Path;

                    TileBase selectedTile = PathTiles[Random.Range(0, PathTiles.Length)];
                    tilemap.SetTile(new Vector3Int(nx - mapSize.x / 2, y - mapSize.y / 2, 0), selectedTile);
                }
            }
        }

        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x< mapSize.x && y < mapSize.y;
        }


        // 벽 체크 메서드
        private void GeneratedCheckWalls()
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {

                    // 주변 8칸 중 바닥이 하나라도 있다면
                    if (mapData[x, y] == TileType.Room ||
                        mapData[x, y] == TileType.Path)
                    {
                        CheckWallFind(x, y);
                        //CarveCorriderHorizontal(x, y);
                        //CarveCorriderVertical(x, y);
                    }

                }
            }
        }

        // 벽 생성 메서드
        private void CheckWallFind(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    // 맵 범위 체크
                    if (nx < 0 || ny < 0 || nx >= mapSize.x || ny >= mapSize.y)
                    {
                        continue;
                    }

                    // empty를 wall로
                    if (mapData[nx, ny] == TileType.Empty)
                    {
                        mapData[nx, ny] = TileType.Wall;
                    }
                }
            }
        }



        private void CreateWallAroundByRoom()
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x - mapSize.x / 2, y - mapSize.y / 2);

                    if (mapData[x, y] == TileType.Room)
                    {
                        floorTilemap.SetTile(pos, floorTile);
                    }
                    else if (mapData[x, y] == TileType.Wall)
                    {
                        wallTilemap.SetTile(pos, wallTile);
                    }
                    else if (mapData[x, y] == TileType.Door)
                    {
                        floorTilemap.SetTile(pos, doorTile);
                    }

                }

            }
        }

        // ----------------- 스폰 포인트 지정 메서드 ---------------------------
        private void PlayerSpawnPoint()
        {
            
        }

        

        // ----------------- 이미지 그리는 메서드 ------------------------------
        // 라인 렌더러를 이용해 라인을 그리는 메소드
        private void OnDrawLine(Vector2 from, Vector2 to)
        {
            LineRenderer lineRenderer = Instantiate(line, lineHolder).GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, from - mapSize / 2);
            lineRenderer.SetPosition(1, to - mapSize / 2);
        }

        // 크기에 맞춰 타일을 생성하는 메소드
        private void OnDrawDungeon(int x, int y, int width, int height)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    mapData[i, j] = TileType.Room;
                    TileBase selectedTile = PathTiles[Random.Range(0, PathTiles.Length)];
                    tilemap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2), selectedTile);
                }
            }
        }

        // 라인 렌더러를 이용해 사각형을 그리는 메소드
        private void OnDrawRectangle(int x, int y, int width, int height)
        {
            LineRenderer lineRenderer = Instantiate(rectangle, lineHolder).GetComponent<LineRenderer>();
            // 위치를 화면 중앙에 맞춤
            lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
            lineRenderer.SetPosition(1, new Vector2(x + width, y) - mapSize / 2);
            lineRenderer.SetPosition(2, new Vector2(x + width, y + height) - mapSize / 2);
            lineRenderer.SetPosition(3, new Vector2(x, y + height) - mapSize / 2);
        }

        // -----------------레거시 메서드-----------------------------

        // 체크형 벽 감지
        private void GenerateWalls()
        {
            for (int x = 1; x < mapSize.x - 1; x++)
            {
                for (int y = 1; y < mapSize.y - 1; y++)
                {
                    if (mapData[x, y] == 0)
                    {
                        // 주변 8칸 중 바닥이 하나라도 있다면
                        if (HasAdjacentFloor(x, y))
                        {
                            mapData[x, y] = TileType.Wall;
                        }
                    }
                }
            }
        }

        // 바닥 유무 확인 메서드
        private bool HasAdjacentFloor(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (mapData[x + dx, y + dy] == TileType.Room)
                        return true;
                }
            }
            return false;
        }


        private void CarveCorriderHorizontal(int x, int y)
        {
            for (int w = -1; w <= 1; w++)
            {
                int ny = y + w;

                if (ny >= 0 && ny < mapSize.y)
                {
                    mapData[x, ny] = TileType.Path;
                }

            }
        }

        private void CarveCorriderVertical(int x, int y)
        {
            for (int w = -1; w <= 1; w++)
            {
                int nx = x + w;

                if (nx >= 0 && nx < mapSize.y)
                {
                    mapData[nx, y] = TileType.Path;
                }

            }
        }

        private int GetCenterX(RectInt size)
        {
            return size.x + size.width / 2;
        }
        private int GetCenterY(RectInt size)
        {
            return size.y + size.height / 2;
        }

    }

}