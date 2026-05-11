using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Generation
{
    public class BspPartitioner
    {
        private DungeonConfig config;
        private Vector2Int minRoomSize;
        public BspPartitioner(DungeonConfig config)
        {
            this.config = config;

            this.minRoomSize = new Vector2Int(
                Mathf.Max(3, (int)(config.minLeafSize.x * 0.6f)),
                Mathf.Max(3, (int)(config.minLeafSize.y * 0.6f))
            );
        }

        public BspNode CreateTree()
        {
            IntRect rootRect = new IntRect
                (0, 0, config.mapWidthInCells, config.mapHeightInCells);
            BspNode root = new BspNode(rootRect);
            SplitRecursive(root, 0);
            return root;
        }

        private void SplitRecursive(BspNode node, int depth)
        {
            // 종료 조건: 크기가 작거나 최대 깊이 도달
            // *단, 크기가 매우 크다면 깊이를 무시하고 쪼개는 것이 공간 활용에 좋음.
            if (depth >= config.maxDepth && node.Bounds.width < config.minLeafSize.x * 2.5f && node.Bounds.height < config.minLeafSize.y * 2.5f)
                return;

            // 1. 강제 종료 조건 완화: 
            // '방이 들어갈 수 있는 크기(config.minLeafSize)'만 확보되면 억지로라도 쪼갠다.
            // 기존의 '최소 크기의 2배' 조건을 '1.2배' 정도로 낮춤.
            float splitThresholdV = 1.2f;
            float splitThresholdH = 1.2f;

            bool canSplitHorizontally = node.Bounds.height >= config.minLeafSize.y * splitThresholdH;
            bool canSplitVertically = node.Bounds.width >= config.minLeafSize.x * splitThresholdV;

            if (!canSplitHorizontally && !canSplitVertically)
                return; // 더 이상 쪼갤 수 없음 (이곳이 리프 노드가 됨)

            bool splitVertical;
            // 비율 기반 방향 결정 (유지)
            if (node.Bounds.width > node.Bounds.height * 1.4f)
                splitVertical = true;
            else if (node.Bounds.height > node.Bounds.width * 1.4f)
                splitVertical = false;
            else
                splitVertical = Random.value > 0.5f;

            // 2. 분할 실행 (Greedy 최적화)
            if (splitVertical && canSplitVertically)
            {
                // 분할 범위 확대 (20% ~ 80%)하여 공간 낭비 최소화
                int minSplitOffset = Mathf.Max(config.minLeafSize.x, (int)(node.Bounds.width * 0.2f));
                int maxSplitOffset = Mathf.Min(node.Bounds.width - config.minLeafSize.x, (int)(node.Bounds.width * 0.8f));

                // 아주 중요한 안전장치: 자르는 지점이 minRoomSize는 확보하게 해야 함
                int finalMin = Mathf.Max(minRoomSize.x + 1, minSplitOffset);
                int finalMax = Mathf.Min(node.Bounds.width - (minRoomSize.x + 1), maxSplitOffset);

                // 만약 이 조건도 통과 못하면, 비율이 나빠지더라도 minLeafSize 기준으로 강제 분할
                if (finalMax <= finalMin)
                {
                    finalMin = config.minLeafSize.x;
                    finalMax = node.Bounds.width - config.minLeafSize.x;
                }

                if (finalMax <= finalMin) return; // 정말 쪼갤 수 없음

                int splitPoint = node.Bounds.x + Random.Range(finalMin, finalMax);

                IntRect left = new IntRect(node.Bounds.x, node.Bounds.y, splitPoint - node.Bounds.x, node.Bounds.height);
                IntRect right = new IntRect(splitPoint, node.Bounds.y, node.Bounds.xMax - splitPoint, node.Bounds.height);

                node.Left = new BspNode(left) { Parent = node };
                node.Right = new BspNode(right) { Parent = node };
            }
            else if (canSplitHorizontally) // 수평 분할 (동일 로직)
            {
                int minSplitOffset = Mathf.Max(config.minLeafSize.y, (int)(node.Bounds.height * 0.2f));
                int maxSplitOffset = Mathf.Min(node.Bounds.height - config.minLeafSize.y, (int)(node.Bounds.height * 0.8f));

                int finalMin = Mathf.Max(minRoomSize.y + 1, minSplitOffset);
                int finalMax = Mathf.Min(node.Bounds.height - (minRoomSize.y + 1), maxSplitOffset);

                if (finalMax <= finalMin)
                {
                    finalMin = config.minLeafSize.y;
                    finalMax = node.Bounds.height - config.minLeafSize.y;
                }

                if (finalMax <= finalMin) return;

                int splitPoint = node.Bounds.y + Random.Range(finalMin, finalMax);

                IntRect bottom = new IntRect(node.Bounds.x, node.Bounds.y, node.Bounds.width, splitPoint - node.Bounds.y);
                IntRect top = new IntRect(node.Bounds.x, splitPoint, node.Bounds.width, node.Bounds.yMax - splitPoint);

                node.Left = new BspNode(bottom) { Parent = node };
                node.Right = new BspNode(top) { Parent = node };
            }
            else
            {
                // 비율 문제로 가로분할을 원했지만 vertical만 가능하거나 그 반대인 경우
                // 억지로라도 가능한 방향으로 쪼갠다. (빈 영역 차단)
                if (canSplitVertically) { /* vertical 분할 로직 강제 실행 (상위 코드와 동일) */ }
                else if (canSplitHorizontally) { /* horizontal 분할 로직 강제 실행 */ }
                else return;
            }

            SplitRecursive(node.Left, depth + 1);
            SplitRecursive(node.Right, depth + 1);
        }

    }


}
// 구 방식 (분할 영역 제한 없음)
//private void SplitRecursive(BspNode node, int depth)
//{

