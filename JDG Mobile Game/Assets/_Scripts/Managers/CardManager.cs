using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.InvocationCards;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : Singleton<CardManager>
{
    protected InGameCard player;
    [SerializeField] protected InvocationCard playerInvocationCard;
    [SerializeField] protected Transform canvas;

    private PlayerCards playerCards1;
    private PlayerCards playerCards2;
    protected InGameInvocationCard attacker;
    protected InGameInvocationCard opponent;

    [SerializeField] protected GameObject p1;

    [SerializeField] protected GameObject p2;

    public InGameCard Player => player;

    public PlayerCards GetCurrentPlayerCards()
    {
        return GameStateManager.Instance.IsP1Turn ? playerCards1 : playerCards2;
    }

    public PlayerCards GetOpponentPlayerCards()
    {
        return GameStateManager.Instance.IsP1Turn ? playerCards2 : playerCards1;
    }

    protected override void Awake()
    {
        base.Awake();
        player = InGameCard.CreateInGameCard(playerInvocationCard, CardOwner.Player2);
        playerCards1 = p1.GetComponent<PlayerCards>();
        playerCards2 = p2.GetComponent<PlayerCards>();
    }

    public void SetAttacker(InGameInvocationCard newAttacker)
    {
        attacker = newAttacker;
    }

    public void SetOpponent(InGameInvocationCard newOpponent)
    {
        opponent = newOpponent;
    }

    public bool CanAttackerAttack()
    {
        return attacker.CanAttack() && GetCurrentPlayerCards().ContainsCardInInvocation(attacker);
    }

    public bool HasAttackerAction()
    {
        return attacker.HasAction();
    }

    public float ComputeDamageAttack()
    {
        return opponent.GetCurrentDefense() - attacker.GetCurrentAttack();
    }

    public void Draw(UnityAction onNoCard)
    {
        var playerCards = GetCurrentPlayerCards();
        if (playerCards.SkipCurrentDraw)
        {
            playerCards.SkipCurrentDraw = false;
        }
        else
        {
            var size = playerCards.deck.Count;
            if (size > 0)
            {
                var c = playerCards.deck[size - 1];
                playerCards.handCards.Add(c);
                playerCards.deck.RemoveAt(size - 1);
            }
            else
            {
                onNoCard();
            }
        }
    }

    public void HandleAttack()
    {
        if (opponent.Title == "Player")
        {
            PlayerManager.Instance.HandleAttackIfOpponentIsPlayer();
        }
        else
        {
            var opponentAbilities = opponent.Abilities;
            var attackerAbilities = attacker.Abilities;
            attacker.AttackTurnDone();
            var playerCard = CardManager.Instance.GetCurrentPlayerCards();
            var opponentPlayerCard = CardManager.Instance.GetOpponentPlayerCards();
            foreach (var ability in opponentAbilities)
            {
                ability.OnCardAttacked(canvas, opponent, attacker, playerCard, opponentPlayerCard,
                    PlayerManager.Instance.GetCurrentPlayerStatus(), PlayerManager.Instance.GetOpponentPlayerStatus());
            }

            foreach (var ability in attackerAbilities)
            {
                ability.OnAttackCard(opponent, attacker, playerCard, opponentPlayerCard);
            }
        }
    }
    
    public void UseSpecialAction()
    {
        var abilities = attacker.Abilities;
        foreach (var ability in abilities)
        {
            ability.OnCardActionTouched(canvas, GetCurrentPlayerCards(), GetOpponentPlayerCards());
        }
    }

    public List<InGameCard> BuildInvocationCardsForAttack()
    {
        var invocationCards = GetOpponentPlayerCards().invocationCards;
        var attackPlayerEffectCard = GetCurrentPlayerCards().effectCards;
        var notEmptyOpponent = invocationCards.Where(t => t != null && t.Title != null).Cast<InGameCard>().ToList();

        if (notEmptyOpponent.Count == 0)
        {
            notEmptyOpponent.Add(CardManager.Instance.Player);
        }
        else
        {
            var newList = (from card in notEmptyOpponent
                let invocationCard = card as InGameInvocationCard
                where invocationCard?.Aggro == true
                select card).ToList();

            if (newList.Count > 0)
            {
                notEmptyOpponent = newList;
            }
            else
            {
                for (var j = notEmptyOpponent.Count - 1; j >= 0; j--)
                {
                    var invocationCard = (InGameInvocationCard)notEmptyOpponent[j];

                    if (invocationCard.CantBeAttack)
                    {
                        notEmptyOpponent.Remove(invocationCard);
                    }
                }

                if (notEmptyOpponent.Count == 0 || attackPlayerEffectCard.Any(card =>
                        card.EffectAbilities.Any(ability => ability is DirectAttackEffectAbility)))
                {
                    notEmptyOpponent.Add(CardManager.Instance.Player);
                }
            }
        }

        if (!notEmptyOpponent.Contains(CardManager.Instance.Player) && attacker.CanDirectAttack)
        {
            notEmptyOpponent.Add(CardManager.Instance.Player);
        }

        return notEmptyOpponent;
    }

    public bool IsSpecialActionPossible()
    {
        return attacker.Abilities.TrueForAll(ability =>
            ability.IsActionPossible(GetCurrentPlayerCards())) && !attacker.CancelEffect;
    }

    public void OnTurnStart()
    {
        var playerCards = GetCurrentPlayerCards();
        playerCards.ResetInvocationCardNewTurn();
        var opponentPlayerCards = GetOpponentPlayerCards();
        var playerStatus = PlayerManager.Instance.GetCurrentPlayerStatus();
        var opponentPlayerStatus = PlayerManager.Instance.GetOpponentPlayerStatus();
        var copyInvocationCards = playerCards.invocationCards.ToList();
        var copyOpponentInvocationCards = opponentPlayerCards.invocationCards.ToList();

        var copyEffectCards = playerCards.effectCards.ToList();
        var copyOpponentEffectCards = opponentPlayerCards.effectCards.ToList();

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

        foreach (var invocationCard in copyOpponentInvocationCards)
        {
            foreach (var invocationCardAbility in invocationCard.Abilities)
            {
                invocationCardAbility.OnTurnStart(canvas, opponentPlayerCards, playerCards);
            }

            if (invocationCard.EquipmentCard != null)
            {
                foreach (var equipmentCardEquipmentAbility in invocationCard.EquipmentCard.EquipmentAbilities)
                {
                    equipmentCardEquipmentAbility.OnTurnStart(invocationCard);
                }
            }
        }

        foreach (var effectCardAbility in copyEffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectCardAbility.OnTurnStart(canvas, playerStatus, playerCards, opponentPlayerStatus, opponentPlayerCards);
        }

        foreach (var effectCardAbility in copyOpponentEffectCards.SelectMany(effectCard => effectCard.EffectAbilities))
        {
            effectCardAbility.OnTurnStart(canvas, opponentPlayerStatus, opponentPlayerCards, opponentPlayerStatus,
                playerCards);
        }

        if (playerCards.FieldCard != null)
        {
            foreach (var fieldCardFieldAbility in playerCards.FieldCard.FieldAbilities)
            {
                fieldCardFieldAbility.OnTurnStart(canvas, playerCards, playerStatus);
            }
        }
    }
}