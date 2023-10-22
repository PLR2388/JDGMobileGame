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
    /// </summary>
    public class InvocationFunctions : MonoBehaviour
    {
        [SerializeField] protected Transform canvas;

        /// <summary>
        /// Public event that is raised to cancel an invocation.
        /// </summary>
        public static readonly CancelInvocationEvent CancelInvocationEvent = new CancelInvocationEvent();


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
        /// </summary>
        /// <param name="invocationCard">The invocation card to process.</param>
        private void OnCancelEffect(InGameInvocationCard invocationCard)
        {
            var currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();
            if (invocationCard.CancelEffect)
            {
                foreach (var ability in invocationCard.Abilities)
                {
                    ability.CancelEffect(currentPlayerCard);
                }
            }
            else
            {
                foreach (var ability in invocationCard.Abilities)
                {
                    ability.ReactivateEffect(currentPlayerCard);
                }
            }
        }

        /// <summary>
        /// Places the invocation card on the field and applies its effect.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>

        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            if (CanAddCardToField())
            {
                AddCardToField(invocationCard);
                ApplyCardEffect(invocationCard);
            }
        }

        /// <summary>
        /// Checks if a card can be added to the field based on existing conditions.
        /// </summary>
        /// <returns>True if card can be added, false otherwise.</returns>

        protected bool CanAddCardToField()
        {
            var currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();
            return currentPlayerCard.InvocationCards.Count < 4;
        }

        /// <summary>
        /// Adds the specified invocation card to the field.
        /// </summary>
        /// <param name="invocationCard">The invocation card to add.</param>
        protected void AddCardToField(InGameInvocationCard invocationCard)
        {
            var currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();
            currentPlayerCard.InvocationCards.Add(invocationCard);
            currentPlayerCard.HandCards.Remove(invocationCard);
        }

        /// <summary>
        /// Applies the effect of the specified invocation card.
        /// </summary>
        /// <param name="invocationCard">The invocation card whose effect should be applied.</param>
        private void ApplyCardEffect(InGameInvocationCard invocationCard)
        {
            var currentPlayerCard = CardManager.Instance.GetCurrentPlayerCards();
            var opponentPlayerCards = CardManager.Instance.GetOpponentPlayerCards();
            foreach (var ability in invocationCard.Abilities)
            {
                ability.ApplyEffect(canvas, currentPlayerCard, opponentPlayerCards);
            }
        }
    }
}