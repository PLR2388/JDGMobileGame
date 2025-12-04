using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Presentation.Presenters;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Phase 5: UIManager is being decomposed into focused presenters (MVP pattern).
/// This class now delegates to CardDisplayPresenter, DialogPresenter, and CardSelectorPresenter.
/// UIManager will eventually be removed once all callsites migrate to the new presenters.
/// </summary>
[System.Obsolete("UIManager is being phased out. Use CardDisplayPresenter, DialogPresenter, and CardSelectorPresenter directly via dependency injection instead. This singleton will be removed in a future phase.")]
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject bigImageCard;
    [SerializeField] protected GameObject nextPhaseButton;
    [SerializeField] protected Transform canvas;

    private Image bigImageCardImage;

    // Phase 5: Presenter instances
    private CardDisplayPresenter _cardDisplayPresenter;
    private DialogPresenter _dialogPresenter;
    private CardSelectorPresenter _cardSelectorPresenter;

    /// <summary>
    /// Initialize component references and presenters.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        bigImageCardImage = bigImageCard.GetComponent<Image>();

        // Phase 5: Create presenter instances
        InitializePresenters();
    }

    private void InitializePresenters()
    {
        // Phase 5: Create simple presenter instances (not MonoBehaviours)
        _cardDisplayPresenter = new CardDisplayPresenter(bigImageCard);
        _dialogPresenter = new DialogPresenter(canvas);
        _cardSelectorPresenter = new CardSelectorPresenter(canvas, nextPhaseButton);
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