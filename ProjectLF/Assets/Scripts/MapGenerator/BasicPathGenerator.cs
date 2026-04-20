using System.Collections.Generic;
using BSPDungeonGenrator.Core;
using UnityEngine;

public class BasicPathGenerator : MonoBehaviour
{
    private DungeonContext ctx;

    public List<Vector2Int> DoorCandidates { get; private set; } = new List<Vector2Int>();
}
