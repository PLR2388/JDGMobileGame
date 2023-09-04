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
        [SerializeField] private PlayerCards player1Cards;
        [SerializeField] private PlayerCards player2Cards;
        [SerializeField] private Transform canvas;
        private PlayerCards currentPlayerCard;
        private PlayerCards opponentPlayerCards;

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
            GameStateManager.ChangePlayer.AddListener(ChangePlayer);
            AttachInvocationEventListeners();
            InitializePlayers();
        }


        /// <summary>
        /// Attaches listeners for invocation events.
        /// </summary>
        private void AttachInvocationEventListeners()
        {
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            TutoInGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
            CancelInvocationEvent.AddListener(OnCancelEffect);
        }

        /// <summary>
        /// Initializes the starting player states.
        /// </summary>
        private void InitializePlayers()
        {
            currentPlayerCard = player1Cards;
            opponentPlayerCards = player2Cards;
        }

        /// <summary>
        /// Changes the current player, swapping the roles of player and opponent.
        /// </summary>
        private void ChangePlayer()
        {
            Swap(ref currentPlayerCard, ref opponentPlayerCards);
        }

        /// <summary>
        /// Swaps the references of two objects.
        /// </summary>
        /// <typeparam name="T">Type of objects to be swapped.</typeparam>
        /// <param name="a">First object.</param>
        /// <param name="b">Second object.</param>
        private static void Swap<T>(ref T a, ref T b)
        {
            (a, b) = (b, a);
        }

        /// <summary>
        /// Processes the cancellation effect on an invocation card.
        /// </summary>
        /// <param name="invocationCard">The invocation card to process.</param>
        private void OnCancelEffect(InGameInvocationCard invocationCard)
        {
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

        private bool CanAddCardToField()
        {
            return currentPlayerCard.InvocationCards.Count < 4;
        }

        /// <summary>
        /// Adds the specified invocation card to the field.
        /// </summary>
        /// <param name="invocationCard">The invocation card to add.</param>
        private void AddCardToField(InGameInvocationCard invocationCard)
        {
            currentPlayerCard.InvocationCards.Add(invocationCard);
            currentPlayerCard.HandCards.Remove(invocationCard);
        }

        /// <summary>
        /// Applies the effect of the specified invocation card.
        /// </summary>
        /// <param name="invocationCard">The invocation card whose effect should be applied.</param>
        private void ApplyCardEffect(InGameInvocationCard invocationCard)
        {
            foreach (var ability in invocationCard.Abilities)
            {
                ability.ApplyEffect(canvas, currentPlayerCard, opponentPlayerCards);
            }
        }
    }
}