using System.Collections;
using UnityEngine;

namespace OnePlayer
{
    /// <summary>
    /// Provides functionality to highlight a physical card in the game.
    /// </summary>
    public class HighLightPhysicalCard : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;
        [SerializeField] private Color pulseColor = Color.green;
        [SerializeField] private Color defaultColor = Color.white;


        private static readonly string TargetCardName = CardNameMappings.CardNameMap[CardNames.Tentacules];

        private bool isActivated = false;
        private bool waitEndTurn = true;

        private MeshRenderer meshRenderer;
        private PhysicalCardDisplay cardDisplay;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            cardDisplay = GetComponent<PhysicalCardDisplay>();
        }

        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Start()
        {
            HighLightPlane.Highlight.AddListener(UpdateStatus);
        }

        /// <summary>
        /// Unsubscribe from events when this object is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            HighLightPlane.Highlight.RemoveListener(UpdateStatus);
        }

        /// <summary>
        /// Updates the activation status of the highlight.
        /// </summary>
        /// <param name="highlightElement">The element to be checked.</param>
        /// <param name="activated">Whether the element is activated.</param>
        private void UpdateStatus(HighlightElement highlightElement, bool activated)
        {
            if (highlightElement == element && cardDisplay.Card.Title == TargetCardName)
            {
                isActivated = activated;
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        private void Update()
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
                meshRenderer.material.color = defaultColor;
                waitEndTurn = true;
            }
        }
        
        /// <summary>
        /// Coroutine to handle the pulsing effect on the card.
        /// </summary>
        private IEnumerator PulseCoroutine()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(0.5f);
            meshRenderer.material.color = pulseColor;
            yield return new WaitForSeconds(0.5f);
            meshRenderer.material.color = defaultColor;
            waitEndTurn = true;
        }
    }
}