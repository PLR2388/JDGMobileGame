using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using JDG.Application.Abilities;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.ValueObjects;
using JDG.Infrastructure.Services;
using UnityEngine;

/// <summary>
/// Service for managing card placement operations.
/// Extracts business logic from *Functions MonoBehaviours.
/// Part of Phase 6 - MonoBehaviour logic extraction.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 37: Uses IAudioService instead of AudioSystem.Instance.
/// Phase 114: Uses IAbilityExecutor for effect card abilities.
/// Phase 116: Uses modern IAbility for field card abilities.
/// Phase 118: Uses only ModernAbilities, removed legacy Ability references.
/// Phase 135: Added GameStateService for phase validation.
/// Phase 155: Added ICardSyncService for proper SourceCard in AbilityContext.
///
/// Note: This service is in the default assembly because it depends on legacy types.
/// It will be moved to JDG.Infrastructure once legacy types are refactored.
/// </summary>
public class CardPlacementService : ICardPlacementService
{
    private readonly ICardCollectionService _cardCollectionService;
    private readonly IPlayerStatusProvider _playerStatusProvider;
    private readonly IAudioService _audioService;
    private readonly IAbilityExecutor _abilityExecutor;
    private readonly ICardSyncService _cardSyncService;

    /// <summary>
    /// Phase 148: GameStateService is intentionally optional for backward compatibility.
    /// When null, phase validation is skipped. This allows the service to work in
    /// test scenarios or legacy code paths where GameStateService isn't available.
    /// In production, GameStateService is always injected via VContainer.
    /// </summary>
    private readonly GameStateService _gameStateService;

    public CardPlacementService(
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        IAudioService audioService,
        IAbilityExecutor abilityExecutor,
        GameStateService gameStateService,
        ICardSyncService cardSyncService = null)
    {
        _cardCollectionService = cardCollectionService;
        _playerStatusProvider = playerStatusProvider;
        _audioService = audioService;
        _abilityExecutor = abilityExecutor;
        _gameStateService = gameStateService;
        _cardSyncService = cardSyncService;
    }

