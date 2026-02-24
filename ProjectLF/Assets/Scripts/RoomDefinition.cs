using BSPDuengeonGenrator;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BSPDuengeonGenrator.DuengeonGeneraterByBSP;

public class RoomDefinition : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField]
    private Vector2Int mapSize;


    [Header("Room Markers (Debug)")]
    [SerializeField]
    private Transform markerHolder;
    [SerializeField]
    private GameObject startMarkerPrefab;
    [SerializeField]
    private GameObject stairMarkerPrefab;
    [SerializeField]
    private GameObject shopMarkerPrefab;
    [SerializeField]
    private GameObject encounterMarkerPrefab;
    [SerializeField]
    private GameObject MonsterMarkerPrefab;

    [Header("Wall, FloorTile, Door")]
    // 바닥과 벽을 정의
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private Tilemap wallTilemap;

    private void SpawnRoomMarkers(List<RoomInfo> rooms)
    {
        foreach (var room in rooms)
        {
            GameObject prefab = room.type switch
            {
                RoomType.Start => startMarkerPrefab,
                RoomType.Stairs => stairMarkerPrefab,
                RoomType.Shop => shopMarkerPrefab,
                RoomType.Encounter => encounterMarkerPrefab,
                RoomType.Monster => MonsterMarkerPrefab,
                // 이부분은 throw Exception 써도될듯?
                _ => null
            };

            if (prefab == null) continue;

            Vector2Int c = room.Center;
            Vector3Int cellPos = new Vector3Int(c.x - mapSize.x / 2, c.y - mapSize.y / 2, 0);

            Vector3 worldPos = floorTilemap.CellToWorld(cellPos) + new Vector3(0.5f, 0.5f, 0.5f);
            Instantiate(prefab, worldPos, Quaternion.identity, markerHolder);
        }

    }

   
    
   

}
