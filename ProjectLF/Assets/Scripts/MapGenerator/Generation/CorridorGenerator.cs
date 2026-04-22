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
            List<BspNode> leafNodes = new List<BspNode>();

            CollectLeaves(root, leafNodes);

            Debug.Log($"[Corridor] leaf count = {leafNodes.Count}");

            for(int i = 0; i < leafNodes.Count - 1; i++)
            {
                BspNode a = leafNodes[i];
                BspNode b = leafNodes[i + 1];

                if (!a.RoomBounds.HasValue || !b.RoomBounds.HasValue)
                    continue;

                IntRect roomA = a.RoomBounds.Value;
                IntRect roomB = b.RoomBounds.Value;

                DoorDir dirA = GetDoorDirection(roomA, roomB);
                DoorDir dirB = GetOpposite(dirA);


                Vector2Int roomDoorA = GetRoomDoorCell(roomA, dirA);
                Vector2Int roomDoorB = GetRoomDoorCell(roomB, dirB);

                Vector2Int start = GetOutsideDoorCell(roomA, dirA);
                Vector2Int end = GetOutsideDoorCell(roomB, dirB);

                context.RoomEnteranceCells.Add(roomDoorA);
                context.RoomEnteranceCells.Add(roomDoorB);

                Connect(start, end);
            }
        }

        private void CollectLeaves(BspNode node, List<BspNode> leaves)
        {
            if (node == null) 
                return;
            if(node.IsLeaf)
            {
                if(node.RoomBounds.HasValue)
                {
                    leaves.Add(node);
                }

                return;
            }
            CollectLeaves(node.Left, leaves);
            CollectLeaves(node.Right, leaves);
        }


        private void Connect(Vector2Int val1, Vector2Int val2)
        {
            if(Random.value > 0.5f)
            {
                DigHorizontal(val1.x, val2.x, val1.y);
                DigVertical(val1.y, val2.y, val2.x);
            }
            else
            {
                DigVertical(val1.y, val2.y, val1.x);
                DigHorizontal(val1.x, val2.x, val2.y);
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

            for(int dx = -half; dx <= half; dx++)
            {
                for(int dy = -half; dy <= half; dy++)
                {
                    int x = centerX + dx;
                    int y = centerY + dy;
                    
                    if(!context.IsInside(x, y))
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
                        return;

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

            int centerX = room.xMin + room.width / 2;
            int centerY = room.yMin + room.height / 2;
            
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
            Vector2Int a = from.Center;
            Vector2Int b = to.Center;

            int dx = b.x - a.x;
            int dy = b.y - a.y;

            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            {
                return dx >= 0 ? DoorDir.Right : DoorDir.Left;
            }
            else            
            {
                return dy >= 0 ? DoorDir.Up : DoorDir.Down;
            }
        }

        private Vector2Int GetOutsideDoorCell(IntRect room, DoorDir dir)
        {
            int left = room.xMin;
            int right = room.xMax - 1;
            int bottom = room.yMin;
            int top = room.yMax - 1;

            int centerX = room.xMin + room.width / 2;
            int centerY = room.yMin + room.height / 2;

            switch (dir)
            {
                case DoorDir.Up:
                    return new Vector2Int(centerX, top + 1);
                case DoorDir.Down:
                    return new Vector2Int(centerX, bottom - 1);
                case DoorDir.Left:
                    return new Vector2Int(left - 1, centerY);
                case DoorDir.Right:
                    return new Vector2Int(right + 1, centerY);

            }

            return new Vector2Int(centerX, centerY);
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


    }
}


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