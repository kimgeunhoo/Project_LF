using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using BSPDuengeonGenrator.Core;
using BSPDuengeonGenrator.Config;
using BSPDuengeonGenrator.Generation;
using BSPDuengeonGenrator.Rendering;
using BSPDuengeonGenrator.marker;

namespace BSPDuengeonGenrator
{

    public class DuengeonGeneraterByBSP : MonoBehaviour
    {
        [Header("Duengeon Data")]
        [SerializeField]
        private DuengeonData duengeonData;

        [Header("Wall, FloorTile, Door")]
        // 바닥과 벽을 정의
        [SerializeField]
        private Tilemap floorTilemap;
        [SerializeField]
        private Tilemap wallTilemap;
        [SerializeField]
        private Tilemap doorTilemap;

        [Header("Line Holder")]
        [SerializeField]
        private Transform lineHolder;

        [Header("SpawnPoint")]
        [SerializeField]
        private Vector3 spawnPoint;

        // 컨텍스트 정의
        private static DuengeonContext ctx = new DuengeonContext();

        // 방 생성에 사용할 생성 클래스 정의
        private BspSplitter bspSplitter;
        private RoomGenerater roomGenerater;
        private WallGenerator wallGenerator;
        private RoadGenerator roadGenerator;
        private DoorGenerator doorGenerator;
        private BspDrawer bspDrawer;
        private TileMapRenderer tileMapRenderer;

        // 방 배분에 사용할 생성 클래스 정의
        private RoomDistribute roomDistribute;

        private void Awake()
        {
            // 데이터 무결성 체크
            if(duengeonData == null)
            {
                Debug.LogError("[Generator] duengeonData가 할당되지 않았습니다.");
            }
            
            // ctx 정의
            ctx = Build();
            // 벽과 관련된 데이터 배열 생성
            InitializeMap();
            // 던전 사이즈에 맞게 벽을 그림
            bspDrawer = GetComponent<BspDrawer>();
            bspDrawer.OnDrawRectangle(ctx, duengeonData, lineHolder);

            // 트리 분할 메서드            
            bspSplitter = GetComponent<BspSplitter>();
            bspSplitter.Run(ctx);

            bspDrawer = GetComponent<BspDrawer>();
            //Splitter로 값 가져오고 라인 그리기
            bspDrawer.OnDrawLine(ctx, duengeonData, lineHolder);

            // 방 생성
            roomGenerater = GetComponent<RoomGenerater>();
            roomGenerater.Run(ctx);
            // 길 연결
            roadGenerator = GetComponent<RoadGenerator>();
            roadGenerator.Run(ctx);
            // 문 생성
            doorGenerator = GetComponent<DoorGenerator>();
            doorGenerator.Run(ctx);
            // 벽 생성 범위 체크
            wallGenerator = GetComponent<WallGenerator>();
            wallGenerator.Run(ctx);
            // 최종 타일맵 생성
            tileMapRenderer = GetComponent<TileMapRenderer>();
            tileMapRenderer.Run(ctx);

            roomDistribute = GetComponent<RoomDistribute>();
            roomDistribute.Run(ctx);
        }

        private DuengeonContext Build()
        {
            var ctx = new DuengeonContext();

            // 컨텍스트에 값 대입
            ctx.MapSize = duengeonData.MapSize;
            ctx.MaxNode = duengeonData.MaxNode;
            ctx.MinNode = duengeonData.MinNode;
            ctx.MaxDivideSize = duengeonData.MaxDivideSize;
            ctx.MinDivideSize = duengeonData.MinDivideSize;

            ctx.FloorTilemap = floorTilemap;
            ctx.WallTilemap = wallTilemap;
            ctx.DoorTilemap = doorTilemap;

            ctx.FloorTile = duengeonData.FloorTile;
            ctx.WallTile = duengeonData.WallTile;
            ctx.DoorTile = duengeonData.DoorTile;
            ctx.PathTiles = duengeonData.PathTiles;
            ctx.DoorHalfwidth = duengeonData.DoorHalfwidth;

            ctx.Rooms = new List<RoomInfo>();

            // 맵 데이터는 런타임 배열이므로 dungeonData에서 가져오지 않는다.
            ctx.MapData = new TileType[ctx.MapSize.x, ctx.MapSize.y];
            // 루트가 될 트리 생성
            TreeNode rootNode = new TreeNode(0, 0, ctx.MapSize.x, ctx.MapSize.y);
            ctx.Root = rootNode;

            return ctx;
        }

        // 벽 함수 초기화
        private void InitializeMap()
        {
            ctx.MapData = new TileType[ctx.MapSize.x, ctx.MapSize.y];
        }


    }

}