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
    private float attackWarningTime = 0.8f;
    [SerializeField] 
    private float attackDuration = 0.5f;
    [SerializeField] 
    private float attackCooldown = 1f;

    [SerializeField]
    private float attackOffset = 0.5f;

    [SerializeField]
    private Animator animator;

    private Transform playerTrs;

    private bool isAttacking = false;

    private Vector3 maskBaseScale;
    private Vector3 spriteBaseScale;

    private Vector2 colliderBaseSize;
    private Vector2 colliderBaseOffset;

    private void Awake()
    {
        maskBaseScale = attackMask.transform.localScale;
        spriteBaseScale = attackSprite.transform.localScale;
        attackSprite.transform.localPosition = Vector3.zero;
        attackMask.transform.localPosition = Vector3.zero;
        attackCollider.transform.localPosition = Vector3.zero;

    }

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

            Vector2 dir =((Vector2)playerTrs.position - (Vector2)transform.position).normalized;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            attackRoot.position = transform.position + (Vector3)(dir * attackOffset);

            attackRoot.rotation = Quaternion.Euler(0f, 0f, angle);

            attackSprite.enabled = true;
            attackCollider.enabled = false;


            attackSprite.transform.localScale = spriteBaseScale;

            attackMask.transform.localScale = new Vector3(0f, maskBaseScale.y, maskBaseScale.z);

            attackMask.transform.localPosition = Vector3.zero;

            attackCollider.size = new Vector2(0f, colliderBaseSize.y);

            attackCollider.offset = new Vector2(0f, colliderBaseOffset.y);

            float elapsed = 0f;

            while (elapsed < attackWarningTime)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / attackWarningTime);

                float currentScaleX = maskBaseScale.x * t;

                attackMask.transform.localScale =
                    new Vector3(currentScaleX, maskBaseScale.y, maskBaseScale.z);

                attackMask.transform.localPosition = new Vector3(currentScaleX * 0.5f, 0f, 0f);

                attackCollider.size = new Vector2(colliderBaseSize.x * t, colliderBaseSize.y);

                attackCollider.offset = 
                    new Vector2((colliderBaseSize.x * t) * 0.5f, colliderBaseOffset.y);

                yield return null;
            }

            attackCollider.enabled = true;

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            yield return new WaitForSeconds(attackDuration);

            attackCollider.enabled = false;
            attackSprite.enabled = false;

            yield return new WaitForSeconds(attackCooldown);

            isAttacking = false;
        }
    }

}
