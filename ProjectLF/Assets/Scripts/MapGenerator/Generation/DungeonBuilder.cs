using GameScript.Manager;
using MapGenerator.Generation;
using MapGenerator.Marker;
using ModularBSP.Config;
using ModularBSP.Core;
using ModularBSP.Marker;
using ModularBSP.Rendering;
using UnityEngine;
using Unity.Cinemachine;

namespace ModularBSP.Generation
{

    public class DungeonBuilder : MonoBehaviour
    {
        [SerializeField]
        private DungeonConfig config;

        [SerializeField]
        private bool useFixedGridPartition = false;

        [SerializeField]
        private GameObject lineParentObject;

        private DungeonContext context;

        public DungeonContext Context => context;
        public DungeonConfig Config => config;

        private BspLineDrawing lineDrawer;
        private DungeonManager dungeonManager;

        [SerializeField] 
        private Transform emptyParent;
        [SerializeField] 
        private Transform roomParent;
        [SerializeField] 
        private Transform roadParent;
        [SerializeField] 
        private Transform markerParent;
        [SerializeField] 
        private Transform triggerParent;
        [SerializeField] 
        private Transform doorParent;

        [SerializeField]
        private Transform playerParent;

        [SerializeField] 
        private CinemachineCamera cinemachineCamera;

        private void Awake()
        {
            lineDrawer = GetComponent<BspLineDrawing>();
            if (lineDrawer == null)
            {
                Debug.LogError("BspLineDrawing component is missing on the same GameObject.");
            }
        }
        

        private void Start()
        {
            BuildDungeon();
            lineDrawer.DrawLines();
           
        }

        [ContextMenu("Generate Dungeon")]
        public void BuildDungeon()
        {
            if (config == null)
            {
                Debug.LogError("DungeonConfig is not assigned.");
                return;
            }

            ClearChildren(roomParent);
            ClearChildren(roadParent);
            ClearChildren(markerParent);

            TryBuildLayout();

            RoomSlotGenerator roomSlotGenerator = new RoomSlotGenerator(config, context);
            roomSlotGenerator.Generate(context.Root);

            CorridorGenerator corridorGenerator = new CorridorGenerator(config, context);
            corridorGenerator.Run(context.Root);

            RoomTypeDistributor typeDistributor = new RoomTypeDistributor();
            context.RoomStates = typeDistributor.BuildRoomStates(context.Rooms, config.cellSize);

            PrefabPlacer placer = new PrefabPlacer(config, context, roomParent, roadParent, emptyParent);
            placer.PlaceAll();

            DoorPlacer doorPlacer = new DoorPlacer(config, context, doorParent);
            doorPlacer.PlaceDoor();

            RoomMarkerPlacer markerPlacer = new RoomMarkerPlacer(config);
            markerPlacer.PlaceMarkers(context.RoomStates, markerParent);

            RoomTriggerPlacer triggerPlacer = new RoomTriggerPlacer(config, dungeonManager);
            triggerPlacer.PlaceTriggers(context.RoomStates, markerParent);

            PlayerSpawner playerSpawner = new PlayerSpawner(config, context, playerParent);
            GameObject player = playerSpawner.SpawnPlayer();

            lineParentObject.SetActive(false);

            if (player != null && cinemachineCamera != null)
            {
                cinemachineCamera.Follow = player.transform;
                cinemachineCamera.LookAt = player.transform;
            }
        }
        private bool TryBuildLayout()
        {
            context = new DungeonContext
                (config.cellSize,
                new Vector2Int(config.mapWidthInCells, config.mapHeightInCells)
                );

            if (useFixedGridPartition)
            {
                FixedGridPartitioner partitioner = new FixedGridPartitioner(config);
                context.Root = partitioner.CreateTree();
            }
            else
            {
                BspPartitioner partitioner = new BspPartitioner(config);
                context.Root = partitioner.CreateTree();
            }

            return context.Rooms.Count >= config.minRoomCount;
        }


        private void ClearChildren(Transform parent)
        {
            if (parent == null) return;

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(parent.GetChild(i).gameObject);
#else
                Destroy(parent.GetChild(i).gameObject);
#endif
            }
        }
    }
}
