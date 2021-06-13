using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class EffectFonctions : MonoBehaviour
{
    [SerializeField] private GameObject miniCardMenu;
    [SerializeField] private GameObject messageBox;
    private PlayerCards currentPlayerCard;
    private PlayerStatus currentPlayerStatus;
    private PlayerCards opponentPlayerCard;
    private PlayerStatus opponentPlayerStatus;
    private GameObject P1;

    private GameObject P2;

    // Start is called before the first frame update
    void Start()
    {
        GameLoop.ChangePlayer.AddListener(ChangePlayer);
        InGameMenuScript.EffectCardEvent.AddListener(PutEffectCard);
        P1 = GameObject.Find("Player1");
        P2 = GameObject.Find("Player2");
        currentPlayerCard = P1.GetComponent<PlayerCards>();
    }

    private int FindFirstEmptyEffectLocationCurrentPlayer()
    {
        EffectCard[] effectCards = currentPlayerCard.EffectCards;
        bool end = false;
        int i = 0;
        while (i < 4 && !end)
        {
            if (effectCards[i] != null)
            {
                if (effectCards[i].Nom == null)
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

    private int FindSizeInvocationCard(InvocationCard[] invocationCards)
    {
        int cpt = 0;
        for (int i = 0; i < invocationCards.Length; i++)
        {
            if (invocationCards[i] != null)
            {
                if (invocationCards[i].Nom != null)
                {
                    cpt++;
                }
            }
        }

        return cpt;
    }

    private void PutEffectCard(EffectCard effectCard)
    {
        int size = FindFirstEmptyEffectLocationCurrentPlayer();

        if (size < 4)
        {
            bool cancelled = ApplyEffectCard(effectCard, effectCard.GetEffectCardEffect());

            if (!cancelled)
            {
                miniCardMenu.SetActive(false);
                currentPlayerCard.handCards.Remove(effectCard);
            }
        }
        else
        {
            //TODO messageBox to indicate why we cannot put another effectCard
        }
    }

    private bool ApplyEffectCard(EffectCard effectCard, EffectCardEffect effectCardEffect)
    {
        List<Effect> keys = effectCardEffect.Keys;
        List<string> values = effectCardEffect.Values;

        float pvAffected = 0;
        int handCardsNumber = 0;
        bool affectOpponent = false;
        bool cancelled = false;
        string[] sources = null;
        bool canDisplayRemoveOption = false;

        for (int i = 0; i < keys.Count && !cancelled; i++)
        {
            if (!cancelled)
            {
                Effect effect = keys[i];
                string value = values[i];
                switch (effect)
                {
                    case Effect.AffectPV:
                    {
                        if (value == "ALL")
                        {
                            pvAffected = 100.0f;
                        }
                        else
                        {
                            pvAffected = float.Parse(value);
                        }
                    }
                        break;
                    case Effect.AffectOpponent:
                    {
                        affectOpponent = Boolean.Parse(value);
                    }
                        break;
                    case Effect.NumberInvocationCard:
                    {
                        if (affectOpponent)
                        {
                            AffectOpponentNumberInvocation(pvAffected);
                        }
                        else
                        {
                            AffectCurrentNumberInvocation(pvAffected);
                        }
                    }
                        break;
                    case Effect.NumberHandCard:
                    {
                        if (affectOpponent)
                        {
                            AffectOpponentNumberHandCard(pvAffected);
                        }
                        else
                        {
                            AffectCurrentNumberHandCard(pvAffected);
                        }
                    }
                        break;
                    case Effect.DestroyCards:
                    {
                        switch (value)
                        {
                            case "field":
                            {
                                List<Card> fieldCards = new List<Card>();
                                FieldCard fieldCard1 = currentPlayerCard.Field;
                                FieldCard fieldCard2 = opponentPlayerCard.Field;
                                if (fieldCard1.isValid())
                                {
                                    fieldCards.Add(fieldCard1);
                                }

                                if (fieldCard2.isValid())
                                {
                                    fieldCards.Add(fieldCard2);
                                }

                                if (fieldCards.Count > 0)
                                {
                                    if (pvAffected < 0)
                                    {
                                        GameObject message = Instantiate(messageBox);
                                        message.GetComponent<MessageBox>().title = "Choix du terrain à détruire";
                                        message.GetComponent<MessageBox>().description = "";
                                        message.GetComponent<MessageBox>().displayCardsScript.cardslist = fieldCards;
                                        message.GetComponent<MessageBox>().displayCards = true;


                                        message.GetComponent<MessageBox>().positiveAction = () =>
                                        {
                                            FieldCard fieldCard =
                                                (FieldCard) message.GetComponent<MessageBox>().getSelectedCard();
                                            if (fieldCard.Nom == fieldCard1.Nom)
                                            {
                                                currentPlayerCard.YellowTrash.Add(fieldCard);
                                                currentPlayerCard.Field = null;
                                            }
                                            else
                                            {
                                                opponentPlayerCard.YellowTrash.Add(fieldCard);
                                                opponentPlayerCard.Field = null;
                                            }

                                            currentPlayerStatus.changePV(pvAffected);

                                            Destroy(message);
                                        };
                                        message.GetComponent<MessageBox>().negativeAction = () =>
                                        {
                                            cancelled = true;
                                            Destroy(message);
                                        };
                                    }
                                }
                            }
                                break;
                            case "all":
                            {
                                FieldCard fieldCard1 = currentPlayerCard.Field;
                                FieldCard fieldCard2 = opponentPlayerCard.Field;
                                EffectCard[] effectCards1 = currentPlayerCard.EffectCards;
                                EffectCard[] effectCards2 = opponentPlayerCard.EffectCards;
                                InvocationCard[] invocationCards1 = currentPlayerCard.InvocationCards;
                                InvocationCard[] invocationCards2 = opponentPlayerCard.InvocationCards;
                                if (fieldCard1.isValid())
                                {
                                    currentPlayerCard.Field = null;
                                    currentPlayerCard.YellowTrash.Add(fieldCard1);
                                }

                                if (fieldCard2.isValid())
                                {
                                    opponentPlayerCard.Field = null;
                                    opponentPlayerCard.YellowTrash.Add(fieldCard2);
                                }

                                for (int j = 0; j < effectCards1.Length; j++)
                                {
                                    EffectCard card = effectCards1[j];
                                    if (card.isValid())
                                    {
                                        currentPlayerCard.YellowTrash.Add(card);
                                        currentPlayerCard.EffectCards[j] = null;
                                    }
                                }

                                for (int j = 0; j < effectCards2.Length; j++)
                                {
                                    EffectCard card = effectCards2[j];
                                    if (card.isValid())
                                    {
                                        opponentPlayerCard.YellowTrash.Add(card);
                                        opponentPlayerCard.EffectCards[j] = null;
                                    }
                                }

                                for (int j = 0; j < invocationCards1.Length; j++)
                                {
                                    InvocationCard card = invocationCards1[j];
                                    if (card.isValid())
                                    {
                                        currentPlayerCard.YellowTrash.Add(card);
                                        currentPlayerCard.InvocationCards[j] = null;
                                    }
                                }

                                for (int j = 0; j < invocationCards2.Length; j++)
                                {
                                    InvocationCard card = invocationCards2[j];
                                    if (card.isValid())
                                    {
                                        opponentPlayerCard.YellowTrash.Add(card);
                                        opponentPlayerCard.InvocationCards[j] = null;
                                    }
                                }
                            }
                                break;
                            case "invocation":
                            {
                                if (affectOpponent)
                                {
                                    List<Card> invocationOpponentValid = new List<Card>();

                                    foreach (var card in opponentPlayerCard.InvocationCards)
                                    {
                                        if (card.isValid())
                                        {
                                            invocationOpponentValid.Add(card);
                                        }
                                    }

                                    GameObject message = Instantiate(messageBox);
                                    message.GetComponent<MessageBox>().title = "Choix de l'invocation à détruire";
                                    message.GetComponent<MessageBox>().description = "";
                                    message.GetComponent<MessageBox>().displayCardsScript.cardslist =
                                        invocationOpponentValid;
                                    message.GetComponent<MessageBox>().displayCards = true;


                                    message.GetComponent<MessageBox>().positiveAction = () =>
                                    {
                                        InvocationCard invocationCard =
                                            (InvocationCard) message.GetComponent<MessageBox>().getSelectedCard();
                                        if (invocationCard.isValid())
                                        {
                                            opponentPlayerCard.YellowTrash.Add(invocationCard);
                                            for (int j = 0; j < opponentPlayerCard.InvocationCards.Length; j++)
                                            {
                                                InvocationCard currentCard = opponentPlayerCard.InvocationCards[j];
                                                if (currentCard.isValid() && currentCard.Nom == invocationCard.Nom)
                                                {
                                                    opponentPlayerCard.InvocationCards[j] = null;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            cancelled = true;
                                        }

                                        Destroy(message);
                                    };
                                    message.GetComponent<MessageBox>().negativeAction = () =>
                                    {
                                        cancelled = true;
                                        Destroy(message);
                                    };
                                }
                            }
                                break;
                            case "1":
                            {
                                FieldCard fieldCard1 = currentPlayerCard.Field;
                                FieldCard fieldCard2 = opponentPlayerCard.Field;
                                EffectCard[] effectCards1 = currentPlayerCard.EffectCards;
                                EffectCard[] effectCards2 = opponentPlayerCard.EffectCards;
                                InvocationCard[] invocationCards1 = currentPlayerCard.InvocationCards;
                                InvocationCard[] invocationCards2 = opponentPlayerCard.InvocationCards;

                                List<Card> allCardsOnField = new List<Card>();

                                if (fieldCard1.isValid())
                                {
                                    allCardsOnField.Add(fieldCard1);
                                }

                                if (fieldCard2.isValid())
                                {
                                    allCardsOnField.Add(fieldCard2);
                                }

                                foreach (var card in effectCards1)
                                {
                                    if (card.isValid())
                                    {
                                        allCardsOnField.Add(card);
                                    }
                                }

                                foreach (var card in effectCards2)
                                {
                                    if (card.isValid())
                                    {
                                        allCardsOnField.Add(card);
                                    }
                                }

                                foreach (var card in invocationCards1)
                                {
                                    if (card.isValid())
                                    {
                                        allCardsOnField.Add(card);
                                    }
                                }

                                foreach (var card in invocationCards2)
                                {
                                    if (card.isValid())
                                    {
                                        allCardsOnField.Add(card);
                                    }
                                }

                                GameObject message = Instantiate(messageBox);
                                message.GetComponent<MessageBox>().title = "Choix de la carte à détruire";
                                message.GetComponent<MessageBox>().description = "";
                                message.GetComponent<MessageBox>().displayCardsScript.cardslist = allCardsOnField;
                                message.GetComponent<MessageBox>().displayCards = true;


                                message.GetComponent<MessageBox>().positiveAction = () =>
                                {
                                    Card card = message.GetComponent<MessageBox>().getSelectedCard();
                                    if (card.isValid())
                                    {
                                        if (card.Type == "invocation")
                                        {
                                            FindCardInArrayAndSendItToTrash(invocationCards1, invocationCards2, card);
                                        }
                                        else if (card.Type == "effect")
                                        {
                                            FindCardInArrayAndSendItToTrash(effectCards1, effectCards2, card);
                                        }
                                        else if (card.Type == "field")
                                        {
                                            if (fieldCard1.Nom == card.Nom)
                                            {
                                                currentPlayerCard.YellowTrash.Add(card);
                                                currentPlayerCard.Field = null;
                                            }
                                            else if (fieldCard2.Nom == card.Nom)
                                            {
                                                opponentPlayerCard.YellowTrash.Add(fieldCard2);
                                                opponentPlayerCard.Field = null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cancelled = true;
                                    }

                                    Destroy(message);
                                };
                                message.GetComponent<MessageBox>().negativeAction = () =>
                                {
                                    cancelled = true;
                                    Destroy(message);
                                };
                            }
                                break;
                        }
                    }
                        break;
                    case Effect.SacrificeInvocation:
                    {
                        switch (value)
                        {
                            case "true":
                            {
                                InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
                                List<Card> invocationCardsValid = new List<Card>();

                                foreach (var invocationCard in invocationCards)
                                {
                                    if (invocationCard.isValid())
                                    {
                                        invocationCardsValid.Add(invocationCard);
                                    }
                                }

                                if (invocationCardsValid.Count == 0)
                                {
                                    cancelled = true;
                                    // TODO MESSAGE BOX
                                }
                                else
                                {
                                    cancelled = GenerateSacrificeInvocationMessageBox(invocationCardsValid,
                                        invocationCards, pvAffected);
                                }
                            }
                                break;
                            case "5":
                            {
                                InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
                                List<Card> invocationCardsValid = new List<Card>();

                                foreach (var invocationCard in invocationCards)
                                {
                                    if (invocationCard.isValid())
                                    {
                                        if (invocationCard.GetCurrentAttack() >= 5 ||
                                            invocationCard.GetCurrentDefense() >= 5)
                                        {
                                            invocationCardsValid.Add(invocationCard);
                                        }
                                    }
                                }

                                if (invocationCardsValid.Count == 0)
                                {
                                    cancelled = true;
                                    // TODO MESSAGE BOX
                                }
                                else
                                {
                                    cancelled = GenerateSacrificeInvocationMessageBox(invocationCardsValid,
                                        invocationCards, pvAffected);
                                }
                            }
                                break;
                            case "3":
                            {
                                InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
                                List<Card> invocationCardsValid = new List<Card>();

                                foreach (var invocationCard in invocationCards)
                                {
                                    if (invocationCard.isValid())
                                    {
                                        if (invocationCard.GetCurrentAttack() >= 3 ||
                                            invocationCard.GetCurrentDefense() >= 3)
                                        {
                                            invocationCardsValid.Add(invocationCard);
                                        }
                                    }
                                }

                                if (invocationCardsValid.Count == 0)
                                {
                                    cancelled = true;
                                    // TODO MESSAGE BOX
                                }
                                else
                                {
                                    cancelled = GenerateSacrificeInvocationMessageBox(invocationCardsValid,
                                        invocationCards, pvAffected);
                                }
                            }
                                break;
                        }
                    }
                        break;
                    case Effect.SameFamily:
                    {
                        FieldCard fieldCard = currentPlayerCard.Field;
                        if (fieldCard.isValid())
                        {
                            // TODO Ask at the end of a turn if want to keep it
                            string familyField = fieldCard.getFamily();
                            foreach (var card in currentPlayerCard.InvocationCards)
                            {
                                if (card.isValid())
                                {
                                    card.SetCurrentFamily(familyField);
                                }
                            }
                        }
                        else
                        {
                            cancelled = true;
                        }
                    }
                        break;
                    case Effect.CheckTurn:
                    {
                        int size = FindFirstEmptyEffectLocationCurrentPlayer();
                        currentPlayerCard.EffectCards[i] = effectCard;
                    }
                        break;
                    case Effect.ChangeHandCards:
                    {
                        handCardsNumber = int.Parse(value);
                    }
                        break;
                    case Effect.Sources:
                    {
                        sources = value.Split(';');
                    }
                        break;
                    case Effect.HandMax:
                    {
                        List<Card> handCard1 = currentPlayerCard.handCards;
                        List<Card> handCard2 = opponentPlayerCard.handCards;

                        if (handCard1.Count < handCardsNumber)
                        {
                            int size = currentPlayerCard.Deck.Count;
                            while (size > 0 && currentPlayerCard.handCards.Count < handCardsNumber)
                            {
                                Card c = currentPlayerCard.Deck[size - 1];
                                currentPlayerCard.handCards.Add(c);
                                currentPlayerCard.Deck.RemoveAt(size - 1);
                                size--;
                            }
                        }
                        else if (handCard1.Count > handCardsNumber)
                        {
                            //TODO multiple selection on a messageBox
                        }

                        if (handCard2.Count < handCardsNumber)
                        {
                            int size = opponentPlayerCard.Deck.Count;
                            while (size > 0 && opponentPlayerCard.handCards.Count < handCardsNumber)
                            {
                                Card c = opponentPlayerCard.Deck[size - 1];
                                opponentPlayerCard.handCards.Add(c);
                                opponentPlayerCard.Deck.RemoveAt(size - 1);
                                size--;
                            }
                        }
                        else if (handCard2.Count > handCardsNumber)
                        {
                            //TODO multiple selection on a messageBox
                        }
                    }
                        break;
                    case Effect.SeeOpponentHand:
                    {
                        List<Card> handCardOpponent = opponentPlayerCard.handCards;
                        GameObject message = Instantiate(messageBox);
                        message.GetComponent<MessageBox>().title = "Voici les cartes de l'adversaire";
                        message.GetComponent<MessageBox>().description = "";
                        message.GetComponent<MessageBox>().isInformation = true;
                        message.GetComponent<MessageBox>().displayCardsScript.cardslist = handCardOpponent;
                        message.GetComponent<MessageBox>().displayCards = true;


                        message.GetComponent<MessageBox>().okAction = () =>
                        {
                            canDisplayRemoveOption = true;
                            Destroy(message);
                        };
                    }
                        break;
                    case Effect.RemoveCardOption:
                    {
                        while (!canDisplayRemoveOption)
                        {
                        }

                        GameObject message = Instantiate(messageBox);
                        message.GetComponent<MessageBox>().title = "Choix";
                        message.GetComponent<MessageBox>().description =
                            "Veux-tu te défausser d'une carte pour en défausser une à l'adversaire ?";


                        message.GetComponent<MessageBox>().positiveAction = () =>
                        {
                            canDisplayRemoveOption = true;
                            Destroy(message);
                            List<Card> handCardOpponent = opponentPlayerCard.handCards;
                            GameObject message1 = Instantiate(messageBox);
                            message1.GetComponent<MessageBox>().title = "Quel carte veux-tu enlever à l'adversaire ?";
                            message1.GetComponent<MessageBox>().description = "";
                            message1.GetComponent<MessageBox>().displayCardsScript.cardslist = handCardOpponent;
                            message1.GetComponent<MessageBox>().displayCards = true;
                            message1.GetComponent<MessageBox>().positiveAction = () =>
                            {
                                Card cardOpponent = message1.GetComponent<MessageBox>().getSelectedCard();
                                if (cardOpponent.isValid())
                                {
                                    Destroy(message1);
                                    List<Card> handCardPlayer = currentPlayerCard.handCards;
                                    GameObject message2 = Instantiate(messageBox);
                                    message2.GetComponent<MessageBox>().title = "Quel carte veux-tu te défausser?";
                                    message2.GetComponent<MessageBox>().description = "";
                                    message2.GetComponent<MessageBox>().displayCardsScript.cardslist = handCardPlayer;
                                    message2.GetComponent<MessageBox>().displayCards = true;

                                    message2.GetComponent<MessageBox>().positiveAction = () =>
                                    {
                                        Card cardPlayer = message2.GetComponent<MessageBox>().getSelectedCard();
                                        if (cardPlayer.isValid())
                                        {
                                            currentPlayerCard.YellowTrash.Add(cardPlayer);
                                            opponentPlayerCard.YellowTrash.Add(cardOpponent);
                                            currentPlayerCard.handCards.Remove(cardPlayer);
                                            opponentPlayerCard.handCards.Remove(cardOpponent);
                                        }
                                        else
                                        {
                                            cancelled = true;
                                        }

                                        Destroy(message2);
                                    };

                                    message2.GetComponent<MessageBox>().negativeAction = () =>
                                    {
                                        cancelled = true;
                                        Destroy(message2);
                                    };
                                }
                                else
                                {
                                    cancelled = true;
                                }
                            };

                            message1.GetComponent<MessageBox>().negativeAction = () =>
                            {
                                cancelled = true;
                                Destroy(message1);
                            };
                        };

                        message.GetComponent<MessageBox>().negativeAction = () =>
                        {
                            cancelled = true;
                            Destroy(message);
                        };
                    }
                        break;
                    case Effect.RemoveHand:
                    {
                        List<Card> handCardPlayer = currentPlayerCard.handCards;
                        GameObject message = Instantiate(messageBox);
                        message.GetComponent<MessageBox>().title = "Quel carte veux-tu te défausser?";
                        message.GetComponent<MessageBox>().description = "";
                        message.GetComponent<MessageBox>().displayCardsScript.cardslist = handCardPlayer;
                        message.GetComponent<MessageBox>().displayCards = true;

                        message.GetComponent<MessageBox>().positiveAction = () =>
                        {
                            Card card = message.GetComponent<MessageBox>().getSelectedCard();
                            if (card.isValid())
                            {
                                currentPlayerCard.YellowTrash.Add(card);
                                currentPlayerCard.handCards.Remove(card);
                            }
                            else
                            {
                                cancelled = true;
                            }

                            Destroy(message);
                        };

                        message.GetComponent<MessageBox>().negativeAction = () =>
                        {
                            cancelled = true;
                            Destroy(message);
                        };
                    }
                        break;
                    case Effect.RemoveDeck:
                    {
                        int size = currentPlayerCard.Deck.Count;
                        if (size > 0)
                        {
                            Card c = currentPlayerCard.Deck[size - 1];
                            currentPlayerCard.YellowTrash.Add(c);
                            currentPlayerCard.Deck.RemoveAt(size - 1);
                        }
                        else
                        {
                            cancelled = true;
                        }
                    }
                        break;
                    case Effect.SpecialInvocation:
                    {
                        int place = FindFirstEmptyInvocationLocationCurrentPlayer();
                        if (place < 4)
                        {
                            List<Card> yellowTrash = currentPlayerCard.YellowTrash;
                            InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;

                            List<Card> invocationFromYellowTrash = new List<Card>();
                            foreach (var card in yellowTrash)
                            {
                                if (card.Type == "invocation")
                                {
                                    invocationFromYellowTrash.Add(card);
                                }
                            }

                            if (invocationFromYellowTrash.Count == 0)
                            {
                                cancelled = true;
                                // TODO MessageBox
                            }
                            else
                            {
                                GameObject message = Instantiate(messageBox);
                                message.GetComponent<MessageBox>().title = "Quel carte veux-tu invoquer spécialement ?";
                                message.GetComponent<MessageBox>().description = "";
                                message.GetComponent<MessageBox>().displayCardsScript.cardslist =
                                    invocationFromYellowTrash;
                                message.GetComponent<MessageBox>().displayCards = true;

                                message.GetComponent<MessageBox>().positiveAction = () =>
                                {
                                    InvocationCard card =
                                        (InvocationCard) message.GetComponent<MessageBox>().getSelectedCard();
                                    if (card.isValid())
                                    {
                                        currentPlayerCard.YellowTrash.Remove(card);
                                        currentPlayerCard.InvocationCards[place] = card;
                                    }
                                    else
                                    {
                                        cancelled = true;
                                    }

                                    Destroy(message);
                                };

                                message.GetComponent<MessageBox>().negativeAction = () =>
                                {
                                    cancelled = true;
                                    Destroy(message);
                                };
                            }
                        }
                        else
                        {
                            cancelled = true;
                            // TODO MessageBox
                        }
                    }
                        break;
                    case Effect.DividInvocation:
                    {
                        InvocationCard[] opponentInvocationCard = opponentPlayerCard.InvocationCards;
                        foreach (var card in opponentInvocationCard)
                        {
                            if (card.isValid())
                            {
                                // TODO Keep previous bonus
                                float newBonusDefense = card.getBonusDefense() - card.GetCurrentDefense() / 2;
                                card.setBonusDefense(newBonusDefense);
                            }
                        }
                    }
                        break;
                    case Effect.Duration:
                    {
                        //TODO Do something with it
                    }
                        break;
                    case Effect.Combine:
                    {
                    }
                        break;
                    case Effect.RevertStat:
                    {
                        InvocationCard[] invocationCards1 = currentPlayerCard.InvocationCards;
                        InvocationCard[] invocationCards2 = opponentPlayerCard.InvocationCards;

                        foreach (var card in invocationCards1)
                        {
                            if (card.isValid())
                            {
                                // TODO Keep previous bonus
                                float newBonusAttack = card.GetCurrentDefense() - card.GetAttack();
                                float newBonusDefense = card.GetCurrentAttack() - card.GetDefense();
                                card.setBonusDefense(newBonusDefense);
                                card.setBonusAttack(newBonusAttack);
                            }
                        }

                    }
                        break;
                    case Effect.TakeControl:
                    {
                    }
                        break;
                    case Effect.NumberAttacks:
                    {
                    }
                        break;
                    case Effect.SkipAttack:
                    {
                    }
                        break;
                    case Effect.SeeCards:
                    {
                    }
                        break;
                    case Effect.ChangeOrder:
                    {
                    }
                        break;
                    case Effect.AttackDirectly:
                    {
                    }
                        break;
                    case Effect.ProtectAttack:
                    {
                    }
                        break;
                    case Effect.SkipFieldsEffect:
                    {
                    }
                        break;
                    case Effect.ChangeField:
                    {
                    }
                        break;
                    case Effect.SkipContre:
                    {
                    }
                        break;
                }
            }
        }

        //TODO Add effectCard to yellow trash if necessary
        return cancelled;
    }

    private bool GenerateSacrificeInvocationMessageBox(List<Card> invocationCardsValid,
        InvocationCard[] invocationCards,
        float pvAffected)
    {
        bool cancelled = false;
        GameObject message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Choix de l'invocation à sacrifier";
        message.GetComponent<MessageBox>().description = "";
        message.GetComponent<MessageBox>().displayCardsScript.cardslist = invocationCardsValid;
        message.GetComponent<MessageBox>().displayCards = true;


        message.GetComponent<MessageBox>().positiveAction = () =>
        {
            Card card = message.GetComponent<MessageBox>().getSelectedCard();
            if (card.isValid())
            {
                for (int j = 0; j < invocationCards.Length; j++)
                {
                    InvocationCard invocationCard = invocationCards[j];
                    if (invocationCard.isValid())
                    {
                        if (invocationCard.Nom == card.Nom)
                        {
                            currentPlayerCard.YellowTrash.Add(card);
                            currentPlayerCard.InvocationCards[j] = null;

                            if (pvAffected > 0)
                            {
                                currentPlayerStatus.changePV(pvAffected);
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                cancelled = true;
            }

            Destroy(message);
        };
        message.GetComponent<MessageBox>().negativeAction = () =>
        {
            cancelled = true;
            Destroy(message);
        };
        return cancelled;
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

    private void FindCardInArrayAndSendItToTrash(Card[] cards1, Card[] cards2, Card card)
    {
        bool found = false;
        int j = 0;
        while (j < cards2.Length && !found)
        {
            Card card2 = cards2[j];
            if (card2.isValid())
            {
                if (card2.Nom == card.Nom)
                {
                    found = true;
                    opponentPlayerCard.YellowTrash.Add(card);
                    switch (card.Type)
                    {
                        case "invocation":
                        {
                            opponentPlayerCard.InvocationCards[j] = null;
                        }
                            break;
                        case "effect":
                        {
                            opponentPlayerCard.EffectCards[j] = null;
                        }
                            break;
                    }
                }
            }

            j++;
        }

        if (!found)
        {
            j = 0;
            while (j < cards1.Length && !found)
            {
                Card card1 = cards1[j];
                if (card1.isValid())
                {
                    if (card1.Nom == card.Nom)
                    {
                        found = true;
                        currentPlayerCard.YellowTrash.Add(card);
                        switch (card.Type)
                        {
                            case "invocation":
                            {
                                currentPlayerCard.InvocationCards[j] = null;
                            }
                                break;
                            case "effect":
                            {
                                currentPlayerCard.EffectCards[j] = null;
                            }
                                break;
                        }
                    }
                }

                j++;
            }
        }
    }

    private void AffectOpponentNumberInvocation(float pvAffected)
    {
        InvocationCard[] invocationCards = opponentPlayerCard.InvocationCards;
        int size = FindSizeInvocationCard(invocationCards);
        float dammage = size * pvAffected;
        opponentPlayerStatus.changePV(dammage);
    }

    private void AffectCurrentNumberInvocation(float pvAffected)
    {
        InvocationCard[] invocationCards = currentPlayerCard.InvocationCards;
        int size = FindSizeInvocationCard(invocationCards);
        float dammage = size * pvAffected;
        currentPlayerStatus.changePV(dammage);
    }

    private void AffectOpponentNumberHandCard(float pvAffected)
    {
        List<Card> handCards = opponentPlayerCard.handCards;
        int size = handCards.Count;
        float dammage = size * pvAffected;
        opponentPlayerStatus.changePV(dammage);
    }

    private void AffectCurrentNumberHandCard(float pvAffected)
    {
        List<Card> handCards = currentPlayerCard.handCards;
        int size = handCards.Count;
        float dammage = size * pvAffected;
        currentPlayerStatus.changePV(dammage);
    }

    void ChangePlayer()
    {
        if (GameLoop.isP1Turn)
        {
            currentPlayerCard = P1.GetComponent<PlayerCards>();
            currentPlayerStatus = P1.GetComponent<PlayerStatus>();
            opponentPlayerCard = P2.GetComponent<PlayerCards>();
            opponentPlayerStatus = P2.GetComponent<PlayerStatus>();
        }
        else
        {
            currentPlayerCard = P2.GetComponent<PlayerCards>();
            currentPlayerStatus = P2.GetComponent<PlayerStatus>();
            opponentPlayerCard = P1.GetComponent<PlayerCards>();
            opponentPlayerStatus = P1.GetComponent<PlayerStatus>();
        }
    }
}