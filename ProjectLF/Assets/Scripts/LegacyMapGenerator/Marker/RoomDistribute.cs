using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Generation;
using UnityEditor.Experimental.GraphView;

namespace BSPDungeonGenrator.marker
{
    public class RoomDistribute : MonoBehaviour
    {
        private Vector2Int startSpawnPoint;
        public Vector2Int StartSpawnPoint {  get { return startSpawnPoint; } }

        private readonly List<Vector2Int> monsterSpawnPoint = new List<Vector2Int>();
        public List<Vector2Int> MonsterSpawnPoint {  get { return monsterSpawnPoint; } }

        private OldDungeonContext ctx;

        public void Run(OldDungeonContext _ctx)
        {
            this.ctx = _ctx;

            ClearRoomResults();

            CollectLeafRooms(this.ctx.Root, 0, this.ctx.Rooms);
            AssignRoomTypes(this.ctx.Rooms);
            ResolveRoomPoints(this.ctx);
        }

        private void ClearRoomResults()
        {
            ctx.Rooms.Clear();
            ctx.EncounterPoints.Clear();
            ctx.MonsterPoints.Clear();

            ctx.StartPoint = Vector2Int.zero;
            ctx.StairPoint = Vector2Int.zero;
            ctx.ShopPoint = Vector2Int.zero;

            monsterSpawnPoint.Clear();
            startSpawnPoint = Vector2Int.zero;
        }

        private void ResolveRoomPoints(OldDungeonContext _ctx)
        {
            foreach (var room in _ctx.Rooms)
            {
                Vector2Int center = room.Center;

                switch (room.Type)
                {
                    case RoomType.Start:
                        _ctx.StartPoint = center;
                        startSpawnPoint = center;
                        break;

                    case RoomType.Stairs:
                        _ctx.StairPoint = center;
                        break;

                    case RoomType.Shop:
                        _ctx.ShopPoint = center;
                        break;

                    case RoomType.Encounter:
                        _ctx.EncounterPoints.Add(center);
                        break;

                    case RoomType.Monster:
                        _ctx.MonsterPoints.Add(center);
                        monsterSpawnPoint.Add(center);
                        break;
                }
            }
        }

        private void CollectLeafRooms(TreeNode node,int depth, List<RoomInfo> rooms)
        {
            if (node == null)
                return;

            bool isLeaf = node.leftTree == null && node.rightTree == null;
            if (isLeaf)
            {
                rooms.Add(new RoomInfo(rooms.Count, node.dungeonSize));
                return;
            }

            CollectLeafRooms(node.leftTree, depth + 1, rooms);
            CollectLeafRooms(node.rightTree, depth + 1, rooms);
        }
        
        private void AssignRoomTypes(List<RoomInfo> rooms)
        {
            if (rooms.Count < 3)
                return;

            // 시작점
            int startIndex = Random.Range(0, rooms.Count);
            rooms[startIndex].Type = RoomType.Start;

            // 도착점
            int stairIndex = GetFarthestRoomIndex(rooms,  startIndex, excludeIndices : null);
            rooms[startIndex].Type = RoomType.Stairs;

            var excluded = new HashSet<int> { startIndex, stairIndex };

            // 상점 포인트
            int shopIndex = GetMidDistanceRoomIndex(rooms, startIndex, stairIndex, excluded);
            rooms[shopIndex].Type = RoomType.Shop;
            excluded.Add(shopIndex);
            List<int> remainIndices = new List<int>();
            for (int i = 0; i < rooms.Count; i++)
            {
                if(!excluded.Contains(i))
                {
                    remainIndices.Add(i);
                }
            }

            // 인카운터
            int encounterCount = Mathf.RoundToInt(remainIndices.Count * 0.3f);
            encounterCount = Mathf.Clamp(encounterCount, 2, remainIndices.Count);

            foreach (int idx in remainIndices)
            {
                rooms[idx].Type = RoomType.Monster;
            }

            for (int i = 0; i < remainIndices.Count; i++)
            {
                int rand = Random.Range(i, remainIndices.Count);
                (remainIndices[i], remainIndices[rand]) = (remainIndices[rand], remainIndices[i]);
            }

            for(int i = 0; i <encounterCount; i++)
            {
                rooms[remainIndices[i]].Type = RoomType.Encounter;
            }

        }

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
                int distance = Mathf.Abs(center.x - from.x) + Mathf.Abs(center.y - from.y);

                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    index = i;
                }
            }

            return index;
        }

        private int GetMidDistanceRoomIndex(List<RoomInfo> rooms, int startIndex, int stairIndex, HashSet<int> excludeIndices)
        {
            Vector2Int start = rooms[startIndex].Center;
            Vector2Int stairs = rooms[stairIndex].Center;

            int totalDist = Mathf.Abs(start.x - stairs.x) + Mathf.Abs(stairs.y - start.y);
            float target = totalDist * 0.5f;

            int index = -1;
            float bestScore = float.MaxValue;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (excludeIndices.Contains(i))
                    continue;

                Vector2Int c = rooms[i].Center;
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

    }


}