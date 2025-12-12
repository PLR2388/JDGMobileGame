using System.Collections.Generic;
using Cards;
using JDG.Application;
using JDG.Domain.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// Manages in-game card interactions, handling events, and displaying UI elements related to cards.
/// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
/// Phase 23: Migrated HandCardChange invocations to EventBus.
/// Phase 28: Added IPlayerStatusProvider for player status access in card handlers.
/// </summary>
public class InGameMenuScript : MonoBehaviour
{
    // Phase 17-18: Injected dependency (protected so TutoInGameMenuScript can access)
    protected ICardCollectionService _cardCollectionService;

    // Phase 28: Injected player status provider
    protected IPlayerStatusProvider _playerStatusProvider;

    // Phase 23: EventBus for hand card display events
    protected IEventBus _eventBus;
    // Serialized fields for UI components
    [SerializeField] protected TextMeshProUGUI buttonText;
    [SerializeField] protected GameObject handScreen;
    [SerializeField] protected GameObject miniMenuCard;
    [SerializeField] protected GameObject detailCardPanel;
    [SerializeField] private TextMeshProUGUI detailButtonText;
    [SerializeField] protected GameObject inHandButton;
    [SerializeField] protected GameObject backgroundInformation;

    // Used in CardHandler
    [SerializeField] public TextMeshProUGUI putCardButtonText;
    [SerializeField] public Button putCardButton;

    /// <summary>
    /// Represents the currently selected in-game card.
    /// </summary>
    protected InGameCard CurrentSelectedCard;

    [SerializeField] protected GameObject invocationMenu;

    // Static events for various card interactions
    public static readonly CardEvent EventClick = new CardEvent();
    public static readonly InvocationCardEvent InvocationCardEvent = new InvocationCardEvent();
    public static readonly FieldCardEvent FieldCardEvent = new FieldCardEvent();
    public static readonly EffectCardEvent EffectCardEvent = new EffectCardEvent();
    public static readonly EquipmentCardEvent EquipmentCardEvent = new EquipmentCardEvent();


    private const float ButtonGroupPosX = 600f;
    private const float ButtonGroupPosY = 400f;
    private const float ButtonGroupPosZ = 0f;

    private const float PaddingX = 490f;
    private const float PaddingY = -350f;
    private const float PaddingZ = 0f;

    private readonly Vector3 buttonGroupPosition = new Vector3(ButtonGroupPosX, ButtonGroupPosY, ButtonGroupPosZ);
    private readonly Vector3 padding = new Vector3(PaddingX, PaddingY, PaddingZ);

    /// <summary>
    /// Dictionary mapping card types to their respective handlers.
    /// </summary>
    protected readonly Dictionary<CardType, CardHandler> CardHandlerMap = new Dictionary<CardType, CardHandler>();

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 17-18: Inject ICardCollectionService instead of CardManager.Instance.
    /// Phase 23: Inject IEventBus for hand card display events.
    /// Phase 28: Inject IPlayerStatusProvider for player status access.
    /// </summary>
    [Inject]
    public void Construct(ICardCollectionService cardCollectionService, IEventBus eventBus, IPlayerStatusProvider playerStatusProvider)
    {
        _cardCollectionService = cardCollectionService;
        _eventBus = eventBus;
        _playerStatusProvider = playerStatusProvider;
    }

    /// <summary>
    /// Initializes handlers for different types of cards.
    /// Phase 17-18: Pass ICardCollectionService to handlers.
    /// Phase 28: Pass IPlayerStatusProvider to handlers.
    /// </summary>
    protected void InitializeCardHandlers()
    {
        CardHandlerMap[CardType.Invocation] = new InvocationCardHandler(this, _cardCollectionService, _playerStatusProvider);
        CardHandlerMap[CardType.Effect] = new EffectCardHandler(this, _cardCollectionService, _playerStatusProvider);
        CardHandlerMap[CardType.Contre] = new ContreCardHandler(this, _cardCollectionService, _playerStatusProvider);
        CardHandlerMap[CardType.Field] = new FieldCardHandler(this, _cardCollectionService, _playerStatusProvider);
        CardHandlerMap[CardType.Equipment] = new EquipmentCardHandler(this, _cardCollectionService, _playerStatusProvider);
    }

