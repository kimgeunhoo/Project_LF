using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
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
            ctx.CorriderCells.Clear();
            GeneratePath(ctx.Root, 0);
        }
        // 길 연결 메서드
        private void GeneratePath(TreeNode treeNode, int depth)
        {
            // 노드가 최하위일 때는 길을 연결하지 않음. 최하위 노드는 자식 트리가 없다.
            if (treeNode == null || treeNode.leftTree == null || treeNode.rightTree == null) 
                return;
            // 자식 트리의 던전 중앙 위치를 가져옴
            RectInt leftRoom = treeNode.leftTree.dungeonSize;
            RectInt rightRoom = treeNode.rightTree.dungeonSize;

            // 중심 계산
            Vector2Int leftCenter = GetRoomCenter(leftRoom);
            Vector2Int rightCenter = GetRoomCenter(rightRoom);

            Vector2Int leftExit = GetExitPoint(leftRoom, rightCenter);
            Vector2Int rightExit = GetExitPoint(rightRoom, leftCenter);

            // 연결 방향은 랜덤, L자 방식
            if (Random.value < 0.5f)
            {
                AddHorizontalCorrider(leftExit.x, rightExit.x, leftExit.y);
                AddVerticalCorridor(leftExit.y, rightExit.y, rightExit.x);
            }
            else
            {
                AddVerticalCorridor(leftExit.y, rightExit.y, leftExit.x);
                AddHorizontalCorrider(leftExit.x, rightExit.x, rightExit.y);
            }

            // 길 생성
            GeneratePath(treeNode.leftTree, depth + 1);
            GeneratePath(treeNode.rightTree, depth + 1);
        }

        private void AddHorizontalCorrider(int xStart, int xEnd, int y)
        {
            int min = Mathf.Min(xStart, xEnd);
            int max = Mathf.Max(xStart, xEnd);

            for (int x = min; x <= max; x++)
            {
                AddCorriderCell(x, y);
            }
        }
        private void AddVerticalCorridor(int yStart, int yEnd, int x)
        {
            int min = Mathf.Min(yStart, yEnd);
            int max = Mathf.Max(yStart, yEnd);

            for (int y = min; y <= max; y++)
            {
                AddCorriderCell(x, y);
            }
        }

        private void AddCorriderCell(int x, int y)
        {
            if (!IsInsideMap(x, y))
                return;
            Vector2Int pos = new Vector2Int(x, y);

            if (ctx.MapData[x, y] == TileType.Room)
                return; 

            ctx.CorriderCells.Add(pos);
        }

        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }


        // 방 중심 계산
        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int
                (room.x + room.width / 2, room.y + room.height / 2);
        }
        // 출구 계산
        private Vector2Int GetExitPoint(RectInt room, Vector2Int targetCenter)
        {
            Vector2Int roomCenter = GetRoomCenter(room);

            int dx = targetCenter.x - roomCenter.x;
            int dy = targetCenter.y - roomCenter.y;

            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            {
                if (dx > 0)
                {
                    return new Vector2Int(room.xMax, roomCenter.y);
                }
                else
                {
                    return new Vector2Int(room.xMin - 1, roomCenter.y);
                }
            }
            else
            {
                if (dy > 0)
                {
                    return new Vector2Int(roomCenter.x, room.yMax);
                }
                else
                {
                    return new Vector2Int(roomCenter.x, room.yMin - 1);
                }
            }
        }


       

    }

}