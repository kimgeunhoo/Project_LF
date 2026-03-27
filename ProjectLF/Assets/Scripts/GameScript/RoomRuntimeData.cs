using BSPDungeonGenrator.Config;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RoomRuntimeData : MonoBehaviour
{
    public int RoomId;
    public RoomInfo RoomInfo;
    public List<DoorController> Doors = new List<DoorController>();
    public int AliveMonsterCount;
}
