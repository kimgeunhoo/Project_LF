using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField]
    private Character p_data;



    [SerializeField]
    private Collider2D attackCollider;
    [SerializeField]
    private float attackDuration = 0.2f;
    [SerializeField]
    private float attackCooldown = 0.2f;

    [SerializeField]
    private SpriteRenderer attackSprite;

    private Animator animator;
    

    private bool isAttacking = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        attackCollider.gameObject.SetActive(true);
        if(attackCollider == null)
        {    
            attackCollider = GetComponentInChildren<Collider2D>();
            //Debug.Log($"[PlayerAttack] attackCollider auto find = {(attackCollider != null ? attackCollider.name : "NULL")}");
        }
        else
        {
            //Debug.Log($"[PlayerAttack] attackCollider assigned = {attackCollider.name}");
        }

        
        attackCollider.enabled = false;
        attackSprite.enabled = false;
        //Debug.Log("[PlayerAttack] attackCollider disabled at start");
        

      
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"[PlayerAttack] Left Click detected / isAttacking = {isAttacking}");
            TryAttack();
        }
    }

    private void TryAttack()
    {
        Debug.Log("[PlayerAttack] TryAttack called");
        if (isAttacking)
        {
            //Debug.Log("[PlayerAttack] blocked - already attacking");
            return;
        }
        StartCoroutine(AttackCouroutine());
    }

    public IEnumerator AttackCouroutine()
    {
       // Debug.Log("[PlayerAttack] AttackRoutine START");
        isAttacking = true;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDuration);

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

}
