using System;
using Cards;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VContainer;
using JDG.Application;
using JDG.Domain.Events;

// Phase 144: Removed dead CardSelectedEvent : UnityEvent<InGameCard> class
// (was never instantiated, superseded by EventBus CardSelectedEvent struct)

[RequireComponent(typeof(Image))]
public class OnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject numberTextObject;
    private Text numberText;
    private int number = 0;
    private InGameCard card;

    private Image image;

    private CardState currentState; // This will be an abstract base class or interface for different card states

    public bool bIsInGame = false;

    // Phase 41: Migrated from legacy ICardSelectionService to clean architecture
    private JDG.Application.Services.ICardSelectionService _cardSelectionService;
    private IEventBus _eventBus;
    private IDisposable _cardDeselectedSubscription;
    private IDisposable _cardNumberedSubscription; // Phase 121: EventBus subscription

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 41: Inject clean ICardSelectionService and IEventBus instead of legacy service.
    /// </summary>
    [Inject]
    public void Construct(
        JDG.Application.Services.ICardSelectionService cardSelectionService,
        IEventBus eventBus)
    {
        _cardSelectionService = cardSelectionService;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Indicates if the card is currently selected.
    /// </summary>
    public bool IsSelected => currentState is SelectedCardState;

    /// <summary>
    /// Initialize component references and set up event listeners.
    /// Phase 121: Subscribe to CardNumberedEvent via EventBus.
    /// Phase 143: Check for CardToHighlight and apply pulsing effect.
    /// </summary>
    private void Start()
    {
        image = GetComponent<Image>();
        numberText = numberTextObject.GetComponent<Text>();
        card = gameObject.GetComponent<CardDisplay>().InGameCard;

        // Phase 41: Subscribe to EventBus instead of legacy UnityEvent
        _cardDeselectedSubscription = _eventBus?.Subscribe<CardRemovedFromSelectionEvent>(OnCardRemovedFromSelection);
        // Phase 121: Subscribe to CardNumberedEvent via EventBus
        _cardNumberedSubscription = _eventBus?.Subscribe<CardNumberedEvent>(OnCardNumbered);

        // Initialize default state
        SetState(new DefaultCardState(this, card, _cardSelectionService));

        // Phase 143: Check if this card should be highlighted for tutorial
        if (!string.IsNullOrEmpty(DisplayCards.CardToHighlight) && card?.Title == DisplayCards.CardToHighlight)
        {
            StartCoroutine(PulseHighlight());
        }
    }

    /// <summary>
    /// Phase 143: Coroutine to pulse the card with green color for tutorial highlighting.
    /// </summary>
    private System.Collections.IEnumerator PulseHighlight()
    {
        while (!string.IsNullOrEmpty(DisplayCards.CardToHighlight) && card?.Title == DisplayCards.CardToHighlight)
        {
            SetImageColor(Color.green);
            yield return new WaitForSeconds(0.5f);
            SetImageColor(Color.white);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Cleanup and unsubscribe from events.
    /// Phase 121: Dispose CardNumberedEvent subscription.
    /// </summary>
    private void OnDestroy()
    {
        // Phase 41: Dispose EventBus subscriptions
        _cardDeselectedSubscription?.Dispose();
        // Phase 121: Dispose CardNumberedEvent subscription
        _cardNumberedSubscription?.Dispose();
    }

    /// <summary>
    /// Handles card removed from selection event.
    /// Phase 41: EventBus handler replacing legacy UnityEvent listener.
    /// </summary>
    private void OnCardRemovedFromSelection(CardRemovedFromSelectionEvent evt)
    {
        if (evt.Card is InGameCard deselectedCard)
        {
            UnSelectCard(deselectedCard);
        }
    }

    /// <summary>
    /// Handles card numbered event.
    /// Phase 121: EventBus handler replacing CardSelector.NumberedCardEvent static UnityEvent.
    /// </summary>
    private void OnCardNumbered(CardNumberedEvent evt)
    {
        if (evt.Card is InGameCard cardToModify)
        {
            UpdateNumberOnCard(cardToModify, evt.Number);
        }
    }

    /// <summary>
    /// Deselect the card.
    /// </summary>
    /// <param name="cardToUnselect">The card to deselect.</param>
    private void UnSelectCard(InGameCard cardToUnselect)
    {
        if (cardToUnselect == card)
        {
            SetState(new DefaultCardState(this, card, _cardSelectionService));
        }
    }

    /// <summary>
    /// Update the number displayed on the card.
    /// Phase 121: Now called from OnCardNumbered EventBus handler.
    /// Phase 144: Added null checks to prevent NullReferenceException.
    /// </summary>
    /// <param name="cardToModify">The card to modify.</param>
    /// <param name="numberToApply">The number to display on the card.</param>
    private void UpdateNumberOnCard(InGameCard cardToModify, int numberToApply)
    {
        // Phase 144: Add null checks
        if (card == null || cardToModify == null) return;

        if (card.Title == cardToModify.Title)
        {
            number = numberToApply;
            SetState(new NumberCardState(this, card, _cardSelectionService));
        }
    }

    /// <summary>
    /// Hide the number display on the card.
    /// </summary>
    public void HideNumber()
    {
        numberTextObject.SetActive(false);
    }

    /// <summary>
    /// Display the card's number.
    /// </summary>
    public void DisplayNumber()
    {
        numberText.text = "" + number;
        numberTextObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    /// <summary>
    /// Handle card click events.
    /// Phase 109: Publishes InGameCardClickedEvent via EventBus instead of static UnityEvent.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (bIsInGame)
        {
            // Phase 109: Publish via EventBus instead of static event
            _eventBus?.Publish(new InGameCardClickedEvent { Card = card });
        }
        else
        {
            currentState.OnClick();
        }
    }

    /// <summary>
    /// Set the card's current state.
    /// </summary>
    /// <param name="state">The new state for the card.</param>
    public void SetState(CardState state)
    {
        currentState = state;
        currentState.EnterState();
    }

    public void SetImageColor(Color color)
    {
        image.color = color;
    }
}