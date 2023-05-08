using Cards;
using UnityEngine;

public class PhysicalCardDisplay : MonoBehaviour
{
    public InGameCard card;
    public Material defaultMaterial;
    public bool bIsFaceHidden;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = defaultMaterial;
    }

    public void Hide()
    {
        bIsFaceHidden = true;
        meshRenderer.material = defaultMaterial;
    }

    public void Display()
    {
        bIsFaceHidden = false;
        meshRenderer.material = card != null ? card.MaterialCard : defaultMaterial;
    }
}