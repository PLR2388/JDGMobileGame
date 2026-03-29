using Cards;
using JDG.Infrastructure.Cards;
using UnityEngine;

/// <summary>
/// Service for managing card placement operations.
/// Replaces business logic from *Functions MonoBehaviours.
/// Part of Phase 6 - MonoBehaviour logic extraction.
///
/// Note: This service is in the default assembly because it depends on legacy types.
/// </summary>
public interface ICardPlacementService
{
    /// <summary>
    /// Places an invocation card on the field.
    /// </summary>
    /// <param name="card">The invocation card to place.</param>
    /// <param name="canvas">Canvas for UI operations.</param>
    /// <returns>True if placement was successful, false otherwise.</returns>
    bool PlaceInvocationCard(InGameInvocationCard card, Transform canvas);

    /// <summary>
    /// Places an effect card on the field.
    /// </summary>
    /// <param name="card">The effect card to place.</param>
    /// <param name="canvas">Canvas for UI operations.</param>
    /// <returns>True if placement was successful, false otherwise.</returns>
    bool PlaceEffectCard(InGameEffectCard card, Transform canvas);

    /// <summary>
    /// Places a field card on the field.
    /// </summary>
    /// <param name="card">The field card to place.</param>
    /// <returns>True if placement was successful, false otherwise.</returns>
    bool PlaceFieldCard(InGameFieldCard card);

    /// <summary>
    /// Checks if an equipment card can be placed and returns valid targets.
    /// </summary>
    /// <param name="card">The equipment card to check.</param>
    /// <returns>List of valid invocation targets for the equipment.</returns>
    System.Collections.Generic.List<InGameCard> GetEquipmentTargets(InGameEquipmentCard card);

    /// <summary>
    /// Places an equipment card on a target invocation.
    /// </summary>
    /// <param name="equipment">The equipment card to place.</param>
    /// <param name="target">The target invocation card.</param>
    /// <param name="canvas">Canvas for UI operations.</param>
    /// <returns>True if placement was successful, false otherwise.</returns>
    bool PlaceEquipmentCard(InGameEquipmentCard equipment, InGameInvocationCard target, Transform canvas);

    /// <summary>
    /// Handles cancellation/reactivation of invocation card effects.
    /// </summary>
    /// <param name="card">The invocation card to process.</param>
    void HandleInvocationCancelEffect(InGameInvocationCard card);
}
