using GameScript.Manager;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    private MonsterData data;

    private int currentHp;
    private bool isDead;
    private GameObject monster;
    private int roomId;
    private DungeonManager dungeonManager;


    private Animator animator;

    public MonsterData Data => data;

    public int CurrentHp => currentHp;

    private void Awake()
    {
        if(data == null)
        {
            Debug.LogError($"[{name}] MonsterData가 없습니다.");
            return;
        }
        animator = GetComponentInChildren<Animator>();
        currentHp = data.MaxHp;
    }

    public void Init(int _roomId, DungeonManager _dungeonManager)
    {
        roomId = _roomId;
        dungeonManager = _dungeonManager;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        int finalDamage = Mathf.Max(1, damage - data.Def);

        currentHp -= finalDamage;
        animator.SetTrigger("Damaged");
        Debug.Log($"[Monster] {name} Damage={finalDamage} / HP={currentHp}");

        if(currentHp <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        StartCoroutine(DyingAnimation());

        if (dungeonManager != null)
        {
            dungeonManager.OnMonsterDead(roomId);
        }

        
    }

    IEnumerator DyingAnimation()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

}
