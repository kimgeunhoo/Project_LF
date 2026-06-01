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
                // 몬스터 방일떄만 실행되야함
                MonsterSpawnPoint[] spawnPoints =
                roomObj.GetComponentsInChildren<MonsterSpawnPoint>(true);

                roomState.monsterSpawnPoints.AddRange(spawnPoints);

                //Debug.Log($"[Room {roomState.RoomId}] SpawnPoint Count = {spawnPoints.Length}");
                //Debug.Log($"[PrefabPlacer] Room {roomState.RoomId} SpawnPoint 등록 = {roomState.monsterSpawnPoints.Count}");


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
            int step = config.corridorWidthInCells;
            HashSet<Vector2Int> placed = new HashSet<Vector2Int>();

            //Debug.Log($"[PlaceCorridors] CorridorCells Count = {context.CorridorCells.Count}");


            foreach (var cell in context.CorridorCells)
            {
                Vector2Int moduleCell = new Vector2Int(
                    Mathf.FloorToInt(cell.x / (float)step) * step,
                    Mathf.FloorToInt(cell.y / (float)step) * step
                    );

                if (placed.Contains(moduleCell))
                    continue;

                placed.Add(moduleCell);

                Vector2Int centerCell = moduleCell + new Vector2Int(step / 2, step / 2);

                GameObject roadPrefab = GetCorridorPrefab(centerCell.x, centerCell.y);

                if (roadPrefab == null)
                    continue;

                Vector3 worldPos = GridToWorldForModule(moduleCell, step , step);

                Object.Instantiate(roadPrefab, worldPos, Quaternion.identity, roadParent);

            }
        }

        private Vector3 GridToWorldForModule(Vector2Int moduleCell, int width, int height)
        {
            float worldX = moduleCell.x * config.cellSize + width * config.cellSize * 0.5f;
            float worldY = moduleCell.y * config.cellSize + height * config.cellSize * 0.5f;

            return new Vector3(worldX, worldY, 0f);
        }

        private GameObject PickRandomPrefab(GameObject[] roomPrefab)
        {
            if (roomPrefab == null || roomPrefab.Length == 0)
                return null;

            return roomPrefab[Random.Range(0, roomPrefab.Length)];
        }

        private GameObject GetCorridorPrefab(int x, int y)
        {
            int step = config.corridorWidthInCells;

            bool up = IsConnectedForPath(context, x, y + step);
            bool right = IsConnectedForPath(context, x + step, y);
            bool down = IsConnectedForPath(context, x, y - step);
            bool left = IsConnectedForPath(context, x - step, y);

            //bool isRoom = IsConnectedForPath(context, x, y);

            int mask = 0;

            if (up)
                mask |= 1;
            if (right)
                mask |= 2;
            if (down)
                mask |= 4;
            if (left)
                mask |= 8;

            //Debug.Log($"[RoadPF] cell=({x},{y}) up={up}, right={right}, down={down}, left={left}, mask={mask}");

            switch (mask)
            {
                //case 0:
                //case 1:
                //case 2:
                //case 4:
                //case 8: return config.pathPrefabs.Empty;
                case 0: return config.pathPrefabs.Empty;
                case 1: return config.pathPrefabs.UpEnd;
                case 2: return config.pathPrefabs.RightEnd;
                case 4: return config.pathPrefabs.DownEnd;
                case 8: return config.pathPrefabs.LeftEnd;

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
        private bool IsConnectedForPath(DungeonContext context, int x, int y)
        {
            if (!context.IsInside(x, y))
                return false;

            Vector2Int pos = new Vector2Int(x, y);
            CellType cell = context.Grid[x, y];

            if (context.CorridorCells.Contains(pos))
                return true;
            if (context.RoomEnteranceCells.Contains(pos))
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
