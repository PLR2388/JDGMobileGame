using Cards;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    private InGameCard _inGameCard;
    private Card _card;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private bool isFaceHidden;
    private Image image;

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
    /// </summary>
    private void InitializeCard()
    {
        if (Card != null && InGameCard == null)
        {
            InGameCard = InGameCard.CreateInGameCard(Card, CardOwner.NotDefined);
        }
        else if (Card == null && InGameCard != null)
        {
            image = GetComponent<Image>();
            Card = InGameCard.baseCard;
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
    /// </summary>
    private void UpdateCardMaterial()
    {
        image.material = CurrentMaterial;
    }
}