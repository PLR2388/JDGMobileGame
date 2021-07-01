using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ContreCard")]
public class ContreCard : Card
{
    private void Awake()
    {
        this.type = CardType.Contre;
    }
}