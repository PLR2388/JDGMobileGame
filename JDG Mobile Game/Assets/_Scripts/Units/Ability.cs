using System.Linq;
using _Scripts.Units.Invocation;
using UnityEngine;

/// <summary>
/// Represents a set of ability names that can be used in the game.
/// </summary>
public enum AbilityName
{
    CanOnlyAttackItself,
    AddSpatialFromDeck,
    SacrificeArchibaldVonGrenier,
    CantBeAttackIfComics,
    CantLiveWithoutBenzaieOrBenzaieJeune,
    GiveAtkDefToComics,
    SendAllCardToHands,
    SacrificeBenzaieJeune,
    GetNounoursFromDeck,
    SacrificeJoueurDuGrenier,
    GetPetitePortionDeRizFromDeck,
    InvokeTentacules,
    GetLycéeMagiqueGeorgesPompidouFromDeck,
    SacrificeSebDuGrenierOnHardCornerForAtkDef,
    Win1Atk1DefDeveloper,
    Sacrifice3Atk3Def,
    ChangeFieldWithFieldFromDeck,
    Win1ATK1DefJaponWith2ATK2DEFCondition,
    InvokeDresseurBidulmon,
    GetZozanKebabFromDeck,
    SacrificeWizard,
    GetConvocationAuLyceeFromDeck,
    ProtectedBehindStarlightUnicorn,
    GetCanardSignal,
    SacrificeDeveloper3Atk3Def,
    SacrificeHardCorner3Atk3Def,
    CantBeAttackKill,
    ComesBackFromDeath,
    Sacrifice2Japan,
    DestroyFieldATK,
    KillOpponentInvocation,
    CantLiveWithoutJDG,
    GetForetElfesSylvains,
    InvokeSebOrJDG,
    CantLiveWithoutComics,
    Sacrifice2Incarnation,
    DestroyFieldDEF,
    GetBenzaieJeuneFromDeck,
    GetEquipmentCardWithoutAttack,
    SacrificeGranolax,
    SacrificeJDGOnStudioDevForAtkDef,
    CantLiveWithoutHuman,
    CopyBenzaieJeune,
    SurviveOneTurn,
    GiveDeathWhenDie,
    ProtectBehindGreaterDef,
    SacrificeSebDuGrenier,
    Win1Atk1DefFistiland,
    SacrificeClicheRaciste,
    KillEnemyIfDestroy,
    SacrificeToInvoke,
    GetPatronInfogramesFromDeckYellowTrash,
    CantLiveWithoutGranolaxOrMechaGranolax,
    SkipOpponentAttackEveryTurn,
    ComesBackFromDeath5Times,
    CantLiveWithoutJapon,
    Draw2Cards,
    GiveAktDefToRpgMember,
    GiveAktDefToFistilandMember,
    Default
}

/// <summary>
/// Provides a base class for all abilities in the game. 
/// Each ability has a name, description, and can have associated effects or actions.
/// </summary>
public abstract class Ability
{
    /// <summary>
    /// The name of the ability.
    /// </summary>
    public AbilityName Name { get; set; }
    
    /// <summary>
    /// A description of the ability.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Indicates whether the ability has an associated action.
    /// </summary>
    public bool IsAction { get; protected set; }

    /// <summary>
    /// The invocation card associated with this ability.
    /// </summary>
    protected InGameInvocationCard invocationCard;

    /// <summary>
    /// Assigns or updates the invocation card for this ability.
    /// </summary>
    public InGameInvocationCard InvocationCard
    {
        set => invocationCard = value;
    }

    /// <summary>
    /// Applies the ability's effect when an invocation card is placed on the field without special invocation.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's cards.</param>
    public virtual void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    /// <summary>
    /// Cancels the ability's effect.
    /// </summary>
    /// <param name="playerCards">The current player's cards.</param>
    public virtual void CancelEffect(PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Reactivates the ability's effect.
    /// </summary>
    /// <param name="playerCards">The current player's cards.</param>
    public virtual void ReactivateEffect(PlayerCards playerCards)
    {
        
    }

    /// <summary>
    /// Performs operations related to the ability at the start of a turn.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's cards.</param>
    public virtual void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
    }

