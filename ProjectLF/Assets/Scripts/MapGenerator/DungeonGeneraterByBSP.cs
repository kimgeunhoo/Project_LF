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
        [SerializeField]
        private GameObject lineHolderPF;

        private DungeonContext ctx;

        private readonly BspSplitter bspSplitter = new BspSplitter();
        private readonly RoomGenerater roomGenerater = new RoomGenerater();
        private readonly WallGenerator wallGenerator = new WallGenerator();
        private readonly PathGenerator pathGenerator = new PathGenerator();
        private readonly MapDataPainter mapDataPainter = new MapDataPainter();

        private DungeonCtxBuilder contextBuilder;
        private DungeonRoomStateInitializer roomStateInitializer;

        private DoorGenerator doorGenerator;
        private DoorSpawner doorSpawner;
        private BspDrawer bspDrawer;
        private TileMapRenderer tileMapRenderer;
        private RoomDistribute roomDistribute;
        private RoomMarkerRenderer roomMarkerRenderer;

        // 스폰포인트 넘겨주기
        public DungeonContext Ctx { get { return ctx; } }

        private void Awake()
        {
            CacheComponents();
            ValidateRefernces();

            contextBuilder = new DungeonCtxBuilder();
            roomStateInitializer = new DungeonRoomStateInitializer();

            ctx = contextBuilder.Build(dungeonData, floorTilemap, wallTilemap, pathTilemap, doorTilemap, openDoorTilemap);

            RunGenerationPipeline();
            RunRoomLayoutPipeLine();
            RunDoorPipeline();

            if(lineHolderPF != null)
            {
                lineHolderPF.SetActive(true);
            }

        }

        private void CacheComponents()
        {
            bspDrawer = GetComponent<BspDrawer>();
            tileMapRenderer = GetComponent<TileMapRenderer>();
            roomDistribute = GetComponent<RoomDistribute>();
            roomMarkerRenderer = GetComponent<RoomMarkerRenderer>();
            doorGenerator = GetComponent<DoorGenerator>();
            doorSpawner = GetComponent<DoorSpawner>();
        }

        private void ValidateRefernces()
        {
            if (dungeonData == null)
            {
                Debug.LogError("dungeonData가 할당되지 않았습니다.");
            }
        }

        private void RunGenerationPipeline()
        {
            bspDrawer.DrawRectangle(ctx.MapSize, lineHolder);

            bspSplitter.Run(ctx);
            bspDrawer.DrawLine(ctx.SplitLines, ctx.MapSize, lineHolder);

            roomGenerater.Run(ctx);
            pathGenerator.Run(ctx);
            wallGenerator.Run(ctx);
            mapDataPainter.Run(ctx);
        }

        private void RunRoomLayoutPipeLine()
        {
            roomDistribute.Run(ctx);
            roomStateInitializer.Initialize(ctx, roomDistribute);
        }

        // 생성 메서드
        private void RunDoorPipeline()
        {
            doorGenerator.Run(ctx);
            tileMapRenderer.Render(
                ctx.MapData,
                ctx.MapSize,
                BuildRenderRefs(),
                BuildAssetRefs()
                );

            roomMarkerRenderer.Run(ctx);
            doorSpawner.Run(ctx);
        }

        private TilemapRenderRefs BuildRenderRefs()
        {
            return new TilemapRenderRefs
            {
                FloorTilemap = floorTilemap,
                WallTilemap = wallTilemap,
                DoorTilemap = doorTilemap,
                OpenDoorTileMap = openDoorTilemap,
                PathTilemap = pathTilemap,
            };
        }


        private TileAssetRefs BuildAssetRefs()
        {
            return new TileAssetRefs
            {
                FloorTile = ctx.FloorTile,
                WallTile = ctx.WallTile,
                DoorTile = ctx.DoorTile,
                OpenDoorTile = ctx.OpenDoorTile,
                PathTile = ctx.PathTile,
                RoomTiles = ctx.RoomTiles,

            };
        }

    }

}