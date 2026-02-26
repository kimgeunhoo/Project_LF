using System.Xml.Linq;
using UnityEngine;
using static BspTree;

public class BspSplitter : MonoBehaviour
{

    [Header("Map Size")]
    [SerializeField]
    private Vector2Int mapSize;

    // 노드 값이 라인의 갯수를 판별
    [Header("Node Value")]
    [SerializeField]
    private int maxNode;
    [SerializeField]
    private int minNode;

    [Header("Room Magnification")]
    [SerializeField]
    private float minDivideSize;
    [SerializeField]
    private float maxDivideSize;

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

    public void Run(DuengeonContext ctx)
    {
        this.ctx = ctx;
        DivideTree(ctx.Root, 0);
    }


    // 재귀 함수
    private void DivideTree(TreeNode treeNode, int n)
    {
        if (n < maxNode) // 0부터 노드 최대값에 이를 때 까지 반복
        {
            // 이진 트리의 범위 값 저장, 사각형 범위 담기
            RectInt size = treeNode.treeSize;
            // 사각형의 가로와 세로 길이 중 길이가 긴 축을 트리 반으로 나누는 기준선으로
            int length = size.width >= size.height ? size.width : size.height;
            // 기준선 위에서 최소 범위와 최대 범위 사이의 값 무작위 선택
            int split = Mathf.RoundToInt(Random.Range(length * minDivideSize, length * maxDivideSize));
            // 노드 크기 안정처리
            split = Mathf.Clamp(split, minNode, length - minNode);
            // 가로
            if (size.width >= size.height)
            {
                // 기준선을 반으로 나눈 값인 split을 가로 길이로, 이진트리의 height값을 세로 길이로 사용
                treeNode.leftTree = new TreeNode(size.x, size.y, split, size.height);
                // x 값에 split값을 더해 좌표 설정. 이전 트리의 width값에 split값을 빼 가로 길이 설정
                treeNode.rightTree = new TreeNode(size.x + split, size.y, size.width - split, size.height);
                OnDrawLine(new Vector2(size.x + split, size.y),
                    new Vector2(size.x + split, size.y + size.height));
            }
            // 세로
            else
            {
                treeNode.leftTree = new TreeNode(size.x, size.y, size.width, split);
                treeNode.rightTree = new TreeNode(size.x, size.y + split, size.width, size.height - split);
                OnDrawLine(new Vector2(size.x, size.y + split),
                    new Vector2(size.x + size.width, size.y + split));
            }
            // 분할한 트리의 부모 트리를 매개 변수로 받은 트리로 할당
            treeNode.leftTree.parentTree = treeNode;
            treeNode.rightTree.parentTree = treeNode;
            // 재귀 함수, 자식 트리를 매개변수로 넘기고 노드 값 1 증가시킴
            // 순회 방식
            DivideTree(treeNode.leftTree, n + 1);
            DivideTree(treeNode.rightTree, n + 1);
        }
    }

    private void OnDrawLine(Vector2 from, Vector2 to)
    {
        LineRenderer lineRenderer = Instantiate(line, lineHolder).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }
}
