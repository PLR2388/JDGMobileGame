using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using JDG.Presentation.Views;
using VContainer;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for round/turn display logic.
    /// Part of Phase 28 - MonoBehaviour Wave 1 migration to MVP pattern.
    /// Handles business logic for what to display based on game state.
    /// </summary>
    public class RoundDisplayPresenter
    {
        private readonly IRoundDisplayView _view;
        private readonly IEventBus _eventBus;
        private readonly ILocalizationService _localizationService;
        private PlayerId _currentPlayer;
        private Phase _currentPhase;

        [Inject]
        public RoundDisplayPresenter(
            IRoundDisplayView view,
            IEventBus eventBus,
            ILocalizationService localizationService)
        {
            _view = view;
            _eventBus = eventBus;
            _localizationService = localizationService;

            // Subscribe to game events
            _eventBus.Subscribe<PhaseChangedEvent>(OnPhaseChanged);
            _eventBus.Subscribe<PlayerTurnChangedEvent>(OnPlayerTurnChanged);
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
                    _view.SetRoundText(_localizationService.GetLocalizedValue(LocalizationKeys.PHASE_DRAW));

                    if (shouldRotateCamera)
                    {
                        _view.RotateCamera();
                    }

                    UpdatePlayerTurnText();
                    break;

                case Phase.Attack:
                    _view.SetInHandButtonVisible(false);
                    _view.SetRoundText(_localizationService.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK));
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
            _currentPlayer = evt.NewPlayer;
            UpdatePlayerTurnText();
        }

        private void UpdateDisplay()
        {
            // Update phase text based on current phase
            string phaseText = _currentPhase switch
            {
                Phase.Draw => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_DRAW),
                Phase.Choose => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_CHOOSE),
                Phase.Attack => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_ATTACK),
                Phase.End => _localizationService.GetLocalizedValue(LocalizationKeys.PHASE_END),
                _ => ""
            };

            _view.SetRoundText(phaseText);
        }

        private void UpdatePlayerTurnText()
        {
            bool isPlayer1Turn = _currentPlayer == PlayerId.Player1;

            // Note: Display shows the NEXT player's name at end phase
            string playerName = isPlayer1Turn
                ? _localizationService.GetLocalizedValue(LocalizationKeys.PLAYER_TWO)
                : _localizationService.GetLocalizedValue(LocalizationKeys.PLAYER_ONE);

            _view.SetPlayerTurnText(playerName);
        }
    }
}
