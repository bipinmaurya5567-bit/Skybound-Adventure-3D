using UnityEngine;
using UnityEngine.UI;

namespace SkyboundAdventure
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject levelCompletePanel;

        [Header("HUD Elements")]
        [SerializeField] private Text coinText;
        [SerializeField] private Button pauseButton;

        [Header("Main Menu Elements")]
        [SerializeField] private Button playButton;

        [Header("Pause Menu Elements")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseRestartButton;
        [SerializeField] private Button pauseMenuButton;

        [Header("Game Over Elements")]
        [SerializeField] private Text gameOverScoreText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button gameOverMenuButton;

        [Header("Level Complete Elements")]
        [SerializeField] private Text completeScoreText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button completeMenuButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            BindButtons();
        }

        private void BindButtons()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.PauseGame();
                });
            }

            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.RestartFullLevel();
                });
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveAllListeners();
                resumeButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.ResumeGame();
                });
            }

            if (pauseRestartButton != null)
            {
                pauseRestartButton.onClick.RemoveAllListeners();
                pauseRestartButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.RestartFullLevel();
                });
            }

            if (pauseMenuButton != null)
            {
                pauseMenuButton.onClick.RemoveAllListeners();
                pauseMenuButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.ReturnToMainMenu();
                });
            }

            if (retryButton != null)
            {
                retryButton.onClick.RemoveAllListeners();
                retryButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.RestartFullLevel();
                });
            }

            if (gameOverMenuButton != null)
            {
                gameOverMenuButton.onClick.RemoveAllListeners();
                gameOverMenuButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.ReturnToMainMenu();
                });
            }

            if (playAgainButton != null)
            {
                playAgainButton.onClick.RemoveAllListeners();
                playAgainButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.RestartFullLevel();
                });
            }

            if (completeMenuButton != null)
            {
                completeMenuButton.onClick.RemoveAllListeners();
                completeMenuButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayClick();
                    GameManager.Instance?.ReturnToMainMenu();
                });
            }
        }

        public void SetupReferences(
            GameObject hud, GameObject main, GameObject pause, GameObject gameOver, GameObject complete,
            Text coins, Text goScore, Text compScore,
            Button pauseBtn, Button playBtn, Button resumeBtn, Button pauseRestartBtn, Button pauseMenuBtn,
            Button retryBtn, Button goMenuBtn, Button playAgainBtn, Button compMenuBtn)
        {
            hudPanel = hud;
            mainMenuPanel = main;
            pauseMenuPanel = pause;
            gameOverPanel = gameOver;
            levelCompletePanel = complete;

            coinText = coins;
            gameOverScoreText = goScore;
            completeScoreText = compScore;

            pauseButton = pauseBtn;
            playButton = playBtn;
            resumeButton = resumeBtn;
            pauseRestartButton = pauseRestartBtn;
            pauseMenuButton = pauseMenuBtn;
            retryButton = retryBtn;
            gameOverMenuButton = goMenuBtn;
            playAgainButton = playAgainBtn;
            completeMenuButton = compMenuBtn;

            BindButtons();
        }

        public void UpdateScoreUI(int current, int total)
        {
            if (coinText != null)
            {
                coinText.text = $"★ COINS: {current} / {total}";
            }
        }

        public void ShowHUD()
        {
            SetPanelActive(hudPanel, true);
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(pauseMenuPanel, false);
            SetPanelActive(gameOverPanel, false);
            SetPanelActive(levelCompletePanel, false);
        }

        public void ShowMainMenu()
        {
            SetPanelActive(hudPanel, false);
            SetPanelActive(mainMenuPanel, true);
            SetPanelActive(pauseMenuPanel, false);
            SetPanelActive(gameOverPanel, false);
            SetPanelActive(levelCompletePanel, false);
        }

        public void ShowPauseMenu()
        {
            SetPanelActive(hudPanel, true);
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(pauseMenuPanel, true);
            SetPanelActive(gameOverPanel, false);
            SetPanelActive(levelCompletePanel, false);
        }

        public void ShowGameOver(int score, int total)
        {
            SetPanelActive(hudPanel, false);
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(pauseMenuPanel, false);
            SetPanelActive(gameOverPanel, true);
            SetPanelActive(levelCompletePanel, false);

            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = $"You fell into the clouds!\nCoins Collected: {score} / {total}";
            }
        }

        public void ShowLevelComplete(int score, int total)
        {
            SetPanelActive(hudPanel, false);
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(pauseMenuPanel, false);
            SetPanelActive(gameOverPanel, false);
            SetPanelActive(levelCompletePanel, true);

            if (completeScoreText != null)
            {
                completeScoreText.text = $"You reached the Sky Sanctuary!\nTotal Coins Collected: {score} / {total}";
            }
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null) panel.SetActive(active);
        }
    }
}