    /// <summary>
    /// Responds to the event of a new invocation card being added to the field.
    /// </summary>
    /// <param name="newCard">The new invocation card added.</param>
    /// <param name="playerCards">The current player's cards.</param>
    public virtual void OnCardAdded(InGameInvocationCard newCard, PlayerCards playerCards)
    {
    }

    /// <summary>
    /// Responds to the event of an invocation card being removed from the field.
    /// </summary>
    /// <param name="removeCard">The invocation card being removed.</param>
    /// <param name="playerCards">The current player's cards.</param>
    public virtual void OnCardRemove(InGameInvocationCard removeCard, PlayerCards playerCards)
    {
    }

    /// <summary>
    /// Handles events related to one invocation card attacking another.
    /// Specific use case for Sandrine (KillBothCardsIfAttackAbility).
    /// </summary>
    /// <param name="attackedCard">The card being attacked.</param>
    /// <param name="attacker">The card initiating the attack.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's cards.</param>
    public virtual void OnAttackCard(InGameInvocationCard attackedCard, InGameInvocationCard attacker, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
    }

    /// <summary>
    /// Handles the pre-attack logic for invocation cards.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="attackedCard">The card being attacked.</param>
    /// <param name="attacker">The card initiating the attack.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's cards.</param>
    /// <param name="currentPlayerStatus">The status of the current player.</param>
    /// <param name="opponentPlayerStatus">The status of the opponent.</param>
    public virtual void OnCardAttacked(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards,
        PlayerStatus currentPlayerStatus, PlayerStatus opponentPlayerStatus)
    {
        float resultAttack = attackedCard.Defense - attacker.Attack;
        if (resultAttack > 0)
        {
            HandlePositiveAttackResult(canvas, attacker, playerCards, opponentPlayerCards, currentPlayerStatus, resultAttack);
        }
        else if (resultAttack == 0)
        {
            HandleNeutralAttackResult(canvas, attackedCard, attacker, playerCards, opponentPlayerCards);
        }
        else
        {
            HandleNegativeAttackResult(canvas, attackedCard, playerCards, opponentPlayerCards, opponentPlayerStatus, resultAttack);
        }
    }

    /// <summary>
    /// Handles the outcome of a negative attack result. If the attacked card is not protected by equipment, 
    /// it is subjected to potential death and the opponent's player status may change based on the result of the attack.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="attackedCard">The card that was attacked.</param>
    /// <param name="playerCards">The cards of the player performing the attack.</param>
    /// <param name="opponentPlayerCards">The cards of the player being attacked.</param>
    /// <param name="opponentPlayerStatus">The status of the player being attacked.</param>
    /// <param name="resultAttack">The resultant value of the attack calculation.</param>
    private void HandleNegativeAttackResult(Transform canvas, InGameInvocationCard attackedCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards, PlayerStatus opponentPlayerStatus, float resultAttack)
    {
        if (!IsEquipmentCardProtect(attackedCard, opponentPlayerCards))
        {
            var value = OnCardDeath(canvas, attackedCard, opponentPlayerCards, playerCards);
            if (value)
            {
                opponentPlayerStatus.ChangePv(resultAttack);
            }
        }
    }

    /// <summary>
    /// Handles the outcome of a neutral attack result. Determines if either the attacker or the attacked cards 
    /// are protected by equipment and processes potential deaths accordingly.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="attackedCard">The card that was attacked.</param>
    /// <param name="attacker">The card initiating the attack.</param>
    /// <param name="playerCards">The cards of the player performing the attack.</param>
    /// <param name="opponentPlayerCards">The cards of the player being attacked.</param>
    private void HandleNeutralAttackResult(Transform canvas, InGameInvocationCard attackedCard,
        InGameInvocationCard attacker, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        var isProtectedAttacker = IsEquipmentCardProtect(attacker, playerCards);
        var isProtectedAttacked = IsEquipmentCardProtect(attackedCard, opponentPlayerCards);
        if (!isProtectedAttacked)
        {
            OnCardDeath(canvas, attackedCard, opponentPlayerCards, playerCards);
        }

        if (!isProtectedAttacker)
        {
            OnCardDeath(canvas, attacker, playerCards, opponentPlayerCards);
        }
    }

