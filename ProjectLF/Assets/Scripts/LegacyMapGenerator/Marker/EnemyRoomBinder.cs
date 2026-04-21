using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Generation;
using BSPDungeonGenrator.Core;
using UnityEngine;

namespace BSPDungeonGenrator.marker
{
    public class EnemyRoomBinder : MonoBehaviour
    {
        [SerializeField]
        private DungeonManager dungeonManager;

        public void Bind(GameObject obj, RoomInfo room, int roomId, OldDungeonContext ctx)
        {
            BindTrigger(obj, roomId);
            SetupEnemyRoomCollider(obj, room);
        }

        private void BindTrigger(GameObject obj, int roomId)
        {
            EnemyRoomTrigger trigger = obj.GetComponent<EnemyRoomTrigger>();
            if (trigger != null)
            {
                trigger.Init(roomId, dungeonManager);
            }

        }

        private void SetupEnemyRoomCollider(GameObject obj, RoomInfo room)
        {
            BoxCollider2D col = obj.GetComponent<BoxCollider2D>();
            if (col == null)
                return;

            col.isTrigger = true;

            RectInt rect = room.Rect;
            col.size = new Vector2(rect.width, rect.height);

            Vector2 rectCenter = new Vector2(
                rect.xMin + rect.width * 0.5f,
                rect.yMin + rect.height *  0.5f
                );

            Vector2 roomCenter = new Vector2(room.Center.x, room.Center.y);
            Vector2 baseOffset = rectCenter - roomCenter;

            col.offset = baseOffset;

        }

    }

}
