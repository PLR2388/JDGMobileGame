using JDG.Domain;

namespace JDG.Application.Abilities.Implementations
{
    /// <summary>
    /// A no-op ability used as a placeholder or default when no specific ability is assigned.
    /// Always succeeds and does nothing.
    /// </summary>
    public class DefaultAbility : IAbility
    {
        public AbilityName Name => AbilityName.Default;
        public string Description => "No effect";

        public bool CanActivate(AbilityContext context) => true;

        public AbilityResult Execute(AbilityContext context)
        {
            return AbilityResult.Success("No effect");
        }
    }
}
