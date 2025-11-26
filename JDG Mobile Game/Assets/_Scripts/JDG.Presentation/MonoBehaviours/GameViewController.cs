using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JDG.Application.DTOs;
using JDG.Application.UseCases;
using JDG.Domain;
using JDG.Infrastructure.DI;
using JDG.Presentation.Views;
using JDG.Presentation.Presenters;

namespace JDG.Presentation.MonoBehaviours
{
    /// <summary>
    /// MonoBehaviour implementation of IGameView.
    /// Connects Unity UI to the GamePresenter using MVP pattern.
    /// </summary>
    public class GameViewController : MonoBehaviour, IGameView
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _phaseText;
        [SerializeField] private TextMeshProUGUI _turnNumberText;
        [SerializeField] private TextMeshProUGUI _currentPlayerText;
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _messageText;

        [Header("Player UI")]
        [SerializeField] private PlayerStatusViewController _player1Status;
        [SerializeField] private PlayerStatusViewController _player2Status;

        private GamePresenter _presenter;

        private void Start()
        {
            // Get dependencies from service locator
            var eventBus = ServiceLocator.GetEventBus();
            var gameStateRepo = ServiceLocator.GetGameStateRepository();
            var playerRepo = ServiceLocator.GetPlayerRepository();
            var endTurnUseCase = ServiceLocator.GetEndTurnUseCase();
            var startGameUseCase = ServiceLocator.GetStartGameUseCase();

            // Create presenter
            _presenter = new GamePresenter(
                this,
                eventBus,
                gameStateRepo,
                playerRepo,
                endTurnUseCase,
                startGameUseCase
            );

            // Setup UI events
            _endTurnButton.onClick.AddListener(OnEndTurnClicked);

            // Hide game over panel initially
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(false);

            // Start the game
            _presenter.StartGame();
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
            _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
        }

        private void OnEndTurnClicked()
        {
            _presenter?.EndTurn();
        }

        #region IGameView Implementation

        public void ShowPhase(Phase phase)
        {
            if (_phaseText != null)
            {
                _phaseText.text = $"Phase: {phase}";
            }
        }

        public void ShowTurnNumber(int turnNumber)
        {
            if (_turnNumberText != null)
            {
                _turnNumberText.text = $"Turn: {turnNumber}";
            }
        }

        public void ShowCurrentPlayer(CardOwner player)
        {
            if (_currentPlayerText != null)
            {
                _currentPlayerText.text = $"Current: {player}";
            }
        }

        public void UpdatePlayer1(PlayerDTO player)
        {
            _player1Status?.UpdateDisplay(player);
        }

        public void UpdatePlayer2(PlayerDTO player)
        {
            _player2Status?.UpdateDisplay(player);
        }

        public void ShowGameOver(CardOwner winner, string reason)
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true);
            }

            if (_gameOverText != null)
            {
                _gameOverText.text = $"{winner} Wins!\n{reason}";
            }
        }

        public void SetEndTurnButtonEnabled(bool enabled)
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.interactable = enabled;
            }
        }

        public void ShowMessage(string message)
        {
            if (_messageText != null)
            {
                _messageText.text = message;
                // Could add fade out animation here
            }

            Debug.Log($"GameView: {message}");
        }

        #endregion
    }
}
