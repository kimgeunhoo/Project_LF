using UnityEngine;

public class Player : MonoBehaviour
{
    private int[,] inventory;

    private int[] weaponSlot = new int[2];

    [Header("Player Data")]
    [SerializeField]
    private Character p_character;

    Rigidbody2D rigid;

    [SerializeField]
    private Vector3 movePosition = new Vector3();

    private Player player;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        movePosition = transform.position;
    }

    private void FixedUpdate()
    {
        rigid.linearVelocity = movePosition * p_character.Speed;
    }

    private void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movePosition = new Vector3(moveX, moveY, 0);
    }


}
