using Cards;
using UnityEngine;

public class PhysicalCardDisplay : MonoBehaviour
{
    private InGameCard card;
    private bool isFaceHidden = true;
    private MeshRenderer meshRenderer;
    
    /// <summary>
    /// Gets or sets the card associated with this display. Setting the card will also update its display.
    /// </summary>
    public InGameCard Card 
    {
        get => card;
        set 
        {
            card = value;
            UpdateCardDisplay();
        }
    }
    
    /// <summary>
    /// The default material to be used when a card is not set or when it's face hidden.
    /// </summary>
    public Material defaultMaterial;
    
    /// <summary>
    /// Indicates whether the card face is hidden.
    /// </summary>
    public bool IsFaceHidden => isFaceHidden;

    /// <summary>
    /// Initialization of components.
    /// </summary>
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = defaultMaterial;
    }

    /// <summary>
    /// Hides the face of the card.
    /// </summary>
    public void Hide()
    {
        isFaceHidden = true;
        meshRenderer.material = defaultMaterial;
    }

    /// <summary>
    /// Displays the card face. If the card is not set, the default material is shown.
    /// </summary>
    public void Display()
    {
        isFaceHidden = false;
        meshRenderer.material = Card != null ? Card.MaterialCard : defaultMaterial;
    }
    
    /// <summary>
    /// Updates the card's material display based on its status and assigned card.
    /// </summary>
    private void UpdateCardDisplay()
    {
        if (isFaceHidden || card == null)
        {
            meshRenderer.material = defaultMaterial;
            return;
        }

        if (card != null)
        {
            meshRenderer.material = card.MaterialCard;
        }
    }
}