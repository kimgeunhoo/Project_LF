using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace LegacyGameScrpit
{

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
        private float attackWarnningTime = 0.8f;
        [SerializeField]
        private float attackDuration = 0.2f;
        [SerializeField]
        private float attackCooldown = 1f;

        private Animator animator;

        private Transform visualRoot;
        private Transform player;
        private Rigidbody2D rigid;

        private bool isAttacking;

        private State currentState = State.Idle;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            visualRoot = GetComponent<Transform>();
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

            // 공격 도중엔 이동 제한
            // 일부 적은 달라질 수 있음
            if (isAttacking)
            {
                rigid.linearVelocity = Vector2.zero;
                return;
            }

            float distance = Vector2.Distance(transform.position, player.position);
            //Debug.Log($"[{name}] 현재 상태={currentState}, 거리={distance}");

            float dirX = player.position.x - transform.position.x;

            if (dirX > 0.01f)
            {
                visualRoot.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (dirX < -0.01f)
            {
                visualRoot.localScale = new Vector3(1f, 1f, 1f);
            }

            switch (currentState)
            {
                case State.Idle:
                    rigid.linearVelocity = Vector2.zero;
                    //Debug.Log($"[{name}] Idle 상태");
                    animator.SetBool("Move", false);
                    if (distance <= detectedRange)
                    {
                        // Debug.Log($"[{name}] detectRange 진입 -> Chase 전환");
                        currentState = State.Tracking;
                    }
                    break;
                case State.Tracking:
                    //Debug.Log($"[{name}] Tracking 상태");
                    animator.SetBool("Move", true);
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

                    if (!isAttacking)
                    {
                        //Debug.Log($"[{name}] Attack 시작");
                        StartCoroutine(MonsterAttack());
                    }

                    if (distance > attackRange)
                    {
                        // Debug.Log($"[{name}] Attack 해제 -> Chase 전환");
                        currentState = State.Tracking;
                    }
                    break;
            }
        }
        //private IEnumerator MonsterAttack()
        //{
        //    // Debug.Log("[PlayerAttack] AttackRoutine START");
        //    isAttacking = true;

        //    Vector2 targetPos = player.position;

        //    attackSprite.transform.position = targetPos;
        //    attackCollider.transform.position = targetPos;

        //    attackSprite.enabled = true;
        //    attackCollider.enabled = false;

        //    attackSprite.transform.localScale = Vector3.zero;

        //    float elapsed = 0f;
        //    while (elapsed < attackWarnningTime)
        //    {
        //        elapsed += Time.deltaTime;
        //        float t = elapsed / attackWarnningTime;
        //        attackSprite.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        //        yield return null;
        //    }

        //    yield return new WaitForSeconds(attackWarnningTime);

        //    // 공격 활성
        //    attackCollider.enabled = true;

        //    yield return new WaitForSeconds(attackDuration);
        //    // Debug.Log($"[PlayerAttack] attackCollider ON / enabled = {attackCollider.enabled}");

        //    // 공격 종료
        //    attackCollider.enabled = false;
        //    attackSprite.enabled = false;
        //    // Debug.Log($"[PlayerAttack] attackCollider OFF / enabled = {attackCollider.enabled}");
        //    yield return new WaitForSeconds(attackCooldown);

        //    isAttacking = false;
        //}

        // 이거 유도성 메테오공격 등 으로 써먹을수 있겠다
        private IEnumerator MonsterAttack()
        {
            // Debug.Log("[PlayerAttack] AttackRoutine START");
            isAttacking = true;

            Vector2 targetPos = player.position;

            attackSprite.transform.position = targetPos;
            attackCollider.transform.position = targetPos;

            attackSprite.enabled = true;
            attackCollider.enabled = false;

            attackSprite.transform.localScale = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < attackWarnningTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / attackWarnningTime;
                attackSprite.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }

            // 공격 활성
            attackCollider.enabled = true;
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackDuration);
            // Debug.Log($"[PlayerAttack] attackCollider ON / enabled = {attackCollider.enabled}");

            // 공격 종료
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
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

}