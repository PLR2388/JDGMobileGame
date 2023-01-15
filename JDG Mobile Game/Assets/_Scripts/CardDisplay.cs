using Cards;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public InGameCard inGameCard;
    public Card card;
    public Material defaultMaterial;
    public bool bIsFaceHidden;
    private Image image;

    // Start is called before the first frame update
    private void Start()
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
        if (bIsFaceHidden)
        {
            image.material = defaultMaterial;
        }
        else
        {
            image.material = card ? card.MaterialCard : defaultMaterial;
        }
    }
}