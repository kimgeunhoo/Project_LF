using UnityEngine;

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
