using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Generation
{
    public class BspPartitioner
    {
        private DungeonConfig config;

        public BspPartitioner(DungeonConfig config)
        {
            this.config = config;
        }

        public BspNode CreateTree()
        {
            IntRect rootRect = new IntRect
                (0, 0, config.mapWidthInCells, config.mapHeightInCells);
            BspNode root = new BspNode(rootRect);
            SplitRecursive(root, 0);
            return root;
        }

        private void SplitRecursive(BspNode node, int depth)
        {

            float area = node.Bounds.width * node.Bounds.height;
            float minArea = config.minLeafSize.x * config.minLeafSize.y * 2.5f;

            if (area < minArea && depth >= (config.maxDepth - 1))
                return;

            bool canSplitHorizontally = 
                node.Bounds.height >= config.minLeafSize.y * 2;
            bool canSplitVertically = 
                node.Bounds.width >= config.minLeafSize.x * 2;

            if(!canSplitHorizontally && !canSplitVertically)
                return;

            bool splitVertical;
            if (canSplitHorizontally && canSplitVertically)
            {
                splitVertical = Random.value > 0.5f;
            }
            else
            {
                splitVertical = canSplitVertically;
            }

            if (splitVertical)
            {
                int min = node.Bounds.x + config.minLeafSize.x;
                int max = node.Bounds.x + node.Bounds.width - config.minLeafSize.x;
                
                if (max <= min)
                    return;

                int splitX = Random.Range(min, max);

                IntRect left = new IntRect
                    (node.Bounds.x, node.Bounds.y, splitX - node.Bounds.x, node.Bounds.height);
                IntRect right = new IntRect
                    (splitX, node.Bounds.y, node.Bounds.xMax - splitX, node.Bounds.height);

                node.Left = new BspNode(left) { Parent = node };
                node.Right = new BspNode(right) { Parent = node };
            }
            else
            {
                int min = node.Bounds.y + config.minLeafSize.y;
                int max = node.Bounds.y + node.Bounds.height - config.minLeafSize.y;

                if (max <= min)
                    return;

                int splitY = Random.Range(min, max);

                IntRect bottom = new IntRect
                    (node.Bounds.x, node.Bounds.y, node.Bounds.width, splitY - node.Bounds.y);
                IntRect top = new IntRect
                    (node.Bounds.x, splitY, node.Bounds.width, node.Bounds.yMax - splitY);

                node.Left = new BspNode(bottom) { Parent = node };
                node.Right = new BspNode(top) { Parent = node };

            }

            SplitRecursive(node.Left, depth + 1);
            SplitRecursive(node.Right, depth + 1);

        }
    }
}
