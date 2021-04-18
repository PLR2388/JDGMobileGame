using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractPhysicalCards : MonoBehaviour
{


    [SerializeField] private GameObject bigImageCard;

    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = 0f;
    }
    
    private void DisplayCurrentCard(Card card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.MaterialCard;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime > 1f)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                string tag = hitInfo.transform.gameObject.tag;

                if (tag == "card1" || tag == "card2")
                {
                    GameObject cardObject = hitInfo.transform.gameObject;
                    DisplayCurrentCard(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            startTime += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            startTime = 0f;
            bigImageCard.SetActive(false);
        }

    }
}