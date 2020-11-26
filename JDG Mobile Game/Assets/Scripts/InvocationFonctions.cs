using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class InvocationFonctions : MonoBehaviour
{
    private PlayerCards currentPlayerCard;
    private GameObject P1;
    private GameObject P2;
    
    public void Start()
    {
        GameLoop.ChangePlayer.AddListener(ChangePlayer);
        InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
        P1 = GameObject.Find("Player1");
        P2 = GameObject.Find("Player2");
        currentPlayerCard = P1.GetComponent<PlayerCards>();
    }

    void ChangePlayer()
    {
        if (GameLoop.isP1Turn)
        {
            currentPlayerCard = P1.GetComponent<PlayerCards>();
        }
        else
        {
            currentPlayerCard = P2.GetComponent<PlayerCards>();
        }
    }

    private void PutInvocationCard(InvocationCard invocationCard)
    {
        if (currentPlayerCard.InvocationCards.Length < 4)
        {
            
        }
    }

    public void Shuffle(List<Card> cards)
    {
        int deckSize = cards.Count;
        for (int i = 0; i < deckSize; i++)
        {
            Card tmp = cards[i];
            int randomIndex = Random.Range(i, deckSize);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = cards[i];
        }
    }

    private static InvocationCard CheckSpecificCardOnField(string cardName,PlayerCards currentPlayerCards)
    {
        bool found = false;
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        int size = invocationCards.Length;
        InvocationCard invocationCard = null;

        int j = 0;
        while (j < size && !found)
        {
            if (invocationCards[j].GetNom() == cardName)
            {
                found = true;
                invocationCard = invocationCards[j];
            }

            j++;
        }

        return invocationCard;
    }

    private static InvocationCard CheckSacrificeSpecificCard(string cardName,PlayerCards currentPlayerCards)
    {
        InvocationCard foundCard = null;
        bool found = false;
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        int size = invocationCards.Length;
        int j = 0;

        while (j < size && !found)
        {
            if (invocationCards[j].GetNom() == cardName)
            {
                foundCard = invocationCards[j];
                found = true;
            }

            j++;
        }

        return foundCard;
    }

    private static bool CheckSpecificEquipmentAttached(InvocationCard invocationCard, string equipmentName)
    {
        bool isChecked = false;
        if (invocationCard)
        {
            if (invocationCard.getEquipmentCard())
            {
                isChecked = invocationCard.getEquipmentCard().GetNom() == equipmentName;
            }
        }

        return isChecked;
    }

    private static bool CheckSpecificField(string fieldName,PlayerCards currentPlayerCards)
    {
        bool isCorrectField = false;
        FieldCard fieldCard = currentPlayerCards.Field;
        if (fieldCard)
        {
            isCorrectField = fieldCard.GetNom() == fieldName;
        }

        return isCorrectField;
    }

    private static List<InvocationCard> CheckFamily(string familyName,PlayerCards currentPlayerCards)
    {
        List<InvocationCard> sameFamilyCards = new List<InvocationCard>();
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        foreach (var invocationCard in invocationCards)
        {
            string[] families = invocationCard.GetFamily();

            foreach (var family in families)
            {
                if (family == familyName)
                {
                    sameFamilyCards.Add(invocationCard);
                }
            }
        }

        return sameFamilyCards;
    }
    
    private static List<InvocationCard> CheckThreshold(bool isAttack,float value,PlayerCards currentPlayerCards)
    {
        List<InvocationCard> Threshold = new List<InvocationCard>();
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        if (isAttack)
        {
            foreach (var invocationCard in invocationCards)
            {
                if (invocationCard.GetAttack() >= value)
                {
                    Threshold.Add(invocationCard);
                }
            }
        }
        else
        {
            foreach (var invocationCard in invocationCards)
            {
                if (invocationCard.GetDefense() >= value)
                {
                    Threshold.Add(invocationCard);
                }
            }
        }

        return Threshold;
    }

    static int checkNumberInvocationCardInYellowTrash(PlayerCards currentPlayerCards)
    {
        List<Card> trashCards = currentPlayerCards.YellowTrash;
        int count = 0;
        
        foreach (var card in trashCards)
        {
            if (card.GetType() == "invocation")
            {
                count++;
            }
        }

        return count;
    }

    public static bool isInvocationPossible(InvocationConditions conditions,string playerName)
    {
        GameObject player = GameObject.Find(playerName);
        PlayerCards currentPlayerCard = player.GetComponent<PlayerCards>();
        if (conditions == null)
        {
            return true;
        }
        else
        {
            List<Condition> cardConditions = conditions.Keys;
            List<string> cardExplanation = conditions.Values;

            InvocationCard specificCardFound = null;

            List<InvocationCard> sacrificedCards = new List<InvocationCard>();

            bool isInvocationPossible = true;

            int i = 0;
            while (i < cardConditions.Count && isInvocationPossible)
            {
                switch (cardConditions[i])
                {
                    case Condition.SpecificCardOnField:
                    {
                        string cardName = cardExplanation[i];
                        InvocationCard invocationCard = CheckSpecificCardOnField(cardName,currentPlayerCard);
                        isInvocationPossible = (invocationCard != null);
                    }
                        break;
                    case Condition.SacrificeSpecificCard:
                    {
                        string cardName = cardExplanation[i];
                        if (!specificCardFound)
                        {
                            specificCardFound = CheckSacrificeSpecificCard(cardName,currentPlayerCard);
                            isInvocationPossible = specificCardFound != null;
                        }
                    }
                        break;
                    case Condition.SpecificEquipmentAttached:
                    {
                        string equipmentName = cardExplanation[i];
                        isInvocationPossible = CheckSpecificEquipmentAttached(specificCardFound, equipmentName);
                    }
                        break;
                    case Condition.SpecificField:
                    {
                        string fieldName = cardExplanation[i];
                        isInvocationPossible = CheckSpecificField(fieldName,currentPlayerCard);
                    }
                        break;
                    case Condition.SacrificeFamily:
                    {
                        string familyName = cardExplanation[i];
                        sacrificedCards = CheckFamily(familyName,currentPlayerCard);
                        isInvocationPossible = sacrificedCards.Count > 0;
                    }
                        break;
                    case Condition.SpecificFamilyOnField:
                    {
                        string familyName = cardExplanation[i];
                        sacrificedCards = CheckFamily(familyName,currentPlayerCard);
                        isInvocationPossible = sacrificedCards.Count > 0;
                    }
                        break;
                    case Condition.NumberCard:
                    {
                        int numberCard = Int32.Parse(cardExplanation[i]);
                        if (sacrificedCards.Count > 0)
                        {
                            isInvocationPossible = sacrificedCards.Count >= numberCard;
                        }
                        else
                        {
                            isInvocationPossible = false;
                        }
                    }
                        break;
                    case Condition.SacrificeThresholdATK: 
                    {
                        float threshold = float.Parse(cardExplanation[i]);
                        if(sacrificedCards.Count > 0)
                        {
                            int j = 0;
                            while (j < sacrificedCards.Count)
                            {
                                if (sacrificedCards[j].GetAttack() < threshold)
                                {
                                    sacrificedCards.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                        }
                        else
                        {
                            sacrificedCards = CheckThreshold(true, threshold,currentPlayerCard);
                        }
                    } 
                        break;
                    case Condition.SacrificeThresholdDEF:
                    {
                        int threshold = Int32.Parse(cardExplanation[i]);
                        if(sacrificedCards.Count > 0)
                        {
                            int j = 0;
                            while (j < sacrificedCards.Count)
                            {
                                if (sacrificedCards[j].GetDefense() < threshold)
                                {
                                    sacrificedCards.RemoveAt(j);
                                }
                                else
                                {
                                    j++;
                                }
                            }
                        }
                        else
                        {
                            sacrificedCards = CheckThreshold(false, threshold,currentPlayerCard);
                        }
                    }
                        break;
                    case Condition.NumberInvocationCardInYellowTrash:
                    {
                        int numberCardToHave = Int32.Parse(cardExplanation[i]);
                        int realNumber = checkNumberInvocationCardInYellowTrash(currentPlayerCard);

                        isInvocationPossible = (realNumber >= numberCardToHave);
                    }
                        break;
                    case Condition.ComeFromYellowTrash:
                    {
                        if (specificCardFound != null)
                        {
                            isInvocationPossible = (specificCardFound.getNumberDeaths() > 0);
                        }
                    }
                        break;
                }
                i++;
            }
            return isInvocationPossible;
        }
    }
}