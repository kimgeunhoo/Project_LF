using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BspTree;
using static DuengeonData;

public class RoomGenerater : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField]
    private Vector2Int mapSize;

    // 맵 데이터 배열 생성, 초기화
    private TileType[,] mapData;
    // 0 = 빈공간
    // 1 = 바닥
    // 2 = 벽

    // 노드 값이 라인의 갯수를 판별
    [Header("Node Value")]
    [SerializeField]
    private int maxNode;
    [SerializeField]
    private int minNode;

    [Header("Random TileArray")]
    [SerializeField]
    private TileBase[] PathTiles;

    // 타일맵 배치
    [Header("Tile")]
    [SerializeField]
    private Tile tile;
    // 타일맵 랜덤변수
    [SerializeField]
    private Tilemap tilemap;

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
}
