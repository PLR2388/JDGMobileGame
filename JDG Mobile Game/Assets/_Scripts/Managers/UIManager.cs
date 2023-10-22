using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manages the user interface elements and interactions for the game.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject bigImageCard;
    [SerializeField] protected GameObject nextPhaseButton;
    [SerializeField] protected Transform canvas;

    private Image bigImageCardImage;

    /// <summary>
    /// Initialize component references.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        bigImageCardImage = bigImageCard.GetComponent<Image>();
    }

    /// <summary>
    /// Displays the given card on the large card viewer.
    /// </summary>
    /// <param name="card">Card to be displayed.</param>
    public void DisplayCardOnLargeView(InGameCard card)
    {
        if (!bigImageCard || !bigImageCardImage) return;
        bigImageCard.SetActive(true);
        bigImageCardImage.material = card.MaterialCard;
    }

    /// <summary>
    /// Displays a message box to inform the user about the available opponents for invocation.
    /// </summary>
    /// <param name="invocationCards">List of invocable cards.</param>
    /// <param name="positiveAction">Action on positive button click.</param>
    /// <param name="negativeAction">Action on negative button click.</param>
    public void DisplayOpponentAvailableMessageBox(
        List<InGameCard> invocationCards,
        UnityAction<InGameInvocationCard> positiveAction,
        UnityAction negativeAction)
    {
        nextPhaseButton.SetActive(false);

        if (invocationCards.Count > 0)
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT),
                invocationCards,
                showNegativeButton: true,
                showPositiveButton: true,
                positiveAction: (invocationCard) =>
                {
                    positiveAction(invocationCard as InGameInvocationCard);
                    nextPhaseButton.SetActive(true);
                },
                negativeAction: () =>
                {
                    negativeAction();
                    nextPhaseButton.SetActive(true);
                }
            );
            CardSelector.Instance.CreateCardSelection(
                canvas,
                config
            );
        }
        else
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_CANNOT_ATTACK_MESSAGE),
                showOkButton: true
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
    
    /// <summary>
    /// Hides the large card viewer.
    /// </summary>
    public void HideBigImage()
    {
        bigImageCard.SetActive(false);
    }

    /// <summary>
    /// Displays a pause menu with given positive action.
    /// </summary>
    /// <param name="onPositiveAction">Action to execute on positive button click.</param>
    public void DisplayPauseMenu(UnityAction onPositiveAction)
    {
        MessageBoxConfig config = new MessageBoxConfig(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PAUSE_TITLE),
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.PAUSE_MESSAGE),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: onPositiveAction
        );
        MessageBox.Instance.CreateMessageBox(
            canvas,
            config
        );
    }
}