using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OnePlayer
{
    public enum HighlightElement
    {
        Invocations, Space, Deck, YellowTrash, Effect, Field
    }

    [System.Serializable]
    public class HighlightEvent : UnityEvent<HighlightElement, bool>
    {
    }

    public class HighLightPlane : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;
        
        private bool isActivated = false;
        private bool waitEndTurn = true;
        
        public static readonly HighlightEvent Highlight = new HighlightEvent();
        // Start is called before the first frame update
        void Start()
        {
            Highlight.AddListener(UpdateStatus);
        }

        void UpdateStatus(HighlightElement element, bool isActivated)
        {
            if (element == this.element)
            {
                this.isActivated = isActivated;
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