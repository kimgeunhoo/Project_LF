using System.Collections.Generic;
using UnityEngine;

namespace ModularBSP.Core
{
    public class BspNode
    {
        public IntRect Bounds;
        public BspNode Left;
        public BspNode Right;
        public BspNode Parent;

        public IntRect? RoomBounds;

        // leaf 저장방식
        public List<BspNode> FixedLeaves = new List<BspNode>();
        public bool IsLeaf => Left == null && Right == null;

        public BspNode(IntRect bounds)
        {
            Bounds = bounds;
        }

    }
}