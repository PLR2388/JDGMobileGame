using JDG.Application.Services;
using JDG.Presentation.Views;
using UnityEngine;
using VContainer;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for invocation menu display logic.
    /// Part of Phase 28 - MonoBehaviour Wave 1 migration to MVP pattern.
    /// Phase 40: Moved to JDG.Presentation assembly, now uses ICombatQueryService.
    /// Handles business logic for what to display based on combat state.
    /// </summary>
    public class InvocationMenuPresenter
    {
        private readonly IInvocationMenuView _view;
        private readonly ICombatQueryService _combatService;

        [Inject]
        public InvocationMenuPresenter(
            IInvocationMenuView view,
            ICombatQueryService combatService)
        {
            _view = view;
            _combatService = combatService;
        }

        /// <summary>
        /// Displays the invocation menu at the specified position with appropriate button states.
        /// </summary>
        /// <param name="screenPosition">Screen position to display menu</param>
        /// <param name="isAttackPhase">Whether the game is in attack phase</param>
        public void Display(Vector3 screenPosition, bool isAttackPhase)
        {
            // Show the menu at the specified position
            _view.ShowMenu(screenPosition);

            // Attack button logic
            bool canAttack = _combatService.CanAttackerAttack();
            _view.SetAttackButtonState(
                visible: isAttackPhase,
                interactable: canAttack
            );

            // Action button logic
            bool hasAction = _combatService.HasAttackerAction();
            bool actionPossible = hasAction && _combatService.IsSpecialActionPossible();
            _view.SetActionButtonState(
                visible: hasAction && !isAttackPhase,
                interactable: actionPossible
            );
        }

        /// <summary>
        /// Hides the invocation menu.
        /// </summary>
        public void Hide()
        {
            _view.HideMenu();
        }

        /// <summary>
        /// Updates the attack button state based on current combat conditions.
        /// </summary>
        public void UpdateAttackButton()
        {
            bool canAttack = _combatService.CanAttackerAttack();
            _view.SetAttackButtonState(
                visible: true, // Keep visible, just update interactability
                interactable: canAttack
            );
        }

        /// <summary>
        /// Enables the attack button.
        /// Legacy method for backward compatibility.
        /// </summary>
        public void EnableAttackButton()
        {
            _view.EnableAttackButton();
        }
    }
}
