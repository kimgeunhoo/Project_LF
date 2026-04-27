using ModularBSP.Core;
using UnityEngine;

namespace MapGenerator.Core
{
    [System.Serializable]
    public class RoomRuntimeData
    {
        public int RoomId;
        public IntRect RoomRect;
        public RoomType RoomType;
        public Vector2Int CenterCell;
        public Vector3 CenterWorld;

        public RoomRuntimeData(int roomId, IntRect roomRect, RoomType roomType, Vector2Int centerCell, Vector3 centerWorld)
        {
            RoomId = roomId;
            RoomRect = roomRect;
            RoomType = roomType;
            CenterCell = centerCell;
            CenterWorld = centerWorld;
        }

    }
}
