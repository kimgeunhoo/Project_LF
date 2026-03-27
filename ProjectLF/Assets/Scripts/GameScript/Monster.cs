using UnityEngine;

public class Monster : MonoBehaviour
{
    public int RoomId { get; private set; }

    private RoomBattleSystem roomBattleSystem;

    public void SetRoom(int roomId, RoomBattleSystem battleSys)
    {
        RoomId = roomId;
        roomBattleSystem = battleSys;
    }

    public void Die()
    {
        if (roomBattleSystem != null)
            roomBattleSystem.MonsterDeadMethod(RoomId);

        Destroy(gameObject);
    }

}
