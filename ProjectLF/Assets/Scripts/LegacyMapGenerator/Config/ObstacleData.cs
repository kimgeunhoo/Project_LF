using UnityEngine;

[System.Serializable]
public class ObstacleData : ScriptableObject
{
    public GameObject[] obstaclePrefabs;
    public int minCount = 2;
    public int maxCount = 2;
    public int edgePadding = 1;
    public int doorPadding = 2;
    public int spawnPadding = 2;
}
