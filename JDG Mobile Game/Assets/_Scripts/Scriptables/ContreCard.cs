using Cards;
using UnityEngine;

namespace _Scripts.Scriptables
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