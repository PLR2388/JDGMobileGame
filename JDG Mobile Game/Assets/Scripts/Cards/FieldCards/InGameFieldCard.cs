using System;
using Cards;
using Cards.FieldCards;
using UnityEngine;

public class InGameFieldCard : InGameCard
{
    public FieldCard baseFieldCard;
    [SerializeField] private CardFamily family;
    [SerializeField] private FieldCardEffect fieldCardEffect;

    public FieldCardEffect FieldCardEffect => fieldCardEffect;


    public static InGameFieldCard Init(FieldCard fieldCard)
    {
        InGameFieldCard inGameFieldCard = new InGameFieldCard
        {
            baseFieldCard = fieldCard
        };
        inGameFieldCard.Reset();
        return inGameFieldCard;
    }
    private void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        title = baseFieldCard.Nom;
        description = baseFieldCard.Description;
        baseCard = baseFieldCard;
        detailedDescription = baseFieldCard.DetailedDescription;
        type = baseFieldCard.Type;
        materialCard = baseFieldCard.MaterialCard;
        collector = baseFieldCard.Collector;
        family = baseFieldCard.GetFamily();
        fieldCardEffect = baseFieldCard.FieldCardEffect;
        CardOwner = baseFieldCard.CardOwner;
    }

    public CardFamily GetFamily()
    {
        return family;
    }
}
