using System.Collections.Generic;
using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Generation
{
    public enum DoorDir
    {
        Up,
        Right,
        Down,
        Left
    }

    public class CorridorGenerator
    {
        private readonly DungeonConfig config;
        private readonly DungeonContext context;

        public CorridorGenerator(DungeonConfig config, DungeonContext context)
        {
            this.config = config;
            this.context = context;
        }

        public void Run(BspNode root)
        {
            if (root == null)
                return;


            List<BspNode> leafNodes = new List<BspNode>();
            CollectLeaves(root, leafNodes);

            if (leafNodes.Count <= 1)
                return;

            HashSet<string> connectedPairs = new HashSet<string>();
            Dictionary<BspNode, int> degreeMap = new Dictionary<BspNode, int>();

            foreach (var node in leafNodes)
            {
                degreeMap[node] = 0;
            }

            //List<BspNode> ordered = BuildNearestOrder(leafNodes);

            ConnectNodesRecursive(root, connectedPairs, degreeMap);

            //for (int i = 0; i < ordered.Count - 1; i++)
            //{
            //    TryConnectRooms(ordered[i], ordered[i + 1], connectedPairs, degreeMap);
            //}

            AddExtraConnections(leafNodes, connectedPairs, degreeMap);
        }

        private void TryConnectRooms(BspNode bspNode1, BspNode bspNode2, HashSet<string> connectedPairs, Dictionary<BspNode, int> degreeMap)
        {
            if (!degreeMap.ContainsKey(bspNode1))
                degreeMap[bspNode1] = 0;
            if (!degreeMap.ContainsKey(bspNode2))
                degreeMap[bspNode2] = 0;

            if (!bspNode1.RoomBounds.HasValue || !bspNode2.RoomBounds.HasValue)
                return;

            string key = GetConnectionKey(bspNode1.RoomBounds.Value, bspNode2.RoomBounds.Value);
            if (connectedPairs.Contains(key))
                return;

            IntRect roomA = bspNode1.RoomBounds.Value;
            IntRect roomB = bspNode2.RoomBounds.Value;

            DoorDir dirA = GetDoorDirection(roomA, roomB);
            DoorDir dirB = GetOpposite(dirA);

            Vector2Int roomDoorA = GetRoomDoorCell(roomA, dirA);
            Vector2Int roomDoorB = GetRoomDoorCell(roomB, dirB);


            Vector2Int start = GetOutsideDoorCell(roomA, dirA);
            Vector2Int end = GetOutsideDoorCell(roomB, dirB);

            context.RoomEnteranceCells.Add(roomDoorA);
            context.RoomEnteranceCells.Add(roomDoorB);

            RegisterRoomConnection(roomA, dirA);
            RegisterRoomConnection(roomB, dirB);

            Connect(start, end, dirA);

            connectedPairs.Add(key);
            degreeMap[bspNode1]++;
            degreeMap[bspNode2]++;
        }

        private void AddExtraConnections(List<BspNode> leafNodes, HashSet<string> connectedPairs, Dictionary<BspNode, int> degreeMap)
        {
            const float extraChance = 0.9f;
            const int maxDegree = 3;

            foreach (var node in leafNodes)
            {
                if (!node.RoomBounds.HasValue)
                    continue;

                if (degreeMap[node] >= maxDegree)
                    continue;

                if (Random.value > extraChance)
                    continue;

                List<BspNode> candidates = GetNearestCandidates(node, leafNodes, connectedPairs, degreeMap, maxDegree);

                if (candidates.Count == 0)
                    continue;

                BspNode target = candidates[Random.Range(0, candidates.Count)];
                TryConnectRooms(node, target, connectedPairs, degreeMap);
            }
        }

        // bsp 식 재귀 연결
        private void ConnectNodesRecursive(
            BspNode node,
            HashSet<string> connectedPairs,
            Dictionary<BspNode, int> degreeMap
            )
        {
            if (node.IsLeaf)
                return;

            ConnectNodesRecursive(node.Left, connectedPairs, degreeMap);
            ConnectNodesRecursive(node.Right, connectedPairs, degreeMap);

            BspNode leftLeaf = GetAnyLeafWithRoom(node.Left);
            BspNode rightLeaf = GetAnyLeafWithRoom(node.Right);

            if (leftLeaf != null && rightLeaf != null)
            {
                TryConnectRooms(leftLeaf, rightLeaf, connectedPairs, degreeMap);
            }
        }

        private BspNode GetAnyLeafWithRoom(BspNode node)
        {
            if (node == null)
                return null;
            if (node.IsLeaf && node.RoomBounds.HasValue)
                return node;
            BspNode leftResult = GetAnyLeafWithRoom(node.Left);
            if (leftResult != null)
                return leftResult;

            return GetAnyLeafWithRoom(node.Right);

        }

        // 그리디 방식
        private List<BspNode> GetNearestCandidates(BspNode node, List<BspNode> leafNodes, HashSet<string> connectedPairs, Dictionary<BspNode, int> degreeMap, int maxDegree)
        {
            List<(BspNode node, float dist)> temp = new List<(BspNode, float)>();

            Vector2Int sourceCenter = node.RoomBounds.Value.Center;

            foreach (var other in leafNodes)
            {
                if (other == node || !other.RoomBounds.HasValue)
                    continue;
                if (degreeMap[other] >= maxDegree)
                    continue;
                string key = GetConnectionKey(node.RoomBounds.Value, other.RoomBounds.Value);
                if (connectedPairs.Contains(key))
                    continue;

                Vector2Int targetCenter = other.RoomBounds.Value.Center;
                float dist = Vector2Int.Distance(sourceCenter, targetCenter);
                if (dist > 20f) // 임의값. 거리제한이니 값 조절 필요하면
                    continue;
                temp.Add((other, dist));
            }
            temp.Sort((a, b) => a.dist.CompareTo(b.dist));

            List<BspNode> candidates = new List<BspNode>();
            int count = Mathf.Min(3, temp.Count);

            for (int i = 0; i < count; i++)
            {
                candidates.Add(temp[i].node);
            }

            return candidates;
        }

        

        private void CollectLeaves(BspNode node, List<BspNode> leaves)
        {
            if (node == null)
                return;
            if (node.IsLeaf)
            {
                if (node.RoomBounds.HasValue)
                {
                    leaves.Add(node);
                }

                return;
            }
            CollectLeaves(node.Left, leaves);
            CollectLeaves(node.Right, leaves);
        }


        private void Connect(Vector2Int start, Vector2Int end, DoorDir startDir)
        {
            if (startDir == DoorDir.Left || startDir == DoorDir.Right)
            {
                DigHorizontal(start.x, end.x, start.y);
                DigVertical(start.y, end.y, end.x);
            }
            else
            {
                DigVertical(start.y, end.y, start.x);
                DigHorizontal(start.x, end.x, end.y);
            }
        }

        private void DigHorizontal(int x1, int x2, int y)
        {
            int min = Mathf.Min(x1, x2);
            int max = Mathf.Max(x1, x2);
            for (int x = min; x <= max; x++)
            {
                PaintCorridorWidth(x, y);
            }
        }

        private void DigVertical(int y1, int y2, int x)
        {
            int min = Mathf.Min(y1, y2);
            int max = Mathf.Max(y1, y2);

            for (int y = min; y <= max; y++)
            {
                PaintCorridorHeight(x, y);
            }
        }

        private void PaintCorridorWidth(int centerX, int centerY)
        {
            int width = config.corridorWidthInCells;
            int half = width / 2;

            for (int dx = -half; dx <= half; dx++)
            {
                for (int dy = -half; dy <= half; dy++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (!context.IsInside(x, y))
                        continue;

                    if (context.Grid[x, y] == CellType.Room)
                        continue;

                    if (context.Grid[x, y] == CellType.Empty)
                    {
                        context.Grid[x, y] = CellType.Corridor;
                    }
                    context.CorridorCells.Add(new Vector2Int(x, y));
                }
            }
        }

        private void PaintCorridorHeight(int centerX, int centerY)
        {
            int height = config.corridorWidthInCells;
            int half = height / 2;

            for (int dy = -half; dy <= half; dy++)
            {
                for (int dx = -half; dx <= half; dx++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (!context.IsInside(x, y))
                        continue;

                    if (context.Grid[x, y] == CellType.Room)
                        continue;

                    if (context.Grid[x, y] == CellType.Empty)
                    {
                        context.Grid[x, y] = CellType.Corridor;
                    }
                    context.CorridorCells.Add(new Vector2Int(x, y));
                }
            }
        }


        private Vector2Int GetRoomDoorCell(IntRect room, DoorDir dir)
        {
            int left = room.xMin;
            int right = room.xMax - 1;
            int bottom = room.yMin;
            int top = room.yMax - 1;

            int centerX = room.xMin + (room.width - 1) / 2;
            int centerY = room.yMin + (room.height - 1) / 2;
            switch (dir)
            {
                case DoorDir.Up:
                    return new Vector2Int(centerX, top);
                case DoorDir.Down:
                    return new Vector2Int(centerX, bottom);
                case DoorDir.Left:
                    return new Vector2Int(left, centerY);
                case DoorDir.Right:
                    return new Vector2Int(right, centerY);
            }
            return new Vector2Int(centerX, centerY);

        }

        private DoorDir GetDoorDirection(IntRect from, IntRect to)
        {          
            bool xOverlap = from.xMin < to.xMax && from.xMax > to.xMin;
            bool yOverlap = from.yMin < to.yMax && from.yMax > to.yMin;

            if (yOverlap)
                return (to.Center.x > from.Center.x)? DoorDir.Right : DoorDir.Left;

            if (xOverlap)
                return (to.Center.y > from.Center.y) ? DoorDir.Up : DoorDir.Down;

            int dx = to.Center.x - from.Center.x;
            int dy = to.Center.y - from.Center.y;

            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
                return dx >= 0 ? DoorDir.Right : DoorDir.Left;
            else
                return dy >= 0 ? DoorDir.Up : DoorDir.Down;
        }

        private Vector2Int GetOutsideDoorCell(IntRect room, DoorDir dir)
        {
            Vector2Int doorCell = GetRoomDoorCell(room, dir);

            switch (dir)
            {
                case DoorDir.Up:
                    return doorCell + Vector2Int.up;
                case DoorDir.Down:
                    return doorCell + Vector2Int.down;
                case DoorDir.Left:
                    return doorCell + Vector2Int.left;
                case DoorDir.Right:
                    return doorCell + Vector2Int.right;
            }
            return doorCell;
        }

        private DoorDir GetOpposite(DoorDir dir)
        {
            switch (dir)
            {
                case DoorDir.Up: return DoorDir.Down;
                case DoorDir.Down: return DoorDir.Up;
                case DoorDir.Left: return DoorDir.Right;
                case DoorDir.Right: return DoorDir.Left;
            }
            return DoorDir.Down;
        }


        // vector2Int 정중앙 좌표 방식
        private string GetConnectionKey(IntRect a, IntRect b)
        {
            Vector2Int ca = a.Center;
            Vector2Int cb = b.Center;

            string sa = $"{ca.x}_{ca.y}";
            string sb = $"{cb.x}_{cb.y}";

            return string.Compare(sa, sb) < 0 ? $"{sa}|{sb}" : $"{sb}|{sa}";

        }

        private void RegisterRoomConnection(IntRect room, DoorDir dir)
        {
            string key = GetRoomKey(room);

            if (!context.RoomConnectedDirs.ContainsKey(key))
            {
                context.RoomConnectedDirs[key] = new HashSet<DoorDir>();
            }
            context.RoomConnectedDirs[key].Add(dir);
        }


        private string GetRoomKey(IntRect room)
        {
            return $"{room.x}_{room.y}_{room.width}_{room.height}";
        }


    }
}

