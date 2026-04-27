using BSPDungeonGenrator.Core;
using LegacyGameScrpit;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDungeonGenrator.Generation
{
    public class DoorSpawner : MonoBehaviour
    {
        [SerializeField] 
        private GameObject doorPrefab;
        [SerializeField]
        private Tilemap floorTilemap;
        [SerializeField]
        private Transform doorParent;

        public void Run(OldDungeonContext _ctx)
        {
            //Debug.Log($"[DoorSpawner] DoorPos Count = {_ctx.DoorPositions.Count}");

            if (doorPrefab == null)
            {
                Debug.LogError("[DoorSpawner] doorPrefab is NULL");
                return;
            }

            if (floorTilemap == null)
            {
                Debug.LogError("[DoorSpawner] floorTilemap is NULL");
                return;
            }

            if (_ctx.RoomStates == null)
            {
                Debug.LogError("[DoorSpawner] ctx.RoomStates is NULL");
                return;
            }

            ClearOldDoors();

            foreach (var pos in _ctx.DoorPositions)
            {
                SpawnDoor(_ctx, pos);
            }
        }

        private void ClearOldDoors()
        {
            if (doorParent == null)
            {
                Debug.LogError("[DoorSpawner] doorParent is NULL");
                return;
            }

            for (int i = doorParent.childCount - 1; i >= 0; i--)
            {
                Destroy(doorParent.GetChild(i).gameObject);
            }
        }

        private void SpawnDoor(OldDungeonContext _ctx, Vector2Int mapPos)
        {
            //Debug.Log($"[DoorSpawner] Spawning door at {mapPos}");

            Vector3Int cellPos = new Vector3Int(
                mapPos.x - _ctx.MapSize.x / 2,
                mapPos.y - _ctx.MapSize.y / 2,
                0
                );
            Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0f);

            GameObject doorObj = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorParent);

            DoorController doorController = doorObj.GetComponent<DoorController>();
            if (doorController == null)
                return;

            int roomId = FindRoomIdFromDoorPos(_ctx, mapPos);
            if (roomId >= 0 && roomId < _ctx.RoomStates.Count)
            {
                doorController.SetRoomId(roomId);
                _ctx.RoomStates[roomId].Doors.Add(doorController);
            }

        }


        private int FindRoomIdFromDoorPos(OldDungeonContext _ctx, Vector2Int doorPos)
        {
            for (int i = 0; i < _ctx.RoomStates.Count; i++)
            {
                RectInt rect = _ctx.RoomStates[i].RoomInfo.Rect;

                bool adjacent =
                    rect.Contains(doorPos + Vector2Int.left) ||
                    rect.Contains(doorPos + Vector2Int.right) ||
                    rect.Contains(doorPos + Vector2Int.up) ||
                    rect.Contains(doorPos + Vector2Int.down);

                if (adjacent)
                    return i;
            }
 
            return -1;
        }

    }

}
