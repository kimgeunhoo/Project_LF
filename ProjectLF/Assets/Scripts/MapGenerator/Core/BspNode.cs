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

        public bool IsLeaf => Left == null && Right == null;

        public BspNode(IntRect bounds)
        {
            Bounds = bounds;
        }

    }
}