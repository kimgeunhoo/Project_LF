using BSPDuengeonGenrator.Config;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Generation;
using BSPDuengeonGenrator.Rendering;
using System.Xml.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BSPDuengeonGenrator.Generation
{
    public class BspSplitter
    {
        private DuengeonContext ctx;

        // 라인 렌더링은 따로 호출시킨다.
        private LineRenderInterface lineRender;
        public BspSplitter(LineRenderInterface _lineRender = null)
        {
            this.lineRender = _lineRender;
        }

 
        public void Run(DuengeonContext ctx)
        {
            //Debug.Log($"[BspSplitter.Run] MaxNode={ctx.MaxNode}, RootSize=({ctx.Root.treeSize.width}, {ctx.Root.treeSize.height})");
            //Debug.Log($"[BspSplitter.Run] this={GetHashCode()} ctx={ctx.GetHashCode()}");
            this.ctx = ctx;
            DivideTree(ctx.Root, 0);
            //Debug.Log($"[BspSplitter.Run] After Divide: left={(ctx.Root.leftTree == null ? "NULL" : "OK")}, right={(ctx.Root.rightTree == null ? "NULL" : "OK")}");
        }


        // 재귀 함수
        private void DivideTree(TreeNode treeNode, int depth)
        {
            //Debug.Log($"[DivideTree] depth={depth}, width={treeNode.treeSize.width}, height={treeNode.treeSize.height}, MaxNode={ctx.MaxNode}, MinNode={ctx.MinNode}");
            if (depth < ctx.MaxNode) // 0부터 노드 최대값에 이를 때 까지 반복
            {
                // 이진 트리의 범위 값 저장, 사각형 범위 담기
                RectInt size = treeNode.treeSize;
                // 사각형의 가로와 세로 길이 중 길이가 긴 축을 트리 반으로 나누는 기준선으로
                int length = size.width >= size.height ? size.width : size.height;
                // 기준선 위에서 최소 범위와 최대 범위 사이의 값 무작위 선택
                int split = Mathf.RoundToInt(Random.Range(length * ctx.MinDivideSize, length * ctx.MaxDivideSize));
                // 노드 크기 안정처리
                split = Mathf.Clamp(split, ctx.MinNode, length - ctx.MinNode);

                //Debug.Log($"[DivideTree] split={split}, length={length}");

                // 가로
                if (size.width >= size.height)
                {
                    // 기준선을 반으로 나눈 값인 split을 가로 길이로, 이진트리의 height값을 세로 길이로 사용
                    treeNode.leftTree = new TreeNode(size.x, size.y, split, size.height);
                    // x 값에 split값을 더해 좌표 설정. 이전 트리의 width값에 split값을 빼 가로 길이 설정
                    treeNode.rightTree = new TreeNode(size.x + split, size.y, size.width - split, size.height);

                    ctx.SplitLines.Add(new LineSegment(
                        new Vector2(size.x + split, size.y),
                        new Vector2(size.x + split, size.y + size.height)));
                    //Debug.Log($"[DivideTree] horizontal split → left=({treeNode.leftTree.treeSize.width},{treeNode.leftTree.treeSize.height}), right=({treeNode.rightTree.treeSize.width},{treeNode.rightTree.treeSize.height})");
                }
                // 세로
                else
                {
                    treeNode.leftTree = new TreeNode(size.x, size.y, size.width, split);
                    treeNode.rightTree = new TreeNode(size.x, size.y + split, size.width, size.height - split);

                    ctx.SplitLines.Add(new LineSegment(
                        new Vector2(size.x, size.y + split),
                        new Vector2(size.x + size.width, size.y + split)));
                    //Debug.Log($"[DivideTree] vertical split → left=({treeNode.leftTree.treeSize.width},{treeNode.leftTree.treeSize.height}), right=({treeNode.rightTree.treeSize.width},{treeNode.rightTree.treeSize.height})");
                }
                // 분할한 트리의 부모 트리를 매개 변수로 받은 트리로 할당
                treeNode.leftTree.parentTree = treeNode;
                treeNode.rightTree.parentTree = treeNode;
                // 재귀 함수, 자식 트리를 매개변수로 넘기고 노드 값 1 증가시킴
                // 순회 방식
                DivideTree(treeNode.leftTree, depth + 1);
                DivideTree(treeNode.rightTree, depth + 1);
            }
            else
            {
                //Debug.Log($"[DivideTree] stop: depth={depth} >= MaxNode={ctx.MaxNode}");
            }
        }

    }

}