// 그리디 방식
//private List<BspNode> BuildNearestOrder(List<BspNode> leafNodes)
//{
//    List<BspNode> result = new List<BspNode>();

//    if (leafNodes == null || leafNodes.Count == 0)
//        return result;

//    List<BspNode> remaining = new List<BspNode>();

//    foreach (var node in leafNodes)
//    {
//        if (node != null && node.RoomBounds.HasValue)
//            remaining.Add(node);
//    }

//    if (remaining.Count == 0)
//        return result;

//    remaining.Sort((a, b) =>
//    {
//        Vector2Int ca = a.RoomBounds.Value.Center;
//        Vector2Int cb = b.RoomBounds.Value.Center;

//        int yCompare = ca.y.CompareTo(cb.y);
//        if (yCompare != 0)
//            return yCompare;

//        return ca.x.CompareTo(cb.x);
//    });

//    BspNode current = remaining[0];
//    result.Add(current);
//    remaining.RemoveAt(0);

//    while (remaining.Count > 0)
//    {
//        BspNode nearest = null;
//        float nestDist = float.MaxValue;

//        Vector2Int currentCenter = current.RoomBounds.Value.Center;

//        foreach (var candidate in remaining)
//        {
//            Vector2Int candidateCenter = candidate.RoomBounds.Value.Center;
//            float dist = Vector2Int.Distance(currentCenter, candidateCenter);

