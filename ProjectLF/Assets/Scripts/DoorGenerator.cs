using UnityEngine;
using UnityEngine.Tilemaps;
using static DuengeonData;

public class DoorGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField]
    private Vector2Int mapSize;

    // 맵 데이터 배열 생성, 초기화
    private TileType[,] mapData;
    // 0 = 빈공간
    // 1 = 바닥
    // 2 = 벽

    [SerializeField]
    private TileBase doorTile;

    [SerializeField]
    private int doorHalfwidth;


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
                if (horizontal >= vertical)
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

    // 맵 범위 체크 (예외방지)
    private bool IsInsideMap(int x, int y)
    {
        return x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y;
    }

    private void PlaceDoorVertical(int x, int y)
    {
        for (int w = -doorHalfwidth; w <= doorHalfwidth; w++)
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
}
