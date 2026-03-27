using BSPDungeonGenrator.Generation;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.marker;
using BSPDungeonGenrator.Rendering;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BSPDungeonGenrator
{

    public class DungeonGeneraterByBSP : MonoBehaviour
    {
        [Header("Duengeon Data")]
        [SerializeField]
        private DungeonData dungeonData;

        [Header("Wall, FloorTile, Path, Door")]
        // 바닥과 벽을 정의
        [SerializeField]
        private Tilemap floorTilemap;
        [SerializeField]
        private Tilemap wallTilemap;
        [SerializeField]
        private Tilemap pathTilemap;
        [SerializeField]
        private Tilemap doorTilemap;
        [SerializeField]
        private Tilemap openDoorTilemap;

        [Header("Line Holder")]
        [SerializeField]
        private Transform lineHolder;


        // 컨텍스트 정의
        private static DungeonContext ctx = new DungeonContext();

        // 방 생성에 사용할 생성 클래스 정의
        private BspSplitter bspSplitter = new BspSplitter();
        private RoomGenerater roomGenerater = new RoomGenerater();
        private WallGenerator wallGenerator = new WallGenerator();
        private PathGenerator pathGenerator = new PathGenerator();
        private DoorGenerator doorGenerator = new DoorGenerator();
        private RoomDef roomDef = new RoomDef();
        private BspDrawer bspDrawer;
        private TileMapRenderer tileMapRenderer;


        // 방 배분에 사용할 생성 클래스 정의
        private RoomDistribute roomDistribute;



        // 스폰포인트 넘겨주기
        public DungeonContext Ctx { get { return ctx; } }

        private void Awake()
        {
            // 데이터 무결성 체크
            if(dungeonData == null)
            {
                //Debug.LogError("[Generator] dungeonData가 할당되지 않았습니다.");
            }
            
            // ctx 정의
            ctx = Build();
            // 벽과 관련된 데이터 배열 생성
            InitializeMap();
            // 던전 사이즈에 맞게 벽을 그림
            bspDrawer = GetComponent<BspDrawer>();
            bspDrawer.OnDrawRectangle(ctx, dungeonData, lineHolder);

            // 트리 분할 메서드            
            // bspSplitter = GetComponent<BspSplitter>();
            // Debug.Log($"[Generator] ctx id = {RuntimeHelpers.GetHashCode(ctx)}");
            bspSplitter.Run(ctx);

            bspDrawer = GetComponent<BspDrawer>();
            // Splitter로 값 가져오고 라인 그리기
            bspDrawer.OnDrawLine(ctx, dungeonData, lineHolder);

            // 방 생성
            roomGenerater.Run(ctx);
            // 길 연결
            pathGenerator.Run(ctx);
            // 벽 생성 범위 체크 
            wallGenerator.Run(ctx);
            // 문 생성
            doorGenerator.Run(ctx);
            // 최종 타일맵 생성
            tileMapRenderer = GetComponent<TileMapRenderer>();
            tileMapRenderer.Run(ctx);

            // 방 정의, 마커 생성
            roomDistribute = GetComponent<RoomDistribute>();
            roomDistribute.Run(ctx);

            //Debug.Log($"[Awake] spawnPoint null 여부: {spawnPoint == null}");
            //Debug.Log($"[Awake] ctx.Rooms null 여부: {ctx.Rooms == null}");
            //Debug.Log($"[Awake] ctx.EncounterPoints null 여부: {ctx.EncounterPoints == null}");
            //Debug.Log($"[Awake] ctx.MonsterPoints null 여부: {ctx.MonsterPoints == null}");

            // 정의한 방 ctx에 집어넣기
            roomDef.Run(ctx);
        }

        private DungeonContext Build()
        {
            var ctx = new DungeonContext();

            // 컨텍스트에 값 대입
            ctx.MapSize = dungeonData.MapSize;
            ctx.MaxNode = dungeonData.MaxNode;
            ctx.MinNode = dungeonData.MinNode;
            ctx.MaxDivideSize = dungeonData.MaxDivideSize;
            ctx.MinDivideSize = dungeonData.MinDivideSize;

            ctx.FloorTilemap = floorTilemap;
            ctx.WallTilemap = wallTilemap;
            ctx.PathTilemap = pathTilemap;
            ctx.DoorTilemap = doorTilemap;
            ctx.OpenDoorTileMap = openDoorTilemap;

            ctx.FloorTile = dungeonData.FloorTile;
            ctx.WallTile = dungeonData.WallTile;
            ctx.DoorTile = dungeonData.DoorTile;
            ctx.PathTile = dungeonData.PathTile;
            ctx.OpenDoorTile = dungeonData.OpenDoorTile;
            ctx.DoorHalfwidth = dungeonData.DoorHalfwidth;

            ctx.PathTiles = dungeonData.PathTiles;
            ctx.RoomTiles = dungeonData.RoomTiles;

            ctx.Rooms = new List<RoomInfo>();

            


            ctx.SplitLines = new List<LineSegment>();
            // 맵 데이터는 런타임 배열이므로 dungeonData에서 가져오지 않는다.
            ctx.MapData = new TileType[ctx.MapSize.x, ctx.MapSize.y];
            // 루트가 될 트리 생성
            TreeNode rootNode = new TreeNode(0, 0, ctx.MapSize.x, ctx.MapSize.y);
            ctx.Root = rootNode;
            //Debug.Log($"[Build] duengeonData.PathTile = {(dungeonData.PathTile == null ? "NULL" : "OK")}");
            //Debug.Log($"[Build] duengeonData.PathTiles length = {(dungeonData.PathTiles == null ? -1 : dungeonData.PathTiles.Length)}");
            InitializedMapNum(ctx);

            return ctx;
        }

        // 벽 함수 초기화
        private void InitializeMap()
        {
            ctx.MapData = new TileType[ctx.MapSize.x, ctx.MapSize.y];
        }

        // 방 번호 배정
        private void InitializedMapNum(DungeonContext ctx)
        {
            for (int i = 0; i < ctx.Rooms.Count; i++)
            {
                RoomRuntimeData roomstate = new RoomRuntimeData();
                roomstate.RoomId = i;
                roomstate.RoomInfo = ctx.Rooms[i];
                roomstate.AliveMonsterCount = 0;

                ctx.RoomStates.Add(roomstate);
            }

        }
       
        
        


    }

}