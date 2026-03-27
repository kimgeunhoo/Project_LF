using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.marker;
using BSPDungeonGenrator.Rendering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;



namespace BSPDungeonGenrator.Generation
{
    public class DungeonManager : MonoBehaviour
    {
        [Header("User Object")]
        [SerializeField]
        private GameObject user;

        [Header("Monster Object")]
        [SerializeField] 
        private GameObject[] monster;
        [SerializeField]
        private int monsterCount = 7;
        [SerializeField]
        private Transform monsterHolder;


        [Header("Map Generator")]
        [SerializeField]
        private DungeonGeneraterByBSP dungeonGenerator;
        [SerializeField]
        private Tilemap floorTilemap;

        [SerializeField]
        private RoomDistribute roomDistribute;



        private void Start()
        {
            var ctx = dungeonGenerator.Ctx;
            GameObject playerObj = Instantiate(user);
            PlayerSpawn(ctx, playerObj);

            var cam = FindFirstObjectByType<CinemachineCamera>();
            cam.Follow = playerObj.transform;
            cam.LookAt = playerObj.transform;
            MonsterSpawn(ctx);
        }

        private void PlayerSpawn(DungeonContext _ctx, GameObject _playerObj)
        {
            Vector2Int psp = roomDistribute.StartSpawnPoint;

            Vector3Int cellPos = new Vector3Int(psp.x - _ctx.MapSize.x / 2, psp.y - _ctx.MapSize.y / 2, 0);

            Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0);

            _playerObj.transform.position = worldPos;

        }
        private void MonsterSpawn(DungeonContext ctx)
        {
            List<Vector2Int> msp = new List<Vector2Int>();
            msp = roomDistribute.MonsterSpawnPoint;

            foreach (var spawnP in msp)
            {
                Vector3Int cellPos =
                  new Vector3Int(spawnP.x - ctx.MapSize.x / 2, spawnP.y - ctx.MapSize.y / 2, 0);

                Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.5f);

                for (int j = 0; j < monsterCount; j++)
                {
                    Vector3 offset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);

                    Instantiate(monster[0], worldPos + offset, Quaternion.identity, monsterHolder);
                }
            }
        }

        private void ShopSpawn()
        {

        }
        private void EncounterSpawn()
        {

        }
    }
}
