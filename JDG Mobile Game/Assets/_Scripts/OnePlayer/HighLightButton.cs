using System.Collections;
using OnePlayer;
using UnityEngine;
using UnityEngine.UI;

public class HighLightButton : MonoBehaviour
{
    [SerializeField] private HighlightElement element;
    
    public bool isActivated = false;
    private bool waitEndTurn = true;
    // Start is called before the first frame update
    void Start()
    {
        HighLightPlane.Highlight.AddListener(UpdateStatus);
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
            gameObject.GetComponent<Button>().interactable = true;
            if (waitEndTurn)
            {
                StartCoroutine("Pulse");
            }
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.white;
            waitEndTurn = true;
        }
    }

    IEnumerator Pulse()
    {
        waitEndTurn = false;
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<Image>().color = Color.white;
        waitEndTurn = true;
    }
}