    /// <summary>
    /// Places an invocation card on the field.
    /// Phase 118: Uses ModernAbilities with OnSummon trigger instead of legacy Abilities.
    /// Phase 135: Added phase validation.
    /// </summary>
    /// <param name="card">The invocation card to place.</param>
    /// <param name="canvas">Canvas for UI operations (no longer needed, kept for API compatibility).</param>
    /// <returns>True if placement was successful, false if field is full (4 cards max).</returns>
    public bool PlaceInvocationCard(InGameInvocationCard card, Transform canvas)
    {
        if (card == null)
            return false;

        // Phase 135: Validate game state before placing card
        if (_gameStateService != null)
        {
            if (_gameStateService.IsGameOver)
            {
#if UNITY_EDITOR
                Debug.Log("CardPlacementService: Cannot place invocation card - game is over");
#endif
                return false;
            }
            if (_gameStateService.CurrentPhase != JDG.Domain.Phase.Choose)
            {
#if UNITY_EDITOR
                Debug.Log($"CardPlacementService: Cannot place invocation card - current phase is {_gameStateService.CurrentPhase}");
#endif
                return false;
            }
        }

        var currentPlayerCard = _cardCollectionService.GetCurrentPlayerCards();

        // Check if field has space (max 4 invocations)
        if (currentPlayerCard.InvocationCards.Count >= 4)
            return false;

        // Add card to field and remove from hand
        currentPlayerCard.InvocationCards.Add(card);
        currentPlayerCard.HandCards.Remove(card);

        // Phase 118: Apply card abilities using ModernAbilities
        // Phase 155: Create linked domain card for proper SourceCard in AbilityContext
        var opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();
        var owner = currentPlayerCard.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        var ownerId = PlayerId.FromCardOwner(owner);
        var opponentOwner = currentPlayerCard.IsPlayerOne ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;
        var opponentId = PlayerId.FromCardOwner(opponentOwner);

        // Phase 155: Create linked domain card so abilities have proper SourceCard
        JDG.Domain.Entities.Card domainCard = null;
        if (_cardSyncService != null)
        {
            domainCard = _cardSyncService.CreateLinkedCard(card);
        }
        var context = new AbilityContext(ownerId, opponentId, domainCard, JDG.Domain.AbilityName.Default);

        // Phase 148: Added null-coalescing to prevent NullReferenceException
        foreach (var ability in card.ModernAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
        {
            if (ability.CanActivate(context))
            {
                ability.Execute(context);
            }
        }

        // Phase 155: Sync state changes back to InGameInvocationCard
        if (_cardSyncService != null && domainCard != null)
        {
            _cardSyncService.SyncCardState(domainCard);
        }

        // Notify ability executor for OnCardAddedToField triggers on other cards
        _abilityExecutor.ExecuteOnCardAddedToField(card, currentPlayerCard, opponentPlayerCards);

        return true;
    }

    /// <summary>
    /// Places an effect card on the field.
    /// Phase 114: Updated to use IAbilityExecutor for modern ability execution.
    /// Phase 135: Added phase validation.
    /// </summary>
    /// <param name="card">The effect card to place.</param>
    /// <param name="canvas">Canvas for UI operations (no longer needed, kept for API compatibility).</param>
    /// <returns>True if placement was successful, false if field is full (4 cards max).</returns>
    public bool PlaceEffectCard(InGameEffectCard card, Transform canvas)
    {
        if (card == null)
            return false;

        // Phase 135: Validate game state before placing card
        if (_gameStateService != null)
        {
            if (_gameStateService.IsGameOver)
            {
#if UNITY_EDITOR
                Debug.Log("CardPlacementService: Cannot place effect card - game is over");
#endif
                return false;
            }
            if (_gameStateService.CurrentPhase != JDG.Domain.Phase.Choose)
            {
#if UNITY_EDITOR
                Debug.Log($"CardPlacementService: Cannot place effect card - current phase is {_gameStateService.CurrentPhase}");
#endif
                return false;
            }
        }

        var currentPlayerCard = _cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCard = _cardCollectionService.GetOpponentPlayerCards();

        // Check if field has space (max 4 effects)
        if (currentPlayerCard.EffectCards.Count >= 4)
            return false;

        // Phase 114: Execute effect abilities through IAbilityExecutor
        _abilityExecutor.ExecuteOnEffectCardPlayed(card, currentPlayerCard, opponentPlayerCard);

        // Remove from hand and add to field
        currentPlayerCard.HandCards.Remove(card);
        currentPlayerCard.EffectCards.Add(card);

        return true;
    }

    /// <summary>
    /// Places a field card on the field.
    /// Phase 116: Updated to use modern IAbility for field abilities.
    /// Phase 135: Added phase validation.
    /// </summary>
    /// <param name="card">The field card to place.</param>
    /// <returns>True if placement was successful, false if field card slot is occupied or card is null.</returns>
    public bool PlaceFieldCard(InGameFieldCard card)
    {
        if (card == null)
            return false;

        // Phase 135: Validate game state before placing card
        if (_gameStateService != null)
        {
            if (_gameStateService.IsGameOver)
            {
#if UNITY_EDITOR
                Debug.Log("CardPlacementService: Cannot place field card - game is over");
#endif
                return false;
            }
            if (_gameStateService.CurrentPhase != JDG.Domain.Phase.Choose)
            {
#if UNITY_EDITOR
                Debug.Log($"CardPlacementService: Cannot place field card - current phase is {_gameStateService.CurrentPhase}");
#endif
                return false;
            }
        }

        var currentPlayerCard = _cardCollectionService.GetCurrentPlayerCards();

        // Check if field card slot is empty (only 1 field card allowed)
        if (currentPlayerCard.FieldCard != null)
            return false;

        // Set field card and remove from hand
        currentPlayerCard.FieldCard = card;
        currentPlayerCard.HandCards.Remove(card);

        // Phase 116: Apply field abilities using modern IAbility
        var owner = currentPlayerCard.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        var ownerId = PlayerId.FromCardOwner(owner);
        var opponentOwner = currentPlayerCard.IsPlayerOne ? JDG.Domain.CardOwner.Player2 : JDG.Domain.CardOwner.Player1;
        var opponentId = PlayerId.FromCardOwner(opponentOwner);
        var context = new AbilityContext(ownerId, opponentId, null, JDG.Domain.AbilityName.Default);

        // Phase 148: Added null-coalescing to prevent NullReferenceException
        foreach (var ability in card.ModernFieldAbilities ?? System.Linq.Enumerable.Empty<IAbility>())
        {
            if (ability.CanActivate(context))
            {
                ability.Execute(context);
            }
        }

        // Play family-specific music
        // Phase 37: Use IAudioService instead of AudioSystem.Instance
        _audioService.PlayFamilyMusic(card.Family);

        return true;
    }

    /// <summary>
    /// Checks if an equipment card can be placed and returns valid targets.
    /// Phase 117: Uses CanAlwaysBePlaced property instead of iterating legacy abilities.
    /// </summary>
    /// <param name="card">The equipment card to check.</param>
    /// <returns>List of valid invocation targets for the equipment.</returns>
    public List<InGameCard> GetEquipmentTargets(InGameEquipmentCard card)
    {
        if (card == null)
            return new List<InGameCard>();

        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();

        // Combine invocations from both players
        var currentInvocationCards = currentPlayerCards.InvocationCards;
        var opponentInvocationCards = opponentPlayerCards.InvocationCards;
        var allInvocationCards = currentInvocationCards.Concat(opponentInvocationCards);

        // Phase 117: Use CanAlwaysBePlaced property instead of legacy ability iteration
        var canAlwaysBePlaced = card.CanAlwaysBePlaced;

        // Filter valid targets: all cards if CanAlwaysBePlaced, otherwise only cards without equipment
        var validTargets = allInvocationCards
            .Where(invocationCard => canAlwaysBePlaced || invocationCard.EquipmentCard == null)
            .Cast<InGameCard>()
            .ToList();

        return validTargets;
    }

    /// <summary>
    /// Places an equipment card on a target invocation.
    /// Phase 117: Uses IAbilityExecutor for modern equipment ability execution.
    /// Phase 135: Added phase validation.
    /// </summary>
    /// <param name="equipment">The equipment card to place.</param>
    /// <param name="target">The target invocation card.</param>
    /// <param name="canvas">Canvas for UI operations (no longer needed, kept for API compatibility).</param>
    /// <returns>True if placement was successful, false if equipment or target is null.</returns>
    public bool PlaceEquipmentCard(InGameEquipmentCard equipment, InGameInvocationCard target, Transform canvas)
    {
        if (equipment == null || target == null)
            return false;

        // Phase 135: Validate game state before placing card
        if (_gameStateService != null)
        {
            if (_gameStateService.IsGameOver)
            {
#if UNITY_EDITOR
                Debug.Log("CardPlacementService: Cannot place equipment card - game is over");
#endif
                return false;
            }
            if (_gameStateService.CurrentPhase != JDG.Domain.Phase.Choose)
            {
#if UNITY_EDITOR
                Debug.Log($"CardPlacementService: Cannot place equipment card - current phase is {_gameStateService.CurrentPhase}");
#endif
                return false;
            }
        }

        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();

        // Phase 117: Execute equipment abilities through IAbilityExecutor
        _abilityExecutor.ExecuteOnEquipmentAttached(equipment, target, currentPlayerCards, opponentPlayerCards);

        // Set equipment on invocation and remove from hand
        target.SetEquipmentCard(equipment);
        currentPlayerCards.HandCards.Remove(equipment);

        return true;
    }

    /// <summary>
    /// Handles cancellation/reactivation of invocation card effects.
    /// Phase 118: Uses ModernAbilities - cancel/reactivate handled via card state.
    /// </summary>
    /// <param name="card">The invocation card to process.</param>
    public void HandleInvocationCancelEffect(InGameInvocationCard card)
    {
        if (card == null)
            return;

        // Phase 118: Cancel/reactivate is handled by the CancelEffect property on the card.
        // Modern abilities check card.CancelEffect in their CanActivate implementation.
        // The InvocationCancelledEvent is published by the card's CancelEffect setter.
        // No additional legacy ability calls needed.
    }
}
