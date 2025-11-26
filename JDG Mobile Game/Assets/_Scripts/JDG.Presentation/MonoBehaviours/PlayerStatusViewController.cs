using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JDG.Application.DTOs;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.DI;
using JDG.Presentation.Views;
using JDG.Presentation.Presenters;

namespace JDG.Presentation.MonoBehaviours
{
    /// <summary>
    /// MonoBehaviour implementation of IPlayerStatusView.
    /// Displays player health, shields, and deck/hand counts.
    /// </summary>
    public class PlayerStatusViewController : MonoBehaviour, IPlayerStatusView
    {
        [Header("Configuration")]
        [SerializeField] private int _playerNumber = 1; // 1 or 2

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _shieldsText;
        [SerializeField] private TextMeshProUGUI _deckCountText;
        [SerializeField] private TextMeshProUGUI _handCountText;
        [SerializeField] private GameObject _activeTurnIndicator;
        [SerializeField] private Image _damageFlashImage;

        private PlayerStatusPresenter _presenter;

        private void Start()
        {
            var playerId = _playerNumber == 1 ? PlayerId.Player1 : PlayerId.Player2;

            // Get dependencies
            var eventBus = ServiceLocator.GetEventBus();
            var playerRepo = ServiceLocator.GetPlayerRepository();

            // Create presenter
            _presenter = new PlayerStatusPresenter(
                this,
                eventBus,
                playerRepo,
                playerId
            );

            // Hide damage flash initially
            if (_damageFlashImage != null)
                _damageFlashImage.enabled = false;

            // Hide active turn indicator initially
            if (_activeTurnIndicator != null)
                _activeTurnIndicator.SetActive(false);
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
        }

        /// <summary>
        /// Public method to update display (called by GameViewController).
        /// </summary>
        public void UpdateDisplay(PlayerDTO player)
        {
            UpdateStatus(player);
        }

        #region IPlayerStatusView Implementation

        public void UpdateStatus(PlayerDTO player)
        {
            if (_healthText != null)
            {
                _healthText.text = $"{player.Health}/{player.MaxHealth}";
            }

            if (_healthSlider != null)
            {
                _healthSlider.maxValue = player.MaxHealth;
                _healthSlider.value = player.Health;
            }

            if (_shieldsText != null)
            {
                _shieldsText.text = player.Shields > 0 ? $"Shield: {player.Shields}" : "";
            }

            if (_deckCountText != null)
            {
                _deckCountText.text = $"Deck: {player.DeckCount}";
            }

            if (_handCountText != null)
            {
                _handCountText.text = $"Hand: {player.HandCount}";
            }
        }

        public void AnimateHealthChange(int oldHealth, int newHealth)
        {
            // Simple lerp animation (could be improved with DOTween or coroutines)
            if (_healthSlider != null)
            {
                // Immediate update for now - could add smooth transition
                _healthSlider.value = newHealth;
            }
        }

        public void AnimateShieldChange(int oldShields, int newShields)
        {
            // Could add shield gain/loss animation
            if (_shieldsText != null)
            {
                _shieldsText.text = newShields > 0 ? $"Shield: {newShields}" : "";
            }
        }

        public void ShowDamageEffect(int damage)
        {
            // Flash red to indicate damage
            if (_damageFlashImage != null)
            {
                StartCoroutine(FlashDamage());
            }
        }

        public void SetActiveTurn(bool isActive)
        {
            if (_activeTurnIndicator != null)
            {
                _activeTurnIndicator.SetActive(isActive);
            }
        }

        #endregion

        private System.Collections.IEnumerator FlashDamage()
        {
            if (_damageFlashImage == null)
                yield break;

            _damageFlashImage.enabled = true;
            var color = _damageFlashImage.color;
            color.a = 0.5f;
            _damageFlashImage.color = color;

            yield return new WaitForSeconds(0.2f);

            _damageFlashImage.enabled = false;
        }
    }
}
