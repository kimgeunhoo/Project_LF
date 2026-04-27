using ModularBSP.Core;
using UnityEngine;

namespace GameScript.Manager
{
    public class DungeonManager : MonoBehaviour
    {
        
        public void OnEnterRoom(int roomId, RoomType roomType)
        {

            switch (roomType)
            {
                case RoomType.Start:
                    break;
                case RoomType.Shop:
                    break;
                case RoomType.Stairs:
                    break;
                case RoomType.Encounter:
                    break;
                case RoomType.Enemy:
                    EnterEnemyRoom(roomId);
                    break;
                default:
                    break;
            }

        }

        private void EnterEnemyRoom(int roomId)
        {
            Debug.Log($"Enemy room entered: {roomId}");
        }
    }

}
