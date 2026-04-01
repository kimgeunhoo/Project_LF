using BSPDungeonGenrator.Generation;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private int roomId;
    private DungeonManager dungeonManager;
    private RoomBattleSystem roomBattleSystem;

    public void Init(int _roomId, DungeonManager _dungeonManager)
    {
        this.roomId = _roomId;
        this.dungeonManager = _dungeonManager;
    }
    
    public void Die()
    {
        dungeonManager.OnMonsterDead(roomId);
        Destroy(gameObject);
    }

}
