using UnityEngine;



public class MonsterSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;

    private void Awake()
    {
        spawnPoint = GetComponent<Transform>();   
    }
}
