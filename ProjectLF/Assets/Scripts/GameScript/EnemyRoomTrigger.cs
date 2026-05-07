using UnityEngine;


namespace GameScript.Manager
{
    public class EnemyRoomTrigger : MonoBehaviour
    {
        private int roomId;

        public void Init(int _roomId)
        {
            this.roomId = _roomId;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
                return;

            GameManager.Instance.OnEnterRoom(roomId);


        }

    }

}
