using BSPDungeonGenrator;
using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Generation;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomBattleSystem : MonoBehaviour
{

    [SerializeField]
    private DungeonGeneraterByBSP dungeonGenerater;

    public void RoomMonsterDeadMethod(int roomId)
    {
        OnMonstersDeadInRoom(roomId);
    }

    private void OnMonstersDeadInRoom(int roomId)
    {
        DungeonContext ctx = dungeonGenerater.Ctx;
        RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);

        if (room == null)
            return;

        room.AliveMonsterCount--;

        if (room.AliveMonsterCount <= 0)
        {
            room.AliveMonsterCount = 0;

            foreach (var door in room.Doors)
            {
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }

    }
    //public void LockRoom(int roomId)
    //{
    //    DungeonContext ctx = dungeonGenerater.Ctx;
    //    RoomRuntimeData room = ctx.RoomStates.Find(r => r.RoomId == roomId);

    //    if (room == null)
    //        return;

    //    foreach (var door in room.Doors)
    //    {
    //        //if (door != null)
    //           // door.SetOpen(false);
    //    }
    //}


}
