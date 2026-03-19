using UnityEngine;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.marker;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Rendering;
using UnityEngine.Tilemaps;



namespace BSPDungeonGenrator.Generation
{
    public class DungeonManager : MonoBehaviour
    {
        [Header("User Object")]
        [SerializeField]
        private GameObject user;

        [SerializeField]
        private DungeonGeneraterByBSP dungeonGenerator;
        [SerializeField]
        private Tilemap floorTilemap;


        private void Start()
        {
            var ctx = dungeonGenerator.Ctx;

            Debug.Log($"[Spawn] ctx.SpawnPoint = {ctx.StartPoint}");
            Debug.Log($"[Spawn] floorTilemap.transform.position = {floorTilemap.transform.position}");
            Debug.Log($"[Spawn] floorTilemap.layoutGrid.transform.position = {floorTilemap.layoutGrid.transform.position}");
            Debug.Log($"[Spawn] floorTilemap.cellBounds = {floorTilemap.cellBounds}");

            Vector3Int cellPos = new Vector3Int(ctx.StartPoint.x, ctx.StartPoint.y, 0);
            Vector3 worldPos = floorTilemap.GetCellCenterWorld(cellPos);
            Debug.Log($"[Spawn] ctx.SpawnPoint {ctx.StartPoint}");
            Debug.Log($"[Spawn] worldPos {worldPos}");
            Debug.Log($"[Spawn] user before {user.transform.position}");

            user.transform.position = cellPos;
                
        }


    }

}
