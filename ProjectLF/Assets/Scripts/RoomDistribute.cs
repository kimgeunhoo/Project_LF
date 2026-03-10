using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;

namespace BSPDuengeonGenrator.marker
{
    public class RoomDistribute : MonoBehaviour
    {

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

        private DuengeonContext ctx;
        public void Run(DuengeonContext _ctx)
        {
            Debug.Log($"[Generator] Root left = {(_ctx.Root.leftTree == null ? "NULL" : "OK")}");
            Debug.Log($"[Generator] Root right = {(_ctx.Root.rightTree == null ? "NULL" : "OK")}");

            Debug.Log($"[RoomDistribute] Root = {(_ctx.Root == null ? "NULL" : "OK")}");
            Debug.Log($"[RoomDistribute] MaxNode = {_ctx.MaxNode}");

            this.ctx = _ctx;

            ctx.Rooms.Clear();

            CollectLeafRooms(ctx.Root, 0, ctx.Rooms);
            Debug.Log($"[RoomDistribute] collected rooms = {ctx.Rooms.Count}");
            AssignRoomTypes(ctx.Rooms);
            SpawnRoomMarkers(ctx.Rooms);
        }

        // 리프 방 수집
        private void CollectLeafRooms(TreeNode node, int depth, List<RoomInfo> rooms)
        {
            Debug.Log($"[CollectLeafRooms] depth={depth} left={(node.leftTree != null)} right={(node.rightTree != null)}");
            if (node == null)
                return;

            bool isLeaf = node.leftTree == null && node.rightTree == null;
            Debug.Log($"[CollectLeafRooms] isLeaf={isLeaf}");
            if (isLeaf)
            {
                Debug.Log($"[CollectLeafRooms] leaf dungeonSize = {node.dungeonSize}");
                Debug.Log($"[RoomDistribute] collected rooms = {ctx.Rooms.Count}");
                rooms.Add(new RoomInfo(node.dungeonSize));
                return;
            }

            CollectLeafRooms(node.leftTree, depth + 1, rooms);
            CollectLeafRooms(node.rightTree, depth + 1, rooms);
        }

        // 방 할당 로직
        private void AssignRoomTypes(List<RoomInfo> rooms)
        {
            if (rooms.Count < 3)
            {
                Debug.Log($"방 개수: {rooms.Count}");
                Debug.Log("방이 3개 미만이라 배치 불가");
                return;
            }

            // start 시작 지점
            int startIndex = Random.Range(0, rooms.Count);
            rooms[startIndex].Type = RoomType.Start;

            // stair 계단
            int stairIndex = GetFarthestRoomIndex(rooms, startIndex, excludeIndices: null);
            rooms[stairIndex].Type = RoomType.Stairs;

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

            for (int i = 0; i < rooms.Count; i++)
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
                    if (!excludeIndices.Contains(i))
                        return i;
                }
            }
            return index;
        }

        // 방 마커 출력
        private void SpawnRoomMarkers(List<RoomInfo> rooms)
        {
            Debug.Log($"rooms={rooms.Count}");
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
                Vector3Int cellPos = new Vector3Int(c.x - ctx.MapSize.x / 2, c.y - ctx.MapSize.y / 2, 0);

                Vector3 worldPos = ctx.FloorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.5f);
                Instantiate(prefab, worldPos, Quaternion.identity, markerHolder);
            }

        }

    }


}