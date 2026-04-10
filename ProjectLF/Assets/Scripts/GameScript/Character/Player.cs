using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int[,] inventory;

    private int[] weaponSlot = new int[2];

    [Header("Player Data")]
    [SerializeField]
    private Character p_character;

    private Rigidbody2D rigid;

    [SerializeField]
    private Vector3 moveInput = new Vector3();


    private Player player;

    private GameObject atkCol;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        moveInput = Vector2.zero;
    }


    private void FixedUpdate()
    {
        rigid.linearVelocity = moveInput * p_character.Speed;
    }

    private void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, moveY, 0).normalized;
    }



}
