using System.Collections;
using UnityEngine;

public class MonsterAtk : MonoBehaviour
{
    [SerializeField] 
    private Transform attackRoot;
    [SerializeField] 
    private SpriteRenderer attackSprite;
    [SerializeField] 
    private SpriteMask attackMask;
    [SerializeField] 
    private BoxCollider2D attackCollider;

    [SerializeField] 
    private float attackRange = 3f;
    [SerializeField] 
    private float attackWidth = 1f;
    [SerializeField] 
    private float attackWarnningTime = 0.8f;
    [SerializeField] 
    private float attackDuration = 0.5f;
    [SerializeField] 
    private float attackCooldown = 1f;

    [SerializeField]
    private Animator animator;

    private Transform playerTrs;

    private bool isAttacking = false;


    public IEnumerator OnMonsterAttack(Transform _playerTrs)
    {
        playerTrs = _playerTrs;
        StartCoroutine(MonsterAttack());
        yield return null;
    }

    private IEnumerator MonsterAttack()
    {
        if (isAttacking == false)
        { 
            isAttacking = true;
            Vector2 dir = ((Vector2)playerTrs.position - (Vector2)transform.position).normalized;

            // 공격 범위의 중심 위치
            Vector2 centerPos = (Vector2)transform.position + dir * (attackRange * 0.5f);

            attackRoot.position = transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            attackRoot.rotation = Quaternion.Euler(0f, 0f, angle);

            attackSprite.enabled = true;
            attackCollider.enabled = false;

            // 공격 범위 전체 크기
            attackSprite.transform.localScale = new Vector3(attackRange, attackWidth, 1f);

            // Collider도 같은 위치/크기
            attackCollider.transform.position = centerPos;
            attackCollider.transform.rotation = attackRoot.rotation;
            attackCollider.size = new Vector2(attackRange, attackWidth);

            float elapsed = 0f;

            while (elapsed < attackWarnningTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / attackWarnningTime;

                // 왼쪽에서 오른쪽으로 차오르는 효과
                float currentLength = Mathf.Lerp(0f, attackRange, t);

                attackMask.transform.localScale = new Vector3(currentLength, attackWidth, 1f);

                // 마스크 중심을 왼쪽 시작점 기준으로 보정
                attackMask.transform.localPosition =
                    new Vector3(
                        -attackRange * 0.5f + currentLength * 0.5f,
                        0f,
                        0f
                    );

                yield return null;
            }

            attackCollider.enabled = true;

            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackDuration);

            attackCollider.enabled = false;
            attackSprite.enabled = false;

            yield return new WaitForSeconds(attackCooldown);

            isAttacking = false;
        
        }
    }

}
