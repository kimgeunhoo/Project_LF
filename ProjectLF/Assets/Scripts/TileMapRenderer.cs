using UnityEngine;
using UnityEngine.Tilemaps;
using static DuengeonData;

public class TileMapRenderer : MonoBehaviour
{
    [Header("Wall, FloorTile, Door")]
    // 바닥과 벽을 정의
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    private TileBase wallTile;
    [SerializeField]
    private TileBase doorTile;

    [SerializeField]
    private TileType[,] mapData;

}
