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

    public PlayerCards CurrentPlayerCard
    {
        get
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
    }

    public void displayEquipmentPopUp(EquipmentCard equipmentCard)
    {
        PlayerCards playerCards = CurrentPlayerCard;
        InvocationCard[] invocationCards = playerCards.InvocationCards;
        GameObject message = DisplayEquipmentMessageBox(invocationCards);

        message.GetComponent<MessageBox>().positiveAction = () =>
        {
            InvocationCard currentSelectedInvocationCard = (InvocationCard)message.GetComponent<MessageBox>().getSelectedCard();
            if (currentSelectedInvocationCard != null)
            {
                int index = indexInvocationCard(invocationCards, currentSelectedInvocationCard.Nom);
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
                string invocationCardName = invocationCards[i].Nom;
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
    
    private GameObject DisplayEquipmentMessageBox( InvocationCard[] invocationCards)
    {
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Choisis l'invocation auquelle associée l'équipement :";

        List<Card> cards = new List<Card>();
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null && invocationCards[i].Nom != null)
            {
                cards.Add(invocationCards[i]);
            }
        }
        message.GetComponent<MessageBox>().displayCardsScript.cardslist = cards;
        message.GetComponent<MessageBox>().displayCards = true;

        return message;
    }

    public void DealWithInstantEffect(InvocationCard invocationCard, EquipmentInstantEffect equipmentInstantEffect)
    {
        List<InstantEffect> keys = equipmentInstantEffect.Keys; 
        List<string> values = equipmentInstantEffect.Values;

        for (int i = 0; i < keys.Count; i++)
        {

            switch (keys[i])
            {
                case InstantEffect.AddATK:
                    break;
                case InstantEffect.AddDEF:
                {
                    
                }
                    break;
                case InstantEffect.MultiplyATK:
                {
                    
                }
                    break;
                case InstantEffect.MultiplyDEF:
                {
                    
                }
                    break;
                case InstantEffect.SetATK:
                {
                    
                }
                    break;
                case InstantEffect.SetDEF:
                {
                    
                }
                    break;
                case InstantEffect.BlockATK:
                {
                    
                }
                    break;
                case InstantEffect.DirectATK:
                {
                    
                }
                    break;
                case InstantEffect.SwitchEquipment:
                {
                    
                }
                    break;
                case InstantEffect.DisableBonus:
                {
                    
                }
                    break;
            }
        }
    }


}