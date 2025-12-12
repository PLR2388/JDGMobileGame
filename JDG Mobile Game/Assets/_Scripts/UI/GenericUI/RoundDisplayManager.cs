using JDG.Application;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using JDG.Presentation.Views;
using TMPro;
using UnityEngine;
using VContainer;

/// <summary>
/// View implementation for round/turn display.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 24-25: Removed ServiceLocator, using VContainer DI.
/// Phase 27: Updated to use JDG.Domain.Phase after global Phase enum was removed with GameStateManager.
/// Phase 28: Migrated to MVP pattern - now implements IRoundDisplayView, business logic moved to RoundDisplayPresenter.
/// </summary>
public class RoundDisplayManager : MonoBehaviour, IRoundDisplayView
{
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] protected GameObject inHandButton;

    private readonly Vector3 cameraRotation = new Vector3(0, 0, 180);

    // Phase 28: Presenter handles business logic
    private RoundDisplayPresenter _presenter;
    private GameStateService _gameStateService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 28: Injects services needed to create the presenter.
    /// </summary>
    [Inject]
    public void Construct(
        GameStateService gameStateService,
        IEventBus eventBus,
        ILocalizationService localizationService)
    {
        _gameStateService = gameStateService;

        // Phase 28: Create presenter with this view and injected services
        _presenter = new RoundDisplayPresenter(this, eventBus, localizationService);
    }

    private void Start()
    {
        // Initialize presenter with current game state
        _presenter.Initialize(_gameStateService.CurrentPlayer, _gameStateService.CurrentPhase);
    }

    #region IRoundDisplayView Implementation

    /// <summary>
    /// Sets the displayed round/phase text.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void SetRoundText(string text)
    {
        if (roundText != null)
        {
            roundText.text = text;
        }
    }

    /// <summary>
    /// Sets the player turn indicator text.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void SetPlayerTurnText(string playerName)
    {
        if (playerText != null)
        {
            playerText.text = playerName;
        }
    }

    /// <summary>
    /// Shows or hides the "in hand" button.
    /// Phase 28: Pure view operation, no business logic.
    /// </summary>
    public void SetInHandButtonVisible(bool visible)
    {
        if (inHandButton != null)
        {
            inHandButton.SetActive(visible);
        }
    }

    /// <summary>
    /// Rotates the camera for the end phase transition.
    /// Phase 28: Unity-specific operation kept in view.
    /// </summary>
    public void RotateCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.transform.Rotate(cameraRotation);
        }
    }

    #endregion

    #region Legacy Public Methods (for backward compatibility)

    /// <summary>
    /// Updates the UI elements based on the game phase in the next round.
    /// Phase 28: Delegates to presenter for business logic.
    /// DEPRECATED: Direct calls to this method should eventually use presenter directly.
    /// </summary>
    public void AdaptUIToPhaseIdInNextRound(bool rotate)
    {
        if (_presenter != null)
        {
            _presenter.AdaptUIToNextRound(rotate);
        }
    }

    #endregion
}