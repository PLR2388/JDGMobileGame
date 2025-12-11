using UnityEngine;

namespace JDG.Presentation.Views
{
    /// <summary>
    /// View interface for displaying the invocation context menu.
    /// Part of Phase 28 - MonoBehaviour Wave 1 migration to MVP pattern.
    /// </summary>
    public interface IInvocationMenuView
    {
        /// <summary>
        /// Shows the invocation menu at the specified screen position.
        /// </summary>
        /// <param name="screenPosition">Screen position to display the menu</param>
        void ShowMenu(Vector3 screenPosition);

        /// <summary>
        /// Hides the invocation menu.
        /// </summary>
        void HideMenu();

        /// <summary>
        /// Sets the attack button's visibility and interactability.
        /// </summary>
        /// <param name="visible">Whether the attack button should be visible</param>
        /// <param name="interactable">Whether the attack button should be interactable</param>
        void SetAttackButtonState(bool visible, bool interactable);

        /// <summary>
        /// Sets the action button's visibility and interactability.
        /// </summary>
        /// <param name="visible">Whether the action button should be visible</param>
        /// <param name="interactable">Whether the action button should be interactable</param>
        void SetActionButtonState(bool visible, bool interactable);

        /// <summary>
        /// Enables the attack button.
        /// Legacy method for backward compatibility.
        /// </summary>
        void EnableAttackButton();
    }
}
