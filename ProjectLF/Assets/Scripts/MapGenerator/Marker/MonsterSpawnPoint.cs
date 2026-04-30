using UnityEngine;



public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;
}
