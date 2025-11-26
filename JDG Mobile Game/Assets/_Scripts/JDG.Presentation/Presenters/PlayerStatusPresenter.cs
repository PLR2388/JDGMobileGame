using System;
using JDG.Application;
using JDG.Application.Repositories;
using JDG.Application.Mappers;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Presentation.Views;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for player status display (health, shields, etc.).
    /// </summary>
    public class PlayerStatusPresenter : IDisposable
    {
        private readonly IPlayerStatusView _view;
        private readonly IEventBus _eventBus;
        private readonly IPlayerRepository _playerRepository;
        private readonly PlayerId _playerId;

        private IDisposable _healthChangedSubscription;
        private IDisposable _shieldChangedSubscription;
        private IDisposable _turnChangedSubscription;
        private IDisposable _damagedSubscription;

        private int _cachedHealth;
        private int _cachedShields;

        public PlayerStatusPresenter(
            IPlayerStatusView view,
            IEventBus eventBus,
            IPlayerRepository playerRepository,
            PlayerId playerId)
        {
            _view = view;
            _eventBus = eventBus;
            _playerRepository = playerRepository;
            _playerId = playerId;

            SubscribeToEvents();
            RefreshStatus();
        }

        /// <summary>
        /// Refreshes the player status display.
        /// </summary>
        public void RefreshStatus()
        {
            var player = _playerRepository.GetPlayer(_playerId);
            if (player == null)
                return;

            _cachedHealth = player.Health;
            _cachedShields = player.Shields;

            _view.UpdateStatus(player.ToDTO());
        }

        private void SubscribeToEvents()
        {
            _healthChangedSubscription = _eventBus.Subscribe<PlayerHealthChangedEvent>(OnHealthChanged);
            _shieldChangedSubscription = _eventBus.Subscribe<PlayerShieldChangedEvent>(OnShieldChanged);
            _turnChangedSubscription = _eventBus.Subscribe<PlayerTurnChangedEvent>(OnTurnChanged);
            _damagedSubscription = _eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        }

        private void OnHealthChanged(PlayerHealthChangedEvent evt)
        {
            if (evt.Player == _playerId.ToCardOwner())
            {
                _view.AnimateHealthChange(evt.OldHealth, evt.NewHealth);
                _cachedHealth = evt.NewHealth;
                RefreshStatus();
            }
        }

        private void OnShieldChanged(PlayerShieldChangedEvent evt)
        {
            if (evt.Player == _playerId.ToCardOwner())
            {
                _view.AnimateShieldChange(evt.OldShields, evt.NewShields);
                _cachedShields = evt.NewShields;
                RefreshStatus();
            }
        }

        private void OnTurnChanged(PlayerTurnChangedEvent evt)
        {
            bool isMyTurn = evt.NewPlayer == _playerId.ToCardOwner();
            _view.SetActiveTurn(isMyTurn);
        }

        private void OnPlayerDamaged(PlayerDamagedEvent evt)
        {
            if (evt.PlayerId == _playerId.ToCardOwner())
            {
                _view.ShowDamageEffect(evt.Damage);
                RefreshStatus();
            }
        }

        public void Dispose()
        {
            _healthChangedSubscription?.Dispose();
            _shieldChangedSubscription?.Dispose();
            _turnChangedSubscription?.Dispose();
            _damagedSubscription?.Dispose();
        }
    }
}
