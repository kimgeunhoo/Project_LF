using MapGenerator.Core;
using ModularBSP.Core;
using UnityEngine;


namespace GameScript.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Managers")]
        [SerializeField]
        private DungeonManager dungeonManager;

        [Header("Runtime")]
        [SerializeField]
        private DungeonContext ctx;

        [Header("GameOverUI")]
        [SerializeField]
        private GameOverUI gameOverUI;

        private bool isGameStarted;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            StartGame();
        }
        public void StartGame()
        {
            if (isGameStarted)
                return;

            isGameStarted = true;

            if (dungeonManager == null)
            {
                dungeonManager = FindFirstObjectByType<DungeonManager>();

            }

            Debug.Log("[GameManager] Game Start");
        }

        public void RegisterPlayer(Player player)
        {
            player.OnDead += HandlePlayerDead;
        }

        private void HandlePlayerDead()
        {
            gameOverUI.ShowGameOver();
        }

        public void OnEnterRoom(int roomId)
        {
            if(ctx == null)
            {
                Debug.LogError("[GameManager] DungeonContext가 없습니다");
                return;
            }

            if(roomId < 0 || roomId >= ctx.RoomStates.Count)
            {
                Debug.LogError($"[GameManager] 잘못된 roomId: {roomId}");
                return;
            }

            RoomRuntimeData room = ctx.RoomStates[roomId];

            dungeonManager.OnEnterRoom(room.RoomId, room.RoomType);
        }


    }

}
