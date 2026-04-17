using System.Collections;
using BSPDungeonGenrator.Generation;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private int roomId;
    private DungeonManager dungeonManager;
    private RoomBattleSystem roomBattleSystem;
    private Animator animator;

    [Header("Monster Data")]
    [SerializeField]
    public Character m_Monster;

    public int Hp;

    private bool isDead = false;

    private void Awake()
    {
        Hp = m_Monster.Hp;
        animator = GetComponentInChildren<Animator>();
    }

    public void Init(int _roomId, DungeonManager _dungeonManager)
    {
        //Debug.Log($"[Monster] Init called / roomId={roomId}");
        this.roomId = _roomId;
        this.dungeonManager = _dungeonManager;
    }
    
    private void Die()
    {
        if (isDead) 
            return;
        isDead = true;
        StartCoroutine(DiyingAnimation());
        dungeonManager.OnMonsterDead(roomId);
        Destroy(gameObject);
    }

    public IEnumerator DiyingAnimation()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.5f); // 애니메이션 길이에 맞게 조절
        Die();
    }

    public IEnumerator DamageAnimation()
    {
        animator.SetTrigger("Damaged");
        yield return new WaitForSeconds(0.3f); // 애니메이션 길이에 맞게 조절
    }

}
