using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Generation
{ 
    public class CorriderGenerator
    {
        private readonly DungeonConfig config;
        private readonly DungeonContext context;

        public CorriderGenerator(DungeonConfig config, DungeonContext context)
        {
            this.config = config;
            this.context = context;
        }

        public void Generate(BspNode node)
        {
            if(node == null || node.IsLeaf) 
                return;

            Generate(node.Left);
            Generate(node.Right);

            Vector2Int? leftCenter = FindRoomCenter(node.Left);
            Vector2Int? rightCenter = FindRoomCenter(node.Right);

            if(leftCenter.HasValue && rightCenter.HasValue)
            {
                Connect(leftCenter.Value, rightCenter.Value);
            }
        }
        private Vector2Int? FindRoomCenter(BspNode node)
        {
            if (node == null) 
                return null;

            if(node.IsLeaf)
            {
                //if(node.RoomBounds.HasValue)
                //    return node.RoomBounds.Value.Center;
                return null;
            }

            Vector2Int? left = FindRoomCenter(node.Left);
            if(left.HasValue)
                return left;

            return FindRoomCenter(node.Right);
        }

        private void Connect(Vector2Int val1, Vector2Int val2)
        {
            if(Random.value > 0.5f)
            {
                DigHorizontal(val1.x, val2.x, val1.y);
                DigVertical(val1.y, val2.y, val2.x);
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
                PaintCorridorWidth(x, y);
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

                    if (context.Grid[x, y] == CellType.Empty)
                    {
                        context.Grid[x, y] = CellType.Corridor;
                    }
                    context.CorridorCells.Add(new Vector2Int(x, y));
                }
            }
        }
    }
}
