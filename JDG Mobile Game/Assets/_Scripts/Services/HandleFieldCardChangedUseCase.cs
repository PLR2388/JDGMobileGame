using System;
using JDG.Application;
using JDG.Domain.Events;

/// <summary>
/// Use case for handling field card changes.
/// Phase 21-22: Extracted from PlayerCards.FieldCard property setter.
/// Phase 79: OBSOLETE - Use JDG.Application.UseCases.HandleFieldCardChangedUseCase instead.
///
/// This legacy version remains for backward compatibility with code that
/// passes concrete InGameFieldCard and PlayerCards types.
/// </summary>
[Obsolete("Use JDG.Application.UseCases.HandleFieldCardChangedUseCase with IInGameFieldCard instead. Phase 79.")]
public class HandleFieldCardChangedUseCase
{
    private readonly IEventBus _eventBus;

    public HandleFieldCardChangedUseCase(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Handles a field card being removed from the field.
    /// Triggers OnFieldCardRemoved for all field abilities on the old card.
    /// </summary>
    /// <param name="oldFieldCard">The field card being removed (null if none).</param>
    /// <param name="ownerPlayerCards">The PlayerCards instance that owns the field card.</param>
    public void HandleFieldCardRemoved(
        InGameFieldCard oldFieldCard,
        PlayerCards ownerPlayerCards)
    {
        if (oldFieldCard != null)
        {
            foreach (var fieldCardFieldAbility in oldFieldCard.FieldAbilities)
            {
                fieldCardFieldAbility.OnFieldCardRemoved(ownerPlayerCards);
            }

            // Publish event to notify other systems
            var domainOwner = (JDG.Domain.CardOwner)(int)oldFieldCard.CardOwner;
            _eventBus.Publish(new CardRemovedFromFieldEvent
            {
                CardId = System.Guid.Empty, // Legacy cards don't have Guid IDs yet
                Owner = domainOwner
            });
        }
    }
}