    /// <summary>
    /// Handles the outcome of a positive attack result. If the attacker card is not protected by equipment, 
    /// it is subjected to potential death and the current player's status may change based on the result of the attack.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="attacker">The card initiating the attack.</param>
    /// <param name="playerCards">The cards of the player performing the attack.</param>
    /// <param name="opponentPlayerCards">The cards of the player being attacked.</param>
    /// <param name="currentPlayerStatus">The status of the player performing the attack.</param>
    /// <param name="resultAttack">The resultant value of the attack calculation.</param>
    private void HandlePositiveAttackResult(Transform canvas, InGameInvocationCard attacker, PlayerCards playerCards,
        PlayerCards opponentPlayerCards, PlayerStatus currentPlayerStatus, float resultAttack)
    {
        var isProtected = IsEquipmentCardProtect(attacker, playerCards);

        if (isProtected == false)
        {
            var value = OnCardDeath(canvas, attacker, playerCards, opponentPlayerCards);
            if (value)
            {
                currentPlayerStatus.ChangePv(-resultAttack);
            }
        }
    }

    /// <summary>
    /// Determines if the given invocation card is protected by an equipment card. Protection is based on 
    /// any associated equipment ability that prevents the card's destruction.
    /// </summary>
    /// <param name="attacker">The invocation card to check for protection.</param>
    /// <param name="playerCards">The cards of the associated player.</param>
    /// <returns>True if the invocation card is protected by equipment; otherwise, false.</returns>
    private bool IsEquipmentCardProtect(InGameInvocationCard attacker, PlayerCards playerCards)
    {
        var equipmentCard = attacker.EquipmentCard;
        var isProtected = false;
        if (equipmentCard != null)
        {
            isProtected = equipmentCard.EquipmentAbilities.Any(ability => ability.OnInvocationPreDestroy(attacker, playerCards) == false);
        }

        return isProtected;
    }

    /// <summary>
    /// Executes actions associated with the death of a card that has a particular ability.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="deadCard">The card that died.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentPlayerCards">The opponent's cards.</param>
    /// <returns>True if certain conditions are met; otherwise, false.</returns>
    public virtual bool OnCardDeath(Transform canvas, InGameInvocationCard deadCard, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        if (playerCards.YellowCards.Contains(deadCard)) return false;
        var equipmentCard = deadCard.EquipmentCard;
        if (equipmentCard != null)
        {
            foreach (var equipmentCardEquipmentAbility in equipmentCard.EquipmentAbilities)
            {
                equipmentCardEquipmentAbility.RemoveEffect(deadCard, playerCards, opponentPlayerCards);
            }
            playerCards.YellowCards.Add(equipmentCard);
            deadCard.EquipmentCard = null;
        }

        deadCard.IncrementNumberDeaths();

        playerCards.InvocationCards.Remove(deadCard);
        playerCards.YellowCards.Add(deadCard);
        return true;
    }

    /// <summary>
    /// Determines if the ability's action can be executed based on the state of the invocation card.
    /// </summary>
    /// <param name="playerCards">The current player's cards.</param>
    /// <returns>True if the action is possible; otherwise, false.</returns>
    public virtual bool IsActionPossible(PlayerCards playerCards)
    {
        return !invocationCard.CancelEffect;
    }

    /// <summary>
    /// Executes actions when an invocation card's action is touched or triggered.
    /// </summary>
    /// <param name="canvas">The current game canvas.</param>
    /// <param name="playerCards">The current player's cards.</param>
    /// <param name="opponentCards">The opponent's cards.</param>
    public virtual void OnCardActionTouched(Transform canvas, PlayerCards playerCards,
        PlayerCards opponentCards)
    {
    }
}