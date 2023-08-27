using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OnePlayer
{
    public enum HighlightElement
    {
        Invocations, Space, Deck, YellowTrash, Effect, Field, InHandButton, NextPhaseButton, Tentacules, LifePoints
    }

    [System.Serializable]
    public class HighlightEvent : UnityEvent<HighlightElement, bool>
    {
    }
    
    [System.Serializable]
    public class RemoveHighlightEvent : UnityEvent<HighlightElement>
    {
    }

    public class HighLightPlane : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;
        
        private bool isActivated = false;
        private bool waitEndTurn = true;
        
        public static readonly HighlightEvent Highlight = new HighlightEvent();
        public static readonly RemoveHighlightEvent RemoveHighlight = new RemoveHighlightEvent();
        // Start is called before the first frame update
        void Start()
        {
            Highlight.AddListener(UpdateStatus);
            RemoveHighlight.AddListener(HideStatus);
        }

        void HideStatus(HighlightElement highlightElement)
        {
            if (highlightElement == element)
            {
                gameObject.SetActive(false);
            }
        }

        void UpdateStatus(HighlightElement highlightElement, bool activated)
        {
            if (highlightElement == element)
            {
                isActivated = activated;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isActivated)
            {
                if (waitEndTurn)
                {
                    StartCoroutine("Pulse");
                }
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = Color.clear;
                waitEndTurn = true;
            }
        }

        IEnumerator Pulse()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.clear;
            waitEndTurn = true;
        }
    }
}