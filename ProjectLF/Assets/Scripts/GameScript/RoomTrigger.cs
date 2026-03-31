using BSPDungeonGenrator.Generation;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField]
    private int RoomId;
    [SerializeField]
    private DungeonManager dungeonManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        dungeonManager.EnterEnemyRoom(RoomId);
    }
}
