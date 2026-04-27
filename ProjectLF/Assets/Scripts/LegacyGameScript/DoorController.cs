using UnityEngine;

namespace LegacyGameScrpit
{

    public class DoorController : MonoBehaviour
    {
        private int roomId = -1;

        public int RoomId { get { return roomId; } }

        public void SetRoomId(int id)
        {
            roomId = id;
        }

        public void OpenDoor()
        {
            gameObject.SetActive(false);
        }

        public void CloseDoor()
        {
            gameObject.SetActive(true);
        }

    }

}