using ModularBSP.Config;
using ModularBSP.Core;
using ModularBSP.Rendering;
using UnityEngine;

namespace ModularBSP.Generation
{

    public class DungeonBuilder : MonoBehaviour
    {
        [SerializeField]
        private DungeonConfig config;

        [SerializeField]
        private bool useFixedGridPartition = false;

        private DungeonContext context;

        public DungeonContext Context => context;
        public DungeonConfig Config => config;

        private BspLineDrawing lineDrawer;

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

            ClearChildren(config.roomParent);
            ClearChildren(config.roadParent);

            context = new DungeonContext
                (config.cellSize,
                new Vector2Int(config.mapWidthInCells, config.mapHeightInCells)
                );
            //BspPartitioner partitioner = new BspPartitioner(config);
            //context.Root = partitioner.CreateTree();

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

            RoomSlotGenerator roomSlotGenerator = new RoomSlotGenerator(config, context);
            roomSlotGenerator.Generate(context.Root);

            CorridorGenerator corridorGenerator = new CorridorGenerator(config, context);
            corridorGenerator.Run(context.Root);

            PrefabPlacer placer = new PrefabPlacer(config, context);
            placer.PlaceAll();
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
