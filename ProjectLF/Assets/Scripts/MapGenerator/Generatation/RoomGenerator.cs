using UnityEngine;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;

namespace BSPDungeonGenrator.Generation
{
    public class RoomGenerater
    {

        private DungeonContext ctx;
        public void Run(DungeonContext ctx)
        {
            this.ctx = ctx;
            GenerateDeungeuon(ctx.Root);
        }

        private RectInt GetRoomBuildArea(RectInt area, int padding)
        {
            int x = area.x + padding;
            int y = area.y + padding;
            int w = area.width - padding * 2;
            int h = area.height - padding * 2;
            if (w < 5 || h < 5) 
                return new RectInt(area.x, area.y, 0, 0);

            return new RectInt(x, y, w, h);
        }
        private RectInt GenerateDeungeuon(TreeNode treeNode)
        {
            if (treeNode == null)
                return default;

            bool isLeaf = treeNode.leftTree == null && treeNode.rightTree == null;

            if(isLeaf)
            {
                RectInt room = CreateRoomInLeaf(treeNode.treeSize);
                treeNode.dungeonSize = room;
                PaintRoom(room);
                return room;
            }

            RectInt leftRoom = GenerateDeungeuon(treeNode.leftTree);
            RectInt rightRoom = GenerateDeungeuon(treeNode.rightTree);

            treeNode.dungeonSize = Random.value < 0.5f ? leftRoom : rightRoom;
            return treeNode.dungeonSize;
        }

        private RectInt CreateRoomInLeaf(RectInt leaf)
        {
            int padding = Mathf.Max(1, ctx.RoomPadding);

            int availableWidth = leaf.width - padding * 2;
            int availableHeight = leaf.height - padding * 2;

            int minWidth = Mathf.Min(ctx.MinRoomWidth, availableWidth);
            int minHeight = Mathf.Min(ctx.MinRoomHeight, availableHeight);

            // leaf가 너무 작으면 안전하게 fallback
            if (availableWidth <= 2 || availableHeight <= 2)
            {
                int fx = Mathf.Clamp(leaf.x + 1, leaf.x, leaf.xMax - 1);
                int fy = Mathf.Clamp(leaf.y + 1, leaf.y, leaf.yMax - 1);
                int fw = Mathf.Max(1, leaf.width - 2);
                int fh = Mathf.Max(1, leaf.height - 2);

                return new RectInt(fx, fy, fw, fh);
            }

            int roomWidth = Random.Range(minWidth, availableWidth + 1);
            int roomHeight = Random.Range(minHeight, availableHeight + 1);

            int minX = leaf.x + padding;
            int maxX = leaf.xMax - padding - roomWidth;

            int minY = leaf.y + padding;
            int maxY = leaf.yMax - padding - roomHeight;

            if (maxX < minX) maxX = minX;
            if (maxY < minY) maxY = minY;

            int roomX = Random.Range(minX, maxX + 1);
            int roomY = Random.Range(minY, maxY + 1);

            return new RectInt(roomX, roomY, roomWidth, roomHeight);
        }
        // 크기에 맞춰 타일을 생성하는 메소드
        private void PaintRoom(RectInt room)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    if (!IsInsideMap(x, y))
                        continue;

                    ctx.MapData[x, y] = TileType.Room;
                }
            }    
        }
        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && y >= 0 && x < ctx.MapSize.x && y < ctx.MapSize.y;
        }

    }

}
//if (depth == ctx.MaxNode)
//{
//    RectInt size = treeNode.treeSize;
//    // 트리 범위 내에서 무작위 크기 선택, 최소 크기 : width / 2
//    //int width = Mathf.Max(Random.Range(size.width / 2, size.width - 1));
//    //int height = Mathf.Max(Random.Range(size.height / 2, size.height - 1));

//    int width = Random.Range(size.width / 2, size.width - 1);
//    int height = Random.Range(size.height / 2, size.height - 1);

//    // 최대 크기 : width / 2
//    int x = treeNode.treeSize.x + Random.Range(1, size.width - width);
//    int y = treeNode.treeSize.y + Random.Range(1, size.height - height);
//    // 던전 렌더링
//    OnDrawDungeon(x, y, width, height);
//    // 리턴 값은 던전의 크기로 길을 생성할 때 크기 정보로 활용
//    return new RectInt(x, y, width, height);
//}
//// 리턴 값 = 던전 크기
//treeNode.leftTree.dungeonSize = GenerateDeungeuon(treeNode.leftTree, depth + 1);
//treeNode.rightTree.dungeonSize = GenerateDeungeuon(treeNode.rightTree, depth + 1);

//treeNode.dungeonSize = Random.value < 0.5f
//    ? treeNode.leftTree.dungeonSize : treeNode.rightTree.dungeonSize;

//// 부모 트리의 던전 크기는 자식 트리의 던전 크기 그대로 사용
//return treeNode.dungeonSize;