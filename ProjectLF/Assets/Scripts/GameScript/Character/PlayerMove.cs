using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField]
    private Character p_character;



    private Rigidbody2D rigid;

    private Vector3 moveInput = new Vector3();


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
