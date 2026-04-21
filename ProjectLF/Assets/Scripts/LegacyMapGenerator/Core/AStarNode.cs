using UnityEngine;

public class AStarNode 
{
    public Vector2Int Pos;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public AStarNode AParent;

    public AStarNode(Vector2Int _pos)
    {
        this.Pos = _pos;
        GCost = 0;
        HCost = 0;
        AParent = null;
    }
}
