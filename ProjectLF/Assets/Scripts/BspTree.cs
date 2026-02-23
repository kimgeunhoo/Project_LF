using UnityEngine;
using UnityEngine.Tilemaps;

public class BspTree : MonoBehaviour
{
    public class TreeNode
    {
        public TreeNode leftTree;
        public TreeNode rightTree;
        public TreeNode parentTree;
        // RectInt 
        // 정수 좌표(x, y)와 크기(width, height)로 정의되는 2D 직사각형 구조체
        public RectInt treeSize;
        public RectInt dungeonSize;


        private TileBase[] RoomTiles;

        // 맵 데이터 생성, 초기화
        //private int[,] mapData = new int[mapSize.x, mapSize.y];
        // 0 = 빈공간
        // 1 = 바닥
        // 2 = 벽

        public TreeNode(int _x, int _y, int _width, int _height)
        {
            treeSize.x = _x;
            treeSize.y = _y;
            treeSize.width = _width;
            treeSize.height = _height;
        }
    }
}
