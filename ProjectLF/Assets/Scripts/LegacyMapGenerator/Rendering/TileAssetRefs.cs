using UnityEngine;
using UnityEngine.Tilemaps;


namespace BSPDungeonGenrator.Rendering
{
    public sealed class TileAssetRefs
    {
        public TileBase FloorTile;
        public TileBase WallTile;
        public TileBase DoorTile;
        public TileBase OpenDoorTile;
        public TileBase PathTile;

        public TileBase[] PathTiles;
        public TileBase[] RoomTiles;
        public TileBase[] BackgroundTiles;
    }
}

