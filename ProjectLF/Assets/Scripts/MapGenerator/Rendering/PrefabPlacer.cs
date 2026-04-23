using UnityEngine;
using ModularBSP.Config;
using ModularBSP.Core;
using System.Collections.Generic;
using ModularBSP.Generation;
using ModularBSP.Marker;

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
                GameObject roomObj = Object.Instantiate
                    (config.roomPrefab, worldPos, Quaternion.identity, config.roomParent);

                RoomInstance roomInstance = roomObj.GetComponent<RoomInstance>();
                if(roomInstance != null)
                {
                    HashSet<DoorDir> connectedDirs = GetConnectedDirections(room);
                    roomInstance.SetupBlockedDoors(connectedDirs);
                }

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
            bool isRoom = IsConnectedForPath(x, y);

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
                //case 1: return config.PathPrefabs.UpEnd;
                //case 2: return config.PathPrefabs.RightEnd;
                //case 4: return config.PathPrefabs.DownEnd;
                //case 8: return config.PathPrefabs.LeftEnd;

                case 1:
                case 4:
                case 5: return config.PathPrefabs.Vertical;
                case 2:
                case 8:
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

        private HashSet<DoorDir> GetConnectedDirections(IntRect room)
        {
            HashSet<DoorDir> result = new HashSet<DoorDir>();

            Vector2Int upOutside = GetOutsideDoorCell(room, DoorDir.Up);
            Vector2Int rightOutside = GetOutsideDoorCell(room, DoorDir.Right);
            Vector2Int downOutside = GetOutsideDoorCell(room, DoorDir.Down);
            Vector2Int leftOutside = GetOutsideDoorCell(room, DoorDir.Left);

            if (IsCorridorCell(upOutside))
                result.Add(DoorDir.Up);
            if (IsCorridorCell(rightOutside))
                result.Add(DoorDir.Right);
            if (IsCorridorCell(downOutside))
                result.Add(DoorDir.Down);
            if (IsCorridorCell(leftOutside))
                result.Add(DoorDir.Left);

            return result;  
        }

        private bool IsCorridorCell(Vector2Int pos)
        {
            if(!context.IsInside(pos.x, pos.y))
                return false;

            return context.Grid[pos.x, pos.y] == CellType.Corridor;
        }

        private Vector2Int GetOutsideDoorCell(IntRect room, DoorDir dir)
        {
            int left = room.xMin;
            int right = room.xMax - 1;
            int top = room.yMax - 1;
            int bottom = room.yMin;

            int centerX = room.xMin + room.width / 2;
            int centerY = room.yMin + room.height / 2; 

            switch (dir)
            {
                case DoorDir.Up: return new Vector2Int(centerX, top + 1);
                case DoorDir.Right: return new Vector2Int(right + 1, centerY);
                case DoorDir.Down: return new Vector2Int(centerX, bottom - 1);
                case DoorDir.Left: return new Vector2Int(left - 1, centerY);
            }

            return new Vector2Int(centerX, centerY);

        }
    }
}
