using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
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
            GenerateDeungeuon(ctx.Root, 0);
        }
        private RectInt GenerateDeungeuon(TreeNode treeNode, int node)
        {
            if (node == ctx.MaxNode)
            {
                RectInt size = treeNode.treeSize;
                // 트리 범위 내에서 무작위 크기 선택, 최소 크기 : width / 2
                //int width = Mathf.Max(Random.Range(size.width / 2, size.width - 1));
                //int height = Mathf.Max(Random.Range(size.height / 2, size.height - 1));

                int width = Random.Range(size.width / 2, size.width - 1);
                int height = Random.Range(size.height / 2, size.height - 1);

                // 최대 크기 : width / 2
                int x = treeNode.treeSize.x + Random.Range(1, size.width - width);
                int y = treeNode.treeSize.y + Random.Range(1, size.height - height);
                // 던전 렌더링
                OnDrawDungeon(x, y, width, height);
                // 리턴 값은 던전의 크기로 길을 생성할 때 크기 정보로 활용
                return new RectInt(x, y, width, height);
            }
            // 리턴 값 = 던전 크기
            treeNode.leftTree.dungeonSize = GenerateDeungeuon(treeNode.leftTree, node + 1);
            treeNode.rightTree.dungeonSize = GenerateDeungeuon(treeNode.rightTree, node + 1);
            // 부모 트리의 던전 크기는 자식 트리의 던전 크기 그대로 사용
            return treeNode.leftTree.dungeonSize;
        }

        // 크기에 맞춰 타일을 생성하는 메소드
        private void OnDrawDungeon(int x, int y, int width, int height)
        {
            //if (ctx == null) { Debug.LogError("[OnDrawDuengeon] ctx null"); return; }
            //if (ctx.MapData == null) { Debug.LogError("[OnDrawDuengeon] MapData null"); return; }
            //if (ctx.FloorTilemap == null && ctx.FloorTilemap == null) { Debug.LogError("[OnDrawDuengeon] Tilemap null"); return; }
            //if (ctx.PathTiles == null || ctx.PathTiles.Length == 0) { Debug.LogError("[OnDrawDuengeon] PathTiles empty"); return; }
            //// 실제 사용 타일맵 확인용 로그
            //Debug.Log($"[OnDrawDuengeon] x={x} y={y} w={width} h={height}");
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    ctx.MapData[i, j] = TileType.Room;

                }
            }
        }
    }

}