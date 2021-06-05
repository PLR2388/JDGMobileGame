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
                if (invocationCards[i].Nom == null)
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
                if (invocationCards[i].Nom == null)
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

    private void DealWithStartEffect(InvocationCard currentInvocationCard,InvocationStartEffect invocationStartEffect)
    {
        List<StartEffect> keys = invocationStartEffect.Keys;
        List<String> values = invocationStartEffect.Values;

        String cardName = "";
        String typeCard = "";
        String familyName = "";
        List<String> invokeCardNames = new List<string>();
        List<Card> cardFound = new List<Card>();
        bool mustDividAttack = false;
        bool mustDividDefense = false; 
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
                                if (deck[j].Nom == cardName)
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
                                    if (deck[j].Nom == invokeCardName)
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
                                        "Voulez-vous aussi invoquer " + cardFound[0].Nom + " ?";
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
                                    message.GetComponent<MessageBox>().title = "Choix de l'invocation";
                                    message.GetComponent<MessageBox>().description = "";
                                    message.GetComponent<MessageBox>().displayCardsScript.cardslist = cardFound;
                                    message.GetComponent<MessageBox>().displayCards = true;
                                
                                    
                                    message.GetComponent<MessageBox>().positiveAction = () =>
                                    {
                                        InvocationCard invocationCard =
                                            (InvocationCard) message.GetComponent<MessageBox>().getSelectedCard();
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
                                if (deck[j].Type == typeCard)
                                {
                                    cardFound.Add(deck[j]);
                                }
                            }

                            if (cardFound.Count > 0)
                            {
                                
                            }
                            
                        }
                        else if (familyName != "")
                        {
                            for (int j = 0; j < deck.Count; j++)
                            {
                                if (deck[i].Type == "invocation")
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
                                if (trash[j].Nom == cardName)
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
                                    if (trash[j].Nom == invokeCardName)
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
                                if (trash[j].Type == typeCard)
                                {
                                    cardFound.Add(trash[j]);
                                }
                            }
                        }
                        else if (familyName != "")
                        {
                            for (int j = 0; j < trash.Count; j++)
                            {
                                if (trash[i].Type == "invocation")
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
                        if (P1InvocationCards[j].Nom != dontRemoveCard)
                        {
                            P1.GetComponent<PlayerCards>().handCards.Add(P1InvocationCards[j]);
                            P1.GetComponent<PlayerCards>().InvocationCards[j] = null;
                        }
                    }

                    for (int j = 0; j < FindFirstEmptyInvocationLocation(P2InvocationCards); j++)
                    {
                        if (P2InvocationCards[j].Nom != dontRemoveCard)
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
                    if (cardFound.Count > 0)
                    { 
                        if (currentPlayerCard.Field == null) 
                        {
                            GameObject message = Instantiate(messageBox);
                            message.GetComponent<MessageBox>().title = "Choix du terrain à poser";
                            message.GetComponent<MessageBox>().displayCardsScript.cardslist = cardFound;
                            message.GetComponent<MessageBox>().displayCards = true;
                                    
                                        
                            message.GetComponent<MessageBox>().positiveAction = () =>
                            {
                                FieldCard fieldCard =
                                    (FieldCard) message.GetComponent<MessageBox>().getSelectedCard();

                                if (fieldCard != null)
                                {
                                    currentPlayerCard.Field = fieldCard;
                                    currentPlayerCard.Deck.Remove(fieldCard);
                                    Destroy(message);
                                }
                                else
                                {
                                    Destroy(message);
                                    GameObject informativeMessage = Instantiate(messageBox);
                                    informativeMessage.GetComponent<MessageBox>().title = "Information";
                                    informativeMessage.GetComponent<MessageBox>().description =
                                        "Aucune carte n'a été choisie";
                                    informativeMessage.GetComponent<MessageBox>().isInformation = true;
                                }
                               
                            };
                            message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
                        }
                        else
                        {
                            GameObject message = Instantiate(messageBox);
                            message.GetComponent<MessageBox>().title = "Choix du terrain à prendre en main";
                            message.GetComponent<MessageBox>().displayCardsScript.cardslist = cardFound;
                            message.GetComponent<MessageBox>().displayCards = true;
                                    
                                        
                            message.GetComponent<MessageBox>().positiveAction = () =>
                            {
                                FieldCard fieldCard =
                                    (FieldCard) message.GetComponent<MessageBox>().getSelectedCard();

                                if (fieldCard != null)
                                {
                                    currentPlayerCard.Deck.Add(fieldCard);
                                    currentPlayerCard.Deck.Remove(fieldCard);
                                    Destroy(message);
                                }
                                else
                                {
                                    Destroy(message);
                                    GameObject informativeMessage = Instantiate(messageBox);
                                    informativeMessage.GetComponent<MessageBox>().title = "Information";
                                    informativeMessage.GetComponent<MessageBox>().description =
                                        "Aucune carte n'a été choisie";
                                    informativeMessage.GetComponent<MessageBox>().isInformation = true;
                                }
                            };
                            message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
                        } 
                    }
                }
                    break;
                case StartEffect.DestroyField:
                {
                    FieldCard fieldCardP1 = P1.GetComponent<PlayerCards>().Field;
                    FieldCard fieldCardP2 = P2.GetComponent<PlayerCards>().Field;

                    if (fieldCardP1 != null)
                    {
                        cardFound.Add(fieldCardP1);
                    }

                    if (fieldCardP2 != null)
                    {
                        cardFound.Add(fieldCardP2);
                    }

                    if (cardFound.Count > 0)
                    {
                        GameObject message = Instantiate(messageBox);
                        message.GetComponent<MessageBox>().title = "Choix du terrain à détruire";
                        message.GetComponent<MessageBox>().displayCardsScript.cardslist = cardFound;
                        message.GetComponent<MessageBox>().displayCards = true;
                        
                                                            
                        message.GetComponent<MessageBox>().positiveAction = () =>
                        {
                            FieldCard fieldCard =
                                (FieldCard) message.GetComponent<MessageBox>().getSelectedCard();

                            if (fieldCard != null)
                            {
                                if (mustDividAttack)
                                {
                                    currentInvocationCard.setBonusAttack(-currentInvocationCard.GetAttack()/2);
                                }

                                if (mustDividDefense)
                                {
                                    currentInvocationCard.setBonusAttack(-currentInvocationCard.GetDefense()/2);
                                }
                                currentPlayerCard.Deck.Add(fieldCard);
                                currentPlayerCard.Deck.Remove(fieldCard);
                                Destroy(message);
                            }
                            else
                            {
                                Destroy(message);
                                GameObject informativeMessage = Instantiate(messageBox);
                                informativeMessage.GetComponent<MessageBox>().title = "Information";
                                informativeMessage.GetComponent<MessageBox>().description =
                                    "Aucune carte n'a été choisie";
                                informativeMessage.GetComponent<MessageBox>().isInformation = true;
                            }
                        };

                        message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
                    }
                }
                    break;
                case StartEffect.Divide2ATK:
                {
                    mustDividAttack = true;
                }
                    break;
                case StartEffect.Divide2DEF:
                {
                    mustDividDefense = true;
                }
                    break;
                case StartEffect.SendToDeath:
                {
                    PlayerCards opponentPlayerCards = null;
                    if (GameLoop.isP1Turn)
                    {
                        opponentPlayerCards = P2.GetComponent<PlayerCards>();
                        InvocationCard[] P2InvocationCards = opponentPlayerCards.InvocationCards;

                        for (int j = 0; j < P2InvocationCards.Length; j++)
                        {
                            if (P2InvocationCards[j] != null && P2InvocationCards[j].Nom != null)
                            {
                                cardFound.Add(P2InvocationCards[i]);
                            }
                        }
                    }
                    else
                    {
                        opponentPlayerCards = P1.GetComponent<PlayerCards>();
                        InvocationCard[] P1InvocationCards = opponentPlayerCards.InvocationCards;

                        for (int j = 0; j < P1InvocationCards.Length; j++)
                        {
                            if (P1InvocationCards[j] != null && P1InvocationCards[j].Nom != null)
                            {
                                cardFound.Add(P1InvocationCards[i]);
                            }
                        }
                    }
                    
                    
                    GameObject message = Instantiate(messageBox);
                    message.GetComponent<MessageBox>().title = "Choix de la carte à tuer :";
                    message.GetComponent<MessageBox>().displayCardsScript.cardslist = cardFound;
                    message.GetComponent<MessageBox>().displayCards = true;
                    message.GetComponent<MessageBox>().positiveAction = () =>
                    {
                        InvocationCard invocationCardSelected = (InvocationCard)message.GetComponent<MessageBox>().getSelectedCard();
                        if (invocationCardSelected != null)
                        {
                            InvocationCard[] invocationCards = opponentPlayerCards.InvocationCards;
                            int k = 0;
                            bool found = false;
                            while (!found && k < invocationCards.Length)
                            {
                                if (invocationCards[k] != null &&
                                    invocationCards[k].Nom == invocationCardSelected.Nom)
                                {
                                    found = true;
                                }
                                else
                                {
                                    k++;
                                }
                            }

                            if (found)
                            {
                                if (GameLoop.isP1Turn)
                                {
                                    P2.GetComponent<PlayerCards>().InvocationCards[k] = null;
                                    P2.GetComponent<PlayerCards>().YellowTrash.Add(invocationCardSelected);
                                }
                                else
                                {
                                    P1.GetComponent<PlayerCards>().InvocationCards[k] = null;
                                    P1.GetComponent<PlayerCards>().YellowTrash.Add(invocationCardSelected);
                                }
                                currentInvocationCard.incrementNumberDeaths();
                            }
                            else
                            {
                                Debug.Log("Something went wrong!");
                            }
                            Destroy(message);
                        }
                        else
                        {
                            Destroy(message);
                            GameObject informativeMessage = Instantiate(messageBox);
                            informativeMessage.GetComponent<MessageBox>().title = "Information";
                            informativeMessage.GetComponent<MessageBox>().description =
                                "Aucune carte n'a été choisie";
                            informativeMessage.GetComponent<MessageBox>().isInformation = true;
                        }
                    };
                    message.GetComponent<MessageBox>().negativeAction = () =>
                    {
                        Destroy(message);
                    };
                }
                    break;
                case StartEffect.DrawXCards:
                {
                    int X = int.Parse(values[i]);
                    int size = currentPlayerCard.Deck.Count;
                    if (size >= 0)
                    {
                        int j = size - 1;
                        while (j >= 0 && X != 0)
                        {
                            Card c = currentPlayerCard.Deck[j];
                            currentPlayerCard.handCards.Add(c);
                            currentPlayerCard.Deck.RemoveAt(j);
                            j--;
                            X--;
                        }
                    }
                    
                }
                    break;
                case StartEffect.Condition:
                {
                    string condition = values[i];
                    switch (condition)
                    {
                        case "skipAttack":
                        {
                            currentInvocationCard.blockAttack();
                        }
                            break;
                    }
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
            currentPlayerCard.InvocationCards[size] = invocationCard;

            currentPlayerCard.handCards.Remove(invocationCard);

            InvocationStartEffect invocationStartEffect = invocationCard.GetInvocationStartEffect();
            InvocationActionEffect invocationActionEffect = invocationCard.InvocationActionEffect;

            if (invocationStartEffect != null)
            {
                DealWithStartEffect(invocationCard,invocationStartEffect);
            }

            if (invocationActionEffect != null && CheckIfContainsSacrificeInvocation(invocationActionEffect))
            {
                AskIfUserWantToUseActionEffect(invocationCard, invocationActionEffect);
            }
        }
    }

    private bool CheckIfContainsSacrificeInvocation(InvocationActionEffect invocationActionEffect)
    {
        return invocationActionEffect.Keys[0] == ActionEffect.SacrificeInvocation;
    }

    private void AskIfUserWantToUseActionEffect(InvocationCard invocationCard, InvocationActionEffect invocationActionEffect)
    {
        List<ActionEffect> keys = invocationActionEffect.Keys;
        List<string> values = invocationActionEffect.Values;
        string cardName = null;
        string fieldName = null;
        for (int i = 0; i < keys.Count; i++)
        {
            switch (keys[i])
            {
                case ActionEffect.SacrificeInvocation:
                    cardName = values[i];
                break;
                case ActionEffect.SpecificField:
                    fieldName = values[i];
                    break;
                case ActionEffect.IncreaseStarsATKAndDEF:
                {
                    float valueStars = float.Parse(values[i]);
                    if (fieldName != null && cardName != null)
                    {
                        bool isPossible = currentPlayerCard.Field.Nom == fieldName;
                        if (isPossible)
                        {
                            InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
                            AskToSacrifice(invocationCard, invocationCards, cardName, valueStars);
                        }

                    }
                    else if (cardName != null)
                    {
                        InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
                        AskToSacrifice(invocationCard, invocationCards, cardName, valueStars);
                    }
                }
                    break;
                case ActionEffect.BackToLife:
                    break;
                
            }
        }
    }

    private void AskToSacrifice(InvocationCard invocationCard, InvocationCard[] invocationCards, string cardName,
        float valueStars)
    {
        int j = 0;
        bool cardFound = false;
        Card card = null;
        while (j < invocationCards.Length && !cardFound)
        {
            if (invocationCards[j] != null && invocationCards[j].Nom == cardName)
            {
                cardFound = true;
                card = invocationCards[j];
            }

            j++;
        }

        if (cardFound)
        {
            GameObject message = Instantiate(messageBox);
            message.GetComponent<MessageBox>().title = "Amélioration";
            message.GetComponent<MessageBox>().description =
                "Voulez-vous augmenter de " + valueStars + " l'ATQ et la DEF de " + invocationCard.Nom + " en sacrifiant " +
                cardName + " ?";
            message.GetComponent<MessageBox>().positiveAction = () =>
            {
                currentPlayerCard.sendCardToYellowTrash(card);
                invocationCard.setBonusAttack(valueStars);
                invocationCard.setBonusDefense(valueStars);

                Destroy(message);
            };
            message.GetComponent<MessageBox>().negativeAction = () => { Destroy(message); };
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
            if (invocationCards[j] != null && invocationCards[j].Nom == cardName)
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
            if (invocationCards[j] != null && invocationCards[j].Nom == cardName)
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
                isChecked = invocationCard.getEquipmentCard().Nom == equipmentName;
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
            isCorrectField = fieldCard.Nom == fieldName;
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
                if (invocationCard != null && invocationCard.Nom != null && invocationCard.GetAttack() >= value)
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
            if (card.Type == "invocation")
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