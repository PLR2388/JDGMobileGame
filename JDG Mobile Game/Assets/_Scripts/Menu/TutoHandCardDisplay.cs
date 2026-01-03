using System;
using System.Collections.ObjectModel;
using Cards;
using JDG.Application;
using JDG.Domain.Events;
using JDG.Infrastructure.Services;
using UnityEngine;
using VContainer;

/// <summary>
/// Represents the display of cards in a tutorial scenario.
/// Phase 123: Uses EventBus instead of static DialogueUI.DialogIndex.
/// </summary>
public class TutoHandCardDisplay : HandCardDisplay
{
    private int currentDialogIndex = 0;

    // Phase 123: EventBus for dialogue index changes
    private IEventBus _tutoEventBus;
    private IDisposable _dialogueIndexSubscription;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 123: Added IEventBus for DialogueIndexChangedEvent.
    /// Phase 132: Added IObjectResolver for injecting dynamically created OnHover components.
    /// </summary>
    [Inject]
    public new void Construct(IEventBus eventBus, GameStateService gameStateService, VContainer.IObjectResolver container)
    {
        base.Construct(eventBus, gameStateService, container);
        _tutoEventBus = eventBus;
    }

    private const int StartHighlightMusiqueDeMegadriveIndex = 35;

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Subscribes to relevant events.
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Called when the behaviour becomes disabled.
    /// Unsubscribes from events and clears created cards.
    /// </summary>
    private void OnDisable()
    {
        ClearCreatedCards();
    }

    /// <summary>
    /// Called before the object is destroyed.
    /// Cleans up by clearing created cards and unsubscribing from events.
    /// </summary>
    private void OnDestroy()
    {
        ClearCreatedCards();
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Subscribes to necessary events for card display updates.
    /// Phase 23: Removed HandCardChange static event, using base class EventBus subscription.
    /// Phase 123: Uses EventBus instead of static DialogueUI.DialogIndex.
    /// </summary>
    private new void SubscribeToEvents()
    {
        base.SubscribeToEvents(); // Subscribe to EventBus in base class
        // Phase 123: Subscribe to EventBus instead of static DialogIndex
        _dialogueIndexSubscription = _tutoEventBus?.Subscribe<DialogueIndexChangedEvent>(OnDialogueIndexChanged);
    }

    /// <summary>
    /// Unsubscribes from hand card change events.
    /// Phase 23: Removed HandCardChange static event, using base class EventBus subscription.
    /// Phase 123: Disposes EventBus subscription instead of static DialogIndex.
    /// </summary>
    private new void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents(); // Unsubscribe from EventBus in base class
        // Phase 123: Dispose EventBus subscription
        _dialogueIndexSubscription?.Dispose();
    }

    /// <summary>
    /// Handles DialogueIndexChangedEvent from EventBus.
    /// Phase 123: Replaces static DialogueUI.DialogIndex listener.
    /// </summary>
    private void OnDialogueIndexChanged(DialogueIndexChangedEvent evt)
    {
        UpdateCurrentDialogIndex(evt.DialogueIndex);
    }

    /// <summary>
    /// Updates the current dialog index and checks for highlighting conditions.
    /// </summary>
    /// <param name="index">The new dialog index.</param>
    private void UpdateCurrentDialogIndex(int index)
    {
        currentDialogIndex = index;
        HighlightCardOnDialogChange(index);
    }
    
    /// <summary>
    /// Highlights specific cards based on the current dialog index.
    /// </summary>
    /// <param name="index">The current dialog index.</param>
    private void HighlightCardOnDialogChange(int index)
    {
        if (index > StartHighlightMusiqueDeMegadriveIndex)
        {
            var card = CreatedCards.Find(card => card.GetComponent<CardDisplay>().Card.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive]);
            if (card != null)
            {
                card.AddComponent<HighLightCard>();
            }
        }
    }

    /// <summary>
    /// Determines if a specific in-game card should be highlighted.
    /// </summary>
    /// <param name="handCard">The in-game card to check.</param>
    /// <returns>True if the card should be highlighted, false otherwise.</returns>
    private bool ShouldHighlightCard(InGameCard handCard)
    {
        return handCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste] ||
               (handCard.Title == CardNameMappings.CardNameMap[CardNames.MusiqueDeMegaDrive] && currentDialogIndex > StartHighlightMusiqueDeMegadriveIndex);
    }

    /// <summary>
    /// Creates visual representations for the provided cards with tutorial-specific highlighting.
    /// Phase 132: Added injection for dynamically created OnHover components.
    /// Phase 141: Changed to protected override to properly override base class method.
    /// </summary>
    /// <param name="handCards">Collection of in-game cards.</param>
    protected override void CreateCards(ObservableCollection<InGameCard> handCards)
    {
        foreach (var handCard in handCards)
        {
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
            newCard.transform.SetParent(transform, true);
            newCard.GetComponent<CardDisplay>().InGameCard = handCard;

            var onHover = newCard.GetComponent<OnHover>();
            if (onHover != null)
            {
                onHover.bIsInGame = true;
                // Phase 132: Inject IEventBus and ICardSelectionService into dynamically created OnHover
                _container?.Inject(onHover);
            }

            // Tutorial-specific: Add highlighting for specific cards
            if (ShouldHighlightCard(handCard))
            {
                var highlightCard = newCard.AddComponent<HighLightCard>();
                // Phase 141: Inject the dynamically created HighLightCard component
                _container?.Inject(highlightCard);
            }

            CreatedCards.Add(newCard);
        }

        AdjustRectTransformSize(handCards.Count);
    }

    // Phase 141: Removed shadowing methods (RebuildHandDisplay, ShouldDisplayHandCard, DisplayHandCard)
    // The base class now calls our overridden CreateCards() method for proper polymorphism.
}