    /// <summary>
    /// Unity's start method, called before the first frame update. Initializes UI states and card handlers.
    /// </summary>
    private void Start()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        EventClick.AddListener(ClickOnCard);
        InitializeCardHandlers();
    }

    /// <summary>
    /// Handles the event of clicking on a card.
    /// </summary>
    /// <param name="card">The card that was clicked on.</param>
    private void ClickOnCard(InGameCard card)
    {
        CurrentSelectedCard = card;
        if (CardHandlerMap.TryGetValue(card.Type, out var handler))
        {
            handler.HandleCard(card);
        }
        else
        {
            Debug.LogError($"Unexpected card type: {card.Type}");
        }

        var clickPosition = GetClickPosition();
        DisplayMiniMenuCardAtPosition(clickPosition);
    }

    /// <summary>
    /// Gets the click or touch position based on the platform.
    /// </summary>
    /// <returns>
    /// Returns the mouse position in the Unity editor, touch position on Android, 
    /// and Vector3.zero as a default for other platforms.
    /// </returns>
    protected Vector3 GetClickPosition()
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#elif UNITY_ANDROID
    return Input.GetTouch(0).position;
#else
    return Vector3.zero; // Default
#endif
    }


    /// <summary>
    /// Displays a mini-menu at the specified position, usually next to the clicked card.
    /// </summary>
    /// <param name="mousePosition">The position where the mini-menu should appear.</param>
    protected void DisplayMiniMenuCardAtPosition(Vector3 mousePosition)
    {
        miniMenuCard.transform.position = mousePosition + padding;
        if (!miniMenuCard.activeSelf)
        {
            miniMenuCard.SetActive(true);
        }
    }

    /// <summary>
    /// Handles the "Put Card" action, triggering the appropriate event based on the card's type.
    /// </summary>
    public void ClickPutCard()
    {
        if (CardHandlerMap.TryGetValue(CurrentSelectedCard.Type, out var handler))
        {
            handler.HandleCardPut(CurrentSelectedCard);
        }
        else
        {
            Debug.LogError($"Unexpected card type: {CurrentSelectedCard.Type}");
        }

        miniMenuCard.SetActive(false);
        if (!detailCardPanel.activeSelf) return;
        // The detail panel is visible. One must go back to card display
        detailCardPanel.SetActive(false);
        handScreen.SetActive(true);
        inHandButton.SetActive(true);
    }

    /// <summary>
    /// Manages the card detail view, toggling between card details and hand view.
    /// </summary>
    public void DetailCardClick()
    {
        if (detailCardPanel.activeSelf)
        {
            detailButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_DETAILS));
            miniMenuCard.SetActive(false);
            detailCardPanel.SetActive(false);
            handScreen.SetActive(true);
            inHandButton.SetActive(true);
            // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
            // Phase 23: Publish to EventBus instead of static UnityEvent
            var playerCards = _cardCollectionService.GetCurrentPlayerCards();
            var domainOwner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
            _eventBus.Publish(new HandCardsDisplayChangedEvent
            {
                Player = domainOwner,
                HandCards = playerCards.HandCards
            });
        }
        else
        {
            handScreen.SetActive(false);

            miniMenuCard.transform.position = buttonGroupPosition;

            detailButtonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_BACK));
            detailCardPanel.transform.GetChild(0).gameObject.GetComponent<CardDisplay>().InGameCard =
                CurrentSelectedCard;
            detailCardPanel.SetActive(true);
            inHandButton.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the hand card display on and off.
    /// </summary>
    public void ClickHandCard()
    {
        invocationMenu.SetActive(false);
        if (handScreen.activeSelf)
        {
            HideHand();
        }
        else
        {
            DisplayHand();
        }
    }

    /// <summary>
    /// Displays the hand cards.
    /// </summary>
    private void DisplayHand()
    {
        handScreen.SetActive(true);
        backgroundInformation.SetActive(false);
        buttonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_BACK));
        // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
        // Phase 23: Publish to EventBus instead of static UnityEvent
        var playerCards = _cardCollectionService.GetCurrentPlayerCards();
        var domainOwner = playerCards.IsPlayerOne ? JDG.Domain.CardOwner.Player1 : JDG.Domain.CardOwner.Player2;
        _eventBus.Publish(new HandCardsDisplayChangedEvent
        {
            Player = domainOwner,
            HandCards = playerCards.HandCards
        });
    }

    /// <summary>
    /// Hides the hand cards display.
    /// </summary>
    private void HideHand()
    {
        miniMenuCard.SetActive(false);
        detailCardPanel.SetActive(false);
        handScreen.SetActive(false);
        backgroundInformation.SetActive(true);
        buttonText.SetText(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.BUTTON_HAND));
    }
}