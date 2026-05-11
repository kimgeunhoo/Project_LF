using UnityEngine;

public class DoorController : MonoBehaviour
{
    private int roomId = - 1;

    private bool isClosed;

    public int RoomId => roomId;
    public bool IsClosed => isClosed;


    public void SetRoomId(int id)
    {
        roomId = id;
    }

    public void Close()
    {
        isClosed = true;
        gameObject.SetActive(true);
    }

    public void Open()
    {
        isClosed = false;
        gameObject.SetActive(false);
    }
}
