using UnityEngine;

namespace Cards.ContreCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "ContreCard")]
    public class ContreCard : Card
    {
        private void Awake()
        {
            type = CardType.Contre;
        }
    }
}