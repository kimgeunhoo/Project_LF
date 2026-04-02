using UnityEngine;

public class HitBox : MonoBehaviour
{

    [SerializeField]
    private Character _player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponent<Monster>();
        if (monster != null)
        {
            monster.Hp -= _player.Atk;
            if (monster.Hp <= 0)
            {
                monster.Die();
            }
            monster.Die();
        }
    }
}
