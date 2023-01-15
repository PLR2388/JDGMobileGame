using System;
using Cards;
using Cards.EquipmentCards;
using Cards.InvocationCards;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
    public class InvocationCard : Card
    {
        [SerializeField] private InvocationCardStats _invocationCardStats;

        public InvocationCardStats BaseInvocationCardStats => _invocationCardStats;

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
    public InvocationConditions InvocationConditions;
    public InvocationStartEffect InvocationStartEffect;
    public InvocationPermEffect InvocationPermEffect;
    public InvocationActionEffect InvocationActionEffect;
    public InvocationDeathEffect InvocationDeathEffect;
    public bool AffectedByEffect;
}