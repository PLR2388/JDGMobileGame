using System;
using System.Collections.Generic;
using Cards;
using JDG.Domain;
using JDG.Domain.Enums;
using UnityEngine;

namespace Cards.InvocationCards
{
    /// <summary>
    /// Represents an invocation card in the game, extending the base functionality of the Card class.
    /// Phase 24-25: Updated to use JDG.Domain.AbilityName instead of legacy global AbilityName.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "InvocationCard")]
    public class InvocationCard : Card
    {
        [SerializeField] private InvocationCardStats _invocationCardStats;

        /// <summary>
        /// Gets the base statistics of the invocation card.
        /// </summary>
        public InvocationCardStats BaseInvocationCardStats => _invocationCardStats;

        /// <summary>
        /// List of conditions associated with this invocation card.
        /// </summary>
        public List<ConditionName> Conditions = new List<ConditionName>();

        /// <summary>
        /// List of abilities associated with this invocation card.
        /// Phase 24-25: Now uses JDG.Domain.AbilityName (domain layer enum).
        /// </summary>
        public List<JDG.Domain.AbilityName> Abilities = new List<JDG.Domain.AbilityName>();

        private void Awake()
        {
            type = CardType.Invocation;
        }
    }
}

/// <summary>
/// Contains the statistics for an invocation card, including attack, defense, families, and effect affinity.
/// </summary>
[Serializable]
public struct InvocationCardStats
{
    /// <summary>
    /// The attack value of the invocation card.
    /// </summary>
    public float Attack;

    /// <summary>
    /// The defense value of the invocation card.
    /// </summary>
    public float Defense;

    /// <summary>
    /// The families to which this invocation card belongs.
    /// </summary>
    public CardFamily[] Families;

    /// <summary>
    /// Indicates whether the invocation card is affected by certain effects.
    /// </summary>
    public bool AffectedByEffect;
}