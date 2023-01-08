﻿using Cards;
using UnityEngine;
using UnityEngine.UI;

public class InteractPhysicalCards : MonoBehaviour
{
    [SerializeField] private GameObject bigImageCard;
    [SerializeField] private GameObject handScreen;

    private float startTime;

    // Start is called before the first frame update
    private void Start()
    {
        startTime = 0f;
    }

    private void DisplayCurrentCard(InGameCard card)
    {
        bigImageCard.SetActive(true);
        bigImageCard.GetComponent<Image>().material = card.MaterialCard;
    }

    // Update is called once per frame
    private void Update()
    {
        if (startTime > 1f)
        {
            if (Camera.main == null || handScreen.activeSelf) return;
            var hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo);
            if (hit)
            {
                var objectTag = hitInfo.transform.gameObject.tag;

                if (objectTag == "card1" || objectTag == "card2")
                {
                    var cardObject = hitInfo.transform.gameObject;
                    DisplayCurrentCard(cardObject.GetComponent<PhysicalCardDisplay>().card);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            startTime += Time.deltaTime;
        }

        if (!Input.GetMouseButtonUp(0)) return;
        startTime = 0f;
        bigImageCard.SetActive(false);
    }
}