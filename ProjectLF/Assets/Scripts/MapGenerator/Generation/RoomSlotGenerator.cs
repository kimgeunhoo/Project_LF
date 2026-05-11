using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Generation
{
    public class RoomSlotGenerator
    {
        private readonly DungeonConfig config;
        private readonly DungeonContext context;

        public RoomSlotGenerator(DungeonConfig config, DungeonContext context)
        {
            this.config = config;
            this.context = context;
        }

        public void Generate(BspNode node)
        {
            if(node == null) 
                return;

            if(node.IsLeaf)
            {
                TryCreateRoom(node);
            }

            Generate(node.Left);
            Generate(node.Right);
        }

        private void TryCreateRoom(BspNode node)
        {
            int pad = config.splitPadding;

            int maxAvailableWidth = node.Bounds.width - 2 * pad;
            int maxAvailableHeight = node.Bounds.height - 2 * pad;

            int minRequiredWidth = Mathf.Max(3, config.roomSizeInCells.x - 2);
            int minRequiredHeight = Mathf.Max(3, config.roomSizeInCells.y - 2);

            if(maxAvailableWidth < minRequiredWidth || maxAvailableHeight < minRequiredHeight)
            {
                node.RoomBounds = null;
                return;
            }

            int roomWidth = Mathf.Min(config.roomSizeInCells.x, maxAvailableWidth);
            int roomHeight = Mathf.Min(config.roomSizeInCells.y, maxAvailableHeight);

            int minX = node.Bounds.x + pad;
            int maxX = node.Bounds.xMax - pad - roomWidth;
            int minY = node.Bounds.y + pad;
            int maxY = node.Bounds.yMax - pad - roomHeight;

            int roomX = (maxX > minX) ? Random.Range(minX, maxX + 1) : minX;
            int roomY = (maxY > minY) ? Random.Range(minY, maxY + 1) : minY;

            IntRect roomRect = new IntRect(roomX, roomY, roomWidth, roomHeight);

            node.RoomBounds = roomRect;
            context.Rooms.Add(roomRect);

            for (int x = roomRect.x; x < roomRect.xMax; x++)
            {
                for (int y = roomRect.y; y < roomRect.yMax; y++)
                {
                    context.Grid[x, y] = CellType.Room;
                }
            }

        }


        // ºýºýÇÑ ¹æ »ý¼º±â
        //private void TryCreateRoom(BspNode node)
        //{
        //    int roomWidth = config.roomSizeInCells.x;
        //    int roomHeight = config.roomSizeInCells.y;
        //    int pad = config.splitPadding;

        //    int availableWidth = node.Bounds.width - 2 * pad;
        //    int availableHeight = node.Bounds.height - 2 * pad;

        //    if (availableWidth < roomWidth || availableHeight < roomHeight)
        //    {
        //        node.RoomBounds = null;
        //        return;
        //    }

        //    int minX = node.Bounds.x + pad;
        //    int maxX = node.Bounds.xMax - pad - roomWidth;
        //    int minY = node.Bounds.y + pad;
        //    int maxY = node.Bounds.yMax - pad - roomHeight;

        //    int roomX = node.Bounds.x + (node.Bounds.width - roomWidth) / 2;
        //    int roomY = node.Bounds.y + (node.Bounds.height - roomHeight) / 2;


        //    //int roomX = (maxX > minX) ? Random.Range(minX, maxX + 1) : minX;
        //    //int roomY = (maxY > minY) ? Random.Range(minY, maxY + 1) : minY;

        //    IntRect roomRect = new IntRect(roomX, roomY, roomWidth, roomHeight);
        //    node.RoomBounds = roomRect;
        //    context.Rooms.Add(roomRect);

        //    for (int x = roomRect.x; x < roomRect.xMax; x++)
        //    {
        //        for (int y = roomRect.y; y < roomRect.yMax; y++)
        //        {
        //            context.Grid[x, y] = CellType.Room;
        //        }
        //    }
        //}

    }

}