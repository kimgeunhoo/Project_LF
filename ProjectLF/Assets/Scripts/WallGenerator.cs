using UnityEngine;
using static DuengeonData;

public class WallGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField]
    private Vector2Int mapSize;

    // 맵 데이터 배열 생성, 초기화
    private TileType[,] mapData;
    // 0 = 빈공간
    // 1 = 바닥
    // 2 = 벽

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
}
