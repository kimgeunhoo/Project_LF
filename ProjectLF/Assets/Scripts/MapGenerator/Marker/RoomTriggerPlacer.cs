using System.Collections.Generic;
using GameScript.Manager;
using MapGenerator.Core;
using ModularBSP.Config;
using ModularBSP.Core;
using ModularBSP.Trigger;
using UnityEngine;


namespace MapGenerator.Marker
{
    public class RoomTriggerPlacer
    {
        private readonly DungeonConfig config;
        private readonly DungeonManager dungeonManager;


        public RoomTriggerPlacer(DungeonConfig config, DungeonManager dungeonManager)
        {
            this.config = config;
            this.dungeonManager = dungeonManager;
        }

        public void PlaceTriggers(List<RoomRuntimeData> roomStates, Transform parents)
        {
            if (roomStates == null)
                return;

            foreach (var room in roomStates)
            {
                CreateTrigger(room, parents);
            }
        }

        private void CreateTrigger(RoomRuntimeData room, Transform parent)
        {
            GameObject obj = new GameObject($"RoomTrigger_{room.RoomId}_{room.RoomType}");
            obj.transform.SetParent(parent);
            obj.transform.position = room.CenterWorld;

            BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            float width = room.RoomRect.width * config.cellSize;
            float height = room.RoomRect.height * config.cellSize;

            float basePadding = 3f;
            float topExPadding = 1f;

            float finalWidth = width - basePadding;
            float finalHeight = height - basePadding - 2f;

            float offsetY = -topExPadding;

            col.size = new Vector2(finalWidth, finalHeight);
            col.offset = new Vector2(0f, offsetY);

            RoomTrigger trigger = obj.AddComponent<RoomTrigger>();
            trigger.Init(room.RoomId, room.RoomType, dungeonManager);

            GameObject markerPF = GetMarkerPF(room.RoomType);

            if(markerPF != null)
            {
                GameObject marker = Object.Instantiate(
                    markerPF,
                    room.CenterWorld,
                    Quaternion.identity,
                    obj.transform 
                );
           
                marker.transform.localPosition = new Vector3(0, 0, -1f);
            }

        }

        private GameObject GetMarkerPF(RoomType roomType)
        {
            switch (roomType) 
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

            }
            return null;

        }


    }

    
}
