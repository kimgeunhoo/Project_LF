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

        private DungeonContext ctx;
        private DungeonData dungeonData;

        [SerializeField]
        private GameObject linePrefab;
        [SerializeField]
        private GameObject rectanglePrefab;

        public void OnDrawLine(DungeonContext ctx, DungeonData dungeonData, Transform lineHolder)
        {
            if (dungeonData == null || ctx == null || ctx.Root == null) 
                return;

            this.ctx = ctx;
            this.dungeonData = dungeonData;

            DrawLine(lineHolder);
        }

        public void OnDrawRectangle(DungeonContext ctx, DungeonData dungeonData, Transform lineHolder)
        {
            if (ctx == null || dungeonData == null || lineHolder == null || ctx.Root == null)
                return;

            this.ctx = ctx;
            this.dungeonData = dungeonData;

            DrawNodeRectangles(ctx.Root, lineHolder);
        }

        public void OnDrawLeafRectangle(DungeonContext ctx, DungeonData dungeonData, Transform lineHolder)
        {
            if (ctx == null || dungeonData == null || lineHolder == null || ctx.Root == null)
                return;
            this.ctx = ctx;
            this.dungeonData = dungeonData;

            DrawLeafRectangles(ctx.Root, lineHolder);
        }

        private void DrawLine(Transform lineHolder)
        {
            if (ctx.SplitLines == null || ctx.SplitLines.Count == 0)
                return;

            foreach (var seg in ctx.SplitLines)
            {
                if (dungeonData.Line == null)
                    continue;

                LineRenderer lineRenderer = Instantiate(dungeonData.Line, lineHolder).GetComponent<LineRenderer>();
                if (lineRenderer == null)
                    continue;

                lineRenderer.positionCount = 2;
                lineRenderer.loop = false;

                Vector3 from = new Vector3(seg.from.x, seg.from.y, 0f) - 
                    new Vector3((ctx.MapSize.x / 2), (ctx.MapSize.y / 2), 0f);
                Vector3 to = new Vector3(seg.to.x, seg.to.y, 0f) - 
                    new Vector3((ctx.MapSize.x / 2), (ctx.MapSize.y / 2), 0f);

                lineRenderer.SetPosition(0, from);
                lineRenderer.SetPosition(1, to);
            }
        }
        private void DrawNodeRectangles(TreeNode node, Transform lineHolder)
        {
            if (node == null)
                return;

            DrawSingleRectangle(node.treeSize, lineHolder);

            DrawNodeRectangles(node.leftTree, lineHolder);
            DrawNodeRectangles(node.rightTree, lineHolder);
        }

        private void DrawLeafRectangles(TreeNode node, Transform lineHolder)
        {
            if (node == null)
                return;

            bool isLeaf = node.leftTree == null && node.rightTree == null;

            if (isLeaf)
            {
                DrawSingleRectangle(node.treeSize, lineHolder);
                return;
            }

            DrawLeafRectangles(node.leftTree, lineHolder);
            DrawLeafRectangles(node.rightTree, lineHolder);
        }

        private void DrawSingleRectangle(RectInt rect, Transform lineHolder)
        {
            if (dungeonData.Rectangle == null)
                return;

            LineRenderer lineRenderer = Instantiate(dungeonData.Rectangle, lineHolder).GetComponent<LineRenderer>();
            if (lineRenderer == null)
                return;

            lineRenderer.positionCount = 5;
            lineRenderer.loop = false;

            Vector3 p0 = new Vector3(rect.xMin, rect.yMin, 0f) - 
                new Vector3((ctx.MapSize.x / 2), (ctx.MapSize.y / 2), 0f);
            Vector3 p1 = new Vector3(rect.xMax, rect.yMin, 0f) - 
                new Vector3((ctx.MapSize.x / 2), (ctx.MapSize.y / 2), 0f);
            Vector3 p2 = new Vector3(rect.xMax, rect.yMax, 0f) - 
                new Vector3((ctx.MapSize.x / 2), (ctx.MapSize.y / 2), 0f);
            Vector3 p3 = new Vector3(rect.xMin, rect.yMax, 0f) - 
                new Vector3((ctx.MapSize.x / 2), (ctx.MapSize.y / 2), 0f);

            lineRenderer.SetPosition(0, p0);
            lineRenderer.SetPosition(1, p1);
            lineRenderer.SetPosition(2, p2);
            lineRenderer.SetPosition(3, p3);
            lineRenderer.SetPosition(4, p0);
        }

    }

}