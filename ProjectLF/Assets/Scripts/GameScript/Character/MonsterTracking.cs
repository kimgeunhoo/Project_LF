using System.Collections;
using UnityEngine;

public class MonsterTracking : MonoBehaviour
{
    private enum State
    { 
        Idle,
        Tracking,
        Attack
    }

    [SerializeField]
    private float moveSpeed = 3f;
    [SerializeField]
    private float detectedRange = 8f;
    [SerializeField]
    private float attackRange = 1.2f;
    [SerializeField]
    private SpriteRenderer attackSprite;
    [SerializeField]
    private Collider2D attackCollider;

    [SerializeField]
    private float attackDuration = 0.2f;
    [SerializeField]
    private float attackCooldown = 1f;

    private Transform player;
    private Rigidbody2D rigid;

    private bool isAttacking;

    private State currentState = State.Idle;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();   
        //if(rigid == null)
        //{
        //    //Debug.LogError($"[{name}] Rigidbody2D가 없습니다.");
        //}
    }

    private void Start()
    {
        isAttacking = false;
        attackSprite.enabled = false;
        attackCollider.enabled = false;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            //Debug.LogError($"[{name}] Player 태그 오브젝트를 찾지 못했습니다.");
            return;
        }

        player = playerObj.transform;
        //Debug.Log($"[{name}] Player 찾음: {player.name}");
    }

    private void FixedUpdate()
    {
        TrackingState();
    }
    private void TrackingState()
    {
        if (player == null)
        {
            //Debug.LogWarning($"[{name}] player가 null 입니다.");
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        //Debug.Log($"[{name}] 현재 상태={currentState}, 거리={distance}");


        switch (currentState)
        {
            case State.Idle:
                rigid.linearVelocity = Vector2.zero;
                //Debug.Log($"[{name}] Idle 상태");
                if (distance <= detectedRange)
                {
                   // Debug.Log($"[{name}] detectRange 진입 -> Chase 전환");
                    currentState = State.Tracking;
                }
                break;
            case State.Tracking:
                //Debug.Log($"[{name}] Tracking 상태");
                if (distance <= attackRange)
                {
                   // Debug.Log($"[{name}] attackRange 진입 -> Attack 전환");
                    rigid.linearVelocity = Vector2.zero;
                    currentState = State.Attack;
                }
                else if (distance > detectedRange)
                {
                    //Debug.Log($"[{name}] detectRange 이탈 -> Idle 전환");
                    rigid.linearVelocity = Vector2.zero;
                    currentState = State.Idle;
                }
                else
                {
                    Vector2 dir = ((Vector2)player.position - rigid.position).normalized;
                    rigid.linearVelocity = dir * moveSpeed;

                   // Debug.Log($"[{name}] 이동 방향={dir}, 속도={rigid.linearVelocity}");
                }
                break;

            case State.Attack:
                rigid.linearVelocity = Vector2.zero;
                //Debug.Log($"[{name}] Attack 상태");
                StartCoroutine(MonsterAttack());
                if (distance > attackRange)
                {
                   // Debug.Log($"[{name}] Attack 해제 -> Chase 전환");
                    currentState = State.Tracking;
                }
                break;
        }
    }

    private IEnumerator MonsterAttack()
    {
        // Debug.Log("[PlayerAttack] AttackRoutine START");
        isAttacking = true;
        attackSprite.enabled = true;

        if (attackCollider == null)
        {
            //Debug.LogError("[PlayerAttack] AttackRoutine failed - attackCollider is NULL");
            yield break;
        }

        attackCollider.enabled = true;
        // Debug.Log($"[PlayerAttack] attackCollider ON / enabled = {attackCollider.enabled}");
        yield return new WaitForSeconds(attackDuration);

        attackCollider.enabled = false;
        attackSprite.enabled = false;
        // Debug.Log($"[PlayerAttack] attackCollider OFF / enabled = {attackCollider.enabled}");
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectedRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, attackRange);
    }
}
