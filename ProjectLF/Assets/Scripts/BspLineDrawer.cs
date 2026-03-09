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

        // 라인 렌더러를 이용해 라인을 그리는 메소드
        public void OnDrawLine(DuengeonContext ctx)
        {

            foreach (var seg in ctx.SplitLines)
            {
                LineRenderer lineRenderer = Instantiate(line, lineHolder).GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, seg.from - ctx.MapSize / 2);
                lineRenderer.SetPosition(1, seg.to - ctx.MapSize / 2);
            }
           
        }

    }

}