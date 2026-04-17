using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField]
    private Character p_data;

    private Animator animator;

    private Rigidbody2D rigid;

    private Vector3 moveInput = new Vector3();


    private bool isMove = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        //Debug.Log($"[PlayerMove] rigid = {rigid}");
        //Debug.Log($"[PlayerMove] animator = {animator}");
        //Debug.Log($"[PlayerMove] p_character = {p_character}");
    }

    private void Start()
    {
        moveInput = Vector2.zero;

    }


    private void FixedUpdate()
    {
        rigid.linearVelocity = moveInput * p_data.Speed;
    }

    private void Update()
    {
        OnMove();
        UpdateAnimation();
    }

    private void OnMove()
    {
        
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, moveY, 0).normalized;
    }

    private void UpdateAnimation()
    {
        bool isMove = moveInput != Vector3.zero;
        animator.SetBool("Move", isMove);
    }

}
