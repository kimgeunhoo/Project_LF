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
        private readonly Transform roomParent;
        private readonly Transform roadParent;
        private readonly Transform emptyParent;

        public PrefabPlacer(
            DungeonConfig config,
            DungeonContext context,
            Transform roomParent,
            Transform roadParent,
            Transform emptyParent
            )
        {
            this.config = config;
            this.context = context;
            this.roomParent = roomParent;
            this.roadParent = roadParent;
            this.emptyParent = emptyParent;
        }

        public void PlaceAll()
        {
            PlaceEmptyCells();
            PlaceRooms();
            PlaceCorridors();
        }

        private void PlaceEmptyCells()
        {
            if (config.pathPrefabs.Empty == null)
                return;

            for (int x = 0; x < context.MapSizeInCells.x; x++)
            {
                for (int y = 0; y < context.MapSizeInCells.y; y++)
                {
                    if (context.Grid[x, y] != CellType.Empty)
                        continue;
                    Vector3 worldPos = GridToWorldCell(x, y);

                    Object.Instantiate(config.pathPrefabs.Empty, worldPos, Quaternion.identity, emptyParent);
                }
            }
        }

        private void PlaceRooms()
        {
            foreach (var roomState in context.RoomStates)
            {
                GameObject prefab = GetRoomPrefab(roomState.RoomType);
                if (prefab == null)
                {
                    Debug.LogWarning($"Room prefab missing");
                    continue;
                }

                Vector3 worldPos = GridToWorldForRoom(roomState.RoomRect);

                GameObject roomObj = Object.Instantiate(
                    prefab, 
                    worldPos, 
                    Quaternion.identity, 
                    roomParent);

                RoomInstance roomInstance = roomObj.GetComponent<RoomInstance>();
                if(roomInstance != null)
                {
                    string key = GetRoomKey(roomState.RoomRect);

                    if(!context.RoomConnectedDirs.TryGetValue(key, out HashSet<DoorDir> connectedDirs))
                    {
                        connectedDirs = new HashSet<DoorDir>();
                    }

                    roomInstance.SetupBlockedDoors(connectedDirs);
                }
            }

        }

        private string GetRoomKey(IntRect room)
        {
            return $"{room.x}_{room.y}_{room.width}_{room.height}";
        }

        private GameObject GetRoomPrefab(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Start:
                    return config.startRoomPrefab;
                case RoomType.Shop:
                    return config.shopRoomPrefab;
                case RoomType.Stairs:
                    return config.stairsRoomPrefab;
                case RoomType.Encounter:
                    return PickRandomPrefab(config.encounterRoomPrefab);
                case RoomType.Enemy:
                    return PickRandomPrefab(config.enemyRoomPrefab);
            }

            return null;
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
                    (roadPrefab, worldPos, Quaternion.identity, roadParent);
            }
        }

        private GameObject PickRandomPrefab(GameObject[] roomPrefab)
        {
            if (roomPrefab == null || roomPrefab.Length == 0)
                return null;

            return roomPrefab[Random.Range(0, roomPrefab.Length)];
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
                case 0: return config.pathPrefabs.Empty;
                case 1: return config.pathPrefabs.DownEnd;
                case 2: return config.pathPrefabs.LeftEnd;
                case 4: return config.pathPrefabs.UpEnd;
                case 8: return config.pathPrefabs.RightEnd;

                //case 1:
                //case 4:
                case 5: return config.pathPrefabs.Vertical;
                //case 2:
                //case 8:
                case 10: return config.pathPrefabs.Horizontal;

                case 3: return config.pathPrefabs.UpRightCorner;
                case 6: return config.pathPrefabs.RightDownCorner;
                case 9: return config.pathPrefabs.LeftUpCorner;
                case 12: return config.pathPrefabs.DownLeftCorner;

                case 7: return config.pathPrefabs.UpRightDownTJunction;
                case 11: return config.pathPrefabs.LeftUpRightTJunction;
                case 13: return config.pathPrefabs.DownLeftUpTJunction;
                case 14: return config.pathPrefabs.RightDownLeftTJunction;

                case 15: return config.pathPrefabs.cross;
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
