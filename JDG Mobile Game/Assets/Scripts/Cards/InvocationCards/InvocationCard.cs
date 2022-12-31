using System;
using Cards.EquipmentCards;
using UnityEngine;

namespace Cards.InvocationCards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
    public class InvocationCard : Card
    {
        [SerializeField] protected float attack;
        [SerializeField] protected float defense;
        [SerializeField] private CardFamily[] family;
        [SerializeField] private InvocationConditions invocationConditions;
        [SerializeField] private InvocationStartEffect invocationStartEffect;
        [SerializeField] private InvocationPermEffect invocationPermEffect;
        [SerializeField] private InvocationActionEffect invocationActionEffect;
        [SerializeField] private InvocationDeathEffect invocationDeathEffect;
        [SerializeField] private bool affectedByEffect = true;
        private void Awake()
        {
            type = CardType.Invocation;
        }
        
        public bool IsAffectedByEffectCard => affectedByEffect;
        public float GetAttack() => attack;
        public float GetDefense() => defense;
        public CardFamily[] Family => family;
        public InvocationConditions InvocationConditions => invocationConditions;
        public InvocationStartEffect GetInvocationStartEffect() => invocationStartEffect;
        public InvocationPermEffect InvocationPermEffect => invocationPermEffect;
        public InvocationActionEffect InvocationActionEffect => invocationActionEffect;
        public InvocationDeathEffect GetInvocationDeathEffect() => invocationDeathEffect;

    }
}