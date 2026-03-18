using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace BSPDuengeonGenrator.Generation
{

    public class PathGenerator
    {

        private DungeonContext ctx;

        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
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

            // 연결 방향은 랜덤
            if (Random.value < 0.5f)
            {
                CreateHorizontalCorridor(leftCenter.x, rightCenter.x, leftCenter.y);
                CreateVerticalCorridor(leftCenter.y, rightCenter.y, rightCenter.x);
            }
            else
            {
                CreateVerticalCorridor(leftCenter.y, rightCenter.y, leftCenter.x);
                CreateHorizontalCorridor(leftCenter.x, rightCenter.x, leftCenter.y);
            }

            // 길 생성
            GeneratePath(treeNode.leftTree, depth + 1);
            GeneratePath(treeNode.rightTree, depth + 1);
        }
        // 수평 통로
        private void CreateHorizontalCorridor(int xStart, int xEnd, int y)
        {

            for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            {
                // 통로 두께 계산
                for (int w = -1; w <= 1; w++)
                {
                    int ny = y + w;
                    if (!IsInsideMap(x, ny)) 
                        continue;

                    if (ctx.MapData[x, ny] == TileType.Room)
                        continue;

                    ctx.MapData[x, ny] = TileType.Path;
                }
            }
        }

        // 수직 통로
        private void CreateVerticalCorridor(int yStart, int yEnd, int x)
        { 
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
                // 통로 두께 계산
                for (int w = -1; w <= 1; w++)
                {
                    int nx = x + w;
                    if (!IsInsideMap(nx, y))
                        continue;

                    if (ctx.MapData[nx, y] == TileType.Room)
                        continue;

                    ctx.MapData[nx, y] = TileType.Path;
                }
            }
        }


        // 방 중심 계산
        private Vector2Int GetRoomCenter(RectInt room)
        {
            return new Vector2Int
                (room.x + room.width / 2, room.y + room.height / 2);
        }
        // 맵 범위 체크 (예외방지)
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }

        private Vector2Int GetExitPoint(RectInt room, Vector2Int targetCenter)
        {
            Vector2Int roomCenter = GetRoomCenter(room);

            int dx = targetCenter.x - roomCenter.x;
            int dy = targetCenter.y - roomCenter.y;

            if (Mathf.Abs(dx) > Mathf.Abs(dy))
            {
                if (dx > 0)
                {
                    return new Vector2Int(room.xMax -1, roomCenter.y);
                }
                else
                {
                    return new Vector2Int(room.xMin, roomCenter.y);
                }
            }
            else
            {
                if (dy > 0)
                {
                    return new Vector2Int(roomCenter.x, room.yMax - 1);
                }
                else
                {
                    return new Vector2Int(roomCenter.x, room.yMin);
                }
            }
        }

        private void CreateCorrider()
        {

        }

    }

}