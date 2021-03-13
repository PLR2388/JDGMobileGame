using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentFonctions : MonoBehaviour
{ 
    [SerializeField] private GameObject messageBox;
    [SerializeField] private GameObject miniCardMenu;
    void Start()
    {
        InGameMenuScript.EquipmentCardEvent.AddListener(displayEquipmentPopUp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlayerCards getCurrentPlayerCard()
    {
        PlayerCards currentPlayerCard = null;
        if (GameLoop.isP1Turn)
        {
            GameObject player = GameObject.Find("Player1");
            currentPlayerCard = player.GetComponent<PlayerCards>();
        }
        else
        {
            GameObject player = GameObject.Find("Player2");
            currentPlayerCard = player.GetComponent<PlayerCards>();
        }

        return currentPlayerCard;
    }

    public void displayEquipmentPopUp(EquipmentCard equipmentCard)
    {
        PlayerCards playerCards = getCurrentPlayerCard();
        InvocationCard[] invocationCards = playerCards.InvocationCards;
        GameObject message = displayEquipmentMessageBox(invocationCards);

        message.GetComponent<MessageBox>().positiveAction = () =>
        {
            InvocationCard currentSelectedInvocationCard = (InvocationCard)message.GetComponent<MessageBox>().getSelectedCard();
            if (currentSelectedInvocationCard != null)
            {
                int index = indexInvocationCard(invocationCards, currentSelectedInvocationCard.GetNom());
                miniCardMenu.SetActive(false);
                playerCards.EquipmentCards[index] = equipmentCard;

                playerCards.handCards.Remove(equipmentCard);
                
                Destroy(message);
            }
        };
        message.GetComponent<MessageBox>().negativeAction = () =>
        {
            miniCardMenu.SetActive(false);
            Destroy(message);
        };

    }

    private int indexInvocationCard(InvocationCard[] invocationCards,string nameInvocationCard)
    {
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null)
            {
                string invocationCardName = invocationCards[i].GetNom();
                if (invocationCardName != null)
                {
                    if (invocationCardName == nameInvocationCard)
                    {
                        return i;
                    }
                }
            }
        }
        return -1;
    }
    
    private GameObject displayEquipmentMessageBox( InvocationCard[] invocationCards)
    {
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Choisis l'invocation auquelle associée l'équipement :";

        List<Card> cards = new List<Card>();
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null && invocationCards[i].GetNom() != null)
            {
                cards.Add(invocationCards[i]);
            }
        }
        message.GetComponent<MessageBox>().displayCardsScript.cardslist = cards;
        message.GetComponent<MessageBox>().displayCards = true;

        return message;
    }


}
