using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;
using BSPDuengeonGenrator.Generation;
using BSPDuengeonGenrator.Rendering;

namespace BSPDuengeonGenrator
{

    public class DuengeonGeneraterByBSP : MonoBehaviour
    {
        [Header("Duengeon Data")]
        [SerializeField]
        private DuengeonData duengeonData;

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

        [Header("Room Markers (Debug)")]
        [SerializeField]
        private Transform markerHolder;
        [SerializeField]
        private GameObject startMarkerPrefab;
        [SerializeField]
        private GameObject stairMarkerPrefab;
        [SerializeField]
        private GameObject shopMarkerPrefab;
        [SerializeField]
        private GameObject encounterMarkerPrefab;
        [SerializeField]
        private GameObject MonsterMarkerPrefab;


        private static DuengeonContext ctx = new DuengeonContext();

        private BspSplitter bspSplitter;
        private RoomGenerater roomGenerater;
        private WallGenerator wallGenerator;
        private RoadGenerator roadGenerator;
        private DoorGenerator doorGenerator;
        private BspLineDrawer lineDrawer;
        private void Awake()
        {
            // 데이터 무결성 체크
            if(duengeonData == null)
            {
                Debug.LogError("[Generator] duengeonData가 할당되지 않았습니다.");
            }
            
            // ctx 정의
            ctx = Build();
            // 벽과 관련된 데이터 배열 생성
            InitializeMap();
            // 던전 사이즈에 맞게 벽을 그림
            OnDrawRectangle(0, 0, mapSize.x, mapSize.y);

            // 트리 분할 메서드            
            bspSplitter = GetComponent<BspSplitter>();
            bspSplitter.Run(ctx);

            lineDrawer = GetComponent<BspLineDrawer>();
            //Splitter로 값 가져오고 라인 그리기
            lineDrawer.OnDrawLine(ctx);

            // 방 생성
            roomGenerater = GetComponent<RoomGenerater>();
            roomGenerater.Run(ctx);
            // 길 연결
            roadGenerator = GetComponent<RoadGenerator>();
            roadGenerator.Run(ctx);
            // 벽 생성 범위 체크, 생성
            wallGenerator = GetComponent<WallGenerator>();
            wallGenerator.Run(ctx);
            // 문 생성
            doorGenerator = GetComponent<DoorGenerator>();
            doorGenerator.Run(ctx);

            List<RoomInfo> rooms = new List<RoomInfo>();
            CollectLeafRooms(ctx.Root, 0, rooms);
            AssignRoomTypes(rooms);
            SpawnRoomMarkers(rooms);

            //LineRenderer.SetActive(false);
        }

        private DuengeonContext Build()
        {
            var ctx = new DuengeonContext();

            // 컨텍스트에 값 대입
            ctx.MapSize = duengeonData.MapSize;
            ctx.MapData = duengeonData.MapData;
            ctx.MaxNode = duengeonData.MaxNode;
            ctx.MinNode = duengeonData.MinNode;
            ctx.MaxDivideSize = duengeonData.MaxDivideSize;
            ctx.MinDivideSize = duengeonData.MinDivideSize;

            ctx.FloorTilemap = floorTilemap;
            ctx.WallTilemap = wallTilemap;

            ctx.FloorTile = duengeonData.FloorTile;
            ctx.WallTile = duengeonData.WallTile;
            ctx.DoorTile = duengeonData.DoorTile;
            ctx.PathTiles = duengeonData.PathTiles;

            ctx.Rooms = new List<RoomInfo>();

            // 루트가 될 트리 생성
            TreeNode rootNode = new TreeNode(0, 0, mapSize.x, mapSize.y);
            ctx.Root = rootNode;

            return ctx;
        }

        // 벽 함수 초기화
        private void InitializeMap()
        {
            mapData = new TileType[mapSize.x, mapSize.y];
        }
 
        // 리프 방 수집
        private void CollectLeafRooms(TreeNode node, int depth, List<RoomInfo> rooms)
        {
            if (node == null) 
                return;

            if (depth == maxNode)
            {
                rooms.Add(new RoomInfo(node.dungeonSize));
                return;
            }

            CollectLeafRooms(node.leftTree, depth+1, rooms);
            CollectLeafRooms(node.rightTree, depth+1, rooms);
        }

