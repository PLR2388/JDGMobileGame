using System.Collections;
using System.Collections.Generic;
using OnePlayer;
using UnityEngine;
using UnityEngine.UI;

public class HighLightCard : MonoBehaviour
{
        [SerializeField] private HighlightElement element;
        
        private bool isActivated = true;
        private bool waitEndTurn = true;
        
        // Start is called before the first frame update
        void Start()
        {
            HighLightPlane.Highlight.AddListener(UpdateStatus);
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
                gameObject.GetComponent<Image>().color = Color.clear;
                waitEndTurn = true;
            }
        }

        IEnumerator Pulse()
        {
            waitEndTurn = false;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<Image>().color = Color.clear;
            waitEndTurn = true;
        }
    }