//    float area = node.Bounds.width * node.Bounds.height;
//    float minArea = config.minLeafSize.x * config.minLeafSize.y * 2.5f;

//    if (depth >= (config.maxDepth))
//        return;

//    bool canSplitHorizontally = 
//        node.Bounds.height >= config.minLeafSize.y * 2;
//    bool canSplitVertically = 
//        node.Bounds.width >= config.minLeafSize.x * 2;

//    if(!canSplitHorizontally && !canSplitVertically)
//        return;

//    bool splitVertical;

//    if (canSplitHorizontally && canSplitVertically)
//    {
//        splitVertical = Random.value > 0.5f;
//    }
//    else
//    {
//        splitVertical = canSplitVertically;
//    }

//    if (splitVertical)
//    {
//        int min = node.Bounds.x + config.minLeafSize.x;
//        int max = node.Bounds.x + node.Bounds.width - config.minLeafSize.x;

//        if (max <= min)
//            return;

//        int splitX = Random.Range(min, max);

//        IntRect left = new IntRect
//            (node.Bounds.x, node.Bounds.y, splitX - node.Bounds.x, node.Bounds.height);
//        IntRect right = new IntRect
//            (splitX, node.Bounds.y, node.Bounds.xMax - splitX, node.Bounds.height);

//        node.Left = new BspNode(left) { Parent = node };
//        node.Right = new BspNode(right) { Parent = node };
//    }
//    else
//    {
//        int min = node.Bounds.y + config.minLeafSize.y;
//        int max = node.Bounds.y + node.Bounds.height - config.minLeafSize.y;

//        if (max <= min)
//            return;

//        int splitY = Random.Range(min, max);

//        IntRect bottom = new IntRect
//            (node.Bounds.x, node.Bounds.y, node.Bounds.width, splitY - node.Bounds.y);
//        IntRect top = new IntRect
//            (node.Bounds.x, splitY, node.Bounds.width, node.Bounds.yMax - splitY);

//        node.Left = new BspNode(bottom) { Parent = node };
//        node.Right = new BspNode(top) { Parent = node };

//    }

//    SplitRecursive(node.Left, depth + 1);
//    SplitRecursive(node.Right, depth + 1);

//}