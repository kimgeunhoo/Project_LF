using System.Drawing;
using UnityEngine;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;


namespace BSPDuengeonGenrator.Rendering
{

    public class BspLineDrawer : MonoBehaviour
    {
        [Header("Random Liner")]
        [SerializeField]
        private GameObject line;
        [SerializeField]
        private Transform lineHolder;
        [SerializeField]
        private GameObject rectangle;
        [SerializeField]
        private GameObject LineRenderer;

        private DuengeonContext ctx;

        // 라인 렌더러를 이용해 라인을 그리는 메소드
        private void OnDrawLine(Vector2 from, Vector2 to)
        {
            LineRenderer lineRenderer = Instantiate(line, lineHolder).GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, from - ctx.MapSize / 2);
            lineRenderer.SetPosition(1, to - ctx.MapSize / 2);
        }

        // 라인 렌더러를 이용해 사각형을 그리는 메소드
        private void OnDrawRectangle(int x, int y, int width, int height)
        {
            LineRenderer lineRenderer = Instantiate(rectangle, lineHolder).GetComponent<LineRenderer>();
            // 위치를 화면 중앙에 맞춤
            lineRenderer.SetPosition(0, new Vector2(x, y) - ctx.MapSize / 2);
            lineRenderer.SetPosition(1, new Vector2(x + width, y) - ctx.MapSize / 2);
            lineRenderer.SetPosition(2, new Vector2(x + width, y + height) - ctx.MapSize / 2);
            lineRenderer.SetPosition(3, new Vector2(x, y + height) - ctx.MapSize / 2);
        }
    }

}