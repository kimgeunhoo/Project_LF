using BSPDungeonGenrator.Core;
using UnityEngine;

public class BspRoomFinder
{
    public static RectInt GetLeafRoom(TreeNode node)
    {
        if (node.leftTree == null && node.rightTree == null)
            return node.dungeonSize;
        

        if(node.leftTree != null)
            return GetLeafRoom(node.leftTree);

        return GetLeafRoom(node.rightTree);
    }

    public static RectInt GetRightLeafRoom(TreeNode node)
    {
        if (node.leftTree == null && node.rightTree == null)
            return node.dungeonSize;

        if (node.rightTree != null)
            return GetLeafRoom(node.rightTree);

        return GetLeafRoom(node.leftTree);
    }
}
