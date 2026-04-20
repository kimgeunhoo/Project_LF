using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using System.Collections.Generic;
using UnityEngine;

namespace BSPDungeonGenrator.Utility
{
    public class AStarPathFinder
    {
        private DungeonContext ctx;

        private static readonly Vector2Int[] Directions =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
    };

        public AStarPathFinder(DungeonContext _ctx)
        {
            

            this.ctx = _ctx;

            if (ctx == null)
            {
                Debug.LogError("AStarPathFinder: ctx is null");
                return;
            }

            if (ctx.MapData == null)
            {
                Debug.LogError("AStarPathFinder: ctx.MapData is null");
                return;
            }

        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            //Debug.Log($"[FindPath] start={start}, end={end}");

            if (ctx == null)
            {
                Debug.LogError("[FindPath] ctx is null");
                return null;
            }

            if (ctx.MapData == null)
            {
                Debug.LogError("[FindPath] ctx.MapData is null");
                return null;
            }

            //Debug.Log($"[FindPath] MapData size = {ctx.MapData.GetLength(0)} x {ctx.MapData.GetLength(1)}");

            List<AStarNode> openList = new List<AStarNode>();
            HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
            Dictionary<Vector2Int, AStarNode> allNodes = new Dictionary<Vector2Int, AStarNode>();

            AStarNode startNode = new AStarNode(start);
            startNode.GCost = 0;
            startNode.HCost = Heuristic(start, end);

            openList.Add(startNode);
            allNodes[start] = startNode;

            while (openList.Count > 0)
            {
                //Debug.Log($"[FindPath] openList.Count={openList.Count}");
                AStarNode current = GetLowestFCostNode(openList);
                //Debug.Log($"[FindPath] current is null? {current == null}");
                if (current == null)
                {
                    Debug.LogError("[FindPath] current is null");
                    return null;
                }
                if (current.Pos == end)
                {
                    return ReconstructPath(current);
                }
                openList.Remove(current);
                closedSet.Add(current.Pos);

                foreach (var dir in Directions)
                {
                    Vector2Int nextPos = current.Pos + dir;

                    if (!IsInBounds(nextPos))
                        continue;

                    if (closedSet.Contains(nextPos))
                        continue;

                    if (!IsWalkable(nextPos, start, end))
                        continue;

                    int moveCost = current.GCost + GetTileMoveCost(nextPos);

                    if (!allNodes.TryGetValue(nextPos, out AStarNode nextNode))
                    {
                        nextNode = new AStarNode(nextPos);
                        allNodes[nextPos] = nextNode;

                    }
                    if(!openList.Contains(nextNode) || moveCost < nextNode.GCost)
                    {

                        nextNode.GCost = moveCost;
                        nextNode.HCost = Heuristic(nextPos, end);
                        nextNode.AParent = current;

                        if (!openList.Contains(nextNode))
                        {
                            openList.Add(nextNode);
                        }
                    }

                }
            }
            return null;
        }

        private AStarNode GetLowestFCostNode(List<AStarNode> openList)
        {
            if(openList == null || openList.Count == 0)
                return null;

            AStarNode best = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < best.FCost ||
                    (openList[i].FCost == best.FCost && openList[i].HCost < best.HCost))
                {
                    best = openList[i];
                }

            }
            return best;
        }

        private int Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private List<Vector2Int> ReconstructPath(AStarNode endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            AStarNode current = endNode;

            while (current != null)
            {
                path.Add(current.Pos);
                current = current.AParent;
            }
            path.Reverse();
            return path;
        }

        private bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < ctx.MapSize.x &&
                pos.y >= 0 && pos.y < ctx.MapSize.y;
        }

        private bool IsWalkable(Vector2Int pos, Vector2Int start, Vector2Int goal)
        {
            if (pos == start || pos == goal)
                return true;

            TileType tile = ctx.MapData[pos.x, pos.y];

            // Room은 통과하지 못하게
            if (tile == TileType.Room)
                return true;

            return true;
        }

        private int GetTileMoveCost(Vector2Int pos)
        {
            TileType tile = ctx.MapData[pos.x, pos.y];

            // 통로 생성 후에 벽을 생성해야 안전
            if (tile == TileType.Room)
                return 9999;
            if (IsNearRoom(pos, 3))
                return 80;
            if (IsNearRoom(pos, 4))
                return 30;
            if (tile == TileType.Path)
                return 1;
            if (tile == TileType.Wall)
                return 5;
            return 10;
        }

        private bool IsNearRoom(Vector2Int pos, int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = - radius; dy <= radius; dy++)
                {
                    int nx = pos.x + dx;
                    int ny = pos.y + dy;

                    if (nx < 0 || ny < 0 || nx >= ctx.MapSize.x || ny >= ctx.MapSize.y)
                        continue;

                    if (ctx.MapData[nx, ny] == TileType.Room)
                        return true;

                }
            }
            return false;
        }


    }

}
