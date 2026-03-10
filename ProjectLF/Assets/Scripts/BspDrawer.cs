using System.Drawing;
using UnityEngine;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;


namespace BSPDuengeonGenrator.Rendering
{

    public class BspDrawer : MonoBehaviour
    {
        private DuengeonContext ctx;
        private DuengeonData duengeonData;
        public void OnDrawLine(DuengeonContext ctx, DuengeonData duengeonData, Transform lineHolder)
        {
            if (duengeonData == null)
            {
                Debug.Log($"던전 데이터가 없습니다.");
            }
            if (ctx == null)
            {
                Debug.Log($"ctx 데이터가 없습니다.");
            }
            this.ctx = ctx;
            this.duengeonData = duengeonData;
           
            DrawLine(ctx, duengeonData, lineHolder);
        }
        public void OnDrawRectangle(DuengeonContext ctx, DuengeonData duengeonData, Transform lineHolder)
        {
            if (duengeonData == null)
            {
                Debug.Log($"던전 데이터가 없습니다.");
            }
            if (ctx == null)
            {
                Debug.Log($"ctx 데이터가 없습니다.");
            }
            this.ctx = ctx;
            this.duengeonData = duengeonData;
            DrawRectangle(0,0, ctx, duengeonData, lineHolder);
        }

        // 라인 렌더러를 이용해 라인을 그리는 메소드
        private void DrawLine(DuengeonContext ctx, DuengeonData duengeonData, Transform lineHolder)
        {

            foreach (var seg in ctx.SplitLines)
            {
                LineRenderer lineRenderer = Instantiate(duengeonData.Line, lineHolder).GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, seg.from - ctx.MapSize / 2);
                lineRenderer.SetPosition(1, seg.to - ctx.MapSize / 2);
            }
           
        }
        private void DrawRectangle(int x, int y, DuengeonContext ctx, DuengeonData duengeonData, Transform lineHolder)
        {
            LineRenderer lineRenderer = Instantiate(duengeonData.Rectangle, lineHolder).GetComponent<LineRenderer>();
            // 위치를 화면 중앙에 맞춤
            lineRenderer.SetPosition(0, new Vector2(x, y) - ctx.MapSize / 2);
            lineRenderer.SetPosition(1, new Vector2(x + ctx.MapSize.x, y) - ctx.MapSize / 2);
            lineRenderer.SetPosition(2, new Vector2(x + ctx.MapSize.x, y + ctx.MapSize.y) - ctx.MapSize / 2);
            lineRenderer.SetPosition(3, new Vector2(x, y + ctx.MapSize.y) - ctx.MapSize / 2);
        }
    }

}