        // 방 할당 로직
        private void AssignRoomTypes(List<RoomInfo> rooms)
        {
            if(rooms.Count < 3)
            {
                Debug.Log("방이 3개 미만이라 배치 불가");
                return;
            }

            // start 시작 지점
            int startIndex = Random.Range(0, rooms.Count);
            rooms[startIndex].Type = RoomType.Start;

            // stair 계단
            int stairIndex = GetFarthestRoomIndex(rooms, startIndex, excludeIndices: null);
            rooms[startIndex].Type = RoomType.Stairs;

            // 계단, 시작지점 제외한 중간 지점(랜덤)
            var excluded = new HashSet<int> { startIndex, stairIndex };
            int shopIndex = GetMidDistanceRoomIndex(rooms, startIndex, stairIndex, excluded);
            rooms[shopIndex].Type = RoomType.Shop;

            // 남은 부분: 인카운터 몬스터 약 2:8 비율로
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i == startIndex || i == stairIndex || i == shopIndex) 
                    continue;
                rooms[i].Type = (Random.value < 0.2f) ? RoomType.Encounter : RoomType.Monster;
            }

        }
        
        // 계단 위치 지정
        private int GetFarthestRoomIndex(List<RoomInfo> rooms, int startIndex, HashSet<int> excludeIndices)
        {
            Vector2Int from = rooms[startIndex].Center;

            int index = -1;
            int bestDistance = int.MinValue;

            for(int i = 0; i < rooms.Count; i++)
            {
                if (i == startIndex) 
                    continue;
                if (excludeIndices != null && excludeIndices.Contains(i))
                    continue;

                Vector2Int center = rooms[i].Center;
                // 맨해튼 식
                int distance = Mathf.Abs(center.x - from.x) + Mathf.Abs(center.y - from.y);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    index = i;
                }
            }
            return index;
        }

        // 상점 거리 계산
        private int GetMidDistanceRoomIndex(List<RoomInfo> rooms, int startIndex, int stairIndex, HashSet<int> excludeIndices)
        {
            Vector2Int start = rooms[startIndex].Center;
            Vector2Int stairs = rooms[stairIndex].Center;

            int totalDist = Mathf.Abs((start.x - stairs.x) + Mathf.Abs(stairs.y - start.y));
            float target = totalDist * 0.5f;

            int index = -1;
            float bestScore = float.MaxValue;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (excludeIndices.Contains(i))
                    continue;

                Vector2Int c = rooms[i].Center;
                // 맨해튼 식
                int distance = Mathf.Abs(c.x - start.x) + Mathf.Abs(c.y - start.y);

                float score = Mathf.Abs(distance - target);
                if (score < bestScore)
                {
                    bestScore = score;
                    index = i;
                }
            }

            if (index == -1)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if(!excludeIndices.Contains(i)) 
                        return i;
                }
            }
            return index;
        }

        // 방 마커 출력
        private void SpawnRoomMarkers(List<RoomInfo> rooms)
        {
            foreach (var room in rooms)
            {
                GameObject prefab = room.Type switch
                {
                    RoomType.Start => startMarkerPrefab,
                    RoomType.Stairs => stairMarkerPrefab,
                    RoomType.Shop => shopMarkerPrefab,
                    RoomType.Encounter => encounterMarkerPrefab,
                    RoomType.Monster => MonsterMarkerPrefab,
                    // 이부분은 throw Exception 써도될듯?
                    _ => null
                };

                if (prefab == null) continue;

                Vector2Int c = room.Center;
                Vector3Int cellPos = new Vector3Int(c.x - mapSize.x / 2, c.y - mapSize.y / 2, 0);

                Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.5f);
                Instantiate(prefab, worldPos, Quaternion.identity, markerHolder);
            }

        }


        // ----------------- 스폰 포인트 지정 메서드 ---------------------------
        private void PlayerSpawnPoint()
        {
            
        }

        

        // ----------------- 이미지 그리는 메서드 ------------------------------
        // 라인 렌더러를 이용해 라인을 그리는 메소드

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
    }

}