using UnityEngine;
using ModularBSP.Config;
using ModularBSP.Core;

namespace ModularBSP.Rendering
{
    public class PrefabPlacer
    {
        private readonly DungeonConfig config;
        private readonly DungeonContext context;

        public PrefabPlacer(DungeonConfig config, DungeonContext context)
        {
            this.config = config;
            this.context = context;
        }

        public void PlaceAll()
        {
            PlaceRooms();
            PlaceCorridors();
        }

        private void PlaceRooms()
        {
            foreach (var room in context.Rooms)
            {
                Vector3 worldPos = GridToWorldForRoom(room);
                Object.Instantiate
                    (config.roomPrefab, worldPos, Quaternion.identity, config.roomParent);
            }
        }

        private void PlaceCorridors()
        {
            foreach (var cell in context.CorridorCells)
            {
                GameObject roadPrefab = GetCorridorPrefab(cell.x, cell.y);

                if (roadPrefab == null)
                {
                    Debug.LogWarning($"No corridor prefab matched at {cell}");
                    continue;
                }

                Vector3 worldPos = GridToWorldCell(cell.x, cell.y);
                Object.Instantiate
                    (roadPrefab, worldPos, Quaternion.identity, config.roadParent);
            }
        }

        private GameObject GetCorridorPrefab(int x, int y)
        {
            bool up = IsConnectedForPath(x, y + 1);
            bool right = IsConnectedForPath(x + 1, y);
            bool down = IsConnectedForPath(x, y - 1);
            bool left = IsConnectedForPath(x - 1, y);

            int mask = 0;
            if (up)
                mask |= 1;
            if (right)
                mask |= 2;
            if (down)
                mask |= 4;
            if (left)
                mask |= 8;

            switch (mask)
            {
                case 0: return config.PathPrefabs.Empty; 
                case 1: return config.PathPrefabs.UpEnd;
                case 2: return config.PathPrefabs.RightEnd;
                case 4: return config.PathPrefabs.DownEnd;
                case 8: return config.PathPrefabs.LeftEnd;

                case 5: return config.PathPrefabs.Vertical;
                case 10: return config.PathPrefabs.Horizontal;

                case 3: return config.PathPrefabs.UpRightCorner;
                case 6: return config.PathPrefabs.RightDownCorner;
                case 9: return config.PathPrefabs.LeftUpCorner;
                case 12: return config.PathPrefabs.DownLeftCorner;

                case 7: return config.PathPrefabs.UpRightDownTJunction;
                case 11: return config.PathPrefabs.LeftUpRightTJunction;
                case 13: return config.PathPrefabs.DownLeftUpTJunction;
                case 14: return config.PathPrefabs.RightDownLeftTJunction;

                case 15: return config.PathPrefabs.cross;
            }
            return null;
        }
        private bool IsConnectedForPath(int x, int y)
        {
            if (!context.IsInside(x, y))
                return false;

            Vector2Int pos = new Vector2Int(x, y);
            CellType cell = context.Grid[x, y];

            if(cell == CellType.Corridor)
                return true;

            if(cell == CellType.Room && context.RoomEnteranceCells.Contains(pos))
                return true;

            return false;
        }

        private Vector3 GridToWorldCell(int x, int y)
        {
            float worldX = x * config.cellSize + config.cellSize * 0.5f;
            float worldY = y * config.cellSize + config.cellSize * 0.5f;
            return new Vector3(worldX, worldY, 0f);
        }

        private Vector3 GridToWorldForRoom(IntRect room)
        {
            float roomWorldWidth = config.cellSize * room.width;
            float roomWorldHeight = config.cellSize * room.height;

            float worldX = room.x * config.cellSize + roomWorldWidth * 0.5f;
            float worldY = room.y * config.cellSize + roomWorldHeight * 0.5f;
            return new Vector3(worldX, worldY, 0f);
        }

    }
}
