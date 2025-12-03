using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using JDG.Application.Services;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.DI;
using JDG.Infrastructure.Services;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Phase 4: CardManager is being decomposed into focused services.
/// This class now delegates to CardCollectionService, CombatService, TurnService, and CardDrawService.
/// CardManager will eventually be removed once all callsites migrate to the new services.
/// </summary>
[System.Obsolete("CardManager is being phased out. Use ICardCollectionService, ICombatService, ITurnService, and ICardDrawService via dependency injection instead. This singleton will be removed in a future phase.")]
public class CardManager : Singleton<CardManager>
{
    [SerializeField] protected Transform canvas;

    // Phase 2: Temporary bridge to GameStateService during migration
    private GameStateService GameStateService => ServiceLocator.Get<GameStateService>();
    private bool IsP1Turn => GameStateService.CurrentPlayer == PlayerId.Player1;

    [SerializeField] private PlayerCardManager player1CardManager;
    [SerializeField] private PlayerCardManager player2CardManager;

    // Phase 4: New service instances (created on Awake)
    private ICardCollectionService _cardCollectionService;
    private ICombatService _combatService;
    private ITurnService _turnService;
    private ICardDrawService _cardDrawService;

    /// <summary>
    /// Get attacker from Player's touch externally
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    public InGameInvocationCard Attacker
    {
        get => _combatService?.Attacker;
        set { if (_combatService != null) _combatService.Attacker = value; }
    }

    /// <summary>
    /// Get opponent from Player's touch externally
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    public InGameInvocationCard Opponent
    {
        get => _combatService?.Opponent;
        set { if (_combatService != null) _combatService.Opponent = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        // Phase 4: Initialize service instances
        _cardCollectionService = new CardCollectionService(
            GameStateService,
            player1CardManager,
            player2CardManager);

        _cardDrawService = new CardDrawService(
            GameStateService,
            player1CardManager,
            player2CardManager);

        _turnService = new TurnService(
            GameStateService,
            player1CardManager,
            player2CardManager,
            canvas);

        _combatService = new CombatService(_cardCollectionService, canvas);
    }

    /// <summary>
    /// Retrieves the card set for the current player.
    /// Phase 4: Now delegates to CardCollectionService
    /// </summary>
    /// <returns>The card set of the current player.</returns>
    public PlayerCards GetCurrentPlayerCards()
    {
        return _cardCollectionService.GetCurrentPlayerCards();
    }

    /// <summary>
    /// Retrieves the card set for the opponent player.
    /// Phase 4: Now delegates to CardCollectionService
    /// </summary>
    /// <returns>The card set of the opponent player.</returns>
    public PlayerCards GetOpponentPlayerCards()
    {
        return _cardCollectionService.GetOpponentPlayerCards();
    }

    /// <summary>
    /// Retrieves the card manager for the current player.
    /// </summary>
    /// <returns>The card manager of the current player.</returns>
    private PlayerCardManager GetCurrentPlayerCardManager()
    {
        return IsP1Turn ? player1CardManager : player2CardManager;
    }

    /// <summary>
    /// Retrieves the card manager for the opposing player.
    /// </summary>
    /// <returns>The card manager of the opponent.</returns>
    private PlayerCardManager GetOpponentPlayerCardManager()
    {
        return IsP1Turn ? player2CardManager : player1CardManager;
    }

    /// <summary>
    /// Executes a draw action for the current player.
    /// Phase 4: Now delegates to CardDrawService
    /// </summary>
    /// <param name="onNoCard">Action to perform when there are no cards.</param>
    public void Draw(UnityAction onNoCard)
    {
        _cardDrawService.DrawCard(onNoCard);
    }

    /// <summary>
    /// Handles the end turn for the current player.
    /// Phase 4: Now delegates to TurnService
    /// </summary>
    public void HandleEndTurn()
    {
        _turnService.HandleEndTurn();
    }

    /// <summary>
    /// Checks if the attacker can execute an attack.
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    /// <returns>True if an attack is possible; otherwise, false.</returns>
    public bool CanAttackerAttack()
    {
        return _combatService.CanAttackerAttack();
    }

    /// <summary>
    /// Checks if the attacker has any actions available.
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    /// <returns>True if actions are available; otherwise, false.</returns>
    public bool HasAttackerAction()
    {
        return _combatService.HasAttackerAction();
    }

    /// <summary>
    /// Calculates the damage result from an attack.
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    /// <returns>The computed damage value.</returns>
    public float ComputeDamageAttack()
    {
        return _combatService.ComputeDamageAttack();
    }

    /// <summary>
    /// Handles the attack logic between the attacker and opponent.
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    public void HandleAttack()
    {
        _combatService.HandleAttack();
    }

    /// <summary>
    /// Executes the special action for the attacker.
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    public void UseSpecialAction()
    {
        _combatService.UseSpecialAction();
    }

    /// <summary>
    /// Builds a list of cards that can be targeted for an attack.
    /// Phase 4: Now delegates to CombatService.BuildValidTargets()
    /// </summary>
    /// <returns>A list of valid target cards.</returns>
    public List<InGameCard> BuildInvocationCardsForAttack()
    {
        return _combatService.BuildValidTargets();
    }

    /// <summary>
    /// Checks if it's possible for the attacker to perform a special action.
    /// Phase 4: Now delegates to CombatService
    /// </summary>
    /// <returns>True if a special action is possible; otherwise, false.</returns>
    public bool IsSpecialActionPossible()
    {
        return _combatService.IsSpecialActionPossible();
    }


    /// <summary>
    /// Handles actions and effects at the start of a turn.
    /// Phase 4: Now delegates to TurnService
    /// </summary>
    public void OnTurnStart()
    {
        _turnService.OnTurnStart();
    }
}