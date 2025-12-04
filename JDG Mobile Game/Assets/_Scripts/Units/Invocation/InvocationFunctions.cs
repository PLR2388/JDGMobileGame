using System;
using _Scripts.Units.Invocation;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Cards.InvocationCards
{

    /// <summary>
    /// Event class that's triggered when an invocation needs to be cancelled.
    /// </summary>
    [Serializable]
    public class CancelInvocationEvent : UnityEvent<InGameInvocationCard>
    {
    }

    /// <summary>
    /// Handles the operations related to invocation cards, including placing them on the field,
    /// canceling their effects, and more.
    /// Phase 6: Refactored to delegate business logic to ICardPlacementService.
    /// </summary>
    public class InvocationFunctions : MonoBehaviour
    {
        [SerializeField] protected Transform canvas;

        /// <summary>
        /// Public event that is raised to cancel an invocation.
        /// </summary>
        public static readonly CancelInvocationEvent CancelInvocationEvent = new CancelInvocationEvent();

        // Phase 6: Use service for business logic
        private ICardPlacementService CardPlacementService => ServiceLocator.Get<ICardPlacementService>();


        /// <summary>
        /// Sets up the initial state and event listeners.
        /// </summary>
        private void Start()
        {
            // Attach listeners
            AttachInvocationEventListeners();
        }


        /// <summary>
        /// Attaches listeners for invocation events.
        /// </summary>
        private void AttachInvocationEventListeners()
        {
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            CancelInvocationEvent.AddListener(OnCancelEffect);
        }

        /// <summary>
        /// Processes the cancellation effect on an invocation card.
        /// Phase 6: Delegates to CardPlacementService.
        /// </summary>
        /// <param name="invocationCard">The invocation card to process.</param>
        private void OnCancelEffect(InGameInvocationCard invocationCard)
        {
            CardPlacementService.HandleInvocationCancelEffect(invocationCard);
        }

        /// <summary>
        /// Places the invocation card on the field and applies its effect.
        /// Phase 6: Delegates to CardPlacementService.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            bool success = CardPlacementService.PlaceInvocationCard(invocationCard, canvas);

            if (!success)
            {
                // Show warning if field is full (4 invocations max)
                var config = new MessageBoxConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_LIMIT_INVOCATION_CARDS),
                    showOkButton: true
                );
                MessageBox.Instance.CreateMessageBox(canvas, config);
            }
        }
    }
}