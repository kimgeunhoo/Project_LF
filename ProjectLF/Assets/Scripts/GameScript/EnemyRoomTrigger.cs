using BSPDungeonGenrator.Generation;
using UnityEngine;

public class EnemyRoomTrigger : MonoBehaviour
{
    private int roomId;
    private DungeonManager dungeonManager;

    public void Init(int _roomId, DungeonManager _manager)
    {
        this.roomId = _roomId;
        this.dungeonManager = _manager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (dungeonManager == null)
        {
            Debug.LogWarning($"[Trigger] DungeonManager is null / roomId={roomId}");
            return;
        }
        //Debug.Log($"[Trigger] Player entered room : {roomId}");
        dungeonManager.EnterEnemyRoom(roomId);
    }

}
