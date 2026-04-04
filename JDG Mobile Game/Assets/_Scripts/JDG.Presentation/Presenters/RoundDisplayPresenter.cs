using System;
using JDG.Application;
using JDG.Application.Services;
using JDG.Core;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Presentation.Views;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for round/turn display logic.
    /// Part of Phase 28 - MonoBehaviour Wave 1 migration to MVP pattern.
    /// Phase 42: Moved to JDG.Presentation assembly.
    /// Phase 144: Implements IDisposable for proper subscription cleanup.
    /// Handles business logic for what to display based on game state.
    /// </summary>
    public class RoundDisplayPresenter : IDisposable
    {
        private readonly IRoundDisplayView _view;
        private readonly IEventBus _eventBus;
        private readonly ILocalizationService _localizationService;
        private PlayerId _currentPlayer;
        private Phase _currentPhase;

        // Phase 144: Store subscriptions for proper disposal
        private readonly IDisposable _phaseChangedSubscription;
        private readonly IDisposable _playerTurnChangedSubscription;
        private bool _disposed;

        /// <summary>
        /// Constructor for RoundDisplayPresenter.
        /// Phase 144: Removed misleading [Inject] attribute - this class is manually
        /// constructed with `new RoundDisplayPresenter(...)` in RoundDisplayManager.
        /// </summary>
        public RoundDisplayPresenter(
            IRoundDisplayView view,
            IEventBus eventBus,
            ILocalizationService localizationService)
        {
            _view = view;
            _eventBus = eventBus;
            _localizationService = localizationService;

            // Subscribe to game events
            // Phase 144: Store subscriptions for disposal
            _phaseChangedSubscription = _eventBus.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            _playerTurnChangedSubscription = _eventBus.Subscribe<PlayerTurnChangedEvent>(OnPlayerTurnChanged);
        }

        /// <summary>
        /// Initializes the presenter with current game state.
        /// Call this after construction to set initial display.
        /// </summary>
        public void Initialize(PlayerId currentPlayer, Phase currentPhase)
        {
            _currentPlayer = currentPlayer;
            _currentPhase = currentPhase;
            UpdateDisplay();
        }

        /// <summary>
        /// Updates the UI for the next round based on current phase.
        /// </summary>
        /// <param name="shouldRotateCamera">Whether to rotate camera during transition</param>
        public void AdaptUIToNextRound(bool shouldRotateCamera)
        {
            switch (_currentPhase)
            {
                case Phase.End:
                    _view.SetInHandButtonVisible(true);
                    _view.SetRoundText(_localizationService.GetLocalizedValue(LocalizationKeys.PHASE_DRAW.ToString()));

                    if (shouldRotateCamera)
                    {
                        _view.RotateCamera();
                    }

                    UpdatePlayerTurnText();
                    break;

                case Phase.Attack:
                    _view.SetInHandButtonVisible(false);
                    _view.SetRoundText(_localizationService.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK.ToString()));
                    break;

                // Add more phase handling as needed
            }
        }

        private void OnPhaseChanged(PhaseChangedEvent evt)
        {
            _currentPhase = evt.NewPhase;
            UpdateDisplay();
        }

        private void OnPlayerTurnChanged(PlayerTurnChangedEvent evt)
        {
            // Convert CardOwner to PlayerId
            _currentPlayer = evt.NewPlayer == JDG.Domain.CardOwner.Player1
                ? PlayerId.Player1
                : PlayerId.Player2;
            UpdatePlayerTurnText();
        }

        private void UpdateDisplay()
        {
            // Update phase text based on current phase
            // Note: LocalizationKeys is an enum, need to convert to string
            string phaseText = _currentPhase switch
            {
                Phase.Draw => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_DRAW.ToString()),
                Phase.Choose => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_CHOOSE.ToString()),
                Phase.Attack => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK.ToString()),
                Phase.End => "", // PHASE_END not in LocalizationKeys enum
                _ => ""
            };

            _view.SetRoundText(phaseText);
        }

        private void UpdatePlayerTurnText()
        {
            bool isPlayer1Turn = _currentPlayer == PlayerId.Player1;

            // Note: Display shows the NEXT player's name at end phase
            string playerName = isPlayer1Turn
                ? _localizationService.GetLocalizedValue(LocalizationKeys.PLAYER_TWO.ToString())
                : _localizationService.GetLocalizedValue(LocalizationKeys.PLAYER_ONE.ToString());

            _view.SetPlayerTurnText(playerName);
        }

        /// <summary>
        /// Phase 144: Disposes all EventBus subscriptions to prevent memory leaks.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _phaseChangedSubscription?.Dispose();
            _playerTurnChangedSubscription?.Dispose();

            _disposed = true;
        }
    }
}
