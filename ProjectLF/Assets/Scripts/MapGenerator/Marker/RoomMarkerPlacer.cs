using System.Collections.Generic;
using MapGenerator.Core;
using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;

namespace ModularBSP.Marker
{
    public class RoomMarkerPlacer 
    {
        private readonly DungeonConfig config;

        public RoomMarkerPlacer(DungeonConfig config)
        {
            this.config = config;
        }

        public void PlaceMarkers(List<RoomRuntimeData> roomStates, Transform markerParent)
        {
            if (roomStates == null)
                return;

            foreach (var room in roomStates)
            {
                GameObject prefab = GetMarkerPrefab(room.RoomType);
                if (prefab != null)
                    continue;

                Object.Instantiate(
                    prefab,
                    room.CenterWorld,
                    Quaternion.identity,
                    markerParent
                );
            }

        }

        private GameObject GetMarkerPrefab(RoomType roomType)
        {
            switch(roomType)
            {
                case RoomType.Start:
                    return config.roomMarkerSet.StartMarkerPrefab;
                case RoomType.Shop:
                    return config.roomMarkerSet.ShopMarkerPrefab;
                case RoomType.Stairs:
                    return config.roomMarkerSet.StairMarkerPrefab;
                case RoomType.Encounter:
                    return config.roomMarkerSet.EncounterMarkerPrefab;
                case RoomType.Enemy:
                    return config.roomMarkerSet.EnemyMarkerPrefab;

                default:
                    return null;
            }
        }


    }

}