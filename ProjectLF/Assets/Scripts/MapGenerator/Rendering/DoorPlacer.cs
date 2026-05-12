using System.Collections.Generic;
using MapGenerator.Core;
using ModularBSP.Config;
using ModularBSP.Core;
using ModularBSP.Generation;
using UnityEngine;




namespace ModularBSP.Rendering
{
    public class DoorPlacer
    {
        private readonly DungeonConfig config;
        private readonly DungeonContext context;
        private readonly Transform doorParent;

        public DoorPlacer(DungeonConfig config, DungeonContext context, Transform doorParent)
        {
            this.config = config;
            this.context = context;
            this.doorParent = doorParent;
        }

        public void PlaceDoor()
        {

            //Debug.Log($"[DoorPlacer] Rooms={context.Rooms.Count}, ConnectedRoomCount={context.RoomConnectedDirs.Count}");

            foreach (var room in context.Rooms)
            {
                string key = GetRoomKey(room);

                if (!context.RoomConnectedDirs.TryGetValue(key, out HashSet<DoorDir> connectedDirs))
                {
                    Debug.LogWarning($"[DoorPlacer] No connected dirs for room key={key}");
                    continue;
                }

                //Debug.Log($"[DoorPlacer] Room={key}, dirs={string.Join(",", connectedDirs)}");

                foreach (DoorDir dir in connectedDirs)
                {
                    DoorSet(room, dir);
                }

            }
        }

        private void DoorSet(IntRect room, DoorDir dir)
        {
            GameObject prefab = GetDoorPF(dir);
            if (prefab == null)
                return;

            Vector3 worldPos = GetDoorWorldPos(room, dir);

            GameObject doorObj = Object.Instantiate(prefab, worldPos, Quaternion.identity, doorParent);

            RoomRuntimeData runtimeData = FindRoomRuntimeData(room);

            if (runtimeData == null)
            {
                doorObj.SetActive(false);
                return;
            }

            DoorController doorController = doorObj.GetComponent<DoorController>();
            if (doorController == null)
            {
                doorController = doorObj.AddComponent<DoorController>();
            }
            doorController.SetRoomId(runtimeData.RoomId);
            runtimeData.Doors.Add(doorController);

            doorObj.SetActive(false);
        }
        private GameObject GetDoorPF(DoorDir dir)
        {
            switch (dir)
            {
                case DoorDir.Up:
                case DoorDir.Down:
                    return config.doorHorizontalPrefab;
                case DoorDir.Left:
                case DoorDir.Right:
                    return config.doorVerticalPrefab;
            }

            return null;
        }

        private Vector3 GetDoorWorldPos(IntRect room, DoorDir dir)
        {
            Vector2 roomdoorCell = GetRoomDoorCell(room, dir);
            Vector2 outsideDoorCell = GetOutsideDoorCell(room, dir);

            Vector3 roomPos = GridToWorldCell(roomdoorCell);
            Vector3 outsidePos = GridToWorldCell(outsideDoorCell);

            return (roomPos + outsidePos) * 0.5f + GetDoorOffset(dir);
        }

        private Vector3 GridToWorldCell(Vector2 Cell)
        {
            return new Vector3(Cell.x * config.cellSize + config.cellSize * 0.5f,
                Cell.y * config.cellSize + config.cellSize * 0.5f, 0f);
        }

        private Vector2Int GetOutsideDoorCell(IntRect room, DoorDir dir)
        {
            int left = room.xMin;
            int right = room.xMax - 1;
            int bottom = room.yMin;
            int top = room.yMax - 1;

            int centerX = room.xMin + (room.width - 1) / 2;
            int centerY = room.yMin + (room.height - 1) / 2;

            switch (dir)
            {
                case DoorDir.Up:
                    return new Vector2Int(centerX, top + 1);
                case DoorDir.Right:
                    return new Vector2Int(right + 1, centerY);
                case DoorDir.Down:
                    return new Vector2Int(centerX, bottom - 1);
                case DoorDir.Left:
                    return new Vector2Int(left - 1, centerY);
            }

            return new Vector2Int(centerX, centerY);
        }

        private Vector2Int GetRoomDoorCell(IntRect room, DoorDir dir)
        {
            int left = room.xMin;
            int right = room.xMax - 1;
            int bottom = room.yMin;
            int top = room.yMax - 1;

            int centerX = room.xMin + (room.width - 1)/ 2;
            int centerY = room.yMin + (room.height - 1)/ 2;

            switch (dir)
            {
                case DoorDir.Up:
                    return new Vector2Int(centerX, top);
                case DoorDir.Down:
                    return new Vector2Int(centerX, bottom);
                case DoorDir.Left:
                    return new Vector2Int(left, centerY);
                case DoorDir.Right:
                    return new Vector2Int(right, centerY);
            }

            return new Vector2Int(centerX, centerY);
        }


        private Vector3 GetDoorOffset(DoorDir dir)
        {
            float upOffset = 2.5f;
            float downoffset = 0.5f;
            float leftoffset = 0.5f;
            float rightofffset = -0.5f;

            switch (dir)
            {
                case DoorDir.Up:
                    return new Vector3(0f, -upOffset, 0f);
                case DoorDir.Down:
                    return new Vector3(0f, downoffset, 0f);
                case DoorDir.Left:
                    return new Vector3(leftoffset, -1f, 0f);
                case DoorDir.Right:
                    return new Vector3(rightofffset, -1f, 0f);
            }

            return Vector3.zero;
        }


        private string GetRoomKey(IntRect room)
        {
            return $"{room.x}_{room.y}_{room.width}_{room.height}";
        }

        private RoomRuntimeData FindRoomRuntimeData(IntRect room)
        {
            if (context.RoomStates == null)
                return null;

            foreach (RoomRuntimeData runtimeRoom in context.RoomStates)
            {
                if (runtimeRoom.RoomRect.x == room.x &&
                    runtimeRoom.RoomRect.y == room.y &&
                    runtimeRoom.RoomRect.width == room.width &&
                    runtimeRoom.RoomRect.height == room.height)
                {
                    return runtimeRoom;
                }
            }

            return null;
        }
    }

}
