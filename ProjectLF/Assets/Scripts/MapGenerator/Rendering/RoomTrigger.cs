using GameScript.Manager;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Trigger
{
	public class RoomTrigger : MonoBehaviour
	{
		private int roomId;
		private RoomType roomType;
		private DungeonManager dungeonManager;

		public void Init(int roomId, RoomType roomType, DungeonManager dungeonManger)
		{
			this.roomId = roomId;
			this.roomType = roomType;
			this.dungeonManager = dungeonManger;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log($"[RoomTrigger] Enter 감지: {collision.name}, tag={collision.tag}");

            if (!collision.CompareTag("Player"))
                return;

            Debug.Log($"[RoomTrigger] Player 입장 / roomId={roomId}, roomType={roomType}");

            if (dungeonManager == null)
            {
                dungeonManager = FindFirstObjectByType<DungeonManager>();
            }

            if (dungeonManager != null)
			{
				dungeonManager.OnEnterRoom(roomId, roomType);
			}


        }

    } 
}
