using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ModularBSP.Core
{
    public class GridCoord : MonoBehaviour
    {
        [Serializable]
        public struct GridPos
        {
            public int x;
            public int y;
            public GridPos(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static GridPos operator +(GridPos a, GridPos b) 
                => new GridPos(a.x + b.x, a.y + b.y);
            public static GridPos operator -(GridPos a, GridPos b) 
                => new GridPos(a.x - b.x, a.y - b.y);

            public Vector2Int ToVector2Int() => new Vector2Int(x, y);
            public override string ToString() => $"({x}, {y})";

        }
    }
}
