using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private GameObject messageBox;

    public GameObject gameState;

    private int numberSelected = 0;
    private int numberRare = 0;

    private const int maxSelectedCards = 30;
    // Start is called before the first frame update
    void Start()
    {
        List<Card> AllCards=gameState.GetComponent<GameState>().allCards;
       for (int i = 0; i < AllCards.Count; i++)
            {
                GameObject newCard=Instantiate(prefabCard, Vector3.zero, Quaternion.identity);

                newCard.GetComponent<OnHover>().bIsInGame = false;
                newCard.transform.SetParent(transform,true);
                newCard.GetComponent<CardDisplay>().card = AllCards[i];
            }
    }

    private void displayMessageBox(String msg)
    {
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Modifie ton deck";
        message.GetComponent<MessageBox>().isInformation = true;
        message.GetComponent<MessageBox>().description = msg;
    }

    private void Update()
    {
        numberSelected = 0;
        numberRare = 0;
        Transform[] children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            GameObject gameObject = children[i].gameObject;
            if(gameObject.GetComponent<OnHover>() != null)
            {
                bool isSelected = gameObject.GetComponent<OnHover>().bIsSelected;
                if (isSelected)
                {
                    if (numberSelected < GameState.maxDeckCards)
                    {
                        numberSelected++;
                        if (gameObject.GetComponent<CardDisplay>().card.IsCollector())
                        {
                            if (numberRare < GameState.maxRare)
                            {
                                numberRare++;
                            }
                            else
                            {
                                gameObject.GetComponent<OnHover>().bIsSelected = false;
                                displayMessageBox("Tu ne peux pas avoir plus de 5 cartes brillantes !");
                            }
                        }
                    }
                    else
                    {
                        gameObject.GetComponent<OnHover>().bIsSelected = false;
                        displayMessageBox("Tu ne peux pas avoir plus de 30 cartes !");
                    }
                }

            }

        }
    }
}
