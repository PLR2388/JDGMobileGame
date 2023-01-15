using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using UnityEngine;

public class ResourceSystem : StaticInstance<ResourceSystem>
{
    public List<Card> Cards { get; private set; }
    private Dictionary<string, Card> _CardsDict;

    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        Cards = Resources.LoadAll<Card>("Cards").ToList();
        _CardsDict = Cards.ToDictionary(card => card.Nom, card => card);
    }

    public Card GetCardByName(string title) => _CardsDict[title];
}
