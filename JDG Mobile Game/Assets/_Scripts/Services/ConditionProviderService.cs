using JDG.Application.Services;

/// <summary>
/// Provides conditions using ConditionLibrary.
/// Phase 56: Wraps legacy singleton for DI-compatible access.
/// </summary>
public class ConditionProviderService : IConditionProvider
{
    /// <summary>
    /// Gets a condition by its name.
    /// </summary>
    /// <param name="conditionName">The condition name to look up.</param>
    /// <returns>The condition, or null if not found.</returns>
    public object GetCondition(object conditionName)
    {
        if (conditionName is not ConditionName name)
            return null;

#pragma warning disable CS0618 // Type or member is obsolete
        var library = ConditionLibrary.Instance;
        if (library == null || library.ConditionDictionary == null)
            return null;

        library.ConditionDictionary.TryGetValue(name, out var condition);
        return condition;
#pragma warning restore CS0618
    }

    /// <summary>
    /// Gets a typed condition by its name.
    /// </summary>
    /// <param name="conditionName">The condition name to look up.</param>
    /// <returns>The condition, or null if not found.</returns>
    public Condition GetConditionTyped(ConditionName conditionName)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var library = ConditionLibrary.Instance;
        if (library == null || library.ConditionDictionary == null)
            return null;

        library.ConditionDictionary.TryGetValue(conditionName, out var condition);
        return condition;
#pragma warning restore CS0618
    }

    /// <summary>
    /// Checks if a condition exists for the given name.
    /// </summary>
    /// <param name="conditionName">The condition name to check.</param>
    /// <returns>True if the condition exists.</returns>
    public bool HasCondition(object conditionName)
    {
        if (conditionName is not ConditionName name)
            return false;

#pragma warning disable CS0618 // Type or member is obsolete
        var library = ConditionLibrary.Instance;
        if (library == null || library.ConditionDictionary == null)
            return false;

        return library.ConditionDictionary.ContainsKey(name);
#pragma warning restore CS0618
    }
}
