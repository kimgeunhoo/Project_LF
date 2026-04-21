using UnityEngine;

namespace BSPDungeonGenrator.Utility
{
    public class ExitFindEndConnection : MonoBehaviour
    {
        public static void GetConnectionPoints(RectInt roomA, RectInt roomB,
            out Vector2Int pointA, out Vector2Int pointB)
        {
            Vector2Int centerA = new Vector2Int
                (roomA.x + roomA.width / 2,
                 roomA.y + roomA.height / 2);

            Vector2Int centerB = new Vector2Int
               (roomB.x + roomB.width / 2,
                roomB.y + roomB.height / 2);

            int dx = centerB.x - centerA.x;
            int dy = centerB.y - centerA.y;

            if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            {
                // 좌우 연결
                if (dx >= 0)
                {
                    pointA = new Vector2Int(
                        roomA.xMax - 1, Mathf.Clamp(centerA.y, roomA.yMin + 1, roomA.yMax - 2));
                    pointB = new Vector2Int(
                        roomB.xMin, Mathf.Clamp(centerB.y, roomB.yMin + 1, roomB.yMax - 2));
                }
                else
                {
                    pointA = new Vector2Int(
                        roomA.xMax, Mathf.Clamp(centerA.y, roomA.yMin + 1, roomA.yMax - 2));
                    pointB = new Vector2Int(
                        roomB.xMin - 1, Mathf.Clamp(centerB.y, roomB.yMin + 1, roomB.yMax - 2));
                }               
            }
            else
            {
                // 상하 연결
                if (dy >= 0)
                {
                    pointA = new Vector2Int(
                        Mathf.Clamp(centerA.x, roomA.xMin + 1, roomA.xMax - 2), roomA.yMax - 1);
                    pointB = new Vector2Int(
                        Mathf.Clamp(centerB.x, roomB.xMin + 1, roomB.xMax - 2), roomB.yMin);
                }
                else
                {
                    pointA = new Vector2Int(
                       Mathf.Clamp(centerA.x, roomA.xMin + 1, roomA.xMax - 2), roomA.yMin);
                    pointB = new Vector2Int(
                        Mathf.Clamp(centerB.x, roomB.xMin + 1, roomB.xMax - 2), roomB.yMax - 1);
                }
            }

        }

        
    }

}