//            if (dist < nestDist)
//            {
//                nestDist = dist;
//                nearest = candidate;
//            }
//        }

//        if (nearest == null)
//            break;

//        result.Add(nearest);
//        remaining.Remove(nearest);
//        current = nearest;

//    }
//    return result;
//}

//// 연결 키 방식
//private string GetConnectionKey(BspNode a, BspNode b)
//{
//    int ha = a.GetHashCode();
//    int hb = b.GetHashCode();

//    return ha < hb ? $"{ha}_{hb}" : $"{hb}_{ha}";
//}

//private Vector2Int GetExitPoint(IntRect room, Vector2Int targetCenter)
//{
//    Vector2Int roomCenter = room.Center;

//    int dx = targetCenter.x - roomCenter.x;
//    int dy = targetCenter.y - roomCenter.y;

//    int left = room.xMin;
//    int right = room.xMax - 1;
//    int bottom = room.yMin;
//    int top = room.yMax - 1;

//    if (Mathf.Abs(dx) >= Mathf.Abs(dy))
//    {
//        if (dx >= 0)
//            return new Vector2Int(right + 1, roomCenter.y);
//        else
//            return new Vector2Int(left - 1, roomCenter.y);
//    }
//    else
//    {
//        if (dy >= 0)
//            return new Vector2Int(roomCenter.x, top + 1);
//        else
//            return new Vector2Int(roomCenter.x, bottom - 1);
//    }
//}

//private Vector2Int? FindRoomCenter(BspNode node)
//{
//    if (node == null)
//        return null;

//    if (node.IsLeaf)
//    {
//        if (node.RoomBounds.HasValue)
//            return node.RoomBounds.Value.Center;
//        return null;
//    }

//    Vector2Int? left = FindRoomCenter(node.Left);
//    Vector2Int? right = FindRoomCenter(node.Right);
//    if (left.HasValue)
//        return left;
//    if (right.HasValue)
//        return right;

//    return null;
//}