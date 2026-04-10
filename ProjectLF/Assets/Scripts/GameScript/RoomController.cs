using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Info")]
    [SerializeField] 
    private int roomId;
    [SerializeField]
    private bool isCleared = false;
    [SerializeField]
    private bool isBattleStarted = false;

    [Header("Runtime")]
    [SerializeField] private List<DoorController> doors = new List<DoorController>();
    [SerializeField] private List<Monster> monsters = new List<Monster>();

    private int aliveMonsterCount = 0;

    public int RoomId => roomId;
    public bool IsCleared => isCleared;
    public bool IsBattleStarted => isBattleStarted;
    public int AliveMonsterCount => aliveMonsterCount;
    public List<DoorController> Doors => doors; 

    public void Init(int roomId)
    {
        this.roomId = roomId;
        isCleared = false;
        isBattleStarted = false;
        aliveMonsterCount = 0;

        doors.Clear();
        monsters.Clear();
    }

    //private void RegisterDoor(DoorController doorController)
    //{
    //    if (doorController == null)
    //        return;
    //    if (doors.Contains(doorController))
    //        return;

    //    doors.Add(doorController);
    //}

    //private void RegisterMonster(Monster monster)
    //{
    //    if (monster == null) 
    //        return;
    //    if (monsters.Contains(monster))
    //        return;

    //    isBattleStarted = true;

    //    if(aliveMonsterCount > 0)
    //    {
    //        CloseAllDoors();
    //    }
    //    else
    //    {
    //        ClearRoom();
    //    }

    //}
    public void OnMonsterDead(Monster monster)
    {
        if (monster != null)
        {
            monsters.Remove(monster);
        }

        aliveMonsterCount--;

        if (aliveMonsterCount < 0)
            aliveMonsterCount = 0;

        Debug.Log($"[Room {roomId}] Monster Dead / Alive Count = {aliveMonsterCount}");

        if (aliveMonsterCount == 0)
        {
            ClearRoom();
        }
    }

    public void ClearRoom()
    {
        if (isCleared) return;

        isCleared = true;
        isBattleStarted = false;

        OpenAllDoors();

        Debug.Log($"[Room {roomId}] Clear!");
    }

    public void OpenAllDoors()
    {
        foreach (var door in doors)
        {
            if (door != null)
                door.OpenDoor();
        }
    }

    public void CloseAllDoors()
    {
        foreach (var door in doors)
        {
            if (door != null)
                door.CloseDoor();
        }
    }

}
