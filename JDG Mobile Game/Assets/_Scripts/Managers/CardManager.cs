using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : Singleton<CardManager>
{
    [SerializeField] protected Transform canvas;

    /// <summary>
    /// Get attacker from Player's touch externally
    /// </summary>
    public InGameInvocationCard Attacker { private get; set; }

    /// <summary>
    /// Get opponent from Player's touch externally
    /// </summary>
    public InGameInvocationCard Opponent { private get; set; }

    [SerializeField] private PlayerCardManager player1CardManager;
    [SerializeField] private PlayerCardManager player2CardManager;

    /// <summary>
    /// Retrieves the card set for the current player.
    /// </summary>
    /// <returns>The card set of the current player.</returns>
    public PlayerCards GetCurrentPlayerCards()
    {
        return GameStateManager.Instance.IsP1Turn ? player1CardManager.playerCards : player2CardManager.playerCards;
    }

    /// <summary>
    /// Retrieves the card manager for the current player.
    /// </summary>
    /// <returns>The card manager of the current player.</returns>
    private PlayerCardManager GetCurrentPlayerCardManager()
    {
        return GameStateManager.Instance.IsP1Turn ? player1CardManager : player2CardManager;
    }

    /// <summary>
    /// Retrieves the card manager for the opposing player.
    /// </summary>
    /// <returns>The card manager of the opponent.</returns>
    private PlayerCardManager GetOpponentPlayerCardManager()
    {
        return GameStateManager.Instance.IsP1Turn ? player2CardManager : player1CardManager;
    }

    /// <summary>
    /// Executes a draw action for the current player.
    /// </summary>
    /// <param name="onNoCard">Action to perform when there are no cards.</param>
    public void Draw(UnityAction onNoCard)
    {
        GetCurrentPlayerCardManager().Draw(onNoCard);
    }

    /// <summary>
    /// Handles the end turn for the current player.
    /// </summary>
    public void HandleEndTurn()
    {
        GetCurrentPlayerCardManager().HandleEndTurn();
    }

    /// <summary>
    /// Checks if the attacker can execute an attack.
    /// </summary>
    /// <returns>True if an attack is possible; otherwise, false.</returns>
    public bool CanAttackerAttack()
    {
        var currentPlayerCards = GetCurrentPlayerCards();
        return Attacker != null && Attacker.CanAttack() && currentPlayerCards.ContainsCardInInvocation(Attacker);
    }

    /// <summary>
    /// Checks if the attacker has any actions available.
    /// </summary>
    /// <returns>True if actions are available; otherwise, false.</returns>
    public bool HasAttackerAction()
    {
        return Attacker != null && Attacker.HasAction();
    }

    /// <summary>
    /// Calculates the damage result from an attack.
    /// </summary>
    /// <returns>The computed damage value.</returns>
    public float ComputeDamageAttack()
    {
        if (Opponent == null || Attacker == null)
        {
            return 0f;
        }
        return Opponent.GetCurrentDefense() - Attacker.GetCurrentAttack();
    }

    /// <summary>
    /// Handles the attack logic between the attacker and opponent.
    /// </summary>
    public void HandleAttack()
    {
        if (Attacker == null || Opponent == null)
        {
            return;
        }
        Attacker.AttackTurnDone();
        if (Opponent.Title == CardNameMappings.CardNameMap[CardNames.Player])
        {
            PlayerManager.Instance.HandleAttackIfOpponentIsPlayer();
        }
        else
        {
            HandleAttackOverInvocation();
        }
    }

    private void HandleAttackOverInvocation()
    {

        var opponentAbilities = Opponent.Abilities;
        var attackerAbilities = Attacker.Abilities;
        var playerCard = GetCurrentPlayerCards();
        var opponentPlayerCard = GetOpponentPlayerCardManager().playerCards;
        foreach (var ability in opponentAbilities)
        {
            ability.OnCardAttacked(canvas, Opponent, Attacker, playerCard, opponentPlayerCard,
                PlayerManager.Instance.GetCurrentPlayerStatus(), PlayerManager.Instance.GetOpponentPlayerStatus());
        }

        foreach (var ability in attackerAbilities)
        {
            ability.OnAttackCard(Opponent, Attacker, playerCard, opponentPlayerCard);
        }
    }

    /// <summary>
    /// Executes the special action for the attacker.
    /// </summary>
    public void UseSpecialAction()
    {
        if (Attacker == null)
        {
            return;
        }
        var abilities = Attacker.Abilities;
        var playerCard = GetCurrentPlayerCards();
        var opponentPlayerCard = GetOpponentPlayerCardManager().playerCards;
        foreach (var ability in abilities)
        {
            ability.OnCardActionTouched(canvas, playerCard, opponentPlayerCard);
        }
    }

    /// <summary>
    /// Builds a list of cards that can be targeted for an attack.
    /// </summary>
    /// <returns>A list of valid target cards.</returns>
    public List<InGameCard> BuildInvocationCardsForAttack()
    {
        if (Attacker == null)
        {
            return new List<InGameCard>();
        }

        var opponentInvocationCards = GetOpponentPlayerCardManager().playerCards.InvocationCards;
        var attackPlayerEffectCard = GetCurrentPlayerCards().EffectCards;

        var validOpponentCards = FilterValidOpponentCards(opponentInvocationCards);

        if (HasAggroCard(validOpponentCards))
        {
            validOpponentCards = GetOnlyAggroCards(validOpponentCards);
        }
        else
        {
            RemoveCantBeAttackedCards(validOpponentCards);
            if (ShouldAddPlayerToTarget(attackPlayerEffectCard, validOpponentCards))
            {
                validOpponentCards.Add(GetOpponentPlayerCardManager().playerCards.Player);
            }
        }

        if (AttackerCanDirectAttack() && !validOpponentCards.Contains(GetOpponentPlayerCardManager().playerCards.Player))
        {
            validOpponentCards.Add(GetOpponentPlayerCardManager().playerCards.Player);
        }

        return validOpponentCards;
    }

    /// <summary>
    /// Filters the opponent's cards to only include valid cards.
    /// </summary>
    /// <param name="cards">A collection of opponent cards.</param>
    /// <returns>A list of valid opponent cards.</returns>
    private List<InGameCard> FilterValidOpponentCards(IEnumerable<InGameCard> cards)
    {
        return cards
            .Where(card => card != null && card.Title != null)
            .ToList();
    }

    /// <summary>
    /// Checks if the provided list contains any card with the Aggro property set.
    /// </summary>
    /// <param name="cards">A list of in-game cards.</param>
    /// <returns>True if any card has the Aggro property set, otherwise false.</returns>
    private bool HasAggroCard(List<InGameCard> cards)
    {
        return cards.Any(card => card is InGameInvocationCard invocationCard && invocationCard.Aggro);
    }

    /// <summary>
    /// Filters the provided list to return only cards with the Aggro property set.
    /// </summary>
    /// <param name="cards">A list of in-game cards.</param>
    /// <returns>A list of cards that have the Aggro property set.</returns>
    private List<InGameCard> GetOnlyAggroCards(List<InGameCard> cards)
    {
        return cards
            .Where(card => card is InGameInvocationCard invocationCard && invocationCard.Aggro)
            .ToList();
    }

    /// <summary>
    /// Removes all cards from the provided list that have the CantBeAttack property set.
    /// </summary>
    /// <param name="cards">A list of in-game cards.</param>
    private void RemoveCantBeAttackedCards(List<InGameCard> cards)
    {
        cards.RemoveAll(card => card is InGameInvocationCard invocationCard && invocationCard.CantBeAttack);
    }

    /// <summary>
    /// Determines if the player should be added as a target based on the provided cards and abilities.
    /// </summary>
    /// <param name="attackPlayerEffectCard">A collection of effect cards of the current player.</param>
    /// <param name="validOpponentCards">A list of valid opponent cards.</param>
    /// <returns>True if the player should be added as a target, otherwise false.</returns>
    private bool ShouldAddPlayerToTarget(ObservableCollection<InGameEffectCard> attackPlayerEffectCard, List<InGameCard> validOpponentCards)
    {
        return !validOpponentCards.Any() || attackPlayerEffectCard.Any(card =>
            card.EffectAbilities.Any(ability => ability is DirectAttackEffectAbility));
    }

    /// <summary>
    /// Checks if the attacker has the ability to directly attack.
    /// </summary>
    /// <returns>True if the attacker can perform a direct attack, otherwise false.</returns>
    private bool AttackerCanDirectAttack()
    {
        return Attacker.CanDirectAttack;
    }

    /// <summary>
    /// Checks if it's possible for the attacker to perform a special action.
    /// </summary>
    /// <returns>True if a special action is possible; otherwise, false.</returns>
    public bool IsSpecialActionPossible()
    {
        if (Attacker == null)
        {
            return false;
        }
        var playerCard = GetCurrentPlayerCards();
        return Attacker.Abilities.TrueForAll(ability =>
            ability.IsActionPossible(playerCard)) && !Attacker.CancelEffect;
    }


    /// <summary>
    /// Handles actions and effects at the start of a turn.
    /// </summary>
    public void OnTurnStart()
    {
        var playerCards = GetCurrentPlayerCards();
        var opponentPlayerCards = GetOpponentPlayerCardManager().playerCards;
        playerCards.ResetInvocationCardNewTurn();
        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
        var copyInvocationCards = playerCards.InvocationCards.ToList();
        var copyOpponentInvocationCards = opponentPlayerCards.InvocationCards.ToList();

        var copyEffectCards = playerCards.EffectCards.ToList();
        var copyOpponentEffectCards = opponentPlayerCards.EffectCards.ToList();

        ApplyInvocationOnTurnStart(copyInvocationCards, playerCards, opponentPlayerCards);
        ApplyInvocationOnTurnStart(copyOpponentInvocationCards, opponentPlayerCards, playerCards);

        ApplyEffectOnTurnStart(copyEffectCards, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCards);
        ApplyEffectOnTurnStart(copyOpponentEffectCards, opponentPlayerStatus, opponentPlayerCards, playerStatus, playerCards);

        ApplyFieldOnTurnStart(playerCards, playerStatus);
    }

    /// <summary>
    /// Applies field card effects at the start of a turn.
    /// </summary>
    /// <param name="playerCards">The player's card set.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    private void ApplyFieldOnTurnStart(PlayerCards playerCards, PlayerStatus playerStatus)
    {
        if (playerCards.FieldCard != null)
        {
            foreach (var fieldCardFieldAbility in playerCards.FieldCard.FieldAbilities)
            {
                fieldCardFieldAbility.OnTurnStart(canvas, playerCards, playerStatus);
            }
        }
    }

    /// <summary>
    /// Applies effect card actions at the start of a turn.
    /// </summary>
    /// <param name="copyEffectCards">A copy of the effect cards to process.</param>
    /// <param name="playerStatus">The current status of the player.</param>
    /// <param name="playerCards">The player's card set.</param>
    /// <param name="opponentPlayerStatus">The current status of the opponent.</param>
    /// <param name="opponentPlayerCards">The opponent's card set.</param>
    private void ApplyEffectOnTurnStart(List<InGameEffectCard> copyEffectCards, PlayerStatus playerStatus, PlayerCards playerCards, PlayerStatus opponentPlayerStatus,
        PlayerCards opponentPlayerCards)
    {
        foreach (var effectCardAbility in copyEffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectCardAbility.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCards);
        }
    }

    /// <summary>
    /// Applies invocation card actions at the start of a turn.
    /// </summary>
    /// <param name="copyInvocationCards">A copy of the invocation cards to process.</param>
    /// <param name="playerCards">The player's card set.</param>
    /// <param name="opponentPlayerCards">The opponent's card set.</param>
    private void ApplyInvocationOnTurnStart(List<InGameInvocationCard> copyInvocationCards, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        foreach (var invocationCard in copyInvocationCards)
        {
            foreach (var invocationCardAbility in invocationCard.Abilities)
            {
                invocationCardAbility.OnTurnStart(canvas, playerCards, opponentPlayerCards);
            }

            if (invocationCard.EquipmentCard != null)
            {
                foreach (var equipmentCardEquipmentAbility in invocationCard.EquipmentCard.EquipmentAbilities)
                {
                    equipmentCardEquipmentAbility.OnTurnStart(invocationCard);
                }
            }
        }
    }
}