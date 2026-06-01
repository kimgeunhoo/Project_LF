using GameScript.Manager;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [Header("Data")]
    [SerializeField]
    private MonsterData data;
    [SerializeField]
    private Collider2D bodyCollider;

    [Header("Image")]
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Transform hpBarCanvas;
    [SerializeField]
    private RectTransform hpBarFill;

    [Header("Att Method")]
    [SerializeField]
    private Collider2D attackCollider;
    [SerializeField]
    private SpriteRenderer attackSprite;


    private int currentHp;
    private bool isDead;
    private GameObject monster;
    private int roomId;
    private DungeonManager dungeonManager;


    private Animator animator;
    
    private Vector3 hpBarBaseScale;
    private float parentSign;

    public MonsterData Data => data;

    public int CurrentHp => currentHp;

    private float maxWidth;

    private void Awake()
    {
        if(data == null)
        {
            Debug.LogError($"[{name}] MonsterData가 없습니다.");
            return;
        }
        animator = GetComponentInChildren<Animator>();
        currentHp = data.MaxHp;
        hpBarBaseScale = hpBarCanvas.localScale;
        maxWidth = hpBarFill.sizeDelta.x;
        SetHp();
    }

    private void LateUpdate()
    {
        hpBarCanvas.transform.rotation = Quaternion.identity;

        parentSign = Mathf.Sign(transform.lossyScale.x);

        hpBarCanvas.localScale = new Vector3(hpBarBaseScale.x * parentSign, hpBarBaseScale.y, hpBarBaseScale.z);
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

        SetHp();
        if(currentHp <= 0)
        {
            Die();
        }

    }

    private void SetHp()
    {
        float ratio = Mathf.Clamp01((float)currentHp / data.MaxHp);

        hpBarFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth * ratio);
    }


    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        StopAllCoroutines();

        bodyCollider.enabled = false;

        attackCollider.enabled = false;

        attackSprite.enabled = false;

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
