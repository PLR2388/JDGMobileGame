namespace JDG.Application.Services
{
    /// <summary>
    /// Service for querying combat state without exposing legacy types.
    /// Phase 40: Created to enable InvocationMenuPresenter migration to JDG.Presentation.
    ///
    /// This is a subset of ICombatService that uses only primitive return types,
    /// allowing it to be in JDG.Application assembly.
    /// </summary>
    public interface ICombatQueryService
    {
        /// <summary>
        /// Checks if the current attacker can perform an attack.
        /// </summary>
        bool CanAttackerAttack();

        /// <summary>
        /// Checks if the current attacker has actions available.
        /// </summary>
        bool HasAttackerAction();

        /// <summary>
        /// Checks if a special action is possible for the current attacker.
        /// </summary>
        bool IsSpecialActionPossible();
    }
}
