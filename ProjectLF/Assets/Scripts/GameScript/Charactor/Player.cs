using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    public int[,] inventory;

    public int[] weaponSlot = new int[2];

    public GameObject atkCol;

    public Animator animator;

    private bool isDead = false;

    [Header("Player Data")]
    [SerializeField]
    private PlayerData m_Player;

    public int Hp;

    private void Awake()
    {
        Hp = m_Player.Hp;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        Hp -= damage;
        DamageAnimation();

        if (Hp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageAnimation());
        }
    }

    private void Die()
    {
        if (isDead)
            return;
        isDead = true;
        StartCoroutine(DiyingAnimation());
        Destroy(gameObject);
    }

    public IEnumerator DiyingAnimation()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.5f); // 애니메이션 길이에 맞게 조절
    }

    public IEnumerator DamageAnimation()
    {
        animator.SetTrigger("Damaged");
        yield return new WaitForSeconds(0.3f); // 애니메이션 길이에 맞게 조절
    }

}
