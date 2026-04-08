using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BSPDungeonGenrator.Generation
{

    public class PathGenerator
    {

        private DungeonContext ctx;

        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            ctx.RoomConnections.Clear();


            //List<RoomConnection> connections = new List<RoomConnection> ();
            BuildRoomConnections(ctx.Root);

            foreach (var conn in ctx.RoomConnections)
            {
                RoomInfo roomA = GetRoomById(conn.FromRoomId);
                RoomInfo roomB = GetRoomById(conn.ToRoomId);   

                if (roomA == null || roomB == null) 
                    continue;

                ConnectRooms(roomA, roomB);
            }

            Debug.Log($"[PathGenerator] connections.Count = {ctx.RoomConnections.Count}");
            foreach (var conn in ctx.RoomConnections)
            {
                Debug.Log($"Connect {conn.FromRoomId} -> {conn.ToRoomId}");
            }

        }

        // 방 연결요소 저장
        private RoomInfo BuildRoomConnections(TreeNode node)
        {
            if(node == null) 
                return null;

            bool isLeaf = node.leftTree == null && node.rightTree == null;
            if(isLeaf)
            {
                RoomInfo room = FindRoomByRect(node.dungeonSize);
                Debug.Log($"[Leaf] dungeonSize={node.dungeonSize}, matched={(room != null ? room.RoomId.ToString() : "NULL")}");
                return room;
            }

            RoomInfo leftRoom = BuildRoomConnections(node.leftTree);
            RoomInfo rightRoom = BuildRoomConnections(node.rightTree);

            if(leftRoom != null && rightRoom != null && leftRoom.RoomId != rightRoom.RoomId)
            {
                Debug.Log($"[ConnectCheck] left={leftRoom.RoomId}, right={rightRoom.RoomId}");
                ctx.RoomConnections.Add(new DungeonContext.RoomConnection(leftRoom.RoomId, rightRoom.RoomId));
            }

            if(leftRoom != null && rightRoom != null)
                return Random.value < 0.5f ? leftRoom : rightRoom;

            return leftRoom ?? rightRoom;
        }

        // 방 직접연결
        private void ConnectRooms (RoomInfo roomA,  RoomInfo roomB)
        {
            Vector2Int start = GetClosestEdgePoint(roomA.Rect, roomB.Center);
            Vector2Int end = GetClosestEdgePoint(roomB.Rect, roomA.Center);

            List<Vector2Int> pathA = BuildPathMidX(start, end);
            List<Vector2Int> pathB = BuildPathMidY(start, end);

            int scoreA = EvaluatePath(pathA);
            int scoreB = EvaluatePath(pathB);

            List<Vector2Int> bestPath = scoreA <= scoreB ? pathA : pathB;
            ApplyPath(bestPath);

            Debug.Log($"roomA={roomA.RoomId}, roomB={roomB.RoomId}");
            Debug.Log($"scoreMidX={scoreA}, scoreMidY={scoreB}");
            Debug.Log($"pathMidX.Count={pathA.Count}, pathMidY.Count={pathB.Count}");
        }

        // 가까운 벽 체크
        private Vector2Int GetClosestEdgePoint(RectInt room, Vector2Int target)
        {
            Vector2Int center = new Vector2Int(
                room.x + room.width / 2,
                room.y + room.height / 2
                );

            int dx = target.x - center.x;
            int dy = target.y - center.y;

            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            {
                if (dx >= 0)
                    return new Vector2Int(room.xMax, center.y);
                else
                    return new Vector2Int(room.xMin - 1, center.y);
            }
            else
            {
                if (dy >= 0)
                    return new Vector2Int(center.x, room.yMax);
                else
                    return new Vector2Int(center.x, room.yMin - 1);
            }
        }

        // x축 경로 후보
        private List<Vector2Int> BuildPathMidX(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            int midX = (start.x + end.x) / 2;

            AppendHorizontal(result, start.x, midX, start.y);
            AppendVertical(result, start.y, end.y, midX);
            AppendHorizontal(result, midX, end.x, end.y);

            return result;
        }

        // y축 경로 후보
        private List<Vector2Int> BuildPathMidY(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            int midY = (start.y + end.y) / 2;

            AppendVertical(result, start.y, midY, start.x);
            AppendHorizontal(result, start.x, end.x, midY);
            AppendVertical(result, midY, end.y, end.x);

            return result;
        }

        private int EvaluatePath(List<Vector2Int> path)
        {
            int score = 0;

            foreach (var p in path)
            {
                if (!IsInsideMap(p.x, p.y))
                    return int.MaxValue;

                if (ctx.MapData[p.x, p.y] == TileType.Room)
                    return int.MaxValue;

                int roomAdj = CountAdjacentRooms(p.x, p.y);
                int diagAdj = CountDiagonalRooms(p.x, p.y);

                score += roomAdj * 10;
                score += diagAdj * 3;
                score += 1; // 길이 최소
            }
            return score;
        }


        

        // 경로 적용
        private void ApplyPath(List<Vector2Int> path)
        {
            foreach(var p in path)
            {
                if (!IsInsideMap(p.x, p.y))
                    continue;
                if (ctx.MapData[p.x, p.y] == TileType.Room)
                    continue;

                ctx.MapData[p.x, p.y] = TileType.Path;
            }
        }


        private void AppendHorizontal(List<Vector2Int> list, int xStart, int xEnd, int y)
        {
            for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            {
                Vector2Int p = new Vector2Int(x, y);
                if (list.Count == 0 || list[list.Count - 1] != p)
                    list.Add(p);
            }
        }

        private void AppendVertical(List<Vector2Int> list, int yStart, int yEnd, int x)
        {
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
                Vector2Int p = new Vector2Int(x, y);
                if (list.Count == 0 || list[list.Count - 1] != p)
                    list.Add(p);
            }
        }


        private int CountAdjacentRooms(int x, int y)
        {
            int count = 0;

            if (IsInsideMap(x + 1, y) && ctx.MapData[x + 1, y] == TileType.Room) count++;
            if (IsInsideMap(x - 1, y) && ctx.MapData[x - 1, y] == TileType.Room) count++;
            if (IsInsideMap(x, y + 1) && ctx.MapData[x, y + 1] == TileType.Room) count++;
            if (IsInsideMap(x, y - 1) && ctx.MapData[x, y - 1] == TileType.Room) count++;

            return count;
        }

        private int CountDiagonalRooms(int x, int y)
        {
            int count = 0;

            if (IsInsideMap(x + 1, y + 1) && ctx.MapData[x + 1, y + 1] == TileType.Room) count++;
            if (IsInsideMap(x - 1, y + 1) && ctx.MapData[x - 1, y + 1] == TileType.Room) count++;
            if (IsInsideMap(x + 1, y - 1) && ctx.MapData[x + 1, y - 1] == TileType.Room) count++;
            if (IsInsideMap(x - 1, y - 1) && ctx.MapData[x - 1, y - 1] == TileType.Room) count++;

            return count;
        }

        // 방 Id 참조하기
        private RoomInfo GetRoomById(int roomId)
        {
            for (int i = 0; i < ctx.Rooms.Count; i++)
            {
                if (ctx.Rooms[i].RoomId == roomId)
                    return ctx.Rooms[i];
            }
            return null;
        }

        // 방 배열 찾기
        private RoomInfo FindRoomByRect(RectInt rect)
        {
            for (int i = 0; i < ctx.Rooms.Count; i++)
            {
                Debug.Log($"[FindRoomByRect] target={rect}, room[{i}]={ctx.Rooms[i].Rect}");
                if (ctx.Rooms[i].Rect == rect)
                    return ctx.Rooms[i];
            }
            return null;
        }

        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }

        public struct RoomConnection
        {
            public int FromRoomId;
            public int ToRoomId;

            public RoomConnection(int fromRoomId, int toRoomId)
            {
                FromRoomId = fromRoomId;
                ToRoomId = toRoomId;
            }
        }
    }

}