using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Jobs;
using UnityEngine;

public class InvocationFonctions : MonoBehaviour
{
    [SerializeField] private GameObject previousPlayer;
    [SerializeField] private GameObject currentPlayer;

    private PlayerCards currentPlayerCards;
    // Start is called before the first frame update
    void Start()
    {
        currentPlayerCards = currentPlayer.GetComponent<PlayerCards>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer != previousPlayer)
        {
            previousPlayer = currentPlayer;
            currentPlayerCards = currentPlayer.GetComponent<PlayerCards>();
        }
    }

    bool isInvocationCardOnField(string invocationCardName)
    {
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i].GetNom() == invocationCardName)
            {
                return true;
            }
        }

        return false;
    }

    bool isInvocationCardOfTheSameFamilyOnField(string currentCardName, string familyName)
    {
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        for (int i = 0; i < invocationCards.Length; i++)
        {
            
            if (invocationCards[i].GetNom() != currentCardName)
            {
                string[] families = invocationCards[i].GetFamily();
                for (int j = 0; j < families.Length; j++)
                {
                    if (families[i] == familyName)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    bool isInvocationPossible(string necessaryInvocation, FieldCard necessaryField)
    {
        FieldCard fieldCard = currentPlayerCards.Field;

        if (fieldCard.GetNom() == necessaryField.GetNom())
        {
            InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
            for (int i = 0; i < invocationCards.Length; i++)
            {
                if (invocationCards[i].GetNom() == necessaryInvocation)
                {
                    return true;
                }
            }
            List<Card> handCards = currentPlayerCards.handCards;
            for(int i=0;i<handCards.Count;i++)
            {
                if (handCards[i].GetNom() == necessaryInvocation)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool isInvocationPossible(string necessaryInvocation, EquipmentCard necessaryEquipment)
    {
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        bool foundInvocation = false;
        
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i].GetNom() == necessaryInvocation)
            {
                if (invocationCards[i].getEquipmentCard().GetNom() == necessaryEquipment.GetNom())
                {
                    return true;
                }

                foundInvocation = true;
            }
        }

        if (foundInvocation)
        {
            List<Card> handCards = currentPlayerCards.handCards;
            for (int i = 0; i < handCards.Count; i++)
            {
                if (handCards[i] is EquipmentCard)
                {
                    if (handCards[i].GetNom() == necessaryEquipment.GetNom())
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            List<Card> handCards = currentPlayerCards.handCards;
            for (int i = 0; i < handCards.Count; i++)
            {
                if (handCards[i] is EquipmentCard)
                {
                    if (handCards[i].GetNom() == necessaryEquipment.GetNom())
                    {
                        if (foundInvocation)
                        {
                            return true;
                        }
                    }
                }
                else if (handCards[i] is InvocationCard)
                {
                    if (handCards[i].GetNom() == necessaryInvocation)
                    {
                        foundInvocation = true;
                    }
                }
            }
        }
        return false;
    }
        

 
    
    
    
}
