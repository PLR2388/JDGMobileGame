using System.Collections;
using UnityEngine;

namespace OnePlayer
{
    public class HighLightPhysicalCard : MonoBehaviour
    {
        [SerializeField] private HighlightElement element;

        private bool isActivated = false;
        private bool waitEndTurn = true;

        // Start is called before the first frame update
        void Start()
        {
            OnePlayer.HighLightPlane.Highlight.AddListener(UpdateStatus);
        }

        void UpdateStatus(HighlightElement highlightElement, bool activated)
        {
            if (highlightElement == element)
            {
                if (GetComponent<PhysicalCardDisplay>().Card.Title == "Tentacules")
                {
                    isActivated = activated;
                }
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
                gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                waitEndTurn = true;
            }
        }

        IEnumerator Pulse()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
            waitEndTurn = true;
        }
    }
}