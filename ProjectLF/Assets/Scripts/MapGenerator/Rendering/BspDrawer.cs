using System.Drawing;
using UnityEngine;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Config;
using NUnit.Framework;
using System.Collections.Generic;
using System;


namespace BSPDungeonGenrator.Rendering
{

    public class BspDrawer : MonoBehaviour
    {
        [SerializeField]
        private GameObject linePrefab;
        [SerializeField]
        private GameObject rectanglePrefab;

        public void DrawRectangle(Vector2Int mapSize, Transform lineHolder)
        {
            if (rectanglePrefab == null || lineHolder == null)
                return;
            Vector3 center = new Vector3 (0f,0f,0f);
            GameObject rect = Instantiate(rectanglePrefab, center, Quaternion.identity, lineHolder);

            LineRenderer lineRenderer = rect.GetComponent<LineRenderer>();
            if (lineRenderer == null)
                return;

            float halfX = mapSize.x * 0.5f;
            float halfY = mapSize.y * 0.5f;

            Vector3 p0 = new Vector3(-halfX, -halfY, 0f);
            Vector3 p1 = new Vector3(-halfX, halfY, 0f);
            Vector3 p2 = new Vector3(halfX, halfY, 0f);
            Vector3 p3 = new Vector3(halfX, -halfY, 0f);

            lineRenderer.positionCount = 5;
            lineRenderer.SetPosition(0, p0);
            lineRenderer.SetPosition(1, p1);
            lineRenderer.SetPosition(2, p2);
            lineRenderer.SetPosition(3, p3);
            lineRenderer.SetPosition(4, p0);
        }

        // 라인 렌더러를 이용해 라인을 그리는 메소드
        public void DrawLine(List<LineSegment> splitLines, Vector2Int mapSize, Transform lineHolder)
        {
            if (splitLines == null || linePrefab == null || lineHolder == null)
                return;

            foreach (var seg in splitLines)
            {
                GameObject obj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineHolder);

                LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                    continue;

                Vector3 from = ToWorld(seg.from, mapSize);
                Vector3 to = ToWorld(seg.to, mapSize);

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, from);
                lineRenderer.SetPosition(1, to);
            }
           
        }

        private Vector3 ToWorld(Vector2 point, Vector2Int mapSize)
        {
            return new Vector3(
                point.x - mapSize.x * 0.5f,
                point.y - mapSize.y * 0.5f,
                0f);
        }

        private void DrawRectangle(int x, int y, DungeonContext ctx, DungeonData duengeonData, Transform lineHolder)
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