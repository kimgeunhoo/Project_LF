using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Utility;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BSPDungeonGenrator.Generation
{

    public class PathGenerator
    {

        private OldDungeonContext ctx;
        private AStarPathFinder aStarPathFinder;

        //private int corriderPathWidth = 1;

        public void Run(OldDungeonContext _ctx)
        {
            if (_ctx == null)
            {
                Debug.LogError("PathGenerator.Run: ctx is null");
                return;
            }

            if (_ctx.MapData == null)
            {
                Debug.LogError("PathGenerator.Run: ctx.MapData is null");
                return;
            }

            this.ctx = _ctx;
            this.aStarPathFinder = new AStarPathFinder(ctx);

            //Debug.Log($"[PathGenerator] MapSize={ctx.MapSize}, Root={(ctx.Root == null ? "null" : "ok")}");

            GeneratePath(ctx.Root, 0);
        }

        private void GeneratePath(TreeNode treeNode, int depth)
        {
            if (treeNode == null)
                return;

            if (depth == ctx.MaxNode)
                return;

            if (treeNode.leftTree != null)
            {
                GeneratePath(treeNode.leftTree, depth + 1);
            }

            if (treeNode.rightTree != null)
            {
                GeneratePath(treeNode.rightTree, depth + 1);
            }

            if (treeNode.leftTree == null || treeNode.rightTree == null)
                return;

            RectInt leftRoom = BspRoomFinder.GetRightLeafRoom(treeNode.leftTree);
            RectInt rightRoom = BspRoomFinder.GetLeafRoom(treeNode.rightTree);

            ConnectRoomWithAstar(leftRoom, rightRoom);
        }

        private void ConnectRoomWithAstar(RectInt roomA, RectInt roomB)
        {
            ExitFindEndConnection.GetConnectionPoints(roomA, roomB, out Vector2Int start, out Vector2Int end);

            Debug.Log(
                   $"[ConnectRoomWithAstar] " +
                   $"roomA={roomA}, roomB={roomB}, start={start}, end={end}"
               );

            Debug.Log(
                $"[ConnectRoomWithAstar] " +
                $"startInBounds={IsInBounds(start)}, endInBounds={IsInBounds(end)}"
            );

            if (IsInBounds(start))
            {
                Debug.Log($"[ConnectRoomWithAstar] startTile={ctx.MapData[start.x, start.y]}");
            }

            if (IsInBounds(end))
            {
                Debug.Log($"[ConnectRoomWithAstar] endTile={ctx.MapData[end.x, end.y]}");
            }

            ctx.DoorCandidates.Add(start);
            ctx.DoorCandidates.Add(end);

            List<Vector2Int> path = aStarPathFinder.FindPath(start, end);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning($"A* path not found: {start} -> {end}");
                return;
            }

            Debug.Log($"[ConnectRoomWithAstar] path found, count={path.Count}");

            CarvePath(path, start, end);
        }

        // 문 밖 영역 추출 메서드
        private Vector2Int GetPorchPoint(Vector2Int doorPos, Vector2Int outwardDir, int porchLength)
        {
            return doorPos + outwardDir * porchLength;
        }

        private void CarvePath(List<Vector2Int> path, Vector2Int start, Vector2Int end)
        {

            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int p = path[i];

                if (!IsInBounds(p))
                    continue;

                TileType current = ctx.MapData[p.x, p.y];
                if (p == start || p == end)
                {
                    continue;
                }

                if (current == TileType.Room)
                {
                    Debug.Log($"[CarvePath] Skip room tile: {p}");
                    continue;
                }

                Debug.Log($"[CarvePath] Carve path at {p}, previous={current}");
                ctx.MapData[p.x, p.y] = TileType.Path;
            }
        }

        private bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < ctx.MapSize.x &&
                pos.y >= 0 && pos.y < ctx.MapSize.y;
        }

    }

}