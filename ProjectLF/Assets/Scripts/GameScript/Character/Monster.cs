using BSPDungeonGenrator.Generation;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private int roomId;
    private DungeonManager dungeonManager;
    private RoomBattleSystem roomBattleSystem;

    [Header("Monster Data")]
    [SerializeField]
    public Character m_Monster;

    public int Hp;

    private bool isDead = false;

    private void Awake()
    {
        Hp = m_Monster.Hp;
    }

    public void Init(int _roomId, DungeonManager _dungeonManager)
    {
        //Debug.Log($"[Monster] Init called / roomId={roomId}");
        this.roomId = _roomId;
        this.dungeonManager = _dungeonManager;
    }
    
    public void Die()
    {
        if (isDead) 
            return;
        isDead = true;

        dungeonManager.OnMonsterDead(roomId);
        Destroy(gameObject);
    }

}
