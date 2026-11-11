using System.Collections.Generic;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SkyboundAdventure
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        LevelComplete
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Playing;
        [SerializeField] private Transform initialSpawnPoint;

        private Vector3 currentCheckpointPos;
        private Quaternion currentCheckpointRot;
        private int currentScore = 0;
        private int totalCoins = 0;
        private List<Coin> allCoins = new List<Coin>();
        private HashSet<Coin> collectedCoins = new HashSet<Coin>();
        private List<Checkpoint> allCheckpoints = new List<Checkpoint>();

        private PlayerController player;
        private LevelGoal goal;
        private ThirdPersonCamera camFollow;

        public GameState CurrentState => currentState;
        public bool IsPlaying => currentState == GameState.Playing;
        public int CurrentScore => currentScore;
        public int TotalCoins => totalCoins;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            player = FindAnyObjectByType<PlayerController>();
            goal = FindAnyObjectByType<LevelGoal>();
            camFollow = FindAnyObjectByType<ThirdPersonCamera>();

            RegisterCoins();
            RegisterCheckpoints();

            if (initialSpawnPoint == null && player != null)
            {
                GameObject sp = new GameObject("PlayerSpawnPoint");
                sp.transform.position = player.transform.position;
                sp.transform.rotation = player.transform.rotation;
                initialSpawnPoint = sp.transform;
            }

            if (initialSpawnPoint != null)
            {
                currentCheckpointPos = initialSpawnPoint.position;
                currentCheckpointRot = initialSpawnPoint.rotation;
            }

            StartGame();
        }

        private void Update()
        {
            if (ReadEscapeInputThisFrame())
            {
                if (currentState == GameState.Playing)
                {
                    PauseGame();
                }
                else if (currentState == GameState.Paused)
                {
                    ResumeGame();
                }
            }
        }

        private bool ReadEscapeInputThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) return true;
            return false;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }

        public void RegisterCoins()
        {
            allCoins.Clear();
            allCoins.AddRange(FindObjectsByType<Coin>(FindObjectsInactive.Include));
            totalCoins = allCoins.Count;
            collectedCoins.Clear();
            currentScore = 0;
        }

        public void RegisterCheckpoints()
        {
            allCheckpoints.Clear();
            allCheckpoints.AddRange(FindObjectsByType<Checkpoint>(FindObjectsInactive.Include));
        }

        public void SetCheckpoint(Vector3 pos, Quaternion rot)
        {
            currentCheckpointPos = pos;
            currentCheckpointRot = rot;
        }

        public void StartGame()
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;

            if (camFollow == null) camFollow = FindAnyObjectByType<ThirdPersonCamera>();
            if (camFollow != null) camFollow.SetCursorLocked(true);

            if (player != null)
            {
                player.ResetToSpawn(currentCheckpointPos, currentCheckpointRot);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowHUD();
                UIManager.Instance.UpdateScoreUI(currentScore, totalCoins);
            }
        }

        public void AddCoin(Coin coin)
        {
            if (currentState != GameState.Playing && currentState != GameState.Paused) return;
            if (coin == null || collectedCoins.Contains(coin)) return;

            // Add to centralized current-run collection
            collectedCoins.Add(coin);
            currentScore = collectedCoins.Count;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScoreUI(currentScore, totalCoins);
            }
        }

        public void OnPlayerDeath()
        {
            if (currentState != GameState.Playing) return;
            currentState = GameState.GameOver;

            if (player != null)
            {
                player.SetMovementEnabled(false);
            }

            if (camFollow != null) camFollow.SetCursorLocked(false);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDeath();
            }

            // Note: Collected coins remain collected on the Game Over screen
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver(currentScore, totalCoins);
            }
        }

        public void OnLevelComplete()
        {
            if (currentState != GameState.Playing) return;
            currentState = GameState.LevelComplete;

            if (player != null)
            {
                player.SetMovementEnabled(false);
            }

            if (camFollow != null) camFollow.SetCursorLocked(false);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowLevelComplete(currentScore, totalCoins);
            }
        }

        public void PauseGame()
        {
            if (currentState != GameState.Playing) return;
            currentState = GameState.Paused;
            Time.timeScale = 0f;

            if (camFollow != null) camFollow.SetCursorLocked(false);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseMenu();
            }
        }

        public void ResumeGame()
        {
            if (currentState != GameState.Paused) return;
            currentState = GameState.Playing;
            Time.timeScale = 1f;

            if (camFollow != null) camFollow.SetCursorLocked(true);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowHUD();
            }
        }

        public void RetryFromCheckpoint()
        {
            // Resetting / Retrying after dying or restarting restores full level coins
            RestartFullLevel();
        }

        public void RestartFullLevel()
        {
            Time.timeScale = 1f;
            currentScore = 0;
            collectedCoins.Clear();

            if (initialSpawnPoint != null)
            {
                currentCheckpointPos = initialSpawnPoint.position;
                currentCheckpointRot = initialSpawnPoint.rotation;
            }

            // Ensure full coin registry is maintained
            if (allCoins == null || allCoins.Count == 0)
            {
                RegisterCoins();
            }

            // Reactivate all 20 coins
            for (int i = 0; i < allCoins.Count; i++)
            {
                if (allCoins[i] != null)
                {
                    allCoins[i].ResetCoin();
                }
            }

            // Reset checkpoints
            for (int i = 0; i < allCheckpoints.Count; i++)
            {
                if (allCheckpoints[i] != null)
                {
                    allCheckpoints[i].ResetCheckpoint();
                }
            }

            if (goal != null)
            {
                goal.ResetGoal();
            }

            StartGame();
        }

        public void ReturnToMainMenu()
        {
            currentState = GameState.MainMenu;
            Time.timeScale = 1f;

            if (camFollow != null) camFollow.SetCursorLocked(false);

            if (player != null && initialSpawnPoint != null)
            {
                player.ResetToSpawn(initialSpawnPoint.position, initialSpawnPoint.rotation);
                player.SetMovementEnabled(false);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMainMenu();
            }
        }
    }
}
