using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Config;
using BSPDuengeonGenrator.Core;


namespace BSPDuengeonGenrator.Generation
{

    public class RoadGenerator : MonoBehaviour
    {

        // 노드 값이 라인의 갯수를 판별
        [Header("Node Value")]
        [SerializeField]
        private int maxNode;
        [SerializeField]
        private int minNode;

        [Header("Map Size")]
        [SerializeField]
        private Vector2Int mapSize;

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

        // 맵 데이터 배열 생성, 초기화
        private TileType[,] mapData;

        private DuengeonContext ctx;

        public void Run(DuengeonContext ctx)
        {
            this.ctx = ctx;
            GenerateRoad(ctx.Root, 0);
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

        // 방 중심 계산
        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int
                (room.x + room.width / 2, room.y + room.height / 2);
        }
        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y;
        }

    }

}