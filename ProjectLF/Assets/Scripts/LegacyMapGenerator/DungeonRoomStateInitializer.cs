using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.marker;
using System.Collections.Generic;
using UnityEngine;

namespace BSPDungeonGenrator
{
    public sealed class DungeonRoomStateInitializer
    {
        public void Initialize(OldDungeonContext ctx, RoomDistribute roomDistribute)
        {
            BuildRoomStates(ctx);
            AssignSpawnPointsToRooms(ctx, roomDistribute.MonsterSpawnPoint);
        }

        // 방 상태 저장
        private void BuildRoomStates(OldDungeonContext ctx)
        {
            //ctx.RoomStates = new List<RoomRuntimeData>();

            for (int i = 0; i < ctx.Rooms.Count; i++)
            {
                //RoomRuntimeData data = new RoomRuntimeData
                //{
                //    RoomId = i,
                //    RoomInfo = ctx.Rooms[i],
                //    Doors = new List<DoorController>(),
                //    AliveMonsterCount = 0
                //};

                //ctx.RoomStates.Add(data);
            }
        }
        // 스폰 포인트 저장
        private void AssignSpawnPointsToRooms(OldDungeonContext ctx, List<Vector2Int> monsterSpawnPoints)
        {
            foreach (var spawnPos in monsterSpawnPoints)
            {
                //for (int i = 0; i < ctx.RoomStates.Count; i++)
                //{
                //    RectInt rect = ctx.RoomStates[i].RoomInfo.Rect;

                //    if (rect.Contains(spawnPos))
                //    {
                //        ctx.RoomStates[i].SpawnPoint = spawnPos;
                //        break;
                //    }
                //}
            }
        }
    }
}
