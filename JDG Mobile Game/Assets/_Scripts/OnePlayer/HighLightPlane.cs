using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OnePlayer
{
    /// <summary>
    /// Enum representing different highlight elements.
    /// </summary>
    public enum HighlightElement
    {
        Invocations, Space, Deck, YellowTrash, Effect, Field, InHandButton, NextPhaseButton, Tentacules, LifePoints
    }

    /// <summary>
    /// Custom UnityEvent for the highlight feature. It contains the highlight element and a boolean indicating its activation state.
    /// </summary>
    [System.Serializable]
    public class HighlightEvent : UnityEvent<HighlightElement, bool>
    {
    }

    /// <summary>
    /// Component responsible for handling the visual highlighting of certain game elements.
    /// </summary>
    public class HighLightPlane : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;
        
        private const float PulseDuration = 0.5f;
        private static readonly Color PulseColor = Color.green;

        private bool isActivated;
        private bool waitEndTurn = true;
        
        private MeshRenderer meshRenderer;
        
        /// <summary>
        /// Global event to notify listeners of highlight status changes.
        /// </summary>
        public static readonly HighlightEvent Highlight = new HighlightEvent();
        
        /// <summary>
        /// Initialize component references.
        /// </summary>
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        
        /// <summary>
        /// Set up event listeners when the component starts.
        /// </summary>
        private void Start()
        {
            Highlight.AddListener(UpdateStatus);
        }
        
        /// <summary>
        /// Ensure event listeners are cleaned up when the component is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Highlight.RemoveListener(UpdateStatus);
        }

        /// <summary>
        /// Updates the activation status of the highlight.
        /// </summary>
        /// <param name="highlightElement">The element to check.</param>
        /// <param name="activated">Whether the highlight is activated or not.</param>
        private void UpdateStatus(HighlightElement highlightElement, bool activated)
        {
            if (highlightElement == element)
            {
                isActivated = activated;
            }
        }

        /// <summary>
        /// Handles the visual update of the highlight effect every frame.
        /// </summary>
        void Update()
        {
            if (isActivated)
            {
                if (waitEndTurn)
                {
                    StartCoroutine(PulseCoroutine());
                }
            }
            else
            {
                meshRenderer.material.color = Color.clear;
                waitEndTurn = true;
            }
        }

        /// <summary>
        /// Coroutine that manages the pulsing highlight effect.
        /// </summary>
        /// <returns>An IEnumerator to be used in a Coroutine.</returns>
        private IEnumerator PulseCoroutine()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(PulseDuration);
            meshRenderer.material.color = PulseColor;
            yield return new WaitForSeconds(PulseDuration);
            meshRenderer.material.color = Color.clear;
            waitEndTurn = true;
        }
    }
}