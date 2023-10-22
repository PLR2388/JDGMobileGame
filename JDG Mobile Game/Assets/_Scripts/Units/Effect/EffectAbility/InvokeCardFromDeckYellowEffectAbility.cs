using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;

/// <summary>
/// Provides an effect ability to invoke a card either from the deck or yellow trash.
/// </summary>
public class InvokeCardFromDeckYellowEffectAbility : EffectAbility
{
    private readonly bool fromYellowTrash;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeCardFromDeckYellowEffectAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the effect ability.</param>
    /// <param name="description">The description of the effect ability.</param>
    /// <param name="fromYellowTrash">If set to true, cards will be invoked from the yellow trash; otherwise, they will be invoked from the deck.</param>
    public InvokeCardFromDeckYellowEffectAbility(EffectAbilityName name, string description,
        bool fromYellowTrash = false)
    {
        Name = name;
        Description = description;
        this.fromYellowTrash = fromYellowTrash;
    }

    /// <summary>
    /// Determines if the effect can be used based on the current state of the player's and opponent's cards.
    /// </summary>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent player.</param>
    /// <returns>True if the effect can be used; otherwise, false.</returns>
    public override bool CanUseEffect(PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus opponentPlayerStatus)
    {
        var hasSpaces = playerCards.InvocationCards.Count < 4;
        if (fromYellowTrash)
        {
            return hasSpaces && playerCards.YellowCards.Count(elt => elt.Type == CardType.Invocation) > 0;
        }
        return hasSpaces;
    }

    /// <summary>
    /// Applies the effect on the players.
    /// </summary>
    /// <param name="canvas">The current UI canvas.</param>
    /// <param name="playerCards">The cards of the player.</param>
    /// <param name="opponentPlayerCard">The cards of the opponent.</param>
    /// <param name="playerStatus">The status of the player.</param>
    /// <param name="opponentStatus">The status of the opponent.</param>
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCard, PlayerStatus playerStatus,
        PlayerStatus opponentStatus)
    {
        base.ApplyEffect(canvas, playerCards, opponentPlayerCard, playerStatus, opponentStatus);

        // Only from Yellow Trash right now
        if (fromYellowTrash)
        {
            var config = new CardSelectorConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOICE_INVOKE),
                playerCards.YellowCards.ToList(),
                showOkButton: true,
                okAction: (card) =>
                {
                    if (card is InGameInvocationCard invocationCard)
                    {
                        playerCards.YellowCards.Remove(invocationCard);
                        playerCards.InvocationCards.Add(invocationCard);
                    }
                    else
                    {
                        var config = new MessageBoxConfig(
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_MUST_CHOOSE_CARD),
                            showOkButton: true
                        );
                        MessageBox.Instance.CreateMessageBox(
                            canvas,
                            config
                        );
                    }
                }
            );
            CardSelector.Instance.CreateCardSelection(canvas, config);
        }
    }
}