namespace JDG.Application.Services
{
    /// <summary>
    /// Provides access to condition definitions for invocation cards.
    /// Phase 56: Created to replace ConditionLibrary singleton.
    /// </summary>
    public interface IConditionProvider
    {
        /// <summary>
        /// Gets a condition by its name.
        /// </summary>
        /// <param name="conditionName">The condition name to look up.</param>
        /// <returns>The condition, or null if not found.</returns>
        object GetCondition(object conditionName);

        /// <summary>
        /// Checks if a condition exists for the given name.
        /// </summary>
        /// <param name="conditionName">The condition name to check.</param>
        /// <returns>True if the condition exists.</returns>
        bool HasCondition(object conditionName);
    }
}
