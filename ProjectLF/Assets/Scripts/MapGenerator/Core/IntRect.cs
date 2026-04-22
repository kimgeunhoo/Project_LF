using UnityEngine;


namespace ModularBSP.Core
{
    [System.Serializable]
    public struct IntRect
    {
        public int x;
        public int y; 
        public int width;
        public int height;

        public int xMin => x;
        public int xMax => x + width;
        public int yMin => y;
        public int yMax => y + height;

        public Vector2Int Center => new Vector2Int(x + width / 2, y + height / 2);

        public IntRect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public override string ToString() 
            => $"IntRect(x: {x}, y: {y}, width: {width}, height: {height})";
    }
}