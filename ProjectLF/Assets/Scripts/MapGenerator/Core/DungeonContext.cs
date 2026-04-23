using System.Collections.Generic;
using ModularBSP.Generation;
using UnityEngine;

namespace ModularBSP.Core
{
    public enum CellType
    {
        Empty,
        Room,
        Corridor
    }

    public class DungeonContext
    {
        public int CellSize;
        public Vector2Int MapSizeInCells;
        public CellType[,] Grid;

        public BspNode Root;
        public List<IntRect> Rooms = new List<IntRect>();
        public HashSet<Vector2Int> CorridorCells = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> RoomEnteranceCells = new HashSet<Vector2Int>();

        public Dictionary<string, HashSet<DoorDir>> RoomConnectedDirs = new Dictionary<string, HashSet<DoorDir>>();

        public List<RoomRuntimeData> RoomStates = new List<RoomRuntimeData>();

        public DungeonContext(int cellSize, Vector2Int mapSizeInCells)
        {
            CellSize = cellSize;
            MapSizeInCells = mapSizeInCells;
            Grid = new CellType[mapSizeInCells.x, mapSizeInCells.y];
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && x < MapSizeInCells.x && y >= 0 && y < MapSizeInCells.y;
        }

    }
}
