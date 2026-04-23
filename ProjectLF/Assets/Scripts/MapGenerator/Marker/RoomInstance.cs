using UnityEngine;
using System.Collections.Generic;
using ModularBSP.Generation;
using System;

namespace ModularBSP.Marker
{ 
    public class RoomInstance : MonoBehaviour
    {
        [Header("Door Marker")]
        [SerializeField]
        private Transform doorUpMarker;
        [SerializeField]
        private Transform doorDownMarker;
        [SerializeField]
        private Transform doorLeftMarker;
        [SerializeField]
        private Transform doorRightMarker;

        [Header("Block PF")]
        [SerializeField]
        private GameObject blockPF;

        private readonly List<GameObject> spawnedBlocks = new List<GameObject>();

        public void SetupBlockedDoors(HashSet<DoorDir> connectedDirs)
        {
            ClearBlocks();

            TryPlaceBlock(DoorDir.Up, doorUpMarker, connectedDirs);
            TryPlaceBlock(DoorDir.Down, doorDownMarker, connectedDirs);
            TryPlaceBlock(DoorDir.Left, doorLeftMarker, connectedDirs);
            TryPlaceBlock(DoorDir.Right, doorRightMarker, connectedDirs);
        }

        private void TryPlaceBlock(DoorDir dir, Transform marker, HashSet<DoorDir> connectedDirs)
        {
            if (marker == null || blockPF == null)
                return;

            if (connectedDirs.Contains(dir))
                return;

            GameObject obj = Instantiate(blockPF, marker.position, marker.rotation, transform);
            spawnedBlocks.Add(obj);
        }

        private void ClearBlocks()
        {
            for (int i = spawnedBlocks.Count - 1; i >= 0; i--)
            {
                if (spawnedBlocks[i] != null)
                    Destroy(spawnedBlocks[i]);
            }
            spawnedBlocks.Clear();
        }

    }
}
