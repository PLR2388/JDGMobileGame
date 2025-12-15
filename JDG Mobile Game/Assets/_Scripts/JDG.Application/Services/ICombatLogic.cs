using System.Collections.Generic;
using JDG.Domain;
using JDG.Domain.Entities;

namespace JDG.Application.Services
{
    /// <summary>
    /// Pure combat logic that can be unit tested without Unity dependencies.
    /// Extracted from CombatService in Phase: Test coverage extraction.
    /// </summary>
    public interface ICombatLogic
    {
        /// <summary>
        /// Computes the damage result of an attack.
        /// </summary>
        /// <param name="attackerAttack">Attacker's attack power.</param>
        /// <param name="defenderDefense">Defender's defense value.</param>
        /// <returns>The remaining defense (negative means destroyed).</returns>
        float ComputeDamage(float attackerAttack, float defenderDefense);

        /// <summary>
        /// Checks if any card in the list has aggro.
        /// </summary>
        /// <param name="cards">Cards to check.</param>
        /// <returns>True if at least one card has aggro.</returns>
        bool HasAggroCard(IEnumerable<CombatCardInfo> cards);

        /// <summary>
        /// Filters to only cards with aggro.
        /// </summary>
        /// <param name="cards">Cards to filter.</param>
        /// <returns>Only cards with aggro enabled.</returns>
        List<CombatCardInfo> GetOnlyAggroCards(IEnumerable<CombatCardInfo> cards);

        /// <summary>
        /// Removes cards that can't be attacked from the list.
        /// </summary>
        /// <param name="cards">Cards to filter.</param>
        /// <returns>Cards that can be attacked.</returns>
        List<CombatCardInfo> RemoveCantBeAttackedCards(IEnumerable<CombatCardInfo> cards);

        /// <summary>
        /// Determines if the player should be a valid target (when field is empty or has direct attack ability).
        /// </summary>
        /// <param name="validTargetCount">Number of valid card targets.</param>
        /// <param name="hasDirectAttackAbility">Whether attacker has direct attack ability.</param>
        /// <returns>True if player should be a valid target.</returns>
        bool ShouldAddPlayerToTarget(int validTargetCount, bool hasDirectAttackAbility);

        /// <summary>
        /// Builds the list of valid targets for an attacker.
        /// </summary>
        /// <param name="opponentCards">Opponent's invocation cards.</param>
        /// <param name="attackerCanDirectAttack">Whether attacker can attack directly.</param>
        /// <param name="hasDirectAttackEffect">Whether any effect enables direct attack.</param>
        /// <returns>Information about valid targets.</returns>
        CombatTargetResult BuildValidTargets(
            IEnumerable<CombatCardInfo> opponentCards,
            bool attackerCanDirectAttack,
            bool hasDirectAttackEffect);
    }

    /// <summary>
    /// Lightweight representation of a card for combat calculations.
    /// Allows combat logic to be tested without legacy InGameCard dependencies.
    /// </summary>
    public class CombatCardInfo
    {
        public string Title { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public bool Aggro { get; set; }
        public bool CantBeAttacked { get; set; }
        public bool CanDirectAttack { get; set; }

        public static CombatCardInfo Create(string title, float attack, float defense, bool aggro = false, bool cantBeAttacked = false)
        {
            return new CombatCardInfo
            {
                Title = title,
                Attack = attack,
                Defense = defense,
                Aggro = aggro,
                CantBeAttacked = cantBeAttacked
            };
        }
    }

    /// <summary>
    /// Result of building valid combat targets.
    /// </summary>
    public class CombatTargetResult
    {
        public List<CombatCardInfo> ValidTargets { get; set; } = new List<CombatCardInfo>();
        public bool IncludesPlayer { get; set; }
        public bool HasAggroCards { get; set; }
    }
}
