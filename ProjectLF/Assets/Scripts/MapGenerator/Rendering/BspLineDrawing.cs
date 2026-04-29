using System;
using System.Collections.Generic;
using ModularBSP.Config;
using ModularBSP.Core;
using ModularBSP.Generation;
using ModularBSP.Rendering;
using UnityEngine;

namespace ModularBSP.Rendering
{
    public class BspLineDrawing : MonoBehaviour
    {
        [SerializeField]
        private DungeonBuilder dungeonBuilder;
        [SerializeField]
        private Transform lineParent;
        [SerializeField]
        private Material lineMaterial;
        [SerializeField]
        private float lineWidth = 0.1f;
        [SerializeField]
        private float zOffset = -0.1f;

        [Header("Draw Option")]
        [SerializeField]
        private bool drawAllNodes = true;
        [SerializeField]
        private bool drawLeafOnly = false;
        [SerializeField]
        private bool drawRoomBounds = true;

        private readonly List<GameObject> spawnedLines
            = new List<GameObject>();

        [ContextMenu("Draw BSP Lines")]
        public void DrawLines()
        {
            ClearLines();
            if (dungeonBuilder == null)
            {
                Debug.LogError("BspLineDrawer: DungeonBuilder is not assigned.");
                return;
            }

            if (dungeonBuilder.Context == null || dungeonBuilder.Context.Root == null)
            {
                Debug.LogError("BspLineDrawer: DungeonBuilder Context or Root is null. Build first.");
                return;
            }

            DrawNodeRecursive(dungeonBuilder.Context.Root);

            Debug.Log($"Draw Complete line count = {spawnedLines.Count}");
        }

        [ContextMenu("Clear BSP Lines")]
        public void ClearLines()
        {
            for (int i = spawnedLines.Count - 1; i >= 0; i--)
            {
                if (spawnedLines[i] != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(spawnedLines[i]);
#else
                    Destroy(spawnedLines[i]);
#endif
                }
            }
            spawnedLines.Clear();

            if (lineParent != null)
            {
                for (int i = lineParent.childCount - 1; i >= 0; i--)
                {
#if UNITY_EDITOR
                    DestroyImmediate(lineParent.GetChild(i).gameObject);
#else
                    Destroy(lineParent.GetChild(i).gameObject);
#endif
                }
            }
        }
        private void DrawNodeRecursive(BspNode node)
        {
            if (node == null)
                return;

            bool shouldDrawBounds = false;

            if(drawAllNodes)
            {
                shouldDrawBounds = true;
            }
            else if (drawLeafOnly && node.IsLeaf)
            {
                shouldDrawBounds = true;
            }

            if (shouldDrawBounds)
            {
                DrawRect(node.Bounds, $"BSP_{node.Bounds.x}_{node.Bounds.y}");
            }

            if (drawRoomBounds && node.RoomBounds.HasValue)
            {
                DrawRect(node.RoomBounds.Value, $"ROOM_{node.RoomBounds.Value.x}_{node.RoomBounds.Value.y}", true);
            }

            DrawNodeRecursive(node.Left);
            DrawNodeRecursive(node.Right);
        }

        private void DrawRect(IntRect rect, string name, bool isRoom = false)
        {
            Vector3 bl = GridToWorld(rect.x, rect.y);
            Vector3 br = GridToWorld(rect.x + rect.width, rect.y);
            Vector3 tr = GridToWorld(rect.x + rect.width, rect.y + rect.height);
            Vector3 tl = GridToWorld(rect.x, rect.y + rect.height);

            GameObject go = new GameObject(name);
            if (lineParent != null)
            {
                go.transform.SetParent(lineParent, false);
            }
            LineRenderer lr = go.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = isRoom ? lineWidth * 1.4f : lineWidth;
            lr.positionCount = 5;
            lr.useWorldSpace = true;
            lr.loop = false;

            lr.SetPosition(0, bl);
            lr.SetPosition(1, br);
            lr.SetPosition(2, tr);
            lr.SetPosition(3, tl);
            lr.SetPosition(4, bl); // Close the loop

            spawnedLines.Add(go);
            
        }

        private Vector3 GridToWorld(int cellX, int cellY)
        {
            int cellSize = dungeonBuilder.Config.cellSize;
            return new Vector3(cellX * cellSize, cellY * cellSize, zOffset);
        }

    }
}
