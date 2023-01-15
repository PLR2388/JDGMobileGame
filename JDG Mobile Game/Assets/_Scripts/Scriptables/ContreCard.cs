using UnityEngine;

namespace Cards.ContreCards
{
    [CreateAssetMenu(fileName = "NewContreCard", menuName = "ContreCard")]
    public class ContreCard : Card
    {
        private void Awake()
        {
            type = CardType.Contre;
        }
    }
}