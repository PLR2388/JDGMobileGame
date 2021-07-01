using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "FieldCard")]
public class FieldCard : Card
{
    [SerializeField] private CardFamily family;

    private void Awake()
    {
        this.type = CardType.Field;
    }

    public CardFamily GETFamily()
    {
        return family;
    }
}