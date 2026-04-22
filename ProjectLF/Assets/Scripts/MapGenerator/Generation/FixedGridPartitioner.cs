using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace ModularBSP.Generation
{
    public class FixedGridPartitioner : MonoBehaviour
    {
        private readonly DungeonConfig config;

        public FixedGridPartitioner(DungeonConfig config)
        {
            this.config = config;
        }

        public BspNode CreateTree()
        {
            IntRect rootRect = new IntRect(0, 0, config.mapWidthInCells, config.mapHeightInCells);
            BspNode root = new BspNode(rootRect);

            int leafWidth = config.cellSize * config.roomSizeInCells.x;
            int leafHeight = config.cellSize * config.roomSizeInCells.y;

            int cols = config.mapWidthInCells / leafWidth;
            int rows = config.mapHeightInCells / leafHeight;

            BspNode currentRoot = root;
            List<BspNode> leaves = new List<BspNode>();

            for(int y = 0; y < rows; y++)
            {
                for(int x = 0; x < cols; x++)
                {
                    IntRect leafRect = new IntRect
                        (x * leafWidth,
                        y * leafHeight, 
                        leafWidth, 
                        leafHeight
                        );
                    leaves.Add(new BspNode(leafRect));
                }
            }

            if (leaves.Count == 0)
                return root;

            currentRoot = leaves[0];

            for(int i = 1; i < leaves.Count; i++)
            {
                IntRect merged = MergeRects(currentRoot.Bounds, leaves[i].Bounds);
                BspNode parent = new BspNode(merged);

                parent.Left = currentRoot;
                parent.Right = leaves[i];

                currentRoot.Parent = parent;
                leaves[i].Parent = parent;

                currentRoot = parent;

            }
            return currentRoot;

        }

        private IntRect MergeRects(IntRect bounds1, IntRect bounds2)
        {
            int xMin = Math.Min(bounds1.xMin, bounds2.xMin);
            int yMin = Math.Min(bounds1.yMin, bounds2.yMin);
            int xMax = Math.Max(bounds1.xMax, bounds2.xMax);
            int yMax = Math.Max(bounds1.yMax, bounds2.yMax);

            return new IntRect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}