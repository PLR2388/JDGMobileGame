namespace JDG.Application.Services
{
    /// <summary>
    /// Service interface for invocation menu management.
    /// Phase 9: Removes InvocationMenuManager singleton dependency.
    /// </summary>
    public interface IInvocationMenuService
    {
        /// <summary>
        /// Displays the invocation menu and sets button states based on game conditions.
        /// </summary>
        /// <param name="isAttackPhase">If true, indicates that the game is in the attack phase.</param>
        void Display(bool isAttackPhase);

        /// <summary>
        /// Hides the invocation menu.
        /// </summary>
        void Hide();

        /// <summary>
        /// Enables the attack button.
        /// </summary>
        void Enable();

        /// <summary>
        /// Updates the state of the attack button based on whether the attacker can attack.
        /// </summary>
        void UpdateAttackButton();
    }
}
