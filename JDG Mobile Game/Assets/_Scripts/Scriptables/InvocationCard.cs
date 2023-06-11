using System;
using System.Collections.Generic;
using Cards;
using Cards.InvocationCards;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
    public class InvocationCard : Card
    {
        [SerializeField] private InvocationCardStats _invocationCardStats;

        public InvocationCardStats BaseInvocationCardStats => _invocationCardStats;

        public List<ConditionName> Conditions = new List<ConditionName>();
        public List<AbilityName> Abilities = new List<AbilityName>();

        private void Awake()
        {
            type = CardType.Invocation;
        }
    }
}

[Serializable]
public struct InvocationCardStats
{
    public float Attack;
    public float Defense;
    public CardFamily[] Families;
    public bool AffectedByEffect;
}