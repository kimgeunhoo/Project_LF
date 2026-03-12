using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using Unity.VisualScripting;


namespace BSPDungeonGenrator.Generation
{
   

    public class PathGenerator
    {

        private DungeonContext ctx;
        private int pathLength = 0; 

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

            Vector2Int leftDoor = GetExitPoint(leftRoom, rightCenter);
            Vector2Int rightDoor = GetExitPoint(rightRoom, leftCenter);
            
            Vector2Int leftOutside = GetOutsidePoint(leftRoom, leftDoor);
            Vector2Int rightOutside = GetOutsidePoint(rightRoom, rightDoor);

            //ctx.MapData[leftDoor.x, leftDoor.y] = TileType.Door;
            //ctx.MapData[rightDoor.x, rightDoor.y] = TileType.Door;

            // 연결 방향은 랜덤
            if (treeNode.isVerticalSplit)
            {
                ConnectViaVerticalSplit(leftOutside, rightOutside, treeNode.splitCoord);
            }
            else
            {
                ConnectViaHoriaontalSplit(leftOutside,rightOutside,treeNode.splitCoord);
            }

            // 길 생성
            GeneratePath(treeNode.leftTree, depth + 1);
            GeneratePath(treeNode.rightTree, depth + 1);
        }

        // 분할선 따라서 통로 꺾기 : 세로
        private void ConnectViaVerticalSplit(Vector2Int a,  Vector2Int b, int splitX)
        {
            Vector2Int p1 = new Vector2Int(splitX, a.y);
            Vector2Int p2 = new Vector2Int(splitX, b.y);

            CreateHorizontalCorridor(a.x, p1.x, a.y);
            CreateVerticalCorridor(p1.y, p2.y, splitX);
            CreateHorizontalCorridor(p2.x, b.x, b.y);
        }

        // 분할선 따라서 통로 꺾기 : 가로
        private void ConnectViaHoriaontalSplit(Vector2Int a, Vector2Int b, int splitY)
        {
            Vector2Int p1 = new Vector2Int(a.x, splitY);
            Vector2Int p2 = new Vector2Int(b.x, splitY);

            CreateVerticalCorridor(a.y, p1.y, a.x);
            CreateHorizontalCorridor(p1.x, p2.x, splitY);
            CreateVerticalCorridor(p2.y, b.y, b.x);
        }


        // 수평 통로
        private void CreateHorizontalCorridor(int xStart, int xEnd, int y)
        {
            for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
            {
                // 통로 두께 계산
                for (int w = -pathLength; w <= pathLength; w++) 
                {
                    int ny = y + w;
                    if (!IsInsideMap(x, y)) 
                        continue;
                    
                    if (ctx.MapData[x, y] == TileType.Room) 
                        continue;
                    // 이미 같은 경로에 통로가 생성되어 있다면 스킵한다
                    if (ctx.MapData[x, y] == TileType.Path) 
                        continue;
                    ctx.MapData[x, y] = TileType.Path;
                
                    //if (ctx.MapData[x, y] == TileType.Wall || ctx.MapData[x, ny] == TileType.Empty)
                    //{
                    //    ctx.MapData[x, y] = TileType.Path;
                    //}

                }
            }
        }

        // 수직 통로
        private void CreateVerticalCorridor(int yStart, int yEnd, int x)
        {
            for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
            {
                // 통로 두께 계산
                for (int w = -pathLength; w <= pathLength; w++) 
                {
                    int nx = x + w;
                    if (!IsInsideMap(x, y)) 
                        continue;

                    // 이미 같은 경로에 통로, 방이 생성되어 있다면 스킵한다
                    if (ctx.MapData[x, y] == TileType.Room) 
                        continue;
                    if (ctx.MapData[x, y] == TileType.Path) 
                        continue;

                    ctx.MapData[x, y] = TileType.Path;
                
                    //if (ctx.MapData[nx, y] == TileType.Wall || ctx.MapData[nx, y] == TileType.Empty)
                    //{
                    //    ctx.MapData[nx, y] = TileType.Path;
                    //}
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
                    return new Vector2Int(room.xMax - 1, roomCenter.y);
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
        private Vector2Int GetOutsidePoint(RectInt room, Vector2Int door)
        {
            Vector2Int center = GetRoomCenter(room);

            if (door.x == room.xMax - 1)
                return new Vector2Int(door.x + 1, door.y);
            if (door.x == room.xMin)
                return new Vector2Int(door.x - 1, door.y);
            if (door.y == room.yMax - 1)
                return new Vector2Int(door.x, door.y + 1);
            return new Vector2Int(door.x, door.y - 1);
        }


        private void CreateCorridor()
        {

        }

    }

}