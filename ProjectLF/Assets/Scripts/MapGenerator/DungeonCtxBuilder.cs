using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDungeonGenrator
{
    public sealed class DungeonCtxBuilder
    {
        public DungeonContext Build(
            DungeonData dungeonData,
            Tilemap floorTilemap,
            Tilemap wallTilemap,
            Tilemap pathTilemap,
            Tilemap doorTilemap,
            Tilemap backgroundTileMap)
        {
            var ctx = new DungeonContext();

            // ctx에 값 넣기
            ctx.MapSize = dungeonData.MapSize;
            ctx.MaxNode = dungeonData.MaxNode;
            ctx.MinNode = dungeonData.MinNode;
            ctx.MaxDivideSize = dungeonData.MaxDivideSize;
            ctx.MinDivideSize = dungeonData.MinDivideSize;
            ctx.DoorHalfwidth = dungeonData.DoorHalfwidth;

            // 렌더링 리소스 넣기
            ctx.FloorTilemap = floorTilemap;
            ctx.WallTilemap = wallTilemap;
            ctx.PathTilemap = pathTilemap;
            ctx.DoorTilemap = doorTilemap;
            ctx.BackgroundTilemap = backgroundTileMap;

            ctx.FloorTile = dungeonData.FloorTile;
            ctx.WallTile = dungeonData.WallTile;
            ctx.DoorTile = dungeonData.DoorTile;
            ctx.PathTile = dungeonData.PathTile;
            ctx.PathTiles = dungeonData.PathTiles;
            ctx.RoomTiles = dungeonData.RoomTiles;
            ctx.BackgroundTile = dungeonData.BackgroundTile;

            // 결과 값 넣기
            ctx.MapData = new TileType[ctx.MapSize.x, ctx.MapSize.y];
            ctx.Rooms = new List<RoomInfo>();
            ctx.RoomStates = new List<RoomRuntimeData>();
            ctx.SplitLines = new List<LineSegment>();
            ctx.EncounterPoints = new List<Vector2Int>();
            ctx.MonsterPoints = new List<Vector2Int>();
            ctx.DoorPositions = new List<Vector2Int>();
            ctx.CorriderCells = new HashSet<Vector2Int>();
            ctx.WallCells = new HashSet<Vector2Int>();

            // Root
            ctx.Root = new TreeNode(0, 0, ctx.MapSize.x, ctx.MapSize.y);

            return ctx;
        }

    }

}

//[Header("Duengeon Data")]
//[SerializeField]
//private DungeonData dungeonData;

//[Header("Wall, FloorTile, Path, Door")]
//// 바닥과 벽을 정의
//[SerializeField]
//private Tilemap floorTilemap;
//[SerializeField]
//private Tilemap wallTilemap;
//[SerializeField]
//private Tilemap pathTilemap;
//[SerializeField]
//private Tilemap doorTilemap;
//[SerializeField]
//private Tilemap openDoorTilemap;

//public DungeonctxBuilder(DungeonContext _ctx)
//{
//    _ctx = _ctx;
//}

//public void GetBuild(DungeonContext _ctx)
//{
//    _ctx = Build();
//}

//private DungeonContext Build()
//{
//    // 데이터 무결성 체크
//    if (dungeonData == null)
//    {
//        //Debug.LogError("[Generator] dungeonData가 할당되지 않았습니다.");
//    }

//    var ctx = new DungeonContext();

//    // 컨텍스트에 값 대입
//    ctx.MapSize = dungeonData.MapSize;
//    ctx.MaxNode = dungeonData.MaxNode;
//    ctx.MinNode = dungeonData.MinNode;
//    ctx.MaxDivideSize = dungeonData.MaxDivideSize;
//    ctx.MinDivideSize = dungeonData.MinDivideSize;

//    ctx.FloorTilemap = floorTilemap;
//    ctx.WallTilemap = wallTilemap;
//    ctx.PathTilemap = pathTilemap;
//    ctx.DoorTilemap = doorTilemap;
//    ctx.OpenDoorTileMap = openDoorTilemap;

//    ctx.FloorTile = dungeonData.FloorTile;
//    ctx.WallTile = dungeonData.WallTile;
//    ctx.DoorTile = dungeonData.DoorTile;
//    ctx.PathTile = dungeonData.PathTile;
//    ctx.OpenDoorTile = dungeonData.OpenDoorTile;
//    ctx.DoorHalfwidth = dungeonData.DoorHalfwidth;

//    ctx.PathTiles = dungeonData.PathTiles;
//    ctx.RoomTiles = dungeonData.RoomTiles;

//    // 맵 데이터는 런타임 배열이므로 dungeonData에서 가져오지 않는다.
//    ctx.MapData = new TileType[ctx.MapSize.x, ctx.MapSize.y];
//    ctx.Rooms = new List<RoomInfo>();
//    ctx.RoomStates = new List<RoomRuntimeData>();
//    ctx.SplitLines = new List<LineSegment>();

//    ctx.EncounterPoints = new List<Vector2Int>();
//    ctx.MonsterPoints = new List<Vector2Int>();

//    // 루트가 될 트리 생성
//    TreeNode rootNode = new TreeNode(0, 0, ctx.MapSize.x, ctx.MapSize.y);
//    ctx.Root = rootNode;
//    //Debug.Log($"[Build] duengeonData.PathTile = {(dungeonData.PathTile == null ? "NULL" : "OK")}");
//    //Debug.Log($"[Build] duengeonData.PathTiles length = {(dungeonData.PathTiles == null ? -1 : dungeonData.PathTiles.Length)}");
//    //InitializedMapNum(ctx);

//    return ctx;
//}
