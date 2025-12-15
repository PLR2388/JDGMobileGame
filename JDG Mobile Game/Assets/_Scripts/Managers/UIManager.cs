using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// Phase 5: UIManager is being decomposed into focused presenters (MVP pattern).
/// This class now delegates to CardDisplayPresenter, DialogPresenter, and CardSelectorPresenter.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 34: Uses ILocalizationService via DI instead of LocalizationSystem.Instance.
/// Phase 40: Uses IDialogService via DI instead of MessageBox.Instance/CardSelector.Instance.
/// UIManager will eventually be removed once all callsites migrate to the new presenters.
/// </summary>
[System.Obsolete("UIManager is being phased out. Use CardDisplayPresenter, DialogPresenter, and CardSelectorPresenter directly via dependency injection instead. This MonoBehaviour will be removed in a future phase.")]
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject bigImageCard;
    [SerializeField] protected GameObject nextPhaseButton;
    [SerializeField] protected Transform canvas;

    private Image bigImageCardImage;

    // Phase 5: Presenter instances
    private CardDisplayPresenter _cardDisplayPresenter;
    private DialogPresenter _dialogPresenter;
    private CardSelectorPresenter _cardSelectorPresenter;

    // Phase 34: Injected services
    private ILocalizationService _localizationService;
    // Phase 40: IDialogService for presenters
    private IDialogService _dialogService;
    // Phase 39: ICardVisualService for presenter migration
    private ICardVisualService _cardVisualService;

    [Inject]
    public void Construct(ILocalizationService localizationService, IDialogService dialogService, ICardVisualService cardVisualService)
    {
        _localizationService = localizationService;
        _dialogService = dialogService;
        _cardVisualService = cardVisualService;
    }

    /// <summary>
    /// Initialize component references and presenters.
    /// Phase 19-20: No longer calls base.Awake() since not a singleton.
    /// </summary>
    private void Awake()
    {
        bigImageCardImage = bigImageCard.GetComponent<Image>();
    }

    private void Start()
    {
        // Phase 34: Create presenters in Start() to ensure injection has occurred
        InitializePresenters();
    }

    private void InitializePresenters()
    {
        // Phase 5: Create simple presenter instances (not MonoBehaviours)
        // Phase 34: Pass ILocalizationService to presenters
        // Phase 40: Pass IDialogService to presenters
        // Phase 39: Pass ICardVisualService for Material resolution
        _cardDisplayPresenter = new CardDisplayPresenter(bigImageCard, _cardVisualService);
        _dialogPresenter = new DialogPresenter(canvas, _localizationService, _dialogService);
        _cardSelectorPresenter = new CardSelectorPresenter(canvas, nextPhaseButton, _localizationService, _dialogService);
    }

    /// <summary>
    /// Displays the given card on the large card viewer.
    /// Phase 5: Now delegates to CardDisplayPresenter
    /// </summary>
    /// <param name="card">Card to be displayed.</param>
    public void DisplayCardOnLargeView(InGameCard card)
    {
        _cardDisplayPresenter?.ShowCard(card);
    }

    /// <summary>
    /// Displays a message box to inform the user about the available opponents for invocation.
    /// Phase 5: Now delegates to CardSelectorPresenter
    /// </summary>
    /// <param name="invocationCards">List of invocable cards.</param>
    /// <param name="positiveAction">Action on positive button click.</param>
    /// <param name="negativeAction">Action on negative button click.</param>
    public void DisplayOpponentAvailableMessageBox(
        List<InGameCard> invocationCards,
        UnityAction<InGameInvocationCard> positiveAction,
        UnityAction negativeAction)
    {
        _cardSelectorPresenter?.ShowOpponentSelector(invocationCards, positiveAction, negativeAction);
    }

    /// <summary>
    /// Hides the large card viewer.
    /// Phase 5: Now delegates to CardDisplayPresenter
    /// </summary>
    public void HideBigImage()
    {
        _cardDisplayPresenter?.HideCard();
    }

    /// <summary>
    /// Displays a pause menu with given positive action.
    /// Phase 5: Now delegates to DialogPresenter
    /// </summary>
    /// <param name="onPositiveAction">Action to execute on positive button click.</param>
    public void DisplayPauseMenu(UnityAction onPositiveAction)
    {
        _dialogPresenter?.ShowPauseMenu(onPositiveAction);
    }
}