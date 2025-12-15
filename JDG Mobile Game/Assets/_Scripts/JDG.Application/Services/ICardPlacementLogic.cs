using System.Collections.Generic;

namespace JDG.Application.Services
{
    /// <summary>
    /// Pure card placement logic that can be unit tested without Unity dependencies.
    /// Extracted from CardPlacementService in Phase: Test coverage extraction.
    /// </summary>
    public interface ICardPlacementLogic
    {
        /// <summary>
        /// Checks if a new invocation card can be placed on the field.
        /// </summary>
        /// <param name="currentInvocationCount">Current number of invocations on field.</param>
        /// <returns>True if placement is allowed.</returns>
        bool CanPlaceInvocation(int currentInvocationCount);

        /// <summary>
        /// Checks if a new effect card can be placed on the field.
        /// </summary>
        /// <param name="currentEffectCount">Current number of effects on field.</param>
        /// <returns>True if placement is allowed.</returns>
        bool CanPlaceEffect(int currentEffectCount);

        /// <summary>
        /// Checks if a field card can be placed.
        /// </summary>
        /// <param name="hasFieldCard">Whether a field card is already placed.</param>
        /// <returns>True if placement is allowed.</returns>
        bool CanPlaceFieldCard(bool hasFieldCard);

        /// <summary>
        /// Gets the list of valid targets for equipment placement.
        /// </summary>
        /// <param name="allInvocations">All invocation cards (both players).</param>
        /// <param name="canAlwaysBePut">Whether the equipment can be put on any card.</param>
        /// <returns>List of valid target cards.</returns>
        List<EquipmentTargetInfo> GetValidEquipmentTargets(
            IEnumerable<EquipmentTargetInfo> allInvocations,
            bool canAlwaysBePut);

        /// <summary>
        /// Maximum number of invocation cards allowed on field.
        /// </summary>
        int MaxInvocations { get; }

        /// <summary>
        /// Maximum number of effect cards allowed on field.
        /// </summary>
        int MaxEffects { get; }
    }

    /// <summary>
    /// Lightweight representation of an invocation for equipment targeting.
    /// </summary>
    public class EquipmentTargetInfo
    {
        public string Title { get; set; }
        public bool HasEquipment { get; set; }
        public bool IsCurrentPlayer { get; set; }

        public static EquipmentTargetInfo Create(string title, bool hasEquipment, bool isCurrentPlayer = true)
        {
            return new EquipmentTargetInfo
            {
                Title = title,
                HasEquipment = hasEquipment,
                IsCurrentPlayer = isCurrentPlayer
            };
        }
    }
}
