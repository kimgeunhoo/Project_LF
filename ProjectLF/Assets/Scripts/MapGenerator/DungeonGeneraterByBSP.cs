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
using System;

namespace BSPDungeonGenrator
{

    public class DungeonGeneraterByBSP : MonoBehaviour
    {
        [Header("Duengeon Data")]
        [SerializeField] 
        private DungeonData dungeonData;

        [Header("Wall, FloorTile, Path, Door, background")]
        [SerializeField] 
        private Tilemap floorTilemap;
        [SerializeField] 
        private Tilemap wallTilemap;
        [SerializeField] 
        private Tilemap pathTilemap;
        [SerializeField]
        private Tilemap doorTilemap;
        [SerializeField]
        private Tilemap backgroundTilemap;

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
        private readonly MapDataPainter mapDataPainter;

        private DungeonCtxBuilder contextBuilder;
        private DungeonRoomStateInitializer roomStateInitializer;

        private DoorGenerator doorGenerator = new DoorGenerator();
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

            ctx = contextBuilder.Build(dungeonData, floorTilemap, wallTilemap, pathTilemap, doorTilemap, backgroundTilemap);

            // split, debug draw, room generator
            RunGenerationPipeline();
            // roomdistribute, roomstate init
            RunRoomLayoutPipeline();
            // path, wall, optional point
            RunPathAndWallPipeline();
            // door, render, marker, spawn
            RunDoorPipeline();

       

            lineHolderPF.SetActive(false);
        }

        private void CacheComponents()
        {
            bspDrawer = GetComponent<BspDrawer>();
            tileMapRenderer = GetComponent<TileMapRenderer>();
            roomDistribute = GetComponent<RoomDistribute>();
            roomMarkerRenderer = GetComponent<RoomMarkerRenderer>();
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
            bspSplitter.Run(ctx);
            if (bspDrawer != null)
            {
                bspDrawer.OnDrawRectangle(ctx, dungeonData, lineHolder);
                bspDrawer.OnDrawLine(ctx, dungeonData, lineHolder);
            }

            roomGenerater.Run(ctx);
        }

        private void RunPathAndWallPipeline()
        {

            pathGenerator.Run(ctx);
           
            if (mapDataPainter != null)
            {
                mapDataPainter.Run(ctx);
            }
            wallGenerator.Run(ctx);

            int wallCount = 0;
            for (int x = 0; x < ctx.MapSize.x; x++)
            {
                for (int y = 0; y < ctx.MapSize.y; y++)
                {
                    if (ctx.MapData[x, y] == TileType.Wall)
                        wallCount++;
                }
            }
           // Debug.Log($"[WallGenerator] wallCount = {wallCount}");
        }

        private void RunRoomLayoutPipeline()
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
                PathTile = ctx.PathTile,
                RoomTiles = ctx.RoomTiles,

            };
        }

    }

}