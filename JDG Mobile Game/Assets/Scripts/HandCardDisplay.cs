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

    private void Awake()
    {
        GameLoop=GameObject.Find("GameLoop").GetComponent<GameLoop>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameLoop.isP1Turn)
        {
            List<Card> handCard = Player1.GetComponent<PlayerCards>().handCards;
            for (int i = 0; i < handCard.Count; i++)
            {
                GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                newCard.transform.SetParent(transform,true);
                newCard.GetComponent<CardDisplay>().card = handCard[i];
            }
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100 * handCard.Count,rectTransform.sizeDelta.y);
            
        }
        else
        {
            List<Card> handCard = Player2.GetComponent<PlayerCards>().handCards;
            for (int i = 0; i < handCard.Count; i++)
            {
                GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                newCard.transform.SetParent(transform,true);
                newCard.GetComponent<CardDisplay>().card = handCard[i];
            }
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(420 * handCard.Count,rectTransform.sizeDelta.y);
        }

   

    }

    void Update()
    {
        
    }
}
