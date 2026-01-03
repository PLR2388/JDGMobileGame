using System.Collections.Generic;
using JDG.Application.Abilities;
using JDG.Domain;
using NUnit.Framework;

namespace JDG.PlayMode.Tests.Fixtures
{
    /// <summary>
    /// Parameterized test case data for ability E2E testing.
    /// Provides TestCaseSource data for systematic ability testing across all trigger types.
    /// </summary>
    public static class AbilityTestCases
    {
        #region OnSummon Trigger Cases

        /// <summary>
        /// Test cases for OnSummon trigger abilities.
        /// These abilities activate when a card is played to the field.
        /// </summary>
        public static IEnumerable<TestCaseData> OnSummonCases()
        {
            // Draw abilities
            yield return new TestCaseData(
                AbilityName.Draw1Card,
                AbilityTrigger.OnSummon,
                "Draws 1 card when summoned"
            ).SetName("OnSummon_Draw1Card");

            yield return new TestCaseData(
                AbilityName.Draw2Cards,
                AbilityTrigger.OnSummon,
                "Draws 2 cards when summoned"
            ).SetName("OnSummon_Draw2Cards");

            yield return new TestCaseData(
                AbilityName.Draw3Cards,
                AbilityTrigger.OnSummon,
                "Draws 3 cards when summoned"
            ).SetName("OnSummon_Draw3Cards");

            // Send cards to hands ability
            yield return new TestCaseData(
                AbilityName.SendAllCardToHands,
                AbilityTrigger.OnSummon,
                "Sends all cards to hands when summoned"
            ).SetName("OnSummon_SendAllCardToHands");
        }

        #endregion

        #region OnDeath Trigger Cases

        /// <summary>
        /// Test cases for OnDeath trigger abilities.
        /// These abilities activate when a card is destroyed.
        /// </summary>
        public static IEnumerable<TestCaseData> OnDeathCases()
        {
            yield return new TestCaseData(
                AbilityName.ComesBackFromDeath,
                AbilityTrigger.OnDeath,
                "Returns to hand when destroyed"
            ).SetName("OnDeath_ComesBackFromDeath");

            yield return new TestCaseData(
                AbilityName.GiveDeathWhenDie,
                AbilityTrigger.OnDeath,
                "Destroys a target when destroyed"
            ).SetName("OnDeath_GiveDeathWhenDie");

            yield return new TestCaseData(
                AbilityName.KillEnemyIfDestroy,
                AbilityTrigger.OnDeath,
                "Kills enemy when destroyed"
            ).SetName("OnDeath_KillEnemyIfDestroy");
        }

        #endregion

        #region OnAttack Trigger Cases

        /// <summary>
        /// Test cases for OnAttack trigger abilities.
        /// These abilities activate when this card attacks.
        /// </summary>
        public static IEnumerable<TestCaseData> OnAttackCases()
        {
            yield return new TestCaseData(
                AbilityName.SkipOpponentAttackEveryTurn,
                AbilityTrigger.OnAttack,
                "Skips opponent attack every turn"
            ).SetName("OnAttack_SkipOpponentAttack");

            yield return new TestCaseData(
                AbilityName.KillOpponentInvocation,
                AbilityTrigger.OnAttack,
                "Kills opponent invocation"
            ).SetName("OnAttack_KillOpponentInvocation");
        }

        #endregion

        #region Combat Ability Cases

        /// <summary>
        /// Test cases for combat-related abilities.
        /// These affect how combat is resolved.
        /// </summary>
        public static IEnumerable<TestCaseData> CombatAbilityCases()
        {
            // Protection abilities - can't be killed
            yield return new TestCaseData(
                AbilityName.CantBeAttackKill,
                false, // canDirectAttack
                true,  // cantBeAttacked (conditional)
                false, // hasAggro
                "Protected from lethal attacks"
            ).SetName("Combat_CantBeAttackKill");

            // Protection with condition
            yield return new TestCaseData(
                AbilityName.CantBeAttackIfComics,
                false, // canDirectAttack
                true,  // cantBeAttacked (if Comics)
                false, // hasAggro
                "Cannot be targeted if Comics present"
            ).SetName("Combat_CantBeAttackIfComics");

            // Protected behind another card
            yield return new TestCaseData(
                AbilityName.ProtectedBehindStarlightUnicorn,
                false, // canDirectAttack
                true,  // cantBeAttacked (conditional)
                false, // hasAggro
                "Protected behind Starlight Unicorn"
            ).SetName("Combat_ProtectedBehindStarlightUnicorn");
        }

        #endregion

        #region Stat Modifier Cases

        /// <summary>
        /// Test cases for stat modification abilities.
        /// These change ATK/DEF values.
        /// </summary>
        public static IEnumerable<TestCaseData> StatModifierCases()
        {
            // Equipment stat bonuses
            yield return new TestCaseData(
                "Equipment",
                3f, // attackBonus
                2f, // defenseBonus
                "Adds +3 ATK and +2 DEF"
            ).SetName("StatMod_EquipmentBonus");

            yield return new TestCaseData(
                "Equipment",
                5f, // attackBonus
                0f, // defenseBonus
                "Adds +5 ATK only"
            ).SetName("StatMod_AttackOnlyBonus");

            yield return new TestCaseData(
                "Equipment",
                0f, // attackBonus
                5f, // defenseBonus
                "Adds +5 DEF only"
            ).SetName("StatMod_DefenseOnlyBonus");
        }

        #endregion

        #region Field Ability Cases

