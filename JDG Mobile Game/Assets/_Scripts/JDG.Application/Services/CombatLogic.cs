using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Services
{
    /// <summary>
    /// Implementation of pure combat logic.
    /// No Unity dependencies - fully testable.
    /// Extracted from CombatService in Phase: Test coverage extraction.
    /// </summary>
    public class CombatLogic : ICombatLogic
    {
        /// <summary>
        /// Computes the damage result of an attack.
        /// Returns defender's remaining defense (negative means destroyed).
        /// </summary>
        public float ComputeDamage(float attackerAttack, float defenderDefense)
        {
            return defenderDefense - attackerAttack;
        }

        /// <summary>
        /// Checks if any card in the list has aggro.
        /// </summary>
        public bool HasAggroCard(IEnumerable<CombatCardInfo> cards)
        {
            if (cards == null)
                return false;

            return cards.Any(card => card != null && card.Aggro);
        }

        /// <summary>
        /// Filters to only cards with aggro.
        /// </summary>
        public List<CombatCardInfo> GetOnlyAggroCards(IEnumerable<CombatCardInfo> cards)
        {
            if (cards == null)
                return new List<CombatCardInfo>();

            return cards.Where(card => card != null && card.Aggro).ToList();
        }

        /// <summary>
        /// Removes cards that can't be attacked from the list.
        /// </summary>
        public List<CombatCardInfo> RemoveCantBeAttackedCards(IEnumerable<CombatCardInfo> cards)
        {
            if (cards == null)
                return new List<CombatCardInfo>();

            return cards.Where(card => card != null && !card.CantBeAttacked).ToList();
        }

        /// <summary>
        /// Determines if the player should be a valid target.
        /// Player is a valid target when:
        /// - No valid card targets exist (field is empty)
        /// - OR attacker has direct attack ability
        /// </summary>
        public bool ShouldAddPlayerToTarget(int validTargetCount, bool hasDirectAttackAbility)
        {
            return validTargetCount == 0 || hasDirectAttackAbility;
        }

        /// <summary>
        /// Builds the list of valid targets for an attacker.
        ///
        /// Rules:
        /// 1. If any opponent card has Aggro, only Aggro cards can be targeted
        /// 2. Cards with CantBeAttacked are removed (unless they have Aggro)
        /// 3. Player is a valid target if:
        ///    - Field is empty (no valid card targets)
        ///    - OR attacker has CanDirectAttack
        ///    - OR any effect enables direct attack
        /// </summary>
        public CombatTargetResult BuildValidTargets(
            IEnumerable<CombatCardInfo> opponentCards,
            bool attackerCanDirectAttack,
            bool hasDirectAttackEffect)
        {
            var result = new CombatTargetResult();

            if (opponentCards == null)
            {
                result.IncludesPlayer = true;
                return result;
            }

            // Filter out null and invalid cards
            var validCards = opponentCards
                .Where(card => card != null && !string.IsNullOrEmpty(card.Title))
                .ToList();

            // Check for aggro cards
            result.HasAggroCards = HasAggroCard(validCards);

            if (result.HasAggroCards)
            {
                // Only aggro cards can be targeted
                result.ValidTargets = GetOnlyAggroCards(validCards);
            }
            else
            {
                // Remove cards that can't be attacked
                result.ValidTargets = RemoveCantBeAttackedCards(validCards);

                // Determine if player is a valid target
                bool fieldEffectivelyEmpty = result.ValidTargets.Count == 0;
                result.IncludesPlayer = fieldEffectivelyEmpty || hasDirectAttackEffect;
            }

            // Attacker with CanDirectAttack can always target player
            if (attackerCanDirectAttack && !result.IncludesPlayer)
            {
                result.IncludesPlayer = true;
            }

            return result;
        }
    }
}
