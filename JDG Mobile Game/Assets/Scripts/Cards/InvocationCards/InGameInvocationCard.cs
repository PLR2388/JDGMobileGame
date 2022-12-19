using System;
using UnityEngine;

namespace Cards.InvocationCards
{
    public class InGameInvocationCard : InGameCard
    {
        public InvocationCard baseInvocationCard;
        [SerializeField] protected float attack;
        [SerializeField] protected float defense;
        [SerializeField] private CardFamily[] family;
        [SerializeField] private EquipmentCard equipmentCard;
        [SerializeField] private InvocationConditions invocationConditions;
        [SerializeField] private InvocationStartEffect invocationStartEffect;
        [SerializeField] private InvocationPermEffect invocationPermEffect;
        [SerializeField] private InvocationActionEffect invocationActionEffect;
        [SerializeField] private InvocationDeathEffect invocationDeathEffect;
        [SerializeField] private int numberTurnOnField;
        [SerializeField] private int numberDeaths;
        [SerializeField] private bool blockAttackNextTurn;
        [SerializeField] private bool affectedByEffect = true;
        [SerializeField] private int remainedAttackThisTurn;
        [SerializeField] private bool isControlled;

        private void Awake()
        {
            title = baseInvocationCard.Nom;
            description = baseInvocationCard.Description;
            detailedDescription = baseInvocationCard.DetailedDescription;
            type = CardType.Invocation;
            materialCard = baseInvocationCard.MaterialCard;
            collector = baseInvocationCard.Collector;
            numberTurnOnField = 0;
            numberDeaths = 0;
            remainedAttackThisTurn = 1;
        }

        public void Reset()
        {
            title = baseInvocationCard.Nom;
            description = baseInvocationCard.Description;
            detailedDescription = baseInvocationCard.DetailedDescription;
            type = CardType.Invocation;
            materialCard = baseInvocationCard.MaterialCard;
            collector = baseInvocationCard.Collector;
            numberTurnOnField = 0;
            numberDeaths = 0;
            remainedAttackThisTurn = 1;
            numberTurnOnField = 0;
            numberDeaths = 0;

            isControlled = false;
            if (invocationActionEffect != null)
            {
                var keys = invocationActionEffect.Keys;
                var values = invocationActionEffect.Values;
                for (var i = keys.Count - 1; i >= 0; i--)
                {
                    if (keys[i] != ActionEffect.Beneficiary) continue;
                    keys.Remove(keys[i]);
                    values.Remove(values[i]);
                }
            }

            attack = baseInvocationCard.GetAttack();
            defense = baseInvocationCard.GetDefense();
            family = baseInvocationCard.GetFamily();
            equipmentCard = null;
            invocationConditions = baseInvocationCard.InvocationConditions;
            invocationStartEffect = baseInvocationCard.GetInvocationStartEffect();
            invocationPermEffect = baseInvocationCard.InvocationPermEffect;
            invocationActionEffect = baseInvocationCard.InvocationActionEffect;
            invocationDeathEffect = baseInvocationCard.GetInvocationDeathEffect();

        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}