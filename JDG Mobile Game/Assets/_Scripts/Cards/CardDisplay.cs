using Cards;
using JDG.Application;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// Phase 24-25: Added VContainer injection for IEventBus and ICardCollectionService.
/// Phase 7: Added IAbilityProvider for ability system migration.
/// Phase 61: Added all ability providers (required by CardFactory).
/// </summary>
public class CardDisplay : MonoBehaviour
{
    private InGameCard _inGameCard;
    private Card _card;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private bool isFaceHidden;
    private Image image;

    // Phase 62: ICardFactory for card creation (replaces individual providers)
    private ICardFactory _cardFactory;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 62: Simplified - uses ICardFactory instead of individual ability providers.
    /// </summary>
    [Inject]
    public void Construct(ICardFactory cardFactory)
    {
        _cardFactory = cardFactory;
    }

    /// <summary>
    /// Public property for accessing and setting the InGameCard. When set, it also initializes the card and updates its material.
    /// </summary>
    public InGameCard InGameCard
    {
        get => _inGameCard;
        set
        {
            _inGameCard = value;
            InitializeCard();
            UpdateCardMaterial();
        }
    }

    /// <summary>
    /// Public property for accessing and setting the Card. When set, it also initializes the card and updates its material.
    /// </summary>
    public Card Card
    {
        get => _card;
        set
        {
            _card = value;
            InitializeCard();
            UpdateCardMaterial();
        }
    }

    /// <summary>
    /// Determines the current material to be used for the card, based on whether the card face is hidden or the Card is null.
    /// </summary>
    private Material CurrentMaterial
    {
        get
        {
            if (isFaceHidden || Card == null)
            {
                return defaultMaterial;
            }
            return Card.MaterialCard;
        }
    }

    /// <summary>
    /// Initializes the component, ensuring it's done before any Start methods in other scripts.
    /// </summary>
    private void Awake()
    {
        image = GetComponent<Image>();
        UpdateCardMaterial();
    }

    /// <summary>
    /// Initializes the card. If the Card exists and InGameCard doesn't, a new InGameCard is created.
    /// If Card doesn't exist but InGameCard does, the base card of the InGameCard is set as the Card.
    /// Phase 62: Uses ICardFactory instead of static CardFactory.CreateInGameCard.
    /// </summary>
    private void InitializeCard()
    {
        if (Card != null && InGameCard == null)
        {
            InGameCard = _cardFactory?.CreateCard(Card, JDG.Domain.CardOwner.NotDefined) as InGameCard;
        }
        else if (Card == null && InGameCard != null)
        {
            image = GetComponent<Image>();
            Card = InGameCard.BaseCard;
        }
    }

    /// <summary>
    /// NOT USED BUT MAYBE USEFUL LATER
    /// Makes the card face visible and updates the card material.
    /// </summary>
    public void ShowCardFace()
    {
        isFaceHidden = false;
        UpdateCardMaterial();
    }

    /// <summary>
    /// NOT USED BUT MAYBE USEFUL LATER
    /// Hides the card face and updates the card material.
    /// </summary>
    public void HideCardFace()
    {
        isFaceHidden = true;
        UpdateCardMaterial();
    }

    /// <summary>
    /// Updates the material used for the card's display.
    /// Includes null safety to prevent NullReferenceException when called before Awake().
    /// </summary>
    private void UpdateCardMaterial()
    {
        // Ensure image component is available (may be called before Awake)
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        if (image == null)
        {
            Debug.LogError($"CardDisplay.UpdateCardMaterial: Image component not found on {gameObject.name}");
            return;
        }

        var material = CurrentMaterial;
        if (material != null)
        {
            image.material = material;
        }
        else
        {
            Debug.LogWarning($"CardDisplay.UpdateCardMaterial: CurrentMaterial is null for card '{Card?.Title ?? "Unknown"}'");
            image.material = defaultMaterial;
        }
    }
}