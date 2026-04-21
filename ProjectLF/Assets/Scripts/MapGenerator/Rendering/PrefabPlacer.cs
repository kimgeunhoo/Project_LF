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
                    (config.roomPrefab, worldPos, Quaternion.identity, config.roadParent);
            }
        }

        private void PlaceCorridors()
        {
            foreach (var cell in context.CorridorCells)
            {
                Vector3 worldPos = GridToWorldCell(cell.x, cell.y);
                Object.Instantiate
                    (config.roadPrefab, worldPos, Quaternion.identity, config.roadParent);
            }
        }


        private Vector3 GridToWorldCell(int x, int y)
        {
            float worldX = x * config.cellSize;
            float worldY = y * config.cellSize;
            return new Vector3(worldX, worldY, 0f);
        }

        private Vector3 GridToWorldForRoom(IntRect room)
        {
            float worldX = room.x * config.cellSize;
            float worldY = room.y * config.cellSize;
            return new Vector3(worldX, worldY, 0f);
        }

    }
}
