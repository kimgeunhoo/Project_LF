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
            if(!collision.CompareTag("Player"))
				return;

			if (dungeonManager != null)
			{
				dungeonManager.OnEnterRoom(roomId, roomType);
			}


        }

    } 
}
