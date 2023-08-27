using Cards;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public InGameCard inGameCard;
    public Card card;
    [SerializeField] private Material defaultMaterial;
    public bool isFaceHidden;
    private Image image;

    // Start is called before the first frame update
    private void Start()
    {
        InitializeCard();
    }
    
    private void InitializeCard()
    {

        image = GetComponent<Image>();
        image.material = defaultMaterial;
        if (card != null && inGameCard == null)
        {
            inGameCard = InGameCard.CreateInGameCard(card, CardOwner.NotDefined);
        }

        if (card == null && inGameCard != null)
        {
            card = inGameCard.baseCard;
        }
    }

    private void Update()
    {
        if (isFaceHidden)
        {
            image.material = defaultMaterial;
        }
        else
        {
            image.material = card ? card.MaterialCard : defaultMaterial;
        }
    }
}