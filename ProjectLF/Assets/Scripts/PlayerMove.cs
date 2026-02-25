using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3f;

    Rigidbody2D rigid;

    [SerializeField]
    private Vector3 movePosition = new Vector3();

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        movePosition = transform.position;
    }

    private void FixedUpdate()
    {
        rigid.linearVelocity = movePosition * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movePosition = new Vector3 (moveX, moveY, 0);
    }



}
