using System.Collections.Generic;
using MapGenerator.Core;
using ModularBSP.Core;
using UnityEngine;

namespace MapGenerator.Generation
{
    public class RoomTypeDistributor
    {
        public List<RoomRuntimeData> BuildRoomStates(List<IntRect> rooms, int cellSize)
        {
            List<RoomRuntimeData> result = new List<RoomRuntimeData>();

            for (int i = 0; i < rooms.Count; i++)
            {
                IntRect room = rooms[i];

                RoomRuntimeData data = new RoomRuntimeData
                (
                    i,
                    room,
                    RoomType.Enemy,
                    room.Center,
                    GridToWorldCenter(room, cellSize)
                );

                result.Add(data);
            }

            int startIndex = Random.Range(0, rooms.Count);
            result[startIndex].RoomType = RoomType.Start;

            int farthestIndex = GetFarthestRoomIndex(result, startIndex);
            result[farthestIndex].RoomType = RoomType.Stairs;

            int shopIndex = GetRandomAvailableRoomIndex(result, startIndex, startIndex);
            if (shopIndex != -1)
            {
                result[shopIndex].RoomType = RoomType.Shop;
            }


            AssignOtherRoomType(result);

            return result;
        }

        private Vector3 GridToWorldCenter(IntRect room, int cellSize)
        {
            float centerX = (room.x + room.width * 0.5f) * cellSize;
            float centerY = (room.y + room.height * 0.5f) * cellSize;

            return new Vector3(centerX, centerY, 0f);
        }


        private int GetFarthestRoomIndex(List<RoomRuntimeData> result, int startIndex)
        {
            Vector3 startPos = result[startIndex].CenterWorld;

            float maxDist = -1f;
            int farthestIndex = startIndex;

            for (int i = 0; i < result.Count; i++)
            {
                if (i == startIndex)
                    continue;

                float dist = Vector3.Distance(startPos, result[i].CenterWorld);

                if(dist >  maxDist)
                {
                    maxDist = dist;
                    farthestIndex = i;
                }

            }


            return farthestIndex;
        }



        private int GetRandomAvailableRoomIndex(List<RoomRuntimeData> rooms, int startIndex, int stairIndex) 
        {
            List<int> candidates = new List<int>();

            for(int i = 0; i < rooms.Count; i++)
            {
                if (i == startIndex || i == stairIndex)
                    continue;

                candidates.Add(i);
            }

            if( candidates.Count == 0)
            {
                Debug.LogWarning("[RoomTypeDistributor] Shop room candidate not found.");
                return -1;
            }

            return candidates[Random.Range(0, candidates.Count)];
        }


        private void AssignOtherRoomType(List<RoomRuntimeData> result)
        {
            foreach (var room in result)
            {
                if (room.RoomType == RoomType.Start || room.RoomType == RoomType.Stairs || room.RoomType == RoomType.Shop)
                    continue;

                float roll = Random.value;

                if (roll < 0.3f)
                    room.RoomType = RoomType.Encounter;
                else
                    room.RoomType = RoomType.Enemy;

            }
        }
    }
}

//public List<RoomRuntimeData> BuildRoomStates(List<IntRect> rooms, int cellSize)
//{
//    List<RoomRuntimeData> result = new List<RoomRuntimeData>();
//    if(rooms == null || rooms.Count == 0)
//        return result;

//    List<int> indices = new List<int>();
//    for(int i = 0; i < rooms.Count; i++)
//        indices.Add(i);

//    int startIndex = FindBottomLeftMostRoom(rooms);

//    int stairIndex = FindTopRightRoom(rooms);

//    List<int> candidates = new List<int>(indices);

//    candidates.Remove(startIndex);
//    candidates.Add(stairIndex);

//    int shopIndex = -1;
//    if(candidates.Count > 0)
//    {
//        shopIndex = candidates[Random.Range(0, candidates.Count)];
//    }

//    for (int i = 0; i < rooms.Count; i++)
//    {
//        RoomType type;

//        if(i == startIndex)
//            type = RoomType.Start;
//        else if(i == stairIndex)
//            type = RoomType.Stairs;
//        else if(i == shopIndex)
//            type = RoomType.Shop;
//        else
//            type = Random.value < 0.8f ? RoomType.Enemy : RoomType.Encounter;

//        IntRect room = rooms[i];
//        Vector2Int centerCell = room.Center;
//        Vector3 centerWorld = new Vector3(
//            centerCell.x * cellSize + cellSize * 0.5f,
//            centerCell.y * cellSize + cellSize * 0.5f,
//            0f
//            );

//        result.Add(new RoomRuntimeData(i, room, type, centerCell, centerWorld));
//    }


//    return result;

//}

// °íÁ¤°ª
//private int FindTopRightRoom(List<IntRect> rooms)
//{
//    int bestIndex = 0;
//    int bestScore = int.MaxValue;

//    for (int i = 0; i < rooms.Count; i++)
//    {
//        Vector2Int c = rooms[i].Center;
//        int score = c.x + c.y;
//        if (score < bestScore)
//        {
//            bestScore = score;
//            bestIndex = i;
//        }
//    }

//    return bestIndex;
//}

//private int FindBottomLeftMostRoom(List<IntRect> rooms)
//{
//    int bestIndex = 0;
//    int bestScore = int.MinValue;

//    for (int i = 0; i < rooms.Count; i++)
//    {
//        Vector2Int c = rooms[i].Center;
//        int score = c.x + c.y;
//        if (score > bestScore)
//        {
//            bestScore = score;
//            bestIndex = i;
//        }
//    }

//    return bestIndex;
//}
