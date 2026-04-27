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
            if(rooms == null || rooms.Count == 0)
                return result;

            List<int> indices = new List<int>();
            for(int i = 0; i < rooms.Count; i++)
                indices.Add(i);

            int startIndex = FindBottomLeftMostRoom(rooms);

            int stairIndex = FindTopRightRoom(rooms);

            List<int> candidates = new List<int>(indices);

            candidates.Remove(startIndex);
            candidates.Add(stairIndex);

            int shopIndex = -1;
            if(candidates.Count > 0)
            {
                shopIndex = candidates[Random.Range(0, candidates.Count)];
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                RoomType type;

                if(i == startIndex)
                    type = RoomType.Start;
                else if(i == stairIndex)
                    type = RoomType.Stairs;
                else if(i == shopIndex)
                    type = RoomType.Shop;
                else
                    type = Random.value < 0.8f ? RoomType.Enemy : RoomType.Encounter;

                IntRect room = rooms[i];
                Vector2Int centerCell = room.Center;
                Vector3 centerWorld = new Vector3(
                    centerCell.x * cellSize + cellSize * 0.5f,
                    centerCell.y * cellSize + cellSize * 0.5f,
                    0f
                    );

                result.Add(new RoomRuntimeData(i, room, type, centerCell, centerWorld));
            }


            return result;

        }

        private int FindTopRightRoom(List<IntRect> rooms)
        {
            int bestIndex = 0;
            int bestScore = int.MaxValue;

            for (int i = 0; i < rooms.Count; i++)
            {
                Vector2Int c = rooms[i].Center;
                int score = c.x + c.y;
                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private int FindBottomLeftMostRoom(List<IntRect> rooms)
        {
            int bestIndex = 0;
            int bestScore = int.MinValue;

            for (int i = 0; i < rooms.Count; i++)
            {
                Vector2Int c = rooms[i].Center;
                int score = c.x + c.y;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }
    }
}
