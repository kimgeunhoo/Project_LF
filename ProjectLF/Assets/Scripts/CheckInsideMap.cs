using UnityEditor.U2D.Aseprite;
using UnityEngine;
using BSPDuengeonGenrator.Config;

namespace BSPDuengeonGenrator
{
    public class CheckInsideMap : MonoBehaviour
    {
        [Header("Map Size")]
        [SerializeField]
        private Vector2Int mapSize;
        // 맵 데이터 배열 생성, 초기화
        private TileType[,] mapData;
        // 0 = 빈공간
        // 1 = 바닥
        // 2 = 벽

        // 벽 함수 초기화
        private void InitializeMap()
        {
            mapData = new TileType[mapSize.x, mapSize.y];
        }

        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y;
        }

        // 방 중심 계산
        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int
                (room.x + room.width / 2, room.y + room.height / 2);
        }


    } 
}
