using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "SuperInvocationCard")]
public class SuperInvocationCard : InvocationCard
{
    List<InvocationCard> invocationCards;

    public void init(List<InvocationCard> invocationCards) {
        this.invocationCards = invocationCards;
        var attack = 0.0f;
        var defense = 0.0f;
        var name = "";
        foreach(InvocationCard invocationCard in invocationCards) {
            attack += invocationCard.GetAttack();
            defense += invocationCard.GetDefense();
            name += invocationCard.Nom;
        }
        this.attack = attack;
        this.defense = defense;
        this.nom = name;
    }
}
