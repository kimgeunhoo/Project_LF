using UnityEngine;


namespace ModularBSP.Config
{
    [CreateAssetMenu(fileName = "DungeonConfig", menuName = "Dungeon/Modular BSP Config")]
    public class DungeonConfig : ScriptableObject
    {
        // 프리팹 공간사이즈를 6으로 만들었는데, 크기를 바꾼다면 기초값도 그에 따라 바뀔거다.
        [Header("Grid")]
        public int cellSize = 6;
        public int mapWidthInCells = 48;
        public int mapHeightInCells = 48;

        [Header("Room")]
        public Vector2Int roomSizeInCells = new Vector2Int(3, 3);   // 18x18
        public Vector2Int minLeafSize = new Vector2Int(4, 4);// def 6x6
        public Vector2Int maxLeafSize = new Vector2Int(6, 6);// def 12x12

        [Header("BSP")]
        public int maxDepth = 5; // def 4
        public int splitPadding = 1;

        [Header("Corridor")]
        public int corridorWidthInCells = 1; // 6x6
        //public bool useRandomExtraWidth = false;

        [Header("Prefabs")]
        public GameObject roomPrefab;
        public PathPrefabSet PathPrefabs;

        public Transform roomParent;
        public Transform roadParent;
    }

}
