using UnityEngine;


namespace ModularBSP.Marker
{

    public enum DoorDirection
    {
        North,
        South,
        East,
        West
    }
    public class RoomDoorMarker : MonoBehaviour
    {
        public DoorDirection direction;
    }
}