        /// <summary>
        /// Test cases for field card abilities.
        /// These affect all cards while the field card is active.
        /// </summary>
        public static IEnumerable<TestCaseData> FieldAbilityCases()
        {
            yield return new TestCaseData(
                "FamilyBoost",
                2f, // attackBoost
                2f, // defenseBoost
                "Boosts matching family cards by +2/+2"
            ).SetName("Field_FamilyBoost");

            yield return new TestCaseData(
                "DrawBonus",
                1, // extraDraws
                "Draws extra card each turn"
            ).SetName("Field_DrawBonus");
        }

        #endregion

        #region Trigger Lifecycle Cases

        /// <summary>
        /// Test cases for all trigger types to verify lifecycle.
        /// </summary>
        public static IEnumerable<TestCaseData> TriggerLifecycleCases()
        {
            yield return new TestCaseData(
                AbilityTrigger.OnSummon,
                "Triggers when card is played to field"
            ).SetName("Lifecycle_OnSummon");

            yield return new TestCaseData(
                AbilityTrigger.OnDeath,
                "Triggers when card is destroyed"
            ).SetName("Lifecycle_OnDeath");

            yield return new TestCaseData(
                AbilityTrigger.OnAttack,
                "Triggers when card attacks"
            ).SetName("Lifecycle_OnAttack");

            yield return new TestCaseData(
                AbilityTrigger.OnDefend,
                "Triggers when card is attacked"
            ).SetName("Lifecycle_OnDefend");

            yield return new TestCaseData(
                AbilityTrigger.OnTurnStart,
                "Triggers at the start of owner's turn"
            ).SetName("Lifecycle_OnTurnStart");

            yield return new TestCaseData(
                AbilityTrigger.OnTurnEnd,
                "Triggers at the end of owner's turn"
            ).SetName("Lifecycle_OnTurnEnd");

            yield return new TestCaseData(
                AbilityTrigger.Continuous,
                "Always active while card is on field"
            ).SetName("Lifecycle_Continuous");

            yield return new TestCaseData(
                AbilityTrigger.OnEquip,
                "Triggers when equipment is attached"
            ).SetName("Lifecycle_OnEquip");

            yield return new TestCaseData(
                AbilityTrigger.OnUnequip,
                "Triggers when equipment is removed"
            ).SetName("Lifecycle_OnUnequip");
        }

        #endregion

        #region Draw Ability Cases

        /// <summary>
        /// Test cases specifically for draw abilities.
        /// </summary>
        public static IEnumerable<TestCaseData> DrawAbilityCases()
        {
            yield return new TestCaseData(
                AbilityName.Draw1Card,
                1,
                "Draws exactly 1 card"
            ).SetName("Draw_1Card");

            yield return new TestCaseData(
                AbilityName.Draw2Cards,
                2,
                "Draws exactly 2 cards"
            ).SetName("Draw_2Cards");

            yield return new TestCaseData(
                AbilityName.Draw3Cards,
                3,
                "Draws exactly 3 cards"
            ).SetName("Draw_3Cards");
        }

        #endregion

        #region Sacrifice Ability Cases

        /// <summary>
        /// Test cases for sacrifice abilities.
        /// </summary>
        public static IEnumerable<TestCaseData> SacrificeAbilityCases()
        {
            yield return new TestCaseData(
                AbilityName.SacrificeArchibaldVonGrenier,
                "Archibald Von Grenier",
                "Sacrifices specific named card"
            ).SetName("Sacrifice_ArchibaldVonGrenier");
        }

        #endregion

        #region Protection Ability Cases

        /// <summary>
        /// Test cases for protection abilities.
        /// </summary>
        public static IEnumerable<TestCaseData> ProtectionAbilityCases()
        {
            yield return new TestCaseData(
                AbilityName.CantBeAttackKill,
                true,  // preventAllAttacks
                false, // conditionalProtection
                "Protected from lethal attacks"
            ).SetName("Protection_CantBeAttackKill");

            yield return new TestCaseData(
                AbilityName.CantBeAttackIfComics,
                false, // preventAllAttacks
                true,  // conditionalProtection
                "Protected if Comics are present"
            ).SetName("Protection_CantBeAttackIfComics");
        }

        #endregion

        #region Invoke From Deck Cases

        /// <summary>
        /// Test cases for abilities that summon cards from deck.
        /// </summary>
        public static IEnumerable<TestCaseData> InvokeFromDeckCases()
        {
            yield return new TestCaseData(
                AbilityName.GetNounoursFromDeck,
                "Nounours",
                "Summons Nounours from deck"
            ).SetName("InvokeFromDeck_Nounours");

            yield return new TestCaseData(
                AbilityName.GetBenzaieJeuneFromDeck,
                "Benzaie Jeune",
                "Summons Benzaie Jeune from deck"
            ).SetName("InvokeFromDeck_BenzaieJeune");
        }

        #endregion
    }

    #region Ability Test Data Structures

    /// <summary>
    /// Configuration for testing an ability's effect.
    /// </summary>
    public class AbilityTestConfig
    {
        public AbilityName AbilityName { get; set; }
        public AbilityTrigger ExpectedTrigger { get; set; }
        public string Description { get; set; }
        public bool ExpectsSuccess { get; set; } = true;
    }

    /// <summary>
    /// Configuration for testing combat ability effects.
    /// </summary>
    public class CombatAbilityTestConfig
    {
        public AbilityName AbilityName { get; set; }
        public bool CanDirectAttack { get; set; }
        public bool CantBeAttacked { get; set; }
        public bool HasAggro { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Configuration for testing stat modifier effects.
    /// </summary>
    public class StatModifierTestConfig
    {
        public string ModifierType { get; set; }
        public float AttackBonus { get; set; }
        public float DefenseBonus { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
