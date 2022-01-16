using System.Collections.Generic;
using UnityEngine;

namespace Cards.InvocatonCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "SuperInvocationCard")]
    public class SuperInvocationCard : InvocationCard
    {
        public List<InvocationCard> invocationCards;

        public void Init(List<InvocationCard> invocationCardsList) {
            invocationCards = invocationCardsList;
            var totalAttack = 0.0f;
            var totalDefense = 0.0f;
            var newName = "";
            foreach(var invocationCard in invocationCardsList) {
                totalAttack += invocationCard.GetAttack();
                totalDefense += invocationCard.GetDefense();
                newName += invocationCard.Nom;
            }
            attack = totalAttack;
            defense = totalDefense;
            nom = newName;
        }
    }
}
