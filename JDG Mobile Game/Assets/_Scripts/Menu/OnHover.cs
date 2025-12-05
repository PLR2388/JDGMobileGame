using Cards;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using VContainer;

[System.Serializable]
public class CardSelectedEvent : UnityEvent<InGameCard>
{
}

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

    // Phase 9: Injected dependencies
    private ICardSelectionService _cardSelectionService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 9: Inject ICardSelectionService instead of using singleton.
    /// </summary>
    [Inject]
    public void Construct(ICardSelectionService cardSelectionService)
    {
        _cardSelectionService = cardSelectionService;
    }

    /// <summary>
    /// Indicates if the card is currently selected.
    /// </summary>
    public bool IsSelected => currentState is SelectedCardState;

    /// <summary>
    /// Initialize component references and set up event listeners.
    /// </summary>
    private void Start()
    {
        image = GetComponent<Image>();
        numberText = numberTextObject.GetComponent<Text>();
        CardSelector.NumberedCardEvent.AddListener(UpdateNumberOnCard);
        card = gameObject.GetComponent<CardDisplay>().InGameCard;

        // Phase 9: Use injected service instead of CardSelectionManager.Instance
        _cardSelectionService?.CardDeselected.AddListener(UnSelectCard);

        // Initialize default state
        SetState(new DefaultCardState(this, card, _cardSelectionService));
    }

    /// <summary>
    /// Cleanup and unsubscribe from events.
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from events
        CardSelector.NumberedCardEvent.RemoveListener(UpdateNumberOnCard);
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
    /// </summary>
    /// <param name="cardToModify">The card to modify.</param>
    /// <param name="numberToApply">The number to display on the card.</param>
    private void UpdateNumberOnCard(InGameCard cardToModify, int numberToApply)
    {
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
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (bIsInGame)
        {
            InGameMenuScript.EventClick.Invoke(card);
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