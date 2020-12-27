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
    [SerializeField] private GameObject miniCardMenu;
    [SerializeField] private GameObject messageBox;

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

    private int FindFirstEmptyInvocationLocationCurrentPlayer()
    {
        InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
        bool end = false;
        int i = 0;
        while (i < 4 && !end)
        {
            if (invocationCards[i] != null)
            {
                if (invocationCards[i].GetNom() == null)
                {
                    end = true;
                }
                else
                {
                    i++;
                }
            }
            else
            {
                end = true;
            }
        }

        return i;
    }

    private int FindFirstEmptyInvocationLocation(InvocationCard[] invocationCards)
    {
        bool end = false;
        int i = 0;
        while (i < 4 && !end)
        {
            if (invocationCards[i] != null)
            {
                if (invocationCards[i].GetNom() == null)
                {
                    end = true;
                }
                else
                {
                    i++;
                }
            }
            else
            {
                end = true;
            }
        }

        return i;
    }

    private void DealWithStartEffect(InvocationStartEffect invocationStartEffect)
    {
        List<StartEffect> keys = invocationStartEffect.Keys;
        List<String> values = invocationStartEffect.Values;

        String cardName = "";
        String typeCard = "";
        String familyName = "";
        List<String> invokeCardNames = new List<string>();

        List<Card> cardFound = new List<Card>();

        for (int i = 0; i < keys.Count; i++)
        {
            switch (keys[i])
            {
                case StartEffect.GetSpecificCard:
                {
                    cardName = values[i];
                }
                    break;
                case StartEffect.GetSpecificTypeCard:
                {
                    typeCard = values[i];
                }
                    break;
                case StartEffect.GetCardFamily:
                {
                    familyName = values[i];
                }
                    break;
                case StartEffect.GetCardSource:
                {
                    String source = values[i];
                    if (source == "deck")
                    {
                        List<Card> deck = currentPlayerCard.Deck;
                        if (cardName != "")
                        {
                            bool isFound = false;
                            int j = 0;
                            while (j < deck.Count && !isFound)
                            {
                                if (deck[j].GetNom() == cardName)
                                {
                                    isFound = true;
                                    cardFound.Add(deck[j]);
                                }

                                j++;
                            }

                            if (isFound)
                            {
                                GameObject message = Instantiate(messageBox);
                                message.GetComponent<MessageBox>().title = "Carte en main";

                                message.GetComponent<MessageBox>().description =
                                    "Voulez-vous aussi ajouter " + cardName + " à votre main ?";
                                message.GetComponent<MessageBox>().positiveAction = () =>
                                {
                                    currentPlayerCard.handCards.Add(cardFound[0]);

                                    currentPlayerCard.Deck.Remove(cardFound[0]);

                                    Destroy(message);
                                };
                                message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
                            }
                        }
                        else if (invokeCardNames.Count > 0)
                        {
                            for (int j = 0; j < deck.Count; j++)
                            {
                                foreach (var invokeCardName in invokeCardNames)
                                {
                                    if (deck[j].GetNom() == invokeCardName)
                                    {
                                        cardFound.Add(deck[j]);
                                    }
                                }
                            }

                            int size = FindFirstEmptyInvocationLocationCurrentPlayer();
                            if (cardFound.Count > 0 && size < 4)
                            {
                                if (cardFound.Count == 1)
                                {
                                    GameObject message = Instantiate(messageBox);
                                    message.GetComponent<MessageBox>().title = "Invocation";
                                    message.GetComponent<MessageBox>().description =
                                        "Voulez-vous aussi invoquer " + cardFound[0].GetNom() + " ?";
                                    message.GetComponent<MessageBox>().positiveAction = () =>
                                    {
                                        InvocationCard invocationCard = (InvocationCard) cardFound[cardFound.Count - 1];
                                        currentPlayerCard.InvocationCards[size] = invocationCard;

                                        currentPlayerCard.Deck.Remove(invocationCard);

                                        Destroy(message);
                                    };
                                    message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
                                }
                                else
                                {
                                    GameObject message = Instantiate(messageBox);
                                    message.GetComponent<MessageBox>().title = "Invocation";
                                    message.GetComponent<MessageBox>().positiveText = cardFound[0].GetNom();
                                    message.GetComponent<MessageBox>().negativeText = cardFound[1].GetNom();
                                    message.GetComponent<MessageBox>().description =
                                        "Voulez-vous aussi invoquer " + invokeCardNames[0] + " ou " +
                                        invokeCardNames[1] + " ?";
                                    message.GetComponent<MessageBox>().positiveAction = () =>
                                    {
                                        InvocationCard invocationCard = (InvocationCard) cardFound[cardFound.Count - 1];
                                        currentPlayerCard.InvocationCards[size] = invocationCard;

                                        currentPlayerCard.Deck.Remove(invocationCard);

                                        Destroy(message);
                                    };
                                    message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
                                }
                            }
                        }
                        else if (typeCard != "")
                        {
                            for (int j = 0; j < deck.Count; j++)
                            {
                                if (deck[j].GetType() == typeCard)
                                {
                                    cardFound.Add(deck[j]);
                                }
                            }
                        }
                        else if (familyName != "")
                        {
                            for (int j = 0; j < deck.Count; j++)
                            {
                                if (deck[i].GetType() == "invocation")
                                {
                                    InvocationCard invocationCard = (InvocationCard) deck[i];

                                    String[] listFamily = invocationCard.GetFamily();
                                    for (int k = 0; k < listFamily.Length; k++)
                                    {
                                        if (listFamily[k] == familyName)
                                        {
                                            cardFound.Add(invocationCard);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (source == "trash")
                    {
                        List<Card> trash = currentPlayerCard.YellowTrash;
                        if (cardName != "")
                        {
                            bool isFound = false;
                            int j = 0;
                            while (j < trash.Count && !isFound)
                            {
                                if (trash[j].GetNom() == cardName)
                                {
                                    isFound = true;
                                    cardFound.Add(trash[j]);
                                }

                                j++;
                            }
                        }
                        else if (invokeCardNames.Count>0)
                        {
                            for (int j = 0; j < trash.Count; j++)
                            {
                                foreach (var invokeCardName in invokeCardNames)
                                {
                                    if (trash[j].GetNom() == invokeCardName)
                                    {
                                        cardFound.Add(trash[j]);
                                    }
                                }
                            }
                        }
                        else if (typeCard != "")
                        {
                            for (int j = 0; j < trash.Count; j++)
                            {
                                if (trash[j].GetType() == typeCard)
                                {
                                    cardFound.Add(trash[j]);
                                }
                            }
                        }
                        else if (familyName != "")
                        {
                            for (int j = 0; j < trash.Count; j++)
                            {
                                if (trash[i].GetType() == "invocation")
                                {
                                    InvocationCard invocationCard = (InvocationCard) trash[i];

                                    String[] listFamily = invocationCard.GetFamily();
                                    for (int k = 0; k < listFamily.Length; k++)
                                    {
                                        if (listFamily[k] == familyName)
                                        {
                                            cardFound.Add(invocationCard);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                    break;
                case StartEffect.RemoveAllInvocationCards:
                {
                    String dontRemoveCard = values[i];
                    InvocationCard[] P1InvocationCards = this.P1.GetComponent<PlayerCards>().InvocationCards;
                    InvocationCard[] P2InvocationCards = this.P2.GetComponent<PlayerCards>().InvocationCards;


                    for (int j = 0; j < FindFirstEmptyInvocationLocation(P1InvocationCards); j++)
                    {
                        if (P1InvocationCards[j].GetNom() != dontRemoveCard)
                        {
                            P1.GetComponent<PlayerCards>().handCards.Add(P1InvocationCards[j]);
                            P1.GetComponent<PlayerCards>().InvocationCards[j] = null;
                        }
                    }

                    for (int j = 0; j < FindFirstEmptyInvocationLocation(P2InvocationCards); j++)
                    {
                        if (P2InvocationCards[j].GetNom() != dontRemoveCard)
                        {
                            P2.GetComponent<PlayerCards>().handCards.Add(P2InvocationCards[j]);
                            P2.GetComponent<PlayerCards>().InvocationCards[j] = null;
                        }
                    }
                }
                    break;
                case StartEffect.InvokeSpecificCard:
                {
                    invokeCardNames.Add(values[i]);
                }
                    break;
                case StartEffect.PutField:
                {
                }
                    break;
                case StartEffect.DestroyField:
                {
                }
                    break;
                case StartEffect.Divide2ATK:
                {
                }
                    break;
                case StartEffect.Divide2DEF:
                {
                }
                    break;
                case StartEffect.SendToDeath:
                {
                }
                    break;
                case StartEffect.DrawXCards:
                {
                }
                    break;
                case StartEffect.Condition:
                {
                }
                    break;
            }
        }
    }

    public void PutInvocationCard(InvocationCard invocationCard)
    {
        int size = FindFirstEmptyInvocationLocationCurrentPlayer();

        if (size < 4)
        {
            miniCardMenu.SetActive(false);
            currentPlayerCard.InvocationCards[size] = invocationCard;

            currentPlayerCard.handCards.Remove(invocationCard);

            InvocationStartEffect invocationStartEffect = invocationCard.GetInvocationStartEffect();

            if (invocationStartEffect != null)
            {
                DealWithStartEffect(invocationStartEffect);
            }
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

    private static InvocationCard CheckSpecificCardOnField(string cardName, PlayerCards currentPlayerCards)
    {
        bool found = false;
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        int size = invocationCards.Length;
        InvocationCard invocationCard = null;

        int j = 0;
        while (j < size && !found)
        {
            if (invocationCards[j] != null && invocationCards[j].GetNom() == cardName)
            {
                found = true;
                invocationCard = invocationCards[j];
            }

            j++;
        }

        return invocationCard;
    }

    private static InvocationCard CheckSacrificeSpecificCard(string cardName, PlayerCards currentPlayerCards)
    {
        InvocationCard foundCard = null;
        bool found = false;
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        int size = invocationCards.Length;
        int j = 0;

        while (j < size && !found)
        {
            if (invocationCards[j] != null && invocationCards[j].GetNom() == cardName)
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

    private static bool CheckSpecificField(string fieldName, PlayerCards currentPlayerCards)
    {
        bool isCorrectField = false;
        FieldCard fieldCard = currentPlayerCards.Field;
        if (fieldCard)
        {
            isCorrectField = fieldCard.GetNom() == fieldName;
        }

        return isCorrectField;
    }

    private static List<InvocationCard> CheckFamily(string familyName, PlayerCards currentPlayerCards)
    {
        List<InvocationCard> sameFamilyCards = new List<InvocationCard>();
        InvocationCard[] invocationCards = currentPlayerCards.InvocationCards;
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null)
            {
                string[] families = invocationCards[i].GetFamily();

                foreach (var family in families)
                {
                    if (family == familyName)
                    {
                        sameFamilyCards.Add(invocationCards[i]);
                    }
                }
            }
        }

        return sameFamilyCards;
    }

    private static List<InvocationCard> CheckThreshold(bool isAttack, float value, PlayerCards currentPlayerCards)
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

    public static bool isInvocationPossible(InvocationConditions conditions, string playerName)
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
                        InvocationCard invocationCard = CheckSpecificCardOnField(cardName, currentPlayerCard);
                        isInvocationPossible = (invocationCard != null);
                    }
                        break;
                    case Condition.SacrificeSpecificCard:
                    {
                        string cardName = cardExplanation[i];
                        if (!specificCardFound)
                        {
                            specificCardFound = CheckSacrificeSpecificCard(cardName, currentPlayerCard);
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
                        isInvocationPossible = CheckSpecificField(fieldName, currentPlayerCard);
                    }
                        break;
                    case Condition.SacrificeFamily:
                    {
                        string familyName = cardExplanation[i];
                        sacrificedCards = CheckFamily(familyName, currentPlayerCard);
                        isInvocationPossible = sacrificedCards.Count > 0;
                    }
                        break;
                    case Condition.SpecificFamilyOnField:
                    {
                        string familyName = cardExplanation[i];
                        sacrificedCards = CheckFamily(familyName, currentPlayerCard);
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
                        if (sacrificedCards.Count > 0)
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
                            sacrificedCards = CheckThreshold(true, threshold, currentPlayerCard);
                        }
                    }
                        break;
                    case Condition.SacrificeThresholdDEF:
                    {
                        int threshold = Int32.Parse(cardExplanation[i]);
                        if (sacrificedCards.Count > 0)
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
                            sacrificedCards = CheckThreshold(false, threshold, currentPlayerCard);
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