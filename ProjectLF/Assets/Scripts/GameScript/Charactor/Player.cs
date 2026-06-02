using System;
using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    private int[,] inventory;

    private int[] weaponSlot = new int[2];


    [Header("Player Data")]
    [SerializeField]
    private PlayerData playerdata;

    [Header("Player UI")]
    [SerializeField]
    private PlayerHpUI hpUI;

    [Header("Death")]
    private Animator animator;
    private int currentHp;
    private bool isDead = false;

    [Header("GameOver")]
    [SerializeField]
    private GameObject playerPF;
    [SerializeField]
    private GameOverUI playerPanel;
    [SerializeField] 
    private GameOverUI gameOverUI;

    public int CurrentHp => currentHp;
    public int MaxHp => playerdata.Hp;
    public bool IsDead => isDead;

    public event Action OnDead;

    private void Awake()
    {
        currentHp = MaxHp;
        animator = GetComponentInChildren<Animator>();
        if (gameOverUI == null)
            gameOverUI = FindFirstObjectByType<GameOverUI>();
        UpdateHpUI();
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHp += amount;
        currentHp = Mathf.Clamp(currentHp, 0, MaxHp);

        UpdateHpUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHp -= damage;
        DamageAnimation();
        UpdateHpUI();

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageAnimation());
        }
    }


    private void UpdateHpUI()
    {
        if (hpUI != null)
        {
            hpUI.SetHp(CurrentHp, MaxHp);
        }
    }


    private void Die()
    {
        if (isDead)
            return;
        isDead = true;
        OnDead?.Invoke();
        StartCoroutine(DiyingAnimation());
        gameOverUI.ShowGameOver();
    }

    public IEnumerator DiyingAnimation()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.5f); // 애니메이션 길이에 맞게 조절

        playerPF.SetActive(false);
        //Destroy(gameObject);
    }

    public IEnumerator DamageAnimation()
    {
        animator.SetTrigger("Damaged");
        yield return new WaitForSeconds(0.3f); // 애니메이션 길이에 맞게 조절
    }

}
