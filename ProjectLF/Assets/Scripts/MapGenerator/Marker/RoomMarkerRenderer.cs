using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using UnityEngine;

namespace BSPDungeonGenrator.marker
{
    public class RoomMarkerRenderer : MonoBehaviour
    {
        [SerializeField] 
        private Transform markerHolder;
        [SerializeField] 
        private GameObject startMarkerPrefab;
        [SerializeField] 
        private GameObject stairMarkerPrefab;
        [SerializeField] 
        private GameObject shopMarkerPrefab;
        [SerializeField] 
        private GameObject encounterMarkerPrefab;
        [SerializeField] 
        private GameObject monsterMarkerPrefab;

        //[SerializeField] 
        //private EnemyRoomBinder enemyRoomBinder;

        public void Run(DungeonContext _ctx)
        {
            ClearMarkers();
            for (int i = 0; i < _ctx.Rooms.Count; i++)
            {
                RoomInfo room = _ctx.Rooms[i];
                GameObject prefab = GetPrefab(room.Type);

                if (prefab == null)
                    continue;

                Vector2Int center = room.Center;
                Vector3Int cellPos = new Vector3Int(
                    center.x - _ctx.MapSize.x / 2,
                    center.y - _ctx.MapSize.y / 2,
                    0
                    );

                Vector3 worldPos = _ctx.FloorTilemap.CellToWorld(cellPos);

                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
            }

        }

        private GameObject GetPrefab(RoomType type)
        {
            return type switch
            {
                RoomType.Start => startMarkerPrefab,
                RoomType.Stairs => stairMarkerPrefab,
                RoomType.Shop => shopMarkerPrefab,
                RoomType.Encounter => encounterMarkerPrefab,
                RoomType.Monster => monsterMarkerPrefab,
                _ => null
            };
        }

        private void ClearMarkers()
        {
            if (markerHolder == null)
                return;

            for (int i = markerHolder.childCount - 1; i >= 0; i--)
            {
                Destroy(markerHolder.GetChild(i).gameObject);
            }
        }
    }
}
