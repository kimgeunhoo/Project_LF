using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.marker;
using BSPDungeonGenrator.Rendering;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;



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


        [SerializeField]
        private DungeonGeneraterByBSP dungeonGenerator;
        [SerializeField]
        private Tilemap floorTilemap;

        [SerializeField]
        private RoomDistribute roomDistribute;

        private void Start()
        {
            var ctx = dungeonGenerator.Ctx;
            PlayerSpawn(ctx);
            MonsterSpawn(ctx);
        }

        private void PlayerSpawn(DungeonContext ctx)
        {
            Vector2Int psp = roomDistribute.StartSpawnPoint;

            Vector3Int cellPos =
                new Vector3Int(psp.x - ctx.MapSize.x / 2, psp.y - ctx.MapSize.y / 2, 0);

            Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.5f);

            Debug.Log($"[Spawn] cellPos = {cellPos}");
            Debug.Log($"[Spawn] worldPos = {worldPos}");

            Debug.Log($"[Spawn] ctx.StartPoint = {ctx.StartPoint}");
            Debug.Log($"[Spawn] roomDistribute.StartSpawnPoint = {roomDistribute.StartSpawnPoint}");

            user.transform.position = worldPos;
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

                Debug.Log($"[Spawn] cellPos = {cellPos}");
                Debug.Log($"[Spawn] worldPos = {worldPos}");

                Debug.Log($"[Spawn] ctx.StartPoint = {ctx.StartPoint}");
                Debug.Log($"[Spawn] roomDistribute.StartSpawnPoint = {roomDistribute.MonsterSpawnPoint}");

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
