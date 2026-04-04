using System.Collections.Generic;
using System.Linq;

namespace JDG.Application.Services
{
    /// <summary>
    /// Implementation of pure card placement logic.
    /// No Unity dependencies - fully testable.
    /// Extracted from CardPlacementService in Phase: Test coverage extraction.
    /// </summary>
    public class CardPlacementLogic : ICardPlacementLogic
    {
        /// <summary>
        /// Maximum invocations allowed on field per player.
        /// </summary>
        public int MaxInvocations => 4;

        /// <summary>
        /// Maximum effects allowed on field per player.
        /// </summary>
        public int MaxEffects => 4;

        /// <summary>
        /// Checks if a new invocation card can be placed on the field.
        /// Field can hold maximum 4 invocation cards.
        /// </summary>
        public bool CanPlaceInvocation(int currentInvocationCount)
        {
            return currentInvocationCount < MaxInvocations;
        }

        /// <summary>
        /// Checks if a new effect card can be placed on the field.
        /// Field can hold maximum 4 effect cards.
        /// </summary>
        public bool CanPlaceEffect(int currentEffectCount)
        {
            return currentEffectCount < MaxEffects;
        }

        /// <summary>
        /// Checks if a field card can be placed.
        /// Only one field card allowed at a time.
        /// </summary>
        public bool CanPlaceFieldCard(bool hasFieldCard)
        {
            return !hasFieldCard;
        }

        /// <summary>
        /// Gets the list of valid targets for equipment placement.
        ///
        /// Rules:
        /// - If equipment has CanAlwaysBePut ability, all invocations are valid targets
        /// - Otherwise, only invocations without equipment are valid targets
        /// </summary>
        public List<EquipmentTargetInfo> GetValidEquipmentTargets(
            IEnumerable<EquipmentTargetInfo> allInvocations,
            bool canAlwaysBePut)
        {
            if (allInvocations == null)
                return new List<EquipmentTargetInfo>();

            if (canAlwaysBePut)
            {
                // All invocations are valid targets
                return allInvocations
                    .Where(inv => inv != null)
                    .ToList();
            }

            // Only invocations without equipment are valid
            return allInvocations
                .Where(inv => inv != null && !inv.HasEquipment)
                .ToList();
        }
    }
}
