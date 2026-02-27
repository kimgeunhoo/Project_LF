using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;

public class CollectLeafRoom : MonoBehaviour
{
    // 노드 값이 라인의 갯수를 판별
    [Header("Node Value")]
    [SerializeField]
    private int maxNode;
    [SerializeField]
    private int minNode;

    DuengeonContext context;

    // 리프 방 수집
    private void CollectLeafRooms(TreeNode node, int depth, List<RoomInfo> rooms)
    {
        if (node == null)
            return;

        if (depth == maxNode)
        {
            rooms.Add(new RoomInfo(node.dungeonSize));
            return;
        }

        CollectLeafRooms(node.leftTree, depth + 1, rooms);
        CollectLeafRooms(node.rightTree, depth + 1, rooms);
    }
}
