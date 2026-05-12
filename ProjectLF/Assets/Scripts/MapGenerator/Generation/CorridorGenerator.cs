using ModularBSP.Config;
using ModularBSP.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ModularBSP.Generation
{
    struct DoorPort
    {
        public DoorDir dir;
        public Vector2Int doorCell;
        public Vector2Int outsideCell;

        public DoorPort(DoorDir _dir, Vector2Int _doorCell, Vector2Int _outsideCell)
        { 
            this.dir = _dir;
            this.doorCell = _doorCell;
            this.outsideCell = _outsideCell;
        }
    }

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

            ConnectNodesRecursive(root, connectedPairs, degreeMap);

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

            DoorPort portA = SelectBestDoorPort(roomA, roomB);
            DoorPort portB = SelectBestDoorPort(roomB, roomA);

            Vector2Int roomDoorA = portA.doorCell;
            Vector2Int roomDoorB = portB.doorCell;

            Vector2Int start = portA.outsideCell;
            Vector2Int end = portB.outsideCell;

            List<Vector2Int> path = FindCorridorPath(start, end, roomA, roomB);

            if (!TryFindBestDoorPath(roomA, roomB, out portA, out portB, out path))
            {
                Debug.LogWarning($"[Corridor] 연결 실패: {roomA.Center} -> {roomB.Center}");
                return;
            }

            context.RoomEnteranceCells.Add(portA.doorCell);
            context.RoomEnteranceCells.Add(portB.doorCell);

            RegisterRoomConnection(roomA, portA.dir);
            RegisterRoomConnection(roomB, portB.dir);

            ForceAddCorridorCell(portA.outsideCell);
            ForceAddCorridorCell(portB.outsideCell);

            PaintPath(path);

            connectedPairs.Add(key);
            degreeMap[bspNode1]++;
            degreeMap[bspNode2]++;
        }

        private bool TryFindBestDoorPath(IntRect roomA, IntRect roomB, out DoorPort portA, out DoorPort portB, out List<Vector2Int> bestPath)
        {
            DoorPort[] portsA =
    {
        MakeDoorPort(roomA, DoorDir.Up),
        MakeDoorPort(roomA, DoorDir.Right),
        MakeDoorPort(roomA, DoorDir.Down),
        MakeDoorPort(roomA, DoorDir.Left),
    };

            DoorPort[] portsB =
            {
        MakeDoorPort(roomB, DoorDir.Up),
        MakeDoorPort(roomB, DoorDir.Right),
        MakeDoorPort(roomB, DoorDir.Down),
        MakeDoorPort(roomB, DoorDir.Left),
    };

            portA = portsA[0];
            portB = portsB[0];
            bestPath = null;

            int bestScore = int.MaxValue;

            foreach (var a in portsA)
            {
                foreach (var b in portsB)
                {
                    List<Vector2Int> path =
                        FindCorridorPath(a.outsideCell, b.outsideCell, roomA, roomB);

                    if (path == null || path.Count == 0)
                        continue;

                    int score = path.Count;

                    if (score < bestScore)
                    {
                        bestScore = score;
                        portA = a;
                        portB = b;
                        bestPath = path;
                    }
                }
            }

            return bestPath != null;
        }

        private void ForceAddCorridorCell(Vector2Int cell)
        {
            if (!context.IsInside(cell.x, cell.y))
                return;

            if (context.Grid[cell.x, cell.y] == CellType.Room)
                return;

            context.Grid[cell.x, cell.y] = CellType.Corridor;

            if (!context.CorridorCells.Contains(cell))
                context.CorridorCells.Add(cell);
        }

        private DoorPort SelectBestDoorPort(IntRect from, IntRect to)
        {
            DoorPort[] ports =
            {
                MakeDoorPort(from, DoorDir.Up),
                MakeDoorPort(from, DoorDir.Right),
                MakeDoorPort(from, DoorDir.Down),
                MakeDoorPort(from, DoorDir.Left),
            };

            DoorPort best = ports[0];
            int bestScore = int.MaxValue;

            Vector2Int target = to.Center;

            foreach (var port in ports)
            {
                int dx = Mathf.Abs(port.outsideCell.x - target.x);
                int dy = Mathf.Abs(port.outsideCell.y - target.y);
                int score = dx + dy;

                if(score < bestScore)
                {
                    bestScore = score;
                    best = port;
                }
            }

            return best;
        }

        private DoorPort MakeDoorPort(IntRect room, DoorDir dir)
        {
            Vector2Int door = GetRoomDoorCell(room, dir);
            Vector2Int outside = GetOutsideDoorCell(room, dir);
            return new DoorPort(dir, door, outside);
        }

        private List<Vector2Int> FindCorridorPath(Vector2Int start, Vector2Int end, IntRect roomA, IntRect roomB)
        {
            Vector2Int cornerA = new Vector2Int(end.x, start.y);
            Vector2Int cornerB = new Vector2Int(start.x, end.y);

            if (IsCorridorLineClear(start, cornerA) &&
                IsCorridorLineClear(cornerA, end))
            {
                return BuildPathFromLines(start, cornerA, end);
            }

            if (IsCorridorLineClear(start, cornerB) &&
                IsCorridorLineClear(cornerB, end))
            {
                return BuildPathFromLines(start, cornerB, end);
            }

            return null;
        }

        private List<Vector2Int> BuildPathFromLines(Vector2Int start, Vector2Int corner, Vector2Int end)
        {
            List<Vector2Int> path = new List<Vector2Int>();

            AddLineCells(path, start, corner);
            AddLineCells(path, corner, end);

            return path;
        }

        private void AddLineCells(List<Vector2Int> path, Vector2Int start, Vector2Int end)
        {
            if (start.x == end.x)
            {
                int min = Mathf.Min(start.y, end.y);
                int max = Mathf.Max(start.y, end.y);

                for (int y = min; y <= max; y++)
                    AddUnique(path, new Vector2Int(start.x, y));
            }
            else if (start.y == end.y)
            {
                int min = Mathf.Min(start.x, end.x);
                int max = Mathf.Max(start.x, end.x);

                for (int x = min; x <= max; x++)
                    AddUnique(path, new Vector2Int(x, start.y));
            }
        }

        private void AddUnique(List<Vector2Int> path, Vector2Int cell)
        {
            if (!path.Contains(cell))
                path.Add(cell);
        }

        private void PaintPath(List<Vector2Int> path)
        {
            if (path == null)
                return;

            foreach (var cell in path)
            {
                PaintCorridor(cell.x, cell.y);
            }
        }

        private void PaintCorridor(int centerX, int centerY)
        {
            int width = config.corridorWidthInCells;

            int start = -(width / 2);
            int end = start + width - 1;

            for (int dx = start; dx <= end; dx++)
            {
                for (int dy = start; dy <= end; dy++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (!context.IsInside(x, y))
                        continue;

                    if (context.Grid[x, y] == CellType.Room)
                        continue;

                    context.Grid[x, y] = CellType.Corridor;
                    context.CorridorCells.Add(new Vector2Int(x, y));
                }
            }
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
                if (dist > 20f) 
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
        private bool IsCorridorLineClear(Vector2Int start, Vector2Int end)
        {
            if (start.x != end.x && start.y != end.y)
                return false;

            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!context.IsInside(x, y))
                        return false;

                    if (context.Grid[x, y] == CellType.Room)
                        return false;
                }
            }

            return true;
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
