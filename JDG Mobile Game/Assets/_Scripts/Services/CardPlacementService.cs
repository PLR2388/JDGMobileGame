using System.Collections.Generic;
using System.Linq;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using JDG.Application.Abilities;
using JDG.Application.Services;
using UnityEngine;

/// <summary>
/// Service for managing card placement operations.
/// Extracts business logic from *Functions MonoBehaviours.
/// Part of Phase 6 - MonoBehaviour logic extraction.
/// Phase 28: Uses IPlayerStatusProvider instead of PlayerManager.Instance.
/// Phase 37: Uses IAudioService instead of AudioSystem.Instance.
/// Phase 114: Uses IAbilityExecutor for effect card abilities.
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

    public CardPlacementService(
        ICardCollectionService cardCollectionService,
        IPlayerStatusProvider playerStatusProvider,
        IAudioService audioService,
        IAbilityExecutor abilityExecutor)
    {
        _cardCollectionService = cardCollectionService;
        _playerStatusProvider = playerStatusProvider;
        _audioService = audioService;
        _abilityExecutor = abilityExecutor;
    }

    /// <summary>
    /// Places an invocation card on the field.
    /// </summary>
    /// <param name="card">The invocation card to place.</param>
    /// <param name="canvas">Canvas for UI operations (passed to abilities).</param>
    /// <returns>True if placement was successful, false if field is full (4 cards max).</returns>
    public bool PlaceInvocationCard(InGameInvocationCard card, Transform canvas)
    {
        if (card == null)
            return false;

        var currentPlayerCard = _cardCollectionService.GetCurrentPlayerCards();

        // Check if field has space (max 4 invocations)
        if (currentPlayerCard.InvocationCards.Count >= 4)
            return false;

        // Add card to field and remove from hand
        currentPlayerCard.InvocationCards.Add(card);
        currentPlayerCard.HandCards.Remove(card);

        // Apply card abilities
        var opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();
        foreach (var ability in card.Abilities)
        {
            ability.ApplyEffect(canvas, currentPlayerCard, opponentPlayerCards);
        }

        return true;
    }

    /// <summary>
    /// Places an effect card on the field.
    /// Phase 114: Updated to use IAbilityExecutor for modern ability execution.
    /// </summary>
    /// <param name="card">The effect card to place.</param>
    /// <param name="canvas">Canvas for UI operations (no longer needed, kept for API compatibility).</param>
    /// <returns>True if placement was successful, false if field is full (4 cards max).</returns>
    public bool PlaceEffectCard(InGameEffectCard card, Transform canvas)
    {
        if (card == null)
            return false;

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
    /// </summary>
    /// <param name="card">The field card to place.</param>
    /// <returns>True if placement was successful, false if field card slot is occupied or card is null.</returns>
    public bool PlaceFieldCard(InGameFieldCard card)
    {
        if (card == null)
            return false;

        var currentPlayerCard = _cardCollectionService.GetCurrentPlayerCards();

        // Check if field card slot is empty (only 1 field card allowed)
        if (currentPlayerCard.FieldCard != null)
            return false;

        // Set field card and remove from hand
        currentPlayerCard.FieldCard = card;
        currentPlayerCard.HandCards.Remove(card);

        // Apply field abilities
        foreach (var ability in card.FieldAbilities)
        {
            ability.ApplyEffect(currentPlayerCard);
        }

        // Play family-specific music
        // Phase 37: Use IAudioService instead of AudioSystem.Instance
        _audioService.PlayFamilyMusic(card.Family);

        return true;
    }

    /// <summary>
    /// Checks if an equipment card can be placed and returns valid targets.
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

        // Check if equipment has "CanAlwaysBePut" ability
        var canAlwaysBePut = card.EquipmentAbilities.Any(ability => ability.CanAlwaysBePut);

        // Filter valid targets: all cards if CanAlwaysBePut, otherwise only cards without equipment
        var validTargets = allInvocationCards
            .Where(invocationCard => canAlwaysBePut || invocationCard.EquipmentCard == null)
            .Cast<InGameCard>()
            .ToList();

        return validTargets;
    }

    /// <summary>
    /// Places an equipment card on a target invocation.
    /// </summary>
    /// <param name="equipment">The equipment card to place.</param>
    /// <param name="target">The target invocation card.</param>
    /// <param name="canvas">Canvas for UI operations (passed to abilities).</param>
    /// <returns>True if placement was successful, false if equipment or target is null.</returns>
    public bool PlaceEquipmentCard(InGameEquipmentCard equipment, InGameInvocationCard target, Transform canvas)
    {
        if (equipment == null || target == null)
            return false;

        var currentPlayerCards = _cardCollectionService.GetCurrentPlayerCards();
        var opponentPlayerCards = _cardCollectionService.GetOpponentPlayerCards();

        // Apply equipment abilities
        foreach (var equipmentAbility in equipment.EquipmentAbilities)
        {
            equipmentAbility.ApplyEffect(target, currentPlayerCards, opponentPlayerCards);
        }

        // Set equipment on invocation and remove from hand
        target.SetEquipmentCard(equipment);
        currentPlayerCards.HandCards.Remove(equipment);

        return true;
    }

    /// <summary>
    /// Handles cancellation/reactivation of invocation card effects.
    /// </summary>
    /// <param name="card">The invocation card to process.</param>
    public void HandleInvocationCancelEffect(InGameInvocationCard card)
    {
        if (card == null)
            return;

        var currentPlayerCard = _cardCollectionService.GetCurrentPlayerCards();

        if (card.CancelEffect)
        {
            // Cancel effects
            foreach (var ability in card.Abilities)
            {
                ability.CancelEffect(currentPlayerCard);
            }
        }
        else
        {
            // Reactivate effects
            foreach (var ability in card.Abilities)
            {
                ability.ReactivateEffect(currentPlayerCard);
            }
        }
    }
}
