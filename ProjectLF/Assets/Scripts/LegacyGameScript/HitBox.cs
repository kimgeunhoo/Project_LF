using UnityEngine;

namespace LegacyGameScrpit
{

    public class HitBox : MonoBehaviour
    {

        [SerializeField]
        private Character _player;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                Debug.Log($"[HitBox] Hit detected with {monster.name} / Monster HP before hit = {monster.Hp}");
                monster.Hp -= _player.Atk;
                if (monster.Hp <= 0)
                {
                    monster.StartCoroutine(monster.DiyingAnimation());
                }
                else
                {
                    monster.StartCoroutine(monster.DamageAnimation());
                }
            }
        }

    }

}