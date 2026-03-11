using UnityEngine;

namespace BSPDuengeonGenrator.Core
{
    [System.Serializable]
    public class LineSegment
    {
        public Vector2 from;
        public Vector2 to;

        public LineSegment(Vector2 from, Vector2 to)
        {
            this.from = from;
            this.to = to;
        }
    }

}