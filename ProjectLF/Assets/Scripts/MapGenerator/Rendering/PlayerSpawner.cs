using MapGenerator.Core;
using ModularBSP.Config;
using ModularBSP.Core;
using UnityEngine;


namespace ModularBSP.Rendering
{
    public class PlayerSpawner
    {
        private readonly DungeonConfig config;
        private readonly DungeonContext context;
        private readonly Transform spawnParent;  

        public PlayerSpawner(DungeonConfig config, DungeonContext context, Transform spawnParent)
        {
            this.config = config;
            this.context = context;
            this.spawnParent = spawnParent;
        }


        public GameObject SpawnPlayer()
        {
            if(config.playerPrefab == null)
            {
                Debug.Log("[PlayerSpawner] playerPrefab is null.");
                return null;
            }

            RoomRuntimeData startRoom = FindStartRoom();

            if(startRoom == null)
            {
                Debug.LogError("[PlayerSpawner] startRoom not found.");
                return null;
            }

            Vector3 spawnPos = startRoom.CenterWorld;
            spawnPos.z = 0f;

            GameObject player = Object.Instantiate(
                config.playerPrefab, 
                spawnPos, 
                Quaternion.identity, 
                spawnParent
                );


            return player;
        }

        private RoomRuntimeData FindStartRoom()
        {
            foreach (var room in context.RoomStates)
            {
                if (room.RoomType == RoomType.Start)
                    return room;
            }

            return null;
        }
    }





}
