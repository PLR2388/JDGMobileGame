using System.Collections.Generic;

namespace Cards.InvocationCards
{
    public class InGameSuperInvocationCard : InGameInvocationCard
    {
        public List<InGameInvocationCard> invocationCards;

        public void Init(List<InGameInvocationCard> invocationCardsList)
        {
            invocationCards = invocationCardsList;
            var totalAttack = 0.0f;
            var totalDefense = 0.0f;
            var newName = "";
            foreach (var invocationCard in invocationCardsList)
            {
                totalAttack += invocationCard.Attack;
                totalDefense += invocationCard.Defense;
                newName += invocationCard.Title;
            }

            attack = totalAttack;
            defense = totalDefense;
            title = newName;
        }
    }
}
