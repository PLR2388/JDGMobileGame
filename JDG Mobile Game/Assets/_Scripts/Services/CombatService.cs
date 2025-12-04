using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using UnityEngine;

/// <summary>
/// Implementation of ICombatService.
/// Manages combat operations including attack validation, execution, and targeting.
///
/// Note: This service is in the default assembly because it depends on legacy types
/// (InGameCard, InGameInvocationCard, PlayerCards, etc.). It will be moved to
/// JDG.Infrastructure once these types are fully refactored.
///
/// Part of Phase 4 migration - decomposes CardManager god class.
/// </summary>
public class CombatService : ICombatService
{
    private readonly ICardCollectionService _cardCollectionService;
    private readonly Transform _canvas;

    public InGameInvocationCard Attacker { get; set; }
    public InGameInvocationCard Opponent { get; set; }

    public CombatService(ICardCollectionService cardCollectionService, Transform canvas)
    {
        _cardCollectionService = cardCollectionService;
        _canvas = canvas;
    }

    public bool CanAttackerAttack()
    {
        if (Attacker == null)
            return false;

        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();
        return Attacker.CanAttack() && currentPlayerCards.ContainsCardInInvocation(Attacker);
    }

    public bool HasAttackerAction()
    {
        return Attacker != null && Attacker.HasAction();
    }

    public float ComputeDamageAttack()
    {
        if (Opponent == null || Attacker == null)
            return 0f;

        return Opponent.GetCurrentDefense() - Attacker.GetCurrentAttack();
    }

    public void HandleAttack()
    {
        if (Attacker == null || Opponent == null)
            return;

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

    public List<InGameCard> BuildValidTargets()
    {
        if (Attacker == null)
            return new List<InGameCard>();

        var opponentCards = _cardCollectionService.GetOpponentPlayerCards();
        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();

        var validTargets = FilterValidOpponentCards(opponentCards.InvocationCards);

        if (HasAggroCard(validTargets))
        {
            validTargets = GetOnlyAggroCards(validTargets);
        }
        else
        {
            RemoveCantBeAttackedCards(validTargets);

            if (ShouldAddPlayerToTarget(currentPlayerCards.EffectCards, validTargets))
            {
                validTargets.Add(opponentCards.Player);
            }
        }

        if (AttackerCanDirectAttack() && !validTargets.Contains(opponentCards.Player))
        {
            validTargets.Add(opponentCards.Player);
        }

        return validTargets;
    }

    public void UseSpecialAction()
    {
        if (Attacker == null)
            return;

        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentCards = _cardCollectionService.GetOpponentPlayerCards();

        foreach (var ability in Attacker.Abilities)
        {
            ability.OnCardActionTouched(_canvas, playerCards, opponentCards);
        }
    }

    public bool IsSpecialActionPossible()
    {
        if (Attacker == null)
            return false;

        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        return Attacker.Abilities.TrueForAll(ability => ability.IsActionPossible(playerCards))
               && !Attacker.CancelEffect;
    }

    // Private helper methods

    private void HandleAttackOverInvocation()
    {
        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentCards = _cardCollectionService.GetOpponentPlayerCards();
        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentStatus = PlayerManager.Instance.GetOpponentPlayerStatus();

        foreach (var ability in Opponent.Abilities)
        {
            ability.OnCardAttacked(_canvas, Opponent, Attacker, playerCards, opponentCards,
                playerStatus, opponentStatus);
        }

        foreach (var ability in Attacker.Abilities)
        {
            ability.OnAttackCard(Opponent, Attacker, playerCards, opponentCards);
        }
    }

    private List<InGameCard> FilterValidOpponentCards(System.Collections.ObjectModel.ObservableCollection<InGameCard> cards)
    {
        return cards.Where(card => card != null && card.Title != null).ToList();
    }

    private bool HasAggroCard(List<InGameCard> cards)
    {
        return cards.Any(card => card is InGameInvocationCard invocationCard && invocationCard.Aggro);
    }

    private List<InGameCard> GetOnlyAggroCards(List<InGameCard> cards)
    {
        return cards.Where(card => card is InGameInvocationCard invocationCard && invocationCard.Aggro).ToList();
    }

    private void RemoveCantBeAttackedCards(List<InGameCard> cards)
    {
        cards.RemoveAll(card => card is InGameInvocationCard invocationCard && invocationCard.CantBeAttack);
    }

    private bool ShouldAddPlayerToTarget(
        System.Collections.ObjectModel.ObservableCollection<InGameEffectCard> effectCards,
        List<InGameCard> validTargets)
    {
        return !validTargets.Any() ||
               effectCards.Any(card => card.EffectAbilities.Any(ability => ability is DirectAttackEffectAbility));
    }

    private bool AttackerCanDirectAttack()
    {
        return Attacker.CanDirectAttack;
    }
}
