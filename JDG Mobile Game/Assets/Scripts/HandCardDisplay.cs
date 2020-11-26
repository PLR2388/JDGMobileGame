using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    //[SerializeField] private List<Card> AllCards;

    public GameObject Player1;

    public GameObject Player2;
    
    private GameLoop GameLoop;

    private List<GameObject> createdCards;

    private void Awake()
    {
        GameLoop=GameObject.Find("GameLoop").GetComponent<GameLoop>();
        createdCards = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (GameLoop.isP1Turn)
        {
            List<Card> handCard = Player1.GetComponent<PlayerCards>().handCards;
            if (createdCards.Count != handCard.Count)
            {
                for (int i = 0; i < handCard.Count; i++)
                {
                    GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                    newCard.transform.SetParent(transform,true);
                    newCard.GetComponent<CardDisplay>().card = handCard[i];
                    createdCards.Add(newCard);
                }
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(420 * handCard.Count,rectTransform.sizeDelta.y);
            }

            
        }
        else
        {
            List<Card> handCard = Player2.GetComponent<PlayerCards>().handCards;
            if (createdCards.Count != handCard.Count)
            {
                for (int i = 0; i < handCard.Count; i++)
                {
                    GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                    newCard.transform.SetParent(transform,true);
                    newCard.GetComponent<CardDisplay>().card = handCard[i];
                    createdCards.Add(newCard);
                }
                RectTransform rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(420 * handCard.Count,rectTransform.sizeDelta.y);
            }
        }
    }

    private void OnDisable()
    {
        if (createdCards.Count > 0)
        {
            for (int i = 0; i < createdCards.Count; i++)
            {
                Destroy(createdCards[i]);
            }
            createdCards.Clear();
        }
    }
}
